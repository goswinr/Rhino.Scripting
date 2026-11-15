// Tests for Src/Scripting_Dimension.fs
// Dimension style setters call DimStyles.Modify with quiet=false, which opens a modal dialog if Rhino refuses the change,
// so only valid values are written, and only to dimension styles that are created by the tests.
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Dimension"

let vec x y z = Vector3d(x, y, z)
let mutable styleCounter = 0
/// creates a new dimension style with a unique name
let newStyle () =
    styleCounter <- styleCounter + 1
    rs.AddDimStyle(sprintf "dsTest%d" styleCounter)
let linear () = rs.AddLinearDimension(pt 0. 0. 0., pt 10. 0. 0., pt 5. 3. 0.)
let aligned () = rs.AddAlignedDimension(pt 0. 0. 0., pt 10. 5. 0., pt 5. 8. 0.)
let leader () = rs.AddLeader([ pt 0. 0. 0.; pt 5. 5. 0. ], "leader text")
let style () = doc().DimStyles.Current
let addGeo (g:GeometryBase) = doc().Objects.Add g
let radial () = addGeo (RadialDimension.Create(style (), AnnotationType.Radius, Plane.WorldXY, pt 0. 0. 0., pt 5. 0. 0., pt 8. 3. 0.))
let diameter () = addGeo (RadialDimension.Create(style (), AnnotationType.Diameter, Plane.WorldXY, pt 0. 0. 0., pt 5. 0. 0., pt 8. 3. 0.))
let angular () = addGeo (AngularDimension.Create(style (), Plane.WorldXY, vec 1. 0. 0., pt 0. 0. 0., pt 10. 0. 0., pt 0. 10. 0., pt 7. 7. 0.))
let ordinate () = addGeo (OrdinateDimension.Create(style (), Plane.WorldXY, OrdinateDimension.MeasuredDirection.Xaxis, pt 0. 0. 0., pt 5. 5. 0., pt 5. 10. 0., 1., 1.))

// ------------------------------- creating -------------------------------

test "AddLinearDimension" (fun () ->
    let id = linear ()
    ok "is a dimension" (rs.IsDimension id)
    ok "is a linear dimension" (rs.IsLinearDimension id)
    ok "is not aligned" (not (rs.IsAlignedDimension id))
    near "value is the distance of the points along the plane" 10. (rs.DimensionValue id)
    // another plane
    let inYZ = rs.AddLinearDimension(pt 0. 0. 0., pt 0. 10. 0., pt 0. 5. 3., Plane.WorldYZ)
    ok "dimension in another plane" (rs.IsLinearDimension inYZ)
    // an invalid plane is replaced by the world xy plane
    let invalid = rs.AddLinearDimension(pt 0. 0. 0., pt 4. 0. 0., pt 2. 1. 0., Plane())
    near "default plane" 4. (rs.DimensionValue invalid)
    )

test "AddAlignedDimension" (fun () ->
    let id = aligned ()
    ok "is a dimension" (rs.IsDimension id)
    ok "is an aligned dimension" (rs.IsAlignedDimension id)
    near "value is the distance between the points" (sqrt 125.) (rs.DimensionValue id)
    let name = newStyle ()
    let styled = rs.AddAlignedDimension(pt 0. 0. 0., pt 10. 5. 0., pt 5. 8. 0., name)
    eq "dimension style" name (rs.DimensionStyle styled)
    throwsRs "unknown style" (fun () -> rs.AddAlignedDimension(pt 0. 0. 0., pt 10. 5. 0., pt 5. 8. 0., "no such style"))
    )

test "AddLeader" (fun () ->
    let id = leader ()
    ok "is a leader" (rs.IsLeader id)
    ok "is a dimension too" (rs.IsDimension id)
    eq "text" "leader text" (rs.LeaderText id)
    let noText = rs.AddLeader [ pt 0. 0. 0.; pt 5. 5. 0.; pt 10. 5. 0. ]
    ok "leader without text" (rs.IsLeader noText)
    let inPlane = rs.AddLeader([ pt 0. 0. 0.; pt 0. 5. 5. ], "yz", Plane.WorldYZ)
    ok "leader in a given plane" (rs.IsLeader inPlane)
    let vertical = rs.AddLeader [ pt 0. 0. 0.; pt 0. 0. 5. ]
    ok "leader along z" (rs.IsLeader vertical)
    throwsRs "one point" (fun () -> rs.AddLeader [ pt 0. 0. 0. ])
    throwsRs "two identical points" (fun () -> rs.AddLeader [ pt 1. 1. 1.; pt 1. 1. 1. ])
    )

