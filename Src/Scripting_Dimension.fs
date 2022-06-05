
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenDimension =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    ///<summary>Adds an aligned dimension object to the document. An aligned dimension
    ///    is a linear dimension lined up with two points.</summary>
    ///<param name="startPoint">(Point3d) First point of dimension</param>
    ///<param name="endPoint">(Point3d) Second point of dimension</param>
    ///<param name="pointOnDimensionLine">(Point3d) Location point of dimension line</param>
    ///<param name="style">(string) Optional, Default Value: <c>""</c> Name of dimension style</param>
    ///<returns>(Guid) identifier of new dimension.</returns>
    static member AddAlignedDimension(  startPoint:Point3d,
                                        endPoint:Point3d,
                                        pointOnDimensionLine:Point3d,  // TODO allow Point3d.Unset an then draw dim in XY plane
                                        [<OPT;DEF("")>]style:string) : Guid = 
        let plane = Geometry.Plane(startPoint, endPoint, pointOnDimensionLine)
        if not plane.IsValid then RhinoScriptingException.Raise "Rhino.Scripting.AddAlignedDimension failed to create Plane.  startPoint:'%O' endPoint:'%O' pointOnDimensionLine:'%O'" startPoint endPoint pointOnDimensionLine
        let success, s, t = plane.ClosestParameter(startPoint)
        let start2 = Point2d(s, t)
        let success, s, t = plane.ClosestParameter(endPoint)
        let ende2 = Point2d(s, t)
        let success, s, t = plane.ClosestParameter(pointOnDimensionLine)
        let onpoint2 = Point2d(s, t)
        let ldim = new LinearDimension(plane, start2, ende2, onpoint2)
        if isNull ldim then  RhinoScriptingException.Raise "Rhino.Scripting.AddAlignedDimension failed.startPoint:'%O' endPoint:'%O' pointOnDimensionLine:'%O' style:'%s'" startPoint endPoint pointOnDimensionLine style
        ldim.Aligned <- true
        if style <> "" then
            let ds = State.Doc.DimStyles.FindName(style)
            if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.AddAlignedDimension, style not found.  startPoint:'%O' endPoint:'%O' pointOnDimensionLine:'%O' style:'%s'" startPoint endPoint pointOnDimensionLine style
            ldim.DimensionStyleId <- ds.Id
        let rc = State.Doc.Objects.AddLinearDimension(ldim)
        if rc = Guid.Empty then  RhinoScriptingException.Raise "Rhino.Scripting.AddAlignedDimension: Unable to add dimension to document. startPoint:'%O' endPoint:'%O' pointOnDimensionLine:'%O' style:'%s'" startPoint endPoint pointOnDimensionLine style
        State.Doc.Views.Redraw()
        rc


    ///<summary>Adds a new dimension style to the document. The new dimension style will
    ///    be initialized with the current default dimension style properties.</summary>
    ///<param name="dimStyleName">(string) Name of the new dimension style</param>
    ///<returns>(unit) void, nothing.</returns>
    static member AddDimStyle(dimStyleName:string) : unit = 
        let index = State.Doc.DimStyles.Add(dimStyleName)
        if index<0 then  RhinoScriptingException.Raise "Rhino.Scripting.AddDimStyle failed. dimStyleName:'%A'" dimStyleName



    ///<summary>Adds a leader to the document. Leader objects are planar.
    ///    The 3D points passed will define the Plane if no Plane given.
    ///    If ther are only two Points the World XY plane is used.</summary>
    ///<param name="points">(Point3d seq) List of (at least 2) 3D points</param>
    ///<param name="text">(string) Leader's text</param>
    ///<param name="plane">(Geometry.Plane) Optional, Default Value: <c>defined by points arg</c>
    ///    If points will be projected to this Plane</param>
    ///<returns>(Guid) identifier of the new leader.</returns>
    static member AddLeader( points:Point3d seq,
                             text:string,
                             [<OPT;DEF(Plane())>]plane:Plane) : Guid = 
        let points2d = Rarr()
        let plane0 = 
            if plane.IsValid then plane
            else
                let ps= Rarr(points)
                if ps.Count<2 then 
                    RhinoScriptingException.Raise "Rhino.Scripting.AddLeader needs at least two points.  given %A, text:%s" points text
                elif ps.Count=2 then 
                    let y =  ps.[1] - ps.[0]
                    if y.IsTiny(State.Doc.ModelAbsoluteTolerance*100.) then RhinoScriptingException.Raise "Rhino.Scripting.AddLeader two points given are identical %A, text:%s" points text
                    let pl = Plane(ps.[0], Vector3d.CrossProduct (y, Vector3d.ZAxis), y)
                    if not pl.IsValid then RhinoScriptingException.Raise "Rhino.Scripting.AddLeader failed to find plane from two points: %A, text:%s"  points text
                    pl
                else
                    let o = ps.GetNeg(-2)
                    let mutable x = ps.GetNeg(-1) - o
                    let mutable y = ps.[0]-ps.[1]
                    if y.Z < 0.0 then y <- -y
                    if y.Y < 0.0 then y <- -y
                    if x.X < 0.0 then x <- -x
                    let pl = Plane(o, x, y)
                    if not pl.IsValid then RhinoScriptingException.Raise "Rhino.Scripting.AddLeader failed to find plane from %d points: %A, text:%s" ps.Count points text
                    pl

        for point in points do
            let cprc, s, t = plane0.ClosestParameter( point )
            if not cprc then  RhinoScriptingException.Raise "Rhino.Scripting.AddLeader failed.  points %A, text:%s, plane %A" points text plane
            points2d.Add( Rhino.Geometry.Point2d(s, t))
        State.Doc.Objects.AddLeader(text, plane0, points2d)


    ///<summary>Adds a linear dimension to the document.</summary>
    ///<param name="startPoint">(Point3d) The origin, or first point of the dimension</param>
    ///<param name="endPoint">(Point3d) The offset, or second point of the dimension</param>
    ///<param name="pointOnDimensionLine">(Point3d) A point that lies on the dimension line</param>
    ///<param name="plane">(Plane) Optional, The Plane on which the dimension will lie. The default is World XY Plane</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member AddLinearDimension(   startPoint:Point3d,
                                        endPoint:Point3d,
                                        pointOnDimensionLine:Point3d, // TODO allow Point3d.Unset an then draw dim in XY plane
                                        [<OPT;DEF(Plane())>] plane:Plane ) : Guid = 
        let mutable plane0 = if not plane.IsValid then Plane.WorldXY else Plane(plane) // copy // TODO or fail RhinoScriptingException.Raise "Rhino.Scripting.AddAlignedDimension failed to creat Plane.  startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A'" startPoint endPoint pointOnDimensionLine
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
            RhinoScriptingException.Raise "Rhino.Scripting.AddLinearDimension failed.  plane:'%A' startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A'" plane startPoint endPoint pointOnDimensionLine
        let rc = State.Doc.Objects.AddLinearDimension(ldim)
        if rc= Guid.Empty then
            RhinoScriptingException.Raise "Rhino.Scripting.AddLinearDimension: Unable to add dimension to document. plane:'%A' startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A'" plane startPoint endPoint pointOnDimensionLine
        State.Doc.Views.Redraw()
        rc


    ///<summary>Returns the current default dimension style.</summary>
    ///<returns>(string) Name of the current dimension style.</returns>
    static member CurrentDimStyle() : string = //GET
        State.Doc.DimStyles.Current.Name

    ///<summary>Changes the current default dimension style. 
    ///  Raise a RhinoScriptingException if the style does not exist.</summary>
    ///<param name="dimStyleName">(string) Name of an existing dimension style to make current</param>
    ///<returns>(unit) void, nothing.</returns>
    static member CurrentDimStyle(dimStyleName:string) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyleName)
        if notNull ds  then  RhinoScriptingException.Raise "Rhino.Scripting.SetCurrentDimStyle to '%s' failed. " dimStyleName
        if not <| State.Doc.DimStyles.SetCurrent(ds.Index, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.SetCurrentDimStyle to '%s' failed." dimStyleName



    ///<summary>Removes an existing dimension style from the document. The dimension style
    ///    to be removed cannot be referenced by any dimension objects.</summary>
    ///<param name="dimStyleName">(string) The name of an unreferenced dimension style</param>
    ///<returns>(unit) void, nothing (fails on error).</returns>
    static member DeleteDimStyle(dimStyleName:string) : unit = 
        let ds = State.Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then
            RhinoScriptingException.Raise "Rhino.Scripting.DeleteDimStyle failed. dimStyleName:'%s'" dimStyleName
        let ok = State.Doc.DimStyles.Delete(ds.Index, quiet=true)
        if not ok then
            RhinoScriptingException.Raise "Rhino.Scripting.DeleteDimStyle failed. dimStyleName:' %s '" dimStyleName


    ///<summary>Returns the dimension style of a dimension object.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The object's current dimension style name.</returns>
    static member DimensionStyle(objectId:Guid) : string = //GET
        let annotationObject = Scripting.CoerceAnnotation(objectId)
        //let annotation = annotationObject.Geometry :?> AnnotationBase
        let ds = annotationObject.AnnotationGeometry.ParentDimensionStyle
        ds.Name
        // this is how Rhino Python is doing it :
        // let ds:DocObjects.DimensionStyle = annotationObject?DimensionStyle //TODO verify Duck typing works ok

    ///<summary>Modifies the dimension style of a dimension object.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="dimStyleName">(string) The name of an existing dimension style</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimensionStyle(objectId:Guid, dimStyleName:string) : unit = //SET
        let annotationObject = Scripting.CoerceAnnotation(objectId)
        let ds =  State.Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimensionStyle set failed.  objectId:'%s' dimStyleName:'%s'" (Print.guid objectId) dimStyleName
        let mutable annotation = annotationObject.Geometry:?> AnnotationBase
        annotation.DimensionStyleId <- ds.Id
        annotationObject.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.DimensionStyle : CommitChanges failed"
        State.Doc.Views.Redraw()

    ///<summary>Modifies the dimension style of multiple dimension objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifier of the objects</param>
    ///<param name="dimStyleName">(string) The name of multiple existing dimension style</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimensionStyle(objectIds:Guid seq, dimStyleName:string) : unit = //MULTISET
        let ds =  State.Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimensionStyle set failed.  objectId:'%s' dimStyleName:'%s'" (Print.nice objectIds) dimStyleName
        for objectId in objectIds do
            let annotationObject = Scripting.CoerceAnnotation(objectId)
            let mutable annotation = annotationObject.Geometry:?> AnnotationBase
            annotation.DimensionStyleId <- ds.Id
            annotationObject.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.DimensionStyle : CommitChanges failed"
        State.Doc.Views.Redraw()

    ///<summary>Returns the text displayed by a dimension object.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The text displayed by a dimension object.</returns>
    static member DimensionText(objectId:Guid) : string = 
        let annotationObject = Scripting.CoerceAnnotation(objectId)
        annotationObject.DisplayText


    ///<summary>Returns the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The current usertext string.</returns>
    static member DimensionUserText(objectId:Guid) : string = //GET
        let annotationObject = Scripting.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> AnnotationBase
        geo.PlainText

    ///<summary>Modifies the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="usertext">(string) The new user text string value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimensionUserText(objectId:Guid, usertext:string) : unit = //SET
        let annotationObject = Scripting.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> AnnotationBase
        geo.PlainText <- usertext
        annotationObject.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.DimensionUserText : CommitChanges failed"
        State.Doc.Views.Redraw()

    ///<summary>Modifies the user text string of multiple dimension objects. The user
    /// text is the string that gets printed when the dimension is defined.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of the objects</param>
    ///<param name="usertext">(string) The new user text string value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimensionUserText(objectIds:Guid seq, usertext:string) : unit = //MULTISET
        for objectId in objectIds do
            let annotationObject = Scripting.CoerceAnnotation(objectId)
            let geo = annotationObject.Geometry :?> AnnotationBase
            geo.PlainText <- usertext
            annotationObject.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.DimensionUserText : CommitChanges failed"
        State.Doc.Views.Redraw()

    ///<summary>Returns the value of a dimension object.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) numeric value of the dimension.</returns>
    static member DimensionValue(objectId:Guid) : float = 
        let annotationObject = Scripting.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> Dimension
        geo.NumericValue


    ///<summary>Returns the angle display precision of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current angle precision.</returns>
    static member DimStyleAnglePrecision(dimStyle:string) : int = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleAnglePrecision get failed. dimStyle:'%s'" dimStyle
        ds.AngleResolution

    ///<summary>Changes the angle display precision of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="precision">(int) The new angle precision value.</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleAnglePrecision(dimStyle:string, precision:int) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleAnglePrecision set failed. dimStyle:'%s' precision:%d" dimStyle precision
        let rc = ds.AngleResolution
        if precision >= 0 then
            ds.AngleResolution <- precision
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then RhinoScriptingException.Raise "Rhino.Scripting.DimStyleAnglePrecision set failed. dimStyle:'%s' precision:%d" dimStyle precision
            State.Doc.Views.Redraw()



    ///<summary>Returns the arrow size of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current arrow size.</returns>
    static member DimStyleArrowSize(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleArrowSize get failed. dimStyle:'%s'" dimStyle
        ds.ArrowLength

    ///<summary>Changes the arrow size of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="size">(float) The new arrow size</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleArrowSize(dimStyle:string, size:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleArrowSize set failed. dimStyle:'%s' size:'%A'" dimStyle size
        let rc = ds.ArrowLength
        if size > 0.0 then
            ds.ArrowLength <- size
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then RhinoScriptingException.Raise "Rhino.Scripting.DimStyleArrowSize set failed. dimStyle:'%s' size: %g" dimStyle size
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleArrowSize set failed. dimStyle:'%s' size:%g" dimStyle size



    ///<summary>Returns the number of dimension styles in the document.</summary>
    ///<returns>(int) The number of dimension styles in the document.</returns>
    static member DimStyleCount() : int = 
        State.Doc.DimStyles.Count


    ///<summary>Returns the extension line extension of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current extension line extension.</returns>
    static member DimStyleExtension(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleExtension get failed. dimStyle:'%s'" dimStyle
        ds.ExtensionLineExtension

    ///<summary>Changes the extension line extension of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="extension">(float) The new extension line extension</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleExtension(dimStyle:string, extension:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleExtension set failed. dimStyle:'%s' extension:'%A'" dimStyle extension
        let rc = ds.ExtensionLineExtension
        if extension > 0.0 then
            ds.ExtensionLineExtension <- extension
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "Rhino.Scripting.DimStyleExtension failed. dimStyle:'%s' extension:'%A'" dimStyle extension
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleExtension set failed. dimStyle:'%s' extension:'%A'" dimStyle extension



    ///<summary>Returns the font used by a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(string) The current font.</returns>
    static member DimStyleFont(dimStyle:string) : string = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleFont get failed. dimStyle:'%s'" dimStyle
        ds.Font.FaceName


    ///<summary>Changes the font used by a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="font">(string) The new font face name</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleFont(dimStyle:string, font:string) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleFont set failed. dimStyle:'%s' font:'%A'" dimStyle font

        ds.Font <- DocObjects.Font(font) // TODO check if works OK !
        // let newindex = State.Doc.Fonts.FindOrCreate(font, false, false) // deprecated ??
        // ds.Font <- State.Doc.Fonts.[newindex]
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleFont set failed. dimStyle:'%s' font:'%A'" dimStyle font
        State.Doc.Views.Redraw()


    ///<summary>Gets all Available Font Face Names.</summary>
    ///<returns>(string array) array of all available font names.</returns>
    static member DimStyleAvailableFonts() : array<string> = // not part of original rhinoscriptsyntax
        DocObjects.Font.AvailableFontFaceNames()



    ///<summary>Returns the leader arrow size of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current leader arrow size.</returns>
    static member DimStyleLeaderArrowSize(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLeaderArrowSize get failed. dimStyle:'%s'" dimStyle
        ds.LeaderArrowLength

    ///<summary>Changes the leader arrow size of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="size">(float) The new leader arrow size</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleLeaderArrowSize(dimStyle:string, size:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLeaderArrowSize set failed. dimStyle:'%s' size:'%A'" dimStyle size
        if size > 0.0 then
            ds.LeaderArrowLength <- size
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLeaderArrowSize set failed. dimStyle:'%s' size:'%A'" dimStyle size
            State.Doc.Views.Redraw()



    ///<summary>Returns the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) if factor is not defined, the current length factor.</returns>
    static member DimStyleLengthFactor(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLengthFactor get failed. dimStyle:'%s'" dimStyle
        ds.LengthFactor

    ///<summary>Changes the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="factor">(float) The new length factor</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleLengthFactor(dimStyle:string, factor:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLengthFactor set failed. dimStyle:'%s' factor:'%A'" dimStyle factor
        ds.LengthFactor <- factor
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLengthFactor set failed. dimStyle:'%s' factor:'%A'" dimStyle factor
        State.Doc.Views.Redraw()



    ///<summary>Returns the linear display precision of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current linear precision value.</returns>
    static member DimStyleLinearPrecision(dimStyle:string) : int = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLinearPrecision get failed. dimStyle:'%s'" dimStyle
        ds.LengthResolution

    ///<summary>Changes the linear display precision of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="precision">(int) The new linear precision value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleLinearPrecision(dimStyle:string, precision:int) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLinearPrecision set failed. dimStyle:'%s' precision: %d" dimStyle precision
        if precision >= 0 then
            ds.LengthResolution <- precision
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLinearPrecision set failed. dimStyle:'%s' precision: %d" dimStyle precision
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleLinearPrecision set failed. dimStyle:'%s' precision: %d" dimStyle precision



    ///<summary>Returns the names of all dimension styles in the document.</summary>
    ///<returns>(string Rarr) The names of all dimension styles in the document.</returns>
    static member DimStyleNames() : string Rarr = 
        rarr {for  ds in State.Doc.DimStyles -> ds.Name }


    ///<summary>Returns the number display format of a dimension style.</summary>
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
    ///     Miles            9  Decimal Miles.</returns>
    static member DimStyleNumberFormat(dimStyle:string) : int = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleNumberFormat get failed. dimStyle:'%s'" dimStyle
        int ds.DimensionLengthDisplay


    ///<summary>Changes the number display format of a dimension style.</summary>
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
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleNumberFormat(dimStyle:string, format:int) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleNumberFormat set failed. dimStyle:'%s' format:'%A'" dimStyle format
        if  format<0 || format>9 then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleNumberFormat set failed. dimStyle:'%s' format:'%A'" dimStyle format
        ds.DimensionLengthDisplay <- LanguagePrimitives.EnumOfValue format
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleNumberFormat set failed. dimStyle:'%s' format:'%A'" dimStyle format
        State.Doc.Views.Redraw()


    ///<summary>Returns the extension line offset of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current extension line offset.</returns>
    static member DimStyleOffset(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleOffset get failed. dimStyle:'%s'" dimStyle
        ds.ExtensionLineOffset

    ///<summary>Changes the extension line offset of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="offset">(float) The new extension line offset</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleOffset(dimStyle:string, offset:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleOffset set failed. dimStyle:'%s' offset:'%A'" dimStyle offset
        ds.ExtensionLineOffset <- offset
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleOffset set failed. dimStyle:'%s' offset:'%A'" dimStyle offset
        State.Doc.Views.Redraw()



    ///<summary>Returns the prefix of a dimension style - the text to
    /// prefix to the dimension text.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<returns>(string) The current prefix.</returns>
    static member DimStylePrefix(dimStyle:string) : string = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStylePrefix get failed. dimStyle:'%s'" dimStyle
        ds.Prefix

    ///<summary>Changes the prefix of a dimension style - the text to
    /// prefix to the dimension text.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<param name="prefix">(string) The new prefix</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStylePrefix(dimStyle:string, prefix:string) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStylePrefix set failed. dimStyle:'%s' prefix:'%A'" dimStyle prefix
        ds.Prefix <- prefix
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.DimStylePrefix set failed. dimStyle:'%s' prefix:'%A'" dimStyle prefix
        State.Doc.Views.Redraw()


    ///<summary>Returns the scale of a dimension style .</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<returns>(string) The current suffix.</returns>
    static member DimStyleScale(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleScale get failed. dimStyle:'%s'" dimStyle
        ds.DimensionScale

    ///<summary>Changes the scale of a dimension style .</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<param name="scale">(float) The new scale</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleScale(dimStyle:string, scale:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleScale set failed. dimStyle:'%s' scale:'%A'" dimStyle scale
        ds.DimensionScale <- scale
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleScale set failed. dimStyle:'%s' scale:'%A'" dimStyle scale
        State.Doc.Views.Redraw()


    ///<summary>Returns the suffix of a dimension style - the text to
    /// append to the dimension text.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<returns>(string) The current suffix.</returns>
    static member DimStyleSuffix(dimStyle:string) : string = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleSuffix get failed. dimStyle:'%s'" dimStyle
        ds.Suffix

    ///<summary>Changes the suffix of a dimension style - the text to
    /// append to the dimension text.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<param name="suffix">(string) The new suffix</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleSuffix(dimStyle:string, suffix:string) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleSuffix set failed. dimStyle:'%s' suffix:'%A'" dimStyle suffix
        ds.Suffix <- suffix
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleSuffix set failed. dimStyle:'%s' suffix:'%A'" dimStyle suffix
        State.Doc.Views.Redraw()



    ///<summary>Returns the text alignment mode of a dimension style.</summary>
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
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextAlignment get failed. dimStyle:'%s'" dimStyle
        int ds.TextVerticalAlignment

    ///<summary>Changes the text alignment mode of a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="alignment">(int) The new text alignment
    ///     Top                   0   Attach to top of an 'I' on the first line. (Independent of glyphs being displayed.)
    ///     MiddleOfTop           1   Attach to middle of an 'I' on the first line. (Independent of glyphs being displayed.)
    ///     BottomOfTop           2   Attach to baseline of first line. (Independent of glyphs being displayed.)
    ///     Middle                3   Attach to middle of text vertical advance. (Independent of glyphs being displayed.)
    ///     MiddleOfBottom        4   Attach to middle of an 'I' on the last line. (Independent of glyphs being displayed.)
    ///     Bottom                5   Attach to the basline of the last line. (Independent of glyphs being displayed.)
    ///     BottomOfBoundingBox   6   Attach to the bottom of the boudning box of the visible glyphs.</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleTextAlignment(dimStyle:string, alignment:int) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextAlignment not found. dimStyle:'%s' alignment:'%A'" dimStyle alignment
        elif alignment<0 || alignment>6 then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextAlignment set failed. dimStyle:'%s' alignment:'%A'" dimStyle alignment
        ds.TextVerticalAlignment <- LanguagePrimitives.EnumOfValue (byte alignment)
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextAlignment set failed. dimStyle:'%s' alignment:'%A'" dimStyle alignment
        State.Doc.Views.Redraw()


    ///<summary>Returns the text gap used by a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current text gap.</returns>
    static member DimStyleTextGap(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextGap get failed. dimStyle:'%s'" dimStyle
        ds.TextGap

    ///<summary>Changes the text gap used by a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="gap">(float) The new text gap</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleTextGap(dimStyle:string, gap:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextGap set failed. dimStyle:'%s' gap:'%A'" dimStyle gap
        if gap >= 0.0 then
            ds.TextGap <- gap
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextGap set failed. dimStyle:'%s' gap:'%A'" dimStyle gap
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextGap set failed. dimStyle:'%s' gap:'%A'" dimStyle gap



    ///<summary>Returns the text height used by a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current text height.</returns>
    static member DimStyleTextHeight(dimStyle:string) : float = //GET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextHeight get failed. dimStyle:'%s'" dimStyle
        ds.TextHeight

    ///<summary>Changes the text height used by a dimension style.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="height">(float) The new text height</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DimStyleTextHeight(dimStyle:string, height:float) : unit = //SET
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextHeight set failed. dimStyle:'%s' height:'%A'" dimStyle height
        if height>0.0 then
            ds.TextHeight <- height
            if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
                RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextHeight set failed. dimStyle:'%s' height:'%A'" dimStyle height
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "Rhino.Scripting.DimStyleTextHeight set failed. dimStyle:'%s' height:'%A'" dimStyle height


    ///<summary>Checks if  an object is an aligned dimension object. Returns false for any other Rhino object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsAlignedDimension(objectId:Guid) : bool = 
        match Scripting.CoerceGeometry objectId with
        | :? LinearDimension as g -> g.Aligned
        | _ -> false


    ///<summary>Checks if  an object is an angular dimension object. Returns false for any other Rhino object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsAngularDimension(objectId:Guid) : bool = 
        match Scripting.CoerceGeometry objectId with
        | :? AngularDimension -> true
        | _ -> false


    ///<summary>Checks if  an object is a diameter dimension object. Returns false for any other Rhino object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDiameterDimension(objectId:Guid) : bool = 
        match Scripting.CoerceGeometry objectId with
        | :? RadialDimension as g -> g.IsDiameterDimension
        | _ -> false


    ///<summary>Checks if  an object is a dimension object. Returns false for any other Rhino object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDimension(objectId:Guid) : bool = 
        match Scripting.CoerceGeometry objectId with
        | :? AnnotationBase  -> true
        | _ -> false


    ///<summary>Checks if  the existance of a dimension style in the document. Returns false for any other Rhino object.</summary>
    ///<param name="dimStyle">(string) The name of a dimStyle to test for</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDimStyle(dimStyle:string) : bool = 
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        notNull ds


    ///<summary>Checks if  that an existing dimension style is from a reference file. Returns false for any other Rhino object.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDimStyleReference(dimStyle:string) : bool = 
        let ds = State.Doc.DimStyles.FindName(dimStyle)
        if isNull ds then false
        else ds.IsReference


    ///<summary>Checks if  an object is a dimension leader object. Returns false for any other Rhino object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLeader(objectId:Guid) : bool = 
            match Scripting.CoerceGeometry objectId with
            | :? Leader  -> true
            | _ -> false


    ///<summary>Checks if  an object is a linear dimension object. Returns false for any other Rhino object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLinearDimension(objectId:Guid) : bool = 
            match Scripting.CoerceGeometry objectId with
            | :? LinearDimension  -> true
            | _ -> false


    ///<summary>Checks if  an object is an ordinate dimension object. Returns false for any other Rhino object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsOrdinateDimension(objectId:Guid) : bool = 
            match Scripting.CoerceGeometry objectId with
            | :? OrdinateDimension  -> true
            | _ -> false


    ///<summary>Checks if  an object is a radial dimension object. Returns false for any other Rhino object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsRadialDimension(objectId:Guid) : bool = 
            match Scripting.CoerceGeometry objectId with
            | :? RadialDimension  -> true
            | _ -> false


    ///<summary>Returns the text string of a dimension leader object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(string) The current text string.</returns>
    static member LeaderText(objectId:Guid) : string = //GET
            match Scripting.CoerceGeometry objectId with
            | :? Leader as g ->
                let annotationObject = Scripting.CoerceAnnotation(objectId)
                annotationObject.DisplayText
            | _ -> RhinoScriptingException.Raise "Rhino.Scripting.LeaderText get failed.  objectId:'%s'" (Print.guid objectId)

    ///<summary>Modifies the text string of a dimension leader object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="text">(string) The new text string</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LeaderText(objectId:Guid, text:string) : unit = //SET
            match Scripting.CoerceGeometry objectId with
            | :? Leader as g ->
                let annotationObject = Scripting.CoerceAnnotation(objectId)
                g.PlainText <- text               // TODO or use rich text?
                if not <| State.Doc.Objects.Replace(objectId,g) then RhinoScriptingException.Raise "Rhino.Scripting.LeaderText: Objects.Replace(objectId,g) get failed. objectId:'%s'" (Print.guid objectId)
                annotationObject.CommitChanges() |> RhinoScriptingException.FailIfFalse "Rhino.Scripting.LeaderText : CommitChanges failed"
                State.Doc.Views.Redraw()
            | _ -> RhinoScriptingException.Raise "Rhino.Scripting.LeaderText set failed for  %s"  (Print.guid objectId)

    ///<summary>Modifies the text string of multiple dimension leader objects.</summary>
    ///<param name="objectIds">(Guid seq) The objects's identifiers</param>
    ///<param name="text">(string) The new text string</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LeaderText(objectIds:Guid seq, text:string) : unit = //MULTISET
        for objectId in objectIds do
            Scripting.LeaderText(objectId,text)

    ///<summary>Renames an existing dimension style.</summary>
    ///<param name="oldstyle">(string) The name of an existing dimension style</param>
    ///<param name="newstyle">(string) The new dimension style name</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenameDimStyle(oldstyle:string, newstyle:string) : unit = 
        let mutable ds = State.Doc.DimStyles.FindName(oldstyle)
        if isNull ds then  RhinoScriptingException.Raise "Rhino.Scripting.RenameDimStyle failed.  oldstyle:'%s' newstyle:'%s'" oldstyle newstyle
        ds.Name <- newstyle
        if not <| State.Doc.DimStyles.Modify(ds, ds.Id, quiet=false) then
            RhinoScriptingException.Raise "Rhino.Scripting.RenameDimStyle failed.  oldstyle:'%s' newstyle:'%s'" oldstyle newstyle



