// Generates a Markdown table of the public API of Rhino.Scripting.
// The static members of RhinoScriptSyntax get one table per source file Src/Scripting_*.fs, so they are grouped by topic as in the sources.
// The other public types, modules and type extensions follow after them.
// Each row shows a name, its signature with the default values of optional parameters, and the first sentence of its summary.
//
// Usage, from the repo root:
//     dotnet fsi API-Table/ApiTable.fsx [output.md]
// The default output is API-Table/ApiTable.md.
//
// The script type checks the files listed in ForEditing.fsproj with FSharp.Compiler.Service.
// So it needs no build and always reflects the current sources.
// It checks them like the net8.0 build of ForPublishing.fsproj: without the RH7 define and against the packages that build references.
// The packages come from the local NuGet cache, so restore ForPublishing.fsproj once if one is missing.
// The Rhino 7 and Rhino 8 builds differ only inside method bodies, so their API is the same.

#r "nuget: FSharp.Compiler.Service, 43.12.401" // needs to match the runtime FSharp.Core dependency!

open System
open System.IO
open System.Net
open System.Text
open System.Text.RegularExpressions
open System.Xml.Linq
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

do
    let compilerAssembly = typeof<FSharpChecker>.Assembly
    let requiredCore =
        compilerAssembly.GetReferencedAssemblies()
        |> Array.find (fun reference -> reference.Name = "FSharp.Core")
    let runtimeCore = typeof<unit>.Assembly.GetName()
    if runtimeCore.Version <> requiredCore.Version then
        failwith $"FSharp.Compiler.Service {compilerAssembly.GetName().Version} requires FSharp.Core {requiredCore.Version}, but F# Interactive loaded {runtimeCore.Version}. Use a matching .NET SDK or update the FSharp.Compiler.Service package reference."

// #region Type checking

let root = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, ".."))
let editingProject = Path.Combine(root, "ForEditing.fsproj")
let publishingProject = Path.Combine(root, "ForPublishing.fsproj")

/// The target framework of ForPublishing.fsproj whose package references are used.
let targetFramework = "net8.0"

let outFile =
    match fsi.CommandLineArgs |> Array.tail with
    | [| path |] -> Path.GetFullPath path
    | _ -> Path.Combine(__SOURCE_DIRECTORY__, "ApiTable.md")

let attr (e: XElement) (name: string) =
    match e.Attribute(XName.Get name) with
    | null -> ""
    | a -> a.Value

/// The source files in compile order.
let sourceFiles =
    XDocument.Load(editingProject).Descendants(XName.Get "Compile")
    |> Seq.map (fun e -> Path.GetFullPath(Path.Combine(root, attr e "Include")))
    |> Array.ofSeq

/// TRUE if an MSBuild condition like "'$(TargetFramework)' != 'net48'" holds for 'targetFramework'.
let conditionHolds (condition: string) =
    if condition.Trim() = "" then true
    else
        let m = Regex.Match(condition.Replace("$(TargetFramework)", targetFramework), @"^\s*'([^']*)'\s*(==|!=)\s*'([^']*)'\s*$")
        if not m.Success then failwith $"Cannot evaluate the condition {condition} in {publishingProject}"
        (m.Groups.[1].Value = m.Groups.[3].Value) = (m.Groups.[2].Value = "==")

let nugetRoot =
    match Environment.GetEnvironmentVariable "NUGET_PACKAGES" with
    | null | "" -> Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget", "packages")
    | path -> path

/// How well a framework name of a package fits 'targetFramework', or None if it does not fit.
/// The name is a lib folder like "net7.0" or a nuspec group like ".NETStandard2.0", "" for a nuspec without groups.
/// .NET versions up to 'targetFramework' rank above all .NET Standard versions, the higher the version the better.
let rank (framework: string) =
    let name = framework.ToLowerInvariant().Replace(".netframework", "net").Replace(".netstandard", "netstandard")
    let m = Regex.Match(name, @"^net(standard)?(\d+\.\d+)$")
    if name = "" then Some(-1, Version())
    elif not m.Success then None
    else
        let v = Version m.Groups.[2].Value
        if m.Groups.[1].Success then Some(0, v)
        elif v.Major >= 5 && v <= Version(targetFramework.Substring 3) then Some(1, v)
        else None

