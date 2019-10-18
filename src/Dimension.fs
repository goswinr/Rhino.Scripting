namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
open Microsoft.FSharp.Core.LanguagePrimitives
open System.Collections.Generic

[<AutoOpen>]
module ExtensionsDimension =
  
   
  type RhinoScriptSyntax with
    
    
    [<EXT>]  
    ///<summary>Adds an aligned dimension object to the document. An aligned dimension
    ///  is a linear dimension lined up with two points</summary>
    ///<param name="startPoint">(Point3d) First point of dimension</param>
    ///<param name="endPoint">(Point3d) Second point of dimension</param>
    ///<param name="pointOnDimensionLine">(Point3d) Location point of dimension line</param>
    ///<param name="style">(string) Optional, Default Value: <c>""</c>
    ///Name of dimension style</param>
    ///<returns>(Guid) identifier of new dimension on success</returns>
    static member AddAlignedDimension(startPoint:Point3d, endPoint:Point3d, pointOnDimensionLine:Point3d, [<OPT;DEF("")>]style:string) : Guid =
        let mutable start = startPoint
        let mutable ende = endPoint
        let mutable onpoint = pointOnDimensionLine
        let mutable plane = Geometry.Plane(start, ende, onpoint)
        let mutable success, s, t = plane.ClosestParameter(start)
        let start2 = Point2d(s,t)
        let success, s, t = plane.ClosestParameter(ende)
        let ende2 = Point2d(s,t)
        let success, s, t = plane.ClosestParameter(onpoint)
        let onpoint2 = Point2d(s,t)
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
    (*
    def AddAlignedDimension(start_point, end_point, point_on_dimension_line, style=None):
        '''Adds an aligned dimension object to the document. An aligned dimension
        is a linear dimension lined up with two points
        Parameters:
          start_point (point): first point of dimension
          end_point (point): second point of dimension
          point_on_dimension_line (point): location point of dimension line
          style (str, optional): name of dimension style
        Returns:
          guid: identifier of new dimension on success
          None: on error
        '''
        start = rhutil.Coerce3dpoint(start_point, True)
        end = rhutil.Coerce3dpoint(end_point, True)
        onpoint = rhutil.Coerce3dpoint(point_on_dimension_line, True)
        plane = Rhino.Geometry.Plane(start, end, onpoint)
        success, s, t = plane.ClosestParameter(start)
        start = Rhino.Geometry.Point2d(s,t)
        success, s, t = plane.ClosestParameter(end)
        end = Rhino.Geometry.Point2d(s,t)
        success, s, t = plane.ClosestParameter(onpoint)
        onpoint = Rhino.Geometry.Point2d(s,t)
        ldim = Rhino.Geometry.LinearDimension(plane, start, end, onpoint)
        if not ldim: return scriptcontext.errorhandler()
        ldim.Aligned = True
        if style:
            ds = scriptcontext.doc.DimStyles.FindName(style)
            if ds is None: return scriptcontext.errorhandler()
            ldim.DimensionStyleId = ds.Id
        rc = scriptcontext.doc.Objects.AddLinearDimension(ldim)
        if rc==System.Guid.Empty: raise Exception("unable to add dimension to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Adds a new dimension style to the document. The new dimension style will
    ///  be initialized with the current default dimension style properties.</summary>
    ///<param name="dimStyleName">(string) Name of the new dimension style. </param>
    ///<returns>(unit) void, nothing</returns>
    static member AddDimStyle(dimStyleName:string) : unit =
        let index = Doc.DimStyles.Add(dimStyleName)
        if index<0 then  failwithf "addDimStyle failed.  dimStyleName:'%A'" dimStyleName
        
        
    (*
    def AddDimStyle(dimStyle_name=None):
        '''Adds a new dimension style to the document. The new dimension style will
        be initialized with the current default dimension style properties.
        Parameters:
          dimStyle_name (str, optional): name of the new dimension style. If omitted, Rhino automatically generates the dimension style name
        Returns:
          str: name of the new dimension style on success
          None: on error
        '''
        index = scriptcontext.doc.DimStyles.Add(dimStyle_name)
        if index<0: return scriptcontext.errorhandler()
        ds = scriptcontext.doc.DimStyles[index]
        return ds.Name
    *)


    [<EXT>]  
    ///<summary>Adds a leader to the document. Leader objects are planar.
    ///  The 3D points passed will define the plane if no Plane given</summary>
    ///<param name="points">(Point3d seq) List of (at least 2) 3D points</param>
    ///<param name="text">(string) Leader's text </param>
    ///<param name="plane">(string) Optional, Default Value: <c>defined by points arg</c>
    ///  If points will be projected to this plane</param>    
    ///<returns>(Guid) identifier of the new leader on success</returns>
    static member AddLeader(points:Point3d seq, text:string, [<OPT;DEF(Plane())>]plane:Plane) : Guid =
        let points2d = ResizeArray()
        let plane0 = 
            if plane.IsValid then plane 
            else 
                let ps=ResizeArray(points)
                let o = ps.GetItem(-2)
                let mutable x = ps.GetItem(-1)-o
                let mutable y = ps.[0]-ps.[1]
                if y.Z < 0.0 then y <- -y
                if y.Y < 0.0 then y <- -y
                if x.X < 0.0 then x <- -x
                Plane(o,x,y)
                |> fun pl-> 
                    if not pl.IsValid then failwithf "AddLeader failed to find plane.  points %A, text:%s" points text 
                    pl

        for point in points do
            let cprc, s, t = plane0.ClosestParameter( point )
            if not cprc then  failwithf "AddLeader failed.  points %A, text:%s, plane %A" points text plane
            points2d.Add( Rhino.Geometry.Point2d(s,t) )
        Doc.Objects.AddLeader(text, plane0, points2d)
    (*
    def AddLeader(points, view_or_plane=None, text=None):
        '''Adds a leader to the document. Leader objects are planar.
        The 3D points passed to this function should be co-planar
        Parameters:
          points ([point, point, ....])list of (at least 2) 3D points
          view_or_plane (str, optional): If a view name is specified, points will be constrained
            to the view's construction plane. If a view is not specified, points
            will be constrained to a plane fit through the list of points
          text (str, optional): leader's text string
        Returns:
          guid: identifier of the new leader on success
          None: on error
        '''
        points = rhutil.Coerce3dpointlist(points)
        if points is None or len(points)<2: raise ValueError("points must have at least two items")
        rc = System.Guid.Empty
        view = None
        if text and not isinstance(text, str): 
            text = str(text)
        if not view_or_plane:
            if len(points) == 2:
                plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
                rc = scriptcontext.doc.Objects.AddLeader(text, plane, [Rhino.Geometry.Point2d(p.X, p.Y) for p in points])
            else:
                rc = scriptcontext.doc.Objects.AddLeader(text, points)
        else:
            plane = rhutil.Coerceplane(view_or_plane)
            if not plane:
                view = __viewhelper(view_or_plane)
                plane = view.ActiveViewport.ConstructionPlane()
            points2d = []
            for point in points:
                cprc, s, t = plane.ClosestParameter( point )
                if not cprc: return scriptcontext.errorhandler()
                points2d.append( Rhino.Geometry.Point2d(s,t) )
            if text is None:
                rc = scriptcontext.doc.Objects.AddLeader(plane, points2d)
            else:
                if not isinstance(text, str): text = str(text)
                rc = scriptcontext.doc.Objects.AddLeader(text, plane, points2d)
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Adds a linear dimension to the document</summary>
    ///<param name="plane">(Plane) The plane on which the dimension will lie.</param>
    ///<param name="startPoint">(Point3d) The origin, or first point of the dimension.</param>
    ///<param name="endPoint">(Point3d) The offset, or second point of the dimension.</param>
    ///<param name="pointOnDimensionLine">(Point3d) A point that lies on the dimension line.</param>
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddLinearDimension(plane:Plane, startPoint:Point3d, endPoint:Point3d, pointOnDimensionLine:Point3d) : Guid =
        let mutable plane0 = if plane = Plane.Unset then Doc.Views.ActiveView.ActiveViewport.ConstructionPlane() else Plane(plane) // copy
        plane0.Origin <- startPoint // needed ?
        // Calculate 2d dimension points
        let success, s, t = plane0.ClosestParameter(startPoint)
        let start = Point2d(s,t)
        let success, s, t = plane0.ClosestParameter(endPoint)
        let ende = Point2d(s,t)
        let success, s, t = plane0.ClosestParameter(pointOnDimensionLine)
        let onpoint = Point2d(s,t)
        // Add the dimension
        let ldim = new LinearDimension(plane0, start, ende, onpoint)
        if isNull ldim then
            failwithf "addLinearDimension failed.  plane:'%A' startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A'" plane startPoint endPoint pointOnDimensionLine
        let rc = Doc.Objects.AddLinearDimension(ldim)
        if rc=Guid.Empty then
            failwithf "addLinearDimension Unable to add dimension to document.  plane:'%A' startPoint:'%A' endPoint:'%A' pointOnDimensionLine:'%A'" plane startPoint endPoint pointOnDimensionLine
        Doc.Views.Redraw()
        rc
    (*
    def AddLinearDimension(plane, start_point, end_point, point_on_dimension_line):
        '''Adds a linear dimension to the document
        Parameters:
          plane (plane): The plane on which the dimension will lie.
          start_point (point): The origin, or first point of the dimension.
          end_point (point): The offset, or second point of the dimension.
          point_on_dimension_line (point): A point that lies on the dimension line.
        Returns:
          guid: identifier of the new object on success
          None: on error
        '''
        if not plane: 
          plane = ViewCPlane()
        else:
          plane = rhutil.Coerceplane(plane, True)
        start = rhutil.Coerce3dpoint(start_point, True)
        plane.Origin = start
        end = rhutil.Coerce3dpoint(end_point, True)
        onpoint = rhutil.Coerce3dpoint(point_on_dimension_line, True)
        # Calculate 2d dimension points
        success, s, t = plane.ClosestParameter(start)
        start = Rhino.Geometry.Point2d(s,t)
        success, s, t = plane.ClosestParameter(end)
        end = Rhino.Geometry.Point2d(s,t)
        success, s, t = plane.ClosestParameter(onpoint)
        onpoint = Rhino.Geometry.Point2d(s,t)    
        # Add the dimension
        ldim = Rhino.Geometry.LinearDimension(plane, start, end, onpoint)
        if not ldim: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddLinearDimension(ldim)
        if rc==System.Guid.Empty: raise Exception("unable to add dimension to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the current default dimension style</summary>
    ///<returns>(string) Name of the current dimension style</returns>
    static member CurrentDimStyle() : string = //GET
        Doc.DimStyles.Current.Name
    (*
    def CurrentDimStyle(dimStyle_name=None):
        '''Returns or changes the current default dimension style
        Parameters:
          dimStyle_name (str, optional): name of an existing dimension style to make current
        Returns:
          str: if dimStyle_name is not specified, name of the current dimension style
          str: if dimStyle_name is specified, name of the previous dimension style
          None: on error
        '''
        rc = scriptcontext.doc.DimStyles.Current.Name
        if dimStyle_name:
            ds = scriptcontext.doc.DimStyles.FindName(dimStyle_name)
            if ds is None: return scriptcontext.errorhandler()
            scriptcontext.doc.DimStyles.SetCurrent(ds.Index, False)
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the current default dimension style</summary>
    ///<param name="dimStyleName">(string)Name of an existing dimension style to make current</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentDimStyle(dimStyleName:string) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyleName)
        if notNull ds  then  failwithf "setCurrentDimStyle failed.  dimStyleName:'%A'" dimStyleName
        if not <| Doc.DimStyles.SetCurrent(ds.Index, false) then
            failwithf "setCurrentDimStyle failed.  dimStyleName:'%A'" dimStyleName
        
    (*
    def CurrentDimStyle(dimStyle_name=None):
        '''Returns or changes the current default dimension style
        Parameters:
          dimStyle_name (str, optional): name of an existing dimension style to make current
        Returns:
          str: if dimStyle_name is not specified, name of the current dimension style
          str: if dimStyle_name is specified, name of the previous dimension style
          None: on error
        '''
        rc = scriptcontext.doc.DimStyles.Current.Name
        if dimStyle_name:
            ds = scriptcontext.doc.DimStyles.FindName(dimStyle_name)
            if ds is None: return scriptcontext.errorhandler()
            scriptcontext.doc.DimStyles.SetCurrent(ds.Index, False)
        return rc
    *)


    [<EXT>]  
    ///<summary>Removes an existing dimension style from the document. The dimension style
    ///  to be removed cannot be referenced by any dimension objects.</summary>
    ///<param name="dimStyleName">(string) The name of an unreferenced dimension style</param>
    ///<returns>(unit) void, nothing (fails on error)</returns>
    static member DeleteDimStyle(dimStyleName:string) : unit =
        let ds = Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then
            failwithf "deleteDimStyle failed. dimStyleName:'%A'" dimStyleName
        let ok = Doc.DimStyles.Delete(ds.Index, true)
        if not ok then
            failwithf "deleteDimStyle failed. dimStyleName:' %A '" dimStyleName
    (*
    def DeleteDimStyle(dimStyle_name):
        '''Removes an existing dimension style from the document. The dimension style
        to be removed cannot be referenced by any dimension objects.
        Parameters:
          dimStyle_name (str): the name of an unreferenced dimension style
        Returns:
          str: The name of the deleted dimension style if successful
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle_name)
        if ds and scriptcontext.doc.DimStyles.DeleteDimensionStyle(ds.Index, True):
            return dimStyle_name
        return scriptcontext.errorhandler()
    *)


    [<EXT>]  
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
    (*
    def DimensionStyle(object_id, dimStyle_name=None):
        '''Returns or modifies the dimension style of a dimension object
        Parameters:
          object_id (guid): identifier of the object
          dimStyle_name (str, optional): the name of an existing dimension style
        Returns:
          str: if dimStyle_name is not specified, the object's current dimension style name
          str: if dimStyle_name is specified, the object's previous dimension style name
          None: on error
        '''
        annotation_object = __Coerceannotation(object_id)
        ds = annotation_object.AnnotationGeometry.ParentDimensionStyle
        rc = ds.Name
        if dimStyle_name:
            ds = scriptcontext.doc.DimStyles.FindName(dimStyle_name)
            if not ds: return scriptcontext.errorhandler()
            annotation = annotation_object.Geometry
            annotation.DimensionStyleId = ds.Id
            annotation_object.CommitChanges()
        return rc
    *)

    [<EXT>]  
    ///<summary>Modifies the dimension style of a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="dimStyleName">(string)The name of an existing dimension style</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimensionStyle(objectId:Guid, dimStyleName:string) : unit = //SET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let ds =  Doc.DimStyles.FindName(dimStyleName)
        if isNull ds then  failwithf "set DimensionStyle failed.  objectId:'%A' dimStyleName:'%A'" objectId dimStyleName
        let mutable annotation = annotationObject.Geometry:?> AnnotationBase
        annotation.DimensionStyleId <- ds.Id
        annotationObject.CommitChanges() |>ignore // TODO verify this works ok
        
    (*
    def DimensionStyle(object_id, dimStyle_name=None):
        '''Returns or modifies the dimension style of a dimension object
        Parameters:
          object_id (guid): identifier of the object
          dimStyle_name (str, optional): the name of an existing dimension style
        Returns:
          str: if dimStyle_name is not specified, the object's current dimension style name
          str: if dimStyle_name is specified, the object's previous dimension style name
          None: on error
        '''
        annotation_object = __Coerceannotation(object_id)
        ds = annotation_object.AnnotationGeometry.ParentDimensionStyle
        rc = ds.Name
        if dimStyle_name:
            ds = scriptcontext.doc.DimStyles.FindName(dimStyle_name)
            if not ds: return scriptcontext.errorhandler()
            annotation = annotation_object.Geometry
            annotation.DimensionStyleId = ds.Id
            annotation_object.CommitChanges()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the text displayed by a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) the text displayed by a dimension object</returns>
    static member DimensionText(objectId:Guid) : string =
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        annotationObject.DisplayText
    (*
    def DimensionText(object_id):
        '''Returns the text displayed by a dimension object
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          str: the text displayed by a dimension object
        '''
        annotation_object = __Coerceannotation(object_id)
        return annotation_object.DisplayText
    *)


    [<EXT>]  
    ///<summary>Returns the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The current usertext string</returns>
    static member DimensionUserText(objectId:Guid) : string = //GET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> AnnotationBase
        geo.PlainText
    (*
    def DimensionUserText(object_id, usertext=None):
        '''Returns or modifies the user text string of a dimension object. The user
        text is the string that gets printed when the dimension is defined
        Parameters:
          object_id (guid): identifier of the object
          usertext (str, optional): the new user text string value
        Returns:
          str: if usertext is not specified, the current usertext string
          str: if usertext is specified, the previous usertext string
        '''
        annotation_object = __Coerceannotation(object_id)
        rc = annotation_object.Geometry.Text
        if usertext is not None:
            annotation_object.Geometry.PlainText = usertext
            annotation_object.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Modifies the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="usertext">(string)The new user text string value</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimensionUserText(objectId:Guid, usertext:string) : unit = //SET
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> AnnotationBase
        geo.PlainText <- usertext
        annotationObject.CommitChanges() |>ignore
        Doc.Views.Redraw()
        
    (*
    def DimensionUserText(object_id, usertext=None):
        '''Returns or modifies the user text string of a dimension object. The user
        text is the string that gets printed when the dimension is defined
        Parameters:
          object_id (guid): identifier of the object
          usertext (str, optional): the new user text string value
        Returns:
          str: if usertext is not specified, the current usertext string
          str: if usertext is specified, the previous usertext string
        '''
        annotation_object = __Coerceannotation(object_id)
        rc = annotation_object.Geometry.PlainText
        if usertext is not None:
            annotation_object.Geometry.PlainText = usertext
            annotation_object.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the value of a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) numeric value of the dimension</returns>
    static member DimensionValue(objectId:Guid) : float =
        let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
        let geo = annotationObject.Geometry :?> Dimension
        geo.NumericValue
    (*
    def DimensionValue(object_id):
        '''Returns the value of a dimension object
        Parameters:
          object_id (guid): identifier of the object
        Returns:
          number: numeric value of the dimension if successful
        '''
        annotation_object = __Coerceannotation(object_id)
        return annotation_object.Geometry.NumericValue
    *)


    [<EXT>]  
    ///<summary>Returns the angle display precision of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current angle precision</returns>
    static member DimStyleAnglePrecision(dimStyle:string) : int = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleAnglePrecision failed.  dimStyle:'%A'" dimStyle
        ds.AngleResolution
    (*
    def DimStyleAnglePrecision(dimStyle, precision=None):
        '''Returns or changes the angle display precision of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          precision (number, optional): the new angle precision value. If omitted, the current angle
            precision is returned
        Returns:
          number: If a precision is not specified, the current angle precision
          number: If a precision is specified, the previous angle precision
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.AngleResolution
        if precision is not None:
            ds.AngleResolution = precision
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the angle display precision of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="precision">(int)The new angle precision value. If omitted, the current angle
    ///  precision is returned</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleAnglePrecision(dimStyle:string, precision:int) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleAnglePrecision failed.  dimStyle:'%A' precision:'%A'" dimStyle precision
        let rc = ds.AngleResolution
        if precision >= 0 then
            ds.AngleResolution <- precision
            if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleAnglePrecision failed.  dimStyle:'%A' precision:'%A'" dimStyle precision
            Doc.Views.Redraw()
        
    (*
    def DimStyleAnglePrecision(dimStyle, precision=None):
        '''Returns or changes the angle display precision of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          precision (number, optional): the new angle precision value. If omitted, the current angle
            precision is returned
        Returns:
          number: If a precision is not specified, the current angle precision
          number: If a precision is specified, the previous angle precision
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.AngleResolution
        if precision is not None:
            ds.AngleResolution = precision
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the arrow size of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current arrow size</returns>
    static member DimStyleArrowSize(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleArrowSize failed.  dimStyle:'%A'" dimStyle
        ds.ArrowLength
    (*
    def DimStyleArrowSize(dimStyle, size=None):
        '''Returns or changes the arrow size of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          size (number, optional): the new arrow size. If omitted, the current arrow size is returned
        Returns:
          number: If size is not specified, the current arrow size
          number: If size is specified, the previous arrow size
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.ArrowLength
        if size is not None:
            ds.ArrowLength = size
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the arrow size of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="size">(float)The new arrow size.</param>
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
       
    (*
    def DimStyleArrowSize(dimStyle, size=None):
        '''Returns or changes the arrow size of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          size (number, optional): the new arrow size. If omitted, the current arrow size is returned
        Returns:
          number: If size is not specified, the current arrow size
          number: If size is specified, the previous arrow size
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.ArrowLength
        if size is not None:
            ds.ArrowLength = size
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the number of dimension styles in the document</summary>
    ///<returns>(int) the number of dimension styles in the document</returns>
    static member DimStyleCount() : int =
        Doc.DimStyles.Count
    (*
    def DimStyleCount():
        '''Returns the number of dimension styles in the document
        Returns:
          number: the number of dimension styles in the document
        '''
        return scriptcontext.doc.DimStyles.Count
    *)


    [<EXT>]  
    ///<summary>Returns the extension line extension of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current extension line extension</returns>
    static member DimStyleExtension(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleExtension failed.  dimStyle:'%A'" dimStyle
        ds.ExtensionLineExtension
    (*
    def DimStyleExtension(dimStyle, extension=None):
        '''Returns or changes the extension line extension of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          extension (number, optional): the new extension line extension
        Returns:
          number: if extension is not specified, the current extension line extension
          number: if extension is specified, the previous extension line extension
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.ExtensionLineExtension
        if extension is not None:
            ds.ExtensionLineExtension = extension
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the extension line extension of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="extension">(float)The new extension line extension</param>
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
       
    (*
    def DimStyleExtension(dimStyle, extension=None):
        '''Returns or changes the extension line extension of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          extension (number, optional): the new extension line extension
        Returns:
          number: if extension is not specified, the current extension line extension
          number: if extension is specified, the previous extension line extension
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.ExtensionLineExtension
        if extension is not None:
            ds.ExtensionLineExtension = extension
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the font used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(string) The current font</returns>
    static member DimStyleFont(dimStyle:string) : string = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleFont failed.  dimStyle:'%A'" dimStyle
        ds.Font.FaceName
    (*
    def DimStyleFont(dimStyle, font=None):
        '''Returns or changes the font used by a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          font (str, optional): the new font face name
        Returns:
          str: if font is not specified, the current font if successful
          str: if font is specified, the previous font if successful
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.Font.FaceName
        if font:
            newindex = scriptcontext.doc.Fonts.FindOrCreate(font, False, False)
            ds.Font = scriptcontext.doc.Fonts[newindex]
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Changes the font used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="font">(string)The new font face name</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleFont(dimStyle:string, font:string) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleFont failed.  dimStyle:'%A' font:'%A'" dimStyle font
        
        ds.Font <- DocObjects.Font(font) // TODO check if works OK !
        // let newindex = Doc.Fonts.FindOrCreate(font, false, false) // deprecated ??
        // ds.Font <- Doc.Fonts.[newindex]
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then  failwithf "set DimStyleFont failed.  dimStyle:'%A' font:'%A'" dimStyle font
        Doc.Views.Redraw()
        
    (*
    def DimStyleFont(dimStyle, font=None):
        '''Returns or changes the font used by a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          font (str, optional): the new font face name
        Returns:
          str: if font is not specified, the current font if successful
          str: if font is specified, the previous font if successful
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.Font.FaceName
        if font:
            newindex = scriptcontext.doc.Fonts.FindOrCreate(font, False, False)
            ds.Font = scriptcontext.doc.Fonts[newindex]
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Gets all Available Font Face Names</summary>    
    ///<returns>(string array) array of all available font names</returns>
    static member DimStyleAvailableFonts() :string[] = // not part of original rhinoscriptsyntax
        DocObjects.Font.AvailableFontFaceNames()
        


    [<EXT>]  
    ///<summary>Returns the leader arrow size of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current leader arrow size</returns>
    static member DimStyleLeaderArrowSize(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleLeaderArrowSize failed.  dimStyle:'%A'" dimStyle
        ds.LeaderArrowLength
    (*
    def DimStyleLeaderArrowSize(dimStyle, size=None):
        '''Returns or changes the leader arrow size of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          size (number, optional) the new leader arrow size
        Returns:
          number: if size is not specified, the current leader arrow size
          number: if size is specified, the previous leader arrow size
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.LeaderArrowLength
        if size is not None:
            ds.LeaderArrowLength = size
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the leader arrow size of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="size">(float)The new leader arrow size</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleLeaderArrowSize(dimStyle:string, size:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleLeaderArrowSize failed.  dimStyle:'%A' size:'%A'" dimStyle size
        if size > 0.0 then
            ds.LeaderArrowLength <- size
            if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleLeaderArrowSize failed.  dimStyle:'%A' size:'%A'" dimStyle size
            Doc.Views.Redraw()
       
    (*
    def DimStyleLeaderArrowSize(dimStyle, size=None):
        '''Returns or changes the leader arrow size of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          size (number, optional) the new leader arrow size
        Returns:
          number: if size is not specified, the current leader arrow size
          number: if size is specified, the previous leader arrow size
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.LeaderArrowLength
        if size is not None:
            ds.LeaderArrowLength = size
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) if factor is not defined, the current length factor</returns>
    static member DimStyleLengthFactor(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleLengthFactor failed.  dimStyle:'%A'" dimStyle
        ds.LengthFactor
    (*
    def DimStyleLengthFactor(dimStyle, factor=None):
        '''Returns or changes the length factor of a dimension style. Length factor
        is the conversion between Rhino units and dimension units
        Parameters:
          dimStyle (str): the name of an existing dimension style
          factor (number, optional): the new length factor
        Returns:
          number: if factor is not defined, the current length factor
          number: if factor is defined, the previous length factor
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.LengthFactor
        if factor is not None:
            ds.LengthFactor = factor
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="factor">(float)The new length factor</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleLengthFactor(dimStyle:string, factor:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleLengthFactor failed.  dimStyle:'%A' factor:'%A'" dimStyle factor
        ds.LengthFactor <- factor
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleLengthFactor failed.  dimStyle:'%A' factor:'%A'" dimStyle factor
        Doc.Views.Redraw()
        
    (*
    def DimStyleLengthFactor(dimStyle, factor=None):
        '''Returns or changes the length factor of a dimension style. Length factor
        is the conversion between Rhino units and dimension units
        Parameters:
          dimStyle (str): the name of an existing dimension style
          factor (number, optional): the new length factor
        Returns:
          number: if factor is not defined, the current length factor
          number: if factor is defined, the previous length factor
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.LengthFactor
        if factor is not None:
            ds.LengthFactor = factor
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the linear display precision of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current linear precision value</returns>
    static member DimStyleLinearPrecision(dimStyle:string) : int = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleLinearPrecision failed.  dimStyle:'%A'" dimStyle
        ds.LengthResolution
    (*
    def DimStyleLinearPrecision(dimStyle, precision=None):
        '''Returns or changes the linear display precision of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          precision (number, optional): the new linear precision value
        Returns:
          number: if precision is not specified, the current linear precision value
          number: if precision is specified, the previous linear precision value
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.LengthResolution
        if precision is not None:
            ds.LengthResolution = precision
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the linear display precision of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="precision">(int)The new linear precision value</param>
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
        
    (*
    def DimStyleLinearPrecision(dimStyle, precision=None):
        '''Returns or changes the linear display precision of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          precision (number, optional): the new linear precision value
        Returns:
          number: if precision is not specified, the current linear precision value
          number: if precision is specified, the previous linear precision value
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.LengthResolution
        if precision is not None:
            ds.LengthResolution = precision
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the names of all dimension styles in the document</summary>
    ///<returns>(string []) the names of all dimension styles in the document</returns>
    static member DimStyleNames() : string [] =
        [| for ds in Doc.DimStyles -> ds.Name |]
    (*
    def dimStyleNames(sort=False):
        '''Returns the names of all dimension styles in the document
        Parameters:
          sort (bool): sort the list if True, not sorting is the default (False)
        Returns:
          list(str, ...): the names of all dimension styles in the document
        '''
        rc = [ds.Name for ds in scriptcontext.doc.DimStyles]
        if sort: rc.sort()
        return rc
    *)


    // [<EXT>]  
    ///<summary>Returns the number display format of a dimension style</summary>
    // ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    // ///<returns>(float) The current display format
    // ///  0 = Decimal
    // ///  1 = Fractional
    // ///  2 = Feet and inches</returns>
    // static member DimStyleNumberFormat(dimStyle:string) : int = //GET
    //     let ds = Doc.DimStyles.FindName(dimStyle)
    //     if isNull ds then  failwithf "get DimStyleNumberFormat failed.  dimStyle:'%A'" dimStyle
    //     int(ds.LengthFormat) FIXME
    (*
    def DimStyleNumberFormat(dimStyle, format=None):
        '''Returns or changes the number display format of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          format (number, optional) the new number format
             0 = Decimal
             1 = Fractional
             2 = Feet and inches
        Returns:
          number: if format is not specified, the current display format
          number: if format is specified, the previous display format
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = int(ds.LengthFormat)
        if format is not None:
            if format==0: ds.LengthFormat = Rhino.DocObjects.DistanceDisplayMode.Decimal
            if format==1: ds.LengthFormat = Rhino.DocObjects.DistanceDisplayMode.Feet
            if format==2: ds.LengthFormat = Rhino.DocObjects.DistanceDisplayMode.FeetAndInches
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    // [<EXT>]  
    ///<summary>Changes the number display format of a dimension style</summary>
    // ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    // ///<param name="format">(int)The new number format
    // ///  0 = Decimal
    // ///  1 = Fractional
    // ///  2 = Feet and inches</param>
    // ///<returns>(unit) void, nothing</returns>
    // static member DimStyleNumberFormat(dimStyle:string, format:int) : unit = //SET
    //     let ds = Doc.DimStyles.FindName(dimStyle)
    //     if isNull ds then  failwithf "set DimStyleNumberFormat failed.  dimStyle:'%A' format:'%A'" dimStyle format
    //     if   format=0 then  ds.LengthFormat <- DocObjects.DistanceDisplayMode.Decimal
    //     elif format=1 then  ds.LengthFormat <- DocObjects.DistanceDisplayMode.Feet
    //     elif format=2 then  ds.LengthFormat <- DocObjects.DistanceDisplayMode.FeetAndInches
    //     else failwithf "set DimStyleNumberFormat failed.  dimStyle:'%A' format:'%A'" dimStyle format
    //     if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleNumberFormat failed.  dimStyle:'%A' format:'%A'" dimStyle format
    //     Doc.Views.Redraw()        
    (*
    def DimStyleNumberFormat(dimStyle, format=None):
        '''Returns or changes the number display format of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          format (number, optional) the new number format
             0 = Decimal
             1 = Fractional
             2 = Feet and inches
        Returns:
          number: if format is not specified, the current display format
          number: if format is specified, the previous display format
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = float(ds.LengthFormat)
        if format is not None:
            if format==0: ds.LengthFormat = Rhino.DocObjects.DistanceDisplayMode.Decimal
            if format==1: ds.LengthFormat = Rhino.DocObjects.DistanceDisplayMode.Feet
            if format==2: ds.LengthFormat = Rhino.DocObjects.DistanceDisplayMode.FeetAndInches
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the extension line offset of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current extension line offset</returns>
    static member DimStyleOffset(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleOffset failed.  dimStyle:'%A'" dimStyle
        ds.ExtensionLineOffset
    (*
    def DimStyleOffset(dimStyle, offset=None):
        '''Returns or changes the extension line offset of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          offset (number, optional): the new extension line offset
        Returns:
          number: if offset is not specified, the current extension line offset
          number: if offset is specified, the previous extension line offset
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.ExtensionLineOffset
        if offset is not None:
            ds.ExtensionLineOffset = offset
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the extension line offset of a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="offset">(float)The new extension line offset</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleOffset(dimStyle:string, offset:float) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleOffset failed.  dimStyle:'%A' offset:'%A'" dimStyle offset
        ds.ExtensionLineOffset <- offset
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleOffset failed.  dimStyle:'%A' offset:'%A'" dimStyle offset
        Doc.Views.Redraw()
   
    (*
    def DimStyleOffset(dimStyle, offset=None):
        '''Returns or changes the extension line offset of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          offset (number, optional): the new extension line offset
        Returns:
          number: if offset is not specified, the current extension line offset
          number: if offset is specified, the previous extension line offset
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.ExtensionLineOffset
        if offset is not None:
            ds.ExtensionLineOffset = offset
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the prefix of a dimension style - the text to
    /// prefix to the dimension text.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<returns>(string) The current prefix</returns>
    static member DimStylePrefix(dimStyle:string) : string = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStylePrefix failed.  dimStyle:'%A'" dimStyle
        ds.Prefix
    (*
    def DimStylePrefix(dimStyle, prefix=None):
        '''Returns or changes the prefix of a dimension style - the text to
        prefix to the dimension text.
        Parameters:
          dimStyle (str): the name of an existing dimStyle
          prefix (str, optional): the new prefix
        Returns:
          str: if prefix is not specified, the current prefix
          str: if prefix is specified, the previous prefix
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.Prefix
        if prefix is not None:
            ds.Prefix = prefix
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the prefix of a dimension style - the text to
    /// prefix to the dimension text.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<param name="prefix">(string)The new prefix</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStylePrefix(dimStyle:string, prefix:string) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStylePrefix failed.  dimStyle:'%A' prefix:'%A'" dimStyle prefix
        ds.Prefix <- prefix
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStylePrefix failed.  dimStyle:'%A' prefix:'%A'" dimStyle prefix
        Doc.Views.Redraw()
       
    (*
    def DimStylePrefix(dimStyle, prefix=None):
        '''Returns or changes the prefix of a dimension style - the text to
        prefix to the dimension text.
        Parameters:
          dimStyle (str): the name of an existing dimStyle
          prefix (str, optional): the new prefix
        Returns:
          str: if prefix is not specified, the current prefix
          str: if prefix is specified, the previous prefix
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.Prefix
        if prefix is not None:
            ds.Prefix = prefix
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the suffix of a dimension style - the text to
    /// append to the dimension text.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<returns>(string) The current suffix</returns>
    static member DimStyleSuffix(dimStyle:string) : string = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleSuffix failed.  dimStyle:'%A'" dimStyle
        ds.Suffix
    (*
    def DimStyleSuffix(dimStyle, suffix=None):
        '''Returns or changes the suffix of a dimension style - the text to
        append to the dimension text.
        Parameters:
          dimStyle (str): the name of an existing dimStyle
          suffix (str, optional): the new suffix
        Returns:
          str: if suffix is not specified, the current suffix
          str: if suffix is specified, the previous suffix
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.Suffix
        if suffix is not None:
            ds.Suffix = suffix
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the suffix of a dimension style - the text to
    /// append to the dimension text.</summary>
    ///<param name="dimStyle">(string) The name of an existing dimStyle</param>
    ///<param name="suffix">(string)The new suffix</param>
    ///<returns>(unit) void, nothing</returns>
    static member DimStyleSuffix(dimStyle:string, suffix:string) : unit = //SET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "set DimStyleSuffix failed.  dimStyle:'%A' suffix:'%A'" dimStyle suffix
        ds.Suffix <- suffix
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleSuffix failed.  dimStyle:'%A' suffix:'%A'" dimStyle suffix
        Doc.Views.Redraw()
        
    (*
    def DimStyleSuffix(dimStyle, suffix=None):
        '''Returns or changes the suffix of a dimension style - the text to
        append to the dimension text.
        Parameters:
          dimStyle (str): the name of an existing dimStyle
          suffix (str, optional): the new suffix
        Returns:
          str: if suffix is not specified, the current suffix
          str: if suffix is specified, the previous suffix
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.Suffix
        if suffix is not None:
            ds.Suffix = suffix
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    // [<EXT>]  
    ///<summary>Returns the text alignment mode of a dimension style</summary>
    // ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    // ///<returns>(int) The current text alignment
    // ///  0 = Normal (same as 2)
    // ///  1 = Horizontal to view
    // ///  2 = Above the dimension line
    // ///  3 = In the dimension line</returns>
    // static member DimStyleTextAlignment(dimStyle:string) : int = //GET
    //     let ds = Doc.DimStyles.FindName(dimStyle)
    //     if isNull ds then  failwithf "get DimStyleTextAlignment failed.  dimStyle:'%A'" dimStyle
    //     int(ds.TextAlignment) //FIXME
    (*
    def DimStyleTextAlignment(dimStyle, alignment=None):
        '''Returns or changes the text alignment mode of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          alignment (number, optional): the new text alignment
              0 = Normal (same as 2)
              1 = Horizontal to view
              2 = Above the dimension line
              3 = In the dimension line
        Returns:
          number: if alignment is not specified, the current text alignment
          number: if alignment is specified, the previous text alignment
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = int(ds.TextAlignment)
        if alignment is not None:
            if alignment==0: ds.TextAlignment = Rhino.DocObjects.TextDisplayAlignment.Normal
            if alignment==1: ds.TextAlignment = Rhino.DocObjects.TextDisplayAlignment.Horizontal
            if alignment==2: ds.TextAlignment = Rhino.DocObjects.TextDisplayAlignment.AboveLine
            if alignment==3: ds.TextAlignment = Rhino.DocObjects.TextDisplayAlignment.InLine
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    // [<EXT>]  
    ///<summary>Changes the text alignment mode of a dimension style</summary>
    // ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    // ///<param name="alignment">(int)The new text alignment
    // ///  0 = Normal (same as 2)
    // ///  1 = Horizontal to view
    // ///  2 = Above the dimension line
    // ///  3 = In the dimension line</param>
    // ///<returns>(unit) void, nothing</returns>
    // static member DimStyleTextAlignment(dimStyle:string, alignment:int) : unit = //SET
    //     let ds = Doc.DimStyles.FindName(dimStyle)
    //     if isNull ds then  failwithf "set DimStyleTextAlignment failed.  dimStyle:'%A' alignment:'%A'" dimStyle alignment
    //     elif alignment=0 then  ds.TextAlignment <- DocObjects.TextDisplayAlignment.Normal //FIXME
    //     elif alignment=1 then  ds.TextAlignment <- DocObjects.TextDisplayAlignment.Horizontal
    //     elif alignment=2 then  ds.TextAlignment <- DocObjects.TextDisplayAlignment.AboveLine
    //     elif alignment=3 then  ds.TextAlignment <- DocObjects.TextDisplayAlignment.InLine
    //     else failwithf "set DimStyleTextAlignment failed.  dimStyle:'%A' alignment:'%A'" dimStyle alignment
    //     if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then failwithf "set DimStyleTextAlignment failed.  dimStyle:'%A' alignment:'%A'" dimStyle alignment
    //     Doc.Views.Redraw()        
    (*
    def DimStyleTextAlignment(dimStyle, alignment=None):
        '''Returns or changes the text alignment mode of a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          alignment (number, optional): the new text alignment
              0 = Normal (same as 2)
              1 = Horizontal to view
              2 = Above the dimension line
              3 = In the dimension line
        Returns:
          number: if alignment is not specified, the current text alignment
          number: if alignment is specified, the previous text alignment
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = float(ds.TextAlignment)
        if alignment is not None:
            if alignment==0: ds.TextAlignment = Rhino.DocObjects.TextDisplayAlignment.Normal
            if alignment==1: ds.TextAlignment = Rhino.DocObjects.TextDisplayAlignment.Horizontal
            if alignment==2: ds.TextAlignment = Rhino.DocObjects.TextDisplayAlignment.AboveLine
            if alignment==3: ds.TextAlignment = Rhino.DocObjects.TextDisplayAlignment.InLine
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the text gap used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current text gap</returns>
    static member DimStyleTextGap(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleTextGap failed.  dimStyle:'%A'" dimStyle
        ds.TextGap
    (*
    def DimStyleTextGap(dimStyle, gap=None):
        '''Returns or changes the text gap used by a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          gap (number, optional): the new text gap
        Returns:
          number: if gap is not specified, the current text gap
          number: if gap is specified, the previous text gap
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.TextGap
        if gap is not None:
            ds.TextGap = gap
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the text gap used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="gap">(float)The new text gap</param>
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
     
    (*
    def DimStyleTextGap(dimStyle, gap=None):
        '''Returns or changes the text gap used by a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          gap (number, optional): the new text gap
        Returns:
          number: if gap is not specified, the current text gap
          number: if gap is specified, the previous text gap
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.TextGap
        if gap is not None:
            ds.TextGap = gap
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Returns the text height used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(float) The current text height</returns>
    static member DimStyleTextHeight(dimStyle:string) : float = //GET
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then  failwithf "get DimStyleTextHeight failed.  dimStyle:'%A'" dimStyle
        ds.TextHeight
    (*
    def DimStyleTextHeight(dimStyle, height=None):
        '''Returns or changes the text height used by a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          height (number, optional): the new text height
        Returns:
          number: if height is not specified, the current text height
          number: if height is specified, the previous text height
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.TextHeight
        if height:
            ds.TextHeight = height
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Changes the text height used by a dimension style</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<param name="height">(float)The new text height</param>
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
    (*
    def DimStyleTextHeight(dimStyle, height=None):
        '''Returns or changes the text height used by a dimension style
        Parameters:
          dimStyle (str): the name of an existing dimension style
          height (number, optional): the new text height
        Returns:
          number: if height is not specified, the current text height
          number: if height is specified, the previous text height
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        rc = ds.TextHeight
        if height:
            ds.TextHeight = height
            scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Verifies an object is an aligned dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsAlignedDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
            match g with
            | :? LinearDimension as g -> g.Aligned
            | _ -> false
    (*
    def IsAlignedDimension(object_id):
        '''Verifies an object is an aligned dimension object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True or False.  None on error
        '''
        annotation_object = __Coerceannotation(object_id)
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        if isinstance(geom, Rhino.Geometry.LinearDimension): return geom.Aligned
        return False
    *)


    [<EXT>]  
    ///<summary>Verifies an object is an angular dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsAngularDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
            match g with
            | :? AngularDimension -> true
            | _ -> false
    (*
    def IsAngularDimension(object_id):
        '''Verifies an object is an angular dimension object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True or False.
          None: on error
        '''
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        return isinstance(geom, Rhino.Geometry.AngularDimension)
    *)


    [<EXT>]  
    ///<summary>Verifies an object is a diameter dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDiameterDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
            match g with
            | :? RadialDimension as g -> g.IsDiameterDimension
            | _ -> false
    (*
    def IsDiameterDimension(object_id):
        '''Verifies an object is a diameter dimension object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True or False.
          None: on error
        '''
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        if isinstance(geom, Rhino.Geometry.RadialDimension):
            return geom.IsDiameterDimension
        return False
    *)


    [<EXT>]  
    ///<summary>Verifies an object is a dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
            match g with
            | :? AnnotationBase  -> true
            | _ -> false
    (*
    def IsDimension(object_id):
        '''Verifies an object is a dimension object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True or False.
          None: on error
        '''
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        return isinstance(geom, Rhino.Geometry.AnnotationBase)
    *)


    [<EXT>]  
    ///<summary>Verifies the existance of a dimension style in the document</summary>
    ///<param name="dimStyle">(string) The name of a dimStyle to test for</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDimStyle(dimStyle:string) : bool =
        let ds = Doc.DimStyles.FindName(dimStyle)
        notNull ds
    (*
    def IsDimStyle(dimStyle):
        '''Verifies the existance of a dimension style in the document
        Parameters:
          dimStyle (str): the name of a dimStyle to test for
        Returns:
          bool: True or False.
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        return ds is not None
    *)


    [<EXT>]  
    ///<summary>Verifies that an existing dimension style is from a reference file</summary>
    ///<param name="dimStyle">(string) The name of an existing dimension style</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDimStyleReference(dimStyle:string) : bool =
        let ds = Doc.DimStyles.FindName(dimStyle)
        if isNull ds then false
        else ds.IsReference
    (*
    def IsDimStyleReference(dimStyle):
        '''Verifies that an existing dimension style is from a reference file
        Parameters:
          dimStyle (str): the name of an existing dimension style
        Returns:
          bool: True or False.
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(dimStyle)
        if ds is None: return scriptcontext.errorhandler()
        return ds.IsReference
    *)


    [<EXT>]  
    ///<summary>Verifies an object is a dimension leader object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLeader(objectId:Guid) : bool =
         match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
         | None -> false
         | Some g ->
            match g with
            | :? Leader  -> true
            | _ -> false
    (*
    def IsLeader(object_id):
        '''Verifies an object is a dimension leader object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True or False.
          None: on error
        '''
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        return isinstance(geom, Rhino.Geometry.Leader)
    *)


    [<EXT>]  
    ///<summary>Verifies an object is a linear dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLinearDimension(objectId:Guid) : bool =
         match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
         | None -> false
         | Some g ->
            match g with
            | :? LinearDimension  -> true
            | _ -> false
    (*
    def IsLinearDimension(object_id):
        '''Verifies an object is a linear dimension object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True or False.
          None: on error
        '''
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        return isinstance(geom, Rhino.Geometry.LinearDimension)
    *)


    [<EXT>]  
    ///<summary>Verifies an object is an ordinate dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsOrdinateDimension(objectId:Guid) : bool =
         match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
         | None -> false
         | Some g ->
            match g with
            | :? OrdinateDimension  -> true
            | _ -> false
    (*
    def IsOrdinateDimension(object_id):
        '''Verifies an object is an ordinate dimension object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True or False.
          None: on error
        '''
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        return isinstance(geom, Rhino.Geometry.OrdinateDimension)
    *)


    [<EXT>]  
    ///<summary>Verifies an object is a radial dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsRadialDimension(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> false
        | Some g ->
           match g with
           | :? RadialDimension  -> true
           | _ -> false
    (*
    def IsRadialDimension(object_id):
        '''Verifies an object is a radial dimension object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True or False.
          None: on error
        '''
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        return isinstance(geom, Rhino.Geometry.RadialDimension)
    *)


    [<EXT>]  
    ///<summary>Returns the text string of a dimension leader object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(string) The current text string</returns>
    static member LeaderText(objectId:Guid) : string = //GET
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> failwithf "getLeaderText failed.  objectId:'%A'" objectId
        | Some g ->
            match g with
            | :? Leader as g ->
                let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
                annotationObject.DisplayText
            | _ -> failwithf "getLeaderText failed.  objectId:'%A'" objectId
    (*
    def LeaderText(object_id, text=None):
        '''Returns or modifies the text string of a dimension leader object
        Parameters:
          object_id (guid): the object's identifier
          text (string, optional): the new text string
        Returns:
          str: if text is not specified, the current text string
          str: if text is specified, the previous text string
          None on error
        '''
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        if not isinstance(geom, Rhino.Geometry.Leader):
            return scriptcontext.errorhandler()
        rc = annotation_object.DisplayText
        if text is not None:
            geom.RichText = text
            annotation_object.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]  
    ///<summary>Modifies the text string of a dimension leader object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="text">(string)The new text string</param>
    ///<returns>(unit) void, nothing</returns>
    static member LeaderText(objectId:Guid, text:string) : unit = //SET
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | None -> failwithf "getLeaderText failed.  objectId:'%A'" objectId
        | Some g ->
            match g with
            | :? Leader as g ->
                let annotationObject = RhinoScriptSyntax.CoerceAnnotation(objectId)
                g.PlainText <- text               // TODO or use rich text?
                annotationObject.CommitChanges()|> ignore
                Doc.Views.Redraw()                
            | _ -> failwithf "getLeaderText failed.  objectId:'%A'" objectId
    (*
    def LeaderText(object_id, text=None):
        '''Returns or modifies the text string of a dimension leader object
        Parameters:
          object_id (guid): the object's identifier
          text (string, optional): the new text string
        Returns:
          str: if text is not specified, the current text string
          str: if text is specified, the previous text string
          None on error
        '''
        objectId = rhutil.Coerceguid(object_id, True)
        annotation_object = scriptcontext.doc.Objects.FindId(objectId)
        geom = annotation_object.Geometry
        if not isinstance(geom, Rhino.Geometry.Leader):
            return scriptcontext.errorhandler()
        rc = annotation_object.DisplayText
        if text is not None:
            geom.RichText = text
            annotation_object.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]  
    ///<summary>Renames an existing dimension style</summary>
    ///<param name="oldstyle">(string) The name of an existing dimension style</param>
    ///<param name="newstyle">(string) The new dimension style name</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenameDimStyle(oldstyle:string, newstyle:string) : unit =
        let mutable ds = Doc.DimStyles.FindName(oldstyle)
        if isNull ds then  failwithf "renameDimStyle failed.  oldstyle:'%A' newstyle:'%A'" oldstyle newstyle
        ds.Name <- newstyle
        if not <| Doc.DimStyles.Modify(ds, ds.Id, false) then  failwithf "renameDimStyle failed.  oldstyle:'%A' newstyle:'%A'" oldstyle newstyle
    (*
    def RenameDimStyle(oldstyle, newstyle):
        '''Renames an existing dimension style
        Parameters:
          oldstyle (str): the name of an existing dimension style
          newstyle (str): the new dimension style name
        Returns:
          str: the new dimension style name if successful
          None: on error
        '''
        ds = scriptcontext.doc.DimStyles.FindName(oldstyle)
        if not ds: return scriptcontext.errorhandler()
        ds.Name = newstyle
        if scriptcontext.doc.DimStyles.Modify(ds, ds.Id, False): return newstyle
        return None
    *)


