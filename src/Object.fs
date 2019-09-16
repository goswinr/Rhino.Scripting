namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
[<AutoOpen>]
module ExtensionsObject =
  type RhinoScriptSyntax with
    
    ///<summary>Copies object from one location to another, or in-place.</summary>
    ///<param name="objectId">(Guid) Object to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value: <c>null</c>
    ///Translation vector to apply</param>
    ///<returns>(Guid) id for the copy</returns>
    static member CopyObject(objectId:Guid, [<OPT;DEF(null)>]translation:Vector3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def CopyObject(object_id, translation=None):
        '''Copies object from one location to another, or in-place.
        Parameters:
          object_id (guid): object to copy
          translation (vector, optional): translation vector to apply
        Returns:
          guid: id for the copy if successful
          None: if not able to copy
        '''
        rc = CopyObjects(object_id, translation)
        if rc: return rc[0]
    *)


    ///<summary>Copies one or more objects from one location to another, or in-place.</summary>
    ///<param name="objectIds">(Guid seq) List of objects to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value: <c>null</c>
    ///List of three numbers or Vector3d representing
    ///  translation vector to apply to copied set</param>
    ///<returns>(Guid seq) identifiers for the copies</returns>
    static member CopyObjects(objectIds:Guid seq, [<OPT;DEF(null)>]translation:Vector3d) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def CopyObjects(object_ids, translation=None):
        '''Copies one or more objects from one location to another, or in-place.
        Parameters:
          object_ids ([guid, ...])list of objects to copy
          translation (vector, optional): list of three numbers or Vector3d representing
                             translation vector to apply to copied set
        Returns:
          list(guid, ...): identifiers for the copies if successful
        '''
        if translation:
            translation = rhutil.coerce3dvector(translation, True)
            translation = Rhino.Geometry.Transform.Translation(translation)
        else:
            translation = Rhino.Geometry.Transform.Identity
        return TransformObjects(object_ids, translation, True)
    *)


    ///<summary>Deletes a single object from the document</summary>
    ///<param name="objectId">(Guid) Identifier of object to delete</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member DeleteObject(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def DeleteObject(object_id):
        '''Deletes a single object from the document
        Parameters:
          object_id (guid): identifier of object to delete
        Returns:
          bool: True of False indicating success or failure
        '''
        object_id = rhutil.coerceguid(object_id, True)
        rc = scriptcontext.doc.Objects.Delete(object_id, True)
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Deletes one or more objects from the document</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to delete</param>
    ///<returns>(float) Number of objects deleted</returns>
    static member DeleteObjects(objectIds:Guid seq) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def DeleteObjects(object_ids):
        '''Deletes one or more objects from the document
        Parameters:
          object_ids ([guid, ...]): identifiers of objects to delete
        Returns:
          number: Number of objects deleted
        '''
        rc = 0
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        for id in object_ids:
            id = rhutil.coerceguid(id, True)
            if scriptcontext.doc.Objects.Delete(id, True): rc+=1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Causes the selection state of one or more objects to change momentarily
    ///  so the object appears to flash on the screen</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to flash</param>
    ///<param name="style">(bool) Optional, Default Value: <c>true</c>
    ///If True, flash between object color and selection color.
    ///  If False, flash between visible and invisible</param>
    ///<returns>(unit) </returns>
    static member FlashObject(objectIds:Guid seq, [<OPT;DEF(true)>]style:bool) : unit =
        failNotImpl () // genreation temp disabled !!
    (*
    def FlashObject(object_ids, style=True):
        '''Causes the selection state of one or more objects to change momentarily
        so the object appears to flash on the screen
        Parameters:
          object_ids ([guid, ...]) identifiers of objects to flash
          style (bool, optional): If True, flash between object color and selection color.
            If False, flash between visible and invisible
        Returns:
          None
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        rhobjs = [rhutil.coercerhinoobject(id, True, True) for id in object_ids]
        if rhobjs: scriptcontext.doc.Views.FlashObjects(rhobjs, style)
    *)


    ///<summary>Hides a single object</summary>
    ///<param name="objectId">(Guid) Id of object to hide</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member HideObject(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def HideObject(object_id):
        '''Hides a single object
        Parameters:
          object_id (guid): id of object to hide
        Returns:
          bool: True of False indicating success or failure
        '''
        return HideObjects(object_id)==1
    *)


    ///<summary>Hides one or more objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to hide</param>
    ///<returns>(int) Number of objects hidden</returns>
    static member HideObjects(objectIds:Guid seq) : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def HideObjects(object_ids):
        '''Hides one or more objects
        Parameters:
          object_ids ([guid, ...]): identifiers of objects to hide
        Returns:
          number: Number of objects hidden
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        rc = 0
        for id in object_ids:
            id = rhutil.coerceguid(id, True)
            if scriptcontext.doc.Objects.Hide(id, False): rc += 1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Verifies that an object is in either page layout space or model space</summary>
    ///<param name="objectId">(Guid) Id of an object to test</param>
    ///<returns>(bool) True if the object is in page layout space, False if the object is in model space</returns>
    static member IsLayoutObject(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayoutObject(object_id):
        '''Verifies that an object is in either page layout space or model space
        Parameters:
          object_id (guid): id of an object to test
        Returns:
          bool: True if the object is in page layout space, False if the object is in model space
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.Attributes.Space == Rhino.DocObjects.ActiveSpace.PageSpace
    *)


    ///<summary>Verifies the existence of an object</summary>
    ///<param name="objectId">(Guid) An object to test</param>
    ///<returns>(bool) True if the object exists, False if the object does not exist</returns>
    static member IsObject(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObject(object_id):
        '''Verifies the existence of an object
        Parameters:
          object_id (guid): an object to test
        Returns:
          bool: True if the object exists, False if the object does not exist
        '''
        return rhutil.coercerhinoobject(object_id, True, False) is not None
    *)


    ///<summary>Verifies that an object is hidden. Hidden objects are not visible, cannot
    ///  be snapped to, and cannot be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is hidden, False if the object is not hidden</returns>
    static member IsObjectHidden(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectHidden(object_id):
        '''Verifies that an object is hidden. Hidden objects are not visible, cannot
        be snapped to, and cannot be selected
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True if the object is hidden, False if the object is not hidden
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.IsHidden
    *)


    ///<summary>Verifies an object's bounding box is inside of another bounding box</summary>
    ///<param name="objectId">(Guid) Identifier of an object to be tested</param>
    ///<param name="box">(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) Bounding box to test for containment</param>
    ///<param name="testMode">(bool) Optional, Default Value: <c>true</c>
    ///If True, the object's bounding box must be contained by box
    ///  If False, the object's bounding box must be contained by or intersect box</param>
    ///<returns>(bool) True if object is inside box, False is object is not inside box</returns>
    static member IsObjectInBox(objectId:Guid, box:Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d, [<OPT;DEF(true)>]testMode:bool) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectInBox(object_id, box, test_mode=True):
        '''Verifies an object's bounding box is inside of another bounding box
        Parameters:
          object_id (guid): identifier of an object to be tested
          box ([point, point, point, point, point, point, point, point]): bounding box to test for containment
          test_mode (bool, optional): If True, the object's bounding box must be contained by box
            If False, the object's bounding box must be contained by or intersect box
        Returns:
          bool: True if object is inside box, False is object is not inside box
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        box = rhutil.coerceboundingbox(box, True)
        objbox = rhobj.Geometry.GetBoundingBox(True)
        if test_mode: return box.Contains(objbox)
        union = Rhino.Geometry.BoundingBox.Intersection(box, objbox)
        return union.IsValid
    *)


    //(FIXME) VarOutTypes
    ///<summary>Verifies that an object is a member of a group</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="groupName">(string) Optional, Default Value: <c>null</c>
    ///The name of a group. If omitted, the function
    ///  verifies that the object is a member of any group</param>
    ///<returns>(bool) True if the object is a member of the specified group. If a group_name
    ///  was not specified, the object is a member of some group.
    ///  False if the object  is not a member of the specified group.
    ///  If a group_name was not specified, the object is not a member of any group</returns>
    static member IsObjectInGroup(objectId:Guid, [<OPT;DEF(null)>]groupName:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectInGroup(object_id, group_name=None):
        '''Verifies that an object is a member of a group
        Parameters:
          object_id (guid): The identifier of an object
          group_name (str, optional): The name of a group. If omitted, the function
            verifies that the object is a member of any group
        Returns:
          bool: True if the object is a member of the specified group. If a group_name
            was not specified, the object is a member of some group. 
            False if the object  is not a member of the specified group. 
            If a group_name was not specified, the object is not a member of any group
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        count = rhobj.GroupCount
        if count<1: return False
        if not group_name: return True
        index = scriptcontext.doc.Groups.Find(group_name)
        if index<0: raise ValueError("%s group does not exist"%group_name)
        group_ids = rhobj.GetGroupList()
        for id in group_ids:
            if id==index: return True
        return False
    *)


    ///<summary>Verifies that an object is locked. Locked objects are visible, and can
    ///  be snapped to, but cannot be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is locked, False if the object is not locked</returns>
    static member IsObjectLocked(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectLocked(object_id):
        '''Verifies that an object is locked. Locked objects are visible, and can
        be snapped to, but cannot be selected
        Parameters:
          object_id (guid): The identifier of an object to be tested
        Returns:
          bool: True if the object is locked, False if the object is not locked
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.IsLocked
    *)


    ///<summary>Verifies that an object is normal. Normal objects are visible, can be
    ///  snapped to, and can be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is normal, False if the object is not normal</returns>
    static member IsObjectNormal(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectNormal(object_id):
        '''Verifies that an object is normal. Normal objects are visible, can be
        snapped to, and can be selected
        Parameters:
          object_id (guid): The identifier of an object to be tested
        Returns:
          bool: True if the object is normal, False if the object is not normal
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.IsNormal
    *)


    ///<summary>Verifies that an object is a reference object. Reference objects are
    ///  objects that are not part of the current document</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is a reference object, False if the object is not a reference object</returns>
    static member IsObjectReference(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectReference(object_id):
        '''Verifies that an object is a reference object. Reference objects are
        objects that are not part of the current document
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True if the object is a reference object, False if the object is not a reference object
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.IsReference
    *)


    ///<summary>Verifies that an object can be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True or False</returns>
    static member IsObjectSelectable(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectSelectable(object_id):
        '''Verifies that an object can be selected
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True or False
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.IsSelectable(True,False,False,False)
    *)


    ///<summary>Verifies that an object is currently selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(int) 0, the object is not selected
    ///  1, the object is selected
    ///  2, the object is entirely persistently selected
    ///  3, one or more proper sub-objects are selected</returns>
    static member IsObjectSelected(objectId:Guid) : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectSelected(object_id):
        '''Verifies that an object is currently selected.
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          int: 
            0, the object is not selected
            1, the object is selected
            2, the object is entirely persistently selected
            3, one or more proper sub-objects are selected
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.IsSelected(False)
    *)


    ///<summary>Determines if an object is closed, solid</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is solid, or a mesh is closed., False otherwise.</returns>
    static member IsObjectSolid(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectSolid(object_id):
        '''Determines if an object is closed, solid
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True if the object is solid, or a mesh is closed., False otherwise.
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        geom = rhobj.Geometry
        geometry_type = geom.ObjectType
        
        if geometry_type == Rhino.DocObjects.ObjectType.Mesh:
            return geom.IsClosed
        if (geometry_type == Rhino.DocObjects.ObjectType.Surface or
            geometry_type == Rhino.DocObjects.ObjectType.Brep or
            geometry_type == Rhino.DocObjects.ObjectType.Extrusion):
            return geom.IsSolid
        return False
    *)


    ///<summary>Verifies an object's geometry is valid and without error</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is valid</returns>
    static member IsObjectValid(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsObjectValid(object_id):
        '''Verifies an object's geometry is valid and without error
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True if the object is valid
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.IsValid
    *)


    ///<summary>Verifies an object is visible in a view</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///He title of the view.  If omitted, the current active view is used.</param>
    ///<returns>(bool) True if the object is visible in the specified view, otherwise False.</returns>
    static member IsVisibleInView(objectId:Guid, [<OPT;DEF(null)>]view:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsVisibleInView(object_id, view=None):
        '''Verifies an object is visible in a view
        Parameters:
          object_id (guid): the identifier of an object to test
          view (str, optional): he title of the view.  If omitted, the current active view is used.
        Returns:
          bool: True if the object is visible in the specified view, otherwise False.  None on error
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        viewport = __viewhelper(view).MainViewport
        bbox = rhobj.Geometry.GetBoundingBox(True)
        return rhobj.Visible and viewport.IsVisible(bbox)
    *)


    ///<summary>Locks a single object. Locked objects are visible, and they can be
    ///  snapped to. But, they cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member LockObject(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def LockObject(object_id):
        '''Locks a single object. Locked objects are visible, and they can be
        snapped to. But, they cannot be selected.
        Parameters:
          object_id (guid): The identifier of an object
        Returns:
          bool: True or False indicating success or failure
        '''
        return LockObjects(object_id)==1
    *)


    ///<summary>Locks one or more objects. Locked objects are visible, and they can be
    ///  snapped to. But, they cannot be selected.</summary>
    ///<param name="objectIds">(Guid seq) List of Strings or Guids. The identifiers of objects</param>
    ///<returns>(float) number of objects locked</returns>
    static member LockObjects(objectIds:Guid seq) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def LockObjects(object_ids):
        '''Locks one or more objects. Locked objects are visible, and they can be
        snapped to. But, they cannot be selected.
        Parameters:
          object_ids ([guid, ...]): list of Strings or Guids. The identifiers of objects
        Returns:
          number: number of objects locked
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        rc = 0
        for id in object_ids:
            id = rhutil.coerceguid(id, True)
            if scriptcontext.doc.Objects.Lock(id, False): rc += 1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Matches, or copies the attributes of a source object to a target object</summary>
    ///<param name="targetIds">(Guid seq) Identifiers of objects to copy attributes to</param>
    ///<param name="sourceId">(Guid) Optional, Default Value: <c>null</c>
    ///Identifier of object to copy attributes from. If None,
    ///  then the default attributes are copied to the targetIds</param>
    ///<returns>(float) number of objects modified</returns>
    static member MatchObjectAttributes(targetIds:Guid seq, [<OPT;DEF(null)>]sourceId:Guid) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def MatchObjectAttributes(target_ids, source_id=None):
        '''Matches, or copies the attributes of a source object to a target object
        Parameters:
          target_ids ([guid, ...]): identifiers of objects to copy attributes to
          source_id (guid, optional): identifier of object to copy attributes from. If None,
            then the default attributes are copied to the target_ids
        Returns:
          number: number of objects modified
        '''
        id = rhutil.coerceguid(target_ids, False)
        if id: target_ids = [id]
        source_attr = Rhino.DocObjects.ObjectAttributes()
        if source_id:
            source = rhutil.coercerhinoobject(source_id, True, True)
            source_attr = source.Attributes.Duplicate()
        rc = 0
        for id in target_ids:
            id = rhutil.coerceguid(id, True)
            if scriptcontext.doc.Objects.ModifyAttributes(id, source_attr, True):
                rc += 1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Mirrors a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to mirror</param>
    ///<param name="startPoint">(Point3d) Start of the mirror plane</param>
    ///<param name="endePoint">(Point3d) End of the mirror plane</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the mirrored object</returns>
    static member MirrorObject(objectId:Guid, startPoint:Point3d, endePoint:Point3d, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def MirrorObject(object_id, start_point, end_point, copy=False):
        '''Mirrors a single object
        Parameters:
          object_id (guid): The identifier of an object to mirror
          start_point (point): start of the mirror plane
          end_point (point): end of the mirror plane
          copy (bool, optional): copy the object
        Returns:
          guid: Identifier of the mirrored object if successful
          None: on error
        '''
        rc = MirrorObjects(object_id, start_point, end_point, copy)
        if rc: return rc[0]
    *)


    ///<summary>Mirrors a list of objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to mirror</param>
    ///<param name="startPoint">(Point3d) Start of the mirror plane</param>
    ///<param name="endePoint">(Point3d) End of the mirror plane</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid seq) List of identifiers of the mirrored objects</returns>
    static member MirrorObjects(objectIds:Guid seq, startPoint:Point3d, endePoint:Point3d, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def MirrorObjects(object_ids, start_point, end_point, copy=False):
        '''Mirrors a list of objects
        Parameters:
          object_ids ([guid, ...]): identifiers of objects to mirror
          start_point (point): start of the mirror plane
          end_point (point): end of the mirror plane
          copy (bool, optional): copy the objects
        Returns:
          list(guid, ...): List of identifiers of the mirrored objects if successful
        '''
        start_point = rhutil.coerce3dpoint(start_point, True)
        end_point = rhutil.coerce3dpoint(end_point, True)
        vec = end_point-start_point
        if vec.IsTiny(0): raise Exception("start and end points are too close to each other")
        normal = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane().Normal
        vec = Rhino.Geometry.Vector3d.CrossProduct(vec, normal)
        vec.Unitize()
        xf = Rhino.Geometry.Transform.Mirror(start_point, vec)
        rc = TransformObjects(object_ids, xf, copy)
        return rc
    *)


    ///<summary>Moves a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(Guid) Identifier of the moved object</returns>
    static member MoveObject(objectId:Guid, translation:Vector3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def MoveObject(object_id, translation):
        '''Moves a single object
        Parameters:
          object_id (guid): The identifier of an object to move
          translation (vector): list of 3 numbers or Vector3d
        Returns:
          guid: Identifier of the moved object if successful
          None: on error
        '''
        rc = MoveObjects(object_id, translation)
        if rc: return rc[0]
    *)


    ///<summary>Moves one or more objects</summary>
    ///<param name="objectIds">(Guid seq) The identifiers objects to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(Guid seq) identifiers of the moved objects</returns>
    static member MoveObjects(objectIds:Guid seq, translation:Vector3d) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def MoveObjects(object_ids, translation):
        '''Moves one or more objects
        Parameters:
          object_ids ([guid, ...]): The identifiers objects to move
          translation (vector): list of 3 numbers or Vector3d
        Returns:
          list(guid, ...): identifiers of the moved objects if successful
        '''
        translation = rhutil.coerce3dvector(translation, True)
        xf = Rhino.Geometry.Transform.Translation(translation)
        rc = TransformObjects(object_ids, xf)
        return rc
    *)


    ///<summary>Returns the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectIds">(Guid) Id or ids of object(s)</param>
    ///<returns>(Drawing.Color) The current color value</returns>
    static member ObjectColor(objectIds:Guid) : Drawing.Color = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectColor(object_ids, color=None):
        '''Returns or modifies the color of an object. Object colors are represented
        as RGB colors. An RGB color specifies the relative intensity of red, green,
        and blue to cause a specific color to be displayed
        Parameters:
            object_ids ([guid, ...]): id or ids of object(s)
            color (color, optional): the new color value. If omitted, then current object
                color is returned. If object_ids is a list, color is required
        Returns:
            color: If color value is not specified, the current color value
            color: If color value is specified, the previous color value
            number: If object_ids is a list, then the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        rhino_object = None
        rhino_objects = None
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
        else:
            rhino_objects = [rhutil.coercerhinoobject(id, True, True) for id in object_ids]
            if len(rhino_objects)==1:
                rhino_object = rhino_objects[0]
                rhino_objects = None
        if color is None:
            #get the color
            if rhino_objects: raise ValueError("color must be specified when a list of rhino objects is provided")
            return rhino_object.Attributes.DrawColor(scriptcontext.doc)
        color = rhutil.coercecolor(color, True)
        if rhino_objects is not None:
            for rh_obj in rhino_objects:
                attr = rh_obj.Attributes
                attr.ObjectColor = color
                attr.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject
                scriptcontext.doc.Objects.ModifyAttributes( rh_obj, attr, True)
            scriptcontext.doc.Views.Redraw()
            return len(rhino_objects)
        rc = rhino_object.Attributes.DrawColor(scriptcontext.doc)
        attr = rhino_object.Attributes
        attr.ObjectColor = color
        attr.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject
        scriptcontext.doc.Objects.ModifyAttributes( rhino_object, attr, True )
        scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectIds">(Guid) Id or ids of object(s)</param>
    ///<param name="color">(Drawing.Color)The new color value. If omitted, then current object
    ///  color is returned. If objectIds is a list, color is required</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectColor(objectIds:Guid, color:Drawing.Color) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectColor(object_ids, color=None):
        '''Returns or modifies the color of an object. Object colors are represented
        as RGB colors. An RGB color specifies the relative intensity of red, green,
        and blue to cause a specific color to be displayed
        Parameters:
            object_ids ([guid, ...]): id or ids of object(s)
            color (color, optional): the new color value. If omitted, then current object
                color is returned. If object_ids is a list, color is required
        Returns:
            color: If color value is not specified, the current color value
            color: If color value is specified, the previous color value
            number: If object_ids is a list, then the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        rhino_object = None
        rhino_objects = None
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
        else:
            rhino_objects = [rhutil.coercerhinoobject(id, True, True) for id in object_ids]
            if len(rhino_objects)==1:
                rhino_object = rhino_objects[0]
                rhino_objects = None
        if color is None:
            #get the color
            if rhino_objects: raise ValueError("color must be specified when a list of rhino objects is provided")
            return rhino_object.Attributes.DrawColor(scriptcontext.doc)
        color = rhutil.coercecolor(color, True)
        if rhino_objects is not None:
            for rh_obj in rhino_objects:
                attr = rh_obj.Attributes
                attr.ObjectColor = color
                attr.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject
                scriptcontext.doc.Objects.ModifyAttributes( rh_obj, attr, True)
            scriptcontext.doc.Views.Redraw()
            return len(rhino_objects)
        rc = rhino_object.Attributes.DrawColor(scriptcontext.doc)
        attr = rhino_object.Attributes
        attr.ObjectColor = color
        attr.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject
        scriptcontext.doc.Objects.ModifyAttributes( rhino_object, attr, True )
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Modifies the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectIds">(Guid seq) Id or ids of object(s)</param>
    ///<param name="color">(Drawing.Color)The new color value. If omitted, then current object
    ///  color is returned. If objectIds is a list, color is required</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectColor(objectIds:Guid, color:Drawing.Color seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectColor(object_ids, color=None):
        '''Returns or modifies the color of an object. Object colors are represented
        as RGB colors. An RGB color specifies the relative intensity of red, green,
        and blue to cause a specific color to be displayed
        Parameters:
            object_ids ([guid, ...]): id or ids of object(s)
            color (color, optional): the new color value. If omitted, then current object
                color is returned. If object_ids is a list, color is required
        Returns:
            color: If color value is not specified, the current color value
            color: If color value is specified, the previous color value
            number: If object_ids is a list, then the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        rhino_object = None
        rhino_objects = None
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
        else:
            rhino_objects = [rhutil.coercerhinoobject(id, True, True) for id in object_ids]
            if len(rhino_objects)==1:
                rhino_object = rhino_objects[0]
                rhino_objects = None
        if color is None:
            #get the color
            if rhino_objects: raise ValueError("color must be specified when a list of rhino objects is provided")
            return rhino_object.Attributes.DrawColor(scriptcontext.doc)
        color = rhutil.coercecolor(color, True)
        if rhino_objects is not None:
            for rh_obj in rhino_objects:
                attr = rh_obj.Attributes
                attr.ObjectColor = color
                attr.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject
                scriptcontext.doc.Objects.ModifyAttributes( rh_obj, attr, True)
            scriptcontext.doc.Views.Redraw()
            return len(rhino_objects)
        rc = rhino_object.Attributes.DrawColor(scriptcontext.doc)
        attr = rhino_object.Attributes
        attr.ObjectColor = color
        attr.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject
        scriptcontext.doc.Objects.ModifyAttributes( rhino_object, attr, True )
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the color source of an object.</summary>
    ///<param name="objectIds">(Guid) Single identifier of list of identifiers</param>
    ///<returns>(int) The current color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</returns>
    static member ObjectColorSource(objectIds:Guid) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectColorSource(object_ids, source=None):
        '''Returns or modifies the color source of an object.
        Parameters:
          object_ids ([guid, ...]): single identifier of list of identifiers
          source (number, optional) = new color source
              0 = color from layer
              1 = color from object
              2 = color from material
              3 = color from parent
        Returns:
          int: if color source is not specified, the current color source
          int: is color source is specified, the previous color source
          int: if color_ids is a list, then the number of objects modifief
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhobj = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhobj.Attributes.ColorSource)
            if source is not None:
                rhobj.Attributes.ColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        else:
            rc = 0
            source = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
            for id in object_ids:
                rhobj = rhutil.coercerhinoobject(id, True, True)
                rhobj.Attributes.ColorSource = source
                rhobj.CommitChanges()
                rc += 1
            if rc: scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the color source of an object.</summary>
    ///<param name="objectIds">(Guid) Single identifier of list of identifiers</param>
    ///<param name="source">(int)New color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectColorSource(objectIds:Guid, source:int) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectColorSource(object_ids, source=None):
        '''Returns or modifies the color source of an object.
        Parameters:
          object_ids ([guid, ...]): single identifier of list of identifiers
          source (number, optional) = new color source
              0 = color from layer
              1 = color from object
              2 = color from material
              3 = color from parent
        Returns:
          int: if color source is not specified, the current color source
          int: is color source is specified, the previous color source
          int: if color_ids is a list, then the number of objects modifief
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhobj = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhobj.Attributes.ColorSource)
            if source is not None:
                rhobj.Attributes.ColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        else:
            rc = 0
            source = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
            for id in object_ids:
                rhobj = rhutil.coercerhinoobject(id, True, True)
                rhobj.Attributes.ColorSource = source
                rhobj.CommitChanges()
                rc += 1
            if rc: scriptcontext.doc.Views.Redraw()
            return rc
    *)


    ///<summary>Modifies the color source of an object.</summary>
    ///<param name="objectIds">(Guid seq) Single identifier of list of identifiers</param>
    ///<param name="source">(int)New color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectColorSource(objectIds:Guid, source:int seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectColorSource(object_ids, source=None):
        '''Returns or modifies the color source of an object.
        Parameters:
          object_ids ([guid, ...]): single identifier of list of identifiers
          source (number, optional) = new color source
              0 = color from layer
              1 = color from object
              2 = color from material
              3 = color from parent
        Returns:
          int: if color source is not specified, the current color source
          int: is color source is specified, the previous color source
          int: if color_ids is a list, then the number of objects modifief
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhobj = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhobj.Attributes.ColorSource)
            if source is not None:
                rhobj.Attributes.ColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        else:
            rc = 0
            source = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
            for id in object_ids:
                rhobj = rhutil.coercerhinoobject(id, True, True)
                rhobj.Attributes.ColorSource = source
                rhobj.CommitChanges()
                rc += 1
            if rc: scriptcontext.doc.Views.Redraw()
            return rc
    *)


    ///<summary>Returns a short text description of an object</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(string) A short text description of the object .</returns>
    static member ObjectDescription(objectId:Guid) : string =
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectDescription(object_id):
        '''Returns a short text description of an object
        Parameters:
          object_id (guid): identifier of an object
        Returns:
          str: A short text description of the object if successful.
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        return rhobj.ShortDescription(False)
    *)


    ///<summary>Returns all of the group names that an object is assigned to</summary>
    ///<param name="objectId">(Guid) Identifier of an object(s)</param>
    ///<returns>(string seq) list of group names on success</returns>
    static member ObjectGroups(objectId:Guid) : string seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectGroups(object_id):
        '''Returns all of the group names that an object is assigned to
        Parameters:
          object_id ([guid, ...]): identifier of an object(s)
        Returns:
          list(str, ...): list of group names on success
        '''
        rhino_object = rhutil.coercerhinoobject(object_id, True, True)
        if rhino_object.GroupCount<1: return []
        group_indices = rhino_object.GetGroupList()
        rc = [scriptcontext.doc.Groups.GroupName(index) for index in group_indices]
        return rc
    *)


    ///<summary>Returns the layer of an object</summary>
    ///<param name="objectId">(Guid) The identifier of the object(s)</param>
    ///<returns>(string) The object's current layer</returns>
    static member ObjectLayer(objectId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLayer(object_id, layer=None):
        '''Returns or modifies the layer of an object
        Parameters:
          object_id ([guid, ...]) the identifier of the object(s)
          layer (str, optional):  name of an existing layer
        Returns:
          str: If a layer is not specified, the object's current layer
          str: If a layer is specified, the object's previous layer
          number: If object_id is a list or tuple, the number of objects modified
        '''
        if type(object_id) is not str and hasattr(object_id, "__len__"):
            layer = __getlayer(layer, True)
            index = layer.LayerIndex
            for id in object_id:
                obj = rhutil.coercerhinoobject(id, True, True)
                obj.Attributes.LayerIndex = index
                obj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
            return len(object_id)
        obj = rhutil.coercerhinoobject(object_id, True, True)
        if obj is None: return scriptcontext.errorhandler()
        index = obj.Attributes.LayerIndex
        rc = scriptcontext.doc.Layers[index].FullPath
        if layer:
            layer = __getlayer(layer, True)
            index = layer.LayerIndex
            obj.Attributes.LayerIndex = index
            obj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the layer of an object</summary>
    ///<param name="objectId">(Guid) The identifier of the object(s)</param>
    ///<param name="layer">(string)Name of an existing layer</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLayer(objectId:Guid, layer:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLayer(object_id, layer=None):
        '''Returns or modifies the layer of an object
        Parameters:
          object_id ([guid, ...]) the identifier of the object(s)
          layer (str, optional):  name of an existing layer
        Returns:
          str: If a layer is not specified, the object's current layer
          str: If a layer is specified, the object's previous layer
          number: If object_id is a list or tuple, the number of objects modified
        '''
        if type(object_id) is not str and hasattr(object_id, "__len__"):
            layer = __getlayer(layer, True)
            index = layer.LayerIndex
            for id in object_id:
                obj = rhutil.coercerhinoobject(id, True, True)
                obj.Attributes.LayerIndex = index
                obj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
            return len(object_id)
        obj = rhutil.coercerhinoobject(object_id, True, True)
        if obj is None: return scriptcontext.errorhandler()
        index = obj.Attributes.LayerIndex
        rc = scriptcontext.doc.Layers[index].FullPath
        if layer:
            layer = __getlayer(layer, True)
            index = layer.LayerIndex
            obj.Attributes.LayerIndex = index
            obj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Modifies the layer of an object</summary>
    ///<param name="objectId">(Guid seq) The identifier of the object(s)</param>
    ///<param name="layer">(string)Name of an existing layer</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLayer(objectId:Guid, layer:string seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLayer(object_id, layer=None):
        '''Returns or modifies the layer of an object
        Parameters:
          object_id ([guid, ...]) the identifier of the object(s)
          layer (str, optional):  name of an existing layer
        Returns:
          str: If a layer is not specified, the object's current layer
          str: If a layer is specified, the object's previous layer
          number: If object_id is a list or tuple, the number of objects modified
        '''
        if type(object_id) is not str and hasattr(object_id, "__len__"):
            layer = __getlayer(layer, True)
            index = layer.LayerIndex
            for id in object_id:
                obj = rhutil.coercerhinoobject(id, True, True)
                obj.Attributes.LayerIndex = index
                obj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
            return len(object_id)
        obj = rhutil.coercerhinoobject(object_id, True, True)
        if obj is None: return scriptcontext.errorhandler()
        index = obj.Attributes.LayerIndex
        rc = scriptcontext.doc.Layers[index].FullPath
        if layer:
            layer = __getlayer(layer, True)
            index = layer.LayerIndex
            obj.Attributes.LayerIndex = index
            obj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the layout or model space of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The object's current page layout view</returns>
    static member ObjectLayout(objectId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLayout(object_id, layout=None, return_name=True):
        '''Returns or changes the layout or model space of an object
        Parameters:
          object_id (guid): identifier of the object
          layout (str|guid, optional): to change, or move, an object from model space to page
            layout space, or from one page layout to another, then specify the
            title or identifier of an existing page layout view. To move an object
            from page layout space to model space, just specify None
          return_name(bool,optional): If True, the name, or title, of the page layout view
            is returned. If False, the identifier of the page layout view is returned
        Returns:
          str: if layout is not specified, the object's current page layout view
          str: if layout is specified, the object's previous page layout view
          None: if not successful
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        rc = None
        if rhobj.Attributes.Space==Rhino.DocObjects.ActiveSpace.PageSpace:
            page_id = rhobj.Attributes.ViewportId
            pageview = scriptcontext.doc.Views.Find(page_id)
            if return_name: rc = pageview.MainViewport.Name
            else: rc = pageview.MainViewport.Id
            if layout is None: #move to model space
                rhobj.Attributes.Space = Rhino.DocObjects.ActiveSpace.ModelSpace
                rhobj.Attributes.ViewportId = System.Guid.Empty
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
        else:
            if layout:
                layout = scriptcontext.doc.Views.Find(layout, False)
                if layout is not None and isinstance(layout, Rhino.Display.RhinoPageView):
                    rhobj.Attributes.ViewportId = layout.MainViewport.Id
                    rhobj.Attributes.Space = Rhino.DocObjects.ActiveSpace.PageSpace
                    rhobj.CommitChanges()
                    scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the layout or model space of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="layout">(string)To change, or move, an object from model space to page
    ///  layout space, or from one page layout to another, then specify the
    ///  title or identifier of an existing page layout view. To move an object
    ///  from page layout space to model space, just specify None</param>
    ///<param name="returnName">(bool)If True, the name, or title, of the page layout view
    ///  is returned. If False, the identifier of the page layout view is returned</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLayout(objectId:Guid, layout:string, [<OPT;DEF(true)>]returnName:bool) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLayout(object_id, layout=None, return_name=True):
        '''Returns or changes the layout or model space of an object
        Parameters:
          object_id (guid): identifier of the object
          layout (str|guid, optional): to change, or move, an object from model space to page
            layout space, or from one page layout to another, then specify the
            title or identifier of an existing page layout view. To move an object
            from page layout space to model space, just specify None
          return_name(bool,optional): If True, the name, or title, of the page layout view
            is returned. If False, the identifier of the page layout view is returned
        Returns:
          str: if layout is not specified, the object's current page layout view
          str: if layout is specified, the object's previous page layout view
          None: if not successful
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        rc = None
        if rhobj.Attributes.Space==Rhino.DocObjects.ActiveSpace.PageSpace:
            page_id = rhobj.Attributes.ViewportId
            pageview = scriptcontext.doc.Views.Find(page_id)
            if return_name: rc = pageview.MainViewport.Name
            else: rc = pageview.MainViewport.Id
            if layout is None: #move to model space
                rhobj.Attributes.Space = Rhino.DocObjects.ActiveSpace.ModelSpace
                rhobj.Attributes.ViewportId = System.Guid.Empty
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
        else:
            if layout:
                layout = scriptcontext.doc.Views.Find(layout, False)
                if layout is not None and isinstance(layout, Rhino.Display.RhinoPageView):
                    rhobj.Attributes.ViewportId = layout.MainViewport.Id
                    rhobj.Attributes.Space = Rhino.DocObjects.ActiveSpace.PageSpace
                    rhobj.CommitChanges()
                    scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the linetype of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(string) The object's current linetype</returns>
    static member ObjectLinetype(objectIds:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLinetype(object_ids, linetype=None):
        '''Returns or modifies the linetype of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          linetype (str, optional): name of an existing linetype. If omitted, the current
            linetype is returned. If object_ids is a list of identifiers, this parameter
            is required
        Returns:
          str: If a linetype is not specified, the object's current linetype
          str: If linetype is specified, the object's previous linetype
          number: If object_ids is a list, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            oldindex = scriptcontext.doc.Linetypes.LinetypeIndexForObject(rhino_object)
            if linetype:
                newindex = scriptcontext.doc.Linetypes.Find(linetype)
                rhino_object.Attributes.LinetypeSource = Rhino.DocObjects.ObjectLinetypeSource.LinetypeFromObject
                rhino_object.Attributes.LinetypeIndex = newindex
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return scriptcontext.doc.Linetypes[oldindex].Name
        newindex = scriptcontext.doc.Linetypes.Find(linetype)
        if newindex<0: raise Exception("%s does not exist in LineTypes table"%linetype)
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.LinetypeSource = Rhino.DocObjects.ObjectLinetypeSource.LinetypeFromObject
            rhino_object.Attributes.LinetypeIndex = newindex
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)

    ///<summary>Modifies the linetype of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="linetyp">(string)Name of an existing linetyp. If omitted, the current
    ///  linetyp is returned. If objectIds is a list of identifiers, this parameter
    ///  is required</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLinetype(objectIds:Guid, linetyp:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLinetype(object_ids, linetype=None):
        '''Returns or modifies the linetype of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          linetype (str, optional): name of an existing linetype. If omitted, the current
            linetype is returned. If object_ids is a list of identifiers, this parameter
            is required
        Returns:
          str: If a linetype is not specified, the object's current linetype
          str: If linetype is specified, the object's previous linetype
          number: If object_ids is a list, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            oldindex = scriptcontext.doc.Linetypes.LinetypeIndexForObject(rhino_object)
            if linetype:
                newindex = scriptcontext.doc.Linetypes.Find(linetype)
                rhino_object.Attributes.LinetypeSource = Rhino.DocObjects.ObjectLinetypeSource.LinetypeFromObject
                rhino_object.Attributes.LinetypeIndex = newindex
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return scriptcontext.doc.Linetypes[oldindex].Name
        newindex = scriptcontext.doc.Linetypes.Find(linetype)
        if newindex<0: raise Exception("%s does not exist in LineTypes table"%linetype)
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.LinetypeSource = Rhino.DocObjects.ObjectLinetypeSource.LinetypeFromObject
            rhino_object.Attributes.LinetypeIndex = newindex
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Modifies the linetype of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="linetyp">(string)Name of an existing linetyp. If omitted, the current
    ///  linetyp is returned. If objectIds is a list of identifiers, this parameter
    ///  is required</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLinetype(objectIds:Guid, linetyp:string seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLinetype(object_ids, linetype=None):
        '''Returns or modifies the linetype of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          linetype (str, optional): name of an existing linetype. If omitted, the current
            linetype is returned. If object_ids is a list of identifiers, this parameter
            is required
        Returns:
          str: If a linetype is not specified, the object's current linetype
          str: If linetype is specified, the object's previous linetype
          number: If object_ids is a list, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            oldindex = scriptcontext.doc.Linetypes.LinetypeIndexForObject(rhino_object)
            if linetype:
                newindex = scriptcontext.doc.Linetypes.Find(linetype)
                rhino_object.Attributes.LinetypeSource = Rhino.DocObjects.ObjectLinetypeSource.LinetypeFromObject
                rhino_object.Attributes.LinetypeIndex = newindex
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return scriptcontext.doc.Linetypes[oldindex].Name
        newindex = scriptcontext.doc.Linetypes.Find(linetype)
        if newindex<0: raise Exception("%s does not exist in LineTypes table"%linetype)
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.LinetypeSource = Rhino.DocObjects.ObjectLinetypeSource.LinetypeFromObject
            rhino_object.Attributes.LinetypeIndex = newindex
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Returns the linetype source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(int) The object's current linetype source
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</returns>
    static member ObjectLinetypeSource(objectIds:Guid) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLinetypeSource(object_ids, source=None):
        '''Returns or modifies the linetype source of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          source (number, optional): new linetype source. If omitted, the current source is returned.
            If object_ids is a list of identifiers, this parameter is required
              0 = By Layer
              1 = By Object
              3 = By Parent
        Returns:
          number: If a source is not specified, the object's current linetype source
          number: If source is specified, the object's previous linetype source
          number: If object_ids is a list, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            oldsource = rhino_object.Attributes.LinetypeSource
            if source is not None:
                source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
                rhino_object.Attributes.LinetypeSource = source
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return int(oldsource)
        source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.LinetypeSource = source
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)

    ///<summary>Modifies the linetype source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="source">(int)New linetype source. If omitted, the current source is returned.
    ///  If objectIds is a list of identifiers, this parameter is required
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLinetypeSource(objectIds:Guid, source:int) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLinetypeSource(object_ids, source=None):
        '''Returns or modifies the linetype source of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          source (number, optional): new linetype source. If omitted, the current source is returned.
            If object_ids is a list of identifiers, this parameter is required
              0 = By Layer
              1 = By Object
              3 = By Parent
        Returns:
          number: If a source is not specified, the object's current linetype source
          number: If source is specified, the object's previous linetype source
          number: If object_ids is a list, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            oldsource = rhino_object.Attributes.LinetypeSource
            if source is not None:
                source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
                rhino_object.Attributes.LinetypeSource = source
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return int(oldsource)
        source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.LinetypeSource = source
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Modifies the linetype source of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="source">(int)New linetype source. If omitted, the current source is returned.
    ///  If objectIds is a list of identifiers, this parameter is required
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLinetypeSource(objectIds:Guid, source:int seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectLinetypeSource(object_ids, source=None):
        '''Returns or modifies the linetype source of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          source (number, optional): new linetype source. If omitted, the current source is returned.
            If object_ids is a list of identifiers, this parameter is required
              0 = By Layer
              1 = By Object
              3 = By Parent
        Returns:
          number: If a source is not specified, the object's current linetype source
          number: If source is specified, the object's previous linetype source
          number: If object_ids is a list, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            oldsource = rhino_object.Attributes.LinetypeSource
            if source is not None:
                source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
                rhino_object.Attributes.LinetypeSource = source
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return int(oldsource)
        source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.LinetypeSource = source
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Returns the material index of an object. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(int) If the return value of ObjectMaterialSource is "material by object", then
    ///  the return value of this function is the index of the object's rendering
    ///  material. A material index of -1 indicates no material has been assigned,
    ///  and that Rhino's internal default material has been assigned to the object.</returns>
    static member ObjectMaterialIndex(objectId:Guid) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectMaterialIndex(object_id, material_index=None):
        '''Returns or changes the material index of an object. Rendering materials are stored in
        Rhino's rendering material table. The table is conceptually an array. Render
        materials associated with objects and layers are specified by zero based
        indices into this array.
        Parameters:
          object_id (guid): identifier of an object
          material_index (number, optional): the new material index
        Returns:
          number: If the return value of ObjectMaterialSource is "material by object", then
              the return value of this function is the index of the object's rendering
              material. A material index of -1 indicates no material has been assigned,
              and that Rhino's internal default material has been assigned to the object.          
          None: on failure
        '''
        rhino_object = rhutil.coercerhinoobject(object_id, True, True)
        if material_index is not None and material_index < scriptcontext.doc.Materials.Count:
          attrs = rhino_object.Attributes
          attrs.MaterialIndex = material_index
          scriptcontext.doc.Objects.ModifyAttributes(rhino_object, attrs, True)
        return rhino_object.Attributes.MaterialIndex
    *)

    ///<summary>Changes the material index of an object. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<param name="materialIndex">(int)The new material index</param>
    static member ObjectMaterialIndex(objectId:Guid, materialIndex:int) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectMaterialIndex(object_id, material_index=None):
        '''Returns or changes the material index of an object. Rendering materials are stored in
        Rhino's rendering material table. The table is conceptually an array. Render
        materials associated with objects and layers are specified by zero based
        indices into this array.
        Parameters:
          object_id (guid): identifier of an object
          material_index (number, optional): the new material index
        Returns:
          number: If the return value of ObjectMaterialSource is "material by object", then
              the return value of this function is the index of the object's rendering
              material. A material index of -1 indicates no material has been assigned,
              and that Rhino's internal default material has been assigned to the object.          
          None: on failure
        '''
        rhino_object = rhutil.coercerhinoobject(object_id, True, True)
        if material_index is not None and material_index < scriptcontext.doc.Materials.Count:
          attrs = rhino_object.Attributes
          attrs.MaterialIndex = material_index
          scriptcontext.doc.Objects.ModifyAttributes(rhino_object, attrs, True)
        return rhino_object.Attributes.MaterialIndex
    *)


    ///<summary>Returns the rendering material source of an object.</summary>
    ///<param name="objectIds">(Guid) One or more object identifiers</param>
    ///<returns>(int) The current rendering material source
    ///  0 = Material from layer
    ///  1 = Material from object
    ///  3 = Material from parent</returns>
    static member ObjectMaterialSource(objectIds:Guid) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectMaterialSource(object_ids, source=None):
        '''Returns or modifies the rendering material source of an object.
        Parameters:
          object_ids ([guid, ...]): one or more object identifiers
          source (number, optional): The new rendering material source. If omitted and a single
            object is provided in object_ids, then the current material source is
            returned. This parameter is required if multiple objects are passed in
            object_ids
            0 = Material from layer
            1 = Material from object
            3 = Material from parent
        Returns:
          number: If source is not specified, the current rendering material source
          number: If source is specified, the previous rendering material source
          number: If object_ids is a list, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: # working with single object
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhino_object.Attributes.MaterialSource)
            if source is not None:
                rhino_object.Attributes.MaterialSource = System.Enum.ToObject(Rhino.DocObjects.ObjectMaterialSource, source)
                rhino_object.CommitChanges()
            return rc
        # else working with multiple objects
        if source is None: raise Exception("source is required when object_ids represents multiple objects")
        source = System.Enum.ToObject(Rhino.DocObjects.ObjectMaterialSource, source)
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.MaterialSource = source
            rhino_object.CommitChanges()
        return len(object_ids)
    *)

    ///<summary>Modifies the rendering material source of an object.</summary>
    ///<param name="objectIds">(Guid) One or more object identifiers</param>
    ///<param name="source">(int)The new rendering material source. If omitted and a single
    ///  object is provided in objectIds, then the current material source is
    ///  returned. This parameter is required if multiple objects are passed in
    ///  objectIds
    ///  0 = Material from layer
    ///  1 = Material from object
    ///  3 = Material from parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectMaterialSource(objectIds:Guid, source:int) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectMaterialSource(object_ids, source=None):
        '''Returns or modifies the rendering material source of an object.
        Parameters:
          object_ids ([guid, ...]): one or more object identifiers
          source (number, optional): The new rendering material source. If omitted and a single
            object is provided in object_ids, then the current material source is
            returned. This parameter is required if multiple objects are passed in
            object_ids
            0 = Material from layer
            1 = Material from object
            3 = Material from parent
        Returns:
          number: If source is not specified, the current rendering material source
          number: If source is specified, the previous rendering material source
          number: If object_ids is a list, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: # working with single object
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhino_object.Attributes.MaterialSource)
            if source is not None:
                rhino_object.Attributes.MaterialSource = System.Enum.ToObject(Rhino.DocObjects.ObjectMaterialSource, source)
                rhino_object.CommitChanges()
            return rc
        # else working with multiple objects
        if source is None: raise Exception("source is required when object_ids represents multiple objects")
        source = System.Enum.ToObject(Rhino.DocObjects.ObjectMaterialSource, source)
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.MaterialSource = source
            rhino_object.CommitChanges()
        return len(object_ids)
    *)


    ///<summary>Modifies the rendering material source of an object.</summary>
    ///<param name="objectIds">(Guid seq) One or more object identifiers</param>
    ///<param name="source">(int)The new rendering material source. If omitted and a single
    ///  object is provided in objectIds, then the current material source is
    ///  returned. This parameter is required if multiple objects are passed in
    ///  objectIds
    ///  0 = Material from layer
    ///  1 = Material from object
    ///  3 = Material from parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectMaterialSource(objectIds:Guid, source:int seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectMaterialSource(object_ids, source=None):
        '''Returns or modifies the rendering material source of an object.
        Parameters:
          object_ids ([guid, ...]): one or more object identifiers
          source (number, optional): The new rendering material source. If omitted and a single
            object is provided in object_ids, then the current material source is
            returned. This parameter is required if multiple objects are passed in
            object_ids
            0 = Material from layer
            1 = Material from object
            3 = Material from parent
        Returns:
          number: If source is not specified, the current rendering material source
          number: If source is specified, the previous rendering material source
          number: If object_ids is a list, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: # working with single object
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhino_object.Attributes.MaterialSource)
            if source is not None:
                rhino_object.Attributes.MaterialSource = System.Enum.ToObject(Rhino.DocObjects.ObjectMaterialSource, source)
                rhino_object.CommitChanges()
            return rc
        # else working with multiple objects
        if source is None: raise Exception("source is required when object_ids represents multiple objects")
        source = System.Enum.ToObject(Rhino.DocObjects.ObjectMaterialSource, source)
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.MaterialSource = source
            rhino_object.CommitChanges()
        return len(object_ids)
    *)


    ///<summary>Returns the name of an object</summary>
    ///<param name="objectId">(Guid) Id or ids of object(s)</param>
    ///<returns>(string) The current object name</returns>
    static member ObjectName(objectId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectName(object_id, name=None):
        '''Returns or modifies the name of an object
        Parameters:
          object_id ([guid, ...]): id or ids of object(s)
          name (str, optional): the new object name. If omitted, the current name is returned
        Returns:
          str: If name is not specified, the current object name
          str: If name is specified, the previous object name
          number: If object_id is a list, the number of objects changed
        '''
        id = rhutil.coerceguid(object_id, False)
        rhino_object = None
        rhino_objects = None
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
        else:
            rhino_objects = [rhutil.coercerhinoobject(id, True, True) for id in object_id]
            if not rhino_objects: return 0
            if len(rhino_objects)==1:
                rhino_object = rhino_objects[0]
                rhino_objects = None
        if name is None: #get the name
            if rhino_objects: raise Exception("name required when object_id represents multiple objects")
            return rhino_object.Name
        if rhino_objects:
            for rh_obj in rhino_objects:
                attr = rh_obj.Attributes
                attr.Name = name
                scriptcontext.doc.Objects.ModifyAttributes(rh_obj, attr, True)
            return len(rhino_objects)
        rc = rhino_object.Name
        if not type(name) is str: name = str(name)
        rhino_object.Attributes.Name = name
        rhino_object.CommitChanges()
        return rc
    *)

    ///<summary>Modifies the name of an object</summary>
    ///<param name="objectId">(Guid) Id or ids of object(s)</param>
    ///<param name="name">(string)The new object name. If omitted, the current name is returned</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectName(objectId:Guid, name:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectName(object_id, name=None):
        '''Returns or modifies the name of an object
        Parameters:
          object_id ([guid, ...]): id or ids of object(s)
          name (str, optional): the new object name. If omitted, the current name is returned
        Returns:
          str: If name is not specified, the current object name
          str: If name is specified, the previous object name
          number: If object_id is a list, the number of objects changed
        '''
        id = rhutil.coerceguid(object_id, False)
        rhino_object = None
        rhino_objects = None
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
        else:
            rhino_objects = [rhutil.coercerhinoobject(id, True, True) for id in object_id]
            if not rhino_objects: return 0
            if len(rhino_objects)==1:
                rhino_object = rhino_objects[0]
                rhino_objects = None
        if name is None: #get the name
            if rhino_objects: raise Exception("name required when object_id represents multiple objects")
            return rhino_object.Name
        if rhino_objects:
            for rh_obj in rhino_objects:
                attr = rh_obj.Attributes
                attr.Name = name
                scriptcontext.doc.Objects.ModifyAttributes(rh_obj, attr, True)
            return len(rhino_objects)
        rc = rhino_object.Name
        if not type(name) is str: name = str(name)
        rhino_object.Attributes.Name = name
        rhino_object.CommitChanges()
        return rc
    *)


    ///<summary>Modifies the name of an object</summary>
    ///<param name="objectId">(Guid seq) Id or ids of object(s)</param>
    ///<param name="name">(string)The new object name. If omitted, the current name is returned</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectName(objectId:Guid, name:string seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectName(object_id, name=None):
        '''Returns or modifies the name of an object
        Parameters:
          object_id ([guid, ...]): id or ids of object(s)
          name (str, optional): the new object name. If omitted, the current name is returned
        Returns:
          str: If name is not specified, the current object name
          str: If name is specified, the previous object name
          number: If object_id is a list, the number of objects changed
        '''
        id = rhutil.coerceguid(object_id, False)
        rhino_object = None
        rhino_objects = None
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
        else:
            rhino_objects = [rhutil.coercerhinoobject(id, True, True) for id in object_id]
            if not rhino_objects: return 0
            if len(rhino_objects)==1:
                rhino_object = rhino_objects[0]
                rhino_objects = None
        if name is None: #get the name
            if rhino_objects: raise Exception("name required when object_id represents multiple objects")
            return rhino_object.Name
        if rhino_objects:
            for rh_obj in rhino_objects:
                attr = rh_obj.Attributes
                attr.Name = name
                scriptcontext.doc.Objects.ModifyAttributes(rh_obj, attr, True)
            return len(rhino_objects)
        rc = rhino_object.Name
        if not type(name) is str: name = str(name)
        rhino_object.Attributes.Name = name
        rhino_object.CommitChanges()
        return rc
    *)


    ///<summary>Returns the print color of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(Drawing.Color) The object's current print color</returns>
    static member ObjectPrintColor(objectIds:Guid) : Drawing.Color = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintColor(object_ids, color=None):
        '''Returns or modifies the print color of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          color (color, optional): new print color. If omitted, the current color is returned.
        Returns:
          color: If color is not specified, the object's current print color
          color: If color is specified, the object's previous print color
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = rhino_object.Attributes.PlotColor
            if color:
                rhino_object.Attributes.PlotColorSource = Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject
                rhino_object.Attributes.PlotColor = rhutil.coercecolor(color, True)
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            color = rhutil.coercecolor(color, True)
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotColorSource = Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject
            rhino_object.Attributes.PlotColor = color
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)

    ///<summary>Modifies the print color of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="color">(Drawing.Color)New print color. If omitted, the current color is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintColor(objectIds:Guid, color:Drawing.Color) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintColor(object_ids, color=None):
        '''Returns or modifies the print color of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          color (color, optional): new print color. If omitted, the current color is returned.
        Returns:
          color: If color is not specified, the object's current print color
          color: If color is specified, the object's previous print color
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = rhino_object.Attributes.PlotColor
            if color:
                rhino_object.Attributes.PlotColorSource = Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject
                rhino_object.Attributes.PlotColor = rhutil.coercecolor(color, True)
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            color = rhutil.coercecolor(color, True)
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotColorSource = Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject
            rhino_object.Attributes.PlotColor = color
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Modifies the print color of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="color">(Drawing.Color)New print color. If omitted, the current color is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintColor(objectIds:Guid, color:Drawing.Color seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintColor(object_ids, color=None):
        '''Returns or modifies the print color of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          color (color, optional): new print color. If omitted, the current color is returned.
        Returns:
          color: If color is not specified, the object's current print color
          color: If color is specified, the object's previous print color
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = rhino_object.Attributes.PlotColor
            if color:
                rhino_object.Attributes.PlotColorSource = Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject
                rhino_object.Attributes.PlotColor = rhutil.coercecolor(color, True)
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            color = rhutil.coercecolor(color, True)
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotColorSource = Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject
            rhino_object.Attributes.PlotColor = color
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Returns the print color source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(int) The object's current print color source
    ///  0 = print color by layer
    ///  1 = print color by object
    ///  3 = print color by parent</returns>
    static member ObjectPrintColorSource(objectIds:Guid) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintColorSource(object_ids, source=None):
        '''Returns or modifies the print color source of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          source (number, optional): new print color source
            0 = print color by layer
            1 = print color by object
            3 = print color by parent
        Returns:
          number: If source is not specified, the object's current print color source
          number: If source is specified, the object's previous print color source
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhino_object.Attributes.PlotColorSource)
            if source is not None:
                rhino_object.Attributes.PlotColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotColorSource, source)
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotColorSource, source)
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)

    ///<summary>Modifies the print color source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="source">(int)New print color source
    ///  0 = print color by layer
    ///  1 = print color by object
    ///  3 = print color by parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintColorSource(objectIds:Guid, source:int) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintColorSource(object_ids, source=None):
        '''Returns or modifies the print color source of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          source (number, optional): new print color source
            0 = print color by layer
            1 = print color by object
            3 = print color by parent
        Returns:
          number: If source is not specified, the object's current print color source
          number: If source is specified, the object's previous print color source
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhino_object.Attributes.PlotColorSource)
            if source is not None:
                rhino_object.Attributes.PlotColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotColorSource, source)
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotColorSource, source)
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Modifies the print color source of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="source">(int)New print color source
    ///  0 = print color by layer
    ///  1 = print color by object
    ///  3 = print color by parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintColorSource(objectIds:Guid, source:int seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintColorSource(object_ids, source=None):
        '''Returns or modifies the print color source of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          source (number, optional): new print color source
            0 = print color by layer
            1 = print color by object
            3 = print color by parent
        Returns:
          number: If source is not specified, the object's current print color source
          number: If source is specified, the object's previous print color source
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhino_object.Attributes.PlotColorSource)
            if source is not None:
                rhino_object.Attributes.PlotColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotColorSource, source)
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotColorSource, source)
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Returns the print width of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(float) The object's current print width</returns>
    static member ObjectPrintWidth(objectIds:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintWidth(object_ids, width=None):
        '''Returns or modifies the print width of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          width (number, optional): new print width value in millimeters, where width=0 means use
            the default width, and width<0 means do not print (visible for screen display,
            but does not show on print). If omitted, the current width is returned.
        Returns:
          number: If width is not specified, the object's current print width
          number: If width is specified, the object's previous print width
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = rhino_object.Attributes.PlotWeight
            if width is not None:
                rhino_object.Attributes.PlotWeightSource = Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
                rhino_object.Attributes.PlotWeight = width
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotWeightSource = Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
            rhino_object.Attributes.PlotWeight = width
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)

    ///<summary>Modifies the print width of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="width">(float)New print width value in millimeters, where width=0 means use
    ///  the default width, and width<0 means do not print (visible for screen display,
    ///  but does not show on print). If omitted, the current width is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintWidth(objectIds:Guid, width:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintWidth(object_ids, width=None):
        '''Returns or modifies the print width of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          width (number, optional): new print width value in millimeters, where width=0 means use
            the default width, and width<0 means do not print (visible for screen display,
            but does not show on print). If omitted, the current width is returned.
        Returns:
          number: If width is not specified, the object's current print width
          number: If width is specified, the object's previous print width
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = rhino_object.Attributes.PlotWeight
            if width is not None:
                rhino_object.Attributes.PlotWeightSource = Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
                rhino_object.Attributes.PlotWeight = width
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotWeightSource = Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
            rhino_object.Attributes.PlotWeight = width
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Modifies the print width of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="width">(float)New print width value in millimeters, where width=0 means use
    ///  the default width, and width<0 means do not print (visible for screen display,
    ///  but does not show on print). If omitted, the current width is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintWidth(objectIds:Guid, width:float seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintWidth(object_ids, width=None):
        '''Returns or modifies the print width of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          width (number, optional): new print width value in millimeters, where width=0 means use
            the default width, and width<0 means do not print (visible for screen display,
            but does not show on print). If omitted, the current width is returned.
        Returns:
          number: If width is not specified, the object's current print width
          number: If width is specified, the object's previous print width
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = rhino_object.Attributes.PlotWeight
            if width is not None:
                rhino_object.Attributes.PlotWeightSource = Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
                rhino_object.Attributes.PlotWeight = width
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotWeightSource = Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
            rhino_object.Attributes.PlotWeight = width
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Returns the print width source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(int) The object's current print width source
    ///  0 = print width by layer
    ///  1 = print width by object
    ///  3 = print width by parent</returns>
    static member ObjectPrintWidthSource(objectIds:Guid) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintWidthSource(object_ids, source=None):
        '''Returns or modifies the print width source of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          source (number, optional): new print width source
            0 = print width by layer
            1 = print width by object
            3 = print width by parent
        Returns:
          number: If source is not specified, the object's current print width source
          number: If source is specified, the object's previous print width source
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhino_object.Attributes.PlotWeightSource)
            if source is not None:
                rhino_object.Attributes.PlotWeightSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotWeightSource, source)
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotWeightSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotWeightSource, source)
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)

    ///<summary>Modifies the print width source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="source">(int)New print width source
    ///  0 = print width by layer
    ///  1 = print width by object
    ///  3 = print width by parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintWidthSource(objectIds:Guid, source:int) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintWidthSource(object_ids, source=None):
        '''Returns or modifies the print width source of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          source (number, optional): new print width source
            0 = print width by layer
            1 = print width by object
            3 = print width by parent
        Returns:
          number: If source is not specified, the object's current print width source
          number: If source is specified, the object's previous print width source
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhino_object.Attributes.PlotWeightSource)
            if source is not None:
                rhino_object.Attributes.PlotWeightSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotWeightSource, source)
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotWeightSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotWeightSource, source)
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Modifies the print width source of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="source">(int)New print width source
    ///  0 = print width by layer
    ///  1 = print width by object
    ///  3 = print width by parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintWidthSource(objectIds:Guid, source:int seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectPrintWidthSource(object_ids, source=None):
        '''Returns or modifies the print width source of an object
        Parameters:
          object_ids ([guid, ...]): identifiers of object(s)
          source (number, optional): new print width source
            0 = print width by layer
            1 = print width by object
            3 = print width by parent
        Returns:
          number: If source is not specified, the object's current print width source
          number: If source is specified, the object's previous print width source
          number: If object_ids is a list or tuple, the number of objects modified
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rc = int(rhino_object.Attributes.PlotWeightSource)
            if source is not None:
                rhino_object.Attributes.PlotWeightSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotWeightSource, source)
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        for id in object_ids:
            rhino_object = rhutil.coercerhinoobject(id, True, True)
            rhino_object.Attributes.PlotWeightSource = System.Enum.ToObject(Rhino.DocObjects.ObjectPlotWeightSource, source)
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Returns the object type</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(int) The object type .
    ///  The valid object types are as follows:
    ///  Value   Description
    ///    0           Unknown object
    ///    1           Point
    ///    2           Point cloud
    ///    4           Curve
    ///    8           Surface or single-face brep
    ///    16          Polysurface or multiple-face
    ///    32          Mesh
    ///    256         Light
    ///    512         Annotation
    ///    4096        Instance or block reference
    ///    8192        Text dot object
    ///    16384       Grip object
    ///    32768       Detail
    ///    65536       Hatch
    ///    131072      Morph control
    ///    134217728   Cage
    ///    268435456   Phantom
    ///    536870912   Clipping plane
    ///    1073741824  Extrusion</returns>
    static member ObjectType(objectId:Guid) : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def ObjectType(object_id):
        '''Returns the object type
        Parameters:
          object_id (guid): identifier of an object
        Returns:
          number: The object type if successful.
            The valid object types are as follows:
            Value   Description
              0           Unknown object
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
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        geom = rhobj.Geometry
        if isinstance(geom, Rhino.Geometry.Brep) and geom.Faces.Count==1:
            return 8 #surface
        return int(geom.ObjectType)
    *)


    ///<summary>Orients a single object based on input points.
    ///  If two 3-D points are specified, then this method will function similar to Rhino's Orient command.  If more than two 3-D points are specified, then the function will orient similar to Rhino's Orient3Pt command.
    ///  The orient flags values can be added together to specify multiple options.
    ///    Value   Description
    ///    1       Copy object.  The default is not to copy the object.
    ///    2       Scale object.  The default is not to scale the object.  Note, the scale option only applies if both reference and target contain only two 3-D points.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="reference">(Point3d seq) List of 3-D reference points.</param>
    ///<param name="target">(Point3d seq) List of 3-D target points</param>
    ///<param name="flags">(int) Optional, Default Value: <c>0</c>
    ///1 = copy object
    ///  2 = scale object
    ///  3 = copy and scale</param>
    ///<returns>(Guid) The identifier of the oriented object .</returns>
    static member OrientObject(objectId:Guid, reference:Point3d seq, target:Point3d seq, [<OPT;DEF(0)>]flags:int) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def OrientObject(object_id, reference, target, flags=0):
        '''Orients a single object based on input points.  
        If two 3-D points are specified, then this method will function similar to Rhino's Orient command.  If more than two 3-D points are specified, then the function will orient similar to Rhino's Orient3Pt command.
        The orient flags values can be added together to specify multiple options.
            Value   Description
            1       Copy object.  The default is not to copy the object.
            2       Scale object.  The default is not to scale the object.  Note, the scale option only applies if both reference and target contain only two 3-D points.
        Parameters:
            object_id (guid): The identifier of an object
            reference ([point, point, ...]): list of 3-D reference points.
            target  ([point, point, ...]): list of 3-D target points
            flags (number):  1 = copy object
                             2 = scale object
                             3 = copy and scale
        Returns:
          guid: The identifier of the oriented object if successful.
        '''
        object_id = rhutil.coerceguid(object_id, True)
        from_array = rhutil.coerce3dpointlist(reference)
        to_array = rhutil.coerce3dpointlist(target)
        if from_array is None or to_array is None:
            raise ValueError("Could not convert reference or target to point list")
        from_count = len(from_array)
        to_count = len(to_array)
        if from_count<2 or to_count<2: raise Exception("point lists must have at least 2 values")
        copy = ((flags & 1) == 1)
        scale = ((flags & 2) == 2)
        xform_final = None
        if from_count>2 and to_count>2:
            #Orient3Pt
            from_plane = Rhino.Geometry.Plane(from_array[0], from_array[1], from_array[2])
            to_plane = Rhino.Geometry.Plane(to_array[0], to_array[1], to_array[2])
            if not from_plane.IsValid or not to_plane.IsValid:
                raise Exception("unable to create valid planes from point lists")
            xform_final = Rhino.Geometry.Transform.PlaneToPlane(from_plane, to_plane)
        else:
            #Orient2Pt
            xform_move = Rhino.Geometry.Transform.Translation( to_array[0]-from_array[0] )
            xform_scale = Rhino.Geometry.Transform.Identity
            v0 = from_array[1] - from_array[0]
            v1 = to_array[1] - to_array[0]
            if scale:
                len0 = v0.Length
                len1 = v1.Length
                if len0<0.000001 or len1<0.000001: raise Exception("vector lengths too short")
                scale = len1 / len0
                if abs(1.0-scale)>=0.000001:
                    plane = Rhino.Geometry.Plane(from_array[0], v0)
                    xform_scale = Rhino.Geometry.Transform.Scale(plane, scale, scale, scale)
            v0.Unitize()
            v1.Unitize()
            xform_rotate = Rhino.Geometry.Transform.Rotation(v0, v1, from_array[0])
            xform_final = xform_move * xform_scale * xform_rotate
        rc = scriptcontext.doc.Objects.Transform(object_id, xform_final, not copy)
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Rotates a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to rotate</param>
    ///<param name="centerPoint">(Point3d) The center of rotation</param>
    ///<param name="rotationAngle">(float) In degrees</param>
    ///<param name="axis">(Plane) Optional, Default Value: <c>null</c>
    ///Axis of rotation, If omitted, the Z axis of the active
    ///  construction plane is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the rotated object</returns>
    static member RotateObject(objectId:Guid, centerPoint:Point3d, rotationAngle:float, [<OPT;DEF(null)>]axis:Plane, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def RotateObject(object_id, center_point, rotation_angle, axis=None, copy=False):
        '''Rotates a single object
        Parameters:
          object_id (guid): The identifier of an object to rotate
          center_point (point): the center of rotation
          rotation_angle (number): in degrees
          axis (plane, optional): axis of rotation, If omitted, the Z axis of the active
            construction plane is used as the rotation axis
          copy (bool, optional): copy the object
        Returns:
          guid: Identifier of the rotated object if successful
          None: on error
        '''
        rc = RotateObjects(object_id, center_point, rotation_angle, axis, copy)
        if rc: return rc[0]
        return scriptcontext.errorhandler()
    *)


    ///<summary>Rotates multiple objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to rotate</param>
    ///<param name="centerPoint">(Point3d) The center of rotation</param>
    ///<param name="rotationAngle">(float) In degrees</param>
    ///<param name="axis">(Plane) Optional, Default Value: <c>null</c>
    ///Axis of rotation, If omitted, the Z axis of the active
    ///  construction plane is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid seq) identifiers of the rotated objects</returns>
    static member RotateObjects(objectIds:Guid seq, centerPoint:Point3d, rotationAngle:float, [<OPT;DEF(null)>]axis:Plane, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def RotateObjects( object_ids, center_point, rotation_angle, axis=None, copy=False):
        '''Rotates multiple objects
        Parameters:
          object_ids ([guid, ...]): Identifiers of objects to rotate
          center_point (point): the center of rotation
          rotation_angle (number): in degrees
          axis (plane, optional): axis of rotation, If omitted, the Z axis of the active
            construction plane is used as the rotation axis
          copy (bool, optional): copy the object
        Returns:
          list(guid, ...): identifiers of the rotated objects if successful
        '''
        center_point = rhutil.coerce3dpoint(center_point, True)
        if not axis:
            axis = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane().Normal
        axis = rhutil.coerce3dvector(axis, True)
        rotation_angle = Rhino.RhinoMath.ToRadians(rotation_angle)
        xf = Rhino.Geometry.Transform.Rotation(rotation_angle, axis, center_point)
        rc = TransformObjects(object_ids, xf, copy)
        return rc
    *)


    ///<summary>Scales a single object. Can be used to perform a uniform or non-uniform
    ///  scale transformation. Scaling is based on the active construction plane.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the scaled object</returns>
    static member ScaleObject(objectId:Guid, origin:Point3d, scale:float*float*float, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def ScaleObject(object_id, origin, scale, copy=False):
        '''Scales a single object. Can be used to perform a uniform or non-uniform
        scale transformation. Scaling is based on the active construction plane.
        Parameters:
          object_id (guid): The identifier of an object
          origin (point): the origin of the scale transformation
          scale ([number, number, number]): three numbers that identify the X, Y, and Z axis scale factors to apply
          copy (bool, optional): copy the object
        Returns:
          guid: Identifier of the scaled object if successful
          None: on error
        '''
        rc = ScaleObjects(object_id, origin, scale, copy )
        if rc: return rc[0]
        return scriptcontext.errorhandler()
    *)


    ///<summary>Scales one or more objects. Can be used to perform a uniform or non-
    ///  uniform scale transformation. Scaling is based on the active construction plane.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to scale</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid seq) identifiers of the scaled objects</returns>
    static member ScaleObjects(objectIds:Guid seq, origin:Point3d, scale:float*float*float, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ScaleObjects(object_ids, origin, scale, copy=False):
        '''Scales one or more objects. Can be used to perform a uniform or non-
        uniform scale transformation. Scaling is based on the active construction plane.
        Parameters:
          object_ids ([guid, ...]): Identifiers of objects to scale
          origin (point): the origin of the scale transformation
          scale ([number, number, number]): three numbers that identify the X, Y, and Z axis scale factors to apply
          copy (bool, optional): copy the objects
        Returns:
          list(guid, ...): identifiers of the scaled objects if successful
          None: on error
        '''
        origin = rhutil.coerce3dpoint(origin, True)
        scale = rhutil.coerce3dpoint(scale, True)
        plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
        plane.Origin = origin
        xf = Rhino.Geometry.Transform.Scale(plane, scale.X, scale.Y, scale.Z)
        rc = TransformObjects(object_ids, xf, copy)
        return rc
    *)


    ///<summary>Selects a single object</summary>
    ///<param name="objectId">(Guid) The identifier of the object to select</param>
    ///<param name="redraw">(bool) Optional, Default Value: <c>true</c>
    ///Redraw view too</param>
    ///<returns>(bool) True on success</returns>
    static member SelectObject(objectId:Guid, [<OPT;DEF(true)>]redraw:bool) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def SelectObject(object_id, redraw=True):
        '''Selects a single object
        Parameters:
          object_id (guid): the identifier of the object to select
          redraw (bool, optional): redraw view too
        Returns:
          bool: True on success
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        rhobj.Select(True)
        if redraw: scriptcontext.doc.Views.Redraw()
        return True
    *)


    ///<summary>Selects one or more objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of the objects to select</param>
    ///<returns>(float) number of selected objects</returns>
    static member SelectObjects(objectIds:Guid seq) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SelectObjects( object_ids):
        '''Selects one or more objects
        Parameters:
          object_ids ([guid, ...]): identifiers of the objects to select
        Returns:
          number: number of selected objects
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        rc = 0
        for id in object_ids:
            if SelectObject(id, False)==True: rc += 1
        if rc > 0: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Perform a shear transformation on a single object</summary>
    ///<param name="objectId">(Guid seq) The identifier of an object</param>
    ///<param name="origin">(Point3d) Origin point of the shear transformation</param>
    ///<param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    ///<param name="angleDegrees">(int) The shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid) Identifier of the sheared object</returns>
    static member ShearObject(objectId:Guid seq, origin:Point3d, referencePoint:Point3d, angleDegrees:int, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def ShearObject(object_id, origin, reference_point, angle_degrees, copy=False):
        '''Perform a shear transformation on a single object
        Parameters:
          object_id (guid, ...): The identifier of an object
          origin (point): origin point of the shear transformation
          reference_point (point): reference point of the shear transformation
          angle_degrees (number): the shear angle in degrees
          copy (bool, optional): copy the objects
        Returns:
          guid: Identifier of the sheared object if successful
          None: on error
        '''
        rc = ShearObjects(object_id, origin, reference_point, angle_degrees, copy)
        if rc: return rc[0]
    *)


    ///<summary>Shears one or more objects</summary>
    ///<param name="objectIds">(Guid seq) The identifiers objects to shear</param>
    ///<param name="origin">(Point3d) Origin point of the shear transformation</param>
    ///<param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    ///<param name="angleDegrees">(int) The shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid seq) identifiers of the sheared objects</returns>
    static member ShearObjects(objectIds:Guid seq, origin:Point3d, referencePoint:Point3d, angleDegrees:int, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ShearObjects(object_ids, origin, reference_point, angle_degrees, copy=False):
        '''Shears one or more objects
        Parameters:
          object_ids ([guid, ...]): The identifiers objects to shear
          origin (point): origin point of the shear transformation
          reference_point (point): reference point of the shear transformation
          angle_degrees (number): the shear angle in degrees
          copy (bool, optional): copy the objects
        Returns:
          list(guid, ...]): identifiers of the sheared objects if successful
        '''
        origin = rhutil.coerce3dpoint(origin, True)
        reference_point = rhutil.coerce3dpoint(reference_point, True)
        if (origin-reference_point).IsTiny(): return None
        plane = scriptcontext.doc.Views.ActiveView.MainViewport.ConstructionPlane()
        frame = Rhino.Geometry.Plane(plane)
        frame.Origin = origin
        frame.ZAxis = plane.Normal
        yaxis = reference_point-origin
        yaxis.Unitize()
        frame.YAxis = yaxis
        xaxis = Rhino.Geometry.Vector3d.CrossProduct(frame.ZAxis, frame.YAxis)
        xaxis.Unitize()
        frame.XAxis = xaxis
        world_plane = Rhino.Geometry.Plane.WorldXY
        cob = Rhino.Geometry.Transform.ChangeBasis(world_plane, frame)
        shear2d = Rhino.Geometry.Transform.Identity
        shear2d[0,1] = math.tan(math.radians(angle_degrees))
        cobinv = Rhino.Geometry.Transform.ChangeBasis(frame, world_plane)
        xf = cobinv * shear2d * cob
        rc = TransformObjects(object_ids, xf, copy)
        return rc
    *)


    ///<summary>Shows a previously hidden object. Hidden objects are not visible, cannot
    ///  be snapped to and cannot be selected</summary>
    ///<param name="objectId">(Guid) Representing id of object to show</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member ShowObject(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def ShowObject(object_id):
        '''Shows a previously hidden object. Hidden objects are not visible, cannot
        be snapped to and cannot be selected
        Parameters:
          object_id (guid): representing id of object to show
        Returns:
          bool: True of False indicating success or failure
        '''
        return ShowObjects(object_id)==1
    *)


    ///<summary>Shows one or more objects. Hidden objects are not visible, cannot be
    ///  snapped to and cannot be selected</summary>
    ///<param name="objectIds">(Guid seq) Ids of objects to show</param>
    ///<returns>(float) Number of objects shown</returns>
    static member ShowObjects(objectIds:Guid seq) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def ShowObjects(object_ids):
        '''Shows one or more objects. Hidden objects are not visible, cannot be
        snapped to and cannot be selected
        Parameters:
          object_ids ([guid, ...]): ids of objects to show
        Returns:
          number: Number of objects shown
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        rc = 0
        for id in object_ids:
            id = rhutil.coerceguid(id, True)
            if scriptcontext.doc.Objects.Show(id, False): rc += 1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Moves, scales, or rotates an object given a 4x4 transformation matrix.
    ///  The matrix acts on the left.</summary>
    ///<param name="objectId">(Guid) The identifier of the object.</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers).</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object.</param>
    ///<returns>(Guid) The identifier of the transformed object</returns>
    static member TransformObject(objectId:Guid, matrix:Transform, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def TransformObject(object_id, matrix, copy=False):
        '''Moves, scales, or rotates an object given a 4x4 transformation matrix.
        The matrix acts on the left.
        Parameters:
          object_id (guid): The identifier of the object.
          matrix (transform): The transformation matrix (4x4 array of numbers).
          copy (bool, optional): Copy the object.
        Returns:
          guid: The identifier of the transformed object
          None: if not successful, or on error
        '''
        rc = TransformObjects(object_id, matrix, copy)
        if rc: return rc[0]
        return scriptcontext.errorhandler()
    __allowed_transform_types = [
        Rhino.Geometry.Point3d, Rhino.Geometry.Line, Rhino.Geometry.Rectangle3d,
        Rhino.Geometry.Circle, Rhino.Geometry.Ellipse, Rhino.Geometry.Arc,
        Rhino.Geometry.Polyline, Rhino.Geometry.Box, Rhino.Geometry.Sphere]
    # this is also called by Copy, Scale, Mirror, Move, and Rotate functions defined above
    *)


    ///<summary>Moves, scales, or rotates a list of objects given a 4x4 transformation
    ///  matrix. The matrix acts on the left.</summary>
    ///<param name="objectIds">(Guid seq) List of object identifiers.</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers).</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid seq) ids identifying the newly transformed objects</returns>
    static member TransformObjects(objectIds:Guid seq, matrix:Transform, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def TransformObjects(object_ids, matrix, copy=False):
        '''Moves, scales, or rotates a list of objects given a 4x4 transformation
        matrix. The matrix acts on the left.
        Parameters:
          object_ids ([guid, ...]): List of object identifiers.
          matrix (transform): The transformation matrix (4x4 array of numbers).
          copy (bool, optional): Copy the objects
        Returns:
          list(guid, ...): ids identifying the newly transformed objects
        '''
        xform = rhutil.coercexform(matrix, True)
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        elif isinstance(object_ids, Rhino.Geometry.GeometryBase): object_ids = [object_ids]
        elif type(object_ids) in __allowed_transform_types: object_ids = [object_ids]
        rc = []
        for object_id in object_ids:
            id = System.Guid.Empty
            old_id = rhutil.coerceguid(object_id, False)
            if old_id:
                id = scriptcontext.doc.Objects.Transform(old_id, xform, not copy)
            elif isinstance(object_id, Rhino.Geometry.GeometryBase):
                if copy: object_id = object_id.Duplicate()
                if not object_id.Transform(xform): raise Exception("Cannot apply transform to geometry.")
                id = scriptcontext.doc.Objects.Add(object_id)
            else:
                type_of_id = type(object_id)
                if type_of_id in __allowed_transform_types:
                    if copy: object_id = System.ICloneable.Clone(object_id)
                    if object_id.Transform(xform) == False: #some of the Transform methods return bools, others have no return
                        raise Exception("Cannot apply transform to geometry.")
                    ot = scriptcontext.doc.Objects
                    fs = [ot.AddPoint,ot.AddLine,ot.AddRectangle,ot.AddCircle,ot.AddEllipse,ot.AddArc,ot.AddPolyline,ot.AddBox,ot.AddSphere]
                    t_index = __allowed_transform_types.index(type_of_id)
                    id = fs[t_index](object_id)
                else:
                    raise Exception("The {0} cannot be tranformed. A Guid or geometry types are expected.".format(type_of_id))
            if id!=System.Guid.Empty: rc.append(id)
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Unlocks an object. Locked objects are visible, and can be snapped to,
    ///  but they cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member UnlockObject(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def UnlockObject(object_id):
        '''Unlocks an object. Locked objects are visible, and can be snapped to,
        but they cannot be selected.
        Parameters:
          object_id (guid): The identifier of an object
        Returns:
          bool: True or False indicating success or failure
        '''
        return UnlockObjects(object_id)==1
    *)


    ///<summary>Unlocks one or more objects. Locked objects are visible, and can be
    ///  snapped to, but they cannot be selected.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of objects</param>
    ///<returns>(float) number of objects unlocked</returns>
    static member UnlockObjects(objectIds:Guid seq) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def UnlockObjects(object_ids):
        '''Unlocks one or more objects. Locked objects are visible, and can be
        snapped to, but they cannot be selected.
        Parameters:
          object_ids ([guid, ...]): The identifiers of objects
        Returns:
          number: number of objects unlocked
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        rc = 0
        for id in object_ids:
            id = rhutil.coerceguid(id, True)
            if scriptcontext.doc.Objects.Unlock(id, False): rc += 1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Unselects a single selected object</summary>
    ///<param name="objectId">(Guid) Id of object to unselect</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member UnselectObject(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def UnselectObject(object_id):
        '''Unselects a single selected object
        Parameters:
          object_id: (guid): id of object to unselect
        Returns:
          bool: True of False indicating success or failure
        '''
        return UnselectObjects(object_id)==1
    *)


    ///<summary>Unselects one or more selected objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of the objects to unselect.</param>
    ///<returns>(float) The number of objects unselected</returns>
    static member UnselectObjects(objectIds:Guid seq) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def UnselectObjects(object_ids):
        '''Unselects one or more selected objects.
        Parameters:
          object_ids ([guid, ...]): identifiers of the objects to unselect.
        Returns:
          number: The number of objects unselected
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        count = len(object_ids)
        for id in object_ids:
            obj = rhutil.coercerhinoobject(id, True, True)
            obj.Select(False)
        if count: scriptcontext.doc.Views.Redraw()
        return count
    *)


