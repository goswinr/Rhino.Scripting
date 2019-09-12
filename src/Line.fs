namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsLine =
  type RhinoScriptSyntax with
    
    ///<summary>Finds the point on an infinite line that is closest to a test point</summary>
    ///<param name="line">(Point3d * Point3d) List of 6 numbers or 2 Point3d.  Two 3-D points identifying the starting and ending points of the line.</param>
    ///<param name="testpoint">(Point3d) List of 3 numbers or Point3d.  The test point.</param>
    ///<returns>(Point3d) the point on the line that is closest to the test point , otherwise None</returns>
    static member LineClosestPoint(line:Point3d * Point3d, testpoint:Point3d) : Point3d =
        failNotImpl () // done in 2018


    ///<summary>Calculates the intersection of a line and a cylinder</summary>
    ///<param name="line">(Line) The line to intersect</param>
    ///<param name="cylinderPlane">(Plane) Base plane of the cylinder</param>
    ///<param name="cylinderHeight">(float) Height of the cylinder</param>
    ///<param name="cylinderRadius">(float) Radius of the cylinder</param>
    ///<returns>(Point3d seq) list of intersection points (0, 1, or 2 points)</returns>
    static member LineCylinderIntersection(line:Line, cylinderPlane:Plane, cylinderHeight:float, cylinderRadius:float) : Point3d seq =
        failNotImpl () // done in 2018


    ///<summary>Determines if the shortest distance from a line to a point or another
    ///  line is greater than a specified distance</summary>
    ///<param name="line">(Line) List of 6 numbers, 2 Point3d, or Line.</param>
    ///<param name="distance">(float) The distance</param>
    ///<param name="pointOrLine">(Point3d) The test point or the test line</param>
    ///<returns>(bool) True if the shortest distance from the line to the other project is
    ///  greater than distance, False otherwise</returns>
    static member LineIsFartherThan(line:Line, distance:float, pointOrLine:Point3d) : bool =
        failNotImpl () // done in 2018


    ///<summary>Calculates the intersection of two non-parallel lines. The lines are considered endless.
    ///  If the two lines do not actually intersect the closest point on each is returned</summary>
    ///<param name="lineA">(Line) LineA of 'lines to intersect' (FIXME 0)</param>
    ///<param name="lineB">(Line) LineB of 'lines to intersect' (FIXME 0)</param>
    ///<returns>(Point3d * Point3d) containing a point on the first line and a point on the second line</returns>
    static member LineLineIntersection(lineA:Line, lineB:Line) : Point3d * Point3d =
        failNotImpl () // done in 2018


    ///<summary>Finds the longest distance between a line as a finite chord, and a point
    ///  or another line</summary>
    ///<param name="line">(Line) List of 6 numbers, two Point3d, or Line.</param>
    ///<param name="pointOrLine">(Point3d) The test point or test line.</param>
    ///<returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object, then D >= Rhino.Distance(Q, P).</returns>
    static member LineMaxDistanceTo(line:Line, pointOrLine:Point3d) : float =
        failNotImpl () // done in 2018


    ///<summary>Finds the shortest distance between a line as a finite chord, and a point
    ///  or another line</summary>
    ///<param name="line">(Line) List of 6 numbers, two Point3d, or Line.</param>
    ///<param name="pointOrLine">(Point3d) The test point or test line.</param>
    ///<returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object, then D <= Rhino.Distance(Q, P).</returns>
    static member LineMinDistanceTo(line:Line, pointOrLine:Point3d) : float =
        failNotImpl () // done in 2018


    ///<summary>Returns a plane that contains the line. The origin of the plane is at the start of
    ///  the line. If possible, a plane parallel to the world XY, YZ, or ZX plane is returned</summary>
    ///<param name="line">(Line) List of 6 numbers, two Point3d, or Line.</param>
    ///<returns>(Plane) the plane</returns>
    static member LinePlane(line:Line) : Plane =
        failNotImpl () // done in 2018


    ///<summary>Calculates the intersection of a line and a plane.</summary>
    ///<param name="line">(Point3d * Point3d) Two 3D points identifying the starting and ending points of the line to intersect.</param>
    ///<param name="plane">(Plane) The plane to intersect.</param>
    ///<returns>(Point3d) The 3D point of intersection is successful.</returns>
    static member LinePlaneIntersection(line:Point3d * Point3d, plane:Plane) : Point3d =
        failNotImpl () // done in 2018


    ///<summary>Calculates the intersection of a line and a sphere</summary>
    ///<param name="line">(Line) The line</param>
    ///<param name="sphereCenter">(Point3d) The center point of the sphere</param>
    ///<param name="sphereRadius">(float) The radius of the sphere</param>
    ///<returns>(Point3d seq) list of intersection points , otherwise None</returns>
    static member LineSphereIntersection(line:Line, sphereCenter:Point3d, sphereRadius:float) : Point3d seq =
        failNotImpl () // done in 2018


    ///<summary>Transforms a line</summary>
    ///<param name="line">(Guid) The line to transform</param>
    ///<param name="xform">(Transform) The transformation to apply</param>
    ///<returns>(Guid) transformed line</returns>
    static member LineTransform(line:Guid, xform:Transform) : Guid =
        failNotImpl () // done in 2018


