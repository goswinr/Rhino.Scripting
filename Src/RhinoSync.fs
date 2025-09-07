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
/// If you are not running from the Fesh Editor, it will use RhinoApp.MainWindow.Invoke.
[<AbstractClass>]
[<Sealed>] //use these attributes to match C# static class and make in visible in C# // https://stackoverflow.com/questions/13101995/defining-static-classes-in-f
type RhinoSync private () =
    static let mutable logErrors = true

    static let mutable feshRhinoSyncModule:Type = null

    static let mutable syncContext: Threading.SynchronizationContext  = null //set via reflection below ; from Fesh.Rhino

    static let mutable feshRhAssembly : Reflection.Assembly = null //set via reflection below ; from Fesh.Rhino

    static let mutable hideEditor = Action(fun ()->()) //set via reflection below ; from Fesh.Rhino

    static let mutable showEditor = Action(fun ()->()) //set via reflection below ; from Fesh.Rhino

    static let mutable isEditorVisible = new Func<bool>(fun () -> false) //set via reflection below ; from Fesh.Rhino

    /// Red green blue text
    static let mutable printFeshLogColor = //changed via reflection below from Fesh.Rhino
        new Action<int,int,int,string>(fun r g b s -> Console.Write s)

    /// Red green blue text
    static let mutable printnFeshLogColor  = //changed via reflection below from Fesh.Rhino
        new Action<int,int,int,string>(fun r g b s -> Console.WriteLine s)

    static let mutable clearFeshLog = Action(fun ()->()) // changed via reflection below from Fesh

    static let mutable prettyFormatters: ResizeArray<obj -> option<string>> = null

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


    static let log msg = Printf.kprintf(fun s -> if logErrors then (RhinoApp.WriteLine s ; eprintfn "%s" s))  msg

    static let mutable initIsPending = true

    //only called when actually needed in DoSync methods below.
    static let initSync() =
        initIsPending <- false
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
            // The code is not used from within Fesh.Rhino plugin
            // let just use AsyncInvoke from Eto.Forms.Application
            // https://pages.picoe.ca/docs/api/html/M_Eto_Forms_Application_AsyncInvoke.htm#!
            syncContext <- Threading.SynchronizationContext.Current

            //
            // // try to get just the sync context from Windows form ( that works on Mac.Mono) (WPF is not anymore referenced by this project)
            // try
            //     RhinoApp.InvokeOnUiThread(new RunOnUiDelegate(fun () -> syncContext <- Windows.Forms.WindowsFormsSynchronizationContext.Current ))
            // with e ->
            //     // TODO better not log anything here ??
            //     log "Rhino.Scripting.dll: Fesh.Rhino.Sync.syncContext failed to init via Windows.Forms.WindowsFormsSynchronizationContext: %O" e


    static let initialize = // Don't rename !!!invoked via reflection from Fesh.Rhino.
        // should be called via reflection from Fesh.Rhino in case Rhino.Scripting is loaded already by another plugin.
        // Reinitialize Rhino.Scripting just in case it is loaded already in the current AppDomain:
        // to have showEditor and hideEditor actions setup correctly.
        new Action(initSync)


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

    /// Set to false to disable the logging off errors to
    /// RhinoApp.WriteLine and the error stream (eprintfn).
    static member LogErrors
        with get() : bool = logErrors
        and  set v : unit = logErrors <- v

    /// The SynchronizationContext of the currently Running Rhino Instance,
    /// This SynchronizationContext is loaded via reflection from the Fesh.Rhino plugin
    static member SyncContext
        with get() : Threading.SynchronizationContext =
            if isNull syncContext then
                initialize.Invoke()  // this is the same as  init()
            syncContext
        and set v : unit =
            syncContext <- v

    /// Hide the WPF Window of currently running Fesh Editor.
    /// Or do nothing if not running in Fesh Editor.
    static member HideEditor() = hideEditor.Invoke() //Action

    /// Show the WPF Window of currently running Fesh Editor.
    /// Or do nothing if not running in Fesh Editor.
    static member ShowEditor() = showEditor.Invoke() //Action

    // The Assembly currently running Fesh Editor Window.
    // Or 'null' if not running in Fesh Editor.
    // static member FeshRhinoAssembly :Reflection.Assembly = feshRhAssembly

    //static member Initialize() =  init() // not called in ActiveDocument module anymore , only called when actually needed in DoSync methods below.


    /// This is a collection of pretty formatters that the libray Pretty is using if present. Needs to be set via reflection !!
    static member PrettyFormatters : ResizeArray<obj -> option<string>> =
        prettyFormatters

    /// Prints in both RhinoApp.WriteLine and Console.WriteLine, or Fesh Editor with color if running.
    /// Red green blue text , NO new line
    static member PrintColor r g b s =
        printFeshLogColor.Invoke(r, g, b, s)
        RhinoApp.Write s
        RhinoApp.Wait()

    /// Prints in both RhinoApp.WriteLine and Console.WriteLine, or Fesh Editor with color  if running.
    /// Red green blue text , with new line
    static member PrintnColor r g b s =
        printnFeshLogColor.Invoke(r, g, b, s)
        RhinoApp.WriteLine s
        RhinoApp.Wait()

    /// Clears the Fesh log window
    /// and the Rhino command history window.
    static member ClearLog() =
        clearFeshLog.Invoke() //Action
        RhinoApp.ClearCommandHistoryWindow()
        RhinoApp.Wait()

    /// Evaluates a function on UI Thread.
    static member DoSync (func:unit->'T) : 'T =
        if RhinoApp.InvokeRequired then
            if initIsPending then initSync()
            if isNull syncContext then
                Eto.Forms.Application.Instance.Invoke func // This should never get called, even when not hosted in Fesh Editor.

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
    static member DoSyncRedraw (func:unit->'T) : 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        if RhinoApp.InvokeRequired then
            if initIsPending then initSync()
            if isNull syncContext then
                Eto.Forms.Application.Instance.Invoke func

                // RhinoSyncException.Raise "This code needs to run on the main UI thread. Rhino.RhinoSync.syncContext is still null or not set up. An automatic context switch is not possible. You are calling a function of Rhino.Scripting that need the UI thread."
                // TODO: better would be to call https://developer.rhino3d.com/api/RhinoCommon/html/M_Rhino_RhinoApp_InvokeOnUiThread.htm and then pass in a continuation function?
                // or somehow get the syncContext from RhinoCode or RhinoCommon or the Rhino .NET host via reflection.
                // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130
            else
                async{
                    do! Async.SwitchToContext syncContext
                    if  not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true
                    let res = func()
                    if  not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
                    return res
                    } |> Async.RunSynchronously
        else
            if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true
            let res = func()
            if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
            res

    /// Evaluates a function on UI Thread.
    /// Also ensures that redraw is enabled and disabled afterwards again if it was disabled initially.
    /// Hides Fesh editor window if it exists. Shows it afterwards again
    static member DoSyncRedrawHideEditor (func:unit->'T) : 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled

        if RhinoApp.InvokeRequired then
            if initIsPending then initSync() // do first
            if isNull syncContext then
                Eto.Forms.Application.Instance.Invoke func

                // RhinoSyncException.Raise "This code needs to run on the main UI thread. Rhino.RhinoSync.syncContext is still null or not set up. An automatic context switch is not possible. You are calling a function of Rhino.Scripting that need the UI thread."
                // TODO: better would be to call https://developer.rhino3d.com/api/RhinoCommon/html/M_Rhino_RhinoApp_InvokeOnUiThread.htm and then pass in a continuation function?
                // or somehow get the syncContext from RhinoCode or RhinoCommon or the Rhino .NET host via reflection.
                // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130

            else
                async{
                    do! Async.SwitchToContext syncContext
                    let isWinVis = isEditorVisible.Invoke() // do after init
                    //eprintfn "Hiding Fesh async..isWinVis:%b" isWinVis
                    if isWinVis then
                        hideEditor.Invoke() //Action
                    if not redraw then
                        RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true
                    RhinoApp.SetFocusToMainWindow() //Action
                    let res = func()
                    if not redraw then
                        RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
                    if isWinVis then
                        showEditor.Invoke() //Action
                    return res
                    } |> Async.RunSynchronously
        else
            if initIsPending then initSync() // because even when we are in sync we still need to see if the Fesh window is showing or not.
            let isWinVis = isEditorVisible.Invoke() // do after init
            //eprintfn "Hiding Fesh sync..isWinVis:%b" isWinVis
            if isWinVis then
                hideEditor.Invoke() //Action
            if not redraw then
                RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true
            let res = func()
            if not redraw then
                RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
            if isWinVis then
                showEditor.Invoke() //Action
            res
