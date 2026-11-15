// Tests for Src/Scripting_Mesh.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Mesh"

let vec x y z = Vector3d(x, y, z)
let rgb (c:Drawing.Color) = (int c.R, int c.G, int c.B)
let geo (id:Guid) = (doc().Objects.FindId id).Geometry
let meshGeo (id:Guid) = geo id :?> Mesh
/// a welded mesh box with 8 vertices (Mesh.CreateFromBox makes 24 unwelded vertices)
let weldedBox (dx:float) =
    let m = Mesh.CreateFromBox(Box(Plane.WorldXY, Interval(dx, dx + 1.), Interval(0., 1.), Interval(0., 1.)), 1, 1, 1)
    m.Vertices.CombineIdentical(true, true) |> ignore
    m.Normals.ComputeNormals() |> ignore
    m.Compact() |> ignore
    doc().Objects.AddMesh m
/// a finer welded mesh box, boolean operations on boxes with one quad per side give inaccurate results
let fine (dx:float) =
    let m = Mesh.CreateFromBox(Box(Plane.WorldXY, Interval(dx, dx + 1.), Interval(0., 1.), Interval(0., 1.)), 8, 8, 8)
    m.Vertices.CombineIdentical(true, true) |> ignore
    m.Normals.ComputeNormals() |> ignore
    doc().Objects.AddMesh m
/// a finer welded box that is moved in all three directions, so that it has no faces in the same plane as the other box
let fineOff () =
    let m = Mesh.CreateFromBox(Box(Plane.WorldXY, Interval(0.5, 1.5), Interval(0.3, 1.3), Interval(0.3, 1.3)), 8, 8, 8)
    m.Vertices.CombineIdentical(true, true) |> ignore
    m.Normals.ComputeNormals() |> ignore
    doc().Objects.AddMesh m
let mbox () = weldedBox 0.
/// a welded mesh box that is moved by dx in x direction
let boxAt (dx:float) = weldedBox dx
let faces (f: int list list) : int[][] = f |> List.map Array.ofList |> Array.ofList
/// two quads sharing an edge, in the XY plane
let twoQuads () =
    rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0.; pt 0. 1. 0.; pt 2. 0. 0.; pt 2. 1. 0. ], faces ([ [ 0; 1; 2; 3 ]; [ 1; 4; 5; 2 ] ]))
let nonMesh () = rs.AddPoint(0., 0., 0.)

/// calls a Guid overload and the Mesh overload and checks that both give the same result
let both (name:string) (byId: Guid -> 'a) (byGeo: Mesh -> 'a) (id:Guid) (expected: 'a) =
    eq (name + " (by id)") expected (byId id)
    eq (name + " (by geometry)") expected (byGeo (meshGeo id))

// ------------------------------- adding -------------------------------

test "AddMesh" (fun () ->
    // faces as lists of indices
    let id = twoQuads ()
    ok "is a mesh" (rs.IsMesh id)
    eq "vertices" 6 (rs.MeshVertexCount id)
    eq "faces" 2 (rs.MeshFaceCount id)
    // triangles and quads together, faces as a list of tuples
    let tri = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 0. 1. 0. ], [ (0, 1, 2, 2) ])
    eq "a triangle if the third and fourth index are the same" 1 (rs.MeshTriangleCount tri)
    let quad = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0.; pt 0. 1. 0. ], [ (0, 1, 2, 3) ])
    eq "a quad" 1 (rs.MeshQuadCount quad)
    // normals, texture coordinates and colors
    let full =
        rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0.; pt 0. 1. 0. ], faces ([ [ 0; 1; 2; 3 ] ]),
                   [ Vector3f(0.f, 0.f, 1.f); Vector3f(0.f, 0.f, 1.f); Vector3f(0.f, 0.f, 1.f); Vector3f(0.f, 0.f, 1.f) ],
                   [ Point2f(0.f, 0.f); Point2f(1.f, 0.f); Point2f(1.f, 1.f); Point2f(0.f, 1.f) ],
                   [ Drawing.Color.Red; Drawing.Color.Green; Drawing.Color.Blue; Drawing.Color.White ])
    ok "vertex normals" (rs.MeshHasVertexNormals full)
    ok "texture coordinates" (rs.MeshHasTextureCoordinates full)
    ok "vertex colors" (rs.MeshHasVertexColors full)
    let plain = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0.; pt 0. 1. 0. ], faces ([ [ 0; 1; 2; 3 ] ]))
    ok "no texture coordinates by default" (not (rs.MeshHasTextureCoordinates plain))
    ok "no vertex colors by default" (not (rs.MeshHasVertexColors plain))
    throwsRs "a face with five indices" (fun () -> rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ], faces ([ [ 0; 1; 2; 0; 1 ] ])))
    throwsRs "a face with two indices" (fun () -> rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ], faces ([ [ 0; 1 ] ])))
    )

