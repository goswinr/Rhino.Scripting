
namespace Rhino.Scripting 

open Rhino 

open System
open System.Collections.Generic

open Rhino.Geometry


open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore

[<AutoOpen>]
module AutoOpenPointVector =
  type RhinoScriptSyntax with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>Compares two vectors to see if they are parallel within one degree or custom tolerance.</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to compare</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to compare</param>
    ///<param name="toleranceDegree">(float) Optional, default value: <c>1.0</c>
    ///    Angle Tolerance in degree</param>
    ///<returns>(int) The value represents
    ///     -1 = the vectors are anti-parallel
    ///      0 = the vectors are not parallel
    ///      1 = the vectors are parallel.</returns>
    static member IsVectorParallelTo(   vector1:Vector3d,
                                        vector2:Vector3d,
                                        [<OPT;DEF(0.0)>]toleranceDegree:float) : int = 
        if toleranceDegree = 0.0 then vector1.IsParallelTo(vector2)
        else vector1.IsParallelTo(vector2, toRadians(toleranceDegree))


    ///<summary>Compares two vectors to see if they are perpendicular.</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to compare</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to compare</param>
    ///<param name="toleranceDegree">(float) Optional, default value: <c>1.0</c>
    ///    Angle Tolerance in degree</param>
    ///<returns>(bool) True if vectors are perpendicular, otherwise False.</returns>
    static member IsVectorPerpendicularTo(  vector1:Vector3d,
                                            vector2:Vector3d,
                                            [<OPT;DEF(0.0)>]toleranceDegree:float) : bool = 
        if toleranceDegree = 0.0 then vector1.IsPerpendicularTo(vector2)
        else vector1.IsPerpendicularTo(vector2, toRadians(toleranceDegree))



    ///<summary>Checks if a vector is very short. The X, Y, Z elements are smaller than 1.0e-12.</summary>
    ///<param name="vector">(Vector3d) The vector to check</param>
    ///<returns>(bool) True if the vector is tiny, otherwise False.</returns>
    static member IsVectorTiny(vector:Vector3d) : bool = 
        vector.IsTiny( 1.0e-12 )


    ///<summary>Checks if a vector is zero. The X, Y, Z elements are equal to 0.0.</summary>
    ///<param name="vector">(Vector3d) The vector to check</param>
    ///<returns>(bool) True if the vector is zero, otherwise False.</returns>
    static member IsVectorZero(vector:Vector3d) : bool = 
        vector.IsZero


    ///<summary>Adds a 3D point or a 3D vector to a 3D point.</summary>
    ///<param name="point1">(Point3d) Point1 of the points to add</param>
    ///<param name="point2">(Point3d) Point2 of the points to add</param>
    ///<returns>(Point3d) The resulting 3D point.</returns>
    static member PointAdd(point1:Point3d, point2:Point3d) : Point3d = 
        point1 + point2


    ///<summary>Finds the point in a list of 3D points that is closest to a test point.</summary>
    ///<param name="points">(Point3d IList) List of points</param>
    ///<param name="testPoint">(Point3d) The point to compare against</param>
    ///<returns>(int) index of the element in the point list that is closest to the test point.</returns>
    static member PointArrayClosestPoint(points:Point3d IList, testPoint:Point3d) : int = 
        let index = Rhino.Collections.Point3dList.ClosestIndexInList(points, testPoint)
        if index>=0 then index
        else RhinoScriptingException.Raise "RhinoScriptSyntax.PointArrayClosestPoint failed on %A, %A" points testPoint


    ///<summary>Transforms a list of 3D points.</summary>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<param name="xForm">(Transform) Transformation to apply</param>
    ///<returns>(Point3d Rarr) transformed points.</returns>
    static member PointArrayTransform(points:Point3d seq, xForm:Transform) : Point3d Rarr = 
        rarr {for point in points do
                let p = Point3d(point) //copy first !
                p.Transform(xForm)
                p}

    ///<summary>Finds the object that is closest to a test point.</summary>
    ///<param name="point">(Point3d) Point to test</param>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more objects</param>
    ///<returns>(Guid * Point3d * float) Tuple of 3 values
    ///      [0] Guid, closest  objectId
    ///      [1] the point on object
    ///      [2] the distance.</returns>
    static member PointClosestObject(point:Point3d, objectIds:Guid seq) : Guid * Point3d * float = 
        let mutable closest = Unchecked.defaultof<Guid*Point3d*float>
        let mutable distance = Double.MaxValue
        for objectId in objectIds do
            let geom = RhinoScriptSyntax.CoerceGeometry(objectId)
            match geom with
            | :?  Point as pointgeometry ->
                distance <- point.DistanceTo( pointgeometry.Location )
                if distance < t3 closest then
                    closest  <-  objectId, pointgeometry.Location, distance

            | :?  TextDot as dot ->
                distance <- point.DistanceTo(dot.Point)
                if distance < t3 closest then
                    closest  <-  objectId, dot.Point, distance

            | :?  PointCloud as pointcloud ->
                let index = pointcloud.ClosestPoint(point)
                if index>=0 then
                    distance <- point.DistanceTo( pointcloud.[index].Location )
                    if distance < t3 closest then
                        closest  <-  objectId, pointcloud.[index].Location, distance

            | :?  Curve as curve ->
                let rc, t = curve.ClosestPoint(point)
                if rc then
                    distance <- point.DistanceTo( curve.PointAt(t))
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

            | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.PointClosestObject: non supported object type %A %A  Point, PointCloud, Curve, Brep or Mesh" (RhinoScriptSyntax.ObjectDescription(objectId)) objectId

        if t1 closest <> Guid.Empty then closest
        else RhinoScriptingException.Raise "RhinoScriptSyntax.PointClosestObject failed on %A and %A" point objectIds


    ///<summary>Compares two 3D points.</summary>
    ///<param name="point1">(Point3d) Point1 of the points to compare</param>
    ///<param name="point2">(Point3d) Point2 of the points to compare</param>
    ///<param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c>
    ///    Tolerance to use for comparison.</param>
    ///<returns>(bool) True or False.</returns>
    static member PointCompare( point1:Point3d,
                                point2:Point3d,
                                [<OPT;DEF(0.0)>]tolerance:float) : bool = 
        let tolerance = Util.ifZero2 RhinoMath.ZeroTolerance  tolerance
        let vector = point2-point1
        vector.IsTiny(tolerance)


    ///<summary>Divides a 3D point by a value.</summary>
    ///<param name="point">(Point3d) The point to divide</param>
    ///<param name="divide">(float) A non-zero value to divide</param>
    ///<returns>(Point3d) resulting point.</returns>
    static member PointDivide(point:Point3d, divide:float) : Point3d = 
        if divide < RhinoMath.ZeroTolerance && divide > -RhinoMath.ZeroTolerance then
            RhinoScriptingException.Raise "RhinoScriptSyntax.PointDivide: Cannot divide by Zero or almost Zero %f" divide
        else
            point/divide


    ///<summary>Checks if a list of 3D points are coplanar.</summary>
    ///<param name="points">(Point3d seq) 3D points to test</param>
    ///<param name="tolerance">(float) Optional, default value: <c>1.0e-12</c> = RhinoMath.ZeroTolerance
    ///    Tolerance to use when verifying</param>
    ///<returns>(bool) True or False.</returns>
    static member PointsAreCoplanar(points:Point3d seq, [<OPT;DEF(0.0)>]tolerance:float) : bool = 
        let tolerance = Util.ifZero1 tolerance RhinoMath.ZeroTolerance
        Point3d.ArePointsCoplanar(points, tolerance)


    ///<summary>Scales a 3D point by a value.</summary>
    ///<param name="point">(Point3d) The point to divide</param>
    ///<param name="scale">(float) Scale factor to apply</param>
    ///<returns>(Point3d) resulting point.</returns>
    static member PointScale(point:Point3d, scale:float) : Point3d = 
        point*scale


    ///<summary>Subtracts a 3D point or a 3D vector from a 3D point.</summary>
    ///<param name="point1">(Point3d) Point1 of the points to subtract</param>
    ///<param name="point2">(Point3d) Point2 of the points to subtract</param>
    ///<returns>(Point3d) The resulting 3D point.</returns>
    static member PointSubtract(point1:Point3d, point2:Point3d) : Point3d = 
        let v = point1-point2
        Point3d(v)


    ///<summary>Transforms a 3D point.</summary>
    ///<param name="point">(Point3d) The point to transform</param>
    ///<param name="xForm">(Transform) A valid 4x4 transformation matrix</param>
    ///<returns>(Point3d) transformed Point.</returns>
    static member PointTransform(point:Point3d, xForm:Transform) : Point3d = 
        let p = Point3d(point) //copy first !
        p.Transform(xForm)
        p



    ///<summary>Projects one or more points onto one or more Meshes.</summary>
    ///<param name="points">(Point3d seq) One or more 3D points</param>
    ///<param name="meshIds">(Guid seq) Identifiers of one or more Meshes</param>
    ///<param name="direction">(Vector3d) Direction vector to project the points</param>
    ///<returns>(Point3d array) projected points.</returns>
    static member ProjectPointToMesh( points:Point3d seq,
                                      meshIds:Guid seq,
                                      direction:Vector3d) : Point3d array = 
        let meshes =  rarr { for objectId in meshIds do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let tolerance = State.Doc.ModelAbsoluteTolerance
        Intersect.Intersection.ProjectPointsToMeshes(meshes, points, direction, tolerance)



    ///<summary>Projects one or more points onto one or more Surfaces or Polysurfaces.</summary>
    ///<param name="points">(Point3d seq) One or more 3D points</param>
    ///<param name="surfaceIds">(Guid seq) Identifiers of one or more Surfaces/polysurfaces</param>
    ///<param name="direction">(Vector3d) Direction vector to project the points</param>
    ///<returns>(Point3d array) projected points.</returns>
    static member ProjectPointToSurface( points:Point3d seq,
                                         surfaceIds:Guid seq,
                                         direction:Vector3d) : Point3d array = 
        let breps =  rarr { for objectId in surfaceIds do yield RhinoScriptSyntax.CoerceBrep(objectId) }
        let tolerance = State.Doc.ModelAbsoluteTolerance
        Intersect.Intersection.ProjectPointsToBreps(breps, points, direction, tolerance)


    ///<summary>Pulls an array of points to a Surface or Mesh object. For more
    ///    information, see the Rhino help file Pull command.</summary>
    ///<param name="objectId">(Guid) The identifier of the Surface or Mesh object that pulls</param>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<returns>(Point3d array) 3D points pulled onto Surface or Mesh.</returns>
    static member PullPoints(objectId:Guid, points:Point3d seq) : Point3d array = 
        match RhinoScriptSyntax.CoerceGeometry(objectId) with
        | :? Mesh as mesh->
            let points = mesh.PullPointsToMesh(points)
            points       
        | :? Brep as brep->
            if brep.Faces.Count = 1 then
                let tolerance = State.Doc.ModelAbsoluteTolerance
                brep.Faces.[0].PullPointsToFace(points, tolerance)
            else
                RhinoScriptingException.Raise "RhinoScriptSyntax.PullPoints only works on surface and single sided breps not %d sided ones" brep.Faces.Count
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.PullPoints does not support %A" (RhinoScriptSyntax.ObjectDescription(objectId))


    ///<summary>Adds two 3D vectors.</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to add</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to add</param>
    ///<returns>(Vector3d) The resulting 3D vector.</returns>
    static member VectorAdd(vector1:Vector3d, vector2:Vector3d) : Vector3d = 
        vector1 + vector2


    ///<summary>Returns the angle, in degrees, between two 3-D vectors.</summary>
    ///<param name="vector1">(Vector3d) The first 3-D vector</param>
    ///<param name="vector2">(Vector3d) The second 3-D vector</param>
    ///<returns>(float) The angle in degrees.</returns>
    static member VectorAngle(vector1:Vector3d, vector2:Vector3d) : float = 
        let vector1 = Vector3d(vector1.X, vector1.Y, vector1.Z)
        let vector2 = Vector3d(vector2.X, vector2.Y, vector2.Z)
        if not <| vector1.Unitize() || not <| vector2.Unitize() then
            RhinoScriptingException.Raise "RhinoScriptSyntax.VectorAngle: Unable to unitize vector.  vector1:'%A' vector2:'%A'" vector1 vector2
        let mutable dot = vector1 * vector2
        dot <- RhinoScriptSyntax.Clamp(-1.0 , 1.0 , dot)
        let radians = Math.Acos(dot)
        toDegrees(radians)


    ///<summary>Compares two 3D vectors.</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the two vectors to compare</param>
    ///<param name="vector2">(Vector3d) Vector2 of the two vectors to compare</param>
    ///<returns>(int) result of comparing the vectors.
    ///    -1 if vector1 is less than vector2
    ///    0 if vector1 is equal to vector2
    ///    1 if vector1 is greater than vector2.</returns>
    static member VectorCompare(vector1:Vector3d, vector2:Vector3d) : int = 
        vector1.CompareTo(vector2)


    ///<summary>Creates a vector from two 3D points.</summary>
    ///<param name="fromPoint">(Point3d) Start point of vector</param>
    ///<param name="toPoint">(Point3d) End point vector</param>
    ///<returns>(Vector3d) The resulting vector.</returns>
    static member VectorCreate( fromPoint:Point3d, toPoint:Point3d) : Vector3d = 
        toPoint-fromPoint


    ///<summary>Calculates the cross product of two 3D vectors.</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to perform cross product on</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to perform cross product on</param>
    ///<returns>(Vector3d) The resulting cross product direction.</returns>
    static member VectorCrossProduct(vector1:Vector3d, vector2:Vector3d) : Vector3d = 
        Vector3d.CrossProduct( vector1, vector2 )


    ///<summary>Divides a 3D vector by a value.</summary>
    ///<param name="vector">(Vector3d) The vector to divide</param>
    ///<param name="divide">(float) A non-zero value to divide</param>
    ///<returns>(Vector3d) resulting vector.</returns>
    static member VectorDivide(vector:Vector3d, divide:float) : Vector3d = 
        if divide < RhinoMath.ZeroTolerance && divide > -RhinoMath.ZeroTolerance then
            RhinoScriptingException.Raise "RhinoScriptSyntax.VectorDivide: Cannot divide by Zero or almost Zero %f" divide
        else
            vector/divide


    ///<summary>Calculates the dot product of two 3D vectors.</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to perform the dot product on</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to perform the dot product on</param>
    ///<returns>(float) The resulting dot product.</returns>
    static member VectorDotProduct(vector1:Vector3d, vector2:Vector3d) : float = 
        vector1*vector2


    ///<summary>Returns the length of a 3D vector.</summary>
    ///<param name="vector">(Vector3d) The 3-D vector</param>
    ///<returns>(float) The length of the vector.</returns>
    static member VectorLength(vector:Vector3d) : float = 
        vector.Length


    ///<summary>Multiplies two 3D vectors, same as Dot Product.</summary>
    ///<param name="vector1">(Vector3d) Vector1 of the vectors to multiply</param>
    ///<param name="vector2">(Vector3d) Vector2 of the vectors to multiply</param>
    ///<returns>(float) The resulting inner (dot) product.</returns>
    static member VectorMultiply(vector1:Vector3d, vector2:Vector3d) : float = 
        vector1* vector2


    ///<summary>Reverses the direction of a 3D vector.</summary>
    ///<param name="vector">(Vector3d) The vector to reverse</param>
    ///<returns>(Vector3d) reversed vector.</returns>
    static member VectorReverse(vector:Vector3d) : Vector3d = 
        Vector3d(-vector.X, -vector.Y, -vector.Z)



    ///<summary>Rotates a 3D vector.</summary>
    ///<param name="vector">(Vector3d) The vector to rotate</param>
    ///<param name="angleDegrees">(float) Rotation angle</param>
    ///<param name="axis">(Vector3d) Axis of rotation</param>
    ///<returns>(Vector3d) rotated vector.</returns>
    static member VectorRotate( vector:Vector3d,
                                angleDegrees:float,
                                axis:Vector3d) : Vector3d = 
        let angleradians = RhinoMath.ToRadians(angleDegrees)
        let rc = Vector3d(vector.X, vector.Y, vector.Z)
        if rc.Rotate(angleradians, axis) then rc
        else RhinoScriptingException.Raise "RhinoScriptSyntax.VectorRotate failed on %A, %A, %A" vector angleDegrees axis


    ///<summary>Scales a 3-D vector.</summary>
    ///<param name="vector">(Vector3d) The vector to scale</param>
    ///<param name="scale">(float) Scale factor to apply</param>
    ///<returns>(Vector3d) resulting vector.</returns>
    static member VectorScale(vector:Vector3d, scale:float) : Vector3d = 
        vector*scale


    ///<summary>Subtracts two 3D vectors.</summary>
    ///<param name="vector1">(Vector3d) The vector to subtract from</param>
    ///<param name="vector2">(Vector3d) The vector to subtract</param>
    ///<returns>(Vector3d) The resulting 3D vector.</returns>
    static member VectorSubtract(vector1:Vector3d, vector2:Vector3d) : Vector3d = 
        vector1-vector2


    ///<summary>Transforms a 3D vector.</summary>
    ///<param name="vector">(Vector3d) The vector to transform</param>
    ///<param name="xForm">(Transform) A valid 4x4 transformation matrix</param>
    ///<returns>(Vector3d) transformed vector.</returns>
    static member VectorTransform(vector:Vector3d, xForm:Transform) : Vector3d =         
        let v = Vector3d(vector)
        v.Transform(xForm)
        v


    ///<summary>Unitizes, or normalizes a 3D vector. Note, zero vectors cannot be unitized.</summary>
    ///<param name="vector">(Vector3d) The vector to unitize</param>
    ///<returns>(Vector3d) unitized vector.</returns>
    static member inline VectorUnitize(vector:Vector3d) : Vector3d = 
        let le = sqrt (vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z)
        if Double.IsInfinity le || le < RhinoMath.ZeroTolerance then RhinoScriptingException.Raise "RhinoScriptSyntax.VectorUnitize failed on zero length or very short Vector %s" vector.ToNiceString
        let f = 1. / le
        Vector3d(vector.X*f, vector.Y*f, vector.Z*f)


    ///<summary>Returns either a world axis-aligned or a construction Plane axis-aligned
    ///    bounding box of an array of 3-D point locations.</summary>
    ///<param name="points">(Point3d seq) A list of 3-D points</param>
    ///<param name="plane">(Plane) Optional, default value: <c>Plane.WorldXY</c>
    ///    Plane to which the bounding box should be aligned,
    ///    If omitted, a world axis-aligned bounding box
    ///    will be calculated</param>
    ///<returns>(Box) A Rhino.Geometry.Box.</returns>
    static member PointArrayBoundingBox( points:Point3d seq, [<OPT;DEF(Plane())>]plane:Plane) : Box = // TODO verify this works the same way as python !!
        if plane.IsValid then
            Box(plane, points)
        else
            Box(BoundingBox(points))



