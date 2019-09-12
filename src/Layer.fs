namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsLayer =
  type RhinoScriptSyntax with
    
    
    static member internal Getlayer() : obj =
    (*
    def __getlayer(name_or_id, raise_if_missing):
        if not name_or_id: raise TypeError("Parameter must be a string or Guid")
        id = rhutil.coerceguid(name_or_id)
        if id:
            layer = scriptcontext.doc.Layers.FindId(id)
        else:
            name = name_or_id
            layer_index = scriptcontext.doc.Layers.FindByFullPath(name, UnsetIntIndex)
            if layer_index != UnsetIntIndex: return scriptcontext.doc.Layers[layer_index]
            layer = scriptcontext.doc.Layers.FindName(name)
        if layer: return layer
        if raise_if_missing: raise ValueError("%s does not exist in LayerTable" % name_or_id)
    *)


    ///<summary>Add a new layer to the document</summary>
    ///<param name="name">(string) Optional, Default Value: <c>null</c>
    ///The name of the new layer. If omitted, Rhino automatically
    ///  generates the layer name.</param>
    ///<param name="color">(Drawing.Color) Optional, Default Value: <c>null</c>
    ///A Red-Green-Blue color value. If omitted, the color Black is assigned.</param>
    ///<param name="visible">(bool) Optional, Default Value: <c>true</c>
    ///Layer's visibility</param>
    ///<param name="locked">(bool) Optional, Default Value: <c>false</c>
    ///Layer's locked state</param>
    ///<param name="parent">(string) Optional, Default Value: <c>null</c>
    ///Name of the new layer's parent layer. If omitted, the new
    ///  layer will not have a parent layer.</param>
    ///<returns>(string) The full name of the new layer .</returns>
    static member AddLayer([<OPT;DEF(null)>]name:string, [<OPT;DEF(null)>]color:Drawing.Color, [<OPT;DEF(true)>]visible:bool, [<OPT;DEF(false)>]locked:bool, [<OPT;DEF(null)>]parent:string) : string =
        let names = .[""]
        if name then
          if not <| isinstance(name, str) then name <- string(name)
          names <- [| for n in name.split("::") if name -> n |]
        let last_parent_index = -1
        let last_parent = null
        for idx, name in enumerate(names) do
          let layer = Rhino.DocObjects.Layer.GetDefaultLayerProperties()
          if idx = 0 then
            if parent then
              last_parent <- __getlayer(parent, true)
          else
            if last_parent_index <> -1 then
              last_parent <- Doc.Layers.[last_parent_index]
          if last_parent then
            let layer.ParentLayerId = last_parent.Id
          if name then
            let layer.Name = name
          let color = Coerce.coercecolor(color)
          if color then layer.Color <- color
          let layer.IsVisible = visible
          let layer.IsLocked = locked
          last_parent_index <- Doc.Layers.Add(layer)
          if last_parent_index = -1 then
            let full_path = layer.Name
            if last_parent then
                full_path <- last_parent.FullPath + "::" + full_path
            last_parent_index <- Doc.Layers.FindByFullPath(full_path, RhinoMath.UnsetIntIndex)
        Doc.Layers.[last_parent_index].FullPath
    (*
    def AddLayer(name=None, color=None, visible=True, locked=False, parent=None):
        """Add a new layer to the document
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
        Example:
          import rhinoscriptsyntax as rs
          from System.Drawing import Color
          print "New layer:", rs.AddLayer()
          print "New layer:", rs.AddLayer("MyLayer1")
          print "New layer:", rs.AddLayer("MyLayer2", Color.DarkSeaGreen)
          print "New layer:", rs.AddLayer("MyLayer3", Color.Cornsilk)
          print "New layer:", rs.AddLayer("MyLayer4",parent="MyLayer3")
        See Also:
          CurrentLayer
          DeleteLayer
          RenameLayer
        """
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


    ///<summary>Returns the current layer</summary>
    ///<returns>(string) The full name of the current layer</returns>
    static member CurrentLayer() : string =
        let rc = Doc.Layers.CurrentLayer.FullPath
        if layer then
            let layer = __getlayer(layer, true)
            Doc.Layers.SetCurrentLayerIndex(layer.LayerIndex, true)
        rc
    (*
    def CurrentLayer(layer=None):
        """Returns or changes the current layer
        Parameters:
          layer (guid): the name or Guid of an existing layer to make current
        Returns:
          str: If a layer name is not specified, the full name of the current layer
          str: If a layer name is specified, the full name of the previous current layer
        Example:
          import rhinoscriptsyntax as rs
          rs.AddLayer("MyLayer")
          rs.CurrentLayer("MyLayer")
        See Also:
          AddLayer
          DeleteLayer
          RenameLayer
        """
        rc = scriptcontext.doc.Layers.CurrentLayer.FullPath
        if layer:
            layer = __getlayer(layer, True)
            scriptcontext.doc.Layers.SetCurrentLayerIndex(layer.LayerIndex, True)
        return rc
    *)

    ///<summary>Changes the current layer</summary>
    ///<param name="layer">(Guid)The name or Guid of an existing layer to make current</param>
    ///<returns>(unit) unit</returns>
    static member CurrentLayer(layer:Guid) : unit =
        let rc = Doc.Layers.CurrentLayer.FullPath
        if layer then
            let layer = __getlayer(layer, true)
            Doc.Layers.SetCurrentLayerIndex(layer.LayerIndex, true)
        rc
    (*
    def CurrentLayer(layer=None):
        """Returns or changes the current layer
        Parameters:
          layer (guid): the name or Guid of an existing layer to make current
        Returns:
          str: If a layer name is not specified, the full name of the current layer
          str: If a layer name is specified, the full name of the previous current layer
        Example:
          import rhinoscriptsyntax as rs
          rs.AddLayer("MyLayer")
          rs.CurrentLayer("MyLayer")
        See Also:
          AddLayer
          DeleteLayer
          RenameLayer
        """
        rc = scriptcontext.doc.Layers.CurrentLayer.FullPath
        if layer:
            layer = __getlayer(layer, True)
            scriptcontext.doc.Layers.SetCurrentLayerIndex(layer.LayerIndex, True)
        return rc
    *)


    ///<summary>Removes an existing layer from the document. The layer to be removed
    ///  cannot be the current layer. Unlike the PurgeLayer method, the layer must
    ///  be empty, or contain no objects, before it can be removed. Any layers that
    ///  are children of the specified layer will also be removed if they are also
    ///  empty.</summary>
    ///<param name="layer">(string) The name or id of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteLayer(layer:string) : bool =
        let layer = __getlayer(layer, true)
        Doc.Layers.Delete( layer.LayerIndex, true)
    (*
    def DeleteLayer(layer):
        """Removes an existing layer from the document. The layer to be removed
        cannot be the current layer. Unlike the PurgeLayer method, the layer must
        be empty, or contain no objects, before it can be removed. Any layers that
        are children of the specified layer will also be removed if they are also
        empty.
        Parameters:
          layer (str|guid): the name or id of an existing empty layer
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer to remove")
          if layer: rs.DeleteLayer(layer)
        See Also:
          AddLayer
          CurrentLayer
          PurgeLayer
          RenameLayer
        """
        layer = __getlayer(layer, True)
        return scriptcontext.doc.Layers.Delete( layer.LayerIndex, True)
    *)


    ///<summary>Expands a layer. Expanded layers can be viewed in Rhino's layer dialog</summary>
    ///<param name="layer">(string) Name of the layer to expand</param>
    ///<param name="expand">(bool) True to expand, False to collapse</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ExpandLayer(layer:string, expand:bool) : bool =
        let layer = __getlayer(layer, true)
        if layer.IsExpanded=expand then false
        let layer.IsExpanded = expand
        true
    (*
    def ExpandLayer( layer, expand ):
        """Expands a layer. Expanded layers can be viewed in Rhino's layer dialog
        Parameters:
          layer (str): name of the layer to expand
          expand (bool): True to expand, False to collapse
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          if rs.IsLayerExpanded("Default"):
              rs.ExpandLayer( "Default", False )
        See Also:
          IsLayerExpanded
        """
        layer = __getlayer(layer, True)
        if layer.IsExpanded==expand: return False
        layer.IsExpanded = expand
        return True
    *)


    ///<summary>Verifies the existance of a layer in the document</summary>
    ///<param name="layer">(string) The name or id of a layer to search for</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayer(layer:string) : bool =
        let layer = __getlayer(layer, false)
        layer <> null
    (*
    def IsLayer(layer):
        """Verifies the existance of a layer in the document
        Parameters:
          layer (str|guid): the name or id of a layer to search for
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer name")
          if rs.IsLayer(layer):
              print "The layer exists."
          else:
              print "The layer does not exist."
        See Also:
          IsLayerChangeable
          IsLayerEmpty
          IsLayerLocked
          IsLayerOn
          IsLayerReference
          IsLayerSelectable
          IsLayerVisible
        """
        layer = __getlayer(layer, False)
        return layer is not None
    *)


    ///<summary>Verifies that the objects on a layer can be changed (normal)</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerChangeable(layer:string) : bool =
        let layer = __getlayer(layer, true)
        let rc = layer.IsVisible && not <| layer.IsLocked
        rc
    (*
    def IsLayerChangeable(layer):
        """Verifies that the objects on a layer can be changed (normal)
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer name")
          if rs.IsLayer(layer):
              if rs.IsLayerChangeable(layer): print "The layer is changeable."
              else: print "The layer is not changeable."
          else:
              print "The layer does not exist."
        See Also:
          IsLayer
          IsLayerEmpty
          IsLayerLocked
          IsLayerOn
          IsLayerReference
          IsLayerSelectable
          IsLayerVisible
        """
        layer = __getlayer(layer, True)
        rc = layer.IsVisible and not layer.IsLocked
        return rc
    *)


    ///<summary>Verifies that a layer is a child of another layer</summary>
    ///<param name="layer">(string) The name or id of the layer to test against</param>
    ///<param name="test">(string) The name or id to the layer to test</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerChildOf(layer:string, test:string) : bool =
        let layer = __getlayer(layer, true)
        let test = __getlayer(test, true)
        test.IsChildOf(layer)
    (*
    def IsLayerChildOf(layer, test):
        """Verifies that a layer is a child of another layer
        Parameters:
          layer (str|guid): the name or id of the layer to test against
          test (str|guid): the name or id to the layer to test
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          rs.AddLayer("MyLayer1")
          rs.AddLayer("MyLayer2", parent="MyLayer1")
          rs.AddLayer("MyLayer3", parent="MyLayer2")
          rs.MessageBox( rs.IsLayerChildOf("MyLayer1", "MyLayer3") )
        See Also:
          IsLayerParentOf
        """
        layer = __getlayer(layer, True)
        test = __getlayer(test, True)
        return test.IsChildOf(layer)
    *)


    ///<summary>Verifies that a layer is the current layer</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerCurrent(layer:string) : bool =
        let layer = __getlayer(layer, true)
        layer.LayerIndex = Doc.Layers.CurrentLayerIndex
    (*
    def IsLayerCurrent(layer):
        """Verifies that a layer is the current layer
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer name")
          if rs.IsLayer(layer):
              if rs.IsLayerCurrent(layer): print "The layer is current."
              else: print "The layer is not current."
          else:
              print "The layer does not exist."
        See Also:
          IsLayer
          IsLayerEmpty
          IsLayerLocked
          IsLayerOn
          IsLayerReference
          IsLayerSelectable
          IsLayerVisible
        """
        layer = __getlayer(layer, True)
        return layer.LayerIndex == scriptcontext.doc.Layers.CurrentLayerIndex
    *)


    ///<summary>Verifies that an existing layer is empty, or contains no objects</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerEmpty(layer:string) : bool =
        let layer = __getlayer(layer, true)
        let rhobjs = Doc.Objects.FindByLayer(layer)
        if not <| rhobjs then true
        false
    (*
    def IsLayerEmpty(layer):
        """Verifies that an existing layer is empty, or contains no objects
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer name")
          if rs.IsLayer(layer):
              if rs.IsLayerEmpty(layer): print "The layer is empty."
              else: print "The layer is not empty."
          else:
              print "The layer does not exist."
        See Also:
          IsLayerChangeable
          IsLayerLocked
          IsLayerOn
          IsLayerReference
          IsLayerSelectable
          IsLayerVisible
        """
        layer = __getlayer(layer, True)
        rhobjs = scriptcontext.doc.Objects.FindByLayer(layer)
        if not rhobjs: return True
        return False
    *)


    ///<summary>Verifies that a layer is expanded. Expanded layers can be viewed in
    ///  Rhino's layer dialog</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerExpanded(layer:string) : bool =
        let layer = __getlayer(layer, true)
        layer.IsExpanded
    (*
    def IsLayerExpanded(layer):
        """Verifies that a layer is expanded. Expanded layers can be viewed in
        Rhino's layer dialog
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          if rs.IsLayerExpanded("Default"):
              rs.ExpandLayer( "Default", False )
        See Also:
          ExpandLayer
        """
        layer = __getlayer(layer, True)
        return layer.IsExpanded
    *)


    ///<summary>Verifies that a layer is locked.</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerLocked(layer:string) : bool =
        let layer = __getlayer(layer, true)
        layer.IsLocked
    (*
    def IsLayerLocked(layer):
        """Verifies that a layer is locked.
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer name")
          if rs.IsLayer(layer):
              if rs.IsLayerLocked(layer): print "The layer is locked."
              else: print "The layer is not locked."
          else:
              print "The layer does not exist."
        See Also:
          IsLayer
          IsLayerChangeable
          IsLayerEmpty
          IsLayerOn
          IsLayerReference
          IsLayerSelectable
          IsLayerVisible
        """
        layer = __getlayer(layer, True)
        return layer.IsLocked
    *)


    ///<summary>Verifies that a layer is on.</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerOn(layer:string) : bool =
        let layer = __getlayer(layer, true)
        layer.IsVisible
    (*
    def IsLayerOn(layer):
        """Verifies that a layer is on.
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer name")
          if rs.IsLayer(layer):
              if rs.IsLayerOn(layer): print "The layer is on."
              else: print "The layer is not on."
          else:
              print "The layer does not exist."
        See Also:
          IsLayer
          IsLayerChangeable
          IsLayerEmpty
          IsLayerLocked
          IsLayerReference
          IsLayerSelectable
          IsLayerVisible
        """
        layer = __getlayer(layer, True)
        return layer.IsVisible
    *)


    ///<summary>Verifies that an existing layer is selectable (normal and reference)</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerSelectable(layer:string) : bool =
        let layer = __getlayer(layer, true)
        layer.IsVisible && not <| layer.IsLocked
    (*
    def IsLayerSelectable(layer):
        """Verifies that an existing layer is selectable (normal and reference)
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer name")
          if rs.IsLayer(layer):
              if rs.IsLayerSelectable(layer): print "The layer is selectable."
              else: print "The layer is not selectable."
          else:
              print "The layer does not exist."
        See Also:
          IsLayer
          IsLayerChangeable
          IsLayerEmpty
          IsLayerLocked
          IsLayerOn
          IsLayerReference
          IsLayerVisible
        """
        layer = __getlayer(layer, True)
        return layer.IsVisible and not layer.IsLocked
    *)


    ///<summary>Verifies that a layer is a parent of another layer</summary>
    ///<param name="layer">(string) The name or id of the layer to test against</param>
    ///<param name="test">(string) The name or id to the layer to test</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerParentOf(layer:string, test:string) : bool =
        let layer = __getlayer(layer, true)
        let test = __getlayer(test, true)
        test.IsParentOf(layer)
    (*
    def IsLayerParentOf(layer, test):
        """Verifies that a layer is a parent of another layer
        Parameters:
          layer (str|guid): the name or id of the layer to test against
          test (str|guid): the name or id to the layer to test
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          rs.AddLayer("MyLayer1")
          rs.AddLayer("MyLayer2", parent="MyLayer1")
          rs.AddLayer("MyLayer3", parent="MyLayer2")
          rs.MessageBox( rs.IsLayerParentOf("MyLayer3", "MyLayer1") )
        See Also:
          IsLayerChildOf
        """
        layer = __getlayer(layer, True)
        test = __getlayer(test, True)
        return test.IsParentOf(layer)
    *)


    ///<summary>Verifies that a layer is from a reference file.</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerReference(layer:string) : bool =
        let layer = __getlayer(layer, true)
        layer.IsReference
    (*
    def IsLayerReference(layer):
        """Verifies that a layer is from a reference file.
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer name")
          if rs.IsLayer(layer):
              if rs.IsLayerReference(layer): print "The layer is a reference layer."
              else: print "The layer is not a reference layer."
          else:
              print "The layer does not exist."
        See Also:
          IsLayer
          IsLayerChangeable
          IsLayerEmpty
          IsLayerLocked
          IsLayerOn
          IsLayerSelectable
          IsLayerVisible
        """
        layer = __getlayer(layer, True)
        return layer.IsReference
    *)


    ///<summary>Verifies that a layer is visible (normal, locked, and reference)</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerVisible(layer:string) : bool =
        let layer = __getlayer(layer, true)
        layer.IsVisible
    (*
    def IsLayerVisible(layer):
        """Verifies that a layer is visible (normal, locked, and reference)
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          bool: True on success otherwise False
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer name")
          if rs.IsLayer(layer):
              if rs.IsLayerVisible(layer): print "The layer is visible"
              else: print "The layer is not visible"
          else:
              print "The layer does not exist."
        See Also:
          IsLayer
          IsLayerChangeable
          IsLayerEmpty
          IsLayerLocked
          IsLayerOn
          IsLayerReference
          IsLayerSelectable
        """
        layer = __getlayer(layer, True)
        return layer.IsVisible
    *)


    ///<summary>Returns the number of immediate child layers of a layer</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(int) the number of immediate child layers</returns>
    static member LayerChildCount(layer:string) : int =
        let layer = __getlayer(layer, true)
        let children = layer.GetChildren()
        if children then Seq.length(children)
        0
    (*
    def LayerChildCount(layer):
        """Returns the number of immediate child layers of a layer
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          number: the number of immediate child layers if successful
        Example:
          import rhinoscriptsyntax as rs
          children = rs.LayerChildCount("Default")
          if children: rs.ExpandLayer("Default", True)
        See Also:
          LayerChildren
        """
        layer = __getlayer(layer, True)
        children = layer.GetChildren()
        if children: return len(children)
        return 0
    *)


    ///<summary>Returns the immediate child layers of a layer</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(string seq) List of children layer names</returns>
    static member LayerChildren(layer:string) : string seq =
        let layer = __getlayer(layer, true)
        let children = layer.GetChildren()
        if children then [| for child in children -> child.FullPath |]
        [] //empty list
    (*
    def LayerChildren(layer):
        """Returns the immediate child layers of a layer
        Parameters:
          layer (str|guid): the name or id of an existing layer
        Returns:
          list(str, ...): List of children layer names
        Example:
          import rhinoscriptsyntax as rs
          children = rs.LayerChildren("Default")
          if children:
              for child in children: print child
        See Also:
          LayerChildCount
          ParentLayer
        """
        layer = __getlayer(layer, True)
        children = layer.GetChildren()
        if children: return [child.FullPath for child in children]
        return [] #empty list
    *)


    ///<summary>Returns the color of a layer.</summary>
    ///<param name="layer">(string) Name or id of an existing layer</param>
    ///<returns>(Drawing.Color) The current color value on success</returns>
    static member LayerColor(layer:string) : Drawing.Color =
        let layer = __getlayer(layer, true)
        let rc = layer.Color
        if color then
            let color = Coerce.coercecolor(color)
            if color <> null then
              let layer.Color = color
              Doc.Views.Redraw()
        rc
    (*
    def LayerColor(layer, color=None):
        """Returns or changes the color of a layer.
        Parameters:
          layer (str|guid): name or id of an existing layer
          color (color): the new color value. If omitted, the current layer color is returned.
        Returns:
          color: If a color value is not specified, the current color value on success
          color: If a color value is specified, the previous color value on success
        Example:
          import rhinoscriptsyntax as rs
          import random
          from System.Drawing import Color
           
          def randomcolor():
              red = int(255*random.random())
              green = int(255*random.random())
              blue = int(255*random.random())
              return Color.FromArgb(red,green,blue)
           
          layerNames = rs.LayerNames()
          if layerNames:
              for name in layerNames: rs.LayerColor(name, randomcolor())
        See Also:
          
        """
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
    ///<returns>(unit) unit</returns>
    static member LayerColor(layer:string, color:Drawing.Color) : unit =
        let layer = __getlayer(layer, true)
        let rc = layer.Color
        if color then
            let color = Coerce.coercecolor(color)
            if color <> null then
              let layer.Color = color
              Doc.Views.Redraw()
        rc
    (*
    def LayerColor(layer, color=None):
        """Returns or changes the color of a layer.
        Parameters:
          layer (str|guid): name or id of an existing layer
          color (color): the new color value. If omitted, the current layer color is returned.
        Returns:
          color: If a color value is not specified, the current color value on success
          color: If a color value is specified, the previous color value on success
        Example:
          import rhinoscriptsyntax as rs
          import random
          from System.Drawing import Color
           
          def randomcolor():
              red = int(255*random.random())
              green = int(255*random.random())
              blue = int(255*random.random())
              return Color.FromArgb(red,green,blue)
           
          layerNames = rs.LayerNames()
          if layerNames:
              for name in layerNames: rs.LayerColor(name, randomcolor())
        See Also:
          
        """
        layer = __getlayer(layer, True)
        rc = layer.Color
        if color:
            color = rhutil.coercecolor(color)
            if color is not None:
              layer.Color = color
              scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the number of layers in the document</summary>
    ///<returns>(int) the number of layers in the document</returns>
    static member LayerCount() : int =
        Doc.Layers.ActiveCount
    (*
    def LayerCount():
        """Returns the number of layers in the document
        Returns:
          number: the number of layers in the document
        Example:
          import rhinoscriptsyntax as rs
          count = rs.LayerCount()
          print "There are", count, "layers."
        See Also:
          LayerNames
        """
        return scriptcontext.doc.Layers.ActiveCount
    *)


    ///<summary>Return identifiers of all layers in the document</summary>
    ///<returns>(Guid seq) the identifiers of all layers in the document</returns>
    static member LayerIds() : Guid seq =
        [| for layer in Doc.Layers if not <| layer.IsDeleted -> layer.Id |]
    (*
    def LayerIds():
        """Return identifiers of all layers in the document
        Returns:
          list(guid, ...): the identifiers of all layers in the document
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerIds()
          for layer in layers: print layer
        See Also:
          LayerCount
          LayerNames
        """
        return [layer.Id for layer in scriptcontext.doc.Layers if not layer.IsDeleted]
    *)


    ///<summary>Returns the linetype of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) Name of the current linetype</returns>
    static member LayerLinetype(layer:string) : string =
        let layer = __getlayer(layer, true)
        let index = layer.LinetypIndex
        let rc = Doc.Linetyps.[index].Name
        if linetyp then
            if not <| isinstance(linetyp, str) then linetyp <- string(linetyp)
            if linetyp = Doc.Linetyps.ContinuousLinetypName then
              index <- -1
            else
              let lt = Doc.Linetyps.FindName(linetyp)
              if lt = null then failwithf "Rhino.Scripting Error:LayerLinetype failed.  layer:"%A" linetyp:"%A"" layer linetyp
              index <- lt.LinetypIndex
            let layer.LinetypIndex = index
            Doc.Views.Redraw()
        rc
    (*
    def LayerLinetype(layer, linetype=None):
        """Returns or changes the linetype of a layer
        Parameters:
          layer (str): name of an existing layer
          linetype (str, optional): name of a linetype
        Returns:
          str: If linetype is not specified, name of the current linetype
          str: If linetype is specified, name of the previous linetype
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  if rs.LayerLinetype(layer)!="Continuous":
                      rs.LayerLinetype(layer,"Continuous")
        See Also:
          LayerPrintColor
          LayerPrintWidth
        """
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
    ///<returns>(unit) unit</returns>
    static member LayerLinetype(layer:string, linetyp:string) : unit =
        let layer = __getlayer(layer, true)
        let index = layer.LinetypIndex
        let rc = Doc.Linetyps.[index].Name
        if linetyp then
            if not <| isinstance(linetyp, str) then linetyp <- string(linetyp)
            if linetyp = Doc.Linetyps.ContinuousLinetypName then
              index <- -1
            else
              let lt = Doc.Linetyps.FindName(linetyp)
              if lt = null then failwithf "Rhino.Scripting Error:LayerLinetype failed.  layer:"%A" linetyp:"%A"" layer linetyp
              index <- lt.LinetypIndex
            let layer.LinetypIndex = index
            Doc.Views.Redraw()
        rc
    (*
    def LayerLinetype(layer, linetype=None):
        """Returns or changes the linetype of a layer
        Parameters:
          layer (str): name of an existing layer
          linetype (str, optional): name of a linetype
        Returns:
          str: If linetype is not specified, name of the current linetype
          str: If linetype is specified, name of the previous linetype
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  if rs.LayerLinetype(layer)!="Continuous":
                      rs.LayerLinetype(layer,"Continuous")
        See Also:
          LayerPrintColor
          LayerPrintWidth
        """
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


    ///<summary>Returns the locked mode of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(bool) The current layer locked mode</returns>
    static member LayerLocked(layer:string) : bool =
        let layer = __getlayer(layer, true)
        let rc = layer.IsLocked
        if locked<>null && locked<>rc then
            let layer.IsLocked = locked
            Doc.Views.Redraw()
        rc
    (*
    def LayerLocked(layer, locked=None):
        """Returns or changes the locked mode of a layer
        Parameters:
          layer (str): name of an existing layer
          locked (bool, optional): new layer locked mode
        Returns:
          bool: If locked is not specified, the current layer locked mode
          bool: If locked is specified, the previous layer locked mode
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  if rs.LayerLocked(layer): rs.LayerLocked(layer, False)
        See Also:
          LayerVisible
        """
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
    ///<returns>(unit) unit</returns>
    static member LayerLocked(layer:string, locked:bool) : unit =
        let layer = __getlayer(layer, true)
        let rc = layer.IsLocked
        if locked<>null && locked<>rc then
            let layer.IsLocked = locked
            Doc.Views.Redraw()
        rc
    (*
    def LayerLocked(layer, locked=None):
        """Returns or changes the locked mode of a layer
        Parameters:
          layer (str): name of an existing layer
          locked (bool, optional): new layer locked mode
        Returns:
          bool: If locked is not specified, the current layer locked mode
          bool: If locked is specified, the previous layer locked mode
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  if rs.LayerLocked(layer): rs.LayerLocked(layer, False)
        See Also:
          LayerVisible
        """
        layer = __getlayer(layer, True)
        rc = layer.IsLocked
        if locked!=None and locked!=rc:
            layer.IsLocked = locked
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) a zero-based material index</returns>
    static member LayerMaterialIndex(layer:string) : int =
        let layer = __getlayer(layer, true)
        let rc = layer.RenderMaterialIndex
        if index <> null && index>=-1 then
            let layer.RenderMaterialIndex = index
            Doc.Views.Redraw()
        rc
    (*
    def LayerMaterialIndex(layer,index=None):
        """Returns or changes the material index of a layer. A material index of -1
        indicates that no material has been assigned to the layer. Thus, the layer
        will use Rhino's default layer material
        Parameters:
          layer (str):  name of existing layer
          index (number, optional): the new material index
        Returns:
          number: a zero-based material index if successful
          number: a zero-based material index if successful
        Example:
          import rhinoscriptsyntax as rs
          index = rs.LayerMaterialIndex("Default")
          if index is not None:
              if index==-1:
                  print "The default layer does not have a material assigned."
              else:
                  print "The default layer has a material assigned."
        See Also:
          
        """
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
    static member LayerMaterialIndex(layer:string, index:int) : int =
        let layer = __getlayer(layer, true)
        let rc = layer.RenderMaterialIndex
        if index <> null && index>=-1 then
            let layer.RenderMaterialIndex = index
            Doc.Views.Redraw()
        rc
    (*
    def LayerMaterialIndex(layer,index=None):
        """Returns or changes the material index of a layer. A material index of -1
        indicates that no material has been assigned to the layer. Thus, the layer
        will use Rhino's default layer material
        Parameters:
          layer (str):  name of existing layer
          index (number, optional): the new material index
        Returns:
          number: a zero-based material index if successful
          number: a zero-based material index if successful
        Example:
          import rhinoscriptsyntax as rs
          index = rs.LayerMaterialIndex("Default")
          if index is not None:
              if index==-1:
                  print "The default layer does not have a material assigned."
              else:
                  print "The default layer has a material assigned."
        See Also:
          
        """
        layer = __getlayer(layer, True)
        rc = layer.RenderMaterialIndex
        if index is not None and index>=-1:
            layer.RenderMaterialIndex = index
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the identifier of a layer given the layer's name.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Guid) The layer's identifier .</returns>
    static member LayerId(layer:string) : Guid =
        let idx = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        string(Doc.Layers.[idx].Id) if idx !<- RhinoMath.UnsetIntIndex else null
    (*
    def LayerId(layer):
        """Returns the identifier of a layer given the layer's name.
        Parameters:
          layer (str): name of existing layer
        Returns:
          guid : The layer's identifier if successful.
          None: If not successful, or on error.
        Example:
          import rhinoscriptsyntax as  rs
          id = rs.LayerId('Layer 01')
        See Also:
          LayerName
        """
        idx = scriptcontext.doc.Layers.FindByFullPath(layer, UnsetIntIndex)
        return str(scriptcontext.doc.Layers[idx].Id) if idx != UnsetIntIndex else None
    *)


    ///<summary>Return the name of a layer given it's identifier</summary>
    ///<param name="layerId">(Guid) Layer identifier</param>
    ///<param name="fullpath">(bool) Optional, Default Value: <c>true</c>
    ///Return the full path name `True` or short name `False`</param>
    ///<returns>(string) the layer's name</returns>
    static member LayerName(layerId:Guid, [<OPT;DEF(true)>]fullpath:bool) : string =
        let layer = __getlayer(layerId, true)
        if fullpath then layer.FullPath
        layer.Name
    (*
    def LayerName(layer_id, fullpath=True):
        """Return the name of a layer given it's identifier
        Parameters:
          layer_id (guid): layer identifier
          fullpath (bool, optional): return the full path name `True` or short name `False`
        Returns:
          str: the layer's name if successful
          None: if not successful
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerIds()
          if layers:
              for layer in layers: print rs.LayerName(layer)
        See Also:
          LayerId
        """
        layer = __getlayer(layer_id, True)
        if fullpath: return layer.FullPath
        return layer.Name
    *)


    ///<summary>Returns the names of all layers in the document.</summary>
    ///<returns>(string seq) list of layer names</returns>
    static member LayerNames() : string seq =
        let rc = ResizeArray()
        for layer in Doc.Layers do
            if not <| layer.IsDeleted then rc.Add(layer.FullPath)
        if sort then rc.sort()
        rc
    (*
    def LayerNames(sort=False):
        """Returns the names of all layers in the document.
        Parameters:
          sort (bool, optional): return a sorted list of the layer names
        Returns:
          list(str, ...): list of layer names
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers: print layer
        See Also:
          LayerCount
        """
        rc = []
        for layer in scriptcontext.doc.Layers:
            if not layer.IsDeleted: rc.append(layer.FullPath)
        if sort: rc.sort()
        return rc
    *)


    ///<summary>Returns the current display order index of a layer as displayed in Rhino's
    ///  layer dialog box. A display order index of -1 indicates that the current
    ///  layer dialog filter does not allow the layer to appear in the layer list</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(float) 0 based index of layer</returns>
    static member LayerOrder(layer:string) : float =
        let layer = __getlayer(layer, true)
        layer.SortIndex
    (*
    def LayerOrder(layer):
        """Returns the current display order index of a layer as displayed in Rhino's
        layer dialog box. A display order index of -1 indicates that the current
        layer dialog filter does not allow the layer to appear in the layer list
        Parameters:
          layer (str): name of existing layer
        Returns:
          number: 0 based index of layer
        Example:
          import rhinoscriptsyntax as rs
          index = rs.LayerOrder("Default")
          if index is not None:
              if index==-1: print "The layer does not display in the Layer dialog."
              else: print "The layer does display in the Layer dialog."
        See Also:
          
        """
        layer = __getlayer(layer, True)
        return layer.SortIndex
    *)


    ///<summary>Returns the print color of a layer. Layer print colors are
    /// represented as RGB colors.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Drawing.Color) The current layer print color</returns>
    static member LayerPrintColor(layer:string) : Drawing.Color =
        let layer = __getlayer(layer, true)
        let rc = layer.PlotColor
        if color then
            let color = Coerce.coercecolor(color)
            let layer.PlotColor = color
            Doc.Views.Redraw()
        rc
    (*
    def LayerPrintColor(layer, color=None):
        """Returns or changes the print color of a layer. Layer print colors are
        represented as RGB colors.
        Parameters:
          layer (str): name of existing layer
          color (color): new print color
        Returns:
          color: if color is not specified, the current layer print color
          color: if color is specified, the previous layer print color
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  black = rs.CreateColor((0,0,0))
                  if rs.LayerPrintColor(layer)!=black:
                      rs.LayerPrintColor(layer, black)
        See Also:
          LayerLinetype
          LayerPrintWidth
        """
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
    ///<returns>(unit) unit</returns>
    static member LayerPrintColor(layer:string, color:Drawing.Color) : unit =
        let layer = __getlayer(layer, true)
        let rc = layer.PlotColor
        if color then
            let color = Coerce.coercecolor(color)
            let layer.PlotColor = color
            Doc.Views.Redraw()
        rc
    (*
    def LayerPrintColor(layer, color=None):
        """Returns or changes the print color of a layer. Layer print colors are
        represented as RGB colors.
        Parameters:
          layer (str): name of existing layer
          color (color): new print color
        Returns:
          color: if color is not specified, the current layer print color
          color: if color is specified, the previous layer print color
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  black = rs.CreateColor((0,0,0))
                  if rs.LayerPrintColor(layer)!=black:
                      rs.LayerPrintColor(layer, black)
        See Also:
          LayerLinetype
          LayerPrintWidth
        """
        layer = __getlayer(layer, True)
        rc = layer.PlotColor
        if color:
            color = rhutil.coercecolor(color)
            layer.PlotColor = color
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(float) The current layer print width</returns>
    static member LayerPrintWidth(layer:string) : float =
        let layer = __getlayer(layer, true)
        let rc = layer.PlotWeight
        if width <> null && width<>rc then
            let layer.PlotWeight = width
            Doc.Views.Redraw()
        rc
    (*
    def LayerPrintWidth(layer, width=None):
        """Returns or changes the print width of a layer. Print width is specified
        in millimeters. A print width of 0.0 denotes the "default" print width.
        Parameters:
          layer (str): name of existing layer
          width (number, optional): new print width
        Returns:
          number: if width is not specified, the current layer print width
          number: if width is specified, the previous layer print width
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  if rs.LayerPrintWidth(layer)!=0:
                      rs.LayerPrintWidth(layer, 0)
        See Also:
          LayerLinetype
          LayerPrintColor
        """
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
    ///<returns>(unit) unit</returns>
    static member LayerPrintWidth(layer:string, width:float) : unit =
        let layer = __getlayer(layer, true)
        let rc = layer.PlotWeight
        if width <> null && width<>rc then
            let layer.PlotWeight = width
            Doc.Views.Redraw()
        rc
    (*
    def LayerPrintWidth(layer, width=None):
        """Returns or changes the print width of a layer. Print width is specified
        in millimeters. A print width of 0.0 denotes the "default" print width.
        Parameters:
          layer (str): name of existing layer
          width (number, optional): new print width
        Returns:
          number: if width is not specified, the current layer print width
          number: if width is specified, the previous layer print width
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  if rs.LayerPrintWidth(layer)!=0:
                      rs.LayerPrintWidth(layer, 0)
        See Also:
          LayerLinetype
          LayerPrintColor
        """
        layer = __getlayer(layer, True)
        rc = layer.PlotWeight
        if width is not None and width!=rc:
            layer.PlotWeight = width
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the visible property of a layer.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(bool) The current layer visibility</returns>
    static member LayerVisible(layer:string) : bool =
        let layer = __getlayer(layer, true)
        let rc = layer.IsVisible
        if visible <> null then
            let layer.IsVisible = visible
            if visible && forcevisibleOrDonotpersist then
                Doc.Layers.ForceLayerVisible(layer.Id)
            if not <| visible && not <| forcevisibleOrDonotpersist then
              if layer.ParentLayerId <> Guid.Empty then
                layer.SetPersistentVisibility(visible)
            Doc.Views.Redraw()
        rc
    (*
    def LayerVisible(layer, visible=None, forcevisible_or_donotpersist=False):
        """Returns or changes the visible property of a layer.
        Parameters:
          layer (str): name of existing layer
          visible (bool, optional): new visible state
          forcevisible_or_donotpersist (bool, optional): if visible is True then turn parent layers on if True.  If visible is False then do not persist if True
        Returns:
          bool: if visible is not specified, the current layer visibility
          bool: if visible is specified, the previous layer visibility
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  if rs.LayerVisible(layer)==False:
                      rs.LayerVisible(layer,True)
        See Also:
          LayerLocked
        """
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
    ///<returns>(unit) unit</returns>
    static member LayerVisible(layer:string, visible:bool, [<OPT;DEF(false)>]forcevisibleOrDonotpersist:bool) : unit =
        let layer = __getlayer(layer, true)
        let rc = layer.IsVisible
        if visible <> null then
            let layer.IsVisible = visible
            if visible && forcevisibleOrDonotpersist then
                Doc.Layers.ForceLayerVisible(layer.Id)
            if not <| visible && not <| forcevisibleOrDonotpersist then
              if layer.ParentLayerId <> Guid.Empty then
                layer.SetPersistentVisibility(visible)
            Doc.Views.Redraw()
        rc
    (*
    def LayerVisible(layer, visible=None, forcevisible_or_donotpersist=False):
        """Returns or changes the visible property of a layer.
        Parameters:
          layer (str): name of existing layer
          visible (bool, optional): new visible state
          forcevisible_or_donotpersist (bool, optional): if visible is True then turn parent layers on if True.  If visible is False then do not persist if True
        Returns:
          bool: if visible is not specified, the current layer visibility
          bool: if visible is specified, the previous layer visibility
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          if layers:
              for layer in layers:
                  if rs.LayerVisible(layer)==False:
                      rs.LayerVisible(layer,True)
        See Also:
          LayerLocked
        """
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


    ///<summary>Return the parent layer of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) The name of the current parent layer</returns>
    static member ParentLayer(layer:string) : string =
        let layer = __getlayer(layer, true)
        let parent_id = layer.ParentLayerId
        let oldparent = null
        if parent_id<>Guid.Empty then
            let oldparentlayer = Doc.Layers.Find(parent_id, -1)
            if oldparentlayer <> null then
                oldparentlayer <- Doc.Layers.[oldparentlayer]
                oldparent <- oldparentlayer.FullPath
        if parent = null then oldparent
        if parent="" then
            let layer.ParentLayerId = Guid.Empty
        else
            let parent = __getlayer(parent, true)
            layer.ParentLayerId <- parent.Id
        oldparent
    (*
    def ParentLayer(layer, parent=None):
        """Return or modify the parent layer of a layer
        Parameters:
          layer (str): name of an existing layer
          parent (str, optional):  name of new parent layer. To remove the parent layer,
            thus making a root-level layer, specify an empty string
        Returns:
          str: If parent is not specified, the name of the current parent layer
          str: If parent is specified, the name of the previous parent layer
          None: if the layer does not have a parent
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          for layer in layers:
              parent = rs.ParentLayer(layer)
              print "Layer:", layer, ", Parent:", parent
        See Also:
          LayerChildren
        """
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
    ///<returns>(unit) unit</returns>
    static member ParentLayer(layer:string, parent:string) : unit =
        let layer = __getlayer(layer, true)
        let parent_id = layer.ParentLayerId
        let oldparent = null
        if parent_id<>Guid.Empty then
            let oldparentlayer = Doc.Layers.Find(parent_id, -1)
            if oldparentlayer <> null then
                oldparentlayer <- Doc.Layers.[oldparentlayer]
                oldparent <- oldparentlayer.FullPath
        if parent = null then oldparent
        if parent="" then
            let layer.ParentLayerId = Guid.Empty
        else
            let parent = __getlayer(parent, true)
            layer.ParentLayerId <- parent.Id
        oldparent
    (*
    def ParentLayer(layer, parent=None):
        """Return or modify the parent layer of a layer
        Parameters:
          layer (str): name of an existing layer
          parent (str, optional):  name of new parent layer. To remove the parent layer,
            thus making a root-level layer, specify an empty string
        Returns:
          str: If parent is not specified, the name of the current parent layer
          str: If parent is specified, the name of the previous parent layer
          None: if the layer does not have a parent
        Example:
          import rhinoscriptsyntax as rs
          layers = rs.LayerNames()
          for layer in layers:
              parent = rs.ParentLayer(layer)
              print "Layer:", layer, ", Parent:", parent
        See Also:
          LayerChildren
        """
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


    ///<summary>Removes an existing layer from the document. The layer will be removed
    ///  even if it contains geometry objects. The layer to be removed cannot be the
    ///  current layer
    ///  empty.</summary>
    ///<param name="layer">(string) The name or id of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member PurgeLayer(layer:string) : bool =
        let layer = __getlayer(layer, true)
        let rc = Doc.Layers.Purge( layer.LayerIndex, true)
        Doc.Views.Redraw()
        rc
    (*
    def PurgeLayer(layer):
        """Removes an existing layer from the document. The layer will be removed
        even if it contains geometry objects. The layer to be removed cannot be the
        current layer
        empty.
        Parameters:
          layer (str|guid): the name or id of an existing empty layer
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          layer = rs.GetString("Layer to purge")
          if layer: rs.PurgeLayer(layer)
        See Also:
          AddLayer
          CurrentLayer
          DeleteLayer
          RenameLayer
        """
        layer = __getlayer(layer, True)
        rc = scriptcontext.doc.Layers.Purge( layer.LayerIndex, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Renames an existing layer</summary>
    ///<param name="oldname">(string) Original layer name</param>
    ///<param name="newname">(string) New layer name</param>
    ///<returns>(string) The new layer name  otherwise None</returns>
    static member RenameLayer(oldname:string, newname:string) : string =
        if oldname && newname then
            let layer = __getlayer(oldname, true)
            let layer.Name = newname
            newname
    (*
    def RenameLayer(oldname, newname):
        """Renames an existing layer
        Parameters:
          oldname (str): original layer name
          newname (str): new layer name
        Returns: 
          str: The new layer name if successful otherwise None
        Example:
          import rhinoscriptsyntax as rs
          oldname = rs.GetString("Old layer name")
          if oldname:
              newname = rs.GetString("New layer name")
              if newname: rs.RenameLayer(oldname, newname)
        See Also:
          AddLayer
          CurrentLayer
          DeleteLayer
        """
        if oldname and newname:
            layer = __getlayer(oldname, True)
            layer.Name = newname
            return newname
    *)