test "AddMeshQuad" (fun () ->
    let id = rs.AddMeshQuad(pt 0. 0. 0., pt 1. 0. 0., pt 1. 1. 0., pt 0. 1. 0.)
    ok "is a mesh" (rs.IsMesh id)
    eq "one quad" 1 (rs.MeshQuadCount id)
    near "area" 1. (rs.MeshArea id)
    )

test "AddMeshTriangle" (fun () ->
    let id = rs.AddMeshTriangle(pt 0. 0. 0., pt 2. 0. 0., pt 0. 2. 0.)
    ok "is a mesh" (rs.IsMesh id)
    eq "one triangle" 1 (rs.MeshTriangleCount id)
    near "area" 2. (rs.MeshArea id)
    )

test "AddPlanarMesh" (fun () ->
    let c = addSquare 4.
    let m = rs.AddPlanarMesh c
    ok "is a mesh" (rs.IsMesh m)
    near "area of the mesh" 16. (rs.MeshArea m)
    ok "the input is kept by default" (rs.IsObject c)
    let c2 = addSquare 2.
    rs.AddPlanarMesh(c2, true) |> ignore
    ok "the input is deleted on request" (not (rs.IsObject c2))
    throwsRs "open curve" (fun () -> rs.AddPlanarMesh (rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.)))
    throws "not a curve" (fun () -> rs.AddPlanarMesh (nonMesh ()))
    )

test "MakePlanarMesh" (fun () ->
    let n = (rs.AllObjects()).Count
    let m = rs.MakePlanarMesh (new PolylineCurve([ pt 0. 0. 0.; pt 4. 0. 0.; pt 4. 4. 0.; pt 0. 4. 0.; pt 0. 0. 0. ]) :> Curve)
    near "area of the mesh" 16. (rs.MeshArea m)
    eq "nothing is added to the document" n (rs.AllObjects()).Count
    throwsRs "open curve" (fun () -> rs.MakePlanarMesh (new LineCurve(pt 0. 0. 0., pt 1. 0. 0.) :> Curve))
    )

test "IsMesh" (fun () ->
    let m = mbox ()
    ok "mesh" (rs.IsMesh m)
    ok "mesh geometry" (rs.IsMesh (geo m))
    ok "a polysurface is no mesh" (not (rs.IsMesh (addBox 1. 1. 1.)))
    ok "unknown id" (not (rs.IsMesh (Guid.NewGuid())))
    )

// ------------------------------- counts and properties -------------------------------

test "MeshVertexCount" (fun () -> both "8 vertices" (fun (i:Guid) -> rs.MeshVertexCount i) (fun (m:Mesh) -> rs.MeshVertexCount m) (mbox ()) 8; throwsRs "not a mesh" (fun () -> rs.MeshVertexCount (nonMesh ())))
test "MeshFaceCount" (fun () -> both "6 faces" (fun (i:Guid) -> rs.MeshFaceCount i) (fun (m:Mesh) -> rs.MeshFaceCount m) (mbox ()) 6; throwsRs "not a mesh" (fun () -> rs.MeshFaceCount (nonMesh ())))
test "MeshQuadCount" (fun () -> both "6 quads" (fun (i:Guid) -> rs.MeshQuadCount i) (fun (m:Mesh) -> rs.MeshQuadCount m) (mbox ()) 6; throwsRs "not a mesh" (fun () -> rs.MeshQuadCount (nonMesh ())))
test "MeshTriangleCount" (fun () ->
    both "no triangles" (fun (i:Guid) -> rs.MeshTriangleCount i) (fun (m:Mesh) -> rs.MeshTriangleCount m) (mbox ()) 0
    both "one triangle" (fun (i:Guid) -> rs.MeshTriangleCount i) (fun (m:Mesh) -> rs.MeshTriangleCount m) (rs.AddMeshTriangle(pt 0. 0. 0., pt 1. 0. 0., pt 0. 1. 0.)) 1
    throwsRs "not a mesh" (fun () -> rs.MeshTriangleCount (nonMesh ()))
    )

test "MeshVertices" (fun () ->
    let m = mbox ()
    let byId = rs.MeshVertices m
    eq "by id: count" 8 byId.Count
    eq "by geometry: count" 8 (rs.MeshVertices (meshGeo m)).Count
    ok "all vertices are corners of the box" (byId |> Seq.forall (fun v -> (v.X = 0. || v.X = 1.) && (v.Y = 0. || v.Y = 1.) && (v.Z = 0. || v.Z = 1.)))
    throwsRs "not a mesh" (fun () -> rs.MeshVertices (nonMesh ()))
    )

test "MeshFaceVertices" (fun () ->
    let m = mbox ()
    let f = rs.MeshFaceVertices m
    eq "by id: six faces" 6 f.Count
    eq "by geometry: six faces" 6 (rs.MeshFaceVertices (meshGeo m)).Count
    ok "quads have four different indices" (f |> Seq.forall (fun (a, b, c, d) -> c <> d))
    let t = rs.MeshFaceVertices (rs.AddMeshTriangle(pt 0. 0. 0., pt 1. 0. 0., pt 0. 1. 0.))
    let (_, _, c, d) = t.[0]
    eq "a triangle repeats the third index" c d
    throwsRs "not a mesh" (fun () -> rs.MeshFaceVertices (nonMesh ()))
    )

