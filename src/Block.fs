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
    
    ///<summary>Returns the Rhino Block instance object for a given Id</summary>
    ///<param name="id">(Guid) Id of block instance</param>
    ///<param name="raiseIfMissing">(bool) Optional, Default Value: <c>false</c>
    ///Raise error if id is missing?</param>
    ///<returns>(Rhino.DocObjects.InstanceObject) block instance object</returns>
    static member InstanceObjectFromId(id:Guid, [<OPT;DEF(false)>]raiseIfMissing:bool) : Rhino.DocObjects.InstanceObject =
        failNotImpl () // genreation temp disabled !!
    (*
    def InstanceObjectFromId(id, raise_if_missing=False):
        '''Returns the Rhino Block instance object for a given Id
        Parameters:
          id (guid): Id of block instance
          raise_if_missing(bool,optional): raise error if id is missing?
        Returns:
          Rhino.DocObjects.InstanceObject: block instance object
        '''
        rhobj = rhutil.coercerhinoobject(id, True, raise_if_missing)
        if isinstance(rhobj, Rhino.DocObjects.InstanceObject): return rhobj
        if raise_if_missing: raise ValueError("unable to find InstanceObject")
    *)


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
        failNotImpl () // genreation temp disabled !!
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
        for id in object_ids:
            obj = rhutil.coercerhinoobject(id, True)
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


    ///<summary>Returns number of block definitions that contain a specified
    ///  block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(int) the number of block definitions that contain a specified block definition</returns>
    static member BlockContainerCount(blockName:string) : int =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns names of the block definitions that contain a specified block
    ///  definition.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string seq) A list of block definition names</returns>
    static member BlockContainers(blockName:string) : string seq =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns the number of block definitions in the document</summary>
    ///<returns>(int) the number of block definitions in the document</returns>
    static member BlockCount() : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def BlockCount():
        '''Returns the number of block definitions in the document
        Returns:
          number: the number of block definitions in the document
        '''
        return scriptcontext.doc.InstanceDefinitions.ActiveCount
    *)


    ///<summary>Returns the description of a block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<returns>(string) The current description</returns>
    static member BlockDescription(blockName:string) : string = //GET
        failNotImpl () // genreation temp disabled !!
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

    ///<summary>Sets the description of a block definition</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="description">(string)The new description.</param>
    ///<returns>(unit) unit</returns>
    static member BlockDescription(blockName:string, description:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
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
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns the insertion point of a block instance.</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Point3d) The insertion 3D point</returns>
    static member BlockInstanceInsertPoint(objectId:Guid) : Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def BlockInstanceInsertPoint(object_id):
        '''Returns the insertion point of a block instance.
        Parameters:
          object_id (guid): The identifier of an existing block insertion object
        Returns:
          point: The insertion 3D point if successful
        '''
        instance = __InstanceObjectFromId(object_id, True)
        xf = instance.InstanceXform
        pt = Rhino.Geometry.Point3d.Origin
        pt.Transform(xf)
        return pt
    *)


    ///<summary>Returns the block name of a block instance</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(string) the block name of a block instance</returns>
    static member BlockInstanceName(objectId:Guid) : string =
        failNotImpl () // genreation temp disabled !!
    (*
    def BlockInstanceName(object_id):
        '''Returns the block name of a block instance
        Parameters:
          object_id (guid): The identifier of an existing block insertion object
        Returns:
          str: the block name of a block instance
        '''
        instance = __InstanceObjectFromId(object_id, True)
        idef = instance.InstanceDefinition
        return idef.Name
    *)


    ///<summary>Returns the identifiers of the inserted instances of a block.</summary>
    ///<param name="blockName">(string) The name of an existing block definition</param>
    ///<param name="whereToLook">(int) Optional, Default Value: <c>0</c>
    ///0 = get top level references in active document.
    ///  1 = get top level and nested references in active document.
    ///  2 = check for references from other instance definitions</param>
    ///<returns>(Guid seq) Ids identifying the instances of a block in the model.</returns>
    static member BlockInstances(blockName:string, [<OPT;DEF(0)>]whereToLook:int) : Guid seq =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns the location of a block instance relative to the world coordinate
    ///  system origin (0,0,0). The position is returned as a 4x4 transformation
    ///  matrix</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(Transform) the location, as a transform matrix, of a block instance relative to the world coordinate
    ///  system origin</returns>
    static member BlockInstanceXform(objectId:Guid) : Transform =
        failNotImpl () // genreation temp disabled !!
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
        instance = __InstanceObjectFromId(object_id, True)
        return instance.InstanceXform
    *)


    ///<summary>Returns the names of all block definitions in the document</summary>
    ///<returns>(string seq) the names of all block definitions in the document</returns>
    static member BlockNames() : string seq =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns number of objects that make up a block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(int) the number of objects that make up a block definition</returns>
    static member BlockObjectCount(blockName:string) : int =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns identifiers of the objects that make up a block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(Guid seq) list of identifiers on success</returns>
    static member BlockObjects(blockName:string) : Guid seq =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns path to the source of a linked or embedded block definition.
    ///  A linked or embedded block definition is a block definition that was
    ///  inserted from an external file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(string) path to the linked block on success</returns>
    static member BlockPath(blockName:string) : string =
        failNotImpl () // genreation temp disabled !!
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
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Deletes a block definition and all of it's inserted instances.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteBlock(blockName:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Explodes a block instance into it's geometric components. The
    ///  exploded objects are added to the document</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<param name="explodeNestedInstances">(bool) Optional, Default Value: <c>false</c>
    ///By default nested blocks are not exploded.</param>
    ///<returns>(Guid seq) identifiers for the newly exploded objects on success</returns>
    static member ExplodeBlockInstance(objectId:Guid, [<OPT;DEF(false)>]explodeNestedInstances:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
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
        instance = __InstanceObjectFromId(object_id, True)
        guids = scriptcontext.doc.Objects.AddExplodedInstancePieces(instance, explodeNestedInstances=explode_nested_instances, deleteInstance=True)
        if guids:
          scriptcontext.doc.Views.Redraw()
        return guids
    *)


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
        failNotImpl () // genreation temp disabled !!
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
          guid: id for the block that was added to the doc
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


    ///<summary>Inserts a block whose definition already exists in the document</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="xform">(Transform) 4x4 transformation matrix to apply</param>
    ///<returns>(Guid) id for the block that was added to the doc on success</returns>
    static member InsertBlock2(blockName:string, xform:Transform) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def InsertBlock2(block_name, xform):
        '''Inserts a block whose definition already exists in the document
        Parameters:
          block_name (str): name of an existing block definition
          xform (transform): 4x4 transformation matrix to apply
        Returns:
          guid: id for the block that was added to the doc on success
        '''
        idef = scriptcontext.doc.InstanceDefinitions.Find(block_name)
        if not idef: raise ValueError("%s does not exist in InstanceDefinitionsTable"%block_name)
        xform = rhutil.coercexform(xform, True)
        id = scriptcontext.doc.Objects.AddInstanceObject(idef.Index, xform )
        if id!=System.Guid.Empty:
            scriptcontext.doc.Views.Redraw()
            return id
    *)


    ///<summary>Verifies the existence of a block definition in the document.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlock(blockName:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Verifies a block definition is embedded, or linked, from an external file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockEmbedded(blockName:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Verifies an object is a block instance</summary>
    ///<param name="objectId">(Guid) The identifier of an existing block insertion object</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockInstance(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsBlockInstance(object_id):
        '''Verifies an object is a block instance
        Parameters:
          object_id (guid): The identifier of an existing block insertion object
        Returns:
          bool: True or False
        '''
        return  __InstanceObjectFromId(object_id, False) is not None
    *)


    ///<summary>Verifies that a block definition is being used by an inserted instance</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="whereToLook">(float) Optional, Default Value: <c>0</c>
    ///One of the following values
    ///  0 = Check for top level references in active document
    ///  1 = Check for top level and nested references in active document
    ///  2 = Check for references in other instance definitions</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockInUse(blockName:string, [<OPT;DEF(0)>]whereToLook:float) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Verifies that a block definition is from a reference file.</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<returns>(bool) True or False</returns>
    static member IsBlockReference(blockName:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Renames an existing block definition</summary>
    ///<param name="blockName">(string) Name of an existing block definition</param>
    ///<param name="newName">(string) Name to change to</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RenameBlock(blockName:string, newName:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


