// Tests for Src/Scripting_Group.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Group"

let pts n = [ for i in 1 .. n -> rs.AddPoint(float i, 0., 0.) ]
let obj (id:Guid) = doc().Objects.FindId id

test "GroupNames" (fun () ->
    let empty = rs.GroupNames()
    ok "no groups gives an empty array, not null" (not (isNull empty) && empty.Length = 0)
    let a = rs.AddGroup("gnA")
    let b = rs.AddGroup("gnB")
    let names = rs.GroupNames()
    ok "contains both names" (Array.contains "gnA" names && Array.contains "gnB" names)
    eq "count matches" names.Length (rs.GroupCount())
    rs.DeleteGroup a
    rs.DeleteGroup b
    )

test "GroupCount" (fun () ->
    let n = rs.GroupCount()
    rs.AddGroup("gcA") |> ignore
    eq "one more group" (n + 1) (rs.GroupCount())
    rs.DeleteGroup "gcA"
    eq "back to the old count" n (rs.GroupCount())
    )

test "AddGroup" (fun () ->
    let named = rs.AddGroup("agNamed")
    eq "returns the given name" "agNamed" named
    ok "group exists" (rs.IsGroup "agNamed")
    let auto = rs.AddGroup()
    ok "a name is generated if omitted" (not (String.IsNullOrEmpty auto))
    ok "generated group exists" (rs.IsGroup auto)
    let auto2 = rs.AddGroup(null)
    ok "null generates another new name" (auto2 <> auto)
    ok "group is empty" (rs.IsGroupEmpty named)
    let dup = rs.AddGroup("agNamed")
    ok "a duplicate name gets a new unique name (Rhino adds a number)" (dup <> "agNamed" && rs.IsGroup dup)
    rs.DeleteGroup dup
    rs.DeleteGroup named
    rs.DeleteGroup auto
    rs.DeleteGroup auto2
    )

test "IsGroup" (fun () ->
    rs.AddGroup("igA") |> ignore
    ok "existing group" (rs.IsGroup "igA")
    ok "unknown group" (not (rs.IsGroup "igUnknown"))
    rs.DeleteGroup "igA"
    ok "deleted group" (not (rs.IsGroup "igA"))
    )

test "IsGroupEmpty" (fun () ->
    rs.AddGroup("ieA") |> ignore
    ok "new group is empty" (rs.IsGroupEmpty "ieA")
    rs.AddObjectToGroup((pts 1).[0], "ieA")
    ok "group with an object is not empty" (not (rs.IsGroupEmpty "ieA"))
    throwsRs "unknown group" (fun () -> rs.IsGroupEmpty "ieUnknown")
    rs.DeleteGroup "ieA"
    )

test "AddObjectToGroup" (fun () ->
    rs.AddGroup("aoA") |> ignore
    let p = (pts 1).[0]
    rs.AddObjectToGroup(p, "aoA")
    eq "object is in the group" [ "aoA" ] (List.ofSeq (rs.ObjectGroups p))
    throwsRs "unknown group" (fun () -> rs.AddObjectToGroup(p, "aoUnknown"))
    throws "unknown object" (fun () -> rs.AddObjectToGroup(Guid.NewGuid(), "aoA"))
    rs.DeleteGroup "aoA"
    )

test "AddObjectsToGroup" (fun () ->
    rs.AddGroup("asA") |> ignore
    let ps = pts 3
    rs.AddObjectsToGroup(ps, "asA")
    for p in ps do eq "object is in the group" [ "asA" ] (List.ofSeq (rs.ObjectGroups p))
    throwsRs "unknown group" (fun () -> rs.AddObjectsToGroup(ps, "asUnknown"))
    throwsRs "empty list" (fun () -> rs.AddObjectsToGroup([], "asA"))
    rs.DeleteGroup "asA"
    )

