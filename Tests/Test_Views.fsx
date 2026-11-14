// Tests for Src/Scripting_Views.fs
// The tests change the views of the test document only. Display modes are global to Rhino, they are used but never modified.
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Views"

let vec x y z = Vector3d(x, y, z)
let tempDir =
    let d = IO.Path.Combine(IO.Path.GetTempPath(), "RhinoScriptingViewsTest")
    IO.Directory.CreateDirectory d |> ignore
    d

// ------------------------------- views -------------------------------

test "ViewNames" (fun () ->
    let names = rs.ViewNames()
    for n in [ "Top"; "Front"; "Right"; "Perspective" ] do
        ok (sprintf "contains the standard view %s" n) (names.Contains n)
    eq "no layouts at first" 0 (rs.ViewNames 1).Count
    let _, layout = rs.AddLayout "vnLayout"
    ok "layouts are listed with viewType 1" ((rs.ViewNames 1).Contains layout)
    ok "layouts are not in the standard views" (not ((rs.ViewNames 0).Contains layout))
    let both = rs.ViewNames 2
    ok "standard views and layouts with viewType 2" (both.Contains layout && both.Contains "Top")
    throwsRs "invalid view type" (fun () -> rs.ViewNames 5)
    )

test "IsView" (fun () ->
    ok "standard view" (rs.IsView "Top")
    ok "unknown view" (not (rs.IsView "no such view"))
    let _, layout = rs.AddLayout "ivLayout"
    ok "layouts are views too" (rs.IsView layout)
    )

test "CurrentView" (fun () ->
    let original = rs.CurrentView()
    try
        rs.CurrentView "Top"
        eq "current view was set" "Top" (rs.CurrentView())
        ok "IsViewCurrent agrees" (rs.IsViewCurrent "Top")
        ok "another view is not current" (not (rs.IsViewCurrent "Front"))
        rs.CurrentView "Front"
        eq "set again" "Front" (rs.CurrentView())
        throwsRs "unknown view" (fun () -> rs.CurrentView "no such view")
    finally
        rs.CurrentView original
    )

test "IsViewCurrent" (fun () ->
    let original = rs.CurrentView()
    ok "the current view" (rs.IsViewCurrent original)
    ok "unknown view" (not (rs.IsViewCurrent "no such view"))
    )

test "RenameView" (fun () ->
    rs.RenameView("Front", "rvFront")
    ok "new title exists" (rs.IsView "rvFront")
    ok "old title is gone" (not (rs.IsView "Front"))
    rs.RenameView("rvFront", "Front")
    ok "renamed back" (rs.IsView "Front")
    throwsRs "unknown view" (fun () -> rs.RenameView("no such view", "x"))
    throws "null title" (fun () -> rs.RenameView("Front", null))
    )

test "ViewTitle" (fun () ->
    let id = (RhinoDoc.ActiveDoc.Views |> Seq.find (fun v -> v.MainViewport.Name = "Top")).MainViewport.Id
    eq "title of a view id" "Top" (rs.ViewTitle id)
    throws "unknown id" (fun () -> rs.ViewTitle (Guid.NewGuid()))
    )

test "ViewSize" (fun () ->
    let w, h = rs.ViewSize "Top"
    ok "positive size" (w > 0 && h > 0)
    let w2, h2 = rs.ViewSize()
    ok "the active view is used if omitted" (w2 > 0 && h2 > 0)
    throws "unknown view" (fun () -> rs.ViewSize "no such view")
    )

test "IsViewMaximized" (fun () ->
    ok "not maximized at first" (not (rs.IsViewMaximized "Top"))
    ok "default is the active view" (not (rs.IsViewMaximized()))
    throws "unknown view" (fun () -> rs.IsViewMaximized "no such view")
    )

