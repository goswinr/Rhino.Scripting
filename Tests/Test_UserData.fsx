// Tests for Src/Scripting_UserData.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "UserData"

let newPoint () = rs.AddPoint(1., 2., 3.)

// ------------------------------- document data (sections and entries) -------------------------------

test "SetDocumentData" (fun () ->
    rs.SetDocumentData("secA", "entry1", "value1")
    eq "value can be read back" "value1" (rs.GetDocumentDataEntry("secA", "entry1"))
    rs.SetDocumentData("secA", "entry1", "changed")
    eq "value is overwritten" "changed" (rs.GetDocumentDataEntry("secA", "entry1"))
    rs.SetDocumentData("secA", "entry2", "value2")
    eq "second entry in the same section" 2 (rs.GetDocumentData "secA").Length
    rs.DeleteDocumentData()
    )

test "GetDocumentData" (fun () ->
    rs.DeleteDocumentData()
    rs.SetDocumentData("secB", "e1", "v1")
    rs.SetDocumentData("secB", "e2", "v2")
    rs.SetDocumentData("secC", "e3", "v3")
    let sections = rs.GetDocumentData()
    ok "section names if no argument" (Array.contains "secB" sections && Array.contains "secC" sections)
    let entries = rs.GetDocumentData "secB"
    eq "entry names of a section" [ "e1"; "e2" ] (entries |> Array.sort |> List.ofArray)
    eq "unknown section has no entries" 0 (rs.GetDocumentData "secUnknown").Length
    rs.DeleteDocumentData()
    )

test "GetDocumentDataEntry" (fun () ->
    rs.SetDocumentData("secD", "e1", "v1")
    eq "existing entry" "v1" (rs.GetDocumentDataEntry("secD", "e1"))
    ok "missing entry gives null or empty" (String.IsNullOrEmpty (rs.GetDocumentDataEntry("secD", "missing")))
    ok "missing section gives null or empty" (String.IsNullOrEmpty (rs.GetDocumentDataEntry("secUnknown", "e1")))
    rs.DeleteDocumentData()
    )

test "DeleteDocumentData" (fun () ->
    rs.SetDocumentData("secE", "e1", "v1")
    rs.SetDocumentData("secE", "e2", "v2")
    rs.SetDocumentData("secF", "e3", "v3")
    rs.DeleteDocumentData("secE", "e1")
    eq "one entry removed" [ "e2" ] (rs.GetDocumentData "secE" |> List.ofArray)
    rs.DeleteDocumentData("secE")
    eq "section removed" 0 (rs.GetDocumentData "secE").Length
    ok "other section remains" (Array.contains "secF" (rs.GetDocumentData()))
    rs.DeleteDocumentData()
    eq "everything removed if called without arguments" 0 (rs.DocumentDataCount())
    )

test "DocumentDataCount" (fun () ->
    rs.DeleteDocumentData()
    eq "empty" 0 (rs.DocumentDataCount())
    rs.SetDocumentData("secG", "e1", "v1")
    rs.SetDocumentData("secG", "e2", "v2")
    eq "two entries" 2 (rs.DocumentDataCount())
    rs.DeleteDocumentData()
    )

test "IsDocumentData" (fun () ->
    rs.DeleteDocumentData()
    ok "none at first" (not (rs.IsDocumentData()))
    rs.SetDocumentData("secH", "e1", "v1")
    ok "true after setting" (rs.IsDocumentData())
    rs.DeleteDocumentData()
    ok "false after deleting" (not (rs.IsDocumentData()))
    )

// ------------------------------- document user text -------------------------------

// Not using rs.DeleteDocumentUserText here: it leaves the key in the document (see the test DeleteDocumentUserText)
let clearDocumentUserText () =
    for k in Seq.toArray (rs.GetDocumentUserTextKeys()) do doc().Strings.Delete k

test "SetDocumentUserText" (fun () ->
    clearDocumentUserText ()
    rs.SetDocumentUserText("dutKey", "dutValue")
    eq "value round trip" "dutValue" (rs.GetDocumentUserText "dutKey")
    rs.SetDocumentUserText("dutKey", "other")
    eq "overwritten" "other" (rs.GetDocumentUserText "dutKey")
    rs.SetDocumentUserText("dutEmpty", "") // accepted, but Rhino does not store an empty value
    ok "empty value gives no value back" (String.IsNullOrEmpty (rs.GetDocumentUserText "dutEmpty"))
    throwsRs "empty key" (fun () -> rs.SetDocumentUserText("", "v"))
    throwsRs "key with a tab" (fun () -> rs.SetDocumentUserText("a\tb", "v"))
    throwsRs "key with leading whitespace" (fun () -> rs.SetDocumentUserText(" a", "v"))
    throwsRs "value with a line return" (fun () -> rs.SetDocumentUserText("dutK2", "a\nb"))
    throwsRs "value with a look-alike unicode quote character" (fun () -> rs.SetDocumentUserText("dutK3", "a‘b"))
    rs.SetDocumentUserText("dutK3", "äö", true)
    eq "all unicode characters are allowed if requested" "äö" (rs.GetDocumentUserText "dutK3")
    clearDocumentUserText ()
    )

