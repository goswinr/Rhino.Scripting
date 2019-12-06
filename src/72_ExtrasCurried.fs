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
module ExtrasCurried =

    ///RhinoScriptSyntax.Print
    let print x = RhinoScriptSyntax.Print x //shadows FsEx.TypeExtensionsObject.print

    ///RhinoScriptSyntax.PrintFull
    let printFull x = RhinoScriptSyntax.PrintFull x //shadows FsEx.TypeExtensionsObject.printFull
    

    //[<Extension>] //Error 3246
    type RhinoScriptSyntax with

        [<Extension>]
        ///<summary>Modifies the layer of an object, creates layer if not yet existing</summary>
        ///<param name="layer">(string) Name of an existing layer</param>
        ///<param name="objectId">(Guid) The identifier of the object</param>    
        ///<returns>(unit) void, nothing</returns>
        static member setLayer( layer:string) (objectId:Guid) : unit =             
            let layerIndex =
                let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
                if i <> RhinoMath.UnsetIntIndex then 
                    i
                else
                    let names = layer.Split([| "::"|],StringSplitOptions.RemoveEmptyEntries)
                    let mutable lastparentindex =  -1
                    let mutable lastparent      =  null : DocObjects.Layer
                    for idx, name in Seq.indexed(names) do
                        let layer = DocObjects.Layer.GetDefaultLayerProperties()
                        if idx > 0 && lastparentindex <> -1 then
                            lastparent <- Doc.Layers.[lastparentindex]
                        if notNull lastparent then
                            layer.ParentLayerId <- lastparent.Id
                        layer.Name <- name
                        layer.Color <- Color.randomColorForRhino()
                        layer.IsVisible <- true
                        layer.IsLocked <- false
                        lastparentindex <- Doc.Layers.Add(layer)                        
                        if lastparentindex = -1 then
                            let mutable fullpath = layer.Name
                            if notNull lastparent then
                                fullpath <- lastparent.FullPath + "::" + fullpath
                            lastparentindex <- Doc.Layers.FindByFullPath(fullpath, RhinoMath.UnsetIntIndex)
                    lastparentindex

            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)            
            obj.Attributes.LayerIndex <- layerIndex
            if not <| obj.CommitChanges() then failwithf "setLayer failed for '%A' and '%A'"  layer objectId
            Doc.Views.Redraw()

        static member setName( name:string) (objectId:Guid) : unit = RhinoScriptSyntax.ObjectName(objectId,name)    
    
        static member setUserText( key:string) ( value :string) (objectId:Guid) : unit = RhinoScriptSyntax.SetUserText(objectId,key,value)