test "MaximizeRestoreView" (fun () ->
    try
        rs.MaximizeRestoreView "Top"
        ok "maximized" (rs.IsViewMaximized "Top")
        rs.MaximizeRestoreView "Top"
        ok "restored" (not (rs.IsViewMaximized "Top"))
    finally
        if rs.IsViewMaximized "Top" then rs.MaximizeRestoreView "Top"
    throws "unknown view" (fun () -> rs.MaximizeRestoreView "no such view")
    )

test "IsViewPerspective" (fun () ->
    ok "Perspective is perspective" (rs.IsViewPerspective "Perspective")
    ok "Top is not" (not (rs.IsViewPerspective "Top"))
    throws "unknown view" (fun () -> rs.IsViewPerspective "no such view")
    )

test "IsViewTitleVisible" (fun () ->
    ok "title is visible by default" (rs.IsViewTitleVisible "Top")
    ok "active view if omitted" (rs.IsViewTitleVisible())
    throws "unknown view" (fun () -> rs.IsViewTitleVisible "no such view")
    )

test "ShowViewTitle" (fun () ->
    try
        rs.ShowViewTitle("Top", false)
        ok "title hidden" (not (rs.ShowViewTitle "Top"))
        ok "IsViewTitleVisible agrees" (not (rs.IsViewTitleVisible "Top"))
        rs.ShowViewTitle("Top", true)
        ok "title shown" (rs.ShowViewTitle "Top")
    finally
        rs.ShowViewTitle("Top", true)
    ok "empty string means the active view" (rs.ShowViewTitle "" || not (rs.ShowViewTitle ""))
    throws "unknown view" (fun () -> rs.ShowViewTitle "no such view")
    throws "unknown view, set" (fun () -> rs.ShowViewTitle("no such view", true))
    )

test "ShowGrid" (fun () ->
    let original = rs.ShowGrid "Top"
    try
        rs.ShowGrid("Top", not original)
        eq "grid toggled" (not original) (rs.ShowGrid "Top")
        rs.ShowGrid("Top", original)
        eq "grid restored" original (rs.ShowGrid "Top")
    finally
        rs.ShowGrid("Top", original)
    throws "unknown view" (fun () -> rs.ShowGrid "no such view")
    )

test "ShowGridAxes" (fun () ->
    let original = rs.ShowGridAxes "Top"
    try
        rs.ShowGridAxes("Top", not original)
        eq "grid axes toggled" (not original) (rs.ShowGridAxes "Top")
    finally
        rs.ShowGridAxes("Top", original)
    eq "restored" original (rs.ShowGridAxes "Top")
    throws "unknown view" (fun () -> rs.ShowGridAxes "no such view")
    )

test "ShowWorldAxes" (fun () ->
    let original = rs.ShowWorldAxes "Top"
    try
        rs.ShowWorldAxes("Top", not original)
        eq "world axes toggled" (not original) (rs.ShowWorldAxes "Top")
    finally
        rs.ShowWorldAxes("Top", original)
    eq "restored" original (rs.ShowWorldAxes "Top")
    throws "unknown view" (fun () -> rs.ShowWorldAxes "no such view")
    )

// ------------------------------- camera -------------------------------

test "ViewCamera" (fun () ->
    rs.ViewCamera("Perspective", pt 10. 20. 30.)
    nearPt "camera round trip" (pt 10. 20. 30.) (rs.ViewCamera "Perspective")
    throws "unknown view" (fun () -> rs.ViewCamera "no such view")
    throws "unknown view, set" (fun () -> rs.ViewCamera("no such view", pt 0. 0. 0.))
    )

test "ViewTarget" (fun () ->
    rs.ViewTarget("Perspective", pt 1. 2. 3.)
    nearPt "target round trip" (pt 1. 2. 3.) (rs.ViewTarget "Perspective")
    throws "unknown view" (fun () -> rs.ViewTarget "no such view")
    throws "unknown view, set" (fun () -> rs.ViewTarget("no such view", pt 0. 0. 0.))
    )

