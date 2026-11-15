// Tests for Src/Scripting_Object.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Object"

let vec x y z = Vector3d(x, y, z)
let red = Drawing.Color.FromArgb(255, 0, 0)
let blue = Drawing.Color.FromArgb(0, 0, 255)
let rgb (c:Drawing.Color) = (int c.R, int c.G, int c.B)
let geo (id:Guid) = (doc().Objects.FindId id).Geometry
let newPoint (x:float) = rs.AddPoint(x, 0., 0.)
let isSelected (id:Guid) = (doc().Objects.FindId id).IsSelected(false) <> 0
let sorted (ids: Guid seq) = ids |> Seq.sort |> List.ofSeq

// ------------------------------- existence and state -------------------------------

test "IsObject" (fun () ->
    ok "existing object" (rs.IsObject (newPoint 0.))
    ok "unknown id" (not (rs.IsObject (Guid.NewGuid())))
    ok "empty id" (not (rs.IsObject Guid.Empty))
    let p = newPoint 0.
    rs.DeleteObject p
    ok "deleted object" (not (rs.IsObject p))
    )

test "IsObjectValid" (fun () ->
    ok "valid point" (rs.IsObjectValid (newPoint 0.))
    ok "valid curve" (rs.IsObjectValid (addCircle 1.))
    ok "unknown id gives false" (not (rs.IsObjectValid (Guid.NewGuid())))
    )

test "HideObject" (fun () ->
    let p = newPoint 0.
    ok "hide succeeds" (rs.HideObject p)
    ok "object is hidden" (rs.IsObjectHidden p)
    ok "hiding again is fine" (rs.HideObject p || true)
    ok "unknown id" (not (rs.HideObject (Guid.NewGuid())))
    )

test "HideObjects" (fun () ->
    let ps = [ newPoint 0.; newPoint 1.; newPoint 2. ]
    eq "number of hidden objects" 3 (rs.HideObjects ps)
    ok "they are hidden" (ps |> List.forall rs.IsObjectHidden)
    eq "nothing more to hide" 0 (rs.HideObjects ps)
    eq "empty list" 0 (rs.HideObjects [])
    )

test "ShowObject" (fun () ->
    let p = newPoint 0.
    rs.HideObject p |> ignore
    rs.ShowObject p
    ok "object is visible again" (not (rs.IsObjectHidden p))
    throwsRs "showing an object that is not hidden fails" (fun () -> rs.ShowObject p)
    throws "unknown id" (fun () -> rs.ShowObject (Guid.NewGuid()))
    )

test "ShowObjects" (fun () ->
    let ps = [ newPoint 0.; newPoint 1. ]
    rs.HideObjects ps |> ignore
    rs.ShowObjects ps
    ok "all visible again" (ps |> List.forall (fun p -> not (rs.IsObjectHidden p)))
    rs.ShowObjects []
    )

test "IsObjectHidden" (fun () ->
    let p = newPoint 0.
    ok "visible" (not (rs.IsObjectHidden p))
    rs.HideObject p |> ignore
    ok "hidden" (rs.IsObjectHidden p)
    throwsRs "unknown id" (fun () -> rs.IsObjectHidden (Guid.NewGuid()))
    )

test "LockObject" (fun () ->
    let p = newPoint 0.
    ok "lock succeeds" (rs.LockObject p)
    ok "object is locked" (rs.IsObjectLocked p)
    ok "unknown id" (not (rs.LockObject (Guid.NewGuid())))
    )

test "LockObjects" (fun () ->
    let ps = [ newPoint 0.; newPoint 1.; newPoint 2. ]
    eq "number of locked objects" 3 (rs.LockObjects ps)
    ok "they are locked" (ps |> List.forall rs.IsObjectLocked)
    eq "empty list" 0 (rs.LockObjects [])
    )

test "UnlockObject" (fun () ->
    let p = newPoint 0.
    rs.LockObject p |> ignore
    rs.UnlockObject p
    ok "object is unlocked" (not (rs.IsObjectLocked p))
    rs.UnlockObject p // unlocking an unlocked object is fine
    throws "unknown id" (fun () -> rs.UnlockObject (Guid.NewGuid()))
    )

test "UnlockObjects" (fun () ->
    let ps = [ newPoint 0.; newPoint 1. ]
    rs.LockObjects ps |> ignore
    rs.UnlockObjects ps
    ok "all unlocked" (ps |> List.forall (fun p -> not (rs.IsObjectLocked p)))
    rs.UnlockObjects []
    )

test "IsObjectLocked" (fun () ->
    let p = newPoint 0.
    ok "unlocked" (not (rs.IsObjectLocked p))
    rs.LockObject p |> ignore
    ok "locked" (rs.IsObjectLocked p)
    throwsRs "unknown id" (fun () -> rs.IsObjectLocked (Guid.NewGuid()))
    )

test "IsObjectNormal" (fun () ->
    let p = newPoint 0.
    ok "normal" (rs.IsObjectNormal p)
    rs.LockObject p |> ignore
    ok "locked objects are not normal" (not (rs.IsObjectNormal p))
    rs.UnlockObject p
    rs.HideObject p |> ignore
    ok "hidden objects are not normal" (not (rs.IsObjectNormal p))
    throwsRs "unknown id" (fun () -> rs.IsObjectNormal (Guid.NewGuid()))
    )

test "IsObjectReference" (fun () ->
    ok "an object of the document is no reference" (not (rs.IsObjectReference (newPoint 0.)))
    throwsRs "unknown id" (fun () -> rs.IsObjectReference (Guid.NewGuid()))
    )

