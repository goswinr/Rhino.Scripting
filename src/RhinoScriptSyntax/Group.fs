namespace Rhino.Scripting

open System
open Rhino

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

 
[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsGroup =
  
  
  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with


    [<Extension>]
    ///<summary>Adds a new empty group to the document</summary>
    ///<param name="groupName">(string) Optional, Name of the new group. If omitted, rhino automatically
    ///    generates the group name</param>
    ///<returns>(string) name of the new group</returns>
    static member AddGroup([<OPT;DEF(null:string)>]groupName:string) : string =
        let mutable index = -1
        if groupName|> isNull  then
            index <- Doc.Groups.Add()
        else
            index <- Doc.Groups.Add( groupName )
        let rc = Doc.Groups.GroupName(index)
        if rc|> isNull  then failwithf "Rhino.Scripting: AddGroup failed.  groupName:'%A'" groupName
        rc


    [<Extension>]
    ///<summary>Adds one or more objects to an existing group</summary>
    ///<param name="objectIds">(Guid seq) List of Strings or Guids representing the object identifiers</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(unit) void, nothing</returns>
    static member AddObjectToGroup(objectIds:Guid seq, groupName:string) : unit = //PLURAL
        let index = Doc.Groups.Find(groupName)
        if index < 0 then failwithf "Can't add objects to group, group '%s' not found" groupName
        if Seq.isEmpty objectIds then failwithf "Can't add empty seq to group %s" groupName
        if not <|  Doc.Groups.AddToGroup(index, objectIds) then failwithf "AddObjectsToGroup failed '%s' and %A" groupName objectIds


    [<Extension>]
    ///<summary>Adds two or more objects to new group</summary>
    ///<param name="objectIds">(Guid seq) List of Strings or Guids representing the object identifiers</param>
    ///<returns>(unit) void, nothing</returns>
    static member GroupObjects(objectIds:Guid seq) : unit = 
        let index = Doc.Groups.Add()
        if Seq.length objectIds < 2 then failwithf "GroupObjects needes to have more than one objects"
        if not <|  Doc.Groups.AddToGroup(index, objectIds) then failwithf "GroupObjects failed on %A"  objectIds
        //Doc.Groups.GroupName(index)


    [<Extension>]
    ///<summary>Adds a single object to an existing group</summary>
    ///<param name="objectId">(Guid) String or Guid representing the object identifier</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(unit) void, nothing</returns>
    static member AddObjectToGroup(objectId:Guid, groupName:string) : unit =
        let index = Doc.Groups.Find(groupName)
        if index < 0 then failwithf "Can't add object to group, group '%s' not found" groupName
        if not <|  Doc.Groups.AddToGroup(index, objectId) then failwithf "AddObjectToGroup failed '%s' and %A" groupName objectId



    [<Extension>]
    ///<summary>Removes an existing group from the document. Reference groups cannot be
    ///    removed. Deleting a group does not delete the member objects</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteGroup(groupName:string) : unit =
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Can't DeleteGroup, group '%s' not found" groupName
        if not <| Doc.Groups.Delete(index) then failwithf "DeleteGroup failed for group '%s' " groupName


    [<Extension>]
    ///<summary>Returns the number of groups in the document</summary>
    ///<returns>(int) the number of groups in the document</returns>
    static member GroupCount() : int =
        Doc.Groups.Count


    [<Extension>]
    ///<summary>Returns the names of all the groups in the document
    ///    None if no names exist in the document</summary>
    ///<returns>(string array) the names of all the groups in the document.  None if no names exist in the document</returns>
    static member GroupNames() : string array =
        let names = Doc.Groups.GroupNames(true)
        if names|> isNull  then [| |]
        else names


    [<Extension>]
    ///<summary>Hides a group of objects. Hidden objects are not visible, cannot be
    ///    snapped to, and cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) The number of objects that were hidden</returns>
    static member HideGroup(groupName:string) : int =
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Can't HideGroup, group '%s' not found" groupName
        Doc.Groups.Hide(index);


    [<Extension>]
    ///<summary>Verifies the existance of a group</summary>
    ///<param name="groupName">(string) The name of the group to check for</param>
    ///<returns>(bool) True or False</returns>
    static member IsGroup(groupName:string) : bool =
        Doc.Groups.Find(groupName)>=0


    [<Extension>]
    ///<summary>Verifies that an existing group is empty, or contains no object members</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(bool) True or False if groupName is empty</returns>
    static member IsGroupEmpty(groupName:string) : bool =
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Can't check IsGroupEmpty, group '%s' not found" groupName
        Doc.Groups.GroupObjectCount(index)>0


    [<Extension>]
    ///<summary>Locks a group of objects. Locked objects are visible and they can be
    ///    snapped to. But, they cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) Number of objects that were locked</returns>
    static member LockGroup(groupName:string) : int =
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: LockGroup failed.  groupName:'%A'" groupName
        Doc.Groups.Lock(index);


    [<Extension>]
    ///<summary>Removes a single object from any and all groups that it is a member.
    ///    Neither the object nor the group can be reference objects</summary>
    ///<param name="objectId">(Guid) The object identifier</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RemoveObjectFromAllGroups(objectId:Guid) : bool =
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if rhinoobject.GroupCount<1 then false
        else
            let attrs = rhinoobject.Attributes
            attrs.RemoveFromAllGroups()
            Doc.Objects.ModifyAttributes(rhinoobject, attrs, true)


    [<Extension>]
    ///<summary>Remove a single object from an existing group</summary>
    ///<param name="objectId">(Guid) The object identifier</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(unit) void, nothing</returns>
    static member RemoveObjectFromGroup(objectId:Guid, groupName:string) : unit =
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: RemoveObjectsFromGroup failed.  objectId:'%A' groupName:'%A'" objectId groupName
        let attrs = rhinoobject.Attributes
        attrs.RemoveFromGroup(index)
        if not <| Doc.Objects.ModifyAttributes(rhinoobject, attrs, true) then
            failwithf "Rhino.Scripting: RemoveObjectsFromGroup failed.  objectId:'%A' groupName:'%A'" objectId groupName


    [<Extension>]
    ///<summary>Removes multiple objects from an existing group</summary>
    ///<param name="objectIds">(Guid seq) A list of object identifiers</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(unit) void, nothing</returns>
    static member RemoveObjectFromGroup(objectIds:Guid seq, groupName:string) : unit = //PLURAL
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: RemoveObjectsFromGroup failed.  objectIds:'%A' groupName:'%A'" objectIds groupName        
        for objectId in objectIds do
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let attrs = rhinoobject.Attributes
            attrs.RemoveFromGroup(index)
            if not <| Doc.Objects.ModifyAttributes(rhinoobject, attrs, true) then
                failwithf "Rhino.Scripting: RemoveObjectsFromGroup failed.  objectId:'%A' groupName:'%A'" objectId groupName
        


    [<Extension>]
    ///<summary>Renames an existing group</summary>
    ///<param name="oldName">(string) The name of an existing group</param>
    ///<param name="newName">(string) The new group name</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenameGroup(oldName:string, newName:string) : unit =
        let index = Doc.Groups.Find(oldName)
        if index<0 then failwithf "Rhino.Scripting: RenameGroup failed.  oldName:'%A' newName:'%A'" oldName newName
        if not <| Doc.Groups.ChangeGroupName(index, newName) then
            failwithf "Rhino.Scripting: RenameGroup failed.  oldName:'%A' newName:'%A'" oldName newName


    [<Extension>]
    ///<summary>Shows a group of previously hidden objects. Hidden objects are not
    ///    visible, cannot be snapped to, and cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) The number of objects that were shown</returns>
    static member ShowGroup(groupName:string) : int =
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: ShowGroup failed.  groupName:'%A'" groupName
        Doc.Groups.Show(index)


    [<Extension>]
    ///<summary>Unlocks a group of previously locked objects. Lockes objects are visible,
    ///    can be snapped to, but cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) The number of objects that were unlocked</returns>
    static member UnlockGroup(groupName:string) : int =
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: UnlockGroup failed.  groupName:'%A'" groupName
        Doc.Groups.Unlock(index)


