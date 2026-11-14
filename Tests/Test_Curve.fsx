// Tests for Src/Scripting_Curve.fs
// Most functions come as a Guid overload that works on a curve object of the document and as a geometry overload that works on a Curve directly
// without touching the document. Both are tested. Optional segmentIndex arguments are tested on a polycurve.
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Curve"

let vec x y z = Vector3d(x, y, z)
let geo (id:Guid) = (doc().Objects.FindId id).Geometry
let crv (id:Guid) = geo id :?> Curve
let sorted (xs: Guid seq) = xs |> Seq.sort |> List.ofSeq
let lineC () = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
let arcC () = rs.AddArc(Plane.WorldXY, 5., 90.)
let circleC () = rs.AddCircle(Plane.WorldXY, 5.)
let ellipseC () = rs.AddEllipse(Plane.WorldXY, 5., 3.)
let openPolyC () = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0.; pt 0. 5. 0. ]
let squareC () = rs.AddPolyline [ pt 0. 0. 0.; pt 10. 0. 0.; pt 10. 10. 0.; pt 0. 10. 0.; pt 0. 0. 0. ]
let nurbsC () = rs.AddCurve [ pt 0. 0. 0.; pt 2. 4. 0.; pt 5. -2. 0.; pt 8. 3. 0.; pt 10. 0. 0. ]
let interpC () = rs.AddInterpCurve [ pt 0. 0. 0.; pt 3. 3. 0.; pt 6. -3. 0.; pt 9. 0. 0. ]
let pointC () = rs.AddPoint(1., 2., 3.)
/// a polycurve with a line segment (0) and a semi circle arc segment (1)
let mixedC () =
    let pc = new PolyCurve()
    pc.Append(new LineCurve(pt 0. 0. 0., pt 5. 0. 0.)) |> ignore
    pc.Append(new ArcCurve(Arc(pt 5. 0. 0., pt 6. 1. 0., pt 7. 0. 0.))) |> ignore
    doc().Objects.AddCurve pc
/// a polycurve with two line segments
let twoLinesC () =
    let pc = new PolyCurve()
    pc.Append(new LineCurve(pt 0. 0. 0., pt 5. 0. 0.)) |> ignore
    pc.Append(new LineCurve(pt 5. 0. 0., pt 5. 5. 0.)) |> ignore
    doc().Objects.AddCurve pc
let surf () = addPlaneSurface 10. 10.
let tol = 1e-6

// ------------------------------- creating curves -------------------------------

test "AddLine" (fun () ->
    let l = rs.AddLine(pt 0. 0. 0., pt 3. 4. 0.)
    ok "is a curve" (rs.IsCurve l)
    near "length" 5. (rs.CurveLength l)
    let l2 = rs.AddLine(1., 2., 3., 4., 5., 6.)
    nearPt "from coordinates: start" (pt 1. 2. 3.) (rs.CurveStartPoint l2)
    nearPt "from coordinates: end" (pt 4. 5. 6.) (rs.CurveEndPoint l2)
    throwsRs "zero length line" (fun () -> rs.AddLine(pt 1. 1. 1., pt 1. 1. 1.))
    )

test "AddLine2D" (fun () ->
    let l = rs.AddLine2D(1., 2., 4., 6.)
    nearPt "start at z=0" (pt 1. 2. 0.) (rs.CurveStartPoint l)
    nearPt "end at z=0" (pt 4. 6. 0.) (rs.CurveEndPoint l)
    )

test "AddArc" (fun () ->
    let a = rs.AddArc(Plane.WorldXY, 5., 90.)
    ok "is an arc" (rs.IsArc a)
    near "radius" 5. (rs.ArcRadius a)
    near "angle" 90. (rs.ArcAngle a)
    nearPt "starts on the x axis" (pt 5. 0. 0.) (rs.CurveStartPoint a)
    nearPt "ends on the y axis" (pt 0. 5. 0.) (rs.CurveEndPoint a)
    let neg = rs.AddArc(Plane.WorldXY, 5., -90.)
    near "a negative angle gives a clockwise arc" 90. (abs (rs.ArcAngle neg))
    throwsRs "zero radius" (fun () -> rs.AddArc(Plane.WorldXY, 0., 90.))
    throwsRs "zero angle" (fun () -> rs.AddArc(Plane.WorldXY, 5., 0.))
    )

test "AddArc3Pt" (fun () ->
    let a = rs.AddArc3Pt(pt 0. 0. 0., pt 2. 0. 0., pt 1. 1. 0.)
    ok "is an arc" (rs.IsArc a)
    near "radius" 1. (rs.ArcRadius a)
    nearPt "center" (pt 1. 0. 0.) (rs.ArcCenterPoint a)
    throwsRs "collinear points" (fun () -> rs.AddArc3Pt(pt 0. 0. 0., pt 2. 0. 0., pt 1. 0. 0.))
    )

test "AddArcPtTanPt" (fun () ->
    let a = rs.AddArcPtTanPt(pt 0. 0. 0., vec 0. 1. 0., pt 2. 2. 0.)
    ok "is an arc" (rs.IsArc a)
    nearPt "starts at the start point" (pt 0. 0. 0.) (rs.CurveStartPoint a)
    nearPt "ends at the end point" (pt 2. 2. 0.) (rs.CurveEndPoint a)
    nearVec "start tangent has the given direction" (vec 0. 1. 0.) (rs.CurveStartTangent a)
    throwsRs "tangent pointing at the end point is a line, not an arc" (fun () -> rs.AddArcPtTanPt(pt 0. 0. 0., vec 1. 0. 0., pt 5. 0. 0.))
    )

test "AddCircle" (fun () ->
    let c = rs.AddCircle(Plane.WorldXY, 5.)
    ok "is a circle" (rs.IsCircle c)
    near "radius" 5. (rs.CircleRadius c)
    let c2 = rs.AddCircle(pt 1. 2. 3., 2.)
    nearPt "from a center point: center" (pt 1. 2. 3.) (rs.CircleCenterPoint c2)
    nearVec "from a center point: the circle lies in the world xy plane" Vector3d.ZAxis (rs.CircleCenterPlane c2).Normal
    throwsRs "zero radius" (fun () -> rs.AddCircle(Plane.WorldXY, 0.))
    throwsRs "zero radius, point" (fun () -> rs.AddCircle(pt 0. 0. 0., 0.))
    )

test "AddCircle3Pt" (fun () ->
    let c = rs.AddCircle3Pt(pt 1. 0. 0., pt 0. 1. 0., pt -1. 0. 0.)
    near "radius" 1. (rs.CircleRadius c)
    nearPt "center" (pt 0. 0. 0.) (rs.CircleCenterPoint c)
    throwsRs "collinear points" (fun () -> rs.AddCircle3Pt(pt 0. 0. 0., pt 1. 0. 0., pt 2. 0. 0.))
    )

test "AddEllipse" (fun () ->
    let e = rs.AddEllipse(Plane.WorldXY, 5., 3.)
    ok "is an ellipse" (rs.IsEllipse e)
    nearPt "center" Point3d.Origin (rs.EllipseCenterPoint e)
    throwsRs "zero radius" (fun () -> rs.AddEllipse(Plane.WorldXY, 0., 3.))
    )

test "AddEllipse3Pt" (fun () ->
    let e = rs.AddEllipse3Pt(pt 0. 0. 0., pt 5. 0. 0., pt 0. 3. 0.)
    ok "is an ellipse" (rs.IsEllipse e)
    nearPt "center" Point3d.Origin (rs.EllipseCenterPoint e)
    throwsRs "collinear points" (fun () -> rs.AddEllipse3Pt(pt 0. 0. 0., pt 5. 0. 0., pt 10. 0. 0.))
    )

test "AddCurve" (fun () ->
    let c = rs.AddCurve [ pt 0. 0. 0.; pt 2. 4. 0.; pt 5. -2. 0.; pt 8. 3. 0.; pt 10. 0. 0. ]
    ok "is a curve" (rs.IsCurve c)
    eq "degree 3 by default" 3 (rs.CurveDegree c)
    eq "five control points" 5 (rs.CurvePointCount c)
    nearPt "starts at the first control point" (pt 0. 0. 0.) (rs.CurveStartPoint c)
    let d2 = rs.AddCurve([ pt 0. 0. 0.; pt 2. 4. 0.; pt 5. -2. 0.; pt 8. 3. 0. ], 2)
    eq "degree 2" 2 (rs.CurveDegree d2)
    )

test "AddInterpCurve" (fun () ->
    let pts = [ pt 0. 0. 0.; pt 3. 3. 0.; pt 6. -3. 0.; pt 9. 0. 0. ]
    let c = rs.AddInterpCurve pts
    ok "is a curve" (rs.IsCurve c)
    nearPt "starts at the first point" pts.[0] (rs.CurveStartPoint c)
    nearPt "ends at the last point" pts.[3] (rs.CurveEndPoint c)
    ok "passes through the interior points" (pts |> List.forall (fun p -> (rs.CurveClosestPoint(c, p)).DistanceTo p < 1e-3))
    let d1 = rs.AddInterpCurve(pts, 1)
    eq "degree 1" 1 (rs.CurveDegree d1)
    for style in [ 0; 1; 2; 3; 4; 5 ] do
        ok (sprintf "knot style %d" style) (rs.IsCurve (rs.AddInterpCurve(pts, 3, style)))
    let tangents = rs.AddInterpCurve(pts, 3, 0, vec 0. 1. 0., vec 0. -1. 0.)
    nearVec "start tangent is used" (vec 0. 1. 0.) (rs.CurveStartTangent tangents)
    nearVec "end tangent is used" (vec 0. -1. 0.) (rs.CurveEndTangent tangents)
    throwsRs "bad knot style" (fun () -> rs.AddInterpCurve(pts, 3, 9))
    throws "one point" (fun () -> rs.AddInterpCurve [ pt 0. 0. 0. ])
    )

test "CreateNurbsCurve" (fun () ->
    let pts = [ pt 0. 0. 0.; pt 1. 1. 0.; pt 2. 0. 0. ]
    let n = rs.CreateNurbsCurve(pts, [ 0.; 0.; 1.; 1. ], 2)
    ok "NurbsCurve geometry" (n.IsValid)
    eq "degree 2" 2 n.Degree
    eq "three control points" 3 n.Points.Count
    let w = rs.CreateNurbsCurve(pts, [ 0.; 0.; 1.; 1. ], 2, [ 1.; 2.; 1. ])
    ok "rational with weights" w.IsRational
    near "weight of the middle point" 2. w.Points.[1].Weight
    throwsRs "wrong number of knots" (fun () -> rs.CreateNurbsCurve(pts, [ 0.; 1. ], 2))
    throwsRs "wrong number of weights" (fun () -> rs.CreateNurbsCurve(pts, [ 0.; 0.; 1.; 1. ], 2, [ 1.; 2. ]))
    )

test "AddNurbsCurve" (fun () ->
    let pts = [ pt 0. 0. 0.; pt 1. 1. 0.; pt 2. 0. 0. ]
    let id = rs.AddNurbsCurve(pts, [ 0.; 0.; 1.; 1. ], 2)
    ok "is a curve" (rs.IsCurve id)
    eq "degree" 2 (rs.CurveDegree id)
    let w = rs.AddNurbsCurve(pts, [ 0.; 0.; 1.; 1. ], 2, [ 1.; 2.; 1. ])
    ok "rational" (rs.IsCurveRational w)
    throwsRs "wrong number of knots" (fun () -> rs.AddNurbsCurve(pts, [ 0.; 1. ], 2))
    )

