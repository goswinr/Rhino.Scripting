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
        failNotImpl()

    ///<summary>Creates a planar mesh from a closed, planar curve</summary>
    ///<param name="objectId">(Guid) Identifier of a closed, planar curve</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///If True, delete the input curve defined by objectId</param>
    ///<returns>(Guid) id of the new mesh on success</returns>
    static member AddPlanarMesh(objectId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        failNotImpl()

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
        failNotImpl()

    ///<summary>Returns number of meshes that could be created by calling SplitDisjointMesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of meshes that could be created</returns>
    static member DisjointMeshCount(objectId:Guid) : int =
        failNotImpl()

    ///<summary>Creates curves that duplicates a mesh border</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Guid seq) list of curve ids on success</returns>
    static member DuplicateMeshBorder(meshId:Guid) : Guid seq =
        failNotImpl()

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
        failNotImpl()

    ///<summary>Verifies if an object is a mesh</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsMesh(objectId:Guid) : bool =
        failNotImpl()

    ///<summary>Verifies a mesh object is closed</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsMeshClosed(objectId:Guid) : bool =
        failNotImpl()

    ///<summary>Verifies a mesh object is manifold. A mesh for which every edge is shared
    ///  by at most two faces is called manifold. If a mesh has at least one edge
    ///  that is shared by more than two faces, then that mesh is called non-manifold</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsMeshManifold(objectId:Guid) : bool =
        failNotImpl()

    ///<summary>Verifies a point is on a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="point">(Point3d) Test point</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsPointOnMesh(objectId:Guid, point:Point3d) : bool =
        failNotImpl()

    ///<summary>Joins two or or more mesh objects together</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of two or more mesh objects</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input after joining</param>
    ///<returns>(Guid) identifier of newly created mesh on success</returns>
    static member JoinMeshes(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        failNotImpl()

    ///<summary>Returns approximate area of one or more mesh objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more mesh objects</param>
    ///<returns>(float * float * float)   where
    ///    [0] = number of meshes used in calculation
    ///    [1] = total area of all meshes
    ///    [2] = the error estimate</returns>
    static member MeshArea(objectIds:Guid seq) : float * float * float =
        failNotImpl()

    ///<summary>Calculates the area centroid of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d) representing the area centroid</returns>
    static member MeshAreaCentroid(objectId:Guid) : Point3d =
        failNotImpl()

    ///<summary>Performs boolean difference operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid) Input0 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="input1">(Guid) Input1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///A positive tolerance value, or None to use the default of the document.</param>
    ///<returns>(Guid seq) identifiers of newly created meshes</returns>
    static member MeshBooleanDifference(input0:Guid, input1:Guid, [<OPT;DEF(true)>]deleteInput:bool, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        failNotImpl()

    ///<summary>Performs boolean intersection operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid) Input0 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="input1">(Guid) Input1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid seq) identifiers of new meshes on success</returns>
    static member MeshBooleanIntersection(input0:Guid, input1:Guid, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        failNotImpl()

    ///<summary>Performs boolean split operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid) Input0 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="input1">(Guid) Input1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid seq) identifiers of new meshes on success</returns>
    static member MeshBooleanSplit(input0:Guid, input1:Guid, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        failNotImpl()

    ///<summary>Performs boolean union operation on a set of input meshes</summary>
    ///<param name="meshIds">(Guid seq) Identifiers of meshes</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid seq) identifiers of new meshes</returns>
    static member MeshBooleanUnion(meshIds:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        failNotImpl()

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
        failNotImpl()

    ///<summary>Returns the center of each face of the mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d seq) points defining the center of each face</returns>
    static member MeshFaceCenters(meshId:Guid) : Point3d seq =
        failNotImpl()

    ///<summary>Returns total face count of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) the number of mesh faces</returns>
    static member MeshFaceCount(objectId:Guid) : int =
        failNotImpl()

    ///<summary>Returns the face unit normal for each face of a mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Vector3d seq) 3D vectors that define the face unit normals of the mesh</returns>
    static member MeshFaceNormals(meshId:Guid) : Vector3d seq =
        failNotImpl()

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
        failNotImpl()

    ///<summary>Returns the vertex indices of all faces of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(float seq) containing tuples of 4 numbers that define the vertex indices for
    ///  each face of the mesh. Both quad and triangle faces are returned. If the
    ///  third and fourth vertex indices are identical, the face is a triangle.</returns>
    static member MeshFaceVertices(objectId:Guid) : float seq =
        failNotImpl()

    ///<summary>Verifies a mesh object has face normals</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasFaceNormals(objectId:Guid) : bool =
        failNotImpl()

    ///<summary>Verifies a mesh object has texture coordinates</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasTextureCoordinates(objectId:Guid) : bool =
        failNotImpl()

    ///<summary>Verifies a mesh object has vertex colors</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasVertexColors(objectId:Guid) : bool =
        failNotImpl()

    ///<summary>Verifies a mesh object has vertex normals</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasVertexNormals(objectId:Guid) : bool =
        failNotImpl()

    ///<summary>Calculates the intersections of a mesh object with another mesh object</summary>
    ///<param name="mesh1">(Guid) Mesh1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="mesh2">(Guid) Mesh2 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///The intersection tolerance</param>
    ///<returns>(Point3d seq) of points that define the vertices of the intersection curves</returns>
    static member MeshMeshIntersection(mesh1:Guid, mesh2:Guid, [<OPT;DEF(null)>]tolerance:float) : Point3d seq =
        failNotImpl()

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
        failNotImpl()

    ///<summary>Makes a new mesh with vertices offset at a distance in the opposite
    ///  direction of the existing vertex normals</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<param name="distance">(float) The distance to offset</param>
    ///<returns>(Guid) identifier of the new mesh object</returns>
    static member MeshOffset(meshId:Guid, distance:float) : Guid =
        failNotImpl()

    ///<summary>Creates polyline curve outlines of mesh objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of meshes to outline</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///View to use for outline direction</param>
    ///<returns>(Guid seq) polyline curve identifiers on success</returns>
    static member MeshOutline(objectIds:Guid seq, [<OPT;DEF(null)>]view:string) : Guid seq =
        failNotImpl()

    ///<summary>Returns the number of quad faces of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) the number of quad mesh faces</returns>
    static member MeshQuadCount(objectId:Guid) : int =
        failNotImpl()

    ///<summary>Converts a mesh object's quad faces to triangles</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member MeshQuadsToTriangles(objectId:Guid) : bool =
        failNotImpl()

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
        failNotImpl()

    ///<summary>Returns number of triangular faces of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of triangular mesh faces</returns>
    static member MeshTriangleCount(objectId:Guid) : int =
        failNotImpl()

    ///<summary>Returns vertex colors of a mesh</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Drawing.Color) The current vertex colors</returns>
    static member MeshVertexColors(meshId:Guid) : Drawing.Color =
        failNotImpl()

    ///<summary>Modifies vertex colors of a mesh</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<param name="colors">(Drawing.Color seq), optional) A list of color values. Note, for each vertex, there must
    ///  be a corresponding vertex color. If the value is None, then any
    ///  existing vertex colors will be removed from the mesh</param>
    ///<returns>(unit) unit</returns>
    static member MeshVertexColors(meshId:Guid, colors:Drawing.Color seq) : unit =
        failNotImpl()

    ///<summary>Returns the vertex count of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of mesh vertices .</returns>
    static member MeshVertexCount(objectId:Guid) : int =
        failNotImpl()

    ///<summary>Returns the mesh faces that share a specified mesh vertex</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<param name="vertexIndex">(int) Index of the mesh vertex to find faces for</param>
    ///<returns>(float seq) face indices on success</returns>
    static member MeshVertexFaces(meshId:Guid, vertexIndex:int) : float seq =
        failNotImpl()

    ///<summary>Returns the vertex unit normal for each vertex of a mesh</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Vector3d seq) of vertex normals, (empty list if no normals exist)</returns>
    static member MeshVertexNormals(meshId:Guid) : Vector3d seq =
        failNotImpl()

    ///<summary>Returns the vertices of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d seq) vertex points in the mesh</returns>
    static member MeshVertices(objectId:Guid) : Point3d seq =
        failNotImpl()

    ///<summary>Returns the approximate volume of one or more closed meshes</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more mesh objects</param>
    ///<returns>(float * float * float) containing 3 velues  where
    ///  [0] = number of meshes used in volume calculation
    ///  [1] = total volume of all meshes
    ///  [2] = the error estimate</returns>
    static member MeshVolume(objectIds:Guid seq) : float * float * float =
        failNotImpl()

    ///<summary>Calculates the volume centroid of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d) Point3d representing the volume centroid</returns>
    static member MeshVolumeCentroid(objectId:Guid) : Point3d =
        failNotImpl()

    ///<summary>Pulls a curve to a mesh. The function makes a polyline approximation of
    ///  the input curve and gets the closest point on the mesh for each point on
    ///  the polyline. Then it "connects the points" to create a polyline on the mesh</summary>
    ///<param name="meshId">(Guid) Identifier of mesh that pulls</param>
    ///<param name="curveId">(Guid) Identifier of curve to pull</param>
    ///<returns>(Guid) identifier new curve on success</returns>
    static member PullCurveToMesh(meshId:Guid, curveId:Guid) : Guid =
        failNotImpl()

    ///<summary>Splits up a mesh into its unconnected pieces</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete the input object</param>
    ///<returns>(Guid seq) identifiers for the new meshes</returns>
    static member SplitDisjointMesh(objectId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid seq =
        failNotImpl()

    ///<summary>Fixes inconsistencies in the directions of faces of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(float) the number of faces that were modified</returns>
    static member UnifyMeshNormals(objectId:Guid) : float =
        failNotImpl()

