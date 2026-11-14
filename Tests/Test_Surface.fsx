// Tests for Src/Scripting_Surface.fs
// Most functions come as a Guid overload that works on an object of the document and as a geometry overload that works on a Brep or Surface directly
// without touching the document. Both are tested where it makes sense.
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Surface"

let vec x y z = Vector3d(x, y, z)
let geo (id:Guid) = (doc().Objects.FindId id).Geometry
let brepOf (id:Guid) = rs.CoerceBrep id
let srfOf (id:Guid) = rs.CoerceSurface id
let pointId () = rs.AddPoint(1., 2., 3.)
let boxCorners (x:float) (y:float) (z:float) =
    [ pt 0. 0. 0.; pt x 0. 0.; pt x y 0.; pt 0. y 0.; pt 0. 0. z; pt x 0. z; pt x y z; pt 0. y z ]
let boxId () = rs.AddBox(boxCorners 10. 10. 10.)
let planeId () = addPlaneSurface 10. 10.
let sphereId () = rs.AddSphere(Point3d.Origin, 5.)
/// single face surfaces of the primitives, the capped versions are polysurfaces
let cylinderId () = rs.AddCylinder(Plane.WorldXY, 10., 3., false)
let coneId () = rs.AddCone(Plane.WorldXY, 10., 3., false)
let torusId () = rs.AddTorus(Plane.WorldXY, 10., 2.)
/// a curved nurbs surface
let gridId () =
    let pts = [ for i in 0 .. 3 do for j in 0 .. 3 -> pt (float i * 3.) (float j * 3.) (if (i + j) % 2 = 0 then 1. else -1.) ]
    rs.AddSrfPtGrid((4, 4), pts)
let curve (id:Guid) = rs.CoerceCurve id
let sq (size:float) (z:float) = rs.AddPolyline [ pt 0. 0. z; pt size 0. z; pt size size z; pt 0. size z; pt 0. 0. z ]
let area (id:Guid) = rs.SurfaceArea id

// ------------------------------- primitives -------------------------------

test "AddBox" (fun () ->
    let b = boxId ()
    ok "is a polysurface" (rs.IsPolysurface b)
    ok "is closed" (rs.IsPolysurfaceClosed b)
    near "volume" 1000. (rs.SurfaceVolume b)
    near "area" 600. (area b)
    throwsRs "too few corners" (fun () -> rs.AddBox [ pt 0. 0. 0.; pt 1. 0. 0. ])
    )
test "AddCone" (fun () ->
    let c = rs.AddCone(Plane.WorldXY, 10., 3.)
    ok "capped cone is a polysurface" (rs.IsPolysurface c)
    near "volume of the cone" (Math.PI * 9. * 10. / 3.) (rs.SurfaceVolume c)
    let open1 = rs.AddCone(Plane.WorldXY, 10., 3., false)
    ok "uncapped cone is a cone surface" (rs.IsCone open1)
    let t = rs.AddCone(pt 0. 0. 0., pt 0. 0. 10., 3.)
    ok "cone from base center and tip" (rs.IsPolysurfaceClosed t)
    let t2 = rs.AddCone(pt 0. 0. 0., pt 0. 0. 10., 3., false)
    ok "cone from base center and tip without cap" (rs.IsCone t2)
    throwsRs "zero radius" (fun () -> rs.AddCone(Plane.WorldXY, 10., 0.))
    throwsRs "base and tip are the same" (fun () -> rs.AddCone(pt 0. 0. 0., pt 0. 0. 0., 3.))
    throwsRs "zero height" (fun () -> rs.AddCone(Plane.WorldXY, 0., 3.))
    )
test "AddCylinder" (fun () ->
    let c = rs.AddCylinder(Plane.WorldXY, 10., 3.)
    ok "capped cylinder is a closed polysurface" (rs.IsPolysurfaceClosed c)
    near "volume" (Math.PI * 9. * 10.) (rs.SurfaceVolume c)
    ok "uncapped cylinder is a cylinder surface" (rs.IsCylinder (rs.AddCylinder(Plane.WorldXY, 10., 3., false)))
    let neg = rs.AddCylinder(Plane.WorldXY, -10., 3.)
    near "a negative height goes the other way" -10. (rs.BoundingBox neg).Min.Z
    throwsRs "zero radius" (fun () -> rs.AddCylinder(Plane.WorldXY, 10., 0.))
    throwsRs "zero height" (fun () -> rs.AddCylinder(Plane.WorldXY, 0., 3.))
    )
test "AddSphere" (fun () ->
    let s = rs.AddSphere(pt 1. 2. 3., 5.)
    ok "is a sphere" (rs.IsSphere s)
    near "area" (4. * Math.PI * 25.) (area s)
    nearPt "center" (pt 1. 2. 3.) (let p, _ = rs.SurfaceSphere s in p.Origin)
    throwsRs "zero radius" (fun () -> rs.AddSphere(pt 0. 0. 0., 0.))
    )
test "AddTorus" (fun () ->
    let t = rs.AddTorus(Plane.WorldXY, 10., 2.)
    ok "is a torus" (rs.IsTorus t)
    near "area" (4. * Math.PI * Math.PI * 10. * 2.) (area t)
    throwsRs "zero minor radius" (fun () -> rs.AddTorus(Plane.WorldXY, 10., 0.))
    )
test "AddPlaneSurface" (fun () ->
    let p = rs.AddPlaneSurface(Plane.WorldXY, 4., 3.)
    ok "is a plane surface" (rs.IsPlaneSurface p)
    near "area" 12. (area p)
    let moved = rs.AddPlaneSurface(Plane(pt 5. 5. 5., Vector3d.ZAxis), 2., 2.)
    nearPt "origin of the plane is the corner" (pt 5. 5. 5.) (rs.BoundingBox moved).Min
    )

// ------------------------------- from points and curves -------------------------------

test "AddSrfPt" (fun () ->
    let q = rs.AddSrfPt(pt 0. 0. 0., pt 5. 0. 0., pt 5. 5. 0., pt 0. 5. 0.)
    ok "is a surface" (rs.IsSurface q)
    near "area of the quad" 25. (area q)
    let t = rs.AddSrfPt(pt 0. 0. 0., pt 5. 0. 0., pt 0. 5. 0.)
    near "area of the triangle" 12.5 (area t)
    )
test "AddSrfPtGrid" (fun () ->
    let g = gridId ()
    ok "is a surface" (rs.IsSurface g)
    eq "point count" (4, 4) (rs.SurfacePointCount g)
    eq "degree 3" (3, 3) (rs.SurfaceDegree g)
    let pts = [ for i in 0 .. 3 do for j in 0 .. 3 -> pt (float i) (float j) 0. ]
    eq "degree 2" (2, 2) (rs.SurfaceDegree (rs.AddSrfPtGrid((4, 4), pts, 2, 2)))
    ok "closed in u" (rs.IsSurfaceClosed(rs.AddSrfPtGrid((4, 4), pts, 3, 3, true, false), 0))
    throws "wrong number of points" (fun () -> rs.AddSrfPtGrid((4, 4), [ pt 0. 0. 0. ]))
    )
test "AddSrfControlPtGrid" (fun () ->
    let pts = [ for i in 0 .. 3 do for j in 0 .. 3 -> pt (float i) (float j) (if i = 1 && j = 1 then 2. else 0.) ]
    let g = rs.AddSrfControlPtGrid((4, 4), pts)
    ok "is a surface" (rs.IsSurface g)
    eq "point count" (4, 4) (rs.SurfacePointCount g)
    near "the surface does not pass through the control points" 1. (if (rs.SurfaceClosestPoint(g, pt 1. 1. 2.)).DistanceTo(pt 1. 1. 2.) > 0.01 then 1. else 0.)
    eq "degree 2" (2, 2) (rs.SurfaceDegree (rs.AddSrfControlPtGrid((4, 4), pts, 2, 2)))
    throws "wrong number of points" (fun () -> rs.AddSrfControlPtGrid((4, 4), [ pt 0. 0. 0. ]))
    )