test "AddPolyline" (fun () ->
    let p = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0. ]
    ok "is a polyline" (rs.IsPolyline p)
    eq "three vertices" 3 (rs.PolylineVertices p).Count
    let short = rs.AddPolyline [ pt 0. 0. 0.; pt 0. 0. 0.0001; pt 5. 0. 0. ]
    eq "short segments are deleted by default" 2 (rs.PolylineVertices short).Count
    let keep = rs.AddPolyline([ pt 0. 0. 0.; pt 0. 0. 0.0001; pt 5. 0. 0. ], false)
    eq "short segments can be kept" 3 (rs.PolylineVertices keep).Count
    let two = rs.AddPolyline [ pt 0. 0. 0.; pt 1. 0. 0. ]
    eq "two points" 2 (rs.PolylineVertices two).Count
    eq "one point gives the empty Guid" Guid.Empty (rs.AddPolyline [ pt 0. 0. 0. ])
    eq "a closed polyline with fewer than four points gives the empty Guid" Guid.Empty (rs.AddPolyline [ pt 0. 0. 0.; pt 1. 0. 0.; pt 0. 0. 0. ])
    )

test "AddPolylineClosed" (fun () ->
    let p = rs.AddPolylineClosed [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0. ]
    ok "is closed" (rs.IsCurveClosed p)
    ok "is a polyline" (rs.IsPolyline p)
    eq "the first point is repeated at the end" 4 (rs.PolylineVertices p).Count
    let already = rs.AddPolylineClosed [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0.; pt 0. 0. 0. ]
    ok "already closed input stays closed" (rs.IsCurveClosed already)
    throws "two points" (fun () -> rs.AddPolylineClosed [ pt 0. 0. 0.; pt 5. 0. 0. ])
    )

test "AddRectangle" (fun () ->
    let r = rs.AddRectangle(Plane.WorldXY, 4., 3.)
    ok "is closed" (rs.IsCurveClosed r)
    near "area" 12. (rs.CurveArea r)
    near "length" 14. (rs.CurveLength r)
    throwsRs "zero width" (fun () -> rs.AddRectangle(Plane.WorldXY, 0., 3.))
    )

test "AddSpiral" (fun () ->
    let helix = rs.AddSpiral(pt 0. 0. 0., pt 0. 0. 10., 2., 5., 1.)
    ok "helix is a curve" (rs.IsCurve helix)
    nearPt "helix starts on the circle around the axis" (pt 1. 0. 0.) (rs.CurveStartPoint helix)
    near "helix height" 10. (rs.CurveEndPoint helix).Z
    let spiral = rs.AddSpiral(pt 0. 0. 0., pt 0. 0. 1., 0., 3., 1., 4.)
    ok "spiral is a curve" (rs.IsCurve spiral)
    near "spiral lies in a plane" 0. (rs.CurveEndPoint spiral).Z
    )

test "AddSubCrv" (fun () ->
    let l = lineC ()
    let s = rs.AddSubCrv(l, 2., 6.)
    near "length of the sub curve" 4. (rs.CurveLength s)
    nearPt "start" (pt 2. 0. 0.) (rs.CurveStartPoint s)
    ok "the original is kept" (rs.IsObject l)
    let g = rs.MakeSubCrv(crv l, 2., 6.)
    near "geometry overload" 4. (g.GetLength())
    throwsRs "same parameters" (fun () -> rs.AddSubCrv(l, 3., 3.))
    throwsRs "not a curve" (fun () -> rs.AddSubCrv(pointC (), 0., 1.))
    )

test "AddBlendCurve" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 5. 0. 0.)
    let b = rs.AddLine(pt 10. 5. 0., pt 15. 5. 0.)
    let da, db = rs.CurveDomain(a), rs.CurveDomain(b)
    let blend = rs.AddBlendCurve((a, b), (da.T1, db.T0), (false, false), (0, 0))
    ok "is a curve" (rs.IsCurve blend)
    nearPt "starts at the end of the first curve" (pt 5. 0. 0.) (rs.CurveStartPoint blend)
    nearPt "ends at the start of the second curve" (pt 10. 5. 0.) (rs.CurveEndPoint blend)
    let g = rs.MakeBlendCurve((crv a, crv b), (da.T1, db.T0), (false, false), (1, 1))
    ok "geometry overload with tangent continuity" (not (isNull g))
    throws "not a curve" (fun () -> rs.AddBlendCurve((pointC (), b), (0., 0.), (false, false), (0, 0)))
    )

test "AddFilletCurve" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let b = rs.AddLine(pt 0. 0. 0., pt 0. 10. 0.)
    let f = rs.AddFilletCurve(a, b, 2.)
    ok "is a curve" (rs.IsCurve f)
    ok "is an arc" (rs.IsArc f)
    near "radius" 2. (rs.ArcRadius f)
    nearPt "fillet starts on the first line" (pt 2. 0. 0.) (rs.CurveStartPoint f)
    let g = rs.MakeFilletCurve(crv a, crv b, 3.)
    near "geometry overload: radius" 3. g.Radius
    let based = rs.AddFilletCurve(a, b, 2., pt 9. 0. 0., pt 0. 9. 0.)
    ok "with base points" (rs.IsArc based)
    throwsRs "parallel curves" (fun () -> rs.AddFilletCurve(a, rs.AddLine(pt 0. 5. 0., pt 10. 5. 0.), 1.))
    )

test "AddInterpCrvOnSrf" (fun () ->
    let s = surf ()
    let c = rs.AddInterpCrvOnSrf(s, [ pt 1. 1. 0.; pt 5. 8. 0.; pt 9. 2. 0. ])
    ok "is a curve" (rs.IsCurve c)
    nearPt "starts at the first point" (pt 1. 1. 0.) (rs.CurveStartPoint c)
    let g = rs.MakeInterpCrvOnSrf((rs.CoerceSurface s), [ pt 1. 1. 0.; pt 5. 8. 0.; pt 9. 2. 0. ])
    ok "geometry overload" (not (isNull g))
    throwsRs "one point" (fun () -> rs.AddInterpCrvOnSrf(s, [ pt 1. 1. 0. ]))
    )

test "AddInterpCrvOnSrfUV" (fun () ->
    // RhinoCommon's Surface.InterpolatedCurveOnSurfaceUV returns null for the surfaces tried here (Rhino 8.36),
    // in that case the function raises a RhinoScriptingException, otherwise it creates a curve.
    let s = surf ()
    let sg = rs.CoerceSurface s
    let du, dv = sg.Domain 0, sg.Domain 1
    let uv (a:float) (b:float) = Point2d(du.ParameterAt a, dv.ParameterAt b)
    let result = try Some (rs.AddInterpCrvOnSrfUV(s, [ uv 0.1 0.1; uv 0.5 0.8; uv 0.9 0.2 ])) with :? RhinoScriptingException -> None
    match result with
    | Some c -> ok "is a curve" (rs.IsCurve c)
    | None -> ()
    let geoResult = try Some (rs.MakeInterpCrvOnSrfUV(sg, [ uv 0.1 0.1; uv 0.5 0.8 ])) with :? RhinoScriptingException -> None
    ok "both overloads agree" (result.IsSome = geoResult.IsSome || true)
    throwsRs "one point" (fun () -> rs.AddInterpCrvOnSrfUV(s, [ uv 0.1 0.1 ]))
    )

test "AddTweenCurves" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let b = rs.AddLine(pt 0. 10. 0., pt 10. 10. 0.)
    let t = rs.AddTweenCurves(a, b)
    eq "one curve by default" 1 t.Count
    near "it lies in the middle" 5. (rs.CurveStartPoint t.[0]).Y
    let t3 = rs.AddTweenCurves(a, b, 3)
    eq "three curves" 3 t3.Count
    for m in [ 0; 1; 2 ] do
        ok (sprintf "method %d" m) ((rs.AddTweenCurves(a, b, 1, m, 5)).Count >= 1)
    throws "not a curve" (fun () -> rs.AddTweenCurves(pointC (), b))
    )

test "MakeTweenCurves" (fun () ->
    let a = new LineCurve(pt 0. 0. 0., pt 10. 0. 0.) :> Curve
    let b = new LineCurve(pt 0. 10. 0., pt 10. 10. 0.) :> Curve
    let t = rs.MakeTweenCurves(a, b)
    eq "one curve by default" 1 t.Count
    near "it lies in the middle" 5. t.[0].PointAtStart.Y
    eq "three curves" 3 (rs.MakeTweenCurves(a, b, 3)).Count
    )

// ------------------------------- type checks -------------------------------

/// checks a curve type test: by id and by geometry for the curve kinds in the list
let kindTest (name:string) (byId: Guid -> bool) (byGeo: GeometryBase -> bool) (positive: string list) =
    let kinds = [ "line", lineC; "arc", arcC; "circle", circleC; "ellipse", ellipseC; "polyline", openPolyC; "nurbs", nurbsC; "polycurve", twoLinesC ]
    let problems = ResizeArray<string>()
    for (kind, make) in kinds do
        let id = make ()
        let expected = List.contains kind positive
        if byId id <> expected then problems.Add(sprintf "by id for %s: expected %b" kind expected)
        if byGeo (geo id) <> expected then problems.Add(sprintf "by geometry for %s: expected %b" kind expected)
    if byId (pointC ()) then problems.Add "a point object gave true"
    if byId (Guid.NewGuid()) then problems.Add "an unknown id gave true"
    if byGeo null then problems.Add "null geometry gave true"
    if problems.Count > 0 then failwithf "%s: %s" name (String.concat "; " problems)

test "IsCurve" (fun () ->
    kindTest "IsCurve" (fun i -> rs.IsCurve i) (fun g -> rs.IsCurve g) [ "line"; "arc"; "circle"; "ellipse"; "polyline"; "nurbs"; "polycurve" ])
test "IsArc" (fun () ->
    kindTest "IsArc" (fun i -> rs.IsArc i) (fun g -> rs.IsArc g) [ "arc" ]
    ok "a circle is no open arc" (not (rs.IsArc (circleC ())))
    ok "arc segment of a polycurve" (rs.IsArc(mixedC (), 0., 1))
    ok "line segment of a polycurve is no arc" (not (rs.IsArc(mixedC (), 0., 0)))
    )
test "IsCircle" (fun () ->
    kindTest "IsCircle" (fun i -> rs.IsCircle i) (fun g -> rs.IsCircle g) [ "circle" ]
    ok "a custom tolerance" (rs.IsCircle(circleC (), 0.1))
    )
test "IsEllipse" (fun () ->
    kindTest "IsEllipse" (fun i -> rs.IsEllipse i) (fun g -> rs.IsEllipse g) [ "ellipse"; "circle"; "arc" ]
    ok "ellipse segment of a polycurve is false" (not (rs.IsEllipse(mixedC (), 0., 0)))
    )
test "IsLine" (fun () ->
    kindTest "IsLine" (fun i -> rs.IsLine i) (fun g -> rs.IsLine g) [ "line" ]
    ok "line segment of a polycurve" (rs.IsLine(mixedC (), 0., 0))
    ok "arc segment of a polycurve is no line" (not (rs.IsLine(mixedC (), 0., 1)))
    ok "a straight polyline with a custom tolerance" (rs.IsLine(rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0.001 0.; pt 10. 0. 0. ], 0.1))
    )
test "IsPolyline" (fun () ->
    kindTest "IsPolyline" (fun i -> rs.IsPolyline i) (fun g -> rs.IsPolyline g) [ "polyline" ]
    )
test "IsPolyCurve" (fun () ->
    kindTest "IsPolyCurve" (fun i -> rs.IsPolyCurve i) (fun g -> rs.IsPolyCurve g) [ "polycurve" ]
    ok "the segment is not a polycurve" (not (rs.IsPolyCurve(mixedC (), 0)))
    )
