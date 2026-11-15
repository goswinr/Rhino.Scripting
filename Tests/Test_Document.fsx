// Tests for Src/Scripting_Document.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Document"

let red = Drawing.Color.FromArgb(255, 0, 0)
let rgb (c:Drawing.Color) = (int c.R, int c.G, int c.B)
let tmp name = IO.Path.Combine(IO.Path.GetTempPath(), name)

// ------------------------------- file and state -------------------------------

test "DocumentName" (fun () -> eq "unsaved document has an empty name" "" (rs.DocumentName()))
test "DocumentPath" (fun () -> eq "unsaved document has an empty path" "" (rs.DocumentPath()))

test "DocumentModified" (fun () ->
    rs.DocumentModified false
    ok "set to false" (not (rs.DocumentModified()))
    rs.DocumentModified true
    ok "set to true" (rs.DocumentModified())
    rs.DocumentModified false
    )

test "IsDocumentModified" (fun () ->
    rs.DocumentModified false
    ok "not modified" (not (rs.IsDocumentModified()))
    rs.DocumentModified true
    ok "modified" (rs.IsDocumentModified())
    rs.DocumentModified false
    )

test "Notes" (fun () ->
    eq "unset notes" "" (rs.Notes())
    rs.Notes "first line\r\nsecond line"
    eq "notes round trip" "first line\r\nsecond line" (rs.Notes())
    rs.Notes ""
    eq "empty notes" "" (rs.Notes())
    )

test "ReadFileVersion" (fun () ->
    // a document that was not read from a file has no file version (-1 or 0), it must not fail
    let v = rs.ReadFileVersion()
    ok (sprintf "returns a number (%d)" v) (v >= -1)
    )

test "EnableRedraw" (fun () ->
    rs.DisableRedraw()
    ok "redraw is off" (not (doc().Views.RedrawEnabled))
    rs.EnableRedraw()
    ok "redraw is on by default" (doc().Views.RedrawEnabled)
    rs.EnableRedraw false
    ok "redraw is off" (not (doc().Views.RedrawEnabled))
    rs.EnableRedraw true
    ok "redraw is on" (doc().Views.RedrawEnabled)
    )

test "DisableRedraw" (fun () ->
    rs.DisableRedraw()
    ok "redraw is off" (not (doc().Views.RedrawEnabled))
    rs.EnableRedraw()
    )

test "Redraw" (fun () ->
    rs.DisableRedraw()
    rs.Redraw()
    ok "redraw state is restored after Redraw" (not (doc().Views.RedrawEnabled))
    rs.EnableRedraw()
    rs.Redraw()
    ok "redraw state is on after Redraw" (doc().Views.RedrawEnabled)
    )

test "CreatePreviewImage" (fun () ->
    rs.AddPoint(0., 0., 0.) |> ignore
    let file = tmp "RhinoScriptingPreviewTest.png"
    let size (path:string) =
        use bmp = new Drawing.Bitmap(new IO.MemoryStream(IO.File.ReadAllBytes path))
        bmp.Width, bmp.Height
    try
        ok "wireframe image is created" (rs.CreatePreviewImage(file, "Top", 100, 80, 0, true))
        ok "file exists" (IO.File.Exists file)
        eq "image size" (100, 80) (size file)
        IO.File.Delete file
        ok "shaded image with the width only" (rs.CreatePreviewImage(file, "Top", 120))
        let w, h = size file
        eq "width" 120 w
        ok "height is scaled to keep the screen ratio" (h > 0)
    finally
        try IO.File.Delete file with _ -> ()
    throws "unknown view" (fun () -> rs.CreatePreviewImage(file, "no such view", 10, 10))
    )

test "ExtractPreviewImage" (fun () ->
    throwsRs "an unsaved document has no preview" (fun () -> rs.ExtractPreviewImage(tmp "RhinoScriptingExtract.png"))
    throws "model file does not exist" (fun () -> rs.ExtractPreviewImage(tmp "RhinoScriptingExtract.png", @"C:\no\such\model.3dm"))
    )

