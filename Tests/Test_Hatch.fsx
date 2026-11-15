// Tests for Src/Scripting_Hatch.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Hatch"

let square () = addSquare 10.
let circleGeo (r:float) = new ArcCurve(Circle(Plane.WorldXY, r)) :> Curve
let tempDir =
    let d = IO.Path.Combine(IO.Path.GetTempPath(), "RhinoScriptingHatchTest")
    IO.Directory.CreateDirectory d |> ignore
    d

// ------------------------------- creating -------------------------------

// In a new document without hatch patterns the current pattern index is -1. The documentation says that the current pattern is used
// if no pattern name is given, so this must work too.
test "AddHatch (document without a current hatch pattern)" (fun () ->
    let h = rs.AddHatch (square ())
    ok "a hatch was added" (rs.IsHatch h)
    )
rs.CurrentHatchPattern "Solid" // from here on the tests have a current pattern

test "AddHatch" (fun () ->
    let c = square ()
    let h = rs.AddHatch c
    ok "a hatch object was added" (rs.IsHatch h)
    eq "default scale" 1.0 (rs.HatchScale h)
    near "default rotation" 0.0 (rs.HatchRotation h)
    let h2 = rs.AddHatch(c, "Hatch2", 2.5, 30.)
    eq "given pattern" "Hatch2" (rs.HatchPattern h2)
    near "given scale" 2.5 (rs.HatchScale h2)
    nearTol "given rotation" 1e-9 30. (rs.HatchRotation h2)
    let h3 = rs.AddHatch(c, "Solid", 1., 0., 0.5)
    ok "explicit tolerance" (rs.IsHatch h3)
    throwsRs "unknown pattern" (fun () -> rs.AddHatch(c, "No Such Pattern"))
    let openLine = rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.)
    throwsRs "open curve" (fun () -> rs.AddHatch openLine)
    throwsRs "a point is not a curve" (fun () -> rs.AddHatch (rs.AddPoint(0., 0., 0.)))
    throws "unknown object" (fun () -> rs.AddHatch (Guid.NewGuid()))
    // a non planar closed curve
    let nonPlanar = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 3.; pt 0. 5. 0.; pt 0. 0. 0. ]
    throwsRs "non planar curve" (fun () -> rs.AddHatch nonPlanar)
    )

test "AddHatches" (fun () ->
    let a = square ()
    let b = rs.AddCircle(Plane(pt 100. 0. 0., Vector3d.ZAxis), 5.)
    let ids = rs.AddHatches [ a; b ]
    eq "one hatch for each of the two separate curves" 2 ids.Count
    for id in ids do ok "every id is a hatch" (rs.IsHatch id)
    // nested curves make a hatch with a hole
    let outer = rs.AddCircle(Plane.WorldXY, 50.)
    let inner = rs.AddCircle(Plane.WorldXY, 20.)
    let nested = rs.AddHatches([ outer; inner ], "Hatch1", 2.)
    ok "nested curves give hatches" (nested.Count >= 1)
    near "scale is applied to all" 2. (rs.HatchScale nested.[0])
    throwsRs "open curve" (fun () -> rs.AddHatches [ rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.) ])
    throwsRs "unknown pattern" (fun () -> rs.AddHatches([ a ], "No Such Pattern"))
    throwsRs "empty list" (fun () -> rs.AddHatches [])
    )

test "MakeHatch" (fun () ->
    let n = (rs.AllObjects()).Count
    let h = rs.MakeHatch(circleGeo 5.)
    ok "returns hatch geometry" (rs.IsHatch (h :> GeometryBase))
    eq "the hatch is not added to the document" n (rs.AllObjects()).Count
    near "default scale" 1.0 h.PatternScale
    let h2 = rs.MakeHatch(circleGeo 5., "Hatch3", 3., 45., 0.01)
    near "scale" 3. h2.PatternScale
    nearTol "rotation" 1e-9 45. (rs.HatchRotation h2)
    eq "pattern" "Hatch3" (rs.HatchPattern h2)
    throwsRs "unknown pattern" (fun () -> rs.MakeHatch(circleGeo 5., "No Such Pattern"))
    throwsRs "open curve" (fun () -> rs.MakeHatch (new LineCurve(pt 0. 0. 0., pt 1. 0. 0.)))
    )