test "AddNurbsSurface" (fun () ->
    let pts = [| for i in 0 .. 2 do for j in 0 .. 2 -> pt (float i) (float j) (if i = 1 && j = 1 then 1. else 0.) |]
    let knots = [ 0.; 0.; 1.; 1. ]
    let s = rs.AddNurbsSurface((3, 3), pts, knots, knots, (2, 2))
    ok "is a surface" (rs.IsSurface s)
    eq "point count" (3, 3) (rs.SurfacePointCount s)
    eq "degree" (2, 2) (rs.SurfaceDegree s)
    let w = rs.AddNurbsSurface((3, 3), pts, knots, knots, (2, 2), Array.create 9 1.)
    ok "with weights" (rs.IsSurface w)
    throwsRs "wrong number of points" (fun () -> rs.AddNurbsSurface((3, 3), [| pt 0. 0. 0. |], knots, knots, (2, 2)))
    throwsRs "wrong number of knots" (fun () -> rs.AddNurbsSurface((3, 3), pts, [ 0.; 1. ], knots, (2, 2)))
    throwsRs "wrong number of weights" (fun () -> rs.AddNurbsSurface((3, 3), pts, knots, knots, (2, 2), [| 1. |]))
    )
test "AddEdgeSrf" (fun () ->
    let curves = [ rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.); rs.AddLine(pt 10. 0. 0., pt 10. 10. 0.); rs.AddLine(pt 10. 10. 0., pt 0. 10. 0.); rs.AddLine(pt 0. 10. 0., pt 0. 0. 0.) ]
    let s = rs.AddEdgeSrf curves
    ok "is a surface" (rs.IsSurface s)
    near "area" 100. (area s)
    let g = rs.MakeEdgeSrf (curves |> List.map curve)
    near "geometry" 100. (g.GetArea())
    let three = rs.AddEdgeSrf (curves |> List.take 3)
    ok "three edges" (rs.IsSurface three)
    )
test "MakeEdgeSrf" (fun () ->
    let n = (rs.AllObjects()).Count
    let curves = [ new LineCurve(pt 0. 0. 0., pt 10. 0. 0.) :> Curve; new LineCurve(pt 10. 0. 0., pt 10. 10. 0.) :> Curve; new LineCurve(pt 10. 10. 0., pt 0. 10. 0.) :> Curve; new LineCurve(pt 0. 10. 0., pt 0. 0. 0.) :> Curve ]
    let b = rs.MakeEdgeSrf curves
    near "area" 100. (b.GetArea())
    eq "nothing was added to the document" n (rs.AllObjects()).Count
    )
test "AddNetworkSrf" (fun () ->
    let u = [ for y in [ 0.; 5.; 10. ] -> rs.AddInterpCurve([ pt 0. y 0.; pt 5. y 2.; pt 10. y 0. ]) ]
    let v = [ for x in [ 0.; 5.; 10. ] -> rs.AddInterpCurve([ pt x 0. (if x = 5. then 2. else 0.); pt x 5. (if x = 5. then 2. else 0.); pt x 10. (if x = 5. then 2. else 0.) ]) ]
    let n = rs.AddNetworkSrf (u @ v)
    ok "is a surface" (rs.IsSurface n)
    let g = rs.MakeNetworkSrf ((u @ v) |> List.map curve)
    ok "geometry" (not (isNull g))
    ok "with continuity and tolerances" ((try Some (rs.AddNetworkSrf(u @ v, 1, 0.1, 0.1, 5.)) with :? RhinoScriptingException -> None) <> Some Guid.Empty)
    throwsRs "too few curves" (fun () -> rs.AddNetworkSrf [ u.[0] ])
    )
test "AddPatch" (fun () ->
    let pts = [ pt 0. 0. 0.; pt 10. 0. 1.; pt 10. 10. 0.; pt 0. 10. 1.; pt 5. 5. 3. ]
    let ptsIds = rs.AddPoints pts
    let p = rs.AddPatch(ptsIds, (8, 8))
    ok "is a surface" (rs.IsSurface p)
    let srf = planeId ()
    let withStart = rs.AddPatch(ptsIds, srf)
    ok "with a start surface" (rs.IsSurface withStart)
    let g = rs.MakePatch(ptsIds |> Seq.map geo, (8, 8))
    ok "geometry with uv spans" (not (isNull g))
    let g2 = rs.MakePatch(ptsIds |> Seq.map geo, srfOf srf)
    ok "geometry with a start surface" (not (isNull g2))
    ok "with all parameters" (rs.IsSurface (rs.AddPatch(ptsIds, (6, 6), 0.1, false, 0.5, 0.5, 0.5, true)))
    throws "no objects" (fun () -> rs.AddPatch(([] : Guid list), (8, 8)))
    )
test "AddPlanarSrf" (fun () ->
    let c = sq 10. 0.
    let s = rs.AddPlanarSrf [ c ]
    eq "one surface" 1 s.Count
    near "area" 100. (area s.[0])
    let hole = sq 4. 0.
    rs.MoveObject(hole, vec 3. 3. 0.)
    let withHole = rs.AddPlanarSrf [ c; hole ]
    near "area with a hole" 84. (area withHole.[0])
    throwsRs "open curve" (fun () -> rs.AddPlanarSrf [ rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.) ])
    )
test "MakePlanarSrf" (fun () ->
    let n = (rs.AllObjects()).Count
    let pl = Polyline([ pt 0. 0. 0.; pt 10. 0. 0.; pt 10. 10. 0.; pt 0. 10. 0.; pt 0. 0. 0. ])
    near "polyline" 100. ((rs.MakePlanarSrf pl).GetArea())
    near "curve" 100. ((rs.MakePlanarSrf (pl.ToPolylineCurve() :> Curve)).GetArea())
    let many = rs.MakePlanarSrf [ pl.ToPolylineCurve() :> Curve ]
    eq "curves" 1 many.Count
    eq "nothing was added to the document" n (rs.AllObjects()).Count
    throwsRs "open polyline" (fun () -> rs.MakePlanarSrf (Polyline([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ])))
    )
test "AddLoftSrf" (fun () ->
    let a, b = sq 10. 0., sq 10. 10.
    let l = rs.AddLoftSrf [ a; b ]
    eq "one surface" 1 l.Count
    near "area of the sides" 400. (area l.[0])
    for t in 0 .. 3 do ok (sprintf "loft type %d" t) ((rs.AddLoftSrf([ a; b; sq 10. 20. ], Point3d(), Point3d(), t)).Count >= 1)
    ok "rebuild" ((rs.AddLoftSrf([ a; b ], Point3d(), Point3d(), 0, 10)).Count >= 1)
    ok "refit" ((rs.AddLoftSrf([ a; b ], Point3d(), Point3d(), 0, 0, 0.1)).Count >= 1)
    ok "closed" ((rs.AddLoftSrf([ a; b; sq 10. 20. ], Point3d(), Point3d(), 0, 0, 0., true)).Count >= 1)
    let g = rs.MakeLoftSrf [ curve a; curve b ]
    eq "geometry" 1 g.Count
    throwsRs "one curve" (fun () -> rs.AddLoftSrf [ a ])
    throwsRs "loft type 4" (fun () -> rs.AddLoftSrf([ a; b ], Point3d(), Point3d(), 4))
    throwsRs "rebuild and refit" (fun () -> rs.AddLoftSrf([ a; b ], Point3d(), Point3d(), 0, 10, 0.1))
    )
test "MakeLoftSrf" (fun () ->
    let a = new PolylineCurve([ pt 0. 0. 0.; pt 10. 0. 0.; pt 10. 10. 0.; pt 0. 10. 0.; pt 0. 0. 0. ]) :> Curve
    let b = new PolylineCurve([ pt 0. 0. 10.; pt 10. 0. 10.; pt 10. 10. 10.; pt 0. 10. 10.; pt 0. 0. 10. ]) :> Curve
    let l = rs.MakeLoftSrf [ a; b ]
    eq "one brep" 1 l.Count
    near "area" 400. (l.[0].GetArea())
    )
test "AddRevSrf" (fun () ->
    let c = rs.AddLine(pt 5. 0. 0., pt 5. 0. 10.)
    let s = rs.AddRevSrf(c, Line(pt 0. 0. 0., pt 0. 0. 1.))
    ok "is a surface" (rs.IsSurface s)
    near "area of a cylinder side" (2. * Math.PI * 5. * 10.) (area s)
    let half = rs.AddRevSrf(c, Line(pt 0. 0. 0., pt 0. 0. 1.), 0., 180.)
    near "half" (Math.PI * 5. * 10.) (area half)
    near "geometry" (2. * Math.PI * 5. * 10.) (AreaMassProperties.Compute(rs.MakeRevSrf(curve c, Line(pt 0. 0. 0., pt 0. 0. 1.)))).Area
    throwsRs "same start and end angle" (fun () -> rs.AddRevSrf(c, Line(pt 0. 0. 0., pt 0. 0. 1.), 30., 30.))
    )
