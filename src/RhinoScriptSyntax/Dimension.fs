namespace Rhino.Scripting.Modules

open FsEx
open System
open Rhino
open Rhino.Geometry

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting Namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsDimension =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with


    [<Extension>]
    ///<summary>Adds an aligned dimension object to the document. An aligned dimension
    ///    is a linear dimension lined up with two points</summary>
    ///<param name="startPoint">(Point3d) First point of dimension</param>
    ///<param name="endPoint">(Point3d) Second point of dimension</param>
    ///<param name="pointOnDimensionLine">(Point3d) Location point of dimension line</param>
    ///<param name="style">(string) Optional, Default Value: <c>""</c>
    ///    Name of dimension style</param>
    ///<returns>(Guid) identifier of new dimension</returns>
    static member AddAlignedDimension(startPoint:Point3d, endPoint:Point3d, pointOnDimensionLine:Point3d, [<OPT;DEF("")>]style:string) : Guid =
        let mutable start = startPoint
        let mutable ende = endPoint
        let mutable onpoint = pointOnDimensionLine
        let mutable plane = Geometry.Plane(start, ende, onpoint)
        let mutable success, s, t = plane.ClosestParameter(start)
        let start2 = Point2d(s, t)
        let success, s, t = plane.ClosestParameter(ende)
        let ende2 = Point2d(s, t)
        let success, s, t = plane.ClosestParameter(onpoint)
        let onpoint2 = Point2d(s, t)
        let ldim = new LinearDimension(plane, start2, ende2, onpoint2)
        if isNull ldim then  failwithf "addAlignedDimension failed.  startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A' style:'%A'" startPoint endPoint pointOnDimensionLine style
        ldim.Aligned <- true
        if style <> "" then
            let ds = Doc.DimStyles.FindName(style)
            if isNull ds then  failwithf "addAlignedDimension, style not found, failed.  startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A' style:'%s'" startPoint endPoint pointOnDimensionLine style
            ldim.DimensionStyleId <- ds.Id
        let rc = Doc.Objects.AddLinearDimension(ldim)
        if rc = Guid.Empty then  failwithf "addAlignedDimension Unable to add dimension to document.  startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A' style:'%A'" startPoint endPoint pointOnDimensionLine style
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a new dimension style to the document. The new dimension style will
    ///    be initialized with the current default dimension style properties</summary>
    ///<param name="dimStyleName">(string) Name of the new dimension style</param>
    ///<returns>(unit) void, nothing</returns>
    static member AddDimStyle(dimStyleName:string) : unit =
        let index = Doc.DimStyles.Add(dimStyleName)
        if index<0 then  failwithf "addDimStyle failed.  dimStyleName:'%A'" dimStyleName




    [<Extension>]
    ///<summary>Adds a leader to the document. Leader objects are planar.
    ///    The 3D points passed will define the plane if no Plane given</summary>
    ///<param name="points">(Point3d seq) List of (at least 2) 3D points</param>
    ///<param name="text">(string) Leader's text</param>
    ///<param name="plane">(Geometry.Plane) Optional, Default Value: <c>defined by points arg</c>
    ///    If points will be projected to this plane</param>
    ///<returns>(Guid) identifier of the new leader</returns>
    static member AddLeader(points:Point3d seq, text:string, [<OPT;DEF(Plane())>]plane:Plane) : Guid =
        let points2d = ResizeArray()
        let plane0 =
            if plane.IsValid then plane
            else
                let ps= ResizeArray(points)
                let o = ps.GetItem(-2)
                let mutable x = ps.GetItem(-1)-o
                let mutable y = ps.[0]-ps.[1]
                if y.Z < 0.0 then y <- -y
                if y.Y < 0.0 then y <- -y
                if x.X < 0.0 then x <- -x
                Plane(o, x, y)
                |> fun pl->
                    if not pl.IsValid then failwithf "AddLeader failed to find plane.  points %A, text:%s" points text
                    pl

        for point in points do
            let cprc, s, t = plane0.ClosestParameter( point )
            if not cprc then  failwithf "AddLeader failed.  points %A, text:%s, plane %A" points text plane
            points2d.Add( Rhino.Geometry.Point2d(s, t) )
        Doc.Objects.AddLeader(text, plane0, points2d)


    [<Extension>]
    ///<summary>Adds a linear dimension to the document</summary>
    ///<param name="startPoint">(Point3d) The origin, or first point of the dimension</param>
    ///<param name="endPoint">(Point3d) The offset, or second point of the dimension</param>
    ///<param name="pointOnDimensionLine">(Point3d) A point that lies on the dimension line</param>
    ///<param name="plane">(Plane) Optional, The plane on which the dimension will lie. The default is World XY Plane</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddLinearDimension(   startPoint:Point3d,
                                        endPoint:Point3d,
                                        pointOnDimensionLine:Point3d,
                                        [<OPT;DEF(Plane())>] plane:Plane ) : Guid =
        let mutable plane0 = if not plane.IsValid then Plane.WorldXY else Plane(plane) // copy
        plane0.Origin <- startPoint // needed ?
        // Calculate 2d dimension points
        let success, s, t = plane0.ClosestParameter(startPoint)
        let start = Point2d(s, t)
        let success, s, t = plane0.ClosestParameter(endPoint)
        let ende = Point2d(s, t)
        let success, s, t = plane0.ClosestParameter(pointOnDimensionLine)
        let onpoint = Point2d(s, t)
        // Add the dimension
        let ldim = new LinearDimension(plane0, start, ende, onpoint)
        if isNull ldim then
            failwithf "addLinearDimension failed.  plane:'%A' startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A'" plane startPoint endPoint pointOnDimensionLine
        let rc = Doc.Objects.AddLinearDimension(ldim)
        if rc= Guid.Empty then
            failwithf "addLinearDimension Unable to add dimension to document.  plane:'%A' startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A'" plane startPoint endPoint pointOnDimensionLine
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Returns the current default dimension style</summary>
    ///<returns>(string) Name of the current dimension style</returns>
    static member CurrentDimStyle() : string = //GET
        Doc.DimStyles.Current.Name

    [<Extension>]
    ///<summary>Changes the current default dimension style</summary>
    ///<param name="dimStyleName">(string) Name of an existing dimension style to make current</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentDimStyle(dimStyleName:string) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyleName)
        if notNull ds  then  failwithf "setCurrentDimStyle failed.  dimStyleName:'%A'" dimStyleName
        if not <| Doc.DimStyles.SetCurrent(ds.Index, false) then
            failwithf "setCurrentDimStyle failed.  dimStyleName:'%A'" dimStyleName



    [<Extension>]
    ///<summary>Removes an existing dimension style from the document. The dimension style
    ///    to be removed cannot be referenced by any dimension objects</summary>
    ///<param name="dimStyleName">(string) The name of an unreferenced dimension style</param>
    ///<returns>(unit) void, nothing (fails on error)</returns>
    static member DeleteDimStyle(dimStyleName:string) : unit =
        let ds = Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then
            failwithf "deleteDimStyle failed. dimStyleName:'%A'" dimStyleName
        let ok = Doc.DimStyles.Delete(ds.Index, true)
        if not ok then
            failwithf "deleteDimStyle failed. dimStyleName:' %A '" dimStyleName


    [<Extension>]
    ///<summary>Returns the dimension style of a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The object's current dimension style name</returns>
    static member DimensionStyle(objectId:Guid) : string = //GET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let annotation = annotationObject.Geometry:?> AnnotationBase
        let ds = annotationObject.AnnotationGeometry.ParentDimensionStyle
        ds.Name
        // this is how Rhino Python is doing it :
        // let ds:DocObjects.DimensionStyle = annotationObject?DimensionStyle //TODO verify Duck typing works ok

    [<Extension>]
    ///<summary>Modifies the dimension style of a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="dimStyleName">(string) The name of an existing dimension style</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimensionStyle(objectId:Guid, dimStyleName:string) : unit = //SET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let ds =  Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then  failwithf "set DimensionStyle failed.  objectId:'%A' dimStyleName:'%A'" objectId dimStyleName
        let mutable annotation = annotationObject.Geometry:?> AnnotationBase
        annotation.DimensionStyleId <- ds.Id
        annotationObject.CommitChanges() |>ignore // TODO verify this works ok
        Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Modifies the dimension style of multiple dimension objects</summary>
    ///<param name="objectsIds">(Guid seq) Identifier of the objects</param>
    ///<param name="dimStyleName">(string) The name of multiple existing dimension style</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimensionStyle(objectIds:Guid seq, dimStyleName:string) : unit = //MULTISET
        let ds =  Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then  failwithf "set DimensionStyle failed.  objectId:'%A' dimStyleName:'%A'" objectIds dimStyleName
        for objectId in objectIds do         
            let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
            let mutable annotation = annotationObject.Geometry:?> AnnotationBase
            annotation.DimensionStyleId <- ds.Id
            annotationObject.CommitChanges() |>ignore // TODO verify this works ok
        Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Returns the text displayed by a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) the text displayed by a dimension object</returns>
    static member DimensionText(objectId:Guid) : string =
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        annotationObject.DisplayText


    [<Extension>]
    ///<summary>Returns the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The current usertext string</returns>
    static member DimensionUserText(objectId:Guid) : string = //GET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> AnnotationBase
        geo.PlainText

    [<Extension>]
    ///<summary>Modifies the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="usertext">(string) The new user text string value</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimensionUserText(objectId:Guid, usertext:string) : unit = //SET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> AnnotationBase
        geo.PlainText <- usertext
        annotationObject.CommitChanges() |>ignore
        Doc.Views.Redraw()
    
    [<Extension>]
    ///<summary>Modifies the user text string of multiple dimension objects. The user
    /// text is the string that gets printed when the dimension is defined</summary>
    ///<param name="objectsIds">(Guid seq) Identifiers of the objects</param>
    ///<param name="usertext">(string) The new user text string value</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimensionUserText(objectIds:Guid seq, usertext:string) : unit = //MULTISET
        for objectId in objectIds do 
            let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
            let geo = annotationObject.Geometry :?> AnnotationBase
            geo.PlainText <- usertext
            annotationObject.CommitChanges() |>ignore
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the value of a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) numeric value of the dimension</returns>
    static member DimensionValue(objectId:Guid) : float =
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> Dimension
        geo.NumericValue


    [<Extension>]
    ///<summary>Returns the angle display precision of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current angle precision</returns>
    static member DimStyleAnglePrecision(dimStyle:string) : int = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleAnglePrecision failed.  dimStyle:'%A'" dimStyle
        ds.AngleResolution

    [<Extension>]
    ///<summary>Changes the angle display precision of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="precision">(int) The new angle precision value.</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleAnglePrecision(dimStyle:string, precision:int) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleAnglePrecision failed.  dimStyle:'%A' precision:'%A'" dimStyle precision
        let rc = ds.AngleResolution
        if precision >= 0 then
            ds.AngleResolution <- precision
            if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleAnglePrecision failed.  dimStyle:'%A' precision:'%A'" dimStyle precision
            Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the arrow size of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current arrow size</returns>
    static member DimStyleArrowSize(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleArrowSize failed.  dimStyle:'%A'" dimStyle
        ds.ArrowLength

    [<Extension>]
    ///<summary>Changes the arrow size of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="size">(float) The new arrow size</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleArrowSize(dimStyle:string, size:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleArrowSize failed.  dimStyle:'%A' size:'%A'" dimStyle size
        let rc = ds.ArrowLength
        if size > 0.0 then
            ds.ArrowLength <- size
            if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleArrowSize failed.  dimStyle:'%A' size:'%A'" dimStyle size
            Doc.Views.Redraw()
        else
            failwithf "set DimStyleArrowSize failed.  dimStyle:'%A' size:'%A'" dimStyle size



    [<Extension>]
    ///<summary>Returns the number of dimension styles in the document</summary>
    ///<returns>(int) the number of dimension styles in the document</returns>
    static member DimStyleCount() : int =
        Doc.DimStyles.Count


    [<Extension>]
    ///<summary>Returns the extension line extension of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current extension line extension</returns>
    static member DimStyleExtension(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleExtension failed.  dimStyle:'%A'" dimStyle
        ds.ExtensionLineExtension

    [<Extension>]
    ///<summary>Changes the extension line extension of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="extension">(float) The new extension line extension</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleExtension(dimStyle:string, extension:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleExtension failed.  dimStyle:'%A' extension:'%A'" dimStyle extension
        let rc = ds.ExtensionLineExtension
        if extension > 0.0 then
            ds.ExtensionLineExtension <- extension
            if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "DimStyleExtension failed.  dimStyle:'%A' extension:'%A'" dimStyle extension
            Doc.Views.Redraw()
        else
            failwithf "set DimStyleExtension failed.  dimStyle:'%A' extension:'%A'" dimStyle extension



    [<Extension>]
    ///<summary>Returns the font used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(string) The current font</returns>
    static member DimStyleFont(dimStyle:string) : string = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleFont failed.  dimStyle:'%A'" dimStyle
        ds.Font.FaceName


    [<Extension>]
    ///<summary>Changes the font used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="font">(string) The new font face name</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleFont(dimStyle:string, font:string) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleFont failed.  dimStyle:'%A' font:'%A'" dimStyle font

        ds.Font <- DocObjects.Font(font) // TODO check if works OK !
        // let newindex = Doc.Fonts.FindOrCreate(font, false, false) // deprecated ??
        // ds.Font <- Doc.Fonts.[newindex]
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then  failwithf "set DimStyleFont failed.  dimStyle:'%A' font:'%A'" dimStyle font
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Gets all Available Font Face Names</summary>
    ///<returns>(string array) array of all available font names</returns>
    static member DimStyleAvailableFonts() : array<string> = // not part of original rhinoscriptsyntax
        DocObjects.Font.AvailableFontFaceNames()



    [<Extension>]
    ///<summary>Returns the leader arrow size of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current leader arrow size</returns>
    static member DimStyleLeaderArrowSize(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleLeaderArrowSize failed.  dimStyle:'%A'" dimStyle
        ds.LeaderArrowLength

    [<Extension>]
    ///<summary>Changes the leader arrow size of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="size">(float) The new leader arrow size</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleLeaderArrowSize(dimStyle:string, size:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleLeaderArrowSize failed.  dimStyle:'%A' size:'%A'" dimStyle size
        if size > 0.0 then
            ds.LeaderArrowLength <- size
            if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleLeaderArrowSize failed.  dimStyle:'%A' size:'%A'" dimStyle size
            Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) if factor is not defined, the current length factor</returns>
    static member DimStyleLengthFactor(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleLengthFactor failed.  dimStyle:'%A'" dimStyle
        ds.LengthFactor

    [<Extension>]
    ///<summary>Changes the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="factor">(float) The new length factor</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleLengthFactor(dimStyle:string, factor:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleLengthFactor failed.  dimStyle:'%A' factor:'%A'" dimStyle factor
        ds.LengthFactor <- factor
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleLengthFactor failed.  dimStyle:'%A' factor:'%A'" dimStyle factor
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the linear display precision of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current linear precision value</returns>
    static member DimStyleLinearPrecision(dimStyle:string) : int = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleLinearPrecision failed.  dimStyle:'%A'" dimStyle
        ds.LengthResolution

    [<Extension>]
    ///<summary>Changes the linear display precision of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="precision">(int) The new linear precision value</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleLinearPrecision(dimStyle:string, precision:int) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleLinearPrecision failed.  dimStyle:'%A' precision:'%A'" dimStyle precision
        if precision >= 0 then
            ds.LengthResolution <- precision
            if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleLinearPrecision failed.  dimStyle:'%A' precision:'%A'" dimStyle precision
            Doc.Views.Redraw()
        else
            failwithf "set DimStyleLinearPrecision failed.  dimStyle:'%A' precision:'%A'" dimStyle precision



    [<Extension>]
    ///<summary>Returns the names of all dimension styles in the document</summary>
    ///<returns>(string ResizeArray) the names of all dimension styles in the document</returns>
    static member DimStyleNames() : string ResizeArray =
        resizeArray {for  ds in Doc.DimStyles -> ds.Name }


    [<Extension>]
    ///<summary>Returns the number display format of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current display format
    ///     ModelUnits       0  Decimal current model units
    ///     Millmeters       3  Decimal Millimeters
    ///     Centimeters      4  Decimal Centimeters
    ///     Meters           5  Decimal Meters
    ///     Kilometers       6  Decimal Kilometers
    ///     InchesDecimal    7  Decimal Inches
    ///     InchesFractional 1  Fractional Inches ( 1.75 inches displays as 1-3/4 )
    ///     FeetDecimal      8  Decimal Feet
    ///     FeetAndInches    2  Feet and Inches ( 14.75 inches displays as 1'-2-3/4" )
    ///     Miles            9  Decimal Miles</returns>
    static member DimStyleNumberFormat(dimStyle:string) : int = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleNumberFormat failed.  dimStyle:'%A'" dimStyle
        int ds.DimensionLengthDisplay


    [<Extension>]
    ///<summary>Changes the number display format of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="format">(int) The new number format
    ///     ModelUnits       0  Decimal current model units
    ///     Millmeters       3  Decimal Millimeters
    ///     Centimeters      4  Decimal Centimeters
    ///     Meters           5  Decimal Meters
    ///     Kilometers       6  Decimal Kilometers
    ///     InchesDecimal    7  Decimal Inches
    ///     InchesFractional 1  Fractional Inches ( 1.75 inches displays as 1-3/4 )
    ///     FeetDecimal      8  Decimal Feet
    ///     FeetAndInches    2  Feet and Inches ( 14.75 inches displays as 1'-2-3/4" )
    ///     Miles            9  Decimal Miles</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleNumberFormat(dimStyle:string, format:int) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleNumberFormat failed.  dimStyle:'%A' format:'%A'" dimStyle format
        if  format<0 || format>9 then  failwithf "set DimStyleNumberFormat failed.  dimStyle:'%A' format:'%A'" dimStyle format
        ds.DimensionLengthDisplay <- LanguagePrimitives.EnumOfValue format
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleNumberFormat failed.  dimStyle:'%A' format:'%A'" dimStyle format
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the extension line offset of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current extension line offset</returns>
    static member DimStyleOffset(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleOffset failed.  dimStyle:'%A'" dimStyle
        ds.ExtensionLineOffset

    [<Extension>]
    ///<summary>Changes the extension line offset of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="offset">(float) The new extension line offset</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleOffset(dimStyle:string, offset:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleOffset failed.  dimStyle:'%A' offset:'%A'" dimStyle offset
        ds.ExtensionLineOffset <- offset
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleOffset failed.  dimStyle:'%A' offset:'%A'" dimStyle offset
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the prefix of a dimension style - the text to
    /// prefix to the dimension text</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<returns>(string) The current prefix</returns>
    static member DimStylePrefix(dimStyle:string) : string = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStylePrefix failed.  dimStyle:'%A'" dimStyle
        ds.Prefix

    [<Extension>]
    ///<summary>Changes the prefix of a dimension style - the text to
    /// prefix to the dimension text</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<param name="prefix">(string) The new prefix</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStylePrefix(dimStyle:string, prefix:string) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStylePrefix failed.  dimStyle:'%A' prefix:'%A'" dimStyle prefix
        ds.Prefix <- prefix
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStylePrefix failed.  dimStyle:'%A' prefix:'%A'" dimStyle prefix
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the suffix of a dimension style - the text to
    /// append to the dimension text</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<returns>(string) The current suffix</returns>
    static member DimStyleSuffix(dimStyle:string) : string = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleSuffix failed.  dimStyle:'%A'" dimStyle
        ds.Suffix

    [<Extension>]
    ///<summary>Changes the suffix of a dimension style - the text to
    /// append to the dimension text</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<param name="suffix">(string) The new suffix</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleSuffix(dimStyle:string, suffix:string) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleSuffix failed.  dimStyle:'%A' suffix:'%A'" dimStyle suffix
        ds.Suffix <- suffix
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleSuffix failed.  dimStyle:'%A' suffix:'%A'" dimStyle suffix
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the text alignment mode of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current text alignment
    ///     Top                   0   Attach to top of an 'I' on the first line. (Independent of glyphs being displayed.)
    ///     MiddleOfTop           1   Attach to middle of an 'I' on the first line. (Independent of glyphs being displayed.)
    ///     BottomOfTop           2   Attach to baseline of first line. (Independent of glyphs being displayed.)
    ///     Middle                3   Attach to middle of text vertical advance. (Independent of glyphs being displayed.)
    ///     MiddleOfBottom        4   Attach to middle of an 'I' on the last line. (Independent of glyphs being displayed.)
    ///     Bottom                5   Attach to the basline of the last line. (Independent of glyphs being displayed.)
    ///     BottomOfBoundingBox   6   Attach to the bottom of the boudning box of the visible glyphs.</returns>
    static member DimStyleTextAlignment(dimStyle:string) : int = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleTextAlignment failed.  dimStyle:'%A'" dimStyle
        int ds.TextVerticalAlignment

    [<Extension>]
    ///<summary>Changes the text alignment mode of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="alignment">(int) The new text alignment
    ///     Top                   0   Attach to top of an 'I' on the first line. (Independent of glyphs being displayed.)
    ///     MiddleOfTop           1   Attach to middle of an 'I' on the first line. (Independent of glyphs being displayed.)
    ///     BottomOfTop           2   Attach to baseline of first line. (Independent of glyphs being displayed.)
    ///     Middle                3   Attach to middle of text vertical advance. (Independent of glyphs being displayed.)
    ///     MiddleOfBottom        4   Attach to middle of an 'I' on the last line. (Independent of glyphs being displayed.)
    ///     Bottom                5   Attach to the basline of the last line. (Independent of glyphs being displayed.)
    ///     BottomOfBoundingBox   6   Attach to the bottom of the boudning box of the visible glyphs.</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleTextAlignment(dimStyle:string, alignment:int) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "DimStyleTextAlignment not found.  dimStyle:'%A' alignment:'%A'" dimStyle alignment
        elif alignment<0 || alignment>6 then  failwithf "set DimStyleTextAlignment failed.  dimStyle:'%A' alignment:'%A'" dimStyle alignment
        ds.TextVerticalAlignment <- LanguagePrimitives.EnumOfValue (byte alignment)
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleTextAlignment failed.  dimStyle:'%A' alignment:'%A'" dimStyle alignment
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the text gap used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current text gap</returns>
    static member DimStyleTextGap(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleTextGap failed.  dimStyle:'%A'" dimStyle
        ds.TextGap

    [<Extension>]
    ///<summary>Changes the text gap used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="gap">(float) The new text gap</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleTextGap(dimStyle:string, gap:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleTextGap failed.  dimStyle:'%A' gap:'%A'" dimStyle gap
        if gap >= 0.0 then
            ds.TextGap <- gap
            if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleTextGap failed.  dimStyle:'%A' gap:'%A'" dimStyle gap
            Doc.Views.Redraw()
        else
            failwithf "set DimStyleTextGap failed.  dimStyle:'%A' gap:'%A'" dimStyle gap



    [<Extension>]
    ///<summary>Returns the text height used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current text height</returns>
    static member DimStyleTextHeight(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleTextHeight failed.  dimStyle:'%A'" dimStyle
        ds.TextHeight

    [<Extension>]
    ///<summary>Changes the text height used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="height">(float) The new text height</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleTextHeight(dimStyle:string, height:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleTextHeight failed.  dimStyle:'%A' height:'%A'" dimStyle height
        if height>0.0 then
            ds.TextHeight <- height
            if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleTextHeight failed.  dimStyle:'%A' height:'%A'" dimStyle height
            Doc.Views.Redraw()
        else
            failwithf "set DimStyleTextHeight failed.  dimStyle:'%A' height:'%A'" dimStyle height


    [<Extension>]
    ///<summary>Verifies an object is an aligned dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsAlignedDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
            match g with
            | :? LinearDimension as g -> g.Aligned
            | _ -> false


    [<Extension>]
    ///<summary>Verifies an object is an angular dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsAngularDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
            match g with
            | :? AngularDimension -> true
            | _ -> false


    [<Extension>]
    ///<summary>Verifies an object is a diameter dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsDiameterDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
            match g with
            | :? RadialDimension as g -> g.IsDiameterDimension
            | _ -> false


    [<Extension>]
    ///<summary>Verifies an object is a dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
            match g with
            | :? AnnotationBase  -> true
            | _ -> false


    [<Extension>]
    ///<summary>Verifies the existance of a dimension style in the document</summary>
    ///<param name="dimStyle">(string) The name of a dimStyle to test for</param>
    ///<returns>(bool) True or False</returns>
    static member IsDimStyle(dimStyle:string) : bool =
        let ds = Doc.DimStyles.FindName(dimStyle)
        notNull ds


    [<Extension>]
    ///<summary>Verifies that an existing dimension style is from a reference file</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(bool) True or False</returns>
    static member IsDimStyleReference(dimStyle:string) : bool =
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then false
        else ds.IsReference


    [<Extension>]
    ///<summary>Verifies an object is a dimension leader object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLeader(objectId:Guid) : bool =
         match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
         | None -> false
         | Some g ->
            match g with
            | :? Leader  -> true
            | _ -> false


    [<Extension>]
    ///<summary>Verifies an object is a linear dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinearDimension(objectId:Guid) : bool =
         match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
         | None -> false
         | Some g ->
            match g with
            | :? LinearDimension  -> true
            | _ -> false


    [<Extension>]
    ///<summary>Verifies an object is an ordinate dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsOrdinateDimension(objectId:Guid) : bool =
         match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
         | None -> false
         | Some g ->
            match g with
            | :? OrdinateDimension  -> true
            | _ -> false


    [<Extension>]
    ///<summary>Verifies an object is a radial dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsRadialDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
           match g with
           | :? RadialDimension  -> true
           | _ -> false


    [<Extension>]
    ///<summary>Returns the text string of a dimension leader object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(string) The current text string</returns>
    static member LeaderText(objectId:Guid) : string = //GET
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> failwithf "LeaderText failed.  objectId:'%A'" objectId
        | Some g ->
            match g with
            | :? Leader as g ->
                let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
                annotationObject.DisplayText
            | _ -> failwithf "get LeaderText failed.  objectId:'%A'" objectId

    [<Extension>]
    ///<summary>Modifies the text string of a dimension leader object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="text">(string) The new text string</param>
    ///<returns>(unit) void, nothing</returns>
    static member LeaderText(objectId:Guid, text:string) : unit = //SET
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> failwithf "set LeaderText failed. objectId:'%A'" objectId
        | Some g ->
            match g with
            | :? Leader as g ->
                let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
                g.PlainText <- text               // TODO or use rich text?
                if not <| Doc.Objects.Replace(objectId,g) then failwithf "get LeaderText: Objects.Replace(objectId,g) failed. objectId:'%A'" objectId                
                annotationObject.CommitChanges()|> ignore
                Doc.Views.Redraw()
            | _ -> failwithf "set LeaderText failed for  %s"  (rhType objectId)
    [<Extension>]
    ///<summary>Modifies the text string of multiple dimension leader objects</summary>
    ///<param name="objectsIds">(Guid seq) The objects's identifiers</param>
    ///<param name="text">(string) The new text string</param>
    ///<returns>(unit) void, nothing</returns>
    static member LeaderText(objectIds:Guid seq, text:string) : unit = //MULTISET
        for objectId in objectIds do 
            RhinoScriptSyntax.LeaderText(objectId,text) 

    [<Extension>]
    ///<summary>Renames an existing dimension style</summary>
    ///<param name="oldstyle">(string) The name of an existing dimension style</param>
    ///<param name="newstyle">(string) The new dimension style name</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenameDimStyle(oldstyle:string, newstyle:string) : unit =
        let mutable ds = Doc.DimStyles.FindName(oldstyle)
        if isNull ds then  failwithf "renameDimStyle failed.  oldstyle:'%A' newstyle:'%A'" oldstyle newstyle
        ds.Name <- newstyle
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then  failwithf "renameDimStyle failed.  oldstyle:'%A' newstyle:'%A'" oldstyle newstyle


