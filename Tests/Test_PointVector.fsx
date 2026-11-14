// Tests for Src/Scripting_PointVector.fs
#load "Harness.fsx"
open System
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "PointVector"

let vec x y z = Vector3d(x, y, z)

// ------------------------------- vectors -------------------------------

test "IsVectorParallelTo" (fun () ->
    eq "parallel" 1 (rs.IsVectorParallelTo(vec 1. 0. 0., vec 5. 0. 0.))
    eq "anti-parallel" -1 (rs.IsVectorParallelTo(vec 1. 0. 0., vec -3. 0. 0.))
    eq "not parallel" 0 (rs.IsVectorParallelTo(vec 1. 0. 0., vec 0. 1. 0.))
    eq "0.5 degrees off is parallel within the default 1 degree" 1 (rs.IsVectorParallelTo(vec 1. 0. 0., vec 1. 0.0087 0.))
    eq "3 degrees off is not parallel within the default 1 degree" 0 (rs.IsVectorParallelTo(vec 1. 0. 0., vec 1. 0.0524 0.))
    eq "3 degrees off is parallel within a custom 5 degree tolerance" 1 (rs.IsVectorParallelTo(vec 1. 0. 0., vec 1. 0.0524 0., 5.))
    eq "anti-parallel within a custom tolerance" -1 (rs.IsVectorParallelTo(vec 1. 0. 0., vec -1. 0.0524 0., 5.))
    )

test "IsVectorPerpendicularTo" (fun () ->
    ok "perpendicular" (rs.IsVectorPerpendicularTo(vec 1. 0. 0., vec 0. 2. 0.))
    ok "parallel is not perpendicular" (not (rs.IsVectorPerpendicularTo(vec 1. 0. 0., vec 1. 0. 0.)))
    ok "3 degrees off is not perpendicular within the default 1 degree" (not (rs.IsVectorPerpendicularTo(vec 1. 0. 0., vec -0.0524 1. 0.)))
    ok "3 degrees off is perpendicular within a custom 5 degree tolerance" (rs.IsVectorPerpendicularTo(vec 1. 0. 0., vec -0.0524 1. 0., 5.))
    )

test "IsVectorTiny" (fun () ->
    ok "1e-13 is tiny" (rs.IsVectorTiny(vec 1e-13 1e-13 1e-13))
    ok "1e-6 is not tiny" (not (rs.IsVectorTiny(vec 1e-6 0. 0.)))
    ok "zero vector is tiny" (rs.IsVectorTiny(vec 0. 0. 0.))
    )

test "IsVectorZero" (fun () ->
    ok "zero vector" (rs.IsVectorZero(vec 0. 0. 0.))
    ok "tiny vector is not exactly zero" (not (rs.IsVectorZero(vec 1e-13 0. 0.)))
    ok "unit vector" (not (rs.IsVectorZero(vec 1. 0. 0.)))
    )

test "VectorAdd" (fun () -> nearVec "sum" (vec 5. 7. 9.) (rs.VectorAdd(vec 1. 2. 3., vec 4. 5. 6.)))

test "VectorAngle" (fun () ->
    near "90 degrees" 90. (rs.VectorAngle(vec 1. 0. 0., vec 0. 3. 0.))
    near "0 degrees" 0. (rs.VectorAngle(vec 1. 0. 0., vec 7. 0. 0.))
    near "180 degrees" 180. (rs.VectorAngle(vec 1. 0. 0., vec -2. 0. 0.))
    near "45 degrees" 45. (rs.VectorAngle(vec 1. 0. 0., vec 1. 1. 0.))
    throwsRs "zero vector can not be unitized" (fun () -> rs.VectorAngle(vec 0. 0. 0., vec 1. 0. 0.))
    )

