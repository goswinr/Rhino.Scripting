// Tests for Src/Scripting_Layer.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Layer"

let red = Drawing.Color.FromArgb(255, 0, 0)
let green = Drawing.Color.FromArgb(0, 255, 0)
let rgb (c:Drawing.Color) = (int c.R, int c.G, int c.B)
let layers () = doc().Layers
let fullPath (index:int) = (layers ()).[index].FullPath
let newPoint () = rs.AddPoint(1., 2., 3.)
let remove (names: string list) = for n in names do (try rs.PurgeLayer n |> ignore with _ -> ())

// ------------------------------- AddLayer -------------------------------

test "AddLayer" (fun () ->
    let i = rs.AddLayer("alA")
    eq "full name of the new layer" "alA" (fullPath i)
    ok "layer exists" (rs.IsLayer "alA")
    eq "adding it again returns the same index" i (rs.AddLayer("alA"))
    // color
    let c = rs.AddLayer("alColor", red)
    eq "given color" (rgb red) (rgb (layers ()).[c].Color)
    let c2 = rs.AddLayer("alColor", green)
    eq "existing layer: same index" c c2
    eq "existing layer: color parameter is ignored" (rgb red) (rgb (layers ()).[c].Color)
    // parent
    let child = rs.AddLayer("alChild", Drawing.Color(), 2, 2, "alParent")
    eq "child gets the full path with parent" "alParent::alChild" (fullPath child)
    ok "the parent was created" (rs.IsLayer "alParent")
    eq "parent of the child" "alParent" (rs.ParentLayer "alParent::alChild")
    // visibility
    let off = rs.AddLayer("alOff", Drawing.Color(), 0)
    ok "visible=0 means off" (not (rs.IsLayerOn "alOff"))
    let on = rs.AddLayer("alOn", Drawing.Color(), 1)
    ok "visible=1 means on" (rs.IsLayerOn "alOn")
    // locking
    rs.AddLayer("alLocked", Drawing.Color(), 2, 1) |> ignore
    ok "locked=1 means locked" (rs.IsLayerLocked "alLocked")
    rs.AddLayer("alUnlocked", Drawing.Color(), 2, 0) |> ignore
    ok "locked=0 means unlocked" (not (rs.IsLayerLocked "alUnlocked"))
    // nested name in one string
    let nested = rs.AddLayer("alN1::alN2::alN3")
    eq "nested name creates all levels" "alN1::alN2::alN3" (fullPath nested)
    ok "intermediate levels exist" (rs.IsLayer "alN1::alN2")
    // generated name
    let n0 = rs.LayerCount()
    let auto = rs.AddLayer()
    eq "a layer was added without a name" (n0 + 1) (rs.LayerCount())
    ok "the generated layer has a name" (not (String.IsNullOrEmpty (fullPath auto)))
    // unicode
    throwsRs "ambiguous unicode in the name" (fun () -> rs.AddLayer("al‘Quote"))
    let u = rs.AddLayer("alÄÖ", Drawing.Color(), 2, 2, null, true)
    eq "all unicode if requested" "alÄÖ" (fullPath u)
    // errors
    throwsRs "parent without a name" (fun () -> rs.AddLayer(null, Drawing.Color(), 2, 2, "alParent"))
    throwsRs "bad visible value" (fun () -> rs.AddLayer("alBad1", Drawing.Color(), 5))
    throwsRs "bad locked value" (fun () -> rs.AddLayer("alBad2", Drawing.Color(), 2, 5))
    throwsRs "name with two colons at the end" (fun () -> rs.AddLayer("alBad3::"))
    throwsRs "name with leading whitespace" (fun () -> rs.AddLayer(" alBad4"))
    throwsRs "name that starts with a bracket" (fun () -> rs.AddLayer("(alBad5"))
    throwsRs "empty name" (fun () -> rs.AddLayer(""))
    )

// ------------------------------- ObjectLayer -------------------------------