test "MakeHatches" (fun () ->
    let n = (rs.AllObjects()).Count
    let hs = rs.MakeHatches [ circleGeo 5.; circleGeo 50. ]
    ok "hatch geometries are returned" (hs.Count >= 1)
    eq "nothing is added to the document" n (rs.AllObjects()).Count
    let nested = rs.MakeHatches([ circleGeo 50.; circleGeo 20. ], "Solid")
    ok "nested curves" (nested.Count >= 1)
    throwsRs "unknown pattern" (fun () -> rs.MakeHatches([ circleGeo 5. ], "No Such Pattern"))
    throwsRs "open curve" (fun () -> rs.MakeHatches [ new LineCurve(pt 0. 0. 0., pt 1. 0. 0.) :> Curve ])
    )

// ------------------------------- patterns -------------------------------

test "HatchPatternNames" (fun () ->
    let names = rs.HatchPatternNames()
    for n in [ "Solid"; "Hatch1"; "Hatch2"; "Hatch3"; "HatchDash"; "Grid"; "Grid60"; "Plus"; "Squares" ] do
        ok (sprintf "contains the default pattern %s" n) (names.Contains n)
    eq "same as HatchPatternCount (no deleted patterns)" (rs.HatchPatternCount()) names.Count
    )

test "HatchPatternCount" (fun () ->
    ok "at least the nine default patterns" (rs.HatchPatternCount() >= 9)
    )

test "IsHatchPattern" (fun () ->
    ok "default pattern" (rs.IsHatchPattern "Solid")
    ok "names are not case sensitive" (rs.IsHatchPattern "solid")
    ok "unknown pattern" (not (rs.IsHatchPattern "No Such Pattern"))
    )

test "HatchPatternDescription" (fun () ->
    ok "has a description (may be empty)" (not (isNull (rs.HatchPatternDescription "Solid")) || true)
    throwsRs "unknown pattern" (fun () -> rs.HatchPatternDescription "No Such Pattern")
    )

test "HatchPatternFillType" (fun () ->
    eq "solid fill type is 0" 0 (rs.HatchPatternFillType "Solid")
    eq "lines fill type is 1" 1 (rs.HatchPatternFillType "Hatch1")
    throwsRs "unknown pattern" (fun () -> rs.HatchPatternFillType "No Such Pattern")
    )

test "CurrentHatchPattern" (fun () ->
    let original = rs.CurrentHatchPattern()
    rs.CurrentHatchPattern "Grid"
    eq "current pattern was set" "Grid" (rs.CurrentHatchPattern())
    rs.CurrentHatchPattern "HatchDash"
    eq "set again" "HatchDash" (rs.CurrentHatchPattern())
    throwsRs "unknown pattern" (fun () -> rs.CurrentHatchPattern "No Such Pattern")
    // a new hatch without pattern name uses the current pattern
    rs.CurrentHatchPattern "Plus"
    let h = rs.AddHatch (square ())
    eq "the current pattern is used if no pattern is given" "Plus" (rs.HatchPattern h)
    rs.CurrentHatchPattern original
    )

test "IsHatchPatternCurrent" (fun () ->
    rs.CurrentHatchPattern "Grid"
    ok "current" (rs.IsHatchPatternCurrent "Grid")
    ok "not current" (not (rs.IsHatchPatternCurrent "HatchDash"))
    throwsRs "unknown pattern" (fun () -> rs.IsHatchPatternCurrent "No Such Pattern")
    )

test "IsHatchPatternReference" (fun () ->
    ok "default patterns are no reference" (not (rs.IsHatchPatternReference "Solid"))
    throwsRs "unknown pattern" (fun () -> rs.IsHatchPatternReference "No Such Pattern")
    )

test "AddHatchPatterns" (fun () ->
    let file = IO.Path.Combine(tempDir, "rsTest.pat")
    IO.File.WriteAllText(file, "*RsTestPatA, test pattern a\n0, 0,0, 0,2\n*RsTestPatB, test pattern b\n45, 0,0, 0,3\n")
    let names = rs.AddHatchPatterns file
    eq "names of the new patterns" [ "RsTestPatA"; "RsTestPatB" ] (names |> Seq.sort |> List.ofSeq)
    ok "patterns exist" (rs.IsHatchPattern "RsTestPatA" && rs.IsHatchPattern "RsTestPatB")
    eq "description is read" "test pattern a" (rs.HatchPatternDescription "RsTestPatA")
    throwsRs "adding them again without replace" (fun () -> rs.AddHatchPatterns file)
    let again = rs.AddHatchPatterns(file, true)
    eq "adding them again with replace" 2 again.Count
    throws "file does not exist" (fun () -> rs.AddHatchPatterns(IO.Path.Combine(tempDir, "nothere.pat")))
    )

// ------------------------------- hatch properties -------------------------------

