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


    [<EXT>]
    ///<summary>Prompts the user to pick a single object grip</summary>
    ///<param name="message">(string) Optional, Prompt for picking</param>
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
            let rc = Input.RhinoGet.GetGrip(grip,message)
            let grip = !grip
            return
                if rc <> Commands.Result.Success || isNull grip then
                    None
                else
                    if select then
                        grip.Select(true, true)|> ignore
                        Doc.Views.Redraw()
                    Some (grip.OwnerId, grip.Index, grip.CurrentLocation)

            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously



    [<EXT>]
    ///<summary>Prompts user to pick one or more object grips from one or more objects.</summary>
    ///<param name="message">(string) Optional, Prompt for picking</param>
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
            let re = Input.RhinoGet.GetGrips(grips,message)
            let grips = !grips
            let rc = ResizeArray()
            if re = Commands.Result.Success && notNull grips then
                for grip in grips do
                    let objectId = grip.OwnerId
                    let index = grip.Index
                    let location = grip.CurrentLocation
                    rc.Add((objectId, index, location))
                    if select then grip.Select(true, true)|>ignore
                if select then Doc.Views.Redraw()
            return rc
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously



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







    [<EXT>]
    ///<summary>Returns number of grips owned by an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int) number of grips</returns>
    static member ObjectGripCount(objectId:Guid) : int =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let grips = rhobj.GetGrips()
        if isNull grips then failwithf "Rhino.Scripting: ObjectGripCount failed.  objectId:'%A'" objectId
        grips.Length


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

    ///<summary>Modifies the location of an object's grip</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to either query or modify</param>
    ///<param name="point">(Point3d) 3D point defining new location of the grip</param>
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



    ///<summary>Modifies the location of all grips owned by an object. The
    /// locations of the grips are returned in a list of Point3d with each position
    /// in the list corresponding to that grip's index. To modify the locations of
    /// the grips, you must provide a list of points that contain the same number
    /// of points at grips</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="points">(Point3d seq) List of 3D points identifying the new grip locations</param>
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



    [<EXT>]
    ///<summary>Verifies that an object's grips are turned on</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(bool) True or False indicating Grips state</returns>
    static member ObjectGripsOn(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.GripsOn


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


