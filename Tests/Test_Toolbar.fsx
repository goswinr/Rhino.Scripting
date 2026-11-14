// Tests for Src/Scripting_Toolbar.fs
// Only read-only functions are tested, because toolbar collections belong to the Rhino session of the user,
// not to the test document. Opening, closing and saving collection files is skipped.
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Toolbar"

let names = rs.ToolbarCollectionNames()
// a collection that has toolbars. In Rhino 8 the default collection has toolbars but no toolbar groups.
let collection = names |> Seq.tryFind (fun n -> rs.ToolbarCount(n, false) > 0)
let hasGroups (c:string) = rs.ToolbarCount(c, true) > 0

test "ToolbarCollectionCount" (fun () ->
    eq "same as the number of names" (rs.ToolbarCollectionCount()) names.Count
    ok "Rhino has at least one open toolbar collection" (names.Count > 0)
    )

test "ToolbarCollectionNames" (fun () ->
    ok "names are not empty" (names |> Seq.forall (fun n -> not (String.IsNullOrEmpty n)))
    eq "names are unique" names.Count (Seq.distinct names |> Seq.length)
    )

test "ToolbarCollectionPath" (fun () ->
    for n in names do
        let p = rs.ToolbarCollectionPath n
        ok (sprintf "path of '%s' is a file path" n) (not (String.IsNullOrEmpty p) && p.EndsWith(".rui", StringComparison.OrdinalIgnoreCase))
    eq "unknown collection gives an empty string" "" (rs.ToolbarCollectionPath "no such collection")
    )

test "IsToolbarCollection" (fun () ->
    for n in names do
        eq (sprintf "path of '%s' leads back to its name" n) n (rs.IsToolbarCollection (rs.ToolbarCollectionPath n))
    eq "unknown path gives an empty string" "" (rs.IsToolbarCollection @"C:\no\such\file.rui")
    )

test "ToolbarCount" (fun () ->
    match collection with
    | None -> failwith "no open toolbar collection with toolbars found"
    | Some c ->
        ok "has toolbars" (rs.ToolbarCount c > 0)
        ok "group count is not negative for a known collection" (rs.ToolbarCount(c, true) >= 0)
        eq "default is toolbars, not groups" (rs.ToolbarCount(c, false)) (rs.ToolbarCount c)
    eq "unknown collection gives -1" -1 (rs.ToolbarCount "no such collection")
    eq "unknown collection gives -1 for groups" -1 (rs.ToolbarCount("no such collection", true))
    )

test "ToolbarNames" (fun () ->
    match collection with
    | None -> failwith "no open toolbar collection with toolbars found"
    | Some c ->
        eq "number of toolbar names" (rs.ToolbarCount c) (rs.ToolbarNames c).Count
        eq "number of group names" (rs.ToolbarCount(c, true)) (rs.ToolbarNames(c, true)).Count
        ok "toolbar names are not empty" (rs.ToolbarNames c |> Seq.forall (fun n -> not (String.IsNullOrEmpty n)))
    eq "unknown collection gives an empty list" 0 (rs.ToolbarNames "no such collection").Count
    eq "unknown collection gives an empty list of groups" 0 (rs.ToolbarNames("no such collection", true)).Count
    )

test "IsToolbar" (fun () ->
    match collection with
    | None -> failwith "no open toolbar collection with toolbars found"
    | Some c ->
        let t = (rs.ToolbarNames c).[0]
        ok "existing toolbar" (rs.IsToolbar(c, t))
        if hasGroups c then ok "existing group" (rs.IsToolbar(c, (rs.ToolbarNames(c, true)).[0], true))
        ok "a toolbar name is not a group name" (hasGroups c || not (rs.IsToolbar(c, t, true)))
        ok "unknown toolbar" (not (rs.IsToolbar(c, "no such toolbar")))
        ok "unknown group" (not (rs.IsToolbar(c, "no such group", true)))
    ok "unknown collection" (not (rs.IsToolbar("no such collection", "x")))
    )

// Note: ShowToolbar, HideToolbar, IsToolbarVisible and IsToolbarDocked are obsolete, so their tests are commented out.
// They work on toolbar groups: RhinoCommon lists no toolbar groups in Rhino 8, and in Rhino 9 a group can be shown but not hidden again.
// The old round trip test showed a group and could not hide it, which left the group visible in the Rhino session.
// test "IsToolbarDocked" (fun () ->
//     match collection with
//     | None -> failwith "no open toolbar collection with toolbars found"
//     | Some c ->
//         if hasGroups c then rs.IsToolbarDocked(c, (rs.ToolbarNames(c, true)).[0]) |> ignore // either result is fine, it must not fail
//         throwsRs "unknown group" (fun () -> rs.IsToolbarDocked(c, "no such group"))
//     throwsRs "unknown collection" (fun () -> rs.IsToolbarDocked("no such collection", "x"))
//     )

// test "IsToolbarVisible" (fun () ->
//     match collection with
//     | None -> failwith "no open toolbar collection with toolbars found"
//     | Some c ->
//         if hasGroups c then rs.IsToolbarVisible(c, (rs.ToolbarNames(c, true)).[0]) |> ignore
//         throwsRs "unknown group" (fun () -> rs.IsToolbarVisible(c, "no such group"))
//     throwsRs "unknown collection" (fun () -> rs.IsToolbarVisible("no such collection", "x"))
//     )

// test "ShowToolbar and HideToolbar (unknown names)" (fun () ->
//     ok "unknown collection can not be shown" (not (rs.ShowToolbar("no such collection", "x")))
//     ok "unknown collection can not be hidden" (not (rs.HideToolbar("no such collection", "x")))
//     match collection with
//     | Some c ->
//         ok "unknown group can not be shown" (not (rs.ShowToolbar(c, "no such group")))
//         ok "unknown group can not be hidden" (not (rs.HideToolbar(c, "no such group")))
//     | None -> ()
//     )

skip "ShowToolbar, HideToolbar, IsToolbarVisible and IsToolbarDocked" "obsolete: RhinoCommon lists no toolbar groups in Rhino 8, in Rhino 9 a group can be shown but not hidden again"

skip "OpenToolbarCollection" "would change the toolbars of the user's Rhino session"
skip "CloseToolbarCollection" "would close toolbars of the user's Rhino session"
skip "SaveToolbarCollection" "would write the user's toolbar files"
skip "SaveToolbarCollectionAs" "would write files"

printfn "%s" (finish ())