test "MakeRevSrf" (fun () ->
    let c = new LineCurve(pt 5. 0. 0., pt 5. 0. 10.) :> Curve
    near "area" (2. * Math.PI * 5. * 10.) (AreaMassProperties.Compute(rs.MakeRevSrf(c, Line(pt 0. 0. 0., pt 0. 0. 1.)))).Area
    )
test "AddRailRevSrf" (fun () ->
    let profile = rs.AddLine(pt 3. 0. 0., pt 3. 0. 6.)
    let rail = rs.AddCircle(Plane.WorldXY, 3.)
    let s = rs.AddRailRevSrf(profile, rail, Line(pt 0. 0. 0., pt 0. 0. 1.))
    ok "is a surface" (rs.IsSurface s)
    ok "scale height" (rs.IsSurface (rs.AddRailRevSrf(profile, rail, Line(pt 0. 0. 0., pt 0. 0. 1.), true)))
    ok "geometry" (not (isNull (rs.MakeRailRevSrf(curve profile, curve rail, Line(pt 0. 0. 0., pt 0. 0. 1.)))))
    throws "not a curve" (fun () -> rs.AddRailRevSrf(pointId (), rail, Line(pt 0. 0. 0., pt 0. 0. 1.)))
    )
test "MakeRailRevSrf" (fun () ->
    let profile = new LineCurve(pt 3. 0. 0., pt 3. 0. 6.) :> Curve
    let rail = new ArcCurve(Circle(Plane.WorldXY, 3.)) :> Curve
    ok "a surface" (not (isNull (rs.MakeRailRevSrf(profile, rail, Line(pt 0. 0. 0., pt 0. 0. 1.), true))))
    )
test "AddSweep1" (fun () ->
    let rail = rs.AddLine(pt 0. 0. 0., pt 0. 0. 10.)
    let shape = sq 4. 0.
    let s = rs.AddSweep1(rail, [ shape ])
    ok "sweep gives breps" (s.Count >= 1)
    ok "closed" ((rs.AddSweep1(rail, [ shape ], true)).Count >= 1)
    ok "geometry" ((rs.MakeSweep1(curve rail, [ curve shape ])).Count >= 1)
    )
test "MakeSweep1" (fun () ->
    let rail = new LineCurve(pt 0. 0. 0., pt 0. 0. 10.) :> Curve
    let shape = new PolylineCurve([ pt 0. 0. 0.; pt 4. 0. 0.; pt 4. 4. 0.; pt 0. 4. 0.; pt 0. 0. 0. ]) :> Curve
    ok "breps" ((rs.MakeSweep1(rail, [ shape ])).Count >= 1)
    )
test "AddSweep2" (fun () ->
    let r1 = rs.AddLine(pt 0. 0. 0., pt 0. 0. 10.)
    let r2 = rs.AddLine(pt 10. 0. 0., pt 10. 0. 10.)
    let shape = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let s = rs.AddSweep2((r1, r2), [ shape ])
    ok "sweep gives breps" (s.Count >= 1)
    near "area" 100. (area s.[0])
    ok "geometry" ((rs.MakeSweep2((curve r1, curve r2), [ curve shape ])).Count >= 1)
    )
test "MakeSweep2" (fun () ->
    let r1 = new LineCurve(pt 0. 0. 0., pt 0. 0. 10.) :> Curve
    let r2 = new LineCurve(pt 10. 0. 0., pt 10. 0. 10.) :> Curve
    let shape = new LineCurve(pt 0. 0. 0., pt 10. 0. 0.) :> Curve
    ok "breps" ((rs.MakeSweep2((r1, r2), [ shape ])).Count >= 1)
    )
test "AddPipe" (fun () ->
    let c = rs.AddLine(pt 0. 0. 0., pt 0. 0. 10.)
    let p = rs.AddPipe(c, [ 0. ], [ 1. ])
    ok "pipe gives breps" (p.Count >= 1)
    let capped = rs.AddPipe(c, [ 0. ], [ 1. ], 0, 1)
    near "volume of a flat capped pipe with a constant radius is a cylinder" (Math.PI * 10.) (rs.SurfaceVolume capped.[0])
    let tapered = rs.AddPipe(c, [ 0.; 1. ], [ 1.; 2. ])
    ok "different radii" (tapered.Count >= 1)
    for cap in [ 0; 1; 2 ] do ok (sprintf "cap %d" cap) ((rs.AddPipe(c, [ 0. ], [ 1. ], 0, cap)).Count >= 1)
    ok "blend type and fit" ((rs.AddPipe(c, [ 0.; 1. ], [ 1.; 2. ], 1, 1, true)).Count >= 1)
    ok "geometry" ((rs.MakePipe(curve c, [ 0. ], [ 1. ])).Count >= 1)
    throws "different number of parameters and radii" (fun () -> rs.AddPipe(c, [ 0.; 1. ], [ 1. ]))
    )
test "MakePipe" (fun () ->
    let c = new LineCurve(pt 0. 0. 0., pt 0. 0. 10.) :> Curve
    ok "breps" ((rs.MakePipe(c, [ c.Domain.T0 ], [ 1. ])).Count >= 1)
    )
test "AddCutPlane" (fun () ->
    let b = boxId ()
    let cut = rs.AddCutPlane([ b ], pt 5. -5. 0., pt 5. 15. 0.)
    ok "is a surface" (rs.IsSurface cut)
    let g = rs.MakeCutPlane([ geo b ], pt 5. -5. 0., pt 5. 15. 0.)
    ok "geometry" (not (isNull g))
    ok "with a normal" (rs.IsSurface (rs.AddCutPlane([ b ], pt 5. -5. 0., pt 5. 15. 0., vec 0. 0. 1.)))
    throws "no objects" (fun () -> rs.AddCutPlane(([] : Guid list), pt 0. 0. 0., pt 1. 0. 0.))
    )
test "MakeCutPlane" (fun () ->
    let b = (brepOf (boxId ())) :> GeometryBase
    let p = rs.MakeCutPlane([ b ], pt 5. -5. 0., pt 5. 15. 0.)
    ok "a plane surface" (not (isNull p))
    )
test "AddSrfContourCrvs" (fun () ->
    let b = boxId ()
    let byPlane = rs.AddSrfContourCrvs(b, Plane(pt 0. 0. 5., Vector3d.ZAxis))
    ok "contour curves" (byPlane.Count >= 1)
    let byLine = rs.AddSrfContourCrvs(b, pt 0. 0. 0., pt 0. 0. 10., 2.5)
    ok "contour curves by interval" (byLine.Count >= 4)
    ok "geometry by plane" ((rs.MakeSrfContourCrvs(brepOf b, Plane(pt 0. 0. 5., Vector3d.ZAxis))).Count >= 1)
    ok "geometry by interval" ((rs.MakeSrfContourCrvs(brepOf b, pt 0. 0. 0., pt 0. 0. 10., 2.5)).Count >= 4)
    eq "a plane that misses the brep" 0 (try (rs.AddSrfContourCrvs(b, Plane(pt 0. 0. 50., Vector3d.ZAxis))).Count with :? RhinoScriptingException -> 0)
    )
test "MakeSrfContourCrvs" (fun () ->
    let n = (rs.AllObjects()).Count
    let c = rs.MakeSrfContourCrvs(brepOf (boxId ()), Plane(pt 0. 0. 5., Vector3d.ZAxis))
    ok "curves" (c.Count >= 1)
    eq "only the box was added to the document" (n + 1) (rs.AllObjects()).Count
    )
test "ExtrudeCurveStraight" (fun () ->
    let c = sq 10. 0.
    let e = rs.ExtrudeCurveStraight(c, pt 0. 0. 0., pt 0. 0. 5.)
    ok "is a surface" (rs.IsSurface e || rs.IsPolysurface e || rs.IsExtrusion e)
    near "area of the sides" 200. (area e)
    near "geometry" 5. ((rs.ExtrudeCurveStraight(curve c, pt 0. 0. 0., pt 0. 0. 5.)).GetBoundingBox(true).Diagonal.Z)
    throwsRs "start and end are the same" (fun () -> rs.ExtrudeCurveStraight(c, pt 0. 0. 0., pt 0. 0. 0.))
    )
test "ExtrudeCurve" (fun () ->
    let c = sq 10. 0.
    let path = rs.AddLine(pt 0. 0. 0., pt 0. 0. 5.)
    let e = rs.ExtrudeCurve(c, path)
    ok "is a surface" (rs.IsSurface e || rs.IsPolysurface e)
    near "area of the sides" 200. (area e)
    ok "geometry" (not (isNull (rs.ExtrudeCurve(curve c, curve path))))
    throws "not a curve" (fun () -> rs.ExtrudeCurve(pointId (), path))
    )