test "MeshFaces" (fun () ->
    let m = mbox ()
    eq "quads: four points per face" 24 (rs.MeshFaces m).Count
    eq "quads, by geometry" 24 (rs.MeshFaces (meshGeo m)).Count
    eq "quads broken down into triangles" 36 (rs.MeshFaces(m, false)).Count
    eq "quads broken down, by geometry" 36 (rs.MeshFaces(meshGeo m, false)).Count
    let t = rs.AddMeshTriangle(pt 0. 0. 0., pt 1. 0. 0., pt 0. 1. 0.)
    let pts = rs.MeshFaces t
    eq "a triangle gives four points" 4 pts.Count
    nearPt "the third and fourth point are the same" pts.[2] pts.[3]
    eq "triangle with faceType=false gives three points" 3 (rs.MeshFaces(t, false)).Count
    throwsRs "not a mesh" (fun () -> rs.MeshFaces (nonMesh ()))
    )

test "MeshFacePoints" (fun () ->
    let m = mbox ()
    eq "six faces" 6 (rs.MeshFacePoints m).Count
    eq "by geometry" 6 (rs.MeshFacePoints (meshGeo m)).Count
    let (a, b, c, d) = (rs.MeshFacePoints m).[0]
    ok "the four points of a quad are different" (a <> b && b <> c && c <> d)
    throwsRs "not a mesh" (fun () -> rs.MeshFacePoints (nonMesh ()))
    )

test "MeshNgonFaceVertices" (fun () ->
    let m = mbox ()
    let r = rs.MeshNgonFaceVertices m
    ok "returns a list (empty for a mesh without ngons)" (not (isNull r))
    ok "by geometry" (not (isNull (rs.MeshNgonFaceVertices (meshGeo m))))
    // a mesh with an ngon
    let mesh = new Mesh()
    for p in [ pt 0. 0. 0.; pt 2. 0. 0.; pt 3. 1. 0.; pt 2. 2. 0.; pt 0. 2. 0. ] do mesh.Vertices.Add p |> ignore
    mesh.Faces.AddFace(0, 1, 2) |> ignore
    mesh.Faces.AddFace(0, 2, 3) |> ignore
    mesh.Faces.AddFace(0, 3, 4) |> ignore
    mesh.Ngons.AddNgon(MeshNgon.Create([| 0; 1; 2; 3; 4 |], [| 0; 1; 2 |])) |> ignore
    let n = rs.MeshNgonFaceVertices mesh
    eq "one ngon" 1 n.Count
    eq "the ngon has five vertices" 5 n.[0].Count
    throwsRs "not a mesh" (fun () -> rs.MeshNgonFaceVertices (nonMesh ()))
    )

test "MeshFaceCenters" (fun () ->
    let m = mbox ()
    let c = rs.MeshFaceCenters m
    eq "six centers" 6 c.Count
    eq "by geometry" 6 (rs.MeshFaceCenters (meshGeo m)).Count
    ok "the center of every face of the box is half way" (c |> Seq.forall (fun p -> (p.X = 0.5 || p.X = 0. || p.X = 1.) && abs (p.X - 0.5) + abs (p.Y - 0.5) + abs (p.Z - 0.5) = 0.5))
    throwsRs "not a mesh" (fun () -> rs.MeshFaceCenters (nonMesh ()))
    )

test "MeshFaceNormals" (fun () ->
    let m = mbox ()
    let n = rs.MeshFaceNormals m
    eq "six normals" 6 n.Count
    eq "by geometry" 6 (rs.MeshFaceNormals (meshGeo m)).Count
    ok "unit length" (n |> Seq.forall (fun v -> abs (v.Length - 1.) < 1e-6))
    ok "they point outwards: one per axis direction" (n |> Seq.map (fun v -> (round v.X, round v.Y, round v.Z)) |> Seq.distinct |> Seq.length = 6)
    throwsRs "not a mesh" (fun () -> rs.MeshFaceNormals (nonMesh ()))
    )

test "MeshVertexNormals" (fun () ->
    let m = mbox ()
    let n = rs.MeshVertexNormals m
    eq "eight normals" 8 n.Count
    eq "by geometry" 8 (rs.MeshVertexNormals (meshGeo m)).Count
    ok "unit length" (n |> Seq.forall (fun v -> abs (v.Length - 1.) < 1e-6))
    let noNormals = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ], faces ([ [ 0; 1; 2 ] ]))
    ok "the list may be empty if no normals exist" ((rs.MeshVertexNormals noNormals).Count = 0 || (rs.MeshVertexNormals noNormals).Count = 3)
    throwsRs "not a mesh" (fun () -> rs.MeshVertexNormals (nonMesh ()))
    )