test "GroupObjects" (fun () ->
    let before = rs.GroupCount()
    let ps = pts 2
    rs.GroupObjects ps
    eq "one new group" (before + 1) (rs.GroupCount())
    let g = rs.ObjectGroups ps.[0]
    eq "first object is in one group" 1 g.Count
    eq "both objects are in the same group" (List.ofSeq g) (List.ofSeq (rs.ObjectGroups ps.[1]))
    throwsRs "only one object" (fun () -> rs.GroupObjects [ ps.[0] ])
    throwsRs "no objects" (fun () -> rs.GroupObjects [])
    // with a name
    let ps2 = pts 3
    rs.GroupObjects(ps2, "goNamed")
    ok "the named group exists" (rs.IsGroup "goNamed")
    for p in ps2 do eq "objects are in the named group" [ "goNamed" ] (List.ofSeq (rs.ObjectGroups p))
    throwsRs "named, only one object" (fun () -> rs.GroupObjects([ ps2.[0] ], "goNamed2"))
    for n in rs.GroupNames() do rs.DeleteGroup n
    )

test "DeleteGroup" (fun () ->
    let ps = pts 2
    rs.GroupObjects(ps, "dgA")
    rs.DeleteGroup "dgA"
    ok "group is gone" (not (rs.IsGroup "dgA"))
    ok "member objects still exist" (ps |> List.forall (fun p -> not (isNull (obj p))))
    eq "objects are in no group anymore" 0 (rs.ObjectGroups ps.[0]).Count
    throwsRs "unknown group" (fun () -> rs.DeleteGroup "dgUnknown")
    )

test "RenameGroup" (fun () ->
    let ps = pts 2
    rs.GroupObjects(ps, "rgOld")
    rs.RenameGroup("rgOld", "rgNew")
    ok "new name exists" (rs.IsGroup "rgNew")
    ok "old name is gone" (not (rs.IsGroup "rgOld"))
    eq "objects keep their group" [ "rgNew" ] (List.ofSeq (rs.ObjectGroups ps.[0]))
    throwsRs "unknown group" (fun () -> rs.RenameGroup("rgUnknown", "rgOther"))
    rs.AddGroup("rgOther") |> ignore
    throwsRs "name already in use" (fun () -> rs.RenameGroup("rgNew", "rgOther"))
    rs.DeleteGroup "rgNew"
    rs.DeleteGroup "rgOther"
    )

test "RemoveObjectFromGroup" (fun () ->
    let ps = pts 3
    rs.GroupObjects(ps, "rmA")
    rs.RemoveObjectFromGroup(ps.[0], "rmA")
    eq "removed object is in no group" 0 (rs.ObjectGroups ps.[0]).Count
    eq "other object is still in the group" 1 (rs.ObjectGroups ps.[1]).Count
    throwsRs "unknown group" (fun () -> rs.RemoveObjectFromGroup(ps.[1], "rmUnknown"))
    throws "unknown object" (fun () -> rs.RemoveObjectFromGroup(Guid.NewGuid(), "rmA"))
    rs.DeleteGroup "rmA"
    )

test "RemoveObjectsFromGroup" (fun () ->
    let ps = pts 4
    rs.GroupObjects(ps, "rmsA")
    rs.RemoveObjectsFromGroup([ ps.[0]; ps.[1] ], "rmsA")
    eq "first removed" 0 (rs.ObjectGroups ps.[0]).Count
    eq "second removed" 0 (rs.ObjectGroups ps.[1]).Count
    eq "third remains" 1 (rs.ObjectGroups ps.[2]).Count
    throwsRs "unknown group" (fun () -> rs.RemoveObjectsFromGroup(ps, "rmsUnknown"))
    throws "unknown object" (fun () -> rs.RemoveObjectsFromGroup([ Guid.NewGuid() ], "rmsA"))
    rs.DeleteGroup "rmsA"
    )

test "RemoveObjectFromAllGroups" (fun () ->
    let ps = pts 3
    rs.GroupObjects(ps, "raA")
    rs.GroupObjects(ps, "raB")
    eq "object is in two groups" 2 (rs.ObjectGroups ps.[0]).Count
    ok "returns true when groups were removed" (rs.RemoveObjectFromAllGroups ps.[0])
    eq "object is in no group" 0 (rs.ObjectGroups ps.[0]).Count
    ok "returns false when the object is in no group" (not (rs.RemoveObjectFromAllGroups ps.[0]))
    eq "other objects are still in two groups" 2 (rs.ObjectGroups ps.[1]).Count
    throws "unknown object" (fun () -> rs.RemoveObjectFromAllGroups(Guid.NewGuid()))
    rs.DeleteGroup "raA"
    rs.DeleteGroup "raB"
    )

