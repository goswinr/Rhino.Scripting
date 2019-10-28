namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
open System.Collections.Generic

[<AutoOpen>]
module ExtensionsMesh =
  
  type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Add a mesh object to the document</summary>
    ///<param name="vertices">(Point3d seq) List of 3D points defining the vertices of the mesh</param>
    ///<param name="faceVertices">(seq<IList<int>>) List containing lists of 3 or 4 numbers that define the
    ///  vertex indices for each face of the mesh. If the third a fourth vertex
    ///    indices of a face are identical, a triangular face will be created.</param>
    ///<param name="vertexNormals">(Vector3f seq) Optional, List of 3D vectors defining the vertex normals of
    ///  the mesh. Note, for every vertex, there must be a corresponding vertex
    ///  normal</param>
    ///<param name="textureCoordinates">(Point2f seq) Optional,List of 2D texture coordinates. For every
    ///  vertex, there must be a corresponding texture coordinate</param>
    ///<param name="vertexColors">(Drawing.Color seq) Optional, A list of color values. For every vertex,
    ///  there must be a corresponding vertex color</param>
    ///<returns>(Guid) Identifier of the new object</returns>
    static member AddMesh( vertices:Point3d seq, //TODO how to construct Ngon Mesh ???
                           faceVertices:seq<IList<int>>, // TODO teset if nested arrays are accepted  
                           [<OPT;DEF(null:Vector3f seq)>]vertexNormals:Vector3f seq, 
                           [<OPT;DEF(null:Point2f seq)>]textureCoordinates:Point2f seq, 
                           [<OPT;DEF(null:Drawing.Color seq)>]vertexColors:Drawing.Color seq) : Guid =
        let mesh = new Mesh()
        for pt in vertices do 
            mesh.Vertices.Add(pt) |> ignore
        
        for face in faceVertices do
            if Seq.length(face)<4 then
                mesh.Faces.AddFace(face.[0], face.[1], face.[2]) |> ignore
            else 
                mesh.Faces.AddFace(face.[0], face.[1], face.[2], face.[3]) |> ignore
        
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
        if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add mesh to document.  vertices:'%A' faceVertices:'%A' vertexNormals:'%A' textureCoordinates:'%A' vertexColors:'%A'" vertices faceVertices vertexNormals textureCoordinates vertexColors
        Doc.Views.Redraw()
        rc
    (*
    def AddMesh(vertices, face_vertices, vertex_normals=None, texture_coordinates=None, vertex_colors=None):
        '''Add a mesh object to the document
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
        '''
    
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


    [<EXT>]
    ///<summary>Creates a planar mesh from a closed, planar curve</summary>
    ///<param name="objectId">(Guid) Identifier of a closed, planar curve</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///If True, delete the input curve defined by objectId</param>
    ///<returns>(Guid) id of the new mesh on success</returns>
    static member AddPlanarMesh(objectId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(objectId)
        let tolerance = Doc.ModelAbsoluteTolerance
        let mesh = Mesh.CreateFromPlanarBoundary(curve, MeshingParameters.Default, tolerance)
        if isNull mesh then failwithf "Rhino.Scripting: AddPlanarMesh failed.  objectId:'%A' deleteInput:'%A'" objectId deleteInput
        if deleteInput then 
            let ob = RhinoScriptSyntax.CoerceGuid(objectId)
            if not<| Doc.Objects.Delete(ob,true)then failwithf "Rhino.Scripting: AddPlanarMesh failed to delete input.  objectId:'%A' deleteInput:'%A'" objectId deleteInput
        let rc = Doc.Objects.AddMesh(mesh)
        if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add mesh to document.  objectId:'%A' deleteInput:'%A'" objectId deleteInput
        Doc.Views.Redraw()
        rc
    (*
    def AddPlanarMesh(object_id, delete_input=False):
        '''Creates a planar mesh from a closed, planar curve
        Parameters:
          object_id (guid): identifier of a closed, planar curve
          delete_input (bool, optional) if True, delete the input curve defined by object_id
        Returns:
          guid: id of the new mesh on success
          None: on error
        '''
    
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


    [<EXT>]
    ///<summary>Calculates the intersection of a curve object and a mesh object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="meshId">(Guid) Identifier or a mesh object</param>
    ///<returns>(array<Point3d>*array<int>) two arrays as tuple:
    ///      [0] = point of intersection
    ///      [1] = mesh face index where intersection lies</returns>
    static member CurveMeshIntersection( curveId:Guid, 
                                         meshId:Guid) : array<Point3d>*array<int> =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        let tolerance = Doc.ModelAbsoluteTolerance
        let polylinecurve = curve.ToPolyline(0 , 0 , 0.0 , 0.0 , 0.0, tolerance , 0.0 , 0.0 , true)
        let pts, faceids = Intersect.Intersection.MeshPolyline(mesh, polylinecurve)
        if isNull pts then failwithf "Rhino.Scripting: CurveMeshIntersection failed.  curveId:'%A' meshId:'%A' " curveId meshId         
        pts, faceids
    (*
    def CurveMeshIntersection(curve_id, mesh_id, return_faces=False):
        '''Calculates the intersection of a curve object and a mesh object
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
        '''
    
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


    [<EXT>]
    ///<summary>Returns number of meshes that could be created by calling SplitDisjointMesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) The number of meshes that could be created</returns>
    static member DisjointMeshCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.DisjointMeshCount
    (*
    def DisjointMeshCount(object_id):
        '''Returns number of meshes that could be created by calling SplitDisjointMesh
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          number: The number of meshes that could be created
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.DisjointMeshCount
    *)


    [<EXT>]
    ///<summary>Creates curves that duplicates a mesh border</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Guid ResizeArray) list of curve ids on success</returns>
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
    (*
    def DuplicateMeshBorder(mesh_id):
        '''Creates curves that duplicates a mesh border
        Parameters:
          mesh_id (guid): identifier of a mesh object
        Returns:
          list(guid, ...): list of curve ids on success
          None: on error
        '''
    
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


    [<EXT>]
    ///<summary>Explodes a mesh object, or mesh objects int submeshes. A submesh is a
    ///  collection of mesh faces that are contained within a closed loop of
    ///  unwelded mesh edges. Unwelded mesh edges are where the mesh faces that
    ///  share the edge have unique mesh vertices (not mesh topology vertices)
    ///  at both ends of the edge</summary>
    ///<param name="meshIds">(Guid seq) List of mesh identifiers</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid ResizeArray) List of resulting objects after explode.</returns>
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
    (*
    def ExplodeMeshes(mesh_ids, delete=False):
        '''Explodes a mesh object, or mesh objects int submeshes. A submesh is a
        collection of mesh faces that are contained within a closed loop of
        unwelded mesh edges. Unwelded mesh edges are where the mesh faces that
        share the edge have unique mesh vertices (not mesh topology vertices)
        at both ends of the edge
        Parameters:
          mesh_ids ([guid, ...]): list of mesh identifiers
          delete (bool, optional): delete the input meshes
        Returns:
          list(guid, ...): List of resulting objects after explode.
        '''
    
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


    [<EXT>]
    ///<summary>Verifies if an object is a mesh</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsMesh(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceMesh(objectId)
        |> Option.isSome
    (*
    def IsMesh(object_id):
        '''Verifies if an object is a mesh
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if successful, otherwise False
        '''
    
        mesh = rhutil.coercemesh(object_id)
        return mesh is not None
    *)


    [<EXT>]
    ///<summary>Verifies a mesh object is closed</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsMeshClosed(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceMesh(objectId) with 
        | Some mesh -> mesh.IsClosed
        | None -> false
    (*
    def IsMeshClosed(object_id):
        '''Verifies a mesh object is closed
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.IsClosed
    *)


    [<EXT>]
    ///<summary>Verifies a mesh object is manifold. A mesh for which every edge is shared
    ///  by at most two faces is called manifold. If a mesh has at least one edge
    ///  that is shared by more than two faces, then that mesh is called non-manifold</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True, otherwise False.</returns>
    static member IsMeshManifold(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceMesh(objectId) with 
        | Some mesh -> mesh.IsManifold(true)  |> t1
        | None -> false

    (*
    def IsMeshManifold(object_id):
        '''Verifies a mesh object is manifold. A mesh for which every edge is shared
        by at most two faces is called manifold. If a mesh has at least one edge
        that is shared by more than two faces, then that mesh is called non-manifold
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        rc = mesh.IsManifold(True)
        return rc[0]
    *)


    [<EXT>]
    ///<summary>Verifies a point is on a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="point">(Point3d) Test point</param>
    ///<param name="tolerance">(float) Optional, Defalut Value <c>RhinoMath.SqrtEpsilon</c>
    ///  The testing tolerance /param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsPointOnMesh(    objectId:Guid, 
                                    point:Point3d,
                                    [<OPT;DEF(0.0)>]tolerance:float): bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        let maxdistance = max tolerance Rhino.RhinoMath.SqrtEpsilon
        let pt = ref Point3d.Origin
        let face = mesh.ClosestPoint(point,pt, maxdistance)
        face>=0
    (*
    def IsPointOnMesh(object_id, point):
        '''Verifies a point is on a mesh
        Parameters:
          object_id (guid): identifier of a mesh object
          point (point): test point
        Returns:
          bool: True if successful, otherwise False.
          None: on error.
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        point = rhutil.coerce3dpoint(point, True)
        max_distance = Rhino.RhinoMath.SqrtEpsilon
        face, pt = mesh.ClosestPoint(point, max_distance)
        return face>=0
    *)


    [<EXT>]
    ///<summary>Joins two or or more mesh objects together</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of two or more mesh objects</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input after joining</param>
    ///<returns>(Guid) identifier of newly created mesh on success</returns>
    static member JoinMeshes(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let meshes =  resizeArray { for objectId in objectIds do yield RhinoScriptSyntax.CoerceMesh(objectId) } 
        let joinedmesh = new Mesh()
        joinedmesh.Append(meshes)
        let rc = Doc.Objects.AddMesh(joinedmesh)
        if deleteInput then
            for objectId in objectIds do
                //guid = RhinoScriptSyntax.Coerceguid(objectId)
                Doc.Objects.Delete(objectId,true) |> ignore
        Doc.Views.Redraw()
        rc
    (*
    def JoinMeshes(object_ids, delete_input=False):
        '''Joins two or or more mesh objects together
        Parameters:
          object_ids ([guid, ...]): identifiers of two or more mesh objects
          delete_input (bool, optional): delete input after joining
        Returns:
          guid: identifier of newly created mesh on success
        '''
    
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


    [<EXT>]
    ///<summary>Returns approximate area of onemesh object</summary>
    ///<param name="objectId">(Guid seq) Identifiers of one or more mesh objects</param>
    ///<returns>(float) total area of mesh</returns>
    static member MeshArea(objectId:Guid ) :float =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mp = AreaMassProperties.Compute(mesh)
        if notNull mp then
            mp.Area
        else
            failwithf "Rhino.Scripting: MeshArea failed.  objectId:'%A'" objectId
        
    (*
    def MeshArea(object_ids):
        '''Returns approximate area of one or more mesh objects
        Parameters:
          object_ids ([guid, ...]): identifiers of one or more mesh objects
        Returns:
          list(number, number, number): if successful where
            [0] = number of meshes used in calculation
            [1] = total area of all meshes
            [2] = the error estimate
          None: if not successful
        '''
    
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


    [<EXT>]
    ///<summary>Calculates the area centroid of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d) representing the area centroid</returns>
    static member MeshAreaCentroid(objectId:Guid) : Point3d =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let mp = AreaMassProperties.Compute(mesh)
        if mp|> isNull  then failwithf "Rhino.Scripting: MeshAreaCentroid failed.  objectId:'%A'" objectId
        mp.Centroid
    (*
    def MeshAreaCentroid(object_id):
        '''Calculates the area centroid of a mesh object
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          point: representing the area centroid if successful
          None: on error
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        mp = Rhino.Geometry.AreaMassProperties.Compute(mesh)
        if mp is None: return scriptcontext.errorhandler()
        return mp.Centroid
    *)


    [<EXT>]
    ///<summary>Performs boolean difference operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid seq) Meshes to subtract from </param>
    ///<param name="input1">(Guid seq) Meshes to subtract with</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>   
    ///<returns>(Guid ResizeArray) identifiers of newly created meshes</returns>
    static member MeshBooleanDifference( input0:Guid seq, 
                                         input1:Guid seq, 
                                         [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =        
        let meshes0 =  resizeArray { for objectId in input0 do yield RhinoScriptSyntax.CoerceMesh(objectId) } 
        let meshes1 =  resizeArray { for objectId in input1 do yield RhinoScriptSyntax.CoerceMesh(objectId) } 
        if meshes0.Count=0 || meshes1.Count=0 then failwithf "Rhino.Scripting.MeshBooleanDifference: No meshes to work with.  input0:'%A' input1:'%A' deleteInput:'%A' " input0 input1 deleteInput 
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
    (*
    def MeshBooleanDifference(input0, input1, delete_input=True, tolerance=None):
        '''Performs boolean difference operation on two sets of input meshes
        Parameters:
          input0, input1 (guid): identifiers of meshes
          delete_input (bool, optional): delete the input meshes
          tolerance (float, optional): a positive tolerance value, or None to use the default of the document.
        Returns:
          list(guid, ...): identifiers of newly created meshes
        '''
    
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


    [<EXT>]
    ///<summary>Performs boolean intersection operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid seq) Meshes to intersect </param>
    ///<param name="input1">(Guid seq) Meshes to intersect</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid ResizeArray) identifiers of new meshes on success</returns>
    static member MeshBooleanIntersection( input0:Guid seq, 
                                           input1:Guid seq, 
                                           [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let meshes0 =  resizeArray { for objectId in input0 do yield RhinoScriptSyntax.CoerceMesh(objectId) } 
        let meshes1 =  resizeArray { for objectId in input1 do yield RhinoScriptSyntax.CoerceMesh(objectId) } 
        if meshes0.Count=0 || meshes1.Count=0 then failwithf "Rhino.Scripting.MeshBooleanIntersection: No meshes to work with.  input0:'%A' input1:'%A' deleteInput:'%A' " input0 input1 deleteInput 
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
        
    (*
    def MeshBooleanIntersection(input0, input1, delete_input=True):
        '''Performs boolean intersection operation on two sets of input meshes
        Parameters:
          input0, input1 (guid): identifiers of meshes
          delete_input (bool, optional): delete the input meshes
        Returns:
          list(guid, ...): identifiers of new meshes on success
        '''
    
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


    [<EXT>]
    ///<summary>Performs boolean split operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid seq) Meshes to split from </param>
    ///<param name="input1">(Guid seq) Meshes to split with</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid ResizeArray) identifiers of new meshes on success</returns>
    static member MeshBooleanSplit( input0:Guid seq, 
                                    input1:Guid seq, 
                                    [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let meshes0 =  resizeArray { for objectId in input0 do yield RhinoScriptSyntax.CoerceMesh(objectId) } 
        let meshes1 =  resizeArray { for objectId in input1 do yield RhinoScriptSyntax.CoerceMesh(objectId) } 
        if meshes0.Count=0 || meshes1.Count=0 then failwithf "Rhino.Scripting.CreateBooleanSplit: No meshes to work with.  input0:'%A' input1:'%A' deleteInput:'%A' " input0 input1 deleteInput
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

    (*
    def MeshBooleanSplit(input0, input1, delete_input=True):
        '''Performs boolean split operation on two sets of input meshes
        Parameters:
          input0, input1 (guid): identifiers of meshes
          delete_input (bool, optional): delete the input meshes
        Returns:
          list(guid, ...): identifiers of new meshes on success
          None: on error
        '''
    
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


    [<EXT>]
    ///<summary>Performs boolean union operation on a set of input meshes</summary>
    ///<param name="meshIds">(Guid seq) Identifiers of meshes</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input meshes</param>
    ///<returns>(Guid ResizeArray) identifiers of new meshes</returns>
    static member MeshBooleanUnion(meshIds:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        if Seq.length(meshIds)<2 then failwithf "Rhino.Scripting: MeshIds must contain at least 2 meshes.  meshIds:'%A' deleteInput:'%A'" meshIds deleteInput
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
    (*
    def MeshBooleanUnion(mesh_ids, delete_input=True):
        '''Performs boolean union operation on a set of input meshes
        Parameters:
          mesh_ids ([guid, ...]): identifiers of meshes
          delete_input (bool, optional): delete the input meshes
        Returns:
          list(guid, ...): identifiers of new meshes
        '''
    
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


    [<EXT>]
    ///<summary>Returns the point on a mesh that is closest to a test point</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="point">(Point3d) Point to test</param>
    ///<param name="maximumDistance">(float) Optional, Upper bound used for closest point calculation.
    ///  If you are only interested in finding a point Q on the mesh when
    ///  point.DistanceTo(Q) < maximumDistance, then set maximumDistance to
    ///  that value</param>
    ///<returns>(Point3d * float) containing the results of the calculation where
    ///  [0] = the 3-D point on the mesh
    ///  [1] = the index of the mesh face on which the 3-D point lies</returns>
    static member MeshClosestPoint( objectId:Guid, 
                                    point:Point3d, 
                                    [<OPT;DEF(0.0)>]maximumDistance:float) : Point3d * int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)  
        let pt = ref Point3d.Origin
        let face = mesh.ClosestPoint(point, pt, maximumDistance)
        if face<0 then failwithf "Rhino.Scripting: MeshClosestPoint failed.  objectId:'%A' point:'%A' maximumDistance:'%A'" objectId point maximumDistance
        !pt, face
    (*
    def MeshClosestPoint(object_id, point, maximum_distance=None):
        '''Returns the point on a mesh that is closest to a test point
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
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        point = rhutil.coerce3dpoint(point, True)
        tolerance=maximum_distance if maximum_distance else 0.0
        face, closest_point = mesh.ClosestPoint(point, tolerance)
        if face<0: return scriptcontext.errorhandler()
        return closest_point, face
    *)


    [<EXT>]
    ///<summary>Returns the center of each face of the mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of a mesh object</param>
    ///<returns>(Point3d ResizeArray) points defining the center of each face</returns>
    static member MeshFaceCenters(meshId:Guid) : Point3d ResizeArray =
        let mesh = RhinoScriptSyntax.CoerceMesh(meshId)
        resizeArray {for i in range(mesh.Faces.Count) do mesh.Faces.GetFaceCenter(i) }
    (*
    def MeshFaceCenters(mesh_id):
        '''Returns the center of each face of the mesh object
        Parameters:
          mesh_id (guid): identifier of a mesh object
        Returns:
          list(point, ...): points defining the center of each face
        '''
    
        mesh = rhutil.coercemesh(mesh_id, True)
        return [mesh.Faces.GetFaceCenter(i) for i in range(mesh.Faces.Count)]
    *)


    [<EXT>]
    ///<summary>Returns total face count of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(int) the number of mesh faces</returns>
    static member MeshFaceCount(objectId:Guid) : int =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.Faces.Count
    (*
    def MeshFaceCount(object_id):
        '''Returns total face count of a mesh object
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          number: the number of mesh faces if successful
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.Faces.Count
    *)


    [<EXT>]
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
    (*
    def MeshFaceNormals(mesh_id):
        '''Returns the face unit normal for each face of a mesh object
        Parameters:
          mesh_id (guid): identifier of a mesh object
        Returns:
          list(vector, ...): 3D vectors that define the face unit normals of the mesh
          None: on error
        '''
    
        mesh = rhutil.coercemesh(mesh_id, True)
        if mesh.FaceNormals.Count != mesh.Faces.Count:
            mesh.FaceNormals.ComputeFaceNormals()
        rc = []
        for i in xrange(mesh.FaceNormals.Count):
            normal = mesh.FaceNormals[i]
            rc.append(Rhino.Geometry.Vector3d(normal))
        return rc
    *)


    [<EXT>]
    ///<summary>Returns face vertices of a mesh</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<param name="faceType">(bool) Optional, Default Value: <c>true</c>
    ///The face type to be returned. True = both triangles
    ///  and quads. False = Quads are broken down into triangles</param>
    ///<returns>(Point3d ResizeArray) 3D points that define the face vertices of the mesh. If
    ///  face_type is True, then faces are returned as both quads and triangles
    ///  (4 3D points). For triangles, the third and fourth vertex will be
    ///  identical. If face_type is False, then faces are returned as only
    ///  triangles(3 3D points). Quads will be converted to triangles.</returns>
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
    (*
    def MeshFaces(object_id, face_type=True):
        '''Returns face vertices of a mesh
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
        '''
    
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


    ///<summary>Returns the vertex indices of all faces of a Ngon mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(ResizeArray<ResizeArray<int>) containing a nested List that define the vertex indices for
    ///  each face of the mesh. Ngons, quad and triangle faces are returned.</returns>
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


    [<EXT>]
    ///<summary>Returns the vertex indices of all faces of a mesh object</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(ResizeArray<int*int*int*int>) containing tuples of 4 numbers that define the vertex indices for
    ///  each face of the mesh. Both quad and triangle faces are returned. If the
    ///  third and fourth vertex indices are identical, the face is a triangle.</returns>
    static member MeshFaceVertices(objectId:Guid) : ResizeArray<int*int*int*int> =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        let rc = ResizeArray()
        for i in range(mesh.Faces.Count) do
            let face = mesh.Faces.GetFace(i)
            rc.Add( (face.A, face.B, face.C, face.D) ) //TODO add ngon support
        rc
    (*
    def MeshFaceVertices(object_id):
        '''Returns the vertex indices of all faces of a mesh object
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          list((number, number, number, number), ...): containing tuples of 4 numbers that define the vertex indices for
          each face of the mesh. Both quad and triangle faces are returned. If the
          third and fourth vertex indices are identical, the face is a triangle.
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        rc = []
        for i in xrange(mesh.Faces.Count):
            face = mesh.Faces.GetFace(i)
            rc.append( (face.A, face.B, face.C, face.D) )
        return rc
    *)


    [<EXT>]
    ///<summary>Verifies a mesh object has face normals</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasFaceNormals(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.FaceNormals.Count>0
    (*
    def MeshHasFaceNormals(object_id):
        '''Verifies a mesh object has face normals
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.FaceNormals.Count>0
    *)


    [<EXT>]
    ///<summary>Verifies a mesh object has texture coordinates</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasTextureCoordinates(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.TextureCoordinates.Count>0
    (*
    def MeshHasTextureCoordinates(object_id):
        '''Verifies a mesh object has texture coordinates
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.TextureCoordinates.Count>0
    *)


    [<EXT>]
    ///<summary>Verifies a mesh object has vertex colors</summary>
    ///<param name="objectId">(Guid) Identifier of a mesh object</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasVertexColors(objectId:Guid) : bool =
        let mesh = RhinoScriptSyntax.CoerceMesh(objectId)
        mesh.VertexColors.Count>0
    (*
    def MeshHasVertexColors(object_id):
        '''Verifies a mesh object has vertex colors
        Parameters:
          object_id (guid): identifier of a mesh object
        Returns:
          bool: True if successful, otherwise False.
        '''
    
        mesh = rhutil.coercemesh(object_id, True)
        return mesh.VertexColors.Count>0
    *)