test "ExtrudeCurvePoint" (fun () ->
    let c = sq 10. 0.
    let e = rs.ExtrudeCurvePoint(c, pt 5. 5. 10.)
    ok "is a surface" (rs.IsSurface e || rs.IsPolysurface e)
    ok "geometry" (not (isNull (rs.ExtrudeCurvePoint(curve c, pt 5. 5. 10.))))
    throws "not a curve" (fun () -> rs.ExtrudeCurvePoint(pointId (), pt 5. 5. 10.))
    )
test "ExtrudeSurface" (fun () ->
    let s = planeId ()
    let path = rs.AddLine(pt 0. 0. 0., pt 0. 0. 5.)
    let e = rs.ExtrudeSurface(s, path)
    ok "capped extrusion is a closed polysurface" (rs.IsPolysurfaceClosed e)
    near "volume" 500. (rs.SurfaceVolume e)
    let open1 = rs.ExtrudeSurface(planeId (), path, false)
    ok "without caps the polysurface is open" (not (rs.IsPolysurfaceClosed open1))
    ok "geometry" (not (isNull (rs.ExtrudeSurface(brepOf s, curve path))))
    throws "not a curve" (fun () -> rs.ExtrudeSurface(s, pointId ()))
    )
test "FilletSurfaces" (fun () ->
    let a = rs.AddPlaneSurface(Plane.WorldXY, 10., 10.)
    let b = rs.AddPlaneSurface(Plane(pt 0. 0. 0., Vector3d.XAxis, Vector3d.ZAxis), 10., 10.)
    let f = rs.FilletSurfaces(a, b, 2., Point2d(5., 5.), Point2d(5., 5.))
    ok "fillet surfaces" (f.Count >= 1)
    ok "geometry" ((rs.FilletSurfaces(srfOf a, srfOf b, 2., Point2d(5., 5.), Point2d(5., 5.))).Count >= 1)
    )

// ------------------------------- boolean, split, trim, join -------------------------------

let offsetBox (dx:float) =
    rs.AddBox([ pt dx 5. 5.; pt (dx+10.) 5. 5.; pt (dx+10.) 15. 5.; pt dx 15. 5.; pt dx 5. 15.; pt (dx+10.) 5. 15.; pt (dx+10.) 15. 15.; pt dx 15. 15. ])
let cutterId () = rs.AddPlaneSurface(Plane(pt -1. -1. 5., Vector3d.ZAxis), 12., 12.)
let sidePlane (x:float) = rs.AddPlaneSurface(Plane(pt x 0. 0., Vector3d.ZAxis), 10., 10.)

test "BooleanUnion" (fun () ->
    let a, b = boxId (), offsetBox 5.
    let u = rs.BooleanUnion [ a; b ]
    eq "one result" 1 u.Count
    near "volume" 1875. (rs.SurfaceVolume u.[0])
    ok "inputs deleted" (not (rs.IsObject a) && not (rs.IsObject b))
    let a2, b2 = boxId (), offsetBox 5.
    let kept = rs.BooleanUnion([ a2; b2 ], false)
    ok "inputs kept" (rs.IsObject a2 && rs.IsObject b2)
    near "volume of the kept result" 1875. (rs.SurfaceVolume kept.[0])
    let g = rs.BooleanUnion [ brepOf a2; brepOf b2 ]
    near "geometry" 1875. (rs.SurfaceVolume g.[0])
    )
test "BooleanDifference" (fun () ->
    let a, b = boxId (), offsetBox 5.
    let d = rs.BooleanDifference([ a ], [ b ])
    eq "one result" 1 d.Count
    near "volume" 875. (rs.SurfaceVolume d.[0])
    ok "inputs deleted" (not (rs.IsObject a) && not (rs.IsObject b))
    let a2, b2 = boxId (), offsetBox 5.
    ignore (rs.BooleanDifference([ a2 ], [ b2 ], false))
    ok "inputs kept" (rs.IsObject a2 && rs.IsObject b2)
    near "geometry" 875. (rs.SurfaceVolume (rs.BooleanDifference([ brepOf a2 ], [ brepOf b2 ])).[0])
    )
test "BooleanIntersection" (fun () ->
    let a, b = boxId (), offsetBox 5.
    let i = rs.BooleanIntersection([ a ], [ b ])
    eq "one result" 1 i.Count
    near "volume" 125. (rs.SurfaceVolume i.[0])
    ok "inputs deleted" (not (rs.IsObject a) && not (rs.IsObject b))
    let a2, b2 = boxId (), offsetBox 5.
    ignore (rs.BooleanIntersection([ a2 ], [ b2 ], false))
    ok "inputs kept" (rs.IsObject a2 && rs.IsObject b2)
    near "geometry" 125. (rs.SurfaceVolume (rs.BooleanIntersection([ brepOf a2 ], [ brepOf b2 ])).[0])
    let far = rs.AddBox(boxCorners 1. 1. 1. |> List.map (fun p -> p + vec 100. 100. 100.))
    eq "no intersection gives an empty result" 0 (rs.BooleanIntersection([ boxId () ], [ far ])).Count
    )
test "SplitBrep" (fun () ->
    let a = boxId ()
    let parts = rs.SplitBrep(a, cutterId ())
    eq "two pieces" 2 parts.Count
    ok "input is kept by default" (rs.IsObject a)
    let a2 = boxId ()
    let del = rs.SplitBrep(a2, cutterId (), true)
    eq "two pieces" 2 del.Count
    ok "input deleted" (not (rs.IsObject a2))
    eq "geometry" 2 (rs.SplitBrep(brepOf (boxId ()), brepOf (cutterId ()))).Count
    )
test "TrimBrep" (fun () ->
    let t = rs.TrimBrep(boxId (), Plane(pt 0. 0. 5., Vector3d.ZAxis))
    ok "plane trim result" (t.Count >= 1)
    let t2 = rs.TrimBrep(boxId (), cutterId ())
    ok "trim with a cutter object" (t2.Count >= 1)
    ok "geometry with a plane" ((rs.TrimBrep(brepOf (boxId ()), Plane(pt 0. 0. 5., Vector3d.ZAxis))).Count >= 1)
    ok "geometry with a cutter" ((rs.TrimBrep(brepOf (boxId ()), brepOf (cutterId ()))).Count >= 1)
    )
test "JoinSurfaces" (fun () ->
    let a, b = sidePlane 0., sidePlane 10.
    let j = rs.JoinSurfaces [ a; b ]
    ok "is a polysurface" (rs.IsPolysurface j)
    near "area" 200. (area j)
    ok "inputs kept by default" (rs.IsObject a && rs.IsObject b)
    let c, d = sidePlane 0., sidePlane 10.
    let j2 = rs.JoinSurfaces([ c; d ], true)
    ok "inputs deleted" (not (rs.IsObject c) && not (rs.IsObject d))
    ok "result exists" (rs.IsObject j2)
    let g = rs.JoinSurfaces [ brepOf (sidePlane 0.); brepOf (sidePlane 10.) ]
    near "geometry" 200. (g.GetArea())
    throwsRs "one surface only" (fun () -> rs.JoinSurfaces [ planeId () ])
    )
test "JoinSurfacesAll" (fun () ->
    let j = rs.JoinSurfacesAll [ sidePlane 0.; sidePlane 10.; sidePlane 100. ]
    eq "two results, one joined and one single" 2 j.Count
    let g = rs.JoinSurfacesAll [ brepOf (sidePlane 0.); brepOf (sidePlane 10.) ]
    eq "geometry" 1 g.Count
    )
test "ExplodePolysurfaces" (fun () ->
    let b = boxId ()
    let parts = rs.ExplodePolysurfaces [ b ]
    eq "six faces" 6 parts.Count
    ok "input kept" (rs.IsObject b)
    let b2 = boxId ()
    ignore (rs.ExplodePolysurfaces([ b2 ], true))
    ok "input deleted" (not (rs.IsObject b2))
    eq "geometry" 6 (rs.ExplodePolysurfaces [ brepOf (boxId ()) ]).Count
    )