test "ObjectLayer" (fun () ->
    rs.AddLayer("olA") |> ignore
    rs.AddLayer("olB") |> ignore
    let p = newPoint ()
    ok "new objects are on the current layer" (rs.ObjectLayer p = rs.CurrentLayer())
    rs.ObjectLayer(p, "olA")
    eq "set by name" "olA" (rs.ObjectLayer p)
    rs.ObjectLayer(p, "olB")
    eq "set by name again" "olB" (rs.ObjectLayer p)
    throwsRs "layer does not exist" (fun () -> rs.ObjectLayer(p, "olMissing"))
    rs.ObjectLayer(p, "olNew::sub", true)
    eq "layer is created if requested" "olNew::sub" (rs.ObjectLayer p)
    ok "the new parent layer exists" (rs.IsLayer "olNew")
    // multiple objects
    let ps = [ newPoint (); newPoint () ]
    rs.ObjectLayer(ps, "olA")
    for q in ps do eq "multiple objects set by name" "olA" (rs.ObjectLayer q)
    rs.ObjectLayer(ps, "olMulti", true)
    for q in ps do eq "multiple objects, layer created" "olMulti" (rs.ObjectLayer q)
    throwsRs "multiple objects, layer does not exist" (fun () -> rs.ObjectLayer(ps, "olMissing2"))
    // by index
    let bIndex = rs.AddLayer "olB"
    rs.ObjectLayer(p, bIndex)
    eq "set by index" "olB" (rs.ObjectLayer p)
    rs.ObjectLayer(ps, bIndex)
    for q in ps do eq "multiple objects set by index" "olB" (rs.ObjectLayer q)
    throwsRs "index too big" (fun () -> rs.ObjectLayer(p, 100000))
    throwsRs "negative index" (fun () -> rs.ObjectLayer(p, -1))
    throwsRs "multiple objects, bad index" (fun () -> rs.ObjectLayer(ps, 100000))
    // deleted layer
    let delIndex = rs.AddLayer "olDel"
    rs.DeleteLayer "olDel" |> ignore
    throwsRs "deleted layer index" (fun () -> rs.ObjectLayer(p, delIndex))
    throws "unknown object" (fun () -> rs.ObjectLayer(Guid.NewGuid()))
    )

// ------------------------------- names -------------------------------

test "ChangeLayerName" (fun () ->
    rs.AddLayer("clParent::clOld") |> ignore
    rs.ChangeLayerName("clParent::clOld", "clNew")
    ok "new name exists below the same parent" (rs.IsLayer "clParent::clNew")
    ok "old name is gone" (not (rs.IsLayer "clParent::clOld"))
    rs.AddLayer("clOther") |> ignore
    throwsRs "new name exists already" (fun () -> rs.ChangeLayerName("clParent::clNew", "clNew") |> ignore; rs.AddLayer("clParent::clTwo") |> ignore; rs.ChangeLayerName("clParent::clTwo", "clNew"))
    throwsRs "layer does not exist" (fun () -> rs.ChangeLayerName("clMissing", "x"))
    throwsRs "new name has two colons" (fun () -> rs.ChangeLayerName("clOther", "a::b"))
    throwsRs "new name is empty" (fun () -> rs.ChangeLayerName("clOther", ""))
    throwsRs "ambiguous unicode" (fun () -> rs.ChangeLayerName("clOther", "a‘b"))
    rs.ChangeLayerName("clOther", "clÄ", true)
    ok "unicode allowed on request" (rs.IsLayer "clÄ")
    )

test "RenameLayer" (fun () ->
    rs.AddLayer("rlOld") |> ignore
    rs.RenameLayer("rlOld", "rlNew")
    ok "new name exists" (rs.IsLayer "rlNew")
    ok "old name is gone" (not (rs.IsLayer "rlOld"))
    rs.RenameLayer("rlNew", "rlNew") // same name does nothing
    ok "same name is fine" (rs.IsLayer "rlNew")
    throwsRs "layer does not exist" (fun () -> rs.RenameLayer("rlMissing", "x"))
    )

test "IsLayer" (fun () ->
    rs.AddLayer("ilP::ilC") |> ignore
    ok "existing full path" (rs.IsLayer "ilP::ilC")
    ok "existing parent" (rs.IsLayer "ilP")
    ok "short name of a child is found too" (rs.IsLayer "ilC")
    ok "unknown layer" (not (rs.IsLayer "ilMissing"))
    )