// ------------------------------- render settings -------------------------------

test "RenderAntialias" (fun () ->
    for style in [ 0; 1; 2 ] do
        rs.RenderAntialias style
        eq (sprintf "antialias %d" style) style (rs.RenderAntialias())
    rs.RenderAntialias 1
    rs.RenderAntialias 7 // out of range values are ignored
    eq "out of range value is ignored" 1 (rs.RenderAntialias())
    )

test "RenderColor" (fun () ->
    rs.RenderColor(0, red)
    eq "ambient light" (rgb red) (rgb (rs.RenderColor 0))
    rs.RenderColor(1, Drawing.Color.FromArgb(0, 0, 255))
    eq "background" (0, 0, 255) (rgb (rs.RenderColor 1))
    throwsRs "bad item, get" (fun () -> rs.RenderColor 2)
    throwsRs "bad item, set" (fun () -> rs.RenderColor(2, red))
    throwsRs "negative item" (fun () -> rs.RenderColor(-1, red))
    )

test "RenderResolution" (fun () ->
    rs.RenderResolution(640, 480)
    eq "resolution round trip" (640, 480) (rs.RenderResolution())
    rs.RenderResolution(1000, 20)
    eq "another resolution" (1000, 20) (rs.RenderResolution())
    )

test "RenderSettings" (fun () ->
    rs.RenderSettings 0
    eq "all off" 0 (rs.RenderSettings())
    rs.RenderSettings 14
    eq "hidden lights, curves and annotations" 14 (rs.RenderSettings())
    rs.RenderSettings 1
    ok "shadows flag is stored" ((rs.RenderSettings()) &&& 1 = 1)
    rs.RenderSettings 0
    )

/// round trip test of a float meshing parameter
let meshFloat (name:string) (getter: unit -> float) (setter: float -> unit) (value:float) (bad:float option) =
    setter value
    near (name + " round trip") value (getter ())
    eq (name + " sets the mesh quality to custom (2)") 2 (rs.RenderMeshQuality())
    match bad with
    | Some b -> throwsRs (name + " bad value") (fun () -> setter b)
    | None -> ()

test "RenderMeshDensity" (fun () ->
    meshFloat "density" rs.RenderMeshDensity rs.RenderMeshDensity 0.5 None
    rs.RenderMeshDensity 0.3
    rs.RenderMeshDensity 1.5 // out of 0..1 is ignored
    near "out of range value is ignored" 0.3 (rs.RenderMeshDensity())
    rs.RenderMeshDensity -0.1
    near "negative value is ignored" 0.3 (rs.RenderMeshDensity())
    )
test "RenderMeshMaxAngle" (fun () ->
    meshFloat "max angle" rs.RenderMeshMaxAngle rs.RenderMeshMaxAngle 25. (Some -1.)
    rs.RenderMeshMaxAngle 0.
    near "0 disables the constraint" 0. (rs.RenderMeshMaxAngle())
    )
test "RenderMeshMaxAspectRatio" (fun () -> meshFloat "aspect ratio" rs.RenderMeshMaxAspectRatio rs.RenderMeshMaxAspectRatio 6. (Some -1.))
test "RenderMeshMaxDistEdgeToSrf" (fun () -> meshFloat "distance edge to surface" rs.RenderMeshMaxDistEdgeToSrf rs.RenderMeshMaxDistEdgeToSrf 0.25 (Some -1.))
test "RenderMeshMaxEdgeLength" (fun () -> meshFloat "max edge length" rs.RenderMeshMaxEdgeLength rs.RenderMeshMaxEdgeLength 12. (Some -1.))
test "RenderMeshMinEdgeLength" (fun () -> meshFloat "min edge length" rs.RenderMeshMinEdgeLength rs.RenderMeshMinEdgeLength 0.5 (Some -1.))

test "RenderMeshMinInitialGridQuads" (fun () ->
    rs.RenderMeshMinInitialGridQuads 16
    eq "quads round trip" 16 (rs.RenderMeshMinInitialGridQuads())
    eq "mesh quality is custom" 2 (rs.RenderMeshQuality())
    throwsRs "negative" (fun () -> rs.RenderMeshMinInitialGridQuads -1)
    )

