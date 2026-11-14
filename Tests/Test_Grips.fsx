// Tests for Src/Scripting_Grips.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Grips"

/// a polyline with 4 vertices, it has 4 grips when they are on
let newPolyline () = rs.AddPolyline [ pt 0. 0. 0.; pt 10. 0. 0.; pt 10. 10. 0.; pt 20. 10. 0. ]

test "EnableObjectGrips" (fun () ->
    let c = newPolyline ()
    ok "grips are off at first" (not (rs.ObjectGripsOn c))
    ok "enable succeeds" (rs.EnableObjectGrips c)
    ok "grips are on" (rs.ObjectGripsOn c)
    ok "enable again succeeds" (rs.EnableObjectGrips(c, true))
    ok "disable succeeds" (rs.EnableObjectGrips(c, false))
    ok "grips are off" (not (rs.ObjectGripsOn c))
    throws "unknown object" (fun () -> rs.EnableObjectGrips (Guid.NewGuid()))
    )

test "ObjectGripsOn" (fun () ->
    let c = newPolyline ()
    ok "off" (not (rs.ObjectGripsOn c))
    rs.EnableObjectGrips c |> ignore
    ok "on" (rs.ObjectGripsOn c)
    throws "unknown object" (fun () -> rs.ObjectGripsOn (Guid.NewGuid()))
    )

test "ObjectGripCount" (fun () ->
    let c = newPolyline ()
    throwsRs "grips are off: the grips can not be counted" (fun () -> rs.ObjectGripCount c)
    rs.EnableObjectGrips c |> ignore
    eq "grip count of a polyline with 4 vertices" 4 (rs.ObjectGripCount c)
    let l = rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.)
    rs.EnableObjectGrips l |> ignore
    eq "grip count of a line with grips on" 2 (rs.ObjectGripCount l)
    // point objects have no grips
    let p = rs.AddPoint(0., 0., 0.)
    ok "grips can not be turned on for a point" (not (rs.EnableObjectGrips p))
    throwsRs "a point has no grips" (fun () -> rs.ObjectGripCount p)
    throws "unknown object" (fun () -> rs.ObjectGripCount (Guid.NewGuid()))
    )

test "ObjectGripLocation" (fun () ->
    let c = newPolyline ()
    throwsRs "get: grips are off" (fun () -> rs.ObjectGripLocation(c, 0))
    throwsRs "set: grips are off" (fun () -> rs.ObjectGripLocation(c, 0, pt 1. 1. 1.))
    rs.EnableObjectGrips c |> ignore
    nearPt "first grip" (pt 0. 0. 0.) (rs.ObjectGripLocation(c, 0))
    nearPt "last grip" (pt 20. 10. 0.) (rs.ObjectGripLocation(c, 3))
    rs.ObjectGripLocation(c, 1, pt 10. 5. 0.)
    nearPt "grip was moved" (pt 10. 5. 0.) (rs.ObjectGripLocation(c, 1))
    throwsRs "get: negative index" (fun () -> rs.ObjectGripLocation(c, -1))
    throwsRs "get: index too big" (fun () -> rs.ObjectGripLocation(c, 4))
    throwsRs "set: index too big" (fun () -> rs.ObjectGripLocation(c, 4, pt 0. 0. 0.))
    )

test "ObjectGripLocations" (fun () ->
    let c = newPolyline ()
    throwsRs "get: grips are off" (fun () -> rs.ObjectGripLocations c)
    rs.EnableObjectGrips c |> ignore
    let locs = rs.ObjectGripLocations c
    eq "four locations" 4 locs.Count
    nearPt "first" (pt 0. 0. 0.) locs.[0]
    nearPt "last" (pt 20. 10. 0.) locs.[3]
    rs.ObjectGripLocations(c, [ pt 0. 0. 1.; pt 10. 0. 1.; pt 10. 10. 1.; pt 20. 10. 1. ])
    let moved = rs.ObjectGripLocations c
    nearPt "moved first" (pt 0. 0. 1.) moved.[0]
    nearPt "moved last" (pt 20. 10. 1.) moved.[3]
    throwsRs "wrong number of points" (fun () -> rs.ObjectGripLocations(c, [ pt 0. 0. 0. ]))
    rs.EnableObjectGrips(c, false) |> ignore
    throwsRs "set: grips are off" (fun () -> rs.ObjectGripLocations(c, [ pt 0. 0. 0.; pt 1. 0. 0.; pt 2. 0. 0.; pt 3. 0. 0. ]))
    )

