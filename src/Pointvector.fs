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
        let vector1 = Coerce.coerce3dvector(vector1, true)
        let vector2 = Coerce.coerce3dvector(vector2, true)
        vector1.IsParallelTo(vector2)
    (*
    def IsVectorParallelTo(vector1, vector2):
        """Compares two vectors to see if they are parallel
        Parameters:
          vector1, vector2 (vector): the vectors to compare
        Returns:
        number: the value represents
                -1 = the vectors are anti-parallel
                 0 = the vectors are not parallel
                 1 = the vectors are parallel
        Example:
          import rhinoscriptsyntax as rs
          vector1 = (1,0,0)
          vector2 = (0,1,0)
          print rs.IsVectorParallelTo( vector1, vector2 )
        See Also:
          IsVectorPerpendicularTo
          IsVectorTiny
          IsVectorZero
        """
        vector1 = rhutil.coerce3dvector(vector1, True)
        vector2 = rhutil.coerce3dvector(vector2, True)
        return vector1.IsParallelTo(vector2)
    *)


    ///<summary>Compares two vectors to see if they are perpendicular</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to compare' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to compare' (FIXME 0)</param>
    ///<returns>(bool) True if vectors are perpendicular, otherwise False</returns>
    static member IsVectorPerpendicularTo(vector1:Vector3d, vector2:Vector3d) : bool =
        let vector1 = Coerce.coerce3dvector(vector1, true)
        let vector2 = Coerce.coerce3dvector(vector2, true)
        vector1.IsPerpendicularTo(vector2)
    (*
    def IsVectorPerpendicularTo(vector1, vector2):
        """Compares two vectors to see if they are perpendicular
        Parameters:
          vector1, vector2 (vector): the vectors to compare
        Returns:
          bool: True if vectors are perpendicular, otherwise False
        Example:
          import rhinoscriptsyntax as rs
          vector1 = (1,0,0)
          vector2 = (0,1,0)
          print rs.IsVectorPerpendicularTo( vector1, vector2 )
        See Also:
          IsVectorParallelTo
          IsVectorTiny
          IsVectorZero
        """
        vector1 = rhutil.coerce3dvector(vector1, True)
        vector2 = rhutil.coerce3dvector(vector2, True)
        return vector1.IsPerpendicularTo(vector2)
    *)


    ///<summary>Verifies that a vector is very short. The X,Y,Z elements are <= 1.0e-12</summary>
    ///<param name="vector">(Vector3d) The vector to check</param>
    ///<returns>(bool) True if the vector is tiny, otherwise False</returns>
    static member IsVectorTiny(vector:Vector3d) : bool =
        let vector = Coerce.coerce3dvector(vector, true)
        vector.IsTiny( 1.0e-12 )
    (*
    def IsVectorTiny(vector):
        """Verifies that a vector is very short. The X,Y,Z elements are <= 1.0e-12
        Parameters:
          vector (vector): the vector to check
        Returns:
          bool: True if the vector is tiny, otherwise False
        Example:
          import rhinoscriptsyntax as rs
          pt1 = rs.GetPoint("First point")
          pt2 = rs.GetPoint("Next point")
          vector = pt2 - pt1
          if rs.IsVectorTiny(vector):
              print "The vector is tiny."
          else:
              print "The vector is not tiny."
        See Also:
          IsVectorZero
          VectorCreate
        """
        vector = rhutil.coerce3dvector(vector, True)
        return vector.IsTiny( 1.0e-12 )
    *)


    ///<summary>Verifies that a vector is zero, or tiny. The X,Y,Z elements are equal to 0.0</summary>
    ///<param name="vector">(Vector3d) The vector to check</param>
    ///<returns>(bool) True if the vector is zero, otherwise False</returns>
    static member IsVectorZero(vector:Vector3d) : bool =
        let vector = Coerce.coerce3dvector(vector, true)
        vector.IsZero
    (*
    def IsVectorZero(vector):
        """Verifies that a vector is zero, or tiny. The X,Y,Z elements are equal to 0.0
        Parameters:
          vector (vector): the vector to check
        Returns:
          bool: True if the vector is zero, otherwise False
        Example:
          import rhinoscriptsyntax as rs
          pt1 = rs.GetPoint("First point")
          pt2 = rs.GetPoint("Next point")
          vector = pt2 - pt1
          if rs.IsVectorZero(vector):
              print "The vector is zero."
          else:
              print "The vector is not zero."
        See Also:
          IsVectorTiny
          VectorCreate
        """
        vector = rhutil.coerce3dvector(vector, True)
        return vector.IsZero
    *)


    ///<summary>Adds a 3D point or a 3D vector to a 3D point</summary>
    ///<param name="point1">(Point3d) Point1 of 'the points to add' (FIXME 0)</param>
    ///<param name="point2">(Point3d) Point2 of 'the points to add' (FIXME 0)</param>
    ///<returns>(Point3d) the resulting 3D point</returns>
    static member PointAdd(point1:Point3d, point2:Point3d) : Point3d =
        let point1 = Coerce.coerce3dpoint(point1, true)
        let point2 = Coerce.coerce3dpoint(point2, true)
        point1+point2
    (*
    def PointAdd(point1, point2):
        """Adds a 3D point or a 3D vector to a 3D point
        Parameters:
          point1, point2 (point): the points to add
        Returns:
          point: the resulting 3D point if successful
        Example:
          import rhinoscriptsyntax as rs
          point1 = (1,1,1)
          point2 = (2,2,2)
          point = rs.PointAdd(point1, point2)
          print point
        See Also:
          PointCompare
          PointDivide
          PointScale
          PointSubtract
          PointTransform
        """
        point1 = rhutil.coerce3dpoint(point1, True)
        point2 = rhutil.coerce3dpoint(point2, True)
        return point1+point2
    *)


    ///<summary>Finds the point in a list of 3D points that is closest to a test point</summary>
    ///<param name="points">(Point3d seq) List of points</param>
    ///<param name="testPoint">(Point3d) The point to compare against</param>
    ///<returns>(float) index of the element in the point list that is closest to the test point</returns>
    static member PointArrayClosestPoint(points:Point3d seq, testPoint:Point3d) : float =
        let points = Coerce.coerce3dpointlist(points, true)
        let testPoint = Coerce.coerce3dpoint(testPoint, true)
        let index = Rhino.Collections.Point3dList.ClosestIndexInList(points, testPoint)
        if index>=0 then index
    (*
    def PointArrayClosestPoint(points, test_point):
        """Finds the point in a list of 3D points that is closest to a test point
        Parameters:
          points ([point, ...]): list of points
          test_point (point): the point to compare against
        Returns:
          number: index of the element in the point list that is closest to the test point
        Example:
          import rhinoscriptsyntax as rs
          cloud = rs.GetObject("Select point cloud")
          if cloud:
              point = rs.GetPoint("Point to test")
              if point:
                  cloud = rs.PointCloudPoints(cloud)
                  index = rs.PointArrayClosestPoint(cloud, point)
                  if index is not None:
                      point_id = rs.AddPoint(cloud[index])
                      rs.SelectObject( point_id )
        See Also:
          CurveClosestPoint
          SurfaceClosestPoint
        """
        points = rhutil.coerce3dpointlist(points, True)
        test_point = rhutil.coerce3dpoint(test_point, True)
        index = Rhino.Collections.Point3dList.ClosestIndexInList(points, test_point)
        if index>=0: return index
    *)


    ///<summary>Transforms a list of 3D points</summary>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<param name="xform">(Transform) Transformation to apply</param>
    ///<returns>(Point3d seq) transformed points on success</returns>
    static member PointArrayTransform(points:Point3d seq, xform:Transform) : Point3d seq =
        let points = Coerce.coerce3dpointlist(points, true)
        let xform = Coerce.coercexform(xform, true)
        [| for point in points -> xform*point |]
    (*
    def PointArrayTransform(points, xform):
        """Transforms a list of 3D points
        Parameters:
          points ([point, ...]): list of 3D points
          xform (transform): transformation to apply
        Returns:
          list(point, ...): transformed points on success
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          points = rs.BoundingBox(obj)
          xform = rs.XformRotation2(45.0, (0,0,1), (0,0,0))
          points = rs.PointArrayTransform(points, xform)
          rs.AddPoints(points)
        See Also:
          PointArrayClosestPoint
        """
        points = rhutil.coerce3dpointlist(points, True)
        xform = rhutil.coercexform(xform, True)
        return [xform*point for point in points]
    *)


    ///<summary>Finds the object that is closest to a test point</summary>
    ///<param name="point">(Point3d) Point to test</param>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more objects</param>
    ///<returns>(Guid * Point3d) closest [0] object_id and [1] point on object on success</returns>
    static member PointClosestObject(point:Point3d, objectIds:Guid seq) : Guid * Point3d =
        let objectIds = Coerce.coerceguidlist(objectIds)
        let point = Coerce.coerce3dpoint(point, true)
        let closest = null
        for id in objectIds do
            let geom = Coerce.coercegeometry(id, true)
            let point_geometry = geom
            if isinstance(point_geometry, Point) then
                let distance = point.DistanceTo( point_geometry.Location )
                if closest = null || distance<closest.[0] then
                    closest <- distance, id, point_geometry.Location
                continue
            let point_cloud = geom
            if isinstance(point_cloud, PointCloud) then
                let index = point_cloud.ClosestPoint(point)
                if index>=0 then
                    distance <- point.DistanceTo( point_cloud.[index].Location )
                    if closest = null || distance<closest.[0] then
                        closest <- distance, id, point_cloud.[index].Location
                continue
            let curve = geom
            if isinstance(curve, Curve) then
                let rc, t = curve.ClosestPoint(point)
                if rc then
                    distance <- point.DistanceTo( curve.PointAt(t) )
                    if closest = null || distance<closest.[0] then
                        closest <- distance, id, curve.PointAt(t)
                continue
            let brep = geom
            if isinstance(brep, Brep) then
                let brep_closest = brep.ClosestPoint(point)
                distance <- point.DistanceTo( brep_closest )
                if closest = null || distance<closest.[0] then
                    closest <- distance, id, brep_closest
                continue
            let mesh = geom
            if isinstance(mesh, Mesh) then
                let mesh_closest = mesh.ClosestPoint(point)
                distance <- point.DistanceTo( mesh_closest )
                if closest = null || distance<closest.[0] then
                    closest <- distance, id, mesh_closest
                continue
        if closest then closest.[1], closest.[2]
    (*
    def PointClosestObject(point, object_ids):
        """Finds the object that is closest to a test point
        Parameters:
          point (point): point to test
          object_ids ([guid, ...]): identifiers of one or more objects
        Returns:
          list(guid, point): closest [0] object_id and [1] point on object on success
          None: on failure
        Example:
          import rhinoscriptsyntax as rs
          objs = rs.GetObjects("Select target objects for closest point", 63)
          if objs:
              point = rs.GetPoint("Test point")
              if point:
                  results = rs.PointClosestObject(point, objs)
                  if results:
                      print "Object id:", results[0]
                      rs.AddPoint( results[1] )
        See Also:
          CurveClosestObject
        """
        object_ids = rhutil.coerceguidlist(object_ids)
        point = rhutil.coerce3dpoint(point, True)
        closest = None
        for id in object_ids:
            geom = rhutil.coercegeometry(id, True)
            point_geometry = geom
            if isinstance(point_geometry, Rhino.Geometry.Point):
                distance = point.DistanceTo( point_geometry.Location )
                if closest is None or distance<closest[0]:
                    closest = distance, id, point_geometry.Location
                continue
            point_cloud = geom
            if isinstance(point_cloud, Rhino.Geometry.PointCloud):
                index = point_cloud.ClosestPoint(point)
                if index>=0:
                    distance = point.DistanceTo( point_cloud[index].Location )
                    if closest is None or distance<closest[0]:
                        closest = distance, id, point_cloud[index].Location
                continue
            curve = geom
            if isinstance(curve, Rhino.Geometry.Curve):
                rc, t = curve.ClosestPoint(point)
                if rc:
                    distance = point.DistanceTo( curve.PointAt(t) )
                    if closest is None or distance<closest[0]:
                        closest = distance, id, curve.PointAt(t)
                continue
            brep = geom
            if isinstance(brep, Rhino.Geometry.Brep):
                brep_closest = brep.ClosestPoint(point)
                distance = point.DistanceTo( brep_closest )
                if closest is None or distance<closest[0]:
                    closest = distance, id, brep_closest
                continue
            mesh = geom
            if isinstance(mesh, Rhino.Geometry.Mesh):
                mesh_closest = mesh.ClosestPoint(point)
                distance = point.DistanceTo( mesh_closest )
                if closest is None or distance<closest[0]:
                    closest = distance, id, mesh_closest
                continue
        if closest: return closest[1], closest[2]
    *)


    ///<summary>Compares two 3D points</summary>
    ///<param name="point1">(Point3d) Point1 of 'the points to compare' (FIXME 0)</param>
    ///<param name="point2">(Point3d) Point2 of 'the points to compare' (FIXME 0)</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///Tolerance to use for comparison. If omitted,
    ///  Rhino's internal zero tolerance is used</param>
    ///<returns>(bool) True or False</returns>
    static member PointCompare(point1:Point3d, point2:Point3d, [<OPT;DEF(null)>]tolerance:float) : bool =
        let point1 = Coerce.coerce3dpoint(point1, true)
        let point2 = Coerce.coerce3dpoint(point2, true)
        if tolerance = null then tolerance <- Rhino.RhinoMath.ZeroTolerance
        let vector = point2-point1
        vector.IsTiny(tolerance)
    (*
    def PointCompare(point1, point2, tolerance=None):
        """Compares two 3D points
        Parameters:
          point1, point2 (point): the points to compare
          tolerance (number, optional): tolerance to use for comparison. If omitted,
                                        Rhino's internal zero tolerance is used
        Returns:
          bool: True or False
        Example:
          import rhinoscriptsyntax as rs
          point1 = (1,1,1)
          point2 = (2,2,2)
          print rs.PointCompare(point1, point2)
        See Also:
          PointAdd
          PointDivide
          PointScale
          PointSubtract
          PointTransform
        """
        point1 = rhutil.coerce3dpoint(point1, True)
        point2 = rhutil.coerce3dpoint(point2, True)
        if tolerance is None: tolerance = Rhino.RhinoMath.ZeroTolerance
        vector = point2-point1
        return vector.IsTiny(tolerance)
    *)


    ///<summary>Divides a 3D point by a value</summary>
    ///<param name="point">(Point3d) The point to divide</param>
    ///<param name="divide">(int) A non-zero value to divide</param>
    ///<returns>(Point3d) resulting point</returns>
    static member PointDivide(point:Point3d, divide:int) : Point3d =
        let point = Coerce.coerce3dpoint(point, true)
        point/divide
    (*
    def PointDivide(point, divide):
        """Divides a 3D point by a value
        Parameters:
          point (point): the point to divide
          divide (number): a non-zero value to divide
        Returns:
          point: resulting point
        Example:
          import rhinoscriptsyntax as rs
          point = rs.PointDivide([5,5,0], 5)
          print point
        See Also:
          PointAdd
          PointCompare
          PointScale
          PointSubtract
          PointTransform
        """
        point = rhutil.coerce3dpoint(point, True)
        return point/divide
    *)


    ///<summary>Verifies that a list of 3D points are coplanar</summary>
    ///<param name="points">(Point3d seq) 3D points to test</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>1.0e-12</c>
    ///Tolerance to use when verifying</param>
    ///<returns>(bool) True or False</returns>
    static member PointsAreCoplanar(points:Point3d seq, [<OPT;DEF(1.0e-12)>]tolerance:float) : bool =
        let points = Coerce.coerce3dpointlist(points, true)
        Point3d.ArePointsCoplanar(points, tolerance)
    (*
    def PointsAreCoplanar(points, tolerance=1.0e-12):
        """Verifies that a list of 3D points are coplanar
        Parameters:
          points ([point, ...]): 3D points to test
          tolerance (number, optional): tolerance to use when verifying
        Returns:
          bool: True or False
        Example:
          import rhinoscriptsyntax as rs
          def SurfacesAreCoplanar(srf1, srf2):
              if( not rs.IsSurface(srf1) or not rs.IsSurface(srf2) ): return False
              if( not rs.IsSurfacePlanar(srf1) or not rs.IsSurfacePlanar(srf2) ): return False
              pts1 = rs.SurfacePoints(srf1)
              pts2 = rs.SurfacePoints(srf2)
              if( pts1==None or pts2==None ): return False
              pts1.extend(pts2)
              return rs.PointsAreCoplanar(pts1)
           
          x = rs.GetObject( "First surface to test", rs.filter.surface)
          y = rs.GetObject( "Second surface to test", rs.filter.surface)
          print SurfacesAreCoplanar(x, y)
        See Also:
          IsPoint
          IsPointCloud
          PointCoordinates
        """
        points = rhutil.coerce3dpointlist(points, True)
        return Rhino.Geometry.Point3d.ArePointsCoplanar(points, tolerance)
    *)


    ///<summary>Scales a 3D point by a value</summary>
    ///<param name="point">(Point3d) The point to divide</param>
    ///<param name="scale">(float) Scale factor to apply</param>
    ///<returns>(Point3d) resulting point on success</returns>
    static member PointScale(point:Point3d, scale:float) : Point3d =
        let point = Coerce.coerce3dpoint(point, true)
        point*scale
    (*
    def PointScale(point, scale):
        """Scales a 3D point by a value
        Parameters:
          point (point): the point to divide
          scale (number): scale factor to apply
        Returns:
          point: resulting point on success
        Example:
          import rhinoscriptsyntax as rs
          point = rs.PointScale([1,0,0], 5)
          print point
        See Also:
          PointAdd
          PointCompare
          PointDivide
          PointSubtract
          PointTransform
        """
        point = rhutil.coerce3dpoint(point, True)
        return point*scale
    *)


    ///<summary>Subtracts a 3D point or a 3D vector from a 3D point</summary>
    ///<param name="point1">(Point3d) Point1 of 'the points to subtract' (FIXME 0)</param>
    ///<param name="point2">(Point3d) Point2 of 'the points to subtract' (FIXME 0)</param>
    ///<returns>(Point3d) the resulting 3D point</returns>
    static member PointSubtract(point1:Point3d, point2:Point3d) : Point3d =
        let point1 = Coerce.coerce3dpoint(point1, true)
        let point2 = Coerce.coerce3dpoint(point2, true)
        let v = point1-point2
        Point3d(v)
    (*
    def PointSubtract(point1, point2):
        """Subtracts a 3D point or a 3D vector from a 3D point
        Parameters:
          point1, point2 (point): the points to subtract
        Returns:
          point: the resulting 3D point if successful
        Example:
          import rhinoscriptsyntax as rs
          point1 = (1,1,1)
          point2 = (2,2,2)
          point = rs.PointSubtract(point1, point2)
          print point
        See Also:
          PointAdd
          PointCompare
          PointDivide
          PointScale
          PointTransform
        """
        point1 = rhutil.coerce3dpoint(point1, True)
        point2 = rhutil.coerce3dpoint(point2, True)
        v = point1-point2
        return Rhino.Geometry.Point3d(v)
    *)


    ///<summary>Transforms a 3D point</summary>
    ///<param name="point">(Point3d) The point to transform</param>
    ///<param name="xform">(Transform) A valid 4x4 transformation matrix</param>
    ///<returns>(Vector3d) transformed vector on success</returns>
    static member PointTransform(point:Point3d, xform:Transform) : Vector3d =
        let point = Coerce.coerce3dpoint(point, true)
        let xform = Coerce.coercexform(xform, true)
        xform*point
    (*
    def PointTransform(point, xform):
        """Transforms a 3D point
        Parameters:
          point (point): the point to transform
          xform (transform): a valid 4x4 transformation matrix
        Returns:
          vector: transformed vector on success
        Example:
          # Translate (move) objects by (10,10,0)
          import rhinoscriptsyntax as rs
          point = 5,5,0
          matrix = rs.XformTranslation((10,10,0))
          result = rs.PointTransform(point, matrix)
          print result
        See Also:
          PointAdd
          PointCompare
          PointDivide
          PointScale
          PointSubtract
        """
        point = rhutil.coerce3dpoint(point, True)
        xform = rhutil.coercexform(xform, True)
        return xform*point
    *)


    ///<summary>Projects one or more points onto one or more meshes</summary>
    ///<param name="points">(Point3d seq) One or more 3D points</param>
    ///<param name="meshIds">(Guid seq) Identifiers of one or more meshes</param>
    ///<param name="direction">(Vector3d) Direction vector to project the points</param>
    ///<returns>(Point3d seq) projected points on success</returns>
    static member ProjectPointToMesh(points:Point3d seq, meshIds:Guid seq, direction:Vector3d) : Point3d seq =
        let pts = Coerce.coerce3dpointlist(points, false)
        if pts = null then
            pts <- .[Coerce.coerce3dpoint(points, true)]
        let direction = Coerce.coerce3dvector(direction, true)
        let id = Coerce.coerceguid(meshIds, false)
        if id then meshIds <- .[id]
        let meshes = [| for id in meshIds -> Coerce.coercemesh(id, true) |]
        let tolerance = Doc.ModelAbsoluteTolerance
        let rc = Intersect.Intersection.ProjectPointsToMeshes(meshes, pts, direction, tolerance)
        rc
    (*
    def ProjectPointToMesh(points, mesh_ids, direction):
        """Projects one or more points onto one or more meshes
        Parameters:
          points ([point, ...]): one or more 3D points
          mesh_ids ([guid, ...]): identifiers of one or more meshes
          direction (vector): direction vector to project the points
        Returns:
         list(point, ...): projected points on success
        Example:
          import rhinoscriptsyntax as rs
          mesh = rs.GetObject("Select mesh to project onto", rs.filter.mesh)
          objects = rs.GetObjects("Select points to project", rs.filter.point)
          points = [rs.PointCoordinates(obj) for obj in objects]
          # project down...
          results = rs.ProjectPointToMesh(points, mesh, (0,0,-1))
          rs.AddPoints( results )
        See Also:
          ProjectCurveToMesh
          ProjectCurveToSurface
          ProjectPointToSurface
        """
        pts = rhutil.coerce3dpointlist(points, False)
        if pts is None:
            pts = [rhutil.coerce3dpoint(points, True)]
        direction = rhutil.coerce3dvector(direction, True)
        id = rhutil.coerceguid(mesh_ids, False)
        if id: mesh_ids = [id]
        meshes = [rhutil.coercemesh(id, True) for id in mesh_ids]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        rc = Rhino.Geometry.Intersect.Intersection.ProjectPointsToMeshes(meshes, pts, direction, tolerance)
        return rc
    *)


    ///<summary>Projects one or more points onto one or more surfaces or polysurfaces</summary>
    ///<param name="points">(Point3d seq) One or more 3D points</param>
    ///<param name="surfaceIds">(Guid seq) Identifiers of one or more surfaces/polysurfaces</param>
    ///<param name="direction">(Vector3d) Direction vector to project the points</param>
    ///<returns>(Point3d seq) projected points on success</returns>
    static member ProjectPointToSurface(points:Point3d seq, surfaceIds:Guid seq, direction:Vector3d) : Point3d seq =
        let pts = Coerce.coerce3dpointlist(points)
        if pts = null then
            pts <- .[Coerce.coerce3dpoint(points, true)]
        let direction = Coerce.coerce3dvector(direction, true)
        let id = Coerce.coerceguid(surfaceIds, false)
        if id then surfaceIds <- .[id]
        let breps = [| for id in surfaceIds -> Coerce.coercebrep(id, true) |]
        let tolerance = Doc.ModelAbsoluteTolerance
        Intersect.Intersection.ProjectPointsToBreps(breps, pts, direction, tolerance)
    (*
    def ProjectPointToSurface(points, surface_ids, direction):
        """Projects one or more points onto one or more surfaces or polysurfaces
        Parameters:
          points ([point, ...]): one or more 3D points
          surface_ids ([guid, ...]): identifiers of one or more surfaces/polysurfaces
          direction (vector): direction vector to project the points
        Returns:
         list(point, ...): projected points on success
        Example:
          import rhinoscriptsyntax as rs
          surface = rs.GetObject("Select surface to project onto", rs.filter.surface)
          objects = rs.GetObjects("Select points to project", rs.filter.point)
          points = [rs.PointCoordinates(obj) for obj in objects]
          # Project down...
          results = rs.ProjectPointToSurface(points, surface, (0,0,-1))
          rs.AddPoints(results)
        See Also:
          ProjectCurveToMesh
          ProjectCurveToSurface
          ProjectPointToMesh
        """
        pts = rhutil.coerce3dpointlist(points)
        if pts is None:
            pts = [rhutil.coerce3dpoint(points, True)]
        direction = rhutil.coerce3dvector(direction, True)
        id = rhutil.coerceguid(surface_ids, False)
        if id: surface_ids = [id]
        breps = [rhutil.coercebrep(id, True) for id in surface_ids]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        return Rhino.Geometry.Intersect.Intersection.ProjectPointsToBreps(breps, pts, direction, tolerance)
    *)


    ///<summary>Pulls an array of points to a surface or mesh object. For more
    ///  information, see the Rhino help file Pull command</summary>
    ///<param name="objectId">(Guid) The identifier of the surface or mesh object that pulls</param>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<returns>(Point3d seq) 3D points pulled onto surface or mesh</returns>
    static member PullPoints(objectId:Guid, points:Point3d seq) : Point3d seq =
        let id = Coerce.coerceguid(objectId, true)
        let points = Coerce.coerce3dpointlist(points, true)
        let mesh = Coerce.coercemesh(id, false)
        if mesh then
            points <- mesh.PullPointsToMesh(points)
            list(points)
        let brep = Coerce.coercebrep(id, false)
        if brep && brep.Faces.Count=1 then
            let tolerance = Doc.ModelAbsoluteTolerance
            points <- brep.Faces.[0].PullPointsToFace(points, tolerance)
            list(points)
        []
    (*
    def PullPoints(object_id, points):
        """Pulls an array of points to a surface or mesh object. For more
        information, see the Rhino help file Pull command
        Parameters:
          object_id (guid): the identifier of the surface or mesh object that pulls
          points ([point, ...]): list of 3D points
        Returns:
          list(point, ...): 3D points pulled onto surface or mesh
        Example:
          import rhinoscriptsyntax as rs
          surface = rs.GetObject("Select surface that pulls", rs.filter.surface)
          objects = rs.GetObjects("Select points to pull", rs.filter.point)
          points = [rs.PointCoordinates(obj) for obj in objects]
          results = rs.PullPoints( surface, points )
          rs.AddPoints( results )
        See Also:
          PullCurve
        """
        id = rhutil.coerceguid(object_id, True)
        points = rhutil.coerce3dpointlist(points, True)
        mesh = rhutil.coercemesh(id, False)
        if mesh:
            points = mesh.PullPointsToMesh(points)
            return list(points)
        brep = rhutil.coercebrep(id, False)
        if brep and brep.Faces.Count==1:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
            points = brep.Faces[0].PullPointsToFace(points, tolerance)
            return list(points)
        return []
    *)


    ///<summary>Adds two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to add' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to add' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting 3D vector</returns>
    static member VectorAdd(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        let vector1 = Coerce.coerce3dvector(vector1, true)
        let vector2 = Coerce.coerce3dvector(vector2, true)
        vector1+vector2
    (*
    def VectorAdd(vector1, vector2):
        """Adds two 3D vectors
        Parameters:
          vector1, vector2 (vector): the vectors to add
        Returns:
          vector: the resulting 3D vector if successful
        Example:
          import rhinoscriptsyntax as rs
          vector1 = (1,0,0)
          vector2 = (0,1,0)
          vector = rs.VectorAdd(vector1, vector2)
          print vector
        See Also:
          VectorCreate
          VectorScale
          VectorSubtract
        """
        vector1 = rhutil.coerce3dvector(vector1, True)
        vector2 = rhutil.coerce3dvector(vector2, True)
        return vector1+vector2
    *)


    ///<summary>Returns the angle, in degrees, between two 3-D vectors</summary>
    ///<param name="vector1">(Vector3d) The first 3-D vector.</param>
    ///<param name="vector2">(Vector3d) The second 3-D vector.</param>
    ///<returns>(float) The angle in degrees</returns>
    static member VectorAngle(vector1:Vector3d, vector2:Vector3d) : float =
        let vector1 = Coerce.coerce3dvector(vector1, true)
        let vector2 = Coerce.coerce3dvector(vector2, true)
        vector1 <- Vector3d(vector1.X, vector1.Y, vector1.Z)
        vector2 <- Vector3d(vector2.X, vector2.Y, vector2.Z)
        if not <| vector1.Unitize() |> ignore || not <| vector2.Unitize() |> ignore then
            failwithf "Rhino.Scripting Error:Unable to unitize vector.  vector1:"%A" vector2:"%A"" vector1 vector2
        let dot = vector1 * vector2
        dot <- Coerce.clamp(-1,1,dot)
        let radians = Math.Acos(dot)
        toDegrees(radians)
    (*
    def VectorAngle(vector1, vector2):
        """Returns the angle, in degrees, between two 3-D vectors
        Parameters:
          vector1 (vector): The first 3-D vector.
          vector2 (vector): The second 3-D vector.
        Returns:
          number: The angle in degrees if successful
          None: if not successful
        Example:
          import rhinoscriptsyntax as rs
          s0 = rs.GetObject("Surface 0", rs.filter.surface)
          s1 = rs.GetObject("Surface 1", rs.filter.surface)
          du0 = rs.SurfaceDomain(s0, 0)
          dv0 = rs.SurfaceDomain(s0, 1)
          du1 = rs.SurfaceDomain(s1, 0)
          dv1 = rs.SurfaceDomain(s1, 1)
          n0 = rs.SurfaceNormal(s0, (du0[0], dv0[0]))
          n1 = rs.SurfaceNormal(s1, (du1[0], dv1[0]))
          print rs.VectorAngle(n0, n1)
          print rs.VectorAngle(n0, rs.VectorReverse(n1))
        See Also:
          Angle
          Angle2
        """
        vector1 = rhutil.coerce3dvector(vector1, True)
        vector2 = rhutil.coerce3dvector(vector2, True)
        vector1 = Rhino.Geometry.Vector3d(vector1.X, vector1.Y, vector1.Z)
        vector2 = Rhino.Geometry.Vector3d(vector2.X, vector2.Y, vector2.Z)
        if not vector1.Unitize() or not vector2.Unitize():
            raise ValueError("unable to unitize vector")
        dot = vector1 * vector2
        dot = rhutil.clamp(-1,1,dot)
        radians = math.acos(dot)
        return math.degrees(radians)
    *)


    ///<summary>Compares two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the two vectors to compare' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the two vectors to compare' (FIXME 0)</param>
    ///<returns>(float) result of comparing the vectors.
    ///  -1 if vector1 is less than vector2
    ///  0 if vector1 is equal to vector2
    ///  1 if vector1 is greater than vector2</returns>
    static member VectorCompare(vector1:Vector3d, vector2:Vector3d) : float =
        let vector1 = Coerce.coerce3dvector(vector1, true)
        let vector2 = Coerce.coerce3dvector(vector2, true)
        vector1.CompareTo(vector2)
    (*
    def VectorCompare(vector1, vector2):
        """Compares two 3D vectors
        Parameters:
          vector1, vector2 (vector): the two vectors to compare
        Returns:
          number: result of comparing the vectors.
                  -1 if vector1 is less than vector2
                  0 if vector1 is equal to vector2
                  1 if vector1 is greater than vector2
        Example:
          import rhinoscriptsyntax as rs
          vector1 = (1,0,0)
          vector2 = (0,1,0)
          rc = rs.VectorCompare(vector1 , vector2)
          print rc
        See Also:
          IsVectorTiny
          IsVectorZero
          VectorCreate
        """
        vector1 = rhutil.coerce3dvector(vector1, True)
        vector2 = rhutil.coerce3dvector(vector2, True)
        return vector1.CompareTo(vector2)
    *)


    ///<summary>Creates a vector from two 3D points</summary>
    ///<param name="toPoint">(Point3d) To point of 'the points defining the vector' (FIXME 0)</param>
    ///<param name="fromPoint">(Point3d) From point of 'the points defining the vector' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting vector</returns>
    static member VectorCreate(toPoint:Point3d, fromPoint:Point3d) : Vector3d =
        let to_point = Coerce.coerce3dpoint(to_point, true)
        let fromPoint = Coerce.coerce3dpoint(fromPoint, true)
        to_point-fromPoint
    (*
    def VectorCreate(to_point, from_point):
        """Creates a vector from two 3D points
        Parameters:
          to_point, from_point (point): the points defining the vector
        Returns:
          vector: the resulting vector if successful
        Example:
          import rhinoscriptsyntax as rs
          point1 = rs.GetPoint("First point")
          point2 = rs.GetPoint("Next point")
          vector = rs.VectorCreate(point2, point1)
          print vector
        See Also:
          IsVectorTiny
          IsVectorZero
          VectorCompare
          VectorUnitize
        """
        to_point = rhutil.coerce3dpoint(to_point, True)
        from_point = rhutil.coerce3dpoint(from_point, True)
        return to_point-from_point
    *)


    ///<summary>Calculates the cross product of two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to perform cross product on' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to perform cross product on' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting cross product direction</returns>
    static member VectorCrossProduct(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        let vector1 = Coerce.coerce3dvector(vector1, true)
        let vector2 = Coerce.coerce3dvector(vector2, true)
        Vector3d.CrossProduct( vector1, vector2 )
    (*
    def VectorCrossProduct(vector1, vector2):
        """Calculates the cross product of two 3D vectors
        Parameters:
          vector1, vector2 (vector): the vectors to perform cross product on
        Returns:
          vector: the resulting cross product direction if successful
        Example:
          import rhinoscriptsyntax as rs
          vector1 = (1,0,0)
          vector2 = (0,1,0)
          vector = rs.VectorCrossProduct(vector1, vector2)
          print vector
        See Also:
          VectorDotProduct
          VectorUnitize
        """
        vector1 = rhutil.coerce3dvector(vector1, True)
        vector2 = rhutil.coerce3dvector(vector2, True)
        return Rhino.Geometry.Vector3d.CrossProduct( vector1, vector2 )
    *)


    ///<summary>Divides a 3D vector by a value</summary>
    ///<param name="vector">(Vector3d) The vector to divide</param>
    ///<param name="divide">(int) A non-zero value to divide</param>
    ///<returns>(Vector3d) resulting vector on success</returns>
    static member VectorDivide(vector:Vector3d, divide:int) : Vector3d =
        let vector = Coerce.coerce3dvector(vector, true)
        vector/divide
    (*
    def VectorDivide(vector, divide):
        """Divides a 3D vector by a value
        Parameters:
          vector (vector): the vector to divide
          divide (number): a non-zero value to divide
        Returns:
          vector: resulting vector on success
        Example:
          import rhinoscriptsyntax as rs
          vector = rs.VectorDivide((5,5,0), 5)
          print vector
        See Also:
          VectorAdd
          VectorCreate
          VectorSubtract
        """
        vector = rhutil.coerce3dvector(vector, True)
        return vector/divide
    *)


    ///<summary>Calculates the dot product of two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to perform the dot product on' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to perform the dot product on' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting dot product</returns>
    static member VectorDotProduct(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        let vector1 = Coerce.coerce3dvector(vector1, true)
        let vector2 = Coerce.coerce3dvector(vector2, true)
        vector1*vector2
    (*
    def VectorDotProduct(vector1, vector2):
        """Calculates the dot product of two 3D vectors
        Parameters:
          vector1, vector2 (vector): the vectors to perform the dot product on
        Returns:
          vector: the resulting dot product if successful
        Example:
          import rhinoscriptsyntax as rs
          vector1 = [1,0,0]
          vector2 = [0,1,0]
          dblDotProduct = rs.VectorDotProduct(vector1, vector2)
          print dblDotProduct
        See Also:
          VectorCrossProduct
          VectorUnitize
        """
        vector1 = rhutil.coerce3dvector(vector1, True)
        vector2 = rhutil.coerce3dvector(vector2, True)
        return vector1*vector2
    *)


    ///<summary>Returns the length of a 3D vector</summary>
    ///<param name="vector">(Vector3d) The 3-D vector.</param>
    ///<returns>(float) The length of the vector , otherwise None</returns>
    static member VectorLength(vector:Vector3d) : float =
        let vector = Coerce.coerce3dvector(vector, true)
        vector.Length
    (*
    def VectorLength(vector):
        """Returns the length of a 3D vector
        Parameters:
          vector (vector):  The 3-D vector.
        Returns:
          number: The length of the vector if successful, otherwise None
        Example:
          import rhinoscriptsyntax as rs
          point1 = rs.GetPoint("First point")
          point2 = rs.GetPoint("Next point")
          vector = rs.VectorCreate(point1, point2)
          print rs.VectorLength(vector)
        See Also:
          VectorAdd
          VectorCreate
          VectorSubtract
          VectorUnitize
        """
        vector = rhutil.coerce3dvector(vector, True)
        return vector.Length
    *)


    ///<summary>Multiplies two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) Vector1 of 'the vectors to multiply' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) Vector2 of 'the vectors to multiply' (FIXME 0)</param>
    ///<returns>(Vector3d) the resulting inner (dot) product</returns>
    static member VectorMultiply(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        VectorDotProduct(vector1, vector2)
    (*
    def VectorMultiply(vector1, vector2):
        """Multiplies two 3D vectors
        Parameters:
          vector1, vector2 (vector): the vectors to multiply
        Returns:
          vector: the resulting inner (dot) product if successful
        Example:
          import rhinoscriptsyntax as rs
          product = rs.VectorMultiply( [2,2,2], [3,3,3] )
          print product
        See Also:
          VectorAdd
          VectorCreate
          VectorSubtract
        """
        return VectorDotProduct(vector1, vector2)
    *)


    ///<summary>Reverses the direction of a 3D vector</summary>
    ///<param name="vector">(Vector3d) The vector to reverse</param>
    ///<returns>(Vector3d) reversed vector on success</returns>
    static member VectorReverse(vector:Vector3d) : Vector3d =
        let vector = Coerce.coerce3dvector(vector, true)
        let rc = Vector3d(vector.X, vector.Y, vector.Z)
        rc.Reverse()
        rc
    (*
    def VectorReverse(vector):
        """Reverses the direction of a 3D vector
        Parameters:
          vector (vector): the vector to reverse
        Returns:
          vector: reversed vector on success
        Example:
          import rhinoscriptsyntax as rs
          vector = rs.VectorReverse([1,0,0])
          print vector
        See Also:
          VectorCreate
          VectorUnitize
        """
        vector = rhutil.coerce3dvector(vector, True)
        rc = Rhino.Geometry.Vector3d(vector.X, vector.Y, vector.Z)
        rc.Reverse()
        return rc
    *)


    ///<summary>Rotates a 3D vector</summary>
    ///<param name="vector">(Vector3d) The vector to rotate</param>
    ///<param name="angleDegrees">(int) Rotation angle</param>
    ///<param name="axis">(Vector3d) Axis of rotation</param>
    ///<returns>(Vector3d) rotated vector on success</returns>
    static member VectorRotate(vector:Vector3d, angleDegrees:int, axis:Vector3d) : Vector3d =
        let vector = Coerce.coerce3dvector(vector, true)
        let axis = Coerce.coerce3dvector(axis, true)
        let angle_radians = Rhino.RhinoMath.ToRadians(angleDegrees)
        let rc = Vector3d(vector.X, vector.Y, vector.Z)
        if rc.Rotate(angle_radians, axis) then rc
    (*
    def VectorRotate(vector, angle_degrees, axis):
        """Rotates a 3D vector
        Parameters:
          vector (vector): the vector to rotate
          angle_degrees (number): rotation angle
          axis (vector): axis of rotation
        Returns:
          vector: rotated vector on success
        Example:
          import rhinoscriptsyntax as rs
          vector = rs.VectorRotate([1,0,0], 90.0, [0,0,1])
          print vector
        See Also:
          VectorCreate
          VectorScale
        """
        vector = rhutil.coerce3dvector(vector, True)
        axis = rhutil.coerce3dvector(axis, True)
        angle_radians = Rhino.RhinoMath.ToRadians(angle_degrees)
        rc = Rhino.Geometry.Vector3d(vector.X, vector.Y, vector.Z)
        if rc.Rotate(angle_radians, axis): return rc
    *)


    ///<summary>Scales a 3-D vector</summary>
    ///<param name="vector">(Vector3d) The vector to scale</param>
    ///<param name="scale">(float) Scale factor to apply</param>
    ///<returns>(Vector3d) resulting vector on success</returns>
    static member VectorScale(vector:Vector3d, scale:float) : Vector3d =
        let vector = Coerce.coerce3dvector(vector, true)
        vector*scale
    (*
    def VectorScale(vector, scale):
        """Scales a 3-D vector
        Parameters:
          vector (vector): the vector to scale
          scale (number): scale factor to apply
        Returns:
          vector: resulting vector on success
        Example:
          import rhinoscriptsyntax as rs
          vector = rs.VectorScale([1,0,0], 5)
          print vector
        See Also:
          VectorAdd
          VectorCreate
          VectorSubtract
        """
        vector = rhutil.coerce3dvector(vector, True)
        return vector*scale
    *)


    ///<summary>Subtracts two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) The vector to subtract from</param>
    ///<param name="vector2">(Vector3d) The vector to subtract</param>
    ///<returns>(Vector3d) the resulting 3D vector</returns>
    static member VectorSubtract(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        let vector1 = Coerce.coerce3dvector(vector1, true)
        let vector2 = Coerce.coerce3dvector(vector2, true)
        vector1-vector2
    (*
    def VectorSubtract(vector1, vector2):
        """Subtracts two 3D vectors
        Parameters:
          vector1 (vector): the vector to subtract from
          vector2 (vector): the vector to subtract
        Returns:
          vector: the resulting 3D vector
        Example:
          import rhinoscriptsyntax as rs
          vector1 = [1,0,0]
          vector2 = [0,1,0]
          vector = rs.VectorSubtract(vector1, vector2)
          print vector
        See Also:
          VectorAdd
          VectorCreate
          VectorScale
        """
        vector1 = rhutil.coerce3dvector(vector1, True)
        vector2 = rhutil.coerce3dvector(vector2, True)
        return vector1-vector2
    *)


    ///<summary>Transforms a 3D vector</summary>
    ///<param name="vector">(Vector3d) The vector to transform</param>
    ///<param name="xform">(Transform) A valid 4x4 transformation matrix</param>
    ///<returns>(Vector3d) transformed vector on success</returns>
    static member VectorTransform(vector:Vector3d, xform:Transform) : Vector3d =
        let vector = Coerce.coerce3dvector(vector, true)
        let xform = Coerce.coercexform(xform, true)
        xform*vector
    (*
    def VectorTransform(vector, xform):
        """Transforms a 3D vector
        Parameters:
          vector (vector): the vector to transform
          xform (transform): a valid 4x4 transformation matrix
        Returns:
          vector: transformed vector on success
        Example:
          import rhinoscriptsyntax as rs
          vector = (1,0,0) #world x-axis
          xform = rs.XformRotation2(90.0, (0,0,1), (0,0,0))
          vector = rs.VectorTransform(vector, xform)
          print vector
        See Also:
          IsVectorZero
          VectorCreate
          VectorUnitize
        """
        vector = rhutil.coerce3dvector(vector, True)
        xform = rhutil.coercexform(xform, True)
        return xform*vector
    *)


    ///<summary>Unitizes, or normalizes a 3D vector. Note, zero vectors cannot be unitized</summary>
    ///<param name="vector">(Vector3d) The vector to unitize</param>
    ///<returns>(unit) unitized vector on success</returns>
    static member VectorUnitize(vector:Vector3d) : unit =
        let vector = Coerce.coerce3dvector(vector, true)
        let rc = Vector3d(vector.X, vector.Y, vector.Z)
        if rc.Unitize() |> ignore then rc
    (*
    def VectorUnitize(vector):
        """Unitizes, or normalizes a 3D vector. Note, zero vectors cannot be unitized
        Parameters:
          vector (vector): the vector to unitize
        Returns:
          vector: unitized vector on success
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          vector = rs.VectorUnitize( [1.5,-4.1,3.6] )
          print vector
        See Also:
          IsVectorZero
          VectorCreate
        """
        vector = rhutil.coerce3dvector(vector, True)
        rc = Rhino.Geometry.Vector3d(vector.X, vector.Y, vector.Z)
        if rc.Unitize(): return rc
    *)


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
        let points = Coerce.coerce3dpointlist(points)
        if not <| points then
          null
        let bbox = BoundingBox(points)
        let xform = null
        let plane = Coerce.coerceplane(view_or_plane)
        if plane = null && view_or_plane then
            let view = view_or_plane
            let modelviews = Doc.Views.GetStandardRhinoViews()
            for item in modelviews do
                let viewport = item.MainViewport
                if typ(view) = string && viewport.Name=view then
                    plane <- viewport.ConstructionPlane()
                    break
                elif typ(view) = Guid && viewport.Id=view then
                    plane <- viewport.ConstructionPlane()
                    break
            if plane = null then failwithf "Rhino.Scripting Error:PointArrayBoundingBox failed.  points:"%A" viewOrPlane:"%A" inWorldCoords:"%A"" points viewOrPlane inWorldCoords
        if plane then
            xform <- Transform.ChangeBasis(Plane.WorldXY, plane)
            bbox <- xform.TransformBoundingBox(bbox)
        if not <| bbox.IsValid then failwithf "Rhino.Scripting Error:PointArrayBoundingBox failed.  points:"%A" viewOrPlane:"%A" inWorldCoords:"%A"" points viewOrPlane inWorldCoords
        let corners = list(bbox.GetCorners())
        if inWorldCoords && plane <> null then
            let plane_to_world = Transform.ChangeBasis(plane, Plane.WorldXY)
            for pt in corners do pt.Transform(plane_to_world)
        corners
    (*
    def PointArrayBoundingBox(points, view_or_plane=None, in_world_coords=True):
        """Returns either a world axis-aligned or a construction plane axis-aligned 
        bounding box of an array of 3-D point locations.
        Parameters:
          points ([point, ...]): A list of 3-D points
          view_or_plane (str|plane, optional): Title or id of the view that contains the
              construction plane to which the bounding box should be aligned -or-
              user defined plane. If omitted, a world axis-aligned bounding box
              will be calculated
          in_world_coords (bool, optional): return the bounding box as world coordinates or
              construction plane coordinates. Note, this option does not apply to
              world axis-aligned bounding boxes.
        Returns:
          list(point, ....): Eight points that define the bounding box. Points returned in counter-
          clockwise order starting with the bottom rectangle of the box.
          None: on error
        Example:
          
        See Also:
          BoundingBox
        """
        points = rhutil.coerce3dpointlist(points)
        if not points:
          return None
        bbox = Rhino.Geometry.BoundingBox(points)
        xform = None
        plane = rhutil.coerceplane(view_or_plane)
        if plane is None and view_or_plane:
            view = view_or_plane
            modelviews = scriptcontext.doc.Views.GetStandardRhinoViews()
            for item in modelviews:
                viewport = item.MainViewport
                if type(view) is str and viewport.Name==view:
                    plane = viewport.ConstructionPlane()
                    break
                elif type(view) is System.Guid and viewport.Id==view:
                    plane = viewport.ConstructionPlane()
                    break
            if plane is None: return scriptcontext.errorhandler()
        if plane:
            xform = Rhino.Geometry.Transform.ChangeBasis(Rhino.Geometry.Plane.WorldXY, plane)
            bbox = xform.TransformBoundingBox(bbox)
        if not bbox.IsValid: return scriptcontext.errorhandler()
        corners = list(bbox.GetCorners())
        if in_world_coords and plane is not None:
            plane_to_world = Rhino.Geometry.Transform.ChangeBasis(plane, Rhino.Geometry.Plane.WorldXY)
            for pt in corners: pt.Transform(plane_to_world)
        return corners
    *)


