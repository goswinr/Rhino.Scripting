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
        failNotImpl()

    ///<summary>Adds material to an object and returns the new material's index. If the
    ///  object already has a material, the the object's current material index is
    ///  returned.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(float) material index of the object</returns>
    static member AddMaterialToObject(objectId:Guid) : float =
        failNotImpl()

    ///<summary>Copies definition of a source material to a destination material</summary>
    ///<param name="sourceIndex">(int) Source index of 'indices of materials to copy' (FIXME 0)</param>
    ///<param name="destinationIndex">(int) Destination index of 'indices of materials to copy' (FIXME 0)</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CopyMaterial(sourceIndex:int, destinationIndex:int) : bool =
        failNotImpl()

    ///<summary>Verifies a material is a copy of Rhino's built-in "default" material.
    ///  The default material is used by objects and layers that have not been
    ///  assigned a material.</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialDefault(materialIndex:int) : bool =
        failNotImpl()

    ///<summary>Verifies a material is referenced from another file</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialReference(materialIndex:int) : bool =
        failNotImpl()

    ///<summary>Copies the material definition from one material to one or more objects</summary>
    ///<param name="source">(Guid) Source material index -or- identifier of the source object.
    ///  The object must have a material assigned</param>
    ///<param name="destination">(Guid seq) Identifiers(s) of the destination object(s)</param>
    ///<returns>(float) number of objects that were modified</returns>
    static member MatchMaterial(source:Guid, destination:Guid seq) : float =
        failNotImpl()

    ///<summary>Returns a material's bump bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current bump bitmap filename</returns>
    static member MaterialBump(materialIndex:int) : string =
        failNotImpl()

    ///<summary>Modifies a material's bump bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string)The bump bitmap filename</param>
    ///<returns>(unit) unit</returns>
    static member MaterialBump(materialIndex:int, filename:string) : unit =
        failNotImpl()

    ///<summary>Returns a material's diffuse color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material color</returns>
    static member MaterialColor(materialIndex:int) : Drawing.Color =
        failNotImpl()

    ///<summary>Modifies a material's diffuse color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color)The new color value</param>
    ///<returns>(unit) unit</returns>
    static member MaterialColor(materialIndex:int, color:Drawing.Color) : unit =
        failNotImpl()

    ///<summary>Returns a material's environment bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current environment bitmap filename</returns>
    static member MaterialEnvironmentMap(materialIndex:int) : string =
        failNotImpl()

    ///<summary>Modifies a material's environment bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string)The environment bitmap filename</param>
    ///<returns>(unit) unit</returns>
    static member MaterialEnvironmentMap(materialIndex:int, filename:string) : unit =
        failNotImpl()

    ///<summary>Returns a material's user defined name</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current material name</returns>
    static member MaterialName(materialIndex:int) : string =
        failNotImpl()

    ///<summary>Modifies a material's user defined name</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="name">(string)The new name</param>
    ///<returns>(unit) unit</returns>
    static member MaterialName(materialIndex:int, name:string) : unit =
        failNotImpl()

    ///<summary>Returns a material's reflective color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material reflective color</returns>
    static member MaterialReflectiveColor(materialIndex:int) : Drawing.Color =
        failNotImpl()

    ///<summary>Modifies a material's reflective color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color)The new color value</param>
    ///<returns>(unit) unit</returns>
    static member MaterialReflectiveColor(materialIndex:int, color:Drawing.Color) : unit =
        failNotImpl()

    ///<summary>Returns a material's shine value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(int) The current material shine value
    ///  0.0 being matte and 255.0 being glossy</returns>
    static member MaterialShine(materialIndex:int) : int =
        failNotImpl()

    ///<summary>Modifies a material's shine value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="shine">(float)The new shine value. A material's shine value ranges from 0.0 to 255.0, with
    ///  0.0 being matte and 255.0 being glossy</param>
    ///<returns>(unit) unit</returns>
    static member MaterialShine(materialIndex:int, shine:float) : unit =
        failNotImpl()

    ///<summary>Returns a material's texture bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current texture bitmap filename</returns>
    static member MaterialTexture(materialIndex:int) : string =
        failNotImpl()

    ///<summary>Modifies a material's texture bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string)The texture bitmap filename</param>
    ///<returns>(unit) unit</returns>
    static member MaterialTexture(materialIndex:int, filename:string) : unit =
        failNotImpl()

    ///<summary>Returns a material's transparency value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(int) The current material transparency value
    ///  0.0 being opaque and 1.0 being transparent</returns>
    static member MaterialTransparency(materialIndex:int) : int =
        failNotImpl()

    ///<summary>Modifies a material's transparency value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="transparency">(float)The new transparency value. A material's transparency value ranges from 0.0 to 1.0, with
    ///  0.0 being opaque and 1.0 being transparent</param>
    ///<returns>(unit) unit</returns>
    static member MaterialTransparency(materialIndex:int, transparency:float) : unit =
        failNotImpl()

    ///<summary>Returns a material's transparency bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current transparency bitmap filename</returns>
    static member MaterialTransparencyMap(materialIndex:int) : string =
        failNotImpl()

    ///<summary>Modifies a material's transparency bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string)The transparency bitmap filename</param>
    ///<returns>(unit) unit</returns>
    static member MaterialTransparencyMap(materialIndex:int, filename:string) : unit =
        failNotImpl()

    ///<summary>Resets a material to Rhino's default material</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ResetMaterial(materialIndex:int) : bool =
        failNotImpl()

