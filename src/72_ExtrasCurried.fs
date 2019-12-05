namespace Rhino.Scripting


open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.UtilMath
open Rhino.Scripting.ActiceDocument
open Microsoft.FSharp.Core.LanguagePrimitives
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?

[<AutoOpen>]
module ExtensionsCurve =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    [<Extension>]
    ///<summary>Modifies the layer of an object</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="objectId">(Guid) The identifier of the object</param>    
    ///<returns>(unit) void, nothing</returns>
    static member setObjectLayer( layer:string) (objectId:Guid) : unit = 
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        
        let layer = 
            if i <> RhinoMath.UnsetIntIndex then 
                Doc.Layers.[i]
            else
                let names = layer.Split([| "::"|],StringSplitOptions.RemoveEmptyEntries)
                let mutable lastparentindex = -1
                let mutable lastparent:DocObjects.Layer = null
                for idx, name in Seq.indexed(names) do
                  let layer = DocObjects.Layer.GetDefaultLayerProperties()
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

                
        let index = layer.Index
        obj.Attributes.LayerIndex <- index
        if not <| obj.CommitChanges() then failwithf "Set ObjectLayer failed for '%A' and '%A'"  layer objectId
        Doc.Views.Redraw()