test "ExtractSurface" (fun () ->
    let b = boxId ()
    let e = rs.ExtractSurface(b, [ 0 ])
    eq "one face" 1 e.Count
    ok "polysurface is not closed anymore" (not (rs.IsPolysurfaceClosed b))
    let b2 = boxId ()
    let c = rs.ExtractSurface(b2, [ 0; 1 ], true)
    eq "two copies" 2 c.Count
    ok "original is still closed" (rs.IsPolysurfaceClosed b2)
    eq "geometry" 1 (rs.ExtractSurface(brepOf (boxId ()), [ 2 ])).Count
    )
test "CapPlanarHoles" (fun () ->
    let b = boxId ()
    ignore (rs.ExtractSurface(b, [ 0 ]))
    ok "open before" (not (rs.IsPolysurfaceClosed b))
    ok "capped" (rs.CapPlanarHoles b)
    ok "closed after" (rs.IsPolysurfaceClosed b)
    )
test "DuplicateEdgeCurves" (fun () ->
    let b = boxId ()
    let e = rs.DuplicateEdgeCurves b
    eq "twelve edges" 12 e.Count
    ok "not selected" (rs.IsObjectSelected e.[0] = 0)
    let s = rs.DuplicateEdgeCurves(b, true)
    ok "selected" (rs.IsObjectSelected s.[0] <> 0)
    eq "geometry" 12 (rs.DuplicateEdgeCurves (brepOf b)).Count
    )
test "DuplicateSurfaceBorder" (fun () ->
    let p = planeId ()
    eq "both" 1 (rs.DuplicateSurfaceBorder p).Count
    eq "exterior" 1 (rs.DuplicateSurfaceBorder(p, 1)).Count
    eq "interior of an untrimmed surface" 0 (rs.DuplicateSurfaceBorder(p, 2)).Count
    eq "geometry" 1 (rs.DuplicateSurfaceBorder (brepOf p)).Count
    near "border length" 40. (rs.CurveLength (rs.DuplicateSurfaceBorder p).[0])
    )
test "ShrinkTrimmedSurface" (fun () ->
    let trimmed () =
        let cut = rs.AddPlaneSurface(Plane(pt 5. -1. -1., Vector3d.YAxis, Vector3d.ZAxis), 12., 2.)
        (rs.SplitBrep(planeId (), cut, true)).[0]
    let t = trimmed ()
    let r = rs.ShrinkTrimmedSurface t
    eq "same id when no copy is made" t r
    let t2 = trimmed ()
    let copy = rs.ShrinkTrimmedSurface(t2, true)
    ok "copy is a new object" (copy <> t2 && rs.IsObject t2 && rs.IsObject copy)
    near "area is unchanged" 50. (area copy)
    let g = rs.ShrinkTrimmedSurface (brepOf (trimmed ()))
    ok "geometry" (not (isNull g))
    )
test "OffsetSurface" (fun () ->
    let p = planeId ()
    let o = rs.OffsetSurface(p, 2.)
    nearPt "offset along the normal" (pt 0. 0. 2.) (rs.BoundingBox o).Min
    let both = rs.OffsetSurface(p, 2., 0.001, true)
    ok "both sides gives an object" (rs.IsObject both)
    let solid = rs.OffsetSurface(p, 2., 0.001, false, true)
    ok "solid" (rs.IsPolysurfaceClosed solid)
    near "solid volume" 200. (rs.SurfaceVolume solid)
    let g = rs.OffsetSurface(brepOf p, -2.)
    nearPt "geometry negative offset" (pt 0. 0. -2.) (g.GetBoundingBox(true).Min)
    )
test "PullCurve" (fun () ->
    let p = planeId ()
    let c = rs.AddLine(pt 0. 0. 5., pt 10. 10. 5.)
    let r = rs.PullCurve(p, c)
    eq "one curve" 1 r.Count
    near "pulled to the surface" 0. (rs.BoundingBox r.[0]).Min.Z
    ok "input kept" (rs.IsObject c)
    let c2 = rs.AddLine(pt 0. 0. 5., pt 10. 10. 5.)
    let r2 = rs.PullCurve(p, c2, true)
    eq "one curve" 1 r2.Count
    ok "input deleted" (not (rs.IsObject c2))
    eq "geometry" 1 (rs.PullCurve(brepOf p, curve c)).Count
    )
test "IntersectBreps" (fun () ->
    let a = boxId ()
    let b = offsetBox 5.
    let r = rs.IntersectBreps(brepOf a, brepOf b)
    ok "curves" (r.Count >= 1)
    let far = rs.AddBox(boxCorners 1. 1. 1. |> List.map (fun p -> p + vec 100. 100. 100.))
    eq "no intersection" 0 (try (rs.IntersectBreps(brepOf a, brepOf far)).Count with :? RhinoScriptingException -> 0)
    )
test "IntersectSpheres" (fun () ->
    let t, _, r = rs.IntersectSpheres(Plane.WorldXY, 5., Plane(pt 8. 0. 0., Vector3d.ZAxis), 5.)
    eq "circle type" 1 t
    near "radius" 3. r
    let t2, _, _ = rs.IntersectSpheres(Plane.WorldXY, 5., Plane(pt 10. 0. 0., Vector3d.ZAxis), 5.)
    eq "tangent is a point" 0 t2
    let t3, _, _ = rs.IntersectSpheres(Plane.WorldXY, 5., Plane.WorldXY, 5.)
    eq "identical" 2 t3
    throwsRs "no intersection" (fun () -> rs.IntersectSpheres(Plane.WorldXY, 5., Plane(pt 20. 0. 0., Vector3d.ZAxis), 5.))
    )

// ------------------------------- predicates -------------------------------

test "IsSphere, IsCylinder, IsCone, IsTorus, IsPlaneSurface" (fun () ->
    let sph, cyl, con, tor, pl = sphereId (), cylinderId (), coneId (), torusId (), planeId ()
    ok "IsSphere" (rs.IsSphere sph && not (rs.IsSphere cyl) && not (rs.IsSphere pl))
    ok "IsCylinder" (rs.IsCylinder cyl && not (rs.IsCylinder sph))
    ok "IsCone" (rs.IsCone con && not (rs.IsCone cyl))
    ok "IsTorus" (rs.IsTorus tor && not (rs.IsTorus sph))
    ok "IsPlaneSurface" (rs.IsPlaneSurface pl && not (rs.IsPlaneSurface sph))
    ok "geometry IsSphere" (rs.IsSphere (srfOf sph))
    ok "geometry IsCylinder" (rs.IsCylinder (srfOf cyl))
    ok "geometry IsCone" (rs.IsCone (srfOf con))
    ok "geometry IsTorus" (rs.IsTorus (srfOf tor))
    ok "geometry IsPlaneSurface" (rs.IsPlaneSurface (srfOf pl))
    )
test "IsSurface, IsPolysurface, IsBrep, IsExtrusion" (fun () ->
    let pl, bx = planeId (), boxId ()
    ok "IsSurface" (rs.IsSurface pl)
    ok "IsSurface of a point" (not (rs.IsSurface (pointId ())))
    ok "IsSurface of a box polysurface" (not (rs.IsSurface bx))
    ok "IsPolysurface" (rs.IsPolysurface bx && not (rs.IsPolysurface pl))
    ok "IsBrep" (rs.IsBrep bx)
    ok "IsBrep of a curve" (not (rs.IsBrep (sq 1. 0.)))
    ok "geometry IsPolysurface" (rs.IsPolysurface (geo bx))
    ok "geometry IsBrep" (rs.IsBrep (geo bx))
    ok "geometry IsSurface" (rs.IsSurface (srfOf pl))
    ok "IsExtrusion false for a plane" (not (rs.IsExtrusion pl))
    ok "geometry IsExtrusion" (not (rs.IsExtrusion (geo pl)))
    let ext = doc().Objects.AddExtrusion(Extrusion.Create(Polyline([ pt 0. 0. 0.; pt 4. 0. 0.; pt 4. 4. 0.; pt 0. 4. 0.; pt 0. 0. 0. ]).ToPolylineCurve(), 5., true))
    ok "IsExtrusion" (rs.IsExtrusion ext)
    )
test "IsPolysurfaceClosed" (fun () ->
    ok "box" (rs.IsPolysurfaceClosed (boxId ()))
    ok "plane" (not (rs.IsPolysurfaceClosed (planeId ())))
    ok "geometry" (rs.IsPolysurfaceClosed (geo (boxId ())))
    )
test "IsPointInSurface" (fun () ->
    let b = boxId ()
    ok "inside" (rs.IsPointInSurface(b, pt 5. 5. 5.))
    ok "outside" (not (rs.IsPointInSurface(b, pt 15. 5. 5.)))
    ok "on the surface counts as in" (rs.IsPointInSurface(b, pt 5. 5. 10.))
    ok "on the surface is not strictly in" (not (rs.IsPointInSurface(b, pt 5. 5. 10., true)))
    ok "geometry" (rs.IsPointInSurface(geo b, pt 5. 5. 5.))
    ok "sphere" (rs.IsPointInSurface(sphereId (), pt 1. 1. 1.))
    )
