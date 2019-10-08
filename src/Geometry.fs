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
        let viewlist = 
            if isNull views then [Doc.Views.ActiveView.ActiveViewportID]
            else
                let modelviews = Doc.Views.GetViewList(true, false)
                [for view in views do
                    for item in modelviews do
                        if item.ActiveViewport.Name = view then
                            yield item.ActiveViewportID]
        let rc = Doc.Objects.AddClippingPlane(plane, uMagnitude, vMagnitude, viewlist)
        if rc = Guid.Empty then failwithf "Scripting: Unable to add clipping plane to document.  plane:'%A' uMagnitude:'%A' vMagnitude:'%A' views:'%A'" plane uMagnitude vMagnitude views
        Doc.Views.Redraw()
        rc
    (*
    def AddClippingPlane(plane, umagnitude, vmagnitude, views=None):
        '''Create a clipping plane for visibly clipping away geometry in a specific
        view. Note, clipping planes are infinite
        Parameters:
          plane (plane): the plane
          umagnitude, vmagnitude (number): size of the plane
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
                        rc = AddClippingPlane(plane, umagnitude, vmagnitude, id)
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
        rc = scriptcontext.doc.Objects.AddClippingPlane(plane, umagnitude, vmagnitude, viewlist)
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
      if not <| IO.File.Exists(filename) then failwithf "image %s does not exist" filename
      let rc = Doc.Objects.AddPictureFrame(plane, filename, makeMesh, width, height, selfIllumination, embed)
      if rc = Guid.Empty then failwithf "Scripting: Unable to add picture frame to document.  plane:'%A' filename:'%A' width:'%A' height:'%A' selfIllumination:'%A' embed:'%A' useAlpha:'%A' makeMesh:'%A'" plane filename width height selfIllumination embed useAlpha makeMesh
      Doc.Views.Redraw()
      rc
    (*
    def AddPictureFrame(plane, filename, width=0.0, height=0.0, selfillumination=True, embed=False, usealpha=False, makemesh=False):
        '''Creates a picture frame and adds it to the document.
      Parameters:
        plane (plane): The plane in which the PictureFrame will be created.  The bottom-left corner of picture will be at plane's origin. The width will be in the plane's X axis direction, and the height will be in the plane's Y axis direction.
        filename (str): The path to a bitmap or image file.
        width (number, optional): If both dblWidth and dblHeight = 0, then the width and height of the PictureFrame will be the width and height of the image. If dblWidth = 0 and dblHeight is > 0, or if dblWidth > 0 and dblHeight = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, which ever one is = 0. If both dblWidth and dblHeight are > 0, then these are assumed to be the width and height of in the current unit system.
        height (number, optional):  If both dblWidth and dblHeight = 0, then the width and height of the PictureFrame will be the width and height of the image. If dblWidth = 0 and dblHeight is > 0, or if dblWidth > 0 and dblHeight = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, which ever one is = 0. If both dblWidth and dblHeight are > 0, then these are assumed to be the width and height of in the current unit system.
        selfillumination (bool, optional): If True, then the image mapped to the picture frame plane always displays at full intensity and is not affected by light or shadow.
        embed (bool, optional): If True, then the function adds the image to Rhino's internal bitmap table, thus making the document self-contained.
        usealpha (bool, optional): If False, the picture frame is created without any transparency texture.  If True, a transparency texture is created with a "mask texture" set to alpha, and an instance of the diffuse texture in the source texture slot.
        makemesh (bool, optional): If True, the function will make a PictureFrame object from a mesh rather than a plane surface.
      Returns:
        guid: object identifier on success
        None: on failure
      '''
    
      plane = rhutil.coerceplane(plane, True)
      if type(filename) is not System.String or not System.IO.File.Exists(filename): raise Exception( does not exist or is not a file name'.format(filename))
      rc = scriptcontext.doc.Objects.AddPictureFrame(plane, filename, makemesh, width, height, selfillumination, embed)
      if rc==System.Guid.Empty: raise Exception("unable to add picture frame to document")
      scriptcontext.doc.Views.Redraw()
      return rc
    *)

    [<EXT>]
    ///<summary>Adds point object to the document.</summary>
    ///<param name="X">(float) X location of point to add</param>
    ///<param name="y">(float)Y location of point to add</param>
    ///<param name="z">(float) Z location of point to add</param>
    ///<returns>(Guid) identifier for the object that was added to the doc</returns>
    static member AddPoint(x:float, y:float, z:float) : Guid =
        let rc = Doc.Objects.AddPoint(Point3d(x,y,x))
        if rc = Guid.Empty then failwithf "Scripting: Unable to add point to document.  x:'%A' y:'%A' z:'%A'" x y z
        Doc.Views.Redraw()
        rc

    [<EXT>]
    ///<summary>Adds point object to the document.</summary>
    ///<param name="point">(Point3d) point to draw</param>   
    ///<returns>(Guid) identifier for the object that was added to the doc</returns>
    static member AddPoint(point:Point3d) : Guid =
        let rc = Doc.Objects.AddPoint(point)
        if rc = Guid.Empty then failwithf "Scripting: Unable to add point to document.  point:'%A' " point
        Doc.Views.Redraw()
        rc
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
    
        if y is not None: point = Geometry.Point3d(pointOrX, y, z or 0.0)
        point = rhutil.coerce3dpoint(point, True)
        rc = scriptcontext.doc.Objects.AddPoint(point)
        if rc==System.Guid.Empty: raise Exception("unable to add point to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds point cloud object to the document</summary>
    ///<param name="points">(Point3d array) List of values where every multiple of three represents a point</param>
    ///<param name="colors">(Drawing.Color array) Optional, Default Value: <c>null:Drawing.Color seq</c>
    ///List of colors to apply to each point</param>
    ///<returns>(Guid) identifier of point cloud on success</returns>
    static member AddPointCloud(points:Point3d [], [<OPT;DEF(null:Drawing.Color seq)>]colors:Drawing.Color []) : Guid =
        if notNull colors && Seq.length(colors) = Seq.length(points) then
            let pc = new PointCloud()
            for i = 0  to -1 + (Seq.length(points)) do
                let color = RhinoScriptSyntax.CoerceColor(colors.[i])
                pc.Add(points.[i],color)            
            let rc = Doc.Objects.AddPointCloud(pc)
            if rc = Guid.Empty then failwithf "Scripting: Unable to add point cloud to document.  points:'%A' colors:'%A'" points colors
            Doc.Views.Redraw()
            rc
        else
            let rc = Doc.Objects.AddPointCloud(points)
            if rc = Guid.Empty then failwithf "Scripting: Unable to add point cloud to document.  points:'%A' colors:'%A'" points colors
            Doc.Views.Redraw()
            rc
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
        if colors and Seq.length(colors)==len(points):
            pc = Geometry.PointCloud()
            for i = 0  to -1 + (Seq.length(points)):
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
    ///<returns>(Guid ResizeArray) List of identifiers of the new objects on success</returns>
    static member AddPoints(points:Point3d seq) : Guid ResizeArray =
        let rc = resizeArray{ for point in points do yield Doc.Objects.AddPoint(point) }
        Doc.Views.Redraw()
        rc
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
    ///<param name="plane">(Plane) the plane on which the text will lie.
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
    ///<param name="horizontalAlignment">(DocObjects.TextHorizontalAlignment) Optional, Default Value: <c>DocObjects.TextHorizontalAlignment.Left</c></param>
    ///<param name="verticalAlignment">(DocObjects.TextVerticalAlignment) Optional, Default Value: <c>DocObjects.TextVerticalAlignment.Top</c></param>
    ///<returns>(Guid) identifier for the object that was added to the doc on success</returns>
    static member AddText(  text:string, 
                            plane:Plane, 
                            [<OPT;DEF(1.0)>]height:float, 
                            [<OPT;DEF(null:string)>]font:string,
                            [<OPT;DEF(0)>]fontStyle:int, 
                            [<OPT;DEF(1)>]horizontalAlignment:DocObjects.TextHorizontalAlignment, 
                            [<OPT;DEF(1)>]verticalAlignment:DocObjects.TextVerticalAlignment) : Guid =
        if isNull text || text = "" then failwithf "Scripting: Text invalid.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
        let bold = (1 = fontStyle || 3 = fontStyle)
        let italic = (2 = fontStyle || 3 = fontStyle)
        let ds = Doc.DimStyles.Current
        let qn, quartetBoldProp ,quartetItalicProp =
            if isNull font then              
              ds.Font.QuartetName, ds.Font.Bold, ds.Font.Italic
            else 
              font,false,false
        
        let f = DocObjects.Font.FromQuartetProperties(qn, quartetBoldProp, quartetItalicProp)

        if isNull f then
            failwithf "Scripting: AddText failed.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
        let te = TextEntity.Create(text, plane, ds, false, 0.0, 0.0)
        te.TextHeight <- height
        if font |> notNull then
          te.Font <- f
        if bold <> quartetBoldProp then
            if DocObjects.Font.FromQuartetProperties(qn, bold, false) |> isNull then
              failwithf "Scripting: AddText failed.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
            else 
              te.SetBold(bold)|> ignore
        if italic <> quartetItalicProp then
            if DocObjects.Font.FromQuartetProperties(qn, false, italic) |> isNull then
              failwithf "Scripting: AddText failed.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
            else 
              te.SetItalic(italic)|> ignore
       
        te.TextHorizontalAlignment <- horizontalAlignment
        te.TextVerticalAlignment <- verticalAlignment
        let id = Doc.Objects.Add(te);
        if id = Guid.Empty then failwithf "Scripting: Unable to add text to document.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
        Doc.Views.Redraw()
        id
    (*
    def AddText(text, point_orplane, height=1.0, font=None, fontstyle=0, justification=None):
        '''Adds a text string to the document
        Parameters:
          text (str): the text to display
          point_orplane (point|plane): a 3-D point or the plane on which the text will lie.
              The origin of the plane will be the origin point of the text
          height (number, optional): the text height
          font (str, optional): the text font
          fontstyle (number, optional): any of the following flags
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
        if not text :? str: text = str(text)
        point = rhutil.coerce3dpoint(point_orplane)
        plane = None
        if not point: plane = rhutil.coerceplane(point_orplane, True)
        if not plane:
            plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
            plane.Origin = point
        if font != None and type(font) != str:
          raise ValueError("font needs to be a quartet name")
        bold = (1==fontstyle or 3==fontstyle)
        italic = (2==fontstyle or 3==fontstyle)
    
        ds = scriptcontext.doc.DimStyles.Current
        if font == None:
          qn = ds.Font.QuartetName
          quartetBoldProp = ds.Font.Bold
          quartetItalicProp = ds.Font.Bold
        else:
          qn = font
          quartetBoldProp = False
          quartetItalicProp = False
    
        f = DocObjects.Font.FromQuartetProperties(qn, quartetBoldProp, quartetItalicProp)
        if f == None:
            print("font error: there is a problem with the font {} and cannot be used to create a text entity".format(font))
            return scriptcontext.errorhandler()
    
        te = Geometry.TextEntity.Create(text, plane, ds, False, 0, 0)
        te.TextHeight = height
    
        if font != None:
          te.Font = f
    
        if bold != quartetBoldProp:
            if DocObjects.Font.FromQuartetProperties(qn, bold, False) == None:
              print("'{}' does not have a 'bold' property so it will not be set.".format(qn))
            else:
              te.SetBold(bold)
        if italic != quartetItalicProp:
            if DocObjects.Font.FromQuartetProperties(qn, False, italic) == None:
              print("'{}' does not have an 'italic' property so it will not be set.".format(qn))
            else:
              te.SetItalic(italic)
    
        if justification is not None:
            hmap = [(1,0), (2,1), (4,2)]
            vmap = [(65536,5), (131072,3), (262144,0)]
    
            def getOneAlignFromMap(j, m, e):
                lst = []
                for k, v in m:
                    if j & k:
                        lst.append(v)
                return System.Enum.ToObject(e, lst[0]) if lst else None
    
            h = getOneAlignFromMap(justification, hmap, DocObjects.TextHorizontalAlignment)
            if h != None:
                te.TextHorizontalAlignment = h
            v = getOneAlignFromMap(justification, vmap, DocObjects.TextVerticalAlignment)
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
        let rc = Doc.Objects.AddTextDot(text, point)
        if rc = Guid.Empty then failwithf "Scripting: Unable to add text dot to document.  text:'%A' point:'%A'" text point
        Doc.Views.Redraw()
        rc
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
        if not text :? str: text = str(text)
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
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId, true)
        let mp = AreaMassProperties.Compute([rhobj.Geometry])
        if mp |> isNull then failwithf "Scripting: Unable to compute area mass properties.  objectId:'%A'" objectId
        mp.Area
    (*
    def Area(objectid):
        '''Compute the area of a closed curve, hatch, surface, polysurface, or mesh
        Parameters:
          objectid (guid): the object's identifier
        Returns:
          number: area if successful
          None: on error
        '''
    
        rhobj = rhutil.coercerhinoobject(objectid, True, True)
        mp = Geometry.AreaMassProperties.Compute(rhobj.Geometry)
        if mp is None: raise Exception("unable to compute area mass properties")
        return mp.Area
    *)


    [<EXT>]
    ///<summary>Returns either world axis-aligned or a construction plane axis-aligned
    ///  bounding box of an object or of several objects</summary>
    ///<param name="objects">(Guid seq) The identifiers of the objects</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///  plane to which the bounding box should be aligned
    ///  If omitted, a world axis-aligned bounding box
    ///  will be calculated</param>
    ///<param name="inWorldCoords">(bool) Optional, Default Value: <c>true</c>
    ///Return the bounding box as world coordinates or
    ///  construction plane coordinates. Note, this option does not apply to
    ///  world axis-aligned bounding boxes.</param>
    ///<returns>(Point3d array) Eight 3D points that define the bounding box.
    ///  Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBox(objects:Guid seq, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(true)>]inWorldCoords:bool) : Point3d array =
        let mutable bbox = BoundingBox.Empty
        
        if plane.IsValid then
            let xform = Transform.ChangeBasis(Plane.WorldXY, plane)
            objects 
            |> Seq.map RhinoScriptSyntax.CoerceGeometry
            |> Seq.iter (fun g -> 
                bbox <- BoundingBox.Union(bbox, g.GetBoundingBox(xform)) )
            
            bbox.GetCorners()
        else
            let xform = Transform.ChangeBasis(Plane.WorldXY, plane)
            objects 
            |> Seq.map RhinoScriptSyntax.CoerceGeometry
            |> Seq.iter (fun g -> bbox <- BoundingBox.Union(bbox, g.GetBoundingBox(true)) )

            if inWorldCoords then
                let planetoworld = Transform.ChangeBasis(plane, Plane.WorldXY)
                bbox.GetCorners() |> Array.map(fun p -> p.Transform(planetoworld);p)
            else
                bbox.GetCorners()


    (*
    def BoundingBox(objects, view_orplane=None, in_worldcoords=True):
        '''Returns either world axis-aligned or a construction plane axis-aligned
        bounding box of an object or of several objects
        Parameters:
          objects ([guid, ...]): The identifiers of the objects
          view_orplane (str|guid): Title or id of the view that contains the
              construction plane to which the bounding box should be aligned -or-
              user defined plane. If omitted, a world axis-aligned bounding box
              will be calculated
          in_worldcoords (bool, optional): return the bounding box as world coordinates or
              construction plane coordinates. Note, this option does not apply to
              world axis-aligned bounding boxes.
        Returns:
          list(point, point, point, point, point, point, point, point): Eight 3D points that define the bounding box.
               Points returned in counter-clockwise order starting with the bottom rectangle of the box.
          None: on error
        '''
    
        def _objectbbox(object, xform):
            geom = rhutil.coercegeometry(object, False)
            if not geom:
                pt = rhutil.coerce3dpoint(object, True)
                if xform: pt = xform * pt
                return Geometry.BoundingBox(pt,pt)
            if xform: return geom.GetBoundingBox(xform)
            return geom.GetBoundingBox(True)
    
        xform = None
        plane = rhutil.coerceplane(view_orplane)
        if plane is None and view_orplane:
            view = view_orplane
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
            xform = Geometry.Transform.ChangeBasis(Geometry.Plane.WorldXY, plane)
        bbox = Geometry.BoundingBox.Empty
        if type(objects) is list or type(objects) is tuple:
            for object in objects:
                objectbbox = _objectbbox(object, xform)
                bbox = Geometry.BoundingBox.Union(bbox,objectbbox)
        else:
            objectbbox = _objectbbox(objects, xform)
            bbox = Geometry.BoundingBox.Union(bbox,objectbbox)
        if not bbox.IsValid: return scriptcontext.errorhandler()
    
        corners = list(bbox.GetCorners())
        if in_worldcoords and plane is not None:
            plane_toworld = Geometry.Transform.ChangeBasis(plane, Geometry.Plane.WorldXY)
            for pt in corners: pt.Transform(plane_toworld)
        return corners
    *)


    [<EXT>]
    ///<summary>Compares two objects to determine if they are geometrically identical.</summary>
    ///<param name="first">(Guid) The identifier of the first object to compare.</param>
    ///<param name="second">(Guid) The identifier of the second object to compare.</param>
    ///<returns>(bool) True if the objects are geometrically identical, otherwise False.</returns>
    static member CompareGeometry(first:Guid, second:Guid) : bool =
        let firstG = RhinoScriptSyntax.CoerceGeometry(first)
        let secondG = RhinoScriptSyntax.CoerceGeometry(second)
        GeometryBase.GeometryEquals(firstG, secondG)
    (*
    def CompareGeometry(first, second):
        '''Compares two objects to determine if they are geometrically identical.
        Parameters:
          first (guid|geometry): The identifier of the first object to compare.
          second (guid|geometry): The identifier of the second object to compare.
        Returns:
          bool: True if the objects are geometrically identical, otherwise False.
        '''
    
        firstg = rhutil.coercegeometry(first, True)
        secondg = rhutil.coercegeometry(second, True)
    
        return Geometry.GeometryBase.GeometryEquals(firstg, secondg)
    *)


    [<EXT>]
    ///<summary>Creates outline curves for a given text entity</summary>
    ///<param name="textId">(Guid) Identifier of Text object to explode</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///Delete the text object after the curves have been created</param>
    ///<returns>(Guid array) of outline curves</returns>
    static member ExplodeText(textId:Guid, [<OPT;DEF(false)>]delete:bool) : Guid [] =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(textId, true)
        let curves = (rhobj.Geometry:?>TextEntity).Explode()
        let attr = rhobj.Attributes
        let rc = [| for curve in curves do yield Doc.Objects.AddCurve(curve,attr) |]
        if notNull delete then Doc.Objects.Delete(rhobj,true) |>ignore
        Doc.Views.Redraw()
        rc
    (*
    def ExplodeText(textid, delete=False):
        '''Creates outline curves for a given text entity
        Parameters:
          textid (guid): identifier of Text object to explode
          delete (bool, optional): delete the text object after the curves have been created
        Returns:
          list(guid): of outline curves
        '''
    
        rhobj = rhutil.coercerhinoobject(textid, True, True)
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
        let pc = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if pc.IsNone then false else pc.Value :? ClippingPlaneSurface
    (*
    def IsClippingPlane(objectid):
        '''Verifies that an object is a clipping plane object
        Parameters:
          objectid (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a clipping plane
        '''
    
        cp = rhutil.TryCoerceGeometry(objectid)
        return isinstance(cp, Geometry.ClippingPlaneSurface)
    *)


    [<EXT>]
    ///<summary>Verifies an object is a point object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given id is a point</returns>
    static member IsPoint(objectId:Guid) : bool =
        let p = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if p.IsNone then false else p.Value :? Point
    (*
    def IsPoint(objectid):
        '''Verifies an object is a point object.
        Parameters:
          objectid (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a point
        '''
    
        p = rhutil.coercegeometry(objectid)
        return isinstance(p, Geometry.Point)
    *)


    [<EXT>]
    ///<summary>Verifies an object is a point cloud object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given id is a point cloud</returns>
    static member IsPointCloud(objectId:Guid) : bool =
        let pc = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if pc.IsNone then false else pc.Value :? PointCloud
    (*
    def IsPointCloud(objectid):
        '''Verifies an object is a point cloud object.
        Parameters:
          objectid (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a point cloud
        '''
    
        pc = rhutil.coercegeometry(objectid)
        return isinstance(pc, Geometry.PointCloud)
    *)


    [<EXT>]
    ///<summary>Verifies an object is a text object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given id is a text object</returns>
    static member IsText(objectId:Guid) : bool =
        let p = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if p.IsNone then false else p.Value :? TextEntity
    (*
    def IsText(objectid):
        '''Verifies an object is a text object.
        Parameters:
          objectid (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a text object
        '''
    
        text = rhutil.coercegeometry(objectid)
        return isinstance(text, Geometry.TextEntity)
    *)


    [<EXT>]
    ///<summary>Verifies an object is a text dot object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given id is a text dot object</returns>
    static member IsTextDot(objectId:Guid) : bool =
        let p = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if p.IsNone then false else p.Value :? TextDot
    (*
    def IsTextDot(objectid):
        '''Verifies an object is a text dot object.
        Parameters:
          objectid (guid): the object's identifier
        Returns:
          bool: True if the object with a given id is a text dot object
        '''
    
        td = rhutil.coercegeometry(objectid)
        return isinstance(td, Geometry.TextDot)
    *)


    [<EXT>]
    ///<summary>Returns the point count of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(int) number of points</returns>
    static member PointCloudCount(objectId:Guid) : int =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.Count
    (*
    def PointCloudCount(objectid):
        '''Returns the point count of a point cloud object
        Parameters:
          objectid (guid): the point cloud object's identifier
        Returns:
          number: number of points if successful
        '''
    
        pc = rhutil.coercegeometry(objectid, True)
        if isinstance(pc, Geometry.PointCloud): return pc.Count
    *)


    [<EXT>]
    ///<summary>Verifies that a point cloud has hidden points</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(bool) True if cloud has hidden points, otherwise False</returns>
    static member PointCloudHasHiddenPoints(objectId:Guid) : bool =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.HiddenPointCount>0
    (*
    def PointCloudHasHiddenPoints(objectid):
        '''Verifies that a point cloud has hidden points
        Parameters:
          objectid (guid): the point cloud object's identifier
        Returns:
          bool: True if cloud has hidden points, otherwise False
        '''
    
        pc = rhutil.coercegeometry(objectid, True)
        if isinstance(pc, Geometry.PointCloud): return pc.HiddenPointCount>0
    *)


    [<EXT>]
    ///<summary>Verifies that a point cloud has point colors</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(bool) True if cloud has point colors, otherwise False</returns>
    static member PointCloudHasPointColors(objectId:Guid) : bool =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.ContainsColors
    (*
    def PointCloudHasPointColors(objectid):
        '''Verifies that a point cloud has point colors
        Parameters:
          objectid (guid): the point cloud object's identifier
        Returns:
          bool: True if cloud has point colors, otherwise False
        '''
    
        pc = rhutil.coercegeometry(objectid, True)
        if isinstance(pc, Geometry.PointCloud): return pc.ContainsColors
    *)


    [<EXT>]
    ///<summary>Returns the hidden points of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(bool []) List of point cloud hidden states</returns>
    static member PointCloudHidePoints(objectId:Guid) : bool [] = //GET
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)        
        [| for item in pc do yield item.Hidden |] 
        
            
    (*
    def PointCloudHidePoints(objectid, hidden=[]):
        '''Returns or modifies the hidden points of a point cloud object
        Parameters:
          objectid (guid): the point cloud object's identifier
          hidden ([bool, ....]): list of booleans matched to the index of points to be hidden
        Returns:
          list(bool, ....): List of point cloud hidden states
          list(bool, ....): List of point cloud hidden states
        '''
    
        rhobj = rhutil.coercerhinoobject(objectid)
        if rhobj: pc = rhobj.Geometry
        else: pc = rhutil.coercegeometry(objectid, True)
        if isinstance(pc, Geometry.PointCloud):
            rc = None
            if pc.ContainsHiddenFlags: rc = [item.Hidden for item in pc]
            if hidden is None:
                pc.ClearHiddenFlags()
            elif Seq.length(hidden)==pc.Count:
                for i = 0  to -1 + (pc.Count): pc[i].Hidden = hidden[i]
            if rhobj:
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the hidden points of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<param name="hidden">(bool seq)List of booleans matched to the index of points to be hidden, On empty seq all point wil be shown</param>
    ///<returns>(unit) void, nothing</returns>
    static member PointCloudHidePoints(objectId:Guid, hidden:bool seq) : unit = //SET
        let pc = RhinoScriptSyntax.CoercePointCloud objectId
        if Seq.isEmpty hidden then 
            pc.ClearHiddenFlags()
        
        elif Seq.length(hidden) = pc.Count then
                for i,h in Seq.indexed hidden do 
                    pc.[i].Hidden <- h
        else
            failwithf "PointCloudHidePoints length of hidden values does not match point cloud point count"
            
        (RhinoScriptSyntax.CoerceRhinoObject objectId).CommitChanges()|> ignore
        Doc.Views.Redraw()
        
    (*
    def PointCloudHidePoints(objectid, hidden=[]):
        '''Returns or modifies the hidden points of a point cloud object
        Parameters:
          objectid (guid): the point cloud object's identifier
          hidden ([bool, ....]): list of booleans matched to the index of points to be hidden
        Returns:
          list(bool, ....): List of point cloud hidden states
          list(bool, ....): List of point cloud hidden states
        '''
    
        rhobj = rhutil.coercerhinoobject(objectid)
        if rhobj: pc = rhobj.Geometry
        else: pc = rhutil.coercegeometry(objectid, True)
        if isinstance(pc, Geometry.PointCloud):
            rc = None
            if pc.ContainsHiddenFlags: rc = [item.Hidden for item in pc]
            if hidden is None:
                pc.ClearHiddenFlags()
            elif Seq.length(hidden)==pc.Count:
                for i = 0  to -1 + (pc.Count): pc[i].Hidden = hidden[i]
            if rhobj:
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the point colors of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(Drawing.Color array) List of point cloud colors</returns>
    static member PointCloudPointColors(objectId:Guid) : Drawing.Color [] = //GET
        let pc = RhinoScriptSyntax.CoercePointCloud objectId
        [| for item in pc do yield item.Color |]
           
    (*
    def PointCloudPointColors(objectid, colors=[]):
        '''Returns or modifies the point colors of a point cloud object
        Parameters:
          objectid (guid): the point cloud object's identifier
          colors ([color, ...]) list of color values if you want to adjust colors
        Returns:
          list(color, ...): List of point cloud colors
          list(color, ...): List of point cloud colors
        '''
    
        rhobj = rhutil.coercerhinoobject(objectid)
        if rhobj: pc = rhobj.Geometry
        else: pc = rhutil.coercegeometry(objectid, True)
        if isinstance(pc, Geometry.PointCloud):
            rc = None
            if pc.ContainsColors: rc = [item.Color for item in pc]
            if colors is None:
                pc.ClearColors()
            elif Seq.length(colors)==pc.Count:
                for i = 0  to -1 + (pc.Count): pc[i].Color = rhutil.coercecolor(colors[i])
            if rhobj:
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the point colors of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<param name="colors">(Drawing.Color seq)List of color values if you want to adjust colors, empty Seq to clear colors</param>
    ///<returns>(unit) void, nothing</returns>
    static member PointCloudPointColors(objectId:Guid, colors:Drawing.Color seq) : unit = //SET
        let pc = RhinoScriptSyntax.CoercePointCloud objectId
        if colors |> Seq.isEmpty then
            pc.ClearColors()
        elif Seq.length(colors) = pc.Count then
            for i,c in Seq.indexed colors do pc.[i].Color <- c
        else
            failwithf "PointCloudHidePoints length of hidden values does not match point cloud point count"
        (RhinoScriptSyntax.CoerceRhinoObject objectId).CommitChanges()|> ignore
        Doc.Views.Redraw()
            
    (*
    def PointCloudPointColors(objectid, colors=[]):
        '''Returns or modifies the point colors of a point cloud object
        Parameters:
          objectid (guid): the point cloud object's identifier
          colors ([color, ...]) list of color values if you want to adjust colors
        Returns:
          list(color, ...): List of point cloud colors
          list(color, ...): List of point cloud colors
        '''
    
        rhobj = rhutil.coercerhinoobject(objectid)
        if rhobj: pc = rhobj.Geometry
        else: pc = rhutil.coercegeometry(objectid, True)
        if isinstance(pc, Geometry.PointCloud):
            rc = None
            if pc.ContainsColors: rc = [item.Color for item in pc]
            if colors is None:
                pc.ClearColors()
            elif Seq.length(colors)==pc.Count:
                for i = 0  to -1 + (pc.Count): pc[i].Color = rhutil.coercecolor(colors[i])
            if rhobj:
                rhobj.CommitChanges()
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the points of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(Point3d array) list of points</returns>
    static member PointCloudPoints(objectId:Guid) : Point3d [] =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.GetPoints()
    (*
    def PointCloudPoints(objectid):
        '''Returns the points of a point cloud object
        Parameters:
          objectid (guid): the point cloud object's identifier
        Returns:
          list(guid, ...): list of points if successful
        '''
    
        pc = rhutil.coercegeometry(objectid, True)
        if isinstance(pc, Geometry.PointCloud): return pc.GetPoints()
    *)


    [<EXT>]
    
    static member internal SimplifyPointCloudKNeighbors()  =
        ()
    (*
    def __simplify_PointCloudKNeighbors(result, amount):
    if amount == 1:
        return [item[0] for item in result]
    else:
        return [list(item) for item in result]
    *)


    [<EXT>]
    ///<summary>Returns amount indices of points in a point cloud that are near needlePoints.</summary>
    ///<param name="ptCloud">(Point3d seq) The point cloud to be searched, or the "hay stack". This can also be a list of points.</param>
    ///<param name="needlePoints">(Point3d seq) A list of points to search in the pointcloud. This can also be specified as a point cloud.</param>
    ///<param name="amount">(int) Optional, Default Value: <c>1</c>
    ///The amount of required closest points. Defaults to 1.</param>
    ///<returns>(seq<int array>) nested lists with amount items within a list, with the indices of the found points.</returns>
    static member PointCloudKNeighbors(ptCloud:Point3d seq, needlePoints:Point3d seq, [<OPT;DEF(1)>]amount:int) : seq<int[]> =
        if Seq.length(needlePoints) > 100 then
            RTree.Point3dKNeighbors(ptCloud, needlePoints, amount)
        else 
            Collections.RhinoList.Point3dKNeighbors(ptCloud, needlePoints, amount)
        
        
        
    (*
    def PointCloudKNeighbors(ptcloud, needlepoints, amount=1):
        '''Returns amount indices of points in a point cloud that are near needlepoints.
        Parameters:
          ptcloud (guid|[point, ...]): the point cloud to be searched, or the "hay stack". This can also be a list of points.
          needlepoints (guid|[point, ...]): a list of points to search in the pointcloud. This can also be specified as a point cloud.
          amount (int, optional): the amount of required closest points. Defaults to 1.
        Returns:
          [int, int,...]: a list of indices of the found points, if amount equals 1.
            [[int, ...], ...]: nested lists with amount items within a list, with the indices of the found points.
        '''
    
        needles = rhutil.coercegeometry(needlepoints, False)
        if isinstance(needles, Geometry.PointCloud): needles = needles.AsReadOnlyListOfPoints()
        else: needles = rhutil.coerce3dpointlist(needlepoints, True)
    
        pcgeom = rhutil.coercegeometry(ptcloud, False)
        if isinstance(pcgeom, Geometry.PointCloud):
            if Seq.length(needles) > 100:
                search = Geometry.RTree.PointCloudKNeighbors
            else:
                search = Collections.RhinoList.PointCloudKNeighbors
    
            return __simplifyPointCloudKNeighbors(search(pcgeom, needles, amount), amount)
    
        if Seq.length(needles) > 100:
            search = Geometry.RTree.Point3dKNeighbors
        else:
            search = Collections.RhinoList.Point3dKNeighbors
    
        if isinstance(ptcloud, System.Collections.Generic.IEnumerable[Geometry.Point3d]):
            return __simplifyPointCloudKNeighbors(search(ptcloud, needles, amount), amount)
        pts = rhutil.coerce3dpointlist(ptcloud, True)
        return __simplifyPointCloudKNeighbors(search(pts, needles, amount), amount)
    *)



    [<EXT>]
    ///<summary>Returns a list of lists of point indices in a point cloud that are
    ///  closest to needlePoints. Each inner list references all points within or on the surface of a sphere of distance radius.</summary>
    ///<param name="ptCloud">(Point3d seq) The point cloud to be searched, or the "hay stack". This can also be a list of points.</param>
    ///<param name="needlePoints">(Point3d seq) A list of points to search in the pointcloud. This can also be specified as a point cloud.</param>
    ///<param name="distance">(float) The included limit for listing points.</param>
    ///<returns>(seq<int array>) a seq of arrays with the indices of the found points.</returns>
    static member PointCloudClosestPoints(ptCloud:Point3d seq, needlePoints:Point3d seq, distance:float) : seq<int []> =
        RTree.Point3dClosestPoints(ptCloud, needlePoints, distance)
        

    (*
    def PointCloudClosestPoints(ptcloud, needlepoints, distance):
        '''Returns a list of lists of point indices in a point cloud that are
        closest to needlepoints. Each inner list references all points within or on the surface of a sphere of distance radius.
        Parameters:
          ptcloud (guid|[point, ...]): the point cloud to be searched, or the "hay stack". This can also be a list of points.
          needlepoints (guid|[point, ...]): a list of points to search in the pointcloud. This can also be specified as a point cloud.
          distance (float): the included limit for listing points.
        Returns:
          [[int, ...], ...]: a list of lists with the indices of the found points.
        '''
    
        needles = rhutil.coercegeometry(needlepoints, False)
        if isinstance(needles, Geometry.PointCloud): needles = needles.AsReadOnlyListOfPoints()
        else: needles = rhutil.coerce3dpointlist(needlepoints, True)
        pcgeom = rhutil.coercegeometry(ptcloud, False)
        if isinstance(pcgeom, Geometry.PointCloud):
            return __simplifyPointCloudClosestPoints(Geometry.RTree.PointCloudClosestPoints(pcgeom, needles, distance))
        if isinstance(ptcloud, System.Collections.Generic.IEnumerable[Geometry.Point3d]):
            return __simplifyPointCloudClosestPoints(Geometry.RTree.Point3dClosestPoints(ptcloud, needles, distance))
        pts = rhutil.coerce3dpointlist(ptcloud, True)
        return __simplifyPointCloudClosestPoints(Geometry.RTree.Point3dClosestPoints(pts, needles, distance))
    *)


    [<EXT>]
    ///<summary>Returns the X, Y, and Z coordinates of a point object</summary>
    ///<param name="objectId">(Guid) The identifier of a point object</param>
    ///<returns>(Point3d) The current 3-D point location</returns>
    static member PointCoordinates(objectId:Guid) : Point3d = //GET
        RhinoScriptSyntax.Coerce3dPoint(objectId)

    (*
    def PointCoordinates(objectid, point=None):
        '''Returns or modifies the X, Y, and Z coordinates of a point object
        Parameters:
          objectid (guid): The identifier of a point object
          point (point, optional): A new 3D point location.
        Returns:
          point: If point is not specified, the current 3-D point location
          point: If point is specified, the previous 3-D point location
        '''
    
        pointgeometry = rhutil.coercegeometry(objectid, True)
        if isinstance(pointgeometry, Geometry.Point):
            rc = pointgeometry.Location
            if point:
                point = rhutil.coerce3dpoint(point, True)
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, point)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the X, Y, and Z coordinates of a point object</summary>
    ///<param name="objectId">(Guid) The identifier of a point object</param>
    ///<param name="point">(Point3d)A new 3D point location.</param>
    ///<returns>(unit) void, nothing</returns>
    static member PointCoordinates(objectId:Guid, point:Point3d) : unit = //SET
        let pt = RhinoScriptSyntax.Coerce3dPoint(objectId)
        if not <| Doc.Objects.Replace(objectId, pt) then failwithf "PointCoordinates failed to change object %A to %A " objectId point
        Doc.Views.Redraw()
          
    (*
    def PointCoordinates(objectid, point=None):
        '''Returns or modifies the X, Y, and Z coordinates of a point object
        Parameters:
          objectid (guid): The identifier of a point object
          point (point, optional): A new 3D point location.
        Returns:
          point: If point is not specified, the current 3-D point location
          point: If point is specified, the previous 3-D point location
        '''
    
        pointgeometry = rhutil.coercegeometry(objectid, True)
        if isinstance(pointgeometry, Geometry.Point):
            rc = pointgeometry.Location
            if point:
                point = rhutil.coerce3dpoint(point, True)
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, point)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the font of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<returns>(string) The current text dot font</returns>
    static member TextDotFont(objectId:Guid) : string = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).FontFace
    (*
    def TextDotFont(objectid, fontface=None):
        '''Returns or modifies the font of a text dot
        Parameters:
          objectid (guid): identifier of a text dot object
          fontface (str, optional): new font face name
        Returns:
          str: If font is not specified, the current text dot font
          str: If font is specified, the previous text dot font
          None: on error
        '''
    
        textdot = rhutil.coercegeometry(objectid, True)
        if isinstance(textdot, Geometry.TextDot):
            rc = textdot.FontFace
            if fontface:
                textdot.FontFace = fontface
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the font of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<param name="fontface">(string)New font face name</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotFont(objectId:Guid, fontface:string) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.FontFace <-  fontface
        if not <| Doc.Objects.Replace(objectId, textdot) then failwithf "TextDotFont failed to change object %A to %A " objectId fontface
        Doc.Views.Redraw()
        (*
        
    def TextDotFont(objectid, fontface=None):
        '''Returns or modifies the font of a text dot
        Parameters:
          objectid (guid): identifier of a text dot object
          fontface (str, optional): new font face name
        Returns:
          str: If font is not specified, the current text dot font
          str: If font is specified, the previous text dot font
          None: on error
        '''
    
        textdot = rhutil.coercegeometry(objectid, True)
        if isinstance(textdot, Geometry.TextDot):
            rc = textdot.FontFace
            if fontface:
                textdot.FontFace = fontface
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the font height of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<returns>(int) The current text dot height</returns>
    static member TextDotHeight(objectId:Guid) : int = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).FontHeight
    (*
    def TextDotHeight(objectid, height=None):
        '''Returns or modifies the font height of a text dot
        Parameters:
          objectid (guid): identifier of a text dot object
          height (number, optional) new font height
        Returns:
          number: If height is not specified, the current text dot height
          number: If height is specified, the previous text dot height
          None: on error
        '''
    
        textdot = rhutil.coercegeometry(objectid, True)
        if isinstance(textdot, Geometry.TextDot):
            rc = textdot.FontHeight
            if height and height>0:
                textdot.FontHeight = height
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the font height of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<param name="height">(int)New font height</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotHeight(objectId:Guid, height:int) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.FontHeight <- height
        if not <| Doc.Objects.Replace(objectId, textdot) then failwithf "TextDotHeight failed to change object %A to %A " objectId height
        Doc.Views.Redraw()
        
        (*
    def TextDotHeight(objectid, height=None):
        '''Returns or modifies the font height of a text dot
        Parameters:
          objectid (guid): identifier of a text dot object
          height (number, optional) new font height
        Returns:
          number: If height is not specified, the current text dot height
          number: If height is specified, the previous text dot height
          None: on error
        '''
    
        textdot = rhutil.coercegeometry(objectid, True)
        if isinstance(textdot, Geometry.TextDot):
            rc = textdot.FontHeight
            if height and height>0:
                textdot.FontHeight = height
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the location, or insertion point, on a text dot object</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<returns>(Point3d) The current 3-D text dot location</returns>
    static member TextDotPoint(objectId:Guid) : Point3d = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).Point
        
    (*
    def TextDotPoint(objectid, point=None):
        '''Returns or modifies the location, or insertion point, on a text dot object
        Parameters:
          objectid (guid): identifier of a text dot object
          point (point, optional): A new 3D point location.
        Returns:
          point: If point is not specified, the current 3-D text dot location
          point: If point is specified, the previous 3-D text dot location
          None: if not successful, or on error
        '''
    
        textdot = rhutil.coercegeometry(objectid, True)
        if isinstance(textdot, Geometry.TextDot):
            rc = textdot.Point
            if point:
                textdot.Point = rhutil.coerce3dpoint(point, True)
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the location, or insertion point, on a text dot object</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<param name="point">(Point3d)A new 3D point location.</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotPoint(objectId:Guid, point:Point3d) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.Point <-  point
        if not <| Doc.Objects.Replace(objectId, textdot) then failwithf "TextDotPoint failed to change object %A to %A " objectId point
        Doc.Views.Redraw()
        
        
    (*
    def TextDotPoint(objectid, point=None):
        '''Returns or modifies the location, or insertion point, on a text dot object
        Parameters:
          objectid (guid): identifier of a text dot object
          point (point, optional): A new 3D point location.
        Returns:
          point: If point is not specified, the current 3-D text dot location
          point: If point is specified, the previous 3-D text dot location
          None: if not successful, or on error
        '''
    
        textdot = rhutil.coercegeometry(objectid, True)
        if isinstance(textdot, Geometry.TextDot):
            rc = textdot.Point
            if point:
                textdot.Point = rhutil.coerce3dpoint(point, True)
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the text on a text dot object</summary>
    ///<param name="objectId">(Guid) The identifier of a text dot object</param>
    ///<returns>(string) The current text dot text</returns>
    static member TextDotText(objectId:Guid) : string = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).Text


    (*
    def TextDotText(objectid, text=None):
        '''Returns or modifies the text on a text dot object
        Parameters:
          objectid (guid): The identifier of a text dot object
          text (str, optional): a new string for the dot
        Returns:
          str: If text is not specified, the current text dot text
          str: If text is specified, the previous text dot text
          None: if not successful, or on error
        '''
    
        textdot = rhutil.coercegeometry(objectid, True)
        if isinstance(textdot, Geometry.TextDot):
            rc = textdot.Text
            if text is not None:
                if not text :? str: text = str(text)
                textdot.Text = text
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)

    ///<summary>Modifies the text on a text dot object</summary>
    ///<param name="objectId">(Guid) The identifier of a text dot object</param>
    ///<param name="text">(string)A new string for the dot</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotText(objectId:Guid, text:string) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.Text <-  text
        if not <| Doc.Objects.Replace(objectId, textdot) then failwithf "TextDotText failed to change object %A to %A " objectId text
        Doc.Views.Redraw()
        
        
    (*
    def TextDotText(objectid, text=None):
        '''Returns or modifies the text on a text dot object
        Parameters:
          objectid (guid): The identifier of a text dot object
          text (str, optional): a new string for the dot
        Returns:
          str: If text is not specified, the current text dot text
          str: If text is specified, the previous text dot text
          None: if not successful, or on error
        '''
    
        textdot = rhutil.coercegeometry(objectid, True)
        if isinstance(textdot, Geometry.TextDot):
            rc = textdot.Text
            if text is not None:
                if not text :? str: text = str(text)
                textdot.Text = text
                id = rhutil.coerceguid(objectid, True)
                scriptcontext.doc.Objects.Replace(id, textdot)
                scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Returns the font used by a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(string) The current font face name</returns>
    static member TextObjectFont(objectId:Guid) : string = //GET
        (RhinoScriptSyntax.CoerceTextEntity(objectId)).Font.FamilyPlusFaceName


    (*
    def TextObjectFont(objectid, font=None):
        '''Returns or modifies the font used by a text object
        Parameters:
          objectid (guid): the identifier of a text object
          font (str): the new font face name
        Returns:
          str: if a font is not specified, the current font face name
          str: if a font is specified, the previous font face name
          None: if not successful, or on error
        '''
    
        annotation = rhutil.coercegeometry(objectid, True)
        if not isinstance(annotation, Geometry.TextEntity):
            return scriptcontext.errorhandler()
        fontdata = annotation.Font
        rc = fontdata.FaceName
        if font:
            index = scriptcontext.doc.Fonts.FindOrCreate( font, fontdata.Bold, fontdata.Italic )
            annotation.Font = scriptcontext.doc.Fonts[index]
            id = rhutil.coerceguid(objectid, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the font used by a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="font">(string)The new font face name</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectFont(objectId:Guid, font:string) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let f = DocObjects.Font(font) 
        if isNull f then  failwithf "set TextObjectFont failed.  font:'%A'" font        
        annotation.Font <- f
        Doc.Views.Redraw()

        (*
        let index = Doc.Fonts.FindOrCreate( font, fontdata.Bold, fontdata.Italic )
        annotation.Font <-  Doc.Fonts.[index]
        let id = RhinoScriptSyntax.CoerceGuid(objectId)
        Doc.Objects.Replace(id, annotation)
        Doc.Views.Redraw()
        *)
        
    (*
    def TextObjectFont(objectid, font=None):
        '''Returns or modifies the font used by a text object
        Parameters:
          objectid (guid): the identifier of a text object
          font (str): the new font face name
        Returns:
          str: if a font is not specified, the current font face name
          str: if a font is specified, the previous font face name
          None: if not successful, or on error
        '''
    
        annotation = rhutil.coercegeometry(objectid, True)
        if not isinstance(annotation, Geometry.TextEntity):
            return scriptcontext.errorhandler()
        fontdata = annotation.Font
        rc = fontdata.FaceName
        if font:
            index = scriptcontext.doc.Fonts.FindOrCreate( font, fontdata.Bold, fontdata.Italic )
            annotation.Font = scriptcontext.doc.Fonts[index]
            id = rhutil.coerceguid(objectid, True)
            scriptcontext.doc.Objects.Replace(id, annotation)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)