test "LayerId" (fun () ->
    let i = rs.AddLayer("liA")
    eq "id of the layer" (layers ()).[i].Id (rs.LayerId "liA")
    throwsRs "unknown layer" (fun () -> rs.LayerId "liMissing")
    )

test "LayerName" (fun () ->
    let i = rs.AddLayer("lnP::lnC")
    let id = (layers ()).[i].Id
    eq "full path is the default" "lnP::lnC" (rs.LayerName id)
    eq "full path" "lnP::lnC" (rs.LayerName(id, true))
    eq "short name" "lnC" (rs.LayerName(id, false))
    throws "unknown id" (fun () -> rs.LayerName(Guid.NewGuid()))
    )

test "LayerNames" (fun () ->
    rs.AddLayer("lsB") |> ignore
    rs.AddLayer("lsA") |> ignore
    let names = rs.LayerNames()
    ok "contains both" (names.Contains "lsA" && names.Contains "lsB")
    eq "same as LayerCount" (rs.LayerCount()) names.Count
    let sorted = rs.LayerNames true
    eq "sorted in ordinal order" (names |> Seq.sortWith (fun a b -> String.CompareOrdinal(a, b)) |> List.ofSeq) (List.ofSeq sorted)
    rs.DeleteLayer "lsA" |> ignore
    ok "deleted layers are not listed" (not ((rs.LayerNames()).Contains "lsA"))
    )

test "LayerCount" (fun () ->
    let n = rs.LayerCount()
    rs.AddLayer("lcA") |> ignore
    eq "one more" (n + 1) (rs.LayerCount())
    rs.DeleteLayer "lcA" |> ignore
    eq "deleted layers are not counted" n (rs.LayerCount())
    )

test "LayerIds" (fun () ->
    let i = rs.AddLayer("lidA")
    let id = (layers ()).[i].Id
    let ids = rs.LayerIds()
    ok "contains the new id" (ids.Contains id)
    eq "same as LayerCount" (rs.LayerCount()) ids.Count
    rs.DeleteLayer "lidA" |> ignore
    ok "deleted layers are not listed" (not ((rs.LayerIds()).Contains id))
    )

test "LayerOrder" (fun () ->
    rs.AddLayer("loA") |> ignore
    rs.AddLayer("loB") |> ignore
    let a = rs.LayerOrder "loA"
    let b = rs.LayerOrder "loB"
    ok "indices are not negative" (a >= 0 && b >= 0)
    ok "layers have different positions" (a <> b)
    throwsRs "unknown layer" (fun () -> rs.LayerOrder "loMissing")
    )

// ------------------------------- current layer -------------------------------

test "CurrentLayer" (fun () ->
    let before = rs.CurrentLayer()
    rs.AddLayer("curA::curB") |> ignore
    rs.CurrentLayer "curA::curB"
    eq "current layer was set" "curA::curB" (rs.CurrentLayer())
    ok "IsLayerCurrent agrees" (rs.IsLayerCurrent "curA::curB")
    ok "other layer is not current" (not (rs.IsLayerCurrent "curA"))
    throwsRs "unknown layer" (fun () -> rs.CurrentLayer "curMissing")
    rs.CurrentLayer before
    )

test "IsLayerCurrent" (fun () ->
    let before = rs.CurrentLayer()
    ok "current layer" (rs.IsLayerCurrent before)
    rs.AddLayer("icA") |> ignore
    ok "other layer" (not (rs.IsLayerCurrent "icA"))
    throwsRs "unknown layer" (fun () -> rs.IsLayerCurrent "icMissing")
    )

// ------------------------------- deleting -------------------------------

test "DeleteLayer" (fun () ->
    rs.AddLayer("dlA") |> ignore
    ok "empty layer is deleted" (rs.DeleteLayer "dlA")
    ok "layer is gone" (not (rs.IsLayer "dlA"))
    rs.AddLayer("dlFull") |> ignore
    rs.ObjectLayer(newPoint (), "dlFull")
    ok "layer with objects is not deleted" (not (rs.DeleteLayer "dlFull"))
    ok "it still exists" (rs.IsLayer "dlFull")
    rs.AddLayer("dlCur") |> ignore
    let before = rs.CurrentLayer()
    rs.CurrentLayer "dlCur"
    ok "current layer is not deleted" (not (rs.DeleteLayer "dlCur"))
    rs.CurrentLayer before
    throwsRs "unknown layer" (fun () -> rs.DeleteLayer "dlMissing")
    )

