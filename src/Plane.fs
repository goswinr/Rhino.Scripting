namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
[<AutoOpen>]
module ExtensionsPlane =
  type RhinoScriptSyntax with
    
    ///<summary>Returns the distance from a 3D point to a plane</summary>
    ///<param name="plane">(Plane) The plane</param>
    ///<param name="point">(Point3d) List of 3 numbers or Point3d</param>
    ///<returns>(float) The distance , otherwise None</returns>
    static member DistanceToPlane(plane:Plane, point:Point3d) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def DistanceToPlane(plane, point):
        '''Returns the distance from a 3D point to a plane
        Parameters:
          plane (plane): the plane
          point (point): List of 3 numbers or Point3d
        Returns:
          number: The distance if successful, otherwise None
        '''
        plane = rhutil.coerceplane(plane, True)
        point = rhutil.coerce3dpoint(point, True)
        return plane.DistanceTo(point)
    *)


    ///<summary>Evaluates a plane at a U,V parameter</summary>
    ///<param name="plane">(Plane) The plane to evaluate</param>
    ///<param name="parameter">(float * float) List of two numbers defining the U,V parameter to evaluate</param>
    ///<returns>(Point3d) Point3d on success</returns>
    static member EvaluatePlane(plane:Plane, parameter:float * float) : Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def EvaluatePlane(plane, parameter):
        '''Evaluates a plane at a U,V parameter
        Parameters:
          plane (plane): the plane to evaluate
          parameter ([number, number]): list of two numbers defining the U,V parameter to evaluate
        Returns:
          point: Point3d on success
        '''
        plane = rhutil.coerceplane(plane, True)
        return plane.PointAt(parameter[0], parameter[1])
    *)


    ///<summary>Calculates the intersection of three planes</summary>
    ///<param name="plane1">(Plane) The 1st plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd plane to intersect</param>
    ///<param name="plane3">(Plane) The 3rd plane to intersect</param>
    ///<returns>(Point3d) the intersection point between the 3 planes on success</returns>
    static member IntersectPlanes(plane1:Plane, plane2:Plane, plane3:Plane) : Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def IntersectPlanes(plane1, plane2, plane3):
        '''Calculates the intersection of three planes
        Parameters:
          plane1 (plane): the 1st plane to intersect
          plane2 (plane): the 2nd plane to intersect
          plane3 (plane): the 3rd plane to intersect
        Returns:
          point: the intersection point between the 3 planes on success
          None: on error
        '''
        plane1 = rhutil.coerceplane(plane1, True)
        plane2 = rhutil.coerceplane(plane2, True)
        plane3 = rhutil.coerceplane(plane3, True)
        rc, point = Rhino.Geometry.Intersect.Intersection.PlanePlanePlane(plane1, plane2, plane3)
        if rc: return point
    *)


    ///<summary>Moves the origin of a plane</summary>
    ///<param name="plane">(Plane) Plane or ConstructionPlane</param>
    ///<param name="origin">(Point3d) Point3d or list of three numbers</param>
    ///<returns>(Plane) moved plane</returns>
    static member MovePlane(plane:Plane, origin:Point3d) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def MovePlane(plane, origin):
        '''Moves the origin of a plane
        Parameters:
          plane (plane): Plane or ConstructionPlane
          origin (point): Point3d or list of three numbers
        Returns:
          plane: moved plane
        '''
        plane = rhutil.coerceplane(plane, True)
        origin = rhutil.coerce3dpoint(origin, True)
        rc = Rhino.Geometry.Plane(plane)
        rc.Origin = origin
        return rc
    *)


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
        failNotImpl () // genreation temp disabled !!
    (*
    def PlaneClosestPoint(plane, point, return_point=True):
        '''Returns the point on a plane that is closest to a test point.
        Parameters:
          plane (plane): The plane
          point (point): The 3-D point to test.
          return_point (bool, optional): If omitted or True, then the point on the plane
             that is closest to the test point is returned. If False, then the
             parameter of the point on the plane that is closest to the test
             point is returned.
        Returns:
          point: If return_point is omitted or True, then the 3-D point
          point: If return_point is False, then an array containing the U,V parameters of the point
          None: if not successful, or on error.
        '''
        plane = rhutil.coerceplane(plane, True)
        point = rhutil.coerce3dpoint(point, True)
        if return_point:
            return plane.ClosestPoint(point)
        else:
            rc, s, t = plane.ClosestParameter(point)
            if rc: return s, t
    *)


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
        failNotImpl () // genreation temp disabled !!
    (*
    def PlaneCurveIntersection(plane, curve, tolerance=None):
        '''Intersect an infinite plane and a curve object
        Parameters:
          plane (plane): The plane to intersect.
          curve (guid): The identifier of the curve object
          tolerance (number, optional): The intersection tolerance. If omitted, the document's absolute tolerance is used.
        Returns:
          [(number,Point3d,Point3d,Point3d,Point3d,number,number,number,number,number)]:a list of intersection information tuple if successful.  The list will contain one or more of the following tuple:
            Element Type        Description
            [0]       Number      The intersection event type, either Point (1) or Overlap (2).
            [1]       Point3d     If the event type is Point (1), then the intersection point on the curve.
                                If the event type is Overlap (2), then intersection start point on the curve.
            [2]       Point3d     If the event type is Point (1), then the intersection point on the curve.
                                If the event type is Overlap (2), then intersection end point on the curve.
            [3]       Point3d     If the event type is Point (1), then the intersection point on the plane.
                                If the event type is Overlap (2), then intersection start point on the plane.
            [4]       Point3d     If the event type is Point (1), then the intersection point on the plane.
                                If the event type is Overlap (2), then intersection end point on the plane.
            [5]       Number      If the event type is Point (1), then the curve parameter.
                                If the event type is Overlap (2), then the start value of the curve parameter range.
                                
            [6]       Number      If the event type is Point (1), then the curve parameter.
                                If the event type is Overlap (2),  then the end value of the curve parameter range.
            [7]       Number      If the event type is Point (1), then the U plane parameter.
                                If the event type is Overlap (2), then the U plane parameter for curve at (n, 5).
            [8]       Number      If the event type is Point (1), then the V plane parameter.
                                If the event type is Overlap (2), then the V plane parameter for curve at (n, 5).
            [9]       Number      If the event type is Point (1), then the U plane parameter.
                                If the event type is Overlap (2), then the U plane parameter for curve at (n, 6).
                                
            [10]      Number      If the event type is Point (1), then the V plane parameter.
                                If the event type is Overlap (2), then the V plane parameter for curve at (n, 6).
          None: on error
        '''
        plane = rhutil.coerceplane(plane, True)
        curve = rhutil.coercecurve(curve, -1, True)
        if tolerance is None: tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        intersections = Rhino.Geometry.Intersect.Intersection.CurvePlane(curve, plane, tolerance)
        if intersections:
            rc = []
            for intersection in intersections:
                a = 1
                if intersection.IsOverlap: a = 2
                b = intersection.PointA
                c = intersection.PointA2
                d = intersection.PointB
                e = intersection.PointB2
                f = intersection.ParameterA
                g = intersection.ParameterB
                h = intersection.OverlapA[0]
                i = intersection.OverlapA[1]
                j = intersection.OverlapB[0]
                k = intersection.OverlapB[1]
                rc.append( (a,b,c,d,e,f,g,h,i,j,k) )
            return rc
    *)


    ///<summary>Returns the equation of a plane as a tuple of four numbers. The standard
    ///  equation of a plane with a non-zero vector is Ax+By+Cz+D=0</summary>
    ///<param name="plane">(Plane) The plane to deconstruct</param>
    ///<returns>(float * float * float * float) containing four numbers that represent the coefficients of the equation  (A, B, C, D)</returns>
    static member PlaneEquation(plane:Plane) : float * float * float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def PlaneEquation(plane):
        '''Returns the equation of a plane as a tuple of four numbers. The standard
        equation of a plane with a non-zero vector is Ax+By+Cz+D=0
        Parameters:
          plane (plane): the plane to deconstruct
        Returns:
          tuple(number, number, number, number): containing four numbers that represent the coefficients of the equation  (A, B, C, D) if successful
          None: if not successful
        '''
        plane = rhutil.coerceplane(plane, True)
        rc = plane.GetPlaneEquation()
        return rc[0], rc[1], rc[2], rc[3]
    *)


    ///<summary>Returns a plane that was fit through an array of 3D points.</summary>
    ///<param name="points">(Point3d) An array of 3D points.</param>
    ///<returns>(Plane) The plane</returns>
    static member PlaneFitFromPoints(points:Point3d) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def PlaneFitFromPoints(points):
        '''Returns a plane that was fit through an array of 3D points.
        Parameters:
        points (point): An array of 3D points.
        Returns: 
          plane: The plane if successful
          None: if not successful
        '''
        points = rhutil.coerce3dpointlist(points, True)
        rc, plane = Rhino.Geometry.Plane.FitPlaneToPoints(points)
        if rc==Rhino.Geometry.PlaneFitResult.Success: return plane
    *)


    ///<summary>Construct a plane from a point, and two vectors in the plane.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the plane.</param>
    ///<param name="xAxis">(Vector3d) A non-zero 3D vector in the plane that determines the X axis
    ///  direction.</param>
    ///<param name="yAxis">(Vector3d) A non-zero 3D vector not parallel to xAxis that is used
    ///  to determine the Y axis direction. Note, yAxis does not
    ///  have to be perpendicular to xAxis.</param>
    ///<returns>(Plane) The plane .</returns>
    static member PlaneFromFrame(origin:Point3d, xAxis:Vector3d, yAxis:Vector3d) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def PlaneFromFrame(origin, x_axis, y_axis):
        '''Construct a plane from a point, and two vectors in the plane.
        Parameters:
          origin (point): A 3D point identifying the origin of the plane.
          x_axis (vector): A non-zero 3D vector in the plane that determines the X axis
                   direction.
          y_axis (vector): A non-zero 3D vector not parallel to x_axis that is used
                   to determine the Y axis direction. Note, y_axis does not
                   have to be perpendicular to x_axis.
        Returns:
          plane: The plane if successful.
        '''
        origin = rhutil.coerce3dpoint(origin, True)
        x_axis = rhutil.coerce3dvector(x_axis, True)
        y_axis = rhutil.coerce3dvector(y_axis, True)
        return Rhino.Geometry.Plane(origin, x_axis, y_axis)
    *)


    ///<summary>Creates a plane from an origin point and a normal direction vector.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the plane.</param>
    ///<param name="normal">(Vector3d) A 3D vector identifying the normal direction of the plane.</param>
    ///<param name="xaxis">(Vector3d) Optional, Default Value: <c>null</c>
    ///Optional vector defining the plane's x-axis</param>
    ///<returns>(Plane) The plane .</returns>
    static member PlaneFromNormal(origin:Point3d, normal:Vector3d, [<OPT;DEF(null)>]xaxis:Vector3d) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def PlaneFromNormal(origin, normal, xaxis=None):
        '''Creates a plane from an origin point and a normal direction vector.
        Parameters:
          origin (point): A 3D point identifying the origin of the plane.
          normal (vector): A 3D vector identifying the normal direction of the plane.
          xaxis (vector, optional): optional vector defining the plane's x-axis
        Returns:
          plane: The plane if successful.
        '''
        origin = rhutil.coerce3dpoint(origin, True)
        normal = rhutil.coerce3dvector(normal, True)
        rc = Rhino.Geometry.Plane(origin, normal)
        if xaxis:
            xaxis = rhutil.coerce3dvector(xaxis, True)
            xaxis = Rhino.Geometry.Vector3d(xaxis)#prevent original xaxis parameter from being unitized too
            xaxis.Unitize()
            yaxis = Rhino.Geometry.Vector3d.CrossProduct(rc.Normal, xaxis)
            rc = Rhino.Geometry.Plane(origin, xaxis, yaxis)
        return rc
    *)


    ///<summary>Creates a plane from three non-colinear points</summary>
    ///<param name="origin">(Point3d) Origin point of the plane</param>
    ///<param name="x">(Point3d) X of 'points on the plane's x and y axes' (FIXME 0)</param>
    ///<param name="y">(Point3d) Y of 'points on the plane's x and y axes' (FIXME 0)</param>
    ///<returns>(Plane) The plane , otherwise None</returns>
    static member PlaneFromPoints(origin:Point3d, x:Point3d, y:Point3d) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def PlaneFromPoints(origin, x, y):
        '''Creates a plane from three non-colinear points
        Parameters:
          origin (point): origin point of the plane
          x, y (point): points on the plane's x and y axes
        Returns:
          plane: The plane if successful, otherwise None
        '''
        origin = rhutil.coerce3dpoint(origin, True)
        x = rhutil.coerce3dpoint(x, True)
        y = rhutil.coerce3dpoint(y, True)
        plane = Rhino.Geometry.Plane(origin, x, y)
        if plane.IsValid: return plane
    *)


    ///<summary>Calculates the intersection of two planes</summary>
    ///<param name="plane1">(Plane) The 1st plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd plane to intersect</param>
    ///<returns>(Line) a line with two 3d points identifying the starting/ending points of the intersection</returns>
    static member PlanePlaneIntersection(plane1:Plane, plane2:Plane) : Line =
        failNotImpl () // genreation temp disabled !!
    (*
    def PlanePlaneIntersection(plane1, plane2):
        '''Calculates the intersection of two planes
        Parameters:
          plane1 (plane): the 1st plane to intersect
          plane2 (plane): the 2nd plane to intersect
        Returns:
          line:  a line with two 3d points identifying the starting/ending points of the intersection
          None: on error
        '''
        plane1 = rhutil.coerceplane(plane1, True)
        plane2 = rhutil.coerceplane(plane2, True)
        rc, line = Rhino.Geometry.Intersect.Intersection.PlanePlane(plane1, plane2)
        if rc: return line.From, line.To
    *)


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
        failNotImpl () // genreation temp disabled !!
    (*
    def PlaneSphereIntersection(plane, sphere_plane, sphere_radius):
        '''Calculates the intersection of a plane and a sphere
        Parameters:
          plane (plane): the plane to intersect
          sphere_plane (plane): equatorial plane of the sphere. origin of the plane is
            the center of the sphere
          sphere_radius (number): radius of the sphere
        Returns:
          list(number, point|plane, number): of intersection results
              Element    Type      Description
              [0]       number     The type of intersection, where 0 = point and 1 = circle.
              [1]   point or plane If a point intersection, the a Point3d identifying the 3-D intersection location.
                                   If a circle intersection, then the circle's plane. The origin of the plane will be the center point of the circle
              [2]       number     If a circle intersection, then the radius of the circle.
          None: on error
        '''
        plane = rhutil.coerceplane(plane, True)
        sphere_plane = rhutil.coerceplane(sphere_plane, True)
        sphere = Rhino.Geometry.Sphere(sphere_plane, sphere_radius)
        rc, circle = Rhino.Geometry.Intersect.Intersection.PlaneSphere(plane, sphere)
        if rc==Rhino.Geometry.Intersect.PlaneSphereIntersection.Point:
            return 0, circle.Center
        if rc==Rhino.Geometry.Intersect.PlaneSphereIntersection.Circle:
            return 1, circle.Plane, circle.Radius
    *)


    ///<summary>Transforms a plane</summary>
    ///<param name="plane">(Plane) Plane to transform</param>
    ///<param name="xform">(Transform) Transformation to apply</param>
    ///<returns>(Plane) the resulting plane</returns>
    static member PlaneTransform(plane:Plane, xform:Transform) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def PlaneTransform(plane, xform):
        '''Transforms a plane
        Parameters:
          plane (plane): Plane to transform
          xform (transform): Transformation to apply
        Returns:
          plane:the resulting plane if successful
          None: if not successful
        '''
        plane = rhutil.coerceplane(plane, True)
        xform = rhutil.coercexform(xform, True)
        rc = Rhino.Geometry.Plane(plane)
        if rc.Transform(xform): return rc
    *)


    ///<summary>Rotates a plane</summary>
    ///<param name="plane">(Plane) Plane to rotate</param>
    ///<param name="angleDegrees">(int) Rotation angle in degrees</param>
    ///<param name="axis">(Vector3d) Axis of rotation or list of three numbers</param>
    ///<returns>(Plane) rotated plane on success</returns>
    static member RotatePlane(plane:Plane, angleDegrees:int, axis:Vector3d) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def RotatePlane(plane, angle_degrees, axis):
        '''Rotates a plane
        Parameters:
          plane (plane): Plane to rotate
          angle_degrees (number): rotation angle in degrees
          axis (vector): Axis of rotation or list of three numbers
        Returns:
          plane: rotated plane on success
        '''
        plane = rhutil.coerceplane(plane, True)
        axis = rhutil.coerce3dvector(axis, True)
        angle_radians = math.radians(angle_degrees)
        rc = Rhino.Geometry.Plane(plane)
        if rc.Rotate(angle_radians, axis): return rc
    *)


    ///<summary>Returns Rhino's world XY plane</summary>
    ///<returns>(Plane) Rhino's world XY plane</returns>
    static member WorldXYPlane() : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def WorldXYPlane():
        '''Returns Rhino's world XY plane
        Returns:
          plane: Rhino's world XY plane
        '''
        return Rhino.Geometry.Plane.WorldXY
    *)


    ///<summary>Returns Rhino's world YZ plane</summary>
    ///<returns>(Plane) Rhino's world YZ plane</returns>
    static member WorldYZPlane() : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def WorldYZPlane():
        '''Returns Rhino's world YZ plane
        Returns:
          plane: Rhino's world YZ plane
        '''
        return Rhino.Geometry.Plane.WorldYZ
    *)


    ///<summary>Returns Rhino's world ZX plane</summary>
    ///<returns>(Plane) Rhino's world ZX plane</returns>
    static member WorldZXPlane() : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def WorldZXPlane():
        '''Returns Rhino's world ZX plane
        Returns:
          plane: Rhino's world ZX plane
        '''
        return Rhino.Geometry.Plane.WorldZX
    *)


