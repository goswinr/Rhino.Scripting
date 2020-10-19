namespace Rhino.Scripting

open FsEx
open System
open Rhino

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

 
[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsLayer =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with


    //static member AddLayer() // moved to file RhinoScriptSyntax.fs

    [<Extension>]
    ///<summary>Returns the current layer</summary>
    ///<returns>(string) The full name of the current layer</returns>
    static member CurrentLayer() : string = //GET
        Doc.Layers.CurrentLayer.FullPath


    [<Extension>]
    ///<summary>Changes the current layer</summary>
    ///<param name="layer">(string) The name or Guid of an existing layer to make current</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentLayer(layer:string) : unit = //SET
        let rc = Doc.Layers.CurrentLayer.FullPath
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then Error.Raise <| sprintf "RhinoScriptSyntax.CoerceLayer: could not Coerce Layer from name'%A'" layer
        if not<|  Doc.Layers.SetCurrentLayerIndex(i, true) then Error.Raise <| sprintf "RhinoScriptSyntax.Set CurrentLayer to %A failed" layer



    [<Extension>]
    ///<summary>Removes an existing layer from the document. The layer to be removed
    ///    cannot be the current layer. Unlike the PurgeLayer method, the layer must
    ///    be empty, or contain no objects, before it can be removed. Any layers that
    ///    are children of the specified layer will also be removed if they are also
    ///    empty</summary>
    ///<param name="layer">(string) The name of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteLayer(layer:string) : bool =
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then Error.Raise <| sprintf "RhinoScriptSyntax.CoerceLayer: could not Coerce Layer from name'%A'" layer
        Doc.Layers.Delete(i, true)


    [<Extension>]
    ///<summary>Expands a layer. Expanded layers can be viewed in Rhino's layer dialog</summary>
    ///<param name="layer">(string) Name of the layer to expand</param>
    ///<param name="expand">(bool) True to expand, False to collapse</param>
    ///<returns>(unit) void, nothing</returns>
    static member ExpandLayer(layer:string, expand:bool) : unit =
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then Error.Raise <| sprintf "RhinoScriptSyntax.CoerceLayer: could not Coerce Layer from name'%A'" layer
        let layer = Doc.Layers.[i]
        if layer.IsExpanded <> expand then
            layer.IsExpanded <- expand




    [<Extension>]
    ///<summary>Verifies the existance of a layer in the document</summary>
    ///<param name="layer">(string) The name of a layer to search for</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayer(layer:string) : bool =
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        i <> RhinoMath.UnsetIntIndex


    [<Extension>]
    ///<summary>Verifies that the objects on a layer can be changed (normal)</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerChangeable(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.IsVisible && not layer.IsLocked
        rc


    [<Extension>]
    ///<summary>Verifies that a layer is a child of another layer</summary>
    ///<param name="layer">(string) The name of the layer to test against</param>
    ///<param name="test">(string) The name to the layer to test</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerChildOf(layer:string, test:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let test = RhinoScriptSyntax.CoerceLayer(test)
        test.IsChildOf(layer)


    [<Extension>]
    ///<summary>Verifies that a layer is the current layer</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerCurrent(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.Index = Doc.Layers.CurrentLayerIndex


    [<Extension>]
    ///<summary>Verifies that an existing layer is empty, or contains no objects</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerEmpty(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rhobjs = Doc.Objects.FindByLayer(layer)
        isNull rhobjs || rhobjs.Length = 0


    [<Extension>]
    ///<summary>Verifies that a layer is expanded. Expanded layers can be viewed in
    ///    Rhino's layer dialog</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerExpanded(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsExpanded


    [<Extension>]
    ///<summary>Verifies that a layer is locked</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerLocked(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsLocked


    [<Extension>]
    ///<summary>Verifies that a layer is on</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerOn(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible


    [<Extension>]
    ///<summary>Verifies that an existing layer is selectable (normal and reference)</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerSelectable(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible && not layer.IsLocked


    [<Extension>]
    ///<summary>Verifies that a layer is a parent of another layer</summary>
    ///<param name="layer">(string) The name of the layer to test against</param>
    ///<param name="test">(string) The name to the layer to test</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerParentOf(layer:string, test:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let test = RhinoScriptSyntax.CoerceLayer(test)
        test.IsParentOf(layer)


    [<Extension>]
    ///<summary>Verifies that a layer is from a reference file</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerReference(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsReference


    [<Extension>]
    ///<summary>Verifies that a layer is visible (normal, locked, and reference)</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False</returns>
    static member IsLayerVisible(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible


    [<Extension>]
    ///<summary>Returns the number of immediate child layers of a layer</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(int) the number of immediate child layers</returns>
    static member LayerChildCount(layer:string) : int =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let children = layer.GetChildren()
        if notNull children then children.Length
        else 0


    [<Extension>]
    ///<summary>Returns the immediate child layers of a layer</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(string Rarr) List of children layer names</returns>
    static member LayerChildren(layer:string) : string Rarr =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let children = layer.GetChildren()
        if notNull children then rarr {for child in children do child.FullPath }
        else rarr { () } //empty list


    [<Extension>]
    ///<summary>Returns the color of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(Drawing.Color) The current color value </returns>
    static member LayerColor(layer:string) : Drawing.Color = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.Color

    [<Extension>]
    ///<summary>Changes the color of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerColor(layer:string, color:Drawing.Color) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.Color <- color
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the number of layers in the document</summary>
    ///<returns>(int) the number of layers in the document</returns>
    static member LayerCount() : int =
        Doc.Layers.ActiveCount


    [<Extension>]
    ///<summary>Return identifiers of all layers in the document</summary>
    ///<returns>(Guid Rarr) the identifiers of all layers in the document</returns>
    static member LayerIds() : Guid Rarr =
        rarr {for layer in Doc.Layers do
                        if not layer.IsDeleted then
                            layer.Id }


    [<Extension>]
    ///<summary>Returns the linetype of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) Name of the current linetype</returns>
    static member LayerLinetype(layer:string) : string = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let index = layer.LinetypeIndex
        Doc.Linetypes.[index].Name


    [<Extension>]
    ///<summary>Changes the linetype of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="linetyp">(string) Name of a linetype</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerLinetype(layer:string, linetyp:string) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let mutable index = layer.LinetypeIndex
        if linetyp = Doc.Linetypes.ContinuousLinetypeName then
            index <- -1
        else
            let lt = Doc.Linetypes.FindName(linetyp)
            if lt|> isNull  then Error.Raise <| sprintf "RhinoScriptSyntax.LayerLinetype not found.  layer:'%A' linetyp:'%A'" layer linetyp
            index <- lt.LinetypeIndex
        layer.LinetypeIndex <- index
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the locked mode of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(bool) The current layer locked mode</returns>
    static member LayerLocked(layer:string) : bool = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsLocked


    [<Extension>]
    ///<summary>Changes the locked mode of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="locked">(bool) New layer locked mode</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerLocked(layer:string, locked:bool) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if locked <> layer.IsLocked then
            layer.IsLocked <- locked
            Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) a zero-based material index</returns>
    static member LayerMaterialIndex(layer:string) : int = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.RenderMaterialIndex

    [<Extension>]
    ///<summary>Changes the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="index">(int) The new material index</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerMaterialIndex(layer:string, index:int) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if  index >= -1 then
            layer.RenderMaterialIndex <- index
            Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the identifier of a layer given the layer's name</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Guid) The layer's identifier</returns>
    static member LayerId(layer:string) : Guid =
        let idx = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if idx = RhinoMath.UnsetIntIndex then Error.Raise <| sprintf "RhinoScriptSyntax.LayerId not found for name %s" layer
        Doc.Layers.[idx].Id



    [<Extension>]
    ///<summary>Return the name of a layer given it's identifier</summary>
    ///<param name="layerId">(Guid) Layer identifier</param>
    ///<param name="fullpath">(bool) Optional, Default Value: <c>true</c>
    ///    Return the full path name `True` or short name `False`</param>
    ///<returns>(string) the layer's name</returns>
    static member LayerName(layerId:Guid, [<OPT;DEF(true)>]fullpath:bool) : string =
        let layer = RhinoScriptSyntax.CoerceLayer(layerId)
        if fullpath then layer.FullPath
        else layer.Name


    [<Extension>]
    ///<summary>Returns the names of all layers in the document</summary>
    ///<returns>(string Rarr) list of layer names</returns>
    static member LayerNames() : string Rarr =
        let rc = Rarr()
        for layer in Doc.Layers do
            if not layer.IsDeleted then rc.Add(layer.FullPath)
        rc


    [<Extension>]
    ///<summary>Returns the current display order index of a layer as displayed in Rhino's
    ///    layer dialog box. A display order index of -1 indicates that the current
    ///    layer dialog filter does not allow the layer to appear in the layer list</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) 0 based index of layer</returns>
    static member LayerOrder(layer:string) : int =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.SortIndex


    [<Extension>]
    ///<summary>Returns the print color of a layer. Layer print colors are
    /// represented as RGB colors</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Drawing.Color) The current layer print color</returns>
    static member LayerPrintColor(layer:string) : Drawing.Color = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotColor
        rc

    [<Extension>]
    ///<summary>Changes the print color of a layer. Layer print colors are
    /// represented as RGB colors</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="color">(Drawing.Color) New print color</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerPrintColor(layer:string, color:Drawing.Color) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotColor
        //color = RhinoScriptSyntax.Coercecolor(color)
        layer.PlotColor <- color
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(float) The current layer print width</returns>
    static member LayerPrintWidth(layer:string) : float = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotWeight
        rc

    [<Extension>]
    ///<summary>Changes the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="width">(float) New print width</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerPrintWidth(layer:string, width:float) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotWeight
        if width <> rc then
            layer.PlotWeight <- width
            Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the visible property of a layer</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(bool) The current layer visibility</returns>
    static member LayerVisible(layer:string) : bool = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.IsVisible
        rc

    [<Extension>]
    ///<summary>Changes the visible property of a layer</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="visible">(bool) New visible state</param>
    ///<param name="forcevisibleOrDonotpersist">(bool) If visible is True then turn parent layers on if True.  If visible is False then do not persist if True</param>
    ///<returns>(unit) void, nothing</returns>
    static member LayerVisible(layer:string, visible:bool, [<OPT;DEF(false)>]forcevisibleOrDonotpersist:bool) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible <- visible
        if visible && forcevisibleOrDonotpersist then
            Doc.Layers.ForceLayerVisible(layer.Id) |> ignore
        if not visible && not forcevisibleOrDonotpersist then
            if layer.ParentLayerId <> Guid.Empty then
                layer.SetPersistentVisibility(visible)
            // layer.CommitChanges() |> ignore //obsolete !!
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Return the parent layer of a layer or mepty string if no parent present</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) The name of the current parent layer</returns>
    static member ParentLayer(layer:string) : string = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if layer.ParentLayerId = Guid.Empty then ""
        else
            let oldparentlayer = Doc.Layers.FindId(layer.ParentLayerId)
            if isNull oldparentlayer then "" // TODO test! is fail better ?
            else
                oldparentlayer.FullPath


    [<Extension>]
    ///<summary>Modify the parent layer of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="parent">(string) Name of new parent layer. To remove the parent layer,
    ///    thus making a root-level layer, specify an empty string</param>
    ///<returns>(unit) void, nothing</returns>
    static member ParentLayer(layer:string, parent:string) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if parent = "" then
            layer.ParentLayerId <- Guid.Empty
        else
            let parent = RhinoScriptSyntax.CoerceLayer(parent)
            layer.ParentLayerId <- parent.Id



    [<Extension>]
    ///<summary>Removes an existing layer from the document. The layer will be removed
    ///    even if it contains geometry objects. The layer to be removed cannot be the
    ///    current layer
    ///    empty</summary>
    ///<param name="layer">(string) The name of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member PurgeLayer(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = Doc.Layers.Purge( layer.Index, true)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Renames an existing layer</summary>
    ///<param name="oldname">(string) Original layer name</param>
    ///<param name="newname">(string) New layer name</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenameLayer(oldname:string, newname:string) : unit =
        if oldname <> newname then
            let layer = RhinoScriptSyntax.CoerceLayer(oldname)
            layer.Name <- newname // TODO test with bad chars in layer string



