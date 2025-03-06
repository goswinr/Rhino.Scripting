namespace Rhino.Scripting

open Rhino
open System
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils


/// This module provides type extensions for pretty printing.
/// It adds a 'rhObj.Pretty' property to many Rhino geometry objects.
/// This module is automatically opened when Rhino namespace is opened.
/// These type extensions are only visible in F# (not in C#)
/// This auto-open module is under the 'Rhino.Scripting' namespace so that making these extensions available works
/// with 'open Rhino.Scripting'.
[<AutoOpen>]
module AutoOpenPrettyExtensions =

    type Point3d with
        ///Like the ToString function but with appropriate precision formatting
        member pt.Pretty =
            if pt = Point3d.Unset then
                "Point3d.Unset"
            else
                sprintf "Point3d(%s, %s, %s)" (PrettyFormat.float  pt.X) (PrettyFormat.float  pt.Y) (PrettyFormat.float  pt.Z)

    type Point3f with
        ///Like the ToString function but with appropriate precision formatting
        member pt.Pretty =
            if pt = Point3f.Unset then
                "Point3f.Unset"
            else
                sprintf "Point3f(%s, %s, %s)" (PrettyFormat.single  pt.X) (PrettyFormat.single  pt.Y) (PrettyFormat.single  pt.Z)

    type Vector3d with
        ///Like the ToString function but with appropriate precision formatting
        member v.Pretty =
            if v = Vector3d.Unset then
                "Vector3d.Unset"
            else
                sprintf "Vector3d(%s, %s, %s)" (PrettyFormat.float  v.X) (PrettyFormat.float  v.Y) (PrettyFormat.float  v.Z)

    type Vector3f with
        ///Like the ToString function but with appropriate precision formatting
        member v.Pretty =
            if v = Vector3f.Unset then
                "Vector3f.Unset"
            else
                sprintf "Vector3f(%s, %s, %s)" (PrettyFormat.single  v.X) (PrettyFormat.single  v.Y) (PrettyFormat.single  v.Z)

    type Line with
        ///Like the ToString function but with appropriate precision formatting
        member ln.Pretty =
            sprintf "Rhino.Geometry.Line from %s, %s, %s to %s, %s, %s" (PrettyFormat.float  ln.From.X) (PrettyFormat.float  ln.From.Y) (PrettyFormat.float  ln.From.Z) (PrettyFormat.float  ln.To.X) (PrettyFormat.float  ln.To.Y) (PrettyFormat.float  ln.To.Z)

    type Transform with
        /// returns a string showing the Transformation Matrix in an aligned 4x4 grid
        member m.Pretty =
            let vs =
                m.ToFloatArray(true)
                |> Array.map (sprintf "%g")
            let cols =
                [| for i=0 to 3 do
                    [| vs.[0+i];vs.[4+i];vs.[8+i];vs.[12+i] |]
                    |> Array.map String.length
                    |> Array.max
                |]
            let b = Text.StringBuilder()
            b.AppendLine "Rhino.Geometry.Transform:"  |> ignore
            for i,s in Seq.indexed vs do
                let coli = i%4
                let len = cols.[coli]
                b.Append("| ")  |> ignore
                b.Append(s.PadLeft(len,' '))  |> ignore
                if coli = 3 then
                    b.AppendLine() |> ignore
            b.ToString()


    type Plane with
        /// returns a string showing the Transformation Matrix in an aligned 4x4 grid
        member p.Pretty =
            if not p.IsValid then "invalid Rhino.Geometry.Plane"
            else
                [|
                "Rhino.Geometry.Plane:"
                sprintf "Origin X=%s Y=%s Z=%s" (PrettyFormat.float  p.Origin.X)(PrettyFormat.float  p.Origin.Y) (PrettyFormat.float  p.Origin.Z)
                sprintf "X-Axis X=%s Y=%s Z=%s" (PrettyFormat.float  p.XAxis.X) (PrettyFormat.float  p.XAxis.Y) (PrettyFormat.float  p.XAxis.Z)
                sprintf "Y-Axis X=%s Y=%s Z=%s" (PrettyFormat.float  p.YAxis.X) (PrettyFormat.float  p.YAxis.Y) (PrettyFormat.float  p.YAxis.Z)
                |]
                |> String.concat Environment.NewLine

    type BoundingBox with
        /// returns a string showing the Transformation Matrix in an aligned 4x4 grid
        member b.Pretty =
            [|
            if b.IsDegenerate(Rhino.Scripting.State.Doc.ModelAbsoluteTolerance) > 0 then
                sprintf "flat Rhino.Geometry.BoundingBox: Size X=%s Y=%s Z=%s" (PrettyFormat.float b.Diagonal.X) (PrettyFormat.float  b.Diagonal.Y) (PrettyFormat.float  b.Diagonal.Z)
            elif not b.IsValid then
                "invalid (decreasing?) Rhino.Geometry.BoundingBox"
            else
                "Rhino.Geometry.BoundingBox:"
            sprintf "Size X=%s Y=%s Z=%s" (PrettyFormat.float b.Diagonal.X) (PrettyFormat.float  b.Diagonal.Y) (PrettyFormat.float  b.Diagonal.Z)
            sprintf "from X=%s Y=%s Z=%s" (PrettyFormat.float  b.Min.X) (PrettyFormat.float  b.Min.Y) (PrettyFormat.float  b.Min.Z)
            sprintf "till X=%s Y=%s Z=%s" (PrettyFormat.float  b.Max.X) (PrettyFormat.float  b.Max.Y) (PrettyFormat.float  b.Max.Z)
            |]
            |> String.concat Environment.NewLine



