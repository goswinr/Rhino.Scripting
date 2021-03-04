namespace Rhino.Scripting

open FsEx
open System
open Rhino

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

 
[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsMaterial =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with


    ///<summary>Add material to a layer and returns the new material's index. If the
    ///    layer already has a material, then the layer's current material index is
    ///    returned.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(int) Material index of the layer.</returns>
    [<Extension>]
    static member AddMaterialToLayer(layer:string) : int =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if layer.RenderMaterialIndex> -1 then layer.RenderMaterialIndex
        else
            let materialindex = Doc.Materials.Add()
            layer.RenderMaterialIndex <- materialindex
            Doc.Views.Redraw()
            materialindex

    ///<summary>Adds material to an object and returns the new material's index. If the
    ///    object already has a material, the the object's current material index is returned.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(int) material index of the object.</returns>
    [<Extension>]
    static member AddMaterialToObject(objectId:Guid) : int =
        let rhinoobject = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let mutable attr = rhinoobject.Attributes
        if attr.MaterialSource <> DocObjects.ObjectMaterialSource.MaterialFromObject then
            attr.MaterialSource <- DocObjects.ObjectMaterialSource.MaterialFromObject
            Doc.Objects.ModifyAttributes(rhinoobject, attr, true)|> ignore
            attr <- rhinoobject.Attributes
        let mutable materialindex = attr.MaterialIndex
        if materialindex> -1 then materialindex
        else
            materialindex <- Doc.Materials.Add()
            attr.MaterialIndex <- materialindex
            Doc.Objects.ModifyAttributes(rhinoobject, attr, true)|> ignore
            materialindex


    ///<summary>Copies definition of a source material to a destination material.</summary>
    ///<param name="sourceIndex">(int) Source index of materials to copy</param>
    ///<param name="destinationIndex">(int) Destination index materials to copy</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member CopyMaterial(sourceIndex:int, destinationIndex:int) : bool =
        if sourceIndex = destinationIndex then true // orignaly false
        else
            let source = Doc.Materials.[sourceIndex]
            if source|> isNull  then false
            else
                let rc = Doc.Materials.Modify(source, destinationIndex, true)
                if rc then Doc.Views.Redraw()
                rc


    ///<summary>Verifies a material is a copy of Rhino's built-in "default" material.
    ///    The default material is used by objects and layers that have not been
    ///    assigned a material.</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member IsMaterialDefault(materialIndex:int) : bool =
        let mat = Doc.Materials.[materialIndex]
        notNull mat && mat.IsDefaultMaterial


    ///<summary>Verifies a material is referenced from another file.</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member IsMaterialReference(materialIndex:int) : bool =
        let mat = Doc.Materials.[materialIndex]
        notNull mat && mat.IsReference


    ///<summary>Copies the material definition from one material to one or more objects.</summary>
    ///<param name="source">(Guid) Source material index -or- identifier of the source object.
    ///    The object must have a material assigned</param>
    ///<param name="destination">(Guid seq) Id of the destination object</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MatchMaterial(source:Guid, destination:Guid seq) : unit =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(source)
        let source = rhobj.Attributes.MaterialIndex
        let mat = Doc.Materials.[source]
        if isNull mat then RhinoScriptingException.Raise "RhinoScriptSyntax.MatchMaterial failed.  source:'%A' destination:'%A'" source destination

        for objectId in destination do
            let rhobj = Doc.Objects.FindId(objectId)
            if notNull rhobj then
                rhobj.Attributes.MaterialIndex <- source
                rhobj.Attributes.MaterialSource <- DocObjects.ObjectMaterialSource.MaterialFromObject
                rhobj.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 
        Doc.Views.Redraw()



    ///<summary>Returns a material's bump bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current bump bitmap filename.</returns>
    [<Extension>]
    static member MaterialBump(materialIndex:int) : string option= //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialBump failed.  materialIndex:'%A'" materialIndex
        #if RHINO6
        let texture = mat.GetBumpTexture() 
        #else
        let texture = mat.GetTexture(DocObjects.TextureType.Bump)
        #endif
        if notNull texture then Some texture.FileName else None


    ///<summary>Modifies a material's bump bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The bump bitmap filename</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MaterialBump(materialIndex:int, filename:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        if IO.File.Exists filename then
            #if RHINO6            
            if not <| mat.SetBumpTexture(filename) then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename        
            #else
            let texture = new DocObjects.Texture()
            texture.FileName <- filename
            if not <| mat.SetTexture(texture,DocObjects.TextureType.Bump) then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename  
            #endif
        
            mat.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 
            Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename


    ///<summary>Returns a material's diffuse color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material color.</returns>
    [<Extension>]
    static member MaterialColor(materialIndex:int) : Drawing.Color = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialColor failed.  materialIndex:'%A'" materialIndex
        let rc = mat.DiffuseColor
        rc

    ///<summary>Modifies a material's diffuse color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MaterialColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialColor failed.  materialIndex:'%A' color:'%A'" materialIndex color
        mat.DiffuseColor <- color
        mat.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 
        Doc.Views.Redraw()


    ///<summary>Returns a material's environment bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current environment bitmap filename.</returns>
    [<Extension>]
    static member MaterialEnvironmentMap(materialIndex:int) : string option= //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialEnvironmentMap failed.  materialIndex:'%A'" materialIndex
        #if RHINO6
        let texture = mat.GetEnvironmentTexture()
        #else
        let texture = mat.GetTexture(DocObjects.TextureType.Emap)
        #endif
        if notNull texture then Some texture.FileName  else None

    ///<summary>Modifies a material's environment bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The environment bitmap filename</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MaterialEnvironmentMap(materialIndex:int, filename:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        if IO.File.Exists filename then
            #if RHINO6
            if not <| mat.SetEnvironmentTexture(filename) then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename            
            #else
            let texture = new DocObjects.Texture()
            texture.FileName <- filename
            if not <| mat.SetTexture(texture,DocObjects.TextureType.Emap)then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename     
            #endif
            mat.CommitChanges() |> ignore
            Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename



    ///<summary>Returns a material's user defined name.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current material name.</returns>
    [<Extension>]
    static member MaterialName(materialIndex:int) : string = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialName failed.  materialIndex:'%A'" materialIndex
        let rc = mat.Name
        rc

    ///<summary>Modifies a material's user defined name.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="name">(string) The new name</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MaterialName(materialIndex:int, name:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialName failed.  materialIndex:'%A' name:'%A'" materialIndex name
        mat.Name <- name
        mat.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 



    ///<summary>Returns a material's reflective color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material reflective color.</returns>
    [<Extension>]
    static member MaterialReflectiveColor(materialIndex:int) : Drawing.Color = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialReflectiveColor failed.  materialIndex:'%A'" materialIndex
        let rc = mat.ReflectionColor
        rc

    ///<summary>Modifies a material's reflective color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MaterialReflectiveColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialReflectiveColor failed.  materialIndex:'%A' color:'%A'" materialIndex color
        mat.ReflectionColor <- color
        mat.CommitChanges() |> ignore
        Doc.Views.Redraw()



    ///<summary>Returns a material's shine value.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(float) The current material shine value
    ///    0.0 being matte and 255.0 being glossy.</returns>
    [<Extension>]
    static member MaterialShine(materialIndex:int) : float = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialShine failed.  materialIndex:'%A'" materialIndex
        let rc = mat.Shine
        rc

    ///<summary>Modifies a material's shine value.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="shine">(float) The new shine value. A material's shine value ranges from 0.0 to 255.0, with
    ///    0.0 being matte and 255.0 being glossy</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MaterialShine(materialIndex:int, shine:float) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialShine failed.  materialIndex:'%A' shine:'%A'" materialIndex shine
        mat.Shine <- shine
        mat.CommitChanges() |> ignore
        Doc.Views.Redraw()



    ///<summary>Returns a material's texture bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current texture bitmap filename.</returns>
    [<Extension>]
    static member MaterialTexture(materialIndex:int) : string option = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTexture failed.  materialIndex:'%A'" materialIndex
        #if RHINO6
        let texture = mat.GetBitmapTexture()
        #else
        let texture = mat.GetTexture(DocObjects.TextureType.Bitmap)
        #endif
        if notNull texture then  Some texture.FileName else None

    ///<summary>Modifies a material's texture bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The texture bitmap filename</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MaterialTexture(materialIndex:int, filename:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        if IO.File.Exists filename then
            #if RHINO6
            if  not <| mat.SetBitmapTexture(filename) then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #else
            let texture = new DocObjects.Texture()
            texture.FileName <- filename
            if not <| mat.SetTexture(texture,DocObjects.TextureType.Bitmap) then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #endif
            
            mat.CommitChanges() |> ignore
            Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename


    ///<summary>Returns a material's transparency value.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(float) The current material transparency value
    ///    0.0 being opaque and 1.0 being transparent.</returns>
    [<Extension>]
    static member MaterialTransparency(materialIndex:int) : float = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTransparency failed.  materialIndex:'%A'" materialIndex
        let rc = mat.Transparency
        rc

    ///<summary>Modifies a material's transparency value.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="transparency">(float) The new transparency value. A material's transparency value ranges from 0.0 to 1.0, with
    ///    0.0 being opaque and 1.0 being transparent</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MaterialTransparency(materialIndex:int, transparency:float) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTransparency failed.  materialIndex:'%A' transparency:'%A'" materialIndex transparency
        mat.Transparency <- transparency
        mat.CommitChanges() |> ignore
        Doc.Views.Redraw()



    ///<summary>Returns a material's transparency bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current transparency bitmap filename.</returns>
    [<Extension>]
    static member MaterialTransparencyMap(materialIndex:int) : string option = //GET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTransparencyMap failed.  materialIndex:'%A'" materialIndex
        #if RHINO6
        let texture = mat.GetTransparencyTexture()
        #else
        let texture = mat.GetTexture(DocObjects.TextureType.Transparency)
        #endif
        if notNull texture then  Some texture.FileName else None


    ///<summary>Modifies a material's transparency bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The transparency bitmap filename</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MaterialTransparencyMap(materialIndex:int, filename:string) : unit = //SET
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        if IO.File.Exists filename then
            #if RHINO6
            if  not <| mat.SetTransparencyTexture(filename) then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #else
            let texture = new DocObjects.Texture()
            texture.FileName <- filename
            if not <| mat.SetTexture(texture,DocObjects.TextureType.Transparency)then RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename            
            #endif            
            mat.CommitChanges() |> ignore
            Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename



    ///<summary>Resets a material to Rhino's default material.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member ResetMaterial(materialIndex:int) : bool =
        let mat = Doc.Materials.[materialIndex]
        if mat|> isNull  then false
        else
            let rc = Doc.Materials.ResetMaterial(materialIndex)
            Doc.Views.Redraw()
            rc


