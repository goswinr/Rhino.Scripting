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

    ///RhinoScriptSyntax.Print (shadowing in print from FsEx)
    let print x = RhinoScriptSyntax.Print x //shadows FsEx.TypeExtensionsObject.print

    ///RhinoScriptSyntax.PrintFull (shadowing in printFull from FsEx)
    let printFull x = RhinoScriptSyntax.PrintFull x //shadows FsEx.TypeExtensionsObject.printFull

    //[<Extension>] //Error 3246
    type RhinoScriptSyntax with

        [<Extension>]
        ///<summary>Modifies the layer of an object, creates layer if not yet existing</summary>
        ///<param name="layer">(string) Name of layer or empty string for current layer</param>
        ///<param name="objectId">(Guid) The identifier of the object</param>    
        ///<returns>(unit) void, nothing</returns>
        static member setLayer( layer:string) (objectId:Guid) : unit = 
            Debug.setLayer layer objectId  
        
        [<Extension>]
        static member setName( name:string) (objectId:Guid) : unit = 
            RhinoScriptSyntax.ObjectName(objectId, name)    
        
        [<Extension>]
        static member setUserText( key:string) ( value :string) (objectId:Guid) : unit = 
            RhinoScriptSyntax.SetUserText(objectId, key, value)
        
        [<Extension>]
        static member getUserText( key:string) (objectId:Guid) : string = 
            RhinoScriptSyntax.GetUserText(objectId, key)
        
        [<Extension>]
        static member tryGetUserText( key:string) (objectId:Guid) : string option= 
            RhinoScriptSyntax.TryGetUserText(objectId, key)

        [<Extension>]
        static member matchAllUserText (sourceId:Guid) (targetId:Guid) : unit= 
            let sc = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
            let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
            let usg = sc.Geometry.GetUserStrings()
            for  i = 0 to usg.Count-1 do 
                let key = usg.GetKey(i)
                if not <|de.Geometry.SetUserString(key,sc.Geometry.GetUserString(key)) then 
                    failwithf "matchAllUserText: Geometry failed to set key '%s' on %A from %A" key  targetId sourceId
            let usa = sc.Attributes.GetUserStrings()
            for  i = 0 to usa.Count-1 do 
                let key = usa.GetKey(i)
                if not <|de.Attributes.SetUserString(key,sc.Attributes.GetUserString(key))then 
                    failwithf "matchAllUserText: Attributes failed to set key '%s' on %A from %A" key targetId sourceId
        
        [<Extension>]
        static member matchUserText (sourceId:Guid) ( key:string) (targetId:Guid) : unit= 
            let sc = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
            let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
            let v = sc.Attributes.GetUserString(key)
            if isNull v || v="" then failwithf "matchUserText: key '%s' not found on %A" key sourceId
            if not <| de.Attributes.SetUserString(key,v) then failwithf "matchUserText: failed to set key '%s' to '%s' on %A" key v targetId
        
        [<Extension>]
        /// copies the object name
        static member matchName (sourceId:Guid) (targetId:Guid) : unit = 
            let sc = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
            let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
            let n = sc.Attributes.Name 
            if isNull n then failwithf "matchUserText: scource object '%A' has no name. Targets name: '%A'"  sourceId de.Attributes.Name
            de.Attributes.Name <- n
            if not <| de.CommitChanges() then failwithf "matchUserText failed for '%A' and '%A'"  sourceId targetId
        
        [<Extension>]
        ///<summary>Draws any Geometry object to a given or current layer</summary>
        ///<param name="layer">(string) Name of an layer or empty string for current layer</param>
        ///<param name="geo">(GeometryBase) Geometry</param>    
        ///<returns>(unit) void, nothing</returns>
        static member draw (layer:string) (geo:GeometryBase) : unit =  
            Doc.Objects.Add(geo) |> RhinoScriptSyntax.setLayer layer
        
        [<Extension>]
        ///<summary>Draws a Line to a given or current layer</summary>
        ///<param name="layer">(string) Name of an layer or empty string for current layer</param>
        ///<param name="line">(Line) Geometry</param>    
        ///<returns>(unit) void, nothing</returns>
        static member drawLine (layer:string) (line:Line) : unit =  
            Doc.Objects.AddLine(line) |> RhinoScriptSyntax.setLayer layer  
        
        [<Extension>]
        ///<summary>Draws a Point to a given or current layer</summary>
        ///<param name="layer">(string) Name of an layer or empty string for current layer</param>
        ///<param name="pt">(Point3d) Pont</param>    
        ///<returns>(unit) void, nothing</returns>
        static member drawPoint (layer:string) (pt:Point3d) : unit =  
            Doc.Objects.AddPoint(pt) |> RhinoScriptSyntax.setLayer layer  