test "RenderMeshQuality" (fun () ->
    rs.RenderMeshQuality 0
    eq "fast" 0 (rs.RenderMeshQuality())
    rs.RenderMeshQuality 1
    eq "quality" 1 (rs.RenderMeshQuality())
    rs.RenderMeshQuality 2
    eq "custom" 2 (rs.RenderMeshQuality())
    throwsRs "bad value" (fun () -> rs.RenderMeshQuality 3)
    throwsRs "negative value" (fun () -> rs.RenderMeshQuality -1)
    )

test "RenderMeshSettings" (fun () ->
    rs.RenderMeshSettings 0
    eq "all off" 0 (rs.RenderMeshSettings())
    rs.RenderMeshSettings 15
    eq "all flags" 15 (rs.RenderMeshSettings())
    rs.RenderMeshSettings 5
    eq "refine grid and simple planes" 5 (rs.RenderMeshSettings())
    eq "mesh quality is custom" 2 (rs.RenderMeshQuality())
    )

// ------------------------------- units and tolerances -------------------------------

test "UnitAbsoluteTolerance" (fun () ->
    rs.UnitAbsoluteTolerance 0.05
    near "model tolerance round trip" 0.05 (rs.UnitAbsoluteTolerance())
    near "same through the model selector" 0.05 (rs.UnitAbsoluteTolerance true)
    throwsRs "zero" (fun () -> rs.UnitAbsoluteTolerance 0.)
    throwsRs "negative" (fun () -> rs.UnitAbsoluteTolerance -1.)
    rs.UnitAbsoluteTolerance(0.2, false)
    near "page tolerance round trip" 0.2 (rs.UnitAbsoluteTolerance false)
    near "model tolerance is unchanged" 0.05 (rs.UnitAbsoluteTolerance())
    rs.UnitAbsoluteTolerance(0.07, true)
    near "model selector in the setter" 0.07 (rs.UnitAbsoluteTolerance())
    throwsRs "page, zero" (fun () -> rs.UnitAbsoluteTolerance(0., false))
    )

test "UnitAngleTolerance" (fun () ->
    rs.UnitAngleTolerance 2.5
    near "model angle tolerance round trip" 2.5 (rs.UnitAngleTolerance())
    throwsRs "zero" (fun () -> rs.UnitAngleTolerance 0.)
    throwsRs "negative" (fun () -> rs.UnitAngleTolerance -1.)
    rs.UnitAngleTolerance(1.5, false)
    near "page angle tolerance round trip" 1.5 (rs.UnitAngleTolerance false)
    near "model angle tolerance is unchanged" 2.5 (rs.UnitAngleTolerance true)
    rs.UnitAngleTolerance(3.5, true)
    near "model selector in the setter" 3.5 (rs.UnitAngleTolerance())
    throwsRs "page, zero" (fun () -> rs.UnitAngleTolerance(0., false))
    )

test "UnitRelativeTolerance" (fun () ->
    rs.UnitRelativeTolerance 0.5
    near "model relative tolerance round trip" 0.5 (rs.UnitRelativeTolerance())
    throwsRs "zero" (fun () -> rs.UnitRelativeTolerance 0.)
    throwsRs "negative" (fun () -> rs.UnitRelativeTolerance -1.)
    rs.UnitRelativeTolerance(0.7, false)
    near "page relative tolerance round trip" 0.7 (rs.UnitRelativeTolerance false)
    near "model relative tolerance is unchanged" 0.5 (rs.UnitRelativeTolerance true)
    rs.UnitRelativeTolerance(0.9, true)
    near "model selector in the setter" 0.9 (rs.UnitRelativeTolerance())
    throwsRs "page, zero" (fun () -> rs.UnitRelativeTolerance(0., false))
    )

