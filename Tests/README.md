# Rhino.Scripting test scripts

One F# script per `Src/Scripting_*.fs` file, `Test_<Name>.fsx`. Together they call every public function of `RhinoScriptSyntax` that can run without a person at the keyboard,
check the behavior and edge cases described in the doc strings, and check the Guid overload and the geometry overload where both exist.

The library only works inside a running Rhino, so these are not `dotnet test` tests. They are F# scripts that run inside the Rhino process.

Last full run (Rhino 8.36): 27 suites, 927 passing tests (each test block holds several assertions), 19 failing tests that point at real library bugs, 58 functions skipped on purpose. See [todo.md](todo.md).

## What you need

1. A running Rhino 8 (the scripts were developed on 8.36) with an F# Interactive that lives inside it.
   The tested way is the Caddis MCP server (`caddis_eval`). Fesh.Rhino or any other in-process FSI should also work.
2. A Release build of the library. The harness references it by relative path:

   ```fsx
   #r "../bin/Release/net8.0/Rhino.Scripting.dll"
   ```

   ```bash
   dotnet build ForPublishing.fsproj -c Release
   ```

   Rhino locks that dll while a script session has loaded it. To test a new build, `caddis_reset` (or restart the FSI session) first, then rebuild.
   The tests exercise `Src/` code, so they test the combined `AllScriptingFilesCombinedIntoOne_DontEditThisFile.fs` that the Release build compiles.

## Running

With Caddis, one suite:

```
caddis_eval  path = d:\Git\_Rhino_\Rhino.Scripting\Tests\Test_Surface.fsx
```

Everything in one go (about one minute, use a long `waitMs`):

```
caddis_eval  path = d:\Git\_Rhino_\Rhino.Scripting\Tests\RunAll.fsx   waitMs = 300000
```

`caddis_check path = ...` type-checks a script without running it. It is much faster than a run and is the authoritative check while editing.

Each suite ends by printing a summary like this, followed by the failures and the skipped functions grouped by reason:

```
=== Surface: 84 passed, 3 failed, 0 skipped ===
FAIL AddCone -> Exception: zero radius: expected a RhinoScriptingException but got 00000000-0000-0000-0000-000000000000
```

A failing test is either a bug in the library (see [todo.md](todo.md)) or a wrong test. A test that fails on purpose is never "fixed" by loosening the test.

## How a suite is built

```fsx
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Line"                      // creates and activates a fresh scratch document

test "LineLineIntersection" (fun () ->
    let a = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
    ok "an id came back" (a <> Guid.Empty)
    near "length" 10. (rs.CurveLength a)
    throwsRs "same line twice" (fun () -> ...)
    )

skip "GetPoint" "waits for the user to pick in the viewport"

printfn "%s" (finish ())          // closes the scratch document, returns the summary text
```

[Harness.fsx](Harness.fsx) provides:

| Helper | Meaning |
|---|---|
| `start name`, `finish ()` | Create and activate the scratch document, dispose it and return the summary. `State.Doc` of the library follows the active document. |
| `test name f` | Runs `f`, catches any exception as a failure, then deletes all objects, blocks and groups so the next test starts clean. |
| `skip name reason` | Records a function that is not run. `finish` groups skips by reason. |
| `ok`, `eq`, `near`, `nearTol`, `nearPt`, `nearVec` | Assertions. `near` has a relative tolerance of 1e-6. |
| `throws`, `throwsRs` | Expect any exception / expect a `RhinoScriptingException`. |
| `pt`, `line`, `addLine`, `addSquare`, `addCircle`, `addBox`, `addPlaneSurface`, `addSphere`, `addMeshBox`, `doc ()` | Small geometry helpers and the scratch document. |
| `rs` | Alias for `RhinoScriptSyntax`. |

The user's own document is never touched: everything happens in the scratch document. Side effects outside the document are avoided (see the skipped list), with these exceptions:
the Application suite changes and restores some Rhino options, and the Utility suite briefly uses the clipboard (restored afterwards).

## What is skipped, and why

| Group | Functions |
|---|---|
| Wait for a person in a dialog or the viewport | all `Get*` pick functions, the dialog functions (`MessageBox`, `ListBox`, `OpenFileName`, ...), `GetObjectGrip(s)` |
| Would harm the Rhino session | `Exit`, `SendKeystrokes`, `ClearCommandHistory`, toolbar open/close/save functions |
| Not in the library | `GetSettings`, `SaveSettings` (commented out in the source) |
| Covered elsewhere | `CoerceViewOrActive` (internal, exercised by all view functions) |

## Hazards when writing or running tests

- A function that can open a modal dialog. The UI thread blocks, Caddis cannot cancel a UI thread run, and Rhino still shows as "Responding".
  This happened once, see [Repro_RenameBlockHang.fsx](Repro_RenameBlockHang.fsx) and the recovery steps below. This should be fixed now by setting
  `quiet=true` in `Doc.InstanceDefinitions.Modify`.
- Don't define top-level names such as `box` in a test script: the FSI session persists and the name shadows the core function for everything that runs later. Use `caddis_reset` if it happened.
- Rhino 8 specifics the tests rely on: object enumeration is newest first; `Mesh.CreateFromBox` has 24 unwelded vertices; mesh booleans need non-coplanar boxes with 3 or 8 divisions;
  the default hatch pattern "Dash" is named "HatchDash"; `InterpolatedCurveOnSurfaceUV` returns null in this build.
- F# gotchas: write `f (-1)`, not `f -1`; use arrays for `IList<T>` parameters; give empty lists a type annotation when calling overloaded methods.

## Recovering from a hung Rhino

Symptoms: Rhino is not drawing or reacting, but the task manager says "Responding", and a Caddis run stays "running" forever.
A modal dialog is open somewhere, probably hidden behind the main window.

1. List the windows of the Rhino process:

   ```
   powershell -NoProfile -ExecutionPolicy Bypass -File Tests\CloseRhinoDialog.ps1
   ```

   A dialog shows up as a window with an `Owner` that is the main window, usually class `#32770`.
2. Close it: `... CloseRhinoDialog.ps1 -Close` (every owned `#32770` window) or `... CloseRhinoDialog.ps1 -Hwnd 0x60A22` (one window).
   The script sends `WM_CLOSE`, the same as pressing the window's close button, so the blocked script continues (or fails) and Rhino survives with your unsaved work.
3. If you can't tell which window it is, `dotnet-stack report -p <pid of Rhino>` shows the blocked UI thread; a `MessageBox`/`DialogBox` frame on top of the library call names the culprit.
4. Last resort is killing Rhino, which loses unsaved work.

The `#32770` class and the "owned by the main window" rule are what the one real hang looked like, the script has only been verified in its listing mode since.