test "ViewCameraTarget" (fun () ->
    rs.ViewCameraTarget("Perspective", pt 10. 10. 10., pt 0. 0. 0.)
    let c, t = rs.ViewCameraTarget "Perspective"
    nearPt "camera" (pt 10. 10. 10.) c
    nearPt "target" (pt 0. 0. 0.) t
    throws "unknown view" (fun () -> rs.ViewCameraTarget "no such view")
    throws "unknown view, set" (fun () -> rs.ViewCameraTarget("no such view", pt 1. 1. 1., pt 0. 0. 0.))
    )

test "ViewCameraUp" (fun () ->
    rs.ViewCameraTarget("Perspective", pt 10. 10. 10., pt 0. 0. 0.)
    rs.ViewCameraUp("Perspective", vec 0. 0. 1.)
    let up = rs.ViewCameraUp "Perspective"
    ok "up vector points up" (up.Z > 0.)
    throws "unknown view" (fun () -> rs.ViewCameraUp "no such view")
    throws "unknown view, set" (fun () -> rs.ViewCameraUp("no such view", vec 0. 0. 1.))
    )

test "ViewCameraPlane" (fun () ->
    rs.ViewCameraTarget("Perspective", pt 10. 0. 0., pt 0. 0. 0.)
    let p = rs.ViewCameraPlane "Perspective"
    nearPt "plane origin is the camera location" (pt 10. 0. 0.) p.Origin
    near "the plane normal points along the view direction" 1. (abs p.Normal.X)
    ok "the active view if omitted" ((rs.ViewCameraPlane()).IsValid)
    throws "unknown view" (fun () -> rs.ViewCameraPlane "no such view")
    )

test "ViewCameraLens" (fun () ->
    rs.ViewCameraLens("Perspective", 80.)
    near "lens length round trip" 80. (rs.ViewCameraLens "Perspective")
    throws "unknown view" (fun () -> rs.ViewCameraLens "no such view")
    throws "unknown view, set" (fun () -> rs.ViewCameraLens("no such view", 50.))
    )

test "ViewNearCorners" (fun () ->
    let a, b, c, d = rs.ViewNearCorners "Top"
    let pts = [ a; b; c; d ]
    ok "four distinct corners" (pts |> List.distinct |> List.length = 4)
    near "the corners lie in a plane parallel to the view" 0. (abs (a.Z - b.Z) + abs (b.Z - c.Z) + abs (c.Z - d.Z))
    near "counter clockwise order: first edge is as long as the opposite one" (a.DistanceTo b) (c.DistanceTo d)
    ok "active view if omitted" (let a2, _, _, _ = rs.ViewNearCorners() in a2.IsValid)
    throws "unknown view" (fun () -> rs.ViewNearCorners "no such view")
    )

test "ViewProjection" (fun () ->
    eq "Top is parallel" 1 (rs.ViewProjection "Top")
    eq "Perspective is perspective" 2 (rs.ViewProjection "Perspective")
    try
        rs.ViewProjection("Top", 2)
        eq "changed to perspective" 2 (rs.ViewProjection "Top")
        rs.ViewProjection("Top", 3)
        eq "changed to two point perspective" 3 (rs.ViewProjection "Top")
        rs.ViewProjection("Top", 1)
        eq "changed to parallel" 1 (rs.ViewProjection "Top")
        throwsRs "mode too small" (fun () -> rs.ViewProjection("Top", 0))
        throwsRs "mode too big" (fun () -> rs.ViewProjection("Top", 4))
    finally
        rs.ViewProjection("Top", 1)
    throws "unknown view" (fun () -> rs.ViewProjection "no such view")
    )

test "ViewRadius" (fun () ->
    rs.ZoomBoundingBox(BoundingBox(pt -10. -10. 0., pt 10. 10. 0.), "Top")
    let r = rs.ViewRadius "Top"
    ok "positive radius" (r > 0.)
    rs.ViewRadius("Top", 50.)
    nearTol "radius was set" 0.5 50. (rs.ViewRadius "Top")
    rs.ViewRadius("Top", 25., true)
    ok "dolly mode does not fail" (rs.ViewRadius "Top" > 0.)
    throwsRs "a perspective view has no radius" (fun () -> rs.ViewRadius "Perspective")
    throwsRs "a perspective view has no radius, set" (fun () -> rs.ViewRadius("Perspective", 10.))
    )

