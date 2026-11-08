namespace Rhino.Scripting

open Rhino
open System.ComponentModel
open System
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils

[<AutoOpen; EditorBrowsable(EditorBrowsableState.Never)>]
module AutoOpenDimension =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and IronPython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    /// <summary>Adds an aligned dimension object to the document. An aligned dimension
    ///    is a linear dimension lined up with two points.</summary>
    /// <param name="startPoint">(Point3d) First point of the dimension.</param>
    /// <param name="endPoint">(Point3d) Second point of the dimension.</param>
    /// <param name="pointOnDimensionLine">(Point3d) Location point of the dimension line.</param>
    /// <param name="style">(string) Optional, default value: <c>""</c>. Name of the dimension style.</param>
    /// <returns>(Guid) Identifier of the new dimension.</returns>
    static member AddAlignedDimension(  startPoint:Point3d,
                                        endPoint:Point3d,
                                        pointOnDimensionLine:Point3d,  // TODO allow Point3d.Unset an then draw dim in XY plane
                                        [<OPT;DEF("")>]style:string) : Guid =
        let plane =
            let candidate = Geometry.Plane(startPoint, endPoint, pointOnDimensionLine)
            if candidate.IsValid then candidate
            else (RhinoScriptSyntax.CoerceViewOrActive(null)).ActiveViewport.ConstructionPlane()
        let _, s, t = plane.ClosestParameter(startPoint)
        let start2 = Point2d(s, t)
        let _, s, t = plane.ClosestParameter(endPoint)
        let ende2 = Point2d(s, t)
        let _, s, t = plane.ClosestParameter(pointOnDimensionLine)
        let onpoint2 = Point2d(s, t)
        let ldim = new LinearDimension(plane, start2, ende2, onpoint2)
        if isNull ldim then  RhinoScriptingException.Raise "AddAlignedDimension failed.startPoint:'%O' endPoint:'%O' pointOnDimensionLine:'%O' style:'%s'" startPoint endPoint pointOnDimensionLine style
        ldim.Aligned <- true
        if style <> "" then
            let ds = State.Doc.DimStyles.FindName(style)
            if isNull ds then  RhinoScriptingException.Raise "AddAlignedDimension, style not found.  startPoint:'%O' endPoint:'%O' pointOnDimensionLine:'%O' style:'%s'" startPoint endPoint pointOnDimensionLine style
            ldim.DimensionStyleId <- ds.Id
        let rc = State.Doc.Objects.AddLinearDimension(ldim)
        if rc = Guid.Empty then  RhinoScriptingException.Raise "AddAlignedDimension: Unable to add dimension to document. startPoint:'%O' endPoint:'%O' pointOnDimensionLine:'%O' style:'%s'" startPoint endPoint pointOnDimensionLine style
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a new dimension style to the document. The new dimension style will
    ///    be initialized with the current default dimension style properties.</summary>
    /// <param name="dimStyleName">(string) Name of the new dimension style.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member AddDimStyle(dimStyleName:string) : unit =
        let index = State.Doc.DimStyles.Add(dimStyleName)
        if index<0 then  RhinoScriptingException.Raise "AddDimStyle failed. dimStyleName:'%A'" dimStyleName



    /// <summary>Adds a dimension style with an automatically generated name and the current default properties.</summary>
    /// <returns>(string) The name assigned to the new dimension style.</returns>
    static member AddDimStyle() : string =
        let index = State.Doc.DimStyles.Add(null:string)
        if index < 0 then RhinoScriptingException.Raise "AddDimStyle: unable to add an automatically named dimension style"
        State.Doc.DimStyles.[index].Name


    /// <summary>Adds a leader to the document. Leader objects are planar.
    ///    The 3D points passed will define the plane if no plane is given.
    ///    If there are only two points, the World XY plane is used, except for a vertical segment, which uses a plane containing the segment.</summary>
    /// <param name="points">(Point3d seq) List of (at least 2) 3D points.</param>
    /// <param name="text">(string) Optional, default value: <c>""</c>. Leader text; omit to create a leader without text.</param>
    /// <param name="plane">(Geometry.Plane) Optional, default value: <c>defined by points arg</c>.
    ///    Points will be projected to this plane if provided.</param>
    /// <returns>(Guid) Identifier of the new leader.</returns>
    static member AddLeader( points:Point3d seq,
                             [<OPT;DEF("")>]text:string,
                             [<OPT;DEF(Plane())>]plane:Plane) : Guid =
        let points = points |> Seq.toArray
        if points.Length < 2 then RhinoScriptingException.Raise "AddLeader: at least two points are required"
        let points2d = ResizeArray()
        let plane0 =
            if plane.IsValid then plane
            elif points.Length = 2 then
                let direction = points.[1] - points.[0]
                if direction.IsTiny() then RhinoScriptingException.Raise "AddLeader: the two points must be distinct"
                if abs direction.X <= RhinoMath.ZeroTolerance && abs direction.Y <= RhinoMath.ZeroTolerance then
                    Plane(points.[0], direction, Vector3d.XAxis)
                else Plane.WorldXY
            else
                let result, fitted = Plane.FitPlaneToPoints(points)
                if result = PlaneFitResult.Failure then RhinoScriptingException.Raise "AddLeader: unable to fit a plane to the points"
                fitted

        for point in points do
            let cprc, s, t = plane0.ClosestParameter( point )
            if not cprc then  RhinoScriptingException.Raise "AddLeader failed.  points %A, text:%s, plane %A" points text plane
            points2d.Add( Rhino.Geometry.Point2d(s, t))
        let rc = State.Doc.Objects.AddLeader(text, plane0, points2d)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddLeader: Unable to add leader to document. points:'%A' text:'%A' plane:'%A'" points text plane0
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a linear dimension to the document.</summary>
    /// <param name="startPoint">(Point3d) The origin, or first point of the dimension</param>
    /// <param name="endPoint">(Point3d) The offset, or second point of the dimension</param>
    /// <param name="pointOnDimensionLine">(Point3d) A point that lies on the dimension line</param>
    /// <param name="plane">(Plane) Optional, The Plane on which the dimension will lie. The default is World XY Plane</param>
    /// <returns>(Guid) identifier of the new object.</returns>
    static member AddLinearDimension(   startPoint:Point3d,
                                        endPoint:Point3d,
                                        pointOnDimensionLine:Point3d, // TODO allow Point3d.Unset an then draw dim in XY plane
                                        [<OPT;DEF(Plane())>] plane:Plane ) : Guid =
        let mutable plane0 = if not plane.IsValid then Plane.WorldXY else Plane(plane) // copy // TODO or fail
        plane0.Origin <- startPoint // needed ?
        // Calculate 2d dimension points
        let _, s, t = plane0.ClosestParameter(startPoint)
        let start = Point2d(s, t)
        let _, s, t = plane0.ClosestParameter(endPoint)
        let ende = Point2d(s, t)
        let _, s, t = plane0.ClosestParameter(pointOnDimensionLine)
        let onpoint = Point2d(s, t)
        // Add the dimension
        let ldim = new LinearDimension(plane0, start, ende, onpoint)
        if isNull ldim then
            RhinoScriptingException.Raise "AddLinearDimension failed.  plane:'%A' startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A'" plane startPoint endPoint pointOnDimensionLine
        let rc = State.Doc.Objects.AddLinearDimension(ldim)
        if rc= Guid.Empty then
            RhinoScriptingException.Raise "AddLinearDimension: Unable to add dimension to document. plane:'%A' startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A'" plane startPoint endPoint pointOnDimensionLine
        State.Doc.Views.Redraw()
        rc


    /// <summary>Returns the current default dimension style.</summary>
    /// <returns>(string) Name of the current dimension style.</returns>
    static member CurrentDimStyle() : string = //GET
        State.Doc.DimStyles.Current.Name

    /// <summary>Changes the current default dimension style.
    ///  Raise a RhinoScriptingException if the style does not exist.</summary>
    /// <param name="dimStyleName">(string) Name of an existing dimension style to make current</param>
    /// <returns>(unit) void, nothing.</returns>
    static member CurrentDimStyle(dimStyleName:string) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyleName)
        if isNull ds  then  RhinoScriptingException.Raise "CurrentDimStyle setting failed. not found: '%s' . " dimStyleName
        if State.Doc.DimStyles.CurrentIndex <> ds.Index then // because SetCurrent returns false if it is already current
            if not <| State.Doc.DimStyles.SetCurrent(ds.Index, quiet=true) then
                RhinoScriptingException.Raise "CurrentDimStyle setting '%s' failed." dimStyleName



    /// <summary>Removes an existing dimension style from the document. The dimension style
    ///    to be removed cannot be referenced by any dimension objects.</summary>
    /// <param name="dimStyleName">(string) The name of an unreferenced dimension style</param>
    /// <returns>(unit) void, nothing (fails on error).</returns>
    static member DeleteDimStyle(dimStyleName:string) : unit =
        let ds = State.Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then
            RhinoScriptingException.Raise "DeleteDimStyle failed. dimStyleName:'%s'" dimStyleName
        let ok = State.Doc.DimStyles.Delete(ds.Index, quiet=true)
        if not ok then
            RhinoScriptingException.Raise "DeleteDimStyle failed. dimStyleName:' %s '" dimStyleName


    /// <summary>Returns the dimension style of a dimension object.</summary>
    /// <param name="objectId">(Guid) Identifier of the object</param>
    /// <returns>(string) The object's current dimension style name.</returns>
    static member DimensionStyle(objectId:Guid) : string = //GET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        //let annotation = annotationObject.Geometry :?> AnnotationBase
        let ds = annotationObject.AnnotationGeometry.ParentDimensionStyle
        ds.Name
        // this is how Rhino Python is doing it :
        // let ds:DocObjects.DimensionStyle = annotationObject?DimensionStyle //TODO verify Duck typing works ok

    /// <summary>Modifies the dimension style of a dimension object.</summary>
    /// <param name="objectId">(Guid) Identifier of the object</param>
    /// <param name="dimStyleName">(string) The name of an existing dimension style</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimensionStyle(objectId:Guid, dimStyleName:string) : unit = //SET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let ds =  State.Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then  RhinoScriptingException.Raise "DimensionStyle set failed.  objectId:'%s' dimStyleName:'%s'" (Pretty.str objectId) dimStyleName
        let mutable annotation = annotationObject.Geometry:?> AnnotationBase
        annotation.DimensionStyleId <- ds.Id
        annotationObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the dimension style of multiple dimension objects.</summary>
    /// <param name="objectIds">(Guid seq) Identifier of the objects</param>
    /// <param name="dimStyleName">(string) The name of multiple existing dimension style</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimensionStyle(objectIds:Guid seq, dimStyleName:string) : unit = //MULTISET
        let ds =  State.Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then  RhinoScriptingException.Raise "DimensionStyle set failed.  objectId:'%s' dimStyleName:'%s'" (Pretty.str objectIds) dimStyleName
        for objectId in objectIds do
            let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
            let mutable annotation = annotationObject.Geometry:?> AnnotationBase
            annotation.DimensionStyleId <- ds.Id
            annotationObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Returns the text displayed by a dimension object.</summary>
    /// <param name="objectId">(Guid) Identifier of the object</param>
    /// <returns>(string) The text displayed by a dimension object.</returns>
    static member DimensionText(objectId:Guid) : string =
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        annotationObject.DisplayText


    /// <summary>Returns the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined.</summary>
    /// <param name="objectId">(Guid) Identifier of the object</param>
    /// <returns>(string) The current usertext string.</returns>
    static member DimensionUserText(objectId:Guid) : string = //GET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> AnnotationBase
        geo.PlainText

    /// <summary>Modifies the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined.</summary>
    /// <param name="objectId">(Guid) Identifier of the object</param>
    /// <param name="usertext">(string) The new user text string value</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimensionUserText(objectId:Guid, usertext:string) : unit = //SET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> AnnotationBase
        geo.PlainText <- usertext
        annotationObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Modifies the user text string of multiple dimension objects. The user
    /// text is the string that gets printed when the dimension is defined.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of the objects</param>
    /// <param name="usertext">(string) The new user text string value</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimensionUserText(objectIds:Guid seq, usertext:string) : unit = //MULTISET
        for objectId in objectIds do
            let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
            let geo = annotationObject.Geometry :?> AnnotationBase
            geo.PlainText <- usertext
            annotationObject.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    /// <summary>Returns the value of a dimension object.</summary>
    /// <param name="objectId">(Guid) Identifier of the object</param>
    /// <returns>(float) numeric value of the dimension.</returns>
    static member DimensionValue(objectId:Guid) : float =
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> Dimension
        geo.NumericValue


    /// <summary>Returns the angle display precision of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(int) The current angle precision.</returns>
    static member DimStyleAnglePrecision(dimStyle:string) : int = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleAnglePrecision get failed. dimStyle:'%s'" dimStyle
        ds.AngleResolution

    /// <summary>Changes the angle display precision of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="precision">(int) The new angle precision value.</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleAnglePrecision(dimStyle:string, precision:int) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleAnglePrecision set failed. dimStyle:'%s' precision:%d" dimStyle precision
        if precision >= 0 then
            ds.AngleResolution <- precision
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then RhinoScriptingException.Raise "DimStyleAnglePrecision set failed. dimStyle:'%s' precision:%d" dimStyle precision
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "DimStyleAnglePrecision: precision must be nonnegative"

    /// <summary>Returns the arrow size of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(float) The current arrow size.</returns>
    static member DimStyleArrowSize(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleArrowSize get failed. dimStyle:'%s'" dimStyle
        ds.ArrowLength

    /// <summary>Changes the arrow size of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="size">(float) The new arrow size</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleArrowSize(dimStyle:string, size:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleArrowSize set failed. dimStyle:'%s' size:'%A'" dimStyle size
        if size > 0.0 then
            ds.ArrowLength <- size
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then RhinoScriptingException.Raise "DimStyleArrowSize set failed. dimStyle:'%s' size: %g" dimStyle size
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "DimStyleArrowSize set failed. dimStyle:'%s' size:%g" dimStyle size



    /// <summary>Returns the number of dimension styles in the document.</summary>
    /// <returns>(int) The number of dimension styles in the document.</returns>
    static member DimStyleCount() : int =
        State.Doc.DimStyles.Count


    /// <summary>Returns the extension line extension of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(float) The current extension line extension.</returns>
    static member DimStyleExtension(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleExtension get failed. dimStyle:'%s'" dimStyle
        ds.ExtensionLineExtension

    /// <summary>Changes the extension line extension of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="extension">(float) The new extension line extension</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleExtension(dimStyle:string, extension:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleExtension set failed. dimStyle:'%s' extension:'%A'" dimStyle extension
        if extension >= 0.0 then
            ds.ExtensionLineExtension <- extension
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "DimStyleExtension failed. dimStyle:'%s' extension:'%A'" dimStyle extension
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "DimStyleExtension set failed. dimStyle:'%s' extension:'%A'" dimStyle extension



    /// <summary>Returns the font used by a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(string) The current font.</returns>
    static member DimStyleFont(dimStyle:string) : string = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleFont get failed. dimStyle:'%s'" dimStyle
        ds.Font.FaceName


    /// <summary>Changes the font used by a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="font">(string) The new font face name</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleFont(dimStyle:string, font:string) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleFont set failed. dimStyle:'%s' font:'%A'" dimStyle font

        ds.Font <- DocObjects.Font(font) // TODO check if works OK !
        // let newIndex = State.Doc.Fonts.FindOrCreate(font, false, false) // deprecated ??
        // ds.Font <- State.Doc.Fonts.[newIndex]
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "DimStyleFont set failed. dimStyle:'%s' font:'%A'" dimStyle font
        State.Doc.Views.Redraw()


    /// <summary>Gets all Available Font Face Names.</summary>
    /// <returns>(string array) array of all available font names.</returns>
    static member DimStyleAvailableFonts() : array<string> = // not part of original rhinoscriptsyntax
        DocObjects.Font.AvailableFontFaceNames()



    /// <summary>Returns the leader arrow size of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(float) The current leader arrow size.</returns>
    static member DimStyleLeaderArrowSize(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleLeaderArrowSize get failed. dimStyle:'%s'" dimStyle
        ds.LeaderArrowLength

    /// <summary>Changes the leader arrow size of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="size">(float) The new leader arrow size</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleLeaderArrowSize(dimStyle:string, size:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleLeaderArrowSize set failed. dimStyle:'%s' size:'%A'" dimStyle size
        if size >= 0.0 then
            ds.LeaderArrowLength <- size
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "DimStyleLeaderArrowSize set failed. dimStyle:'%s' size:'%A'" dimStyle size
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "DimStyleLeaderArrowSize: size must be nonnegative"



    /// <summary>Returns the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(float) if factor is not defined, the current length factor.</returns>
    static member DimStyleLengthFactor(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleLengthFactor get failed. dimStyle:'%s'" dimStyle
        ds.LengthFactor

    /// <summary>Changes the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="factor">(float) The new length factor</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleLengthFactor(dimStyle:string, factor:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleLengthFactor set failed. dimStyle:'%s' factor:'%A'" dimStyle factor
        ds.LengthFactor <- factor
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "DimStyleLengthFactor set failed. dimStyle:'%s' factor:'%A'" dimStyle factor
        State.Doc.Views.Redraw()



    /// <summary>Returns the linear display precision of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(int) The current linear precision value.</returns>
    static member DimStyleLinearPrecision(dimStyle:string) : int = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleLinearPrecision get failed. dimStyle:'%s'" dimStyle
        ds.LengthResolution

    /// <summary>Changes the linear display precision of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="precision">(int) The new linear precision value</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleLinearPrecision(dimStyle:string, precision:int) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleLinearPrecision set failed. dimStyle:'%s' precision: %d" dimStyle precision
        if precision >= 0 then
            ds.LengthResolution <- precision
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "DimStyleLinearPrecision set failed. dimStyle:'%s' precision: %d" dimStyle precision
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "DimStyleLinearPrecision set failed. dimStyle:'%s' precision: %d" dimStyle precision



    /// <summary>Returns the names of all dimension styles in the document.</summary>
    /// <returns>(string ResizeArray) The names of all dimension styles in the document.</returns>
    static member DimStyleNames() : string ResizeArray =
        State.Doc.DimStyles |> RArr.mapSeq _.Name


    /// <summary>Returns the names, optionally sorted in ordinal order, like in RhinoPython.</summary>
    /// <param name="sort">(bool) True to sort the names; False to preserve document order.</param>
    /// <returns>(string ResizeArray) The names.</returns>
    static member DimStyleNames(sort:bool) : string ResizeArray =
        let names = RhinoScriptSyntax.DimStyleNames()
        if sort then names.Sort(StringComparer.Ordinal)
        names


    /// <summary>Returns the number display format of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(int) The current display format
    ///     ModelUnits       0  Decimal current model units
    ///     Millimeters      3  Decimal Millimeters
    ///     Centimeters      4  Decimal Centimeters
    ///     Meters           5  Decimal Meters
    ///     Kilometers       6  Decimal Kilometers
    ///     InchesDecimal    7  Decimal Inches
    ///     InchesFractional 1  Fractional Inches ( 1.75 inches displays as 1-3/4 )
    ///     FeetDecimal      8  Decimal Feet
    ///     FeetAndInches    2  Feet and Inches ( 14.75 inches displays as 1'-2-3/4" )
    ///     Miles            9  Decimal Miles.</returns>
    static member DimStyleNumberFormat(dimStyle:string) : int = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleNumberFormat get failed. dimStyle:'%s'" dimStyle
        int ds.DimensionLengthDisplay


    /// <summary>Changes the number display format of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="format">(int) The new number format
    ///     ModelUnits       0  Decimal current model units
    ///     Millimeters      3  Decimal Millimeters
    ///     Centimeters      4  Decimal Centimeters
    ///     Meters           5  Decimal Meters
    ///     Kilometers       6  Decimal Kilometers
    ///     InchesDecimal    7  Decimal Inches
    ///     InchesFractional 1  Fractional Inches ( 1.75 inches displays as 1-3/4 )
    ///     FeetDecimal      8  Decimal Feet
    ///     FeetAndInches    2  Feet and Inches ( 14.75 inches displays as 1'-2-3/4" )
    ///     Miles            9  Decimal Miles</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleNumberFormat(dimStyle:string, format:int) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleNumberFormat set failed. dimStyle:'%s' format:'%A'" dimStyle format
        if  format<0 || format>9 then  RhinoScriptingException.Raise "DimStyleNumberFormat set failed. dimStyle:'%s' format:'%A'" dimStyle format
        ds.DimensionLengthDisplay <- LanguagePrimitives.EnumOfValue format
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "DimStyleNumberFormat set failed. dimStyle:'%s' format:'%A'" dimStyle format
        State.Doc.Views.Redraw()


    /// <summary>Returns the extension line offset of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(float) The current extension line offset.</returns>
    static member DimStyleOffset(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleOffset get failed. dimStyle:'%s'" dimStyle
        ds.ExtensionLineOffset

    /// <summary>Changes the extension line offset of a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="offset">(float) The new extension line offset</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleOffset(dimStyle:string, offset:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleOffset set failed. dimStyle:'%s' offset:'%A'" dimStyle offset
        ds.ExtensionLineOffset <- offset
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "DimStyleOffset set failed. dimStyle:'%s' offset:'%A'" dimStyle offset
        State.Doc.Views.Redraw()



    /// <summary>Returns the prefix of a dimension style - the text to
    /// prefix to the dimension text.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimStyle</param>
    /// <returns>(string) The current prefix.</returns>
    static member DimStylePrefix(dimStyle:string) : string = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStylePrefix get failed. dimStyle:'%s'" dimStyle
        ds.Prefix

    /// <summary>Changes the prefix of a dimension style - the text to
    /// prefix to the dimension text.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimStyle</param>
    /// <param name="prefix">(string) The new prefix</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStylePrefix(dimStyle:string, prefix:string) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStylePrefix set failed. dimStyle:'%s' prefix:'%A'" dimStyle prefix
        ds.Prefix <- prefix
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "DimStylePrefix set failed. dimStyle:'%s' prefix:'%A'" dimStyle prefix
        State.Doc.Views.Redraw()


    /// <summary>Returns the scale of a dimension style .</summary>
    /// <param name="dimStyle">(string) The name of an existing dimStyle</param>
    /// <returns>(float) The current dimension scale.</returns>
    static member DimStyleScale(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleScale get failed. dimStyle:'%s'" dimStyle
        ds.DimensionScale

    /// <summary>Changes the scale of a dimension style .</summary>
    /// <param name="dimStyle">(string) The name of an existing dimStyle</param>
    /// <param name="scale">(float) The new scale</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleScale(dimStyle:string, scale:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleScale set failed. dimStyle:'%s' scale:'%A'" dimStyle scale
        ds.DimensionScale <- scale
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "DimStyleScale set failed. dimStyle:'%s' scale:'%A'" dimStyle scale
        State.Doc.Views.Redraw()


    /// <summary>Returns the suffix of a dimension style - the text to
    /// append to the dimension text.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimStyle</param>
    /// <returns>(string) The current suffix.</returns>
    static member DimStyleSuffix(dimStyle:string) : string = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleSuffix get failed. dimStyle:'%s'" dimStyle
        ds.Suffix

    /// <summary>Changes the suffix of a dimension style - the text to
    /// append to the dimension text.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimStyle</param>
    /// <param name="suffix">(string) The new suffix</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleSuffix(dimStyle:string, suffix:string) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then
            RhinoScriptingException.Raise "DimStyleSuffix set failed. dimStyle:'%s' suffix:'%A'" dimStyle suffix
        ds.Suffix <- suffix
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "DimStyleSuffix set failed. dimStyle:'%s' suffix:'%A'" dimStyle suffix
        State.Doc.Views.Redraw()


    /// <summary>Returns the text alignment mode of a dimension style.
    /// Rhino 6 replaced the single Rhino 5 text alignment setting with a text location
    /// (above, in, or below the dimension line) and a text orientation
    /// (in the dimension plane, or horizontal to the view).
    /// This function maps them back onto the Rhino 5 values.
    /// It reads the settings for linear and angular dimensions.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(int) The current text alignment
    ///    1 = Horizontal to view
    ///    2 = In the dimension plane, above or below the dimension line
    ///    3 = In the dimension plane, in the dimension line
    ///    0 is never returned.
    /// </returns>
    static member DimStyleTextAlignment(dimStyle:string) : int = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then
            RhinoScriptingException.Raise "DimStyleTextAlignment get failed. dimStyle:'%s'" dimStyle
        // https://github.com/mcneel/rhinoscriptsyntax/commit/b2bf485894 (RH-44732)
        if   ds.DimTextOrientation = DocObjects.TextOrientation.InView then 1
        elif ds.DimTextLocation    = DocObjects.DimensionStyle.TextLocation.InDimLine then 3
        else 2

    /// <summary>Changes the text alignment mode of a dimension style.
    /// Rhino 6 replaced the single Rhino 5 text alignment setting with a text location
    /// (above, in, or below the dimension line) and a text orientation
    /// (in the dimension plane, or horizontal to the view),
    /// stored separately for linear and angular dimensions and for radial dimensions.
    /// This function sets both, for linear, angular and radial dimensions.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="alignment">(int) The new text alignment
    ///    0 = Same as 2
    ///    1 = Orientation horizontal to view, location is left unchanged
    ///    2 = Orientation in the dimension plane, location above the dimension line
    ///    3 = Orientation in the dimension plane, location in the dimension line
    /// </param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleTextAlignment(dimStyle:string, alignment:int) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then
            RhinoScriptingException.Raise "DimStyleTextAlignment not found. dimStyle:'%s' alignment:'%d'" dimStyle alignment
        elif alignment<0 || alignment>3 then
            RhinoScriptingException.Raise "DimStyleTextAlignment invalid alignment. dimStyle:'%s' alignment:'%d' is not between 0 and 3" dimStyle alignment

        // https://github.com/mcneel/rhinoscriptsyntax/commit/b2bf485894 (RH-44732)
        // Rhino 5 had one text alignment for every dimension type. Rhino 6 keeps
        // the linear/angular and the radial settings apart, so write both pairs.
        if alignment = 1 then
            ds.DimTextOrientation       <- DocObjects.TextOrientation.InView
            ds.DimRadialTextOrientation <- DocObjects.TextOrientation.InView
        else
            let location =
                if alignment = 3 then DocObjects.DimensionStyle.TextLocation.InDimLine
                else                  DocObjects.DimensionStyle.TextLocation.AboveDimLine
            ds.DimTextOrientation       <- DocObjects.TextOrientation.InPlane
            ds.DimRadialTextOrientation <- DocObjects.TextOrientation.InPlane
            ds.DimTextLocation          <- location
            ds.DimRadialTextLocation    <- location

        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "DimStyleTextAlignment set failed. dimStyle:'%s' alignment:'%A'" dimStyle alignment
        State.Doc.Views.Redraw()


    /// <summary>Returns the text gap used by a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(float) The current text gap.</returns>
    static member DimStyleTextGap(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleTextGap get failed. dimStyle:'%s'" dimStyle
        ds.TextGap

    /// <summary>Changes the text gap used by a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="gap">(float) The new text gap</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleTextGap(dimStyle:string, gap:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleTextGap set failed. dimStyle:'%s' gap:'%A'" dimStyle gap
        if gap >= 0.0 then
            ds.TextGap <- gap
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "DimStyleTextGap set failed. dimStyle:'%s' gap:'%A'" dimStyle gap
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "DimStyleTextGap set failed. dimStyle:'%s' gap:'%A'" dimStyle gap



    /// <summary>Returns the text height used by a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(float) The current text height.</returns>
    static member DimStyleTextHeight(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleTextHeight get failed. dimStyle:'%s'" dimStyle
        ds.TextHeight

    /// <summary>Changes the text height used by a dimension style.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <param name="height">(float) The new text height</param>
    /// <returns>(unit) void, nothing.</returns>
    static member DimStyleTextHeight(dimStyle:string, height:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "DimStyleTextHeight set failed. dimStyle:'%s' height:'%A'" dimStyle height
        if height>0.0 then
            ds.TextHeight <- height
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "DimStyleTextHeight set failed. dimStyle:'%s' height:'%A'" dimStyle height
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "DimStyleTextHeight set failed. dimStyle:'%s' height:'%A'" dimStyle height


    /// <summary>Checks if  an object is an aligned dimension object. Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsAlignedDimension(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceGeometry objectId
        |> Option.exists (fun geometry -> RhinoScriptSyntax.IsAlignedDimension(geometry))

    /// <summary>Checks if  an object is an aligned dimension object. Returns false for any other Rhino object or if the geometry is null.</summary>
    /// <param name="geometryObject">(GeometryBase) The geometry to test</param>
    /// <returns>(bool) True or False.</returns>
    static member IsAlignedDimension(geometryObject:GeometryBase) : bool =
        match geometryObject with
        | :? LinearDimension as g -> g.Aligned
        | _ -> false


    /// <summary>Checks if  an object is an angular dimension object. Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsAngularDimension(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceGeometry objectId
        |> Option.exists (fun geometry -> RhinoScriptSyntax.IsAngularDimension(geometry))

    /// <summary>Checks if  an object is an angular dimension object. Returns false for any other Rhino object or if the geometry is null.</summary>
    /// <param name="geometryObject">(GeometryBase) The geometry to test</param>
    /// <returns>(bool) True or False.</returns>
    static member IsAngularDimension(geometryObject:GeometryBase) : bool =
        match geometryObject with
        | :? AngularDimension -> true
        | _ -> false


    /// <summary>Checks if  an object is a diameter dimension object. Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsDiameterDimension(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceGeometry objectId
        |> Option.exists (fun geometry -> RhinoScriptSyntax.IsDiameterDimension(geometry))

    /// <summary>Checks if  an object is a diameter dimension object. Returns false for any other Rhino object or if the geometry is null.</summary>
    /// <param name="geometryObject">(GeometryBase) The geometry to test</param>
    /// <returns>(bool) True or False.</returns>
    static member IsDiameterDimension(geometryObject:GeometryBase) : bool =
        match geometryObject with
        | :? RadialDimension as g -> g.IsDiameterDimension
        | _ -> false


    /// <summary>Checks if  an object is a dimension object. Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsDimension(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceGeometry objectId
        |> Option.exists (fun geometry -> RhinoScriptSyntax.IsDimension(geometry))

    /// <summary>Checks if  an object is a dimension object. Returns false for any other Rhino object or if the geometry is null.</summary>
    /// <param name="geometryObject">(GeometryBase) The geometry to test</param>
    /// <returns>(bool) True or False.</returns>
    static member IsDimension(geometryObject:GeometryBase) : bool =
        match geometryObject with
        | :? AnnotationBase -> true
        | _ -> false


    /// <summary>Checks if  the existence of a dimension style in the document. Returns false for any other Rhino object.</summary>
    /// <param name="dimStyle">(string) The name of a dimStyle to test for</param>
    /// <returns>(bool) True or False.</returns>
    static member IsDimStyle(dimStyle:string) : bool =
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        notNull ds


    /// <summary>Checks if  that an existing dimension style is from a reference file. Returns false for any other Rhino object.</summary>
    /// <param name="dimStyle">(string) The name of an existing dimension style</param>
    /// <returns>(bool) True or False.</returns>
    static member IsDimStyleReference(dimStyle:string) : bool =
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then false
        else ds.IsReference


    /// <summary>Checks if  an object is a dimension leader object. Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsLeader(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceGeometry objectId
        |> Option.exists (fun geometry -> RhinoScriptSyntax.IsLeader(geometry))

    /// <summary>Checks if  an object is a dimension leader object. Returns false for any other Rhino object or if the geometry is null.</summary>
    /// <param name="geometryObject">(GeometryBase) The geometry to test</param>
    /// <returns>(bool) True or False.</returns>
    static member IsLeader(geometryObject:GeometryBase) : bool =
            match geometryObject with
            | :? Leader -> true
            | _ -> false


    /// <summary>Checks if  an object is a linear dimension object. Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsLinearDimension(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceGeometry objectId
        |> Option.exists (fun geometry -> RhinoScriptSyntax.IsLinearDimension(geometry))

    /// <summary>Checks if  an object is a linear dimension object. Returns false for any other Rhino object or if the geometry is null.</summary>
    /// <param name="geometryObject">(GeometryBase) The geometry to test</param>
    /// <returns>(bool) True or False.</returns>
    static member IsLinearDimension(geometryObject:GeometryBase) : bool =
            match geometryObject with
            | :? LinearDimension -> true
            | _ -> false


    /// <summary>Checks if  an object is an ordinate dimension object. Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsOrdinateDimension(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceGeometry objectId
        |> Option.exists (fun geometry -> RhinoScriptSyntax.IsOrdinateDimension(geometry))

    /// <summary>Checks if  an object is an ordinate dimension object. Returns false for any other Rhino object or if the geometry is null.</summary>
    /// <param name="geometryObject">(GeometryBase) The geometry to test</param>
    /// <returns>(bool) True or False.</returns>
    static member IsOrdinateDimension(geometryObject:GeometryBase) : bool =
            match geometryObject with
            | :? OrdinateDimension -> true
            | _ -> false


    /// <summary>Checks if  an object is a radial dimension object. Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsRadialDimension(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceGeometry objectId
        |> Option.exists (fun geometry -> RhinoScriptSyntax.IsRadialDimension(geometry))

    /// <summary>Checks if  an object is a radial dimension object. Returns false for any other Rhino object or if the geometry is null.</summary>
    /// <param name="geometryObject">(GeometryBase) The geometry to test</param>
    /// <returns>(bool) True or False.</returns>
    static member IsRadialDimension(geometryObject:GeometryBase) : bool =
            match geometryObject with
            | :? RadialDimension -> true
            | _ -> false


    /// <summary>Returns the text string of a dimension leader object.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(string) The current text string.</returns>
    static member LeaderText(objectId:Guid) : string = //GET
            match RhinoScriptSyntax.CoerceGeometry objectId with
            | :? Leader ->
                let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
                annotationObject.DisplayText
            | _ -> RhinoScriptingException.Raise "LeaderText get failed.  objectId:'%s'" (Pretty.str objectId)

    /// <summary>Modifies the text string of a dimension leader object.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <param name="text">(string) The new text string</param>
    /// <returns>(unit) void, nothing.</returns>
    static member LeaderText(objectId:Guid, text:string) : unit = //SET
            match RhinoScriptSyntax.CoerceGeometry objectId with
            | :? Leader as g ->
                let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
                g.PlainText <- text               // TODO or use rich text?
                if not <| State.Doc.Objects.Replace(objectId,g) then RhinoScriptingException.Raise "LeaderText: Objects.Replace(objectId,g) get failed. objectId:'%s'" (Pretty.str objectId)
                annotationObject.CommitChanges() |> ignore<bool>
                State.Doc.Views.Redraw()
            | _ -> RhinoScriptingException.Raise "LeaderText set failed for  %s"  (Pretty.str objectId)

    /// <summary>Modifies the text string of multiple dimension leader objects.</summary>
    /// <param name="objectIds">(Guid seq) The objects's identifiers</param>
    /// <param name="text">(string) The new text string</param>
    /// <returns>(unit) void, nothing.</returns>
    static member LeaderText(objectIds:Guid seq, text:string) : unit = //MULTISET
        for objectId in objectIds do
            RhinoScriptSyntax.LeaderText(objectId,text)

    /// <summary>Renames an existing dimension style.</summary>
    /// <param name="oldStyle">(string) The name of an existing dimension style</param>
    /// <param name="newStyle">(string) The new dimension style name</param>
    /// <returns>(unit) void, nothing.</returns>
    static member RenameDimStyle(oldStyle:string, newStyle:string) : unit =
        let mutable ds = State.Doc.DimStyles.FindName(oldStyle)
        if isNull ds then  RhinoScriptingException.Raise "RenameDimStyle failed.  oldStyle:'%s' newStyle:'%s'" oldStyle newStyle
        ds.Name <- newStyle
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "RenameDimStyle failed.  oldStyle:'%s' newStyle:'%s'" oldStyle newStyle
