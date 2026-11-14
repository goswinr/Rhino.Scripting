// Tests for Src/Scripting_Selection.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Selection"

let red = Drawing.Color.FromArgb(255, 0, 0)
let sorted (ids: Guid seq) = ids |> Seq.sort |> List.ofSeq
let isSelected (id:Guid) = (doc().Objects.FindId id).IsSelected(false) <> 0

// ------------------------------- all, first, last, next -------------------------------

test "AllObjects" (fun () ->
    eq "empty document" 0 (rs.AllObjects()).Count
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.)
    let c = rs.AddPoint(5., 5., 5.)
    rs.HideObject c |> ignore
    let d = rs.AddPoint(6., 6., 6.)
    rs.LockObject d |> ignore
    eq "includes hidden and locked objects" (sorted [ a; b; c; d ]) (sorted (rs.AllObjects()))
    ok "nothing is selected by default" (not (isSelected a))
    let sel = rs.AllObjects(true)
    eq "select=true returns the same ids" 4 sel.Count
    ok "select=true selects them" (isSelected a && isSelected b)
    rs.UnselectAllObjects() |> ignore
    // lights and grips
    let light = rs.AddPointLight(pt 0. 0. 10.)
    eq "lights are excluded by default" 4 (rs.AllObjects()).Count
    eq "lights can be included" 5 (rs.AllObjects(false, true)).Count
    ok "the light is in the list" ((rs.AllObjects(false, true)).Contains light)
    )

// Rhino enumerates objects newest first: the "first" object is the last one created, the "last" object is the oldest one.
test "FirstObject" (fun () ->
    throwsRs "empty document" (fun () -> rs.FirstObject())
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    eq "the first object is the last one created" b (rs.FirstObject())
    ok "not selected by default" (not (isSelected b))
    rs.FirstObject(true) |> ignore
    ok "selected on request" (isSelected b && not (isSelected a))
    rs.UnselectAllObjects() |> ignore
    rs.AddPointLight(pt 0. 0. 10.) |> ignore
    eq "lights are not the first object by default" b (rs.FirstObject())
    )

test "LastObject" (fun () ->
    throwsRs "empty document" (fun () -> rs.LastObject())
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    eq "the last object is the oldest one" a (rs.LastObject())
    ok "not selected by default" (not (isSelected a))
    rs.LastObject(true) |> ignore
    ok "selected on request" (isSelected a && not (isSelected b))
    rs.UnselectAllObjects() |> ignore
    rs.DeleteObject a |> ignore
    eq "deleted objects are not found" b (rs.LastObject())
    )

test "NextObject" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    let c = rs.AddPoint(2., 0., 0.)
    eq "next after the first (c) is b" b (rs.NextObject c)
    eq "next after b is a" a (rs.NextObject b)
    throwsRs "there is no next object after the last" (fun () -> rs.NextObject a)
    throwsRs "unknown object" (fun () -> rs.NextObject (Guid.NewGuid()))
    rs.NextObject(c, true) |> ignore
    ok "selected on request" (isSelected b)
    )

// ------------------------------- states -------------------------------

test "NormalObjects" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let hidden = rs.AddPoint(1., 0., 0.)
    rs.HideObject hidden |> ignore
    let locked = rs.AddPoint(2., 0., 0.)
    rs.LockObject locked |> ignore
    eq "only normal (not hidden, not locked) objects" [ a ] (List.ofSeq (rs.NormalObjects()))
    let light = rs.AddPointLight(pt 0. 0. 5.)
    ok "lights are excluded by default" (not ((rs.NormalObjects()).Contains light))
    ok "lights can be included" ((rs.NormalObjects(true)).Contains light)
    )