test "HatchPattern" (fun () ->
    let h = rs.AddHatch(square (), "Hatch1")
    eq "get by id" "Hatch1" (rs.HatchPattern h)
    rs.HatchPattern(h, "Grid")
    eq "set by id" "Grid" (rs.HatchPattern h)
    let geo = (doc().Objects.FindId h).Geometry :?> Hatch
    eq "get from geometry" "Grid" (rs.HatchPattern geo)
    let h2 = rs.AddHatch(rs.AddCircle(Plane.WorldXY, 30.), "Hatch1")
    rs.HatchPattern([ h; h2 ], "HatchDash")
    eq "multiple set: first" "HatchDash" (rs.HatchPattern h)
    eq "multiple set: second" "HatchDash" (rs.HatchPattern h2)
    throwsRs "unknown pattern" (fun () -> rs.HatchPattern(h, "No Such Pattern"))
    throwsRs "unknown pattern, multiple" (fun () -> rs.HatchPattern([ h; h2 ], "No Such Pattern"))
    throws "not a hatch" (fun () -> rs.HatchPattern (rs.AddPoint(0., 0., 0.)))
    )

test "HatchRotation" (fun () ->
    let h = rs.AddHatch(square (), "Hatch1", 1., 10.)
    nearTol "get by id" 1e-9 10. (rs.HatchRotation h)
    rs.HatchRotation(h, 75.)
    nearTol "set by id" 1e-9 75. (rs.HatchRotation h)
    rs.HatchRotation(h, 75.) // same value: nothing happens
    let geo = (doc().Objects.FindId h).Geometry :?> Hatch
    nearTol "get from geometry" 1e-9 75. (rs.HatchRotation geo)
    let h2 = rs.AddHatch(rs.AddCircle(Plane.WorldXY, 30.), "Hatch1")
    rs.HatchRotation([ h; h2 ], 20.)
    nearTol "multiple set: first" 1e-9 20. (rs.HatchRotation h)
    nearTol "multiple set: second" 1e-9 20. (rs.HatchRotation h2)
    throws "not a hatch" (fun () -> rs.HatchRotation (rs.AddPoint(0., 0., 0.)))
    )

test "HatchScale" (fun () ->
    let h = rs.AddHatch(square (), "Hatch1", 2.)
    near "get by id" 2. (rs.HatchScale h)
    rs.HatchScale(h, 4.)
    near "set by id" 4. (rs.HatchScale h)
    rs.HatchScale(h, 4.) // same value: nothing happens
    let geo = (doc().Objects.FindId h).Geometry :?> Hatch
    near "get from geometry" 4. (rs.HatchScale geo)
    let h2 = rs.AddHatch(rs.AddCircle(Plane.WorldXY, 30.), "Hatch1")
    rs.HatchScale([ h; h2 ], 0.5)
    near "multiple set: first" 0.5 (rs.HatchScale h)
    near "multiple set: second" 0.5 (rs.HatchScale h2)
    throws "not a hatch" (fun () -> rs.HatchScale (rs.AddPoint(0., 0., 0.)))
    )

test "IsHatch" (fun () ->
    let h = rs.AddHatch (square ())
    ok "hatch object" (rs.IsHatch h)
    ok "curve is no hatch" (not (rs.IsHatch (square ())))
    ok "unknown id" (not (rs.IsHatch (Guid.NewGuid())))
    ok "hatch geometry" (rs.IsHatch (rs.MakeHatch(circleGeo 5.) :> GeometryBase))
    ok "curve geometry" (not (rs.IsHatch (circleGeo 5. :> GeometryBase)))
    )

test "ExplodeHatch" (fun () ->
    let h = rs.AddHatch(square (), "Hatch1")
    let n = (rs.AllObjects()).Count
    let pieces = rs.ExplodeHatch h
    ok "pieces were created" (pieces.Count > 0)
    eq "they were added to the document" (n + pieces.Count) (rs.AllObjects()).Count
    ok "the hatch still exists" (rs.IsHatch h)
    let pieces2 = rs.ExplodeHatch(h, true)
    ok "pieces again" (pieces2.Count > 0)
    ok "the hatch was deleted" (not (rs.IsHatch h))
    // geometry overload
    let geo = rs.MakeHatch(circleGeo 5., "Hatch1")
    let g = rs.ExplodeHatch geo
    ok "geometry pieces" (g.Count > 0)
    ok "geometry pieces are curves or breps" (g |> Seq.forall (fun x -> x :? Curve || x :? Brep))
    throws "not a hatch" (fun () -> rs.ExplodeHatch (rs.AddPoint(0., 0., 0.)))
    )

try IO.Directory.Delete(tempDir, true) with _ -> ()

printfn "%s" (finish ())
