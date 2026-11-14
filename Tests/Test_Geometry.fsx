// Tests for Src/Scripting_Geometry.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Geometry"

let vec x y z = Vector3d(x, y, z)
let rgb (c:Drawing.Color) = (int c.R, int c.G, int c.B)
let geo (id:Guid) = (doc().Objects.FindId id).Geometry
let text1 () = rs.AddText("hello", Plane.WorldXY, 2.)
let dot1 () = rs.AddTextDot("dot", pt 1. 2. 3.)
let cloud1 () = rs.AddPointCloud [| pt 0. 0. 0.; pt 1. 0. 0.; pt 2. 0. 0. |]
let tempDir =
    let d = IO.Path.Combine(IO.Path.GetTempPath(), "RhinoScriptingGeometryTest")
    IO.Directory.CreateDirectory d |> ignore
    d

// ------------------------------- points -------------------------------

test "AddPoint" (fun () ->
    let a = rs.AddPoint(1., 2., 3.)
    ok "is a point" (rs.IsPoint a)
    nearPt "coordinates" (pt 1. 2. 3.) (rs.PointCoordinates a)
    let b = rs.AddPoint(pt 4. 5. 6.)
    nearPt "from a Point3d" (pt 4. 5. 6.) (rs.PointCoordinates b)
    )

test "AddPoints" (fun () ->
    let ids = rs.AddPoints [ pt 0. 0. 0.; pt 1. 1. 1.; pt 2. 2. 2. ]
    eq "one id per point" 3 ids.Count
    nearPt "second point" (pt 1. 1. 1.) (rs.PointCoordinates ids.[1])
    eq "empty list" 0 (rs.AddPoints []).Count
    )

test "AddPointCloud" (fun () ->
    let id = rs.AddPointCloud [| pt 0. 0. 0.; pt 1. 0. 0. |]
    ok "is a point cloud" (rs.IsPointCloud id)
    eq "two points" 2 (rs.PointCloudCount id)
    ok "no colors" (not (rs.PointCloudHasPointColors id))
    let colored = rs.AddPointCloud([| pt 0. 0. 0.; pt 1. 0. 0. |], [| Drawing.Color.Red; Drawing.Color.Blue |])
    ok "colors" (rs.PointCloudHasPointColors colored)
    eq "first color" (255, 0, 0) (rgb (rs.PointCloudPointColors colored).[0])
    let mismatch = rs.AddPointCloud([| pt 0. 0. 0.; pt 1. 0. 0. |], [| Drawing.Color.Red |])
    ok "a different number of colors: the colors are ignored" (not (rs.PointCloudHasPointColors mismatch))
    )

test "PointCoordinates" (fun () ->
    let a = rs.AddPoint(1., 2., 3.)
    nearPt "get by id" (pt 1. 2. 3.) (rs.PointCoordinates a)
    rs.PointCoordinates(a, pt 7. 8. 9.)
    nearPt "set by id" (pt 7. 8. 9.) (rs.PointCoordinates a)
    nearPt "from a Point3d" (pt 1. 1. 1.) (rs.PointCoordinates (pt 1. 1. 1.))
    nearPt "from a Point geometry" (pt 7. 8. 9.) (rs.PointCoordinates (geo a :?> Point))
    throwsRs "not a point, get" (fun () -> rs.PointCoordinates (addCircle 1.))
    throwsRs "not a point, set" (fun () -> rs.PointCoordinates(addCircle 1., pt 0. 0. 0.))
    )

test "IsPoint" (fun () ->
    let p = rs.AddPoint(0., 0., 0.)
    ok "point" (rs.IsPoint p)
    ok "point geometry" (rs.IsPoint (geo p))
    ok "curve is no point" (not (rs.IsPoint (addCircle 1.)))
    ok "curve geometry is no point" (not (rs.IsPoint (geo (addCircle 1.))))
    ok "unknown id" (not (rs.IsPoint (Guid.NewGuid())))
    )

test "IsPointCloud" (fun () ->
    let c = cloud1 ()
    ok "point cloud" (rs.IsPointCloud c)
    ok "point cloud geometry" (rs.IsPointCloud (geo c))
    ok "point is no point cloud" (not (rs.IsPointCloud (rs.AddPoint(0., 0., 0.))))
    ok "unknown id" (not (rs.IsPointCloud (Guid.NewGuid())))
    )