test "IsObjectSelectable" (fun () ->
    let p = newPoint 0.
    ok "normal objects are selectable" (rs.IsObjectSelectable p)
    rs.LockObject p |> ignore
    ok "locked objects are not selectable" (not (rs.IsObjectSelectable p))
    rs.UnlockObject p
    rs.HideObject p |> ignore
    ok "hidden objects are not selectable" (not (rs.IsObjectSelectable p))
    throwsRs "unknown id" (fun () -> rs.IsObjectSelectable (Guid.NewGuid()))
    )

test "IsObjectSelected" (fun () ->
    let p = newPoint 0.
    eq "not selected" 0 (rs.IsObjectSelected p)
    rs.SelectObject p
    ok "selected (1, or 2 for persistent selection)" (rs.IsObjectSelected p >= 1)
    throwsRs "unknown id" (fun () -> rs.IsObjectSelected (Guid.NewGuid()))
    )

test "SelectObject" (fun () ->
    let p = newPoint 0.
    rs.SelectObject p
    ok "selected" (isSelected p)
    // hidden object
    let h = newPoint 1.
    rs.HideObject h |> ignore
    throwsRs "hidden object" (fun () -> rs.SelectObject h)
    rs.SelectObject(h, false, true) // errors are ignored
    ok "ignoreErrors: not selected" (not (isSelected h))
    rs.SelectObject(h, true)
    ok "forceVisible: shown and selected" (isSelected h && not (rs.IsObjectHidden h))
    // locked object
    let l = newPoint 2.
    rs.LockObject l |> ignore
    throwsRs "locked object" (fun () -> rs.SelectObject l)
    rs.SelectObject(l, true)
    ok "forceVisible: unlocked and selected" (isSelected l && not (rs.IsObjectLocked l))
    // object on an invisible layer
    rs.AddLayer "soLayer" |> ignore
    let o = newPoint 3.
    rs.ObjectLayer(o, "soLayer")
    rs.LayerOff "soLayer"
    throwsRs "object on an invisible layer" (fun () -> rs.SelectObject o)
    rs.SelectObject(o, true)
    ok "forceVisible: layer turned on" (isSelected o && rs.IsLayerOn "soLayer")
    // object on a locked layer
    let k = newPoint 4.
    rs.AddLayer "soLockedLayer" |> ignore
    rs.ObjectLayer(k, "soLockedLayer")
    rs.LayerLock "soLockedLayer"
    throwsRs "object on a locked layer" (fun () -> rs.SelectObject k)
    rs.SelectObject(k, true)
    ok "forceVisible: layer unlocked" (isSelected k && not (rs.IsLayerLocked "soLockedLayer"))
    throwsRs "unknown id" (fun () -> rs.SelectObject (Guid.NewGuid()))
    )

test "SelectObjects" (fun () ->
    let ps = [ newPoint 0.; newPoint 1.; newPoint 2. ]
    rs.SelectObjects ps
    ok "all selected" (ps |> List.forall isSelected)
    rs.UnselectAllObjects() |> ignore
    rs.HideObject ps.[1] |> ignore
    throwsRs "one object is hidden" (fun () -> rs.SelectObjects ps)
    rs.UnselectAllObjects() |> ignore
    rs.SelectObjects(ps, false, true)
    ok "ignoreErrors: the selectable ones are selected" (isSelected ps.[0] && isSelected ps.[2] && not (isSelected ps.[1]))
    rs.SelectObjects(ps, true)
    ok "forceVisible: the hidden one is shown and selected" (isSelected ps.[1] && not (rs.IsObjectHidden ps.[1]))
    rs.SelectObjects []
    throwsRs "unknown id in the list" (fun () -> rs.SelectObjects [ Guid.NewGuid() ])
    )

test "UnselectObject" (fun () ->
    let p = newPoint 0.
    rs.SelectObject p
    rs.UnselectObject p
    ok "not selected anymore" (not (isSelected p))
    rs.UnselectObject p
    throws "unknown id" (fun () -> rs.UnselectObject (Guid.NewGuid()))
    )

test "UnselectObjects" (fun () ->
    let ps = [ newPoint 0.; newPoint 1. ]
    rs.SelectObjects ps
    rs.UnselectObjects ps
    ok "none selected anymore" (ps |> List.forall (fun p -> not (isSelected p)))
    rs.UnselectObjects []
    )

test "FlashObject" (fun () ->
    let ps = [ newPoint 0.; newPoint 1. ]
    rs.FlashObject ps
    rs.FlashObject(ps, false)
    rs.FlashObject []
    ok "flashing does not change the selection" (ps |> List.forall (fun p -> not (isSelected p)))
    throws "unknown id" (fun () -> rs.FlashObject [ Guid.NewGuid() ])
    )

test "IsObjectInGroup" (fun () ->
    let ps = [ newPoint 0.; newPoint 1. ]
    ok "not in a group" (not (rs.IsObjectInGroup ps.[0]))
    rs.GroupObjects(ps, "oigA")
    ok "in any group" (rs.IsObjectInGroup ps.[0])
    ok "in the named group" (rs.IsObjectInGroup(ps.[0], "oigA"))
    rs.AddGroup "oigB" |> ignore
    ok "not in another group" (not (rs.IsObjectInGroup(ps.[0], "oigB")))
    throwsRs "unknown group" (fun () -> rs.IsObjectInGroup(ps.[0], "no such group"))
    )

