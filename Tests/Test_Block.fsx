// Tests for Src/Scripting_Block.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Block"

let vec x y z = Vector3d(x, y, z)
/// Makes a block called name from a line and a point and returns the name
let makeBlock (name:string) =
    let l = rs.AddLine(pt 1. 0. 0., pt 2. 0. 0.)
    let p = rs.AddPoint(0., 1., 0.)
    rs.AddBlock([ l; p ], pt 0. 0. 0., name, true)

test "AddBlock" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.)
    let b = rs.AddPoint(5., 5., 5.)
    let name = rs.AddBlock([ a; b ], pt 0. 0. 0., "abNamed")
    eq "returns the given name" "abNamed" name
    ok "block exists" (rs.IsBlock "abNamed")
    eq "two objects in the block" 2 (rs.BlockObjectCount "abNamed")
    ok "input objects are kept by default" (not (isNull (doc().Objects.FindId a)) && not (isNull (doc().Objects.FindId b)))
    // generated name
    let auto = rs.AddBlock([ a ], pt 0. 0. 0.)
    ok "a name is generated if omitted" (not (String.IsNullOrEmpty auto) && rs.IsBlock auto)
    // delete input
    let c = rs.AddPoint(1., 1., 1.)
    let n3 = rs.AddBlock([ c ], pt 0. 0. 0., "abDelete", true)
    ok "input is deleted with deleteInput=true" (isNull (doc().Objects.FindId c))
    eq "the block has the object" 1 (rs.BlockObjectCount n3)
    // existing name: the definition is modified
    let d = rs.AddPoint(2., 2., 2.)
    let e = rs.AddPoint(3., 3., 3.)
    rs.AddBlock([ d; e ], pt 0. 0. 0., "abNamed") |> ignore
    eq "existing block definition is modified, not duplicated" 2 (rs.BlockObjectCount "abNamed")
    // base point: the instance inserted at the origin is moved relative to the base point
    let f = rs.AddPoint(10., 0., 0.)
    rs.AddBlock([ f ], pt 10. 0. 0., "abBase") |> ignore
    let inst = rs.InsertBlock("abBase", pt 0. 0. 0.)
    let obj = rs.ExplodeBlockInstance inst
    nearPt "base point maps to the insertion point" (pt 0. 0. 0.) (rs.PointCoordinates obj.[0])
    // errors
    throwsRs "a light can not be in a block" (fun () -> rs.AddBlock([ rs.AddPointLight(pt 0. 0. 5.) ], pt 0. 0. 0., "abLight"))
    throws "unknown object" (fun () -> rs.AddBlock([ Guid.NewGuid() ], pt 0. 0. 0., "abUnknown"))
    // an instance of the block can not be added to itself
    let self = rs.InsertBlock("abNamed", pt 0. 0. 0.)
    throwsRs "an instance of the block can not be added to the block itself" (fun () -> rs.AddBlock([ self ], pt 0. 0. 0., "abNamed"))
    )

test "IsBlock" (fun () ->
    makeBlock "ibA" |> ignore
    ok "existing" (rs.IsBlock "ibA")
    ok "unknown" (not (rs.IsBlock "ibUnknown"))
    )

test "BlockCount" (fun () ->
    let n = rs.BlockCount()
    makeBlock "bcA" |> ignore
    makeBlock "bcB" |> ignore
    eq "two more blocks" (n + 2) (rs.BlockCount())
    rs.DeleteBlock "bcA" |> ignore
    eq "deleted blocks are not counted" (n + 1) (rs.BlockCount())
    )

test "BlockNames" (fun () ->
    makeBlock "bnB" |> ignore
    makeBlock "bnA" |> ignore
    let names = rs.BlockNames()
    ok "contains both" (names.Contains "bnA" && names.Contains "bnB")
    eq "same count as BlockCount" (rs.BlockCount()) names.Count
    let sorted = rs.BlockNames(true)
    let expected = names |> Seq.sortWith (fun a b -> String.CompareOrdinal(a, b)) |> List.ofSeq
    eq "sorted in ordinal order" expected (List.ofSeq sorted)
    rs.DeleteBlock "bnA" |> ignore
    ok "deleted blocks are not listed" (not ((rs.BlockNames()).Contains "bnA"))
    )

test "BlockDescription" (fun () ->
    makeBlock "bdA" |> ignore
    eq "empty description at first" "" (rs.BlockDescription "bdA")
    rs.BlockDescription("bdA", "my description")
    eq "description round trip" "my description" (rs.BlockDescription "bdA")
    throwsRs "get on an unknown block" (fun () -> rs.BlockDescription "bdUnknown")
    throwsRs "set on an unknown block" (fun () -> rs.BlockDescription("bdUnknown", "x"))
    )