test "PurgeLayer" (fun () ->
    rs.AddLayer("plA") |> ignore
    let p = newPoint ()
    rs.ObjectLayer(p, "plA")
    ok "purge succeeds" (rs.PurgeLayer "plA")
    ok "layer is gone" (not (rs.IsLayer "plA"))
    ok "its objects are gone too" (isNull (doc().Objects.FindId p) || (doc().Objects.FindId p).IsDeleted)
    throwsRs "unknown layer" (fun () -> rs.PurgeLayer "plMissing")
    )

test "PurgeEmptyLayers" (fun () ->
    rs.AddLayer("peFull::peSub") |> ignore
    rs.ObjectLayer(newPoint (), "peFull::peSub")
    rs.AddLayer("peEmpty1") |> ignore
    rs.AddLayer("peEmpty2::peEmptySub") |> ignore
    let before = rs.CurrentLayer()
    rs.PurgeEmptyLayers()
    ok "empty layer is removed" (not (rs.IsLayer "peEmpty1"))
    ok "empty layer with an empty parent is removed" (not (rs.IsLayer "peEmpty2::peEmptySub") && not (rs.IsLayer "peEmpty2"))
    ok "layer with objects is kept" (rs.IsLayer "peFull::peSub")
    ok "parent of a layer with objects is kept" (rs.IsLayer "peFull")
    eq "the current layer is kept by default" before (rs.CurrentLayer())
    // keepCurrent = false: the current layer is removed if it is empty
    rs.AddLayer("peCur") |> ignore
    rs.CurrentLayer "peCur"
    rs.PurgeEmptyLayers false
    ok "the empty current layer is removed" (not (rs.IsLayer "peCur"))
    ok "layer with objects is still there" (rs.IsLayer "peFull::peSub")
    ok "a layer with objects became the current layer" (rs.CurrentLayer() = "peFull::peSub")
    )

// ------------------------------- hierarchy -------------------------------

test "ParentLayer" (fun () ->
    rs.AddLayer("pnP") |> ignore
    rs.AddLayer("pnC") |> ignore
    eq "no parent gives an empty string" "" (rs.ParentLayer "pnC")
    rs.ParentLayer("pnC", "pnP")
    eq "parent was set" "pnP" (rs.ParentLayer "pnP::pnC")
    ok "layer is now below the parent" (rs.IsLayer "pnP::pnC")
    rs.ParentLayer("pnP::pnC", "")
    eq "empty string removes the parent" "" (rs.ParentLayer "pnC")
    throwsRs "unknown layer" (fun () -> rs.ParentLayer "pnMissing")
    throwsRs "unknown parent" (fun () -> rs.ParentLayer("pnC", "pnMissing"))
    )

test "LayerChildCount" (fun () ->
    rs.AddLayer("ccP::ccA") |> ignore
    rs.AddLayer("ccP::ccB") |> ignore
    rs.AddLayer("ccP::ccA::ccAA") |> ignore
    eq "only immediate children" 2 (rs.LayerChildCount "ccP")
    eq "no children" 0 (rs.LayerChildCount "ccP::ccB")
    throwsRs "unknown layer" (fun () -> rs.LayerChildCount "ccMissing")
    )

test "LayerChildren" (fun () ->
    rs.AddLayer("chP::chA") |> ignore
    rs.AddLayer("chP::chB") |> ignore
    rs.AddLayer("chP::chA::chAA") |> ignore
    eq "only immediate children, full paths" [ "chP::chA"; "chP::chB" ] (rs.LayerChildren "chP" |> Seq.sort |> List.ofSeq)
    eq "no children gives an empty list" 0 (rs.LayerChildren "chP::chB").Count
    throwsRs "unknown layer" (fun () -> rs.LayerChildren "chMissing")
    )

