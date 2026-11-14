// Tests for Src/Scripting_Utility.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Utility"

let vec x y z = Vector3d(x, y, z)

test "ContextIsRhino" (fun () -> ok "running in Rhino" (rs.ContextIsRhino()))

test "ContextIsGrasshopper" (fun () -> ok "not called from a Grasshopper component" (not (rs.ContextIsGrasshopper())))

test "Angle" (fun () ->
    let a, e, dx, dy, dz = rs.Angle(pt 0. 0. 0., pt 1. 1. 0.)
    near "XY angle" 45. a
    near "elevation" 0. e
    near "dx" 1. dx
    near "dy" 1. dy
    near "dz" 0. dz
    let a2, e2, _, _, dz2 = rs.Angle(pt 0. 0. 0., pt 1. 0. 1.)
    near "XY angle along x" 0. a2
    near "elevation 45" 45. e2
    near "dz" 1. dz2
    let a3, _, _, _, _ = rs.Angle(pt 0. 0. 0., pt -1. 0. 0.)
    near "pointing to -x gives 180" 180. a3
    let a4, _, _, _, _ = rs.Angle(pt 0. 0. 0., pt 0. -1. 0.)
    near "pointing to -y gives -90" -90. a4
    // with respect to a rotated plane
    let rotated = Plane(Point3d.Origin, Vector3d.YAxis, -Vector3d.XAxis)
    let a5, _, dx5, dy5, _ = rs.Angle(pt 0. 0. 0., pt 1. 0. 0., rotated)
    near "angle with respect to a plane rotated by 90 degrees" -90. a5
    near "dx in plane coordinates" 0. dx5
    near "dy in plane coordinates" -1. dy5
    )

test "Angle2" (fun () ->
    let a, r = rs.Angle2(line (pt 0. 0. 0.) (pt 1. 0. 0.), line (pt 0. 0. 0.) (pt 0. 1. 0.))
    near "angle" 90. a
    near "reflex angle" 270. r
    let a2, r2 = rs.Angle2(line (pt 0. 0. 0.) (pt 1. 0. 0.), line (pt 5. 5. 5.) (pt 7. 5. 5.))
    near "parallel lines" 0. a2
    near "parallel lines reflex" 360. r2
    let a3, _ = rs.Angle2(line (pt 0. 0. 0.) (pt 1. 0. 0.), line (pt 0. 0. 0.) (pt -1. 0. 0.))
    near "opposite direction" 180. a3
    throwsRs "zero length line" (fun () -> rs.Angle2(line (pt 0. 0. 0.) (pt 0. 0. 0.), line (pt 0. 0. 0.) (pt 1. 0. 0.)))
    )

test "ClipboardText" (fun () ->
    let original = rs.ClipboardText()
    try
        rs.ClipboardText("Rhino.Scripting test 123")
        eq "text round trip" "Rhino.Scripting test 123" (rs.ClipboardText())
    finally
        // restore what the user had in the clipboard
        if original <> "" then rs.ClipboardText(original)
        else
            // an empty text can not be set, so clear the clipboard through reflection (like the library does it)
            let t = Type.GetType "System.Windows.Forms.Clipboard, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            if not (isNull t) then t.GetMethod("Clear", Type.EmptyTypes).Invoke(null, null) |> ignore
    )

