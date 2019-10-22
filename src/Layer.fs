namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsLayer =
 type RhinoScriptSyntax with
    
    [<EXT>]
    
    static member Getlayer() : obj =  failNotImpl () // genreation temp disabled !! //TODO remove !!

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
    static member AddLayer( [<OPT;DEF(null:string)>]name:string, 
                            [<OPT;DEF(Drawing.Color())>]color:Drawing.Color, 
                            [<OPT;DEF(true)>]visible:bool, 
                            [<OPT;DEF(false)>]locked:bool, 
                            [<OPT;DEF(null:string)>]parent:string) : string =
        let names =  
            if isNull name then [| "" |]
            else name.Split([| "::"|],StringSplitOptions.RemoveEmptyEntries)       
        let mutable lastparentindex = -1
        let mutable lastparent:DocObjects.Layer = null
        for idx, name in Seq.indexed(names) do
          let layer = Rhino.DocObjects.Layer.GetDefaultLayerProperties()
          if idx = 0 then
            if notNull parent then
              lastparent <- RhinoScriptSyntax.CoerceLayer(parent)
          else 
            if lastparentindex <> -1 then
              lastparent <- Doc.Layers.[lastparentindex]
          if notNull lastparent then
            layer.ParentLayerId <- lastparent.Id
          if notNull name then
            layer.Name <- name
          //color = RhinoScriptSyntax.Coercecolor(color)
          if not color.IsEmpty then layer.Color <- color
          layer.IsVisible <- visible
          layer.IsLocked <- locked
          lastparentindex <- Doc.Layers.Add(layer)
          if lastparentindex = -1 then
            let mutable fullpath = layer.Name
            if notNull lastparent then
                fullpath <- lastparent.FullPath + "::" + fullpath
            lastparentindex <- Doc.Layers.FindByFullPath(fullpath, RhinoMath.UnsetIntIndex)
        Doc.Layers.[lastparentindex].FullPath
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
        Doc.Layers.CurrentLayer.FullPath
        
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
    ///<param name="layer">(string)The name or Guid of an existing layer to make current</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentLayer(layer:string) : unit = //SET
        let rc = Doc.Layers.CurrentLayer.FullPath 
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then failwithf "CoerceLayer: could not Coerce Layer from name'%A'" layer        
        if not<|  Doc.Layers.SetCurrentLayerIndex(i, true) then failwithf "Set CurrentLayer to %A failed" layer
        
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
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then failwithf "CoerceLayer: could not Coerce Layer from name'%A'" layer  
        Doc.Layers.Delete(i, true)
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
    ///<returns>(unit) void, nothing</returns>
    static member ExpandLayer(layer:string, expand:bool) : unit =
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then failwithf "CoerceLayer: could not Coerce Layer from name'%A'" layer  
        let layer = Doc.Layers.[i]
        if layer.IsExpanded <> expand then 
            layer.IsExpanded <- expand
            
        
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
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then false else true
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