test "LayerChildrenAll" (fun () ->
    rs.AddLayer("caP::caA") |> ignore
    rs.AddLayer("caP::caB") |> ignore
    rs.AddLayer("caP::caA::caAA") |> ignore
    eq "all descendants" [ "caP::caA"; "caP::caA::caAA"; "caP::caB" ] (rs.LayerChildrenAll "caP" |> Seq.sort |> List.ofSeq)
    eq "no children gives an empty list" 0 (rs.LayerChildrenAll "caP::caB").Count
    throwsRs "unknown layer" (fun () -> rs.LayerChildrenAll "caMissing")
    )

test "IsLayerChildOf" (fun () ->
    rs.AddLayer("ioP::ioC::ioG") |> ignore
    ok "child of the parent" (rs.IsLayerChildOf("ioP", "ioP::ioC"))
    ok "grandchild is a child too" (rs.IsLayerChildOf("ioP", "ioP::ioC::ioG"))
    ok "parent is not a child of the child" (not (rs.IsLayerChildOf("ioP::ioC", "ioP")))
    ok "a layer is not its own child" (not (rs.IsLayerChildOf("ioP", "ioP")))
    throwsRs "unknown layer" (fun () -> rs.IsLayerChildOf("ioP", "ioMissing"))
    )

test "IsLayerParentOf" (fun () ->
    rs.AddLayer("ipP::ipC::ipG") |> ignore
    // like in RhinoScript: the second layer is tested for being a parent of the first
    ok "the parent is a parent of the child" (rs.IsLayerParentOf("ipP::ipC", "ipP"))
    ok "the grandparent is a parent too" (rs.IsLayerParentOf("ipP::ipC::ipG", "ipP"))
    ok "the child is not a parent of the parent" (not (rs.IsLayerParentOf("ipP", "ipP::ipC")))
    throwsRs "unknown layer" (fun () -> rs.IsLayerParentOf("ipP", "ipMissing"))
    )

// ------------------------------- states -------------------------------

test "IsLayerEmpty" (fun () ->
    rs.AddLayer("ieA") |> ignore
    ok "new layer is empty" (rs.IsLayerEmpty "ieA")
    rs.ObjectLayer(newPoint (), "ieA")
    ok "layer with an object is not empty" (not (rs.IsLayerEmpty "ieA"))
    throwsRs "unknown layer" (fun () -> rs.IsLayerEmpty "ieMissing")
    )

test "IsLayerReference" (fun () ->
    rs.AddLayer("irA") |> ignore
    ok "normal layer is no reference" (not (rs.IsLayerReference "irA"))
    throwsRs "unknown layer" (fun () -> rs.IsLayerReference "irMissing")
    )

test "IsLayerOn" (fun () ->
    rs.AddLayer("ioA") |> ignore
    ok "new layer is on" (rs.IsLayerOn "ioA")
    rs.LayerOff "ioA"
    ok "off after LayerOff" (not (rs.IsLayerOn "ioA"))
    throwsRs "unknown layer" (fun () -> rs.IsLayerOn "ioMissing")
    )

test "IsLayerVisible" (fun () ->
    rs.AddLayer("ivP::ivC") |> ignore
    ok "visible" (rs.IsLayerVisible "ivP::ivC")
    rs.LayerOff "ivP"
    ok "a child of an invisible parent is invisible" (not (rs.IsLayerVisible "ivP::ivC"))
    throwsRs "unknown layer" (fun () -> rs.IsLayerVisible "ivMissing")
    )

test "LayerVisible" (fun () ->
    rs.AddLayer("lvP::lvC") |> ignore
    ok "visible" (rs.LayerVisible "lvP::lvC")
    rs.LayerVisibleSetFalse "lvP"
    ok "invisible because of the parent" (not (rs.LayerVisible "lvP::lvC"))
    throwsRs "unknown layer" (fun () -> rs.LayerVisible "lvMissing")
    )

test "LayerVisibleSetFalse" (fun () ->
    rs.AddLayer("lfP::lfC") |> ignore
    rs.LayerVisibleSetFalse "lfP::lfC"
    ok "child is invisible" (not (rs.IsLayerVisible "lfP::lfC"))
    ok "parent is still visible" (rs.IsLayerVisible "lfP")
    rs.LayerVisibleSetTrue "lfP::lfC"
    ok "child is visible again" (rs.IsLayerVisible "lfP::lfC")
    // persist: the child stays off when the parent is toggled
    rs.LayerVisibleSetFalse("lfP::lfC", true)
    rs.LayerOff "lfP"
    rs.LayerOn "lfP"
    ok "persistently invisible child stays invisible when the parent comes back" (not (rs.IsLayerVisible "lfP::lfC"))
    rs.LayerVisibleSetFalse "lfP::lfC" // already invisible: no failure
    throwsRs "unknown layer" (fun () -> rs.LayerVisibleSetFalse "lfMissing")
    )

