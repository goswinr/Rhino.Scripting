
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
module AutoOpenPlane =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>Returns the distance from a 3D point to a Plane.</summary>
    ///<param name="plane">(Plane) The Plane</param>
    ///<param name="point">(Point3d) List of 3 numbers or Point3d</param>
    ///<returns>(float) The distance.</returns>
    static member DistanceToPlane(plane:Plane, point:Point3d) : float = 
        //plane = Scripting.Coerceplane(plane)
        //point = Scripting.Coerce3dpoint(point)
        plane.DistanceTo(point)


    ///<summary>Evaluates a Plane at a U, V parameter.</summary>
    ///<param name="plane">(Plane) The Plane to evaluate</param>
    ///<param name="u">(float) U parameter to evaluate</param>
    ///<param name="v">(float) V parameter to evaluate</param>
    ///<returns>(Point3d) Point3d.</returns>
    static member EvaluatePlane(plane:Plane, u:float , v: float) : Point3d = 
        //plane = Scripting.Coerceplane(plane)
        plane.PointAt(u, v)


    ///<summary>Calculates the intersection of three Planes.</summary>
    ///<param name="plane1">(Plane) The 1st Plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd Plane to intersect</param>
    ///<param name="plane3">(Plane) The 3rd Plane to intersect</param>
    ///<returns>(Point3d) The intersection point between the 3 Planes.</returns>
    static member IntersectPlanes( plane1:Plane,
                                   plane2:Plane,
                                   plane3:Plane) : Point3d = 
        //plane1 = Scripting.Coerceplane(plane1)
        //plane2 = Scripting.Coerceplane(plane2)
        //plane3 = Scripting.Coerceplane(plane3)
        let rc, point = Intersect.Intersection.PlanePlanePlane(plane1, plane2, plane3)
        if rc then point
        else RhinoScriptingException.Raise "Rhino.Scripting.IntersectPlanes failed, are they parallel? %A; %A; %A" plane1 plane2 plane3


    ///<summary>Moves the origin of a Plane.</summary>
    ///<param name="plane">(Plane) Plane </param>
    ///<param name="origin">(Point3d) Point3d or list of three numbers</param>
    ///<returns>(Plane) moved Plane.</returns>
    static member MovePlane(plane:Plane, origin:Point3d) : Plane = 
        //plane = Scripting.Coerceplane(plane)
        //origin = Scripting.Coerce3dpoint(origin)
        let mutable rc = Plane(plane)
        rc.Origin <- origin
        rc

    ///<summary>Flip this Plane by swapping out the X and Y axes and inverting the Z axis.</summary>
    ///<param name="plane">(Plane) Plane </param>
    ///<returns>(Plane) moved Plane.</returns>
    static member FlipPlane(plane:Plane) : Plane = 
        let pl = Plane(plane)
        pl.Flip()
        pl


    ///<summary>Returns the point on a Plane that is closest to a test point.</summary>
    ///<param name="plane">(Plane) The Plane</param>
    ///<param name="point">(Point3d) The 3-D point to test</param>
    ///<returns>(Point3d) The 3-D point.</returns>
    static member PlaneClosestPoint( plane:Plane, point:Point3d) : Point3d = 
        plane.ClosestPoint(point)


    ///<summary>Returns the point on a Plane that is closest to a test point.</summary>
    ///<param name="plane">(Plane) The Plane</param>
    ///<param name="point">(Point3d) The 3-D point to test</param>
    ///<returns>(float*float) The u and v parameter on the Plane of the closest point.</returns>
    static member PlaneClosestParameter( plane:Plane, point:Point3d) : float*float = 
        let rc, s, t = plane.ClosestParameter(point)
        if rc then s, t
        else RhinoScriptingException.Raise "Rhino.Scripting.PlaneClosestParameter failed for %A; %A" plane point


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
    static member PlaneCurveIntersection( plane:Plane,
                                          curve:Guid,
                                          [<OPT;DEF(0.0)>]tolerance:float) : Rarr<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float* float * float > = 
        let curve = Scripting.CoerceCurve(curve)
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
            RhinoScriptingException.Raise "Rhino.Scripting.PlaneCurveIntersection failed on %A; %A tolerance %A" plane curve tolerance


    ///<summary>Returns the equation of a Plane as a tuple of four numbers. The standard
    ///    equation of a Plane with a non-zero vector is Ax + By + Cz + D = 0.</summary>
    ///<param name="plane">(Plane) The Plane to deconstruct</param>
    ///<returns>(float * float * float * float) containing four numbers that represent the coefficients of the equation  (A, B, C, D).</returns>
    static member PlaneEquation(plane:Plane) : float * float * float * float = 
        //plane = Scripting.Coerceplane(plane)
        let rc = plane.GetPlaneEquation()
        rc.[0], rc.[1], rc.[2], rc.[3]


    ///<summary>Returns a Plane that was fit through an array of 3D points.</summary>
    ///<param name="points">(Point3d seq) An array of 3D points</param>
    ///<returns>(Plane) The Plane.</returns>
    static member PlaneFitFromPoints(points:Point3d seq) : Plane = 
        //points = Scripting.Coerce3dpointlist(points)
        let rc, plane = Plane.FitPlaneToPoints(points)
        if rc = PlaneFitResult.Success then plane
        else RhinoScriptingException.Raise "Rhino.Scripting.PlaneFitFromPoints failed for %A" points


    ///<summary>Construct a Plane from a point, and two vectors in the Plane.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the Plane</param>
    ///<param name="xAxis">(Vector3d) A non-zero 3D vector in the Plane that determines the X axis
    ///    direction</param>
    ///<param name="yAxis">(Vector3d) A non-zero 3D vector not parallel to xAxis that is used
    ///    to determine the Y axis direction. Note, yAxis does not
    ///    have to be perpendicular to xAxis</param>
    ///<returns>(Plane) The Plane.</returns>
    static member PlaneFromFrame( origin:Point3d,
                                  xAxis:Vector3d,
                                  yAxis:Vector3d) : Plane = 
        //origin = Scripting.Coerce3dpoint(origin)
        //xAxis = Scripting.Coerce3dvector(xAxis)
        //yAxis = Scripting.Coerce3dvector(yAxis)
        Plane(origin, xAxis, yAxis)


    ///<summary>Creates a Plane from an origin point and a normal direction vector.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the Plane</param>
    ///<param name="normal">(Vector3d) A 3D vector identifying the normal direction of the Plane</param>
    ///<param name="xAxis">(Vector3d) Optional, vector defining the Plane's x-axis</param>
    ///<returns>(Plane) The Plane.</returns>
    static member PlaneFromNormal( origin:Point3d,
                                   normal:Vector3d,
                                   [<OPT;DEF(Vector3d())>]xAxis:Vector3d) : Plane = 
        //origin = Scripting.Coerce3dpoint(origin)
        //normal = Scripting.Coerce3dvector(normal)
        let mutable rc = Plane(origin, normal)
        if not xAxis.IsZero then
            //x axis = Scripting.Coerce3dvector(x axis)
            let xAxis = Vector3d(xAxis)//prevent original x axis parameter from being unitized too
            xAxis.Unitize() |> ignore
            let yAxis = Vector3d.CrossProduct(rc.Normal, xAxis)
            rc <- Plane(origin, xAxis, yAxis)
        rc


    ///<summary>Creates a Plane from three non-collinear points.</summary>
    ///<param name="origin">(Point3d) Origin point of the Plane</param>
    ///<param name="x">(Point3d) X point on the Plane's x  axis</param>
    ///<param name="y">(Point3d) Y point on the Plane's y axis</param>
    ///<returns>(Plane) The Plane.</returns>
    static member PlaneFromPoints( origin:Point3d,
                                   x:Point3d,
                                   y:Point3d) : Plane = 
        //origin = Scripting.Coerce3dpoint(origin)
        //x = Scripting.Coerce3dpoint(x)
        //y = Scripting.Coerce3dpoint(y)
        let plane = Plane(origin, x, y)
        if plane.IsValid then plane
        else RhinoScriptingException.Raise "Rhino.Scripting.PlaneFromPoints failed for %A; %A; %A" origin x y


    ///<summary>Calculates the intersection of two Planes.</summary>
    ///<param name="plane1">(Plane) The 1st Plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd Plane to intersect</param>
    ///<returns>(Line) a line with two 3d points identifying the starting/ending points of the intersection.</returns>
    static member PlanePlaneIntersection(plane1:Plane, plane2:Plane) : Line = 
        //plane1 = Scripting.Coerceplane(plane1)
        //plane2 = Scripting.Coerceplane(plane2)
        let rc, line = Intersect.Intersection.PlanePlane(plane1, plane2)
        if rc then line
        else RhinoScriptingException.Raise "Rhino.Scripting.PlanePlaneIntersection failed for %A; %A" plane1 plane2


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
    static member PlaneSphereIntersection( plane:Plane,
                                           spherePlane:Plane,
                                           sphereRadius:float) : int * Plane * float = 
        //plane = Scripting.Coerceplane(plane)
        //spherePlane = Scripting.Coerceplane(spherePlane)
        let sphere = Sphere(spherePlane, sphereRadius)
        let rc, circle = Intersect.Intersection.PlaneSphere(plane, sphere)
        if rc = Intersect.PlaneSphereIntersection.Point then
            0, circle.Plane, circle.Radius //was just circle.Center
        elif rc = Intersect.PlaneSphereIntersection.Circle then
            1, circle.Plane, circle.Radius
        else
            RhinoScriptingException.Raise "Rhino.Scripting.PlaneSphereIntersection failed for %A; %A, %A" plane spherePlane sphereRadius


    ///<summary>Transforms a Plane.</summary>
    ///<param name="plane">(Plane) Plane to transform</param>
    ///<param name="xForm">(Transform) Transformation to apply</param>
    ///<returns>(Plane) The resulting Plane.</returns>
    static member PlaneTransform(plane:Plane, xForm:Transform) : Plane = 
        //plane = Scripting.Coerceplane(plane)
        //xForm = Scripting.CoercexForm(xForm)
        let rc = Plane(plane)
        if rc.Transform(xForm) then rc
        else RhinoScriptingException.Raise "Rhino.Scripting.PlaneTransform failed for %A; %A" plane xForm


    ///<summary>Rotates a Plane.</summary>
    ///<param name="plane">(Plane) Plane to rotate</param>
    ///<param name="angleDegrees">(float) Rotation angle in degrees</param>
    ///<param name="axis">(Vector3d) Axis of rotation or list of three numbers</param>
    ///<returns>(Plane) rotated Plane.</returns>
    static member RotatePlane( plane:Plane,
                               angleDegrees:float,
                               axis:Vector3d) : Plane = 
        //plane = Scripting.Coerceplane(plane)
        //axis = Scripting.Coerce3dvector(axis)
        let angleradians = toRadians(angleDegrees)
        let rc = Plane(plane)
        if rc.Rotate(angleradians, axis) then rc
        else RhinoScriptingException.Raise "Rhino.Scripting.RotatePlane failed for %A; %A; %A" plane angleDegrees axis


    ///<summary>Returns Rhino's world XY Plane.</summary>
    ///<returns>(Plane) Rhino's world XY Plane.</returns>
    static member WorldXYPlane() : Plane = 
        Plane.WorldXY


    ///<summary>Returns Rhino's world YZ Plane.</summary>
    ///<returns>(Plane) Rhino's world YZ Plane.</returns>
    static member WorldYZPlane() : Plane = 
        Plane.WorldYZ


    ///<summary>Returns Rhino's world ZX Plane.</summary>
    ///<returns>(Plane) Rhino's world ZX Plane.</returns>
    static member WorldZXPlane() : Plane = 
        Plane.WorldZX





