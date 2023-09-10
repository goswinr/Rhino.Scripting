namespace Rhino.Scripting 

open Rhino 

open FsEx
open System
open Rhino.Runtime
open FsEx.SaveIgnore


type internal RunOnUiDelegate = delegate of unit -> unit

/// All methods in Rhino.Scripting.dll are thread safe and can be called from any thread.
/// However concurrent writing to Rhino Object Tables might corrupt its state.
/// This class provides a way to run all UI methods on the UI thread.
/// It tries to use the Seff Editor UI thread if it is running.
/// If you are not running from the Seff Editor, it will use RhinoApp.MainWindow.Invoke.
[<AbstractClass>]
[<Sealed>] //use these attributes to match C# static class and make in visible in C# // https://stackoverflow.com/questions/13101995/defining-static-classes-in-f
type RhinoSync private () = 

    static let mutable seffRhinoSyncModule:Type = null

    static let mutable syncContext: Threading.SynchronizationContext  = null //set via reflection below ; from Seff.Rhino

    static let mutable seffAssembly : Reflection.Assembly = null //set via reflection below ; from Seff.Rhino

    static let mutable hideEditor : Action = null //set via reflection below ; from Seff.Rhino

    static let mutable showEditor : Action = null //set via reflection below ; from Seff.Rhino
    
    static let mutable isEditorVisible : Func<bool> = null //set via reflection below ; from Seff.Rhino

    static let mutable logErrors = true
    
    static let log msg = Printf.kprintf(fun s ->  if logErrors then (RhinoApp.WriteLine s ; eprintfn "%s" s))  msg       
    
    static let mutable initIsPending = true

    //only called when actually needed in DoSync methods below.
    static let init() =
        initIsPending <- false            
        if isNull seffAssembly then
            // it s ok to log errors here since we check 'if notNull seffRh then' 
            try
                // some reflection hacks because Rhinocommon does not expose a UI sync context
                // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130/7
                let seffId = System.Guid("01dab273-99ae-4760-8695-3f29f4887831") // the GUID of Seff.Rhino Plugin set in it's AssemblyInfo.fs
                let seffRh = Rhino.PlugIns.PlugIn.Find(seffId)
                if notNull seffRh then 
                    seffAssembly <- seffRh.Assembly
                    seffRhinoSyncModule <- seffAssembly.GetType("Seff.Rhino.Sync")
            with e ->
                log "Rhino.Scripting.dll could not get seffRhinoSyncModule from Seff Assembly via Reflection: %A" e
            
        if notNull seffRhinoSyncModule then
            // it s ok to log errors here since seffRhinoSyncModule is not null and we expect to find those all:
            try
                hideEditor <- seffRhinoSyncModule.GetProperty("hideEditor").GetValue(seffAssembly)  :?> Action
                //if isNull hideEditor then
                    //log "Rhino.Scripting.dll: Seff.Rhino.Sync.hideEditor is null" // null is expected when the Seff plugin is loaded but the editor is not running.
            with e ->
                log "Rhino.Scripting.dll: Seff.Rhino.Sync.hideEditor failed with: %O" e                
            
            try
                showEditor <- seffRhinoSyncModule.GetProperty("showEditor").GetValue(seffAssembly) :?> Action
                // if isNull showEditor then
                //     log "Rhino.Scripting.dll: Seff.Rhino.Sync showEditor is null" // null is expected when the Seff plugin is loaded but the editor is not running.
            with e ->
                log "Rhino.Scripting.dll: Seff.Rhino.Sync.showEditor failed with: %O" e
                
            try
                isEditorVisible <- seffRhinoSyncModule.GetProperty("isEditorVisible").GetValue(seffAssembly) :?> Func<bool>
                //if isNull isEditorVisible then
                //    log "Rhino.Scripting.dll: Seff.Rhino.Sync isEditorVisible is null" // null is expected when the Seff plugin is loaded but the editor is not running.
            with e ->
                log "Rhino.Scripting.dll: Seff.Rhino.Sync.isEditorVisible failed with: %O" e
            
            try
                syncContext <- seffRhinoSyncModule.GetProperty("syncContext").GetValue(seffAssembly) :?> Threading.SynchronizationContext
                //if isNull syncContext then
                    //log "Rhino.Scripting.dll: Seff.Rhino.Sync.syncContext is null"// null is expected when the Seff plugin is loaded but the editor is not running.
                    
            with e ->
                log "Rhino.Scripting.dll: Seff.Rhino.Sync.syncContext failed with: %O" e 
        
        else
            // try to get just the sync context from Windows form ( that works on Mac.Mono) (WPF is not anymore referenced by this project)
            try
                RhinoApp.InvokeOnUiThread(new RunOnUiDelegate(fun () -> syncContext <- Windows.Forms.WindowsFormsSynchronizationContext.Current ))
            with e -> 
                // TODO better not log anything here ??
                log "Rhino.Scripting.dll: Seff.Rhino.Sync.syncContext failed to init via Windows.Forms.WindowsFormsSynchronizationContext: %O" e

                    
    static let initialize = // Don't rename !!!invoked via reflection from Seff.Rhino.
        // should be called via reflection from Seff.Rhino in case Rhino.Scripting is loaded already by another plugin.
        // Reinitialize Rhino.Scripting just in case it is loaded already in the current AppDomain:
        // to have showEditor and hideEditor actions setup correctly.
        new Action(init) 


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
        with get() = logErrors
        and  set v = logErrors <- v

    /// The SynchronizationContext of the currently Running Rhino Instance,
    /// This SynchronizationContext is loaded via reflection from the Seff.Rhino plugin
    static member SyncContext  
        with get() = 
            if isNull syncContext then init()
            syncContext
        and set v = 
            syncContext <- v
        
    /// Hide the WPF Window of currently running Seff Editor.
    /// Or do nothing if not running in Seff Editor.
    static member HideEditor() = if notNull hideEditor then hideEditor.Invoke()
    
    /// Show the WPF Window of currently running Seff Editor.
    /// Or do nothing if not running in Seff Editor.
    static member ShowEditor() = if notNull showEditor then showEditor.Invoke()

    /// The Assembly currently running Seff Editor Window.
    /// Or 'null' if not running in Seff Editor.
    static member SeffRhinoAssembly = seffAssembly
        
    //static member Initialize() =  init() // not called in ActiveDocument module anymore , only called when actaully neded in DoSync methods below.

    /// Evaluates a function on UI Thread.
    static member DoSync (func:unit->'T) : 'T = 
        if RhinoApp.InvokeRequired then            
            if initIsPending then init()
            if isNull syncContext then 
                RhinoSyncException.Raise "%s\r\n%s\r\n%s" "This code needs to run on the main UI thread." 
                        "Rhino.RhinoSync.syncContext is still null or not set up. An automatic context switch is not possible."
                        "You are calling a function of Rhino.Scripting that need the UI thread."
                // TODO: better would be to call https://developer.rhino3d.com/api/RhinoCommon/html/M_Rhino_RhinoApp_InvokeOnUiThread.htm and then pass in a continuation function?
                // or somehow get the syncContext from RhinoCode or RhinoCommon or the Rhino .NET host via reflection.
                // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130
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
            if initIsPending then init()
            if isNull syncContext then 
                RhinoSyncException.Raise "This code needs to run on the main UI thread. Rhino.RhinoSync.syncContext is still null or not set up. An automatic context switch is not possible. You are calling a function of Rhino.Scripting that need the UI thread."
                // TODO: better would be to call https://developer.rhino3d.com/api/RhinoCommon/html/M_Rhino_RhinoApp_InvokeOnUiThread.htm and then pass in a continuation function?
                // or somehow get the syncContext from RhinoCode or RhinoCommon or the Rhino .NET host via reflection.
                // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130
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
    /// Hides Seff editor window if it exists. Shows it afterwards again
    static member DoSyncRedrawHideEditor (func:unit->'T) : 'T = 
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        
        if RhinoApp.InvokeRequired then
            if initIsPending then init() // do first
            if isNull syncContext then 
                RhinoSyncException.Raise "This code needs to run on the main UI thread. Rhino.RhinoSync.syncContext is still null or not set up. An automatic context switch is not possible. You are calling a function of Rhino.Scripting that need the UI thread."
                // TODO: better would be to call https://developer.rhino3d.com/api/RhinoCommon/html/M_Rhino_RhinoApp_InvokeOnUiThread.htm and then pass in a continuation function?
                // or somehow get the syncContext from RhinoCode or RhinoCommon or the Rhino .NET host via reflection.
                // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130           
            async{                    
                do! Async.SwitchToContext syncContext
                let isWinVis = if isNull isEditorVisible then false else isEditorVisible.Invoke() // do after init                    
                //eprintfn "Hiding Seff async..isWinVis:%b" isWinVis
                if isWinVis && notNull hideEditor then hideEditor.Invoke()
                if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true         
                RhinoApp.SetFocusToMainWindow()
                let res = func()
                if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
                if isWinVis && notNull showEditor then showEditor.Invoke()
                return res
                } |> Async.RunSynchronously
        else
            if initIsPending then init() // because even when we are in sync we still need to see if the Seff window is showing or not.
            let isWinVis = if isNull isEditorVisible then false else isEditorVisible.Invoke() // do after init   
            //eprintfn "Hiding Seff sync..isWinVis:%b" isWinVis            
            if isWinVis && notNull hideEditor then hideEditor.Invoke()
            if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true            
            let res = func()
            if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
            if isWinVis && notNull showEditor then showEditor.Invoke()
            res
