namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsView =
    type RhinoScriptSyntax with

    [<EXT>]
    ///<summary>Add new detail view to an existing layout view</summary>
    ///<param name="layoutName">(string) Name of an existing layout</param>
    ///<param name="corner1">(Point2d) Corner1 of the detail in the layout's unit system</param>
    ///<param name="corner2">(Point2d) Corner2 of the detail in the layout's unit system</param>
    ///<param name="title">(string) Optional, Title of the new detail</param>
    ///<param name="projection">(int) Optional, Default Value: <c>1</c>
    ///Type of initial view projection for the detail
    ///  1 = parallel top view
    ///  2 = parallel bottom view
    ///  3 = parallel left view
    ///  4 = parallel right view
    ///  5 = parallel front view
    ///  6 = parallel back view
    ///  7 = perspective view</param>
    ///<returns>(Guid) identifier of the newly created detail on success</returns>
    static member AddDetail( layoutName:string,
                             corner1:Point2d,
                             corner2:Point2d,
                             [<OPT;DEF(null:string)>]title:string,
                             [<OPT;DEF(1)>]projection:int) : Guid =
        if projection<1 || projection>7 then failwithf "Rhino.Scripting: Projection must be a value between 1-7.  layoutId:'%A' corner1:'%A' corner2:'%A' title:'%A' projection:'%A'" layoutName corner1 corner2 title projection
        let layout = RhinoScriptSyntax.CoercePageView(layoutName)//TODO test this
        if isNull layout then failwithf "Rhino.Scripting: No layout found for given layoutId.  layoutId:'%A' corner1:'%A' corner2:'%A' title:'%A' projection:'%A'" layoutName corner1 corner2 title projection
        let projection : Display.DefinedViewportProjection = LanguagePrimitives.EnumOfValue  projection
        let detail = layout.AddDetailView(title, corner1, corner2, projection)
        if isNull detail then failwithf "Rhino.Scripting: AddDetail failed.  layoutId:'%A' corner1:'%A' corner2:'%A' title:'%A' projection:'%A'" layoutName corner1 corner2 title projection
        Doc.Views.Redraw()
        detail.Id


    [<EXT>]
    ///<summary>Adds a new page layout view</summary>
    ///<param name="title">(string) Optional, Title of new layout</param>
    ///<param name="size">(float * float) Optional, Width and height of paper for the new layout</param>
    ///<returns>(Guid*string) Id and Name of new layout</returns>
    static member AddLayout([<OPT;DEF(null:string)>]title:string, [<OPT;DEF(null)>]size:float * float) : Guid*string =
        let page =
            if Object.ReferenceEquals(size, null)  then Doc.Views.AddPageView(title)
            else  Doc.Views.AddPageView(title, size|> fst, size|> snd)
        if notNull page then page.MainViewport.Id, page.PageName
        else failwithf "AddLayout failed for %A %A" title size


    [<EXT>]
    ///<summary>Adds new named construction plane to the document</summary>
    ///<param name="cplaneName">(string) The name of the new named construction plane</param>
    ///<param name="plane">(Plane) The construction plane</param>
    ///<returns>(unit) void, nothing</returns>
    static member AddNamedCPlane(cplaneName:string, plane:Plane) : unit =
        if isNull cplaneName then failwithf "Rhino.Scripting: CplaneName = null.  cplaneName:'%A' plane:'%A'" cplaneName plane
        let index = Doc.NamedConstructionPlanes.Add(cplaneName, plane)
        if index<0 then failwithf "Rhino.Scripting: AddNamedCPlane failed.  cplaneName:'%A' plane:'%A'" cplaneName plane
        ()


    [<EXT>]
    ///<summary>Adds a new named view to the document</summary>
    ///<param name="name">(string) The name of the new named view</param>
    ///<param name="view">(string) Optional, The title of the view to save. If omitted, the current
    ///  active view is saved</param>
    ///<returns>(unit) void, nothing</returns>
    static member AddNamedView(name:string, [<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        if isNull name then failwithf "Rhino.Scripting: Name = empty.  name:'%A' view:'%A'" name view
        let viewportId = view.MainViewport.Id
        let index = Doc.NamedViews.Add(name, viewportId)
        if index<0 then failwithf "Rhino.Scripting: AddNamedView failed.  name:'%A' view:'%A'" name view



    [<EXT>]
    ///<summary>Returns the current detail view in a page layout view</summary>
    ///<param name="layout">(string) Title of an existing page layout view</param>
    ///<returns>(string option) Option of The name  the current detail view, None if Page is current view</returns>
    static member CurrentDetail(layout:string) : string option = //GET
        let page = RhinoScriptSyntax.CoercePageView(layout)
        if page.MainViewport.Id = page.ActiveViewport.Id then None
        else  Some  page.ActiveViewport.Name

    [<EXT>]
    ///<summary>Changes the current detail view in a page layout view</summary>
    ///<param name="layout">(string) Title of an existing page layout view</param>
    ///<param name="detail">(string) Title of the detail view to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentDetail(layout:string, detail:string) : unit = //SET
        let page = RhinoScriptSyntax.CoercePageView(layout)
        if layout = detail then page.SetPageAsActive()
        else
            if not <| page.SetActiveDetail(detail, false) then failwithf "set CurrentDetail failed for %s to %s" layout detail
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the currently active view</summary>
    ///<returns>(string) The title of the current view</returns>
    static member CurrentView() : string = //GET
        Doc.Views.ActiveView.MainViewport.Name

    [<EXT>]
    ///<summary>Sets the currently active view</summary>
    ///<param name="view">(string) Title of the view to set current</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentView(view:string) : unit = //SET
        Doc.Views.ActiveView <- RhinoScriptSyntax.CoerceView(view)



    [<EXT>]
    ///<summary>Removes a named construction plane from the document</summary>
    ///<param name="name">(string) Name of the construction plane to remove</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteNamedCPlane(name:string) : bool =
        Doc.NamedConstructionPlanes.Delete(name)


    [<EXT>]
    ///<summary>Removes a named view from the document</summary>
    ///<param name="name">(string) Name of the named view to remove</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteNamedView(name:string) : bool =
        Doc.NamedViews.Delete(name)


    [<EXT>]
    ///<summary>Returns the projection locked state of a detail viewport rectangle</summary>
    ///<param name="detailId">(Guid) Identifier of a detail rectangle object</param>
    ///<returns>(bool) the current detail projection locked state</returns>
    static member DetailLock(detailId:Guid) : bool = //GET
        let detail = RhinoScriptSyntax.CoerceDetailView(detailId)
        detail.IsProjectionLocked

    [<EXT>]
    ///<summary>Modifies the projection locked state of a detail</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<param name="lock">(bool) The new lock state</param>
    ///<returns>(unit) void, nothing</returns>
    static member DetailLock(detailId:Guid, lock:bool) : unit = //SET
        let detail =
            try Doc.Objects.FindId(detailId) :?> DocObjects.DetailViewObject
            with _ ->  failwithf "Rhino.Scripting: Set DetailLock failed. detailId is a %s '%A' lock:'%A'" (rhtype detailId) detailId lock
        if lock <> detail.DetailGeometry.IsProjectionLocked then
            detail.DetailGeometry.IsProjectionLocked <- lock
            detail.CommitChanges() |> ignore



    [<EXT>]
    ///<summary>Returns the scale of a detail object</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<returns>(float) current page to model scale ratio if model Length and page Length are both None</returns>
    static member DetailScale(detailId:Guid) : float = //GET
        let detail = RhinoScriptSyntax.CoerceDetailViewObject(detailId)
        detail.DetailGeometry.PageToModelRatio

    [<EXT>]
    ///<summary>Modifies the scale of a detail object</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<param name="modelLength">(float) A length in the current model units</param>
    ///<param name="pageLength">(float) A length in the current page units</param>
    ///<returns>(unit) void, nothing</returns>
    static member DetailScale(detailId:Guid, modelLength:float, pageLength:float) : unit = //SET
        let detail = RhinoScriptSyntax.CoerceDetailViewObject(detailId)
        let modelunits = Doc.ModelUnitSystem
        let pageunits = Doc.PageUnitSystem
        if detail.DetailGeometry.SetScale(modelLength, modelunits, pageLength, pageunits) then
            detail.CommitChanges()|> ignore
            Doc.Views.Redraw()
        else
            failwithf "Rhino.Scripting: DetailScale failed.  detailId:'%A' modelLength:'%A' pageLength:'%A'" detailId modelLength pageLength



    [<EXT>]
    ///<summary>Verifies that a detail view exists on a page layout view</summary>
    ///<param name="layout">(string) Title of an existing page layout</param>
    ///<param name="detail">(string) Title of an existing detail view</param>
    ///<returns>(bool) True if detail is a detail view, False if detail is not a detail view</returns>
    static member IsDetail(layout:string, detail:string) : bool =
        let view = RhinoScriptSyntax.CoercePageView(layout)
        let det = RhinoScriptSyntax.CoerceView(detail)
        view.GetDetailViews()
        |> Array.exists (fun v -> v.Name = det.ActiveViewport.Name) // TODO test


    [<EXT>]
    ///<summary>Verifies that a view is a page layout view</summary>
    ///<param name="layout">(string) Title of an existing page layout view</param>
    ///<returns>(bool) True if layout is a page layout view, False is layout is a standard model view</returns>
    static member IsLayout(layout:string) : bool =
        //layoutid = RhinoScriptSyntax.Coerceguid(layout)
        if   Doc.Views.GetViewList(false, true) |> Array.exists ( fun v -> v.MainViewport.Name = layout) then true
        elif Doc.Views.GetViewList(true, false) |> Array.exists ( fun v -> v.MainViewport.Name = layout) then false
        else failwithf "Rhino.Scripting: IsLayout View does not exist at all.  layout:'%A'" layout // or false


    [<EXT>]
    ///<summary>Verifies that the specified view exists</summary>
    ///<param name="view">(string) Title of the view</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member IsView(view:string) : bool =
        Doc.Views.GetViewList(false, true) |> Array.exists ( fun v -> v.MainViewport.Name = view)



    [<EXT>]
    ///<summary>Verifies that the specified view is the current, or active view</summary>
    ///<param name="view">(string) Title of the view</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member IsViewCurrent(view:string) : bool =
        let activeview = Doc.Views.ActiveView
        view = activeview.MainViewport.Name


    [<EXT>]
    ///<summary>Verifies that the specified view is maximized (enlarged so as to fill
    ///  the entire Rhino window)</summary>
    ///<param name="view">(string) Optional, Title of the view. If omitted, the current
    ///  view is used</param>
    ///<returns>(bool) True of False</returns>
    static member IsViewMaximized([<OPT;DEF("":string)>]view:string) : bool =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.Maximized


    [<EXT>]
    ///<summary>Verifies that the specified view's projection is set to perspective</summary>
    ///<param name="view">(string) Title of the view</param>
    ///<returns>(bool) True of False</returns>
    static member IsViewPerspective(view:string) : bool =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.MainViewport.IsPerspectiveProjection


    [<EXT>]
    ///<summary>Verifies that the specified view's title window is visible</summary>
    ///<param name="view">(string) Optional, The title of the view. If omitted, the current
    ///  active view is used</param>
    ///<returns>(bool) True of False</returns>
    static member IsViewTitleVisible([<OPT;DEF("":string)>]view:string) : bool =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.TitleVisible


    [<EXT>]
    ///<summary>Verifies that the specified view contains a wallpaper image</summary>
    ///<param name="view">(string) View to verify</param>
    ///<returns>(bool) True or False</returns>
    static member IsWallpaper(view:string) : bool =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.MainViewport.WallpaperFilename.Length > 0


    [<EXT>]
    ///<summary>Toggles a view's maximized/restore window state of the specified view</summary>
    ///<param name="view">(string) Optional, The title of the view. If omitted, the current
    ///  active view is used</param>
    ///<returns>(unit)</returns>
    static member MaximizeRestoreView([<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.Maximized <- not view.Maximized


    [<EXT>]
    ///<summary>Returns the plane geometry of the specified named construction plane</summary>
    ///<param name="name">(string) The name of the construction plane</param>
    ///<returns>(Plane) a plane on success</returns>
    static member NamedCPlane(name:string) : Plane =
        let index = Doc.NamedConstructionPlanes.Find(name)
        if index<0 then failwithf "Rhino.Scripting: NamedCPlane failed.  name:'%A'" name
        Doc.NamedConstructionPlanes.[index].Plane


    [<EXT>]
    ///<summary>Returns the names of all named construction planes in the document</summary>
    ///<returns>(string ResizeArray) the names of all named construction planes in the document</returns>
    static member NamedCPlanes() : string ResizeArray =
        let count = Doc.NamedConstructionPlanes.Count
        resizeArray {for i in range(count) do Doc.NamedConstructionPlanes.[i].Name }



    [<EXT>]
    ///<summary>Returns the names of all named views in the document</summary>
    ///<returns>(string ResizeArray) the names of all named views in the document</returns>
    static member NamedViews() : string ResizeArray =
        let count = Doc.NamedViews.Count
        resizeArray {for i in range(count) do Doc.NamedViews.[i].Name }


    [<EXT>]
    ///<summary>Changes the title of the specified view</summary>
    ///<param name="oldTitle">(string) The title of the view to rename</param>
    ///<param name="newTitle">(string) The new title of the view</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenameView(oldTitle:string, newTitle:string) : unit =
        if isNull oldTitle || isNull newTitle then failwithf "Rhino.Scripting: RenameView failed.  oldTitle:'%A' newTitle:'%A'" oldTitle newTitle
        let foundview = RhinoScriptSyntax.CoerceView(oldTitle)
        foundview.MainViewport.Name <- newTitle



    [<EXT>]
    ///<summary>Restores a named construction plane to the specified view</summary>
    ///<param name="cplaneName">(string) Name of the construction plane to restore</param>
    ///<param name="view">(string) Optional, The title of the view. If omitted, the current
    ///  active view is used</param>
    ///<returns>(string) name of the restored named construction plane</returns>
    static member RestoreNamedCPlane(cplaneName:string, [<OPT;DEF("":string)>]view:string) : string =
        let view = RhinoScriptSyntax.CoerceView(view)
        let index = Doc.NamedConstructionPlanes.Find(cplaneName)
        if index<0 then failwithf "Rhino.Scripting: RestoreNamedCPlane failed.  cplaneName:'%A' view:'%A'" cplaneName view
        let cplane = Doc.NamedConstructionPlanes.[index]
        view.MainViewport.PushConstructionPlane(cplane)
        view.Redraw()
        cplaneName


    [<EXT>]
    ///<summary>Restores a named view to the specified view</summary>
    ///<param name="namedView">(string) Name of the named view to restore</param>
    ///<param name="view">(string) Optional, Title of the view to restore the named view.
    ///  If omitted, the current active view is used</param>
    ///<param name="restoreBitmap">(bool) Optional, Default Value: <c>false</c>
    ///Restore the named view's background bitmap</param>
    ///<returns>(unit) void, nothing</returns>
    static member RestoreNamedView( namedView:string,
                                    [<OPT;DEF("":string)>]view:string,
                                    [<OPT;DEF(false)>]restoreBitmap:bool) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        let index = Doc.NamedViews.FindByName(namedView)
        if index<0 then failwithf "Rhino.Scripting: RestoreNamedView failed.  namedView:'%A' view:'%A' restoreBitmap:'%A'" namedView view restoreBitmap
        let viewinfo = Doc.NamedViews.[index]
        if view.MainViewport.PushViewInfo(viewinfo, restoreBitmap) then
            view.Redraw()
            //view.MainViewport.Name
        else
            failwithf "Rhino.Scripting: RestoreNamedView failed.  namedView:'%A' view:'%A' restoreBitmap:'%A'" namedView view restoreBitmap


    [<EXT>]
    ///<summary>Rotates a perspective-projection view's camera. See the RotateCamera
    ///  command in the Rhino help file for more details</summary>
    ///<param name="direction">(int)
    ///The direction to rotate the camera where
    ///  0= right
    ///  1= left
    ///  2= down
    ///  3= up</param>
    ///<param name="angle">(float) The angle to rotate</param>
    ///<param name="view">(string) Optional, Title of the view. If omitted, current active view is used</param>
    ///<returns>(unit) void, nothing</returns>
    static member RotateCamera( direction:int,
                                angle:float,
                                [<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable angle = RhinoMath.ToRadians( abs(angle) )
        let targetdistance = (viewport.CameraLocation-viewport.CameraTarget)*viewport.CameraZ
        let mutable axis = viewport.CameraY
        if direction = 0 || direction = 2 then angle <- -angle
        if direction = 0 || direction = 1 then
            if ApplicationSettings.ViewSettings.RotateToView then
                axis <- viewport.CameraY
            else
                axis <- Vector3d.ZAxis
        elif direction = 2 || direction = 3 then
            axis <- viewport.CameraX

        if ApplicationSettings.ViewSettings.RotateReverseKeyboard then angle<- -angle
        let rot = Transform.Rotation(angle, axis, Point3d.Origin)
        let camUp = rot * viewport.CameraY
        let camDir = -(rot * viewport.CameraZ)
        let target = viewport.CameraLocation + targetdistance*camDir
        viewport.SetCameraLocations(target, viewport.CameraLocation)
        viewport.CameraUp <- camUp
        view.Redraw()



    [<EXT>]
    ///<summary>Rotates a view. See RotateView command in Rhino help for more information</summary>
    ///<param name="direction">(int) Optional, The direction to rotate the view where
    ///  0= right
    ///  1= left
    ///  2= down
    ///  3= up</param>
    ///<param name="angle">(float) Angle to rotate. If omitted, the angle of rotation is specified
    ///  by the "Increment in divisions of a circle" parameter specified in
    ///  Options command's View tab</param>
    ///<param name="view">(string) Optional, Title of the view. If omitted, the current active view is used</param>
    ///<returns>(unit) void, nothing</returns>
    static member RotateView( direction:int,
                              angle:float,
                              [<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable angle =  RhinoMath.ToRadians( abs(angle) )
        if ApplicationSettings.ViewSettings.RotateReverseKeyboard then angle <- -angle
        if direction = 0 then viewport.KeyboardRotate(true, angle)       |> ignore
        elif direction = 1 then viewport.KeyboardRotate(true, -angle)    |> ignore
        elif direction = 2 then viewport.KeyboardRotate(false, -angle)   |> ignore
        elif direction = 3 then viewport.KeyboardRotate(false, angle)    |> ignore
        view.Redraw()


    [<EXT>]
    ///<summary>Get status of a view's construction plane grid</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The grid display state</returns>
    static member ShowGrid(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        viewport.ConstructionGridVisible

    [<EXT>]
    ///<summary>Shows or hides a view's construction plane grid</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="show">(bool) The grid state to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member ShowGrid(view:string, show:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.ConstructionGridVisible
        if  rc <> show then
            viewport.ConstructionGridVisible <- show
            view.Redraw()



    [<EXT>]
    ///<summary>Get status of a view's construction plane grid axes</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The grid axes display state</returns>
    static member ShowGridAxes(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.ConstructionAxesVisible
        rc

    [<EXT>]
    ///<summary>Shows or hides a view's construction plane grid axes</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="show">(bool) The state to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member ShowGridAxes(view:string, show:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.ConstructionAxesVisible
        if  rc <> show then
            viewport.ConstructionAxesVisible <- show
            view.Redraw()



    [<EXT>]
    ///<summary>Get status of the title window of a view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The state to View Title visibility</returns>
    static member ShowViewTitle(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.TitleVisible

    [<EXT>]
    ///<summary>Shows or hides the title window of a view</summary>
    ///<param name="view">(string) Title of the view. If omitted, the current active view is used</param>
    ///<param name="show">(bool) The state to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member ShowViewTitle(view:string, show:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.TitleVisible <- show


    [<EXT>]
    ///<summary>Get status of a view's world axis icon</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The world axes display state</returns>
    static member ShowWorldAxes(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.WorldAxesVisible
        rc

    [<EXT>]
    ///<summary>Shows or hides a view's world axis icon</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="show">(bool) The state to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member ShowWorldAxes(view:string, show:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.WorldAxesVisible
        if rc <> show then
            viewport.WorldAxesVisible <- show
            view.Redraw()



    [<EXT>]
    ///<summary>Tilts a view by rotating the camera up vector. See the TiltView command in
    ///  the Rhino help file for more details</summary>
    ///<param name="direction">(int) The direction to rotate the view where
    ///  0= right
    ///  1= left</param>
    ///<param name="angle">(float) The angle in degrees to rotate</param>
    ///<param name="view">(string) Optional, Title of the view. If omitted, the current active view is used</param>
    ///<returns>(unit) void, nothing</returns>
    static member TiltView( direction:int,
                            angle:float,
                            [<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable angle = angle
        if ApplicationSettings.ViewSettings.RotateReverseKeyboard then angle <- -angle
        let axis = viewport.CameraLocation - viewport.CameraTarget
        if direction = 0 then viewport.Rotate(angle, axis, viewport.CameraLocation) |> ignore
        elif direction = 1 then viewport.Rotate(-angle, axis, viewport.CameraLocation)   |> ignore
        view.Redraw()



    [<EXT>]
    ///<summary>Returns the camera location of the specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Point3d) The current camera location</returns>
    static member ViewCamera(view:string) : Point3d = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.CameraLocation

    [<EXT>]
    ///<summary>Sets the camera location of the specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="cameraLocation">(Point3d) A 3D point identifying the new camera location</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCamera(view:string, cameraLocation:Point3d) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.SetCameraLocation(cameraLocation, true)
        view.Redraw()



    [<EXT>]
    ///<summary>Returns the 35mm camera lens length of the specified perspective
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    /// projection view</summary>
    ///<returns>(float) The current lens length</returns>
    static member ViewCameraLens(view:string) : float = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.Camera35mmLensLength

    [<EXT>]
    ///<summary>Sets the 35mm camera lens length of the specified perspective
    /// projection view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="length">(float) The new 35mm camera lens length</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCameraLens(view:string, length:float) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.Camera35mmLensLength <- length
        view.Redraw()



    [<EXT>]
    ///<summary>Returns the orientation of a view's camera</summary>
    ///<param name="view">(string) Optional, Title of the view. If omitted, the current active view is used</param>
    ///<returns>(Plane) the view's camera plane</returns>
    static member ViewCameraPlane([<OPT;DEF("":string)>]view:string) : Plane =
        let view = RhinoScriptSyntax.CoerceView(view)
        let rc, frame = view.ActiveViewport.GetCameraFrame()
        if not rc then failwithf "Rhino.Scripting: ViewCameraPlane failed.  view:'%A'" view
        frame


    [<EXT>]
    ///<summary>Returns the camera and target positions of the specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Point3d * Point3d) the 3d points containing the current camera and target locations</returns>
    static member ViewCameraTarget(view:string) : Point3d * Point3d = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.CameraLocation, view.ActiveViewport.CameraTarget

    [<EXT>]
    ///<summary>Sets the camera and target positions of the specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="camera">(Point3d) 3d point identifying the new camera location</param>
    ///<param name="target">(Point3d) 3d point identifying the new target location</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCameraTarget(view:string, camera:Point3d, target:Point3d) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.SetCameraLocations(target, camera)
        view.Redraw()


    [<EXT>]
    ///<summary>Returns the camera up direction of a specified</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Vector3d) The current camera up direction</returns>
    static member ViewCameraUp(view:string) : Vector3d = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.CameraUp

    [<EXT>]
    ///<summary>Sets the camera up direction of a specified</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="upVector">(Vector3d) 3D vector identifying the new camera up direction</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCameraUp(view:string, upVector:Vector3d) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.CameraUp <- upVector
        view.Redraw()



    [<EXT>]
    ///<summary>Return a view's construction plane</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Plane) The current construction plane</returns>
    static member ViewCPlane(view:string) : Plane = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.ConstructionPlane()

    [<EXT>]
    ///<summary>Set a view's construction plane</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="plane">(Plane) The new construction plane if setting</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCPlane(view:string, plane:Plane) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.SetConstructionPlane(plane)
        view.Redraw()



    [<EXT>]
    ///<summary>Return a view display mode</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(string) the current mode</returns>
    static member ViewDisplayMode(view:string) : string = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let current = view.ActiveViewport.DisplayMode
        current.EnglishName

    [<EXT>]
    ///<summary>Set a view display mode</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="mode">(string) Name of a display mode</param>
    ///<returns>(string) If mode is not specified, the current mode</returns>
    static member ViewDisplayMode(view:string, mode:string) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let desc = Rhino.Display.DisplayModeDescription.FindByName(mode)
        if notNull desc then
            view.ActiveViewport.DisplayMode <- desc
            Doc.Views.Redraw()
        else
            failwithf "set ViewDisplayMode mode %s not found." mode



    [<EXT>]
    ///<summary>Return id of a display mode given it's name</summary>
    ///<param name="name">(string) Name of the display mode</param>
    ///<returns>(Guid) The id of the display mode , otherwise None</returns>
    static member ViewDisplayModeId(name:string) : Guid =
        let desc = Rhino.Display.DisplayModeDescription.FindByName(name)
        if notNull desc then desc.Id
        else
            failwithf "set ViewDisplayModeId mode %s not found." name


    [<EXT>]
    ///<summary>Return name of a display mode given it's id</summary>
    ///<param name="modeId">(Guid) The identifier of the display mode obtained from the ViewDisplayModes method</param>
    ///<returns>(string) The name of the display mode , otherwise None</returns>
    static member ViewDisplayModeName(modeId:Guid) : string =
        //modeId = RhinoScriptSyntax.Coerceguid(modeId)
        let desc = Rhino.Display.DisplayModeDescription.GetDisplayMode(modeId)
        if notNull desc then desc.EnglishName
        else
            failwithf "set ViewDisplayModeName Id %A not found." modeId


    [<EXT>]
    ///<summary>Return list of display modes</summary>
    ///<returns>(string ResizeArray) strings identifying the display mode names</returns>
    static member ViewDisplayModes() : string ResizeArray =
        let modes = Rhino.Display.DisplayModeDescription.GetDisplayModes()
        resizeArray {for mode in modes do mode.EnglishName }


    [<EXT>]
    ///<summary>Return the names/titles, of all views in the document</summary>
    ///<param name="viewType">(int) Optional, Default: standard model views: <c>0</c>
    ///The type of view to return
    ///  0 = standard model views
    ///  1 = page layout views
    ///  2 = both standard and page layout views</param>
    ///<returns>(string ResizeArray) of the view names on success</returns>
    static member ViewNames([<OPT;DEF(0)>]viewType:int) : string ResizeArray =
        let views = Doc.Views.GetViewList(viewType <> 1, viewType>0)
        if views|> isNull  then failwithf "Rhino.Scripting: ViewNames failed. viewType:'%A'" viewType
        resizeArray { for view in views do view.MainViewport.Name}



    [<EXT>]
    ///<summary>Return 3d corners of a view's near clipping plane rectangle. Useful
    ///  in determining the "real world" size of a parallel-projected view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four Point3d that define the corners of the rectangle (counter-clockwise order)</returns>
    static member ViewNearCorners([<OPT;DEF("":string)>]view:string) : Point3d * Point3d * Point3d * Point3d =
        let view = RhinoScriptSyntax.CoerceView(view)
        let rc = view.ActiveViewport.GetNearRect()
        rc.[0], rc.[1], rc.[3], rc.[2]


    [<EXT>]
    ///<summary>Return a view's projection mode</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(int) The current projection mode for the specified view
    ///  1 = parallel
    ///  2 = perspective
    ///  3 = two point perspective</returns>
    static member ViewProjection(view:string) : int = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable rc = 2
        if viewport.IsParallelProjection then rc <- 1
        elif viewport.IsTwoPointPerspectiveProjection then rc <- 3
        rc

    [<EXT>]
    ///<summary>Set a view's projection mode</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="mode">(int) The projection mode
    ///  1 = parallel
    ///  2 = perspective
    ///  3 = two point perspective</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewProjection(view:string, mode:int) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        if mode = 1 then viewport.ChangeToParallelProjection(true) |> ignore
        elif mode = 2 then viewport.ChangeToPerspectiveProjection(true, 35.)|> ignore
        elif mode = 3 then viewport.ChangeToTwoPointPerspectiveProjection(35.)       |> ignore
        view.Redraw()



    [<EXT>]
    ///<summary>Returns the radius of a parallel-projected view. Useful
    /// when you need an absolute zoom factor for a parallel-projected view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(float) The current view radius for the specified view</returns>
    static member ViewRadius(view:string) : float = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        if not viewport.IsParallelProjection then failwithf "Rhino.Scripting: ViewRadius view is not ParallelProjection.  view:'%A' " view
        let ok, a, b, c, d, e, f = viewport.GetFrustum()
        let frusright = b
        let frustop = d
        let oldradius = min frustop frusright
        oldradius

    [<EXT>]
    ///<summary>Sets the radius of a parallel-projected view. Useful
    /// when you need an absolute zoom factor for a parallel-projected view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="radius">(float) The view radius</param>
    ///<param name="mode">(bool) Perform a "dolly" magnification by moving the camera
    ///  towards/away from the target so that the amount of the screen
    ///  subtended by an object changes.  true = perform a "zoom"
    ///  magnification by adjusting the "lens" angle</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewRadius(view:string, radius:float, mode:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        if not viewport.IsParallelProjection then failwithf "Rhino.Scripting: ViewRadius view is not ParallelProjection.  view:'%A' " view
        let ok, a, b, c, d, e, f = viewport.GetFrustum()
        let frusright = b
        let frustop = d
        let oldradius = min frustop frusright
        let magnificationfactor = radius / oldradius
        let d = 1.0 / magnificationfactor
        viewport.Magnify(d, mode) |> ignore
        view.Redraw()



    [<EXT>]
    ///<summary>Returns the width and height in pixels of the specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(int * int ) of two numbers identifying width and height</returns>
    static member ViewSize([<OPT;DEF(null:string)>]view:string) : int * int =
        let view = RhinoScriptSyntax.CoerceView(view)
        let cr = view.ClientRectangle
        cr.Width, cr.Height


    [<EXT>]
    ///<summary>Test's Rhino's display performance</summary>
    ///<param name="view">(string) Optional, The title of the view.  If omitted, the current active view is used</param>
    ///<param name="frames">(int) Optional, Default Value: <c>100</c>
    ///The number of frames, or times to regenerate the view. If omitted, the view will be regenerated 100 times</param>
    ///<param name="freeze">(bool) Optional, Default Value: <c>true</c>
    ///If True (Default), then Rhino's display list will not be updated with every frame redraw. If False, then Rhino's display list will be updated with every frame redraw</param>
    ///<param name="direction">(int) Optional, Default Value: <c>0</c>
    ///The direction to rotate the view. The default direction is Right (0). Modes:
    ///  0 = Right
    ///  1 = Left
    ///  2 = Down
    ///  3 = Up</param>
    ///<param name="angleDegrees">(float) Optional, Default Value: <c>5</c>
    ///The angle to rotate. If omitted, the rotation angle of 5.0 degrees will be used</param>
    ///<returns>(float) The number of seconds it took to regenerate the view frames number of times,</returns>
    static member ViewSpeedTest( [<OPT;DEF("":string)>]view:string,
                                 [<OPT;DEF(100)>]frames:int,
                                 [<OPT;DEF(true)>]freeze:bool,
                                 [<OPT;DEF(0)>]direction:int,
                                 [<OPT;DEF(5)>]angleDegrees:float) : float =
        let view = RhinoScriptSyntax.CoerceView(view)
        let angleradians = toRadians(angleDegrees)
        view.SpeedTest(frames, freeze, direction, angleradians)


    [<EXT>]
    ///<summary>Returns the target location of the specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Point3d) The current target location</returns>
    static member ViewTarget(view:string) : Point3d = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        viewport.CameraTarget

    [<EXT>]
    ///<summary>Sets the target location of the specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="target">(Point3d) 3d point identifying the new target location</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewTarget(view:string, target:Point3d) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        viewport.SetCameraTarget(target, true)
        view.Redraw()



    [<EXT>]
    ///<summary>Returns the name, or title, of a given view's identifier</summary>
    ///<param name="viewId">(Guid) The identifier of the view</param>
    ///<returns>(string) name or title of the view on success</returns>
    static member ViewTitle(viewId:Guid) : string =
        let view = RhinoScriptSyntax.CoerceView(viewId)
        view.MainViewport.Name


    [<EXT>]
    ///<summary>Returns the wallpaper bitmap of the specified view. To remove a
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    /// wallpaper bitmap, pass an empty string ""</summary>
    ///<returns>(string option) The current wallpaper bitmap filename</returns>
    static member Wallpaper(view:string) : string option= //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let f= view.ActiveViewport.WallpaperFilename
        if isNull f then None else Some f

    [<EXT>]
    ///<summary>Sets the wallpaper bitmap of the specified view. To remove a
    /// wallpaper bitmap, pass an empty string ""</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="filename">(string) Name of the bitmap file to set as wallpaper</param>
    ///<returns>(unit) void, nothing</returns>
    static member Wallpaper(view:string, filename:string) : unit = //SET
        let viewo = RhinoScriptSyntax.CoerceView(view)
        let rc = viewo.ActiveViewport.WallpaperFilename
        if not <| viewo.ActiveViewport.SetWallpaper(filename, false) then failwithf "Wallpaper faild to set walleper to %s in view %s" filename view
        viewo.Redraw()



    [<EXT>]
    ///<summary>Returns the grayscale display option of the wallpaper bitmap in a
    /// specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The current grayscale display option</returns>
    static member WallpaperGrayScale(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.WallpaperGrayscale

    [<EXT>]
    ///<summary>Sets the grayscale display option of the wallpaper bitmap in a
    /// specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="grayscale">(bool) Display the wallpaper in gray(True) or color (False)</param>
    ///<returns>(unit) void, nothing</returns>
    static member WallpaperGrayScale(view:string, grayscale:bool) : unit = //SET
        let viewo = RhinoScriptSyntax.CoerceView(view)
        let filename = viewo.ActiveViewport.WallpaperFilename
        if not <| viewo.ActiveViewport.SetWallpaper(filename, grayscale) then failwithf "WallpaperGrayScale faild to set walleper to %s in view %s" filename view
        viewo.Redraw()



    [<EXT>]
    ///<summary>Returns the visibility of the wallpaper bitmap in a specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The current hidden state</returns>
    static member WallpaperHidden(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        not view.ActiveViewport.WallpaperVisible


    [<EXT>]
    ///<summary>Sets the visibility of the wallpaper bitmap in a specified view</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="hidden">(bool) Show or hide the wallpaper</param>
    ///<returns>(unit) void, nothing</returns>
    static member WallpaperHidden(view:string, hidden:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let filename = view.ActiveViewport.WallpaperFilename
        let gray = view.ActiveViewport.WallpaperGrayscale
        view.ActiveViewport.SetWallpaper(filename, gray, not hidden) |> ignore
        view.Redraw()



    [<EXT>]
    ///<summary>Zooms to the extents of a specified bounding box in the specified view</summary>
    ///<param name="boundingBox">(Geometry.BoundingBox) a BoundingBox class instance</param>
    ///<param name="view">(string) Optional, Title of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///Zoom extents in all views</param>
    ///<returns>(unit)</returns>
    static member ZoomBoundingBox( boundingBox:BoundingBox,
                                   [<OPT;DEF("":string)>]view:string,
                                   [<OPT;DEF(false)>]all:bool) : unit =
          if all then
              let views = Doc.Views.GetViewList(true, true)
              for view in views do view.ActiveViewport.ZoomBoundingBox(boundingBox) |> ignore
          else
              let view = RhinoScriptSyntax.CoerceView(view)
              view.ActiveViewport.ZoomBoundingBox(boundingBox) |> ignore
          Doc.Views.Redraw()


    [<EXT>]
    ///<summary>Zooms to extents of visible objects in the specified view</summary>
    ///<param name="view">(string) Optional, Title of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///Zoom extents in all views</param>
    ///<returns>(unit)</returns>
    static member ZoomExtents([<OPT;DEF("":string)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        if  all then
            let views = Doc.Views.GetViewList(true, true)
            for view in views do view.ActiveViewport.ZoomExtents()|> ignore
        else
            let view = RhinoScriptSyntax.CoerceView(view)
            view.ActiveViewport.ZoomExtents()|> ignore
        Doc.Views.Redraw()


    [<EXT>]
    ///<summary>Zoom to extents of selected objects in a view</summary>
    ///<param name="view">(string) Optional, Title of the view. If omitted, active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///Zoom extents in all views</param>
    ///<returns>(unit)</returns>
    static member ZoomSelected([<OPT;DEF("":string)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        if all then
            let views = Doc.Views.GetViewList(true, true)
            for view in views do view.ActiveViewport.ZoomExtentsSelected()|> ignore
        else
            let view = RhinoScriptSyntax.CoerceView(view)
            view.ActiveViewport.ZoomExtentsSelected()|> ignore
        Doc.Views.Redraw()


