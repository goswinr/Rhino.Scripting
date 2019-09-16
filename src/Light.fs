namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
[<AutoOpen>]
module ExtensionsLight =
  type RhinoScriptSyntax with
    
    ///<summary>Adds a new directional light object to the document</summary>
    ///<param name="startPoint">(Point3d) Starting point of the light</param>
    ///<param name="endePoint">(Point3d) Ending point and direction of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddDirectionalLight(startPoint:Point3d, endePoint:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddDirectionalLight(start_point, end_point):
        '''Adds a new directional light object to the document
        Parameters:
          start_point(point): starting point of the light
          end_point (point): ending point and direction of the light
        Returns:
          guid: identifier of the new object if successful
        '''
        start = rhutil.coerce3dpoint(start_point, True)
        end = rhutil.coerce3dpoint(end_point, True)
        light = Rhino.Geometry.Light()
        light.LightStyle = Rhino.Geometry.LightStyle.WorldDirectional
        light.Location = start
        light.Direction = end-start
        index = scriptcontext.doc.Lights.Add(light)
        if index<0: raise Exception("unable to add light to LightTable")
        rc = scriptcontext.doc.Lights[index].Id
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a new linear light object to the document</summary>
    ///<param name="startPoint">(Point3d) Starting point of the light</param>
    ///<param name="endePoint">(Point3d) Ending point and direction of the light</param>
    ///<param name="width">(float) Optional, Default Value: <c>null</c>
    ///Width of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddLinearLight(startPoint:Point3d, endePoint:Point3d, [<OPT;DEF(null)>]width:float) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddLinearLight(start_point, end_point, width=None):
        '''Adds a new linear light object to the document
        Parameters:
          start_point (point): starting point of the light
          end_point (point): ending point and direction of the light
          width (number): width of the light
        Returns:
          guid: identifier of the new object if successful
          None: on error
        '''
        start = rhutil.coerce3dpoint(start_point, True)
        end = rhutil.coerce3dpoint(end_point, True)
        if width is None:
            radius=0.5
            units = scriptcontext.doc.ModelUnitSystem
            if units!=Rhino.UnitSystem.None:
                scale = Rhino.RhinoMath.UnitScale(Rhino.UnitSystem.Inches, units)
                radius *= scale
            width = radius
        light = Rhino.Geometry.Light()
        light.LightStyle = Rhino.Geometry.LightStyle.WorldLinear
        light.Location = start
        v = end-start
        light.Direction = v
        light.Length = light.Direction
        light.Width = -light.Width
        plane = Rhino.Geometry.Plane(light.Location, light.Direction)
        xaxis = plane.XAxis
        xaxis.Unitize()
        plane.XAxis = xaxis
        light.Width = xaxis * min(width, v.Length/20)
        #light.Location = start - light.Direction
        index = scriptcontext.doc.Lights.Add(light)
        if index<0: raise Exception("unable to add light to LightTable")
        rc = scriptcontext.doc.Lights[index].Id
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a new point light object to the document</summary>
    ///<param name="point">(Point3d) The 3d location of the point</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddPointLight(point:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddPointLight(point):
        '''Adds a new point light object to the document
        Parameters:
          point (point): the 3d location of the point
        Returns:
          guid: identifier of the new object if successful
        '''
        point = rhutil.coerce3dpoint(point, True)
        light = Rhino.Geometry.Light()
        light.LightStyle = Rhino.Geometry.LightStyle.WorldPoint
        light.Location = point
        index = scriptcontext.doc.Lights.Add(light)
        if index<0: raise Exception("unable to add light to LightTable")
        rc = scriptcontext.doc.Lights[index].Id
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a new rectangular light object to the document</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="widthPoint">(Point3d) 3d width and direction point of the light</param>
    ///<param name="heightPoint">(Point3d) 3d height and direction point of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddRectangularLight(origin:Point3d, widthPoint:Point3d, heightPoint:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddRectangularLight(origin, width_point, height_point):
        '''Adds a new rectangular light object to the document
        Parameters:
          origin (point): 3d origin point of the light
          width_point (point): 3d width and direction point of the light
          height_point (point): 3d height and direction point of the light
        Returns:
          guid: identifier of the new object if successful
        '''
        origin = rhutil.coerce3dpoint(origin, True)
        ptx = rhutil.coerce3dpoint(width_point, True)
        pty = rhutil.coerce3dpoint(height_point, True)
        length = pty-origin
        width = ptx-origin
        normal = Rhino.Geometry.Vector3d.CrossProduct(width, length)
        normal.Unitize()
        light = Rhino.Geometry.Light()
        light.LightStyle = Rhino.Geometry.LightStyle.WorldRectangular
        light.Location = origin
        light.Width = width
        light.Length = length
        light.Direction = normal
        index = scriptcontext.doc.Lights.Add(light)
        if index<0: raise Exception("unable to add light to LightTable")
        rc = scriptcontext.doc.Lights[index].Id
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a new spot light object to the document</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="radius">(float) Radius of the cone</param>
    ///<param name="apexPoint">(Point3d) 3d apex point of the light</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddSpotLight(origin:Point3d, radius:float, apexPoint:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddSpotLight(origin, radius, apex_point):
        '''Adds a new spot light object to the document
        Parameters:
          origin (point): 3d origin point of the light
          radius (number):  radius of the cone
          apex_point (point): 3d apex point of the light
        Returns:
          guid: identifier of the new object
        '''
        origin = rhutil.coerce3dpoint(origin, True)
        apex_point = rhutil.coerce3dpoint(apex_point, True)
        if radius<0: radius=1.0
        light = Rhino.Geometry.Light()
        light.LightStyle = Rhino.Geometry.LightStyle.WorldSpot
        light.Location = apex_point
        light.Direction = origin-apex_point
        light.SpotAngleRadians = math.atan(radius / (light.Direction.Length))
        light.HotSpot = 0.50
        index = scriptcontext.doc.Lights.Add(light)
        if index<0: raise Exception("unable to add light to LightTable")
        rc = scriptcontext.doc.Lights[index].Id
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Get status of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) The current enabled status</returns>
    static member EnableLight(objectId:Guid) : bool = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def EnableLight(object_id, enable=None):
        '''Enables or disables a light object
        Parameters:
          object_id (guid): the light object's identifier
          enable (bool, optional): the light's enabled status
        Returns:
          bool: if enable is not specified, the current enabled status
          bool: if enable is specified, the previous enabled status
          None: on error
        '''
        light = __coercelight(object_id, True)
        rc = light.IsEnabled
        if enable is not None and enable!=rc:
            light.IsEnabled = enable
            id = rhutil.coerceguid(object_id)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Enables or disables a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="enable">(bool)The light's enabled status</param>
    ///<returns>(unit) void, nothing</returns>
    static member EnableLight(objectId:Guid, enable:bool) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def EnableLight(object_id, enable=None):
        '''Enables or disables a light object
        Parameters:
          object_id (guid): the light object's identifier
          enable (bool, optional): the light's enabled status
        Returns:
          bool: if enable is not specified, the current enabled status
          bool: if enable is specified, the previous enabled status
          None: on error
        '''
        light = __coercelight(object_id, True)
        rc = light.IsEnabled
        if enable is not None and enable!=rc:
            light.IsEnabled = enable
            id = rhutil.coerceguid(object_id)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Verifies a light object is a directional light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsDirectionalLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsDirectionalLight(object_id):
        '''Verifies a light object is a directional light
        Parameters:
          object_id (guid): the light object's identifier
        Returns:
          bool: True or False
        '''
        light = __coercelight(object_id, True)
        return light.IsDirectionalLight
    *)


    ///<summary>Verifies an object is a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLight(object_id):
        '''Verifies an object is a light object
        Parameters:
          object_id (guid): the light object's identifier
        Returns:
          bool: True or False
        '''
        light = __coercelight(object_id, False)
        return light is not None
    *)


    ///<summary>Verifies a light object is enabled</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLightEnabled(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLightEnabled(object_id):
        '''Verifies a light object is enabled
        Parameters:
          object_id (guid): the light object's identifier
        Returns:
          bool: True or False
        '''
        light = __coercelight(object_id, True)
        return light.IsEnabled
    *)


    ///<summary>Verifies a light object is referenced from another file</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLightReference(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLightReference(object_id):
        '''Verifies a light object is referenced from another file
        Parameters:
          object_id (guid): the light object's identifier
        Returns:
          bool: True or False
        '''
        light = __coercelight(object_id, True)
        return light.IsReference
    *)


    ///<summary>Verifies a light object is a linear light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinearLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsLinearLight(object_id):
        '''Verifies a light object is a linear light
        Parameters:
          object_id (guid): the light object's identifier
        Returns:
          bool: True or False
        '''
        light = __coercelight(object_id, True)
        return light.IsLinearLight
    *)


    ///<summary>Verifies a light object is a point light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsPointLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsPointLight(object_id):
        '''Verifies a light object is a point light
        Parameters:
          object_id (guid): the light object's identifier
        Returns:
          bool: True or False
        '''
        light = __coercelight(object_id, True)
        return light.IsPointLight
    *)


    ///<summary>Verifies a light object is a rectangular light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsRectangularLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsRectangularLight(object_id):
        '''Verifies a light object is a rectangular light
        Parameters:
          object_id (guid): the light object's identifier
        Returns:
          bool: True or False
        '''
        light = __coercelight(object_id, True)
        return light.IsRectangularLight
    *)


    ///<summary>Verifies a light object is a spot light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsSpotLight(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsSpotLight(object_id):
        '''Verifies a light object is a spot light
        Parameters:
          object_id (guid): the light object's identifier
        Returns:
          bool: True or False
        '''
        light = __coercelight(object_id, True)
        return light.IsSpotLight
    *)


    ///<summary>Returns the color of a light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Drawing.Color) The current color</returns>
    static member LightColor(objectId:Guid) : Drawing.Color = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LightColor(object_id, color=None):
        '''Returns or changes the color of a light
        Parameters:
          object_id (guid): the light object's identifier
          color (color, optional): the light's new color
        Returns:
          color: if color is not specified, the current color
          color: if color is specified, the previous color
        '''
        light = __coercelight(object_id, True)
        rc = light.Diffuse
        if color:
            color = rhutil.coercecolor(color, True)
            if color!=rc:
                light.Diffuse = color
                id = rhutil.coerceguid(object_id, True)
                if not scriptcontext.doc.Lights.Modify(id, light):
                    return scriptcontext.errorhandler()
                scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the color of a light</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="color">(Drawing.Color)The light's new color</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightColor(objectId:Guid, color:Drawing.Color) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LightColor(object_id, color=None):
        '''Returns or changes the color of a light
        Parameters:
          object_id (guid): the light object's identifier
          color (color, optional): the light's new color
        Returns:
          color: if color is not specified, the current color
          color: if color is specified, the previous color
        '''
        light = __coercelight(object_id, True)
        rc = light.Diffuse
        if color:
            color = rhutil.coercecolor(color, True)
            if color!=rc:
                light.Diffuse = color
                id = rhutil.coerceguid(object_id, True)
                if not scriptcontext.doc.Lights.Modify(id, light):
                    return scriptcontext.errorhandler()
                scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the number of light objects in the document</summary>
    ///<returns>(int) the number of light objects in the document</returns>
    static member LightCount() : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def LightCount():
        '''Returns the number of light objects in the document
        Returns:
          number: the number of light objects in the document
        '''
        return scriptcontext.doc.Lights.Count
    *)


    ///<summary>Returns the direction of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Vector3d) The current direction</returns>
    static member LightDirection(objectId:Guid) : Vector3d = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LightDirection(object_id, direction=None):
        '''Returns or changes the direction of a light object
        Parameters:
          object_id (guid): the light object's identifier
          direction (vector, optional): the light's new direction
        Returns:
          vector: if direction is not specified, the current direction
          vector: if direction is specified, the previous direction
        '''
        light = __coercelight(object_id, True)
        rc = light.Direction
        if direction:
            direction = rhutil.coerce3dvector(direction, True)
            if direction!=rc:
                light.Direction = direction
                id = rhutil.coerceguid(object_id, True)
                if not scriptcontext.doc.Lights.Modify(id, light):
                    return scriptcontext.errorhandler()
                scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the direction of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="direction">(Vector3d)The light's new direction</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightDirection(objectId:Guid, direction:Vector3d) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LightDirection(object_id, direction=None):
        '''Returns or changes the direction of a light object
        Parameters:
          object_id (guid): the light object's identifier
          direction (vector, optional): the light's new direction
        Returns:
          vector: if direction is not specified, the current direction
          vector: if direction is specified, the previous direction
        '''
        light = __coercelight(object_id, True)
        rc = light.Direction
        if direction:
            direction = rhutil.coerce3dvector(direction, True)
            if direction!=rc:
                light.Direction = direction
                id = rhutil.coerceguid(object_id, True)
                if not scriptcontext.doc.Lights.Modify(id, light):
                    return scriptcontext.errorhandler()
                scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the location of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Point3d) The current location</returns>
    static member LightLocation(objectId:Guid) : Point3d = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LightLocation(object_id, location=None):
        '''Returns or changes the location of a light object
        Parameters:
          object_id (guid): the light object's identifier
          location (point, optional): the light's new location
        Returns:
          point: if location is not specified, the current location
          point: if location is specified, the previous location
        '''
        light = __coercelight(object_id, True)
        rc = light.Location
        if location:
            location = rhutil.coerce3dpoint(location, True)
            if location!=rc:
                light.Location = location
                id = rhutil.coerceguid(object_id, True)
                if not scriptcontext.doc.Lights.Modify(id, light):
                    return scriptcontext.errorhandler()
                scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the location of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="location">(Point3d)The light's new location</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightLocation(objectId:Guid, location:Point3d) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LightLocation(object_id, location=None):
        '''Returns or changes the location of a light object
        Parameters:
          object_id (guid): the light object's identifier
          location (point, optional): the light's new location
        Returns:
          point: if location is not specified, the current location
          point: if location is specified, the previous location
        '''
        light = __coercelight(object_id, True)
        rc = light.Location
        if location:
            location = rhutil.coerce3dpoint(location, True)
            if location!=rc:
                light.Location = location
                id = rhutil.coerceguid(object_id, True)
                if not scriptcontext.doc.Lights.Modify(id, light):
                    return scriptcontext.errorhandler()
                scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the name of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(string) The current name</returns>
    static member LightName(objectId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def LightName(object_id, name=None):
        '''Returns or changes the name of a light object
        Parameters:
          object_id (guid): the light object's identifier
          name (str, optional): the light's new name
        Returns:
          str: if name is not specified, the current name
          str: if name is specified, the previous name
        '''
        light = __coercelight(object_id, True)
        rc = light.Name
        if name and name!=rc:
            light.Name = name
            id = rhutil.coerceguid(object_id, True)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the name of a light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="name">(string)The light's new name</param>
    ///<returns>(unit) void, nothing</returns>
    static member LightName(objectId:Guid, name:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def LightName(object_id, name=None):
        '''Returns or changes the name of a light object
        Parameters:
          object_id (guid): the light object's identifier
          name (str, optional): the light's new name
        Returns:
          str: if name is not specified, the current name
          str: if name is specified, the previous name
        '''
        light = __coercelight(object_id, True)
        rc = light.Name
        if name and name!=rc:
            light.Name = name
            id = rhutil.coerceguid(object_id, True)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns list of identifiers of light objects in the document</summary>
    ///<returns>(Guid seq) the list of identifiers of light objects in the document</returns>
    static member LightObjects() : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def LightObjects():
        '''Returns list of identifiers of light objects in the document
        Returns:
          list(guid, ...): the list of identifiers of light objects in the document
        '''
        count = scriptcontext.doc.Lights.Count
        rc = []
        for i in range(count):
            rhlight = scriptcontext.doc.Lights[i]
            if not rhlight.IsDeleted: rc.append(rhlight.Id)
        return rc
    *)


    ///<summary>Returns the plane of a rectangular light object</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(Plane) the plane</returns>
    static member RectangularLightPlane(objectId:Guid) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def RectangularLightPlane(object_id):
        '''Returns the plane of a rectangular light object
        Parameters:
          object_id (guid): the light object's identifier
        Returns:
          plane: the plane if successful
          None: on error
        '''
        light = __coercelight(object_id, True)
        if light.LightStyle!=Rhino.Geometry.LightStyle.WorldRectangular:
            return scriptcontext.errorhandler()
        location = light.Location
        length = light.Length
        width = light.Width
        direction = light.Direction
        plane = Rhino.Geometry.Plane(location, length, width)
        return plane, (length.Length, width.Length)
    *)


    ///<summary>Returns the hardness of a spot light. Spotlight hardness
    /// controls the fully illuminated region.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current hardness</returns>
    static member SpotLightHardness(objectId:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def SpotLightHardness(object_id, hardness=None):
        '''Returns or changes the hardness of a spot light. Spotlight hardness
        controls the fully illuminated region.
        Parameters:
          object_id (guid): the light object's identifier
          hardness (number, optional): the light's new hardness
        Returns:
          number: if hardness is not specified, the current hardness
          number: if hardness is specified, the previous hardness
        '''
        light = __coercelight(object_id, True)
        if light.LightStyle!=Rhino.Geometry.LightStyle.WorldSpot:
            return scriptcontext.errorhandler()
        rc = light.HotSpot
        if hardness and hardness!=rc:
            light.HotSpot = hardness
            id = rhutil.coerceguid(object_id, True)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the hardness of a spot light. Spotlight hardness
    /// controls the fully illuminated region.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="hardness">(float)The light's new hardness</param>
    ///<returns>(unit) void, nothing</returns>
    static member SpotLightHardness(objectId:Guid, hardness:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def SpotLightHardness(object_id, hardness=None):
        '''Returns or changes the hardness of a spot light. Spotlight hardness
        controls the fully illuminated region.
        Parameters:
          object_id (guid): the light object's identifier
          hardness (number, optional): the light's new hardness
        Returns:
          number: if hardness is not specified, the current hardness
          number: if hardness is specified, the previous hardness
        '''
        light = __coercelight(object_id, True)
        if light.LightStyle!=Rhino.Geometry.LightStyle.WorldSpot:
            return scriptcontext.errorhandler()
        rc = light.HotSpot
        if hardness and hardness!=rc:
            light.HotSpot = hardness
            id = rhutil.coerceguid(object_id, True)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the radius of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current radius</returns>
    static member SpotLightRadius(objectId:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def SpotLightRadius(object_id, radius=None):
        '''Returns or changes the radius of a spot light.
        Parameters:
          object_id (guid): the light object's identifier
          radius (number, optional): the light's new radius
        Returns:
          number: if radius is not specified, the current radius
          number: if radius is specified, the previous radius
        '''
        light = __coercelight(object_id, True)
        if light.LightStyle!=Rhino.Geometry.LightStyle.WorldSpot:
            return scriptcontext.errorhandler()
        radians = light.SpotAngleRadians
        rc = light.Direction.Length * math.tan(radians)
        if radius and radius!=rc:
            radians = math.atan(radius/light.Direction.Length)
            light.SpotAngleRadians = radians
            id = rhutil.coerceguid(object_id, True)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the radius of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="radius">(float)The light's new radius</param>
    ///<returns>(unit) void, nothing</returns>
    static member SpotLightRadius(objectId:Guid, radius:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def SpotLightRadius(object_id, radius=None):
        '''Returns or changes the radius of a spot light.
        Parameters:
          object_id (guid): the light object's identifier
          radius (number, optional): the light's new radius
        Returns:
          number: if radius is not specified, the current radius
          number: if radius is specified, the previous radius
        '''
        light = __coercelight(object_id, True)
        if light.LightStyle!=Rhino.Geometry.LightStyle.WorldSpot:
            return scriptcontext.errorhandler()
        radians = light.SpotAngleRadians
        rc = light.Direction.Length * math.tan(radians)
        if radius and radius!=rc:
            radians = math.atan(radius/light.Direction.Length)
            light.SpotAngleRadians = radians
            id = rhutil.coerceguid(object_id, True)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the shadow intensity of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<returns>(float) The current intensity</returns>
    static member SpotLightShadowIntensity(objectId:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def SpotLightShadowIntensity(object_id, intensity=None):
        '''Returns or changes the shadow intensity of a spot light.
        Parameters:
          object_id (guid): the light object's identifier
          intensity (number, optional): the light's new intensity
        Returns:
          number: if intensity is not specified, the current intensity
          number: if intensity is specified, the previous intensity
        '''
        light = __coercelight(object_id, True)
        if light.LightStyle!=Rhino.Geometry.LightStyle.WorldSpot:
            return scriptcontext.errorhandler()
        rc = light.SpotLightShadowIntensity
        if intensity and intensity!=rc:
            light.SpotLightShadowIntensity = intensity
            id = rhutil.coerceguid(object_id, True)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the shadow intensity of a spot light.</summary>
    ///<param name="objectId">(Guid) The light object's identifier</param>
    ///<param name="intensity">(float)The light's new intensity</param>
    ///<returns>(unit) void, nothing</returns>
    static member SpotLightShadowIntensity(objectId:Guid, intensity:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def SpotLightShadowIntensity(object_id, intensity=None):
        '''Returns or changes the shadow intensity of a spot light.
        Parameters:
          object_id (guid): the light object's identifier
          intensity (number, optional): the light's new intensity
        Returns:
          number: if intensity is not specified, the current intensity
          number: if intensity is specified, the previous intensity
        '''
        light = __coercelight(object_id, True)
        if light.LightStyle!=Rhino.Geometry.LightStyle.WorldSpot:
            return scriptcontext.errorhandler()
        rc = light.SpotLightShadowIntensity
        if intensity and intensity!=rc:
            light.SpotLightShadowIntensity = intensity
            id = rhutil.coerceguid(object_id, True)
            if not scriptcontext.doc.Lights.Modify(id, light):
                return scriptcontext.errorhandler()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


