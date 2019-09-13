namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
[<AutoOpen>]
module ExtensionsMaterial =
  type RhinoScriptSyntax with
    
    ///<summary>Add material to a layer and returns the new material's index. If the
    ///  layer already has a material, then the layer's current material index is
    ///  returned</summary>
    ///<param name="layer">(string) Name of an existing layer.</param>
    ///<returns>(float) Material index of the layer</returns>
    static member AddMaterialToLayer(layer:string) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddMaterialToLayer(layer):
        '''Add material to a layer and returns the new material's index. If the
        layer already has a material, then the layer's current material index is
        returned
        Parameters:
          layer (str): name of an existing layer.
        Returns:
          number: Material index of the layer if successful
          None: if not successful or on error
        '''
        layer = __getlayer(layer, True)
        if layer.RenderMaterialIndex>-1: return layer.RenderMaterialIndex
        material_index = scriptcontext.doc.Materials.Add()
        layer.RenderMaterialIndex = material_index
        scriptcontext.doc.Views.Redraw()
        return material_index
        #return scriptcontext.errorhandler()
    *)


    ///<summary>Adds material to an object and returns the new material's index. If the
    ///  object already has a material, the the object's current material index is
    ///  returned.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(float) material index of the object</returns>
    static member AddMaterialToObject(objectId:Guid) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddMaterialToObject(object_id):
        '''Adds material to an object and returns the new material's index. If the
        object already has a material, the the object's current material index is
        returned.
        Parameters:
          object_id (guid): identifier of an object
        Returns:
          number: material index of the object
        '''
        rhino_object = rhutil.coercerhinoobject(object_id, True, True)
        attr = rhino_object.Attributes
        if attr.MaterialSource!=Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject:
            attr.MaterialSource = Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject
            scriptcontext.doc.Objects.ModifyAttributes(rhino_object, attr, True)
            attr = rhino_object.Attributes
        material_index = attr.MaterialIndex
        if material_index>-1: return material_index
        material_index = scriptcontext.doc.Materials.Add()
        attr.MaterialIndex = material_index
        scriptcontext.doc.Objects.ModifyAttributes(rhino_object, attr, True)
        return material_index
    *)


    ///<summary>Copies definition of a source material to a destination material</summary>
    ///<param name="sourceIndex">(int) Source index of 'indices of materials to copy' (FIXME 0)</param>
    ///<param name="destinationIndex">(int) Destination index of 'indices of materials to copy' (FIXME 0)</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CopyMaterial(sourceIndex:int, destinationIndex:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def CopyMaterial(source_index, destination_index):
        '''Copies definition of a source material to a destination material
        Parameters:
          source_index, destination_index (number): indices of materials to copy
        Returns:
          bool: True or False indicating success or failure
        '''
        if source_index==destination_index: return False
        source = scriptcontext.doc.Materials[source_index]
        if source is None: return False
        rc = scriptcontext.doc.Materials.Modify(source, destination_index, True)
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Verifies a material is a copy of Rhino's built-in "default" material.
    ///  The default material is used by objects and layers that have not been
    ///  assigned a material.</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialDefault(materialIndex:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsMaterialDefault(material_index):
        '''Verifies a material is a copy of Rhino's built-in "default" material.
        The default material is used by objects and layers that have not been
        assigned a material.
        Parameters:
          material_index (number): the zero-based material index
        Returns:
          bool: True or False indicating success or failure
        '''
        mat = scriptcontext.doc.Materials[material_index]
        return mat and mat.IsDefaultMaterial
    *)


    ///<summary>Verifies a material is referenced from another file</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialReference(materialIndex:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsMaterialReference(material_index):
        '''Verifies a material is referenced from another file
        Parameters:
          material_index (number): the zero-based material index
        Returns:
          bool: True or False indicating success or failure
        '''
        mat = scriptcontext.doc.Materials[material_index]
        return mat and mat.IsReference
    *)


    ///<summary>Copies the material definition from one material to one or more objects</summary>
    ///<param name="source">(Guid) Source material index -or- identifier of the source object.
    ///  The object must have a material assigned</param>
    ///<param name="destination">(Guid seq) Identifiers(s) of the destination object(s)</param>
    ///<returns>(float) number of objects that were modified</returns>
    static member MatchMaterial(source:Guid, destination:Guid seq) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def MatchMaterial(source, destination):
        '''Copies the material definition from one material to one or more objects
        Parameters:
          source (number|guid): source material index -or- identifier of the source object.
            The object must have a material assigned
          destination ([guid, ...]) identifiers(s) of the destination object(s)
        Returns:
          number: number of objects that were modified if successful
          None: if not successful or on error
        '''
        source_id = rhutil.coerceguid(source)
        source_mat = None
        if source_id:
            rhobj = rhutil.coercerhinoobject(source_id, True, True)
            source = rhobj.Attributes.MaterialIndex
        mat = scriptcontext.doc.Materials[source]
        if not mat: return scriptcontext.errorhandler()
        destination_id = rhutil.coerceguid(destination)
        if destination_id: destination = [destination]
        ids = [rhutil.coerceguid(d) for d in destination]
        rc = 0
        for id in ids:
            rhobj = scriptcontext.doc.Objects.Find(id)
            if rhobj:
                rhobj.Attributes.MaterialIndex = source
                rhobj.Attributes.MaterialSource = Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject
                rhobj.CommitChanges()
                rc += 1
        if rc>0: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns a material's bump bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current bump bitmap filename</returns>
    static member MaterialBump(materialIndex:int) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialBump(material_index, filename=None):
        '''Returns or modifies a material's bump bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the bump bitmap filename
        Returns:
          str: if filename is not specified, the current bump bitmap filename
          str: if filename is specified, the previous bump bitmap filename
          None: if not successful or on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        texture = mat.GetBumpTexture()
        rc = texture.FileName if texture else ""
        if filename:
            mat.SetBumpTexture(filename)
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies a material's bump bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string)The bump bitmap filename</param>
    ///<returns>(unit) unit</returns>
    static member MaterialBump(materialIndex:int, filename:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialBump(material_index, filename=None):
        '''Returns or modifies a material's bump bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the bump bitmap filename
        Returns:
          str: if filename is not specified, the current bump bitmap filename
          str: if filename is specified, the previous bump bitmap filename
          None: if not successful or on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        texture = mat.GetBumpTexture()
        rc = texture.FileName if texture else ""
        if filename:
            mat.SetBumpTexture(filename)
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns a material's diffuse color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material color</returns>
    static member MaterialColor(materialIndex:int) : Drawing.Color = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialColor(material_index, color=None):
        '''Returns or modifies a material's diffuse color.
        Parameters:
          material_index (number): zero based material index
          color (color, optional): the new color value
        Returns:
          color: if color is not specified, the current material color
          color: if color is specified, the previous material color
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.DiffuseColor
        color = rhutil.coercecolor(color)
        if color:
            mat.DiffuseColor = color
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies a material's diffuse color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color)The new color value</param>
    ///<returns>(unit) unit</returns>
    static member MaterialColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialColor(material_index, color=None):
        '''Returns or modifies a material's diffuse color.
        Parameters:
          material_index (number): zero based material index
          color (color, optional): the new color value
        Returns:
          color: if color is not specified, the current material color
          color: if color is specified, the previous material color
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.DiffuseColor
        color = rhutil.coercecolor(color)
        if color:
            mat.DiffuseColor = color
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns a material's environment bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current environment bitmap filename</returns>
    static member MaterialEnvironmentMap(materialIndex:int) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialEnvironmentMap(material_index, filename=None):
        '''Returns or modifies a material's environment bitmap filename.
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the environment bitmap filename
        Returns:
          str: if filename is not specified, the current environment bitmap filename
          str: if filename is specified, the previous environment bitmap filename
          None: if not successful or on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        texture = mat.GetEnvironmentTexture()
        rc = texture.FileName if texture else ""
        if filename:
            mat.SetEnvironmentTexture(filename)
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies a material's environment bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string)The environment bitmap filename</param>
    ///<returns>(unit) unit</returns>
    static member MaterialEnvironmentMap(materialIndex:int, filename:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialEnvironmentMap(material_index, filename=None):
        '''Returns or modifies a material's environment bitmap filename.
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the environment bitmap filename
        Returns:
          str: if filename is not specified, the current environment bitmap filename
          str: if filename is specified, the previous environment bitmap filename
          None: if not successful or on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        texture = mat.GetEnvironmentTexture()
        rc = texture.FileName if texture else ""
        if filename:
            mat.SetEnvironmentTexture(filename)
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns a material's user defined name</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current material name</returns>
    static member MaterialName(materialIndex:int) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialName(material_index, name=None):
        '''Returns or modifies a material's user defined name
        Parameters:
          material_index (number): zero based material index
          name (str, optional): the new name
        Returns:
          str: if name is not specified, the current material name
          str: if name is specified, the previous material name
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.Name
        if name:
            mat.Name = name
            mat.CommitChanges()
        return rc
    *)

    ///<summary>Modifies a material's user defined name</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="name">(string)The new name</param>
    ///<returns>(unit) unit</returns>
    static member MaterialName(materialIndex:int, name:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialName(material_index, name=None):
        '''Returns or modifies a material's user defined name
        Parameters:
          material_index (number): zero based material index
          name (str, optional): the new name
        Returns:
          str: if name is not specified, the current material name
          str: if name is specified, the previous material name
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.Name
        if name:
            mat.Name = name
            mat.CommitChanges()
        return rc
    *)


    ///<summary>Returns a material's reflective color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material reflective color</returns>
    static member MaterialReflectiveColor(materialIndex:int) : Drawing.Color = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialReflectiveColor(material_index, color=None):
        '''Returns or modifies a material's reflective color.
        Parameters:
          material_index (number): zero based material index
          color (color, optional): the new color value
        Returns:
          color: if color is not specified, the current material reflective color
          color: if color is specified, the previous material reflective color
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.ReflectionColor
        color = rhutil.coercecolor(color)
        if color:
            mat.ReflectionColor = color
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies a material's reflective color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color)The new color value</param>
    ///<returns>(unit) unit</returns>
    static member MaterialReflectiveColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialReflectiveColor(material_index, color=None):
        '''Returns or modifies a material's reflective color.
        Parameters:
          material_index (number): zero based material index
          color (color, optional): the new color value
        Returns:
          color: if color is not specified, the current material reflective color
          color: if color is specified, the previous material reflective color
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.ReflectionColor
        color = rhutil.coercecolor(color)
        if color:
            mat.ReflectionColor = color
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns a material's shine value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(int) The current material shine value
    ///  0.0 being matte and 255.0 being glossy</returns>
    static member MaterialShine(materialIndex:int) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialShine(material_index, shine=None):
        '''Returns or modifies a material's shine value
        Parameters:
          material_index (number): zero based material index
          shine (number, optional): the new shine value. A material's shine value ranges from 0.0 to 255.0, with
            0.0 being matte and 255.0 being glossy
        Returns:
          number: if shine is not specified, the current material shine value
          number: if shine is specified, the previous material shine value
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.Shine
        if shine:
            mat.Shine = shine
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies a material's shine value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="shine">(float)The new shine value. A material's shine value ranges from 0.0 to 255.0, with
    ///  0.0 being matte and 255.0 being glossy</param>
    ///<returns>(unit) unit</returns>
    static member MaterialShine(materialIndex:int, shine:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialShine(material_index, shine=None):
        '''Returns or modifies a material's shine value
        Parameters:
          material_index (number): zero based material index
          shine (number, optional): the new shine value. A material's shine value ranges from 0.0 to 255.0, with
            0.0 being matte and 255.0 being glossy
        Returns:
          number: if shine is not specified, the current material shine value
          number: if shine is specified, the previous material shine value
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.Shine
        if shine:
            mat.Shine = shine
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns a material's texture bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current texture bitmap filename</returns>
    static member MaterialTexture(materialIndex:int) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialTexture(material_index, filename=None):
        '''Returns or modifies a material's texture bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the texture bitmap filename
        Returns:
          str: if filename is not specified, the current texture bitmap filename
          str: if filename is specified, the previous texture bitmap filename
          None: if not successful or on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        texture = mat.GetBitmapTexture()
        rc = texture.FileName if texture else ""
        if filename:
            mat.SetBitmapTexture(filename)
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies a material's texture bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string)The texture bitmap filename</param>
    ///<returns>(unit) unit</returns>
    static member MaterialTexture(materialIndex:int, filename:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialTexture(material_index, filename=None):
        '''Returns or modifies a material's texture bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the texture bitmap filename
        Returns:
          str: if filename is not specified, the current texture bitmap filename
          str: if filename is specified, the previous texture bitmap filename
          None: if not successful or on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        texture = mat.GetBitmapTexture()
        rc = texture.FileName if texture else ""
        if filename:
            mat.SetBitmapTexture(filename)
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns a material's transparency value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(int) The current material transparency value
    ///  0.0 being opaque and 1.0 being transparent</returns>
    static member MaterialTransparency(materialIndex:int) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialTransparency(material_index, transparency=None):
        '''Returns or modifies a material's transparency value
        Parameters:
          material_index (number): zero based material index
          transparency (number, optional): the new transparency value. A material's transparency value ranges from 0.0 to 1.0, with
            0.0 being opaque and 1.0 being transparent
        Returns:
          number: if transparency is not specified, the current material transparency value
          number: if transparency is specified, the previous material transparency value
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.Transparency
        if transparency:
            mat.Transparency = transparency
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies a material's transparency value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="transparency">(float)The new transparency value. A material's transparency value ranges from 0.0 to 1.0, with
    ///  0.0 being opaque and 1.0 being transparent</param>
    ///<returns>(unit) unit</returns>
    static member MaterialTransparency(materialIndex:int, transparency:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialTransparency(material_index, transparency=None):
        '''Returns or modifies a material's transparency value
        Parameters:
          material_index (number): zero based material index
          transparency (number, optional): the new transparency value. A material's transparency value ranges from 0.0 to 1.0, with
            0.0 being opaque and 1.0 being transparent
        Returns:
          number: if transparency is not specified, the current material transparency value
          number: if transparency is specified, the previous material transparency value
          None: on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        rc = mat.Transparency
        if transparency:
            mat.Transparency = transparency
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns a material's transparency bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current transparency bitmap filename</returns>
    static member MaterialTransparencyMap(materialIndex:int) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialTransparencyMap(material_index, filename=None):
        '''Returns or modifies a material's transparency bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the transparency bitmap filename
        Returns:
          str: if filename is not specified, the current transparency bitmap filename
          str: if filename is specified, the previous transparency bitmap filename
          None: if not successful or on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        texture = mat.GetTransparencyTexture()
        rc = texture.FileName if texture else ""
        if filename:
            mat.SetTransparencyTexture(filename)
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies a material's transparency bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string)The transparency bitmap filename</param>
    ///<returns>(unit) unit</returns>
    static member MaterialTransparencyMap(materialIndex:int, filename:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def MaterialTransparencyMap(material_index, filename=None):
        '''Returns or modifies a material's transparency bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the transparency bitmap filename
        Returns:
          str: if filename is not specified, the current transparency bitmap filename
          str: if filename is specified, the previous transparency bitmap filename
          None: if not successful or on error
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return scriptcontext.errorhandler()
        texture = mat.GetTransparencyTexture()
        rc = texture.FileName if texture else ""
        if filename:
            mat.SetTransparencyTexture(filename)
            mat.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Resets a material to Rhino's default material</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ResetMaterial(materialIndex:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def ResetMaterial(material_index):
        '''Resets a material to Rhino's default material
        Parameters:
          material_index (number) zero based material index
        Returns:
          bool: True or False indicating success or failure
        '''
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return False
        rc = scriptcontext.doc.Materials.ResetMaterial(material_index)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