test "ViewCPlane" (fun () ->
    let p = rs.ViewCPlane "Top"
    nearVec "Top cplane normal" Vector3d.ZAxis p.Normal
    let moved = Plane(pt 1. 2. 3., Vector3d.XAxis, Vector3d.YAxis)
    rs.ViewCPlane("Top", moved)
    nearPt "cplane origin was set" (pt 1. 2. 3.) (rs.ViewCPlane "Top").Origin
    rs.ViewCPlane("Top", Plane.WorldXY)
    nearPt "restored" Point3d.Origin (rs.ViewCPlane "Top").Origin
    throws "unknown view" (fun () -> rs.ViewCPlane "no such view")
    throws "unknown view, set" (fun () -> rs.ViewCPlane("no such view", Plane.WorldXY))
    )

test "RotateCamera" (fun () ->
    rs.ViewCameraTarget("Perspective", pt 10. 0. 0., pt 0. 0. 0.)
    let before = rs.ViewCamera "Perspective"
    rs.RotateCamera(0, 90., "Perspective")
    let after = rs.ViewCamera "Perspective"
    nearPt "the camera location stays" before after
    ok "the target moved" ((rs.ViewTarget "Perspective").DistanceTo (pt 0. 0. 0.) > 1.)
    near "the target stays at the same distance from the camera" 10. (after.DistanceTo (rs.ViewTarget "Perspective"))
    for d in 1 .. 3 do rs.RotateCamera(d, 10., "Perspective")
    ok "the active view is used if omitted" (rs.RotateCamera(0, 1.); true)
    throwsRs "bad direction" (fun () -> rs.RotateCamera(4, 10., "Perspective"))
    throwsRs "negative direction" (fun () -> rs.RotateCamera(-1, 10., "Perspective"))
    throws "unknown view" (fun () -> rs.RotateCamera(0, 10., "no such view"))
    )

test "RotateView" (fun () ->
    rs.ViewCameraTarget("Perspective", pt 10. 0. 0., pt 0. 0. 0.)
    let before = rs.ViewCamera "Perspective"
    rs.RotateView(0, 20., "Perspective")
    ok "the camera moved" (before.DistanceTo (rs.ViewCamera "Perspective") > 0.1)
    for d in 1 .. 3 do rs.RotateView(d, 10., "Perspective")
    throws "unknown view" (fun () -> rs.RotateView(0, 10., "no such view"))
    )

test "TiltView" (fun () ->
    rs.ViewCameraTarget("Perspective", pt 10. 0. 0., pt 0. 0. 0.)
    rs.ViewCameraUp("Perspective", vec 0. 0. 1.)
    let before = rs.ViewCameraUp "Perspective"
    rs.TiltView(0, 30., "Perspective")
    let after = rs.ViewCameraUp "Perspective"
    ok "the up vector turned" (before.IsParallelTo after = 0)
    near "by 30 degrees" 30. (Vector3d.VectorAngle(before, after) |> RhinoMath.ToDegrees)
    rs.TiltView(1, 30., "Perspective")
    nearTol "tilting back" 0.01 0. (Vector3d.VectorAngle(before, rs.ViewCameraUp "Perspective") |> RhinoMath.ToDegrees)
    throws "unknown view" (fun () -> rs.TiltView(0, 10., "no such view"))
    )

// ------------------------------- zoom -------------------------------

test "ZoomBoundingBox" (fun () ->
    let bb = BoundingBox(pt 100. 100. 0., pt 120. 120. 0.)
    rs.ZoomBoundingBox(bb, "Top")
    let c, t = rs.ViewCameraTarget "Top"
    nearTol "the view is centered on the box (x)" 1. 110. t.X
    nearTol "the view is centered on the box (y)" 1. 110. t.Y
    rs.ZoomBoundingBox(BoundingBox(pt 0. 0. 0., pt 10. 10. 0.), "", true)
    let _, tf = rs.ViewCameraTarget "Front"
    nearTol "all=true zooms every view (front view x)" 1. 5. tf.X
    throws "unknown view" (fun () -> rs.ZoomBoundingBox(bb, "no such view"))
    )