test "MeshHasFaceNormals" (fun () ->
    let plain = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ], faces ([ [ 0; 1; 2 ] ]))
    ok "the property is readable by id and geometry" (rs.MeshHasFaceNormals plain = rs.MeshHasFaceNormals (meshGeo plain))
    ok "a mesh box has face normals" (rs.MeshHasFaceNormals (mbox ()))
    throwsRs "not a mesh" (fun () -> rs.MeshHasFaceNormals (nonMesh ()))
    )

test "MeshHasVertexNormals" (fun () ->
    ok "mesh box" (rs.MeshHasVertexNormals (mbox ()))
    ok "by geometry" (rs.MeshHasVertexNormals (meshGeo (mbox ())))
    throwsRs "not a mesh" (fun () -> rs.MeshHasVertexNormals (nonMesh ()))
    )

test "MeshHasTextureCoordinates" (fun () ->
    let plain = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ], faces ([ [ 0; 1; 2 ] ]))
    ok "none" (not (rs.MeshHasTextureCoordinates plain))
    ok "none, by geometry" (not (rs.MeshHasTextureCoordinates (meshGeo plain)))
    let tc = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ], faces ([ [ 0; 1; 2 ] ]), null, [ Point2f(0.f, 0.f); Point2f(1.f, 0.f); Point2f(1.f, 1.f) ])
    ok "some" (rs.MeshHasTextureCoordinates tc)
    throwsRs "not a mesh" (fun () -> rs.MeshHasTextureCoordinates (nonMesh ()))
    )

test "MeshTextureCoordinates" (fun () ->
    let tc = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ], faces ([ [ 0; 1; 2 ] ]), null, [ Point2f(0.f, 0.f); Point2f(1.f, 0.f); Point2f(1.f, 1.f) ])
    let r = rs.MeshTextureCoordinates tc
    eq "one per vertex" 3 r.Count
    eq "second coordinate" (Point2f(1.f, 0.f)) r.[1]
    eq "by geometry" 3 (rs.MeshTextureCoordinates (meshGeo tc)).Count
    throwsRs "not a mesh" (fun () -> rs.MeshTextureCoordinates (nonMesh ()))
    )

test "MeshHasVertexColors" (fun () ->
    let m = mbox ()
    ok "none" (not (rs.MeshHasVertexColors m))
    ok "none, by geometry" (not (rs.MeshHasVertexColors (meshGeo m)))
    rs.MeshVertexColors(m, Seq.replicate 8 Drawing.Color.Red)
    ok "some" (rs.MeshHasVertexColors m)
    throwsRs "not a mesh" (fun () -> rs.MeshHasVertexColors (nonMesh ()))
    )

test "MeshVertexColors" (fun () ->
    let m = mbox ()
    eq "no colors at first" 0 (rs.MeshVertexColors m).Count
    rs.MeshVertexColors(m, Seq.replicate 8 Drawing.Color.Red)
    let c = rs.MeshVertexColors m
    eq "eight colors" 8 c.Count
    eq "red" (255, 0, 0) (rgb c.[0])
    eq "by geometry" 8 (rs.MeshVertexColors (meshGeo m)).Count
    rs.MeshVertexColors(m, [])
    ok "an empty list removes the colors" (not (rs.MeshHasVertexColors m))
    rs.MeshVertexColors(m, Seq.replicate 8 Drawing.Color.Red)
    rs.MeshVertexColors(m, null)
    ok "null removes the colors" (not (rs.MeshHasVertexColors m))
    throwsRs "wrong number of colors" (fun () -> rs.MeshVertexColors(m, [ Drawing.Color.Red ]))
    throwsRs "not a mesh, get" (fun () -> rs.MeshVertexColors (nonMesh ()))
    )

test "MeshVertexFaces" (fun () ->
    let q = twoQuads ()
    eq "a corner vertex touches one face" [ 0 ] (rs.MeshVertexFaces(q, 0) |> Array.sort |> List.ofArray)
    eq "a shared vertex touches both faces" [ 0; 1 ] (rs.MeshVertexFaces(q, 1) |> Array.sort |> List.ofArray)
    eq "by geometry" [ 0; 1 ] (rs.MeshVertexFaces(meshGeo q, 2) |> Array.sort |> List.ofArray)
    eq "a vertex of a box touches three faces" 3 (rs.MeshVertexFaces(mbox (), 0)).Length
    throwsRs "vertex index too big" (fun () -> rs.MeshVertexFaces(q, 99))
    throwsRs "negative vertex index" (fun () -> rs.MeshVertexFaces(q, -1))
    throwsRs "not a mesh" (fun () -> rs.MeshVertexFaces(nonMesh (), 0))
    )

test "MeshNakedEdgePoints" (fun () ->
    let q = twoQuads ()
    let r = rs.MeshNakedEdgePoints q
    eq "one flag per vertex" 6 r.Length
    ok "every vertex of a flat open mesh is on the naked edge" (r |> Array.forall id)
    eq "by geometry" 6 (rs.MeshNakedEdgePoints (meshGeo q)).Length
    // a mesh box made of one piece has no naked edge points
    let b = rs.MeshNakedEdgePoints (mbox ())
    eq "one flag per vertex of the box" 8 b.Length
    throwsRs "not a mesh" (fun () -> rs.MeshNakedEdgePoints (nonMesh ()))
    )