test "ColorAdjustLuma" (fun () ->
    let red = Drawing.Color.FromArgb(255, 0, 0)
    let l0 = (rs.ColorRGBToHLS red).L
    near "red has luminance 0.5" 0.5 l0
    let dark = rs.ColorAdjustLuma(red, 250.)
    nearTol "absolute luminance 250 means 25 percent (8 bit rounding)" 0.01 0.25 ((rs.ColorRGBToHLS dark).L)
    let lighter = rs.ColorAdjustLuma(red, 100., true)
    nearTol "relative: increments the luminance by 10 percent" 0.01 0.6 ((rs.ColorRGBToHLS lighter).L)
    let darker = rs.ColorAdjustLuma(red, -100., true)
    nearTol "relative: negative decrements" 0.01 0.4 ((rs.ColorRGBToHLS darker).L)
    let white = rs.ColorAdjustLuma(red, 1000.)
    eq "maximum luminance is white" (Drawing.Color.FromArgb(255, 255, 255)) (Drawing.Color.FromArgb(int white.R, int white.G, int white.B))
    // hue is not affected
    nearTol "hue is kept" 0.01 ((rs.ColorRGBToHLS red).H) ((rs.ColorRGBToHLS dark).H)
    )

test "ColorBlueValue" (fun () -> eq "blue" 30 (rs.ColorBlueValue (Drawing.Color.FromArgb(10, 20, 30))))
test "ColorGreenValue" (fun () -> eq "green" 20 (rs.ColorGreenValue (Drawing.Color.FromArgb(10, 20, 30))))
test "ColorRedValue" (fun () -> eq "red" 10 (rs.ColorRedValue (Drawing.Color.FromArgb(10, 20, 30))))

test "ColorRGBToHLS" (fun () ->
    let hsl = rs.ColorRGBToHLS (Drawing.Color.FromArgb(255, 0, 0))
    near "hue of red" 0. hsl.H
    near "saturation of red" 1. hsl.S
    near "luminance of red" 0.5 hsl.L
    let g = rs.ColorRGBToHLS (Drawing.Color.FromArgb(128, 128, 128))
    near "saturation of gray" 0. g.S
    ok "luminance of gray" (abs (g.L - 0.5) < 0.01)
    )

test "ColorHLSToRGB" (fun () ->
    // the HLS color is passed as a Drawing.Color with the values on a 0 to 240 scale: R=hue, G=luminance, B=saturation (A=240 for full opacity)
    let red = rs.ColorHLSToRGB (Drawing.Color.FromArgb(240, 0, 120, 240))
    eq "pure red" (255, 0, 0) (int red.R, int red.G, int red.B)
    let white = rs.ColorHLSToRGB (Drawing.Color.FromArgb(240, 0, 240, 0))
    eq "full luminance is white" (255, 255, 255) (int white.R, int white.G, int white.B)
    let black = rs.ColorHLSToRGB (Drawing.Color.FromArgb(240, 0, 0, 0))
    eq "zero luminance is black" (0, 0, 0) (int black.R, int black.G, int black.B)
    )

test "CullDuplicateNumbers" (fun () ->
    eq "exact duplicates" [ 1.; 2.; 3. ] (List.ofSeq (rs.CullDuplicateNumbers [ 3.; 1.; 2.; 2.; 1. ]))
    eq "within the custom tolerance" [ 1.; 2. ] (List.ofSeq (rs.CullDuplicateNumbers([ 1.; 1.0004; 2.; 2.0003 ], 0.001)))
    eq "outside the default tolerance" 2 (rs.CullDuplicateNumbers [ 1.; 1.0004 ]).Count
    eq "empty" 0 (rs.CullDuplicateNumbers []).Count
    )

test "CullDuplicatePoints" (fun () ->
    eq "exact duplicates" 2 (rs.CullDuplicatePoints [ pt 0. 0. 0.; pt 1. 1. 1.; pt 0. 0. 0. ]).Length
    eq "within a custom tolerance" 1 (rs.CullDuplicatePoints([ pt 0. 0. 0.; pt 0.01 0. 0. ], 0.1)).Length
    eq "outside the default tolerance" 2 (rs.CullDuplicatePoints [ pt 0. 0. 0.; pt 0.01 0. 0. ]).Length
    let empty = rs.CullDuplicatePoints []
    ok "empty input returns an empty array, not null" (not (isNull (box empty)) && empty.Length = 0)
    )

