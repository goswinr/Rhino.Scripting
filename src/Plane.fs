namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsPlane =
  type RhinoScriptSyntax with
    
    ///<summary>Returns the distance from a 3D point to a plane</summary>
    ///<param name="plane">(Plane) The plane</param>
    ///<param name="point">(Point3d) List of 3 numbers or Point3d</param>
    ///<returns>(float) The distance , otherwise None</returns>
    static member DistanceToPlane(plane:Plane, point:Point3d) : float =
        failNotImpl () 


    ///<summary>Evaluates a plane at a U,V parameter</summary>
    ///<param name="plane">(Plane) The plane to evaluate</param>
    ///<param name="parameter">(float * float) List of two numbers defining the U,V parameter to evaluate</param>
    ///<returns>(Point3d) Point3d on success</returns>
    static member EvaluatePlane(plane:Plane, parameter:float * float) : Point3d =
        failNotImpl () 


    ///<summary>Calculates the intersection of three planes</summary>
    ///<param name="plane1">(Plane) The 1st plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd plane to intersect</param>
    ///<param name="plane3">(Plane) The 3rd plane to intersect</param>
    ///<returns>(Point3d) the intersection point between the 3 planes on success</returns>
    static member IntersectPlanes(plane1:Plane, plane2:Plane, plane3:Plane) : Point3d =
        failNotImpl () 


    ///<summary>Moves the origin of a plane</summary>
    ///<param name="plane">(Plane) Plane or ConstructionPlane</param>
    ///<param name="origin">(Point3d) Point3d or list of three numbers</param>
    ///<returns>(Plane) moved plane</returns>
    static member MovePlane(plane:Plane, origin:Point3d) : Plane =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Returns the point on a plane that is closest to a test point.</summary>
    ///<param name="plane">(Plane) The plane</param>
    ///<param name="point">(Point3d) The 3-D point to test.</param>
    ///<param name="returnPoint">(bool) Optional, Default Value: <c>true</c>
    ///If omitted or True, then the point on the plane
    ///  that is closest to the test point is returned. If False, then the
    ///  parameter of the point on the plane that is closest to the test
    ///  point is returned.</param>
    ///<returns>(Point3d) If return_point is omitted or True, then the 3-D point</returns>
    static member PlaneClosestPoint(plane:Plane, point:Point3d, [<OPT;DEF(true)>]returnPoint:bool) : Point3d =
        failNotImpl () 


    ///<summary>Intersect an infinite plane and a curve object</summary>
    ///<param name="plane">(Plane) The plane to intersect.</param>
    ///<param name="curve">(Guid) The identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///The intersection tolerance. If omitted, the document's absolute tolerance is used.</param>
    ///<returns>(float * Point3d * Point3d * Point3d * Point3d * float * float * float * float * float) a list of intersection information tuple .  The list will contain one or more of the following tuple:
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
    static member PlaneCurveIntersection(plane:Plane, curve:Guid, [<OPT;DEF(null)>]tolerance:float) : float * Point3d * Point3d * Point3d * Point3d * float * float * float * float * float =
        failNotImpl () 


    ///<summary>Returns the equation of a plane as a tuple of four numbers. The standard
    ///  equation of a plane with a non-zero vector is Ax+By+Cz+D=0</summary>
    ///<param name="plane">(Plane) The plane to deconstruct</param>
    ///<returns>(float * float * float * float) containing four numbers that represent the coefficients of the equation  (A, B, C, D)</returns>
    static member PlaneEquation(plane:Plane) : float * float * float * float =
        failNotImpl () 


    ///<summary>Returns a plane that was fit through an array of 3D points.</summary>
    ///<param name="points">(Point3d) An array of 3D points.</param>
    ///<returns>(Plane) The plane</returns>
    static member PlaneFitFromPoints(points:Point3d) : Plane =
        failNotImpl () 


    ///<summary>Construct a plane from a point, and two vectors in the plane.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the plane.</param>
    ///<param name="xAxis">(Vector3d) A non-zero 3D vector in the plane that determines the X axis
    ///  direction.</param>
    ///<param name="yAxis">(Vector3d) A non-zero 3D vector not parallel to xAxis that is used
    ///  to determine the Y axis direction. Note, yAxis does not
    ///  have to be perpendicular to xAxis.</param>
    ///<returns>(Plane) The plane .</returns>
    static member PlaneFromFrame(origin:Point3d, xAxis:Vector3d, yAxis:Vector3d) : Plane =
        failNotImpl () 


    ///<summary>Creates a plane from an origin point and a normal direction vector.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the plane.</param>
    ///<param name="normal">(Vector3d) A 3D vector identifying the normal direction of the plane.</param>
    ///<param name="xaxis">(Vector3d) Optional, Default Value: <c>null</c>
    ///Optional vector defining the plane's x-axis</param>
    ///<returns>(Plane) The plane .</returns>
    static member PlaneFromNormal(origin:Point3d, normal:Vector3d, [<OPT;DEF(null)>]xaxis:Vector3d) : Plane =
        failNotImpl () 


    ///<summary>Creates a plane from three non-colinear points</summary>
    ///<param name="origin">(Point3d) Origin point of the plane</param>
    ///<param name="x">(Point3d) X of 'points on the plane's x and y axes' (FIXME 0)</param>
    ///<param name="y">(Point3d) Y of 'points on the plane's x and y axes' (FIXME 0)</param>
    ///<returns>(Plane) The plane , otherwise None</returns>
    static member PlaneFromPoints(origin:Point3d, x:Point3d, y:Point3d) : Plane =
        failNotImpl () 


    ///<summary>Calculates the intersection of two planes</summary>
    ///<param name="plane1">(Plane) The 1st plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd plane to intersect</param>
    ///<returns>(Line) a line with two 3d points identifying the starting/ending points of the intersection</returns>
    static member PlanePlaneIntersection(plane1:Plane, plane2:Plane) : Line =
        failNotImpl () 


    ///<summary>Calculates the intersection of a plane and a sphere</summary>
    ///<param name="plane">(Plane) The plane to intersect</param>
    ///<param name="spherePlane">(Plane) Equatorial plane of the sphere. origin of the plane is
    ///  the center of the sphere</param>
    ///<param name="sphereRadius">(float) Radius of the sphere</param>
    ///<returns>(float * Plane * float) of intersection results
    ///  Element    Type      Description
    ///  [0]       number     The type of intersection, where 0 = point and 1 = circle.
    ///  [1]   point or plane If a point intersection, the a Point3d identifying the 3-D intersection location.
    ///    If a circle intersection, then the circle's plane. The origin of the plane will be the center point of the circle
    ///  [2]       number     If a circle intersection, then the radius of the circle.</returns>
    static member PlaneSphereIntersection(plane:Plane, spherePlane:Plane, sphereRadius:float) : float * Plane * float =
        failNotImpl () 


    ///<summary>Transforms a plane</summary>
    ///<param name="plane">(Plane) Plane to transform</param>
    ///<param name="xform">(Transform) Transformation to apply</param>
    ///<returns>(Plane) the resulting plane</returns>
    static member PlaneTransform(plane:Plane, xform:Transform) : Plane =
        failNotImpl () 


    ///<summary>Rotates a plane</summary>
    ///<param name="plane">(Plane) Plane to rotate</param>
    ///<param name="angleDegrees">(int) Rotation angle in degrees</param>
    ///<param name="axis">(Vector3d) Axis of rotation or list of three numbers</param>
    ///<returns>(Plane) rotated plane on success</returns>
    static member RotatePlane(plane:Plane, angleDegrees:int, axis:Vector3d) : Plane =
        failNotImpl () 


    ///<summary>Returns Rhino's world XY plane</summary>
    ///<returns>(Plane) Rhino's world XY plane</returns>
    static member WorldXYPlane() : Plane =
        failNotImpl () 


    ///<summary>Returns Rhino's world YZ plane</summary>
    ///<returns>(Plane) Rhino's world YZ plane</returns>
    static member WorldYZPlane() : Plane =
        failNotImpl () 


    ///<summary>Returns Rhino's world ZX plane</summary>
    ///<returns>(Plane) Rhino's world ZX plane</returns>
    static member WorldZXPlane() : Plane =
        failNotImpl () 