test "ZoomExtents" (fun () ->
    rs.AddPoint(500., 500., 0.) |> ignore
    rs.AddPoint(520., 520., 0.) |> ignore
    rs.ZoomExtents "Top"
    let _, t = rs.ViewCameraTarget "Top"
    nearTol "centered on the objects (x)" 5. 510. t.X
    rs.ZoomExtents("", true)
    let _, tr = rs.ViewCameraTarget "Right"
    nearTol "all=true zooms every view (right view y)" 5. 510. tr.Y
    throws "unknown view" (fun () -> rs.ZoomExtents "no such view")
    )

test "ZoomSelected" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1000., 1000., 0.)
    rs.SelectObject b |> ignore
    rs.ZoomSelected "Top"
    let _, t = rs.ViewCameraTarget "Top"
    nearTol "centered on the selected object (x)" 5. 1000. t.X
    rs.ZoomSelected("", true)
    let _, tf = rs.ViewCameraTarget "Front"
    nearTol "all=true zooms every view (front view x)" 5. 1000. tf.X
    throws "unknown view" (fun () -> rs.ZoomSelected "no such view")
    )

test "ViewSpeedTest" (fun () ->
    let seconds = rs.ViewSpeedTest("Perspective", 3, true, 0, 5.)
    ok "a time in seconds" (seconds >= 0.)
    ok "defaults" (rs.ViewSpeedTest("Perspective", 2) >= 0.)
    throws "unknown view" (fun () -> rs.ViewSpeedTest("no such view", 1))
    )

// ------------------------------- display modes -------------------------------

test "ViewDisplayModes" (fun () ->
    let modes = rs.ViewDisplayModes()
    ok "there are display modes" (modes.Count > 0)
    ok "wireframe exists" (modes.Contains "Wireframe")
    ok "shaded exists" (modes.Contains "Shaded")
    )

test "ViewDisplayModeId" (fun () ->
    let id = rs.ViewDisplayModeId "Wireframe"
    ok "id is not empty" (id <> Guid.Empty)
    throwsRs "unknown mode" (fun () -> rs.ViewDisplayModeId "No Such Mode")
    )

test "ViewDisplayModeName" (fun () ->
    eq "name from the id" "Wireframe" (rs.ViewDisplayModeName (rs.ViewDisplayModeId "Wireframe"))
    throwsRs "unknown id" (fun () -> rs.ViewDisplayModeName (Guid.NewGuid()))
    )

test "ViewDisplayMode" (fun () ->
    let original = rs.ViewDisplayMode "Top"
    try
        rs.ViewDisplayMode("Top", "Shaded")
        eq "mode was set" "Shaded" (rs.ViewDisplayMode "Top")
        rs.ViewDisplayMode("Top", "Wireframe")
        eq "mode was set again" "Wireframe" (rs.ViewDisplayMode "Top")
        throwsRs "unknown mode" (fun () -> rs.ViewDisplayMode("Top", "No Such Mode"))
    finally
        rs.ViewDisplayMode("Top", original)
    throws "unknown view" (fun () -> rs.ViewDisplayMode "no such view")
    )

// ------------------------------- named views and cplanes -------------------------------

test "AddNamedView" (fun () ->
    let n = (rs.NamedViews()).Count
    rs.AddNamedView("nvA", "Top")
    eq "one more named view" (n + 1) (rs.NamedViews()).Count
    ok "it is listed" ((rs.NamedViews()).Contains "nvA")
    rs.AddNamedView "nvB" // the active view
    ok "active view if omitted" ((rs.NamedViews()).Contains "nvB")
    throwsRs "null name" (fun () -> rs.AddNamedView(null, "Top"))
    throws "unknown view" (fun () -> rs.AddNamedView("nvC", "no such view"))
    )

