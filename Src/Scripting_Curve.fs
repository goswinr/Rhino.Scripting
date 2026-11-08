namespace Rhino.Scripting

open Rhino
open System.ComponentModel
open System
open Microsoft.FSharp.Core.LanguagePrimitives
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils


[<AutoOpen; EditorBrowsable(EditorBrowsableState.Never)>]
module AutoOpenCurve =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    static member private TryDirectCurveInput(geometry:GeometryBase, segmentIndex:int) : Curve option =
        match geometry with
        | :? PolyCurve as curve when segmentIndex >= 0 ->
            let segment = curve.SegmentCurve(segmentIndex)
            if isNull segment then None else Some segment
        | :? Curve as curve -> Some curve
        | _ -> None

    static member private DirectCurveInput(curve:Curve, segmentIndex:int) : Curve =
        match RhinoScriptSyntax.TryDirectCurveInput(curve, segmentIndex) with
        | Some result -> result
        | None -> RhinoScriptingException.Raise "Curve: Null curve or invalid segment index %d" segmentIndex

    /// <summary>Adds an arc curve to the document.</summary>
    /// <param name="plane">(Plane) Plane on which the arc will lie. The origin of the plane will be the center point of the arc. The x-axis of the plane defines the 0 angle direction.</param>
    /// <param name="radius">(float) Radius of the arc.</param>
    /// <param name="angleDegrees">(float) Interval of arc in degrees.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddArc(plane:Plane, radius:float, angleDegrees:float) : Guid =
        let radians = toRadians(angleDegrees)
        let arc = Arc(plane, radius, radians)
        let rc = State.Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddArc: Unable to add arc to document.  plane:'%A' radius:'%A' angleDegrees:'%A'" plane radius angleDegrees
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a 3-point arc curve to the document.</summary>
    /// <param name="start">(Point3d) Start of the arc.</param>
    /// <param name="ende">(Point3d) Endpoint of the arc.</param>
    /// <param name="pointOnArc">(Point3d) A point on the arc.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddArc3Pt(start:Point3d, ende:Point3d, pointOnArc:Point3d) : Guid =
        let arc = Arc(start, pointOnArc, ende)
        let rc = State.Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddArc3Pt: Unable to add arc to document.  start:'%A' ende:'%A' pointOnArc:'%A'" start ende pointOnArc
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds an arc curve, created from a start point, a start direction, and an end point, to the document.</summary>
    /// <param name="start">(Point3d) The starting point of the arc.</param>
    /// <param name="direction">(Vector3d) The arc direction at start.</param>
    /// <param name="ende">(Point3d) The ending point of the arc.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddArcPtTanPt(start:Point3d, direction:Vector3d, ende:Point3d) : Guid =
        let arc = Arc(start, direction, ende)
        let rc = State.Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddArcPtTanPt: Unable to add arc to document.  start:'%A' direction:'%A' ende:'%A'" start direction ende
        State.Doc.Views.Redraw()
        rc


    /// <summary>Makes a curve blend between two curves.</summary>
    /// <param name="curves">(Guid * Guid) Tuple of two curves.</param>
    /// <param name="parameters">(float * float) Tuple of two curve parameters defining the blend end points.</param>
    /// <param name="reverses">(bool * bool) Tuple of two boolean values specifying to use the natural or opposite direction of the curve.</param>
    /// <param name="continuities">(int * int) Tuple of two numbers specifying continuity at end points: 0 = position, 1 = tangency, 2 = curvature.</param>
    /// <returns>(Guid) Identifier of new curve.</returns>
    static member AddBlendCurve(curves:Guid * Guid, parameters:float * float, reverses:bool * bool, continuities:int * int) : Guid =
        let geometry = RhinoScriptSyntax.MakeBlendCurve((RhinoScriptSyntax.CoerceCurve(fst curves), RhinoScriptSyntax.CoerceCurve(snd curves)), parameters, reverses, continuities)
        let rc = State.Doc.Objects.AddCurve(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddBlendCurve: Unable to add geometry to document"
        State.Doc.Views.Redraw()
        rc

    /// <summary>Computes MakeBlendCurve from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curves">(Curve * Curve) The pair of input curves.</param>
    /// <param name="parameters">(float * float) Tuple of two curve parameters defining the blend end points.</param>
    /// <param name="reverses">(bool * bool) Tuple of two boolean values specifying to use the natural or opposite direction of the curve.</param>
    /// <param name="continuities">(int * int) Tuple of two numbers specifying continuity at end points: 0 = position, 1 = tangency, 2 = curvature.</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member MakeBlendCurve(curves:Curve * Curve, parameters:float * float, reverses:bool * bool, continuities:int * int) : Curve =
        let crv0 = RhinoScriptSyntax.DirectCurveInput(fst curves, -1)
        let crv1 = RhinoScriptSyntax.DirectCurveInput(snd curves, -1)
        let c0:BlendContinuity = EnumOfValue(fst continuities)
        let c1:BlendContinuity = EnumOfValue(snd continuities)
        let curve = Curve.CreateBlendCurve(crv0, fst parameters, fst reverses, c0, crv1, snd parameters, snd reverses, c1)
        if isNull curve then RhinoScriptingException.Raise "MakeBlendCurve: Unable to create blend curve"
        curve

    /// <summary>Adds a circle curve to the document.</summary>
    /// <param name="plane">(Plane) Plane on which the circle will lie.</param>
    /// <param name="radius">(float) The radius of the circle.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddCircle(plane:Plane, radius:float) : Guid =
        let circle = Circle(plane, radius)
        let rc = State.Doc.Objects.AddCircle(circle)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddCircle: Unable to add circle to document.  plane:'%O' radius:'%f'" plane radius
        State.Doc.Views.Redraw()
        rc

    /// <summary>Adds a circle curve to the document.</summary>
    /// <param name="center">(Point3d) Center of the circle. Plane will be world XY.</param>
    /// <param name="radius">(float) The radius of the circle.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddCircle(center:Point3d, radius:float) : Guid =
        let circle = Circle(center, radius)
        let rc = State.Doc.Objects.AddCircle(circle)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddCircle: Unable to add circle to document.  center:'%s' radius:'%f'" center.Pretty radius
        State.Doc.Views.Redraw()
        rc

    /// <summary>Adds a 3-point circle curve to the document.</summary>
    /// <param name="first">(Point3d) First point on the circle.</param>
    /// <param name="second">(Point3d) Second point on the circle.</param>
    /// <param name="third">(Point3d) Third point on the circle.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddCircle3Pt(first:Point3d, second:Point3d, third:Point3d) : Guid =
        let circle = Circle(first, second, third)
        let rc = State.Doc.Objects.AddCircle(circle)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddCircle3Pt: Unable to add circle to document.  first:'%A' second:'%A' third:'%A'" first second third
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a control points curve object to the document.</summary>
    /// <param name="points">(Point3d seq) A list of points.</param>
    /// <param name="degree">(int) Optional, default value: <c>3</c>. Degree of the curve.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddCurve(points:Point3d seq, [<OPT;DEF(3)>]degree:int) : Guid =
        let  curve = Curve.CreateControlPointCurve(points, degree)
        if isNull curve then RhinoScriptingException.Raise "AddCurve: Unable to create control point curve from given points.  points:'%A' degree:'%A'" points degree
        let  rc = State.Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddCurve: Unable to add curve to document.  points:'%A' degree:'%A'" points degree
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds an elliptical curve to the document.</summary>
    /// <param name="plane">(Plane) The plane on which the ellipse will lie. The origin of the plane will be the center of the ellipse.</param>
    /// <param name="radiusX">(float) Radius in the X axis direction.</param>
    /// <param name="radiusY">(float) Radius in the Y axis direction.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddEllipse(plane:Plane, radiusX:float, radiusY:float) : Guid =
        let ellipse = Ellipse(plane, radiusX, radiusY)
        let rc = State.Doc.Objects.AddEllipse(ellipse)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddEllipse: Unable to add curve to document. plane:'%A' radiusX:'%A' radiusY:'%A'" plane radiusX radiusY
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a 3-point elliptical curve to the document.</summary>
    /// <param name="center">(Point3d) Center point of the ellipse.</param>
    /// <param name="second">(Point3d) End point of the x axis.</param>
    /// <param name="third">(Point3d) End point of the y axis.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddEllipse3Pt(center:Point3d, second:Point3d, third:Point3d) : Guid =
        let  ellipse = Ellipse(center, second, third)
        let  rc = State.Doc.Objects.AddEllipse(ellipse)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddEllipse3Pt: Unable to add curve to document.  center:'%A' second:'%A' third:'%A'" center second third
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a fillet curve between two curve objects.</summary>
    /// <param name="curveA">(Guid) Identifier of the first curve object.</param>
    /// <param name="curveB">(Guid) Identifier of the second curve object.</param>
    /// <param name="radius">(float) Optional, default value: <c>1.0</c>. Fillet radius.</param>
    /// <param name="basePointA">(Point3d) Optional. Base point of the first curve. If omitted, starting point of the curve is used.</param>
    /// <param name="basePointB">(Point3d) Optional. Base point of the second curve. If omitted, starting point of the curve is used.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddFilletCurve(curveA:Guid, curveB:Guid, [<OPT;DEF(1.0)>]radius:float, [<OPT;DEF(Point3d())>]basePointA:Point3d, [<OPT;DEF(Point3d())>]basePointB:Point3d) : Guid =
        let geometry = RhinoScriptSyntax.MakeFilletCurve(RhinoScriptSyntax.CoerceCurve(curveA), RhinoScriptSyntax.CoerceCurve(curveB), radius, basePointA, basePointB)
        let rc = State.Doc.Objects.AddCurve(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddFilletCurve: Unable to add geometry to document"
        State.Doc.Views.Redraw()
        rc

    /// <summary>Computes MakeFilletCurve from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveA">(Curve) The input geometry.</param>
    /// <param name="curveB">(Curve) The input geometry.</param>
    /// <param name="radius">(float) Optional, default value: <c>1.0</c>. Fillet radius.</param>
    /// <param name="basePointA">(Point3d) Optional. Base point of the first curve. If omitted, starting point of the curve is used.</param>
    /// <param name="basePointB">(Point3d) Optional. Base point of the second curve. If omitted, starting point of the curve is used.</param>
    /// <returns>(ArcCurve) The resulting geometry; it is not added to a document.</returns>
    static member MakeFilletCurve(curveA:Curve, curveB:Curve, [<OPT;DEF(1.0)>]radius:float, [<OPT;DEF(Point3d())>]basePointA:Point3d, [<OPT;DEF(Point3d())>]basePointB:Point3d) : ArcCurve =
        //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let basePointA = if basePointA = Point3d.Origin then Point3d.Unset else basePointA
        let basePointB = if basePointB = Point3d.Origin then Point3d.Unset else basePointB
        let  curve0 = RhinoScriptSyntax.DirectCurveInput(curveA, -1)
        let  curve1 = RhinoScriptSyntax.DirectCurveInput(curveB, -1)
        let mutable crv0T = 0.0
        if basePointA= Point3d.Unset then
            crv0T <- curve0.Domain.Min
        else
            let rc, t = curve0.ClosestPoint(basePointA)
            if not <| rc then RhinoScriptingException.Raise "MakeFilletCurve ClosestPoint failed.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
            crv0T <- t
        let mutable crv1T = 0.0
        if basePointB= Point3d.Unset then
            crv1T <- curve1.Domain.Min
        else
            let rc, t = curve1.ClosestPoint(basePointB)
            if not <| rc then RhinoScriptingException.Raise "MakeFilletCurve ClosestPoint failed.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
            crv1T <- t
        let arc = Curve.CreateFillet(curve0, curve1, radius, crv0T, crv1T)
        if not arc.IsValid then RhinoScriptingException.Raise "MakeFilletCurve: Unable to create a fillet arc.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
        new ArcCurve(arc)

    /// <summary>Adds an interpolated curve object that lies on a specified surface. Note, this function will not create periodic curves, but it will create closed curves.</summary>
    /// <param name="surfaceId">(Guid) Identifier of the surface to create the curve on.</param>
    /// <param name="points">(Point3d seq) List of 3D points that lie on the specified surface. The list must contain at least 2 points.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddInterpCrvOnSrf(surfaceId:Guid, points:Point3d seq) : Guid =
        let geometry = RhinoScriptSyntax.MakeInterpCrvOnSrf(RhinoScriptSyntax.CoerceSurface(surfaceId), points)
        let rc = State.Doc.Objects.AddCurve(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddInterpCrvOnSrf: Unable to add geometry to document"
        State.Doc.Views.Redraw()
        rc

    /// <summary>Computes MakeInterpCrvOnSrf from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="surfaceGeo">(Surface) The input geometry.</param>
    /// <param name="points">(Point3d seq) List of 3D points that lie on the specified surface. The list must contain at least 2 points.</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member MakeInterpCrvOnSrf(surfaceGeo:Surface, points:Point3d seq) : Curve =
        let  surface = (surfaceGeo)
        let  tolerance = State.Doc.ModelAbsoluteTolerance
        let  curve = surface.InterpolatedCurveOnSurface(points, tolerance)
        if isNull curve then RhinoScriptingException.Raise "MakeInterpCrvOnSrf: Unable to create InterpolatedCurveOnSurface.  surfaceGeo:'%s' points:'%A'" (Pretty.str surfaceGeo) points
        curve

    /// <summary>Adds an interpolated curve object based on surface parameters, that lies on a specified surface. Note, this function will not create periodic curves, but it will create closed curves.</summary>
    /// <param name="surfaceId">(Guid) Identifier of the surface to create the curve on.</param>
    /// <param name="points">(Point2d seq) A list of 2D surface parameters. The list must contain at least 2 sets of parameters.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddInterpCrvOnSrfUV(surfaceId:Guid, points:Point2d seq) : Guid =
        let geometry = RhinoScriptSyntax.MakeInterpCrvOnSrfUV(RhinoScriptSyntax.CoerceSurface(surfaceId), points)
        let rc = State.Doc.Objects.AddCurve(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddInterpCrvOnSrfUV: Unable to add geometry to document"
        State.Doc.Views.Redraw()
        rc

    /// <summary>Computes MakeInterpCrvOnSrfUV from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="surfaceGeo">(Surface) The input geometry.</param>
    /// <param name="points">(Point2d seq) A list of 2D surface parameters. The list must contain at least 2 sets of parameters.</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member MakeInterpCrvOnSrfUV(surfaceGeo:Surface, points:Point2d seq) : Curve =
        let mutable surface = (surfaceGeo)
        let mutable tolerance = State.Doc.ModelAbsoluteTolerance
        let mutable curve = surface.InterpolatedCurveOnSurfaceUV(points, tolerance)
        if isNull curve then RhinoScriptingException.Raise "MakeInterpCrvOnSrfUV: Unable to create InterpolatedCurveOnSurfaceUV.  surfaceGeo:'%s' points:'%A'" (Pretty.str surfaceGeo) points
        curve

    ///<summary>Adds an interpolated Curve object to the document. Options exist to make
    ///    a periodic Curve or to specify the tangent at the endpoints. The resulting
    ///    Curve is a non-rational NURBS Curve of the specified degree.</summary>
    ///<param name="points">(Point3d seq) A list containing 3D points to interpolate. For periodic Curves,
    ///    if the final point is a duplicate of the initial point, it is
    ///    ignored. The number of control points must be bigger than 'degree' number</param>
    ///<param name="degree">(int) Optional, default value: <c>3</c>
    ///    Periodic Curves must have a degree bigger than 1. For knot-style = 1 or 2,
    ///    the degree must be 3. For knot-style = 4 or 5, the degree must be odd</param>
    ///<param name="knotStyle">(int) Optional, default value: <c>0</c>
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
    static member AddInterpCurve(   points:Point3d seq,
                                    [<OPT;DEF(3)>]degree:int,
                                    [<OPT;DEF(0)>]knotStyle:int,
                                    [<OPT;DEF(Vector3d())>]startTangent:Vector3d,  //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
                                    [<OPT;DEF(Vector3d())>]endTangent:Vector3d) : Guid =
        let endTangent   = if endTangent.IsZero then Vector3d.Unset else endTangent
        let startTangent = if startTangent.IsZero then Vector3d.Unset else startTangent
        let knotstyle : CurveKnotStyle = EnumOfValue knotStyle
        let  curve = Curve.CreateInterpolatedCurve(points, degree, knotstyle, startTangent, endTangent)
        if isNull curve then RhinoScriptingException.Raise "AddInterpCurve: Unable to CreateInterpolatedCurve.  points:'%A' degree:'%A' knotStyle:%d startTangent:'%A' endTangent:'%A'" points degree knotStyle startTangent endTangent
        let  rc = State.Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddInterpCurve: Unable to add curve to document.  points:'%A' degree:'%A' knotStyle:%d startTangent:'%A' endeTangent:'%A'" points degree knotStyle startTangent endTangent
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a line curve to the current model.</summary>
    /// <param name="start">(Point3d) Startpoint of the line.</param>
    /// <param name="ende">(Point3d) Endpoint of the line.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddLine(start:Point3d, ende:Point3d) : Guid =
        let  rc = State.Doc.Objects.AddLine(start, ende)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddLine: Unable to add line to document. start:%s ende:%s" start.Pretty ende.Pretty
        State.Doc.Views.Redraw()
        rc

    /// <summary>Adds a line curve to the current model.</summary>
    /// <param name="startX">(float) Startpoint of the line: X position.</param>
    /// <param name="startY">(float) Startpoint of the line: Y position.</param>
    /// <param name="startZ">(float) Startpoint of the line: Z position.</param>
    /// <param name="endX">(float) Endpoint of the line: X position.</param>
    /// <param name="endY">(float) Endpoint of the line: Y position.</param>
    /// <param name="endZ">(float) Endpoint of the line:Z position.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddLine(startX,startY,startZ,endX,endY,endZ:float) : Guid =
        let start = Point3d(startX,startY,startZ)
        let ende = Point3d(endX,endY,endZ)
        let  rc = State.Doc.Objects.AddLine(start, ende)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddLine: Unable to add line to document. startX:%g ,startY:%g ,startZ:%g and endX:%g ,endY:%g ,endZ:%g" startX startY startZ endX endY endZ
        State.Doc.Views.Redraw()
        rc

    /// <summary>Adds a 2D line curve to the current model at z level 0.0</summary>
    /// <param name="startX">(float) Startpoint of the line: X position.</param>
    /// <param name="startY">(float) Startpoint of the line: Y position.</param>
    /// <param name="endX">(float) Endpoint of the line: X position.</param>
    /// <param name="endY">(float) Endpoint of the line: Y position.</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddLine2D(startX,startY,endX,endY:float) : Guid =
        let start = Point3d(startX,startY,0.0)
        let ende = Point3d(endX,endY,0.0)
        let  rc = State.Doc.Objects.AddLine(start, ende)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddLine2D: Unable to add line to document. startX:%g ,startY:%g  and  endX:%g ,endY:%g," startX startY  endX endY
        State.Doc.Views.Redraw()
        rc

    /// <summary>Creates a NURBS Curve geometry, but does not add or draw it to the document.</summary>
    /// <param name="points">(Point3d seq) A list containing 3D control points.</param>
    /// <param name="knots">(float seq) Knot values for the Curve. The number of elements in knots must
    ///    equal the number of elements in points plus degree minus 1.</param>
    /// <param name="degree">(int) Degree of the Curve. must be greater than of equal to 1.</param>
    /// <param name="weights">(float seq) Optional, Weight values for the Curve. Number of elements should
    ///    equal the number of elements in points. Values must be greater than 0.</param>
    /// <returns>(NurbsCurve) a NurbsCurve geometry.</returns>
    static member CreateNurbsCurve(points:Point3d seq, knots:float seq, degree:int, [<OPT;DEF(null: float seq)>]weights:float seq) : NurbsCurve =
        let cvcount = Seq.length(points)
        let knotcount = cvcount + degree - 1
        if Seq.length(knots)<>knotcount then
            RhinoScriptingException.Raise "CreateNurbsCurve:Number of elements in knots must equal the number of elements in points plus degree minus 1.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        let rational =
            if notNull weights then
                if Seq.length(weights)<>cvcount then
                    RhinoScriptingException.Raise "CreateNurbsCurve:Number of elements in weights should equal the number of elements in points.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
                true
            else
            false
        let nc = new NurbsCurve(3, rational, degree + 1, cvcount)
        for i, k in Seq.indexed knots do
            nc.Knots.[i] <- k
        if notNull weights then
            if Seq.length(weights)<>cvcount then
                RhinoScriptingException.Raise "CreateNurbsCurve:Number of elements in weights should equal the number of elements in points.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
            for i,(p, w)  in Seq.indexed (Seq.zip points weights) do
                nc.Points.SetPoint(i, p, w) |> ignore<bool>
        else
            for i, p in Seq.indexed points do
                nc.Points.SetPoint(i, p) |> ignore<bool>
        if not nc.IsValid then RhinoScriptingException.Raise "CreateNurbsCurve:Unable to create curve.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        nc

    /// <summary>Adds a NURBS Curve object to the document.</summary>
    /// <param name="points">(Point3d seq) A list containing 3D control points.</param>
    /// <param name="knots">(float seq) Knot values for the Curve. The number of elements in knots must
    ///    equal the number of elements in points plus degree minus 1.</param>
    /// <param name="degree">(int) Degree of the Curve. must be greater than of equal to 1.</param>
    /// <param name="weights">(float seq) Optional, Weight values for the Curve. Number of elements should
    ///    equal the number of elements in points. Values must be greater than 0.</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member AddNurbsCurve(points:Point3d seq, knots:float seq, degree:int, [<OPT;DEF(null: float seq)>]weights:float seq) : Guid =
        let nc = RhinoScriptSyntax.CreateNurbsCurve(points, knots, degree, weights)
        let rc = State.Doc.Objects.AddCurve(nc)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddNurbsCurve: Unable to add curve to document.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a Polyline Curve.</summary>
    /// <param name="points">(Point3d seq) List of 3D points. The list must contain at least two points. If the list contains less than four points, then the first point and last point must be different.</param>
    /// <param name="deleteShortSegments">(bool) Optional, default value: <c>true</c>. If true, segments shorter than the document's absolute tolerance will be deleted.</param>
    /// <param name="drawDotsAndRaiseIfFailed">(bool) Optional, default value: <c>false</c>. If true, text dots will be drawn on layer `ERROR-AddPolyline` at each point and an exception will be raised if the operation fails.</param>
    /// <returns>(Guid) ObjectId of the new curve object. Empty Guid if the operation fails.</returns>
    static member AddPolyline(points:Point3d seq,  [<OPT;DEF(true)>]deleteShortSegments: bool, [<OPT;DEF(false)>]drawDotsAndRaiseIfFailed: bool) : Guid =
        let pl = Polyline(points)
        if deleteShortSegments then
            pl.DeleteShortSegments(State.Doc.ModelAbsoluteTolerance) |> ignore<int>
        let rc = State.Doc.Objects.AddPolyline(pl)
        if drawDotsAndRaiseIfFailed && rc = Guid.Empty then
            for i,pt in Seq.indexed(points) do
                let d = State.Doc.Objects.AddTextDot(string i, pt)
                RhinoScriptSyntax.ObjectLayer(d,"ERROR-AddPolyline", createLayerIfMissing=true)
            // eprintfn "See %d TextDots on layer 'ERROR-AddPolyline'"  (Seq.length points)
            RhinoScriptingException.Raise "AddPolyline: Unable to add polyline to document form points:%s'%A'" Environment.NewLine (Pretty.str points)
        State.Doc.Views.Redraw()
        rc

    /// <summary>Adds a closed Polyline Curve.
    ///    If the endpoint is already closer than State.Doc.ModelAbsoluteTolerance to the start it wil be set to start point
    ///    else an additional point will be added with the same position as start.
    /// </summary>
    /// <param name="points">(Point3d seq) List of 3D points. The list must contain at least three points.</param>
    /// <param name="deleteShortSegments">(bool) Optional, default value: <c>true</c>. If true, segments shorter than the document's absolute tolerance will be deleted.</param>
    /// <param name="drawDotsAndRaiseIfFailed">(bool) Optional, default value: <c>false</c>. If true, text dots will be drawn on layer `ERROR-AddPolylineClosed` at each point and an exception will be raised if the operation fails.</param>
    /// <returns>(Guid) ObjectId of the new curve object. Empty Guid if the operation fails.</returns>
    static member AddPolylineClosed(points:Point3d seq,  [<OPT;DEF(true)>]deleteShortSegments: bool, [<OPT;DEF(false)>]drawDotsAndRaiseIfFailed: bool) : Guid =
        let pl = Polyline(points)
        if pl.Count < 3 then RhinoScriptingException.Raise "AddPolylineClosed: Unable to add closed polyline to document from points:%s'%A'" Environment.NewLine (Pretty.str points)
        if (pl.First-pl.Last).Length <= State.Doc.ModelAbsoluteTolerance then
            pl.[pl.Count-1] <- pl.First
        else
            pl.Add pl.First
        if deleteShortSegments then
            pl.DeleteShortSegments(State.Doc.ModelAbsoluteTolerance) |> ignore<int>
        let rc = State.Doc.Objects.AddPolyline(pl)
        if drawDotsAndRaiseIfFailed && rc = Guid.Empty then
            for i,pt in Seq.indexed(points) do
                let d = State.Doc.Objects.AddTextDot(string i, pt)
                RhinoScriptSyntax.ObjectLayer(d,"ERROR-AddPolylineClosed", createLayerIfMissing=true)
            // eprintfn "See %d TextDots on layer 'ERROR-AddPolylineClosed'"  (Seq.length points)
            RhinoScriptingException.Raise "AddPolylineClosed: Unable to add closed polyline to document. points:'%A'" points
        State.Doc.Views.Redraw()
        rc

    /// <summary>Add a rectangular Curve to the document.</summary>
    /// <param name="plane">(Plane) Plane on which the rectangle will lie</param>
    /// <param name="width">(float) Width of rectangle as measured along the Plane's
    ///    x and y axes</param>
    /// <param name="height">(float) Height of rectangle as measured along the Plane's
    ///    x and y axes</param>
    /// <returns>(Guid) ObjectId of new rectangle.</returns>
    static member AddRectangle(plane:Plane, width:float, height:float) : Guid =
        let rect = Rectangle3d(plane, width, height)
        let poly = rect.ToPolyline()
        let rc = State.Doc.Objects.AddPolyline(poly)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddRectangle: Unable to add polyline to document.  plane:'%A' width:'%A' height:'%A'" plane width height
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a spiral or helical curve to the document.</summary>
    /// <param name="point0">(Point3d) Helix axis start point or center of spiral.</param>
    /// <param name="point1">(Point3d) Helix axis end point or point normal on spiral Plane.</param>
    /// <param name="pitch">(float) Distance between turns. If 0, then a spiral. If > 0 then the
    ///    distance between helix "threads".</param>
    /// <param name="turns">(float) Number of turns.</param>
    /// <param name="radius0">(float) Starting radius of spiral.</param>
    /// <param name="radius1">(float) Optional, Ending radius of spiral. If omitted, the starting radius is used for the complete spiral.</param>
    /// <returns>(Guid) ObjectId of new curve.</returns>
    static member AddSpiral(point0:Point3d, point1:Point3d, pitch:float, turns:float, radius0:float, [<OPT;DEF(0.0)>]radius1:float) : Guid =
        let dir = point1 - point0
        let plane = Plane(point0, dir)
        let point2 = point0 + plane.XAxis
        let r2 = if radius1 = 0.0 then radius0 else radius1
        let curve = NurbsCurve.CreateSpiral(point0, dir, point2, pitch, turns, radius0, r2)
        if isNull curve then RhinoScriptingException.Raise "AddSpiral: Unable to add curve to document.  point0:'%A' point1:'%A' pitch:'%A' turns:'%A' radius0:'%A' radius1:'%A'" point0 point1 pitch turns radius0 radius1
        let rc = State.Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddSpiral: Unable to add curve to document.  point0:'%A' point1:'%A' pitch:'%A' turns:'%A' radius0:'%A' radius1:'%A'" point0 point1 pitch turns radius0 radius1
        State.Doc.Views.Redraw()
        rc


    /// <summary>Add a Curve object based on a portion, or interval of an existing Curve
    ///    object. Similar in operation to Rhino's SubCrv command.</summary>
    /// <param name="curveId">(Guid) Identifier of a closed planar Curve object</param>
    /// <param name="param0">(float) First parameters on the source Curve</param>
    /// <param name="param1">(float) Second parameters on the source Curve</param>
    /// <returns>(Guid) ObjectId of the new curve object.</returns>
    static member AddSubCrv(curveId:Guid, param0:float, param1:float) : Guid =
        let geometry = RhinoScriptSyntax.MakeSubCrv(RhinoScriptSyntax.CoerceCurve(curveId), param0, param1)
        let rc = State.Doc.Objects.AddCurve(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddSubCrv: Unable to add geometry to document"
        State.Doc.Views.Redraw()
        rc

    /// <summary>Computes MakeSubCrv from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="param0">(float) First parameters on the source Curve</param>
    /// <param name="param1">(float) Second parameters on the source Curve</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member MakeSubCrv(curveGeo:Curve, param0:float, param1:float) : Curve =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let trimcurve = curve.Trim(param0, param1)
        if isNull trimcurve then RhinoScriptingException.Raise "MakeSubCrv: Unable to trim curve. curveGeo:'%s' param0:'%A' param1:'%A'" (Pretty.str curveGeo) param0 param1
        trimcurve

    /// <summary>Returns the angle of an arc Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    Identifies the Curve segment if CurveId identifies a poly-curve</param>
    /// <returns>(float) The angle in degrees.</returns>
    static member ArcAngle(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        RhinoScriptSyntax.ArcAngle(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the angle of an arc Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    Identifies the Curve segment if CurveId identifies a poly-curve</param>
    /// <returns>(float) The angle in degrees.</returns>
    static member ArcAngle(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "ArcAngle: Curve is not an arc. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        (arc.Value).AngleDegrees


    /// <summary>Returns the center point of an arc Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(Point3d) The 3D center point of the arc.</returns>
    static member ArcCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        RhinoScriptSyntax.ArcCenterPoint(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the center point of an arc Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(Point3d) The 3D center point of the arc.</returns>
    static member ArcCenterPoint(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "ArcCenterPoint: Curve is not an arc. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        (arc.Value).Center


    /// <summary>Returns the mid point of an arc Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(Point3d) The 3D mid point of the arc.</returns>
    static member ArcMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        RhinoScriptSyntax.ArcMidPoint(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the mid point of an arc Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(Point3d) The 3D mid point of the arc.</returns>
    static member ArcMidPoint(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "ArcMidPoint: Curve is not an arc. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        (arc.Value).MidPoint


    /// <summary>Returns the radius of an arc Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(float) The radius of the arc.</returns>
    static member ArcRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        RhinoScriptSyntax.ArcRadius(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the radius of an arc Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(float) The radius of the arc.</returns>
    static member ArcRadius(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "ArcRadius: Curve is not an arc. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        (arc.Value).Radius



    /// <summary>Returns the center point of a circle Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(Point3d) The 3D center point of the circle.</returns>
    static member CircleCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        RhinoScriptSyntax.CircleCenterPoint(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the center point of a circle Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(Point3d) The 3D center point of the circle.</returns>
    static member CircleCenterPoint(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "CircleCenterPoint: Curve is not circle. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        (!circle).Center


    /// <summary>Returns the center Plane of a circle Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(Plane) The 3D Plane at the center point of the circle.</returns>
    static member CircleCenterPlane(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        RhinoScriptSyntax.CircleCenterPlane(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the center Plane of a circle Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(Plane) The 3D Plane at the center point of the circle.</returns>
    static member CircleCenterPlane(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "CircleCenterPlane: Curve is not circle. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        (!circle).Plane





    /// <summary>Returns the circumference of a circle Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(float) The circumference of the circle.</returns>
    static member CircleCircumference(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        RhinoScriptSyntax.CircleCircumference(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the circumference of a circle Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(float) The circumference of the circle.</returns>
    static member CircleCircumference(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "CircleCircumference: Curve is not circle. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        (!circle).Circumference


    /// <summary>Returns the radius of a circle Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(float) The radius of the circle.</returns>
    static member CircleRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        RhinoScriptSyntax.CircleRadius(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the radius of a circle Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a poly-curve</param>
    /// <returns>(float) The radius of the circle.</returns>
    static member CircleRadius(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then RhinoScriptingException.Raise "CircleRadius: Curve is not circle. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        (!circle).Radius


    /// <summary>Closes an open Curve object by making adjustments to the end points so
    ///    they meet at a point.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Maximum allowable distance between start and end point</param>
    /// <returns>(Guid) ObjectId of the new Curve object.</returns>
    static member CloseCurve(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if curve.IsClosed then curveId
        else
            let closed = RhinoScriptSyntax.CloseCurve(curve, tolerance)
            let rc = State.Doc.Objects.AddCurve(closed)
            if rc = Guid.Empty then RhinoScriptingException.Raise "CloseCurve: Unable to add curve to document"
            State.Doc.Views.Redraw()
            rc

    /// <summary>Computes CloseCurve from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input curve geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Maximum allowable distance between start and end point</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member CloseCurve(curveGeo:Curve, [<OPT;DEF(0.0)>]tolerance:float) : Curve =
        let curve = (RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)).DuplicateCurve()
        if not curve.IsClosed && not (curve.MakeClosed(Util.ifZero1 tolerance State.Doc.ModelAbsoluteTolerance)) then
            RhinoScriptingException.Raise "CloseCurve: Unable to close curve"
        curve

    /// <summary>Determine the orientation (counter-clockwise or clockwise) of a closed,
    ///    planar Curve.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="direction">(Vector3d) Optional, default value: <c>Vector3d.ZAxis</c>
    ///    3d vector that identifies up, or Z axs, direction of
    ///    the Plane to test against</param>
    /// <returns>(int) 1 if the Curve's orientation is clockwise
    ///    -1 if the Curve's orientation is counter-clockwise
    ///     0 if unable to compute the Curve's orientation.</returns>
    static member ClosedCurveOrientation(curveId:Guid, [<OPT;DEF(Vector3d())>]direction:Vector3d) : int = //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        RhinoScriptSyntax.ClosedCurveOrientation(RhinoScriptSyntax.CoerceCurve(curveId), direction)

    /// <summary>Determine the orientation (counter-clockwise or clockwise) of a closed,
    ///    planar Curve. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="direction">(Vector3d) Optional, default value: <c>Vector3d.ZAxis</c>
    ///    3d vector that identifies up, or Z axs, direction of
    ///    the Plane to test against</param>
    /// <returns>(int) 1 if the Curve's orientation is clockwise
    ///    -1 if the Curve's orientation is counter-clockwise
    ///     0 if unable to compute the Curve's orientation.</returns>
    static member ClosedCurveOrientation(curveGeo:Curve, [<OPT;DEF(Vector3d())>]direction:Vector3d) : int = //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let direction0 =if direction.IsZero then Vector3d.Unset else direction
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        if not <| curve.IsClosed then  0
        else
            let orientation = curve.ClosedCurveOrientation(direction0)
            int(orientation)


    /// <summary>Convert Curve to a Polyline Curve.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="angleTolerance">(float) Optional, default value: <c>5.0</c>
    ///    The maximum angle between Curve tangents at line endpoints.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>0.01</c>
    ///    The distance tolerance at segment midpoints.</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Delete the Curve object specified by CurveId. If omitted, CurveId will not be deleted</param>
    /// <param name="minEdgeLength">(float) Optional, Minimum segment length</param>
    /// <param name="maxEdgeLength">(float) Optional, Maximum segment length</param>
    /// <returns>(Guid) The new Curve.</returns>
    static member ConvertCurveToPolyline(   curveId:Guid,
                                            [<OPT;DEF(0.0)>]angleTolerance:float,
                                            [<OPT;DEF(0.0)>]tolerance:float,
                                            [<OPT;DEF(false)>]deleteInput:bool,
                                            [<OPT;DEF(0.0)>]minEdgeLength:float,
                                            [<OPT;DEF(0.0)>]maxEdgeLength:float) : Guid =
        let polyline = RhinoScriptSyntax.ConvertCurveToPolyline(RhinoScriptSyntax.CoerceCurve(curveId), angleTolerance, tolerance, minEdgeLength, maxEdgeLength)
        if deleteInput then
            if not (State.Doc.Objects.Replace(curveId, polyline)) then RhinoScriptingException.Raise "ConvertCurveToPolyline: Unable to replace curve %O" curveId
            curveId
        else
            let rc = State.Doc.Objects.AddCurve(polyline)
            if rc = Guid.Empty then RhinoScriptingException.Raise "ConvertCurveToPolyline: Unable to add curve to document"
            rc

    /// <summary>Computes ConvertCurveToPolyline from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input curve geometry.</param>
    /// <param name="angleTolerance">(float) Optional, default value: <c>5.0</c>
    ///    The maximum angle between Curve tangents at line endpoints.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>0.01</c>
    ///    The distance tolerance at segment midpoints.</param>
    /// <param name="minEdgeLength">(float) Optional, Minimum segment length</param>
    /// <param name="maxEdgeLength">(float) Optional, Maximum segment length</param>
    /// <returns>(PolylineCurve) The resulting geometry; it is not added to a document.</returns>
    static member ConvertCurveToPolyline(   curveGeo:Curve,
                                            [<OPT;DEF(0.0)>]angleTolerance:float,
                                            [<OPT;DEF(0.0)>]tolerance:float,
                                            [<OPT;DEF(0.0)>]minEdgeLength:float,
                                            [<OPT;DEF(0.0)>]maxEdgeLength:float) : PolylineCurve =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let angleTolerance0 = toRadians (Util.ifZero1 angleTolerance 0.5)
        let tolerance0 = Util.ifZero1 tolerance 0.01
        let polyline = curve.ToPolyline(0, 0, angleTolerance0, 0.0, 0.0, tolerance0, minEdgeLength, maxEdgeLength, keepStartPoint=true)
        if isNull polyline then RhinoScriptingException.Raise "ConvertCurveToPolyline: Unable to convert curve"
        polyline

    /// <summary>Returns the point on the Curve that is a specified arc length
    ///    from the start of the Curve.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="length">(float) The arc length from the start of the Curve to evaluate</param>
    /// <param name="fromStart">(bool) Optional, default value: <c>true</c>
    ///    If not specified or True, then the arc length point is
    ///    calculated from the start of the Curve. If False, the arc length
    ///    point is calculated from the end of the Curve</param>
    /// <returns>(Point3d) on Curve.</returns>
    static member CurveArcLengthPoint(curveId:Guid, length:float, [<OPT;DEF(true)>]fromStart:bool) : Point3d =
        RhinoScriptSyntax.CurveArcLengthPoint(RhinoScriptSyntax.CoerceCurve(curveId), length, fromStart)

    /// <summary>Returns the point on the Curve that is a specified arc length
    ///    from the start of the Curve. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="length">(float) The arc length from the start of the Curve to evaluate</param>
    /// <param name="fromStart">(bool) Optional, default value: <c>true</c>
    ///    If not specified or True, then the arc length point is
    ///    calculated from the start of the Curve. If False, the arc length
    ///    point is calculated from the end of the Curve</param>
    /// <returns>(Point3d) on Curve.</returns>
    static member CurveArcLengthPoint(curveGeo:Curve, length:float, [<OPT;DEF(true)>]fromStart:bool) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let curveLength = curve.GetLength()
        if curveLength >= length then
            let mutable s = 0.0
            if length = 0.0 then  s <- 0.0
            elif abs(length-curveLength) < State.Doc.ModelAbsoluteTolerance then  s <- 1.0
            else s <- length / curveLength
            let dupe = if not fromStart then curve.Duplicate() :?> Curve else curve
            if notNull dupe then
                if not fromStart then  dupe.Reverse() |> ignore<bool>
                let rc, t = dupe.NormalizedLengthParameter(s)
                if rc then
                    let pt = dupe.PointAt(t)
                    if not fromStart then dupe.Dispose()
                    pt
                else RhinoScriptingException.Raise "CurveArcLengthPoint: Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveGeo length fromStart
            else RhinoScriptingException.Raise "CurveArcLengthPoint: Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveGeo length fromStart
        else RhinoScriptingException.Raise "CurveArcLengthPoint: Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveGeo length fromStart


    /// <summary>Returns area of closed planar Curves. The results are based on the
    ///    current drawing units.</summary>
    /// <param name="curveId">(Guid) The identifier of a closed, planar Curve object</param>
    /// <returns>(float) The area.</returns>
    static member CurveArea(curveId:Guid) : float =
        RhinoScriptSyntax.CurveArea(RhinoScriptSyntax.CoerceCurve(curveId))

    /// <summary>Returns area of closed planar Curves. The results are based on the
    ///    current drawing units. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <returns>(float) The area.</returns>
    static member CurveArea(curveGeo:Curve) : float =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let tol = State.Doc.ModelAbsoluteTolerance
        let mp = AreaMassProperties.Compute(curve, tol)
        if isNull mp  then RhinoScriptingException.Raise "CurveArea failed on %A" curveGeo
        mp.Area


    /// <summary>Returns area centroid of closed, planar Curves. The results are based
    ///    on the current drawing units.</summary>
    /// <param name="curveId">(Guid) The identifier of a closed, planar Curve object</param>
    /// <returns>(Point3d ) The 3d centroid point.</returns>
    static member CurveAreaCentroid(curveId:Guid) : Point3d =
        RhinoScriptSyntax.CurveAreaCentroid(RhinoScriptSyntax.CoerceCurve(curveId))

    /// <summary>Returns area centroid of closed, planar Curves. The results are based
    ///    on the current drawing units. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <returns>(Point3d ) The 3d centroid point.</returns>
    static member CurveAreaCentroid(curveGeo:Curve) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let tol = State.Doc.ModelAbsoluteTolerance
        let mp = AreaMassProperties.Compute(curve, tol)
        if isNull mp  then RhinoScriptingException.Raise "CurveAreaCentroid failed on %A" curveGeo
        mp.Centroid


    /// <summary>Get status of a Curve object's annotation arrows.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve</param>
    /// <returns>(int) The current annotation arrow style
    ///    0 = no arrows
    ///    1 = display arrow at start of Curve
    ///    2 = display arrow at end of Curve
    ///    3 = display arrow at both start and end of Curve.</returns>
    static member CurveArrows(curveId:Guid) : int = //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let attr = rhobj.Attributes
        let rc = attr.ObjectDecoration
        if   rc = DocObjects.ObjectDecoration.None then  0
        elif rc = DocObjects.ObjectDecoration.StartArrowhead then 1
        elif rc = DocObjects.ObjectDecoration.EndArrowhead then 2
        elif rc = DocObjects.ObjectDecoration.BothArrowhead then 3
        else RhinoScriptingException.Raise "CurveArrows: illegal state %A on curve %A" rc curveId

    /// <summary>Enables or disables a Curve object's annotation arrows.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve</param>
    /// <param name="arrowStyle">(int) The style of annotation arrow to be displayed.
    ///    0 = no arrows
    ///    1 = display arrow at start of Curve
    ///    2 = display arrow at end of Curve
    ///    3 = display arrow at both start and end of Curve</param>
    /// <returns>(unit) void, nothing.</returns>
    static member CurveArrows(curveId:Guid, arrowStyle:int) : unit = //SET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let attr = rhobj.Attributes
        //let rc = attr.ObjectDecoration
        if arrowStyle >= 0 && arrowStyle <= 3 then
            if arrowStyle = 0 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.None
            elif arrowStyle = 1 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.StartArrowhead
            elif arrowStyle = 2 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.EndArrowhead
            elif arrowStyle = 3 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.BothArrowhead
            if not <| State.Doc.Objects.ModifyAttributes(curveId, attr, quiet=true) then
                RhinoScriptingException.Raise "CurveArrows ModifyAttributes failed on style %d on %s" arrowStyle  (Pretty.str curveId)
            State.Doc.Views.Redraw()
        else
            RhinoScriptingException.Raise "CurveArrows style %d is invalid" arrowStyle

    /// <summary>Enables or disables multiple Curve objects's annotation arrows.</summary>
    /// <param name="curveIds">(Guid seq) Identifier of multiple Curve</param>
    /// <param name="arrowStyle">(int) The style of annotation arrow to be displayed.
    ///    0 = no arrows
    ///    1 = display arrow at start of Curve
    ///    2 = display arrow at end of Curve
    ///    3 = display arrow at both start and end of Curve</param>
    /// <returns>(unit) void, nothing.</returns>
    static member CurveArrows(curveIds:Guid seq, arrowStyle:int) : unit = //MULTISET
        for curveId in curveIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
            let attr = rhobj.Attributes
            //let rc = attr.ObjectDecoration
            if arrowStyle >= 0 && arrowStyle <= 3 then
                if arrowStyle = 0 then
                    attr.ObjectDecoration <- DocObjects.ObjectDecoration.None
                elif arrowStyle = 1 then
                    attr.ObjectDecoration <- DocObjects.ObjectDecoration.StartArrowhead
                elif arrowStyle = 2 then
                    attr.ObjectDecoration <- DocObjects.ObjectDecoration.EndArrowhead
                elif arrowStyle = 3 then
                    attr.ObjectDecoration <- DocObjects.ObjectDecoration.BothArrowhead
                if not <| State.Doc.Objects.ModifyAttributes(curveId, attr, quiet=true) then
                    RhinoScriptingException.Raise "CurveArrows ModifyAttributes failed on style %d on %s" arrowStyle  (Pretty.str curveId)
            else
                RhinoScriptingException.Raise "CurveArrows: Curve Arrow style %d is invalid" arrowStyle
        State.Doc.Views.Redraw()

    /// <summary>Calculates the difference between two closed, planar Curves and
    ///    adds the results to the document. Note, Curves must be coplanar.</summary>
    /// <param name="curveA">(Guid) Identifier of the first Curve object</param>
    /// <param name="curveB">(Guid) Identifier of the second Curve object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    /// <returns>(Guid ResizeArray) The identifiers of the new objects.</returns>
    static member CurveBooleanDifference(curveA:Guid, curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let curves = RhinoScriptSyntax.CurveBooleanDifference(RhinoScriptSyntax.CoerceCurve(curveA), RhinoScriptSyntax.CoerceCurve(curveB), tolerance)
        let ids = ResizeArray<Guid>()
        for curve in curves do
            let rc = State.Doc.Objects.AddCurve(curve)
            curve.Dispose()
            if rc = Guid.Empty then RhinoScriptingException.Raise "CurveBooleanDifference: Unable to add curve to document"
            ids.Add(rc)
        State.Doc.Views.Redraw()
        ids

    /// <summary>Computes CurveBooleanDifference from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveA">(Curve) The input geometry.</param>
    /// <param name="curveB">(Curve) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    /// <returns>(Curve ResizeArray) The resulting geometry; it is not added to a document.</returns>
    static member CurveBooleanDifference(curveA:Curve, curveB:Curve, [<OPT;DEF(0.0)>]tolerance:float) : Curve ResizeArray =
        let curve0 = RhinoScriptSyntax.DirectCurveInput(curveA, -1)
        let curve1 = RhinoScriptSyntax.DirectCurveInput(curveB, -1)
        let tolerance = Util.ifZero2 State.Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanDifference(curve0, curve1, tolerance)

        let curves = ResizeArray<Curve>()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then curves.Add(curve)
        else RhinoScriptingException.Raise "CurveBooleanDifference: Unable to compute boolean result"
        curves

    /// <summary>Calculates the intersection of two closed, planar Curves and adds
    ///    the results to the document. Note, Curves must be coplanar.</summary>
    /// <param name="curveA">(Guid) Identifier of the first Curve object</param>
    /// <param name="curveB">(Guid) Identifier of the second Curve object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    /// <returns>(Guid ResizeArray) The identifiers of the new objects.</returns>
    static member CurveBooleanIntersection(curveA:Guid, curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let curves = RhinoScriptSyntax.CurveBooleanIntersection(RhinoScriptSyntax.CoerceCurve(curveA), RhinoScriptSyntax.CoerceCurve(curveB), tolerance)
        let ids = ResizeArray<Guid>()
        for curve in curves do
            let rc = State.Doc.Objects.AddCurve(curve)
            curve.Dispose()
            if rc = Guid.Empty then RhinoScriptingException.Raise "CurveBooleanIntersection: Unable to add curve to document"
            ids.Add(rc)
        State.Doc.Views.Redraw()
        ids

    /// <summary>Computes CurveBooleanIntersection from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveA">(Curve) The input geometry.</param>
    /// <param name="curveB">(Curve) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    /// <returns>(Curve ResizeArray) The resulting geometry; it is not added to a document.</returns>
    static member CurveBooleanIntersection(curveA:Curve, curveB:Curve, [<OPT;DEF(0.0)>]tolerance:float) : Curve ResizeArray =
        let curve0 = RhinoScriptSyntax.DirectCurveInput(curveA, -1)
        let curve1 = RhinoScriptSyntax.DirectCurveInput(curveB, -1)
        let tolerance = Util.ifZero2 State.Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanIntersection(curve0, curve1, tolerance)

        let curves = ResizeArray<Curve>()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then curves.Add(curve)
        else RhinoScriptingException.Raise "CurveBooleanIntersection: Unable to compute boolean result"
        curves

    /// <summary>Calculate the union of two or more closed, planar Curves and
    ///    add the results to the document. Note, Curves must be coplanar.</summary>
    /// <param name="curveIds">(Guid seq) List of two or more close planar Curves identifiers</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    /// <returns>(Guid ResizeArray) The identifiers of the new objects.</returns>
    static member CurveBooleanUnion(curveIds:Guid seq, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let curves = RhinoScriptSyntax.CurveBooleanUnion((curveIds |> Seq.map RhinoScriptSyntax.CoerceCurve), tolerance)
        let ids = ResizeArray<Guid>()
        for curve in curves do
            let rc = State.Doc.Objects.AddCurve(curve)
            curve.Dispose()
            if rc = Guid.Empty then RhinoScriptingException.Raise "CurveBooleanUnion: Unable to add curve to document"
            ids.Add(rc)
        State.Doc.Views.Redraw()
        ids

    /// <summary>Computes CurveBooleanUnion from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveInputs">(Curve seq) The input objects.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    A positive tolerance value, or None for the doc default</param>
    /// <returns>(Curve ResizeArray) The resulting geometry; it is not added to a document.</returns>
    static member CurveBooleanUnion(curveInputs:Curve seq, [<OPT;DEF(0.0)>]tolerance:float) : Curve ResizeArray =
        let inCurves = ResizeArray()
        for objectId in curveInputs do
            let curve = RhinoScriptSyntax.DirectCurveInput(objectId, -1)
            inCurves.Add(curve)

        if inCurves.Count < 2 then RhinoScriptingException.Raise "CurveBooleanUnion:curveInputs must have at least 2 curves %A" curveInputs
        let tolerance = Util.ifZero2 State.Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanUnion(inCurves, tolerance)

        let curves = ResizeArray<Curve>()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then curves.Add(curve)
        curves

    /// <summary>Intersects a Curve object with a brep object. Note, unlike the
    ///    CurveSurfaceIntersection function, this function works on trimmed Surfaces.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="brepId">(Guid) Identifier of a brep object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Distance tolerance at segment midpoints.</param>
    /// <returns>(Point3d ResizeArray * Curve ResizeArray) List of points and List of Curves.</returns>
    static member CurveBrepIntersect(curveId:Guid, brepId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Point3d ResizeArray * Curve ResizeArray=
        RhinoScriptSyntax.CurveBrepIntersect(RhinoScriptSyntax.CoerceCurve(curveId), RhinoScriptSyntax.CoerceBrep(brepId), tolerance)

    /// <summary>Intersects a Curve object with a brep object. Note, unlike the
    ///    CurveSurfaceIntersection function, this function works on trimmed Surfaces. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="brepInput">(Brep) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Distance tolerance at segment midpoints.</param>
    /// <returns>(Point3d ResizeArray * Curve ResizeArray) List of points and List of Curves.</returns>
    static member CurveBrepIntersect(curveGeo:Curve, brepInput:Brep, [<OPT;DEF(0.0)>]tolerance:float) : Point3d ResizeArray * Curve ResizeArray=
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let brep = (brepInput)
        let tolerance0 = Util.ifZero2 State.Doc.ModelAbsoluteTolerance tolerance
        let rc, outCurves, outPoints = Intersect.Intersection.CurveBrep(curve, brep, tolerance0)
        if not <| rc then  RhinoScriptingException.Raise "CurveBrepIntersect: Intersection failed. curveGeo:'%s' brepInput:'%s' tolerance:'%A'" (Pretty.str curveGeo) (Pretty.str brepInput) tolerance

        let curves = ResizeArray(0)
        let points = ResizeArray(0)
        for curve in outCurves do
            if notNull curve && curve.IsValid then
                curves.Add(curve)
                //let rc = State.Doc.Objects.AddCurve(curve)
                //curve.Dispose()
                //if rc = Guid.Empty then RhinoScriptingException.Raise "CurveBrepIntersect: Unable to add curve to document. curveGeo:'%s' brepInput:'%s' tolerance:'%A'" (Pretty.str curveGeo) brepInput tolerance
                //curves.Add(rc)
        for point in outPoints do
            if point.IsValid then
                points.Add(point)
                //let rc = State.Doc.Objects.AddPoint(point)
                //points.Add(rc)
        //State.Doc.Views.Redraw()
        points, curves //TODO or  Guid as originally done ??


    /// <summary>Returns the 3D point locations on two objects where they are closest to
    ///    each other. Note, this function provides similar functionality to that of
    ///    Rhino's ClosestPt command.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object to test</param>
    /// <param name="objectIds">(Guid seq) List of identifiers of point cloud, Curve, Surface, or
    ///    Polysurface to test against</param>
    /// <returns>(Guid * Point3d * Point3d) containing the results of the closest point calculation.
    ///    The elements are as follows:
    ///      [0]    The identifier of the closest object.
    ///      [1]    The 3-D point that is closest to the closest object.
    ///      [2]    The 3-D point that is closest to the test Curve.</returns>
    static member CurveClosestObject(curveId:Guid, objectIds:Guid seq) : Guid * Point3d * Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let objects = objectIds |> Seq.map (fun id -> id, RhinoScriptSyntax.CoerceGeometry(id)) |> Seq.toArray
        let geometry, objectPoint, curvePoint = RhinoScriptSyntax.CurveClosestObject(curve, objects |> Seq.map snd)
        let objectId = objects |> Array.find (fun (_, candidate) -> Object.ReferenceEquals(candidate, geometry)) |> fst
        objectId, objectPoint, curvePoint

    /// <summary>Computes CurveClosestObject from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="objectInputs">(GeometryBase seq) The input objects.</param>
    /// <returns>(GeometryBase * Point3d * Point3d) The resulting geometry; it is not added to a document.</returns>
    static member CurveClosestObject(curveGeo:Curve, objectInputs:GeometryBase seq) : GeometryBase * Point3d * Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let geometry = ResizeArray<GeometryBase>(objectInputs)
        if geometry.Count = 0 then RhinoScriptingException.Raise "CurveClosestObject: Objects must contain at least one item"
        let curvePoint = ref Point3d.Unset
        let geomPoint = ref Point3d.Unset
        let whichGeom = ref 0
        if curve.ClosestPoints(geometry, curvePoint, geomPoint, whichGeom) then
            geometry.[!whichGeom], !geomPoint, !curvePoint
        else RhinoScriptingException.Raise "CurveClosestObject: Closest point calculation failed"

    /// <summary>Returns the 3D point locations on the Curve and finite line where they are closest to
    ///    each other. Note, this function provides similar functionality to that of
    ///    Rhino's ClosestPt command.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object to test</param>
    /// <param name="line">(Line) a Line Geometry</param>
    /// <returns>(Point3d * Point3d) first point on Curve, second point on Line.</returns>
    static member CurveLineClosestPoint(curveId:Guid, line:Line) : Point3d * Point3d =
        RhinoScriptSyntax.CurveLineClosestPoint(RhinoScriptSyntax.CoerceCurve(curveId), line)

    /// <summary>Returns the 3D point locations on the Curve and finite line where they are closest to
    ///    each other. Note, this function provides similar functionality to that of
    ///    Rhino's ClosestPt command. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="line">(Line) a Line Geometry</param>
    /// <returns>(Point3d * Point3d) first point on Curve, second point on Line.</returns>
    static member CurveLineClosestPoint(curveGeo:Curve, line:Line) : Point3d * Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let curvePoint = ref Point3d.Unset
        let linePoint  = ref Point3d.Unset
        let success = curve.ClosestPoints(line.ToNurbsCurve(), curvePoint, linePoint)
        if success then  !linePoint, !curvePoint
        else RhinoScriptingException.Raise "CurveLineClosestPoint failed  curveGeo:'%s' Line:'%A'" (Pretty.str curveGeo) line


    /// <summary>Returns parameter of the point on a Curve that is closest to a test point.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="point">(Point3d) Sampling point</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    Curve segment index if `curveId` identifies a Polycurve</param>
    /// <returns>(float) The parameter of the closest point on the Curve.</returns>
    static member CurveClosestParameter(curveId:Guid, point:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        RhinoScriptSyntax.CurveClosestParameter(RhinoScriptSyntax.CoerceCurve(curveId), point, segmentIndex)

    /// <summary>Returns parameter of the point on a Curve that is closest to a test point. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="point">(Point3d) Sampling point</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    Curve segment index if `curveId` identifies a Polycurve</param>
    /// <returns>(float) The parameter of the closest point on the Curve.</returns>
    static member CurveClosestParameter(curveGeo:Curve, point:Point3d, segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let t = ref 0.
        let rc = curve.ClosestPoint(point, t)
        if not <| rc then RhinoScriptingException.Raise "CurveClosestParameter failed. curveGeo:'%s' segmentIndex:'%d'" (Pretty.str curveGeo) segmentIndex
        !t

    /// <summary>Returns parameter of the point on a Curve that is closest to a test point.</summary>
    /// <param name="curve">(Geometry.Curve) A Curve geometry object</param>
    /// <param name="point">(Point3d) Sampling point</param>
    /// <returns>(float) The parameter of the closest point on the Curve.</returns>
    static member CurveClosestParameter(curve:Curve, point:Point3d) : float =
        let t = ref 0.
        let rc = curve.ClosestPoint(point, t)
        if not <| rc then RhinoScriptingException.Raise "CurveClosestParameter failed on Curve Geometry"
        !t


    /// <summary>Returns the point on a Curve that is closest to a test point.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="point">(Point3d) Sampling point</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    Curve segment index if `curveId` identifies a Polycurve</param>
    /// <returns>(Point3d) The closest point on the Curve.</returns>
    static member CurveClosestPoint(curveId:Guid, point:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        RhinoScriptSyntax.CurveClosestPoint(RhinoScriptSyntax.CoerceCurve(curveId), point, segmentIndex)

    /// <summary>Returns the point on a Curve that is closest to a test point. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="point">(Point3d) Sampling point</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    Curve segment index if `curveId` identifies a Polycurve</param>
    /// <returns>(Point3d) The closest point on the Curve.</returns>
    static member CurveClosestPoint(curveGeo:Curve, point:Point3d, segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let rc, t = curve.ClosestPoint(point)
        if not <| rc then RhinoScriptingException.Raise "CurveClosestPoint failed. curveGeo:'%s' segmentIndex:'%d'" (Pretty.str curveGeo) segmentIndex
        curve.PointAt(t)

    /// <summary>Returns the point on a Curve that is closest to a test point.</summary>
    /// <param name="curve">(Geometry.Curve) A Curve geometry object</param>
    /// <param name="point">(Point3d) Sampling point</param>
    /// <returns>(Point3d) The closest point on the Curve.</returns>
    static member CurveClosestPoint(curve:Curve, point:Point3d) : Point3d =
        let rc, t = curve.ClosestPoint(point)
        if not <| rc then RhinoScriptingException.Raise "CurveClosestPoint failed on Curve Geometry"
        curve.PointAt(t)


    /// <summary>Returns the 3D point locations calculated by contouring a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="startPoint">(Point3d) 3D starting point of a center line</param>
    /// <param name="endPoint">(Point3d) 3D ending point of a center line</param>
    /// <param name="interval">(float) The distance between contour Curves</param>
    /// <returns>(Point3d array) A list of 3D points, one for each contour.</returns>
    static member CurveContourPoints(curveId:Guid, startPoint:Point3d, endPoint:Point3d, interval:float) : array<Point3d> =
        RhinoScriptSyntax.CurveContourPoints(RhinoScriptSyntax.CoerceCurve(curveId), startPoint, endPoint, interval)

    /// <summary>Returns the 3D point locations calculated by contouring a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="startPoint">(Point3d) 3D starting point of a center line</param>
    /// <param name="endPoint">(Point3d) 3D ending point of a center line</param>
    /// <param name="interval">(float) The distance between contour Curves</param>
    /// <returns>(Point3d array) A list of 3D points, one for each contour.</returns>
    static member CurveContourPoints(curveGeo:Curve, startPoint:Point3d, endPoint:Point3d, interval:float) : array<Point3d> =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        if startPoint.DistanceTo(endPoint)<RhinoMath.ZeroTolerance then
            RhinoScriptingException.Raise "CurveContourPoints: Start && ende point are too close to define a line. curveGeo:'%s' startPoint:'%A' endPoint:'%A'" (Pretty.str curveGeo) startPoint endPoint
        curve.DivideAsContour( startPoint, endPoint, interval)


    /// <summary>Returns the curvature of a Curve at a parameter. See the Rhino help for
    ///    details on Curve curvature.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve</param>
    /// <param name="parameter">(float) Parameter to evaluate</param>
    /// <returns>(Point3d * Vector3d * Point3d * float * Vector3d) of curvature information
    ///    [0] = point at specified parameter
    ///    [1] = tangent vector
    ///    [2] = center of radius of curvature
    ///    [3] = radius of curvature
    ///    [4] = curvature vector.</returns>
    static member CurveCurvature(curveId:Guid, parameter:float) : Point3d * Vector3d * Point3d * float * Vector3d =
        RhinoScriptSyntax.CurveCurvature(RhinoScriptSyntax.CoerceCurve(curveId), parameter)

    /// <summary>Returns the curvature of a Curve at a parameter. See the Rhino help for
    ///    details on Curve curvature. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="parameter">(float) Parameter to evaluate</param>
    /// <returns>(Point3d * Vector3d * Point3d * float * Vector3d) of curvature information
    ///    [0] = point at specified parameter
    ///    [1] = tangent vector
    ///    [2] = center of radius of curvature
    ///    [3] = radius of curvature
    ///    [4] = curvature vector.</returns>
    static member CurveCurvature(curveGeo:Curve, parameter:float) : Point3d * Vector3d * Point3d * float * Vector3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let point = curve.PointAt(parameter)
        let tangent = curve.TangentAt(parameter)
        if tangent.IsTiny(1e-10) then  RhinoScriptingException.Raise "CurveCurvature: failed on tangent that is too small %A" curveGeo
        let cv = curve.CurvatureAt(parameter)
        let k = cv.Length
        if k<RhinoMath.SqrtEpsilon then  RhinoScriptingException.Raise "CurveCurvature: failed on tangent that is too small %A" curveGeo
        let rv = cv / (k*k)
        let circle = Circle(point, tangent, point + 2.0*rv)
        let center = point + rv
        let radius = circle.Radius
        point, tangent, center, radius, cv


    /// <summary>Calculates intersection of two Curve objects.</summary>
    /// <param name="curveA">(Guid) Identifier of the first Curve object</param>
    /// <param name="curveB">(Guid) Optional, Identifier of the second Curve object. If omitted, then a
    ///    self-intersection test will be performed on CurveA</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Absolute tolerance in drawing units. If omitted,
    ///    the document's current absolute tolerance is used</param>
    /// <returns>( a ResizeArray of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float)
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
    static member CurveCurveIntersection(curveA:Guid, [<OPT;DEF(Guid())>]curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) ResizeArray =
        let curve1 = RhinoScriptSyntax.CoerceCurve(curveA)
        let curve2 = if curveB = Guid.Empty || curveB = curveA then null else RhinoScriptSyntax.CoerceCurve(curveB)
        RhinoScriptSyntax.CurveCurveIntersection(curve1, curve2, tolerance)

    /// <summary>Calculates intersection of two Curve objects. Uses the supplied geometry directly.</summary>
    /// <param name="curveA">(Curve) The input geometry.</param>
    /// <param name="curveB">(Curve) Optional, default value: <c>null</c>. The second curve.
    ///    If omitted, or if it is the same Curve object as curveA, then a self-intersection test will be performed on curveA</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Absolute tolerance in drawing units. If omitted,
    ///    the document's current absolute tolerance is used</param>
    /// <returns>( a ResizeArray of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float)
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
    static member CurveCurveIntersection(curveA:Curve, [<OPT;DEF(null:Curve)>]curveB:Curve, [<OPT;DEF(0.0)>]tolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) ResizeArray =
        let curve1 = RhinoScriptSyntax.DirectCurveInput(curveA, -1)
        let tolerance0 = Util.ifZero1 tolerance State.Doc.ModelAbsoluteTolerance
        let rc =
            if isNull curveB || obj.ReferenceEquals(curveA, curveB) then
                Intersect.Intersection.CurveSelf(curve1, tolerance0)
            else
                let curve2 = RhinoScriptSyntax.DirectCurveInput(curveB, -1)
                Intersect.Intersection.CurveCurve(curve1, curve2, tolerance0, State.Doc.ModelAbsoluteTolerance)
        if isNull rc then RhinoScriptingException.Raise "CurveCurveIntersection failed on %A; %A tolerance %f" curveB curveA tolerance
        let events = ResizeArray()
        for i =0 to rc.Count-1 do
            let mutable eventType = 1
            if( rc.[i].IsOverlap ) then  eventType <- 2
            let oa = rc.[i].OverlapA
            let ob = rc.[i].OverlapB
            let element = (eventType, rc.[i].PointA, rc.[i].PointA2, rc.[i].PointB, rc.[i].PointB2, oa.[0], oa.[1], ob.[0], ob.[1])
            events.Add(element)
        events



    /// <summary>Returns the degree of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(int) The degree of the Curve.</returns>
    static member CurveDegree(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        RhinoScriptSyntax.CurveDegree(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the degree of a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(int) The degree of the Curve.</returns>
    static member CurveDegree(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        curve.Degree


    /// <summary>Returns the minimum and maximum deviation between two Curve objects.</summary>
    /// <param name="curveA">(Guid) first Curve</param>
    /// <param name="curveB">(Guid) second Curve</param>
    /// <returns>(float * float * float * float * float * float) of deviation information
    ///    [0] = CurveA parameter at maximum overlap distance point
    ///    [1] = CurveB parameter at maximum overlap distance point
    ///    [2] = maximum overlap distance
    ///    [3] = CurveA parameter at minimum overlap distance point
    ///    [4] = CurveB parameter at minimum overlap distance point
    ///    [5] = minimum distance between Curves.</returns>
    static member CurveDeviation(curveA:Guid, curveB:Guid) : float * float * float * float * float * float =
        RhinoScriptSyntax.CurveDeviation(RhinoScriptSyntax.CoerceCurve(curveA), RhinoScriptSyntax.CoerceCurve(curveB))

    /// <summary>Returns the minimum and maximum deviation between two Curve objects. Uses the supplied geometry directly.</summary>
    /// <param name="curveA">(Curve) The input geometry.</param>
    /// <param name="curveB">(Curve) The input geometry.</param>
    /// <returns>(float * float * float * float * float * float) of deviation information
    ///    [0] = CurveA parameter at maximum overlap distance point
    ///    [1] = CurveB parameter at maximum overlap distance point
    ///    [2] = maximum overlap distance
    ///    [3] = CurveA parameter at minimum overlap distance point
    ///    [4] = CurveB parameter at minimum overlap distance point
    ///    [5] = minimum distance between Curves.</returns>
    static member CurveDeviation(curveA:Curve, curveB:Curve) : float * float * float * float * float * float =
        let curveA = RhinoScriptSyntax.DirectCurveInput(curveA, -1)
        let curveB = RhinoScriptSyntax.DirectCurveInput(curveB, -1)
        let tol = State.Doc.ModelAbsoluteTolerance
        let ok, maxa, maxb, maxd, mina, minb, mind = Curve.GetDistancesBetweenCurves(curveA, curveB, tol)
        if not ok then  RhinoScriptingException.Raise "CurveDeviation failed for %A; %A" curveB curveA
        else
            maxa, maxb, maxd, mina, minb, mind


    /// <summary>Returns the dimension of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    The Curve segment if CurveId identifies a PolyCurve</param>
    /// <returns>(int) The dimension of the Curve .</returns>
    static member CurveDim(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        RhinoScriptSyntax.CurveDim(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the dimension of a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    The Curve segment if CurveId identifies a PolyCurve</param>
    /// <returns>(int) The dimension of the Curve .</returns>
    static member CurveDim(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        curve.Dimension


    /// <summary>Tests if two Curve objects are generally in the same direction or if they
    ///    would be more in the same direction if one of them were flipped. When testing
    ///    Curve directions, both Curves must be either open or closed - you cannot test
    ///    one open Curve and one closed Curve.</summary>
    /// <param name="curveA">(Guid) Identifier of first Curve object</param>
    /// <param name="curveB">(Guid) Identifier of second Curve object</param>
    /// <returns>(bool) True if the Curve directions match, otherwise False.</returns>
    static member CurveDirectionsMatch(curveA:Guid, curveB:Guid) : bool =
        RhinoScriptSyntax.CurveDirectionsMatch(RhinoScriptSyntax.CoerceCurve(curveA), RhinoScriptSyntax.CoerceCurve(curveB))

    /// <summary>Tests if two Curve objects are generally in the same direction or if they
    ///    would be more in the same direction if one of them were flipped. When testing
    ///    Curve directions, both Curves must be either open or closed - you cannot test
    ///    one open Curve and one closed Curve. Uses the supplied geometry directly.</summary>
    /// <param name="curveA">(Curve) The input geometry.</param>
    /// <param name="curveB">(Curve) The input geometry.</param>
    /// <returns>(bool) True if the Curve directions match, otherwise False.</returns>
    static member CurveDirectionsMatch(curveA:Curve, curveB:Curve) : bool =
        let curve0 = RhinoScriptSyntax.DirectCurveInput(curveA, -1)
        let curve1 = RhinoScriptSyntax.DirectCurveInput(curveB, -1)
        Curve.DoDirectionsMatch(curve0, curve1)


    /// <summary>Search for a derivative, tangent, or curvature discontinuity in
    ///    a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of Curve object</param>
    /// <param name="style">(int) The type of continuity to test for. The types of
    ///    continuity are as follows:
    ///    Value    Description
    ///    1        C0 - Continuous function
    ///    2        C1 - Continuous first derivative
    ///    3        C2 - Continuous first and second derivative
    ///    4        G1 - Continuous unit tangent
    ///    5        G2 - Continuous unit tangent and curvature</param>
    /// <returns>(Point3d ResizeArray) 3D points where the Curve is discontinuous.</returns>
    static member CurveDiscontinuity(curveId:Guid, style:int) : Point3d ResizeArray =
        RhinoScriptSyntax.CurveDiscontinuity(RhinoScriptSyntax.CoerceCurve(curveId), style)

    /// <summary>Search for a derivative, tangent, or curvature discontinuity in
    ///    a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="style">(int) The type of continuity to test for. The types of
    ///    continuity are as follows:
    ///    Value    Description
    ///    1        C0 - Continuous function
    ///    2        C1 - Continuous first derivative
    ///    3        C2 - Continuous first and second derivative
    ///    4        G1 - Continuous unit tangent
    ///    5        G2 - Continuous unit tangent and curvature</param>
    /// <returns>(Point3d ResizeArray) 3D points where the Curve is discontinuous.</returns>
    static member CurveDiscontinuity(curveGeo:Curve, style:int) : Point3d ResizeArray =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
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


    /// <summary>Returns the domain of a Curve object
    ///    as an indexable object with two elements.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Interval) The domain of the Curve.</returns>
    static member CurveDomain(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Interval =
        RhinoScriptSyntax.CurveDomain(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the domain of a Curve object
    ///    as an indexable object with two elements. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Interval) The domain of the Curve.</returns>
    static member CurveDomain(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Interval =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        curve.Domain


    /// <summary>Returns the Edit-, or Greville-Points of a Curve object.
    ///    For each Curve control point, there is a corresponding edit point.
    /// see rs.CurveEditParameters for parameters, not points</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index is `curveId` identifies a PolyCurve</param>
    /// <returns>(Collections.Point3dList) Curve edit points.</returns>
    static member CurveEditPoints(curveId:Guid,[<OPT;DEF(-1)>]segmentIndex:int) : Collections.Point3dList =
        RhinoScriptSyntax.CurveEditPoints(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the Edit-, or Greville-Points of a Curve object.
    ///    For each Curve control point, there is a corresponding edit point.
    /// see rs.CurveEditParameters for parameters, not points Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index is `curveId` identifies a PolyCurve</param>
    /// <returns>(Collections.Point3dList) Curve edit points.</returns>
    static member CurveEditPoints(curveGeo:Curve,[<OPT;DEF(-1)>]segmentIndex:int) : Collections.Point3dList =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let nc = curve.ToNurbsCurve()
        if isNull nc then  RhinoScriptingException.Raise "CurveEditPoints failed for %A" curveGeo
        nc.GrevillePoints()


    /// <summary>Returns the Edit-, or Greville-Parameters of a Curve object.
    ///    For each Curve control point, there is a corresponding edit point parameter.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index is `curveId` identifies a PolyCurve</param>
    /// <returns>(Collections.Point3dList)  A list of Curve parameters</returns>
    static member CurveEditParameters(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float[] =
        RhinoScriptSyntax.CurveEditParameters(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the Edit-, or Greville-Parameters of a Curve object.
    ///    For each Curve control point, there is a corresponding edit point parameter. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index is `curveId` identifies a PolyCurve</param>
    /// <returns>(Collections.Point3dList)  A list of Curve parameters</returns>
    static member CurveEditParameters(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : float[] =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let nc = curve.ToNurbsCurve()
        if isNull nc then  RhinoScriptingException.Raise "CurveEditParameters failed for %A" curveGeo
        nc.GrevilleParameters()

    /// <summary>Returns the end point of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d) The 3d endpoint of the Curve.</returns>
    static member CurveEndPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        RhinoScriptSyntax.CurveEndPoint(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the end point of a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d) The 3d endpoint of the Curve.</returns>
    static member CurveEndPoint(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        curve.PointAtEnd

    /// <summary>Returns the tangent at end point of a Curve object
    /// pointing away from the Curve .</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Vector3d) The tangent, same as Curve.TangentAtEnd property .</returns>
    static member CurveEndTangent(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        RhinoScriptSyntax.CurveEndTangent(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the tangent at end point of a Curve object
    /// pointing away from the Curve . Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Vector3d) The tangent, same as Curve.TangentAtEnd property .</returns>
    static member CurveEndTangent(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        curve.TangentAtEnd

    /// <summary>Returns the tangent at start point of a Curve object
    /// pointing in direction of  the Curve .</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Vector3d) The tangent, same as Curve.TangentAtStart property .</returns>
    static member CurveStartTangent(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        RhinoScriptSyntax.CurveStartTangent(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the tangent at start point of a Curve object
    /// pointing in direction of  the Curve . Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Vector3d) The tangent, same as Curve.TangentAtStart property .</returns>
    static member CurveStartTangent(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        curve.TangentAtStart

    /// <summary>Find points at which to cut a pair of Curves so that a fillet of a
    ///    specified radius fits. A fillet point is a pair of points (point0, point1)
    ///    such that there is a circle of radius tangent to Curve Curve0 at point0 and
    ///    tangent to Curve Curve1 at point1. Of all possible fillet points, this
    ///    function returns the one which is the closest to the base point basePointA,
    ///    basePointB. Distance from the base point is measured by the sum of arc
    ///    lengths along the two Curves.</summary>
    /// <param name="curveA">(Guid) Identifier of the first Curve object</param>
    /// <param name="curveB">(Guid) Identifier of the second Curve object</param>
    /// <param name="radius">(float) The fillet radius</param>
    /// <param name="basePointA">(Point3d) Optional. The base point on the first Curve.
    ///    If omitted, the starting point of the Curve is used</param>
    /// <param name="basePointB">(Point3d) Optional. The base point on the second Curve. If omitted,
    ///    the starting point of the Curve is used</param>
    /// <returns>(Point3d * Point3d * Plane)
    ///    . The list elements are as follows:
    ///      [0]    A point on the first Curve at which to cut (point).
    ///      [1]    A point on the second Curve at which to cut (point).
    ///      [2]    The fillet Plane.</returns>
    static member CurveFilletPoints(curveA:Guid,
                                    curveB:Guid,
                                    radius:float,
                                    [<OPT;DEF(Point3d())>]basePointA:Point3d,
                                    [<OPT;DEF(Point3d())>]basePointB:Point3d) : Point3d * Point3d * Plane = //TODO make overload instead, this may leak  see draw vector and transform point!
        RhinoScriptSyntax.CurveFilletPoints(RhinoScriptSyntax.CoerceCurve(curveA), RhinoScriptSyntax.CoerceCurve(curveB), radius, basePointA, basePointB)

    /// <summary>Find points at which to cut a pair of Curves so that a fillet of a
    ///    specified radius fits. A fillet point is a pair of points (point0, point1)
    ///    such that there is a circle of radius tangent to Curve Curve0 at point0 and
    ///    tangent to Curve Curve1 at point1. Of all possible fillet points, this
    ///    function returns the one which is the closest to the base point basePointA,
    ///    basePointB. Distance from the base point is measured by the sum of arc
    ///    lengths along the two Curves. Uses the supplied geometry directly.</summary>
    /// <param name="curveA">(Curve) The input geometry.</param>
    /// <param name="curveB">(Curve) The input geometry.</param>
    /// <param name="radius">(float) The fillet radius</param>
    /// <param name="basePointA">(Point3d) Optional. The base point on the first Curve.
    ///    If omitted, the starting point of the Curve is used</param>
    /// <param name="basePointB">(Point3d) Optional. The base point on the second Curve. If omitted,
    ///    the starting point of the Curve is used</param>
    /// <returns>(Point3d * Point3d * Plane)
    ///    . The list elements are as follows:
    ///      [0]    A point on the first Curve at which to cut (point).
    ///      [1]    A point on the second Curve at which to cut (point).
    ///      [2]    The fillet Plane.</returns>
    static member CurveFilletPoints(curveA:Curve,
                                    curveB:Curve,
                                    radius:float,
                                    [<OPT;DEF(Point3d())>]basePointA:Point3d,
                                    [<OPT;DEF(Point3d())>]basePointB:Point3d) : Point3d * Point3d * Plane = //TODO make overload instead, this may leak  see draw vector and transform point!
        //  [<OPT;DEF(true)>]returnPoints:bool)
        //<param name="returnPoints">(bool) Optional, default value: <c>true</c>
        //If True (Default), then fillet points are
        //  returned. Otherwise, a fillet curve is created and                       // TODO not Implemented
        //  it's identifier is returned</param>
        let curve0 = RhinoScriptSyntax.DirectCurveInput(curveA, -1)
        let curve1 = RhinoScriptSyntax.DirectCurveInput(curveB, -1)
        let basePointA = if basePointA = Point3d.Origin then Point3d.Unset else basePointA
        let basePointB = if basePointB = Point3d.Origin then Point3d.Unset else basePointB

        let inline distance  (a:Point3d)(b:Point3d) = (a-b).Length

        let t0Base =
            if basePointA <> Point3d.Unset then
                let ok, t = curve0.ClosestPoint(basePointA)
                if not ok then RhinoScriptingException.Raise "CurveFilletPoints failed 1 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
                t
            else
                let distEnde  = min  (distance curve1.PointAtStart curve0.PointAtEnd)   (distance curve1.PointAtEnd curve0.PointAtEnd)
                let distStart = min  (distance curve1.PointAtStart curve0.PointAtStart) (distance curve1.PointAtEnd curve0.PointAtStart)
                if distStart < distEnde then curve0.Domain.Min else curve0.Domain.Max

        let t1Base =
            if basePointB <> Point3d.Unset then
                let ok, t = curve1.ClosestPoint(basePointB)
                if not ok then RhinoScriptingException.Raise "CurveFilletPoints failed 2 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
                t
            else
                let distEnde  = min  (distance curve0.PointAtStart curve1.PointAtEnd)   (distance curve0.PointAtEnd curve1.PointAtEnd)
                let distStart = min  (distance curve0.PointAtStart curve1.PointAtStart) (distance curve0.PointAtEnd curve1.PointAtStart)
                if distStart < distEnde then curve1.Domain.Min else curve1.Domain.Max

        let ok, a, b, pl = Curve.GetFilletPoints(curve0, curve1, radius, t0Base, t1Base)
        if not ok then RhinoScriptingException.Raise "CurveFilletPoints failed 3 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
        curve0.PointAt(a), curve0.PointAt(b), pl


    /// <summary>Returns the Plane at a parameter of a Curve. The Plane is based on the
    ///    tangent and curvature vectors at a parameter.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="parameter">(float) Parameter to evaluate</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Plane) The Plane at the specified parameter.</returns>
    static member CurveFrame(curveId:Guid, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        RhinoScriptSyntax.CurveFrame(RhinoScriptSyntax.CoerceCurve(curveId), parameter, segmentIndex)

    /// <summary>Returns the Plane at a parameter of a Curve. The Plane is based on the
    ///    tangent and curvature vectors at a parameter. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="parameter">(float) Parameter to evaluate</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Plane) The Plane at the specified parameter.</returns>
    static member CurveFrame(curveGeo:Curve, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        let mutable para = parameter
        let  curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let  domain = curve.Domain
        if not <| domain.IncludesParameter(parameter) then
            let  tol = State.Doc.ModelAbsoluteTolerance
            if parameter>domain.Max && (para-domain.Max)<=tol then
                para <- domain.Max
            elif parameter<domain.Min && (domain.Min-para)<=tol then
                para <- domain.Min
            else
                RhinoScriptingException.Raise "CurveFrame failed. curveGeo:'%s' parameter:'%A' segmentIndex:'%A'" (Pretty.str curveGeo) parameter segmentIndex
        let  rc, frame = curve.FrameAt(para)
        if rc && frame.IsValid then  frame
        else RhinoScriptingException.Raise "CurveFrame failed. curveGeo:'%s' parameter:'%A' segmentIndex:'%A'" (Pretty.str curveGeo) parameter segmentIndex


    /// <summary>Returns the knot count of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment if `curveId` identifies a PolyCurve</param>
    /// <returns>(int) The number of knots.</returns>
    static member CurveKnotCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        RhinoScriptSyntax.CurveKnotCount(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the knot count of a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment if `curveId` identifies a PolyCurve</param>
    /// <returns>(int) The number of knots.</returns>
    static member CurveKnotCount(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let  curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let  nc = curve.ToNurbsCurve()
        if isNull nc then  RhinoScriptingException.Raise "CurveKnotCount failed. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        nc.Knots.Count


    /// <summary>Returns the knots, or knot vector, of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(float ResizeArray) knot values.</returns>
    static member CurveKnots(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : ResizeArray<float> =
        RhinoScriptSyntax.CurveKnots(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the knots, or knot vector, of a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(float ResizeArray) knot values.</returns>
    static member CurveKnots(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : ResizeArray<float> =
        let  curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let  nc = curve.ToNurbsCurve()
        if isNull nc then  RhinoScriptingException.Raise "CurveKnots failed. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        let knots = ResizeArray(nc.Knots.Count)
        for i = 0 to nc.Knots.Count - 1 do
            knots.Add(nc.Knots.[i])
        knots

    /// <summary>Returns the length of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <param name="subDomain">(Interval) Optional, List of two numbers identifying the sub-domain of the
    ///    Curve on which the calculation will be performed. The two parameters
    ///    (sub-domain) must be non-decreasing. If omitted, the length of the
    ///    entire Curve is returned</param>
    /// <returns>(float) The length of the Curve.</returns>
    static member CurveLength(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int, [<OPT;DEF(Interval())>]subDomain:Interval) : float =
        RhinoScriptSyntax.CurveLength(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex, subDomain)

    /// <summary>Returns the length of a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <param name="subDomain">(Interval) Optional, List of two numbers identifying the sub-domain of the
    ///    Curve on which the calculation will be performed. The two parameters
    ///    (sub-domain) must be non-decreasing. If omitted, the length of the
    ///    entire Curve is returned</param>
    /// <returns>(float) The length of the Curve.</returns>
    static member CurveLength(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int, [<OPT;DEF(Interval())>]subDomain:Interval) : float =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        if subDomain.T0 = 0.0 && subDomain.T1 = 0.0 then curve.GetLength()
        else curve.GetLength(subDomain)


    /// <summary>Returns the average unitized direction of a Curve object between start and end point,
    ///    Optionally allows non linear Curves too.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="allowNonLinear">(bool) Optional, allow non linear Curves, Default Value <c>False</c> </param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Vector3d) The direction of the Curve.</returns>
    static member CurveDirection(curveId:Guid, [<OPT;DEF(false)>]allowNonLinear:bool,[<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        RhinoScriptSyntax.CurveDirection(RhinoScriptSyntax.CoerceCurve(curveId), allowNonLinear, segmentIndex)

    /// <summary>Returns the average unitized direction of a Curve object between start and end point,
    ///    Optionally allows non linear Curves too. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="allowNonLinear">(bool) Optional, allow non linear Curves, Default Value <c>False</c> </param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Vector3d) The direction of the Curve.</returns>
    static member CurveDirection(curveGeo:Curve, [<OPT;DEF(false)>]allowNonLinear:bool,[<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let  curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        if allowNonLinear || curve.IsLinear(RhinoMath.ZeroTolerance) then
            if curve.IsClosed || curve.IsClosable(State.Doc.ModelAbsoluteTolerance) then RhinoScriptingException.Raise "CurveDirection failed on closed or closable curve. curveGeo:'%s' allowNonLinear '%A' segmentIndex:'%A'" (Pretty.str curveGeo) allowNonLinear segmentIndex
            let v = curve.PointAtEnd - curve.PointAtStart
            if v.Unitize() then v
            else
                RhinoScriptingException.Raise "CurveDirection failed. start and end are the same point. curveGeo:'%s' allowNonLinear '%A' segmentIndex:'%A'" (Pretty.str curveGeo) allowNonLinear segmentIndex

        else
            RhinoScriptingException.Raise "CurveDirection failed. curveGeo:'%s' allowNonLinear '%A' segmentIndex:'%A'" (Pretty.str curveGeo) allowNonLinear segmentIndex


    /// <summary>Returns the mid point of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d) The 3D midpoint of the Curve.</returns>
    static member CurveMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        RhinoScriptSyntax.CurveMidPoint(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the mid point of a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d) The 3D midpoint of the Curve.</returns>
    static member CurveMidPoint(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let  curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let  rc, t = curve.NormalizedLengthParameter(0.5)
        if rc then  curve.PointAt(t)
        else RhinoScriptingException.Raise "CurveMidPoint failed. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex


    /// <summary>Returns the normal direction of the Plane in which a planar Curve object lies.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment if CurveId identifies a PolyCurve</param>
    /// <returns>(Vector3d) The 3D normal vector.</returns>
    static member CurveNormal(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        RhinoScriptSyntax.CurveNormal(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the normal direction of the Plane in which a planar Curve object lies. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment if CurveId identifies a PolyCurve</param>
    /// <returns>(Vector3d) The 3D normal vector.</returns>
    static member CurveNormal(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let tol = State.Doc.ModelAbsoluteTolerance
        let plane = ref Plane.WorldXY
        let rc = curve.TryGetPlane(plane, tol)
        if rc then  (!plane).Normal
        else RhinoScriptingException.Raise "CurveNormal failed. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex


    /// <summary>Converts a Curve parameter to a normalized Curve parameter;
    ///    one that ranges between 0-1.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="parameter">(float) The Curve parameter to convert</param>
    /// <returns>(float) normalized Curve parameter.</returns>
    static member CurveNormalizedParameter(curveId:Guid, parameter:float) : float =
        RhinoScriptSyntax.CurveNormalizedParameter(RhinoScriptSyntax.CoerceCurve(curveId), parameter)

    /// <summary>Converts a Curve parameter to a normalized Curve parameter;
    ///    one that ranges between 0-1. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="parameter">(float) The Curve parameter to convert</param>
    /// <returns>(float) normalized Curve parameter.</returns>
    static member CurveNormalizedParameter(curveGeo:Curve, parameter:float) : float =
        let  curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        curve.Domain.NormalizedParameterAt(parameter)


    /// <summary>Converts a normalized Curve parameter to a Curve parameter;
    ///    one within the Curve's domain.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="parameter">(float) The normalized Curve parameter to convert</param>
    /// <returns>(float) Curve parameter.</returns>
    static member CurveParameter(curveId:Guid, parameter:float) : float =
        RhinoScriptSyntax.CurveParameter(RhinoScriptSyntax.CoerceCurve(curveId), parameter)

    /// <summary>Converts a normalized Curve parameter to a Curve parameter;
    ///    one within the Curve's domain. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="parameter">(float) The normalized Curve parameter to convert</param>
    /// <returns>(float) Curve parameter.</returns>
    static member CurveParameter(curveGeo:Curve, parameter:float) : float =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        curve.Domain.ParameterAt(parameter)


    /// <summary>Returns the perpendicular Plane at a parameter of a Curve. The result
    ///    is relatively parallel (zero-twisting) Plane.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="parameter">(float) Parameter to evaluate</param>
    /// <returns>(Plane) Plane.</returns>
    static member CurvePerpFrame(curveId:Guid, parameter:float) : Plane =
        RhinoScriptSyntax.CurvePerpFrame(RhinoScriptSyntax.CoerceCurve(curveId), parameter)

    /// <summary>Returns the perpendicular Plane at a parameter of a Curve. The result
    ///    is relatively parallel (zero-twisting) Plane. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="parameter">(float) Parameter to evaluate</param>
    /// <returns>(Plane) Plane.</returns>
    static member CurvePerpFrame(curveGeo:Curve, parameter:float) : Plane =
        let  curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let  rc, plane = curve.PerpendicularFrameAt(parameter)
        if rc then  plane else RhinoScriptingException.Raise "CurvePerpFrame failed. curveGeo:'%s' parameter:'%f'"  (Pretty.str curveGeo) parameter


    /// <summary>Returns the Plane in which a planar Curve lies. Note, this function works
    ///    only on planar Curves.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Plane) The Plane in which the Curve lies.</returns>
    static member CurvePlane(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        RhinoScriptSyntax.CurvePlane(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the Plane in which a planar Curve lies. Note, this function works
    ///    only on planar Curves. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Plane) The Plane in which the Curve lies.</returns>
    static member CurvePlane(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let tol = State.Doc.ModelAbsoluteTolerance
        let plane = ref Plane.WorldXY
        let rc = curve.TryGetPlane(plane, tol)
        if rc then  !plane
        else RhinoScriptingException.Raise "CurvePlane failed. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex


    /// <summary>Returns the control points count of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment if `curveId` identifies a PolyCurve</param>
    /// <returns>(int) Number of control points.</returns>
    static member CurvePointCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        RhinoScriptSyntax.CurvePointCount(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the control points count of a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment if `curveId` identifies a PolyCurve</param>
    /// <returns>(int) Number of control points.</returns>
    static member CurvePointCount(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let mutable nc = curve.ToNurbsCurve()
        if notNull nc then  nc.Points.Count
        else RhinoScriptingException.Raise "CurvePointCount failed. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex

    /// <summary>Returns the control points, or control vertices, of a Curve object.
    ///    If the Curve is a rational NURBS Curve, the euclidean control vertices
    ///    are returned.</summary>
    /// <param name="curve">(Curve) The Curve Geometry</param>
    /// <returns>(Point3d ResizeArray) The control points, or control vertices, of a Curve object.</returns>
    static member CurvePoints(curve:Curve) : Point3d ResizeArray =
        match curve with
        | :? PolylineCurve as pl ->
            let points = ResizeArray(pl.PointCount)
            for i = 0 to pl.PointCount - 1 do points.Add(pl.Point(i))
            points
        | :? NurbsCurve as nc ->
            let points = ResizeArray(nc.Points.Count)
            for i = 0 to nc.Points.Count-1 do points.Add(nc.Points.[i].Location)
            points
        | _ ->
            let nc = curve.ToNurbsCurve()
            if isNull nc then  RhinoScriptingException.Raise "CurvePoints failed. curve:'%A'" curve
            let points = ResizeArray(nc.Points.Count)
            for i = 0 to nc.Points.Count-1 do points.Add(nc.Points.[i].Location)
            points

    /// <summary>Returns the control points, or control vertices, of a Curve object.
    ///    If the Curve is a rational NURBS Curve, the euclidean control vertices
    ///    are returned.</summary>
    /// <param name="curveId">(Guid) The object's identifier</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d ResizeArray) The control points, or control vertices, of a Curve object.</returns>
    static member CurvePoints(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d ResizeArray =
        RhinoScriptSyntax.CurvePoints(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the control points, or control vertices, of a Curve object.
    ///    If the Curve is a rational NURBS Curve, the euclidean control vertices
    ///    are returned. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) The Curve segment if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d ResizeArray) The control points, or control vertices, of a Curve object.</returns>
    static member CurvePoints(curveGeo:Curve, segmentIndex:int) : Point3d ResizeArray =
        RhinoScriptSyntax.CurvePoints(RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex))

    /// <summary>Returns the radius of curvature at a point on a Curve.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="testPoint">(Point3d) Sampling point</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment if CurveId identifies a PolyCurve</param>
    /// <returns>(float) The radius of curvature at the point on the Curve.</returns>
    static member CurveRadius(curveId:Guid, testPoint:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        RhinoScriptSyntax.CurveRadius(RhinoScriptSyntax.CoerceCurve(curveId), testPoint, segmentIndex)

    /// <summary>Returns the radius of curvature at a point on a Curve. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="testPoint">(Point3d) Sampling point</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment if CurveId identifies a PolyCurve</param>
    /// <returns>(float) The radius of curvature at the point on the Curve.</returns>
    static member CurveRadius(curveGeo:Curve, testPoint:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let mutable rc, t = curve.ClosestPoint(testPoint)//, 0.0)
        if not <| rc then  RhinoScriptingException.Raise "CurveRadius failed. curveGeo:'%s' testPoint:'%A' segmentIndex:'%A'" (Pretty.str curveGeo) testPoint segmentIndex
        let mutable v = curve.CurvatureAt( t )
        let mutable k = v.Length
        if k>RhinoMath.ZeroTolerance then  1.0/k
        else RhinoScriptingException.Raise "CurveRadius failed. curveGeo:'%s' testPoint:'%A' segmentIndex:'%A'" (Pretty.str curveGeo) testPoint segmentIndex


    /// <summary>Adjusts the seam, or start/end, point of a closed Curve.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="parameter">(float) The parameter of the new start/end point.
    ///    Note, if successful, the resulting Curve's
    ///    domain will start at `parameter`</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
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
                    State.Doc.Objects.Replace(curveId, dupe)
            else
                false


    /// <summary>Returns the start point of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d) The 3D starting point of the Curve.</returns>
    static member CurveStartPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        RhinoScriptSyntax.CurveStartPoint(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns the start point of a Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d) The 3D starting point of the Curve.</returns>
    static member CurveStartPoint(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        curve.PointAtStart

    /// <summary>Sets the start point of a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="point">(Point3d) New start point</param>
    /// <returns>(unit).</returns>
    static member CurveStartPoint(curveId:Guid, point:Point3d) : unit =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if not <|curve.SetStartPoint(point) then RhinoScriptingException.Raise "CurveStartPoint failed on '%A' and '%A'" point curveId
        State.Doc.Objects.Replace(curveId, curve) |> ignore<bool>
        State.Doc.Views.Redraw()



    /// <summary>Calculates intersection of a Curve object with a Surface object.
    ///    Note, this function works on the untrimmed portion of the Surface.</summary>
    /// <param name="curveId">(Guid) The identifier of the first Curve object</param>
    /// <param name="surfaceId">(Guid) The identifier of the second Curve object. If omitted,
    ///    the a self-intersection test will be performed on Curve</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The absolute tolerance in drawing units.</param>
    /// <param name="angleTolerance">(float) Optional, default value: <c>State.Doc.ModelAngleToleranceRadians</c>
    ///    Angle tolerance in degrees. The angle
    ///    tolerance is used to determine when the Curve is tangent to the
    ///    Surface.</param>
    /// <returns>(ResizeArray of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) of intersection information .
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
    static member CurveSurfaceIntersection(curveId:Guid, surfaceId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(0.0)>]angleTolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float*float*float) ResizeArray =
        RhinoScriptSyntax.CurveSurfaceIntersection(RhinoScriptSyntax.CoerceCurve(curveId), RhinoScriptSyntax.CoerceSurface(surfaceId), tolerance, angleTolerance)

    /// <summary>Calculates intersection of a Curve object with a Surface object.
    ///    Note, this function works on the untrimmed portion of the Surface. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="surfaceGeo">(Surface) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The absolute tolerance in drawing units.</param>
    /// <param name="angleTolerance">(float) Optional, default value: <c>State.Doc.ModelAngleToleranceRadians</c>
    ///    Angle tolerance in degrees. The angle
    ///    tolerance is used to determine when the Curve is tangent to the
    ///    Surface.</param>
    /// <returns>(ResizeArray of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) of intersection information .
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
    static member CurveSurfaceIntersection(curveGeo:Curve, surfaceGeo:Surface, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(0.0)>]angleTolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float*float*float) ResizeArray =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let surface = (surfaceGeo)
        let tolerance0 = Util.ifZero1 tolerance State.Doc.ModelAbsoluteTolerance
        let angleTolerance0 = Util.ifZero1 (toRadians(angleTolerance)) State.Doc.ModelAngleToleranceRadians
        let  rc = Intersect.Intersection.CurveSurface(curve, surface, tolerance0, angleTolerance0)
        if isNull rc then RhinoScriptingException.Raise "CurveSurfaceIntersection failed. (surfaceGeo:%A) (curveGeo:%A) (angleTolerance:%f) (tolerance:%f) " surfaceGeo curveGeo angleTolerance tolerance
        let events = ResizeArray()
        for i = 0 to rc.Count - 1 do
            let eventType = if rc.[i].IsOverlap then 2 else 1
            let item = rc.[i]
            let oa = item.OverlapA
            let u, v = item.SurfaceOverlapParameter()
            let e = eventType, item.PointA, item.PointA2, item.PointB, item.PointB2, oa.[0], oa.[1], u.[0], u.[1], v.[0], v.[1]
            events.Add(e)
        events

    /// <summary>Returns a 3D vector that is the tangent to a Curve at a parameter.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="parameter">(float) Parameter to evaluate</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Vector3d) A 3D vector.</returns>
    static member CurveTangent(curveId:Guid, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        RhinoScriptSyntax.CurveTangent(RhinoScriptSyntax.CoerceCurve(curveId), parameter, segmentIndex)

    /// <summary>Returns a 3D vector that is the tangent to a Curve at a parameter. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="parameter">(float) Parameter to evaluate</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Vector3d) A 3D vector.</returns>
    static member CurveTangent(curveGeo:Curve, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        if curve.Domain.IncludesParameter(parameter) then
            curve.TangentAt(parameter)
        else
            RhinoScriptingException.Raise "CurveTangent failed. curveGeo:'%s' parameter:'%A' segmentIndex:'%A'" (Pretty.str curveGeo) parameter segmentIndex


    /// <summary>Returns list of weights that are assigned to the control points of a Curve.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(float ResizeArray) The weight values of the Curve.</returns>
    static member CurveWeights(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) :  float ResizeArray =
        RhinoScriptSyntax.CurveWeights(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    /// <summary>Returns list of weights that are assigned to the control points of a Curve. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(float ResizeArray) The weight values of the Curve.</returns>
    static member CurveWeights(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) :  float ResizeArray =
        let nc =
            match RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex) with
            | :? NurbsCurve as nc -> nc
            | c -> c.ToNurbsCurve()
        if isNull nc then  RhinoScriptingException.Raise "CurveWeights failed. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex
        let weights = ResizeArray(nc.Points.Count)
        for i = 0 to nc.Points.Count-1 do weights.Add(nc.Points.[i].Weight)
        weights

    /// <summary>Divides a Curve Geometry into a specified number of segments, including start and end point.</summary>
    /// <param name="curve">(Geometry.Curve) Curve geometry</param>
    /// <param name="segments">(int) The number of segments</param>
    /// <returns>(Point3d array) Array containing points at divisions.</returns>
    static member DivideCurveIntoPoints(curve:Curve, segments:int) : Point3d array =
        let pts = ref (Array.zeroCreate (segments + 1))
        let rc = curve.DivideByCount(segments, includeEnds=true, points=pts)
        if isNull rc then  RhinoScriptingException.Raise "DivideCurveIntoPoints failed. curve:'%A' segments:'%A'" curve segments
        !pts

    /// <summary>Divides a Curve object into a specified number of segments, including start and end point.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segments">(int) The number of segments</param>
    /// <returns>(Point3d array) Array containing points at divisions.</returns>
    static member DivideCurveIntoPoints(curveId:Guid, segments:int) : Point3d array =
        RhinoScriptSyntax.DivideCurveIntoPoints(RhinoScriptSyntax.CoerceCurve(curveId), segments)

    /// <summary>Divides a Curve Geometry into a specified number of segments.</summary>
    /// <param name="curve">(Geometry.Curve) Curve geometry</param>
    /// <param name="segments">(int) The number of segments</param>
    /// <returns>( float array ) array containing 3D division parameters.</returns>
    static member DivideCurve(curve:Curve, segments:int) :  float array =
        let rc = curve.DivideByCount(segments, includeEnds=true)
        if isNull rc then  RhinoScriptingException.Raise "DivideCurve failed. curve:'%A' segments:'%A'" curve segments
        rc

    /// <summary>Divides a Curve object into a specified number of segments.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segments">(int) The number of segments</param>
    /// <returns>( float array ) array containing 3D division parameters.</returns>
    static member DivideCurve(curveId:Guid, segments:int) :  float array =
        RhinoScriptSyntax.DivideCurve(RhinoScriptSyntax.CoerceCurve(curveId), segments)

    /// <summary>Divides a Curve Geometry such that the linear distance between the points is equal.</summary>
    /// <param name="curve">(Geometry.Curve) Curve geometry</param>
    /// <param name="distance">(float) Linear distance between division points</param>
    /// <returns>(Point3d array) array containing 3D division points.</returns>
    static member DivideCurveEquidistant(curve:Curve, distance:float) : array<Point3d> =
        let  points = curve.DivideEquidistant(distance)
        if isNull points then
            let len = curve.GetLength()
            if len < distance then
                RhinoScriptingException.Raise "DivideCurveEquidistant failed on too short curve. curve:'%A' distance:%f, curveLength=%f" curve distance len
            else
                RhinoScriptingException.Raise "DivideCurveEquidistant failed. curve:'%A' distance:%f, curveLength=%f" curve distance len
        points


    /// <summary>Divides a Curve object such that the linear distance between the points is equal.</summary>
    /// <param name="curveId">(Guid) The object's identifier</param>
    /// <param name="distance">(float) Linear distance between division points</param>
    /// <returns>(Point3d array) array containing 3D division points.</returns>
    static member DivideCurveEquidistant(curveId:Guid, distance:float) : array<Point3d> =
        RhinoScriptSyntax.DivideCurveEquidistant(RhinoScriptSyntax.CoerceCurve(curveId), distance)

    /// <summary>Divides a Curve Geometry into segments of a specified length.
    /// If length is more than Curve length it fails.</summary>
    /// <param name="curve">(Geometry.Curve) Curve geometry</param>
    /// <param name="length">(float) The length of each segment</param>
    /// <returns>(Point3d ResizeArray) a list containing division points.</returns>
    static member DivideCurveLengthIntoPoints(curve:Curve, length:float) : Point3d ResizeArray =
        let rc = curve.DivideByLength(length, includeEnds=true)
        if isNull rc then
            let len = curve.GetLength()
            if len < length then
                RhinoScriptingException.Raise "DivideCurveLengthIntoPoints failed on too short curve. curve:'%A' dived-length:%f, curveLength=%f" curve length len
            else
                RhinoScriptingException.Raise "DivideCurveLengthIntoPoints failed. curve:'%A' dived-length:%f, curveLength=%f" curve length len
        let pts = ResizeArray()
        for r in rc do pts.Add(curve.PointAt(r))
        pts

    /// <summary>Divides a Curve object into segments of a specified length.
    /// If length is more than Curve length it fails.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="length">(float) The length of each segment</param>
    /// <returns>(Point3d ResizeArray) a list containing division points.</returns>
    static member DivideCurveLengthIntoPoints(curveId:Guid, length:float) : Point3d ResizeArray =
        RhinoScriptSyntax.DivideCurveLengthIntoPoints(RhinoScriptSyntax.CoerceCurve(curveId), length)

    /// <summary>Divides a Curve Geometry into segments of a specified length.
    /// If length is more than Curve length it fails.</summary>
    /// <param name="curve">(Geometry.Curve) Curve geometry</param>
    /// <param name="length">(float) The length of each segment</param>
    /// <returns>( float array) a list containing 3D division parameters.</returns>
    static member DivideCurveLength(curve:Curve, length:float) :  float [] =
        let rc = curve.DivideByLength(length, includeEnds=true)
        if isNull rc then
            let len = curve.GetLength()
            if len < length then
                RhinoScriptingException.Raise "DivideCurveLength failed on too short curve. curve:'%A' dived-length:%f, curveLength=%f" curve length len
            else
                RhinoScriptingException.Raise "DivideCurveLength failed. curve:'%A' dived-length:%f, curveLength=%f" curve length len
        rc

    /// <summary>Divides a Curve object into segments of a specified length.
    /// If length is more than Curve length it fails.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="length">(float) The length of each segment</param>
    /// <returns>( float array) a list containing 3D division parameters.</returns>
    static member DivideCurveLength(curveId:Guid, length:float) :  float [] =
        RhinoScriptSyntax.DivideCurveLength(RhinoScriptSyntax.CoerceCurve(curveId), length)

    /// <summary>Returns the center point of an elliptical-shaped Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <returns>(Point3d) The 3D center point of the ellipse.</returns>
    static member EllipseCenterPoint(curveId:Guid) : Point3d =
        RhinoScriptSyntax.EllipseCenterPoint(RhinoScriptSyntax.CoerceCurve(curveId))

    /// <summary>Returns the center point of an elliptical-shaped Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <returns>(Point3d) The 3D center point of the ellipse.</returns>
    static member EllipseCenterPoint(curveGeo:Curve) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let rc, ellipse = curve.TryGetEllipse()
        if not <| rc then  RhinoScriptingException.Raise "EllipseCenterPoint: Curve is not an ellipse. curveGeo:'%s'" (Pretty.str curveGeo)
        ellipse.Plane.Origin


    /// <summary>Returns the quadrant points of an elliptical-shaped Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <returns>(Point3d * Point3d * Point3d * Point3d) Four points identifying the quadrants of the ellipse.</returns>
    static member EllipseQuadPoints(curveId:Guid) : Point3d * Point3d * Point3d * Point3d =
        RhinoScriptSyntax.EllipseQuadPoints(RhinoScriptSyntax.CoerceCurve(curveId))

    /// <summary>Returns the quadrant points of an elliptical-shaped Curve object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <returns>(Point3d * Point3d * Point3d * Point3d) Four points identifying the quadrants of the ellipse.</returns>
    static member EllipseQuadPoints(curveGeo:Curve) : Point3d * Point3d * Point3d * Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let rc, ellipse = curve.TryGetEllipse()
        if not <| rc then  RhinoScriptingException.Raise "EllipseQuadPoints: Curve is not an ellipse. curveGeo:'%s'" (Pretty.str curveGeo)
        let origin = ellipse.Plane.Origin;
        let xAxis = ellipse.Radius1 * ellipse.Plane.XAxis;
        let yAxis = ellipse.Radius2 * ellipse.Plane.YAxis;
        (origin-xAxis, origin + xAxis, origin-yAxis, origin + yAxis)


    /// <summary>Evaluates a Curve at a parameter and returns a 3D point.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="t">(float) The parameter to evaluate</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d) a 3-D point.</returns>
    static member EvaluateCurve(curveId:Guid, t:float, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        RhinoScriptSyntax.EvaluateCurve(RhinoScriptSyntax.CoerceCurve(curveId), t, segmentIndex)

    /// <summary>Evaluates a Curve at a parameter and returns a 3D point. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="t">(float) The parameter to evaluate</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(Point3d) a 3-D point.</returns>
    static member EvaluateCurve(curveGeo:Curve, t:float, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        curve.PointAt(t)


    /// <summary>Explodes, or un-joins, one Curve. PolyCurves will be exploded into Curve
    ///    segments. Polylines will be exploded into line segments. ExplodeCurves will
    ///    return the Curves in topological order.</summary>
    /// <param name="curveId">(Guid) The Curve object to explode</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Delete input objects after exploding if True</param>
    /// <returns>(Guid ResizeArray) identifying the newly created Curve objects.</returns>
    static member ExplodeCurve(curveId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid ResizeArray =
        let pieces = RhinoScriptSyntax.ExplodeCurve(RhinoScriptSyntax.CoerceCurve(curveId))
        let ids = pieces |> RArr.mapSeq State.Doc.Objects.AddCurve
        if deleteInput && pieces.Count > 0 then State.Doc.Objects.Delete(curveId, quiet=true) |> ignore<bool>
        if ids.Count > 0 then State.Doc.Views.Redraw()
        ids

    /// <summary>Computes ExplodeCurve from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input curve geometry.</param>
    /// <returns>(Curve ResizeArray) The resulting geometry; it is not added to a document.</returns>
    static member ExplodeCurve(curveGeo:Curve) : Curve ResizeArray =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let pieces = curve.DuplicateSegments()
        if isNull pieces then ResizeArray<Curve>() else ResizeArray(pieces)

    /// <summary>Explodes, or un-joins, several curves Curve. PolyCurves will be exploded into Curve
    ///    segments. Polylines will be exploded into line segments. ExplodeCurves will
    ///    return the Curves in topological order.</summary>
    /// <param name="curveIds">(Guid seq) The Curve objects to explode</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Delete input objects after exploding if True</param>
    /// <returns>(Guid ResizeArray) identifying the newly created Curve objects.</returns>
    static member ExplodeCurves(curveIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid ResizeArray =
        let ids = ResizeArray<Guid>()
        for curveId in curveIds do
            let pieces = RhinoScriptSyntax.ExplodeCurve(RhinoScriptSyntax.CoerceCurve(curveId))
            for piece in pieces do ids.Add(State.Doc.Objects.AddCurve(piece))
            if deleteInput && pieces.Count > 0 then State.Doc.Objects.Delete(curveId, quiet=true) |> ignore<bool>
        if ids.Count > 0 then State.Doc.Views.Redraw()
        ids

    /// <summary>Computes ExplodeCurves from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveInputs">(Curve seq) The input objects.</param>
    /// <returns>(Curve ResizeArray) The resulting geometry; it is not added to a document.</returns>
    static member ExplodeCurves(curveInputs:Curve seq) : Curve ResizeArray =
        let pieces = ResizeArray<Curve>()
        for curve in curveInputs do pieces.AddRange(RhinoScriptSyntax.ExplodeCurve(curve))
        pieces

    /// <summary>Extends a non-closed Curve object by a line, arc, or smooth extension
    ///    until it intersects a collection of objects.</summary>
    /// <param name="curveId">(Guid) Identifier of Curve to extend</param>
    /// <param name="extensionType">(int)
    ///    0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    /// <param name="side">(int)
    ///    0 = extend from the start of the Curve
    ///    1 = extend from the end of the Curve
    ///    2 = extend from both the start and the end of the Curve</param>
    /// <param name="boundaryCurveIds">(Guid seq) Curve, Surface, and Polysurface objects to extend to</param>
    /// <param name="replaceInput">(bool) Optional, Default Value <c>false</c> Replace input or add new?</param>
    /// <returns>(Guid) The identifier of the new or modified Curve.</returns>
    static member ExtendCurve(  curveId:Guid,
                                extensionType:int,
                                side:int,
                                boundaryCurveIds:Guid seq,
                                [<OPT;DEF(false)>]replaceInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let newCurve = RhinoScriptSyntax.ExtendCurve(curve, extensionType, side, boundaryCurveIds |> Seq.map RhinoScriptSyntax.CoerceGeometry)
        if replaceInput then
            if not (State.Doc.Objects.Replace(curveId, newCurve)) then RhinoScriptingException.Raise "ExtendCurve: Unable to replace curve %O" curveId
            State.Doc.Views.Redraw()
            curveId
        else
            let rc = State.Doc.Objects.AddCurve(newCurve)
            State.Doc.Views.Redraw()
            rc

    /// <summary>Computes ExtendCurve from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input curve geometry.</param>
    /// <param name="extensionType">(int)
    ///    0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    /// <param name="side">(int)
    ///    0 = extend from the start of the Curve
    ///    1 = extend from the end of the Curve
    ///    2 = extend from both the start and the end of the Curve</param>
    /// <param name="boundaryCurves">(GeometryBase seq) The input objects.</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member ExtendCurve(  curveGeo:Curve,
                                extensionType:int,
                                side:int,
                                boundaryCurves:GeometryBase seq) : Curve =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let mutable extensionTypet = CurveExtensionStyle.Line
        if extensionType = 0   then extensionTypet <- CurveExtensionStyle.Line
        elif extensionType = 1 then extensionTypet <- CurveExtensionStyle.Arc
        elif extensionType = 2 then extensionTypet <- CurveExtensionStyle.Smooth
        else RhinoScriptingException.Raise "ExtendCurve ExtensionType must be 0, 1, or 2. curveGeo:'%s' extensionType:'%A' side:'%A' boundaryCurves:'%s'" (Pretty.str curveGeo) extensionType side  (Pretty.str boundaryCurves)

        let sidet =
            match side with
            |0  -> CurveEnd.Start
            |1  -> CurveEnd.End
            |2  -> CurveEnd.Both
            |_  -> RhinoScriptingException.Raise "ExtendCurve Side must be 0, 1, or 2. curveGeo:'%s' extensionType:'%A' side:'%A' boundaryCurves:'%s'" (Pretty.str curveGeo) extensionType side  (Pretty.str boundaryCurves)

        let rhobjs = ResizeArray()
        for o in boundaryCurves do rhobjs.Add(o)

        if rhobjs.Count=0 then  RhinoScriptingException.Raise "ExtendCurve boundaryCurves failed. They must contain at least one item. curveGeo:'%s' extensionType:'%A' side:'%A' boundaryCurves:'%s'" (Pretty.str curveGeo) extensionType side (Pretty.str boundaryCurves)
        let geometry = rhobjs
        let newCurve = curve.Extend(sidet, extensionTypet, geometry)
        if isNull newCurve || not newCurve.IsValid then RhinoScriptingException.Raise "ExtendCurve: Unable to extend curve"
        newCurve

    /// <summary>Extends a non-closed Curve by a line, arc, or smooth extension for a specified distance.</summary>
    /// <param name="curveId">(Guid) Curve to extend</param>
    /// <param name="extensionType">(int)
    ///    0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    /// <param name="side">(int)
    ///    0 = extend from start of the Curve
    ///    1 = extend from end of the Curve
    ///    2 = Extend from both ends</param>
    /// <param name="length">(float) Distance to extend</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member ExtendCurveLength(    curveId:Guid,
                                        extensionType:int,
                                        side:int,
                                        length:float) : Guid =
        let newCurve = RhinoScriptSyntax.ExtendCurveLength(RhinoScriptSyntax.CoerceCurve(curveId), extensionType, side, length)
        if not (State.Doc.Objects.Replace(curveId, newCurve)) then RhinoScriptingException.Raise "ExtendCurveLength: Unable to replace curve %O" curveId
        State.Doc.Views.Redraw()
        curveId

    /// <summary>Computes ExtendCurveLength from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input curve geometry.</param>
    /// <param name="extensionType">(int)
    ///    0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    /// <param name="side">(int)
    ///    0 = extend from start of the Curve
    ///    1 = extend from end of the Curve
    ///    2 = Extend from both ends</param>
    /// <param name="length">(float) Distance to extend</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member ExtendCurveLength(    curveGeo:Curve,
                                        extensionType:int,
                                        side:int,
                                        length:float) : Curve =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let mutable extensionTypet = CurveExtensionStyle.Line
        if extensionType   = 0 then extensionTypet <- CurveExtensionStyle.Line
        elif extensionType = 1 then extensionTypet <- CurveExtensionStyle.Arc
        elif extensionType = 2 then extensionTypet <- CurveExtensionStyle.Smooth
        else RhinoScriptingException.Raise "ExtendCurveLength ExtensionType must be 0, 1, or 2. curveGeo:'%s' extensionType:'%A' side:'%A' length:'%A'" (Pretty.str curveGeo) extensionType side length

        let sideT =
            match side with
            |0  -> CurveEnd.Start
            |1  -> CurveEnd.End
            |2  -> CurveEnd.Both
            |_  -> RhinoScriptingException.Raise "ExtendCurveLength Side must be 0, 1, or 2. curveGeo:'%s' extensionType:'%A' side:'%A' length:'%A'" (Pretty.str curveGeo) extensionType side length

        let newCurve =
            if length<0. then curve.Trim(sideT, -length)
            else curve.Extend(sideT, length, extensionTypet)
        if isNull newCurve || not newCurve.IsValid then RhinoScriptingException.Raise "ExtendCurveLength: Unable to extend curve"
        newCurve

    /// <summary>Extends a non-closed Curve by smooth extension to a point.</summary>
    /// <param name="curveId">(Guid) Curve to extend</param>
    /// <param name="side">(int)
    ///    0 = extend from start of the Curve
    ///    1 = extend from end of the Curve
    ///    2 = extend from both the start and the end of the Curve</param>
    /// <param name="point">(Point3d) Point to extend to</param>
    /// <param name="extensionType">(int) Optional, default value: <c>2</c> ( CurveExtensionStyle.Smooth))
    ///    0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member ExtendCurvePoint( curveId:Guid,
                                    side:int,
                                    point:Point3d,
                                    [<OPT;DEF(-1)>]extensionType:int) : Guid =
        let newCurve = RhinoScriptSyntax.ExtendCurvePoint(RhinoScriptSyntax.CoerceCurve(curveId), side, point, extensionType)
        if not (State.Doc.Objects.Replace(curveId, newCurve)) then RhinoScriptingException.Raise "ExtendCurvePoint: Unable to replace curve %O" curveId
        State.Doc.Views.Redraw()
        curveId

    /// <summary>Computes ExtendCurvePoint from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input curve geometry.</param>
    /// <param name="side">(int)
    ///    0 = extend from start of the Curve
    ///    1 = extend from end of the Curve
    ///    2 = extend from both the start and the end of the Curve</param>
    /// <param name="point">(Point3d) Point to extend to</param>
    /// <param name="extensionType">(int) Optional, default value: <c>2</c> ( CurveExtensionStyle.Smooth))
    ///    0 = line
    ///    1 = arc
    ///    2 = smooth</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member ExtendCurvePoint( curveGeo:Curve,
                                    side:int,
                                    point:Point3d,
                                    [<OPT;DEF(-1)>]extensionType:int) : Curve =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let extensionTypet =
            match extensionType with
            | -1 ->  CurveExtensionStyle.Smooth
            |  0 ->  CurveExtensionStyle.Line
            |  1 ->  CurveExtensionStyle.Arc
            |  2 ->  CurveExtensionStyle.Smooth
            |  _ ->  RhinoScriptingException.Raise "ExtendCurvePoint ExtensionType must be 0, 1, or 2. curveGeo:'%s' side:'%A' point:'%A' extensionType:'%A'" (Pretty.str curveGeo) side point extensionType

        let sidet =
            match side with
            | 0  -> CurveEnd.Start
            | 1  -> CurveEnd.End
            | 2  -> CurveEnd.Both
            | _  -> RhinoScriptingException.Raise "ExtendCurvePoint Side must be 0, 1, or 2. curveGeo:'%s' side:'%A' point:'%A' extensionType:'%A'" (Pretty.str curveGeo) side point extensionType

        let newCurve = curve.Extend(sidet, extensionTypet, point)
        if isNull newCurve || not newCurve.IsValid then RhinoScriptingException.Raise "ExtendCurvePoint: Unable to extend curve"
        newCurve

    /// <summary>Fairs a Curve. Fair works best on degree 3 (cubic) Curves. Fair attempts
    ///    to remove large curvature variations while limiting the geometry changes to
    ///    be no more than the specified tolerance. Sometimes several applications of
    ///    this method are necessary to remove nasty curvature problems.</summary>
    /// <param name="curveId">(Guid) Curve to fair</param>
    /// <param name="tolerance">(float) Fairing tolerance</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member FairCurve(curveId:Guid, tolerance:float) : bool =
        let mutable curve = RhinoScriptSyntax.CoerceCurve curveId
        let angleTol = 0.0
        let mutable clamp = 0
        if curve.IsPeriodic then
            curve <- curve.ToNurbsCurve()
            clamp <- 1
        let newCurve = curve.Fair(tolerance, angleTol, clamp, clamp, 100)
        if notNull newCurve then  false
        else
            if State.Doc.Objects.Replace(curveId, newCurve) then
                State.Doc.Views.Redraw()
                true
            else
                false


    /// <summary>Reduces number of Curve control points while maintaining the Curve's same
    ///    general shape. Use this function for replacing Curves with many control
    ///    points. For more information, see the Rhino help for the FitCrv command.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="degree">(int) Optional, default value: <c>3</c>
    ///    The Curve degree, which must be greater than 1.
    ///    The default is 3</param>
    /// <param name="distanceTolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The fitting tolerance.</param>
    /// <param name="angleTolerance">(float) Optional, default value: <c>State.Doc.ModelAngleToleranceRadians</c>
    ///    The kink smoothing tolerance in degrees. If
    ///    angleTolerance is 0.0, all kinks are smoothed. If angleTolerance
    ///    is bigger than  0.0, kinks smaller than angleTolerance are smoothed. If
    ///    angleTolerance is not specified or smaller than 0.0, the document angle
    ///    tolerance is used for the kink smoothing</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member FitCurve( curveId:Guid,
                            [<OPT;DEF(3)>]degree:int,
                            [<OPT;DEF(0.0)>]distanceTolerance:float,
                            [<OPT;DEF(-1.0)>]angleTolerance:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let fitted = RhinoScriptSyntax.FitCurve(curve, degree, distanceTolerance, angleTolerance)
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let rc = State.Doc.Objects.AddCurve(fitted, rhobj.Attributes)
        if rc = Guid.Empty then RhinoScriptingException.Raise "FitCurve: Unable to add curve to document"
        State.Doc.Views.Redraw()
        rc

    /// <summary>Computes FitCurve from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input curve geometry.</param>
    /// <param name="degree">(int) Optional, default value: <c>3</c>
    ///    The Curve degree, which must be greater than 1.
    ///    The default is 3</param>
    /// <param name="distanceTolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The fitting tolerance.</param>
    /// <param name="angleTolerance">(float) Optional, default value: <c>State.Doc.ModelAngleToleranceRadians</c>
    ///    The kink smoothing tolerance in degrees. If
    ///    angleTolerance is 0.0, all kinks are smoothed. If angleTolerance
    ///    is bigger than  0.0, kinks smaller than angleTolerance are smoothed. If
    ///    angleTolerance is not specified or smaller than 0.0, the document angle
    ///    tolerance is used for the kink smoothing</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member FitCurve( curveGeo:Curve,
                            [<OPT;DEF(3)>]degree:int,
                            [<OPT;DEF(0.0)>]distanceTolerance:float,
                            [<OPT;DEF(-1.0)>]angleTolerance:float) : Curve =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let distanceTolerance0 = Util.ifZero1 distanceTolerance State.Doc.ModelAbsoluteTolerance
        let angleTolerance0 = if angleTolerance < 0.0 then State.Doc.ModelAngleToleranceRadians else toRadians angleTolerance
        let fitted = curve.Fit(degree, distanceTolerance0, angleTolerance0)
        if isNull fitted then RhinoScriptingException.Raise "FitCurve: Unable to fit curve"
        fitted

    /// <summary>Inserts a knot into a Curve object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="parameter">(float) Parameter on the Curve</param>
    /// <param name="symmetrical">(bool) Optional, default value: <c>false</c>
    ///    If True, then knots are added on both sides of
    ///    the center of the Curve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
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
                            if rc then  State.Doc.Views.Redraw()
                    if rc then
                        rc <- State.Doc.Objects.Replace(curveId, nc)
                        if rc then  State.Doc.Views.Redraw()
                rc


    /// <summary>Checks if an object is an open arc Curve. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c>
    ///    If the Curve is not a circle, then the tolerance used
    ///    to determine whether or not the NURBS form of the Curve has the
    ///    properties of a arc.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False.</returns>
    static member IsArc(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsArc((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), tolerance, segmentIndex)

    /// <summary>Checks if an object is an open arc Curve. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c>
    ///    If the Curve is not a circle, then the tolerance used
    ///    to determine whether or not the NURBS form of the Curve has the
    ///    properties of a arc.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False.</returns>
    static member IsArc(curveGeo:GeometryBase, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = Util.ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsArc(tol) && not curve.IsClosed


    /// <summary>Checks if an object is a circle Curve.  Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c>
    ///    If the Curve is not a circle, then the tolerance used
    ///    to determine whether or not the NURBS form of the Curve has the
    ///    properties of a circle.</param>
    /// <returns>(bool) True or False.</returns>
    static member IsCircle(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        RhinoScriptSyntax.IsCircle((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), tolerance)

    /// <summary>Checks if an object is a circle Curve.  Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c>
    ///    If the Curve is not a circle, then the tolerance used
    ///    to determine whether or not the NURBS form of the Curve has the
    ///    properties of a circle.</param>
    /// <returns>(bool) True or False.</returns>
    static member IsCircle(curveGeo:GeometryBase, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tol = Util.ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, -1) with
        |None -> false
        |Some curve  -> curve.IsCircle(tol)


    /// <summary>Checks if an object is a Curve. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) The object's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsCurve(curveId:Guid) : bool =
        RhinoScriptSyntax.IsCurve((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null))

    /// <summary>Checks if an object is a Curve. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <returns>(bool) True or False.</returns>
    static member IsCurve(curveGeo:GeometryBase) : bool =
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, -1) with
        |None -> false
        |Some _  -> true


    /// <summary>Decide if it makes sense to close off the Curve by moving the end point
    ///    to the start point based on start-end gap size and length of Curve as
    ///    approximated by chord defined by 6 points. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Maximum allowable distance between start point and end point.</param>
    /// <returns>(bool) True or False.</returns>
    static member IsCurveClosable(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        RhinoScriptSyntax.IsCurveClosable((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), tolerance)

    /// <summary>Decide if it makes sense to close off the Curve by moving the end point
    ///    to the start point based on start-end gap size and length of Curve as
    ///    approximated by chord defined by 6 points. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Maximum allowable distance between start point and end point.</param>
    /// <returns>(bool) True or False.</returns>
    static member IsCurveClosable(curveGeo:GeometryBase, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance0 = Util.ifZero2 State.Doc.ModelAbsoluteTolerance tolerance
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, -1) with
        |None -> false
        |Some curve  -> curve.IsClosable(tolerance0)


    /// <summary>Checks if an object is a closed Curve object. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) The object's identifier</param>
    /// <returns>(bool) If Curve is Closed True,  otherwise False.</returns>
    static member IsCurveClosed(curveId:Guid) : bool =
        RhinoScriptSyntax.IsCurveClosed((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null))

    /// <summary>Checks if an object is a closed Curve object. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <returns>(bool) If Curve is Closed True,  otherwise False.</returns>
    static member IsCurveClosed(curveGeo:GeometryBase) : bool =
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, -1) with
        |None -> false
        |Some curve  -> curve.IsClosed


    /// <summary>Test a Curve to see if it lies in a specific Plane. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) The object's identifier</param>
    /// <param name="plane">(Plane) Plane to test</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <returns>(bool) True or False.</returns>
    static member IsCurveInPlane(curveId:Guid, plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        RhinoScriptSyntax.IsCurveInPlane((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), plane, tolerance)

    /// <summary>Test a Curve to see if it lies in a specific Plane. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="plane">(Plane) Plane to test</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <returns>(bool) True or False.</returns>
    static member IsCurveInPlane(curveGeo:GeometryBase, plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance0 = Util.ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, -1) with
        |None -> false
        |Some curve  -> curve.IsInPlane(plane, tolerance0)


    /// <summary>Checks if an object is a linear Curve. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>    ///
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsCurveLinear(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsCurveLinear((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), tolerance, segmentIndex)

    /// <summary>Checks if an object is a linear Curve. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>    ///
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsCurveLinear(curveGeo:GeometryBase, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tolerance0 = Util.ifZero2 RhinoMath.ZeroTolerance tolerance
        match  RhinoScriptSyntax.TryDirectCurveInput(curveGeo, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsLinear(tolerance0)


    /// <summary>Checks if an object is a periodic Curve object. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False.</returns>
    static member IsCurvePeriodic(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsCurvePeriodic((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), segmentIndex)

    /// <summary>Checks if an object is a periodic Curve object. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional,
    ///    The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False.</returns>
    static member IsCurvePeriodic(curveGeo:GeometryBase, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsPeriodic


    /// <summary>Checks if an object is a planar Curve. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsCurvePlanar(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsCurvePlanar((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), tolerance, segmentIndex)

    /// <summary>Checks if an object is a planar Curve. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsCurvePlanar(curveGeo:GeometryBase, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = Util.ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsPlanar(tol)


    /// <summary>Checks if an object is a rational NURBS Curve. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsCurveRational(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsCurveRational((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), segmentIndex)

    /// <summary>Checks if an object is a rational NURBS Curve. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsCurveRational(curveGeo:GeometryBase, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, segmentIndex) with
        |None -> false
        |Some c  ->
            match c with
            | :? NurbsCurve as curve -> curve.IsRational
            |_ -> false


    /// <summary>Checks if an object is an elliptical-shaped Curve. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <param name="segmentIndex">(int) Optional,
    ///    The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsEllipse(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsEllipse((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), tolerance, segmentIndex)

    /// <summary>Checks if an object is an elliptical-shaped Curve. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <param name="segmentIndex">(int) Optional,
    ///    The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsEllipse(curveGeo:GeometryBase, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = Util.ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsEllipse(tol)


    /// <summary>Checks if an object is a line Curve. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <param name="segmentIndex">(int) Optional,
    ///    The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsLine(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsLine((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), tolerance, segmentIndex)

    /// <summary>Checks if an object is a line Curve. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c></param>
    /// <param name="segmentIndex">(int) Optional,
    ///    The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsLine(curveGeo:GeometryBase, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = Util.ifZero2 RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, segmentIndex) with
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


    /// <summary>Checks if a point is on a Curve. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="point">(Point3d) The test point</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.SqrtEpsilon</c></param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsPointOnCurve(curveId:Guid, point:Point3d, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsPointOnCurve(RhinoScriptSyntax.CoerceCurve(curveId), point, tolerance, segmentIndex)

    /// <summary>Checks if a point is on a Curve. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="point">(Point3d) The test point</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.SqrtEpsilon</c></param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member IsPointOnCurve(curveGeo:Curve, point:Point3d, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = Util.ifZero2 RhinoMath.SqrtEpsilon tolerance
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let t = ref 0.0
        curve.ClosestPoint(point, t, tol)


    /// <summary>Checks if an object is a PolyCurve Curve. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False.</returns>
    static member IsPolyCurve(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsPolyCurve((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), segmentIndex)

    /// <summary>Checks if an object is a PolyCurve Curve. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False.</returns>
    static member IsPolyCurve(curveGeo:GeometryBase, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        // TODO can a PolyCurve be nested in a PolyCurve ?
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, segmentIndex) with
        |None               -> false
        |Some c  ->
            match c with
            | :? PolyCurve  -> true
            | _             -> false


    /// <summary>Checks if an object is a Polyline Curve object or a nurbs cure with degree 1 and moer than 2 points
    /// Lines return false. Returns false for any other Rhino object.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False.</returns>
    static member IsPolyline(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        RhinoScriptSyntax.IsPolyline((RhinoScriptSyntax.TryCoerceGeometry(curveId) |> Option.defaultValue null), segmentIndex)

    /// <summary>Checks if an object is a Polyline Curve object or a nurbs cure with degree 1 and moer than 2 points
    /// Lines return false. Returns false for any other Rhino object. Uses the supplied geometry directly.</summary>
    /// <param name="curveGeo">(GeometryBase) The input geometry.</param>
    /// <param name="segmentIndex">(int) Optional, The Curve segment index if `curveId` identifies a PolyCurve</param>
    /// <returns>(bool) True or False.</returns>
    static member IsPolyline(curveGeo:GeometryBase, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryDirectCurveInput(curveGeo, segmentIndex) with
        |None               -> false
        |Some c  ->
            match c with
            | :? PolylineCurve  -> true
            | :? NurbsCurve as nc -> nc.Points.Count > 2 && c.Degree = 1
            | _ -> false



    /// <summary>Joins multiple Curves together to form one or more Curves or polycurves.</summary>
    /// <param name="curveIds">(Guid seq) List of multiple Curves</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Delete input objects after joining</param>
    /// <param name="tolerance">(float) Optional, default value: <c>2.1 * State.Doc.ModelAbsoluteTolerance</c>
    ///    Join tolerance. If omitted, 2.1 * document absolute
    ///    tolerance is used</param>
    /// <returns>(Guid ResizeArray) Object objectId representing the new Curves.</returns>
    static member JoinCurves(curveIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let curveIds = Seq.toArray curveIds
        let curves = RhinoScriptSyntax.JoinCurves(curveIds |> Seq.map RhinoScriptSyntax.CoerceCurve, tolerance)
        let ids = curves |> RArr.mapSeq State.Doc.Objects.AddCurve
        if deleteInput then
            for objectId in curveIds do State.Doc.Objects.Delete(objectId, quiet=false) |> ignore<bool>
        State.Doc.Views.Redraw()
        ids

    /// <summary>Computes JoinCurves from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveInputs">(Curve seq) The input objects.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>2.1 * State.Doc.ModelAbsoluteTolerance</c>
    ///    Join tolerance. If omitted, 2.1 * document absolute
    ///    tolerance is used</param>
    /// <returns>(Curve ResizeArray) The resulting geometry; it is not added to a document.</returns>
    static member JoinCurves(curveInputs:Curve seq, [<OPT;DEF(0.0)>]tolerance:float) : Curve ResizeArray =
        let curves = curveInputs |> Seq.map (fun curve -> RhinoScriptSyntax.DirectCurveInput(curve, -1)) |> Seq.toArray
        if curves.Length = 0 then RhinoScriptingException.Raise "JoinCurves: Curves must contain at least two items"
        let tolerance0 = Util.ifZero1 tolerance (2.1 * State.Doc.ModelAbsoluteTolerance)
        let joined = Curve.JoinCurves(curves, tolerance0)
        if isNull joined then RhinoScriptingException.Raise "JoinCurves: Unable to join curves"
        ResizeArray(joined)

    /// <summary>Returns a line that was fit through an array of 3D points.</summary>
    /// <param name="points">(Point3d seq) A list of at least two 3D points</param>
    /// <returns>(Line) line.</returns>
    static member LineFitFromPoints(points:Point3d seq) : Line =
        let rc, line = Line.TryFitLineToPoints(points)
        if rc then  line
        else RhinoScriptingException.Raise "LineFitFromPoints failed.  points:'%A'" points


    /// <summary>Makes a periodic Curve non-periodic. Non-periodic Curves can develop
    ///    kinks when deformed.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve object</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Delete the input Curve. If omitted, the input Curve will not be deleted</param>
    /// <returns>(Guid) objectId of the new or modified Curve.</returns>
    static member MakeCurveNonPeriodic(curveId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let curve = RhinoScriptSyntax.MakeCurveNonPeriodic(RhinoScriptSyntax.CoerceCurve(curveId))
        if deleteInput then
            if not (State.Doc.Objects.Replace(curveId, curve)) then RhinoScriptingException.Raise "MakeCurveNonPeriodic: Unable to replace curve %O" curveId
            State.Doc.Views.Redraw()
            curveId
        else
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
            let rc = State.Doc.Objects.AddCurve(curve, rhobj.Attributes)
            if rc = Guid.Empty then RhinoScriptingException.Raise "MakeCurveNonPeriodic: Unable to add curve to document"
            State.Doc.Views.Redraw()
            rc

    /// <summary>Computes MakeCurveNonPeriodic from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input curve geometry.</param>
    /// <returns>(NurbsCurve) The resulting geometry; it is not added to a document.</returns>
    static member MakeCurveNonPeriodic(curveGeo:Curve) : NurbsCurve =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        if not curve.IsPeriodic then RhinoScriptingException.Raise "MakeCurveNonPeriodic: Curve must be periodic"
        use duplicate = curve.DuplicateCurve()
        let result = duplicate.ToNurbsCurve()
        if isNull result || not (result.Knots.ClampEnd(CurveEnd.Both)) then
            RhinoScriptingException.Raise "MakeCurveNonPeriodic: Unable to clamp curve knots"
        result

    /// <summary>Creates an average Curve from two Curves.</summary>
    /// <param name="curve0">(Guid) identifiers of first Curve</param>
    /// <param name="curve1">(Guid) identifiers of second Curve</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Angle tolerance used to match kinks between Curves</param>
    /// <returns>(Guid) objectId of the new or modified Curve.</returns>
    static member MeanCurve(curve0:Guid, curve1:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid =
        let curve = RhinoScriptSyntax.MeanCurve(RhinoScriptSyntax.CoerceCurve(curve0), RhinoScriptSyntax.CoerceCurve(curve1), tolerance)
        let rc = State.Doc.Objects.AddCurve(curve)
        State.Doc.Views.Redraw()
        rc

    /// <summary>Computes MeanCurve from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curve0">(Curve) The input geometry.</param>
    /// <param name="curve1">(Curve) The input geometry.</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Angle tolerance used to match kinks between Curves</param>
    /// <returns>(Curve) The resulting geometry; it is not added to a document.</returns>
    static member MeanCurve(curve0:Curve, curve1:Curve, [<OPT;DEF(0.0)>]tolerance:float) : Curve =
        let curve0 = RhinoScriptSyntax.DirectCurveInput(curve0, -1)
        let curve1 = RhinoScriptSyntax.DirectCurveInput(curve1, -1)
        let tolerance = if tolerance = 0.0 then RhinoMath.UnsetValue else abs tolerance
        let curve = Curve.CreateMeanCurve(curve0, curve1, tolerance)
        if isNull curve then RhinoScriptingException.Raise "MeanCurve: Unable to create mean curve"
        curve

    /// <summary>Creates a polygon Mesh object based on a closed Polyline Curve object.
    ///    The created Mesh object is added to the document.</summary>
    /// <param name="polylineId">(Guid) Identifier of the Polyline Curve object</param>
    /// <returns>(Guid) identifier of the new Mesh object.</returns>
    static member MeshPolyline(polylineId:Guid) : Guid =
        let geometry = RhinoScriptSyntax.MeshPolyline(RhinoScriptSyntax.CoerceCurve(polylineId))
        let rc = State.Doc.Objects.AddMesh(geometry)
        State.Doc.Views.Redraw()
        rc

    /// <summary>Computes MeshPolyline from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="polylineInput">(Curve) The input geometry.</param>
    /// <returns>(Mesh) The resulting geometry; it is not added to a document.</returns>
    static member MeshPolyline(polylineInput:Curve) : Mesh =
        let curve = RhinoScriptSyntax.DirectCurveInput(polylineInput, -1)
        let ispolyline, polyline = curve.TryGetPolyline()
        if not <| ispolyline then  RhinoScriptingException.Raise "MeshPolyline failed.  polylineInput:'%s'" (Pretty.str polylineInput)
        let mesh = Mesh.CreateFromClosedPolyline(polyline)
        if isNull mesh then  RhinoScriptingException.Raise "MeshPolyline failed.  polylineInput:'%s'" (Pretty.str polylineInput)
        mesh

    /// <summary>Offsets a Curve by a distance. The offset Curve will be added to Rhino.</summary>
    /// <param name="curveId">(Guid) Identifier of a Curve object</param>
    /// <param name="direction">(Point3d) Point describing direction of the offset</param>
    /// <param name="distance">(float) Distance of the offset</param>
    /// <param name="normal">(Vector3d) Optional, default value: <c>Vector3d.ZAxis</c>
    ///    Normal of the Plane in which the offset will occur.
    ///    If omitted, the WorldXY Plane will be used</param>
    /// <param name="style">(int) Optional, default value: <c>1</c>
    ///    The corner style. If omitted, the style is sharp.
    ///    0 = None
    ///    1 = Sharp
    ///    2 = Round
    ///    3 = Smooth
    ///    4 = Chamfer</param>
    /// <returns>(Guid ResizeArray) list of ids for the new Curves.</returns>
    static member OffsetCurve(curveId:Guid, direction:Point3d, distance:float, [<OPT;DEF(Vector3d())>]normal:Vector3d, [<OPT;DEF(1)>]style:int) : Guid ResizeArray = //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let curves = RhinoScriptSyntax.OffsetCurve(RhinoScriptSyntax.CoerceCurve(curveId), direction, distance, normal, style)
        let ids = curves |> RArr.mapSeq State.Doc.Objects.AddCurve
        ids

    /// <summary>Computes OffsetCurve from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="direction">(Point3d) Point describing direction of the offset</param>
    /// <param name="distance">(float) Distance of the offset</param>
    /// <param name="normal">(Vector3d) Optional, default value: <c>Vector3d.ZAxis</c>
    ///    Normal of the Plane in which the offset will occur.
    ///    If omitted, the WorldXY Plane will be used</param>
    /// <param name="style">(int) Optional, default value: <c>1</c>
    ///    The corner style. If omitted, the style is sharp.
    ///    0 = None
    ///    1 = Sharp
    ///    2 = Round
    ///    3 = Smooth
    ///    4 = Chamfer</param>
    /// <returns>(Curve ResizeArray) The resulting geometry; it is not added to a document.</returns>
    static member OffsetCurve(curveGeo:Curve, direction:Point3d, distance:float, [<OPT;DEF(Vector3d())>]normal:Vector3d, [<OPT;DEF(1)>]style:int) : Curve ResizeArray = //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let normal0 = if normal.IsZero then Vector3d.ZAxis else normal
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let stylee:CurveOffsetCornerStyle = EnumOfValue style
        let curves = curve.Offset(direction, normal0, distance, tolerance, stylee)
        if isNull curves then  RhinoScriptingException.Raise "OffsetCurve failed. curveGeo:'%s' direction:'%A' distance:'%A' normal:'%A' style:%d" (Pretty.str curveGeo) direction distance normal style
        ResizeArray(curves)

    /// <summary>Offset a Curve on a Surface. The source Curve must lie on the Surface.
    ///    The offset Curve or Curves will be added to Rhino.</summary>
    /// <param name="curveId">(Guid) Curve identifiers</param>
    /// <param name="surfaceId">(Guid) Surface identifiers</param>
    /// <param name="parameter">(Point2d))  U, V parameter that the Curve will be offset through</param>
    /// <returns>(Guid ResizeArray) identifiers of the new Curves.</returns>
    static member OffsetCurveOnSurfaceUV(curveId:Guid, surfaceId:Guid, parameter:Point2d) : Guid ResizeArray =
        let curves = RhinoScriptSyntax.OffsetCurveOnSurfaceUV(RhinoScriptSyntax.CoerceCurve(curveId), RhinoScriptSyntax.CoerceSurface(surfaceId), parameter)
        let ids = curves |> RArr.mapSeq State.Doc.Objects.AddCurve
        ids

    /// <summary>Computes OffsetCurveOnSurfaceUV from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="surfaceGeo">(Surface) The input geometry.</param>
    /// <param name="parameter">(Point2d))  U, V parameter that the Curve will be offset through</param>
    /// <returns>(Curve ResizeArray) The resulting geometry; it is not added to a document.</returns>
    static member OffsetCurveOnSurfaceUV(curveGeo:Curve, surfaceGeo:Surface, parameter:Point2d) : Curve ResizeArray =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let surface = (surfaceGeo)
        let tol = State.Doc.ModelAbsoluteTolerance
        let curves = curve.OffsetOnSurface(surface, parameter, tol)
        if isNull curves then  RhinoScriptingException.Raise "OffsetCurveOnSurfaceUV failed. curveGeo:'%s' surfaceGeo:'%s' parameter:'%A'" (Pretty.str curveGeo) (Pretty.str surfaceGeo) parameter
        ResizeArray(curves)

    /// <summary>Offset a Curve on a Surface. The source Curve must lie on the Surface.
    ///    The offset Curve or Curves will be added to Rhino Document.</summary>
    /// <param name="curveId">(Guid) The Curve identifiers</param>
    /// <param name="surfaceId">(Guid) The Surface identifiers</param>
    /// <param name="distance">(float)) The distance of the offset. Based on the Curve's direction, a positive value
    ///    will offset to the left and a negative value will offset to the right</param>
    /// <returns>(Guid ResizeArray) identifiers of the new Curves.</returns>
    static member OffsetCurveOnSurface(curveId:Guid, surfaceId:Guid, distance:float) : Guid ResizeArray =
        let curves = RhinoScriptSyntax.OffsetCurveOnSurface(RhinoScriptSyntax.CoerceCurve(curveId), RhinoScriptSyntax.CoerceSurface(surfaceId), distance)
        let ids = curves |> RArr.mapSeq State.Doc.Objects.AddCurve
        State.Doc.Views.Redraw()
        ids

    /// <summary>Computes OffsetCurveOnSurface from geometry without modifying the document. Inputs are not modified.</summary>
    /// <param name="curveGeo">(Curve) The input geometry.</param>
    /// <param name="surfaceGeo">(Surface) The input geometry.</param>
    /// <param name="distance">(float)) The distance of the offset. Based on the Curve's direction, a positive value
    ///    will offset to the left and a negative value will offset to the right</param>
    /// <returns>(Curve ResizeArray) The resulting geometry; it is not added to a document.</returns>
    static member OffsetCurveOnSurface(curveGeo:Curve, surfaceGeo:Surface, distance:float) : Curve ResizeArray =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let surface = (surfaceGeo)
        let tol = State.Doc.ModelAbsoluteTolerance
        let curves = curve.OffsetOnSurface(surface, distance, tol)
        if isNull curves then  RhinoScriptingException.Raise "OffsetCurveOnSurface failed. curveGeo:'%s' surfaceGeo:'%s' distance:'%A'" (Pretty.str curveGeo) (Pretty.str surfaceGeo) distance
        let curves = curves  |> RArr.mapSeq (fun curve -> curve.ExtendOnSurface(Rhino.Geometry.CurveEnd.Both, surface) )//https://github.com/mcneel/rhinoscriptsyntax/pull/186
        curves

    ///<summary>Determines the relationship between the regions bounded by two coplanar simple closed Curves.</summary>
    ///<param name="curveA">(Guid) identifier of the first  planar, closed Curve</param>
    ///<param name="curveB">(Guid) identifier of the second planar, closed Curve</param>
    ///<param name="plane">(Plane) Optional, default value: <c>Plane.WorldXY</c>
    ///    Test Plane. If omitted, the Plane.WorldXY Plane is used</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(int) a number identifying the relationship
    ///    0 = the regions bounded by the Curves are disjoint
    ///    1 = the two Curves intersect
    ///    2 = the region bounded by CurveA is inside of CurveB
    ///    3 = the region bounded by CurveB is inside of CurveA.</returns>
    static member PlanarClosedCurveContainment(curveA:Guid, curveB:Guid, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        RhinoScriptSyntax.PlanarClosedCurveContainment(RhinoScriptSyntax.CoerceCurve(curveA), RhinoScriptSyntax.CoerceCurve(curveB), plane, tolerance)

    ///<summary>Determines the relationship between the regions bounded by two coplanar simple closed Curves. Uses the supplied geometry directly.</summary>
    ///<param name="curveA">(Curve) The input geometry.</param>
    ///<param name="curveB">(Curve) The input geometry.</param>
    ///<param name="plane">(Plane) Optional, default value: <c>Plane.WorldXY</c>
    ///    Test Plane. If omitted, the Plane.WorldXY Plane is used</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(int) a number identifying the relationship
    ///    0 = the regions bounded by the Curves are disjoint
    ///    1 = the two Curves intersect
    ///    2 = the region bounded by CurveA is inside of CurveB
    ///    3 = the region bounded by CurveB is inside of CurveA.</returns>
    static member PlanarClosedCurveContainment(curveA:Curve, curveB:Curve, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        let curveA = RhinoScriptSyntax.DirectCurveInput(curveA, -1)
        let curveB = RhinoScriptSyntax.DirectCurveInput(curveB, -1)
        let tolerance0 = Util.ifZero2 State.Doc.ModelAbsoluteTolerance tolerance
        let plane0 = if plane.IsValid then plane else Plane.WorldXY
        let rc = Curve.PlanarClosedCurveRelationship(curveA, curveB, plane0, tolerance0)
        int(rc)


    ///<summary>Determines if two coplanar Curves intersect.</summary>
    ///<param name="curveA">(Guid) identifier of the first  planar Curve</param>
    ///<param name="curveB">(Guid) identifier of the second planar Curve</param>
    ///<param name="plane">(Plane) Optional, default value: <c>Plane.WorldXY</c>
    ///    Test Plane. If omitted, the Plane.WorldXY Plane is used</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(bool) True if the Curves intersect; otherwise False.</returns>
    static member PlanarCurveCollision(curveA:Guid, curveB:Guid, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        RhinoScriptSyntax.PlanarCurveCollision(RhinoScriptSyntax.CoerceCurve(curveA), RhinoScriptSyntax.CoerceCurve(curveB), plane, tolerance)

    ///<summary>Determines if two coplanar Curves intersect. Uses the supplied geometry directly.</summary>
    ///<param name="curveA">(Curve) The input geometry.</param>
    ///<param name="curveB">(Curve) The input geometry.</param>
    ///<param name="plane">(Plane) Optional, default value: <c>Plane.WorldXY</c>
    ///    Test Plane. If omitted, the Plane.WorldXY Plane is used</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(bool) True if the Curves intersect; otherwise False.</returns>
    static member PlanarCurveCollision(curveA:Curve, curveB:Curve, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let curveA = RhinoScriptSyntax.DirectCurveInput(curveA, -1)
        let curveB = RhinoScriptSyntax.DirectCurveInput(curveB, -1)
        let tolerance0 = Util.ifZero2 State.Doc.ModelAbsoluteTolerance tolerance
        let plane0 = if plane.IsValid then plane else Plane.WorldXY
        Curve.PlanarCurveCollision(curveA, curveB, plane0, tolerance0)


    ///<summary>Determines if a point is inside of a closed Curve, on a closed Curve, or
    ///    outside of a closed Curve.</summary>
    ///<param name="point">(Point3d) Text point</param>
    ///<param name="curve">(Guid) Identifier of a Curve object</param>
    ///<param name="plane">(Plane) Optional, default value: <c>Plane.WorldXY</c>
    ///    Plane containing the closed Curve and point. If omitted, Plane.WorldXY  is used</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(int) number identifying the result
    ///    0 = point is outside of the Curve
    ///    1 = point is inside of the Curve
    ///    2 = point is on the Curve.</returns>
    static member PointInPlanarClosedCurve(point:Point3d, curve:Guid, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        RhinoScriptSyntax.PointInPlanarClosedCurve(point, RhinoScriptSyntax.CoerceCurve(curve), plane, tolerance)

    ///<summary>Determines if a point is inside of a closed Curve, on a closed Curve, or
    ///    outside of a closed Curve. Uses the supplied geometry directly.</summary>
    ///<param name="point">(Point3d) Text point</param>
    ///<param name="curve">(Curve) The input geometry.</param>
    ///<param name="plane">(Plane) Optional, default value: <c>Plane.WorldXY</c>
    ///    Plane containing the closed Curve and point. If omitted, Plane.WorldXY  is used</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(int) number identifying the result
    ///    0 = point is outside of the Curve
    ///    1 = point is inside of the Curve
    ///    2 = point is on the Curve.</returns>
    static member PointInPlanarClosedCurve(point:Point3d, curve:Curve, [<OPT;DEF(Plane())>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        let curve = RhinoScriptSyntax.DirectCurveInput(curve, -1)
        let tolerance0 = Util.ifZero2 State.Doc.ModelAbsoluteTolerance tolerance
        let plane0 = if plane.IsValid then plane else Plane.WorldXY
        let rc = curve.Contains(point, plane0, tolerance0)
        if rc= PointContainment.Unset then
            RhinoScriptingException.Raise "PointInPlanarClosedCurve Curve.Contains is Unset.  point:'%A' curve:'%A' plane:'%A' tolerance:'%A'" point curve plane tolerance
        if rc= PointContainment.Outside then  0
        elif rc= PointContainment.Inside then  1
        else 2


    ///<summary>Returns the number of Curve segments that make up a PolyCurve.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    If `curveId` identifies a PolyCurve object, then `segmentIndex` identifies the Curve segment of the PolyCurve to query</param>
    ///<returns>(int) The number of Curve segments in a PolyCurve.</returns>
    static member PolyCurveCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        RhinoScriptSyntax.PolyCurveCount(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    ///<summary>Returns the number of Curve segments that make up a PolyCurve. Uses the supplied geometry directly.</summary>
    ///<param name="curveGeo">(Curve) The input geometry.</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    If `curveId` identifies a PolyCurve object, then `segmentIndex` identifies the Curve segment of the PolyCurve to query</param>
    ///<returns>(int) The number of Curve segments in a PolyCurve.</returns>
    static member PolyCurveCount(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        match curve with
        | :? PolyCurve as curve ->  curve.SegmentCount
        | _ -> RhinoScriptingException.Raise "PolyCurveCount: CurveId does not reference a PolyCurve. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex


    ///<summary>Returns the vertices of a Polyline Curve.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    If CurveId identifies a PolyCurve object, then segmentIndex identifies the Curve segment of the PolyCurve to query</param>
    ///<returns>(Point3d ResizeArray) an list of Point3d vertex points.</returns>
    static member PolylineVertices(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d ResizeArray =
        RhinoScriptSyntax.PolylineVertices(RhinoScriptSyntax.CoerceCurve(curveId), segmentIndex)

    ///<summary>Returns the vertices of a Polyline Curve. Uses the supplied geometry directly.</summary>
    ///<param name="curveGeo">(Curve) The input geometry.</param>
    ///<param name="segmentIndex">(int) Optional,
    ///    If CurveId identifies a PolyCurve object, then segmentIndex identifies the Curve segment of the PolyCurve to query</param>
    ///<returns>(Point3d ResizeArray) an list of Point3d vertex points.</returns>
    static member PolylineVertices(curveGeo:Curve, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d ResizeArray =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, segmentIndex)
        let rc, polyline = curve.TryGetPolyline()
        if rc then
            let pts = ResizeArray(polyline.Count)
            for pt in polyline do pts.Add(pt)
            pts
        else RhinoScriptingException.Raise "PolylineVertices: CurveId does not <| reference a polyline. curveGeo:'%s' segmentIndex:'%A'" (Pretty.str curveGeo) segmentIndex


    ///<summary>Projects one or more Curves onto one or more Surfaces or Meshes.</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of Curves to project</param>
    ///<param name="meshIds">(Guid seq) Identifiers of Meshes to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid ResizeArray) list of identifiers for the resulting Curves.</returns>
    static member ProjectCurveToMesh(curveIds:Guid seq, meshIds:Guid seq, direction:Vector3d) : Guid ResizeArray =
        let curves = RhinoScriptSyntax.ProjectCurveToMesh((curveIds |> Seq.map RhinoScriptSyntax.CoerceCurve), (meshIds |> Seq.map RhinoScriptSyntax.CoerceMesh), direction)
        let ids = curves |> RArr.mapSeq State.Doc.Objects.AddCurve
        if ids.Count > 0 then State.Doc.Views.Redraw()
        ids

    ///<summary>Computes ProjectCurveToMesh from geometry without modifying the document. Inputs are not modified.</summary>
    ///<param name="curveInputs">(Curve seq) The input objects.</param>
    ///<param name="meshInputs">(Mesh seq) The input objects.</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Curve ResizeArray) The resulting curves, without adding them to the document.</returns>
    static member ProjectCurveToMesh(curveInputs:Curve seq, meshInputs:Mesh seq, direction:Vector3d) : Curve ResizeArray =
        let curves = curveInputs |> RArr.mapSeq id
        let meshes =  meshInputs |> RArr.mapSeq id
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let newCurves = Curve.ProjectToMesh(curves, meshes, direction, tolerance)
        ResizeArray(newCurves)

    ///<summary>Projects one or more Curves onto one or more Surfaces or Polysurfaces.</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of Curves to project</param>
    ///<param name="surfaceIds">(Guid seq) Identifiers of Surfaces to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid ResizeArray) list of identifiers.</returns>
    static member ProjectCurveToSurface(curveIds:Guid seq, surfaceIds:Guid seq, direction:Vector3d) : Guid ResizeArray =
        let curves = RhinoScriptSyntax.ProjectCurveToSurface((curveIds |> Seq.map RhinoScriptSyntax.CoerceCurve), (surfaceIds |> Seq.map RhinoScriptSyntax.CoerceBrep), direction)
        let ids = curves |> RArr.mapSeq State.Doc.Objects.AddCurve
        if ids.Count > 0 then State.Doc.Views.Redraw()
        ids

    ///<summary>Computes ProjectCurveToSurface from geometry without modifying the document. Inputs are not modified.</summary>
    ///<param name="curveInputs">(Curve seq) The input objects.</param>
    ///<param name="surfaceInputs">(Brep seq) The input objects.</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Curve ResizeArray) The resulting curves, without adding them to the document.</returns>
    static member ProjectCurveToSurface(curveInputs:Curve seq, surfaceInputs:Brep seq, direction:Vector3d) : Curve ResizeArray =
        let curves = curveInputs  |> RArr.mapSeq  id
        let breps = surfaceInputs  |> RArr.mapSeq  id
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let newCurves = Curve.ProjectToBrep(curves, breps, direction, tolerance)
        ResizeArray(newCurves)

    ///<summary>Rebuilds a Curve to a given degree and control point count. For more
    ///    information, see the Rhino help for the Rebuild command.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<param name="degree">(int) New degree (must be greater than 0)</param>
    ///<param name="pointCount">(int) New point count, which must be bigger than degree</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member RebuildCurve(curveId:Guid, degree:int, pointCount:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if degree<1 then  RhinoScriptingException.Raise "RebuildCurve: Degree must be greater than 0. curveId:'%s' degree:'%A' pointCount:'%A'" (Pretty.str curveId) degree pointCount
        let newCurve = curve.Rebuild(pointCount, degree, preserveTangents=false)
        if isNull newCurve then  false
        else
            State.Doc.Objects.Replace(curveId, newCurve) |> ignore<bool>
            State.Doc.Views.Redraw()
            true


    ///<summary>Deletes a knot from a Curve object.</summary>
    ///<param name="curve">(Guid) The reference of the source object</param>
    ///<param name="parameter">(float) The parameter on the Curve. Note, if the parameter is not equal to one
    ///    of the existing knots, then the knot closest to the specified parameter
    ///    will be removed</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
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
                    State.Doc.Objects.Replace(curve, nCurve)|> ignore<bool>
                    State.Doc.Views.Redraw()
                    true


    ///<summary>Reverses the direction of a Curve object. Same as Rhino's Dir command.</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve object</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member ReverseCurve(curveId:Guid) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        if curve.Reverse() then
            State.Doc.Objects.Replace(curveId, curve)|> ignore<bool>
            true
        else
            false


    ///<summary>Replace a Curve with a geometrically equivalent PolyCurve.
    ///    The PolyCurve will have the following properties:
    ///      - All the PolyCurve segments are lines, polylines, arcs, or NURBS Curves.
    ///      - The NURBS Curves segments do not have fully multiple interior knots.
    ///      - Rational NURBS Curves do not have constant weights.
    ///      - Any segment for which IsCurveLinear or IsArc is True:  a line, Polyline segment, or an arc.
    ///      - Adjacent co-linear or co-circular segments are combined.
    ///      - Segments that meet with G1-continuity have there ends tuned up so that they meet with G1-continuity to within machine precision.
    ///      - If the PolyCurve is a polyline, a Polyline will be created.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="flags">(int) Optional, default value: <c>0</c>
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
    static member SimplifyCurve(curveId:Guid, [<OPT;DEF(0)>]flags:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let mutable flagsDefault = 63
        if (flags &&& 1 )= 1  then flagsDefault <- flagsDefault &&& ( ~~~ (int CurveSimplifyOptions.SplitAtFullyMultipleKnots))
        if (flags &&& 2 )= 2  then flagsDefault <- flagsDefault &&& ( ~~~ (int CurveSimplifyOptions.RebuildLines))
        if (flags &&& 4 )= 4  then flagsDefault <- flagsDefault &&& ( ~~~ (int CurveSimplifyOptions.RebuildArcs))
        if (flags &&& 8 )= 8  then flagsDefault <- flagsDefault &&& ( ~~~ (int CurveSimplifyOptions.RebuildRationals))
        if (flags &&& 16)= 16 then flagsDefault <- flagsDefault &&& ( ~~~ (int CurveSimplifyOptions.AdjustG1))
        if (flags &&& 32)= 32 then flagsDefault <- flagsDefault &&& ( ~~~ (int CurveSimplifyOptions.Merge))
        let flags0: CurveSimplifyOptions = EnumOfValue flagsDefault
        //TODO test bitwise operations
        let tol = State.Doc.ModelAbsoluteTolerance
        let angTol = State.Doc.ModelAngleToleranceRadians
        let newCurve = curve.Simplify(flags0, tol, angTol)
        if notNull newCurve then
            State.Doc.Objects.Replace(curveId, newCurve)|> ignore<bool>
            State.Doc.Views.Redraw()
            true
        else
            false


    ///<summary>Splits, or divides, a Curve at a specified parameter. The parameter must
    ///    be in the interior of the Curve's domain.</summary>
    ///<param name="curveId">(Guid) The Curve to split</param>
    ///<param name="parameter">(float seq) One or more parameters to split the Curve at</param>
    ///<param name="deleteInput">(bool) Optional, default value: <c>true</c>
    ///    Delete the input Curve</param>
    ///<returns>(Guid ResizeArray) list of new Curves.</returns>
    static member SplitCurve(curveId:Guid, parameter:float seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let curves = RhinoScriptSyntax.SplitCurve(RhinoScriptSyntax.CoerceCurve(curveId), parameter)
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let ids = curves |> RArr.mapSeq (fun curve -> State.Doc.Objects.AddCurve(curve, rhobj.Attributes))
        if deleteInput then State.Doc.Objects.Delete(curveId, quiet=true) |> ignore<bool>
        State.Doc.Views.Redraw()
        ids

    ///<summary>Computes SplitCurve from geometry without modifying the document. Inputs are not modified.</summary>
    ///<param name="curveGeo">(Curve) The input curve geometry.</param>
    ///<param name="parameter">(float seq) One or more parameters to split the Curve at</param>
    ///<returns>(Curve ResizeArray) The resulting curves, without adding them to the document.</returns>
    static member SplitCurve(curveGeo:Curve, parameter:float seq) : Curve ResizeArray =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let curves = curve.Split(parameter)
        if isNull curves then RhinoScriptingException.Raise "SplitCurve: Unable to split curve"
        ResizeArray(curves)

    ///<summary>Trims a Curve by removing portions of the Curve outside a specified interval.</summary>
    ///<param name="curveId">(Guid) The Curve to trim</param>
    ///<param name="interval">(float * float) Two numbers identifying the interval to keep. Portions of
    ///    the Curve before domain[0] and after domain[1] will be removed. If the
    ///    input Curve is open, the interval must be increasing. If the input
    ///    Curve is closed and the interval is decreasing, then the portion of
    ///    the Curve across the start and end of the Curve is returned</param>
    ///<param name="deleteInput">(bool) Optional, default value: <c>true</c>
    ///    Delete the input Curve. If omitted the input Curve is deleted</param>
    ///<returns>(Guid) identifier of the new Curve.</returns>
    static member TrimCurve(curveId:Guid, interval:float * float, [<OPT;DEF(true)>]deleteInput:bool) : Guid  =
        let curve = RhinoScriptSyntax.TrimCurve(RhinoScriptSyntax.CoerceCurve(curveId), interval)
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)
        let rc = State.Doc.Objects.AddCurve(curve, rhobj.Attributes)
        if deleteInput then State.Doc.Objects.Delete(curveId, quiet=true) |> ignore<bool>
        State.Doc.Views.Redraw()
        rc

    ///<summary>Computes TrimCurve from geometry without modifying the document. Inputs are not modified.</summary>
    ///<param name="curveGeo">(Curve) The input curve geometry.</param>
    ///<param name="interval">(float * float) Two numbers identifying the interval to keep. Portions of
    ///    the Curve before domain[0] and after domain[1] will be removed. If the
    ///    input Curve is open, the interval must be increasing. If the input
    ///    Curve is closed and the interval is decreasing, then the portion of
    ///    the Curve across the start and end of the Curve is returned</param>
    ///<returns>(Curve) The trimmed curve, without adding it to the document.</returns>
    static member TrimCurve(curveGeo:Curve, interval:float * float) : Curve =
        let curve = RhinoScriptSyntax.DirectCurveInput(curveGeo, -1)
        let trimmed = curve.Trim(fst interval, snd interval)
        if isNull trimmed then RhinoScriptingException.Raise "TrimCurve: Unable to trim curve"
        trimmed

    ///<summary>Changes the degree of a Curve object. For more information see the Rhino help file for the ChangeDegree command.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="degree">(int) The new degree</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member ChangeCurveDegree(curveId:Guid, degree:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve curveId
        let nc = curve.ToNurbsCurve()
        if degree > 2 && degree < 12 && curve.Degree <> degree then
            if not <| nc.IncreaseDegree(degree) then false
            else
                State.Doc.Objects.Replace(curveId, nc)
        else
            false


    ///<summary>Creates Curves between two open or closed input Curves.</summary>
    ///<param name="fromCurveId">(Guid) Identifier of the first Curve object</param>
    ///<param name="toCurveId">(Guid) Identifier of the second Curve object</param>
    ///<param name="numberOfCurves">(int) Optional, default value: <c>1</c>
    ///    The number of Curves to create. The default is 1</param>
    ///<param name="method">(int) Optional, default value: <c>0</c>
    ///    The method for refining the output Curves, where:
    ///    0: (Default) Uses the control points of the Curves for matching. So the first control point of first Curve is matched to first control point of the second Curve.
    ///    1: Refits the output Curves like using the FitCurve method. Both the input Curve and the output Curve will have the same structure. The resulting Curves are usually more complex than input unless input Curves are compatible.
    ///    2: Input Curves are divided to the specified number of points on the Curve, corresponding points define new points that output Curves go through. If you are making one tween Curve, the method essentially does the following: divides the two Curves into an equal number of points, finds the midpoint between the corresponding points on the Curves, and interpolates the tween Curve through those points</param>
    ///<param name="sampleNumber">(int) Optional, default value: <c>10</c>
    ///    The number of samples points to use if method is 2. The default is 10</param>
    ///<returns>(Guid ResizeArray) The identifiers of the new tween objects.</returns>
    static member AddTweenCurves(fromCurveId:Guid, toCurveId:Guid, [<OPT;DEF(1)>]numberOfCurves:int, [<OPT;DEF(0)>]method:int, [<OPT;DEF(10)>]sampleNumber:int) : Guid ResizeArray =
        let curves = RhinoScriptSyntax.MakeTweenCurves(RhinoScriptSyntax.CoerceCurve(fromCurveId), RhinoScriptSyntax.CoerceCurve(toCurveId), numberOfCurves, method, sampleNumber)
        let ids = ResizeArray<Guid>()
        for curve in curves do
            let rc = State.Doc.Objects.AddCurve(curve)
            if rc = Guid.Empty then RhinoScriptingException.Raise "AddTweenCurves: Unable to add curve to document"
            ids.Add(rc)
        State.Doc.Views.Redraw()
        ids

    ///<summary>Computes MakeTweenCurves from geometry without modifying the document. Inputs are not modified.</summary>
    ///<param name="fromCurve">(Curve) The input geometry.</param>
    ///<param name="toCurve">(Curve) The input geometry.</param>
    ///<param name="numberOfCurves">(int) Optional, default value: <c>1</c>
    ///    The number of Curves to create. The default is 1</param>
    ///<param name="method">(int) Optional, default value: <c>0</c>
    ///    The method for refining the output Curves, where:
    ///    0: (Default) Uses the control points of the Curves for matching. So the first control point of first Curve is matched to first control point of the second Curve.
    ///    1: Refits the output Curves like using the FitCurve method. Both the input Curve and the output Curve will have the same structure. The resulting Curves are usually more complex than input unless input Curves are compatible.
    ///    2: Input Curves are divided to the specified number of points on the Curve, corresponding points define new points that output Curves go through. If you are making one tween Curve, the method essentially does the following: divides the two Curves into an equal number of points, finds the midpoint between the corresponding points on the Curves, and interpolates the tween Curve through those points</param>
    ///<param name="sampleNumber">(int) Optional, default value: <c>10</c>
    ///    The number of samples points to use if method is 2. The default is 10</param>
    ///<returns>(Curve ResizeArray) The resulting curves, without adding them to the document.</returns>
    static member MakeTweenCurves(fromCurve:Curve, toCurve:Curve, [<OPT;DEF(1)>]numberOfCurves:int, [<OPT;DEF(0)>]method:int, [<OPT;DEF(10)>]sampleNumber:int) : Curve ResizeArray =
        let curve0 = RhinoScriptSyntax.DirectCurveInput(fromCurve, -1)
        let curve1 = RhinoScriptSyntax.DirectCurveInput(toCurve, -1)
        let mutable outCurves = Array.empty
        let tolerance = State.Doc.ModelAbsoluteTolerance
        if method = 0 then
            outCurves <- Curve.CreateTweenCurves(curve0, curve1, numberOfCurves, tolerance)
            outCurves <- Curve.CreateTweenCurvesWithMatching(curve0, curve1, numberOfCurves, tolerance)
        elif method = 2 then
            outCurves <- Curve.CreateTweenCurvesWithSampling(curve0, curve1, numberOfCurves, sampleNumber, tolerance)
        else RhinoScriptingException.Raise "MakeTweenCurves Method must be 0, 1, or 2.  fromCurve:'%s' toCurve:'%s' numberOfCurves:'%A' method:'%A' sampleNumber:'%A'"  (Pretty.str fromCurve) (Pretty.str toCurve) numberOfCurves method sampleNumber
        let curves = ResizeArray<Curve>()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then curves.Add(curve)
        curves