test "PointCloudCount" (fun () ->
    let c = cloud1 ()
    eq "by id" 3 (rs.PointCloudCount c)
    eq "by geometry" 3 (rs.PointCloudCount (geo c :?> PointCloud))
    throwsRs "not a point cloud" (fun () -> rs.PointCloudCount (rs.AddPoint(0., 0., 0.)))
    )

test "PointCloudPoints" (fun () ->
    let c = cloud1 ()
    let pts = rs.PointCloudPoints c
    eq "three points" 3 pts.Length
    nearPt "last point" (pt 2. 0. 0.) pts.[2]
    eq "by geometry" 3 (rs.PointCloudPoints (geo c :?> PointCloud)).Length
    throwsRs "not a point cloud" (fun () -> rs.PointCloudPoints (rs.AddPoint(0., 0., 0.)))
    )

test "PointCloudHasPointColors" (fun () ->
    let c = cloud1 ()
    ok "no colors" (not (rs.PointCloudHasPointColors c))
    rs.PointCloudPointColors(c, [ Drawing.Color.Red; Drawing.Color.Green; Drawing.Color.Blue ])
    ok "colors" (rs.PointCloudHasPointColors c)
    ok "by geometry" (rs.PointCloudHasPointColors (geo c :?> PointCloud))
    throwsRs "not a point cloud" (fun () -> rs.PointCloudHasPointColors (rs.AddPoint(0., 0., 0.)))
    )

test "PointCloudPointColors" (fun () ->
    let c = cloud1 ()
    rs.PointCloudPointColors(c, [ Drawing.Color.Red; Drawing.Color.FromArgb(0, 255, 0); Drawing.Color.Blue ])
    let colors = rs.PointCloudPointColors c
    eq "three colors" 3 colors.Count
    eq "second color" (0, 255, 0) (rgb colors.[1])
    eq "by geometry" 3 (rs.PointCloudPointColors (geo c :?> PointCloud)).Count
    rs.PointCloudPointColors(c, [])
    ok "an empty list clears the colors" (not (rs.PointCloudHasPointColors c))
    throwsRs "wrong number of colors" (fun () -> rs.PointCloudPointColors(c, [ Drawing.Color.Red ]))
    throwsRs "not a point cloud" (fun () -> rs.PointCloudPointColors (rs.AddPoint(0., 0., 0.)))
    )

test "PointCloudHasHiddenPoints" (fun () ->
    let c = cloud1 ()
    ok "none hidden" (not (rs.PointCloudHasHiddenPoints c))
    rs.PointCloudHidePoints(c, [ false; true; false ])
    ok "one hidden" (rs.PointCloudHasHiddenPoints c)
    ok "by geometry" (rs.PointCloudHasHiddenPoints (geo c :?> PointCloud))
    throwsRs "not a point cloud" (fun () -> rs.PointCloudHasHiddenPoints (rs.AddPoint(0., 0., 0.)))
    )

test "PointCloudHidePoints" (fun () ->
    let c = cloud1 ()
    eq "nothing hidden at first" [ false; false; false ] (List.ofSeq (rs.PointCloudHidePoints c))
    rs.PointCloudHidePoints(c, [ true; false; true ])
    eq "hidden flags" [ true; false; true ] (List.ofSeq (rs.PointCloudHidePoints c))
    eq "by geometry" [ true; false; true ] (List.ofSeq (rs.PointCloudHidePoints (geo c :?> PointCloud)))
    rs.PointCloudHidePoints(c, [])
    eq "an empty list shows all points" [ false; false; false ] (List.ofSeq (rs.PointCloudHidePoints c))
    throwsRs "wrong number of flags" (fun () -> rs.PointCloudHidePoints(c, [ true ]))
    throwsRs "not a point cloud" (fun () -> rs.PointCloudHidePoints (rs.AddPoint(0., 0., 0.)))
    )