test "HideGroup" (fun () ->
    let ps = pts 3
    rs.GroupObjects(ps, "hgA")
    eq "number of hidden objects" 3 (rs.HideGroup "hgA")
    ok "objects are hidden" (ps |> List.forall (fun p -> (obj p).IsHidden))
    eq "hiding again hides nothing" 0 (rs.HideGroup "hgA")
    throwsRs "unknown group" (fun () -> rs.HideGroup "hgUnknown")
    rs.DeleteGroup "hgA"
    )

test "ShowGroup" (fun () ->
    let ps = pts 3
    rs.GroupObjects(ps, "shA")
    rs.HideGroup "shA" |> ignore
    eq "number of shown objects" 3 (rs.ShowGroup "shA")
    ok "objects are visible" (ps |> List.forall (fun p -> not (obj p).IsHidden))
    eq "showing again shows nothing" 0 (rs.ShowGroup "shA")
    throwsRs "unknown group" (fun () -> rs.ShowGroup "shUnknown")
    rs.DeleteGroup "shA"
    )

test "LockGroup" (fun () ->
    let ps = pts 3
    rs.GroupObjects(ps, "lgA")
    eq "number of locked objects" 3 (rs.LockGroup "lgA")
    ok "objects are locked" (ps |> List.forall (fun p -> (obj p).IsLocked))
    ok "locking again does not fail" (rs.LockGroup "lgA" >= 0)
    throwsRs "unknown group" (fun () -> rs.LockGroup "lgUnknown")
    rs.UnlockGroup "lgA" |> ignore
    rs.DeleteGroup "lgA"
    )

test "UnlockGroup" (fun () ->
    let ps = pts 3
    rs.GroupObjects(ps, "ulA")
    rs.LockGroup "ulA" |> ignore
    eq "number of unlocked objects" 3 (rs.UnlockGroup "ulA")
    ok "objects are unlocked" (ps |> List.forall (fun p -> not (obj p).IsLocked))
    ok "unlocking again does not fail" (rs.UnlockGroup "ulA" >= 0)
    throwsRs "unknown group" (fun () -> rs.UnlockGroup "ulUnknown")
    rs.DeleteGroup "ulA"
    )

test "ObjectGroups" (fun () ->
    let ps = pts 3
    ok "no group gives an empty list" ((rs.ObjectGroups ps.[0]).Count = 0)
    rs.GroupObjects(ps, "ogFirst")
    rs.GroupObjects([ ps.[0]; ps.[1] ], "ogSecond")
    rs.GroupObjects([ ps.[0]; ps.[1] ], "ogThird")
    eq "nested groups, sorted from bottom to top (by group index)" [ "ogFirst"; "ogSecond"; "ogThird" ] (List.ofSeq (rs.ObjectGroups ps.[0]))
    eq "object in only one group" [ "ogFirst" ] (List.ofSeq (rs.ObjectGroups ps.[2]))
    throws "unknown object" (fun () -> rs.ObjectGroups(Guid.NewGuid()))
    for n in rs.GroupNames() do rs.DeleteGroup n
    )

test "ObjectTopGroup" (fun () ->
    let ps = pts 3
    rs.GroupObjects(ps, "otFirst")
    eq "single group" "otFirst" (rs.ObjectTopGroup ps.[0])
    rs.GroupObjects([ ps.[0]; ps.[1] ], "otSecond")
    eq "the group with the highest index is the top group" "otSecond" (rs.ObjectTopGroup ps.[0])
    eq "other object keeps its only group" "otFirst" (rs.ObjectTopGroup ps.[2]) |> ignore
    throwsRs "object that is in no group" (fun () -> rs.ObjectTopGroup (rs.AddPoint(9., 9., 9.)))
    throws "unknown object" (fun () -> rs.ObjectTopGroup(Guid.NewGuid()))
    for n in rs.GroupNames() do rs.DeleteGroup n
    )

printfn "%s" (finish ())