test "VectorCompare" (fun () ->
    eq "equal" 0 (rs.VectorCompare(vec 1. 2. 3., vec 1. 2. 3.))
    eq "less" -1 (rs.VectorCompare(vec 1. 2. 3., vec 2. 0. 0.))
    eq "greater" 1 (rs.VectorCompare(vec 2. 0. 0., vec 1. 2. 3.))
    eq "x equal, y decides" -1 (rs.VectorCompare(vec 1. 1. 9., vec 1. 2. 0.))
    )

test "VectorCreate" (fun () ->
    nearVec "from p1 to p2" (vec 3. 3. 3.) (rs.VectorCreate(pt 1. 1. 1., pt 4. 4. 4.))
    nearVec "reversed order gives the reversed vector" (vec -3. -3. -3.) (rs.VectorCreate(pt 4. 4. 4., pt 1. 1. 1.))
    )

test "VectorCrossProduct" (fun () ->
    nearVec "x cross y is z" (vec 0. 0. 1.) (rs.VectorCrossProduct(vec 1. 0. 0., vec 0. 1. 0.))
    nearVec "y cross x is -z" (vec 0. 0. -1.) (rs.VectorCrossProduct(vec 0. 1. 0., vec 1. 0. 0.))
    nearVec "parallel vectors give the zero vector" (vec 0. 0. 0.) (rs.VectorCrossProduct(vec 1. 0. 0., vec 2. 0. 0.))
    )

test "VectorDivide" (fun () ->
    nearVec "divide" (vec 1. 2. 3.) (rs.VectorDivide(vec 2. 4. 6., 2.))
    throwsRs "divide by zero" (fun () -> rs.VectorDivide(vec 2. 4. 6., 0.))
    throwsRs "divide by almost zero" (fun () -> rs.VectorDivide(vec 2. 4. 6., 1e-20))
    )

test "VectorDotProduct" (fun () ->
    near "dot" 32. (rs.VectorDotProduct(vec 1. 2. 3., vec 4. 5. 6.))
    near "perpendicular vectors" 0. (rs.VectorDotProduct(vec 1. 0. 0., vec 0. 1. 0.))
    )

test "VectorLength" (fun () ->
    near "3-4-5" 5. (rs.VectorLength(vec 3. 4. 0.))
    near "zero vector" 0. (rs.VectorLength(vec 0. 0. 0.))
    )

test "VectorMultiply" (fun () -> near "same as dot product" 32. (rs.VectorMultiply(vec 1. 2. 3., vec 4. 5. 6.)))

test "VectorReverse" (fun () ->
    let v = vec 1. -2. 3.
    nearVec "reversed" (vec -1. 2. -3.) (rs.VectorReverse v)
    nearVec "input unchanged" (vec 1. -2. 3.) v
    )

test "VectorRotate" (fun () ->
    nearVec "x around z by 90" (vec 0. 1. 0.) (rs.VectorRotate(vec 1. 0. 0., 90., vec 0. 0. 1.))
    nearVec "x around x stays" (vec 1. 0. 0.) (rs.VectorRotate(vec 1. 0. 0., 90., vec 1. 0. 0.))
    let v = vec 1. 0. 0.
    rs.VectorRotate(v, 90., vec 0. 0. 1.) |> ignore
    nearVec "input unchanged" (vec 1. 0. 0.) v
    )

test "VectorScale" (fun () ->
    let v = vec 1. 2. 3.
    nearVec "scaled" (vec 2. 4. 6.) (rs.VectorScale(v, 2.))
    nearVec "input unchanged" (vec 1. 2. 3.) v
    nearVec "scale by zero" (vec 0. 0. 0.) (rs.VectorScale(v, 0.))
    )

test "VectorSubtract" (fun () -> nearVec "difference" (vec -3. -3. -3.) (rs.VectorSubtract(vec 1. 2. 3., vec 4. 5. 6.)))

