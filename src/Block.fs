namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?

[<AutoOpen>]
module ExtensionsBlock =
  
  
  type RhinoScriptSyntax with


    [<EXT>]
    ///<summary>Adds a new block definition to the document</summary>
    ///<param name="objectIds">(Guid seq) Objects that will be included in the block</param>
    ///<param name="basisPoint">(Point3d) 3D base point for the block definition</param>
    ///<param name="name">(string) Optional, Default Value: <c>InstanceDefinitions.GetUnusedInstanceDefinitionName()</c>
    ///Name of the block definition. If omitted a name will be automatically generated</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///If True, the objectIds will be deleted</param>
    ///<returns>(string) name of new block definition on success</returns>
    static member AddBlock(objectIds:Guid seq, basePoint:Point3d, [<OPT;DEF("")>]name:string, [<OPT;DEF(false)>]deleteInput:bool) : string =
        let name = if name="" then Doc.InstanceDefinitions.GetUnusedInstanceDefinitionName() else name
        let found = Doc.InstanceDefinitions.Find(name)
        let objects = ResizeArray()
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)  //Coerce should not be needed
            if obj.IsReference then  failwithf "AddBlock: cannt add Refrence Object %A to %s" objectId name
            let ot = obj.ObjectType
            if   ot=DocObjects.ObjectType.Light then  failwithf "AddBlock: cannot add Light Object %A to %s" objectId name
            elif ot=DocObjects.ObjectType.Grip then  failwithf "AddBlock: cannot add Grip Object %A to %s" objectId name
            elif ot=DocObjects.ObjectType.Phantom then failwithf "AddBlock: cannot add Phantom Object %A to %s" objectId name
            elif ot=DocObjects.ObjectType.InstanceReference && notNull found then
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
        
    (*
    def AddBlock(object_ids, base_point, name=None, delete_input=False):
        '''Adds a new block definition to the document
        Parameters:
          object_ids ([guid, ....]) objects that will be included in the block
          base_point (point): 3D base point for the block definition
          name (str, optional): name of the block definition. If omitted a name will be
            automatically generated
          delete_input (bool): if True, the object_ids will be deleted
        Returns:
          str: name of new block definition on success
        '''
        base_point = rhutil.coerce3dpoint(base_point, True)
        if not name:
            name = scriptcontext.doc.InstanceDefinitions.GetUnusedInstanceDefinitionName()
        found = scriptcontext.doc.InstanceDefinitions.Find(name)
        objects = []
        for objectId in object_ids:
            obj = rhutil.coercerhinoobject(objectId, True)
            if obj.IsReference: return
            ot = obj.ObjectType
            if ot==Rhino.DocObjects.ObjectType.Light: return
            if ot==Rhino.DocObjects.ObjectType.Grip: return
            if ot==Rhino.DocObjects.ObjectType.Phantom: return
            if ot==Rhino.DocObjects.ObjectType.InstanceReference and found:
                uses, nesting = obj.UsesDefinition(found.Index)
                if uses: return
            objects.append(obj)
        if objects:
            geometry = [obj.Geometry for obj in objects]
            attrs = [obj.Attributes for obj in objects]
            rc = 0
            if found:
              rc = scriptcontext.doc.InstanceDefinitions.ModifyGeometry(found.Index, geometry, attrs)
            else:
              rc = scriptcontext.doc.InstanceDefinitions.Add(name, "", base_point, geometry, attrs)
            if rc>=0:
                if delete_input:
                    for obj in objects: scriptcontext.doc.Objects.Delete(obj, True)
                scriptcontext.doc.Views.Redraw()
        return name
    *)





    [<EXT>]
    ///<summary>Returns names of the block definitions that contain a specified block
    ///  definition.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string List/ResizeArray) A list of block definition names</returns>
    static member BlockContainers(blockName:string) : string ResizeArray =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let containers = idef.GetContainers()
        let rc = ResizeArray()
        for item in containers do
            if not <| item.IsDeleted then  rc.Add(item.Name)
        rc
    (*
    def BlockContainers(block_name):
        '''Returns names of the block definitions that contain a specified block
        definition.
        Parameters:
          block_name (str): the name of an existing block definition
        Returns:
          list(str, ...): A list of block definition names
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        containers = idef.GetContainers()
        rc = []
        for item in containers:
            if not item.IsDeleted: rc.append(item.Name)
        return rc
    *)

    [<EXT>]
    ///<summary>Returns number of block definitions that contain a specified
    ///  block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(int) the number of block definitions that contain a specified block definition</returns>
    static member BlockContainerCount(blockName:string) : int =
        (RhinoScriptSyntax.BlockContainers(blockName)).Count
    (*
    def BlockContainerCount(block_name):
        '''Returns number of block definitions that contain a specified
        block definition
        Parameters:
          block_name (str): the name of an existing block definition
        Returns:
          number: the number of block definitions that contain a specified block definition
        '''
        return len(BlockContainers(block_name))
    *)


    [<EXT>]
    ///<summary>Returns the number of block definitions in the document</summary>
    ///<returns>(int) the number of block definitions in the document</returns>
    static member BlockCount() : int =
        Doc.InstanceDefinitions.ActiveCount
    (*
    def BlockCount():
        '''Returns the number of block definitions in the document
        Returns:
          number: the number of block definitions in the document
        '''
        return scriptcontext.doc.InstanceDefinitions.ActiveCount
    *)


    [<EXT>]
    ///<summary>Returns the description of a block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string) The current description</returns>
    static member BlockDescription(blockName:string) : string = //GET
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.Description
    (*
    def BlockDescription(block_name, description=None):
        '''Returns or sets the description of a block definition
        Parameters:
          block_name (str): the name of an existing block definition
          description (str, optional): The new description.
        Returns:
          str: if description is not specified, the current description
          str: if description is specified, the previous description
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        rc = idef.Description
        if description: scriptcontext.doc.InstanceDefinitions.Modify( idef, idef.Name, description, True )
        return rc
    *)

    [<EXT>]
    ///<summary>Sets the description of a block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="description">(string)The new description.</param>
    ///<returns>(unit) void, nothing</returns>
    static member BlockDescription(blockName:string, description:string) : unit = //SET
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        Doc.InstanceDefinitions.Modify( idef, idef.Name, description, true ) |>ignore
    (*
    def BlockDescription(block_name, description=None):
        '''Returns or sets the description of a block definition
        Parameters:
          block_name (str): the name of an existing block definition
          description (str, optional): The new description.
        Returns:
          str: if description is not specified, the current description
          str: if description is specified, the previous description
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        rc = idef.Description
        if description: scriptcontext.doc.InstanceDefinitions.Modify( idef, idef.Name, description, True )
        return rc
    *)


    [<EXT>]
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
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let  refs = idef.GetReferences(whereToLook)
        refs.Length
    (*
    def BlockInstanceCount(block_name,where_to_look=0):
        '''Counts number of instances of the block in the document.
        Nested instances are not included in the count. Attention this may include deleted blocks.
        Parameters:
          block_name (str): the name of an existing block definition
          where_to_look (number, optional):
            0 = get top level references in active document.
            1 = get top level and nested references in active document. 
                If a block is nested more than once within another block it will be counted only once.
            2 = check for references from other instance definitions, counts every instance of nested block
        Returns:
          number: the number of instances of the block in the document
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        refs = idef.GetReferences(where_to_look)
        return len(refs)
    *)


    [<EXT>]
    ///<summary>Returns the insertion point of a block instance.</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Point3d) The insertion 3D point</returns>
    static member BlockInstanceInsertPoint(objectId:Guid) : Point3d =
        let  instance = RhinoScriptSyntax.CoerceBlockInstanceObject(objectId)
        let  xf = instance.InstanceXform
        let  pt = Geometry.Point3d.Origin
        pt.Transform(xf)
        pt
    (*
    def BlockInstanceInsertPoint(object_id):
        '''Returns the insertion point of a block instance.
        Parameters:
          object_id (guid): The identifier of an existing block insertion object
        Returns:
          point: The insertion 3D point if successful
        '''
        instance = __CoerceBlockInstanceObject(object_id, True)
        xf = instance.InstanceXform
        pt = Rhino.Geometry.Point3d.Origin
        pt.Transform(xf)
        return pt
    *)


    [<EXT>]
    ///<summary>Returns the block name of a block instance</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(string) the block name of a block instance</returns>
    static member BlockInstanceName(objectId:Guid) : string =
        let mutable instance = RhinoScriptSyntax.CoerceBlockInstanceObject(objectId)
        let mutable idef = instance.InstanceDefinition
        idef.Name
    (*
    def BlockInstanceName(object_id):
        '''Returns the block name of a block instance
        Parameters:
          object_id (guid): The identifier of an existing block insertion object
        Returns:
          str: the block name of a block instance
        '''
        instance = __CoerceBlockInstanceObject(object_id, True)
        idef = instance.InstanceDefinition
        return idef.Name
    *)


    [<EXT>]
    ///<summary>Returns the identifiers of the inserted instances of a block.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///0 = get top level references in active document.
    ///  1 = get top level and nested references in active document.
    ///  2 = check for references from other instance definitions</param>
    ///<returns>(Guid seq) Ids identifying the instances of a block in the model.</returns>
    static member BlockInstances(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : Guid [] =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let instances = idef.GetReferences(0)
        [| for item in instances do yield item.Id |]
    (*
    def BlockInstances(block_name,where_to_look=0):
        '''Returns the identifiers of the inserted instances of a block.
        Parameters:
          block_name (str): the name of an existing block definition
          where_to_look (int, optional):
            0 = get top level references in active document.
            1 = get top level and nested references in active document.
            2 = check for references from other instance definitions
        Returns:
          list(guid, ...): Ids identifying the instances of a block in the model.
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        instances = idef.GetReferences(where_to_look)
        return [item.Id for item in instances]
    *)


    [<EXT>]
    ///<summary>Returns the location of a block instance relative to the world coordinate
    ///  system origin (0,0,0). The position is returned as a 4x4 transformation
    ///  matrix</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Transform) the location, as a transform matrix, of a block instance relative to the world coordinate
    ///  system origin</returns>
    static member BlockInstanceXform(objectId:Guid) : Transform =
        let  instance = RhinoScriptSyntax.CoerceBlockInstanceObject(objectId)
        instance.InstanceXform
    (*
    def BlockInstanceXform(object_id):
        '''Returns the location of a block instance relative to the world coordinate
        system origin (0,0,0). The position is returned as a 4x4 transformation
        matrix
        Parameters:
          object_id (guid): The identifier of an existing block insertion object
        Returns:
          transform: the location, as a transform matrix, of a block instance relative to the world coordinate
        system origin
        '''
        instance = __CoerceBlockInstanceObject(object_id, True)
        return instance.InstanceXform
    *)


    [<EXT>]
    ///<summary>Returns the names of all block definitions in the document</summary>
    ///<returns>(string seq) the names of all block definitions in the document</returns>
    static member BlockNames() : string [] =
        let  ideflist = Doc.InstanceDefinitions.GetList(true)
        [| for item in ideflist do yield item.Name|]
        
    (*
    def BlockNames( sort=False ):
        '''Returns the names of all block definitions in the document
        Parameters:
          sort (bool): True to return a sorted list
        Returns:
          list(str, ...): the names of all block definitions in the document
        '''
        ideflist = scriptcontext.doc.InstanceDefinitions.GetList(True)
        rc = [item.Name for item in ideflist]
        if(sort): rc.sort()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns number of objects that make up a block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(int) the number of objects that make up a block definition</returns>
    static member BlockObjectCount(blockName:string) : int =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.ObjectCount
    (*
    def BlockObjectCount(block_name):
        '''Returns number of objects that make up a block definition
        Parameters:
          block_name (str): name of an existing block definition
        Returns:
          number: the number of objects that make up a block definition
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        return idef.ObjectCount
    *)


    [<EXT>]
    ///<summary>Returns identifiers of the objects that make up a block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(Guid seq) list of identifiers on success</returns>
    static member BlockObjects(blockName:string) : Guid [] =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let  rhobjs = idef.GetObjects()
        [|for obj in rhobjs -> obj.Id|]
    (*
    def BlockObjects(block_name):
        '''Returns identifiers of the objects that make up a block definition
        Parameters:
          block_name (str): name of an existing block definition
        Returns:
          list(guid, ...): list of identifiers on success
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        rhobjs = idef.GetObjects()
        return [obj.Id for obj in rhobjs]
    *)


    [<EXT>]
    ///<summary>Returns path to the source of a linked or embedded block definition.
    ///  A linked or embedded block definition is a block definition that was
    ///  inserted from an external file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(string) path to the linked block on success</returns>
    static member BlockPath(blockName:string) : string =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.SourceArchive
    (*
    def BlockPath(block_name):
        '''Returns path to the source of a linked or embedded block definition.
        A linked or embedded block definition is a block definition that was
        inserted from an external file.
        Parameters:
          block_name (str): name of an existing block definition
        Returns:
          str: path to the linked block on success
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        return idef.SourceArchive
    *)


    [<EXT>]
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
        let  idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  -3
        else int(idef.ArchiveFileStatus)
    (*
    def BlockStatus(block_name):
        '''Returns the status of a linked block
        Parameters:
            block_name (str): Name of an existing block
        Returns:
          number: the status of a linked block
            Value Description
            -3    Not a linked block definition.
            -2    The linked block definition's file could not be opened or could not be read.
            -1    The linked block definition's file could not be found.
             0    The linked block definition is up-to-date.
             1    The linked block definition's file is newer than definition.
             2    The linked block definition's file is older than definition.
             3    The linked block definition's file is different than definition.
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: return -3
        return int(idef.ArchiveFileStatus)
    *)


    [<EXT>]
    ///<summary>Deletes a block definition and all of it's inserted instances.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteBlock(blockName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let  rc = Doc.InstanceDefinitions.Delete(idef.Index, true, false)
        Doc.Views.Redraw()
        rc
    (*
    def DeleteBlock(block_name):
        '''Deletes a block definition and all of it's inserted instances.
        Parameters:
          block_name (str): name of an existing block definition
        Returns:
          bool: True or False indicating success or failure
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        rc = scriptcontext.doc.InstanceDefinitions.Delete(idef.Index, True, False)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Explodes a block instance into it's geometric components. The
    ///  exploded objects are added to the document</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<param name="explodeNestedInstances">(bool) Optional, Default Value: <c>false</c>
    ///By default nested blocks are not exploded.</param>
    ///<returns>(Guid seq) identifiers for the newly exploded objects on success</returns>
    static member ExplodeBlockInstance(objectId:Guid, [<OPT;DEF(false)>]explodeNestedInstances:bool) : Guid [] =
        let  instance = RhinoScriptSyntax.CoerceBlockInstanceObject(objectId)
        let  guids = Doc.Objects.AddExplodedInstancePieces(instance, explodeNestedInstances, deleteInstance=true)
        if guids.Length > 0 then Doc.Views.Redraw()
        guids
    (*
    def ExplodeBlockInstance(object_id, explode_nested_instances=False):
        '''Explodes a block instance into it's geometric components. The
        exploded objects are added to the document
        Parameters:
          object_id (guid): The identifier of an existing block insertion object
          explode_nested_instances (bool, optional): By default nested blocks are not exploded.
        Returns:
          list(guid, ...): identifiers for the newly exploded objects on success
        '''
        instance = __CoerceBlockInstanceObject(object_id, True)
        guids = scriptcontext.doc.Objects.AddExplodedInstancePieces(instance, explodeNestedInstances=explode_nested_instances, deleteInstance=True)
        if guids:
          scriptcontext.doc.Views.Redraw()
        return guids
    *)



    [<EXT>]
    ///<summary>Inserts a block whose definition already exists in the document</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="xform">(Transform) 4x4 transformation matrix to apply</param>
    ///<returns>(Guid) objectId for the block that was added to the doc on success</returns>
    static member InsertBlock2(blockName:string, xform:Transform) : Guid =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let objectId = Doc.Objects.AddInstanceObject(idef.Index, xform )
        if objectId<>System.Guid.Empty then
            Doc.Views.Redraw()
        objectId
    (*
    def InsertBlock2(block_name, xform):
        '''Inserts a block whose definition already exists in the document
        Parameters:
          block_name (str): name of an existing block definition
          xform (transform): 4x4 transformation matrix to apply
        Returns:
          guid: objectId for the block that was added to the doc on success
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        xform = rhutil.coercexform(xform, True)
        objectId = scriptcontext.doc.Objects.AddInstanceObject(idef.Index, xform )
        if objectId!=System.Guid.Empty:
            scriptcontext.doc.Views.Redraw()
            return objectId
    *)
    
    
    [<EXT>]
    ///<summary>Inserts a block whose definition already exists in the document</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="insertionPoint">(Point3d) Insertion point for the block</param>
    ///<param name="scale">(float*float*float) Optional, Default Value: <c>Vector3d(1. , 1. , 1.)</c>
    ///  X,y,z scale factors</param>
    ///<param name="angleDegrees">(float) Optional, Default Value: <c>0</c>
    ///  Rotation angle in degrees</param>
    ///<param name="rotationNormal">(Vector3d) Optional, Default Value: <c> Vector3d.ZAxis</c>
    ///  The axis of rotation.</param>
    ///<returns>(Guid) objectId for the block that was added to the doc</returns>
    static member InsertBlock(blockName:string, insertionPoint:Point3d, [<OPT;DEF(Vector3d())>]scale:Vector3d, [<OPT;DEF(0.0)>]angleDegrees:float, [<OPT;DEF(Vector3d())>]rotationNormal:Vector3d) : Guid =
        let angleRadians = UtilMath.toRadians(angleDegrees)
        let sc= if scale.IsZero then Vector3d(1. ,1. ,1.) else scale
        let rotationNormal0= if rotationNormal.IsZero then Vector3d.ZAxis else rotationNormal
        let move = Transform.Translation(insertionPoint.X,insertionPoint.Y,insertionPoint.Z)
        let scale = Transform.Scale(Geometry.Plane.WorldXY, sc.X, sc.Y, sc.Z)
        let rotate = Transform.Rotation(angleRadians, rotationNormal0, Geometry.Point3d.Origin)
        let xform = move * scale * rotate
        RhinoScriptSyntax.InsertBlock2 (blockName,xform)
    (*
    def InsertBlock( block_name, insertion_point, scale=(1,1,1), angle_degrees=0, rotation_normal=(0,0,1) ):
        '''Inserts a block whose definition already exists in the document
        Parameters:
          block_name (str): name of an existing block definition
          insertion_point (point): insertion point for the block
          scale ([number, number, number]): x,y,z scale factors
          angle_degrees (number, optional): rotation angle in degrees
          rotation_normal (vector, optional): the axis of rotation.
        Returns:
          guid: objectId for the block that was added to the doc
        '''
        insertion_point = rhutil.coerce3dpoint(insertion_point, True)
        rotation_normal = rhutil.coerce3dvector(rotation_normal, True)
        angle_radians = math.radians(angle_degrees)
        trans = Rhino.Geometry.Transform
        move = trans.Translation(insertion_point[0],insertion_point[1],insertion_point[2])
        scale = trans.Scale(Rhino.Geometry.Plane.WorldXY, scale[0], scale[1], scale[2])
        rotate = trans.Rotation(angle_radians, rotation_normal, Rhino.Geometry.Point3d.Origin)
        xform = move * scale * rotate
        return InsertBlock2( block_name, xform )
    *)


    [<EXT>]
    ///<summary>Verifies the existence of a block definition in the document.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlock(blockName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        not <| isNull idef
    (*
    def IsBlock(block_name):
        '''Verifies the existence of a block definition in the document.
        Parameters:
          block_name (str): name of an existing block definition
        Returns:
          bool: True or False
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        return (idef is not None)
    *)


    [<EXT>]
    ///<summary>Verifies a block definition is embedded, or linked, from an external file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockEmbedded(blockName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        match int( idef.UpdateType) with
        | 1  -> true //DocObjects.InstanceDefinitionUpdateType.Embedded
        | 2  -> true //DocObjects.InstanceDefinitionUpdateType.LinkedAndEmbedded
        |_-> false
    (*
    def IsBlockEmbedded(block_name):
        '''Verifies a block definition is embedded, or linked, from an external file.
        Parameters:
          block_name (str): name of an existing block definition
        Returns:
          bool: True or False
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        ut = Rhino.DocObjects.InstanceDefinitionUpdateType
        return (idef.UpdateType==ut.Embedded or idef.UpdateType==ut.Static or idef.UpdateType==ut.LinkedAndEmbedded)
    *)


    [<EXT>]
    ///<summary>Verifies an object is a block instance</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockInstance(objectId:Guid) : bool =
         match RhinoScriptSyntax.CoerceRhinoObject(objectId) with  //Coerce should not be needed
         | :? DocObjects.InstanceObject as b -> true
         | _ -> false
    (*
    def IsBlockInstance(object_id):
        '''Verifies an object is a block instance
        Parameters:
          object_id (guid): The identifier of an existing block insertion object
        Returns:
          bool: True or False
        '''
        return  __CoerceBlockInstanceObject(object_id, False) is not None
    *)


    [<EXT>]
    ///<summary>Verifies that a block definition is being used by an inserted instance</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///One of the following values
    ///  0 = Check for top level references in active document
    ///  1 = Check for top level and nested references in active document
    ///  2 = Check for references in other instance definitions</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockInUse(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.InUse(whereToLook)
    (*
    def IsBlockInUse(block_name, where_to_look=0):
        '''Verifies that a block definition is being used by an inserted instance
        Parameters:
          block_name (str): name of an existing block definition
          where_to_look (number, optional): One of the following values
               0 = Check for top level references in active document
               1 = Check for top level and nested references in active document
               2 = Check for references in other instance definitions
        Returns:
          bool: True or False
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        return idef.InUse(where_to_look)
    *)


    [<EXT>]
    ///<summary>Verifies that a block definition is from a reference file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockReference(blockName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        idef.IsReference
    (*
    def IsBlockReference(block_name):
        '''Verifies that a block definition is from a reference file.
        Parameters:
          block_name (str): name of an existing block definition
        Returns:
          bool: True or False
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        return idef.IsReference
    *)


    [<EXT>]
    ///<summary>Renames an existing block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="newName">(string) Name to change to</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RenameBlock(blockName:string, newName:string) : bool =
        let idef = Doc.InstanceDefinitions.Find(blockName)
        if isNull idef then  failwithf "%s does not exist in InstanceDefinitionsTable" blockName
        let description = idef.Description
        Doc.InstanceDefinitions.Modify(idef, newName, description, false)
    (*
    def RenameBlock( block_name, new_name ):
        '''Renames an existing block definition
        Parameters:
          block_name (str): name of an existing block definition
          new_name (str): name to change to
        Returns:
          bool: True or False indicating success or failure
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        description = idef.Description
        rc = scriptcontext.doc.InstanceDefinitions.Modify(idef, new_name, description, False)
        return rc
    *)