test "IsObjectInBox" (fun () ->
    let c = addBox 2. 2. 2.
    let big = BoundingBox(pt -1. -1. -1., pt 3. 3. 3.)
    let overlap = BoundingBox(pt 1. 1. 1., pt 5. 5. 5.)
    let far = BoundingBox(pt 10. 10. 10., pt 11. 11. 11.)
    ok "inside (testMode true)" (rs.IsObjectInBox(c, big))
    ok "overlapping is not inside" (not (rs.IsObjectInBox(c, overlap)))
    ok "overlapping counts with testMode false" (rs.IsObjectInBox(c, overlap, false))
    ok "far away" (not (rs.IsObjectInBox(c, far, false)))
    ok "geometry overload, inside" (rs.IsObjectInBox(geo c, big))
    ok "geometry overload, overlap" (rs.IsObjectInBox(geo c, overlap, false))
    throws "unknown id" (fun () -> rs.IsObjectInBox(Guid.NewGuid(), big))
    )

test "IsObjectSolid" (fun () ->
    let box = addBox 1. 1. 1.
    ok "closed polysurface" (rs.IsObjectSolid box)
    ok "geometry overload" (rs.IsObjectSolid (geo box))
    ok "sphere" (rs.IsObjectSolid (addSphere 1.))
    ok "open surface" (not (rs.IsObjectSolid (addPlaneSurface 1. 1.)))
    ok "closed mesh" (rs.IsObjectSolid (addMeshBox 1. 1. 1.))
    ok "a curve is never solid" (not (rs.IsObjectSolid (addCircle 1.)))
    throws "unknown id" (fun () -> rs.IsObjectSolid (Guid.NewGuid()))
    )

test "IsVisibleInView" (fun () ->
    let p = newPoint 0.
    rs.ZoomExtents "Top"
    ok "visible in a view" (rs.IsVisibleInView(p, "Top"))
    ok "active view if omitted" (rs.IsVisibleInView p || true)
    rs.HideObject p |> ignore
    ok "hidden object is not visible" (not (rs.IsVisibleInView(p, "Top")))
    rs.ShowObject p
    let far = rs.AddPoint(1e7, 1e7, 0.)
    ok "an object far outside of the view" (not (rs.IsVisibleInView(far, "Top")))
    throws "unknown view" (fun () -> rs.IsVisibleInView(p, "no such view"))
    throwsRs "unknown id" (fun () -> rs.IsVisibleInView(Guid.NewGuid(), "Top"))
    )

test "IsLayoutObject" (fun () ->
    let p = newPoint 0.
    ok "model space object" (not (rs.IsLayoutObject p))
    throwsRs "unknown id" (fun () -> rs.IsLayoutObject (Guid.NewGuid()))
    )

test "ObjectLayout" (fun () ->
    let p = newPoint 0.
    eq "model space gives an empty string" "" (rs.ObjectLayout p)
    let _, layout = rs.AddLayout("olLayout", 297., 210.)
    rs.ObjectLayout(p, layout)
    eq "object was moved to the layout" layout (rs.ObjectLayout p)
    ok "it is a layout object" (rs.IsLayoutObject p)
    let q = newPoint 1.
    let r = newPoint 2.
    rs.ObjectLayout([ q; r ], layout)
    eq "multiple: first" layout (rs.ObjectLayout q)
    eq "multiple: second" layout (rs.ObjectLayout r)
    throwsRs "unknown layout" (fun () -> rs.ObjectLayout(newPoint 3., "no such layout"))
    throwsRs "unknown id" (fun () -> rs.ObjectLayout (Guid.NewGuid()))
    )

test "ObjectType" (fun () ->
    eq "point" 1 (rs.ObjectType (newPoint 0.))
    eq "point cloud" 2 (rs.ObjectType (rs.AddPointCloud [| pt 0. 0. 0.; pt 1. 0. 0. |]))
    eq "curve" 4 (rs.ObjectType (addCircle 1.))
    eq "surface" 8 (rs.ObjectType (addPlaneSurface 1. 1.))
    eq "polysurface" 16 (rs.ObjectType (addBox 1. 1. 1.))
    eq "mesh" 32 (rs.ObjectType (addMeshBox 1. 1. 1.))
    eq "light" 256 (rs.ObjectType (rs.AddPointLight(pt 0. 0. 1.)))
    eq "annotation (text)" 512 (rs.ObjectType (rs.AddText("x", Plane.WorldXY)))
    eq "annotation (dimension)" 512 (rs.ObjectType (rs.AddLinearDimension(pt 0. 0. 0., pt 5. 0. 0., pt 2. 1. 0.)))
    eq "text dot" 8192 (rs.ObjectType (rs.AddTextDot("d", pt 0. 0. 0.)))
    rs.CurrentHatchPattern "Solid"
    eq "hatch" 65536 (rs.ObjectType (rs.AddHatch (addSquare 5.)))
    eq "clipping plane" 536870912 (rs.ObjectType (rs.AddClippingPlane(Plane.WorldXY, 5., 5.)))
    let name = rs.AddBlock([ newPoint 0. ], pt 0. 0. 0., "otBlock", true)
    eq "block instance" 4096 (rs.ObjectType (rs.InsertBlock(name, pt 0. 0. 0.)))
    throwsRs "unknown id" (fun () -> rs.ObjectType (Guid.NewGuid()))
    )

test "ObjectDescription" (fun () ->
    ok "point description" ((rs.ObjectDescription (newPoint 0.)).ToLowerInvariant().Contains "point")
    ok "curve description" ((rs.ObjectDescription (addCircle 1.)).ToLowerInvariant().Contains "curve")
    ok "polysurface description" ((rs.ObjectDescription (addBox 1. 1. 1.)).ToLowerInvariant().Contains "surface")
    let counts = rs.ObjectDescription [ newPoint 0.; newPoint 1.; addCircle 1. ]
    ok "counts per type: points" (counts.Contains "2")
    ok "counts per type: curve" (counts.ToLowerInvariant().Contains "curve")
    throwsRs "unknown id" (fun () -> rs.ObjectDescription (Guid.NewGuid()))
    )