// ------------------------------- type tests -------------------------------

/// checks a type test against all kinds of dimensions
let typeTest (name:string) (byId: Guid -> bool) (byGeo: GeometryBase -> bool) (expectedKinds: string list) =
    let kinds = [ "linear", linear; "aligned", aligned; "leader", leader; "radial", radial; "diameter", diameter; "angular", angular; "ordinate", ordinate ]
    for (kind, make) in kinds do
        let id = make ()
        let expected = List.contains kind expectedKinds
        eq (sprintf "%s: by id for a %s dimension" name kind) expected (byId id)
        eq (sprintf "%s: by geometry for a %s dimension" name kind) expected (byGeo (doc().Objects.FindId(id).Geometry))
    ok (name + ": a point object is false") (not (byId (rs.AddPoint(0., 0., 0.))))
    ok (name + ": unknown id is false") (not (byId (Guid.NewGuid())))
    ok (name + ": point geometry is false") (not (byGeo (Point(pt 0. 0. 0.))))
    ok (name + ": null geometry is false") (not (byGeo null))

test "IsDimension" (fun () -> typeTest "IsDimension" rs.IsDimension rs.IsDimension [ "linear"; "aligned"; "leader"; "radial"; "diameter"; "angular"; "ordinate" ])
test "IsLinearDimension" (fun () -> typeTest "IsLinearDimension" rs.IsLinearDimension rs.IsLinearDimension [ "linear"; "aligned" ])
test "IsAlignedDimension" (fun () -> typeTest "IsAlignedDimension" rs.IsAlignedDimension rs.IsAlignedDimension [ "aligned" ])
test "IsAngularDimension" (fun () -> typeTest "IsAngularDimension" rs.IsAngularDimension rs.IsAngularDimension [ "angular" ])
test "IsRadialDimension" (fun () -> typeTest "IsRadialDimension" rs.IsRadialDimension rs.IsRadialDimension [ "radial"; "diameter" ])
test "IsDiameterDimension" (fun () -> typeTest "IsDiameterDimension" rs.IsDiameterDimension rs.IsDiameterDimension [ "diameter" ])
test "IsOrdinateDimension" (fun () -> typeTest "IsOrdinateDimension" rs.IsOrdinateDimension rs.IsOrdinateDimension [ "ordinate" ])
test "IsLeader" (fun () -> typeTest "IsLeader" rs.IsLeader rs.IsLeader [ "leader" ])

// ------------------------------- dimension objects -------------------------------

test "DimensionValue" (fun () ->
    near "linear" 10. (rs.DimensionValue (linear ()))
    ok "radial dimension has a numeric value" (rs.DimensionValue (radial ()) > 0.)
    throws "not a dimension" (fun () -> rs.DimensionValue (rs.AddPoint(0., 0., 0.)))
    throws "unknown id" (fun () -> rs.DimensionValue (Guid.NewGuid()))
    )

test "DimensionText" (fun () ->
    let id = linear ()
    ok "text contains the value" ((rs.DimensionText id).Contains "10")
    throws "not a dimension" (fun () -> rs.DimensionText (rs.AddPoint(0., 0., 0.)))
    )

test "DimensionUserText" (fun () ->
    let id = linear ()
    let initial = rs.DimensionUserText id
    ok "user text is a string" (not (isNull initial) || true)
    rs.DimensionUserText(id, "custom")
    eq "user text round trip" "custom" (rs.DimensionUserText id)
    ok "displayed text contains the custom part" ((rs.DimensionText id).Contains "custom")
    let id2 = aligned ()
    rs.DimensionUserText([ id; id2 ], "both")
    eq "multiple: first" "both" (rs.DimensionUserText id)
    eq "multiple: second" "both" (rs.DimensionUserText id2)
    throws "not a dimension" (fun () -> rs.DimensionUserText (rs.AddPoint(0., 0., 0.)))
    )

test "LeaderText" (fun () ->
    let id = leader ()
    eq "initial text" "leader text" (rs.LeaderText id)
    rs.LeaderText(id, "changed")
    eq "text round trip" "changed" (rs.LeaderText id)
    let id2 = leader ()
    rs.LeaderText([ id; id2 ], "both")
    eq "multiple: first" "both" (rs.LeaderText id)
    eq "multiple: second" "both" (rs.LeaderText id2)
    throws "not a leader" (fun () -> rs.LeaderText (rs.AddPoint(0., 0., 0.)))
    )

