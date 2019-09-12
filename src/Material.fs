namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsMaterial =
  type RhinoScriptSyntax with
    
    ///<summary>Add material to a layer and returns the new material's index. If the
    ///  layer already has a material, then the layer's current material index is
    ///  returned</summary>
    ///<param name="layer">(string) Name of an existing layer.</param>
    ///<returns>(float) Material index of the layer</returns>
    static member AddMaterialToLayer(layer:string) : float =
        let layer = __getlayer(layer, true)
        if layer.RenderMaterialIndex>-1 then layer.RenderMaterialIndex
        let material_index = Doc.Materials.Add()
        let layer.RenderMaterialIndex = material_index
        Doc.Views.Redraw()
        material_index
        //failwithf "Rhino.Scripting Error:AddMaterialToLayer failed.  layer:"%A"" layer
    (*
    def AddMaterialToLayer(layer):
        """Add material to a layer and returns the new material's index. If the
        layer already has a material, then the layer's current material index is
        returned
        Parameters:
          layer (str): name of an existing layer.
        Returns:
          number: Material index of the layer if successful
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.CurrentLayer()
          index = rs.LayerMaterialIndex(layer)
          if index==-1: index = rs.AddMaterialToLayer(layer)
        See Also:
          LayerMaterialIndex
          IsMaterialDefault
        """
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
        let rhino_object = Coerce.coercerhinoobject(objectId, true, true)
        let attr = rhino_object.Attributes
        if attr.MaterialSource<>Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject then
            let attr.MaterialSource = Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject
            Doc.Objects.ModifyAttributes(rhino_object, attr, true)
            attr <- rhino_object.Attributes
        let material_index = attr.MaterialIndex
        if material_index>-1 then material_index
        material_index <- Doc.Materials.Add()
        let attr.MaterialIndex = material_index
        Doc.Objects.ModifyAttributes(rhino_object, attr, true)
        material_index
    (*
    def AddMaterialToObject(object_id):
        """Adds material to an object and returns the new material's index. If the
        object already has a material, the the object's current material index is
        returned.
        Parameters:
          object_id (guid): identifier of an object
        Returns:
          number: material index of the object
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject()
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index==-1: index = rs.AddMaterialToObject(obj)
        See Also:
          IsMaterialDefault
          ObjectMaterialIndex
          ObjectMaterialSource
        """
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
        if source_index=destinationIndex then false
        let source = Doc.Materials.[source_index]
        if source = null then false
        let rc = Doc.Materials.Modify(source, destinationIndex, true)
        if rc then Doc.Views.Redraw()
        rc
    (*
    def CopyMaterial(source_index, destination_index):
        """Copies definition of a source material to a destination material
        Parameters:
          source_index, destination_index (number): indices of materials to copy
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          src = rs.LayerMaterialIndex("Default")
          dest = rs.LayerMaterialIndex(rs.CurrentLayer())
          if src>=0 and dest>=0 and src!=dest:
              rs.CopyMaterial( src, dest )
        See Also:
          LayerMaterialIndex
          ObjectMaterialIndex
        """
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
        let mat = Doc.Materials.[materialIndex]
        mat && mat.IsDefaultMaterial
    (*
    def IsMaterialDefault(material_index):
        """Verifies a material is a copy of Rhino's built-in "default" material.
        The default material is used by objects and layers that have not been
        assigned a material.
        Parameters:
          material_index (number): the zero-based material index
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject()
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if rs.IsMaterialDefault(index):
                  print "Object is assigned default material."
              else:
                  print "Object is not assigned default material."
        See Also:
          LayerMaterialIndex
          ObjectMaterialIndex
        """
        mat = scriptcontext.doc.Materials[material_index]
        return mat and mat.IsDefaultMaterial
    *)


    ///<summary>Verifies a material is referenced from another file</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialReference(materialIndex:int) : bool =
        let mat = Doc.Materials.[materialIndex]
        mat && mat.IsReference
    (*
    def IsMaterialReference(material_index):
        """Verifies a material is referenced from another file
        Parameters:
          material_index (number): the zero-based material index
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject()
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if rs.IsMaterialReference(index):
                  print "The material is referenced from another file."
              else:
                  print "The material is not referenced from another file."
        See Also:
          IsLayerReference
          IsLightReference
          IsObjectReference
        """
        mat = scriptcontext.doc.Materials[material_index]
        return mat and mat.IsReference
    *)


    ///<summary>Copies the material definition from one material to one or more objects</summary>
    ///<param name="source">(Guid) Source material index -or- identifier of the source object.
    ///  The object must have a material assigned</param>
    ///<param name="destination">(Guid seq) Identifiers(s) of the destination object(s)</param>
    ///<returns>(float) number of objects that were modified</returns>
    static member MatchMaterial(source:Guid, destination:Guid seq) : float =
        let source_id = Coerce.coerceguid(source)
        let source_mat = null
        if source_id then
            let rhobj = Coerce.coercerhinoobject(source_id, true, true)
            let source = rhobj.Attributes.MaterialIndex
        let mat = Doc.Materials.[source]
        if not <| mat then failwithf "Rhino.Scripting Error:MatchMaterial failed.  source:"%A" destination:"%A"" source destination
        let destination_id = Coerce.coerceguid(destination)
        if destination_id then destination <- .[destination]
        let ids = [| for d in destination -> Coerce.coerceguid(d) |]
        let rc = 0
        for id in ids do
            rhobj <- Doc.Objects.Find(id)
            if rhobj then
                let rhobj.Attributes.MaterialIndex = source
                let rhobj.Attributes.MaterialSource = Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject
                rhobj.CommitChanges()
                rc += 1
        if rc>0 then Doc.Views.Redraw()
        rc
    (*
    def MatchMaterial(source, destination):
        """Copies the material definition from one material to one or more objects
        Parameters:
          source (number|guid): source material index -or- identifier of the source object.
            The object must have a material assigned
          destination ([guid, ...]) identifiers(s) of the destination object(s)
        Returns:
          number: number of objects that were modified if successful
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select source object")
          if obj and rs.ObjectMaterialIndex(obj)>-1:
              objects = rs.GetObjects("Select destination objects")
              if objects: rs.MatchMaterial( obj, objects )
        See Also:
          CopyMaterial
          LayerMaterialIndex
          ObjectMaterialIndex
        """
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
    static member MaterialBump(materialIndex:int) : string =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialBump failed.  materialIndex:"%A" filename:"%A"" materialIndex filename
        let texture = mat.GetBumpTexture()
        rc <- texture.FileName if texture else ""
        if filename then
            mat.SetBumpTexture(filename)
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialBump(material_index, filename=None):
        """Returns or modifies a material's bump bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the bump bitmap filename
        Returns:
          str: if filename is not specified, the current bump bitmap filename
          str: if filename is specified, the previous bump bitmap filename
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialBump( index, "C:\\Users\\Steve\\Desktop\\bumpimage.png" )
        See Also:
          MaterialColor
          MaterialName
          MaterialReflectiveColor
          MaterialShine
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialBump(materialIndex:int, filename:string) : unit =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialBump failed.  materialIndex:"%A" filename:"%A"" materialIndex filename
        let texture = mat.GetBumpTexture()
        rc <- texture.FileName if texture else ""
        if filename then
            mat.SetBumpTexture(filename)
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialBump(material_index, filename=None):
        """Returns or modifies a material's bump bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the bump bitmap filename
        Returns:
          str: if filename is not specified, the current bump bitmap filename
          str: if filename is specified, the previous bump bitmap filename
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialBump( index, "C:\\Users\\Steve\\Desktop\\bumpimage.png" )
        See Also:
          MaterialColor
          MaterialName
          MaterialReflectiveColor
          MaterialShine
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialColor(materialIndex:int) : Drawing.Color =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialColor failed.  materialIndex:"%A" color:"%A"" materialIndex color
        let rc = mat.DiffuseColor
        let color = Coerce.coercecolor(color)
        if color then
            let mat.DiffuseColor = color
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialColor(material_index, color=None):
        """Returns or modifies a material's diffuse color.
        Parameters:
          material_index (number): zero based material index
          color (color, optional): the new color value
        Returns:
          color: if color is not specified, the current material color
          color: if color is specified, the previous material color
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialColor( index, (127, 255, 191) )
        See Also:
          MaterialBump
          MaterialName
          MaterialReflectiveColor
          MaterialShine
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialColor(materialIndex:int, color:Drawing.Color) : unit =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialColor failed.  materialIndex:"%A" color:"%A"" materialIndex color
        let rc = mat.DiffuseColor
        let color = Coerce.coercecolor(color)
        if color then
            let mat.DiffuseColor = color
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialColor(material_index, color=None):
        """Returns or modifies a material's diffuse color.
        Parameters:
          material_index (number): zero based material index
          color (color, optional): the new color value
        Returns:
          color: if color is not specified, the current material color
          color: if color is specified, the previous material color
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialColor( index, (127, 255, 191) )
        See Also:
          MaterialBump
          MaterialName
          MaterialReflectiveColor
          MaterialShine
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialEnvironmentMap(materialIndex:int) : string =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialEnvironmentMap failed.  materialIndex:"%A" filename:"%A"" materialIndex filename
        let texture = mat.GetEnvironmentTexture()
        rc <- texture.FileName if texture else ""
        if filename then
            mat.SetEnvironmentTexture(filename)
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialEnvironmentMap(material_index, filename=None):
        """Returns or modifies a material's environment bitmap filename.
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the environment bitmap filename
        Returns:
          str: if filename is not specified, the current environment bitmap filename
          str: if filename is specified, the previous environment bitmap filename
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialEnvironmentMap( index, "C:\\Users\\Steve\\Desktop\\emapimage.png" )
        See Also:
          MaterialBump
          MaterialTexture
          MaterialTransparencyMap
        """
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
    static member MaterialEnvironmentMap(materialIndex:int, filename:string) : unit =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialEnvironmentMap failed.  materialIndex:"%A" filename:"%A"" materialIndex filename
        let texture = mat.GetEnvironmentTexture()
        rc <- texture.FileName if texture else ""
        if filename then
            mat.SetEnvironmentTexture(filename)
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialEnvironmentMap(material_index, filename=None):
        """Returns or modifies a material's environment bitmap filename.
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the environment bitmap filename
        Returns:
          str: if filename is not specified, the current environment bitmap filename
          str: if filename is specified, the previous environment bitmap filename
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialEnvironmentMap( index, "C:\\Users\\Steve\\Desktop\\emapimage.png" )
        See Also:
          MaterialBump
          MaterialTexture
          MaterialTransparencyMap
        """
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
    static member MaterialName(materialIndex:int) : string =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialName failed.  materialIndex:"%A" name:"%A"" materialIndex name
        let rc = mat.Name
        if name then
            let mat.Name = name
            mat.CommitChanges()
        rc
    (*
    def MaterialName(material_index, name=None):
        """Returns or modifies a material's user defined name
        Parameters:
          material_index (number): zero based material index
          name (str, optional): the new name
        Returns:
          str: if name is not specified, the current material name
          str: if name is specified, the previous material name
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialName( index, "Fancy_Material" )
        See Also:
          MaterialBump
          MaterialColor
          MaterialReflectiveColor
          MaterialShine
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialName(materialIndex:int, name:string) : unit =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialName failed.  materialIndex:"%A" name:"%A"" materialIndex name
        let rc = mat.Name
        if name then
            let mat.Name = name
            mat.CommitChanges()
        rc
    (*
    def MaterialName(material_index, name=None):
        """Returns or modifies a material's user defined name
        Parameters:
          material_index (number): zero based material index
          name (str, optional): the new name
        Returns:
          str: if name is not specified, the current material name
          str: if name is specified, the previous material name
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialName( index, "Fancy_Material" )
        See Also:
          MaterialBump
          MaterialColor
          MaterialReflectiveColor
          MaterialShine
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialReflectiveColor(materialIndex:int) : Drawing.Color =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialReflectiveColor failed.  materialIndex:"%A" color:"%A"" materialIndex color
        let rc = mat.ReflectionColor
        let color = Coerce.coercecolor(color)
        if color then
            let mat.ReflectionColor = color
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialReflectiveColor(material_index, color=None):
        """Returns or modifies a material's reflective color.
        Parameters:
          material_index (number): zero based material index
          color (color, optional): the new color value
        Returns:
          color: if color is not specified, the current material reflective color
          color: if color is specified, the previous material reflective color
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialReflectiveColor( index, (191, 191, 255) )
        See Also:
          MaterialBump
          MaterialColor
          MaterialName
          MaterialShine
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialReflectiveColor(materialIndex:int, color:Drawing.Color) : unit =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialReflectiveColor failed.  materialIndex:"%A" color:"%A"" materialIndex color
        let rc = mat.ReflectionColor
        let color = Coerce.coercecolor(color)
        if color then
            let mat.ReflectionColor = color
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialReflectiveColor(material_index, color=None):
        """Returns or modifies a material's reflective color.
        Parameters:
          material_index (number): zero based material index
          color (color, optional): the new color value
        Returns:
          color: if color is not specified, the current material reflective color
          color: if color is specified, the previous material reflective color
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialReflectiveColor( index, (191, 191, 255) )
        See Also:
          MaterialBump
          MaterialColor
          MaterialName
          MaterialShine
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialShine(materialIndex:int) : int =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialShine failed.  materialIndex:"%A" shine:"%A"" materialIndex shine
        let rc = mat.Shine
        if shine then
            let mat.Shine = shine
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialShine(material_index, shine=None):
        """Returns or modifies a material's shine value
        Parameters:
          material_index (number): zero based material index
          shine (number, optional): the new shine value. A material's shine value ranges from 0.0 to 255.0, with
            0.0 being matte and 255.0 being glossy
        Returns:
          number: if shine is not specified, the current material shine value
          number: if shine is specified, the previous material shine value
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          MAX_SHINE = 255.0
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialShine( index, MAX_SHINE/2 )
        See Also:
          MaterialBump
          MaterialColor
          MaterialName
          MaterialReflectiveColor
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialShine(materialIndex:int, shine:float) : unit =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialShine failed.  materialIndex:"%A" shine:"%A"" materialIndex shine
        let rc = mat.Shine
        if shine then
            let mat.Shine = shine
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialShine(material_index, shine=None):
        """Returns or modifies a material's shine value
        Parameters:
          material_index (number): zero based material index
          shine (number, optional): the new shine value. A material's shine value ranges from 0.0 to 255.0, with
            0.0 being matte and 255.0 being glossy
        Returns:
          number: if shine is not specified, the current material shine value
          number: if shine is specified, the previous material shine value
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          MAX_SHINE = 255.0
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialShine( index, MAX_SHINE/2 )
        See Also:
          MaterialBump
          MaterialColor
          MaterialName
          MaterialReflectiveColor
          MaterialTexture
          MaterialTransparency
        """
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
    static member MaterialTexture(materialIndex:int) : string =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialTexture failed.  materialIndex:"%A" filename:"%A"" materialIndex filename
        let texture = mat.GetBitmapTexture()
        rc <- texture.FileName if texture else ""
        if filename then
            mat.SetBitmapTexture(filename)
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialTexture(material_index, filename=None):
        """Returns or modifies a material's texture bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the texture bitmap filename
        Returns:
          str: if filename is not specified, the current texture bitmap filename
          str: if filename is specified, the previous texture bitmap filename
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialTexture( index, "C:\\Users\\Steve\\Desktop\\textureimage.png" )
        See Also:
          MaterialBump
          MaterialColor
          MaterialName
          MaterialReflectiveColor
          MaterialShine
          MaterialTransparency
        """
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
    static member MaterialTexture(materialIndex:int, filename:string) : unit =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialTexture failed.  materialIndex:"%A" filename:"%A"" materialIndex filename
        let texture = mat.GetBitmapTexture()
        rc <- texture.FileName if texture else ""
        if filename then
            mat.SetBitmapTexture(filename)
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialTexture(material_index, filename=None):
        """Returns or modifies a material's texture bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the texture bitmap filename
        Returns:
          str: if filename is not specified, the current texture bitmap filename
          str: if filename is specified, the previous texture bitmap filename
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialTexture( index, "C:\\Users\\Steve\\Desktop\\textureimage.png" )
        See Also:
          MaterialBump
          MaterialColor
          MaterialName
          MaterialReflectiveColor
          MaterialShine
          MaterialTransparency
        """
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
    static member MaterialTransparency(materialIndex:int) : int =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialTransparency failed.  materialIndex:"%A" transparency:"%A"" materialIndex transparency
        let rc = mat.Transparency
        if transparency then
            let mat.Transparency = transparency
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialTransparency(material_index, transparency=None):
        """Returns or modifies a material's transparency value
        Parameters:
          material_index (number): zero based material index
          transparency (number, optional): the new transparency value. A material's transparency value ranges from 0.0 to 1.0, with
            0.0 being opaque and 1.0 being transparent
        Returns:
          number: if transparency is not specified, the current material transparency value
          number: if transparency is specified, the previous material transparency value
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialTransparency( index, 0.50 )
        See Also:
          MaterialBump
          MaterialColor
          MaterialName
          MaterialReflectiveColor
          MaterialShine
          MaterialTexture
        """
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
    static member MaterialTransparency(materialIndex:int, transparency:float) : unit =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialTransparency failed.  materialIndex:"%A" transparency:"%A"" materialIndex transparency
        let rc = mat.Transparency
        if transparency then
            let mat.Transparency = transparency
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialTransparency(material_index, transparency=None):
        """Returns or modifies a material's transparency value
        Parameters:
          material_index (number): zero based material index
          transparency (number, optional): the new transparency value. A material's transparency value ranges from 0.0 to 1.0, with
            0.0 being opaque and 1.0 being transparent
        Returns:
          number: if transparency is not specified, the current material transparency value
          number: if transparency is specified, the previous material transparency value
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialTransparency( index, 0.50 )
        See Also:
          MaterialBump
          MaterialColor
          MaterialName
          MaterialReflectiveColor
          MaterialShine
          MaterialTexture
        """
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
    static member MaterialTransparencyMap(materialIndex:int) : string =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialTransparencyMap failed.  materialIndex:"%A" filename:"%A"" materialIndex filename
        let texture = mat.GetTransparencyTexture()
        rc <- texture.FileName if texture else ""
        if filename then
            mat.SetTransparencyTexture(filename)
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialTransparencyMap(material_index, filename=None):
        """Returns or modifies a material's transparency bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the transparency bitmap filename
        Returns:
          str: if filename is not specified, the current transparency bitmap filename
          str: if filename is specified, the previous transparency bitmap filename
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialTransparencyMap( index, "C:\\Users\\Steve\\Desktop\\texture.png" )
        See Also:
          MaterialBump
          MaterialEnvironmentMap
          MaterialTexture
        """
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
    static member MaterialTransparencyMap(materialIndex:int, filename:string) : unit =
        let mat = Doc.Materials.[materialIndex]
        if mat = null then failwithf "Rhino.Scripting Error:MaterialTransparencyMap failed.  materialIndex:"%A" filename:"%A"" materialIndex filename
        let texture = mat.GetTransparencyTexture()
        rc <- texture.FileName if texture else ""
        if filename then
            mat.SetTransparencyTexture(filename)
            mat.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def MaterialTransparencyMap(material_index, filename=None):
        """Returns or modifies a material's transparency bitmap filename
        Parameters:
          material_index (number): zero based material index
          filename (str, optional): the transparency bitmap filename
        Returns:
          str: if filename is not specified, the current transparency bitmap filename
          str: if filename is specified, the previous transparency bitmap filename
          None: if not successful or on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1:
                  rs.MaterialTransparencyMap( index, "C:\\Users\\Steve\\Desktop\\texture.png" )
        See Also:
          MaterialBump
          MaterialEnvironmentMap
          MaterialTexture
        """
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
        let mat = Doc.Materials.[materialIndex]
        if mat = null then false
        let rc = Doc.Materials.ResetMaterial(materialIndex)
        Doc.Views.Redraw()
        rc
    (*
    def ResetMaterial(material_index):
        """Resets a material to Rhino's default material
        Parameters:
          material_index (number) zero based material index
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if obj:
              index = rs.ObjectMaterialIndex(obj)
              if index>-1: rs.ResetMaterial(index)
        See Also:
          LayerMaterialIndex
          ObjectMaterialIndex
        """
        mat = scriptcontext.doc.Materials[material_index]
        if mat is None: return False
        rc = scriptcontext.doc.Materials.ResetMaterial(material_index)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


