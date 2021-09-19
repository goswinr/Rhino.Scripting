namespace Rhino

open FsEx
open System
open Rhino.Runtime
open FsEx.SaveIgnore

/// A static classs to help access the UI thread of Rhino from other threads
[<AbstractClass>]
[<Sealed>] //use these attributes to match C# static class and make in visible in C# // https://stackoverflow.com/questions/13101995/defining-static-classes-in-f   
type RhinoSync private () = 
    
    static let mutable seffRhinoSyncModule:Type = null

    static let mutable syncContext: Threading.SynchronizationContext  = null //set via reflection below ; from Seff.Rhino    
   
    static let mutable seffAssembly : Reflection.Assembly = null //set via reflection below ; from Seff.Rhino    
    
    static let mutable seffWindow : System.Windows.Window = null //set via reflection below ; from Seff.Rhino
        
    static let log msg = Printf.kprintf(fun s -> RhinoApp.WriteLine s; eprintfn "%s" s) msg  
    
    static let mutable initIsPending = true

    static let init() =
        if initIsPending then 
            if not HostUtils.RunningInRhino || not HostUtils.RunningOnWindows then 
                log "RhinoSync.init() not done because: HostUtils.RunningInRhino %b or HostUtils.RunningOnWindows: %b" HostUtils.RunningInRhino  HostUtils.RunningOnWindows
            else 
                if isNull seffAssembly then
                    try
                        // some reflection hacks because Rhinocommon does not expose a UI sync context
                        // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130/7 
                        let seffId = System.Guid("01dab273-99ae-4760-8695-3f29f4887831") // the GUID of Seff.Rhino Plugin set in it's AssemblyInfo.fs
                        let seffRh = Rhino.PlugIns.PlugIn.Find(seffId)
                        seffAssembly <- seffRh.Assembly
                        seffRhinoSyncModule <- seffAssembly.GetType("Seff.Rhino.Sync")
                    with e ->
                        log "Rhino.Scripting.dll did not find Seff Assembly via Reflection, If you are not using the Seff Editor Plugin this is normal."
                
                if isNull seffWindow && notNull seffRhinoSyncModule then 
                    try   
                        seffWindow <- seffRhinoSyncModule.GetProperty("window").GetValue(seffAssembly)  :?> System.Windows.Window
                        if isNull seffWindow then 
                            log "**Seff.Rhino.Sync.window is null"
                        
                    with e ->
                        log "**Seff.Rhino.Sync.window failed with: %A" e
                
                if isNull syncContext then 
                    try
                        Windows.Threading.DispatcherSynchronizationContext(Windows.Application.Current.Dispatcher) |> Threading.SynchronizationContext.SetSynchronizationContext
                        syncContext <- Threading.SynchronizationContext.Current
                        if isNull syncContext then 
                            log "**Threading.SynchronizationContext.Current is null"
                    with  e -> 
                        log "**Failed to SetSynchronizationContext from DispatcherSynchronizationContext: %A" e           

                            
                if isNull syncContext && notNull seffRhinoSyncModule then //its probly already set abouve via DispatcherSynchronizationContext
                    try
                        syncContext <- seffRhinoSyncModule.GetProperty("syncContext").GetValue(seffAssembly) :?> Threading.SynchronizationContext
                        if isNull syncContext then 
                            log "**Seff.Rhino.Sync.syncContext is null"
                    with e -> 
                        log "**Seff.Rhino.Sync.syncContext failed with: %A" e
                    
        initIsPending <- false               
        
  
    // ---------------------------------
    // Public members:
    // ---------------------------------
    
    // Test if the current thread is the main UI thread
    // just calls RhinoApp.InvokeRequired
    //static member IsCurrrenThreadUIThread =
        // Threading.Thread.CurrentThread = Windows.Threading.Dispatcher.CurrentDispatcher.Thread // fails ! if not in WPF ??
        //RhinoApp.InvokeRequired
        //    this calls: (via ILSpy)
        //    [DllImport("rhcommon_c", CallingConvention = CallingConvention.Cdecl)]
        //    [return: MarshalAs(UnmanagedType.U1)]
        //    internal static extern bool CRhMainFrame_InvokeRequired();
   

    /// The SynchronizationContext of the currently Running Rhino Instance,
    /// This SynchronizationContext is loaded via reflection from the Seff.Rhino plugin      
    static member SyncContext = 
        if isNull syncContext then init()
        syncContext
    
    /// The WPF Window of currently running Seff Editor
    static member SeffWindow = seffWindow

    /// The Assembly currently running Seff Editor Window
    static member SeffRhinoAssembly = seffAssembly

    /// Set up Sync Context and Refrence to Seff Window via reflection on Seff Plugin
    static member Initialize() = init() // called in ActiveDocument module

    /// Evaluates a function on UI Thread.
    static member DoSync (func:unit->'T) : 'T =        
        if RhinoApp.InvokeRequired then
             if isNull syncContext then RhinoSync.Initialize()
             if isNull syncContext then RhinoSyncException.Raise "Rhino.RhinoSync.syncContext is still null and not set up. UI code only works when started in sync mode."                
             async{
                    do! Async.SwitchToContext syncContext
                    return func()  
                    } |> Async.RunSynchronously        
        else
            func()

    /// Evaluates a function on UI Thread. 
    /// Also ensures that redraw is enabled and disabled afterwards again if it was disabled initailly.
    static member DoSyncRedraw (func:unit->'T) : 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        if RhinoApp.InvokeRequired then
             if isNull syncContext then RhinoSync.Initialize()
             if isNull syncContext then RhinoSyncException.Raise "Rhino.RhinoSync.syncContext is still null and not set up. UI code only works when started in sync mode."                
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
    /// Also ensures that redraw is enabled and disabled afterwards again if it was disabled initailly.  
    /// Hides Seff editor window if it exists. Shows it afterwards again
    static member DoSyncRedrawHideEditor (func:unit->'T) : 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        let isWin = if isNull seffWindow then false else seffWindow.Visibility = Windows.Visibility.Visible
        if RhinoApp.InvokeRequired then
             if isNull syncContext then RhinoSync.Initialize()
             if isNull syncContext then RhinoSyncException.Raise "Rhino.RhinoSync.syncContext is still null and not set up. UI code only works when started in sync mode."                
             async{
                    do! Async.SwitchToContext syncContext
                    if isWin then seffWindow.Hide()
                    if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true                
                    let res = func()
                    if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
                    if isWin then seffWindow.Show() 
                    return res
                    } |> Async.RunSynchronously        
        else
            if isWin then seffWindow.Hide()
            if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true
            let res = func()
            if not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
            if isWin then seffWindow.Show() 
            res