test "GetDocumentUserText" (fun () ->
    clearDocumentUserText ()
    rs.SetDocumentUserText("gutKey", "gutValue")
    eq "existing key" "gutValue" (rs.GetDocumentUserText "gutKey")
    ok "missing key gives null or empty" (String.IsNullOrEmpty (rs.GetDocumentUserText "gutMissing"))
    clearDocumentUserText ()
    )

test "GetDocumentUserTextKeys" (fun () ->
    clearDocumentUserText ()
    rs.DeleteDocumentData()
    rs.SetDocumentUserText("gkA", "1")
    rs.SetDocumentUserText("gkB", "2")
    rs.SetDocumentData("secKeys", "entry", "value") // stored with a backslash in the key, must not be listed as user text
    let keys = rs.GetDocumentUserTextKeys()
    eq "only the user text keys" [ "gkA"; "gkB" ] (keys |> Seq.sort |> List.ofSeq)
    rs.DeleteDocumentData()
    clearDocumentUserText ()
    )

test "DocumentUserTextCount" (fun () ->
    clearDocumentUserText ()
    let n = rs.DocumentUserTextCount()
    rs.SetDocumentUserText("cntA", "1")
    rs.SetDocumentUserText("cntB", "2")
    eq "two more" (n + 2) (rs.DocumentUserTextCount())
    clearDocumentUserText ()
    eq "back to the old count" n (rs.DocumentUserTextCount())
    )

test "IsDocumentUserText" (fun () ->
    clearDocumentUserText ()
    ok "none at first" (not (rs.IsDocumentUserText()))
    rs.SetDocumentUserText("isuA", "1")
    ok "true after setting" (rs.IsDocumentUserText())
    clearDocumentUserText ()
    ok "false after deleting" (not (rs.IsDocumentUserText()))
    )

test "DeleteDocumentUserText" (fun () ->
    clearDocumentUserText ()
    rs.SetDocumentUserText("delA", "1")
    rs.DeleteDocumentUserText "delA"
    ok "key is removed" (not (Seq.contains "delA" (rs.GetDocumentUserTextKeys())))
    throwsRs "key does not exist anymore" (fun () -> rs.DeleteDocumentUserText "delA")
    throwsRs "null key" (fun () -> rs.DeleteDocumentUserText null)
    )

// ------------------------------- object user text -------------------------------

test "SetUserText" (fun () ->
    let p = newPoint ()
    rs.SetUserText(p, "k1", "v1")
    eq "attribute user text round trip" "v1" (rs.GetUserText(p, "k1"))
    ok "not on the geometry" (not (rs.HasUserText(p, "k1", true)))
    rs.SetUserText(p, "k2", "v2", true)
    eq "geometry user text round trip" "v2" (rs.GetUserText(p, "k2", true))
    ok "not on the attributes" (not (rs.HasUserText(p, "k2")))
    rs.SetUserText(p, "k1", "changed")
    eq "overwritten" "changed" (rs.GetUserText(p, "k1"))
    rs.SetUserText(p, "k3", "äö", false, true)
    eq "all unicode if requested" "äö" (rs.GetUserText(p, "k3"))
    throwsRs "empty value" (fun () -> rs.SetUserText(p, "k4", ""))
    throwsRs "empty key" (fun () -> rs.SetUserText(p, "", "v"))
    throwsRs "key with a tab" (fun () -> rs.SetUserText(p, "a\tb", "v"))
    throwsRs "value with a line return" (fun () -> rs.SetUserText(p, "k5", "a\nb"))
    throwsRs "look-alike unicode quote in the value" (fun () -> rs.SetUserText(p, "k6", "a‘b"))
    throws "unknown object" (fun () -> rs.SetUserText(Guid.NewGuid(), "k", "v"))
    )

test "SetUserText (several objects)" (fun () ->
    let ps = [ newPoint (); newPoint (); newPoint () ]
    rs.SetUserText(ps, "multi", "same")
    for p in ps do eq "every object has the text" "same" (rs.GetUserText(p, "multi"))
    rs.SetUserText(ps, "multiGeo", "geo", true)
    for p in ps do eq "every object has the geometry text" "geo" (rs.GetUserText(p, "multiGeo", true))
    throwsRs "empty value" (fun () -> rs.SetUserText(ps, "multi", ""))
    throwsRs "bad key" (fun () -> rs.SetUserText(ps, " bad", "v"))
    throws "unknown object in the list" (fun () -> rs.SetUserText([ ps.[0]; Guid.NewGuid() ], "multi", "x"))
    )

