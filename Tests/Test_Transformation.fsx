// Tests for Src/Scripting_Transformation.fs
#load "Harness.fsx"
open System
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Transformation"

let vec x y z = Vector3d(x, y, z)
let rotZ90 = Transform.Rotation(Math.PI / 2., Vector3d.ZAxis, Point3d.Origin)

test "IsXformIdentity" (fun () ->
    ok "identity" (rs.IsXformIdentity Transform.Identity)
    ok "translation is not identity" (not (rs.IsXformIdentity (Transform.Translation(1., 0., 0.))))
    ok "zero is not identity" (not (rs.IsXformIdentity (Transform())))
    )

test "IsXformSimilarity" (fun () ->
    ok "identity is a similarity" (rs.IsXformSimilarity Transform.Identity)
    ok "rotation is a similarity" (rs.IsXformSimilarity rotZ90)
    ok "uniform scale is a similarity" (rs.IsXformSimilarity (Transform.Scale(Plane.WorldXY, 2., 2., 2.)))
    ok "mirror is a similarity, an orientation reversing one" (rs.IsXformSimilarity (Transform.Mirror(Point3d.Origin, Vector3d.XAxis)))
    ok "non-uniform scale is not a similarity" (not (rs.IsXformSimilarity (Transform.Scale(Plane.WorldXY, 1., 2., 3.))))
    )

test "IsXformZero" (fun () ->
    ok "zero transform" (rs.IsXformZero (rs.XformZero()))
    ok "identity is not zero" (not (rs.IsXformZero Transform.Identity))
    )

test "XformZero" (fun () ->
    let z = rs.XformZero()
    ok "all entries zero" (z.M00 = 0. && z.M11 = 0. && z.M22 = 0. && z.M33 = 0.)
    )

test "XformIdentity" (fun () ->
    let i = rs.XformIdentity()
    ok "is identity" (i = Transform.Identity)
    nearPt "does not move points" (pt 1. 2. 3.) (i * pt 1. 2. 3.)
    )

test "XformDiagonal" (fun () ->
    let d = rs.XformDiagonal 2.
    nearPt "scales x y z by the diagonal value" (pt 2. 4. 6.) (d * pt 1. 2. 3.)
    near "bottom right entry is 1" 1. d.M33
    )

test "XformDeterminant" (fun () ->
    near "identity" 1. (rs.XformDeterminant Transform.Identity)
    near "uniform scale 2 in 3D" 8. (rs.XformDeterminant (Transform.Scale(Plane.WorldXY, 2., 2., 2.)))
    near "rotation" 1. (rs.XformDeterminant rotZ90)
    near "mirror" -1. (rs.XformDeterminant (Transform.Mirror(Point3d.Origin, Vector3d.XAxis)))
    near "zero is singular" 0. (rs.XformDeterminant (rs.XformZero()))
    )

test "XformInverse" (fun () ->
    let t = Transform.Translation(1., 2., 3.)
    let inv = rs.XformInverse t
    nearPt "inverse of a translation" (pt 0. 0. 0.) (inv * (t * pt 0. 0. 0.))
    ok "inverse of rotation times rotation is identity" (let p = (rs.XformInverse rotZ90) * (rotZ90 * pt 1. 2. 3.) in p.DistanceTo (pt 1. 2. 3.) < 1e-9)
    throwsRs "singular matrix" (fun () -> rs.XformInverse (rs.XformZero()))
    )

test "XformMultiply" (fun () ->
    // result = xForm1 * xForm2: xForm2 is applied first
    let m = rs.XformMultiply(Transform.Translation(1., 0., 0.), Transform.Scale(Plane.WorldXY, 2., 2., 2.))
    nearPt "scale first, then translate" (pt 3. 0. 0.) (m * pt 1. 0. 0.)
    let m2 = rs.XformMultiply(Transform.Scale(Plane.WorldXY, 2., 2., 2.), Transform.Translation(1., 0., 0.))
    nearPt "translate first, then scale" (pt 4. 0. 0.) (m2 * pt 1. 0. 0.)
    ok "multiplying by identity changes nothing" (rs.XformMultiply(m, Transform.Identity) = m)
    )

test "XformCompare" (fun () ->
    eq "same" 0 (rs.XformCompare(Transform.Identity, Transform.Identity))
    let a = Transform.Translation(1., 0., 0.)
    let b = Transform.Translation(2., 0., 0.)
    let ab = rs.XformCompare(a, b)
    let ba = rs.XformCompare(b, a)
    ok "different matrices do not compare equal" (ab <> 0)
    eq "comparison is antisymmetric" (-ab) ba
    )

test "XformTranslation" (fun () ->
    let t = rs.XformTranslation(vec 1. 2. 3.)
    nearPt "moves a point" (pt 2. 3. 4.) (t * pt 1. 1. 1.)
    nearVec "does not change a vector" (vec 1. 1. 1.) (t * vec 1. 1. 1.)
    )

