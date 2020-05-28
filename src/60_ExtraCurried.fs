namespace Rhino.Scripting


open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.UtilMath
open Rhino.Scripting.ActiceDocument
open Microsoft.FSharp.Core.LanguagePrimitives
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?

[<AutoOpen>] // I think it is OK to auto open this module
/// This module provides curried F# functions for easy use with pipeline operator |>
/// This module is automatically opened when Rhino.Scripting Namspace is opened.
module ExtraCurried =

  ///same as RhinoScriptSyntax.Print (shadows print from FsEx)
  let print x = RhinoScriptSyntax.Print x 

  /// prints two values separated by a space using FsEx.NiceString.toNiceString
  ///(shadows print2 from FsEx)
  let print2 x y = RhinoScriptSyntax.Print (sprintf "%s %s" (NiceString.toNiceString x) (NiceString.toNiceString y))
    
  /// prints three values separated by a space using FsEx.NiceString.toNiceString
  ///(shadows print3 from FsEx)
  let print3 x y z = RhinoScriptSyntax.Print (sprintf "%s %s %s" (NiceString.toNiceString x) (NiceString.toNiceString y) (NiceString.toNiceString z) )

  /// prints four values separated by a space using FsEx.NiceString.toNiceString
  ///(shadows print4 from FsEx)
  let print4 w x y z = RhinoScriptSyntax.Print (sprintf "%s %s %s %s" (NiceString.toNiceString w) (NiceString.toNiceString x) (NiceString.toNiceString y) (NiceString.toNiceString z) )
  
  ///RhinoScriptSyntax.PrintFull (shadows printFull from FsEx)
  let printFull x = RhinoScriptSyntax.PrintFull x //shadows FsEx.TypeExtensionsObject.printFull

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    [<Extension>]
    ///<summary>Modifies the layer of an object, creates layer if not yet existing</summary>
    ///<param name="layer">(string) Name of layer or empty string for current layer</param>
    ///<param name="objectId">(Guid) The identifier of the object</param>    
    ///<returns>(unit) void, nothing</returns>
    static member setLayer( layer:string) (objectId:Guid) : unit = 
        RhinoScriptSyntax.ObjectLayer(objectId,layer,true)
        
    [<Extension>]
    ///<summary>Sets the name of an object</summary>
    ///<param name="name">(string) The new object name.</param>
    ///<param name="objectId">(Guid)Id of object</param>
    ///<returns>(unit) void, nothing</returns>
    static member setName( name:string) (objectId:Guid) : unit = 
        RhinoScriptSyntax.ObjectName(objectId, name)    
        
    [<Extension>]
    ///<summary>Sets a user text stored on an object</summary>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) The string value to set. Cannot be empty string. use rs.DeleteUserText to delete keys</param>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(unit) void, nothing</returns>
    static member setUserText( key:string) ( value :string) (objectId:Guid) : unit = 
        RhinoScriptSyntax.SetUserText(objectId, key, value)
        
    [<Extension>]
    ///<summary>Returns user text stored on an object, fails if non existing</summary>
    ///<param name="key">(string) The key name</param>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<returns>(string) if key is specified, the associated value,fails if non existing</returns>
    static member getUserText( key:string) (objectId:Guid) : string = 
        RhinoScriptSyntax.GetUserText(objectId, key)
        
    [<Extension>]
    ///<summary>Checks if a User Text key is stored on an object</summary>
    ///<param name="key">(string) The key name</param>   
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<returns>(bool) if key exist true</returns>
    static member hasUserText( key:string) (objectId:Guid) : bool = 
        RhinoScriptSyntax.HasUserText(objectId, key)
        
    [<Extension>]
    ///<summary>Returns user text stored on an object, returns Option.None if non existing</summary>
    ///<param name="key">(string) The key name</param>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<returns>(string Option) if key is specified, Some(value) else None </returns>
    static member tryGetUserText( key:string) (objectId:Guid) : string option= 
        RhinoScriptSyntax.TryGetUserText(objectId, key)

    [<Extension>]
    ///<summary>Copies all user text keys and values from  one object to another
    ///from both fGeometry and from Attributes. Existing values are overwitten.</summary>
    ///<param name="sourceId">(Guid) The object to take all keys from </param>
    ///<param name="targetId">(Guid) The object to write  all keys to </param>
    ///<returns>(unit) void, nothing</returns>
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
        
    ///<summary>Copies the value for a given user text key from a scource object to a target object</summary>
    ///<param name="sourceId">(Guid) The object to take all keys from </param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="targetId">(Guid) The object to write  all keys to </param>
    ///<returns>(unit) void, nothing</returns>
    [<Extension>]
    static member matchUserText (sourceId:Guid) ( key:string) (targetId:Guid) : unit= 
        let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
        let v = RhinoScriptSyntax.GetUserText(sourceId,key)
        if not <| de.Attributes.SetUserString(key,v) then failwithf "matchUserText: failed to set key '%s' to '%s' on %A" key v targetId
        
    [<Extension>]
    ///<summary>Copies the object name from a scource object to a target object</summary>
    ///<param name="sourceId">(Guid) The object to take all keys from </param>
    ///<param name="targetId">(Guid) The object to write  all keys to </param>
    ///<returns>(unit) void, nothing</returns>
    static member matchName (sourceId:Guid) (targetId:Guid) : unit = 
        let sc = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
        let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
        let n = sc.Attributes.Name 
        if isNull n then failwithf "matchUserText: scource object '%A' has no name. Targets name: '%A'"  sourceId de.Attributes.Name
        de.Attributes.Name <- n
        if not <| de.CommitChanges() then failwithf "matchUserText failed for '%A' and '%A'"  sourceId targetId
    
    [<Extension>]
    ///<summary>Matches all properties from a scource object to a target object by duplicating attributes. 
    /// and copying user strings on geometry. </summary>
    ///<param name="sourceId">(Guid) The object to take all keys from </param>
    ///<param name="targetId">(Guid) The object to write  all keys to </param>
    ///<returns>(unit) void, nothing</returns>
    static member matchAllProperties (sourceId:Guid) (targetId:Guid) : unit = 
        let sc = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
        let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
        de.Attributes <- sc.Attributes.Duplicate()
        if not <| de.CommitChanges() then failwithf "matchAllProperties failed for '%A' and '%A'"  sourceId targetId
        let usg = sc.Geometry.GetUserStrings()
        for  i = 0 to usg.Count-1 do 
            let key = usg.GetKey(i)
            if not <|de.Geometry.SetUserString(key,sc.Geometry.GetUserString(key)) then 
                failwithf "matchAllProperties: Geometry failed to set key '%s' on %A from %A" key  targetId sourceId

    [<Extension>]
    ///<summary>Draws any Geometry object to a given or current layer</summary>
    ///<param name="layer">(string) Name of an layer or empty string for current layer</param>
    ///<param name="geo">(GeometryBase) Geometry</param>    
    ///<returns>(unit) void, nothing</returns>
    static member draw (layer:string) (geo:'AnyRhinoGeometry) : unit =  
        RhinoScriptSyntax.Add(geo) |> RhinoScriptSyntax.setLayer layer
        