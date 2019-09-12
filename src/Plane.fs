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
        let plane = Coerce.coerceplane(plane, true)
        let point = Coerce.coerce3dpoint(point, true)
        plane.DistanceTo(point)
    (*
    def DistanceToPlane(plane, point):
        """Returns the distance from a 3D point to a plane
        Parameters:
          plane (plane): the plane
          point (point): List of 3 numbers or Point3d
        Returns:
          number: The distance if successful, otherwise None
        Example:
          import rhinoscriptsyntax as rs
          point = rs.GetPoint("Point to test")
          if point:
              plane = rs.ViewCPlane()
              if plane:
                  distance = rs.DistanceToPlane(plane, point)
                  if distance is not None:
                      print "Distance to plane: ", distance
        See Also:
          Distance
          PlaneClosestPoint
        """
        plane = rhutil.coerceplane(plane, True)
        point = rhutil.coerce3dpoint(point, True)
        return plane.DistanceTo(point)
    *)


    ///<summary>Evaluates a plane at a U,V parameter</summary>
    ///<param name="plane">(Plane) The plane to evaluate</param>
    ///<param name="parameter">(float * float) List of two numbers defining the U,V parameter to evaluate</param>
    ///<returns>(Point3d) Point3d on success</returns>
    static member EvaluatePlane(plane:Plane, parameter:float * float) : Point3d =
        let plane = Coerce.coerceplane(plane, true)
        plane.PointAt(parameter.[0], parameter.[1])
    (*
    def EvaluatePlane(plane, parameter):
        """Evaluates a plane at a U,V parameter
        Parameters:
          plane (plane): the plane to evaluate
          parameter ([number, number]): list of two numbers defining the U,V parameter to evaluate
        Returns:
          point: Point3d on success
        Example:
          import rhinoscriptsyntax as rs
          view = rs.CurrentView()
          plane = rs.ViewCPlane(view)
          point = rs.EvaluatePlane(plane, (5,5))
          rs.AddPoint( point )
        See Also:
          PlaneClosestPoint
        """
        plane = rhutil.coerceplane(plane, True)
        return plane.PointAt(parameter[0], parameter[1])
    *)


    ///<summary>Calculates the intersection of three planes</summary>
    ///<param name="plane1">(Plane) The 1st plane to intersect</param>
    ///<param name="plane2">(Plane) The 2nd plane to intersect</param>
    ///<param name="plane3">(Plane) The 3rd plane to intersect</param>
    ///<returns>(Point3d) the intersection point between the 3 planes on success</returns>
    static member IntersectPlanes(plane1:Plane, plane2:Plane, plane3:Plane) : Point3d =
        let plane1 = Coerce.coerceplane(plane1, true)
        let plane2 = Coerce.coerceplane(plane2, true)
        let plane3 = Coerce.coerceplane(plane3, true)
        let rc, point = Intersect.Intersection.PlanePlanePlane(plane1, plane2, plane3)
        if rc then point
    (*
    def IntersectPlanes(plane1, plane2, plane3):
        """Calculates the intersection of three planes
        Parameters:
          plane1 (plane): the 1st plane to intersect
          plane2 (plane): the 2nd plane to intersect
          plane3 (plane): the 3rd plane to intersect
        Returns:
          point: the intersection point between the 3 planes on success
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          plane1 = rs.WorldXYPlane()
          plane2 = rs.WorldYZPlane()
          plane3 = rs.WorldZXPlane()
          point = rs.IntersectPlanes(plane1, plane2, plane3)
          if point: rs.AddPoint(point)
        See Also:
          LineLineIntersection
          LinePlaneIntersection
          PlanePlaneIntersection
        """
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
        let plane = Coerce.coerceplane(plane, true)
        let origin = Coerce.coerce3dpoint(origin, true)
        let rc = Plane(plane)
        let rc.Origin = origin
        rc
    (*
    def MovePlane(plane, origin):
        """Moves the origin of a plane
        Parameters:
          plane (plane): Plane or ConstructionPlane
          origin (point): Point3d or list of three numbers
        Returns:
          plane: moved plane
        Example:
          import rhinoscriptsyntax as rs
          origin = rs.GetPoint("CPlane origin")
          if origin:
              plane = rs.ViewCPlane()
              plane = rs.MovePlane(plane,origin)
              rs.ViewCplane(plane)
        See Also:
          PlaneFromFrame
          PlaneFromNormal
          RotatePlane
        """
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
        let plane = Coerce.coerceplane(plane, true)
        let point = Coerce.coerce3dpoint(point, true)
        if Point then
            plane.ClosestPoint(point)
        else
            let rc, s, t = plane.ClosestParameter(point)
            if rc then s, t
    (*
    def PlaneClosestPoint(plane, point, return_point=True):
        """Returns the point on a plane that is closest to a test point.
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
        Example:
          import rhinoscriptsyntax as rs
          point = rs.GetPoint("Point to test")
          if point:
              plane = rs.ViewCPlane()
              if plane:
                  print rs.PlaneClosestPoint(plane, point)
        See Also:
          DistanceToPlane
          EvaluatePlane
        """
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
        let plane = Coerce.coerceplane(plane, true)
        let curve = Coerce.coercecurve(curve, -1, true)
        if tolerance = null then tolerance <- Doc.ModelAbsoluteTolerance
        let intersections = Intersect.Intersection.CurvePlane(curve, plane, tolerance)
        if intersections then
            let rc = ResizeArray()
            for intersection in intersections do
                let a = 1
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
    (*
    def PlaneCurveIntersection(plane, curve, tolerance=None):
        """Intersect an infinite plane and a curve object
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
        Example:
          import rhinoscriptsyntax as rs
          curve = rs.GetObject("Select curve", rs.filter.curve)
          if curve:
              plane = rs.WorldXYPlane()
              intersections = rs.PlaneCurveIntersection(plane, curve)
              if intersections:
                  for intersection in intersections:
                      rs.AddPoint(intersection[1])
        See Also:
          IntersectPlanes
          PlanePlaneIntersection
          PlaneSphereIntersection
        """
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
        let plane = Coerce.coerceplane(plane, true)
        let rc = plane.GetPlaneEquation()
        rc.[0], rc.[1], rc.[2], rc.[3]
    (*
    def PlaneEquation(plane):
        """Returns the equation of a plane as a tuple of four numbers. The standard
        equation of a plane with a non-zero vector is Ax+By+Cz+D=0
        Parameters:
          plane (plane): the plane to deconstruct
        Returns:
          tuple(number, number, number, number): containing four numbers that represent the coefficients of the equation  (A, B, C, D) if successful
          None: if not successful
        Example:
          import rhinoscriptsyntax as rs
          plane = rs.ViewCPlane()
          equation = rs.PlaneEquation(plane)
          print "A =", equation[0]
          print "B =", equation[1]
          print "C =", equation[2]
          print "D =", equation[3]
        See Also:
          PlaneFromFrame
          PlaneFromNormal
          PlaneFromPoints
        """
        plane = rhutil.coerceplane(plane, True)
        rc = plane.GetPlaneEquation()
        return rc[0], rc[1], rc[2], rc[3]
    *)


    ///<summary>Returns a plane that was fit through an array of 3D points.</summary>
    ///<param name="points">(Point3d) An array of 3D points.</param>
    ///<returns>(Plane) The plane</returns>
    static member PlaneFitFromPoints(points:Point3d) : Plane =
        let points = Coerce.coerce3dpointlist(points, true)
        let rc, plane = Plane.FitPlaneToPoints(points)
        if rc=PlaneFitResult.Success then plane
    (*
    def PlaneFitFromPoints(points):
        """Returns a plane that was fit through an array of 3D points.
        Parameters:
        points (point): An array of 3D points.
        Returns: 
          plane: The plane if successful
          None: if not successful
        Example:
          import rhinoscriptsyntax as rs
          points = rs.GetPoints()
          if points:
              plane = rs.PlaneFitFromPoints(points)
              if plane:
                  magX = plane.XAxis.Length
                  magY = plane.YAxis.Length
                  rs.AddPlaneSurface( plane, magX, magY )
        See Also:
          PlaneFromFrame
          PlaneFromNormal
          PlaneFromPoints
        """
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
        let origin = Coerce.coerce3dpoint(origin, true)
        let x_axis = Coerce.coerce3dvector(x_axis, true)
        let yAxis = Coerce.coerce3dvector(yAxis, true)
        Plane(origin, x_axis, yAxis)
    (*
    def PlaneFromFrame(origin, x_axis, y_axis):
        """Construct a plane from a point, and two vectors in the plane.
        Parameters:
          origin (point): A 3D point identifying the origin of the plane.
          x_axis (vector): A non-zero 3D vector in the plane that determines the X axis
                   direction.
          y_axis (vector): A non-zero 3D vector not parallel to x_axis that is used
                   to determine the Y axis direction. Note, y_axis does not
                   have to be perpendicular to x_axis.
        Returns:
          plane: The plane if successful.
        Example:
          import rhinoscriptsyntax as rs
          origin = rs.GetPoint("CPlane origin")
          if origin:
              xaxis = (1,0,0)
              yaxis = (0,0,1)
              plane = rs.PlaneFromFrame( origin, xaxis, yaxis )
              rs.ViewCPlane(None, plane)
        See Also:
          MovePlane
          PlaneFromNormal
          PlaneFromPoints
          RotatePlane
        """
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
        let origin = Coerce.coerce3dpoint(origin, true)
        let normal = Coerce.coerce3dvector(normal, true)
        let rc = Plane(origin, normal)
        if xaxis then
            let xaxis = Coerce.coerce3dvector(xaxis, true)
            xaxis <- Vector3d(xaxis)//prevent original xaxis parameter from being unitized too
            xaxis.Unitize() |> ignore
            let yaxis = Vector3d.CrossProduct(rc.Normal, xaxis)
            rc <- Plane(origin, xaxis, yaxis)
        rc
    (*
    def PlaneFromNormal(origin, normal, xaxis=None):
        """Creates a plane from an origin point and a normal direction vector.
        Parameters:
          origin (point): A 3D point identifying the origin of the plane.
          normal (vector): A 3D vector identifying the normal direction of the plane.
          xaxis (vector, optional): optional vector defining the plane's x-axis
        Returns:
          plane: The plane if successful.
        Example:
          import rhinoscriptsyntax as rs
          origin = rs.GetPoint("CPlane origin")
          if origin:
              direction = rs.GetPoint("CPlane direction")
              if direction:
                  normal = direction - origin
                  normal = rs.VectorUnitize(normal)
                  rs.ViewCPlane( None, rs.PlaneFromNormal(origin, normal) )
        See Also:
          MovePlane
          PlaneFromFrame
          PlaneFromPoints
          RotatePlane
        """
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
        let origin = Coerce.coerce3dpoint(origin, true)
        let x = Coerce.coerce3dpoint(x, true)
        let y = Coerce.coerce3dpoint(y, true)
        let plane = Plane(origin, x, y)
        if plane.IsValid then plane
    (*
    def PlaneFromPoints(origin, x, y):
        """Creates a plane from three non-colinear points
        Parameters:
          origin (point): origin point of the plane
          x, y (point): points on the plane's x and y axes
        Returns:
          plane: The plane if successful, otherwise None
        Example:
          import rhinoscriptsyntax as rs
          corners = rs.GetRectangle()
          if corners:
              rs.ViewCPlane( rs.PlaneFromPoints(corners[0], corners[1], corners[3]))
        See Also:
          PlaneFromFrame
          PlaneFromNormal
        """
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
        let plane1 = Coerce.coerceplane(plane1, true)
        let plane2 = Coerce.coerceplane(plane2, true)
        let rc, line = Intersect.Intersection.PlanePlane(plane1, plane2)
        if rc then line.From, line.To
    (*
    def PlanePlaneIntersection(plane1, plane2):
        """Calculates the intersection of two planes
        Parameters:
          plane1 (plane): the 1st plane to intersect
          plane2 (plane): the 2nd plane to intersect
        Returns:
          line:  a line with two 3d points identifying the starting/ending points of the intersection
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          plane1 = rs.WorldXYPlane()
          plane2 = rs.WorldYZPlane()
          line = rs.PlanePlaneIntersection(plane1, plane2)
          if line: rs.AddLine(line[0], line[1])
        See Also:
          IntersectPlanes
          LineLineIntersection
          LinePlaneIntersection
        """
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
        let plane = Coerce.coerceplane(plane, true)
        let sphere_plane = Coerce.coerceplane(sphere_plane, true)
        let sphere = Sphere(sphere_plane, sphereRadius)
        let rc, circle = Intersect.Intersection.PlaneSphere(plane, sphere)
        if rc=Intersect.PlaneSphereIntersection.Point then
            0, circle.Center
        if rc=Intersect.PlaneSphereIntersection.Circle then
            1, circle.Plane, circle.Radius
    (*
    def PlaneSphereIntersection(plane, sphere_plane, sphere_radius):
        """Calculates the intersection of a plane and a sphere
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
        Example:
          import rhinoscriptsyntax as rs
          plane = rs.WorldXYPlane()
          radius = 10
          results = rs.PlaneSphereIntersection(plane, plane, radius)
          if results:
              if results[0]==0:
                  rs.AddPoint(results[1])
              else:
                  rs.AddCircle(results[1], results[2])
        See Also:
          IntersectPlanes
          LinePlaneIntersection
          PlanePlaneIntersection
        """
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
        let plane = Coerce.coerceplane(plane, true)
        let xform = Coerce.coercexform(xform, true)
        let rc = Plane(plane)
        if rc.Transform(xform) then rc
    (*
    def PlaneTransform(plane, xform):
        """Transforms a plane
        Parameters:
          plane (plane): Plane to transform
          xform (transform): Transformation to apply
        Returns:
          plane:the resulting plane if successful
          None: if not successful
        Example:
          import rhinoscriptsyntax as rs
          plane = rs.ViewCPlane()
          xform = rs.XformRotation(45.0, plane.Zaxis, plane.Origin)
          plane = rs.PlaneTransform(plane, xform)
          rs.ViewCPlane(None, plane)
        See Also:
          PlaneFromFrame
          PlaneFromNormal
          PlaneFromPoints
        """
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
        let plane = Coerce.coerceplane(plane, true)
        let axis = Coerce.coerce3dvector(axis, true)
        let angle_radians = toRadians(angleDegrees)
        let rc = Plane(plane)
        if rc.Rotate(angle_radians, axis) then rc
    (*
    def RotatePlane(plane, angle_degrees, axis):
        """Rotates a plane
        Parameters:
          plane (plane): Plane to rotate
          angle_degrees (number): rotation angle in degrees
          axis (vector): Axis of rotation or list of three numbers
        Returns:
          plane: rotated plane on success
        Example:
          import rhinoscriptsyntax as rs
          plane = rs.ViewCPlane()
          rotated = rs.RotatePlane(plane, 45.0, plane.XAxis)
          rs.ViewCPlane( None, rotated )
        See Also:
          MovePlane
          PlaneFromFrame
          PlaneFromNormal
        """
        plane = rhutil.coerceplane(plane, True)
        axis = rhutil.coerce3dvector(axis, True)
        angle_radians = math.radians(angle_degrees)
        rc = Rhino.Geometry.Plane(plane)
        if rc.Rotate(angle_radians, axis): return rc
    *)


    ///<summary>Returns Rhino's world XY plane</summary>
    ///<returns>(Plane) Rhino's world XY plane</returns>
    static member WorldXYPlane() : Plane =
        Plane.WorldXY
    (*
    def WorldXYPlane():
        """Returns Rhino's world XY plane
        Returns:
          plane: Rhino's world XY plane
        Example:
          import rhinoscriptsyntax as rs
          view = rs.CurrentView()
          rs.ViewCPlane( view, rs.WorldXYPlane() )
        See Also:
          WorldYZPlane
          WorldZXPlane
        """
        return Rhino.Geometry.Plane.WorldXY
    *)


    ///<summary>Returns Rhino's world YZ plane</summary>
    ///<returns>(Plane) Rhino's world YZ plane</returns>
    static member WorldYZPlane() : Plane =
        Plane.WorldYZ
    (*
    def WorldYZPlane():
        """Returns Rhino's world YZ plane
        Returns:
          plane: Rhino's world YZ plane
        Example:
          import rhinoscriptsyntax as rs
          view = rs.CurrentView()
          rs.ViewCPlane( view, rs.WorldYZPlane() )
        See Also:
          WorldXYPlane
          WorldZXPlane
        """
        return Rhino.Geometry.Plane.WorldYZ
    *)


    ///<summary>Returns Rhino's world ZX plane</summary>
    ///<returns>(Plane) Rhino's world ZX plane</returns>
    static member WorldZXPlane() : Plane =
        Plane.WorldZX
    (*
    def WorldZXPlane():
        """Returns Rhino's world ZX plane
        Returns:
          plane: Rhino's world ZX plane
        Example:
          import rhinoscriptsyntax as rs
          view = rs.CurrentView()
          rs.ViewCPlane( view, rs.WorldZXPlane() )
        See Also:
          WorldXYPlane
          WorldYZPlane
        """
        return Rhino.Geometry.Plane.WorldZX
    *)


