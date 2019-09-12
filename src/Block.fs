namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsBlock =
  type RhinoScriptSyntax with
    
    ///<summary>Returns the Rhino Block instance object for a given Id</summary>
    ///<param name="id">(Guid) Id of block instance</param>
    ///<param name="raiseIfMissing">(bool) Optional, Default Value: <c>false</c>
    ///Raise error if id is missing?</param>
    ///<returns>(Rhino.DocObjects.InstanceObject) block instance object</returns>
    static member InstanceObjectFromId(id:Guid, [<OPT;DEF(false)>]raiseIfMissing:bool) : Rhino.DocObjects.InstanceObject =
        failNotImpl () // done in 2018


    ///<summary>Adds a new block definition to the document</summary>
    ///<param name="objectIds">(Guid seq) Objects that will be included in the block</param>
    ///<param name="basisPoint">(Point3d) 3D base point for the block definition</param>
    ///<param name="name">(string) Optional, Default Value: <c>null</c>
    ///Name of the block definition. If omitted a name will be
    ///  automatically generated</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///If True, the objectIds will be deleted</param>
    ///<returns>(string) name of new block definition on success</returns>
    static member AddBlock(objectIds:Guid seq, basisPoint:Point3d, [<OPT;DEF(null)>]name:string, [<OPT;DEF(false)>]deleteInput:bool) : string =
        failNotImpl () // done in 2018


    ///<summary>Returns number of block definitions that contain a specified
    ///  block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(int) the number of block definitions that contain a specified block definition</returns>
    static member BlockContainerCount(blockName:string) : int =
        failNotImpl () // done in 2018


    ///<summary>Returns names of the block definitions that contain a specified block
    ///  definition.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string seq) A list of block definition names</returns>
    static member BlockContainers(blockName:string) : string seq =
        failNotImpl () // done in 2018


    ///<summary>Returns the number of block definitions in the document</summary>
    ///<returns>(int) the number of block definitions in the document</returns>
    static member BlockCount() : int =
        failNotImpl () // done in 2018


    ///<summary>Returns the description of a block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string) The current description</returns>
    static member BlockDescription(blockName:string) : string =
        failNotImpl () // done in 2018

    ///<summary>Sets the description of a block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="description">(string)The new description.</param>
    ///<returns>(unit) unit</returns>
    static member BlockDescription(blockName:string, description:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Counts number of instances of the block in the document.
    ///  Nested instances are not included in the count. Attention this may include deleted blocks.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///0 = get top level references in active document.
    ///  1 = get top level and nested references in active document.
    ///    If a block is nested more than once within another block it will be counted only once.
    ///  2 = check for references from other instance definitions, counts every instance of nested block</param>
    ///<returns>(int) the number of instances of the block in the document</returns>
    static member BlockInstanceCount(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : int =
        failNotImpl () // done in 2018


    ///<summary>Returns the insertion point of a block instance.</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Point3d) The insertion 3D point</returns>
    static member BlockInstanceInsertPoint(objectId:Guid) : Point3d =
        failNotImpl () // done in 2018


    ///<summary>Returns the block name of a block instance</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(string) the block name of a block instance</returns>
    static member BlockInstanceName(objectId:Guid) : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the identifiers of the inserted instances of a block.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///0 = get top level references in active document.
    ///  1 = get top level and nested references in active document.
    ///  2 = check for references from other instance definitions</param>
    ///<returns>(Guid seq) Ids identifying the instances of a block in the model.</returns>
    static member BlockInstances(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : Guid seq =
        failNotImpl () // done in 2018


    ///<summary>Returns the location of a block instance relative to the world coordinate
    ///  system origin (0,0,0). The position is returned as a 4x4 transformation
    ///  matrix</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Transform) the location, as a transform matrix, of a block instance relative to the world coordinate
    ///  system origin</returns>
    static member BlockInstanceXform(objectId:Guid) : Transform =
        failNotImpl () // done in 2018


    ///<summary>Returns the names of all block definitions in the document</summary>
    ///<returns>(string seq) the names of all block definitions in the document</returns>
    static member BlockNames() : string seq =
        failNotImpl () // done in 2018


    ///<summary>Returns number of objects that make up a block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(int) the number of objects that make up a block definition</returns>
    static member BlockObjectCount(blockName:string) : int =
        failNotImpl () // done in 2018


    ///<summary>Returns identifiers of the objects that make up a block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(Guid seq) list of identifiers on success</returns>
    static member BlockObjects(blockName:string) : Guid seq =
        failNotImpl () // done in 2018


    ///<summary>Returns path to the source of a linked or embedded block definition.
    ///  A linked or embedded block definition is a block definition that was
    ///  inserted from an external file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(string) path to the linked block on success</returns>
    static member BlockPath(blockName:string) : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the status of a linked block</summary>
    ///<param name="blockName">(string) Name of an existing block</param>
    ///<returns>(int) the status of a linked block
    ///  Value Description
    ///  -3    Not a linked block definition.
    ///  -2    The linked block definition's file could not be opened or could not be read.
    ///  -1    The linked block definition's file could not be found.
    ///    0    The linked block definition is up-to-date.
    ///    1    The linked block definition's file is newer than definition.
    ///    2    The linked block definition's file is older than definition.
    ///    3    The linked block definition's file is different than definition.</returns>
    static member BlockStatus(blockName:string) : int =
        failNotImpl () // done in 2018


    ///<summary>Deletes a block definition and all of it's inserted instances.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteBlock(blockName:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Explodes a block instance into it's geometric components. The
    ///  exploded objects are added to the document</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<param name="explodeNestedInstances">(bool) Optional, Default Value: <c>false</c>
    ///By default nested blocks are not exploded.</param>
    ///<returns>(Guid seq) identifiers for the newly exploded objects on success</returns>
    static member ExplodeBlockInstance(objectId:Guid, [<OPT;DEF(false)>]explodeNestedInstances:bool) : Guid seq =
        failNotImpl () // done in 2018


    ///<summary>Inserts a block whose definition already exists in the document</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="insertionPoint">(Point3d) Insertion point for the block</param>
    ///<param name="scale">(float*float*float) Optional, Default Value: <c>1*1*1</c>
    ///X,y,z scale factors</param>
    ///<param name="angleDegrees">(int) Optional, Default Value: <c>0</c>
    ///Rotation angle in degrees</param>
    ///<param name="rotationNormal">(Vector3d) Optional, Default Value: <c>0*0*1</c>
    ///The axis of rotation.</param>
    ///<returns>(Guid) id for the block that was added to the doc</returns>
    static member InsertBlock(blockName:string, insertionPoint:Point3d, [<OPT;DEF(null)>]scale:float*float*float, [<OPT;DEF(0)>]angleDegrees:int, [<OPT;DEF(null)>]rotationNormal:Vector3d) : Guid =
        failNotImpl () // done in 2018


    ///<summary>Inserts a block whose definition already exists in the document</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="xform">(Transform) 4x4 transformation matrix to apply</param>
    ///<returns>(Guid) id for the block that was added to the doc on success</returns>
    static member InsertBlock2(blockName:string, xform:Transform) : Guid =
        failNotImpl () // done in 2018


    ///<summary>Verifies the existence of a block definition in the document.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlock(blockName:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies a block definition is embedded, or linked, from an external file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockEmbedded(blockName:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies an object is a block instance</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockInstance(objectId:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies that a block definition is being used by an inserted instance</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="whereToLook">(float) Optional, Default Value: <c>0</c>
    ///One of the following values
    ///  0 = Check for top level references in active document
    ///  1 = Check for top level and nested references in active document
    ///  2 = Check for references in other instance definitions</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockInUse(blockName:string, [<OPT;DEF(0)>]whereToLook:float) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies that a block definition is from a reference file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockReference(blockName:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Renames an existing block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="newName">(string) Name to change to</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RenameBlock(blockName:string, newName:string) : bool =
        failNotImpl () // done in 2018


