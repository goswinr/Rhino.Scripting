namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsLine =
  [<EXT>] 
  type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Finds the point on an infinite line that is closest to a test point</summary>
    ///<param name="line">(Point3d * Point3d) List of 6 numbers or 2 Point3d.  Two 3-D points identifying the starting and ending points of the line.</param>
    ///<param name="testpoint">(Point3d) List of 3 numbers or Point3d.  The test point.</param>
    ///<returns>(Point3d) the point on the line that is closest to the test point , otherwise None</returns>
    static member LineClosestPoint(line:Point3d * Point3d, testpoint:Point3d) : Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def LineClosestPoint(line, testpoint):
        '''Finds the point on an infinite line that is closest to a test point
        Parameters:
          line ([point, point]): List of 6 numbers or 2 Point3d.  Two 3-D points identifying the starting and ending points of the line.
          testpoint (point): List of 3 numbers or Point3d.  The test point.
        Returns:
          point: the point on the line that is closest to the test point if successful, otherwise None
        '''
        line = rhutil.coerceline(line, True)
        testpoint = rhutil.coerce3dpoint(testpoint, True)
        return line.ClosestPoint(testpoint, False)
    *)


    [<EXT>]
    ///<summary>Calculates the intersection of a line and a cylinder</summary>
    ///<param name="line">(Line) The line to intersect</param>
    ///<param name="cylinderPlane">(Plane) Base plane of the cylinder</param>
    ///<param name="cylinderHeight">(float) Height of the cylinder</param>
    ///<param name="cylinderRadius">(float) Radius of the cylinder</param>
    ///<returns>(Point3d seq) list of intersection points (0, 1, or 2 points)</returns>
    static member LineCylinderIntersection(line:Line, cylinderPlane:Plane, cylinderHeight:float, cylinderRadius:float) : Point3d seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def LineCylinderIntersection(line, cylinder_plane, cylinder_height, cylinder_radius):
        '''Calculates the intersection of a line and a cylinder
        Parameters:
          line (guid|line): the line to intersect
          cylinder_plane (plane): base plane of the cylinder
          cylinder_height (number): height of the cylinder
          cylinder_radius (number): radius of the cylinder
        Returns:
          list(point, ...): list of intersection points (0, 1, or 2 points)
        '''
        line = rhutil.coerceline(line, True)
        cylinder_plane = rhutil.coerceplane(cylinder_plane, True)
        circle = Rhino.Geometry.Circle( cylinder_plane, cylinder_radius )
        if not circle.IsValid: raise ValueError("unable to create valid circle with given plane and radius")
        cyl = Rhino.Geometry.Cylinder( circle, cylinder_height )
        if not cyl.IsValid: raise ValueError("unable to create valid cylinder with given circle and height")
        rc, pt1, pt2 = Rhino.Geometry.Intersect.Intersection.LineCylinder(line, cyl)
        if rc==Rhino.Geometry.Intersect.LineCylinderIntersection.None:
            return []
        if rc==Rhino.Geometry.Intersect.LineCylinderIntersection.Single:
            return [pt1]
        return [pt1, pt2]
    *)


    [<EXT>]
    ///<summary>Determines if the shortest distance from a line to a point or another
    ///  line is greater than a specified distance</summary>
    ///<param name="line">(Line) List of 6 numbers, 2 Point3d, or Line.</param>
    ///<param name="distance">(float) The distance</param>
    ///<param name="pointOrLine">(Point3d) The test point or the test line</param>
    ///<returns>(bool) True if the shortest distance from the line to the other project is
    ///  greater than distance, False otherwise</returns>
    static member LineIsFartherThan(line:Line, distance:float, pointOrLine:Point3d) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def LineIsFartherThan(line, distance, point_or_line):
        '''Determines if the shortest distance from a line to a point or another
        line is greater than a specified distance
        Parameters:
          line (line | [point, point]): List of 6 numbers, 2 Point3d, or Line.
          distance (number): the distance
          point_or_line (point|line) the test point or the test line
        Returns:
          bool: True if the shortest distance from the line to the other project is
                greater than distance, False otherwise
          None: on error
        '''
        line = rhutil.coerceline(line, True)
        test = rhutil.coerceline(point_or_line)
        if not test: test = rhutil.coerce3dpoint(point_or_line, True)
        minDist = line.MinimumDistanceTo(test)
        return minDist>distance
    *)


    [<EXT>]
    ///<summary>Calculates the intersection of two non-parallel lines. The lines are considered endless.
    ///  If the two lines do not actually intersect the closest point on each is returned</summary>
    ///<param name="lineA">(Line) LineA of 'lines to intersect' (FIXME 0)</param>
    ///<param name="lineB">(Line) LineB of 'lines to intersect' (FIXME 0)</param>
    ///<returns>(Point3d * Point3d) containing a point on the first line and a point on the second line</returns>
    static member LineLineIntersection(lineA:Line, lineB:Line) : Point3d * Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def LineLineIntersection(lineA, lineB):
        '''Calculates the intersection of two non-parallel lines. The lines are considered endless.
        If the two lines do not actually intersect the closest point on each is returned
        Parameters:
          lineA, lineB (line): lines to intersect
        Returns:
          tuple(point, point): containing a point on the first line and a point on the second line if successful
          None: on error
        '''
        lineA = rhutil.coerceline(lineA, True)
        lineB = rhutil.coerceline(lineB, True)
        rc, a, b = Rhino.Geometry.Intersect.Intersection.LineLine(lineA, lineB)
        if not rc: return None
        return lineA.PointAt(a), lineB.PointAt(b)
    *)


    [<EXT>]
    ///<summary>Finds the longest distance between a line as a finite chord, and a point
    ///  or another line</summary>
    ///<param name="line">(Line) List of 6 numbers, two Point3d, or Line.</param>
    ///<param name="pointOrLine">(Point3d) The test point or test line.</param>
    ///<returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object, then D >= Rhino.Distance(Q, P).</returns>
    static member LineMaxDistanceTo(line:Line, pointOrLine:Point3d) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def LineMaxDistanceTo(line, point_or_line):
        '''Finds the longest distance between a line as a finite chord, and a point
        or another line
        Parameters:
          line (line | [point, point]): List of 6 numbers, two Point3d, or Line.
          point_or_line (point|line): the test point or test line.
        Returns:
          number: A distance (D) such that if Q is any point on the line and P is any point on the other object, then D >= Rhino.Distance(Q, P).
          None: on error
        '''
        line = rhutil.coerceline(line, True)
        test = rhutil.coerceline(point_or_line)
        if test is None: test = rhutil.coerce3dpoint(point_or_line, True)
        return line.MaximumDistanceTo(test)
    *)


    [<EXT>]
    ///<summary>Finds the shortest distance between a line as a finite chord, and a point
    ///  or another line</summary>
    ///<param name="line">(Line) List of 6 numbers, two Point3d, or Line.</param>
    ///<param name="pointOrLine">(Point3d) The test point or test line.</param>
    ///<returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object, then D <= Rhino.Distance(Q, P).</returns>
    static member LineMinDistanceTo(line:Line, pointOrLine:Point3d) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def LineMinDistanceTo(line, point_or_line):
        '''Finds the shortest distance between a line as a finite chord, and a point
        or another line
        Parameters:
          line (line | [point, point]): List of 6 numbers, two Point3d, or Line.
          point_or_line (point|line): the test point or test line.
        Returns:
          number: A distance (D) such that if Q is any point on the line and P is any point on the other object, then D <= Rhino.Distance(Q, P).
          None: on error
        '''
        line = rhutil.coerceline(line, True)
        test = rhutil.coerceline(point_or_line)
        if test is None: test = rhutil.coerce3dpoint(point_or_line, True)
        return line.MinimumDistanceTo(test)
    *)


    [<EXT>]
    ///<summary>Returns a plane that contains the line. The origin of the plane is at the start of
    ///  the line. If possible, a plane parallel to the world XY, YZ, or ZX plane is returned</summary>
    ///<param name="line">(Line) List of 6 numbers, two Point3d, or Line.</param>
    ///<returns>(Plane) the plane</returns>
    static member LinePlane(line:Line) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def LinePlane(line):
        '''Returns a plane that contains the line. The origin of the plane is at the start of
        the line. If possible, a plane parallel to the world XY, YZ, or ZX plane is returned
        Parameters:
          line (line | [point, point]):  List of 6 numbers, two Point3d, or Line.
        Returns:
          plane: the plane if successful
          None: if not successful
        '''
        line = rhutil.coerceline(line, True)
        rc, plane = line.TryGetPlane()
        if not rc: return scriptcontext.errorhandler()
        return plane
    *)


    [<EXT>]
    ///<summary>Calculates the intersection of a line and a plane.</summary>
    ///<param name="line">(Point3d * Point3d) Two 3D points identifying the starting and ending points of the line to intersect.</param>
    ///<param name="plane">(Plane) The plane to intersect.</param>
    ///<returns>(Point3d) The 3D point of intersection is successful.</returns>
    static member LinePlaneIntersection(line:Point3d * Point3d, plane:Plane) : Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def LinePlaneIntersection(line, plane):
        '''Calculates the intersection of a line and a plane.
        Parameters:
          line ([point, point]): Two 3D points identifying the starting and ending points of the line to intersect.
          plane (plane): The plane to intersect.
        Returns:
          point: The 3D point of intersection is successful.
          None: if not successful, or on error.
        '''
        plane = rhutil.coerceplane(plane, True)
        line_points = rhutil.coerce3dpointlist(line, True)
        line = Rhino.Geometry.Line(line_points[0], line_points[1])
        rc, t = Rhino.Geometry.Intersect.Intersection.LinePlane(line, plane) 
        if  not rc: return scriptcontext.errorhandler()
        return line.PointAt(t)
    *)


    [<EXT>]
    ///<summary>Calculates the intersection of a line and a sphere</summary>
    ///<param name="line">(Line) The line</param>
    ///<param name="sphereCenter">(Point3d) The center point of the sphere</param>
    ///<param name="sphereRadius">(float) The radius of the sphere</param>
    ///<returns>(Point3d seq) list of intersection points , otherwise None</returns>
    static member LineSphereIntersection(line:Line, sphereCenter:Point3d, sphereRadius:float) : Point3d seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def LineSphereIntersection(line, sphere_center, sphere_radius):
        '''Calculates the intersection of a line and a sphere
        Parameters:
          line (line | [point, point]): the line
          sphere_center (point): the center point of the sphere
          sphere_radius (number): the radius of the sphere
        Returns:
          list(point, ...): list of intersection points if successful, otherwise None
        '''
        line = rhutil.coerceline(line, True)
        sphere_center = rhutil.coerce3dpoint(sphere_center, True)
        sphere = Rhino.Geometry.Sphere(sphere_center, sphere_radius)
        rc, pt1, pt2 = Rhino.Geometry.Intersect.Intersection.LineSphere(line, sphere)
        if rc==Rhino.Geometry.Intersect.LineSphereIntersection.None: return []
        if rc==Rhino.Geometry.Intersect.LineSphereIntersection.Single: return [pt1]
        return [pt1, pt2]
    *)


    [<EXT>]
    ///<summary>Transforms a line</summary>
    ///<param name="line">(Guid) The line to transform</param>
    ///<param name="xform">(Transform) The transformation to apply</param>
    ///<returns>(Guid) transformed line</returns>
    static member LineTransform(line:Guid, xform:Transform) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def LineTransform(line, xform):
        '''Transforms a line
        Parameters:
          line (guid): the line to transform
          xform (transform): the transformation to apply
        Returns:
          guid: transformed line
        '''
        line = rhutil.coerceline(line, True)
        xform = rhutil.coercexform(xform, True)
        success = line.Transform(xform)
        if not success: raise Execption("unable to transform line")
        return line
    *)


