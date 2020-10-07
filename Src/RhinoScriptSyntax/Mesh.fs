namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry

open System.Collections.Generic
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

 
[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsMesh =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    [<Extension>]
    ///<summary>Add a mesh object to the document</summary>
    ///<param name="vertices">(Point3d seq) List of 3D points defining the vertices of the mesh</param>
    ///<param name="faceVertices">(int IList seq) List containing lists of 3 or 4 numbers that define the
    ///    vertex indices for each face of the mesh. If the third a fourth vertex
    ///      indices of a face are identical, a triangular face will be created</param>
    ///<param name="vertexNormals">(Vector3f seq) Optional, List of 3D vectors defining the vertex normals of
    ///    the mesh. Note, for every vertex, there must be a corresponding vertex
    ///    normal</param>
    ///<param name="textureCoordinates">(Point2f seq) Optional, List of 2D texture coordinates. For every
    ///    vertex, there must be a corresponding texture coordinate</param>
    ///<param name="vertexColors">(Drawing.Color seq) Optional, A list of color values. For every vertex,
    ///    there must be a corresponding vertex color</param>
    ///<returns>(Guid) Identifier of the new object</returns>
    static member AddMesh( vertices:Point3d seq, //TODO how to construct Ngon Mesh ???
                           faceVertices:seq<IList<int>>, // TODO test if nested arrays are accepted
                           [<OPT;DEF(null:Vector3f seq)>]vertexNormals:Vector3f seq,
                           [<OPT;DEF(null:Point2f seq)>]textureCoordinates:Point2f seq,
                           [<OPT;DEF(null:Drawing.Color seq)>]vertexColors:Drawing.Color seq) : Guid =
        let mesh = new Mesh()
        for pt in vertices do
            mesh.Vertices.Add(pt) |> ignore

        for face in faceVertices do
            let l = Seq.length(face)
            if l = 3 then
                mesh.Faces.AddFace(face.[0], face.[1], face.[2]) |> ignore
            elif l = 4 then 
                mesh.Faces.AddFace(face.[0], face.[1], face.[2], face.[3]) |> ignore
            else 
                Error.Raise <| sprintf "RhinoScriptSyntax.AddMesh: Expected 3 or 4 indices for a face but got %d" l

        if notNull vertexNormals then
            let count = Seq.length(vertexNormals)
            let normals = Array.zeroCreate count
            for i, normal in Seq.indexed(vertexNormals) do
                normals.[i] <- normal
            mesh.Normals.SetNormals(normals)    |> ignore

        if notNull textureCoordinates then
            let count = Seq.length(textureCoordinates)
            let tcs = Array.zeroCreate count
            for i, tc in Seq.indexed(textureCoordinates) do
                tcs.[i] <-  tc
            mesh.TextureCoordinates.SetTextureCoordinates(tcs)  |> ignore

        if notNull vertexColors then
            let count = Seq.length(vertexColors)
            let colors = Array.zeroCreate count
            for i, color in Seq.indexed(vertexColors) do
                //colors.[i] = RhinoScriptSyntax.Coercecolor(color)
            mesh.VertexColors.SetColors(colors)   |>   ignore

        let rc = Doc.Objects.AddMesh(mesh)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.Unable to add mesh to document.  vertices:'%A' faceVertices:'%A' vertexNormals:'%A' textureCoordinates:'%A' vertexColors:'%A'" vertices faceVertices vertexNormals textureCoordinates vertexColors
        Doc.Views.Redraw()
        rc
    
    [<Extension>]
    ///<summary>Add a mesh object to the document</summary>
    ///<param name="vertices">(Point3d seq) List of 3D points defining the vertices of the mesh</param>
    ///<param name="faceVertices">(int*int*int*int seq) Tuple  of  4 integers that define the
    ///    vertex indices for each face of the mesh. If the third a fourth vertex
    ///      indices of a face are identical, a triangular face will be created</param>
    ///<param name="vertexNormals">(Vector3f seq) Optional, List of 3D vectors defining the vertex normals of
    ///    the mesh. Note, for every vertex, there must be a corresponding vertex
    ///    normal</param>
    ///<param name="textureCoordinates">(Point2f seq) Optional, List of 2D texture coordinates. For every
    ///    vertex, there must be a corresponding texture coordinate</param>
    ///<param name="vertexColors">(Drawing.Color seq) Optional, A list of color values. For every vertex,
    ///    there must be a corresponding vertex color</param>
    ///<returns>(Guid) Identifier of the new object</returns>
    static member AddMesh( vertices:Point3d seq, //TODO how to construct Ngon Mesh ???
                           faceVertices:seq<int*int*int*int>, 
                           [<OPT;DEF(null:Vector3f seq)>]vertexNormals:Vector3f seq,
                           [<OPT;DEF(null:Point2f seq)>]textureCoordinates:Point2f seq,
                           [<OPT;DEF(null:Drawing.Color seq)>]vertexColors:Drawing.Color seq) : Guid =
        let mesh = new Mesh()
        for pt in vertices do
            mesh.Vertices.Add(pt) |> ignore

        for face in faceVertices do
            let a,b,c,d = face 
            if c = d then
                mesh.Faces.AddFace(a,b,c) |> ignore
            else
                mesh.Faces.AddFace(a,b,c,d) |> ignore

        if notNull vertexNormals then
            let count = Seq.length(vertexNormals)
            let normals = Array.zeroCreate count
            for i, normal in Seq.indexed(vertexNormals) do
                normals.[i] <- normal
            mesh.Normals.SetNormals(normals)    |> ignore

        if notNull textureCoordinates then
            let count = Seq.length(textureCoordinates)
            let tcs = Array.zeroCreate count
            for i, tc in Seq.indexed(textureCoordinates) do
                tcs.[i] <-  tc
            mesh.TextureCoordinates.SetTextureCoordinates(tcs)  |> ignore

        if notNull vertexColors then
            let count = Seq.length(vertexColors)
            let colors = Array.zeroCreate count
            for i, color in Seq.indexed(vertexColors) do
                //colors.[i] = RhinoScriptSyntax.Coercecolor(color)
            mesh.VertexColors.SetColors(colors)   |>   ignore

        let rc = Doc.Objects.AddMesh(mesh)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.Unable to add mesh to document.  vertices:'%A' faceVertices:'%A' vertexNormals:'%A' textureCoordinates:'%A' vertexColors:'%A'" vertices faceVertices vertexNormals textureCoordinates vertexColors
        Doc.Views.Redraw()
        rc



    [<Extension>]
    ///<summary>Creates a new mesh with just one quad face </summary>
    ///<param name="pointA">(Point3d) First corner point</param>
    ///<param name="pointB">(Point3d) Second  corner point</param>
    ///<param name="pointC">(Point3d) Third corner point</param>
    ///<param name="pointD">(Point3d) Fourth corner point</param>
    ///<returns>(Guid) The identifier of the new mesh</returns>
    static member AddMeshQuad(pointA:Point3d , pointB:Point3d , pointC: Point3d , pointD: Point3d) : Guid =
          let mesh = new Mesh()
          mesh.Vertices.Add(pointA) |> ignore
          mesh.Vertices.Add(pointB) |> ignore
          mesh.Vertices.Add(pointC) |> ignore
          mesh.Vertices.Add(pointD) |> ignore
          mesh.Faces.AddFace(0,1,2,3) |> ignore
          let rc = Doc.Objects.AddMesh(mesh)
          if rc = Guid.Empty then  Error.Raise <| sprintf "RhinoScriptSyntax.AddMeshQuad failed.  points:'%A, %A, %A and %A" pointA pointB pointC pointD
          Doc.Views.Redraw()
          rc

    [<Extension>]
    ///<summary>Creates a new mesh with just one triangle face </summary>
    ///<param name="pointA">(Point3d) First corner point</param>
    ///<param name="pointB">(Point3d) Second  corner point</param>
    ///<param name="pointC">(Point3d) Third corner point</param>
    ///<returns>(Guid) The identifier of the new mesh</returns>
    static member AddMeshTriangle(pointA:Point3d , pointB:Point3d , pointC: Point3d ) : Guid =
          let mesh = new Mesh()
          mesh.Vertices.Add(pointA) |> ignore
          mesh.Vertices.Add(pointB) |> ignore
          mesh.Vertices.Add(pointC) |> ignore
          mesh.Faces.AddFace(0,1,2) |> ignore
          let rc = Doc.Objects.AddMesh(mesh)
          if rc = Guid.Empty then  Error.Raise <| sprintf "RhinoScriptSyntax.AddMeshQuad failed.  points:'%A, %A and %A" pointA pointB pointC 
          Doc.Views.Redraw()
          rc

    [<Extension>]
    ///<summary>Creates a planar mesh from a closed, planar curve</summary>
    ///<param name="objectId">(Guid) Identifier of a closed, planar curve</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    If True, delete the input curve defined by objectId</param>
    ///<returns>(Guid) id of the new mesh</returns>
    static member AddPlanarMesh(objectId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(objectId)
        let tolerance = Doc.ModelAbsoluteTolerance
        let mesh = Mesh.CreateFromPlanarBoundary(curve, MeshingParameters.Default, tolerance)
        if isNull mesh then Error.Raise <| sprintf "RhinoScriptSyntax.AddPlanarMesh failed.  objectId:'%A' deleteInput:'%A'" objectId deleteInput
        if deleteInput then
            let ob = RhinoScriptSyntax.CoerceGuid(objectId)
            if not<| Doc.Objects.Delete(ob, true) then Error.Raise <| sprintf "RhinoScriptSyntax.AddPlanarMesh failed to delete input.  objectId:'%A' deleteInput:'%A'" objectId deleteInput
        let rc = Doc.Objects.AddMesh(mesh)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.Unable to add mesh to document.  objectId:'%A' deleteInput:'%A'" objectId deleteInput
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Calculates the intersection of a curve object and a mesh object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="meshId">(Guid) Identifier or a mesh object</param>
    ///<returns>(Point3d array * int array) two arrays as tuple:
    ///        [0] = point of intersection
    ///        [1] = mesh face index where intersection lies</returns>
    static member CurveMeshIntersection( curveId:Guid,
                                         meshId:Guid) : array<Point3d>*array<int> =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let tolerance = Doc.ModelAbsoluteTolerance
        let polylinecurve = curve.ToPolyline(0 , 0 , 0.0 , 0.0 , 0.0, tolerance , 0.0 , 0.0 , true)
        let pts, faceids = Intersect.Intersection.MeshPolyline(mesh, polylinecurve)
        if isNull pts then Error.Raise <| sprintf "RhinoScriptSyntax.CurveMeshIntersection failed.  curveId:'%A' meshId:'%A' " curveId meshId
        pts, faceids


    [<Extension>]
    ///<summary>Returns number of meshes that could be created by calling SplitDisjointMesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of meshes that could be created</returns>
    static member DisjointMeshCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.DisjointMeshCount


    [<Extension>]
    ///<summary>Creates curves that duplicates a mesh border</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Guid ResizeArray) list of curve ids</returns>
    static member DuplicateMeshBorder(meshId:Guid) : Guid ResizeArray =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let polylines = mesh.GetNakedEdges()
        let rc = ResizeArray()
        if notNull polylines then
            for polyline in polylines do
                let objectId = Doc.Objects.AddPolyline(polyline)
                if objectId <> Guid.Empty then rc.Add(objectId)
        if rc.Count <> 0 then Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Explodes a mesh object, or mesh objects int submeshes. A submesh is a
    ///    collection of mesh faces that are contained within a closed loop of
    ///    unwelded mesh edges. Unwelded mesh edges are where the mesh faces that
    ///    share the edge have unique mesh vertices (not mesh topology vertices)
    ///    at both ends of the edge</summary>
    ///<param name="meshIds">(Guid seq) List of mesh identifiers</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the input meshes</param>
    ///<returns>(Guid ResizeArray) List of resulting objects after explode</returns>
    static member ExplodeMeshes(meshIds:Guid seq, [<OPT;DEF(false)>]delete:bool) : Guid ResizeArray =
        //id = RhinoScriptSyntax.Coerceguid(meshIds)
        let rc = ResizeArray()
        for meshid in meshIds do
            let mesh = RhinoScriptSyntax.CoerceMesh(meshid)
            if notNull mesh then
                let submeshes = mesh.ExplodeAtUnweldedEdges()
                if notNull submeshes then
                    for submesh in submeshes do
                        let objectId = Doc.Objects.AddMesh(submesh)
                        if objectId <> Guid.Empty then rc.Add(objectId)
                if delete then
                    Doc.Objects.Delete(meshid, true)|> ignore
        if rc.Count>0 then Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Verifies if an object is a mesh</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsMesh(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceMesh(objectId)
        |> Option.isSome


    [<Extension>]
    ///<summary>Verifies a mesh object is closed</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsMeshClosed(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceMesh(objectId) with
        | Some mesh -> mesh.IsClosed
        | None -> false


    [<Extension>]
    ///<summary>Verifies a mesh object is manifold. A mesh for which every edge is shared
    ///    by at most two faces is called manifold. If a mesh has at least one edge
    ///    that is shared by more than two faces, then that mesh is called non-manifold</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True, otherwise False</returns>
    static member IsMeshManifold(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceMesh(objectId) with
        | Some mesh -> mesh.IsManifold(true)  |> t1
        | None -> false



    [<Extension>]
    ///<summary>Verifies a point is on a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="point">(Point3d) Test point</param>
    ///<param name="tolerance">(float) Optional, Defalut Value <c>RhinoMath.SqrtEpsilon</c>
    ///    The testing tolerance</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsPointOnMesh(    objectId:Guid,
                                    point:Point3d,
                                    [<OPT;DEF(0.0)>]tolerance:float): bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        let maxdistance = ifZero1 tolerance RhinoMath.SqrtEpsilon
        let pt = ref Point3d.Origin
        let face = mesh.ClosestPoint(point, pt, maxdistance)
        face>=0

    
    [<Extension>]
    ///<summary>Joins two or or more mesh objects together</summary>
    ///<param name="meshes">(Mesh seq) Mesh objects</param>
    ///<returns>(Mesh) newly created Mesh</returns>
    static member JoinMeshes(meshes:Mesh seq, [<OPT;DEF(false)>]deleteInput:bool) : Mesh =
        let joinedmesh = new Mesh()
        joinedmesh.Append(meshes)
        joinedmesh

    [<Extension>]
    ///<summary>Joins two or or more mesh objects together</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of two or more mesh objects</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input after joining</param>
    ///<returns>(Guid) identifier of newly created mesh</returns>
    static member JoinMeshes(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let meshes =  resizeArray { for objectId in objectIds do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let joinedmesh = new Mesh()
        joinedmesh.Append(meshes)
        let rc = Doc.Objects.AddMesh(joinedmesh)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.Failed to join Meshes %A" objectIds
        if deleteInput then
            for objectId in objectIds do
                //guid = RhinoScriptSyntax.Coerceguid(objectId)
                Doc.Objects.Delete(objectId, true) |> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Returns approximate area of onemesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh objects</param>
    ///<returns>(float) total area of mesh</returns>
    static member MeshArea(objectId:Guid ) :float =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mp = AreaMassProperties.Compute(mesh)
        if notNull mp then
            mp.Area
        else
            Error.Raise <| sprintf "RhinoScriptSyntax.MeshArea failed.  objectId:'%A'" objectId



    [<Extension>]
    ///<summary>Calculates the area centroid of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d) representing the area centroid</returns>
    static member MeshAreaCentroid(objectId:Guid) : Point3d =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mp = AreaMassProperties.Compute(mesh)
        if mp|> isNull  then Error.Raise <| sprintf "RhinoScriptSyntax.MeshAreaCentroid failed.  objectId:'%A'" objectId
        mp.Centroid


    [<Extension>]
    ///<summary>Performs boolean difference operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid seq) Meshes to subtract from</param>
    ///<param name="input1">(Guid seq) Meshes to subtract with</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input meshes</param>
    ///<returns>(Guid ResizeArray) identifiers of newly created meshes</returns>
    static member MeshBooleanDifference( input0:Guid seq,
                                         input1:Guid seq,
                                         [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let meshes0 =  resizeArray { for objectId in input0 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let meshes1 =  resizeArray { for objectId in input1 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        if meshes0.Count = 0 || meshes1.Count = 0 then Error.Raise <| sprintf "RhinoScriptSyntax.Rhino.Scripting.MeshBooleanDifference: No meshes to work with.  input0:'%A' input1:'%A' deleteInput:'%A' " input0 input1 deleteInput
        let newmeshes = Mesh.CreateBooleanDifference  (meshes0, meshes1)
        let rc = ResizeArray()
        for mesh in newmeshes do
            let objectId = Doc.Objects.AddMesh(mesh)
            if objectId <> Guid.Empty then rc.Add(objectId)
        if deleteInput then
            for objectId in Seq.append input0 input1 do
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                Doc.Objects.Delete(objectId, true) |> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Performs boolean intersection operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid seq) Meshes to intersect</param>
    ///<param name="input1">(Guid seq) Meshes to intersect</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input meshes</param>
    ///<returns>(Guid ResizeArray) identifiers of new meshes</returns>
    static member MeshBooleanIntersection( input0:Guid seq,
                                           input1:Guid seq,
                                           [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let meshes0 =  resizeArray { for objectId in input0 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let meshes1 =  resizeArray { for objectId in input1 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        if meshes0.Count = 0 || meshes1.Count = 0 then Error.Raise <| sprintf "RhinoScriptSyntax.Rhino.Scripting.MeshBooleanIntersection: No meshes to work with.  input0:'%A' input1:'%A' deleteInput:'%A' " input0 input1 deleteInput
        let newmeshes = Mesh.CreateBooleanIntersection  (meshes0, meshes1)
        let rc = ResizeArray()
        for mesh in newmeshes do
            let objectId = Doc.Objects.AddMesh(mesh)
            if objectId <> Guid.Empty then rc.Add(objectId)
        if deleteInput then
            for objectId in Seq.append input0 input1 do
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                Doc.Objects.Delete(objectId, true) |> ignore
        Doc.Views.Redraw()
        rc



    [<Extension>]
    ///<summary>Performs boolean split operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid seq) Meshes to split from</param>
    ///<param name="input1">(Guid seq) Meshes to split with</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input meshes</param>
    ///<returns>(Guid ResizeArray) identifiers of new meshes</returns>
    static member MeshBooleanSplit( input0:Guid seq,
                                    input1:Guid seq,
                                    [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let meshes0 =  resizeArray { for objectId in input0 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let meshes1 =  resizeArray { for objectId in input1 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        if meshes0.Count = 0 || meshes1.Count = 0 then Error.Raise <| sprintf "RhinoScriptSyntax.Rhino.Scripting.CreateBooleanSplit: No meshes to work with.  input0:'%A' input1:'%A' deleteInput:'%A' " input0 input1 deleteInput
        let newmeshes = Mesh.CreateBooleanSplit  (meshes0, meshes1)
        let rc = ResizeArray()
        for mesh in newmeshes do
            let objectId = Doc.Objects.AddMesh(mesh)
            if objectId <> Guid.Empty then rc.Add(objectId)
        if deleteInput then
            for objectId in Seq.append input0 input1 do
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                Doc.Objects.Delete(objectId, true) |> ignore
        Doc.Views.Redraw()
        rc



    [<Extension>]
    ///<summary>Performs boolean union operation on a set of input meshes</summary>
    ///<param name="meshIds">(Guid seq) Identifiers of meshes</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input meshes</param>
    ///<returns>(Guid ResizeArray) identifiers of new meshes</returns>
    static member MeshBooleanUnion(meshIds:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        if Seq.length(meshIds)<2 then Error.Raise <| sprintf "RhinoScriptSyntax.MeshIds must contain at least 2 meshes.  meshIds:'%A' deleteInput:'%A'" meshIds deleteInput
        let meshes =  resizeArray { for objectId in meshIds do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let newmeshes = Mesh.CreateBooleanUnion(meshes)
        let rc = ResizeArray()
        for mesh in newmeshes do
            let objectId = Doc.Objects.AddMesh(mesh)
            if objectId <> Guid.Empty then rc.Add(objectId)
        if rc.Count>0 && deleteInput then
            for objectId in meshIds do
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                Doc.Objects.Delete(objectId, true) |> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Returns the point on a mesh that is closest to a test point</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="point">(Point3d) Point to test</param>
    ///<param name="maximumDistance">(float) Optional, Upper bound used for closest point calculation.
    ///    If you are only interested in finding a point Q on the mesh when
    ///    point.DistanceTo(Q) is smaller than maximumDistance, then set maximumDistance to
    ///    that value</param>
    ///<returns>(Point3d * int) containing the results of the calculation where
    ///    [0] = the 3-D point on the mesh
    ///    [1] = the index of the mesh face on which the 3-D point lies</returns>
    static member MeshClosestPoint( objectId:Guid,
                                    point:Point3d,
                                    [<OPT;DEF(0.0)>]maximumDistance:float) : Point3d * int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        let pt = ref Point3d.Origin
        let face = mesh.ClosestPoint(point, pt, maximumDistance)
        if face<0 then Error.Raise <| sprintf "RhinoScriptSyntax.MeshClosestPoint failed.  objectId:'%A' point:'%A' maximumDistance:'%A'" objectId point maximumDistance
        !pt, face


    [<Extension>]
    ///<summary>Returns the center of each face of the mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d ResizeArray) points defining the center of each face</returns>
    static member MeshFaceCenters(meshId:Guid) : Point3d ResizeArray =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        resizeArray {for i in range(mesh.Faces.Count) do mesh.Faces.GetFaceCenter(i) }


    [<Extension>]
    ///<summary>Returns total face count of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) the number of mesh faces</returns>
    static member MeshFaceCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Faces.Count


    [<Extension>]
    ///<summary>Returns the face unit normal for each face of a mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Vector3d ResizeArray) 3D vectors that define the face unit normals of the mesh</returns>
    static member MeshFaceNormals(meshId:Guid) : Vector3d ResizeArray =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        if mesh.FaceNormals.Count <> mesh.Faces.Count then
            mesh.FaceNormals.ComputeFaceNormals() |> ignore
        let rc = ResizeArray()
        for i in range(mesh.FaceNormals.Count) do
            let normal = mesh.FaceNormals.[i]
            rc.Add(Vector3d(normal))
        rc


    [<Extension>]
    ///<summary>Returns face vertices of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="faceType">(bool) Optional, Default Value: <c>true</c>
    ///    The face type to be returned. 
    ///    True = both triangles and quads. 
    ///    False = Quads are broken down into triangles</param>
    ///<returns>(Point3d ResizeArray) List of 3D points that define the face vertices of the mesh. 
    ///    If faceType is True, then faces are returned as both quads and triangles
    ///    (every four  3D points). For triangles, the third and fourth vertex will be identical.     
    ///    If faceType is False, then faces are returned as only triangles
    ///    (very three 3D points). Quads will be converted to triangles.</returns>
    static member MeshFaces(objectId:Guid, [<OPT;DEF(true)>]faceType:bool) : Point3d ResizeArray =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = ResizeArray()
        for i in range(mesh.Faces.Count) do
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
                if p2 <> p3 then
                    rc.Add( p2 )
                    rc.Add( p3 )
                    rc.Add( p0 )
        rc

    [<Extension>]
    ///<summary>Returns vertices of each face in a mesh as tuple of 4 points</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d ResizeArray) List of 3D points that define the face vertices of the mesh. 
    ///    the faces are returned as both quads and triangles. For triangles, the third and fourth vertex will be identical.</returns>
    static member MeshFacePoints(objectId:Guid) : (Point3d*Point3d*Point3d*Point3d) ResizeArray = // TODO mark functions not part of rhinopython
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = ResizeArray()
        for i in range(mesh.Faces.Count) do
            let getrc, p0, p1, p2, p3 = mesh.Faces.GetFaceVertices(i)
            let p0 = Point3d(p0)
            let p1 = Point3d(p1)
            let p2 = Point3d(p2)
            let p3 = Point3d(p3)
            rc.Add( p0,p1,p2,p3 )
        rc



    [<Extension>]
    ///<summary>Returns the vertex indices of all faces of a Ngon mesh object.</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object.</param>
    ///<returns>(int ResizeArray ResizeArray) containing a nested List that define the vertex indices for
    ///    each face of the mesh. Ngons, quad and triangle faces are returned.</returns>
    static member MeshNgonFaceVertices(objectId:Guid) : ResizeArray<ResizeArray<int>> = //TODO add more ngon support functions like this ???
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = ResizeArray()
        for ng in mesh.GetNgonAndFacesEnumerable() do
            let uixs = ng.BoundaryVertexIndexList()
            let ixs= ResizeArray()
            for ix in uixs do
                ixs.Add(int(ix))
            rc.Add(ixs)
        rc


    [<Extension>]
    ///<summary>Returns the vertex indices of all faces of a mesh object, Does not suport Ngons yet</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>((int*int*int*int) ResizeArray) containing tuples of 4 numbers that define the vertex indices for
    ///    each face of the mesh. Both quad and triangle faces are returned. If the
    ///    third and fourth vertex indices are identical, the face is a triangle</returns>
    static member MeshFaceVertices(objectId:Guid) : ResizeArray<int*int*int*int> =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = ResizeArray()
        for i in range(mesh.Faces.Count) do
            let face = mesh.Faces.GetFace(i)
            rc.Add( (face.A, face.B, face.C, face.D) ) //TODO add ngon support
        rc


    [<Extension>]
    ///<summary>Verifies a mesh object has face normals</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member MeshHasFaceNormals(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.FaceNormals.Count>0


    [<Extension>]
    ///<summary>Verifies a mesh object has texture coordinates</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member MeshHasTextureCoordinates(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.TextureCoordinates.Count>0


    [<Extension>]
    ///<summary>Verifies a mesh object has vertex colors</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member MeshHasVertexColors(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.VertexColors.Count>0


    [<Extension>]
    ///<summary>Verifies a mesh object has vertex normals</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member MeshHasVertexNormals(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Normals.Count>0


    [<Extension>]
    ///<summary>Calculates the intersections of a mesh object with another mesh object</summary>
    ///<param name="mesh1">(Guid) Mesh1</param>
    ///<param name="mesh2">(Guid) Mesh2</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///    The intersection tolerance</param>
    ///<returns>(Polyline array) of points that define the vertices of the intersection curves</returns>
    static member MeshMeshIntersection( mesh1:Guid,
                                        mesh2:Guid,
                                        [<OPT;DEF(0.0)>]tolerance:float) : Polyline array =
        let mesh1 = RhinoScriptSyntax.CoerceMesh(mesh1)
        let mesh2 = RhinoScriptSyntax.CoerceMesh(mesh2)
        let tolerance = ifZero1 tolerance RhinoMath.ZeroTolerance
        Intersect.Intersection.MeshMeshAccurate(mesh1, mesh2, tolerance)


    [<Extension>]
    ///<summary>Identifies the naked edge points of a mesh object. This function shows
    ///    where mesh vertices are not completely surrounded by faces. Joined
    ///    meshes, such as are made by MeshBox, have naked mesh edge points where
    ///    the sub-meshes are joined</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool array) of boolean values that represent whether or not a mesh vertex is
    ///    naked or not. The number of elements in the list will be equal to
    ///    the value returned by MeshVertexCount. In which case, the list will
    ///    identify the naked status for each vertex returned by MeshVertices</returns>
    static member MeshNakedEdgePoints(objectId:Guid) : bool array =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.GetNakedEdgePointStatus()



    [<Extension>]
    ///<summary>Makes a new mesh with vertices offset at a distance in the opposite
    ///    direction of the existing vertex normals</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<param name="distance">(float) The distance to offset</param>
    ///<returns>(Guid) identifier of the new mesh object</returns>
    static member MeshOffset(meshId:Guid, distance:float) : Guid =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let offsetmesh = mesh.Offset(distance)
        if offsetmesh|> isNull  then Error.Raise <| sprintf "RhinoScriptSyntax.MeshOffset failed.  meshId:'%A' distance:'%A'" meshId distance
        let rc = Doc.Objects.AddMesh(offsetmesh)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.Unable to add mesh to document.  meshId:'%A' distance:'%A'" meshId distance
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Creates polyline curve outlines of mesh objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of meshes to outline</param>
    ///<param name="view">(string) Optional, Default Value: <c>Top View</c>
    ///    View to use for outline direction</param>
    ///<returns>(Guid ResizeArray) polyline curve identifiers</returns>
    static member MeshOutline(objectIds:Guid seq, [<OPT;DEF(null:string)>]view:string) : Guid ResizeArray =
        let  meshes =  resizeArray { for objectId in objectIds do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let rc = ResizeArray()
        if notNull view then
            let viewport = Doc.Views.Find(view, false).MainViewport
            if isNull viewport then Error.Raise <| sprintf "RhinoScriptSyntax.Rhino.Scripting.MeshOutline: did not find view named '%A'" view
            else
                for mesh in meshes do
                    let polylines = mesh.GetOutlines(viewport)
                    if notNull polylines then
                        for polyline in polylines do
                            let objectId = Doc.Objects.AddPolyline(polyline)
                            rc.Add(objectId)
        else
            for mesh in meshes do
                let polylines = mesh.GetOutlines(Plane.WorldXY)
                if notNull polylines then
                    for polyline in polylines do
                        let objectId = Doc.Objects.AddPolyline(polyline)
                        rc.Add(objectId)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Returns the number of quad faces of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) the number of quad mesh faces</returns>
    static member MeshQuadCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Faces.QuadCount


    [<Extension>]
    ///<summary>Converts a mesh object's quad faces to triangles</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member MeshQuadsToTriangles(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mutable rc = true
        if mesh.Faces.QuadCount>0 then
            rc <- mesh.Faces.ConvertQuadsToTriangles()
            if rc  then
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                Doc.Objects.Replace(objectId, mesh) |> ignore
                Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Duplicates each polygon in a mesh with a NURBS surface. The resulting
    ///    surfaces are then joined into a polysurface and added to the document</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="trimmedTriangles">(bool) Optional, Default Value: <c>true</c>
    ///    If True, triangles in the mesh will be
    ///    represented by a trimmed plane</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input object</param>
    ///<returns>(Guid ResizeArray) identifiers for the new breps</returns>
    static member MeshToNurb( objectId:Guid,
                              [<OPT;DEF(true)>]trimmedTriangles:bool,
                              [<OPT;DEF(false)>]deleteInput:bool) : Guid ResizeArray =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let pieces = mesh.SplitDisjointPieces()
        let breps =  resizeArray { for piece in pieces do yield Brep.CreateFromMesh(piece, trimmedTriangles) }
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let attr = rhobj.Attributes
        let ids =  resizeArray { for brep in breps do yield Doc.Objects.AddBrep(brep, attr) }
        if deleteInput then Doc.Objects.Delete(rhobj, true)|> ignore
        Doc.Views.Redraw()
        ids


    [<Extension>]
    ///<summary>Returns number of triangular faces of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of triangular mesh faces</returns>
    static member MeshTriangleCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Faces.TriangleCount


    [<Extension>]
    ///<summary>Returns vertex colors of a mesh</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Drawing.Color ResizeArray) The current vertex colors</returns>
    static member MeshVertexColors(meshId:Guid) : Drawing.Color ResizeArray= //GET
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        resizeArray { for i in range(mesh.VertexColors.Count) do mesh.VertexColors.[i] }


    [<Extension>]
    ///<summary>Modifies vertex colors of a mesh</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<param name="colors">(Drawing.Color seq), optional) A list of color values. Note, for each vertex, there must
    ///    be a corresponding vertex color. If the value is null or empty list , then any
    ///    existing vertex colors will be removed from the mesh</param>
    ///<returns>(unit) void, nothing</returns>
    static member MeshVertexColors(meshId:Guid, colors:Drawing.Color seq) : unit = //SET
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        if colors|> isNull || Seq.isEmpty colors   then
            mesh.VertexColors.Clear()
        else
            let colorcount = Seq.length(colors)
            if colorcount <> mesh.Vertices.Count then
                Error.Raise <| sprintf "RhinoScriptSyntax.Length of colors must match vertex count.  meshId:'%A' colors:'%A'" meshId colors
            mesh.VertexColors.Clear()
            for c in colors do mesh.VertexColors.Add(c) |> ignore
        Doc.Objects.Replace(meshId, mesh) |> ignore
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the vertex count of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of mesh vertices</returns>
    static member MeshVertexCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Vertices.Count


    [<Extension>]
    ///<summary>Returns the mesh faces that share a specified mesh vertex</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<param name="vertexIndex">(int) Index of the mesh vertex to find faces for</param>
    ///<returns>(int array) face indices</returns>
    static member MeshVertexFaces(meshId:Guid, vertexIndex:int) : int array =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        mesh.Vertices.GetVertexFaces(vertexIndex)


    [<Extension>]
    ///<summary>Returns the vertex unit normal for each vertex of a mesh</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Vector3d ResizeArray) of vertex normals, (empty list if no normals exist)</returns>
    static member MeshVertexNormals(meshId:Guid) : Vector3d ResizeArray =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let count = mesh.Normals.Count
        if count<1 then resizeArray {()}
        else resizeArray { for i in range(count) do Vector3d(mesh.Normals.[i])}


    [<Extension>]
    ///<summary>Returns the vertices of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d ResizeArray) vertex points in the mesh</returns>
    static member MeshVertices(objectId:Guid) : Point3d ResizeArray =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let count = mesh.Vertices.Count
        let rc = ResizeArray()
        for i in range(count) do
            let vertex = mesh.Vertices.[i]
            rc.Add(Point3d(vertex))
        rc


    [<Extension>]
    ///<summary>Returns the approximate volume of one or more closed meshes</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more mesh objects</param>
    ///<returns>(float)  total volume of all meshes</returns>
    static member MeshVolume(objectIds:Guid seq) : float =
        let mutable totalvolume  = 0.0
        for objectId in objectIds do
            let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
            let mp = VolumeMassProperties.Compute(mesh)
            if notNull mp then
                totalvolume <- totalvolume + mp.Volume
            else
                Error.Raise <| sprintf "RhinoScriptSyntax.MeshVolume failed on objectId:'%A'" objectId
        totalvolume


    [<Extension>]
    ///<summary>Calculates the volume centroid of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d) Point3d representing the volume centroid</returns>
    static member MeshVolumeCentroid(objectId:Guid) : Point3d =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mp = VolumeMassProperties.Compute(mesh)
        if notNull mp then mp.Centroid
        else Error.Raise <| sprintf "RhinoScriptSyntax.MeshVolumeCentroid failed.  objectId:'%A'" objectId


    [<Extension>]
    ///<summary>Pulls a curve to a mesh. The function makes a polyline approximation of
    ///    the input curve and gets the closest point on the mesh for each point on
    ///    the polyline. Then it "connects the points" to create a polyline on the mesh</summary>
    ///<param name="meshId">(Guid) Identifier of mesh that pulls</param>
    ///<param name="curveId">(Guid) Identifier of curve to pull</param>
    ///<returns>(Guid) identifier new curve</returns>
    static member PullCurveToMesh(meshId:Guid, curveId:Guid) : Guid =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let tol = Doc.ModelAbsoluteTolerance
        let polyline = curve.PullToMesh(mesh, tol)
        if isNull polyline then Error.Raise <| sprintf "RhinoScriptSyntax.PullCurveToMesh failed.  meshId:'%A' curveId:'%A'" meshId curveId
        let rc = Doc.Objects.AddCurve(polyline)
        if rc = Guid.Empty then Error.Raise <| sprintf "RhinoScriptSyntax.Unable to add polyline to document.  meshId:'%A' curveId:'%A'" meshId curveId
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Splits up a mesh into its unconnected pieces</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the input object</param>
    ///<returns>(Guid ResizeArray) identifiers for the new meshes</returns>
    static member SplitDisjointMesh(objectId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid ResizeArray =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let pieces = mesh.SplitDisjointPieces()
        let rc =  resizeArray { for piece in pieces do yield Doc.Objects.AddMesh(piece) }
        if rc.Count <> 0 && deleteInput then
            //id = RhinoScriptSyntax.Coerceguid(objectId)
            Doc.Objects.Delete(objectId, true) |> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Fixes inconsistencies in the directions of faces of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) the number of faces that were modified</returns>
    static member UnifyMeshNormals(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = mesh.UnifyNormals()
        if rc>0 then
            //id = RhinoScriptSyntax.Coerceguid(objectId)
            Doc.Objects.Replace(objectId, mesh)|> ignore
            Doc.Views.Redraw()
        rc