test "DistanceSquare" (fun () ->
    near "3-4-5 squared" 25. (rs.DistanceSquare(pt 0. 0. 0., pt 3. 4. 0.))
    near "same point" 0. (rs.DistanceSquare(pt 1. 1. 1., pt 1. 1. 1.))
    )

test "Distance" (fun () ->
    near "3-4-5" 5. (rs.Distance(pt 0. 0. 0., pt 3. 4. 0.))
    near "3D" (sqrt 3.) (rs.Distance(pt 0. 0. 0., pt 1. 1. 1.))
    )

test "Polar" (fun () ->
    nearPt "angle 0 goes along x" (pt 3. 1. 0.) (rs.Polar(pt 1. 1. 0., 0., 2.))
    nearPt "angle 90 goes along y" (pt 1. 3. 0.) (rs.Polar(pt 1. 1. 0., 90., 2.))
    nearPt "angle 180 goes along -x" (pt -1. 1. 0.) (rs.Polar(pt 1. 1. 0., 180., 2.))
    nearPt "negative distance goes backwards" (pt -1. 1. 0.) (rs.Polar(pt 1. 1. 0., 0., -2.))
    let yz = Plane.WorldYZ // x axis is world y, normal is world x
    nearPt "with respect to a plane" (pt 0. 2. 0.) (rs.Polar(pt 0. 0. 0., 0., 2., yz))
    nearPt "with respect to a plane, rotated 90 degrees" (pt 0. 0. 2.) (rs.Polar(pt 0. 0. 0., 90., 2., yz))
    )

test "SimplifyArray" (fun () ->
    eq "flattened" [ 1.; 2.; 3.; 4.; 5.; 6. ] (List.ofSeq (rs.SimplifyArray [ pt 1. 2. 3.; pt 4. 5. 6. ]))
    eq "empty" 0 (rs.SimplifyArray []).Count
    )

test "Sleep" (fun () ->
    let sw = Diagnostics.Stopwatch.StartNew()
    rs.Sleep 50
    ok "sleeps at least about 50 ms" (sw.ElapsedMilliseconds >= 45L)
    rs.Sleep 0
    )

test "SortPointList" (fun () ->
    let sorted = rs.SortPointList [ pt 2. 0. 0.; pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 0. 0. ]
    eq "duplicates are removed" 3 sorted.Length
    let xs = sorted |> Array.map (fun p -> p.X)
    ok "points are sorted along the line, in one of the two directions" (xs = [| 0.; 1.; 2. |] || xs = [| 2.; 1.; 0. |])
    eq "custom tolerance removes close points" 2 (rs.SortPointList([ pt 0. 0. 0.; pt 0.01 0. 0.; pt 5. 0. 0. ], 0.1)).Length
    )

test "SortPoints" (fun () ->
    let pts = [ pt 2. 1. 0.; pt 1. 3. 5.; pt 1. 2. 9. ]
    let by order asc = rs.SortPoints(pts, asc, order) |> Seq.map (fun p -> (p.X, p.Y, p.Z)) |> List.ofSeq
    eq "default order X, Y, Z ascending" [ (1., 2., 9.); (1., 3., 5.); (2., 1., 0.) ] (by 0 true)
    eq "descending" [ (2., 1., 0.); (1., 3., 5.); (1., 2., 9.) ] (by 0 false)
    eq "order 2: Y first" [ (2., 1., 0.); (1., 2., 9.); (1., 3., 5.) ] (by 2 true)
    eq "order 4: Z first" [ (2., 1., 0.); (1., 3., 5.); (1., 2., 9.) ] (by 4 true)
    eq "order 5: Z, Y, X descending" [ (1., 2., 9.); (1., 3., 5.); (2., 1., 0.) ] (by 5 false)
    eq "order 1: X, Z, Y" [ (1., 3., 5.); (1., 2., 9.); (2., 1., 0.) ] (by 1 true)
    eq "order 3: Y, Z, X" [ (2., 1., 0.); (1., 2., 9.); (1., 3., 5.) ] (by 3 true)
    eq "the optional arguments have defaults" (by 0 true) (rs.SortPoints pts |> Seq.map (fun p -> (p.X, p.Y, p.Z)) |> List.ofSeq)
    throwsRs "order out of range" (fun () -> rs.SortPoints(pts, true, 6))
    )