/// The item whose framework name fits 'targetFramework' best.
let best (frameworkOf: 'T -> string) (items: seq<'T>) =
    items
    |> Seq.choose (fun i -> rank (frameworkOf i) |> Option.map (fun r -> r, i))
    |> Seq.sortByDescending fst
    |> Seq.tryHead
    |> Option.map snd

/// The assemblies of a package and of its dependencies for 'targetFramework', from the local NuGet cache.
/// FSharp.Core is left out, the one of F# Interactive is used.
let rec assembliesOf (visited: Collections.Generic.HashSet<string>) (id: string) (version: string) : string list =
    if id = "FSharp.Core" || not (visited.Add(id.ToLowerInvariant())) then []
    else
        let dir = Path.Combine(nugetRoot, id.ToLowerInvariant(), version)
        if not (Directory.Exists dir) then
            failwith $"Package {id} {version} not found in {nugetRoot}. Run 'dotnet restore ForPublishing.fsproj' first."
        let lib = Path.Combine(dir, "lib")
        let assemblies =
            if not (Directory.Exists lib) then [] // a build tool like Ionide.KeepAChangelog.Tasks
            else
                match Directory.GetDirectories lib |> best Path.GetFileName with
                | Some d -> Directory.GetFiles(d, "*.dll") |> List.ofArray
                | None -> []
        let dependencies =
            let nuspec = XDocument.Load(Directory.GetFiles(dir, "*.nuspec") |> Array.head)
            let named (name: string) (e: XElement) = e.Elements() |> Seq.filter (fun c -> c.Name.LocalName = name)
            nuspec.Descendants()
            |> Seq.filter (fun e -> e.Name.LocalName = "dependencies")
            |> Seq.collect (fun d -> if Seq.isEmpty (named "group" d) then Seq.singleton d else named "group" d)
            |> best (fun g -> attr g "targetFramework")
            |> Option.map (fun g -> [ for d in named "dependency" g -> attr d "id", (attr d "version").Trim('[', '(').Split(',', ']', ')').[0].Trim() ])
            |> Option.defaultValue []
        assemblies @ (dependencies |> List.collect (fun (i, v) -> assembliesOf visited i v))

/// The assemblies of the packages that ForPublishing.fsproj references for 'targetFramework', and of their dependencies.
let packageAssemblies =
    let visited = Collections.Generic.HashSet<string>()
    [ for e in XDocument.Load(publishingProject).Descendants(XName.Get "PackageReference") do
        if conditionHolds (attr e "Condition") then
            yield! assembliesOf visited (attr e "Include") (attr e "Version") ]
    // RhinoCommon ships its own Eto.dll. Like NuGet, keep only the highest version of an assembly.
    |> List.groupBy (fun p -> Path.GetFileName(p).ToLowerInvariant())
    |> List.map (fun (_, paths) -> paths |> List.maxBy (fun p -> Reflection.AssemblyName.GetAssemblyName(p).Version))

let checker = FSharpChecker.Create()

let projectOptions =
    // borrow the .NET references that fsi uses for scripts
    let scriptOptions, _ =
        checker.GetProjectOptionsFromScript(Path.Combine(root, "ApiTable.fsx"), SourceText.ofString "", assumeDotNetFramework = false)
        |> Async.RunSynchronously
    let frameworkReferences = scriptOptions.OtherOptions |> Array.filter (fun o -> o.StartsWith "-r:")
    let packageReferences = packageAssemblies |> List.map (fun p -> $"-r:{p}") |> Array.ofList
    { scriptOptions with
        ProjectFileName = editingProject
        SourceFiles = sourceFiles
        OtherOptions = Array.concat [ frameworkReferences; packageReferences; [| "--noframework"; "--targetprofile:netcore" |] ] }

printfn $"Type checking {sourceFiles.Length} files of {editingProject} ..."
let results = checker.ParseAndCheckProject projectOptions |> Async.RunSynchronously
let errors = results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
if errors.Length > 0 then
    for e in errors do eprintfn $"{e}"
    failwith $"Type checking Rhino.Scripting failed with {errors.Length} errors."

