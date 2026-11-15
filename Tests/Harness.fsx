// Shared test harness for the Tests/Test_*.fsx scripts.
// These scripts are meant to be run inside a running Rhino, e.g. through the Caddis MCP server (caddis_eval with path=...),
// or any other F# Interactive session that lives inside the Rhino process.
//
// Each Test_Xxx.fsx file starts with `#load "Harness.fsx"`, opens `Harness`, calls `start "Xxx"`, runs `test "FunctionName" (fun () -> ...)`
// for the public functions of Src/Scripting_Xxx.fs and ends with `finish ()`.
// The tests run in a fresh, separate Rhino document that is closed again by `finish`, so the document the user works in stays untouched.
// Functions that need user interaction or would harm the Rhino session are not called, they are listed with `skip "Name" "reason"`.

module Harness

#if !RHINO // so that when viewed outside of Rhino, no compiler errors show
#r "nuget: RhinoCommon, 8.19.25132.1001"
#endif
#r "../bin/Release/net8.0/Rhino.Scripting.dll"

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting

type rs = RhinoScriptSyntax

type Outcome = Pass | Fail of string | Skip of string

let results = ResizeArray<string * Outcome>()
let mutable private userDoc : RhinoDoc = null
let mutable private testDoc : RhinoDoc = null
let mutable private suite = ""

/// Document used by the tests
let doc () = testDoc

// ---------------------------- assertions ----------------------------

let private fmt (x:obj) =
    match x with
    | null -> "null"
    | :? string as s -> sprintf "\"%s\"" s
    | :? Collections.IEnumerable as e -> e |> Seq.cast<obj> |> Seq.truncate 12 |> Seq.map string |> String.concat "; " |> sprintf "[%s]"
    | x -> string x

/// Fails the current test if the condition is false
let ok (msg:string) (cond:bool) = if not cond then failwithf "%s" msg

/// Fails if the two values are not equal
let eq (msg:string) (expected:'T) (actual:'T) =
    if not (expected = actual) then failwithf "%s: expected %s but got %s" msg (fmt (box expected)) (fmt (box actual))

/// Fails if the two floats differ by more than tol (default 1e-6)
let near (msg:string) (expected:float) (actual:float) =
    if not (abs (expected - actual) <= 1e-6 * (1.0 + abs expected)) then failwithf "%s: expected %g but got %g" msg expected actual

/// Fails if the two floats differ by more than tol
let nearTol (msg:string) (tol:float) (expected:float) (actual:float) =
    if not (abs (expected - actual) <= tol) then failwithf "%s: expected %g (+-%g) but got %g" msg expected tol actual

/// Fails if the two points differ by more than 1e-6
let nearPt (msg:string) (expected:Point3d) (actual:Point3d) =
    if expected.DistanceTo actual > 1e-6 then failwithf "%s: expected %s but got %s" msg (expected.ToString()) (actual.ToString())

/// Fails if the two vectors differ by more than 1e-6
let nearVec (msg:string) (expected:Vector3d) (actual:Vector3d) =
    if (expected - actual).Length > 1e-6 then failwithf "%s: expected %s but got %s" msg (expected.ToString()) (actual.ToString())

