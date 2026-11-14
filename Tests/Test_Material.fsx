// Tests for Src/Scripting_Material.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Material"

let red = Drawing.Color.FromArgb(255, 0, 0)
let blue = Drawing.Color.FromArgb(0, 0, 255)
let rgb (c:Drawing.Color) = (int c.R, int c.G, int c.B)
/// adds a new material to the document and returns its index
let newMaterial () = doc().Materials.Add()
let bad = 100000

/// A file that exists, to be used as texture file name
let textureFile = IO.Path.Combine(IO.Path.GetTempPath(), "RhinoScriptingTestTexture.png")
IO.File.WriteAllBytes(textureFile, [| 0uy |])

/// Tests a getter/setter pair of a texture slot
let textureTest (getter: int -> string) (setter: int * string -> unit) =
    let m = newMaterial ()
    eq "no texture gives an empty string" "" (getter m)
    setter (m, textureFile)
    eq "file name round trip" (textureFile.ToLowerInvariant()) ((getter m).ToLowerInvariant())
    throwsRs "file does not exist" (fun () -> setter (m, @"C:\this\file\does\not\exist.png"))
    throwsRs "get with an invalid index" (fun () -> getter bad)
    throwsRs "get with a negative index" (fun () -> getter (-1))
    throwsRs "set with an invalid index" (fun () -> setter (bad, textureFile))

test "AddMaterialToObject" (fun () ->
    let p = rs.AddPoint(0., 0., 0.)
    let i = rs.AddMaterialToObject p
    ok "valid index" (i >= 0)
    eq "calling again returns the same index" i (rs.AddMaterialToObject p)
    eq "the object uses the material" i (doc().Objects.FindId(p).Attributes.MaterialIndex)
    eq "the material source is the object" DocObjects.ObjectMaterialSource.MaterialFromObject (doc().Objects.FindId(p).Attributes.MaterialSource)
    let q = rs.AddPoint(1., 0., 0.)
    ok "another object gets another material" (rs.AddMaterialToObject q <> i)
    throws "unknown object" (fun () -> rs.AddMaterialToObject (Guid.NewGuid()))
    )

test "AddMaterialToLayer" (fun () ->
    rs.AddLayer("matLayer") |> ignore
    let layer = "matLayer"
    let i = rs.AddMaterialToLayer layer
    ok "valid index" (i >= 0)
    eq "calling again returns the same index" i (rs.AddMaterialToLayer layer)
    throws "unknown layer" (fun () -> rs.AddMaterialToLayer "no such layer")
    )

test "CopyMaterial" (fun () ->
    let a = newMaterial ()
    let b = newMaterial ()
    rs.MaterialColor(a, red)
    ok "copy succeeds" (rs.CopyMaterial(a, b))
    eq "color was copied" (rgb red) (rgb (rs.MaterialColor b))
    ok "the same index succeeds without a change" (rs.CopyMaterial(a, a))
    eq "source is unchanged" (rgb red) (rgb (rs.MaterialColor a))
    ok "invalid source index" (not (rs.CopyMaterial(bad, b)))
    ok "invalid destination index" (not (rs.CopyMaterial(a, bad)))
    ok "negative source index" (not (rs.CopyMaterial(-1, b)))
    ok "negative destination index" (not (rs.CopyMaterial(a, -1)))
    ok "invalid index is false even when equal" (not (rs.CopyMaterial(bad, bad)))
    )

test "IsMaterialDefault" (fun () ->
    let m = newMaterial ()
    rs.MaterialColor(m, red)
    ok "a changed material is not the default" (not (rs.IsMaterialDefault m))
    // In Rhino 8 Material.IsDefaultMaterial is false even for Material.DefaultMaterial, so a positive result can not be tested
    ok "reset succeeds" (rs.ResetMaterial m)
    ok "invalid index" (not (rs.IsMaterialDefault bad))
    ok "negative index" (not (rs.IsMaterialDefault (-1)))
    )

test "IsMaterialReference" (fun () ->
    let m = newMaterial ()
    ok "a material of the document is no reference" (not (rs.IsMaterialReference m))
    ok "invalid index" (not (rs.IsMaterialReference bad))
    ok "negative index" (not (rs.IsMaterialReference (-1)))
    )