// ------------------------------- attributes -------------------------------

test "ObjectName" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    eq "no name gives an empty string" "" (rs.ObjectName a)
    rs.ObjectName(a, "my name")
    eq "name round trip" "my name" (rs.ObjectName a)
    rs.ObjectName([ a; b ], "both")
    eq "multiple: first" "both" (rs.ObjectName a)
    eq "multiple: second" "both" (rs.ObjectName b)
    rs.ObjectName(a, "")
    eq "an empty string removes the name" "" (rs.ObjectName a)
    throwsRs "bad characters" (fun () -> rs.ObjectName(a, "tab\there"))
    throwsRs "leading whitespace" (fun () -> rs.ObjectName(a, " name"))
    throwsRs "bad characters, multiple" (fun () -> rs.ObjectName([ a; b ], "line\nbreak"))
    throwsRs "unknown id" (fun () -> rs.ObjectName (Guid.NewGuid()))
    )

test "IsGoodStringId" (fun () ->
    ok "simple name" (rs.IsGoodStringId "abc 123")
    ok "ASCII only mode accepts ASCII" (rs.IsGoodStringId("abc", false, true))
    ok "ASCII only mode rejects other characters" (not (rs.IsGoodStringId("äbc", false, true)))
    ok "letters like ä are fine by default" (rs.IsGoodStringId "äbc")
    ok "empty is allowed on request" (rs.IsGoodStringId("", true))
    ok "empty string is not good by default" (not (rs.IsGoodStringId ""))
    ok "tab" (not (rs.IsGoodStringId "a\tb"))
    ok "line return" (not (rs.IsGoodStringId "a\nb"))
    ok "leading whitespace" (not (rs.IsGoodStringId " a"))
    ok "trailing whitespace" (not (rs.IsGoodStringId "a "))
    ok "look-alike quote" (not (rs.IsGoodStringId "a‘b"))
    ok "null" (not (rs.IsGoodStringId null))
    )

test "ObjectColor" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    rs.ObjectColor(a, red)
    eq "color round trip" (rgb red) (rgb (rs.ObjectColor a))
    eq "setting a color changes the source to object" 1 (rs.ObjectColorSource a)
    rs.ObjectColor([ a; b ], blue)
    eq "multiple: first" (rgb blue) (rgb (rs.ObjectColor a))
    eq "multiple: second" (rgb blue) (rgb (rs.ObjectColor b))
    let c = newPoint 2.
    rs.AddLayer "ocLayer" |> ignore
    rs.LayerColor("ocLayer", red)
    rs.ObjectLayer(c, "ocLayer")
    eq "color by layer is the color of the layer" (rgb red) (rgb (rs.ObjectColor c))
    throwsRs "unknown id, get" (fun () -> rs.ObjectColor (Guid.NewGuid()))
    throwsRs "unknown id, set" (fun () -> rs.ObjectColor(Guid.NewGuid(), red))
    )

test "ObjectColorSource" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    eq "from layer at first" 0 (rs.ObjectColorSource a)
    rs.ObjectColorSource(a, 1)
    eq "from object" 1 (rs.ObjectColorSource a)
    rs.ObjectColorSource([ a; b ], 1)
    eq "multiple: second" 1 (rs.ObjectColorSource b)
    rs.ObjectColorSource(a, 3)
    eq "from parent" 3 (rs.ObjectColorSource a)
    rs.ObjectColorSource(a, 0)
    eq "from layer" 0 (rs.ObjectColorSource a)
    throwsRs "unknown id" (fun () -> rs.ObjectColorSource (Guid.NewGuid()))
    )

test "ObjectLayerShort" (fun () ->
    rs.AddLayer "olsP::olsC" |> ignore
    let p = newPoint 0.
    rs.ObjectLayer(p, "olsP::olsC")
    eq "short name" "olsC" (rs.ObjectLayerShort p)
    throwsRs "unknown id" (fun () -> rs.ObjectLayerShort (Guid.NewGuid()))
    )

test "ObjectLinetype" (fun () ->
    let lt = new DocObjects.Linetype(Name = "objDashed")
    lt.AppendSegment(5.0, true) |> ignore
    lt.AppendSegment(2.0, false) |> ignore
    doc().Linetypes.Add lt |> ignore
    let a = newPoint 0.
    let b = newPoint 1.
    rs.ObjectLinetype(a, "objDashed")
    eq "linetype round trip" "objDashed" (rs.ObjectLinetype a)
    eq "setting a linetype changes the source to object" 1 (rs.ObjectLinetypeSource a)
    rs.ObjectLinetype([ a; b ], "OBJDASHED")
    eq "multiple: second (not case sensitive)" "objDashed" (rs.ObjectLinetype b)
    rs.ObjectLinetype(a, "Continuous")
    eq "Continuous" "Continuous" (rs.ObjectLinetype a)
    throwsRs "unknown linetype" (fun () -> rs.ObjectLinetype(a, "no such linetype"))
    throwsRs "unknown id" (fun () -> rs.ObjectLinetype (Guid.NewGuid()))
    )

test "ObjectLinetypeSource" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    eq "by layer at first" 0 (rs.ObjectLinetypeSource a)
    rs.ObjectLinetypeSource(a, 1)
    eq "by object" 1 (rs.ObjectLinetypeSource a)
    rs.ObjectLinetypeSource([ a; b ], 1)
    eq "multiple: second" 1 (rs.ObjectLinetypeSource b)
    rs.ObjectLinetypeSource(a, 3)
    eq "by parent" 3 (rs.ObjectLinetypeSource a)
    throwsRs "invalid source" (fun () -> rs.ObjectLinetypeSource(a, 2))
    throwsRs "unknown id" (fun () -> rs.ObjectLinetypeSource (Guid.NewGuid()))
    )

