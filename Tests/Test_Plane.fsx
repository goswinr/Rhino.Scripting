// Tests for Src/Scripting_Plane.fs
#load "Harness.fsx"
open System
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Plane"

let vec x y z = Vector3d(x, y, z)
let planeAtZ (z:float) = Plane(pt 0. 0. z, Vector3d.ZAxis)

test "DistanceToPlane" (fun () ->
    near "above the plane" 5. (rs.DistanceToPlane(Plane.WorldXY, pt 1. 2. 5.))
    near "below the plane the distance is signed" -5. (rs.DistanceToPlane(Plane.WorldXY, pt 1. 2. -5.))
    near "on the plane" 0. (rs.DistanceToPlane(Plane.WorldXY, pt 7. 8. 0.))
    )

test "EvaluatePlane" (fun () ->
    let p = Plane(pt 1. 1. 1., Vector3d.ZAxis)
    nearPt "u and v along the axes" (pt 3. 4. 1.) (rs.EvaluatePlane(p, 2., 3.))
    nearPt "0,0 is the origin" (pt 1. 1. 1.) (rs.EvaluatePlane(p, 0., 0.))
    nearPt "negative parameters" (pt 0. -1. 1.) (rs.EvaluatePlane(p, -1., -2.))
    )

test "IntersectPlanes" (fun () ->
    let p1 = planeAtZ 1.
    let p2 = Plane(pt 2. 0. 0., Vector3d.XAxis)
    let p3 = Plane(pt 0. 3. 0., Vector3d.YAxis)
    nearPt "three orthogonal planes" (pt 2. 3. 1.) (rs.IntersectPlanes(p1, p2, p3))
    throwsRs "two parallel planes" (fun () -> rs.IntersectPlanes(p1, planeAtZ 2., p3))
    )

test "MovePlane" (fun () ->
    let p = Plane.WorldXY
    let moved = rs.MovePlane(p, pt 1. 2. 3.)
    nearPt "new origin" (pt 1. 2. 3.) moved.Origin
    nearVec "orientation is kept" Vector3d.ZAxis moved.Normal
    nearPt "the input plane is not modified" Point3d.Origin p.Origin
    )

test "FlipPlane" (fun () ->
    let p = Plane(pt 1. 2. 3., Vector3d.XAxis, Vector3d.YAxis)
    let f = rs.FlipPlane p
    nearVec "normal is inverted" (-Vector3d.ZAxis) f.Normal
    nearVec "x axis is the old y axis" Vector3d.YAxis f.XAxis
    nearVec "y axis is the old x axis" Vector3d.XAxis f.YAxis
    nearPt "origin is kept" (pt 1. 2. 3.) f.Origin
    nearVec "the input plane is not modified" Vector3d.ZAxis p.Normal
    )

test "PlaneClosestPoint" (fun () ->
    nearPt "projected onto the plane" (pt 3. 4. 0.) (rs.PlaneClosestPoint(Plane.WorldXY, pt 3. 4. 5.))
    nearPt "point already on the plane" (pt 3. 4. 0.) (rs.PlaneClosestPoint(Plane.WorldXY, pt 3. 4. 0.))
    )

test "PlaneClosestParameter" (fun () ->
    let u, v = rs.PlaneClosestParameter(Plane(pt 1. 1. 0., Vector3d.ZAxis), pt 4. 6. 9.)
    near "u" 3. u
    near "v" 5. v
    )