test "PointCloudKNeighbors" (fun () ->
    let cloud = [ pt 0. 0. 0.; pt 1. 0. 0.; pt 5. 0. 0.; pt 10. 0. 0. ]
    let r = rs.PointCloudKNeighbors(cloud, [ pt 0.9 0. 0. ]) |> Seq.toArray
    eq "one needle gives one result" 1 r.Length
    eq "nearest neighbor is the second point" [| 1 |] r.[0]
    let r2 = rs.PointCloudKNeighbors(cloud, [ pt 0.9 0. 0. ], 2) |> Seq.toArray
    eq "two neighbors" 2 r2.[0].Length
    eq "they are the first two points" [ 0; 1 ] (r2.[0] |> Array.sort |> List.ofArray)
    // more than 100 needle points use another algorithm (RTree)
    let needles = [ for i in 0 .. 120 -> pt (float i * 0.1) 0. 0. ]
    let many = rs.PointCloudKNeighbors(cloud, needles) |> Seq.toArray
    eq "one result for every needle" 121 many.Length
    eq "the first needle is closest to the first point" [| 0 |] many.[0]
    )

test "PointCloudClosestPoints" (fun () ->
    let cloud = [ pt 0. 0. 0.; pt 1. 0. 0.; pt 5. 0. 0. ]
    let r = rs.PointCloudClosestPoints(cloud, [ pt 0.5 0. 0.; pt 100. 0. 0. ], 1.) |> Seq.toArray
    eq "one result per needle" 2 r.Length
    eq "two points within 1 unit of the first needle" [ 0; 1 ] (r.[0] |> Array.sort |> List.ofArray)
    eq "no points near the second needle" 0 r.[1].Length
    )

// ------------------------------- text dots -------------------------------

test "AddTextDot" (fun () ->
    let d = rs.AddTextDot("hello", pt 1. 2. 3.)
    ok "is a text dot" (rs.IsTextDot d)
    eq "text" "hello" (rs.TextDotText d)
    nearPt "location" (pt 1. 2. 3.) (rs.TextDotPoint d)
    let d2 = rs.AddTextDot("xyz", 4., 5., 6.)
    nearPt "from coordinates" (pt 4. 5. 6.) (rs.TextDotPoint d2)
    )

test "IsTextDot" (fun () ->
    let d = dot1 ()
    ok "text dot" (rs.IsTextDot d)
    ok "text dot geometry" (rs.IsTextDot (geo d))
    ok "point is no text dot" (not (rs.IsTextDot (rs.AddPoint(0., 0., 0.))))
    ok "unknown id" (not (rs.IsTextDot (Guid.NewGuid())))
    )

test "TextDotText" (fun () ->
    let a = dot1 ()
    let b = dot1 ()
    eq "get" "dot" (rs.TextDotText a)
    rs.TextDotText(a, "changed")
    eq "set" "changed" (rs.TextDotText a)
    eq "by geometry" "changed" (rs.TextDotText (geo a :?> TextDot))
    rs.TextDotText([ a; b ], "both")
    eq "multiple: first" "both" (rs.TextDotText a)
    eq "multiple: second" "both" (rs.TextDotText b)
    throwsRs "not a text dot" (fun () -> rs.TextDotText (rs.AddPoint(0., 0., 0.)))
    )

test "TextDotPoint" (fun () ->
    let a = dot1 ()
    nearPt "get" (pt 1. 2. 3.) (rs.TextDotPoint a)
    rs.TextDotPoint(a, pt 9. 8. 7.)
    nearPt "set" (pt 9. 8. 7.) (rs.TextDotPoint a)
    nearPt "by geometry" (pt 9. 8. 7.) (rs.TextDotPoint (geo a :?> TextDot))
    throwsRs "not a text dot" (fun () -> rs.TextDotPoint (rs.AddPoint(0., 0., 0.)))
    )

test "TextDotFont" (fun () ->
    let a = dot1 ()
    let b = dot1 ()
    rs.TextDotFont(a, "Arial")
    eq "set and get" "Arial" (rs.TextDotFont a)
    eq "by geometry" "Arial" (rs.TextDotFont (geo a :?> TextDot))
    rs.TextDotFont([ a; b ], "Courier New")
    eq "multiple: first" "Courier New" (rs.TextDotFont a)
    eq "multiple: second" "Courier New" (rs.TextDotFont b)
    throwsRs "not a text dot" (fun () -> rs.TextDotFont (rs.AddPoint(0., 0., 0.)))
    )

