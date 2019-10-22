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
        let mutable index = -1
        if groupName|> isNull  then
            index <- Doc.Groups.Add()
        else             
            index <- Doc.Groups.Add( groupName )
        let rc = Doc.Groups.GroupName(index)
        if rc|> isNull  then failwithf "Rhino.Scripting: AddGroup failed.  groupName:'%A'" groupName
        rc
    (*
    def AddGroup(group_name=None):
        '''Adds a new empty group to the document
        Parameters:
          group_name (str, optional): name of the new group. If omitted, rhino automatically
              generates the group name
        Returns:
          str: name of the new group if successful
          None: is not successful or on error
        '''
    
        index = -1
        if group_name is None:
            index = scriptcontext.doc.Groups.Add()
        else:
            if not isinstance(group_name, str): group_name = str(group_name)
            index = scriptcontext.doc.Groups.Add( group_name )
        rc = scriptcontext.doc.Groups.GroupName(index)
        if rc is None: return scriptcontext.errorhandler()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds one or more objects to an existing group.</summary>
    ///<param name="objectIds">(Guid seq) List of Strings or Guids representing the object identifiers</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(unit) void, nothing</returns>
    static member AddObjectsToGroup(objectIds:Guid seq, groupName:string) : unit =
        //if not isinstance(groupName, str) then groupName <- string(groupName)
        let index = Doc.Groups.Find(groupName)
        //objectIds = RhinoScriptSyntax.Coerceguidlist(objectIds)
        if index<0 then failwithf "Can't add objects to group, group '%s' not found" groupName
        if Seq.isEmpty objectIds then failwithf "Can't add empty seq to group %s" groupName
        if not <|  Doc.Groups.AddToGroup(index, objectIds) then failwithf "AddObjectsToGroup failed '%s' and %A" groupName objectIds
        
    (*
    def AddObjectsToGroup(object_ids, group_name):
        '''Adds one or more objects to an existing group.
        Parameters:
          object_ids ([guid, ...]) list of Strings or Guids representing the object identifiers
          group_name (str): the name of an existing group
        Returns:
          number: number of objects added to the group
        '''
    
        if not isinstance(group_name, str): group_name = str(group_name)
        index = scriptcontext.doc.Groups.Find(group_name)
        object_ids = rhutil.coerceguidlist(object_ids)
        if index<0 or not object_ids: return 0
        if not scriptcontext.doc.Groups.AddToGroup(index, object_ids): return 0
        return len(object_ids)
    *)

    [<EXT>]
    ///<summary>Adds a single object to an existing group.</summary>
    ///<param name="objectId">(Guid) String or Guid representing the object identifier</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(unit) void, nothing</returns>
    static member AddObjectToGroup(objectId:Guid, groupName:string) : unit =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //if not isinstance(groupName, str) then groupName <- string(groupName)
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Can't add object to group, group '%s' not found" groupName
        if not <|  Doc.Groups.AddToGroup(index, objectId) then failwithf "AddObjectToGroup failed '%s' and %A" groupName objectId
        
    (*
    def AddObjectToGroup(object_id, group_name):
        '''Adds a single object to an existing group.
        Parameters:
          object_id (guid): String or Guid representing the object identifier
          group_name (str): the name of an existing group
        Returns:
          bool: True or False representing success or failure
        '''
    
        object_id = rhutil.coerceguid(object_id)
        if not isinstance(group_name, str): group_name = str(group_name)
        index = scriptcontext.doc.Groups.Find(group_name)
        if object_id is None or index<0: return False
        return scriptcontext.doc.Groups.AddToGroup(index, object_id)
    *)


    [<EXT>]
    ///<summary>Removes an existing group from the document. Reference groups cannot be
    ///  removed. Deleting a group does not delete the member objects</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(bool) True or False representing success or failure</returns>
    static member DeleteGroup(groupName:string) : bool =
        //if not isinstance(groupName, str) then groupName <- string(groupName)
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Can't DeleteGroup, group '%s' not found" groupName
        Doc.Groups.Delete(index)
    (*
    def DeleteGroup(group_name):
        '''Removes an existing group from the document. Reference groups cannot be
        removed. Deleting a group does not delete the member objects
        Parameters:
          group_name (str): the name of an existing group
        Returns:
          bool: True or False representing success or failure
        '''
    
        if not isinstance(group_name, str): group_name = str(group_name)
        index = scriptcontext.doc.Groups.Find(group_name)
        return scriptcontext.doc.Groups.Delete(index)
    *)


    [<EXT>]
    ///<summary>Returns the number of groups in the document</summary>
    ///<returns>(int) the number of groups in the document</returns>
    static member GroupCount() : int =
        Doc.Groups.Count
    (*
    def GroupCount():
        '''Returns the number of groups in the document
        Returns:
          number: the number of groups in the document
        '''
    
        return scriptcontext.doc.Groups.Count
    *)


    [<EXT>]
    ///<summary>Returns the names of all the groups in the document
    ///  None if no names exist in the document</summary>
    ///<returns>(string array) the names of all the groups in the document.  None if no names exist in the document</returns>
    static member GroupNames() : string array =
        let names = Doc.Groups.GroupNames(true)
        if names|> isNull  then [| |]
        else names
    (*
    def GroupNames():
        '''Returns the names of all the groups in the document
        None if no names exist in the document
        Returns:
          list(str, ...): the names of all the groups in the document.  None if no names exist in the document
        '''
    
        names = scriptcontext.doc.Groups.GroupNames(True)
        if names is None: return None
        return list(names)
    *)


    [<EXT>]
    ///<summary>Hides a group of objects. Hidden objects are not visible, cannot be
    ///  snapped to, and cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) The number of objects that were hidden</returns>
    static member HideGroup(groupName:string) : int =
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Can't HideGroup, group '%s' not found" groupName
        Doc.Groups.Hide(index);
    (*
    def HideGroup(group_name):
        '''Hides a group of objects. Hidden objects are not visible, cannot be
        snapped to, and cannot be selected
        Parameters:
          group_name (str): the name of an existing group
        Returns:
          number: The number of objects that were hidden
        '''
    
        index = scriptcontext.doc.Groups.Find(group_name)
        if index<0: return 0
        return scriptcontext.doc.Groups.Hide(index);
    *)


    [<EXT>]
    ///<summary>Verifies the existance of a group</summary>
    ///<param name="groupName">(string) The name of the group to check for</param>
    ///<returns>(bool) True or False</returns>
    static member IsGroup(groupName:string) : bool =
        //if not isinstance(groupName, str) then groupName <- string(groupName)
        Doc.Groups.Find(groupName)>=0
    (*
    def IsGroup(group_name):
        '''Verifies the existance of a group
        Parameters:
          group_name (str): the name of the group to check for
        Returns:
          bool: True or False
        '''
    
        if not isinstance(group_name, str): group_name = str(group_name)
        return scriptcontext.doc.Groups.Find(group_name)>=0
    *)


    [<EXT>]
    //(FIXME) VarOutTypes
    ///<summary>Verifies that an existing group is empty, or contains no object members</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(bool) True or False if group_name exists</returns>
    static member IsGroupEmpty(groupName:string) : bool =
        //if not isinstance(groupName, str) then groupName <- string(groupName)
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Can't check IsGroupEmpty, group '%s' not found" groupName
        Doc.Groups.GroupObjectCount(index)>0
    (*
    def IsGroupEmpty(group_name):
        '''Verifies that an existing group is empty, or contains no object members
        Parameters:
          group_name (str): the name of an existing group
        Returns:
          bool: True or False if group_name exists
          None: if group_name does not exist
        '''
    
        if not isinstance(group_name, str): group_name = str(group_name)
        index = scriptcontext.doc.Groups.Find(group_name)
        if index<0: return scriptcontext.errorhandler()
        return scriptcontext.doc.Groups.GroupObjectCount(index)>0
    *)


    [<EXT>]
    ///<summary>Locks a group of objects. Locked objects are visible and they can be
    ///  snapped to. But, they cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) Number of objects that were locked</returns>
    static member LockGroup(groupName:string) : int =
        //if not isinstance(groupName, str) then groupName <- string(groupName)
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: LockGroup failed.  groupName:'%A'" groupName
        Doc.Groups.Lock(index);
    (*
    def LockGroup(group_name):
        '''Locks a group of objects. Locked objects are visible and they can be
        snapped to. But, they cannot be selected
        Parameters:
          group_name (str): the name of an existing group
        Returns:
          number: Number of objects that were locked if successful
          None: on error
        '''
    
        if not isinstance(group_name, str): group_name = str(group_name)
        index = scriptcontext.doc.Groups.Find(group_name)
        if index<0: return scriptcontext.errorhandler()
        return scriptcontext.doc.Groups.Lock(index);
    *)


    [<EXT>]
    ///<summary>Removes a single object from any and all groups that it is a member.
    ///  Neither the object nor the group can be reference objects</summary>
    ///<param name="objectId">(Guid) The object identifier</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RemoveObjectFromAllGroups(objectId:Guid) : bool =
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if rhinoobject.GroupCount<1 then false
        else 
            let attrs = rhinoobject.Attributes
            attrs.RemoveFromAllGroups()
            Doc.Objects.ModifyAttributes(rhinoobject, attrs, true)
    (*
    def RemoveObjectFromAllGroups(object_id):
        '''Removes a single object from any and all groups that it is a member.
        Neither the object nor the group can be reference objects
        Parameters:
          object_id (guid): the object identifier
        Returns:
          bool: True or False indicating success or failure
        '''
    
        rhinoobject = rhutil.coercerhinoobject(object_id, True, True)
        if rhinoobject.GroupCount<1: return False
        attrs = rhinoobject.Attributes
        attrs.RemoveFromAllGroups()
        return scriptcontext.doc.Objects.ModifyAttributes(rhinoobject, attrs, True)
    *)


    [<EXT>]
    ///<summary>Remove a single object from an existing group</summary>
    ///<param name="objectId">(Guid) The object identifier</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RemoveObjectFromGroup(objectId:Guid, groupName:string) : bool =
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: RemoveObjectsFromGroup failed.  objectId:'%A' groupName:'%A'" objectId groupName 
        let attrs = rhinoobject.Attributes
        attrs.RemoveFromGroup(index)
        Doc.Objects.ModifyAttributes(rhinoobject, attrs, true)
        
    (*
    def RemoveObjectFromGroup(object_id, group_name):
        '''Remove a single object from an existing group
        Parameters:
          object_id (guid): the object identifier
          group_name (str): the name of an existing group
        Returns:
          bool: True or False indicating success or failure
        '''
    
        count = RemoveObjectsFromGroup(object_id, group_name)
        return not (count is None or count<1)
    *)


    [<EXT>]
    ///<summary>Removes one or more objects from an existing group</summary>
    ///<param name="objectIds">(Guid seq) A list of object identifiers</param>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) The number of objects removed from the group is successful</returns>
    static member RemoveObjectsFromGroup(objectIds:Guid seq, groupName:string) : int =
        //if not isinstance(groupName, str) then groupName <- string(groupName)
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: RemoveObjectsFromGroup failed.  objectIds:'%A' groupName:'%A'" objectIds groupName       
        let mutable objectsremoved = 0
        for objectId in objectIds do
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let attrs = rhinoobject.Attributes
            attrs.RemoveFromGroup(index)
            if Doc.Objects.ModifyAttributes(rhinoobject, attrs, true) then
                objectsremoved<-objectsremoved+1
        objectsremoved
    (*
    def RemoveObjectsFromGroup(object_ids, group_name):
        '''Removes one or more objects from an existing group
        Parameters:
          object_ids ([guid, ...]): a list of object identifiers
          group_name (str): the name of an existing group
        Returns:
          number: The number of objects removed from the group is successful
          None: on error
        '''
    
        if not isinstance(group_name, str): group_name = str(group_name)
        index = scriptcontext.doc.Groups.Find(group_name)
        if index<0: return scriptcontext.errorhandler()
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        objects_removed = 0
        for id in object_ids:
            rhinoobject = rhutil.coercerhinoobject(id, True, True)
            attrs = rhinoobject.Attributes
            attrs.RemoveFromGroup(index)
            if scriptcontext.doc.Objects.ModifyAttributes(rhinoobject, attrs, True):
                objects_removed+=1
        return objects_removed
    *)


    [<EXT>]
    ///<summary>Renames an existing group</summary>
    ///<param name="oldName">(string) The name of an existing group</param>
    ///<param name="newName">(string) The new group name</param>
    ///<returns>(string) the new group name</returns>
    static member RenameGroup(oldName:string, newName:string) : string =
        //if not isinstance(oldName, str) then oldName <- string(oldName)
        let index = Doc.Groups.Find(oldName)
        if index<0 then failwithf "Rhino.Scripting: RenameGroup failed.  oldName:'%A' newName:'%A'" oldName newName
        //if not isinstance(newName, str) then newName <- string(newName)
        if Doc.Groups.ChangeGroupName(index, newName) then
            newName
        else
            failwithf "Rhino.Scripting: RenameGroup failed.  oldName:'%A' newName:'%A'" oldName newName
    (*
    def RenameGroup(old_name, new_name):
        '''Renames an existing group
        Parameters:
          old_name (str): the name of an existing group
          new_name (str): the new group name
        Returns:
          str: the new group name if successful
          None: on error
        '''
    
        if not isinstance(old_name, str): old_name = str(old_name)
        index = scriptcontext.doc.Groups.Find(old_name)
        if index<0: return scriptcontext.errorhandler()
        if not isinstance(new_name, str): new_name = str(new_name)
        if scriptcontext.doc.Groups.ChangeGroupName(index, new_name):
            return new_name
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Shows a group of previously hidden objects. Hidden objects are not
    ///  visible, cannot be snapped to, and cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) The number of objects that were shown</returns>
    static member ShowGroup(groupName:string) : int =
        //if not isinstance(groupName, str) then groupName <- string(groupName)
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: ShowGroup failed.  groupName:'%A'" groupName
        Doc.Groups.Show(index);
    (*
    def ShowGroup(group_name):
        '''Shows a group of previously hidden objects. Hidden objects are not
        visible, cannot be snapped to, and cannot be selected
        Parameters:
          group_name (str): the name of an existing group
        Returns:
          number: The number of objects that were shown if successful
          None: on error
        '''
    
        if not isinstance(group_name, str): group_name = str(group_name)
        index = scriptcontext.doc.Groups.Find(group_name)
        if index<0: return scriptcontext.errorhandler()
        return scriptcontext.doc.Groups.Show(index);
    *)


    [<EXT>]
    ///<summary>Unlocks a group of previously locked objects. Lockes objects are visible,
    ///  can be snapped to, but cannot be selected</summary>
    ///<param name="groupName">(string) The name of an existing group</param>
    ///<returns>(int) The number of objects that were unlocked</returns>
    static member UnlockGroup(groupName:string) : int =
        //if not isinstance(groupName, str) then groupName <- string(groupName)
        let index = Doc.Groups.Find(groupName)
        if index<0 then failwithf "Rhino.Scripting: UnlockGroup failed.  groupName:'%A'" groupName
        Doc.Groups.Unlock(index);
    (*
    def UnlockGroup(group_name):
        '''Unlocks a group of previously locked objects. Lockes objects are visible,
        can be snapped to, but cannot be selected
        Parameters:
          group_name (str): the name of an existing group
        Returns:
          number: The number of objects that were unlocked if successful
          None: on error
        '''
    
        if not isinstance(group_name, str): group_name = str(group_name)
        index = scriptcontext.doc.Groups.Find(group_name)
        if index<0: return scriptcontext.errorhandler()
        return scriptcontext.doc.Groups.Unlock(index);
    *)