test "PlaneCurveIntersection" (fun () ->
    // curve crossing the plane in one point, Guid overload
    let c = rs.AddLine(pt 2. 0. -5., pt 2. 0. 5.)
    let r = rs.PlaneCurveIntersection(Plane.WorldXY, c)
    eq "one intersection event" 1 r.Count
    let (kind, p1, p2, pp1, pp2, t1, t2, u1, v1, u2, v2) = r.[0]
    eq "event type is Point (1)" 1 kind
    nearPt "point on the curve" (pt 2. 0. 0.) p1
    nearPt "point on the curve, second" (pt 2. 0. 0.) p2
    nearPt "point on the plane" (pt 2. 0. 0.) pp1
    nearPt "point on the plane, second" (pt 2. 0. 0.) pp2
    near "curve parameter is the middle of the domain" 0.5 (let d = rs.CurveDomain c in (t1 - d.[0]) / (d.[1] - d.[0]))
    near "curve parameters are the same for a point event" t1 t2
    near "plane u" 2. u1
    near "plane v" 0. v1
    near "plane u of second" 2. u2
    near "plane v of second" 0. v2
    // geometry overload and explicit tolerance
    let geo = new LineCurve(pt 0. 0. -1., pt 0. 0. 1.)
    eq "geometry overload" 1 (rs.PlaneCurveIntersection(Plane.WorldXY, geo, 0.001)).Count
    // overlap: curve lies in the plane
    let inPlane = rs.AddLine(pt 0. 0. 0., pt 5. 0. 0.)
    let o = rs.PlaneCurveIntersection(Plane.WorldXY, inPlane)
    eq "one overlap event" 1 o.Count
    let (okind, os, oe, _, _, _, _, _, _, _, _) = o.[0]
    eq "event type is Overlap (2)" 2 okind
    nearPt "overlap start" (pt 0. 0. 0.) os
    nearPt "overlap end" (pt 5. 0. 0.) oe
    // no intersection
    let none = rs.AddLine(pt 0. 0. 1., pt 5. 0. 1.)
    ok "no intersection raises or returns an empty list" (try (rs.PlaneCurveIntersection(Plane.WorldXY, none)).Count = 0 with :? RhinoScriptingException -> true)
    throws "not a curve object" (fun () -> rs.PlaneCurveIntersection(Plane.WorldXY, rs.AddPoint(1., 2., 3.)))
    )

test "PlaneEquation" (fun () ->
    let a, b, c, d = rs.PlaneEquation Plane.WorldXY
    eq "XY plane equation" (0., 0., 1., 0.) (a, b, c, d)
    let a, b, c, d = rs.PlaneEquation (planeAtZ 5.)
    near "A" 0. a
    near "B" 0. b
    near "C" 1. c
    near "D" -5. d
    )

test "PlaneFitFromPoints" (fun () ->
    let p = rs.PlaneFitFromPoints [ pt 0. 0. 3.; pt 5. 0. 3.; pt 0. 5. 3.; pt 5. 5. 3. ]
    near "plane is at z=3" 0. (abs (p.DistanceTo (pt 100. -50. 3.)))
    near "normal is vertical" 1. (abs p.Normal.Z)
    let p2 = rs.PlaneFitFromPoints [ pt 0. 0. 0.; pt 5. 0. 0.2; pt 0. 5. -0.2; pt 5. 5. 0. ]
    ok "points that are almost planar still give a valid plane" p2.IsValid
    throwsRs "a single point can not be fitted" (fun () -> rs.PlaneFitFromPoints [ pt 0. 0. 0. ])
    )

test "PlaneFromFrame" (fun () ->
    let p = rs.PlaneFromFrame(pt 1. 2. 3., vec 1. 0. 0., vec 1. 1. 0.)
    nearPt "origin" (pt 1. 2. 3.) p.Origin
    nearVec "x axis" Vector3d.XAxis p.XAxis
    nearVec "y axis is made perpendicular to x" Vector3d.YAxis p.YAxis
    nearVec "normal" Vector3d.ZAxis p.Normal
    )

test "PlaneFromNormal" (fun () ->
    let p = rs.PlaneFromNormal(pt 1. 2. 3., vec 0. 0. 5.)
    nearPt "origin" (pt 1. 2. 3.) p.Origin
    nearVec "normal is unitized" Vector3d.ZAxis p.Normal
    let xa = vec 1. 1. 0.
    let p2 = rs.PlaneFromNormal(Point3d.Origin, vec 0. 0. 1., xa)
    nearVec "optional x axis is used" (vec (1./sqrt 2.) (1./sqrt 2.) 0.) p2.XAxis
    nearVec "normal with x axis" Vector3d.ZAxis p2.Normal
    nearVec "the input x axis is not unitized" (vec 1. 1. 0.) xa
    )

