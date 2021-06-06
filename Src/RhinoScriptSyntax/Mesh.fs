namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry

open System.Collections.Generic
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

 
[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namespace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsMesh =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    ///<summary>Add a Mesh object to the document.</summary>
    ///<param name="vertices">(Point3d seq) List of 3D points defining the vertices of the Mesh</param>
    ///<param name="faceVertices">(int IList seq) List containing lists of 3 or 4 numbers that define the
    ///    vertex indices for each face of the Mesh. If the third a fourth vertex
    ///      indices of a face are identical, a triangular face will be created</param>
    ///<param name="vertexNormals">(Vector3f seq) Optional, List of 3D vectors defining the vertex normals of
    ///    the Mesh. Note, for every vertex, there must be a corresponding vertex
    ///    normal</param>
    ///<param name="textureCoordinates">(Point2f seq) Optional, List of 2D texture coordinates. For every
    ///    vertex, there must be a corresponding texture coordinate</param>
    ///<param name="vertexColors">(Drawing.Color seq) Optional, A list of color values. For every vertex,
    ///    there must be a corresponding vertex color</param>
    ///<returns>(Guid) Identifier of the new object.</returns>
    [<Extension>]
    static member AddMesh( vertices:Point3d seq, //TODO how to construct Ngon Mesh ???
                           faceVertices:seq<#IList<int>>, // TODO test if nested arrays are accepted
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
                RhinoScriptingException.Raise "RhinoScriptSyntax.AddMesh: Expected 3 or 4 indices for a face but got %d" l

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

        let rc = State.Doc.Objects.AddMesh(mesh)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Unable to add mesh to document.  vertices:'%A' faceVertices:'%A' vertexNormals:'%A' textureCoordinates:'%A' vertexColors:'%A'" vertices faceVertices vertexNormals textureCoordinates vertexColors
        State.Doc.Views.Redraw()
        rc
    
    ///<summary>Add a Mesh object to the document.</summary>
    ///<param name="vertices">(Point3d seq) List of 3D points defining the vertices of the Mesh</param>
    ///<param name="faceVertices">(int*int*int*int seq) Tuple  of  4 integers that define the
    ///    vertex indices for each face of the Mesh. If the third a fourth vertex
    ///      indices of a face are identical, a triangular face will be created</param>
    ///<param name="vertexNormals">(Vector3f seq) Optional, List of 3D vectors defining the vertex normals of
    ///    the Mesh. Note, for every vertex, there must be a corresponding vertex
    ///    normal</param>
    ///<param name="textureCoordinates">(Point2f seq) Optional, List of 2D texture coordinates. For every
    ///    vertex, there must be a corresponding texture coordinate</param>
    ///<param name="vertexColors">(Drawing.Color seq) Optional, A list of color values. For every vertex,
    ///    there must be a corresponding vertex color</param>
    ///<returns>(Guid) Identifier of the new object.</returns>
    [<Extension>]
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

        let rc = State.Doc.Objects.AddMesh(mesh)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Unable to add mesh to document.  vertices:'%A' faceVertices:'%A' vertexNormals:'%A' textureCoordinates:'%A' vertexColors:'%A'" vertices faceVertices vertexNormals textureCoordinates vertexColors
        State.Doc.Views.Redraw()
        rc



    ///<summary>Creates a new Mesh with just one quad face .</summary>
    ///<param name="pointA">(Point3d) First corner point</param>
    ///<param name="pointB">(Point3d) Second  corner point</param>
    ///<param name="pointC">(Point3d) Third corner point</param>
    ///<param name="pointD">(Point3d) Fourth corner point</param>
    ///<returns>(Guid) The identifier of the new Mesh.</returns>
    [<Extension>]
    static member AddMeshQuad(pointA:Point3d , pointB:Point3d , pointC: Point3d , pointD: Point3d) : Guid =
          let mesh = new Mesh()
          mesh.Vertices.Add(pointA) |> ignore
          mesh.Vertices.Add(pointB) |> ignore
          mesh.Vertices.Add(pointC) |> ignore
          mesh.Vertices.Add(pointD) |> ignore
          mesh.Faces.AddFace(0,1,2,3) |> ignore
          let rc = State.Doc.Objects.AddMesh(mesh)
          if rc = Guid.Empty then  RhinoScriptingException.Raise "RhinoScriptSyntax.AddMeshQuad failed.  points:'%A, %A, %A and %A" pointA pointB pointC pointD
          State.Doc.Views.Redraw()
          rc

    ///<summary>Creates a new Mesh with just one triangle face .</summary>
    ///<param name="pointA">(Point3d) First corner point</param>
    ///<param name="pointB">(Point3d) Second  corner point</param>
    ///<param name="pointC">(Point3d) Third corner point</param>
    ///<returns>(Guid) The identifier of the new Mesh.</returns>
    [<Extension>]
    static member AddMeshTriangle(pointA:Point3d , pointB:Point3d , pointC: Point3d ) : Guid =
          let mesh = new Mesh()
          mesh.Vertices.Add(pointA) |> ignore
          mesh.Vertices.Add(pointB) |> ignore
          mesh.Vertices.Add(pointC) |> ignore
          mesh.Faces.AddFace(0,1,2) |> ignore
          let rc = State.Doc.Objects.AddMesh(mesh)
          if rc = Guid.Empty then  RhinoScriptingException.Raise "RhinoScriptSyntax.AddMeshQuad failed.  points:'%A, %A and %A" pointA pointB pointC 
          State.Doc.Views.Redraw()
          rc

    ///<summary>Creates a planar Mesh from a closed, planar Curve.</summary>
    ///<param name="objectId">(Guid) Identifier of a closed, planar Curve</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    If True, delete the input Curve defined by objectId</param>
    ///<returns>(Guid) id of the new Mesh.</returns>
    [<Extension>]
    static member AddPlanarMesh(objectId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(objectId)
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let mesh = Mesh.CreateFromPlanarBoundary(curve, MeshingParameters.Default, tolerance)
        if isNull mesh then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPlanarMesh failed.  objectId:'%s' deleteInput:'%A'" (Print.guid objectId) deleteInput
        if deleteInput then
            let ob = RhinoScriptSyntax.CoerceGuid(objectId)
            if not<| State.Doc.Objects.Delete(ob, true) then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPlanarMesh failed to delete input.  objectId:'%s' deleteInput:'%A'" (Print.guid objectId) deleteInput
        let rc = State.Doc.Objects.AddMesh(mesh)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Unable to add mesh to document.  objectId:'%s' deleteInput:'%A'" (Print.guid objectId) deleteInput
        State.Doc.Views.Redraw()
        rc


    ///<summary>Calculates the intersection of a Curve object and a Mesh object.</summary>
    ///<param name="curveId">(Guid) Identifier of a Curve object</param>
    ///<param name="meshId">(Guid) Identifier or a Mesh object</param>
    ///<returns>(Point3d array * int array) two arrays as tuple:
    ///        [0] = point of intersection
    ///        [1] = Mesh face index where intersection lies.</returns>
    [<Extension>]
    static member CurveMeshIntersection( curveId:Guid,
                                         meshId:Guid) : array<Point3d>*array<int> =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let polylinecurve = curve.ToPolyline(0 , 0 , 0.0 , 0.0 , 0.0, tolerance , 0.0 , 0.0 , true)
        let pts, faceids = Intersect.Intersection.MeshPolyline(mesh, polylinecurve)
        if isNull pts then RhinoScriptingException.Raise "RhinoScriptSyntax.CurveMeshIntersection failed. curveId:'%s' meshId:'%s'" (Print.guid curveId) (Print.guid meshId)
        pts, faceids


    ///<summary>Returns number of Meshes that could be created by calling SplitDisjointMesh.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(int) The number of Meshes that could be created.</returns>
    [<Extension>]
    static member DisjointMeshCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.DisjointMeshCount


    ///<summary>Creates Curves that duplicates a Mesh border.</summary>
    ///<param name="meshId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(Guid Rarr) list of Curve ids.</returns>
    [<Extension>]
    static member DuplicateMeshBorder(meshId:Guid) : Guid Rarr =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let polylines = mesh.GetNakedEdges()
        let rc = Rarr()
        if notNull polylines then
            for polyline in polylines do
                let objectId = State.Doc.Objects.AddPolyline(polyline)
                if objectId <> Guid.Empty then rc.Add(objectId)
        if rc.Count <> 0 then State.Doc.Views.Redraw()
        rc


    ///<summary>Explodes a Mesh object, or Mesh objects int submeshes. A submesh is a
    ///    collection of Mesh faces that are contained within a closed loop of
    ///    unwelded Mesh edges. Unwelded Mesh edges are where the Mesh faces that
    ///    share the edge have unique Mesh vertices (not Mesh topology vertices)
    ///    at both ends of the edge.</summary>
    ///<param name="meshIds">(Guid seq) List of Mesh identifiers</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the input Meshes</param>
    ///<returns>(Guid Rarr) List of resulting objects after explode.</returns>
    [<Extension>]
    static member ExplodeMeshes(meshIds:Guid seq, [<OPT;DEF(false)>]delete:bool) : Guid Rarr =
        //id = RhinoScriptSyntax.Coerceguid(meshIds)
        let rc = Rarr()
        for meshid in meshIds do
            let mesh = RhinoScriptSyntax.CoerceMesh(meshid)
            if notNull mesh then
                let submeshes = mesh.ExplodeAtUnweldedEdges()
                if notNull submeshes then
                    for submesh in submeshes do
                        let objectId = State.Doc.Objects.AddMesh(submesh)
                        if objectId <> Guid.Empty then rc.Add(objectId)
                if delete then
                    State.Doc.Objects.Delete(meshid, true)|> ignore
        if rc.Count>0 then State.Doc.Views.Redraw()
        rc


    ///<summary>Verifies if an object is a Mesh.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False.</returns>
    [<Extension>]
    static member IsMesh(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceMesh(objectId)
        |> Option.isSome


    ///<summary>Verifies a Mesh object is closed.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    [<Extension>]
    static member IsMeshClosed(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceMesh(objectId) with
        | Some mesh -> mesh.IsClosed
        | None -> false


    ///<summary>Verifies a Mesh object is manifold. A Mesh for which every edge is shared
    ///    by at most two faces is called manifold. If a Mesh has at least one edge
    ///    that is shared by more than two faces, then that Mesh is called non-manifold.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(bool) True, otherwise False.</returns>
    [<Extension>]
    static member IsMeshManifold(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceMesh(objectId) with
        | Some mesh -> mesh.IsManifold(true)  |> t1
        | None -> false



    ///<summary>Verifies a point is on a Mesh.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<param name="point">(Point3d) Test point</param>
    ///<param name="tolerance">(float) Optional, Defalut Value <c>RhinoMath.SqrtEpsilon</c>
    ///    The testing tolerance</param>
    ///<returns>(bool) True , otherwise False.</returns>
    [<Extension>]
    static member IsPointOnMesh(    objectId:Guid,
                                    point:Point3d,
                                    [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        let maxdistance = Util.ifZero1 tolerance RhinoMath.SqrtEpsilon
        let pt = ref Point3d.Origin
        let face = mesh.ClosestPoint(point, pt, maxdistance)
        face>=0

    
    ///<summary>Joins two or or more Mesh objects together.</summary>
    ///<param name="meshes">(Mesh seq) Mesh objects</param>
    ///<param name="deleteInput">(bool) Optional, Defalut Value <c>false</c>
    ///   Delete input objects?</param>
    ///<returns>(Mesh) newly created Mesh.</returns>
    [<Extension>]
    static member JoinMeshes(meshes:Mesh seq, [<OPT;DEF(false)>]deleteInput:bool) : Mesh =
        let joinedmesh = new Mesh()
        joinedmesh.Append(meshes)
        joinedmesh

    ///<summary>Joins two or or more Mesh objects together.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of two or more Mesh objects</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input after joining</param>
    ///<returns>(Guid) identifier of newly created Mesh.</returns>
    [<Extension>]
    static member JoinMeshes(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let meshes =  rarr { for objectId in objectIds do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let joinedmesh = new Mesh()
        joinedmesh.Append(meshes)
        let rc = State.Doc.Objects.AddMesh(joinedmesh)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Failed to join Meshes %A" (RhinoScriptSyntax.ToNiceString objectIds) 
        if deleteInput then
            for objectId in objectIds do
                //guid = RhinoScriptSyntax.Coerceguid(objectId)
                State.Doc.Objects.Delete(objectId, true) |> ignore
        State.Doc.Views.Redraw()
        rc


    ///<summary>Returns approximate area of onemesh object.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh objects</param>
    ///<returns>(float) total area of Mesh.</returns>
    [<Extension>]
    static member MeshArea(objectId:Guid ) : float =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mp = AreaMassProperties.Compute(mesh)
        if notNull mp then
            mp.Area
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.MeshArea failed.  objectId:'%s'" (Print.guid objectId)



    ///<summary>Calculates the area centroid of a Mesh object.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(Point3d) representing the area centroid.</returns>
    [<Extension>]
    static member MeshAreaCentroid(objectId:Guid) : Point3d =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mp = AreaMassProperties.Compute(mesh)
        if mp|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MeshAreaCentroid failed.  objectId:'%s'" (Print.guid objectId)
        mp.Centroid


    ///<summary>Performs boolean difference operation on two sets of input Meshes.</summary>
    ///<param name="input0">(Guid seq) Meshes to subtract from</param>
    ///<param name="input1">(Guid seq) Meshes to subtract with</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input Meshes</param>
    ///<returns>(Guid Rarr) identifiers of newly created Meshes.</returns>
    [<Extension>]
    static member MeshBooleanDifference( input0:Guid seq,
                                         input1:Guid seq,
                                         [<OPT;DEF(true)>]deleteInput:bool) : Guid Rarr =
        let meshes0 =  rarr { for objectId in input0 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let meshes1 =  rarr { for objectId in input1 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        if meshes0.Count = 0 || meshes1.Count = 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.Rhino.Scripting.MeshBooleanDifference: No meshes to work with.  input0:'%A' input1:'%A' deleteInput:'%A'" input0 input1 deleteInput
        let newmeshes = Mesh.CreateBooleanDifference  (meshes0, meshes1)
        let rc = Rarr()
        for mesh in newmeshes do
            let objectId = State.Doc.Objects.AddMesh(mesh)
            if objectId <> Guid.Empty then rc.Add(objectId)
        if deleteInput then
            for objectId in Seq.append input0 input1 do
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                State.Doc.Objects.Delete(objectId, true) |> ignore
        State.Doc.Views.Redraw()
        rc


    ///<summary>Performs boolean intersection operation on two sets of input Meshes.</summary>
    ///<param name="input0">(Guid seq) Meshes to intersect</param>
    ///<param name="input1">(Guid seq) Meshes to intersect</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input Meshes</param>
    ///<returns>(Guid Rarr) identifiers of new Meshes.</returns>
    [<Extension>]
    static member MeshBooleanIntersection( input0:Guid seq,
                                           input1:Guid seq,
                                           [<OPT;DEF(true)>]deleteInput:bool) : Guid Rarr =
        let meshes0 =  rarr { for objectId in input0 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let meshes1 =  rarr { for objectId in input1 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        if meshes0.Count = 0 || meshes1.Count = 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.Rhino.Scripting.MeshBooleanIntersection: No meshes to work with.  input0:'%A' input1:'%A' deleteInput:'%A'" input0 input1 deleteInput
        let newmeshes = Mesh.CreateBooleanIntersection  (meshes0, meshes1)
        let rc = Rarr()
        for mesh in newmeshes do
            let objectId = State.Doc.Objects.AddMesh(mesh)
            if objectId <> Guid.Empty then rc.Add(objectId)
        if deleteInput then
            for objectId in Seq.append input0 input1 do
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                State.Doc.Objects.Delete(objectId, true) |> ignore
        State.Doc.Views.Redraw()
        rc



    ///<summary>Performs boolean split operation on two sets of input Meshes.</summary>
    ///<param name="input0">(Guid seq) Meshes to split from</param>
    ///<param name="input1">(Guid seq) Meshes to split with</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input Meshes</param>
    ///<returns>(Guid Rarr) identifiers of new Meshes.</returns>
    [<Extension>]
    static member MeshBooleanSplit( input0:Guid seq,
                                    input1:Guid seq,
                                    [<OPT;DEF(true)>]deleteInput:bool) : Guid Rarr =
        let meshes0 =  rarr { for objectId in input0 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let meshes1 =  rarr { for objectId in input1 do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        if meshes0.Count = 0 || meshes1.Count = 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.Rhino.Scripting.CreateBooleanSplit: No meshes to work with.  input0:'%A' input1:'%A' deleteInput:'%A'" input0 input1 deleteInput
        let newmeshes = Mesh.CreateBooleanSplit  (meshes0, meshes1)
        let rc = Rarr()
        for mesh in newmeshes do
            let objectId = State.Doc.Objects.AddMesh(mesh)
            if objectId <> Guid.Empty then rc.Add(objectId)
        if deleteInput then
            for objectId in Seq.append input0 input1 do
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                State.Doc.Objects.Delete(objectId, true) |> ignore
        State.Doc.Views.Redraw()
        rc



    ///<summary>Performs boolean union operation on a set of input Meshes.</summary>
    ///<param name="meshIds">(Guid seq) Identifiers of Meshes</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete the input Meshes</param>
    ///<returns>(Guid Rarr) identifiers of new Meshes.</returns>
    [<Extension>]
    static member MeshBooleanUnion(meshIds:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid Rarr =
        if Seq.length(meshIds)<2 then RhinoScriptingException.Raise "RhinoScriptSyntax.MeshIds must contain at least 2 meshes.  meshIds:'%A' deleteInput:'%A'" meshIds deleteInput
        let meshes =  rarr { for objectId in meshIds do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let newmeshes = Mesh.CreateBooleanUnion(meshes)
        let rc = Rarr()
        for mesh in newmeshes do
            let objectId = State.Doc.Objects.AddMesh(mesh)
            if objectId <> Guid.Empty then rc.Add(objectId)
        if rc.Count>0 && deleteInput then
            for objectId in meshIds do
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                State.Doc.Objects.Delete(objectId, true) |> ignore
        State.Doc.Views.Redraw()
        rc


    ///<summary>Returns the point on a Mesh that is closest to a test point.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<param name="point">(Point3d) Point to test</param>
    ///<param name="maximumDistance">(float) Optional, Upper bound used for closest point calculation.
    ///    If you are only interested in finding a point Q on the Mesh when
    ///    point.DistanceTo(Q) is smaller than maximumDistance, then set maximumDistance to
    ///    that value</param>
    ///<returns>(Point3d * int) containing the results of the calculation where
    ///    [0] = the 3-D point on the Mesh
    ///    [1] = the index of the Mesh face on which the 3-D point lies.</returns>
    [<Extension>]
    static member MeshClosestPoint( objectId:Guid,
                                    point:Point3d,
                                    [<OPT;DEF(0.0)>]maximumDistance:float) : Point3d * int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        let pt = ref Point3d.Origin
        let face = mesh.ClosestPoint(point, pt, maximumDistance)
        if face<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.MeshClosestPoint failed.  objectId:'%s' point:'%A' maximumDistance:'%A'" (Print.guid objectId) point maximumDistance
        !pt, face


    ///<summary>Returns the center of each face of the Mesh object.</summary>
    ///<param name="meshId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(Point3d Rarr) points defining the center of each face.</returns>
    [<Extension>]
    static member MeshFaceCenters(meshId:Guid) : Point3d Rarr =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        rarr {for i = 0 to mesh.Faces.Count - 1 do mesh.Faces.GetFaceCenter(i) }


    ///<summary>Returns total face count of a Mesh object.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(int) The number of Mesh faces.</returns>
    [<Extension>]
    static member MeshFaceCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Faces.Count


    ///<summary>Returns the face unit normal for each face of a Mesh object.</summary>
    ///<param name="meshId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(Vector3d Rarr) 3D vectors that define the face unit normals of the Mesh.</returns>
    [<Extension>]
    static member MeshFaceNormals(meshId:Guid) : Vector3d Rarr =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        if mesh.FaceNormals.Count <> mesh.Faces.Count then
            mesh.FaceNormals.ComputeFaceNormals() |> ignore
        let rc = Rarr()
        for i = 0 to mesh.FaceNormals.Count - 1 do
            let normal = mesh.FaceNormals.[i]
            rc.Add(Vector3d(normal))
        rc


    ///<summary>Returns face vertices of a Mesh.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<param name="faceType">(bool) Optional, Default Value: <c>true</c>
    ///    The face type to be returned. 
    ///    True = both triangles and quads. 
    ///    False = Quads are broken down into triangles</param>
    ///<returns>(Point3d Rarr) List of 3D points that define the face vertices of the Mesh. 
    ///    If faceType is True, then faces are returned as both quads and triangles
    ///    (every four  3D points). For triangles, the third and fourth vertex will be identical.     
    ///    If faceType is False, then faces are returned as only triangles
    ///    (very three 3D points). Quads will be converted to triangles.</returns>
    [<Extension>]
    static member MeshFaces(objectId:Guid, [<OPT;DEF(true)>]faceType:bool) : Point3d Rarr =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = Rarr()
        for i = 0 to mesh.Faces.Count - 1 do
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

    ///<summary>Returns vertices of each face in a Mesh as tuple of 4 points.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(Point3d Rarr) List of 3D points that define the face vertices of the Mesh. 
    ///    the faces are returned as both quads and triangles. For triangles, the third and fourth vertex will be identical.</returns>
    [<Extension>]
    static member MeshFacePoints(objectId:Guid) : (Point3d*Point3d*Point3d*Point3d) Rarr = // TODO mark functions not part of rhinopython
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = Rarr()
        for i = 0 to mesh.Faces.Count - 1 do
            let getrc, p0, p1, p2, p3 = mesh.Faces.GetFaceVertices(i)
            let p0 = Point3d(p0)
            let p1 = Point3d(p1)
            let p2 = Point3d(p2)
            let p3 = Point3d(p3)
            rc.Add( p0,p1,p2,p3 )
        rc



    ///<summary>Returns the vertex indices of all faces of a Ngon Mesh object.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object.</param>
    ///<returns>(int Rarr Rarr) containing a nested List that define the vertex indices for
    ///    each face of the Mesh. Ngons, quad and triangle faces are returned.</returns>
    [<Extension>]
    static member MeshNgonFaceVertices(objectId:Guid) : Rarr<Rarr<int>> = //TODO add more ngon support functions like this ???
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = Rarr()
        for ng in mesh.GetNgonAndFacesEnumerable() do
            let uixs = ng.BoundaryVertexIndexList()
            let ixs= Rarr()
            for ix in uixs do
                ixs.Add(int(ix))
            rc.Add(ixs)
        rc


    ///<summary>Returns the vertex indices of all faces of a Mesh object, Does not suport Ngons yet.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>((int*int*int*int) Rarr) containing tuples of 4 numbers that define the vertex indices for
    ///    each face of the Mesh. Both quad and triangle faces are returned. If the
    ///    third and fourth vertex indices are identical, the face is a triangle.</returns>
    [<Extension>]
    static member MeshFaceVertices(objectId:Guid) : Rarr<int*int*int*int> =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = Rarr()
        for i = 0 to mesh.Faces.Count - 1 do
            let face = mesh.Faces.GetFace(i)
            rc.Add( (face.A, face.B, face.C, face.D)) //TODO add ngon support
        rc


    ///<summary>Verifies a Mesh object has face normals.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    [<Extension>]
    static member MeshHasFaceNormals(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.FaceNormals.Count>0


    ///<summary>Verifies a Mesh object has texture coordinates.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    [<Extension>]
    static member MeshHasTextureCoordinates(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.TextureCoordinates.Count>0


    ///<summary>Verifies a Mesh object has vertex colors.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    [<Extension>]
    static member MeshHasVertexColors(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.VertexColors.Count>0


    ///<summary>Verifies a Mesh object has vertex normals.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    [<Extension>]
    static member MeshHasVertexNormals(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Normals.Count>0


    ///<summary>Calculates the intersections of a Mesh object with another Mesh object.</summary>
    ///<param name="mesh1">(Guid) Mesh1</param>
    ///<param name="mesh2">(Guid) Mesh2</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///    The intersection tolerance</param>
    ///<returns>(Polyline array) Array of points that define the vertices of the intersection Curves.</returns>
    [<Extension>]
    static member MeshMeshIntersection( mesh1:Guid,
                                        mesh2:Guid,
                                        [<OPT;DEF(0.0)>]tolerance:float) : Polyline array =
        let mesh1 = RhinoScriptSyntax.CoerceMesh(mesh1)
        let mesh2 = RhinoScriptSyntax.CoerceMesh(mesh2)
        let tolerance = Util.ifZero1 tolerance RhinoMath.ZeroTolerance
        Intersect.Intersection.MeshMeshAccurate(mesh1, mesh2, tolerance)


    ///<summary>Identifies the naked edge points of a Mesh object. This function shows
    ///    where Mesh vertices are not completely surrounded by faces. Joined
    ///    Meshes, such as are made by MeshBox, have naked Mesh edge points where
    ///    the sub-meshes are joined.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(bool array) Array of boolean values that represent whether or not a Mesh vertex is
    ///    naked or not. The number of elements in the list will be equal to
    ///    the value returned by MeshVertexCount. In which case, the list will
    ///    identify the naked status for each vertex returned by MeshVertices.</returns>
    [<Extension>]
    static member MeshNakedEdgePoints(objectId:Guid) : bool array =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.GetNakedEdgePointStatus()



    ///<summary>Makes a new Mesh with vertices offset at a distance in the opposite
    ///    direction of the existing vertex normals.</summary>
    ///<param name="meshId">(Guid) Identifier of a Mesh object</param>
    ///<param name="distance">(float) The distance to offset</param>
    ///<returns>(Guid) identifier of the new Mesh object.</returns>
    [<Extension>]
    static member MeshOffset(meshId:Guid, distance:float) : Guid =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let offsetmesh = mesh.Offset(distance)
        if offsetmesh|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MeshOffset failed.  meshId:'%s' distance:'%A'" (Print.guid meshId) distance
        let rc = State.Doc.Objects.AddMesh(offsetmesh)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Unable to add mesh to document.  meshId:'%s' distance:'%A'" (Print.guid meshId) distance
        State.Doc.Views.Redraw()
        rc


    ///<summary>Creates Polyline Curve outlines of Mesh objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of Meshes to outline</param>
    ///<param name="view">(string) Optional, Default Value: <c>Top View</c>
    ///    View to use for outline direction</param>
    ///<returns>(Guid Rarr) Polyline Curve identifiers.</returns>
    [<Extension>]
    static member MeshOutline(objectIds:Guid seq, [<OPT;DEF(null:string)>]view:string) : Guid Rarr =
        let  meshes =  rarr { for objectId in objectIds do yield RhinoScriptSyntax.CoerceMesh(objectId) }
        let rc = Rarr()
        if notNull view then
            let viewport = State.Doc.Views.Find(view, compareCase=false).MainViewport
            if isNull viewport then RhinoScriptingException.Raise "RhinoScriptSyntax.Rhino.Scripting.MeshOutline: did not find view named '%A'" view
            else
                for mesh in meshes do
                    let polylines = mesh.GetOutlines(viewport)
                    if notNull polylines then
                        for polyline in polylines do
                            let objectId = State.Doc.Objects.AddPolyline(polyline)
                            rc.Add(objectId)
        else
            for mesh in meshes do
                let polylines = mesh.GetOutlines(Plane.WorldXY)
                if notNull polylines then
                    for polyline in polylines do
                        let objectId = State.Doc.Objects.AddPolyline(polyline)
                        rc.Add(objectId)
        State.Doc.Views.Redraw()
        rc


    ///<summary>Returns the number of quad faces of a Mesh object.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(int) The number of quad Mesh faces.</returns>
    [<Extension>]
    static member MeshQuadCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Faces.QuadCount


    ///<summary>Converts a Mesh object's quad faces to triangles.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    [<Extension>]
    static member MeshQuadsToTriangles(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mutable rc = true
        if mesh.Faces.QuadCount>0 then
            rc <- mesh.Faces.ConvertQuadsToTriangles()
            if rc  then
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                State.Doc.Objects.Replace(objectId, mesh) |> ignore
                State.Doc.Views.Redraw()
        rc


    ///<summary>Duplicates each polygon in a Mesh with a NURBS Surface. The resulting
    ///    Surfaces are then joined into a Polysurface and added to the document.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<param name="trimmedTriangles">(bool) Optional, Default Value: <c>true</c>
    ///    If True, triangles in the Mesh will be
    ///    represented by a trimmed Plane</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input object</param>
    ///<returns>(Guid Rarr) identifiers for the new breps.</returns>
    [<Extension>]
    static member MeshToNurb( objectId:Guid,
                              [<OPT;DEF(true)>]trimmedTriangles:bool,
                              [<OPT;DEF(false)>]deleteInput:bool) : Guid Rarr =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let pieces = mesh.SplitDisjointPieces()
        let breps =  rarr { for piece in pieces do yield Brep.CreateFromMesh(piece, trimmedTriangles) }
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let attr = rhobj.Attributes
        let ids =  rarr { for brep in breps do yield State.Doc.Objects.AddBrep(brep, attr) }
        if deleteInput then State.Doc.Objects.Delete(rhobj, quiet=true)|> ignore
        State.Doc.Views.Redraw()
        ids


    ///<summary>Returns number of triangular faces of a Mesh.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(int) The number of triangular Mesh faces.</returns>
    [<Extension>]
    static member MeshTriangleCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Faces.TriangleCount


    ///<summary>Returns vertex colors of a Mesh.</summary>
    ///<param name="meshId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(Drawing.Color Rarr) The current vertex colors.</returns>
    [<Extension>]
    static member MeshVertexColors(meshId:Guid) : Drawing.Color Rarr= //GET
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        rarr { for i = 0 to mesh.VertexColors.Count - 1 do mesh.VertexColors.[i] }


    ///<summary>Modifies vertex colors of a Mesh.</summary>
    ///<param name="meshId">(Guid) Identifier of a Mesh object</param>
    ///<param name="colors">(Drawing.Color seq), optional) A list of color values. Note, for each vertex, there must
    ///    be a corresponding vertex color. If the value is null or empty list , then any
    ///    existing vertex colors will be removed from the Mesh</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member MeshVertexColors(meshId:Guid, colors:Drawing.Color seq) : unit = //SET
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        if colors|> isNull || Seq.isEmpty colors   then
            mesh.VertexColors.Clear()
        else
            let colorcount = Seq.length(colors)
            if colorcount <> mesh.Vertices.Count then
                RhinoScriptingException.Raise "RhinoScriptSyntax.Length of colors must match vertex count.  meshId:'%s' colors:'%A'" (Print.guid meshId) colors
            mesh.VertexColors.Clear()
            for c in colors do mesh.VertexColors.Add(c) |> ignore
        State.Doc.Objects.Replace(meshId, mesh) |> ignore
        State.Doc.Views.Redraw()



    ///<summary>Returns the vertex count of a Mesh.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(int) The number of Mesh vertices.</returns>
    [<Extension>]
    static member MeshVertexCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Vertices.Count


    ///<summary>Returns the Mesh faces that share a specified Mesh vertex.</summary>
    ///<param name="meshId">(Guid) Identifier of a Mesh object</param>
    ///<param name="vertexIndex">(int) Index of the Mesh vertex to find faces for</param>
    ///<returns>(int array) face indices.</returns>
    [<Extension>]
    static member MeshVertexFaces(meshId:Guid, vertexIndex:int) : int array =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        mesh.Vertices.GetVertexFaces(vertexIndex)


    ///<summary>Returns the vertex unit normal for each vertex of a Mesh.</summary>
    ///<param name="meshId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(Vector3d Rarr) List of vertex normals, (empty list if no normals exist).</returns>
    [<Extension>]
    static member MeshVertexNormals(meshId:Guid) : Vector3d Rarr =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let count = mesh.Normals.Count
        if count<1 then rarr {()}
        else rarr { for i = 0 to count - 1 do Vector3d(mesh.Normals.[i])}


    ///<summary>Returns the vertices of a Mesh.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(Point3d Rarr) vertex points in the Mesh.</returns>
    [<Extension>]
    static member MeshVertices(objectId:Guid) : Point3d Rarr =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let count = mesh.Vertices.Count
        let rc = Rarr()
        for i = 0 to count - 1 do
            let vertex = mesh.Vertices.[i]
            rc.Add(Point3d(vertex))
        rc


    ///<summary>Returns the approximate volume of one or more closed Meshes.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of one or more Mesh objects</param>
    ///<returns>(float)  total volume of all Meshes.</returns>
    [<Extension>]
    static member MeshVolume(objectIds:Guid seq) : float =
        let mutable totalvolume  = 0.0
        for objectId in objectIds do
            let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
            let mp = VolumeMassProperties.Compute(mesh)
            if notNull mp then
                totalvolume <- totalvolume + mp.Volume
            else
                RhinoScriptingException.Raise "RhinoScriptSyntax.MeshVolume failed on objectId:'%s'" (Print.guid objectId)
        totalvolume


    ///<summary>Calculates the volume centroid of a Mesh.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(Point3d) Point3d representing the volume centroid.</returns>
    [<Extension>]
    static member MeshVolumeCentroid(objectId:Guid) : Point3d =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mp = VolumeMassProperties.Compute(mesh)
        if notNull mp then mp.Centroid
        else RhinoScriptingException.Raise "RhinoScriptSyntax.MeshVolumeCentroid failed.  objectId:'%s'" (Print.guid objectId)


    ///<summary>Pulls a Curve to a Mesh. The function makes a Polyline approximation of
    ///    the input Curve and gets the closest point on the Mesh for each point on
    ///    the polyline. Then it "connects the points" to create a Polyline on the Mesh.</summary>
    ///<param name="meshId">(Guid) Identifier of Mesh that pulls</param>
    ///<param name="curveId">(Guid) Identifier of Curve to pull</param>
    ///<returns>(Guid) identifier new Curve.</returns>
    [<Extension>]
    static member PullCurveToMesh(meshId:Guid, curveId:Guid) : Guid =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let tol = State.Doc.ModelAbsoluteTolerance
        let polyline = curve.PullToMesh(mesh, tol)
        if isNull polyline then RhinoScriptingException.Raise "RhinoScriptSyntax.PullCurveToMesh failed.  meshId:'%s' curveId:'%s'" (Print.guid meshId)  (Print.guid curveId)
        let rc = State.Doc.Objects.AddCurve(polyline)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.Unable to add polyline to document.  meshId:'%s' curveId:'%s'" (Print.guid meshId) (Print.guid curveId)
        State.Doc.Views.Redraw()
        rc


    ///<summary>Splits up a Mesh into its unconnected pieces.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the input object</param>
    ///<returns>(Guid Rarr) identifiers for the new Meshes.</returns>
    [<Extension>]
    static member SplitDisjointMesh(objectId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid Rarr =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let pieces = mesh.SplitDisjointPieces()
        let rc =  rarr { for piece in pieces do yield State.Doc.Objects.AddMesh(piece) }
        if rc.Count <> 0 && deleteInput then
            //id = RhinoScriptSyntax.Coerceguid(objectId)
            State.Doc.Objects.Delete(objectId, true) |> ignore
        State.Doc.Views.Redraw()
        rc


    ///<summary>Fixes inconsistencies in the directions of faces of a Mesh.</summary>
    ///<param name="objectId">(Guid) Identifier of a Mesh object</param>
    ///<returns>(int) The number of faces that were modified.</returns>
    [<Extension>]
    static member UnifyMeshNormals(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = mesh.UnifyNormals()
        if rc>0 then
            //id = RhinoScriptSyntax.Coerceguid(objectId)
            State.Doc.Objects.Replace(objectId, mesh)|> ignore
            State.Doc.Views.Redraw()
        rc


