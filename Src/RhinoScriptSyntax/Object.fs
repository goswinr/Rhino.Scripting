namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.UtilMath
open FsEx.CompareOperators

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

 
[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsObject =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with


    ///<summary>Moves, scales, or rotates a list of objects given a 4x4 transformation
    ///    matrix. The matrix acts on the left. To transform Geometry objects instead of DocObjects or Guids use their .Transform(xForm) member.</summary>
    ///<param name="objectIds">(Guid seq) List of object identifiers</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers)</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///    Copy the objects</param>
    ///<returns>(Guid Rarr) ids identifying the newly transformed objects.</returns>
    [<Extension>]
    static member TransformObject( objectIds:Guid seq,
                                    matrix:Transform,
                                    [<OPT;DEF(false)>]copy:bool) : Guid Rarr =   //PLURAL    
        // this is also called by Copy, Scale, Mirror, Move, and Rotate functions defined below
        let rc = Rarr()
        for objId in objectIds do
            let objectId = Doc.Objects.Transform(objId, matrix, not copy)
            if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Cannot apply transform to object '%s' from objectId:'%s' matrix:'%A' copy:'%A'" (rhType objId) (rhType objectId) matrix copy
            rc.Add objectId
        Doc.Views.Redraw()
        rc

    ///<summary>Moves, scales, or rotates an object given a 4x4 transformation matrix.
    ///    The matrix acts on the left. To transform Geometry objects instead of DocObjects or Guids use their .Transform(xForm) member.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers)</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///    Copy the object</param>
    ///<returns>(Guid) The identifier of the transformed object.</returns>
    [<Extension>]
    static member TransformObject(  objectId:Guid,
                                    matrix:Transform,
                                    [<OPT;DEF(false)>]copy:bool) : Guid =
        let res = Doc.Objects.Transform(objectId, matrix, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Cannot apply transform to objectId:'%s' matrix:'%A' copy:'%A'"  (rhType objectId) matrix copy
        res


    ///<summary>Copies object from one location to another, or in-place.</summary>
    ///<param name="objectId">(Guid) Object to copy</param>
    ///<param name="translation">(Vector3d) Optional, additional Translation vector to apply</param>
    ///<returns>(Guid) objectId for the copy.</returns>
    [<Extension>]
    static member CopyObject(objectId:Guid, [<OPT;DEF(Vector3d())>]translation:Vector3d) : Guid =
        let translation =
            if not translation.IsZero then
                Transform.Translation(translation)
            else
                Transform.Identity
        let res = Doc.Objects.Transform(objectId, translation, false)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CopyObject failed.  objectId:'%s' translation:'%A'" (rhType objectId) translation
        res



    ///<summary>Copies one or more objects from one location to another, or in-place.</summary>
    ///<param name="objectIds">(Guid seq) List of objects to copy</param>
    ///<param name="translation">(Vector3d) Optional, Vector3d representing translation vector to apply to copied set</param>
    ///<returns>(Guid Rarr) identifiers for the copies.</returns>
    [<Extension>]
    static member CopyObject(objectIds:Guid seq, [<OPT;DEF(Vector3d())>]translation:Vector3d) : Guid Rarr = //PLURAL
        let translation =
            if not translation.IsZero then
                Transform.Translation(translation)
            else
                Transform.Identity
        let rc = Rarr()
        for objectId in objectIds do
            let res = Doc.Objects.Transform(objectId, translation, false)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CopyObjectc failed.  objectId:'%s' translation:'%A'" (rhType objectId) translation
            rc.Add res
        rc


    ///<summary>Deletes a single object from the document.</summary>
    ///<param name="objectId">(Guid) Identifier of object to delete</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member DeleteObject(objectId:Guid) : unit =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        if not <| Doc.Objects.Delete(objectId, true)  then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteObject failed on %s" (rhType objectId)
        Doc.Views.Redraw()



    ///<summary>Deletes one or more objects from the document, Fails if not all objects can be deleted.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to delete</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member DeleteObject(objectIds:Guid seq) : unit = //PLURAL
        let k = Doc.Objects.Delete(objectIds, true)
        let l = Seq.length objectIds
        if k <> l then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteObjects failed on %d out of %s" (l-k) (RhinoScriptSyntax.ToNiceString objectIds)
        Doc.Views.Redraw()
        

    ///<summary>Causes the selection state of one or more objects to change momentarily
    ///    so the object appears to flash on the screen.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to flash</param>
    ///<param name="style">(bool) Optional, Default Value: <c>true</c>
    ///    If True, flash between object color and selection color.
    ///    If False, flash between visible and invisible</param>
    ///<returns>(unit).</returns>
    [<Extension>]
    static member FlashObject(objectIds:Guid seq, [<OPT;DEF(true)>]style:bool) : unit =
        let rhobjs = rarr { for objectId in objectIds do yield RhinoScriptSyntax.CoerceRhinoObject(objectId) }
        if rhobjs.Count>0 then
            Doc.Views.FlashObjects(rhobjs, style)


    ///<summary>Hides a single object.</summary>
    ///<param name="objectId">(Guid) Id of object to hide</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    [<Extension>]
    static member HideObject(objectId:Guid) : bool =
        Doc.Objects.Hide(objectId, false)


    ///<summary>Hides one or more objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to hide</param>
    ///<returns>(int) Number of objects hidden.</returns>
    [<Extension>]
    static member HideObject(objectIds:Guid seq) : int = //PLURAL
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //if notNull objectId then objectId <- .[objectId]
        let mutable rc = 0
        for objectId in objectIds do
            if Doc.Objects.Hide(objectId, false) then rc <- rc + 1
        if  rc>0 then Doc.Views.Redraw()
        rc


    ///<summary>Verifies that an object is in either page layout space or model space.</summary>
    ///<param name="objectId">(Guid) Id of an object to test</param>
    ///<returns>(bool) True if the object is in page layout space, False if the object is in model space.</returns>
    [<Extension>]
    static member IsLayoutObject(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.Space = DocObjects.ActiveSpace.PageSpace


    ///<summary>Verifies the existence of an objectin the Doc.Objects table. Fails on empty Guid.</summary>
    ///<param name="objectId">(Guid) An object to test</param>
    ///<returns>(bool) True if the object exists, False if the object does not exist.</returns>
    [<Extension>]
    static member IsObject(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceRhinoObject(objectId) <> None


    ///<summary>Verifies that an object is hidden. Hidden objects are not visible, cannot
    ///    be snapped to, and cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is hidden, False if the object is not hidden.</returns>
    [<Extension>]
    static member IsObjectHidden(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsHidden


    ///<summary>Verifies an object's bounding box is inside of another bounding box.</summary>
    ///<param name="objectId">(Guid) Identifier of an object to be tested</param>
    ///<param name="box">(Geometry.BoundingBox) Bounding box to test for containment</param>
    ///<param name="testMode">(bool) Optional, Default Value: <c>true</c>
    ///    If True, the object's bounding box must be contained by box
    ///    If False, the object's bounding box must be contained by or intersect box</param>
    ///<returns>(bool) True if object is inside box, False is object is not inside box.</returns>
    [<Extension>]
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


    ///<summary>Verifies that an object is a member of a group.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="groupName">(string) Optional, The name of a group. If omitted, the function
    ///    verifies that the object is a member of any group</param>
    ///<returns>(bool) True if the object is a member of the specified group. If a groupName
    ///    was not specified, the object is a member of some group.
    ///    False if the object  is not a member of the specified group.
    ///    If a groupName was not specified, the object is not a member of any group.</returns>
    [<Extension>]
    static member IsObjectInGroup(objectId:Guid, [<OPT;DEF(null:string)>]groupName:string) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let count = rhobj.GroupCount
        if count<1 then false
        else
          if isNull groupName then true
          else
            let index = Doc.Groups.Find(groupName)
            if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.%s group does not exist" groupName
            let groupids = rhobj.GetGroupList()
            groupids |> Seq.exists ((=) index )



    ///<summary>Verifies that an object is locked. Locked objects are visible, and can
    ///    be snapped to, but cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is locked, False if the object is not locked.</returns>
    [<Extension>]
    static member IsObjectLocked(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsLocked


    ///<summary>Verifies that an object is normal. Normal objects are visible, can be
    ///    snapped to, and can be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is normal, False if the object is not normal.</returns>
    [<Extension>]
    static member IsObjectNormal(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsNormal


    ///<summary>Verifies that an object is a reference object. Reference objects are
    ///    objects that are not part of the current document.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is a reference object, False if the object is not a reference object.</returns>
    [<Extension>]
    static member IsObjectReference(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsReference


    ///<summary>Verifies that an object can be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsObjectSelectable(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsSelectable(true, false, false, false)


    ///<summary>Verifies that an object is currently selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(int) 
    ///    0, the object is not selected
    ///    1, the object is selected
    ///    2, the object is entirely persistently selected
    ///    3, one or more proper sub-objects are selected.</returns>
    [<Extension>]
    static member IsObjectSelected(objectId:Guid) : int =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsSelected(false)


    ///<summary>Determines if an object is closed, solid.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is solid, or a Mesh is closed., False otherwise.</returns>
    [<Extension>]
    static member IsObjectSolid(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let geom = rhobj.Geometry
        match geom with
        | :? Mesh      as m -> m.IsClosed
        | :? Extrusion as s -> s.IsSolid
        | :? Surface   as s -> s.IsSolid
        | :? Brep      as s -> s.IsSolid
        | _                 -> false


    ///<summary>Verifies an object's geometry is valid and without error.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is valid.</returns>
    [<Extension>]
    static member IsObjectValid(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceRhinoObject(objectId) with
        |None -> false
        |Some rhobj ->  rhobj.IsValid


    ///<summary>Verifies an object is visible in a view.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<param name="view">(string) Optional, Default Value: The title of the view. If omitted, the current active view is used</param>
    ///<returns>(bool) True if the object is visible in the specified view, otherwise False.</returns>
    [<Extension>]
    static member IsVisibleInView(objectId:Guid, [<OPT;DEF(null:string)>]view:string) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let viewport = if notNull view then (RhinoScriptSyntax.CoerceView(view)).MainViewport else Doc.Views.ActiveView.MainViewport
        let bbox = rhobj.Geometry.GetBoundingBox(true)
        rhobj.Visible && viewport.IsVisible(bbox)


    ///<summary>Locks a single object. Locked objects are visible, and they can be
    ///    snapped to. But, they cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member LockObject(objectId:Guid) : bool =
        Doc.Objects.Lock(objectId, false)


    ///<summary>Locks multiple objects. Locked objects are visible, and they can be
    ///    snapped to. But, they cannot be selected.</summary>
    ///<param name="objectIds">(Guid seq) List of Strings or Guids. The identifiers of objects</param>
    ///<returns>(int) number of objects locked.</returns>
    [<Extension>]
    static member LockObject(objectIds:Guid seq) : int = //PLURAL
        let mutable rc = 0
        for objectId in objectIds do
            if Doc.Objects.Lock(objectId, false) then rc <- rc +   1
        if 0<> rc then Doc.Views.Redraw()
        rc


    ///<summary>Matches, or copies the attributes of a source object to a target object.</summary>
    ///<param name="targetIds">(Guid seq) Identifiers of objects to copy attributes to</param>
    ///<param name="sourceId">(Guid) Optional, Identifier of object to copy attributes from. If None,
    ///    then the default attributes are copied to the targetIds</param>
    ///<returns>(int) number of objects modified.</returns>
    [<Extension>]
    static member MatchObjectAttributes(targetIds:Guid seq, [<OPT;DEF(Guid())>]sourceId:Guid) : int =
        let sourceattr =
            if Guid.Empty <> sourceId then
                let source = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
                source.Attributes.Duplicate()
            else
                new DocObjects.ObjectAttributes()
        let mutable rc = 0
        for objectId in targetIds do
            if Doc.Objects.ModifyAttributes(objectId, sourceattr, true) then
                rc <- rc +  1
        if 0 <> rc then Doc.Views.Redraw()
        rc


    ///<summary>Mirrors a single object on World XY Plane.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to mirror</param>
    ///<param name="startPoint">(Point3d) Start of the mirror Plane</param>
    ///<param name="endPoint">(Point3d) End of the mirror Plane</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///    Copy the object</param>
    ///<returns>(Guid) Identifier of the mirrored object.</returns>
    [<Extension>]
    static member MirrorObject( objectId:Guid,
                                startPoint:Point3d,
                                endPoint:Point3d,
                                [<OPT;DEF(false)>]copy:bool) : Guid =
        let vec = endPoint-startPoint
        if vec.IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.Start and  end points are too close to each other.  objectId:'%s' startPoint:'%A' endPoint:'%A' copy:'%A'" (rhType objectId) startPoint endPoint copy
        let normal = Plane.WorldXY.Normal
        let xv = Vector3d.CrossProduct(vec, normal)
        xv.Unitize() |> ignore
        let xf = Transform.Mirror(startPoint, vec)
        let res = Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Cannot apply MirrorObject transform to objectId:'%s' startPoint:'%A' endPoint:'%A' copy:'%A'" (rhType objectId) startPoint endPoint copy
        res


    ///<summary>Mirrors a list of objects on World XY Plane.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to mirror</param>
    ///<param name="startPoint">(Point3d) Start of the mirror Plane</param>
    ///<param name="endPoint">(Point3d) End of the mirror Plane</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///    Copy the objects</param>
    ///<returns>(Guid Rarr) List of identifiers of the mirrored objects.</returns>
    [<Extension>]
    static member MirrorObject(  objectIds:Guid seq,
                                 startPoint:Point3d,
                                 endPoint:Point3d,
                                 [<OPT;DEF(false)>]copy:bool) : Guid Rarr = //PLURAL
        let vec = endPoint-startPoint
        if vec.IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.Start and  end points are too close to each other.  objectId:'%s' startPoint:'%A' endPoint:'%A' copy:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) startPoint endPoint copy
        let normal = Plane.WorldXY.Normal
        let xv = Vector3d.CrossProduct(vec, normal)
        xv.Unitize() |> ignore
        let xf = Transform.Mirror(startPoint, vec)
        let rc = Rarr()
        for objectId in objectIds do
            let objectId = Doc.Objects.Transform(objectId, xf, not copy)
            if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Cannot apply MirrorObjects to objectId:'%s' startPoint:'%A' endPoint:'%A' copy:'%A'" (rhType objectId) startPoint endPoint copy
            rc.Add objectId
        rc



    ///<summary>Moves a single object.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MoveObject(objectId:Guid, translation:Vector3d) : unit = //TODO or return unit ??
        let xf = Transform.Translation(translation)
        let res = Doc.Objects.Transform(objectId, xf, true)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Cannot apply move to from objectId:'%s' translation:'%A'" (rhType objectId) translation
        //if objectId <> res

    ///<summary>Moves one or more objects.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers objects to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MoveObject(objectIds:Guid seq, translation:Vector3d) : unit =  //PLURAL        
        let xf = Transform.Translation(translation)
        //let rc = Rarr()
        for objectId in objectIds do
            let res = Doc.Objects.Transform(objectId, xf, true)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Cannot apply MoveObjects Transform to objectId:'%s'  translation:'%A'" (rhType objectId) translation
            //rc.Add objectId
        //rc



    ///<summary>Returns the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed.</summary>
    ///<param name="objectId">(Guid)Id of object</param>
    ///<returns>(Drawing.Color) The current color value.</returns>
    [<Extension>]
    static member ObjectColor(objectId:Guid) : Drawing.Color = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.DrawColor(Doc)

    ///<summary>Modifies the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed.</summary>
    ///<param name="objectId">(Guid)Id of object</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let attr = rhobj.Attributes
        attr.ObjectColor <- color
        attr.ColorSource <- DocObjects.ObjectColorSource.ColorFromObject
        if not <| Doc.Objects.ModifyAttributes( rhobj, attr, true) then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectColor setting failed for %A; %A" (rhType objectId) color
        Doc.Views.Redraw()

    ///<summary>Modifies the color of multiple objects. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed.</summary>
    ///<param name="objectIds">(Guid seq)Ids of objects</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectColor(objectIds:Guid seq, color:Drawing.Color) : unit = //MULTISET
        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let attr = rhobj.Attributes
            attr.ObjectColor <- color
            attr.ColorSource <- DocObjects.ObjectColorSource.ColorFromObject
            if not <| Doc.Objects.ModifyAttributes( rhobj, attr, true) then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectColor setting failed for %A; %A" (rhType objectId) color
        Doc.Views.Redraw()
        



    ///<summary>Returns the color source of an object.</summary>
    ///<param name="objectId">(Guid) Single identifier</param>
    ///<returns>(int) The current color source
    ///    0 = color from layer
    ///    1 = color from object
    ///    2 = color from material
    ///    3 = color from parent.</returns>
    [<Extension>]
    static member ObjectColorSource(objectId:Guid) : int = //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        int(rhobj.Attributes.ColorSource)

    ///<summary>Modifies the color source of an object.</summary>
    ///<param name="objectId">(Guid) Single identifier</param>
    ///<param name="source">(int) New color source
    ///    0 = color from layer
    ///    1 = color from object
    ///    2 = color from material
    ///    3 = color from parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectColorSource(objectId:Guid, source:int) : unit = //SET
        let source : DocObjects.ObjectColorSource = LanguagePrimitives.EnumOfValue source
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.ColorSource <- source
        if not <| rhobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectColorSource failed for '%A' and '%A'" (rhType objectId) source
        Doc.Views.Redraw()
    
    ///<summary>Modifies the color source of multiple objects.</summary>
    ///<param name="objectIds">(Guid seq) Multiple identifiers</param>
    ///<param name="source">(int) New color source
    ///    0 = color from layer
    ///    1 = color from object
    ///    2 = color from material
    ///    3 = color from parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectColorSource(objectIds:Guid seq, source:int) : unit = //MULTISET
        let source : DocObjects.ObjectColorSource = LanguagePrimitives.EnumOfValue source
        for objectId in objectIds do 
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhobj.Attributes.ColorSource <- source
            if not <| rhobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectColorSource failed for '%A' and '%A'" (rhType objectId) source
        Doc.Views.Redraw()
        




    ///<summary>Returns a description of the object type (e.g. Line, Surface, Text,...).</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(string) A short text description of the object.</returns>
    [<Extension>]
    static member ObjectDescription(objectId:Guid) : string =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.ShortDescription(false)



    ///<summary>Returns the count for each object type in a List of objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects</param>
    ///<returns>(string) A short text description of the object.</returns>
    [<Extension>]
    static member ObjectDescription(objectIds:Guid seq) : string =
        let count =  Seq.countBy (fun id -> RhinoScriptSyntax.CoerceRhinoObject(id).ShortDescription(true)) objectIds        
        let typesk = Seq.length count        
        let mutable tx = ""
        if typesk = 0 then  
            tx <- "zero objects"
        elif typesk = 1  then      
            for typ, k in count  do
                tx <- sprintf " %d: %s" k typ
        else
            tx <- sprintf "%d objects of following types:" (Seq.length objectIds)
            for typ, k  in count |> Seq.sortBy snd do
                tx <- sprintf "%s%s    %d: %s" tx Environment.NewLine  k typ
        tx


    ///<summary>Returns all of the group names that an object is assigned to.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(string Rarr) list of group names.</returns>
    [<Extension>]
    static member ObjectGroups(objectId:Guid) : string Rarr =
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if rhinoobject.GroupCount<1 then rarr { () }
        else
            let groupindices = rhinoobject.GetGroupList()
            rarr { for index in groupindices do yield Doc.Groups.GroupName(index) }

    ///<summary>Returns the short layer of an object.
    ///    Without Parent Layers.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<returns>(string) The object's current layer.</returns>
    [<Extension>]
    static member ObjectLayerShort(objectId:Guid) : string = //GET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = obj.Attributes.LayerIndex
        Doc.Layers.[index].Name
    
    
    //static member ObjectLayer()// all 3 overloads moved to file RhinoScriptSyntax.fs

    ///<summary>Returns the layout or model space of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string option) The object's current page layout view, None if it is in Model Space.</returns>
    [<Extension>]
    static member ObjectLayout(objectId:Guid) : string option= //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if rhobj.Attributes.Space = DocObjects.ActiveSpace.PageSpace then
            let pageid = rhobj.Attributes.ViewportId
            let pageview = Doc.Views.Find(pageid)
            Some pageview.MainViewport.Name
        else
            None


    ///<summary>Changes the layout or model space of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="layout">(string option) To change, or move, an object from model space to page
    ///    layout space, or from one page layout to another, then specify the
    ///    title of an existing page layout view. To move an object
    ///    from page layout space to model space, just specify None</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectLayout(objectId:Guid, layout:string option) : unit = //SET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let view=
            if rhobj.Attributes.Space = DocObjects.ActiveSpace.PageSpace then
                let pageid = rhobj.Attributes.ViewportId
                let pageview = Doc.Views.Find(pageid)
                Some pageview.MainViewport.Name
            else
                None

        if view<>layout then
            if layout.IsNone then //move to model space
                rhobj.Attributes.Space <- DocObjects.ActiveSpace.ModelSpace
                rhobj.Attributes.ViewportId <- Guid.Empty
            else
                match Doc.Views.Find(layout.Value, false) with
                | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLayout failed, layout not found for '%A' and '%A'"  layout objectId
                | :? Display.RhinoPageView as layout ->
                    rhobj.Attributes.ViewportId <- layout.MainViewport.Id
                    rhobj.Attributes.Space <- DocObjects.ActiveSpace.PageSpace
                | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLayout failed, layout is not a Page view for '%A' and '%A'"  layout objectId


            if not <| rhobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLayout failed for '%A' and '%A'"  layout objectId
            Doc.Views.Redraw()

    ///<summary>Changes the layout or model space of an objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifier of the objects</param>
    ///<param name="layout">(string option) To change, or move, an objects from model space to page
    ///    layout space, or from one page layout to another, then specify the
    ///    title of an existing page layout view. To move an objects
    ///    from page layout space to model space, just specify None</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectLayout(objectIds:Guid seq, layout:string option) : unit = //MULTISET 
        let lay =
            if layout.IsSome then
                match Doc.Views.Find(layout.Value, false) with
                | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLayout failed, layout not found for '%A' and '%A'"  layout objectIds
                | :? Display.RhinoPageView as layout -> Some layout
                | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLayout failed, layout is not a Page view for '%A' and '%A'"  layout objectIds
            else
                None

        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let view=
                if rhobj.Attributes.Space = DocObjects.ActiveSpace.PageSpace then
                    let pageid = rhobj.Attributes.ViewportId
                    let pageview = Doc.Views.Find(pageid)
                    Some pageview.MainViewport.Name
                else
                    None

            if view<>layout then
                if layout.IsNone then //move to model space
                    rhobj.Attributes.Space <- DocObjects.ActiveSpace.ModelSpace
                    rhobj.Attributes.ViewportId <- Guid.Empty
                else                
                    rhobj.Attributes.ViewportId <- lay.Value.MainViewport.Id
                    rhobj.Attributes.Space <- DocObjects.ActiveSpace.PageSpace
                    


                if not <| rhobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLayout failed for '%A' and '%A'"  layout objectId
        Doc.Views.Redraw()
     

    ///<summary>Returns the linetype of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<returns>(string) The object's current linetype.</returns>
    [<Extension>]
    static member ObjectLinetype(objectId:Guid) : string = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldindex = Doc.Linetypes.LinetypeIndexForObject(rhinoobject)
        Doc.Linetypes.[oldindex].Name

    ///<summary>Modifies the linetype of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<param name="linetype">(string) Name of an existing linetyp</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectLinetype(objectId:Guid, linetype:string) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let newindex = Doc.Linetypes.Find(linetype)
        if newindex <0 then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLinetype failed for '%A' and '%A'"  linetype objectId
        rhinoobject.Attributes.LinetypeSource <- DocObjects.ObjectLinetypeSource.LinetypeFromObject
        rhinoobject.Attributes.LinetypeIndex <- newindex
        if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLinetype failed for '%A' and '%A'"  linetype objectId
        Doc.Views.Redraw()
    
    ///<summary>Modifies the linetype of multiple object.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects</param>
    ///<param name="linetype">(string) Name of an existing linetyp</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectLinetype(objectIds:Guid seq, linetype:string) : unit = //MULTISET
        let newindex = Doc.Linetypes.Find(linetype)
        if newindex <0 then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLinetype failed for '%A' and '%A'"  linetype objectIds
        for objectId in objectIds do
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoobject.Attributes.LinetypeSource <- DocObjects.ObjectLinetypeSource.LinetypeFromObject
            rhinoobject.Attributes.LinetypeIndex <- newindex
            if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLinetype failed for '%A' and '%A'"  linetype objectId
        Doc.Views.Redraw()


    ///<summary>Returns the linetype source of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<returns>(int) The object's current linetype source
    ///      0 = By Layer
    ///      1 = By Object
    ///      3 = By Parent.</returns>
    [<Extension>]
    static member ObjectLinetypeSource(objectId:Guid) : int = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldsource = rhinoobject.Attributes.LinetypeSource
        int(oldsource)

    ///<summary>Modifies the linetype source of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<param name="source">(int) New linetype source.
    ///    If objectId is a list of identifiers, this parameter is required
    ///      0 = By Layer
    ///      1 = By Object
    ///      3 = By Parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectLinetypeSource(objectId:Guid, source:int) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLinetypeSource failed for '%A' and '%A'"  source objectId
        let source : DocObjects.ObjectLinetypeSource = LanguagePrimitives.EnumOfValue source
        rhinoobject.Attributes.LinetypeSource <- source
        if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLinetypeSource failed for '%A' and '%A'"  source objectId
        Doc.Views.Redraw()

    ///<summary>Modifies the linetype source of multiple objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects</param>
    ///<param name="source">(int) New linetype source.
    ///    If objectId is a list of identifiers, this parameter is required
    ///      0 = By Layer
    ///      1 = By Object
    ///      3 = By Parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectLinetypeSource(objectIds:Guid seq, source:int) : unit = //MULTISET        
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLinetypeSource failed for '%A' and '%A'"  source objectIds
        let source : DocObjects.ObjectLinetypeSource = LanguagePrimitives.EnumOfValue source
        for objectId in objectIds do
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoobject.Attributes.LinetypeSource <- source
            if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLinetypeSource failed for '%A' and '%A'"  source objectId
        Doc.Views.Redraw()


    ///<summary>Returns the material index of an object. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(int) If the return value of ObjectMaterialSource is "material by object", then
    ///    the return value of this function is the index of the object's rendering
    ///    material. A material index of -1 indicates no material has been assigned,
    ///    and that Rhino's internal default material has been assigned to the object.</returns>
    [<Extension>]
    static member ObjectMaterialIndex(objectId:Guid) : int = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.MaterialIndex

    ///<summary>Changes the material index of an object. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<param name="materialIndex">(int) The new material index</param>
    [<Extension>]
    static member ObjectMaterialIndex(objectId:Guid, materialIndex:int) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if 0 <=. materialIndex .< Doc.Materials.Count then 
            RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectMaterialIndex failed for '%A' and '%A'"  materialIndex objectId
        let attrs = rhinoobject.Attributes
        attrs.MaterialIndex <- materialIndex
        if not <| Doc.Objects.ModifyAttributes(rhinoobject, attrs, true) then 
            RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectMaterialIndex failed for '%A' and '%A'"  materialIndex objectId

    ///<summary>Changes the material index multiple objects. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of an objects</param>
    ///<param name="materialIndex">(int) The new material index</param>
    [<Extension>]
    static member ObjectMaterialIndex(objectIds:Guid seq, materialIndex:int) : unit = //MULTISET
        if 0 <=. materialIndex .< Doc.Materials.Count then 
            RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectMaterialIndex failed for '%A' and '%A'"  materialIndex objectIds
        for objectId in objectIds do
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let attrs = rhinoobject.Attributes
            attrs.MaterialIndex <- materialIndex
            if not <| Doc.Objects.ModifyAttributes(rhinoobject, attrs, true) then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectMaterialIndex failed for '%A' and '%A'"  materialIndex objectId

    ///<summary>Returns the rendering material source of an object.</summary>
    ///<param name="objectId">(Guid) One or more object identifiers</param>
    ///<returns>(int) The current rendering material source
    ///    0 = Material from layer
    ///    1 = Material from object
    ///    3 = Material from parent.</returns>
    [<Extension>]
    static member ObjectMaterialSource(objectId:Guid) : int = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        int(rhinoobject.Attributes.MaterialSource)


    ///<summary>Modifies the rendering material source of an object.</summary>
    ///<param name="objectId">(Guid) One or more object identifiers</param>
    ///<param name="source">(int) The new rendering material source.
    ///    0 = Material from layer
    ///    1 = Material from object
    ///    3 = Material from parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectMaterialSource(objectId:Guid, source:int) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let rc = int(rhinoobject.Attributes.MaterialSource)
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectMaterialSource failed for '%A' and '%A'"  source objectId
        let source :DocObjects.ObjectMaterialSource  = LanguagePrimitives.EnumOfValue  source
        rhinoobject.Attributes.MaterialSource <- source
        if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectMaterialSource failed for '%A' and '%A'"  source objectId
        Doc.Views.Redraw()
    
    ///<summary>Modifies the rendering material source of multiple objects.</summary>
    ///<param name="objectIds">(Guid seq) One or more objects identifierss</param>
    ///<param name="source">(int) The new rendering material source.
    ///    0 = Material from layer
    ///    1 = Material from objects
    ///    3 = Material from parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectMaterialSource(objectIds:Guid seq, source:int) : unit = //MULTISET
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectMaterialSource failed for '%A' and '%A'"  source objectIds
        let source :DocObjects.ObjectMaterialSource  = LanguagePrimitives.EnumOfValue  source
        for objectId in objectIds do
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let rc = int(rhinoobject.Attributes.MaterialSource)            
            rhinoobject.Attributes.MaterialSource <- source
            if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectMaterialSource failed for '%A' and '%A'"  source objectId
        Doc.Views.Redraw()


    ///<summary>Returns the name of an object or "" if none given.</summary>
    ///<param name="objectId">(Guid)Id of object</param>
    ///<returns>(string) The current object name, empty string if no name given .</returns>
    [<Extension>]
    static member ObjectName(objectId:Guid) : string = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let n = rhinoobject.Attributes.Name
        if isNull n then ""
        else n
        

    ///<summary>Modifies the name of an object.</summary>
    ///<param name="objectId">(Guid)Id of object</param>
    ///<param name="name">(string) The new object name.</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectName(objectId:Guid, name:string) : unit = //SET
        //id = RhinoScriptSyntax.Coerceguid(objectId)
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.Name <- name
        if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectName failed for '%A' and '%A'"  name objectId
    
    ///<summary>Modifies the name of multiple objects.</summary>
    ///<param name="objectIds">(Guid seq)Id of objects</param>
    ///<param name="name">(string) The new objects name.</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectName(objectIds:Guid seq, name:string) : unit = //MULTISET
        for objectId in objectIds do
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoobject.Attributes.Name <- name
            if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectName failed for '%A' and '%A'"  name objectId



    ///<summary>Returns the print color of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<returns>(Drawing.Color) The object's current print color.</returns>
    [<Extension>]
    static member ObjectPrintColor(objectId:Guid) : Drawing.Color = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.PlotColor



    ///<summary>Modifies the print color of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<param name="color">(Drawing.Color) New print color.</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectPrintColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.PlotColorSource <- DocObjects.ObjectPlotColorSource.PlotColorFromObject
        rhinoobject.Attributes.PlotColor <- color
        if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintColor failed for '%A' and '%A'"  color objectId
        Doc.Views.Redraw()

    ///<summary>Modifies the print color of multiple objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifier of objects</param>
    ///<param name="color">(Drawing.Color) New print color.</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectPrintColor(objectIds:Guid seq, color:Drawing.Color) : unit = //MULTISET
        for objectId in objectIds do
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoobject.Attributes.PlotColorSource <- DocObjects.ObjectPlotColorSource.PlotColorFromObject
            rhinoobject.Attributes.PlotColor <- color
            if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintColor failed for '%A' and '%A'"  color objectId
        Doc.Views.Redraw()

    ///<summary>Returns the print color source of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<returns>(int) The object's current print color source
    ///    0 = print color by layer
    ///    1 = print color by object
    ///    3 = print color by parent.</returns>
    [<Extension>]
    static member ObjectPrintColorSource(objectId:Guid) : int = //GET
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            int(rhinoobject.Attributes.PlotColorSource)


    ///<summary>Modifies the print color source of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<param name="source">(int) New print color source
    ///    0 = print color by layer
    ///    1 = print color by object
    ///    3 = print color by parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectPrintColorSource(objectId:Guid, source:int) : unit = //SET
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintColorSource failed for '%A' and '%A'"  source objectId
        let source : DocObjects.ObjectPlotColorSource = LanguagePrimitives.EnumOfValue source
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.PlotColorSource <- source
        if not <| rhobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintColorSource failed for '%A' and '%A'" (rhType objectId) source
        Doc.Views.Redraw()

    ///<summary>Modifies the print color source of multiple objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifier of objects</param>
    ///<param name="source">(int) New print color source
    ///    0 = print color by layer
    ///    1 = print color by objects
    ///    3 = print color by parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectPrintColorSource(objectIds:Guid seq, source:int) : unit = //MULTISET
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintColorSource failed for '%A' and '%A'"  source objectIds
        let source : DocObjects.ObjectPlotColorSource = LanguagePrimitives.EnumOfValue source
        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhobj.Attributes.PlotColorSource <- source
            if not <| rhobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintColorSource failed for '%A' and '%A'" (rhType objectId) source
        Doc.Views.Redraw()

    ///<summary>Returns the print width of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<returns>(float) The object's current print width.</returns>
    [<Extension>]
    static member ObjectPrintWidth(objectId:Guid) : float = //GET
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoobject.Attributes.PlotWeight


    ///<summary>Modifies the print width of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<param name="width">(float) New print width value in millimeters, where width = 0.0 means use
    ///    the default width, and width smaller than 0.0 (e.g. -1.0)means do-not-print (visible for screen display,
    ///    but does not show on print)</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectPrintWidth(objectId:Guid, width:float) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let rc = rhinoobject.Attributes.PlotWeight
        rhinoobject.Attributes.PlotWeightSource <- DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
        rhinoobject.Attributes.PlotWeight <- width
        if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintWidth failed for '%A' and '%A'"  width objectId
        Doc.Views.Redraw()

    ///<summary>Modifies the print width of multiple objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifier of objects</param>
    ///<param name="width">(float) New print width value in millimeters, where width = 0.0 means use
    ///    the default width, and width smaller than 0.0 (e.g. -1.0)means do-not-print (visible for screen display,
    ///    but does not show on print)</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectPrintWidth(objectIds:Guid seq, width:float) : unit = //MULTISET
        for objectId in objectIds do 
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let rc = rhinoobject.Attributes.PlotWeight
            rhinoobject.Attributes.PlotWeightSource <- DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
            rhinoobject.Attributes.PlotWeight <- width
            if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintWidth failed for '%A' and '%A'"  width objectId
        Doc.Views.Redraw()


    ///<summary>Returns the print width source of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<returns>(int) The object's current print width source
    ///    0 = print width by layer
    ///    1 = print width by object
    ///    3 = print width by parent.</returns>
    [<Extension>]
    static member ObjectPrintWidthSource(objectId:Guid) : int = //GET
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            int(rhinoobject.Attributes.PlotWeightSource)


    ///<summary>Modifies the print width source of an object.</summary>
    ///<param name="objectId">(Guid) Identifier of object</param>
    ///<param name="source">(int) New print width source
    ///    0 = print width by layer
    ///    1 = print width by object
    ///    3 = print width by parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectPrintWidthSource(objectId:Guid, source:int) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.PlotWeightSource <- LanguagePrimitives.EnumOfValue source
        if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintWidthSource failed for '%A' and '%A'"  source objectId
        Doc.Views.Redraw()

    ///<summary>Modifies the print width source of multiple objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifier of objects</param>
    ///<param name="source">(int) New print width source
    ///    0 = print width by layer
    ///    1 = print width by objects
    ///    3 = print width by parent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ObjectPrintWidthSource(objectIds:Guid seq, source:int) : unit = //MULTISET
        for objectId in objectIds do     
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoobject.Attributes.PlotWeightSource <- LanguagePrimitives.EnumOfValue source
            if not <| rhinoobject.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectPrintWidthSource failed for '%A' and '%A'"  source objectId
        Doc.Views.Redraw()


    ///<summary>Returns the object type.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(int) The object type .
    ///    The valid object types are as follows:
    ///    Value   Description
    ///      0           Unknown object
    ///      1           Point
    ///      2           Point cloud
    ///      4           Curve
    ///      8           Surface or single-face brep
    ///      16          Polysurface or multiple-face
    ///      32          Mesh
    ///      256         Light
    ///      512         Annotation
    ///      4096        Instance or block reference
    ///      8192        Text dot object
    ///      16384       Grip object
    ///      32768       Detail
    ///      65536       Hatch
    ///      131072      Morph control
    ///      262144      SubD
    ///      134217728   Cage
    ///      268435456   Phantom
    ///      536870912   Clipping Plane
    ///      1073741824  Extrusion.</returns>
    [<Extension>]
    static member ObjectType(objectId:Guid) : int =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let geom = rhobj.Geometry
        match geom with
        | :? Brep as b ->
            if b.Faces.Count = 1 then 8 //surface //TODO extrusion too?
            else int(geom.ObjectType)
        |_ -> int(geom.ObjectType)


    // TODO, not implemented use Xform rotaion or scale instead
    //static member OrientObject( objectId:Guid,  reference:Point3d seq,  target:Point3d seq,   [<OPT;DEF(0)>]flags:int) : Guid =



    ///<summary>Rotates a single object.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to rotate</param>
    ///<param name="centerPoint">(Point3d) The center of rotation</param>
    ///<param name="rotationAngle">(float) In degrees</param>
    ///<param name="axis">(Vector3d) Optional, Default Value: <c>Vector3d.ZAxis</c>
    ///    Axis of rotation, If omitted, the Vector3d.ZAxis is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>. Copy the object</param>
    ///<returns>(Guid) Identifier of the rotated object.</returns>
    [<Extension>]
    static member RotateObject( objectId:Guid,
                                centerPoint:Point3d,
                                rotationAngle:float,
                                [<OPT;DEF(Vector3d())>]axis:Vector3d,
                                [<OPT;DEF(false)>]copy:bool) : Guid =
        let axis =
            if not axis.IsZero then
                Vector3d.ZAxis
            else
                axis
        let rotationAngle = RhinoMath.ToRadians(rotationAngle)
        let xf = Transform.Rotation(rotationAngle, axis, centerPoint)
        let res = Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.RotateObject failed.  objectId:'%s' centerPoint:'%A' rotationAngle:'%A' axis:'%A' copy:'%A'" (rhType objectId) centerPoint rotationAngle axis copy
        res



    ///<summary>Rotates multiple objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to rotate</param>
    ///<param name="centerPoint">(Point3d) The center of rotation</param>
    ///<param name="rotationAngle">(float) In degrees</param>
    ///<param name="axis">(Vector3d) Optional, Default Value: <c>Vector3d.ZAxis</c>
    ///    Axis of rotation, If omitted, the Vector3d.ZAxis is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>. Copy the object</param>
    ///<returns>(Guid Rarr) identifiers of the rotated objects.</returns>
    [<Extension>]
    static member RotateObject( objectIds:Guid seq,
                                 centerPoint:Point3d,
                                 rotationAngle:float,
                                 [<OPT;DEF(Vector3d())>]axis:Vector3d,
                                 [<OPT;DEF(false)>]copy:bool) : Guid Rarr = //PLURAL
        let axis =
            if not axis.IsZero then
                Vector3d.ZAxis
            else
                axis
        let rotationAngle = RhinoMath.ToRadians(rotationAngle)
        let xf = Transform.Rotation(rotationAngle, axis, centerPoint)
        let rc = Rarr()
        for objectId in objectIds do
            let res = Doc.Objects.Transform(objectId, xf, not copy)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.RotateObjects failed.  objectId:'%s' centerPoint:'%A' rotationAngle:'%A' axis:'%A' copy:'%A'" (rhType objectId) centerPoint rotationAngle axis copy
            rc.Add res
        rc



    ///<summary>Scales a single object. Can be used to perform a uniform or non-uniform
    ///    scale transformation. Scaling is based on the WorldXY Plane.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>.Copy the object</param>
    ///<returns>(Guid) Identifier of the scaled object.</returns>
    [<Extension>]
    static member ScaleObject( objectId:Guid,
                               origin:Point3d,
                               scale:float*float*float,
                               [<OPT;DEF(false)>]copy:bool) : Guid =
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let x, y, z = scale
        let xf = Transform.Scale(plane, x, y, z)
        let res = Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ScaleObject failed.  objectId:'%s' origin:'%A' scale:'%A' copy:'%A'" (rhType objectId) origin scale  copy
        res

    ///<summary>Scales a single object. Uniform scale transformation. Scaling is based on the WorldXY Plane.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float) One numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>. Copy the object</param>
    ///<returns>(Guid) Identifier of the scaled object.</returns>
    [<Extension>]
    static member ScaleObject( objectId:Guid,
                               origin:Point3d,
                               scale:float,
                               [<OPT;DEF(false)>]copy:bool) : Guid = //ALT
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let xf = Transform.Scale(plane, scale, scale, scale)
        let res = Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ScaleObject failed.  objectId:'%s' origin:'%A' scale:'%A' copy:'%A'" (rhType objectId) origin scale  copy
        res

    ///<summary>Scales one or more objects. Can be used to perform a uniform or non-
    ///    uniform scale transformation. Scaling is based on the WorldXY Plane.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to scale</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>. Copy the objects</param>
    ///<returns>(Guid Rarr) identifiers of the scaled objects.</returns>
    [<Extension>]
    static member ScaleObject( objectIds:Guid seq,
                                origin:Point3d,
                                scale:float*float*float,
                                [<OPT;DEF(false)>]copy:bool) : Guid Rarr =  //PLURAL
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let x, y, z = scale
        let xf = Transform.Scale(plane, x, y, z)
        let rc = Rarr()
        for objectId in objectIds do
            let res = Doc.Objects.Transform(objectId, xf, not copy)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ScaleObjects failed.  objectId:'%s' origin:'%A' scale:'%A' copy:'%A'" (rhType objectId) origin scale  copy
            rc.Add res
        rc

    ///<summary>Scales one or more objects. Uniform scale transformation. Scaling is based on the WorldXY Plane.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to scale</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float) One numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///    Copy the objects</param>
    ///<returns>(Guid Rarr) identifiers of the scaled objects.</returns>
    [<Extension>]
    static member ScaleObject( objectIds:Guid seq,
                                origin:Point3d,
                                scale:float,
                                [<OPT;DEF(false)>]copy:bool) : Guid Rarr =  //PLURALALT
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let xf = Transform.Scale(plane, scale, scale, scale)
        let rc = Rarr()
        for objectId in objectIds do
            let res = Doc.Objects.Transform(objectId, xf, not copy)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ScaleObjects failed.  objectId:'%s' origin:'%A' scale:'%A' copy:'%A'" (rhType objectId) origin scale  copy
            rc.Add res
        rc



    ///<summary>Selects a single object. 
    /// Throws an exception if object can't be selectyed for some reason
    ///  e.g. when locked , hidden, or on invisible layer .</summary>
    ///<param name="objectId">(Guid) The identifier of the object to select</param>
    ///<param name="forceVisible">(bool) Optional, Default Value: <c>false</c> whether to make objects that a hiddden or layers that are off visible and unlocked </param>
    ///<param name="ignoreErrors">(bool) Optional, Default Value: <c>false</c> whether to ignore errors when object can be set visible </param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member SelectObject( objectId:Guid,
                                [<OPT;DEF(false)>]forceVisible:bool,
                                [<OPT;DEF(false)>]ignoreErrors:bool) : unit =
        Synchronisation.DoSync false false (fun () -> 
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if 0 = rhobj.Select(true) then 
                if not ignoreErrors then 
                    let mutable redo = false
                    let lay = Doc.Layers.[rhobj.Attributes.LayerIndex]
                    if rhobj.IsHidden then 
                        if forceVisible then redo <- true ; Doc.Objects.Show(rhobj, true) |> ignore 
                        else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on hidden object %s" (rhType objectId) 
                    elif rhobj.IsLocked then 
                        if forceVisible then redo <- true ; Doc.Objects.Unlock(rhobj, true) |> ignore 
                        else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on locked object %s" (rhType objectId) 
                    elif not lay.IsVisible then 
                        if forceVisible then redo <- true ; visibleSetTrue(lay,true)
                        else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on invisible layer %s for object %s" lay.FullPath (rhType objectId) 
                    elif not lay.IsLocked then 
                        if forceVisible then redo <- true ; lockedSetFalse(lay,true)
                        else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on locked layer %s for object %s" lay.FullPath (rhType objectId) 
                    else
                        RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on object %s" (rhType objectId) 
                    if redo then 
                        if 0 = rhobj.Select(true) then 
                            RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed dispite forceVisible beeing set to true on object %s" (rhType objectId)     
            Doc.Views.Redraw()
            )

    ///<summary>Selects one or more objects
    /// Throws an exception if object can't be selectyed for some reason
    ///  e.g. when locked , hidden, or on invisible layer .</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of the objects to select</param>
    ///<param name="forceVisible">(bool) Optional, Default Value: <c>false</c> whether to make objects that a hiddden or layers that are off visible and unlocked </param>
    ///<param name="ignoreErrors">(bool) Optional, Default Value: <c>false</c> whether to ignore errors when object can be set visible </param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member SelectObject( objectIds:Guid seq,
                                [<OPT;DEF(false)>]forceVisible:bool,
                                [<OPT;DEF(false)>]ignoreErrors:bool) : unit =  //PLURAL
        Synchronisation.DoSync false false (fun () ->             
            for objectId in objectIds do
                let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
                if 0 = rhobj.Select(true) then 
                    if not ignoreErrors then 
                        let mutable redo = false
                        let lay = Doc.Layers.[rhobj.Attributes.LayerIndex]
                        if rhobj.IsHidden then 
                            if forceVisible then redo <- true ; Doc.Objects.Show(rhobj, true) |> ignore 
                            else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on hidden object %s out of %d objects" (rhType objectId) (Seq.length objectIds)
                        elif rhobj.IsLocked then 
                            if forceVisible then redo <- true ; Doc.Objects.Unlock(rhobj, true) |> ignore 
                            else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on locked object %s out of %d objects" (rhType objectId) (Seq.length objectIds)
                        elif not lay.IsVisible then 
                            if forceVisible then redo <- true ; visibleSetTrue(lay,true)
                            else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on invisible layer %s for object %s out of %d objects" lay.FullPath (rhType objectId) (Seq.length objectIds)
                        elif not lay.IsLocked then 
                            if forceVisible then redo <- true ; lockedSetFalse(lay,true)
                            else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on locked layer %s for object %s out of %d objects" lay.FullPath (rhType objectId) (Seq.length objectIds)
                        else
                            RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on object %s out of %d objects" (rhType objectId) (Seq.length objectIds)
                        if redo then 
                            if 0 = rhobj.Select(true) then 
                                RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed dispite forceVisible beeing set to true on object %s out of %d objects" (rhType objectId) (Seq.length objectIds) 
            Doc.Views.Redraw()
            )
        

    ///<summary>Perform a shear transformation on a single object.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="origin">(Point3d) Origin point of the shear transformation</param>
    ///<param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    ///<param name="angleDegrees">(float) The shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///    Copy the objects</param>
    ///<returns>(Guid) Identifier of the sheared object.</returns>
    [<Extension>]
    static member ShearObject( objectId:Guid,
                               origin:Point3d,
                               referencePoint:Point3d,
                               angleDegrees:float,
                               [<OPT;DEF(false)>]copy:bool) : Guid =
       if (origin-referencePoint).IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.ShearObject failed because (origin-referencePoint).IsTiny(): %A and %A" origin referencePoint
       let plane = Doc.Views.ActiveView.MainViewport.ConstructionPlane()
       let mutable frame = Plane(plane)
       frame.Origin <- origin
       frame.ZAxis <- plane.Normal
       let yaxis = referencePoint-origin
       yaxis.Unitize() |> ignore
       frame.YAxis <- yaxis
       let xaxis = Vector3d.CrossProduct(frame.ZAxis, frame.YAxis)
       xaxis.Unitize() |> ignore
       frame.XAxis <- xaxis
       let worldplane = Plane.WorldXY
       let cob = Transform.ChangeBasis(worldplane, frame)
       let mutable shear2d = Transform.Identity
       shear2d.[0, 1] <- tan(toRadians(angleDegrees))
       let cobinv = Transform.ChangeBasis(frame, worldplane)
       let xf = cobinv * shear2d * cob
       let res = Doc.Objects.Transform(objectId, xf, not copy)
       if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ShearObject failed for %s, origin %A, ref point  %A andangle  %A" (rhType objectId) origin referencePoint angleDegrees
       res


    ///<summary>Shears one or more objects.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers objects to shear</param>
    ///<param name="origin">(Point3d) Origin point of the shear transformation</param>
    ///<param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    ///<param name="angleDegrees">(float) The shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///    Copy the objects</param>
    ///<returns>(Guid Rarr) identifiers of the sheared objects.</returns>
    [<Extension>]
    static member ShearObject( objectIds:Guid seq,
                                origin:Point3d,
                                referencePoint:Point3d,
                                angleDegrees:float,
                                [<OPT;DEF(false)>]copy:bool) : Guid Rarr = //PLURAL
        if (origin-referencePoint).IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.ShearObject failed because (origin-referencePoint).IsTiny(): %A and %A" origin referencePoint
        let plane = Doc.Views.ActiveView.MainViewport.ConstructionPlane()
        let mutable frame = Plane(plane)
        frame.Origin <- origin
        frame.ZAxis <- plane.Normal
        let yaxis = referencePoint-origin
        yaxis.Unitize() |> ignore
        frame.YAxis <- yaxis
        let xaxis = Vector3d.CrossProduct(frame.ZAxis, frame.YAxis)
        xaxis.Unitize() |> ignore
        frame.XAxis <- xaxis
        let worldplane = Plane.WorldXY
        let cob = Transform.ChangeBasis(worldplane, frame)
        let mutable shear2d = Transform.Identity
        shear2d.[0, 1] <- tan(toRadians(angleDegrees))
        let cobinv = Transform.ChangeBasis(frame, worldplane)
        let xf = cobinv * shear2d * cob
        rarr{
            for ob in objectIds do
                let res = Doc.Objects.Transform(ob, xf, not copy)
                if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ShearObject failed for %A, origin %A, ref point  %A andangle  %A" ob origin referencePoint angleDegrees
                res  }


    ///<summary>Shows a previously hidden object. Hidden objects are not visible, cannot
    ///    be snapped to and cannot be selected.</summary>
    ///<param name="objectId">(Guid) Representing id of object to show</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ShowObject(objectId:Guid) : unit =
        if not <| Doc.Objects.Show(objectId, false) then RhinoScriptingException.Raise "RhinoScriptSyntax.ShowObject failed on %A" (rhType objectId)
        Doc.Views.Redraw()


    ///<summary>Shows one or more objects. Hidden objects are not visible, cannot be
    ///    snapped to and cannot be selected.</summary>
    ///<param name="objectIds">(Guid seq) Ids of objects to show</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member ShowObject(objectIds:Guid seq) : unit =
        let mutable rc = 0
        for objectId in objectIds do
            if not <| Doc.Objects.Show(objectId, false) then RhinoScriptingException.Raise "RhinoScriptSyntax.ShowObject failed on %A" (rhType objectId)
        Doc.Views.Redraw()


    ///<summary>Unlocks an object. Locked objects are visible, and can be snapped to,
    ///    but they cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member UnlockObject(objectId:Guid) : unit =
        if not <| Doc.Objects.Unlock(objectId, false) then RhinoScriptingException.Raise "RhinoScriptSyntax.UnlockObject faild on %A" (rhType objectId)
        Doc.Views.Redraw()

    ///<summary>Unlocks one or more objects. Locked objects are visible, and can be
    ///    snapped to, but they cannot be selected.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of objects</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member UnlockObject(objectIds:Guid seq) : unit =  //PLURAL
        let mutable rc = 0
        for objectId in objectIds do
            if not <| Doc.Objects.Unlock(objectId, false) then RhinoScriptingException.Raise "RhinoScriptSyntax.UnlockObject faild on %A" (rhType objectId)
        Doc.Views.Redraw()
        

    ///<summary>Unselects a single selected object.</summary>
    ///<param name="objectId">(Guid) Id of object to unselect</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member UnSelectObject(objectId:Guid) : unit =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if 0 <> obj.Select(false) then RhinoScriptingException.Raise "RhinoScriptSyntax.UnSelectObject failed on %A" (rhType objectId)
        Doc.Views.Redraw()


    ///<summary>Unselects multiple selected objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of the objects to unselect</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member UnSelectObject(objectIds:Guid seq) : unit = //PLURAL
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if 0 <> obj.Select(false) then RhinoScriptingException.Raise "RhinoScriptSyntax.UnSelectObject failed on %A" (rhType objectId)            
        Doc.Views.Redraw()