test "XformMirror" (fun () ->
    let m = rs.XformMirror(pt 0. 0. 0., vec 1. 0. 0.)
    nearPt "mirrored at the YZ plane" (pt -1. 2. 3.) (m * pt 1. 2. 3.)
    let m2 = rs.XformMirror(pt 5. 0. 0., vec 1. 0. 0.)
    nearPt "mirror plane through x=5" (pt 9. 2. 3.) (m2 * pt 1. 2. 3.)
    )

test "XformPlanarProjection" (fun () ->
    let p = rs.XformPlanarProjection Plane.WorldXY
    nearPt "projected to the XY plane" (pt 1. 2. 0.) (p * pt 1. 2. 3.)
    let p2 = rs.XformPlanarProjection (Plane(pt 0. 0. 5., Vector3d.ZAxis))
    nearPt "projected to a plane at z=5" (pt 1. 2. 5.) (p2 * pt 1. 2. 3.)
    )

test "XformRotation1" (fun () ->
    let target = Plane(pt 5. 0. 0., Vector3d.XAxis, vec 0. 0. 1.)
    let x = rs.XformRotation1(Plane.WorldXY, target)
    nearPt "origin goes to the new origin" (pt 5. 0. 0.) (x * Point3d.Origin)
    nearPt "x axis tip goes to the new x axis tip" (pt 6. 0. 0.) (x * pt 1. 0. 0.)
    nearPt "y axis tip goes to the new y axis tip" (pt 5. 0. 1.) (x * pt 0. 1. 0.)
    )

test "XformRotation2" (fun () ->
    nearPt "90 degrees about z" (pt 0. 1. 0.) (rs.XformRotation2(90., vec 0. 0. 1., pt 0. 0. 0.) * pt 1. 0. 0.)
    nearPt "90 degrees about z around another center" (pt 1. 1. 0.) (rs.XformRotation2(90., vec 0. 0. 1., pt 1. 0. 0.) * pt 2. 0. 0.)
    nearPt "the center stays" (pt 1. 0. 0.) (rs.XformRotation2(33., vec 0. 0. 1., pt 1. 0. 0.) * pt 1. 0. 0.)
    nearPt "-90 degrees is the other way" (pt 0. -1. 0.) (rs.XformRotation2(-90., vec 0. 0. 1., pt 0. 0. 0.) * pt 1. 0. 0.)
    )

test "XformRotation3" (fun () ->
    let x = rs.XformRotation3(vec 1. 0. 0., vec 0. 1. 0., pt 0. 0. 0.)
    nearPt "start direction is rotated to the end direction" (pt 0. 1. 0.) (x * pt 1. 0. 0.)
    nearPt "z is not changed" (pt 0. 0. 1.) (x * pt 0. 0. 1.)
    )

test "XformRotation4" (fun () ->
    let x = rs.XformRotation4(vec 1. 0. 0., vec 0. 1. 0., vec 0. 0. 1., vec 0. 1. 0., vec -1. 0. 0., vec 0. 0. 1.)
    nearPt "x frame axis goes to the new x axis" (pt 0. 1. 0.) (x * pt 1. 0. 0.)
    nearPt "y frame axis goes to the new y axis" (pt -1. 0. 0.) (x * pt 0. 1. 0.)
    )

test "XformChangeBasis" (fun () ->
    let final = Plane(pt 10. 0. 0., Vector3d.XAxis, Vector3d.YAxis)
    let x = rs.XformChangeBasis(Plane.WorldXY, final)
    nearPt "the origin of the final plane is 0,0,0 in its own coordinates" (pt 0. 0. 0.) (x * pt 10. 0. 0.)
    nearPt "point in final plane coordinates" (pt 1. 2. 3.) (x * pt 11. 2. 3.)
    )

test "XformChangeBasis2" (fun () ->
    let x = rs.XformChangeBasis2(vec 1. 0. 0., vec 0. 1. 0., vec 0. 0. 1., vec 0. 1. 0., vec -1. 0. 0., vec 0. 0. 1.)
    nearPt "world x seen from a basis rotated by 90 degrees" (pt 0. -1. 0.) (x * pt 1. 0. 0.)
    throwsRs "degenerate basis" (fun () -> rs.XformChangeBasis2(vec 1. 0. 0., vec 0. 1. 0., vec 0. 0. 1., vec 0. 0. 0., vec 0. 0. 0., vec 0. 0. 0.))
    )

