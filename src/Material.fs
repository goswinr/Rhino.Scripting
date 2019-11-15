namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsMaterial =
  
  type RhinoScriptSyntax with


    [<EXT>]
    ///<summary>Add material to a layer and returns the new material's index. If the
    ///  layer already has a material, then the layer's current material index is
    ///  returned</summary>
    ///<param name="layer">(string) Name of an existing layer.</param>
    ///<returns>(int) Material index of the layer</returns>
    static member AddMaterialToLayer(layer:string) : int =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if layer.RenderMaterialIndex> -1 then layer.RenderMaterialIndex
        else
            let materialindex = Doc.Materials.Add()
            layer.RenderMaterialIndex <- materialindex
            Doc.Views.Redraw()
            materialindex
        
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


    [<EXT>]
    ///<summary>Adds material to an object and returns the new material's index. If the
    ///  object already has a material, the the object's current material index is
    ///  returned.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(int) material index of the object</returns>
    static member AddMaterialToObject(objectId:Guid) : int =
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let mutable attr = rhinoobject.Attributes
        if attr.MaterialSource <> Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject then
            attr.MaterialSource <- Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject
            Doc.Objects.ModifyAttributes(rhinoobject, attr, true)|> ignore
            attr <- rhinoobject.Attributes
        let mutable materialindex = attr.MaterialIndex
        if materialindex> -1 then materialindex
        else
            materialindex <- Doc.Materials.Add()
            attr.MaterialIndex <- materialindex
            Doc.Objects.ModifyAttributes(rhinoobject, attr, true)|> ignore
            materialindex
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


    [<EXT>]
    ///<summary>Copies definition of a source material to a destination material</summary>
    ///<param name="sourceIndex">(int) Source index of 'indices of materials to copy' (FIXME 0)</param>
    ///<param name="destinationIndex">(int) Destination index of 'indices of materials to copy' (FIXME 0)</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CopyMaterial(sourceIndex:int, destinationIndex:int) : bool =
        if sourceIndex = destinationIndex then true // orignaly false
        else
            let source = Doc.Materials.[sourceIndex]
            if source|> isNull  then false
            else
                let rc = Doc.Materials.Modify(source, destinationIndex, true)
                if rc then Doc.Views.Redraw()
                rc
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


    [<EXT>]
    ///<summary>Verifies a material is a copy of Rhino's built-in "default" material.
    ///  The default material is used by objects and layers that have not been
    ///  assigned a material.</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialDefault(materialIndex:int) : bool =
        let mat = Doc.Materials.[materialIndex]
        notNull mat && mat.IsDefaultMaterial
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


    [<EXT>]
    ///<summary>Verifies a material is referenced from another file</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialReference(materialIndex:int) : bool =
        let mat = Doc.Materials.[materialIndex]
        notNull mat && mat.IsReference
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


    [<EXT>]
    ///<summary>Copies the material definition from one material to one or more objects</summary>
    ///<param name="source">(Guid) Source material index -or- identifier of the source object.
    ///  The object must have a material assigned</param>
    ///<param name="destination">(Guid seq) Identifiers(s) of the destination object(s)</param>
    ///<returns>(unit) void, nothing</returns>
    static member MatchMaterial(source:Guid, destination:Guid seq) : unit =        
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(source)
        let source = rhobj.Attributes.MaterialIndex
        let mat = Doc.Materials.[source]
        if isNull mat then failwithf "Rhino.Scripting: MatchMaterial failed.  source:'%A' destination:'%A'" source destination
        
        for objectId in destination do
            let rhobj = Doc.Objects.FindId(objectId)
            if notNull rhobj then
                rhobj.Attributes.MaterialIndex <- source
                rhobj.Attributes.MaterialSource <- Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject
                rhobj.CommitChanges()|> ignore
        Doc.Views.Redraw()
        
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
            rhobj = scriptcontext.doc.Objects.FindId(id)
            if rhobj:
                rhobj.Attributes.MaterialIndex = source
                rhobj.Attributes.MaterialSource = Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject
                rhobj.CommitChanges()
                rc += 1
        if rc>0: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns a material's bump bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string Option) The current bump bitmap filename /returns>
    static member MaterialBump(materialIndex:int) : string option= //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialBump failed.  materialIndex:'%A' " materialIndex 
        let texture = mat.GetBumpTexture()
        if notNull texture then Some texture.FileName else None
        
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
    ///<returns>(unit) void, nothing</returns>
    static member MaterialBump(materialIndex:int, filename:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        let texture = mat.GetBumpTexture()
        if IO.File.Exists filename then
            if not <| mat.SetBumpTexture(filename)then failwithf "Rhino.Scripting: MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            mat.CommitChanges()|> ignore
            Doc.Views.Redraw()
        else
            failwithf "Rhino.Scripting: MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
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


    [<EXT>]
    ///<summary>Returns a material's diffuse color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material color</returns>
    static member MaterialColor(materialIndex:int) : Drawing.Color = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialColor failed.  materialIndex:'%A'" materialIndex 
        let rc = mat.DiffuseColor        
        rc
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
    ///<returns>(unit) void, nothing</returns>
    static member MaterialColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialColor failed.  materialIndex:'%A' color:'%A'" materialIndex color        
        mat.DiffuseColor <- color
        mat.CommitChanges()|> ignore
        Doc.Views.Redraw()
        

    [<EXT>]
    ///<summary>Returns a material's environment bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current environment bitmap filename</returns>
    static member MaterialEnvironmentMap(materialIndex:int) : string option= //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialEnvironmentMap failed.  materialIndex:'%A'" materialIndex 
        let texture = mat.GetEnvironmentTexture()
        if notNull texture then Some texture.FileName  else None
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
    ///<returns>(unit) void, nothing</returns>
    static member MaterialEnvironmentMap(materialIndex:int, filename:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename       
        if IO.File.Exists filename then
            if not <| mat.SetEnvironmentTexture(filename) then failwithf "Rhino.Scripting: MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename 
            mat.CommitChanges() |> ignore
            Doc.Views.Redraw()
        else
            failwithf "Rhino.Scripting: MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename 
   


    [<EXT>]
    ///<summary>Returns a material's user defined name</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current material name</returns>
    static member MaterialName(materialIndex:int) : string = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialName failed.  materialIndex:'%A' " materialIndex
        let rc = mat.Name
        rc
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
    ///<returns>(unit) void, nothing</returns>
    static member MaterialName(materialIndex:int, name:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialName failed.  materialIndex:'%A' name:'%A'" materialIndex name        
        mat.Name <- name
        mat.CommitChanges()|> ignore
        
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


    [<EXT>]
    ///<summary>Returns a material's reflective color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material reflective color</returns>
    static member MaterialReflectiveColor(materialIndex:int) : Drawing.Color = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialReflectiveColor failed.  materialIndex:'%A' " materialIndex 
        let rc = mat.ReflectionColor
        rc
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
    ///<returns>(unit) void, nothing</returns>
    static member MaterialReflectiveColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialReflectiveColor failed.  materialIndex:'%A' color:'%A'" materialIndex color        
        mat.ReflectionColor <- color
        mat.CommitChanges() |> ignore
        Doc.Views.Redraw()
        
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


    [<EXT>]
    ///<summary>Returns a material's shine value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(int) The current material shine value
    ///  0.0 being matte and 255.0 being glossy</returns>
    static member MaterialShine(materialIndex:int) : int = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialShine failed.  materialIndex:'%A' " materialIndex 
        let rc = mat.Shine
        rc
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
    ///<returns>(unit) void, nothing</returns>
    static member MaterialShine(materialIndex:int, shine:float) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialShine failed.  materialIndex:'%A' shine:'%A'" materialIndex shine
        
        mat.Shine <- shine
        mat.CommitChanges() |> ignore
        Doc.Views.Redraw()
        
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


    [<EXT>]
    ///<summary>Returns a material's texture bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current texture bitmap filename</returns>
    static member MaterialTexture(materialIndex:int) : string option = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialTexture failed.  materialIndex:'%A' " materialIndex 
        let texture = mat.GetBitmapTexture()
        if notNull texture then  Some texture.FileName else None
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
    ///<returns>(unit) void, nothing</returns>
    static member MaterialTexture(materialIndex:int, filename:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename      
        if IO.File.Exists filename then
            if  not <| mat.SetBitmapTexture(filename) then failwithf "Rhino.Scripting: MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename 
            mat.CommitChanges() |> ignore
            Doc.Views.Redraw()
        else
            failwithf "Rhino.Scripting: MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename    
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


    [<EXT>]
    ///<summary>Returns a material's transparency value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(float) The current material transparency value
    ///  0.0 being opaque and 1.0 being transparent</returns>
    static member MaterialTransparency(materialIndex:int) : float = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialTransparency failed.  materialIndex:'%A' " materialIndex
        let rc = mat.Transparency
        rc
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
    ///<returns>(unit) void, nothing</returns>
    static member MaterialTransparency(materialIndex:int, transparency:float) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialTransparency failed.  materialIndex:'%A' transparency:'%A'" materialIndex transparency        
        mat.Transparency <- transparency
        mat.CommitChanges() |> ignore
        Doc.Views.Redraw()
       
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


    [<EXT>]
    ///<summary>Returns a material's transparency bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current transparency bitmap filename</returns>
    static member MaterialTransparencyMap(materialIndex:int) : string option = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialTransparencyMap failed.  materialIndex:'%A' " materialIndex 
        let texture = mat.GetTransparencyTexture()
        if notNull texture then  Some texture.FileName else None
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
    ///<returns>(unit) void, nothing</returns>
    static member MaterialTransparencyMap(materialIndex:int, filename:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        let texture = mat.GetTransparencyTexture()
        if IO.File.Exists filename then
            if  not <| mat.SetTransparencyTexture(filename) then failwithf "Rhino.Scripting: MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename 
            mat.CommitChanges() |> ignore
            Doc.Views.Redraw()
        else
            failwithf "Rhino.Scripting: MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename  


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


    [<EXT>]
    ///<summary>Resets a material to Rhino's default material</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ResetMaterial(materialIndex:int) : bool =
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then false
        else
            let rc = Doc.Materials.ResetMaterial(materialIndex)
            Doc.Views.Redraw()
            rc
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