// #endregion
// #region Formatting of signatures

let ctx = FSharpDisplayContext.Empty.WithShortTypeNames true

let fmt (t: FSharpType) = t.Format ctx

let isUnit (t: FSharpType) = fmt t = "unit"

/// Function and tuple types need parentheses when they are a single argument.
let fmtArg (t: FSharpType) =
    if t.IsFunctionType || t.IsTupleType || t.IsStructTupleType then $"({fmt t})" else fmt t

/// Function types need parentheses when they are returned by a member.
let fmtReturn (t: FSharpType) =
    if t.IsFunctionType then $"({fmt t})" else fmt t

/// The full name of an attribute type, also when it is used through an abbreviation like OPT or DEF.
let rec attributeName (e: FSharpEntity) =
    if e.IsFSharpAbbreviation && e.AbbreviatedType.HasTypeDefinition then attributeName e.AbbreviatedType.TypeDefinition
    else defaultArg e.TryFullName e.DisplayName

/// A default parameter value as an F# literal.
let literal (value: obj) =
    match value with
    | null -> "null"
    | :? string as s -> "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""
    | :? bool as b -> if b then "true" else "false"
    | :? float as f -> sprintf "%A" f
    | :? float32 as f -> sprintf "%A" f
    | :? byte as b -> $"{b}uy"
    | v -> Convert.ToString(v, Globalization.CultureInfo.InvariantCulture)

let sourceLines = Collections.Generic.Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)

/// The default value of an optional parameter as it is written in the source, e.g. "Point3d()" for '[<OPT;DEF(Point3d())>]pt:Point3d'.
/// This is needed because FCS reports no attributes on the parameters of intrinsic members, and null for a struct default like Point3d().
/// A type annotation like in 'DEF(null:string)' or 'DEF(Drawing.Color():Drawing.Color)' is left out.
let defaultInSource (p: FSharpParameter) =
    let r = p.DeclarationLocation
    let file = Path.GetFullPath r.FileName
    if not (File.Exists file) then None
    else
        let lines =
            match sourceLines.TryGetValue file with
            | true, ls -> ls
            | _ ->
                let ls = File.ReadAllLines file
                sourceLines.[file] <- ls
                ls
        // the attributes right before the parameter name
        let before = lines.[r.StartLine - 1].Substring(0, r.StartColumn)
        let start = before.LastIndexOf "[<"
        if start < 0 || not (Regex.IsMatch(before, @">\]\s*$")) then None
        else
            let m = Regex.Match(before.Substring start, @"\b(?:DEF|DefaultParameterValue)\((.*?)\)\s*(?:;|>\])")
            if not m.Success then None
            else
                let value = m.Groups.[1].Value.Trim()
                let typed = Regex.Match(value, @"^((?:[^""():]|""[^""]*""|\([^()]*\))+?)\s*:")
                Some(if typed.Success then typed.Groups.[1].Value else value)

/// A parameter like 'x: float'. An optional one is shown as '?x: float = 0.0' with its default value.
let fmtParam (p: FSharpParameter) =
    match p.Name with
    | Some n when n <> "" ->
        let attributes = p.Attributes |> Seq.map (fun a -> attributeName a.AttributeType, a) |> List.ofSeq
        let optional = p.IsOptionalArg || attributes |> List.exists (fun (name, _) -> name = "System.Runtime.InteropServices.OptionalAttribute")
        let defaultValue =
            defaultInSource p
            |> Option.orElse (
                attributes |> List.tryPick (fun (name, a) ->
                    if name = "System.Runtime.InteropServices.DefaultParameterValueAttribute" && a.ConstructorArguments.Count = 1 then
                        Some(literal (snd a.ConstructorArguments.[0]))
                    else None))
        let n = if optional then $"?{n}" else n
        let n = if p.IsParamArrayArg then $"[<ParamArray>] {n}" else n
        let value = match defaultValue with Some v -> $" = {v}" | None -> ""
        $"{n}: {fmtArg p.Type}{value}"
    | _ -> fmtArg p.Type

/// A group of tupled parameters, e.g. 'x: float * y: float'.
let fmtGroup (group: Collections.Generic.IList<FSharpParameter>) =
    if group.Count = 0 || (group.Count = 1 && isUnit group.[0].Type) then "unit"
    else group |> Seq.map fmtParam |> String.concat " * "

