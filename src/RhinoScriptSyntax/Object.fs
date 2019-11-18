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
            let objectId = Doc.Objects.Transform(objectid, matrix, not copy)
            if objectId = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply transform to object '%A' from objectId:'%A' matrix:'%A' copy:'%A'" objectid objectId matrix copy
            rc.Add objectId
        Doc.Views.Redraw()
        rc

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
        let res = Doc.Objects.Transform(objectId, matrix, not copy)
        if res = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply transform to object '%A' from objectId:'%A' matrix:'%A' copy:'%A'" objectId objectId matrix copy
        res


    [<EXT>]
    ///<summary>Copies object from one location to another, or in-place.</summary>
    ///<param name="objectId">(Guid) Object to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value: <c>Vector3d()</c>
    ///Translation vector to apply</param>
    ///<returns>(Guid) objectId for the copy</returns>
    static member CopyObject(objectId:Guid, [<OPT;DEF(Vector3d())>]translation:Vector3d) : Guid =
        let translation =
            if not translation.IsZero then
                Transform.Translation(translation)
            else
                Transform.Identity
        let res = Doc.Objects.Transform(objectId, translation, false)
        if res = Guid.Empty then failwithf "Rhino.Scripting: CopyObject failed.  objectId:'%A' translation:'%A'" objectId translation
        res



    [<EXT>]
    ///<summary>Copies one or more objects from one location to another, or in-place.</summary>
    ///<param name="objectIds">(Guid seq) List of objects to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value: <c>Vector3d()</c>
    ///List of three numbers or Vector3d representing
    ///  translation vector to apply to copied set</param>
    ///<returns>(Guid ResizeArray) identifiers for the copies</returns>
    static member CopyObjects(objectIds:Guid seq, [<OPT;DEF(Vector3d())>]translation:Vector3d) : Guid ResizeArray =
        let translation =
            if not translation.IsZero then
                Transform.Translation(translation)
            else
                Transform.Identity
        let rc = ResizeArray()
        for objectid in objectIds do
            let res = Doc.Objects.Transform(objectid, translation, false)
            if res = Guid.Empty then failwithf "Rhino.Scripting: CopyObjectc failed.  objectId:'%A' translation:'%A'" objectid translation
            rc.Add res
        rc


    [<EXT>]
    ///<summary>Deletes a single object from the document</summary>
    ///<param name="objectId">(Guid) Identifier of object to delete</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteObject(objectId:Guid) : unit =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        if not <| Doc.Objects.Delete(objectId,true)  then failwithf "DeleteObject failed on %A" objectId
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Deletes one or more objects from the document, Fails if not all objects can be deleted</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to delete</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteObjects(objectIds:Guid seq) : unit =
        let k = Doc.Objects.Delete(objectIds,true)
        let l = Seq.length objectIds
        if k <> l then failwithf "DeleteObjects failed on %d out of %A" (l-k) objectIds
        Doc.Views.Redraw()
        //let mutable rc = 0
        ////objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //for objectId in objectId do
        //    //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //    if Doc.Objects.Delete(objectId) then rc <- rc + 1
        //if rc > 0 then Doc.Views.Redraw()
        //rc


    [<EXT>]
    ///<summary>Causes the selection state of one or more objects to change momentarily
    ///  so the object appears to flash on the screen</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to flash</param>
    ///<param name="style">(bool) Optional, Default Value: <c>true</c>
    ///If True, flash between object color and selection color.
    ///  If False, flash between visible and invisible</param>
    ///<returns>(unit)</returns>
    static member FlashObject(objectIds:Guid seq, [<OPT;DEF(true)>]style:bool) : unit =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //if notNull objectId then objectId <- .[objectId]
        let rhobjs = resizeArray { for objectId in objectIds do yield RhinoScriptSyntax.CoerceRhinoObject(objectId) }
        if rhobjs.Count>0 then
            Doc.Views.FlashObjects(rhobjs, style)


    [<EXT>]
    ///<summary>Hides a single object</summary>
    ///<param name="objectId">(Guid) Id of object to hide</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member HideObject(objectId:Guid) : bool =
        Doc.Objects.Hide(objectId, false)


    [<EXT>]
    ///<summary>Hides one or more objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to hide</param>
    ///<returns>(int) Number of objects hidden</returns>
    static member HideObjects(objectIds:Guid seq) : int =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //if notNull objectId then objectId <- .[objectId]
        let mutable rc = 0
        for objectId in objectIds do
            //objectId = RhinoScriptSyntax.Coerceguid(objectId)
            if Doc.Objects.Hide(objectId, false) then rc <- rc + 1
        if  rc>0 then Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Verifies that an object is in either page layout space or model space</summary>
    ///<param name="objectId">(Guid) Id of an object to test</param>
    ///<returns>(bool) True if the object is in page layout space, False if the object is in model space</returns>
    static member IsLayoutObject(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.Space = Rhino.DocObjects.ActiveSpace.PageSpace


    [<EXT>]
    ///<summary>Verifies the existence of an object</summary>
    ///<param name="objectId">(Guid) An object to test</param>
    ///<returns>(bool) True if the object exists, False if the object does not exist</returns>
    static member IsObject(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceRhinoObject(objectId) <> None


    [<EXT>]
    ///<summary>Verifies that an object is hidden. Hidden objects are not visible, cannot
    ///  be snapped to, and cannot be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is hidden, False if the object is not hidden</returns>
    static member IsObjectHidden(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsHidden


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



    [<EXT>]
    ///<summary>Verifies that an object is locked. Locked objects are visible, and can
    ///  be snapped to, but cannot be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is locked, False if the object is not locked</returns>
    static member IsObjectLocked(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsLocked


    [<EXT>]
    ///<summary>Verifies that an object is normal. Normal objects are visible, can be
    ///  snapped to, and can be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is normal, False if the object is not normal</returns>
    static member IsObjectNormal(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsNormal


    [<EXT>]
    ///<summary>Verifies that an object is a reference object. Reference objects are
    ///  objects that are not part of the current document</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is a reference object, False if the object is not a reference object</returns>
    static member IsObjectReference(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsReference


    [<EXT>]
    ///<summary>Verifies that an object can be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True or False</returns>
    static member IsObjectSelectable(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsSelectable(true,false,false,false)


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


    [<EXT>]
    ///<summary>Verifies an object's geometry is valid and without error</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is valid</returns>
    static member IsObjectValid(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceRhinoObject(objectId) with
        |None -> false
        |Some rhobj ->  rhobj.IsValid


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


    [<EXT>]
    ///<summary>Locks a single object. Locked objects are visible, and they can be
    ///  snapped to. But, they cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member LockObject(objectId:Guid) : bool =
        Doc.Objects.Lock(objectId, false)


    [<EXT>]
    ///<summary>Locks one or more objects. Locked objects are visible, and they can be
    ///  snapped to. But, they cannot be selected.</summary>
    ///<param name="objectIds">(Guid seq) List of Strings or Guids. The identifiers of objects</param>
    ///<returns>(int) number of objects locked</returns>
    static member LockObjects(objectIds:Guid seq) : int =
        let mutable rc = 0
        for objectId in objectIds do
            if Doc.Objects.Lock(objectId, false) then rc <- rc +   1
        if 0<> rc then Doc.Views.Redraw()
        rc


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


    [<EXT>]
    ///<summary>Mirrors a single object on World XY Plane</summary>
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
        let normal = Plane.WorldXY.Normal
        let xv = Vector3d.CrossProduct(vec, normal)
        xv.Unitize() |> ignore
        let xf = Transform.Mirror(startPoint, vec)
        let res = Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply MirrorObject transform to objectId:'%A' startPoint:'%A' endPoint:'%A' copy:'%A'" objectId startPoint endPoint copy
        res


    [<EXT>]
    ///<summary>Mirrors a list of objects on World XY Plane</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to mirror</param>
    ///<param name="startPoint">(Point3d) Start of the mirror plane</param>
    ///<param name="endPoint">(Point3d) End of the mirror plane</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid ResizeArray) List of identifiers of the mirrored objects</returns>
    static member MirrorObjects( objectIds:Guid seq,
                                 startPoint:Point3d,
                                 endPoint:Point3d,
                                 [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =
        let vec = endPoint-startPoint
        if vec.IsTiny() then failwithf "Rhino.Scripting: Start and  end points are too close to each other.  objectId:'%A' startPoint:'%A' endPoint:'%A' copy:'%A'" objectIds startPoint endPoint copy
        let normal = Plane.WorldXY.Normal
        let xv = Vector3d.CrossProduct(vec, normal)
        xv.Unitize() |> ignore
        let xf = Transform.Mirror(startPoint, vec)
        let rc = ResizeArray()
        for objectid in objectIds do
            let objectId = Doc.Objects.Transform(objectid, xf, not copy)
            if objectId = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply MirrorObjects to objectId:'%A' startPoint:'%A' endPoint:'%A' copy:'%A'" objectId startPoint endPoint copy
            rc.Add objectId
        rc



    [<EXT>]
    ///<summary>Moves a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(Guid) Identifier of the moved object</returns>
    static member MoveObject(objectId:Guid, translation:Vector3d) : Guid =
        let xf = Transform.Translation(translation)
        let res = Doc.Objects.Transform(objectId, xf, true)
        if res = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply move to from objectId:'%A' translation:'%A'" objectId translation
        res


    [<EXT>]
    ///<summary>Moves one or more objects</summary>
    ///<param name="objectIds">(Guid seq) The identifiers objects to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(Guid ResizeArray) identifiers of the moved objects</returns>
    static member MoveObjects(objectIds:Guid seq, translation:Vector3d) : Guid ResizeArray =
        //translation = RhinoScriptSyntax.Coerce3dvector(translation)
        let xf = Transform.Translation(translation)
        let rc = ResizeArray()
        for objectid in objectIds do
            let objectId = Doc.Objects.Transform(objectid, xf, true)
            if objectId = Guid.Empty then failwithf "Rhino.Scripting: Cannot apply MoveObjects Transform to objectId:'%A'  translation:'%A'" objectId translation
            rc.Add objectId
        rc



    [<EXT>]
    ///<summary>Returns the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectId">(Guid) Id object</param>
    ///<returns>(Drawing.Color) The current color value</returns>
    static member ObjectColor(objectId:Guid) : Drawing.Color = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.DrawColor(Doc)

    [<EXT>]
    ///<summary>Modifies the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectId">(Guid) Id or ids of object</param>
    ///<param name="color">(Drawing.Color) The new color value.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let attr = rhobj.Attributes
        attr.ObjectColor <- color
        attr.ColorSource <- Rhino.DocObjects.ObjectColorSource.ColorFromObject
        if not <| Doc.Objects.ModifyAttributes( rhobj, attr, true) then failwithf "set ObjectColor faile for %A; %A" objectId color
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the color source of an object.</summary>
    ///<param name="objectId">(Guid) Single identifier of list of identifiers</param>
    ///<returns>(int) The current color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</returns>
    static member ObjectColorSource(objectId:Guid) : int = //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        int(rhobj.Attributes.ColorSource)

    ///<summary>Modifies the color source of an object.</summary>
    ///<param name="objectId">(Guid) Single identifier of list of identifiers</param>
    ///<param name="source">(int) New color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectColorSource(objectId:Guid, source:int) : unit = //SET
        let source : Rhino.DocObjects.ObjectColorSource = LanguagePrimitives.EnumOfValue source
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.ColorSource <- source
        if not <| rhobj.CommitChanges() then failwithf "Set ObjectColorSource failed for '%A' and '%A'" objectId source
        Doc.Views.Redraw()




    [<EXT>]
    ///<summary>Returns a short text description of an object</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(string) A short text description of the object .</returns>
    static member ObjectDescription(objectId:Guid) : string =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.ShortDescription(false)


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



    [<EXT>]
    ///<summary>Returns the layer of an object</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<returns>(string) The object's current layer</returns>
    static member ObjectLayer(objectId:Guid) : string = //GET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = obj.Attributes.LayerIndex
        Doc.Layers.[index].FullPath

    ///<summary>Modifies the layer of an object</summary>
    ///<param name="objectId">(Guid) The identifier of the objects</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectLayer(objectId:Guid, layer:string) : unit = //SET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let index = layer.Index
        obj.Attributes.LayerIndex <- index
        if not <| obj.CommitChanges() then failwithf "Set ObjectLayer failed for '%A' and '%A'"  layer objectId
        Doc.Views.Redraw()

    ///<summary>Modifies the layer of an objects</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of the objects</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectsLayer(objectIds:Guid seq, layer:string) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let index = layer.Index
        for objectId in objectIds do
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



    ///<summary>Changes the layout or model space of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="layout">(string) To change, or move, an object from model space to page
    ///  layout space, or from one page layout to another, then specify the
    ///  title or identifier of an existing page layout view. To move an object
    ///  from page layout space to model space, just specify Empty String ""</param>
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
    ///<param name="objectId">(Guid) Identifiers of object</param>
    ///<returns>(string) The object's current linetype</returns>
    static member ObjectLinetype(objectId:Guid) : string = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldindex = Doc.Linetypes.LinetypeIndexForObject(rhinoobject)
        Doc.Linetypes.[oldindex].Name


    ///<summary>Modifies the linetype of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object</param>
    ///<param name="linetype">(string) Name of an existing linetyp. If omitted, the current
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




    [<EXT>]
    ///<summary>Returns the linetype source of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object</param>
    ///<returns>(int) The object's current linetype source
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</returns>
    static member ObjectLinetypeSource(objectId:Guid) : int = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldsource = rhinoobject.Attributes.LinetypeSource
        int(oldsource)



    ///<summary>Modifies the linetype source of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects</param>
    ///<param name="source">(int) New linetype source. If omitted, the current source is returned.
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

    ///<summary>Changes the material index of an object. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<param name="materialIndex">(int) The new material index</param>
    static member ObjectMaterialIndex(objectId:Guid, materialIndex:int) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if 0 <=. materialIndex .< Doc.Materials.Count then failwithf "Set ObjectMaterialIndex failed for '%A' and '%A'"  materialIndex objectId
        let attrs = rhinoobject.Attributes
        attrs.MaterialIndex <- materialIndex
        if not <| Doc.Objects.ModifyAttributes(rhinoobject, attrs, true) then failwithf "Set ObjectMaterialIndex failed for '%A' and '%A'"  materialIndex objectId


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


    ///<summary>Modifies the rendering material source of an object.</summary>
    ///<param name="objectId">(Guid) One or more object identifiers</param>
    ///<param name="source">(int) The new rendering material source. If omitted and a single
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




    [<EXT>]
    ///<summary>Returns the name of an object</summary>
    ///<param name="objectId">(Guid) Id or ids of object(s)</param>
    ///<returns>(string) The current object name</returns>
    static member ObjectName(objectId:Guid) : string = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.Name

    ///<summary>Modifies the name of an object</summary>
    ///<param name="objectId">(Guid) Id or ids of object(s)</param>
    ///<param name="name">(string) The new object name. If omitted, the current name is returned</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectName(objectId:Guid, name:string) : unit = //SET
        //id = RhinoScriptSyntax.Coerceguid(objectId)
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.Name <- name
        if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectName failed for '%A' and '%A'"  name objectId


    [<EXT>]
    ///<summary>Returns the print color of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object</param>
    ///<returns>(Drawing.Color) The object's current print color</returns>
    static member ObjectPrintColor(objectId:Guid) : Drawing.Color = //GET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.PlotColor



    ///<summary>Modifies the print color of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects</param>
    ///<param name="color">(Drawing.Color) New print color. If omitted, the current color is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.PlotColorSource <- Rhino.DocObjects.ObjectPlotColorSource.PlotColorFromObject
        rhinoobject.Attributes.PlotColor <- color
        if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectPrintColor failed for '%A' and '%A'"  color objectId
        Doc.Views.Redraw()


    [<EXT>]
    ///<summary>Returns the print color source of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects</param>
    ///<returns>(int) The object's current print color source
    ///  0 = print color by layer
    ///  1 = print color by object
    ///  3 = print color by parent</returns>
    static member ObjectPrintColorSource(objectId:Guid) : int = //GET
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            int(rhinoobject.Attributes.PlotColorSource)


    ///<summary>Modifies the print color source of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects</param>
    ///<param name="source">(int) New print color source
    ///  0 = print color by layer
    ///  1 = print color by object
    ///  3 = print color by parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintColorSource(objectId:Guid, source:int) : unit = //SET
        if source <0 || source >3 || source=2 then failwithf "Set ObjectPrintColorSource failed for '%A' and '%A'"  source objectId
        let source : Rhino.DocObjects.ObjectPlotColorSource = LanguagePrimitives.EnumOfValue source
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.PlotColorSource <- source
        if not <| rhobj.CommitChanges() then failwithf "Set ObjectPrintColorSource failed for '%A' and '%A'" objectId source
        Doc.Views.Redraw()





    [<EXT>]
    ///<summary>Returns the print width of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object</param>
    ///<returns>(float) The object's current print width</returns>
    static member ObjectPrintWidth(objectId:Guid) : float = //GET
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoobject.Attributes.PlotWeight


    ///<summary>Modifies the print width of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects</param>
    ///<param name="width">(float) New print width value in millimeters, where width=0 means use
    ///  the default width, and width<0 means do not print (visible for screen display,
    ///  but does not show on print).</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintWidth(objectId:Guid, width:float) : unit = //SET
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let rc = rhinoobject.Attributes.PlotWeight
            rhinoobject.Attributes.PlotWeightSource <- Rhino.DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
            rhinoobject.Attributes.PlotWeight <- width
            if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectPrintWidth failed for '%A' and '%A'"  width objectId
            Doc.Views.Redraw()


    [<EXT>]
    ///<summary>Returns the print width source of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object</param>
    ///<returns>(int) The object's current print width source
    ///  0 = print width by layer
    ///  1 = print width by object
    ///  3 = print width by parent</returns>
    static member ObjectPrintWidthSource(objectId:Guid) : int = //GET
            let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            int(rhinoobject.Attributes.PlotWeightSource)


    ///<summary>Modifies the print width source of an object</summary>
    ///<param name="objectId">(Guid) Identifiers of object</param>
    ///<param name="source">(int) New print width source
    ///  0 = print width by layer
    ///  1 = print width by object
    ///  3 = print width by parent</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectPrintWidthSource(objectId:Guid, source:int) : unit = //SET
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoobject.Attributes.PlotWeightSource <- LanguagePrimitives.EnumOfValue source
        if not <| rhinoobject.CommitChanges() then failwithf "Set ObjectPrintWidthSource failed for '%A' and '%A'"  source objectId
        Doc.Views.Redraw()



    [<EXT>]
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let geom = rhobj.Geometry
        match geom with
        | :? Brep as b ->
            if b.Faces.Count = 1 then 8 //surface //TODO extrusion too?
            else int(geom.ObjectType)
        |_ -> int(geom.ObjectType)


    // TODO, not implemented use Xform rotaion or scale instead
    //static member OrientObject( objectId:Guid,   reference:Point3d seq,   target:Point3d seq,    [<OPT;DEF(0)>]flags:int) : Guid =



    [<EXT>]
    ///<summary>Rotates a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to rotate</param>
    ///<param name="centerPoint">(Point3d) The center of rotation</param>
    ///<param name="rotationAngle">(float) In degrees</param>
    ///<param name="axis">(Vector3D) Optional, Default Value: <c>Vector3d.ZAxis</c>
    ///Axis of rotation, If omitted, the Vector3d.ZAxis is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the rotated object</returns>
    static member RotateObject( objectId:Guid,
                                centerPoint:Point3d,
                                rotationAngle:float,
                                [<OPT;DEF(Plane())>]axis:Vector3d,
                                [<OPT;DEF(false)>]copy:bool) : Guid =
        let axis =
            if not axis.IsZero then
                Vector3d.ZAxis
            else
                axis
        let rotationAngle = Rhino.RhinoMath.ToRadians(rotationAngle)
        let xf = Transform.Rotation(rotationAngle, axis, centerPoint)
        let res = Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then failwithf "Rhino.Scripting: RotateObject failed.  objectId:'%A' centerPoint:'%A' rotationAngle:'%A' axis:'%A' copy:'%A'" objectId centerPoint rotationAngle axis copy
        res



    [<EXT>]
    ///<summary>Rotates multiple objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to rotate</param>
    ///<param name="centerPoint">(Point3d) The center of rotation</param>
    ///<param name="rotationAngle">(float) In degrees</param>
    ///<param name="axis">(Vector3D) Optional, Default Value: <c>Vector3d.ZAxis</c>
    ///Axis of rotation, If omitted, the Vector3d.ZAxis is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid ResizeArray) identifiers of the rotated objects</returns>
    static member RotateObjects( objectIds:Guid seq,
                                 centerPoint:Point3d,
                                 rotationAngle:float,
                                 [<OPT;DEF(Vector3d())>]axis:Vector3d,
                                 [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =
        let axis =
            if not axis.IsZero then
                Vector3d.ZAxis
            else
                axis
        let rotationAngle = Rhino.RhinoMath.ToRadians(rotationAngle)
        let xf = Transform.Rotation(rotationAngle, axis, centerPoint)
        let rc = ResizeArray()
        for objectid in objectIds do
            let res = Doc.Objects.Transform(objectid, xf, not copy)
            if res = Guid.Empty then failwithf "Rhino.Scripting: RotateObjects failed.  objectId:'%A' centerPoint:'%A' rotationAngle:'%A' axis:'%A' copy:'%A'" objectid centerPoint rotationAngle axis copy
            rc.Add res
        rc



    [<EXT>]
    ///<summary>Scales a single object. Can be used to perform a uniform or non-uniform
    ///  scale transformation. Scaling is based on the WorldXY plane.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the scaled object</returns>
    static member ScaleObject( objectId:Guid,
                               origin:Point3d,
                               scale:float*float*float,
                               [<OPT;DEF(false)>]copy:bool) : Guid =
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let x,y,z = scale
        let xf = Transform.Scale(plane,x,y,z)
        let res = Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then failwithf "Rhino.Scripting: ScaleObject failed.  objectId:'%A' origin:'%A' scale:'%A' copy:'%A'" objectId origin scale  copy
        res

    [<EXT>]
    ///<summary>Scales a single object. Uniform scale transformation. Scaling is based on the WorldXY plane.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float) One numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the scaled object</returns>
    static member ScaleObject( objectId:Guid,
                               origin:Point3d,
                               scale:float,
                               [<OPT;DEF(false)>]copy:bool) : Guid =
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let xf = Transform.Scale(plane,scale,scale,scale)
        let res = Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then failwithf "Rhino.Scripting: ScaleObject failed.  objectId:'%A' origin:'%A' scale:'%A' copy:'%A'" objectId origin scale  copy
        res

    [<EXT>]
    ///<summary>Scales one or more objects. Can be used to perform a uniform or non-
    ///  uniform scale transformation. Scaling is based on the WorldXY plane.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to scale</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid ResizeArray) identifiers of the scaled objects</returns>
    static member ScaleObjects( objectIds:Guid seq,
                                origin:Point3d,
                                scale:float*float*float,
                                [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let x,y,z = scale
        let xf = Transform.Scale(plane,x,y,z)
        let rc = ResizeArray()
        for objectid in objectIds do
            let res = Doc.Objects.Transform(objectid, xf, not copy)
            if res = Guid.Empty then failwithf "Rhino.Scripting: ScaleObjects failed.  objectId:'%A' origin:'%A' scale:'%A' copy:'%A'" objectid origin scale  copy
            rc.Add res
        rc

    [<EXT>]
    ///<summary>Scales one or more objects. Uniform scale transformation. Scaling is based on the WorldXY plane.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to scale</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float) One numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid ResizeArray) identifiers of the scaled objects</returns>
    static member ScaleObjects( objectIds:Guid seq,
                                origin:Point3d,
                                scale:float,
                                [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let xf = Transform.Scale(plane,scale,scale,scale)
        let rc = ResizeArray()
        for objectid in objectIds do
            let res = Doc.Objects.Transform(objectid, xf, not copy)
            if res = Guid.Empty then failwithf "Rhino.Scripting: ScaleObjects failed.  objectId:'%A' origin:'%A' scale:'%A' copy:'%A'" objectid origin scale  copy
            rc.Add res
        rc




    [<EXT>]
    ///<summary>Selects a single object</summary>
    ///<param name="objectId">(Guid) The identifier of the object to select</param>
    ///<param name="redraw">(bool) Optional, Default Value: <c>true</c>
    ///Redraw view too</param>
    ///<returns>(bool) True on success</returns>
    static member SelectObject(objectId:Guid, [<OPT;DEF(true)>]redraw:bool) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let r = rhobj.Select(true)
        if redraw then Doc.Views.Redraw()
        r>0



    [<EXT>]
    ///<summary>Selects one or more objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of the objects to select</param>
    ///<returns>(int) number of selected objects</returns>
    static member SelectObjects(objectIds:Guid seq) : int =
        let mutable rc = 0
        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let r = rhobj.Select(true)
            if r>1 then rc <- rc +   1
        if rc > 0 then Doc.Views.Redraw()
        rc

    [<EXT>]
    ///<summary>Perform a shear transformation on a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="origin">(Point3d) Origin point of the shear transformation</param>
    ///<param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    ///<param name="angleDegrees">(float) The shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid) Identifier of the sheared object</returns>
    static member ShearObject( objectId:Guid,
                               origin:Point3d,
                               referencePoint:Point3d,
                               angleDegrees:float,
                               [<OPT;DEF(false)>]copy:bool) : Guid =
       if (origin-referencePoint).IsTiny() then failwithf "ShearObject failed because (origin-referencePoint).IsTiny(): %A and %A " origin referencePoint
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
       shear2d.[0,1] <- tan(toRadians(angleDegrees))
       let cobinv = Transform.ChangeBasis(frame, worldplane)
       let xf = cobinv * shear2d * cob
       let res = Doc.Objects.Transform(objectId, xf, not copy)
       if res = Guid.Empty then failwithf "Rhino.Scripting: ShearObject failed for %A, origin %A, ref point  %A andangle  %A " objectId origin referencePoint angleDegrees
       res




    [<EXT>]
    ///<summary>Shears one or more objects</summary>
    ///<param name="objectIds">(Guid seq) The identifiers objects to shear</param>
    ///<param name="origin">(Point3d) Origin point of the shear transformation</param>
    ///<param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    ///<param name="angleDegrees">(float) The shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid ResizeArray) identifiers of the sheared objects</returns>
    static member ShearObjects( objectIds:Guid seq,
                                origin:Point3d,
                                referencePoint:Point3d,
                                angleDegrees:float,
                                [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =
        if (origin-referencePoint).IsTiny() then failwithf "ShearObject failed because (origin-referencePoint).IsTiny(): %A and %A " origin referencePoint
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
        shear2d.[0,1] <- tan(toRadians(angleDegrees))
        let cobinv = Transform.ChangeBasis(frame, worldplane)
        let xf = cobinv * shear2d * cob
        resizeArray{
            for ob in objectIds do
                let res = Doc.Objects.Transform(ob, xf, not copy)
                if res = Guid.Empty then failwithf "Rhino.Scripting: ShearObject failed for %A, origin %A, ref point  %A andangle  %A " ob origin referencePoint angleDegrees
                res  }


    [<EXT>]
    ///<summary>Shows a previously hidden object. Hidden objects are not visible, cannot
    ///  be snapped to and cannot be selected</summary>
    ///<param name="objectId">(Guid) Representing id of object to show</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member ShowObject(objectId:Guid) : bool =
        Doc.Objects.Show(objectId, false)


    [<EXT>]
    ///<summary>Shows one or more objects. Hidden objects are not visible, cannot be
    ///  snapped to and cannot be selected</summary>
    ///<param name="objectIds">(Guid seq) Ids of objects to show</param>
    ///<returns>(int) Number of objects shown</returns>
    static member ShowObjects(objectIds:Guid seq) : int =
        let mutable rc = 0
        for objectId in objectIds do
            if Doc.Objects.Show(objectId, false) then rc <- rc +   1
        if 0<> rc then Doc.Views.Redraw()
        rc




    [<EXT>]
    ///<summary>Unlocks an object. Locked objects are visible, and can be snapped to,
    ///  but they cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member UnlockObject(objectId:Guid) : bool =
        Doc.Objects.Unlock(objectId, false)


    [<EXT>]
    ///<summary>Unlocks one or more objects. Locked objects are visible, and can be
    ///  snapped to, but they cannot be selected.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of objects</param>
    ///<returns>(int) number of objects unlocked</returns>
    static member UnlockObjects(objectIds:Guid seq) : int =
        let mutable rc = 0
        for objectId in objectIds do
            if Doc.Objects.Unlock(objectId, false) then rc <- rc +   1
        if 0 <> rc then Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Unselects a single selected object</summary>
    ///<param name="objectId">(Guid) Id of object to unselect</param>
    ///<returns>(unit) void, nothing</returns>
    static member UnselectObject(objectId:Guid) : unit =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        obj.Select(false)
        |> ignore



    [<EXT>]
    ///<summary>Unselects one or more selected objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of the objects to unselect.</param>
    ///<returns>(unit) void, nothing</returns>
    static member UnselectObjects(objectIds:Guid seq) : unit =
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            obj.Select(false)
            |>ignore