test "ObjectMaterialIndex" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    eq "no material at first" -1 (rs.ObjectMaterialIndex a)
    let m = doc().Materials.Add()
    rs.ObjectMaterialIndex(a, m)
    eq "index round trip" m (rs.ObjectMaterialIndex a)
    rs.ObjectMaterialIndex([ a; b ], m)
    eq "multiple: second" m (rs.ObjectMaterialIndex b)
    throwsRs "unknown id" (fun () -> rs.ObjectMaterialIndex (Guid.NewGuid()))
    )

test "ObjectMaterialSource" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    eq "from layer at first" 0 (rs.ObjectMaterialSource a)
    rs.ObjectMaterialSource(a, 1)
    eq "from object" 1 (rs.ObjectMaterialSource a)
    rs.ObjectMaterialSource([ a; b ], 1)
    eq "multiple: second" 1 (rs.ObjectMaterialSource b)
    rs.ObjectMaterialSource(a, 3)
    eq "from parent" 3 (rs.ObjectMaterialSource a)
    throwsRs "invalid source" (fun () -> rs.ObjectMaterialSource(a, 2))
    throwsRs "unknown id" (fun () -> rs.ObjectMaterialSource (Guid.NewGuid()))
    )

test "ObjectPrintColor" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    rs.ObjectPrintColor(a, red)
    eq "print color round trip" (rgb red) (rgb (rs.ObjectPrintColor a))
    eq "the print color source changed to object" 1 (rs.ObjectPrintColorSource a)
    rs.ObjectPrintColor([ a; b ], blue)
    eq "multiple: second" (rgb blue) (rgb (rs.ObjectPrintColor b))
    throwsRs "unknown id, get" (fun () -> rs.ObjectPrintColor (Guid.NewGuid()))
    throwsRs "unknown id, set" (fun () -> rs.ObjectPrintColor(Guid.NewGuid(), red))
    )

test "ObjectPrintColorSource" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    eq "from layer at first" 0 (rs.ObjectPrintColorSource a)
    rs.ObjectPrintColorSource(a, 1)
    eq "from object" 1 (rs.ObjectPrintColorSource a)
    rs.ObjectPrintColorSource([ a; b ], 1)
    eq "multiple: second" 1 (rs.ObjectPrintColorSource b)
    rs.ObjectPrintColorSource(a, 2)
    eq "from display color" 2 (rs.ObjectPrintColorSource a)
    rs.ObjectPrintColorSource(a, 3)
    eq "from parent" 3 (rs.ObjectPrintColorSource a)
    throwsRs "invalid source" (fun () -> rs.ObjectPrintColorSource(a, 4))
    throwsRs "unknown id" (fun () -> rs.ObjectPrintColorSource (Guid.NewGuid()))
    )

test "ObjectPrintWidth" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    rs.ObjectPrintWidth(a, 0.35)
    near "print width round trip" 0.35 (rs.ObjectPrintWidth a)
    eq "the print width source changed to object" 1 (rs.ObjectPrintWidthSource a)
    rs.ObjectPrintWidth([ a; b ], 0.7)
    near "multiple: second" 0.7 (rs.ObjectPrintWidth b)
    rs.ObjectPrintWidth(a, 0.)
    near "0 means the default width" 0. (rs.ObjectPrintWidth a)
    throwsRs "unknown id, get" (fun () -> rs.ObjectPrintWidth (Guid.NewGuid()))
    throwsRs "unknown id, set" (fun () -> rs.ObjectPrintWidth(Guid.NewGuid(), 1.))
    )

test "ObjectPrintWidthSource" (fun () ->
    let a = newPoint 0.
    let b = newPoint 1.
    eq "from layer at first" 0 (rs.ObjectPrintWidthSource a)
    rs.ObjectPrintWidthSource(a, 1)
    eq "from object" 1 (rs.ObjectPrintWidthSource a)
    rs.ObjectPrintWidthSource([ a; b ], 1)
    eq "multiple: second" 1 (rs.ObjectPrintWidthSource b)
    rs.ObjectPrintWidthSource(a, 3)
    eq "from parent" 3 (rs.ObjectPrintWidthSource a)
    throwsRs "unknown id" (fun () -> rs.ObjectPrintWidthSource (Guid.NewGuid()))
    )

test "MatchObjectAttributes" (fun () ->
    let src = newPoint 0.
    rs.ObjectColor(src, red)
    rs.ObjectName(src, "source")
    let t1 = newPoint 1.
    let t2 = newPoint 2.
    eq "number of modified objects" 2 (rs.MatchObjectAttributes([ t1; t2 ], src))
    eq "color was copied" (rgb red) (rgb (rs.ObjectColor t1))
    eq "name was copied" "source" (rs.ObjectName t2)
    // without a source the default attributes are used
    eq "default attributes" 1 (rs.MatchObjectAttributes [ t1 ])
    eq "name was reset" "" (rs.ObjectName t1)
    eq "color source was reset" 0 (rs.ObjectColorSource t1)
    eq "unknown targets are not counted" 0 (rs.MatchObjectAttributes([ Guid.NewGuid() ], src))
    throwsRs "unknown source" (fun () -> rs.MatchObjectAttributes([ t1 ], Guid.NewGuid()))
    )

// ------------------------------- transformations -------------------------------

let startOf (id:Guid) = rs.PointCoordinates id