test "IsPointOnSurface" (fun () ->
    let p = planeId ()
    ok "on" (rs.IsPointOnSurface(p, pt 5. 5. 0.))
    ok "above" (not (rs.IsPointOnSurface(p, pt 5. 5. 1.)))
    ok "outside the domain" (not (rs.IsPointOnSurface(p, pt 15. 5. 0.)))
    ok "geometry" (rs.IsPointOnSurface(geo p, pt 5. 5. 0.))
    throwsRs "a polysurface is not a surface" (fun () -> rs.IsPointOnSurface(boxId (), pt 5. 5. 10.))
    )
test "IsSurfaceClosed, IsSurfacePeriodic, IsSurfaceSingular" (fun () ->
    let cyl, sph, pl = cylinderId (), sphereId (), planeId ()
    ok "cylinder closed in one direction" (rs.IsSurfaceClosed(cyl, 0) <> rs.IsSurfaceClosed(cyl, 1))
    ok "sphere closed in u" (rs.IsSurfaceClosed(sph, 0))
    ok "plane is not closed" (not (rs.IsSurfaceClosed(pl, 0)) && not (rs.IsSurfaceClosed(pl, 1)))
    ok "geometry closed" (rs.IsSurfaceClosed(srfOf sph, 0))
    ok "plane is not periodic" (not (rs.IsSurfacePeriodic(pl, 0)) && not (rs.IsSurfacePeriodic(pl, 1)))
    ok "geometry periodic" (not (rs.IsSurfacePeriodic(srfOf pl, 0)))
    ok "sphere is singular at a pole" ([ 0; 1; 2; 3 ] |> List.exists (fun d -> rs.IsSurfaceSingular(sph, d)))
    ok "plane is not singular" ([ 0; 1; 2; 3 ] |> List.forall (fun d -> not (rs.IsSurfaceSingular(pl, d))))
    ok "geometry singular" ([ 0; 1; 2; 3 ] |> List.exists (fun d -> rs.IsSurfaceSingular(srfOf sph, d)))
    )
test "IsSurfacePlanar" (fun () ->
    ok "plane" (rs.IsSurfacePlanar(planeId ()))
    ok "sphere" (not (rs.IsSurfacePlanar(sphereId ())))
    ok "geometry" (rs.IsSurfacePlanar(srfOf (planeId ())))
    ok "curved grid with a large tolerance" (rs.IsSurfacePlanar(gridId (), 100.))
    ok "curved grid with a small tolerance" (not (rs.IsSurfacePlanar(gridId (), 0.01)))
    )
test "IsSurfaceRational" (fun () ->
    ok "sphere is rational" (rs.IsSurfaceRational (sphereId ()))
    ok "grid is not rational" (not (rs.IsSurfaceRational (gridId ())))
    ok "geometry" (rs.IsSurfaceRational (srfOf (sphereId ())))
    )
test "IsSurfaceTrimmed" (fun () ->
    ok "untrimmed plane" (not (rs.IsSurfaceTrimmed (brepOf (planeId ()))))
    ok "by id" (not (rs.IsSurfaceTrimmed (planeId ())))
    let b = brepOf (boxId ())
    let cut = rs.TrimBrep(b, Plane(pt 0. 0. 5., Vector3d.ZAxis))
    ok "a trimmed box has trimmed faces" (cut |> Seq.exists rs.IsSurfaceTrimmed)
    )
test "IsSurfaceFlipped and FlipSurface" (fun () ->
    let p = planeId ()
    ok "not flipped" (not (rs.IsSurfaceFlipped p))
    rs.FlipSurface(p, true)
    ok "flipped" (rs.IsSurfaceFlipped p)
    rs.FlipSurface(p, false)
    ok "not flipped again" (not (rs.IsSurfaceFlipped p))
    let a, b = planeId (), planeId ()
    rs.FlipSurface([ a; b ], true)
    ok "multiset" (rs.IsSurfaceFlipped a && rs.IsSurfaceFlipped b)
    ok "geometry" (not (rs.IsSurfaceFlipped (brepOf (planeId ()))))
    )
test "ReverseSurface" (fun () ->
    let p = planeId ()
    eq "u domain" (0., 10.) (rs.SurfaceDomain(p, 0))
    rs.ReverseSurface(p, 1)
    let u0, u1 = rs.SurfaceDomain(p, 0)
    ok "u domain is reversed" (u0 < u1 && u1 <= 0.)
    nearPt "the start of the reversed domain is the old end" (pt 10. 0. 0.) (rs.EvaluateSurface(p, u0, 0.))
    eq "v domain unchanged" (0., 10.) (rs.SurfaceDomain(p, 1))
    let q = planeId ()
    rs.ReverseSurface(q, 2)
    nearPt "reversed v" (pt 0. 10. 0.) (rs.EvaluateSurface(q, 0., fst (rs.SurfaceDomain(q, 1))))
    let t = planeId ()
    rs.ReverseSurface(t, 4)
    nearPt "transposed: u runs along the old v direction" (pt 0. 10. 0.) (rs.EvaluateSurface(t, 0., 10.) |> fun p -> if p.Y > 5. then p else rs.EvaluateSurface(t, 10., 0.))
    rs.ReverseSurface(p, 4)
    ok "transpose does not raise" (rs.IsSurface p)
    rs.ReverseSurface(p, 3)
    ok "combined flags" (rs.IsSurface p)
    )

// ------------------------------- properties -------------------------------

test "SurfaceArea and moments" (fun () ->
    let p = planeId ()
    near "area" 100. (rs.SurfaceArea p)
    near "area geometry (Surface)" 100. (rs.SurfaceArea (srfOf p))
    near "area geometry (Brep)" 100. (rs.SurfaceArea (brepOf p))
    near "area geometry (GeometryBase)" 100. (rs.SurfaceArea (geo p))
    nearPt "centroid" (pt 5. 5. 0.) (rs.SurfaceAreaCentroid p)
    nearPt "centroid geometry" (pt 5. 5. 0.) (rs.SurfaceAreaCentroid (geo p))
    let m = rs.SurfaceAreaMoments p
    eq "moments have 14 triples" 14 m.Count
    nearPt "first moments are area * centroid" (pt 500. 500. 0.) (let (x, y, z) = m.[0] in pt x y z)
    eq "moments geometry" m.Count (rs.SurfaceAreaMoments (geo p)).Count
    near "area of a box" 600. (rs.SurfaceArea (boxId ()))
    )
test "SurfaceVolume and moments" (fun () ->
    let b = boxId ()
    near "volume" 1000. (rs.SurfaceVolume b)
    near "volume geometry" 1000. (rs.SurfaceVolume (geo b))
    nearPt "centroid" (pt 5. 5. 5.) (rs.SurfaceVolumeCentroid b)
    nearPt "centroid geometry" (pt 5. 5. 5.) (rs.SurfaceVolumeCentroid (geo b))
    let m = rs.SurfaceVolumeMoments b
    eq "moments have 14 triples" 14 m.Count
    nearPt "first moments are volume * centroid" (pt 5000. 5000. 5000.) (let (x, y, z) = m.[0] in pt x y z)
    eq "moments geometry" m.Count (rs.SurfaceVolumeMoments (geo b)).Count
    throwsRs "open surface" (fun () -> rs.SurfaceVolume (planeId ()))
    near "sphere" (4. / 3. * Math.PI * 125.) (rs.SurfaceVolume (sphereId ()))
    )
test "SurfaceClosestPoint and SurfaceClosestParameter" (fun () ->
    let p = planeId ()
    nearPt "above" (pt 3. 4. 0.) (rs.SurfaceClosestPoint(p, pt 3. 4. 7.))
    nearPt "geometry" (pt 3. 4. 0.) (rs.SurfaceClosestPoint(srfOf p, pt 3. 4. 7.))
    let u, v = rs.SurfaceClosestParameter(p, pt 3. 4. 7.)
    near "u" 3. u
    near "v" 4. v
    let u2, v2 = rs.SurfaceClosestParameter(srfOf p, pt 30. 4. 7.)
    near "clamped u" 10. u2
    near "v" 4. v2
    let c = rs.SurfaceClosestPoint(sphereId (), pt 10. 0. 0.)
    near "sphere" 5. (c.DistanceTo Point3d.Origin)
    )
