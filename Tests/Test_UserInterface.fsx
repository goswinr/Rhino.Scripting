// Tests for Src/Scripting_UserInterface.fs
// Nearly every function of this file shows a dialog or waits for the user in the viewport, they are all listed as skipped.
// Only GetCursorPos, Print (object) and Print (string) run without user input.
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "UserInterface"

test "GetCursorPos" (fun () ->
    let world, screen, viewportId, client = rs.GetCursorPos()
    ok "world point is valid" (world.IsValid)
    ok "screen point is valid" (not (Double.IsNaN screen.X || Double.IsNaN screen.Y))
    ok "client point is valid" (not (Double.IsNaN client.X || Double.IsNaN client.Y))
    ok "the viewport id belongs to a view of the test document" (viewportId = (doc().Views.ActiveView.ActiveViewport.Id))
    )

test "Print" (fun () ->
    // string overload
    rs.Print "Rhino.Scripting test: string"
    // object overloads
    rs.Print 123
    rs.Print [ 1; 2; 3 ]
    rs.Print (pt 1. 2. 3.)
    rs.Print (Guid.NewGuid())
    rs.Print (null : obj)
    rs.Print ""
    // the text arrives on the Rhino command line
    let before = RhinoApp.CommandHistoryWindowText
    rs.Print "Rhino.Scripting print marker 4711"
    ok "the text is in the command history" (RhinoApp.CommandHistoryWindowText.Contains "print marker 4711" || before <> RhinoApp.CommandHistoryWindowText)
    )

skip "BrowseForFolder" "waits for user input in a dialog or in the viewport"
skip "CheckListBox" "waits for user input in a dialog or in the viewport"
skip "ComboListBox" "waits for user input in a dialog or in the viewport"
skip "EditBox" "waits for user input in a dialog or in the viewport"
skip "GetAngle" "waits for user input in a dialog or in the viewport"
skip "GetBoolean" "waits for user input in a dialog or in the viewport"
skip "GetBox" "waits for user input in a dialog or in the viewport"
skip "GetColor" "waits for user input in a dialog or in the viewport"
skip "GetDistance" "waits for user input in a dialog or in the viewport"
skip "GetEdgeCurves" "waits for user input in a dialog or in the viewport"
skip "GetInteger" "waits for user input in a dialog or in the viewport"
skip "GetLayer" "waits for user input in a dialog or in the viewport"
skip "GetLayers" "waits for user input in a dialog or in the viewport"
skip "GetLine" "waits for user input in a dialog or in the viewport"
skip "GetLinetype" "waits for user input in a dialog or in the viewport"
skip "GetMeshFaces" "waits for user input in a dialog or in the viewport"
skip "GetMeshVertices" "waits for user input in a dialog or in the viewport"
skip "GetPoint" "waits for user input in a dialog or in the viewport"
skip "GetPointOnCurve" "waits for user input in a dialog or in the viewport"
skip "GetPointOnMesh" "waits for user input in a dialog or in the viewport"
skip "GetPointOnSurface" "waits for user input in a dialog or in the viewport"
skip "GetPoints" "waits for user input in a dialog or in the viewport"
skip "GetPolyline" "waits for user input in a dialog or in the viewport"
skip "GetReal" "waits for user input in a dialog or in the viewport"
skip "GetRectangle" "waits for user input in a dialog or in the viewport"
skip "GetString" "waits for user input in a dialog or in the viewport"
skip "ListBox" "waits for user input in a dialog or in the viewport"
skip "MessageBox" "waits for user input in a dialog or in the viewport"
skip "PropertyListBox" "waits for user input in a dialog or in the viewport"
skip "MultiListBox" "waits for user input in a dialog or in the viewport"
skip "OpenFileName" "waits for user input in a dialog or in the viewport"
skip "OpenFileNames" "waits for user input in a dialog or in the viewport"
skip "PopupMenu" "waits for user input in a dialog or in the viewport"
skip "RealBox" "waits for user input in a dialog or in the viewport"
skip "SaveFileName" "waits for user input in a dialog or in the viewport"
skip "StringBox" "waits for user input in a dialog or in the viewport"
skip "TextOut" "waits for user input in a dialog or in the viewport"

printfn "%s" (finish ())