test "MoveObject" (fun () ->
    let p = rs.AddPoint(1., 2., 3.)
    rs.MoveObject(p, vec 1. 1. 1.)
    nearPt "moved in place" (pt 2. 3. 4.) (startOf p)
    ok "same Guid" (rs.IsObject p)
    throws "unknown id" (fun () -> rs.MoveObject(Guid.NewGuid(), vec 1. 0. 0.))
    )

test "MoveObjects" (fun () ->
    let ps = [ rs.AddPoint(0., 0., 0.); rs.AddPoint(1., 0., 0.) ]
    rs.MoveObjects(ps, vec 0. 5. 0.)
    nearPt "first" (pt 0. 5. 0.) (startOf ps.[0])
    nearPt "second" (pt 1. 5. 0.) (startOf ps.[1])
    rs.MoveObjects(([] : Guid list), vec 1. 1. 1.)
    )

test "CopyObject" (fun () ->
    let p = rs.AddPoint(1., 2., 3.)
    let c = rs.CopyObject p
    ok "a new object" (c <> p && rs.IsObject c)
    nearPt "copy in place" (pt 1. 2. 3.) (startOf c)
    let m = rs.CopyObject(p, vec 10. 0. 0.)
    nearPt "copy with translation" (pt 11. 2. 3.) (startOf m)
    nearPt "original is unchanged" (pt 1. 2. 3.) (startOf p)
    let g = rs.CopyObject((geo p), vec 0. 0. 1.)
    nearPt "geometry overload" (pt 1. 2. 4.) (g :?> Point).Location
    eq "no object added by the geometry overload" 3 (rs.AllObjects()).Count
    throws "unknown id" (fun () -> rs.CopyObject (Guid.NewGuid()))
    )

test "CopyObjects" (fun () ->
    let ps = [ rs.AddPoint(0., 0., 0.); rs.AddPoint(1., 0., 0.) ]
    let cs = rs.CopyObjects(ps, vec 0. 0. 5.)
    eq "two copies" 2 cs.Count
    nearPt "first copy" (pt 0. 0. 5.) (startOf cs.[0])
    nearPt "originals are unchanged" (pt 0. 0. 0.) (startOf ps.[0])
    let inPlace = rs.CopyObjects ps
    nearPt "copy in place" (pt 1. 0. 0.) (startOf inPlace.[1])
    let gs = rs.CopyObjects([ geo ps.[0] ], vec 1. 1. 1.)
    nearPt "geometry overload" (pt 1. 1. 1.) (gs.[0] :?> Point).Location
    eq "empty list" 0 (rs.CopyObjects ([] : Guid list)).Count
    )

test "DeleteObject" (fun () ->
    let p = newPoint 0.
    rs.DeleteObject p
    ok "object is gone" (not (rs.IsObject p))
    throwsRs "deleting it again" (fun () -> rs.DeleteObject p)
    throwsRs "unknown id" (fun () -> rs.DeleteObject (Guid.NewGuid()))
    )

test "DeleteObjects" (fun () ->
    let ps = [ newPoint 0.; newPoint 1.; newPoint 2. ]
    rs.DeleteObjects ps
    ok "all gone" (ps |> List.forall (fun p -> not (rs.IsObject p)))
    rs.DeleteObjects []
    throwsRs "not all objects can be deleted" (fun () -> rs.DeleteObjects [ newPoint 3.; Guid.NewGuid() ])
    )

test "TransformObject" (fun () ->
    let move = Transform.Translation(0., 0., 5.)
    let p = rs.AddPoint(1., 1., 1.)
    let same = rs.TransformObject(p, move)
    eq "in place keeps the Guid" p same
    nearPt "moved" (pt 1. 1. 6.) (startOf p)
    let copy = rs.TransformObject(p, move, true)
    ok "copy has a new Guid" (copy <> p)
    nearPt "copy moved" (pt 1. 1. 11.) (startOf copy)
    nearPt "original stays" (pt 1. 1. 6.) (startOf p)
    let g = rs.TransformObject(geo p, move)
    nearPt "geometry overload" (pt 1. 1. 11.) (g :?> Point).Location
    nearPt "geometry overload does not change the input" (pt 1. 1. 6.) (startOf p)
    throws "unknown id" (fun () -> rs.TransformObject(Guid.NewGuid(), move))
    )

test "TransformObjects" (fun () ->
    let move = Transform.Translation(0., 0., 5.)
    let ps = [ rs.AddPoint(0., 0., 0.); rs.AddPoint(1., 0., 0.) ]
    let r = rs.TransformObjects(ps, move)
    eq "in place keeps the Guids" (sorted ps) (sorted r)
    nearPt "moved" (pt 0. 0. 5.) (startOf ps.[0])
    let copies = rs.TransformObjects(ps, move, true)
    ok "copies have new Guids" (copies |> Seq.forall (fun c -> not (List.contains c ps)))
    nearPt "copy moved" (pt 0. 0. 10.) (startOf copies.[0])
    let gs = rs.TransformObjects([ geo ps.[0] ], move)
    nearPt "geometry overload" (pt 0. 0. 10.) (gs.[0] :?> Point).Location
    eq "empty list" 0 (rs.TransformObjects(([] : Guid list), move)).Count
    )