test "LockedObjects" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let locked = rs.AddPoint(2., 0., 0.)
    rs.LockObject locked |> ignore
    eq "only locked objects" [ locked ] (List.ofSeq (rs.LockedObjects()))
    // an object on a locked layer counts as locked
    rs.AddLayer("lockedLayer") |> ignore
    let onLayer = rs.AddPoint(3., 0., 0.)
    rs.ObjectLayer(onLayer, "lockedLayer")
    rs.LayerLock "lockedLayer"
    eq "object on a locked layer" (sorted [ locked; onLayer ]) (sorted (rs.LockedObjects()))
    rs.LayerUnlock "lockedLayer"
    eq "after unlocking the layer" [ locked ] (List.ofSeq (rs.LockedObjects()))
    ok "lights can be included" ((rs.LockedObjects(true)).Count >= 1)
    )

test "HiddenObjects" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let hidden = rs.AddPoint(1., 0., 0.)
    rs.HideObject hidden |> ignore
    eq "only hidden objects" [ hidden ] (List.ofSeq (rs.HiddenObjects()))
    rs.AddLayer("hiddenLayer") |> ignore
    let onLayer = rs.AddPoint(3., 0., 0.)
    rs.ObjectLayer(onLayer, "hiddenLayer")
    rs.LayerOff "hiddenLayer"
    eq "object on an invisible layer counts as hidden" (sorted [ hidden; onLayer ]) (sorted (rs.HiddenObjects()))
    rs.LayerOn "hiddenLayer"
    eq "after turning the layer on" [ hidden ] (List.ofSeq (rs.HiddenObjects()))
    )

// ------------------------------- selecting -------------------------------

test "SelectedObjects" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    eq "nothing selected" 0 (rs.SelectedObjects()).Count
    rs.SelectObject a |> ignore
    eq "one selected" [ a ] (List.ofSeq (rs.SelectedObjects()))
    rs.SelectObject b |> ignore
    eq "two selected" (sorted [ a; b ]) (sorted (rs.SelectedObjects()))
    let light = rs.AddPointLight(pt 0. 0. 5.)
    rs.SelectObject light |> ignore
    ok "lights are excluded by default" (not ((rs.SelectedObjects()).Contains light))
    ok "lights can be included" ((rs.SelectedObjects(true)).Contains light)
    )

test "UnselectAllObjects" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    eq "nothing to unselect" 0 (rs.UnselectAllObjects())
    rs.SelectObjects [ a; b ] |> ignore
    eq "number of unselected objects" 2 (rs.UnselectAllObjects())
    eq "nothing is selected afterwards" 0 (rs.SelectedObjects()).Count
    )

test "InvertSelectedObjects" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    let c = rs.AddPoint(2., 0., 0.)
    rs.SelectObject a |> ignore
    let newlySelected = rs.InvertSelectedObjects()
    eq "the previously unselected objects are returned" (sorted [ b; c ]) (sorted newlySelected)
    eq "they are selected now" (sorted [ b; c ]) (sorted (rs.SelectedObjects()))
    ok "the old selection is deselected" (not (isSelected a))
    // objects that can not be selected stay unselected
    rs.UnselectAllObjects() |> ignore
    rs.LockObject c |> ignore
    let inv = rs.InvertSelectedObjects()
    ok "locked objects are not selected" (not (inv.Contains c))
    eq "unlocked objects are selected" (sorted [ a; b ]) (sorted inv)
    )

test "LastCreatedObjects" (fun () ->
    // The list depends on the Rhino command that is running, so only the shape of the result can be tested
    let r = rs.LastCreatedObjects()
    ok "returns a list" (not (isNull r))
    let r2 = rs.LastCreatedObjects(true)
    ok "with select returns a list too" (not (isNull r2))
    for id in r2 do ok "listed objects exist" (not (isNull (doc().Objects.FindId id)))
    )

// ------------------------------- by property -------------------------------

test "ObjectsByColor" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    let c = rs.AddPoint(2., 0., 0.)
    rs.ObjectColor(a, red) |> ignore
    rs.ObjectColor(b, red) |> ignore
    eq "objects with the color" (sorted [ a; b ]) (sorted (rs.ObjectsByColor red))
    eq "color without objects" 0 (rs.ObjectsByColor(Drawing.Color.FromArgb(1, 2, 3))).Count
    ok "not selected by default" (not (isSelected a))
    rs.ObjectsByColor(red, true) |> ignore
    ok "selected on request" (isSelected a && isSelected b && not (isSelected c))
    )

