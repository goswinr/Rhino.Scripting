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
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(int) Material index of the layer</returns>
    static member AddMaterialToLayer(layer:string) : int =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if layer.RenderMaterialIndex> -1 then layer.RenderMaterialIndex
        else
            let materialindex = Doc.Materials.Add()
            layer.RenderMaterialIndex <- materialindex
            Doc.Views.Redraw()
            materialindex



    [<EXT>]
    ///<summary>Adds material to an object and returns the new material's index. If the
    ///  object already has a material, the the object's current material index is
    ///  returned</summary>
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


    [<EXT>]
    ///<summary>Copies definition of a source material to a destination material</summary>
    ///<param name="sourceIndex">(int) Source index of materials to copy</param>
    ///<param name="destinationIndex">(int) Destination index materials to copy</param>
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


    [<EXT>]
    ///<summary>Verifies a material is a copy of Rhino's built-in "default" material.
    ///  The default material is used by objects and layers that have not been
    ///  assigned a material</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialDefault(materialIndex:int) : bool =
        let mat = Doc.Materials.[materialIndex]
        notNull mat && mat.IsDefaultMaterial


    [<EXT>]
    ///<summary>Verifies a material is referenced from another file</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialReference(materialIndex:int) : bool =
        let mat = Doc.Materials.[materialIndex]
        notNull mat && mat.IsReference


    [<EXT>]
    ///<summary>Copies the material definition from one material to one or more objects</summary>
    ///<param name="source">(Guid) Source material index -or- identifier of the source object.
    ///  The object must have a material assigned</param>
    ///<param name="destination">(Guid seq) Id of the destination object</param>
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



    [<EXT>]
    ///<summary>Returns a material's bump bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current bump bitmap filename /returns>
    static member MaterialBump(materialIndex:int) : string option= //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialBump failed.  materialIndex:'%A' " materialIndex
        let texture = mat.GetBumpTexture()
        if notNull texture then Some texture.FileName else None


    [<EXT>]
    ///<summary>Modifies a material's bump bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The bump bitmap filename</param>
    ///<returns>(unit) void, nothing</returns>
    static member MaterialBump(materialIndex:int, filename:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        let texture = mat.GetBumpTexture()
        if IO.File.Exists filename then
            if not <| mat.SetBumpTexture(filename) then failwithf "Rhino.Scripting: MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            mat.CommitChanges()|> ignore
            Doc.Views.Redraw()
        else
            failwithf "Rhino.Scripting: MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename


    [<EXT>]
    ///<summary>Returns a material's diffuse color</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material color</returns>
    static member MaterialColor(materialIndex:int) : Drawing.Color = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialColor failed.  materialIndex:'%A'" materialIndex
        let rc = mat.DiffuseColor
        rc

    [<EXT>]
    ///<summary>Modifies a material's diffuse color</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing</returns>
    static member MaterialColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialColor failed.  materialIndex:'%A' color:'%A'" materialIndex color
        mat.DiffuseColor <- color
        mat.CommitChanges()|> ignore
        Doc.Views.Redraw()


    [<EXT>]
    ///<summary>Returns a material's environment bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current environment bitmap filename</returns>
    static member MaterialEnvironmentMap(materialIndex:int) : string option= //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialEnvironmentMap failed.  materialIndex:'%A'" materialIndex
        let texture = mat.GetEnvironmentTexture()
        if notNull texture then Some texture.FileName  else None

    [<EXT>]
    ///<summary>Modifies a material's environment bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The environment bitmap filename</param>
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

    [<EXT>]
    ///<summary>Modifies a material's user defined name</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="name">(string) The new name</param>
    ///<returns>(unit) void, nothing</returns>
    static member MaterialName(materialIndex:int, name:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialName failed.  materialIndex:'%A' name:'%A'" materialIndex name
        mat.Name <- name
        mat.CommitChanges()|> ignore



    [<EXT>]
    ///<summary>Returns a material's reflective color</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material reflective color</returns>
    static member MaterialReflectiveColor(materialIndex:int) : Drawing.Color = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialReflectiveColor failed.  materialIndex:'%A' " materialIndex
        let rc = mat.ReflectionColor
        rc

    [<EXT>]
    ///<summary>Modifies a material's reflective color</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing</returns>
    static member MaterialReflectiveColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialReflectiveColor failed.  materialIndex:'%A' color:'%A'" materialIndex color
        mat.ReflectionColor <- color
        mat.CommitChanges() |> ignore
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns a material's shine value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(float) The current material shine value
    ///  0.0 being matte and 255.0 being glossy</returns>
    static member MaterialShine(materialIndex:int) : float = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialShine failed.  materialIndex:'%A' " materialIndex
        let rc = mat.Shine
        rc

    [<EXT>]
    ///<summary>Modifies a material's shine value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="shine">(float) The new shine value. A material's shine value ranges from 0.0 to 255.0, with
    ///  0.0 being matte and 255.0 being glossy</param>
    ///<returns>(unit) void, nothing</returns>
    static member MaterialShine(materialIndex:int, shine:float) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialShine failed.  materialIndex:'%A' shine:'%A'" materialIndex shine

        mat.Shine <- shine
        mat.CommitChanges() |> ignore
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns a material's texture bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current texture bitmap filename</returns>
    static member MaterialTexture(materialIndex:int) : string option = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialTexture failed.  materialIndex:'%A' " materialIndex
        let texture = mat.GetBitmapTexture()
        if notNull texture then  Some texture.FileName else None

    [<EXT>]
    ///<summary>Modifies a material's texture bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The texture bitmap filename</param>
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

    [<EXT>]
    ///<summary>Modifies a material's transparency value</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="transparency">(float) The new transparency value. A material's transparency value ranges from 0.0 to 1.0, with
    ///  0.0 being opaque and 1.0 being transparent</param>
    ///<returns>(unit) void, nothing</returns>
    static member MaterialTransparency(materialIndex:int, transparency:float) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialTransparency failed.  materialIndex:'%A' transparency:'%A'" materialIndex transparency
        mat.Transparency <- transparency
        mat.CommitChanges() |> ignore
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns a material's transparency bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current transparency bitmap filename</returns>
    static member MaterialTransparencyMap(materialIndex:int) : string option = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then failwithf "Rhino.Scripting: MaterialTransparencyMap failed.  materialIndex:'%A' " materialIndex
        let texture = mat.GetTransparencyTexture()
        if notNull texture then  Some texture.FileName else None


    [<EXT>]
    ///<summary>Modifies a material's transparency bitmap filename</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The transparency bitmap filename</param>
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


