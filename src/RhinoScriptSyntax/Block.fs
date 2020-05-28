namespace Rhino.Scripting.Modules

open System
open Rhino
open Rhino.Geometry
open FsEx.Util

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx
open FsEx.SaveIgnore

[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting Namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsBlock =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with


    [<Extension>]
    ///<summary>Adds a new block definition to the document</summary>
    ///<param name="objectIds">(Guid seq) Objects that will be included in the block</param>
    ///<param name="basePoint">(Point3d) 3D base point for the block definition</param>
    ///<param name="name">(string) Optional, Default Value: <c>InstanceDefinitions.GetUnusedInstanceDefinitionName()</c>
    ///    Name of the block definition. If omitted a name will be automatically generated</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    If True, the objectIds will be deleted</param>
    ///<returns>(string) name of new block definition</returns>
    static member AddBlock(objectIds:Guid seq, basePoint:Point3d, [<OPT;DEF("")>]name:string, [<OPT;DEF(false)>]deleteInput:bool) : string =
        let name = if name="" then Doc.InstanceDefinitions.GetUnusedInstanceDefinitionName() else name
        let found = Doc.InstanceDefinitions.Find(name)
        let objects = ResizeArray()
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)  //Coerce should not be needed
            if obj.IsReference then  failwithf "AddBlock: cannt add Refrence Object %A to %s" objectId name
            let ot = obj.ObjectType
            if   ot= DocObjects.ObjectType.Light then  failwithf "AddBlock: cannot add Light Object %A to %s" objectId name
            elif ot= DocObjects.ObjectType.Grip then  failwithf "AddBlock: cannot add Grip Object %A to %s" objectId name
            elif ot= DocObjects.ObjectType.Phantom then failwithf "AddBlock: cannot add Phantom Object %A to %s" objectId name
            elif ot= DocObjects.ObjectType.InstanceReference && notNull found then
                let bli = RhinoScriptSyntax.CoerceBlockInstanceObject(objectId) // not obj ?
                let uses, nesting = bli.UsesDefinition(found.Index)
                if uses then failwithf "AddBlock: cannt add Instance Ref Object %A to %s" objectId name

            objects.Add(obj)
        if objects.Count>0 then
            let geometry = [ for obj in objects -> obj.Geometry]
            let attrs = [ for obj in objects -> obj.Attributes]
            let mutable rc = -1
            if notNull found then
              rc <- if Doc.InstanceDefinitions.ModifyGeometry(found.Index, geometry, attrs) then 0 else -1
            else
              rc <- Doc.InstanceDefinitions.Add(name, "", basePoint, geometry, attrs)
            if rc >= 0 then
                if deleteInput then
                    for obj in objects do Doc.Objects.Delete(obj, true) |>ignore
                Doc.Views.Redraw()
        name






    [<Extension>]
    ///<summary>Returns names of the block definitions that contain a specified block
    ///    definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string ResizeArray) A list of block definition names</returns>
    static member BlockContainers(blockName:string) : string ResizeArray =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let containers = idef.GetContainers()
        let rc = ResizeArray()
        for item in containers do
            if not <| item.IsDeleted then  rc.Add(item.Name)
        rc

    [<Extension>]
    ///<summary>Returns number of block definitions that contain a specified
    ///    block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(int) the number of block definitions that contain a specified block definition</returns>
    static member BlockContainerCount(blockName:string) : int =
        (RhinoScriptSyntax.BlockContainers(blockName)).Count


    [<Extension>]
    ///<summary>Returns the number of block definitions in the document</summary>
    ///<returns>(int) the number of block definitions in the document</returns>
    static member BlockCount() : int =
        Doc.InstanceDefinitions.ActiveCount


    [<Extension>]
    ///<summary>Returns the description of a block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string) The current description</returns>
    static member BlockDescription(blockName:string) : string = //GET
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.Description

    [<Extension>]
    ///<summary>Sets the description of a block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="description">(string) The new description</param>
    ///<returns>(unit) void, nothing</returns>
    static member BlockDescription(blockName:string, description:string) : unit = //SET
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        Doc.InstanceDefinitions.Modify( idef, idef.Name, description, true ) |>ignore


    [<Extension>]
    ///<summary>Counts number of instances of the block in the document.
    ///    Nested instances are not included in the count. Attention this may include deleted blocks</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///    0 = get top level references in active document.
    ///    1 = get top level and nested references in active document.
    ///      If a block is nested more than once within another block it will be counted only once.
    ///    2 = check for references from other instance definitions, counts every instance of nested block</param>
    ///<returns>(int) the number of instances of the block in the document</returns>
    static member BlockInstanceCount(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : int =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let  refs = idef.GetReferences(whereToLook)
        refs.Length


    [<Extension>]
    ///<summary>Returns the insertion point of a block instance</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Point3d) The insertion 3D point</returns>
    static member BlockInstanceInsertPoint(objectId:Guid) : Point3d =
        let  instance = RhinoScriptSyntax.CoerceBlockInstanceObject(objectId)
        let  xf = instance.InstanceXform
        let  pt = Geometry.Point3d.Origin
        pt.Transform(xf)
        pt


    [<Extension>]
    ///<summary>Returns the block name of a block instance</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(string) the block name of a block instance</returns>
    static member BlockInstanceName(objectId:Guid) : string =
        let mutable instance = RhinoScriptSyntax.CoerceBlockInstanceObject(objectId)
        let mutable idef = instance.InstanceDefinition
        idef.Name


    [<Extension>]
    ///<summary>Returns the identifiers of the inserted instances of a block</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///    0 = get top level references in active document.
    ///    1 = get top level and nested references in active document.
    ///    2 = check for references from other instance definitions</param>
    ///<returns>(Guid ResizeArray) Ids identifying the instances of a block in the model</returns>
    static member BlockInstances(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : ResizeArray<Guid> =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let instances = idef.GetReferences(0)
        resizeArray { for item in instances do yield item.Id }


    [<Extension>]
    ///<summary>Returns the location of a block instance relative to the world coordinate
    ///    system origin (0, 0, 0). The position is returned as a 4x4 transformation
    ///    matrix</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Transform) the location, as a transform matrix, of a block instance relative to the world coordinate
    ///    system origin</returns>
    static member BlockInstanceXform(objectId:Guid) : Transform =
        let  instance = RhinoScriptSyntax.CoerceBlockInstanceObject(objectId)
        instance.InstanceXform


    [<Extension>]
    ///<summary>Returns the names of all block definitions in the document</summary>
    ///<returns>(string ResizeArray) the names of all block definitions in the document</returns>
    static member BlockNames() : string ResizeArray =
        let  ideflist = Doc.InstanceDefinitions.GetList(true)
        resizeArray { for item in ideflist do yield item.Name}



    [<Extension>]
    ///<summary>Returns number of objects that make up a block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(int) the number of objects that make up a block definition</returns>
    static member BlockObjectCount(blockName:string) : int =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.ObjectCount


    [<Extension>]
    ///<summary>Returns identifiers of the objects that make up a block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(Guid ResizeArray) list of identifiers</returns>
    static member BlockObjects(blockName:string) : ResizeArray<Guid> =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let  rhobjs = idef.GetObjects()
        resizeArray { for obj in rhobjs -> obj.Id}


    [<Extension>]
    ///<summary>Returns path to the source of a linked or embedded block definition.
    ///    A linked or embedded block definition is a block definition that was
    ///    inserted from an external file</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(string) path to the linked block</returns>
    static member BlockPath(blockName:string) : string =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.SourceArchive


    [<Extension>]
    ///<summary>Returns the status of a linked block</summary>
    ///<param name="blockName">(string) Name of an existing block</param>
    ///<returns>(int) the status of a linked block
    ///    Value Description
    ///    -3    Not a linked block definition.
    ///    -2    The linked block definition's file could not be opened or could not be read.
    ///    -1    The linked block definition's file could not be found.
    ///      0    The linked block definition is up-to-date.
    ///      1    The linked block definition's file is newer than definition.
    ///      2    The linked block definition's file is older than definition.
    ///      3    The linked block definition's file is different than definition</returns>
    static member BlockStatus(blockName:string) : int =
        let  idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  -3
        else int(idef.ArchiveFileStatus)


    [<Extension>]
    ///<summary>Deletes a block definition and all of it's inserted instances</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteBlock(blockName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let  rc = Doc.InstanceDefinitions.Delete(idef.Index, true, false)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Explodes a block instance into it's geometric components. The
    ///    exploded objects are added to the document</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<param name="explodeNestedInstances">(bool) Optional, Default Value: <c>false</c>
    ///    By default nested blocks are not exploded</param>
    ///<returns>(Guid array) identifiers for the newly exploded objects</returns>
    static member ExplodeBlockInstance(objectId:Guid, [<OPT;DEF(false)>]explodeNestedInstances:bool) : array<Guid> =
        let  instance = RhinoScriptSyntax.CoerceBlockInstanceObject(objectId)
        let  guids = Doc.Objects.AddExplodedInstancePieces(instance, explodeNestedInstances, deleteInstance= true)
        if guids.Length > 0 then Doc.Views.Redraw()
        guids



    [<Extension>]
    ///<summary>Inserts a block whose definition already exists in the document</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="xform">(Transform) 4x4 transformation matrix to apply</param>
    ///<returns>(Guid) objectId for the block that was added to the doc</returns>
    static member InsertBlock2(blockName:string, xform:Transform) : Guid =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let objectId = Doc.Objects.AddInstanceObject(idef.Index, xform )
        if objectId<>System.Guid.Empty then
            Doc.Views.Redraw()
        objectId


    [<Extension>]
    ///<summary>Inserts a block whose definition already exists in the document</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="insertionPoint">(Point3d) Insertion point for the block</param>
    ///<param name="scale">(Vector3d) Optional, Default Value: <c>Vector3d(1.0 , 1.0 , 1.0)</c>
    ///    X, y, z scale factors</param>
    ///<param name="angleDegrees">(float) Optional, Default Value: <c>0</c>
    ///    Rotation angle in degrees</param>
    ///<param name="rotationNormal">(Vector3d) Optional, Default Value: <c> Vector3d.ZAxis</c>
    ///    The axis of rotation</param>
    ///<returns>(Guid) objectId for the block that was added to the doc</returns>
    static member InsertBlock(blockName:string, insertionPoint:Point3d, [<OPT;DEF(Vector3d())>]scale:Vector3d, [<OPT;DEF(0.0)>]angleDegrees:float, [<OPT;DEF(Vector3d())>]rotationNormal:Vector3d) : Guid =
        let angleRadians = UtilMath.toRadians(angleDegrees)
        let sc= if scale.IsZero then Vector3d(1. , 1. , 1.) else scale
        let rotationNormal0= if rotationNormal.IsZero then Vector3d.ZAxis else rotationNormal
        let move = Transform.Translation(insertionPoint.X, insertionPoint.Y, insertionPoint.Z)
        let scale = Transform.Scale(Geometry.Plane.WorldXY, sc.X, sc.Y, sc.Z)
        let rotate = Transform.Rotation(angleRadians, rotationNormal0, Geometry.Point3d.Origin)
        let xform = move * scale * rotate
        RhinoScriptSyntax.InsertBlock2 (blockName, xform)


    [<Extension>]
    ///<summary>Verifies the existence of a block definition in the document</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlock(blockName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        not <| isNull idef


    [<Extension>]
    ///<summary>Verifies a block definition is embedded, or linked, from an external file</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockEmbedded(blockName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        match int( idef.UpdateType) with
        | 1  -> true //DocObjects.InstanceDefinitionUpdateType.Embedded
        | 2  -> true //DocObjects.InstanceDefinitionUpdateType.LinkedAndEmbedded
        |_-> false


    [<Extension>]
    ///<summary>Verifies an object is a block instance</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockInstance(objectId:Guid) : bool =
         match RhinoScriptSyntax.CoerceRhinoObject(objectId) with  //Coerce should not be needed
         | :? DocObjects.InstanceObject as b -> true
         | _ -> false


    [<Extension>]
    ///<summary>Verifies that a block definition is being used by an inserted instance</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///    One of the following values
    ///    0 = Check for top level references in active document
    ///    1 = Check for top level and nested references in active document
    ///    2 = Check for references in other instance definitions</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockInUse(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.InUse(whereToLook)


    [<Extension>]
    ///<summary>Verifies that a block definition is from a reference file</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockReference(blockName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.IsReference


    [<Extension>]
    ///<summary>Renames an existing block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="newName">(string) Name to change to</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RenameBlock(blockName:string, newName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let description = idef.Description
        Doc.InstanceDefinitions.Modify(idef, newName, description, false)


