namespace Rhino.Scripting


open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.UtilMath

open Microsoft.FSharp.Core.LanguagePrimitives
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?

[<AutoOpen>] // I think it is OK to auto open this module
/// This module provides curried F# functions for easy use with pipeline operator |>
/// This module is automatically opened when Rhino.Scripting namespace is opened.
module AutoOpenCurried =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    ///<summary>Modifies the layer of an object, creates layer if not yet existing.</summary>
    ///<param name="layer">(string) Name of layer or empty string for current layer</param>
    ///<param name="objectId">(Guid) The identifier of the object</param>    
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member setLayer (layer:string) (objectId:Guid) : unit =
        RhinoScriptSyntax.ObjectLayer(objectId, layer, createLayerIfMissing=true)

    ///<summary>Modifies the layer of several objects, creates layer if not yet existing.</summary>
    ///<param name="layer">(string) Name of layer or empty string for current layer</param>
    ///<param name="objectIds">(Guid seq) The identifiers of several objects</param>    
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member setLayers (layer:string) (objectIds:seq<Guid>) : unit =
        RhinoScriptSyntax.ObjectLayer(objectIds,layer, createLayerIfMissing=true)

    
    ///<summary>Returns the full layername of an object. 
    /// parent layers are separated by <c>::</c>.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<returns>(string) The object's current layer.</returns>
    [<Extension>]
    static member getLayer (objectId:Guid) : string = 
        RhinoScriptSyntax.ObjectLayer(objectId)

    ///<summary>Returns the short layer of an object.
    ///    Without Parent Layers.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<returns>(string) The object's current layer.</returns>
    [<Extension>]
    static member getLayerShort (objectId:Guid) : string = 
        RhinoScriptSyntax.ObjectLayerShort(objectId)
        
    ///<summary>Sets the name of an object.</summary>
    ///<param name="name">(string) The new object name.</param>
    ///<param name="objectId">(Guid)Id of object</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member setName (name:string) (objectId:Guid) : unit = 
        RhinoScriptSyntax.ObjectName(objectId, name)  
        
    ///<summary>Sets the name of several objects.</summary>
    ///<param name="name">(string) The new object name.</param>
    ///<param name="objectIds">(Guid seq)Ids of several objects</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member setNames (name:string) (objectIds:seq<Guid>) : unit = 
        RhinoScriptSyntax.ObjectName(objectIds, name)  

    
    ///<summary>Returns the name of an object or "" if none given.</summary>
    ///<param name="objectId">(Guid)Id of object</param>
    ///<returns>(string) The current object name, empty string if no name given .</returns>
    [<Extension>]
    static member getName (objectId:Guid) : string = 
        RhinoScriptSyntax.ObjectName(objectId)    


    ///<summary>Sets the Color of an object.</summary>
    ///<param name="color">(Drawing.Color) The new object color.</param>
    ///<param name="objectId">(Guid)Id of object</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member setColor(color:Drawing.Color) (objectId:Guid) : unit = 
        RhinoScriptSyntax.ObjectColor(objectId, color)  
        
    ///<summary>Sets the Color of several objects.</summary>
    ///<param name="color">(Drawing.Color) The new object color.</param>
    ///<param name="objectIds">(Guid seq)Id of several objects</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member setColors(color:Drawing.Color) (objectIds:seq<Guid>) : unit = 
        RhinoScriptSyntax.ObjectColor(objectIds, color)  
    
    ///<summary>Returns the color of an object .</summary>
    ///<param name="objectId">(Guid)Id of object</param>
    ///<returns>(string) The current object color.</returns>
    [<Extension>]
    static member getColor (objectId:Guid) : Drawing.Color = 
        RhinoScriptSyntax.ObjectColor(objectId) 

    ///<summary>Sets a user text stored on an object.</summary>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) The string value to set. Cannot be empty string. use rs.DeleteUserText to delete keys</param>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member setUserText( key:string) ( value :string) (objectId:Guid) : unit = 
        RhinoScriptSyntax.SetUserText(objectId, key, value)

    ///<summary>Sets a user text stored on several objects.</summary>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) The string value to set. Cannot be empty string. use rs.DeleteUserText to delete keys</param>
    ///<param name="objectIds">(Guid seq) The identifiers of several objects</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member setUserTexts( key:string) ( value :string) (objectIds:seq<Guid>) : unit = 
        RhinoScriptSyntax.SetUserText(objectIds, key, value)
    
    ///<summary>Append a string to a possibly already existing usertext value.</summary>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) The string value to append. Cannot be empty string. use rs.DeleteUserText to delete keys</param>
    ///<param name="objectId">(Guid) The identifier of the objects</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member appendtUserText(key:string) (value :string) (objectId:Guid) : unit = 
        if String.IsNullOrWhiteSpace key then RhinoScriptingException.Raise "RhinoScriptSyntax.appendtUserText key is String.IsNullOrWhiteSpace for value  %s on %s" value (Print.guid objectId)
        if isNull value then RhinoScriptingException.Raise "RhinoScriptSyntax.appendtUserText value is null  for key %s on %s" key (Print.guid objectId)        
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let existing = obj.Attributes.GetUserString(key)
        if isNull existing then // only if a value already exist  appending a white space  or empty string is OK too
            if String.IsNullOrWhiteSpace value  then RhinoScriptingException.Raise "RhinoScriptSyntax.appendtUserText failed on %s for key '%s' but value IsNullOrWhiteSpace" (Print.guid objectId) key
            if not <| obj.Attributes.SetUserString(key, value) then RhinoScriptingException.Raise "RhinoScriptSyntax.appendtUserText failed on %s for key '%s' and value '%s'" (Print.guid objectId) key value
        else 
            if not <| obj.Attributes.SetUserString(key,  existing + value ) then RhinoScriptingException.Raise "RhinoScriptSyntax.appendtUserText failed on %s for key '%s' and value '%s'" (Print.guid objectId) key value
       
    ///<summary>Returns user text stored on an object, fails if non existing.</summary>
    ///<param name="key">(string) The key name</param>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<returns>(string) if key is specified, the associated value,fails if non existing.</returns>
    [<Extension>]
    static member getUserText( key:string) (objectId:Guid) : string = 
        RhinoScriptSyntax.GetUserText(objectId, key)

    ///<summary>Checks if the user text stored on an object matches a given string, fails if non existing.</summary>
    ///<param name="key">(string) The key name</param>
    ///<param name="valueToMatch">(string) The value to check for equality with</param>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<returns>(string) if key is specified, the associated value,fails if non existing.</returns>
    [<Extension>]
    static member isUserTextValue( key:string) (valueToMatch:string) (objectId:Guid) : bool = 
        valueToMatch = RhinoScriptSyntax.GetUserText(objectId, key)

        
    ///<summary>Checks if a User Text key is stored on an object.</summary>
    ///<param name="key">(string) The key name</param>   
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<returns>(bool) if key exist true.</returns>
    [<Extension>]
    static member hasUserText( key:string) (objectId:Guid) : bool = 
        RhinoScriptSyntax.HasUserText(objectId, key)
        
    ///<summary>Returns user text stored on an object, returns Option.None if non existing.</summary>
    ///<param name="key">(string) The key name</param>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<returns>(string Option) if key is specified, Some(value) else None .</returns>
    [<Extension>]
    static member tryGetUserText( key:string) (objectId:Guid) : string option= 
        RhinoScriptSyntax.TryGetUserText(objectId, key)

    ///<summary>Copies all user text keys and values from  one object to another
    ///from both Geometry and Object.Attributes. Existing values are overwitten.</summary>
    ///<param name="sourceId">(Guid) The object to take all keys from </param>
    ///<param name="targetId">(Guid) The object to write  all keys to </param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member matchAllUserText (sourceId:Guid) (targetId:Guid) : unit= 
        let sc = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
        let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
        let usg = sc.Geometry.GetUserStrings()
        for  i = 0 to usg.Count-1 do 
            let key = usg.GetKey(i)
            if not <|de.Geometry.SetUserString(key,sc.Geometry.GetUserString(key)) then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.matchAllUserText: Geometry failed to set key '%s' from %s on %s" key (Print.guid sourceId) (Print.guid targetId)
        let usa = sc.Attributes.GetUserStrings()
        for  i = 0 to usa.Count-1 do 
            let key = usa.GetKey(i)
            if not <|de.Attributes.SetUserString(key,sc.Attributes.GetUserString(key))then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.matchAllUserText: Attributes failed to set key '%s' from %s on %s" key (Print.guid sourceId) (Print.guid targetId)
        
    ///<summary>Copies the value for a given user text key from a scource object to a target object.</summary>
    ///<param name="sourceId">(Guid) The object to take all keys from </param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="targetId">(Guid) The object to write  all keys to </param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member matchUserText (sourceId:Guid) ( key:string) (targetId:Guid) : unit= 
        let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
        let v = RhinoScriptSyntax.GetUserText(sourceId,key)
        if not <| de.Attributes.SetUserString(key,v) then RhinoScriptingException.Raise "RhinoScriptSyntax.matchUserText: failed to set key '%s' to '%s' on %s" key v (Print.guid targetId) 
        
    ///<summary>Copies the object name from a scource object to a target object.</summary>
    ///<param name="sourceId">(Guid) The object to take the name from </param>
    ///<param name="targetId">(Guid) The object to write the name to </param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member matchName (sourceId:Guid) (targetId:Guid) : unit = 
        let sc = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
        let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
        let n = sc.Attributes.Name 
        if isNull n then RhinoScriptingException.Raise "RhinoScriptSyntax.matchName: scource object %s has no name. Targets name: '%s'" (Print.guid sourceId) de.Attributes.Name
        de.Attributes.Name <- n
        if not <| de.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.matchName failed from %s on %s" (Print.guid sourceId) (Print.guid targetId)

    ///<summary>Puts target object on the same Layer as a scource object .</summary>
    ///<param name="sourceId">(Guid) The object to take the layer from </param>
    ///<param name="targetId">(Guid) The object to change the layer</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member matchLayer (sourceId:Guid) (targetId:Guid) : unit = 
        let sc = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
        let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
        de.Attributes.LayerIndex <- sc.Attributes.LayerIndex 
        if not <| de.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.matchLayer failed from %s on %s" (Print.guid sourceId) (Print.guid targetId)

    
    ///<summary>Matches all properties( layer, name, user text, ....) from a scource object to a target object by duplicating attributes. 
    /// and copying user strings on geometry. .</summary>
    ///<param name="sourceId">(Guid) The object to take all keys from </param>
    ///<param name="targetId">(Guid) The object to write  all keys to </param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member matchAllProperties (sourceId:Guid) (targetId:Guid) : unit = 
        let sc = RhinoScriptSyntax.CoerceRhinoObject(sourceId)
        let de = RhinoScriptSyntax.CoerceRhinoObject(targetId)
        de.Attributes <- sc.Attributes.Duplicate()
        if not <| de.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.matchAllProperties failed from %s on %s" (Print.guid sourceId) (Print.guid targetId)
        let usg = sc.Geometry.GetUserStrings()
        for  i = 0 to usg.Count-1 do 
            let key = usg.GetKey(i)
            if not <|de.Geometry.SetUserString(key,sc.Geometry.GetUserString(key)) then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.matchAllProperties: Geometry failed to set key '%s' from %s on %s" key (Print.guid sourceId) (Print.guid targetId)

    ///<summary>Draws any Geometry object to a given or current layer.</summary>
    ///<param name="layer">(string) Name of an layer or empty string for current layer</param>
    ///<param name="geo">(GeometryBase) Geometry</param>    
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member draw (layer:string) (geo:'AnyRhinoGeometry) : unit =  
        RhinoScriptSyntax.Add(geo) |> RhinoScriptSyntax.setLayer layer

        
    ///<summary>Moves, scales, or rotates an object given a 4x4 transformation matrix.
    ///    The matrix acts on the left. To transform Geometry objects instead of DocObjects or Guids use their .Transform(xForm) member.</summary>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers)</param>
    ///<param name="objectId">(Guid) The identifier of the object</param> 
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member transform (matrix:Transform) (objectId:Guid) : Guid =  
        RhinoScriptSyntax.TransformObject(objectId, matrix, copy=false)       
        // TODO test to ensure GUID is the same ?

    ///<summary>Scales a single object. Uniform scale transformation. Scaling is based on the WorldXY Plane.</summary>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float) One numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(unit) void, nothing.</returns>    
    [<Extension>]
    static member scale(origin:Point3d) (scale:float) (objectId:Guid) : unit = 
        let mutable plane = Plane.WorldXY
        plane.Origin <- origin
        let xf = Transform.Scale(plane, scale, scale, scale)
        let res = State.Doc.Objects.Transform(objectId, xf, deleteOriginal=true)
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.scale failed. objectId:'%s' origin:'%s' scale:'%g'" (Print.guid objectId) origin.ToNiceString scale
       

    ///<summary>Moves a single object.</summary>
    ///<param name="translation">(Vector3d) Vector3d</param>
    ///<param name="objectId">(Guid) The identifier of an object to move</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member move (translation:Vector3d)  (objectId:Guid): unit = 
        let xf = Transform.Translation(translation)
        let res = State.Doc.Objects.Transform(objectId, xf, deleteOriginal=true) // TODO test to ensure GUID is the same ?
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.move to from objectId:'%s' translation:'%A'" (Print.guid objectId) translation

    ///<summary>Moves a single object in X Direction.</summary>
    ///<param name="translationX">(float) movement in X direction</param>
    ///<param name="objectId">(Guid) The identifier of an object to move</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member moveX (translationX:float)  (objectId:Guid): unit = 
        let xf = Transform.Translation(Vector3d(translationX, 0.0, 0.0 ))
        let res = State.Doc.Objects.Transform(objectId, xf, deleteOriginal=true) // TODO test to ensure GUID is the same ?
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.moveX to from objectId:'%s' translation:'%A'" (Print.guid objectId) translationX

    ///<summary>Moves a single object in Y Direction.</summary>
    ///<param name="translationY">(float) movement in Y direction</param>
    ///<param name="objectId">(Guid) The identifier of an object to move</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member moveY (translationY:float)  (objectId:Guid): unit = 
        let xf = Transform.Translation(Vector3d(0.0, translationY, 0.0))
        let res = State.Doc.Objects.Transform(objectId, xf, deleteOriginal=true) // TODO test to ensure GUID is the same ?
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.moveY to from objectId:'%s' translation:'%A'" (Print.guid objectId) translationY

    ///<summary>Moves a single object in Z Direction.</summary>
    ///<param name="translationZ">(float) movement in Z direction</param>
    ///<param name="objectId">(Guid) The identifier of an object to move</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member moveZ (translationZ:float)  (objectId:Guid): unit = 
        let xf = Transform.Translation(Vector3d(0.0, 0.0, translationZ))
        let res = State.Doc.Objects.Transform(objectId, xf, deleteOriginal=true) // TODO test to ensure GUID is the same ?
        if res = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.moveZ to from objectId:'%s' translation:'%A'" (Print.guid objectId) translationZ


    ///<summary>Moves a Geometry.</summary>
    ///<param name="translation">(Vector3d) Vector3d</param>
    ///<param name="geo">(GeometryBase) The Geometry to move</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member moveGeo (translation:Vector3d)  (geo:GeometryBase): unit = 
        if not <|  geo.Translate translation then RhinoScriptingException.Raise "RhinoScriptSyntax.moveGeo to from geo:'%A' translation:'%A'"  geo translation
        
    ///<summary>Moves a Geometry in X Direction.</summary>
    ///<param name="translationX">(float) movement in X direction</param>
    ///<param name="geo">(GeometryBase) The Geometry to move</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member moveGeoX (translationX:float)  (geo:GeometryBase): unit = 
        if not <| geo.Translate (Vector3d(translationX, 0.0, 0.0 )) then RhinoScriptingException.Raise "RhinoScriptSyntax.moveGeoX to from geo:'%A' translation:'%f'"  geo translationX

    ///<summary>Moves a Geometry in Y Direction.</summary>
    ///<param name="translationY">(float) movement in Y direction</param>
    ///<param name="geo">(GeometryBase) The Geometry to move</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member moveGeoY (translationY:float)  (geo:GeometryBase): unit = 
        if not <| geo.Translate (Vector3d(0.0, translationY, 0.0)) then RhinoScriptingException.Raise "RhinoScriptSyntax.moveGeoY to from geo:'%A' translation:'%f'"  geo translationY

    ///<summary>Moves a Geometry in Z Direction.</summary>
    ///<param name="translationZ">(float) movement in Z direction</param>
    ///<param name="geo">(GeometryBase) The Geometry to move</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member moveGeoZ (translationZ:float)  (geo:GeometryBase): unit =
        if not <| geo.Translate (Vector3d(0.0, 0.0, translationZ)) then RhinoScriptingException.Raise "RhinoScriptSyntax.moveGeoZ to from geo:'%A' translation:'%f'"  geo translationZ
    
