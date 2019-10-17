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
  
            xform = rhutil.coercexform(matrix, True)
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
        RhinoScriptSyntax.TransformObject(objectId, translation, true)
        
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
            translation = rhutil.coerce3dvector(translation, True)
            translation = Rhino.Geometry.Transform.Translation(translation)
        else:
            translation = Rhino.Geometry.Transform.Identity
        return TransformObjects(object_ids, translation, True)
    *)


    [<EXT>]
    ///<summary>Deletes a single object from the document</summary>
    ///<param name="objectId">(Guid) Identifier of object to delete</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member DeleteObject(objectId:Guid) : bool =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        let rc = Doc.Objects.Delete(objectId, true)
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
    
        object_id = rhutil.coerceguid(object_id, True)
        rc = scriptcontext.doc.Objects.Delete(object_id, True)
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
            if Doc.Objects.Delete(id, true) then rc <- rc + 1
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
            id = rhutil.coerceguid(id, True)
            if scriptcontext.doc.Objects.Delete(id, True): rc+=1
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
        rhobjs = [rhutil.coercerhinoobject(id, True, True) for id in object_ids]
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
            id = rhutil.coerceguid(id, True)
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
    
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
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