test "NamedViews" (fun () ->
    rs.AddNamedView("nvListA", "Top")
    rs.AddNamedView("nvListB", "Front")
    let names = rs.NamedViews()
    ok "contains both" (names.Contains "nvListA" && names.Contains "nvListB")
    )

test "RestoreNamedView" (fun () ->
    rs.ViewCameraTarget("Perspective", pt 10. 10. 10., pt 0. 0. 0.)
    rs.AddNamedView("rnvA", "Perspective")
    rs.ViewCameraTarget("Perspective", pt 50. 0. 0., pt 0. 0. 0.)
    try
        rs.RestoreNamedView("rnvA", "Perspective")
        // Rhino gives the view the name of the named view, so it is renamed back below
        let restoredTitle = if rs.IsView "Perspective" then "Perspective" else "rnvA"
        nearPt "camera restored" (pt 10. 10. 10.) (rs.ViewCamera restoredTitle)
        rs.RestoreNamedView("rnvA", restoredTitle, true)
    finally
        for v in [ "rnvA" ] do
            if rs.IsView v then rs.RenameView(v, "Perspective")
    throwsRs "unknown named view" (fun () -> rs.RestoreNamedView("no such named view", "Perspective"))
    throws "unknown view" (fun () -> rs.RestoreNamedView("rnvA", "no such view"))
    )

test "RestoreNamedView (the view keeps its title)" (fun () ->
    rs.AddNamedView("rnvTitle", "Front")
    try
        rs.RestoreNamedView("rnvTitle", "Front")
        ok "the restored view still has the title Front" (rs.IsView "Front")
    finally
        if rs.IsView "rnvTitle" then rs.RenameView("rnvTitle", "Front")
    )

test "DeleteNamedView" (fun () ->
    rs.AddNamedView("dnvA", "Top")
    ok "delete succeeds" (rs.DeleteNamedView "dnvA")
    ok "it is gone" (not ((rs.NamedViews()).Contains "dnvA"))
    ok "deleting again fails" (not (rs.DeleteNamedView "dnvA"))
    ok "unknown name" (not (rs.DeleteNamedView "no such named view"))
    )

test "AddNamedCPlane" (fun () ->
    let p = Plane(pt 1. 2. 3., Vector3d.XAxis, Vector3d.YAxis)
    rs.AddNamedCPlane("ncA", p)
    ok "it is listed" ((rs.NamedCPlanes()).Contains "ncA")
    nearPt "plane origin" (pt 1. 2. 3.) (rs.NamedCPlane "ncA").Origin
    throwsRs "null name" (fun () -> rs.AddNamedCPlane(null, p))
    )

test "NamedCPlanes" (fun () ->
    rs.AddNamedCPlane("ncListA", Plane.WorldXY)
    rs.AddNamedCPlane("ncListB", Plane.WorldYZ)
    let names = rs.NamedCPlanes()
    ok "contains both" (names.Contains "ncListA" && names.Contains "ncListB")
    )

test "NamedCPlane" (fun () ->
    rs.AddNamedCPlane("ncGetA", Plane(pt 5. 5. 5., Vector3d.YAxis, Vector3d.ZAxis))
    let p = rs.NamedCPlane "ncGetA"
    nearPt "origin" (pt 5. 5. 5.) p.Origin
    nearVec "x axis" Vector3d.YAxis p.XAxis
    throwsRs "unknown name" (fun () -> rs.NamedCPlane "no such cplane")
    )

test "RestoreNamedCPlane" (fun () ->
    rs.AddNamedCPlane("ncRestA", Plane(pt 7. 8. 9., Vector3d.XAxis, Vector3d.YAxis))
    eq "returns the name" "ncRestA" (rs.RestoreNamedCPlane("ncRestA", "Top"))
    nearPt "the view's cplane was set" (pt 7. 8. 9.) (rs.ViewCPlane "Top").Origin
    eq "active view if omitted" "ncRestA" (rs.RestoreNamedCPlane "ncRestA")
    rs.ViewCPlane("Top", Plane.WorldXY)
    throwsRs "unknown name" (fun () -> rs.RestoreNamedCPlane("no such cplane", "Top"))
    throws "unknown view" (fun () -> rs.RestoreNamedCPlane("ncRestA", "no such view"))
    )

