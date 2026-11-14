

// this should be fixed now by setting quiet=true in
//Doc.InstanceDefinitions.Modify(instDef, newName, description, quiet=true)



// Reproduction of the Rhino hang found while writing the tests.
//
// WARNING: running this with `runIt = true` blocks the UI thread of Rhino until the hidden modal dialog is closed.
// Save your work in Rhino first. Do NOT run it through a Caddis 'ui' mode run without a way to close the dialog:
// Caddis cannot cancel a UI thread run. Use the recovery steps in README.md (Tests/CloseRhinoDialog.ps1).
//
// Cause: RenameBlock calls  InstanceDefinitions.Modify(instDef, newName, description, quiet=false)  (Src/Scripting_Block.fs, RenameBlock).
// When newName is already the name of another block definition, RhinoCommon shows a modal message box
// ("block definition name already exists") because quiet=false. The box is owned by the Rhino main window, may be hidden behind it,
// and Rhino still reports "Responding" because the message loop of the box is pumping messages. Nothing returns until it is closed.
// Not named Test_*.fsx on purpose, so a "run everything" glob never picks it up.



#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

/// Flip to true to really run the repro. Left false so that merely loading the file is harmless.
let runIt = false

if runIt then
    start "ReproRenameBlockHang"
    let makeBlock (name:string) =
        let l = rs.AddLine(pt 1. 0. 0., pt 2. 0. 0.)
        let p = rs.AddPoint(0., 1., 0.)
        rs.AddBlock([ l; p ], pt 0. 0. 0., name, true)
    makeBlock "reproA" |> ignore
    makeBlock "reproB" |> ignore
    printfn "calling RenameBlock reproA -> reproB, expect Rhino to freeze behind a hidden modal dialog ..."
    // hangs here until the dialog is closed:
    let result = rs.RenameBlock("reproA", "reproB")
    printfn "RenameBlock returned %b (reached only after the dialog was closed)" result
    printfn "%s" (finish ())
else
    printfn "Repro_RenameBlockHang.fsx: nothing done, set runIt = true to reproduce the hang (see the comment at the top first)."
