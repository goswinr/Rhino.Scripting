namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsGroup =
  type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Adds a new empty group to the document</summary>
    ///<param name="groupName">(string) Optional, Default Value: <c>null:string</c>
    ///Name of the new group. If omitted, rhino automatically
    ///  generates the group name</param>
    ///<returns>(string) name of the new group</returns>
    static member AddGroup([<OPT;DEF(null:string)>]groupName:string) : string =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Adds one or more objects to an existing group.</summary>
    ///<param name="objectIds">(Guid seq) List of Strings or Guids representing the object identifiers</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(float) number of objects added to the group</returns>
    static member AddObjectsToGroup(objectIds:Guid seq, groupName:string) : float =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Adds a single object to an existing group.</summary>
    ///<param name="objectId">(Guid) String or Guid representing the object identifier</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(bool) True or False representing success or failure</returns>
    static member AddObjectToGroup(objectId:Guid, groupName:string) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Removes an existing group from the document. Reference groups cannot be
    ///  removed. Deleting a group does not delete the member objects</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(bool) True or False representing success or failure</returns>
    static member DeleteGroup(groupName:string) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the number of groups in the document</summary>
    ///<returns>(int) the number of groups in the document</returns>
    static member GroupCount() : int =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the names of all the groups in the document
    ///  None if no names exist in the document</summary>
    ///<returns>(string seq) the names of all the groups in the document.  None if no names exist in the document</returns>
    static member GroupNames() : string seq =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Hides a group of objects. Hidden objects are not visible, cannot be
    ///  snapped to, and cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) The number of objects that were hidden</returns>
    static member HideGroup(groupName:string) : int =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies the existance of a group</summary>
    ///<param name="groupName">(string) The name of the group to check for</param>
    ///<returns>(bool) True or False</returns>
    static member IsGroup(groupName:string) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    //(FIXME) VarOutTypes
    ///<summary>Verifies that an existing group is empty, or contains no object members</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(bool) True or False if group_name exists</returns>
    static member IsGroupEmpty(groupName:string) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Locks a group of objects. Locked objects are visible and they can be
    ///  snapped to. But, they cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(float) Number of objects that were locked</returns>
    static member LockGroup(groupName:string) : float =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Removes a single object from any and all groups that it is a member.
    ///  Neither the object nor the group can be reference objects</summary>
    ///<param name="objectId">(Guid) The object identifier</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RemoveObjectFromAllGroups(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Remove a single object from an existing group</summary>
    ///<param name="objectId">(Guid) The object identifier</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RemoveObjectFromGroup(objectId:Guid, groupName:string) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Removes one or more objects from an existing group</summary>
    ///<param name="objectIds">(Guid seq) A list of object identifiers</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(float) The number of objects removed from the group is successful</returns>
    static member RemoveObjectsFromGroup(objectIds:Guid seq, groupName:string) : float =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Renames an existing group</summary>
    ///<param name="oldName">(string) The name of an existing group</param>
    ///<param name="newName">(string) The new group name</param>
    ///<returns>(string) the new group name</returns>
    static member RenameGroup(oldName:string, newName:string) : string =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Shows a group of previously hidden objects. Hidden objects are not
    ///  visible, cannot be snapped to, and cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(float) The number of objects that were shown</returns>
    static member ShowGroup(groupName:string) : float =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Unlocks a group of previously locked objects. Lockes objects are visible,
    ///  can be snapped to, but cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(float) The number of objects that were unlocked</returns>
    static member UnlockGroup(groupName:string) : float =
        failNotImpl () // genreation temp disabled !!