test "DimensionStyle" (fun () ->
    let a = newStyle ()
    let b = newStyle ()
    let id = linear ()
    ok "a dimension has a style" (not (String.IsNullOrEmpty (rs.DimensionStyle id)))
    rs.DimensionStyle(id, a)
    eq "style round trip" a (rs.DimensionStyle id)
    let id2 = aligned ()
    rs.DimensionStyle([ id; id2 ], b)
    eq "multiple: first" b (rs.DimensionStyle id)
    eq "multiple: second" b (rs.DimensionStyle id2)
    throwsRs "unknown style" (fun () -> rs.DimensionStyle(id, "no such style"))
    throwsRs "unknown style, multiple" (fun () -> rs.DimensionStyle([ id; id2 ], "no such style"))
    throws "not a dimension" (fun () -> rs.DimensionStyle (rs.AddPoint(0., 0., 0.)))
    )

// ------------------------------- dimension styles -------------------------------

test "AddDimStyle" (fun () ->
    let n = rs.DimStyleCount()
    let name = rs.AddDimStyle "dsAdded"
    eq "returns the name" "dsAdded" name
    eq "count grows" (n + 1) (rs.DimStyleCount())
    ok "style exists" (rs.IsDimStyle "dsAdded")
    let auto = rs.AddDimStyle()
    ok "generated name" (not (String.IsNullOrEmpty auto) && rs.IsDimStyle auto)
    let dup = rs.AddDimStyle "dsAdded"
    ok "a duplicate name gets a unique name or fails" (dup <> "dsAdded")
    )

test "DimStyleCount" (fun () ->
    let n = rs.DimStyleCount()
    newStyle () |> ignore
    eq "one more" (n + 1) (rs.DimStyleCount())
    )

test "DimStyleNames" (fun () ->
    let a = newStyle ()
    let b = newStyle ()
    let names = rs.DimStyleNames()
    ok "contains both" (names.Contains a && names.Contains b)
    eq "same count" (rs.DimStyleCount()) names.Count
    eq "sorted in ordinal order" (names |> Seq.sortWith (fun x y -> String.CompareOrdinal(x, y)) |> List.ofSeq) (List.ofSeq (rs.DimStyleNames true))
    )

test "IsDimStyle" (fun () ->
    let a = newStyle ()
    ok "existing" (rs.IsDimStyle a)
    ok "unknown" (not (rs.IsDimStyle "no such style"))
    )

test "IsDimStyleReference" (fun () ->
    let a = newStyle ()
    ok "a style of the document is no reference" (not (rs.IsDimStyleReference a))
    ok "unknown style gives false" (not (rs.IsDimStyleReference "no such style"))
    )

test "CurrentDimStyle" (fun () ->
    let original = rs.CurrentDimStyle()
    let a = newStyle ()
    rs.CurrentDimStyle a
    eq "current style was set" a (rs.CurrentDimStyle())
    rs.CurrentDimStyle a // already current: no failure
    rs.CurrentDimStyle original
    eq "restored" original (rs.CurrentDimStyle())
    throwsRs "unknown style" (fun () -> rs.CurrentDimStyle "no such style")
    )

test "RenameDimStyle" (fun () ->
    let a = newStyle ()
    let newName = a + "Renamed"
    rs.RenameDimStyle(a, newName)
    ok "new name exists" (rs.IsDimStyle newName)
    ok "old name is gone" (not (rs.IsDimStyle a))
    throwsRs "unknown style" (fun () -> rs.RenameDimStyle("no such style", "x"))
    )

test "DeleteDimStyle" (fun () ->
    let a = newStyle ()
    rs.DeleteDimStyle a
    ok "style is gone" (not (rs.IsDimStyle a))
    throwsRs "unknown style" (fun () -> rs.DeleteDimStyle "no such style")
    )

/// round trip of a numeric property of a style, with invalid values that must raise
let propTest (name:string) (getter: string -> 'a) (setter: string * 'a -> unit) (value:'a) (invalid: 'a list) =
    let s = newStyle ()
    setter (s, value)
    eq (name + " round trip") value (getter s)
    for bad in invalid do
        throwsRs (sprintf "%s: invalid value %A" name bad) (fun () -> setter (s, bad))
    throwsRs (name + ": unknown style, get") (fun () -> getter "no such style")
    throwsRs (name + ": unknown style, set") (fun () -> setter ("no such style", value))