test "LayerVisibleSetTrue" (fun () ->
    rs.AddLayer("ltP::ltC") |> ignore
    rs.LayerOff "ltP"
    ok "child is invisible because of the parent" (not (rs.IsLayerVisible "ltP::ltC"))
    rs.LayerVisibleSetTrue("ltP::ltC", true)
    ok "forceVisible turns the parents on" (rs.IsLayerVisible "ltP" && rs.IsLayerVisible "ltP::ltC")
    rs.LayerOff "ltP"
    rs.LayerVisibleSetTrue("ltP::ltC", false)
    ok "without force the parent stays off" (not (rs.IsLayerVisible "ltP"))
    rs.LayerVisibleSetTrue "ltP::ltC" |> ignore // default is force
    throwsRs "unknown layer" (fun () -> rs.LayerVisibleSetTrue "ltMissing")
    )

test "LayerOff" (fun () ->
    rs.AddLayer("ofP::ofC") |> ignore
    rs.LayerOff "ofP::ofC"
    ok "layer is off" (not (rs.IsLayerOn "ofP::ofC"))
    rs.LayerOff "ofP"
    rs.LayerOff "ofP::ofC" // does nothing, no failure
    ok "parent is off" (not (rs.IsLayerOn "ofP"))
    throwsRs "unknown layer" (fun () -> rs.LayerOff "ofMissing")
    )

test "LayerOn" (fun () ->
    rs.AddLayer("onP::onC") |> ignore
    rs.LayerOff "onP"
    rs.LayerOff "onP::onC"
    rs.LayerOn "onP::onC"
    ok "layer is on" (rs.IsLayerOn "onP::onC")
    ok "parents are on too" (rs.IsLayerOn "onP")
    throwsRs "unknown layer" (fun () -> rs.LayerOn "onMissing")
    )

test "IsLayerLocked" (fun () ->
    rs.AddLayer("ikP::ikC") |> ignore
    ok "new layer is not locked" (not (rs.IsLayerLocked "ikP::ikC"))
    rs.LayerLock "ikP"
    ok "a child of a locked parent is locked" (rs.IsLayerLocked "ikP::ikC")
    throwsRs "unknown layer" (fun () -> rs.IsLayerLocked "ikMissing")
    )

test "LayerLocked" (fun () ->
    rs.AddLayer("lkA") |> ignore
    ok "not locked" (not (rs.LayerLocked "lkA"))
    rs.LayerLock "lkA"
    ok "locked" (rs.LayerLocked "lkA")
    throwsRs "unknown layer" (fun () -> rs.LayerLocked "lkMissing")
    )

test "LayerLockedSetTrue" (fun () ->
    rs.AddLayer("lsP::lsC") |> ignore
    rs.LayerLockedSetTrue "lsP::lsC"
    ok "child is locked" (rs.IsLayerLocked "lsP::lsC")
    ok "parent is not locked" (not (rs.IsLayerLocked "lsP"))
    rs.LayerLockedSetFalse "lsP::lsC"
    ok "child is unlocked" (not (rs.IsLayerLocked "lsP::lsC"))
    // forceLocked: the child stays locked when the parent is unlocked
    rs.LayerLockedSetTrue("lsP::lsC", true)
    rs.LayerLock "lsP"
    rs.LayerLockedSetFalse("lsP", false)
    ok "forced lock stays when the parent is unlocked" (rs.IsLayerLocked "lsP::lsC")
    throwsRs "unknown layer" (fun () -> rs.LayerLockedSetTrue "lsMissing")
    )

