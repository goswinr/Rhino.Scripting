namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsSelection =
  type RhinoScriptSyntax with
    ///<summary>Returns identifiers of all objects in the document.</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grips objects</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///Include refrence objects such as work session objects</param>
    ///<returns>(Guid seq) identifiers for all the objects in the document</returns>
    static member AllObjects([<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool, [<OPT;DEF(false)>]includeReferences:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns identifier of the first object in the document. The first
    ///  object is the last object created by the user.</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the object.  If omitted (False), the object is not selected.</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects.  If omitted (False), light objects are not returned.</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grips objects.  If omitted (False), grips objects are not returned.</param>
    ///<returns>(Guid) The identifier of the object .</returns>
    static member FirstObject([<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid =
        failNotImpl()

    ///<summary>A helper Function for Rhino.DocObjects.ObjectType Enum</summary>
    ///<param name="filter">(int) Int representing one or several Enums as used ion Rhinopython for object types.</param>
    ///<returns>(Rhino.DocObjects.ObjectType) translated Rhino.DocObjects.ObjectType Enum</returns>
    static member __FilterHelper(filter:int) : Rhino.DocObjects.ObjectType =
        failNotImpl()

    ///<summary>Prompts user to pick or select a single curve object</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message.</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked objects. If False, objects that
    ///  are picked are not selected.</param>
    ///<returns>(Guid * bool * int * Point3d * float * string) Tuple containing the following information
    ///  [0]  guid     identifier of the curve object
    ///  [1]  bool     True if the curve was preselected, otherwise False
    ///  [2]  number   selection method
    ///    0 = selected by non-mouse method (SelAll, etc.).
    ///    1 = selected by mouse click on the object.
    ///    2 = selected by being inside of a mouse window.
    ///    3 = selected by intersecting a mouse crossing window.
    ///  [3]  point    selection point
    ///  [4]  number   the curve parameter of the selection point
    ///  [5]  str      name of the view selection was made</returns>
    static member GetCurveObject([<OPT;DEF(null)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid * bool * int * Point3d * float * string =
        failNotImpl()

    ///<summary>Prompts user to pick, or select, a single object.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message.</param>
    ///<param name="filter">(float) Optional, Default Value: <c>0</c>
    ///The type(s) of geometry (points, curves, surfaces, meshes,...)
    ///  that can be selected. Object types can be added together to filter
    ///  several different kinds of geometry. use the filter class to get values</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked objects.  If False, the objects that are
    ///  picked are not selected.</param>
    ///<param name="customFilter">(obj->unit) Optional, Default Value: <c>null</c>
    ///A custom filter function</param>
    ///<param name="subobjects">(bool) Optional, Default Value: <c>false</c>
    ///If True, subobjects can be selected. When this is the
    ///  case, an ObjRef is returned instead of a Guid to allow for tracking
    ///  of the subobject when passed into other functions</param>
    ///<returns>(Guid) Identifier of the picked object</returns>
    static member GetObject([<OPT;DEF(null)>]message:string, [<OPT;DEF(0)>]filter:float, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null)>]customFilter:obj->unit, [<OPT;DEF(false)>]subobjects:bool) : Guid =
        failNotImpl()

    ///<summary>Prompts user to pick or select one or more objects.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message.</param>
    ///<param name="filter">(float) Optional, Default Value: <c>0</c>
    ///The type(s) of geometry (points, curves, surfaces, meshes,...)
    ///  that can be selected. Object types can be added together to filter
    ///  several different kinds of geometry. use the filter class to get values
    ///    Value         Description
    ///    0             All objects (default)
    ///    1             Point
    ///    2             Point cloud
    ///    4             Curve
    ///    8             Surface or single-face brep
    ///    16            Polysurface or multiple-face
    ///    32            Mesh
    ///    256           Light
    ///    512           Annotation
    ///    4096          Instance or block reference
    ///    8192          Text dot object
    ///    16384         Grip object
    ///    32768         Detail
    ///    65536         Hatch
    ///    131072        Morph control
    ///    134217728     Cage
    ///    268435456     Phantom
    ///    536870912     Clipping plane
    ///    1073741824    Extrusion</param>
    ///<param name="group">(bool) Optional, Default Value: <c>true</c>
    ///Honor object grouping.  If omitted and the user picks a group,
    ///  the entire group will be picked (True). Note, if filter is set to a
    ///  value other than 0 (All objects), then group selection will be disabled.</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked objects.  If False, the objects that are
    ///  picked are not selected.</param>
    ///<param name="objects">(Guid seq) Optional, Default Value: <c>null</c>
    ///List of objects that are allowed to be selected</param>
    ///<param name="minimumCount">(int) Optional, Default Value: <c>1</c>
    ///Minimum count of 'limits on number of objects allowed to be selected' (FIXME 0)</param>
    ///<param name="maximumCount">(int) Optional, Default Value: <c>0</c>
    ///Maximum count of 'limits on number of objects allowed to be selected' (FIXME 0)</param>
    ///<param name="customFilter">(string) Optional, Default Value: <c>null</c>
    ///Calls a custom function in the script and passes the Rhino Object, Geometry, and component index and returns true or false indicating if the object can be selected</param>
    ///<returns>(Guid seq) identifiers of the picked objects</returns>
    static member GetObjects([<OPT;DEF(null)>]message:string, [<OPT;DEF(0)>]filter:float, [<OPT;DEF(true)>]group:bool, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null)>]objects:Guid seq, [<OPT;DEF(1)>]minimumCount:int, [<OPT;DEF(0)>]maximumCount:int, [<OPT;DEF(null)>]customFilter:string) : Guid seq =
        failNotImpl()

    ///<summary>Prompts user to pick, or select one or more objects</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message.</param>
    ///<param name="filter">(float) Optional, Default Value: <c>0</c>
    ///The type(s) of geometry (points, curves, surfaces, meshes,...)
    ///  that can be selected. Object types can be added together to filter
    ///  several different kinds of geometry. use the filter class to get values</param>
    ///<param name="group">(bool) Optional, Default Value: <c>true</c>
    ///Honor object grouping.  If omitted and the user picks a group,
    ///  the entire group will be picked (True). Note, if filter is set to a
    ///  value other than 0 (All objects), then group selection will be disabled.</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked objects. If False, the objects that are
    ///  picked are not selected.</param>
    ///<param name="objects">(Guid seq) Optional, Default Value: <c>null</c>
    ///List of object identifiers specifying objects that are
    ///  allowed to be selected</param>
    ///<returns>((Guid*bool*int*Point3d*string) seq) containing the following information
    ///  [n][0]  identifier of the object
    ///  [n][1]  True if the object was preselected, otherwise False
    ///  [n][2]  selection method (see help)
    ///  [n][3]  selection point
    ///  [n][4]  name of the view selection was made</returns>
    static member GetObjectsEx([<OPT;DEF(null)>]message:string, [<OPT;DEF(0)>]filter:float, [<OPT;DEF(true)>]group:bool, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null)>]objects:Guid seq) : (Guid*bool*int*Point3d*string) seq =
        failNotImpl()

    ///<summary>Prompts the user to select one or more point objects.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Select points"</c>
    ///A prompt message.</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.  If omitted (False), pre-selected objects are not accepted.</param>
    ///<returns>(Point3d seq) 3d coordinates of point objects on success</returns>
    static member GetPointCoordinates([<OPT;DEF("Select points")>]message:string, [<OPT;DEF(false)>]preselect:bool) : Point3d seq =
        failNotImpl()

    ///<summary>Prompts the user to select a single surface</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Select surface"</c>
    ///Prompt displayed</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for preselected objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked object</param>
    ///<returns>(Guid * bool * float * Point3d * (float * float) * string) of information on success
    ///  [0]  identifier of the surface
    ///  [1]  True if the surface was preselected, otherwise False
    ///  [2]  selection method ( see help )
    ///  [3]  selection point
    ///  [4]  u,v surface parameter of the selection point
    ///  [5]  name of the view in which the selection was made</returns>
    static member GetSurfaceObject([<OPT;DEF("Select surface")>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid * bool * float * Point3d * (float * float) * string =
        failNotImpl()

    ///<summary>Returns identifiers of all locked objects in the document. Locked objects
    ///  cannot be snapped to, and cannot be selected</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grip objects</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///Include refrence objects such as work session objects</param>
    ///<returns>(Guid seq) identifiers the locked objects .</returns>
    static member LockedObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool, [<OPT;DEF(false)>]includeReferences:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns identifiers of all hidden objects in the document. Hidden objects
    ///  are not visible, cannot be snapped to, and cannot be selected</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grip objects</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///Include refrence objects such as work session objects</param>
    ///<returns>(Guid seq) identifiers of the hidden objects .</returns>
    static member HiddenObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool, [<OPT;DEF(false)>]includeReferences:bool) : Guid seq =
        failNotImpl()

    ///<summary>Inverts the current object selection. The identifiers of the newly
    ///  selected objects are returned</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects.  If omitted (False), light objects are not returned.</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grips objects.  If omitted (False), grips objects are not returned.</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///Include refrence objects such as work session objects</param>
    ///<returns>(Guid seq) identifiers of the newly selected objects .</returns>
    static member InvertSelectedObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool, [<OPT;DEF(false)>]includeReferences:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns identifiers of the objects that were most recently created or changed
    ///  by scripting a Rhino command using the Command function. It is important to
    ///  call this function immediately after calling the Command function as only the
    ///  most recently created or changed object identifiers will be returned</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the object.  If omitted (False), the object is not selected.</param>
    ///<returns>(Guid seq) identifiers of the most recently created or changed objects .</returns>
    static member LastCreatedObjects([<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns the identifier of the last object in the document. The last object
    ///  in the document is the first object created by the user</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the object</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include lights in the potential set</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grips in the potential set</param>
    ///<returns>(Guid) identifier of the object on success</returns>
    static member LastObject([<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid =
        failNotImpl()

    ///<summary>Returns the identifier of the next object in the document</summary>
    ///<param name="objectId">(Guid) The identifier of the object from which to get the next object</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the object</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include lights in the potential set</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grips in the potential set</param>
    ///<returns>(Guid) identifier of the object on success</returns>
    static member NextObject(objectId:Guid, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid =
        failNotImpl()

    ///<summary>Returns identifiers of all normal objects in the document. Normal objects
    ///  are visible, can be snapped to, and are independent of selection state</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects.  If omitted (False), light objects are not returned.</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grips objects.  If omitted (False), grips objects are not returned.</param>
    ///<returns>(Guid seq) identifier of normal objects .</returns>
    static member NormalObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns identifiers of all objects based on color</summary>
    ///<param name="color">(Drawing.Color) Color to get objects by</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include lights in the set</param>
    ///<returns>(Guid seq) identifiers of objects of the selected color.</returns>
    static member ObjectsByColor(color:Drawing.Color, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns identifiers of all objects based on the objects' group name</summary>
    ///<param name="groupName">(string) Name of the group</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<returns>(Guid seq) identifiers for objects in the group on success</returns>
    static member ObjectsByGroup(groupName:string, [<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns identifiers of all objects based on the objects' layer name</summary>
    ///<param name="layerName">(string) Name of the layer</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<returns>(Guid seq) identifiers for objects in the specified layer</returns>
    static member ObjectsByLayer(layerName:string, [<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns identifiers of all objects based on user-assigned name</summary>
    ///<param name="name">(string) Name of the object or objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///Include refrence objects such as work session objects</param>
    ///<returns>(Guid seq) identifiers for objects with the specified name.</returns>
    static member ObjectsByName(name:string, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeReferences:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns identifiers of all objects based on the objects' geometry type.</summary>
    ///<param name="geometryType">(int) The type(s) of geometry objects (points, curves, surfaces,
    ///  meshes, etc.) that can be selected. Object types can be
    ///  added together as bit-coded flags to filter several different kinds of geometry.
    ///    Value        Description
    ///      0           All objects
    ///      1           Point
    ///      2           Point cloud
    ///      4           Curve
    ///      8           Surface or single-face brep
    ///      16          Polysurface or multiple-face
    ///      32          Mesh
    ///      256         Light
    ///      512         Annotation
    ///      4096        Instance or block reference
    ///      8192        Text dot object
    ///      16384       Grip object
    ///      32768       Detail
    ///      65536       Hatch
    ///      131072      Morph control
    ///      134217728   Cage
    ///      268435456   Phantom
    ///      536870912   Clipping plane
    ///      1073741824  Extrusion</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<param name="state">(bool) Optional, Default Value: <c>0</c>
    ///The object state (normal, locked, and hidden). Object states can be
    ///  added together to filter several different states of geometry.
    ///    Value     Description
    ///    0         All objects
    ///    1         Normal objects
    ///    2         Locked objects
    ///    4         Hidden objects</param>
    ///<returns>(Guid seq) identifiers of object that fit the specified type(s).</returns>
    static member ObjectsByType(geometryType:int, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(0)>]state:bool) : Guid seq =
        failNotImpl()

    ///<summary>Returns the identifiers of all objects that are currently selected</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grip objects</param>
    ///<returns>(Guid seq) identifiers of selected objects</returns>
    static member SelectedObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid seq =
        failNotImpl()

    ///<summary>Unselects all objects in the document</summary>
    ///<returns>(float) the number of objects that were unselected</returns>
    static member UnselectAllObjects() : float =
        failNotImpl()

    ///<summary>Return identifiers of all objects that are visible in a specified view</summary>
    ///<param name="view">(bool) Optional, Default Value: <c>null</c>
    ///The view to use. If omitted, the current active view is used</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grip objects</param>
    ///<returns>(Guid seq) identifiers of the visible objects</returns>
    static member VisibleObjects([<OPT;DEF(null)>]view:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid seq =
        failNotImpl()

    ///<summary>Picks objects using either a window or crossing selection</summary>
    ///<param name="corner1">(Point3d) Corner1 of 'corners of selection window' (FIXME 0)</param>
    ///<param name="corner2">(Point3d) Corner2 of 'corners of selection window' (FIXME 0)</param>
    ///<param name="view">(bool) Optional, Default Value: <c>null</c>
    ///View to perform the selection in</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select picked objects</param>
    ///<param name="inWindow">(bool) Optional, Default Value: <c>true</c>
    ///If False, then a crossing window selection is performed</param>
    ///<returns>(Guid seq) identifiers of selected objects on success</returns>
    static member WindowPick(corner1:Point3d, corner2:Point3d, [<OPT;DEF(null)>]view:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(true)>]inWindow:bool) : Guid seq =
        failNotImpl()