test "NextObjectGrip" (fun () ->
    let c = newPolyline ()
    rs.EnableObjectGrips c |> ignore
    eq "next of 0" 1 (rs.NextObjectGrip(c, 0))
    eq "next of 2" 3 (rs.NextObjectGrip(c, 2))
    eq "next with direction 0 and enable=false" 2 (rs.NextObjectGrip(c, 1, 0, false))
    throwsRs "there is no grip after the last one" (fun () -> rs.NextObjectGrip(c, 3))
    throwsRs "index too big" (fun () -> rs.NextObjectGrip(c, 10))
    throwsRs "negative index" (fun () -> rs.NextObjectGrip(c, -1))
    throws "unknown object" (fun () -> rs.NextObjectGrip(Guid.NewGuid(), 0))
    )

test "PrevObjectGrip" (fun () ->
    let c = newPolyline ()
    rs.EnableObjectGrips c |> ignore
    eq "previous of 3" 2 (rs.PrevObjectGrip(c, 3))
    eq "previous of 1" 0 (rs.PrevObjectGrip(c, 1))
    eq "previous with direction 0 and enable=false" 1 (rs.PrevObjectGrip(c, 2, 0, false))
    throwsRs "there is no grip before the first one" (fun () -> rs.PrevObjectGrip(c, 0))
    throwsRs "index too big" (fun () -> rs.PrevObjectGrip(c, 10))
    throws "unknown object" (fun () -> rs.PrevObjectGrip(Guid.NewGuid(), 1))
    )

test "SelectObjectGrip" (fun () ->
    let c = newPolyline ()
    ok "grips are off: no selection" (not (rs.SelectObjectGrip(c, 0)))
    rs.EnableObjectGrips c |> ignore
    ok "select grip 1" (rs.SelectObjectGrip(c, 1))
    eq "it is selected" [ 1 ] (List.ofSeq (rs.SelectedObjectGrips c))
    ok "select grip 2 too" (rs.SelectObjectGrip(c, 2))
    eq "both are selected" [ 1; 2 ] (rs.SelectedObjectGrips c |> Seq.sort |> List.ofSeq)
    ok "index too big" (not (rs.SelectObjectGrip(c, 10)))
    ok "negative index" (not (rs.SelectObjectGrip(c, -1)))
    )

test "UnselectObjectGrip" (fun () ->
    let c = newPolyline ()
    ok "grips are off: nothing to unselect" (not (rs.UnselectObjectGrip(c, 0)))
    rs.EnableObjectGrips c |> ignore
    rs.SelectObjectGrips c |> ignore
    ok "unselect grip 1" (rs.UnselectObjectGrip(c, 1))
    eq "the others stay selected" [ 0; 2; 3 ] (rs.SelectedObjectGrips c |> Seq.sort |> List.ofSeq)
    ok "index too big" (not (rs.UnselectObjectGrip(c, 10)))
    )

test "SelectObjectGrips" (fun () ->
    let c = newPolyline ()
    throwsRs "grips are off" (fun () -> rs.SelectObjectGrips c)
    rs.EnableObjectGrips c |> ignore
    eq "all four grips are selected" 4 (rs.SelectObjectGrips c)
    ok "grips are selected" (rs.ObjectGripsSelected c)
    )

test "UnselectObjectGrips" (fun () ->
    let c = newPolyline ()
    throwsRs "grips are off" (fun () -> rs.UnselectObjectGrips c)
    rs.EnableObjectGrips c |> ignore
    rs.SelectObjectGrips c |> ignore
    eq "all four grips are unselected" 4 (rs.UnselectObjectGrips c)
    ok "no grip is selected" (not (rs.ObjectGripsSelected c))
    ok "grips stay on" (rs.ObjectGripsOn c)
    )

test "ObjectGripsSelected" (fun () ->
    let c = newPolyline ()
    ok "grips off: false" (not (rs.ObjectGripsSelected c))
    rs.EnableObjectGrips c |> ignore
    ok "grips on but none selected: false" (not (rs.ObjectGripsSelected c))
    rs.SelectObjectGrip(c, 2) |> ignore
    ok "one selected: true" (rs.ObjectGripsSelected c)
    )

test "SelectedObjectGrips" (fun () ->
    let c = newPolyline ()
    eq "grips off: empty" 0 (rs.SelectedObjectGrips c).Count
    rs.EnableObjectGrips c |> ignore
    eq "none selected: empty" 0 (rs.SelectedObjectGrips c).Count
    rs.SelectObjectGrip(c, 0) |> ignore
    rs.SelectObjectGrip(c, 3) |> ignore
    eq "selected indices" [ 0; 3 ] (rs.SelectedObjectGrips c |> Seq.sort |> List.ofSeq)
    )

skip "GetObjectGrip" "waits for the user to pick a grip in the viewport"
skip "GetObjectGrips" "waits for the user to pick grips in the viewport"

printfn "%s" (finish ())
