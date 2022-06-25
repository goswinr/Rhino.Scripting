
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenViews =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    ///<summary>Add new detail view to an existing layout view.</summary>
    ///<param name="layoutName">(string) Name of an existing layout</param>
    ///<param name="corner1">(Point2d) Corner1 of the detail in the layout's unit system</param>
    ///<param name="corner2">(Point2d) Corner2 of the detail in the layout's unit system</param>
    ///<param name="title">(string) Optional, Title of the new detail</param>
    ///<param name="projection">(int) Optional, Default Value: <c>1</c>
    ///    Type of initial view projection for the detail
    ///    1 = parallel top view
    ///    2 = parallel bottom view
    ///    3 = parallel left view
    ///    4 = parallel right view
    ///    5 = parallel front view
    ///    6 = parallel back view
    ///    7 = perspective view</param>
    ///<returns>(Guid) identifier of the newly created detail.</returns>
    static member AddDetail( layoutName:string,
                             corner1:Point2d,
                             corner2:Point2d,
                             [<OPT;DEF(null:string)>]title:string,
                             [<OPT;DEF(1)>]projection:int) : Guid = 
        if projection<1 || projection>7 then RhinoScriptingException.Raise "Rhino.Scripting.Projection must be a value between 1-7.  layoutId:'%s' corner1:'%A' corner2:'%A' title:'%A' projection:'%A'" layoutName corner1 corner2 title projection
        let layout = Scripting.CoercePageView(layoutName)//TODO test this
        if isNull layout then RhinoScriptingException.Raise "Rhino.Scripting.No layout found for given layoutId.  layoutId:'%s' corner1:'%A' corner2:'%A' title:'%A' projection:'%A'" layoutName corner1 corner2 title projection
        let projection : Display.DefinedViewportProjection = LanguagePrimitives.EnumOfValue  projection
        let detail = layout.AddDetailView(title, corner1, corner2, projection)
        if isNull detail then RhinoScriptingException.Raise "Rhino.Scripting.AddDetail failed.  layoutId:'%s' corner1:'%A' corner2:'%A' title:'%A' projection:'%A'" layoutName corner1 corner2 title projection
        State.Doc.Views.Redraw()
        detail.Id


    ///<summary>Adds a new page layout view.</summary>
    ///<param name="title">(string) Optional, Title of new layout</param>
    ///<param name="width">(float)  Optional, width  of paper for the new layout</param>
    ///<param name="height">(float) Optional, height of paper for the new layout</param>
    ///<returns>(Guid*string) Id and Name of new layout.</returns>
    static member AddLayout([<OPT;DEF(null:string)>]title:string,
                            [<OPT;DEF(0.0)>]width:float,
                            [<OPT;DEF(0.0)>]height:float) : Guid*string = 
        let page = 
            if width=0.0 || height=0.0  then State.Doc.Views.AddPageView(title)
            else                             State.Doc.Views.AddPageView(title, width, height)
        if notNull page then page.MainViewport.Id, page.PageName
        else RhinoScriptingException.Raise "Rhino.Scripting.AddLayout failed for %A %A" title (width, height)


    ///<summary>Adds new named construction Plane to the document.</summary>
    ///<param name="cplaneName">(string) The name of the new named construction Plane</param>
    ///<param name="plane">(Plane) The construction Plane</param>
    ///<returns>(unit) void, nothing.</returns>
    static member AddNamedCPlane(cplaneName:string, plane:Plane) : unit = 
        if isNull cplaneName then RhinoScriptingException.Raise "Rhino.Scripting.CplaneName = null.  cplaneName:'%A' plane:'%A'" cplaneName plane
        let index = State.Doc.NamedConstructionPlanes.Add(cplaneName, plane)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.AddNamedCPlane failed.  cplaneName:'%A' plane:'%A'" cplaneName plane
        ()


    ///<summary>Adds a new named view to the document.</summary>
    ///<param name="name">(string) The name of the new named view</param>
    ///<param name="view">(string) Optional, The title of the view to save. If omitted, the current
    ///    active view is saved</param>
    ///<returns>(unit) void, nothing.</returns>
    static member AddNamedView(name:string, [<OPT;DEF("")>]view:string) : unit = 
        let view = Scripting.CoerceView(view)
        if isNull name then RhinoScriptingException.Raise "Rhino.Scripting.Name = empty.  name:'%A' view:'%A'" name view
        let viewportId = view.MainViewport.Id
        let index = State.Doc.NamedViews.Add(name, viewportId)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.AddNamedView failed.  name:'%A' view:'%A'" name view



    ///<summary>Returns the current detail view in a page layout view.</summary>
    ///<param name="layout">(string) Title of an existing page layout view</param>
    ///<returns>(string option) Option of The name  the current detail view, None if Page is current view.</returns>
    static member CurrentDetail(layout:string) : string option = //GET
        let page = Scripting.CoercePageView(layout)
        if page.MainViewport.Id = page.ActiveViewport.Id then None
        else  Some  page.ActiveViewport.Name

    ///<summary>Changes the current detail view in a page layout view.</summary>
    ///<param name="layout">(string) Title of an existing page layout view</param>
    ///<param name="detail">(string) Title of the detail view to set</param>
    ///<returns>(unit) void, nothing.</returns>
    static member CurrentDetail(layout:string, detail:string) : unit = //SET
        let page = Scripting.CoercePageView(layout)
        if layout = detail then page.SetPageAsActive()
        else
            if not <| page.SetActiveDetail(detail, compareCase=false) then
                RhinoScriptingException.Raise "Rhino.Scripting.CurrentDetail set failed for %s to %s" layout detail
        State.Doc.Views.Redraw()



    ///<summary>Returns the currently active view.</summary>
    ///<returns>(string) The title of the current view.</returns>
    static member CurrentView() : string = //GET
        State.Doc.Views.ActiveView.MainViewport.Name

    ///<summary>Sets the currently active view.</summary>
    ///<param name="view">(string) Title of the view to set current</param>
    ///<returns>(unit) void, nothing.</returns>
    static member CurrentView(view:string) : unit = //SET
        State.Doc.Views.ActiveView <- Scripting.CoerceView(view)



    ///<summary>Removes a named construction Plane from the document.</summary>
    ///<param name="name">(string) Name of the construction Plane to remove</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member DeleteNamedCPlane(name:string) : bool = 
        State.Doc.NamedConstructionPlanes.Delete(name)


    ///<summary>Removes a named view from the document.</summary>
    ///<param name="name">(string) Name of the named view to remove</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member DeleteNamedView(name:string) : bool = 
        State.Doc.NamedViews.Delete(name)


    ///<summary>Returns the projection locked state of a detail view-port rectangle.</summary>
    ///<param name="detailId">(Guid) Identifier of a detail rectangle object</param>
    ///<returns>(bool) The current detail projection locked state.</returns>
    static member DetailLock(detailId:Guid) : bool = //GET
        let detail = Scripting.CoerceDetailView(detailId)
        detail.IsProjectionLocked

    ///<summary>Modifies the projection locked state of a detail.</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<param name="lock">(bool) The new lock state</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DetailLock(detailId:Guid, lock:bool) : unit = //SET
        let detail = 
            try State.Doc.Objects.FindId(detailId) :?> DocObjects.DetailViewObject
            with _ ->  RhinoScriptingException.Raise "Rhino.Scripting.Set DetailLock failed. detailId is a %s  lock:'%A'" (Print.guid detailId)  lock
        if lock <> detail.DetailGeometry.IsProjectionLocked then
            detail.DetailGeometry.IsProjectionLocked <- lock
            detail.CommitChanges() |> ignore



    ///<summary>Returns the scale of a detail object.</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<returns>(float) current page to model scale ratio if model Length and page Length are both None.</returns>
    static member DetailScale(detailId:Guid) : float = //GET
        let detail = Scripting.CoerceDetailViewObject(detailId)
        detail.DetailGeometry.PageToModelRatio

    ///<summary>Modifies the scale of a detail object.</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<param name="modelLength">(float) A length in the current model units</param>
    ///<param name="pageLength">(float) A length in the current page units</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DetailScale(detailId:Guid, modelLength:float, pageLength:float) : unit = //SET
        let detail = Scripting.CoerceDetailViewObject(detailId)
        let modelunits = State.Doc.ModelUnitSystem
        let pageunits = State.Doc.PageUnitSystem
        if detail.DetailGeometry.SetScale(modelLength, modelunits, pageLength, pageunits) then
            detail.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.DetailScale : CommitChanges failed"
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.DetailScale failed.  detailId:'%s' modelLength:'%A' pageLength:'%A'" (Print.guid detailId) modelLength pageLength



    ///<summary>Checks if a detail view exists on a page layout view.</summary>
    ///<param name="layout">(string) Title of an existing page layout</param>
    ///<param name="detail">(string) Title of an existing detail view</param>
    ///<returns>(bool) True if detail is a detail view, False if detail is not a detail view.</returns>
    static member IsDetail(layout:string, detail:string) : bool = 
        let view = Scripting.CoercePageView(layout)
        let det = Scripting.CoerceView(detail)
        view.GetDetailViews()
        |> Array.exists (fun v -> v.Name = det.ActiveViewport.Name) // TODO test


    ///<summary>Checks if a view is a page layout view.</summary>
    ///<param name="layout">(string) Title of an existing page layout view</param>
    ///<returns>(bool) True if layout is a page layout view, False is layout is a standard model view.</returns>
    static member IsLayout(layout:string) : bool = 
        //layout id = Scripting.Coerceguid(layout)
        if   State.Doc.Views.GetViewList(includeStandardViews=false,includePageViews=true) |> Array.exists (fun v -> v.MainViewport.Name = layout) then
            true
        elif State.Doc.Views.GetViewList(includeStandardViews=true ,includePageViews=false) |> Array.exists (fun v -> v.MainViewport.Name = layout) then
            false
        else
            RhinoScriptingException.Raise "Rhino.Scripting.IsLayout View does not exist at all.  layout:'%A'" layout // or false


    ///<summary>Checks if the specified view exists.</summary>
    ///<param name="view">(string) Title of the view</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member IsView(view:string) : bool = 
        State.Doc.Views.GetViewList(includeStandardViews=false, includePageViews=true) |> Array.exists ( fun v -> v.MainViewport.Name = view)



    ///<summary>Checks if the specified view is the current, or active view.</summary>
    ///<param name="view">(string) Title of the view</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member IsViewCurrent(view:string) : bool = 
        let activeview = State.Doc.Views.ActiveView
        view = activeview.MainViewport.Name


    ///<summary>Checks if the specified view is maximized (enlarged so as to fill
    ///    the entire Rhino window).</summary>
    ///<param name="view">(string) Optional, Title of the view. If omitted, the current
    ///    view is used</param>
    ///<returns>(bool) True of False.</returns>
    static member IsViewMaximized([<OPT;DEF("")>]view:string) : bool = 
        let view = Scripting.CoerceView(view)
        view.Maximized


    ///<summary>Checks if the specified view's projection is set to perspective.</summary>
    ///<param name="view">(string) Title of the view</param>
    ///<returns>(bool) True of False.</returns>
    static member IsViewPerspective(view:string) : bool = 
        let view = Scripting.CoerceView(view)
        view.MainViewport.IsPerspectiveProjection


    ///<summary>Checks if the specified view's title window is visible.</summary>
    ///<param name="view">(string) Optional, The title of the view. If omitted, the current
    ///    active view is used</param>
    ///<returns>(bool) True of False.</returns>
    static member IsViewTitleVisible([<OPT;DEF("")>]view:string) : bool = 
        let view = Scripting.CoerceView(view)
        view.TitleVisible


    ///<summary>Checks if the specified view contains a wallpaper image.</summary>
    ///<param name="view">(string) View to verify</param>
    ///<returns>(bool) True or False.</returns>
    static member IsWallpaper(view:string) : bool = 
        let view = Scripting.CoerceView(view)
        view.MainViewport.WallpaperFilename.Length > 0


    ///<summary>Toggles a view's maximized/restore window state of the specified view.</summary>
    ///<param name="view">(string) Optional, The title of the view. If omitted, the current
    ///    active view is used</param>
    ///<returns>(unit).</returns>
    static member MaximizeRestoreView([<OPT;DEF("")>]view:string) : unit = 
        let view = Scripting.CoerceView(view)
        view.Maximized <- not view.Maximized


    ///<summary>Returns the Plane geometry of the specified named construction Plane.</summary>
    ///<param name="name">(string) The name of the construction Plane</param>
    ///<returns>(Plane) a Plane.</returns>
    static member NamedCPlane(name:string) : Plane = 
        let index = State.Doc.NamedConstructionPlanes.Find(name)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.NamedCPlane failed.  name:'%A'" name
        State.Doc.NamedConstructionPlanes.[index].Plane


    ///<summary>Returns the names of all named construction Planes in the document.</summary>
    ///<returns>(string Rarr) The names of all named construction Planes in the document.</returns>
    static member NamedCPlanes() : string Rarr = 
        let count = State.Doc.NamedConstructionPlanes.Count
        rarr {for i = 0 to count - 1 do State.Doc.NamedConstructionPlanes.[i].Name }



    ///<summary>Returns the names of all named views in the document.</summary>
    ///<returns>(string Rarr) The names of all named views in the document.</returns>
    static member NamedViews() : string Rarr = 
        let count = State.Doc.NamedViews.Count
        rarr {for i = 0 to count - 1 do State.Doc.NamedViews.[i].Name }


    ///<summary>Changes the title of the specified view.</summary>
    ///<param name="oldTitle">(string) The title of the view to rename</param>
    ///<param name="newTitle">(string) The new title of the view</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenameView(oldTitle:string, newTitle:string) : unit = 
        if isNull oldTitle || isNull newTitle then RhinoScriptingException.Raise "Rhino.Scripting.RenameView failed.  oldTitle:'%A' newTitle:'%A'" oldTitle newTitle
        let foundview = Scripting.CoerceView(oldTitle)
        foundview.MainViewport.Name <- newTitle



    ///<summary>Restores a named construction Plane to the specified view.</summary>
    ///<param name="cplaneName">(string) Name of the construction Plane to restore</param>
    ///<param name="view">(string) Optional, The title of the view. If omitted, the current
    ///    active view is used</param>
    ///<returns>(string) name of the restored named construction Plane.</returns>
    static member RestoreNamedCPlane(cplaneName:string, [<OPT;DEF("")>]view:string) : string = 
        let view = Scripting.CoerceView(view)
        let index = State.Doc.NamedConstructionPlanes.Find(cplaneName)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.RestoreNamedCPlane failed.  cplaneName:'%A' view:'%A'" cplaneName view
        let cplane = State.Doc.NamedConstructionPlanes.[index]
        view.MainViewport.PushConstructionPlane(cplane)
        view.Redraw()
        cplaneName


    ///<summary>Restores a named view to the specified view.</summary>
    ///<param name="namedView">(string) Name of the named view to restore</param>
    ///<param name="view">(string) Optional, Title of the view to restore the named view.
    ///    If omitted, the current active view is used</param>
    ///<param name="restoreBitmap">(bool) Optional, Default Value: <c>false</c>
    ///    Restore the named view's background bitmap</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RestoreNamedView( namedView:string,
                                    [<OPT;DEF("")>]view:string,
                                    [<OPT;DEF(false)>]restoreBitmap:bool) : unit = 
        let view = Scripting.CoerceView(view)
        let index = State.Doc.NamedViews.FindByName(namedView)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.RestoreNamedView failed.  namedView:'%A' view:'%A' restoreBitmap:'%A'" namedView view restoreBitmap
        let viewinfo = State.Doc.NamedViews.[index]
        if view.MainViewport.PushViewInfo(viewinfo, restoreBitmap) then
            view.Redraw()
            //view.MainViewport.Name
        else
            RhinoScriptingException.Raise "Rhino.Scripting.RestoreNamedView failed.  namedView:'%A' view:'%A' restoreBitmap:'%A'" namedView view restoreBitmap


    ///<summary>Rotates a perspective-projection view's camera. See the RotateCamera
    ///    command in the Rhino help file for more details.</summary>
    ///<param name="direction">(int)
    ///    The direction to rotate the camera where
    ///    0 = right
    ///    1 = left
    ///    2 = down
    ///    3 = up</param>
    ///<param name="angle">(float) The angle to rotate</param>
    ///<param name="view">(string) Optional, Title of the view. If omitted, current active view is used</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RotateCamera( direction:int,
                                angle:float,
                                [<OPT;DEF("")>]view:string) : unit = 
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable angle = RhinoMath.ToRadians( abs(angle))
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



    ///<summary>Rotates a view. See RotateView command in Rhino help for more information.</summary>
    ///<param name="direction">(int) Optional, The direction to rotate the view where
    ///    0 = right
    ///    1 = left
    ///    2 = down
    ///    3 = up</param>
    ///<param name="angle">(float) Angle to rotate. If omitted, the angle of rotation is specified
    ///    by the "Increment in divisions of a circle" parameter specified in
    ///    Options command's View tab</param>
    ///<param name="view">(string) Optional, Title of the view. If omitted, the current active view is used</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RotateView( direction:int,
                              angle:float,
                              [<OPT;DEF("")>]view:string) : unit = 
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable angle =  RhinoMath.ToRadians( abs(angle))
        if ApplicationSettings.ViewSettings.RotateReverseKeyboard then angle <- -angle
        if direction = 0 then viewport.KeyboardRotate(true, angle)       |> ignore
        elif direction = 1 then viewport.KeyboardRotate(true, -angle)    |> ignore
        elif direction = 2 then viewport.KeyboardRotate(false, -angle)   |> ignore
        elif direction = 3 then viewport.KeyboardRotate(false, angle)    |> ignore
        view.Redraw()


    ///<summary>Get status of a view's construction Plane grid.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The grid display state.</returns>
    static member ShowGrid(view:string) : bool = //GET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        viewport.ConstructionGridVisible

    ///<summary>Shows or hides a view's construction Plane grid.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="show">(bool) The grid state to set</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ShowGrid(view:string, show:bool) : unit = //SET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.ConstructionGridVisible
        if  rc <> show then
            viewport.ConstructionGridVisible <- show
            view.Redraw()



    ///<summary>Get status of a view's construction Plane grid axes.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The grid axes display state.</returns>
    static member ShowGridAxes(view:string) : bool = //GET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.ConstructionAxesVisible
        rc

    ///<summary>Shows or hides a view's construction Plane grid axes.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="show">(bool) The state to set</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ShowGridAxes(view:string, show:bool) : unit = //SET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.ConstructionAxesVisible
        if  rc <> show then
            viewport.ConstructionAxesVisible <- show
            view.Redraw()



    ///<summary>Get status of the title window of a view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The state to View Title visibility.</returns>
    static member ShowViewTitle(view:string) : bool = //GET
        let view = Scripting.CoerceView(view)
        view.TitleVisible

    ///<summary>Shows or hides the title window of a view.</summary>
    ///<param name="view">(string) Title of the view. If omitted, the current active view is used</param>
    ///<param name="show">(bool) The state to set</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ShowViewTitle(view:string, show:bool) : unit = //SET
        let view = Scripting.CoerceView(view)
        view.TitleVisible <- show


    ///<summary>Get status of a view's world axis icon.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The world axes display state.</returns>
    static member ShowWorldAxes(view:string) : bool = //GET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.WorldAxesVisible
        rc

    ///<summary>Shows or hides a view's world axis icon.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="show">(bool) The state to set</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ShowWorldAxes(view:string, show:bool) : unit = //SET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.WorldAxesVisible
        if rc <> show then
            viewport.WorldAxesVisible <- show
            view.Redraw()



    ///<summary>Tilts a view by rotating the camera up vector. See the TiltView command in
    ///    the Rhino help file for more details.</summary>
    ///<param name="direction">(int) The direction to rotate the view where
    ///    0 = right
    ///    1 = left</param>
    ///<param name="angle">(float) The angle in degrees to rotate</param>
    ///<param name="view">(string) Optional, Title of the view. If omitted, the current active view is used</param>
    ///<returns>(unit) void, nothing.</returns>
    static member TiltView( direction:int,
                            angle:float,
                            [<OPT;DEF("")>]view:string) : unit = 
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable angle = angle
        if ApplicationSettings.ViewSettings.RotateReverseKeyboard then angle <- -angle
        let axis = viewport.CameraLocation - viewport.CameraTarget
        if direction = 0 then viewport.Rotate(angle, axis, viewport.CameraLocation) |> ignore
        elif direction = 1 then viewport.Rotate(-angle, axis, viewport.CameraLocation)   |> ignore
        view.Redraw()



    ///<summary>Returns the camera location of the specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Point3d) The current camera location.</returns>
    static member ViewCamera(view:string) : Point3d = //GET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.CameraLocation

    ///<summary>Sets the camera location of the specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="cameraLocation">(Point3d) A 3D point identifying the new camera location</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ViewCamera(view:string, cameraLocation:Point3d) : unit = //SET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.SetCameraLocation(cameraLocation, true)
        view.Redraw()



    ///<summary>Returns the 35mm camera lens length of the specified perspective
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    /// projection view.</summary>
    ///<returns>(float) The current lens length.</returns>
    static member ViewCameraLens(view:string) : float = //GET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.Camera35mmLensLength

    ///<summary>Sets the 35mm camera lens length of the specified perspective
    /// projection view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="length">(float) The new 35mm camera lens length</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ViewCameraLens(view:string, length:float) : unit = //SET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.Camera35mmLensLength <- length
        view.Redraw()



    ///<summary>Returns the orientation of a view's camera.</summary>
    ///<param name="view">(string) Optional, Title of the view. If omitted, the current active view is used</param>
    ///<returns>(Plane) The view's camera Plane.</returns>
    static member ViewCameraPlane([<OPT;DEF("")>]view:string) : Plane = 
        let view = Scripting.CoerceView(view)
        let rc, frame = view.ActiveViewport.GetCameraFrame()
        if not rc then RhinoScriptingException.Raise "Rhino.Scripting.ViewCameraPlane failed.  view:'%A'" view
        frame


    ///<summary>Returns the camera and target positions of the specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Point3d * Point3d) The 3d points containing the current camera and target locations.</returns>
    static member ViewCameraTarget(view:string) : Point3d * Point3d = //GET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.CameraLocation, view.ActiveViewport.CameraTarget

    ///<summary>Sets the camera and target positions of the specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="camera">(Point3d) 3d point identifying the new camera location</param>
    ///<param name="target">(Point3d) 3d point identifying the new target location</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ViewCameraTarget(view:string, camera:Point3d, target:Point3d) : unit = //SET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.SetCameraLocations(target, camera)
        view.Redraw()


    ///<summary>Returns the camera up direction of a specified.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Vector3d) The current camera up direction.</returns>
    static member ViewCameraUp(view:string) : Vector3d = //GET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.CameraUp

    ///<summary>Sets the camera up direction of a specified.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="upVector">(Vector3d) 3D vector identifying the new camera up direction</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ViewCameraUp(view:string, upVector:Vector3d) : unit = //SET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.CameraUp <- upVector
        view.Redraw()



    ///<summary>Return a view's construction Plane.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Plane) The current construction Plane.</returns>
    static member ViewCPlane(view:string) : Plane = //GET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.ConstructionPlane()

    ///<summary>Set a view's construction Plane.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="plane">(Plane) The new construction Plane if setting</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ViewCPlane(view:string, plane:Plane) : unit = //SET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.SetConstructionPlane(plane)
        view.Redraw()



    ///<summary>Return a view display mode.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(string) The current mode.</returns>
    static member ViewDisplayMode(view:string) : string = //GET
        let view = Scripting.CoerceView(view)
        let current = view.ActiveViewport.DisplayMode
        current.EnglishName

    ///<summary>Set a view display mode.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="mode">(string) Name of a display mode</param>
    ///<returns>(string) If mode is not specified, the current mode.</returns>
    static member ViewDisplayMode(view:string, mode:string) : unit = //SET
        let view = Scripting.CoerceView(view)
        let desc = Display.DisplayModeDescription.FindByName(mode)
        if notNull desc then
            view.ActiveViewport.DisplayMode <- desc
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.ViewDisplayMode set mode %s not found." mode



    ///<summary>Return id of a display mode given it's name.</summary>
    ///<param name="name">(string) Name of the display mode</param>
    ///<returns>(Guid) The id of the display mode.</returns>
    static member ViewDisplayModeId(name:string) : Guid = 
        let desc = Display.DisplayModeDescription.FindByName(name)
        if notNull desc then desc.Id
        else
            RhinoScriptingException.Raise "Rhino.Scripting.ViewDisplayModeId set mode %s not found." name


    ///<summary>Return name of a display mode given it's id.</summary>
    ///<param name="modeId">(Guid) The identifier of the display mode obtained from the ViewDisplayModes method</param>
    ///<returns>(string) The name of the display mode.</returns>
    static member ViewDisplayModeName(modeId:Guid) : string = 
        //modeId = Scripting.Coerceguid(modeId)
        let desc = Display.DisplayModeDescription.GetDisplayMode(modeId)
        if notNull desc then desc.EnglishName
        else
            RhinoScriptingException.Raise "Rhino.Scripting.ViewDisplayModeName set Id %A not found." modeId


    ///<summary>Return list of display modes.</summary>
    ///<returns>(string Rarr) strings identifying the display mode names.</returns>
    static member ViewDisplayModes() : string Rarr = 
        let modes = Display.DisplayModeDescription.GetDisplayModes()
        rarr {for mode in modes do mode.EnglishName }


    ///<summary>Return the names/titles, of all views in the document.</summary>
    ///<param name="viewType">(int) Optional, Default Value: <c>0</c>. The standard model views.
    ///    The type of view to return
    ///    0 = standard model views
    ///    1 = page layout views
    ///    2 = both standard and page layout views</param>
    ///<returns>(string Rarr) List of the view names.</returns>
    static member ViewNames([<OPT;DEF(0)>]viewType:int) : string Rarr = 
        let views = State.Doc.Views.GetViewList(viewType <> 1, viewType>0)
        if views|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.ViewNames failed. viewType:'%A'" viewType
        rarr { for view in views do view.MainViewport.Name}



    ///<summary>Return 3d corners of a view's near clipping Plane rectangle. Useful
    ///    in determining the "real world" size of a parallel-projected view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four Point3d that define the corners of the rectangle (counter-clockwise order).</returns>
    static member ViewNearCorners([<OPT;DEF("")>]view:string) : Point3d * Point3d * Point3d * Point3d = 
        let view = Scripting.CoerceView(view)
        let rc = view.ActiveViewport.GetNearRect()
        rc.[0], rc.[1], rc.[3], rc.[2]


    ///<summary>Return a view's projection mode.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(int) The current projection mode for the specified view
    ///    1 = parallel
    ///    2 = perspective
    ///    3 = two point perspective.</returns>
    static member ViewProjection(view:string) : int = //GET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable rc = 2
        if viewport.IsParallelProjection then rc <- 1
        elif viewport.IsTwoPointPerspectiveProjection then rc <- 3
        rc

    ///<summary>Set a view's projection mode.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="mode">(int) The projection mode
    ///    1 = parallel
    ///    2 = perspective
    ///    3 = two point perspective</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ViewProjection(view:string, mode:int) : unit = //SET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        if mode = 1 then viewport.ChangeToParallelProjection(true) |> ignore
        elif mode = 2 then viewport.ChangeToPerspectiveProjection(true, 35.)|> ignore
        elif mode = 3 then viewport.ChangeToTwoPointPerspectiveProjection(35.)       |> ignore
        view.Redraw()



    ///<summary>Returns the radius of a parallel-projected view. Useful
    /// when you need an absolute zoom factor for a parallel-projected view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(float) The current view radius for the specified view.</returns>
    static member ViewRadius(view:string) : float = //GET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        if not viewport.IsParallelProjection then RhinoScriptingException.Raise "Rhino.Scripting.ViewRadius view is not ParallelProjection.  view:'%A'" view
        let ok, a, b, c, d, e, f = viewport.GetFrustum()
        let frusright = b
        let frustop = d
        let oldradius = min frustop frusright
        oldradius

    ///<summary>Sets the radius of a parallel-projected view. Useful
    /// when you need an absolute zoom factor for a parallel-projected view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="radius">(float) The view radius</param>
    ///<param name="mode">(bool) Perform a "dolly" magnification by moving the camera
    ///    towards/away from the target so that the amount of the screen
    ///    subtended by an object changes. true = perform a "zoom"
    ///    magnification by adjusting the "lens" angle</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ViewRadius(view:string, radius:float, mode:bool) : unit = //SET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        if not viewport.IsParallelProjection then RhinoScriptingException.Raise "Rhino.Scripting.ViewRadius view is not ParallelProjection.  view:'%A'" view
        let ok, a, b, c, d, e, f = viewport.GetFrustum()
        let frusright = b
        let frustop = d
        let oldradius = min frustop frusright
        let magnificationfactor = radius / oldradius
        let d = 1.0 / magnificationfactor
        viewport.Magnify(d, mode) |> ignore
        view.Redraw()



    ///<summary>Returns the width and height in pixels of the specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(int * int ) of two numbers identifying width and height.</returns>
    static member ViewSize([<OPT;DEF(null:string)>]view:string) : int * int = 
        let view = Scripting.CoerceView(view)
        let cr = view.ClientRectangle
        cr.Width, cr.Height


    ///<summary>Test's Rhino's display performance.</summary>
    ///<param name="view">(string) Optional, The title of the view. If omitted, the current active view is used</param>
    ///<param name="frames">(int) Optional, Default Value: <c>100</c>
    ///    The number of frames, or times to regenerate the view. If omitted, the view will be regenerated 100 times</param>
    ///<param name="freeze">(bool) Optional, Default Value: <c>true</c>
    ///    If True (Default), then Rhino's display list will not be updated with every frame redraw. If False, then Rhino's display list will be updated with every frame redraw</param>
    ///<param name="direction">(int) Optional, Default Value: <c>0</c>
    ///    The direction to rotate the view. The default direction is Right (0). Modes:
    ///    0 = Right
    ///    1 = Left
    ///    2 = Down
    ///    3 = Up</param>
    ///<param name="angleDegrees">(float) Optional, Default Value: <c>5</c>
    ///    The angle to rotate. If omitted, the rotation angle of 5.0 degrees will be used</param>
    ///<returns>(float) The number of seconds it took to regenerate the view frames number of times.</returns>
    static member ViewSpeedTest( [<OPT;DEF("")>]view:string,
                                 [<OPT;DEF(100)>]frames:int,
                                 [<OPT;DEF(true)>]freeze:bool,
                                 [<OPT;DEF(0)>]direction:int,
                                 [<OPT;DEF(5.0)>]angleDegrees:float) : float = 
        let view = Scripting.CoerceView(view)
        let angleradians = toRadians(angleDegrees)
        view.SpeedTest(frames, freeze, direction, angleradians)


    ///<summary>Returns the target location of the specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(Point3d) The current target location.</returns>
    static member ViewTarget(view:string) : Point3d = //GET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        viewport.CameraTarget

    ///<summary>Sets the target location of the specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="target">(Point3d) 3d point identifying the new target location</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ViewTarget(view:string, target:Point3d) : unit = //SET
        let view = Scripting.CoerceView(view)
        let viewport = view.ActiveViewport
        viewport.SetCameraTarget(target, true)
        view.Redraw()



    ///<summary>Returns the name, or title, of a given view's identifier.</summary>
    ///<param name="viewId">(Guid) The identifier of the view</param>
    ///<returns>(string) name or title of the view.</returns>
    static member ViewTitle(viewId:Guid) : string = 
        let view = Scripting.CoerceView(viewId)
        view.MainViewport.Name


    ///<summary>Returns the wallpaper bitmap of the specified view. To remove a
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    /// wallpaper bitmap, pass an empty string "".</summary>
    ///<returns>(string option) The current wallpaper bitmap filename.</returns>
    static member Wallpaper(view:string) : string option= //GET
        let view = Scripting.CoerceView(view)
        let f= view.ActiveViewport.WallpaperFilename
        if isNull f then None else Some f

    ///<summary>Sets the wallpaper bitmap of the specified view. To remove a
    /// wallpaper bitmap, pass an empty string "".</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="filename">(string) Name of the bitmap file to set as wallpaper</param>
    ///<returns>(unit) void, nothing.</returns>
    static member Wallpaper(view:string, filename:string) : unit = //SET
        let viewo = Scripting.CoerceView(view)
        let rc = viewo.ActiveViewport.WallpaperFilename
        if not <| viewo.ActiveViewport.SetWallpaper(filename, grayscale=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.Wallpaper failed to set wallpaper to %s in view %s" filename view
        viewo.Redraw()


    ///<summary>Returns the gray-scale display option of the wallpaper bitmap in a
    /// specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The current gray-scale display option.</returns>
    static member WallpaperGrayScale(view:string) : bool = //GET
        let view = Scripting.CoerceView(view)
        view.ActiveViewport.WallpaperGrayscale

    ///<summary>Sets the gray-scale display option of the wallpaper bitmap in a
    /// specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="grayscale">(bool) Display the wallpaper in gray(True) or color (False)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member WallpaperGrayScale(view:string, grayscale:bool) : unit = //SET
        let viewo = Scripting.CoerceView(view)
        let filename = viewo.ActiveViewport.WallpaperFilename
        if not <| viewo.ActiveViewport.SetWallpaper(filename, grayscale) then
            RhinoScriptingException.Raise "Rhino.Scripting.WallpaperGrayScale failed to set wallpaper to %s in view %s" filename view
        viewo.Redraw()



    ///<summary>Returns the visibility of the wallpaper bitmap in a specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<returns>(bool) The current hidden state.</returns>
    static member WallpaperHidden(view:string) : bool = //GET
        let view = Scripting.CoerceView(view)
        not view.ActiveViewport.WallpaperVisible


    ///<summary>Sets the visibility of the wallpaper bitmap in a specified view.</summary>
    ///<param name="view">(string) Title of the view. Use "" empty string for the current active view</param>
    ///<param name="hidden">(bool) Show or hide the wallpaper</param>
    ///<returns>(unit) void, nothing.</returns>
    static member WallpaperHidden(view:string, hidden:bool) : unit = //SET
        let view = Scripting.CoerceView(view)
        let filename = view.ActiveViewport.WallpaperFilename
        let gray = view.ActiveViewport.WallpaperGrayscale
        view.ActiveViewport.SetWallpaper(filename, gray, not hidden) |> ignore
        view.Redraw()



    ///<summary>Zooms to the extents of a specified bounding box in the specified view.</summary>
    ///<param name="boundingBox">(Geometry.BoundingBox) a BoundingBox class instance</param>
    ///<param name="view">(string) Optional, Title of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///    Zoom extents in all views</param>
    ///<returns>(unit).</returns>
    static member ZoomBoundingBox( boundingBox:BoundingBox,
                                   [<OPT;DEF("")>]view:string,
                                   [<OPT;DEF(false)>]all:bool) : unit = 
          if all then
              let views = State.Doc.Views.GetViewList(true, true)
              for view in views do view.ActiveViewport.ZoomBoundingBox(boundingBox) |> ignore
          else
              let view = Scripting.CoerceView(view)
              view.ActiveViewport.ZoomBoundingBox(boundingBox) |> ignore
          State.Doc.Views.Redraw()


    ///<summary>Zooms to extents of visible objects in the specified view.</summary>
    ///<param name="view">(string) Optional, Title of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///    Zoom extents in all views</param>
    ///<returns>(unit).</returns>
    static member ZoomExtents([<OPT;DEF("")>]view:string, [<OPT;DEF(false)>]all:bool) : unit = 
        if  all then
            let views = State.Doc.Views.GetViewList(true, true)
            for view in views do view.ActiveViewport.ZoomExtents()|> ignore
        else
            let view = Scripting.CoerceView(view)
            view.ActiveViewport.ZoomExtents()|> ignore
        State.Doc.Views.Redraw()


    ///<summary>Zoom to extents of selected objects in a view.</summary>
    ///<param name="view">(string) Optional, Title of the view. If omitted, active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///    Zoom extents in all views</param>
    ///<returns>(unit).</returns>
    static member ZoomSelected([<OPT;DEF("")>]view:string, [<OPT;DEF(false)>]all:bool) : unit = 
        if all then
            let views = State.Doc.Views.GetViewList(true, true)
            for view in views do view.ActiveViewport.ZoomExtentsSelected()|> ignore
        else
            let view = Scripting.CoerceView(view)
            view.ActiveViewport.ZoomExtentsSelected()|> ignore
        State.Doc.Views.Redraw()


