namespace Rhino.Scripting

open Rhino
open System
open System.Collections.Generic
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils


[<AutoOpen>]
module AutoOpenObject =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    /// <summary>Moves, scales, or rotates a list of objects given a 4x4 transformation
    ///    matrix. The matrix acts on the left. To transform geometry objects instead of DocObjects or Guids use their .Transform(xForm) member.</summary>
    /// <param name="objectIds">(Guid seq) List of object identifiers</param>
    /// <param name="matrix">(Transform) The transformation matrix (4x4 array of numbers)</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>
    ///    Copy the objects</param>
    /// <returns>(Guid ResizeArray) IDs identifying the newly transformed objects.</returns>
    static member TransformObjects( objectIds:Guid seq,
                                    matrix:Transform,
                                    [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =   //PLURAL
        // this is also called by Copy, Scale, Mirror, Move, and Rotate functions defined below
        let rc = ResizeArray()
        for objId in objectIds do
            let objectId = State.Doc.Objects.Transform(objId, matrix, not copy)
            if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.TransformObjects: Cannot apply transform to object '%s' from objectId:'%s' matrix:'%A' copy:'%A'" (Pretty.str objId) (Pretty.str objectId) matrix copy
            rc.Add objectId
        State.Doc.Views.Redraw()
        rc

    /// <summary>Moves, scales, or rotates an object given a 4x4 transformation matrix.
    ///    The matrix acts on the left. To transform geometry objects instead of DocObjects or Guids use their .Transform(xForm) member.</summary>
    /// <param name="objectId">(Guid) The identifier of the object</param>
    /// <param name="matrix">(Transform) The transformation matrix (4x4 array of numbers)</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>
    ///    Copy the object</param>
    /// <returns>(Guid) The identifier of the transformed object.</returns>
    static member TransformObject(  objectId:Guid,
                                    matrix:Transform,
                                    [<OPT;DEF(false)>]copy:bool) : Guid =
        let res = State.Doc.Objects.Transform(objectId, matrix, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.TransformObject: Cannot apply transform to objectId:'%s' matrix:'%A' copy:'%A'"  (Pretty.str objectId) matrix copy
        res


    /// <summary>Copies an object from one location to another, or in-place.</summary>
    /// <param name="objectId">(Guid) Object to copy</param>
    /// <param name="translation">(Vector3d) Optional, additional translation vector to apply</param>
    /// <returns>(Guid) objectId for the copy.</returns>
    static member CopyObject(objectId:Guid, [<OPT;DEF(Vector3d())>]translation:Vector3d) : Guid =
        let translation =
            if not translation.IsZero then
                Transform.Translation(translation)
            else
                Transform.Identity
        let res = State.Doc.Objects.Transform(objectId, translation, deleteOriginal=false)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CopyObject failed.  objectId:'%s' translation:'%A'" (Pretty.str objectId) translation
        res



    /// <summary>Copies one or more objects from one location to another, or in-place.</summary>
    /// <param name="objectIds">(Guid seq) List of objects to copy</param>
    /// <param name="translation">(Vector3d) Optional, translation vector to apply to copied set</param>
    /// <returns>(Guid ResizeArray) Identifiers for the copies.</returns>
    static member CopyObjects(objectIds:Guid seq, [<OPT;DEF(Vector3d())>]translation:Vector3d) : Guid ResizeArray = //PLURAL
        let translation =
            if not translation.IsZero then
                Transform.Translation(translation)
            else
                Transform.Identity
        let rc = ResizeArray()
        for objectId in objectIds do
            let res = State.Doc.Objects.Transform(objectId, translation, deleteOriginal=false)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CopyObjects failed.  objectId:'%s' translation:'%A'" (Pretty.str objectId) translation
            rc.Add res
        rc


    /// <summary>Deletes a single object from the document.</summary>
    /// <param name="objectId">(Guid) Identifier of object to delete</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DeleteObject(objectId:Guid) : unit =
        if not <| State.Doc.Objects.Delete(objectId, quiet=true)  then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteObject failed on %s" (Pretty.str objectId)
        State.Doc.Views.Redraw()



    /// <summary>Deletes one or more objects from the document, Fails if not all objects can be deleted.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects to delete</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DeleteObjects(objectIds:Guid seq) : unit = //PLURAL
        let k = State.Doc.Objects.Delete(objectIds, quiet=true)
        let l = Seq.length objectIds
        if k <> l then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteObjects failed on %d out of %s" (l-k) (Pretty.str objectIds)
        State.Doc.Views.Redraw()


    /// <summary>Causes the selection state of one or more objects to change momentarily
    ///    so the object appears to flash on the screen.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects to flash</param>
    /// <param name="style">(bool) Optional, default value: <c>true</c>
    ///    If True, flash between object color and selection color.
    ///    If False, flash between visible and invisible</param>
    /// <returns>(unit).</returns>
    static member FlashObject(objectIds:Guid seq, [<OPT;DEF(true)>]style:bool) : unit =
        let rhobjs =  objectIds |> RArr.mapSeq RhinoScriptSyntax.CoerceRhinoObject
        if rhobjs.Count>0 then
            State.Doc.Views.FlashObjects(rhobjs, style)


    /// <summary>Hides a single object.</summary>
    /// <param name="objectId">(Guid) Id of object to hide</param>
    /// <returns>(bool) True of False indicating success or failure.</returns>
    static member HideObject(objectId:Guid) : bool =
        State.Doc.Objects.Hide(objectId, ignoreLayerMode=false)


    /// <summary>Hides one or more objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects to hide</param>
    /// <returns>(int) Number of objects hidden.</returns>
    static member HideObjects(objectIds:Guid seq) : int = //PLURAL
        let mutable rc = 0
        for objectId in objectIds do
            if State.Doc.Objects.Hide(objectId, ignoreLayerMode=false) then rc <- rc + 1
        if  rc>0 then State.Doc.Views.Redraw()
        rc


    /// <summary>Checks if an object is in on a page layout space. (as opposed to model space).</summary>
    /// <param name="objectId">(Guid) Id of an object to test</param>
    /// <returns>(bool) True if the object is in page layout space, False if the object is in model space.</returns>
    static member IsLayoutObject(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.Space = DocObjects.ActiveSpace.PageSpace


    /// <summary>Verifies the existence of an object in the State.Doc.Objects table. Fails on empty Guid.</summary>
    /// <param name="objectId">(Guid) An object to test</param>
    /// <returns>(bool) True if the object exists, False if the object does not exist.</returns>
    static member IsObject(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceRhinoObject(objectId) <> None


    /// <summary>Checks if an object is hidden. Hidden objects are not visible, cannot
    ///    be snapped to, and cannot be selected.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to test</param>
    /// <returns>(bool) True if the object is hidden, False if the object is not hidden.</returns>
    static member IsObjectHidden(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsHidden


    /// <summary>Verifies an object's bounding box is inside of another bounding box.</summary>
    /// <param name="objectId">(Guid) Identifier of an object to be tested</param>
    /// <param name="box">(Geometry.BoundingBox) Bounding box to test for containment</param>
    /// <param name="testMode">(bool) Optional, default value: <c>true</c>
    ///    If True, the object's bounding box must be contained by box
    ///    If False, the object's bounding box must be contained by or intersect box</param>
    /// <returns>(bool) True if object is inside box, False is object is not inside box.</returns>
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


    /// <summary>Checks if an object is a member of a group.</summary>
    /// <param name="objectId">(Guid) The identifier of an object</param>
    /// <param name="groupName">(string) Optional, The name of a group. If omitted, the function checks if the object is a member of any group</param>
    /// <returns>(bool) True if the object is a member of the specified group. If a groupName
    ///    was not specified, the object is a member of some group.
    ///    False if the object  is not a member of the specified group.
    ///    If a groupName was not specified, the object is not a member of any group.</returns>
    static member IsObjectInGroup(objectId:Guid, [<OPT;DEF(null:string)>]groupName:string) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let count = rhobj.GroupCount
        if count<1 then false
        else
          if isNull groupName then true
          else
            let index = State.Doc.Groups.Find(groupName)
            if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.IsObjectInGroup: '%s' group does not exist" groupName
            let groupids = rhobj.GetGroupList()
            groupids |> Seq.exists ((=) index )



    /// <summary>Checks if an object is locked. Locked objects are visible, and can
    ///    be snapped to, but cannot be selected.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to be tested</param>
    /// <returns>(bool) True if the object is locked, False if the object is not locked.</returns>
    static member IsObjectLocked(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsLocked


    /// <summary>Checks if an object is normal. Normal objects are visible, can be
    ///    snapped to, and can be selected.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to be tested</param>
    /// <returns>(bool) True if the object is normal, False if the object is not normal.</returns>
    static member IsObjectNormal(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsNormal


    /// <summary>Checks if an object is a reference object. Reference objects are
    ///    objects that are not part of the current document.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to test</param>
    /// <returns>(bool) True if the object is a reference object, False if the object is not a reference object.</returns>
    static member IsObjectReference(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsReference


    /// <summary>Checks if an object can be selected.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to test</param>
    /// <returns>(bool) True or False.</returns>
    static member IsObjectSelectable(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsSelectable( ignoreSelectionState=true,
                            ignoreGripsState    =false ,
                            ignoreLayerLocking  =false ,
                            ignoreLayerVisibility=false )


    /// <summary>Checks if an object is currently selected.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to test</param>
    /// <returns>(int)
    ///    0, the object is not selected
    ///    1, the object is selected
    ///    2, the object is entirely persistently selected
    ///    3, one or more proper sub-objects are selected.</returns>
    static member IsObjectSelected(objectId:Guid) : int =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.IsSelected(false)


    /// <summary>Determines if an object is closed or solid.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to test</param>
    /// <returns>(bool) True if the object is solid, or a Mesh is closed, False otherwise.</returns>
    static member IsObjectSolid(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        // rhobj.IsSolid //TODO  'rhobj.IsSolid'  could be used from RhCommon 7.5 onwards see https://github.com/mcneel/rhinoscriptsyntax/pull/197
        // and https://github.com/mcneel/rhinoscriptsyntax/commit/aaa0906fa43003db093c1e6f58bf12c813eddbeb
        let geom = rhobj.Geometry
        match geom with
        | :? Mesh      as m -> m.IsClosed
        | :? Extrusion as s -> s.IsSolid
        | :? Surface   as s -> s.IsSolid
        | :? Brep      as s -> s.IsSolid
        | :? SubD      as s -> s.IsSolid // only for Rh7 and higher
        | _                 ->
            RhinoScriptingException.Raise "RhinoScriptSyntax.IsObjectSolid only Mesh, Extrusion, Surface, Brep or SubD can be tested for solidity but not %s" (Pretty.str objectId)



    /// <summary>Verifies an object's geometry is valid and without error.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to test</param>
    /// <returns>(bool) True if the object is valid.</returns>
    static member IsObjectValid(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceRhinoObject(objectId) with
        |None -> false
        |Some rhobj ->  rhobj.IsValid


    /// <summary>Verifies an object is visible in a view.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to test</param>
    /// <param name="view">(string) Optional, default value: <c>ActiveView.MainViewport</c> The title of the view. If omitted, the current active view is used</param>
    /// <returns>(bool) True if the object is visible in the specified view, otherwise False.</returns>
    static member IsVisibleInView(objectId:Guid, [<OPT;DEF(null:string)>]view:string) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let viewport = if notNull view then (RhinoScriptSyntax.CoerceView(view)).MainViewport else State.Doc.Views.ActiveView.MainViewport
        let bbox = rhobj.Geometry.GetBoundingBox(true)
        rhobj.Visible && viewport.IsVisible(bbox)


    /// <summary>Locks a single object. Locked objects are visible, and they can be
    ///    snapped to. But, they cannot be selected.</summary>
    /// <param name="objectId">(Guid) The identifier of an object</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member LockObject(objectId:Guid) : bool =
        State.Doc.Objects.Lock(objectId, ignoreLayerMode=false)


    /// <summary>Locks multiple objects. Locked objects are visible, and they can be
    ///    snapped to. But, they cannot be selected.</summary>
    /// <param name="objectIds">(Guid seq) List of Strings or Guids. The identifiers of objects</param>
    /// <returns>(int) number of objects locked.</returns>
    static member LockObjects(objectIds:Guid seq) : int = //PLURAL
        let mutable rc = 0
        for objectId in objectIds do
            if State.Doc.Objects.Lock(objectId, ignoreLayerMode=false) then rc <- rc +   1
        if 0<> rc then State.Doc.Views.Redraw()
        rc


    /// <summary>Matches, or copies the attributes of a source object to a target object.</summary>
    /// <param name="targetIds">(Guid seq) Identifiers of objects to copy attributes to</param>
    /// <param name="sourceId">(Guid) Optional, Identifier of object to copy attributes from. If None,
    ///    then the default attributes are copied to the targetIds</param>
    /// <returns>(int) number of objects modified.</returns>
    static member MatchObjectAttributes(targetIds:Guid seq, [<OPT;DEF(Guid())>]sourceId:Guid) : int =
        let sourceAttr =
            if Guid.Empty <> sourceId then
                let source = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
                source.Attributes.Duplicate()
            else
                new DocObjects.ObjectAttributes()
        let mutable rc = 0
        for objectId in targetIds do
            if State.Doc.Objects.ModifyAttributes(objectId, sourceAttr, quiet=true) then
                rc <- rc +  1
        if 0 <> rc then State.Doc.Views.Redraw()
        rc


    /// <summary>Mirrors a single object on World XY Plane.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to mirror</param>
    /// <param name="startPoint">(Point3d) Start of the mirror Plane</param>
    /// <param name="endPoint">(Point3d) End of the mirror Plane</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>
    ///    Copy the object</param>
    /// <returns>(Guid) Identifier of the mirrored object.</returns>
    static member MirrorObject( objectId:Guid,
                                startPoint:Point3d,
                                endPoint:Point3d,
                                [<OPT;DEF(false)>]copy:bool) : Guid =
        let vec = endPoint-startPoint
        if vec.IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.MirrorObject Start and  end points are too close to each other.  objectId:'%s' startPoint:'%A' endPoint:'%A' copy:'%A'" (Pretty.str objectId) startPoint endPoint copy
        let normal = Plane.WorldXY.Normal
        let xv = Vector3d.CrossProduct(vec, normal)
        xv.Unitize() |> ignore<bool>
        let xf = Transform.Mirror(startPoint, vec)
        let res = State.Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.MirrorObject Cannot apply MirrorObject transform to objectId:'%s' startPoint:'%A' endPoint:'%A' copy:'%A'" (Pretty.str objectId) startPoint endPoint copy
        res


    /// <summary>Mirrors a list of objects on World XY Plane.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects to mirror</param>
    /// <param name="startPoint">(Point3d) Start of the mirror Plane</param>
    /// <param name="endPoint">(Point3d) End of the mirror Plane</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>
    ///    Copy the objects</param>
    /// <returns>(Guid ResizeArray) List of identifiers of the mirrored objects.</returns>
    static member MirrorObjects(  objectIds:Guid seq,
                                 startPoint:Point3d,
                                 endPoint:Point3d,
                                 [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray = //PLURAL
        let vec = endPoint-startPoint
        if vec.IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.MirrorObjects Start and  end points are too close to each other.  objectId:'%s' startPoint:'%A' endPoint:'%A' copy:'%A'" (Pretty.str objectIds) startPoint endPoint copy
        let normal = Plane.WorldXY.Normal
        let xv = Vector3d.CrossProduct(vec, normal)
        xv.Unitize() |> ignore<bool>
        let xf = Transform.Mirror(startPoint, vec)
        let rc = ResizeArray()
        for objectId in objectIds do
            let objectId = State.Doc.Objects.Transform(objectId, xf, not copy)
            if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.MirrorObjects Cannot apply MirrorObjects to objectId:'%s' startPoint:'%A' endPoint:'%A' copy:'%A'" (Pretty.str objectId) startPoint endPoint copy
            rc.Add objectId
        rc



    /// <summary>Moves a single object.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to move</param>
    /// <param name="translation">(Vector3d) Vector3d</param>
    /// <returns>(unit) void, nothing.</returns>
    static member MoveObject(objectId:Guid, translation:Vector3d) : unit = //TODO or return unit ??
        let xf = Transform.Translation(translation)
        let res = State.Doc.Objects.Transform(objectId, xf, deleteOriginal=true)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.MoveObject Cannot apply move to from objectId:'%s' translation:'%A'" (Pretty.str objectId) translation
        //if objectId <> res

    /// <summary>Moves one or more objects.</summary>
    /// <param name="objectIds">(Guid seq) The identifiers objects to move</param>
    /// <param name="translation">(Vector3d)Vector3d</param>
    /// <returns>(unit) void, nothing.</returns>
    static member MoveObjects(objectIds:Guid seq, translation:Vector3d) : unit =  //PLURAL
        let xf = Transform.Translation(translation)
        //let rc = ResizeArray()
        for objectId in objectIds do
            let res = State.Doc.Objects.Transform(objectId, xf, deleteOriginal=true)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.MoveObjects Cannot apply MoveObjects Transform to objectId:'%s'  translation:'%A'" (Pretty.str objectId) translation
            //rc.Add objectId
        //rc


    /// <summary>Returns the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed.</summary>
    /// <param name="objectId">(Guid)Id of object</param>
    /// <returns>(Drawing.Color) The current color value.</returns>
    static member ObjectColor(objectId:Guid) : Drawing.Color = //GET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoObject.Attributes.DrawColor(State.Doc)

    /// <summary>Modifies the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed.</summary>
    /// <param name="objectId">(Guid)Id of object</param>
    /// <param name="color">(Drawing.Color) The new color value</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let attr = rhobj.Attributes
        attr.ObjectColor <- color
        attr.ColorSource <- DocObjects.ObjectColorSource.ColorFromObject
        if not <| State.Doc.Objects.ModifyAttributes( rhobj, attr, quiet=true) then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectColor setting failed for %A; %A" (Pretty.str objectId) color
        State.Doc.Views.Redraw()

    /// <summary>Modifies the color of multiple objects. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed.</summary>
    /// <param name="objectIds">(Guid seq)Ids of objects</param>
    /// <param name="color">(Drawing.Color) The new color value</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectColor(objectIds:Guid seq, color:Drawing.Color) : unit = //MULTISET
        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let attr = rhobj.Attributes
            attr.ObjectColor <- color
            attr.ColorSource <- DocObjects.ObjectColorSource.ColorFromObject
            if not <| State.Doc.Objects.ModifyAttributes( rhobj, attr, quiet=true) then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectColor setting failed for %A; %A" (Pretty.str objectId) color
        State.Doc.Views.Redraw()




    /// <summary>Returns the color source of an object.</summary>
    /// <param name="objectId">(Guid) Single identifier</param>
    /// <returns>(int) The current color source
    ///    0 = color from layer
    ///    1 = color from object
    ///    2 = color from material
    ///    3 = color from parent.</returns>
    static member ObjectColorSource(objectId:Guid) : int = //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        int(rhobj.Attributes.ColorSource)

    /// <summary>Modifies the color source of an object.</summary>
    /// <param name="objectId">(Guid) Single identifier</param>
    /// <param name="source">(int) New color source
    ///    0 = color from layer
    ///    1 = color from object
    ///    2 = color from material
    ///    3 = color from parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectColorSource(objectId:Guid, source:int) : unit = //SET
        let source : DocObjects.ObjectColorSource = LanguagePrimitives.EnumOfValue source
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.ColorSource <- source
        rhobj.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the color source of multiple objects.</summary>
    /// <param name="objectIds">(Guid seq) Multiple identifiers</param>
    /// <param name="source">(int) New color source
    ///    0 = color from layer
    ///    1 = color from object
    ///    2 = color from material
    ///    3 = color from parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectColorSource(objectIds:Guid seq, source:int) : unit = //MULTISET
        let source : DocObjects.ObjectColorSource = LanguagePrimitives.EnumOfValue source
        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhobj.Attributes.ColorSource <- source
            rhobj.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()





    /// <summary>Returns a description of the object type (e.g. Line, Surface, Text,...).</summary>
    /// <param name="objectId">(Guid) Identifier of an object</param>
    /// <returns>(string) A short text description of the object.</returns>
    static member ObjectDescription(objectId:Guid) : string =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.ShortDescription(false)



    /// <summary>Returns the count for each object type in a List of objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects</param>
    /// <returns>(string) A short text description of the object.</returns>
    static member ObjectDescription(objectIds:Guid seq) : string =
        let count =  objectIds|> Seq.countBy (fun id ->
                        /// TODO could be optimized by using Object-type integer instead of string
                        let o = RhinoScriptSyntax.CoerceRhinoObject(id)
                        o.ShortDescription(true) + if o.IsDeleted then " (deleted !)" else ""
                        )
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



    /// <summary>Returns the short layer of an object.
    ///    Without Parent Layers.</summary>
    /// <param name="objectId">(Guid) The identifier of the object</param>
    /// <returns>(string) The object's current layer.</returns>
    static member ObjectLayerShort(objectId:Guid) : string = //GET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = obj.Attributes.LayerIndex
        State.Doc.Layers.[index].Name


    //static member ObjectLayer()// all 3 overloads moved to  top of file Scripting_Layer.fs

    /// <summary>Returns the layout or model space of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of the object</param>
    /// <returns>(string) The object's current page layout view, Empty String if it is in Model Space.</returns>
    static member ObjectLayout(objectId:Guid) : string = //GET
        // this fixes bug in rhinoscriptsyntax, see https://github.com/mcneel/rhinoscriptsyntax/pull/203
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if rhobj.Attributes.Space = DocObjects.ActiveSpace.PageSpace then
            let pageid = rhobj.Attributes.ViewportId
            let pageview = State.Doc.Views.Find(pageid)
            pageview.MainViewport.Name
        else
            ""


    /// <summary>Changes the layout or model space of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of the object</param>
    /// <param name="layout">(string) To change, or move, an object from model space to page
    ///    layout space, or from one page layout to another, then specify the
    ///    title of an existing page layout view. To move an object
    ///    from page layout space to model space, just specify an empty string.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectLayout(objectId:Guid, layout:string) : unit = //SET
        // this fixes bug in rhinoscriptsyntax, see https://github.com/mcneel/rhinoscriptsyntax/pull/203
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let view=
            if rhobj.Attributes.Space = DocObjects.ActiveSpace.PageSpace then
                let pageid = rhobj.Attributes.ViewportId
                let pageview = State.Doc.Views.Find(pageid)
                pageview.MainViewport.Name
            else
                ""

        if view<>layout then
            if layout="" then //move to model space
                rhobj.Attributes.Space <- DocObjects.ActiveSpace.ModelSpace
                rhobj.Attributes.ViewportId <- Guid.Empty
            else
                match State.Doc.Views.Find(layout, compareCase=false) with
                | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLayout: Setting it failed, layout not found for '%s' and '%A'"  layout objectId
                | :? Display.RhinoPageView as layout ->
                    rhobj.Attributes.ViewportId <- layout.MainViewport.Id
                    rhobj.Attributes.Space <- DocObjects.ActiveSpace.PageSpace
                | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLayout: Setting it failed, layout is not a Page view for '%s' and '%A'"  layout objectId

            rhobj.CommitChanges() |> ignore<bool>
            State.Doc.Views.Redraw()

    /// <summary>Changes the layout or model space of an objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifier of the objects</param>
    /// <param name="layout">(string) To change, or move, an objects from model space to page
    ///    layout space, or from one page layout to another, then specify the
    ///    title of an existing page layout view. To move an objects
    ///    from page layout space to model space, just specify an empty string.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectLayout(objectIds:Guid seq, layout:string) : unit = //MULTISET
        let lay =
            if layout<>"" then
                match State.Doc.Views.Find(layout, compareCase=false) with
                | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLayout: Setting it failed, layout not found for '%s' and '%A'"  layout objectIds
                | :? Display.RhinoPageView as layout -> Some layout
                | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLayout: Setting it failed, layout is not a Page view for '%s' and '%A'"  layout objectIds
            else
                None

        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let view=
                if rhobj.Attributes.Space = DocObjects.ActiveSpace.PageSpace then
                    let pageid = rhobj.Attributes.ViewportId
                    let pageview = State.Doc.Views.Find(pageid)
                    pageview.MainViewport.Name
                else
                    ""

            if view<>layout then
                if layout="" then //move to model space
                    rhobj.Attributes.Space <- DocObjects.ActiveSpace.ModelSpace
                    rhobj.Attributes.ViewportId <- Guid.Empty
                else
                    rhobj.Attributes.ViewportId <- lay.Value.MainViewport.Id
                    rhobj.Attributes.Space <- DocObjects.ActiveSpace.PageSpace

                rhobj.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()


    /// <summary>Returns the linetype of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <returns>(string) The object's current linetype.</returns>
    static member ObjectLinetype(objectId:Guid) : string = //GET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldIndex = State.Doc.Linetypes.LinetypeIndexForObject(rhinoObject)
        State.Doc.Linetypes.[oldIndex].Name

    /// <summary>Modifies the linetype of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <param name="linetype">(string) Name of an existing linetype</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectLinetype(objectId:Guid, linetype:string) : unit = //SET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let newIndex = State.Doc.Linetypes.Find(linetype)
        if newIndex <0 then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLinetype: Setting it failed for '%A' and '%A'"  linetype objectId
        rhinoObject.Attributes.LinetypeSource <- DocObjects.ObjectLinetypeSource.LinetypeFromObject
        rhinoObject.Attributes.LinetypeIndex <- newIndex
        rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the linetype of multiple object.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects</param>
    /// <param name="linetype">(string) Name of an existing linetype</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectLinetype(objectIds:Guid seq, linetype:string) : unit = //MULTISET
        let newIndex = State.Doc.Linetypes.Find(linetype)
        if newIndex <0 then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLinetype: Setting it failed for '%A' and '%A'"  linetype objectIds
        for objectId in objectIds do
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoObject.Attributes.LinetypeSource <- DocObjects.ObjectLinetypeSource.LinetypeFromObject
            rhinoObject.Attributes.LinetypeIndex <- newIndex
            rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()


    /// <summary>Returns the linetype source of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <returns>(int) The object's current linetype source
    ///      0 = By Layer
    ///      1 = By Object
    ///      3 = By Parent.</returns>
    static member ObjectLinetypeSource(objectId:Guid) : int = //GET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let oldSource = rhinoObject.Attributes.LinetypeSource
        int(oldSource)

    /// <summary>Modifies the linetype source of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <param name="source">(int) New linetype source.
    ///    If objectId is a list of identifiers, this parameter is required
    ///      0 = By Layer
    ///      1 = By Object
    ///      3 = By Parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectLinetypeSource(objectId:Guid, source:int) : unit = //SET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLinetypeSource: Setting it failed for '%A' and '%A'"  source objectId
        let source : DocObjects.ObjectLinetypeSource = LanguagePrimitives.EnumOfValue source
        rhinoObject.Attributes.LinetypeSource <- source
        rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the linetype source of multiple objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects</param>
    /// <param name="source">(int) New linetype source.
    ///    If objectId is a list of identifiers, this parameter is required
    ///      0 = By Layer
    ///      1 = By Object
    ///      3 = By Parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectLinetypeSource(objectIds:Guid seq, source:int) : unit = //MULTISET
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLinetypeSource: Setting it failed for '%A' and '%A'"  source objectIds
        let source : DocObjects.ObjectLinetypeSource = LanguagePrimitives.EnumOfValue source
        for objectId in objectIds do
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoObject.Attributes.LinetypeSource <- source
            rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()


    /// <summary>Returns the material index of an object. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    /// <param name="objectId">(Guid) Identifier of an object</param>
    /// <returns>(int) If the return value of ObjectMaterialSource is "material by object", then
    ///    the return value of this function is the index of the object's rendering
    ///    material. A material index of -1 indicates no material has been assigned,
    ///    and that Rhino's internal default material has been assigned to the object.</returns>
    static member ObjectMaterialIndex(objectId:Guid) : int = //GET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoObject.Attributes.MaterialIndex

    /// <summary>Changes the material index of an object. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    /// <param name="objectId">(Guid) Identifier of an object</param>
    /// <param name="materialIndex">(int) The new material index</param>
    static member ObjectMaterialIndex(objectId:Guid, materialIndex:int) : unit = //SET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if 0 <= materialIndex && materialIndex < State.Doc.Materials.Count then
            RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectMaterialIndex: Setting it failed for '%A' and '%A'"  materialIndex objectId
        let attrs = rhinoObject.Attributes
        attrs.MaterialIndex <- materialIndex
        if not <| State.Doc.Objects.ModifyAttributes(rhinoObject, attrs, quiet=true) then
            RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectMaterialIndex: Setting it failed for '%A' and '%A'"  materialIndex objectId

    /// <summary>Changes the material index multiple objects. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of an objects</param>
    /// <param name="materialIndex">(int) The new material index</param>
    static member ObjectMaterialIndex(objectIds:Guid seq, materialIndex:int) : unit = //MULTISET
        if 0 <= materialIndex && materialIndex < State.Doc.Materials.Count then
            RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectMaterialIndex: Setting it failed for '%A' and '%A'"  materialIndex objectIds
        for objectId in objectIds do
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let attrs = rhinoObject.Attributes
            attrs.MaterialIndex <- materialIndex
            if not <| State.Doc.Objects.ModifyAttributes(rhinoObject, attrs, quiet=true) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectMaterialIndex: Setting it failed for '%A' and '%A'"  materialIndex objectId

    /// <summary>Returns the rendering material source of an object.</summary>
    /// <param name="objectId">(Guid) One or more object identifiers</param>
    /// <returns>(int) The current rendering material source
    ///    0 = Material from layer
    ///    1 = Material from object
    ///    3 = Material from parent.</returns>
    static member ObjectMaterialSource(objectId:Guid) : int = //GET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        int(rhinoObject.Attributes.MaterialSource)


    /// <summary>Modifies the rendering material source of an object.</summary>
    /// <param name="objectId">(Guid) One or more object identifiers</param>
    /// <param name="source">(int) The new rendering material source.
    ///    0 = Material from layer
    ///    1 = Material from object
    ///    3 = Material from parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectMaterialSource(objectId:Guid, source:int) : unit = //SET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectMaterialSource: Setting it failed for '%A' and '%A'"  source objectId
        let source :DocObjects.ObjectMaterialSource  = LanguagePrimitives.EnumOfValue  source
        rhinoObject.Attributes.MaterialSource <- source
        rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the rendering material source of multiple objects.</summary>
    /// <param name="objectIds">(Guid seq) One or more objects identifiers</param>
    /// <param name="source">(int) The new rendering material source.
    ///    0 = Material from layer
    ///    1 = Material from objects
    ///    3 = Material from parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectMaterialSource(objectIds:Guid seq, source:int) : unit = //MULTISET
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectMaterialSource: Setting it failed for '%A' and '%A'"  source objectIds
        let source :DocObjects.ObjectMaterialSource  = LanguagePrimitives.EnumOfValue  source
        for objectId in objectIds do
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoObject.Attributes.MaterialSource <- source
            rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()


    /// <summary>Checks if a string is a good string for use in Rhino Object Names or User Dictionary key as and values.
    /// A good string may not include line returns, tabs, and leading or trailing whitespace.
    /// Confusing characters that look like ASCII but are some other Unicode are also not allowed. </summary>
    /// <param name="name">(string) The string to check.</param>
    /// <param name="allowEmpty">(bool) Optional, default value: <c>false</c> , set to true to make empty strings pass. </param>
    /// <param name="limitToAscii">(bool) Optional, default value: <c>false</c> , set to true to only allow chars between Unicode points 32 till 126 (ASCII) </param>
    /// <returns>(bool) true if the string is a valid name.</returns>
    static member IsGoodStringId    ( name:string
                                    , [<OPT;DEF(false)>]allowEmpty:bool
                                    , [<OPT;DEF(false)>]limitToAscii:bool
                                    ) : bool =
        if limitToAscii then
            Util.isASCIIStringId( name, allowEmpty)
        else
            Util.isGoodStringId( name, allowEmpty)



    /// <summary>Returns the name of an object or "" if none given.</summary>
    /// <param name="objectId">(Guid)Id of object</param>
    /// <returns>(string) The current object name, or empty string if no name given .</returns>
    static member ObjectName(objectId:Guid) : string = //GET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let n = rhinoObject.Attributes.Name
        if isNull n then ""
        else n

    /// <summary>Modifies the name of an object.</summary>
    /// <param name="objectId">(Guid)Id of object</param>
    /// <param name="name">(string) The new object name. Or empty string</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectName(objectId:Guid, name:string) : unit = //SET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if RhinoScriptSyntax.IsGoodStringId( name, allowEmpty=true) then
            rhinoObject.Attributes.Name <- name
            rhinoObject.CommitChanges() |> ignore<bool>
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectName: Setting it string '%s' cannot be used as Name. see RhinoScriptSyntax.IsGoodStringId. You can use RhinoCommon to bypass some of these restrictions." name

    /// <summary>Modifies the name of multiple objects.</summary>
    /// <param name="objectIds">(Guid seq)Id of objects</param>
    /// <param name="name">(string) The new objects name. Or empty string</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectName(objectIds:Guid seq, name:string) : unit = //MULTISET
        if RhinoScriptSyntax.IsGoodStringId( name, allowEmpty=true) then
            for objectId in objectIds do
                let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
                rhinoObject.Attributes.Name <- name
                rhinoObject.CommitChanges() |> ignore<bool>
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectName: Setting it string '%s' cannot be used as Name. see RhinoScriptSyntax.IsGoodStringId. You can use RhinoCommon to bypass some of these restrictions." name



    /// <summary>Returns the print color of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <returns>(Drawing.Color) The object's current print color.</returns>
    static member ObjectPrintColor(objectId:Guid) : Drawing.Color = //GET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoObject.Attributes.PlotColor



    /// <summary>Modifies the print color of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <param name="color">(Drawing.Color) New print color.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectPrintColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoObject.Attributes.PlotColorSource <- DocObjects.ObjectPlotColorSource.PlotColorFromObject
        rhinoObject.Attributes.PlotColor <- color
        rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the print color of multiple objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifier of objects</param>
    /// <param name="color">(Drawing.Color) New print color.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectPrintColor(objectIds:Guid seq, color:Drawing.Color) : unit = //MULTISET
        for objectId in objectIds do
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoObject.Attributes.PlotColorSource <- DocObjects.ObjectPlotColorSource.PlotColorFromObject
            rhinoObject.Attributes.PlotColor <- color
            rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Returns the print color source of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <returns>(int) The object's current print color source
    ///    0 = print color by layer
    ///    1 = print color by object
    ///    3 = print color by parent.</returns>
    static member ObjectPrintColorSource(objectId:Guid) : int = //GET
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            int(rhinoObject.Attributes.PlotColorSource)


    /// <summary>Modifies the print color source of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <param name="source">(int) New print color source
    ///    0 = print color by layer
    ///    1 = print color by object
    ///    3 = print color by parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectPrintColorSource(objectId:Guid, source:int) : unit = //SET
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectPrintColorSource: Setting it failed for '%A' and '%A'"  source objectId
        let source : DocObjects.ObjectPlotColorSource = LanguagePrimitives.EnumOfValue source
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhobj.Attributes.PlotColorSource <- source
        rhobj.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the print color source of multiple objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifier of objects</param>
    /// <param name="source">(int) New print color source
    ///    0 = print color by layer
    ///    1 = print color by objects
    ///    3 = print color by parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectPrintColorSource(objectIds:Guid seq, source:int) : unit = //MULTISET
        if source <0 || source >3 || source = 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectPrintColorSource: Setting it failed for '%A' and '%A'"  source objectIds
        let source : DocObjects.ObjectPlotColorSource = LanguagePrimitives.EnumOfValue source
        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhobj.Attributes.PlotColorSource <- source
            rhobj.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Returns the print width of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <returns>(float) The object's current print width.</returns>
    static member ObjectPrintWidth(objectId:Guid) : float = //GET
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoObject.Attributes.PlotWeight


    /// <summary>Modifies the print width of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <param name="width">(float) New print width value in millimeters, where width = 0.0 means use
    ///    the default width, and width smaller than 0.0 (e.g. -1.0)means do-not-print (visible for screen display,
    ///    but does not show on print)</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectPrintWidth(objectId:Guid, width:float) : unit = //SET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoObject.Attributes.PlotWeightSource <- DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
        rhinoObject.Attributes.PlotWeight <- width
        rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the print width of multiple objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifier of objects</param>
    /// <param name="width">(float) New print width value in millimeters, where width = 0.0 means use
    ///    the default width, and width smaller than 0.0 (e.g. -1.0)means do-not-print (visible for screen display,
    ///    but does not show on print)</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectPrintWidth(objectIds:Guid seq, width:float) : unit = //MULTISET
        for objectId in objectIds do
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoObject.Attributes.PlotWeightSource <- DocObjects.ObjectPlotWeightSource.PlotWeightFromObject
            rhinoObject.Attributes.PlotWeight <- width
            rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()


    /// <summary>Returns the print width source of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <returns>(int) The object's current print width source
    ///    0 = print width by layer
    ///    1 = print width by object
    ///    3 = print width by parent.</returns>
    static member ObjectPrintWidthSource(objectId:Guid) : int = //GET
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            int(rhinoObject.Attributes.PlotWeightSource)


    /// <summary>Modifies the print width source of an object.</summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <param name="source">(int) New print width source
    ///    0 = print width by layer
    ///    1 = print width by object
    ///    3 = print width by parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectPrintWidthSource(objectId:Guid, source:int) : unit = //SET
        let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        rhinoObject.Attributes.PlotWeightSource <- LanguagePrimitives.EnumOfValue source
        rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the print width source of multiple objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifier of objects</param>
    /// <param name="source">(int) New print width source
    ///    0 = print width by layer
    ///    1 = print width by objects
    ///    3 = print width by parent</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ObjectPrintWidthSource(objectIds:Guid seq, source:int) : unit = //MULTISET
        for objectId in objectIds do
            let rhinoObject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            rhinoObject.Attributes.PlotWeightSource <- LanguagePrimitives.EnumOfValue source
            rhinoObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()


    /// <summary>Returns the object type.</summary>
    /// <param name="objectId">(Guid) Identifier of an object</param>
    /// <returns>(int) The object type .
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
    static member ObjectType(objectId:Guid) : int =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let geom = rhobj.Geometry
        match geom with
        | :? Brep as b ->
            if b.Faces.Count = 1 then 8 //surface //TODO extrusion too?
            else int(geom.ObjectType)
        |_ -> int(geom.ObjectType)


    /// <summary>Orients a single object based on input points.
    /// If two 3-D points are specified, then this method will
    /// function similar to Rhino's Orient command.
    /// If more than two 3-D points are specified,
    /// then the function will orient similar to Rhino's Orient3Pt command.
    /// </summary>
    /// <param name="objectId">(Guid) Identifier of object</param>
    /// <param name="referencePts">(IList of Point3d) list of 3D reference points.</param>
    /// <param name="targetPts">(IList of Point3d) list of 3D target points.</param>
    /// <param name="flags">(int) The orient flags values can be added together to specify multiple options.
    ///    Value   Description
    ///    1       Copy object.  The default is not to copy the object.
    ///    2       Scale object.  The default is not to scale the object.  Note, the scale option only applies if both reference and target contain only two 3-D points.
    ///    3       Scale and copy.
    /// </param>
    /// <returns>(Guid) The identifier of the oriented object. The original or the copied object. Depending on given flags</returns>
    static member OrientObject( objectId:Guid,  referencePts:IList<Point3d>,  targetPts:IList<Point3d>,   [<OPT;DEF(0)>]flags:int) : Guid =
        if referencePts.Count <> targetPts.Count then
            RhinoScriptingException.Raise "RhinoScriptSyntax.OrientObject referencePts.Count <> targetPts.Count: %d <> %d " referencePts.Count targetPts.Count
        if referencePts.Count < 2 then
            RhinoScriptingException.Raise "RhinoScriptSyntax.OrientObject referencePts.Count < 2: %d  " referencePts.Count
        if targetPts.Count < 2 then
            RhinoScriptingException.Raise "RhinoScriptSyntax.OrientObject targetPts.Count < 2 : %d"  targetPts.Count

        let copy  = (flags &&& 1) = 1
        let scale = (flags &&& 2) = 2
        let xform =
            if referencePts.Count > 2 then
                //Orient3Pt
                let fromPlane = Rhino.Geometry.Plane(referencePts.[0], referencePts.[1], referencePts.[2])
                let toPlane = Rhino.Geometry.Plane(targetPts.[0], targetPts.[1], targetPts.[2])
                if not fromPlane.IsValid || not toPlane.IsValid then
                    RhinoScriptingException.Raise "RhinoScriptSyntax.OrientObject unable to create valid planes from point lists %A and %A " referencePts targetPts
                Rhino.Geometry.Transform.PlaneToPlane(fromPlane, toPlane)
            else
                //Orient2Pt
                let xformMove = Rhino.Geometry.Transform.Translation( targetPts.[0]-referencePts.[0] )
                let v0 = referencePts.[1] - referencePts.[0]
                let v1 = targetPts.[1] - targetPts.[0]
                let xformScale =
                    if scale then
                        let len0 = v0.Length
                        let len1 = v1.Length
                        if len0 < 0.000001 || len1 < 0.000001 then RhinoScriptingException.Raise "RhinoScriptSyntax.OrientObject vector lengths too short"
                        let scaleF = len1 / len0
                        if abs(1.0-scaleF) >= 0.000001 then
                            let plane = Rhino.Geometry.Plane(referencePts.[0], v0)
                            Rhino.Geometry.Transform.Scale(plane, scaleF, scaleF, scaleF)
                        else
                            Rhino.Geometry.Transform.Identity
                    else
                        Rhino.Geometry.Transform.Identity
                v0.Unitize()  |> ignore<bool>
                v1.Unitize()  |> ignore<bool>
                let xformRotate = Rhino.Geometry.Transform.Rotation(v0, v1, referencePts.[0])
                xformMove * xformScale * xformRotate

        let rc = RhinoScriptSyntax.Ot.Transform(objectId, xform, not copy)
        if rc=System.Guid.Empty then
            RhinoScriptingException.Raise "RhinoScriptSyntax.OrientObject failed"
        RhinoScriptSyntax.Doc.Views.Redraw()
        rc


    /// <summary>Rotates a single object.</summary>
    /// <param name="objectId">(Guid) The identifier of an object to rotate</param>
    /// <param name="centerPoint">(Point3d) The center of rotation</param>
    /// <param name="rotationAngle">(float) In degrees</param>
    /// <param name="axis">(Vector3d) Optional, default value: <c>Vector3d.ZAxis</c>
    ///    Axis of rotation, If omitted, the Vector3d.ZAxis is used as the rotation axis</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>. Copy the object</param>
    /// <returns>(Guid) Identifier of the rotated object.</returns>
    static member RotateObject( objectId:Guid,
                                centerPoint:Point3d,
                                rotationAngle:float,
                                [<OPT;DEF(Vector3d())>]axis:Vector3d,
                                [<OPT;DEF(false)>]copy:bool) : Guid =
        let axis = if axis.IsZero then Vector3d.ZAxis else axis
        let rotationAngle = RhinoMath.ToRadians(rotationAngle)
        let xf = Transform.Rotation(rotationAngle, axis, centerPoint)
        let res = State.Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.RotateObject failed.  objectId:'%s' centerPoint:'%A' rotationAngle:'%A' axis:'%A' copy:'%A'" (Pretty.str objectId) centerPoint rotationAngle axis copy
        res


    /// <summary>Rotates multiple objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects to rotate</param>
    /// <param name="centerPoint">(Point3d) The center of rotation</param>
    /// <param name="rotationAngle">(float) In degrees</param>
    /// <param name="axis">(Vector3d) Optional, default value: <c>Vector3d.ZAxis</c>
    ///    Axis of rotation, If omitted, the Vector3d.ZAxis is used as the rotation axis</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>. Copy the object</param>
    /// <returns>(Guid ResizeArray) identifiers of the rotated objects.</returns>
    static member RotateObjects( objectIds:Guid seq,
                                 centerPoint:Point3d,
                                 rotationAngle:float,
                                 [<OPT;DEF(Vector3d())>]axis:Vector3d,
                                 [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray = //PLURAL
        let axis = if axis.IsZero then Vector3d.ZAxis else axis
        let rotationAngle = RhinoMath.ToRadians(rotationAngle)
        let xf = Transform.Rotation(rotationAngle, axis, centerPoint)
        let rc = ResizeArray()
        for objectId in objectIds do
            let res = State.Doc.Objects.Transform(objectId, xf, not copy)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.RotateObjects failed.  objectId:'%s' centerPoint:'%A' rotationAngle:'%A' axis:'%A' copy:'%A'" (Pretty.str objectId) centerPoint rotationAngle axis copy
            rc.Add res
        rc


    /// <summary>Scales a single object. Can be used to perform a uniform or non-uniform
    ///    scale transformation. Scaling is based on the WorldXY Plane.</summary>
    /// <param name="objectId">(Guid) The identifier of an object</param>
    /// <param name="origin">(Point3d) The origin of the scale transformation</param>
    /// <param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>.Copy the object</param>
    /// <returns>(Guid) Identifier of the scaled object.</returns>
    static member ScaleObject( objectId:Guid,
                               origin:Point3d,
                               scale:float*float*float,
                               [<OPT;DEF(false)>]copy:bool) : Guid =
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let x, y, z = scale
        let xf = Transform.Scale(plane, x, y, z)
        let res = State.Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ScaleObject failed.  objectId:'%s' origin:'%A' scale:'%A' copy:'%A'" (Pretty.str objectId) origin scale  copy
        res

    /// <summary>Scales a single object. Uniform scale transformation. Scaling is based on the WorldXY Plane.</summary>
    /// <param name="objectId">(Guid) The identifier of an object</param>
    /// <param name="origin">(Point3d) The origin of the scale transformation</param>
    /// <param name="scale">(float) One numbers that identify the X, Y, and Z axis scale factors to apply</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>. Copy the object</param>
    /// <returns>(Guid) Identifier of the scaled object.</returns>
    static member ScaleObject( objectId:Guid,
                               origin:Point3d,
                               scale:float,
                               [<OPT;DEF(false)>]copy:bool) : Guid = //ALT
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let xf = Transform.Scale(plane, scale, scale, scale)
        let res = State.Doc.Objects.Transform(objectId, xf, not copy)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ScaleObject failed.  objectId:'%s' origin:'%A' scale:'%A' copy:'%A'" (Pretty.str objectId) origin scale  copy
        res

    /// <summary>Scales one or more objects. Can be used to perform a uniform or non-
    ///    uniform scale transformation. Scaling is based on the WorldXY Plane.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects to scale</param>
    /// <param name="origin">(Point3d) The origin of the scale transformation</param>
    /// <param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>. Copy the objects</param>
    /// <returns>(Guid ResizeArray) identifiers of the scaled objects.</returns>
    static member ScaleObjects( objectIds:Guid seq,
                                origin:Point3d,
                                scale:float*float*float,
                                [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =  //PLURAL
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let x, y, z = scale
        let xf = Transform.Scale(plane, x, y, z)
        let rc = ResizeArray()
        for objectId in objectIds do
            let res = State.Doc.Objects.Transform(objectId, xf, not copy)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ScaleObjects failed.  objectId:'%s' origin:'%s' scale:'%A' copy:'%b'" (Pretty.str objectId) origin.Pretty scale  copy
            rc.Add res
        rc

    /// <summary>Scales one or more objects. Uniform scale transformation. Scaling is based on the WorldXY Plane.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects to scale</param>
    /// <param name="origin">(Point3d) The origin of the scale transformation</param>
    /// <param name="scale">(float) One numbers that identify the X, Y, and Z axis scale factors to apply</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>
    ///    Copy the objects</param>
    /// <returns>(Guid ResizeArray) identifiers of the scaled objects.</returns>
    static member ScaleObjects( objectIds:Guid seq,
                                origin:Point3d,
                                scale:float,
                                [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =  //PLURAL ALT
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let xf = Transform.Scale(plane, scale, scale, scale)
        let rc = ResizeArray()
        for objectId in objectIds do
            let res = State.Doc.Objects.Transform(objectId, xf, not copy)
            if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ScaleObjects failed.  objectId:'%s' origin:'%A' scale:'%A' copy:'%A'" (Pretty.str objectId) origin scale  copy
            rc.Add res
        rc



    /// <summary>Selects a single object.
    /// Throws an exception if object can't be selected for some reason
    ///  e.g. when locked , hidden, or on invisible layer .</summary>
    /// <param name="objectId">(Guid) The identifier of the object to select</param>
    /// <param name="forceVisible">(bool) Optional, default value: <c>false</c> whether to make objects that a hidden or layers that are off visible and unlocked </param>
    /// <param name="ignoreErrors">(bool) Optional, default value: <c>false</c> whether to ignore errors when object can be set visible </param>
    /// <returns>(unit) void, nothing.</returns>
    static member SelectObject( objectId:Guid,
                                [<OPT;DEF(false)>]forceVisible:bool,
                                [<OPT;DEF(false)>]ignoreErrors:bool) : unit =
        RhinoSync.DoSync (fun () ->
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if 0 = rhobj.Select(true) then
                if not ignoreErrors then
                    let mutable redo = false
                    let lay = State.Doc.Layers.[rhobj.Attributes.LayerIndex]
                    if rhobj.IsHidden then
                        if forceVisible then redo <- true ; State.Doc.Objects.Show(rhobj, ignoreLayerMode=true) |> ignore<bool>
                        else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on hidden object %s" (Pretty.str objectId)
                    elif rhobj.IsLocked then
                        if forceVisible then redo <- true ; State.Doc.Objects.Unlock(rhobj, ignoreLayerMode=true) |> ignore<bool>
                        else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on locked object %s" (Pretty.str objectId)
                    elif not lay.IsVisible then
                        if forceVisible then redo <- true ; UtilLayer.visibleSetTrue(lay, true)
                        else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on invisible layer %s for object %s" lay.FullPath (Pretty.str objectId)
                    elif not lay.IsLocked then
                        if forceVisible then redo <- true ; UtilLayer.lockedSetFalse(lay, true)
                        else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on locked layer %s for object %s" lay.FullPath (Pretty.str objectId)
                    else
                        RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed on object %s" (Pretty.str objectId)
                    if redo then
                        if 0 = rhobj.Select(true) then
                            RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObject failed despite forceVisible being set to true on object %s" (Pretty.str objectId)
            State.Doc.Views.Redraw()
            )

    /// <summary>Selects one or more objects
    /// Throws an exception if object can't be selected for some reason
    ///  e.g. when locked , hidden, or on invisible layer .</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of the objects to select</param>
    /// <param name="forceVisible">(bool) Optional, default value: <c>false</c> whether to make objects that a hidden or layers that are off visible and unlocked </param>
    /// <param name="ignoreErrors">(bool) Optional, default value: <c>false</c> whether to ignore errors when object can be set visible </param>
    /// <returns>(unit) void, nothing.</returns>
    static member SelectObjects( objectIds:Guid seq,
                                [<OPT;DEF(false)>]forceVisible:bool,
                                [<OPT;DEF(false)>]ignoreErrors:bool) : unit =  //PLURAL
        RhinoSync.DoSync (fun () ->
            for objectId in objectIds do
                let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
                if 0 = rhobj.Select(true) then
                    if not ignoreErrors then
                        let mutable redo = false
                        let lay = State.Doc.Layers.[rhobj.Attributes.LayerIndex]
                        if rhobj.IsHidden then
                            if forceVisible then redo <- true ; State.Doc.Objects.Show(rhobj, ignoreLayerMode=true) |> ignore<bool>
                            else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObjects failed on hidden object %s out of %d objects" (Pretty.str objectId) (Seq.length objectIds)
                        elif rhobj.IsLocked then
                            if forceVisible then redo <- true ; State.Doc.Objects.Unlock(rhobj, ignoreLayerMode=true) |> ignore<bool>
                            else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObjects failed on locked object %s out of %d objects" (Pretty.str objectId) (Seq.length objectIds)
                        elif not lay.IsVisible then
                            if forceVisible then redo <- true ; UtilLayer.visibleSetTrue(lay, true)
                            else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObjects failed on invisible layer %s for object %s out of %d objects" lay.FullPath (Pretty.str objectId) (Seq.length objectIds)
                        elif not lay.IsLocked then
                            if forceVisible then redo <- true ; UtilLayer.lockedSetFalse(lay, true)
                            else RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObjects failed on locked layer %s for object %s out of %d objects" lay.FullPath (Pretty.str objectId) (Seq.length objectIds)
                        else
                            RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObjects failed on object %s out of %d objects" (Pretty.str objectId) (Seq.length objectIds)
                        if redo then
                            if 0 = rhobj.Select(true) then
                                RhinoScriptingException.Raise "RhinoScriptSyntax.SelectObjects failed despite forceVisible being set to true on object %s out of %d objects" (Pretty.str objectId) (Seq.length objectIds)
            State.Doc.Views.Redraw()
            )


    /// <summary>Perform a shear transformation on a single object.</summary>
    /// <param name="objectId">(Guid) The identifier of an object</param>
    /// <param name="origin">(Point3d) Origin point of the shear transformation</param>
    /// <param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    /// <param name="angleDegrees">(float) The shear angle in degrees</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>
    ///    Copy the objects</param>
    /// <returns>(Guid) Identifier of the sheared object.</returns>
    static member ShearObject( objectId:Guid,
                               origin:Point3d,
                               referencePoint:Point3d,
                               angleDegrees:float,
                               [<OPT;DEF(false)>]copy:bool) : Guid =
       if (origin-referencePoint).IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.ShearObject failed because (origin-referencePoint).IsTiny() : %s and %s" origin.Pretty referencePoint.Pretty
       let plane = State.Doc.Views.ActiveView.MainViewport.ConstructionPlane()
       let mutable frame = Plane(plane)
       frame.Origin <- origin
       frame.ZAxis <- plane.Normal
       let yAxis = referencePoint-origin
       yAxis.Unitize() |> ignore<bool>
       frame.YAxis <- yAxis
       let xAxis = Vector3d.CrossProduct(frame.ZAxis, frame.YAxis)
       xAxis.Unitize() |> ignore<bool>
       frame.XAxis <- xAxis
       let worldPlane = Plane.WorldXY
       let cob = Transform.ChangeBasis(worldPlane, frame)
       let mutable shear2d = Transform.Identity
       shear2d.[0, 1] <- tan(toRadians(angleDegrees))
       let cobinv = Transform.ChangeBasis(frame, worldPlane)
       let xf = cobinv * shear2d * cob
       let res = State.Doc.Objects.Transform(objectId, xf, not copy)
       if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ShearObject failed for %s, origin %s, ref point  %s and angle in Deg  %f" (Pretty.str objectId) origin.Pretty referencePoint.Pretty angleDegrees
       res


    /// <summary>Shears one or more objects.</summary>
    /// <param name="objectIds">(Guid seq) The identifiers objects to shear</param>
    /// <param name="origin">(Point3d) Origin point of the shear transformation</param>
    /// <param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    /// <param name="angleDegrees">(float) The shear angle in degrees</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>
    ///    Copy the objects</param>
    /// <returns>(Guid ResizeArray) identifiers of the sheared objects.</returns>
    static member ShearObjects( objectIds:Guid seq,
                                origin:Point3d,
                                referencePoint:Point3d,
                                angleDegrees:float,
                                [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray = //PLURAL
        if (origin-referencePoint).IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.ShearObjects failed because (origin-referencePoint).IsTiny() : %s and %s" origin.Pretty referencePoint.Pretty
        let plane = State.Doc.Views.ActiveView.MainViewport.ConstructionPlane()
        let mutable frame = Plane(plane)
        frame.Origin <- origin
        frame.ZAxis <- plane.Normal
        let yAxis = referencePoint-origin
        yAxis.Unitize() |> ignore<bool>
        frame.YAxis <- yAxis
        let xAxis = Vector3d.CrossProduct(frame.ZAxis, frame.YAxis)
        xAxis.Unitize() |> ignore<bool>
        frame.XAxis <- xAxis
        let worldPlane = Plane.WorldXY
        let cob = Transform.ChangeBasis(worldPlane, frame)
        let mutable shear2d = Transform.Identity
        shear2d.[0, 1] <- tan(toRadians(angleDegrees))
        let cobinv = Transform.ChangeBasis(frame, worldPlane)
        let xf = cobinv * shear2d * cob
        let r = ResizeArray()
        for ob in objectIds do
            let res = State.Doc.Objects.Transform(ob, xf, not copy)
            if res = Guid.Empty then
                RhinoScriptingException.Raise "RhinoScriptSyntax.ShearObjects failed for %s, origin %s, ref point  %s and angle in Deg  %f" (Pretty.str ob) origin.Pretty referencePoint.Pretty angleDegrees
            else
                r.Add res
        r


    /// <summary>Shows a previously hidden object. Hidden objects are not visible, cannot
    ///    be snapped to and cannot be selected.</summary>
    /// <param name="objectId">(Guid) Representing id of object to show</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ShowObject(objectId:Guid) : unit =
        if not <| State.Doc.Objects.Show(objectId, ignoreLayerMode=false) then RhinoScriptingException.Raise "RhinoScriptSyntax.ShowObject failed on %s" (Pretty.str objectId)
        State.Doc.Views.Redraw()


    /// <summary>Shows one or more objects. Hidden objects are not visible, cannot be
    ///    snapped to and cannot be selected.</summary>
    /// <param name="objectIds">(Guid seq) Ids of objects to show.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ShowObjects(objectIds:Guid seq) : unit = //PLURAL
        for objectId in objectIds do
            if not <| State.Doc.Objects.Show(objectId, ignoreLayerMode=false) then RhinoScriptingException.Raise "RhinoScriptSyntax.ShowObjects failed on %s" (Pretty.str objectId)
        State.Doc.Views.Redraw()


    /// <summary>Unlocks an object. Locked objects are visible, and can be snapped to,
    ///    but they cannot be selected.</summary>
    /// <param name="objectId">(Guid) The identifier of an object</param>
    /// <returns>(unit) void, nothing.</returns>
    static member UnlockObject(objectId:Guid) : unit =
        if not <| State.Doc.Objects.Unlock(objectId, ignoreLayerMode=false) then RhinoScriptingException.Raise "RhinoScriptSyntax.UnlockObject failed on %s" (Pretty.str objectId)
        State.Doc.Views.Redraw()

    /// <summary>Unlocks one or more objects. Locked objects are visible, and can be
    ///    snapped to, but they cannot be selected.</summary>
    /// <param name="objectIds">(Guid seq) The identifiers of objects</param>
    /// <returns>(unit) void, nothing.</returns>
    static member UnlockObjects(objectIds:Guid seq) : unit =  //PLURAL
        for objectId in objectIds do
            if not <| State.Doc.Objects.Unlock(objectId, ignoreLayerMode=false) then RhinoScriptingException.Raise "RhinoScriptSyntax.UnlockObjects failed on %s" (Pretty.str objectId)
        State.Doc.Views.Redraw()


    /// <summary>Unselects a single selected object.</summary>
    /// <param name="objectId">(Guid) Id of object to unselect.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member UnselectObject(objectId:Guid) : unit =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if 0 <> obj.Select(false) then RhinoScriptingException.Raise "RhinoScriptSyntax.UnselectObject failed on %s" (Pretty.str objectId)
        State.Doc.Views.Redraw()


    /// <summary>Unselects multiple selected objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of the objects to unselect.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member UnselectObjects(objectIds:Guid seq) : unit = //PLURAL
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if 0 <> obj.Select(false) then RhinoScriptingException.Raise "RhinoScriptSyntax.UnselectObjects failed on %s" (Pretty.str objectId)
        State.Doc.Views.Redraw()

