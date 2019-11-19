namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
open System.Collections.Generic

[<AutoOpen>]
module ExtensionsPointvector =

  type RhinoScriptSyntax with
    [<EXT>]
    ///<summary>Compares two vectors to see if they are parallel within one degree or custom tolerance</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to compare</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to compare</param>
    ///<param name="toleranceDegree">(float) Optional, Default Value: <c>1.0</c>
    ///  Angle Tolerance in degree</param>
    ///<returns>(int) the value represents
    ///   -1 = the vectors are anti-parallel
    ///    0 = the vectors are not parallel
    ///    1 = the vectors are parallel</returns>
    static member IsVectorParallelTo(   vector1:Vector3d,
                                        vector2:Vector3d,
                                        [<OPT;DEF(0.0)>]toleranceDegree:float) : int =
        //vector1 = RhinoScriptSyntax.Coerce3dvector(vector1)
        //vector2 = RhinoScriptSyntax.Coerce3dvector(vector2)
        if toleranceDegree = 0.0 then vector1.IsParallelTo(vector2)
        else vector1.IsParallelTo(vector2, toRadians(toleranceDegree))


    [<EXT>]
    ///<summary>Compares two vectors to see if they are perpendicular</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to compare</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to compare</param>
    ///<param name="toleranceDegree">(float) Optional, Default Value: <c>1.0</c>
    ///  Angle Tolerance in degree</param>
    ///<returns>(bool) True if vectors are perpendicular, otherwise False</returns>
    static member IsVectorPerpendicularTo(  vector1:Vector3d,
                                            vector2:Vector3d,
                                            [<OPT;DEF(0.0)>]toleranceDegree:float) : bool =
        //vector1 = RhinoScriptSyntax.Coerce3dvector(vector1)
        //vector2 = RhinoScriptSyntax.Coerce3dvector(vector2)
        if toleranceDegree = 0.0 then vector1.IsPerpendicularTo(vector2)
        else vector1.IsPerpendicularTo(vector2, toRadians(toleranceDegree))



    [<EXT>]
    ///<summary>Verifies that a vector is very short. The X, Y, Z elements are smaler than 1.0e-12</summary>
    ///<param name="vector">(Vector3d) The vector to check</param>
    ///<returns>(bool) True if the vector is tiny, otherwise False</returns>
    static member IsVectorTiny(vector:Vector3d) : bool =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        vector.IsTiny( 1.0e-12 )


    [<EXT>]
    ///<summary>Verifies that a vector is zero, or tiny. The X, Y, Z elements are equal to 0.0</summary>
    ///<param name="vector">(Vector3d) The vector to check</param>
    ///<returns>(bool) True if the vector is zero, otherwise False</returns>
    static member IsVectorZero(vector:Vector3d) : bool =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        vector.IsZero


    [<EXT>]
    ///<summary>Adds a 3D point or a 3D vector to a 3D point</summary>
    ///<param name="point1">(Point3d) Point1 of the points to add</param>
    ///<param name="point2">(Point3d) Point2 of the points to add</param>
    ///<returns>(Point3d) the resulting 3D point</returns>
    static member PointAdd(point1:Point3d, point2:Point3d) : Point3d =
        //point1 = RhinoScriptSyntax.Coerce3dpoint(point1)
        //point2 = RhinoScriptSyntax.Coerce3dpoint(point2)
        point1+point2


    [<EXT>]
    ///<summary>Finds the point in a list of 3D points that is closest to a test point</summary>
    ///<param name="points">(Point3d IList) List of points</param>
    ///<param name="testPoint">(Point3d) The point to compare against</param>
    ///<returns>(int) index of the element in the point list that is closest to the test point</returns>
    static member PointArrayClosestPoint(points:Point3d IList, testPoint:Point3d) : int =
        //points = RhinoScriptSyntax.Coerce3dpointlist(points)
        //testPoint = RhinoScriptSyntax.Coerce3dpoint(testPoint)
        let index = Rhino.Collections.Point3dList.ClosestIndexInList(points, testPoint)
        if index>=0 then index
        else failwithf "PointArrayClosestPoint failed on %A, %A" points testPoint


    [<EXT>]
    ///<summary>Transforms a list of 3D points</summary>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<param name="xform">(Transform) Transformation to apply</param>
    ///<returns>(Point3d ResizeArray) transformed points on success</returns>
    static member PointArrayTransform(points:Point3d seq, xform:Transform) : Point3d ResizeArray =
        //points = RhinoScriptSyntax.Coerce3dpointlist(points)
        //xform = RhinoScriptSyntax.Coercexform(xform)
        resizeArray {for point in points do xform*point}

    [<EXT>]
    ///<summary>Finds the object that is closest to a test point</summary>
    ///<param name="point">(Point3d) Point to test</param>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more objects</param>
    ///<returns>(Guid * Point3d * float) Tuple of 3 values
    ///    [0] Guid, closest  objectId
    ///    [1] the point on object
    ///    [2] the distance</returns>
    static member PointClosestObject(point:Point3d, objectIds:Guid seq) : Guid * Point3d * float =
        //objectIds = RhinoScriptSyntax.Coerceguidlist(objectIds)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        let mutable closest = Unchecked.defaultof<Guid*Point3d*float>
        let mutable distance = Double.MaxValue
        for objectId in objectIds do
            let geom = RhinoScriptSyntax.CoerceGeometry(objectId)
            match geom with
            | :?  Point as pointgeometry ->
                distance <- point.DistanceTo( pointgeometry.Location )
                if distance < t3 closest then
                    closest  <-  objectId, pointgeometry.Location, distance

            | :?  PointCloud as pointcloud ->
                let index = pointcloud.ClosestPoint(point)
                if index>=0 then
                    distance <- point.DistanceTo( pointcloud.[index].Location )
                    if distance < t3 closest then
                        closest  <-  objectId, pointcloud.[index].Location, distance

            | :?  Curve as curve ->
                let rc, t = curve.ClosestPoint(point)
                if rc then
                    distance <- point.DistanceTo( curve.PointAt(t) )
                    if distance < t3 closest then
                        closest  <-  objectId, curve.PointAt(t), distance

            | :?  Surface as srf ->
                let ok, u, v = srf.ClosestPoint(point)
                if ok then
                    let srfclosest = srf.PointAt(u, v)
                    distance <- point.DistanceTo( srfclosest )
                    if distance < t3 closest then
                        closest  <-  objectId, srfclosest, distance

            | :?  Brep as brep ->
                let brepclosest = brep.ClosestPoint(point)
                distance <- point.DistanceTo( brepclosest )
                if distance < t3 closest then
                    closest  <-  objectId, brepclosest, distance

            | :?  Mesh as mesh ->
                let meshclosest = mesh.ClosestPoint(point)
                distance <- point.DistanceTo( meshclosest )
                if distance < t3 closest then
                    closest  <-  objectId, meshclosest, distance

            | _ -> failwithf "PointClosestObject: non supported object type %A %A ose Point, Pointcloud, Curve, Brep or Mesh" (RhinoScriptSyntax.ObjectDescription(objectId)) objectId

        if t1 closest <> Guid.Empty then closest
        else failwithf "PointClosestObject failed on %A and %A " point objectIds


    [<EXT>]
    ///<summary>Compares two 3D points</summary>
    ///<param name="point1">(Point3d) Point1 of the points to compare</param>
    ///<param name="point2">(Point3d) Point2 of the points to compare</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///Tolerance to use for comparison.</param>
    ///<returns>(bool) True or False</returns>
    static member PointCompare( point1:Point3d,
                                point2:Point3d,
                                [<OPT;DEF(0.0)>]tolerance:float) : bool =
        //point1 = RhinoScriptSyntax.Coerce3dpoint(point1)
        //point2 = RhinoScriptSyntax.Coerce3dpoint(point2)
        let tolerance = ifZero2 RhinoMath.ZeroTolerance  tolerance
        let vector = point2-point1
        vector.IsTiny(tolerance)


    [<EXT>]
    ///<summary>Divides a 3D point by a value</summary>
    ///<param name="point">(Point3d) The point to divide</param>
    ///<param name="divide">(float) A non-zero value to divide</param>
    ///<returns>(Point3d) resulting point</returns>
    static member PointDivide(point:Point3d, divide:float) : Point3d =
        if divide < RhinoMath.ZeroTolerance && divide > -RhinoMath.ZeroTolerance then
            failwithf "PointDivide: Cannot devide by Zero or almost Zero %f" divide
        else
            point/divide


    [<EXT>]
    ///<summary>Verifies that a list of 3D points are coplanar</summary>
    ///<param name="points">(Point3d seq) 3D points to test</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>1.0e-12</c> = RhinoMath.ZeroTolerance
    ///Tolerance to use when verifying </param>
    ///<returns>(bool) True or False</returns>
    static member PointsAreCoplanar(points:Point3d seq, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        //points = RhinoScriptSyntax.Coerce3dpointlist(points)
        let tolerance = ifZero1 tolerance RhinoMath.ZeroTolerance
        Point3d.ArePointsCoplanar(points, tolerance)


    [<EXT>]
    ///<summary>Scales a 3D point by a value</summary>
    ///<param name="point">(Point3d) The point to divide</param>
    ///<param name="scale">(float) Scale factor to apply</param>
    ///<returns>(Point3d) resulting point on success</returns>
    static member PointScale(point:Point3d, scale:float) : Point3d =
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        point*scale


    [<EXT>]
    ///<summary>Subtracts a 3D point or a 3D vector from a 3D point</summary>
    ///<param name="point1">(Point3d) Point1 of the points to subtract</param>
    ///<param name="point2">(Point3d) Point2 of the points to subtract</param>
    ///<returns>(Point3d) the resulting 3D point</returns>
    static member PointSubtract(point1:Point3d, point2:Point3d) : Point3d =
        //point1 = RhinoScriptSyntax.Coerce3dpoint(point1)
        //point2 = RhinoScriptSyntax.Coerce3dpoint(point2)
        let v = point1-point2
        Point3d(v)


    [<EXT>]
    ///<summary>Transforms a 3D point</summary>
    ///<param name="point">(Point3d) The point to transform</param>
    ///<param name="xform">(Transform) A valid 4x4 transformation matrix</param>
    ///<returns>(Point3d) transformed Pooint on success</returns>
    static member PointTransform(point:Point3d, xform:Transform) : Point3d =
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        //xform = RhinoScriptSyntax.Coercexform(xform)
        xform*point


    [<EXT>]
    ///<summary>Projects one or more points onto one or more meshes</summary>
    ///<param name="points">(Point3d seq) One or more 3D points</param>
    ///<param name="meshIds">(Guid seq) Identifiers of one or more meshes</param>
    ///<param name="direction">(Vector3d) Direction vector to project the points</param>
    ///<returns>(Point3d array) projected points on success</returns>
    static member ProjectPointToMesh( points:Point3d seq,
                                      meshIds:Guid seq,
                                      direction:Vector3d) : Point3d array =
        let meshes =  resizeArray { for objectId in meshIds do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        Intersect.Intersection.ProjectPointsToMeshes(meshes, points, direction, tolerance)



    [<EXT>]
    ///<summary>Projects one or more points onto one or more surfaces or polysurfaces</summary>
    ///<param name="points">(Point3d seq) One or more 3D points</param>
    ///<param name="surfaceIds">(Guid seq) Identifiers of one or more surfaces/polysurfaces</param>
    ///<param name="direction">(Vector3d) Direction vector to project the points</param>
    ///<returns>(Point3d array) projected points on success</returns>
    static member ProjectPointToSurface( points:Point3d seq,
                                         surfaceIds:Guid seq,
                                         direction:Vector3d) : Point3d array =
        let breps =  resizeArray { for objectId in surfaceIds do yield RhinoScriptSyntax.CoerceBrep(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        Intersect.Intersection.ProjectPointsToBreps(breps, points, direction, tolerance)


    [<EXT>]
    ///<summary>Pulls an array of points to a surface or mesh object. For more
    ///  information, see the Rhino help file Pull command</summary>
    ///<param name="objectId">(Guid) The identifier of the surface or mesh object that pulls</param>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<returns>(Point3d array) 3D points pulled onto surface or mesh</returns>
    static member PullPoints(objectId:Guid, points:Point3d seq) : Point3d array =
        //id = RhinoScriptSyntax.Coerceguid(objectId)
        //points = RhinoScriptSyntax.Coerce3dpointlist(points)
        match RhinoScriptSyntax.CoerceGeometry(objectId) with
        | :? Mesh as mesh->
            let points = mesh.PullPointsToMesh(points)
            points
        //brep = RhinoScriptSyntax.Coercebrep(objectId)
        | :? Brep as brep->
            if brep.Faces.Count = 1 then
                let tolerance = Doc.ModelAbsoluteTolerance
                brep.Faces.[0].PullPointsToFace(points, tolerance)
            else
                failwithf "PullPoints only works on surface and single sided breps not %d sided ones" brep.Faces.Count
        | _ -> failwithf "PullPoints does not support %A" (RhinoScriptSyntax.ObjectDescription(objectId) )


    [<EXT>]
    ///<summary>Adds two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to add</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to add</param>
    ///<returns>(Vector3d) the resulting 3D vector</returns>
    static member VectorAdd(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        //vector1 = RhinoScriptSyntax.Coerce3dvector(vector1)
        //vector2 = RhinoScriptSyntax.Coerce3dvector(vector2)
        vector1+vector2


    [<EXT>]
    ///<summary>Returns the angle, in degrees, between two 3-D vectors</summary>
    ///<param name="vector1">(Vector3d) The first 3-D vector</param>
    ///<param name="vector2">(Vector3d) The second 3-D vector</param>
    ///<returns>(float) The angle in degrees</returns>
    static member VectorAngle(vector1:Vector3d, vector2:Vector3d) : float =
        //vector1 = RhinoScriptSyntax.Coerce3dvector(vector1)
        //vector2 = RhinoScriptSyntax.Coerce3dvector(vector2)
        let vector1 = Vector3d(vector1.X, vector1.Y, vector1.Z)
        let vector2 = Vector3d(vector2.X, vector2.Y, vector2.Z)
        if not <| vector1.Unitize() || not <| vector2.Unitize() then
            failwithf "Rhino.Scripting.VectorAngle: Unable to unitize vector.  vector1:'%A' vector2:'%A'" vector1 vector2
        let mutable dot = vector1 * vector2
        dot <- RhinoScriptSyntax.Clamp(-1.0 ,1.0 , dot)
        let radians = Math.Acos(dot)
        toDegrees(radians)


    [<EXT>]
    ///<summary>Compares two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the two vectors to compare</param>
    ///<param name="vector2">(Vector3d) Vector2 of the two vectors to compare</param>
    ///<returns>(int) result of comparing the vectors.
    ///  -1 if vector1 is less than vector2
    ///  0 if vector1 is equal to vector2
    ///  1 if vector1 is greater than vector2</returns>
    static member VectorCompare(vector1:Vector3d, vector2:Vector3d) : int =
        //vector1 = RhinoScriptSyntax.Coerce3dvector(vector1)
        //vector2 = RhinoScriptSyntax.Coerce3dvector(vector2)
        vector1.CompareTo(vector2)


    [<EXT>]
    ///<summary>Creates a vector from two 3D points</summary>
    ///<param name="fromPoint">(Point3d) Start point of vector</param>
    ///<param name="toPoint">(Point3d) End point vector</param>
    ///<returns>(Vector3d) the resulting vector</returns>
    static member VectorCreate( fromPoint:Point3d, toPoint:Point3d) : Vector3d =
        //toPoint = RhinoScriptSyntax.Coerce3dpoint(toPoint)
        //fromPoint = RhinoScriptSyntax.Coerce3dpoint(fromPoint)
        toPoint-fromPoint


    [<EXT>]
    ///<summary>Calculates the cross product of two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to perform cross product on</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to perform cross product on</param>
    ///<returns>(Vector3d) the resulting cross product direction</returns>
    static member VectorCrossProduct(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        //vector1 = RhinoScriptSyntax.Coerce3dvector(vector1)
        //vector2 = RhinoScriptSyntax.Coerce3dvector(vector2)
        Vector3d.CrossProduct( vector1, vector2 )


    [<EXT>]
    ///<summary>Divides a 3D vector by a value</summary>
    ///<param name="vector">(Vector3d) The vector to divide</param>
    ///<param name="divide">(float) A non-zero value to divide</param>
    ///<returns>(Vector3d) resulting vector on success</returns>
    static member VectorDivide(vector:Vector3d, divide:float) : Vector3d =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        if divide < RhinoMath.ZeroTolerance && divide > -RhinoMath.ZeroTolerance then
            failwithf "VectorDivide: Cannot devide by Zero or almost Zero %f" divide
        else
            vector/divide


    [<EXT>]
    ///<summary>Calculates the dot product of two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to perform the dot product on</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to perform the dot product on</param>
    ///<returns>(float) the resulting dot product</returns>
    static member VectorDotProduct(vector1:Vector3d, vector2:Vector3d) : float =
        //vector1 = RhinoScriptSyntax.Coerce3dvector(vector1)
        //vector2 = RhinoScriptSyntax.Coerce3dvector(vector2)
        vector1*vector2


    [<EXT>]
    ///<summary>Returns the length of a 3D vector</summary>
    ///<param name="vector">(Vector3d) The 3-D vector</param>
    ///<returns>(float) The length of the vector , otherwise None</returns>
    static member VectorLength(vector:Vector3d) : float =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        vector.Length


    [<EXT>]
    ///<summary>Multiplies two 3D vectors, same as Dot Product</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to multiply</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to multiply</param>
    ///<returns>(float) the resulting inner (dot) product</returns>
    static member VectorMultiply(vector1:Vector3d, vector2:Vector3d) : float =
        vector1* vector2


    [<EXT>]
    ///<summary>Reverses the direction of a 3D vector</summary>
    ///<param name="vector">(Vector3d) The vector to reverse</param>
    ///<returns>(Vector3d) reversed vector on success</returns>
    static member VectorReverse(vector:Vector3d) : Vector3d =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        Vector3d(-vector.X, -vector.Y, -vector.Z)



    [<EXT>]
    ///<summary>Rotates a 3D vector</summary>
    ///<param name="vector">(Vector3d) The vector to rotate</param>
    ///<param name="angleDegrees">(float) Rotation angle</param>
    ///<param name="axis">(Vector3d) Axis of rotation</param>
    ///<returns>(Vector3d) rotated vector on success</returns>
    static member VectorRotate( vector:Vector3d,
                                angleDegrees:float,
                                axis:Vector3d) : Vector3d =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        //axis = RhinoScriptSyntax.Coerce3dvector(axis)
        let angleradians = RhinoMath.ToRadians(angleDegrees)
        let rc = Vector3d(vector.X, vector.Y, vector.Z)
        if rc.Rotate(angleradians, axis) then rc
        else failwithf "VectorRotate failed on %A, %A, %A" vector angleDegrees axis


    [<EXT>]
    ///<summary>Scales a 3-D vector</summary>
    ///<param name="vector">(Vector3d) The vector to scale</param>
    ///<param name="scale">(float) Scale factor to apply</param>
    ///<returns>(Vector3d) resulting vector on success</returns>
    static member VectorScale(vector:Vector3d, scale:float) : Vector3d =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        vector*scale


    [<EXT>]
    ///<summary>Subtracts two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) The vector to subtract from</param>
    ///<param name="vector2">(Vector3d) The vector to subtract</param>
    ///<returns>(Vector3d) the resulting 3D vector</returns>
    static member VectorSubtract(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        //vector1 = RhinoScriptSyntax.Coerce3dvector(vector1)
        //vector2 = RhinoScriptSyntax.Coerce3dvector(vector2)
        vector1-vector2


    [<EXT>]
    ///<summary>Transforms a 3D vector</summary>
    ///<param name="vector">(Vector3d) The vector to transform</param>
    ///<param name="xform">(Transform) A valid 4x4 transformation matrix</param>
    ///<returns>(Vector3d) transformed vector on success</returns>
    static member VectorTransform(vector:Vector3d, xform:Transform) : Vector3d =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        //xform = RhinoScriptSyntax.Coercexform(xform)
        xform*vector


    [<EXT>]
    ///<summary>Unitizes, or normalizes a 3D vector. Note, zero vectors cannot be unitized</summary>
    ///<param name="vector">(Vector3d) The vector to unitize</param>
    ///<returns>(Vector3d) unitized vector on success</returns>
    static member VectorUnitize(vector:Vector3d) : Vector3d =
        let ll = sqrt vector.SquareLength
        if Double.IsInfinity ll || ll < RhinoMath.ZeroTolerance then failwithf "VectorUnitize failed on zero length or very short Vector %A" vector
        let l = 1. / ll
        Vector3d(vector.X*l, vector.Y*l, vector.Z*l)


    [<EXT>]
    ///<summary>Returns either a world axis-aligned or a construction plane axis-aligned
    ///  bounding box of an array of 3-D point locations</summary>
    ///<param name="points">(Point3d seq) A list of 3-D points</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldXY</c>
    ///  Plane to which the bounding box should be aligned,
    /// If omitted, a world axis-aligned bounding box
    ///  will be calculated</param>
    ///<returns>(Box) A Rhino.Geometry.Box</returns>
    static member PointArrayBoundingBox( points:Point3d seq, [<OPT;DEF(Plane())>]plane:Plane) : Box = // TODO verify this works the same way as python !!
        if plane.IsValid then
            Box(plane, points)
        else
            Box(BoundingBox(points))


