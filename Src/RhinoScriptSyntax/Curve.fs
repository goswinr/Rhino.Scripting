namespace Rhino.Scripting


open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.UtilMath

open Microsoft.FSharp.Core.LanguagePrimitives
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsCurve =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    [<Extension>]
    ///<summary>Adds an arc curve to the document</summary>
    ///<param name="plane">(Plane) Plane on which the arc will lie. The origin of the plane will be
    ///    the center point of the arc. x-axis of the plane defines the 0 angle
    ///    direction</param>
    ///<param name="radius">(float) Radius of the arc</param>
    ///<param name="angleDegrees">(float) Interval of arc in degrees</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddArc(plane:Plane, radius:float, angleDegrees:float) : Guid =
        let radians = toRadians(angleDegrees)
        let arc = Arc(plane, radius, radians)
        let rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddArc: Unable to add arc to document.  plane:'%A' radius:'%A' angleDegrees:'%A'" plane radius angleDegrees
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a 3-point arc curve to the document</summary>
    ///<param name="start">(Point3d) Start of the arc</param>
    ///<param name="ende">(Point3d) Endpoint of the arc</param>
    ///<param name="pointOnArc">(Point3d) A point on the arc</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddArc3Pt(start:Point3d, ende:Point3d, pointOnArc:Point3d) : Guid =
        let arc = Arc(start, pointOnArc, ende)
        let rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddArc3Pt: Unable to add arc to document.  start:'%A' ende:'%A' pointOnArc:'%A'" start ende pointOnArc
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds an arc curve, created from a start point, a start direction, and an
    ///    end point, to the document</summary>
    ///<param name="start">(Point3d) The starting point of the arc</param>
    ///<param name="direction">(Vector3d) The arc direction at start</param>
    ///<param name="ende">(Point3d) The ending point of the arc</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddArcPtTanPt(start:Point3d, direction:Vector3d, ende:Point3d) : Guid =
        let arc = Arc(start, direction, ende)
        let rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddArcPtTanPt: Unable to add arc to document.  start:'%A' direction:'%A' ende:'%A'" start direction ende
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Makes a curve blend between two curves</summary>
    ///<param name="curves">(Guid * Guid) List of two curves</param>
    ///<param name="parameters">(float * float) List of two curve parameters defining the blend end points</param>
    ///<param name="reverses">(bool * bool) List of two boolean values specifying to use the natural or opposite direction of the curve</param>
    ///<param name="continuities">(int * int) List of two numbers specifying continuity at end points
    ///    0 = position
    ///    1 = tangency
    ///    2 = curvature</param>
    ///<returns>(Guid) identifier of new curve</returns>
    static member AddBlendCurve(curves:Guid * Guid, parameters:float * float, reverses:bool * bool, continuities:int * int) : Guid =
        let crv0 = RhinoScriptSyntax.CoerceCurve (fst curves)
        let crv1 = RhinoScriptSyntax.CoerceCurve (snd curves)
        let c0:BlendContinuity = EnumOfValue(fst continuities)
        let c1:BlendContinuity = EnumOfValue(snd continuities)
        let curve = Curve.CreateBlendCurve(crv0, fst parameters, fst reverses, c0, crv1, snd parameters, snd reverses, c1)
        let rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then  Error.Raise <| sprintf "RhinoScriptSyntax.AddBlendCurve: Unable to add curve to document.  curves:'%A' parameters:'%A' reverses:'%A' continuities:'%A'" curves parameters reverses continuities
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a circle curve to the document</summary>
    ///<param name="plane">(Plane) Plane on which the circle will lie. If a point is
    ///    passed, this will be the center of the circle on the active construction plane</param>
    ///<param name="radius">(float) The radius of the circle</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddCircle(plane:Plane, radius:float) : Guid = //(TODO add overload point)
        let circle = Circle(plane, radius)
        let rc = Doc.Objects.AddCircle(circle)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddCircle: Unable to add circle to document.  planeOrCenter:'%A' radius:'%A'" plane radius
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a 3-point circle curve to the document</summary>
    ///<param name="first">(Point3d) First point on the circle'</param>
    ///<param name="second">(Point3d) Second point on the circle'</param>
    ///<param name="third">(Point3d) Third point on the circle'</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddCircle3Pt(first:Point3d, second:Point3d, third:Point3d) : Guid =
        let circle = Circle(first, second, third)
        let rc = Doc.Objects.AddCircle(circle)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddCircle3Pt: Unable to add circle to document.  first:'%A' second:'%A' third:'%A'" first second third
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a control points curve object to the document</summary>
    ///<param name="points">(Point3d seq) A list of points</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///    Degree of the curve</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddCurve(points:Point3d seq, [<OPT;DEF(3)>]degree:int) : Guid =
        let  curve = Curve.CreateControlPointCurve(points, degree)
        if isNull curve then Error.Raise <| sprintf "RhinoScriptSyntax.AddCurve: Unable to create control point curve from given points.  points:'%A' degree:'%A'" points degree
        let  rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddCurve: Unable to add curve to document.  points:'%A' degree:'%A'" points degree
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds an elliptical curve to the document</summary>
    ///<param name="plane">(Plane) The plane on which the ellipse will lie. The origin of
    ///    the plane will be the center of the ellipse</param>
    ///<param name="radiusX">(float) radius in the X axis direction</param>
    ///<param name="radiusY">(float) radius in the Y axis direction</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddEllipse(plane:Plane, radiusX:float, radiusY:float) : Guid =
        let ellipse = Ellipse(plane, radiusX, radiusY)
        let rc = Doc.Objects.AddEllipse(ellipse)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddEllipse: Unable to add curve to document. plane:'%A' radiusX:'%A' radiusY:'%A'" plane radiusX radiusY
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a 3-point elliptical curve to the document</summary>
    ///<param name="center">(Point3d) Center point of the ellipse</param>
    ///<param name="second">(Point3d) End point of the x axis</param>
    ///<param name="third">(Point3d) End point of the y axis</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddEllipse3Pt(center:Point3d, second:Point3d, third:Point3d) : Guid =
        let  ellipse = Ellipse(center, second, third)
        let  rc = Doc.Objects.AddEllipse(ellipse)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddEllipse3Pt: Unable to add curve to document.  center:'%A' second:'%A' third:'%A'" center second third
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a fillet curve between two curve objects</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object</param>
    ///<param name="radius">(float) Optional, Default Value: <c>1.0</c>
    ///    Fillet radius</param>
    ///<param name="basePointA">(Point3d) Optional, Base point of the first curve. If omitted,
    ///    starting point of the curve is used</param>
    ///<param name="basePointB">(Point3d) Optional, Base point of the second curve. If omitted,
    ///    starting point of the curve is used</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddFilletCurve(curveA:Guid, curveB:Guid, [<OPT;DEF(1.0)>]radius:float, [<OPT;DEF(Point3d())>]basePointA:Point3d, [<OPT;DEF(Point3d())>]basePointB:Point3d) : Guid = 
        //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let basePointA = if basePointA = Point3d.Origin then Point3d.Unset else basePointA
        let basePointB = if basePointB = Point3d.Origin then Point3d.Unset else basePointB
        let  curve0 = RhinoScriptSyntax.CoerceCurve (curveA)
        let  curve1 = RhinoScriptSyntax.CoerceCurve (curveB)
        let mutable crv0T = 0.0
        if basePointA= Point3d.Unset then
            crv0T <- curve0.Domain.Min
        else
            let rc, t = curve0.ClosestPoint(basePointA)
            if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.AddFilletCurve ClosestPoint failed.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
            crv0T <- t
        let mutable crv1T = 0.0
        if basePointB= Point3d.Unset then
            crv1T <- curve1.Domain.Min
        else
            let rc, t = curve1.ClosestPoint(basePointB)
            if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.AddFilletCurve ClosestPoint failed.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
            crv1T <- t
        let mutable arc = Curve.CreateFillet(curve0, curve1, radius, crv0T, crv1T)
        let mutable rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddFilletCurve: Unable to add fillet curve to document.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds an interpolated curve object that lies on a specified
    ///    surface.  Note, this function will not create periodic curves,
    ///    but it will create closed curves</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to create the curve on</param>
    ///<param name="points">(Point3d seq) List of 3D points that lie on the specified surface.
    ///    The list must contain at least 2 points</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddInterpCrvOnSrf(surfaceId:Guid, points:Point3d seq) : Guid =
        let  surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let  tolerance = Doc.ModelAbsoluteTolerance
        let  curve = surface.InterpolatedCurveOnSurface(points, tolerance)
        if isNull curve then Error.Raise <| sprintf "RhinoScriptSyntax.AddInterpCrvOnSrf: Unable to create InterpolatedCurveOnSurface.  surfaceId:'%A' points:'%A'" surfaceId points
        let mutable rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddInterpCrvOnSrf: Unable to add curve to document.  surfaceId:'%A' points:'%A'" surfaceId points
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds an interpolated curve object based on surface parameters,
    ///    that lies on a specified surface. Note, this function will not
    ///    create periodic curves, but it will create closed curves</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to create the curve on</param>
    ///<param name="points">(Point2d seq) A list of 2D surface parameters. The list must contain
    ///    at least 2 sets of parameters</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddInterpCrvOnSrfUV(surfaceId:Guid, points:Point2d seq) : Guid =
        let mutable surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let mutable tolerance = Doc.ModelAbsoluteTolerance
        let mutable curve = surface.InterpolatedCurveOnSurfaceUV(points, tolerance)
        if isNull curve then Error.Raise <| sprintf "RhinoScriptSyntax.AddInterpCrvOnSrfUV: Unable to create InterpolatedCurveOnSurfaceUV.  surfaceId:'%A' points:'%A'" surfaceId points
        let mutable rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddInterpCrvOnSrfUV: Unable to add curve to document.  surfaceId:'%A' points:'%A'" surfaceId points
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds an interpolated curve object to the document. Options exist to make
    ///    a periodic curve or to specify the tangent at the endpoints. The resulting
    ///    curve is a non-rational NURBS curve of the specified degree</summary>
    ///<param name="points">(Point3d seq) A list containing 3D points to interpolate. For periodic curves,
    ///    if the final point is a duplicate of the initial point, it is
    ///    ignored. The number of control points must be bigger than 'degree' number</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>    
    ///    Periodic curves must have a degree bigger than 1. For knotstyle = 1 or 2,
    ///    the degree must be 3. For knotstyle = 4 or 5, the degree must be odd</param>
    ///<param name="knotstyle">(int) Optional, Default Value: <c>0</c>
    ///    0 Uniform knots.  Parameter spacing between consecutive knots is 1.0.
    ///    1 Chord length spacing.  Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///    2 Sqrt (chord length).  Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///    3 Periodic with uniform spacing.
    ///    4 Periodic with chord length spacing.  Requires an odd degree value.
    ///    5 Periodic with sqrt (chord length) spacing.  Requires an odd degree value</param>
    ///<param name="startTangent">(Vector3d) Optional, A vector that specifies a tangency condition at the
    ///    beginning of the curve. If the curve is periodic, this argument must be omitted</param>
    ///<param name="endTangent">(Vector3d) Optional, 3d vector that specifies a tangency condition at the
    ///    end of the curve. If the curve is periodic, this argument must be omitted</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddInterpCurve(   points:Point3d seq, 
                                    [<OPT;DEF(3)>]degree:int, 
                                    [<OPT;DEF(0)>]knotstyle:int, 
                                    [<OPT;DEF(Vector3d())>]startTangent:Vector3d,  //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
                                    [<OPT;DEF(Vector3d())>]endTangent:Vector3d) : Guid =
        let endTangent   = if endTangent.IsZero then Vector3d.Unset else endTangent
        let startTangent = if startTangent.IsZero then Vector3d.Unset else startTangent
        let knotstyle : CurveKnotStyle = EnumOfValue knotstyle
        let  curve = Curve.CreateInterpolatedCurve(points, degree, knotstyle, startTangent, endTangent)
        if isNull curve then Error.Raise <| sprintf "RhinoScriptSyntax.AddInterpCurve: Unable to CreateInterpolatedCurve.  points:'%A' degree:'%A' knotstyle:'%A' startTangent:'%A' endTangent:'%A'" points degree knotstyle startTangent endTangent
        let  rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddInterpCurve: Unable to add curve to document.  points:'%A' degree:'%A' knotstyle:'%A' startTangent:'%A' endeTangent:'%A'" points degree knotstyle startTangent endTangent
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a line curve to the current model</summary>
    ///<param name="start">(Point3d) Startpoint of the line</param>
    ///<param name="ende">(Point3d) Endpoint of the line</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddLine(start:Point3d, ende:Point3d) : Guid =
        let  rc = Doc.Objects.AddLine(start, ende)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddLine: Unable to add line to document.  start:'%A' ende:'%A'" start ende
        Doc.Views.Redraw()
        rc
   
    [<Extension>]
    ///<summary>Creats a NURBS curve geomety, but does not add or draw it to the document</summary>
    ///<param name="points">(Point3d seq) A list containing 3D control points</param>
    ///<param name="knots">(float seq) Knot values for the curve. The number of elements in knots must
    ///    equal the number of elements in points plus degree minus 1</param>
    ///<param name="degree">(int) Degree of the curve. must be greater than of equal to 1</param>
    ///<param name="weights">(float seq) Optional, Weight values for the curve. Number of elements should
    ///    equal the number of elements in points. Values must be greater than 0</param>
    ///<returns>(NurbsCurve) a NurbsCurve geometry</returns>
    static member CreateNurbsCurve(points:Point3d seq, knots:float seq, degree:int, [<OPT;DEF(null: float seq)>]weights:float seq) : NurbsCurve =
        let cvcount = Seq.length(points)
        let knotcount = cvcount + degree - 1        
        if Seq.length(knots)<>knotcount then
            Error.Raise <| sprintf "RhinoScriptSyntax.CreateNurbsCurve:Number of elements in knots must equal the number of elements in points plus degree minus 1.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        let rational =
            if notNull weights then
                if Seq.length(weights)<>cvcount then
                    Error.Raise <| sprintf "RhinoScriptSyntax.CreateNurbsCurve:Number of elements in weights should equal the number of elements in points.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
                true
            else
            false
        let nc = new NurbsCurve(3, rational, degree + 1, cvcount)
        for i, k in Seq.indexed knots do
            nc.Knots.[i] <- k
        if notNull weights then
            if Seq.length(weights)<>cvcount then
                Error.Raise <| sprintf "RhinoScriptSyntax.CreateNurbsCurve:Number of elements in weights should equal the number of elements in points.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
            for i,(p, w)  in Seq.indexed (Seq.zip points weights) do
                nc.Points.SetPoint(i, p, w) |> ignore
        else
            for i, p in Seq.indexed points do
                nc.Points.SetPoint(i, p) |> ignore       
        if not nc.IsValid then Error.Raise <| sprintf "RhinoScriptSyntax.CreateNurbsCurve:Unable to create curve.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        nc

    [<Extension>]
    ///<summary>Adds a NURBS curve object to the document</summary>
    ///<param name="points">(Point3d seq) A list containing 3D control points</param>
    ///<param name="knots">(float seq) Knot values for the curve. The number of elements in knots must
    ///    equal the number of elements in points plus degree minus 1</param>
    ///<param name="degree">(int) Degree of the curve. must be greater than of equal to 1</param>
    ///<param name="weights">(float seq) Optional, Weight values for the curve. Number of elements should
    ///    equal the number of elements in points. Values must be greater than 0</param>
    ///<returns>(Guid) the identifier of the new object</returns>
    static member AddNurbsCurve(points:Point3d seq, knots:float seq, degree:int, [<OPT;DEF(null: float seq)>]weights:float seq) : Guid =
        let nc = RhinoScriptSyntax.CreateNurbsCurve(points, knots, degree, weights)
        let rc = Doc.Objects.AddCurve(nc)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddNurbsCurve: Unable to add curve to document.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a polyline curve</summary>
    ///<param name="points">(Point3d seq) List of 3D points. The list must contain at least two points. If the
    ///    list contains less than four points, then the first point and
    ///    last point must be different</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddPolyline(points:Point3d seq) : Guid =
        let pl = Polyline(points)
        //pl.DeleteShortSegments(Doc.ModelAbsoluteTolerance) |>ignore
        let rc = Doc.Objects.AddPolyline(pl)
        if rc = Guid.Empty then 
            for i,pt in Seq.indexed(points) do
                let d = Doc.Objects.AddTextDot(string i, pt) 
                RhinoScriptSyntax.ObjectLayer(d,"AddPolyline failed",true)
            Error.Raise <| sprintf "RhinoScriptSyntax.AddPolyline: Unable to add polyline to document.  points:'%A' " points
        Doc.Views.Redraw()
        rc
    
    [<Extension>]
    ///<summary>Adds a closed polyline curve , 
    ///    if the endpoint is already closer than Doc.ModelAbsoluteTolerance to the start it wil be set to start point
    ///    else an additional point will be added with the same position as start</summary>
    ///<param name="points">(Point3d seq) List of 3D points. The list must contain at least three points.</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddPolylineClosed(points:Point3d seq) : Guid =
        let pl = Polyline(points)
        if pl.Count < 3 then Error.Raise <| sprintf "RhinoScriptSyntax.AddPolylineClosed: Unable to add closed polyline to document.  points:'%A' " points.ToNiceString        
        if (pl.First-pl.Last).Length <= Doc.ModelAbsoluteTolerance then 
            pl.[pl.Count-1] <- pl.First
        else
            pl.Add pl.First
        //pl.DeleteShortSegments(Doc.ModelAbsoluteTolerance) |>ignore
        let rc = Doc.Objects.AddPolyline(pl)
        if rc = Guid.Empty then 
            for i,pt in Seq.indexed(points) do
                let d = Doc.Objects.AddTextDot(string i, pt) 
                RhinoScriptSyntax.ObjectLayer(d,"AddPolylineClosed failed",true)            
            Error.Raise <| sprintf "RhinoScriptSyntax.AddPolylineClosed: Unable to add closed polyline to document.  points:'%A' " points
        Doc.Views.Redraw()
        rc

    [<Extension>]
    ///<summary>Add a rectangular curve to the document</summary>
    ///<param name="plane">(Plane) Plane on which the rectangle will lie</param>
    ///<param name="width">(float) Width of rectangle as measured along the plane's
    ///    x and y axes</param>
    ///<param name="height">(float) Height of rectangle as measured along the plane's
    ///    x and y axes</param>
    ///<returns>(Guid) objectId of new rectangle</returns>
    static member AddRectangle(plane:Plane, width:float, height:float) : Guid =
        let rect = Rectangle3d(plane, width, height)
        let poly = rect.ToPolyline()
        let rc = Doc.Objects.AddPolyline(poly)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddRectangle: Unable to add polyline to document.  plane:'%A' width:'%A' height:'%A'" plane width height
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a spiral or helical curve to the document</summary>
    ///<param name="point0">(Point3d) Helix axis start point or center of spiral</param>
    ///<param name="point1">(Point3d) Helix axis end point or point normal on spiral plane</param>
    ///<param name="pitch">(float) Distance between turns. If 0, then a spiral. If > 0 then the
    ///    distance between helix "threads"</param>
    ///<param name="turns">(float) Number of turns</param>
    ///<param name="radius0">(float) Starting radius of spiral</param>
    ///<param name="radius1">(float) Optional, Ending radius of spiral. If omitted, the starting radius is used for the complete spiral</param>
    ///<returns>(Guid) objectId of new curve</returns>
    static member AddSpiral(point0:Point3d, point1:Point3d, pitch:float, turns:float, radius0:float, [<OPT;DEF(0.0)>]radius1:float) : Guid =
        let dir = point1 - point0
        let plane = Plane(point0, dir)
        let point2 = point0 + plane.XAxis
        let r2 = if radius1 = 0.0 then radius0 else radius1
        let curve = NurbsCurve.CreateSpiral(point0, dir, point2, pitch, turns, radius0, r2)
        if isNull curve then Error.Raise <| sprintf "RhinoScriptSyntax.AddSpiral: Unable to add curve to document.  point0:'%A' point1:'%A' pitch:'%A' turns:'%A' radius0:'%A' radius1:'%A'" point0 point1 pitch turns radius0 radius1
        let rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddSpiral: Unable to add curve to document.  point0:'%A' point1:'%A' pitch:'%A' turns:'%A' radius0:'%A' radius1:'%A'" point0 point1 pitch turns radius0 radius1
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Add a curve object based on a portion, or interval of an existing curve
    ///    object. Similar in operation to Rhino's SubCrv command</summary>
    ///<param name="curveId">(Guid) Identifier of a closed planar curve object</param>
    ///<param name="param0">(float) First parameters on the source curve</param>
    ///<param name="param1">(float) Second parameters on the source curve</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member AddSubCrv(curveId:Guid, param0:float, param1:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId)
        let trimcurve = curve.Trim(param0, param1)
        if isNull trimcurve then Error.Raise <| sprintf "RhinoScriptSyntax.AddSubCrv: Unable to trim curve.  curveId:'%A' param0:'%A' param1:'%A'" curveId param0 param1
        let rc = Doc.Objects.AddCurve(trimcurve)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.AddSubCrv: Unable to add curve to document.  curveId:'%A' param0:'%A' param1:'%A'" curveId param0 param1
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Returns the angle of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    Identifies the curve segment if curveId identifies a polycurve</param>
    ///<returns>(float) The angle in degrees</returns>
    static member ArcAngle(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.Curve is not an arc.  curveId:'%A' segmentIndex:'%A' curveId:'%A'" curveId segmentIndex curveId
        (!arc).AngleDegrees


    [<Extension>]
    ///<summary>Returns the center point of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D center point of the arc</returns>
    static member ArcCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.Curve is not an arc.  curveId:'%A' segmentIndex:'%A' curveId:'%A'" curveId segmentIndex curveId
        (!arc).Center


    [<Extension>]
    ///<summary>Returns the mid point of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D mid point of the arc</returns>
    static member ArcMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.Curve is not an arc.  curveId:'%A' segmentIndex:'%A' curveId:'%A'" curveId segmentIndex curveId
        (!arc).MidPoint


    [<Extension>]
    ///<summary>Returns the radius of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The radius of the arc</returns>
    static member ArcRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.Curve is not an arc.  curveId:'%A' segmentIndex:'%A' curveId:'%A'" curveId segmentIndex curveId
        (!arc).Radius



    [<Extension>]
    ///<summary>Returns the center point of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D center point of the circle</returns>
    static member CircleCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.Curve is not circle.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        (!circle).Center


    [<Extension>]
    ///<summary>Returns the center plane of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The 3D plane at the center point of the circle</returns>
    static member CircleCenterPlane(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.Curve is not circle.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        (!circle).Plane





    [<Extension>]
    ///<summary>Returns the circumference of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The circumference of the circle</returns>
    static member CircleCircumference(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.Curve is not circle.  curveId:'%A' segmentIndex:'%A' " curveId segmentIndex
        (!circle).Circumference


    [<Extension>]
    ///<summary>Returns the radius of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The radius of the circle</returns>
    static member CircleRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.Curve is not circle.  curveId:'%A' segmentIndex:'%A' " curveId segmentIndex
        (!circle).Radius


    [<Extension>]
    ///<summary>Closes an open curve object by making adjustments to the end points so
    ///    they meet at a point</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Maximum allowable distance between start and end point</param>
    ///<returns>(Guid) objectId of the new curve object</returns>
    static member CloseCurve(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if curve.IsClosed then  curveId
        else
            if not <| curve.MakeClosed(ifZero1 tolerance Doc.ModelAbsoluteTolerance) then  Error.Raise <| sprintf "RhinoScriptSyntax.CloseCurve: Unable to add curve to document.  curveId:'%A' tolerance:'%A'" curveId tolerance
            let rc = Doc.Objects.AddCurve(curve)
            if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.CloseCurve: Unable to add curve to document.  curveId:'%A' tolerance:'%A'" curveId tolerance
            Doc.Views.Redraw()
            rc


    [<Extension>]
    ///<summary>Determine the orientation (counter-clockwise or clockwise) of a closed,
    ///    planar curve</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="direction">(Vector3d) Optional, Default Value: <c>Vector3d.ZAxis</c>
    ///    3d vector that identifies up, or Z axs, direction of
    ///    the plane to test against</param>
    ///<returns>(int) 1 if the curve's orientation is clockwise
    ///    -1 if the curve's orientation is counter-clockwise
    ///     0 if unable to compute the curve's orientation</returns>
    static member ClosedCurveOrientation(curveId:Guid, [<OPT;DEF(Vector3d())>]direction:Vector3d) : int = //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let direction0 =if direction.IsZero then Vector3d.Unset else direction
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if not <| curve.IsClosed then  0
        else
            let orientation = curve.ClosedCurveOrientation(direction0)
            int(orientation)


    [<Extension>]
    ///<summary>Convert curve to a polyline curve</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>5.0</c>
    ///    The maximum angle between curve tangents at line endpoints.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.01</c>
    ///    The distance tolerance at segment midpoints.</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the curve object specified by curveId. If omitted, curveId will not be deleted</param>
    ///<param name="minEdgeLength">(float) Optional, Minimum segment length</param>
    ///<param name="maxEdgeLength">(float) Optional, Maximum segment length</param>
    ///<returns>(Guid) The new curve</returns>
    static member ConvertCurveToPolyline(curveId:Guid, [<OPT;DEF(0.0)>]angleTolerance:float, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(false)>]deleteInput:bool, [<OPT;DEF(0.0)>]minEdgeLength:float, [<OPT;DEF(0.0)>]maxEdgeLength:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let angleTolerance0 = toRadians (ifZero1 angleTolerance 0.5 )
        let tolerance0 = ifZero1 tolerance 0.01
        let polylineCurve = curve.ToPolyline( 0, 0, angleTolerance0, 0.0, 0.0, tolerance0, minEdgeLength, maxEdgeLength, true) //TODO what happens on 0.0 input ?
        if isNull polylineCurve then Error.Raise <| sprintf "RhinoScriptSyntax.ConvertCurveToPolyline: Unable to convertCurveToPolyline %A , maxEdgeLength%f, minEdgeLength:%f, deleteInput%b, tolerance%f, angleTolerance %f " curveId   maxEdgeLength minEdgeLength deleteInput tolerance angleTolerance
        if deleteInput then
            if Doc.Objects.Replace( curveId, polylineCurve) then
                curveId
            else
                Error.Raise <| sprintf "RhinoScriptSyntax.ConvertCurveToPolyline: Unable to convertCurveToPolyline %A , maxEdgeLength%f, minEdgeLength:%f, deleteInput%b, tolerance%f, angleTolerance %f " curveId   maxEdgeLength minEdgeLength deleteInput tolerance angleTolerance
        else
            let objectId = Doc.Objects.AddCurve( polylineCurve )
            if System.Guid.Empty= objectId then
                Error.Raise <| sprintf "RhinoScriptSyntax.ConvertCurveToPolyline: Unable to convertCurveToPolyline %A , maxEdgeLength%f, minEdgeLength:%f, deleteInput%b, tolerance%f, angleTolerance %f " curveId   maxEdgeLength minEdgeLength deleteInput tolerance angleTolerance
            else
                objectId


    [<Extension>]
    ///<summary>Returns the point on the curve that is a specified arc length
    ///    from the start of the curve</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="length">(float) The arc length from the start of the curve to evaluate</param>
    ///<param name="fromStart">(bool) Optional, Default Value: <c>true</c>
    ///    If not specified or True, then the arc length point is
    ///    calculated from the start of the curve. If False, the arc length
    ///    point is calculated from the end of the curve</param>
    ///<returns>(Point3d) on curve</returns>
    static member CurveArcLengthPoint(curveId:Guid, length:float, [<OPT;DEF(true)>]fromStart:bool) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let curveLength = curve.GetLength()
        if curveLength >= length then
            let mutable s = 0.0
            if length = 0.0 then  s <- 0.0
            elif abs(length-curveLength) < Doc.ModelAbsoluteTolerance then  s <- 1.0
            else s <- length / curveLength
            let dupe = if not fromStart then curve.Duplicate():?> Curve else curve
            if notNull dupe then
                if not fromStart then  dupe.Reverse() |> ignore
                let rc, t = dupe.NormalizedLengthParameter(s)
                if rc then
                    let pt = dupe.PointAt(t)
                    if not fromStart then dupe.Dispose()
                    pt
                else Error.Raise <| sprintf "RhinoScriptSyntax.CurveArcLengthPoint: Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveId length fromStart
            else Error.Raise <| sprintf "RhinoScriptSyntax.CurveArcLengthPoint: Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveId length fromStart
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurveArcLengthPoint: Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveId length fromStart


    [<Extension>]
    ///<summary>Returns area of closed planar curves. The results are based on the
    ///    current drawing units</summary>
    ///<param name="curveId">(Guid) The identifier of a closed, planar curve object</param>
    ///<returns>(float) The area</returns>
    static member CurveArea(curveId:Guid) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let tol = Doc.ModelAbsoluteTolerance
        let mp = AreaMassProperties.Compute(curve, tol)
        if isNull mp  then Error.Raise <| sprintf "RhinoScriptSyntax.CurveArea failed on %A" curveId
        mp.Area


    [<Extension>]
    ///<summary>Returns area centroid of closed, planar curves. The results are based
    ///    on the current drawing units</summary>
    ///<param name="curveId">(Guid) The identifier of a closed, planar curve object</param>
    ///<returns>(Point3d ) The 3d centroid point</returns>
    static member CurveAreaCentroid(curveId:Guid) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let tol = Doc.ModelAbsoluteTolerance
        let mp = AreaMassProperties.Compute(curve, tol)
        if isNull mp  then Error.Raise <| sprintf "RhinoScriptSyntax.CurveAreaCentroid failed on %A" curveId
        mp.Centroid


    [<Extension>]
    ///<summary>Get status of a curve object's annotation arrows</summary>
    ///<param name="curveId">(Guid) Identifier of a curve</param>
    ///<returns>(int) The current annotation arrow style
    ///    0 = no arrows
    ///    1 = display arrow at start of curve
    ///    2 = display arrow at end of curve
    ///    3 = display arrow at both start and end of curve</returns>
    static member CurveArrows(curveId:Guid) : int = //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let attr = rhobj.Attributes
        let rc = attr.ObjectDecoration
        if rc= DocObjects.ObjectDecoration.None then  0
        elif rc= DocObjects.ObjectDecoration.StartArrowhead then 1
        elif rc= DocObjects.ObjectDecoration.EndArrowhead then 2
        elif rc= DocObjects.ObjectDecoration.BothArrowhead then 3
        else -1

    [<Extension>]
    ///<summary>Enables or disables a curve object's annotation arrows</summary>
    ///<param name="curveId">(Guid) Identifier of a curve</param>
    ///<param name="arrowStyle">(int) The style of annotation arrow to be displayed.
    ///    0 = no arrows
    ///    1 = display arrow at start of curve
    ///    2 = display arrow at end of curve
    ///    3 = display arrow at both start and end of curve</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurveArrows(curveId:Guid, arrowStyle:int) : unit = //SET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let attr = rhobj.Attributes
        let rc = attr.ObjectDecoration
        if arrowStyle >= 0 && arrowStyle <= 3 then
            if arrowStyle = 0 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.None
            elif arrowStyle = 1 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.StartArrowhead
            elif arrowStyle = 2 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.EndArrowhead
            elif arrowStyle = 3 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.BothArrowhead
            Doc.Objects.ModifyAttributes(curveId, attr, true) |> ignore
            Doc.Views.Redraw()
        else
           Error.Raise <| sprintf "RhinoScriptSyntax.CurveArrows style %d invalid" arrowStyle

    [<Extension>]
    ///<summary>Enables or disables multiple curve objects's annotation arrows</summary>
    ///<param name="curveIds">(Guid seq) Identifier of multiple curve</param>
    ///<param name="arrowStyle">(int) The style of annotation arrow to be displayed.
    ///    0 = no arrows
    ///    1 = display arrow at start of curve
    ///    2 = display arrow at end of curve
    ///    3 = display arrow at both start and end of curve</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurveArrows(curveIds:Guid seq, arrowStyle:int) : unit = //MULTISET
        for curveId in curveIds do 
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
            let attr = rhobj.Attributes
            let rc = attr.ObjectDecoration
            if arrowStyle >= 0 && arrowStyle <= 3 then
                if arrowStyle = 0 then
                    attr.ObjectDecoration <- DocObjects.ObjectDecoration.None
                elif arrowStyle = 1 then
                    attr.ObjectDecoration <- DocObjects.ObjectDecoration.StartArrowhead
                elif arrowStyle = 2 then
                    attr.ObjectDecoration <- DocObjects.ObjectDecoration.EndArrowhead
                elif arrowStyle = 3 then
                    attr.ObjectDecoration <- DocObjects.ObjectDecoration.BothArrowhead
                if not <| Doc.Objects.ModifyAttributes(curveId, attr, true) then Error.Raise <| sprintf "RhinoScriptSyntax.CurveArrows style %d invalid" arrowStyle              
            else
               Error.Raise <| sprintf "RhinoScriptSyntax.Curve Arrow style %d invalid" arrowStyle
        Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Calculates the difference between two closed, planar curves and
    ///    adds the results to the document. Note, curves must be coplanar</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    ///<returns>(Guid ResizeArray) The identifiers of the new objects</returns>
    static member CurveBooleanDifference(curveA:Guid, curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB
        let tolerance = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanDifference(curve0, curve1, tolerance)
        let curves = ResizeArray()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.CurveBooleanDifference: Unable to add curve to document.  curveA:'%A' curveB:'%A' " curveA curveB
                    curves.Add(rc)
            Doc.Views.Redraw()
            curves
        else
            Error.Raise <| sprintf "RhinoScriptSyntax.CurveBooleanDifference: Unable to add curve to document.  curveA:'%A' curveB:'%A' " curveA curveB


    [<Extension>]
    ///<summary>Calculates the intersection of two closed, planar curves and adds
    ///    the results to the document. Note, curves must be coplanar</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    ///<returns>(Guid ResizeArray) The identifiers of the new objects</returns>
    static member CurveBooleanIntersection(curveA:Guid, curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB
        let tolerance = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanIntersection(curve0, curve1, tolerance)
        let curves = ResizeArray()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.CurveBooleanIntersection: Unable to add curve to document.  curveA:'%A' curveB:'%A' " curveA curveB
                    curves.Add(rc)
            Doc.Views.Redraw()
            curves
        else
            Error.Raise <| sprintf "RhinoScriptSyntax.CurveBooleanIntersection: Unable to add curve to document.  curveA:'%A' curveB:'%A' " curveA curveB


    [<Extension>]
    ///<summary>Calculate the union of two or more closed, planar curves and
    ///    add the results to the document. Note, curves must be coplanar</summary>
    ///<param name="curveIds">(Guid seq) List of two or more close planar curves identifiers</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    ///<returns>(Guid ResizeArray) The identifiers of the new objects</returns>
    static member CurveBooleanUnion(curveIds:Guid seq, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let inCurves = resizeArray { for objectId in curveIds -> RhinoScriptSyntax.CoerceCurve objectId }
        if inCurves.Count < 2 then Error.Raise <| sprintf "RhinoScriptSyntax.CurveBooleanUnion:curveIds must have at least 2 curves %A" curveIds
        let tolerance = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanUnion(inCurves, tolerance)
        let curves = ResizeArray()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.CurveBooleanUnion: Unable to add curve to document.  curveIds:'%A' " curveIds
                    curves.Add(rc)
            Doc.Views.Redraw()
        curves 


    [<Extension>]
    ///<summary>Intersects a curve object with a brep object. Note, unlike the
    ///    CurveSurfaceIntersection function, this function works on trimmed surfaces</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="brepId">(Guid) Identifier of a brep object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Distance tolerance at segment midpoints.</param>
    ///<returns>(Point3d ResizeArray * Curve ResizeArray) List of points and List of Curves</returns>
    static member CurveBrepIntersect(curveId:Guid, brepId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Point3d ResizeArray * Curve ResizeArray=
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let brep = RhinoScriptSyntax.CoerceBrep(brepId)
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let rc, outCurves, outPoints = Intersect.Intersection.CurveBrep(curve, brep, tolerance0)
        if not <| rc then  Error.Raise <| sprintf "RhinoScriptSyntax.CurveBrepIntersect: Intersection failed.  curveId:'%A' brepId:'%A' tolerance:'%A'" curveId brepId tolerance

        let curves = ResizeArray(0)
        let points = ResizeArray(0)
        for curve in outCurves do
            if notNull curve && curve.IsValid then
                curves.Add(curve)
                //let rc = Doc.Objects.AddCurve(curve)
                //curve.Dispose()
                //if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.CurveBrepIntersect: Unable to add curve to document.  curveId:'%A' brepId:'%A' tolerance:'%A'" curveId brepId tolerance
                //curves.Add(rc)
        for point in outPoints do
            if point.IsValid then
                points.Add(point)
                //let rc = Doc.Objects.AddPoint(point)
                //points.Add(rc)
        //Doc.Views.Redraw()
        points, curves //TODO or  Guid as originaly done ??


    [<Extension>]
    ///<summary>Returns the 3D point locations on two objects where they are closest to
    ///    each other. Note, this function provides similar functionality to that of
    ///    Rhino's ClosestPt command</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object to test</param>
    ///<param name="objectsIds">(Guid seq) List of identifiers of point cloud, curve, surface, or
    ///    polysurface to test against</param>
    ///<returns>(Guid * Point3d * Point3d) containing the results of the closest point calculation.
    ///    The elements are as follows:
    ///      [0]    The identifier of the closest object.
    ///      [1]    The 3-D point that is closest to the closest object.
    ///      [2]    The 3-D point that is closest to the test curve</returns>
    static member CurveClosestObject(curveId:Guid, objectsIds:Guid seq) : Guid * Point3d * Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let geometry = ResizeArray()
        for curveId in objectsIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
            geometry.Add( rhobj.Geometry )
        if Seq.isEmpty geometry then Error.Raise <| sprintf "RhinoScriptSyntax.CurveClosestObject: objectsIds must contain at least one item.  curveId:'%A' curveIds:'%A'" curveId objectsIds
        let curvePoint = ref Point3d.Unset
        let geomPoint  = ref Point3d.Unset
        let whichGeom = ref 0
        let success = curve.ClosestPoints(geometry, curvePoint, geomPoint, whichGeom)
        if success then  objectsIds|> Seq.item !whichGeom, !geomPoint, !curvePoint
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurveClosestObject failed  curveId:'%A' objectsIds:'%A'" curveId objectsIds

    [<Extension>]
    ///<summary>Returns the 3D point locations on the curve and finite line where they are closest to
    ///    each other. Note, this function provides similar functionality to that of
    ///    Rhino's ClosestPt command</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object to test</param>
    ///<param name="line">(Line) a Line Geometry</param>
    ///<returns>(Point3d * Point3d) first point on Curve, second point on Line</returns>
    static member CurveLineClosestPoint(curveId:Guid, line:Line) : Point3d * Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)       
        let curvePoint = ref Point3d.Unset
        let linePoint  = ref Point3d.Unset
        let success = curve.ClosestPoints(line.ToNurbsCurve(), curvePoint, linePoint)
        if success then  !linePoint, !curvePoint
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurveLineClosestPoint failed  curveId:'%A' Line:'%A'" curveId line



    [<Extension>]
    ///<summary>Returns parameter of the point on a curve that is closest to a test point</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="point">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The parameter of the closest point on the curve</returns>
    static member CurveClosestParameter(curveId:Guid, point:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let t = ref 0.
        let rc = curve.ClosestPoint(point, t)
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.CurveClosestParameter failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        !t

    [<Extension>]
    ///<summary>Returns the point on a curve that is closest to a test point</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="point">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The closest point on the curve</returns>
    static member CurveClosestPoint(curveId:Guid, point:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let rc, t = curve.ClosestPoint(point)
        if not <| rc then Error.Raise <| sprintf "RhinoScriptSyntax.CurveClosestPoint failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        curve.PointAt(t)




    [<Extension>]
    ///<summary>Returns the 3D point locations calculated by contouring a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="startPoint">(Point3d) 3D starting point of a center line</param>
    ///<param name="endPoint">(Point3d) 3D ending point of a center line</param>
    ///<param name="interval">(float) The distance between contour curves</param>
    ///<returns>(Point3d array) A list of 3D points, one for each contour</returns>
    static member CurveContourPoints(curveId:Guid, startPoint:Point3d, endPoint:Point3d, interval:float) : array<Point3d> =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if startPoint.DistanceTo(endPoint)<RhinoMath.ZeroTolerance then
            Error.Raise <| sprintf "RhinoScriptSyntax.Start && ende point are too close to define a line.  curveId:'%A' startPoint:'%A' endPoint:'%A'" curveId startPoint endPoint
        curve.DivideAsContour( startPoint, endPoint, interval)


    [<Extension>]
    ///<summary>Returns the curvature of a curve at a parameter. See the Rhino help for
    ///    details on curve curvature</summary>
    ///<param name="curveId">(Guid) Identifier of the curve</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<returns>(Point3d * Vector3d * Point3d * float * Vector3d) of curvature information
    ///    [0] = point at specified parameter
    ///    [1] = tangent vector
    ///    [2] = center of radius of curvature
    ///    [3] = radius of curvature
    ///    [4] = curvature vector</returns>
    static member CurveCurvature(curveId:Guid, parameter:float) : Point3d * Vector3d * Point3d * float * Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let point = curve.PointAt(parameter)
        let tangent = curve.TangentAt(parameter)
        if tangent.IsTiny(1e-10) then  Error.Raise <| sprintf "RhinoScriptSyntax.CurveCurvature: failed on tangent that is too small %A" curveId
        let cv = curve.CurvatureAt(parameter)
        let k = cv.Length
        if k<RhinoMath.SqrtEpsilon then  Error.Raise <| sprintf "RhinoScriptSyntax.CurveCurvature: failed on tangent that is too small %A" curveId
        let rv = cv / (k*k)
        let circle = Circle(point, tangent, point + 2.0*rv)
        let center = point + rv
        let radius = circle.Radius
        point, tangent, center, radius, cv


    [<Extension>]
    ///<summary>Calculates intersection of two curve objects</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object</param>
    ///<param name="curveB">(Guid) Optional, Identifier of the second curve object. If omitted, then a
    ///    self-intersection test will be performed on curveA</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Absolute tolerance in drawing units. If omitted,
    ///    the document's current absolute tolerance is used</param>
    ///<returns>( a ResizeArray of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float)
    ///    List of tuples: containing intersection information .
    ///    The list will contain one or more of the following elements:
    ///      Element Type     Description
    ///      [n][0]  Number   The intersection event type, either Point (1) or Overlap (2).
    ///      [n][1]  Point3d  If the event type is Point (1), then the intersection point
    ///        on the first curve. If the event type is Overlap (2), then
    ///        intersection start point on the first curve.
    ///      [n][2]  Point3d  If the event type is Point (1), then the intersection point
    ///        on the first curve. If the event type is Overlap (2), then
    ///        intersection end point on the first curve.
    ///      [n][3]  Point3d  If the event type is Point (1), then the intersection point
    ///        on the second curve. If the event type is Overlap (2), then
    ///        intersection start point on the second curve.
    ///      [n][4]  Point3d  If the event type is Point (1), then the intersection point
    ///        on the second curve. If the event type is Overlap (2), then
    ///        intersection end point on the second curve.
    ///      [n][5]  Number   If the event type is Point (1), then the first curve parameter.
    ///        If the event type is Overlap (2), then the start value of the
    ///        first curve parameter range.
    ///      [n][6]  Number   If the event type is Point (1), then the first curve parameter.
    ///        If the event type is Overlap (2), then the end value of the
    ///        first curve parameter range.
    ///      [n][7]  Number   If the event type is Point (1), then the second curve parameter.
    ///        If the event type is Overlap (2), then the start value of the
    ///        second curve parameter range.
    ///      [n][8]  Number   If the event type is Point (1), then the second curve parameter.
    ///        If the event type is Overlap (2), then the end value of the
    ///        second curve parameter range</returns>
    static member CurveCurveIntersection(curveA:Guid, [<OPT;DEF(Guid())>]curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) ResizeArray =
        let curve1 = RhinoScriptSyntax.CoerceCurve curveA
        let curve2 = if curveB= Guid.Empty then curve1 else RhinoScriptSyntax.CoerceCurve curveB
        let tolerance0 = ifZero1 tolerance Doc.ModelAbsoluteTolerance
        let mutable rc = null
        if curveB<>curveA then
            rc <- Intersect.Intersection.CurveCurve(curve1, curve2, tolerance0, Doc.ModelAbsoluteTolerance)
        else
            rc <- Intersect.Intersection.CurveSelf(curve1, tolerance0)
        if isNull rc then Error.Raise <| sprintf "RhinoScriptSyntax.CurveCurveIntersection faile dor %A; %A tolerance %f" curveB curveA tolerance
        let events = ResizeArray()
        for i =0 to rc.Count-1 do
            let mutable eventType = 1
            if( rc.[i].IsOverlap ) then  eventType <- 2
            let oa = rc.[i].OverlapA
            let ob = rc.[i].OverlapB
            let element = (eventType, rc.[i].PointA, rc.[i].PointA2, rc.[i].PointB, rc.[i].PointB2, oa.[0], oa.[1], ob.[0], ob.[1])
            events.Add(element)
        events



    [<Extension>]
    ///<summary>Returns the degree of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(int) The degree of the curve</returns>
    static member CurveDegree(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.Degree


    [<Extension>]
    ///<summary>Returns the minimum and maximum deviation between two curve objects</summary>
    ///<param name="curveA">(Guid) first Curve</param>
    ///<param name="curveB">(Guid) second Curve</param>
    ///<returns>(float * float * float * float * float * float) of deviation information
    ///    [0] = curveA parameter at maximum overlap distance point
    ///    [1] = curveB parameter at maximum overlap distance point
    ///    [2] = maximum overlap distance
    ///    [3] = curveAparameter at minimum overlap distance point
    ///    [4] = curveB parameter at minimum overlap distance point
    ///    [5] = minimum distance between curves</returns>
    static member CurveDeviation(curveA:Guid, curveB:Guid) : float * float * float * float * float * float =
        let curveA = RhinoScriptSyntax.CoerceCurve curveA
        let curveB = RhinoScriptSyntax.CoerceCurve curveB
        let tol = Doc.ModelAbsoluteTolerance
        let ok, maxa, maxb, maxd, mina, minb, mind = Curve.GetDistancesBetweenCurves(curveA, curveB, tol)
        if not ok then  Error.Raise <| sprintf "RhinoScriptSyntax.CurveDeviation failed for %A; %A" curveB curveA
        else
            maxa, maxb, maxd, mina, minb, mind


    [<Extension>]
    ///<summary>Returns the dimension of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    The curve segment if curveId identifies a polycurve</param>
    ///<returns>(int) The dimension of the curve .</returns>
    static member CurveDim(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.Dimension


    [<Extension>]
    ///<summary>Tests if two curve objects are generally in the same direction or if they
    ///    would be more in the same direction if one of them were flipped. When testing
    ///    curve directions, both curves must be either open or closed - you cannot test
    ///    one open curve and one closed curve</summary>
    ///<param name="curveA">(Guid) Identifier of first curve object</param>
    ///<param name="curveB">(Guid) Identifier of second curve object</param>
    ///<returns>(bool) True if the curve directions match, otherwise False</returns>
    static member CurveDirectionsMatch(curveA:Guid, curveB:Guid) : bool =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB
        Curve.DoDirectionsMatch(curve0, curve1)


    [<Extension>]
    ///<summary>Search for a derivatitive, tangent, or curvature discontinuity in
    ///    a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of curve object</param>
    ///<param name="style">(int) The type of continuity to test for. The types of
    ///    continuity are as follows:
    ///    Value    Description
    ///    1        C0 - Continuous function
    ///    2        C1 - Continuous first derivative
    ///    3        C2 - Continuous first and second derivative
    ///    4        G1 - Continuous unit tangent
    ///    5        G2 - Continuous unit tangent and curvature</param>
    ///<returns>(Point3d ResizeArray) 3D points where the curve is discontinuous</returns>
    static member CurveDiscontinuity(curveId:Guid, style:int) : Point3d ResizeArray =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let dom = curve.Domain
        let mutable t0 = dom.Min
        let t1 = dom.Max
        let points = ResizeArray()
        let mutable getNext = true
        while getNext do
            let st : Continuity = EnumOfValue style
            let getN, t = curve.GetNextDiscontinuity(st, t0, t1)
            getNext <- getN
            if getNext then
                points.Add(curve.PointAt(t))
                t0 <- t // Advance to the next parameter
        points


    [<Extension>]
    ///<summary>Returns the domain of a curve object
    ///    as an indexable object with two elements</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Interval) the domain of the curve</returns>
    static member CurveDomain(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Interval =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.Domain


    [<Extension>]
    ///<summary>Returns the edit, or Greville, points of a curve object.
    ///    For each curve control point, there is a corresponding edit point</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="returnParameters">(bool) Optional, Default Value: <c>false</c>
    ///    If True, return as a list of curve parameters.
    ///    If False, return as a list of 3d points</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index is `curveId` identifies a polycurve</param>
    ///<returns>(Collections.Point3dList) curve edit points</returns>
    static member CurveEditPoints(curveId:Guid, [<OPT;DEF(false)>]returnParameters:bool, [<OPT;DEF(-1)>]segmentIndex:int) : Collections.Point3dList =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let nc = curve.ToNurbsCurve()
        if isNull nc then  Error.Raise <| sprintf "RhinoScriptSyntax.CurveEditPoints faile for %A" curveId
        nc.GrevillePoints()


    [<Extension>]
    ///<summary>Returns the end point of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3d endpoint of the curve</returns>
    static member CurveEndPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.PointAtEnd

    [<Extension>]
    ///<summary>Returns the tangent at end point of a curve object
    /// pointing away from the curve </summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) the tangent, same as curve.TangentAtEnd property </returns>
    static member CurveEndTangent(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.TangentAtEnd

    [<Extension>]
    ///<summary>Returns the tangent at start point of a curve object
    /// pointing in direction of  the curve </summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) the tangent, same as curve.TangentAtStart property </returns>
    static member CurveStartTangent(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.TangentAtStart

    [<Extension>]
    ///<summary>Find points at which to cut a pair of curves so that a fillet of a
    ///    specified radius fits. A fillet point is a pair of points (point0, point1)
    ///    such that there is a circle of radius tangent to curve curve0 at point0 and
    ///    tangent to curve curve1 at point1. Of all possible fillet points, this
    ///    function returns the one which is the closest to the base point basePointA,
    ///    basePointB. Distance from the base point is measured by the sum of arc
    ///    lengths along the two curves</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object</param>
    ///<param name="radius">(float) The fillet radius</param>
    ///<param name="basePointA">(Point3d) Optional, The base point on the first curve.
    ///    If omitted, the starting point of the curve is used</param>
    ///<param name="basePointB">(Point3d) Optional, The base point on the second curve. If omitted,
    ///    the starting point of the curve is used</param>
    ///<returns>(Point3d * Point3d * Plane)
    ///    . The list elements are as follows:
    ///      [0]    A point on the first curve at which to cut (point).
    ///      [1]    A point on the second curve at which to cut (point).
    ///      [2]    The fillet plane</returns>
    static member CurveFilletPoints(curveA:Guid,
                                    curveB:Guid,
                                    radius:float,
                                    [<OPT;DEF(Point3d())>]basePointA:Point3d,
                                    [<OPT;DEF(Point3d())>]basePointB:Point3d) : Point3d * Point3d * Plane = //TODO make overload instead, this may leak  see draw vector and transform point!
        //  [<OPT;DEF(true)>]returnPoints:bool)
        //<param name="returnPoints">(bool) Optional, Default Value: <c>true</c>
        //If True (Default), then fillet points are
        //  returned. Otherwise, a fillet curve is created and                       // TODO not Implemented
        //  it's identifier is returned</param>
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB
        let basePointA = if basePointA = Point3d.Origin then Point3d.Unset else basePointA
        let basePointB = if basePointB = Point3d.Origin then Point3d.Unset else basePointB

        let inline distance  (a:Point3d)(b:Point3d) = (a-b).Length

        let t0Base =
            if basePointA <> Point3d.Unset then
                let ok, t = curve0.ClosestPoint(basePointA)
                if not ok then Error.Raise <| sprintf "RhinoScriptSyntax.CurveFilletPoints failed 1 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
                t
            else
                let distEnde  = min  (distance curve1.PointAtStart curve0.PointAtEnd)   (distance curve1.PointAtEnd curve0.PointAtEnd)
                let distStart = min  (distance curve1.PointAtStart curve0.PointAtStart) (distance curve1.PointAtEnd curve0.PointAtStart)
                if distStart < distEnde then curve0.Domain.Min else curve0.Domain.Max

        let t1Base =
            if basePointB <> Point3d.Unset then
                let ok, t = curve1.ClosestPoint(basePointB)
                if not ok then Error.Raise <| sprintf "RhinoScriptSyntax.CurveFilletPoints failed 2 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
                t
            else
                let distEnde  = min  (distance curve0.PointAtStart curve1.PointAtEnd)   (distance curve0.PointAtEnd curve1.PointAtEnd)
                let distStart = min  (distance curve0.PointAtStart curve1.PointAtStart) (distance curve0.PointAtEnd curve1.PointAtStart)
                if distStart < distEnde then curve1.Domain.Min else curve1.Domain.Max

        let ok, a, b, pl = Curve.GetFilletPoints(curve0, curve1, radius, t0Base, t1Base)
        if not ok then Error.Raise <| sprintf "RhinoScriptSyntax.CurveFilletPoints failed 3 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
        curve0.PointAt(a), curve0.PointAt(b), pl


    [<Extension>]
    ///<summary>Returns the plane at a parameter of a curve. The plane is based on the
    ///    tangent and curvature vectors at a parameter</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The plane at the specified parameter</returns>
    static member CurveFrame(curveId:Guid, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        let mutable para = parameter
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        let  domain = curve.Domain
        if not <| domain.IncludesParameter(parameter) then
            let  tol = Doc.ModelAbsoluteTolerance
            if parameter>domain.Max && (para-domain.Max)<=tol then
                para <- domain.Max
            elif parameter<domain.Min && (domain.Min-para)<=tol then
                para <- domain.Min
            else
                Error.Raise <| sprintf "RhinoScriptSyntax.CurveFrame failed.  curveId:'%A' parameter:'%A' segmentIndex:'%A'" curveId parameter segmentIndex
        let  rc, frame = curve.FrameAt(para)
        if rc && frame.IsValid then  frame
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurveFrame failed.  curveId:'%A' parameter:'%A' segmentIndex:'%A'" curveId parameter segmentIndex


    [<Extension>]
    ///<summary>Returns the knot count of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(int) The number of knots</returns>
    static member CurveKnotCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let  nc = curve.ToNurbsCurve()
        if notNull nc then  Error.Raise <| sprintf "RhinoScriptSyntax.CurveKnotCount failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        nc.Knots.Count


    [<Extension>]
    ///<summary>Returns the knots, or knot vector, of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float ResizeArray) knot values</returns>
    static member CurveKnots(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : ResizeArray<float> =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let  nc = curve.ToNurbsCurve()
        if notNull nc then  Error.Raise <| sprintf "RhinoScriptSyntax.CurveKnots failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        resizeArray{ for i = 0 to nc.Knots.Count - 1 do yield nc.Knots.[i] }



    [<Extension>]
    ///<summary>Returns the length of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="subDomain">(Interval) Optional, List of two numbers identifying the sub-domain of the
    ///    curve on which the calculation will be performed. The two parameters
    ///    (sub-domain) must be non-decreasing. If omitted, the length of the
    ///    entire curve is returned</param>
    ///<returns>(float) The length of the curve</returns>
    static member CurveLength(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int, [<OPT;DEF(Interval())>]subDomain:Interval) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        if subDomain.T0 = 0.0 && subDomain.T1 = 0.0 then curve.GetLength() 
        else curve.GetLength(subDomain)


    [<Extension>]
    ///<summary>Returns the average unitized direction of a curve object between start and end point, 
    ///    Optionally allows non linear curves too.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="allowNonLinear">(bool) Optional, allow non linear curves, Default Value <c>False</c> </param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) The direction of the curve</returns>
    static member CurveDirection(curveId:Guid, [<OPT;DEF(false)>]allowNonLinear:bool,[<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        if allowNonLinear || curve.IsLinear(RhinoMath.ZeroTolerance) then
            if curve.IsClosed || curve.IsClosable(Doc.ModelAbsoluteTolerance) then Error.Raise <| sprintf "RhinoScriptSyntax.CurveDirection failed on closed or closable curve.  curveId:'%A' allowNonLinear '%A' segmentIndex:'%A'" curveId allowNonLinear segmentIndex
            let v = curve.PointAtEnd - curve.PointAtStart
            v.Unitized
        else
            Error.Raise <| sprintf "RhinoScriptSyntax.CurveDirection failed.  curveId:'%A' allowNonLinear '%A' segmentIndex:'%A'" curveId allowNonLinear segmentIndex


    [<Extension>]
    ///<summary>Returns the mid point of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D midpoint of the curve</returns>
    static member CurveMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let  rc, t = curve.NormalizedLengthParameter(0.5)
        if rc then  curve.PointAt(t)
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurveMidPoint failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex


    [<Extension>]
    ///<summary>Returns the normal direction of the plane in which a planar curve object lies</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment if curveId identifies a polycurve</param>
    ///<returns>(Vector3d) The 3D normal vector</returns>
    static member CurveNormal(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let tol = Doc.ModelAbsoluteTolerance
        let plane = ref Plane.WorldXY
        let rc = curve.TryGetPlane(plane, tol)
        if rc then  (!plane).Normal
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurveNormal failed. curveId:'%A' segmentIndex:'%A'" curveId segmentIndex


    [<Extension>]
    ///<summary>Converts a curve parameter to a normalized curve parameter;
    ///    one that ranges between 0-1</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) The curve parameter to convert</param>
    ///<returns>(float) normalized curve parameter</returns>
    static member CurveNormalizedParameter(curveId:Guid, parameter:float) : float =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        curve.Domain.NormalizedParameterAt(parameter)


    [<Extension>]
    ///<summary>Converts a normalized curve parameter to a curve parameter;
    ///    one within the curve's domain</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) The normalized curve parameter to convert</param>
    ///<returns>(float) curve parameter</returns>
    static member CurveParameter(curveId:Guid, parameter:float) : float =
        let curve = RhinoScriptSyntax.CoerceCurve curveId
        curve.Domain.ParameterAt(parameter)


    [<Extension>]
    ///<summary>Returns the perpendicular plane at a parameter of a curve. The result
    ///    is relatively parallel (zero-twisting) plane</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<returns>(Plane) Plane</returns>
    static member CurvePerpFrame(curveId:Guid, parameter:float) : Plane =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        let  rc, plane = curve.PerpendicularFrameAt(parameter)
        if rc then  plane else Error.Raise <| sprintf "RhinoScriptSyntax.CurvePerpFrame failed. curveId:'%A' parameter:'%f'" curveId parameter


    [<Extension>]
    ///<summary>Returns the plane in which a planar curve lies. Note, this function works
    ///    only on planar curves</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The plane in which the curve lies</returns>
    static member CurvePlane(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let tol = Doc.ModelAbsoluteTolerance
        let plane = ref Plane.WorldXY
        let rc = curve.TryGetPlane(plane, tol)
        if rc then  !plane
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurvePlane failed. curveId:'%A' segmentIndex:'%A'" curveId segmentIndex


    [<Extension>]
    ///<summary>Returns the control points count of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(int) Number of control points</returns>
    static member CurvePointCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let mutable nc = curve.ToNurbsCurve()
        if notNull nc then  nc.Points.Count
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurvePointCount failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex


    [<Extension>]
    ///<summary>Returns the control points, or control vertices, of a curve object.
    ///    If the curve is a rational NURBS curve, the euclidean control vertices
    ///    are returned</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d ResizeArray) the control points, or control vertices, of a curve object</returns>
    static member CurvePoints(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d ResizeArray =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let  nc = curve.ToNurbsCurve()
        if isNull nc then  Error.Raise <| sprintf "RhinoScriptSyntax.CurvePoints failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        resizeArray { for i = 0 to nc.Points.Count-1 do yield nc.Points.[i].Location }


    [<Extension>]
    ///<summary>Returns the radius of curvature at a point on a curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="testPoint">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment if curveId identifies a polycurve</param>
    ///<returns>(float) The radius of curvature at the point on the curve</returns>
    static member CurveRadius(curveId:Guid, testPoint:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let mutable rc, t = curve.ClosestPoint(testPoint)//, 0.0)
        if not <| rc then  Error.Raise <| sprintf "RhinoScriptSyntax.CurveRadius failed.  curveId:'%A' testPoint:'%A' segmentIndex:'%A'" curveId testPoint segmentIndex
        let mutable v = curve.CurvatureAt( t )
        let mutable k = v.Length
        if k>RhinoMath.ZeroTolerance then  1.0/k
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurveRadius failed.  curveId:'%A' testPoint:'%A' segmentIndex:'%A'" curveId testPoint segmentIndex


    [<Extension>]
    ///<summary>Adjusts the seam, or start/end, point of a closed curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) The parameter of the new start/end point.
    ///    Note, if successful, the resulting curve's
    ///    domain will start at `parameter`</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CurveSeam(curveId:Guid, parameter:float) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if (not <| curve.IsClosed || not <| curve.Domain.IncludesParameter(parameter)) then
            false
        else
            let dupe = curve.Duplicate():?>Curve
            if notNull dupe then
                let r = dupe.ChangeClosedCurveSeam(parameter)
                if not r then r
                else
                    Doc.Objects.Replace(curveId, dupe)
            else
                false


    [<Extension>]
    ///<summary>Returns the start point of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="point">(Point3d) Optional, New start point</param>
    ///<returns>(Point3d) The 3D starting point of the curve</returns>
    static member CurveStartPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.PointAtStart

    [<Extension>]
    ///<summary>Sets the start point of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="point">(Point3d) New start point</param>
    ///<returns>(unit)</returns>
    static member CurveStartPoint(curveId:Guid, point:Point3d) : unit =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if not <|curve.SetStartPoint(point) then Error.Raise <| sprintf "RhinoScriptSyntax.CurveStartPoint failed on '%A' and '%A'" point curveId
        Doc.Objects.Replace(curveId, curve) |> ignore
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Calculates intersection of a curve object with a surface object.
    ///    Note, this function works on the untrimmed portion of the surface</summary>
    ///<param name="curveId">(Guid) The identifier of the first curve object</param>
    ///<param name="surfaceId">(Guid) The identifier of the second curve object. If omitted,
    ///    the a self-intersection test will be performed on curve</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    The absolute tolerance in drawing units.</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>Doc.ModelAngleToleranceRadians</c>
    ///    Angle tolerance in degrees. The angle
    ///    tolerance is used to determine when the curve is tangent to the
    ///    surface.</param>
    ///<returns>(ResizeArray of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) of intersection information .
    ///    The list will contain one or more of the following elements:
    ///      Element Type     Description
    ///      [n][0]  Number   The intersection event type, either Point(1) or Overlap(2).
    ///      [n][1]  Point3d  If the event type is Point(1), then the intersection point
    ///        on the first curve. If the event type is Overlap(2), then
    ///        intersection start point on the first curve.
    ///      [n][2]  Point3d  If the event type is Point(1), then the intersection point
    ///        on the first curve. If the event type is Overlap(2), then
    ///        intersection end point on the first curve.
    ///      [n][3]  Point3d  If the event type is Point(1), then the intersection point
    ///        on the second curve. If the event type is Overlap(2), then
    ///        intersection start point on the surface.
    ///      [n][4]  Point3d  If the event type is Point(1), then the intersection point
    ///        on the second curve. If the event type is Overlap(2), then
    ///        intersection end point on the surface.
    ///      [n][5]  Number   If the event type is Point(1), then the first curve parameter.
    ///        If the event type is Overlap(2), then the start value of the
    ///        first curve parameter range.
    ///      [n][6]  Number   If the event type is Point(1), then the first curve parameter.
    ///        If the event type is Overlap(2), then the end value of the
    ///        curve parameter range.
    ///      [n][7]  Number   If the event type is Point(1), then the U surface parameter.
    ///        If the event type is Overlap(2), then the U surface parameter
    ///        for curve at (n, 5).
    ///      [n][8]  Number   If the event type is Point(1), then the V surface parameter.
    ///        If the event type is Overlap(2), then the V surface parameter
    ///        for curve at (n, 5).
    ///      [n][9]  Number   If the event type is Point(1), then the U surface parameter.
    ///        If the event type is Overlap(2), then the U surface parameter
    ///        for curve at (n, 6).
    ///      [n][10] Number   If the event type is Point(1), then the V surface parameter.
    ///        If the event type is Overlap(2), then the V surface parameter
    ///        for curve at (n, 6)</returns>
    static member CurveSurfaceIntersection(curveId:Guid, surfaceId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(0.0)>]angleTolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float*float*float) ResizeArray =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let surface = RhinoScriptSyntax.CoerceSurface surfaceId
        let tolerance0 = ifZero1 tolerance Doc.ModelAbsoluteTolerance
        let angleTolerance0 = ifZero1 (toRadians(angleTolerance)) Doc.ModelAngleToleranceRadians
        let  rc = Intersect.Intersection.CurveSurface(curve, surface, tolerance0, angleTolerance0)
        if isNull rc then Error.Raise <| sprintf "RhinoScriptSyntax.CurveSurfaceIntersection failed. (surfaceId:%A) (curveId:%A) (angleTolerance:%f) (tolerance:%f) " surfaceId curveId angleTolerance tolerance
        let events = ResizeArray()
        for i = 0 to rc.Count - 1 do
            let eventType = if rc.[i].IsOverlap then 2 else 1
            let item = rc.[i]
            let oa = item.OverlapA
            let u, v = item.SurfaceOverlapParameter()
            let e = eventType, item.PointA, item.PointA2, item.PointB, item.PointB2, oa.[0], oa.[1], u.[0], u.[1], v.[0], v.[1]
            events.Add(e)
        events




    [<Extension>]
    ///<summary>Returns a 3D vector that is the tangent to a curve at a parameter</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) A 3D vector</returns>
    static member CurveTangent(curveId:Guid, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let mutable rc = Point3d.Unset
        if curve.Domain.IncludesParameter(parameter) then
            curve.TangentAt(parameter)
        else
            Error.Raise <| sprintf "RhinoScriptSyntax.CurveTangent failed.  curveId:'%A' parameter:'%A' segmentIndex:'%A'" curveId parameter segmentIndex


    [<Extension>]
    ///<summary>Returns list of weights that are assigned to the control points of a curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float ResizeArray) The weight values of the curve</returns>
    static member CurveWeights(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) :  float ResizeArray =
        let nc =
            match RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex) with
            | :? NurbsCurve as nc -> nc
            | c -> c.ToNurbsCurve()
        if isNull nc then  Error.Raise <| sprintf "RhinoScriptSyntax.CurveWeights failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        resizeArray { for pt in nc.Points -> pt.Weight }



    [<Extension>]
    ///<summary>Divides a curve object into a specified number of segments</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segments">(int) The number of segments</param>
    ///<returns>(Point3d array) Array containing division curve parameters</returns>
    static member DivideCurveIntoPoints(curveId:Guid, segments:int) : Point3d array =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        let pts = ref (Array.zeroCreate (segments + 1))
        let rc = curve.DivideByCount(segments, true, pts)
        if isNull rc then  Error.Raise <| sprintf "RhinoScriptSyntax.DivideCurve failed.  curveId:'%A' segments:'%A'" curveId segments
        !pts


    [<Extension>]
    ///<summary>Divides a curve object into a specified number of segments</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segments">(int) The number of segments</param>
    ///<returns>( float array ) array containing 3D division points</returns>
    static member DivideCurve(curveId:Guid, segments:int) :  float array =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        let rc = curve.DivideByCount(segments, true)
        if isNull rc then  Error.Raise <| sprintf "RhinoScriptSyntax.DivideCurve failed.  curveId:'%A' segments:'%A'" curveId segments
        rc


    [<Extension>]
    ///<summary>Divides a curve such that the linear distance between the points is equal</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="distance">(float) Linear distance between division points</param>
    ///<returns>(Point3d array) array containing 3D division points</returns>
    static member DivideCurveEquidistant(curveId:Guid, distance:float) : array<Point3d> =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        let  points = curve.DivideEquidistant(distance)
        if isNull points then  Error.Raise <| sprintf "RhinoScriptSyntax.DivideCurveEquidistant failed.  curveId:'%A' distance:'%A'" curveId distance
        points
        //let  tvals = ResizeArray()
        //for point in points do
        //    let mutable rc, t = curve.ClosestPoint(point)
        //    tvals.Add(t)
        //tvals


    [<Extension>]
    ///<summary>Divides a curve object into segments of a specified length</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="length">(float) The length of each segment</param>
    ///<returns>(Point3d ResizeArray) a list containing division points</returns>
    static member DivideCurveLengthIntoPoints(curveId:Guid, length:float) : Point3d ResizeArray =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let rc = curve.DivideByLength(length, true)
        if isNull rc then  Error.Raise <| sprintf "RhinoScriptSyntax.DivideCurveLength failed.  curveId:'%A' length:'%A'" curveId length
        ResizeArray.init rc.Length (fun i-> curve.PointAt(rc.[i]))

    [<Extension>]
    ///<summary>Divides a curve object into segments of a specified length</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="length">(float) The length of each segment</param>
    ///<returns>( float array) a list containing division parameters</returns>
    static member DivideCurveLength(curveId:Guid, length:float) :  float [] =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let rc = curve.DivideByLength(length, true)
        if isNull rc then  Error.Raise <| sprintf "RhinoScriptSyntax.DivideCurveLength failed.  curveId:'%A' length:'%A'" curveId length
        rc


    [<Extension>]
    ///<summary>Returns the center point of an elliptical-shaped curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<returns>(Point3d) The 3D center point of the ellipse</returns>
    static member EllipseCenterPoint(curveId:Guid) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let rc, ellipse = curve.TryGetEllipse()
        if not <| rc then  Error.Raise <| sprintf "RhinoScriptSyntax.EllipseCenterPoint: Curve is not an ellipse.  curveId:'%A'" curveId
        ellipse.Plane.Origin


    [<Extension>]
    ///<summary>Returns the quadrant points of an elliptical-shaped curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four points identifying the quadrants of the ellipse</returns>
    static member EllipseQuadPoints(curveId:Guid) : Point3d * Point3d * Point3d * Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let rc, ellipse = curve.TryGetEllipse()
        if not <| rc then  Error.Raise <| sprintf "RhinoScriptSyntax.EllipseQuadPoints: Curve is not an ellipse.  curveId:'%A'" curveId
        let origin = ellipse.Plane.Origin;
        let xaxis = ellipse.Radius1 * ellipse.Plane.XAxis;
        let yaxis = ellipse.Radius2 * ellipse.Plane.YAxis;
        (origin-xaxis, origin + xaxis, origin-yaxis, origin + yaxis)


    [<Extension>]
    ///<summary>Evaluates a curve at a parameter and returns a 3D point</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="t">(float) The parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) a 3-D point</returns>
    static member EvaluateCurve(curveId:Guid, t:float, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.PointAt(t)


    [<Extension>]
    ///<summary>Explodes, or un-joins, one curve. Polycurves will be exploded into curve
    ///    segments. Polylines will be exploded into line segments. ExplodeCurves will
    ///    return the curves in topological order</summary>
    ///<param name="curveId">(Guid) The curve object to explode</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input objects after exploding if True</param>
    ///<returns>(Guid ResizeArray) identifying the newly created curve objects</returns>
    static member ExplodeCurve(curveId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid ResizeArray =
        let rc = ResizeArray()
        let curve = RhinoScriptSyntax.CoerceCurve curveId
        let pieces = curve.DuplicateSegments()
        if notNull pieces then
            for piece in pieces do
                rc.Add(Doc.Objects.AddCurve(piece))
            if deleteInput then
                Doc.Objects.Delete(curveId, true) |>ignore
        if rc.Count>0 then  Doc.Views.Redraw()
        rc






    [<Extension>]
    ///<summary>Extends a non-closed curve object by a line, arc, or smooth extension
    ///    until it intersects a collection of objects</summary>
    ///<param name="curveId">(Guid) Identifier of curve to extend</param>
    ///<param name="extensionType">(int) 0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    ///<param name="side">(int) 0= extend from the start of the curve
    ///    1= extend from the end of the curve
    ///    2= extend from both the start and the end of the curve</param>
    ///<param name="boundarycurveIds">(Guid seq) Curve, surface, and polysurface objects to extend to</param>
    ///<param name="replaceInput">(bool) Optional, Default Value <c>false</c> Replace input or add new?</param>
    ///<returns>(Guid) The identifier of the new object or orignal curve ( depending on 'replaceInput')</returns>
    static member ExtendCurve(curveId:Guid, extensionType:int, side:int, boundarycurveIds:Guid seq,[<OPT;DEF(false)>]replaceInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let mutable extensionTypet = CurveExtensionStyle.Line
        if extensionType = 0 then  extensionTypet <- CurveExtensionStyle.Line
        elif extensionType = 1 then extensionTypet <- CurveExtensionStyle.Arc
        elif extensionType = 2 then extensionTypet <- CurveExtensionStyle.Smooth
        else Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurve ExtensionType must be 0, 1, || 2.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds

        let mutable sidet = CurveEnd.Start
        if side = 0 then  sidet <- CurveEnd.Start
        elif side = 1 then  sidet <- CurveEnd.End
        elif side = 2 then sidet <- CurveEnd.Both
        else Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurve Side must be 0, 1, || 2.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds

        let rhobjs = resizeArray { for objectId in boundarycurveIds -> RhinoScriptSyntax.CoerceRhinoObject(objectId) }
        if isNull rhobjs then  Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurve BoundarycurveIds failed. they must contain at least one item.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds
        let geometry = resizeArray { for obj in rhobjs -> obj.Geometry }
        let newcurve = curve.Extend(sidet, extensionTypet, geometry)
        if notNull newcurve && newcurve.IsValid then
            if replaceInput then
              if Doc.Objects.Replace(curveId, newcurve) then
                 Doc.Views.Redraw()
                 curveId
              else
                 Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurve failed.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds
            else
              let g= Doc.Objects.AddCurve(newcurve)
              Doc.Views.Redraw()
              g
        else
            Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurve failed.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds


    [<Extension>]
    ///<summary>Extends a non-closed curve by a line, arc, or smooth extension for a
    ///    specified distance</summary>
    ///<param name="curveId">(Guid) Curve to extend</param>
    ///<param name="extensionType">(int) 0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    ///<param name="side">(int) 0= extend from start of the curve
    ///    1= extend from end of the curve
    ///    2= Extend from both ends</param>
    ///<param name="length">(float) Distance to extend</param>
    ///<returns>(Guid) The identifier of the new object</returns>
    static member ExtendCurveLength(curveId:Guid, extensionType:int, side:int, length:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let mutable extensionTypet = CurveExtensionStyle.Line
        if extensionType = 0 then  extensionTypet <- CurveExtensionStyle.Line
        elif extensionType = 1 then extensionTypet <- CurveExtensionStyle.Arc
        elif extensionType = 2 then extensionTypet <- CurveExtensionStyle.Smooth
        else Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurveLength ExtensionType must be 0, 1, || 2.  curveId:'%A' extensionType:'%A' side:'%A' length:'%A'" curveId extensionType side length

        let mutable sidet = CurveEnd.Start
        if side = 0 then  sidet <- CurveEnd.Start
        elif side = 1 then  sidet <- CurveEnd.End
        elif side = 2 then sidet <- CurveEnd.Both
        else Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurveLength Side must be 0, 1, || 2.  curveId:'%A' extensionType:'%A' side:'%A' length:'%A'" curveId extensionType side length

        let newcurve =
            if length<0. then curve.Trim(sidet, -length)
            else curve.Extend(sidet, length, extensionTypet)

        if notNull newcurve && newcurve.IsValid then
            if Doc.Objects.Replace(curveId, newcurve) then
                Doc.Views.Redraw()
                curveId
            else Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurveLength failed.  curveId:'%A' extensionType:'%A' side:'%A' length:'%A'" curveId extensionType side length
        else Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurveLength failed.  curveId:'%A' extensionType:'%A' side:'%A' length:'%A'" curveId extensionType side length


    [<Extension>]
    ///<summary>Extends a non-closed curve by smooth extension to a point</summary>
    ///<param name="curveId">(Guid) Curve to extend</param>
    ///<param name="side">(int) 0= extend from start of the curve
    ///    1= extend from end of the curve</param>
    ///<param name="point">(Point3d) Point to extend to</param>
    ///<returns>(Guid) The identifier of the new object</returns>
    static member ExtendCurvePoint(curveId:Guid, side:int, point:Point3d) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)

        let mutable sidet = CurveEnd.Start
        if side = 0 then  sidet <- CurveEnd.Start
        elif side = 1 then  sidet <- CurveEnd.End
        elif side = 2 then sidet <- CurveEnd.Both
        else Error.Raise <| sprintf "RhinoScriptSyntax.Side must be 0, 1, || 2.  curveId:'%A' side:'%A' point:'%A'" curveId side point

        let extensionType = CurveExtensionStyle.Smooth
        let newcurve = curve.Extend(sidet, extensionType, point)
        if notNull newcurve && newcurve.IsValid then
            if Doc.Objects.Replace( curveId, newcurve ) then
                Doc.Views.Redraw()
                curveId
            else Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurvePoint failed.  curveId:'%A' side:'%A' point:'%A'" curveId side point
        else Error.Raise <| sprintf "RhinoScriptSyntax.ExtendCurvePoint failed.  curveId:'%A' side:'%A' point:'%A'" curveId side point


    [<Extension>]
    ///<summary>Fairs a curve. Fair works best on degree 3 (cubic) curves. Fair attempts
    ///    to remove large curvature variations while limiting the geometry changes to
    ///    be no more than the specified tolerance. Sometimes several applications of
    ///    this method are necessary to remove nasty curvature problems</summary>
    ///<param name="curveId">(Guid) Curve to fair</param>
    ///<param name="tolerance">(float) Fairing tolerance</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member FairCurve(curveId:Guid, tolerance:float) : bool =
        let mutable curve = RhinoScriptSyntax.CoerceCurve curveId
        let angleTol = 0.0
        let mutable clamp = 0
        if curve.IsPeriodic then
            curve <- curve.ToNurbsCurve()
            clamp <- 1
        let newcurve = curve.Fair(tolerance, angleTol, clamp, clamp, 100)
        if notNull newcurve then  false
        else
            if Doc.Objects.Replace(curveId, newcurve) then
                Doc.Views.Redraw()
                true
            else
                false


    [<Extension>]
    ///<summary>Reduces number of curve control points while maintaining the curve's same
    ///    general shape. Use this function for replacing curves with many control
    ///    points. For more information, see the Rhino help for the FitCrv command</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///    The curve degree, which must be greater than 1.
    ///    The default is 3</param>
    ///<param name="distanceTolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    The fitting tolerance.</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>Doc.ModelAngleToleranceRadians</c>
    ///    The kink smoothing tolerance in degrees. If
    ///    angleTolerance is 0.0, all kinks are smoothed. If angleTolerance
    ///    is bigger than  0.0, kinks smaller than angleTolerance are smoothed. If
    ///    angleTolerance is not specified or smaller than 0.0, the document angle
    ///    tolerance is used for the kink smoothing</param>
    ///<returns>(Guid) The identifier of the new object</returns>
    static member FitCurve( curveId:Guid, 
                            [<OPT;DEF(3)>]degree:int, 
                            [<OPT;DEF(0.0)>]distanceTolerance:float, 
                            [<OPT;DEF(-1.0)>]angleTolerance:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let distanceTolerance0 = ifZero1 distanceTolerance Doc.ModelAbsoluteTolerance
        let angleTolerance0 = if  angleTolerance < 0.0 then  Doc.ModelAngleToleranceRadians else toRadians angleTolerance
        let nc = curve.Fit(degree, distanceTolerance0, angleTolerance0)
        if notNull nc then
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
            let mutable rc = Guid.Empty
            if notNull rhobj then
                rc <- Doc.Objects.AddCurve(nc, rhobj.Attributes)
            else
                rc <- Doc.Objects.AddCurve(nc)
            if rc = Guid.Empty then  Error.Raise <| sprintf "RhinoScriptSyntax.FitCurve: Unable to add curve to document.  curveId:'%A' degree:'%A' distanceTolerance:'%A' angleTolerance:'%A'" curveId degree distanceTolerance angleTolerance
            Doc.Views.Redraw()
            rc
        else
            Error.Raise <| sprintf "RhinoScriptSyntax.FitCurve failed.  curveId:'%A' degree:'%A' distanceTolerance:'%A' angleTolerance:'%A'" curveId degree distanceTolerance angleTolerance


    [<Extension>]
    ///<summary>Inserts a knot into a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter on the curve</param>
    ///<param name="symmetrical">(bool) Optional, Default Value: <c>false</c>
    ///    If True, then knots are added on both sides of
    ///    the center of the curve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member InsertCurveKnot(curveId:Guid, parameter:float, [<OPT;DEF(false)>]symmetrical:bool) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if not <| curve.Domain.IncludesParameter(parameter) then  false
        else
            let nc = curve.ToNurbsCurve()
            if isNull nc then  false
            else
                let rc, t = curve.GetNurbsFormParameterFromCurveParameter(parameter)
                if rc then
                    let mutable rc = nc.Knots.InsertKnot(t, 1)
                    if rc && symmetrical then
                        let domain = nc.Domain
                        let tSym = domain.T1 - t + domain.T0
                        if abs(tSym)>RhinoMath.SqrtEpsilon then
                            rc <- nc.Knots.InsertKnot(tSym, 1)
                            if rc then  Doc.Views.Redraw()
                    if rc then
                        rc <- Doc.Objects.Replace(curveId, nc)
                        if rc then  Doc.Views.Redraw()
                rc


    [<Extension>]
    ///<summary>Verifies an object is an open arc curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///    If the curve is not a circle, then the tolerance used
    ///    to determine whether or not the NURBS form of the curve has the
    ///    properties of a arc.</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsArc(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsArc(tol) && not curve.IsClosed


    [<Extension>]
    ///<summary>Verifies an object is a circle curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///    If the curve is not a circle, then the tolerance used
    ///    to determine whether or not the NURBS form of the curve has the
    ///    properties of a circle.</param>
    ///<returns>(bool) True or False</returns>
    static member IsCircle(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tol = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsCircle(tol)


    [<Extension>]
    ///<summary>Verifies an object is a curve</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurve(curveId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> true


    [<Extension>]
    ///<summary>Decide if it makes sense to close off the curve by moving the end point
    ///    to the start point based on start-end gap size and length of curve as
    ///    approximated by chord defined by 6 points</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Maximum allowable distance between start point and end point.</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurveClosable(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsClosable(tolerance0)


    [<Extension>]
    ///<summary>Verifies an object is a closed curve object</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<returns>(bool) If curve is Closed True,  otherwise False</returns>
    static member IsCurveClosed(curveId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsClosed


    [<Extension>]
    ///<summary>Test a curve to see if it lies in a specific plane</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="plane">(Plane) Plane to test</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<returns>(bool) True or False</returns>
    static member IsCurveInPlane(curveId:Guid, plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance0 = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsInPlane(plane, tolerance0)


    [<Extension>]
    ///<summary>Verifies an object is a linear curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>    ///
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurveLinear(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tolerance0 = ifZero2 RhinoMath.ZeroTolerance tolerance
        match  RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsLinear(tolerance0)


    [<Extension>]
    ///<summary>Verifies an object is a periodic curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurvePeriodic(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsPeriodic


    [<Extension>]
    ///<summary>Verifies an object is a planar curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurvePlanar(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsPlanar(tol)


    [<Extension>]
    ///<summary>Verifies an object is a rational NURBS curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurveRational(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some c  ->
            match c with
            | :? NurbsCurve as curve -> curve.IsRational
            |_ -> false


    [<Extension>]
    ///<summary>Verifies an object is an elliptical-shaped curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<param name="segmentIndex">(int) Optional,
    ///    The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsEllipse(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsEllipse(tol)


    [<Extension>]
    ///<summary>Verifies an object is a line curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<param name="segmentIndex">(int) Optional,
    ///    The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsLine(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some c  ->
            match c with
            | :? LineCurve  -> true
            | curve  ->
                if curve.IsLinear(tol) then true
                else
                    let rc, polyline = curve.TryGetPolyline()
                    if rc && polyline.Count = 2 then  true
                    else false


    [<Extension>]
    ///<summary>Verifies that a point is on a curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="point">(Point3d) The test point</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.SqrtEpsilon</c></param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsPointOnCurve(curveId:Guid, point:Point3d, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = ifZero2 RhinoMath.SqrtEpsilon tolerance
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let t = ref 0.0
        curve.ClosestPoint(point, t, tol)


    [<Extension>]
    ///<summary>Verifies an object is a PolyCurve curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsPolyCurve(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        // TODO can a polycurve be nested in a polycurve ?
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None               -> false
        |Some c  ->
            match c with
            | :? PolyCurve  -> true
            | _             -> false


    [<Extension>]
    ///<summary>Verifies an object is a Polyline curve object or a nurbs cure with degree 1 and moer than 2 points
    /// Lines return false</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsPolyline(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None               -> false
        |Some c  ->
            match c with
            | :? PolylineCurve  -> true           
            | :? NurbsCurve as nc -> nc.Points.Count > 2 && c.Degree = 1
            | _ -> false
                


    [<Extension>]
    ///<summary>Joins multiple curves together to form one or more curves or polycurves</summary>
    ///<param name="curveIds">(Guid seq) List of multiple curves</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input objects after joining</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>2.1 * Doc.ModelAbsoluteTolerance</c>
    ///    Join tolerance. If omitted, 2.1 * document absolute
    ///    tolerance is used</param>
    ///<returns>(Guid ResizeArray) Object objectId representing the new curves</returns>
    static member JoinCurves(curveIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        if Seq.length(curveIds)<2 then
            Error.Raise <| sprintf "RhinoScriptSyntax.CurveIds must contain at least 2 items.  curveIds:'%A' deleteInput:'%A' tolerance:'%A'" curveIds deleteInput tolerance

        let curves = resizeArray { for objectId in curveIds -> RhinoScriptSyntax.CoerceCurve objectId }
        let tolerance0 = ifZero1 tolerance (2.1 * Doc.ModelAbsoluteTolerance)
        let newcurves = Curve.JoinCurves(curves, tolerance0)
        if isNull newcurves then
            Error.Raise <| sprintf "RhinoScriptSyntax.CurveIds must contain at least 2 items.  curveIds:'%A' deleteInput:'%A' tolerance:'%A'" curveIds deleteInput tolerance

        let rc = resizeArray { for crv in newcurves -> Doc.Objects.AddCurve(crv) }
        if deleteInput then
            for objectId in curveIds do
                Doc.Objects.Delete(objectId, false) |> ignore
        Doc.Views.Redraw()
        rc



    [<Extension>]
    ///<summary>Returns a line that was fit through an array of 3D points</summary>
    ///<param name="points">(Point3d seq) A list of at least two 3D points</param>
    ///<returns>(Line) line</returns>
    static member LineFitFromPoints(points:Point3d seq) : Line =
        let rc, line = Line.TryFitLineToPoints(points)
        if rc then  line
        else Error.Raise <| sprintf "RhinoScriptSyntax.LineFitFromPoints failed.  points:'%A'" points


    [<Extension>]
    ///<summary>Makes a periodic curve non-periodic. Non-periodic curves can develop
    ///    kinks when deformed</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the input curve. If omitted, the input curve will not be deleted</param>
    ///<returns>(Guid) objectId of the new or modified curve</returns>
    static member MakeCurveNonPeriodic(curveId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if not <| curve.IsPeriodic then  Error.Raise <| sprintf "RhinoScriptSyntax.MakeCurveNonPeriodic failed.1  curveId:'%A' deleteInput:'%A'" curveId deleteInput
        let nc = curve.ToNurbsCurve()
        if isNull nc  then  Error.Raise <| sprintf "RhinoScriptSyntax.MakeCurveNonPeriodic failed.2  curveId:'%A' deleteInput:'%A'" curveId deleteInput
        if not <| nc.Knots.ClampEnd( CurveEnd.Both ) then Error.Raise <| sprintf "RhinoScriptSyntax.MakeCurveNonPeriodic failed.  curveId:'%A' deleteInput:'%A'" curveId deleteInput
        if deleteInput then
            let rc = Doc.Objects.Replace(curveId, nc)
            if not <| rc then  Error.Raise <| sprintf "RhinoScriptSyntax.MakeCurveNonPeriodic failed.3  curveId:'%A' deleteInput:'%A'" curveId deleteInput
            Doc.Views.Redraw()
            curveId
        else
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
            let rc = Doc.Objects.AddCurve(nc, rhobj.Attributes)
            if rc = Guid.Empty then  Error.Raise <| sprintf "RhinoScriptSyntax.MakeCurveNonPeriodic failed.4  curveId:'%A' deleteInput:'%A'" curveId deleteInput
            Doc.Views.Redraw()
            rc


    [<Extension>]
    ///<summary>Creates an average curve from two curves</summary>
    ///<param name="curve0">(Guid) identifiers of first curve</param>
    ///<param name="curve1">(Guid) identifiers of second curve</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Angle tolerance used to match kinks between curves</param>
    ///<returns>(Guid) objectId of the new or modified curve</returns>
    static member MeanCurve(curve0:Guid, curve1:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid =
        let curve0 = RhinoScriptSyntax.CoerceCurve curve0
        let curve1 = RhinoScriptSyntax.CoerceCurve curve1
        let  tolerance = if tolerance = 0.0 then RhinoMath.UnsetValue else abs (tolerance)
        let crv = Curve.CreateMeanCurve(curve0, curve1, tolerance)
        if notNull crv then
            let rc = Doc.Objects.AddCurve(crv)
            Doc.Views.Redraw()
            rc
        else
            Error.Raise <| sprintf "RhinoScriptSyntax.MeanCurve failed.  curve1:'%A' curve0:'%A' tolerance:'%f'" curve1 curve0 tolerance


    [<Extension>]
    ///<summary>Creates a polygon mesh object based on a closed polyline curve object.
    ///    The created mesh object is added to the document</summary>
    ///<param name="polylineId">(Guid) Identifier of the polyline curve object</param>
    ///<returns>(Guid) identifier of the new mesh object</returns>
    static member MeshPolyline(polylineId:Guid) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve polylineId
        let ispolyline, polyline = curve.TryGetPolyline()
        if not <| ispolyline then  Error.Raise <| sprintf "RhinoScriptSyntax.MeshPolyline failed.  polylineId:'%A'" polylineId
        let mesh = Mesh.CreateFromClosedPolyline(polyline)
        if isNull mesh then  Error.Raise <| sprintf "RhinoScriptSyntax.MeshPolyline failed.  polylineId:'%A'" polylineId
        let rc = Doc.Objects.AddMesh(mesh)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Offsets a curve by a distance. The offset curve will be added to Rhino</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="direction">(Point3d) Point describing direction of the offset</param>
    ///<param name="distance">(float) Distance of the offset</param>
    ///<param name="normal">(Vector3d) Optional, Default Value: <c>Vector3d.ZAxis</c>
    ///    Normal of the plane in which the offset will occur.
    ///    If omitted, the WorldXY  plane will be used</param>
    ///<param name="style">(int) Optional, Default Value: <c>1</c>
    ///    The corner style. If omitted, the style is sharp.
    ///    0 = None
    ///    1 = Sharp
    ///    2 = Round
    ///    3 = Smooth
    ///    4 = Chamfer</param>
    ///<returns>(Guid ResizeArray) list of ids for the new curves</returns>
    static member OffsetCurve(curveId:Guid, direction:Point3d, distance:float, [<OPT;DEF(Vector3d())>]normal:Vector3d, [<OPT;DEF(1)>]style:int) : Guid ResizeArray = //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let normal0 = if normal.IsZero then Vector3d.ZAxis else normal
        let curve = RhinoScriptSyntax.CoerceCurve curveId
        let tolerance = Doc.ModelAbsoluteTolerance
        let style:CurveOffsetCornerStyle = EnumOfValue style
        let curves = curve.Offset(direction, normal0, distance, tolerance, style)
        if isNull curves then  Error.Raise <| sprintf "RhinoScriptSyntax.OffsetCurve failed.  curveId:'%A' direction:'%A' distance:'%A' normal:'%A' style:'%A'" curveId direction distance normal style
        let rc = resizeArray { for curve in curves -> Doc.Objects.AddCurve(curve) }
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Offset a curve on a surface. The source curve must lie on the surface.
    ///    The offset curve or curves will be added to Rhino</summary>
    ///<param name="curveId">(Guid) Curve identifiers</param>
    ///<param name="surfaceId">(Guid) Surface identifiers</param>
    ///<param name="parameter">(Point2d) ):  U, V parameter that the curve will be offset through</param>
    ///<returns>(Guid ResizeArray) identifiers of the new curves</returns>
    static member OffsetCurveOnSurfaceUV(curveId:Guid, surfaceId:Guid, parameter:Point2d) : Guid ResizeArray =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let tol = Doc.ModelAbsoluteTolerance
        let curves = curve.OffsetOnSurface(surface, parameter, tol)
        if isNull curves  then  Error.Raise <| sprintf "RhinoScriptSyntax.OffsetCurveOnSurface failed.  curveId:'%A' surfaceId:'%A' parameter:'%A'" curveId surfaceId parameter
        let rc = resizeArray { for curve in curves -> Doc.Objects.AddCurve(curve) }
        Doc.Views.Redraw()
        rc

    [<Extension>]
    ///<summary>Offset a curve on a surface. The source curve must lie on the surface.
    ///    The offset curve or curves will be added to Rhino</summary>
    ///<param name="curveId">(Guid) Curve identifiers</param>
    ///<param name="surfaceId">(Guid) Surface identifiers</param>
    ///<param name="distance">(float) ):the distance of the offset. Based on the curve's direction, a positive value
    ///    will offset to the left and a negative value will offset to the right</param>
    ///<returns>(Guid ResizeArray) identifiers of the new curves</returns>
    static member OffsetCurveOnSurface(curveId:Guid, surfaceId:Guid, distance:float) : Guid ResizeArray =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let tol = Doc.ModelAbsoluteTolerance
        let curves = curve.OffsetOnSurface(surface, distance, tol)
        if isNull curves  then  Error.Raise <| sprintf "RhinoScriptSyntax.OffsetCurveOnSurface failed.  curveId:'%A' surfaceId:'%A' distance:'%A'" curveId surfaceId distance
        let curves = resizeArray{for curve in curves do curve.ExtendOnSurface(Rhino.Geometry.CurveEnd.Both, surface) } //https://github.com/mcneel/rhinoscriptsyntax/pull/186        
        let rc = resizeArray { for curve in curves -> Doc.Objects.AddCurve(curve) }
        Doc.Views.Redraw()
        rc



    [<Extension>]
    ///<summary>Determines the relationship between the regions bounded by two coplanar simple closed curves</summary>
    ///<param name="curveA">(Guid) identifier of the first  planar, closed curve</param>
    ///<param name="curveB">(Guid) identifier of the second planar, closed curve</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///    Test plane. If omitted, the Plane.WorldXY plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(int) a number identifying the relationship
    ///    0 = the regions bounded by the curves are disjoint
    ///    1 = the two curves intersect
    ///    2 = the region bounded by curveA is inside of curveB
    ///    3 = the region bounded by curveB is inside of curveA</returns>
    static member PlanarClosedCurveContainment(curveA:Guid, curveB:Guid, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        let curveA = RhinoScriptSyntax.CoerceCurve curveA
        let curveB = RhinoScriptSyntax.CoerceCurve curveB
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let plane0 = if plane.IsValid then plane else Plane.WorldXY
        let rc = Curve.PlanarClosedCurveRelationship(curveA, curveB, plane0, tolerance0)
        int(rc)


    [<Extension>]
    ///<summary>Determines if two coplanar curves intersect</summary>
    ///<param name="curveA">(Guid) identifier of the first  planar curve</param>
    ///<param name="curveB">(Guid) identifier of the second planar curve</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///    Test plane. If omitted, the Plane.WorldXY plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(bool) True if the curves intersect; otherwise False</returns>
    static member PlanarCurveCollision(curveA:Guid, curveB:Guid, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let curveA = RhinoScriptSyntax.CoerceCurve curveA
        let curveB = RhinoScriptSyntax.CoerceCurve curveB
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let plane0 = if plane.IsValid then plane else Plane.WorldXY
        Curve.PlanarCurveCollision(curveA, curveB, plane0, tolerance0)


    [<Extension>]
    ///<summary>Determines if a point is inside of a closed curve, on a closed curve, or
    ///    outside of a closed curve</summary>
    ///<param name="point">(Point3d) Text point</param>
    ///<param name="curve">(Guid) Identifier of a curve object</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///    Plane containing the closed curve and point. If omitted, Plane.WorldXY  is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(int) number identifying the result
    ///    0 = point is outside of the curve
    ///    1 = point is inside of the curve
    ///    2 = point is on the curve</returns>
    static member PointInPlanarClosedCurve(point:Point3d, curve:Guid, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        let curve = RhinoScriptSyntax.CoerceCurve curve
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let plane0 = if plane.IsValid then plane else Plane.WorldXY
        let rc = curve.Contains(point, plane0, tolerance0)
        if rc= PointContainment.Unset then
            Error.Raise <| sprintf "RhinoScriptSyntax.PointInPlanarClosedCurve Curve.Contains is Unset.  point:'%A' curve:'%A' plane:'%A' tolerance:'%A'" point curve plane tolerance
        if rc= PointContainment.Outside then  0
        elif rc= PointContainment.Inside then  1
        else 2


    [<Extension>]
    ///<summary>Returns the number of curve segments that make up a polycurve</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    If `curveId` identifies a polycurve object, then `segmentIndex` identifies the curve segment of the polycurve to query</param>
    ///<returns>(int) the number of curve segments in a polycurve</returns>
    static member PolyCurveCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        match curve with
        | :? PolyCurve as curve ->  curve.SegmentCount
        | _ -> Error.Raise <| sprintf "RhinoScriptSyntax.CurveId does not reference a polycurve.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex


    [<Extension>]
    ///<summary>Returns the vertices of a polyline curve</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    If curveId identifies a polycurve object, then segmentIndex identifies the curve segment of the polycurve to query</param>
    ///<returns>(Point3d ResizeArray) an list of Point3d vertex points</returns>
    static member PolylineVertices(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d ResizeArray =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let rc, polyline = curve.TryGetPolyline()
        if rc then  resizeArray { for pt in polyline -> pt }
        else Error.Raise <| sprintf "RhinoScriptSyntax.CurveId does not <| reference a polyline.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex


    [<Extension>]
    ///<summary>Projects one or more curves onto one or more surfaces or meshes</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of curves to project</param>
    ///<param name="meshIds">(Guid seq) Identifiers of meshes to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid ResizeArray) list of identifiers for the resulting curves</returns>
    static member ProjectCurveToMesh(curveIds:Guid seq, meshIds:Guid seq, direction:Vector3d) : Guid ResizeArray =
        let curves = resizeArray { for objectId in curveIds -> RhinoScriptSyntax.CoerceCurve objectId }
        let meshes = resizeArray { for objectId in meshIds -> RhinoScriptSyntax.CoerceMesh(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let newcurves = Curve.ProjectToMesh(curves, meshes, direction, tolerance)
        let ids = resizeArray { for curve in newcurves -> Doc.Objects.AddCurve(curve) }
        if ids.Count >0 then  Doc.Views.Redraw()
        ids


    [<Extension>]
    ///<summary>Projects one or more curves onto one or more surfaces or polysurfaces</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of curves to project</param>
    ///<param name="surfaceIds">(Guid seq) Identifiers of surfaces to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid ResizeArray) list of identifiers</returns>
    static member ProjectCurveToSurface(curveIds:Guid seq, surfaceIds:Guid seq, direction:Vector3d) : Guid ResizeArray =
        let curves = resizeArray { for objectId in curveIds -> RhinoScriptSyntax.CoerceCurve objectId }
        let breps = resizeArray { for objectId in surfaceIds -> RhinoScriptSyntax.CoerceBrep(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let newcurves = Curve.ProjectToBrep(curves, breps, direction, tolerance)
        let ids = resizeArray { for curve in newcurves -> Doc.Objects.AddCurve(curve) }
        if ids.Count > 0 then  Doc.Views.Redraw()
        ids


    [<Extension>]
    ///<summary>Rebuilds a curve to a given degree and control point count. For more
    ///    information, see the Rhino help for the Rebuild command</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="degree">(int) New degree (must be greater than 0)</param>
    ///<param name="pointCount">(int) New point count, which must be bigger than degree</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RebuildCurve(curveId:Guid, degree:int, pointCount:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if degree<1 then  Error.Raise <| sprintf "RhinoScriptSyntax.RebuildCurve: Degree must be greater than 0.  curveId:'%A' degree:'%A' pointCount:'%A'" curveId degree pointCount
        let newcurve = curve.Rebuild(pointCount, degree, false)
        if isNull newcurve then  false
        else
            Doc.Objects.Replace(curveId, newcurve) |> ignore
            Doc.Views.Redraw()
            true


    [<Extension>]
    ///<summary>Deletes a knot from a curve object</summary>
    ///<param name="curve">(Guid) The reference of the source object</param>
    ///<param name="parameter">(float) The parameter on the curve. Note, if the parameter is not equal to one
    ///    of the existing knots, then the knot closest to the specified parameter
    ///    will be removed</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RemoveCurveKnot(curve:Guid, parameter:float) : bool =
         let curveInst = RhinoScriptSyntax.CoerceCurve curve
         let success, nParam = curveInst.GetCurveParameterFromNurbsFormParameter(parameter)
         if not <| success then  false
         else
             let nCurve = curveInst.ToNurbsCurve()
             if isNull nCurve then  false
             else
                 let success = nCurve.Knots.RemoveKnotAt(nParam)
                 if not <| success then  false
                 else
                     Doc.Objects.Replace(curve, nCurve)|> ignore
                     Doc.Views.Redraw()
                     true


    [<Extension>]
    ///<summary>Reverses the direction of a curve object. Same as Rhino's Dir command</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ReverseCurve(curveId:Guid) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if curve.Reverse() then
            Doc.Objects.Replace(curveId, curve)|> ignore
            true
        else
            false


    [<Extension>]
    ///<summary>Replace a curve with a geometrically equivalent polycurve.
    ///    The polycurve will have the following properties:
    ///      - All the polycurve segments are lines, polylines, arcs, or NURBS curves.
    ///      - The NURBS curves segments do not have fully multiple interior knots.
    ///      - Rational NURBS curves do not have constant weights.
    ///      - Any segment for which IsCurveLinear or IsArc is True is a line, polyline segment, or an arc.
    ///      - Adjacent co-linear or co-circular segments are combined.
    ///      - Segments that meet with G1-continuity have there ends tuned up so that they meet with G1-continuity to within machine precision.
    ///      - If the polycurve is a polyline, a polyline will be created</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="flags">(int) Optional, Default Value: <c>0</c>
    ///    The simplification methods to use. By default, all methods are used (flags = 0)
    ///    Value Description
    ///    0     Use all methods.
    ///    1     Do not split NURBS curves at fully multiple knots.
    ///    2     Do not replace segments with IsCurveLinear = True with line curves.
    ///    4     Do not replace segments with IsArc = True with arc curves.
    ///    8     Do not replace rational NURBS curves with constant denominator with an equivalent non-rational NURBS curve.
    ///    16    Do not adjust curves at G1-joins.
    ///    32    Do not merge adjacent co-linear lines or co-circular arcs or combine consecutive line segments into a polyline</param>
    ///<returns>(bool) True or False</returns>
    static member SimplifyCurve(curveId:Guid, [<OPT;DEF(0)>]flags:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let mutable flags = 63
        if (flags &&& 1 )= 1  then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.SplitAtFullyMultipleKnots))
        if (flags &&& 2 )= 2  then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.RebuildLines))
        if (flags &&& 4 )= 4  then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.RebuildArcs))
        if (flags &&& 8 )= 8  then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.RebuildRationals))
        if (flags &&& 16)= 16 then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.AdjustG1))
        if (flags &&& 32)= 32 then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.Merge))
        let flags0: CurveSimplifyOptions = EnumOfValue flags
        //TODO test bitwise operations
        let tol = Doc.ModelAbsoluteTolerance
        let angTol = Doc.ModelAngleToleranceRadians
        let newcurve = curve.Simplify(flags0, tol, angTol)
        if notNull newcurve then
            Doc.Objects.Replace(curveId, newcurve)|> ignore
            Doc.Views.Redraw()
            true
        else
            false


    [<Extension>]
    ///<summary>Splits, or divides, a curve at a specified parameter. The parameter must
    ///    be in the interior of the curve's domain</summary>
    ///<param name="curveId">(Guid) The curve to split</param>
    ///<param name="parameter">(float seq) One or more parameters to split the curve at</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input curve</param>
    ///<returns>(Guid ResizeArray) list of new curves</returns>
    static member SplitCurve(curveId:Guid, parameter:float seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let newcurves = curve.Split(parameter)
        if isNull newcurves then  Error.Raise <| sprintf "RhinoScriptSyntax.SplitCurve failed.  curveId:'%A' parameter:'%A' deleteInput:'%A'" curveId parameter deleteInput
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let rc = resizeArray { for crv in newcurves -> Doc.Objects.AddCurve(crv, rhobj.Attributes) }
        if deleteInput then
            Doc.Objects.Delete(curveId, true)|> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Trims a curve by removing portions of the curve outside a specified interval</summary>
    ///<param name="curveId">(Guid) The curve to trim</param>
    ///<param name="interval">(float * float) Two numbers identifying the interval to keep. Portions of
    ///    the curve before domain[0] and after domain[1] will be removed. If the
    ///    input curve is open, the interval must be increasing. If the input
    ///    curve is closed and the interval is decreasing, then the portion of
    ///    the curve across the start and end of the curve is returned</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input curve. If omitted the input curve is deleted</param>
    ///<returns>(Guid) identifier of the new curve</returns>
    static member TrimCurve(curveId:Guid, interval:float * float, [<OPT;DEF(true)>]deleteInput:bool) : Guid  =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let newcurve = curve.Trim(fst interval, snd interval)
        if isNull newcurve then  Error.Raise <| sprintf "RhinoScriptSyntax.TrimCurve failed.  curveId:'%A' interval:'%A' deleteInput:'%A'" curveId interval deleteInput
        let att = None
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let rc = Doc.Objects.AddCurve(newcurve, rhobj.Attributes)
        if deleteInput then
            Doc.Objects.Delete(curveId, true)|> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Changes the degree of a curve object. For more information see the Rhino help file for the ChangeDegree command</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="degree">(int) The new degree</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member ChangeCurveDegree(curveId:Guid, degree:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve curveId
        let nc = curve.ToNurbsCurve()
        if degree > 2 && degree < 12 && curve.Degree <> degree then
            if not <| nc.IncreaseDegree(degree) then false
            else
                Doc.Objects.Replace(curveId, nc)
        else
            false


    [<Extension>]
    ///<summary>Creates curves between two open or closed input curves</summary>
    ///<param name="fromCurveId">(Guid) Identifier of the first curve object</param>
    ///<param name="toCurveId">(Guid) Identifier of the second curve object</param>
    ///<param name="numberOfCurves">(int) Optional, Default Value: <c>1</c>
    ///    The number of curves to create. The default is 1</param>
    ///<param name="method">(int) Optional, Default Value: <c>0</c>
    ///    The method for refining the output curves, where:
    ///    0: (Default) Uses the control points of the curves for matching. So the first control point of first curve is matched to first control point of the second curve.
    ///    1: Refits the output curves like using the FitCurve method.  Both the input curve and the output curve will have the same structure. The resulting curves are usually more complex than input unless input curves are compatible.
    ///    2: Input curves are divided to the specified number of points on the curve, corresponding points define new points that output curves go through. If you are making one tween curve, the method essentially does the following: divides the two curves into an equal number of points, finds the midpoint between the corresponding points on the curves, and interpolates the tween curve through those points</param>
    ///<param name="sampleNumber">(int) Optional, Default Value: <c>10</c>
    ///    The number of samples points to use if method is 2. The default is 10</param>
    ///<returns>(Guid ResizeArray) The identifiers of the new tween objects</returns>
    static member AddTweenCurves(fromCurveId:Guid, toCurveId:Guid, [<OPT;DEF(1)>]numberOfCurves:int, [<OPT;DEF(0)>]method:int, [<OPT;DEF(10)>]sampleNumber:int) : Guid ResizeArray =
        let curve0 = RhinoScriptSyntax.CoerceCurve fromCurveId
        let curve1 = RhinoScriptSyntax.CoerceCurve toCurveId
        let mutable outCurves = Array.empty
        let tolerance = Doc.ModelAbsoluteTolerance
        if method = 0 then
            outCurves <- Curve.CreateTweenCurves(curve0, curve1, numberOfCurves, tolerance)
            outCurves <- Curve.CreateTweenCurvesWithMatching(curve0, curve1, numberOfCurves, tolerance)
        elif method = 2 then
            outCurves <- Curve.CreateTweenCurvesWithSampling(curve0, curve1, numberOfCurves, sampleNumber, tolerance)
        else Error.Raise <| sprintf "RhinoScriptSyntax.AddTweenCurves Method must be 0, 1, || 2.  fromCurveId:'%A' toCurveId:'%A' numberOfCurves:'%A' method:'%A' sampleNumber:'%A'" fromCurveId toCurveId numberOfCurves method sampleNumber
        let curves = ResizeArray()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    //curve.Dispose()
                    if rc = Guid.Empty then  Error.Raise <| sprintf "RhinoScriptSyntax.AddTweenCurves: Unable to add curve to document.  fromCurveId:'%A' toCurveId:'%A' numberOfCurves:'%A' method:'%A' sampleNumber:'%A'" fromCurveId toCurveId numberOfCurves method sampleNumber
                    curves.Add(rc)
            Doc.Views.Redraw()
        curves