test "TextDotHeight" (fun () ->
    let a = dot1 ()
    let b = dot1 ()
    rs.TextDotHeight(a, 18)
    eq "set and get" 18 (rs.TextDotHeight a)
    eq "by geometry" 18 (rs.TextDotHeight (geo a :?> TextDot))
    rs.TextDotHeight([ a; b ], 24)
    eq "multiple: first" 24 (rs.TextDotHeight a)
    eq "multiple: second" 24 (rs.TextDotHeight b)
    throwsRs "not a text dot" (fun () -> rs.TextDotHeight (rs.AddPoint(0., 0., 0.)))
    )

// ------------------------------- text objects -------------------------------

test "AddText" (fun () ->
    let t = rs.AddText("hello", Plane.WorldXY, 2.)
    ok "is a text object" (rs.IsText t)
    eq "text" "hello" (rs.TextObjectText t)
    near "height" 2. (rs.TextObjectHeight t)
    let atPoint = rs.AddText("here", pt 5. 6. 7., 3.)
    nearPt "text at a point: origin" (pt 5. 6. 7.) (rs.TextObjectPoint atPoint)
    nearVec "text at a point is parallel to the XY plane" Vector3d.ZAxis (rs.TextObjectPlane atPoint).Normal
    let aligned = rs.AddText("aligned", Plane.WorldXY, 1., null, 0, 0uy, 0uy)
    ok "alignments" (rs.IsText aligned)
    throwsRs "empty text" (fun () -> rs.AddText("", Plane.WorldXY))
    throwsRs "null text" (fun () -> rs.AddText(null, Plane.WorldXY))
    )

test "AddText (font style)" (fun () ->
    for style, name in [ 1, "bold"; 2, "italic"; 3, "bold and italic" ] do
        let t = rs.AddText("styled", Plane.WorldXY, 1., "Arial", style)
        eq (sprintf "the font style %d (%s) is applied" style name) style (rs.TextObjectStyle t)
    )

test "IsText" (fun () ->
    let t = text1 ()
    ok "text" (rs.IsText t)
    ok "text geometry" (rs.IsText (geo t))
    ok "text dot is no text object" (not (rs.IsText (dot1 ())))
    ok "unknown id" (not (rs.IsText (Guid.NewGuid())))
    )

test "TextObjectText" (fun () ->
    let a = text1 ()
    let b = text1 ()
    eq "get" "hello" (rs.TextObjectText a)
    rs.TextObjectText(a, "changed")
    eq "set" "changed" (rs.TextObjectText a)
    eq "by geometry" "changed" (rs.TextObjectText (geo a :?> TextEntity))
    rs.TextObjectText([ a; b ], "both")
    eq "multiple: first" "both" (rs.TextObjectText a)
    eq "multiple: second" "both" (rs.TextObjectText b)
    throwsRs "not a text object" (fun () -> rs.TextObjectText (rs.AddPoint(0., 0., 0.)))
    )

test "TextObjectRichText" (fun () ->
    let a = text1 ()
    let b = text1 ()
    let rtf = rs.TextObjectRichText a
    ok "rich text is a string" (not (String.IsNullOrEmpty rtf))
    rs.TextObjectRichText(a, rtf)
    eq "round trip" "hello" (rs.TextObjectText a)
    let styleName = rs.AddDimStyle "richStyle"
    rs.TextObjectRichText(a, rtf, styleName)
    eq "with a dimension style" "hello" (rs.TextObjectText a)
    rs.TextObjectRichText([ a; b ], rtf)
    rs.TextObjectRichText([ a; b ], rtf, styleName)
    ok "by geometry" (not (String.IsNullOrEmpty (rs.TextObjectRichText (geo a :?> TextEntity))))
    throwsRs "unknown style" (fun () -> rs.TextObjectRichText(a, rtf, "no such style"))
    throwsRs "unknown style, multiple" (fun () -> rs.TextObjectRichText([ a; b ], rtf, "no such style"))
    throwsRs "not a text object" (fun () -> rs.TextObjectRichText (rs.AddPoint(0., 0., 0.)))
    )

test "TextObjectHeight" (fun () ->
    let a = text1 ()
    let b = text1 ()
    near "get" 2. (rs.TextObjectHeight a)
    rs.TextObjectHeight(a, 5.)
    near "set" 5. (rs.TextObjectHeight a)
    near "by geometry" 5. (rs.TextObjectHeight (geo a :?> TextEntity))
    rs.TextObjectHeight([ a; b ], 7.)
    near "multiple: first" 7. (rs.TextObjectHeight a)
    near "multiple: second" 7. (rs.TextObjectHeight b)
    throwsRs "not a text object" (fun () -> rs.TextObjectHeight (rs.AddPoint(0., 0., 0.)))
    )

