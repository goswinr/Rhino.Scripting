
namespace Rhino

open System

open Rhino.Geometry

open FsEx
open FsEx.SaveIgnore

[<AutoOpen>]
module AutoOpenLight =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>Adds a new directional light object to the document.</summary>
    ///<param name="startPoint">(Point3d) Starting point of the light</param>
    ///<param name="endPoint">(Point3d) Ending point and direction of the light</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member AddDirectionalLight(startPoint:Point3d, endPoint:Point3d) : Guid = 
        let start =  startPoint
        let ende =  endPoint
        let light = new Light()
        light.LightStyle <- LightStyle.WorldDirectional
        light.Location <- start
        light.Direction <- ende-start
        let index = State.Doc.Lights.Add(light)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.AddDirectionalLight: Unable to add light to LightTable.  startPoint:'%A' endPoint:'%A'" startPoint endPoint
        let rc = State.Doc.Lights.[index].Id
        State.Doc.Views.Redraw()
        rc


    ///<summary>Adds a new linear light object to the document.</summary>
    ///<param name="startPoint">(Point3d) Starting point of the light</param>
    ///<param name="endPoint">(Point3d) Ending point and direction of the light</param>
    ///<param name="lightWidth">(float) Optional, Width of the light</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member AddLinearLight( startPoint:Point3d,
                                  endPoint:Point3d,
                                  [<OPT;DEF(0.0)>]lightWidth:float) : Guid = 
        let start =  startPoint
        let ende =  endPoint
        let mutable width = lightWidth
        if width = 0.0  then
            let mutable radius = 0.5
            let units = State.Doc.ModelUnitSystem
            if units <> UnitSystem.None then
                let scale = RhinoMath.UnitScale(UnitSystem.Millimeters, units)
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
        let xAxis = plane.XAxis
        xAxis.Unitize() |> ignore
        plane.XAxis <- xAxis
        light.Width <- xAxis * ( min width ( v.Length/20.0))
        //light.Location <- start - light.Direction
        let index = State.Doc.Lights.Add(light)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.AddLinearLight: Unable to add light to LightTable.  startPoint:'%A' endPoint:'%A' width:'%A'" startPoint endPoint lightWidth
        let rc = State.Doc.Lights.[index].Id
        State.Doc.Views.Redraw()
        rc


    ///<summary>Adds a new point light object to the document.</summary>
    ///<param name="point">(Point3d) The 3d location of the point</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member AddPointLight(point:Point3d) : Guid = 
        let light = new Light()
        light.LightStyle <- LightStyle.WorldPoint
        light.Location <- point
        let index = State.Doc.Lights.Add(light)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.AddPointLight: Unable to add light to LightTable.  point:'%A'" point
        let rc = State.Doc.Lights.[index].Id
        State.Doc.Views.Redraw()
        rc


    ///<summary>Adds a new rectangular light object to the document.</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="widthPoint">(Point3d) 3d width and direction point of the light</param>
    ///<param name="heightPoint">(Point3d) 3d height and direction point of the light</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member AddRectangularLight( origin:Point3d,
                                       widthPoint:Point3d,
                                       heightPoint:Point3d) : Guid = 
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
        let index = State.Doc.Lights.Add(light)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.AddRectangularLight: Unable to add light to LightTable.  origin:'%A' widthPoint:'%A' heightPoint:'%A'" origin widthPoint heightPoint
        let rc = State.Doc.Lights.[index].Id
        State.Doc.Views.Redraw()
        rc


    ///<summary>Adds a new spot light object to the document.</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="radius">(float) Radius of the cone</param>
    ///<param name="apexPoint">(Point3d) 3d apex point of the light</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member AddSpotLight( origin:Point3d,
                                radius:float,
                                apexPoint:Point3d) : Guid = 
        let mutable radius = radius
        if radius<0.0 then radius<-1.0
        let light = new Light()
        light.LightStyle <- LightStyle.WorldSpot
        light.Location <- apexPoint
        light.Direction <- origin-apexPoint
        light.SpotAngleRadians <- Math.Atan(radius / (light.Direction.Length))
        light.HotSpot <- 0.50
        let index = State.Doc.Lights.Add(light)
        if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.AddSpotLight: Unable to add light to LightTable.  origin:'%A' radius:'%A' apexPoint:'%A'" origin radius apexPoint
        let rc = State.Doc.Lights.[index].Id
        State.Doc.Views.Redraw()
        rc

    ///<summary>Get On / Off status of a light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) The current enabled status.</returns>
    static member EnableLight(objectId:Guid) : bool = //GET
        let light = Scripting.CoerceLight(objectId)
        let rc = light.IsEnabled
        rc

    ///<summary>Enables or disables a light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="enable">(bool) The light's enabled status</param>
    ///<returns>(unit) void, nothing.</returns>
    static member EnableLight(objectId:Guid, enable:bool) : unit = //SET
        let light = Scripting.CoerceLight(objectId)
        light.IsEnabled <- enable
        if not <| State.Doc.Lights.Modify(objectId, light) then
            RhinoScriptingException.Raise "Rhino.Scripting.EnableLight failed.  objectId:'%s' enable:'%A'" (Nice.str objectId) enable
        State.Doc.Views.Redraw()

    ///<summary>Enables or disables multiple light objects.</summary>
    ///<param name="objectIds">(Guid seq) The light objects's identifiers</param>
    ///<param name="enable">(bool) The light's enabled status</param>
    ///<returns>(unit) void, nothing.</returns>
    static member EnableLight(objectIds:Guid seq, enable:bool) : unit = //MULTISET
        for objectId in objectIds do
            let light = Scripting.CoerceLight(objectId)
            light.IsEnabled <- enable
            if not <| State.Doc.Lights.Modify(objectId, light) then
                RhinoScriptingException.Raise "Rhino.Scripting.EnableLight failed.  objectId:'%s' enable:'%A'" (Nice.str objectId) enable
        State.Doc.Views.Redraw()


    ///<summary>Verifies a light object is a directional light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDirectionalLight(objectId:Guid) : bool = 
        let light = Scripting.CoerceLight(objectId)
        light.IsDirectionalLight


    ///<summary>Verifies an object is a light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLight(objectId:Guid) : bool = 
        Scripting.TryCoerceLight(objectId)
        |> Option.isSome


    ///<summary>Verifies a light object is enabled.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLightEnabled(objectId:Guid) : bool = 
        let light = Scripting.CoerceLight(objectId)
        light.IsEnabled


    ///<summary>Verifies a light object is referenced from another file.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLightReference(objectId:Guid) : bool = 
        let light = State.Doc.Lights.FindId(objectId)
        if isNull light then RhinoScriptingException.Raise "Rhino.Scripting.IsLightReference light (a %s) not found" (Nice.str objectId)
        light.IsReference


    ///<summary>Verifies a light object is a linear light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLinearLight(objectId:Guid) : bool = 
        let light = Scripting.CoerceLight(objectId)
        light.IsLinearLight


    ///<summary>Verifies a light object is a point light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsPointLight(objectId:Guid) : bool = 
        let light = Scripting.CoerceLight(objectId)
        light.IsPointLight


    ///<summary>Verifies a light object is a rectangular light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsRectangularLight(objectId:Guid) : bool = 
        let light = Scripting.CoerceLight(objectId)
        light.IsRectangularLight


    ///<summary>Verifies a light object is a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsSpotLight(objectId:Guid) : bool = 
        let light = Scripting.CoerceLight(objectId)
        light.IsSpotLight


    ///<summary>Returns the color of a light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Drawing.Color) The current color.</returns>
    static member LightColor(objectId:Guid) : Drawing.Color = //GET
        let light = Scripting.CoerceLight(objectId)
        let rc = light.Diffuse
        rc

    ///<summary>Changes the color of a light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="color">(Drawing.Color) The light's new color</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LightColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        let light = Scripting.CoerceLight(objectId)
        light.Diffuse <- color
        if not <|  State.Doc.Lights.Modify(objectId, light) then
            RhinoScriptingException.Raise "Rhino.Scripting.LightColor failed.  objectId:'%s' color:'%A'" (Nice.str objectId) color
        State.Doc.Views.Redraw()

    ///<summary>Changes the color of multiple light.</summary>
    ///<param name="objectIds">(Guid seq) The light objects's identifiers</param>
    ///<param name="color">(Drawing.Color) The light's new color</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LightColor(objectIds:Guid seq, color:Drawing.Color) : unit = //MULTISET
        for objectId in objectIds do
            let light = Scripting.CoerceLight(objectId)
            light.Diffuse <- color
            if not <|  State.Doc.Lights.Modify(objectId, light) then
                RhinoScriptingException.Raise "Rhino.Scripting.LightColor failed.  objectId:'%s' color:'%A'" (Nice.str objectId) color
        State.Doc.Views.Redraw()


    ///<summary>Returns the number of light objects in the document.</summary>
    ///<returns>(int) The number of light objects in the document.</returns>
    static member LightCount() : int = 
        State.Doc.Lights.Count


    ///<summary>Returns the direction of a light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Vector3d) The current direction.</returns>
    static member LightDirection(objectId:Guid) : Vector3d = //GET
        let light = Scripting.CoerceLight(objectId)
        let rc = light.Direction
        rc

    ///<summary>Changes the direction of a light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="direction">(Vector3d) The light's new direction</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LightDirection(objectId:Guid, direction:Vector3d) : unit = //SET
        let light = Scripting.CoerceLight(objectId)
        light.Direction <- direction
        if not<|  State.Doc.Lights.Modify(objectId, light) then
            RhinoScriptingException.Raise "Rhino.Scripting.LightDirection failed.  objectId:'%s' direction:'%A'" (Nice.str objectId) direction
        State.Doc.Views.Redraw()

    ///<summary>Changes the direction of multiple light objects.</summary>
    ///<param name="objectIds">(Guid seq) The light objects's identifiers</param>
    ///<param name="direction">(Vector3d) The light's new direction</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LightDirection(objectIds:Guid seq, direction:Vector3d) : unit = //MULTISET
        for objectId in objectIds do
            let light = Scripting.CoerceLight(objectId)
            light.Direction <- direction
            if not<|  State.Doc.Lights.Modify(objectId, light) then
                RhinoScriptingException.Raise "Rhino.Scripting.LightDirection failed.  objectId:'%s' direction:'%A'" (Nice.str objectId) direction
        State.Doc.Views.Redraw()


    ///<summary>Returns the location of a light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Point3d) The current location.</returns>
    static member LightLocation(objectId:Guid) : Point3d = //GET
        let light = Scripting.CoerceLight(objectId)
        let rc = light.Location
        rc

    ///<summary>Changes the location of a light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="location">(Point3d) The light's new location</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LightLocation(objectId:Guid, location:Point3d) : unit = //SET
        let light = Scripting.CoerceLight(objectId)
        light.Location <- location
        if not<|  State.Doc.Lights.Modify(objectId, light) then
            RhinoScriptingException.Raise "Rhino.Scripting.LightLocation failed.  objectId:'%s' location:'%A'" (Nice.str objectId) location
        State.Doc.Views.Redraw()

    ///<summary>Changes the location of multiple light objects.</summary>
    ///<param name="objectIds">(Guid seq) The light objects's identifiers</param>
    ///<param name="location">(Point3d) The light's new location</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LightLocation(objectIds:Guid seq, location:Point3d) : unit = //MULTISET
        for objectId in objectIds do
            let light = Scripting.CoerceLight(objectId)
            light.Location <- location
            if not<|  State.Doc.Lights.Modify(objectId, light) then
                RhinoScriptingException.Raise "Rhino.Scripting.LightLocation failed.  objectId:'%s' location:'%A'" (Nice.str objectId) location
        State.Doc.Views.Redraw()


    ///<summary>Returns the name of a light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(string) The current name.</returns>
    static member LightName(objectId:Guid) : string = //GET
        let light = Scripting.CoerceLight(objectId)
        let rc = light.Name
        rc

    ///<summary>Changes the name of a light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="name">(string) The light's new name</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LightName(objectId:Guid, name:string) : unit = //SET
        let light = Scripting.CoerceLight(objectId)
        light.Name <- name
        if not <|  State.Doc.Lights.Modify(objectId, light) then
            RhinoScriptingException.Raise "Rhino.Scripting.LightName failed.  objectId:'%s' name:'%A'" (Nice.str objectId) name
        State.Doc.Views.Redraw()
    ///<summary>Changes the name of multiple light objects.</summary>
    ///<param name="objectIds">(Guid seq) The light objects's identifiers</param>
    ///<param name="name">(string) The light's new name</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LightName(objectIds:Guid seq, name:string) : unit = //MULTISET
        for objectId in objectIds do
            let light = Scripting.CoerceLight(objectId)
            light.Name <- name
            if not <|  State.Doc.Lights.Modify(objectId, light) then
                RhinoScriptingException.Raise "Rhino.Scripting.LightName failed.  objectId:'%s' name:'%A'" (Nice.str objectId) name
        State.Doc.Views.Redraw()


    ///<summary>Returns list of identifiers of light objects in the document.</summary>
    ///<returns>(Guid Rarr) The list of identifiers of light objects in the document.</returns>
    static member LightObjects() : Guid Rarr = 
        let count = State.Doc.Lights.Count
        let rc = Rarr()
        for i = 0 to count - 1 do
            let rhlight = State.Doc.Lights.[i]
            if not rhlight.IsDeleted then rc.Add(rhlight.Id)
        rc


    ///<summary>Returns the Plane of a rectangular light object.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Plane*float*float) The Plane, X and Y length.</returns>
    static member RectangularLightPlane(objectId:Guid) : Plane*float*float = 
        let light = Scripting.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldRectangular then
            RhinoScriptingException.Raise "Rhino.Scripting.RectangularLightPlane failed.  objectId:'%s'" (Nice.str objectId)
        let location = light.Location
        let length = light.Length
        let width = light.Width
        let direction = light.Direction
        let plane = Plane(location, length, width)
        plane, length.Length, width.Length


    ///<summary>Returns the hardness of a spot light. Spotlight hardness
    /// controls the fully illuminated region.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current hardness.</returns>
    static member SpotLightHardness(objectId:Guid) : float = //GET
        let light = Scripting.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            RhinoScriptingException.Raise "Rhino.Scripting.SpotLightHardness failed.  objectId:'%s'" (Nice.str objectId)
        let rc = light.HotSpot
        rc

    ///<summary>Changes the hardness of a spot light. Spotlight hardness
    /// controls the fully illuminated region.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="hardness">(float) The light's new hardness</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SpotLightHardness(objectId:Guid, hardness:float) : unit = //SET
        let light = Scripting.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            RhinoScriptingException.Raise "Rhino.Scripting.SpotLightHardness failed.  objectId:'%s' hardness:'%A'" (Nice.str objectId) hardness
        light.HotSpot <- hardness
        if not <|  State.Doc.Lights.Modify(objectId, light) then
            RhinoScriptingException.Raise "Rhino.Scripting.SpotLightHardness failed.  objectId:'%s' hardness:'%A'" (Nice.str objectId) hardness
        State.Doc.Views.Redraw()

    ///<summary>Changes the hardness of multiple spot light. Spotlight hardness
    /// controls the fully illuminated region.</summary>
    ///<param name="objectIds">(Guid seq) The light objects's identifiers</param>
    ///<param name="hardness">(float) The light's new hardness</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SpotLightHardness(objectIds:Guid seq, hardness:float) : unit = //MULTISET
        for objectId in objectIds do
            let light = Scripting.CoerceLight(objectId)
            if light.LightStyle <> LightStyle.WorldSpot then
                RhinoScriptingException.Raise "Rhino.Scripting.SpotLightHardness failed.  objectId:'%s' hardness:'%A'" (Nice.str objectId) hardness
            light.HotSpot <- hardness
            if not <|  State.Doc.Lights.Modify(objectId, light) then
                RhinoScriptingException.Raise "Rhino.Scripting.SpotLightHardness failed.  objectId:'%s' hardness:'%A'" (Nice.str objectId) hardness
        State.Doc.Views.Redraw()


    ///<summary>Returns the radius of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current radius.</returns>
    static member SpotLightRadius(objectId:Guid) : float = //GET
        let light = Scripting.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            RhinoScriptingException.Raise "Rhino.Scripting.SpotLightRadius failed.  objectId:'%s'" (Nice.str objectId)
        let radians = light.SpotAngleRadians
        let rc = light.Direction.Length * tan(radians)
        rc

    ///<summary>Changes the radius of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="radius">(float) The light's new radius</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SpotLightRadius(objectId:Guid, radius:float) : unit = //SET
        let light = Scripting.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            RhinoScriptingException.Raise "Rhino.Scripting.SpotLightRadius failed.  objectId:'%s' radius:'%A'" (Nice.str objectId) radius
        let radians = Math.Atan(radius/light.Direction.Length)
        light.SpotAngleRadians <- radians
        if not <|  State.Doc.Lights.Modify(objectId, light) then
            RhinoScriptingException.Raise "Rhino.Scripting.SpotLightRadius failed.  objectId:'%s' radius:'%A'" (Nice.str objectId) radius
        State.Doc.Views.Redraw()

    ///<summary>Changes the radius of multiple spot light.</summary>
    ///<param name="objectIds">(Guid seq) The light objects's identifiers</param>
    ///<param name="radius">(float) The light's new radius</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SpotLightRadius(objectIds:Guid seq, radius:float) : unit = //MULTISET
        for objectId in objectIds do
            let light = Scripting.CoerceLight(objectId)
            if light.LightStyle <> LightStyle.WorldSpot then
                RhinoScriptingException.Raise "Rhino.Scripting.SpotLightRadius failed.  objectId:'%s' radius:'%A'" (Nice.str objectId) radius
            let radians = Math.Atan(radius/light.Direction.Length)
            light.SpotAngleRadians <- radians
            if not <|  State.Doc.Lights.Modify(objectId, light) then
                RhinoScriptingException.Raise "Rhino.Scripting.SpotLightRadius failed.  objectId:'%s' radius:'%A'" (Nice.str objectId) radius
        State.Doc.Views.Redraw()


    ///<summary>Returns the shadow intensity of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current intensity.</returns>
    static member SpotLightShadowIntensity(objectId:Guid) : float = //GET
        let light = Scripting.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            RhinoScriptingException.Raise "Rhino.Scripting.SpotLightShadowIntensity failed.  objectId:'%s'" (Nice.str objectId)
        let rc = light.ShadowIntensity
        rc

    ///<summary>Changes the shadow intensity of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="intensity">(float) The light's new intensity</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SpotLightShadowIntensity(objectId:Guid, intensity:float) : unit = //SET
        let light = Scripting.CoerceLight(objectId)
        if light.LightStyle <> LightStyle.WorldSpot then
            RhinoScriptingException.Raise "Rhino.Scripting.SpotLightShadowIntensity failed.  objectId:'%s' intensity:'%A'" (Nice.str objectId) intensity
        light.ShadowIntensity <- intensity
        if not <|  State.Doc.Lights.Modify(objectId, light) then
            RhinoScriptingException.Raise "Rhino.Scripting.SpotLightShadowIntensity failed.  objectId:'%s' intensity:'%A'" (Nice.str objectId) intensity
        State.Doc.Views.Redraw()

    ///<summary>Changes the shadow intensity of multiple spot light.</summary>
    ///<param name="objectIds">(Guid seq) The light objects's identifiers</param>
    ///<param name="intensity">(float) The light's new intensity</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SpotLightShadowIntensity(objectIds:Guid seq, intensity:float) : unit = //MULTISET
        for objectId in objectIds do
            let light = Scripting.CoerceLight(objectId)
            if light.LightStyle <> LightStyle.WorldSpot then
                RhinoScriptingException.Raise "Rhino.Scripting.SpotLightShadowIntensity failed.  objectId:'%s' intensity:'%A'" (Nice.str objectId) intensity
            light.ShadowIntensity <- intensity
            if not <|  State.Doc.Lights.Modify(objectId, light) then
                RhinoScriptingException.Raise "Rhino.Scripting.SpotLightShadowIntensity failed.  objectId:'%s' intensity:'%A'" (Nice.str objectId) intensity
        State.Doc.Views.Redraw()




