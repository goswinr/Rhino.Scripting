﻿namespace Rhino.Scripting

open Rhino
open System
open System.Collections.Generic
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils


[<AutoOpen>]
module AutoOpenGeometry =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    /// <summary>Create a clipping Plane for visibly clipping away geometry in a specific view. Clipping Planes are infinite.</summary>
    /// <param name="plane">(Plane) The Plane</param>
    /// <param name="uMagnitude">(float) U magnitude of the Plane</param>
    /// <param name="vMagnitude">(float) V magnitude of the Plane</param>
    /// <param name="views">(string seq) Optional, Titles the view(s) to clip. If omitted, the active
    ///    view is used</param>
    /// <returns>(Guid) object identifier.</returns>
    static member AddClippingPlane( plane:Plane,
                                    uMagnitude:float,
                                    vMagnitude:float,
                                    [<OPT;DEF(null:string seq)>]views:string seq) : Guid =
        let viewList =
            if isNull views then [State.Doc.Views.ActiveView.ActiveViewportID]
            else
                let modelViews = State.Doc.Views.GetViewList(includeStandardViews=true, includePageViews=false)
                [for view in views do
                    for item in modelViews do
                        if item.ActiveViewport.Name = view then
                            yield item.ActiveViewportID
                ]
        let rc = State.Doc.Objects.AddClippingPlane(plane, uMagnitude, vMagnitude, viewList)
        if rc = Guid.Empty then
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddClippingPlane: Unable to add clipping plane to document.  plane:'%s' uMagnitude:'%g' vMagnitude:'%g' views:'%s'" plane.Pretty uMagnitude vMagnitude (Pretty.str views)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a picture frame and adds it to the document.</summary>
    /// <param name="plane">(Plane) The plane in which the PictureFrame will be created. The bottom-left corner of the picture will be at the plane's origin. The width will be in the plane's X axis direction, and the height will be in the plane's Y axis direction.</param>
    /// <param name="filename">(string) The path to a bitmap or image file.</param>
    /// <param name="width">(float) Optional. If both width and height are 0.0 or skipped, then the width and height of the PictureFrame will be the width and height of the image. If width = 0 and height > 0, or if width > 0 and height = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, whichever one is 0. If both width and height are > 0, then these are assumed to be the width and height in the current unit system.</param>
    /// <param name="height">(float) Optional. If both width and height are 0.0 or skipped, then the width and height of the PictureFrame will be the width and height of the image. If width = 0 and height > 0, or if width > 0 and height = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, whichever one is 0. If both width and height are > 0, then these are assumed to be the width and height in the current unit system.</param>
    /// <param name="selfIllumination">(bool) Optional, default value: <c>true</c>. If True, then the image mapped to the picture frame plane always displays at full intensity and is not affected by light or shadow.</param>
    /// <param name="embed">(bool) Optional, default value: <c>false</c>. If True, then the function adds the image to Rhino's internal bitmap table, thus making the document self-contained.</param>
    /// <param name="useAlpha">(bool) Optional, default value: <c>false</c>. If False, the picture frame is created without any transparency texture. If True, a transparency texture is created with a "mask texture" set to alpha, and an instance of the diffuse texture in the source texture slot.</param>
    /// <param name="makeMesh">(bool) Optional, default value: <c>false</c>. If True, the function will make a PictureFrame object from a mesh rather than a plane surface.</param>
    /// <returns>(Guid) Object identifier.</returns>
    static member AddPictureFrame(  plane:Plane,
                                    filename:string,
                                    [<OPT;DEF(0.0)>]width:float,
                                    [<OPT;DEF(0.0)>]height:float,
                                    [<OPT;DEF(true)>]selfIllumination:bool,
                                    [<OPT;DEF(false)>]embed:bool,
                                    [<OPT;DEF(false)>]useAlpha:bool,
                                    [<OPT;DEF(false)>]makeMesh:bool) : Guid =
        if not <| IO.File.Exists(filename) then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPictureFrame image %s does not exist" filename
        let rc = State.Doc.Objects.AddPictureFrame(plane, filename, makeMesh, width, height, selfIllumination, embed)
        if rc = Guid.Empty
            then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPictureFrame: Unable to add picture frame to document. plane:'%s' filename:'%s' width:'%g' height:'%g' selfIllumination:'%b' embed:'%b' useAlpha:'%b' makeMesh:'%b'" plane.Pretty filename width height selfIllumination embed useAlpha makeMesh
        State.Doc.Views.Redraw()
        rc

    /// <summary>Adds point object to the document.</summary>
    /// <param name="x">(float) X location of point to add</param>
    /// <param name="y">(float) Y location of point to add</param>
    /// <param name="z">(float) Z location of point to add</param>
    /// <returns>(Guid) identifier for the object that was added to the doc.</returns>
    static member AddPoint(x:float, y:float, z:float) : Guid =
        let rc = State.Doc.Objects.AddPoint(Point3d(x, y, z))
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPoint: Unable to add point to document.  x:'%s' y:'%s' z:'%s'"  (PrettyFormat.float x) (PrettyFormat.float y) (PrettyFormat.float z)
        State.Doc.Views.Redraw()
        rc

    /// <summary>Adds point object to the document.</summary>
    /// <param name="point">(Point3d) point to draw</param>
    /// <returns>(Guid) identifier for the object that was added to the doc.</returns>
    static member AddPoint(point:Point3d) : Guid =
        let rc = State.Doc.Objects.AddPoint(point)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPoint: Unable to add point to document.  point:'%s'" point.Pretty
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds point cloud object to the document.</summary>
    /// <param name="points">(Point3d array) List of values where every multiple of three represents a point</param>
    /// <param name="colors">(Drawing.Color IList) Optional, List of colors to apply to each point</param>
    /// <returns>(Guid) identifier of point cloud.</returns>
    static member AddPointCloud(points:Point3d [], [<OPT;DEF(null:Drawing.Color IList)>]colors:Drawing.Color IList) : Guid =
        if notNull colors && Seq.length(colors) = Seq.length(points) then
            let pc = new PointCloud()
            for i = 0  to -1 + (Seq.length(points)) do
                let color = RhinoScriptSyntax.CoerceColor(colors.[i])
                pc.Add(points.[i], color)
            let rc = State.Doc.Objects.AddPointCloud(pc)
            if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPointCloud: Unable to add point cloud to document. points:'%A' colors:'%A'" points colors
            State.Doc.Views.Redraw()
            rc
        else
            let rc = State.Doc.Objects.AddPointCloud(points)
            if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPointCloud: Unable to add point cloud to document. points:'%A' colors:'%A'" points colors
            State.Doc.Views.Redraw()
            rc


    /// <summary>Adds one or more point objects to the document.</summary>
    /// <param name="points">(Point3d seq) List of points</param>
    /// <returns>(Guid ResizeArray) List of identifiers of the new objects.</returns>
    static member AddPoints(points:Point3d seq) : Guid ResizeArray =
        let rc = points |> RArr.mapSeq State.Doc.Objects.AddPoint
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a text string to the document.</summary>
    /// <param name="text">(string) The text to display</param>
    /// <param name="plane">(Plane) The Plane on which the text will lie.
    ///    The origin of the Plane will be the origin point of the text</param>
    /// <param name="height">(float) Optional, default value: <c>1.0</c>
    ///    The text height</param>
    /// <param name="font">(string) Optional, The text font</param>
    /// <param name="fontStyle">(int) Optional, default value: <c>0</c>
    ///    Any of the following flags
    ///    0 = normal
    ///    1 = bold
    ///    2 = italic
    ///    3 = bold and italic</param>
    /// <param name="horizontalAlignment">(DocObjects.TextHorizontalAlignment) or Byte.
    ///    Optional, default value: <c>TextHorizontalAlignment.Center = 1uy</c>
    ///    0uy = Left
    ///    1uy = Center
    ///    2uy = Right</param>
    /// <param name="verticalAlignment">(DocObjects.TextVerticalAlignment) or Byte.
    ///    Optional, default value: <c>TextVerticalAlignment.Middle = 3uy</c>
    ///    0uy = Top:                    Attach to top of an "I" on the first line.
    ///    1uy = MiddleOfTop:            Attach to middle of an "I" on the first line.
    ///    2uy = BottomOfTop:            Attach to baseline of first line.
    ///    3uy = Middle:                 Attach to middle of text vertical advance.
    ///    4uy = MiddleOfBottom:         Attach to middle of an "I" on the last line.
    ///    5uy = Bottom:                 Attach to the baseline of the last line.
    ///    6uy = BottomOfBoundingBox:    Attach to the bottom of the bounding box of the visible glyphs.</param>
    /// <returns>(Guid) identifier for the object that was added to the doc.</returns>
    static member AddText(  text:string,
                            plane:Plane,
                            [<OPT;DEF(1.0)>]height:float,
                            [<OPT;DEF(null:string)>]font:string,
                            [<OPT;DEF(0)>]fontStyle:int,
                            [<OPT;DEF(1uy)>]horizontalAlignment:byte, //DocObjects.TextHorizontalAlignment, //TODO how to keep enum type and keep parameter optional ???
                            [<OPT;DEF(3uy)>]verticalAlignment  :byte) : Guid = //DocObjects.TextVerticalAlignment) : Guid =

        if isNull text || text = "" then RhinoScriptingException.Raise "RhinoScriptSyntax.AddText Text invalid.  text:'%s' plane:'%s' height:'%g' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane.Pretty height font fontStyle horizontalAlignment verticalAlignment
        let bold   = (1 = fontStyle || 3 = fontStyle)
        let italic = (2 = fontStyle || 3 = fontStyle)
        let ds = State.Doc.DimStyles.Current
        let qn, quartetBoldProp , quartetItalicProp =
            if isNull font then
                ds.Font.QuartetName, ds.Font.Bold, ds.Font.Italic
            else
                font, false, false

        let f = DocObjects.Font.FromQuartetProperties(qn, quartetBoldProp, quartetItalicProp)

        if isNull f then
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddText failed. text:'%s' plane:'%s' height:'%g' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane.Pretty height font fontStyle horizontalAlignment verticalAlignment
        let te = TextEntity.Create(text, plane, ds, false, 0.0, 0.0)
        te.TextHeight <- height
        if font |> notNull then
            te.Font <- f
        if bold <> quartetBoldProp then
            if DocObjects.Font.FromQuartetProperties(qn, bold, false) |> isNull then
                RhinoScriptingException.Raise "RhinoScriptSyntax.AddText failed. text:'%s' plane:'%s' height:'%g' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane.Pretty height font fontStyle horizontalAlignment verticalAlignment
            else
                te.SetBold(bold)|> ignore<bool>
        if italic <> quartetItalicProp then
            if DocObjects.Font.FromQuartetProperties(qn, false, italic) |> isNull then
                RhinoScriptingException.Raise "RhinoScriptSyntax.AddText failed. text:'%s' plane:'%s' height:'%g' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane.Pretty height font fontStyle horizontalAlignment verticalAlignment
            else
                te.SetItalic(italic)|> ignore<bool>

        te.TextHorizontalAlignment <- LanguagePrimitives.EnumOfValue horizontalAlignment
        te.TextVerticalAlignment <- LanguagePrimitives.EnumOfValue verticalAlignment
        let objectId = State.Doc.Objects.Add(te)
        if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddText: Unable to add text to document.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
        State.Doc.Views.Redraw()
        objectId

    /// <summary>Adds a text string to the document.</summary>
    /// <param name="text">(string) The text to display</param>
    /// <param name="pt">(Point3d) a point where to add text. It will be parallel to XY Plane.</param>
    /// <param name="height">(float) Optional, default value: <c>1.0</c>
    ///    The text height</param>
    /// <param name="font">(string) Optional, The text font</param>
    /// <param name="fontStyle">(int) Optional, default value: <c>0</c>
    ///    Any of the following flags
    ///    0 = normal
    ///    1 = bold
    ///    2 = italic
    ///    3 = bold and italic</param>
    /// <param name="horizontalAlignment">(DocObjects.TextHorizontalAlignment) or Byte.
    ///    Optional, default value: <c>TextHorizontalAlignment.Center = 1uy</c>
    ///    0uy = Left
    ///    1uy = Center
    ///    2uy = Right</param>
    /// <param name="verticalAlignment">(DocObjects.TextVerticalAlignment) or Byte.
    ///    Optional, default value: <c>TextVerticalAlignment.Middle = 3uy</c>
    ///    0uy = Top:                    Attach to top of an "I" on the first line.
    ///    1uy = MiddleOfTop:            Attach to middle of an "I" on the first line.
    ///    2uy = BottomOfTop:            Attach to baseline of first line.
    ///    3uy = Middle:                 Attach to middle of text vertical advance.
    ///    4uy = MiddleOfBottom:         Attach to middle of an "I" on the last line.
    ///    5uy = Bottom:                 Attach to the baseline of the last line.
    ///    6uy = BottomOfBoundingBox:    Attach to the bottom of the bounding box of the visible glyphs.</param>
    /// <returns>(Guid) identifier for the object that was added to the doc.</returns>
    static member AddText(  text:string,
                            pt:Point3d,
                            [<OPT;DEF(1.0)>]height:float,
                            [<OPT;DEF(null:string)>]font:string,
                            [<OPT;DEF(0)>]fontStyle:int,
                            [<OPT;DEF(1uy)>]horizontalAlignment:byte, //DocObjects.TextHorizontalAlignment, //TODO how to keep enum type and keep parameter optional ???
                            [<OPT;DEF(3uy)>]verticalAlignment  :byte) : Guid =
        let pl = Plane(pt,Vector3d.XAxis,Vector3d.YAxis)
        RhinoScriptSyntax.AddText(text, pl, height, font, fontStyle, horizontalAlignment, verticalAlignment)

    /// <summary>Add a text dot to the document.</summary>
    /// <param name="text">(string) String in dot</param>
    /// <param name="point">(Point3d) A 3D point identifying the origin point</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member AddTextDot(text:string, point:Point3d) : Guid =
        let rc = State.Doc.Objects.AddTextDot(text, point)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddTextDot: Unable to add TextDot to document. text:'%s' point:'%s'" text (Pretty.str point)
        State.Doc.Views.Redraw()
        rc

    /// <summary>Add a text dot to the document.</summary>
    /// <param name="text">(string) String in dot</param>
    /// <param name="x">(float) X position</param>
    /// <param name="y">(float) Y position</param>
    /// <param name="z">(float) Z position</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member AddTextDot(text:string, x:float,y,z) : Guid =
        let rc = State.Doc.Objects.AddTextDot(text, Point3d(x,y,z))
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddTextDot: Unable to add TextDot to document. text:'%s' at x:%g, y:%g, z:%g" text x y z
        State.Doc.Views.Redraw()
        rc


    /// <summary>Compute the area of a closed Curve, Hatch, Surface, Polysurface, or Mesh.</summary>
    /// <param name="geometry">(GeometryBase) The geometry to use</param>
    /// <returns>(float) area.</returns>
    static member Area(geometry:GeometryBase) : float =
        let mp = AreaMassProperties.Compute([geometry])
        if mp |> isNull then RhinoScriptingException.Raise "RhinoScriptSyntax.Area: Unable to compute area mass properties from geometry:'%s'" (Pretty.str geometry)
        mp.Area

    /// <summary>Compute the area of a closed Curve, Hatch, Surface, Polysurface, or Mesh.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(float) area.</returns>
    static member Area(objectId:Guid) : float =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let mp = AreaMassProperties.Compute([rhobj.Geometry])
        if mp |> isNull then RhinoScriptingException.Raise "RhinoScriptSyntax.Area: Unable to compute area mass properties from objectId:'%s'" (Pretty.str objectId)
        mp.Area

    /// <summary>Returns a world axis-aligned bounding box of several objects.
    ///   Estimated bounding boxes can be computed much (much) faster than accurate (or "tight") bounding boxes.
    ///   Estimated bounding boxes are always similar to or larger than accurate bounding boxes.</summary>
    /// <param name="geometries">(GeometryBase seq) The Geometries of the objects</param>
    /// <returns>(Geometry.BoundingBox) The BoundingBox (oriented to the World XY Plane).
    ///    To get the eight 3D points that define the bounding box call box.GetCorners()
    ///    Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBoxEstimate(geometries: seq<#GeometryBase>) : BoundingBox =
        let mutable bbox = BoundingBox.Empty
        for g in geometries do
            bbox <- BoundingBox.Union(bbox, g.GetBoundingBox(false)) //https://discourse.mcneel.com/t/questions-about-getboundingbox-bool/32092/5
        bbox

    /// <summary>Returns a world axis-aligned bounding box of several objects.
    ///   Estimated bounding boxes can be computed much (much) faster than accurate (or "tight") bounding boxes.
    ///   Estimated bounding boxes are always similar to or larger than accurate bounding boxes.</summary>
    /// <param name="objects">(Guid seq) The identifiers of the objects</param>
    /// <returns>(Geometry.BoundingBox) The BoundingBox (oriented to the World XY Plane).
    ///    To get the eight 3D points that define the bounding box call box.GetCorners()
    ///    Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBoxEstimate(objects:Guid seq) : BoundingBox =
        let mutable bbox = BoundingBox.Empty
        for o in objects do
            let g =  RhinoScriptSyntax.CoerceGeometry o
            bbox <- BoundingBox.Union(bbox, g.GetBoundingBox(false)) //https://discourse.mcneel.com/t/questions-about-getboundingbox-bool/32092/5
        bbox

    /// <summary>Returns a world axis-aligned bounding box of one object.
    ///   Estimated bounding boxes can be computed much (much) faster than accurate (or "tight") bounding boxes.
    ///   Estimated bounding boxes are always similar to or larger than accurate bounding boxes.</summary>
    /// <param name="object">(Guid) The identifier of the object</param>
    /// <returns>(Geometry.BoundingBox) The BoundingBox (oriented to the World XY Plane).
    ///    To get the eight 3D points that define the bounding box call box.GetCorners()
    ///    Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBoxEstimate(object:Guid) : BoundingBox =
        let g =  RhinoScriptSyntax.CoerceGeometry object
        g.GetBoundingBox(false) //https://discourse.mcneel.com/t/questions-about-getboundingbox-bool/32092/5


    /// <summary>Returns a world axis-aligned bounding box of several objects.</summary>
    /// <param name="objects">(Guid seq) The identifiers of the objects</param>
    /// <returns>(Geometry.BoundingBox) The BoundingBox (oriented to the World XY Plane).
    ///    To get the eight 3D points that define the bounding box call box.GetCorners()
    ///    Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBox(objects:Guid seq) : BoundingBox =
        let mutable bbox = BoundingBox.Empty
        for o in objects do
            let g =  RhinoScriptSyntax.CoerceGeometry o
            bbox <- BoundingBox.Union(bbox, g.GetBoundingBox(true))
        bbox

    /// <summary>Returns a world axis-aligned bounding box of several geometry objects.</summary>
    /// <param name="geos">(GeometryBase seq) The geometries</param>
    /// <returns>(Geometry.BoundingBox) The BoundingBox (oriented to the World XY Plane).
    ///    To get the eight 3D points that define the bounding box call box.GetCorners()
    ///    Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBox(geos:seq<#GeometryBase>) : BoundingBox =
        let mutable bbox = BoundingBox.Empty
        for g in geos do
            bbox <- BoundingBox.Union(bbox, g.GetBoundingBox(true))
        bbox


    /// <summary>Returns a world axis-aligned bounding box of one object.</summary>
    /// <param name="object">(Guid) The identifier of the object</param>
    /// <returns>(Geometry.BoundingBox) The BoundingBox (oriented to the World XY Plane).
    ///    To get the eight 3D points that define the bounding box call box.GetCorners()
    ///    Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBox(object:Guid) : BoundingBox =
        let g =  RhinoScriptSyntax.CoerceGeometry object
        g.GetBoundingBox(true) //https://discourse.mcneel.com/t/questions-about-getboundingbox-bool/32092/5

    /// <summary>Returns a custom Plane axis-aligned bounding box of several objects.</summary>
    /// <param name="objects">(Guid seq) The identifiers of the objects</param>
    /// <param name="plane">(Plane) Plane to which the bounding box should be aligned</param>
    /// <param name="inWorldCoords">(bool) Optional, default value: <c>true</c>
    ///    Returns the box as world coordinates or custom Plane coordinates.</param>
    /// <returns>(Geometry.Box) The Box ,oriented to the Plane or in Plane coordinates.
    ///    It cannot be a Geometry.BoundingBox since it is not in World XY
    ///    To get the eight 3D points that define the bounding box call box.GetCorners()
    ///    Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBox(objects:Guid seq, plane:Plane, [<OPT;DEF(true)>]inWorldCoords:bool) : Box =
        let mutable bbox = BoundingBox.Empty
        if not plane.IsValid then RhinoScriptingException.Raise "Invalid Geometry.Plane:%s in RhinoScriptSyntax.BoundingBox of %s" plane.Pretty (Pretty.str objects)
        let worldtoplane = Transform.ChangeBasis(Plane.WorldXY, plane)
        objects
        |> Seq.map RhinoScriptSyntax.CoerceGeometry
        |> Seq.iter (fun g -> bbox <- BoundingBox.Union(bbox, g.GetBoundingBox(worldtoplane)))

        if  inWorldCoords then
            let planetoworld = Transform.ChangeBasis(plane, Plane.WorldXY)
            let box = Box(bbox)
            box.Transform(planetoworld) |> RhinoScriptingException.FailIfFalse "plane Transform in rs.BoundingBox()"
            box
        else
            Box(bbox) // return in Plane coordinates not worldxy

    /// <summary>Returns a custom Plane axis-aligned bounding box of one object.</summary>
    /// <param name="object">(Guid) The identifier of the object</param>
    /// <param name="plane">(Plane) Plane to which the bounding box should be aligned</param>
    /// <param name="inWorldCoords">(bool) Optional, default value: <c>true</c>
    ///    Returns the box as world coordinates or custom Plane coordinates.</param>
    /// <returns>(Geometry.Box) The Box ,oriented to the Plane or in Plane coordinates.
    ///    It cannot be a Geometry.BoundingBox since it is not in World XY
    ///    To get the eight 3D points that define the bounding box call box.GetCorners()
    ///    Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBox(object:Guid, plane:Plane, [<OPT;DEF(true)>]inWorldCoords:bool) : Box =
        RhinoScriptSyntax.BoundingBox([object],plane,inWorldCoords)

    /// <summary>Returns a new inflated the box with equal amounts in all directions.
    ///   Inflating with negative amounts may result in decreasing and invalid boxes.
    ///   This function raises an Exception if the resulting box is decreasing.
    ///   Invalid boxes can not be inflated.</summary>
    /// <param name="bbox">(BoundingBox) Geometry.BoundingBox</param>
    /// <param name="amount">(float) amount in model units to expand</param>
    /// <returns>(Geometry.BoundingBox) The new Box.</returns>
    static member BoundingBoxInflate(bbox:BoundingBox, amount:float) : BoundingBox =
        let b = BoundingBox(bbox.Min,bbox.Max)
        b.Inflate(amount)
        if not b.IsValid then RhinoScriptingException.Raise "RhinoScriptSyntax.BoundingBoxInflate Invalid BoundingBox from rs.BoundingBoxInflate by %f on %s" amount bbox.Pretty
        b

    /// <summary>Returns a new inflated box with custom x, y and z amounts in their directions.
    ///   Inflating with negative amounts may result in decreasing and invalid boxes.
    ///   This function raises an Exception if the resulting box is decreasing.
    ///   InValid boxes can not be inflated.</summary>
    /// <param name="bbox">(BoundingBox) Geometry.BoundingBox</param>
    /// <param name="amountX">(float) amount on X Axis in model units to expand</param>
    /// <param name="amountY">(float) amount on X Axis in model units to expand</param>
    /// <param name="amountZ">(float) amount on X Axis in model units to expand</param>
    /// <returns>(Geometry.BoundingBox) The new Box.</returns>
    static member BoundingBoxInflate(bbox:BoundingBox, amountX:float, amountY:float, amountZ:float) : BoundingBox =
        let b = BoundingBox(bbox.Min,bbox.Max)
        b.Inflate(amountX, amountY, amountZ)
        if not b.IsValid then RhinoScriptingException.Raise "RhinoScriptSyntax.BoundingBoxInflate Invalid BoundingBox from rs.BoundingBoxInflate by x:%f, y:%f, z:%f, on %s" amountX amountY amountZ bbox.Pretty
        b


    /// <summary>Compares two objects to determine if they are geometrically identical.</summary>
    /// <param name="first">(Guid) The identifier of the first object to compare</param>
    /// <param name="second">(Guid) The identifier of the second object to compare</param>
    /// <returns>(bool) True if the objects are geometrically identical, otherwise False.</returns>
    static member CompareGeometry(first:Guid, second:Guid) : bool =
        let firstG = RhinoScriptSyntax.CoerceGeometry(first)
        let secondG = RhinoScriptSyntax.CoerceGeometry(second)
        GeometryBase.GeometryEquals(firstG, secondG)


    /// <summary>Creates outline Curves for a given text entity.</summary>
    /// <param name="textId">(Guid) Identifier of Text object to explode</param>
    /// <param name="delete">(bool) Optional, default value: <c>false</c>
    ///    Delete the text object after the Curves have been created</param>
    /// <returns>(Guid array) Array of outline Curves.</returns>
    static member ExplodeText(textId:Guid, [<OPT;DEF(false)>]delete:bool) : ResizeArray<Guid> =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(textId)
        let curves = (rhobj.Geometry:?>TextEntity).Explode()
        let attr = rhobj.Attributes
        let rc = curves  |> RArr.mapArr ( fun curve -> State.Doc.Objects.AddCurve(curve, attr) )
        if delete then State.Doc.Objects.Delete(rhobj, quiet=true) |> ignore<bool>
        State.Doc.Views.Redraw()
        rc


    /// <summary>Checks if that an object is a clipping Plane object. Returns false for any other Rhino object.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True if the object with a given objectId is a clipping Plane.</returns>
    static member IsClippingPlane(objectId:Guid) : bool =
        RhinoScriptSyntax.CoerceGeometry objectId :? ClippingPlaneSurface


    /// <summary>Checks if an object is a point object. Returns false for any other Rhino object.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True if the object with a given objectId is a point.</returns>
    static member IsPoint(objectId:Guid) : bool =
        RhinoScriptSyntax.CoerceGeometry objectId :? Point


    /// <summary>Checks if an object is a point cloud object. Returns false for any other Rhino object.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True if the object with a given objectId is a point cloud.</returns>
    static member IsPointCloud(objectId:Guid) : bool =
        RhinoScriptSyntax.CoerceGeometry objectId :? PointCloud


    /// <summary>Checks if an object is a text object. Returns false for any other Rhino object.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True if the object with a given objectId is a text object.</returns>
    static member IsText(objectId:Guid) : bool =
        RhinoScriptSyntax.CoerceGeometry objectId :? TextEntity


    /// <summary>Checks if an object is a text dot object. Returns false for any other Rhino object.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True if the object with a given objectId is a text dot object.</returns>
    static member IsTextDot(objectId:Guid) : bool =
        RhinoScriptSyntax.CoerceGeometry objectId :? TextDot


    /// <summary>Returns the point count of a point cloud object.</summary>
    /// <param name="objectId">(Guid) The point cloud object's identifier</param>
    /// <returns>(int) number of points.</returns>
    static member PointCloudCount(objectId:Guid) : int =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.Count


    /// <summary>Checks if a point cloud has hidden points.</summary>
    /// <param name="objectId">(Guid) The point cloud object's identifier</param>
    /// <returns>(bool) True if cloud has hidden points, otherwise False.</returns>
    static member PointCloudHasHiddenPoints(objectId:Guid) : bool =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.HiddenPointCount>0


    /// <summary>Checks if a point cloud has point colors.</summary>
    /// <param name="objectId">(Guid) The point cloud object's identifier</param>
    /// <returns>(bool) True if cloud has point colors, otherwise False.</returns>
    static member PointCloudHasPointColors(objectId:Guid) : bool =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.ContainsColors


    /// <summary>Returns the hidden state of points in a point cloud object.</summary>
    /// <param name="objectId">(Guid) The point cloud object's identifier</param>
    /// <returns>(bool ResizeArray) List of point cloud hidden states.</returns>
    static member PointCloudHidePoints(objectId:Guid) : ResizeArray<bool> = //GET
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        let r = ResizeArray(pc.Count)
        for i = 0 to pc.Count - 1 do
            r.Add(pc.[i].Hidden)
        r

    /// <summary>Modifies the hidden points of a point cloud object.</summary>
    /// <param name="objectId">(Guid) The point cloud object's identifier</param>
    /// <param name="hidden">(bool seq) List of booleans matched to the index of points to be hidden, On empty seq all point wil be shown</param>
    /// <returns>(unit) void, nothing.</returns>
    static member PointCloudHidePoints(objectId:Guid, hidden:bool seq) : unit = //SET
        let pc = RhinoScriptSyntax.CoercePointCloud objectId
        if Seq.isEmpty hidden then
            pc.ClearHiddenFlags()

        elif Seq.length(hidden) = pc.Count then
                for i, h in Seq.indexed hidden do
                    pc.[i].Hidden <- h
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.PointCloudHidePoints length of hidden values does not match point cloud point count"

        (RhinoScriptSyntax.CoerceRhinoObject objectId).CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()



    /// <summary>Returns the point colors of a point cloud object.</summary>
    /// <param name="objectId">(Guid) The point cloud object's identifier</param>
    /// <returns>(Drawing.Color ResizeArray) List of point cloud colors.</returns>
    static member PointCloudPointColors(objectId:Guid) : Drawing.Color ResizeArray = //GET
        let pc = RhinoScriptSyntax.CoercePointCloud objectId
        let r = ResizeArray(pc.Count)
        for i = 0 to pc.Count - 1 do
            r.Add(pc.[i].Color)
        r

    /// <summary>Modifies the point colors of a point cloud object.</summary>
    /// <param name="objectId">(Guid) The point cloud object's identifier</param>
    /// <param name="colors">(Drawing.Color seq) List of color values if you want to adjust colors, empty Seq to clear colors</param>
    /// <returns>(unit) void, nothing.</returns>
    static member PointCloudPointColors(objectId:Guid, colors:Drawing.Color seq) : unit = //SET
        let pc = RhinoScriptSyntax.CoercePointCloud objectId
        if colors |> Seq.isEmpty then
            pc.ClearColors()
        elif Seq.length(colors) = pc.Count then
            for i, c in Seq.indexed colors do pc.[i].Color <- c
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.PointCloudPointColors length of color values does not match PointCloud point count"
        (RhinoScriptSyntax.CoerceRhinoObject objectId).CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()



    /// <summary>Returns the points of a point cloud object.</summary>
    /// <param name="objectId">(Guid) The point cloud object's identifier</param>
    /// <returns>(Point3d array) list of points.</returns>
    static member PointCloudPoints(objectId:Guid) : array<Point3d> =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.GetPoints()


    /// <summary>Returns amount indices of points in a point cloud that are near needlePoints.</summary>
    /// <param name="ptCloud">(Point3d seq) The point cloud to be searched, or the "hay stack".
    /// This can also be a list of points</param>
    /// <param name="needlePoints">(Point3d seq) A list of points to search in the PointCloud.
    /// This can also be specified as a point cloud</param>
    /// <param name="amount">(int) Optional, default value: <c>1</c>
    ///    The amount of required closest points. Defaults to 1</param>
    /// <returns>(int array seq) nested lists with amount items within a list, with the indices of the found points.</returns>
    static member PointCloudKNeighbors(ptCloud:Point3d seq, needlePoints:Point3d seq, [<OPT;DEF(1)>]amount:int) : seq<int[]> =
        if Seq.length(needlePoints) > 100 then
            RTree.Point3dKNeighbors(ptCloud, needlePoints, amount)
        else
            Collections.RhinoList.Point3dKNeighbors(ptCloud, needlePoints, amount)


    /// <summary>Returns a list of lists of point indices in a point cloud that are
    ///    closest to needlePoints. Each inner list references all points within or on the Surface of a sphere of distance radius.</summary>
    /// <param name="ptCloud">(Point3d seq) The point cloud to be searched, or the "hay stack". This can also be a list of points</param>
    /// <param name="needlePoints">(Point3d seq) A list of points to search in the PointCloud. This can also be specified as a point cloud</param>
    /// <param name="distance">(float) The included limit for listing points</param>
    /// <returns>(int array seq) a seq of arrays with the indices of the found points.</returns>
    static member PointCloudClosestPoints(ptCloud:Point3d seq, needlePoints:Point3d seq, distance:float) : seq<int []> =
        RTree.Point3dClosestPoints(ptCloud, needlePoints, distance)


    /// <summary>Returns the X, Y, and Z coordinates of a point object.</summary>
    /// <param name="objectId">(Guid) The identifier of a point object</param>
    /// <returns>(Point3d) The current 3-D point location.</returns>
    static member PointCoordinates(objectId:Guid) : Point3d = //GET
        RhinoScriptSyntax.Coerce3dPoint(objectId)


    /// <summary>Modifies the X, Y, and Z coordinates of a point object.</summary>
    /// <param name="objectId">(Guid) The identifier of a point object</param>
    /// <param name="point">(Point3d) A new 3D point location</param>
    /// <returns>(unit) void, nothing.</returns>
    static member PointCoordinates(objectId:Guid, point:Point3d) : unit = //SET
        let pt = RhinoScriptSyntax.Coerce3dPoint(objectId)
        if not <| State.Doc.Objects.Replace(objectId, pt) then RhinoScriptingException.Raise "RhinoScriptSyntax.PointCoordinates failed to change object %s to %s" (Pretty.str objectId) (Pretty.str point)
        State.Doc.Views.Redraw()



    /// <summary>Returns the font of a text dot.</summary>
    /// <param name="objectId">(Guid) Identifier of a text dot object</param>
    /// <returns>(string) The current text dot font.</returns>
    static member TextDotFont(objectId:Guid) : string = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).FontFace

    /// <summary>Modifies the font of a text dot.</summary>
    /// <param name="objectId">(Guid) Identifier of a text dot object</param>
    /// <param name="fontFace">(string) New font face name</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextDotFont(objectId:Guid, fontFace:string) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.FontFace <-  fontFace
        if not <| State.Doc.Objects.Replace(objectId, textdot) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextDotFont failed to change object %s to '%s'" (Pretty.str objectId) fontFace
        State.Doc.Views.Redraw()

    /// <summary>Modifies the font of multiple text dots.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of multiple text dot objects</param>
    /// <param name="fontFace">(string) New font face name</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextDotFont(objectIds:Guid seq, fontFace:string) : unit = //MULTISET
        for objectId in objectIds do
            let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
            textdot.FontFace <-  fontFace
            if not <| State.Doc.Objects.Replace(objectId, textdot) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextDotFont failed to change object %s to '%s'" (Pretty.str objectId) fontFace
        State.Doc.Views.Redraw()


    /// <summary>Returns the font height of a text dot.</summary>
    /// <param name="objectId">(Guid) Identifier of a text dot object</param>
    /// <returns>(int) The current text dot height.</returns>
    static member TextDotHeight(objectId:Guid) : int = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).FontHeight

    /// <summary>Modifies the font height of a text dot.</summary>
    /// <param name="objectId">(Guid) Identifier of a text dot object</param>
    /// <param name="height">(int) New font height</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextDotHeight(objectId:Guid, height:int) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.FontHeight <- height
        if not <| State.Doc.Objects.Replace(objectId, textdot) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextDotHeight failed to change object %s to %d" (Pretty.str objectId) height
        State.Doc.Views.Redraw()

    /// <summary>Modifies the font height of multiple text dots.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of multiple text dot objects</param>
    /// <param name="height">(int) New font height</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextDotHeight(objectIds:Guid seq, height:int) : unit = //MULTISET
        for objectId in objectIds do
            let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
            textdot.FontHeight <- height
            if not <| State.Doc.Objects.Replace(objectId, textdot) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextDotHeight failed to change object %s to %d" (Pretty.str objectId) height
        State.Doc.Views.Redraw()


    /// <summary>Returns the location, or insertion point, on a text dot object.</summary>
    /// <param name="objectId">(Guid) Identifier of a text dot object</param>
    /// <returns>(Point3d) The current 3-D text dot location.</returns>
    static member TextDotPoint(objectId:Guid) : Point3d = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).Point


    /// <summary>Modifies the location, or insertion point, on a text dot object.</summary>
    /// <param name="objectId">(Guid) Identifier of a text dot object</param>
    /// <param name="point">(Point3d) A new 3D point location</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextDotPoint(objectId:Guid, point:Point3d) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.Point <-  point
        if not <| State.Doc.Objects.Replace(objectId, textdot) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextDotPoint failed to change object %s to %s" (Pretty.str objectId) point.Pretty
        State.Doc.Views.Redraw()




    /// <summary>Returns the text on a text dot object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text dot object</param>
    /// <returns>(string) The current text dot text.</returns>
    static member TextDotText(objectId:Guid) : string = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).Text



    /// <summary>Modifies the text on a text dot object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text dot object</param>
    /// <param name="text">(string) A new string for the dot</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextDotText(objectId:Guid, text:string) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.Text <-  text
        if not <| State.Doc.Objects.Replace(objectId, textdot) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextDotText failed to change object %s to '%s'" (Pretty.str objectId) text
        State.Doc.Views.Redraw()

    /// <summary>Modifies the text on multiple text dot objects.</summary>
    /// <param name="objectIds">(Guid seq) The identifiers of multiple text dot objects</param>
    /// <param name="text">(string) A new string for the dot</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextDotText(objectIds:Guid seq, text:string) : unit = //MULTISET
        for objectId in objectIds do
            let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
            textdot.Text <-  text
            if not <| State.Doc.Objects.Replace(objectId, textdot) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextDotText failed to change object %s to '%s'" (Pretty.str objectId) text
        State.Doc.Views.Redraw()


    /// <summary>Returns the font used by a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <returns>(string) The current font face name.</returns>
    static member TextObjectFont(objectId:Guid) : string = //GET
        (RhinoScriptSyntax.CoerceTextEntity(objectId)).Font.QuartetName



    /// <summary>Returns the height of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <returns>(float) The current text height.</returns>
    static member TextObjectHeight(objectId:Guid) : float = //GET
        (RhinoScriptSyntax.CoerceTextEntity(objectId)).TextHeight

    /// <summary>Modifies the height of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <param name="height">(float) The new text height</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectHeight(objectId:Guid, height:float) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        annotation.TextHeight <-  height
        if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectHeight failed.  objectId:'%s' height:'%s'" (Pretty.str objectId) (PrettyFormat.float height)
        State.Doc.Views.Redraw()

    /// <summary>Modifies the height of multiple text objects.</summary>
    /// <param name="objectIds">(Guid seq) The identifiers of multiple text objects</param>
    /// <param name="height">(float) The new text height</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectHeight(objectIds:Guid seq, height:float) : unit = //MULTISET
        for objectId in objectIds do
            let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
            annotation.TextHeight <-  height
            if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectHeight failed.  objectId:'%s' height:'%s'" (Pretty.str objectId) (PrettyFormat.float height)
        State.Doc.Views.Redraw()
        State.Doc.Views.Redraw()

    /// <summary>Returns the Plane used by a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <returns>(Plane) The current Plane.</returns>
    static member TextObjectPlane(objectId:Guid) : Plane = //GET
        (RhinoScriptSyntax.CoerceTextEntity(objectId)).Plane

    /// <summary>Modifies the Plane used by a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <param name="plane">(Plane) The new text object Plane</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectPlane(objectId:Guid, plane:Plane) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        annotation.Plane <-  plane
        if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectPlane failed.  objectId:'%s' plane:'%s'" (Pretty.str objectId) plane.Pretty
        State.Doc.Views.Redraw()


    /// <summary>Returns the location of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <returns>(Point3d) The 3D point identifying the current location.</returns>
    static member TextObjectPoint(objectId:Guid) : Point3d = //GET
        (RhinoScriptSyntax.CoerceTextEntity(objectId)).Plane.Origin

    /// <summary>Modifies the location of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <param name="point">(Point3d) The new text object location</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectPoint(objectId:Guid, point:Point3d) : unit = //SET
        let text = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let mutable plane = text.Plane
        plane.Origin <-  point
        text.Plane <-  plane

        if not <| State.Doc.Objects.Replace(objectId, text) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectPoint failed.  objectId:'%s' point:'%s'" (Pretty.str objectId) point.Pretty
        State.Doc.Views.Redraw()

    /// <summary>Returns the font style of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <returns>(int) The current font style
    ///    0 = Normal
    ///    1 = Bold
    ///    2 = Italic
    ///    3 = Bold and Italic.</returns>
    static member TextObjectStyle(objectId:Guid) : int = //GET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let fontdata = annotation.Font
        let mutable rc = 0
        if fontdata.Bold   then rc <- 1 + rc
        if fontdata.Italic then rc <- 2 + rc
        rc

    /// <summary>Returns the plain text string of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <returns>(string) The current string value.</returns>
    static member TextObjectText(objectId:Guid) : string = //GET
        let text = RhinoScriptSyntax.CoerceTextEntity(objectId)
        text.PlainText

    /// <summary>Modifies the plain text string of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <param name="text">(string) A new text string</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectText(objectId:Guid, text:string) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        annotation.PlainText <-  text
        if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectText failed.  objectId:'%s' text:'%s'" (Pretty.str objectId) text
        State.Doc.Views.Redraw()



    /// <summary>Modifies the plain text string of multiple text objects.</summary>
    /// <param name="objectIds">(Guid seq) The identifiers of multiple text objects</param>
    /// <param name="text">(string) A new text string</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectText(objectIds:Guid seq, text:string) : unit = //MULTISET
        for objectId in objectIds do
            let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
            annotation.PlainText <-  text
            if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectText failed.  objectId:'%s' text:'%s'" (Pretty.str objectId) text
        State.Doc.Views.Redraw()


     /// <summary>Returns the RichText formatting string of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <returns>(string) The current RichText formatting string.</returns>
    static member TextObjectRichText(objectId:Guid) : string = //GET
        let text = RhinoScriptSyntax.CoerceTextEntity(objectId)
        text.RichText


    /// <summary>Modifies the RichText formatting string of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <param name="rtfString">(string) A new text RichText formatting string</param>
    /// <param name="style">(string) Optional, default value: <c>""</c> Name of dimension style</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectRichText(objectId:Guid, rtfString:string,[<OPT;DEF("")>]style:string ) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        if style <> "" then
            let ds = State.Doc.DimStyles.FindName(style)
            if isNull ds then  RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectRichText, style not found:'%s'"  style
            annotation.SetRichText(rtfString, ds)
        else
            annotation.RichText <-  rtfString
        if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectRichText failed.  objectId:'%s' text:'%s'" (Pretty.str objectId) rtfString
        State.Doc.Views.Redraw()



    /// <summary>Modifies the RichText formatting string of multiple text objects.</summary>
    /// <param name="objectIds">(Guid seq) The identifiers of multiple text objects</param>
    /// <param name="rtfString">(string) A new text RichText formatting string</param>
    /// <param name="style">(string) Optional, default value: <c>""</c> Name of dimension style</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectRichText(objectIds:Guid seq,  rtfString:string, [<OPT;DEF("")>]style:string ) : unit = //MULTISET
        if style <> "" then
            let ds = State.Doc.DimStyles.FindName(style)
            if isNull ds then  RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectRichText, style not found:'%s'"  style
            for objectId in objectIds do
                let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
                if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectRichText failed.  objectId:'%s' text:'%s' style:'%s'" (Pretty.str objectId) rtfString style
                annotation.SetRichText(rtfString, ds)
        else
            for objectId in objectIds do
                let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
                annotation.RichText <-  rtfString
                if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectRichText failed.  objectId:'%s' text:'%s'" (Pretty.str objectId) rtfString
        State.Doc.Views.Redraw()



    /// <summary>Modifies the font style of a text object.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <param name="style">(int) The font style. Can be any of the following flags
    ///    0 = Normal
    ///    1 = Bold
    ///    2 = Italic
    ///    3 = Bold and Italic</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectStyle(objectId:Guid, style:int) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let fontdata = annotation.Font
        let f =
            match style with
            |3 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=true , italic=true)
            |2 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=false, italic=true)
            |1 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=true , italic=false)
            |0 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=false, italic=false)
            |_ -> (RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectStyle failed.  objectId:'%s' bad style:%d" (Pretty.str objectId) style)
        if isNull f then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectStyle failed.  objectId:'%s' style:%d not available for %s" (Pretty.str objectId) style fontdata.QuartetName
        if not <| State.Doc.Objects.Replace(objectId, annotation) then
            RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectStyle failed.  objectId:'%s' bad style:%d" (Pretty.str objectId) style
        State.Doc.Views.Redraw()

    /// <summary>Modifies the font style of multiple text objects. Keeps the font face</summary>
    /// <param name="objectIds">(Guid seq) The identifiers of multiple text objects</param>
    /// <param name="style">(int) The font style. Can be any of the following flags
    ///    0 = Normal
    ///    1 = Bold
    ///    2 = Italic
    ///    3 = Bold and Italic</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectStyle(objectIds:Guid seq, style:int) : unit = //MULTISET
        for objectId in objectIds do RhinoScriptSyntax.TextObjectStyle(objectId, style)


    /// <summary>Modifies the font used by a text object. Keeps the current state of bold and italic when possible.</summary>
    /// <param name="objectId">(Guid) The identifier of a text object</param>
    /// <param name="font">(string) The new Font Name</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectFont(objectId:Guid, font:string) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let fontdata = annotation.Font
        let f =
            DocObjects.Font.FromQuartetProperties(font, fontdata.Bold, fontdata.Italic)
            // normally calls will not  go further than FromQuartetProperties(font, false, false)
            // but there are a few rare fonts that don"t have a regular font
            |? DocObjects.Font.FromQuartetProperties(font, bold=false, italic=false)
            |? DocObjects.Font.FromQuartetProperties(font, bold=true , italic=false)
            |? DocObjects.Font.FromQuartetProperties(font, bold=false, italic=true )
            |? DocObjects.Font.FromQuartetProperties(font, bold=true , italic=true )
            |? (RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectFont failed.  objectId:'%s' font:''%s''" (Pretty.str objectId) font)
        annotation.Font <- f
        if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectFont failed.  objectId:'%s' font:''%s''" (Pretty.str objectId) font
        State.Doc.Views.Redraw()
        State.Doc.Views.Redraw()

    /// <summary>Modifies the font used by multiple text objects. Keeps the current state of bold and italic when possible.</summary>
    /// <param name="objectIds">(Guid seq) The identifiers of multiple text objects</param>
    /// <param name="font">(string) The new Font Name</param>
    /// <returns>(unit) void, nothing.</returns>
    static member TextObjectFont(objectIds:Guid seq, font:string) : unit = //MULTISET
        for objectId in objectIds do  RhinoScriptSyntax.TextObjectFont(objectId, font)
        State.Doc.Views.Redraw()

