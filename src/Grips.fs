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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if enable <> rhobj.GripsOn then
            rhobj.GripsOn <- enable
            Doc.Views.Redraw()
        enable = rhobj.GripsOn
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
    ///<returns>(option<Guid * int * Point3d>) Option of a grip record.
    ///  [0] = identifier of the object that owns the grip
    ///  [1] = index value of the grip
    ///  [2] = location of the grip</returns>
    static member GetObjectGrip( [<OPT;DEF(null:string)>]message:string, 
                                 [<OPT;DEF(false)>]preselect:bool, 
                                 [<OPT;DEF(false)>]select:bool) : option<Guid * int * Point3d> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            if not preselect then
                Doc.Objects.UnselectAll() |> ignore
                Doc.Views.Redraw()
            let grip = ref null
            let rc = Rhino.Input.RhinoGet.GetGrip(grip,message)
            let grip = !grip
            return 
                if rc <> Rhino.Commands.Result.Success || isNull grip then 
                    None
                else
                    if select then
                        grip.Select(true, true)|> ignore
                        Doc.Views.Redraw()
                    Some (grip.OwnerId, grip.Index, grip.CurrentLocation)

            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously

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
    ///<returns>(ResizeArray<Guid * int * Point3d>) containing one or more grip records. Each grip record is a tuple
    ///  [n][0] = identifier of the object that owns the grip
    ///  [n][1] = index value of the grip
    ///  [n][2] = location of the grip</returns>
    static member GetObjectGrips( [<OPT;DEF(null:string)>]message:string, 
                                  [<OPT;DEF(false)>]preselect:bool, 
                                  [<OPT;DEF(false)>]select:bool) : ResizeArray<Guid * int * Point3d> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            if not preselect then
                Doc.Objects.UnselectAll() |> ignore
                Doc.Views.Redraw()
            let grips = ref null
            let re = Rhino.Input.RhinoGet.GetGrips(grips,message)
            let grips = !grips
            let rc = ResizeArray()            
            if re = Rhino.Commands.Result.Success && notNull grips then 
                for grip in grips do
                    let objectId = grip.OwnerId
                    let index = grip.Index
                    let location = grip.CurrentLocation
                    rc.Add((objectId, index, location))
                    if select then grip.Select(true, true)|>ignore
                if select then Doc.Views.Redraw()
            return rc
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously
       
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
            id = grip.OwnerId
            index = grip.Index
            location = grip.CurrentLocation
            rc.append((id, index, location))
            if select: grip.Select(True, True)
        if select: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]    
    static member private Neighborgrip(i, objectId:Guid, index, direction, enable)=
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let grips = rhobj.GetGrips()
        if isNull grips then Error "rhobj.GetGrips() is null"
        else
            if grips.Length <= index then Error "rhobj.GetGrips() failed:  grips.Length <= index "
            else
                let grip = grips.[index]
                let ng = 
                    if direction=0 then
                        grip.NeighborGrip(i,0,0,false)
                    else
                        grip.NeighborGrip(0,i,0,false)
                if notNull ng && enable then
                    ng.Select(true) |> ignore
                    Doc.Views.Redraw()
                Ok ng
   

    [<EXT>]
    ///<summary>Returns the next grip index from a specified grip index of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Zero based grip index from which to get the next grip index</param>
    ///<param name="direction">(int ) Optional, Default Value: <c>0</c>
    ///Direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the next grip index found will be selected</param>
    ///<returns>(int) index of the next grip on success</returns>
    static member NextObjectGrip( objectId:Guid, 
                                  index:int, 
                                  [<OPT;DEF(0)>]direction:int , 
                                  [<OPT;DEF(true)>]enable:bool) : int =
        match RhinoScriptSyntax.Neighborgrip(1, objectId, index, direction, enable) with
        |Ok r -> r.Index
        |Error s -> failwithf "NextObjectGrip failed with %s for index %d, direction %d on %A" s index direction objectId



        
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let grips = rhobj.GetGrips()
        if isNull grips then failwithf "Rhino.Scripting: ObjectGripCount failed.  objectId:'%A'" objectId
        grips.Length
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then failwithf "Rhino.Scripting: ObjectGripLocation failed.  objectId:'%A' index:'%A'" objectId index 
        let grips = rhobj.GetGrips()
        if isNull grips || index<0 || index>=grips.Length then
            failwithf "Rhino.Scripting: ObjectGripLocation failed.  objectId:'%A' index:'%A' " objectId index 
        let grip = grips.[index]
        let rc = grip.CurrentLocation
        rc
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then failwithf "Rhino.Scripting: ObjectGripLocation failed.  objectId:'%A' index:'%A' point:'%A'" objectId index point
        let grips = rhobj.GetGrips()
        if isNull grips || index<0 || index>=grips.Length then
            failwithf "Rhino.Scripting: ObjectGripLocation failed.  objectId:'%A' index:'%A' point:'%A'" objectId index point
        let grip = grips.[index]
        let rc = grip.CurrentLocation       
        grip.CurrentLocation <-  point
        Doc.Objects.GripUpdate(rhobj, true)|> ignore
        Doc.Views.Redraw()
       
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
    ///<returns>(Point3d ResizeArray) The current location of all grips</returns>
    static member ObjectGripLocations(objectId:Guid) : Point3d ResizeArray = //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then failwithf "Rhino.Scripting: ObjectGripLocations failed.  objectId:'%A' " objectId 
        let grips = rhobj.GetGrips()
        if isNull grips then failwithf "Rhino.Scripting: ObjectGripLocations failed.  objectId:'%A' " objectId 
        resizeArray { for grip in grips do yield grip.CurrentLocation }
        
        
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then failwithf "Rhino.Scripting: ObjectGripLocations failed.  objectId:'%A' points:'%A'" objectId points
        let grips = rhobj.GetGrips()
        if grips |> isNull then failwithf "Rhino.Scripting: ObjectGripLocations failed.  objectId:'%A' points:'%A'" objectId points        
        if Seq.length(points) = Seq.length(grips) then            
            for pt, grip in Seq.zip points grips do                
                grip.CurrentLocation <- pt
            Doc.Objects.GripUpdate(rhobj, true)|> ignore
            Doc.Views.Redraw()
        
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.GripsOn
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then false
        else
            let grips = rhobj.GetGrips()
            if isNull grips then false
            else
                grips
                |> Seq.exists (fun g -> g.IsSelected(false) > 0) 

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
    ///<param name="direction">(int) Optional, Default Value: <c>0</c>
    ///Direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the next grip index found will be selected</param>
    ///<returns>(int) index of the next grip on success</returns>
    static member PrevObjectGrip( objectId:Guid, 
                                  index:int, 
                                  [<OPT;DEF(0)>]direction:int, 
                                  [<OPT;DEF(true)>]enable:bool) : int =
        match RhinoScriptSyntax.Neighborgrip(-1, objectId, index, direction, enable) with
        |Ok r -> r.Index
        |Error s -> failwithf "PrevObjectGrip failed with %s for index %d, direction %d on %A" s index direction objectId
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
    ///<returns>(int ResizeArray) list of indices on success</returns>
    static member SelectedObjectGrips(objectId:Guid) : int ResizeArray =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let rc = ResizeArray()
        if not rhobj.GripsOn then rc
        else
            let grips = rhobj.GetGrips()
        
            if notNull grips then
                for i in range(grips.Length) do
                    if grips.[i].IsSelected(false) > 0 then rc.Add(i)
            rc
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then false
        else
            let grips = rhobj.GetGrips()
            if isNull grips then false
            else
                if index<0 || index>=grips.Length then false
                else
                    let grip = grips.[index]
                    if grip.Select(true,true) >0 then
                        Doc.Views.Redraw()
                        true
                    else
                        false
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
    ///<returns>(int) Number of grips selected on success</returns>
    static member SelectObjectGrips(objectId:Guid) : int =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then failwithf "Rhino.Scripting: SelectObjectGrips failed.  objectId:'%A'" objectId
        let grips = rhobj.GetGrips()
        if isNull grips then failwithf "Rhino.Scripting: SelectObjectGrips failed.  objectId:'%A'" objectId
        let mutable count = 0
        for grip in grips do
            if grip.Select(true,true)>0 then count<- count +  1
        if count>0 then
            Doc.Views.Redraw()
            count
        else
            failwithf "Rhino.Scripting: SelectObjectGrips failed.  objectId:'%A'" objectId
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then false
        else
            let grips = rhobj.GetGrips()
            if isNull grips then false
            else
                if index<0 || index>=grips.Length then false
                else
                    let grip = grips.[index]
                    if grip.Select(false) = 0 then
                        Doc.Views.Redraw()
                        true
                    else
                        false
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
    ///<returns>(int) Number of grips unselected on success</returns>
    static member UnselectObjectGrips(objectId:Guid) : int =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then failwithf "Rhino.Scripting: UnselectObjectGrips failed.  objectId:'%A'" objectId
        let grips = rhobj.GetGrips()
        if isNull grips then failwithf "Rhino.Scripting: UnselectObjectGrips failed.  objectId:'%A'" objectId
        let mutable count = 0
        for grip in grips do
            if grip.Select(false) = 0 then count <- count +   1
        if count>0 then
            Doc.Views.Redraw()
            count
        else
            failwithf "Rhino.Scripting: UnselectObjectGrips failed.  objectId:'%A'" objectId
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