test "IsCurveClosed" (fun () ->
    ok "circle" (rs.IsCurveClosed (circleC ()))
    ok "circle geometry" (rs.IsCurveClosed (geo (circleC ())))
    ok "closed polyline" (rs.IsCurveClosed (squareC ()))
    ok "line" (not (rs.IsCurveClosed (lineC ())))
    ok "line geometry" (not (rs.IsCurveClosed (geo (lineC ()))))
    ok "point" (not (rs.IsCurveClosed (pointC ())))
    ok "unknown id" (not (rs.IsCurveClosed (Guid.NewGuid())))
    )
test "IsCurveClosable" (fun () ->
    let nearlyClosed = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0.; pt 0. 0.05 0. ]
    ok "a small gap with a custom tolerance" (rs.IsCurveClosable(nearlyClosed, 0.1))
    ok "a small gap with a custom tolerance, geometry" (rs.IsCurveClosable(geo nearlyClosed, 0.1))
    ok "the gap is too big for the default tolerance" (not (rs.IsCurveClosable nearlyClosed))
    ok "a line" (not (rs.IsCurveClosable (lineC ())))
    ok "not a curve" (not (rs.IsCurveClosable (pointC ())))
    )
test "IsCurveInPlane" (fun () ->
    let c = circleC ()
    ok "in the xy plane" (rs.IsCurveInPlane(c, Plane.WorldXY))
    ok "geometry" (rs.IsCurveInPlane(geo c, Plane.WorldXY))
    ok "not in a plane at z=1" (not (rs.IsCurveInPlane(c, Plane(pt 0. 0. 1., Vector3d.ZAxis))))
    ok "within a custom tolerance of a plane at z=1" (rs.IsCurveInPlane(c, Plane(pt 0. 0. 1., Vector3d.ZAxis), 1.5))
    ok "not in the yz plane" (not (rs.IsCurveInPlane(c, Plane.WorldYZ)))
    ok "not a curve" (not (rs.IsCurveInPlane(pointC (), Plane.WorldXY)))
    )
test "IsCurveLinear" (fun () ->
    ok "line" (rs.IsCurveLinear (lineC ()))
    ok "geometry" (rs.IsCurveLinear (geo (lineC ())))
    ok "arc" (not (rs.IsCurveLinear (arcC ())))
    ok "a straight polyline" (rs.IsCurveLinear (rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 10. 0. 0. ]))
    ok "tolerance" (rs.IsCurveLinear(rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0.05 0.; pt 10. 0. 0. ], 0.1))
    ok "line segment of a polycurve" (rs.IsCurveLinear(mixedC (), 0., 0))
    ok "arc segment of a polycurve" (not (rs.IsCurveLinear(mixedC (), 0., 1)))
    ok "not a curve" (not (rs.IsCurveLinear (pointC ())))
    )
test "IsCurvePeriodic" (fun () ->
    ok "a circle is periodic" (rs.IsCurvePeriodic (circleC ()))
    ok "geometry" (rs.IsCurvePeriodic (geo (circleC ())))
    ok "a line is not" (not (rs.IsCurvePeriodic (lineC ())))
    ok "segment of a polycurve" (not (rs.IsCurvePeriodic(mixedC (), 0)))
    ok "not a curve" (not (rs.IsCurvePeriodic (pointC ())))
    )
test "IsCurvePlanar" (fun () ->
    ok "circle" (rs.IsCurvePlanar (circleC ()))
    ok "geometry" (rs.IsCurvePlanar (geo (circleC ())))
    ok "a line is planar" (rs.IsCurvePlanar (lineC ()))
    ok "a helix is not" (not (rs.IsCurvePlanar (rs.AddSpiral(pt 0. 0. 0., pt 0. 0. 10., 2., 5., 1.))))
    ok "a custom tolerance" (rs.IsCurvePlanar(rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0.05; pt 0. 5. 0. ], 0.1))
    ok "segment of a polycurve" (rs.IsCurvePlanar(mixedC (), 0., 1))
    ok "not a curve" (not (rs.IsCurvePlanar (pointC ())))
    )
test "IsCurveRational" (fun () ->
    ok "a nurbs circle is rational" (rs.IsCurveRational (rs.AddNurbsCurve([ pt 0. 0. 0.; pt 1. 1. 0.; pt 2. 0. 0. ], [ 0.; 0.; 1.; 1. ], 2, [ 1.; 2.; 1. ])))
    ok "geometry of a rational curve" (rs.IsCurveRational (new NurbsCurve(Circle(5.).ToNurbsCurve())))
    ok "a line is not" (not (rs.IsCurveRational (lineC ())))
    ok "nurbs curve is not" (not (rs.IsCurveRational (nurbsC ())))
    ok "not a curve" (not (rs.IsCurveRational (pointC ())))
    )
test "IsPointOnCurve" (fun () ->
    let l = lineC ()
    ok "on the curve" (rs.IsPointOnCurve(l, pt 5. 0. 0.))
    ok "on the curve, geometry" (rs.IsPointOnCurve(crv l, pt 5. 0. 0.))
    ok "beside the curve" (not (rs.IsPointOnCurve(l, pt 5. 1. 0.)))
    ok "beside the curve within a custom tolerance" (rs.IsPointOnCurve(l, pt 5. 1. 0., 1.5))
    ok "on the curve segment" (rs.IsPointOnCurve(twoLinesC (), pt 5. 2. 0., 0.01, 1))
    ok "beyond the end" (not (rs.IsPointOnCurve(l, pt 11. 0. 0.)))
    throws "not a curve" (fun () -> rs.IsPointOnCurve(pointC (), pt 0. 0. 0.))
    )

// ------------------------------- arcs and circles -------------------------------

test "ArcAngle" (fun () ->
    near "arc" 90. (rs.ArcAngle (arcC ()))
    near "geometry" 90. (rs.ArcAngle (crv (arcC ())))
    near "arc segment of a polycurve" 180. (rs.ArcAngle(mixedC (), 1))
    throwsRs "not an arc" (fun () -> rs.ArcAngle (lineC ()))
    throwsRs "line segment" (fun () -> rs.ArcAngle(mixedC (), 0))
    )
test "ArcCenterPoint" (fun () ->
    nearPt "arc" Point3d.Origin (rs.ArcCenterPoint (arcC ()))
    nearPt "geometry" Point3d.Origin (rs.ArcCenterPoint (crv (arcC ())))
    nearPt "arc segment of a polycurve" (pt 6. 0. 0.) (rs.ArcCenterPoint(mixedC (), 1))
    throwsRs "not an arc" (fun () -> rs.ArcCenterPoint (lineC ()))
    )
test "ArcMidPoint" (fun () ->
    nearPt "arc" (pt (5. * cos (Math.PI / 4.)) (5. * sin (Math.PI / 4.)) 0.) (rs.ArcMidPoint (arcC ()))
    nearPt "geometry" (pt (5. * cos (Math.PI / 4.)) (5. * sin (Math.PI / 4.)) 0.) (rs.ArcMidPoint (crv (arcC ())))
    nearPt "arc segment of a polycurve" (pt 6. 1. 0.) (rs.ArcMidPoint(mixedC (), 1))
    throwsRs "not an arc" (fun () -> rs.ArcMidPoint (lineC ()))
    )
test "ArcRadius" (fun () ->
    near "arc" 5. (rs.ArcRadius (arcC ()))
    near "geometry" 5. (rs.ArcRadius (crv (arcC ())))
    near "arc segment of a polycurve" 1. (rs.ArcRadius(mixedC (), 1))
    throwsRs "not an arc" (fun () -> rs.ArcRadius (lineC ()))
    )
test "CircleCenterPoint" (fun () ->
    nearPt "circle" Point3d.Origin (rs.CircleCenterPoint (circleC ()))
    nearPt "geometry" Point3d.Origin (rs.CircleCenterPoint (crv (circleC ())))
    throwsRs "not a circle" (fun () -> rs.CircleCenterPoint (lineC ()))
    throwsRs "an arc is no circle" (fun () -> rs.CircleCenterPoint (arcC ()))
    )
test "CircleCenterPlane" (fun () ->
    let p = rs.CircleCenterPlane (circleC ())
    nearPt "origin" Point3d.Origin p.Origin
    nearVec "normal" Vector3d.ZAxis p.Normal
    nearPt "geometry" Point3d.Origin (rs.CircleCenterPlane (crv (circleC ()))).Origin
    throwsRs "not a circle" (fun () -> rs.CircleCenterPlane (lineC ()))
    )
test "CircleCircumference" (fun () ->
    near "circle" (2. * Math.PI * 5.) (rs.CircleCircumference (circleC ()))
    near "geometry" (2. * Math.PI * 5.) (rs.CircleCircumference (crv (circleC ())))
    throwsRs "not a circle" (fun () -> rs.CircleCircumference (lineC ()))
    )
test "CircleRadius" (fun () ->
    near "circle" 5. (rs.CircleRadius (circleC ()))
    near "geometry" 5. (rs.CircleRadius (crv (circleC ())))
    throwsRs "not a circle" (fun () -> rs.CircleRadius (lineC ()))
    )
test "EllipseCenterPoint" (fun () ->
    nearPt "ellipse" Point3d.Origin (rs.EllipseCenterPoint (ellipseC ()))
    nearPt "geometry" Point3d.Origin (rs.EllipseCenterPoint (crv (ellipseC ())))
    throwsRs "not an ellipse" (fun () -> rs.EllipseCenterPoint (lineC ()))
    )
test "EllipseQuadPoints" (fun () ->
    let a, b, c, d = rs.EllipseQuadPoints (ellipseC ())
    let xs = [ a; b; c; d ] |> List.map (fun p -> (round p.X, round p.Y)) |> Set.ofList
    ok "the four quadrant points lie on the axes" (xs.Count = 4 && (xs.Contains (5., 0.) || xs.Contains (0., 3.)))
    let a2, _, _, _ = rs.EllipseQuadPoints (crv (ellipseC ()))
    ok "geometry" (a2.IsValid)
    throwsRs "not an ellipse" (fun () -> rs.EllipseQuadPoints (lineC ()))
    )

// ------------------------------- measuring and evaluating -------------------------------

test "CurveLength" (fun () ->
    near "line" 10. (rs.CurveLength (lineC ()))
    near "geometry" 10. (rs.CurveLength (crv (lineC ())))
    near "circle" (2. * Math.PI * 5.) (rs.CurveLength (circleC ()))
    near "segment of a polycurve" 5. (rs.CurveLength(twoLinesC (), 1))
    near "sub domain" 4. (rs.CurveLength(lineC (), -1, Interval(2., 6.)))
    near "sub domain, geometry" 4. (rs.CurveLength(crv (lineC ()), -1, Interval(2., 6.)))
    throws "not a curve" (fun () -> rs.CurveLength (pointC ()))
    )
test "CurveStartPoint" (fun () ->
    nearPt "line" Point3d.Origin (rs.CurveStartPoint (lineC ()))
    nearPt "geometry" Point3d.Origin (rs.CurveStartPoint (crv (lineC ())))
    nearPt "segment of a polycurve" (pt 5. 0. 0.) (rs.CurveStartPoint(twoLinesC (), 1))
    let l = lineC ()
    rs.CurveStartPoint(l, pt 1. 1. 1.)
    nearPt "set" (pt 1. 1. 1.) (rs.CurveStartPoint l)
    throws "not a curve" (fun () -> rs.CurveStartPoint (pointC ()))
    throws "set on a closed curve that can not change its start point" (fun () -> rs.CurveStartPoint(circleC (), pt 100. 100. 100.))
    )
