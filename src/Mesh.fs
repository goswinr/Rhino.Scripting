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
            if not<| Doc.Objects.Delete(RhinoScriptSyntax.CoerceGuid(objectId),true)then failwithf "Rhino.Scripting: AddPlanarMesh failed to delete input.  objectId:'%A' deleteInput:'%A'" objectId deleteInput
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


