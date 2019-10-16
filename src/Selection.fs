namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsSelection =

  type RhinoScriptSyntax with

    
    [<EXT>]
    ///<summary>Returns identifiers of all objects in the document.</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grips objects</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///Include refrence objects such as work session objects</param>
    ///<returns>(Guid ResizeArray) identifiers for all the objects in the document</returns>
    static member AllObjects(       [<OPT;DEF(false)>]select:bool, 
                                    [<OPT;DEF(false)>]includeLights:bool, 
                                    [<OPT;DEF(false)>]includeGrips:bool, 
                                    [<OPT;DEF(false)>]includeReferences:bool) : Guid ResizeArray =        
            
            let it = Rhino.DocObjects.ObjectEnumeratorSettings()
            it.IncludeLights <- includeLights
            it.IncludeGrips <- includeGrips
            it.NormalObjects <- true
            it.LockedObjects <- true
            it.HiddenObjects <- true
            it.ReferenceObjects <- includeReferences
            let e = Doc.Objects.GetObjectList(it)
            let objectids = ResizeArray()
            for object in e do
                if select then object.Select(true) |> ignore
                objectids.Add(object.Id)
            if objectids.Count > 0 && select then Doc.Views.Redraw()
            objectids
            
    (*
    def AllObjects(select=False, include_lights=False, include_grips=False, include_references=False):
        '''Returns identifiers of all objects in the document.
        Parameters:
          select(bool, optional): Select the objects
          include_lights (bool, optional): Include light objects
          include_grips (bool, optional): Include grips objects
          include_references(bool, optional): Include refrence objects such as work session objects
        Returns:
          list(guid, ...): identifiers for all the objects in the document
        '''
    
        it = Rhino.DocObjects.ObjectEnumeratorSettings()
        it.IncludeLights = include_lights
        it.IncludeGrips = include_grips
        it.NormalObjects = True
        it.LockedObjects = True
        it.HiddenObjects = True
        it.ReferenceObjects = include_references
        e = scriptcontext.doc.Objects.GetObjectList(it)
        object_ids = []
        for object in e:
            if select: object.Select(True)
            object_ids.append(object.Id)
        if object_ids and select: scriptcontext.doc.Views.Redraw()
        return object_ids
    *)


    [<EXT>]
    ///<summary>Returns identifier of the first object in the document. The first
    ///  object is the last object created by the user.</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the object.  If omitted (False), the object is not selected.</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects.  If omitted (False), light objects are not returned.</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grips objects.  If omitted (False), grips objects are not returned.</param>
    ///<returns>(Guid) The identifier of the object .</returns>
    static member FirstObject(      [<OPT;DEF(false)>]select:bool, 
                                    [<OPT;DEF(false)>]includeLights:bool, 
                                    [<OPT;DEF(false)>]includeGrips:bool) : Guid =
        
            let it = Rhino.DocObjects.ObjectEnumeratorSettings()
            it.IncludeLights <- includeLights
            it.IncludeGrips <- includeGrips
            let e = Doc.Objects.GetObjectList(it).GetEnumerator()
            if not <| e.MoveNext() then failwithf "FirstObject not found"
            let object = e.Current
            if isNull object then failwithf "FirstObject not found(null)"
            if select then object.Select(true) |> ignore
            object.Id
            
    (*
    def FirstObject(select=False, include_lights=False, include_grips=False):
        '''Returns identifier of the first object in the document. The first
        object is the last object created by the user.
        Parameters:
          select (bool, optional): Select the object.  If omitted (False), the object is not selected.
          include_lights (bool, optional): Include light objects.  If omitted (False), light objects are not returned.
          include_grips (bool, optional): Include grips objects.  If omitted (False), grips objects are not returned.
        Returns:
          guid: The identifier of the object if successful.
        '''
    
        it = Rhino.DocObjects.ObjectEnumeratorSettings()
        it.IncludeLights = include_lights
        it.IncludeGrips = include_grips
        e = scriptcontext.doc.Objects.GetObjectList(it).GetEnumerator()
        if not e.MoveNext(): return None
        object = e.Current
        if object:
            if select: object.Select(True)
            return object.Id
    *)


    [<EXT>]
    ///<summary>A helper Function for Rhino.DocObjects.ObjectType Enum</summary>
    ///<param name="filter">(int) Int representing one or several Enums as used ion Rhinopython for object types.</param>
    ///<returns>(Rhino.DocObjects.ObjectType) translated Rhino.DocObjects.ObjectType Enum</returns>
    static member FilterHelper(filter:int) : Rhino.DocObjects.ObjectType =        //TODO make internal ?
        let mutable geometryfilter = Rhino.DocObjects.ObjectType.None
        if 0 <> (filter &&& 1 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Point
        if 0 <> (filter &&& 16384 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Grip
        if 0 <> (filter &&& 2 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.PointSet
        if 0 <> (filter &&& 4 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Curve
        if 0 <> (filter &&& 8 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Surface
        if 0 <> (filter &&& 16 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Brep
        if 0 <> (filter &&& 32 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Mesh
        if 0 <> (filter &&& 512 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Annotation
        if 0 <> (filter &&& 256 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Light
        if 0 <> (filter &&& 4096 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.InstanceReference
        if 0 <> (filter &&& 134217728 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Cage
        if 0 <> (filter &&& 65536 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Hatch
        if 0 <> (filter &&& 131072 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.MorphControl
        if 0 <> (filter &&& 2097152 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.PolysrfFilter
        if 0 <> (filter &&& 268435456 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Phantom
        if 0 <> (filter &&& 8192 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.TextDot
        if 0 <> (filter &&& 32768 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Detail
        if 0 <> (filter &&& 536870912 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.ClipPlane
        if 0 <> (filter &&& 1073741824 ) then
            geometryfilter  <- geometryfilter ||| Rhino.DocObjects.ObjectType.Extrusion
        geometryfilter
           
    (*
    def __FilterHelper(filter):
        '''A helper Function for Rhino.DocObjects.ObjectType Enum
        Parameters:
          filter (int): int representing one or several Enums as used ion Rhinopython for object types.
        Returns:
          Rhino.DocObjects.ObjectType: translated Rhino.DocObjects.ObjectType Enum
        '''
    
        geometry_filter = Rhino.DocObjects.ObjectType.None
        if filter & 1:
            geometry_filter |= Rhino.DocObjects.ObjectType.Point
        if filter & 16384:
            geometry_filter |= Rhino.DocObjects.ObjectType.Grip
        if filter & 2:
            geometry_filter |= Rhino.DocObjects.ObjectType.PointSet
        if filter & 4:
            geometry_filter |= Rhino.DocObjects.ObjectType.Curve
        if filter & 8:
            geometry_filter |= Rhino.DocObjects.ObjectType.Surface
        if filter & 16:
            geometry_filter |= Rhino.DocObjects.ObjectType.Brep
        if filter & 32:
            geometry_filter |= Rhino.DocObjects.ObjectType.Mesh
        if filter & 512:
            geometry_filter |= Rhino.DocObjects.ObjectType.Annotation
        if filter & 256:
            geometry_filter |= Rhino.DocObjects.ObjectType.Light
        if filter & 4096:
            geometry_filter |= Rhino.DocObjects.ObjectType.InstanceReference
        if filter & 134217728:
            geometry_filter |= Rhino.DocObjects.ObjectType.Cage
        if filter & 65536:
            geometry_filter |= Rhino.DocObjects.ObjectType.Hatch
        if filter & 131072:
            geometry_filter |= Rhino.DocObjects.ObjectType.MorphControl
        if filter & 2097152:
            geometry_filter |= Rhino.DocObjects.ObjectType.PolysrfFilter
        if filter & 268435456:
            geometry_filter |= Rhino.DocObjects.ObjectType.Phantom
        if filter & 8192:
            geometry_filter |= Rhino.DocObjects.ObjectType.TextDot
        if filter & 32768:
            geometry_filter |= Rhino.DocObjects.ObjectType.Detail
        if filter & 536870912:
            geometry_filter |= Rhino.DocObjects.ObjectType.ClipPlane
        if filter & 1073741824:
            geometry_filter |= Rhino.DocObjects.ObjectType.Extrusion
        return geometry_filter
    *)


    [<EXT>]
    ///<summary>Prompts user to pick or select a single curve object</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
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
    static member GetCurveObject(   [<OPT;DEF(null:string)>]message:string, 
                                    [<OPT;DEF(false)>]preselect:bool, 
                                    [<OPT;DEF(false)>]select:bool) : option<Guid * bool * int * Point3d * float * string> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            if not <| preselect then
                Doc.Objects.UnselectAll() |> ignore
                Doc.Views.Redraw()
            let go = new Input.Custom.GetObject()
            if notNull message then go.SetCommandPrompt(message)
            go.GeometryFilter <- Rhino.DocObjects.ObjectType.Curve
            go.SubObjectSelect <- false
            go.GroupSelect <- false
            go.AcceptNothing(true)
            return
                if go.Get() <> Rhino.Input.GetResult.Object then None
                else
                    let objref = go.Object(0)
                    let id = objref.ObjectId
                    let presel = go.ObjectsWerePreselected
                    let mutable selmethod = 0
                    let sm = objref.SelectionMethod()
                    if Rhino.DocObjects.SelectionMethod.MousePick = sm then selmethod <- 1
                    elif Rhino.DocObjects.SelectionMethod.WindowBox = sm then selmethod <- 2
                    elif Rhino.DocObjects.SelectionMethod.CrossingBox = sm then selmethod <- 3
                    let point = objref.SelectionPoint()
                    let crv, curveparameter = objref.CurveParameter()
                    let viewname = go.View().ActiveViewport.Name
                    let obj = go.Object(0).Object()
                    go.Dispose()
                    if not <| select && not <| preselect then
                        Doc.Objects.UnselectAll()|> ignore
                        Doc.Views.Redraw()
                    obj.Select(select)  |> ignore
                    Some (id, presel, selmethod, point, curveparameter, viewname)
            } |> Async.RunSynchronously
    (*
    def GetCurveObject(message=None, preselect=False, select=False):
        '''Prompts user to pick or select a single curve object
        Parameters:
          message (str, optional): a prompt or message.
          preselect (bool, optional): Allow for the selection of pre-selected objects.
          select (bool, optional): Select the picked objects. If False, objects that
            are picked are not selected.
        Returns:
          (guid,bool,int,point,number,str): Tuple containing the following information
            [0]  guid     identifier of the curve object
            [1]  bool     True if the curve was preselected, otherwise False
            [2]  number   selection method
                             0 = selected by non-mouse method (SelAll, etc.).
                             1 = selected by mouse click on the object.
                             2 = selected by being inside of a mouse window.
                             3 = selected by intersecting a mouse crossing window.
            [3]  point    selection point
            [4]  number   the curve parameter of the selection point
            [5]  str      name of the view selection was made
          None: if no object picked
        '''
    
        if not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        go = Rhino.Input.Custom.GetObject()
        if message: go.SetCommandPrompt(message)
        go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve
        go.SubObjectSelect = False
        go.GroupSelect = False
        go.AcceptNothing(True)
        if go.Get()!=Rhino.Input.GetResult.Object: return None
    
        objref = go.Object(0)
        id = objref.ObjectId
        presel = go.ObjectsWerePreselected
        selmethod = 0
        sm = objref.SelectionMethod()
        if Rhino.DocObjects.SelectionMethod.MousePick==sm: selmethod = 1
        elif Rhino.DocObjects.SelectionMethod.WindowBox==sm: selmethod = 2
        elif Rhino.DocObjects.SelectionMethod.CrossingBox==sm: selmethod = 3
        point = objref.SelectionPoint()
        crv, curve_parameter = objref.CurveParameter()
        viewname = go.View().ActiveViewport.Name
        obj = go.Object(0).Object()
        go.Dispose()
        if not select and not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        obj.Select(select)
        return id, presel, selmethod, point, curve_parameter, viewname
    *)


    [<EXT>]
    ///<summary>Prompts user to pick, or select, a single object.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message.</param>
    ///<param name="filter">(int) Optional, Default Value: <c>0</c>
    ///The type(s) of geometry (points, curves, surfaces, meshes,...)
    ///  that can be selected. Object types can be added together to filter
    ///  several different kinds of geometry. use the RhinoScriptSyntax.Filter enum to get values, they can be joinded with '+'</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked objects.  If False, the objects that are
    ///  picked are not selected.</param>
    ///<param name="customFilter">(Input.Custom.GetObjectGeometryFilter) Optional, A custom filter function</param>
    ///<param name="subobjects">(bool) Optional, Default Value: <c>false</c>
    ///If True, subobjects can be selected. When this is the
    ///  case, for tracking  of the subobject go via the Object Ref</param>
    ///<returns>(Guid) Identifier of the picked object</returns>
    static member GetObject(        [<OPT;DEF(null:string)>]message:string, 
                                    [<OPT;DEF(0)>]filter:int, 
                                    [<OPT;DEF(false)>]preselect:bool, 
                                    [<OPT;DEF(false)>]select:bool, 
                                    [<OPT;DEF(null:Input.Custom.GetObjectGeometryFilter)>]customFilter:Input.Custom.GetObjectGeometryFilter, 
                                    [<OPT;DEF(false)>]subobjects:bool) : option<Guid> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            if not  preselect then
                Doc.Objects.UnselectAll() |> ignore
                Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull customFilter then go.SetCustomGeometryFilter(customFilter)
            if notNull message then go.SetCommandPrompt(message)
            let geometryfilter = RhinoScriptSyntax.FilterHelper(filter)
            if filter>0 then go.GeometryFilter <- geometryfilter
            go.SubObjectSelect <- subobjects
            go.GroupSelect <- false
            go.AcceptNothing(true)
            return 
                if go.Get() <> Rhino.Input.GetResult.Object then None
                else
                    let objref = go.Object(0)
                    let obj = objref.Object()
                    let presel = go.ObjectsWerePreselected
                    go.Dispose()
                    if not <| select && not <| preselect then
                        Doc.Objects.UnselectAll() |> ignore
                        Doc.Views.Redraw()
                    
                    obj.Select(select)  |> ignore
                    Some obj.Id

            } |> Async.RunSynchronously
    (*
    def GetObject(message=None, filter=0, preselect=False, select=False, custom_filter=None, subobjects=False):
        '''Prompts user to pick, or select, a single object.
        Parameters:
          message(str, optional): a prompt or message.
          filter (number, optional): The type(s) of geometry (points, curves, surfaces, meshes,...)
              that can be selected. Object types can be added together to filter
              several different kinds of geometry. use the filter class to get values
          preselect (bool, optional): Allow for the selection of pre-selected objects.
          select (bool, optional): Select the picked objects.  If False, the objects that are
              picked are not selected.
          custom_filter (function, optional): a custom filter function
          subobjects (bool, optional): If True, subobjects can be selected. When this is the
              case, an ObjRef is returned instead of a Guid to allow for tracking
              of the subobject when passed into other functions
        Returns:
          guid: Identifier of the picked object
          None: if user did not pick an object
        '''
    
        if not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
    
        class CustomGetObject(Rhino.Input.Custom.GetObject):
            def __init__(self, filter_function):
                self.m_filter_function = filter_function
            def CustomGeometryFilter( self, rhino_object, geometry, component_index ):
                rc = True
                if self.m_filter_function is not None:
                    try:
                        rc = self.m_filter_function(rhino_object, geometry, component_index)
                    except:
                        rc = True
                return rc
        go = CustomGetObject(custom_filter)
        if message: go.SetCommandPrompt(message)
        geometry_filter = __FilterHelper(filter)
        if filter>0: go.GeometryFilter = geometry_filter
        go.SubObjectSelect = subobjects
        go.GroupSelect = False
        go.AcceptNothing(True)
        if go.Get()!=Rhino.Input.GetResult.Object: return None
        objref = go.Object(0)
        obj = objref.Object()
        presel = go.ObjectsWerePreselected
        go.Dispose()
        if not select and not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        if subobjects: return objref
        obj.Select(select)
        return obj.Id
    *)

    [<EXT>]
    //(FIXME) VarOutTypes
    ///<summary>Prompts user to pick, or select a single object</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message.</param>
    ///<param name="filter">(int) Optional, Default Value: <c>0</c>
    ///The type(s) of geometry (points, curves, surfaces, meshes,...)
    ///  that can be selected. Object types can be added together to filter
    ///  several different kinds of geometry. use the filter class to get values</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked objects.  If False, the objects that are
    ///  picked are not selected.</param>
    ///<param name="objects">(Guid seq) Optional, Default Value: <c>null:Guid seq</c>
    ///List of object identifiers specifying objects that are
    ///  allowed to be selected</param>
    ///<returns>(Guid * bool * float * Point3d * string) containing the following information
    ///  [0] identifier of the object
    ///  [1] True if the object was preselected, otherwise False
    ///  [2] selection method
    ///       (0) selected by non-mouse method (SelAll,etc.).    
    ///       (1) selected by mouse click on theobject.    
    ///       (2) selected by being inside of amouse window.    
    ///       (3) selected by intersecting a mousecrossing window.    
    ///  [3] selection point
    ///  [4] name of the view selection was made</returns>
    static member GetObjectEx(      [<OPT;DEF(null:string)>]message:string, 
                                    [<OPT;DEF(0)>]filter:int, 
                                    [<OPT;DEF(false)>]preselect:bool, 
                                    [<OPT;DEF(false)>]select:bool, 
                                    [<OPT;DEF(null:Guid seq)>]objects:Guid seq) : option<Guid * bool * int * Point3d * string> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            if not <| preselect then
                Doc.Objects.UnselectAll() |> ignore
                Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull objects then
                let s = System.Collections.Generic.HashSet(objects)
                go.SetCustomGeometryFilter(fun rhinoobject _ _ -> s.Contains(rhinoobject.Id))
            if notNull message then 
                go.SetCommandPrompt(message)            
            if filter>0 then 
                go.GeometryFilter <- RhinoScriptSyntax.FilterHelper(filter)
            go.SubObjectSelect <- false
            go.GroupSelect <- false
            go.AcceptNothing(true)
            return 
                if go.Get() <> Rhino.Input.GetResult.Object then None
                else
                    let objref = go.Object(0)
                    let id = objref.ObjectId
                    let presel = go.ObjectsWerePreselected
                    let mutable selmethod = 0
                    let sm = objref.SelectionMethod()
                    if Rhino.DocObjects.SelectionMethod.MousePick = sm then selmethod <- 1
                    elif Rhino.DocObjects.SelectionMethod.WindowBox = sm then selmethod <- 2
                    elif Rhino.DocObjects.SelectionMethod.CrossingBox = sm then selmethod <- 3
                    let point = objref.SelectionPoint()
                    let viewname = go.View().ActiveViewport.Name
                    let obj = go.Object(0).Object()
                    go.Dispose()
                    if not <| select && not <| presel then
                        Doc.Objects.UnselectAll() |> ignore
                        Doc.Views.Redraw()
                    obj.Select(select) |> ignore
                    Some (id, presel, selmethod, point, viewname)
            } |> Async.RunSynchronously
    (*
    def GetObjectEx(message=None, filter=0, preselect=False, select=False, objects=None):
        '''Prompts user to pick, or select a single object
        Parameters:
          message (str, optional): a prompt or message.
          filter (number, optional): The type(s) of geometry (points, curves, surfaces, meshes,...)
              that can be selected. Object types can be added together to filter
              several different kinds of geometry. use the filter class to get values
          preselect (bool, optional):  Allow for the selection of pre-selected objects.
          select (bool, optional): Select the picked objects.  If False, the objects that are
              picked are not selected.
          objects ([guid, ...]): list of object identifiers specifying objects that are
              allowed to be selected
        Returns:
          tuple(guid, bool, number, point, str): containing the following information
              [0] identifier of the object
              [1] True if the object was preselected, otherwise False
              [2] selection method (see help)
              [3] selection point
              [4] name of the view selection was made
          None: if no object selected
        '''
    
        if not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        go = None
        if objects:
            ids = [rhutil.coerceguid(id, True) for id in objects]
            if ids: go = __CustomGetObjectEx(ids)
        if not go: go = Rhino.Input.Custom.GetObject()
        if message: go.SetCommandPrompt(message)
        geometry_filter = __FilterHelper(filter)
        if filter>0: go.GeometryFilter = geometry_filter
        go.SubObjectSelect = False
        go.GroupSelect = False
        go.AcceptNothing(True)
        if go.Get()!=Rhino.Input.GetResult.Object: return None
        objref = go.Object(0)
        id = objref.ObjectId
        presel = go.ObjectsWerePreselected
        selmethod = 0
        sm = objref.SelectionMethod()
        if Rhino.DocObjects.SelectionMethod.MousePick==sm: selmethod = 1
        elif Rhino.DocObjects.SelectionMethod.WindowBox==sm: selmethod = 2
        elif Rhino.DocObjects.SelectionMethod.CrossingBox==sm: selmethod = 3
        point = objref.SelectionPoint()
        viewname = go.View().ActiveViewport.Name
        obj = go.Object(0).Object()
        go.Dispose()
        if not select and not presel:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        obj.Select(select)
        return id, presel, selmethod, point, viewname
    *)


    [<EXT>]
    ///<summary>Prompts user to pick or select one or more objects.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Select objects"</c>
    ///A prompt or message.</param>
    ///<param name="filter">(int) Optional, Default Value: <c>0</c>
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
    ///<param name="objects">(Guid seq) Optional, List of objects that are allowed to be selected. If set customFilter will be ignored</param>
    ///<param name="minimumCount">(int) Optional, Default Value: <c>1</c>
    ///Minimum count of 'limits on number of objects allowed to be selected' (FIXME 0)</param>
    ///<param name="maximumCount">(int) Optional, Default Value: <c>0</c>
    ///Maximum count of 'limits on number of objects allowed to be selected' (FIXME 0)</param>
    ///<param name="customFilter">(string) Optional, Will be ignored if 'objects' are set. Calls a custom function in the script and passes the Rhino Object, Geometry, and component index and returns true or false indicating if the object can be selected</param>
    ///<returns>(Guid array) identifiers of the picked objects</returns>
    static member GetObjects(       [<OPT;DEF("Select objects":string)>]message:string, 
                                    [<OPT;DEF(0)>]filter:int, 
                                    [<OPT;DEF(true)>]group:bool, 
                                    [<OPT;DEF(false)>]preselect:bool, 
                                    [<OPT;DEF(false)>]select:bool, 
                                    [<OPT;DEF(null:Guid seq)>]objects:Guid seq, 
                                    [<OPT;DEF(1)>]minimumCount:int, 
                                    [<OPT;DEF(0)>]maximumCount:int, 
                                    [<OPT;DEF(null:Input.Custom.GetObjectGeometryFilter)>]customFilter:Input.Custom.GetObjectGeometryFilter)  : option<Guid ResizeArray> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            if not <| preselect then
                Doc.Objects.UnselectAll() |> ignore
                Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull objects then
                let s = System.Collections.Generic.HashSet(objects)
                go.SetCustomGeometryFilter(fun rhinoobject _ _ -> s.Contains(rhinoobject.Id))
            elif notNull customFilter then 
                go.SetCustomGeometryFilter(customFilter)
            go.SetCommandPrompt(message )
            let geometryfilter = RhinoScriptSyntax.FilterHelper(filter)
            if filter>0 then go.GeometryFilter <- geometryfilter
            go.SubObjectSelect <- false
            go.GroupSelect <- group
            go.AcceptNothing(true)
            return
                if go.GetMultiple(minimumCount,maximumCount) <> Rhino.Input.GetResult.Object then None
                else
                    if not <| select && not <| go.ObjectsWerePreselected then
                        Doc.Objects.UnselectAll() |> ignore
                        Doc.Views.Redraw()
                    let rc = ResizeArray()
                    let count = go.ObjectCount
                    for i in range(count) do
                        let objref = go.Object(i)
                        rc.Add(objref.ObjectId)
                        let obj = objref.Object()
                        if select && notNull obj then obj.Select(select) |> ignore            
                    Some rc
            } |> Async.RunSynchronously
    (*
    def GetObjects(message=None, filter=0, group=True, preselect=False, select=False, objects=None, minimum_count=1, maximum_count=0, custom_filter=None):
        '''Prompts user to pick or select one or more objects.
        Parameters:
          message (str, optional): a prompt or message.
          filter (number, optional): The type(s) of geometry (points, curves, surfaces, meshes,...)
              that can be selected. Object types can be added together to filter
              several different kinds of geometry. use the filter class to get values
                  Value         Description
                  0             All objects (default)
                  1             Point
                  2             Point cloud
                  4             Curve
                  8             Surface or single-face brep
                  16            Polysurface or multiple-face
                  32            Mesh
                  256           Light
                  512           Annotation
                  4096          Instance or block reference
                  8192          Text dot object
                  16384         Grip object
                  32768         Detail
                  65536         Hatch
                  131072        Morph control
                  134217728     Cage
                  268435456     Phantom
                  536870912     Clipping plane
                  1073741824    Extrusion
          group (bool, optional): Honor object grouping.  If omitted and the user picks a group,
              the entire group will be picked (True). Note, if filter is set to a
              value other than 0 (All objects), then group selection will be disabled.
          preselect (bool, optional):  Allow for the selection of pre-selected objects.
          select (bool, optional): Select the picked objects.  If False, the objects that are
              picked are not selected.
          objects ([guid, ...]): list of objects that are allowed to be selected
          minimum_count, maximum_count(number): limits on number of objects allowed to be selected
          custom_filter (str, optional): Calls a custom function in the script and passes the Rhino Object, Geometry, and component index and returns true or false indicating if the object can be selected
        Returns:
          list(guid, ...): identifiers of the picked objects
        '''
    
        if not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
    
        objects = rhutil.coerceguidlist(objects)
        class CustomGetObject(Rhino.Input.Custom.GetObject):
            def __init__(self, filter_function):
                self.m_filter_function = filter_function
            def CustomGeometryFilter( self, rhino_object, geometry, component_index ):
                if objects and not rhino_object.Id in objects: return False
                rc = True
                if self.m_filter_function is not None:
                    try:
                        rc = self.m_filter_function(rhino_object, geometry, component_index)
                    except:
                        rc = True
                return rc
        go = CustomGetObject(custom_filter)
        go.SetCommandPrompt(message or "Select objects")
        geometry_filter = __FilterHelper(filter)
        if filter>0: go.GeometryFilter = geometry_filter
        go.SubObjectSelect = False
        go.GroupSelect = group
        go.AcceptNothing(True)
        if go.GetMultiple(minimum_count,maximum_count)!=Rhino.Input.GetResult.Object: return None
        if not select and not go.ObjectsWerePreselected:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        rc = []
        count = go.ObjectCount
        for i in xrange(count):
            objref = go.Object(i)
            rc.append(objref.ObjectId)
            obj = objref.Object()
            if select and obj is not None: obj.Select(select)
        go.Dispose()
        return rc
    *)


    [<EXT>]
    ///<summary>Prompts user to pick, or select one or more objects</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Select objects"</c>
    ///A prompt or message.</param>
    ///<param name="filter">(int) Optional, Default Value: <c>0</c>
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
    ///<param name="objects">(Guid seq) Optional, Default Value: <c>null:Guid seq</c>
    ///List of object identifiers specifying objects that are
    ///  allowed to be selected</param>
    ///<returns>((Guid*bool*int*Point3d*string) ResizeArray) containing the following information
    ///  [n][0]  identifier of the object
    ///  [n][1]  True if the object was preselected, otherwise False
    ///  [n][2]  selection method (see help)
    ///  [n][3]  selection point
    ///  [n][4]  name of the view selection was made</returns>
    static member GetObjectsEx(     [<OPT;DEF("Select objects":string)>]message:string, 
                                    [<OPT;DEF(0)>]filter:int, 
                                    [<OPT;DEF(true)>]group:bool, 
                                    [<OPT;DEF(false)>]preselect:bool, 
                                    [<OPT;DEF(false)>]select:bool, 
                                    [<OPT;DEF(null:Guid seq)>]objects:Guid seq) : option<(Guid*bool*int*Point3d*string) ResizeArray> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            if not <| preselect then
                Doc.Objects.UnselectAll() |> ignore
                Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull objects then
                let s = System.Collections.Generic.HashSet(objects)
                go.SetCustomGeometryFilter(fun rhinoobject _ _ -> s.Contains(rhinoobject.Id))            
            go.SetCommandPrompt(message)
            let geometryfilter = RhinoScriptSyntax.FilterHelper(filter)
            if filter>0 then go.GeometryFilter <- geometryfilter
            go.SubObjectSelect <- false
            go.GroupSelect <- group
            go.AcceptNothing(true)
            return 
                if go.GetMultiple(1,0) <> Rhino.Input.GetResult.Object then None
                else
                    if not <| select && not <| go.ObjectsWerePreselected then
                        Doc.Objects.UnselectAll() |> ignore
                        Doc.Views.Redraw()
                    let rc = ResizeArray()
                    let count = go.ObjectCount
                    for i in range(count) do
                        let objref = go.Object(i)
                        let id = objref.ObjectId
                        let presel = go.ObjectsWerePreselected
                        let mutable selmethod = 0
                        let sm = objref.SelectionMethod()
                        if Rhino.DocObjects.SelectionMethod.MousePick = sm then selmethod <- 1
                        elif Rhino.DocObjects.SelectionMethod.WindowBox = sm then selmethod <- 2
                        elif Rhino.DocObjects.SelectionMethod.CrossingBox = sm then selmethod <- 3
                        let point = objref.SelectionPoint()
                        let viewname = go.View().ActiveViewport.Name
                        rc.Add( (id, presel, selmethod, point, viewname) )
                        let obj = objref.Object()
                        if select && notNull obj then obj.Select(select) |> ignore                    
                    Some rc
            } |> Async.RunSynchronously
    (*
    def GetObjectsEx(message=None, filter=0, group=True, preselect=False, select=False, objects=None):
        '''Prompts user to pick, or select one or more objects
        Parameters:
          message (str, optional):  a prompt or message.
          filter (number, optional): The type(s) of geometry (points, curves, surfaces, meshes,...)
              that can be selected. Object types can be added together to filter
              several different kinds of geometry. use the filter class to get values
          group (bool, optional): Honor object grouping.  If omitted and the user picks a group,
              the entire group will be picked (True). Note, if filter is set to a
              value other than 0 (All objects), then group selection will be disabled.
          preselect (bool, optional):  Allow for the selection of pre-selected objects.
          select (bool, optional): Select the picked objects. If False, the objects that are
              picked are not selected.
          objects ([guid, ...]): list of object identifiers specifying objects that are
              allowed to be selected
        Returns:
          list(tuple(guid, bool, number, point, str), ...): containing the following information
            [n][0]  identifier of the object
            [n][1]  True if the object was preselected, otherwise False
            [n][2]  selection method (see help)
            [n][3]  selection point
            [n][4]  name of the view selection was made
        '''
    
        if not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        go = None
        if objects:
            ids = [rhutil.coerceguid(id) for id in objects]
            if ids: go = __CustomGetObjectEx(ids)
        if not go: go = Rhino.Input.Custom.GetObject()
        go.SetCommandPrompt(message or "Select objects")
        geometry_filter = __FilterHelper(filter)
        if filter>0: go.GeometryFilter = geometry_filter
        go.SubObjectSelect = False
        go.GroupSelect = group
        go.AcceptNothing(True)
        if go.GetMultiple(1,0)!=Rhino.Input.GetResult.Object: return []
        if not select and not go.ObjectsWerePreselected:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        rc = []
        count = go.ObjectCount
        for i in xrange(count):
            objref = go.Object(i)
            id = objref.ObjectId
            presel = go.ObjectsWerePreselected
            selmethod = 0
            sm = objref.SelectionMethod()
            if Rhino.DocObjects.SelectionMethod.MousePick==sm: selmethod = 1
            elif Rhino.DocObjects.SelectionMethod.WindowBox==sm: selmethod = 2
            elif Rhino.DocObjects.SelectionMethod.CrossingBox==sm: selmethod = 3
            point = objref.SelectionPoint()
            viewname = go.View().ActiveViewport.Name
            rc.append( (id, presel, selmethod, point, viewname) )
            obj = objref.Object()
            if select and obj is not None: obj.Select(select)
        go.Dispose()
        return rc
    *)


    [<EXT>]
    ///<summary>Prompts the user to select one or more point objects.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Select Point Objects"</c>
    ///A prompt message.</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.  If omitted (False), pre-selected objects are not accepted.</param>
    ///<returns>(Point3d array) 3d coordinates of point objects on success</returns>
    static member GetPointCoordinates(  [<OPT;DEF("Select Point Objects")>] message:string, 
                                        [<OPT;DEF(false)>]                  preselect:bool) : option<Point3d ResizeArray> =
        maybe{
            let! ids = RhinoScriptSyntax.GetObjects(message, Filter.Point, preselect=preselect)
            let rc = ResizeArray()
            for id in ids do
                let pt = RhinoScriptSyntax.Coerce3dPoint(id)
                rc.Add(pt)
            return rc
            }
 
    (*
    def GetPointCoordinates(message="Select points", preselect=False):
        '''Prompts the user to select one or more point objects.
        Parameters:
          message (str, optional): a prompt message.
          preselect (bool, optional): Allow for the selection of pre-selected objects.  If omitted (False), pre-selected objects are not accepted.
        Returns:
          list(point, ...): 3d coordinates of point objects on success
        '''
    
        ids = GetObjects(message, filter.point, preselect=preselect)
        rc = []
        for id in ids:
            rhobj = scriptcontext.doc.Objects.Find(id)
            rc.append(rhobj.Geometry.Location)
        return rc
    *)