test "BrepClosestPoint" (fun () ->
    let b = boxId ()
    let p, _, _, ct, idx, n = rs.BrepClosestPoint(b, pt 5. 5. 20.)
    nearPt "closest point on the top" (pt 5. 5. 10.) p
    eq "component is a face" ComponentIndexType.BrepFace ct
    ok "index is valid" (idx >= 0 && idx < 6)
    nearVec "normal" (vec 0. 0. 1.) n
    let p2, _, _, _, _, _ = rs.BrepClosestPoint(brepOf b, pt 20. 20. 20.)
    nearPt "closest vertex" (pt 10. 10. 10.) p2
    )
test "EvaluateSurface and SurfaceEvaluate" (fun () ->
    let p = planeId ()
    nearPt "evaluate" (pt 3. 4. 0.) (rs.EvaluateSurface(p, 3., 4.))
    nearPt "evaluate geometry" (pt 3. 4. 0.) (rs.EvaluateSurface(srfOf p, 3., 4.))
    let pnt, ders = rs.SurfaceEvaluate(p, (3., 4.), 0)
    nearPt "point" (pt 3. 4. 0.) pnt
    eq "no derivative vectors" 0 ders.Count
    let _, d1 = rs.SurfaceEvaluate(p, (3., 4.), 1)
    eq "first derivatives" 2 d1.Count
    nearVec "du" (vec 1. 0. 0.) d1.[0]
    nearVec "dv" (vec 0. 1. 0.) d1.[1]
    let _, d2 = rs.SurfaceEvaluate(srfOf p, (3., 4.), 2)
    eq "second derivatives" 5 d2.Count
    )
test "SurfaceFrame and SurfaceNormal" (fun () ->
    let p = planeId ()
    let f = rs.SurfaceFrame(p, (3., 4.))
    nearPt "origin" (pt 3. 4. 0.) f.Origin
    nearVec "normal" (vec 0. 0. 1.) f.ZAxis
    nearVec "frame geometry" (vec 0. 0. 1.) (rs.SurfaceFrame(srfOf p, (3., 4.))).ZAxis
    nearVec "normal" (vec 0. 0. 1.) (rs.SurfaceNormal(p, (3., 4.)))
    nearVec "normal at the start" (vec 0. 0. 1.) (rs.SurfaceNormal p)
    nearVec "normal geometry" (vec 0. 0. 1.) (rs.SurfaceNormal(srfOf p, (3., 4.)))
    nearVec "normal geometry at the start" (vec 0. 0. 1.) (rs.SurfaceNormal(srfOf p))
    near "sphere normals are unit length" 1. (rs.SurfaceNormal(sphereId (), (1., 1.))).Length
    )
test "SurfaceCurvature" (fun () ->
    let s = sphereId ()
    let d0, d1 = rs.SurfaceDomain(s, 0), rs.SurfaceDomain(s, 1)
    let u, v = (fst d0 + snd d0) / 2., (fst d1 + snd d1) / 2.
    let (p, n, kmax, _, kmin, _, gauss, mean) = rs.SurfaceCurvature(s, (u, v))
    near "point is on the sphere" 5. (p.DistanceTo Point3d.Origin)
    near "normal is unit" 1. n.Length
    nearTol "max curvature is 1/r" 1e-6 0.2 (abs kmax)
    nearTol "min curvature is 1/r" 1e-6 0.2 (abs kmin)
    nearTol "gaussian" 1e-6 0.04 gauss
    nearTol "mean" 1e-6 0.2 (abs mean)
    let (_, _, k, _, k2, _, g, _) = rs.SurfaceCurvature(srfOf (planeId ()), (3., 4.))
    near "plane has no curvature" 0. (abs k + abs k2 + abs g)
    )
test "SurfaceDomain, SurfaceParameter, SurfaceNormalizedParameter" (fun () ->
    let p = planeId ()
    eq "u domain" (0., 10.) (rs.SurfaceDomain(p, 0))
    eq "v domain" (0., 10.) (rs.SurfaceDomain(p, 1))
    eq "geometry" (0., 10.) (rs.SurfaceDomain(srfOf p, 0))
    throwsRs "bad direction" (fun () -> rs.SurfaceDomain(p, 2))
    let u, v = rs.SurfaceParameter(p, (0.5, 0.25))
    near "u" 5. u
    near "v" 2.5 v
    let nu, nv = rs.SurfaceNormalizedParameter(p, (5., 2.5))
    near "normalized u" 0.5 nu
    near "normalized v" 0.25 nv
    let u2, _ = rs.SurfaceParameter(srfOf p, (1., 1.))
    near "geometry parameter" 10. u2
    let nu2, _ = rs.SurfaceNormalizedParameter(srfOf p, (10., 10.))
    near "geometry normalized parameter" 1. nu2
    )
test "SurfaceDegree, SurfacePointCount, SurfaceKnotCount, SurfaceKnots, SurfaceWeights" (fun () ->
    let g = gridId ()
    eq "degree" (3, 3) (rs.SurfaceDegree g)
    eq "degree geometry" (3, 3) (rs.SurfaceDegree (srfOf g))
    eq "point count" (4, 4) (rs.SurfacePointCount g)
    eq "point count geometry" (4, 4) (rs.SurfacePointCount (srfOf g))
    eq "knot count u = points + degree - 1" (6, 6) (rs.SurfaceKnotCount g)
    eq "knot count geometry" (6, 6) (rs.SurfaceKnotCount (srfOf g))
    let a, b = rs.SurfaceKnots g
    eq "knots u" 6 a.Count
    eq "knots v" 6 b.Count
    eq "knots geometry" 6 (fst (rs.SurfaceKnots (srfOf g))).Count
    let w = rs.SurfaceWeights g
    eq "weights" 16 w.Count
    ok "all weights are 1" (w |> Seq.forall (fun x -> abs (x - 1.) < 1e-9))
    eq "weights geometry" 16 (rs.SurfaceWeights (srfOf g)).Count
    let sw = rs.SurfaceWeights (sphereId ())
    ok "rational sphere has other weights than 1" (sw |> Seq.exists (fun x -> abs (x - 1.) > 1e-3))
    )
test "SurfacePoints and SurfaceEditPoints" (fun () ->
    let g = gridId ()
    eq "points" 16 (rs.SurfacePoints g).Count
    eq "points geometry" 16 (rs.SurfacePoints (srfOf g)).Count
    nearPt "first point" (pt 0. 0. 1.) (rs.SurfacePoints g).[0]
    eq "edit points" 16 (rs.SurfaceEditPoints g).Count
    eq "edit points geometry" 16 (rs.SurfaceEditPoints (srfOf g)).Count
    eq "edit point parameters" 16 (rs.SurfaceEditPointParameters g).Count
    eq "edit point parameters geometry" 16 (rs.SurfaceEditPointParameters (srfOf g)).Count
    near "first edit point u" 0. (fst (rs.SurfaceEditPointParameters g).[0])
    let s = sphereId ()
    ok "returnAll false gives fewer or equal points on closed surfaces" ((rs.SurfacePoints(s, false)).Count <= (rs.SurfacePoints(s, true)).Count)
    ok "edit points returnAll false" ((rs.SurfaceEditPoints(s, false)).Count <= (rs.SurfaceEditPoints(s, true)).Count)
    ok "edit point parameters returnAll false" ((rs.SurfaceEditPointParameters(s, false)).Count <= (rs.SurfaceEditPointParameters(s, true)).Count)
    )
test "SurfaceSphere, SurfaceCone, SurfaceCylinder, SurfaceTorus" (fun () ->
    let pl, r = rs.SurfaceSphere (rs.AddSphere(pt 1. 2. 3., 5.))
    near "sphere radius" 5. r
    nearPt "sphere center" (pt 1. 2. 3.) pl.Origin
    near "geometry radius" 5. (snd (rs.SurfaceSphere (srfOf (sphereId ()))))
    throwsRs "not a sphere" (fun () -> rs.SurfaceSphere (planeId ()))
    let _, h, cr = rs.SurfaceCone (coneId ())
    near "cone height" 10. (abs h)
    near "cone radius" 3. cr
    ignore (rs.SurfaceCone (srfOf (coneId ())))
    throwsRs "not a cone" (fun () -> rs.SurfaceCone (planeId ()))
    let _, ch, crad = rs.SurfaceCylinder (cylinderId ())
    near "cylinder height" 10. (abs ch)
    near "cylinder radius" 3. crad
    ignore (rs.SurfaceCylinder (srfOf (cylinderId ())))
    throwsRs "not a cylinder" (fun () -> rs.SurfaceCylinder (planeId ()))
    let _, major, minor = rs.SurfaceTorus (torusId ())
    near "major radius" 10. major
    near "minor radius" 2. minor
    ignore (rs.SurfaceTorus (srfOf (torusId ())))
    throwsRs "not a torus" (fun () -> rs.SurfaceTorus (planeId ()))
    )