test "IsMeshClosed" (fun () ->
    both "box is closed" (fun (i:Guid) -> rs.IsMeshClosed i) (fun (m:Mesh) -> rs.IsMeshClosed m) (mbox ()) true
    both "two quads are open" (fun (i:Guid) -> rs.IsMeshClosed i) (fun (m:Mesh) -> rs.IsMeshClosed m) (twoQuads ()) false
    ok "not a mesh gives false" (not (rs.IsMeshClosed (nonMesh ())))
    )

test "IsMeshManifold" (fun () ->
    both "box is manifold" (fun (i:Guid) -> rs.IsMeshManifold i) (fun (m:Mesh) -> rs.IsMeshManifold m) (mbox ()) true
    both "two quads are manifold" (fun (i:Guid) -> rs.IsMeshManifold i) (fun (m:Mesh) -> rs.IsMeshManifold m) (twoQuads ()) true
    // three quads sharing one edge: non manifold
    let nm = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0.; pt 0. 1. 0.; pt 1. -1. 0.; pt 0. -1. 0.; pt 1. 0. 1.; pt 0. 0. 1. ], faces ([ [ 0; 1; 2; 3 ]; [ 0; 1; 4; 5 ]; [ 0; 1; 6; 7 ] ]))
    both "three faces at one edge are not manifold" (fun (i:Guid) -> rs.IsMeshManifold i) (fun (m:Mesh) -> rs.IsMeshManifold m) nm false
    ok "not a mesh gives false" (not (rs.IsMeshManifold (nonMesh ())))
    )

test "IsPointOnMesh" (fun () ->
    let m = mbox ()
    ok "on the top" (rs.IsPointOnMesh(m, pt 0.5 0.5 1.))
    ok "on the top, by geometry" (rs.IsPointOnMesh(meshGeo m, pt 0.5 0.5 1.))
    ok "above the mesh" (not (rs.IsPointOnMesh(m, pt 0.5 0.5 2.)))
    ok "inside of the box is not on the mesh" (not (rs.IsPointOnMesh(m, pt 0.5 0.5 0.5)))
    ok "close, but outside of the default tolerance" (not (rs.IsPointOnMesh(m, pt 0.5 0.5 1.01)))
    ok "close, within a custom tolerance" (rs.IsPointOnMesh(m, pt 0.5 0.5 1.01, 0.1))
    throwsRs "not a mesh" (fun () -> rs.IsPointOnMesh(nonMesh (), pt 0. 0. 0.))
    )

// ------------------------------- measuring -------------------------------

test "MeshArea" (fun () ->
    let m = mbox ()
    near "box by id" 6. (rs.MeshArea m)
    near "box by geometry" 6. (rs.MeshArea (meshGeo m))
    let m2 = boxAt 5.
    near "several by id: total area" 12. (rs.MeshArea [ m; m2 ])
    near "several by geometry: total area" 12. (rs.MeshArea [ meshGeo m; meshGeo m2 ])
    near "an empty collection has the area 0" 0. (rs.MeshArea ([] : Guid list))
    near "an empty collection of meshes has the area 0" 0. (rs.MeshArea ([] : Mesh list))
    throwsRs "not a mesh" (fun () -> rs.MeshArea (nonMesh ()))
    )

test "MeshAreaCentroid" (fun () ->
    let m = mbox ()
    nearPt "centroid of the box" (pt 0.5 0.5 0.5) (rs.MeshAreaCentroid m)
    nearPt "by geometry" (pt 0.5 0.5 0.5) (rs.MeshAreaCentroid (meshGeo m))
    throwsRs "not a mesh" (fun () -> rs.MeshAreaCentroid (nonMesh ()))
    )

test "MeshVolume" (fun () ->
    let m = mbox ()
    near "box" 1. (rs.MeshVolume [ m ])
    near "box by geometry" 1. (rs.MeshVolume [ meshGeo m ])
    near "two boxes" 2. (rs.MeshVolume [ m; boxAt 5. ])
    near "open meshes have no volume" 0. (try rs.MeshVolume [ twoQuads () ] with _ -> 0.)
    throwsRs "not a mesh" (fun () -> rs.MeshVolume [ nonMesh () ])
    )

test "MeshVolumeCentroid" (fun () ->
    let m = mbox ()
    nearPt "centroid of the box" (pt 0.5 0.5 0.5) (rs.MeshVolumeCentroid m)
    nearPt "by geometry" (pt 0.5 0.5 0.5) (rs.MeshVolumeCentroid (meshGeo m))
    throwsRs "not a mesh" (fun () -> rs.MeshVolumeCentroid (nonMesh ()))
    )