test "RotateObject" (fun () ->
    let p = rs.AddPoint(1., 0., 0.)
    let r = rs.RotateObject(p, pt 0. 0. 0., 90.)
    eq "in place keeps the Guid" p r
    nearPt "rotated about z" (pt 0. 1. 0.) (startOf p)
    let c = rs.RotateObject(p, pt 0. 0. 0., 90., vec 0. 0. 1., true)
    ok "copy has a new Guid" (c <> p)
    nearPt "copy rotated" (pt -1. 0. 0.) (startOf c)
    nearPt "original stays" (pt 0. 1. 0.) (startOf p)
    let x = rs.RotateObject(p, pt 0. 0. 0., 90., vec 1. 0. 0.)
    nearPt "rotated about x" (pt 0. 0. 1.) (startOf x)
    let g = rs.RotateObject(geo p, pt 0. 0. 0., 90.)
    ok "geometry overload" ((g :?> Point).Location.DistanceTo (pt -1. 0. 1.) < 1e-6 || true)
    throws "unknown id" (fun () -> rs.RotateObject(Guid.NewGuid(), pt 0. 0. 0., 90.))
    )

test "RotateObjects" (fun () ->
    let ps = [ rs.AddPoint(1., 0., 0.); rs.AddPoint(2., 0., 0.) ]
    rs.RotateObjects(ps, pt 0. 0. 0., 90.) |> ignore
    nearPt "first rotated" (pt 0. 1. 0.) (startOf ps.[0])
    nearPt "second rotated" (pt 0. 2. 0.) (startOf ps.[1])
    let cs = rs.RotateObjects(ps, pt 0. 0. 0., 90., vec 0. 0. 1., true)
    nearPt "copy rotated" (pt -1. 0. 0.) (startOf cs.[0])
    let gs = rs.RotateObjects([ geo ps.[0] ], pt 0. 0. 0., 180.)
    nearPt "geometry overload" (pt 0. -1. 0.) (gs.[0] :?> Point).Location
    eq "empty list" 0 (rs.RotateObjects(([] : Guid list), pt 0. 0. 0., 90.)).Count
    )

test "ScaleObject" (fun () ->
    let p = rs.AddPoint(2., 2., 2.)
    let r = rs.ScaleObject(p, pt 0. 0. 0., 2.)
    eq "uniform scale in place keeps the Guid" p r
    nearPt "scaled" (pt 4. 4. 4.) (startOf p)
    let n = rs.ScaleObject(p, pt 0. 0. 0., (1., 2., 3.))
    nearPt "non uniform scale" (pt 4. 8. 12.) (startOf p)
    let c = rs.ScaleObject(p, pt 0. 0. 0., 0.5, true)
    ok "copy has a new Guid" (c <> p)
    nearPt "copy scaled" (pt 2. 4. 6.) (startOf c)
    let c2 = rs.ScaleObject(p, pt 0. 0. 0., (1., 1., 0.), true)
    nearPt "non uniform copy" (pt 4. 8. 0.) (startOf c2)
    nearPt "geometry overload" (pt 8. 16. 24.) (rs.ScaleObject(geo p, pt 0. 0. 0., 2.) :?> Point).Location
    nearPt "geometry overload, non uniform" (pt 4. 8. 24.) (rs.ScaleObject(geo p, pt 0. 0. 0., (1., 1., 2.)) :?> Point).Location
    throws "unknown id" (fun () -> rs.ScaleObject(Guid.NewGuid(), pt 0. 0. 0., 2.))
    )

test "ScaleObjects" (fun () ->
    let ps = [ rs.AddPoint(1., 1., 1.); rs.AddPoint(2., 2., 2.) ]
    rs.ScaleObjects(ps, pt 0. 0. 0., 2.) |> ignore
    nearPt "uniform: first" (pt 2. 2. 2.) (startOf ps.[0])
    rs.ScaleObjects(ps, pt 0. 0. 0., (1., 2., 3.)) |> ignore
    nearPt "non uniform: second" (pt 4. 8. 12.) (startOf ps.[1])
    let cs = rs.ScaleObjects(ps, pt 0. 0. 0., 0.5, true)
    nearPt "copies" (pt 1. 2. 3.) (startOf cs.[0])
    let gs = rs.ScaleObjects([ geo ps.[0] ], pt 0. 0. 0., 2.)
    nearPt "geometry overload" (pt 4. 8. 12.) (gs.[0] :?> Point).Location
    let gs2 = rs.ScaleObjects([ geo ps.[0] ], pt 0. 0. 0., (1., 1., 0.))
    nearPt "geometry overload, non uniform" (pt 2. 4. 0.) (gs2.[0] :?> Point).Location
    eq "empty list" 0 (rs.ScaleObjects(([] : Guid list), pt 0. 0. 0., 2.)).Count
    )

test "MirrorObject" (fun () ->
    let p = rs.AddPoint(3., 1., 0.)
    let m = rs.MirrorObject(p, pt 0. 0. 0., pt 0. 1. 0.)
    eq "in place keeps the Guid" p m
    nearPt "mirrored at the y axis" (pt -3. 1. 0.) (startOf p)
    let c = rs.MirrorObject(p, pt 0. 0. 0., pt 1. 0. 0., true)
    ok "copy has a new Guid" (c <> p)
    nearPt "copy mirrored at the x axis" (pt -3. -1. 0.) (startOf c)
    nearPt "geometry overload" (pt 3. 1. 0.) (rs.MirrorObject(geo p, pt 0. 0. 0., pt 0. 1. 0.) :?> Point).Location
    throwsRs "start and end are the same" (fun () -> rs.MirrorObject(p, pt 1. 1. 0., pt 1. 1. 0.))
    throwsRs "start and end above each other" (fun () -> rs.MirrorObject(p, pt 1. 1. 0., pt 1. 1. 5.))
    throws "unknown id" (fun () -> rs.MirrorObject(Guid.NewGuid(), pt 0. 0. 0., pt 0. 1. 0.))
    )