test "XformCPlaneToWorld" (fun () ->
    let plane = Plane(pt 1. 1. 1., Vector3d.XAxis, Vector3d.YAxis)
    nearPt "offset plane" (pt 3. 4. 6.) (rs.XformCPlaneToWorld(pt 2. 3. 5., plane))
    let rot = Plane(Point3d.Origin, Vector3d.YAxis, -Vector3d.XAxis)
    nearPt "rotated plane" (pt 0. 1. 0.) (rs.XformCPlaneToWorld(pt 1. 0. 0., rot))
    )

test "XformWorldToCPlane" (fun () ->
    let plane = Plane(pt 1. 1. 1., Vector3d.XAxis, Vector3d.YAxis)
    nearPt "offset plane" (pt 2. 3. 5.) (rs.XformWorldToCPlane(pt 3. 4. 6., plane))
    let rot = Plane(Point3d.Origin, Vector3d.YAxis, -Vector3d.XAxis)
    nearPt "rotated plane" (pt 1. 0. 0.) (rs.XformWorldToCPlane(pt 0. 1. 0., rot))
    let p = pt 3.3 -4.4 5.5
    nearPt "round trip" p (rs.XformCPlaneToWorld(rs.XformWorldToCPlane(p, rot), rot))
    )

test "XformScale" (fun () ->
    nearPt "uniform scale about the origin" (pt 2. 4. 6.) (rs.XformScale(2.0) * pt 1. 2. 3.)
    nearPt "uniform scale about a point" (pt 3. 3. 3.) (rs.XformScale(2.0, pt 1. 1. 1.) * pt 2. 2. 2.)
    nearPt "non-uniform scale about a point" (pt 3. 4. 4.) (rs.XformScale(2.0, 3.0, 4.0, pt 1. 1. 0.) * pt 2. 2. 1.)
    nearPt "non-uniform scale along plane axes" (pt 2. 6. 12.) (rs.XformScale(2.0, 3.0, 4.0, Plane.WorldXY) * pt 1. 2. 3.)
    let rotated = Plane(Point3d.Origin, Vector3d.YAxis, -Vector3d.XAxis)
    nearPt "the plane's x axis is world y" (pt 0. 2. 0.) (rs.XformScale(2.0, 1.0, 1.0, rotated) * pt 0. 1. 0.)
    nearPt "uniform scale about a plane origin" (pt 3. 3. 3.) (rs.XformScale(2.0, Plane(pt 1. 1. 1., Vector3d.ZAxis)) * pt 2. 2. 2.)
    )

test "XformShear" (fun () ->
    let x = rs.XformShear(Plane.WorldXY, vec 1. 0. 0., vec 0.5 1. 0., vec 0. 0. 1.)
    nearPt "x axis stays" (pt 1. 0. 0.) (x * pt 1. 0. 0.)
    nearPt "y axis tip is sheared" (pt 0.5 1. 0.) (x * pt 0. 1. 0.)
    nearPt "z axis stays" (pt 0. 0. 1.) (x * pt 0. 0. 1.)
    )

test "XformWorldToScreen" (fun () ->
    let p2 = rs.XformWorldToScreen(pt 0. 0. 0., "Top")
    let p3 = rs.XformWorldToScreen(pt 10. 0. 0., "Top")
    let p4 = rs.XformWorldToScreen(pt 0. 10. 0., "Top")
    ok "moving in world x moves in screen x on the top view" (p3.X > p2.X && abs (p3.Y - p2.Y) < 1e-6)
    ok "moving in world y moves up the screen (smaller y) on the top view" (p4.Y < p2.Y && abs (p4.X - p2.X) < 1e-6)
    let s = rs.XformWorldToScreen(pt 0. 0. 0., "Top", true)
    ok "screen coordinates are finite" (not (Double.IsNaN s.X || Double.IsNaN s.Y))
    ok "the active view is used when no view is given" (let a = rs.XformWorldToScreen(pt 1. 1. 1.) in not (Double.IsNaN a.X))
    throws "unknown view" (fun () -> rs.XformWorldToScreen(pt 0. 0. 0., "no such view"))
    )

test "XformScreenToWorld" (fun () ->
    let w = pt 12. 7. 0.
    let s = rs.XformWorldToScreen(w, "Top")
    let back = rs.XformScreenToWorld(Point3d(s.X, s.Y, 0.), "Top")
    // the screen z is 0 (near clipping plane), so only x and y of the Top view round trip
    near "round trip through client coordinates, x" w.X back.X
    near "round trip through client coordinates, y" w.Y back.Y
    let sc = rs.XformWorldToScreen(w, "Top", true)
    let back2 = rs.XformScreenToWorld(Point3d(sc.X, sc.Y, 0.), "Top", true)
    near "round trip through screen coordinates, x" w.X back2.X
    near "round trip through screen coordinates, y" w.Y back2.Y
    throws "unknown view" (fun () -> rs.XformScreenToWorld(pt 0. 0. 0., "no such view"))
    )

printfn "%s" (finish ())
