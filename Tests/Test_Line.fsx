// Tests for Src/Scripting_Line.fs
#load "Harness.fsx"
open System
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Line"

let xLine = line (pt 0. 0. 0.) (pt 10. 0. 0.) // finite line on the X axis from 0 to 10

test "LineClosestPointFinite" (fun () ->
    nearPt "inside the segment" (pt 5. 0. 0.) (rs.LineClosestPointFinite(xLine, pt 5. 5. 0.))
    nearPt "beyond the end is clamped to the end" (pt 10. 0. 0.) (rs.LineClosestPointFinite(xLine, pt 15. 5. 0.))
    nearPt "before the start is clamped to the start" (pt 0. 0. 0.) (rs.LineClosestPointFinite(xLine, pt -3. 2. 1.))
    )

test "LineClosestPoint" (fun () ->
    nearPt "inside the segment" (pt 5. 0. 0.) (rs.LineClosestPoint(xLine, pt 5. 5. 0.))
    nearPt "beyond the end is not clamped" (pt 15. 0. 0.) (rs.LineClosestPoint(xLine, pt 15. 5. 0.))
    nearPt "before the start is not clamped" (pt -3. 0. 0.) (rs.LineClosestPoint(xLine, pt -3. 2. 1.))
    )

test "LineCylinderIntersection" (fun () ->
    let hits = rs.LineCylinderIntersection(line (pt -5. 0. 5.) (pt 5. 0. 5.), Plane.WorldXY, 10., 2.)
    eq "two hits through the middle" 2 hits.Length
    let xs = hits |> Array.map (fun p -> p.X) |> Array.sort
    near "first x" -2. xs.[0]
    near "second x" 2. xs.[1]
    eq "no hit when missing" 0 (rs.LineCylinderIntersection(line (pt -5. 5. 5.) (pt 5. 5. 5.), Plane.WorldXY, 10., 2.)).Length
    let tangent = rs.LineCylinderIntersection(line (pt -5. 2. 5.) (pt 5. 2. 5.), Plane.WorldXY, 10., 2.)
    ok "tangent line hits at most once or twice at the same place" (tangent.Length <= 2)
    throwsRs "zero radius is an invalid circle" (fun () -> rs.LineCylinderIntersection(xLine, Plane.WorldXY, 10., 0.))
    )

test "LineIsFartherThan" (fun () ->
    ok "point is farther than 3" (rs.LineIsFartherThan(xLine, 3., pt 5. 5. 0.))
    ok "point is not farther than 6" (not (rs.LineIsFartherThan(xLine, 6., pt 5. 5. 0.)))
    ok "point beyond the end uses the finite chord" (rs.LineIsFartherThan(xLine, 6., pt 15. 5. 0.))
    let l2 = line (pt 0. 4. 0.) (pt 10. 4. 0.)
    ok "parallel line at distance 4 is farther than 3" (rs.LineIsFartherThan(xLine, 3., l2))
    ok "parallel line at distance 4 is not farther than 5" (not (rs.LineIsFartherThan(xLine, 5., l2)))
    )

test "LineLineIntersection" (fun () ->
    let a, b = rs.LineLineIntersection(xLine, line (pt 5. -5. 2.) (pt 5. 5. 2.))
    nearPt "point on first line" (pt 5. 0. 0.) a
    nearPt "point on second line" (pt 5. 0. 2.) b
    let a2, b2 = rs.LineLineIntersection(xLine, line (pt 20. -5. 0.) (pt 20. 5. 0.))
    nearPt "lines are considered endless, point on first" (pt 20. 0. 0.) a2
    nearPt "lines are considered endless, point on second" (pt 20. 0. 0.) b2
    throwsRs "parallel lines" (fun () -> rs.LineLineIntersection(xLine, line (pt 0. 1. 0.) (pt 10. 1. 0.)))
    throwsRs "collinear lines" (fun () -> rs.LineLineIntersection(xLine, line (pt 20. 0. 0.) (pt 30. 0. 0.)))
    )

test "LineMaxDistanceTo" (fun () ->
    near "to a point: far end of the chord" (sqrt 109.) (rs.LineMaxDistanceTo(xLine, pt 0. 3. 0.))
    let l2 = line (pt 0. 4. 0.) (pt 10. 4. 0.)
    ok "to a line: at least the offset" (rs.LineMaxDistanceTo(xLine, l2) >= 4.)
    near "to a line: opposite corners" (sqrt 116.) (rs.LineMaxDistanceTo(xLine, l2))
    )

