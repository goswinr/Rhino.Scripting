namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsMesh =
  type RhinoScriptSyntax with
    
    ///<summary>Add a mesh object to the document</summary>
    ///<param name="vertices">(Point3d seq) List of 3D points defining the vertices of the mesh</param>
    ///<param name="faceVertices">(float seq) List containing lists of 3 or 4 numbers that define the
    ///  vertex indices for each face of the mesh. If the third a fourth vertex
    ///    indices of a face are identical, a triangular face will be created.</param>
    ///<param name="vertexNormals">(Vector3d seq) Optional, Default Value: <c>null</c>
    ///List of 3D vectors defining the vertex normals of
    ///  the mesh. Note, for every vertex, there must be a corresponding vertex
    ///  normal</param>
    ///<param name="textureCoordinates">(float seq) Optional, Default Value: <c>null</c>
    ///List of 2D texture coordinates. For every
    ///  vertex, there must be a corresponding texture coordinate</param>
    ///<param name="vertexColors">(Drawing.Color seq) Optional, Default Value: <c>null</c>
    ///A list of color values. For every vertex,
    ///  there must be a corresponding vertex color</param>
    ///<returns>(Guid) Identifier of the new object</returns>
    static member AddMesh(vertices:Point3d seq, faceVertices:float seq, [<OPT;DEF(null)>]vertexNormals:Vector3d seq, [<OPT;DEF(null)>]textureCoordinates:float seq, [<OPT;DEF(null)>]vertexColors:Drawing.Color seq) : Guid =
        let mesh = Mesh()
        for a, b, c in vertices do mesh.Vertices.Add(a, b, c)
        for face in face_vertices do
            if Seq.length(face)<4 then
                mesh.Faces.AddFace(face.[0], face.[1], face.[2])
            else
                mesh.Faces.AddFace(face.[0], face.[1], face.[2], face.[3])
        if vertex_normals then
            let count = Seq.length(vertex_normals)
            let normals = Array.CreateInstance(Vector3f, count)
            for i, normal in enumerate(vertex_normals) do
                let normals.[i] = Vector3f(normal.[0], normal.[1], normal.[2])
            mesh.Normals.SetNormals(normals)
        if texture_coordinates then
            count <- Seq.length(texture_coordinates)
            let tcs = Array.CreateInstance(Point2f, count)
            for i, tc in enumerate(texture_coordinates) do
                let tcs.[i] = Point2f(tc.[0], tc.[1])
            mesh.TextureCoordinates.SetTextureCoordinates(tcs)
        if vertexColors then
            count <- Seq.length(vertexColors)
            let colors = Array.CreateInstance(Drawing.Color, count)
            for i, color in enumerate(vertexColors) do
                let colors.[i] = Coerce.coercecolor(color)
            mesh.VertexColors.SetColors(colors)
        let rc = Doc.Objects.AddMesh(mesh)
        if rc=Guid.Empty then failwithf "Rhino.Scripting Error:Unable to add mesh to document.  vertices:"%A" faceVertices:"%A" vertexNormals:"%A" textureCoordinates:"%A" vertexColors:"%A"" vertices faceVertices vertexNormals textureCoordinates vertexColors
        Doc.Views.Redraw()
        rc
    (*
    def AddMesh(vertices, face_vertices, vertex_normals=None, texture_coordinates=None, vertex_colors=None):
        """Add a mesh object to the document
        Parameters:
          vertices ([point, ...]) list of 3D points defining the vertices of the mesh
          face_vertices ([[number, number, number], [number, number, number, number], ...]) list containing lists of 3 or 4 numbers that define the
                        vertex indices for each face of the mesh. If the third a fourth vertex
                         indices of a face are identical, a triangular face will be created.
          vertex_normals ([vector, ...], optional) list of 3D vectors defining the vertex normals of
            the mesh. Note, for every vertex, there must be a corresponding vertex
            normal
          texture_coordinates ([[number, number], [number, number], [number, number]], ...], optional): list of 2D texture coordinates. For every
            vertex, there must be a corresponding texture coordinate
          vertex_colors ([color, ...]) a list of color values. For every vertex,
            there must be a corresponding vertex color
        Returns:
          guid: Identifier of the new object if successful
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          vertices = []
          vertices.append((0.0,0.0,0.0))
          vertices.append((5.0, 0.0, 0.0))
          vertices.append((10.0, 0.0, 0.0))
          vertices.append((0.0, 5.0, 0.0))
          vertices.append((5.0, 5.0, 0.0))
          vertices.append((10.0, 5.0, 0.0))
          vertices.append((0.0, 10.0, 0.0))
          vertices.append((5.0, 10.0, 0.0))
          vertices.append((10.0, 10.0, 0.0))
          faceVertices = []
          faceVertices.append((0,1,4,4))
          faceVertices.append((2,4,1,1))
          faceVertices.append((0,4,3,3))
          faceVertices.append((2,5,4,4))
          faceVertices.append((3,4,6,6))
          faceVertices.append((5,8,4,4))
          faceVertices.append((6,4,7,7))
          faceVertices.append((8,7,4,4))
          rs.AddMesh( vertices, faceVertices )
        See Also:
          MeshFaces
          MeshFaceVertices
          MeshVertexNormals
          MeshVertices
        """
        mesh = Rhino.Geometry.Mesh()
        for a, b, c in vertices: mesh.Vertices.Add(a, b, c)
        for face in face_vertices:
            if len(face)<4:
                mesh.Faces.AddFace(face[0], face[1], face[2])
            else:
                mesh.Faces.AddFace(face[0], face[1], face[2], face[3])
        if vertex_normals:
            count = len(vertex_normals)
            normals = System.Array.CreateInstance(Rhino.Geometry.Vector3f, count)
            for i, normal in enumerate(vertex_normals):
                normals[i] = Rhino.Geometry.Vector3f(normal[0], normal[1], normal[2])
            mesh.Normals.SetNormals(normals)
        if texture_coordinates:
            count = len(texture_coordinates)
            tcs = System.Array.CreateInstance(Rhino.Geometry.Point2f, count)
            for i, tc in enumerate(texture_coordinates):
                tcs[i] = Rhino.Geometry.Point2f(tc[0], tc[1])
            mesh.TextureCoordinates.SetTextureCoordinates(tcs)
        if vertex_colors:
            count = len(vertex_colors)
            colors = System.Array.CreateInstance(System.Drawing.Color, count)
            for i, color in enumerate(vertex_colors):
                colors[i] = rhutil.coercecolor(color)
            mesh.VertexColors.SetColors(colors)
        rc = scriptcontext.doc.Objects.AddMesh(mesh)
        if rc==System.Guid.Empty: raise Exception("unable to add mesh to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Creates a planar mesh from a closed, planar curve</summary>
    ///<param name="objectId">(Guid) Identifier of a closed, planar curve</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///If True, delete the input curve defined by objectId</param>
    ///<returns>(Guid) id of the new mesh on success</returns>
    static member AddPlanarMesh(objectId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let curve = Coerce.coercecurve(object_id, -1, true)
        let tolerance = Doc.ModelAbsoluteTolerance
        let mesh = Mesh.CreateFromPlanarBoundary(curve, MeshingParameters.Default, tolerance)
        if not <| mesh then failwithf "Rhino.Scripting Error:AddPlanarMesh failed.  objectId:"%A" deleteInput:"%A"" objectId deleteInput
        if deleteInput then
            let id = Coerce.coerceguid(deleteInput, true)
            let rc = Doc.Objects.Replace(id, mesh)
        else
            rc <- Doc.Objects.AddMesh(mesh)
        if rc=Guid.Empty then failwithf "Rhino.Scripting Error:Unable to add mesh to document.  objectId:"%A" deleteInput:"%A"" objectId deleteInput
        Doc.Views.Redraw()
        rc
    (*
    def AddPlanarMesh(object_id, delete_input=False):
        """Creates a planar mesh from a closed, planar curve
        Parameters:
          object_id (guid): identifier of a closed, planar curve
          delete_input (bool, optional) if True, delete the input curve defined by object_id
        Returns:
          guid: id of the new mesh on success
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select planar curves to build mesh", rs.filter.curve)
          if obj: rs.AddPlanarMesh(obj)
        See Also:
          IsCurveClosed
          IsCurvePlanar
        """
        curve = rhutil.coercecurve(object_id, -1, True)
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        mesh = Rhino.Geometry.Mesh.CreateFromPlanarBoundary(curve, Rhino.Geometry.MeshingParameters.Default, tolerance)
        if not mesh: return scriptcontext.errorhandler()
        if delete_input:
            id = rhutil.coerceguid(delete_input, True)
            rc = scriptcontext.doc.Objects.Replace(id, mesh)
        else:
            rc = scriptcontext.doc.Objects.AddMesh(mesh)
        if rc==System.Guid.Empty: raise Exception("unable to add mesh to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Calculates the intersection of a curve object and a mesh object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="meshId">(Guid) Identifier or a mesh object</param>
    ///<param name="returnFaces">(bool) Optional, Default Value: <c>false</c>
    ///Return both intersection points and face indices.
    ///  If False, then just the intersection points are returned</param>
    ///<returns>(Point3d seq) if return_false is omitted or False, then a list of intersection points
    ///  list([point, number], ...): if return_false is True, the a one-dimensional list containing information
    ///    about each intersection. Each element contains the following two elements
    ///      [0] = point of intersection
    ///      [1] = mesh face index where intersection lies</returns>
    static member CurveMeshIntersection(curveId:Guid, meshId:Guid, [<OPT;DEF(false)>]returnFaces:bool) : Point3d seq =
        let curve = Coerce.coercecurve(curve_id, -1, true)
        let mesh = Coerce.coercemesh(mesh_id, true)
        let tolerance = Doc.ModelAbsoluteTolerance
        let polylinecurve = curve.ToPolyline(0,0,0,0,0.0,tolerance,0.0,0.0,true)
        let pts, faceids = Intersect.Intersection.MeshPolyline(mesh, polylinecurve)
        if not <| pts then failwithf "Rhino.Scripting Error:CurveMeshIntersection failed.  curveId:"%A" meshId:"%A" returnFaces:"%A"" curveId meshId returnFaces
        let pts = list(pts)
        if Faces then
            let faceids = list(faceids)
            zip(pts, faceids)
        pts
    (*
    def CurveMeshIntersection(curve_id, mesh_id, return_faces=False):
        """Calculates the intersection of a curve object and a mesh object
        Parameters:
          curve_id (guid): identifier of a curve object
          mesh_id (guid): identifier or a mesh object
          return_faces (bool, optional): return both intersection points and face indices.
            If False, then just the intersection points are returned
        Returns:
          list(point, ...): if return_false is omitted or False, then a list of intersection points
            list([point, number], ...): if return_false is True, the a one-dimensional list containing information
              about each intersection. Each element contains the following two elements
                [0] = point of intersection
                [1] = mesh face index where intersection lies
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          curve = rs.GetObject("Select curve to intersect", rs.filter.curve)
          if curve:
              mesh = rs.GetObject("Select mesh to intersect", rs.filter.mesh)
              if mesh:
                  cmx = rs.CurveMeshIntersection(curve, mesh, True)
                  if cmx:
                      for element in cmx:
                          print element[0], ", Face index = ", element[1]
                          rs.AddPoint(element[0])
        See Also:
          MeshClosestPoint
          MeshMeshIntersection
        """
        curve = rhutil.coercecurve(curve_id, -1, True)
        mesh = rhutil.coercemesh(mesh_id, True)
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        polylinecurve = curve.ToPolyline(0,0,0,0,0.0,tolerance,0.0,0.0,True)
        pts, faceids = Rhino.Geometry.Intersect.Intersection.MeshPolyline(mesh, polylinecurve)
        if not pts: return scriptcontext.errorhandler()
        pts = list(pts)
        if return_faces:
            faceids = list(faceids)
            return zip(pts, faceids)
        return pts
    *)


    ///<summary>Returns number of meshes that could be created by calling SplitDisjointMesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of meshes that could be created</returns>
    static member DisjointMeshCount(objectId:Guid) : int =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.DisjointMeshCount
    (*
    def DisjointMeshCount(object_id):
        """Returns number of meshes that could be created by calling SplitDisjointMesh
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          number: The number of meshes that could be created
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          if rs.DisjointMeshCount(obj)>1: rs.SplitDisjointMesh(obj)
        See Also:
          IsMesh
          SplitDisjointMesh
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.DisjointMeshCount
    *)


    ///<summary>Creates curves that duplicates a mesh border</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Guid seq) list of curve ids on success</returns>
    static member DuplicateMeshBorder(meshId:Guid) : Guid seq =
        let mesh = Coerce.coercemesh(meshId, true)
        let polylines = mesh.GetNakedEdges()
        let rc = ResizeArray()
        if polylines then
            for polyline in polylines do
                let id = Doc.Objects.AddPolyline(polyline)
                if id<>Guid.Empty then rc.Add(id)
        if rc then Doc.Views.Redraw()
        rc
    (*
    def DuplicateMeshBorder(mesh_id):
        """Creates curves that duplicates a mesh border
        Parameters:
          mesh_id (guid): identifier of a mesh object
        Returns:
          list(guid, ...): list of curve ids on success
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          if obj: rs.DuplicateMeshBorder(obj)
        See Also:
          DuplicateEdgeCurves
          DuplicateSurfaceBorder
        """
        mesh = rhutil.coercemesh(mesh_id, True)
        polylines = mesh.GetNakedEdges()
        rc = []
        if polylines:
            for polyline in polylines:
                id = scriptcontext.doc.Objects.AddPolyline(polyline)
                if id!=System.Guid.Empty: rc.append(id)
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Explodes a mesh object, or mesh objects int submeshes. A submesh is a
    ///  collection of mesh faces that are contained within a closed loop of
    ///  unwelded mesh edges. Unwelded mesh edges are where the mesh faces that
    ///  share the edge have unique mesh vertices (not mesh topology vertices)
    ///  at both ends of the edge</summary>
    ///<param name="meshIds">(Guid seq) List of mesh identifiers</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid seq) List of resulting objects after explode.</returns>
    static member ExplodeMeshes(meshIds:Guid seq, [<OPT;DEF(false)>]delete:bool) : Guid seq =
        let id = Coerce.coerceguid(meshIds)
        if id then meshIds <- .[meshIds]
        let rc = ResizeArray()
        for mesh_id in meshIds do
            let mesh = Coerce.coercemesh(mesh_id, true)
            if mesh then
                let submeshes = mesh.ExplodeAtUnweldedEdges()
                if submeshes then
                    for submesh in submeshes do
                        id <- Doc.Objects.AddMesh(submesh)
                        if id<>Guid.Empty then rc.Add(id)
                if delete then
                    Doc.Objects.Delete(mesh_id, true)
        if rc then Doc.Views.Redraw()
        rc
    (*
    def ExplodeMeshes(mesh_ids, delete=False):
        """Explodes a mesh object, or mesh objects int submeshes. A submesh is a
        collection of mesh faces that are contained within a closed loop of
        unwelded mesh edges. Unwelded mesh edges are where the mesh faces that
        share the edge have unique mesh vertices (not mesh topology vertices)
        at both ends of the edge
        Parameters:
          mesh_ids ([guid, ...]): list of mesh identifiers
          delete (bool, optional): delete the input meshes
        Returns:
          list(guid, ...): List of resulting objects after explode.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh to explode", rs.filter.mesh)
          if rs.IsMesh(obj): rs.ExplodeMeshes(obj)
        See Also:
          IsMesh
        """
        id = rhutil.coerceguid(mesh_ids)
        if id: mesh_ids = [mesh_ids]
        rc = []
        for mesh_id in mesh_ids:
            mesh = rhutil.coercemesh(mesh_id, True)
            if mesh:
                submeshes = mesh.ExplodeAtUnweldedEdges()
                if submeshes:
                    for submesh in submeshes:
                        id = scriptcontext.doc.Objects.AddMesh(submesh)
                        if id!=System.Guid.Empty: rc.append(id)
                if delete:
                    scriptcontext.doc.Objects.Delete(mesh_id, True)
                    
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Verifies if an object is a mesh</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsMesh(objectId:Guid) : bool =
        let mesh = Coerce.coercemesh(objectId)
        mesh <> null
    (*
    def IsMesh(object_id):
        """Verifies if an object is a mesh
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if successful, otherwise False
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select a mesh")
          if rs.IsMesh(obj):
              print "The object is a mesh."
          else:
              print "The object is not a mesh."
        See Also:
          IsMeshClosed
          MeshFaceCount
          MeshFaces
          MeshVertexCount
          MeshVertices
        """
        mesh = rhutil.coercemesh(object_id)
        return mesh is not None
    *)


    ///<summary>Verifies a mesh object is closed</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsMeshClosed(objectId:Guid) : bool =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.IsClosed
    (*
    def IsMeshClosed(object_id):
        """Verifies a mesh object is closed
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select a mesh", rs.filter.mesh)
          if rs.IsMeshClosed(obj):
              print "The mesh is closed."
          else:
              print "The mesh is not closed."
        See Also:
          IsMesh
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.IsClosed
    *)


    ///<summary>Verifies a mesh object is manifold. A mesh for which every edge is shared
    ///  by at most two faces is called manifold. If a mesh has at least one edge
    ///  that is shared by more than two faces, then that mesh is called non-manifold</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsMeshManifold(objectId:Guid) : bool =
        let mesh = Coerce.coercemesh(objectId, true)
        let rc = mesh.IsManifold(true)
        rc.[0]
    (*
    def IsMeshManifold(object_id):
        """Verifies a mesh object is manifold. A mesh for which every edge is shared
        by at most two faces is called manifold. If a mesh has at least one edge
        that is shared by more than two faces, then that mesh is called non-manifold
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select a mesh", rs.filter.mesh)
          if rs.IsMeshClosed(obj):
              print "The mesh is manifold."
          else:
              print "The mesh is non-manifold."
        See Also:
          IsMesh
          IsMeshClosed
        """
        mesh = rhutil.coercemesh(object_id, True)
        rc = mesh.IsManifold(True)
        return rc[0]
    *)


    ///<summary>Verifies a point is on a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="point">(Point3d) Test point</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsPointOnMesh(objectId:Guid, point:Point3d) : bool =
        let mesh = Coerce.coercemesh(objectId, true)
        let point = Coerce.coerce3dpoint(point, true)
        let max_distance = Rhino.RhinoMath.SqrtEpsilon
        let face, pt = mesh.ClosestPoint(point, max_distance)
        face>=0
    (*
    def IsPointOnMesh(object_id, point):
        """Verifies a point is on a mesh
        Parameters:
          object_id (guid): identifier of a mesh object
          point (point): test point
        Returns:
          bool: True if successful, otherwise False.
          None: on error.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select a mesh")
          if rs.IsMesh(obj):
              point = rs.GetPointOnMesh(strObject, "Pick a test point")
              if point:
                  if rs.IsPointOnMesh(obj, point):
                      print "The point is on the mesh"
                  else:
                      print "The point is not on the mesh"
        See Also:
          IsMesh
          MeshClosestPoint
        """
        mesh = rhutil.coercemesh(object_id, True)
        point = rhutil.coerce3dpoint(point, True)
        max_distance = Rhino.RhinoMath.SqrtEpsilon
        face, pt = mesh.ClosestPoint(point, max_distance)
        return face>=0
    *)


    ///<summary>Joins two or or more mesh objects together</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of two or more mesh objects</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input after joining</param>
    ///<returns>(Guid) identifier of newly created mesh on success</returns>
    static member JoinMeshes(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let meshes = [| for id in object_ids -> Coerce.coercemesh(id,true) |]
        let joined_mesh = Mesh()
        joined_mesh.Append(meshes)
        let rc = Doc.Objects.AddMesh(joined_mesh)
        if deleteInput then
            for id in object_ids do
                let guid = Coerce.coerceguid(id)
                Doc.Objects.Delete(guid,true)
        Doc.Views.Redraw()
        rc
    (*
    def JoinMeshes(object_ids, delete_input=False):
        """Joins two or or more mesh objects together
        Parameters:
          object_ids ([guid, ...]): identifiers of two or more mesh objects
          delete_input (bool, optional): delete input after joining
        Returns:
          guid: identifier of newly created mesh on success
        Example:
          import rhinoscriptsyntax as rs
          objs = rs.GetObjects("Select meshes to join", rs.filter.mesh)
          if objs and len(objs)>1: rs.JoinMeshes(objs, True)
        See Also:
          JoinCurves
          JoinSurfaces
        """
        meshes = [rhutil.coercemesh(id,True) for id in object_ids]
        joined_mesh = Rhino.Geometry.Mesh()
        joined_mesh.Append(meshes)
        rc = scriptcontext.doc.Objects.AddMesh(joined_mesh)
        if delete_input:
            for id in object_ids:
                guid = rhutil.coerceguid(id)
                scriptcontext.doc.Objects.Delete(guid,True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns approximate area of one or more mesh objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more mesh objects</param>
    ///<returns>(float * float * float)   where
    ///    [0] = number of meshes used in calculation
    ///    [1] = total area of all meshes
    ///    [2] = the error estimate</returns>
    static member MeshArea(objectIds:Guid seq) : float * float * float =
        let id = Coerce.coerceguid(objectIds)
        if id then objectIds <- .[objectIds]
        let meshes_used = 0
        let total_area = 0.0
        let error_estimate = 0.0
        for id in objectIds do
            let mesh = Coerce.coercemesh(id, true)
            if mesh then
                let mp = AreaMassProperties.Compute(mesh)
                if mp then
                    meshes_used += 1
                    total_area += mp.Area
                    error_estimate += mp.AreaError
        if meshes_used=0 then failwithf "Rhino.Scripting Error:MeshArea failed.  objectIds:"%A"" objectIds
        meshes_used, total_area, error_estimate
    (*
    def MeshArea(object_ids):
        """Returns approximate area of one or more mesh objects
        Parameters:
          object_ids ([guid, ...]): identifiers of one or more mesh objects
        Returns:
          list(number, number, number): if successful where
            [0] = number of meshes used in calculation
            [1] = total area of all meshes
            [2] = the error estimate
          None: if not successful
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh )
          if obj:
              area_rc = rs.MeshArea(obj)
              if area_rc: print "Mesh area:", area_rc[1]
        See Also:
          MeshVolume
        """
        id = rhutil.coerceguid(object_ids)
        if id: object_ids = [object_ids]
        meshes_used = 0
        total_area = 0.0
        error_estimate = 0.0
        for id in object_ids:
            mesh = rhutil.coercemesh(id, True)
            if mesh:
                mp = Rhino.Geometry.AreaMassProperties.Compute(mesh)
                if mp:
                    meshes_used += 1
                    total_area += mp.Area
                    error_estimate += mp.AreaError
        if meshes_used==0: return scriptcontext.errorhandler()
        return meshes_used, total_area, error_estimate
    *)


    ///<summary>Calculates the area centroid of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d) representing the area centroid</returns>
    static member MeshAreaCentroid(objectId:Guid) : Point3d =
        let mesh = Coerce.coercemesh(objectId, true)
        let mp = AreaMassProperties.Compute(mesh)
        if mp = null then failwithf "Rhino.Scripting Error:MeshAreaCentroid failed.  objectId:"%A"" objectId
        mp.Centroid
    (*
    def MeshAreaCentroid(object_id):
        """Calculates the area centroid of a mesh object
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          point: representing the area centroid if successful
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh )
          rs.AddPoint( rs.MeshAreaCentroid(obj) )
        See Also:
          IsMesh
          MeshArea
          MeshVolume
          MeshVolumeCentroid
        """
        mesh = rhutil.coercemesh(object_id, True)
        mp = Rhino.Geometry.AreaMassProperties.Compute(mesh)
        if mp is None: return scriptcontext.errorhandler()
        return mp.Centroid
    *)


    ///<summary>Performs boolean difference operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid) Input0 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="input1">(Guid) Input1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///A positive tolerance value, or None to use the default of the document.</param>
    ///<returns>(Guid seq) identifiers of newly created meshes</returns>
    static member MeshBooleanDifference(input0:Guid, input1:Guid, [<OPT;DEF(true)>]deleteInput:bool, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        let id = Coerce.coerceguid(input0)
        if id then input0 <- .[id]
        id <- Coerce.coerceguid(input1)
        if id then input1 <- .[id]
        let meshes0 = [| for id in input0 -> Coerce.coercemesh(id, true) |]
        let meshes1 = [| for id in input1 -> Coerce.coercemesh(id, true) |]
        if not <| meshes0 || not <| meshes1 then failwithf "Rhino.Scripting Error:No meshes to work with.  input0:"%A" input1:"%A" deleteInput:"%A" tolerance:"%A"" input0 input1 deleteInput tolerance
        let newmeshes = Mesh.CreateBooleanDifference(meshes0, meshes1)
        let rc = ResizeArray()
        for mesh in newmeshes do
            id <- Doc.Objects.AddMesh(mesh)
            if id<>Guid.Empty then rc.Add(id)
        if rc && deleteInput then
            let input = input0 + input1
            for id in input do
                id <- Coerce.coerceguid(id, true)
                Doc.Objects.Delete(id, true)
        Doc.Views.Redraw()
        rc
    (*
    def MeshBooleanDifference(input0, input1, delete_input=True, tolerance=None):
        """Performs boolean difference operation on two sets of input meshes
        Parameters:
          input0, input1 (guid): identifiers of meshes
          delete_input (bool, optional): delete the input meshes
          tolerance (float, optional): a positive tolerance value, or None to use the default of the document.
        Returns:
          list(guid, ...): identifiers of newly created meshes
        Example:
          import rhinoscriptsyntax as rs
          input0 = rs.GetObjects("Select first set of meshes", rs.filter.mesh)
          if input0:
              input1 = rs.GetObjects("Select second set of meshes", rs.filter.mesh)
              if input1: rs.MeshBooleanDifference(input0, input1)
        See Also:
          MeshBooleanIntersection
          MeshBooleanSplit
          MeshBooleanUnion
        """
        id = rhutil.coerceguid(input0)
        if id: input0 = [id]
        id = rhutil.coerceguid(input1)
        if id: input1 = [id]
        meshes0 = [rhutil.coercemesh(id, True) for id in input0]
        meshes1 = [rhutil.coercemesh(id, True) for id in input1]
        if not meshes0 or not meshes1: raise ValueError("no meshes to work with")
        newmeshes = Rhino.Geometry.Mesh.CreateBooleanDifference(meshes0, meshes1)
        rc = []
        for mesh in newmeshes:
            id = scriptcontext.doc.Objects.AddMesh(mesh)
            if id!=System.Guid.Empty: rc.append(id)
        if rc and delete_input:
            input = input0 + input1
            for id in input:
                id = rhutil.coerceguid(id, True)
                scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Performs boolean intersection operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid) Input0 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="input1">(Guid) Input1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid seq) identifiers of new meshes on success</returns>
    static member MeshBooleanIntersection(input0:Guid, input1:Guid, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        let id = Coerce.coerceguid(input0)
        if id then input0 <- .[id]
        id <- Coerce.coerceguid(input1)
        if id then input1 <- .[id]
        let meshes0 = [| for id in input0 -> Coerce.coercemesh(id, true) |]
        let meshes1 = [| for id in input1 -> Coerce.coercemesh(id, true) |]
        if not <| meshes0 || not <| meshes1 then failwithf "Rhino.Scripting Error:No meshes to work with.  input0:"%A" input1:"%A" deleteInput:"%A"" input0 input1 deleteInput
        let newmeshes = Mesh.CreateBooleanIntersection(meshes0, meshes1)
        let rc = ResizeArray()
        for mesh in newmeshes do
            id <- Doc.Objects.AddMesh(mesh)
            if id<>Guid.Empty then rc.Add(id)
        if rc && deleteInput then
            let input = input0 + input1
            for id in input do
                id <- Coerce.coerceguid(id, true)
                Doc.Objects.Delete(id, true)
        Doc.Views.Redraw()
        rc
    (*
    def MeshBooleanIntersection(input0, input1, delete_input=True):
        """Performs boolean intersection operation on two sets of input meshes
        Parameters:
          input0, input1 (guid): identifiers of meshes
          delete_input (bool, optional): delete the input meshes
        Returns:
          list(guid, ...): identifiers of new meshes on success
        Example:
          import rhinoscriptsyntax as rs
          input0 = rs.GetObjects("Select first set of meshes", rs.filter.mesh)
          if input0:
              input1 = rs.GetObjects("Select second set of meshes", rs.filter.mesh)
              if input1: rs.MeshBooleanIntersection(input0, input1)
        See Also:
          MeshBooleanDifference
          MeshBooleanSplit
          MeshBooleanUnion
        """
        id = rhutil.coerceguid(input0)
        if id: input0 = [id]
        id = rhutil.coerceguid(input1)
        if id: input1 = [id]
        meshes0 = [rhutil.coercemesh(id, True) for id in input0]
        meshes1 = [rhutil.coercemesh(id, True) for id in input1]
        if not meshes0 or not meshes1: raise ValueError("no meshes to work with")
        newmeshes = Rhino.Geometry.Mesh.CreateBooleanIntersection(meshes0, meshes1)
        rc = []
        for mesh in newmeshes:
            id = scriptcontext.doc.Objects.AddMesh(mesh)
            if id!=System.Guid.Empty: rc.append(id)
        if rc and delete_input:
            input = input0 + input1
            for id in input:
                id = rhutil.coerceguid(id, True)
                scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Performs boolean split operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid) Input0 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="input1">(Guid) Input1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid seq) identifiers of new meshes on success</returns>
    static member MeshBooleanSplit(input0:Guid, input1:Guid, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        let id = Coerce.coerceguid(input0)
        if id then input0 <- .[id]
        id <- Coerce.coerceguid(input1)
        if id then input1 <- .[id]
        let meshes0 = [| for id in input0 -> Coerce.coercemesh(id, true) |]
        let meshes1 = [| for id in input1 -> Coerce.coercemesh(id, true) |]
        if not <| meshes0 || not <| meshes1 then failwithf "Rhino.Scripting Error:No meshes to work with.  input0:"%A" input1:"%A" deleteInput:"%A"" input0 input1 deleteInput
        let newmeshes = Mesh.CreateBooleanSplit(meshes0, meshes1)
        let rc = ResizeArray()
        for mesh in newmeshes do
            id <- Doc.Objects.AddMesh(mesh)
            if id<>Guid.Empty then rc.Add(id)
        if rc && deleteInput then
            let input = input0 + input1
            for id in input do
                id <- Coerce.coerceguid(id, true)
                Doc.Objects.Delete(id, true)
        Doc.Views.Redraw()
        rc
    (*
    def MeshBooleanSplit(input0, input1, delete_input=True):
        """Performs boolean split operation on two sets of input meshes
        Parameters:
          input0, input1 (guid): identifiers of meshes
          delete_input (bool, optional): delete the input meshes
        Returns:
          list(guid, ...): identifiers of new meshes on success
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          input0 = rs.GetObjects("Select first set of meshes", rs.filter.mesh)
          if input0:
              input1 = rs.GetObjects("Select second set of meshes", rs.filter.mesh)
              if input1: rs.MeshBooleanSplit(input0, input1)
        See Also:
          MeshBooleanDifference
          MeshBooleanIntersection
          MeshBooleanUnion
        """
        id = rhutil.coerceguid(input0)
        if id: input0 = [id]
        id = rhutil.coerceguid(input1)
        if id: input1 = [id]
        meshes0 = [rhutil.coercemesh(id, True) for id in input0]
        meshes1 = [rhutil.coercemesh(id, True) for id in input1]
        if not meshes0 or not meshes1: raise ValueError("no meshes to work with")
        newmeshes = Rhino.Geometry.Mesh.CreateBooleanSplit(meshes0, meshes1)
        rc = []
        for mesh in newmeshes:
            id = scriptcontext.doc.Objects.AddMesh(mesh)
            if id!=System.Guid.Empty: rc.append(id)
        if rc and delete_input:
            input = input0 + input1
            for id in input:
                id = rhutil.coerceguid(id, True)
                scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Performs boolean union operation on a set of input meshes</summary>
    ///<param name="meshIds">(Guid seq) Identifiers of meshes</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid seq) identifiers of new meshes</returns>
    static member MeshBooleanUnion(meshIds:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        if Seq.length(mesh_ids)<2 then failwithf "Rhino.Scripting Error:Mesh_ids must contain at least 2 meshes.  meshIds:"%A" deleteInput:"%A"" meshIds deleteInput
        let meshes = [| for id in mesh_ids -> Coerce.coercemesh(id, true) |]
        let newmeshes = Mesh.CreateBooleanUnion(meshes)
        let rc = ResizeArray()
        for mesh in newmeshes do
            let id = Doc.Objects.AddMesh(mesh)
            if id<>Guid.Empty then rc.Add(id)
        if rc && deleteInput then
            for id in mesh_ids do
                id <- Coerce.coerceguid(id, true)
                Doc.Objects.Delete(id, true)
        Doc.Views.Redraw()
        rc
    (*
    def MeshBooleanUnion(mesh_ids, delete_input=True):
        """Performs boolean union operation on a set of input meshes
        Parameters:
          mesh_ids ([guid, ...]): identifiers of meshes
          delete_input (bool, optional): delete the input meshes
        Returns:
          list(guid, ...): identifiers of new meshes
        Example:
          import rhinoscriptsyntax as rs
          input = rs.GetObjects("Select meshes to union", rs.filter.mesh)
          if input: rs.MeshBooleanUnion(input)
        See Also:
          MeshBooleanDifference
          MeshBooleanIntersection
          MeshBooleanSplit
        """
        if len(mesh_ids)<2: raise ValueError("mesh_ids must contain at least 2 meshes")
        meshes = [rhutil.coercemesh(id, True) for id in mesh_ids]
        newmeshes = Rhino.Geometry.Mesh.CreateBooleanUnion(meshes)
        rc = []
        for mesh in newmeshes:
            id = scriptcontext.doc.Objects.AddMesh(mesh)
            if id!=System.Guid.Empty: rc.append(id)
        if rc and delete_input:
            for id in mesh_ids:
                id = rhutil.coerceguid(id, True)
                scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the point on a mesh that is closest to a test point</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="point">(Point3d) Point to test</param>
    ///<param name="maximumDistance">(float) Optional, Default Value: <c>null</c>
    ///Upper bound used for closest point calculation.
    ///  If you are only interested in finding a point Q on the mesh when
    ///  point.DistanceTo(Q) < maximumDistance, then set maximumDistance to
    ///  that value</param>
    ///<returns>(Point3d * float) containing the results of the calculation where
    ///  [0] = the 3-D point on the mesh
    ///  [1] = the index of the mesh face on which the 3-D point lies</returns>
    static member MeshClosestPoint(objectId:Guid, point:Point3d, [<OPT;DEF(null)>]maximumDistance:float) : Point3d * float =
        let mesh = Coerce.coercemesh(object_id, true)
        let point = Coerce.coerce3dpoint(point, true)
        tolerance<-maximumDistance if maximumDistance else 0.0
        let face, closest_point = mesh.ClosestPoint(point, tolerance)
        if face<0 then failwithf "Rhino.Scripting Error:MeshClosestPoint failed.  objectId:"%A" point:"%A" maximumDistance:"%A"" objectId point maximumDistance
        closest_point, face
    (*
    def MeshClosestPoint(object_id, point, maximum_distance=None):
        """Returns the point on a mesh that is closest to a test point
        Parameters:
          object_id (guid): identifier of a mesh object
          point (point): point to test
          maximum_distance (number, optional): upper bound used for closest point calculation.
            If you are only interested in finding a point Q on the mesh when
            point.DistanceTo(Q) < maximum_distance, then set maximum_distance to
            that value
        Returns:
          tuple(point, number): containing the results of the calculation where
                                [0] = the 3-D point on the mesh
                                [1] = the index of the mesh face on which the 3-D point lies
          None: on error
        Example:
          import rhinocriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          point = rs.GetPoint("Pick test point")
          intersect = rs.MeshClosestPoint(obj, point)
          if intersect: rs.AddPoint(intersect)
        See Also:
          MeshFaceCount
          MeshFaces
        """
        mesh = rhutil.coercemesh(object_id, True)
        point = rhutil.coerce3dpoint(point, True)
        tolerance=maximum_distance if maximum_distance else 0.0
        face, closest_point = mesh.ClosestPoint(point, tolerance)
        if face<0: return scriptcontext.errorhandler()
        return closest_point, face
    *)


    ///<summary>Returns the center of each face of the mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d seq) points defining the center of each face</returns>
    static member MeshFaceCenters(meshId:Guid) : Point3d seq =
        let mesh = Coerce.coercemesh(meshId, true)
        [| for i=0 to mesh.Faces.Count) -> mesh.Faces.GetFaceCenter(i) |]
    (*
    def MeshFaceCenters(mesh_id):
        """Returns the center of each face of the mesh object
        Parameters:
          mesh_id (guid): identifier of a mesh object
        Returns:
          list(point, ...): points defining the center of each face
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          centers = rs.MeshFaceCenters(obj)
          if centers:
              for point in centers: rs.AddPoint(point)
        See Also:
          IsMesh
          MeshFaceCount
          MeshFaces
        """
        mesh = rhutil.coercemesh(mesh_id, True)
        return [mesh.Faces.GetFaceCenter(i) for i in range(mesh.Faces.Count)]
    *)


    ///<summary>Returns total face count of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) the number of mesh faces</returns>
    static member MeshFaceCount(objectId:Guid) : int =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.Faces.Count
    (*
    def MeshFaceCount(object_id):
        """Returns total face count of a mesh object
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          number: the number of mesh faces if successful
        Example:
          import rhinocsriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh )
          print "Quad faces:", rs.MeshQuadCount(obj)
          print "Triangle faces:", rs.MeshTriangleCount(obj)
          print "Total faces:", rs.MeshFaceCount(obj)
        See Also:
          IsMesh
          MeshFaces
          MeshVertexCount
          MeshVertices
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.Faces.Count
    *)


    ///<summary>Returns the face unit normal for each face of a mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Vector3d seq) 3D vectors that define the face unit normals of the mesh</returns>
    static member MeshFaceNormals(meshId:Guid) : Vector3d seq =
        let mesh = Coerce.coercemesh(meshId, true)
        if mesh.FaceNormals.Count <> mesh.Faces.Count then
            mesh.FaceNormals.ComputeFaceNormals()
        let rc = ResizeArray()
        for i in xrange(mesh.FaceNormals.Count) do
            let normal = mesh.FaceNormals.[i]
            rc.Add(Vector3d(normal))
        rc
    (*
    def MeshFaceNormals(mesh_id):
        """Returns the face unit normal for each face of a mesh object
        Parameters:
          mesh_id (guid): identifier of a mesh object
        Returns:
          list(vector, ...): 3D vectors that define the face unit normals of the mesh
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          normals = rs.MeshFaceNormals(obj)
          if normals:
              for vector in normals: print vector
        See Also:
          MeshHasFaceNormals
          MeshFaceCount
          MeshFaces
        """
        mesh = rhutil.coercemesh(mesh_id, True)
        if mesh.FaceNormals.Count != mesh.Faces.Count:
            mesh.FaceNormals.ComputeFaceNormals()
        rc = []
        for i in xrange(mesh.FaceNormals.Count):
            normal = mesh.FaceNormals[i]
            rc.append(Rhino.Geometry.Vector3d(normal))
        return rc
    *)


    ///<summary>Returns face vertices of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="faceType">(bool) Optional, Default Value: <c>true</c>
    ///The face type to be returned. True = both triangles
    ///  and quads. False = only triangles</param>
    ///<returns>(Point3d seq) 3D points that define the face vertices of the mesh. If
    ///  face_type is True, then faces are returned as both quads and triangles
    ///  (4 3D points). For triangles, the third and fourth vertex will be
    ///  identical. If face_type is False, then faces are returned as only
    ///  triangles(3 3D points). Quads will be converted to triangles.</returns>
    static member MeshFaces(objectId:Guid, [<OPT;DEF(true)>]faceType:bool) : Point3d seq =
        let mesh = Coerce.coercemesh(object_id, true)
        let rc = ResizeArray()
        for i in xrange(mesh.Faces.Count) do
            let getrc, p0, p1, p2, p3 = mesh.Faces.GetFaceVertices(i)
            let p0 = Point3d(p0)
            let p1 = Point3d(p1)
            let p2 = Point3d(p2)
            let p3 = Point3d(p3)
            rc.Add( p0 )
            rc.Add( p1 )
            rc.Add( p2 )
            if faceType then
                rc.Add(p3)
            else
                if p2<>p3 then
                    rc.Add( p2 )
                    rc.Add( p3 )
                    rc.Add( p0 )
        rc
    (*
    def MeshFaces(object_id, face_type=True):
        """Returns face vertices of a mesh
        Parameters:
          object_id (guid): identifier of a mesh object
          face_type (bool, optional): The face type to be returned. True = both triangles
            and quads. False = only triangles
        Returns:
          list([point, point, point, point], ...): 3D points that define the face vertices of the mesh. If
          face_type is True, then faces are returned as both quads and triangles
          (4 3D points). For triangles, the third and fourth vertex will be
          identical. If face_type is False, then faces are returned as only
          triangles(3 3D points). Quads will be converted to triangles.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          faces = rs.MeshFaces(obj, False)
          if faces:
              rs.EnableRedraw(False)
              i = 0
              while( i<=len(faces) ):
                  face = faces[i], faces[i+1], faces[i+2], faces[i]
                  rs.AddPolyline( face )
                  i += 3
          rs.EnableRedraw(True)
        See Also:
          IsMesh
          MeshFaceCount
          MeshVertexCount
          MeshVertices
        """
        mesh = rhutil.coercemesh(object_id, True)
        rc = []
        for i in xrange(mesh.Faces.Count):
            getrc, p0, p1, p2, p3 = mesh.Faces.GetFaceVertices(i)
            p0 = Rhino.Geometry.Point3d(p0)
            p1 = Rhino.Geometry.Point3d(p1)
            p2 = Rhino.Geometry.Point3d(p2)
            p3 = Rhino.Geometry.Point3d(p3)
            rc.append( p0 )
            rc.append( p1 )
            rc.append( p2 )
            if face_type:
                rc.append(p3)
            else:
                if p2!=p3:
                    rc.append( p2 )
                    rc.append( p3 )
                    rc.append( p0 )
        return rc
    *)


    ///<summary>Returns the vertex indices of all faces of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(float seq) containing tuples of 4 numbers that define the vertex indices for
    ///  each face of the mesh. Both quad and triangle faces are returned. If the
    ///  third and fourth vertex indices are identical, the face is a triangle.</returns>
    static member MeshFaceVertices(objectId:Guid) : float seq =
        let mesh = Coerce.coercemesh(objectId, true)
        let rc = ResizeArray()
        for i in xrange(mesh.Faces.Count) do
            let face = mesh.Faces.GetFace(i)
            rc.Add( (face.A, face.B, face.C, face.D) )
        rc
    (*
    def MeshFaceVertices(object_id):
        """Returns the vertex indices of all faces of a mesh object
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          list((number, number, number, number), ...): containing tuples of 4 numbers that define the vertex indices for
          each face of the mesh. Both quad and triangle faces are returned. If the
          third and fourth vertex indices are identical, the face is a triangle.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          faceVerts = rs.MeshFaceVertices( obj )
          if faceVerts:
              for count, face in enumerate(faceVerts):
                  print "face(", count, ") = (", face[0], ",", face[1], ",", face[2], ",", face[3], ")"
        See Also:
          IsMesh
          MeshFaceCount
          MeshFaces
        """
        mesh = rhutil.coercemesh(object_id, True)
        rc = []
        for i in xrange(mesh.Faces.Count):
            face = mesh.Faces.GetFace(i)
            rc.append( (face.A, face.B, face.C, face.D) )
        return rc
    *)


    ///<summary>Verifies a mesh object has face normals</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasFaceNormals(objectId:Guid) : bool =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.FaceNormals.Count>0
    (*
    def MeshHasFaceNormals(object_id):
        """Verifies a mesh object has face normals
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select a mesh", rs.filter.mesh)
          if rs.MeshHasFaceNormals(obj):
              print "The mesh has face normal."
          else:
              print "The mesh does not have face normals."
        See Also:
          MeshFaceNormals
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.FaceNormals.Count>0
    *)


    ///<summary>Verifies a mesh object has texture coordinates</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasTextureCoordinates(objectId:Guid) : bool =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.TextureCoordinates.Count>0
    (*
    def MeshHasTextureCoordinates(object_id):
        """Verifies a mesh object has texture coordinates
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select a mesh", rs.filter.mesh)
          if rs.MeshHasTextureCoordinates(obj):
              print "The mesh has texture coordinates."
          else:
              print "The mesh does not have texture coordinates."
        See Also:
          
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.TextureCoordinates.Count>0
    *)


    ///<summary>Verifies a mesh object has vertex colors</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasVertexColors(objectId:Guid) : bool =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.VertexColors.Count>0
    (*
    def MeshHasVertexColors(object_id):
        """Verifies a mesh object has vertex colors
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select a mesh", rs.filter.mesh)
          if rs.mesh.MeshHasVertexColors(obj):
              print "The mesh has vertex colors."
          else:
              print "The mesh does not have vertex colors."
        See Also:
          MeshVertexColors
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.VertexColors.Count>0
    *)


    ///<summary>Verifies a mesh object has vertex normals</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasVertexNormals(objectId:Guid) : bool =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.Normals.Count>0
    (*
    def MeshHasVertexNormals(object_id):
        """Verifies a mesh object has vertex normals
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select a mesh", rs.filter.mesh)
          if rs.MeshHasVertexNormals(obj):
              print "The mesh has vertex normals."
          else:
              print "The mesh does not have vertex normals."
        See Also:
          MeshVertexNormals
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.Normals.Count>0
    *)


    ///<summary>Calculates the intersections of a mesh object with another mesh object</summary>
    ///<param name="mesh1">(Guid) Mesh1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="mesh2">(Guid) Mesh2 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///The intersection tolerance</param>
    ///<returns>(Point3d seq) of points that define the vertices of the intersection curves</returns>
    static member MeshMeshIntersection(mesh1:Guid, mesh2:Guid, [<OPT;DEF(null)>]tolerance:float) : Point3d seq =
        let mesh1 = Coerce.coercemesh(mesh1, true)
        let mesh2 = Coerce.coercemesh(mesh2, true)
        if tolerance = null then tolerance <- Rhino.RhinoMath.ZeroTolerance
        let polylines = Intersect.Intersection.MeshMeshAccurate(mesh1, mesh2, tolerance)
        if polylines then list(polylines)
    (*
    def MeshMeshIntersection(mesh1, mesh2, tolerance=None):
        """Calculates the intersections of a mesh object with another mesh object
        Parameters:
          mesh1, mesh2 (guid): identifiers of meshes
          tolerance (number, optional): the intersection tolerance
        Returns:
          list(point, ...): of points that define the vertices of the intersection curves
        Example:
          import rhinoscriptsyntax as rs
          mesh1 = rs.GetObject("Select first mesh to intersect", rs.filter.mesh)
          mesh2 = rs.GetObject("Select second mesh to intersect", rs.filter.mesh)
          results = rs.MeshMeshIntersection(mesh1, mesh2)
          if results:
              for points in results: rs.AddPolyline(points)
        See Also:
          CurveMeshIntersection
          MeshClosestPoint
        """
        mesh1 = rhutil.coercemesh(mesh1, True)
        mesh2 = rhutil.coercemesh(mesh2, True)
        if tolerance is None: tolerance = Rhino.RhinoMath.ZeroTolerance
        polylines = Rhino.Geometry.Intersect.Intersection.MeshMeshAccurate(mesh1, mesh2, tolerance)
        if polylines: return list(polylines)
    *)


    ///<summary>Identifies the naked edge points of a mesh object. This function shows
    ///  where mesh vertices are not completely surrounded by faces. Joined
    ///  meshes, such as are made by MeshBox, have naked mesh edge points where
    ///  the sub-meshes are joined</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool seq) of boolean values that represent whether or not a mesh vertex is
    ///  naked or not. The number of elements in the list will be equal to
    ///  the value returned by MeshVertexCount. In which case, the list will
    ///  identify the naked status for each vertex returned by MeshVertices</returns>
    static member MeshNakedEdgePoints(objectId:Guid) : bool seq =
        let mesh = Coerce.coercemesh(objectId, true)
        let rc = mesh.GetNakedEdgePointStatus()
        rc
    (*
    def MeshNakedEdgePoints(object_id):
        """Identifies the naked edge points of a mesh object. This function shows
        where mesh vertices are not completely surrounded by faces. Joined
        meshes, such as are made by MeshBox, have naked mesh edge points where
        the sub-meshes are joined
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          list(bool, ...): of boolean values that represent whether or not a mesh vertex is
          naked or not. The number of elements in the list will be equal to
          the value returned by MeshVertexCount. In which case, the list will
          identify the naked status for each vertex returned by MeshVertices
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          vertices = rs.MeshVertices( obj )
          naked = rs.MeshNakedEdgePoints( obj )
          for i, vertex in enumerate(vertices):
              if naked[i]: rs.AddPoint(vertex)
        See Also:
          IsMesh
          MeshVertexCount
          MeshVertices
        """
        mesh = rhutil.coercemesh(object_id, True)
        rc = mesh.GetNakedEdgePointStatus()
        return rc
    *)


    ///<summary>Makes a new mesh with vertices offset at a distance in the opposite
    ///  direction of the existing vertex normals</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<param name="distance">(float) The distance to offset</param>
    ///<returns>(Guid) identifier of the new mesh object</returns>
    static member MeshOffset(meshId:Guid, distance:float) : Guid =
        let mesh = Coerce.coercemesh(meshId, true)
        let offsetmesh = mesh.Offset(distance)
        if offsetmesh = null then failwithf "Rhino.Scripting Error:MeshOffset failed.  meshId:"%A" distance:"%A"" meshId distance
        let rc = Doc.Objects.AddMesh(offsetmesh)
        if rc=Guid.Empty then failwithf "Rhino.Scripting Error:Unable to add mesh to document.  meshId:"%A" distance:"%A"" meshId distance
        Doc.Views.Redraw()
        rc
    (*
    def MeshOffset(mesh_id, distance):
        """Makes a new mesh with vertices offset at a distance in the opposite
        direction of the existing vertex normals
        Parameters:
          mesh_id (guid): identifier of a mesh object
          distance (number, optional): the distance to offset
        Returns:
          guid: identifier of the new mesh object if successful
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          mesh = rs.GetObject("Select mesh to offset", rs.filter.mesh)
          rs.MeshOffset( mesh, 10.0 )
        See Also:
          IsMesh
        """
        mesh = rhutil.coercemesh(mesh_id, True)
        offsetmesh = mesh.Offset(distance)
        if offsetmesh is None: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddMesh(offsetmesh)
        if rc==System.Guid.Empty: raise Exception("unable to add mesh to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Creates polyline curve outlines of mesh objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of meshes to outline</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///View to use for outline direction</param>
    ///<returns>(Guid seq) polyline curve identifiers on success</returns>
    static member MeshOutline(objectIds:Guid seq, [<OPT;DEF(null)>]view:string) : Guid seq =
        let viewport = __viewhelper(view).MainViewport
        let meshes = ResizeArray()
        let mesh = Coerce.coercemesh(objectIds, false)
        if mesh then meshes.Add(mesh)
        else meshes <- [| for id in objectIds -> Coerce.coercemesh(id,true) |]
        let rc = ResizeArray()
        for mesh in meshes do
            let polylines = mesh.GetOutlines(viewport)
            if not <| polylines then continue
            for polyline in polylines do
                let id = Doc.Objects.AddPolyline(polyline)
                rc.Add(id)
        Doc.Views.Redraw()
        rc
    (*
    def MeshOutline(object_ids, view=None):
        """Creates polyline curve outlines of mesh objects
        Parameters:
          object_ids ([guid, ...]): identifiers of meshes to outline
          view (str, optional): view to use for outline direction
        Returns:
          list(guid, ...): polyline curve identifiers on success
        Example:
          import rhinoscriptsyntax as rs
          objs = rs.GetObjects("Select mesh objects to outline", rs.filter.mesh)
          if objs: rs.MeshOutline(objs)
        See Also:
          IsMesh
        """
        viewport = __viewhelper(view).MainViewport
        meshes = []
        mesh = rhutil.coercemesh(object_ids, False)
        if mesh: meshes.append(mesh)
        else: meshes = [rhutil.coercemesh(id,True) for id in object_ids]
        rc = []
        for mesh in meshes:
            polylines = mesh.GetOutlines(viewport)
            if not polylines: continue
            for polyline in polylines:
                id = scriptcontext.doc.Objects.AddPolyline(polyline)
                rc.append(id)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the number of quad faces of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) the number of quad mesh faces</returns>
    static member MeshQuadCount(objectId:Guid) : int =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.Faces.QuadCount
    (*
    def MeshQuadCount(object_id):
        """Returns the number of quad faces of a mesh object
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          number: the number of quad mesh faces if successful
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh )
          print "Quad faces:", rs.MeshQuadCount(obj)
          print "Triangle faces:", rs.MeshTriangleCount(obj)
          print "Total faces:", rs.MeshFaceCount(obj)
        See Also:
          MeshQuadCount
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.Faces.QuadCount
    *)


    ///<summary>Converts a mesh object's quad faces to triangles</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member MeshQuadsToTriangles(objectId:Guid) : bool =
        let mesh = Coerce.coercemesh(objectId, true)
        let rc = true
        if mesh.Faces.QuadCount>0 then
            rc <- mesh.Faces.ConvertQuadsToTriangles()
            if rc then
                let id = Coerce.coerceguid(objectId, true)
                Doc.Objects.Replace(id, mesh)
                Doc.Views.Redraw()
        rc
    (*
    def MeshQuadsToTriangles(object_id):
        """Converts a mesh object's quad faces to triangles
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True or False indicating success or failure
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh )
          if rs.MeshQuadCount(obj)>0:
              rs.MeshQuadsToTriangles(obj)
        See Also:
          
        """
        mesh = rhutil.coercemesh(object_id, True)
        rc = True
        if mesh.Faces.QuadCount>0:
            rc = mesh.Faces.ConvertQuadsToTriangles()
            if rc:
                id = rhutil.coerceguid(object_id, True)
                scriptcontext.doc.Objects.Replace(id, mesh)
                scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Duplicates each polygon in a mesh with a NURBS surface. The resulting
    ///  surfaces are then joined into a polysurface and added to the document</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="trimmedTriangles">(bool) Optional, Default Value: <c>true</c>
    ///If True, triangles in the mesh will be
    ///  represented by a trimmed plane</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input object</param>
    ///<returns>(Guid seq) identifiers for the new breps on success</returns>
    static member MeshToNurb(objectId:Guid, [<OPT;DEF(true)>]trimmedTriangles:bool, [<OPT;DEF(false)>]deleteInput:bool) : Guid seq =
        let mesh = Coerce.coercemesh(object_id, true)
        let pieces = mesh.SplitDisjointPieces()
        let breps = [| for piece in pieces -> Brep.CreateFromMesh(piece,trimmed_triangles) |]
        let rhobj = Coerce.coercerhinoobject(object_id, true, true)
        let attr = rhobj.Attributes
        let ids = [| for brep in breps -> Doc.Objects.AddBrep(brep, attr) |]
        if deleteInput then Doc.Objects.Delete(rhobj, true)
        Doc.Views.Redraw()
        ids
    (*
    def MeshToNurb(object_id, trimmed_triangles=True, delete_input=False):
        """Duplicates each polygon in a mesh with a NURBS surface. The resulting
        surfaces are then joined into a polysurface and added to the document
        Parameters:
          object_id (guid): identifier of a mesh object
          trimmed_triangles (bool, optional): if True, triangles in the mesh will be
            represented by a trimmed plane
          delete_input (bool, optional): delete input object
        Returns:
          list(guid, ...): identifiers for the new breps on success
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          if obj: rs.MeshToNurb(obj)
        See Also:
          IsMesh
          MeshFaces
          MeshVertices
        """
        mesh = rhutil.coercemesh(object_id, True)
        pieces = mesh.SplitDisjointPieces()
        breps = [Rhino.Geometry.Brep.CreateFromMesh(piece,trimmed_triangles) for piece in pieces]
        rhobj = rhutil.coercerhinoobject(object_id, True, True)
        attr = rhobj.Attributes
        ids = [scriptcontext.doc.Objects.AddBrep(brep, attr) for brep in breps]
        if delete_input: scriptcontext.doc.Objects.Delete(rhobj, True)
        scriptcontext.doc.Views.Redraw()
        return ids
    *)


    ///<summary>Returns number of triangular faces of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of triangular mesh faces</returns>
    static member MeshTriangleCount(objectId:Guid) : int =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.Faces.TriangleCount
    (*
    def MeshTriangleCount(object_id):
        """Returns number of triangular faces of a mesh
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          number: The number of triangular mesh faces if successful
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh )
          print "Quad faces:", rs.MeshQuadCount(obj)
          print "Triangle faces:", rs.MeshTriangleCount(obj)
          print "Total faces:", rs.MeshFaceCount(obj)
        See Also:
          IsMesh
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.Faces.TriangleCount
    *)


    ///<summary>Returns vertex colors of a mesh</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Drawing.Color) The current vertex colors</returns>
    static member MeshVertexColors(meshId:Guid) : Drawing.Color =
        let mesh = Coerce.coercemesh(meshId, true)
        let rc = .[mesh.VertexColors.[i] for i=0 to mesh.VertexColors.Count)]
        if colors=0 then rc
        if colors = null then
            mesh.VertexColors.Clear()
        else
            let color_count = Seq.length(colors)
            if color_count<>mesh.Vertices.Count then
                failwithf "Rhino.Scripting Error:Length of colors must match vertex count.  meshId:"%A" colors:"%A"" meshId colors
            let colors = [| for c in colors -> Coerce.coercecolor(c) |]
            mesh.VertexColors.Clear()
            for c in colors do mesh.VertexColors.Add(c)
        let id = Coerce.coerceguid(meshId, true)
        Doc.Objects.Replace(id, mesh)
        Doc.Views.Redraw()
        rc
    (*
    def MeshVertexColors(mesh_id, colors=0):
        """Returns or modifies vertex colors of a mesh
        Parameters:
          mesh_id (guid): identifier of a mesh object
          colors [color, ...], optional) A list of color values. Note, for each vertex, there must
            be a corresponding vertex color. If the value is None, then any
            existing vertex colors will be removed from the mesh
        Returns:
          color: if colors is not specified, the current vertex colors
          color: if colors is specified, the previous vertex colors
        Example:
          import rhinoscriptsyntax as rs
          import random
           
          def randomcolor():
              r = random.randint(0,255)
              g = random.randint(0,255)
              b = random.randint(0,255)
              return r,g,b
           
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          if obj:
              colors = []
              for i in range(rs.MeshVertexCount(obj)): colors.append( randomcolor() )
              rs.MeshVertexColors( obj, colors )
        See Also:
          MeshHasVertexColors
          MeshVertexCount
          MeshVertices
        """
        mesh = rhutil.coercemesh(mesh_id, True)
        rc = [mesh.VertexColors[i] for i in range(mesh.VertexColors.Count)]
        if colors==0: return rc
        if colors is None:
            mesh.VertexColors.Clear()
        else:
            color_count = len(colors)
            if color_count!=mesh.Vertices.Count:
                raise ValueError("length of colors must match vertex count")
            colors = [rhutil.coercecolor(c) for c in colors]
            mesh.VertexColors.Clear()
            for c in colors: mesh.VertexColors.Add(c)
        id = rhutil.coerceguid(mesh_id, True)
        scriptcontext.doc.Objects.Replace(id, mesh)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies vertex colors of a mesh</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<param name="colors">(Drawing.Color seq), optional) A list of color values. Note, for each vertex, there must
    ///  be a corresponding vertex color. If the value is None, then any
    ///  existing vertex colors will be removed from the mesh</param>
    ///<returns>(unit) unit</returns>
    static member MeshVertexColors(meshId:Guid, colors:Drawing.Color seq) : unit =
        let mesh = Coerce.coercemesh(meshId, true)
        let rc = .[mesh.VertexColors.[i] for i=0 to mesh.VertexColors.Count)]
        if colors=0 then rc
        if colors = null then
            mesh.VertexColors.Clear()
        else
            let color_count = Seq.length(colors)
            if color_count<>mesh.Vertices.Count then
                failwithf "Rhino.Scripting Error:Length of colors must match vertex count.  meshId:"%A" colors:"%A"" meshId colors
            let colors = [| for c in colors -> Coerce.coercecolor(c) |]
            mesh.VertexColors.Clear()
            for c in colors do mesh.VertexColors.Add(c)
        let id = Coerce.coerceguid(meshId, true)
        Doc.Objects.Replace(id, mesh)
        Doc.Views.Redraw()
        rc
    (*
    def MeshVertexColors(mesh_id, colors=0):
        """Returns or modifies vertex colors of a mesh
        Parameters:
          mesh_id (guid): identifier of a mesh object
          colors [color, ...], optional) A list of color values. Note, for each vertex, there must
            be a corresponding vertex color. If the value is None, then any
            existing vertex colors will be removed from the mesh
        Returns:
          color: if colors is not specified, the current vertex colors
          color: if colors is specified, the previous vertex colors
        Example:
          import rhinoscriptsyntax as rs
          import random
           
          def randomcolor():
              r = random.randint(0,255)
              g = random.randint(0,255)
              b = random.randint(0,255)
              return r,g,b
           
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          if obj:
              colors = []
              for i in range(rs.MeshVertexCount(obj)): colors.append( randomcolor() )
              rs.MeshVertexColors( obj, colors )
        See Also:
          MeshHasVertexColors
          MeshVertexCount
          MeshVertices
        """
        mesh = rhutil.coercemesh(mesh_id, True)
        rc = [mesh.VertexColors[i] for i in range(mesh.VertexColors.Count)]
        if colors==0: return rc
        if colors is None:
            mesh.VertexColors.Clear()
        else:
            color_count = len(colors)
            if color_count!=mesh.Vertices.Count:
                raise ValueError("length of colors must match vertex count")
            colors = [rhutil.coercecolor(c) for c in colors]
            mesh.VertexColors.Clear()
            for c in colors: mesh.VertexColors.Add(c)
        id = rhutil.coerceguid(mesh_id, True)
        scriptcontext.doc.Objects.Replace(id, mesh)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the vertex count of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of mesh vertices .</returns>
    static member MeshVertexCount(objectId:Guid) : int =
        let mesh = Coerce.coercemesh(objectId, true)
        mesh.Vertices.Count
    (*
    def MeshVertexCount(object_id):
        """Returns the vertex count of a mesh
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          number: The number of mesh vertices if successful.
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh )
          print "Vertex count: ", rs.MeshVertexCount(obj)
        See Also:
          IsMesh
          MeshFaceCount
          MeshFaces
          MeshVertices
        """
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.Vertices.Count
    *)


    ///<summary>Returns the mesh faces that share a specified mesh vertex</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<param name="vertexIndex">(int) Index of the mesh vertex to find faces for</param>
    ///<returns>(float seq) face indices on success</returns>
    static member MeshVertexFaces(meshId:Guid, vertexIndex:int) : float seq =
        let mesh = Coerce.coercemesh(mesh_id, true)
        mesh.Vertices.GetVertexFaces(vertexIndex)
    (*
    def MeshVertexFaces(mesh_id, vertex_index):
        """Returns the mesh faces that share a specified mesh vertex
        Parameters:
          mesh_id (guid): identifier of a mesh object
          vertex_index (number): index of the mesh vertex to find faces for
        Returns:
          list(number, ...): face indices on success
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          import random
          def TestMeshVertexFaces():
              mesh = rs.GetObject("Select mesh", rs.filter.mesh)
              vertices = rs.MeshVertices(mesh)
              meshfaces = rs.MeshFaceVertices(mesh)
              vertex = random.randint(0, len(vertices)-1) #some random vertex
              vertex_faces = rs.MeshVertexFaces(mesh, vertex )
              if vertex_faces:
                  rs.AddPoint( vertices[vertex] )
                  for face_index in vertex_faces:
                      face = meshfaces[face_index]
                      polyline = []
                      polyline.append( vertices[face[0]] )
                      polyline.append( vertices[face[1]] )
                      polyline.append( vertices[face[2]] )
                      if face[2]!=face[3]:
                          polyline.append( vertices[face[3]] )
                      polyline.append( polyline[0] )
                      rs.AddPolyline(polyline)
           
          TestMeshVertexFaces()
        See Also:
          MeshFaces
          MeshFaceVertices
          MeshVertices
        """
        mesh = rhutil.coercemesh(mesh_id, True)
        return mesh.Vertices.GetVertexFaces(vertex_index)
    *)


    ///<summary>Returns the vertex unit normal for each vertex of a mesh</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Vector3d seq) of vertex normals, (empty list if no normals exist)</returns>
    static member MeshVertexNormals(meshId:Guid) : Vector3d seq =
        let mesh = Coerce.coercemesh(meshId, true)
        let count = mesh.Normals.Count
        if count<1 then []
        .[Vector3d(mesh.Normals.[i]) for i in xrange(count)]
    (*
    def MeshVertexNormals(mesh_id):
        """Returns the vertex unit normal for each vertex of a mesh
        Parameters:
          mesh_id (guid): identifier of a mesh object
        Returns:
          list(vector, ...): of vertex normals, (empty list if no normals exist)
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          normals = rs.MeshVertexNormals(obj)
          if normals:
              for normal in normals: print normal
        See Also:
          MeshHasVertexNormals
          MeshVertexCount
          MeshVertices
        """
        mesh = rhutil.coercemesh(mesh_id, True)
        count = mesh.Normals.Count
        if count<1: return []
        return [Rhino.Geometry.Vector3d(mesh.Normals[i]) for i in xrange(count)]
    *)


    ///<summary>Returns the vertices of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d seq) vertex points in the mesh</returns>
    static member MeshVertices(objectId:Guid) : Point3d seq =
        let mesh = Coerce.coercemesh(objectId, true)
        let count = mesh.Vertices.Count
        let rc = ResizeArray()
        for i in xrange(count) do
            let vertex = mesh.Vertices.[i]
            rc.Add(Point3d(vertex))
        rc
    (*
    def MeshVertices(object_id):
        """Returns the vertices of a mesh
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          list(point, ...): vertex points in the mesh
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          vertices = rs.MeshVertices(obj)
          if vertices: rs.AddPointCloud(vertices)
        See Also:
          IsMesh
          MeshFaceCount
          MeshFaces
          MeshVertexCount
        """
        mesh = rhutil.coercemesh(object_id, True)
        count = mesh.Vertices.Count
        rc = []
        for i in xrange(count):
            vertex = mesh.Vertices[i]
            rc.append(Rhino.Geometry.Point3d(vertex))
        return rc
    *)


    ///<summary>Returns the approximate volume of one or more closed meshes</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more mesh objects</param>
    ///<returns>(float * float * float) containing 3 velues  where
    ///  [0] = number of meshes used in volume calculation
    ///  [1] = total volume of all meshes
    ///  [2] = the error estimate</returns>
    static member MeshVolume(objectIds:Guid seq) : float * float * float =
        let id = Coerce.coerceguid(objectIds)
        if id then objectIds <- .[id]
        let meshes_used = 0
        let total_volume = 0.0
        let error_estimate = 0.0
        for id in objectIds do
            let mesh = Coerce.coercemesh(id, true)
            let mp = VolumeMassProperties.Compute(mesh)
            if mp then
                meshes_used += 1
                total_volume += mp.Volume
                error_estimate += mp.VolumeError
        if meshes_used=0 then failwithf "Rhino.Scripting Error:MeshVolume failed.  objectIds:"%A"" objectIds
        meshes_used, total_volume, error_estimate
    (*
    def MeshVolume(object_ids):
        """Returns the approximate volume of one or more closed meshes
        Parameters:
          object_ids ([guid, ...]): identifiers of one or more mesh objects
        Returns:
          tuple(number, number, number): containing 3 velues if successful where
               [0] = number of meshes used in volume calculation
               [1] = total volume of all meshes
               [2] = the error estimate
          None: if not successful
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh )
          if obj and rs.IsMeshClosed(obj):
              volume = rs.MeshVolume(obj)
              if volume: print "Mesh volume:", volume[1]
        See Also:
          IsMeshClosed
          MeshArea
        """
        id = rhutil.coerceguid(object_ids)
        if id: object_ids = [id]
        meshes_used = 0
        total_volume = 0.0
        error_estimate = 0.0
        for id in object_ids:
            mesh = rhutil.coercemesh(id, True)
            mp = Rhino.Geometry.VolumeMassProperties.Compute(mesh)
            if mp:
                meshes_used += 1
                total_volume += mp.Volume
                error_estimate += mp.VolumeError
        if meshes_used==0: return scriptcontext.errorhandler()
        return meshes_used, total_volume, error_estimate
    *)


    ///<summary>Calculates the volume centroid of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d) Point3d representing the volume centroid</returns>
    static member MeshVolumeCentroid(objectId:Guid) : Point3d =
        let mesh = Coerce.coercemesh(objectId, true)
        let mp = VolumeMassProperties.Compute(mesh)
        if mp then mp.Centroid
        failwithf "Rhino.Scripting Error:MeshVolumeCentroid failed.  objectId:"%A"" objectId
    (*
    def MeshVolumeCentroid(object_id):
        """Calculates the volume centroid of a mesh
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          point: Point3d representing the volume centroid
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh )
          centroid = rs.MeshVolumeCentroid(obj)
          rs.AddPoint( centroid )
        See Also:
          IsMesh
          MeshArea
          MeshAreaCentroid
          MeshVolume
        """
        mesh = rhutil.coercemesh(object_id, True)
        mp = Rhino.Geometry.VolumeMassProperties.Compute(mesh)
        if mp: return mp.Centroid
        return scriptcontext.errorhandler()
    *)


    ///<summary>Pulls a curve to a mesh. The function makes a polyline approximation of
    ///  the input curve and gets the closest point on the mesh for each point on
    ///  the polyline. Then it "connects the points" to create a polyline on the mesh</summary>
    ///<param name="meshId">(Guid) Identifier of mesh that pulls</param>
    ///<param name="curveId">(Guid) Identifier of curve to pull</param>
    ///<returns>(Guid) identifier new curve on success</returns>
    static member PullCurveToMesh(meshId:Guid, curveId:Guid) : Guid =
        let mesh = Coerce.coercemesh(mesh_id, true)
        let curve = Coerce.coercecurve(curveId, -1, true)
        let tol = Doc.ModelAbsoluteTolerance
        let polyline = curve.PullToMesh(mesh, tol)
        if not <| polyline then failwithf "Rhino.Scripting Error:PullCurveToMesh failed.  meshId:"%A" curveId:"%A"" meshId curveId
        let rc = Doc.Objects.AddCurve(polyline)
        if rc=Guid.Empty then failwithf "Rhino.Scripting Error:Unable to add polyline to document.  meshId:"%A" curveId:"%A"" meshId curveId
        Doc.Views.Redraw()
        rc
    (*
    def PullCurveToMesh(mesh_id, curve_id):
        """Pulls a curve to a mesh. The function makes a polyline approximation of
        the input curve and gets the closest point on the mesh for each point on
        the polyline. Then it "connects the points" to create a polyline on the mesh
        Parameters:
          mesh_id (guid): identifier of mesh that pulls
          curve_id (guid): identifier of curve to pull
        Returns:
          guid: identifier new curve on success
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          mesh = rs.GetObject("Select mesh that pulls", rs.filter.mesh)
          curve = rs.GetObject("Select curve to pull", rs.filter.curve)
          rs.PullCurveToMesh( mesh, curve )
        See Also:
          IsMesh
        """
        mesh = rhutil.coercemesh(mesh_id, True)
        curve = rhutil.coercecurve(curve_id, -1, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        polyline = curve.PullToMesh(mesh, tol)
        if not polyline: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddCurve(polyline)
        if rc==System.Guid.Empty: raise Exception("unable to add polyline to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Splits up a mesh into its unconnected pieces</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete the input object</param>
    ///<returns>(Guid seq) identifiers for the new meshes</returns>
    static member SplitDisjointMesh(objectId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid seq =
        let mesh = Coerce.coercemesh(object_id, true)
        let pieces = mesh.SplitDisjointPieces()
        let rc = [| for piece in pieces -> Doc.Objects.AddMesh(piece) |]
        if rc && deleteInput then
            let id = Coerce.coerceguid(object_id, true)
            Doc.Objects.Delete(id, true)
        Doc.Views.Redraw()
        rc
    (*
    def SplitDisjointMesh(object_id, delete_input=False):
        """Splits up a mesh into its unconnected pieces
        Parameters:
          object_id (guid): identifier of a mesh object
          delete_input (bool, optional): delete the input object
        Returns:
          list(guid, ...): identifiers for the new meshes
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          if rs.DisjointMeshCount(obj)>0: rs.SplitDisjointMesh(obj)
        See Also:
          IsMesh
          DisjointMeshCount
        """
        mesh = rhutil.coercemesh(object_id, True)
        pieces = mesh.SplitDisjointPieces()
        rc = [scriptcontext.doc.Objects.AddMesh(piece) for piece in pieces]
        if rc and delete_input:
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Fixes inconsistencies in the directions of faces of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(float) the number of faces that were modified</returns>
    static member UnifyMeshNormals(objectId:Guid) : float =
        let mesh = Coerce.coercemesh(objectId, true)
        let rc = mesh.UnifyNormals()
        if rc>0 then
            let id = Coerce.coerceguid(objectId, true)
            Doc.Objects.Replace(id, mesh)
            Doc.Views.Redraw()
        rc
    (*
    def UnifyMeshNormals(object_id):
        """Fixes inconsistencies in the directions of faces of a mesh
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          number: the number of faces that were modified
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select mesh", rs.filter.mesh)
          if rs.IsMesh(obj): rs.UnifyMeshNormals(obj)
        See Also:
          IsMesh
        """
        mesh = rhutil.coercemesh(object_id, True)
        rc = mesh.UnifyNormals()
        if rc>0:
            id = rhutil.coerceguid(object_id, True)
            scriptcontext.doc.Objects.Replace(id, mesh)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