test "DeleteNamedCPlane" (fun () ->
    rs.AddNamedCPlane("ncDelA", Plane.WorldXY)
    ok "delete succeeds" (rs.DeleteNamedCPlane "ncDelA")
    ok "it is gone" (not ((rs.NamedCPlanes()).Contains "ncDelA"))
    ok "deleting again fails" (not (rs.DeleteNamedCPlane "ncDelA"))
    )

// ------------------------------- layouts and details -------------------------------

test "AddLayout" (fun () ->
    let id, name = rs.AddLayout "alLayout"
    ok "id is not empty" (id <> Guid.Empty)
    eq "name is the title" "alLayout" name
    ok "it is a layout" (rs.IsLayout name)
    let id2, name2 = rs.AddLayout("alLayout2", 297., 210.)
    eq "name with a paper size" "alLayout2" name2
    let _, name3 = rs.AddLayout()
    ok "a generated name" (not (String.IsNullOrEmpty name3))
    ok "layout views are listed" ((rs.ViewNames 1).Contains name3)
    )

test "IsLayout" (fun () ->
    let _, name = rs.AddLayout "ilLayout"
    ok "a layout" (rs.IsLayout name)
    ok "a standard view is not a layout" (not (rs.IsLayout "Top"))
    throwsRs "unknown view" (fun () -> rs.IsLayout "no such view")
    )

test "AddDetail" (fun () ->
    let _, layout = rs.AddLayout("adLayout", 297., 210.)
    let d = rs.AddDetail(layout, Point2d(10., 10.), Point2d(100., 100.), "adDetail")
    ok "id is not empty" (d <> Guid.Empty)
    ok "detail exists" (rs.IsDetail(layout, "adDetail"))
    let d2 = rs.AddDetail(layout, Point2d(110., 10.), Point2d(200., 100.), "adDetail2", 3)
    ok "detail with another projection" (d2 <> Guid.Empty)
    throwsRs "projection 0" (fun () -> rs.AddDetail(layout, Point2d(0., 0.), Point2d(5., 5.), "x", 0))
    throwsRs "projection 8" (fun () -> rs.AddDetail(layout, Point2d(0., 0.), Point2d(5., 5.), "x", 8))
    throws "unknown layout" (fun () -> rs.AddDetail("no such layout", Point2d(0., 0.), Point2d(5., 5.), "x"))
    )

test "IsDetail" (fun () ->
    let _, layout = rs.AddLayout("idLayout", 297., 210.)
    rs.AddDetail(layout, Point2d(10., 10.), Point2d(100., 100.), "idDetail") |> ignore
    ok "existing detail" (rs.IsDetail(layout, "idDetail"))
    ok "names are not case sensitive" (rs.IsDetail(layout, "IDDETAIL"))
    ok "unknown detail" (not (rs.IsDetail(layout, "no such detail")))
    throws "unknown layout" (fun () -> rs.IsDetail("no such layout", "idDetail"))
    )

test "CurrentDetail" (fun () ->
    let _, layout = rs.AddLayout("cdLayout", 297., 210.)
    rs.AddDetail(layout, Point2d(10., 10.), Point2d(100., 100.), "cdDetail") |> ignore
    rs.CurrentDetail(layout, "cdDetail")
    eq "current detail was set" "cdDetail" (rs.CurrentDetail layout)
    rs.CurrentDetail(layout, layout)
    eq "the page itself is current: empty string" "" (rs.CurrentDetail layout)
    throwsRs "unknown detail" (fun () -> rs.CurrentDetail(layout, "no such detail"))
    throws "unknown layout" (fun () -> rs.CurrentDetail "no such layout")
    )