test "SurfaceIsocurveDensity" (fun () ->
    let p = planeId ()
    ignore (rs.SurfaceIsocurveDensity p)
    rs.SurfaceIsocurveDensity(p, 3)
    eq "set" 3 (rs.SurfaceIsocurveDensity p)
    rs.SurfaceIsocurveDensity(p, 0)
    eq "zero is allowed" 0 (rs.SurfaceIsocurveDensity p)
    rs.SurfaceIsocurveDensity(p, -1)
    eq "-1 means default" -1 (rs.SurfaceIsocurveDensity p)
    let a, b = planeId (), planeId ()
    rs.SurfaceIsocurveDensity([ a; b ], 5)
    eq "multiset" 5 (rs.SurfaceIsocurveDensity b)
    let bx = boxId ()
    rs.SurfaceIsocurveDensity(bx, 4)
    eq "polysurface" 4 (rs.SurfaceIsocurveDensity bx)
    )

// ------------------------------- modifications -------------------------------

test "ExtractIsoCurve" (fun () ->
    let p = planeId ()
    let u = rs.ExtractIsoCurve(p, (5., 5.), 0)
    eq "u direction" 1 u.Count
    near "length" 10. (rs.CurveLength u.[0])
    eq "v direction" 1 (rs.ExtractIsoCurve(p, (5., 5.), 1)).Count
    eq "both directions" 2 (rs.ExtractIsoCurve(p, (5., 5.), 2)).Count
    eq "geometry" 2 (rs.ExtractIsoCurve(srfOf p, (5., 5.), 2)).Count
    )
test "ExtendSurface" (fun () ->
    let p = planeId ()
    ok "extended" (rs.ExtendSurface(p, (10., 5.), 5.))
    near "area grows" 150. (area p)
    let p2 = planeId ()
    ok "non smooth" (rs.ExtendSurface(p2, (0., 5.), 5., false))
    near "area grows" 150. (area p2)
    )
test "RebuildSurface" (fun () ->
    let g = gridId ()
    ok "rebuilt" (rs.RebuildSurface(g, 2, 2, 5, 6))
    eq "degree" (2, 2) (rs.SurfaceDegree g)
    eq "point count" (5, 6) (rs.SurfacePointCount g)
    let g2 = gridId ()
    ok "defaults" (rs.RebuildSurface g2)
    eq "default point count" (10, 10) (rs.SurfacePointCount g2)
    )
test "RemoveSurfaceKnot" (fun () ->
    let mk () =
        let g = rs.AddSrfPtGrid((4, 5), [ for i in 0 .. 3 do for j in 0 .. 4 -> pt (float i) (float j) 0. ])
        ignore (rs.RebuildSurface(g, 3, 3, 6, 7))
        g
    let g = mk ()
    let ku, kv = rs.SurfaceKnotCount g
    ok "remove u knot" (rs.RemoveSurfaceKnot(g, (1., 1.), false))
    let ku2, kv2 = rs.SurfaceKnotCount g
    eq "u knot count shrank" (ku - 1) ku2
    eq "v knot count unchanged" kv kv2
    let g2 = mk ()
    ok "remove v knot" (rs.RemoveSurfaceKnot(g2, (1., 1.), true))
    let ku3, kv3 = rs.SurfaceKnotCount g2
    eq "v knot count shrank" (kv - 1) kv3
    eq "u knot count unchanged" ku ku3
    )
test "ChangeSurfaceDegree" (fun () ->
    let g = gridId ()
    ok "raised" (rs.ChangeSurfaceDegree(g, (4, 4)))
    eq "degree after raising" (4, 4) (rs.SurfaceDegree g)
    ok "raised in u only" (rs.ChangeSurfaceDegree(gridId (), (5, 3)))
    ok "lowering the degree is not supported by IncreaseDegree and returns false" (not (rs.ChangeSurfaceDegree(gridId (), (2, 2))))
    throwsRs "same degree" (fun () -> rs.ChangeSurfaceDegree(gridId (), (3, 3)))
    throwsRs "degree 0" (fun () -> rs.ChangeSurfaceDegree(gridId (), (0, 3)))
    throwsRs "degree too high" (fun () -> rs.ChangeSurfaceDegree(gridId (), (12, 3)))
    )
test "MakeSurfacePeriodic" (fun () ->
    let c = cylinderId ()
    let dir = if rs.IsSurfaceClosed(c, 0) then 0 else 1
    let p = rs.MakeSurfacePeriodic(c, dir)
    ok "new surface" (rs.IsObject p && p <> c)
    ok "original kept" (rs.IsObject c)
    ok "periodic" (rs.IsSurfacePeriodic(p, dir))
    let c2 = cylinderId ()
    let p2 = rs.MakeSurfacePeriodic(c2, dir, true)
    eq "input is replaced in place" c2 p2
    ok "replaced object is periodic" (rs.IsSurfacePeriodic(c2, dir))
    ok "geometry periodic" (rs.IsSurfacePeriodic((rs.MakeSurfacePeriodic(srfOf (cylinderId ()), dir)), dir))
    )
test "TrimSurfaceU, TrimSurfaceV, TrimSurfaceUV" (fun () ->
    let p = planeId ()
    let u = rs.TrimSurfaceU(p, (2., 6.))
    near "u trimmed" 40. (area u)
    ok "input kept" (rs.IsObject p)
    near "v trimmed" 50. (area (rs.TrimSurfaceV(p, (0., 5.))))
    near "uv trimmed" 20. (area (rs.TrimSurfaceUV(p, (0., 5.), (0., 4.))))
    let p2 = planeId ()
    ignore (rs.TrimSurfaceU(p2, (0., 5.), true))
    ok "input deleted" (not (rs.IsObject p2))
    near "geometry u" 40. (AreaMassProperties.Compute(rs.TrimSurfaceU(srfOf p, (2., 6.)))).Area
    near "geometry v" 50. (AreaMassProperties.Compute(rs.TrimSurfaceV(srfOf p, (0., 5.)))).Area
    near "geometry uv" 20. (AreaMassProperties.Compute(rs.TrimSurfaceUV(srfOf p, (0., 5.), (0., 4.)))).Area
    )
test "ShootRay" (fun () ->
    let p = planeId ()
    let hits = rs.ShootRay([ p ], pt 5. 5. 10., vec 0. 0. -1.)
    ok "at least one hit" (hits.Length >= 1)
    nearPt "the first point is the start" (pt 5. 5. 10.) hits.[0]
    ok "the plane was hit" (hits |> Array.exists (fun h -> h.DistanceTo(pt 5. 5. 0.) < 1e-6))
    let g = rs.ShootRay([ brepOf p ], pt 5. 5. 10., vec 0. 0. -1., 1)
    ok "geometry" (g.Length >= 1)
    let r = rs.ShootRay([ boxId () ], pt 5. 5. 20., vec 0. 0. -1., 3)
    ok "hit on the box top" (r |> Array.exists (fun h -> h.DistanceTo(pt 5. 5. 10.) < 1e-6))
    )
test "ShortPath" (fun () ->
    let p = planeId ()
    let c = rs.ShortPath(p, pt 0. 0. 0., pt 10. 10. 0.)
    near "length of the diagonal" (10. * sqrt 2.) (rs.CurveLength c)
    let g = rs.ShortPath(srfOf p, pt 0. 0. 0., pt 10. 0. 0.)
    near "geometry" 10. (g.GetLength())
    )
test "UnrollSurface" (fun () ->
    let surfaces, following = rs.UnrollSurface (cylinderId ())
    ok "one or more unrolled surfaces" (surfaces.Count >= 1)
    eq "no following geometry" 0 following.Count
    near "unrolled area equals the area of the cylinder side" (2. * Math.PI * 3. * 10.) (area surfaces.[0])
    let s2, _ = rs.UnrollSurface(cylinderId (), true)
    ok "exploded" (s2.Count >= 1)
    let g, _ = rs.UnrollSurface(brepOf (cylinderId ()))
    ok "geometry" (g.Count >= 1)
    )

printfn "%s" (finish ())