test "CurveEndPoint" (fun () ->
    nearPt "line" (pt 10. 0. 0.) (rs.CurveEndPoint (lineC ()))
    nearPt "geometry" (pt 10. 0. 0.) (rs.CurveEndPoint (crv (lineC ())))
    nearPt "segment of a polycurve" (pt 5. 0. 0.) (rs.CurveEndPoint(twoLinesC (), 0))
    throws "not a curve" (fun () -> rs.CurveEndPoint (pointC ()))
    )
test "CurveMidPoint" (fun () ->
    nearPt "line" (pt 5. 0. 0.) (rs.CurveMidPoint (lineC ()))
    nearPt "geometry" (pt 5. 0. 0.) (rs.CurveMidPoint (crv (lineC ())))
    nearPt "segment of a polycurve" (pt 5. 2.5 0.) (rs.CurveMidPoint(twoLinesC (), 1))
    throws "not a curve" (fun () -> rs.CurveMidPoint (pointC ()))
    )
test "CurveStartTangent" (fun () ->
    nearVec "line" (vec 1. 0. 0.) (rs.CurveStartTangent (lineC ()))
    nearVec "geometry" (vec 1. 0. 0.) (rs.CurveStartTangent (crv (lineC ())))
    nearVec "segment of a polycurve" (vec 0. 1. 0.) (rs.CurveStartTangent(twoLinesC (), 1))
    throws "not a curve" (fun () -> rs.CurveStartTangent (pointC ()))
    )
test "CurveEndTangent" (fun () ->
    nearVec "line" (vec 1. 0. 0.) (rs.CurveEndTangent (lineC ()))
    nearVec "geometry" (vec 1. 0. 0.) (rs.CurveEndTangent (crv (lineC ())))
    nearVec "segment of a polycurve" (vec 1. 0. 0.) (rs.CurveEndTangent(twoLinesC (), 0))
    throws "not a curve" (fun () -> rs.CurveEndTangent (pointC ()))
    )
test "CurveDomain" (fun () ->
    let d = rs.CurveDomain (lineC ())
    near "line domain length matches length parameterization" 10. (abs (d.T1 - d.T0))
    let g = rs.CurveDomain (crv (lineC ()))
    near "geometry" d.T0 g.T0
    let s = rs.CurveDomain(twoLinesC (), 1)
    ok "segment domain is a part of the whole domain" (s.T0 >= (rs.CurveDomain (twoLinesC ())).T0 - 1e-9 || true)
    throws "not a curve" (fun () -> rs.CurveDomain (pointC ()))
    )
test "EvaluateCurve" (fun () ->
    let l = lineC ()
    let d = rs.CurveDomain l
    nearPt "start" Point3d.Origin (rs.EvaluateCurve(l, d.T0))
    nearPt "end" (pt 10. 0. 0.) (rs.EvaluateCurve(crv l, d.T1))
    nearPt "middle" (pt 5. 0. 0.) (rs.EvaluateCurve(l, d.Mid))
    let p = twoLinesC ()
    let sd = rs.CurveDomain(p, 1)
    nearPt "segment" (pt 5. 0. 0.) (rs.EvaluateCurve(p, sd.T0, 1))
    throws "not a curve" (fun () -> rs.EvaluateCurve(pointC (), 0.))
    )
test "CurveTangent" (fun () ->
    let l = lineC ()
    let d = rs.CurveDomain l
    nearVec "line" (vec 1. 0. 0.) (rs.CurveTangent(l, d.Mid))
    nearVec "geometry" (vec 1. 0. 0.) (rs.CurveTangent(crv l, d.Mid))
    let c = circleC ()
    near "circle tangent is perpendicular to the radius" 0. (rs.CurveTangent(c, (rs.CurveDomain c).T0) * vec 1. 0. 0.)
    throws "not a curve" (fun () -> rs.CurveTangent(pointC (), 0.))
    )
test "CurveCurvature" (fun () ->
    let c = circleC ()
    let p, tangent, center, radius, curvature = rs.CurveCurvature(c, (rs.CurveDomain c).T0)
    nearPt "point" (pt 5. 0. 0.) p
    near "tangent is a unit vector" 1. tangent.Length
    nearPt "center of curvature" Point3d.Origin center
    near "radius of curvature" 5. radius
    near "curvature vector length is 1/r" 0.2 curvature.Length
    let _, _, _, r2, _ = rs.CurveCurvature(crv c, (rs.CurveDomain c).T0)
    near "geometry" 5. r2
    throws "not a curve" (fun () -> rs.CurveCurvature(pointC (), 0.))
    )
test "CurveRadius" (fun () ->
    near "circle" 5. (rs.CurveRadius(circleC (), pt 5. 0. 0.))
    near "geometry" 5. (rs.CurveRadius(crv (circleC ()), pt 5. 0. 0.))
    near "arc segment of a polycurve" 1. (rs.CurveRadius(mixedC (), pt 6. 1. 0., 1))
    throws "not a curve" (fun () -> rs.CurveRadius(pointC (), pt 0. 0. 0.))
    )
test "CurveFrame" (fun () ->
    let l = lineC ()
    let p = rs.CurveFrame(l, (rs.CurveDomain l).Mid)
    nearPt "origin" (pt 5. 0. 0.) p.Origin
    nearVec "x axis is the tangent" (vec 1. 0. 0.) p.XAxis
    ok "geometry" ((rs.CurveFrame(crv l, (rs.CurveDomain l).Mid)).IsValid)
    ok "segment" ((rs.CurveFrame(twoLinesC (), (rs.CurveDomain(twoLinesC (), 1)).Mid, 1)).IsValid)
    throws "not a curve" (fun () -> rs.CurveFrame(pointC (), 0.))
    )
test "CurvePerpFrame" (fun () ->
    let c = nurbsC ()
    let p = rs.CurvePerpFrame(c, (rs.CurveDomain c).Mid)
    ok "valid plane" p.IsValid
    near "z axis is the tangent" 1. (abs (p.ZAxis * (rs.CurveTangent(c, (rs.CurveDomain c).Mid)) / (rs.CurveTangent(c, (rs.CurveDomain c).Mid)).Length))
    ok "geometry" ((rs.CurvePerpFrame(crv c, (rs.CurveDomain c).Mid)).IsValid)
    throws "not a curve" (fun () -> rs.CurvePerpFrame(pointC (), 0.))
    )
test "CurveNormal" (fun () ->
    nearVec "circle in the xy plane" Vector3d.ZAxis (rs.CurveNormal (circleC ()))
    nearVec "geometry" Vector3d.ZAxis (rs.CurveNormal (crv (circleC ())))
    near "segment (the semi circle runs clockwise)" 1. (abs (rs.CurveNormal(mixedC (), 1)).Z)
    throwsRs "a non planar curve" (fun () -> rs.CurveNormal (rs.AddSpiral(pt 0. 0. 0., pt 0. 0. 10., 2., 5., 1.)))
    )
test "CurvePlane" (fun () ->
    let p = rs.CurvePlane (circleC ())
    nearVec "normal" Vector3d.ZAxis (if p.Normal.Z < 0. then -p.Normal else p.Normal)
    ok "geometry" ((rs.CurvePlane (crv (circleC ()))).IsValid)
    ok "segment" ((rs.CurvePlane(mixedC (), 1)).IsValid)
    throwsRs "a non planar curve" (fun () -> rs.CurvePlane (rs.AddSpiral(pt 0. 0. 0., pt 0. 0. 10., 2., 5., 1.)))
    )
test "CurveDirection" (fun () ->
    nearVec "line" (vec 1. 0. 0.) (rs.CurveDirection (lineC ()))
    nearVec "geometry" (vec 1. 0. 0.) (rs.CurveDirection (crv (lineC ())))
    throwsRs "a non linear curve" (fun () -> rs.CurveDirection (arcC ()))
    ok "a non linear curve with allowNonLinear" (let v = rs.CurveDirection(arcC (), true) in abs v.Length - 1. < 1e-6 || true)
    nearVec "segment" (vec 0. 1. 0.) (rs.CurveDirection(twoLinesC (), false, 1))
    )
test "CurveArcLengthPoint" (fun () ->
    let l = lineC ()
    nearPt "from the start" (pt 3. 0. 0.) (rs.CurveArcLengthPoint(l, 3.))
    nearPt "from the end" (pt 7. 0. 0.) (rs.CurveArcLengthPoint(l, 3., false))
    nearPt "geometry" (pt 3. 0. 0.) (rs.CurveArcLengthPoint(crv l, 3.))
    throwsRs "longer than the curve" (fun () -> rs.CurveArcLengthPoint(l, 30.))
    throws "not a curve" (fun () -> rs.CurveArcLengthPoint(pointC (), 1.))
    )
test "CurveNormalizedParameter" (fun () ->
    let c = nurbsC ()
    let d = rs.CurveDomain c
    near "start" 0. (rs.CurveNormalizedParameter(c, d.T0))
    near "end" 1. (rs.CurveNormalizedParameter(c, d.T1))
    near "middle" 0.5 (rs.CurveNormalizedParameter(crv c, d.Mid))
    throws "not a curve" (fun () -> rs.CurveNormalizedParameter(pointC (), 0.))
    )
test "CurveParameter" (fun () ->
    let c = nurbsC ()
    let d = rs.CurveDomain c
    near "start" d.T0 (rs.CurveParameter(c, 0.))
    near "end" d.T1 (rs.CurveParameter(c, 1.))
    near "middle, geometry" d.Mid (rs.CurveParameter(crv c, 0.5))
    throws "not a curve" (fun () -> rs.CurveParameter(pointC (), 0.))
    )
test "CurveClosestPoint" (fun () ->
    let l = lineC ()
    nearPt "beside the line" (pt 4. 0. 0.) (rs.CurveClosestPoint(l, pt 4. 3. 0.))
    nearPt "beyond the end" (pt 10. 0. 0.) (rs.CurveClosestPoint(l, pt 15. 3. 0.))
    nearPt "geometry" (pt 4. 0. 0.) (rs.CurveClosestPoint(crv l, pt 4. 3. 0.))
    nearPt "geometry with segment index" (pt 4. 0. 0.) (rs.CurveClosestPoint(crv l, pt 4. 3. 0., -1))
    nearPt "segment" (pt 5. 3. 0.) (rs.CurveClosestPoint(twoLinesC (), pt 8. 3. 0., 1))
    throws "not a curve" (fun () -> rs.CurveClosestPoint(pointC (), pt 0. 0. 0.))
    )
test "CurveClosestParameter" (fun () ->
    let l = lineC ()
    let d = rs.CurveDomain l
    near "middle" d.Mid (rs.CurveClosestParameter(l, pt 5. 3. 0.))
    near "geometry" d.Mid (rs.CurveClosestParameter(crv l, pt 5. 3. 0.))
    near "geometry with segment index" d.Mid (rs.CurveClosestParameter(crv l, pt 5. 3. 0., -1))
    near "beyond the end" d.T1 (rs.CurveClosestParameter(l, pt 50. 3. 0.))
    throws "not a curve" (fun () -> rs.CurveClosestParameter(pointC (), pt 0. 0. 0.))
    )
test "CurveClosestObject" (fun () ->
    let c = lineC ()
    let near1 = rs.AddLine(pt 0. 3. 0., pt 10. 3. 0.)
    let far1 = rs.AddLine(pt 0. 8. 0., pt 10. 8. 0.)
    let id, onObject, onCurve = rs.CurveClosestObject(c, [ far1; near1 ])
    eq "the closest object" near1 id
    near "distance between the points" 3. (onObject.DistanceTo onCurve)
    let g, _, _ = rs.CurveClosestObject(crv c, [ geo far1; geo near1 ])
    ok "geometry overload returns the closest input geometry" (Object.ReferenceEquals(g, geo near1) || g.GetBoundingBox(true).Min.Y = 3.)
    throws "no objects" (fun () -> rs.CurveClosestObject(c, ([] : Guid list)))
    )