test "Str2Pt" (fun () ->
    nearPt "simple" (pt 1. 2. 3.) (rs.Str2Pt "1, 2, 3")
    nearPt "negative and decimals" (pt -1.5 2.25 0.) (rs.Str2Pt "-1.5,2.25,0")
    throwsRs "not enough numbers" (fun () -> rs.Str2Pt "1, 2")
    throwsRs "not numbers" (fun () -> rs.Str2Pt "a, b, c")
    )

test "CreatePoint" (fun () ->
    nearPt "from Point3d" (pt 1. 2. 3.) (rs.CreatePoint(pt 1. 2. 3.))
    nearPt "from Point3f" (pt 1. 2. 3.) (rs.CreatePoint(Point3f(1.f, 2.f, 3.f)))
    nearPt "from three floats" (pt 1. 2. 3.) (rs.CreatePoint(1., 2., 3.))
    nearPt "from a sequence" (pt 1. 2. 3.) (rs.CreatePoint([ 1.; 2.; 3. ] : float seq))
    nearPt "from a string" (pt 1. 2. 3.) (rs.CreatePoint("1, 2, 3"))
    let id = rs.AddPoint(4., 5., 6.)
    nearPt "from the Guid of a point object" (pt 4. 5. 6.) (rs.CreatePoint(id))
    nearPt "from a PointObject" (pt 4. 5. 6.) (rs.CreatePoint(doc().Objects.FindId(id) :?> DocObjects.PointObject))
    let dotId = rs.AddTextDot("dot", pt 7. 8. 9.)
    nearPt "from a TextDot" (pt 7. 8. 9.) (rs.CreatePoint((doc().Objects.FindId(dotId).Geometry) :?> TextDot))
    throwsRs "sequence with two items" (fun () -> rs.CreatePoint([ 1.; 2. ] : float seq))
    throwsRs "sequence with four items" (fun () -> rs.CreatePoint([ 1.; 2.; 3.; 4. ] : float seq))
    throwsRs "null sequence" (fun () -> rs.CreatePoint(null : float seq))
    throwsRs "string that is no point" (fun () -> rs.CreatePoint("hello"))
    throwsRs "Guid that does not exist" (fun () -> rs.CreatePoint(Guid.NewGuid()))
    throwsRs "Guid of an object that is not a point" (fun () -> rs.CreatePoint(rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.)))
    throwsRs "null PointObject" (fun () -> rs.CreatePoint(null : DocObjects.PointObject))
    throwsRs "null TextDot" (fun () -> rs.CreatePoint(null : TextDot))
    )

test "CreateVector" (fun () ->
    nearVec "from Vector3d" (vec 1. 2. 3.) (rs.CreateVector(vec 1. 2. 3.))
    nearVec "from Vector3f" (vec 1. 2. 3.) (rs.CreateVector(Vector3f(1.f, 2.f, 3.f)))
    nearVec "from three floats" (vec 1. 2. 3.) (rs.CreateVector(1., 2., 3.))
    nearVec "from a sequence" (vec 1. 2. 3.) (rs.CreateVector([ 1.; 2.; 3. ] : float seq))
    nearVec "from a string" (vec 1. 2. 3.) (rs.CreateVector("1, 2, 3"))
    let id = rs.AddLine(pt 1. 1. 1., pt 4. 5. 1.)
    nearVec "from a line: end minus start, the length is the length of the line" (vec 3. 4. 0.) (rs.CreateVector(id))
    throwsRs "sequence with two items" (fun () -> rs.CreateVector([ 1.; 2. ] : float seq))
    throwsRs "null sequence" (fun () -> rs.CreateVector(null : float seq))
    throwsRs "string that is no vector" (fun () -> rs.CreateVector("hello"))
    throwsRs "Guid that does not exist" (fun () -> rs.CreateVector(Guid.NewGuid()))
    let arc = rs.AddArc(Plane.WorldXY, 5., 90.)
    throwsRs "a curve that is not linear" (fun () -> rs.CreateVector(arc))
    )