test "TextObjectFont" (fun () ->
    let a = text1 ()
    let b = text1 ()
    rs.TextObjectFont(a, "Arial")
    ok "set and get: the font name contains Arial" ((rs.TextObjectFont a).Contains "Arial")
    ok "by geometry" ((rs.TextObjectFont (geo a :?> TextEntity)).Contains "Arial")
    rs.TextObjectFont([ a; b ], "Courier New")
    ok "multiple: first" ((rs.TextObjectFont a).Contains "Courier")
    ok "multiple: second" ((rs.TextObjectFont b).Contains "Courier")
    throwsRs "font does not exist" (fun () -> rs.TextObjectFont(a, "No Such Font 123"))
    throwsRs "not a text object" (fun () -> rs.TextObjectFont (rs.AddPoint(0., 0., 0.)))
    )

test "TextObjectStyle" (fun () ->
    let a = rs.AddText("x", Plane.WorldXY, 1., "Arial")
    let b = rs.AddText("y", Plane.WorldXY, 1., "Arial")
    eq "normal at first" 0 (rs.TextObjectStyle a)
    for style in [ 1; 2; 3; 0 ] do
        rs.TextObjectStyle(a, style)
        eq (sprintf "style %d" style) style (rs.TextObjectStyle a)
    rs.TextObjectStyle(a, 3)
    eq "by geometry" 3 (rs.TextObjectStyle (geo a :?> TextEntity))
    rs.TextObjectStyle([ a; b ], 1)
    eq "multiple: first" 1 (rs.TextObjectStyle a)
    eq "multiple: second" 1 (rs.TextObjectStyle b)
    throwsRs "invalid style" (fun () -> rs.TextObjectStyle(a, 4))
    throwsRs "negative style" (fun () -> rs.TextObjectStyle(a, -1))
    throwsRs "not a text object" (fun () -> rs.TextObjectStyle (rs.AddPoint(0., 0., 0.)))
    )

test "TextObjectPlane" (fun () ->
    let a = text1 ()
    nearVec "get: normal" Vector3d.ZAxis (rs.TextObjectPlane a).Normal
    let p = Plane(pt 1. 2. 3., Vector3d.XAxis, Vector3d.ZAxis)
    rs.TextObjectPlane(a, p)
    nearVec "set: normal" (-Vector3d.YAxis) (rs.TextObjectPlane a).Normal
    nearPt "set: origin" (pt 1. 2. 3.) (rs.TextObjectPlane a).Origin
    nearPt "by geometry" (pt 1. 2. 3.) (rs.TextObjectPlane (geo a :?> TextEntity)).Origin
    throwsRs "not a text object" (fun () -> rs.TextObjectPlane (rs.AddPoint(0., 0., 0.)))
    )

test "TextObjectPoint" (fun () ->
    let a = text1 ()
    nearPt "get" Point3d.Origin (rs.TextObjectPoint a)
    rs.TextObjectPoint(a, pt 4. 5. 6.)
    nearPt "set" (pt 4. 5. 6.) (rs.TextObjectPoint a)
    nearVec "the orientation is kept" Vector3d.ZAxis (rs.TextObjectPlane a).Normal
    nearPt "by geometry" (pt 4. 5. 6.) (rs.TextObjectPoint (geo a :?> TextEntity))
    throwsRs "not a text object" (fun () -> rs.TextObjectPoint (rs.AddPoint(0., 0., 0.)))
    )

test "ExplodeText" (fun () ->
    let t = rs.AddText("HI", Plane.WorldXY, 2., "Arial")
    let n = (rs.AllObjects()).Count
    let curves = rs.ExplodeText t
    ok "outline curves were added" (curves.Count > 0)
    ok "they are curves" (curves |> Seq.forall (fun c -> rs.IsCurve c))
    eq "the text is still there" true (rs.IsText t)
    eq "the document has the new curves" (n + curves.Count) (rs.AllObjects()).Count
    let curves2 = rs.ExplodeText(t, true)
    ok "curves again" (curves2.Count > 0)
    ok "the text was deleted" (not (rs.IsText t))
    // geometry overload
    let g = new TextEntity()
    let te = geo (rs.AddText("AB", Plane.WorldXY, 2., "Arial")) :?> TextEntity
    let outlines = rs.ExplodeText te
    ok "outline geometry" (outlines.Count > 0)
    throwsRs "not a text" (fun () -> rs.ExplodeText (rs.AddPoint(0., 0., 0.)))
    )