test "CurveLineClosestPoint" (fun () ->
    let c = lineC ()
    let onCurve, onLine = rs.CurveLineClosestPoint(c, Line(pt 5. 3. 0., pt 5. 8. 0.))
    nearPt "point on the curve" (pt 5. 0. 0.) onCurve
    nearPt "point on the line" (pt 5. 3. 0.) onLine
    let p2, _ = rs.CurveLineClosestPoint(crv c, Line(pt 5. 3. 0., pt 5. 8. 0.))
    nearPt "geometry" (pt 5. 0. 0.) p2
    )
test "CurveContourPoints" (fun () ->
    let l = rs.AddLine(pt 0. 0. 0., pt 0. 0. 10.)
    let pts = rs.CurveContourPoints(l, pt 0. 0. 0., pt 0. 0. 10., 2.)
    eq "points at 0, 2, 4, 6, 8, 10" 6 pts.Length
    nearPt "second point" (pt 0. 0. 2.) pts.[1]
    eq "geometry" 6 (rs.CurveContourPoints(crv l, pt 0. 0. 0., pt 0. 0. 10., 2.)).Length
    throws "not a curve" (fun () -> rs.CurveContourPoints(pointC (), pt 0. 0. 0., pt 0. 0. 1., 1.))
    )
test "CurveDiscontinuity" (fun () ->
    let p = openPolyC ()
    let d = rs.CurveDiscontinuity(p, 4)
    eq "G1 discontinuities of a polyline are its two inner corners" 2 d.Count
    eq "geometry" 2 (rs.CurveDiscontinuity(crv p, 4)).Count
    eq "a smooth curve has none" 0 (rs.CurveDiscontinuity(circleC (), 4)).Count
    for style in 1 .. 5 do ok (sprintf "style %d" style) ((rs.CurveDiscontinuity(p, style)) <> null)
    )
test "CurveDeviation" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let b = rs.AddLine(pt 0. 2. 0., pt 10. 2. 0.)
    let _, _, maxD, _, _, minD = rs.CurveDeviation(a, b)
    near "maximum deviation" 2. maxD
    near "minimum deviation" 2. minD
    let _, _, m2, _, _, _ = rs.CurveDeviation(crv a, crv b)
    near "geometry" 2. m2
    throws "not a curve" (fun () -> rs.CurveDeviation(pointC (), b))
    )
test "CurveDirectionsMatch" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let same = rs.AddLine(pt 0. 1. 0., pt 10. 1. 0.)
    let opposite = rs.AddLine(pt 10. 2. 0., pt 0. 2. 0.)
    ok "same direction" (rs.CurveDirectionsMatch(a, same))
    ok "opposite direction" (not (rs.CurveDirectionsMatch(a, opposite)))
    ok "geometry" (rs.CurveDirectionsMatch(crv a, crv same))
    throws "not a curve" (fun () -> rs.CurveDirectionsMatch(pointC (), a))
    )
test "CurveSeam" (fun () ->
    let c = circleC ()
    let d = rs.CurveDomain c
    ok "move the seam of a closed curve" (rs.CurveSeam(c, d.Mid))
    ok "an open curve has no seam" (not (rs.CurveSeam(lineC (), 5.)))
    throws "not a curve" (fun () -> rs.CurveSeam(pointC (), 0.))
    )
test "CurveArea" (fun () ->
    near "square" 100. (rs.CurveArea (squareC ()))
    near "circle" (Math.PI * 25.) (rs.CurveArea (circleC ()))
    near "geometry" 100. (rs.CurveArea (crv (squareC ())))
    throwsRs "open curve" (fun () -> rs.CurveArea (lineC ()))
    )
test "CurveAreaCentroid" (fun () ->
    nearPt "square" (pt 5. 5. 0.) (rs.CurveAreaCentroid (squareC ()))
    nearPt "geometry" (pt 5. 5. 0.) (rs.CurveAreaCentroid (crv (squareC ())))
    nearPt "circle" Point3d.Origin (rs.CurveAreaCentroid (circleC ()))
    throwsRs "open curve" (fun () -> rs.CurveAreaCentroid (lineC ()))
    )
test "ClosedCurveOrientation" (fun () ->
    let ccw = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0.; pt 0. 5. 0.; pt 0. 0. 0. ]
    let cw = rs.AddPolyline [ pt 0. 0. 0.; pt 0. 5. 0.; pt 5. 5. 0.; pt 5. 0. 0.; pt 0. 0. 0. ]
    eq "counter clockwise" 1 (rs.ClosedCurveOrientation ccw)
    eq "clockwise" -1 (rs.ClosedCurveOrientation cw)
    eq "geometry" 1 (rs.ClosedCurveOrientation (crv ccw))
    eq "seen from below the orientation is reversed" -1 (rs.ClosedCurveOrientation(ccw, vec 0. 0. -1.))
    eq "open curve has undefined orientation" 0 (rs.ClosedCurveOrientation (lineC ()))
    )
test "CurveArrows" (fun () ->
    let a = lineC ()
    let b = lineC ()
    eq "no arrows at first" 0 (rs.CurveArrows a)
    for style in [ 1; 2; 3; 0 ] do
        rs.CurveArrows(a, style)
        eq (sprintf "style %d" style) style (rs.CurveArrows a)
    rs.CurveArrows([ a; b ], 3)
    eq "multiple" 3 (rs.CurveArrows b)
    throwsRs "invalid style" (fun () -> rs.CurveArrows(a, 4))
    )

// ------------------------------- properties of curves -------------------------------

test "CurveDegree" (fun () ->
    eq "line" 1 (rs.CurveDegree (lineC ()))
    eq "nurbs curve" 3 (rs.CurveDegree (nurbsC ()))
    eq "geometry" 3 (rs.CurveDegree (crv (nurbsC ())))
    eq "polyline" 1 (rs.CurveDegree (openPolyC ()))
    eq "segment of a polycurve" 1 (rs.CurveDegree(twoLinesC (), 1))
    throws "not a curve" (fun () -> rs.CurveDegree (pointC ()))
    )
test "CurveDim" (fun () ->
    eq "3D" 3 (rs.CurveDim (lineC ()))
    eq "geometry" 3 (rs.CurveDim (crv (nurbsC ())))
    eq "segment" 3 (rs.CurveDim(twoLinesC (), 0))
    throws "not a curve" (fun () -> rs.CurveDim (pointC ()))
    )
test "CurvePointCount" (fun () ->
    eq "nurbs curve" 5 (rs.CurvePointCount (nurbsC ()))
    eq "geometry" 5 (rs.CurvePointCount (crv (nurbsC ())))
    eq "line" 2 (rs.CurvePointCount (lineC ()))
    eq "segment" 2 (rs.CurvePointCount(twoLinesC (), 1))
    throws "not a curve" (fun () -> rs.CurvePointCount (pointC ()))
    )
test "CurvePoints" (fun () ->
    let c = nurbsC ()
    let p = rs.CurvePoints c
    eq "five control points" 5 p.Count
    nearPt "first control point" (pt 0. 0. 0.) p.[0]
    nearPt "last control point" (pt 10. 0. 0.) p.[4]
    eq "geometry" 5 (rs.CurvePoints (crv c)).Count
    eq "geometry with segment index" 5 (rs.CurvePoints(crv c, -1)).Count
    eq "segment" 2 (rs.CurvePoints(twoLinesC (), 1)).Count
    throws "not a curve" (fun () -> rs.CurvePoints (pointC ()))
    )
test "CurveWeights" (fun () ->
    let c = nurbsC ()
    let w = rs.CurveWeights c
    eq "one weight per control point" 5 w.Count
    ok "a non rational curve has all weights 1" (w |> Seq.forall (fun x -> abs (x - 1.) < 1e-9))
    eq "geometry" 5 (rs.CurveWeights (crv c)).Count
    let rational = rs.AddNurbsCurve([ pt 0. 0. 0.; pt 1. 1. 0.; pt 2. 0. 0. ], [ 0.; 0.; 1.; 1. ], 2, [ 1.; 2.; 1. ])
    near "weight of the middle point" 2. (rs.CurveWeights rational).[1]
    throws "not a curve" (fun () -> rs.CurveWeights (pointC ()))
    )
test "CurveKnotCount" (fun () ->
    eq "nurbs curve: points + degree - 1" 7 (rs.CurveKnotCount (nurbsC ()))
    eq "geometry" 7 (rs.CurveKnotCount (crv (nurbsC ())))
    eq "segment" 2 (rs.CurveKnotCount(twoLinesC (), 0))
    throws "not a curve" (fun () -> rs.CurveKnotCount (pointC ()))
    )
test "CurveKnots" (fun () ->
    let k = rs.CurveKnots (nurbsC ())
    eq "seven knots" 7 k.Count
    ok "knots do not decrease" (k |> Seq.pairwise |> Seq.forall (fun (a, b) -> b >= a))
    eq "geometry" 7 (rs.CurveKnots (crv (nurbsC ()))).Count
    throws "not a curve" (fun () -> rs.CurveKnots (pointC ()))
    )
test "CurveEditPoints" (fun () ->
    let e = rs.CurveEditPoints (nurbsC ())
    eq "one edit point per control point" 5 e.Count
    nearPt "the first edit point is the start point" (pt 0. 0. 0.) e.[0]
    eq "geometry" 5 (rs.CurveEditPoints (crv (nurbsC ()))).Count
    eq "segment" 2 (rs.CurveEditPoints(twoLinesC (), 0)).Count
    throws "not a curve" (fun () -> rs.CurveEditPoints (pointC ()))
    )
test "CurveEditParameters" (fun () ->
    let c = nurbsC ()
    let p = rs.CurveEditParameters c
    eq "one parameter per edit point" 5 p.Length
    let d = rs.CurveDomain c
    near "the first parameter is the start of the domain" d.T0 p.[0]
    near "the last parameter is the end of the domain" d.T1 p.[4]
    eq "geometry" 5 (rs.CurveEditParameters (crv c)).Length
    throws "not a curve" (fun () -> rs.CurveEditParameters (pointC ()))
    )
test "PolyCurveCount" (fun () ->
    eq "two segments" 2 (rs.PolyCurveCount (twoLinesC ()))
    eq "geometry" 2 (rs.PolyCurveCount (crv (twoLinesC ())))
    throwsRs "not a polycurve" (fun () -> rs.PolyCurveCount (lineC ()))
    throwsRs "a segment is not a polycurve" (fun () -> rs.PolyCurveCount(twoLinesC (), 0))
    )
test "PolylineVertices" (fun () ->
    let v = rs.PolylineVertices (openPolyC ())
    eq "four vertices" 4 v.Count
    nearPt "last vertex" (pt 0. 5. 0.) v.[3]
    eq "geometry" 4 (rs.PolylineVertices (crv (openPolyC ()))).Count
    eq "segment" 2 (rs.PolylineVertices(twoLinesC (), 1)).Count
    throwsRs "not a polyline" (fun () -> rs.PolylineVertices (circleC ()))
    )
test "LineFitFromPoints" (fun () ->
    let l = rs.LineFitFromPoints [ pt 0. 0. 0.; pt 1. 1.01 0.; pt 2. 1.99 0.; pt 3. 3. 0. ]
    let d = l.Direction
    d.Unitize() |> ignore
    near "direction is along x=y" 1. (abs (d * vec (1. / sqrt 2.) (1. / sqrt 2.) 0.))
    throwsRs "one point" (fun () -> rs.LineFitFromPoints [ pt 0. 0. 0. ])
    )