/// The signature of a method, constructor, function or value, without its name.
let fmtFunction (m: FSharpMemberOrFunctionOrValue) =
    if m.CurriedParameterGroups.Count = 0 then
        fmt m.FullType
    else
        [ for g in m.CurriedParameterGroups do yield fmtGroup g
          yield fmtReturn m.ReturnParameter.Type ]
        |> String.concat " -> "

/// The signature of a property from its getter, e.g. 'float with get, set'.
let fmtProperty (getter: FSharpMemberOrFunctionOrValue) (hasSetter: bool) =
    let index = getter.CurriedParameterGroups |> Seq.concat |> Seq.filter (fun p -> not (isUnit p.Type)) |> List.ofSeq
    let access = if hasSetter then " with get, set" else ""
    let ty = fmtReturn getter.ReturnParameter.Type
    if index.IsEmpty then ty + access
    else (index |> List.map fmtParam |> String.concat " * ") + " -> " + ty + access

/// The signature of a property that only has a setter.
let fmtSetterOnly (setter: FSharpMemberOrFunctionOrValue) =
    let ps = setter.CurriedParameterGroups |> Seq.concat |> List.ofSeq
    let index = ps |> List.take (ps.Length - 1)
    let ty = fmt (List.last ps).Type
    if index.IsEmpty then $"{ty} with set"
    else (index |> List.map fmtParam |> String.concat " * ") + $" -> {ty} with set"

/// Operators are shown as '( + )', all other names as they are written in F#.
let displayName (m: FSharpMemberOrFunctionOrValue) =
    if m.DisplayName.StartsWith "(" then $"( {m.DisplayNameCore} )" else m.DisplayName

/// The summary of an XML doc comment on one line, with <c> and <code> as code spans, or "" if there is none.
let summaryOf (doc: FSharpXmlDoc) =
    match doc with
    | FSharpXmlDoc.FromXmlText t ->
        let xml = String.concat "\n" (t.GetElaboratedXmlLines())
        let m = Regex.Match(xml, @"<summary>(.*?)</summary>", RegexOptions.Singleline)
        if not m.Success then ""
        else
            let text = Regex.Replace(m.Groups.[1].Value, @"<(c|code)>(.*?)</\1>", (fun c -> "`" + c.Groups.[2].Value.Trim() + "`"), RegexOptions.Singleline)
            Regex.Replace(WebUtility.HtmlDecode text, @"\s+", " ").Trim()
    | _ -> ""

/// The first sentence of a summary: up to a period followed by an uppercase letter or the end, but not 'e.g.' or 'i.e.'.
let firstSentence (s: string) =
    let m = Regex.Match(s, @"^.*?(?<!\b(e\.g|i\.e))[.!?](?=\s+[A-Z(]|\s*$)")
    if m.Success then m.Value else s

/// A code span that may contain backticks, with '|' escaped for tables.
let code (s: string) =
    if s = "" then ""
    else
        let longest = Regex.Matches(s, "`+") |> Seq.map (fun m -> m.Length) |> Seq.fold max 0
        let fence = String('`', longest + 1)
        let pad = if s.StartsWith "`" || s.EndsWith "`" then " " else ""
        (fence + pad + s + pad + fence).Replace("|", "\\|")

/// Markdown for a summary. Outside of code spans, characters that Markdown or HTML would interpret are escaped.
/// '|' is escaped everywhere, for tables.
let text (s: string) =
    Regex.Split(s, "(`[^`]*`)")
    |> Array.map (fun part ->
        if part.StartsWith "`" then part
        else part.Replace("\\", "\\\\").Replace("*", "\\*").Replace("_", "\\_").Replace("<", "&lt;").Replace(">", "&gt;"))
    |> String.concat ""
    |> fun s -> s.Replace("|", "\\|")

// #endregion
// #region Collecting the API

let fileIndex (file: string) =
    let full = Path.GetFullPath file
    sourceFiles |> Array.tryFindIndex (fun f -> String.Equals(f, full, StringComparison.OrdinalIgnoreCase))

