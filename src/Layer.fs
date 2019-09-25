namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsLayer =
  [<EXT>] 
  type RhinoScriptSyntax with
    
    [<EXT>]
    
    static member internal Getlayer() : obj =
        failNotImpl () // genreation temp disabled !!
    (*
    def __getlayer(name_or_id, raise_if_missing):
        ''''''
    *)


    [<EXT>]
    ///<summary>Add a new layer to the document</summary>
    ///<param name="name">(string) Optional, Default Value: <c>null:string</c>
    ///The name of the new layer. If omitted, Rhino automatically
    ///  generates the layer name.</param>
    ///<param name="color">(Drawing.Color) Optional, Default Value: <c>Drawing.Color()</c>
    ///A Red-Green-Blue color value. If omitted, the color Black is assigned.</param>
    ///<param name="visible">(bool) Optional, Default Value: <c>true</c>
    ///Layer's visibility</param>
    ///<param name="locked">(bool) Optional, Default Value: <c>false</c>
    ///Layer's locked state</param>
    ///<param name="parent">(string) Optional, Default Value: <c>null:string</c>
    ///Name of the new layer's parent layer. If omitted, the new
    ///  layer will not have a parent layer.</param>
    ///<returns>(string) The full name of the new layer .</returns>
    static member AddLayer([<OPT;DEF(null:string)>]name:string, [<OPT;DEF(Drawing.Color())>]color:Drawing.Color, [<OPT;DEF(true)>]visible:bool, [<OPT;DEF(false)>]locked:bool, [<OPT;DEF(null:string)>]parent:string) : string =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddLayer(name=None, color=None, visible=True, locked=False, parent=None):
        '''Add a new layer to the document
        Parameters:
          name (str, optional): The name of the new layer. If omitted, Rhino automatically
              generates the layer name.
          color (color): A Red-Green-Blue color value. If omitted, the color Black is assigned.
          visible (bool optional): layer's visibility
          locked (bool, optional): layer's locked state
          parent (str, optional): name of the new layer's parent layer. If omitted, the new
              layer will not have a parent layer.
        Returns:
          str: The full name of the new layer if successful.
        '''
        names = ['']
        if name:
          if not isinstance(name, str): name = str(name)
          names = [n for n in name.split("::") if name]
          
        last_parent_index = -1
        last_parent = None
        for idx, name in enumerate(names):
          layer = Rhino.DocObjects.Layer.GetDefaultLayerProperties()
          if idx is 0:
            if parent:
              last_parent = __getlayer(parent, True)
          else:
            if last_parent_index <> -1:
              last_parent = scriptcontext.doc.Layers[last_parent_index]
          if last_parent:
            layer.ParentLayerId = last_parent.Id
          if name:
            layer.Name = name
            
          color = rhutil.coercecolor(color)
          if color: layer.Color = color
          layer.IsVisible = visible
          layer.IsLocked = locked
        
          last_parent_index = scriptcontext.doc.Layers.Add(layer)
          if last_parent_index == -1:
            full_path = layer.Name
            if last_parent:
                full_path = last_parent.FullPath + "::" + full_path
            last_parent_index = scriptcontext.doc.Layers.FindByFullPath(full_path, UnsetIntIndex)
        return scriptcontext.doc.Layers[last_parent_index].FullPath
    *)


    [<EXT>]
    ///<summary>Returns the current layer</summary>
    ///<returns>(string) The full name of the current layer</returns>
    static member CurrentLayer() : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def CurrentLayer(layer=None):
        '''Returns or changes the current layer
        Parameters:
          layer (guid): the name or Guid of an existing layer to make current
        Returns:
          str: If a layer name is not specified, the full name of the current layer
          str: If a layer name is specified, the full name of the previous current layer
        '''
        rc = scriptcontext.doc.Layers.CurrentLayer.FullPath
        if layer:
            layer = __getlayer(layer, True)
            scriptcontext.doc.Layers.SetCurrentLayerIndex(layer.LayerIndex, True)
        return rc
    *)

    ///<summary>Changes the current layer</summary>
    ///<param name="layer">(Guid)The name or Guid of an existing layer to make current</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentLayer(layer:Guid) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def CurrentLayer(layer=None):
        '''Returns or changes the current layer
        Parameters:
          layer (guid): the name or Guid of an existing layer to make current
        Returns:
          str: If a layer name is not specified, the full name of the current layer
          str: If a layer name is specified, the full name of the previous current layer
        '''
        rc = scriptcontext.doc.Layers.CurrentLayer.FullPath
        if layer:
            layer = __getlayer(layer, True)
            scriptcontext.doc.Layers.SetCurrentLayerIndex(layer.LayerIndex, True)
        return rc
    *)


    [<EXT>]
    ///<summary>Removes an existing layer from the document. The layer to be removed
    ///  cannot be the current layer. Unlike the PurgeLayer method, the layer must
    ///  be empty, or contain no objects, before it can be removed. Any layers that
    ///  are children of the specified layer will also be removed if they are also
    ///  empty.</summary>
    ///<param name="layer">(string) The name or id of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteLayer(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def DeleteLayer(layer):
        '''Removes an existing layer from the document. The layer to be removed
        cannot be the current layer. Unlike the PurgeLayer method, the layer must
        be empty, or contain no objects, before it can be removed. Any layers that
        are children of the specified layer will also be removed if they are also
        empty.
        Parameters:
          layer (str|guid): the name or id of an existing empty layer
        Returns:
          bool: True or False indicating success or failure
        '''
        layer = __getlayer(layer, True)
        return scriptcontext.doc.Layers.Delete( layer.LayerIndex, True)
    *)


    [<EXT>]
    ///<summary>Expands a layer. Expanded layers can be viewed in Rhino's layer dialog</summary>
    ///<param name="layer">(string) Name of the layer to expand</param>
    ///<param name="expand">(bool) True to expand, False to collapse</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ExpandLayer(layer:string, expand:bool) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExpandLayer( layer, expand ):
        '''Expands a layer. Expanded layers can be viewed in Rhino's layer dialog
        Parameters:
          layer (str): name of the layer to expand
          expand (bool): True to expand, False to collapse
        Returns:
          bool: True or False indicating success or failure
        '''
        layer = __getlayer(layer, True)
        if layer.IsExpanded==expand: return False
        layer.IsExpanded = expand
        return True
    *)


    [<EXT>]
    ///<summary>Verifies the existance of a layer in the document</summary>
    ///<param name="layer">(string) The name or id of a layer to search for</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayer(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayer(layer):
        '''Verifies the existance of a layer in the document
        Parameters:
          layer (str|guid): the name or id of a layer to search for
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, False)
        return layer is not None
    *)


    [<EXT>]
    ///<summary>Verifies that the objects on a layer can be changed (normal)</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerChangeable(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerChangeable(layer):
        '''Verifies that the objects on a layer can be changed (normal)
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        rc = layer.IsVisible and not layer.IsLocked
        return rc
    *)


    [<EXT>]
    ///<summary>Verifies that a layer is a child of another layer</summary>
    ///<param name="layer">(string) The name or id of the layer to test against</param>
    ///<param name="test">(string) The name or id to the layer to test</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerChildOf(layer:string, test:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerChildOf(layer, test):
        '''Verifies that a layer is a child of another layer
        Parameters:
          layer (str|guid): the name or id of the layer to test against
          test (str|guid): the name or id to the layer to test
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        test = __getlayer(test, True)
        return test.IsChildOf(layer)
    *)


    [<EXT>]
    ///<summary>Verifies that a layer is the current layer</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerCurrent(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerCurrent(layer):
        '''Verifies that a layer is the current layer
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        return layer.LayerIndex == scriptcontext.doc.Layers.CurrentLayerIndex
    *)


    [<EXT>]
    ///<summary>Verifies that an existing layer is empty, or contains no objects</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerEmpty(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerEmpty(layer):
        '''Verifies that an existing layer is empty, or contains no objects
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        rhobjs = scriptcontext.doc.Objects.FindByLayer(layer)
        if not rhobjs: return True
        return False
    *)


    [<EXT>]
    ///<summary>Verifies that a layer is expanded. Expanded layers can be viewed in
    ///  Rhino's layer dialog</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerExpanded(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerExpanded(layer):
        '''Verifies that a layer is expanded. Expanded layers can be viewed in
        Rhino's layer dialog
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        return layer.IsExpanded
    *)


    [<EXT>]
    ///<summary>Verifies that a layer is locked.</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerLocked(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerLocked(layer):
        '''Verifies that a layer is locked.
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        return layer.IsLocked
    *)


    [<EXT>]
    ///<summary>Verifies that a layer is on.</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerOn(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerOn(layer):
        '''Verifies that a layer is on.
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        return layer.IsVisible
    *)


    [<EXT>]
    ///<summary>Verifies that an existing layer is selectable (normal and reference)</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerSelectable(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerSelectable(layer):
        '''Verifies that an existing layer is selectable (normal and reference)
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        return layer.IsVisible and not layer.IsLocked
    *)


    [<EXT>]
    ///<summary>Verifies that a layer is a parent of another layer</summary>
    ///<param name="layer">(string) The name or id of the layer to test against</param>
    ///<param name="test">(string) The name or id to the layer to test</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerParentOf(layer:string, test:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerParentOf(layer, test):
        '''Verifies that a layer is a parent of another layer
        Parameters:
          layer (str|guid): the name or id of the layer to test against
          test (str|guid): the name or id to the layer to test
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        test = __getlayer(test, True)
        return test.IsParentOf(layer)
    *)


    [<EXT>]
    ///<summary>Verifies that a layer is from a reference file.</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerReference(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerReference(layer):
        '''Verifies that a layer is from a reference file.
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        return layer.IsReference
    *)


    [<EXT>]
    ///<summary>Verifies that a layer is visible (normal, locked, and reference)</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerVisible(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLayerVisible(layer):
        '''Verifies that a layer is visible (normal, locked, and reference)
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        '''
        layer = __getlayer(layer, True)
        return layer.IsVisible
    *)


    [<EXT>]
    ///<summary>Returns the number of immediate child layers of a layer</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(int) the number of immediate child layers</returns>
    static member LayerChildCount(layer:string) : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerChildCount(layer):
        '''Returns the number of immediate child layers of a layer
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          number: the number of immediate child layers if successful
        '''
        layer = __getlayer(layer, True)
        children = layer.GetChildren()
        if children: return len(children)
        return 0
    *)


    [<EXT>]
    ///<summary>Returns the immediate child layers of a layer</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(string seq) List of children layer names</returns>
    static member LayerChildren(layer:string) : string seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerChildren(layer):
        '''Returns the immediate child layers of a layer
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          list(str, ...): List of children layer names
        '''
        layer = __getlayer(layer, True)
        children = layer.GetChildren()
        if children: return [child.FullPath for child in children]
        return [] #empty list
    *)


    [<EXT>]
    ///<summary>Returns the color of a layer.</summary>
    ///<param name="layer">(string) Name or id of an existing layer</param>
    ///<returns>(Drawing.Color) The current color value on success</returns>
    static member LayerColor(layer:string) : Drawing.Color = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerColor(layer, color=None):
        '''Returns or changes the color of a layer.
        Parameters:
          layer (str|guid): name or id of an existing layer
          color (color): the new color value. If omitted, the current layer color is returned.
        Returns:
          color: If a color value is not specified, the current color value on success
          color: If a color value is specified, the previous color value on success
        '''
        layer = __getlayer(layer, True)
        rc = layer.Color
        if color:
            color = rhutil.coercecolor(color)
            if color is not None:
              layer.Color = color
              scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the color of a layer.</summary>
    ///<param name="layer">(string) Name or id of an existing layer</param>
    ///<param name="color">(Drawing.Color)The new color value. If omitted, the current layer color is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerColor(layer:string, color:Drawing.Color) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerColor(layer, color=None):
        '''Returns or changes the color of a layer.
        Parameters:
          layer (str|guid): name or id of an existing layer
          color (color): the new color value. If omitted, the current layer color is returned.
        Returns:
          color: If a color value is not specified, the current color value on success
          color: If a color value is specified, the previous color value on success
        '''
        layer = __getlayer(layer, True)
        rc = layer.Color
        if color:
            color = rhutil.coercecolor(color)
            if color is not None:
              layer.Color = color
              scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the number of layers in the document</summary>
    ///<returns>(int) the number of layers in the document</returns>
    static member LayerCount() : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerCount():
        '''Returns the number of layers in the document
        Returns:
          number: the number of layers in the document
        '''
        return scriptcontext.doc.Layers.ActiveCount
    *)


    [<EXT>]
    ///<summary>Return identifiers of all layers in the document</summary>
    ///<returns>(Guid seq) the identifiers of all layers in the document</returns>
    static member LayerIds() : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerIds():
        '''Return identifiers of all layers in the document
        Returns:
          list(guid, ...): the identifiers of all layers in the document
        '''
        return [layer.Id for layer in scriptcontext.doc.Layers if not layer.IsDeleted]
    *)


    [<EXT>]
    ///<summary>Returns the linetype of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) Name of the current linetype</returns>
    static member LayerLinetype(layer:string) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerLinetype(layer, linetype=None):
        '''Returns or changes the linetype of a layer
        Parameters:
          layer (str): name of an existing layer
          linetype (str, optional): name of a linetype
        Returns:
          str: If linetype is not specified, name of the current linetype
          str: If linetype is specified, name of the previous linetype
        '''
        layer = __getlayer(layer, True)
        index = layer.LinetypeIndex
        rc = scriptcontext.doc.Linetypes[index].Name
        if linetype:
            if not isinstance(linetype, str): linetype = str(linetype)
            if linetype == scriptcontext.doc.Linetypes.ContinuousLinetypeName:
              index = -1
            else:
              lt = scriptcontext.doc.Linetypes.FindName(linetype)
              if lt == None: return scriptcontext.errorhandler()
              index = lt.LinetypeIndex
            layer.LinetypeIndex = index
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the linetype of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="linetyp">(string)Name of a linetyp</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerLinetype(layer:string, linetyp:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerLinetype(layer, linetype=None):
        '''Returns or changes the linetype of a layer
        Parameters:
          layer (str): name of an existing layer
          linetype (str, optional): name of a linetype
        Returns:
          str: If linetype is not specified, name of the current linetype
          str: If linetype is specified, name of the previous linetype
        '''
        layer = __getlayer(layer, True)
        index = layer.LinetypeIndex
        rc = scriptcontext.doc.Linetypes[index].Name
        if linetype:
            if not isinstance(linetype, str): linetype = str(linetype)
            if linetype == scriptcontext.doc.Linetypes.ContinuousLinetypeName:
              index = -1
            else:
              lt = scriptcontext.doc.Linetypes.FindName(linetype)
              if lt == None: return scriptcontext.errorhandler()
              index = lt.LinetypeIndex
            layer.LinetypeIndex = index
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the locked mode of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(bool) The current layer locked mode</returns>
    static member LayerLocked(layer:string) : bool = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerLocked(layer, locked=None):
        '''Returns or changes the locked mode of a layer
        Parameters:
          layer (str): name of an existing layer
          locked (bool, optional): new layer locked mode
        Returns:
          bool: If locked is not specified, the current layer locked mode
          bool: If locked is specified, the previous layer locked mode
        '''
        layer = __getlayer(layer, True)
        rc = layer.IsLocked
        if locked!=None and locked!=rc:
            layer.IsLocked = locked
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the locked mode of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="locked">(bool)New layer locked mode</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerLocked(layer:string, locked:bool) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerLocked(layer, locked=None):
        '''Returns or changes the locked mode of a layer
        Parameters:
          layer (str): name of an existing layer
          locked (bool, optional): new layer locked mode
        Returns:
          bool: If locked is not specified, the current layer locked mode
          bool: If locked is specified, the previous layer locked mode
        '''
        layer = __getlayer(layer, True)
        rc = layer.IsLocked
        if locked!=None and locked!=rc:
            layer.IsLocked = locked
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) a zero-based material index</returns>
    static member LayerMaterialIndex(layer:string) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerMaterialIndex(layer,index=None):
        '''Returns or changes the material index of a layer. A material index of -1
        indicates that no material has been assigned to the layer. Thus, the layer
        will use Rhino's default layer material
        Parameters:
          layer (str):  name of existing layer
          index (number, optional): the new material index
        Returns:
          number: a zero-based material index if successful
          number: a zero-based material index if successful
        '''
        layer = __getlayer(layer, True)
        rc = layer.RenderMaterialIndex
        if index is not None and index>=-1:
            layer.RenderMaterialIndex = index
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="index">(int)The new material index</param>
    ///<returns>(int) a zero-based material index</returns>
    static member LayerMaterialIndex(layer:string, index:int) : int = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerMaterialIndex(layer,index=None):
        '''Returns or changes the material index of a layer. A material index of -1
        indicates that no material has been assigned to the layer. Thus, the layer
        will use Rhino's default layer material
        Parameters:
          layer (str):  name of existing layer
          index (number, optional): the new material index
        Returns:
          number: a zero-based material index if successful
          number: a zero-based material index if successful
        '''
        layer = __getlayer(layer, True)
        rc = layer.RenderMaterialIndex
        if index is not None and index>=-1:
            layer.RenderMaterialIndex = index
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the identifier of a layer given the layer's name.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Guid) The layer's identifier .</returns>
    static member LayerId(layer:string) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerId(layer):
        '''Returns the identifier of a layer given the layer's name.
        Parameters:
          layer (str): name of existing layer
        Returns:
          guid : The layer's identifier if successful.
          None: If not successful, or on error.
        '''
        idx = scriptcontext.doc.Layers.FindByFullPath(layer, UnsetIntIndex)
        return str(scriptcontext.doc.Layers[idx].Id) if idx != UnsetIntIndex else None
    *)


    [<EXT>]
    ///<summary>Return the name of a layer given it's identifier</summary>
    ///<param name="layerId">(Guid) Layer identifier</param>
    ///<param name="fullpath">(bool) Optional, Default Value: <c>true</c>
    ///Return the full path name `True` or short name `False`</param>
    ///<returns>(string) the layer's name</returns>
    static member LayerName(layerId:Guid, [<OPT;DEF(true)>]fullpath:bool) : string =
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerName(layer_id, fullpath=True):
        '''Return the name of a layer given it's identifier
        Parameters:
          layer_id (guid): layer identifier
          fullpath (bool, optional): return the full path name `True` or short name `False`
        Returns:
          str: the layer's name if successful
          None: if not successful
        '''
        layer = __getlayer(layer_id, True)
        if fullpath: return layer.FullPath
        return layer.Name
    *)


    [<EXT>]
    ///<summary>Returns the names of all layers in the document.</summary>
    ///<returns>(string seq) list of layer names</returns>
    static member LayerNames() : string seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerNames(sort=False):
        '''Returns the names of all layers in the document.
        Parameters:
          sort (bool, optional): return a sorted list of the layer names
        Returns:
          list(str, ...): list of layer names
        '''
        rc = []
        for layer in scriptcontext.doc.Layers:
            if not layer.IsDeleted: rc.append(layer.FullPath)
        if sort: rc.sort()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the current display order index of a layer as displayed in Rhino's
    ///  layer dialog box. A display order index of -1 indicates that the current
    ///  layer dialog filter does not allow the layer to appear in the layer list</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) 0 based index of layer</returns>
    static member LayerOrder(layer:string) : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerOrder(layer):
        '''Returns the current display order index of a layer as displayed in Rhino's
        layer dialog box. A display order index of -1 indicates that the current
        layer dialog filter does not allow the layer to appear in the layer list
        Parameters:
          layer (str): name of existing layer
        Returns:
          number: 0 based index of layer
        '''
        layer = __getlayer(layer, True)
        return layer.SortIndex
    *)


    [<EXT>]
    ///<summary>Returns the print color of a layer. Layer print colors are
    /// represented as RGB colors.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Drawing.Color) The current layer print color</returns>
    static member LayerPrintColor(layer:string) : Drawing.Color = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerPrintColor(layer, color=None):
        '''Returns or changes the print color of a layer. Layer print colors are
        represented as RGB colors.
        Parameters:
          layer (str): name of existing layer
          color (color): new print color
        Returns:
          color: if color is not specified, the current layer print color
          color: if color is specified, the previous layer print color
          None: on error
        '''
        layer = __getlayer(layer, True)
        rc = layer.PlotColor
        if color:
            color = rhutil.coercecolor(color)
            layer.PlotColor = color
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the print color of a layer. Layer print colors are
    /// represented as RGB colors.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="color">(Drawing.Color)New print color</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerPrintColor(layer:string, color:Drawing.Color) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerPrintColor(layer, color=None):
        '''Returns or changes the print color of a layer. Layer print colors are
        represented as RGB colors.
        Parameters:
          layer (str): name of existing layer
          color (color): new print color
        Returns:
          color: if color is not specified, the current layer print color
          color: if color is specified, the previous layer print color
          None: on error
        '''
        layer = __getlayer(layer, True)
        rc = layer.PlotColor
        if color:
            color = rhutil.coercecolor(color)
            layer.PlotColor = color
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(float) The current layer print width</returns>
    static member LayerPrintWidth(layer:string) : float = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerPrintWidth(layer, width=None):
        '''Returns or changes the print width of a layer. Print width is specified
        in millimeters. A print width of 0.0 denotes the "default" print width.
        Parameters:
          layer (str): name of existing layer
          width (number, optional): new print width
        Returns:
          number: if width is not specified, the current layer print width
          number: if width is specified, the previous layer print width
        '''
        layer = __getlayer(layer, True)
        rc = layer.PlotWeight
        if width is not None and width!=rc:
            layer.PlotWeight = width
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="width">(float)New print width</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerPrintWidth(layer:string, width:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerPrintWidth(layer, width=None):
        '''Returns or changes the print width of a layer. Print width is specified
        in millimeters. A print width of 0.0 denotes the "default" print width.
        Parameters:
          layer (str): name of existing layer
          width (number, optional): new print width
        Returns:
          number: if width is not specified, the current layer print width
          number: if width is specified, the previous layer print width
        '''
        layer = __getlayer(layer, True)
        rc = layer.PlotWeight
        if width is not None and width!=rc:
            layer.PlotWeight = width
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the visible property of a layer.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(bool) The current layer visibility</returns>
    static member LayerVisible(layer:string) : bool = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerVisible(layer, visible=None, forcevisible_or_donotpersist=False):
        '''Returns or changes the visible property of a layer.
        Parameters:
          layer (str): name of existing layer
          visible (bool, optional): new visible state
          forcevisible_or_donotpersist (bool, optional): if visible is True then turn parent layers on if True.  If visible is False then do not persist if True
        Returns:
          bool: if visible is not specified, the current layer visibility
          bool: if visible is specified, the previous layer visibility
        '''
        layer = __getlayer(layer, True)
        rc = layer.IsVisible
        if visible is not None:
            layer.IsVisible = visible
            if visible and forcevisible_or_donotpersist:
                scriptcontext.doc.Layers.ForceLayerVisible(layer.Id)
            if not visible and not forcevisible_or_donotpersist:
              if layer.ParentLayerId != System.Guid.Empty:
                layer.SetPersistentVisibility(visible)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the visible property of a layer.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="visible">(bool)New visible state</param>
    ///<param name="forcevisibleOrDonotpersist">(bool)If visible is True then turn parent layers on if True.  If visible is False then do not persist if True</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerVisible(layer:string, visible:bool, [<OPT;DEF(false)>]forcevisibleOrDonotpersist:bool) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LayerVisible(layer, visible=None, forcevisible_or_donotpersist=False):
        '''Returns or changes the visible property of a layer.
        Parameters:
          layer (str): name of existing layer
          visible (bool, optional): new visible state
          forcevisible_or_donotpersist (bool, optional): if visible is True then turn parent layers on if True.  If visible is False then do not persist if True
        Returns:
          bool: if visible is not specified, the current layer visibility
          bool: if visible is specified, the previous layer visibility
        '''
        layer = __getlayer(layer, True)
        rc = layer.IsVisible
        if visible is not None:
            layer.IsVisible = visible
            if visible and forcevisible_or_donotpersist:
                scriptcontext.doc.Layers.ForceLayerVisible(layer.Id)
            if not visible and not forcevisible_or_donotpersist:
              if layer.ParentLayerId != System.Guid.Empty:
                layer.SetPersistentVisibility(visible)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Return the parent layer of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) The name of the current parent layer</returns>
    static member ParentLayer(layer:string) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def ParentLayer(layer, parent=None):
        '''Return or modify the parent layer of a layer
        Parameters:
          layer (str): name of an existing layer
          parent (str, optional):  name of new parent layer. To remove the parent layer,
            thus making a root-level layer, specify an empty string
        Returns:
          str: If parent is not specified, the name of the current parent layer
          str: If parent is specified, the name of the previous parent layer
          None: if the layer does not have a parent
        '''
        layer = __getlayer(layer, True)
        parent_id = layer.ParentLayerId
        oldparent = None
        if parent_id!=System.Guid.Empty:
            oldparentlayer = scriptcontext.doc.Layers.Find(parent_id, -1)
            if oldparentlayer is not None:
                oldparentlayer = scriptcontext.doc.Layers[oldparentlayer]
                oldparent = oldparentlayer.FullPath
        if parent is None: return oldparent
        if parent=="":
            layer.ParentLayerId = System.Guid.Empty
        else:
            parent = __getlayer(parent, True)
            layer.ParentLayerId = parent.Id
        return oldparent
    *)

    ///<summary>Modify the parent layer of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="parent">(string)Name of new parent layer. To remove the parent layer,
    ///  thus making a root-level layer, specify an empty string</param>
    ///<returns>(unit) void, nothing</returns>
    static member ParentLayer(layer:string, parent:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def ParentLayer(layer, parent=None):
        '''Return or modify the parent layer of a layer
        Parameters:
          layer (str): name of an existing layer
          parent (str, optional):  name of new parent layer. To remove the parent layer,
            thus making a root-level layer, specify an empty string
        Returns:
          str: If parent is not specified, the name of the current parent layer
          str: If parent is specified, the name of the previous parent layer
          None: if the layer does not have a parent
        '''
        layer = __getlayer(layer, True)
        parent_id = layer.ParentLayerId
        oldparent = None
        if parent_id!=System.Guid.Empty:
            oldparentlayer = scriptcontext.doc.Layers.Find(parent_id, -1)
            if oldparentlayer is not None:
                oldparentlayer = scriptcontext.doc.Layers[oldparentlayer]
                oldparent = oldparentlayer.FullPath
        if parent is None: return oldparent
        if parent=="":
            layer.ParentLayerId = System.Guid.Empty
        else:
            parent = __getlayer(parent, True)
            layer.ParentLayerId = parent.Id
        return oldparent
    *)


    [<EXT>]
    ///<summary>Removes an existing layer from the document. The layer will be removed
    ///  even if it contains geometry objects. The layer to be removed cannot be the
    ///  current layer
    ///  empty.</summary>
    ///<param name="layer">(string) The name or id of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member PurgeLayer(layer:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def PurgeLayer(layer):
        '''Removes an existing layer from the document. The layer will be removed
        even if it contains geometry objects. The layer to be removed cannot be the
        current layer
        empty.
        Parameters:
          layer (str|guid): the name or id of an existing empty layer
        Returns:
          bool: True or False indicating success or failure
        '''
        layer = __getlayer(layer, True)
        rc = scriptcontext.doc.Layers.Purge( layer.LayerIndex, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Renames an existing layer</summary>
    ///<param name="oldname">(string) Original layer name</param>
    ///<param name="newname">(string) New layer name</param>
    ///<returns>(string) The new layer name  otherwise None</returns>
    static member RenameLayer(oldname:string, newname:string) : string =
        failNotImpl () // genreation temp disabled !!
    (*
    def RenameLayer(oldname, newname):
        '''Renames an existing layer
        Parameters:
          oldname (str): original layer name
          newname (str): new layer name
        Returns: 
          str: The new layer name if successful otherwise None
        '''
        if oldname and newname:
            layer = __getlayer(oldname, True)
            layer.Name = newname
            return newname
    *)


