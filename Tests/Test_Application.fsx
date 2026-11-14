// Tests for Src/Scripting_Application.fs
// Application settings belong to the Rhino session, not to the test document.
// Every setter is tested by setting a value, reading it back and restoring the original value.
// Not tested because they would harm the session: Exit, SendKeystrokes, ClearCommandHistory.
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Application"

let rgb (c:Drawing.Color) = (int c.R, int c.G, int c.B)
let tempDir =
    let d = IO.Path.Combine(IO.Path.GetTempPath(), "RhinoScriptingAppTest")
    IO.Directory.CreateDirectory d |> ignore
    d

/// sets a value, checks that it is read back, and restores the original value in any case
let roundTrip (name:string) (getter: unit -> 'a) (setter: 'a -> unit) (testValue: 'a) =
    let original = getter ()
    try
        setter testValue
        eq (name + " round trip") testValue (getter ())
    finally
        setter original

let toggle (name:string) (getter: unit -> bool) (setter: bool -> unit) =
    let original = getter ()
    try
        setter (not original)
        eq (name + " toggled") (not original) (getter ())
        setter original
        eq (name + " toggled back") original (getter ())
    finally
        setter original

// ------------------------------- information -------------------------------

test "BuildDate" (fun () ->
    let d = rs.BuildDate()
    ok "build date is in the past" (d < DateTime.Now.AddDays 1.)
    ok "build date is not ancient" (d.Year >= 2020)
    )
test "ExeFolder" (fun () -> ok "folder exists" (IO.Directory.Exists (rs.ExeFolder())))
test "ExePlatform" (fun () -> eq "64 bit" (if Environment.Is64BitProcess then 1 else 0) (rs.ExePlatform()))
test "ExeServiceRelease" (fun () -> ok "service release is not negative" (rs.ExeServiceRelease() >= 0))
test "ExeVersion" (fun () -> eq "major version of the running Rhino" RhinoApp.ExeVersion (rs.ExeVersion()))
// Rhino 8 returns a negative Int32 here (RhinoCommon's own value), not a 9 digit number YYYYMMDDn. Only the pass through is checked.
test "SdkVersion" (fun () -> eq "like RhinoApp.SdkVersion" RhinoApp.SdkVersion (rs.SdkVersion()))
test "InstallFolder" (fun () -> ok "folder exists" (IO.Directory.Exists (rs.InstallFolder())))
test "IsRunningOnWindows" (fun () -> eq "like the platform" (Runtime.InteropServices.RuntimeInformation.IsOSPlatform Runtime.InteropServices.OSPlatform.Windows) (rs.IsRunningOnWindows()))
test "LocaleID" (fun () -> ok "locale id is positive" (rs.LocaleID() > 0))
test "ScreenSize" (fun () ->
    let w, h = rs.ScreenSize()
    ok "positive width and height" (w > 0 && h > 0)
    )
test "WindowHandle" (fun () -> ok "main window handle is not zero" (rs.WindowHandle() <> IntPtr.Zero))
test "CommandHistory" (fun () -> ok "history is a string" (not (isNull (rs.CommandHistory()))))

// ------------------------------- commands -------------------------------

test "IsCommand" (fun () ->
    ok "Circle is a command" (rs.IsCommand "Circle")
    ok "names are not case sensitive" (rs.IsCommand "circle")
    ok "unknown name" (not (rs.IsCommand "NoSuchCommandXYZ"))
    )

test "Command" (fun () ->
    ok "a valid command succeeds" (rs.Command("_Circle 0,0,0 5", false))
    eq "it created an object" 1 (rs.AllObjects()).Count
    eq "it was a circle" "Circle" (rs.LastCommandName())
    eq "result code 0 means success" 0 (rs.LastCommandResult())
    eq "LastCreatedObjects lists the new object" 1 (rs.LastCreatedObjects()).Count
    ok "echo can be on" (rs.Command("_Circle 0,0,0 6"))
    eq "second circle" 2 (rs.AllObjects()).Count
    ok "an unknown command fails" (not (rs.Command("_NoSuchCommandXYZ", false)))
    )

test "LastCommandName" (fun () ->
    rs.Command("_Circle 0,0,0 5", false) |> ignore
    eq "name of the last command" "Circle" (rs.LastCommandName())
    )

test "LastCommandResult" (fun () ->
    rs.Command("_Circle 0,0,0 5", false) |> ignore
    eq "success" 0 (rs.LastCommandResult())
    )

test "InCommand" (fun () ->
    // no command is running while the script runs, the number of active commands is 0
    eq "number of active commands" 0 (rs.InCommand())
    )

// ------------------------------- aliases -------------------------------

test "AddAlias" (fun () ->
    let n = rs.AliasCount()
    try
        ok "alias is added" (rs.AddAlias("rsTestAlias", "_Line"))
        eq "one more alias" (n + 1) (rs.AliasCount())
        ok "alias exists" (rs.IsAlias "rsTestAlias")
        rs.AddAlias("rsTestAlias", "_Point") |> ignore // adding an existing alias again must not fail
        eq "still only one more alias" (n + 1) (rs.AliasCount())
        ok "alias can not have the name of a command" (not (rs.AddAlias("Circle", "_Point")))
    finally
        rs.DeleteAlias "rsTestAlias" |> ignore
    )

test "AliasNames" (fun () ->
    try
        rs.AddAlias("rsTestAlias2", "_Line") |> ignore
        let names = rs.AliasNames()
        ok "contains the new alias" (Array.contains "rsTestAlias2" names)
        eq "count matches" (rs.AliasCount()) names.Length
    finally
        rs.DeleteAlias "rsTestAlias2" |> ignore
    )

test "AliasCount" (fun () ->
    let n = rs.AliasCount()
    try
        rs.AddAlias("rsTestAlias3", "_Line") |> ignore
        eq "one more" (n + 1) (rs.AliasCount())
    finally
        rs.DeleteAlias "rsTestAlias3" |> ignore
    eq "back to the old count" n (rs.AliasCount())
    )

test "AliasMacro" (fun () ->
    try
        rs.AddAlias("rsTestAlias4", "_Line") |> ignore
        eq "macro of the alias" "_Line" (rs.AliasMacro "rsTestAlias4")
        rs.AliasMacro("rsTestAlias4", "_Circle")
        eq "macro was changed" "_Circle" (rs.AliasMacro "rsTestAlias4")
    finally
        rs.DeleteAlias "rsTestAlias4" |> ignore
    )

test "IsAlias" (fun () ->
    try
        rs.AddAlias("rsTestAlias5", "_Line") |> ignore
        ok "existing" (rs.IsAlias "rsTestAlias5")
    finally
        rs.DeleteAlias "rsTestAlias5" |> ignore
    ok "deleted" (not (rs.IsAlias "rsTestAlias5"))
    ok "unknown" (not (rs.IsAlias "rsNoSuchAlias"))
    )

test "DeleteAlias" (fun () ->
    rs.AddAlias("rsTestAlias6", "_Line") |> ignore
    ok "delete succeeds" (rs.DeleteAlias "rsTestAlias6")
    ok "alias is gone" (not (rs.IsAlias "rsTestAlias6"))
    ok "deleting an unknown alias fails" (not (rs.DeleteAlias "rsTestAlias6"))
    )

// ------------------------------- search paths and files -------------------------------

test "AddSearchPath" (fun () ->
    let n = rs.SearchPathCount()
    let dir = IO.Path.Combine(tempDir, "sp1")
    IO.Directory.CreateDirectory dir |> ignore
    try
        let i = rs.AddSearchPath dir
        ok "inserted at a valid index" (i >= 0 && i <= n)
        eq "one more path" (n + 1) (rs.SearchPathCount())
        ok "the path is in the list" (rs.SearchPathList() |> Array.exists (fun p -> String.Equals(p.TrimEnd('\\'), dir, StringComparison.OrdinalIgnoreCase)))
        let dir2 = IO.Path.Combine(tempDir, "sp2")
        IO.Directory.CreateDirectory dir2 |> ignore
        let i2 = rs.AddSearchPath(dir2, 0)
        eq "insert at the beginning" 0 i2
        rs.DeleteSearchPath dir2 |> ignore
    finally
        rs.DeleteSearchPath dir |> ignore
    eq "back to the old count" n (rs.SearchPathCount())
    )

test "DeleteSearchPath" (fun () ->
    let dir = IO.Path.Combine(tempDir, "sp3")
    IO.Directory.CreateDirectory dir |> ignore
    rs.AddSearchPath dir |> ignore
    ok "delete succeeds" (rs.DeleteSearchPath dir)
    ok "deleting it again fails" (not (rs.DeleteSearchPath dir))
    ok "unknown folder" (not (rs.DeleteSearchPath @"C:\no\such\folder\for\rhino"))
    )

test "SearchPathCount" (fun () -> eq "same as the list" (rs.SearchPathList().Length) (rs.SearchPathCount()))
test "SearchPathList" (fun () -> ok "list is not null" (not (isNull (rs.SearchPathList()))))

test "FindFile" (fun () ->
    let dir = IO.Path.Combine(tempDir, "sp4")
    IO.Directory.CreateDirectory dir |> ignore
    let file = IO.Path.Combine(dir, "rhinoScriptingFindMe.txt")
    IO.File.WriteAllText(file, "x")
    try
        rs.AddSearchPath dir |> ignore
        let found = rs.FindFile "rhinoScriptingFindMe.txt"
        ok "file in a search path is found" (String.Equals(found, file, StringComparison.OrdinalIgnoreCase))
        ok "an unknown file gives an empty string or null" (String.IsNullOrEmpty (rs.FindFile "rhinoScriptingNoSuchFile.xyz"))
    finally
        rs.DeleteSearchPath dir |> ignore
        IO.File.Delete file
    )

test "WorkingFolder" (fun () -> roundTrip "working folder" rs.WorkingFolder (fun f -> rs.WorkingFolder f) tempDir)

test "TemplateFolder" (fun () -> roundTrip "template folder" rs.TemplateFolder (fun f -> rs.TemplateFolder f) tempDir)

test "TemplateFile" (fun () ->
    let file = IO.Path.Combine(tempDir, "rhinoScriptingTemplate.3dm")
    IO.File.WriteAllText(file, "x")
    try roundTrip "template file" rs.TemplateFile (fun f -> rs.TemplateFile f) file
    finally IO.File.Delete file
    )

// ------------------------------- autosave -------------------------------

test "AutosaveFile" (fun () ->
    roundTrip "autosave file" rs.AutosaveFile (fun f -> rs.AutosaveFile f) (IO.Path.Combine(tempDir, "autosave.3dm"))
    )

test "AutosaveInterval" (fun () ->
    roundTrip "autosave interval" rs.AutosaveInterval (fun m -> rs.AutosaveInterval m) 17.
    )

test "EnableAutosave" (fun () ->
    let original = ApplicationSettings.FileSettings.AutoSaveEnabled
    try
        rs.EnableAutosave false
        eq "disabled" false ApplicationSettings.FileSettings.AutoSaveEnabled
        rs.EnableAutosave()
        eq "enabled by default" true ApplicationSettings.FileSettings.AutoSaveEnabled
        rs.EnableAutosave true
        eq "enabled" true ApplicationSettings.FileSettings.AutoSaveEnabled
    finally
        rs.EnableAutosave original
    )

// ------------------------------- appearance and analysis -------------------------------

test "AppearanceColor" (fun () ->
    for item in 0 .. 13 do
        ok (sprintf "item %d can be read" item) (not (rs.AppearanceColor(item).IsEmpty))
    throwsRs "get: item out of range" (fun () -> rs.AppearanceColor 14)
    throwsRs "get: negative item" (fun () -> rs.AppearanceColor -1)
    for item in [ 7; 10 ] do // default layer color and crosshair color, restored right away
        let original = rs.AppearanceColor item
        try
            rs.AppearanceColor(item, Drawing.Color.FromArgb(12, 34, 56))
            eq (sprintf "item %d round trip" item) (12, 34, 56) (rgb (rs.AppearanceColor item))
        finally
            rs.AppearanceColor(item, original)
    throwsRs "set: item out of range" (fun () -> rs.AppearanceColor(14, Drawing.Color.Red))
    )

test "EdgeAnalysisColor" (fun () -> roundTrip "edge analysis color" (fun () -> rgb (rs.EdgeAnalysisColor())) (fun (r, g, b) -> rs.EdgeAnalysisColor(Drawing.Color.FromArgb(r, g, b))) (12, 34, 56))

test "EdgeAnalysisMode" (fun () ->
    let original = rs.EdgeAnalysisMode()
    try
        for mode in [ 0; 1; 2 ] do
            rs.EdgeAnalysisMode mode
            eq (sprintf "mode %d" mode) mode (rs.EdgeAnalysisMode())
        throwsRs "mode out of range" (fun () -> rs.EdgeAnalysisMode 3)
        throwsRs "negative mode" (fun () -> rs.EdgeAnalysisMode -1)
    finally
        rs.EdgeAnalysisMode original
    )

// ------------------------------- modeling aids -------------------------------

test "Ortho" (fun () -> toggle "ortho" rs.Ortho (fun b -> rs.Ortho b))
test "Planar" (fun () -> toggle "planar" rs.Planar (fun b -> rs.Planar b))
test "Snap" (fun () -> toggle "grid snap" rs.Snap (fun b -> rs.Snap b))
test "Osnap" (fun () -> toggle "object snap" rs.Osnap (fun b -> rs.Osnap b))
test "ProjectOsnaps" (fun () -> toggle "project osnaps" rs.ProjectOsnaps (fun b -> rs.ProjectOsnaps b))
test "OsnapDialog" (fun () -> toggle "osnap dialog" rs.OsnapDialog (fun b -> rs.OsnapDialog b))

test "OsnapMode" (fun () ->
    let original = rs.OsnapMode()
    try
        rs.OsnapMode 0
        eq "none" 0 (rs.OsnapMode())
        rs.OsnapMode 2
        eq "near" 2 (rs.OsnapMode())
        rs.OsnapMode (2 + 32 + 64)
        eq "near, center and vertex added together" (2 + 32 + 64) (rs.OsnapMode())
    finally
        rs.OsnapMode original
    )

// ------------------------------- plug-ins -------------------------------

let someLoadedPlugin () = rs.PlugIns(0, 1) |> Array.tryHead

test "PlugIns" (fun () ->
    let all = rs.PlugIns()
    ok "there are plug-ins" (all.Length > 0)
    let loaded = rs.PlugIns(0, 1)
    let unloaded = rs.PlugIns(0, 2)
    ok "loaded plug-ins are a subset of all" (loaded |> Array.forall (fun p -> Array.contains p all))
    ok "unloaded plug-ins are a subset of all" (unloaded |> Array.forall (fun p -> Array.contains p all))
    eq "loaded and unloaded together are all" all.Length (loaded.Length + unloaded.Length)
    let render = rs.PlugIns(1)
    ok "render plug-ins are a subset of all" (render |> Array.forall (fun p -> Array.contains p all))
    ok "there is a render plug-in" (render.Length > 0)
    ok "render plug-ins are fewer than all" (render.Length < all.Length)
    )

test "PlugInId" (fun () ->
    match someLoadedPlugin () with
    | None -> failwith "no loaded plug-in"
    | Some name ->
        let id = rs.PlugInId name
        ok "id is not empty" (id <> Guid.Empty)
        ok "IsPlugIn knows it" (rs.IsPlugIn id)
    throwsRs "unknown plug-in" (fun () -> rs.PlugInId "No Such Plug-In 123")
    )

test "IsPlugIn" (fun () ->
    match someLoadedPlugin () with
    | None -> failwith "no loaded plug-in"
    | Some name ->
        let id = rs.PlugInId name
        ok "by name" (rs.IsPlugIn name)
        ok "by id string" (rs.IsPlugIn (id.ToString()))
        ok "by id" (rs.IsPlugIn id)
    ok "unknown name" (not (rs.IsPlugIn "No Such Plug-In 123"))
    ok "unknown id" (not (rs.IsPlugIn (Guid.NewGuid())))
    ok "empty id" (not (rs.IsPlugIn Guid.Empty))
    )

test "EnablePlugIn" (fun () ->
    match someLoadedPlugin () with
    | None -> failwith "no loaded plug-in"
    | Some name ->
        let id = rs.PlugInId name
        // only the same value is written back, so the plug-in settings of the user stay as they are
        let byName = rs.EnablePlugIn name
        let byId = rs.EnablePlugIn id
        eq "name and id give the same state" byName byId
        rs.EnablePlugIn(name, byName)
        eq "state is unchanged after setting the same value by name" byName (rs.EnablePlugIn name)
        rs.EnablePlugIn(id, byId)
        eq "state is unchanged after setting the same value by id" byId (rs.EnablePlugIn id)
    throws "unknown name, get" (fun () -> rs.EnablePlugIn "No Such Plug-In 123")
    throwsRs "unknown id, get" (fun () -> rs.EnablePlugIn (Guid.NewGuid()))
    throwsRs "unknown id, set" (fun () -> rs.EnablePlugIn(Guid.NewGuid(), true))
    )

test "DefaultRenderer" (fun () ->
    let current = rs.DefaultRenderer()
    ok "a default renderer is set" (not (String.IsNullOrEmpty current))
    ok "setting the same renderer by name succeeds" (rs.DefaultRenderer current)
    eq "unchanged" current (rs.DefaultRenderer())
    ok "setting the same renderer by id succeeds" (rs.DefaultRenderer (rs.PlugInId current))
    eq "unchanged by id" current (rs.DefaultRenderer())
    ok "an unknown renderer can not be set" (not (rs.DefaultRenderer "No Such Renderer 123"))
    ok "an unknown renderer id can not be set" (not (rs.DefaultRenderer (Guid.NewGuid())))
    eq "still unchanged" current (rs.DefaultRenderer())
    )

test "GetPlugInObject" (fun () ->
    ok "unknown plug-in name gives null" (isNull (rs.GetPlugInObject "No Such Plug-In 123"))
    ok "unknown plug-in id gives null" (isNull (rs.GetPlugInObject (Guid.NewGuid())))
    )

// ------------------------------- status bar and prompt -------------------------------

test "Prompt" (fun () ->
    rs.Prompt "test prompt"
    rs.Prompt()
    )

test "StatusBarMessage" (fun () ->
    rs.StatusBarMessage "test message"
    rs.StatusBarMessage()
    )

test "StatusBarDistance" (fun () ->
    rs.StatusBarDistance 12.5
    rs.StatusBarDistance()
    )

test "StatusBarPoint" (fun () ->
    rs.StatusBarPoint(pt 1. 2. 3.)
    rs.StatusBarPoint()
    )

test "StatusBarProgressMeterShow" (fun () ->
    try
        ok "progress meter shown" (rs.StatusBarProgressMeterShow("test", 0, 100))
    finally
        rs.StatusBarProgressMeterHide()
    )

test "StatusBarProgressMeterUpdate" (fun () ->
    try
        rs.StatusBarProgressMeterShow("test", 0, 100, false, false) |> ignore
        rs.StatusBarProgressMeterUpdate 50
        rs.StatusBarProgressMeterUpdate(10, false)
    finally
        rs.StatusBarProgressMeterHide()
    )

test "StatusBarProgressMeterHide" (fun () ->
    rs.StatusBarProgressMeterShow("test", 0, 10) |> ignore
    rs.StatusBarProgressMeterHide()
    rs.StatusBarProgressMeterHide() // hiding twice is fine
    )

test "DisplayOleAlerts" (fun () ->
    rs.DisplayOleAlerts true
    )

skip "Exit" "would close Rhino"
skip "SendKeystrokes" "types into the command line of the user's Rhino"
skip "ClearCommandHistory" "would erase the command history of the user"

try IO.Directory.Delete(tempDir, true) with _ -> ()

printfn "%s" (finish ())
