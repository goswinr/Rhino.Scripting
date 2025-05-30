﻿namespace Rhino.Scripting

open System
open Rhino.Scripting.RhinoScriptingUtils

[<AutoOpen>]
module AutoOpenGroup =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    /// <summary>Adds a new empty group to the document.</summary>
    /// <param name="groupName">(string) Optional. Name of the new group. If omitted, Rhino automatically generates the group name.</param>
    /// <returns>(string) Name of the new group.</returns>
    static member AddGroup([<OPT;DEF(null:string)>]groupName:string) : string =
        let mutable index = -1
        if isNull groupName then
            index <- State.Doc.Groups.Add()
        else
            index <- State.Doc.Groups.Add( groupName )
        let rc = State.Doc.Groups.GroupName(index)
        if rc|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.AddGroup failed.  groupName:'%A'" groupName
        rc

    /// <summary>Adds one or more objects to an existing group.</summary>
    /// <param name="objectIds">(Guid seq) List of Guids representing the object identifiers.</param>
    /// <param name="groupName">(string) The name of an existing group.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member AddObjectsToGroup(objectIds:Guid seq, groupName:string) : unit = //PLURAL
        let index = State.Doc.Groups.Find(groupName)
        if index < 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.AddObjectsToGroup Can't add objects to group, group '%s' not found" groupName
        if Seq.isEmpty objectIds then RhinoScriptingException.Raise "RhinoScriptSyntax.AddObjectsToGroup Can't add empty seq to group %s" groupName
        if not <|  State.Doc.Groups.AddToGroup(index, objectIds) then RhinoScriptingException.Raise "RhinoScriptSyntax.AddObjectsToGroup failed '%s' and %A" groupName objectIds


    /// <summary>Adds two or more objects to a new group.</summary>
    /// <param name="objectIds">(Guid seq) List of Guids representing the object identifiers.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member GroupObjects(objectIds:Guid seq) : unit =
        let index = State.Doc.Groups.Add()
        if objectIds |> Seq.hasMaximumItems 1 then RhinoScriptingException.Raise "RhinoScriptSyntax.GroupObjects needs to have more than one objects but has %d" (Seq.length objectIds)
        if not <|  State.Doc.Groups.AddToGroup(index, objectIds) then RhinoScriptingException.Raise "RhinoScriptSyntax.GroupObjects failed on %A"  objectIds
        //State.Doc.Groups.GroupName(index)

    /// <summary>Adds two or more objects to a new group, sets group name.</summary>
    /// <param name="objectIds">(Guid seq) List of Guids representing the object identifiers.</param>
    /// <param name="groupName">(string) The name of the group to create.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member GroupObjects(objectIds:Guid seq, groupName:string) : unit =
        let index = State.Doc.Groups.Add( groupName )
        if index < 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.GroupObjects failed to create group with name '%s' for %d objects" groupName (Seq.length objectIds)
        if objectIds |> Seq.hasMaximumItems 1 then RhinoScriptingException.Raise "RhinoScriptSyntax.GroupObjects to '%s' needs to have more than one objects but has %d" groupName (Seq.length objectIds)
        if not <|  State.Doc.Groups.AddToGroup(index, objectIds) then RhinoScriptingException.Raise "RhinoScriptSyntax.GroupObjects failed on %A"  objectIds
        //State.Doc.Groups.GroupName(index)

    /// <summary>Adds a single object to an existing group.</summary>
    /// <param name="objectId">(Guid) Guid representing the object identifier.</param>
    /// <param name="groupName">(string) The name of an existing group.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member AddObjectToGroup(objectId:Guid, groupName:string) : unit =
        let index = State.Doc.Groups.Find(groupName)
        if index < 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.AddObjectToGroup Can't add object to group, group '%s' not found" groupName
        if not <|  State.Doc.Groups.AddToGroup(index, objectId) then RhinoScriptingException.Raise "RhinoScriptSyntax.AddObjectToGroup failed '%s' and %A" groupName objectId


    /// <summary>Removes an existing group from the document. Reference groups cannot be removed. Deleting a group does not delete the member objects.</summary>
    /// <param name="groupName">(string) The name of an existing group.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DeleteGroup(groupName:string) : unit =
        let index = State.Doc.Groups.Find(groupName)
        if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteGroup Can't DeleteGroup, group '%s' not found" groupName
        if not <| State.Doc.Groups.Delete(index) then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteGroup failed for group '%s' " groupName


    /// <summary>Returns the number of groups in the document.</summary>
    /// <returns>(int) The number of groups in the document.</returns>
    static member GroupCount() : int =
        State.Doc.Groups.Count


    /// <summary>Returns the names of all the groups in the document
    ///    None if no names exist in the document.</summary>
    /// <returns>(string array) The names of all the groups in the document. None if no names exist in the document.</returns>
    static member GroupNames() : string array =
        let names = State.Doc.Groups.GroupNames(true)
        if names|> isNull  then [| |]
        else names


    /// <summary>Hides a group of objects. Hidden objects are not visible, cannot be
    ///    snapped to, and cannot be selected.</summary>
    /// <param name="groupName">(string) The name of an existing group</param>
    /// <returns>(int) The number of objects that were hidden.</returns>
    static member HideGroup(groupName:string) : int =
        let index = State.Doc.Groups.Find(groupName)
        if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.HideGroup: Can't HideGroup, group '%s' not found" groupName
        State.Doc.Groups.Hide(index);


    /// <summary>Verifies the existence of a group.</summary>
    /// <param name="groupName">(string) The name of the group to check for</param>
    /// <returns>(bool) True or False.</returns>
    static member IsGroup(groupName:string) : bool =
        State.Doc.Groups.Find(groupName)>=0


    /// <summary>Checks if an existing group is empty, or contains no object members.</summary>
    /// <param name="groupName">(string) The name of an existing group</param>
    /// <returns>(bool) True or False if groupName is empty.</returns>
    static member IsGroupEmpty(groupName:string) : bool =
        let index = State.Doc.Groups.Find(groupName)
        if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.IsGroupEmpty: Can't check IsGroupEmpty, group '%s' not found" groupName
        State.Doc.Groups.GroupObjectCount(index)>0


    /// <summary>Locks a group of objects. Locked objects are visible and they can be
    ///    snapped to. But, they cannot be selected.</summary>
    /// <param name="groupName">(string) The name of an existing group</param>
    /// <returns>(int) Number of objects that were locked.</returns>
    static member LockGroup(groupName:string) : int =
        let index = State.Doc.Groups.Find(groupName)
        if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.LockGroup failed.  groupName:'%A'" groupName
        State.Doc.Groups.Lock(index);


    /// <summary>Removes a single object from any and all groups that it is a member.
    ///    Neither the object nor the group can be reference objects.</summary>
    /// <param name="objectId">(Guid) The object identifier</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member RemoveObjectFromAllGroups(objectId:Guid) : bool =
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if rhinoObject.GroupCount<1 then false
        else
            let attrs = rhinoObject.Attributes
            attrs.RemoveFromAllGroups()
            State.Doc.Objects.ModifyAttributes(rhinoObject, attrs, true)


    /// <summary>Remove a single object from an existing group.</summary>
    /// <param name="objectId">(Guid) The object identifier</param>
    /// <param name="groupName">(string) The name of an existing group</param>
    /// <returns>(unit) void, nothing.</returns>
    static member RemoveObjectFromGroup(objectId:Guid, groupName:string) : unit =
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = State.Doc.Groups.Find(groupName)
        if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.RemoveObjectFromGroup failed.  objectId:'%s' groupName:'%A'" (Pretty.str objectId) groupName
        let attrs = rhinoObject.Attributes
        attrs.RemoveFromGroup(index)
        if not <| State.Doc.Objects.ModifyAttributes(rhinoObject, attrs, true) then
            RhinoScriptingException.Raise "RhinoScriptSyntax.RemoveObjectFromGroup failed.  objectId:'%s' groupName:'%A'" (Pretty.str objectId) groupName


    /// <summary>Removes multiple objects from an existing group.</summary>
    /// <param name="objectIds">(Guid seq) A list of object identifiers</param>
    /// <param name="groupName">(string) The name of an existing group</param>
    /// <returns>(unit) void, nothing.</returns>
    static member RemoveObjectsFromGroup(objectIds:Guid seq, groupName:string) : unit = //PLURAL
        let index = State.Doc.Groups.Find(groupName)
        if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.RemoveObjectsFromGroup failed.  objectIds:'%A' groupName:'%A'" (Pretty.str objectIds) groupName
        for objectId in objectIds do
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let attrs = rhinoObject.Attributes
            attrs.RemoveFromGroup(index)
            if not <| State.Doc.Objects.ModifyAttributes(rhinoObject, attrs, true) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.RemoveObjectsFromGroup failed.  objectId:'%s' groupName:'%A'" (Pretty.str objectId) groupName



    /// <summary>Renames an existing group.</summary>
    /// <param name="oldName">(string) The name of an existing group</param>
    /// <param name="newName">(string) The new group name</param>
    /// <returns>(unit) void, nothing.</returns>
    static member RenameGroup(oldName:string, newName:string) : unit =
        let index = State.Doc.Groups.Find(oldName)
        if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.RenameGroup failed.  oldName:'%A' newName:'%A'" oldName newName
        if not <| State.Doc.Groups.ChangeGroupName(index, newName) then
            RhinoScriptingException.Raise "RhinoScriptSyntax.RenameGroup failed.  oldName:'%A' newName:'%A'" oldName newName


    /// <summary>Shows a group of previously hidden objects. Hidden objects are not
    ///    visible, cannot be snapped to, and cannot be selected.</summary>
    /// <param name="groupName">(string) The name of an existing group</param>
    /// <returns>(int) The number of objects that were shown.</returns>
    static member ShowGroup(groupName:string) : int =
        let index = State.Doc.Groups.Find(groupName)
        if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.ShowGroup failed.  groupName:'%A'" groupName
        State.Doc.Groups.Show(index)


    /// <summary>Unlocks a group of previously locked objects. Locks objects are visible,
    ///    can be snapped to, but cannot be selected.</summary>
    /// <param name="groupName">(string) The name of an existing group</param>
    /// <returns>(int) The number of objects that were unlocked.</returns>
    static member UnlockGroup(groupName:string) : int =
        let index = State.Doc.Groups.Find(groupName)
        if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.UnlockGroup failed.  groupName:'%A'" groupName
        State.Doc.Groups.Unlock(index)

    /// <summary>Returns the top most group name that an object is assigned.
    ///   This function primarily applies to objects that are members of nested groups.</summary>
    /// <param name="objId">(Guid) id of the object to query </param>
    /// <returns>(int) The top group's name. Fails if object is not in a group.</returns>
    static member ObjectTopGroup(objId:Guid) : string =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objId)
        let groupIndexes = obj.GetGroupList()
        if isNull groupIndexes then  RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectTopGroup objId not part of a group:'%s'" (Pretty.str objId)
        else
            let topGroupIndex = Array.max(groupIndexes) // this is a bad assumption. See RH-49189
            State.Doc.Groups.FindIndex(topGroupIndex).Name

    /// <summary>Returns the names of all groups that an object is part of .
    ///   This function primarily applies to objects that are members of nested groups.</summary>
    /// <param name="objId">(Guid) id of the object to query </param>
    /// <returns>(int) The group's names sorted from bottom to top. Or an empty List if object is not in a group.</returns>
    static member ObjectGroups(objId:Guid) : ResizeArray<string> =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objId)
        let groupIndexes = obj.GetGroupList()
        if isNull groupIndexes then  (new ResizeArray<string>(0))
        else
            let ixs = groupIndexes|>  RArr.mapArr id
            ixs.Sort()
            ixs |>  RArr.map (fun i -> State.Doc.Groups.FindIndex(i).Name)


