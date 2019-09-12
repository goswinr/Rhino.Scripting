namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsGrips =
  type RhinoScriptSyntax with
    
    ///<summary>Enables or disables an object's grips. For curves and surfaces, these are
    ///  also called control points.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the specified object's grips will be turned on.
    ///  Otherwise, they will be turned off</param>
    ///<returns>(bool) True on success, False on failure</returns>
    static member EnableObjectGrips(objectId:Guid, [<OPT;DEF(true)>]enable:bool) : bool =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if enable<>rhobj.GripsOn then
            let rhobj.GripsOn = enable
            Doc.Views.Redraw()
    (*
    def EnableObjectGrips(object_id, enable=True):
        """Enables or disables an object's grips. For curves and surfaces, these are
        also called control points.
        Parameters:
          object_id (guid): identifier of the object
          enable (bool, optional): if True, the specified object's grips will be turned on.
            Otherwise, they will be turned off
        Returns:
          bool: True on success, False on failure
        Example:
          import rhinoscriptsyntax as  rs
          objects = rs.GetObjects("Select  objects")
          if objects: [rs.EnableObjectGrips(obj)  for obj in objs]
        See Also:
          ObjectGripCount
          ObjectGripsOn
          ObjectGripsSelected
          SelectObjectGrips
          UnselectObjectGrips
        """
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if enable!=rhobj.GripsOn:
            rhobj.GripsOn = enable
            scriptcontext.doc.Views.Redraw()
    *)


    ///<summary>Prompts the user to pick a single object grip</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///Prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for selection of pre-selected object grip.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked object grip.</param>
    ///<returns>(Guid * float * Point3d) defining a grip record.
    ///  [0] = identifier of the object that owns the grip
    ///  [1] = index value of the grip
    ///  [2] = location of the grip</returns>
    static member GetObjectGrip([<OPT;DEF(null)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid * float * Point3d =
        if not <| preselect then
            Doc.Objects.UnselectAll()
            Doc.Views.Redraw()
        let rc, grip = Rhino.Input.RhinoGet.GetGrip(message)
        if rc<>Rhino.Commands.Result.Success then failwithf "Rhino.Scripting Error:GetObjectGrip failed.  message:"%A" preselect:"%A" select:"%A"" message preselect select
        if select then
            grip.Select(true, true)
            Doc.Views.Redraw()
        grip.OwnerId, grip.Index, grip.CurrentLocation
    (*
    def GetObjectGrip(message=None, preselect=False, select=False):
        """Prompts the user to pick a single object grip
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
        Example:
          import rhinoscriptsyntax as rs
          curve = rs.GetObject("Select a curve", rs.filter.curve)
          if curve:
              rs.EnableObjectGrips( curve )
              grip = rs.GetObjectGrip("Select a curve grip")
              if grip: print grip[2]
        See Also:
          GetObjectGrips
        """
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


    ///<summary>Prompts user to pick one or more object grips from one or more objects.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///Prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for selection of pre-selected object grips</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked object grips</param>
    ///<returns>(Guid seq) containing one or more grip records. Each grip record is a tuple
    ///  [n][0] = identifier of the object that owns the grip
    ///  [n][1] = index value of the grip
    ///  [n][2] = location of the grip</returns>
    static member GetObjectGrips([<OPT;DEF(null)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid seq =
        if not <| preselect then
            Doc.Objects.UnselectAll()
            Doc.Views.Redraw()
        let getrc, grips = Rhino.Input.RhinoGet.GetGrips(message)
        if getrc<>Rhino.Commands.Result.Success || not <| grips then
            failwithf "Rhino.Scripting Error:GetObjectGrips failed.  message:"%A" preselect:"%A" select:"%A"" message preselect select
        let rc = ResizeArray()
        for grip in grips do
            let id = grip.OwnerId
            let index = grip.Index
            let location = grip.CurrentLocation
            rc.Add((id, index, location))
            if select then grip.Select(true, true)
        if select then Doc.Views.Redraw()
        rc
    (*
    def GetObjectGrips(message=None, preselect=False, select=False):
        """Prompts user to pick one or more object grips from one or more objects.
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
        Example:
          import rhinoscriptsyntax as rs
          curves = rs.GetObjects("Select curves", rs.filter.curves)
          if curves:
              for curve in curves: rs.EnableObjectGrips(curve)
              grips = rs.GetObjectGrips("Select curve grips")
              if grips: for grip in grips: print grip[0]
        See Also:
          GetObjectGrip
        """
        if not preselect:
            scriptcontext.doc.Objects.UnselectAll()
            scriptcontext.doc.Views.Redraw()
        getrc, grips = Rhino.Input.RhinoGet.GetGrips(message)
        if getrc!=Rhino.Commands.Result.Success or not grips:
            return scriptcontext.errorhandler()
        rc = []
        for grip in grips:
            id = grip.OwnerId
            index = grip.Index
            location = grip.CurrentLocation
            rc.append((id, index, location))
            if select: grip.Select(True, True)
        if select: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    
    static member internal Neighborgrip() : obj =
    (*
    def __neighborgrip(i, object_id, index, direction, enable):
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        grips = rhobj.GetGrips()
        if not grips or len(grips)<=index: return scriptcontext.errorhandler()
        grip = grips[index]
        next_grip=None
        if direction==0:
            next_grip = grip.NeighborGrip(i,0,0,False)
        else:
            next_grip = grip.NeighborGrip(0,i,0,False)
        if next_grip and enable:
            next_grip.Select(True)
            scriptcontext.doc.Views.Redraw()
        return next_grip
    *)


    ///<summary>Returns the next grip index from a specified grip index of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Zero based grip index from which to get the next grip index</param>
    ///<param name="direction">(float * float) Optional, Default Value: <c>0</c>
    ///Direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the next grip index found will be selected</param>
    ///<returns>(float) index of the next grip on success</returns>
    static member NextObjectGrip(objectId:Guid, index:int, [<OPT;DEF(0)>]direction:float * float, [<OPT;DEF(true)>]enable:bool) : float =
        __neighborgrip(1, objectId, index, direction, enable)
    (*
    def NextObjectGrip(object_id, index, direction=0, enable=True):
        """Returns the next grip index from a specified grip index of an object
        Parameters:
          object_id (guid): identifier of the object
          index (number): zero based grip index from which to get the next grip index
          direction ([number, number], optional): direction to get the next grip index (0=U, 1=V)
          enable (bool, optional): if True, the next grip index found will be selected
        Returns:
          number: index of the next grip on success
          None: on failure
        Example:
          import rhinoscriptsyntax as rs
          object_id = rs.GetObject("Select curve", rs.filter.curve)
          if object_id:
              rs.EnableObjectGrips( object_id )
              count = rs.ObjectGripCount( object_id )
              for i in range(0,count,2):
                  rs.NextObjectGrip(object_id, i, 0, True)
        See Also:
          EnableObjectGrips
          PrevObjectGrip
        """
        return __neighborgrip(1, object_id, index, direction, enable)
    *)


    ///<summary>Returns number of grips owned by an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int) number of grips</returns>
    static member ObjectGripCount(objectId:Guid) : int =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        let grips = rhobj.GetGrips()
        if not <| grips then failwithf "Rhino.Scripting Error:ObjectGripCount failed.  objectId:"%A"" objectId
        grips.Length
    (*
    def ObjectGripCount(object_id):
        """Returns number of grips owned by an object
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          number: number of grips if successful
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if rs.ObjectGripsOn(obj):
              print "Grip count =", rs.ObjectGripCount(obj)
        See Also:
          EnableObjectGrips
          ObjectGripsOn
          ObjectGripsSelected
          SelectObjectGrips
          UnselectObjectGrips
        """
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        grips = rhobj.GetGrips()
        if not grips: return scriptcontext.errorhandler()
        return grips.Length
    *)


    ///<summary>Returns the location of an object's grip</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to either query or modify</param>
    ///<returns>(Point3d) The current location of the grip referenced by index</returns>
    static member ObjectGripLocation(objectId:Guid, index:int) : Point3d =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then failwithf "Rhino.Scripting Error:ObjectGripLocation failed.  objectId:"%A" index:"%A" point:"%A"" objectId index point
        let grips = rhobj.GetGrips()
        if not <| grips || index<0 || index>=grips.Length then
            failwithf "Rhino.Scripting Error:ObjectGripLocation failed.  objectId:"%A" index:"%A" point:"%A"" objectId index point
        let grip = grips.[index]
        let rc = grip.CurrentLocation
        if point then
            let grip.CurrentLocation = Coerce.coerce3dpoint(point, true)
            Doc.Objects.GripUpdate(rhobj, true)
            Doc.Views.Redraw()
        rc
    (*
    def ObjectGripLocation(object_id, index, point=None):
        """Returns or modifies the location of an object's grip
        Parameters:
          object_id (guid) identifier of the object
          index (number): index of the grip to either query or modify
          point (point, optional): 3D point defining new location of the grip
        Returns:
          point: if point is not specified, the current location of the grip referenced by index
          point: if point is specified, the previous location of the grip referenced by index
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select curve", rs.filter.curve)
          if obj:
              rs.EnableObjectGrips(obj)
              point = rs.ObjectGripLocation(obj, 0)
              point[0] = point[0] + 1.0
              point[1] = point[1] + 1.0
              point[2] = point[2] + 1.0
              rs.ObjectGripLocation(obj, 0, point)
              rs.EnableObjectGrips(obj, False)
        See Also:
          EnableObjectGrips
          ObjectGripLocations
        """
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
    ///<returns>(unit) unit</returns>
    static member ObjectGripLocation(objectId:Guid, index:int, point:Point3d) : unit =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then failwithf "Rhino.Scripting Error:ObjectGripLocation failed.  objectId:"%A" index:"%A" point:"%A"" objectId index point
        let grips = rhobj.GetGrips()
        if not <| grips || index<0 || index>=grips.Length then
            failwithf "Rhino.Scripting Error:ObjectGripLocation failed.  objectId:"%A" index:"%A" point:"%A"" objectId index point
        let grip = grips.[index]
        let rc = grip.CurrentLocation
        if point then
            let grip.CurrentLocation = Coerce.coerce3dpoint(point, true)
            Doc.Objects.GripUpdate(rhobj, true)
            Doc.Views.Redraw()
        rc
    (*
    def ObjectGripLocation(object_id, index, point=None):
        """Returns or modifies the location of an object's grip
        Parameters:
          object_id (guid) identifier of the object
          index (number): index of the grip to either query or modify
          point (point, optional): 3D point defining new location of the grip
        Returns:
          point: if point is not specified, the current location of the grip referenced by index
          point: if point is specified, the previous location of the grip referenced by index
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select curve", rs.filter.curve)
          if obj:
              rs.EnableObjectGrips(obj)
              point = rs.ObjectGripLocation(obj, 0)
              point[0] = point[0] + 1.0
              point[1] = point[1] + 1.0
              point[2] = point[2] + 1.0
              rs.ObjectGripLocation(obj, 0, point)
              rs.EnableObjectGrips(obj, False)
        See Also:
          EnableObjectGrips
          ObjectGripLocations
        """
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


    ///<summary>Returns the location of all grips owned by an object. The
    /// locations of the grips are returned in a list of Point3d with each position
    /// in the list corresponding to that grip's index. To modify the locations of
    /// the grips, you must provide a list of points that contain the same number
    /// of points at grips</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(Point3d seq) The current location of all grips</returns>
    static member ObjectGripLocations(objectId:Guid) : Point3d seq =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then failwithf "Rhino.Scripting Error:ObjectGripLocations failed.  objectId:"%A" points:"%A"" objectId points
        let grips = rhobj.GetGrips()
        if grips = null then failwithf "Rhino.Scripting Error:ObjectGripLocations failed.  objectId:"%A" points:"%A"" objectId points
        let rc = [| for grip in grips -> grip.CurrentLocation |]
        if points && Seq.length(points)=len(grips) then
            let points = Coerce.coerce3dpointlist(points, true)
            for i, grip in enumerate(grips) do
                let point = points.[i]
                let grip.CurrentLocation = point
            Doc.Objects.GripUpdate(rhobj, true)
            Doc.Views.Redraw()
        rc
    (*
    def ObjectGripLocations(object_id, points=None):
        """Returns or modifies the location of all grips owned by an object. The
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
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select curve", rs.filter.curve)
          if obj:
              rs.EnableObjectGrips( obj )
              points = rs.ObjectGripLocations(obj)
              for point in points:  print point
        See Also:
          EnableObjectGrips
          ObjectGripCount
          ObjectGripLocation
        """
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
    ///<returns>(unit) unit</returns>
    static member ObjectGripLocations(objectId:Guid, points:Point3d seq) : unit =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then failwithf "Rhino.Scripting Error:ObjectGripLocations failed.  objectId:"%A" points:"%A"" objectId points
        let grips = rhobj.GetGrips()
        if grips = null then failwithf "Rhino.Scripting Error:ObjectGripLocations failed.  objectId:"%A" points:"%A"" objectId points
        let rc = [| for grip in grips -> grip.CurrentLocation |]
        if points && Seq.length(points)=len(grips) then
            let points = Coerce.coerce3dpointlist(points, true)
            for i, grip in enumerate(grips) do
                let point = points.[i]
                let grip.CurrentLocation = point
            Doc.Objects.GripUpdate(rhobj, true)
            Doc.Views.Redraw()
        rc
    (*
    def ObjectGripLocations(object_id, points=None):
        """Returns or modifies the location of all grips owned by an object. The
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
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select curve", rs.filter.curve)
          if obj:
              rs.EnableObjectGrips( obj )
              points = rs.ObjectGripLocations(obj)
              for point in points:  print point
        See Also:
          EnableObjectGrips
          ObjectGripCount
          ObjectGripLocation
        """
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


    ///<summary>Verifies that an object's grips are turned on</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(bool) True or False indicating Grips state</returns>
    static member ObjectGripsOn(objectId:Guid) : bool =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        rhobj.GripsOn
    (*
    def ObjectGripsOn(object_id):
        """Verifies that an object's grips are turned on
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          bool: True or False indicating Grips state
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if rs.ObjectGripsOn(obj):
              print "Grip count =", rs.ObjectGripCount(obj)
        See Also:
          EnableObjectGrips
          ObjectGripCount
          ObjectGripsSelected
          SelectObjectGrips
          UnselectObjectGrips
        """
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.GripsOn
    *)


    ///<summary>Verifies that an object's grips are turned on and at least one grip
    ///  is selected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ObjectGripsSelected(objectId:Guid) : bool =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then false
        let grips = rhobj.GetGrips()
        if grips = null then false
        for grip in grips do
            if grip.IsSelected(false) then true
        false
    (*
    def ObjectGripsSelected(object_id):
        """Verifies that an object's grips are turned on and at least one grip
        is selected
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if rs.ObjectGripsSelected(obj):
              rs.UnselectObjectGrips( obj )
        See Also:
          EnableObjectGrips
          ObjectGripCount
          ObjectGripsOn
          SelectObjectGrips
          UnselectObjectGrips
        """
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return False
        grips = rhobj.GetGrips()
        if grips is None: return False
        for grip in grips:
            if grip.IsSelected(False): return True
        return False
    *)


    ///<summary>Returns the previous grip index from a specified grip index of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Zero based grip index from which to get the previous grip index</param>
    ///<param name="direction">(float * float) Optional, Default Value: <c>0</c>
    ///Direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the next grip index found will be selected</param>
    ///<returns>(float) index of the next grip on success</returns>
    static member PrevObjectGrip(objectId:Guid, index:int, [<OPT;DEF(0)>]direction:float * float, [<OPT;DEF(true)>]enable:bool) : float =
        __neighborgrip(-1, objectId, index, direction, enable)
    (*
    def PrevObjectGrip(object_id, index, direction=0, enable=True):
        """Returns the previous grip index from a specified grip index of an object
        Parameters:
          object_id (guid): identifier of the object
          index (number): zero based grip index from which to get the previous grip index
          direction ([number, number], optional): direction to get the next grip index (0=U, 1=V)
          enable (bool, optional): if True, the next grip index found will be selected
        Returns:
          number: index of the next grip on success
          None: on failure
        Example:
          import rhinoscriptsyntax as rs
          object_id = rs.GetObject("Select curve", rs.filter.curve)
          if object_id:
              rs.EnableObjectGrips(object_id)
              count = rs.ObjectGripCount(object_id)
              for i in range(count-1, 0, -2):
                  rs.PrevObjectGrip(object_id, i, 0, True)
        See Also:
          EnableObjectGrips
          NextObjectGrip
        """
        return __neighborgrip(-1, object_id, index, direction, enable)
    *)


    ///<summary>Returns a list of grip indices indentifying an object's selected grips</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int seq) list of indices on success</returns>
    static member SelectedObjectGrips(objectId:Guid) : int seq =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then null
        let grips = rhobj.GetGrips()
        let rc = ResizeArray()
        if grips then
            for i in xrange(grips.Length) do
                if grips.[i].IsSelected(false) then rc.Add(i)
        rc
    (*
    def SelectedObjectGrips(object_id):
        """Returns a list of grip indices indentifying an object's selected grips
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          list(int,...): list of indices on success
          None: on failure or if no grips are selected
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select curve", rs.filter.curve)
          if obj:
              rs.EnableObjectGrips( obj )
              count = rs.ObjectGripCount( obj )
              for i in xrange(0,count,2):
                  rs.SelectObjectGrip( obj, i )
              grips = rs.SelectedObjectGrips(obj)
              if grips: print len(grips), "grips selected"
        See Also:
          EnableObjectGrips
          SelectObjectGrip
          SelectObjectGrips
        """
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        if not rhobj.GripsOn: return None
        grips = rhobj.GetGrips()
        rc = []
        if grips:
            for i in xrange(grips.Length):
                if grips[i].IsSelected(False): rc.append(i)
        return rc
    *)


    ///<summary>Selects a single grip owned by an object. If the object's grips are
    ///  not turned on, the grips will not be selected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to select</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SelectObjectGrip(objectId:Guid, index:int) : bool =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then false
        let grips = rhobj.GetGrips()
        if grips = null then false
        if index<0 || index>=grips.Length then false
        let grip = grips.[index]
        if grip.Select(true,true)>0 then
            Doc.Views.Redraw()
            true
        false
    (*
    def SelectObjectGrip(object_id, index):
        """Selects a single grip owned by an object. If the object's grips are
        not turned on, the grips will not be selected
        Parameters:
          object_id (guid) identifier of the object
          index (number): index of the grip to select
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select curve", rs.filter.curve)
          if obj:
              rs.EnableObjectGrips( obj )
              count = rs.ObjectGripCount( obj )
              for i in xrange(0,count,2): rs.SelectObjectGrip(obj,i)
        See Also:
          EnableObjectGrips
          ObjectGripCount
          SelectObjectGrips
        """
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


    ///<summary>Selects an object's grips. If the object's grips are not turned on,
    ///  they will not be selected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) Number of grips selected on success</returns>
    static member SelectObjectGrips(objectId:Guid) : float =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then failwithf "Rhino.Scripting Error:SelectObjectGrips failed.  objectId:"%A"" objectId
        let grips = rhobj.GetGrips()
        if grips = null then failwithf "Rhino.Scripting Error:SelectObjectGrips failed.  objectId:"%A"" objectId
        let count = 0
        for grip in grips do
            if grip.Select(true,true)>0 then count+<-1
        if count>0 then
            Doc.Views.Redraw()
            count
        failwithf "Rhino.Scripting Error:SelectObjectGrips failed.  objectId:"%A"" objectId
    (*
    def SelectObjectGrips(object_id):
        """Selects an object's grips. If the object's grips are not turned on,
        they will not be selected
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          number: Number of grips selected on success
          None: on failure
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if rs.ObjectGripsSelected(obj)==False:
              rs.SelectObjectGrips( obj )
        See Also:
          EnableObjectGrips
          ObjectGripCount
          SelectObjectGrip
        """
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


    ///<summary>Unselects a single grip owned by an object. If the object's grips are
    ///  not turned on, the grips will not be unselected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to unselect</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member UnselectObjectGrip(objectId:Guid, index:int) : bool =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then false
        let grips = rhobj.GetGrips()
        if grips = null then false
        if index<0 || index>=grips.Length then false
        let grip = grips.[index]
        if grip.Select(false)=0 then
            Doc.Views.Redraw()
            true
        false
    (*
    def UnselectObjectGrip(object_id, index):
        """Unselects a single grip owned by an object. If the object's grips are
        not turned on, the grips will not be unselected
        Parameters:
          object_id (guid): identifier of the object
          index (number): index of the grip to unselect
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select curve", rs.filter.curve)
          if obj:
              rs.EnableObjectGrips( obj )
              count = rs.ObjectGripCount(obj)
              for i in xrange(0,count,2):
                  rs.UnselectObjectGrip( obj, i )
        See Also:
          EnableObjectGrips
          ObjectGripCount
          UnselectObjectGrips
        """
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


    ///<summary>Unselects an object's grips. Note, the grips will not be turned off.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) Number of grips unselected on success</returns>
    static member UnselectObjectGrips(objectId:Guid) : float =
        let rhobj = Coerce.coercerhinoobject(objectId, true, true)
        if not <| rhobj.GripsOn then failwithf "Rhino.Scripting Error:UnselectObjectGrips failed.  objectId:"%A"" objectId
        let grips = rhobj.GetGrips()
        if grips = null then failwithf "Rhino.Scripting Error:UnselectObjectGrips failed.  objectId:"%A"" objectId
        let count = 0
        for grip in grips do
            if grip.Select(false)=0 then count +<- 1
        if count>0 then
            Doc.Views.Redraw()
            count
        failwithf "Rhino.Scripting Error:UnselectObjectGrips failed.  objectId:"%A"" objectId
    (*
    def UnselectObjectGrips(object_id):
        """Unselects an object's grips. Note, the grips will not be turned off.
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          number: Number of grips unselected on success
          None: on failure
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if rs.ObjectGripsSelected(obj): rs.UnselectObjectGrips(obj)
        See Also:
          EnableObjectGrips
          ObjectGripCount
          UnselectObjectGrip
        """
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


