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

    ///<summary>Adds an arc Curve to the document.</summary>
    ///<param name="plane">(Plane) Plane on which the arc will lie. The origin of the Plane will be
    ///    the center point of the arc. x-axis of the Plane defines the 0 angle
    ///    direction</param>
    ///<param name="radius">(float) Radius of the arc</param>
    ///<param name="angleDegrees">(float) Interval of arc in degrees</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddArc(plane:Plane, radius:float, angleDegrees:float) : Guid =
        let radians = toRadians(angleDegrees)
        let arc = Arc(plane, radius, radians)
        let rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddArc: Unable to add arc to document.  plane:'%A' radius:'%A' angleDegrees:'%A'" plane radius angleDegrees
        Doc.Views.Redraw()
        rc


    ///<summary>Adds a 3-point arc Curve to the document.</summary>
    ///<param name="start">(Point3d) Start of the arc</param>
    ///<param name="ende">(Point3d) Endpoint of the arc</param>
    ///<param name="pointOnArc">(Point3d) A point on the arc</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddArc3Pt(start:Point3d, ende:Point3d, pointOnArc:Point3d) : Guid =
        let arc = Arc(start, pointOnArc, ende)
        let rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddArc3Pt: Unable to add arc to document.  start:'%A' ende:'%A' pointOnArc:'%A'" start ende pointOnArc
        Doc.Views.Redraw()
        rc


    ///<summary>Adds an arc Curve, created from a start point, a start direction, and an
    ///    end point, to the document.</summary>
    ///<param name="start">(Point3d) The starting point of the arc</param>
    ///<param name="direction">(Vector3d) The arc direction at start</param>
    ///<param name="ende">(Point3d) The ending point of the arc</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddArcPtTanPt(start:Point3d, direction:Vector3d, ende:Point3d) : Guid =
        let arc = Arc(start, direction, ende)
        let rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddArcPtTanPt: Unable to add arc to document.  start:'%A' direction:'%A' ende:'%A'" start direction ende
        Doc.Views.Redraw()
        rc


    ///<summary>Makes a Curve blend between two Curves.</summary>
    ///<param name="curves">(Guid * Guid) List of two Curves</param>
    ///<param name="parameters">(float * float) List of two Curve parameters defining the blend end points</param>
    ///<param name="reverses">(bool * bool) List of two boolean values specifying to use the natural or opposite direction of the Curve</param>
    ///<param name="continuities">(int * int) List of two numbers specifying continuity at end points
    ///    0 = position
    ///    1 = tangency
    ///    2 = curvature</param>
    ///<returns>(Guid) identifier of new Curve.</returns>
    [<Extension>]
    static member AddBlendCurve(curves:Guid * Guid, parameters:float * float, reverses:bool * bool, continuities:int * int) : Guid =
        let crv0 = RhinoScriptSyntax.CoerceCurve (fst curves)
        let crv1 = RhinoScriptSyntax.CoerceCurve (snd curves)
        let c0:BlendContinuity = EnumOfValue(fst continuities)
        let c1:BlendContinuity = EnumOfValue(snd continuities)
        let curve = Curve.CreateBlendCurve(crv0, fst parameters, fst reverses, c0, crv1, snd parameters, snd reverses, c1)
        let rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then  RhinoScriptingException.Raise "RhinoScriptSyntax.AddBlendCurve: Unable to add curve to document.  curves:'%A' parameters:'%A' reverses:'%A' continuities:'%A'" curves parameters reverses continuities
        Doc.Views.Redraw()
        rc


    ///<summary>Adds a circle Curve to the document.</summary>
    ///<param name="plane">(Plane) Plane on which the circle will lie. If a point is
    ///    passed, this will be the center of the circle on the active construction Plane</param>
    ///<param name="radius">(float) The radius of the circle</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddCircle(plane:Plane, radius:float) : Guid = //(TODO add overload point)
        let circle = Circle(plane, radius)
        let rc = Doc.Objects.AddCircle(circle)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddCircle: Unable to add circle to document.  planeOrCenter:'%A' radius:'%A'" plane radius
        Doc.Views.Redraw()
        rc


    ///<summary>Adds a 3-point circle Curve to the document.</summary>
    ///<param name="first">(Point3d) First point on the circle'</param>
    ///<param name="second">(Point3d) Second point on the circle'</param>
    ///<param name="third">(Point3d) Third point on the circle'</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddCircle3Pt(first:Point3d, second:Point3d, third:Point3d) : Guid =
        let circle = Circle(first, second, third)
        let rc = Doc.Objects.AddCircle(circle)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddCircle3Pt: Unable to add circle to document.  first:'%A' second:'%A' third:'%A'" first second third
        Doc.Views.Redraw()
        rc


    ///<summary>Adds a control points Curve object to the document.</summary>
    ///<param name="points">(Point3d seq) A list of points</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///    Degree of the Curve</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddCurve(points:Point3d seq, [<OPT;DEF(3)>]degree:int) : Guid =
        let  curve = Curve.CreateControlPointCurve(points, degree)
        if isNull curve then RhinoScriptingException.Raise "RhinoScriptSyntax.AddCurve: Unable to create control point curve from given points.  points:'%A' degree:'%A'" points degree
        let  rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddCurve: Unable to add curve to document.  points:'%A' degree:'%A'" points degree
        Doc.Views.Redraw()
        rc


    ///<summary>Adds an elliptical Curve to the document.</summary>
    ///<param name="plane">(Plane) The Plane on which the ellipse will lie. The origin of
    ///    the Plane will be the center of the ellipse</param>
    ///<param name="radiusX">(float) radius in the X axis direction</param>
    ///<param name="radiusY">(float) radius in the Y axis direction</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddEllipse(plane:Plane, radiusX:float, radiusY:float) : Guid =
        let ellipse = Ellipse(plane, radiusX, radiusY)
        let rc = Doc.Objects.AddEllipse(ellipse)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddEllipse: Unable to add curve to document. plane:'%A' radiusX:'%A' radiusY:'%A'" plane radiusX radiusY
        Doc.Views.Redraw()
        rc


    ///<summary>Adds a 3-point elliptical Curve to the document.</summary>
    ///<param name="center">(Point3d) Center point of the ellipse</param>
    ///<param name="second">(Point3d) End point of the x axis</param>
    ///<param name="third">(Point3d) End point of the y axis</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddEllipse3Pt(center:Point3d, second:Point3d, third:Point3d) : Guid =
        let  ellipse = Ellipse(center, second, third)
        let  rc = Doc.Objects.AddEllipse(ellipse)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddEllipse3Pt: Unable to add curve to document.  center:'%A' second:'%A' third:'%A'" center second third
        Doc.Views.Redraw()
        rc


    ///<summary>Adds a fillet Curve between two Curve objects.</summary>
    ///<param name="curveA">(Guid) Identifier of the first Curve object</param>
    ///<param name="curveB">(Guid) Identifier of the second Curve object</param>
    ///<param name="radius">(float) Optional, Default Value: <c>1.0</c>
    ///    Fillet radius</param>
    ///<param name="basePointA">(Point3d) Optional, Base point of the first Curve. If omitted,
    ///    starting point of the Curve is used</param>
    ///<param name="basePointB">(Point3d) Optional, Base point of the second Curve. If omitted,
    ///    starting point of the Curve is used</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
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
            if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.AddFilletCurve ClosestPoint failed.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
            crv0T <- t
        let mutable crv1T = 0.0
        if basePointB= Point3d.Unset then
            crv1T <- curve1.Domain.Min
        else
            let rc, t = curve1.ClosestPoint(basePointB)
            if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.AddFilletCurve ClosestPoint failed.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
            crv1T <- t
        let mutable arc = Curve.CreateFillet(curve0, curve1, radius, crv0T, crv1T)
        let mutable rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddFilletCurve: Unable to add fillet curve to document.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
        Doc.Views.Redraw()
        rc


    ///<summary>Adds an interpolated Curve object that lies on a specified
    ///    Surface. Note, this function will not create periodic Curves,
    ///    but it will create closed Curves.</summary>
    ///<param name="surfaceId">(Guid) Identifier of the Surface to create the Curve on</param>
    ///<param name="points">(Point3d seq) List of 3D points that lie on the specified Surface.
    ///    The list must contain at least 2 points</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddInterpCrvOnSrf(surfaceId:Guid, points:Point3d seq) : Guid =
        let  surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let  tolerance = Doc.ModelAbsoluteTolerance
        let  curve = surface.InterpolatedCurveOnSurface(points, tolerance)
        if isNull curve then RhinoScriptingException.Raise "RhinoScriptSyntax.AddInterpCrvOnSrf: Unable to create InterpolatedCurveOnSurface.  surfaceId:'%s' points:'%A'" (rhType surfaceId) points
        let mutable rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddInterpCrvOnSrf: Unable to add curve to document.  surfaceId:'%s' points:'%A'" (rhType surfaceId) points
        Doc.Views.Redraw()
        rc


    ///<summary>Adds an interpolated Curve object based on Surface parameters,
    ///    that lies on a specified Surface. Note, this function will not
    ///    create periodic Curves, but it will create closed Curves.</summary>
    ///<param name="surfaceId">(Guid) Identifier of the Surface to create the Curve on</param>
    ///<param name="points">(Point2d seq) A list of 2D Surface parameters. The list must contain
    ///    at least 2 sets of parameters</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddInterpCrvOnSrfUV(surfaceId:Guid, points:Point2d seq) : Guid =
        let mutable surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let mutable tolerance = Doc.ModelAbsoluteTolerance
        let mutable curve = surface.InterpolatedCurveOnSurfaceUV(points, tolerance)
        if isNull curve then RhinoScriptingException.Raise "RhinoScriptSyntax.AddInterpCrvOnSrfUV: Unable to create InterpolatedCurveOnSurfaceUV.  surfaceId:'%s' points:'%A'" (rhType surfaceId) points
        let mutable rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddInterpCrvOnSrfUV: Unable to add curve to document.  surfaceId:'%s' points:'%A'" (rhType surfaceId) points
        Doc.Views.Redraw()
        rc


    ///<summary>Adds an interpolated Curve object to the document. Options exist to make
    ///    a periodic Curve or to specify the tangent at the endpoints. The resulting
    ///    Curve is a non-rational NURBS Curve of the specified degree.</summary>
    ///<param name="points">(Point3d seq) A list containing 3D points to interpolate. For periodic Curves,
    ///    if the final point is a duplicate of the initial point, it is
    ///    ignored. The number of control points must be bigger than 'degree' number</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>    
    ///    Periodic Curves must have a degree bigger than 1. For knotstyle = 1 or 2,
    ///    the degree must be 3. For knotstyle = 4 or 5, the degree must be odd</param>
    ///<param name="knotStyle">(int) Optional, Default Value: <c>0</c>
    ///    0 Uniform knots. Parameter spacing between consecutive knots is 1.0.
    ///    1 Chord length spacing. Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///    2 Sqrt (chord length). Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///    3 Periodic with uniform spacing.
    ///    4 Periodic with chord length spacing. Requires an odd degree value.
    ///    5 Periodic with sqrt (chord length) spacing. Requires an odd degree value</param>
    ///<param name="startTangent">(Vector3d) Optional, A vector that specifies a tangency condition at the
    ///    beginning of the Curve. If the Curve is periodic, this argument must be omitted</param>
    ///<param name="endTangent">(Vector3d) Optional, 3d vector that specifies a tangency condition at the
    ///    end of the Curve. If the Curve is periodic, this argument must be omitted</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddInterpCurve(   points:Point3d seq, 
                                    [<OPT;DEF(3)>]degree:int, 
                                    [<OPT;DEF(0)>]knotStyle:int, 
                                    [<OPT;DEF(Vector3d())>]startTangent:Vector3d,  //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
                                    [<OPT;DEF(Vector3d())>]endTangent:Vector3d) : Guid =
        let endTangent   = if endTangent.IsZero then Vector3d.Unset else endTangent
        let startTangent = if startTangent.IsZero then Vector3d.Unset else startTangent
        let knotstyle : CurveKnotStyle = EnumOfValue knotStyle
        let  curve = Curve.CreateInterpolatedCurve(points, degree, knotstyle, startTangent, endTangent)
        if isNull curve then RhinoScriptingException.Raise "RhinoScriptSyntax.AddInterpCurve: Unable to CreateInterpolatedCurve.  points:'%A' degree:'%A' knotStyle:%d startTangent:'%A' endTangent:'%A'" points degree knotStyle startTangent endTangent
        let  rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddInterpCurve: Unable to add curve to document.  points:'%A' degree:'%A' knotStyle:%d startTangent:'%A' endeTangent:'%A'" points degree knotStyle startTangent endTangent
        Doc.Views.Redraw()
        rc


    ///<summary>Adds a line Curve to the current model.</summary>
    ///<param name="start">(Point3d) Startpoint of the line</param>
    ///<param name="ende">(Point3d) Endpoint of the line</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddLine(start:Point3d, ende:Point3d) : Guid =
        let  rc = Doc.Objects.AddLine(start, ende)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddLine: Unable to add line to document.  start:'%A' ende:'%A'" start ende
        Doc.Views.Redraw()
        rc
   
    ///<summary>Creats a NURBS Curve geomety, but does not add or draw it to the document.</summary>
    ///<param name="points">(Point3d seq) A list containing 3D control points</param>
    ///<param name="knots">(float seq) Knot values for the Curve. The number of elements in knots must
    ///    equal the number of elements in points plus degree minus 1</param>
    ///<param name="degree">(int) Degree of the Curve. must be greater than of equal to 1</param>
    ///<param name="weights">(float seq) Optional, Weight values for the Curve. Number of elements should
    ///    equal the number of elements in points. Values must be greater than 0</param>
    ///<returns>(NurbsCurve) a NurbsCurve geometry.</returns>
    [<Extension>]
    static member CreateNurbsCurve(points:Point3d seq, knots:float seq, degree:int, [<OPT;DEF(null: float seq)>]weights:float seq) : NurbsCurve =
        let cvcount = Seq.length(points)
        let knotcount = cvcount + degree - 1        
        if Seq.length(knots)<>knotcount then
            RhinoScriptingException.Raise "RhinoScriptSyntax.CreateNurbsCurve:Number of elements in knots must equal the number of elements in points plus degree minus 1.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        let rational =
            if notNull weights then
                if Seq.length(weights)<>cvcount then
                    RhinoScriptingException.Raise "RhinoScriptSyntax.CreateNurbsCurve:Number of elements in weights should equal the number of elements in points.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
                true
            else
            false
        let nc = new NurbsCurve(3, rational, degree + 1, cvcount)
        for i, k in Seq.indexed knots do
            nc.Knots.[i] <- k
        if notNull weights then
            if Seq.length(weights)<>cvcount then
                RhinoScriptingException.Raise "RhinoScriptSyntax.CreateNurbsCurve:Number of elements in weights should equal the number of elements in points.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
            for i,(p, w)  in Seq.indexed (Seq.zip points weights) do
                nc.Points.SetPoint(i, p, w) |> ignore
        else
            for i, p in Seq.indexed points do
                nc.Points.SetPoint(i, p) |> ignore       
        if not nc.IsValid then RhinoScriptingException.Raise "RhinoScriptSyntax.CreateNurbsCurve:Unable to create curve.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        nc

    ///<summary>Adds a NURBS Curve object to the document.</summary>
    ///<param name="points">(Point3d seq) A list containing 3D control points</param>
    ///<param name="knots">(float seq) Knot values for the Curve. The number of elements in knots must
    ///    equal the number of elements in points plus degree minus 1</param>
    ///<param name="degree">(int) Degree of the Curve. must be greater than of equal to 1</param>
    ///<param name="weights">(float seq) Optional, Weight values for the Curve. Number of elements should
    ///    equal the number of elements in points. Values must be greater than 0</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    [<Extension>]
    static member AddNurbsCurve(points:Point3d seq, knots:float seq, degree:int, [<OPT;DEF(null: float seq)>]weights:float seq) : Guid =
        let nc = RhinoScriptSyntax.CreateNurbsCurve(points, knots, degree, weights)
        let rc = Doc.Objects.AddCurve(nc)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddNurbsCurve: Unable to add curve to document.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        Doc.Views.Redraw()
        rc


    ///<summary>Adds a Polyline Curve.</summary>
    ///<param name="points">(Point3d seq) List of 3D points. The list must contain at least two points. If the
    ///    list contains less than four points, then the first point and
    ///    last point must be different</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddPolyline(points:Point3d seq) : Guid =
        let pl = Polyline(points)
        //pl.DeleteShortSegments(Doc.ModelAbsoluteTolerance) |>ignore
        let rc = Doc.Objects.AddPolyline(pl)
        if rc = Guid.Empty then 
            for i,pt in Seq.indexed(points) do
                let d = Doc.Objects.AddTextDot(string i, pt) 
                RhinoScriptSyntax.ObjectLayer(d,"ERROR-AddPolyline",true)
            eprintf "See %d TextDots on layer 'ERROR-AddPolyline'"  (Seq.length points)
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddPolyline: Unable to add polyline to document form points:\r\n'%A'" (RhinoScriptSyntax.ToNiceString points)
        Doc.Views.Redraw()
        rc
    
    ///<summary>Adds a closed Polyline Curve , 
    ///    if the endpoint is already closer than Doc.ModelAbsoluteTolerance to the start it wil be set to start point
    ///    else an additional point will be added with the same position as start.</summary>
    ///<param name="points">(Point3d seq) List of 3D points. The list must contain at least three points.</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddPolylineClosed(points:Point3d seq) : Guid =
        let pl = Polyline(points)
        if pl.Count < 3 then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPolylineClosed: Unable to add closed polyline to document from points:\r\n'%A'" (RhinoScriptSyntax.ToNiceString points)       
        if (pl.First-pl.Last).Length <= Doc.ModelAbsoluteTolerance then 
            pl.[pl.Count-1] <- pl.First
        else
            pl.Add pl.First
        //pl.DeleteShortSegments(Doc.ModelAbsoluteTolerance) |>ignore
        let rc = Doc.Objects.AddPolyline(pl)
        if rc = Guid.Empty then 
            for i,pt in Seq.indexed(points) do
                let d = Doc.Objects.AddTextDot(string i, pt) 
                RhinoScriptSyntax.ObjectLayer(d,"ERROR-AddPolylineClosed",true)
            eprintf "See %d TextDots on layer 'ERROR-AddPolylineClosed'"  (Seq.length points)
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddPolylineClosed: Unable to add closed polyline to document.  points:'%A'" points
        Doc.Views.Redraw()
        rc

    ///<summary>Add a rectangular Curve to the document.</summary>
    ///<param name="plane">(Plane) Plane on which the rectangle will lie</param>
    ///<param name="width">(float) Width of rectangle as measured along the Plane's
    ///    x and y axes</param>
    ///<param name="height">(float) Height of rectangle as measured along the Plane's
    ///    x and y axes</param>
    ///<returns>(Guid) objectId of new rectangle.</returns>
    [<Extension>]
    static member AddRectangle(plane:Plane, width:float, height:float) : Guid =
        let rect = Rectangle3d(plane, width, height)
        let poly = rect.ToPolyline()
        let rc = Doc.Objects.AddPolyline(poly)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddRectangle: Unable to add polyline to document.  plane:'%A' width:'%A' height:'%A'" plane width height
        Doc.Views.Redraw()
        rc


    ///<summary>Adds a spiral or helical Curve to the document.</summary>
    ///<param name="point0">(Point3d) Helix axis start point or center of spiral</param>
    ///<param name="point1">(Point3d) Helix axis end point or point normal on spiral Plane</param>
    ///<param name="pitch">(float) Distance between turns. If 0, then a spiral. If > 0 then the
    ///    distance between helix "threads"</param>
    ///<param name="turns">(float) Number of turns</param>
    ///<param name="radius0">(float) Starting radius of spiral</param>
    ///<param name="radius1">(float) Optional, Ending radius of spiral. If omitted, the starting radius is used for the complete spiral</param>
    ///<returns>(Guid) objectId of new Curve.</returns>
    [<Extension>]
    static member AddSpiral(point0:Point3d, point1:Point3d, pitch:float, turns:float, radius0:float, [<OPT;DEF(0.0)>]radius1:float) : Guid =
        let dir = point1 - point0
        let plane = Plane(point0, dir)
        let point2 = point0 + plane.XAxis
        let r2 = if radius1 = 0.0 then radius0 else radius1
        let curve = NurbsCurve.CreateSpiral(point0, dir, point2, pitch, turns, radius0, r2)
        if isNull curve then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSpiral: Unable to add curve to document.  point0:'%A' point1:'%A' pitch:'%A' turns:'%A' radius0:'%A' radius1:'%A'" point0 point1 pitch turns radius0 radius1
        let rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSpiral: Unable to add curve to document.  point0:'%A' point1:'%A' pitch:'%A' turns:'%A' radius0:'%A' radius1:'%A'" point0 point1 pitch turns radius0 radius1
        Doc.Views.Redraw()
        rc


    ///<summary>Add a Curve object based on a portion, or interval of an existing Curve
    ///    object. Similar in operation to Rhino's SubCrv command.</summary>
    ///<param name="curveId">(Guid) Identifier of a closed planar Curve object</param>
    ///<param name="param0">(float) First parameters on the source Curve</param>
    ///<param name="param1">(float) Second parameters on the source Curve</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member AddSubCrv(curveId:Guid, param0:float, param1:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId)
        let trimcurve = curve.Trim(param0, param1)
        if isNull trimcurve then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSubCrv: Unable to trim curve. curveId:'%s' param0:'%A' param1:'%A'" (rhType curveId) param0 param1
        let rc = Doc.Objects.AddCurve(trimcurve)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSubCrv: Unable to add curve to document. curveId:'%s' param0:'%A' param1:'%A'" (rhType curveId) param0 param1
        Doc.Views.Redraw()
        rc


    ///<summary>Returns the angle of an arc Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    Identifies the Curve segment if CurveId identifies a polycurve</param>
    ///<returns>(float) The angle in degrees.</returns>
    [<Extension>]
    static member ArcAngle(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.Curve is not an arc. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        (!arc).AngleDegrees


    ///<summary>Returns the center point of an arc Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D center point of the arc.</returns>
    [<Extension>]
    static member ArcCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.Curve is not an arc. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex 
        (!arc).Center


    ///<summary>Returns the mid point of an arc Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D mid point of the arc.</returns>
    [<Extension>]
    static member ArcMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.Curve is not an arc. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex 
        (!arc).MidPoint


    ///<summary>Returns the radius of an arc Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The radius of the arc.</returns>
    [<Extension>]
    static member ArcRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.Curve is not an arc. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex 
        (!arc).Radius



    ///<summary>Returns the center point of a circle Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D center point of the circle.</returns>
    [<Extension>]
    static member CircleCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.Curve is not circle. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        (!circle).Center


    ///<summary>Returns the center Plane of a circle Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The 3D Plane at the center point of the circle.</returns>
    [<Extension>]
    static member CircleCenterPlane(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.Curve is not circle. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        (!circle).Plane





    ///<summary>Returns the circumference of a circle Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The circumference of the circle.</returns>
    [<Extension>]
    static member CircleCircumference(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.Curve is not circle. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        (!circle).Circumference


    ///<summary>Returns the radius of a circle Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The radius of the circle.</returns>
    [<Extension>]
    static member CircleRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.Curve is not circle. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        (!circle).Radius


    ///<summary>Closes an open Curve object by making adjustments to the end points so
    ///    they meet at a point.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Maximum allowable distance between start and end point</param>
    ///<returns>(Guid) objectId of the new Curve object.</returns>
    [<Extension>]
    static member CloseCurve(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if curve.IsClosed then  curveId
        else
            if not <| curve.MakeClosed(ifZero1 tolerance Doc.ModelAbsoluteTolerance) then  RhinoScriptingException.Raise "RhinoScriptSyntax.CloseCurve: Unable to add curve to document. curveId:'%s' tolerance:'%A'" (rhType curveId) tolerance
            let rc = Doc.Objects.AddCurve(curve)
            if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CloseCurve: Unable to add curve to document. curveId:'%s' tolerance:'%A'" (rhType curveId) tolerance
            Doc.Views.Redraw()
            rc


    ///<summary>Determine the orientation (counter-clockwise or clockwise) of a closed,
    ///    planar Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="direction">(Vector3d) Optional, Default Value: <c>Vector3d.ZAxis</c>
    ///    3d vector that identifies up, or Z axs, direction of
    ///    the Plane to test against</param>
    ///<returns>(int) 1 if the Curve's orientation is clockwise
    ///    -1 if the Curve's orientation is counter-clockwise
    ///     0 if unable to compute the Curve's orientation.</returns>
    [<Extension>]
    static member ClosedCurveOrientation(curveId:Guid, [<OPT;DEF(Vector3d())>]direction:Vector3d) : int = //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let direction0 =if direction.IsZero then Vector3d.Unset else direction
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if not <| curve.IsClosed then  0
        else
            let orientation = curve.ClosedCurveOrientation(direction0)
            int(orientation)


    ///<summary>Convert Curve to a Polyline Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>5.0</c>
    ///    The maximum angle between Curve tangents at line endpoints.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.01</c>
    ///    The distance tolerance at segment midpoints.</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the Curve object specified by CurveId. If omitted, CurveId will not be deleted</param>
    ///<param name="minEdgeLength">(float) Optional, Minimum segment length</param>
    ///<param name="maxEdgeLength">(float) Optional, Maximum segment length</param>
    ///<returns>(Guid) The new Curve.</returns>
    [<Extension>]
    static member ConvertCurveToPolyline(curveId:Guid, [<OPT;DEF(0.0)>]angleTolerance:float, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(false)>]deleteInput:bool, [<OPT;DEF(0.0)>]minEdgeLength:float, [<OPT;DEF(0.0)>]maxEdgeLength:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let angleTolerance0 = toRadians (ifZero1 angleTolerance 0.5 )
        let tolerance0 = ifZero1 tolerance 0.01
        let polylineCurve = curve.ToPolyline( 0, 0, angleTolerance0, 0.0, 0.0, tolerance0, minEdgeLength, maxEdgeLength, true) //TODO what happens on 0.0 input ?
        if isNull polylineCurve then RhinoScriptingException.Raise "RhinoScriptSyntax.ConvertCurveToPolyline: Unable to convertCurveToPolyline %A , maxEdgeLength%f, minEdgeLength:%f, deleteInput%b, tolerance%f, angleTolerance %f " curveId   maxEdgeLength minEdgeLength deleteInput tolerance angleTolerance
        if deleteInput then
            if Doc.Objects.Replace( curveId, polylineCurve) then
                curveId
            else
                RhinoScriptingException.Raise "RhinoScriptSyntax.ConvertCurveToPolyline: Unable to convertCurveToPolyline %A , maxEdgeLength%f, minEdgeLength:%f, deleteInput%b, tolerance%f, angleTolerance %f " curveId   maxEdgeLength minEdgeLength deleteInput tolerance angleTolerance
        else
            let objectId = Doc.Objects.AddCurve( polylineCurve )
            if System.Guid.Empty= objectId then
                RhinoScriptingException.Raise "RhinoScriptSyntax.ConvertCurveToPolyline: Unable to convertCurveToPolyline %A , maxEdgeLength%f, minEdgeLength:%f, deleteInput%b, tolerance%f, angleTolerance %f " curveId   maxEdgeLength minEdgeLength deleteInput tolerance angleTolerance
            else
                objectId


    ///<summary>Returns the point on the Curve that is a specified arc length
    ///    from the start of the Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="length">(float) The arc length from the start of the Curve to evaluate</param>
    ///<param name="fromStart">(bool) Optional, Default Value: <c>true</c>
    ///    If not specified or True, then the arc length point is
    ///    calculated from the start of the Curve. If False, the arc length
    ///    point is calculated from the end of the Curve</param>
    ///<returns>(Point3d) on Curve.</returns>
    [<Extension>]
    static member CurveArcLengthPoint(curveId:Guid, length:float, [<OPT;DEF(true)>]fromStart:bool) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let curveLength = curve.GetLength()
        if curveLength >= length then
            let mutable s = 0.0
            if length = 0.0 then  s <- 0.0
            elif abs(length-curveLength) < Doc.ModelAbsoluteTolerance then  s <- 1.0
            else s <- length / curveLength
            let dupe = if not fromStart then curve.Duplicate() :?> Curve else curve
            if notNull dupe then
                if not fromStart then  dupe.Reverse() |> ignore
                let rc, t = dupe.NormalizedLengthParameter(s)
                if rc then
                    let pt = dupe.PointAt(t)
                    if not fromStart then dupe.Dispose()
                    pt
                else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveArcLengthPoint: Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveId length fromStart
            else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveArcLengthPoint: Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveId length fromStart
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveArcLengthPoint: Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveId length fromStart


    ///<summary>Returns area of closed planar Curves. The results are based on the
    ///    current drawing units.</summary>
    ///<param name="curveId">(Guid) The identifier of a closed, planar Curve object</param>
    ///<returns>(float) The area.</returns>
    [<Extension>]
    static member CurveArea(curveId:Guid) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let tol = Doc.ModelAbsoluteTolerance
        let mp = AreaMassProperties.Compute(curve, tol)
        if isNull mp  then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveArea failed on %A" curveId
        mp.Area


    ///<summary>Returns area centroid of closed, planar Curves. The results are based
    ///    on the current drawing units.</summary>
    ///<param name="curveId">(Guid) The identifier of a closed, planar Curve object</param>
    ///<returns>(Point3d ) The 3d centroid point.</returns>
    [<Extension>]
    static member CurveAreaCentroid(curveId:Guid) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let tol = Doc.ModelAbsoluteTolerance
        let mp = AreaMassProperties.Compute(curve, tol)
        if isNull mp  then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveAreaCentroid failed on %A" curveId
        mp.Centroid


    ///<summary>Get status of a Curve object's annotation arrows.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve</param>
    ///<returns>(int) The current annotation arrow style
    ///    0 = no arrows
    ///    1 = display arrow at start of Curve
    ///    2 = display arrow at end of Curve
    ///    3 = display arrow at both start and end of Curve.</returns>
    [<Extension>]
    static member CurveArrows(curveId:Guid) : int = //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let attr = rhobj.Attributes
        let rc = attr.ObjectDecoration
        if rc= DocObjects.ObjectDecoration.None then  0
        elif rc= DocObjects.ObjectDecoration.StartArrowhead then 1
        elif rc= DocObjects.ObjectDecoration.EndArrowhead then 2
        elif rc= DocObjects.ObjectDecoration.BothArrowhead then 3
        else -1

    ///<summary>Enables or disables a Curve object's annotation arrows.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve</param>
    ///<param name="arrowStyle">(int) The style of annotation arrow to be displayed.
    ///    0 = no arrows
    ///    1 = display arrow at start of Curve
    ///    2 = display arrow at end of Curve
    ///    3 = display arrow at both start and end of Curve</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
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
           RhinoScriptingException.Raise "RhinoScriptSyntax.CurveArrows style %d invalid" arrowStyle

    ///<summary>Enables or disables multiple Curve objects's annotation arrows.</summary>
    ///<param name="curveIds">(Guid seq) Identifier of multiple Curve</param>
    ///<param name="arrowStyle">(int) The style of annotation arrow to be displayed.
    ///    0 = no arrows
    ///    1 = display arrow at start of Curve
    ///    2 = display arrow at end of Curve
    ///    3 = display arrow at both start and end of Curve</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
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
                if not <| Doc.Objects.ModifyAttributes(curveId, attr, true) then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveArrows style %d invalid" arrowStyle              
            else
               RhinoScriptingException.Raise "RhinoScriptSyntax.Curve Arrow style %d invalid" arrowStyle
        Doc.Views.Redraw()

    ///<summary>Calculates the difference between two closed, planar Curves and
    ///    adds the results to the document. Note, Curves must be coplanar.</summary>
    ///<param name="curveA">(Guid) Identifier of the first Curve object</param>
    ///<param name="curveB">(Guid) Identifier of the second Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    ///<returns>(Guid Rarr) The identifiers of the new objects.</returns>
    [<Extension>]
    static member CurveBooleanDifference(curveA:Guid, curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB
        let tolerance = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanDifference(curve0, curve1, tolerance)
        let curves = Rarr()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveBooleanDifference: Unable to add curve to document.  curveA:'%A' curveB:'%A'" curveA curveB
                    curves.Add(rc)
            Doc.Views.Redraw()
            curves
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.CurveBooleanDifference: Unable to add curve to document.  curveA:'%A' curveB:'%A'" curveA curveB


    ///<summary>Calculates the intersection of two closed, planar Curves and adds
    ///    the results to the document. Note, Curves must be coplanar.</summary>
    ///<param name="curveA">(Guid) Identifier of the first Curve object</param>
    ///<param name="curveB">(Guid) Identifier of the second Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    ///<returns>(Guid Rarr) The identifiers of the new objects.</returns>
    [<Extension>]
    static member CurveBooleanIntersection(curveA:Guid, curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB
        let tolerance = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanIntersection(curve0, curve1, tolerance)
        let curves = Rarr()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveBooleanIntersection: Unable to add curve to document.  curveA:'%A' curveB:'%A'" curveA curveB
                    curves.Add(rc)
            Doc.Views.Redraw()
            curves
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.CurveBooleanIntersection: Unable to add curve to document.  curveA:'%A' curveB:'%A'" curveA curveB


    ///<summary>Calculate the union of two or more closed, planar Curves and
    ///    add the results to the document. Note, Curves must be coplanar.</summary>
    ///<param name="curveIds">(Guid seq) List of two or more close planar Curves identifiers</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    ///<returns>(Guid Rarr) The identifiers of the new objects.</returns>
    [<Extension>]
    static member CurveBooleanUnion(curveIds:Guid seq, [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr =
        let inCurves = rarr { for objectId in curveIds -> RhinoScriptSyntax.CoerceCurve objectId }
        if inCurves.Count < 2 then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveBooleanUnion:curveIds must have at least 2 curves %A" curveIds
        let tolerance = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanUnion(inCurves, tolerance)
        let curves = Rarr()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveBooleanUnion: Unable to add curve to document.  curveIds:'%s'" (RhinoScriptSyntax.ToNiceString curveIds)
                    curves.Add(rc)
            Doc.Views.Redraw()
        curves 


    ///<summary>Intersects a Curve object with a brep object. Note, unlike the
    ///    CurveSurfaceIntersection function, this function works on trimmed Surfaces.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="brepId">(Guid) Identifier of a brep object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Distance tolerance at segment midpoints.</param>
    ///<returns>(Point3d Rarr * Curve Rarr) List of points and List of Curves.</returns>
    [<Extension>]
    static member CurveBrepIntersect(curveId:Guid, brepId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Point3d Rarr * Curve Rarr=
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let brep = RhinoScriptSyntax.CoerceBrep(brepId)
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let rc, outCurves, outPoints = Intersect.Intersection.CurveBrep(curve, brep, tolerance0)
        if not <| rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurveBrepIntersect: Intersection failed. curveId:'%s' brepId:'%s' tolerance:'%A'" (rhType curveId) (rhType brepId) tolerance

        let curves = Rarr(0)
        let points = Rarr(0)
        for curve in outCurves do
            if notNull curve && curve.IsValid then
                curves.Add(curve)
                //let rc = Doc.Objects.AddCurve(curve)
                //curve.Dispose()
                //if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveBrepIntersect: Unable to add curve to document. curveId:'%s' brepId:'%s' tolerance:'%A'" (rhType curveId) brepId tolerance
                //curves.Add(rc)
        for point in outPoints do
            if point.IsValid then
                points.Add(point)
                //let rc = Doc.Objects.AddPoint(point)
                //points.Add(rc)
        //Doc.Views.Redraw()
        points, curves //TODO or  Guid as originaly done ??


    ///<summary>Returns the 3D point locations on two objects where they are closest to
    ///    each other. Note, this function provides similar functionality to that of
    ///    Rhino's ClosestPt command.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object to test</param>
    ///<param name="objectIds">(Guid seq) List of identifiers of point cloud, Curve, Surface, or
    ///    Polysurface to test against</param>
    ///<returns>(Guid * Point3d * Point3d) containing the results of the closest point calculation.
    ///    The elements are as follows:
    ///      [0]    The identifier of the closest object.
    ///      [1]    The 3-D point that is closest to the closest object.
    ///      [2]    The 3-D point that is closest to the test Curve.</returns>
    [<Extension>]
    static member CurveClosestObject(curveId:Guid, objectIds:Guid seq) : Guid * Point3d * Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let geometry = Rarr()
        for curveId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
            geometry.Add( rhobj.Geometry )
        if Seq.isEmpty geometry then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveClosestObject: objectIds must contain at least one item. curveId:'%s' objectIds:'%s'" (rhType curveId) (RhinoScriptSyntax.ToNiceString objectIds)
        let curvePoint = ref Point3d.Unset
        let geomPoint  = ref Point3d.Unset
        let whichGeom = ref 0
        let success = curve.ClosestPoints(geometry, curvePoint, geomPoint, whichGeom)
        if success then  objectIds|> Seq.item !whichGeom, !geomPoint, !curvePoint
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveClosestObject failed  curveId:'%s' objectIds:'%A'" (rhType curveId) objectIds

    ///<summary>Returns the 3D point locations on the Curve and finite line where they are closest to
    ///    each other. Note, this function provides similar functionality to that of
    ///    Rhino's ClosestPt command.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object to test</param>
    ///<param name="line">(Line) a Line Geometry</param>
    ///<returns>(Point3d * Point3d) first point on Curve, second point on Line.</returns>
    [<Extension>]
    static member CurveLineClosestPoint(curveId:Guid, line:Line) : Point3d * Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)       
        let curvePoint = ref Point3d.Unset
        let linePoint  = ref Point3d.Unset
        let success = curve.ClosestPoints(line.ToNurbsCurve(), curvePoint, linePoint)
        if success then  !linePoint, !curvePoint
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveLineClosestPoint failed  curveId:'%s' Line:'%A'" (rhType curveId) line



    ///<summary>Returns parameter of the point on a Curve that is closest to a test point.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="point">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The parameter of the closest point on the Curve.</returns>
    [<Extension>]
    static member CurveClosestParameter(curveId:Guid, point:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let t = ref 0.
        let rc = curve.ClosestPoint(point, t)
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveClosestParameter failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        !t

    ///<summary>Returns the point on a Curve that is closest to a test point.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="point">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The closest point on the Curve.</returns>
    [<Extension>]
    static member CurveClosestPoint(curveId:Guid, point:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let rc, t = curve.ClosestPoint(point)
        if not <| rc then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveClosestPoint failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        curve.PointAt(t)




    ///<summary>Returns the 3D point locations calculated by contouring a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="startPoint">(Point3d) 3D starting point of a center line</param>
    ///<param name="endPoint">(Point3d) 3D ending point of a center line</param>
    ///<param name="interval">(float) The distance between contour Curves</param>
    ///<returns>(Point3d array) A list of 3D points, one for each contour.</returns>
    [<Extension>]
    static member CurveContourPoints(curveId:Guid, startPoint:Point3d, endPoint:Point3d, interval:float) : array<Point3d> =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if startPoint.DistanceTo(endPoint)<RhinoMath.ZeroTolerance then
            RhinoScriptingException.Raise "RhinoScriptSyntax.Start && ende point are too close to define a line. curveId:'%s' startPoint:'%A' endPoint:'%A'" (rhType curveId) startPoint endPoint
        curve.DivideAsContour( startPoint, endPoint, interval)


    ///<summary>Returns the curvature of a Curve at a parameter. See the Rhino help for
    ///    details on Curve curvature.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<returns>(Point3d * Vector3d * Point3d * float * Vector3d) of curvature information
    ///    [0] = point at specified parameter
    ///    [1] = tangent vector
    ///    [2] = center of radius of curvature
    ///    [3] = radius of curvature
    ///    [4] = curvature vector.</returns>
    [<Extension>]
    static member CurveCurvature(curveId:Guid, parameter:float) : Point3d * Vector3d * Point3d * float * Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let point = curve.PointAt(parameter)
        let tangent = curve.TangentAt(parameter)
        if tangent.IsTiny(1e-10) then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurveCurvature: failed on tangent that is too small %A" curveId
        let cv = curve.CurvatureAt(parameter)
        let k = cv.Length
        if k<RhinoMath.SqrtEpsilon then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurveCurvature: failed on tangent that is too small %A" curveId
        let rv = cv / (k*k)
        let circle = Circle(point, tangent, point + 2.0*rv)
        let center = point + rv
        let radius = circle.Radius
        point, tangent, center, radius, cv


    ///<summary>Calculates intersection of two Curve objects.</summary>
    ///<param name="curveA">(Guid) Identifier of the first Curve object</param>
    ///<param name="curveB">(Guid) Optional, Identifier of the second Curve object. If omitted, then a
    ///    self-intersection test will be performed on CurveA</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Absolute tolerance in drawing units. If omitted,
    ///    the document's current absolute tolerance is used</param>
    ///<returns>( a Rarr of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float)
    ///    List of tuples: containing intersection information .
    ///    The list will contain one or more of the following elements:
    ///      Element Type     Description
    ///      [n][0]  Number   The intersection event type, either Point (1) or Overlap (2).
    ///      [n][1]  Point3d  If the event type is Point (1), then the intersection point
    ///        on the first Curve. If the event type is Overlap (2), then
    ///        intersection start point on the first Curve.
    ///      [n][2]  Point3d  If the event type is Point (1), then the intersection point
    ///        on the first Curve. If the event type is Overlap (2), then
    ///        intersection end point on the first Curve.
    ///      [n][3]  Point3d  If the event type is Point (1), then the intersection point
    ///        on the second Curve. If the event type is Overlap (2), then
    ///        intersection start point on the second Curve.
    ///      [n][4]  Point3d  If the event type is Point (1), then the intersection point
    ///        on the second Curve. If the event type is Overlap (2), then
    ///        intersection end point on the second Curve.
    ///      [n][5]  Number   If the event type is Point (1), then the first Curve parameter.
    ///        If the event type is Overlap (2), then the start value of the
    ///        first Curve parameter range.
    ///      [n][6]  Number   If the event type is Point (1), then the first Curve parameter.
    ///        If the event type is Overlap (2), then the end value of the
    ///        first Curve parameter range.
    ///      [n][7]  Number   If the event type is Point (1), then the second Curve parameter.
    ///        If the event type is Overlap (2), then the start value of the
    ///        second Curve parameter range.
    ///      [n][8]  Number   If the event type is Point (1), then the second Curve parameter.
    ///        If the event type is Overlap (2), then the end value of the
    ///        second Curve parameter range.</returns>
    [<Extension>]
    static member CurveCurveIntersection(curveA:Guid, [<OPT;DEF(Guid())>]curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) Rarr =
        let curve1 = RhinoScriptSyntax.CoerceCurve curveA
        let curve2 = if curveB= Guid.Empty then curve1 else RhinoScriptSyntax.CoerceCurve curveB
        let tolerance0 = ifZero1 tolerance Doc.ModelAbsoluteTolerance
        let mutable rc = null
        if curveB<>curveA then
            rc <- Intersect.Intersection.CurveCurve(curve1, curve2, tolerance0, Doc.ModelAbsoluteTolerance)
        else
            rc <- Intersect.Intersection.CurveSelf(curve1, tolerance0)
        if isNull rc then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveCurveIntersection faile dor %A; %A tolerance %f" curveB curveA tolerance
        let events = Rarr()
        for i =0 to rc.Count-1 do
            let mutable eventType = 1
            if( rc.[i].IsOverlap ) then  eventType <- 2
            let oa = rc.[i].OverlapA
            let ob = rc.[i].OverlapB
            let element = (eventType, rc.[i].PointA, rc.[i].PointA2, rc.[i].PointB, rc.[i].PointB2, oa.[0], oa.[1], ob.[0], ob.[1])
            events.Add(element)
        events



    ///<summary>Returns the degree of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(int) The degree of the Curve.</returns>
    [<Extension>]
    static member CurveDegree(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.Degree


    ///<summary>Returns the minimum and maximum deviation between two Curve objects.</summary>
    ///<param name="curveA">(Guid) first Curve</param>
    ///<param name="curveB">(Guid) second Curve</param>
    ///<returns>(float * float * float * float * float * float) of deviation information
    ///    [0] = CurveA parameter at maximum overlap distance point
    ///    [1] = CurveB parameter at maximum overlap distance point
    ///    [2] = maximum overlap distance
    ///    [3] = CurveAparameter at minimum overlap distance point
    ///    [4] = CurveB parameter at minimum overlap distance point
    ///    [5] = minimum distance between Curves.</returns>
    [<Extension>]
    static member CurveDeviation(curveA:Guid, curveB:Guid) : float * float * float * float * float * float =
        let curveA = RhinoScriptSyntax.CoerceCurve curveA
        let curveB = RhinoScriptSyntax.CoerceCurve curveB
        let tol = Doc.ModelAbsoluteTolerance
        let ok, maxa, maxb, maxd, mina, minb, mind = Curve.GetDistancesBetweenCurves(curveA, curveB, tol)
        if not ok then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurveDeviation failed for %A; %A" curveB curveA
        else
            maxa, maxb, maxd, mina, minb, mind


    ///<summary>Returns the dimension of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    The Curve segment if CurveId identifies a polycurve</param>
    ///<returns>(int) The dimension of the Curve .</returns>
    [<Extension>]
    static member CurveDim(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.Dimension


    ///<summary>Tests if two Curve objects are generally in the same direction or if they
    ///    would be more in the same direction if one of them were flipped. When testing
    ///    Curve directions, both Curves must be either open or closed - you cannot test
    ///    one open Curve and one closed Curve.</summary>
    ///<param name="curveA">(Guid) Identifier of first Curve object</param>
    ///<param name="curveB">(Guid) Identifier of second Curve object</param>
    ///<returns>(bool) True if the Curve directions match, otherwise False.</returns>
    [<Extension>]
    static member CurveDirectionsMatch(curveA:Guid, curveB:Guid) : bool =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB
        Curve.DoDirectionsMatch(curve0, curve1)


    ///<summary>Search for a derivatitive, tangent, or curvature discontinuity in
    ///    a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of Curve object</param>
    ///<param name="style">(int) The type of continuity to test for. The types of
    ///    continuity are as follows:
    ///    Value    Description
    ///    1        C0 - Continuous function
    ///    2        C1 - Continuous first derivative
    ///    3        C2 - Continuous first and second derivative
    ///    4        G1 - Continuous unit tangent
    ///    5        G2 - Continuous unit tangent and curvature</param>
    ///<returns>(Point3d Rarr) 3D points where the Curve is discontinuous.</returns>
    [<Extension>]
    static member CurveDiscontinuity(curveId:Guid, style:int) : Point3d Rarr =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let dom = curve.Domain
        let mutable t0 = dom.Min
        let t1 = dom.Max
        let points = Rarr()
        let mutable getNext = true
        while getNext do
            let st : Continuity = EnumOfValue style
            let getN, t = curve.GetNextDiscontinuity(st, t0, t1)
            getNext <- getN
            if getNext then
                points.Add(curve.PointAt(t))
                t0 <- t // Advance to the next parameter
        points


    ///<summary>Returns the domain of a Curve object
    ///    as an indexable object with two elements.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Interval) The domain of the Curve.</returns>
    [<Extension>]
    static member CurveDomain(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Interval =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.Domain


    ///<summary>Returns the edit, or Greville, points of a Curve object.
    ///    For each Curve control point, there is a corresponding edit point.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="returnParameters">(bool) Optional, Default Value: <c>false</c>
    ///    If True, return as a list of Curve parameters.
    ///    If False, return as a list of 3d points</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index is `curveId` identifies a polycurve</param>
    ///<returns>(Collections.Point3dList) Curve edit points.</returns>
    [<Extension>]
    static member CurveEditPoints(curveId:Guid, [<OPT;DEF(false)>]returnParameters:bool, [<OPT;DEF(-1)>]segmentIndex:int) : Collections.Point3dList =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let nc = curve.ToNurbsCurve()
        if isNull nc then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurveEditPoints faile for %A" curveId
        nc.GrevillePoints()


    ///<summary>Returns the end point of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3d endpoint of the Curve.</returns>
    [<Extension>]
    static member CurveEndPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.PointAtEnd

    ///<summary>Returns the tangent at end point of a Curve object
    /// pointing away from the Curve .</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) The tangent, same as Curve.TangentAtEnd property .</returns>
    [<Extension>]
    static member CurveEndTangent(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.TangentAtEnd

    ///<summary>Returns the tangent at start point of a Curve object
    /// pointing in direction of  the Curve .</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) The tangent, same as Curve.TangentAtStart property .</returns>
    [<Extension>]
    static member CurveStartTangent(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.TangentAtStart

    ///<summary>Find points at which to cut a pair of Curves so that a fillet of a
    ///    specified radius fits. A fillet point is a pair of points (point0, point1)
    ///    such that there is a circle of radius tangent to Curve Curve0 at point0 and
    ///    tangent to Curve Curve1 at point1. Of all possible fillet points, this
    ///    function returns the one which is the closest to the base point basePointA,
    ///    basePointB. Distance from the base point is measured by the sum of arc
    ///    lengths along the two Curves.</summary>
    ///<param name="curveA">(Guid) Identifier of the first Curve object</param>
    ///<param name="curveB">(Guid) Identifier of the second Curve object</param>
    ///<param name="radius">(float) The fillet radius</param>
    ///<param name="basePointA">(Point3d) Optional, The base point on the first Curve.
    ///    If omitted, the starting point of the Curve is used</param>
    ///<param name="basePointB">(Point3d) Optional, The base point on the second Curve. If omitted,
    ///    the starting point of the Curve is used</param>
    ///<returns>(Point3d * Point3d * Plane)
    ///    . The list elements are as follows:
    ///      [0]    A point on the first Curve at which to cut (point).
    ///      [1]    A point on the second Curve at which to cut (point).
    ///      [2]    The fillet Plane.</returns>
    [<Extension>]
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
                if not ok then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveFilletPoints failed 1 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
                t
            else
                let distEnde  = min  (distance curve1.PointAtStart curve0.PointAtEnd)   (distance curve1.PointAtEnd curve0.PointAtEnd)
                let distStart = min  (distance curve1.PointAtStart curve0.PointAtStart) (distance curve1.PointAtEnd curve0.PointAtStart)
                if distStart < distEnde then curve0.Domain.Min else curve0.Domain.Max

        let t1Base =
            if basePointB <> Point3d.Unset then
                let ok, t = curve1.ClosestPoint(basePointB)
                if not ok then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveFilletPoints failed 2 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
                t
            else
                let distEnde  = min  (distance curve0.PointAtStart curve1.PointAtEnd)   (distance curve0.PointAtEnd curve1.PointAtEnd)
                let distStart = min  (distance curve0.PointAtStart curve1.PointAtStart) (distance curve0.PointAtEnd curve1.PointAtStart)
                if distStart < distEnde then curve1.Domain.Min else curve1.Domain.Max

        let ok, a, b, pl = Curve.GetFilletPoints(curve0, curve1, radius, t0Base, t1Base)
        if not ok then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveFilletPoints failed 3 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
        curve0.PointAt(a), curve0.PointAt(b), pl


    ///<summary>Returns the Plane at a parameter of a Curve. The Plane is based on the
    ///    tangent and curvature vectors at a parameter.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The Plane at the specified parameter.</returns>
    [<Extension>]
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
                RhinoScriptingException.Raise "RhinoScriptSyntax.CurveFrame failed. curveId:'%s' parameter:'%A' segmentIndex:'%A'" (rhType curveId) parameter segmentIndex
        let  rc, frame = curve.FrameAt(para)
        if rc && frame.IsValid then  frame
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveFrame failed. curveId:'%s' parameter:'%A' segmentIndex:'%A'" (rhType curveId) parameter segmentIndex


    ///<summary>Returns the knot count of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(int) The number of knots.</returns>
    [<Extension>]
    static member CurveKnotCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let  nc = curve.ToNurbsCurve()
        if notNull nc then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurveKnotCount failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        nc.Knots.Count


    ///<summary>Returns the knots, or knot vector, of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float Rarr) knot values.</returns>
    [<Extension>]
    static member CurveKnots(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Rarr<float> =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let  nc = curve.ToNurbsCurve()
        if notNull nc then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurveKnots failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        rarr{ for i = 0 to nc.Knots.Count - 1 do yield nc.Knots.[i] }



    ///<summary>Returns the length of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="subDomain">(Interval) Optional, List of two numbers identifying the sub-domain of the
    ///    Curve on which the calculation will be performed. The two parameters
    ///    (sub-domain) must be non-decreasing. If omitted, the length of the
    ///    entire Curve is returned</param>
    ///<returns>(float) The length of the Curve.</returns>
    [<Extension>]
    static member CurveLength(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int, [<OPT;DEF(Interval())>]subDomain:Interval) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        if subDomain.T0 = 0.0 && subDomain.T1 = 0.0 then curve.GetLength() 
        else curve.GetLength(subDomain)


    ///<summary>Returns the average unitized direction of a Curve object between start and end point, 
    ///    Optionally allows non linear Curves too.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="allowNonLinear">(bool) Optional, allow non linear Curves, Default Value <c>False</c> </param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) The direction of the Curve.</returns>
    [<Extension>]
    static member CurveDirection(curveId:Guid, [<OPT;DEF(false)>]allowNonLinear:bool,[<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        if allowNonLinear || curve.IsLinear(RhinoMath.ZeroTolerance) then
            if curve.IsClosed || curve.IsClosable(Doc.ModelAbsoluteTolerance) then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveDirection failed on closed or closable curve. curveId:'%s' allowNonLinear '%A' segmentIndex:'%A'" (rhType curveId) allowNonLinear segmentIndex
            let v = curve.PointAtEnd - curve.PointAtStart
            v.Unitized
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.CurveDirection failed. curveId:'%s' allowNonLinear '%A' segmentIndex:'%A'" (rhType curveId) allowNonLinear segmentIndex


    ///<summary>Returns the mid point of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D midpoint of the Curve.</returns>
    [<Extension>]
    static member CurveMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let  rc, t = curve.NormalizedLengthParameter(0.5)
        if rc then  curve.PointAt(t)
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveMidPoint failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex


    ///<summary>Returns the normal direction of the Plane in which a planar Curve object lies.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment if CurveId identifies a polycurve</param>
    ///<returns>(Vector3d) The 3D normal vector.</returns>
    [<Extension>]
    static member CurveNormal(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let tol = Doc.ModelAbsoluteTolerance
        let plane = ref Plane.WorldXY
        let rc = curve.TryGetPlane(plane, tol)
        if rc then  (!plane).Normal
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveNormal failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex


    ///<summary>Converts a Curve parameter to a normalized Curve parameter;
    ///    one that ranges between 0-1.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="parameter">(float) The Curve parameter to convert</param>
    ///<returns>(float) normalized Curve parameter.</returns>
    [<Extension>]
    static member CurveNormalizedParameter(curveId:Guid, parameter:float) : float =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        curve.Domain.NormalizedParameterAt(parameter)


    ///<summary>Converts a normalized Curve parameter to a Curve parameter;
    ///    one within the Curve's domain.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="parameter">(float) The normalized Curve parameter to convert</param>
    ///<returns>(float) Curve parameter.</returns>
    [<Extension>]
    static member CurveParameter(curveId:Guid, parameter:float) : float =
        let curve = RhinoScriptSyntax.CoerceCurve curveId
        curve.Domain.ParameterAt(parameter)


    ///<summary>Returns the perpendicular Plane at a parameter of a Curve. The result
    ///    is relatively parallel (zero-twisting) Plane.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<returns>(Plane) Plane.</returns>
    [<Extension>]
    static member CurvePerpFrame(curveId:Guid, parameter:float) : Plane =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        let  rc, plane = curve.PerpendicularFrameAt(parameter)
        if rc then  plane else RhinoScriptingException.Raise "RhinoScriptSyntax.CurvePerpFrame failed. curveId:'%s' parameter:'%f'"  (rhType curveId) parameter


    ///<summary>Returns the Plane in which a planar Curve lies. Note, this function works
    ///    only on planar Curves.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The Plane in which the Curve lies.</returns>
    [<Extension>]
    static member CurvePlane(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let tol = Doc.ModelAbsoluteTolerance
        let plane = ref Plane.WorldXY
        let rc = curve.TryGetPlane(plane, tol)
        if rc then  !plane
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurvePlane failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex


    ///<summary>Returns the control points count of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(int) Number of control points.</returns>
    [<Extension>]
    static member CurvePointCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let mutable nc = curve.ToNurbsCurve()
        if notNull nc then  nc.Points.Count
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurvePointCount failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
    
    ///<summary>Returns the control points, or control vertices, of a Curve object.
    ///    If the Curve is a rational NURBS Curve, the euclidean control vertices
    ///    are returned.</summary>
    ///<param name="curve">(Curve) The Curve Geometry</param>
    ///<returns>(Point3d Rarr) The control points, or control vertices, of a Curve object.</returns>
    [<Extension>]
    static member CurvePoints(curve:Curve) : Point3d Rarr = 
        match curve with
        | :? PolylineCurve as pl -> 
            rarr { for i = 0 to pl.PointCount - 1 do pl.Point(i)}
        | :? NurbsCurve as nc -> 
            rarr { for i = 0 to nc.Points.Count-1 do nc.Points.[i].Location }
        | _ -> 
            let nc = curve.ToNurbsCurve()
            if isNull nc then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurvePoints failed. curve:'%A'" curve
            rarr { for i = 0 to nc.Points.Count-1 do nc.Points.[i].Location }

    ///<summary>Returns the control points, or control vertices, of a Curve object.
    ///    If the Curve is a rational NURBS Curve, the euclidean control vertices
    ///    are returned.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d Rarr) The control points, or control vertices, of a Curve object.</returns>
    [<Extension>]
    static member CurvePoints(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d Rarr =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        match curve with
        | :? PolylineCurve as pl -> 
            rarr { for i = 0 to pl.PointCount - 1 do pl.Point(i)}
        | :? NurbsCurve as nc -> 
            rarr { for i = 0 to nc.Points.Count-1 do nc.Points.[i].Location }
        | _ -> 
            let nc = curve.ToNurbsCurve()
            if isNull nc then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurvePoints failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
            rarr { for i = 0 to nc.Points.Count-1 do nc.Points.[i].Location }
        

    ///<summary>Returns the radius of curvature at a point on a Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="testPoint">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment if CurveId identifies a polycurve</param>
    ///<returns>(float) The radius of curvature at the point on the Curve.</returns>
    [<Extension>]
    static member CurveRadius(curveId:Guid, testPoint:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let mutable rc, t = curve.ClosestPoint(testPoint)//, 0.0)
        if not <| rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurveRadius failed. curveId:'%s' testPoint:'%A' segmentIndex:'%A'" (rhType curveId) testPoint segmentIndex
        let mutable v = curve.CurvatureAt( t )
        let mutable k = v.Length
        if k>RhinoMath.ZeroTolerance then  1.0/k
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveRadius failed. curveId:'%s' testPoint:'%A' segmentIndex:'%A'" (rhType curveId) testPoint segmentIndex


    ///<summary>Adjusts the seam, or start/end, point of a closed Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="parameter">(float) The parameter of the new start/end point.
    ///    Note, if successful, the resulting Curve's
    ///    domain will start at `parameter`</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member CurveSeam(curveId:Guid, parameter:float) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if (not <| curve.IsClosed || not <| curve.Domain.IncludesParameter(parameter)) then
            false
        else
            let dupe = curve.Duplicate() :?>Curve
            if notNull dupe then
                let r = dupe.ChangeClosedCurveSeam(parameter)
                if not r then r
                else
                    Doc.Objects.Replace(curveId, dupe)
            else
                false


    ///<summary>Returns the start point of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D starting point of the Curve.</returns>
    [<Extension>]
    static member CurveStartPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.PointAtStart

    ///<summary>Sets the start point of a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="point">(Point3d) New start point</param>
    ///<returns>(unit).</returns>
    [<Extension>]
    static member CurveStartPoint(curveId:Guid, point:Point3d) : unit =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if not <|curve.SetStartPoint(point) then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveStartPoint failed on '%A' and '%A'" point curveId
        Doc.Objects.Replace(curveId, curve) |> ignore
        Doc.Views.Redraw()



    ///<summary>Calculates intersection of a Curve object with a Surface object.
    ///    Note, this function works on the untrimmed portion of the Surface.</summary>
    ///<param name="curveId">(Guid) The identifier of the first Curve object</param>
    ///<param name="surfaceId">(Guid) The identifier of the second Curve object. If omitted,
    ///    the a self-intersection test will be performed on Curve</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    The absolute tolerance in drawing units.</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>Doc.ModelAngleToleranceRadians</c>
    ///    Angle tolerance in degrees. The angle
    ///    tolerance is used to determine when the Curve is tangent to the
    ///    Surface.</param>
    ///<returns>(Rarr of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) of intersection information .
    ///    The list will contain one or more of the following elements:
    ///      Element Type     Description
    ///      [n][0]  Number   The intersection event type, either Point(1) or Overlap(2).
    ///      [n][1]  Point3d  If the event type is Point(1), then the intersection point
    ///        on the first Curve. If the event type is Overlap(2), then
    ///        intersection start point on the first Curve.
    ///      [n][2]  Point3d  If the event type is Point(1), then the intersection point
    ///        on the first Curve. If the event type is Overlap(2), then
    ///        intersection end point on the first Curve.
    ///      [n][3]  Point3d  If the event type is Point(1), then the intersection point
    ///        on the second Curve. If the event type is Overlap(2), then
    ///        intersection start point on the Surface.
    ///      [n][4]  Point3d  If the event type is Point(1), then the intersection point
    ///        on the second Curve. If the event type is Overlap(2), then
    ///        intersection end point on the Surface.
    ///      [n][5]  Number   If the event type is Point(1), then the first Curve parameter.
    ///        If the event type is Overlap(2), then the start value of the
    ///        first Curve parameter range.
    ///      [n][6]  Number   If the event type is Point(1), then the first Curve parameter.
    ///        If the event type is Overlap(2), then the end value of the
    ///        Curve parameter range.
    ///      [n][7]  Number   If the event type is Point(1), then the U Surface parameter.
    ///        If the event type is Overlap(2), then the U Surface parameter
    ///        for Curve at (n, 5).
    ///      [n][8]  Number   If the event type is Point(1), then the V Surface parameter.
    ///        If the event type is Overlap(2), then the V Surface parameter
    ///        for Curve at (n, 5).
    ///      [n][9]  Number   If the event type is Point(1), then the U Surface parameter.
    ///        If the event type is Overlap(2), then the U Surface parameter
    ///        for Curve at (n, 6).
    ///      [n][10] Number   If the event type is Point(1), then the V Surface parameter.
    ///        If the event type is Overlap(2), then the V Surface parameter
    ///        for Curve at (n, 6).</returns>
    [<Extension>]
    static member CurveSurfaceIntersection(curveId:Guid, surfaceId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(0.0)>]angleTolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float*float*float) Rarr =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let surface = RhinoScriptSyntax.CoerceSurface surfaceId
        let tolerance0 = ifZero1 tolerance Doc.ModelAbsoluteTolerance
        let angleTolerance0 = ifZero1 (toRadians(angleTolerance)) Doc.ModelAngleToleranceRadians
        let  rc = Intersect.Intersection.CurveSurface(curve, surface, tolerance0, angleTolerance0)
        if isNull rc then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveSurfaceIntersection failed. (surfaceId:%A) (curveId:%A) (angleTolerance:%f) (tolerance:%f) " surfaceId curveId angleTolerance tolerance
        let events = Rarr()
        for i = 0 to rc.Count - 1 do
            let eventType = if rc.[i].IsOverlap then 2 else 1
            let item = rc.[i]
            let oa = item.OverlapA
            let u, v = item.SurfaceOverlapParameter()
            let e = eventType, item.PointA, item.PointA2, item.PointB, item.PointB2, oa.[0], oa.[1], u.[0], u.[1], v.[0], v.[1]
            events.Add(e)
        events




    ///<summary>Returns a 3D vector that is the tangent to a Curve at a parameter.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) A 3D vector.</returns>
    [<Extension>]
    static member CurveTangent(curveId:Guid, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)
        let mutable rc = Point3d.Unset
        if curve.Domain.IncludesParameter(parameter) then
            curve.TangentAt(parameter)
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.CurveTangent failed. curveId:'%s' parameter:'%A' segmentIndex:'%A'" (rhType curveId) parameter segmentIndex


    ///<summary>Returns list of weights that are assigned to the control points of a Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float Rarr) The weight values of the Curve.</returns>
    [<Extension>]
    static member CurveWeights(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) :  float Rarr =
        let nc =
            match RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex) with
            | :? NurbsCurve as nc -> nc
            | c -> c.ToNurbsCurve()
        if isNull nc then  RhinoScriptingException.Raise "RhinoScriptSyntax.CurveWeights failed. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex
        rarr { for pt in nc.Points -> pt.Weight }

    ///<summary>Divides a Curve Geometry into a specified number of segments, inluding start and end point.</summary>
    ///<param name="curve">(Geometry.Curve) Curve geometry</param>
    ///<param name="segments">(int) The number of segments</param>
    ///<returns>(Point3d array) Array containing points at divisions.</returns>
    [<Extension>]
    static member DivideCurveIntoPoints(curve:Curve, segments:int) : Point3d array =        
        let pts = ref (Array.zeroCreate (segments + 1))
        let rc = curve.DivideByCount(segments, true, pts)
        if isNull rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveIntoPoints failed. curve:'%A' segments:'%A'" curve segments
        !pts

    ///<summary>Divides a Curve object into a specified number of segments, inluding start and end point.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segments">(int) The number of segments</param>
    ///<returns>(Point3d array) Array containing points at divisions.</returns>
    [<Extension>]
    static member DivideCurveIntoPoints(curveId:Guid, segments:int) : Point3d array =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        let pts = ref (Array.zeroCreate (segments + 1))
        let rc = curve.DivideByCount(segments, true, pts)
        if isNull rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveIntoPoints failed. curveId:'%s' segments:'%A'" (rhType curveId) segments
        !pts

    ///<summary>Divides a Curve Geometry into a specified number of segments.</summary>
    ///<param name="curve">(Geometry.Curve) Curve geometry</param>
    ///<param name="segments">(int) The number of segments</param>
    ///<returns>( float array ) array containing 3D division parameters.</returns>
    [<Extension>]
    static member DivideCurve(curve:Curve, segments:int) :  float array =
        let rc = curve.DivideByCount(segments, true)
        if isNull rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurve failed. curve:'%A' segments:'%A'" curve segments
        rc

    ///<summary>Divides a Curve object into a specified number of segments.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segments">(int) The number of segments</param>
    ///<returns>( float array ) array containing 3D division parameters.</returns>
    [<Extension>]
    static member DivideCurve(curveId:Guid, segments:int) :  float array =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        let rc = curve.DivideByCount(segments, true)
        if isNull rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurve failed. curveId:'%s' segments:'%A'" (rhType curveId) segments
        rc


    ///<summary>Divides a Curve Geometry such that the linear distance between the points is equal.</summary>
    ///<param name="curve">(Geometry.Curve) Curve geometry</param>
    ///<param name="distance">(float) Linear distance between division points</param>
    ///<returns>(Point3d array) array containing 3D division points.</returns>
    [<Extension>]
    static member DivideCurveEquidistant(curve:Curve, distance:float) : array<Point3d> =
        let  points = curve.DivideEquidistant(distance)
        if isNull points then  
            let len = curve.GetLength()
            if len < distance then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveEquidistant failed on too short curve. curve:'%A' distance:%f, curveLength=%f" curve distance len
            else 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveEquidistant failed. curve:'%A' distance:%f, curveLength=%f" curve distance len
        points
       

    ///<summary>Divides a Curve object such that the linear distance between the points is equal.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="distance">(float) Linear distance between division points</param>
    ///<returns>(Point3d array) array containing 3D division points.</returns>
    [<Extension>]
    static member DivideCurveEquidistant(curveId:Guid, distance:float) : array<Point3d> =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId
        let  points = curve.DivideEquidistant(distance)
        if isNull points then  
            let len = curve.GetLength()
            if len < distance then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveEquidistant failed on too short curve. curveId:'%s' distance:%f, curveLength=%f" (rhType curveId) distance len
            else 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveEquidistant failed. curveId:'%s' distance:%f, curveLength=%f" (rhType curveId) distance len
        points
        //let  tvals = Rarr()
        //for point in points do
        //    let mutable rc, t = curve.ClosestPoint(point)
        //    tvals.Add(t)
        //tvals
    
    ///<summary>Divides a Curve Geometry into segments of a specified length.
    /// If length is more than Curve length it fails.</summary>
    ///<param name="curve">(Geometry.Curve) Curve geometry</param>
    ///<param name="length">(float) The length of each segment</param>
    ///<returns>(Point3d Rarr) a list containing division points.</returns>
    [<Extension>]
    static member DivideCurveLengthIntoPoints(curve:Curve, length:float) : Point3d Rarr =        
        let rc = curve.DivideByLength(length, true)
        if isNull rc then  
            let len = curve.GetLength()
            if len < length then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveLengthIntoPoints failed on too short curve. curve:'%A' divedlength:%f, curveLength=%f" curve length len
            else 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveLengthIntoPoints failed. curve:'%A' divedlength:%f, curveLength=%f" curve length len
        rarr{ for r in rc do curve.PointAt(r)}

    ///<summary>Divides a Curve object into segments of a specified length.
    /// If length is more than Curve length it fails.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="length">(float) The length of each segment</param>
    ///<returns>(Point3d Rarr) a list containing division points.</returns>
    [<Extension>]
    static member DivideCurveLengthIntoPoints(curveId:Guid, length:float) : Point3d Rarr =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let rc = curve.DivideByLength(length, true)
        if isNull rc then  
            let len = curve.GetLength()
            if len < length then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveLengthIntoPoints failed on too short curve. curveId:'%s' divedlength:%f, curveLength=%f" (rhType curveId) length len
            else 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveLengthIntoPoints failed. curveId:'%s' divedlength:%f, curveLength=%f" (rhType curveId) length len
        rarr{ for r in rc do curve.PointAt(r)}
    
    ///<summary>Divides a Curve Geometry into segments of a specified length.
    /// If length is more than Curve length it fails.</summary>
    ///<param name="curve">(Geometry.Curve) Curve geometry</param>
    ///<param name="length">(float) The length of each segment</param>
    ///<returns>( float array) a list containing division parameters.</returns>
    [<Extension>]
    static member DivideCurveLength(curve:Curve, length:float) :  float [] =        
        let rc = curve.DivideByLength(length, true)
        if isNull rc then  
            let len = curve.GetLength()
            if len < length then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveLength failed on too short curve. curve:'%A' divedlength:%f, curveLength=%f" curve length len
            else 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveLength failed. curve:'%A' divedlength:%f, curveLength=%f" curve length len
        rc

    ///<summary>Divides a Curve object into segments of a specified length.
    /// If length is more than Curve length it fails.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="length">(float) The length of each segment</param>
    ///<returns>( float array) a list containing division parameters.</returns>
    [<Extension>]
    static member DivideCurveLength(curveId:Guid, length:float) :  float [] =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let rc = curve.DivideByLength(length, true)
        if isNull rc then  
            let len = curve.GetLength()
            if len < length then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveLength failed on too short curve. curveId:'%s' divedlength:%f, curveLength=%f" (rhType curveId) length len
            else 
                RhinoScriptingException.Raise "RhinoScriptSyntax.DivideCurveLength failed. curveId:'%s' divedlength:%f, curveLength=%f" (rhType curveId) length len
        rc


    ///<summary>Returns the center point of an elliptical-shaped Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<returns>(Point3d) The 3D center point of the ellipse.</returns>
    [<Extension>]
    static member EllipseCenterPoint(curveId:Guid) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let rc, ellipse = curve.TryGetEllipse()
        if not <| rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.EllipseCenterPoint: Curve is not an ellipse. curveId:'%s'" (rhType curveId)
        ellipse.Plane.Origin


    ///<summary>Returns the quadrant points of an elliptical-shaped Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four points identifying the quadrants of the ellipse.</returns>
    [<Extension>]
    static member EllipseQuadPoints(curveId:Guid) : Point3d * Point3d * Point3d * Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let rc, ellipse = curve.TryGetEllipse()
        if not <| rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.EllipseQuadPoints: Curve is not an ellipse. curveId:'%s'" (rhType curveId)
        let origin = ellipse.Plane.Origin;
        let xaxis = ellipse.Radius1 * ellipse.Plane.XAxis;
        let yaxis = ellipse.Radius2 * ellipse.Plane.YAxis;
        (origin-xaxis, origin + xaxis, origin-yaxis, origin + yaxis)


    ///<summary>Evaluates a Curve at a parameter and returns a 3D point.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="t">(float) The parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) a 3-D point.</returns>
    [<Extension>]
    static member EvaluateCurve(curveId:Guid, t:float, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        curve.PointAt(t)


    ///<summary>Explodes, or un-joins, one Curve. PolyCurves will be exploded into Curve
    ///    segments. Polylines will be exploded into line segments. ExplodeCurves will
    ///    return the Curves in topological order.</summary>
    ///<param name="curveId">(Guid) The Curve object to explode</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input objects after exploding if True</param>
    ///<returns>(Guid Rarr) identifying the newly created Curve objects.</returns>
    [<Extension>]
    static member ExplodeCurve(curveId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid Rarr =
        let rc = Rarr()
        let curve = RhinoScriptSyntax.CoerceCurve curveId
        let pieces = curve.DuplicateSegments()
        if notNull pieces then
            for piece in pieces do
                rc.Add(Doc.Objects.AddCurve(piece))
            if deleteInput then
                Doc.Objects.Delete(curveId, true) |>ignore
        if rc.Count>0 then  Doc.Views.Redraw()
        rc


    ///<summary>Extends a non-closed Curve object by a line, arc, or smooth extension
    ///    until it intersects a collection of objects.</summary>
    ///<param name="curveId">(Guid) Identifier of Curve to extend</param>
    ///<param name="extensionType">(int) 
    ///    0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    ///<param name="side">(int) 
    ///    0 = extend from the start of the Curve
    ///    1 = extend from the end of the Curve
    ///    2 = extend from both the start and the end of the Curve</param>
    ///<param name="boundarycurveIds">(Guid seq) Curve, Surface, and Polysurface objects to extend to</param>
    ///<param name="replaceInput">(bool) Optional, Default Value <c>false</c> Replace input or add new?</param>
    ///<returns>(Guid) The identifier of the new object or orignal Curve ( depending on 'replaceInput').</returns>
    [<Extension>]
    static member ExtendCurve(  curveId:Guid, 
                                extensionType:int, 
                                side:int, 
                                boundarycurveIds:Guid seq,
                                [<OPT;DEF(false)>]replaceInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let mutable extensionTypet = CurveExtensionStyle.Line
        if extensionType = 0 then  extensionTypet <- CurveExtensionStyle.Line
        elif extensionType = 1 then extensionTypet <- CurveExtensionStyle.Arc
        elif extensionType = 2 then extensionTypet <- CurveExtensionStyle.Smooth
        else RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurve ExtensionType must be 0, 1, or 2. curveId:'%s' extensionType:'%A' side:'%A' boundarycurveIds:'%s'" (rhType curveId) extensionType side  (RhinoScriptSyntax.ToNiceString boundarycurveIds)

        let sidet = 
            match side with 
            |0  -> CurveEnd.Start
            |1  -> CurveEnd.End
            |2  -> CurveEnd.Both
            |_  -> RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurve Side must be 0, 1, or 2. curveId:'%s' extensionType:'%A' side:'%A' boundarycurveIds:'%s'" (rhType curveId) extensionType side  (RhinoScriptSyntax.ToNiceString boundarycurveIds)

        let rhobjs = rarr { for objectId in boundarycurveIds -> RhinoScriptSyntax.CoerceRhinoObject(objectId) }
        if rhobjs.IsEmpty then  RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurve BoundarycurveIds failed. They must contain at least one item. curveId:'%s' extensionType:'%A' side:'%A' boundarycurveIds:'%s'" (rhType curveId) extensionType side (RhinoScriptSyntax.ToNiceString boundarycurveIds)
        let geometry = rarr { for obj in rhobjs -> obj.Geometry }
        let newcurve = curve.Extend(sidet, extensionTypet, geometry)
        if notNull newcurve && newcurve.IsValid then
            if replaceInput then
              if Doc.Objects.Replace(curveId, newcurve) then
                 Doc.Views.Redraw()
                 curveId
              else
                 RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurve failed. curveId:'%s' extensionType:'%A' side:'%A' boundarycurveIds:'%s'" (rhType curveId) extensionType side  (RhinoScriptSyntax.ToNiceString boundarycurveIds)
            else
              let g= Doc.Objects.AddCurve(newcurve)
              Doc.Views.Redraw()
              g
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurve failed. curveId:'%s' extensionType:'%A' side:'%A' boundarycurveIds:'%s'" (rhType curveId) extensionType side  (RhinoScriptSyntax.ToNiceString boundarycurveIds)


    ///<summary>Extends a non-closed Curve by a line, arc, or smooth extension for a specified distance.</summary>
    ///<param name="curveId">(Guid) Curve to extend</param>
    ///<param name="extensionType">(int) 
    ///    0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    ///<param name="side">(int) 
    ///    0 = extend from start of the Curve
    ///    1 = extend from end of the Curve
    ///    2 = Extend from both ends</param>
    ///<param name="length">(float) Distance to extend</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    [<Extension>]
    static member ExtendCurveLength(    curveId:Guid, 
                                        extensionType:int, 
                                        side:int, 
                                        length:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let mutable extensionTypet = CurveExtensionStyle.Line
        if extensionType   = 0 then extensionTypet <- CurveExtensionStyle.Line
        elif extensionType = 1 then extensionTypet <- CurveExtensionStyle.Arc
        elif extensionType = 2 then extensionTypet <- CurveExtensionStyle.Smooth
        else RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurveLength ExtensionType must be 0, 1, or 2. curveId:'%s' extensionType:'%A' side:'%A' length:'%A'" (rhType curveId) extensionType side length

        let sidet = 
            match side with 
            |0  -> CurveEnd.Start
            |1  -> CurveEnd.End
            |2  -> CurveEnd.Both
            |_  -> RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurveLength Side must be 0, 1, or 2. curveId:'%s' extensionType:'%A' side:'%A' length:'%A'" (rhType curveId) extensionType side length

        let newcurve =
            if length<0. then curve.Trim(sidet, -length)
            else curve.Extend(sidet, length, extensionTypet)

        if notNull newcurve && newcurve.IsValid then
            if Doc.Objects.Replace(curveId, newcurve) then
                Doc.Views.Redraw()
                curveId
            else RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurveLength failed. curveId:'%s' extensionType:'%A' side:'%A' length:'%A'" (rhType curveId) extensionType side length
        else RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurveLength failed. curveId:'%s' extensionType:'%A' side:'%A' length:'%A'" (rhType curveId) extensionType side length


    ///<summary>Extends a non-closed Curve by smooth extension to a point.</summary>
    ///<param name="curveId">(Guid) Curve to extend</param>
    ///<param name="side">(int) 
    ///    0 = extend from start of the Curve
    ///    1 = extend from end of the Curve
    ///    2 = extend from both the start and the end of the Curve</param>
    ///<param name="point">(Point3d) Point to extend to</param>
    ///<param name="extensionType">(int) Optional, Default Value: <c>2</c> ( CurveExtensionStyle.Smooth)) 
    ///    0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    [<Extension>]
    static member ExtendCurvePoint( curveId:Guid, 
                                    side:int, 
                                    point:Point3d,
                                    [<OPT;DEF(-1)>]extensionType:int) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let extensionTypet = 
            match extensionType with 
            | -1 ->  CurveExtensionStyle.Smooth
            |  0 ->  CurveExtensionStyle.Line
            |  1 ->  CurveExtensionStyle.Arc
            |  2 ->  CurveExtensionStyle.Smooth
            |  x ->  RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurvePoint ExtensionType must be 0, 1, or 2. curveId:'%s' side:'%A' point:'%A' extensionType:'%A'" (rhType curveId) side point extensionType

        let sidet = 
            match side with 
            |0  -> CurveEnd.Start
            |1  -> CurveEnd.End
            |2  -> CurveEnd.Both
            |_  -> RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurvePoint Side must be 0, 1, or 2. curveId:'%s' side:'%A' point:'%A' extensionType:'%A'" (rhType curveId) side point extensionType
        
        let newcurve = curve.Extend(sidet, extensionTypet, point)
        if notNull newcurve && newcurve.IsValid then
            if Doc.Objects.Replace( curveId, newcurve ) then
                Doc.Views.Redraw()
                curveId
            else RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurvePoint failed. curveId:'%s' side:'%A' point:'%A'" (rhType curveId) side point
        else RhinoScriptingException.Raise "RhinoScriptSyntax.ExtendCurvePoint failed. curveId:'%s' side:'%A' point:'%A'" (rhType curveId) side point


    ///<summary>Fairs a Curve. Fair works best on degree 3 (cubic) Curves. Fair attempts
    ///    to remove large curvature variations while limiting the geometry changes to
    ///    be no more than the specified tolerance. Sometimes several applications of
    ///    this method are necessary to remove nasty curvature problems.</summary>
    ///<param name="curveId">(Guid) Curve to fair</param>
    ///<param name="tolerance">(float) Fairing tolerance</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
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


    ///<summary>Reduces number of Curve control points while maintaining the Curve's same
    ///    general shape. Use this function for replacing Curves with many control
    ///    points. For more information, see the Rhino help for the FitCrv command.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///    The Curve degree, which must be greater than 1.
    ///    The default is 3</param>
    ///<param name="distanceTolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    The fitting tolerance.</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>Doc.ModelAngleToleranceRadians</c>
    ///    The kink smoothing tolerance in degrees. If
    ///    angleTolerance is 0.0, all kinks are smoothed. If angleTolerance
    ///    is bigger than  0.0, kinks smaller than angleTolerance are smoothed. If
    ///    angleTolerance is not specified or smaller than 0.0, the document angle
    ///    tolerance is used for the kink smoothing</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    [<Extension>]
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
            if rc = Guid.Empty then  RhinoScriptingException.Raise "RhinoScriptSyntax.FitCurve: Unable to add curve to document. curveId:'%s' degree:'%A' distanceTolerance:'%A' angleTolerance:'%A'" (rhType curveId) degree distanceTolerance angleTolerance
            Doc.Views.Redraw()
            rc
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.FitCurve failed. curveId:'%s' degree:'%A' distanceTolerance:'%A' angleTolerance:'%A'" (rhType curveId) degree distanceTolerance angleTolerance


    ///<summary>Inserts a knot into a Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="parameter">(float) Parameter on the Curve</param>
    ///<param name="symmetrical">(bool) Optional, Default Value: <c>false</c>
    ///    If True, then knots are added on both sides of
    ///    the center of the Curve</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
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


    ///<summary>Verifies an object is an open arc Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///    If the Curve is not a circle, then the tolerance used
    ///    to determine whether or not the NURBS form of the Curve has the
    ///    properties of a arc.</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsArc(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsArc(tol) && not curve.IsClosed


    ///<summary>Verifies an object is a circle Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///    If the Curve is not a circle, then the tolerance used
    ///    to determine whether or not the NURBS form of the Curve has the
    ///    properties of a circle.</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsCircle(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tol = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsCircle(tol)


    ///<summary>Verifies an object is a Curve.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsCurve(curveId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> true


    ///<summary>Decide if it makes sense to close off the Curve by moving the end point
    ///    to the start point based on start-end gap size and length of Curve as
    ///    approximated by chord defined by 6 points.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Maximum allowable distance between start point and end point.</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsCurveClosable(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsClosable(tolerance0)


    ///<summary>Verifies an object is a closed Curve object.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<returns>(bool) If Curve is Closed True,  otherwise False.</returns>
    [<Extension>]
    static member IsCurveClosed(curveId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsClosed


    ///<summary>Test a Curve to see if it lies in a specific Plane.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="plane">(Plane) Plane to test</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsCurveInPlane(curveId:Guid, plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance0 = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsInPlane(plane, tolerance0)


    ///<summary>Verifies an object is a linear Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>    ///
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member IsCurveLinear(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tolerance0 = ifZero2 RhinoMath.ZeroTolerance tolerance
        match  RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsLinear(tolerance0)


    ///<summary>Verifies an object is a periodic Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsCurvePeriodic(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsPeriodic


    ///<summary>Verifies an object is a planar Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member IsCurvePlanar(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsPlanar(tol)


    ///<summary>Verifies an object is a rational NURBS Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member IsCurveRational(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None -> false
        |Some c  ->
            match c with
            | :? NurbsCurve as curve -> curve.IsRational
            |_ -> false


    ///<summary>Verifies an object is an elliptical-shaped Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<param name="segmentIndex">(int) Optional,
    ///    The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member IsEllipse(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsEllipse(tol)


    ///<summary>Verifies an object is a line Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c></param>
    ///<param name="segmentIndex">(int) Optional,
    ///    The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
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


    ///<summary>Verifies that a point is on a Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="point">(Point3d) The test point</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.SqrtEpsilon</c></param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member IsPointOnCurve(curveId:Guid, point:Point3d, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = ifZero2 RhinoMath.SqrtEpsilon tolerance
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let t = ref 0.0
        curve.ClosestPoint(point, t, tol)


    ///<summary>Verifies an object is a PolyCurve Curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsPolyCurve(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        // TODO can a polycurve be nested in a polycurve ?
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None               -> false
        |Some c  ->
            match c with
            | :? PolyCurve  -> true
            | _             -> false


    ///<summary>Verifies an object is a Polyline Curve object or a nurbs cure with degree 1 and moer than 2 points
    /// Lines return false.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsPolyline(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryCoerceCurve(curveId, segmentIndex) with
        |None               -> false
        |Some c  ->
            match c with
            | :? PolylineCurve  -> true           
            | :? NurbsCurve as nc -> nc.Points.Count > 2 && c.Degree = 1
            | _ -> false
                


    ///<summary>Joins multiple Curves together to form one or more Curves or polycurves.</summary>
    ///<param name="curveIds">(Guid seq) List of multiple Curves</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input objects after joining</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>2.1 * Doc.ModelAbsoluteTolerance</c>
    ///    Join tolerance. If omitted, 2.1 * document absolute
    ///    tolerance is used</param>
    ///<returns>(Guid Rarr) Object objectId representing the new Curves.</returns>
    [<Extension>]
    static member JoinCurves(curveIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool, [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr =
        if Seq.hasMaximumItems 1 curveIds then
            RhinoScriptingException.Raise "RhinoScriptSyntax.JoinCurves: curveIds must contain at least two items.  curveIds:'%s' deleteInput:'%A' tolerance:'%A'" (RhinoScriptSyntax.ToNiceString curveIds) deleteInput tolerance

        let curves = rarr { for objectId in curveIds -> RhinoScriptSyntax.CoerceCurve objectId }
        let tolerance0 = ifZero1 tolerance (2.1 * Doc.ModelAbsoluteTolerance)
        let newcurves = Curve.JoinCurves(curves, tolerance0)
        if isNull newcurves then
            RhinoScriptingException.Raise "RhinoScriptSyntax.JoinCurves failed on curveIds:'%s' deleteInput:'%A' tolerance:'%A'" (RhinoScriptSyntax.ToNiceString curveIds) deleteInput tolerance

        let rc = rarr { for crv in newcurves -> Doc.Objects.AddCurve(crv) }
        if deleteInput then
            for objectId in curveIds do
                Doc.Objects.Delete(objectId, false) |> ignore
        Doc.Views.Redraw()
        rc



    ///<summary>Returns a line that was fit through an array of 3D points.</summary>
    ///<param name="points">(Point3d seq) A list of at least two 3D points</param>
    ///<returns>(Line) line.</returns>
    [<Extension>]
    static member LineFitFromPoints(points:Point3d seq) : Line =
        let rc, line = Line.TryFitLineToPoints(points)
        if rc then  line
        else RhinoScriptingException.Raise "RhinoScriptSyntax.LineFitFromPoints failed.  points:'%A'" points


    ///<summary>Makes a periodic Curve non-periodic. Non-periodic Curves can develop
    ///    kinks when deformed.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the input Curve. If omitted, the input Curve will not be deleted</param>
    ///<returns>(Guid) objectId of the new or modified Curve.</returns>
    [<Extension>]
    static member MakeCurveNonPeriodic(curveId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if not <| curve.IsPeriodic then  RhinoScriptingException.Raise "RhinoScriptSyntax.MakeCurveNonPeriodic failed.1  curveId:'%s' deleteInput:'%A'" (rhType curveId) deleteInput
        let nc = curve.ToNurbsCurve()
        if isNull nc  then  RhinoScriptingException.Raise "RhinoScriptSyntax.MakeCurveNonPeriodic failed.2  curveId:'%s' deleteInput:'%A'" (rhType curveId) deleteInput
        if not <| nc.Knots.ClampEnd( CurveEnd.Both ) then RhinoScriptingException.Raise "RhinoScriptSyntax.MakeCurveNonPeriodic failed. curveId:'%s' deleteInput:'%A'" (rhType curveId) deleteInput
        if deleteInput then
            let rc = Doc.Objects.Replace(curveId, nc)
            if not <| rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.MakeCurveNonPeriodic failed.3  curveId:'%s' deleteInput:'%A'" (rhType curveId) deleteInput
            Doc.Views.Redraw()
            curveId
        else
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
            let rc = Doc.Objects.AddCurve(nc, rhobj.Attributes)
            if rc = Guid.Empty then  RhinoScriptingException.Raise "RhinoScriptSyntax.MakeCurveNonPeriodic failed.4  curveId:'%s' deleteInput:'%A'" (rhType curveId) deleteInput
            Doc.Views.Redraw()
            rc


    ///<summary>Creates an average Curve from two Curves.</summary>
    ///<param name="curve0">(Guid) identifiers of first Curve</param>
    ///<param name="curve1">(Guid) identifiers of second Curve</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Angle tolerance used to match kinks between Curves</param>
    ///<returns>(Guid) objectId of the new or modified Curve.</returns>
    [<Extension>]
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
            RhinoScriptingException.Raise "RhinoScriptSyntax.MeanCurve failed.  curve1:'%A' curve0:'%A' tolerance:'%f'" curve1 curve0 tolerance


    ///<summary>Creates a polygon Mesh object based on a closed Polyline Curve object.
    ///    The created Mesh object is added to the document.</summary>
    ///<param name="polylineId">(Guid) Identifier of the Polyline Curve object</param>
    ///<returns>(Guid) identifier of the new Mesh object.</returns>
    [<Extension>]
    static member MeshPolyline(polylineId:Guid) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve polylineId
        let ispolyline, polyline = curve.TryGetPolyline()
        if not <| ispolyline then  RhinoScriptingException.Raise "RhinoScriptSyntax.MeshPolyline failed.  polylineId:'%s'" (rhType polylineId)
        let mesh = Mesh.CreateFromClosedPolyline(polyline)
        if isNull mesh then  RhinoScriptingException.Raise "RhinoScriptSyntax.MeshPolyline failed.  polylineId:'%s'" (rhType polylineId)
        let rc = Doc.Objects.AddMesh(mesh)
        Doc.Views.Redraw()
        rc


    ///<summary>Offsets a Curve by a distance. The offset Curve will be added to Rhino.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="direction">(Point3d) Point describing direction of the offset</param>
    ///<param name="distance">(float) Distance of the offset</param>
    ///<param name="normal">(Vector3d) Optional, Default Value: <c>Vector3d.ZAxis</c>
    ///    Normal of the Plane in which the offset will occur.
    ///    If omitted, the WorldXY  Plane will be used</param>
    ///<param name="style">(int) Optional, Default Value: <c>1</c>
    ///    The corner style. If omitted, the style is sharp.
    ///    0 = None
    ///    1 = Sharp
    ///    2 = Round
    ///    3 = Smooth
    ///    4 = Chamfer</param>
    ///<returns>(Guid Rarr) list of ids for the new Curves.</returns>
    [<Extension>]
    static member OffsetCurve(curveId:Guid, direction:Point3d, distance:float, [<OPT;DEF(Vector3d())>]normal:Vector3d, [<OPT;DEF(1)>]style:int) : Guid Rarr = //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let normal0 = if normal.IsZero then Vector3d.ZAxis else normal
        let curve = RhinoScriptSyntax.CoerceCurve curveId
        let tolerance = Doc.ModelAbsoluteTolerance
        let stylee:CurveOffsetCornerStyle = EnumOfValue style
        let curves = curve.Offset(direction, normal0, distance, tolerance, stylee)
        if isNull curves then  RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetCurve failed. curveId:'%s' direction:'%A' distance:'%A' normal:'%A' style:%d" (rhType curveId) direction distance normal style
        let rc = rarr { for curve in curves -> Doc.Objects.AddCurve(curve) }
        Doc.Views.Redraw()
        rc


    ///<summary>Offset a Curve on a Surface. The source Curve must lie on the Surface.
    ///    The offset Curve or Curves will be added to Rhino.</summary>
    ///<param name="curveId">(Guid) Curve identifiers</param>
    ///<param name="surfaceId">(Guid) Surface identifiers</param>
    ///<param name="parameter">(Point2d))  U, V parameter that the Curve will be offset through</param>
    ///<returns>(Guid Rarr) identifiers of the new Curves.</returns>
    [<Extension>]
    static member OffsetCurveOnSurfaceUV(curveId:Guid, surfaceId:Guid, parameter:Point2d) : Guid Rarr =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let tol = Doc.ModelAbsoluteTolerance
        let curves = curve.OffsetOnSurface(surface, parameter, tol)
        if isNull curves  then  RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetCurveOnSurface failed. curveId:'%s' surfaceId:'%s' parameter:'%A'" (rhType curveId) (rhType surfaceId) parameter
        let rc = rarr { for curve in curves -> Doc.Objects.AddCurve(curve) }
        Doc.Views.Redraw()
        rc

    ///<summary>Offset a Curve on a Surface. The source Curve must lie on the Surface.
    ///    The offset Curve or Curves will be added to Rhino Document.</summary>
    ///<param name="curveId">(Guid) The Curve identifiers</param>
    ///<param name="surfaceId">(Guid) The Surface identifiers</param>
    ///<param name="distance">(float)) The distance of the offset. Based on the Curve's direction, a positive value
    ///    will offset to the left and a negative value will offset to the right</param>
    ///<returns>(Guid Rarr) identifiers of the new Curves.</returns>
    [<Extension>]
    static member OffsetCurveOnSurface(curveId:Guid, surfaceId:Guid, distance:float) : Guid Rarr =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let tol = Doc.ModelAbsoluteTolerance
        let curves = curve.OffsetOnSurface(surface, distance, tol)
        if isNull curves  then  RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetCurveOnSurface failed. curveId:'%s' surfaceId:'%s' distance:'%A'" (rhType curveId) (rhType surfaceId) distance
        let curves = rarr{for curve in curves do curve.ExtendOnSurface(Rhino.Geometry.CurveEnd.Both, surface) } //https://github.com/mcneel/rhinoscriptsyntax/pull/186        
        let rc = rarr { for curve in curves -> Doc.Objects.AddCurve(curve) }
        Doc.Views.Redraw()
        rc



    ///<summary>Determines the relationship between the regions bounded by two coplanar simple closed Curves.</summary>
    ///<param name="curveA">(Guid) identifier of the first  planar, closed Curve</param>
    ///<param name="curveB">(Guid) identifier of the second planar, closed Curve</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///    Test Plane. If omitted, the Plane.WorldXY Plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(int) a number identifying the relationship
    ///    0 = the regions bounded by the Curves are disjoint
    ///    1 = the two Curves intersect
    ///    2 = the region bounded by CurveA is inside of CurveB
    ///    3 = the region bounded by CurveB is inside of CurveA.</returns>
    [<Extension>]
    static member PlanarClosedCurveContainment(curveA:Guid, curveB:Guid, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        let curveA = RhinoScriptSyntax.CoerceCurve curveA
        let curveB = RhinoScriptSyntax.CoerceCurve curveB
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let plane0 = if plane.IsValid then plane else Plane.WorldXY
        let rc = Curve.PlanarClosedCurveRelationship(curveA, curveB, plane0, tolerance0)
        int(rc)


    ///<summary>Determines if two coplanar Curves intersect.</summary>
    ///<param name="curveA">(Guid) identifier of the first  planar Curve</param>
    ///<param name="curveB">(Guid) identifier of the second planar Curve</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///    Test Plane. If omitted, the Plane.WorldXY Plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(bool) True if the Curves intersect; otherwise False.</returns>
    [<Extension>]
    static member PlanarCurveCollision(curveA:Guid, curveB:Guid, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let curveA = RhinoScriptSyntax.CoerceCurve curveA
        let curveB = RhinoScriptSyntax.CoerceCurve curveB
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let plane0 = if plane.IsValid then plane else Plane.WorldXY
        Curve.PlanarCurveCollision(curveA, curveB, plane0, tolerance0)


    ///<summary>Determines if a point is inside of a closed Curve, on a closed Curve, or
    ///    outside of a closed Curve.</summary>
    ///<param name="point">(Point3d) Text point</param>
    ///<param name="curve">(Guid) Identifier of a Curve object</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///    Plane containing the closed Curve and point. If omitted, Plane.WorldXY  is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(int) number identifying the result
    ///    0 = point is outside of the Curve
    ///    1 = point is inside of the Curve
    ///    2 = point is on the Curve.</returns>
    [<Extension>]
    static member PointInPlanarClosedCurve(point:Point3d, curve:Guid, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        let curve = RhinoScriptSyntax.CoerceCurve curve
        let tolerance0 = ifZero2 Doc.ModelAbsoluteTolerance tolerance
        let plane0 = if plane.IsValid then plane else Plane.WorldXY
        let rc = curve.Contains(point, plane0, tolerance0)
        if rc= PointContainment.Unset then
            RhinoScriptingException.Raise "RhinoScriptSyntax.PointInPlanarClosedCurve Curve.Contains is Unset.  point:'%A' curve:'%A' plane:'%A' tolerance:'%A'" point curve plane tolerance
        if rc= PointContainment.Outside then  0
        elif rc= PointContainment.Inside then  1
        else 2


    ///<summary>Returns the number of Curve segments that make up a polycurve.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    If `curveId` identifies a PolyCurve object, then `segmentIndex` identifies the Curve segment of the PolyCurve to query</param>
    ///<returns>(int) The number of Curve segments in a polycurve.</returns>
    [<Extension>]
    static member PolyCurveCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        match curve with
        | :? PolyCurve as curve ->  curve.SegmentCount
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CurveId does not reference a polycurve. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex


    ///<summary>Returns the vertices of a Polyline Curve.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    If CurveId identifies a PolyCurve object, then segmentIndex identifies the Curve segment of the PolyCurve to query</param>
    ///<returns>(Point3d Rarr) an list of Point3d vertex points.</returns>
    [<Extension>]
    static member PolylineVertices(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d Rarr =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)
        let rc, polyline = curve.TryGetPolyline()
        if rc then  rarr { for pt in polyline -> pt }
        else RhinoScriptingException.Raise "RhinoScriptSyntax.CurveId does not <| reference a polyline. curveId:'%s' segmentIndex:'%A'" (rhType curveId) segmentIndex


    ///<summary>Projects one or more Curves onto one or more Surfaces or Meshes.</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of Curves to project</param>
    ///<param name="meshIds">(Guid seq) Identifiers of Meshes to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid Rarr) list of identifiers for the resulting Curves.</returns>
    [<Extension>]
    static member ProjectCurveToMesh(curveIds:Guid seq, meshIds:Guid seq, direction:Vector3d) : Guid Rarr =
        let curves = rarr { for objectId in curveIds -> RhinoScriptSyntax.CoerceCurve objectId }
        let meshes = rarr { for objectId in meshIds -> RhinoScriptSyntax.CoerceMesh(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let newcurves = Curve.ProjectToMesh(curves, meshes, direction, tolerance)
        let ids = rarr { for curve in newcurves -> Doc.Objects.AddCurve(curve) }
        if ids.Count >0 then  Doc.Views.Redraw()
        ids


    ///<summary>Projects one or more Curves onto one or more Surfaces or Polysurfaces.</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of Curves to project</param>
    ///<param name="surfaceIds">(Guid seq) Identifiers of Surfaces to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid Rarr) list of identifiers.</returns>
    [<Extension>]
    static member ProjectCurveToSurface(curveIds:Guid seq, surfaceIds:Guid seq, direction:Vector3d) : Guid Rarr =
        let curves = rarr { for objectId in curveIds -> RhinoScriptSyntax.CoerceCurve objectId }
        let breps = rarr { for objectId in surfaceIds -> RhinoScriptSyntax.CoerceBrep(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let newcurves = Curve.ProjectToBrep(curves, breps, direction, tolerance)
        let ids = rarr { for curve in newcurves -> Doc.Objects.AddCurve(curve) }
        if ids.Count > 0 then  Doc.Views.Redraw()
        ids


    ///<summary>Rebuilds a Curve to a given degree and control point count. For more
    ///    information, see the Rhino help for the Rebuild command.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="degree">(int) New degree (must be greater than 0)</param>
    ///<param name="pointCount">(int) New point count, which must be bigger than degree</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    [<Extension>]
    static member RebuildCurve(curveId:Guid, degree:int, pointCount:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if degree<1 then  RhinoScriptingException.Raise "RhinoScriptSyntax.RebuildCurve: Degree must be greater than 0. curveId:'%s' degree:'%A' pointCount:'%A'" (rhType curveId) degree pointCount
        let newcurve = curve.Rebuild(pointCount, degree, false)
        if isNull newcurve then  false
        else
            Doc.Objects.Replace(curveId, newcurve) |> ignore
            Doc.Views.Redraw()
            true


    ///<summary>Deletes a knot from a Curve object.</summary>
    ///<param name="curve">(Guid) The reference of the source object</param>
    ///<param name="parameter">(float) The parameter on the Curve. Note, if the parameter is not equal to one
    ///    of the existing knots, then the knot closest to the specified parameter
    ///    will be removed</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    [<Extension>]
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


    ///<summary>Reverses the direction of a Curve object. Same as Rhino's Dir command.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member ReverseCurve(curveId:Guid) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if curve.Reverse() then
            Doc.Objects.Replace(curveId, curve)|> ignore
            true
        else
            false


    ///<summary>Replace a Curve with a geometrically equivalent polycurve.
    ///    The PolyCurve will have the following properties:
    ///      - All the PolyCurve segments are lines, polylines, arcs, or NURBS Curves.
    ///      - The NURBS Curves segments do not have fully multiple interior knots.
    ///      - Rational NURBS Curves do not have constant weights.
    ///      - Any segment for which IsCurveLinear or IsArc is True:  a line, Polyline segment, or an arc.
    ///      - Adjacent co-linear or co-circular segments are combined.
    ///      - Segments that meet with G1-continuity have there ends tuned up so that they meet with G1-continuity to within machine precision.
    ///      - If the PolyCurve is a polyline, a Polyline will be created.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="flags">(int) Optional, Default Value: <c>0</c>
    ///    The simplification methods to use. By default, all methods are used (flags = 0)
    ///    Value Description
    ///    0     Use all methods.
    ///    1     Do not split NURBS Curves at fully multiple knots.
    ///    2     Do not replace segments with IsCurveLinear = True with line Curves.
    ///    4     Do not replace segments with IsArc = True with arc Curves.
    ///    8     Do not replace rational NURBS Curves with constant denominator with an equivalent non-rational NURBS Curve.
    ///    16    Do not adjust Curves at G1-joins.
    ///    32    Do not merge adjacent co-linear lines or co-circular arcs or combine consecutive line segments into a polyline</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
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


    ///<summary>Splits, or divides, a Curve at a specified parameter. The parameter must
    ///    be in the interior of the Curve's domain.</summary>
    ///<param name="curveId">(Guid) The Curve to split</param>
    ///<param name="parameter">(float seq) One or more parameters to split the Curve at</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input Curve</param>
    ///<returns>(Guid Rarr) list of new Curves.</returns>
    [<Extension>]
    static member SplitCurve(curveId:Guid, parameter:float seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid Rarr =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let newcurves = curve.Split(parameter)
        if isNull newcurves then  RhinoScriptingException.Raise "RhinoScriptSyntax.SplitCurve failed. curveId:'%s' parameter:'%A' deleteInput:'%A'" (rhType curveId) parameter deleteInput
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let rc = rarr { for crv in newcurves -> Doc.Objects.AddCurve(crv, rhobj.Attributes) }
        if deleteInput then
            Doc.Objects.Delete(curveId, true)|> ignore
        Doc.Views.Redraw()
        rc


    ///<summary>Trims a Curve by removing portions of the Curve outside a specified interval.</summary>
    ///<param name="curveId">(Guid) The Curve to trim</param>
    ///<param name="interval">(float * float) Two numbers identifying the interval to keep. Portions of
    ///    the Curve before domain[0] and after domain[1] will be removed. If the
    ///    input Curve is open, the interval must be increasing. If the input
    ///    Curve is closed and the interval is decreasing, then the portion of
    ///    the Curve across the start and end of the Curve is returned</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input Curve. If omitted the input Curve is deleted</param>
    ///<returns>(Guid) identifier of the new Curve.</returns>
    [<Extension>]
    static member TrimCurve(curveId:Guid, interval:float * float, [<OPT;DEF(true)>]deleteInput:bool) : Guid  =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let newcurve = curve.Trim(fst interval, snd interval)
        if isNull newcurve then  RhinoScriptingException.Raise "RhinoScriptSyntax.TrimCurve failed. curveId:'%s' interval:'%A' deleteInput:'%A'" (rhType curveId) interval deleteInput
        let att = None
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let rc = Doc.Objects.AddCurve(newcurve, rhobj.Attributes)
        if deleteInput then
            Doc.Objects.Delete(curveId, true)|> ignore
        Doc.Views.Redraw()
        rc


    ///<summary>Changes the degree of a Curve object. For more information see the Rhino help file for the ChangeDegree command.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="degree">(int) The new degree</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    [<Extension>]
    static member ChangeCurveDegree(curveId:Guid, degree:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve curveId
        let nc = curve.ToNurbsCurve()
        if degree > 2 && degree < 12 && curve.Degree <> degree then
            if not <| nc.IncreaseDegree(degree) then false
            else
                Doc.Objects.Replace(curveId, nc)
        else
            false


    ///<summary>Creates Curves between two open or closed input Curves.</summary>
    ///<param name="fromCurveId">(Guid) Identifier of the first Curve object</param>
    ///<param name="toCurveId">(Guid) Identifier of the second Curve object</param>
    ///<param name="numberOfCurves">(int) Optional, Default Value: <c>1</c>
    ///    The number of Curves to create. The default is 1</param>
    ///<param name="method">(int) Optional, Default Value: <c>0</c>
    ///    The method for refining the output Curves, where:
    ///    0: (Default) Uses the control points of the Curves for matching. So the first control point of first Curve is matched to first control point of the second Curve.
    ///    1: Refits the output Curves like using the FitCurve method. Both the input Curve and the output Curve will have the same structure. The resulting Curves are usually more complex than input unless input Curves are compatible.
    ///    2: Input Curves are divided to the specified number of points on the Curve, corresponding points define new points that output Curves go through. If you are making one tween Curve, the method essentially does the following: divides the two Curves into an equal number of points, finds the midpoint between the corresponding points on the Curves, and interpolates the tween Curve through those points</param>
    ///<param name="sampleNumber">(int) Optional, Default Value: <c>10</c>
    ///    The number of samples points to use if method is 2. The default is 10</param>
    ///<returns>(Guid Rarr) The identifiers of the new tween objects.</returns>
    [<Extension>]
    static member AddTweenCurves(fromCurveId:Guid, toCurveId:Guid, [<OPT;DEF(1)>]numberOfCurves:int, [<OPT;DEF(0)>]method:int, [<OPT;DEF(10)>]sampleNumber:int) : Guid Rarr =
        let curve0 = RhinoScriptSyntax.CoerceCurve fromCurveId
        let curve1 = RhinoScriptSyntax.CoerceCurve toCurveId
        let mutable outCurves = Array.empty
        let tolerance = Doc.ModelAbsoluteTolerance
        if method = 0 then
            outCurves <- Curve.CreateTweenCurves(curve0, curve1, numberOfCurves, tolerance)
            outCurves <- Curve.CreateTweenCurvesWithMatching(curve0, curve1, numberOfCurves, tolerance)
        elif method = 2 then
            outCurves <- Curve.CreateTweenCurvesWithSampling(curve0, curve1, numberOfCurves, sampleNumber, tolerance)
        else RhinoScriptingException.Raise "RhinoScriptSyntax.AddTweenCurves Method must be 0, 1, or 2.  fromcurveId:'%s' tocurveId:'%s' numberOfCurves:'%A' method:'%A' sampleNumber:'%A'"  (rhType fromCurveId) (rhType toCurveId) numberOfCurves method sampleNumber
        let curves = Rarr()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    //curve.Dispose()
                    if rc = Guid.Empty then  RhinoScriptingException.Raise "RhinoScriptSyntax.AddTweenCurves: Unable to add curve to document.  fromcurveId:'%s' tocurveId:'%s' numberOfCurves:'%A' method:'%A' sampleNumber:'%A'" (rhType fromCurveId) (rhType toCurveId) numberOfCurves method sampleNumber
                    curves.Add(rc)
            Doc.Views.Redraw()
        curves

