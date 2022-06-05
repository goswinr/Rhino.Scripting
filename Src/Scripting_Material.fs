
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenMaterial =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    ///<summary>Add material to a layer and returns the new material's index. If the
    ///    layer already has a material, then the layer's current material index is
    ///    returned.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(int) Material index of the layer.</returns>
    static member AddMaterialToLayer(layer:string) : int = 
        let layer = Scripting.CoerceLayer(layer)
        if layer.RenderMaterialIndex> -1 then layer.RenderMaterialIndex
        else
            let materialindex = State.Doc.Materials.Add()
            layer.RenderMaterialIndex <- materialindex
            State.Doc.Views.Redraw()
            materialindex

    ///<summary>Adds material to an object and returns the new material's index. If the
    ///    object already has a material, the object's current material index is returned.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(int) material index of the object.</returns>
    static member AddMaterialToObject(objectId:Guid) : int = 
        let rhinoobject = Scripting.CoerceRhinoObject(objectId)
        let mutable attr = rhinoobject.Attributes
        if attr.MaterialSource <> DocObjects.ObjectMaterialSource.MaterialFromObject then
            attr.MaterialSource <- DocObjects.ObjectMaterialSource.MaterialFromObject
            State.Doc.Objects.ModifyAttributes(rhinoobject, attr, true)|> ignore
            attr <- rhinoobject.Attributes
        let mutable materialindex = attr.MaterialIndex
        if materialindex> -1 then materialindex
        else
            materialindex <- State.Doc.Materials.Add()
            attr.MaterialIndex <- materialindex
            State.Doc.Objects.ModifyAttributes(rhinoobject, attr, true)|> ignore
            materialindex


    ///<summary>Copies definition of a source material to a destination material.</summary>
    ///<param name="sourceIndex">(int) Source index of materials to copy</param>
    ///<param name="destinationIndex">(int) Destination index materials to copy</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member CopyMaterial(sourceIndex:int, destinationIndex:int) : bool = 
        if sourceIndex = destinationIndex then true // originally false
        else
            let source = State.Doc.Materials.[sourceIndex]
            if source|> isNull  then false
            else
                let rc = State.Doc.Materials.Modify(source, destinationIndex, true)
                if rc then State.Doc.Views.Redraw()
                rc


    ///<summary>Verifies a material is a copy of Rhino's built-in "default" material.
    ///    The default material is used by objects and layers that have not been
    ///    assigned a material.</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsMaterialDefault(materialIndex:int) : bool = 
        let mat = State.Doc.Materials.[materialIndex]
        notNull mat && mat.IsDefaultMaterial


    ///<summary>Verifies a material is referenced from another file.</summary>
    ///<param name="materialIndex">(int) The zero-based material index</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsMaterialReference(materialIndex:int) : bool = 
        let mat = State.Doc.Materials.[materialIndex]
        notNull mat && mat.IsReference


    ///<summary>Copies the material definition from one material to one or more objects.</summary>
    ///<param name="source">(Guid) Source material index -or- identifier of the source object.
    ///    The object must have a material assigned</param>
    ///<param name="destination">(Guid seq) Id of the destination object</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MatchMaterial(source:Guid, destination:Guid seq) : unit = 
        let rhobj = Scripting.CoerceRhinoObject(source)
        let source = rhobj.Attributes.MaterialIndex
        let mat = State.Doc.Materials.[source]
        if isNull mat then RhinoScriptingException.Raise "Rhino.Scripting.MatchMaterial failed.  source:'%A' destination:'%A'" source destination

        for objectId in destination do
            let rhobj = State.Doc.Objects.FindId(objectId)
            if notNull rhobj then
                rhobj.Attributes.MaterialIndex <- source
                rhobj.Attributes.MaterialSource <- DocObjects.ObjectMaterialSource.MaterialFromObject
                rhobj.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.MatchMaterial : CommitChanges failed"
        State.Doc.Views.Redraw()



    ///<summary>Returns a material's bump bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current bump bitmap filename.</returns>
    static member MaterialBump(materialIndex:int) : string option= //GET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialBump failed.  materialIndex:'%A'" materialIndex
        #if RHINO7
        let texture = mat.GetTexture(DocObjects.TextureType.Bump)
        #else
        let texture = mat.GetBumpTexture()
        #endif
        if notNull texture then Some texture.FileName else None


    ///<summary>Modifies a material's bump bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The bump bitmap filename</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MaterialBump(materialIndex:int, filename:string) : unit = //SET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        if IO.File.Exists filename then
            #if RHINO7
            let texture = new DocObjects.Texture()
            texture.FileName <- filename
            if not <| mat.SetTexture(texture,DocObjects.TextureType.Bump) then RhinoScriptingException.Raise "Rhino.Scripting.MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #else
            if not <| mat.SetBumpTexture(filename) then RhinoScriptingException.Raise "Rhino.Scripting.MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #endif

            mat.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.MaterialBump : CommitChanges failed"
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.MaterialBump failed.  materialIndex:'%A' filename:'%A'" materialIndex filename


    ///<summary>Returns a material's diffuse color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material color.</returns>
    static member MaterialColor(materialIndex:int) : Drawing.Color = //GET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialColor failed.  materialIndex:'%A'" materialIndex
        let rc = mat.DiffuseColor
        rc

    ///<summary>Modifies a material's diffuse color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MaterialColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialColor failed.  materialIndex:'%A' color:'%A'" materialIndex color
        mat.DiffuseColor <- color
        mat.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.MaterialColor : CommitChanges failed"
        State.Doc.Views.Redraw()


    ///<summary>Returns a material's environment bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current environment bitmap filename.</returns>
    static member MaterialEnvironmentMap(materialIndex:int) : string option= //GET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialEnvironmentMap failed.  materialIndex:'%A'" materialIndex
        #if RHINO7
        let texture = mat.GetTexture(DocObjects.TextureType.Emap)
        #else
        let texture = mat.GetEnvironmentTexture()
        #endif
        if notNull texture then Some texture.FileName  else None

    ///<summary>Modifies a material's environment bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The environment bitmap filename</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MaterialEnvironmentMap(materialIndex:int, filename:string) : unit = //SET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        if IO.File.Exists filename then
            #if RHINO7
            let texture = new DocObjects.Texture()
            texture.FileName <- filename
            if not <| mat.SetTexture(texture,DocObjects.TextureType.Emap)then RhinoScriptingException.Raise "Rhino.Scripting.MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #else
            if not <| mat.SetEnvironmentTexture(filename) then RhinoScriptingException.Raise "Rhino.Scripting.MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #endif
            mat.CommitChanges() |> ignore
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.MaterialEnvironmentMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename



    ///<summary>Returns a material's user defined name.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string) The current material name.</returns>
    static member MaterialName(materialIndex:int) : string = //GET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialName failed.  materialIndex:'%A'" materialIndex
        let rc = mat.Name
        rc

    ///<summary>Modifies a material's user defined name.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="name">(string) The new name</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MaterialName(materialIndex:int, name:string) : unit = //SET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialName failed.  materialIndex:'%A' name:'%A'" materialIndex name
        mat.Name <- name
        mat.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.MaterialName : CommitChanges failed"



    ///<summary>Returns a material's reflective color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(Drawing.Color) The current material reflective color.</returns>
    static member MaterialReflectiveColor(materialIndex:int) : Drawing.Color = //GET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialReflectiveColor failed.  materialIndex:'%A'" materialIndex
        let rc = mat.ReflectionColor
        rc

    ///<summary>Modifies a material's reflective color.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MaterialReflectiveColor(materialIndex:int, color:Drawing.Color) : unit = //SET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialReflectiveColor failed.  materialIndex:'%A' color:'%A'" materialIndex color
        mat.ReflectionColor <- color
        mat.CommitChanges() |> ignore
        State.Doc.Views.Redraw()



    ///<summary>Returns a material's shine value.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(float) The current material shine value
    ///    0.0 being matte and 255.0 being glossy.</returns>
    static member MaterialShine(materialIndex:int) : float = //GET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialShine failed.  materialIndex:'%A'" materialIndex
        let rc = mat.Shine
        rc

    ///<summary>Modifies a material's shine value.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="shine">(float) The new shine value. A material's shine value ranges from 0.0 to 255.0, with
    ///    0.0 being matte and 255.0 being glossy</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MaterialShine(materialIndex:int, shine:float) : unit = //SET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialShine failed.  materialIndex:'%A' shine:'%A'" materialIndex shine
        mat.Shine <- shine
        mat.CommitChanges() |> ignore
        State.Doc.Views.Redraw()



    ///<summary>Returns a material's texture bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current texture bitmap filename.</returns>
    static member MaterialTexture(materialIndex:int) : string option = //GET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTexture failed.  materialIndex:'%A'" materialIndex
        #if RHINO7
        let texture = mat.GetTexture(DocObjects.TextureType.Bitmap)
        #else
        let texture = mat.GetBitmapTexture()
        #endif
        if notNull texture then  Some texture.FileName else None

    ///<summary>Modifies a material's texture bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The texture bitmap filename</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MaterialTexture(materialIndex:int, filename:string) : unit = //SET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        if IO.File.Exists filename then
            #if RHINO7
            let texture = new DocObjects.Texture()
            texture.FileName <- filename
            if not <| mat.SetTexture(texture,DocObjects.TextureType.Bitmap) then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #else
            if  not <| mat.SetBitmapTexture(filename) then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #endif

            mat.CommitChanges() |> ignore
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.MaterialTexture failed.  materialIndex:'%A' filename:'%A'" materialIndex filename


    ///<summary>Returns a material's transparency value.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(float) The current material transparency value
    ///    0.0 being opaque and 1.0 being transparent.</returns>
    static member MaterialTransparency(materialIndex:int) : float = //GET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTransparency failed.  materialIndex:'%A'" materialIndex
        let rc = mat.Transparency
        rc

    ///<summary>Modifies a material's transparency value.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="transparency">(float) The new transparency value. A material's transparency value ranges from 0.0 to 1.0, with
    ///    0.0 being opaque and 1.0 being transparent</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MaterialTransparency(materialIndex:int, transparency:float) : unit = //SET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTransparency failed.  materialIndex:'%A' transparency:'%A'" materialIndex transparency
        mat.Transparency <- transparency
        mat.CommitChanges() |> ignore
        State.Doc.Views.Redraw()



    ///<summary>Returns a material's transparency bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(string option) The current transparency bitmap filename.</returns>
    static member MaterialTransparencyMap(materialIndex:int) : string option = //GET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTransparencyMap failed.  materialIndex:'%A'" materialIndex
        #if RHINO7
        let texture = mat.GetTexture(DocObjects.TextureType.Transparency)
        #else
        let texture = mat.GetTransparencyTexture()
        #endif
        if notNull texture then  Some texture.FileName else None


    ///<summary>Modifies a material's transparency bitmap filename.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<param name="filename">(string) The transparency bitmap filename</param>
    ///<returns>(unit) void, nothing.</returns>
    static member MaterialTransparencyMap(materialIndex:int, filename:string) : unit = //SET
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
        if IO.File.Exists filename then
            #if RHINO7
            let texture = new DocObjects.Texture()
            texture.FileName <- filename
            if not <| mat.SetTexture(texture,DocObjects.TextureType.Transparency)then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #else
            if  not <| mat.SetTransparencyTexture(filename) then RhinoScriptingException.Raise "Rhino.Scripting.MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename
            #endif
            mat.CommitChanges() |> ignore
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.MaterialTransparencyMap failed.  materialIndex:'%A' filename:'%A'" materialIndex filename



    ///<summary>Resets a material to Rhino's default material.</summary>
    ///<param name="materialIndex">(int) Zero based material index</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member ResetMaterial(materialIndex:int) : bool = 
        let mat = State.Doc.Materials.[materialIndex]
        if mat|> isNull  then false
        else
            let rc = State.Doc.Materials.ResetMaterial(materialIndex)
            State.Doc.Views.Redraw()
            rc



