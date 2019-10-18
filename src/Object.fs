namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
open System.Collections.Generic

[<AutoOpen>]
module ExtensionsObject =
  

  type RhinoScriptSyntax with

    
    [<EXT>]
    ///<summary>Moves, scales, or rotates a list of objects given a 4x4 transformation
    ///  matrix. The matrix acts on the left. To transfrom Geometry objects instead of DocObjects or Guids use their .Transform(xform) member.</summary>
    ///<param name="objectIds">(Guid seq) List of object identifiers.</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers).</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid ResizeArray) ids identifying the newly transformed objects</returns>
    static member TransformObjects( objectIds:Guid seq, 
                                    matrix:Transform, 
                                    [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =      // this is also called by Copy, Scale, Mirror, Move, and Rotate functions defined below 
        let rc = ResizeArray()
        for objectid in objectIds do
            let id = Doc.Objects.Transform(objectid, matrix, not copy)
            if id = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply transform to object '%A' from objectIds:'%A' matrix:'%A' copy:'%A'" objectid objectIds matrix copy
            rc.Add id
        Doc.Views.Redraw()
        rc          
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
  
            xform = rhutil.coercexform(matrix)
            id = rhutil.coerceguid(object_ids, False)
            if id: object_ids = [id]
            elif isinstance(object_ids;GeometryBase): object_ids = [object_ids]
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

    [<EXT>]
    ///<summary>Moves, scales, or rotates an object given a 4x4 transformation matrix.
    ///  The matrix acts on the left.  To transfrom Geometry objects instead of DocObjects or Guids use their .Transform(xform) member.</summary>
    ///<param name="objectId">(Guid) The identifier of the object.</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers).</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object.</param>
    ///<returns>(Guid) The identifier of the transformed object</returns>
    static member TransformObject(  objectId:Guid, 
                                    matrix:Transform, 
                                    [<OPT;DEF(false)>]copy:bool) : Guid =
        let id = Doc.Objects.Transform(objectId, matrix, not copy)
        if id = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply transform to object '%A'  matrix:'%A' copy:'%A'" objectId  matrix copy
        id        

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
    
    [<EXT>]
    ///<summary>Copies object from one location to another, or in-place.</summary>
    ///<param name="objectId">(Guid) Object to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value: <c>Vector3d()</c>
    ///Translation vector to apply</param>
    ///<returns>(Guid) id for the copy</returns>
    static member CopyObject(objectId:Guid, [<OPT;DEF(Vector3d())>]translation:Vector3d) : Guid =
        let translation = 
            if not translation.IsZero then
                //translation = RhinoScriptSyntax.Coerce3dvector(translation)
                Transform.Translation(translation)
            else 
                Transform.Identity
        RhinoScriptSyntax.TransformObject(objectId, translation)
        
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


    [<EXT>]
    ///<summary>Copies one or more objects from one location to another, or in-place.</summary>
    ///<param name="objectIds">(Guid seq) List of objects to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value: <c>Vector3d()</c>
    ///List of three numbers or Vector3d representing
    ///  translation vector to apply to copied set</param>
    ///<returns>(Guid ResizeArray) identifiers for the copies</returns>
    static member CopyObjects(objectIds:Guid seq, [<OPT;DEF(Vector3d())>]translation:Vector3d) : Guid ResizeArray =
        objectIds
        |> Seq.map  RhinoScriptSyntax.CopyObject 
        |> ResizeArray.ofSeq
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
            translation = rhutil.coerce3dvector(translation)
            translation = Rhino.Geometry.Transform.Translation(translation)
        else:
            translation = Rhino.Geometry.Transform.Identity
        return TransformObjects(object_ids, translation)
    *)


    [<EXT>]
    ///<summary>Deletes a single object from the document</summary>
    ///<param name="objectId">(Guid) Identifier of object to delete</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member DeleteObject(objectId:Guid) : bool =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        let rc = Doc.Objects.Delete(objectId)
        if rc then Doc.Views.Redraw()
        rc
    (*
    def DeleteObject(object_id):
        '''Deletes a single object from the document
        Parameters:
          object_id (guid): identifier of object to delete
        Returns:
          bool: True of False indicating success or failure
        '''
    
        object_id = rhutil.coerceguid(object_id)
        rc = scriptcontext.doc.Objects.Delete(object_id)
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Deletes one or more objects from the document</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to delete</param>
    ///<returns>(int) Number of objects deleted</returns>
    static member DeleteObjects(objectIds:Guid seq) : int =
        let mutable rc = 0
        //id = RhinoScriptSyntax.Coerceguid(objectIds)
        for id in objectIds do
            //id = RhinoScriptSyntax.Coerceguid(id)
            if Doc.Objects.Delete(id) then rc <- rc + 1
        if rc > 0 then Doc.Views.Redraw()
        rc
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
            id = rhutil.coerceguid(id)
            if scriptcontext.doc.Objects.Delete(id): rc+=1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Causes the selection state of one or more objects to change momentarily
    ///  so the object appears to flash on the screen</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to flash</param>
    ///<param name="style">(bool) Optional, Default Value: <c>true</c>
    ///If True, flash between object color and selection color.
    ///  If False, flash between visible and invisible</param>
    ///<returns>(unit) </returns>
    static member FlashObject(objectIds:Guid seq, [<OPT;DEF(true)>]style:bool) : unit =
        //id = RhinoScriptSyntax.Coerceguid(objectIds)
        //if notNull id then objectIds <- .[id]
        let rhobjs = resizeArray { for id in objectIds do yield RhinoScriptSyntax.CoerceRhinoObject(id) }
        if rhobjs.Count>0 then 
            Doc.Views.FlashObjects(rhobjs, style)
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
        rhobjs = [rhutil.coercerhinoobject(id, True) for id in object_ids]
        if rhobjs: scriptcontext.doc.Views.FlashObjects(rhobjs, style)
    *)


    [<EXT>]
    ///<summary>Hides a single object</summary>
    ///<param name="objectId">(Guid) Id of object to hide</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member HideObject(objectId:Guid) : bool =
        Doc.Objects.Hide(objectId, false) 
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


    [<EXT>]
    ///<summary>Hides one or more objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to hide</param>
    ///<returns>(int) Number of objects hidden</returns>
    static member HideObjects(objectIds:Guid seq) : int =
        //id = RhinoScriptSyntax.Coerceguid(objectIds)
        //if notNull id then objectIds <- .[id]
        let mutable rc = 0
        for id in objectIds do
            //id = RhinoScriptSyntax.Coerceguid(id)
            if Doc.Objects.Hide(id, false) then rc <- rc + 1
        if  rc>0 then Doc.Views.Redraw()
        rc
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
            id = rhutil.coerceguid(id)
            if scriptcontext.doc.Objects.Hide(id, False): rc += 1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Verifies that an object is in either page layout space or model space</summary>
    ///<param name="objectId">(Guid) Id of an object to test</param>
    ///<returns>(bool) True if the object is in page layout space, False if the object is in model space</returns>
    static member IsLayoutObject(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.Space = Rhino.DocObjects.ActiveSpace.PageSpace
    (*
    def IsLayoutObject(object_id):
        '''Verifies that an object is in either page layout space or model space
        Parameters:
          object_id (guid): id of an object to test
        Returns:
          bool: True if the object is in page layout space, False if the object is in model space
        '''
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        return rhobj.Attributes.Space == Rhino.DocObjects.ActiveSpace.PageSpace
    *)


    [<EXT>]
    ///<summary>Verifies the existence of an object</summary>
    ///<param name="objectId">(Guid) An object to test</param>
    ///<returns>(bool) True if the object exists, False if the object does not exist</returns>
    static member IsObject(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceRhinoObject(objectId) <> None
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


    [<EXT>]
    ///<summary>Verifies that an object is hidden. Hidden objects are not visible, cannot
    ///  be snapped to, and cannot be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is hidden, False if the object is not hidden</returns>
    static member IsObjectHidden(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsHidden
    (*
    def IsObjectHidden(object_id):
        '''Verifies that an object is hidden. Hidden objects are not visible, cannot
        be snapped to, and cannot be selected
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True if the object is hidden, False if the object is not hidden
        '''
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        return rhobj.IsHidden
    *)


    [<EXT>]
    ///<summary>Verifies an object's bounding box is inside of another bounding box</summary>
    ///<param name="objectId">(Guid) Identifier of an object to be tested</param>
    ///<param name="box">(Geometry.BoundingBox) Bounding box to test for containment</param>
    ///<param name="testMode">(bool) Optional, Default Value: <c>true</c>
    ///If True, the object's bounding box must be contained by box
    ///  If False, the object's bounding box must be contained by or intersect box</param>
    ///<returns>(bool) True if object is inside box, False is object is not inside box</returns>
    static member IsObjectInBox( objectId:Guid, 
                                 box:BoundingBox, 
                                 [<OPT;DEF(true)>]testMode:bool) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)        
        let objbox = rhobj.Geometry.GetBoundingBox(true)
        if testMode then 
          box.Contains(objbox)
        else
          let union = BoundingBox.Intersection(box, objbox)
          union.IsValid
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
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        box = rhutil.coerceboundingbox(box)
        objbox = rhobj.Geometry.GetBoundingBox(True)
        if test_mode: return box.Contains(objbox)
        union = Rhino.Geometry.BoundingBox.Intersection(box, objbox)
        return union.IsValid
    *)


    [<EXT>]
    //(FIXME) VarOutTypes
    ///<summary>Verifies that an object is a member of a group</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="groupName">(string) Optional, The name of a group. If omitted, the function
    ///  verifies that the object is a member of any group</param>
    ///<returns>(bool) True if the object is a member of the specified group. If a group_name
    ///  was not specified, the object is a member of some group.
    ///  False if the object  is not a member of the specified group.
    ///  If a group_name was not specified, the object is not a member of any group</returns>
    static member IsObjectInGroup(objectId:Guid, [<OPT;DEF(null:string)>]groupName:string) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let count = rhobj.GroupCount
        if count<1 then false
        else
          if isNull groupName then true
          else
            let index = Doc.Groups.Find(groupName)
            if index<0 then failwith "%s group does not exist" groupName
            let groupids = rhobj.GetGroupList()
            groupids |> Seq.exists ((=) index )
            
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
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
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


    [<EXT>]
    ///<summary>Verifies that an object is locked. Locked objects are visible, and can
    ///  be snapped to, but cannot be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is locked, False if the object is not locked</returns>
    static member IsObjectLocked(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsLocked
    (*
    def IsObjectLocked(object_id):
        '''Verifies that an object is locked. Locked objects are visible, and can
        be snapped to, but cannot be selected
        Parameters:
          object_id (guid): The identifier of an object to be tested
        Returns:
          bool: True if the object is locked, False if the object is not locked
        '''
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        return rhobj.IsLocked
    *)


    [<EXT>]
    ///<summary>Verifies that an object is normal. Normal objects are visible, can be
    ///  snapped to, and can be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is normal, False if the object is not normal</returns>
    static member IsObjectNormal(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsNormal
    (*
    def IsObjectNormal(object_id):
        '''Verifies that an object is normal. Normal objects are visible, can be
        snapped to, and can be selected
        Parameters:
          object_id (guid): The identifier of an object to be tested
        Returns:
          bool: True if the object is normal, False if the object is not normal
        '''
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        return rhobj.IsNormal
    *)


    [<EXT>]
    ///<summary>Verifies that an object is a reference object. Reference objects are
    ///  objects that are not part of the current document</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is a reference object, False if the object is not a reference object</returns>
    static member IsObjectReference(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsReference
    (*
    def IsObjectReference(object_id):
        '''Verifies that an object is a reference object. Reference objects are
        objects that are not part of the current document
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True if the object is a reference object, False if the object is not a reference object
        '''
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        return rhobj.IsReference
    *)


    [<EXT>]
    ///<summary>Verifies that an object can be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True or False</returns>
    static member IsObjectSelectable(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsSelectable(true,false,false,false)
    (*
    def IsObjectSelectable(object_id):
        '''Verifies that an object can be selected
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True or False
        '''
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        return rhobj.IsSelectable(True,False,False,False)
    *)


    [<EXT>]
    ///<summary>Verifies that an object is currently selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(int) 0, the object is not selected
    ///  1, the object is selected
    ///  2, the object is entirely persistently selected
    ///  3, one or more proper sub-objects are selected</returns>
    static member IsObjectSelected(objectId:Guid) : int =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsSelected(false)
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
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        return rhobj.IsSelected(False)
    *)


    [<EXT>]
    ///<summary>Determines if an object is closed, solid</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is solid, or a mesh is closed., False otherwise.</returns>
    static member IsObjectSolid(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let geom = rhobj.Geometry
        match geom with
        | :? Mesh      as m -> m.IsClosed
        | :? Extrusion as s -> s.IsSolid
        | :? Surface   as s -> s.IsSolid              
        | :? Brep      as s -> s.IsSolid
        | _                 -> false
    (*
    def IsObjectSolid(object_id):
        '''Determines if an object is closed, solid
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True if the object is solid, or a mesh is closed., False otherwise.
        '''
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
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


    [<EXT>]
    ///<summary>Verifies an object's geometry is valid and without error</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is valid</returns>
    static member IsObjectValid(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceRhinoObject(objectId) with
        |None -> false
        |Some rhobj ->  rhobj.IsValid
    (*
    def IsObjectValid(object_id):
        '''Verifies an object's geometry is valid and without error
        Parameters:
          object_id (guid): The identifier of an object to test
        Returns:
          bool: True if the object is valid
        '''
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        return rhobj.IsValid
    *)


    [<EXT>]
    ///<summary>Verifies an object is visible in a view</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<param name="view">(string) Optional, Default Value: The title of the view.  If omitted, the current active view is used.</param>
    ///<returns>(bool) True if the object is visible in the specified view, otherwise False.</returns>
    static member IsVisibleInView(objectId:Guid, [<OPT;DEF(null:string)>]view:string) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let viewport = if notNull view then (RhinoScriptSyntax.CoerceView(view)).MainViewport else Doc.Views.ActiveView.MainViewport        
        let bbox = rhobj.Geometry.GetBoundingBox(true)
        rhobj.Visible && viewport.IsVisible(bbox)
    (*
    def IsVisibleInView(object_id, view=None):
        '''Verifies an object is visible in a view
        Parameters:
          object_id (guid): the identifier of an object to test
          view (str, optional): he title of the view.  If omitted, the current active view is used.
        Returns:
          bool: True if the object is visible in the specified view, otherwise False.  None on error
        '''
    
        rhobj = rhutil.coercerhinoobject(object_id, True)
        viewport = __viewhelper(view).MainViewport
        bbox = rhobj.Geometry.GetBoundingBox(True)
        return rhobj.Visible and viewport.IsVisible(bbox)
    *)