test "MeshClosestPoint" (fun () ->
    let m = mbox ()
    let p, face = rs.MeshClosestPoint(m, pt 0.5 0.5 3.)
    nearPt "closest point on the top" (pt 0.5 0.5 1.) p
    ok "face index is valid" (face >= 0 && face < 6)
    let p2, _ = rs.MeshClosestPoint(meshGeo m, pt 5. 0.5 0.5)
    nearPt "by geometry" (pt 1. 0.5 0.5) p2
    let p3, _ = rs.MeshClosestPoint(m, pt 0.5 0.5 3., 10.)
    nearPt "with a maximum distance that is big enough" (pt 0.5 0.5 1.) p3
    throwsRs "not a mesh" (fun () -> rs.MeshClosestPoint(nonMesh (), pt 0. 0. 0.))
    )

// ------------------------------- intersections and outlines -------------------------------

test "CurveMeshIntersection" (fun () ->
    let m = mbox ()
    let c = rs.AddLine(pt 0.5 0.5 -1., pt 0.5 0.5 2.)
    let pts, faces = rs.CurveMeshIntersection(c, m)
    eq "two points" 2 pts.Length
    eq "one face index per point" 2 faces.Length
    ok "the points are on the top and on the bottom" (let zs = pts |> Array.map (fun p -> p.Z) |> Array.sort in abs zs.[0] < 1e-6 && abs (zs.[1] - 1.) < 1e-6)
    let pts2, _ = rs.CurveMeshIntersection(geo c :?> Curve, meshGeo m)
    eq "by geometry" 2 pts2.Length
    let miss = rs.AddLine(pt 5. 5. -1., pt 5. 5. 2.)
    let pts3, _ = (try rs.CurveMeshIntersection(miss, m) with :? RhinoScriptingException -> [||], [||])
    eq "a curve that misses the mesh has no intersection" 0 pts3.Length
    throwsRs "not a mesh" (fun () -> rs.CurveMeshIntersection(c, nonMesh ()))
    )

test "MeshMeshIntersection" (fun () ->
    let a = mbox ()
    let b = boxAt 0.5
    let r = rs.MeshMeshIntersection(a, b)
    ok "intersection polylines" (r.Length > 0)
    ok "they have points" (r |> Array.forall (fun pl -> pl.Count > 1))
    let r2 = rs.MeshMeshIntersection(meshGeo a, meshGeo b)
    ok "by geometry" (r2.Length > 0)
    let r3 = rs.MeshMeshIntersection(a, b, 0.001)
    ok "with a tolerance" (r3.Length > 0)
    let far = boxAt 10.
    let none = rs.MeshMeshIntersection(a, far)
    ok "meshes that do not touch give an empty array, not null" (not (isNull none) && none.Length = 0)
    throwsRs "not a mesh" (fun () -> rs.MeshMeshIntersection(a, nonMesh ()))
    )

test "MeshOutline" (fun () ->
    let m = mbox ()
    let o = rs.MeshOutline [ m ]
    ok "outline curves" (o.Count >= 1)
    ok "they are curves" (o |> Seq.forall rs.IsCurve)
    let o2 = rs.MeshOutline([ m ], "Front")
    ok "in a view" (o2.Count >= 1)
    let o3 = rs.MeshOutline [ meshGeo m ]
    ok "by geometry" (o3.Count >= 1)
    let o4 = rs.MeshOutline([ meshGeo m ], (rs.CoerceView "Top").MainViewport)
    ok "by geometry in a viewport" (o4.Count >= 1)
    throws "unknown view" (fun () -> rs.MeshOutline([ m ], "no such view"))
    throwsRs "not a mesh" (fun () -> rs.MeshOutline [ nonMesh () ])
    )

test "PullCurveToMesh" (fun () ->
    let m = mbox ()
    let c = rs.AddLine(pt 0.2 0.2 3., pt 0.8 0.8 3.)
    let pulled = rs.PullCurveToMesh(m, c)
    ok "a new curve" (pulled <> c && rs.IsCurve pulled)
    near "it lies on the top of the box" 1. (rs.CurveStartPoint pulled).Z
    let g = rs.PullCurveToMesh(meshGeo m, geo c :?> Curve)
    near "by geometry" 1. g.PointAtStart.Z
    throwsRs "not a mesh" (fun () -> rs.PullCurveToMesh(nonMesh (), c))
    throwsRs "not a curve" (fun () -> rs.PullCurveToMesh(m, nonMesh ()))
    )

// ------------------------------- boolean operations -------------------------------

test "MeshBooleanUnion" (fun () ->
    let a = fine 0.
    let b = fineOff ()
    let r = rs.MeshBooleanUnion [ a; b ]
    eq "one resulting mesh" 1 r.Count
    nearTol "volume of the union" 0.02 1.755 (rs.MeshVolume [ r.[0] ])
    ok "the inputs are deleted by default" (not (rs.IsObject a) && not (rs.IsObject b))
    let c = fine 0.
    let d = fineOff ()
    rs.MeshBooleanUnion([ c; d ], false) |> ignore
    ok "the inputs are kept on request" (rs.IsObject c && rs.IsObject d)
    let g = rs.MeshBooleanUnion [ meshGeo c; meshGeo d ]
    nearTol "by geometry" 0.02 1.755 (rs.MeshVolume [ g.[0] ])
    throwsRs "not a mesh" (fun () -> rs.MeshBooleanUnion [ nonMesh (); nonMesh () ])
    )

