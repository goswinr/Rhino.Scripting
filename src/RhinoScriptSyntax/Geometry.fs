namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument


[<AutoOpen>]
module ExtensionsGeometry =
    type RhinoScriptSyntax with

    [<EXT>]
    ///<summary>Create a clipping plane for visibly clipping away geometry in a specific
    ///  view. Note, clipping planes are infinite</summary>
    ///<param name="plane">(Plane) The plane</param>
    ///<param name="uMagnitude">(float) U magnitude of the plane</param>
    ///<param name="vMagnitude">(float) V magnitude of the plane</param>
    ///<param name="views">(string seq) Optional, Titles the the view(s) to clip. If omitted, the active
    ///  view is used</param>
    ///<returns>(Guid) object identifier on success</returns>
    static member AddClippingPlane( plane:Plane, 
                                    uMagnitude:float, 
                                    vMagnitude:float, 
                                    [<OPT;DEF(null:string seq)>]views:string seq) : Guid =
        let viewlist =
            if isNull views then [Doc.Views.ActiveView.ActiveViewportID]
            else
                let modelviews = Doc.Views.GetViewList(true, false)
                [for view in views do
                    for item in modelviews do
                        if item.ActiveViewport.Name = view then
                            yield item.ActiveViewportID]
        let rc = Doc.Objects.AddClippingPlane(plane, uMagnitude, vMagnitude, viewlist)
        if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add clipping plane to document.  plane:'%A' uMagnitude:'%A' vMagnitude:'%A' views:'%A'" plane uMagnitude vMagnitude views
        Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Creates a picture frame and adds it to the document</summary>
    ///<param name="plane">(Plane) The plane in which the PictureFrame will be created.  The bottom-left corner of picture will be at plane's origin. The width will be in the plane's X axis direction, and the height will be in the plane's Y axis direction</param>
    ///<param name="filename">(string) The path to a bitmap or image file</param>
    ///<param name="width">(float) Optional, If both dblWidth and dblHeight are 0.0 or skiped, then the width and height of the PictureFrame will be the width and height of the image. If dblWidth = 0 and dblHeight is > 0, or if dblWidth > 0 and dblHeight = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, which ever one is = 0. If both dblWidth and dblHeight are > 0, then these are assumed to be the width and height of in the current unit system</param>
    ///<param name="height">(float) Optional, If both dblWidth and dblHeight are  0.0 or skied, then the width and height of the PictureFrame will be the width and height of the image. If dblWidth = 0 and dblHeight is > 0, or if dblWidth > 0 and dblHeight = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, which ever one is = 0. If both dblWidth and dblHeight are > 0, then these are assumed to be the width and height of in the current unit system</param>
    ///<param name="selfIllumination">(bool) Optional, Default Value: <c>true</c>
    ///If True, then the image mapped to the picture frame plane always displays at full intensity and is not affected by light or shadow</param>
    ///<param name="embed">(bool) Optional, Default Value: <c>false</c>
    ///If True, then the function adds the image to Rhino's internal bitmap table, thus making the document self-contained</param>
    ///<param name="useAlpha">(bool) Optional, Default Value: <c>false</c>
    ///If False, the picture frame is created without any transparency texture.  If True, a transparency texture is created with a "mask texture" set to alpha, and an instance of the diffuse texture in the source texture slot</param>
    ///<param name="makeMesh">(bool) Optional, Default Value: <c>false</c>
    ///If True, the function will make a PictureFrame object from a mesh rather than a plane surface</param>
    ///<returns>(Guid) object identifier on success</returns>
    static member AddPictureFrame(  plane:Plane, 
                                    filename:string, 
                                    [<OPT;DEF(0.0)>]width:float, 
                                    [<OPT;DEF(0.0)>]height:float, 
                                    [<OPT;DEF(true)>]selfIllumination:bool, 
                                    [<OPT;DEF(false)>]embed:bool, 
                                    [<OPT;DEF(false)>]useAlpha:bool, 
                                    [<OPT;DEF(false)>]makeMesh:bool) : Guid =
        if not <| IO.File.Exists(filename) then failwithf "image %s does not exist" filename
        let rc = Doc.Objects.AddPictureFrame(plane, filename, makeMesh, width, height, selfIllumination, embed)
        if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add picture frame to document.  plane:'%A' filename:'%A' width:'%A' height:'%A' selfIllumination:'%A' embed:'%A' useAlpha:'%A' makeMesh:'%A'" plane filename width height selfIllumination embed useAlpha makeMesh
        Doc.Views.Redraw()
        rc

    [<EXT>]
    ///<summary>Adds point object to the document</summary>
    ///<param name="X">(float) X location of point to add</param>
    ///<param name="y">(float) Y location of point to add</param>
    ///<param name="z">(float) Z location of point to add</param>
    ///<returns>(Guid) identifier for the object that was added to the doc</returns>
    static member AddPoint(x:float, y:float, z:float) : Guid =
        let rc = Doc.Objects.AddPoint(Point3d(x, y, x))
        if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add point to document.  x:'%A' y:'%A' z:'%A'" x y z
        Doc.Views.Redraw()
        rc

    [<EXT>]
    ///<summary>Adds point object to the document</summary>
    ///<param name="point">(Point3d) point to draw</param>
    ///<returns>(Guid) identifier for the object that was added to the doc</returns>
    static member AddPoint(point:Point3d) : Guid =
        let rc = Doc.Objects.AddPoint(point)
        if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add point to document.  point:'%A' " point
        Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Adds point cloud object to the document</summary>
    ///<param name="points">(Point3d array) List of values where every multiple of three represents a point</param>
    ///<param name="colors">(Drawing.Color array) Optional, List of colors to apply to each point</param>
    ///<returns>(Guid) identifier of point cloud on success</returns>
    static member AddPointCloud(points:Point3d [], [<OPT;DEF(null:Drawing.Color seq)>]colors:Drawing.Color []) : Guid =
        if notNull colors && Seq.length(colors) = Seq.length(points) then
            let pc = new PointCloud()
            for i = 0  to -1 + (Seq.length(points)) do
                let color = RhinoScriptSyntax.CoerceColor(colors.[i])
                pc.Add(points.[i],color)
            let rc = Doc.Objects.AddPointCloud(pc)
            if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add point cloud to document.  points:'%A' colors:'%A'" points colors
            Doc.Views.Redraw()
            rc
        else
            let rc = Doc.Objects.AddPointCloud(points)
            if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add point cloud to document.  points:'%A' colors:'%A'" points colors
            Doc.Views.Redraw()
            rc


    [<EXT>]
    ///<summary>Adds one or more point objects to the document</summary>
    ///<param name="points">(Point3d seq) List of points</param>
    ///<returns>(Guid ResizeArray) List of identifiers of the new objects on success</returns>
    static member AddPoints(points:Point3d seq) : Guid ResizeArray =
        let rc = resizeArray{ for point in points do yield Doc.Objects.AddPoint(point) }
        Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Adds a text string to the document</summary>
    ///<param name="text">(string) The text to display</param>
    ///<param name="plane">(Plane) the plane on which the text will lie.
    ///  The origin of the plane will be the origin point of the text</param>
    ///<param name="height">(float) Optional, Default Value: <c>1.0</c>
    ///The text height</param>
    ///<param name="font">(string) Optional, The text font</param>
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

        if isNull text || text = "" then failwithf "Rhino.Scripting: Text invalid.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
        let bold = (1 = fontStyle || 3 = fontStyle)
        let italic = (2 = fontStyle || 3 = fontStyle)
        let ds = Doc.DimStyles.Current
        let qn, quartetBoldProp ,quartetItalicProp =
            if isNull font then
              ds.Font.QuartetName, ds.Font.Bold, ds.Font.Italic
            else
              font, false, false

        let f = DocObjects.Font.FromQuartetProperties(qn, quartetBoldProp, quartetItalicProp)

        if isNull f then
            failwithf "Rhino.Scripting: AddText failed.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
        let te = TextEntity.Create(text, plane, ds, false, 0.0, 0.0)
        te.TextHeight <- height
        if font |> notNull then
          te.Font <- f
        if bold <> quartetBoldProp then
            if DocObjects.Font.FromQuartetProperties(qn, bold, false) |> isNull then
              failwithf "Rhino.Scripting: AddText failed.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
            else
              te.SetBold(bold)|> ignore
        if italic <> quartetItalicProp then
            if DocObjects.Font.FromQuartetProperties(qn, false, italic) |> isNull then
              failwithf "Rhino.Scripting: AddText failed.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
            else
              te.SetItalic(italic)|> ignore

        te.TextHorizontalAlignment <- horizontalAlignment
        te.TextVerticalAlignment <- verticalAlignment
        let objectId = Doc.Objects.Add(te);
        if objectId = Guid.Empty then failwithf "Rhino.Scripting: Unable to add text to document.  text:'%A' plane:'%A' height:'%A' font:'%A' fontStyle:'%A' horizontalAlignment '%A' verticalAlignment:'%A'" text plane height font fontStyle horizontalAlignment verticalAlignment
        Doc.Views.Redraw()
        objectId


    [<EXT>]
    ///<summary>Add a text dot to the document</summary>
    ///<param name="text">(string) String in dot</param>
    ///<param name="point">(Point3d) A 3D point identifying the origin point</param>
    ///<returns>(Guid) The identifier of the new object</returns>
    static member AddTextDot(text:string, point:Point3d) : Guid =
        let rc = Doc.Objects.AddTextDot(text, point)
        if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add text dot to document.  text:'%A' point:'%A'" text point
        Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Compute the area of a closed curve, hatch, surface, polysurface, or mesh</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(float) area</returns>
    static member Area(objectId:Guid) : float =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let mp = AreaMassProperties.Compute([rhobj.Geometry])
        if mp |> isNull then failwithf "Rhino.Scripting: Unable to compute area mass properties.  objectId:'%A'" objectId
        mp.Area


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
    ///  world axis-aligned bounding boxes</param>
    ///<returns>(Point3d array) Eight 3D points that define the bounding box.
    ///  Points returned in counter-clockwise order starting with the bottom rectangle of the box</returns>
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




    [<EXT>]
    ///<summary>Compares two objects to determine if they are geometrically identical</summary>
    ///<param name="first">(Guid) The identifier of the first object to compare</param>
    ///<param name="second">(Guid) The identifier of the second object to compare</param>
    ///<returns>(bool) True if the objects are geometrically identical, otherwise False</returns>
    static member CompareGeometry(first:Guid, second:Guid) : bool =
        let firstG = RhinoScriptSyntax.CoerceGeometry(first)
        let secondG = RhinoScriptSyntax.CoerceGeometry(second)
        GeometryBase.GeometryEquals(firstG, secondG)


    [<EXT>]
    ///<summary>Creates outline curves for a given text entity</summary>
    ///<param name="textId">(Guid) Identifier of Text object to explode</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///Delete the text object after the curves have been created</param>
    ///<returns>(Guid array) of outline curves</returns>
    static member ExplodeText(textId:Guid, [<OPT;DEF(false)>]delete:bool) : ResizeArray<Guid> =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(textId)
        let curves = (rhobj.Geometry:?>TextEntity).Explode()
        let attr = rhobj.Attributes
        let rc = resizeArray { for curve in curves do yield Doc.Objects.AddCurve(curve, attr) }
        if delete then Doc.Objects.Delete(rhobj, true) |>ignore
        Doc.Views.Redraw()
        rc


    [<EXT>]
    ///<summary>Verifies that an object is a clipping plane object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given objectId is a clipping plane</returns>
    static member IsClippingPlane(objectId:Guid) : bool =
        let pc = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if pc.IsNone then false else pc.Value :? ClippingPlaneSurface


    [<EXT>]
    ///<summary>Verifies an object is a point object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given objectId is a point</returns>
    static member IsPoint(objectId:Guid) : bool =
        let p = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if p.IsNone then false else p.Value :? Point


    [<EXT>]
    ///<summary>Verifies an object is a point cloud object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given objectId is a point cloud</returns>
    static member IsPointCloud(objectId:Guid) : bool =
        let pc = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if pc.IsNone then false else pc.Value :? PointCloud


    [<EXT>]
    ///<summary>Verifies an object is a text object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given objectId is a text object</returns>
    static member IsText(objectId:Guid) : bool =
        let p = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if p.IsNone then false else p.Value :? TextEntity


    [<EXT>]
    ///<summary>Verifies an object is a text dot object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if the object with a given objectId is a text dot object</returns>
    static member IsTextDot(objectId:Guid) : bool =
        let p = RhinoScriptSyntax.TryCoerceGeometry(objectId)
        if p.IsNone then false else p.Value :? TextDot


    [<EXT>]
    ///<summary>Returns the point count of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(int) number of points</returns>
    static member PointCloudCount(objectId:Guid) : int =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.Count


    [<EXT>]
    ///<summary>Verifies that a point cloud has hidden points</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(bool) True if cloud has hidden points, otherwise False</returns>
    static member PointCloudHasHiddenPoints(objectId:Guid) : bool =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.HiddenPointCount>0


    [<EXT>]
    ///<summary>Verifies that a point cloud has point colors</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(bool) True if cloud has point colors, otherwise False</returns>
    static member PointCloudHasPointColors(objectId:Guid) : bool =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.ContainsColors


    [<EXT>]
    ///<summary>Returns the hidden points of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(bool ResizeArray) List of point cloud hidden states</returns>
    static member PointCloudHidePoints(objectId:Guid) : ResizeArray<bool> = //GET
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        resizeArray { for item in pc do yield item.Hidden }


    [<EXT>]
    ///<summary>Modifies the hidden points of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<param name="hidden">(bool seq) List of booleans matched to the index of points to be hidden, On empty seq all point wil be shown</param>
    ///<returns>(unit) void, nothing</returns>
    static member PointCloudHidePoints(objectId:Guid, hidden:bool seq) : unit = //SET
        let pc = RhinoScriptSyntax.CoercePointCloud objectId
        if Seq.isEmpty hidden then
            pc.ClearHiddenFlags()

        elif Seq.length(hidden) = pc.Count then
                for i, h in Seq.indexed hidden do
                    pc.[i].Hidden <- h
        else
            failwithf "PointCloudHidePoints length of hidden values does not match point cloud point count"

        (RhinoScriptSyntax.CoerceRhinoObject objectId).CommitChanges()|> ignore
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the point colors of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(Drawing.Color ResizeArray) List of point cloud colors</returns>
    static member PointCloudPointColors(objectId:Guid) : Drawing.Color ResizeArray = //GET
        let pc = RhinoScriptSyntax.CoercePointCloud objectId
        resizeArray { for item in pc do yield item.Color }

    [<EXT>]
    ///<summary>Modifies the point colors of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<param name="colors">(Drawing.Color seq) List of color values if you want to adjust colors, empty Seq to clear colors</param>
    ///<returns>(unit) void, nothing</returns>
    static member PointCloudPointColors(objectId:Guid, colors:Drawing.Color seq) : unit = //SET
        let pc = RhinoScriptSyntax.CoercePointCloud objectId
        if colors |> Seq.isEmpty then
            pc.ClearColors()
        elif Seq.length(colors) = pc.Count then
            for i, c in Seq.indexed colors do pc.[i].Color <- c
        else
            failwithf "PointCloudHidePoints length of hidden values does not match point cloud point count"
        (RhinoScriptSyntax.CoerceRhinoObject objectId).CommitChanges()|> ignore
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the points of a point cloud object</summary>
    ///<param name="objectId">(Guid) The point cloud object's identifier</param>
    ///<returns>(Point3d array) list of points</returns>
    static member PointCloudPoints(objectId:Guid) : array<Point3d> =
        let pc = RhinoScriptSyntax.CoercePointCloud(objectId)
        pc.GetPoints()





    [<EXT>]
    ///<summary>Returns amount indices of points in a point cloud that are near needlePoints</summary>
    ///<param name="ptCloud">(Point3d seq) The point cloud to be searched, or the "hay stack". This can also be a list of points</param>
    ///<param name="needlePoints">(Point3d seq) A list of points to search in the pointcloud. This can also be specified as a point cloud</param>
    ///<param name="amount">(int) Optional, Default Value: <c>1</c>
    ///The amount of required closest points. Defaults to 1</param>
    ///<returns>(seq<int array>) nested lists with amount items within a list, with the indices of the found points</returns>
    static member PointCloudKNeighbors(ptCloud:Point3d seq, needlePoints:Point3d seq, [<OPT;DEF(1)>]amount:int) : seq<int[]> =
        if Seq.length(needlePoints) > 100 then
            RTree.Point3dKNeighbors(ptCloud, needlePoints, amount)
        else
            Collections.RhinoList.Point3dKNeighbors(ptCloud, needlePoints, amount)






    [<EXT>]
    ///<summary>Returns a list of lists of point indices in a point cloud that are
    ///  closest to needlePoints. Each inner list references all points within or on the surface of a sphere of distance radius</summary>
    ///<param name="ptCloud">(Point3d seq) The point cloud to be searched, or the "hay stack". This can also be a list of points</param>
    ///<param name="needlePoints">(Point3d seq) A list of points to search in the pointcloud. This can also be specified as a point cloud</param>
    ///<param name="distance">(float) The included limit for listing points</param>
    ///<returns>(seq<int array>) a seq of arrays with the indices of the found points</returns>
    static member PointCloudClosestPoints(ptCloud:Point3d seq, needlePoints:Point3d seq, distance:float) : seq<int []> =
        RTree.Point3dClosestPoints(ptCloud, needlePoints, distance)




    [<EXT>]
    ///<summary>Returns the X, Y, and Z coordinates of a point object</summary>
    ///<param name="objectId">(Guid) The identifier of a point object</param>
    ///<returns>(Point3d) The current 3-D point location</returns>
    static member PointCoordinates(objectId:Guid) : Point3d = //GET
        RhinoScriptSyntax.Coerce3dPoint(objectId)


    ///<summary>Modifies the X, Y, and Z coordinates of a point object</summary>
    ///<param name="objectId">(Guid) The identifier of a point object</param>
    ///<param name="point">(Point3d) A new 3D point location</param>
    ///<returns>(unit) void, nothing</returns>
    static member PointCoordinates(objectId:Guid, point:Point3d) : unit = //SET
        let pt = RhinoScriptSyntax.Coerce3dPoint(objectId)
        if not <| Doc.Objects.Replace(objectId, pt) then failwithf "PointCoordinates failed to change object %A to %A " objectId point
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the font of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<returns>(string) The current text dot font</returns>
    static member TextDotFont(objectId:Guid) : string = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).FontFace

    ///<summary>Modifies the font of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<param name="fontface">(string) New font face name</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotFont(objectId:Guid, fontface:string) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.FontFace <-  fontface
        if not <| Doc.Objects.Replace(objectId, textdot) then failwithf "TextDotFont failed to change object %A to %A " objectId fontface
        Doc.Views.Redraw()


    [<EXT>]
    ///<summary>Returns the font height of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<returns>(int) The current text dot height</returns>
    static member TextDotHeight(objectId:Guid) : int = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).FontHeight

    ///<summary>Modifies the font height of a text dot</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<param name="height">(int) New font height</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotHeight(objectId:Guid, height:int) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.FontHeight <- height
        if not <| Doc.Objects.Replace(objectId, textdot) then failwithf "TextDotHeight failed to change object %A to %A " objectId height
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the location, or insertion point, on a text dot object</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<returns>(Point3d) The current 3-D text dot location</returns>
    static member TextDotPoint(objectId:Guid) : Point3d = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).Point


    ///<summary>Modifies the location, or insertion point, on a text dot object</summary>
    ///<param name="objectId">(Guid) Identifier of a text dot object</param>
    ///<param name="point">(Point3d) A new 3D point location</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotPoint(objectId:Guid, point:Point3d) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.Point <-  point
        if not <| Doc.Objects.Replace(objectId, textdot) then failwithf "TextDotPoint failed to change object %A to %A " objectId point
        Doc.Views.Redraw()




    [<EXT>]
    ///<summary>Returns the text on a text dot object</summary>
    ///<param name="objectId">(Guid) The identifier of a text dot object</param>
    ///<returns>(string) The current text dot text</returns>
    static member TextDotText(objectId:Guid) : string = //GET
        (RhinoScriptSyntax.CoerceTextDot(objectId)).Text



    ///<summary>Modifies the text on a text dot object</summary>
    ///<param name="objectId">(Guid) The identifier of a text dot object</param>
    ///<param name="text">(string) A new string for the dot</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextDotText(objectId:Guid, text:string) : unit = //SET
        let textdot = RhinoScriptSyntax.CoerceTextDot(objectId)
        textdot.Text <-  text
        if not <| Doc.Objects.Replace(objectId, textdot) then failwithf "TextDotText failed to change object %A to %A " objectId text
        Doc.Views.Redraw()




    [<EXT>]
    ///<summary>Returns the font used by a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(string) The current font face name</returns>
    static member TextObjectFont(objectId:Guid) : string = //GET
        (RhinoScriptSyntax.CoerceTextEntity(objectId)).Font.QuartetName





    static member TextObjectFont(objectId:Guid, font:string) : unit = //SET
        let objectId = RhinoScriptSyntax.CoerceGuid(objectId)
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let fontdata = annotation.Font
        let f =
            DocObjects.Font.FromQuartetProperties(font, fontdata.Bold, fontdata.Italic)
            // normally calls will not  go further than FromQuartetProperties(font, false, false)
            // but there are a few rare fonts that don"t have a regular font
            |? DocObjects.Font.FromQuartetProperties(font, false, false)
            |? DocObjects.Font.FromQuartetProperties(font, true, false)
            |? DocObjects.Font.FromQuartetProperties(font, false, true)
            |? DocObjects.Font.FromQuartetProperties(font, true, true)
            |? failwithf "Rhino.Scripting: TextObjectFont failed.  objectId:'%A' font:'%A'" objectId font

        annotation.Font <- f
        if not <| Doc.Objects.Replace(objectId, annotation) then failwithf "Rhino.Scripting: TextObjectFont failed.  objectId:'%A' font:'%A'" objectId font
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the height of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(float) The current text height</returns>
    static member TextObjectHeight(objectId:Guid) : float = //GET
        (RhinoScriptSyntax.CoerceTextEntity(objectId)).TextHeight

    ///<summary>Modifies the height of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="height">(float) The new text height</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectHeight(objectId:Guid, height:float) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        annotation.TextHeight <-  height
        let objectId = RhinoScriptSyntax.CoerceGuid(objectId)
        if not <| Doc.Objects.Replace(objectId, annotation) then failwithf "Rhino.Scripting: TextObjectHeight failed.  objectId:'%A' height:'%A'" objectId height
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the plane used by a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(Plane) The current plane</returns>
    static member TextObjectPlane(objectId:Guid) : Plane = //GET
        (RhinoScriptSyntax.CoerceTextEntity(objectId)).Plane

    ///<summary>Modifies the plane used by a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="plane">(Plane) The new text object plane</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectPlane(objectId:Guid, plane:Plane) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        annotation.Plane <-  plane
        let objectId = RhinoScriptSyntax.CoerceGuid(objectId)
        if not <| Doc.Objects.Replace(objectId, annotation) then failwithf "Rhino.Scripting: TextObjectPlane failed.  objectId:'%A' plane:'%A'" objectId plane
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the location of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(Point3d) The 3D point identifying the current location</returns>
    static member TextObjectPoint(objectId:Guid) : Point3d = //GET
        (RhinoScriptSyntax.CoerceTextEntity(objectId)).Plane.Origin

    ///<summary>Modifies the location of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="point">(Point3d) The new text object location</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectPoint(objectId:Guid, point:Point3d) : unit = //SET
        let text = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let mutable plane = text.Plane
        plane.Origin <-  point
        text.Plane <-  plane
        let objectId = RhinoScriptSyntax.CoerceGuid(objectId)
        if not <| Doc.Objects.Replace(objectId, text) then failwithf "Rhino.Scripting: TextObjectPoint failed.  objectId:'%A' point:'%A'" objectId point
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the font style of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(int) The current font style
    ///  0 = Normal
    ///  1 = Bold
    ///  2 = Italic
    ///  3 = Bold and Italic</returns>
    static member TextObjectStyle(objectId:Guid) : int = //GET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let fontdata = annotation.Font
        let mutable rc = 0
        if fontdata.Bold   then rc <- 1 + rc
        if fontdata.Italic then rc <- 2 + rc
        rc

    ///<summary>Modifies the font style of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="style">(int) The font style. Can be any of the following flags
    ///  0 = Normal
    ///  1 = Bold
    ///  2 = Italic
    ///  3 = Bold and Italic</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectStyle(objectId:Guid, style:int) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let fontdata = annotation.Font
        let f =
            match style with
            |3 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, true, true)
            |2 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, false, true)
            |1 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, true, false)
            |0 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, false, false)
            |_ -> failwithf "Rhino.Scripting: TextObjectStyle failed.  objectId:'%A' bad style:'%A'" objectId style
            |?    failwithf "Rhino.Scripting: TextObjectStyle failed.  objectId:'%A' style:'%A' not availabe for %s" objectId style fontdata.QuartetName

        let objectId = RhinoScriptSyntax.CoerceGuid(objectId)
        if not <| Doc.Objects.Replace(objectId, annotation) then failwithf "Rhino.Scripting: TextObjectStyle failed.  objectId:'%A' bad style:'%A'" objectId style
        Doc.Views.Redraw()



    [<EXT>]
    ///<summary>Returns the text string of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<returns>(string) The current string value</returns>
    static member TextObjectText(objectId:Guid) : string = //GET
        let text = RhinoScriptSyntax.CoerceTextEntity(objectId)
        text.PlainText


    ///<summary>Modifies the text string of a text object</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="text">(string) A new text string</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextObjectText(objectId:Guid, text:string) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        annotation.PlainText <-  text
        let objectId = RhinoScriptSyntax.CoerceGuid(objectId)
        if not <| Doc.Objects.Replace(objectId, annotation) then failwithf "Rhino.Scripting: TextObjectText failed.  objectId:'%A' text:'%A'" objectId text
        Doc.Views.Redraw()