/// Fails if the function does not throw. Any exception counts.
let throws (msg:string) (f: unit -> 'T) =
    let r = try Choice1Of2 (f ()) with e -> Choice2Of2 e
    match r with
    | Choice1Of2 v -> failwithf "%s: expected an exception but got %s" msg (fmt (box v))
    | Choice2Of2 _ -> ()

/// Fails if the function does not throw a RhinoScriptingException
let throwsRs (msg:string) (f: unit -> 'T) =
    let r = try Choice1Of2 (f ()) with e -> Choice2Of2 e
    match r with
    | Choice1Of2 v -> failwithf "%s: expected a RhinoScriptingException but got %s" msg (fmt (box v))
    | Choice2Of2 (:? RhinoScriptingException) -> ()
    | Choice2Of2 e -> failwithf "%s: expected a RhinoScriptingException but got %s: %s" msg (e.GetType().Name) e.Message

// ---------------------------- document helpers ----------------------------

/// Deletes every object in the test document
let clearObjects () =
    let s = DocObjects.ObjectEnumeratorSettings(NormalObjects = true, LockedObjects = true, HiddenObjects = true, IncludeLights = true, IncludeGrips = true, DeletedObjects = false)
    let objs = testDoc.Objects.GetObjectList(s) |> Seq.toArray
    // Never purge lights: Rhino 9.0 crashes when it later closes a document from which a light was purged. Delete them instead.
    for o in objs do
        if o.ObjectType = DocObjects.ObjectType.Light then testDoc.Objects.Delete(o, true) |> ignore
        else testDoc.Objects.Purge(o) |> ignore

/// Counts the lights that Objects.Purge left behind without a document.
/// Rhino 9.0 crashes when it closes a document that has one. Rhino 8 does not report them and does not crash.
let purgedLightCount (d:RhinoDoc) =
    if isNull d then 0 else
    let s = DocObjects.ObjectEnumeratorSettings(IncludeLights = true, NormalObjects = false, DeletedObjects = true, ObjectTypeFilter = DocObjects.ObjectType.Light)
    d.Objects.GetObjectList(s) |> Seq.filter (fun o -> isNull o.Document) |> Seq.length

/// Deletes the block definitions and groups of the test document
let clearDefinitions () =
    for d in testDoc.InstanceDefinitions.GetList(true) |> Seq.toArray do
        testDoc.InstanceDefinitions.Delete(d.Index, true, true) |> ignore
    for i in 0 .. testDoc.Groups.Count - 1 do
        if not (isNull (testDoc.Groups.GroupName i)) then testDoc.Groups.Delete i |> ignore

/// Runs one named test. The name should be the name of the function in RhinoScriptSyntax that is tested.
/// The test passes if the function returns without exception. Objects added to the document are removed afterwards.
let test (name:string) (f: unit -> unit) =
    let purgedBefore = purgedLightCount testDoc
    let outcome =
        try
            f ()
            Pass
        with e ->
            Fail (e.GetType().Name + ": " + e.Message.Split('\n').[0])
    try clearObjects () with _ -> ()
    try clearDefinitions () with _ -> ()
    let outcome =
        if purgedLightCount testDoc > purgedBefore then
            Fail "a light was purged: Rhino 9 crashes when this document is closed, so the next 'start' refuses to run. Delete lights instead of purging them, then restart Rhino."
        else outcome
    results.Add((name, outcome))

/// Marks a function as not tested, with the reason
let skip (name:string) (reason:string) = results.Add((name, Skip reason))

// ---------------------------- geometry shortcuts ----------------------------

let pt x y z = Point3d(x, y, z)
let line (a:Point3d) (b:Point3d) = Line(a, b)

/// Adds a straight line curve object from p to q
let addLine (p:Point3d) (q:Point3d) = rs.AddLine(p, q)
/// Adds a unit square polyline in the XY plane, closed
let addSquare (size:float) = rs.AddPolyline [| pt 0. 0. 0.; pt size 0. 0.; pt size size 0.; pt 0. size 0.; pt 0. 0. 0. |]
/// Adds a circle curve in the XY plane at the origin
let addCircle (r:float) = rs.AddCircle(Plane.WorldXY, r)
/// Adds a box brep (polysurface) with its corner at the origin
let addBox (x:float) (y:float) (z:float) =
    let b = Box(Plane.WorldXY, Interval(0., x), Interval(0., y), Interval(0., z))
    testDoc.Objects.AddBrep(b.ToBrep())
/// Adds a planar rectangle surface from 0,0,0 to w,h,0
let addPlaneSurface (w:float) (h:float) = rs.AddPlaneSurface(Plane.WorldXY, w, h)
/// Adds a sphere surface (brep)
let addSphere (r:float) = rs.AddSphere(Point3d.Origin, r)
/// Adds a mesh box
let addMeshBox (x:float) (y:float) (z:float) =
    let m = Mesh.CreateFromBox(Box(Plane.WorldXY, Interval(0., x), Interval(0., y), Interval(0., z)), 1, 1, 1)
    testDoc.Objects.AddMesh m

// ---------------------------- lifecycle ----------------------------

/// Creates a fresh Rhino document for the tests and makes it the active one.
let start (suiteName:string) =
    results.Clear()
    suite <- suiteName
    userDoc <- RhinoDoc.ActiveDoc
    // RhinoDoc.Create closes the active document, which would crash Rhino 9 if a light was purged from it
    if purgedLightCount userDoc > 0 then
        failwith "The active document contains a purged light, closing it would crash Rhino 9. Restart Rhino before running the tests."
    testDoc <- RhinoDoc.Create(null)
    if isNull testDoc then failwith "Could not create a test document"
    if RhinoDoc.ActiveDoc.RuntimeSerialNumber <> testDoc.RuntimeSerialNumber then failwith "The test document is not the active document"

/// Closes the test document, prints the summary and returns it as a string.
let finish () =
    let passed = results |> Seq.filter (fun (_, o) -> o = Pass) |> Seq.length
    let fails = results |> Seq.choose (function (n, Fail m) -> Some (n, m) | _ -> None) |> Seq.toList
    let skips = results |> Seq.choose (function (n, Skip m) -> Some (n, m) | _ -> None) |> Seq.toList
    let sb = Text.StringBuilder()
    sb.AppendLine(sprintf "=== %s: %d passed, %d failed, %d skipped ===" suite passed fails.Length skips.Length) |> ignore
    for (n, m) in fails do sb.AppendLine(sprintf "FAIL %s -> %s" n m) |> ignore
    for (reason, group) in skips |> List.groupBy snd do
        sb.AppendLine(sprintf "SKIP (%s): %s" reason (group |> List.map fst |> String.concat ", ")) |> ignore
    try
        if not (isNull testDoc) then
            testDoc.Modified <- false
            testDoc.Dispose()
    with e -> sb.AppendLine("could not close test document: " + e.Message) |> ignore
    testDoc <- null
    sb.ToString()
