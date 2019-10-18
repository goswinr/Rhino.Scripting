namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsGrips =
  type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Enables or disables an object's grips. For curves and surfaces, these are
    ///  also called control points.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the specified object's grips will be turned on.
    ///  Otherwise, they will be turned off</param>
    ///<returns>(bool) True on success, False on failure</returns>
    static member EnableObjectGrips(objectId:Guid, [<OPT;DEF(true)>]enable:bool) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def EnableObjectGrips(object_id, enable=True):
        '''Enables or disables an object's grips. For curves and surfaces, these are
        also called control points.
        Parameters:
          object_id (guid): identifier of the object
          enable (bool, optional): if True, the specified object's grips will be turned on.
            Otherwise, they will be turned off
        Returns:
          bool: True on success, False on failure
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if enable!=rhobj.GripsOn:
            rhobj.GripsOn = enable
            scriptcontext.doc.Views.Redraw()
    *)


    [<EXT>]
    ///<summary>Prompts the user to pick a single object grip</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for selection of pre-selected object grip.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked object grip.</param>
    ///<returns>(Guid * float * Point3d) defining a grip record.
    ///  [0] = identifier of the object that owns the grip
    ///  [1] = index value of the grip
    ///  [2] = location of the grip</returns>
    static member GetObjectGrip([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid * float * Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def GetObjectGrip(message=None, preselect=False, select=False):
        '''Prompts the user to pick a single object grip
        Parameters:
          message (str, optional): prompt for picking
          preselect (bool, optional): allow for selection of pre-selected object grip.
          select (bool, optional): select the picked object grip.
        Returns:
          tuple(guid, number, point): defining a grip record.
             [0] = identifier of the object that owns the grip
             [1] = index value of the grip
             [2] = location of the grip
          None: on error
        '''
        if not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        rc, grip = Rhino.Input.RhinoGet.GetGrip(message)
        if rc!=Rhino.Commands.Result.Success: return scriptcontext.errorhandler()
        if select:
            grip.Select(True, True)
            scriptcontext.doc.Views.Redraw()
        return grip.OwnerId, grip.Index, grip.CurrentLocation
    *)


    [<EXT>]
    ///<summary>Prompts user to pick one or more object grips from one or more objects.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for selection of pre-selected object grips</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked object grips</param>
    ///<returns>(Guid seq) containing one or more grip records. Each grip record is a tuple
    ///  [n][0] = identifier of the object that owns the grip
    ///  [n][1] = index value of the grip
    ///  [n][2] = location of the grip</returns>
    static member GetObjectGrips([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def GetObjectGrips(message=None, preselect=False, select=False):
        '''Prompts user to pick one or more object grips from one or more objects.
        Parameters:
          message (str, optional): prompt for picking
          preselect (bool, optional): allow for selection of pre-selected object grips
          select (bool, optional) select the picked object grips
        Returns:
          list((guid, number, point), ...): containing one or more grip records. Each grip record is a tuple
            [n][0] = identifier of the object that owns the grip
            [n][1] = index value of the grip
            [n][2] = location of the grip
          None: on error
        '''
        if not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        getrc, grips = Rhino.Input.RhinoGet.GetGrips(message)
        if getrc!=Rhino.Commands.Result.Success or not grips:
            return scriptcontext.errorhandler()
        rc = []
        for grip in grips:
            objectId = grip.OwnerId
            index = grip.Index
            location = grip.CurrentLocation
            rc.append((objectId, index, location))
            if select: grip.Select(True, True)
        if select: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    
    static member internal Neighborgrip() : obj =
        failNotImpl () // genreation temp disabled !!
    (*
    def __neighborgrip(i, object_id, index, direction, enable):
        ''''''
    *)


    [<EXT>]
    ///<summary>Returns the next grip index from a specified grip index of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Zero based grip index from which to get the next grip index</param>
    ///<param name="direction">(int * int) Optional, Default Value: <c>0</c>
    ///Direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the next grip index found will be selected</param>
    ///<returns>(float) index of the next grip on success</returns>
    static member NextObjectGrip(objectId:Guid, index:int, [<OPT;DEF(0)>]direction:int * int, [<OPT;DEF(true)>]enable:bool) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def NextObjectGrip(object_id, index, direction=0, enable=True):
        '''Returns the next grip index from a specified grip index of an object
        Parameters:
          object_id (guid): identifier of the object
          index (number): zero based grip index from which to get the next grip index
          direction ([number, number], optional): direction to get the next grip index (0=U, 1=V)
          enable (bool, optional): if True, the next grip index found will be selected
        Returns:
          number: index of the next grip on success
          None: on failure
        '''
        return __neighborgrip(1, object_id, index, direction, enable)
    *)


    [<EXT>]
    ///<summary>Returns number of grips owned by an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int) number of grips</returns>
    static member ObjectGripCount(objectId:Guid) : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectGripCount(object_id):
        '''Returns number of grips owned by an object
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          number: number of grips if successful
          None: on error
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        grips = rhobj.GetGrips()
        if not grips: return scriptcontext.errorhandler()
        return grips.Length
    *)


    [<EXT>]
    ///<summary>Returns the location of an object's grip</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to either query or modify</param>
    ///<returns>(Point3d) The current location of the grip referenced by index</returns>
    static member ObjectGripLocation(objectId:Guid, index:int) : Point3d = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectGripLocation(object_id, index, point=None):
        '''Returns or modifies the location of an object's grip
        Parameters:
          object_id (guid) identifier of the object
          index (number): index of the grip to either query or modify
          point (point, optional): 3D point defining new location of the grip
        Returns:
          point: if point is not specified, the current location of the grip referenced by index
          point: if point is specified, the previous location of the grip referenced by index
          None: on error
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return scriptcontext.errorhandler()
        grips = rhobj.GetGrips()
        if not grips or index<0 or index>=grips.Length:
            return scriptcontext.errorhandler()
        grip = grips[index]
        rc = grip.CurrentLocation
        if point:
            grip.CurrentLocation = rhutil.coerce3dpoint(point, True)
            scriptcontext.doc.Objects.GripUpdate(rhobj, True)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the location of an object's grip</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to either query or modify</param>
    ///<param name="point">(Point3d)3D point defining new location of the grip</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectGripLocation(objectId:Guid, index:int, point:Point3d) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectGripLocation(object_id, index, point=None):
        '''Returns or modifies the location of an object's grip
        Parameters:
          object_id (guid) identifier of the object
          index (number): index of the grip to either query or modify
          point (point, optional): 3D point defining new location of the grip
        Returns:
          point: if point is not specified, the current location of the grip referenced by index
          point: if point is specified, the previous location of the grip referenced by index
          None: on error
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return scriptcontext.errorhandler()
        grips = rhobj.GetGrips()
        if not grips or index<0 or index>=grips.Length:
            return scriptcontext.errorhandler()
        grip = grips[index]
        rc = grip.CurrentLocation
        if point:
            grip.CurrentLocation = rhutil.coerce3dpoint(point, True)
            scriptcontext.doc.Objects.GripUpdate(rhobj, True)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the location of all grips owned by an object. The
    /// locations of the grips are returned in a list of Point3d with each position
    /// in the list corresponding to that grip's index. To modify the locations of
    /// the grips, you must provide a list of points that contain the same number
    /// of points at grips</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(Point3d seq) The current location of all grips</returns>
    static member ObjectGripLocations(objectId:Guid) : Point3d seq = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectGripLocations(object_id, points=None):
        '''Returns or modifies the location of all grips owned by an object. The
        locations of the grips are returned in a list of Point3d with each position
        in the list corresponding to that grip's index. To modify the locations of
        the grips, you must provide a list of points that contain the same number
        of points at grips
        Parameters:
          object_id (guid): identifier of the object
          points ([point, ...], optional) list of 3D points identifying the new grip locations
        Returns:
          list(point, ...): if points is not specified, the current location of all grips
          list(point, ...): if points is specified, the previous location of all grips
          None: if not successful
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return scriptcontext.errorhandler()
        grips = rhobj.GetGrips()
        if grips is None: return scriptcontext.errorhandler()
        rc = [grip.CurrentLocation for grip in grips]
        if points and len(points)==len(grips):
            points = rhutil.coerce3dpointlist(points, True)
            for i, grip in enumerate(grips):
                point = points[i]
                grip.CurrentLocation = point
            scriptcontext.doc.Objects.GripUpdate(rhobj, True)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the location of all grips owned by an object. The
    /// locations of the grips are returned in a list of Point3d with each position
    /// in the list corresponding to that grip's index. To modify the locations of
    /// the grips, you must provide a list of points that contain the same number
    /// of points at grips</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="points">(Point3d seq)List of 3D points identifying the new grip locations</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectGripLocations(objectId:Guid, points:Point3d seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectGripLocations(object_id, points=None):
        '''Returns or modifies the location of all grips owned by an object. The
        locations of the grips are returned in a list of Point3d with each position
        in the list corresponding to that grip's index. To modify the locations of
        the grips, you must provide a list of points that contain the same number
        of points at grips
        Parameters:
          object_id (guid): identifier of the object
          points ([point, ...], optional) list of 3D points identifying the new grip locations
        Returns:
          list(point, ...): if points is not specified, the current location of all grips
          list(point, ...): if points is specified, the previous location of all grips
          None: if not successful
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return scriptcontext.errorhandler()
        grips = rhobj.GetGrips()
        if grips is None: return scriptcontext.errorhandler()
        rc = [grip.CurrentLocation for grip in grips]
        if points and len(points)==len(grips):
            points = rhutil.coerce3dpointlist(points, True)
            for i, grip in enumerate(grips):
                point = points[i]
                grip.CurrentLocation = point
            scriptcontext.doc.Objects.GripUpdate(rhobj, True)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Verifies that an object's grips are turned on</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(bool) True or False indicating Grips state</returns>
    static member ObjectGripsOn(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectGripsOn(object_id):
        '''Verifies that an object's grips are turned on
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          bool: True or False indicating Grips state
          None: on error
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.GripsOn
    *)


    [<EXT>]
    ///<summary>Verifies that an object's grips are turned on and at least one grip
    ///  is selected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ObjectGripsSelected(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectGripsSelected(object_id):
        '''Verifies that an object's grips are turned on and at least one grip
        is selected
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          bool: True or False indicating success or failure
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return False
        grips = rhobj.GetGrips()
        if grips is None: return False
        for grip in grips:
            if grip.IsSelected(False): return True
        return False
    *)


    [<EXT>]
    ///<summary>Returns the previous grip index from a specified grip index of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Zero based grip index from which to get the previous grip index</param>
    ///<param name="direction">(int * int) Optional, Default Value: <c>0</c>
    ///Direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the next grip index found will be selected</param>
    ///<returns>(float) index of the next grip on success</returns>
    static member PrevObjectGrip(objectId:Guid, index:int, [<OPT;DEF(0)>]direction:int * int, [<OPT;DEF(true)>]enable:bool) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def PrevObjectGrip(object_id, index, direction=0, enable=True):
        '''Returns the previous grip index from a specified grip index of an object
        Parameters:
          object_id (guid): identifier of the object
          index (number): zero based grip index from which to get the previous grip index
          direction ([number, number], optional): direction to get the next grip index (0=U, 1=V)
          enable (bool, optional): if True, the next grip index found will be selected
        Returns:
          number: index of the next grip on success
          None: on failure
        '''
        return __neighborgrip(-1, object_id, index, direction, enable)
    *)


    [<EXT>]
    ///<summary>Returns a list of grip indices indentifying an object's selected grips</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int seq) list of indices on success</returns>
    static member SelectedObjectGrips(objectId:Guid) : int seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def SelectedObjectGrips(object_id):
        '''Returns a list of grip indices indentifying an object's selected grips
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          list(int,...): list of indices on success
          None: on failure or if no grips are selected
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return None
        grips = rhobj.GetGrips()
        rc = []
        if grips:
            for i in xrange(grips.Length):
                if grips[i].IsSelected(False): rc.append(i)
        return rc
    *)


    [<EXT>]
    ///<summary>Selects a single grip owned by an object. If the object's grips are
    ///  not turned on, the grips will not be selected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to select</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SelectObjectGrip(objectId:Guid, index:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def SelectObjectGrip(object_id, index):
        '''Selects a single grip owned by an object. If the object's grips are
        not turned on, the grips will not be selected
        Parameters:
          object_id (guid) identifier of the object
          index (number): index of the grip to select
        Returns:
          bool: True or False indicating success or failure
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return False
        grips = rhobj.GetGrips()
        if grips is None: return False
        if index<0 or index>=grips.Length: return False
        grip = grips[index]
        if grip.Select(True,True)>0:
            scriptcontext.doc.Views.Redraw()
            return True
        return False
    *)


    [<EXT>]
    ///<summary>Selects an object's grips. If the object's grips are not turned on,
    ///  they will not be selected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) Number of grips selected on success</returns>
    static member SelectObjectGrips(objectId:Guid) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SelectObjectGrips(object_id):
        '''Selects an object's grips. If the object's grips are not turned on,
        they will not be selected
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          number: Number of grips selected on success
          None: on failure
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return scriptcontext.errorhandler()
        grips = rhobj.GetGrips()
        if grips is None: return scriptcontext.errorhandler()
        count = 0
        for grip in grips:
            if grip.Select(True,True)>0: count+=1
        if count>0:
            scriptcontext.doc.Views.Redraw()
            return count
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Unselects a single grip owned by an object. If the object's grips are
    ///  not turned on, the grips will not be unselected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to unselect</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member UnselectObjectGrip(objectId:Guid, index:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def UnselectObjectGrip(object_id, index):
        '''Unselects a single grip owned by an object. If the object's grips are
        not turned on, the grips will not be unselected
        Parameters:
          object_id (guid): identifier of the object
          index (number): index of the grip to unselect
        Returns:
          bool: True or False indicating success or failure
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return False
        grips = rhobj.GetGrips()
        if grips is None: return False
        if index<0 or index>=grips.Length: return False
        grip = grips[index]
        if grip.Select(False)==0:
            scriptcontext.doc.Views.Redraw()
            return True
        return False
    *)


    [<EXT>]
    ///<summary>Unselects an object's grips. Note, the grips will not be turned off.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) Number of grips unselected on success</returns>
    static member UnselectObjectGrips(objectId:Guid) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def UnselectObjectGrips(object_id):
        '''Unselects an object's grips. Note, the grips will not be turned off.
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          number: Number of grips unselected on success
          None: on failure
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return scriptcontext.errorhandler()
        grips = rhobj.GetGrips()
        if grips is None: return scriptcontext.errorhandler()
        count = 0
        for grip in grips:
            if grip.Select(False)==0: count += 1
        if count>0:
            scriptcontext.doc.Views.Redraw()
            return count
        return scriptcontext.errorhandler()
    *)