test "CreatePlane" (fun () ->
    let p = rs.CreatePlane(pt 1. 2. 3.)
    nearPt "origin" (pt 1. 2. 3.) p.Origin
    nearVec "default x axis" Vector3d.XAxis p.XAxis
    nearVec "default y axis" Vector3d.YAxis p.YAxis
    let p2 = rs.CreatePlane(pt 0. 0. 0., vec 0. 1. 0., vec -1. 0. 0.)
    nearVec "custom x axis" Vector3d.YAxis p2.XAxis
    nearVec "custom y axis" (-Vector3d.XAxis) p2.YAxis
    nearVec "normal of the custom plane" Vector3d.ZAxis p2.Normal
    )

test "CreateXform" (fun () ->
    let m = [ [ 1.; 0.; 0.; 5. ]; [ 0.; 1.; 0.; 6. ]; [ 0.; 0.; 1.; 7. ]; [ 0.; 0.; 0.; 1. ] ]
    let x = rs.CreateXform m
    near "row 0, last column" 5. x.[0, 3]
    nearPt "translates a point" (pt 5. 6. 7.) (x * Point3d.Origin)
    let x2 = rs.CreateXform [| [| 2.; 0.; 0.; 0. |]; [| 0.; 2.; 0.; 0. |]; [| 0.; 0.; 2.; 0. |]; [| 0.; 0.; 0.; 1. |] |]
    nearPt "arrays work too, scale by two" (pt 2. 4. 6.) (x2 * pt 1. 2. 3.)
    throws "only three rows" (fun () -> rs.CreateXform [ [ 1.; 0.; 0.; 0. ]; [ 0.; 1.; 0.; 0. ]; [ 0.; 0.; 1.; 0. ] ])
    throws "row with three numbers" (fun () -> rs.CreateXform [ [ 1.; 0.; 0. ]; [ 0.; 1.; 0.; 0. ]; [ 0.; 0.; 1.; 0. ]; [ 0.; 0.; 0.; 1. ] ])
    )

test "CreateColor" (fun () ->
    let c = rs.CreateColor(10, 20, 30)
    eq "int values" (10, 20, 30) (int c.R, int c.G, int c.B)
    let c2 = rs.CreateColor(10uy, 20uy, 30uy)
    eq "byte values" (10, 20, 30) (int c2.R, int c2.G, int c2.B)
    let c3 = rs.CreateColor(0, 255, 0)
    eq "limits" (0, 255, 0) (int c3.R, int c3.G, int c3.B)
    throwsRs "red too big" (fun () -> rs.CreateColor(256, 0, 0))
    throwsRs "green negative" (fun () -> rs.CreateColor(0, -1, 0))
    throwsRs "blue too big" (fun () -> rs.CreateColor(0, 0, 300))
    )

test "CreateInterval" (fun () ->
    let i = rs.CreateInterval(1., 5.)
    near "T0" 1. i.T0
    near "T1" 5. i.T1
    let d = rs.CreateInterval(5., 1.)
    near "decreasing T0" 5. d.T0
    near "decreasing T1" 1. d.T1
    ok "a decreasing interval is decreasing" d.IsDecreasing
    )

skip "GetSettings" "commented out in the source, ini files are not supported"
skip "SaveSettings" "commented out in the source, ini files are not supported"

printfn "%s" (finish ())