test "ObjectsByGroup" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    let c = rs.AddPoint(2., 0., 0.)
    rs.GroupObjects([ a; b ], "sgA")
    eq "members of the group" (sorted [ a; b ]) (sorted (rs.ObjectsByGroup "sgA"))
    ok "not selected by default" (not (isSelected a))
    rs.ObjectsByGroup("sgA", true) |> ignore
    ok "selected on request" (isSelected a && isSelected b && not (isSelected c))
    rs.AddGroup "sgEmpty" |> ignore
    eq "empty group" 0 (rs.ObjectsByGroup "sgEmpty").Count
    throwsRs "unknown group" (fun () -> rs.ObjectsByGroup "sgMissing")
    )

test "ObjectsByLayer" (fun () ->
    rs.AddLayer("sl1") |> ignore
    rs.AddLayer("sl2") |> ignore
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    let c = rs.AddPoint(2., 0., 0.)
    rs.ObjectLayer([ a; b ], "sl1")
    rs.ObjectLayer(c, "sl2")
    eq "objects of the layer" (sorted [ a; b ]) (sorted (rs.ObjectsByLayer "sl1"))
    ok "not selected by default" (not (isSelected a))
    rs.ObjectsByLayer("sl1", true) |> ignore
    ok "selected on request" (isSelected a && isSelected b && not (isSelected c))
    rs.AddLayer("slEmpty") |> ignore
    eq "empty layer" 0 (rs.ObjectsByLayer "slEmpty").Count
    throwsRs "unknown layer" (fun () -> rs.ObjectsByLayer "slMissing")
    )

test "ObjectsByName" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    let c = rs.AddPoint(2., 0., 0.)
    rs.ObjectName(a, "snA")
    rs.ObjectName(b, "snA")
    rs.ObjectName(c, "snOther")
    eq "objects with the name" (sorted [ a; b ]) (sorted (rs.ObjectsByName "snA"))
    eq "wildcards are supported" (sorted [ a; b; c ]) (sorted (rs.ObjectsByName "sn*"))
    eq "no match" 0 (rs.ObjectsByName "snMissing").Count
    rs.HideObject a |> ignore
    ok "hidden objects are found" ((rs.ObjectsByName "snA").Contains a)
    ok "not selected by default" (not (isSelected b))
    rs.ObjectsByName("snOther", true) |> ignore
    ok "selected on request" (isSelected c)
    let light = rs.AddPointLight(pt 0. 0. 5.)
    rs.ObjectName(light, "snLight")
    eq "lights are not found by default" 0 (rs.ObjectsByName "snLight").Count
    eq "lights can be included" [ light ] (List.ofSeq (rs.ObjectsByName("snLight", false, true)))
    )