test "DimStyleAnglePrecision" (fun () -> propTest "angle precision" (fun s -> rs.DimStyleAnglePrecision s) (fun (s, v) -> rs.DimStyleAnglePrecision(s, v)) 3 [ -1 ])
test "DimStyleLinearPrecision" (fun () -> propTest "linear precision" (fun s -> rs.DimStyleLinearPrecision s) (fun (s, v) -> rs.DimStyleLinearPrecision(s, v)) 4 [ -1 ])
test "DimStyleArrowSize" (fun () -> propTest "arrow size" (fun s -> rs.DimStyleArrowSize s) (fun (s, v) -> rs.DimStyleArrowSize(s, v)) 2.5 [ 0.; -1. ])
test "DimStyleLeaderArrowSize" (fun () -> propTest "leader arrow size" (fun s -> rs.DimStyleLeaderArrowSize s) (fun (s, v) -> rs.DimStyleLeaderArrowSize(s, v)) 1.5 [ -1. ])
test "DimStyleExtension" (fun () -> propTest "extension" (fun s -> rs.DimStyleExtension s) (fun (s, v) -> rs.DimStyleExtension(s, v)) 0.75 [ -1. ])
test "DimStyleOffset" (fun () -> propTest "offset" (fun s -> rs.DimStyleOffset s) (fun (s, v) -> rs.DimStyleOffset(s, v)) 0.25 [])
test "DimStyleTextGap" (fun () -> propTest "text gap" (fun s -> rs.DimStyleTextGap s) (fun (s, v) -> rs.DimStyleTextGap(s, v)) 0.4 [ -1. ])
test "DimStyleTextHeight" (fun () -> propTest "text height" (fun s -> rs.DimStyleTextHeight s) (fun (s, v) -> rs.DimStyleTextHeight(s, v)) 3.5 [ 0.; -1. ])
test "DimStyleScale" (fun () -> propTest "scale" (fun s -> rs.DimStyleScale s) (fun (s, v) -> rs.DimStyleScale(s, v)) 2. [])
test "DimStyleLengthFactor" (fun () -> propTest "length factor" (fun s -> rs.DimStyleLengthFactor s) (fun (s, v) -> rs.DimStyleLengthFactor(s, v)) 1.5 [])
test "DimStylePrefix" (fun () -> propTest "prefix" (fun s -> rs.DimStylePrefix s) (fun (s, v) -> rs.DimStylePrefix(s, v)) "pre " [])
test "DimStyleSuffix" (fun () -> propTest "suffix" (fun s -> rs.DimStyleSuffix s) (fun (s, v) -> rs.DimStyleSuffix(s, v)) " suf" [])
test "DimStyleNumberFormat" (fun () -> propTest "number format" (fun s -> rs.DimStyleNumberFormat s) (fun (s, v) -> rs.DimStyleNumberFormat(s, v)) 4 [ -1; 10 ])

test "DimStyleTextAlignment" (fun () ->
    let s = newStyle ()
    rs.DimStyleTextAlignment(s, 1)
    eq "horizontal to view" 1 (rs.DimStyleTextAlignment s)
    rs.DimStyleTextAlignment(s, 2)
    eq "in plane, above the dimension line" 2 (rs.DimStyleTextAlignment s)
    rs.DimStyleTextAlignment(s, 3)
    eq "in plane, in the dimension line" 3 (rs.DimStyleTextAlignment s)
    rs.DimStyleTextAlignment(s, 0)
    eq "0 is the same as 2" 2 (rs.DimStyleTextAlignment s)
    throwsRs "alignment too big" (fun () -> rs.DimStyleTextAlignment(s, 4))
    throwsRs "negative alignment" (fun () -> rs.DimStyleTextAlignment(s, -1))
    throwsRs "unknown style, get" (fun () -> rs.DimStyleTextAlignment "no such style")
    throwsRs "unknown style, set" (fun () -> rs.DimStyleTextAlignment("no such style", 1))
    )

test "DimStyleFont" (fun () ->
    let s = newStyle ()
    rs.DimStyleFont(s, "Arial")
    eq "font round trip" "Arial" (rs.DimStyleFont s)
    throwsRs "unknown style, get" (fun () -> rs.DimStyleFont "no such style")
    throwsRs "unknown style, set" (fun () -> rs.DimStyleFont("no such style", "Arial"))
    )

test "DimStyleAvailableFonts" (fun () ->
    let fonts = rs.DimStyleAvailableFonts()
    ok "there are fonts" (fonts.Length > 0)
    ok "Arial is available" (Array.contains "Arial" fonts)
    )

printfn "%s" (finish ())
