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
    ///<param name="endPoint">(Point3d) Ending point and direction of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddDirectionalLight(startPoint:Point3d, endPoint:Point3d) : Guid =
        let start =  startPoint
        let ende =  endPoint
        let light = new Light()
        light.LightStyle <- LightStyle.WorldDirectional
        light.Location <- start
        light.Direction <- ende-start
        let index = Doc.Lights.Add(light)
        if index<0 then failwithf "Rhino.Scripting: Unable to add light to LightTable.  startPoint:'%A' endPoint:'%A'" startPoint endPoint
        let rc = Doc.Lights.[index].Id
        Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Adds a new linear light object to the document</summary>
    ///<param name="startPoint">(Point3d) Starting point of the light</param>
    ///<param name="endPoint">(Point3d) Ending point and direction of the light</param>
    ///<param name="width">(float) Optional, Default Value: <c>7e89</c>
    ///Width of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddLinearLight( startPoint:Point3d,
                                  endPoint:Point3d,
                                  [<OPT;DEF(0.0)>]width:float) : Guid =
        let start =  startPoint
        let ende =  endPoint
        let mutable width = width
        if width=0.0  then
            let mutable radius=0.5
            let units = Doc.ModelUnitSystem
            if units <> UnitSystem.None then
                let scale = Rhino.RhinoMath.UnitScale(UnitSystem.Millimeters, units)
                radius <- radius * scale
            width <- radius
        let light = new Light()
        light.LightStyle <- LightStyle.WorldLinear
        light.Location <- start
        let v = ende-start
        light.Direction <- v
        light.Length <- light.Direction
        light.Width <- -light.Width
        let mutable plane = Plane(light.Location, light.Direction)
        let xaxis = plane.XAxis
        xaxis.Unitize() |> ignore
        plane.XAxis <- xaxis
        light.Width <- xaxis * ( min width ( v.Length/20.0))
        //light.Location <- start - light.Direction
        let index = Doc.Lights.Add(light)
        if index<0 then failwithf "Rhino.Scripting: Unable to add light to LightTable.  startPoint:'%A' endPoint:'%A' width:'%A'" startPoint endPoint width
        let rc = Doc.Lights.[index].Id
        Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Adds a new point light object to the document</summary>
    ///<param name="point">(Point3d) The 3d location of the point</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddPointLight(point:Point3d) : Guid =
        let light = new Light()
        light.LightStyle <- LightStyle.WorldPoint
        light.Location <- point
        let index = Doc.Lights.Add(light)
        if index<0 then failwithf "Rhino.Scripting: Unable to add light to LightTable.  point:'%A'" point
        let rc = Doc.Lights.[index].Id
        Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Adds a new rectangular light object to the document</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="widthPoint">(Point3d) 3d width and direction point of the light</param>
    ///<param name="heightPoint">(Point3d) 3d height and direction point of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddRectangularLight( origin:Point3d,
                                       widthPoint:Point3d,
                                       heightPoint:Point3d) : Guid =
        let origin =  origin
        let ptx =  widthPoint
        let pty =  heightPoint
        let length = pty-origin
        let width = ptx-origin
        let normal = Vector3d.CrossProduct(width, length)
        normal.Unitize() |> ignore
        let light = new Light()
        light.LightStyle <- LightStyle.WorldRectangular
        light.Location <- origin
        light.Width <- width
        light.Length <- length
        light.Direction <- normal
        let index = Doc.Lights.Add(light)
        if index<0 then failwithf "Rhino.Scripting: Unable to add light to LightTable.  origin:'%A' widthPoint:'%A' heightPoint:'%A'" origin widthPoint heightPoint
        let rc = Doc.Lights.[index].Id
        Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Adds a new spot light object to the document</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="radius">(float) Radius of the cone</param>
    ///<param name="apexPoint">(Point3d) 3d apex point of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddSpotLight( origin:Point3d,
                                radius:float,
                                apexPoint:Point3d) : Guid =
        let mutable radius = radius
        let apexPoint =  apexPoint
        if radius<0.0 then radius<-1.0
        let light = new Light()
        light.LightStyle <- LightStyle.WorldSpot
        light.Location <- apexPoint
        light.Direction <- origin-apexPoint
        light.SpotAngleRadians <- Math.Atan(radius / (light.Direction.Length))
        light.HotSpot <- 0.50
        let index = Doc.Lights.Add(light)
        if index<0 then failwithf "Rhino.Scripting: Unable to add light to LightTable.  origin:'%A' radius:'%A' apexPoint:'%A'" origin radius apexPoint
        let rc = Doc.Lights.[index].Id
        Doc.Views.Redraw()
        rc

    [<EXT>]
    ///<summary>Get On / Off status of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) The current enabled status</returns>
    static member EnableLight(objectId:Guid) : bool = //GET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        let rc = light.IsEnabled
        rc

    ///<summary>Enables or disables a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="enable">(bool)The light's enabled status</param>
    ///<returns>(unit) void, nothing</returns>
    static member EnableLight(objectId:Guid, enable:bool) : unit = //SET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.IsEnabled <- enable
        //id = RhinoScriptSyntax.Coerceguid(objectId)
        if not <| Doc.Lights.Modify(objectId, light) then
            failwithf "Rhino.Scripting: EnableLight failed.  objectId:'%A' enable:'%A'" objectId enable
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Verifies a light object is a directional light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsDirectionalLight(objectId:Guid) : bool =
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.IsDirectionalLight


    [<EXT>]
    ///<summary>Verifies an object is a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLight(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceLight(objectId)
        |> Option.isSome


    [<EXT>]
    ///<summary>Verifies a light object is enabled</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLightEnabled(objectId:Guid) : bool =
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.IsEnabled


    [<EXT>]
    ///<summary>Verifies a light object is referenced from another file</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLightReference(objectId:Guid) : bool =
        let light = Doc.Lights.FindId(objectId)
        if isNull light then failwithf "IsLightReference light %A (a %s) not found" objectId (rhtype objectId)
        light.IsReference


    [<EXT>]
    ///<summary>Verifies a light object is a linear light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinearLight(objectId:Guid) : bool =
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.IsLinearLight


    [<EXT>]
    ///<summary>Verifies a light object is a point light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsPointLight(objectId:Guid) : bool =
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.IsPointLight


    [<EXT>]
    ///<summary>Verifies a light object is a rectangular light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsRectangularLight(objectId:Guid) : bool =
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.IsRectangularLight


    [<EXT>]
    ///<summary>Verifies a light object is a spot light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsSpotLight(objectId:Guid) : bool =
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.IsSpotLight


    [<EXT>]
    ///<summary>Returns the color of a light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Drawing.Color) The current color</returns>
    static member LightColor(objectId:Guid) : Drawing.Color = //GET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        let rc = light.Diffuse
        rc

    ///<summary>Changes the color of a light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="color">(Drawing.Color)The light's new color</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.Diffuse <- color
        if not <|  Doc.Lights.Modify(objectId, light) then
            failwithf "Rhino.Scripting: LightColor failed.  objectId:'%A' color:'%A'" objectId color
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the number of light objects in the document</summary>
    ///<returns>(int) the number of light objects in the document</returns>
    static member LightCount() : int =
        Doc.Lights.Count


    [<EXT>]
    ///<summary>Returns the direction of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Vector3d) The current direction</returns>
    static member LightDirection(objectId:Guid) : Vector3d = //GET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        let rc = light.Direction
        rc

    ///<summary>Changes the direction of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="direction">(Vector3d)The light's new direction</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightDirection(objectId:Guid, direction:Vector3d) : unit = //SET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.Direction <- direction
        if not<|  Doc.Lights.Modify(objectId, light) then
            failwithf "Rhino.Scripting: LightDirection failed.  objectId:'%A' direction:'%A'" objectId direction
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the location of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Point3d) The current location</returns>
    static member LightLocation(objectId:Guid) : Point3d = //GET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        let rc = light.Location
        rc

    ///<summary>Changes the location of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="location">(Point3d)The light's new location</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightLocation(objectId:Guid, location:Point3d) : unit = //SET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.Location <- location
        if not<|  Doc.Lights.Modify(objectId, light) then
            failwithf "Rhino.Scripting: LightLocation failed.  objectId:'%A' location:'%A'" objectId location
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the name of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(string) The current name</returns>
    static member LightName(objectId:Guid) : string = //GET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        let rc = light.Name
        rc

    ///<summary>Changes the name of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="name">(string)The light's new name</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightName(objectId:Guid, name:string) : unit = //SET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        light.Name <- name
        if not <|  Doc.Lights.Modify(objectId, light) then
            failwithf "Rhino.Scripting: LightName failed.  objectId:'%A' name:'%A'" objectId name
        Doc.Views.Redraw()


    [<EXT>]
    ///<summary>Returns list of identifiers of light objects in the document</summary>
    ///<returns>(Guid ResizeArray) the list of identifiers of light objects in the document</returns>
    static member LightObjects() : Guid ResizeArray =
        let count = Doc.Lights.Count
        let rc = ResizeArray()
        for i in range(count) do
            let rhlight = Doc.Lights.[i]
            if not rhlight.IsDeleted then rc.Add(rhlight.Id)
        rc


    [<EXT>]
    ///<summary>Returns the plane of a rectangular light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Plane*float*float) the plane, X and Y length </returns>
    static member RectangularLightPlane(objectId:Guid) : Plane*float*float =
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldRectangular then
            failwithf "Rhino.Scripting: RectangularLightPlane failed.  objectId:'%A'" objectId
        let location = light.Location
        let length = light.Length
        let width = light.Width
        let direction = light.Direction
        let plane = Plane(location, length, width)
        plane, length.Length, width.Length


    [<EXT>]
    ///<summary>Returns the hardness of a spot light. Spotlight hardness
    /// controls the fully illuminated region.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current hardness</returns>
    static member SpotLightHardness(objectId:Guid) : float = //GET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            failwithf "Rhino.Scripting: SpotLightHardness failed.  objectId:'%A'" objectId
        let rc = light.HotSpot
        rc

    ///<summary>Changes the hardness of a spot light. Spotlight hardness
    /// controls the fully illuminated region.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="hardness">(float)The light's new hardness</param>
    ///<returns>(unit) void, nothing</returns>
    static member SpotLightHardness(objectId:Guid, hardness:float) : unit = //SET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            failwithf "Rhino.Scripting: SpotLightHardness failed.  objectId:'%A' hardness:'%A'" objectId hardness
        light.HotSpot <- hardness
        if not <|  Doc.Lights.Modify(objectId, light) then
            failwithf "Rhino.Scripting: SpotLightHardness failed.  objectId:'%A' hardness:'%A'" objectId hardness
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the radius of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current radius</returns>
    static member SpotLightRadius(objectId:Guid) : float = //GET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            failwithf "Rhino.Scripting: SpotLightRadius failed.  objectId:'%A' " objectId
        let radians = light.SpotAngleRadians
        let rc = light.Direction.Length * tan(radians)
        rc

    ///<summary>Changes the radius of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="radius">(float)The light's new radius</param>
    ///<returns>(unit) void, nothing</returns>
    static member SpotLightRadius(objectId:Guid, radius:float) : unit = //SET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            failwithf "Rhino.Scripting: SpotLightRadius failed.  objectId:'%A' radius:'%A'" objectId radius
        let radians = Math.Atan(radius/light.Direction.Length)
        light.SpotAngleRadians <- radians
        if not <|  Doc.Lights.Modify(objectId, light) then
            failwithf "Rhino.Scripting: SpotLightRadius failed.  objectId:'%A' radius:'%A'" objectId radius
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the shadow intensity of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current intensity</returns>
    static member SpotLightShadowIntensity(objectId:Guid) : float = //GET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            failwithf "Rhino.Scripting: SpotLightShadowIntensity failed.  objectId:'%A' " objectId
        let rc = light.ShadowIntensity
        rc

    ///<summary>Changes the shadow intensity of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="intensity">(float)The light's new intensity</param>
    ///<returns>(unit) void, nothing</returns>
    static member SpotLightShadowIntensity(objectId:Guid, intensity:float) : unit = //SET
        let light = RhinoScriptSyntax.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            failwithf "Rhino.Scripting: SpotLightShadowIntensity failed.  objectId:'%A' intensity:'%A'" objectId intensity
        light.ShadowIntensity <- intensity
        if not <|  Doc.Lights.Modify(objectId, light) then
            failwithf "Rhino.Scripting: SpotLightShadowIntensity failed.  objectId:'%A' intensity:'%A'" objectId intensity
        Doc.Views.Redraw()