test "GetUserText" (fun () ->
    let p = newPoint ()
    rs.SetUserText(p, "gt", "value")
    eq "existing key" "value" (rs.GetUserText(p, "gt"))
    throwsRs "missing key lists the available keys" (fun () -> rs.GetUserText(p, "missing"))
    throwsRs "key is on attributes not on geometry" (fun () -> rs.GetUserText(p, "gt", true))
    throws "unknown object" (fun () -> rs.GetUserText(Guid.NewGuid(), "gt"))
    )

test "TryGetUserText" (fun () ->
    let p = newPoint ()
    rs.SetUserText(p, "tg", "value")
    eq "Some for an existing key" (Some "value") (rs.TryGetUserText(p, "tg"))
    eq "None for a missing key" None (rs.TryGetUserText(p, "missing"))
    eq "None if it is not on the geometry" None (rs.TryGetUserText(p, "tg", true))
    throws "unknown object" (fun () -> rs.TryGetUserText(Guid.NewGuid(), "tg"))
    )

test "HasUserText" (fun () ->
    let p = newPoint ()
    rs.SetUserText(p, "hu", "value")
    ok "existing key" (rs.HasUserText(p, "hu"))
    ok "missing key" (not (rs.HasUserText(p, "missing")))
    ok "not on the geometry" (not (rs.HasUserText(p, "hu", true)))
    rs.SetUserText(p, "hg", "value", true)
    ok "on the geometry" (rs.HasUserText(p, "hg", true))
    throws "unknown object" (fun () -> rs.HasUserText(Guid.NewGuid(), "hu"))
    )

test "GetUserTextKeys" (fun () ->
    let p = newPoint ()
    eq "no keys" 0 (rs.GetUserTextKeys p).Count
    rs.SetUserText(p, "ka", "1")
    rs.SetUserText(p, "kb", "2")
    rs.SetUserText(p, "kg", "3", true)
    eq "attribute keys" [ "ka"; "kb" ] (rs.GetUserTextKeys p |> Seq.sort |> List.ofSeq)
    eq "geometry keys" [ "kg" ] (rs.GetUserTextKeys(p, true) |> List.ofSeq)
    throws "unknown object" (fun () -> rs.GetUserTextKeys(Guid.NewGuid()))
    )

test "IsUserText" (fun () ->
    let p = newPoint ()
    eq "no user text" 0 (rs.IsUserText p)
    rs.SetUserText(p, "a", "1")
    eq "attribute user text" 1 (rs.IsUserText p)
    rs.SetUserText(p, "g", "1", true)
    eq "attribute and geometry user text" 3 (rs.IsUserText p)
    let q = newPoint ()
    rs.SetUserText(q, "g", "1", true)
    eq "geometry user text only" 2 (rs.IsUserText q)
    throws "unknown object" (fun () -> rs.IsUserText(Guid.NewGuid()))
    )

test "DeleteUserText" (fun () ->
    let p = newPoint ()
    rs.SetUserText(p, "d1", "v")
    rs.SetUserText(p, "d2", "v", true)
    rs.DeleteUserText(p, "d1")
    ok "attribute key removed" (not (rs.HasUserText(p, "d1")))
    ok "geometry key stays" (rs.HasUserText(p, "d2", true))
    rs.DeleteUserText(p, "d2", true)
    ok "geometry key removed" (not (rs.HasUserText(p, "d2", true)))
    rs.DeleteUserText(p, "neverExisted") // does nothing
    throws "unknown object" (fun () -> rs.DeleteUserText(Guid.NewGuid(), "d1"))
    )

test "DeleteUserText (several objects)" (fun () ->
    let ps = [ newPoint (); newPoint () ]
    rs.SetUserText(ps, "dm", "v")
    rs.SetUserText(ps, "dg", "v", true)
    rs.DeleteUserText(ps, "dm")
    for p in ps do ok "attribute key removed" (not (rs.HasUserText(p, "dm")))
    for p in ps do ok "geometry key stays" (rs.HasUserText(p, "dg", true))
    rs.DeleteUserText(ps, "dg", true)
    for p in ps do ok "geometry key removed" (not (rs.HasUserText(p, "dg", true)))
    throws "unknown object in the list" (fun () -> rs.DeleteUserText([ ps.[0]; Guid.NewGuid() ], "dm"))
    )

printfn "%s" (finish ())
