namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsView =
  type RhinoScriptSyntax with
    ///<summary>Add new detail view to an existing layout view</summary>
    ///<param name="layoutId">(Guid) Identifier of an existing layout</param>
    ///<param name="corner1">(Point3d) Corner1 of '2d corners of the detail in the layout's unit system' (FIXME 0)</param>
    ///<param name="corner2">(Point3d) Corner2 of '2d corners of the detail in the layout's unit system' (FIXME 0)</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///Title of the new detail</param>
    ///<param name="projection">(float) Optional, Default Value: <c>1</c>
    ///Type of initial view projection for the detail
    ///  1 = parallel top view
    ///  2 = parallel bottom view
    ///  3 = parallel left view
    ///  4 = parallel right view
    ///  5 = parallel front view
    ///  6 = parallel back view
    ///  7 = perspective view</param>
    ///<returns>(Guid) identifier of the newly created detail on success</returns>
    static member AddDetail(layoutId:Guid, corner1:Point3d, corner2:Point3d, [<OPT;DEF(null)>]title:string, [<OPT;DEF(1)>]projection:float) : Guid =
        failNotImpl()

    ///<summary>Adds a new page layout view</summary>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///Title of new layout</param>
    ///<param name="size">(float * float) Optional, Default Value: <c>null</c>
    ///Width and height of paper for the new layout</param>
    ///<returns>(Guid) id of new layout</returns>
    static member AddLayout([<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]size:float * float) : Guid =
        failNotImpl()

    ///<summary>Adds new named construction plane to the document</summary>
    ///<param name="cplaneName">(string) The name of the new named construction plane</param>
    ///<param name="view">(Guid) Optional, Default Value: <c>null</c>
    ///Title or identifier of the view from which to save
    ///  the construction plane. If omitted, the current active view is used.</param>
    ///<returns>(string) name of the newly created construction plane</returns>
    static member AddNamedCPlane(cplaneName:string, [<OPT;DEF(null)>]view:Guid) : string =
        failNotImpl()

    ///<summary>Adds a new named view to the document</summary>
    ///<param name="name">(string) The name of the new named view</param>
    ///<param name="view">(Guid) Optional, Default Value: <c>null</c>
    ///The title or identifier of the view to save. If omitted, the current
    ///  active view is saved</param>
    ///<returns>(string) name fo the newly created named view</returns>
    static member AddNamedView(name:string, [<OPT;DEF(null)>]view:Guid) : string =
        failNotImpl()

    ///<summary>Returns the current detail view in a page layout view</summary>
    ///<param name="layout">(string) Title or identifier of an existing page layout view</param>
    ///<returns>(string) The title or id of the current detail view</returns>
    static member CurrentDetail(layout:string) : string =
        failNotImpl()

    ///<summary>Changes the current detail view in a page layout view</summary>
    ///<param name="layout">(string) Title or identifier of an existing page layout view</param>
    ///<param name="detail">(string)Title or identifier the the detail view to set</param>
    ///<param name="returnName">(bool)Return title if True, else return identifier</param>
    ///<returns>(unit) unit</returns>
    static member CurrentDetail(layout:string, detail:string, [<OPT;DEF(true)>]returnName:bool) : unit =
        failNotImpl()

    ///<summary>Returns the currently active view</summary>
    ///<returns>(string) The title or id of the current view</returns>
    static member CurrentView() : string =
        failNotImpl()

    ///<summary>Sets the currently active view</summary>
    ///<param name="view">(string)Title or id of the view to set current.
    ///  If omitted, only the title or identifier of the current view is returned</param>
    ///<param name="returnName">(bool)If True, then the name, or title, of the view is returned.
    ///  If False, then the identifier of the view is returned</param>
    ///<returns>(unit) unit</returns>
    static member CurrentView(view:string, [<OPT;DEF(true)>]returnName:bool) : unit =
        failNotImpl()

    ///<summary>Removes a named construction plane from the document</summary>
    ///<param name="name">(string) Name of the construction plane to remove</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteNamedCPlane(name:string) : bool =
        failNotImpl()

    ///<summary>Removes a named view from the document</summary>
    ///<param name="name">(string) Name of the named view to remove</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteNamedView(name:string) : bool =
        failNotImpl()

    ///<summary>Returns the projection locked state of a detail</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<returns>(bool) if lock==None, the current detail projection locked state</returns>
    static member DetailLock(detailId:Guid) : bool =
        failNotImpl()

    ///<summary>Modifies the projection locked state of a detail</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<param name="lock">(bool)The new lock state</param>
    ///<returns>(unit) unit</returns>
    static member DetailLock(detailId:Guid, lock:bool) : unit =
        failNotImpl()

    ///<summary>Returns the scale of a detail object</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<returns>(float) current page to model scale ratio if model_length and page_length are both None</returns>
    static member DetailScale(detailId:Guid) : float =
        failNotImpl()

    ///<summary>Modifies the scale of a detail object</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<param name="modelLength">(float)A length in the current model units</param>
    ///<param name="pageLength">(float)A length in the current page units</param>
    ///<returns>(unit) unit</returns>
    static member DetailScale(detailId:Guid, modelLength:float, [<OPT;DEF(null)>]pageLength:float) : unit =
        failNotImpl()

    ///<summary>Verifies that a detail view exists on a page layout view</summary>
    ///<param name="layout">(string) Title or identifier of an existing page layout</param>
    ///<param name="detail">(string) Title or identifier of an existing detail view</param>
    ///<returns>(bool) True if detail is a detail view, False if detail is not a detail view</returns>
    static member IsDetail(layout:string, detail:string) : bool =
        failNotImpl()

    ///<summary>Verifies that a view is a page layout view</summary>
    ///<param name="layout">(Guid) Title or identifier of an existing page layout view</param>
    ///<returns>(bool) True if layout is a page layout view, False is layout is a standard model view</returns>
    static member IsLayout(layout:Guid) : bool =
        failNotImpl()

    ///<summary>Verifies that the specified view exists</summary>
    ///<param name="view">(string) Title or identifier of the view</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member IsView(view:string) : bool =
        failNotImpl()

    ///<summary>Verifies that the specified view is the current, or active view</summary>
    ///<param name="view">(string) Title or identifier of the view</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member IsViewCurrent(view:string) : bool =
        failNotImpl()

    ///<summary>Verifies that the specified view is maximized (enlarged so as to fill
    ///  the entire Rhino window)</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or identifier of the view. If omitted, the current
    ///  view is used</param>
    ///<returns>(bool) True of False</returns>
    static member IsViewMaximized([<OPT;DEF(null)>]view:string) : bool =
        failNotImpl()

    ///<summary>Verifies that the specified view's projection is set to perspective</summary>
    ///<param name="view">(string) Title or identifier of the view</param>
    ///<returns>(bool) True of False</returns>
    static member IsViewPerspective(view:string) : bool =
        failNotImpl()

    ///<summary>Verifies that the specified view's title window is visible</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///The title or identifier of the view. If omitted, the current
    ///  active view is used</param>
    ///<returns>(bool) True of False</returns>
    static member IsViewTitleVisible([<OPT;DEF(null)>]view:string) : bool =
        failNotImpl()

    ///<summary>Verifies that the specified view contains a wallpaper image</summary>
    ///<param name="view">(string) View to verify</param>
    ///<returns>(bool) True or False</returns>
    static member IsWallpaper(view:string) : bool =
        failNotImpl()

    ///<summary>Toggles a view's maximized/restore window state of the specified view</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///The title or identifier of the view. If omitted, the current
    ///  active view is used</param>
    ///<returns>(unit) </returns>
    static member MaximizeRestoreView([<OPT;DEF(null)>]view:string) : unit =
        failNotImpl()

    ///<summary>Returns the plane geometry of the specified named construction plane</summary>
    ///<param name="name">(string) The name of the construction plane</param>
    ///<returns>(Plane) a plane on success</returns>
    static member NamedCPlane(name:string) : Plane =
        failNotImpl()

    ///<summary>Returns the names of all named construction planes in the document</summary>
    ///<returns>(string seq) the names of all named construction planes in the document</returns>
    static member NamedCPlanes() : string seq =
        failNotImpl()

    ///<summary>Returns the names of all named views in the document</summary>
    ///<returns>(string seq) the names of all named views in the document</returns>
    static member NamedViews() : string seq =
        failNotImpl()

    ///<summary>Changes the title of the specified view</summary>
    ///<param name="oldTitle">(string) The title or identifier of the view to rename</param>
    ///<param name="newTitle">(string) The new title of the view</param>
    ///<returns>(unit) unit</returns>
    static member RenameView(oldTitle:string, newTitle:string) : unit =
        failNotImpl()

    ///<summary>Restores a named construction plane to the specified view.</summary>
    ///<param name="cplaneName">(string) Name of the construction plane to restore</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///The title or identifier of the view. If omitted, the current
    ///  active view is used</param>
    ///<returns>(string) name of the restored named construction plane</returns>
    static member RestoreNamedCPlane(cplaneName:string, [<OPT;DEF(null)>]view:string) : string =
        failNotImpl()

    ///<summary>Restores a named view to the specified view</summary>
    ///<param name="namedView">(string) Name of the named view to restore</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view to restore the named view.
    ///  If omitted, the current active view is used</param>
    ///<param name="restoreBitmap">(bool) Optional, Default Value: <c>false</c>
    ///Restore the named view's background bitmap</param>
    ///<returns>(string) name of the restored view</returns>
    static member RestoreNamedView(namedView:string, [<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]restoreBitmap:bool) : string =
        failNotImpl()

    ///<summary>Rotates a perspective-projection view's camera. See the RotateCamera
    ///  command in the Rhino help file for more details</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view. If omitted, current active view is used</param>
    ///<param name="direction">(float) Optional, Default Value: <c>0</c>
    ///The direction to rotate the camera where
    ///  0=right
    ///  1=left
    ///  2=down
    ///  3=up</param>
    ///<param name="angle">(float) Optional, Default Value: <c>null</c>
    ///The angle to rotate. If omitted, the angle of rotation
    ///  is specified by the "Increment in divisions of a circle" parameter
    ///  specified in Options command's View tab</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RotateCamera([<OPT;DEF(null)>]view:string, [<OPT;DEF(0)>]direction:float, [<OPT;DEF(null)>]angle:float) : bool =
        failNotImpl()

    ///<summary>Rotates a view. See RotateView command in Rhino help for more information</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="direction">(float) Optional, Default Value: <c>0</c>
    ///The direction to rotate the view where
    ///  0=right
    ///  1=left
    ///  2=down
    ///  3=up</param>
    ///<param name="angle">(float) Optional, Default Value: <c>null</c>
    ///Angle to rotate. If omitted, the angle of rotation is specified
    ///  by the "Increment in divisions of a circle" parameter specified in
    ///  Options command's View tab</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RotateView([<OPT;DEF(null)>]view:string, [<OPT;DEF(0)>]direction:float, [<OPT;DEF(null)>]angle:float) : bool =
        failNotImpl()

    ///<summary>Get status of a view's construction plane grid</summary>
    ///<returns>(bool) The grid display state</returns>
    static member ShowGrid() : bool =
        failNotImpl()

    ///<summary>Shows or hides a view's construction plane grid</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="show">(bool)The grid state to set. If omitted, the current grid display state is returned</param>
    ///<returns>(unit) unit</returns>
    static member ShowGrid(view:string, [<OPT;DEF(null)>]show:bool) : unit =
        failNotImpl()

    ///<summary>Get status of a view's construction plane grid axes.</summary>
    ///<returns>(bool) The grid axes display state</returns>
    static member ShowGridAxes() : bool =
        failNotImpl()

    ///<summary>Shows or hides a view's construction plane grid axes.</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="show">(bool)The state to set. If omitted, the current grid axes display state is returned</param>
    ///<returns>(unit) unit</returns>
    static member ShowGridAxes(view:string, [<OPT;DEF(null)>]show:bool) : unit =
        failNotImpl()

    ///<summary>Get status of the title window of a view</summary>
    ///<returns>(unit) </returns>
    static member ShowViewTitle() : unit =
        failNotImpl()

    ///<summary>Shows or hides the title window of a view</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="show">(bool)The state to set.</param>
    static member ShowViewTitle(view:string, [<OPT;DEF(true)>]show:bool) : unit =
        failNotImpl()

    ///<summary>Get status of a view's world axis icon</summary>
    ///<returns>(bool) The world axes display state</returns>
    static member ShowWorldAxes() : bool =
        failNotImpl()

    ///<summary>Shows or hides a view's world axis icon</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="show">(bool)The state to set.</param>
    ///<returns>(unit) unit</returns>
    static member ShowWorldAxes(view:string, [<OPT;DEF(null)>]show:bool) : unit =
        failNotImpl()

    ///<summary>Tilts a view by rotating the camera up vector. See the TiltView command in
    ///  the Rhino help file for more details.</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="direction">(float) Optional, Default Value: <c>0</c>
    ///The direction to rotate the view where
    ///  0=right
    ///  1=left</param>
    ///<param name="angle">(float) Optional, Default Value: <c>null</c>
    ///The angle to rotate. If omitted, the angle of rotation is
    ///  specified by the "Increment in divisions of a circle" parameter specified
    ///  in Options command's View tab</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member TiltView([<OPT;DEF(null)>]view:string, [<OPT;DEF(0)>]direction:float, [<OPT;DEF(null)>]angle:float) : bool =
        failNotImpl()

    ///<summary>Returns the camera location of the specified view</summary>
    ///<returns>(Point3d) The current camera location</returns>
    static member ViewCamera() : Point3d =
        failNotImpl()

    ///<summary>Sets the camera location of the specified view</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="cameraLocation">(Point3d)A 3D point identifying the new camera location.
    ///  If omitted, the current camera location is returned</param>
    ///<returns>(unit) unit</returns>
    static member ViewCamera(view:string, [<OPT;DEF(null)>]cameraLocation:Point3d) : unit =
        failNotImpl()

    ///<summary>Returns the 35mm camera lens length of the specified perspective
    /// projection view.</summary>
    ///<returns>(float) The current lens length</returns>
    static member ViewCameraLens() : float =
        failNotImpl()

    ///<summary>Sets the 35mm camera lens length of the specified perspective
    /// projection view.</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="length">(float)The new 35mm camera lens length. If omitted, the previous
    ///  35mm camera lens length is returned</param>
    ///<returns>(unit) unit</returns>
    static member ViewCameraLens(view:string, [<OPT;DEF(null)>]length:float) : unit =
        failNotImpl()

    ///<summary>Returns the orientation of a view's camera.</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view. If omitted, the current active view is used</param>
    ///<returns>(Plane) the view's camera plane</returns>
    static member ViewCameraPlane([<OPT;DEF(null)>]view:string) : Plane =
        failNotImpl()

    ///<summary>Returns the camera and target positions of the specified view</summary>
    ///<returns>(Point3d * Point3d) if both camera and target are not specified, then the 3d points containing
    ///  the current camera and target locations is returned</returns>
    static member ViewCameraTarget() : Point3d * Point3d =
        failNotImpl()

    ///<summary>Sets the camera and target positions of the specified view</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, current active view is used</param>
    ///<param name="camera">(Point3d)3d point identifying the new camera location. If camera and
    ///  target are not specified, current camera and target locations are returned</param>
    ///<param name="target">(Point3d)3d point identifying the new target location. If camera and
    ///  target are not specified, current camera and target locations are returned</param>
    ///<returns>(unit) unit</returns>
    static member ViewCameraTarget(view:string, [<OPT;DEF(null)>]camera:Point3d, [<OPT;DEF(null)>]target:Point3d) : unit =
        failNotImpl()

    ///<summary>Returns the camera up direction of a specified</summary>
    ///<returns>(Vector3d) The current camera up direction</returns>
    static member ViewCameraUp() : Vector3d =
        failNotImpl()

    ///<summary>Sets the camera up direction of a specified</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="upVector">(Vector3d)3D vector identifying the new camera up direction</param>
    ///<returns>(unit) unit</returns>
    static member ViewCameraUp(view:string, [<OPT;DEF(null)>]upVector:Vector3d) : unit =
        failNotImpl()

    ///<summary>Return a view's construction plane</summary>
    ///<returns>(Plane) The current construction plane</returns>
    static member ViewCPlane() : Plane =
        failNotImpl()

    ///<summary>Set a view's construction plane</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, current active view is used.</param>
    ///<param name="plane">(Plane)The new construction plane if setting</param>
    ///<returns>(unit) unit</returns>
    static member ViewCPlane(view:string, [<OPT;DEF(null)>]plane:Plane) : unit =
        failNotImpl()

    ///<summary>Return a view display mode</summary>
    ///<returns>(unit) unit</returns>
    static member ViewDisplayMode() : unit =
        failNotImpl()

    ///<summary>Set a view display mode</summary>
    ///<param name="view">(string)Title or id of a view. If omitted, active view is used</param>
    ///<param name="mode">(string)Name or id of a display mode</param>
    ///<param name="returnName">(bool)If true, return display mode name. If False, display mode id</param>
    ///<returns>(string) If mode is not specified, the current mode</returns>
    static member ViewDisplayMode(view:string, [<OPT;DEF(null)>]mode:string, [<OPT;DEF(true)>]returnName:bool) : string =
        failNotImpl()

    ///<summary>Return id of a display mode given it's name</summary>
    ///<param name="name">(string) Name of the display mode</param>
    ///<returns>(Guid) The id of the display mode , otherwise None</returns>
    static member ViewDisplayModeId(name:string) : Guid =
        failNotImpl()

    ///<summary>Return name of a display mode given it's id</summary>
    ///<param name="modeId">(Guid) The identifier of the display mode obtained from the ViewDisplayModes method.</param>
    ///<returns>(string) The name of the display mode , otherwise None</returns>
    static member ViewDisplayModeName(modeId:Guid) : string =
        failNotImpl()

    ///<summary>Return list of display modes</summary>
    ///<param name="returnNames">(bool) Optional, Default Value: <c>true</c>
    ///If True, return mode names. If False, return ids</param>
    ///<returns>(string seq) strings identifying the display mode names or identifiers</returns>
    static member ViewDisplayModes([<OPT;DEF(true)>]returnNames:bool) : string seq =
        failNotImpl()

    ///<summary>Return the names, titles, or identifiers of all views in the document</summary>
    ///<param name="returnNames">(bool) Optional, Default Value: <c>true</c>
    ///If True then the names of the views are returned.
    ///  If False, then the identifiers of the views are returned</param>
    ///<param name="viewType">(int) Optional, Default Value: <c>0</c>
    ///The type of view to return
    ///  0 = standard model views
    ///  1 = page layout views
    ///  2 = both standard and page layout views</param>
    ///<returns>(string seq) of the view names or identifiers on success</returns>
    static member ViewNames([<OPT;DEF(true)>]returnNames:bool, [<OPT;DEF(0)>]viewType:int) : string seq =
        failNotImpl()

    ///<summary>Return 3d corners of a view's near clipping plane rectangle. Useful
    ///  in determining the "real world" size of a parallel-projected view</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view. If omitted, current active view is used</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four Point3d that define the corners of the rectangle (counter-clockwise order)</returns>
    static member ViewNearCorners([<OPT;DEF(null)>]view:string) : Point3d * Point3d * Point3d * Point3d =
        failNotImpl()

    ///<summary>Return a view's projection mode.</summary>
    ///<returns>(int) The current projection mode for the specified view
    ///  1 = parallel
    ///  2 = perspective
    ///  3 = two point perspective</returns>
    static member ViewProjection() : int =
        failNotImpl()

    ///<summary>Set a view's projection mode.</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, current active view is used</param>
    ///<param name="mode">(float)The projection mode
    ///  1 = parallel
    ///  2 = perspective
    ///  3 = two point perspective</param>
    ///<returns>(unit) unit</returns>
    static member ViewProjection(view:string, [<OPT;DEF(null)>]mode:float) : unit =
        failNotImpl()

    ///<summary>Returns the radius of a parallel-projected view. Useful
    /// when you need an absolute zoom factor for a parallel-projected view</summary>
    ///<returns>(float) The current view radius for the specified view</returns>
    static member ViewRadius() : float =
        failNotImpl()

    ///<summary>Sets the radius of a parallel-projected view. Useful
    /// when you need an absolute zoom factor for a parallel-projected view</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, current active view is used</param>
    ///<param name="radius">(float)The view radius</param>
    ///<param name="mode">(bool)Perform a "dolly" magnification by moving the camera
    ///  towards/away from the target so that the amount of the screen
    ///  subtended by an object changes.  true = perform a "zoom"
    ///  magnification by adjusting the "lens" angle</param>
    ///<returns>(unit) unit</returns>
    static member ViewRadius(view:string, [<OPT;DEF(null)>]radius:float, [<OPT;DEF(false)>]mode:bool) : unit =
        failNotImpl()

    ///<summary>Returns the width and height in pixels of the specified view</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view. If omitted, current active view is used</param>
    ///<returns>(float * float) of two numbers identifying width and height</returns>
    static member ViewSize([<OPT;DEF(null)>]view:string) : float * float =
        failNotImpl()

    ///<summary>Test's Rhino's display performance</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///The title or identifier of the view.  If omitted, the current active view is used</param>
    ///<param name="frames">(float) Optional, Default Value: <c>100</c>
    ///The number of frames, or times to regenerate the view. If omitted, the view will be regenerated 100 times.</param>
    ///<param name="freeze">(bool) Optional, Default Value: <c>true</c>
    ///If True (Default), then Rhino's display list will not be updated with every frame redraw. If False, then Rhino's display list will be updated with every frame redraw.</param>
    ///<param name="direction">(float) Optional, Default Value: <c>0</c>
    ///The direction to rotate the view. The default direction is Right (0). Modes:
    ///  0 = Right
    ///  1 = Left
    ///  2 = Down
    ///  3 = Up.</param>
    ///<param name="angleDegrees">(int) Optional, Default Value: <c>5</c>
    ///The angle to rotate. If omitted, the rotation angle of 5.0 degrees will be used.</param>
    ///<returns>(float) The number of seconds it took to regenerate the view frames number of times,</returns>
    static member ViewSpeedTest([<OPT;DEF(null)>]view:string, [<OPT;DEF(100)>]frames:float, [<OPT;DEF(true)>]freeze:bool, [<OPT;DEF(0)>]direction:float, [<OPT;DEF(5)>]angleDegrees:int) : float =
        failNotImpl()

    ///<summary>Returns the target location of the specified view</summary>
    ///<returns>(Point3d) The current target location</returns>
    static member ViewTarget() : Point3d =
        failNotImpl()

    ///<summary>Sets the target location of the specified view</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, current active view is used</param>
    ///<param name="target">(Point3d)3d point identifying the new target location. If omitted,
    ///  the current target location is returned</param>
    ///<returns>(unit) unit</returns>
    static member ViewTarget(view:string, [<OPT;DEF(null)>]target:Point3d) : unit =
        failNotImpl()

    ///<summary>Returns the name, or title, of a given view's identifier</summary>
    ///<param name="viewId">(string) The identifier of the view</param>
    ///<returns>(string) name or title of the view on success</returns>
    static member ViewTitle(viewId:string) : string =
        failNotImpl()

    ///<summary>Returns the wallpaper bitmap of the specified view. To remove a
    /// wallpaper bitmap, pass an empty string ""</summary>
    ///<returns>(string) The current wallpaper bitmap filename</returns>
    static member Wallpaper() : string =
        failNotImpl()

    ///<summary>Sets the wallpaper bitmap of the specified view. To remove a
    /// wallpaper bitmap, pass an empty string ""</summary>
    ///<param name="view">(string)The identifier of the view. If omitted, the
    ///  active view is used</param>
    ///<param name="filename">(string)Name of the bitmap file to set as wallpaper</param>
    ///<returns>(unit) unit</returns>
    static member Wallpaper(view:string, [<OPT;DEF(null)>]filename:string) : unit =
        failNotImpl()

    ///<summary>Returns the grayscale display option of the wallpaper bitmap in a
    /// specified view</summary>
    ///<returns>(bool) The current grayscale display option</returns>
    static member WallpaperGrayScale() : bool =
        failNotImpl()

    ///<summary>Sets the grayscale display option of the wallpaper bitmap in a
    /// specified view</summary>
    ///<param name="view">(string)The identifier of the view. If omitted, the
    ///  active view is used</param>
    ///<param name="grayscale">(bool)Display the wallpaper in gray(True) or color (False)</param>
    ///<returns>(unit) unit</returns>
    static member WallpaperGrayScale(view:string, [<OPT;DEF(null)>]grayscale:bool) : unit =
        failNotImpl()

    ///<summary>Returns the visibility of the wallpaper bitmap in a specified view</summary>
    ///<returns>(bool) The current hidden state</returns>
    static member WallpaperHidden() : bool =
        failNotImpl()

    ///<summary>Sets the visibility of the wallpaper bitmap in a specified view</summary>
    ///<param name="view">(string)The identifier of the view. If omitted, the
    ///  active view is used</param>
    ///<param name="hidden">(bool)Show or hide the wallpaper</param>
    ///<returns>(unit) unit</returns>
    static member WallpaperHidden(view:string, [<OPT;DEF(null)>]hidden:bool) : unit =
        failNotImpl()

    ///<summary>Zooms to the extents of a specified bounding box in the specified view</summary>
    ///<param name="boundingBox">(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) Eight points that define the corners
    ///  of a bounding box or a BoundingBox class instance</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///Zoom extents in all views</param>
    ///<returns>(unit) </returns>
    static member ZoomBoundingBox(boundingBox:Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d, [<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        failNotImpl()

    ///<summary>Zooms to extents of visible objects in the specified view</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///Zoom extents in all views</param>
    ///<returns>(unit) </returns>
    static member ZoomExtents([<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        failNotImpl()

    ///<summary>Zoom to extents of selected objects in a view</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or id of the view. If omitted, active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///Zoom extents in all views</param>
    ///<returns>(unit) </returns>
    static member ZoomSelected([<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        failNotImpl()