let position (r: range) = defaultArg (fileIndex r.FileName) Int32.MaxValue, r.StartLine

type Item = {
    Rank: int         // 0 for constructors, 1 for fields and union cases in declaration order, 2 for members in alphabetical order
    IsStatic: bool
    Name: string      // shown in the name column
    Bare: string      // the name as declared, to sort by
    Signature: string
    Summary: string
    Order: int * int  // file index and line, to sort overloads
    }

type Section = {
    Title: string
    Note: string      // Markdown below the title: the summary of a type or module, or the source file of RhinoScriptSyntax members
    Position: int * int
    Prefix: string    // qualifies static members, "" for RhinoScriptSyntax
    SkipNames: Set<string> // compiler generated union members
    Fields: ResizeArray<Item>
    Members: ResizeArray<FSharpMemberOrFunctionOrValue>
    }

let sections = Collections.Generic.Dictionary<string, Section>()

let addSection key title note pos prefix skip =
    match sections.TryGetValue key with
    | true, s -> s
    | _ ->
        let s = { Title = title; Note = note; Position = pos; Prefix = prefix; SkipNames = skip; Fields = ResizeArray(); Members = ResizeArray() }
        sections.[key] <- s
        s

let fullName (e: FSharpEntity) = defaultArg e.TryFullName e.CompiledName

let mainType = "Rhino.Scripting.RhinoScriptSyntax"

/// The members of RhinoScriptSyntax and type extensions, sorted into sections once all types are known.
let members = ResizeArray<FSharpMemberOrFunctionOrValue>()

let fieldItem (name: string) (signature: string) (doc: FSharpXmlDoc) (r: range) =
    { Rank = 1; IsStatic = false; Name = name; Bare = name; Signature = signature; Summary = firstSentence (summaryOf doc); Order = position r }

let rec walk (parentPath: string list) (e: FSharpEntity) =
    if e.Accessibility.IsPublic && not e.IsMeasure && not e.IsFSharpAbbreviation then
        let path = parentPath @ [ e.DisplayName ]
        let title = (if e.IsFSharpModule then "module " else "") + String.Join(".", path)
        let skip =
            if e.IsFSharpUnion then e.UnionCases |> Seq.map (fun c -> "Is" + c.Name) |> Seq.append [ "Tag" ] |> set
            else Set.empty
        let s = addSection (fullName e) title (text (summaryOf e.XmlDoc)) (position e.DeclarationLocation) e.DisplayName skip
        let typeName = e.DisplayName

        if e.IsEnum then
            for f in e.FSharpFields do
                if f.IsStatic && f.LiteralValue.IsSome then
                    s.Fields.Add(fieldItem f.DisplayName $"{typeName} = {f.LiteralValue.Value}" f.XmlDoc f.DeclarationLocation)
        elif e.IsFSharpUnion then
            for c in e.UnionCases do
                if c.Accessibility.IsPublic then
                    let signature =
                        if c.Fields.Count = 0 then typeName
                        else
                            let fields =
                                c.Fields
                                |> Seq.map (fun f -> if f.IsNameGenerated then fmtArg f.FieldType else $"{f.Name}: {fmtArg f.FieldType}")
                                |> String.concat " * "
                            $"{fields} -> {typeName}"
                    s.Fields.Add(fieldItem c.DisplayName signature c.XmlDoc c.DeclarationLocation)
        else
            for f in e.FSharpFields do
                if f.Accessibility.IsPublic && not f.IsStatic && not f.IsCompilerGenerated then
                    let ty = fmt f.FieldType
                    let field = fieldItem f.DisplayName (if f.IsMutable then $"mutable {ty}" else ty) f.XmlDoc f.DeclarationLocation
                    s.Fields.Add { field with Name = $".{f.DisplayName}" }

        for n in e.NestedEntities do
            if n.Accessibility.IsPublic && n.IsFSharpAbbreviation then
                s.Fields.Add(fieldItem n.DisplayName $"= {fmt n.AbbreviatedType}" n.XmlDoc n.DeclarationLocation)

        for m in e.MembersFunctionsAndValues do
            if m.IsExtensionMember || fullName e = mainType then members.Add m
            else s.Members.Add m

        for n in e.NestedEntities do
            walk path n