test "MatchMaterial" (fun () ->
    let a = rs.AddPoint(0., 0., 0.)
    let b = rs.AddPoint(1., 0., 0.)
    let c = rs.AddPoint(2., 0., 0.)
    let m = rs.AddMaterialToObject a
    rs.MaterialColor(m, blue)
    // from an object
    let src = doc().Objects.FindId(a).Attributes.MaterialIndex
    eq "a still uses the material that was added to it" m src
    rs.MatchMaterial(a, [ b; c ])
    // Rhino 8 may create a copy of the material for each destination object ("copies the material definition"), so compare the color not the index
    let matOf (id:Guid) = doc().Objects.FindId(id).Attributes.MaterialIndex
    eq "b has the material color of a" (rgb blue) (rgb (rs.MaterialColor (matOf b)))
    eq "c has the material color of a" (rgb blue) (rgb (rs.MaterialColor (matOf c)))
    eq "the material source is the object" DocObjects.ObjectMaterialSource.MaterialFromObject (doc().Objects.FindId(b).Attributes.MaterialSource)
    // from an index
    let d = rs.AddPoint(3., 0., 0.)
    let m2 = newMaterial ()
    rs.MaterialColor(m2, red)
    rs.MatchMaterial(m2, [ d ])
    eq "d has the material color of the index" (rgb red) (rgb (rs.MaterialColor (matOf d)))
    rs.MatchMaterial(m2, [ Guid.NewGuid() ]) // missing destinations are skipped
    throwsRs "invalid material index" (fun () -> rs.MatchMaterial(bad, [ d ]))
    throwsRs "negative material index" (fun () -> rs.MatchMaterial(-1, [ d ]))
    throws "source object does not exist" (fun () -> rs.MatchMaterial(Guid.NewGuid(), [ d ]))
    let noMat = rs.AddPoint(9., 9., 9.)
    throwsRs "source object without a material" (fun () -> rs.MatchMaterial(noMat, [ d ]))
    )

test "MaterialBump" (fun () -> textureTest (fun i -> rs.MaterialBump(i)) (fun (i, f) -> rs.MaterialBump(i, f)))
test "MaterialEnvironmentMap" (fun () -> textureTest (fun i -> rs.MaterialEnvironmentMap(i)) (fun (i, f) -> rs.MaterialEnvironmentMap(i, f)))
test "MaterialTexture" (fun () -> textureTest (fun i -> rs.MaterialTexture(i)) (fun (i, f) -> rs.MaterialTexture(i, f)))
test "MaterialTransparencyMap" (fun () -> textureTest (fun i -> rs.MaterialTransparencyMap(i)) (fun (i, f) -> rs.MaterialTransparencyMap(i, f)))

test "MaterialColor" (fun () ->
    let m = newMaterial ()
    rs.MaterialColor(m, red)
    eq "diffuse color round trip" (rgb red) (rgb (rs.MaterialColor m))
    rs.MaterialColor(m, blue)
    eq "set again" (rgb blue) (rgb (rs.MaterialColor m))
    throwsRs "get with an invalid index" (fun () -> rs.MaterialColor bad)
    throwsRs "get with a negative index" (fun () -> rs.MaterialColor (-1))
    throwsRs "set with an invalid index" (fun () -> rs.MaterialColor(bad, red))
    )

test "MaterialReflectiveColor" (fun () ->
    let m = newMaterial ()
    rs.MaterialReflectiveColor(m, red)
    eq "reflective color round trip" (rgb red) (rgb (rs.MaterialReflectiveColor m))
    throwsRs "get with an invalid index" (fun () -> rs.MaterialReflectiveColor bad)
    throwsRs "set with an invalid index" (fun () -> rs.MaterialReflectiveColor(bad, red))
    )

test "MaterialName" (fun () ->
    let m = newMaterial ()
    rs.MaterialName(m, "my material")
    eq "name round trip" "my material" (rs.MaterialName m)
    throwsRs "get with an invalid index" (fun () -> rs.MaterialName bad)
    throwsRs "set with an invalid index" (fun () -> rs.MaterialName(bad, "x"))
    )

test "MaterialShine" (fun () ->
    let m = newMaterial ()
    rs.MaterialShine(m, 128.)
    nearTol "shine round trip" 0.5 128. (rs.MaterialShine m)
    rs.MaterialShine(m, 0.)
    nearTol "matte" 0.5 0. (rs.MaterialShine m)
    rs.MaterialShine(m, 255.)
    nearTol "glossy" 0.5 255. (rs.MaterialShine m)
    throwsRs "get with an invalid index" (fun () -> rs.MaterialShine bad)
    throwsRs "set with an invalid index" (fun () -> rs.MaterialShine(bad, 1.))
    )

test "MaterialTransparency" (fun () ->
    let m = newMaterial ()
    rs.MaterialTransparency(m, 0.5)
    nearTol "transparency round trip" 0.001 0.5 (rs.MaterialTransparency m)
    rs.MaterialTransparency(m, 0.)
    nearTol "opaque" 0.001 0. (rs.MaterialTransparency m)
    rs.MaterialTransparency(m, 1.)
    nearTol "transparent" 0.001 1. (rs.MaterialTransparency m)
    throwsRs "get with an invalid index" (fun () -> rs.MaterialTransparency bad)
    throwsRs "set with an invalid index" (fun () -> rs.MaterialTransparency(bad, 0.5))
    )

test "ResetMaterial" (fun () ->
    let m = newMaterial ()
    rs.MaterialColor(m, red)
    rs.MaterialName(m, "to be reset")
    ok "reset succeeds" (rs.ResetMaterial m)
    ok "the color is back to the default" (rgb (rs.MaterialColor m) <> rgb red)
    ok "invalid index" (not (rs.ResetMaterial bad))
    ok "negative index" (not (rs.ResetMaterial (-1)))
    )

try IO.File.Delete textureFile with _ -> ()

printfn "%s" (finish ())
