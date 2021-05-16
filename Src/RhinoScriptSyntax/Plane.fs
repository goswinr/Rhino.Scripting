namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.UtilMath

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore
 
[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namespace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsPlane =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    ///<summary>Returns the distance from a 3D point to a Plane.</summary>
    ///<param name="plane">(Plane) The Plane</param>
    ///<param name="point">(Point3d) List of 3 numbers or Point3d</param>
    ///<returns>(float) The distance.</returns>
    [<Extension>]
    static member DistanceToPlane(plane:Plane, point:Point3d) : float =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        plane.DistanceTo(point)


    ///<summary>Evaluates a Plane at a U, V parameter.</summary>
    ///<param name="plane">(Plane) The Plane to evaluate</param>
    ///<param name="u">(float) U parameter to evaluate</param>
    ///<param name="v">(float) V parameter to evaluate</param>
    ///<returns>(Point3d) Point3d.</returns>
    [<Extension>]
    static member EvaluatePlane(plane:Plane, u:float , v: float) : Point3d =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        plane.PointAt(u, v)


    ///<summary>Calculates the intersection of three Planes.</summary>
    ///<param name="plane1">(Plane) The 1st Plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd Plane to intersect</param>
    ///<param name="plane3">(Plane) The 3rd Plane to intersect</param>
    ///<returns>(Point3d) The intersection point between the 3 Planes.</returns>
    [<Extension>]
    static member IntersectPlanes( plane1:Plane,
                                   plane2:Plane,
                                   plane3:Plane) : Point3d =
        //plane1 = RhinoScriptSyntax.Coerceplane(plane1)
        //plane2 = RhinoScriptSyntax.Coerceplane(plane2)
        //plane3 = RhinoScriptSyntax.Coerceplane(plane3)
        let rc, point = Intersect.Intersection.PlanePlanePlane(plane1, plane2, plane3)
        if rc then point
        else RhinoScriptingException.Raise "RhinoScriptSyntax.IntersectPlanes failed, are they paralell? %A; %A; %A" plane1 plane2 plane3


    ///<summary>Moves the origin of a Plane.</summary>
    ///<param name="plane">(Plane) Plane </param>
    ///<param name="origin">(Point3d) Point3d or list of three numbers</param>
    ///<returns>(Plane) moved Plane.</returns>
    [<Extension>]
    static member MovePlane(plane:Plane, origin:Point3d) : Plane =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        //origin = RhinoScriptSyntax.Coerce3dpoint(origin)
        let mutable rc = Plane(plane)
        rc.Origin <- origin
        rc

    ///<summary>Flip this Plane by swapping out the X and Y axes and inverting the Z axis.</summary>
    ///<param name="plane">(Plane) Plane </param>
    ///<returns>(Plane) moved Plane.</returns>
    [<Extension>]
    static member FlipPlane(plane:Plane) : Plane =
        let pl = Plane(plane)
        pl.Flip()
        pl


    ///<summary>Returns the point on a Plane that is closest to a test point.</summary>
    ///<param name="plane">(Plane) The Plane</param>
    ///<param name="point">(Point3d) The 3-D point to test</param>
    ///<returns>(Point3d) The 3-D point.</returns>
    [<Extension>]
    static member PlaneClosestPoint( plane:Plane, point:Point3d) : Point3d =
        plane.ClosestPoint(point)

    
    ///<summary>Returns the point on a Plane that is closest to a test point.</summary>
    ///<param name="plane">(Plane) The Plane</param>
    ///<param name="point">(Point3d) The 3-D point to test</param>
    ///<returns>(float*float) The u and v paramter on the Plane of the closest point.</returns>
    [<Extension>]
    static member PlaneClosestParameter( plane:Plane, point:Point3d) : float*float =
        let rc, s, t = plane.ClosestParameter(point)
        if rc then s, t
        else RhinoScriptingException.Raise "RhinoScriptSyntax.PlaneClosestParameter failed for %A; %A" plane point


    ///<summary>Intersect an infinite Plane and a Curve object.</summary>
    ///<param name="plane">(Plane) The Plane to intersect</param>
    ///<param name="curve">(Guid) The identifier of the Curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The intersection tolerance.</param>
    ///<returns>(Rarr of int * Point3d * Point3d * Point3d * Point3d * float * float * float * float* float * float) a list of intersection information tuple . The list will contain one or more of the following tuple:
    ///    Element Type        Description
    ///    [0]       Number      The intersection event type, either Point (1) or Overlap (2).
    ///    [1]       Point3d     If the event type is Point (1), then the intersection point on the Curve.
    ///      If the event type is Overlap (2), then intersection start point on the Curve.
    ///    [2]       Point3d     If the event type is Point (1), then the intersection point on the Curve.
    ///      If the event type is Overlap (2), then intersection end point on the Curve.
    ///    [3]       Point3d     If the event type is Point (1), then the intersection point on the Plane.
    ///      If the event type is Overlap (2), then intersection start point on the Plane.
    ///    [4]       Point3d     If the event type is Point (1), then the intersection point on the Plane.
    ///      If the event type is Overlap (2), then intersection end point on the Plane.
    ///    [5]       Number      If the event type is Point (1), then the Curve parameter.
    ///      If the event type is Overlap (2), then the start value of the Curve parameter range.
    ///    [6]       Number      If the event type is Point (1), then the Curve parameter.
    ///      If the event type is Overlap (2), then the end value of the Curve parameter range.
    ///    [7]       Number      If the event type is Point (1), then the U Plane parameter.
    ///      If the event type is Overlap (2), then the U Plane parameter for Curve at (n, 5).
    ///    [8]       Number      If the event type is Point (1), then the V Plane parameter.
    ///      If the event type is Overlap (2), then the V Plane parameter for Curve at (n, 5).
    ///    [9]       Number      If the event type is Point (1), then the U Plane parameter.
    ///      If the event type is Overlap (2), then the U Plane parameter for Curve at (n, 6).
    ///    [10]      Number      If the event type is Point (1), then the V Plane parameter.
    ///      If the event type is Overlap (2), then the V Plane parameter for Curve at (n, 6).</returns>
    [<Extension>]
    static member PlaneCurveIntersection( plane:Plane,
                                          curve:Guid,
                                          [<OPT;DEF(0.0)>]tolerance:float) : Rarr<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float* float * float > =
        let curve = RhinoScriptSyntax.CoerceCurve(curve)
        let  tolerance = if tolerance = 0.0 then  State.Doc.ModelAbsoluteTolerance else tolerance
        let intersections = Intersect.Intersection.CurvePlane(curve, plane, tolerance)
        if notNull intersections then
            let rc = Rarr()
            for intersection in intersections do
                let mutable a = 1
                if intersection.IsOverlap then a <- 2
                let b = intersection.PointA
                let c = intersection.PointA2
                let d = intersection.PointB
                let e = intersection.PointB2
                let f = intersection.ParameterA
                let g = intersection.ParameterB
                let h = intersection.OverlapA.[0]
                let i = intersection.OverlapA.[1]
                let j = intersection.OverlapB.[0]
                let k = intersection.OverlapB.[1]
                rc.Add( (a, b, c, d, e, f, g, h, i, j, k))
            rc
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.PlaneCurveIntersection failed on %A; %A tolerance %A" plane curve tolerance


    ///<summary>Returns the equation of a Plane as a tuple of four numbers. The standard
    ///    equation of a Plane with a non-zero vector is Ax + By + Cz + D = 0.</summary>
    ///<param name="plane">(Plane) The Plane to deconstruct</param>
    ///<returns>(float * float * float * float) containing four numbers that represent the coefficients of the equation  (A, B, C, D).</returns>
    [<Extension>]
    static member PlaneEquation(plane:Plane) : float * float * float * float =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        let rc = plane.GetPlaneEquation()
        rc.[0], rc.[1], rc.[2], rc.[3]


    ///<summary>Returns a Plane that was fit through an array of 3D points.</summary>
    ///<param name="points">(Point3d seq) An array of 3D points</param>
    ///<returns>(Plane) The Plane.</returns>
    [<Extension>]
    static member PlaneFitFromPoints(points:Point3d seq) : Plane =
        //points = RhinoScriptSyntax.Coerce3dpointlist(points)
        let rc, plane = Plane.FitPlaneToPoints(points)
        if rc = PlaneFitResult.Success then plane
        else RhinoScriptingException.Raise "RhinoScriptSyntax.PlaneFitFromPoints failed for %A" points


    ///<summary>Construct a Plane from a point, and two vectors in the Plane.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the Plane</param>
    ///<param name="xAxis">(Vector3d) A non-zero 3D vector in the Plane that determines the X axis
    ///    direction</param>
    ///<param name="yAxis">(Vector3d) A non-zero 3D vector not parallel to xAxis that is used
    ///    to determine the Y axis direction. Note, yAxis does not
    ///    have to be perpendicular to xAxis</param>
    ///<returns>(Plane) The Plane.</returns>
    [<Extension>]
    static member PlaneFromFrame( origin:Point3d,
                                  xAxis:Vector3d,
                                  yAxis:Vector3d) : Plane =
        //origin = RhinoScriptSyntax.Coerce3dpoint(origin)
        //xAxis = RhinoScriptSyntax.Coerce3dvector(xAxis)
        //yAxis = RhinoScriptSyntax.Coerce3dvector(yAxis)
        Plane(origin, xAxis, yAxis)


    ///<summary>Creates a Plane from an origin point and a normal direction vector.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the Plane</param>
    ///<param name="normal">(Vector3d) A 3D vector identifying the normal direction of the Plane</param>
    ///<param name="xaxis">(Vector3d) Optional, vector defining the Plane's x-axis</param>
    ///<returns>(Plane) The Plane.</returns>
    [<Extension>]
    static member PlaneFromNormal( origin:Point3d,
                                   normal:Vector3d,
                                   [<OPT;DEF(Vector3d())>]xaxis:Vector3d) : Plane =
        //origin = RhinoScriptSyntax.Coerce3dpoint(origin)
        //normal = RhinoScriptSyntax.Coerce3dvector(normal)
        let mutable rc = Plane(origin, normal)
        if not xaxis.IsZero then
            //xaxis = RhinoScriptSyntax.Coerce3dvector(xaxis)
            let xaxis = Vector3d(xaxis)//prevent original xaxis parameter from being unitized too
            xaxis.Unitize() |> ignore
            let yaxis = Vector3d.CrossProduct(rc.Normal, xaxis)
            rc <- Plane(origin, xaxis, yaxis)
        rc


    ///<summary>Creates a Plane from three non-colinear points.</summary>
    ///<param name="origin">(Point3d) Origin point of the Plane</param>
    ///<param name="x">(Point3d) X point on the Plane's x  axis</param>
    ///<param name="y">(Point3d) Y point on the Plane's y axis</param>
    ///<returns>(Plane) The Plane.</returns>
    [<Extension>]
    static member PlaneFromPoints( origin:Point3d,
                                   x:Point3d,
                                   y:Point3d) : Plane =
        //origin = RhinoScriptSyntax.Coerce3dpoint(origin)
        //x = RhinoScriptSyntax.Coerce3dpoint(x)
        //y = RhinoScriptSyntax.Coerce3dpoint(y)
        let plane = Plane(origin, x, y)
        if plane.IsValid then plane
        else RhinoScriptingException.Raise "RhinoScriptSyntax.PlaneFromPoints failed for %A; %A; %A" origin x y


    ///<summary>Calculates the intersection of two Planes.</summary>
    ///<param name="plane1">(Plane) The 1st Plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd Plane to intersect</param>
    ///<returns>(Line) a line with two 3d points identifying the starting/ending points of the intersection.</returns>
    [<Extension>]
    static member PlanePlaneIntersection(plane1:Plane, plane2:Plane) : Line =
        //plane1 = RhinoScriptSyntax.Coerceplane(plane1)
        //plane2 = RhinoScriptSyntax.Coerceplane(plane2)
        let rc, line = Intersect.Intersection.PlanePlane(plane1, plane2)
        if rc then line
        else RhinoScriptingException.Raise "RhinoScriptSyntax.PlanePlaneIntersection failed for %A; %A" plane1 plane2


    ///<summary>Calculates the intersection of a Plane and a sphere.</summary>
    ///<param name="plane">(Plane) The Plane to intersect</param>
    ///<param name="spherePlane">(Plane) Equatorial Plane of the sphere. origin of the Plane is
    ///    the center of the sphere</param>
    ///<param name="sphereRadius">(float) Radius of the sphere</param>
    ///<returns>(int * Plane * float) of intersection results
    ///    Element  Type      Description
    ///    [0]      number     The type of intersection, where 0 = point and 1 = circle.
    ///    [1]      Plane      If a point intersection, the a Point3d identifying the 3-D intersection location is Plane.Origin
    ///                        If a circle intersection, then the circle's Plane. The origin of the Plane will be the center point of the circle
    ///    [2]      number     If a circle intersection, then the radius of the circle.</returns>
    [<Extension>]
    static member PlaneSphereIntersection( plane:Plane,
                                           spherePlane:Plane,
                                           sphereRadius:float) : int * Plane * float =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        //spherePlane = RhinoScriptSyntax.Coerceplane(spherePlane)
        let sphere = Sphere(spherePlane, sphereRadius)
        let rc, circle = Intersect.Intersection.PlaneSphere(plane, sphere)
        if rc = Intersect.PlaneSphereIntersection.Point then
            0, circle.Plane, circle.Radius //was just circle.Center
        elif rc = Intersect.PlaneSphereIntersection.Circle then
            1, circle.Plane, circle.Radius
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.PlaneSphereIntersection failed for %A; %A, %A" plane spherePlane sphereRadius


    ///<summary>Transforms a Plane.</summary>
    ///<param name="plane">(Plane) Plane to transform</param>
    ///<param name="xForm">(Transform) Transformation to apply</param>
    ///<returns>(Plane) The resulting Plane.</returns>
    [<Extension>]
    static member PlaneTransform(plane:Plane, xForm:Transform) : Plane =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        //xForm = RhinoScriptSyntax.CoercexForm(xForm)
        let rc = Plane(plane)
        if rc.Transform(xForm) then rc
        else RhinoScriptingException.Raise "RhinoScriptSyntax.PlaneTransform failed for %A; %A" plane xForm


    ///<summary>Rotates a Plane.</summary>
    ///<param name="plane">(Plane) Plane to rotate</param>
    ///<param name="angleDegrees">(float) Rotation angle in degrees</param>
    ///<param name="axis">(Vector3d) Axis of rotation or list of three numbers</param>
    ///<returns>(Plane) rotated Plane.</returns>
    [<Extension>]
    static member RotatePlane( plane:Plane,
                               angleDegrees:float,
                               axis:Vector3d) : Plane =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        //axis = RhinoScriptSyntax.Coerce3dvector(axis)
        let angleradians = toRadians(angleDegrees)
        let rc = Plane(plane)
        if rc.Rotate(angleradians, axis) then rc
        else RhinoScriptingException.Raise "RhinoScriptSyntax.RotatePlane failed for %A; %A; %A" plane angleDegrees axis


    ///<summary>Returns Rhino's world XY Plane.</summary>
    ///<returns>(Plane) Rhino's world XY Plane.</returns>
    [<Extension>]
    static member WorldXYPlane() : Plane =
        Plane.WorldXY


    ///<summary>Returns Rhino's world YZ Plane.</summary>
    ///<returns>(Plane) Rhino's world YZ Plane.</returns>
    [<Extension>]
    static member WorldYZPlane() : Plane =
        Plane.WorldYZ


    ///<summary>Returns Rhino's world ZX Plane.</summary>
    ///<returns>(Plane) Rhino's world ZX Plane.</returns>
    [<Extension>]
    static member WorldZXPlane() : Plane =
        Plane.WorldZX


