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
    ///<returns>(Guid seq) identifiers for all the objects in the document</returns>
    static member AllObjects([<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool, [<OPT;DEF(false)>]includeReferences:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
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
    static member FirstObject([<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
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
    static member FilterHelper(filter:int) : Rhino.DocObjects.ObjectType =
        failNotImpl () // genreation temp disabled !!
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
    static member GetCurveObject([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid * bool * int * Point3d * float * string =
        failNotImpl () // genreation temp disabled !!
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
    ///  several different kinds of geometry. use the filter class to get values</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked objects.  If False, the objects that are
    ///  picked are not selected.</param>
    ///<param name="customFilter">(obj->unit) Optional, Default Value: <c>null:obj->unit</c>
    ///A custom filter function</param>
    ///<param name="subobjects">(bool) Optional, Default Value: <c>false</c>
    ///If True, subobjects can be selected. When this is the
    ///  case, an ObjRef is returned instead of a Guid to allow for tracking
    ///  of the subobject when passed into other functions</param>
    ///<returns>(Guid) Identifier of the picked object</returns>
    static member GetObject([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(0)>]filter:int, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null)>]customFilter:obj->unit, [<OPT;DEF(false)>]subobjects:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
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
    class __CustomGetObjectEx(Rhino.Input.Custom.GetObject):
        def __init__(self, allowable_geometry):
            self.m_allowable = allowable_geometry
        def CustomGeometryFilter(self, rhino_object, geometry, component_index):
            for id in self.m_allowable:
                if id==rhino_object.Id: return True
            return False
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
    ///  [2] selection method (see help)
    ///  [3] selection point
    ///  [4] name of the view selection was made</returns>
    static member GetObjectEx([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(0)>]filter:int, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null:Guid seq)>]objects:Guid seq) : Guid * bool * float * Point3d * string =
        failNotImpl () // genreation temp disabled !!
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
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
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
    ///<param name="objects">(Guid seq) Optional, Default Value: <c>null:Guid seq</c>
    ///List of objects that are allowed to be selected</param>
    ///<param name="minimumCount">(int) Optional, Default Value: <c>1</c>
    ///Minimum count of 'limits on number of objects allowed to be selected' (FIXME 0)</param>
    ///<param name="maximumCount">(int) Optional, Default Value: <c>0</c>
    ///Maximum count of 'limits on number of objects allowed to be selected' (FIXME 0)</param>
    ///<param name="customFilter">(string) Optional, Default Value: <c>null:string</c>
    ///Calls a custom function in the script and passes the Rhino Object, Geometry, and component index and returns true or false indicating if the object can be selected</param>
    ///<returns>(Guid seq) identifiers of the picked objects</returns>
    static member GetObjects([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(0)>]filter:int, [<OPT;DEF(true)>]group:bool, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null:Guid seq)>]objects:Guid seq, [<OPT;DEF(1)>]minimumCount:int, [<OPT;DEF(0)>]maximumCount:int, [<OPT;DEF(null:string)>]customFilter:string) : Guid seq =
        failNotImpl () // genreation temp disabled !!
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
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
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
    ///<returns>((Guid*bool*int*Point3d*string) seq) containing the following information
    ///  [n][0]  identifier of the object
    ///  [n][1]  True if the object was preselected, otherwise False
    ///  [n][2]  selection method (see help)
    ///  [n][3]  selection point
    ///  [n][4]  name of the view selection was made</returns>
    static member GetObjectsEx([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(0)>]filter:int, [<OPT;DEF(true)>]group:bool, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null:Guid seq)>]objects:Guid seq) : (Guid*bool*int*Point3d*string) seq =
        failNotImpl () // genreation temp disabled !!
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
    ///<param name="message">(string) Optional, Default Value: <c>"Select points"</c>
    ///A prompt message.</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for the selection of pre-selected objects.  If omitted (False), pre-selected objects are not accepted.</param>
    ///<returns>(Point3d seq) 3d coordinates of point objects on success</returns>
    static member GetPointCoordinates([<OPT;DEF("Select points")>]message:string, [<OPT;DEF(false)>]preselect:bool) : Point3d seq =
        failNotImpl () // genreation temp disabled !!
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


    [<EXT>]
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
        failNotImpl () // genreation temp disabled !!
    (*
    def GetSurfaceObject(message="Select surface", preselect=False, select=False):
        '''Prompts the user to select a single surface
        Parameters:
          message(str, optional): prompt displayed
          preselect (bool, optional): allow for preselected objects
          select (bool, optional):  select the picked object
        Returns:
          tuple(guid, bool, number, point, (number, number), str): of information on success
            [0]  identifier of the surface
            [1]  True if the surface was preselected, otherwise False
            [2]  selection method ( see help )
            [3]  selection point
            [4]  u,v surface parameter of the selection point
            [5]  name of the view in which the selection was made
          None: on error
        '''
        if not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        go = Rhino.Input.Custom.GetObject()
        go.SetCommandPrompt(message)
        go.GeometryFilter = Rhino.DocObjects.ObjectType.Surface
        go.SubObjectSelect = False
        go.GroupSelect = False
        go.AcceptNothing(True)
        if go.Get()!=Rhino.Input.GetResult.Object:
            return scriptcontext.errorhandler()
        objref = go.Object(0)
        rhobj = objref.Object()
        rhobj.Select(select)
        scriptcontext.doc.Views.Redraw()
        id = rhobj.Id
        prepicked = go.ObjectsWerePreselected
        selmethod = objref.SelectionMethod()
        point = objref.SelectionPoint()
        surf, u, v = objref.SurfaceParameter()
        uv = (u,v)
        if not point.IsValid:
            point = None
            uv = None
        view = go.View()
        name = view.ActiveViewport.Name
        go.Dispose()
        if not select and not prepicked:
          scriptcontext.doc.Objects.UnselectAll()
          scriptcontext.doc.Views.Redraw()
        return id, prepicked, selmethod, point, uv, name
    *)


    [<EXT>]
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
        failNotImpl () // genreation temp disabled !!
    (*
    def LockedObjects(include_lights=False, include_grips=False, include_references=False):
        '''Returns identifiers of all locked objects in the document. Locked objects
        cannot be snapped to, and cannot be selected
        Parameters:
          include_lights (bool, optional): include light objects
          include_grips (bool, optional): include grip objects
          include_references(bool, optional): Include refrence objects such as work session objects
        Returns:
          list(guid, ...): identifiers the locked objects if successful.
        '''
        settings = Rhino.DocObjects.ObjectEnumeratorSettings()
        settings.ActiveObjects = True
        settings.NormalObjects = True
        settings.LockedObjects = True
        settings.HiddenObjects = True
        settings.IncludeLights = include_lights
        settings.IncludeGrips = include_grips
        settings.ReferenceObjects = include_references
        return [i.Id for i in scriptcontext.doc.Objects.GetObjectList(settings)
            if i.IsLocked or (scriptcontext.doc.Layers[i.Attributes.LayerIndex]).IsLocked]
    *)


    [<EXT>]
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
        failNotImpl () // genreation temp disabled !!
    (*
    def HiddenObjects(include_lights=False, include_grips=False, include_references=False):
        '''Returns identifiers of all hidden objects in the document. Hidden objects
        are not visible, cannot be snapped to, and cannot be selected
        Parameters:
          include_lights (bool, optional): include light objects
          include_grips (bool, optional): include grip objects
          include_references(bool, optional): Include refrence objects such as work session objects
        Returns:
          list(guid, ...): identifiers of the hidden objects if successful.
        '''
        settings = Rhino.DocObjects.ObjectEnumeratorSettings()
        settings.ActiveObjects = True
        settings.NormalObjects = True
        settings.LockedObjects = True
        settings.HiddenObjects = True
        settings.IncludeLights = include_lights
        settings.IncludeGrips = include_grips
        settings.ReferenceObjects = include_references
        return [i.Id for i in scriptcontext.doc.Objects.GetObjectList(settings)
            if i.IsHidden or not (scriptcontext.doc.Layers[i.Attributes.LayerIndex]).IsVisible]
    *)


    [<EXT>]
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
        failNotImpl () // genreation temp disabled !!
    (*
    def InvertSelectedObjects(include_lights=False, include_grips=False, include_references=False):
        '''Inverts the current object selection. The identifiers of the newly
        selected objects are returned
        Parameters:
          include_lights (bool, optional): Include light objects.  If omitted (False), light objects are not returned.
          include_grips (bool, optional): Include grips objects.  If omitted (False), grips objects are not returned.
          include_references(bool, optional): Include refrence objects such as work session objects
        Returns:
          list(guid, ...): identifiers of the newly selected objects if successful.
        '''
        settings = Rhino.DocObjects.ObjectEnumeratorSettings()
        settings.IncludeLights = include_lights
        settings.IncludeGrips = include_grips
        settings.IncludePhantoms = True
        settings.ReferenceObjects = include_references
        rhobjs = scriptcontext.doc.Objects.GetObjectList(settings)
        rc = []
        for obj in rhobjs:
            if not obj.IsSelected(False) and obj.IsSelectable():
                rc.append(obj.Id)
                obj.Select(True)
            else:
                obj.Select(False)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns identifiers of the objects that were most recently created or changed
    ///  by scripting a Rhino command using the Command function. It is important to
    ///  call this function immediately after calling the Command function as only the
    ///  most recently created or changed object identifiers will be returned</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the object.  If omitted (False), the object is not selected.</param>
    ///<returns>(Guid seq) identifiers of the most recently created or changed objects .</returns>
    static member LastCreatedObjects([<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def LastCreatedObjects(select=False):
        '''Returns identifiers of the objects that were most recently created or changed
        by scripting a Rhino command using the Command function. It is important to
        call this function immediately after calling the Command function as only the
        most recently created or changed object identifiers will be returned
        Parameters:
          select (bool, optional): Select the object.  If omitted (False), the object is not selected.
        Returns:
          list(guid, ...): identifiers of the most recently created or changed objects if successful.
        '''
        serial_numbers = rhapp.__command_serial_numbers
        if serial_numbers is None: return scriptcontext.errorhandler()
        serial_number = serial_numbers[0]
        end = serial_numbers[1]
        rc = []
        while serial_number<end:
            obj = scriptcontext.doc.Objects.Find(serial_number)
            if obj and not obj.IsDeleted:
                rc.append(obj.Id)
                if select: obj.Select(True)
            serial_number += 1
        if select==True and rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
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
        failNotImpl () // genreation temp disabled !!
    (*
    def LastObject(select=False, include_lights=False, include_grips=False):
        '''Returns the identifier of the last object in the document. The last object
        in the document is the first object created by the user
        Parameters:
          select (bool, optional): select the object
          include_lights (bool, optional): include lights in the potential set
          include_grips (bool, optional): include grips in the potential set
        Returns:
          guid: identifier of the object on success
        '''
        settings = Rhino.DocObjects.ObjectEnumeratorSettings()
        settings.IncludeLights = include_lights
        settings.IncludeGrips = include_grips
        settings.DeletedObjects = False
        rhobjs = scriptcontext.doc.Objects.GetObjectList(settings)
        firstobj = None
        for obj in rhobjs: firstobj = obj
        if firstobj is None: return scriptcontext.errorhandler()
        rc = firstobj.Id
        if select:
            firstobj.Select(True)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
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
        failNotImpl () // genreation temp disabled !!
    (*
    def NextObject(object_id, select=False, include_lights=False, include_grips=False):
        '''Returns the identifier of the next object in the document
        Parameters:
          object_id (guid): the identifier of the object from which to get the next object
          select (bool, optional): select the object
          include_lights (bool, optional): include lights in the potential set
          include_grips (bool, optional): include grips in the potential set
        Returns:
          guid: identifier of the object on success
        '''
        current_obj = rhutil.coercerhinoobject(object_id, True)
        settings = Rhino.DocObjects.ObjectEnumeratorSettings()
        settings.IncludeLights = include_lights
        settings.IncludeGrips = include_grips
        settings.DeletedObjects = False
        rhobjs = scriptcontext.doc.Objects.GetObjectList(settings)
        found = False
        for obj in rhobjs:
            if found and obj: return obj.Id
            if obj.Id == current_obj.Id: found = True
    *)


    [<EXT>]
    ///<summary>Returns identifiers of all normal objects in the document. Normal objects
    ///  are visible, can be snapped to, and are independent of selection state</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects.  If omitted (False), light objects are not returned.</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grips objects.  If omitted (False), grips objects are not returned.</param>
    ///<returns>(Guid seq) identifier of normal objects .</returns>
    static member NormalObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def NormalObjects(include_lights=False, include_grips=False):
        '''Returns identifiers of all normal objects in the document. Normal objects
        are visible, can be snapped to, and are independent of selection state
        Parameters:
          include_lights (bool, optional): Include light objects.  If omitted (False), light objects are not returned.
          include_grips (bool, optional): Include grips objects.  If omitted (False), grips objects are not returned.
        Returns:
          list(guid, ...): identifier of normal objects if successful.
        '''
        iter = Rhino.DocObjects.ObjectEnumeratorSettings()
        iter.NormalObjects = True
        iter.LockedObjects = False
        iter.IncludeLights = include_lights
        iter.IncludeGrips = include_grips
        return [obj.Id for obj in scriptcontext.doc.Objects.GetObjectList(iter)]
    *)


    [<EXT>]
    ///<summary>Returns identifiers of all objects based on color</summary>
    ///<param name="color">(Drawing.Color) Color to get objects by</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include lights in the set</param>
    ///<returns>(Guid seq) identifiers of objects of the selected color.</returns>
    static member ObjectsByColor(color:Drawing.Color, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectsByColor(color, select=False, include_lights=False):
        '''Returns identifiers of all objects based on color
        Parameters:
          color (color): color to get objects by
          select (bool, optional): select the objects
          include_lights (bool, optional): include lights in the set
        Returns:
          list(guid, ...): identifiers of objects of the selected color.
        '''
        color = rhutil.coercecolor(color, True)
        rhino_objects = scriptcontext.doc.Objects.FindByDrawColor(color, include_lights)
        if select:
            for obj in rhino_objects: obj.Select(True)
            scriptcontext.doc.Views.Redraw()
        return [obj.Id for obj in rhino_objects]
    *)


    [<EXT>]
    ///<summary>Returns identifiers of all objects based on the objects' group name</summary>
    ///<param name="groupName">(string) Name of the group</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<returns>(Guid seq) identifiers for objects in the group on success</returns>
    static member ObjectsByGroup(groupName:string, [<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectsByGroup(group_name, select=False):
        '''Returns identifiers of all objects based on the objects' group name
        Parameters:
          group_name (str): name of the group
          select (bool, optional): select the objects
        Returns:
          list(guid, ...):identifiers for objects in the group on success
        '''
        group_instance = scriptcontext.doc.Groups.FindName(group_name)
        if group_instance is None: raise ValueError("%s does not exist in GroupTable"%group_name)
        rhino_objects = scriptcontext.doc.Groups.GroupMembers(group_instance.Index)
        if not rhino_objects: return []
        if select:
            for obj in rhino_objects: obj.Select(True)
            scriptcontext.doc.Views.Redraw()
        return [obj.Id for obj in rhino_objects]
    *)


    [<EXT>]
    ///<summary>Returns identifiers of all objects based on the objects' layer name</summary>
    ///<param name="layerName">(string) Name of the layer</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<returns>(Guid seq) identifiers for objects in the specified layer</returns>
    static member ObjectsByLayer(layerName:string, [<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectsByLayer(layer_name, select=False):
        '''Returns identifiers of all objects based on the objects' layer name
        Parameters:
          layer_name (str): name of the layer
          select (bool, optional): select the objects
        Returns:
          list(guid, ...): identifiers for objects in the specified layer
        '''
        layer = __getlayer(layer_name, True)
        rhino_objects = scriptcontext.doc.Objects.FindByLayer(layer)
        if not rhino_objects: return []
        if select:
            for rhobj in rhino_objects: rhobj.Select(True)
            scriptcontext.doc.Views.Redraw()
        return [rhobj.Id for rhobj in rhino_objects]
    *)


    [<EXT>]
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
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectsByName(name, select=False, include_lights=False, include_references=False):
        '''Returns identifiers of all objects based on user-assigned name
        Parameters:
          name (str): name of the object or objects
          select (bool, optional): select the objects
          include_lights (bool, optional): include light objects
          include_references(bool, optional): Include refrence objects such as work session objects
        Returns:
          list(guid, ...): identifiers for objects with the specified name.
        '''
        settings = Rhino.DocObjects.ObjectEnumeratorSettings()
        settings.HiddenObjects = True
        settings.DeletedObjects = False
        settings.IncludeGrips = False
        settings.IncludePhantoms = True
        settings.IncludeLights = include_lights
        settings.NameFilter = name
        settings.ReferenceObjects = include_references
        objects = scriptcontext.doc.Objects.GetObjectList(settings)
        ids = [rhobj.Id for rhobj in objects]
        if ids and select:
            objects = scriptcontext.doc.Objects.GetObjectList(settings)
            for rhobj in objects: rhobj.Select(True)
            scriptcontext.doc.Views.Redraw()
        return ids
    *)


    [<EXT>]
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
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectsByType(geometry_type, select=False, state=0):
        '''Returns identifiers of all objects based on the objects' geometry type.
        Parameters:
          geometry_type (number): The type(s) of geometry objects (points, curves, surfaces,
                 meshes, etc.) that can be selected. Object types can be
                 added together as bit-coded flags to filter several different kinds of geometry.
                  Value        Description
                   0           All objects
                   1           Point
                   2           Point cloud
                   4           Curve
                   8           Surface or single-face brep
                   16          Polysurface or multiple-face
                   32          Mesh
                   256         Light
                   512         Annotation
                   4096        Instance or block reference
                   8192        Text dot object
                   16384       Grip object
                   32768       Detail
                   65536       Hatch
                   131072      Morph control
                   134217728   Cage
                   268435456   Phantom
                   536870912   Clipping plane
                   1073741824  Extrusion
          select (bool, optional): Select the objects
          state (bool, optional): The object state (normal, locked, and hidden). Object states can be 
            added together to filter several different states of geometry.
                  Value     Description
                  0         All objects
                  1         Normal objects
                  2         Locked objects
                  4         Hidden objects
        Returns:
          list(guid, ...): identifiers of object that fit the specified type(s).
        '''
        if not state: state = 7
        bSurface = False
        bPolySurface = False
        bLights = False
        bGrips = False
        bPhantoms = False
        geometry_filter = __FilterHelper(geometry_type)
        if type(geometry_type) is int and geometry_type==0:
            geometry_filter = Rhino.DocObjects.ObjectType.AnyObject
        if geometry_filter & Rhino.DocObjects.ObjectType.Surface: bSurface = True
        if geometry_filter & Rhino.DocObjects.ObjectType.Brep: bPolySurface = True
        if geometry_filter & Rhino.DocObjects.ObjectType.Light: bLights = True
        if geometry_filter & Rhino.DocObjects.ObjectType.Grip: bGrips = True
        if geometry_filter & Rhino.DocObjects.ObjectType.Phantom: bPhantoms = True
        it = Rhino.DocObjects.ObjectEnumeratorSettings()
        it.DeletedObjects = False
        it.ActiveObjects = True
        it.ReferenceObjects = True
        it.IncludeLights = bLights
        it.IncludeGrips = bGrips
        it.IncludePhantoms = bPhantoms
        if state:
            it.NormalObjects = False
            it.LockedObjects = False
        if state & 1: it.NormalObjects = True
        if state & 2: it.LockedObjects = True
        if state & 4: it.HiddenObjects = True
        object_ids = []
        e = scriptcontext.doc.Objects.GetObjectList(it)
        for object in e:
            bFound = False
            object_type = object.ObjectType
            if object_type==Rhino.DocObjects.ObjectType.Brep and (bSurface or bPolySurface):
                brep = rhutil.coercebrep(object.Id)
                if brep:
                    if brep.Faces.Count==1:
                        if bSurface: bFound = True
                    else:
                        if bPolySurface: bFound = True
            elif object_type==Rhino.DocObjects.ObjectType.Extrusion and (bSurface or bPolySurface):
                extrusion = object.Geometry
                profile_count = extrusion.ProfileCount
                cap_count = extrusion.CapCount
                if profile_count==1 and cap_count==0 and bSurface:
                    bFound = True
                elif profile_count>0 and cap_count>0 and bPolySurface:
                    bFound = True
            elif object_type & geometry_filter:
                bFound = True
            if bFound:
                if select: object.Select(True)
                object_ids.append(object.Id)
        if object_ids and select: scriptcontext.doc.Views.Redraw()
        return object_ids
    *)


    [<EXT>]
    ///<summary>Returns the identifiers of all objects that are currently selected</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grip objects</param>
    ///<returns>(Guid seq) identifiers of selected objects</returns>
    static member SelectedObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def SelectedObjects(include_lights=False, include_grips=False):
        '''Returns the identifiers of all objects that are currently selected
        Parameters:
          include_lights (bool, optional): include light objects
          include_grips (bool, optional): include grip objects
        Returns:
          list(guid, ...): identifiers of selected objects
        '''
        selobjects = scriptcontext.doc.Objects.GetSelectedObjects(include_lights, include_grips)
        return [obj.Id for obj in selobjects]
    *)


    [<EXT>]
    ///<summary>Unselects all objects in the document</summary>
    ///<returns>(float) the number of objects that were unselected</returns>
    static member UnselectAllObjects() : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def UnselectAllObjects():
        '''Unselects all objects in the document
        Returns:
          number: the number of objects that were unselected
        '''
        rc = scriptcontext.doc.Objects.UnselectAll()
        if rc>0: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Return identifiers of all objects that are visible in a specified view</summary>
    ///<param name="view">(bool) Optional, Default Value: <c>null:bool</c>
    ///The view to use. If omitted, the current active view is used</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///Include grip objects</param>
    ///<returns>(Guid seq) identifiers of the visible objects</returns>
    static member VisibleObjects([<OPT;DEF(null)>]view:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def VisibleObjects(view=None, select=False, include_lights=False, include_grips=False):
        '''Return identifiers of all objects that are visible in a specified view
        Parameters:
          view (bool, optional): the view to use. If omitted, the current active view is used
          select (bool, optional): Select the objects
          include_lights (bool, optional): include light objects
          include_grips (bool, optional): include grip objects
        Returns:
          list(guid, ...): identifiers of the visible objects
        '''
        it = Rhino.DocObjects.ObjectEnumeratorSettings()
        it.DeletedObjects = False
        it.ActiveObjects = True
        it.ReferenceObjects = True
        it.IncludeLights = include_lights
        it.IncludeGrips = include_grips
        it.VisibleFilter = True
        viewport = __viewhelper(view).MainViewport
        it.ViewportFilter = viewport
        object_ids = []
        e = scriptcontext.doc.Objects.GetObjectList(it)
        for object in e:
            bbox = object.Geometry.GetBoundingBox(True)
            if viewport.IsVisible(bbox):
                if select: object.Select(True)
                object_ids.append(object.Id)
        if object_ids and select: scriptcontext.doc.Views.Redraw()
        return object_ids
    *)


    [<EXT>]
    ///<summary>Picks objects using either a window or crossing selection</summary>
    ///<param name="corner1">(Point3d) Corner1 of 'corners of selection window' (FIXME 0)</param>
    ///<param name="corner2">(Point3d) Corner2 of 'corners of selection window' (FIXME 0)</param>
    ///<param name="view">(bool) Optional, Default Value: <c>null:bool</c>
    ///View to perform the selection in</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select picked objects</param>
    ///<param name="inWindow">(bool) Optional, Default Value: <c>true</c>
    ///If False, then a crossing window selection is performed</param>
    ///<returns>(Guid seq) identifiers of selected objects on success</returns>
    static member WindowPick(corner1:Point3d, corner2:Point3d, [<OPT;DEF(null)>]view:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(true)>]inWindow:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def WindowPick(corner1, corner2, view=None, select=False, in_window=True):
        '''Picks objects using either a window or crossing selection
        Parameters:
          corner1, corner2 (point): corners of selection window
          view (bool, optional): view to perform the selection in
          select (bool, optional): select picked objects
          in_window (bool, optional): if False, then a crossing window selection is performed
        Returns:
          list(guid, ...): identifiers of selected objects on success
        '''
        viewport = __viewhelper(view).MainViewport
        screen1 = Rhino.Geometry.Point2d(rhutil.coerce3dpoint(corner1, True))
        screen2 = Rhino.Geometry.Point2d(rhutil.coerce3dpoint(corner2, True))
        xf = viewport.GetTransform(Rhino.DocObjects.CoordinateSystem.World, Rhino.DocObjects.CoordinateSystem.Screen)
        screen1.Transform(xf)
        screen2.Transform(xf)
        objects = None
        filter = Rhino.DocObjects.ObjectType.AnyObject
        if in_window:
            objects = scriptcontext.doc.Objects.FindByWindowRegion(viewport, screen1, screen2, True, filter)
        else:
            objects = scriptcontext.doc.Objects.FindByCrossingWindowRegion(viewport, screen1, screen2, True, filter)
        if objects:
            rc = []
            for rhobj in objects:
                rc.append(rhobj.Id)
                if select: rhobj.Select(True)
            if select: scriptcontext.doc.Views.Redraw()
            return rc
    *)