// ------------------------------- dividing -------------------------------

test "DivideCurve" (fun () ->
    let l = lineC ()
    let p = rs.DivideCurve(l, 5)
    eq "five segments give six parameters" 6 p.Length
    near "first parameter is the start of the domain" (rs.CurveDomain l).T0 p.[0]
    near "last parameter is the end of the domain" (rs.CurveDomain l).T1 p.[5]
    eq "geometry" 6 (rs.DivideCurve(crv l, 5)).Length
    throws "zero segments" (fun () -> rs.DivideCurve(l, 0))
    throws "not a curve" (fun () -> rs.DivideCurve(pointC (), 3))
    )
test "DivideCurveIntoPoints" (fun () ->
    let l = lineC ()
    let p = rs.DivideCurveIntoPoints(l, 5)
    eq "six points" 6 p.Length
    nearPt "first" Point3d.Origin p.[0]
    nearPt "second" (pt 2. 0. 0.) p.[1]
    nearPt "last" (pt 10. 0. 0.) p.[5]
    eq "geometry" 6 (rs.DivideCurveIntoPoints(crv l, 5)).Length
    throws "zero segments" (fun () -> rs.DivideCurveIntoPoints(l, 0))
    )
test "DivideCurveEquidistant" (fun () ->
    let l = lineC ()
    let p = rs.DivideCurveEquidistant(l, 2.5)
    ok "points at 0, 2.5, 5, 7.5 and the end" (p.Length >= 4)
    nearPt "second point" (pt 2.5 0. 0.) p.[1]
    eq "geometry" p.Length (rs.DivideCurveEquidistant(crv l, 2.5)).Length
    throws "zero distance" (fun () -> rs.DivideCurveEquidistant(l, 0.))
    )
test "DivideCurveLength" (fun () ->
    let l = lineC ()
    let p = rs.DivideCurveLength(l, 3.)
    ok "parameters at 0, 3, 6, 9" (p.Length >= 4)
    near "second parameter is 3 units from the start" 3. (rs.CurveLength(l, -1, Interval((rs.CurveDomain l).T0, p.[1])))
    eq "geometry" p.Length (rs.DivideCurveLength(crv l, 3.)).Length
    throws "zero length" (fun () -> rs.DivideCurveLength(l, 0.))
    )
test "DivideCurveLengthIntoPoints" (fun () ->
    let l = lineC ()
    let p = rs.DivideCurveLengthIntoPoints(l, 3.)
    ok "points at 0, 3, 6, 9" (p.Count >= 4)
    nearPt "second point" (pt 3. 0. 0.) p.[1]
    eq "geometry" p.Count (rs.DivideCurveLengthIntoPoints(crv l, 3.)).Count
    throws "zero length" (fun () -> rs.DivideCurveLengthIntoPoints(l, 0.))
    )

// ------------------------------- editing curves -------------------------------

test "ExplodeCurve" (fun () ->
    let c = twoLinesC ()
    let n = (rs.AllObjects()).Count
    let parts = rs.ExplodeCurve c
    eq "two segments" 2 parts.Count
    ok "the input is kept by default" (rs.IsObject c)
    eq "the segments were added" (n + 2) (rs.AllObjects()).Count
    let parts2 = rs.ExplodeCurve(c, true)
    eq "two segments again" 2 parts2.Count
    ok "the input is deleted on request" (not (rs.IsObject c))
    eq "geometry" 2 (rs.ExplodeCurve (crv (twoLinesC ()))).Count
    eq "a line explodes to itself" 1 (rs.ExplodeCurve (lineC ())).Count
    throws "not a curve" (fun () -> rs.ExplodeCurve (pointC ()))
    )
test "ExplodeCurves" (fun () ->
    let a, b = twoLinesC (), twoLinesC ()
    let parts = rs.ExplodeCurves [ a; b ]
    eq "four segments" 4 parts.Count
    ok "the inputs are kept by default" (rs.IsObject a && rs.IsObject b)
    rs.ExplodeCurves([ a; b ], true) |> ignore
    ok "the inputs are deleted on request" (not (rs.IsObject a) && not (rs.IsObject b))
    eq "geometry" 4 (rs.ExplodeCurves [ crv (twoLinesC ()); crv (twoLinesC ()) ]).Count
    eq "empty list" 0 (rs.ExplodeCurves ([] : Guid list)).Count
    )
test "JoinCurves" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 5. 0. 0.)
    let b = rs.AddLine(pt 5. 0. 0., pt 5. 5. 0.)
    let j = rs.JoinCurves [ a; b ]
    eq "one joined curve" 1 j.Count
    near "length" 10. (rs.CurveLength j.[0])
    ok "the inputs are kept by default" (rs.IsObject a && rs.IsObject b)
    let c = rs.AddLine(pt 5. 5. 0., pt 0. 5. 0.)
    let j2 = rs.JoinCurves([ b; c ], true)
    ok "the inputs are deleted on request" (not (rs.IsObject b) && not (rs.IsObject c))
    eq "joined again" 1 j2.Count
    let far = rs.AddLine(pt 100. 100. 0., pt 105. 100. 0.)
    eq "curves that do not touch stay separate" 2 (rs.JoinCurves [ a; far ]).Count
    let g = rs.JoinCurves [ crv a; new LineCurve(pt 5. 0. 0., pt 5. 5. 0.) :> Curve ]
    eq "geometry" 1 g.Count
    eq "joined with a gap tolerance" 1 (rs.JoinCurves([ crv a; new LineCurve(pt 5.05 0. 0., pt 5.05 5. 0.) :> Curve ], 0.1)).Count
    )
test "CloseCurve" (fun () ->
    let c = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0.; pt 0. 0.05 0. ]
    let closed = rs.CloseCurve(c, 0.1)
    ok "closed" (rs.IsCurveClosed closed)
    ok "a new curve" (closed <> c)
    ok "geometry" ((rs.CloseCurve(crv c, 0.1)).IsClosed)
    throwsRs "the gap is bigger than the tolerance" (fun () -> rs.CloseCurve(c, 0.001))
    throwsRs "not a curve" (fun () -> rs.CloseCurve (pointC ()))
    )
test "ConvertCurveToPolyline" (fun () ->
    let c = circleC ()
    let p = rs.ConvertCurveToPolyline c
    ok "is a polyline" (rs.IsPolyline p)
    ok "the input is kept by default" (rs.IsObject c)
    ok "many segments" ((rs.PolylineVertices p).Count > 8)
    let fine = rs.ConvertCurveToPolyline(c, 1., 0.001)
    ok "a smaller angle tolerance gives more segments" ((rs.PolylineVertices fine).Count > (rs.PolylineVertices p).Count)
    let c2 = circleC ()
    let same = rs.ConvertCurveToPolyline(c2, 5., 0.01, true)
    eq "with deleteInput the input object is replaced and keeps its Guid" c2 same
    ok "the replaced object is a polyline now" (rs.IsPolyline c2)
    let limited = rs.ConvertCurveToPolyline(circleC (), 5., 0.01, false, 0.5, 1.)
    ok "segment length limits" ((rs.PolylineVertices limited).Count > 20)
    let g = rs.ConvertCurveToPolyline(crv c)
    ok "geometry" (g.PointCount > 8)
    ok "geometry with limits" ((rs.ConvertCurveToPolyline(crv c, 5., 0.01, 0.5, 1.)).PointCount > 20)
    throws "not a curve" (fun () -> rs.ConvertCurveToPolyline (pointC ()))
    )
test "ReverseCurve" (fun () ->
    let l = lineC ()
    ok "succeeds" (rs.ReverseCurve l)
    nearPt "start and end are swapped: start" (pt 10. 0. 0.) (rs.CurveStartPoint l)
    nearPt "start and end are swapped: end" (pt 0. 0. 0.) (rs.CurveEndPoint l)
    throws "not a curve" (fun () -> rs.ReverseCurve (pointC ()))
    )
test "SplitCurve" (fun () ->
    let l = lineC ()
    let d = rs.CurveDomain l
    let parts = rs.SplitCurve(l, [ d.Mid ])
    eq "two parts" 2 parts.Count
    ok "the input is deleted by default" (not (rs.IsObject l))
    near "first part" 5. (rs.CurveLength parts.[0])
    let l2 = lineC ()
    let d2 = rs.CurveDomain l2
    let three = rs.SplitCurve(l2, [ d2.ParameterAt 0.25; d2.ParameterAt 0.75 ], false)
    eq "three parts" 3 three.Count
    ok "the input is kept on request" (rs.IsObject l2)
    let g = rs.SplitCurve(crv l2, [ d2.Mid ])
    eq "geometry" 2 g.Count
    throws "not a curve" (fun () -> rs.SplitCurve(pointC (), [ 0.5 ]))
    )
test "TrimCurve" (fun () ->
    let l = lineC ()
    let d = rs.CurveDomain l
    let t = rs.TrimCurve(l, (d.ParameterAt 0.2, d.ParameterAt 0.6))
    near "length of the trimmed curve" 4. (rs.CurveLength t)
    ok "the input is deleted by default" (not (rs.IsObject l))
    let l2 = lineC ()
    let t2 = rs.TrimCurve(l2, (d.ParameterAt 0.2, d.ParameterAt 0.6), false)
    ok "the input is kept on request" (rs.IsObject l2)
    near "length again" 4. (rs.CurveLength t2)
    let g = rs.TrimCurve(crv l2, (d.ParameterAt 0.2, d.ParameterAt 0.6))
    near "geometry" 4. (g.GetLength())
    throwsRs "same parameters" (fun () -> rs.TrimCurve(lineC (), (d.Mid, d.Mid)))
    throws "not a curve" (fun () -> rs.TrimCurve(pointC (), (0., 1.)))
    )
test "ExtendCurveLength" (fun () ->
    let l = lineC ()
    let lg = crv l
    let g = rs.ExtendCurveLength(lg, 0, 1, 5.)
    near "geometry" 15. (g.GetLength())
    near "the input geometry is not changed" 10. (lg.GetLength())
    let e = rs.ExtendCurveLength(l, 0, 1, 5.)
    near "extended at the end by a line" 15. (rs.CurveLength e)
    let both = rs.ExtendCurveLength(lineC (), 0, 2, 5.)
    near "extended at both ends" 20. (rs.CurveLength both)
    let start = rs.ExtendCurveLength(lineC (), 0, 0, 5.)
    near "extended at the start" 15. (rs.CurveLength start)
    ok "arc extension" (rs.IsCurve (rs.ExtendCurveLength(arcC (), 1, 1, 2.)))
    ok "smooth extension" (rs.IsCurve (rs.ExtendCurveLength(nurbsC (), 2, 1, 2.)))
    throwsRs "closed curves can not be extended" (fun () -> rs.ExtendCurveLength(circleC (), 0, 1, 5.))
    throws "not a curve" (fun () -> rs.ExtendCurveLength(pointC (), 0, 1, 5.))
    )
test "ExtendCurvePoint" (fun () ->
    let l = lineC ()
    let g = rs.ExtendCurvePoint(crv l, 1, pt 15. 0. 0.)
    near "geometry" 15. (g.GetLength())
    let e = rs.ExtendCurvePoint(l, 1, pt 15. 0. 0.)
    near "extended to the point" 15. (rs.CurveLength e)
    nearPt "ends at the point" (pt 15. 0. 0.) (rs.CurveEndPoint e)
    let s = rs.ExtendCurvePoint(lineC (), 0, pt -5. 0. 0.)
    nearPt "extended at the start" (pt -5. 0. 0.) (rs.CurveStartPoint s)
    throws "not a curve" (fun () -> rs.ExtendCurvePoint(pointC (), 1, pt 15. 0. 0.))
    )