test "PlaneFromPoints" (fun () ->
    let p = rs.PlaneFromPoints(pt 1. 1. 1., pt 2. 1. 1., pt 1. 2. 1.)
    nearPt "origin" (pt 1. 1. 1.) p.Origin
    nearVec "x axis" Vector3d.XAxis p.XAxis
    nearVec "normal" Vector3d.ZAxis p.Normal
    throwsRs "collinear points" (fun () -> rs.PlaneFromPoints(pt 0. 0. 0., pt 1. 0. 0., pt 2. 0. 0.))
    )

test "PlanePlaneIntersection" (fun () ->
    let l = rs.PlanePlaneIntersection(Plane.WorldXY, Plane.WorldZX)
    near "the line is along the x axis" 1. (abs (l.Direction / l.Length * Vector3d.XAxis))
    near "the line is on the x axis" 0. (l.ClosestPoint(Point3d.Origin, false).DistanceTo Point3d.Origin)
    throwsRs "parallel planes" (fun () -> rs.PlanePlaneIntersection(Plane.WorldXY, planeAtZ 1.))
    )

test "PlaneSphereIntersection" (fun () ->
    let kind, pl, r = rs.PlaneSphereIntersection(Plane.WorldXY, Plane.WorldXY, 5.)
    eq "circle intersection has type 1" 1 kind
    near "circle radius" 5. r
    nearPt "circle center" Point3d.Origin pl.Origin
    let kind2, pl2, r2 = rs.PlaneSphereIntersection(planeAtZ 3., Plane.WorldXY, 5.)
    eq "off center circle has type 1" 1 kind2
    near "circle radius 4" 4. r2
    nearPt "circle center is on the plane" (pt 0. 0. 3.) pl2.Origin
    let kind3, pl3, r3 = rs.PlaneSphereIntersection(planeAtZ 5., Plane.WorldXY, 5.)
    ok "tangent plane gives a point (0) or a degenerate circle" ((kind3 = 0 && r3 < 1e-3) || kind3 = 1)
    nearPt "tangent plane touches the sphere at the pole" (pt 0. 0. 5.) pl3.Origin
    throwsRs "plane misses the sphere" (fun () -> rs.PlaneSphereIntersection(planeAtZ 10., Plane.WorldXY, 5.))
    )

test "PlaneTransform" (fun () ->
    let p = Plane.WorldXY
    let t = rs.PlaneTransform(p, Transform.Translation(1., 2., 3.))
    nearPt "translated origin" (pt 1. 2. 3.) t.Origin
    nearPt "input plane not modified" Point3d.Origin p.Origin
    let r = rs.PlaneTransform(p, Transform.Rotation(Math.PI / 2., Vector3d.ZAxis, Point3d.Origin))
    nearVec "rotated x axis" Vector3d.YAxis r.XAxis
    )

test "RotatePlane" (fun () ->
    let p = Plane.WorldXY
    let r = rs.RotatePlane(p, 90., Vector3d.ZAxis)
    nearVec "x axis turned to y" Vector3d.YAxis r.XAxis
    nearVec "normal unchanged" Vector3d.ZAxis r.Normal
    nearVec "the input plane is not modified" Vector3d.XAxis p.XAxis
    let r2 = rs.RotatePlane(p, 90., Vector3d.XAxis)
    nearVec "rotated around x: normal turned to -y" (-Vector3d.YAxis) r2.Normal
    )

test "WorldXYPlane" (fun () -> nearVec "normal" Vector3d.ZAxis (rs.WorldXYPlane()).Normal)
test "WorldYZPlane" (fun () -> nearVec "normal" Vector3d.XAxis (rs.WorldYZPlane()).Normal)
test "WorldZXPlane" (fun () -> nearVec "normal" Vector3d.YAxis (rs.WorldZXPlane()).Normal)

printfn "%s" (finish ())
