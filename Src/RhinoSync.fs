namespace Rhino.Scripting

open Rhino
open Rhino.Scripting.RhinoScriptingUtils
open System
open Rhino.Runtime

type internal RunOnUiDelegate = delegate of unit -> unit

/// All methods in Rhino.Scripting.dll are thread safe and can be called from any thread.
/// However concurrent writing to Rhino Object Tables might corrupt its state.
/// This class provides a way to run all UI methods on the UI thread.
/// It tries to use the Fesh Editor UI thread if it is running.
/// If you are not running from the Fesh Editor, it will use Eto.Forms.Application.Instance.Invoke.
[<AbstractClass>]
[<Sealed>] //use these attributes to match C# static class and make in visible in C# // https://stackoverflow.com/questions/13101995/defining-static-classes-in-f
type RhinoSync private () =

    static let mutable logErrors =
        true

    static let mutable feshRhinoSyncModule : Type =
        null

    static let mutable syncContext : Threading.SynchronizationContext  =
        null //set via reflection below ; from Fesh.Rhino

    static let mutable feshRhAssembly : Reflection.Assembly =
        null //set via reflection below ; from Fesh.Rhino

    static let mutable hideEditor =
        new Action(fun ()->()) //set via reflection below ; from Fesh.Rhino

    static let mutable showEditor =
        new Action(fun ()->()) //set via reflection below ; from Fesh.Rhino

    static let mutable isEditorVisible =
        new Func<bool>(fun () -> false) //set via reflection below ; from Fesh.Rhino

    /// Red green blue text
    static let mutable printFeshLogColor = //changed via reflection below from Fesh.Rhino
        new Action<int,int,int,string>(fun r g b s -> Console.Write s)

    /// Red green blue text
    static let mutable printnFeshLogColor  = //changed via reflection below from Fesh.Rhino
        new Action<int,int,int,string>(fun r g b s -> Console.WriteLine s)

    static let mutable clearFeshLog =
        new Action(fun ()->()) // changed via reflection below from Fesh

    static let mutable prettyFormatters: ResizeArray<obj -> option<string>> =
        ResizeArray<obj -> option<string>>()

    // Do this one there is a well working Pretty library
    // static let initFeshPrint() =
    //     let allAss = AppDomain.CurrentDomain.GetAssemblies()
    //     match allAss |> Array.tryFind (fun a -> a.GetName().Name = "Pretty") with
    //     | Some prettyAssembly ->
    //         try
    //             let prettySettings = prettyAssembly.GetType "Pretty.PrettySettings"
    //             if notNull prettySettings then
    //                 let formatters = prettySettings.GetProperty("Formatters").GetValue(prettyAssembly) :?> ResizeArray<obj -> option<string>>
    //                 if notNull formatters then
    //                     prettyFormatters <- formatters
    //         with ex ->
    //             eprintfn "An Assembly 'Pretty' was found but setting up color printing failed. The Error was: %A" ex
    //     |None -> ()


    static let log msg =
        Printf.kprintf(fun s ->
            if logErrors then
                RhinoApp.WriteLine s
                eprintfn "%s" s
            )  msg

    [<VolatileField>]
    static let mutable initIsPending = true

    static let initLock = obj()

    // Only called when actually needed, via ensureInit below.
    // Thread safe, initIsPending is only set to false once all fields are set up.
    static let initSync() =
        lock initLock (fun () ->
            if isNull feshRhAssembly then
                // it s ok to log errors here since we check 'if notNull feshRh then'
                try
                    // some reflection hacks because Rhinocommon does not expose a UI sync context
                    // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130/7
                    let feshId = Guid "01dab273-99ae-4760-8695-3f29f4887831" // the GUID of Fesh.Rhino Plugin set in it's AssemblyInfo.fs see https://github.com/goswinr/Fesh.Rhino/blob/main/Src/AssemblyInfo.fs#L15
                    let feshRh = Rhino.PlugIns.PlugIn.Find feshId
                    if notNull feshRh then
                        feshRhAssembly <- feshRh.Assembly
                        feshRhinoSyncModule <- feshRhAssembly.GetType "Fesh.Rhino.Sync"
                with e ->
                    log "Rhino.Scripting.dll could not get feshRhinoSyncModule from Fesh Assembly via Reflection: %A" e

            if notNull feshRhinoSyncModule then
                // it s ok to log errors here since feshRhinoSyncModule is not null and we expect to find those all:
                try hideEditor <- feshRhinoSyncModule.GetProperty("hideEditor").GetValue(feshRhAssembly) :?> Action
                with e -> log "Rhino.Scripting.dll: loading Fesh.Rhino.Sync.hideEditor via reflection failed with: %O" e

                try showEditor <- feshRhinoSyncModule.GetProperty("showEditor").GetValue(feshRhAssembly) :?> Action
                with e -> log "Rhino.Scripting.dll: loading Fesh.Rhino.Sync.showEditor via reflection failed with: %O" e

                try isEditorVisible <- feshRhinoSyncModule.GetProperty("isEditorVisible").GetValue(feshRhAssembly) :?> Func<bool>
                with e -> log "Rhino.Scripting.dll: loading Fesh.Rhino.Sync.isEditorVisible via reflection failed with: %O" e

                try syncContext <- feshRhinoSyncModule.GetProperty("syncContext").GetValue(feshRhAssembly) :?> Threading.SynchronizationContext
                with e -> log "Rhino.Scripting.dll: loading Fesh.Rhino.Sync.syncContext via reflection failed with: %O" e

                try printFeshLogColor <- feshRhinoSyncModule.GetProperty("printFeshLogColor").GetValue(feshRhAssembly) :?> Action<int,int,int,string>
                with e -> log "Rhino.Scripting.dll: loading Fesh.Rhino.Sync.printFeshLogColor via reflection failed with: %O" e

                try printnFeshLogColor <- feshRhinoSyncModule.GetProperty("printnFeshLogColor").GetValue(feshRhAssembly) :?> Action<int,int,int,string>
                with e -> log "Rhino.Scripting.dll: loading Fesh.Rhino.Sync.printnFeshLogColor via reflection failed with: %O" e

                try clearFeshLog <- feshRhinoSyncModule.GetProperty("clearFeshLog").GetValue(feshRhAssembly) :?> Action
                with e -> log "Rhino.Scripting.dll: loading Fesh.Rhino.Sync.clearFeshLog via reflection failed with: %O" e

            else
                // Fesh syncContext not found via reflection,
                // The code is not used from within Fesh.Rhino plugin.
                // Don't use Threading.SynchronizationContext.Current here, this function is usually called from a worker thread,
                // so that would be the worker thread's context (or null), not the one of the UI thread.
                // Leave syncContext as it is (null, or set via the public RhinoSync.SyncContext property),
                // so that Eto.Forms.Application.Instance.Invoke is used in DoSync.
                ()
            initIsPending <- false
            )


    static let initialize = // Don't rename !!!invoked via reflection from Fesh.Rhino.
        // should be called via reflection from Fesh.Rhino in case Rhino.Scripting is loaded already by another plugin.
        // Reinitialize Rhino.Scripting just in case it is loaded already in the current AppDomain:
        // to have showEditor and hideEditor actions setup correctly.
        new Action(initSync)

    // Every member that uses one of the fields set up by initSync must call this first.
    // Until then they hold the defaults: no-op editor actions and uncolored Console printing.
    // Fesh.Rhino calls 'initialize' only once, when its window loads.
    // At that time Rhino.Scripting is usually not loaded yet, scripts load it later with #r.
    static let ensureInit() =
        if initIsPending then initSync()


    // An alternative to do! Async.SwitchToContext syncContext
    //static let runOnUiThread(func:unit->'T)=
    //    if RhinoApp.InvokeRequired then
    //        let mutable isDone = false
    //        let mutable result = Unchecked.defaultof<'T>
    //        let uiDelegate = RunOnUiDelegate (fun () ->
    //            result <- func()
    //            isDone <- true)
    //        RhinoApp.InvokeOnUiThread(uiDelegate, [|  |] ) // does DynamicInvoke on delegate https://stackoverflow.com/questions/12858340/difference-between-invoke-and-dynamicinvoke
    //        while not isDone do
    //            Threading.Thread.Sleep 50
    //        result
    //    else
    //        func()

    // ---------------------------------
    // Public members:
    // ---------------------------------

    /// Set to false to disable the logging of errors to
    /// RhinoApp.WriteLine and the error stream (eprintfn).
    static member LogErrors
        with get() : bool = logErrors
        and  set v : unit = logErrors <- v

    /// The SynchronizationContext of the currently Running Rhino Instance,
    /// This SynchronizationContext is loaded via reflection from the Fesh.Rhino plugin.
    /// It is null if not running in the Fesh Editor and not set here.
    /// Then DoSync uses Eto.Forms.Application.Instance.Invoke instead.
    static member SyncContext
        with get() : Threading.SynchronizationContext =
            if isNull syncContext then
                initialize.Invoke()  // this is the same as  init()
            syncContext
        and set v : unit =
            syncContext <- v

    /// Hide the WPF Window of currently running Fesh Editor.
    /// Or do nothing if not running in Fesh Editor.
    static member HideEditor() =
        ensureInit()
        // A WPF window throws an InvalidOperationException if accessed from another thread than its own,
        // e.g. from a script that Fesh runs asynchronously.
        RhinoSync.DoSync (fun () -> hideEditor.Invoke()) //Action

    /// Show the WPF Window of currently running Fesh Editor.
    /// Or do nothing if not running in Fesh Editor.
    static member ShowEditor() =
        ensureInit()
        // A WPF window throws an InvalidOperationException if accessed from another thread than its own,
        // e.g. from a script that Fesh runs asynchronously.
        RhinoSync.DoSync (fun () -> showEditor.Invoke()) //Action

    // The Assembly currently running Fesh Editor Window.
    // Or 'null' if not running in Fesh Editor.
    // static member FeshRhinoAssembly :Reflection.Assembly = feshRhAssembly

    //static member Initialize() =  init() // not called in ActiveDocument module anymore , only called when actually needed in DoSync methods below.


    /// This is a collection of pretty formatters that the library Pretty is using if present. Needs to be set via reflection.
    static member PrettyFormatters : ResizeArray<obj -> option<string>> =
        prettyFormatters

    /// Prints in both RhinoApp.WriteLine and Console.WriteLine, or Fesh Editor with color if running.
    /// Red green blue text, NO new line
    static member PrintColor r g b s =
        ensureInit() // otherwise the first prints of a script are not colored and do not go to the Fesh log
        printFeshLogColor.Invoke(r, g, b, s)
        RhinoApp.Write s
        RhinoApp.Wait()

    /// Prints in both RhinoApp.WriteLine and Console.WriteLine, or Fesh Editor with color if running.
    /// Red green blue text, with new line
    static member PrintnColor r g b s =
        ensureInit() // otherwise the first prints of a script are not colored and do not go to the Fesh log
        printnFeshLogColor.Invoke(r, g, b, s)
        RhinoApp.WriteLine s
        RhinoApp.Wait()

    /// Clears the Fesh log window
    /// and the Rhino command history window.
    static member ClearLog() =
        ensureInit() // otherwise the Fesh log is not cleared if this is the first call to RhinoSync
        clearFeshLog.Invoke() //Action, safe from any thread, the Fesh log switches to the UI thread itself
        RhinoApp.ClearCommandHistoryWindow()
        RhinoApp.Wait()

    /// Evaluates a function on UI Thread.
    // The explicit <'T> keeps DoSync generic for HideEditor and ShowEditor, which are defined above it.
    static member DoSync<'T> (func:unit->'T) : 'T =
        if RhinoApp.InvokeRequired then
            ensureInit()
            if isNull syncContext then
                // Not hosted in the Fesh Editor, e.g. a script run on a worker thread by Rhino 8 ScriptEditor or by another plugin.
                // Eto's Invoke blocks until func has run on the UI thread.
                Eto.Forms.Application.Instance.Invoke func

                // RhinoSyncException.Raise "%s%s%s%s%s" "This code needs to run on the main UI thread." Environment.NewLine
                //         "Rhino.RhinoSync.syncContext is still null or not set up. An automatic context switch is not possible." Environment.NewLine
                //         "You are calling a function of Rhino.Scripting that need the UI thread."
                // TODO: better would be to call https://developer.rhino3d.com/api/RhinoCommon/html/M_Rhino_RhinoApp_InvokeOnUiThread.htm and then pass in a continuation function?
                // or somehow get the syncContext from RhinoCode or RhinoCommon or the Rhino .NET host via reflection.
                // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130
            else
                async{
                    do! Async.SwitchToContext syncContext
                    return func()
                    } |> Async.RunSynchronously
        else
            func()

    /// Evaluates a function on UI Thread.
    /// Also ensures that redraw is enabled and disabled afterwards again if it was disabled initially.
    /// Redraw is restored even if the function throws an exception.
    static member DoSyncRedraw (func:unit->'T) : 'T =
        RhinoSync.DoSync (fun () ->
            // ActiveDoc is null e.g. on Mac when all documents are closed, then there is no redraw state to restore.
            // State.Doc would raise a clearer error, but State.fs is compiled after this file.
            let doc = RhinoDoc.ActiveDoc
            let views = if isNull doc then null else doc.Views
            let redraw = isNull views || views.RedrawEnabled
            if not redraw then views.RedrawEnabled <- true
            try
                func()
            finally
                if not redraw then views.RedrawEnabled <- false
            )

    /// Evaluates a function on UI Thread.
    /// Also ensures that redraw is enabled and disabled afterwards again if it was disabled initially.
    /// Hides Fesh editor window if it exists. Shows it afterwards again.
    /// Redraw and editor visibility are restored even if the function throws an exception.
    static member DoSyncRedrawHideEditor (func:unit->'T) : 'T =
        ensureInit() // because even when we are on the UI thread we still need to see if the Fesh window is showing or not.
        RhinoSync.DoSync (fun () ->
            let isWinVis = isEditorVisible.Invoke() // do after init
            // ActiveDoc is null e.g. on Mac when all documents are closed, then there is no redraw state to restore.
            // State.Doc would raise a clearer error, but State.fs is compiled after this file.
            let doc = RhinoDoc.ActiveDoc
            let views = if isNull doc then null else doc.Views
            let redraw = isNull views || views.RedrawEnabled
            // Hiding the editor is the first thing inside the try,
            // so that the editor is shown again even if any later step before func throws.
            try
                if isWinVis then
                    hideEditor.Invoke() //Action
                if not redraw then
                    views.RedrawEnabled <- true
                RhinoApp.SetFocusToMainWindow()
                func()
            finally
                // Nested, so that the editor is shown again even if restoring redraw throws,
                // e.g. because func closed the document.
                try
                    if not redraw then
                        views.RedrawEnabled <- false
                finally
                    if isWinVis then
                        showEditor.Invoke() //Action
            )