test "LayerLockedSetFalse" (fun () ->
    rs.AddLayer("luP::luC") |> ignore
    rs.LayerLock "luP"
    rs.LayerLock "luP::luC"
    rs.LayerLockedSetFalse("luP::luC", false)
    ok "child is unlocked, parent stays locked" (not (rs.IsLayerLocked "luP::luC") || rs.IsLayerLocked "luP")
    rs.LayerLockedSetFalse "luP::luC" // parentsToo is true by default
    ok "parents are unlocked too by default" (not (rs.IsLayerLocked "luP") && not (rs.IsLayerLocked "luP::luC"))
    throwsRs "unknown layer" (fun () -> rs.LayerLockedSetFalse "luMissing")
    )

test "LayerLock" (fun () ->
    rs.AddLayer("lyA") |> ignore
    rs.LayerLock "lyA"
    ok "locked" (rs.IsLayerLocked "lyA")
    rs.LayerLock "lyA" // twice: no failure
    throwsRs "unknown layer" (fun () -> rs.LayerLock "lyMissing")
    )

test "LayerUnlock" (fun () ->
    rs.AddLayer("umP::umC") |> ignore
    rs.LayerLock "umP"
    rs.LayerLock "umP::umC"
    rs.LayerUnlock "umP::umC"
    ok "layer and parents are unlocked" (not (rs.IsLayerLocked "umP::umC") && not (rs.IsLayerLocked "umP"))
    rs.LayerUnlock "umP::umC" // twice: no failure
    throwsRs "unknown layer" (fun () -> rs.LayerUnlock "umMissing")
    )

test "IsLayerChangeable" (fun () ->
    rs.AddLayer("icgA") |> ignore
    ok "visible and unlocked" (rs.IsLayerChangeable "icgA")
    rs.LayerLock "icgA"
    ok "locked is not changeable" (not (rs.IsLayerChangeable "icgA"))
    rs.LayerUnlock "icgA"
    rs.LayerOff "icgA"
    ok "invisible is not changeable" (not (rs.IsLayerChangeable "icgA"))
    throwsRs "unknown layer" (fun () -> rs.IsLayerChangeable "icgMissing")
    )

test "IsLayerSelectable" (fun () ->
    rs.AddLayer("iseA") |> ignore
    ok "visible and unlocked" (rs.IsLayerSelectable "iseA")
    rs.LayerLock "iseA"
    ok "locked is not selectable" (not (rs.IsLayerSelectable "iseA"))
    rs.LayerUnlock "iseA"
    rs.LayerOff "iseA"
    ok "invisible is not selectable" (not (rs.IsLayerSelectable "iseA"))
    throwsRs "unknown layer" (fun () -> rs.IsLayerSelectable "iseMissing")
    )

// ------------------------------- UI state -------------------------------

test "ExpandLayer, IsLayerExpanded, CollapseLayer, UnCollapseLayer, ShowLayer" (fun () ->
    rs.AddLayer("exP::exC::exG") |> ignore
    rs.ExpandLayer("exP", true)
    ok "expanded" (rs.IsLayerExpanded "exP")
    rs.ExpandLayer("exP", false)
    ok "collapsed" (not (rs.IsLayerExpanded "exP"))
    rs.ExpandLayer("exP", false) // no change, no failure
    rs.UnCollapseLayer "exP"
    ok "UnCollapseLayer expands" (rs.IsLayerExpanded "exP")
    rs.CollapseLayer "exP"
    ok "CollapseLayer collapses" (not (rs.IsLayerExpanded "exP"))
    // children too
    rs.UnCollapseLayer("exP", true)
    ok "children expanded too" (rs.IsLayerExpanded "exP" && rs.IsLayerExpanded "exP::exC" && rs.IsLayerExpanded "exP::exC::exG")
    rs.CollapseLayer("exP", true)
    ok "children collapsed too" (not (rs.IsLayerExpanded "exP") && not (rs.IsLayerExpanded "exP::exC") && not (rs.IsLayerExpanded "exP::exC::exG"))
    // ShowLayer expands the parents
    rs.ShowLayer "exP::exC::exG"
    ok "ShowLayer expands the parents" (rs.IsLayerExpanded "exP" && rs.IsLayerExpanded "exP::exC")
    throwsRs "unknown layer, expand" (fun () -> rs.ExpandLayer("exMissing", true))
    throwsRs "unknown layer, is expanded" (fun () -> rs.IsLayerExpanded "exMissing")
    throwsRs "unknown layer, collapse" (fun () -> rs.CollapseLayer "exMissing")
    throwsRs "unknown layer, uncollapse" (fun () -> rs.UnCollapseLayer "exMissing")
    throwsRs "unknown layer, show" (fun () -> rs.ShowLayer "exMissing")
    )