test "BlockObjectCount" (fun () ->
    makeBlock "boA" |> ignore
    eq "line and point" 2 (rs.BlockObjectCount "boA")
    throwsRs "unknown block" (fun () -> rs.BlockObjectCount "boUnknown")
    )

test "BlockObjects" (fun () ->
    makeBlock "bjA" |> ignore
    let ids = rs.BlockObjects "bjA"
    eq "two ids" 2 ids.Count
    ok "ids are not the ids of document objects, they belong to the definition" (ids |> Seq.forall (fun i -> i <> Guid.Empty))
    throwsRs "unknown block" (fun () -> rs.BlockObjects "bjUnknown")
    )

test "InsertBlock" (fun () ->
    makeBlock "ibkA" |> ignore
    let id = rs.InsertBlock("ibkA", pt 5. 6. 7.)
    ok "returns a block instance" (rs.IsBlockInstance id)
    nearPt "inserted at the point" (pt 5. 6. 7.) (rs.BlockInstanceInsertPoint id)
    let scaled = rs.InsertBlock("ibkA", pt 0. 0. 0., vec 2. 3. 4.)
    let x = rs.BlockInstanceXform scaled
    near "x scale" 2. x.M00
    near "y scale" 3. x.M11
    near "z scale" 4. x.M22
    let rotated = rs.InsertBlock("ibkA", pt 0. 0. 0., vec 1. 1. 1., 90.)
    nearPt "rotated 90 degrees about z: block x axis points to world y" (pt 0. 1. 0.) ((rs.BlockInstanceXform rotated) * pt 1. 0. 0.)
    let rotatedX = rs.InsertBlock("ibkA", pt 0. 0. 0., vec 1. 1. 1., 90., vec 1. 0. 0.)
    nearPt "rotated 90 degrees about x: block y axis points to world z" (pt 0. 0. 1.) ((rs.BlockInstanceXform rotatedX) * pt 0. 1. 0.)
    throwsRs "unknown block" (fun () -> rs.InsertBlock("ibkUnknown", pt 0. 0. 0.))
    )

test "InsertBlock2" (fun () ->
    makeBlock "ib2A" |> ignore
    let xf = Transform.Translation(1., 2., 3.)
    let id = rs.InsertBlock2("ib2A", xf)
    ok "returns a block instance" (rs.IsBlockInstance id)
    ok "the transform is the instance transform" ((rs.BlockInstanceXform id) = xf)
    throwsRs "unknown block" (fun () -> rs.InsertBlock2("ib2Unknown", xf))
    )

test "IsBlockInstance" (fun () ->
    makeBlock "iiA" |> ignore
    ok "block instance" (rs.IsBlockInstance (rs.InsertBlock("iiA", pt 0. 0. 0.)))
    ok "line is not a block instance" (not (rs.IsBlockInstance (rs.AddLine(pt 0. 0. 0., pt 1. 0. 0.))))
    ok "unknown id" (not (rs.IsBlockInstance (Guid.NewGuid())))
    )

test "BlockInstanceName" (fun () ->
    makeBlock "inA" |> ignore
    eq "name" "inA" (rs.BlockInstanceName (rs.InsertBlock("inA", pt 0. 0. 0.)))
    throws "not a block instance" (fun () -> rs.BlockInstanceName (rs.AddPoint(0., 0., 0.)))
    )

test "BlockInstanceInsertPoint" (fun () ->
    makeBlock "ipA" |> ignore
    nearPt "insertion point" (pt 3. 4. 5.) (rs.BlockInstanceInsertPoint (rs.InsertBlock("ipA", pt 3. 4. 5.)))
    throws "not a block instance" (fun () -> rs.BlockInstanceInsertPoint (rs.AddPoint(0., 0., 0.)))
    )

test "BlockInstanceXform" (fun () ->
    makeBlock "ixA" |> ignore
    let id = rs.InsertBlock("ixA", pt 3. 4. 5.)
    nearPt "transform contains the translation" (pt 3. 4. 5.) ((rs.BlockInstanceXform id) * Point3d.Origin)
    throws "not a block instance" (fun () -> rs.BlockInstanceXform (rs.AddPoint(0., 0., 0.)))
    )

