namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsPointvector =
  type RhinoScriptSyntax with
    
    ///<summary>Compares two vectors to see if they are parallel</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to compare' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to compare' (FIXME 0)</param>
    ///<returns>(float) the value represents
    ///  -1 = the vectors are anti-parallel
    ///    0 = the vectors are not parallel
    ///    1 = the vectors are parallel</returns>
    static member IsVectorParallelTo(vector1:Vector3d, vector2:Vector3d) : float =
        failNotImpl () 


    ///<summary>Compares two vectors to see if they are perpendicular</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to compare' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to compare' (FIXME 0)</param>
    ///<returns>(bool) True if vectors are perpendicular, otherwise False</returns>
    static member IsVectorPerpendicularTo(vector1:Vector3d, vector2:Vector3d) : bool =
        failNotImpl () 


    ///<summary>Verifies that a vector is very short. The X,Y,Z elements are <= 1.0e-12</summary>
    ///<param name="vector">(Vector3d) The vector to check</param>
    ///<returns>(bool) True if the vector is tiny, otherwise False</returns>
    static member IsVectorTiny(vector:Vector3d) : bool =
        failNotImpl () 


    ///<summary>Verifies that a vector is zero, or tiny. The X,Y,Z elements are equal to 0.0</summary>
    ///<param name="vector">(Vector3d) The vector to check</param>
    ///<returns>(bool) True if the vector is zero, otherwise False</returns>
    static member IsVectorZero(vector:Vector3d) : bool =
        failNotImpl () 


    ///<summary>Adds a 3D point or a 3D vector to a 3D point</summary>
    ///<param name="point1">(Point3d) Point1 of 'the points to add' (FIXME 0)</param>
    ///<param name="point2">(Point3d) Point2 of 'the points to add' (FIXME 0)</param>
    ///<returns>(Point3d) the resulting 3D point</returns>
    static member PointAdd(point1:Point3d, point2:Point3d) : Point3d =
        failNotImpl () 


    ///<summary>Finds the point in a list of 3D points that is closest to a test point</summary>
    ///<param name="points">(Point3d seq) List of points</param>
    ///<param name="testPoint">(Point3d) The point to compare against</param>
    ///<returns>(float) index of the element in the point list that is closest to the test point</returns>
    static member PointArrayClosestPoint(points:Point3d seq, testPoint:Point3d) : float =
        failNotImpl () 


    ///<summary>Transforms a list of 3D points</summary>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<param name="xform">(Transform) Transformation to apply</param>
    ///<returns>(Point3d seq) transformed points on success</returns>
    static member PointArrayTransform(points:Point3d seq, xform:Transform) : Point3d seq =
        failNotImpl () 


    ///<summary>Finds the object that is closest to a test point</summary>
    ///<param name="point">(Point3d) Point to test</param>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more objects</param>
    ///<returns>(Guid * Point3d) closest [0] object_id and [1] point on object on success</returns>
    static member PointClosestObject(point:Point3d, objectIds:Guid seq) : Guid * Point3d =
        failNotImpl () 


    ///<summary>Compares two 3D points</summary>
    ///<param name="point1">(Point3d) Point1 of 'the points to compare' (FIXME 0)</param>
    ///<param name="point2">(Point3d) Point2 of 'the points to compare' (FIXME 0)</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///Tolerance to use for comparison. If omitted,
    ///  Rhino's internal zero tolerance is used</param>
    ///<returns>(bool) True or False</returns>
    static member PointCompare(point1:Point3d, point2:Point3d, [<OPT;DEF(null)>]tolerance:float) : bool =
        failNotImpl () 


    ///<summary>Divides a 3D point by a value</summary>
    ///<param name="point">(Point3d) The point to divide</param>
    ///<param name="divide">(int) A non-zero value to divide</param>
    ///<returns>(Point3d) resulting point</returns>
    static member PointDivide(point:Point3d, divide:int) : Point3d =
        failNotImpl () 


    ///<summary>Verifies that a list of 3D points are coplanar</summary>
    ///<param name="points">(Point3d seq) 3D points to test</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>1.0e-12</c>
    ///Tolerance to use when verifying</param>
    ///<returns>(bool) True or False</returns>
    static member PointsAreCoplanar(points:Point3d seq, [<OPT;DEF(1.0e-12)>]tolerance:float) : bool =
        failNotImpl () 


    ///<summary>Scales a 3D point by a value</summary>
    ///<param name="point">(Point3d) The point to divide</param>
    ///<param name="scale">(float) Scale factor to apply</param>
    ///<returns>(Point3d) resulting point on success</returns>
    static member PointScale(point:Point3d, scale:float) : Point3d =
        failNotImpl () 


    ///<summary>Subtracts a 3D point or a 3D vector from a 3D point</summary>
    ///<param name="point1">(Point3d) Point1 of 'the points to subtract' (FIXME 0)</param>
    ///<param name="point2">(Point3d) Point2 of 'the points to subtract' (FIXME 0)</param>
    ///<returns>(Point3d) the resulting 3D point</returns>
    static member PointSubtract(point1:Point3d, point2:Point3d) : Point3d =
        failNotImpl () 


    ///<summary>Transforms a 3D point</summary>
    ///<param name="point">(Point3d) The point to transform</param>
    ///<param name="xform">(Transform) A valid 4x4 transformation matrix</param>
    ///<returns>(Vector3d) transformed vector on success</returns>
    static member PointTransform(point:Point3d, xform:Transform) : Vector3d =
        failNotImpl () 


    ///<summary>Projects one or more points onto one or more meshes</summary>
    ///<param name="points">(Point3d seq) One or more 3D points</param>
    ///<param name="meshIds">(Guid seq) Identifiers of one or more meshes</param>
    ///<param name="direction">(Vector3d) Direction vector to project the points</param>
    ///<returns>(Point3d seq) projected points on success</returns>
    static member ProjectPointToMesh(points:Point3d seq, meshIds:Guid seq, direction:Vector3d) : Point3d seq =
        failNotImpl () 


    ///<summary>Projects one or more points onto one or more surfaces or polysurfaces</summary>
    ///<param name="points">(Point3d seq) One or more 3D points</param>
    ///<param name="surfaceIds">(Guid seq) Identifiers of one or more surfaces/polysurfaces</param>
    ///<param name="direction">(Vector3d) Direction vector to project the points</param>
    ///<returns>(Point3d seq) projected points on success</returns>
    static member ProjectPointToSurface(points:Point3d seq, surfaceIds:Guid seq, direction:Vector3d) : Point3d seq =
        failNotImpl () 


    ///<summary>Pulls an array of points to a surface or mesh object. For more
    ///  information, see the Rhino help file Pull command</summary>
    ///<param name="objectId">(Guid) The identifier of the surface or mesh object that pulls</param>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<returns>(Point3d seq) 3D points pulled onto surface or mesh</returns>
    static member PullPoints(objectId:Guid, points:Point3d seq) : Point3d seq =
        failNotImpl () 


    ///<summary>Adds two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to add' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to add' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting 3D vector</returns>
    static member VectorAdd(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        failNotImpl () 


    ///<summary>Returns the angle, in degrees, between two 3-D vectors</summary>
    ///<param name="vector1">(Vector3d) The first 3-D vector.</param>
    ///<param name="vector2">(Vector3d) The second 3-D vector.</param>
    ///<returns>(float) The angle in degrees</returns>
    static member VectorAngle(vector1:Vector3d, vector2:Vector3d) : float =
        failNotImpl () 


    ///<summary>Compares two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the two vectors to compare' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the two vectors to compare' (FIXME 0)</param>
    ///<returns>(float) result of comparing the vectors.
    ///  -1 if vector1 is less than vector2
    ///  0 if vector1 is equal to vector2
    ///  1 if vector1 is greater than vector2</returns>
    static member VectorCompare(vector1:Vector3d, vector2:Vector3d) : float =
        failNotImpl () 


    ///<summary>Creates a vector from two 3D points</summary>
    ///<param name="toPoint">(Point3d) To point of 'the points defining the vector' (FIXME 0)</param>
    ///<param name="fromPoint">(Point3d) From point of 'the points defining the vector' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting vector</returns>
    static member VectorCreate(toPoint:Point3d, fromPoint:Point3d) : Vector3d =
        failNotImpl () 


    ///<summary>Calculates the cross product of two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to perform cross product on' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to perform cross product on' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting cross product direction</returns>
    static member VectorCrossProduct(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        failNotImpl () 


    ///<summary>Divides a 3D vector by a value</summary>
    ///<param name="vector">(Vector3d) The vector to divide</param>
    ///<param name="divide">(int) A non-zero value to divide</param>
    ///<returns>(Vector3d) resulting vector on success</returns>
    static member VectorDivide(vector:Vector3d, divide:int) : Vector3d =
        failNotImpl () 


    ///<summary>Calculates the dot product of two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to perform the dot product on' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to perform the dot product on' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting dot product</returns>
    static member VectorDotProduct(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        failNotImpl () 


    ///<summary>Returns the length of a 3D vector</summary>
    ///<param name="vector">(Vector3d) The 3-D vector.</param>
    ///<returns>(float) The length of the vector , otherwise None</returns>
    static member VectorLength(vector:Vector3d) : float =
        failNotImpl () 


    ///<summary>Multiplies two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to multiply' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to multiply' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting inner (dot) product</returns>
    static member VectorMultiply(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        failNotImpl () 


    ///<summary>Reverses the direction of a 3D vector</summary>
    ///<param name="vector">(Vector3d) The vector to reverse</param>
    ///<returns>(Vector3d) reversed vector on success</returns>
    static member VectorReverse(vector:Vector3d) : Vector3d =
        failNotImpl () 


    ///<summary>Rotates a 3D vector</summary>
    ///<param name="vector">(Vector3d) The vector to rotate</param>
    ///<param name="angleDegrees">(int) Rotation angle</param>
    ///<param name="axis">(Vector3d) Axis of rotation</param>
    ///<returns>(Vector3d) rotated vector on success</returns>
    static member VectorRotate(vector:Vector3d, angleDegrees:int, axis:Vector3d) : Vector3d =
        failNotImpl () 


    ///<summary>Scales a 3-D vector</summary>
    ///<param name="vector">(Vector3d) The vector to scale</param>
    ///<param name="scale">(float) Scale factor to apply</param>
    ///<returns>(Vector3d) resulting vector on success</returns>
    static member VectorScale(vector:Vector3d, scale:float) : Vector3d =
        failNotImpl () 


    ///<summary>Subtracts two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) The vector to subtract from</param>
    ///<param name="vector2">(Vector3d) The vector to subtract</param>
    ///<returns>(Vector3d) the resulting 3D vector</returns>
    static member VectorSubtract(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        failNotImpl () 


    ///<summary>Transforms a 3D vector</summary>
    ///<param name="vector">(Vector3d) The vector to transform</param>
    ///<param name="xform">(Transform) A valid 4x4 transformation matrix</param>
    ///<returns>(Vector3d) transformed vector on success</returns>
    static member VectorTransform(vector:Vector3d, xform:Transform) : Vector3d =
        failNotImpl () 


    ///<summary>Unitizes, or normalizes a 3D vector. Note, zero vectors cannot be unitized</summary>
    ///<param name="vector">(Vector3d) The vector to unitize</param>
    ///<returns>(unit) unitized vector on success</returns>
    static member VectorUnitize(vector:Vector3d) : unit =
        failNotImpl () 


    ///<summary>Returns either a world axis-aligned or a construction plane axis-aligned
    ///  bounding box of an array of 3-D point locations.</summary>
    ///<param name="points">(Point3d seq) A list of 3-D points</param>
    ///<param name="viewOrPlane">(Plane) Optional, Default Value: <c>null</c>
    ///Title or id of the view that contains the
    ///  construction plane to which the bounding box should be aligned -or-
    ///  user defined plane. If omitted, a world axis-aligned bounding box
    ///  will be calculated</param>
    ///<param name="inWorldCoords">(bool) Optional, Default Value: <c>true</c>
    ///Return the bounding box as world coordinates or
    ///  construction plane coordinates. Note, this option does not apply to
    ///  world axis-aligned bounding boxes.</param>
    ///<returns>(Point3d seq) Eight points that define the bounding box. Points returned in counter-
    ///  clockwise order starting with the bottom rectangle of the box.</returns>
    static member PointArrayBoundingBox(points:Point3d seq, [<OPT;DEF(null)>]viewOrPlane:Plane, [<OPT;DEF(true)>]inWorldCoords:bool) : Point3d seq =
        failNotImpl () 