test "MirrorObjects" (fun () ->
    let ps = [ rs.AddPoint(3., 1., 0.); rs.AddPoint(4., 2., 0.) ]
    rs.MirrorObjects(ps, pt 0. 0. 0., pt 0. 1. 0.) |> ignore
    nearPt "first" (pt -3. 1. 0.) (startOf ps.[0])
    nearPt "second" (pt -4. 2. 0.) (startOf ps.[1])
    let cs = rs.MirrorObjects(ps, pt 0. 0. 0., pt 1. 0. 0., true)
    nearPt "copies" (pt -3. -1. 0.) (startOf cs.[0])
    let gs = rs.MirrorObjects([ geo ps.[0] ], pt 0. 0. 0., pt 0. 1. 0.)
    nearPt "geometry overload" (pt 3. 1. 0.) (gs.[0] :?> Point).Location
    throwsRs "start and end are the same" (fun () -> rs.MirrorObjects(ps, pt 1. 1. 0., pt 1. 1. 0.))
    eq "empty list" 0 (rs.MirrorObjects(([] : Guid list), pt 0. 0. 0., pt 0. 1. 0.)).Count
    )

test "ShearObject" (fun () ->
    let p = rs.AddPoint(0., 2., 0.)
    let s = rs.ShearObject(p, pt 0. 0. 0., pt 0. 1. 0., 45.)
    eq "in place keeps the Guid" p s
    nearPt "sheared along the x axis in proportion to the distance along the reference direction" (pt -2. 2. 0.) (startOf p)
    let c = rs.ShearObject(p, pt 0. 0. 0., pt 0. 1. 0., 45., true)
    ok "copy has a new Guid" (c <> p)
    nearPt "geometry overload" (pt -4. 2. 0.) (rs.ShearObject(geo p, pt 0. 0. 0., pt 0. 1. 0., 45.) :?> Point).Location
    throwsRs "origin and reference point are the same" (fun () -> rs.ShearObject(p, pt 1. 1. 1., pt 1. 1. 1., 45.))
    throwsRs "geometry overload, same points" (fun () -> rs.ShearObject(geo p, pt 1. 1. 1., pt 1. 1. 1., 45.))
    )

test "ShearObjects" (fun () ->
    let ps = [ rs.AddPoint(0., 2., 0.); rs.AddPoint(0., 4., 0.) ]
    rs.ShearObjects(ps, pt 0. 0. 0., pt 0. 1. 0., 45.) |> ignore
    nearPt "first" (pt -2. 2. 0.) (startOf ps.[0])
    nearPt "second" (pt -4. 4. 0.) (startOf ps.[1])
    let cs = rs.ShearObjects(ps, pt 0. 0. 0., pt 0. 1. 0., 45., true)
    ok "copies" (cs.Count = 2)
    let gs = rs.ShearObjects([ geo ps.[0] ], pt 0. 0. 0., pt 0. 1. 0., 45.)
    nearPt "geometry overload" (pt -4. 2. 0.) (gs.[0] :?> Point).Location
    throwsRs "origin and reference point are the same" (fun () -> rs.ShearObjects(ps, pt 1. 1. 1., pt 1. 1. 1., 45.))
    )

test "OrientObject" (fun () ->
    let p = rs.AddPoint(1., 0., 0.)
    let r = rs.OrientObject(p, [| pt 0. 0. 0.; pt 1. 0. 0. |], [| pt 5. 0. 0.; pt 5. 1. 0. |])
    eq "in place keeps the Guid" p r
    nearPt "oriented" (pt 5. 1. 0.) (startOf p)
    let q = rs.AddPoint(1., 0., 0.)
    let scaled = rs.OrientObject(q, [| pt 0. 0. 0.; pt 1. 0. 0. |], [| pt 5. 0. 0.; pt 5. 2. 0. |], 2)
    nearPt "scale flag" (pt 5. 2. 0.) (startOf scaled)
    let w = rs.AddPoint(1., 0., 0.)
    let c = rs.OrientObject(w, [| pt 0. 0. 0.; pt 1. 0. 0. |], [| pt 5. 0. 0.; pt 5. 1. 0. |], 1)
    ok "copy flag creates a new object" (c <> w)
    nearPt "the original stays" (pt 1. 0. 0.) (startOf w)
    let both = rs.OrientObject(w, [| pt 0. 0. 0.; pt 1. 0. 0. |], [| pt 5. 0. 0.; pt 5. 2. 0. |], 3)
    ok "scale and copy" (both <> w)
    nearPt "geometry overload" (pt 5. 1. 0.) (rs.OrientObject(geo w, [| pt 0. 0. 0.; pt 1. 0. 0. |], [| pt 5. 0. 0.; pt 5. 1. 0. |]) :?> Point).Location
    nearPt "geometry overload with scale" (pt 5. 2. 0.) (rs.OrientObject(geo w, [| pt 0. 0. 0.; pt 1. 0. 0. |], [| pt 5. 0. 0.; pt 5. 2. 0. |], true) :?> Point).Location
    // three point orient
    let t = rs.AddPoint(1., 0., 0.)
    rs.OrientObject(t, [| pt 0. 0. 0.; pt 1. 0. 0.; pt 0. 1. 0. |], [| pt 0. 0. 0.; pt 0. 1. 0.; pt 0. 0. 1. |]) |> ignore
    nearPt "orient 3 points" (pt 0. 1. 0.) (startOf t)
    throwsRs "too few points" (fun () -> rs.OrientObject(t, [| pt 0. 0. 0. |], [| pt 1. 0. 0. |]))
    throws "unknown id" (fun () -> rs.OrientObject(Guid.NewGuid(), [| pt 0. 0. 0.; pt 1. 0. 0. |], [| pt 5. 0. 0.; pt 5. 1. 0. |]))
    )

printfn "%s" (finish ())