for e in results.AssemblySignature.Entities do
    let nsPath =
        match e.Namespace with
        | Some ns when ns.StartsWith "Rhino.Scripting." -> ns.Substring("Rhino.Scripting.".Length).Split '.' |> List.ofArray
        | _ -> []
    walk nsPath e

/// The topic of a source file, e.g. "Curve" for Src/Scripting_Curve.fs.
let topicOf (file: string) = Path.GetFileNameWithoutExtension(file).Replace("Scripting_", "")

for m in members do
    match m.ApparentEnclosingEntity with
    | Some target when fullName target = mainType ->
        // one section per source file
        let file = Path.GetFullPath m.DeclarationLocation.FileName
        let fromRoot = Path.GetRelativePath(root, file).Replace('\\', '/')
        let fromOutput = Path.GetRelativePath(Path.GetDirectoryName outFile, file).Replace('\\', '/')
        let note = $"From [{fromRoot}]({fromOutput})."
        let s = addSection file (topicOf file) note (position m.DeclarationLocation) "" Set.empty
        s.Members.Add m
    | Some target ->
        let key = fullName target
        match sections.TryGetValue key with
        | true, s -> s.Members.Add m
        | _ when key.StartsWith "Rhino.Scripting." -> () // extends a type that is not public
        | _ ->
            // extends a type outside of Rhino.Scripting, e.g. Point3d or ResizeArray
            let name =
                match target.DisplayName with
                | "List" -> "ResizeArray"
                | "[]" -> "Array"
                | n -> n
            let s = addSection key $"{name} extensions" "" (position m.DeclarationLocation) name Set.empty
            s.Members.Add m
    | None -> ()

/// Converts the members of a section into rows: one per method, function, value, constructor and property.
let itemsOf (s: Section) : Item list =
    let visible =
        s.Members
        |> Seq.filter (fun m ->
            m.Accessibility.IsPublic
            && not m.IsCompilerGenerated
            && not m.IsExplicitInterfaceImplementation
            && not m.IsEvent
            && not m.IsProperty // properties are listed through their getter and setter methods
            && not (s.SkipNames.Contains m.DisplayName))
        |> List.ofSeq
    let names = visible |> List.map (fun m -> m.DisplayName) |> Set.ofList
    // Keep geometry factories beside their document-adding counterparts in the main API tables.
    let nameOrder (i: Item) =
        if i.Rank <> 2 then "", 0
        elif s.Prefix = "" && i.Bare.StartsWith("Make", StringComparison.Ordinal) then
            let addName = "Add" + i.Bare.Substring(4)
            if names.Contains addName then addName.ToLowerInvariant(), 1
            else i.Bare.ToLowerInvariant(), 0
        else i.Bare.ToLowerInvariant(), 0
    let indexCount (m: FSharpMemberOrFunctionOrValue) =
        m.CurriedParameterGroups |> Seq.concat |> Seq.filter (fun p -> not (isUnit p.Type)) |> Seq.length
    let isAccessorPair (getter: FSharpMemberOrFunctionOrValue) (setter: FSharpMemberOrFunctionOrValue) =
        getter.IsPropertyGetterMethod && setter.IsPropertySetterMethod
        && getter.DisplayName = setter.DisplayName
        && getter.IsInstanceMember = setter.IsInstanceMember
        && indexCount getter = indexCount setter - 1
    /// The summary of a member, for an accessor the one of its property.
    let summary (m: FSharpMemberOrFunctionOrValue) =
        let own = summaryOf m.XmlDoc
        if own <> "" || not (m.IsPropertyGetterMethod || m.IsPropertySetterMethod) then firstSentence own
        else
            s.Members
            |> Seq.tryFind (fun p -> p.IsProperty && p.DisplayName = m.DisplayName && p.IsInstanceMember = m.IsInstanceMember)
            |> Option.map (fun p -> firstSentence (summaryOf p.XmlDoc))
            |> Option.defaultValue ""
    [ yield! s.Fields
      for m in visible do
        let item rank signature =
            let name =
                if m.IsConstructor then "new"
                elif m.IsInstanceMember then $".{displayName m}"
                elif s.Prefix = "" || m.DisplayName.StartsWith "(" then displayName m
                else $"{s.Prefix}.{displayName m}"
            { Rank = rank; IsStatic = not m.IsInstanceMember; Name = name; Bare = m.DisplayName; Signature = signature; Summary = summary m; Order = position m.DeclarationLocation }
        if m.IsConstructor then
            yield item 0 (fmtFunction m)
        elif m.IsPropertyGetterMethod then
            yield item 2 (fmtProperty m (visible |> List.exists (isAccessorPair m)))
        elif m.IsPropertySetterMethod then
            if not (visible |> List.exists (fun g -> isAccessorPair g m)) then
                yield item 2 (fmtSetterOnly m)
        else
            yield item 2 (fmtFunction m)
    ]
    // Constructors, then fields in declaration order, then member families by name.
    // Add overloads precede matching Make overloads; overloads of each name retain source order.
    |> List.sortBy (fun i ->
        let name, factoryOrder = nameOrder i
        i.Rank, name, factoryOrder, i.IsStatic, i.Order)