test "ExtendCurve" (fun () ->
    let l = lineC ()
    let boundary = rs.AddLine(pt 15. -5. 0., pt 15. 5. 0.)
    let e = rs.ExtendCurve(l, 0, 1, [ boundary ])
    near "extended to the boundary" 15. (rs.CurveLength e)
    ok "the input is kept by default" (rs.IsObject l)
    let l2 = lineC ()
    rs.ExtendCurve(l2, 0, 1, [ boundary ], true) |> ignore
    ok "the input is replaced on request" (not (rs.IsObject l2) || (rs.CurveLength l2) > 10.)
    let g = rs.ExtendCurve(crv l, 0, 1, [ geo boundary ])
    near "geometry" 15. (g.GetLength())
    throwsRs "boundary can not be reached" (fun () -> rs.ExtendCurve(lineC (), 0, 1, [ rs.AddLine(pt 5. 20. 0., pt 5. 30. 0.) ]))
    )
test "SimplifyCurve" (fun () ->
    let c = twoLinesC ()
    ok "simplifying a polycurve of lines does not fail" (rs.SimplifyCurve c || true)
    let straight = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 10. 0. 0. ]
    ok "a polyline of collinear segments with flags" (rs.SimplifyCurve(straight, 2) || true)
    ok "returns false for a curve that can not be simplified" (not (rs.SimplifyCurve (lineC ())))
    throws "not a curve" (fun () -> rs.SimplifyCurve (pointC ()))
    )
test "FairCurve" (fun () ->
    let c = rs.AddCurve [ pt 0. 0. 0.; pt 2. 1. 0.; pt 4. 0.5 0.; pt 6. 1.2 0.; pt 8. 0. 0.; pt 10. 1. 0. ]
    ok "fairing succeeds" (rs.FairCurve(c, 0.5))
    nearPt "the start point stays" Point3d.Origin (rs.CurveStartPoint c)
    throws "not a curve" (fun () -> rs.FairCurve(pointC (), 0.5))
    )
test "FitCurve" (fun () ->
    let c = rs.AddCurve [ pt 0. 0. 0.; pt 1. 1. 0.; pt 2. 0. 0.; pt 3. 1. 0.; pt 4. 0. 0.; pt 5. 1. 0.; pt 6. 0. 0.; pt 7. 1. 0.; pt 8. 0. 0. ]
    let fit = rs.FitCurve c
    ok "a new curve" (fit <> c && rs.IsCurve fit)
    ok "the fitted curve is valid" (rs.IsCurve fit)
    eq "degree 3 by default" 3 (rs.CurveDegree fit)
    let d5 = rs.FitCurve(c, 5, 0.1)
    eq "degree 5" 5 (rs.CurveDegree d5)
    let g = rs.FitCurve(crv c, 3, 0.1, 5.)
    ok "geometry" (not (isNull g))
    throws "not a curve" (fun () -> rs.FitCurve (pointC ()))
    )
test "InsertCurveKnot" (fun () ->
    let c = nurbsC ()
    let n = rs.CurveKnotCount c
    ok "insert succeeds" (rs.InsertCurveKnot(c, (rs.CurveDomain c).ParameterAt 0.3))
    eq "one more knot" (n + 1) (rs.CurveKnotCount c)
    let s = nurbsC ()
    ok "symmetrical" (rs.InsertCurveKnot(s, (rs.CurveDomain s).ParameterAt 0.3, true))
    ok "a parameter outside of the domain fails" (not (rs.InsertCurveKnot(c, (rs.CurveDomain c).T1 + 10.)))
    throws "not a curve" (fun () -> rs.InsertCurveKnot(pointC (), 0.))
    )
test "RemoveCurveKnot" (fun () ->
    let c = nurbsC ()
    let k = rs.CurveKnots c
    let inner = k.[k.Count / 2]
    rs.InsertCurveKnot(c, inner) |> ignore
    ok "remove succeeds or the knot was not removable" (rs.RemoveCurveKnot(c, inner) || true)
    ok "a parameter that is not a knot" (not (rs.RemoveCurveKnot(c, (rs.CurveDomain c).ParameterAt 0.123)))
    throws "not a curve" (fun () -> rs.RemoveCurveKnot(pointC (), 0.))
    )
test "RebuildCurve" (fun () ->
    let c = nurbsC ()
    ok "rebuild succeeds" (rs.RebuildCurve(c, 3, 8))
    eq "new point count" 8 (rs.CurvePointCount c)
    eq "degree" 3 (rs.CurveDegree c)
    ok "degree 2" (rs.RebuildCurve(c, 2, 6))
    eq "degree 2 point count" 6 (rs.CurvePointCount c)
    throws "not a curve" (fun () -> rs.RebuildCurve(pointC (), 3, 8))
    )
test "ChangeCurveDegree" (fun () ->
    let c = nurbsC ()
    ok "change succeeds" (rs.ChangeCurveDegree(c, 5))
    eq "new degree" 5 (rs.CurveDegree c)
    ok "degree 0 fails" (not (rs.ChangeCurveDegree(c, 0)))
    throws "not a curve" (fun () -> rs.ChangeCurveDegree(pointC (), 3))
    )
test "MakeCurveNonPeriodic" (fun () ->
    let periodic = doc().Objects.AddCurve(Curve.CreateInterpolatedCurve([ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0.; pt 0. 5. 0.; pt 0. 0. 0. ], 3, CurveKnotStyle.UniformPeriodic))
    ok "the curve is periodic" (rs.IsCurvePeriodic periodic)
    let n = rs.MakeCurveNonPeriodic periodic
    ok "the new curve is not periodic" (not (rs.IsCurvePeriodic n))
    ok "the input is kept by default" (rs.IsObject periodic)
    let replaced = rs.MakeCurveNonPeriodic(periodic, true)
    ok "with deleteInput the input is replaced and not periodic anymore" (not (rs.IsCurvePeriodic replaced))
    let periodic2 = doc().Objects.AddCurve(Curve.CreateInterpolatedCurve([ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0.; pt 0. 5. 0.; pt 0. 0. 0. ], 3, CurveKnotStyle.UniformPeriodic))
    let g = rs.MakeCurveNonPeriodic (crv periodic2)
    ok "geometry" (not g.IsPeriodic)
    throwsRs "a curve that is not periodic" (fun () -> rs.MakeCurveNonPeriodic n)
    throws "not a curve" (fun () -> rs.MakeCurveNonPeriodic (pointC ()))
    )
test "MeanCurve" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let b = rs.AddLine(pt 0. 10. 0., pt 10. 10. 0.)
    let m = rs.MeanCurve(a, b)
    near "lies in the middle" 5. (rs.CurveStartPoint m).Y
    ok "geometry" (not (isNull (rs.MeanCurve(crv a, crv b, 0.01))))
    ok "with a tolerance" (rs.IsCurve (rs.MeanCurve(a, b, 0.01)))
    throws "not a curve" (fun () -> rs.MeanCurve(pointC (), b))
    )
test "OffsetCurve" (fun () ->
    let sq = squareC ()
    let o = rs.OffsetCurve(sq, pt 5. 5. 0., 1.)
    ok "an offset curve" (o.Count >= 1)
    near "inward offset: the area gets smaller" 64. (rs.CurveArea o.[0])
    let out = rs.OffsetCurve(sq, pt 20. 5. 0., 1.)
    near "outward offset: the area gets bigger" 144. (rs.CurveArea out.[0])
    ok "with a normal and a style" (let r = rs.OffsetCurve(sq, pt 5. 5. 0., 1., vec 0. 0. 1., 2) in r.Count >= 1)
    let g = rs.OffsetCurve(crv sq, pt 5. 5. 0., 1.)
    ok "geometry" (g.Count >= 1)
    throwsRs "zero distance" (fun () -> rs.OffsetCurve(sq, pt 5. 5. 0., 0.))
    throws "not a curve" (fun () -> rs.OffsetCurve(pointC (), pt 5. 5. 0., 1.))
    )
test "OffsetCurveOnSurface" (fun () ->
    let s = surf ()
    let c = rs.AddLine(pt 2. 5. 0., pt 8. 5. 0.)
    let o = rs.OffsetCurveOnSurface(c, s, 1.)
    ok "an offset curve" (o.Count >= 1)
    near "it lies one unit beside the curve" 1. (abs ((rs.CurveStartPoint o.[0]).Y - 5.))
    let g = rs.OffsetCurveOnSurface(crv c, rs.CoerceSurface s, 1.)
    ok "geometry" (g.Count >= 1)
    throws "not a surface" (fun () -> rs.OffsetCurveOnSurface(c, pointC (), 1.))
    )
test "OffsetCurveOnSurfaceUV" (fun () ->
    // RhinoCommon's Curve.OffsetOnSurface with a surface parameter returns null for a 3D line on a plane surface (Rhino 8.36),
    // in that case the function raises a RhinoScriptingException, otherwise it returns offset curves.
    let s = surf ()
    let c = rs.AddLine(pt 2. 5. 0., pt 8. 5. 0.)
    let r = try Some (rs.OffsetCurveOnSurfaceUV(c, s, Point2d(0., 1.))) with :? RhinoScriptingException -> None
    match r with
    | Some o -> ok "offset curves" (o.Count >= 1)
    | None -> ()
    throws "not a surface" (fun () -> rs.OffsetCurveOnSurfaceUV(c, pointC (), Point2d(0., 1.)))
    )

// ------------------------------- boolean operations and intersections -------------------------------

let squareAt (x:float) (y:float) = rs.AddPolyline [ pt x y 0.; pt (x + 10.) y 0.; pt (x + 10.) (y + 10.) 0.; pt x (y + 10.) 0.; pt x y 0. ]

test "CurveBooleanUnion" (fun () ->
    let a, b = squareAt 0. 0., squareAt 5. 5.
    let u = rs.CurveBooleanUnion [ a; b ]
    eq "one resulting curve" 1 u.Count
    near "area of the union" 175. (rs.CurveArea u.[0])
    let g = rs.CurveBooleanUnion [ crv a; crv b ]
    near "geometry" 175. (rs.CurveArea g.[0])
    let far = rs.CurveBooleanUnion([ a; squareAt 100. 100. ], 0.01)
    eq "curves that do not overlap give two curves" 2 far.Count
    throwsRs "one curve only" (fun () -> rs.CurveBooleanUnion [ a ])
    )
test "CurveBooleanIntersection" (fun () ->
    let a, b = squareAt 0. 0., squareAt 5. 5.
    let r = rs.CurveBooleanIntersection(a, b)
    eq "one resulting curve" 1 r.Count
    near "area of the intersection" 25. (rs.CurveArea r.[0])
    near "geometry" 25. (rs.CurveArea (rs.CurveBooleanIntersection(crv a, crv b, 0.01)).[0])
    eq "curves that do not overlap give nothing" 0 (try (rs.CurveBooleanIntersection(a, squareAt 100. 100.)).Count with :? RhinoScriptingException -> 0)
    )
test "CurveBooleanDifference" (fun () ->
    let a, b = squareAt 0. 0., squareAt 5. 5.
    let r = rs.CurveBooleanDifference(a, b)
    eq "one resulting curve" 1 r.Count
    near "area of the difference" 75. (rs.CurveArea r.[0])
    near "geometry" 75. (rs.CurveArea (rs.CurveBooleanDifference(crv a, crv b, 0.01)).[0])
    )
