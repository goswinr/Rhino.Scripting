namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsLight =
  type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Adds a new directional light object to the document</summary>
    ///<param name="startPoint">(Point3d) Starting point of the light</param>
    ///<param name="endePoint">(Point3d) Ending point and direction of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddDirectionalLight(startPoint:Point3d, endePoint:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Adds a new linear light object to the document</summary>
    ///<param name="startPoint">(Point3d) Starting point of the light</param>
    ///<param name="endePoint">(Point3d) Ending point and direction of the light</param>
    ///<param name="width">(float) Optional, Default Value: <c>7e89</c>
    ///Width of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddLinearLight(startPoint:Point3d, endePoint:Point3d, [<OPT;DEF(7e89)>]width:float) : Guid =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Adds a new point light object to the document</summary>
    ///<param name="point">(Point3d) The 3d location of the point</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddPointLight(point:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Adds a new rectangular light object to the document</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="widthPoint">(Point3d) 3d width and direction point of the light</param>
    ///<param name="heightPoint">(Point3d) 3d height and direction point of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddRectangularLight(origin:Point3d, widthPoint:Point3d, heightPoint:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Adds a new spot light object to the document</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="radius">(float) Radius of the cone</param>
    ///<param name="apexPoint">(Point3d) 3d apex point of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddSpotLight(origin:Point3d, radius:float, apexPoint:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Get status of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) The current enabled status</returns>
    static member EnableLight(objectId:Guid) : bool = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Enables or disables a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="enable">(bool)The light's enabled status</param>
    ///<returns>(unit) void, nothing</returns>
    static member EnableLight(objectId:Guid, enable:bool) : unit = //SET
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies a light object is a directional light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsDirectionalLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies an object is a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies a light object is enabled</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLightEnabled(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies a light object is referenced from another file</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLightReference(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies a light object is a linear light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinearLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies a light object is a point light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsPointLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies a light object is a rectangular light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsRectangularLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies a light object is a spot light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsSpotLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the color of a light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Drawing.Color) The current color</returns>
    static member LightColor(objectId:Guid) : Drawing.Color = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Changes the color of a light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="color">(Drawing.Color)The light's new color</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the number of light objects in the document</summary>
    ///<returns>(int) the number of light objects in the document</returns>
    static member LightCount() : int =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the direction of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Vector3d) The current direction</returns>
    static member LightDirection(objectId:Guid) : Vector3d = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Changes the direction of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="direction">(Vector3d)The light's new direction</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightDirection(objectId:Guid, direction:Vector3d) : unit = //SET
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the location of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Point3d) The current location</returns>
    static member LightLocation(objectId:Guid) : Point3d = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Changes the location of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="location">(Point3d)The light's new location</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightLocation(objectId:Guid, location:Point3d) : unit = //SET
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the name of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(string) The current name</returns>
    static member LightName(objectId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Changes the name of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="name">(string)The light's new name</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightName(objectId:Guid, name:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns list of identifiers of light objects in the document</summary>
    ///<returns>(Guid seq) the list of identifiers of light objects in the document</returns>
    static member LightObjects() : Guid seq =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the plane of a rectangular light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Plane) the plane</returns>
    static member RectangularLightPlane(objectId:Guid) : Plane =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the hardness of a spot light. Spotlight hardness
    /// controls the fully illuminated region.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current hardness</returns>
    static member SpotLightHardness(objectId:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Changes the hardness of a spot light. Spotlight hardness
    /// controls the fully illuminated region.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="hardness">(float)The light's new hardness</param>
    ///<returns>(unit) void, nothing</returns>
    static member SpotLightHardness(objectId:Guid, hardness:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the radius of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current radius</returns>
    static member SpotLightRadius(objectId:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Changes the radius of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="radius">(float)The light's new radius</param>
    ///<returns>(unit) void, nothing</returns>
    static member SpotLightRadius(objectId:Guid, radius:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the shadow intensity of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(int) The current intensity</returns>
    static member SpotLightShadowIntensity(objectId:Guid) : int = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Changes the shadow intensity of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="intensity">(int)The light's new intensity</param>
    ///<returns>(unit) void, nothing</returns>
    static member SpotLightShadowIntensity(objectId:Guid, intensity:int) : unit = //SET
        failNotImpl () // genreation temp disabled !!