test "BlockInstances" (fun () ->
    makeBlock "bsA" |> ignore
    let a = rs.InsertBlock("bsA", pt 0. 0. 0.)
    let b = rs.InsertBlock("bsA", pt 1. 0. 0.)
    let ids = rs.BlockInstances "bsA"
    eq "both instances" 2 ids.Count
    ok "ids are the inserted ones" (ids.Contains a && ids.Contains b)
    // nested
    let outer = rs.AddBlock([ a ], pt 0. 0. 0., "bsOuter")
    ok "nested instance is found with whereToLook=1" ((rs.BlockInstances("bsA", 1)).Count >= 2)
    throwsRs "unknown block" (fun () -> rs.BlockInstances "bsUnknown")
    )

test "BlockInstanceCount" (fun () ->
    makeBlock "bicA" |> ignore
    eq "no instances at first" 0 (rs.BlockInstanceCount "bicA")
    rs.InsertBlock("bicA", pt 0. 0. 0.) |> ignore
    rs.InsertBlock("bicA", pt 1. 0. 0.) |> ignore
    eq "two instances" 2 (rs.BlockInstanceCount "bicA")
    eq "two instances with whereToLook=1" 2 (rs.BlockInstanceCount("bicA", 1))
    throwsRs "unknown block" (fun () -> rs.BlockInstanceCount "bicUnknown")
    )

test "BlockContainers" (fun () ->
    makeBlock "bkA" |> ignore
    let inst = rs.InsertBlock("bkA", pt 0. 0. 0.)
    ok "not contained anywhere yet" ((rs.BlockContainers "bkA").Count = 0)
    rs.AddBlock([ inst ], pt 0. 0. 0., "bkOuter") |> ignore
    let c = rs.BlockContainers "bkA"
    eq "one container" 1 c.Count
    eq "the container is the outer block" "bkOuter" c.[0]
    throwsRs "unknown block" (fun () -> rs.BlockContainers "bkUnknown")
    )

test "BlockContainerCount" (fun () ->
    makeBlock "bccA" |> ignore
    eq "not contained" 0 (rs.BlockContainerCount "bccA")
    let inst = rs.InsertBlock("bccA", pt 0. 0. 0.)
    rs.AddBlock([ inst ], pt 0. 0. 0., "bccOuter") |> ignore
    eq "contained in one block" 1 (rs.BlockContainerCount "bccA")
    throwsRs "unknown block" (fun () -> rs.BlockContainerCount "bccUnknown")
    )

test "IsBlockInUse" (fun () ->
    makeBlock "iuA" |> ignore
    ok "not in use at first" (not (rs.IsBlockInUse "iuA"))
    let inst = rs.InsertBlock("iuA", pt 0. 0. 0.)
    ok "in use after an insert" (rs.IsBlockInUse "iuA")
    ok "in use with whereToLook=1" (rs.IsBlockInUse("iuA", 1))
    rs.AddBlock([ inst ], pt 0. 0. 0., "iuOuter", true) |> ignore
    ok "in use by another block definition (2)" (rs.IsBlockInUse("iuA", 2))
    throwsRs "unknown block" (fun () -> rs.IsBlockInUse "iuUnknown")
    )

test "IsBlockEmbedded" (fun () ->
    makeBlock "ieA" |> ignore
    ok "a block created in the document is embedded" (rs.IsBlockEmbedded "ieA")
    throwsRs "unknown block" (fun () -> rs.IsBlockEmbedded "ieUnknown")
    )

test "IsBlockReference" (fun () ->
    makeBlock "irA" |> ignore
    ok "a block created in the document is not a reference" (not (rs.IsBlockReference "irA"))
    throwsRs "unknown block" (fun () -> rs.IsBlockReference "irUnknown")
    )

test "BlockPath" (fun () ->
    makeBlock "bpA" |> ignore
    ok "a block that is not linked has no path" (String.IsNullOrEmpty (rs.BlockPath "bpA"))
    throwsRs "unknown block" (fun () -> rs.BlockPath "bpUnknown")
    )

test "BlockStatus" (fun () ->
    makeBlock "bstA" |> ignore
    eq "not a linked block definition" -3 (rs.BlockStatus "bstA")
    eq "unknown block gives -3 too" -3 (rs.BlockStatus "bstUnknown")
    )

test "RenameBlock" (fun () ->
    makeBlock "rbA" |> ignore
    let inst = rs.InsertBlock("rbA", pt 0. 0. 0.)
    ok "rename succeeds" (rs.RenameBlock("rbA", "rbB"))
    ok "new name exists" (rs.IsBlock "rbB")
    ok "old name is gone" (not (rs.IsBlock "rbA"))
    eq "instances follow the new name" "rbB" (rs.BlockInstanceName inst)
    makeBlock "rbC" |> ignore
    ok "duplicate name fails without a dialog" (not (rs.RenameBlock("rbB", "rbC")))
    ok "original name remains" (rs.IsBlock "rbB")
    throwsRs "unknown block" (fun () -> rs.RenameBlock("rbUnknown", "rbX"))
    )