// ------------------------------- clipping planes and pictures -------------------------------

test "AddClippingPlane" (fun () ->
    let c = rs.AddClippingPlane(Plane.WorldXY, 10., 10.)
    ok "is a clipping plane" (rs.IsClippingPlane c)
    let c2 = rs.AddClippingPlane(Plane.WorldXY, 5., 5., [ "Top"; "Front" ])
    ok "for named views" (rs.IsClippingPlane c2)
    throwsRs "no matching view" (fun () -> rs.AddClippingPlane(Plane.WorldXY, 5., 5., [ "no such view" ]))
    )

test "IsClippingPlane" (fun () ->
    let c = rs.AddClippingPlane(Plane.WorldXY, 10., 10.)
    ok "clipping plane" (rs.IsClippingPlane c)
    ok "clipping plane geometry" (rs.IsClippingPlane (geo c))
    ok "point is no clipping plane" (not (rs.IsClippingPlane (rs.AddPoint(0., 0., 0.))))
    ok "unknown id" (not (rs.IsClippingPlane (Guid.NewGuid())))
    )

test "AddPictureFrame" (fun () ->
    let file = IO.Path.Combine(tempDir, "pic.png")
    (use bmp = new Drawing.Bitmap(40, 20) in bmp.Save(file, Drawing.Imaging.ImageFormat.Png))
    let p = rs.AddPictureFrame(Plane.WorldXY, file)
    ok "picture frame was added" (p <> Guid.Empty)
    let b = rs.BoundingBox p
    near "default size: width of the image" 40. (b.Max.X - b.Min.X)
    near "default size: height of the image" 20. (b.Max.Y - b.Min.Y)
    let w = rs.AddPictureFrame(Plane.WorldXY, file, 10.)
    near "only the width given: height is scaled" 5. (let bb = rs.BoundingBox w in bb.Max.Y - bb.Min.Y)
    let both = rs.AddPictureFrame(Plane.WorldXY, file, 8., 6.)
    near "width and height" 8. (let bb = rs.BoundingBox both in bb.Max.X - bb.Min.X)
    ok "as a mesh" (rs.IsMesh (rs.AddPictureFrame(Plane.WorldXY, file, 0., 0., true, false, false, true)))
    ok "embedded and with alpha" (rs.AddPictureFrame(Plane.WorldXY, file, 0., 0., false, true, true) <> Guid.Empty)
    throwsRs "file does not exist" (fun () -> rs.AddPictureFrame(Plane.WorldXY, IO.Path.Combine(tempDir, "nothere.png")))
    )

// ------------------------------- measuring -------------------------------

test "Area" (fun () ->
    near "closed polyline square" 100. (rs.Area (addSquare 10.))
    near "circle" (Math.PI * 25.) (rs.Area (addCircle 5.))
    near "planar surface" 6. (rs.Area (addPlaneSurface 2. 3.))
    near "box" 6. (rs.Area (addBox 1. 1. 1.))
    near "mesh box" 6. (rs.Area (addMeshBox 1. 1. 1.))
    near "geometry overload" 100. (rs.Area (geo (addSquare 10.)))
    rs.CurrentHatchPattern "Solid"
    near "hatch" 100. (rs.Area (rs.AddHatch (addSquare 10.)))
    throws "open curve" (fun () -> rs.Area (rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.)))
    throws "unknown id" (fun () -> rs.Area (Guid.NewGuid()))
    )

