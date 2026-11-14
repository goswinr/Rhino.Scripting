// Tests for Src/Scripting_Linetype.fs
#load "Harness.fsx"
open System
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Linetype"

let addTestLinetype (name:string) =
    let lt = new Rhino.DocObjects.Linetype(Name = name)
    lt.AppendSegment(5.0, true) |> ignore
    lt.AppendSegment(2.0, false) |> ignore
    let index = doc().Linetypes.Add lt
    ok "linetype was added to the document" (index >= 0)

test "IsLinetype" (fun () ->
    ok "Continuous exists" (rs.IsLinetype "Continuous")
    ok "name is not case sensitive" (rs.IsLinetype "cOnTiNuOuS")
    addTestLinetype "TestDashIs"
    ok "added linetype exists" (rs.IsLinetype "TestDashIs")
    ok "added linetype name is not case sensitive" (rs.IsLinetype "testDASHis")
    ok "unknown name is false" (not (rs.IsLinetype "no such linetype 123"))
    )

test "IsLinetypeReference" (fun () ->
    addTestLinetype "TestDashRef"
    eq "a linetype added to the document is not a reference linetype" false (rs.IsLinetypeReference "TestDashRef")
    throwsRs "unknown linetype raises" (fun () -> rs.IsLinetypeReference "no such linetype 123")
    )

test "LinetypeCount" (fun () ->
    let n0 = rs.LinetypeCount()
    addTestLinetype "TestDashA"
    eq "count grows by one after adding a linetype" (n0 + 1) (rs.LinetypeCount())
    eq "count equals the number of names" (rs.LinetypeCount()) (rs.LinetypeNames().Count)
    )

test "LinetypeNames" (fun () ->
    addTestLinetype "TestDashB"
    addTestLinetype "TestDashA2"
    let names = rs.LinetypeNames()
    ok "contains the added linetypes" (names.Contains "TestDashB" && names.Contains "TestDashA2")
    let sorted = rs.LinetypeNames(true)
    eq "same number of names when sorted" names.Count sorted.Count
    let expected = names |> Seq.sortWith (fun a b -> String.CompareOrdinal(a, b)) |> Seq.toList
    eq "sorted in ordinal order" expected (List.ofSeq sorted)
    eq "default and false are the same" (List.ofSeq names) (List.ofSeq (rs.LinetypeNames(false)))
    )

printfn "%s" (finish ())