test "VectorTransform" (fun () ->
    nearVec "translation does not move a vector" (vec 1. 2. 3.) (rs.VectorTransform(vec 1. 2. 3., Transform.Translation(10., 10., 10.)))
    nearVec "scaling" (vec 2. 4. 6.) (rs.VectorTransform(vec 1. 2. 3., Transform.Scale(Plane.WorldXY, 2., 2., 2.)))
    nearVec "rotation" (vec 0. 1. 0.) (rs.VectorTransform(vec 1. 0. 0., Transform.Rotation(Math.PI / 2., Vector3d.ZAxis, Point3d.Origin)))
    )

test "VectorUnitize" (fun () ->
    let v = vec 3. 4. 0.
    nearVec "unitized" (vec 0.6 0.8 0.) (rs.VectorUnitize v)
    nearVec "input unchanged" (vec 3. 4. 0.) v
    throwsRs "zero vector" (fun () -> rs.VectorUnitize (vec 0. 0. 0.))
    throwsRs "very short vector" (fun () -> rs.VectorUnitize (vec 1e-20 0. 0.))
    )

// ------------------------------- points -------------------------------

test "PointAdd" (fun () -> nearPt "sum" (pt 5. 7. 9.) (rs.PointAdd(pt 1. 2. 3., pt 4. 5. 6.)))

test "PointSubtract" (fun () -> nearPt "difference" (pt -3. -3. -3.) (rs.PointSubtract(pt 1. 2. 3., pt 4. 5. 6.)))

test "PointScale" (fun () ->
    let p = pt 1. 2. 3.
    nearPt "scaled" (pt 2. 4. 6.) (rs.PointScale(p, 2.))
    nearPt "input unchanged" (pt 1. 2. 3.) p
    )

test "PointDivide" (fun () ->
    nearPt "divided" (pt 1. 2. 3.) (rs.PointDivide(pt 2. 4. 6., 2.))
    throwsRs "divide by zero" (fun () -> rs.PointDivide(pt 2. 4. 6., 0.))
    throwsRs "divide by almost zero" (fun () -> rs.PointDivide(pt 2. 4. 6., -1e-20))
    )

test "PointCompare" (fun () ->
    ok "equal points" (rs.PointCompare(pt 1. 2. 3., pt 1. 2. 3.))
    ok "different points" (not (rs.PointCompare(pt 1. 2. 3., pt 1. 2. 3.1)))
    ok "within a custom tolerance" (rs.PointCompare(pt 1. 2. 3., pt 1. 2. 3.01, 0.1))
    ok "outside a custom tolerance" (not (rs.PointCompare(pt 1. 2. 3., pt 1. 2. 3.5, 0.1)))
    )

test "PointTransform" (fun () ->
    let p = pt 1. 2. 3.
    nearPt "translated" (pt 11. 12. 13.) (rs.PointTransform(p, Transform.Translation(10., 10., 10.)))
    nearPt "input unchanged" (pt 1. 2. 3.) p
    nearPt "rotated" (pt -2. 1. 3.) (rs.PointTransform(p, Transform.Rotation(Math.PI / 2., Vector3d.ZAxis, Point3d.Origin)))
    )

test "PointArrayTransform" (fun () ->
    let pts = ResizeArray [ pt 0. 0. 0.; pt 1. 0. 0. ]
    let r = rs.PointArrayTransform(pts, Transform.Translation(0., 0., 5.))
    eq "same count" 2 r.Count
    nearPt "first" (pt 0. 0. 5.) r.[0]
    nearPt "second" (pt 1. 0. 5.) r.[1]
    nearPt "input unchanged" (pt 0. 0. 0.) pts.[0]
    eq "empty list" 0 (rs.PointArrayTransform(ResizeArray<Point3d>(), Transform.Identity)).Count
    )

test "PointArrayClosestPoint" (fun () ->
    let pts = ResizeArray [ pt 0. 0. 0.; pt 10. 0. 0.; pt 5. 5. 0. ]
    eq "closest to the second point" 1 (rs.PointArrayClosestPoint(pts, pt 9. 1. 0.))
    eq "closest to the third point" 2 (rs.PointArrayClosestPoint(pts, pt 5. 4. 0.))
    throwsRs "empty list" (fun () -> rs.PointArrayClosestPoint(ResizeArray<Point3d>(), pt 0. 0. 0.))
    )

