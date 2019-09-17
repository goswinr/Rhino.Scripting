namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
open Microsoft.FSharp.Core.LanguagePrimitives
open System.Collections.Generic
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?

[<AutoOpen>]
module ExtensionsCurve =
  type RhinoScriptSyntax with
    
    ///<summary>Adds an arc curve to the document</summary>
    ///<param name="plane">(Plane) Plane on which the arc will lie. The origin of the plane will be
    ///  the center point of the arc. x-axis of the plane defines the 0 angle
    ///  direction.</param>
    ///<param name="radius">(float) Radius of the arc</param>
    ///<param name="angleDegrees">(float) Interval of arc in degrees</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddArc(plane:Plane, radius:float, angleDegrees:float) : Guid =
        let radians = toRadians(angleDegrees)
        let arc = Arc(plane, radius, radians)
        let rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then failwithf "Unable to add arc to document.  plane:'%A' radius:'%A' angleDegrees:'%A'" plane radius angleDegrees
        Doc.Views.Redraw()
        rc
    (*
    def AddArc(plane, radius, angle_degrees):
        '''Adds an arc curve to the document
        Parameters:
          plane (str): plane on which the arc will lie. The origin of the plane will be
            the center point of the arc. x-axis of the plane defines the 0 angle
            direction.
          radius(number): radius of the arc
          angle_degrees (number): interval of arc in degrees
        Returns:
          guid: id of the new curve object
        '''
        plane = rhutil.coerceplane(plane, True)
        radians = math.radians(angle_degrees)
        arc = Rhino.Geometry.Arc(plane, radius, radians)
        rc = scriptcontext.doc.Objects.AddArc(arc)
        if rc==System.Guid.Empty: raise Exception("Unable to add arc to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a 3-point arc curve to the document</summary>
    ///<param name="start">(Point3d) Start of 'endpoints of the arc' (FIXME 0)</param>
    ///<param name="ende">(Point3d) End of 'endpoints of the arc' (FIXME 0)</param>
    ///<param name="pointOnArc">(Point3d) A point on the arc</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddArc3Pt(start:Point3d, ende:Point3d, pointOnArc:Point3d) : Guid =
        let arc = Arc(start, pointOnArc, ende)
        let rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then failwithf "Unable to add arc to document.  start:'%A' ende:'%A' pointOnArc:'%A'" start ende pointOnArc
        Doc.Views.Redraw()
        rc
    (*
    def AddArc3Pt(start, end, point_on_arc):
        '''Adds a 3-point arc curve to the document
        Parameters:
          start, end (point|guid): endpoints of the arc
          point_on_arc (point|guid): a point on the arc
        Returns:
          guid: id of the new curve object
        '''
        start = rhutil.coerce3dpoint(start, True)
        end = rhutil.coerce3dpoint(end, True)
        pton = rhutil.coerce3dpoint(point_on_arc, True)
        arc = Rhino.Geometry.Arc(start, pton, end)
        rc = scriptcontext.doc.Objects.AddArc(arc)
        if rc==System.Guid.Empty: raise Exception("Unable to add arc to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds an arc curve, created from a start point, a start direction, and an
    ///  end point, to the document</summary>
    ///<param name="start">(Point3d) The starting point of the arc</param>
    ///<param name="direction">(Vector3d) The arc direction at start</param>
    ///<param name="ende">(Point3d) The ending point of the arc</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddArcPtTanPt(start:Point3d, direction:Vector3d, ende:Point3d) : Guid =
        let arc = Arc(start, direction, ende)
        let rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then failwithf "Unable to add arc to document.  start:'%A' direction:'%A' ende:'%A'" start direction ende
        Doc.Views.Redraw()
        rc
    (*
    def AddArcPtTanPt(start, direction, end):
        '''Adds an arc curve, created from a start point, a start direction, and an
        end point, to the document
        Parameters:
          start (point): the starting point of the arc
          direction (vector): the arc direction at start
          end (point): the ending point of the arc
        Returns:
          guid: id of the new curve object
        '''
        start = rhutil.coerce3dpoint(start, True)
        direction = rhutil.coerce3dvector(direction, True)
        end = rhutil.coerce3dpoint(end, True)
        arc = Rhino.Geometry.Arc(start, direction, end)
        rc = scriptcontext.doc.Objects.AddArc(arc)
        if rc==System.Guid.Empty: raise Exception("Unable to add arc to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Makes a curve blend between two curves</summary>
    ///<param name="curves">(Guid * Guid) List of two curves</param>
    ///<param name="parameters">(float * float) List of two curve parameters defining the blend end points</param>
    ///<param name="reverses">(bool * bool) List of two boolean values specifying to use the natural or opposite direction of the curve</param>
    ///<param name="continuities">(int * int) List of two numbers specifying continuity at end points
    ///  0 = position
    ///  1 = tangency
    ///  2 = curvature</param>
    ///<returns>(Guid) identifier of new curve on success</returns>
    static member AddBlendCurve(curves:Guid * Guid, parameters:float * float, reverses:bool * bool, continuities:int * int) : Guid =
        let crv0 = RhinoScriptSyntax.CoerceCurve (fst curves)  
        let crv1 = RhinoScriptSyntax.CoerceCurve (snd curves)  
        let c0:BlendContinuity = EnumOfValue(fst continuities)
        let c1:BlendContinuity = EnumOfValue(snd continuities)
        let curve = Curve.CreateBlendCurve(crv0, fst parameters, fst reverses, c0, crv1, snd parameters, snd reverses, c1)
        let rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then  failwithf "Unable to add curve to document.  curves:'%A' parameters:'%A' reverses:'%A' continuities:'%A'" curves parameters reverses continuities
        Doc.Views.Redraw()
        rc
    (*
    def AddBlendCurve(curves, parameters, reverses, continuities):
        '''Makes a curve blend between two curves
        Parameters:
          curves ([guid|curve, guid|curve]): list of two curves
          parameters ([number, number]): list of two curve parameters defining the blend end points
          reverses ([bool, bool]): list of two boolean values specifying to use the natural or opposite direction of the curve
          continuities ([number, number]): list of two numbers specifying continuity at end points
                                                0 = position
                                                1 = tangency
                                                2 = curvature
        Returns:
          guid: identifier of new curve on success
        '''
        crv0 = rhutil.coercecurve(curves[0], -1, True)
        crv1 = rhutil.coercecurve(curves[1], -1, True)
        c0 = System.Enum.ToObject(Rhino.Geometry.BlendContinuity, continuities[0])
        c1 = System.Enum.ToObject(Rhino.Geometry.BlendContinuity, continuities[1])
        curve = Rhino.Geometry.Curve.CreateBlendCurve(crv0, parameters[0], reverses[0], c0, crv1, parameters[1], reverses[1], c1)
        rc = scriptcontext.doc.Objects.AddCurve(curve)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a circle curve to the document</summary>
    ///<param name="plane">(Plane) Plane on which the circle will lie. If a point is
    ///  passed, this will be the center of the circle on the active construction plane</param>
    ///<param name="radius">(float) The radius of the circle</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddCircle(plane:Plane, radius:float) : Guid = //(TODO add overload point)
        let circle = Circle(plane, radius)
        let rc = Doc.Objects.AddCircle(circle)
        if rc = Guid.Empty then failwithf "Unable to add circle to document.  planeOrCenter:'%A' radius:'%A'" plane radius
        Doc.Views.Redraw()
        rc
    (*
    def AddCircle(plane_or_center, radius):
        '''Adds a circle curve to the document
        Parameters:
          plane_or_center (point|plane): plane on which the circle will lie. If a point is
            passed, this will be the center of the circle on the active
            construction plane
          radius (number): the radius of the circle
        Returns:
          guid: id of the new curve object
        '''
        rc = None
        plane = rhutil.coerceplane(plane_or_center, False)
        if plane:
            circle = Rhino.Geometry.Circle(plane, radius)
            rc = scriptcontext.doc.Objects.AddCircle(circle)
        else:
            center = rhutil.coerce3dpoint(plane_or_center, True)
            view = scriptcontext.doc.Views.ActiveView
            plane = view.ActiveViewport.ConstructionPlane()
            plane.Origin = center
            circle = Rhino.Geometry.Circle(plane, radius)
            rc = scriptcontext.doc.Objects.AddCircle(circle)
        if rc==System.Guid.Empty: raise Exception("Unable to add circle to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a 3-point circle curve to the document</summary>
    ///<param name="first">(Point3d) First of 'points on the circle' (FIXME 0)</param>
    ///<param name="second">(Point3d) Second of 'points on the circle' (FIXME 0)</param>
    ///<param name="third">(Point3d) Third of 'points on the circle' (FIXME 0)</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddCircle3Pt(first:Point3d, second:Point3d, third:Point3d) : Guid =
        let circle = Circle(first, second, third)
        let rc = Doc.Objects.AddCircle(circle)
        if rc = Guid.Empty then failwithf "Unable to add circle to document.  first:'%A' second:'%A' third:'%A'" first second third
        Doc.Views.Redraw()
        rc
    (*
    def AddCircle3Pt(first, second, third):
        '''Adds a 3-point circle curve to the document
        Parameters:
          first, second, third (point|guid): points on the circle
        Returns:
          guid: id of the new curve object
        '''
        start = rhutil.coerce3dpoint(first, True)
        end = rhutil.coerce3dpoint(second, True)
        third = rhutil.coerce3dpoint(third, True)
        circle = Rhino.Geometry.Circle(start, end, third)
        rc = scriptcontext.doc.Objects.AddCircle(circle)
        if rc==System.Guid.Empty: raise Exception("Unable to add circle to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a control points curve object to the document</summary>
    ///<param name="points">(Point3d seq) A list of points</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///Degree of the curve</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddCurve(points:Point3d seq, [<OPT;DEF(3)>]degree:int) : Guid =
        let  curve = Curve.CreateControlPointCurve(points, degree)
        if isNull curve then failwithf "Unable to create control point curve from given points.  points:'%A' degree:'%A'" points degree
        let  rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then failwithf "Unable to add curve to document.  points:'%A' degree:'%A'" points degree
        Doc.Views.Redraw()
        rc
    (*
    def AddCurve(points, degree=3):
        '''Adds a control points curve object to the document
        Parameters:
          points ([point|guid, ...]) a list of points
          degree (number): degree of the curve
        Returns:
          guid: id of the new curve object
        '''
        points = rhutil.coerce3dpointlist(points, True)
        curve = Rhino.Geometry.Curve.CreateControlPointCurve(points, degree)
        if not curve: raise Exception("unable to create control point curve from given points")
        rc = scriptcontext.doc.Objects.AddCurve(curve)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds an elliptical curve to the document</summary>
    ///<param name="plane">(Plane) The plane on which the ellipse will lie. The origin of
    ///  the plane will be the center of the ellipse</param>
    ///<param name="radiusX">(float) RadiusX of 'radius in the X and Y axis directions' (FIXME 0)</param>
    ///<param name="radiusY">(float) RadiusY of 'radius in the X and Y axis directions' (FIXME 0)</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddEllipse(plane:Plane, radiusX:float, radiusY:float) : Guid =
        let ellipse = Ellipse(plane, radiusX, radiusY)
        let rc = Doc.Objects.AddEllipse(ellipse)
        if rc = Guid.Empty then failwithf "Unable to add curve to document. plane:'%A' radiusX:'%A' radiusY:'%A'" plane radiusX radiusY
        Doc.Views.Redraw()
        rc
    (*
    def AddEllipse(plane, radiusX, radiusY):
        '''Adds an elliptical curve to the document
        Parameters:
          plane (plane) the plane on which the ellipse will lie. The origin of
                  the plane will be the center of the ellipse
          radiusX, radiusY (number): radius in the X and Y axis directions
        Returns:
          guid: id of the new curve object if successful
        '''
        plane = rhutil.coerceplane(plane, True)
        ellipse = Rhino.Geometry.Ellipse(plane, radiusX, radiusY)
        rc = scriptcontext.doc.Objects.AddEllipse(ellipse)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a 3-point elliptical curve to the document</summary>
    ///<param name="center">(Point3d) Center point of the ellipse</param>
    ///<param name="second">(Point3d) End point of the x axis</param>
    ///<param name="third">(Point3d) End point of the y axis</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddEllipse3Pt(center:Point3d, second:Point3d, third:Point3d) : Guid =
        let  ellipse = Ellipse(center, second, third)
        let  rc = Doc.Objects.AddEllipse(ellipse)
        if rc = Guid.Empty then failwithf "Unable to add curve to document.  center:'%A' second:'%A' third:'%A'" center second third
        Doc.Views.Redraw()
        rc
    (*
    def AddEllipse3Pt(center, second, third):
        '''Adds a 3-point elliptical curve to the document
        Parameters:
          center (point|guid): center point of the ellipse
          second (point|guid): end point of the x axis
          third  (point|guid): end point of the y axis
        Returns:
          guid: id of the new curve object if successful
        '''
        center = rhutil.coerce3dpoint(center, True)
        second = rhutil.coerce3dpoint(second, True)
        third = rhutil.coerce3dpoint(third, True)
        ellipse = Rhino.Geometry.Ellipse(center, second, third)
        rc = scriptcontext.doc.Objects.AddEllipse(ellipse)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a fillet curve between two curve objects</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object</param>
    ///<param name="radius">(float) Optional, Default Value: <c>1.0</c>
    ///Fillet radius</param>
    ///<param name="basisPointA">(Point3d) Optional, Default Value: <c>null</c>
    ///Base point of the first curve. If omitted,
    ///  starting point of the curve is used</param>
    ///<param name="basisPointB">(Point3d) Optional, Default Value: <c>null</c>
    ///Base point of the second curve. If omitted,
    ///  starting point of the curve is used</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddFilletCurve(curveA:Guid, curveB:Guid, [<OPT;DEF(1.0)>]radius:float, [<OPT;DEF(null)>]basePointA:Point3d, [<OPT;DEF(null)>]basePointB:Point3d) : Guid =
        let basePointA = RhinoScriptSyntax.Coerce3dPoint(basePointA, nullIsUnset=true)  
        let basePointB = RhinoScriptSyntax.Coerce3dPoint(basePointB, nullIsUnset=true)    
        let  curve0 = RhinoScriptSyntax.CoerceCurve (curveA)  
        let  curve1 = RhinoScriptSyntax.CoerceCurve (curveB)  
        let mutable crv0T = 0.0
        if basePointA=Point3d.Unset then
            crv0T <- curve0.Domain.Min
        else
            let rc, t = curve0.ClosestPoint(basePointA)
            if not <| rc then failwithf "ClosestPoint failed.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
            crv0T <- t
        let mutable crv1T = 0.0
        if basePointB=Point3d.Unset then
            crv1T <- curve1.Domain.Min
        else
            let rc, t = curve1.ClosestPoint(basePointB)
            if not <| rc then failwithf "ClosestPoint failed.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
            crv1T <- t
        let mutable arc = Curve.CreateFillet(curve0, curve1, radius, crv0T, crv1T)
        let mutable rc = Doc.Objects.AddArc(arc)
        if rc = Guid.Empty then failwithf "Unable to add fillet curve to document.  curveA:'%A' curveB:'%A' radius:'%A' basePointA:'%A' basePointB:'%A'" curveA curveB radius basePointA basePointB
        Doc.Views.Redraw()
        rc
    (*
    def AddFilletCurve(curve0id, curve1id, radius=1.0, base_point0=None, base_point1=None):
        '''Adds a fillet curve between two curve objects
        Parameters:
          curve0id (guid): identifier of the first curve object
          curve1id (guid): identifier of the second curve object
          radius (number, optional): fillet radius
          base_point0 (point|guid, optional): base point of the first curve. If omitted,
                              starting point of the curve is used
          base_point1 (point|guid, optional): base point of the second curve. If omitted,
                              starting point of the curve is used
        Returns:
          guid: id of the new curve object if successful
        '''
        if base_point0: base_point0 = rhutil.coerce3dpoint(base_point0, True)
        else: base_point0 = Rhino.Geometry.Point3d.Unset
        if base_point1: base_point1 = rhutil.coerce3dpoint(base_point1, True)
        else: base_point1 = Rhino.Geometry.Point3d.Unset
        curve0 = rhutil.coercecurve(curve0id, -1, True)
        curve1 = rhutil.coercecurve(curve1id, -1, True)
        crv0_t = 0.0
        if base_point0==Rhino.Geometry.Point3d.Unset:
            crv0_t = curve0.Domain.Min
        else:
            rc, t = curve0.ClosestPoint(base_point0, 0.0)
            if not rc: raise Exception("ClosestPoint failed")
            crv0_t = t
        crv1_t = 0.0
        if base_point1==Rhino.Geometry.Point3d.Unset:
            crv1_t = curve1.Domain.Min
        else:
            rc, t = curve1.ClosestPoint(base_point1, 0.0)
            if not rc: raise Exception("ClosestPoint failed")
            crv1_t = t
        arc = Rhino.Geometry.Curve.CreateFillet(curve0, curve1, radius, crv0_t, crv1_t)
        rc = scriptcontext.doc.Objects.AddArc(arc)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds an interpolated curve object that lies on a specified
    ///  surface.  Note, this function will not create periodic curves,
    ///  but it will create closed curves.</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to create the curve on</param>
    ///<param name="points">(Point3d seq) List of 3D points that lie on the specified surface.
    ///  The list must contain at least 2 points</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddInterpCrvOnSrf(surfaceId:Guid, points:Point3d seq) : Guid =
        let  surface = RhinoScriptSyntax.CoerceSurface(surfaceId)  
        let  tolerance = Doc.ModelAbsoluteTolerance
        let  curve = surface.InterpolatedCurveOnSurface(points, tolerance)
        if isNull curve then failwithf "Unable to create InterpolatedCurveOnSurface.  surfaceId:'%A' points:'%A'" surfaceId points
        let mutable rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then failwithf "Unable to add curve to document.  surfaceId:'%A' points:'%A'" surfaceId points
        Doc.Views.Redraw()
        rc
    (*
    def AddInterpCrvOnSrf(surface_id, points):
        '''Adds an interpolated curve object that lies on a specified
        surface.  Note, this function will not create periodic curves,
        but it will create closed curves.
        Parameters:
          surface_id (guid): identifier of the surface to create the curve on
          points ([point|guid, point|guid, ...])list of 3D points that lie on the specified surface.
                   The list must contain at least 2 points
        Returns:
          guid: id of the new curve object if successful
        '''
        surface = rhutil.coercesurface(surface_id, True)
        points = rhutil.coerce3dpointlist(points, True)
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        curve = surface.InterpolatedCurveOnSurface(points, tolerance)
        if not curve: raise Exception("unable to create InterpolatedCurveOnSurface")
        rc = scriptcontext.doc.Objects.AddCurve(curve)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds an interpolated curve object based on surface parameters,
    ///  that lies on a specified surface. Note, this function will not
    ///  create periodic curves, but it will create closed curves.</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to create the curve on</param>
    ///<param name="points">(Point2d seq) A list of 2D surface parameters. The list must contain
    ///  at least 2 sets of parameters</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddInterpCrvOnSrfUV(surfaceId:Guid, points:Point2d seq) : Guid =
        let mutable surface = RhinoScriptSyntax.CoerceSurface(surfaceId)  
        let mutable tolerance = Doc.ModelAbsoluteTolerance
        let mutable curve = surface.InterpolatedCurveOnSurfaceUV(points, tolerance)
        if isNull curve then failwithf "Unable to create InterpolatedCurveOnSurfaceUV.  surfaceId:'%A' points:'%A'" surfaceId points
        let mutable rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then failwithf "Unable to add curve to document.  surfaceId:'%A' points:'%A'" surfaceId points
        Doc.Views.Redraw()
        rc
    (*
    def AddInterpCrvOnSrfUV(surface_id, points):
        '''Adds an interpolated curve object based on surface parameters,
        that lies on a specified surface. Note, this function will not
        create periodic curves, but it will create closed curves.
        Parameters:
          surface_id (guid): identifier of the surface to create the curve on
          points ([[number, number], [number,number], ...]): a list of 2D surface parameters. The list must contain
                                                             at least 2 sets of parameters
        Returns:
          guid: id of the new curve object if successful
        '''
        surface = rhutil.coercesurface(surface_id, True)
        points = rhutil.coerce2dpointlist(points)
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        curve = surface.InterpolatedCurveOnSurfaceUV(points, tolerance)
        if not curve: raise Exception("unable to create InterpolatedCurveOnSurfaceUV")
        rc = scriptcontext.doc.Objects.AddCurve(curve)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds an interpolated curve object to the document. Options exist to make
    ///  a periodic curve or to specify the tangent at the endpoints. The resulting
    ///  curve is a non-rational NURBS curve of the specified degree.</summary>
    ///<param name="points">(Point3d seq) A list containing 3D points to interpolate. For periodic curves,
    ///  if the final point is a duplicate of the initial point, it is
    ///  ignored. The number of control points must be >= (degree+1).</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///The degree of the curve (must be >=1).
    ///  Periodic curves must have a degree >= 2. For knotstyle = 1 or 2,
    ///  the degree must be 3. For knotstyle = 4 or 5, the degree must be odd</param>
    ///<param name="knotstyle">(int) Optional, Default Value: <c>0</c>
    ///0 Uniform knots.  Parameter spacing between consecutive knots is 1.0.
    ///  1 Chord length spacing.  Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///  2 Sqrt (chord length).  Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///  3 Periodic with uniform spacing.
    ///  4 Periodic with chord length spacing.  Requires an odd degree value.
    ///  5 Periodic with sqrt (chord length) spacing.  Requires an odd degree value.</param>
    ///<param name="startTangent">(Vector3d) Optional, Default Value: <c>null</c>
    ///A vector that specifies a tangency condition at the
    ///  beginning of the curve. If the curve is periodic, this argument must be omitted.</param>
    ///<param name="endTangent">(Vector3d) Optional, Default Value: <c>null</c>
    ///3d vector that specifies a tangency condition at the
    ///  end of the curve. If the curve is periodic, this argument must be omitted.</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddInterpCurve(points:Point3d seq, [<OPT;DEF(3)>]degree:int, [<OPT;DEF(0)>]knotstyle:int, [<OPT;DEF(null)>]startTangent:Vector3d, [<OPT;DEF(null)>]endTangent:Vector3d) : Guid =
        let endTangent   = RhinoScriptSyntax.Coerce3dVector(endTangent, nullIsUnset=true)
        let startTangent = RhinoScriptSyntax.Coerce3dVector(startTangent, nullIsUnset=true)
        let knotstyle : CurveKnotStyle = EnumOfValue knotstyle
        let  curve = Curve.CreateInterpolatedCurve(points, degree, knotstyle, startTangent, endTangent)
        if isNull curve then failwithf "Unable to CreateInterpolatedCurve.  points:'%A' degree:'%A' knotstyle:'%A' startTangent:'%A' endTangent:'%A'" points degree knotstyle startTangent endTangent
        let  rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then failwithf "Unable to add curve to document.  points:'%A' degree:'%A' knotstyle:'%A' startTangent:'%A' endeTangent:'%A'" points degree knotstyle startTangent endTangent
        Doc.Views.Redraw()
        rc
    (*
    def AddInterpCurve(points, degree=3, knotstyle=0, start_tangent=None, end_tangent=None):
        '''Adds an interpolated curve object to the document. Options exist to make
        a periodic curve or to specify the tangent at the endpoints. The resulting
        curve is a non-rational NURBS curve of the specified degree.
        Parameters:
          points (point|guid, point|guid, ...]): a list containing 3D points to interpolate. For periodic curves,
              if the final point is a duplicate of the initial point, it is
              ignored. The number of control points must be >= (degree+1).
          degree (number, optional): The degree of the curve (must be >=1).
              Periodic curves must have a degree >= 2. For knotstyle = 1 or 2,
              the degree must be 3. For knotstyle = 4 or 5, the degree must be odd
          knotstyle(int,optional):
              0 Uniform knots.  Parameter spacing between consecutive knots is 1.0.
              1 Chord length spacing.  Requires degree = 3 with arrCV1 and arrCVn1 specified.
              2 Sqrt (chord length).  Requires degree = 3 with arrCV1 and arrCVn1 specified.
              3 Periodic with uniform spacing.
              4 Periodic with chord length spacing.  Requires an odd degree value.
              5 Periodic with sqrt (chord length) spacing.  Requires an odd degree value.
          start_tangent (vector, optional): a vector that specifies a tangency condition at the
              beginning of the curve. If the curve is periodic, this argument must be omitted.
          end_tangent (vector, optional): 3d vector that specifies a tangency condition at the
              end of the curve. If the curve is periodic, this argument must be omitted.
        Returns:
          guid: id of the new curve object if successful
        '''
        points = rhutil.coerce3dpointlist(points, True)
        if not start_tangent: start_tangent = Rhino.Geometry.Vector3d.Unset
        start_tangent = rhutil.coerce3dvector(start_tangent, True)
        if not end_tangent: end_tangent = Rhino.Geometry.Vector3d.Unset
        end_tangent = rhutil.coerce3dvector(end_tangent, True)
        knotstyle = System.Enum.ToObject(Rhino.Geometry.CurveKnotStyle, knotstyle)
        curve = Rhino.Geometry.Curve.CreateInterpolatedCurve(points, degree, knotstyle, start_tangent, end_tangent)
        if not curve: raise Exception("unable to CreateInterpolatedCurve")
        rc = scriptcontext.doc.Objects.AddCurve(curve)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a line curve to the current model.</summary>
    ///<param name="start">(Point3d) Start of 'end points of the line' (FIXME 0)</param>
    ///<param name="ende">(Point3d) End of 'end points of the line' (FIXME 0)</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddLine(start:Point3d, ende:Point3d) : Guid =
        let  rc = Doc.Objects.AddLine(start, ende)
        if rc = Guid.Empty then failwithf "Unable to add line to document.  start:'%A' ende:'%A'" start ende
        Doc.Views.Redraw()
        rc
    (*
    def AddLine(start, end):
        '''Adds a line curve to the current model.
        Parameters:
          start, end (point|guid) end points of the line
        Returns:
          guid: id of the new curve object
        '''
        start = rhutil.coerce3dpoint(start, True)
        end = rhutil.coerce3dpoint(end, True)
        rc = scriptcontext.doc.Objects.AddLine(start, end)
        if rc==System.Guid.Empty: raise Exception("Unable to add line to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a NURBS curve object to the document</summary>
    ///<param name="points">(Point3d IList) A list containing 3D control points</param>
    ///<param name="knots">(float IList) Knot values for the curve. The number of elements in knots must
    ///  equal the number of elements in points plus degree minus 1</param>
    ///<param name="degree">(int) Degree of the curve. must be greater than of equal to 1</param>
    ///<param name="weights">(float IList) Optional, Default Value: <c>null</c>
    ///Weight values for the curve. Number of elements should
    ///  equal the number of elements in points. Values must be greater than 0</param>
    ///<returns>(Guid) the identifier of the new object , otherwise None</returns>
    static member AddNurbsCurve(points:Point3d IList, knots:float IList, degree:int, [<OPT;DEF(null)>]weights:float IList) : Guid =
        let cvcount = Seq.length(points)
        let knotcount = cvcount + degree - 1
        let noweights= (Seq.isEmpty weights)
        if Seq.length(knots)<>knotcount then
            failwithf "Number of elements in knots must equal the number of elements in points plus degree minus 1.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        
        
        let rational =
          if notNull weights then  
              if Seq.length(weights)<>cvcount then
                failwithf "Number of elements in weights should equal the number of elements in points.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
              true
          else false
        
       
        let nc = new NurbsCurve(3,rational,degree+1,cvcount)

        for i = 0 to cvcount-1 do
          if rational then
              nc.Points.SetPoint(i, points.[i], weights.[i]) |> ignore
          else
              nc.Points.SetPoint(i, points.[i]) |> ignore

        for i =0 to knotcount-1 do
            nc.Knots.[i] <- knots.[i]
        let rc = Doc.Objects.AddCurve(nc)
        if rc = Guid.Empty then failwithf "Unable to add curve to document.  points:'%A' knots:'%A' degree:'%A' weights:'%A'" points knots degree weights
        Doc.Views.Redraw()
        rc
    (*
    def AddNurbsCurve(points, knots, degree, weights=None):
        '''Adds a NURBS curve object to the document
        Parameters:
          points ([guid|point, guid|point, ...]): a list containing 3D control points
          knots ([number, number, ...]): Knot values for the curve. The number of elements in knots must
              equal the number of elements in points plus degree minus 1
          degree (number): degree of the curve. must be greater than of equal to 1
          weights([number, number, ...], optional) weight values for the curve. Number of elements should
              equal the number of elements in points. Values must be greater than 0
        Returns:
          guid: the identifier of the new object if successful, otherwise None
        '''
        points = rhutil.coerce3dpointlist(points, True)
        cvcount = len(points)
        knotcount = cvcount + degree - 1
        if len(knots)!=knotcount:
            raise Exception("Number of elements in knots must equal the number of elements in points plus degree minus 1")
        if weights and len(weights)!=cvcount:
            raise Exception("Number of elements in weights should equal the number of elements in points")
        rational = (weights!=None)
        
        nc = Rhino.Geometry.NurbsCurve(3,rational,degree+1,cvcount)
        if rational: 
            for i in xrange(cvcount):
                nc.Points.SetPoint(i, points[i], weights[i])
        else:
            for i in xrange(cvcount):
                nc.Points.SetPoint(i, points[i])
        for i in xrange(knotcount): nc.Knots[i] = knots[i]
        rc = scriptcontext.doc.Objects.AddCurve(nc)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a polyline curve to the current model</summary>
    ///<param name="points">(Point3d seq) List of 3D points. Duplicate, consecutive points will be
    ///  removed. The list must contain at least two points. If the
    ///  list contains less than four points, then the first point and
    ///  last point must be different.</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddPolyline(points:Point3d seq) : Guid =
        let pl = Polyline(points)
        //pl.DeleteShortSegments(Doc.ModelAbsoluteTolerance) |>ignore
        let rc = Doc.Objects.AddPolyline(pl)
        if rc = Guid.Empty then failwithf "Unable to add polyline to document.  points:'%A' " points
        Doc.Views.Redraw()
        rc
    (*
    def AddPolyline(points, replace_id=None):
        '''Adds a polyline curve to the current model
        Parameters:
          points ([guid|point, guid|point, ...]): list of 3D points. Duplicate, consecutive points will be
                   removed. The list must contain at least two points. If the
                   list contains less than four points, then the first point and
                   last point must be different.
          replace_id (guid, optional): If set to the id of an existing object, the object
                   will be replaced by this polyline
        Returns:
          guid: id of the new curve object if successful
        '''
        points = rhutil.coerce3dpointlist(points, True)
        if replace_id: replace_id = rhutil.coerceguid(replace_id, True)
        rc = System.Guid.Empty
        pl = Rhino.Geometry.Polyline(points)
        pl.DeleteShortSegments(scriptcontext.doc.ModelAbsoluteTolerance)
        if replace_id:
            if scriptcontext.doc.Objects.Replace(replace_id, pl):
                rc = replace_id
        else:
            rc = scriptcontext.doc.Objects.AddPolyline(pl)
        if rc==System.Guid.Empty: raise Exception("Unable to add polyline to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Add a rectangular curve to the document</summary>
    ///<param name="plane">(Plane) Plane on which the rectangle will lie</param>
    ///<param name="width">(float) Width of rectangle as measured along the plane's
    ///  x and y axes</param>
    ///<param name="height">(float) Height of rectangle as measured along the plane's
    ///  x and y axes</param>
    ///<returns>(Guid) id of new rectangle</returns>
    static member AddRectangle(plane:Plane, width:float, height:float) : Guid =
        let rect = Rectangle3d(plane, width, height)
        let poly = rect.ToPolyline()
        let rc = Doc.Objects.AddPolyline(poly)
        if rc = Guid.Empty then failwithf "Unable to add polyline to document.  plane:'%A' width:'%A' height:'%A'" plane width height
        Doc.Views.Redraw()
        rc
    (*
    def AddRectangle(plane, width, height):
        '''Add a rectangular curve to the document
        Parameters:
          plane (plane) plane on which the rectangle will lie
          width, height (number): width and height of rectangle as measured along the plane's
            x and y axes
        Returns:
          guid: id of new rectangle
        '''
        plane = rhutil.coerceplane(plane, True)
        rect = Rhino.Geometry.Rectangle3d(plane, width, height)
        poly = rect.ToPolyline()
        rc = scriptcontext.doc.Objects.AddPolyline(poly)
        if rc==System.Guid.Empty: raise Exception("Unable to add polyline to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Adds a spiral or helical curve to the document</summary>
    ///<param name="point0">(Point3d) Helix axis start point or center of spiral</param>
    ///<param name="point1">(Point3d) Helix axis end point or point normal on spiral plane</param>
    ///<param name="pitch">(float) Distance between turns. If 0, then a spiral. If > 0 then the
    ///  distance between helix "threads"</param>
    ///<param name="turns">(float) Number of turns</param>
    ///<param name="radius0">(float) Starting radius of spiral</param>
    ///<param name="radius1">(float) Optional, Default Value: <c>0.0</c>
    ///Ending radius of spiral. If omitted, the starting radius is used for the complete spiral.</param>
    ///<returns>(Guid) id of new curve on success</returns>
    static member AddSpiral(point0:Point3d, point1:Point3d, pitch:float, turns:float, radius0:float, [<OPT;DEF(0.0)>]radius1:float) : Guid =
        let dir = point1 - point0
        let plane = Plane(point0, dir)
        let point2 = point0 + plane.XAxis
        let r2 = if radius1=0.0 then radius0 else radius1
        let curve = NurbsCurve.CreateSpiral(point0, dir, point2, pitch, turns, radius0, r2)
        if isNull curve then failwithf "Unable to add curve to document.  point0:'%A' point1:'%A' pitch:'%A' turns:'%A' radius0:'%A' radius1:'%A'" point0 point1 pitch turns radius0 radius1
        let rc = Doc.Objects.AddCurve(curve)
        if rc = Guid.Empty then failwithf "Unable to add curve to document.  point0:'%A' point1:'%A' pitch:'%A' turns:'%A' radius0:'%A' radius1:'%A'" point0 point1 pitch turns radius0 radius1
        Doc.Views.Redraw()
        rc
    (*
    def AddSpiral(point0, point1, pitch, turns, radius0, radius1=None):
        '''Adds a spiral or helical curve to the document
        Parameters:
          point0 (point|guid): helix axis start point or center of spiral
          point1 (point|guid): helix axis end point or point normal on spiral plane
          pitch (number): distance between turns. If 0, then a spiral. If > 0 then the
                  distance between helix "threads"
          turns (number): number of turns
          radius0 (number): starting radius of spiral
          radius1 (number, optional): ending radius of spiral. If omitted, the starting radius is used for the complete spiral.
        Returns:
          guid: id of new curve on success
        '''
        if radius1 is None: radius1 = radius0
        point0 = rhutil.coerce3dpoint(point0, True)
        point1 = rhutil.coerce3dpoint(point1, True)
        dir = point1 - point0
        plane = Rhino.Geometry.Plane(point0, dir)
        point2 = point0 + plane.XAxis
        curve = Rhino.Geometry.NurbsCurve.CreateSpiral(point0, dir, point2, pitch, turns, radius0, radius1)
        rc = scriptcontext.doc.Objects.AddCurve(curve)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Add a curve object based on a portion, or interval of an existing curve
    ///  object. Similar in operation to Rhino's SubCrv command</summary>
    ///<param name="curveId">(Guid) Identifier of a closed planar curve object</param>
    ///<param name="param0">(float) First parameters on the source curve</param>
    ///<param name="param1">(float) Second parameters on the source curve</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddSubCrv(curveId:Guid, param0:float, param1:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId)  
        let trimcurve = curve.Trim(param0, param1)
        if isNull trimcurve then failwithf "Unable to trim curve.  curveId:'%A' param0:'%A' param1:'%A'" curveId param0 param1
        let rc = Doc.Objects.AddCurve(trimcurve)
        if rc = Guid.Empty then failwithf "Unable to add curve to document.  curveId:'%A' param0:'%A' param1:'%A'" curveId param0 param1
        Doc.Views.Redraw()
        rc
    (*
    def AddSubCrv(curve_id, param0, param1):
        '''Add a curve object based on a portion, or interval of an existing curve
        object. Similar in operation to Rhino's SubCrv command
        Parameters:
          curve_id (guid): identifier of a closed planar curve object
          param0, param1 (number): first and second parameters on the source curve
        Returns:
          guid: id of the new curve object if successful
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        trimcurve = curve.Trim(param0, param1)
        if not trimcurve: raise Exception("unable to trim curve")
        rc = scriptcontext.doc.Objects.AddCurve(trimcurve)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the angle of an arc curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///Identifies the curve segment if curveId identifies a polycurve</param>
    ///<returns>(float) The angle in degrees .</returns>
    static member ArcAngle(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then failwithf "Curve is not arc.  curveId:'%A' segmentIndex:'%A' curveId:'%A'" curveId segmentIndex curveId
        (!arc).AngleDegrees
    (*
    def ArcAngle(curve_id, segment_index=-1):
        '''Returns the angle of an arc curve object.
        Parameters:
          curve_id (guid): identifier of a curve object
          segment_index (number, optional): identifies the curve segment if curve_id identifies a polycurve
        Returns:
          number: The angle in degrees if successful.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc, arc = curve.TryGetArc( Rhino.RhinoMath.ZeroTolerance )
        if not rc: raise Exception("curve is not arc")
        return arc.AngleDegrees
    *)


    ///<summary>Returns the center point of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D center point of the arc .</returns>
    static member ArcCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then failwithf "Curve is not arc.  curveId:'%A' segmentIndex:'%A' curveId:'%A'" curveId segmentIndex curveId
        (!arc).Center
    (*
    def ArcCenterPoint(curve_id, segment_index=-1):
        '''Returns the center point of an arc curve object
        Parameters:
          curve_id (guid): identifier of a curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          point: The 3D center point of the arc if successful.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc, arc = curve.TryGetArc( Rhino.RhinoMath.ZeroTolerance )
        if not rc: raise Exception("curve is not arc")
        return arc.Center
    *)


    ///<summary>Returns the mid point of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D mid point of the arc .</returns>
    static member ArcMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then failwithf "Curve is not arc.  curveId:'%A' segmentIndex:'%A' curveId:'%A'" curveId segmentIndex curveId
        (!arc).MidPoint
    (*
    def ArcMidPoint(curve_id, segment_index=-1):
        '''Returns the mid point of an arc curve object
        Parameters:
          curve_id (guid): identifier of a curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          point: The 3D mid point of the arc if successful.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc, arc = curve.TryGetArc( Rhino.RhinoMath.ZeroTolerance )
        if not rc: raise Exception("curve is not arc")
        return arc.MidPoint
    *)


    ///<summary>Returns the radius of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The radius of the arc .</returns>
    static member ArcRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let arc = ref Arc.Unset
        let rc = curve.TryGetArc( arc, RhinoMath.ZeroTolerance )
        if not <| rc then failwithf "Curve is not arc.  curveId:'%A' segmentIndex:'%A' curveId:'%A'" curveId segmentIndex curveId
        (!arc).Radius
    (*
    def ArcRadius(curve_id, segment_index=-1):
        '''Returns the radius of an arc curve object
        Parameters:
          curve_id (guid): identifier of a curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          number: The radius of the arc if successful.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc, arc = curve.TryGetArc( Rhino.RhinoMath.ZeroTolerance )
        if not rc: raise Exception("curve is not arc")
        return arc.Radius
    *)


    //(FIXME) VarOutTypes
    ///<summary>Returns the center point of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="returnPlane">(bool) Optional, Default Value: <c>false</c>
    ///If True, the circle's plane is returned. If omitted the plane is not returned.</param>
    ///<returns>(Point3d) The 3D center point of the circle .</returns>
    static member CircleCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int, [<OPT;DEF(false)>]returnPlane:bool) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then failwithf "Curve is not circle.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        (!circle).Center
    (*
    def CircleCenterPoint(curve_id, segment_index=-1, return_plane=False):
        '''Returns the center point of a circle curve object
        Parameters:
          curve_id (guid): identifier of a curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
          return_plane (bool, optional): if True, the circle's plane is returned. If omitted the plane is not returned.
        Returns:
          point: The 3D center point of the circle if successful.
          plane: The plane of the circle if return_plane is True
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc, circle = curve.TryGetCircle(Rhino.RhinoMath.ZeroTolerance)
        if not rc: raise Exception("curve is not circle")
        if return_plane: return circle.Plane
        return circle.Center
    *)


    ///<summary>Returns the circumference of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The circumference of the circle .</returns>
    static member CircleCircumference(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then failwithf "Curve is not circle.  curveId:'%A' segmentIndex:'%A' " curveId segmentIndex
        (!circle).Circumference
    (*
    def CircleCircumference(curve_id, segment_index=-1):
        '''Returns the circumference of a circle curve object
        Parameters:
          curve_id (guid): identifier of a curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          number: The circumference of the circle if successful.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc, circle = curve.TryGetCircle( Rhino.RhinoMath.ZeroTolerance )
        if not rc: raise Exception("curve is not circle")
        return circle.Circumference
    *)


    ///<summary>Returns the radius of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The radius of the circle .</returns>
    static member CircleRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let circle = ref Circle.Unset
        let rc = curve.TryGetCircle(circle, RhinoMath.ZeroTolerance )
        if not <| rc then failwithf "Curve is not circle.  curveId:'%A' segmentIndex:'%A' " curveId segmentIndex
        (!circle).Radius
    (*
    def CircleRadius(curve_id, segment_index=-1):
        '''Returns the radius of a circle curve object
        Parameters:
          curve_id (guid): identifier of a curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          number: The radius of the circle if successful.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc, circle = curve.TryGetCircle( Rhino.RhinoMath.ZeroTolerance )
        if not rc: raise Exception("curve is not circle")
        return circle.Radius
    *)


    ///<summary>Closes an open curve object by making adjustments to the end points so
    ///  they meet at a point</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>-1.0</c>
    ///Maximum allowable distance between start and end
    ///  point. If omitted, the current absolute tolerance is used</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member CloseCurve(curveId:Guid, [<OPT;DEF(-1.0)>]tolerance:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        if curve.IsClosed then  curveId
        else
            if not <| curve.MakeClosed(max tolerance RhinoMath.ZeroTolerance) then  failwithf "Unable to add curve to document.  curveId:'%A' tolerance:'%A'" curveId tolerance
            let rc = Doc.Objects.AddCurve(curve)
            if rc = Guid.Empty then failwithf "Unable to add curve to document.  curveId:'%A' tolerance:'%A'" curveId tolerance
            Doc.Views.Redraw()
            rc
    (*
    def CloseCurve(curve_id, tolerance=-1.0):
        '''Closes an open curve object by making adjustments to the end points so
        they meet at a point
        Parameters:
          curve_id (guid): identifier of a curve object
          tolerance (number, optional): maximum allowable distance between start and end
                                        point. If omitted, the current absolute tolerance is used
        Returns:
          guid: id of the new curve object if successful
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if curve.IsClosed: return curve_id
        if tolerance<0.0: tolerance = Rhino.RhinoMath.ZeroTolerance
        if not curve.MakeClosed(tolerance): return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddCurve(curve)
        if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Determine the orientation (counter-clockwise or clockwise) of a closed,
    ///  planar curve</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="direction">(Vector3d) Optional, Default Value: <c>null</c>
    ///3d vector that identifies up, or Z axs, direction of
    ///  the plane to test against</param>
    ///<returns>(int) 1 if the curve's orientation is clockwise
    ///  -1 if the curve's orientation is counter-clockwise
    ///   0 if unable to compute the curve's orientation</returns>
    static member ClosedCurveOrientation(curveId:Guid, [<OPT;DEF(null)>]direction:Vector3d) : int =
        let direction0 =  match box direction with :?Vector3d as v -> v |_ -> Vector3d.ZAxis
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        if not <| curve.IsClosed then  0
        else
            let orientation = curve.ClosedCurveOrientation(direction0)
            int(orientation)
    (*
    def ClosedCurveOrientation(curve_id, direction=(0,0,1)):
        '''Determine the orientation (counter-clockwise or clockwise) of a closed,
        planar curve
        Parameters:
          curve_id (guid): identifier of a curve object
          direction (vector, optional): 3d vector that identifies up, or Z axs, direction of
                                        the plane to test against
        Returns:
          number: 1 if the curve's orientation is clockwise
                 -1 if the curve's orientation is counter-clockwise
                  0 if unable to compute the curve's orientation
        '''
        curve = rhutil.coercecurve(curve_id, -1 ,True)
        direction = rhutil.coerce3dvector(direction, True)
        if not curve.IsClosed: return 0
        orientation = curve.ClosedCurveOrientation(direction)
        return int(orientation)
    *)


    ///<summary>Convert curve to a polyline curve</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>5.0</c>
    ///The maximum angle between curve tangents at line endpoints.
    ///  If omitted, the angle tolerance is set to 5.0.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.01</c>
    ///The distance tolerance at segment midpoints. If omitted, the tolerance is set to 0.01.</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete the curve object specified by curveId. If omitted, curveId will not be deleted.</param>
    ///<param name="minEdgeLength">(float) Optional, Default Value: <c>0</c>
    ///Minimum segment length</param>
    ///<param name="maxEdgeLength">(float) Optional, Default Value: <c>0</c>
    ///Maximum segment length</param>
    ///<returns>(Guid) The new curve .</returns>
    static member ConvertCurveToPolyline(curveId:Guid, [<OPT;DEF(5.0)>]angleTolerance:float, [<OPT;DEF(0.01)>]tolerance:float, [<OPT;DEF(false)>]deleteInput:bool, [<OPT;DEF(0)>]minEdgeLength:float, [<OPT;DEF(0)>]maxEdgeLength:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let angleTolerance0 = max (Doc.ModelAngleToleranceRadians*10.) (RhinoMath.ToRadians(angleTolerance))
        let tolerance0 = max (Doc.ModelAbsoluteTolerance*10.) tolerance

        let polylineCurve = curve.ToPolyline( 0, 0, angleTolerance0, 0.0, 0.0, tolerance0, minEdgeLength, maxEdgeLength, true)
        if isNull polylineCurve then failwithf "Unable to convertCurveToPolyline %A , maxEdgeLength%f, minEdgeLength:%f,deleteInput%b, tolerance%f, angleTolerance %f " curveId   maxEdgeLength minEdgeLength deleteInput tolerance angleTolerance

        if deleteInput then
            if Doc.Objects.Replace( curveId, polylineCurve) then
                curveId
            else
                failwithf "Unable to convertCurveToPolyline %A , maxEdgeLength%f, minEdgeLength:%f,deleteInput%b, tolerance%f, angleTolerance %f " curveId   maxEdgeLength minEdgeLength deleteInput tolerance angleTolerance
        else
            let id = Doc.Objects.AddCurve( polylineCurve )
            if System.Guid.Empty=id then
                failwithf "Unable to convertCurveToPolyline %A , maxEdgeLength%f, minEdgeLength:%f,deleteInput%b, tolerance%f, angleTolerance %f " curveId   maxEdgeLength minEdgeLength deleteInput tolerance angleTolerance
            else
                id
    (*
    def ConvertCurveToPolyline(curve_id, angle_tolerance=5.0, tolerance=0.01, delete_input=False, min_edge_length=0, max_edge_length=0):
        '''Convert curve to a polyline curve
        Parameters:
          curve_id (guid): identifier of a curve object
          angle_tolerance (number, optional): The maximum angle between curve tangents at line endpoints.
                                              If omitted, the angle tolerance is set to 5.0.
          tolerance(number, optional): The distance tolerance at segment midpoints. If omitted, the tolerance is set to 0.01.
          delete_input(bool, optional): Delete the curve object specified by curve_id. If omitted, curve_id will not be deleted.
          min_edge_length (number, optional): Minimum segment length
          max_edge_length (number, optional): Maximum segment length
        Returns:
          guid: The new curve if successful.
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if angle_tolerance<=0: angle_tolerance = 5.0
        angle_tolerance = Rhino.RhinoMath.ToRadians(angle_tolerance)
        if tolerance<=0.0: tolerance = 0.01;
        polyline_curve = curve.ToPolyline( 0, 0, angle_tolerance, 0.0, 0.0, tolerance, min_edge_length, max_edge_length, True)
        if not polyline_curve: return scriptcontext.errorhandler()
        id = System.Guid.Empty
        if delete_input:
            if scriptcontext.doc.Objects.Replace( curve_id, polyline_curve): id = curve_id
        else:
            id = scriptcontext.doc.Objects.AddCurve( polyline_curve )
        if System.Guid.Empty==id: return scriptcontext.errorhandler()
        return id
    *)


    ///<summary>Returns the point on the curve that is a specified arc length
    ///  from the start of the curve.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="length">(float) The arc length from the start of the curve to evaluate.</param>
    ///<param name="fromStart">(bool) Optional, Default Value: <c>true</c>
    ///If not specified or True, then the arc length point is
    ///  calculated from the start of the curve. If False, the arc length
    ///  point is calculated from the end of the curve.</param>
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
                else failwithf "Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveId length fromStart
            else failwithf "Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveId length fromStart
        else failwithf "Unable to curveArcLengthPoint %A, length:%f, fromStart:%b" curveId length fromStart
    (*
    def CurveArcLengthPoint(curve_id, length, from_start=True):
        '''Returns the point on the curve that is a specified arc length
        from the start of the curve.
        Parameters:
          curve_id (guid): identifier of a curve object
          length (number): The arc length from the start of the curve to evaluate.
          from_start (bool, optional): If not specified or True, then the arc length point is
              calculated from the start of the curve. If False, the arc length
              point is calculated from the end of the curve.
        Returns:
          point: on curve if successful
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        curve_length = curve.GetLength()
        if curve_length>=length:
            s = 0.0
            if length==0.0: s = 0.0
            elif length==curve_length: s = 1.0
            else: s = length / curve_length
            dupe = curve.Duplicate()
            if dupe:
                if from_start==False: dupe.Reverse()
                rc, t = dupe.NormalizedLengthParameter(s)
                if rc: return dupe.PointAt(t)
                dupe.Dispose()
    *)


    ///<summary>Returns area of closed planar curves. The results are based on the
    ///  current drawing units.</summary>
    ///<param name="curveId">(Guid) The identifier of a closed, planar curve object.</param>
    ///<returns>(float) The area. </returns>
    static member CurveArea(curveId:Guid) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let tol = Doc.ModelAbsoluteTolerance
        let mp = AreaMassProperties.Compute(curve, tol)
        if isNull mp  then failwithf "curveArea failed on %A" curveId
        mp.Area
    (*
    def CurveArea(curve_id):
        '''Returns area of closed planar curves. The results are based on the
        current drawing units.
        Parameters:
          curve_id (guid): The identifier of a closed, planar curve object.
        Returns:
          list[number, number]: List of area information. The list will contain the following information:
            Element  Description
            [0]      The area. If more than one curve was specified, the
                       value will be the cumulative area.
            [1]      The absolute (+/-) error bound for the area.
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        mp = Rhino.Geometry.AreaMassProperties.Compute(curve, tol)
        if mp == None: return None
        return mp.Area, mp.AreaError
    *)


    ///<summary>Returns area centroid of closed, planar curves. The results are based
    ///  on the current drawing units.</summary>
    ///<param name="curveId">(Guid) The identifier of a closed, planar curve object.</param>
    ///<returns>(Point3d ) The 3d centroid point. </returns>
    static member CurveAreaCentroid(curveId:Guid) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let tol = Doc.ModelAbsoluteTolerance
        let mp = AreaMassProperties.Compute(curve, tol)
        if isNull mp  then failwithf "curveAreaCentroid failed on %A" curveId
        mp.Centroid
    (*
    def CurveAreaCentroid(curve_id):
        '''Returns area centroid of closed, planar curves. The results are based
        on the current drawing units.
        Parameters:
          curve_id (guid)The identifier of a closed, planar curve object.
        Returns:
          tuple(point, vector): of area centroid information containing the following information:
            Element  Description
            [0]        The 3d centroid point. If more than one curve was specified,
                     the value will be the cumulative area.
            [1]        A 3d vector with the absolute (+/-) error bound for the area
                     centroid.
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        mp = Rhino.Geometry.AreaMassProperties.Compute(curve, tol)
        if mp == None: return None
        return mp.Centroid, mp.CentroidError
    *)


    ///<summary>Get status of a curve object's annotation arrows</summary>
    ///<param name="curveId">(Guid) Identifier of a curve</param>
    ///<returns>(int) The current annotation arrow style
    ///  0 = no arrows
    ///  1 = display arrow at start of curve
    ///  2 = display arrow at end of curve
    ///  3 = display arrow at both start and end of curve</returns>
    static member CurveArrows(curveId:Guid) : int = //GET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)  
        let attr = rhobj.Attributes
        let rc = attr.ObjectDecoration
        if rc=DocObjects.ObjectDecoration.None then  0
        elif rc=DocObjects.ObjectDecoration.StartArrowhead then 1
        elif rc=DocObjects.ObjectDecoration.EndArrowhead then 2
        elif rc=DocObjects.ObjectDecoration.BothArrowhead then 3
        else -1
    (*
    def CurveArrows(curve_id, arrow_style=None):
        '''Enables or disables a curve object's annotation arrows
        Parameters:
          curve_id (guid): identifier of a curve
          arrow_style (number, optional): the style of annotation arrow to be displayed. If omitted the current type is returned.
            0 = no arrows
            1 = display arrow at start of curve
            2 = display arrow at end of curve
            3 = display arrow at both start and end of curve
          Returns:
            number: if arrow_style is not specified, the current annotation arrow style
            number: if arrow_style is specified, the previous arrow style
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        rhobj = rhutil.coercerhinoobject(curve_id, True, True)
        attr = rhobj.Attributes
        rc = attr.ObjectDecoration
        if arrow_style is not None:
            if arrow_style==0:
                attr.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.None
            elif arrow_style==1:
                attr.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.StartArrowhead
            elif arrow_style==2:
                attr.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.EndArrowhead
            elif arrow_style==3:
                attr.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.BothArrowhead
            id = rhutil.coerceguid(curve_id, True)
            scriptcontext.doc.Objects.ModifyAttributes(id, attr, True)
            scriptcontext.doc.Views.Redraw()
        if rc==Rhino.DocObjects.ObjectDecoration.None: return 0
        if rc==Rhino.DocObjects.ObjectDecoration.StartArrowhead: return 1
        if rc==Rhino.DocObjects.ObjectDecoration.EndArrowhead: return 2
        if rc==Rhino.DocObjects.ObjectDecoration.BothArrowhead: return 3
    *)

    ///<summary>Enables or disables a curve object's annotation arrows</summary>
    ///<param name="curveId">(Guid) Identifier of a curve</param>
    ///<param name="arrowStyle">(int)The style of annotation arrow to be displayed. If omitted the current type is returned.
    ///  0 = no arrows
    ///  1 = display arrow at start of curve
    ///  2 = display arrow at end of curve
    ///  3 = display arrow at both start and end of curve</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurveArrows(curveId:Guid, arrowStyle:int) : unit = //SET
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)  
        let attr = rhobj.Attributes
        let rc = attr.ObjectDecoration
        if arrowStyle >= 0 && arrowStyle <= 3 then
            if arrowStyle=0 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.None
            elif arrowStyle=1 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.StartArrowhead
            elif arrowStyle=2 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.EndArrowhead
            elif arrowStyle=3 then
                attr.ObjectDecoration <- DocObjects.ObjectDecoration.BothArrowhead
            Doc.Objects.ModifyAttributes(curveId, attr, true) |> ignore
            Doc.Views.Redraw()
        else
           failwithf "curve Arrow style %d invalid" arrowStyle
        
    (*
    def CurveArrows(curve_id, arrow_style=None):
        '''Enables or disables a curve object's annotation arrows
        Parameters:
          curve_id (guid): identifier of a curve
          arrow_style (number, optional): the style of annotation arrow to be displayed. If omitted the current type is returned.
            0 = no arrows
            1 = display arrow at start of curve
            2 = display arrow at end of curve
            3 = display arrow at both start and end of curve
          Returns:
            number: if arrow_style is not specified, the current annotation arrow style
            number: if arrow_style is specified, the previous arrow style
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        rhobj = rhutil.coercerhinoobject(curve_id, True, True)
        attr = rhobj.Attributes
        rc = attr.ObjectDecoration
        if arrow_style is not None:
            if arrow_style==0:
                attr.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.None
            elif arrow_style==1:
                attr.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.StartArrowhead
            elif arrow_style==2:
                attr.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.EndArrowhead
            elif arrow_style==3:
                attr.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.BothArrowhead
            id = rhutil.coerceguid(curve_id, True)
            scriptcontext.doc.Objects.ModifyAttributes(id, attr, True)
            scriptcontext.doc.Views.Redraw()
        if rc==Rhino.DocObjects.ObjectDecoration.None: return 0
        if rc==Rhino.DocObjects.ObjectDecoration.StartArrowhead: return 1
        if rc==Rhino.DocObjects.ObjectDecoration.EndArrowhead: return 2
        if rc==Rhino.DocObjects.ObjectDecoration.BothArrowhead: return 3
    *)


    ///<summary>Calculates the difference between two closed, planar curves and
    ///  adds the results to the document. Note, curves must be coplanar.</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object.</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///A positive tolerance value, or None for the doc default.</param>
    ///<returns>(Guid List) The identifiers of the new objects , .</returns>
    static member CurveBooleanDifference(curveA:Guid, curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA  
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB  
        let tolerance = max Doc.ModelAbsoluteTolerance tolerance        
        let outCurves = Curve.CreateBooleanDifference(curve0, curve1, tolerance)
        let curves = ResizeArray()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc = Guid.Empty then failwithf "Unable to add curve to document.  curveA:'%A' curveB:'%A' " curveA curveB
                    curves.Add(rc)
            Doc.Views.Redraw()
            curves
        else
            failwithf "Unable to add curve to document.  curveA:'%A' curveB:'%A' " curveA curveB
    (*
    def CurveBooleanDifference(curve_id_0, curve_id_1, tolerance=None):
        '''Calculates the difference between two closed, planar curves and
        adds the results to the document. Note, curves must be coplanar.
        Parameters:
          curve_id_0 (guid): identifier of the first curve object.
          curve_id_1 (guid): identifier of the second curve object.
          tolerance (float, optional): a positive tolerance value, or None for the doc default.
        Returns:
          list(guid, ...): The identifiers of the new objects if successful, None on error.
        '''
        curve0 = rhutil.coercecurve(curve_id_0, -1, True)
        curve1 = rhutil.coercecurve(curve_id_1, -1, True)
        if tolerance is None or tolerance<0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        out_curves = Rhino.Geometry.Curve.CreateBooleanDifference(curve0, curve1, tolerance)
        curves = []
        if out_curves:
            for curve in out_curves:
                if curve and curve.IsValid:
                    rc = scriptcontext.doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc==System.Guid.Empty: raise Exception("unable to add curve to document")
                    curves.append(rc)
        scriptcontext.doc.Views.Redraw()
        return curves
    *)


    ///<summary>Calculates the intersection of two closed, planar curves and adds
    ///  the results to the document. Note, curves must be coplanar.</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object.</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///A positive tolerance value, or None for the doc default.</param>
    ///<returns>(Guid seq) The identifiers of the new objects.</returns>
    static member CurveBooleanIntersection(curveA:Guid, curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA  
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB  
        let tolerance = max Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanIntersection(curve0, curve1, tolerance)
        let curves = ResizeArray()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc = Guid.Empty then failwithf "Unable to add curve to document.  curveA:'%A' curveB:'%A' " curveA curveB
                    curves.Add(rc)
            Doc.Views.Redraw()
            curves
        else
            failwithf "Unable to add curve to document.  curveA:'%A' curveB:'%A' " curveA curveB
    (*
    def CurveBooleanIntersection(curve_id_0, curve_id_1, tolerance=None):
        '''Calculates the intersection of two closed, planar curves and adds
        the results to the document. Note, curves must be coplanar.
        Parameters:
          curve_id_0 (guid): identifier of the first curve object.
          curve_id_1 (guid): identifier of the second curve object.
          tolerance (float, optional): a positive tolerance value, or None for the doc default.
        Returns:
          list(guid, ...): The identifiers of the new objects.
        '''
        curve0 = rhutil.coercecurve(curve_id_0, -1, True)
        curve1 = rhutil.coercecurve(curve_id_1, -1, True)
        if tolerance is None or tolerance<0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        out_curves = Rhino.Geometry.Curve.CreateBooleanIntersection(curve0, curve1, tolerance)
        curves = []
        if out_curves:
            for curve in out_curves:
                if curve and curve.IsValid:
                    rc = scriptcontext.doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc==System.Guid.Empty: raise Exception("unable to add curve to document")
                    curves.append(rc)
        scriptcontext.doc.Views.Redraw()
        return curves
    *)


    ///<summary>Calculate the union of two or more closed, planar curves and
    ///  add the results to the document. Note, curves must be coplanar.</summary>
    ///<param name="curveIds">(Guid seq) List of two or more close planar curves identifiers</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///A positive tolerance value, or None for the doc default.</param>
    ///<returns>(Guid seq) The identifiers of the new objects.</returns>
    static member CurveBooleanUnion(curveIds:Guid seq, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let inCurves = [| for id in curveIds -> RhinoScriptSyntax.CoerceCurve id |]  
        if Array.length(inCurves) < 2 then failwithf "curveBooleanUnion:curveIds must have at least 2 curves %A" curveIds
        let tolerance = max Doc.ModelAbsoluteTolerance tolerance
        let outCurves = Curve.CreateBooleanUnion(inCurves, tolerance)        
        let curves = ResizeArray()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc = Guid.Empty then failwithf "curveBooleanUnion: Unable to add curve to document.  curveIds:'%A' " curveIds
                    curves.Add(rc)
            Doc.Views.Redraw()
        curves
    (*
    def CurveBooleanUnion(curve_id, tolerance=None):
        '''Calculate the union of two or more closed, planar curves and
        add the results to the document. Note, curves must be coplanar.
        Parameters:
          curve_id ([guid, guid, ...])list of two or more close planar curves identifiers
          tolerance (float, optional): a positive tolerance value, or None for the doc default.
        Returns:
          list(guid, ...): The identifiers of the new objects.
        '''
        in_curves = [rhutil.coercecurve(id,-1,True) for id in curve_id]
        if len(in_curves)<2: raise ValueException("curve_id must have at least 2 curves")
        if tolerance is None or tolerance<0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        out_curves = Rhino.Geometry.Curve.CreateBooleanUnion(in_curves, tolerance)
        curves = []
        if out_curves:
            for curve in out_curves:
                if curve and curve.IsValid:
                    rc = scriptcontext.doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc==System.Guid.Empty: raise Exception("unable to add curve to document")
                    curves.append(rc)
            scriptcontext.doc.Views.Redraw()
        return curves
    *)


    ///<summary>Intersects a curve object with a brep object. Note, unlike the
    ///  CurveSurfaceIntersection function, this function works on trimmed surfaces.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="brepId">(Guid) Identifier of a brep object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///Distance tolerance at segment midpoints.
    ///  If omitted, the current absolute tolerance is used.</param>
    ///<returns>(Guid ResizeArray * Guid ResizeArray) List of Curves and List of points .</returns>
    static member CurveBrepIntersect(curveId:Guid, brepId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray * Guid ResizeArray=
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let brep = RhinoScriptSyntax.CoerceBrep(brepId)  
        let tolerance0 = max Doc.ModelAbsoluteTolerance tolerance
        let rc, outCurves, outPoints = Intersect.Intersection.CurveBrep(curve, brep, tolerance0)
        if not <| rc then  failwithf "curveBrepIntersect: Unable to add curve to document.  curveId:'%A' brepId:'%A' tolerance:'%A'" curveId brepId tolerance

        let curves = ResizeArray()
        let points = ResizeArray()
        for curve in outCurves do
            if notNull curve && curve.IsValid then
                let rc = Doc.Objects.AddCurve(curve)
                curve.Dispose()
                if rc = Guid.Empty then failwithf "curveBrepIntersect: Unable to add curve to document.  curveId:'%A' brepId:'%A' tolerance:'%A'" curveId brepId tolerance
                curves.Add(rc)
        for point in outPoints do
            if point.IsValid then
                let rc = Doc.Objects.AddPoint(point)
                points.Add(rc)
        Doc.Views.Redraw()
        curves, points
    (*
    def CurveBrepIntersect(curve_id, brep_id, tolerance=None):
        '''Intersects a curve object with a brep object. Note, unlike the
        CurveSurfaceIntersection function, this function works on trimmed surfaces.
        Parameters:
          curve_id (guid): identifier of a curve object
          brep_id (guid): identifier of a brep object
          tolerance (number, optional): distance tolerance at segment midpoints.
                            If omitted, the current absolute tolerance is used.
        Returns:
          list(guid, ...): identifiers for the newly created intersection objects if successful.
          None: on error.
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        brep = rhutil.coercebrep(brep_id, True)
        if tolerance is None or tolerance<0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        rc, out_curves, out_points = Rhino.Geometry.Intersect.Intersection.CurveBrep(curve, brep, tolerance)
        if not rc: return scriptcontext.errorhandler()
        
        curves = []
        points = []
        for curve in out_curves:
            if curve and curve.IsValid:
                rc = scriptcontext.doc.Objects.AddCurve(curve)
                curve.Dispose()
                if rc==System.Guid.Empty: raise Exception("unable to add curve to document")
                curves.append(rc)
        for point in out_points:
            if point and point.IsValid:
                rc = scriptcontext.doc.Objects.AddPoint(point)
                points.append(rc)
        if not curves and not points: return None
        scriptcontext.doc.Views.Redraw()
        return curves, points
    *)


    ///<summary>Returns the 3D point locations on two objects where they are closest to
    ///  each other. Note, this function provides similar functionality to that of
    ///  Rhino's ClosestPt command.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object to test</param>
    ///<param name="curveIds">(Guid seq) List of identifiers of point cloud, curve, surface, or
    ///  polysurface to test against</param>
    ///<returns>(Guid * Point3d * Point3d) containing the results of the closest point calculation.
    ///  The elements are as follows:
    ///    [0]    The identifier of the closest object.
    ///    [1]    The 3-D point that is closest to the closest object.
    ///    [2]    The 3-D point that is closest to the test curve.</returns>
    static member CurveClosestObject(curveId:Guid, curveIds:Guid seq) : Guid * Point3d * Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let geometry = ResizeArray()
        for curveId in curveIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)  
            geometry.Add( rhobj.Geometry )
        if Seq.isEmpty geometry then failwithf "curveIds must contain at least one item.  curveId:'%A' curveIds:'%A'" curveId curveIds
        let curvePoint = ref Point3d.Unset
        let geomPoint  = ref Point3d.Unset
        let whichGeom = ref 0
        let success = curve.ClosestPoints(geometry,curvePoint, geomPoint, whichGeom)
        if success then  curveIds|> Seq.item !whichGeom, !geomPoint, !curvePoint
        else failwithf "curveClosestObject failed  curveId:'%A' curveIds:'%A'" curveId curveIds
    (*
    def CurveClosestObject(curve_id, object_ids):
        '''Returns the 3D point locations on two objects where they are closest to
        each other. Note, this function provides similar functionality to that of
        Rhino's ClosestPt command.
        Parameters:
          curve_id (guid):identifier of the curve object to test
          object_ids ([guid, ...]) list of identifiers of point cloud, curve, surface, or
            polysurface to test against
        Returns:
          tuple[guid, point, point]: containing the results of the closest point calculation.
            The elements are as follows:
              [0]    The identifier of the closest object.
              [1]    The 3-D point that is closest to the closest object.
              [2]    The 3-D point that is closest to the test curve.
        '''
        curve = rhutil.coercecurve(curve_id,-1,True)
        geometry = []
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        for object_id in object_ids:
            rhobj = rhutil.coercerhinoobject(object_id, True, True)
            geometry.append( rhobj.Geometry )
        if not geometry: raise ValueError("object_ids must contain at least one item")
        success, curve_point, geom_point, which_geom = curve.ClosestPoints(geometry, 0.0)
        if success: return object_ids[which_geom], geom_point, curve_point
    *)


    ///<summary>Returns parameter of the point on a curve that is closest to a test point.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="point">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The parameter of the closest point on the curve</returns>
    static member CurveClosestPoint(curveId:Guid, point:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let t = ref 0.
        let rc = curve.ClosestPoint(point,t)
        if not <| rc then failwithf "curveClosestPoint failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        !t
    (*
    def CurveClosestPoint(curve_id, point, segment_index=-1 ):
        '''Returns parameter of the point on a curve that is closest to a test point.
        Parameters:
          curve_id (guid): identifier of a curve object
          point (point): sampling point
          segment_index (number, optional): curve segment index if `curve_id` identifies a polycurve
        Returns:
          number: The parameter of the closest point on the curve
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        point = rhutil.coerce3dpoint(point, True)
        rc, t = curve.ClosestPoint(point, 0.0)
        if not rc: raise Exception("ClosestPoint failed")
        return t
    *)


    ///<summary>Returns the 3D point locations calculated by contouring a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object.</param>
    ///<param name="startPoint">(Point3d) 3D starting point of a center line.</param>
    ///<param name="endPoint">(Point3d) 3D ending point of a center line.</param>
    ///<param name="interval">(float) The distance between contour curves. </param>
    ///<returns>(Point3d seq) A list of 3D points, one for each contour</returns>
    static member CurveContourPoints(curveId:Guid, startPoint:Point3d, endPoint:Point3d, interval:float) : Point3d [] =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        if startPoint.DistanceTo(endPoint)<RhinoMath.ZeroTolerance then
            failwithf "Start && ende point are too close to define a line.  curveId:'%A' startPoint:'%A' endPoint:'%A'" curveId startPoint endPoint
        curve.DivideAsContour( startPoint, endPoint, interval)
    (*
    def CurveContourPoints(curve_id, start_point, end_point, interval=None):
        '''Returns the 3D point locations calculated by contouring a curve object.
        Parameters:
          curve_id (guid): identifier of a curve object.
          start_point (point): 3D starting point of a center line.
          end_point (point): 3D ending point of a center line.
          interval (number, optional): The distance between contour curves. If omitted,
          the interval will be equal to the diagonal distance of the object's
          bounding box divided by 50.
        Returns:
          list(point, ....): A list of 3D points, one for each contour
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        start_point = rhutil.coerce3dpoint(start_point, True)
        end_point = rhutil.coerce3dpoint(end_point, True)
        if start_point.DistanceTo(end_point)<Rhino.RhinoMath.ZeroTolerance:
            raise Exception("start and end point are too close to define a line")
        if not interval:
            bbox = curve.GetBoundingBox(True)
            diagonal = bbox.Max - bbox.Min
            interval = diagonal.Length / 50.0
        rc = curve.DivideAsContour( start_point, end_point, interval )
        return list(rc)
    *)


    ///<summary>Returns the curvature of a curve at a parameter. See the Rhino help for
    ///  details on curve curvature</summary>
    ///<param name="curveId">(Guid) Identifier of the curve</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<returns>(Point3d * Vector3d * Point3d * float * Vector3d) of curvature information on success
    ///  [0] = point at specified parameter
    ///  [1] = tangent vector
    ///  [2] = center of radius of curvature
    ///  [3] = radius of curvature
    ///  [4] = curvature vector</returns>
    static member CurveCurvature(curveId:Guid, parameter:float) : Point3d * Vector3d * Point3d * float * Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let point = curve.PointAt(parameter)
        let tangent = curve.TangentAt(parameter)
        if tangent.IsTiny(1e-10) then  failwithf "curveCurvature: failed on tangent that is too small %A" curveId
        let cv = curve.CurvatureAt(parameter)
        let k = cv.Length
        if k<RhinoMath.SqrtEpsilon then  failwithf "curveCurvature: failed on tangent that is too small %A" curveId
        let rv = cv / (k*k)
        let circle = Circle(point, tangent, point + 2.0*rv)
        let center = point + rv
        let radius = circle.Radius
        point, tangent, center, radius, cv
    (*
    def CurveCurvature(curve_id, parameter):
        '''Returns the curvature of a curve at a parameter. See the Rhino help for
        details on curve curvature
        Parameters:
          curve_id (guid): identifier of the curve
          parameter (number): parameter to evaluate
        Returns:
          tuple[point, vector, point, number, vector]: of curvature information on success
            [0] = point at specified parameter
            [1] = tangent vector
            [2] = center of radius of curvature
            [3] = radius of curvature
            [4] = curvature vector
          None: on failure
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        point = curve.PointAt(parameter)
        tangent = curve.TangentAt(parameter)
        if tangent.IsTiny(0): return scriptcontext.errorhandler()
        cv = curve.CurvatureAt(parameter)
        k = cv.Length
        if k<Rhino.RhinoMath.SqrtEpsilon: return scriptcontext.errorhandler()
        rv = cv / (k*k)
        circle = Rhino.Geometry.Circle(point, tangent, point + 2.0*rv)
        center = point + rv
        radius = circle.Radius
        return point, tangent, center, radius, cv
    *)


    ///<summary>Calculates intersection of two curve objects.</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object.</param>
    ///<param name="curveB">(Guid) Optional, Default Value: <c>null</c>
    ///Identifier of the second curve object. If omitted, then a
    ///  self-intersection test will be performed on curveA.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///  Absolute tolerance in drawing units. If omitted,
    ///  the document's current absolute tolerance is used.</param>
    ///<returns>( a List of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float)
    ///  List of tuples: containing intersection information .
    ///  The list will contain one or more of the following elements:
    ///    Element Type     Description
    ///    [n][0]  Number   The intersection event type, either Point (1) or Overlap (2).
    ///    [n][1]  Point3d  If the event type is Point (1), then the intersection point
    ///      on the first curve. If the event type is Overlap (2), then
    ///      intersection start point on the first curve.
    ///    [n][2]  Point3d  If the event type is Point (1), then the intersection point
    ///      on the first curve. If the event type is Overlap (2), then
    ///      intersection end point on the first curve.
    ///    [n][3]  Point3d  If the event type is Point (1), then the intersection point
    ///      on the second curve. If the event type is Overlap (2), then
    ///      intersection start point on the second curve.
    ///    [n][4]  Point3d  If the event type is Point (1), then the intersection point
    ///      on the second curve. If the event type is Overlap (2), then
    ///      intersection end point on the second curve.
    ///    [n][5]  Number   If the event type is Point (1), then the first curve parameter.
    ///      If the event type is Overlap (2), then the start value of the
    ///      first curve parameter range.
    ///    [n][6]  Number   If the event type is Point (1), then the first curve parameter.
    ///      If the event type is Overlap (2), then the end value of the
    ///      first curve parameter range.
    ///    [n][7]  Number   If the event type is Point (1), then the second curve parameter.
    ///      If the event type is Overlap (2), then the start value of the
    ///      second curve parameter range.
    ///    [n][8]  Number   If the event type is Point (1), then the second curve parameter.
    ///      If the event type is Overlap (2), then the end value of the
    ///      second curve parameter range.</returns>
    static member CurveCurveIntersection(curveA:Guid, [<OPT;DEF(null)>]curveB:Guid, [<OPT;DEF(0.0)>]tolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) ResizeArray =
        let curve1 = RhinoScriptSyntax.CoerceCurve curveA 
        let curve2 =  match box curveB with :?Guid as g -> RhinoScriptSyntax.CoerceCurve g |_ -> curve1
          
        let tolerance0 = max tolerance Doc.ModelAbsoluteTolerance
        let mutable rc = null
        if curveB<>curveA then
            rc <- Intersect.Intersection.CurveCurve(curve1, curve2, tolerance0, Doc.ModelAbsoluteTolerance)
        else
            rc <- Intersect.Intersection.CurveSelf(curve1, tolerance0)

        if isNull rc then failwithf "curveCurveIntersection faile dor %A %A tolerance %f" curveB curveA tolerance
        let events = ResizeArray()
        for i =0 to rc.Count-1 do
            let mutable eventType = 1
            if( rc.[i].IsOverlap ) then  eventType <- 2
            let oa = rc.[i].OverlapA
            let ob = rc.[i].OverlapB
            let element = (eventType, rc.[i].PointA, rc.[i].PointA2, rc.[i].PointB, rc.[i].PointB2, oa.[0], oa.[1], ob.[0], ob.[1])
            events.Add(element)
        events
       
    (*
    def CurveCurveIntersection(curveA, curveB=None, tolerance=-1):
        '''Calculates intersection of two curve objects.
        Parameters:
          curveA (guid): identifier of the first curve object.
          curveB  (guid, optional): identifier of the second curve object. If omitted, then a
                   self-intersection test will be performed on curveA.
          tolerance (number, optional): absolute tolerance in drawing units. If omitted,
                            the document's current absolute tolerance is used.
        Returns:
          list(list(point, point, point, point, number, number, number, number, number, number), ...):
            list of tuples: containing intersection information if successful.
            The list will contain one or more of the following elements:
              Element Type     Description
              [n][0]  Number   The intersection event type, either Point (1) or Overlap (2).
              [n][1]  Point3d  If the event type is Point (1), then the intersection point 
                              on the first curve. If the event type is Overlap (2), then
                              intersection start point on the first curve.
              [n][2]  Point3d  If the event type is Point (1), then the intersection point
                              on the first curve. If the event type is Overlap (2), then
                              intersection end point on the first curve.
              [n][3]  Point3d  If the event type is Point (1), then the intersection point 
                              on the second curve. If the event type is Overlap (2), then
                              intersection start point on the second curve.
              [n][4]  Point3d  If the event type is Point (1), then the intersection point
                              on the second curve. If the event type is Overlap (2), then
                              intersection end point on the second curve.
              [n][5]  Number   If the event type is Point (1), then the first curve parameter.
                              If the event type is Overlap (2), then the start value of the
                              first curve parameter range.
              [n][6]  Number   If the event type is Point (1), then the first curve parameter.
                              If the event type is Overlap (2), then the end value of the
                              first curve parameter range.
              [n][7]  Number   If the event type is Point (1), then the second curve parameter.
                              If the event type is Overlap (2), then the start value of the
                              second curve parameter range.
              [n][8]  Number   If the event type is Point (1), then the second curve parameter.
                              If the event type is Overlap (2), then the end value of the 
                              second curve parameter range.
        '''
        curveA = rhutil.coercecurve(curveA, -1, True)
        if curveB: curveB = rhutil.coercecurve(curveB, -1, True)
        if tolerance is None or tolerance<0.0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        if curveB:
            rc = Rhino.Geometry.Intersect.Intersection.CurveCurve(curveA, curveB, tolerance, 0.0)
        else:
            rc = Rhino.Geometry.Intersect.Intersection.CurveSelf(curveA, tolerance)
        if rc:
            events = []
            for i in xrange(rc.Count):
                event_type = 1
                if( rc[i].IsOverlap ): event_type = 2
                oa = rc[i].OverlapA
                ob = rc[i].OverlapB
                element = (event_type, rc[i].PointA, rc[i].PointA2, rc[i].PointB, rc[i].PointB2, oa[0], oa[1], ob[0], ob[1])
                events.append(element)
            return events
    *)


    ///<summary>Returns the degree of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve.</param>
    ///<returns>(int) The degree of the curve .</returns>
    static member CurveDegree(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        curve.Degree
    (*
    def CurveDegree(curve_id, segment_index=-1):
        '''Returns the degree of a curve object.
        Parameters:
          curve_id (guid): identifier of a curve object.
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve.
        Returns:
          number: The degree of the curve if successful.
          None: on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        return curve.Degree
    *)


    ///<summary>Returns the minimum and maximum deviation between two curve objects</summary>
    ///<param name="curveA">(Guid) Curve a of 'identifiers of two curves' (FIXME 0)</param>
    ///<param name="curveB">(Guid) Curve b of 'identifiers of two curves' (FIXME 0)</param>
    ///<returns>(float * float * float * float * float * float) of deviation information on success
    ///  [0] = curve_a parameter at maximum overlap distance point
    ///  [1] = curve_b parameter at maximum overlap distance point
    ///  [2] = maximum overlap distance
    ///  [3] = curve_a parameter at minimum overlap distance point
    ///  [4] = curve_b parameter at minimum overlap distance point
    ///  [5] = minimum distance between curves</returns>
    static member CurveDeviation(curveA:Guid, curveB:Guid) : float * float * float * float * float * float =
        let curveA = RhinoScriptSyntax.CoerceCurve curveA  
        let curveB = RhinoScriptSyntax.CoerceCurve curveB  
        let tol = Doc.ModelAbsoluteTolerance
        let ok, maxa, maxb, maxd, mina, minb, mind = Curve.GetDistancesBetweenCurves(curveA, curveB, tol)
        if not ok then  failwithf "curveDeviation failed for %A %A" curveB curveA
        else
            maxa, maxb, maxd, mina, minb, mind
    (*
    def CurveDeviation(curve_a, curve_b):
        '''Returns the minimum and maximum deviation between two curve objects
        Parameters:
          curve_a, curve_b (guid): identifiers of two curves
        Returns:
          tuple[number, number, number, number, number, number]: of deviation information on success
            [0] = curve_a parameter at maximum overlap distance point
            [1] = curve_b parameter at maximum overlap distance point
            [2] = maximum overlap distance
            [3] = curve_a parameter at minimum overlap distance point
            [4] = curve_b parameter at minimum overlap distance point
            [5] = minimum distance between curves
          None on error
        '''
        curve_a = rhutil.coercecurve(curve_a, -1, True)
        curve_b = rhutil.coercecurve(curve_b, -1, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        rc = Rhino.Geometry.Curve.GetDistancesBetweenCurves(curve_a, curve_b, tol)
        if not rc[0]: return scriptcontext.errorhandler()
        maxa = rc[2]
        maxb = rc[3]
        maxd = rc[1]
        mina = rc[5]
        minb = rc[6]
        mind = rc[4]
        return maxa, maxb, maxd, mina, minb, mind
    *)


    ///<summary>Returns the dimension of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///  The curve segment if curveId identifies a polycurve.</param>
    ///<returns>(int) The dimension of the curve . .</returns>
    static member CurveDim(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        curve.Dimension
    (*
    def CurveDim(curve_id, segment_index=-1):
        '''Returns the dimension of a curve object
        Parameters:
          curve_id (guid): identifier of a curve object.
          segment_index (number, optional): the curve segment if `curve_id` identifies a polycurve.
        Returns:
          number: The dimension of the curve if successful. None on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        return curve.Dimension
    *)


    ///<summary>Tests if two curve objects are generally in the same direction or if they
    ///  would be more in the same direction if one of them were flipped. When testing
    ///  curve directions, both curves must be either open or closed - you cannot test
    ///  one open curve and one closed curve.</summary>
    ///<param name="curveA">(Guid) Identifier of first curve object</param>
    ///<param name="curveB">(Guid) Identifier of second curve object</param>
    ///<returns>(bool) True if the curve directions match, otherwise False.</returns>
    static member CurveDirectionsMatch(curveA:Guid, curveB:Guid) : bool =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA  
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB  
        Curve.DoDirectionsMatch(curve0, curve1)
    (*
    def CurveDirectionsMatch(curve_id_0, curve_id_1):
        '''Tests if two curve objects are generally in the same direction or if they
        would be more in the same direction if one of them were flipped. When testing
        curve directions, both curves must be either open or closed - you cannot test
        one open curve and one closed curve.
        Parameters:
          curve_id_0 (guid): identifier of first curve object
          curve_id_1 (guid): identifier of second curve object
        Returns:
          bool: True if the curve directions match, otherwise False.
        '''
        curve0 = rhutil.coercecurve(curve_id_0, -1, True)
        curve1 = rhutil.coercecurve(curve_id_1, -1, True)
        return Rhino.Geometry.Curve.DoDirectionsMatch(curve0, curve1)
    *)


    ///<summary>Search for a derivatitive, tangent, or curvature discontinuity in
    ///  a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of curve object</param>
    ///<param name="style">(int) The type of continuity to test for. The types of
    ///  continuity are as follows:
    ///  Value    Description
    ///  1        C0 - Continuous function
    ///  2        C1 - Continuous first derivative
    ///  3        C2 - Continuous first and second derivative
    ///  4        G1 - Continuous unit tangent
    ///  5        G2 - Continuous unit tangent and curvature</param>
    ///<returns>(Point3d List) 3D points where the curve is discontinuous</returns>
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
    (*
    def CurveDiscontinuity(curve_id, style):
        '''Search for a derivatitive, tangent, or curvature discontinuity in
        a curve object.
        Parameters:
          curve_id (guid): identifier of curve object
          style (number): The type of continuity to test for. The types of
              continuity are as follows:
              Value    Description
              1        C0 - Continuous function
              2        C1 - Continuous first derivative
              3        C2 - Continuous first and second derivative
              4        G1 - Continuous unit tangent
              5        G2 - Continuous unit tangent and curvature
        Returns:
          list(point, ...): 3D points where the curve is discontinuous
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        dom = curve.Domain
        t0 = dom.Min
        t1 = dom.Max
        points = []
        get_next = True
        while get_next:
            get_next, t = curve.GetNextDiscontinuity(System.Enum.ToObject(Rhino.Geometry.Continuity, style), t0, t1)
            if get_next:
                points.append(curve.PointAt(t))
                t0 = t # Advance to the next parameter
        return points
    *)


    ///<summary>Returns the domain of a curve object
    ///  as an indexable object with two elements.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve.</param>
    ///<returns>(Interval) the domain of the curve .</returns>
    static member CurveDomain(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Interval =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        curve.Domain
    (*
    def CurveDomain(curve_id, segment_index=-1):
        '''Returns the domain of a curve object
        as an indexable object with two elements.
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve.
        Returns:
          list(number, number): the domain of the curve if successful.
             [0] Domain minimum
             [1] Domain maximum
          None: on error
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        return curve.Domain
    *)


    ///<summary>Returns the edit, or Greville, points of a curve object.
    ///  For each curve control point, there is a corresponding edit point.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="returnParameters">(bool) Optional, Default Value: <c>false</c>
    ///If True, return as a list of curve parameters.
    ///  If False, return as a list of 3d points</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index is `curveId` identifies a polycurve</param>
    ///<returns>(Point3dList) curve edit points on success</returns>
    static member CurveEditPoints(curveId:Guid, [<OPT;DEF(false)>]returnParameters:bool, [<OPT;DEF(-1)>]segmentIndex:int) : Collections.Point3dList =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let nc = curve.ToNurbsCurve()
        if isNull nc then  failwithf "curveEditPoints faile for %A" curveId
        nc.GrevillePoints()
    (*
    def CurveEditPoints(curve_id, return_parameters=False, segment_index=-1):
        '''Returns the edit, or Greville, points of a curve object. 
        For each curve control point, there is a corresponding edit point.
        Parameters:
          curve_id (guid): identifier of the curve object
          return_parameters (bool, optional): if True, return as a list of curve parameters.
                                              If False, return as a list of 3d points
          segment_index (number, optional): the curve segment index is `curve_id` identifies a polycurve
        Returns:
          list(point, ....): curve edit points on success
          None on error
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        nc = curve.ToNurbsCurve()
        if not nc: return scriptcontext.errorhandler()
        if return_parameters: return nc.GrevilleParameters()
        return list(nc.GrevillePoints())
    *)


    ///<summary>Returns the end point of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3d endpoint of the curve .</returns>
    static member CurveEndPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        curve.PointAtEnd
    (*
    def CurveEndPoint(curve_id, segment_index=-1):
        '''Returns the end point of a curve object
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          point: The 3d endpoint of the curve if successful.
          None: on error
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        return curve.PointAtEnd
    *)


    //(FIXME) VarOutTypes
    ///<summary>Find points at which to cut a pair of curves so that a fillet of a
    ///  specified radius fits. A fillet point is a pair of points (point0, point1)
    ///  such that there is a circle of radius tangent to curve curve0 at point0 and
    ///  tangent to curve curve1 at point1. Of all possible fillet points, this
    ///  function returns the one which is the closest to the base point base_point_0,
    ///  base_point_1. Distance from the base point is measured by the sum of arc
    ///  lengths along the two curves.</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object.</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object.</param>
    ///<param name="radius">(float) The fillet radius. </param>
    ///<param name="basePointA">(Point3d) Optional, Default Value: <c>null</c>
    ///The base point on the first curve.
    ///  If omitted, the starting point of the curve is used.</param>
    ///<param name="basePointB">(Point3d) Optional, Default Value: <c>null</c>
    ///The base point on the second curve. If omitted,
    ///  the starting point of the curve is used.</param>
    ///<param name="returnPoints">(bool) Optional, Default Value: <c>true</c>
    ///If True (Default), then fillet points are
    ///  returned. Otherwise, a fillet curve is created and
    ///  it's identifier is returned.</param>
    ///<returns>(Point3d * Point3d * Plane) 
    ///  . The list elements are as follows:
    ///    [0]    A point on the first curve at which to cut (point).
    ///    [1]    A point on the second curve at which to cut (point).
    ///    [2]    The fillet plane </returns>
    static member CurveFilletPoints(curveA:Guid, curveB:Guid, radius:float, [<OPT;DEF(null)>]basePointA:Point3d, [<OPT;DEF(null)>]basePointB:Point3d, [<OPT;DEF(true)>]returnPoints:bool) : Point3d * Point3d * Plane =
        let curve0 = RhinoScriptSyntax.CoerceCurve curveA  
        let curve1 = RhinoScriptSyntax.CoerceCurve curveB  
        let basePointA = RhinoScriptSyntax.Coerce3dPoint(basePointA, nullIsUnset=true)  
        let basePointB = RhinoScriptSyntax.Coerce3dPoint(basePointB, nullIsUnset=true)   

        let inline distance  (a:Point3d)(b:Point3d) = (a-b).Length

        let t0Base =
            if basePointA <> Point3d.Unset then
                let ok,t = curve0.ClosestPoint(basePointA)
                if not ok then failwithf "curveFilletPoints failed 1 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
                t
            else
                let distEnde  = min  (distance curve1.PointAtStart curve0.PointAtEnd)   (distance curve1.PointAtEnd curve0.PointAtEnd)
                let distStart = min  (distance curve1.PointAtStart curve0.PointAtStart) (distance curve1.PointAtEnd curve0.PointAtStart)
                if distStart < distEnde then curve0.Domain.Min else curve0.Domain.Max

        let t1Base =
            if basePointB <> Point3d.Unset then
                let ok,t = curve1.ClosestPoint(basePointB)
                if not ok then failwithf "curveFilletPoints failed 2 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
                t
            else
                let distEnde  = min  (distance curve0.PointAtStart curve1.PointAtEnd)   (distance curve0.PointAtEnd curve1.PointAtEnd)
                let distStart = min  (distance curve0.PointAtStart curve1.PointAtStart) (distance curve0.PointAtEnd curve1.PointAtStart)
                if distStart < distEnde then curve1.Domain.Min else curve1.Domain.Max

        let ok,a,b,pl = Curve.GetFilletPoints(curve0, curve1, radius, t0Base, t1Base)
        if not ok then failwithf "curveFilletPoints failed 3 curveA:'%A' curveB:'%A' radius:'%A' basePointA: %A basePointB: %A" curveA curveB radius basePointA basePointB
        curve0.PointAt(a), curve0.PointAt(b), pl
    (*
    def CurveFilletPoints(curve_id_0, curve_id_1, radius=1.0, base_point_0=None, base_point_1=None, return_points=True):
        '''Find points at which to cut a pair of curves so that a fillet of a
        specified radius fits. A fillet point is a pair of points (point0, point1)
        such that there is a circle of radius tangent to curve curve0 at point0 and
        tangent to curve curve1 at point1. Of all possible fillet points, this
        function returns the one which is the closest to the base point base_point_0,
        base_point_1. Distance from the base point is measured by the sum of arc
        lengths along the two curves. 
        Parameters:
          curve_id_0 (guid): identifier of the first curve object.
          curve_id_1 (guid): identifier of the second curve object.
          radius (number, optional): The fillet radius. If omitted, a radius
                         of 1.0 is specified.
          base_point_0 (point, optional): The base point on the first curve.
                         If omitted, the starting point of the curve is used.
          base_point_1 (point, optional): The base point on the second curve. If omitted,
                         the starting point of the curve is used.
          return_points (bool, optional): If True (Default), then fillet points are
                         returned. Otherwise, a fillet curve is created and
                         it's identifier is returned.
        Returns:
          list(point, point, point, vector, vector, vector): If return_points is True, then a list of point and vector values
            if successful. The list elements are as follows:
                [0]    A point on the first curve at which to cut (point).
                [1]    A point on the second curve at which to cut (point).
                [2]    The fillet plane's origin (point). This point is also
                        the center point of the fillet
                [3]    The fillet plane's X axis (vector).
                [4]    The fillet plane's Y axis (vector).
                [5]    The fillet plane's Z axis (vector).
          
          guid: If return_points is False, then the identifier of the fillet curve
                if successful.
          None: if not successful, or on error.
        '''
        curve0 = rhutil.coercecurve(curve_id_0, -1, True)
        curve1 = rhutil.coercecurve(curve_id_1, -1, True)
        t0_base = curve0.Domain.Min
        
        if base_point_0:
            rc = curve0.ClosestPoint(base_point_0, t0_base)
            if not rc[0]: return scriptcontext.errorhandler()
        
        t1_base = curve1.Domain.Min
        if base_point_1:
            rc = curve1.ClosestPoint(base_point_1, t1_base)
            if not rc[0]: return scriptcontext.errorhandler()
        r = radius if (radius and radius>0) else 1.0
        rc = Rhino.Geometry.Curve.GetFilletPoints(curve0, curve1, r, t0_base, t1_base)
        if rc[0]:
            point_0 = curve0.PointAt(rc[1])
            point_1 = curve1.PointAt(rc[2])
            return point_0, point_1, rc[3].Origin, rc[3].XAxis, rc[3].YAxis, rc[3].ZAxis
        return scriptcontext.errorhandler()
    *)


    ///<summary>Returns the plane at a parameter of a curve. The plane is based on the
    ///  tangent and curvature vectors at a parameter.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<param name="parameter">(float) Parameter to evaluate.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The plane at the specified parameter .</returns>
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
                failwithf "curveFrame failed.  curveId:'%A' parameter:'%A' segmentIndex:'%A'" curveId parameter segmentIndex
        let  rc, frame = curve.FrameAt(para)
        if rc && frame.IsValid then  frame
        else failwithf  "curveFrame failed.  curveId:'%A' parameter:'%A' segmentIndex:'%A'" curveId parameter segmentIndex
    (*
    def CurveFrame(curve_id, parameter, segment_index=-1):
        '''Returns the plane at a parameter of a curve. The plane is based on the
        tangent and curvature vectors at a parameter.
        Parameters:
          curve_id (guid): identifier of the curve object.
          parameter (number): parameter to evaluate.
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          plane: The plane at the specified parameter if successful.
          None: if not successful, or on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        domain = curve.Domain
        if not domain.IncludesParameter(parameter):
            tol = scriptcontext.doc.ModelAbsoluteTolerance
            if parameter>domain.Max and (parameter-domain.Max)<=tol:
                parameter = domain.Max
            elif parameter<domain.Min and (domain.Min-parameter)<=tol:
                parameter = domain.Min
            else:
                return scriptcontext.errorhandler()
        rc, frame = curve.FrameAt(parameter)
        if rc and frame.IsValid: return frame
        return scriptcontext.errorhandler()
    *)


    ///<summary>Returns the knot count of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if `curveId` identifies a polycurve.</param>
    ///<returns>(int) The number of knots .</returns>
    static member CurveKnotCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)  
        let  nc = curve.ToNurbsCurve()
        if notNull nc then  failwithf "curveKnotCount failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        nc.Knots.Count
    (*
    def CurveKnotCount(curve_id, segment_index=-1):
        '''Returns the knot count of a curve object.
        Parameters:
          curve_id (guid): identifier of the curve object.
          segment_index (number, optional): the curve segment if `curve_id` identifies a polycurve.
        Returns:
          number: The number of knots if successful.
          None: if not successful or on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        nc = curve.ToNurbsCurve()
        if not nc: return scriptcontext.errorhandler()
        return nc.Knots.Count
    *)


    ///<summary>Returns the knots, or knot vector, of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve.</param>
    ///<returns>(float seq) knot values .</returns>
    static member CurveKnots(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Collections.NurbsCurveKnotList =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)  
        let  nc = curve.ToNurbsCurve()
        if notNull nc then  failwithf "curveKnots failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        nc.Knots
        //[nc.Knots.[i] for i in range(nc.Knots.Count)]
    (*
    def CurveKnots(curve_id, segment_index=-1):
        '''Returns the knots, or knot vector, of a curve object
        Parameters:
          curve_id (guid): identifier of the curve object.
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve.
        Returns:
          list(number, ....): knot values if successful.
          None: if not successful or on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        nc = curve.ToNurbsCurve()
        if not nc: return scriptcontext.errorhandler()
        rc = [nc.Knots[i] for i in range(nc.Knots.Count)]
        return rc
    *)


    ///<summary>Returns the length of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="subDomain">(Interval) Optional, Default Value: <c>null</c>
    ///  List of two numbers identifying the sub-domain of the
    ///  curve on which the calculation will be performed. The two parameters
    ///  (sub-domain) must be non-decreasing. If omitted, the length of the
    ///  entire curve is returned.</param>
    ///<returns>(float) The length of the curve .</returns>
    static member CurveLength(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int, [<OPT;DEF(null)>]subDomain:Interval) : float =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        match box subDomain with
        | null -> curve.GetLength()
        | :? Interval as pl -> curve.GetLength(subDomain)
        | _ -> failwithf "curveLength failed on bad interval '%A' subDomain" subDomain
        
        
   
    (*
    def CurveLength(curve_id, segment_index=-1, sub_domain=None):
        '''Returns the length of a curve object.
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
          sub_domain ([number, number], optional): list of two numbers identifying the sub-domain of the
              curve on which the calculation will be performed. The two parameters
              (sub-domain) must be non-decreasing. If omitted, the length of the
              entire curve is returned.
        Returns:
          number: The length of the curve if successful.
          None: if not successful, or on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        if sub_domain:
            if len(sub_domain)==2:
                dom = Rhino.Geometry.Interval(sub_domain[0], sub_domain[1])
                return curve.GetLength(dom)
            return scriptcontext.errorhandler()
        return curve.GetLength()
    *)


    ///<summary>Returns the mid point of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D midpoint of the curve .</returns>
    static member CurveMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)  
        let  rc, t = curve.NormalizedLengthParameter(0.5)
        if rc then  curve.PointAt(t)
        else failwithf "curveMidPoint failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
    (*
    def CurveMidPoint(curve_id, segment_index=-1):
        '''Returns the mid point of a curve object.
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          point: The 3D midpoint of the curve if successful.
          None: if not successful, or on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc, t = curve.NormalizedLengthParameter(0.5)
        if rc: return curve.PointAt(t)
        return scriptcontext.errorhandler()
    *)


    ///<summary>Returns the normal direction of the plane in which a planar curve object lies.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if curveId identifies a polycurve</param>
    ///<returns>(Vector3d) The 3D normal vector .</returns>
    static member CurveNormal(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let tol = Doc.ModelAbsoluteTolerance
        let plane = ref Plane.WorldXY
        let rc = curve.TryGetPlane(plane,tol)
        if rc then  (!plane).Normal
        else failwithf "curveNormal failed. curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
    (*
    def CurveNormal(curve_id, segment_index=-1):
        '''Returns the normal direction of the plane in which a planar curve object lies.
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment if curve_id identifies a polycurve
        Returns:
          vector: The 3D normal vector if successful.
          None: if not successful, or on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        rc, plane = curve.TryGetPlane(tol)
        if rc: return plane.Normal
        return scriptcontext.errorhandler()
    *)


    ///<summary>Converts a curve parameter to a normalized curve parameter;
    ///  one that ranges between 0-1</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) The curve parameter to convert</param>
    ///<returns>(float) normalized curve parameter</returns>
    static member CurveNormalizedParameter(curveId:Guid, parameter:float) : float =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId  
        curve.Domain.NormalizedParameterAt(parameter)
    (*
    def CurveNormalizedParameter(curve_id, parameter):
        '''Converts a curve parameter to a normalized curve parameter;
        one that ranges between 0-1
        Parameters:
          curve_id (guid): identifier of the curve object
          parameter (number): the curve parameter to convert
        Returns:
          number: normalized curve parameter
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        return curve.Domain.NormalizedParameterAt(parameter)
    *)


    ///<summary>Converts a normalized curve parameter to a curve parameter;
    ///  one within the curve's domain</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) The normalized curve parameter to convert</param>
    ///<returns>(float) curve parameter</returns>
    static member CurveParameter(curveId:Guid, parameter:float) : float =
        let curve = RhinoScriptSyntax.CoerceCurve curveId  
        curve.Domain.ParameterAt(parameter)
    (*
    def CurveParameter(curve_id, parameter):
        '''Converts a normalized curve parameter to a curve parameter;
        one within the curve's domain
        Parameters:
          curve_id (guid): identifier of the curve object
          parameter (number): the normalized curve parameter to convert
        Returns:
          number: curve parameter
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        return curve.Domain.ParameterAt(parameter)
    *)


    ///<summary>Returns the perpendicular plane at a parameter of a curve. The result
    ///  is relatively parallel (zero-twisting) plane</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<returns>(Plane) Plane on success</returns>
    static member CurvePerpFrame(curveId:Guid, parameter:float) : Plane =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId  
        let  rc, plane = curve.PerpendicularFrameAt(parameter)
        if rc then  plane else failwithf "curvePerpFrame failed. curveId:'%A' parameter:'%f'" curveId parameter
    (*
    def CurvePerpFrame(curve_id, parameter):
        '''Returns the perpendicular plane at a parameter of a curve. The result
        is relatively parallel (zero-twisting) plane
        Parameters:
          curve_id (guid): identifier of the curve object
          parameter (number): parameter to evaluate
        Returns:
          plane: Plane on success
          None: on error
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        parameter = float(parameter)
        rc, plane = curve.PerpendicularFrameAt(parameter)
        if rc: return plane
    *)


    ///<summary>Returns the plane in which a planar curve lies. Note, this function works
    ///  only on planar curves.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The plane in which the curve lies .</returns>
    static member CurvePlane(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let tol = Doc.ModelAbsoluteTolerance
        let plane = ref Plane.WorldXY
        let rc = curve.TryGetPlane(plane,tol)
        if rc then  !plane
        else failwithf "curvePlane failed. curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
    (*
    def CurvePlane(curve_id, segment_index=-1):
        '''Returns the plane in which a planar curve lies. Note, this function works
        only on planar curves.
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          plane: The plane in which the curve lies if successful.
          None: if not successful, or on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        rc, plane = curve.TryGetPlane(tol)
        if rc: return plane
        return scriptcontext.errorhandler()
    *)


    ///<summary>Returns the control points count of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(int) Number of control points .</returns>
    static member CurvePointCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)  
        let mutable nc = curve.ToNurbsCurve()
        if notNull nc then  nc.Points.Count
        else failwithf "curvePointCount failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
    (*
    def CurvePointCount(curve_id, segment_index=-1):
        '''Returns the control points count of a curve object.
        Parameters:
          curve_id (guid) identifier of the curve object
          segment_index (number, optional): the curve segment if `curve_id` identifies a polycurve
        Returns:
          number: Number of control points if successful.
          None: if not successful
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        nc = curve.ToNurbsCurve()
        if nc: return nc.Points.Count
        return scriptcontext.errorhandler()
    *)


    ///<summary>Returns the control points, or control vertices, of a curve object.
    ///  If the curve is a rational NURBS curve, the euclidean control vertices
    ///  are returned.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d seq) the control points, or control vertices, of a curve object</returns>
    static member CurvePoints(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d [] =
        let  curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)  
        let  nc = curve.ToNurbsCurve()
        if isNull nc then  failwithf "curvePoints failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        [| for i=0 to nc.Points.Count-1 do yield nc.Points.[i].Location |]
    (*
    def CurvePoints(curve_id, segment_index=-1):
        '''Returns the control points, or control vertices, of a curve object.
        If the curve is a rational NURBS curve, the euclidean control vertices
        are returned.
        Parameters:
          curve_id (guid): the object's identifier
          segment_index (number, optional): the curve segment if `curve_id` identifies a polycurve
        Returns:
          list(point, ...): the control points, or control vertices, of a curve object
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        nc = curve.ToNurbsCurve()
        if nc is None: return scriptcontext.errorhandler()
        points = [nc.Points[i].Location for i in xrange(nc.Points.Count)]
        return points
    *)


    ///<summary>Returns the radius of curvature at a point on a curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="testPoint">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if curveId identifies a polycurve</param>
    ///<returns>(float) The radius of curvature at the point on the curve .</returns>
    static member CurveRadius(curveId:Guid, testPoint:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)  
        let mutable rc, t = curve.ClosestPoint(testPoint)//, 0.0)
        if not <| rc then  failwithf "curveRadius failed.  curveId:'%A' testPoint:'%A' segmentIndex:'%A'" curveId testPoint segmentIndex
        let mutable v = curve.CurvatureAt( t )
        let mutable k = v.Length
        if k>RhinoMath.ZeroTolerance then  1.0/k
        else failwithf "curveRadius failed.  curveId:'%A' testPoint:'%A' segmentIndex:'%A'" curveId testPoint segmentIndex
    (*
    def CurveRadius(curve_id, test_point, segment_index=-1):
        '''Returns the radius of curvature at a point on a curve.
        Parameters:
          curve_id (guid): identifier of the curve object
          test_point (point): sampling point
          segment_index (number, optional): the curve segment if curve_id identifies a polycurve
        Returns:
          number: The radius of curvature at the point on the curve if successful.
          None: if not successful, or on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        point = rhutil.coerce3dpoint(test_point, True)
        rc, t = curve.ClosestPoint(point, 0.0)
        if not rc: return scriptcontext.errorhandler()
        v = curve.CurvatureAt( t )
        k = v.Length
        if k>Rhino.RhinoMath.ZeroTolerance: return 1/k
        return scriptcontext.errorhandler()
    *)


    ///<summary>Adjusts the seam, or start/end, point of a closed curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) The parameter of the new start/end point.
    ///  Note, if successful, the resulting curve's
    ///  domain will start at `parameter`.</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
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
                    let dupeObj = Doc.Objects.Replace(curveId, dupe)
                    notNull dupeObj
            else
                false
    (*
    def CurveSeam(curve_id, parameter):
        '''Adjusts the seam, or start/end, point of a closed curve.
        Parameters:
          curve_id (guid): identifier of the curve object
          parameter (number): The parameter of the new start/end point.
                      Note, if successful, the resulting curve's
                      domain will start at `parameter`.
        Returns:
          bool: True or False indicating success or failure.
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if (not curve.IsClosed or not curve.Domain.IncludesParameter(parameter)):
            return False
        dupe = curve.Duplicate()
        if dupe:
            dupe.ChangeClosedCurveSeam(parameter)
            curve_id = rhutil.coerceguid(curve_id)
            dupe_obj = scriptcontext.doc.Objects.Replace(curve_id, dupe)
            return dupe_obj is not None
        return False
    *)


    ///<summary>Returns the start point of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="point">(Point3d) Optional, Default Value: <c>null</c>
    ///New start point</param>
    ///<returns>(Point3d) The 3D starting point of the curve .</returns>
    static member CurveStartPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        curve.PointAtStart

    ///<summary>Sets the start point of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="point">(Point3d) New start point</param>
    ///<returns>(unit)</returns>
    static member CurveStartPoint(curveId:Guid, point:Point3d) : unit =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)  
        if not <|curve.SetStartPoint(point) then failwithf "CurveStartPoint failed on '%A' and '%A'" point curveId
        Doc.Objects.Replace(curveId, curve) |> ignore
        Doc.Views.Redraw()

    (*
    def CurveStartPoint(curve_id, segment_index=-1, point=None):
        '''Returns the start point of a curve object
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
          point (point, optional): new start point
        Returns:
          point: The 3D starting point of the curve if successful.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc = curve.PointAtStart
        if point:
            point = rhutil.coerce3dpoint(point, True)
            if point and curve.SetStartPoint(point):
                curve_id = rhutil.coerceguid(curve_id, True)
                scriptcontext.doc.Objects.Replace(curve_id, curve)
                scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Calculates intersection of a curve object with a surface object.
    ///  Note, this function works on the untrimmed portion of the surface.</summary>
    ///<param name="curveId">(Guid) The identifier of the first curve object.</param>
    ///<param name="surfaceId">(Guid) The identifier of the second curve object. If omitted,
    ///  the a self-intersection test will be performed on curve.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>-1</c>
    ///The absolute tolerance in drawing units. If omitted,
    ///  the document's current absolute tolerance is used.</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>-1</c>
    ///Angle tolerance in degrees. The angle
    ///  tolerance is used to determine when the curve is tangent to the
    ///  surface. If omitted, the document's current angle tolerance is used.</param>
    ///<returns>(List of int*Point3d*Point3d*Point3d*Point3d*float*float*float*float) of intersection information .
    ///  The list will contain one or more of the following elements:
    ///    Element Type     Description
    ///    [n][0]  Number   The intersection event type, either Point(1) or Overlap(2).
    ///    [n][1]  Point3d  If the event type is Point(1), then the intersection point
    ///      on the first curve. If the event type is Overlap(2), then
    ///      intersection start point on the first curve.
    ///    [n][2]  Point3d  If the event type is Point(1), then the intersection point
    ///      on the first curve. If the event type is Overlap(2), then
    ///      intersection end point on the first curve.
    ///    [n][3]  Point3d  If the event type is Point(1), then the intersection point
    ///      on the second curve. If the event type is Overlap(2), then
    ///      intersection start point on the surface.
    ///    [n][4]  Point3d  If the event type is Point(1), then the intersection point
    ///      on the second curve. If the event type is Overlap(2), then
    ///      intersection end point on the surface.
    ///    [n][5]  Number   If the event type is Point(1), then the first curve parameter.
    ///      If the event type is Overlap(2), then the start value of the
    ///      first curve parameter range.
    ///    [n][6]  Number   If the event type is Point(1), then the first curve parameter.
    ///      If the event type is Overlap(2), then the end value of the
    ///      curve parameter range.
    ///    [n][7]  Number   If the event type is Point(1), then the U surface parameter.
    ///      If the event type is Overlap(2), then the U surface parameter
    ///      for curve at (n, 5).
    ///    [n][8]  Number   If the event type is Point(1), then the V surface parameter.
    ///      If the event type is Overlap(2), then the V surface parameter
    ///      for curve at (n, 5).
    ///    [n][9]  Number   If the event type is Point(1), then the U surface parameter.
    ///      If the event type is Overlap(2), then the U surface parameter
    ///      for curve at (n, 6).
    ///    [n][10] Number   If the event type is Point(1), then the V surface parameter.
    ///      If the event type is Overlap(2), then the V surface parameter
    ///      for curve at (n, 6).</returns>
    static member CurveSurfaceIntersection(curveId:Guid, surfaceId:Guid, [<OPT;DEF(-1)>]tolerance:float, [<OPT;DEF(-1)>]angleTolerance:float) : (int*Point3d*Point3d*Point3d*Point3d*float*float*float*float*float*float) ResizeArray =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let surface = RhinoScriptSyntax.CoerceSurface surfaceId  
        let tolerance0 = max tolerance Doc.ModelAbsoluteTolerance
        let angleTolerance0 = max (toRadians(angleTolerance)) Doc.ModelAngleToleranceRadians
        let  rc = Intersect.Intersection.CurveSurface(curve, surface, tolerance0, angleTolerance0)
        if isNull rc then failwithf "curveSurfaceIntersection failed. (surfaceId:%A) (curveId:%A) (angleTolerance:%f) (tolerance:%f) " surfaceId curveId angleTolerance tolerance
        let events = ResizeArray()
        for i=0 to rc.Count - 1 do
            let eventType = if rc.[i].IsOverlap then 2 else 1
            let item = rc.[i]
            let oa = item.OverlapA
            let u,v = item.SurfaceOverlapParameter()
            let e = eventType, item.PointA, item.PointA2, item.PointB, item.PointB2, oa.[0], oa.[1], u.[0], u.[1], v.[0], v.[1]
            events.Add(e)
        events
      
            
    (*
    def CurveSurfaceIntersection(curve_id, surface_id, tolerance=-1, angle_tolerance=-1):
        '''Calculates intersection of a curve object with a surface object.
        Note, this function works on the untrimmed portion of the surface.
        Parameters:
          curve_id (guid): The identifier of the first curve object.
          surface_id (guid): The identifier of the second curve object. If omitted,
              the a self-intersection test will be performed on curve.
          tolerance (number, optional): The absolute tolerance in drawing units. If omitted,
              the document's current absolute tolerance is used.
          angle_tolerance (number, optional) angle tolerance in degrees. The angle
              tolerance is used to determine when the curve is tangent to the
              surface. If omitted, the document's current angle tolerance is used.
        Returns:
          list(list(point, point, point, point, number, number, number, number, number, number), ...): of intersection information if successful.
            The list will contain one or more of the following elements:
              Element Type     Description
              [n][0]  Number   The intersection event type, either Point(1) or Overlap(2).
              [n][1]  Point3d  If the event type is Point(1), then the intersection point
                              on the first curve. If the event type is Overlap(2), then
                              intersection start point on the first curve.
              [n][2]  Point3d  If the event type is Point(1), then the intersection point
                              on the first curve. If the event type is Overlap(2), then
                              intersection end point on the first curve.
              [n][3]  Point3d  If the event type is Point(1), then the intersection point
                              on the second curve. If the event type is Overlap(2), then
                              intersection start point on the surface.
              [n][4]  Point3d  If the event type is Point(1), then the intersection point
                              on the second curve. If the event type is Overlap(2), then
                              intersection end point on the surface.
              [n][5]  Number   If the event type is Point(1), then the first curve parameter.
                              If the event type is Overlap(2), then the start value of the
                              first curve parameter range.
              [n][6]  Number   If the event type is Point(1), then the first curve parameter.
                              If the event type is Overlap(2), then the end value of the
                              curve parameter range.
              [n][7]  Number   If the event type is Point(1), then the U surface parameter.
                              If the event type is Overlap(2), then the U surface parameter
                              for curve at (n, 5).
              [n][8]  Number   If the event type is Point(1), then the V surface parameter.
                              If the event type is Overlap(2), then the V surface parameter
                              for curve at (n, 5).
              [n][9]  Number   If the event type is Point(1), then the U surface parameter.
                              If the event type is Overlap(2), then the U surface parameter
                              for curve at (n, 6).
              [n][10] Number   If the event type is Point(1), then the V surface parameter.
                              If the event type is Overlap(2), then the V surface parameter
                              for curve at (n, 6).
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        surface = rhutil.coercesurface(surface_id, True)
        if tolerance is None or tolerance<0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        if angle_tolerance is None or angle_tolerance<0:
            angle_tolerance = scriptcontext.doc.ModelAngleToleranceRadians
        else:
            angle_tolerance = math.radians(angle_tolerance)
        rc = Rhino.Geometry.Intersect.Intersection.CurveSurface(curve, surface, tolerance, angle_tolerance)
        if rc:
            events = []
            for i in xrange(rc.Count):
                event_type = 2 if rc[i].IsOverlap else 1
                item = rc[i]
                oa = item.OverlapA
                u,v = item.SurfaceOverlapParameter()
                e = (event_type, item.PointA, item.PointA2, item.PointB, item.PointB2, oa[0], oa[1], u[0], u[1], v[0], v[1])
                events.append(e)
            return events
    *)


    ///<summary>Returns a 3D vector that is the tangent to a curve at a parameter.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) A 3D vector .</returns>
    static member CurveTangent(curveId:Guid, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        let curve = RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex)  
        let mutable rc = Point3d.Unset
        if curve.Domain.IncludesParameter(parameter) then
            curve.TangentAt(parameter)
        else
            failwithf "curveTangent failed.  curveId:'%A' parameter:'%A' segmentIndex:'%A'" curveId parameter segmentIndex
    (*
    def CurveTangent(curve_id, parameter, segment_index=-1):
        '''Returns a 3D vector that is the tangent to a curve at a parameter.
        Parameters:
          curve_id (guid): identifier of the curve object
          parameter (number) parameter to evaluate
          segment_index (number, optional) the curve segment index if `curve_id` identifies a polycurve
        Returns:
          vector: A 3D vector if successful.
          None: on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc = Rhino.Geometry.Point3d.Unset
        if curve.Domain.IncludesParameter(parameter):
            return curve.TangentAt(parameter)
        return scriptcontext.errorhandler()
    *)


    ///<summary>Returns list of weights that are assigned to the control points of a curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The weight values of the curve .</returns>
    static member CurveWeights(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float []=
        let nc =
            match RhinoScriptSyntax.CoerceCurve (curveId, segmentIndex) with  
            | :? NurbsCurve as nc -> nc
            | c -> c.ToNurbsCurve()
        if isNull nc then  failwithf "curveWeights failed.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
        [| for pt in nc.Points -> pt.Weight |]
    (*
    def CurveWeights(curve_id, segment_index=-1):
        '''Returns list of weights that are assigned to the control points of a curve
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          number: The weight values of the curve if successful.
          None: if not successful, or on error.
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        nc = curve
        if type(curve) is not Rhino.Geometry.NurbsCurve:
            nc = curve.ToNurbsCurve()
        if nc is None: return scriptcontext.errorhandler()
        return [pt.Weight for pt in nc.Points]
    *)



    ///<summary>Divides a curve object into a specified number of segments.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segments">(int) The number of segments.</param>
    ///<returns>(float seq) Array containing division curve parameters.</returns>
    static member DivideCurveIntoPoints(curveId:Guid, segments:int) : Point3d [] =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId  
        let pts = ref (Array.zeroCreate (segments+1))
        let rc = curve.DivideByCount(segments, true, pts)
        if isNull rc then  failwithf "divideCurve failed.  curveId:'%A' segments:'%A'" curveId segments
        !pts


    ///<summary>Divides a curve object into a specified number of segments.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segments">(int) The number of segments.</param>
    ///<returns>(float []) array containing 3D division points.</returns>
    static member DivideCurve(curveId:Guid, segments:int) : float [] =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId  
        let rc = curve.DivideByCount(segments, true)
        if isNull rc then  failwithf "divideCurve failed.  curveId:'%A' segments:'%A'" curveId segments
        rc


    ///<summary>Divides a curve such that the linear distance between the points is equal.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="distance">(float) Linear distance between division points</param>
    ///<returns>(float []) array containing 3D division points.</returns>
    static member DivideCurveEquidistant(curveId:Guid, distance:float) : Point3d [] =
        let  curve = RhinoScriptSyntax.CoerceCurve curveId  
        let  points = curve.DivideEquidistant(distance)
        if isNull points then  failwithf "divideCurveEquidistant failed.  curveId:'%A' distance:'%A'" curveId distance
        points
        //let  tvals = ResizeArray()
        //for point in points do
        //    let mutable rc, t = curve.ClosestPoint(point)
        //    tvals.Add(t)
        //tvals


    ///<summary>Divides a curve object into segments of a specified length.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="length">(float) The length of each segment.</param>
    ///<returns>(Point3d []) a list containing division points.</returns>
    static member DivideCurveLengthIntoPoints(curveId:Guid, length:float) : Point3d [] =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let rc = curve.DivideByLength(length, true)
        if isNull rc then  failwithf "divideCurveLength failed.  curveId:'%A' length:'%A'" curveId length
        [| for t in rc -> curve.PointAt(t) |]

    ///<summary>Divides a curve object into segments of a specified length.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="length">(float) The length of each segment.</param>
    ///<returns>(float []) a list containing division parameters.</returns>
    static member DivideCurveLength(curveId:Guid, length:float) : float [] =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let rc = curve.DivideByLength(length, true)
        if isNull rc then  failwithf "divideCurveLength failed.  curveId:'%A' length:'%A'" curveId length
        rc


    ///<summary>Returns the center point of an elliptical-shaped curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<returns>(Point3d) The 3D center point of the ellipse .</returns>
    static member EllipseCenterPoint(curveId:Guid) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let rc, ellipse = curve.TryGetEllipse()
        if not <| rc then  failwithf "Curve is not an ellipse.  curveId:'%A'" curveId
        ellipse.Plane.Origin
    (*
    def EllipseCenterPoint(curve_id):
        '''Returns the center point of an elliptical-shaped curve object.
        Parameters:
          curve_id (guid): identifier of the curve object.
        Returns:
          point: The 3D center point of the ellipse if successful.
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        rc, ellipse = curve.TryGetEllipse()
        if not rc: raise ValueError("curve is not an ellipse")
        return ellipse.Plane.Origin
    *)


    ///<summary>Returns the quadrant points of an elliptical-shaped curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four points identifying the quadrants of the ellipse</returns>
    static member EllipseQuadPoints(curveId:Guid) : Point3d * Point3d * Point3d * Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let rc, ellipse = curve.TryGetEllipse()
        if not <| rc then  failwithf "Curve is not an ellipse.  curveId:'%A'" curveId
        let origin = ellipse.Plane.Origin;
        let xaxis = ellipse.Radius1 * ellipse.Plane.XAxis;
        let yaxis = ellipse.Radius2 * ellipse.Plane.YAxis;
        (origin-xaxis, origin+xaxis, origin-yaxis, origin+yaxis)
    (*
    def EllipseQuadPoints(curve_id):
        '''Returns the quadrant points of an elliptical-shaped curve object.
        Parameters:
          curve_id (guid): identifier of the curve object.
        Returns:
          list(point, point, point, point): Four points identifying the quadrants of the ellipse
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        rc, ellipse = curve.TryGetEllipse()
        if not rc: raise ValueError("curve is not an ellipse")
        origin = ellipse.Plane.Origin;
        xaxis = ellipse.Radius1 * ellipse.Plane.XAxis;
        yaxis = ellipse.Radius2 * ellipse.Plane.YAxis;
        return (origin-xaxis, origin+xaxis, origin-yaxis, origin+yaxis)
    *)


    ///<summary>Evaluates a curve at a parameter and returns a 3D point</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="t">(float) The parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) a 3-D point</returns>
    static member EvaluateCurve(curveId:Guid, t:float, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        curve.PointAt(t)
    (*
    def EvaluateCurve(curve_id, t, segment_index=-1):
        '''Evaluates a curve at a parameter and returns a 3D point
        Parameters:
          curve_id (guid): identifier of the curve object
          t (number): the parameter to evaluate
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          point: a 3-D point if successful
          None: if not successful
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        return curve.PointAt(t)
    *)


    ///<summary>Explodes, or un-joins, one curves. Polycurves will be exploded into curve
    ///  segments. Polylines will be exploded into line segments. ExplodeCurves will
    ///  return the curves in topological order.</summary>
    ///<param name="curveId">(Guid) The curve object to explode.</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input objects after exploding if True.</param>
    ///<returns>(Guid seq) identifying the newly created curve objects</returns>
    static member ExplodeCurve(curveId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid ResizeArray =
        let rc = ResizeArray()        
        let curve = RhinoScriptSyntax.CoerceCurve curveId 
        let pieces = curve.DuplicateSegments()
        if notNull pieces then
            for piece in pieces do
                rc.Add(Doc.Objects.AddCurve(piece))
            if deleteInput then
                Doc.Objects.Delete(curveId,true) |>ignore
        if rc.Count>0 then  Doc.Views.Redraw()
        rc




    (*
    def ExplodeCurves(curve_ids, delete_input=False):
        '''Explodes, or un-joins, one curves. Polycurves will be exploded into curve
        segments. Polylines will be exploded into line segments. ExplodeCurves will
        return the curves in topological order. 
        Parameters:
          curve_ids (guid): the curve object(s) to explode.
          delete_input (bool, optional): Delete input objects after exploding if True.
        Returns:
          list(guid, ...): identifying the newly created curve objects
        '''
        if( type(curve_ids) is list or type(curve_ids) is tuple ): pass
        else: curve_ids = [curve_ids]
        rc = []
        for id in curve_ids:
            curve = rhutil.coercecurve(id, -1, True)
            pieces = curve.DuplicateSegments()
            if pieces:
                for piece in pieces:
                    rc.append(scriptcontext.doc.Objects.AddCurve(piece))
                if delete_input:
                    id = rhutil.coerceguid(id, True)
                    scriptcontext.doc.Objects.Delete(id, True)
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Extends a non-closed curve object by a line, arc, or smooth extension
    ///  until it intersects a collection of objects.</summary>
    ///<param name="curveId">(Guid) Identifier of curve to extend</param>
    ///<param name="extensionType">(int) 0 = line
    ///  1 = arc
    ///  2 = smooth</param>
    ///<param name="side">(int) 0=extend from the start of the curve
    ///  1=extend from the end of the curve
    ///  2=extend from both the start and the end of the curve</param>
    ///<param name="boundarycurveIds">(Guid seq) Curve, surface, and polysurface objects to extend to</param>
    ///<param name="replaceInput">(bool) Optional, Default Value <c>false</c> Replace input or add new? </param> 
    ///<returns>(Guid) The identifier of the new object or orignal curve ( depending on 'replaceInput').</returns>
    static member ExtendCurve(curveId:Guid, extensionType:int, side:int, boundarycurveIds:Guid seq,[<OPT;DEF(false)>]replaceInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let mutable extensionTypet = CurveExtensionStyle.Line
        if extensionType=0 then  extensionTypet <- CurveExtensionStyle.Line
        elif extensionType=1 then extensionTypet <- CurveExtensionStyle.Arc
        elif extensionType=2 then extensionTypet <- CurveExtensionStyle.Smooth
        else failwithf "extendCurve ExtensionType must be 0, 1, || 2.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds

        let mutable sidet = CurveEnd.Start
        if side=0 then  sidet <- CurveEnd.Start
        elif side=1 then  sidet <- CurveEnd.End
        elif side=2 then sidet <- CurveEnd.Both
        else failwithf "extendCurve Side must be 0, 1, || 2.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds

        let rhobjs = [| for id in boundarycurveIds -> RhinoScriptSyntax.CoerceRhinoObject(id) |]  
        if isNull rhobjs then  failwithf "extendCurve BoundarycurveIds failed. they must contain at least one item.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds
        let geometry = [| for obj in rhobjs -> obj.Geometry |]
        let newcurve = curve.Extend(sidet, extensionTypet, geometry)
        if notNull newcurve && newcurve.IsValid then
            if replaceInput then 
              if Doc.Objects.Replace(curveId, newcurve) then
                 Doc.Views.Redraw()
                 curveId
              else
                 failwithf "extendCurve failed.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds
            else
              let g=Doc.Objects.AddCurve(newcurve)
              Doc.Views.Redraw()
              g              
        else
            failwithf "extendCurve failed.  curveId:'%A' extensionType:'%A' side:'%A' boundarycurveIds:'%A'" curveId extensionType side boundarycurveIds
    (*
    def ExtendCurve(curve_id, extension_type, side, boundary_object_ids):
        '''Extends a non-closed curve object by a line, arc, or smooth extension
        until it intersects a collection of objects.
        Parameters:
          curve_id (guid): identifier of curve to extend
          extension_type (number):
            0 = line
            1 = arc
            2 = smooth
          side (number):
            0=extend from the start of the curve
            1=extend from the end of the curve
            2=extend from both the start and the end of the curve
          boundary_object_ids (guid): curve, surface, and polysurface objects to extend to
        Returns:
          guid: The identifier of the new object if successful.
          None: if not successful
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if extension_type==0: extension_type = Rhino.Geometry.CurveExtensionStyle.Line
        elif extension_type==1: extension_type = Rhino.Geometry.CurveExtensionStyle.Arc
        elif extension_type==2: extension_type = Rhino.Geometry.CurveExtensionStyle.Smooth
        else: raise ValueError("extension_type must be 0, 1, or 2")
        
        if side==0: side = Rhino.Geometry.CurveEnd.Start
        elif side==1: side = Rhino.Geometry.CurveEnd.End
        elif side==2: side = Rhino.Geometry.CurveEnd.Both
        else: raise ValueError("side must be 0, 1, or 2")
        
        rhobjs = [rhutil.coercerhinoobject(id) for id in boundary_object_ids]
        if not rhobjs: raise ValueError("boundary_object_ids must contain at least one item")
        geometry = [obj.Geometry for obj in rhobjs]
        newcurve = curve.Extend(side, extension_type, geometry)
        if newcurve and newcurve.IsValid:
            curve_id = rhutil.coerceguid(curve_id, True)
            if scriptcontext.doc.Objects.Replace(curve_id, newcurve):
                scriptcontext.doc.Views.Redraw()
                return curve_id
        return scriptcontext.errorhandler()
    *)


    ///<summary>Extends a non-closed curve by a line, arc, or smooth extension for a
    ///  specified distance</summary>
    ///<param name="curveId">(Guid) Curve to extend</param>
    ///<param name="extensionType">(int) 0 = line
    ///  1 = arc
    ///  2 = smooth</param>
    ///<param name="side">(int) 0=extend from start of the curve
    ///  1=extend from end of the curve
    ///  2=Extend from both ends</param>
    ///<param name="length">(float) Distance to extend</param>
    ///<returns>(Guid) The identifier of the new object</returns>
    static member ExtendCurveLength(curveId:Guid, extensionType:int, side:int, length:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let mutable extensionTypet = CurveExtensionStyle.Line
        if extensionType=0 then  extensionTypet <- CurveExtensionStyle.Line
        elif extensionType=1 then extensionTypet <- CurveExtensionStyle.Arc
        elif extensionType=2 then extensionTypet <- CurveExtensionStyle.Smooth
        else failwithf "extendCurveLength ExtensionType must be 0, 1, || 2.  curveId:'%A' extensionType:'%A' side:'%A' length:'%A'" curveId extensionType side length

        let mutable sidet = CurveEnd.Start
        if side=0 then  sidet <- CurveEnd.Start
        elif side=1 then  sidet <- CurveEnd.End
        elif side=2 then sidet <- CurveEnd.Both
        else failwithf "extendCurveLength Side must be 0, 1, || 2.  curveId:'%A' extensionType:'%A' side:'%A' length:'%A'" curveId extensionType side length

        let newcurve =
            if length<0. then curve.Trim(sidet, -length)
            else curve.Extend(sidet, length, extensionTypet)

        if notNull newcurve && newcurve.IsValid then
            if Doc.Objects.Replace(curveId, newcurve) then
                Doc.Views.Redraw()
                curveId
            else failwithf "extendCurveLength failed.  curveId:'%A' extensionType:'%A' side:'%A' length:'%A'" curveId extensionType side length
        else failwithf "extendCurveLength failed.  curveId:'%A' extensionType:'%A' side:'%A' length:'%A'" curveId extensionType side length
    (*
    def ExtendCurveLength(curve_id, extension_type, side, length):
        '''Extends a non-closed curve by a line, arc, or smooth extension for a
        specified distance
        Parameters:
          curve_id (guid): curve to extend
          extension_type (number):
            0 = line
            1 = arc
            2 = smooth
          side (number):
            0=extend from start of the curve
            1=extend from end of the curve
            2=Extend from both ends
          length (number): distance to extend
        Returns:
          guid: The identifier of the new object
          None: if not successful
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if extension_type==0: extension_type = Rhino.Geometry.CurveExtensionStyle.Line
        elif extension_type==1: extension_type = Rhino.Geometry.CurveExtensionStyle.Arc
        elif extension_type==2: extension_type = Rhino.Geometry.CurveExtensionStyle.Smooth
        else: raise ValueError("extension_type must be 0, 1, or 2")
        
        if side==0: side = Rhino.Geometry.CurveEnd.Start
        elif side==1: side = Rhino.Geometry.CurveEnd.End
        elif side==2: side = Rhino.Geometry.CurveEnd.Both
        else: raise ValueError("side must be 0, 1, or 2")
        newcurve = None
        if length<0: newcurve = curve.Trim(side, -length)
        else: newcurve = curve.Extend(side, length, extension_type)
        if newcurve and newcurve.IsValid:
            curve_id = rhutil.coerceguid(curve_id, True)
            if scriptcontext.doc.Objects.Replace(curve_id, newcurve):
                scriptcontext.doc.Views.Redraw()
                return curve_id
        return scriptcontext.errorhandler()
    *)


    ///<summary>Extends a non-closed curve by smooth extension to a point</summary>
    ///<param name="curveId">(Guid) Curve to extend</param>
    ///<param name="side">(int) 0=extend from start of the curve
    ///  1=extend from end of the curve</param>
    ///<param name="point">(Point3d) Point to extend to</param>
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member ExtendCurvePoint(curveId:Guid, side:int, point:Point3d) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 

        let mutable sidet = CurveEnd.Start
        if side=0 then  sidet <- CurveEnd.Start
        elif side=1 then  sidet <- CurveEnd.End
        elif side=2 then sidet <- CurveEnd.Both
        else failwithf "Side must be 0, 1, || 2.  curveId:'%A' side:'%A' point:'%A'" curveId side point

        let extensionType = CurveExtensionStyle.Smooth
        let newcurve = curve.Extend(sidet, extensionType, point)
        if notNull newcurve && newcurve.IsValid then
            if Doc.Objects.Replace( curveId, newcurve ) then
                Doc.Views.Redraw()
                curveId
            else failwithf "extendCurvePoint failed.  curveId:'%A' side:'%A' point:'%A'" curveId side point
        else failwithf "extendCurvePoint failed.  curveId:'%A' side:'%A' point:'%A'" curveId side point
    (*
    def ExtendCurvePoint(curve_id, side, point):
        '''Extends a non-closed curve by smooth extension to a point
        Parameters:
          curve_id (guid): curve to extend
          side (number):
            0=extend from start of the curve
            1=extend from end of the curve
          point (guid|point): point to extend to
        Returns:
          guid: The identifier of the new object if successful.
          None: if not successful, or on error.
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        point = rhutil.coerce3dpoint(point, True)
        
        if side==0: side = Rhino.Geometry.CurveEnd.Start
        elif side==1: side = Rhino.Geometry.CurveEnd.End
        elif side==2: side = Rhino.Geometry.CurveEnd.Both
        else: raise ValueError("side must be 0, 1, or 2")
        
        extension_type = Rhino.Geometry.CurveExtensionStyle.Smooth
        newcurve = curve.Extend(side, extension_type, point)
        if newcurve and newcurve.IsValid:
            curve_id = rhutil.coerceguid(curve_id, True)
            if scriptcontext.doc.Objects.Replace( curve_id, newcurve ):
                scriptcontext.doc.Views.Redraw()
                return curve_id
        return scriptcontext.errorhandler()
    *)


    ///<summary>Fairs a curve. Fair works best on degree 3 (cubic) curves. Fair attempts
    ///  to remove large curvature variations while limiting the geometry changes to
    ///  be no more than the specified tolerance. Sometimes several applications of
    ///  this method are necessary to remove nasty curvature problems.</summary>
    ///<param name="curveId">(Guid) Curve to fair</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>1.0</c>
    ///Fairing tolerance</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member FairCurve(curveId:Guid, [<OPT;DEF(1.0)>]tolerance:float) : bool =
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
    (*
    def FairCurve(curve_id, tolerance=1.0):
        '''Fairs a curve. Fair works best on degree 3 (cubic) curves. Fair attempts
        to remove large curvature variations while limiting the geometry changes to
        be no more than the specified tolerance. Sometimes several applications of
        this method are necessary to remove nasty curvature problems.
        Parameters:
          curve_id (guid): curve to fair
          tolerance (number, optional): fairing tolerance
        Returns:
          bool: True or False indicating success or failure
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        angle_tol = 0.0
        clamp = 0
        if curve.IsPeriodic:
            curve = curve.ToNurbsCurve()
            clamp = 1
        newcurve = curve.Fair(tolerance, angle_tol, clamp, clamp, 100)
        if not newcurve: return False
        curve_id = rhutil.coerceguid(curve_id, True)
        if scriptcontext.doc.Objects.Replace(curve_id, newcurve):
            scriptcontext.doc.Views.Redraw()
            return True
        return False
    *)


    ///<summary>Reduces number of curve control points while maintaining the curve's same
    ///  general shape. Use this function for replacing curves with many control
    ///  points. For more information, see the Rhino help for the FitCrv command.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///The curve degree, which must be greater than 1.
    ///  The default is 3.</param>
    ///<param name="distanceTolerance">(float) Optional, Default Value: <c>-1</c>
    ///The fitting tolerance. If distanceTolerance
    ///  is not specified or <= 0.0, the document absolute tolerance is used.</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>-1</c>
    ///The kink smoothing tolerance in degrees. If
    ///  angleTolerance is 0.0, all kinks are smoothed. If angleTolerance
    ///  is > 0.0, kinks smaller than angleTolerance are smoothed. If
    ///  angleTolerance is not specified or < 0.0, the document angle
    ///  tolerance is used for the kink smoothing.</param>
    ///<returns>(Guid) The identifier of the new object</returns>
    static member FitCurve(curveId:Guid, [<OPT;DEF(3)>]degree:int, [<OPT;DEF(-1)>]distanceTolerance:float, [<OPT;DEF(-1)>]angleTolerance:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let distanceTolerance0 = max distanceTolerance Doc.ModelAbsoluteTolerance
        let angleTolerance0 = max (toRadians angleTolerance) Doc.ModelAngleToleranceRadians
        let nc = curve.Fit(degree, distanceTolerance0, angleTolerance0)
        if notNull nc then
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)  
            let mutable rc = Guid.Empty
            if notNull rhobj then
                rc <- Doc.Objects.AddCurve(nc, rhobj.Attributes)
            else
                rc <- Doc.Objects.AddCurve(nc)
            if rc = Guid.Empty then  failwithf "fitCurve Unable to add curve to document.  curveId:'%A' degree:'%A' distanceTolerance:'%A' angleTolerance:'%A'" curveId degree distanceTolerance angleTolerance
            Doc.Views.Redraw()
            rc
        else
            failwithf "fitCurve failed.  curveId:'%A' degree:'%A' distanceTolerance:'%A' angleTolerance:'%A'" curveId degree distanceTolerance angleTolerance
    (*
    def FitCurve(curve_id, degree=3, distance_tolerance=-1, angle_tolerance=-1):
        '''Reduces number of curve control points while maintaining the curve's same
        general shape. Use this function for replacing curves with many control
        points. For more information, see the Rhino help for the FitCrv command.
        Parameters:
          curve_id (guid): Identifier of the curve object
          degree (number, optional): The curve degree, which must be greater than 1.
                         The default is 3.
          distance_tolerance (number, optional): The fitting tolerance. If distance_tolerance
              is not specified or <= 0.0, the document absolute tolerance is used.
          angle_tolerance (number, optional): The kink smoothing tolerance in degrees. If
              angle_tolerance is 0.0, all kinks are smoothed. If angle_tolerance
              is > 0.0, kinks smaller than angle_tolerance are smoothed. If
              angle_tolerance is not specified or < 0.0, the document angle
              tolerance is used for the kink smoothing.
        Returns:
          guid: The identifier of the new object
          None: if not successful, or on error.
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if distance_tolerance is None or distance_tolerance<0:
            distance_tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        if angle_tolerance is None or angle_tolerance<0:
            angle_tolerance = scriptcontext.doc.ModelAngleToleranceRadians
        nc = curve.Fit(degree, distance_tolerance, angle_tolerance)
        if nc:
            rhobj = rhutil.coercerhinoobject(curve_id)
            rc = None
            if rhobj:
                rc = scriptcontext.doc.Objects.AddCurve(nc, rhobj.Attributes)
            else:
                rc = scriptcontext.doc.Objects.AddCurve(nc)
            if rc==System.Guid.Empty: raise Exception("Unable to add curve to document")
            scriptcontext.doc.Views.Redraw()
            return rc
        return scriptcontext.errorhandler()
    *)


    ///<summary>Inserts a knot into a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter on the curve</param>
    ///<param name="symmetrical">(bool) Optional, Default Value: <c>false</c>
    ///If True, then knots are added on both sides of
    ///  the center of the curve</param>
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
                    let mutable rc = nc.Knots.InsertKnot(t,1)
                    if rc && symmetrical then
                        let domain = nc.Domain
                        let tSym = domain.T1 - t + domain.T0
                        if abs(tSym)>RhinoMath.SqrtEpsilon then
                            rc <- nc.Knots.InsertKnot(tSym,1)
                            if rc then  Doc.Views.Redraw()
                    if rc then
                        rc <- Doc.Objects.Replace(curveId, nc)
                        if rc then  Doc.Views.Redraw()
                rc
    (*
    def InsertCurveKnot(curve_id, parameter, symmetrical=False ):
        '''Inserts a knot into a curve object
        Parameters:
          curve_id (guid): identifier of the curve object
          parameter (number): parameter on the curve
          symmetrical (bool, optional): if True, then knots are added on both sides of
              the center of the curve
        Returns:
          bool: True or False indicating success or failure
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if not curve.Domain.IncludesParameter(parameter): return False
        nc = curve.ToNurbsCurve()
        if not nc: return False
        rc, t = curve.GetNurbsFormParameterFromCurveParameter(parameter)
        if rc:
            rc = nc.Knots.InsertKnot(t,1)
            if rc and symmetrical:
                domain = nc.Domain
                t_sym = domain.T1 - t + domain.T0
                if abs(t_sym)>Rhino.RhinoMath.SqrtEpsilon:
                    rc = nc.Knots.InsertKnot(t_sym,1)
            if rc:
                curve_id = rhutil.coerceguid(curve_id)
                rc = scriptcontext.doc.Objects.Replace(curve_id, nc)
                if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Verifies an object is an open arc curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///  If the curve is not a circle, then the tolerance used
    ///  to determine whether or not the NURBS form of the curve has the
    ///  properties of a arc. If omitted, Rhino's RhinoMath.ZeroTolerance is used</param>
    ///<returns>(bool) True or False</returns>
    static member IsArc(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = max RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve(curveId,segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsArc(tol) && not curve.IsClosed
    (*
    def IsArc(curve_id, segment_index=-1):
        '''Verifies an object is an open arc curve
        Parameters:
          curve_id (guid): Identifier of the curve object
          segment_index (number): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        return curve.IsArc() and not curve.IsClosed
    *)


    ///<summary>Verifies an object is a circle curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///If the curve is not a circle, then the tolerance used
    ///  to determine whether or not the NURBS form of the curve has the
    ///  properties of a circle. If omitted, Rhino's RhinoMath.ZeroTolerance is used</param>
    ///<returns>(bool) True or False</returns>
    static member IsCircle(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tol = max RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsCircle(tol)
    (*
    def IsCircle(curve_id, tolerance=None):
        '''Verifies an object is a circle curve
        Parameters:
          curve_id (guid): Identifier of the curve object
          tolerance (number, optional) If the curve is not a circle, then the tolerance used
            to determine whether or not the NURBS form of the curve has the
            properties of a circle. If omitted, Rhino's internal zero tolerance is used
        Returns:
          bool: True or False
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if tolerance is None or tolerance < 0:
            tolerance = Rhino.RhinoMath.ZeroTolerance
        return curve.IsCircle(tolerance)
    *)


    ///<summary>Verifies an object is a curve</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurve(curveId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> true
    (*
    def IsCurve(object_id):
        '''Verifies an object is a curve
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True or False
        '''
        curve = rhutil.coercecurve(object_id)
        return curve is not None
    *)


    ///<summary>Decide if it makes sense to close off the curve by moving the end point
    ///  to the start point based on start-end gap size and length of curve as
    ///  approximated by chord defined by 6 points</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///  Maximum allowable distance between start point and end
    ///  point. If omitted, the document's current absolute tolerance is used</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurveClosable(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance0 = max Doc.ModelAbsoluteTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsClosable(tolerance0)
    (*
    def IsCurveClosable(curve_id, tolerance=None):
        '''Decide if it makes sense to close off the curve by moving the end point
        to the start point based on start-end gap size and length of curve as
        approximated by chord defined by 6 points
        Parameters:
          curve_id (guid): identifier of the curve object
          tolerance (number,optional) = maximum allowable distance between start point and end
            point. If omitted, the document's current absolute tolerance is used
        Returns:
          bool: True or False
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if tolerance is None: tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        return curve.IsClosable(tolerance)
    *)


    ///<summary>Verifies an object is a closed curve object</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<returns>(bool) True  otherwise False.</returns>
    static member IsCurveClosed(curveId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsClosed
    (*
    def IsCurveClosed(object_id):
        '''Verifies an object is a closed curve object
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if successful otherwise False.  None on error
        '''
        curve = rhutil.coercecurve(object_id)
        return None if not curve else curve.IsClosed
    *)


    ///<summary>Test a curve to see if it lies in a specific plane</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="plane">(Plane) Plane to test.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///  The tolerance used. If omitted, Rhino's RhinoMath.ZeroTolerance is used</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurveInPlane(curveId:Guid, plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance0 = max RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsInPlane(plane, tolerance0)
    (*
    def IsCurveInPlane(object_id, plane=None):
        '''Test a curve to see if it lies in a specific plane
        Parameters:
          object_id (guid): the object's identifier
          plane (plane, optional): plane to test. If omitted, the active construction plane is used
        Returns:
          bool: True or False
        '''
        curve = rhutil.coercecurve(object_id, -1, True)
        if not plane:
            plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
        else:
            plane = rhutil.coerceplane(plane, True)
        return curve.IsInPlane(plane, scriptcontext.doc.ModelAbsoluteTolerance)
    *)


    ///<summary>Verifies an object is a linear curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///  The curve segment index if `curve_id` identifies a polycurve</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///  The tolerance used. If omitted, Rhino's RhinoMath.ZeroTolerance is used</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurveLinear(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tolerance0 = max RhinoMath.ZeroTolerance tolerance
        match  RhinoScriptSyntax.TryCoerceCurve(curveId,segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsLinear(tolerance0)
    (*
    def IsCurveLinear(object_id, segment_index=-1):
        '''Verifies an object is a linear curve
        Parameters:
          object_id (guid):identifier of the curve object
          segment_index (number): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False indicating success or failure
        '''
        curve = rhutil.coercecurve(object_id, segment_index, True)
        return curve.IsLinear()
    *)


    ///<summary>Verifies an object is a periodic curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///  The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurvePeriodic(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryCoerceCurve(curveId,segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsPeriodic
    (*
    def IsCurvePeriodic(curve_id, segment_index=-1):
        '''Verifies an object is a periodic curve object
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        return curve.IsPeriodic
    *)


    ///<summary>Verifies an object is a planar curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///  The tolerance used. If omitted, Rhino's RhinoMath.ZeroTolerance is used</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurvePlanar(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = max RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve(curveId,segmentIndex) with
        |None -> false
        |Some curve  -> curve.IsPlanar(tol)
    (*
    def IsCurvePlanar(curve_id, segment_index=-1):
        '''Verifies an object is a planar curve
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False indicating success or failure
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        return curve.IsPlanar(tol)
    *)


    ///<summary>Verifies an object is a rational NURBS curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurveRational(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryCoerceCurve(curveId,segmentIndex) with
        |None -> false
        |Some c  ->
            match c with
            | :? NurbsCurve as curve -> curve.IsRational
            |_ -> false
    (*
    def IsCurveRational(curve_id, segment_index=-1):
        '''Verifies an object is a rational NURBS curve
        Parameters:
          curve_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False indicating success or failure
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        if isinstance(curve, Rhino.Geometry.NurbsCurve): return curve.IsRational
        return False
    *)


    ///<summary>Verifies an object is an elliptical-shaped curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///  The tolerance used. If omitted, Rhino's RhinoMath.ZeroTolerance is used</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///  The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsEllipse(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = max RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve curveId with
        |None -> false
        |Some curve  -> curve.IsEllipse(tol)
    (*
    def IsEllipse(object_id, segment_index=-1):
        '''Verifies an object is an elliptical-shaped curve
        Parameters:
          object_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False indicating success or failure
        '''
        curve = rhutil.coercecurve(object_id, segment_index, True)
        return curve.IsEllipse()
    *)


    ///<summary>Verifies an object is a line curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///  The tolerance used. If omitted, Rhino's RhinoMath.ZeroTolerance is used</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///  The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsLine(curveId:Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = max RhinoMath.ZeroTolerance tolerance
        match RhinoScriptSyntax.TryCoerceCurve(curveId,segmentIndex) with
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
    (*
    def IsLine(object_id, segment_index=-1):
        '''Verifies an object is a line curve
        Parameters:
          object_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False indicating success or failure
        '''
        curve = rhutil.coercecurve(object_id, segment_index, True)
        if isinstance(curve, Rhino.Geometry.LineCurve): return True
        rc, polyline = curve.TryGetPolyline()
        if rc and polyline.Count==2: return True
        return False
    *)


    ///<summary>Verifies that a point is on a curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="point">(Point3d) The test point</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.SqrtEpsilon</c>
    ///  The tolerance used. If omitted, Rhino's RhinoMath.SqrtEpsilon is used</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsPointOnCurve(curveId:Guid, point:Point3d, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        let tol = max RhinoMath.SqrtEpsilon tolerance
        let curve = RhinoScriptSyntax.CoerceCurve(curveId,segmentIndex) 
        let t = ref 0.0
        curve.ClosestPoint(point, t, tol)
    (*
    def IsPointOnCurve(object_id, point, segment_index=-1):
        '''Verifies that a point is on a curve
        Parameters:
          object_id (guid): identifier of the curve object
          point (point): the test point
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False indicating success or failure
        '''
        curve = rhutil.coercecurve(object_id, segment_index, True)
        point = rhutil.coerce3dpoint(point, True)
        rc, t = curve.ClosestPoint(point, Rhino.RhinoMath.SqrtEpsilon)
        return rc
    *)


    ///<summary>Verifies an object is a PolyCurve curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsPolyCurve(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        // TODO can a polycurve be nested in a polycurve ?
        match RhinoScriptSyntax.TryCoerceCurve(curveId,segmentIndex) with
        |None               -> false
        |Some c  ->
            match c with
            | :? PolyCurve  -> true
            | _             -> false
    (*
    def IsPolyCurve(object_id, segment_index=-1):
        '''Verifies an object is a PolyCurve curve
        Parameters:
          object_id (guid): identifier of the curve object
          segment_index (number, optional) the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False
        '''
        curve = rhutil.coercecurve(object_id, segment_index, True)
        return isinstance(curve, Rhino.Geometry.PolyCurve)
    *)


    ///<summary>Verifies an object is a Polyline curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsPolyline(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        match RhinoScriptSyntax.TryCoerceCurve(curveId,segmentIndex) with
        |None               -> false
        |Some c  ->
            match c with
            | :? PolylineCurve  -> true
            | _             -> false
    (*
    def IsPolyline( object_id, segment_index=-1 ):
        '''Verifies an object is a Polyline curve object
        Parameters:
          object_id (guid): identifier of the curve object
          segment_index (number, optional): the curve segment index if `curve_id` identifies a polycurve
        Returns:
          bool: True or False
        '''
        curve = rhutil.coercecurve(object_id, segment_index, True)
        return isinstance(curve, Rhino.Geometry.PolylineCurve)
    *)


    ///<summary>Joins multiple curves together to form one or more curves or polycurves</summary>
    ///<param name="curveIds">(Guid) List of multiple curves</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input objects after joining</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>2.1 * Doc.ModelAbsoluteTolerance</c>
    ///Join tolerance. If omitted, 2.1 * document absolute
    ///  tolerance is used</param>
    ///<returns>(Guid []) Object id representing the new curves</returns>
    static member JoinCurves(curveIds:Guid [], [<OPT;DEF(false)>]deleteInput:bool, [<OPT;DEF(0.0)>]tolerance:float) : Guid [] =
        if Seq.length(curveIds)<2 then
            failwithf "curveIds must contain at least 2 items.  curveIds:'%A' deleteInput:'%A' tolerance:'%A'" curveIds deleteInput tolerance
        
        let curves = [| for id in curveIds -> RhinoScriptSyntax.CoerceCurve id |]  
        let tolerance0 = max (tolerance)(2.1 * Doc.ModelAbsoluteTolerance)
        let newcurves = Curve.JoinCurves(curves, tolerance0)               
        if isNull newcurves then 
            failwithf "curveIds must contain at least 2 items.  curveIds:'%A' deleteInput:'%A' tolerance:'%A'" curveIds deleteInput tolerance

        let rc = [| for crv in newcurves -> Doc.Objects.AddCurve(crv) |]
        if deleteInput then
            for id in curveIds do
                Doc.Objects.Delete(id, false) |> ignore
        Doc.Views.Redraw()
        rc
          
    (*
    def JoinCurves(object_ids, delete_input=False, tolerance=None):
        '''Joins multiple curves together to form one or more curves or polycurves
        Parameters:
          object_ids (guid): list of multiple curves
          delete_input (bool, optional): delete input objects after joining
          tolerance (number, optional): join tolerance. If omitted, 2.1 * document absolute
              tolerance is used
        Returns:
          list(guid, ...): Object id representing the new curves
        '''
        if len(object_ids)<2: raise ValueError("object_ids must contain at least 2 items")
        curves = [rhutil.coercecurve(id, -1, True) for id in object_ids]
        if tolerance is None:
            tolerance = 2.1 * scriptcontext.doc.ModelAbsoluteTolerance
        newcurves = Rhino.Geometry.Curve.JoinCurves(curves, tolerance)
        rc = []
        if newcurves:
            rc = [scriptcontext.doc.Objects.AddCurve(crv) for crv in newcurves]
        if rc and delete_input:
            for id in object_ids:
                id = rhutil.coerceguid(id, True)
                scriptcontext.doc.Objects.Delete(id, False)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns a line that was fit through an array of 3D points</summary>
    ///<param name="points">(Point3d seq) A list of at least two 3D points</param>
    ///<returns>(Line) line on success</returns>
    static member LineFitFromPoints(points:Point3d seq) : Line =
        let rc, line = Line.TryFitLineToPoints(points)
        if rc then  line
        else failwithf "lineFitFromPoints failed.  points:'%A'" points
    (*
    def LineFitFromPoints(points):
        '''Returns a line that was fit through an array of 3D points
        Parameters:
          points ([point, point, ...]): a list of at least two 3D points
        Returns:
          line: line on success
        '''
        points = rhutil.coerce3dpointlist(points, True)
        rc, line = Rhino.Geometry.Line.TryFitLineToPoints(points)
        if rc: return line
        return scriptcontext.errorhandler()
    *)


    ///<summary>Makes a periodic curve non-periodic. Non-periodic curves can develop
    ///  kinks when deformed</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete the input curve. If omitted, the input curve will not be deleted.</param>
    ///<returns>(Guid) id of the new or modified curve</returns>
    static member MakeCurveNonPeriodic(curveId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        if not <| curve.IsPeriodic then  failwithf "makeCurveNonPeriodic failed.1  curveId:'%A' deleteInput:'%A'" curveId deleteInput
        let nc = curve.ToNurbsCurve()
        if isNull nc  then  failwithf "makeCurveNonPeriodic failed.2  curveId:'%A' deleteInput:'%A'" curveId deleteInput
        if not <| nc.Knots.ClampEnd( CurveEnd.Both )then failwithf "makeCurveNonPeriodic failed.  curveId:'%A' deleteInput:'%A'" curveId deleteInput
        if deleteInput then
            let rc = Doc.Objects.Replace(curveId, nc)
            if not <| rc then  failwithf "makeCurveNonPeriodic failed.3  curveId:'%A' deleteInput:'%A'" curveId deleteInput
            Doc.Views.Redraw()
            curveId
        else
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)  
            let rc = Doc.Objects.AddCurve(nc, rhobj.Attributes)
            if rc = Guid.Empty then  failwithf "makeCurveNonPeriodic failed.4  curveId:'%A' deleteInput:'%A'" curveId deleteInput
            Doc.Views.Redraw()
            rc
    (*
    def MakeCurveNonPeriodic(curve_id, delete_input=False):
        '''Makes a periodic curve non-periodic. Non-periodic curves can develop
        kinks when deformed
        Parameters:
          curve_id (guid): identifier of the curve object
          delete_input (bool): delete the input curve. If omitted, the input curve will not be deleted.
        Returns:
          guid: id of the new or modified curve if successful
          None: on error
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if not curve.IsPeriodic: return scriptcontext.errorhandler()
        nc = curve.ToNurbsCurve()
        if nc is None: return scriptcontext.errorhandler()
        nc.Knots.ClampEnd( Rhino.Geometry.CurveEnd.Both )
        rc = None
        if delete_input:
            if type(curve_id) is Rhino.DocObjects.ObjRef: pass
            else: curve_id = rhutil.coerceguid(curve_id)
            if curve_id:
                rc = scriptcontext.doc.Objects.Replace(curve_id, nc)
                if not rc: return scriptcontext.errorhandler()
                rc = rhutil.coerceguid(curve_id)
        else:
            attrs = None
            if type(scriptcontext.doc) is Rhino.RhinoDoc:
                rhobj = rhutil.coercerhinoobject(curve_id)
                if rhobj: attrs = rhobj.Attributes
            rc = scriptcontext.doc.Objects.AddCurve(nc, attrs)
            if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Creates an average curve from two curves</summary>
    ///<param name="curve0">(Guid) Curve0 of 'identifiers of two curves' (FIXME 0)</param>
    ///<param name="curve1">(Guid) Curve1 of 'identifiers of two curves' (FIXME 0)</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///Angle tolerance used to match kinks between curves</param>
    ///<returns>(Guid) id of the new or modified curve</returns>
    static member MeanCurve(curve0:Guid, curve1:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid =
        let curve0 = RhinoScriptSyntax.CoerceCurve curve0  
        let curve1 = RhinoScriptSyntax.CoerceCurve curve1  
        let  tolerance = if tolerance = 0.0 then RhinoMath.UnsetValue else abs (tolerance)
        let crv = Curve.CreateMeanCurve(curve0,curve1,tolerance)
        if notNull crv then
            let rc = Doc.Objects.AddCurve(crv)
            Doc.Views.Redraw()
            rc
        else
            failwithf "meanCurve failed.  curve1:'%A' curve0:'%A' tolerance:'%f'" curve1 curve0 tolerance
    (*
    def MeanCurve(curve0, curve1, tolerance=None):
        '''Creates an average curve from two curves
        Parameters:
          curve0, curve1 (guid): identifiers of two curves
          tolerance (number, optional): angle tolerance used to match kinks between curves
        Returns:
          guid: id of the new or modified curve if successful
          None: on error
        '''
        curve0 = rhutil.coercecurve(curve0, -1, True)
        curve1 = rhutil.coercecurve(curve1, -1, True)
        if tolerance is None: tolerance=Rhino.RhinoMath.UnsetValue
        crv = Rhino.Geometry.Curve.CreateMeanCurve(curve0,curve1,tolerance)
        if crv:
            rc = scriptcontext.doc.Objects.AddCurve(crv)
            scriptcontext.doc.Views.Redraw()
            return rc
    *)


    ///<summary>Creates a polygon mesh object based on a closed polyline curve object.
    ///  The created mesh object is added to the document</summary>
    ///<param name="polylineId">(Guid) Identifier of the polyline curve object</param>
    ///<returns>(Guid) identifier of the new mesh object</returns>
    static member MeshPolyline(polylineId:Guid) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve polylineId  
        let ispolyline, polyline = curve.TryGetPolyline()
        if not <| ispolyline then  failwithf "meshPolyline failed.  polylineId:'%A'" polylineId
        let mesh = Mesh.CreateFromClosedPolyline(polyline)
        if isNull mesh then  failwithf "meshPolyline failed.  polylineId:'%A'" polylineId
        let rc = Doc.Objects.AddMesh(mesh)
        Doc.Views.Redraw()
        rc
    (*
    def MeshPolyline(polyline_id):
        '''Creates a polygon mesh object based on a closed polyline curve object.
        The created mesh object is added to the document
        Parameters:
          polyline_id (guid): identifier of the polyline curve object
        Returns:
          guid: identifier of the new mesh object
          None: on error
        '''
        curve = rhutil.coercecurve(polyline_id, -1, True)
        ispolyline, polyline = curve.TryGetPolyline()
        if not ispolyline: return scriptcontext.errorhandler()
        mesh = Rhino.Geometry.Mesh.CreateFromClosedPolyline(polyline)
        if not mesh: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddMesh(mesh)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Offsets a curve by a distance. The offset curve will be added to Rhino</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="direction">(Point3d) Point describing direction of the offset</param>
    ///<param name="distance">(float) Distance of the offset</param>
    ///<param name="normal">(Vector3d) Optional, Default Value: <c>Vector3d.ZAxis</c>
    ///  Normal of the plane in which the offset will occur.
    ///  If omitted, the WorldXY  plane will be used</param>
    ///<param name="style">(int) Optional, Default Value: <c>1</c>
    ///The corner style. If omitted, the style is sharp.
    ///  0 = None
    ///  1 = Sharp
    ///  2 = Round
    ///  3 = Smooth
    ///  4 = Chamfer</param>
    ///<returns>(Guid seq) list of ids for the new curves on success</returns>
    static member OffsetCurve(curveId:Guid, direction:Point3d, distance:float, [<OPT;DEF(null)>]normal:Vector3d, [<OPT;DEF(1)>]style:int) : Guid [] =
        let normal0 =  match box normal with :?Vector3d as v -> v |_ -> Vector3d.ZAxis
        let curve = RhinoScriptSyntax.CoerceCurve curveId  
        let tolerance = Doc.ModelAbsoluteTolerance
        let style:CurveOffsetCornerStyle = EnumOfValue style
        let curves = curve.Offset(direction, normal0, distance, tolerance, style)
        if isNull curves then  failwithf "offsetCurve failed.  curveId:'%A' direction:'%A' distance:'%A' normal:'%A' style:'%A'" curveId direction distance normal style
        let rc = [| for curve in curves -> Doc.Objects.AddCurve(curve) |]
        Doc.Views.Redraw()
        rc
    (*
    def OffsetCurve(object_id, direction, distance, normal=None, style=1):
        '''Offsets a curve by a distance. The offset curve will be added to Rhino
        Parameters:
          object_id (guid): identifier of a curve object
          direction (point): point describing direction of the offset
          distance (number): distance of the offset
          normal (vector, optional): normal of the plane in which the offset will occur.
              If omitted, the normal of the active construction plane will be used
          style (number, optional): the corner style. If omitted, the style is sharp.
                                    0 = None
                                    1 = Sharp
                                    2 = Round
                                    3 = Smooth
                                    4 = Chamfer
        Returns:
          list(guid, ...): list of ids for the new curves on success
          None: on error
        '''
        curve = rhutil.coercecurve(object_id, -1, True)
        direction = rhutil.coerce3dpoint(direction, True)
        if normal:
            normal = rhutil.coerce3dvector(normal, True)
        else:
            normal = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane().Normal
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        style = System.Enum.ToObject(Rhino.Geometry.CurveOffsetCornerStyle, style)
        curves = curve.Offset(direction, normal, distance, tolerance, style)
        if curves is None: return scriptcontext.errorhandler()
        rc = [scriptcontext.doc.Objects.AddCurve(curve) for curve in curves]
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Offset a curve on a surface. The source curve must lie on the surface.
    ///  The offset curve or curves will be added to Rhino</summary>
    ///<param name="curveId">(Guid) Curve identifiers</param>
    ///<param name="surfaceId">(Guid) Surface identifiers</param>
    ///<param name="parameter">(Point2d) ):  U,V parameter that the curve will be offset through</param>
    ///<returns>(Guid seq) identifiers of the new curves</returns>
    static member OffsetCurveOnSurfaceUV(curveId:Guid, surfaceId:Guid, parameter:Point2d) : Guid [] =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)      
        let tol = Doc.ModelAbsoluteTolerance
        let curves = curve.OffsetOnSurface(surface, parameter, tol)
        if isNull curves  then  failwithf "offsetCurveOnSurface failed.  curveId:'%A' surfaceId:'%A' parameter:'%A'" curveId surfaceId parameter
        let rc = [| for curve in curves -> Doc.Objects.AddCurve(curve) |]
        Doc.Views.Redraw()
        rc

    ///<summary>Offset a curve on a surface. The source curve must lie on the surface.
    ///  The offset curve or curves will be added to Rhino</summary>
    ///<param name="curveId">(Guid) Curve identifiers</param>
    ///<param name="surfaceId">(Guid) Surface identifiers</param> 
    ///<param name="distance">(float) ):the distance of the offset. Based on the curve's direction, a positive value
    ///  will offset to the left and a negative value will offset to the right.</param>
    ///<returns>(Guid seq) identifiers of the new curves</returns>
    static member OffsetCurveOnSurface(curveId:Guid, surfaceId:Guid, distance:float) : Guid [] =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)  
        let tol = Doc.ModelAbsoluteTolerance
        let curves = curve.OffsetOnSurface(surface, distance, tol)
        if isNull curves  then  failwithf "offsetCurveOnSurface failed.  curveId:'%A' surfaceId:'%A' distance:'%A'" curveId surfaceId distance
        let rc = [| for curve in curves -> Doc.Objects.AddCurve(curve) |]
        Doc.Views.Redraw()
        rc

    (*
    def OffsetCurveOnSurface(curve_id, surface_id, distance_or_parameter):
        '''Offset a curve on a surface. The source curve must lie on the surface.
        The offset curve or curves will be added to Rhino
        Parameters:
          curve_id, surface_id (guid): curve and surface identifiers
          distance_or_parameter (number|tuple(number, number)): If a single number is passed, then this is the
            distance of the offset. Based on the curve's direction, a positive value
            will offset to the left and a negative value will offset to the right.
            If a tuple of two values is passed, this is interpreted as the surface
            U,V parameter that the curve will be offset through
        Returns:
          list(guid, ...): identifiers of the new curves if successful
          None: on error
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        surface = rhutil.coercesurface(surface_id, True)
        x = None
        if type(distance_or_parameter) is list or type(distance_or_parameter) is tuple:
            x = Rhino.Geometry.Point2d( distance_or_parameter[0], distance_or_parameter[1] )
        else:
            x = float(distance_or_parameter)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        curves = curve.OffsetOnSurface(surface, x, tol)
        if curves is None: return scriptcontext.errorhandler()
        rc = [scriptcontext.doc.Objects.AddCurve(curve) for curve in curves]
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Determines the relationship between the regions bounded by two coplanar simple closed curves</summary>
    ///<param name="curveA">(Guid) Curve a of 'identifiers of two planar, closed curves' (FIXME 0)</param>
    ///<param name="curveB">(Guid) Curve b of 'identifiers of two planar, closed curves' (FIXME 0)</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///Test plane. If omitted, the Plane.WorldXY plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///If omitted, the document absolute tolerance is used</param>
    ///<returns>(int) a number identifying the relationship
    ///  0 = the regions bounded by the curves are disjoint
    ///  1 = the two curves intersect
    ///  2 = the region bounded by curve_a is inside of curve_b
    ///  3 = the region bounded by curve_b is inside of curve_a</returns>
    static member PlanarClosedCurveContainment(curveA:Guid, curveB:Guid, [<OPT;DEF(null)>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        let curveA = RhinoScriptSyntax.CoerceCurve curveA  
        let curveB = RhinoScriptSyntax.CoerceCurve curveB  
        let tolerance0 = max Doc.ModelAbsoluteTolerance tolerance
        let plane0 =  match box plane with :?Plane as pl -> pl |_ -> Plane.WorldXY
        let rc = Curve.PlanarClosedCurveRelationship(curveA, curveB, plane0, tolerance0)
        int(rc)
    (*
    def PlanarClosedCurveContainment(curve_a, curve_b, plane=None, tolerance=None):
        '''Determines the relationship between the regions bounded by two coplanar
        simple closed curves
        Parameters:
          curve_a, curve_b (guid): identifiers of two planar, closed curves
          plane (plane, optional): test plane. If omitted, the currently active construction
            plane is used
          tolerance (number, optional): if omitted, the document absolute tolerance is used
        Returns:
          number: a number identifying the relationship if successful
            0 = the regions bounded by the curves are disjoint
            1 = the two curves intersect
            2 = the region bounded by curve_a is inside of curve_b
            3 = the region bounded by curve_b is inside of curve_a
          None: if not successful
        '''
        curve_a = rhutil.coercecurve(curve_a, -1, True)
        curve_b = rhutil.coercecurve(curve_b, -1, True)
        if tolerance is None or tolerance<=0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        if plane:
            plane = rhutil.coerceplane(plane)
        else:
            plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
        rc = Rhino.Geometry.Curve.PlanarClosedCurveRelationship(curve_a, curve_b, plane, tolerance)
        return int(rc)
    *)


    ///<summary>Determines if two coplanar curves intersect</summary>
    ///<param name="curveA">(Guid) Curve a of 'identifiers of two planar curves' (FIXME 0)</param>
    ///<param name="curveB">(Guid) Curve b of 'identifiers of two planar curves' (FIXME 0)</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>null</c>
    ///Test plane. If omitted, the currently active construction
    ///  plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///  If omitted, the document absolute tolerance is used</param>
    ///<returns>(bool) True if the curves intersect; otherwise False</returns>
    static member PlanarCurveCollision(curveA:Guid, curveB:Guid, [<OPT;DEF(null)>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let curveA = RhinoScriptSyntax.CoerceCurve curveA  
        let curveB = RhinoScriptSyntax.CoerceCurve curveB  
        let tolerance0 = max Doc.ModelAbsoluteTolerance tolerance
        let plane0 =  match box plane with :?Plane as pl -> pl |_ -> Plane.WorldXY
        Curve.PlanarCurveCollision(curveA, curveB, plane0, tolerance0)
    (*
    def PlanarCurveCollision(curve_a, curve_b, plane=None, tolerance=None):
        '''Determines if two coplanar curves intersect
        Parameters:
          curve_a, curve_b (guid): identifiers of two planar curves
          plane (plane, optional): test plane. If omitted, the currently active construction
            plane is used
          tolerance (number, optional): if omitted, the document absolute tolerance is used
        Returns:
          bool: True if the curves intersect; otherwise False
        '''
        curve_a = rhutil.coercecurve(curve_a, -1, True)
        curve_b = rhutil.coercecurve(curve_b, -1, True)
        if tolerance is None or tolerance<=0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        if plane:
            plane = rhutil.coerceplane(plane)
        else:
            plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
        return Rhino.Geometry.Curve.PlanarCurveCollision(curve_a, curve_b, plane, tolerance)
    *)


    ///<summary>Determines if a point is inside of a closed curve, on a closed curve, or
    ///  outside of a closed curve</summary>
    ///<param name="point">(Point3d) Text point</param>
    ///<param name="curve">(Guid) Identifier of a curve object</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///Plane containing the closed curve and point. If omitted, Plane.WorldXY  is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///  If omitted, the document abosulte tolerance is used</param>
    ///<returns>(int) number identifying the result
    ///  0 = point is outside of the curve
    ///  1 = point is inside of the curve
    ///  2 = point in on the curve</returns>
    static member PointInPlanarClosedCurve(point:Point3d, curve:Guid, [<OPT;DEF(null)>]plane:Plane, [<OPT;DEF(0.0)>]tolerance:float) : int =
        let curve = RhinoScriptSyntax.CoerceCurve curve  
        let tolerance0 = max Doc.ModelAbsoluteTolerance tolerance
        let plane0 =  match box plane with :?Plane as pl -> pl |_ -> Plane.WorldXY
        let rc = curve.Contains(point, plane0, tolerance0)
        if rc=PointContainment.Unset then
            failwithf "pointInPlanarClosedCurve Curve.Contains is Unset.  point:'%A' curve:'%A' plane:'%A' tolerance:'%A'" point curve plane tolerance
        if rc=PointContainment.Outside then  0
        elif rc=PointContainment.Inside then  1
        else 2
    (*
    def PointInPlanarClosedCurve(point, curve, plane=None, tolerance=None):
        '''Determines if a point is inside of a closed curve, on a closed curve, or
        outside of a closed curve
        Parameters:
          point (point|guid): text point
          curve (guid): identifier of a curve object
          plane (plane, optional): plane containing the closed curve and point. If omitted,
              the currently active construction plane is used
          tolerance (number, optional) it omitted, the document abosulte tolerance is used
        Returns:
          number: number identifying the result if successful
                  0 = point is outside of the curve
                  1 = point is inside of the curve
                  2 = point in on the curve
        '''
        point = rhutil.coerce3dpoint(point, True)
        curve = rhutil.coercecurve(curve, -1, True)
        if tolerance is None or tolerance<=0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        if plane:
            plane = rhutil.coerceplane(plane)
        else:
            plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
        rc = curve.Contains(point, plane, tolerance)
        if rc==Rhino.Geometry.PointContainment.Unset: raise Exception("Curve.Contains returned Unset")
        if rc==Rhino.Geometry.PointContainment.Outside: return 0
        if rc==Rhino.Geometry.PointContainment.Inside: return 1
        return 2
    *)


    ///<summary>Returns the number of curve segments that make up a polycurve</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///  If `curveId` identifies a polycurve object, then `segmentIndex` identifies the curve segment of the polycurve to query.</param>
    ///<returns>(int) the number of curve segments in a polycurve</returns>
    static member PolyCurveCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        match curve with
        | :? PolyCurve as curve ->  curve.SegmentCount
        | _ -> failwithf "CurveId does not reference a polycurve.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
    (*
    def PolyCurveCount(curve_id, segment_index=-1):
        '''Returns the number of curve segments that make up a polycurve
        Parameters:
          curve_id (guid): the object's identifier
          segment_index (number, optional): if `curve_id` identifies a polycurve object, then `segment_index` identifies the curve segment of the polycurve to query.
        Returns:
          number: the number of curve segments in a polycurve if successful
          None: if not successful
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        if isinstance(curve, Rhino.Geometry.PolyCurve): return curve.SegmentCount
        raise ValueError("curve_id does not reference a polycurve")
    *)


    ///<summary>Returns the vertices of a polyline curve on success</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///  If curveId identifies a polycurve object, then segmentIndex identifies the curve segment of the polycurve to query.</param>
    ///<returns>(Point3d seq) an list of Point3d vertex points</returns>
    static member PolylineVertices(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d [] =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, segmentIndex)  
        let rc, polyline = curve.TryGetPolyline()
        if rc then  [| for pt in polyline -> pt |]
        else failwithf "CurveId does not <| reference a polyline.  curveId:'%A' segmentIndex:'%A'" curveId segmentIndex
    (*
    def PolylineVertices(curve_id, segment_index=-1):
        '''Returns the vertices of a polyline curve on success
        Parameters:
          curve_id (guid): the object's identifier
          segment_index (number, optional): if curve_id identifies a polycurve object, then segment_index identifies the curve segment of the polycurve to query.
        Returns:
          list(point, ...): an list of Point3d vertex points if successful
          None: if not successful
        '''
        curve = rhutil.coercecurve(curve_id, segment_index, True)
        rc, polyline = curve.TryGetPolyline()
        if rc: return [pt for pt in polyline]
        raise ValueError("curve_id does not reference a polyline")
    *)


    ///<summary>Projects one or more curves onto one or more surfaces or meshes</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of curves to project</param>
    ///<param name="meshIds">(Guid seq) Identifiers of meshes to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid seq) list of identifiers for the resulting curves.</returns>
    static member ProjectCurveToMesh(curveIds:Guid seq, meshIds:Guid seq, direction:Vector3d) : Guid [] =
        let curves = [| for id in curveIds -> RhinoScriptSyntax.CoerceCurve id |]  
        let meshes = [| for id in meshIds -> RhinoScriptSyntax.CoerceMesh(id) |]  
        let tolerance = Doc.ModelAbsoluteTolerance
        let newcurves = Curve.ProjectToMesh(curves, meshes, direction, tolerance)
        let ids = [| for curve in newcurves -> Doc.Objects.AddCurve(curve) |]
        if ids.Length >0 then  Doc.Views.Redraw()
        ids
    (*
    def ProjectCurveToMesh(curve_ids, mesh_ids, direction):
        '''Projects one or more curves onto one or more surfaces or meshes
        Parameters:
          curve_ids ([guid, ...]): identifiers of curves to project
          mesh_ids ([guid, ...]): identifiers of meshes to project onto
          direction (vector): projection direction
        Returns:
          list(guid, ...): list of identifiers for the resulting curves.
        '''
        curve_ids = rhutil.coerceguidlist(curve_ids)
        mesh_ids = rhutil.coerceguidlist(mesh_ids)
        direction = rhutil.coerce3dvector(direction, True)
        curves = [rhutil.coercecurve(id, -1, True) for id in curve_ids]
        meshes = [rhutil.coercemesh(id, True) for id in mesh_ids]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        newcurves = Rhino.Geometry.Curve.ProjectToMesh(curves, meshes, direction, tolerance)
        ids = [scriptcontext.doc.Objects.AddCurve(curve) for curve in newcurves]
        if ids: scriptcontext.doc.Views.Redraw()
        return ids
    *)


    ///<summary>Projects one or more curves onto one or more surfaces or polysurfaces</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of curves to project</param>
    ///<param name="surfaceIds">(Guid seq) Identifiers of surfaces to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid seq) list of identifiers</returns>
    static member ProjectCurveToSurface(curveIds:Guid seq, surfaceIds:Guid seq, direction:Vector3d) : Guid [] =
        let curves = [| for id in curveIds -> RhinoScriptSyntax.CoerceCurve id |]  
        let breps = [| for id in surfaceIds -> RhinoScriptSyntax.CoerceBrep(id) |]  
        let tolerance = Doc.ModelAbsoluteTolerance
        let newcurves = Curve.ProjectToBrep(curves, breps, direction, tolerance)
        let ids = [| for curve in newcurves -> Doc.Objects.AddCurve(curve) |]
        if ids.Length >0 then  Doc.Views.Redraw()
        ids
    (*
    def ProjectCurveToSurface(curve_ids, surface_ids, direction):
        '''Projects one or more curves onto one or more surfaces or polysurfaces
        Parameters:
          curve_ids ([guid, ...]): identifiers of curves to project
          surface_ids ([guid, ...]): identifiers of surfaces to project onto
          direction (vector): projection direction
        Returns:
          list(guid, ...): list of identifiers
        '''
        curve_ids = rhutil.coerceguidlist(curve_ids)
        surface_ids = rhutil.coerceguidlist(surface_ids)
        direction = rhutil.coerce3dvector(direction, True)
        curves = [rhutil.coercecurve(id, -1, True) for id in curve_ids]
        breps = [rhutil.coercebrep(id, True) for id in surface_ids]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        newcurves = Rhino.Geometry.Curve.ProjectToBrep(curves, breps, direction, tolerance)
        ids = [scriptcontext.doc.Objects.AddCurve(curve) for curve in newcurves]
        if ids: scriptcontext.doc.Views.Redraw()
        return ids
    *)


    ///<summary>Rebuilds a curve to a given degree and control point count. For more
    ///  information, see the Rhino help for the Rebuild command.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="degree">(int) New degree (must be greater than 0)</param>
    ///<param name="pointCount">(int) New point count, which must be bigger than degree.</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RebuildCurve(curveId:Guid, degree:int, pointCount:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        if degree<1 then  failwithf "rebuildCurve: Degree must be greater than 0.  curveId:'%A' degree:'%A' pointCount:'%A'" curveId degree pointCount
        let newcurve = curve.Rebuild(pointCount, degree, false)
        if isNull newcurve then  false
        else
            Doc.Objects.Replace(curveId, newcurve) |> ignore
            Doc.Views.Redraw()
            true

    // Rhino 6 only ??
    //<summary>Deletes a knot from a curve object.</summary>
    //<param name="parameter">(float): The parameter on the curve. Note, if the parameter is not equal to one
    //                of the existing knots, then the knot closest to the specified parameter
    //                will be removed.</param>
    //<param name="curve">(Guid): The reference of the source object</param>
    //<returns>(bool) True of False indicating success or failure</returns>
    //let removeCurveKnot (parameter:float) (curve:Guid) :bool =
    ////    let curveInst = RhinoScriptSyntax.CoerceCurve curve  
    //    let success, nParam = curveInst.GetCurveParameterFromNurbsFormParameter(parameter)
    //    if not <| success then  false
    //    else
    //        let nCurve = curveInst.ToNurbsCurve()
    //        if isNull nCurve then  false
    //        else
    //            let success = nCurve.Knots.RemoveKnotAt(nParam)
    //            if not <| success then  false
    //            else
    //                Doc.Objects.Replace(curve, nCurve)|> ignore
    //                Doc.Views.Redraw()
    //                true
    (*
    def RebuildCurve(curve_id, degree=3, point_count=10):
        '''Rebuilds a curve to a given degree and control point count. For more
        information, see the Rhino help for the Rebuild command.
        Parameters:
          curve_id (guid): identifier of the curve object
          degree (number, optional): new degree (must be greater than 0)
          point_count (number, optional) new point count, which must be bigger than degree.
        Returns:
          bool: True of False indicating success or failure
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if degree<1: raise ValueError("degree must be greater than 0")
        newcurve = curve.Rebuild(point_count, degree, False)
        if not newcurve: return False
        scriptcontext.doc.Objects.Replace(curve_id, newcurve)
        scriptcontext.doc.Views.Redraw()
        return True
    *)


    ///<summary>Deletes a knot from a curve object.</summary>
    ///<param name="curve">(Guid) The reference of the source object</param>
    ///<param name="parameter">(float) The parameter on the curve. Note, if the parameter is not equal to one
    ///  of the existing knots, then the knot closest to the specified parameter
    ///  will be removed.</param>
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
    (*
    def RemoveCurveKnot(curve, parameter):
        '''Deletes a knot from a curve object.
        Parameters:
          curve (guid): The reference of the source object
          parameter (number): The parameter on the curve. Note, if the parameter is not equal to one
                          of the existing knots, then the knot closest to the specified parameter
                          will be removed.
        Returns:
          bool: True of False indicating success or failure
        '''
        curve_inst = rhutil.coercecurve(curve, -1, True)
        success, n_param = curve_inst.GetCurveParameterFromNurbsFormParameter(parameter)
        if not success: return False
        n_curve = curve_inst.ToNurbsCurve()
        if not n_curve: return False
        success = n_curve.Knots.RemoveKnotAt(n_param)
        if not success: return False
        scriptcontext.doc.Objects.Replace(curve, n_curve)
        scriptcontext.doc.Views.Redraw()
        return True
    *)


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
    (*
    def ReverseCurve(curve_id):
        '''Reverses the direction of a curve object. Same as Rhino's Dir command
        Parameters:
          curve_id (guid): identifier of the curve object
        Returns:
          bool: True or False indicating success or failure
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if curve.Reverse():
            curve_id = rhutil.coerceguid(curve_id, True)
            scriptcontext.doc.Objects.Replace(curve_id, curve)
            return True
        return False
    *)


    ///<summary>Replace a curve with a geometrically equivalent polycurve.
    ///  The polycurve will have the following properties:
    ///    - All the polycurve segments are lines, polylines, arcs, or NURBS curves.
    ///    - The NURBS curves segments do not have fully multiple interior knots.
    ///    - Rational NURBS curves do not have constant weights.
    ///    - Any segment for which IsCurveLinear or IsArc is True is a line, polyline segment, or an arc.
    ///    - Adjacent co-linear or co-circular segments are combined.
    ///    - Segments that meet with G1-continuity have there ends tuned up so that they meet with G1-continuity to within machine precision.
    ///    - If the polycurve is a polyline, a polyline will be created</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="flags">(int) Optional, Default Value: <c>0</c>
    ///The simplification methods to use. By default, all methods are used (flags = 0)
    ///  Value Description
    ///  0     Use all methods.
    ///  1     Do not split NURBS curves at fully multiple knots.
    ///  2     Do not replace segments with IsCurveLinear = True with line curves.
    ///  4     Do not replace segments with IsArc = True with arc curves.
    ///  8     Do not replace rational NURBS curves with constant denominator with an equivalent non-rational NURBS curve.
    ///  16    Do not adjust curves at G1-joins.
    ///  32    Do not merge adjacent co-linear lines or co-circular arcs or combine consecutive line segments into a polyline.</param>
    ///<returns>(bool) True or False</returns>
    static member SimplifyCurve(curveId:Guid, [<OPT;DEF(0)>]flags:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let mutable flags = 63
        if flags &&& 1 = 1  then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.SplitAtFullyMultipleKnots))
        if flags &&& 2 = 2  then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.RebuildLines))
        if flags &&& 4 = 4  then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.RebuildArcs))
        if flags &&& 8 = 8  then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.RebuildRationals))
        if flags &&& 16= 16 then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.AdjustG1))
        if flags &&& 32= 32 then flags <- flags &&& ( ~~~ (int CurveSimplifyOptions.Merge))
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
    (*
    def SimplifyCurve(curve_id, flags=0):
        '''Replace a curve with a geometrically equivalent polycurve.
        
        The polycurve will have the following properties:
         - All the polycurve segments are lines, polylines, arcs, or NURBS curves.
         - The NURBS curves segments do not have fully multiple interior knots.
         - Rational NURBS curves do not have constant weights.
         - Any segment for which IsCurveLinear or IsArc is True is a line, polyline segment, or an arc.
         - Adjacent co-linear or co-circular segments are combined.
         - Segments that meet with G1-continuity have there ends tuned up so that they meet with G1-continuity to within machine precision.
         - If the polycurve is a polyline, a polyline will be created
        Parameters:
          curve_id (guid): the object's identifier
          flags (number, optional): the simplification methods to use. By default, all methods are used (flags = 0)
            Value Description
            0     Use all methods.
            1     Do not split NURBS curves at fully multiple knots.
            2     Do not replace segments with IsCurveLinear = True with line curves.
            4     Do not replace segments with IsArc = True with arc curves.
            8     Do not replace rational NURBS curves with constant denominator with an equivalent non-rational NURBS curve.
            16    Do not adjust curves at G1-joins.
            32    Do not merge adjacent co-linear lines or co-circular arcs or combine consecutive line segments into a polyline.
        Returns:
          bool: True or False
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        _flags = Rhino.Geometry.CurveSimplifyOptions.All
        if( flags&1 ==1 ): _flags &= (~Rhino.Geometry.CurveSimplifyOptions.SplitAtFullyMultipleKnots)
        if( flags&2 ==2 ): _flags &= (~Rhino.Geometry.CurveSimplifyOptions.RebuildLines)
        if( flags&4 ==4 ): _flags &= (~Rhino.Geometry.CurveSimplifyOptions.RebuildArcs)
        if( flags&8 ==8 ): _flags &= (~Rhino.Geometry.CurveSimplifyOptions.RebuildRationals)
        if( flags&16==16 ): _flags &= (~Rhino.Geometry.CurveSimplifyOptions.AdjustG1)
        if( flags&32==32 ): _flags &= (~Rhino.Geometry.CurveSimplifyOptions.Merge)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        ang_tol = scriptcontext.doc.ModelAngleToleranceRadians
        newcurve = curve.Simplify(_flags, tol, ang_tol)
        if newcurve:
            curve_id = rhutil.coerceguid(curve_id, True)
            scriptcontext.doc.Objects.Replace(curve_id, newcurve)
            scriptcontext.doc.Views.Redraw()
            return True
        return False
    *)


    ///<summary>Splits, or divides, a curve at a specified parameter. The parameter must
    ///  be in the interior of the curve's domain</summary>
    ///<param name="curveId">(Guid) The curve to split</param>
    ///<param name="parameter">(float seq) One or more parameters to split the curve at</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input curve</param>
    ///<returns>(Guid seq) list of new curves on success</returns>
    static member SplitCurve(curveId:Guid, parameter:float seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid [] =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let newcurves = curve.Split(parameter)
        if isNull newcurves then  failwithf "splitCurve failed.  curveId:'%A' parameter:'%A' deleteInput:'%A'" curveId parameter deleteInput
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)  
        let rc = [| for crv in newcurves -> Doc.Objects.AddCurve(crv, rhobj.Attributes) |]
        if deleteInput then
            Doc.Objects.Delete(curveId, true)|> ignore
        Doc.Views.Redraw()
        rc
    (*
    def SplitCurve(curve_id, parameter, delete_input=True):
        '''Splits, or divides, a curve at a specified parameter. The parameter must
        be in the interior of the curve's domain
        Parameters:
          curve_id (guid): the curve to split
          parameter ([number, ...]) one or more parameters to split the curve at
          delete_input (bool, optional): delete the input curve
        Returns:
          list(guid, ....): list of new curves on success
          None: on error
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        newcurves = curve.Split(parameter)
        if newcurves is None: return scriptcontext.errorhandler()
        att = None
        rhobj = rhutil.coercerhinoobject(curve_id)
        if rhobj: att = rhobj.Attributes
        rc = [scriptcontext.doc.Objects.AddCurve(crv, att) for crv in newcurves]
        if rc and delete_input:
            id = rhutil.coerceguid(curve_id, True)
            scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Trims a curve by removing portions of the curve outside a specified interval</summary>
    ///<param name="curveId">(Guid) The curve to trim</param>
    ///<param name="interval">(float * float) Two numbers identifying the interval to keep. Portions of
    ///  the curve before domain[0] and after domain[1] will be removed. If the
    ///  input curve is open, the interval must be increasing. If the input
    ///  curve is closed and the interval is decreasing, then the portion of
    ///  the curve across the start and end of the curve is returned</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input curve. If omitted the input curve is deleted.</param>
    ///<returns>(Guid) identifier of the new curve on success</returns>
    static member TrimCurve(curveId:Guid, interval:float * float, [<OPT;DEF(true)>]deleteInput:bool) : Guid  =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId) 
        let newcurve = curve.Trim(fst interval, snd interval)
        if isNull newcurve then  failwithf "trimCurve failed.  curveId:'%A' interval:'%A' deleteInput:'%A'" curveId interval deleteInput
        let att = None
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(curveId)  
        let rc = Doc.Objects.AddCurve(newcurve, rhobj.Attributes)
        if deleteInput then
            Doc.Objects.Delete(curveId, true)|> ignore
        Doc.Views.Redraw()
        rc
    (*
    def TrimCurve(curve_id, interval, delete_input=True):
        '''Trims a curve by removing portions of the curve outside a specified interval
        Parameters:
          curve_id (guid):the curve to trim
          interval ([number, number]): two numbers identifying the interval to keep. Portions of
            the curve before domain[0] and after domain[1] will be removed. If the
            input curve is open, the interval must be increasing. If the input
            curve is closed and the interval is decreasing, then the portion of
            the curve across the start and end of the curve is returned
          delete_input (bool): delete the input curve. If omitted the input curve is deleted.
        Returns:
          list(guid, ...): identifier of the new curve on success
          None: on failure
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        if interval[0]==interval[1]: raise ValueError("interval values are equal")
        newcurve = curve.Trim(interval[0], interval[1])
        if not newcurve: return scriptcontext.errorhandler()
        att = None
        rhobj = rhutil.coercerhinoobject(curve_id)
        if rhobj: att = rhobj.Attributes
        rc = scriptcontext.doc.Objects.AddCurve(newcurve, att)
        if delete_input:
            id = rhutil.coerceguid(curve_id, True)
            scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Changes the degree of a curve object. For more information see the Rhino help file for the ChangeDegree command.</summary>
    ///<param name="curveId">(Guid) The object's identifier.</param>
    ///<param name="degree">(int) The new degree.</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member ChangeCurveDegree(curveId:Guid, degree:int) : bool =
        let curve = RhinoScriptSyntax.CoerceCurve curveId  
        let nc = curve.ToNurbsCurve()
        if degree > 2 && degree < 12 && curve.Degree <> degree then
            if not <| nc.IncreaseDegree(degree) then false
            else
                Doc.Objects.Replace(curveId, nc)
        else
            false
    (*
    def ChangeCurveDegree(object_id, degree):
        '''Changes the degree of a curve object. For more information see the Rhino help file for the ChangeDegree command.
          Parameters:
            object_id (guid): the object's identifier.
            degree (number): the new degree.
          Returns:
            bool: True of False indicating success or failure.
            None: on failure
          '''
     
        curve = rhutil.coercerhinoobject(object_id)
        if not curve: return None
        if not isinstance(curve, Rhino.DocObjects.CurveObject): return None
        curve = curve.CurveGeometry
        if not isinstance(curve, Rhino.Geometry.NurbsCurve):
            curve = curve.ToNurbsCurve()
        max_nurbs_degree = 11
        if degree < 1 or degree > max_nurbs_degree or curve.Degree == degree:
            return None
        r = False
        if curve.IncreaseDegree(degree):
            r = scriptcontext.doc.Objects.Replace(object_id, curve)
        return r
    *)


    ///<summary>Creates curves between two open or closed input curves.</summary>
    ///<param name="fromCurveId">(Guid) Identifier of the first curve object.</param>
    ///<param name="toCurveId">(Guid) Identifier of the second curve object.</param>
    ///<param name="numberOfCurves">(int) Optional, Default Value: <c>1</c>
    ///The number of curves to create. The default is 1.</param>
    ///<param name="method">(int) Optional, Default Value: <c>0</c>
    ///The method for refining the output curves, where:
    ///  0: (Default) Uses the control points of the curves for matching. So the first control point of first curve is matched to first control point of the second curve.
    ///  1: Refits the output curves like using the FitCurve method.  Both the input curve and the output curve will have the same structure. The resulting curves are usually more complex than input unless input curves are compatible.
    ///  2: Input curves are divided to the specified number of points on the curve, corresponding points define new points that output curves go through. If you are making one tween curve, the method essentially does the following: divides the two curves into an equal number of points, finds the midpoint between the corresponding points on the curves, and interpolates the tween curve through those points.</param>
    ///<param name="sampleNumber">(int) Optional, Default Value: <c>10</c>
    ///  The number of samples points to use if method is 2. The default is 10.</param>
    ///<returns>(Guid seq) The identifiers of the new tween objects , .</returns>
    static member AddTweenCurves(fromCurveId:Guid, toCurveId:Guid, [<OPT;DEF(1)>]numberOfCurves:int, [<OPT;DEF(0)>]method:int, sampleNumber:int) : Guid ResizeArray =
        let curve0 = RhinoScriptSyntax.CoerceCurve fromCurveId  
        let curve1 = RhinoScriptSyntax.CoerceCurve toCurveId  
        let mutable outCurves = Array.empty
        let tolerance = Doc.ModelAbsoluteTolerance
        if method = 0 then
            outCurves <- Curve.CreateTweenCurves(curve0, curve1, numberOfCurves,tolerance)
            outCurves <- Curve.CreateTweenCurvesWithMatching(curve0, curve1, numberOfCurves, tolerance)
        elif method = 2 then
            outCurves <- Curve.CreateTweenCurvesWithSampling(curve0, curve1, numberOfCurves, sampleNumber, tolerance)
        else failwithf "addTweenCurves Method must be 0, 1, || 2.  fromCurveId:'%A' toCurveId:'%A' numberOfCurves:'%A' method:'%A' sampleNumber:'%A'" fromCurveId toCurveId numberOfCurves method sampleNumber
        let curves = ResizeArray()
        if notNull outCurves then
            for curve in outCurves do
                if notNull curve && curve.IsValid then
                    let rc = Doc.Objects.AddCurve(curve)
                    //curve.Dispose()
                    if rc = Guid.Empty then  failwithf "addTweenCurves Unable to add curve to document.  fromCurveId:'%A' toCurveId:'%A' numberOfCurves:'%A' method:'%A' sampleNumber:'%A'" fromCurveId toCurveId numberOfCurves method sampleNumber
                    curves.Add(rc)
            Doc.Views.Redraw()
        curves
    (*
    def AddTweenCurves(from_curve_id, to_curve_id, number_of_curves = 1, method = 0, sample_number = 10):
        '''Creates curves between two open or closed input curves.
        Parameters:
          from_curve_id (guid): identifier of the first curve object.
          to_curve_id (guid): identifier of the second curve object.
          number_of_curves (number): The number of curves to create. The default is 1.
          method (number): The method for refining the output curves, where:
            0: (Default) Uses the control points of the curves for matching. So the first control point of first curve is matched to first control point of the second curve.
            1: Refits the output curves like using the FitCurve method.  Both the input curve and the output curve will have the same structure. The resulting curves are usually more complex than input unless input curves are compatible.
            2: Input curves are divided to the specified number of points on the curve, corresponding points define new points that output curves go through. If you are making one tween curve, the method essentially does the following: divides the two curves into an equal number of points, finds the midpoint between the corresponding points on the curves, and interpolates the tween curve through those points.
          sample_number (number): The number of samples points to use if method is 2. The default is 10.
        Returns:
          list(guid, ...): The identifiers of the new tween objects if successful, None on error.
        '''
        curve0 = rhutil.coercecurve(from_curve_id, -1, True)
        curve1 = rhutil.coercecurve(to_curve_id, -1, True)
        out_curves = 0
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        if method == 0:
            out_curves = Rhino.Geometry.Curve.CreateTweenCurves(curve0, curve1, number_of_curves, tolerance)
        elif method == 1:
            out_curves = Rhino.Geometry.Curve.CreateTweenCurvesWithMatching(curve0, curve1, number_of_curves, tolerance)
        elif method == 2:
            out_curves = Rhino.Geometry.Curve.CreateTweenCurvesWithSampling(curve0, curve1, number_of_curves, sample_number, tolerance)
        else: raise ValueError("method must be 0, 1, or 2")
        curves = None
        if out_curves:
            curves = []
            for curve in out_curves:
                if curve and curve.IsValid:
                    rc = scriptcontext.doc.Objects.AddCurve(curve)
                curve.Dispose()
                if rc==System.Guid.Empty: raise Exception("unable to add curve to document")
                curves.append(rc)
            scriptcontext.doc.Views.Redraw()
        return curves
    *)