// ------------------------------- properties -------------------------------

test "LayerColor" (fun () ->
    rs.AddLayer("lcoA") |> ignore
    rs.LayerColor("lcoA", red)
    eq "color round trip" (rgb red) (rgb (rs.LayerColor "lcoA"))
    rs.LayerColor("lcoA", green)
    eq "color set again" (rgb green) (rgb (rs.LayerColor "lcoA"))
    throwsRs "unknown layer, get" (fun () -> rs.LayerColor "lcoMissing")
    throwsRs "unknown layer, set" (fun () -> rs.LayerColor("lcoMissing", red))
    )

test "LayerPrintColor" (fun () ->
    rs.AddLayer("lpcA") |> ignore
    rs.LayerPrintColor("lpcA", red)
    eq "print color round trip" (rgb red) (rgb (rs.LayerPrintColor "lpcA"))
    throwsRs "unknown layer, get" (fun () -> rs.LayerPrintColor "lpcMissing")
    throwsRs "unknown layer, set" (fun () -> rs.LayerPrintColor("lpcMissing", red))
    )

test "LayerPrintWidth" (fun () ->
    rs.AddLayer("lpwA") |> ignore
    rs.LayerPrintWidth("lpwA", 0.5)
    near "print width round trip" 0.5 (rs.LayerPrintWidth "lpwA")
    rs.LayerPrintWidth("lpwA", 0.5) // same value: no change
    rs.LayerPrintWidth("lpwA", 0.)
    near "zero width" 0. (rs.LayerPrintWidth "lpwA")
    throwsRs "unknown layer, get" (fun () -> rs.LayerPrintWidth "lpwMissing")
    throwsRs "unknown layer, set" (fun () -> rs.LayerPrintWidth("lpwMissing", 1.))
    )

test "LayerLinetype" (fun () ->
    rs.AddLayer("llA") |> ignore
    eq "default is Continuous" "Continuous" (rs.LayerLinetype "llA")
    let lt = new DocObjects.Linetype(Name = "llDashed")
    lt.AppendSegment(5.0, true) |> ignore
    lt.AppendSegment(2.0, false) |> ignore
    doc().Linetypes.Add lt |> ignore
    rs.LayerLinetype("llA", "llDashed")
    eq "set to a custom linetype" "llDashed" (rs.LayerLinetype "llA")
    rs.LayerLinetype("llA", "CONTINUOUS")
    eq "linetype names are not case sensitive; back to Continuous" "Continuous" (rs.LayerLinetype "llA")
    throwsRs "unknown linetype" (fun () -> rs.LayerLinetype("llA", "llNoSuchLinetype"))
    throwsRs "unknown layer, get" (fun () -> rs.LayerLinetype "llMissing")
    throwsRs "unknown layer, set" (fun () -> rs.LayerLinetype("llMissing", "Continuous"))
    )

test "LayerMaterialIndex" (fun () ->
    rs.AddLayer("lmA") |> ignore
    eq "no material at first" -1 (rs.LayerMaterialIndex "lmA")
    let m = doc().Materials.Add()
    rs.LayerMaterialIndex("lmA", m)
    eq "material index round trip" m (rs.LayerMaterialIndex "lmA")
    rs.LayerMaterialIndex("lmA", -1)
    eq "-1 removes the material" -1 (rs.LayerMaterialIndex "lmA")
    rs.LayerMaterialIndex("lmA", m)
    rs.LayerMaterialIndex("lmA", -5) // values below -1 are ignored
    eq "an index below -1 is ignored" m (rs.LayerMaterialIndex "lmA")
    throwsRs "unknown layer, get" (fun () -> rs.LayerMaterialIndex "lmMissing")
    throwsRs "unknown layer, set" (fun () -> rs.LayerMaterialIndex("lmMissing", 0))
    )

printfn "%s" (finish ())