test "ObjectsByType" (fun () ->
    let point = rs.AddPoint(0., 0., 0.)
    let curve = rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.)
    let srf = addPlaneSurface 2. 2.
    let poly = addBox 1. 1. 1.
    let mesh = addMeshBox 1. 1. 1.
    let dot = rs.AddTextDot("dot", pt 5. 5. 5.)
    let hidden = rs.AddPoint(9., 9., 9.)
    rs.HideObject hidden |> ignore
    eq "points (1)" (sorted [ point; hidden ]) (sorted (rs.ObjectsByType 1))
    eq "curves (4)" [ curve ] (List.ofSeq (rs.ObjectsByType 4))
    eq "surface (8) is a single face brep" [ srf ] (List.ofSeq (rs.ObjectsByType 8))
    eq "polysurface (16)" [ poly ] (List.ofSeq (rs.ObjectsByType 16))
    eq "mesh (32)" [ mesh ] (List.ofSeq (rs.ObjectsByType 32))
    eq "text dot (8192)" [ dot ] (List.ofSeq (rs.ObjectsByType 8192))
    eq "points and curves added together (5)" (sorted [ point; hidden; curve ]) (sorted (rs.ObjectsByType 5))
    eq "type 0 gives everything" 7 (rs.ObjectsByType 0).Count
    // state
    eq "normal points only (state 1)" [ point ] (List.ofSeq (rs.ObjectsByType(1, false, 1)))
    eq "hidden points only (state 4)" [ hidden ] (List.ofSeq (rs.ObjectsByType(1, false, 4)))
    eq "locked points only (state 2)" 0 (rs.ObjectsByType(1, false, 2)).Count
    rs.LockObject point |> ignore
    eq "locked points (state 2)" [ point ] (List.ofSeq (rs.ObjectsByType(1, false, 2)))
    eq "normal and locked points (state 3)" [ point ] (List.ofSeq (rs.ObjectsByType(1, false, 3)))
    // select
    rs.UnlockObject point |> ignore
    ok "not selected by default" (not (isSelected curve))
    rs.ObjectsByType(4, true) |> ignore
    ok "selected on request" (isSelected curve)
    // extrusion
    let ex = rs.AddCircle(Plane.WorldXY, 1.)
    let light = rs.AddPointLight(pt 0. 0. 5.)
    eq "lights (256)" [ light ] (List.ofSeq (rs.ObjectsByType 256))
    )

test "VisibleObjectsInView" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let far = rs.AddPoint(1e6, 1e6, 0.)
    let hidden = rs.AddPoint(1., 0., 0.)
    rs.HideObject hidden |> ignore
    rs.ZoomExtents("Top", false) |> ignore
    let visible = rs.VisibleObjectsInView "Top"
    ok "a visible object is listed" (visible.Contains a)
    ok "a hidden object is not listed" (not (visible.Contains hidden))
    ok "active view can be used" ((rs.VisibleObjectsInView()).Count >= 0)
    ok "not selected by default" (not (isSelected a))
    rs.VisibleObjectsInView("Top", true) |> ignore
    ok "selected on request" (isSelected a)
    throws "unknown view" (fun () -> rs.VisibleObjectsInView "no such view")
    )

test "WindowPick" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(100., 100., 0.)
    rs.ZoomExtents("Top", false) |> ignore
    let all = rs.WindowPick(pt -50. -50. 0., pt 150. 150. 0., "Top")
    ok "window around both objects picks both" (all.Contains a && all.Contains b)
    let some = rs.WindowPick(pt -10. -10. 0., pt 10. 10. 0., "Top")
    ok "small window picks only the first" (some.Contains a && not (some.Contains b))
    let none = rs.WindowPick(pt 40. 40. 0., pt 60. 60. 0., "Top")
    eq "empty window picks nothing" 0 none.Count
    let crossing = rs.WindowPick(pt 50. 50. 0., pt 200. 200. 0., "Top", false, false)
    ok "crossing selection picks objects inside" (crossing.Contains b)
    ok "not selected by default" (not (isSelected a))
    rs.WindowPick(pt -10. -10. 0., pt 10. 10. 0., "Top", true) |> ignore
    ok "selected on request" (isSelected a)
    throws "unknown view" (fun () -> rs.WindowPick(pt 0. 0. 0., pt 1. 1. 0., "no such view"))
    )

skip "GetCurveObject" "waits for the user to pick in the viewport"
skip "GetObjectRef" "waits for the user to pick in the viewport"
skip "GetObject" "waits for the user to pick in the viewport"
skip "GetObjectEx" "waits for the user to pick in the viewport"
skip "GetObjects" "waits for the user to pick in the viewport"
skip "GetObjectsEx" "waits for the user to pick in the viewport"
skip "GetPointCoordinates" "waits for the user to pick in the viewport"
skip "GetSurfaceObject" "waits for the user to pick in the viewport"

printfn "%s" (finish ())
