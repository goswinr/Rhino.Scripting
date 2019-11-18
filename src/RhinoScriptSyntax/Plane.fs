namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsPlane =

  type RhinoScriptSyntax with

    [<EXT>]
    ///<summary>Returns the distance from a 3D point to a plane</summary>
    ///<param name="plane">(Plane) The plane</param>
    ///<param name="point">(Point3d) List of 3 numbers or Point3d</param>
    ///<returns>(float) The distance , otherwise None</returns>
    static member DistanceToPlane(plane:Plane, point:Point3d) : float =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        plane.DistanceTo(point)


    [<EXT>]
    ///<summary>Evaluates a plane at a U,V parameter</summary>
    ///<param name="plane">(Plane) The plane to evaluate</param>
    ///<param name="u">(float) U parameter to evaluate</param>
    ///<param name="v">(float) V parameter to evaluate</param>
    ///<returns>(Point3d) Point3d on success</returns>
    static member EvaluatePlane(plane:Plane, u:float ,v: float) : Point3d =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        plane.PointAt(u,v)


    [<EXT>]
    ///<summary>Calculates the intersection of three planes</summary>
    ///<param name="plane1">(Plane) The 1st plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd plane to intersect</param>
    ///<param name="plane3">(Plane) The 3rd plane to intersect</param>
    ///<returns>(Point3d) the intersection point between the 3 planes on success</returns>
    static member IntersectPlanes( plane1:Plane,
                                   plane2:Plane,
                                   plane3:Plane) : Point3d =
        //plane1 = RhinoScriptSyntax.Coerceplane(plane1)
        //plane2 = RhinoScriptSyntax.Coerceplane(plane2)
        //plane3 = RhinoScriptSyntax.Coerceplane(plane3)
        let rc, point = Intersect.Intersection.PlanePlanePlane(plane1, plane2, plane3)
        if rc then point
        else failwithf "IntersectPlanes failed, are they paralell? %A; %A; %A" plane1 plane2 plane3


    [<EXT>]
    ///<summary>Moves the origin of a plane</summary>
    ///<param name="plane">(Plane) Plane or ConstructionPlane</param>
    ///<param name="origin">(Point3d) Point3d or list of three numbers</param>
    ///<returns>(Plane) moved plane</returns>
    static member MovePlane(plane:Plane, origin:Point3d) : Plane =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        //origin = RhinoScriptSyntax.Coerce3dpoint(origin)
        let mutable rc = Plane(plane)
        rc.Origin <- origin
        rc

    [<EXT>]
    ///<summary>Returns the point on a plane that is closest to a test point.</summary>
    ///<param name="plane">(Plane) The plane</param>
    ///<param name="point">(Point3d) The 3-D point to test.</param>
    ///<returns>(Point3d) the 3-D point</returns>
    static member PlaneClosestPoint( plane:Plane,  point:Point3d) : Point3d =
        plane.ClosestPoint(point)

    [<EXT>]

    ///<summary>Returns the point on a plane that is closest to a test point.</summary>
    ///<param name="plane">(Plane) The plane</param>
    ///<param name="point">(Point3d) The 3-D point to test.</param>
    ///<returns>(float,float) The u and v paramter on the plane of the closest point</returns>
    static member PlaneClosestParameter( plane:Plane, point:Point3d) : float*float =
        let rc, s,t = plane.ClosestParameter(point)
        if rc then s, t
        else failwithf "PlaneClosestParameter faild for %A; %A" plane point


    [<EXT>]
    ///<summary>Intersect an infinite plane and a curve object</summary>
    ///<param name="plane">(Plane) The plane to intersect.</param>
    ///<param name="curve">(Guid) The identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///The intersection tolerance. If omitted, the document's absolute tolerance is used.</param>
    ///<returns>(ResizeArray<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float* float * float >) a list of intersection information tuple .  The list will contain one or more of the following tuple:
    ///  Element Type        Description
    ///  [0]       Number      The intersection event type, either Point (1) or Overlap (2).
    ///  [1]       Point3d     If the event type is Point (1), then the intersection point on the curve.
    ///    If the event type is Overlap (2), then intersection start point on the curve.
    ///  [2]       Point3d     If the event type is Point (1), then the intersection point on the curve.
    ///    If the event type is Overlap (2), then intersection end point on the curve.
    ///  [3]       Point3d     If the event type is Point (1), then the intersection point on the plane.
    ///    If the event type is Overlap (2), then intersection start point on the plane.
    ///  [4]       Point3d     If the event type is Point (1), then the intersection point on the plane.
    ///    If the event type is Overlap (2), then intersection end point on the plane.
    ///  [5]       Number      If the event type is Point (1), then the curve parameter.
    ///    If the event type is Overlap (2), then the start value of the curve parameter range.
    ///  [6]       Number      If the event type is Point (1), then the curve parameter.
    ///    If the event type is Overlap (2),  then the end value of the curve parameter range.
    ///  [7]       Number      If the event type is Point (1), then the U plane parameter.
    ///    If the event type is Overlap (2), then the U plane parameter for curve at (n, 5).
    ///  [8]       Number      If the event type is Point (1), then the V plane parameter.
    ///    If the event type is Overlap (2), then the V plane parameter for curve at (n, 5).
    ///  [9]       Number      If the event type is Point (1), then the U plane parameter.
    ///    If the event type is Overlap (2), then the U plane parameter for curve at (n, 6).
    ///  [10]      Number      If the event type is Point (1), then the V plane parameter.
    ///    If the event type is Overlap (2), then the V plane parameter for curve at (n, 6).</returns>
    static member PlaneCurveIntersection( plane:Plane,
                                          curve:Guid,
                                          [<OPT;DEF(0.0)>]tolerance:float) : ResizeArray<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float* float * float > =
        let curve = RhinoScriptSyntax.CoerceCurve(curve)
        let  tolerance = if tolerance = 0.0 then  Doc.ModelAbsoluteTolerance else tolerance
        let intersections = Intersect.Intersection.CurvePlane(curve, plane, tolerance)
        if notNull intersections then
            let rc = ResizeArray()
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
                rc.Add( (a,b,c,d,e,f,g,h,i,j,k) )
            rc
        else
            failwithf "PlaneCurveIntersection faild on %A; %A tolerance %A" plane curve tolerance


    [<EXT>]
    ///<summary>Returns the equation of a plane as a tuple of four numbers. The standard
    ///  equation of a plane with a non-zero vector is Ax+By+Cz+D=0</summary>
    ///<param name="plane">(Plane) The plane to deconstruct</param>
    ///<returns>(float * float * float * float) containing four numbers that represent the coefficients of the equation  (A, B, C, D)</returns>
    static member PlaneEquation(plane:Plane) : float * float * float * float =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        let rc = plane.GetPlaneEquation()
        rc.[0], rc.[1], rc.[2], rc.[3]


    [<EXT>]
    ///<summary>Returns a plane that was fit through an array of 3D points.</summary>
    ///<param name="points">(Point3d seq) An array of 3D points.</param>
    ///<returns>(Plane) The plane</returns>
    static member PlaneFitFromPoints(points:Point3d seq) : Plane =
        //points = RhinoScriptSyntax.Coerce3dpointlist(points)
        let rc, plane = Plane.FitPlaneToPoints(points)
        if rc = PlaneFitResult.Success then plane
        else failwithf "PlaneFitFromPoints faild for %A" points


    [<EXT>]
    ///<summary>Construct a plane from a point, and two vectors in the plane.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the plane.</param>
    ///<param name="xAxis">(Vector3d) A non-zero 3D vector in the plane that determines the X axis
    ///  direction.</param>
    ///<param name="yAxis">(Vector3d) A non-zero 3D vector not parallel to xAxis that is used
    ///  to determine the Y axis direction. Note, yAxis does not
    ///  have to be perpendicular to xAxis.</param>
    ///<returns>(Plane) The plane .</returns>
    static member PlaneFromFrame( origin:Point3d,
                                  xAxis:Vector3d,
                                  yAxis:Vector3d) : Plane =
        //origin = RhinoScriptSyntax.Coerce3dpoint(origin)
        //xAxis = RhinoScriptSyntax.Coerce3dvector(xAxis)
        //yAxis = RhinoScriptSyntax.Coerce3dvector(yAxis)
        Plane(origin, xAxis, yAxis)


    [<EXT>]
    ///<summary>Creates a plane from an origin point and a normal direction vector.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the plane.</param>
    ///<param name="normal">(Vector3d) A 3D vector identifying the normal direction of the plane.</param>
    ///<param name="xaxis">(Vector3d) Optional, Default Value: <c>Vector3d()</c>
    ///Optional vector defining the plane's x-axis</param>
    ///<returns>(Plane) The plane .</returns>
    static member PlaneFromNormal( origin:Point3d,
                                   normal:Vector3d,
                                   [<OPT;DEF(Vector3d())>]xaxis:Vector3d) : Plane =
        //origin = RhinoScriptSyntax.Coerce3dpoint(origin)
        //normal = RhinoScriptSyntax.Coerce3dvector(normal)
        let mutable rc = Plane(origin, normal)
        if xaxis.IsValid then
            //xaxis = RhinoScriptSyntax.Coerce3dvector(xaxis)
            let xaxis = Vector3d(xaxis)//prevent original xaxis parameter from being unitized too
            xaxis.Unitize() |> ignore
            let yaxis = Vector3d.CrossProduct(rc.Normal, xaxis)
            rc <- Plane(origin, xaxis, yaxis)
        rc


    [<EXT>]
    ///<summary>Creates a plane from three non-colinear points</summary>
    ///<param name="origin">(Point3d) Origin point of the plane</param>
    ///<param name="x">(Point3d) X point on the plane's x  axis</param>
    ///<param name="y">(Point3d) Y point on the plane's y axis</param>
    ///<returns>(Plane) The plane , otherwise None</returns>
    static member PlaneFromPoints( origin:Point3d,
                                   x:Point3d,
                                   y:Point3d) : Plane =
        //origin = RhinoScriptSyntax.Coerce3dpoint(origin)
        //x = RhinoScriptSyntax.Coerce3dpoint(x)
        //y = RhinoScriptSyntax.Coerce3dpoint(y)
        let plane = Plane(origin, x, y)
        if plane.IsValid then plane
        else failwithf "PlaneFromPoints failed for %A; %A; %A" origin x y


    [<EXT>]
    ///<summary>Calculates the intersection of two planes</summary>
    ///<param name="plane1">(Plane) The 1st plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd plane to intersect</param>
    ///<returns>(Line) a line with two 3d points identifying the starting/ending points of the intersection</returns>
    static member PlanePlaneIntersection(plane1:Plane, plane2:Plane) : Line =
        //plane1 = RhinoScriptSyntax.Coerceplane(plane1)
        //plane2 = RhinoScriptSyntax.Coerceplane(plane2)
        let rc, line = Intersect.Intersection.PlanePlane(plane1, plane2)
        if rc then line
        else failwithf "PlanePlaneIntersection failed for %A; %A " plane1 plane2


    [<EXT>]
    ///<summary>Calculates the intersection of a plane and a sphere</summary>
    ///<param name="plane">(Plane) The plane to intersect</param>
    ///<param name="spherePlane">(Plane) Equatorial plane of the sphere. origin of the plane is
    ///  the center of the sphere</param>
    ///<param name="sphereRadius">(float) Radius of the sphere</param>
    ///<returns>(float * Plane * float) of intersection results
    ///  Element    Type      Description
    ///  [0]       number     The type of intersection, where 0 = point and 1 = circle.
    ///  [1]      plane  If a point intersection, the a Point3d identifying the 3-D intersection location is plane.Origin
    ///                  If a circle intersection, then the circle's plane. The origin of the plane will be the center point of the circle
    ///  [2]       number     If a circle intersection, then the radius of the circle.</returns>
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
            failwithf "PlaneSphereIntersection failed for %A; %A, %A " plane spherePlane sphereRadius


    [<EXT>]
    ///<summary>Transforms a plane</summary>
    ///<param name="plane">(Plane) Plane to transform</param>
    ///<param name="xform">(Transform) Transformation to apply</param>
    ///<returns>(Plane) the resulting plane</returns>
    static member PlaneTransform(plane:Plane, xform:Transform) : Plane =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        //xform = RhinoScriptSyntax.Coercexform(xform)
        let rc = Plane(plane)
        if rc.Transform(xform) then rc
        else failwithf "PlaneTransform faild for %A; %A" plane xform


    [<EXT>]
    ///<summary>Rotates a plane</summary>
    ///<param name="plane">(Plane) Plane to rotate</param>
    ///<param name="angleDegrees">(float) Rotation angle in degrees</param>
    ///<param name="axis">(Vector3d) Axis of rotation or list of three numbers</param>
    ///<returns>(Plane) rotated plane on success</returns>
    static member RotatePlane( plane:Plane,
                               angleDegrees:float,
                               axis:Vector3d) : Plane =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        //axis = RhinoScriptSyntax.Coerce3dvector(axis)
        let angleradians = toRadians(angleDegrees)
        let rc = Plane(plane)
        if rc.Rotate(angleradians, axis) then rc
        else failwithf "RotatePlane failed for %A; %A; %A" plane angleDegrees axis


    [<EXT>]
    ///<summary>Returns Rhino's world XY plane</summary>
    ///<returns>(Plane) Rhino's world XY plane</returns>
    static member WorldXYPlane() : Plane =
        Plane.WorldXY


    [<EXT>]
    ///<summary>Returns Rhino's world YZ plane</summary>
    ///<returns>(Plane) Rhino's world YZ plane</returns>
    static member WorldYZPlane() : Plane =
        Plane.WorldYZ


    [<EXT>]
    ///<summary>Returns Rhino's world ZX plane</summary>
    ///<returns>(Plane) Rhino's world ZX plane</returns>
    static member WorldZXPlane() : Plane =
        Plane.WorldZX


