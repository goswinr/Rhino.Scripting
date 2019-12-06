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
        static member setLayer( layer:string) (objectId:Guid) : unit = //TODO optimise, used a lot
            if not (RhinoScriptSyntax.IsLayer(layer)) then 
                RhinoScriptSyntax.AddLayer(layer) |> ignore
            RhinoScriptSyntax.ObjectLayer(objectId,layer)

        static member setName( name:string) (objectId:Guid) : unit = RhinoScriptSyntax.ObjectName(objectId,name)    
    
        static member setUserText( key:string) ( value :string) (objectId:Guid) : unit = RhinoScriptSyntax.SetUserText(objectId,key,value)