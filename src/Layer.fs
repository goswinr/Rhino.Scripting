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
        failNotImpl () 


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
        failNotImpl () 


    ///<summary>Returns the current layer</summary>
    ///<returns>(string) The full name of the current layer</returns>
    static member CurrentLayer() : string =
        failNotImpl () 

    ///<summary>Changes the current layer</summary>
    ///<param name="layer">(Guid)The name or Guid of an existing layer to make current</param>
    ///<returns>(unit) unit</returns>
    static member CurrentLayer(layer:Guid) : unit =
        failNotImpl () 


    ///<summary>Removes an existing layer from the document. The layer to be removed
    ///  cannot be the current layer. Unlike the PurgeLayer method, the layer must
    ///  be empty, or contain no objects, before it can be removed. Any layers that
    ///  are children of the specified layer will also be removed if they are also
    ///  empty.</summary>
    ///<param name="layer">(string) The name or id of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteLayer(layer:string) : bool =
        failNotImpl () 


    ///<summary>Expands a layer. Expanded layers can be viewed in Rhino's layer dialog</summary>
    ///<param name="layer">(string) Name of the layer to expand</param>
    ///<param name="expand">(bool) True to expand, False to collapse</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ExpandLayer(layer:string, expand:bool) : bool =
        failNotImpl () 


    ///<summary>Verifies the existance of a layer in the document</summary>
    ///<param name="layer">(string) The name or id of a layer to search for</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayer(layer:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that the objects on a layer can be changed (normal)</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerChangeable(layer:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a layer is a child of another layer</summary>
    ///<param name="layer">(string) The name or id of the layer to test against</param>
    ///<param name="test">(string) The name or id to the layer to test</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerChildOf(layer:string, test:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a layer is the current layer</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerCurrent(layer:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that an existing layer is empty, or contains no objects</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerEmpty(layer:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a layer is expanded. Expanded layers can be viewed in
    ///  Rhino's layer dialog</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerExpanded(layer:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a layer is locked.</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerLocked(layer:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a layer is on.</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerOn(layer:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that an existing layer is selectable (normal and reference)</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerSelectable(layer:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a layer is a parent of another layer</summary>
    ///<param name="layer">(string) The name or id of the layer to test against</param>
    ///<param name="test">(string) The name or id to the layer to test</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerParentOf(layer:string, test:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a layer is from a reference file.</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerReference(layer:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a layer is visible (normal, locked, and reference)</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerVisible(layer:string) : bool =
        failNotImpl () 


    ///<summary>Returns the number of immediate child layers of a layer</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(int) the number of immediate child layers</returns>
    static member LayerChildCount(layer:string) : int =
        failNotImpl () 


    ///<summary>Returns the immediate child layers of a layer</summary>
    ///<param name="layer">(string) The name or id of an existing layer</param>
    ///<returns>(string seq) List of children layer names</returns>
    static member LayerChildren(layer:string) : string seq =
        failNotImpl () 


    ///<summary>Returns the color of a layer.</summary>
    ///<param name="layer">(string) Name or id of an existing layer</param>
    ///<returns>(Drawing.Color) The current color value on success</returns>
    static member LayerColor(layer:string) : Drawing.Color =
        failNotImpl () 

    ///<summary>Changes the color of a layer.</summary>
    ///<param name="layer">(string) Name or id of an existing layer</param>
    ///<param name="color">(Drawing.Color)The new color value. If omitted, the current layer color is returned.</param>
    ///<returns>(unit) unit</returns>
    static member LayerColor(layer:string, color:Drawing.Color) : unit =
        failNotImpl () 


    ///<summary>Returns the number of layers in the document</summary>
    ///<returns>(int) the number of layers in the document</returns>
    static member LayerCount() : int =
        failNotImpl () 


    ///<summary>Return identifiers of all layers in the document</summary>
    ///<returns>(Guid seq) the identifiers of all layers in the document</returns>
    static member LayerIds() : Guid seq =
        failNotImpl () 


    ///<summary>Returns the linetype of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) Name of the current linetype</returns>
    static member LayerLinetype(layer:string) : string =
        failNotImpl () 

    ///<summary>Changes the linetype of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="linetyp">(string)Name of a linetyp</param>
    ///<returns>(unit) unit</returns>
    static member LayerLinetype(layer:string, linetyp:string) : unit =
        failNotImpl () 


    ///<summary>Returns the locked mode of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(bool) The current layer locked mode</returns>
    static member LayerLocked(layer:string) : bool =
        failNotImpl () 

    ///<summary>Changes the locked mode of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="locked">(bool)New layer locked mode</param>
    ///<returns>(unit) unit</returns>
    static member LayerLocked(layer:string, locked:bool) : unit =
        failNotImpl () 


    ///<summary>Returns the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) a zero-based material index</returns>
    static member LayerMaterialIndex(layer:string) : int =
        failNotImpl () 

    ///<summary>Changes the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="index">(int)The new material index</param>
    ///<returns>(int) a zero-based material index</returns>
    static member LayerMaterialIndex(layer:string, index:int) : int =
        failNotImpl () 


    ///<summary>Returns the identifier of a layer given the layer's name.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Guid) The layer's identifier .</returns>
    static member LayerId(layer:string) : Guid =
        failNotImpl () 


    ///<summary>Return the name of a layer given it's identifier</summary>
    ///<param name="layerId">(Guid) Layer identifier</param>
    ///<param name="fullpath">(bool) Optional, Default Value: <c>true</c>
    ///Return the full path name `True` or short name `False`</param>
    ///<returns>(string) the layer's name</returns>
    static member LayerName(layerId:Guid, [<OPT;DEF(true)>]fullpath:bool) : string =
        failNotImpl () 


    ///<summary>Returns the names of all layers in the document.</summary>
    ///<returns>(string seq) list of layer names</returns>
    static member LayerNames() : string seq =
        failNotImpl () 


    ///<summary>Returns the current display order index of a layer as displayed in Rhino's
    ///  layer dialog box. A display order index of -1 indicates that the current
    ///  layer dialog filter does not allow the layer to appear in the layer list</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(float) 0 based index of layer</returns>
    static member LayerOrder(layer:string) : float =
        failNotImpl () 


    ///<summary>Returns the print color of a layer. Layer print colors are
    /// represented as RGB colors.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Drawing.Color) The current layer print color</returns>
    static member LayerPrintColor(layer:string) : Drawing.Color =
        failNotImpl () 

    ///<summary>Changes the print color of a layer. Layer print colors are
    /// represented as RGB colors.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="color">(Drawing.Color)New print color</param>
    ///<returns>(unit) unit</returns>
    static member LayerPrintColor(layer:string, color:Drawing.Color) : unit =
        failNotImpl () 


    ///<summary>Returns the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(float) The current layer print width</returns>
    static member LayerPrintWidth(layer:string) : float =
        failNotImpl () 

    ///<summary>Changes the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="width">(float)New print width</param>
    ///<returns>(unit) unit</returns>
    static member LayerPrintWidth(layer:string, width:float) : unit =
        failNotImpl () 


    ///<summary>Returns the visible property of a layer.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(bool) The current layer visibility</returns>
    static member LayerVisible(layer:string) : bool =
        failNotImpl () 

    ///<summary>Changes the visible property of a layer.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="visible">(bool)New visible state</param>
    ///<param name="forcevisibleOrDonotpersist">(bool)If visible is True then turn parent layers on if True.  If visible is False then do not persist if True</param>
    ///<returns>(unit) unit</returns>
    static member LayerVisible(layer:string, visible:bool, [<OPT;DEF(false)>]forcevisibleOrDonotpersist:bool) : unit =
        failNotImpl () 


    ///<summary>Return the parent layer of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) The name of the current parent layer</returns>
    static member ParentLayer(layer:string) : string =
        failNotImpl () 

    ///<summary>Modify the parent layer of a layer</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="parent">(string)Name of new parent layer. To remove the parent layer,
    ///  thus making a root-level layer, specify an empty string</param>
    ///<returns>(unit) unit</returns>
    static member ParentLayer(layer:string, parent:string) : unit =
        failNotImpl () 


    ///<summary>Removes an existing layer from the document. The layer will be removed
    ///  even if it contains geometry objects. The layer to be removed cannot be the
    ///  current layer
    ///  empty.</summary>
    ///<param name="layer">(string) The name or id of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member PurgeLayer(layer:string) : bool =
        failNotImpl () 


    ///<summary>Renames an existing layer</summary>
    ///<param name="oldname">(string) Original layer name</param>
    ///<param name="newname">(string) New layer name</param>
    ///<returns>(string) The new layer name  otherwise None</returns>
    static member RenameLayer(oldname:string, newname:string) : string =
        failNotImpl () 