// #endregion
// #region Markdown

/// The anchor GitHub generates for a heading.
let anchor (title: string) =
    let lower = title.Trim().ToLowerInvariant()
    let kept = lower |> String.filter (fun c -> Char.IsLetterOrDigit c || c = ' ' || c = '-' || c = '_')
    kept.Replace(' ', '-')

let plural (n: int) (word: string) =
    if n = 1 then $"1 {word}" else $"{n} {word}s"

let tables =
    [ for s in sections.Values do
        let items = itemsOf s
        if not items.IsEmpty then yield s, items ]

/// The members of RhinoScriptSyntax, the header first and then the other files by topic.
let mainTables, otherTables =
    let isMain (s: Section) = s.Prefix = ""
    let main, other = tables |> List.partition (fst >> isMain)
    main |> List.sortBy (fun (s, _) -> (if s.Title = "Header" then 0 else 1), s.Title),
    other |> List.sortBy (fun (s, _) -> s.Position)

let mainCount = mainTables |> List.sumBy (snd >> List.length)
let mainNames = mainTables |> List.collect snd |> List.distinctBy (fun i -> i.Bare) |> List.length

let sb = StringBuilder()
let line (s: string) = sb.Append(s).Append("\r\n") |> ignore

line "# Rhino.Scripting API"
line ""
line "All public types, members and functions of Rhino.Scripting, with the first sentence of their summary."
line ""
let mainMembers = plural mainCount "static member"
line $"- The {mainMembers} of `RhinoScriptSyntax`, {mainNames} names with their overloads, have one table per source file,"
line "  e.g. [Curve](#curve) for `Src/Scripting_Curve.fs`. Scripts usually call them through an alias, `type rs = RhinoScriptSyntax`."
line "- Overloads of the same name are in the order of the source."
line "- Geometry-only `Make...` methods are listed immediately after their corresponding document-adding `Add...` methods."
line "- `?name: type = value` marks an optional parameter and its default value."
line "- In the other tables, instance members and fields start with `.`, e.g. `.Pretty`. Static members are qualified with their type name."
line "- A property shows only its type. `with get, set` marks a property that can be set."
line ""
line "Generated from the sources by `API-Table/ApiTable.fsx`. Do not edit by hand."
line ""
line "## Contents"
for label, ts in [ "RhinoScriptSyntax", mainTables; "Other types, modules and extensions", otherTables ] do
    line ""
    line $"**{label}**"
    line ""
    for s, items in ts do
        let count = plural items.Length "member"
        line $"- [{s.Title}](#{anchor s.Title}), {count}"

for s, items in mainTables @ otherTables do
    line ""
    line $"## {s.Title}"
    line ""
    if s.Note <> "" then
        line s.Note
        line ""
    line "| Name | Signature | Summary |"
    line "|:--|:--|:--|"
    for i in items do
        line $"| {code i.Name} | {code i.Signature} | {text i.Summary} |"

File.WriteAllText(outFile, sb.ToString(), UTF8Encoding(false))
printfn $"Wrote {tables.Length} tables with {tables |> List.sumBy (snd >> List.length)} rows to {outFile}"

// #endregion