test "BoundingBox" (fun () ->
    let box = addBox 2. 3. 4.
    let b = rs.BoundingBox box
    nearPt "min" (pt 0. 0. 0.) b.Min
    nearPt "max" (pt 2. 3. 4.) b.Max
    let line = rs.AddLine(pt -5. 0. 0., pt 0. 0. 8.)
    let both = rs.BoundingBox [ box; line ]
    nearPt "several objects: min" (pt -5. 0. 0.) both.Min
    nearPt "several objects: max" (pt 2. 3. 8.) both.Max
    nearPt "geometry" (pt 2. 3. 4.) (rs.BoundingBox (geo box)).Max
    nearPt "several geometries" (pt 2. 3. 8.) (rs.BoundingBox [ geo box; geo line ]).Max
    // aligned to a plane
    let pl = Plane(Point3d.Origin, vec 1. 1. 0., vec -1. 1. 0.)
    let sq = rs.AddPolyline [ pt 0. 0. 0.; pt 1. 1. 0.; pt 0. 2. 0.; pt -1. 1. 0.; pt 0. 0. 0. ]
    let world = rs.BoundingBox(sq, pl, true)
    near "oriented box in world coordinates: x length" (sqrt 2.) world.X.Length
    near "oriented box in world coordinates: y length" (sqrt 2.) world.Y.Length
    let local = rs.BoundingBox(sq, pl, false)
    near "oriented box in plane coordinates: x length" (sqrt 2.) local.X.Length
    nearPt "oriented box in plane coordinates: center" (pt (sqrt 2. / 2.) (sqrt 2. / 2.) 0.) local.Center
    nearPt "several objects with a plane" (pt 0. 0. 0.) (rs.BoundingBox([ sq ], pl, true)).Plane.Origin |> ignore
    ok "a geometry with a plane" ((rs.BoundingBox(geo sq, pl)).IsValid)
    ok "several geometries with a plane" ((rs.BoundingBox([ geo sq ], pl, false)).IsValid)
    throwsRs "invalid plane" (fun () -> rs.BoundingBox(sq, Plane(), true))
    throws "unknown id" (fun () -> rs.BoundingBox (Guid.NewGuid()))
    )

test "BoundingBoxEstimate" (fun () ->
    let box = addBox 2. 3. 4.
    let b = rs.BoundingBoxEstimate box
    ok "the estimate contains the exact box" (b.Min.X <= 0. + 1e-9 && b.Max.X >= 2. - 1e-9 && b.Max.Z >= 4. - 1e-9)
    let line = rs.AddLine(pt -5. 0. 0., pt 0. 0. 8.)
    let both = rs.BoundingBoxEstimate [ box; line ]
    ok "several objects" (both.Min.X <= -5. + 1e-9 && both.Max.Z >= 8. - 1e-9)
    ok "geometry" ((rs.BoundingBoxEstimate (geo box)).IsValid)
    ok "several geometries" ((rs.BoundingBoxEstimate [ geo box; geo line ]).IsValid)
    throws "unknown id" (fun () -> rs.BoundingBoxEstimate (Guid.NewGuid()))
    )

test "BoundingBoxInflate" (fun () ->
    let b = BoundingBox(pt 0. 0. 0., pt 1. 1. 1.)
    let i = rs.BoundingBoxInflate(b, 2.)
    nearPt "min" (pt -2. -2. -2.) i.Min
    nearPt "max" (pt 3. 3. 3.) i.Max
    nearPt "input is unchanged" (pt 1. 1. 1.) b.Max
    let xyz = rs.BoundingBoxInflate(b, 1., 2., 3.)
    nearPt "min with xyz" (pt -1. -2. -3.) xyz.Min
    nearPt "max with xyz" (pt 2. 3. 4.) xyz.Max
    throwsRs "a negative amount that makes the box invalid" (fun () -> rs.BoundingBoxInflate(b, -5.))
    throwsRs "a negative xyz amount that makes the box invalid" (fun () -> rs.BoundingBoxInflate(b, -5., 0., 0.))
    )

test "CompareGeometry" (fun () ->
    let a = addCircle 5.
    let b = addCircle 5.
    let c = addCircle 6.
    ok "identical geometry" (rs.CompareGeometry(a, b))
    ok "different geometry" (not (rs.CompareGeometry(a, c)))
    ok "geometry overload, identical" (rs.CompareGeometry(geo a, geo b))
    ok "geometry overload, different" (not (rs.CompareGeometry(geo a, geo c)))
    ok "a curve and a point" (not (rs.CompareGeometry(a, rs.AddPoint(0., 0., 0.))))
    throws "unknown id" (fun () -> rs.CompareGeometry(a, Guid.NewGuid()))
    )

try IO.Directory.Delete(tempDir, true) with _ -> ()

printfn "%s" (finish ())