test "DetailLock" (fun () ->
    let _, layout = rs.AddLayout("dlLayout", 297., 210.)
    let d = rs.AddDetail(layout, Point2d(10., 10.), Point2d(100., 100.), "dlDetail")
    ok "not locked at first" (not (rs.DetailLock d))
    rs.DetailLock(d, true)
    ok "locked" (rs.DetailLock d)
    ok "geometry overload" (rs.DetailLock ((doc().Objects.FindId d).Geometry :?> DetailView))
    rs.DetailLock(d, false)
    ok "unlocked" (not (rs.DetailLock d))
    throwsRs "not a detail, set" (fun () -> rs.DetailLock(rs.AddPoint(0., 0., 0.), true))
    throws "not a detail, get" (fun () -> rs.DetailLock (rs.AddPoint(0., 0., 0.)))
    )

test "DetailScale" (fun () ->
    let _, layout = rs.AddLayout("dsLayout", 297., 210.)
    let d = rs.AddDetail(layout, Point2d(10., 10.), Point2d(100., 100.), "dsDetail")
    rs.DetailScale(d, 100., 10.)
    near "scale: 100 model units on 10 page units" 0.1 (rs.DetailScale d)
    near "geometry overload" 0.1 (rs.DetailScale ((doc().Objects.FindId d).Geometry :?> DetailView))
    rs.DetailScale(d, 1., 1.)
    near "scale 1:1" 1. (rs.DetailScale d)
    throwsRs "zero page length" (fun () -> rs.DetailScale(d, 1., 0.))
    throws "not a detail" (fun () -> rs.DetailScale (rs.AddPoint(0., 0., 0.)))
    )

// ------------------------------- wallpaper -------------------------------

let wallpaperFile =
    let f = IO.Path.Combine(tempDir, "wallpaper.png")
    use bmp = new Drawing.Bitmap(8, 8)
    bmp.Save(f, Drawing.Imaging.ImageFormat.Png)
    f

test "Wallpaper" (fun () ->
    eq "no wallpaper at first" "" (rs.Wallpaper "Top")
    rs.Wallpaper("Top", wallpaperFile)
    eq "wallpaper round trip" (wallpaperFile.ToLowerInvariant()) ((rs.Wallpaper "Top").ToLowerInvariant())
    throwsRs "file does not exist" (fun () -> rs.Wallpaper("Top", IO.Path.Combine(tempDir, "nothere.png")))
    throws "unknown view" (fun () -> rs.Wallpaper "no such view")
    )

test "IsWallpaper" (fun () ->
    ok "no wallpaper in Front" (not (rs.IsWallpaper "Front"))
    rs.Wallpaper("Front", wallpaperFile)
    ok "wallpaper in Front" (rs.IsWallpaper "Front")
    throws "unknown view" (fun () -> rs.IsWallpaper "no such view")
    )

test "WallpaperGrayScale" (fun () ->
    rs.Wallpaper("Right", wallpaperFile)
    ok "color at first" (not (rs.WallpaperGrayScale "Right"))
    rs.WallpaperGrayScale("Right", true)
    ok "gray" (rs.WallpaperGrayScale "Right")
    rs.WallpaperGrayScale("Right", false)
    ok "color again" (not (rs.WallpaperGrayScale "Right"))
    throws "unknown view" (fun () -> rs.WallpaperGrayScale "no such view")
    )

test "WallpaperHidden" (fun () ->
    rs.Wallpaper("Perspective", wallpaperFile)
    ok "visible at first" (not (rs.WallpaperHidden "Perspective"))
    rs.WallpaperHidden("Perspective", true)
    ok "hidden" (rs.WallpaperHidden "Perspective")
    rs.WallpaperHidden("Perspective", false)
    ok "visible again" (not (rs.WallpaperHidden "Perspective"))
    throws "unknown view" (fun () -> rs.WallpaperHidden "no such view")
    )

try IO.Directory.Delete(tempDir, true) with _ -> ()

printfn "%s" (finish ())
