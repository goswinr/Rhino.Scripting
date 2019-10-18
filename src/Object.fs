namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.Compare
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
open System.Collections.Generic

[<AutoOpen>]
module ExtensionsObject =
  

  type RhinoScriptSyntax with

    
    [<EXT>]
    ///<summary>Moves, scales, or rotates a list of objects given a 4x4 transformation
    ///  matrix. The matrix acts on the left. To transfrom Geometry objects instead of DocObjects or Guids use their .Transform(xform) member.</summary>
    ///<param name="objectId">(Guid seq) List of object identifiers.</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers).</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid ResizeArray) ids identifying the newly transformed objects</returns>
    static member TransformObjects( objectId:Guid seq, 
                                    matrix:Transform, 
                                    [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =      // this is also called by Copy, Scale, Mirror, Move, and Rotate functions defined below 
        let rc = ResizeArray()
        for objectid in objectId do
            let objectId = Doc.Objects.Transform(objectid, matrix, not copy)
            if objectId = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply transform to object '%A' from objectId:'%A' matrix:'%A' copy:'%A'" objectid objectId matrix copy
            rc.Add objectId
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
            objectId = rhutil.coerceguid(object_ids, False)
            if objectId: object_ids = [objectId]
            elif isinstance(object_ids;GeometryBase): object_ids = [object_ids]
            elif type(object_ids) in __allowed_transform_types: object_ids = [object_ids]
            rc = []
            for object_id in object_ids:
                objectId = System.Guid.Empty
                old_id = rhutil.coerceguid(object_id, False)
                if old_id:
                    objectId = scriptcontext.doc.Objects.Transform(old_id, xform, not copy)
                elif isinstance(object_id, Rhino.Geometry.GeometryBase):
                    if copy: object_id = object_id.Duplicate()
                    if not object_id.Transform(xform): raise Exception("Cannot apply transform to geometry.")
                    objectId = scriptcontext.doc.Objects.Add(object_id)
                else:
                    type_of_id = type(object_id)
                    if type_of_id in __allowed_transform_types:
                        if copy: object_id = System.ICloneable.Clone(object_id)
                        if object_id.Transform(xform) == False: #some of the Transform methods return bools, others have no return
                            raise Exception("Cannot apply transform to geometry.")
                        ot = scriptcontext.doc.Objects
                        fs = [ot.AddPoint,ot.AddLine,ot.AddRectangle,ot.AddCircle,ot.AddEllipse,ot.AddArc,ot.AddPolyline,ot.AddBox,ot.AddSphere]
                        t_index = __allowed_transform_types.index(type_of_id)
                        objectId = fs[t_index](object_id)
                    else:
                        raise Exception("The {0} cannot be tranformed. A Guid or geometry types are expected.".format(type_of_id))
                if objectId!=System.Guid.Empty: rc.append(objectId)
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
        let objectId = Doc.Objects.Transform(objectId, matrix, not copy)
        if objectId = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply transform to object '%A'  matrix:'%A' copy:'%A'" objectId  matrix copy
        objectId        

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
    ///<returns>(Guid) objectId for the copy</returns>
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
          guid: objectId for the copy if successful
          None: if not able to copy
        '''
    
        rc = CopyObjects(object_id, translation)
        if rc: return rc[0]
    *)


    [<EXT>]
    ///<summary>Copies one or more objects from one location to another, or in-place.</summary>
    ///<param name="objectId">(Guid seq) List of objects to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value: <c>Vector3d()</c>
    ///List of three numbers or Vector3d representing
    ///  translation vector to apply to copied set</param>
    ///<returns>(Guid ResizeArray) identifiers for the copies</returns>
    static member CopyObjects(objectId:Guid seq, [<OPT;DEF(Vector3d())>]translation:Vector3d) : Guid ResizeArray =
        objectId
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
    ///<returns>(unit) void, nothing</returns>
    static member DeleteObject(objectId:Guid) : unit =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        if not <| Doc.Objects.Delete(objectId,true)  then failwithf "DeleteObject failed on %A" objectId
        Doc.Views.Redraw()
        
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
    ///<summary>Deletes one or more objects from the document, Fails if not all objects can be deleted</summary>
    ///<param name="objectId">(Guid seq) Identifiers of objects to delete</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteObjects(objectId:Guid seq) : unit =
        let k = Doc.Objects.Delete(objectId,true) 
        let l = Seq.length objectId
        if k <> l then failwithf "DeleteObjects failed on %d out of %A" (l-k) objectId
        Doc.Views.Redraw()
        //let mutable rc = 0
        ////objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //for objectId in objectId do
        //    //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //    if Doc.Objects.Delete(objectId) then rc <- rc + 1
        //if rc > 0 then Doc.Views.Redraw()
        //rc
    (*
    def DeleteObjects(object_ids):
        '''Deletes one or more objects from the document
        Parameters:
          object_ids ([guid, ...]): identifiers of objects to delete
        Returns:
          number: Number of objects deleted
        '''
    
        rc = 0
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId: object_ids = [objectId]
        for objectId in object_ids:
            objectId = rhutil.coerceguid(objectId)
            if scriptcontext.doc.Objects.Delete(objectId): rc+=1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Causes the selection state of one or more objects to change momentarily
    ///  so the object appears to flash on the screen</summary>
    ///<param name="objectId">(Guid seq) Identifiers of objects to flash</param>
    ///<param name="style">(bool) Optional, Default Value: <c>true</c>
    ///If True, flash between object color and selection color.
    ///  If False, flash between visible and invisible</param>
    ///<returns>(unit) </returns>
    static member FlashObject(objectId:Guid seq, [<OPT;DEF(true)>]style:bool) : unit =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //if notNull objectId then objectId <- .[objectId]
        let rhobjs = resizeArray { for objectId in objectId do yield RhinoScriptSyntax.CoerceRhinoObject(objectId) }
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
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId: object_ids = [objectId]
        rhobjs = [rhutil.coercerhinoobject(objectId, True) for objectId in object_ids]
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
          object_id (guid): objectId of object to hide
        Returns:
          bool: True of False indicating success or failure
        '''
    
        return HideObjects(object_id)==1
    *)


    [<EXT>]
    ///<summary>Hides one or more objects</summary>
    ///<param name="objectId">(Guid seq) Identifiers of objects to hide</param>
    ///<returns>(int) Number of objects hidden</returns>
    static member HideObjects(objectId:Guid seq) : int =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //if notNull objectId then objectId <- .[objectId]
        let mutable rc = 0
        for objectId in objectId do
            //objectId = RhinoScriptSyntax.Coerceguid(objectId)
            if Doc.Objects.Hide(objectId, false) then rc <- rc + 1
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
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId: object_ids = [objectId]
        rc = 0
        for objectId in object_ids:
            objectId = rhutil.coerceguid(objectId)
            if scriptcontext.doc.Objects.Hide(objectId, False): rc += 1
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
          object_id (guid): objectId of an object to test
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
            if index<0 then failwithf "%s group does not exist" groupName
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
        for objectId in group_ids:
            if objectId==index: return True
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


    [<EXT>]
    ///<summary>Locks a single object. Locked objects are visible, and they can be
    ///  snapped to. But, they cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member LockObject(objectId:Guid) : bool =
        Doc.Objects.Lock(objectId, false)
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


    [<EXT>]
    ///<summary>Locks one or more objects. Locked objects are visible, and they can be
    ///  snapped to. But, they cannot be selected.</summary>
    ///<param name="objectId">(Guid seq) List of Strings or Guids. The identifiers of objects</param>
    ///<returns>(int) number of objects locked</returns>
    static member LockObjects(objectId:Guid seq) : int =
        let mutable rc = 0
        for objectId in objectId do
            if Doc.Objects.Lock(objectId, false) then rc <- rc +   1
        if 0<> rc then Doc.Views.Redraw()
        rc
    (*
    def LockObjects(object_ids):
        '''Locks one or more objects. Locked objects are visible, and they can be
        snapped to. But, they cannot be selected.
        Parameters:
          object_ids ([guid, ...]): list of Strings or Guids. The identifiers of objects
        Returns:
          number: number of objects locked
        '''
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId: object_ids = [objectId]
        rc = 0
        for objectId in object_ids:
            objectId = rhutil.coerceguid(objectId, True)
            if scriptcontext.doc.Objects.Lock(objectId, False): rc += 1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Matches, or copies the attributes of a source object to a target object</summary>
    ///<param name="targetIds">(Guid seq) Identifiers of objects to copy attributes to</param>
    ///<param name="sourceId">(Guid) Optional, Default Value: <c>Guid()</c>
    ///Identifier of object to copy attributes from. If None,
    ///  then the default attributes are copied to the targetIds</param>
    ///<returns>(int) number of objects modified</returns>
    static member MatchObjectAttributes(targetIds:Guid seq, [<OPT;DEF(Guid())>]sourceId:Guid) : int =      
        let sourceattr = 
            if Guid.Empty <> sourceId then
                let source = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
                source.Attributes.Duplicate()
            else
                new Rhino.DocObjects.ObjectAttributes()
        let mutable rc = 0
        for objectId in targetIds do
            if Doc.Objects.ModifyAttributes(objectId, sourceattr, true) then
                rc <- rc +  1
        if 0 <> rc then Doc.Views.Redraw()
        rc
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
    
        objectId = rhutil.coerceguid(target_ids, False)
        if objectId: target_ids = [objectId]
        source_attr = Rhino.DocObjects.ObjectAttributes()
        if source_id:
            source = rhutil.coercerhinoobject(source_id, True, True)
            source_attr = source.Attributes.Duplicate()
        rc = 0
        for objectId in target_ids:
            objectId = rhutil.coerceguid(objectId, True)
            if scriptcontext.doc.Objects.ModifyAttributes(objectId, source_attr, True):
                rc += 1
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Mirrors a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to mirror</param>
    ///<param name="startPoint">(Point3d) Start of the mirror plane</param>
    ///<param name="endPoint">(Point3d) End of the mirror plane</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the mirrored object</returns>
    static member MirrorObject( objectId:Guid, 
                                startPoint:Point3d, 
                                endPoint:Point3d, 
                                [<OPT;DEF(false)>]copy:bool) : Guid =
        let vec = endPoint-startPoint
        if vec.IsTiny() then failwithf "Rhino.Scripting: Start and  end points are too close to each other.  objectId:'%A' startPoint:'%A' endPoint:'%A' copy:'%A'" objectId startPoint endPoint copy
        let normal = Doc.Views.ActiveView.ActiveViewport.ConstructionPlane().Normal
        let xv = Vector3d.CrossProduct(vec, normal)
        xv.Unitize() |> ignore
        let xf = Transform.Mirror(startPoint, vec)
        RhinoScriptSyntax.TransformObject(objectId, xf, copy)
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


    [<EXT>]
    ///<summary>Mirrors a list of objects</summary>
    ///<param name="objectId">(Guid seq) Identifiers of objects to mirror</param>
    ///<param name="startPoint">(Point3d) Start of the mirror plane</param>
    ///<param name="endPoint">(Point3d) End of the mirror plane</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid ResizeArray) List of identifiers of the mirrored objects</returns>
    static member MirrorObjects( objectId:Guid seq, 
                                 startPoint:Point3d, 
                                 endPoint:Point3d, 
                                 [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =
        let vec = endPoint-startPoint
        if vec.IsTiny() then failwithf "Rhino.Scripting: Start and  end points are too close to each other.  objectId:'%A' startPoint:'%A' endPoint:'%A' copy:'%A'" objectId startPoint endPoint copy
        let normal = Doc.Views.ActiveView.ActiveViewport.ConstructionPlane().Normal
        let xv = Vector3d.CrossProduct(vec, normal)
        xv.Unitize() |> ignore
        let xf = Transform.Mirror(startPoint, vec)
        RhinoScriptSyntax.TransformObjects(objectId, xf, copy)
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


    [<EXT>]
    ///<summary>Moves a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(Guid) Identifier of the moved object</returns>
    static member MoveObject(objectId:Guid, translation:Vector3d) : Guid =
        let xf = Transform.Translation(translation)
        let rc = RhinoScriptSyntax.TransformObject(objectId, xf)
        rc
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


    [<EXT>]
    ///<summary>Moves one or more objects</summary>
    ///<param name="objectId">(Guid seq) The identifiers objects to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(Guid ResizeArray) identifiers of the moved objects</returns>
    static member MoveObjects(objectId:Guid seq, translation:Vector3d) : Guid ResizeArray =
        //translation = RhinoScriptSyntax.Coerce3dvector(translation)
        let xf = Transform.Translation(translation)
        let rc = RhinoScriptSyntax.TransformObjects(objectId, xf)
        rc
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


    [<EXT>]
    ///<summary>Returns the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectId">(Guid) Id object</param>
    ///<returns>(Drawing.Color) The current color value</returns>
    static member ObjectColor(objectId:Guid) : Drawing.Color = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.DrawColor(Doc)
    (*
    def ObjectColor(object_ids, color=None):
        '''Returns or modifies the color of an object. Object colors are represented
        as RGB colors. An RGB color specifies the relative intensity of red, green,
        and blue to cause a specific color to be displayed
        Parameters:
            object_ids ([guid, ...]): objectId or ids of object
            color (color, optional): the new color value. If omitted, then current object
                color is returned. If object_ids is a list, color is required
        Returns:
            color: If color value is not specified, the current color value
            color: If color value is specified, the previous color value
            number: If object_ids is a list, then the number of objects modified
        '''
    
        objectId = rhutil.coerceguid(object_ids, False)
        rhino_object = None
        rhino_objects = None
        if objectId:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
        else:
            rhino_objects = [rhutil.coercerhinoobject(objectId, True, True) for objectId in object_ids]
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

    [<EXT>]
    ///<summary>Modifies the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectId">(Guid) Id or ids of object</param>
    ///<param name="color">(Drawing.Color)The new color value.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let attr = rhobj.Attributes
        attr.ObjectColor <- color
        attr.ColorSource <- Rhino.DocObjects.ObjectColorSource.ColorFromObject
        if not <| Doc.Objects.ModifyAttributes( rhobj, attr, true) then failwithf "set ObjectColor faile for %A %A" objectId color
        Doc.Views.Redraw()
    (*
    def ObjectColor(object_ids, color=None):
        '''Returns or modifies the color of an object. Object colors are represented
        as RGB colors. An RGB color specifies the relative intensity of red, green,
        and blue to cause a specific color to be displayed
        Parameters:
            object_ids ([guid, ...]): objectId or ids of object
            color (color, optional): the new color value. If omitted, then current object
                color is returned. If object_ids is a list, color is required
        Returns:
            color: If color value is not specified, the current color value
            color: If color value is specified, the previous color value
            number: If object_ids is a list, then the number of objects modified
        '''
    
        objectId = rhutil.coerceguid(object_ids, False)
        rhino_object = None
        rhino_objects = None
        if objectId:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
        else:
            rhino_objects = [rhutil.coercerhinoobject(objectId, True, True) for objectId in object_ids]
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



    [<EXT>]
    ///<summary>Returns the color source of an object.</summary>
    ///<param name="objectId">(Guid) Single identifier of list of identifiers</param>
    ///<returns>(int) The current color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</returns>
    static member ObjectColorSource(objectId:Guid) : int = //GET        
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId, true)
        int(rhobj.Attributes.ColorSource)
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
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId:
            rhobj = rhutil.coercerhinoobject(objectId, True, True)
            rc = int(rhobj.Attributes.ColorSource)
            if source is not None:
                rhobj.Attributes.ColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        else:
            rc = 0
            source = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
            for objectId in object_ids:
                rhobj = rhutil.coercerhinoobject(objectId, True, True)
                rhobj.Attributes.ColorSource = source
                rhobj.CommitChanges()
                rc += 1
            if rc: scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the color source of an object.</summary>
    ///<param name="objectId">(Guid) Single identifier of list of identifiers</param>
    ///<param name="source">(int)New color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectColorSource(objectId:Guid, source:int) : unit = //SET
        let source : Rhino.DocObjects.ObjectColorSource = LanguagePrimitives.EnumOfValue source
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId, true)
        rhobj.Attributes.ColorSource <- source
        if not <| rhobj.CommitChanges() then failwithf "Set ObjectColorSource failed for '%A' and '%A'" objectId source
        Doc.Views.Redraw()
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
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId:
            rhobj = rhutil.coercerhinoobject(objectId, True, True)
            rc = int(rhobj.Attributes.ColorSource)
            if source is not None:
                rhobj.Attributes.ColorSource = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
        else:
            rc = 0
            source = System.Enum.ToObject(Rhino.DocObjects.ObjectColorSource, source)
            for objectId in object_ids:
                rhobj = rhutil.coercerhinoobject(objectId, True, True)
                rhobj.Attributes.ColorSource = source
                rhobj.CommitChanges()
                rc += 1
            if rc: scriptcontext.doc.Views.Redraw()
            return rc
    *)




    [<EXT>]
    ///<summary>Returns a short text description of an object</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(string) A short text description of the object .</returns>
    static member ObjectDescription(objectId:Guid) : string =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId, true)
        rhobj.ShortDescription(false)
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


    [<EXT>]
    ///<summary>Returns all of the group names that an object is assigned to</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(string ResizeArray) list of group names on success</returns>
    static member ObjectGroups(objectId:Guid) : string ResizeArray =
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if rhinoobject.GroupCount<1 then resizeArray { () }
        else
            let groupindices = rhinoobject.GetGroupList()
            resizeArray { for index in groupindices do yield Doc.Groups.GroupName(index) }
            
    (*
    def ObjectGroups(object_id):
        '''Returns all of the group names that an object is assigned to
        Parameters:
          object_id ([guid, ...]): identifier of an object
        Returns:
          list(str, ...): list of group names on success
        '''
    
        rhino_object = rhutil.coercerhinoobject(object_id, True, True)
        if rhino_object.GroupCount<1: return []
        group_indices = rhino_object.GetGroupList()
        rc = [scriptcontext.doc.Groups.GroupName(index) for index in group_indices]
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the layer of an object</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<returns>(string) The object's current layer</returns>
    static member ObjectLayer(objectId:Guid) : string = //GET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = obj.Attributes.LayerIndex
        Doc.Layers.[index].FullPath
    (*
    def ObjectLayer(object_id, layer=None):
        '''Returns or modifies the layer of an object
        Parameters:
          object_id ([guid, ...]) the identifier of the object
          layer (str, optional):  name of an existing layer
        Returns:
          str: If a layer is not specified, the object's current layer
          str: If a layer is specified, the object's previous layer
          number: If object_id is a list or tuple, the number of objects modified
        '''
    
        if type(object_id) is not str and hasattr(object_id, "__len__"):
            layer = __getlayer(layer, True)
            index = layer.LayerIndex
            for objectId in object_id:
                obj = rhutil.coercerhinoobject(objectId, True, True)
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
    ///<param name="objectId">(Guid) The identifier of the objects</param>
    ///<param name="layer">(string)Name of an existing layer</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLayer(objectId:Guid, layer:string) : unit = //SET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let index = layer.Index
        obj.Attributes.LayerIndex <- index
        if not <| obj.CommitChanges() then failwithf "Set ObjectLayer failed for '%A' and '%A'"  layer objectId
        Doc.Views.Redraw()
    (*
    def ObjectLayer(object_id, layer=None):
        '''Returns or modifies the layer of an object
        Parameters:
          object_id ([guid, ...]) the identifier of the object
          layer (str, optional):  name of an existing layer
        Returns:
          str: If a layer is not specified, the object's current layer
          str: If a layer is specified, the object's previous layer
          number: If object_id is a list or tuple, the number of objects modified
        '''
    
        if type(object_id) is not str and hasattr(object_id, "__len__"):
            layer = __getlayer(layer, True)
            index = layer.LayerIndex
            for objectId in object_id:
                obj = rhutil.coercerhinoobject(objectId, True, True)
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

    ///<summary>Modifies the layer of an objects</summary>
    ///<param name="objectId">(Guid seq) The identifiers of the objects</param>
    ///<param name="layer">(string)Name of an existing layer</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectsLayer(objectId:Guid seq, layer:string) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let index = layer.Index
        for objectId in objectId do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)        
            obj.Attributes.LayerIndex <- index
            if not <| obj.CommitChanges() then failwithf "Set ObjectLayer failed for '%A' and '%A'"  layer objectId
        Doc.Views.Redraw()


    [<EXT>]
    ///<summary>Returns the layout or model space of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string ) The object's current page layout view, "" empty string if in Model Space</returns>
    static member ObjectLayout(objectId:Guid) : string = //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if rhobj.Attributes.Space = Rhino.DocObjects.ActiveSpace.PageSpace then
            let pageid = rhobj.Attributes.ViewportId
            let pageview = Doc.Views.Find(pageid)
            pageview.MainViewport.Name
        else 
            ""


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
    ///  from page layout space to model space, just specify Empty String "" </param>   
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLayout(objectId:Guid, layout:string, [<OPT;DEF(true)>]returnName:bool) : unit = //SET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let view=
            if rhobj.Attributes.Space = Rhino.DocObjects.ActiveSpace.PageSpace then
                let pageid = rhobj.Attributes.ViewportId
                let pageview = Doc.Views.Find(pageid)
                pageview.MainViewport.Name
            else
                ""
        if view<>layout then
            if layout = "" then //move to model space
                rhobj.Attributes.Space <- Rhino.DocObjects.ActiveSpace.ModelSpace
                rhobj.Attributes.ViewportId <- Guid.Empty
            else
                match Doc.Views.Find(layout, false) with
                | null -> failwithf "Set ObjectLayout failed, layout not found for '%A' and '%A'"  layout objectId
                | :? Rhino.Display.RhinoPageView as layout ->
                    rhobj.Attributes.ViewportId <- layout.MainViewport.Id
                    rhobj.Attributes.Space <- Rhino.DocObjects.ActiveSpace.PageSpace
                | _ -> failwithf "Set ObjectLayout failed, layout is not a Page view for '%A' and '%A'"  layout objectId
                    
            
            if not <| rhobj.CommitChanges() then failwithf "Set ObjectLayout failed for '%A' and '%A'"  layout objectId
            Doc.Views.Redraw()
   
    [<EXT>]
    ///<summary>Returns the linetype of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object(s)</param>
    ///<returns>(string) The object's current linetype</returns>
    static member ObjectLinetype(objectId:Guid) : string = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldindex = Doc.Linetypes.LinetypeIndexForObject(rhinoobject)
        Doc.Linetypes.[oldindex].Name
          
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
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
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
        for objectId in object_ids:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
            rhino_object.Attributes.LinetypeSource = Rhino.DocObjects.ObjectLinetypeSource.LinetypeFromObject
            rhino_object.Attributes.LinetypeIndex = newindex
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)

    ///<summary>Modifies the linetype of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object(s)</param>
    ///<param name="linetype">(string)Name of an existing linetyp. If omitted, the current
    ///  linetyp is returned. If objectId is a list of identifiers, this parameter
    ///  is required</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLinetype(objectId:Guid, linetype:string) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldindex = Doc.Linetypes.LinetypeIndexForObject(rhinoobject)        
        let newindex = Doc.Linetypes.Find(linetype)
        if newindex <0 then failwithf "Set ObjectLinetype failed for '%A' and '%A'"  linetype objectId
        rhinoobject.Attributes.LinetypeSource <- Rhino.DocObjects.ObjectLinetypeSource.LinetypeFromObject
        rhinoobject.Attributes.LinetypeIndex <- newindex
        if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectLinetype failed for '%A' and '%A'"  linetype objectId
        Doc.Views.Redraw()


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
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
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
        for objectId in object_ids:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
            rhino_object.Attributes.LinetypeSource = Rhino.DocObjects.ObjectLinetypeSource.LinetypeFromObject
            rhino_object.Attributes.LinetypeIndex = newindex
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    [<EXT>]
    ///<summary>Returns the linetype source of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object(s)</param>
    ///<returns>(int) The object's current linetype source
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</returns>
    static member ObjectLinetypeSource(objectId:Guid) : int = //GET  
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldsource = rhinoobject.Attributes.LinetypeSource
        int(oldsource)
      
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
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
            oldsource = rhino_object.Attributes.LinetypeSource
            if source is not None:
                source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
                rhino_object.Attributes.LinetypeSource = source
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return int(oldsource)
        source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
        for objectId in object_ids:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
            rhino_object.Attributes.LinetypeSource = source
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)

    ///<summary>Modifies the linetype source of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object(s)</param>
    ///<param name="source">(int)New linetype source. If omitted, the current source is returned.
    ///  If objectId is a list of identifiers, this parameter is required
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLinetypeSource(objectId:Guid, source:int) : unit = //SET        
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldsource = rhinoobject.Attributes.LinetypeSource
        let source:  Rhino.DocObjects.ObjectLinetypeSource = LanguagePrimitives.EnumOfValue  source
        rhinoobject.Attributes.LinetypeSource <- source
        if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectLinetypeSource failed for '%A' and '%A'"  source objectId

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
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
            oldsource = rhino_object.Attributes.LinetypeSource
            if source is not None:
                source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
                rhino_object.Attributes.LinetypeSource = source
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return int(oldsource)
        source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
        for objectId in object_ids:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
            rhino_object.Attributes.LinetypeSource = source
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    ///<summary>Modifies the linetype source of an object</summary>
    ///<param name="objectId">(Guid seq) Identifiers of object(s)</param>
    ///<param name="source">(int)New linetype source. If omitted, the current source is returned.
    ///  If objectId is a list of identifiers, this parameter is required
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLinetypeSource(objectId:Guid, source:int) : unit = //SET        
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)                      
        if source <0 || source >3 || source=2 then failwithf "Set ObjectLinetypeSource failed for '%A' and '%A'"  source objectId
        let source : Rhino.DocObjects.ObjectLinetypeSource = LanguagePrimitives.EnumOfValue source
        rhinoobject.Attributes.LinetypeSource <- source
        if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectLinetypeSource failed for '%A' and '%A'"  source objectId
        Doc.Views.Redraw()
            
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
    
        objectId = rhutil.coerceguid(object_ids, False)
        if objectId:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
            oldsource = rhino_object.Attributes.LinetypeSource
            if source is not None:
                source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
                rhino_object.Attributes.LinetypeSource = source
                rhino_object.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return int(oldsource)
        source = System.Enum.ToObject(Rhino.DocObjects.ObjectLinetypeSource, source)
        for objectId in object_ids:
            rhino_object = rhutil.coercerhinoobject(objectId, True, True)
            rhino_object.Attributes.LinetypeSource = source
            rhino_object.CommitChanges()
        scriptcontext.doc.Views.Redraw()
        return len(object_ids)
    *)


    [<EXT>]
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
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)   
        rhinoobject.Attributes.MaterialIndex
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
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if 0 <=. materialIndex .< Doc.Materials.Count then failwithf "Set ObjectMaterialIndex failed for '%A' and '%A'"  materialIndex objectId
        let attrs = rhinoobject.Attributes
        attrs.MaterialIndex <- materialIndex
        if not <| Doc.Objects.ModifyAttributes(rhinoobject, attrs, true) then failwithf "Set ObjectMaterialIndex failed for '%A' and '%A'"  materialIndex objectId
        
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

    [<EXT>]
    ///<summary>Returns the rendering material source of an object.</summary>
    ///<param name="objectId">(Guid) One or more object identifiers</param>
    ///<returns>(int) The current rendering material source
    ///  0 = Material from layer
    ///  1 = Material from object
    ///  3 = Material from parent</returns>
    static member ObjectMaterialSource(objectId:Guid) : int = //GET
        //id = RhinoScriptSyntax.Coerceguid(objectId)        
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        int(rhinoobject.Attributes.MaterialSource)
        
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
    ///<param name="objectId">(Guid) One or more object identifiers</param>
    ///<param name="source">(int)The new rendering material source. If omitted and a single
    ///  object is provided in objectId, then the current material source is
    ///  returned. This parameter is required if multiple objects are passed in
    ///  objectId
    ///  0 = Material from layer
    ///  1 = Material from object
    ///  3 = Material from parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectMaterialSource(objectId:Guid, source:int) : unit = //SET       
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let rc = int(rhinoobject.Attributes.MaterialSource)
        if source <0 || source >3 || source=2 then failwithf "Set ObjectMaterialSource failed for '%A' and '%A'"  source objectId
        let source :Rhino.DocObjects.ObjectMaterialSource  = LanguagePrimitives.EnumOfValue  source  
        rhinoobject.Attributes.MaterialSource <- source
        if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectMaterialSource failed for '%A' and '%A'"  source objectId
        
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



    [<EXT>]
    ///<summary>Returns the name of an object</summary>
    ///<param name="objectId">(Guid) Id or ids of object(s)</param>
    ///<returns>(string) The current object name</returns>
    static member ObjectName(objectId:Guid) : string = //GET 
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.Name
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
        //id = RhinoScriptSyntax.Coerceguid(objectId)
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId) 
        rhinoobject.Attributes.Name <- name        
        if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectName failed for '%A' and '%A'"  name objectId
           
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

    [<EXT>]
    ///<summary>Returns the print color of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object(s)</param>
    ///<returns>(Drawing.Color) The object's current print color</returns>
    static member ObjectPrintColor(objectId:Guid) : Drawing.Color = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.PlotColor
            
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
    ///<param name="objectId">(Guid seq) Identifiers of object(s)</param>
    ///<param name="color">(Drawing.Color)New print color. If omitted, the current color is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintColor(objectId:Guid, color:Drawing.Color) : unit = //SET        
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)            
        rhinoobject.Attributes.PlotColorSource <- Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject
        rhinoobject.Attributes.PlotColor <- color
        if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectPrintColor failed for '%A' and '%A'"  color objectId
        Doc.Views.Redraw()
         
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
    
    [<EXT>]
    ///<summary>Returns the print color source of an object</summary>
    ///<param name="objectId">(Guid seq) Identifiers of object(s)</param>
    ///<returns>(int) The object's current print color source
    ///  0 = print color by layer
    ///  1 = print color by object
    ///  3 = print color by parent</returns>
    static member ObjectPrintColorSource(objectId:Guid) : int = //GET       
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            int(rhinoobject.Attributes.PlotColorSource)
            
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
    static member ObjectPrintColorSource(objectId:Guid, source:int) : unit = //SET
        if source <0 || source >3 || source=2 then failwithf "Set ObjectPrintColorSource failed for '%A' and '%A'"  source objectId
        let source : Rhino.DocObjects.ObjectPlotColorSource = LanguagePrimitives.EnumOfValue source
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId, true)
        rhobj.Attributes.PlotColorSource <- source
        if not <| rhobj.CommitChanges() then failwithf "Set ObjectPrintColorSource failed for '%A' and '%A'" objectId source
        Doc.Views.Redraw()

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




    [<EXT>]
    ///<summary>Returns the print width of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object(s)</param>
    ///<returns>(float) The object's current print width</returns>
    static member ObjectPrintWidth(objectId:Guid) : float = //GET
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoobject.Attributes.PlotWeight
            
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
    ///  but does not show on print). </param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintWidth(objectId:Guid, width:float) : unit = //SET
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let rc = rhinoobject.Attributes.PlotWeight            
            rhinoobject.Attributes.PlotWeightSource <- Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
            rhinoobject.Attributes.PlotWeight <- width
            if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectPrintWidth failed for '%A' and '%A'"  width objectId
            Doc.Views.Redraw()
           
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