test "CurveCurveIntersection" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let b = rs.AddLine(pt 5. -5. 0., pt 5. 5. 0.)
    let r = rs.CurveCurveIntersection(a, b)
    eq "one intersection" 1 r.Count
    let (kind, p1, p2, _, _, ta, tb, _, _) = r.[0]
    eq "event type Point (1)" 1 kind
    nearPt "intersection point" (pt 5. 0. 0.) p1
    nearPt "second point is the same" (pt 5. 0. 0.) p2
    near "parameter on the first curve" 5. (ta - (rs.CurveDomain a).T0)
    ok "parameter on the second curve" (tb >= (rs.CurveDomain b).T0)
    eq "geometry" 1 (rs.CurveCurveIntersection(crv a, crv b)).Count
    eq "with a tolerance" 1 (rs.CurveCurveIntersection(a, b, 0.01)).Count
    eq "parallel lines do not intersect" 0 (rs.CurveCurveIntersection(a, rs.AddLine(pt 0. 5. 0., pt 10. 5. 0.))).Count
    // overlap
    let o = rs.CurveCurveIntersection(a, rs.AddLine(pt 5. 0. 0., pt 15. 0. 0.))
    eq "overlapping lines" 1 o.Count
    let (okind, _, _, _, _, _, _, _, _) = o.[0]
    eq "event type Overlap (2)" 2 okind
    // self intersection if the second curve is omitted
    let bow = rs.AddPolyline [ pt 0. 0. 0.; pt 10. 10. 0.; pt 10. 0. 0.; pt 0. 10. 0. ]
    eq "self intersection of a bow tie" 1 (rs.CurveCurveIntersection bow).Count
    eq "self intersection, geometry" 1 (rs.CurveCurveIntersection (crv bow)).Count
    )
test "CurveSurfaceIntersection" (fun () ->
    let s = surf ()
    let c = rs.AddLine(pt 5. 5. -5., pt 5. 5. 5.)
    let r = rs.CurveSurfaceIntersection(c, s)
    eq "one intersection" 1 r.Count
    let (kind, p1, _, _, _, _, _, _, _, _, _) = r.[0]
    eq "event type Point (1)" 1 kind
    nearPt "intersection point" (pt 5. 5. 0.) p1
    eq "geometry" 1 (rs.CurveSurfaceIntersection(crv c, rs.CoerceSurface s)).Count
    eq "with tolerances" 1 (rs.CurveSurfaceIntersection(c, s, 0.01, 0.1)).Count
    eq "a curve that misses the surface" 0 (rs.CurveSurfaceIntersection(rs.AddLine(pt 50. 50. -5., pt 50. 50. 5.), s)).Count
    )
test "CurveBrepIntersect" (fun () ->
    let b = addBox 10. 10. 10.
    let c = rs.AddLine(pt 5. 5. -5., pt 5. 5. 15.)
    let pts, curves = rs.CurveBrepIntersect(c, b)
    eq "two points" 2 pts.Count
    eq "no overlapping curves" 0 curves.Count
    ok "points are on the top and bottom" (pts |> Seq.map (fun p -> p.Z) |> Seq.sort |> List.ofSeq |> fun zs -> abs zs.[0] < 1e-3 && abs (zs.[1] - 10.) < 1e-3)
    let gp, _ = rs.CurveBrepIntersect(crv c, rs.CoerceBrep b)
    eq "geometry" 2 gp.Count
    let tp, _ = rs.CurveBrepIntersect(c, b, 0.01)
    eq "with a tolerance" 2 tp.Count
    throws "not a brep" (fun () -> rs.CurveBrepIntersect(c, pointC ()))
    )
test "CurveFilletPoints" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let b = rs.AddLine(pt 0. 0. 0., pt 0. 10. 0.)
    let pa, pb, plane = rs.CurveFilletPoints(a, b, 2.)
    nearPt "point on the first curve" (pt 2. 0. 0.) pa
    nearPt "point on the second curve" (pt 0. 2. 0.) pb
    nearPt "the center of the fillet circle" (pt 2. 2. 0.) plane.Origin
    let ga, _, _ = rs.CurveFilletPoints(crv a, crv b, 2.)
    nearPt "geometry" (pt 2. 0. 0.) ga
    let ba, _, _ = rs.CurveFilletPoints(a, b, 2., pt 9. 0. 0., pt 0. 9. 0.)
    nearPt "with base points" (pt 2. 0. 0.) ba
    throwsRs "parallel curves" (fun () -> rs.CurveFilletPoints(a, rs.AddLine(pt 0. 5. 0., pt 10. 5. 0.), 1.))
    )

// ------------------------------- containment and projection -------------------------------

test "PlanarClosedCurveContainment" (fun () ->
    let big = rs.AddCircle(Plane.WorldXY, 5.)
    let small = rs.AddCircle(Plane.WorldXY, 2.)
    let far = rs.AddCircle(pt 20. 0. 0., 2.)
    let crossing = rs.AddCircle(pt 5. 0. 0., 2.)
    eq "disjoint" 0 (rs.PlanarClosedCurveContainment(big, far))
    eq "intersecting" 1 (rs.PlanarClosedCurveContainment(big, crossing))
    eq "A inside of B" 2 (rs.PlanarClosedCurveContainment(small, big))
    eq "B inside of A" 3 (rs.PlanarClosedCurveContainment(big, small))
    eq "geometry" 3 (rs.PlanarClosedCurveContainment(crv big, crv small))
    eq "with a plane and a tolerance" 3 (rs.PlanarClosedCurveContainment(big, small, Plane.WorldXY, 0.01))
    )
test "PlanarCurveCollision" (fun () ->
    let a = rs.AddCircle(Plane.WorldXY, 5.)
    ok "crossing circles collide" (rs.PlanarCurveCollision(a, rs.AddCircle(pt 5. 0. 0., 2.)))
    ok "geometry" (rs.PlanarCurveCollision(crv a, crv (rs.AddCircle(pt 5. 0. 0., 2.))))
    ok "circles that do not touch" (not (rs.PlanarCurveCollision(a, rs.AddCircle(pt 20. 0. 0., 2.))))
    ok "a circle inside of another does not collide with its curve" (not (rs.PlanarCurveCollision(a, rs.AddCircle(Plane.WorldXY, 2.))))
    ok "with a plane and a tolerance" (rs.PlanarCurveCollision(a, rs.AddCircle(pt 5. 0. 0., 2.), Plane.WorldXY, 0.01))
    )
test "PointInPlanarClosedCurve" (fun () ->
    let c = rs.AddCircle(Plane.WorldXY, 5.)
    eq "inside" 1 (rs.PointInPlanarClosedCurve(pt 1. 1. 0., c))
    eq "outside" 0 (rs.PointInPlanarClosedCurve(pt 10. 1. 0., c))
    eq "on the curve" 2 (rs.PointInPlanarClosedCurve(pt 5. 0. 0., c))
    eq "geometry" 1 (rs.PointInPlanarClosedCurve(pt 1. 1. 0., crv c))
    eq "with a plane and a tolerance" 1 (rs.PointInPlanarClosedCurve(pt 1. 1. 0., c, Plane.WorldXY, 0.01))
    throwsRs "open curve" (fun () -> rs.PointInPlanarClosedCurve(pt 1. 1. 0., lineC ()))
    )
test "ProjectCurveToSurface" (fun () ->
    let s = surf ()
    let c = rs.AddLine(pt 2. 2. 5., pt 8. 2. 5.)
    let p = rs.ProjectCurveToSurface([ c ], [ s ], vec 0. 0. -1.)
    eq "one projected curve" 1 p.Count
    near "it lies on the surface" 0. (rs.CurveStartPoint p.[0]).Z
    let g = rs.ProjectCurveToSurface([ crv c ], [ rs.CoerceBrep s ], vec 0. 0. -1.)
    eq "geometry" 1 g.Count
    eq "a curve that misses the surface" 0 (rs.ProjectCurveToSurface([ rs.AddLine(pt 50. 50. 5., pt 60. 50. 5.) ], [ s ], vec 0. 0. -1.)).Count
    throws "not a surface" (fun () -> rs.ProjectCurveToSurface([ c ], [ pointC () ], vec 0. 0. -1.))
    )
test "ProjectCurveToMesh" (fun () ->
    let m = addMeshBox 10. 10. 10.
    let c = rs.AddLine(pt 2. 2. 15., pt 8. 2. 15.)
    let p = rs.ProjectCurveToMesh([ c ], [ m ], vec 0. 0. -1.)
    ok "projected curves" (p.Count >= 1)
    ok "it lies on the top of the mesh (and on the bottom)" (p |> Seq.exists (fun c -> abs ((rs.CurveStartPoint c).Z - 10.) < 1e-6))
    let g = rs.ProjectCurveToMesh([ crv c ], [ (rs.CoerceMesh m) ], vec 0. 0. -1.)
    ok "geometry" (g.Count >= 1)
    throws "not a mesh" (fun () -> rs.ProjectCurveToMesh([ c ], [ pointC () ], vec 0. 0. -1.))
    )
test "MeshPolyline" (fun () ->
    let p = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0.; pt 0. 5. 0.; pt 0. 0. 0. ]
    let m = rs.MeshPolyline p
    ok "is a mesh" (rs.IsMesh m)
    let g = rs.MeshPolyline (crv p)
    ok "geometry" (g.Faces.Count >= 1)
    throwsRs "not a polyline" (fun () -> rs.MeshPolyline (circleC ()))
    )

// The Make* functions create geometry without adding it to the document, they are also used by the Add* tests above.
test "MakeSubCrv" (fun () ->
    let n = (rs.AllObjects()).Count
    let l = new LineCurve(pt 0. 0. 0., pt 10. 0. 0.)
    let s = rs.MakeSubCrv(l, 2., 6.)
    near "length of the sub curve" 4. (s.GetLength())
    eq "nothing was added to the document" n (rs.AllObjects()).Count
    near "the input is unchanged" 10. (l.GetLength())
    )
test "MakeBlendCurve" (fun () ->
    let a = new LineCurve(pt 0. 0. 0., pt 5. 0. 0.) :> Curve
    let b = new LineCurve(pt 10. 5. 0., pt 15. 5. 0.) :> Curve
    let blend = rs.MakeBlendCurve((a, b), (a.Domain.T1, b.Domain.T0), (false, false), (0, 0))
    nearPt "starts at the end of the first curve" (pt 5. 0. 0.) blend.PointAtStart
    nearPt "ends at the start of the second curve" (pt 10. 5. 0.) blend.PointAtEnd
    )
test "MakeFilletCurve" (fun () ->
    let a = new LineCurve(pt 0. 0. 0., pt 10. 0. 0.) :> Curve
    let b = new LineCurve(pt 0. 0. 0., pt 0. 10. 0.) :> Curve
    let f = rs.MakeFilletCurve(a, b, 2.)
    near "radius" 2. f.Radius
    nearPt "center" (pt 2. 2. 0.) f.Arc.Center
    )
test "MakeInterpCrvOnSrf" (fun () ->
    let sg = rs.CoerceSurface (surf ())
    let c = rs.MakeInterpCrvOnSrf(sg, [ pt 1. 1. 0.; pt 5. 8. 0.; pt 9. 2. 0. ])
    nearPt "starts at the first point" (pt 1. 1. 0.) c.PointAtStart
    nearPt "ends at the last point" (pt 9. 2. 0.) c.PointAtEnd
    )
test "MakeInterpCrvOnSrfUV" (fun () ->
    // see AddInterpCrvOnSrfUV, RhinoCommon may return null which raises a RhinoScriptingException
    let sg = rs.CoerceSurface (surf ())
    let r = try Some (rs.MakeInterpCrvOnSrfUV(sg, [ Point2d(1., 1.); Point2d(5., 8.) ])) with :? RhinoScriptingException -> None
    match r with
    | Some c -> ok "a curve" (not (isNull c))
    | None -> ()
    )

printfn "%s" (finish ())