/// Part of Rhino.Scripting nuget.
/// An internal module to set up nice printing of Rhino Objects.
/// (It is public only for access from Rhino.Scripting.Fsharp project)
[<RequireQualifiedAccess>]
module PrettySetup =

    let mutable private initIsPending = true // to delay setup of printing till first print call

    /// Gets a description on Rhino object type (curve , point, Surface ....).
    /// Including Layer and object name
    let internal guid (g:Guid)=
        if g = Guid.Empty then
            "-Guid.Empty-"
        elif Runtime.HostUtils.RunningInRhino  then // because Rhino.Scripting might be referenced from outside of Rhino too
            let o = State.Doc.Objects.FindId(g)
            if isNull o then sprintf "Guid %O (not in State.Doc.Objects table of this Rhino file)." g
            else
                let name = o.Attributes.Name // null if unset
                if String.IsNullOrWhiteSpace name then
                    sprintf "Guid %O (a %s%s%s on Layer '%s')" g  (if o.IsDeleted then "deleted " else "") (if o.IsHidden then "hidden " else "") (o.ShortDescription(false)) (State.Doc.Layers.[o.Attributes.LayerIndex].FullPath)
                else
                    sprintf "Guid %O (a %s%s%s named '%s' on Layer '%s')" g (if o.IsDeleted then "deleted " else "") (if o.IsHidden then "hidden " else "") (o.ShortDescription(false)) name (State.Doc.Layers.[o.Attributes.LayerIndex].FullPath)
        else
            // the Pretty.str function gets injected into FsEx.Pretty, below
            // so that using the print function still works on other Guids if Rhino.Scripting is referenced from outside of rhino where there is no active Doc
            sprintf "Guid %O" g

    /// Rhino specific formatting function that is set to be the
    /// externalFormatter in FsEx.PrettySettings. It is set in the init() function.
    let internal formatRhinoObject : obj -> option<string> =
        function
        | :? Guid        as x -> Some <| (guid x)
        | :? Point3d     as x -> Some <| x.Pretty
        | :? Vector3d    as x -> Some <| x.Pretty
        | :? Line        as x -> Some <| x.Pretty
        | :? Point3f     as x -> Some <| x.Pretty
        | :? Vector3f    as x -> Some <| x.Pretty
        | :? Transform   as x -> Some <| x.Pretty
        | :? Plane       as x -> Some <| x.Pretty
        | :? BoundingBox as x -> Some <| x.Pretty
        | _                   -> None

    /// This should only needs to be called by other libraries that build on top of this pretty printing functions
    let init()=
        if initIsPending then
            initIsPending <- false
            try
                //PrettySettings.externalFormatter  <-  (fun o -> formatRhinoObject o |> Option.map PrettySettings.Element)
                if notNull RhinoSync.PrettyFormatters then
                    RhinoSync.PrettyFormatters.Add(formatRhinoObject)
                if Rhino.Runtime.HostUtils.RunningInRhino then
                    // these below fail if not running inside rhino.exe
                    // scripts that reference Rhino.Scripting from outside of rhino is still Ok , but all function that call the C++ API don't work
                    // but accessing the current Doc obviously does not work.
                    PrettySettings.userZeroTolerance                                              <- State.Doc.ModelAbsoluteTolerance  * 0.1 // so that any float smaller than State.Doc.ModelAbsoluteTolerance wil be shown as 0.0
                    RhinoApp.AppSettingsChanged.Add    (fun _ -> PrettySettings.userZeroTolerance <- State.Doc.ModelAbsoluteTolerance  * 0.1 )
                    RhinoDoc.ActiveDocumentChanged.Add (fun a -> PrettySettings.userZeroTolerance <- a.Document.ModelAbsoluteTolerance * 0.1 )
                    RhinoDoc.EndOpenDocument.Add       (fun a -> PrettySettings.userZeroTolerance <- a.Document.ModelAbsoluteTolerance * 0.1 )
            with e ->
                // try to log errors to error stream:
                eprintfn "Initializing Pretty pretty printing in Rhino.InternalToPrettySetup.init() via Rhino.Scripting.dll failed with:%s%A" Environment.NewLine e


    /// Like printfn but in Blue if used from Fesh Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    let internal printfnBlue msg =
        init()
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            printfn "%s" s
            // Printfn.blue "%s" s
            RhinoApp.Wait()
            )  msg // no switch to UI Thread needed !

    /// Like printfn but in Red if used from Fesh Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    let internal printfnErr msg =
        init()
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            eprintfn "%s" s
            // Printfn.blue "%s" s
            RhinoApp.Wait()
            )  msg // no switch to UI Thread needed !


[<RequireQualifiedAccess>]
module internal Pretty  =

    /// Pretty formatting for Rhino and .Net types, e.g. numbers including thousand Separator and (nested) sequences, first five items are printed out.
    /// Settings are exposed in FsEx.Pretty.PrettySettings:
    /// - thousandSeparator       = '     ; set this to change the printing of floats and integers larger than 10'000
    /// - maxNestingDepth         = 3     ; set this to change how deep the content of nested seq is printed (printFull ignores this)
    /// - maxNestingDepth         = 6     ; set this to change how how many items per seq are printed (printFull ignores this)
    /// - maxCharsInString        = 2000  ; set this to change how many characters of a string might be printed at once.
    let str (x:'T) :string =
        PrettySetup.init() // the shadowing is only done to ensure init() is called once
        // Pretty.toPretty x
        x.ToString()