test "UnitDistanceDisplayPrecision" (fun () ->
    rs.UnitDistanceDisplayPrecision 4
    eq "model precision round trip" 4 (rs.UnitDistanceDisplayPrecision())
    rs.UnitDistanceDisplayPrecision(6, false)
    eq "page precision round trip" 6 (rs.UnitDistanceDisplayPrecision false)
    eq "model precision is unchanged" 4 (rs.UnitDistanceDisplayPrecision true)
    rs.UnitDistanceDisplayPrecision(3, true)
    eq "model selector in the setter" 3 (rs.UnitDistanceDisplayPrecision())
    )

test "UnitSystem" (fun () ->
    let original = rs.UnitSystem()
    rs.UnitSystem 2
    eq "millimeters (2)" 2 (rs.UnitSystem())
    rs.UnitSystem 4
    eq "meters (4)" 4 (rs.UnitSystem())
    rs.UnitSystem(UnitSystem.Feet)
    eq "enum overload: feet (9)" 9 (rs.UnitSystem())
    throwsRs "too big" (fun () -> rs.UnitSystem 26)
    throwsRs "negative" (fun () -> rs.UnitSystem -1)
    // scale existing geometry
    rs.UnitSystem 2
    let p = rs.AddPoint(1000., 0., 0.)
    rs.UnitSystem(4, true)
    near "geometry is scaled when scale=true" 1. (rs.PointCoordinates(p)).X
    rs.UnitSystem(2, false)
    near "geometry is not scaled when scale=false" 1. (rs.PointCoordinates(p)).X
    // model and page selector
    rs.UnitSystem(3, false, false)
    eq "page unit system round trip" 3 (rs.UnitSystem false)
    eq "model unit system is unchanged" 2 (rs.UnitSystem true)
    rs.UnitSystem(UnitSystem.Meters, false, false)
    eq "page unit system with enum" 4 (rs.UnitSystem false)
    rs.UnitSystem(8, false, true)
    eq "model selector in the setter" 8 (rs.UnitSystem())
    rs.UnitSystem(UnitSystem.Millimeters, false, true)
    eq "model selector with enum in the setter" 2 (rs.UnitSystem())
    throwsRs "page, too big" (fun () -> rs.UnitSystem(26, false, false))
    throwsRs "page, negative" (fun () -> rs.UnitSystem(-1, false, false))
    rs.UnitSystem original
    )

test "UnitSystemName" (fun () ->
    rs.UnitSystem 2
    eq "default: lower case, singular" "millimeter" (rs.UnitSystemName())
    eq "capitalized" "Millimeter" (rs.UnitSystemName(true))
    eq "plural" "millimeters" (rs.UnitSystemName(false, false))
    eq "abbreviated" "mm" (rs.UnitSystemName(false, true, true))
    rs.UnitSystem(8, false, false)
    eq "page units: inches" "inch" (rs.UnitSystemName(false, true, false, false))
    eq "model units are selected by default" "millimeter" (rs.UnitSystemName())
    )

test "UnitScale" (fun () ->
    near "millimeters to meters (int)" 0.001 (rs.UnitScale(4, 2))
    near "meters to millimeters (int)" 1000. (rs.UnitScale(2, 4))
    near "same system" 1. (rs.UnitScale(2, 2))
    near "inches to millimeters (int)" 25.4 (rs.UnitScale(2, 8))
    near "enum overload" 0.001 (rs.UnitScale(UnitSystem.Meters, UnitSystem.Millimeters))
    near "enum and int overload" 0.001 (rs.UnitScale(UnitSystem.Meters, 2))
    near "int and enum overload" 0.001 (rs.UnitScale(4, UnitSystem.Millimeters))
    // only the destination: the model units are the source
    rs.UnitSystem 2
    near "destination only (int)" 0.001 (rs.UnitScale 4)
    near "destination only (enum)" 0.001 (rs.UnitScale UnitSystem.Meters)
    rs.UnitSystem 4
    near "destination only follows the model units" 1000. (rs.UnitScale 2)
    rs.UnitSystem 2
    )

skip "CreatePreviewImage (flags)" "covered by the shaded and wireframe cases above"

printfn "%s" (finish ())
