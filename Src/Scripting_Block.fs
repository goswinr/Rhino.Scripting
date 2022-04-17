
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenBlock =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    ///<summary>Adds a new block definition to the document.</summary>
    ///<param name="objectIds">(Guid seq) Objects that will be included in the block</param>
    ///<param name="basePoint">(Point3d) 3D base point for the block definition</param>
    ///<param name="name">(string) Optional, Default Value: <c>InstanceDefinitions.GetUnusedInstanceDefinitionName()</c>
    ///    Name of the block definition. If omitted a name will be automatically generated</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    If True, the objectIds will be deleted</param>
    ///<returns>(string) name of new block definition.</returns>
    static member AddBlock(objectIds:Guid seq, basePoint:Point3d, [<OPT;DEF("")>]name:string, [<OPT;DEF(false)>]deleteInput:bool) : string = 
        let name = if name="" then State.Doc.InstanceDefinitions.GetUnusedInstanceDefinitionName() else name
        let found = State.Doc.InstanceDefinitions.Find(name)
        let objects = Rarr()
        for objectId in objectIds do
            let obj = Scripting.CoerceRhinoObject(objectId)  //Coerce should not be needed
            if obj.IsReference then  RhinoScriptingException.Raise "Rhino.Scripting.AddBlock: cannt add Refrence object %s to %s" (Print.guid objectId) name
            let ot = obj.ObjectType
            if   ot= DocObjects.ObjectType.Light then  RhinoScriptingException.Raise "Rhino.Scripting.AddBlock: cannot add Light object %s to %s" (Print.guid objectId) name
            elif ot= DocObjects.ObjectType.Grip then  RhinoScriptingException.Raise "Rhino.Scripting.AddBlock: cannot add Grip object %s to %s" (Print.guid objectId) name
            elif ot= DocObjects.ObjectType.Phantom then RhinoScriptingException.Raise "Rhino.Scripting.AddBlock: cannot add Phantom object %s to %s" (Print.guid objectId) name
            elif ot= DocObjects.ObjectType.InstanceReference && notNull found then
                let bli = Scripting.CoerceBlockInstanceObject(objectId) // not obj ?
                let uses, nesting = bli.UsesDefinition(found.Index)
                if uses then RhinoScriptingException.Raise "Rhino.Scripting.AddBlock: cannt add Instance Ref object %s to %s" (Print.guid objectId) name

            objects.Add(obj)
        if objects.Count>0 then
            let geometry = [ for obj in objects -> obj.Geometry]
            let attrs = [ for obj in objects -> obj.Attributes]
            let mutable rc = -1
            if notNull found then
                rc <- if State.Doc.InstanceDefinitions.ModifyGeometry(found.Index, geometry, attrs) then 0 else -1
            else
                rc <- State.Doc.InstanceDefinitions.Add(name, "", basePoint, geometry, attrs)
            if rc >= 0 then
                if deleteInput then
                    for obj in objects do State.Doc.Objects.Delete(obj, quiet=true) |>ignore
                State.Doc.Views.Redraw()
        name






    ///<summary>Returns names of the block definitions that contain a specified block
    ///    definition.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string Rarr) A list of block definition names.</returns>
    static member BlockContainers(blockName:string) : string Rarr = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        let containers = idef.GetContainers()
        let rc = Rarr()
        for item in containers do
            if not <| item.IsDeleted then  rc.Add(item.Name)
        rc

    ///<summary>Returns number of block definitions that contain a specified
    ///    block definition.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(int) The number of block definitions that contain a specified block definition.</returns>
    static member BlockContainerCount(blockName:string) : int = 
        (Scripting.BlockContainers(blockName)).Count


    ///<summary>Returns the number of block definitions in the document.</summary>
    ///<returns>(int) The number of block definitions in the document.</returns>
    static member BlockCount() : int = 
        State.Doc.InstanceDefinitions.ActiveCount


    ///<summary>Returns the description of a block definition.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string) The current description.</returns>
    static member BlockDescription(blockName:string) : string = //GET
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        idef.Description

    ///<summary>Sets the description of a block definition.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="description">(string) The new description</param>
    ///<returns>(unit) void, nothing.</returns>
    static member BlockDescription(blockName:string, description:string) : unit = //SET
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        State.Doc.InstanceDefinitions.Modify( idef, idef.Name, description, true ) |>ignore


    ///<summary>Counts number of instances of the block in the document.
    ///    Nested instances are not included in the count. Attention this may include deleted blocks.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///    0 = get top level references in active document.
    ///    1 = get top level and nested references in active document.
    ///      If a block is nested more than once within another block it will be counted only once.
    ///    2 = check for references from other instance definitions, counts every instance of nested block</param>
    ///<returns>(int) The number of instances of the block in the document.</returns>
    static member BlockInstanceCount(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : int = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        let  refs = idef.GetReferences(whereToLook)
        refs.Length


    ///<summary>Returns the insertion point of a block instance.</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Point3d) The insertion 3D point.</returns>
    static member BlockInstanceInsertPoint(objectId:Guid) : Point3d = 
        let  instance = Scripting.CoerceBlockInstanceObject(objectId)
        let  xf = instance.InstanceXform
        let  pt = Point3d(0. , 0. , 0.)
        pt.Transform(xf)
        pt


    ///<summary>Returns the block name of a block instance.</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(string) The block name of a block instance.</returns>
    static member BlockInstanceName(objectId:Guid) : string = 
        let mutable instance = Scripting.CoerceBlockInstanceObject(objectId)
        let mutable idef = instance.InstanceDefinition
        idef.Name


    ///<summary>Returns the identifiers of the inserted instances of a block.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///    0 = get top level references in active document.
    ///    1 = get top level and nested references in active document.
    ///    2 = check for references from other instance definitions</param>
    ///<returns>(Guid Rarr) Ids identifying the instances of a block in the model.</returns>
    static member BlockInstances(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : Rarr<Guid> = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        let instances = idef.GetReferences(0)
        rarr { for item in instances do yield item.Id }


    ///<summary>Returns the location of a block instance relative to the world coordinate
    ///    system origin (0, 0, 0). The position is returned as a 4x4 transformation
    ///    matrix.</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Transform) The location, as a transform matrix, of a block instance relative to the world coordinate
    ///    system origin.</returns>
    static member BlockInstanceXform(objectId:Guid) : Transform = 
        let  instance = Scripting.CoerceBlockInstanceObject(objectId)
        instance.InstanceXform


    ///<summary>Returns the names of all block definitions in the document.</summary>
    ///<returns>(string Rarr) The names of all block definitions in the document.</returns>
    static member BlockNames() : string Rarr = 
        let  ideflist = State.Doc.InstanceDefinitions.GetList(true)
        rarr { for item in ideflist do yield item.Name}



    ///<summary>Returns number of objects that make up a block definition.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(int) The number of objects that make up a block definition.</returns>
    static member BlockObjectCount(blockName:string) : int = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        idef.ObjectCount


    ///<summary>Returns identifiers of the objects that make up a block definition.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(Guid Rarr) list of identifiers.</returns>
    static member BlockObjects(blockName:string) : Rarr<Guid> = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        let  rhobjs = idef.GetObjects()
        rarr { for obj in rhobjs -> obj.Id}


    ///<summary>Returns path to the source of a linked or embedded block definition.
    ///    A linked or embedded block definition is a block definition that was
    ///    inserted from an external file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(string) path to the linked block.</returns>
    static member BlockPath(blockName:string) : string = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        idef.SourceArchive


    ///<summary>Returns the status of a linked block.</summary>
    ///<param name="blockName">(string) Name of an existing block</param>
    ///<returns>(int) The status of a linked block
    ///    Value Description
    ///    -3    Not a linked block definition.
    ///    -2    The linked block definition's file could not be opened or could not be read.
    ///    -1    The linked block definition's file could not be found.
    ///      0    The linked block definition is up-to-date.
    ///      1    The linked block definition's file is newer than definition.
    ///      2    The linked block definition's file is older than definition.
    ///      3    The linked block definition's file is different than definition.</returns>
    static member BlockStatus(blockName:string) : int = 
        let  idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  -3
        else int(idef.ArchiveFileStatus)


    ///<summary>Deletes a block definition and all of it's inserted instances.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member DeleteBlock(blockName:string) : bool = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        let  rc = State.Doc.InstanceDefinitions.Delete(idef.Index, deleteReferences=true, quiet=false)
        State.Doc.Views.Redraw()
        rc


    ///<summary>Explodes a block instance into it's geometric components. The exploded objects are added to the document. The block instance is deleted</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<param name="explodeNestedInstances">(bool) Optional, Default Value: <c>false</c>
    ///    By default nested blocks are not exploded</param>
    ///<returns>(Guid array) identifiers for the newly exploded objects.</returns>
    static member ExplodeBlockInstance(objectId:Guid, [<OPT;DEF(false)>]explodeNestedInstances:bool) : array<Guid> = 
        let  instance = Scripting.CoerceBlockInstanceObject(objectId)
        let  guids = State.Doc.Objects.AddExplodedInstancePieces(instance, explodeNestedInstances, deleteInstance = true)
        if guids.Length > 0 then State.Doc.Views.Redraw()
        guids



    ///<summary>Inserts a block whose definition already exists in the document.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="xForm">(Transform) 4x4 transformation matrix to apply</param>
    ///<returns>(Guid) objectId for the block that was added to the doc.</returns>
    static member InsertBlock2(blockName:string, xForm:Transform) : Guid = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        let objectId = State.Doc.Objects.AddInstanceObject(idef.Index, xForm )
        if objectId<>System.Guid.Empty then
            State.Doc.Views.Redraw()
        objectId


    ///<summary>Inserts a block whose definition already exists in the document.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="insertionPoint">(Point3d) Insertion point for the block</param>
    ///<param name="scale">(Vector3d) Optional, Default Value: <c>Vector3d(1.0 , 1.0 , 1.0)</c>
    ///    X, y, z scale factors</param>
    ///<param name="angleDegrees">(float) Optional, Default Value: <c>0</c>
    ///    Rotation angle in degrees</param>
    ///<param name="rotationNormal">(Vector3d) Optional, Default Value: <c> Vector3d.ZAxis</c>
    ///    The axis of rotation</param>
    ///<returns>(Guid) objectId for the block that was added to the doc.</returns>
    static member InsertBlock(blockName:string, insertionPoint:Point3d, [<OPT;DEF(Vector3d())>]scale:Vector3d, [<OPT;DEF(0.0)>]angleDegrees:float, [<OPT;DEF(Vector3d())>]rotationNormal:Vector3d) : Guid = 
        let angleRadians = UtilMath.toRadians(angleDegrees)
        let sc= if scale.IsZero then Vector3d(1. , 1. , 1.) else scale
        let rotationNormal0 = if rotationNormal.IsZero then Vector3d.ZAxis else rotationNormal
        let move = Transform.Translation(insertionPoint.X, insertionPoint.Y, insertionPoint.Z)
        let scale = Transform.Scale(Geometry.Plane.WorldXY, sc.X, sc.Y, sc.Z)
        let rotate = Transform.Rotation(angleRadians, rotationNormal0, Geometry.Point3d.Origin)
        let xForm = move * scale * rotate
        Scripting.InsertBlock2 (blockName, xForm)


    ///<summary>Verifies the existence of a block definition in the document.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False.</returns>
    static member IsBlock(blockName:string) : bool = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        not <| isNull idef


    ///<summary>Verifies a block definition is embedded, or linked, from an external file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False.</returns>
    static member IsBlockEmbedded(blockName:string) : bool = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        match int( idef.UpdateType) with
        | 1  -> true //DocObjects.InstanceDefinitionUpdateType.Embedded
        | 2  -> true //DocObjects.InstanceDefinitionUpdateType.LinkedAndEmbedded
        |_-> false


    ///<summary>Verifies an object is a block instance.</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(bool) True or False.</returns>
    static member IsBlockInstance(objectId:Guid) : bool = 
        match Scripting.CoerceRhinoObject(objectId) with  //Coerce should not be needed
        | :? DocObjects.InstanceObject as b -> true
        | _ -> false


    ///<summary>Verifies that a block definition is being used by an inserted instance.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///    One of the following values
    ///    0 = Check for top level references in active document
    ///    1 = Check for top level and nested references in active document
    ///    2 = Check for references in other instance definitions</param>
    ///<returns>(bool) True or False.</returns>
    static member IsBlockInUse(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : bool = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        idef.InUse(whereToLook)


    ///<summary>Verifies that a block definition is from a reference file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False.</returns>
    static member IsBlockReference(blockName:string) : bool = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        idef.IsReference


    ///<summary>Renames an existing block definition.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="newName">(string) Name to change to</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member RenameBlock(blockName:string, newName:string) : bool = 
        let idef = State.Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in InstanceDefinitionsTable" blockName
        let description = idef.Description
        State.Doc.InstanceDefinitions.Modify(idef, newName, description, quiet=false)



