namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsGeometry =
  [<EXT>] 
  type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Create a clipping plane for visibly clipping away geometry in a specific
    ///  view. Note, clipping planes are infinite</summary>
    ///<param name="plane">(Plane) The plane</param>
    ///<param name="uMagnitude">(float) U magnitude of 'size of the plane' (FIXME 0)</param>
    ///<param name="vMagnitude">(float) V magnitude of 'size of the plane' (FIXME 0)</param>
    ///<param name="views">(string seq) Optional, Default Value: <c>null:string seq</c>
    ///Titles or ids the the view(s) to clip. If omitted, the active
    ///  view is used.</param>
    ///<returns>(Guid) object identifier on success</returns>
    static member AddClippingPlane(plane:Plane, uMagnitude:float, vMagnitude:float, [<OPT;DEF(null:string seq)>]views:string seq) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddClippingPlane(plane, u_magnitude, v_magnitude, views=None):
        '''Create a clipping plane for visibly clipping away geometry in a specific
        view. Note, clipping planes are infinite
        Parameters:
          plane (plane): the plane
          u_magnitude, v_magnitude (number): size of the plane
          views ([str|guid, ...]): Titles or ids the the view(s) to clip. If omitted, the active
            view is used.
        Returns:
          guid: object identifier on success
          None: on failure
        '''
        viewlist = []
        if views:
            if type(views) is System.Guid:
                viewlist.append(views)
            elif type(views) is str:
                modelviews = scriptcontext.doc.Views.GetViewList(True, False)
                rc = None
                for item in modelviews:
                    if item.ActiveViewport.Name == views:
                        id = item.ActiveViewportID
                        rc = AddClippingPlane(plane, u_magnitude, v_magnitude, id)
                        break
                return rc
            else:
                if type(views[0]) is System.Guid:
                    viewlist = views
                elif( type(views[0]) is str ):
                    modelviews = scriptcontext.doc.Views.GetViewList(True,False)
                    for viewname in views:
                        for item in modelviews:
                            if item.ActiveViewport.Name==viewname:
                                viewlist.append(item.ActiveViewportID)
                                break
        else:
            viewlist.append(scriptcontext.doc.Views.ActiveView.ActiveViewportID)
        if not viewlist: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddClippingPlane(plane, u_magnitude, v_magnitude, viewlist)
        if rc==System.Guid.Empty: raise Exception("unable to add clipping plane to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Creates a picture frame and adds it to the document.</summary>
    ///<param name="plane">(Plane) The plane in which the PictureFrame will be created.  The bottom-left corner of picture will be at plane's origin. The width will be in the plane's X axis direction, and the height will be in the plane's Y axis direction.</param>
    ///<param name="filename">(string) The path to a bitmap or image file.</param>
    ///<param name="width">(float) Optional, Default Value: <c>0.0</c>
    ///If both dblWidth and dblHeight = 0, then the width and height of the PictureFrame will be the width and height of the image. If dblWidth = 0 and dblHeight is > 0, or if dblWidth > 0 and dblHeight = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, which ever one is = 0. If both dblWidth and dblHeight are > 0, then these are assumed to be the width and height of in the current unit system.</param>
    ///<param name="height">(float) Optional, Default Value: <c>0.0</c>
    ///If both dblWidth and dblHeight = 0, then the width and height of the PictureFrame will be the width and height of the image. If dblWidth = 0 and dblHeight is > 0, or if dblWidth > 0 and dblHeight = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, which ever one is = 0. If both dblWidth and dblHeight are > 0, then these are assumed to be the width and height of in the current unit system.</param>
    ///<param name="selfIllumination">(bool) Optional, Default Value: <c>true</c>
    ///If True, then the image mapped to the picture frame plane always displays at full intensity and is not affected by light or shadow.</param>
    ///<param name="embed">(bool) Optional, Default Value: <c>false</c>
    ///If True, then the function adds the image to Rhino's internal bitmap table, thus making the document self-contained.</param>
    ///<param name="useAlpha">(bool) Optional, Default Value: <c>false</c>
    ///If False, the picture frame is created without any transparency texture.  If True, a transparency texture is created with a "mask texture" set to alpha, and an instance of the diffuse texture in the source texture slot.</param>
    ///<param name="makeMesh">(bool) Optional, Default Value: <c>false</c>
    ///If True, the function will make a PictureFrame object from a mesh rather than a plane surface.</param>
    ///<returns>(Guid) object identifier on success</returns>
    static member AddPictureFrame(plane:Plane, filename:string, [<OPT;DEF(0.0)>]width:float, [<OPT;DEF(0.0)>]height:float, [<OPT;DEF(true)>]selfIllumination:bool, [<OPT;DEF(false)>]embed:bool, [<OPT;DEF(false)>]useAlpha:bool, [<OPT;DEF(false)>]makeMesh:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
  


    [<EXT>]
    ///<summary>Adds point object to the document.</summary>
    ///<param name="pointOrX">(float) A point3d or X location of point to add</param>
    ///<param name="y">(float) Optional, Default Value: <c>7e89</c>
    ///Y location of point to add</param>
    ///<param name="z">(float) Optional, Default Value: <c>7e89</c>
    ///Z location of point to add</param>
    ///<returns>(Guid) identifier for the object that was added to the doc</returns>
    static member AddPoint(pointOrX:float, [<OPT;DEF(7e89)>]y:float, [<OPT;DEF(7e89)>]z:float) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddPoint(pointOrX, y=None, z=None):
        '''Adds point object to the document.
        Parameters:
          pointOrX (point|number): a point3d or X location of point to add
          y (number, optional): Y location of point to add
          z (number, optional): Z location of point to add
        Returns:
          guid: identifier for the object that was added to the doc
        '''
        if y is not None: point = Rhino.Geometry.Point3d(pointOrX, y, z or 0.0)
        point = rhutil.coerce3dpoint(point, True)
        rc = scriptcontext.doc.Objects.AddPoint(point)
        if rc==System.Guid.Empty: raise Exception("unable to add point to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds point cloud object to the document</summary>
    ///<param name="points">(Point3d seq) List of values where every multiple of three represents a point</param>
    ///<param name="colors">(Drawing.Color seq) Optional, Default Value: <c>null:Drawing.Color seq</c>
    ///List of colors to apply to each point</param>
    ///<returns>(Guid) identifier of point cloud on success</returns>
    static member AddPointCloud(points:Point3d seq, [<OPT;DEF(null:Drawing.Color seq)>]colors:Drawing.Color seq) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddPointCloud(points, colors=None):
        '''Adds point cloud object to the document
        Parameters:
          points ([point, ....]): list of values where every multiple of three represents a point
          colors ([color, ...]): list of colors to apply to each point
        Returns:
          guid: identifier of point cloud on success
        '''
        points = rhutil.coerce3dpointlist(points, True)
        if colors and len(colors)==len(points):
            pc = Rhino.Geometry.PointCloud()
            for i in range(len(points)):
                color = rhutil.coercecolor(colors[i],True)
                pc.Add(points[i],color)
            points = pc
        rc = scriptcontext.doc.Objects.AddPointCloud(points)
        if rc==System.Guid.Empty: raise Exception("unable to add point cloud to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds one or more point objects to the document</summary>
    ///<param name="points">(Point3d seq) List of points</param>
    ///<returns>(Guid seq) identifiers of the new objects on success</returns>
    static member AddPoints(points:Point3d seq) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddPoints(points):
        '''Adds one or more point objects to the document
        Parameters:
          points ([point, ...]): list of points
        Returns:
          list(guid, ...): identifiers of the new objects on success
        '''
        points = rhutil.coerce3dpointlist(points, True)
        rc = [scriptcontext.doc.Objects.AddPoint(point) for point in points]
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds a text string to the document</summary>
    ///<param name="text">(string) The text to display</param>
    ///<param name="pointOrPlane">(Plane) A 3-D point or the plane on which the text will lie.
    ///  The origin of the plane will be the origin point of the text</param>
    ///<param name="height">(float) Optional, Default Value: <c>1.0</c>
    ///The text height</param>
    ///<param name="font">(string) Optional, Default Value: <c>null:string</c>
    ///The text font</param>
    ///<param name="fontStyle">(int) Optional, Default Value: <c>0</c>
    ///Any of the following flags
    ///  0 = normal
    ///  1 = bold
    ///  2 = italic
    ///  3 = bold and italic</param>
    ///<param name="justification">(int) Optional, Default Value: <c>987654321</c>
    ///Text justification. Values may be added to create combinations.
    ///  1 = Left
    ///  2 = Center (horizontal)
    ///  4 = Right
    ///  65536 = Bottom
    ///  131072 = Middle (vertical)
    ///  262144 = Top</param>
    ///<returns>(Guid) identifier for the object that was added to the doc on success</returns>
    static member AddText(text:string, pointOrPlane:Plane, [<OPT;DEF(1.0)>]height:float, [<OPT;DEF(null:string)>]font:string, [<OPT;DEF(0)>]fontStyle:int, [<OPT;DEF(987654321)>]justification:int) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddText(text, point_or_plane, height=1.0, font=None, font_style=0, justification=None):
        '''Adds a text string to the document
        Parameters:
          text (str): the text to display
          point_or_plane (point|plane): a 3-D point or the plane on which the text will lie.
              The origin of the plane will be the origin point of the text
          height (number, optional): the text height
          font (str, optional): the text font
          font_style (number, optional): any of the following flags
             0 = normal
             1 = bold
             2 = italic
             3 = bold and italic
          justification (number, optional): text justification. Values may be added to create combinations.
             1 = Left
             2 = Center (horizontal)
             4 = Right
             65536 = Bottom
             131072 = Middle (vertical)
             262144 = Top
        Returns:
          guid: identifier for the object that was added to the doc on success
          None: on failure
        '''
        if not text: raise ValueError("text invalid")
        if not isinstance(text, str): text = str(text)
        point = rhutil.coerce3dpoint(point_or_plane)
        plane = None
        if not point: plane = rhutil.coerceplane(point_or_plane, True)
        if not plane:
            plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
            plane.Origin = point
        if font != None and type(font) != str:
          raise ValueError("font needs to be a quartet name")
        bold = (1==font_style or 3==font_style)
        italic = (2==font_style or 3==font_style)
        ds = scriptcontext.doc.DimStyles.Current
        if font == None:
          qn = ds.Font.QuartetName
          quartetBoldProp = ds.Font.Bold
          quartetItalicProp = ds.Font.Bold
        else:
          qn = font
          quartetBoldProp = False
          quartetItalicProp = False
        f = Rhino.DocObjects.Font.FromQuartetProperties(qn, quartetBoldProp, quartetItalicProp)
        if f == None:
            print("font error: there is a problem with the font {} and cannot be used to create a text entity".format(font))
            return scriptcontext.errorhandler()
        te = Rhino.Geometry.TextEntity.Create(text, plane, ds, False, 0, 0)
        te.TextHeight = height
        if font != None:
          te.Font = f
        if bold != quartetBoldProp:
            if Rhino.DocObjects.Font.FromQuartetProperties(qn, bold, False) == None:
              print("'{}' does not have a 'bold' property so it will not be set.".format(qn))
            else:
              te.SetBold(bold)
        if italic != quartetItalicProp:
            if Rhino.DocObjects.Font.FromQuartetProperties(qn, False, italic) == None:
              print("'{}' does not have an 'italic' property so it will not be set.".format(qn))
            else:
              te.SetItalic(italic)
        if justification is not None:
            h_map = [(1,0), (2,1), (4,2)]
            v_map = [(65536,5), (131072,3), (262144,0)]
            def getOneAlignFromMap(j, m, e):
                lst = []
                for k, v in m:
                    if j & k:
                        lst.append(v)
                return System.Enum.ToObject(e, lst[0]) if lst else None
            h = getOneAlignFromMap(justification, h_map, Rhino.DocObjects.TextHorizontalAlignment)
            if h != None:
                te.TextHorizontalAlignment = h
            v = getOneAlignFromMap(justification, v_map, Rhino.DocObjects.TextVerticalAlignment)
            if v != None:
                te.TextVerticalAlignment = v
        id = scriptcontext.doc.Objects.Add(te);
        if id==System.Guid.Empty: raise ValueError("unable to add text to document")
        scriptcontext.doc.Views.Redraw()
        return id
    *)


    [<EXT>]
    ///<summary>Add a text dot to the document.</summary>
    ///<param name="text">(string) String in dot</param>
    ///<param name="point">(Point3d) A 3D point identifying the origin point.</param>
    ///<returns>(Guid) The identifier of the new object</returns>
    static member AddTextDot(text:string, point:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddTextDot(text, point):
        '''Add a text dot to the document.
        Parameters:
          text (str): string in dot
          point (point): A 3D point identifying the origin point.
        Returns:
          guid: The identifier of the new object if successful
        '''
        point = rhutil.coerce3dpoint(point, True)
        if not isinstance(text, str): text = str(text)
        rc = scriptcontext.doc.Objects.AddTextDot(text, point)
        if rc==System.Guid.Empty: raise ValueError("unable to add text dot to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Compute the area of a closed curve, hatch, surface, polysurface, or mesh</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(float) area</returns>
    static member Area(objectId:Guid) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def Area(object_id):
        '''Compute the area of a closed curve, hatch, surface, polysurface, or mesh
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          number: area if successful
          None: on error
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        mp = Rhino.Geometry.AreaMassProperties.Compute(rhobj.Geometry)
        if mp is None: raise Exception("unable to compute area mass properties")
        return mp.Area
    *)


    [<EXT>]
    ///<summary>Returns either world axis-aligned or a construction plane axis-aligned
    ///  bounding box of an object or of several objects</summary>
    ///<param name="objects">(Guid seq) The identifiers of the objects</param>
    ///<param name="viewOrPlane">(string) Optional, Default Value: <c>null:string</c>
    ///Title or id of the view that contains the
    ///  construction plane to which the bounding box should be aligned -or-
    ///  user defined plane. If omitted, a world axis-aligned bounding box
    ///  will be calculated</param>
    ///<param name="inWorldCoords">(bool) Optional, Default Value: <c>true</c>
    ///Return the bounding box as world coordinates or
    ///  construction plane coordinates. Note, this option does not apply to
    ///  world axis-aligned bounding boxes.</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) Eight 3D points that define the bounding box.
    ///  Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBox(objects:Guid seq, [<OPT;DEF(null:string)>]viewOrPlane:string, [<OPT;DEF(true)>]inWorldCoords:bool) : Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def BoundingBox(objects, view_or_plane=None, in_world_coords=True):
        '''Returns either world axis-aligned or a construction plane axis-aligned
        bounding box of an object or of several objects
        Parameters:
          objects ([guid, ...]): The identifiers of the objects
          view_or_plane (str|guid): Title or id of the view that contains the
              construction plane to which the bounding box should be aligned -or-
              user defined plane. If omitted, a world axis-aligned bounding box
              will be calculated
          in_world_coords (bool, optional): return the bounding box as world coordinates or
              construction plane coordinates. Note, this option does not apply to
              world axis-aligned bounding boxes.
        Returns:
          list(point, point, point, point, point, point, point, point): Eight 3D points that define the bounding box.
               Points returned in counter-clockwise order starting with the bottom rectangle of the box.
          None: on error
        '''
        def __objectbbox(object, xform):
            geom = rhutil.coercegeometry(object, False)
            if not geom:
                pt = rhutil.coerce3dpoint(object, True)
                if xform: pt = xform * pt
                return Rhino.Geometry.BoundingBox(pt,pt)
            if xform: return geom.GetBoundingBox(xform)
            return geom.GetBoundingBox(True)
        xform = None
        plane = rhutil.coerceplane(view_or_plane)
        if plane is None and view_or_plane:
            view = view_or_plane
            modelviews = scriptcontext.doc.Views.GetStandardRhinoViews()
            for item in modelviews:
                viewport = item.MainViewport
                if type(view) is str and viewport.Name==view:
                    plane = viewport.ConstructionPlane()
                    break
                elif type(view) is System.Guid and viewport.Id==view:
                    plane = viewport.ConstructionPlane()
                    break
            if plane is None: return scriptcontext.errorhandler()
        if plane:
            xform = Rhino.Geometry.Transform.ChangeBasis(Rhino.Geometry.Plane.WorldXY, plane)
        bbox = Rhino.Geometry.BoundingBox.Empty
        if type(objects) is list or type(objects) is tuple:
            for object in objects:
                objectbbox = __objectbbox(object, xform)
                bbox = Rhino.Geometry.BoundingBox.Union(bbox,objectbbox)
        else:
            objectbbox = __objectbbox(objects, xform)
            bbox = Rhino.Geometry.BoundingBox.Union(bbox,objectbbox)
        if not bbox.IsValid: return scriptcontext.errorhandler()
        corners = list(bbox.GetCorners())
        if in_world_coords and plane is not None:
            plane_to_world = Rhino.Geometry.Transform.ChangeBasis(plane, Rhino.Geometry.Plane.WorldXY)
            for pt in corners: pt.Transform(plane_to_world)
        return corners
    *)


    [<EXT>]
    ///<summary>Compares two objects to determine if they are geometrically identical.</summary>
    ///<param name="first">(Guid) The identifier of the first object to compare.</param>
    ///<param name="second">(Guid) The identifier of the second object to compare.</param>
    ///<returns>(bool) True if the objects are geometrically identical, otherwise False.</returns>
    static member CompareGeometry(first:Guid, second:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def CompareGeometry(first, second):
        '''Compares two objects to determine if they are geometrically identical.
        Parameters:
          first (guid|geometry): The identifier of the first object to compare.
          second (guid|geometry): The identifier of the second object to compare.
        Returns:
          bool: True if the objects are geometrically identical, otherwise False.
        '''
        first_g = rhutil.coercegeometry(first, True)
        second_g = rhutil.coercegeometry(second, True)
        return Rhino.Geometry.GeometryBase.GeometryEquals(first_g, second_g)
    *)


    [<EXT>]
    ///<summary>Creates outline curves for a given text entity</summary>
    ///<param name="textId">(Guid) Identifier of Text object to explode</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///Delete the text object after the curves have been created</param>
    ///<returns>(Guid seq) of outline curves</returns>
    static member ExplodeText(textId:Guid, [<OPT;DEF(false)>]delete:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExplodeText(text_id, delete=False):
        '''Creates outline curves for a given text entity
        Parameters:
          text_id (guid): identifier of Text object to explode
          delete (bool, optional): delete the text object after the curves have been created
        Returns:
          list(guid): of outline curves
        '''
        rhobj = rhutil.coercerhinoobject(text_id, True, True)
        curves = rhobj.Geometry.Explode()
        attr = rhobj.Attributes
        rc = [scriptcontext.doc.Objects.AddCurve(curve,attr) for curve in curves]
        if delete: scriptcontext.doc.Objects.Delete(rhobj,True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Verifies that an object is a clipping plane object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given id is a clipping plane</returns>
    static member IsClippingPlane(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsClippingPlane(object_id):
        '''Verifies that an object is a clipping plane object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a clipping plane
        '''
        cp = rhutil.coercegeometry(object_id)
        return isinstance(cp, Rhino.Geometry.ClippingPlaneSurface)
    *)


    [<EXT>]
    ///<summary>Verifies an object is a point object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given id is a point</returns>
    static member IsPoint(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsPoint(object_id):
        '''Verifies an object is a point object.
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a point
        '''
        p = rhutil.coercegeometry(object_id)
        return isinstance(p, Rhino.Geometry.Point)
    *)


    [<EXT>]
    ///<summary>Verifies an object is a point cloud object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given id is a point cloud</returns>
    static member IsPointCloud(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsPointCloud(object_id):
        '''Verifies an object is a point cloud object.
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a point cloud
        '''
        pc = rhutil.coercegeometry(object_id)
        return isinstance(pc, Rhino.Geometry.PointCloud)
    *)


    [<EXT>]
    ///<summary>Verifies an object is a text object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given id is a text object</returns>
    static member IsText(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsText(object_id):
        '''Verifies an object is a text object.
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a text object
        '''
        text = rhutil.coercegeometry(object_id)
        return isinstance(text, Rhino.Geometry.TextEntity)
    *)


    [<EXT>]
    ///<summary>Verifies an object is a text dot object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given id is a text dot object</returns>
    static member IsTextDot(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsTextDot(object_id):
        '''Verifies an object is a text dot object.
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a text dot object
        '''
        td = rhutil.coercegeometry(object_id)
        return isinstance(td, Rhino.Geometry.TextDot)
    *)


    [<EXT>]
    ///<summary>Returns the point count of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(int) number of points</returns>
    static member PointCloudCount(objectId:Guid) : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudCount(object_id):
        '''Returns the point count of a point cloud object
        Parameters:
          object_id (guid): the point cloud object's identifier
        Returns:
          number: number of points if successful
        '''
        pc = rhutil.coercegeometry(object_id, True)
        if isinstance(pc, Rhino.Geometry.PointCloud): return pc.Count
    *)


    [<EXT>]
    ///<summary>Verifies that a point cloud has hidden points</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(bool) True if cloud has hidden points, otherwise False</returns>
    static member PointCloudHasHiddenPoints(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudHasHiddenPoints(object_id):
        '''Verifies that a point cloud has hidden points
        Parameters:
          object_id (guid): the point cloud object's identifier
        Returns:
          bool: True if cloud has hidden points, otherwise False
        '''
        pc = rhutil.coercegeometry(object_id, True)
        if isinstance(pc, Rhino.Geometry.PointCloud): return pc.HiddenPointCount>0
    *)


    [<EXT>]
    ///<summary>Verifies that a point cloud has point colors</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(bool) True if cloud has point colors, otherwise False</returns>
    static member PointCloudHasPointColors(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudHasPointColors(object_id):
        '''Verifies that a point cloud has point colors
        Parameters:
          object_id (guid): the point cloud object's identifier
        Returns:
          bool: True if cloud has point colors, otherwise False
        '''
        pc = rhutil.coercegeometry(object_id, True)
        if isinstance(pc, Rhino.Geometry.PointCloud): return pc.ContainsColors
    *)


    [<EXT>]
    ///<summary>Returns the hidden points of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(bool seq) List of point cloud hidden states</returns>
    static member PointCloudHidePoints(objectId:Guid) : bool seq = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudHidePoints(object_id, hidden=[]):
        '''Returns or modifies the hidden points of a point cloud object
        Parameters:
          object_id (guid): the point cloud object's identifier
          hidden ([bool, ....]): list of booleans matched to the index of points to be hidden
        Returns:
          list(bool, ....): List of point cloud hidden states
          list(bool, ....): List of point cloud hidden states
        '''
        rhobj = rhutil.coercerhinoobject(object_id)
        if rhobj: pc = rhobj.Geometry
        else: pc = rhutil.coercegeometry(object_id, True)
        if isinstance(pc, Rhino.Geometry.PointCloud):
            rc = None
            if pc.ContainsHiddenFlags: rc = [item.Hidden for item in pc]
            if hidden is None:
                pc.ClearHiddenFlags()
            elif len(hidden)==pc.Count:
                for i in range(pc.Count): pc[i].Hidden = hidden[i]
            if rhobj:
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the hidden points of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<param name="hidden">(bool seq)List of booleans matched to the index of points to be hidden</param>
    ///<returns>(bool seq) List of point cloud hidden states</returns>
    static member PointCloudHidePoints(objectId:Guid, hidden:bool seq) : bool seq = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudHidePoints(object_id, hidden=[]):
        '''Returns or modifies the hidden points of a point cloud object
        Parameters:
          object_id (guid): the point cloud object's identifier
          hidden ([bool, ....]): list of booleans matched to the index of points to be hidden
        Returns:
          list(bool, ....): List of point cloud hidden states
          list(bool, ....): List of point cloud hidden states
        '''
        rhobj = rhutil.coercerhinoobject(object_id)
        if rhobj: pc = rhobj.Geometry
        else: pc = rhutil.coercegeometry(object_id, True)
        if isinstance(pc, Rhino.Geometry.PointCloud):
            rc = None
            if pc.ContainsHiddenFlags: rc = [item.Hidden for item in pc]
            if hidden is None:
                pc.ClearHiddenFlags()
            elif len(hidden)==pc.Count:
                for i in range(pc.Count): pc[i].Hidden = hidden[i]
            if rhobj:
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the point colors of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(Drawing.Color seq) List of point cloud colors</returns>
    static member PointCloudPointColors(objectId:Guid) : Drawing.Color seq = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudPointColors(object_id, colors=[]):
        '''Returns or modifies the point colors of a point cloud object
        Parameters:
          object_id (guid): the point cloud object's identifier
          colors ([color, ...]) list of color values if you want to adjust colors
        Returns:
          list(color, ...): List of point cloud colors
          list(color, ...): List of point cloud colors
        '''
        rhobj = rhutil.coercerhinoobject(object_id)
        if rhobj: pc = rhobj.Geometry
        else: pc = rhutil.coercegeometry(object_id, True)
        if isinstance(pc, Rhino.Geometry.PointCloud):
            rc = None
            if pc.ContainsColors: rc = [item.Color for item in pc]
            if colors is None:
                pc.ClearColors()
            elif len(colors)==pc.Count:
                for i in range(pc.Count): pc[i].Color = rhutil.coercecolor(colors[i])
            if rhobj:
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the point colors of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<param name="colors">(Drawing.Color seq)List of color values if you want to adjust colors</param>
    ///<returns>(Drawing.Color seq) List of point cloud colors</returns>
    static member PointCloudPointColors(objectId:Guid, colors:Drawing.Color seq) : Drawing.Color seq = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudPointColors(object_id, colors=[]):
        '''Returns or modifies the point colors of a point cloud object
        Parameters:
          object_id (guid): the point cloud object's identifier
          colors ([color, ...]) list of color values if you want to adjust colors
        Returns:
          list(color, ...): List of point cloud colors
          list(color, ...): List of point cloud colors
        '''
        rhobj = rhutil.coercerhinoobject(object_id)
        if rhobj: pc = rhobj.Geometry
        else: pc = rhutil.coercegeometry(object_id, True)
        if isinstance(pc, Rhino.Geometry.PointCloud):
            rc = None
            if pc.ContainsColors: rc = [item.Color for item in pc]
            if colors is None:
                pc.ClearColors()
            elif len(colors)==pc.Count:
                for i in range(pc.Count): pc[i].Color = rhutil.coercecolor(colors[i])
            if rhobj:
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the points of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(Guid seq) list of points</returns>
    static member PointCloudPoints(objectId:Guid) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudPoints(object_id):
        '''Returns the points of a point cloud object
        Parameters:
          object_id (guid): the point cloud object's identifier
        Returns:
          list(guid, ...): list of points if successful
        '''
        pc = rhutil.coercegeometry(object_id, True)
        if isinstance(pc, Rhino.Geometry.PointCloud): return pc.GetPoints()
    *)


    [<EXT>]
    
    static member internal SimplifyPointCloudKNeighbors() : obj =
        failNotImpl () // genreation temp disabled !!
    (*
    def __simplify_PointCloudKNeighbors(result, amount):
        ''''''
    *)


    [<EXT>]
    ///<summary>Returns amount indices of points in a point cloud that are near needlePoints.</summary>
    ///<param name="ptCloud">(Point3d seq) The point cloud to be searched, or the "hay stack". This can also be a list of points.</param>
    ///<param name="needlePoints">(Point3d seq) A list of points to search in the point_cloud. This can also be specified as a point cloud.</param>
    ///<param name="amount">(int) Optional, Default Value: <c>1</c>
    ///The amount of required closest points. Defaults to 1.</param>
    ///<returns>(int seq) a list of indices of the found points, if amount equals 1.
    ///  [[int, ...], ...]: nested lists with amount items within a list, with the indices of the found points.</returns>
    static member PointCloudKNeighbors(ptCloud:Point3d seq, needlePoints:Point3d seq, [<OPT;DEF(1)>]amount:int) : int seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudKNeighbors(pt_cloud, needle_points, amount=1):
        '''Returns amount indices of points in a point cloud that are near needle_points.
        Parameters:
          pt_cloud (guid|[point, ...]): the point cloud to be searched, or the "hay stack". This can also be a list of points.
          needle_points (guid|[point, ...]): a list of points to search in the point_cloud. This can also be specified as a point cloud.
          amount (int, optional): the amount of required closest points. Defaults to 1.
        Returns:
          [int, int,...]: a list of indices of the found points, if amount equals 1.
            [[int, ...], ...]: nested lists with amount items within a list, with the indices of the found points.
        '''
        needles = rhutil.coercegeometry(needle_points, False)
        if isinstance(needles, Rhino.Geometry.PointCloud): needles = needles.AsReadOnlyListOfPoints()
        else: needles = rhutil.coerce3dpointlist(needle_points, True)
        pc_geom = rhutil.coercegeometry(pt_cloud, False)
        if isinstance(pc_geom, Rhino.Geometry.PointCloud):
            if len(needles) > 100:
                search = Rhino.Geometry.RTree.PointCloudKNeighbors
            else:
                search = Rhino.Collections.RhinoList.PointCloudKNeighbors
            return __simplify_PointCloudKNeighbors(search(pc_geom, needles, amount), amount)
        if len(needles) > 100:
            search = Rhino.Geometry.RTree.Point3dKNeighbors
        else:
            search = Rhino.Collections.RhinoList.Point3dKNeighbors
        if isinstance(pt_cloud, System.Collections.Generic.IEnumerable[Rhino.Geometry.Point3d]):
            return __simplify_PointCloudKNeighbors(search(pt_cloud, needles, amount), amount)
        pts = rhutil.coerce3dpointlist(pt_cloud, True)
        return __simplify_PointCloudKNeighbors(search(pts, needles, amount), amount)
    *)


    [<EXT>]
    
    static member internal SimplifyPointCloudClosestPoints() : obj =
        failNotImpl () // genreation temp disabled !!
    (*
    def __simplify_PointCloudClosestPoints(result):
        ''''''
    *)


    [<EXT>]
    ///<summary>Returns a list of lists of point indices in a point cloud that are
    ///  closest to needlePoints. Each inner list references all points within or on the surface of a sphere of distance radius.</summary>
    ///<param name="ptCloud">(Point3d seq) The point cloud to be searched, or the "hay stack". This can also be a list of points.</param>
    ///<param name="needlePoints">(Point3d seq) A list of points to search in the point_cloud. This can also be specified as a point cloud.</param>
    ///<param name="distance">(float) The included limit for listing points.</param>
    ///<returns>(int seq) a list of lists with the indices of the found points.</returns>
    static member PointCloudClosestPoints(ptCloud:Point3d seq, needlePoints:Point3d seq, distance:float) : int seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCloudClosestPoints(pt_cloud, needle_points, distance):
        '''Returns a list of lists of point indices in a point cloud that are
        closest to needle_points. Each inner list references all points within or on the surface of a sphere of distance radius.
        Parameters:
          pt_cloud (guid|[point, ...]): the point cloud to be searched, or the "hay stack". This can also be a list of points.
          needle_points (guid|[point, ...]): a list of points to search in the point_cloud. This can also be specified as a point cloud.
          distance (float): the included limit for listing points.
        Returns:
          [[int, ...], ...]: a list of lists with the indices of the found points.
        '''
        needles = rhutil.coercegeometry(needle_points, False)
        if isinstance(needles, Rhino.Geometry.PointCloud): needles = needles.AsReadOnlyListOfPoints()
        else: needles = rhutil.coerce3dpointlist(needle_points, True)
        pc_geom = rhutil.coercegeometry(pt_cloud, False)
        if isinstance(pc_geom, Rhino.Geometry.PointCloud):
            return __simplify_PointCloudClosestPoints(Rhino.Geometry.RTree.PointCloudClosestPoints(pc_geom, needles, distance))
        if isinstance(pt_cloud, System.Collections.Generic.IEnumerable[Rhino.Geometry.Point3d]):
            return __simplify_PointCloudClosestPoints(Rhino.Geometry.RTree.Point3dClosestPoints(pt_cloud, needles, distance))
        pts = rhutil.coerce3dpointlist(pt_cloud, True)
        return __simplify_PointCloudClosestPoints(Rhino.Geometry.RTree.Point3dClosestPoints(pts, needles, distance))
    *)


    [<EXT>]
    ///<summary>Returns the X, Y, and Z coordinates of a point object</summary>
    ///<param name="objectId">(Guid) The identifier of a point object</param>
    ///<returns>(Point3d) The current 3-D point location</returns>
    static member PointCoordinates(objectId:Guid) : Point3d = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCoordinates(object_id, point=None):
        '''Returns or modifies the X, Y, and Z coordinates of a point object
        Parameters:
          object_id (guid): The identifier of a point object
          point (point, optional): A new 3D point location.
        Returns:
          point: If point is not specified, the current 3-D point location
          point: If point is specified, the previous 3-D point location
        '''
        point_geometry = rhutil.coercegeometry(object_id, True)
        if isinstance(point_geometry, Rhino.Geometry.Point):
            rc = point_geometry.Location
            if point:
                point = rhutil.coerce3dpoint(point, True)
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, point)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the X, Y, and Z coordinates of a point object</summary>
    ///<param name="objectId">(Guid) The identifier of a point object</param>
    ///<param name="point">(Point3d)A new 3D point location.</param>
    ///<returns>(unit) void, nothing</returns>
    static member PointCoordinates(objectId:Guid, point:Point3d) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def PointCoordinates(object_id, point=None):
        '''Returns or modifies the X, Y, and Z coordinates of a point object
        Parameters:
          object_id (guid): The identifier of a point object
          point (point, optional): A new 3D point location.
        Returns:
          point: If point is not specified, the current 3-D point location
          point: If point is specified, the previous 3-D point location
        '''
        point_geometry = rhutil.coercegeometry(object_id, True)
        if isinstance(point_geometry, Rhino.Geometry.Point):
            rc = point_geometry.Location
            if point:
                point = rhutil.coerce3dpoint(point, True)
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, point)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the font of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<returns>(string) The current text dot font</returns>
    static member TextDotFont(objectId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextDotFont(object_id, fontface=None):
        '''Returns or modifies the font of a text dot
        Parameters:
          object_id (guid): identifier of a text dot object
          fontface (str, optional): new font face name
        Returns:
          str: If font is not specified, the current text dot font
          str: If font is specified, the previous text dot font
          None: on error
        '''
        textdot = rhutil.coercegeometry(object_id, True)
        if isinstance(textdot, Rhino.Geometry.TextDot):
            rc = textdot.FontFace
            if fontface:
                textdot.FontFace = fontface
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the font of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<param name="fontface">(string)New font face name</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotFont(objectId:Guid, fontface:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextDotFont(object_id, fontface=None):
        '''Returns or modifies the font of a text dot
        Parameters:
          object_id (guid): identifier of a text dot object
          fontface (str, optional): new font face name
        Returns:
          str: If font is not specified, the current text dot font
          str: If font is specified, the previous text dot font
          None: on error
        '''
        textdot = rhutil.coercegeometry(object_id, True)
        if isinstance(textdot, Rhino.Geometry.TextDot):
            rc = textdot.FontFace
            if fontface:
                textdot.FontFace = fontface
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the font height of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<returns>(float) The current text dot height</returns>
    static member TextDotHeight(objectId:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextDotHeight(object_id, height=None):
        '''Returns or modifies the font height of a text dot
        Parameters:
          object_id (guid): identifier of a text dot object
          height (number, optional) new font height
        Returns:
          number: If height is not specified, the current text dot height
          number: If height is specified, the previous text dot height
          None: on error
        '''
        textdot = rhutil.coercegeometry(object_id, True)
        if isinstance(textdot, Rhino.Geometry.TextDot):
            rc = textdot.FontHeight
            if height and height>0:
                textdot.FontHeight = height
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the font height of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<param name="height">(float)New font height</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotHeight(objectId:Guid, height:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextDotHeight(object_id, height=None):
        '''Returns or modifies the font height of a text dot
        Parameters:
          object_id (guid): identifier of a text dot object
          height (number, optional) new font height
        Returns:
          number: If height is not specified, the current text dot height
          number: If height is specified, the previous text dot height
          None: on error
        '''
        textdot = rhutil.coercegeometry(object_id, True)
        if isinstance(textdot, Rhino.Geometry.TextDot):
            rc = textdot.FontHeight
            if height and height>0:
                textdot.FontHeight = height
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the location, or insertion point, on a text dot object</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<returns>(Point3d) The current 3-D text dot location</returns>
    static member TextDotPoint(objectId:Guid) : Point3d = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextDotPoint(object_id, point=None):
        '''Returns or modifies the location, or insertion point, on a text dot object
        Parameters:
          object_id (guid): identifier of a text dot object
          point (point, optional): A new 3D point location.
        Returns:
          point: If point is not specified, the current 3-D text dot location
          point: If point is specified, the previous 3-D text dot location
          None: if not successful, or on error
        '''
        textdot = rhutil.coercegeometry(object_id, True)
        if isinstance(textdot, Rhino.Geometry.TextDot):
            rc = textdot.Point
            if point:
                textdot.Point = rhutil.coerce3dpoint(point, True)
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the location, or insertion point, on a text dot object</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<param name="point">(Point3d)A new 3D point location.</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotPoint(objectId:Guid, point:Point3d) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextDotPoint(object_id, point=None):
        '''Returns or modifies the location, or insertion point, on a text dot object
        Parameters:
          object_id (guid): identifier of a text dot object
          point (point, optional): A new 3D point location.
        Returns:
          point: If point is not specified, the current 3-D text dot location
          point: If point is specified, the previous 3-D text dot location
          None: if not successful, or on error
        '''
        textdot = rhutil.coercegeometry(object_id, True)
        if isinstance(textdot, Rhino.Geometry.TextDot):
            rc = textdot.Point
            if point:
                textdot.Point = rhutil.coerce3dpoint(point, True)
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the text on a text dot object</summary>
    ///<param name="objectId">(Guid) The identifier of a text dot object</param>
    ///<returns>(string) The current text dot text</returns>
    static member TextDotText(objectId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextDotText(object_id, text=None):
        '''Returns or modifies the text on a text dot object
        Parameters:
          object_id (guid): The identifier of a text dot object
          text (str, optional): a new string for the dot
        Returns:
          str: If text is not specified, the current text dot text
          str: If text is specified, the previous text dot text
          None: if not successful, or on error
        '''
        textdot = rhutil.coercegeometry(object_id, True)
        if isinstance(textdot, Rhino.Geometry.TextDot):
            rc = textdot.Text
            if text is not None:
                if not isinstance(text, str): text = str(text)
                textdot.Text = text
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the text on a text dot object</summary>
    ///<param name="objectId">(Guid) The identifier of a text dot object</param>
    ///<param name="text">(string)A new string for the dot</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotText(objectId:Guid, text:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextDotText(object_id, text=None):
        '''Returns or modifies the text on a text dot object
        Parameters:
          object_id (guid): The identifier of a text dot object
          text (str, optional): a new string for the dot
        Returns:
          str: If text is not specified, the current text dot text
          str: If text is specified, the previous text dot text
          None: if not successful, or on error
        '''
        textdot = rhutil.coercegeometry(object_id, True)
        if isinstance(textdot, Rhino.Geometry.TextDot):
            rc = textdot.Text
            if text is not None:
                if not isinstance(text, str): text = str(text)
                textdot.Text = text
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the font used by a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(string) The current font face name</returns>
    static member TextObjectFont(objectId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectFont(object_id, font=None):
        '''Returns or modifies the font used by a text object
        Parameters:
          object_id (guid): the identifier of a text object
          font (str): the new font face name
        Returns:
          str: if a font is not specified, the current font face name
          str: if a font is specified, the previous font face name
          None: if not successful, or on error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        fontdata = annotation.Font
        rc = fontdata.FaceName
        if font:
            index = scriptcontext.doc.Fonts.FindOrCreate( font, fontdata.Bold, fontdata.Italic )
            annotation.Font = scriptcontext.doc.Fonts[index]
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the font used by a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="font">(string)The new font face name</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectFont(objectId:Guid, font:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectFont(object_id, font=None):
        '''Returns or modifies the font used by a text object
        Parameters:
          object_id (guid): the identifier of a text object
          font (str): the new font face name
        Returns:
          str: if a font is not specified, the current font face name
          str: if a font is specified, the previous font face name
          None: if not successful, or on error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        fontdata = annotation.Font
        rc = fontdata.FaceName
        if font:
            index = scriptcontext.doc.Fonts.FindOrCreate( font, fontdata.Bold, fontdata.Italic )
            annotation.Font = scriptcontext.doc.Fonts[index]
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the height of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(float) The current text height</returns>
    static member TextObjectHeight(objectId:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectHeight(object_id, height=None):
        '''Returns or modifies the height of a text object
        Parameters:
          object_id (guid): the identifier of a text object
          height (number, optional): the new text height.
        Returns:
          number: if height is not specified, the current text height
          number: if height is specified, the previous text height
          None: if not successful, or on error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        rc = annotation.TextHeight
        if height:
            annotation.TextHeight = height
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the height of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="height">(float)The new text height.</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectHeight(objectId:Guid, height:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectHeight(object_id, height=None):
        '''Returns or modifies the height of a text object
        Parameters:
          object_id (guid): the identifier of a text object
          height (number, optional): the new text height.
        Returns:
          number: if height is not specified, the current text height
          number: if height is specified, the previous text height
          None: if not successful, or on error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        rc = annotation.TextHeight
        if height:
            annotation.TextHeight = height
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the plane used by a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(Plane) The current plane</returns>
    static member TextObjectPlane(objectId:Guid) : Plane = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectPlane(object_id, plane=None):
        '''Returns or modifies the plane used by a text object
        Parameters:
          object_id (guid): the identifier of a text object
          plane (plane): the new text object plane
        Returns:
          plane: if a plane is not specified, the current plane if successful
          plane: if a plane is specified, the previous plane if successful
          None: if not successful, or on Error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        rc = annotation.Plane
        if plane:
            annotation.Plane = rhutil.coerceplane(plane, True)
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the plane used by a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="plane">(Plane)The new text object plane</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectPlane(objectId:Guid, plane:Plane) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectPlane(object_id, plane=None):
        '''Returns or modifies the plane used by a text object
        Parameters:
          object_id (guid): the identifier of a text object
          plane (plane): the new text object plane
        Returns:
          plane: if a plane is not specified, the current plane if successful
          plane: if a plane is specified, the previous plane if successful
          None: if not successful, or on Error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        rc = annotation.Plane
        if plane:
            annotation.Plane = rhutil.coerceplane(plane, True)
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the location of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(Point3d) The 3D point identifying the current location</returns>
    static member TextObjectPoint(objectId:Guid) : Point3d = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectPoint(object_id, point=None):
        '''Returns or modifies the location of a text object
        Parameters:
          object_id (guid): the identifier of a text object
          point (point, optional) the new text object location
        Returns:
          point: if point is not specified, the 3D point identifying the current location
          point: if point is specified, the 3D point identifying the previous location
          None: if not successful, or on Error
        '''
        text = rhutil.coercegeometry(object_id, True)
        if not isinstance(text, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        plane = text.Plane
        rc = plane.Origin
        if point:
            plane.Origin = rhutil.coerce3dpoint(point, True)
            text.Plane = plane
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, text)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the location of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="point">(Point3d)The new text object location</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectPoint(objectId:Guid, point:Point3d) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectPoint(object_id, point=None):
        '''Returns or modifies the location of a text object
        Parameters:
          object_id (guid): the identifier of a text object
          point (point, optional) the new text object location
        Returns:
          point: if point is not specified, the 3D point identifying the current location
          point: if point is specified, the 3D point identifying the previous location
          None: if not successful, or on Error
        '''
        text = rhutil.coercegeometry(object_id, True)
        if not isinstance(text, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        plane = text.Plane
        rc = plane.Origin
        if point:
            plane.Origin = rhutil.coerce3dpoint(point, True)
            text.Plane = plane
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, text)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the font style of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(int) The current font style
    ///  0 = Normal
    ///  1 = Bold
    ///  2 = Italic</returns>
    static member TextObjectStyle(objectId:Guid) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectStyle(object_id, style=None):
        '''Returns or modifies the font style of a text object
        Parameters:
          object_id (guid) the identifier of a text object
          style (number, optional) the font style. Can be any of the following flags
             0 = Normal
             1 = Bold
             2 = Italic
        Returns:
          number: if style is not specified, the current font style
          number: if style is specified, the previous font style
          None: if not successful, or on Error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        fontdata = annotation.Font
        if fontdata is None: return scriptcontext.errorhandler()
        rc = 0
        if fontdata.Bold: rc += 1
        if fontdata.Italic: rc += 2
        if style is not None and style!=rc:
            index = scriptcontext.doc.Fonts.FindOrCreate( fontdata.FaceName, (style&1)==1, (style&2)==2 )
            annotation.Font = scriptcontext.doc.Fonts[index]
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the font style of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="style">(int)The font style. Can be any of the following flags
    ///  0 = Normal
    ///  1 = Bold
    ///  2 = Italic</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectStyle(objectId:Guid, style:int) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectStyle(object_id, style=None):
        '''Returns or modifies the font style of a text object
        Parameters:
          object_id (guid) the identifier of a text object
          style (number, optional) the font style. Can be any of the following flags
             0 = Normal
             1 = Bold
             2 = Italic
        Returns:
          number: if style is not specified, the current font style
          number: if style is specified, the previous font style
          None: if not successful, or on Error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        fontdata = annotation.Font
        if fontdata is None: return scriptcontext.errorhandler()
        rc = 0
        if fontdata.Bold: rc += 1
        if fontdata.Italic: rc += 2
        if style is not None and style!=rc:
            index = scriptcontext.doc.Fonts.FindOrCreate( fontdata.FaceName, (style&1)==1, (style&2)==2 )
            annotation.Font = scriptcontext.doc.Fonts[index]
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the text string of a text object.</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(string) The current string value</returns>
    static member TextObjectText(objectId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectText(object_id, text=None):
        '''Returns or modifies the text string of a text object.
        Parameters:
          object_id (guid): the identifier of a text object
          text (str, optional): a new text string
        Returns:
          str: if text is not specified, the current string value if successful
          str: if text is specified, the previous string value if successful
          None: if not successful, or on error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        rc = annotation.Text
        if text:
            if not isinstance(text, str): text = str(text)
            annotation.Text = text
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the text string of a text object.</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="text">(string)A new text string</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectText(objectId:Guid, text:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def TextObjectText(object_id, text=None):
        '''Returns or modifies the text string of a text object.
        Parameters:
          object_id (guid): the identifier of a text object
          text (str, optional): a new text string
        Returns:
          str: if text is not specified, the current string value if successful
          str: if text is specified, the previous string value if successful
          None: if not successful, or on error
        '''
        annotation = rhutil.coercegeometry(object_id, True)
        if not isinstance(annotation, Rhino.Geometry.TextEntity):
            return scriptcontext.errorhandler()
        rc = annotation.Text
        if text:
            if not isinstance(text, str): text = str(text)
            annotation.Text = text
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