test "MeshBooleanDifference" (fun () ->
    let a = fine 0.
    let b = fineOff ()
    let r = rs.MeshBooleanDifference([ a ], [ b ])
    eq "one resulting mesh" 1 r.Count
    nearTol "volume of the difference" 0.02 0.755 (rs.MeshVolume [ r.[0] ])
    ok "the inputs are deleted by default" (not (rs.IsObject a) && not (rs.IsObject b))
    let c = fine 0.
    let d = fineOff ()
    rs.MeshBooleanDifference([ c ], [ d ], false) |> ignore
    ok "the inputs are kept on request" (rs.IsObject c && rs.IsObject d)
    let g = rs.MeshBooleanDifference([ meshGeo c ], [ meshGeo d ])
    nearTol "by geometry" 0.02 0.755 (rs.MeshVolume [ g.[0] ])
    throwsRs "not a mesh" (fun () -> rs.MeshBooleanDifference([ nonMesh () ], [ nonMesh () ]))
    )

test "MeshBooleanIntersection" (fun () ->
    let a = fine 0.
    let b = fineOff ()
    let r = rs.MeshBooleanIntersection([ a ], [ b ])
    eq "one resulting mesh" 1 r.Count
    nearTol "volume of the intersection" 0.02 0.245 (rs.MeshVolume [ r.[0] ])
    ok "the inputs are deleted by default" (not (rs.IsObject a) && not (rs.IsObject b))
    let c = fine 0.
    let d = fineOff ()
    rs.MeshBooleanIntersection([ c ], [ d ], false) |> ignore
    ok "the inputs are kept on request" (rs.IsObject c && rs.IsObject d)
    let g = rs.MeshBooleanIntersection([ meshGeo c ], [ meshGeo d ])
    nearTol "by geometry" 0.02 0.245 (rs.MeshVolume [ g.[0] ])
    throwsRs "not a mesh" (fun () -> rs.MeshBooleanIntersection([ nonMesh () ], [ nonMesh () ]))
    )

test "MeshBooleanSplit" (fun () ->
    let a = fine 0.
    let b = fineOff ()
    let r = rs.MeshBooleanSplit([ a ], [ b ])
    ok "at least two resulting meshes" (r.Count >= 2)
    nearTol "the volumes add up to the volume of the first mesh" 0.05 1. (rs.MeshVolume r)
    ok "the inputs are deleted by default" (not (rs.IsObject a) && not (rs.IsObject b))
    let c = fine 0.
    let d = fineOff ()
    rs.MeshBooleanSplit([ c ], [ d ], false) |> ignore
    ok "the inputs are kept on request" (rs.IsObject c && rs.IsObject d)
    let g = rs.MeshBooleanSplit([ meshGeo c ], [ meshGeo d ])
    ok "by geometry" (g.Count >= 2)
    throwsRs "not a mesh" (fun () -> rs.MeshBooleanSplit([ nonMesh () ], [ nonMesh () ]))
    )

// ------------------------------- editing -------------------------------

test "JoinMeshes" (fun () ->
    let a = mbox ()
    let b = boxAt 5.
    let j = rs.JoinMeshes [ a; b ]
    ok "a new mesh" (rs.IsMesh j)
    eq "all vertices" 16 (rs.MeshVertexCount j)
    ok "the inputs are kept by default" (rs.IsObject a && rs.IsObject b)
    let j2 = rs.JoinMeshes([ a; b ], true)
    ok "the inputs are deleted on request" (not (rs.IsObject a) && not (rs.IsObject b))
    eq "joined again" 16 (rs.MeshVertexCount j2)
    let g = rs.JoinMeshes [ meshGeo j; meshGeo j2 ]
    eq "by geometry" 32 g.Vertices.Count
    throwsRs "not a mesh" (fun () -> rs.JoinMeshes [ nonMesh (); nonMesh () ])
    )

test "DisjointMeshCount" (fun () ->
    let j = rs.JoinMeshes [ mbox (); boxAt 5. ]
    both "two pieces" (fun (i:Guid) -> rs.DisjointMeshCount i) (fun (m:Mesh) -> rs.DisjointMeshCount m) j 2
    both "one piece" (fun (i:Guid) -> rs.DisjointMeshCount i) (fun (m:Mesh) -> rs.DisjointMeshCount m) (mbox ()) 1
    throwsRs "not a mesh" (fun () -> rs.DisjointMeshCount (nonMesh ()))
    )