test "LineMinDistanceTo" (fun () ->
    near "to a point beside the line" 3. (rs.LineMinDistanceTo(xLine, pt 5. 3. 0.))
    near "to a point beyond the end, finite chord" 5. (rs.LineMinDistanceTo(xLine, pt 15. 0. 0.))
    near "to a parallel line" 4. (rs.LineMinDistanceTo(xLine, line (pt 0. 4. 0.) (pt 10. 4. 0.)))
    near "to a crossing line" 0. (rs.LineMinDistanceTo(xLine, line (pt 5. -1. 0.) (pt 5. 1. 0.)))
    )

test "LinePlane" (fun () ->
    let p = rs.LinePlane xLine
    nearPt "origin at the start of the line" (pt 0. 0. 0.) p.Origin
    near "end point lies in the plane" 0. (abs (p.DistanceTo (xLine.To)))
    near "the line direction is perpendicular to the normal" 0. (abs (p.Normal * Vector3d.XAxis))
    let diag = line (pt 1. 1. 1.) (pt 4. 5. 1.)
    let p2 = rs.LinePlane diag
    nearPt "origin at the start of a skew line" (pt 1. 1. 1.) p2.Origin
    near "end point of a skew line lies in the plane" 0. (abs (p2.DistanceTo diag.To))
    ok "a plane parallel to world XY is preferred if possible" (abs (abs p2.Normal.Z) > 0.999)
    throwsRs "zero length line has no plane" (fun () -> rs.LinePlane (line (pt 1. 1. 1.) (pt 1. 1. 1.)))
    )

test "LinePlaneIntersection" (fun () ->
    nearPt "vertical line through XY" (pt 0. 0. 0.) (rs.LinePlaneIntersection(line (pt 0. 0. -5.) (pt 0. 0. 5.), Plane.WorldXY))
    nearPt "the intersection is on the infinite line" (pt 0. 0. 0.) (rs.LinePlaneIntersection(line (pt 0. 0. 5.) (pt 0. 0. 10.), Plane.WorldXY))
    throwsRs "parallel line" (fun () -> rs.LinePlaneIntersection(line (pt 0. 0. 1.) (pt 5. 0. 1.), Plane.WorldXY))
    )

test "LineSphereIntersection" (fun () ->
    let hits = rs.LineSphereIntersection(line (pt -5. 0. 0.) (pt 5. 0. 0.), Point3d.Origin, 2.)
    eq "two hits" 2 hits.Length
    let xs = hits |> Array.map (fun p -> p.X) |> Array.sort
    near "first x" -2. xs.[0]
    near "second x" 2. xs.[1]
    eq "no hit" 0 (rs.LineSphereIntersection(line (pt -5. 5. 0.) (pt 5. 5. 0.), Point3d.Origin, 2.)).Length
    ok "tangent gives at most one hit (floating point may miss it)" ((rs.LineSphereIntersection(line (pt -5. 2. 0.) (pt 5. 2. 0.), Point3d.Origin, 2.)).Length <= 1)
    )

test "LineTransform" (fun () ->
    let move = Transform.Translation(0., 0., 5.)
    // Line geometry overload: returns a new line and leaves the input alone
    let moved = rs.LineTransform(xLine, move)
    nearPt "new line start" (pt 0. 0. 5.) moved.From
    nearPt "new line end" (pt 10. 0. 5.) moved.To
    nearPt "input line is unchanged" (pt 0. 0. 0.) xLine.From
    // object overload, in place
    let id = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    let n = doc().Objects.Count
    let id2 = rs.LineTransform(id, move)
    eq "copy=false keeps the Guid" id id2
    eq "no object added" n (doc().Objects.Count)
    nearPt "moved in place" (pt 0. 0. 5.) (rs.CurveStartPoint id)
    // object overload, copy
    let id3 = rs.LineTransform(id, move, true)
    ok "copy=true creates a new Guid" (id3 <> id)
    eq "one object added" (n + 1) (doc().Objects.Count)
    nearPt "original unchanged" (pt 0. 0. 5.) (rs.CurveStartPoint id)
    nearPt "copy transformed" (pt 0. 0. 10.) (rs.CurveStartPoint id3)
    // errors
    throws "a non-line object" (fun () -> rs.LineTransform(rs.AddCircle(Plane.WorldXY, 1.), move))
    throws "unknown Guid" (fun () -> rs.LineTransform(Guid.NewGuid(), move))
    )

printfn "%s" (finish ())
