﻿
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings
open Rhino.ScriptingFSharp

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenGrips =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>Enables or disables an object's grips. For Curves and Surfaces, these are
    ///    also called control points.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///    If True, the specified object's grips will be turned on.
    ///    If False, they will be turned off</param>
    ///<returns>(bool) True on success, False on failure.</returns>
    static member EnableObjectGrips(objectId:Guid, [<OPT;DEF(true)>]enable:bool) : bool = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if enable <> rhobj.GripsOn then
            rhobj.GripsOn <- enable
            State.Doc.Views.Redraw()
        enable = rhobj.GripsOn


    ///<summary>Prompts the user to pick a single object grip. Fails if selection is empty.</summary>
    ///<param name="message">(string) Optional, Prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///    Allow for selection of pre-selected object grip</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the picked object grip</param>
    ///<returns>(Guid * int * Point3d) of a grip record.
    ///    [0] = identifier of the object that owns the grip
    ///    [1] = index value of the grip
    ///    [2] = location of the grip.</returns>
    static member GetObjectGrip( [<OPT;DEF(null:string)>]message:string,
                                 [<OPT;DEF(false)>]preselect:bool,
                                 [<OPT;DEF(false)>]select:bool) : Guid * int * Point3d = 
        let get () = 
            if not preselect then
                State.Doc.Objects.UnselectAll() |> ignore
                State.Doc.Views.Redraw()
            let grip = ref null
            let rc = Input.RhinoGet.GetGrip(grip, message)
            let grip = !grip
            if rc <> Commands.Result.Success || isNull grip then
                RhinoScriptingException.Raise "Rhino.Scripting.GetObjectGrip User failed to select a Grip for : %s " message
            else
                if select then
                    grip.Select(true, true)|> ignore
                    State.Doc.Views.Redraw()
                (grip.OwnerId, grip.Index, grip.CurrentLocation)            
        RhinoSync.DoSyncRedrawHideEditor get



    ///<summary>Prompts user to pick one or more object grips from one or more objects.</summary>
    ///<param name="message">(string) Optional, Prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///    Allow for selection of pre-selected object grips</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the picked object grips</param>
    ///<returns>((Guid * int * Point3d) Rarr) containing one or more grip records. Each grip record is a tuple
    ///    [n][0] = identifier of the object that owns the grip
    ///    [n][1] = index value of the grip
    ///    [n][2] = location of the grip.</returns>
    static member GetObjectGrips( [<OPT;DEF(null:string)>]message:string,
                                  [<OPT;DEF(false)>]preselect:bool,
                                  [<OPT;DEF(false)>]select:bool) : Rarr<Guid * int * Point3d> = 
        let get () = 
            if not preselect then
                State.Doc.Objects.UnselectAll() |> ignore
                State.Doc.Views.Redraw()
            let grips = ref null
            let re = Input.RhinoGet.GetGrips(grips, message)
            let grips = !grips
            let rc = Rarr()
            if re = Commands.Result.Success && notNull grips then
                for grip in grips do
                    let objectId = grip.OwnerId
                    let index = grip.Index
                    let location = grip.CurrentLocation
                    rc.Add((objectId, index, location))
                    if select then grip.Select(true, true)|>ignore
                if select then State.Doc.Views.Redraw()            
            rc
        RhinoSync.DoSyncRedrawHideEditor get



    /// Internal helper
    static member private neighborGrip(i, objectId:Guid, index, direction, enable) : Result<DocObjects.GripObject, string> = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        let grips = rhobj.GetGrips()
        if isNull grips then Result.Error "rhobj.GetGrips() is null"
        else
            if grips.Length <= index then Result.Error "rhobj.GetGrips() failed:  grips.Length <= index "
            else
                let grip = grips.[index]
                let ng = 
                    if direction = 0 then
                        grip.NeighborGrip(i, 0, 0, wrap=false)
                    else
                        grip.NeighborGrip(0, i, 0, wrap=false)
                if notNull ng && enable then
                    ng.Select(true) |> ignore //TODO make sync ?
                    State.Doc.Views.Redraw()
                Ok ng


    ///<summary>Returns the next grip index from a specified grip index of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Zero based grip index from which to get the next grip index</param>
    ///<param name="direction">(int ) Optional, Default Value: <c>0</c>
    ///    Direction to get the next grip index (0 = U, 1 = V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///    If True, the next grip index found will be selected</param>
    ///<returns>(int) index of the next grip.</returns>
    static member NextObjectGrip( objectId:Guid,
                                  index:int,
                                  [<OPT;DEF(0)>]direction:int ,
                                  [<OPT;DEF(true)>]enable:bool) : int = 
        match Scripting.neighborGrip(1, objectId, index, direction, enable) with
        |Ok r -> r.Index
        |Error s -> RhinoScriptingException.Raise "Rhino.Scripting.NextObjectGrip failed with %s for index %d, direction %d on %A" s index direction objectId

    ///<summary>Returns number of grips owned by an object.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int) number of grips.</returns>
    static member ObjectGripCount(objectId:Guid) : int = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        let grips = rhobj.GetGrips()
        if isNull grips then RhinoScriptingException.Raise "Rhino.Scripting.ObjectGripCount failed.  objectId:'%s'" (Nice.str objectId)
        grips.Length


    ///<summary>Returns the location of an object's grip.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to either query or modify</param>
    ///<returns>(Point3d) The current location of the grip referenced by index.</returns>
    static member ObjectGripLocation(objectId:Guid, index:int) : Point3d = //GET
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then RhinoScriptingException.Raise "Rhino.Scripting.ObjectGripLocation failed.  objectId:'%s' index:'%A'" (Nice.str objectId) index
        let grips = rhobj.GetGrips()
        if isNull grips || index<0 || index>=grips.Length then
            RhinoScriptingException.Raise "Rhino.Scripting.ObjectGripLocation failed.  objectId:'%s' index:'%A'" (Nice.str objectId) index
        let grip = grips.[index]
        let rc = grip.CurrentLocation
        rc

    ///<summary>Modifies the location of an object's grip.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to either query or modify</param>
    ///<param name="point">(Point3d) 3D point defining new location of the grip</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectGripLocation(objectId:Guid, index:int, point:Point3d) : unit = //SET
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then RhinoScriptingException.Raise "Rhino.Scripting.ObjectGripLocation failed.  objectId:'%s' index:'%A' point:'%A'" (Nice.str objectId) index point
        let grips = rhobj.GetGrips()
        if isNull grips || index<0 || index>=grips.Length then
            RhinoScriptingException.Raise "Rhino.Scripting.ObjectGripLocation failed.  objectId:'%s' index:'%A' point:'%A'" (Nice.str objectId) index point
        let grip = grips.[index]
        let rc = grip.CurrentLocation
        grip.CurrentLocation <-  point
        State.Doc.Objects.GripUpdate(rhobj, true)|> ignoreObj
        State.Doc.Views.Redraw()



    ///<summary>Returns the location of all grips owned by an object. The
    /// locations of the grips are returned in a list of Point3d with each position
    /// in the list corresponding to that grip's index. To modify the locations of
    /// the grips, you must provide a list of points that contain the same number
    /// of points at grips.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(Point3d Rarr) The current location of all grips.</returns>
    static member ObjectGripLocations(objectId:Guid) : Point3d Rarr = //GET
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then RhinoScriptingException.Raise "Rhino.Scripting.ObjectGripLocations failed.  objectId:'%s'" (Nice.str objectId)
        let grips = rhobj.GetGrips()
        if isNull grips then RhinoScriptingException.Raise "Rhino.Scripting.ObjectGripLocations failed.  objectId:'%s'" (Nice.str objectId)
        rarr { for grip in grips do yield grip.CurrentLocation }



    ///<summary>Modifies the location of all grips owned by an object. The
    /// locations of the grips are returned in a list of Point3d with each position
    /// in the list corresponding to that grip's index. To modify the locations of
    /// the grips, you must provide a list of points that contain the same number
    /// of points at grips.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="points">(Point3d seq) List of 3D points identifying the new grip locations</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectGripLocations(objectId:Guid, points:Point3d seq) : unit = //SET
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then RhinoScriptingException.Raise "Rhino.Scripting.ObjectGripLocations failed.  objectId:'%s' points:'%A'" (Nice.str objectId) points
        let grips = rhobj.GetGrips()
        if grips |> isNull then RhinoScriptingException.Raise "Rhino.Scripting.ObjectGripLocations failed.  objectId:'%s' points:'%A'" (Nice.str objectId) points
        if Seq.length(points) = Seq.length(grips) then
            for pt, grip in Seq.zip points grips do
                grip.CurrentLocation <- pt
            State.Doc.Objects.GripUpdate(rhobj, true)|> ignoreObj
            State.Doc.Views.Redraw()



    ///<summary>Checks if an object's grips are turned on.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(bool) True or False indicating Grips state.</returns>
    static member ObjectGripsOn(objectId:Guid) : bool = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        rhobj.GripsOn


    ///<summary>Checks if an object's grips are turned on and at least one grip
    ///    is selected.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member ObjectGripsSelected(objectId:Guid) : bool = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then false
        else
            let grips = rhobj.GetGrips()
            if isNull grips then false
            else
                grips
                |> Seq.exists (fun g -> g.IsSelected(false) > 0)



    ///<summary>Returns the previous grip index from a specified grip index of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Zero based grip index from which to get the previous grip index</param>
    ///<param name="direction">(int) Optional, Default Value: <c>0</c>
    ///    Direction to get the next grip index (0 = U, 1 = V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///    If True, the next grip index found will be selected</param>
    ///<returns>(int) index of the next grip.</returns>
    static member PrevObjectGrip( objectId:Guid,
                                  index:int,
                                  [<OPT;DEF(0)>]direction:int,
                                  [<OPT;DEF(true)>]enable:bool) : int = 
        match Scripting.neighborGrip(-1, objectId, index, direction, enable) with
        |Ok r -> r.Index
        |Error s -> RhinoScriptingException.Raise "Rhino.Scripting.PrevObjectGrip failed with %s for index %d, direction %d on %A" s index direction objectId


    ///<summary>Returns a list of grip indices identifying an object's selected grips.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int Rarr) list of indices.</returns>
    static member SelectedObjectGrips(objectId:Guid) : int Rarr = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        let rc = Rarr()
        if not rhobj.GripsOn then rc
        else
            let grips = rhobj.GetGrips()

            if notNull grips then
                for i = 0 to grips.Length - 1 do
                    if grips.[i].IsSelected(false) > 0 then rc.Add(i)
            rc


    ///<summary>Selects a single grip owned by an object. If the object's grips are
    ///    not turned on, the grips will not be selected.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to select</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member SelectObjectGrip(objectId:Guid, index:int) : bool = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then false
        else
            let grips = rhobj.GetGrips()
            if isNull grips then false
            else
                if index<0 || index>=grips.Length then false
                else
                    let grip = grips.[index]
                    if grip.Select(true, true) >0 then
                        State.Doc.Views.Redraw()
                        true
                    else
                        false


    ///<summary>Selects an object's grips. If the object's grips are not turned on,
    ///    they will not be selected.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int) Number of grips selected.</returns>
    static member SelectObjectGrips(objectId:Guid) : int = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then RhinoScriptingException.Raise "Rhino.Scripting.SelectObjectGrips failed.  objectId:'%s'" (Nice.str objectId)
        let grips = rhobj.GetGrips()
        if isNull grips then RhinoScriptingException.Raise "Rhino.Scripting.SelectObjectGrips failed.  objectId:'%s'" (Nice.str objectId)
        let mutable count = 0
        for grip in grips do
            if grip.Select(true, true)>0 then count<- count +  1
        if count>0 then
            State.Doc.Views.Redraw()
            count
        else
            RhinoScriptingException.Raise "Rhino.Scripting.SelectObjectGrips failed.  objectId:'%s'" (Nice.str objectId)


    ///<summary>Unselects a single grip owned by an object. If the object's grips are
    ///    not turned on, the grips will not be unselected.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to unselect</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member UnselectObjectGrip(objectId:Guid, index:int) : bool = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then false
        else
            let grips = rhobj.GetGrips()
            if isNull grips then false
            else
                if index<0 || index>=grips.Length then false
                else
                    let grip = grips.[index]
                    if grip.Select(false) = 0 then
                        State.Doc.Views.Redraw()
                        true
                    else
                        false


    ///<summary>Unselects an object's grips. Note, the grips will not be turned off.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int) Number of grips unselected.</returns>
    static member UnselectObjectGrips(objectId:Guid) : int = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        if not rhobj.GripsOn then RhinoScriptingException.Raise "Rhino.Scripting.UnselectObjectGrips failed.  objectId:'%s'" (Nice.str objectId)
        let grips = rhobj.GetGrips()
        if isNull grips then RhinoScriptingException.Raise "Rhino.Scripting.UnselectObjectGrips failed.  objectId:'%s'" (Nice.str objectId)
        let mutable count = 0
        for grip in grips do
            if grip.Select(false) = 0 then count <- count +   1
        if count>0 then
            State.Doc.Views.Redraw()
            count
        else
            RhinoScriptingException.Raise "Rhino.Scripting.UnselectObjectGrips failed.  objectId:'%s'" (Nice.str objectId)