test "PointArrayBoundingBox" (fun () ->
    let pts = [ pt 1. 2. 3.; pt 4. 6. 9.; pt 2. 3. 4. ]
    let b = rs.PointArrayBoundingBox pts
    nearPt "min corner" (pt 1. 2. 3.) (b.BoundingBox.Min)
    nearPt "max corner" (pt 4. 6. 9.) (b.BoundingBox.Max)
    // aligned to a plane
    let pl = Plane(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis)
    let b2 = rs.PointArrayBoundingBox(pts, pl)
    near "box is aligned to the plane: x size" 3. (b2.X.Length)
    near "box is aligned to the plane: y size" 4. (b2.Y.Length)
    near "box is aligned to the plane: z size" 6. (b2.Z.Length)
    // rotated plane gives a different (not world axis aligned) box
    let rot = Plane(Point3d.Origin, vec 1. 1. 0., vec -1. 1. 0.)
    let b3 = rs.PointArrayBoundingBox([ pt 0. 0. 0.; pt 1. 1. 0.; pt 2. 2. 0. ], rot)
    near "plane aligned box along the diagonal" (2. * sqrt 2.) (b3.X.Length)
    near "plane aligned box has no width" 0. (b3.Y.Length)
    )

test "PointsAreCoplanar" (fun () ->
    ok "points in a plane" (rs.PointsAreCoplanar [ pt 0. 0. 0.; pt 1. 0. 0.; pt 0. 1. 0.; pt 1. 1. 0. ])
    ok "points not in a plane" (not (rs.PointsAreCoplanar [ pt 0. 0. 0.; pt 1. 0. 0.; pt 0. 1. 0.; pt 1. 1. 1. ]))
    ok "custom tolerance accepts a small deviation" (rs.PointsAreCoplanar([ pt 0. 0. 0.; pt 1. 0. 0.; pt 0. 1. 0.; pt 1. 1. 0.01 ], 0.1))
    ok "default tolerance rejects the same deviation" (not (rs.PointsAreCoplanar [ pt 0. 0. 0.; pt 1. 0. 0.; pt 0. 1. 0.; pt 1. 1. 0.01 ]))
    )

test "PointClosestObject" (fun () ->
    let c1 = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let c2 = rs.AddLine(pt 0. 5. 0., pt 10. 5. 0.)
    let id, p, d = rs.PointClosestObject(pt 3. 4. 0., [ c1; c2 ])
    eq "closest curve is the second" c2 id
    nearPt "closest point on it" (pt 3. 5. 0.) p
    near "distance" 1. d
    let pointId = rs.AddPoint(3., 4., 0.5)
    let id2, _, d2 = rs.PointClosestObject(pt 3. 4. 0., [ c1; pointId ])
    eq "a point object can be closest" pointId id2
    near "distance to the point object" 0.5 d2
    let box = addBox 2. 2. 2.
    let id3, p3, d3 = rs.PointClosestObject(pt 5. 1. 1., [ c2; box ])
    eq "a polysurface can be closest" box id3
    nearPt "closest point on the box" (pt 2. 1. 1.) p3
    near "distance to the box" 3. d3
    // geometry overload
    let geoA = LineCurve(pt 0. 0. 0., pt 10. 0. 0.) :> GeometryBase
    let geoB = LineCurve(pt 0. 5. 0., pt 10. 5. 0.) :> GeometryBase
    let g, _, dg = rs.PointClosestObject(pt 3. 4. 0., [ geoA; geoB ])
    ok "geometry overload returns the input geometry" (obj.ReferenceEquals(g, geoB))
    near "geometry overload distance" 1. dg
    throws "empty list" (fun () -> rs.PointClosestObject(pt 0. 0. 0., ([]: Guid list)))
    throws "unknown guid" (fun () -> rs.PointClosestObject(pt 0. 0. 0., [ Guid.NewGuid() ]))
    )