test "DeleteBlock" (fun () ->
    makeBlock "dbA" |> ignore
    let inst = rs.InsertBlock("dbA", pt 0. 0. 0.)
    ok "returns true" (rs.DeleteBlock "dbA")
    ok "block is gone" (not (rs.IsBlock "dbA"))
    ok "instances are deleted too" (not (rs.IsBlockInstance inst))
    throwsRs "unknown block" (fun () -> rs.DeleteBlock "dbUnknown")
    )

test "ExplodeBlockInstance" (fun () ->
    makeBlock "ebA" |> ignore
    let inst = rs.InsertBlock("ebA", pt 10. 0. 0.)
    let n = doc().Objects.Count
    let ids = rs.ExplodeBlockInstance inst
    eq "two new objects" 2 ids.Length
    ok "the block instance is deleted" (not (rs.IsBlockInstance inst))
    ok "the two new objects are in the document" (ids |> Array.forall (fun i -> not (isNull (doc().Objects.FindId i)) && not (doc().Objects.FindId i).IsDeleted))
    let line = ids |> Array.find (fun i -> rs.IsCurve i)
    nearPt "exploded line start is moved by the insertion" (pt 11. 0. 0.) (rs.CurveStartPoint line)
    // nested
    let nestedInst = rs.InsertBlock("ebA", pt 0. 0. 0.)
    rs.AddBlock([ nestedInst ], pt 0. 0. 0., "ebOuter", true) |> ignore
    let outer = rs.InsertBlock("ebOuter", pt 0. 0. 0.)
    let flat = rs.ExplodeBlockInstance(outer, false)
    eq "nested block stays a block instance when not exploding nested" 1 flat.Length
    ok "the result is a block instance" (rs.IsBlockInstance flat.[0])
    let outer2 = rs.InsertBlock("ebOuter", pt 0. 0. 0.)
    let deep = rs.ExplodeBlockInstance(outer2, true)
    eq "nested blocks are exploded" 2 deep.Length
    ok "no block instances remain" (deep |> Array.forall (fun i -> not (rs.IsBlockInstance i)))
    throws "not a block instance" (fun () -> rs.ExplodeBlockInstance (rs.AddPoint(0., 0., 0.)))
    )

test "ExplodeBlockInstance (geometry)" (fun () ->
    makeBlock "egA" |> ignore
    let inst = rs.InsertBlock("egA", pt 10. 0. 0.)
    let n = doc().Objects.Count
    let geo = (doc().Objects.FindId inst).Geometry :?> InstanceReferenceGeometry
    let pieces = rs.ExplodeBlockInstance geo
    eq "two pieces" 2 pieces.Length
    eq "the document is not changed" n (doc().Objects.Count)
    ok "the block instance still exists" (rs.IsBlockInstance inst)
    let c = pieces |> Array.pick (function :? LineCurve as c -> Some c | _ -> None)
    nearPt "transformed copy" (pt 11. 0. 0.) c.PointAtStart
    throwsRs "null geometry" (fun () -> rs.ExplodeBlockInstance (null : InstanceReferenceGeometry))
    )

test "ReplaceBlockObjects" (fun () ->
    makeBlock "rpA" |> ignore
    let c = rs.AddCircle(Plane.WorldXY, 3.)
    ok "replace succeeds" (rs.ReplaceBlockObjects("rpA", [ c ]))
    eq "one object in the block" 1 (rs.BlockObjectCount "rpA")
    ok "input is deleted by default" (isNull (doc().Objects.FindId c))
    let c2 = rs.AddCircle(Plane.WorldXY, 4.)
    let l = rs.AddLine(pt 0. 0. 0., pt 1. 1. 1.)
    ok "replace again keeping the input" (rs.ReplaceBlockObjects("rpA", [ c2; l ], false))
    eq "two objects in the block" 2 (rs.BlockObjectCount "rpA")
    ok "inputs are kept with deleteInput=false" (not (isNull (doc().Objects.FindId c2)) && not (isNull (doc().Objects.FindId l)))
    throwsRs "unknown block" (fun () -> rs.ReplaceBlockObjects("rpUnknown", [ c2 ]))
    )

printfn "%s" (finish ())