test "SplitDisjointMesh" (fun () ->
    let j = rs.JoinMeshes [ mbox (); boxAt 5. ]
    let parts = rs.SplitDisjointMesh j
    eq "two pieces" 2 parts.Count
    ok "the input is kept by default" (rs.IsObject j)
    let parts2 = rs.SplitDisjointMesh(j, true)
    eq "two pieces again" 2 parts2.Count
    ok "the input is deleted on request" (not (rs.IsObject j))
    let g = rs.SplitDisjointMesh (rs.JoinMeshes [ meshGeo parts.[0]; meshGeo parts.[1] ])
    eq "by geometry" 2 g.Count
    throwsRs "not a mesh" (fun () -> rs.SplitDisjointMesh (nonMesh ()))
    )

test "ExplodeMeshes" (fun () ->
    let j = rs.JoinMeshes [ mbox (); boxAt 5. ]
    let parts = rs.ExplodeMeshes [ j ]
    ok "pieces" (parts.Count >= 2)
    ok "the input is kept by default" (rs.IsObject j)
    let parts2 = rs.ExplodeMeshes([ j ], true)
    ok "pieces again" (parts2.Count >= 2)
    ok "the input is deleted on request" (not (rs.IsObject j))
    let g = rs.ExplodeMeshes [ meshGeo parts.[0] ]
    ok "by geometry" (g.Count >= 1)
    throwsRs "not a mesh" (fun () -> rs.ExplodeMeshes [ nonMesh () ])
    )

test "DuplicateMeshBorder" (fun () ->
    let q = twoQuads ()
    let b = rs.DuplicateMeshBorder q
    eq "one closed border curve" 1 b.Count
    ok "it is a curve" (rs.IsCurve b.[0])
    ok "it is closed" (rs.IsCurveClosed b.[0])
    near "length of the border" 6. (rs.CurveLength b.[0])
    let g = rs.DuplicateMeshBorder (meshGeo q)
    eq "by geometry" 1 g.Count
    eq "a closed mesh has no border" 0 (rs.DuplicateMeshBorder (mbox ())).Count
    throwsRs "not a mesh" (fun () -> rs.DuplicateMeshBorder (nonMesh ()))
    )

test "MeshOffset" (fun () ->
    let m = mbox ()
    let o = rs.MeshOffset(m, 0.1)
    ok "a new mesh" (o <> m && rs.IsMesh o)
    let bb = rs.BoundingBox o
    ok "the size changed" (abs ((bb.Max.X - bb.Min.X) - 1.) > 0.05)
    let g = rs.MeshOffset(meshGeo m, 0.1)
    ok "by geometry" (abs ((g.GetBoundingBox(true).Max.X - g.GetBoundingBox(true).Min.X) - 1.) > 0.05)
    throwsRs "not a mesh" (fun () -> rs.MeshOffset(nonMesh (), 0.1))
    )

test "MeshQuadsToTriangles" (fun () ->
    let m = mbox ()
    ok "succeeds" (rs.MeshQuadsToTriangles m)
    eq "no quads left" 0 (rs.MeshQuadCount m)
    eq "twelve triangles" 12 (rs.MeshTriangleCount m)
    ok "converting again does not fail" (rs.MeshQuadsToTriangles m || true)
    throwsRs "not a mesh" (fun () -> rs.MeshQuadsToTriangles (nonMesh ()))
    )

test "UnifyMeshNormals" (fun () ->
    let flipped = rs.AddMesh([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0.; pt 0. 1. 0.; pt 2. 0. 0.; pt 2. 1. 0. ], faces ([ [ 0; 1; 2; 3 ]; [ 2; 5; 4; 1 ] ]))
    eq "one face had to be flipped" 1 (rs.UnifyMeshNormals flipped)
    let zs = rs.MeshFaceNormals flipped |> Seq.map (fun v -> sign v.Z) |> Seq.distinct |> Seq.length
    eq "all face normals point in the same direction afterwards" 1 zs
    eq "nothing to do the second time" 0 (rs.UnifyMeshNormals flipped)
    eq "consistent mesh" 0 (rs.UnifyMeshNormals (twoQuads ()))
    throwsRs "not a mesh" (fun () -> rs.UnifyMeshNormals (nonMesh ()))
    )

test "MeshToNurb" (fun () ->
    let m = mbox ()
    let b = rs.MeshToNurb m
    eq "one brep for the connected piece" 1 b.Count
    ok "it is a brep" (rs.IsBrep b.[0])
    ok "the input is kept by default" (rs.IsObject m)
    let tri = rs.AddMeshTriangle(pt 0. 0. 0., pt 1. 0. 0., pt 0. 1. 0.)
    ok "trimmed triangles" ((rs.MeshToNurb(tri, true)).Count = 1)
    ok "triangles as degenerate surfaces" ((rs.MeshToNurb(tri, false)).Count = 1)
    let d = rs.MeshToNurb(m, true, true)
    ok "the input is deleted on request" (not (rs.IsObject m))
    let g = rs.MeshToNurb (meshGeo (mbox ()))
    eq "by geometry" 1 g.Count
    ok "by geometry without trimmed triangles" ((rs.MeshToNurb(meshGeo tri, false)).Count = 1)
    throwsRs "not a mesh" (fun () -> rs.MeshToNurb (nonMesh ()))
    )

printfn "%s" (finish ())