test "ProjectPointToMesh" (fun () ->
    let m = addMeshBox 10. 10. 10.
    let down = vec 0. 0. -1.
    let r = rs.ProjectPointToMesh([ pt 2. 3. 20. ], [ m ], down)
    ok "a closed box is hit on top and bottom" (r.Length >= 1)
    nearPt "hits the top of the box" (pt 2. 3. 10.) (r |> Array.maxBy (fun p -> p.Z))
    let r2 = rs.ProjectPointToMesh([ pt 50. 50. 20. ], [ m ], down)
    eq "point beside the mesh has no projection" 0 r2.Length
    // geometry overload
    let mesh = Mesh.CreateFromBox(Box(Plane.WorldXY, Interval(0., 10.), Interval(0., 10.), Interval(0., 10.)), 1, 1, 1)
    let r3 = rs.ProjectPointToMesh([ pt 2. 3. 20. ], [ mesh ], down)
    nearPt "geometry overload" (pt 2. 3. 10.) (r3 |> Array.maxBy (fun p -> p.Z))
    throws "not a mesh" (fun () -> rs.ProjectPointToMesh([ pt 5. 5. 20. ], [ rs.AddPoint(1., 1., 1.) ], down))
    )

test "ProjectPointToSurface" (fun () ->
    let s = addPlaneSurface 10. 10.
    let r = rs.ProjectPointToSurface([ pt 5. 5. 20. ], [ s ], vec 0. 0. -1.)
    eq "one projected point" 1 r.Length
    nearPt "hits the surface" (pt 5. 5. 0.) r.[0]
    eq "point outside the surface has no projection" 0 (rs.ProjectPointToSurface([ pt 50. 50. 20. ], [ s ], vec 0. 0. -1.)).Length
    let brep = (Brep.CreateFromBox(BoundingBox(pt 0. 0. 0., pt 10. 10. 10.)))
    let r2 = rs.ProjectPointToSurface([ pt 5. 5. 20. ], [ brep ], vec 0. 0. -1.)
    ok "geometry overload hits the top of the box" (r2.Length >= 1 && abs ((r2 |> Array.maxBy (fun p -> p.Z)).Z - 10.) < 1e-6)
    throws "not a surface" (fun () -> rs.ProjectPointToSurface([ pt 5. 5. 20. ], [ rs.AddPoint(1., 1., 1.) ], vec 0. 0. -1.))
    )

test "PullPoints" (fun () ->
    let s = addPlaneSurface 10. 10.
    let r = rs.PullPoints(s, [ pt 5. 5. 7.; pt 2. 3. -4. ])
    eq "two pulled points" 2 r.Length
    nearPt "first pulled onto the surface" (pt 5. 5. 0.) r.[0]
    nearPt "second pulled onto the surface" (pt 2. 3. 0.) r.[1]
    // mesh
    let m = addMeshBox 10. 10. 10.
    let rm = rs.PullPoints(m, [ pt 5. 5. 15. ])
    nearPt "pulled onto the mesh" (pt 5. 5. 10.) rm.[0]
    // geometry overload
    let geo = (rs.CoerceBrep s) :> GeometryBase
    nearPt "geometry overload" (pt 1. 1. 0.) (rs.PullPoints(geo, [ pt 1. 1. 3. ])).[0]
    // a closed box has many faces
    throwsRs "polysurface with many faces" (fun () -> rs.PullPoints(addBox 2. 2. 2., [ pt 1. 1. 5. ]))
    throws "unsupported object type" (fun () -> rs.PullPoints(rs.AddPoint(1., 1., 1.), [ pt 1. 1. 5. ]))
    )

printfn "%s" (finish ())
