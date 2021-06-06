namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Runtime
open FsEx.SaveIgnore

/// A static classs to help access the UI thread of Rhino from other threads
[<AbstractClass; Sealed>] 
type SyncRhino private () = //static class, use these attributes to match C# static class and make in visible in C# // https://stackoverflow.com/questions/13101995/defining-static-classes-in-f   
    
    static let mutable seffRhinoSyncModule:Type = null

    static let mutable syncContext: Threading.SynchronizationContext  = null //set via reflection below ;from Seff.Rhino    
   
    static let mutable seffAssembly : Reflection.Assembly = null //set via reflection belo;w from Seff.Rhino    
    
    static let mutable seffWindow : System.Windows.Window = null //set via reflection below; from Seff.Rhino
       
    static let init()=
        if HostUtils.RunningInRhino && HostUtils.RunningOnWindows then 
            if isNull syncContext || isNull seffAssembly then
                try
                    // some reflection hacks because Rhinocommon does not expose a UI sync context
                    // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130/7 
                    let seffId = System.Guid("01dab273-99ae-4760-8695-3f29f4887831") // the GUID of Seff.Rhino Plugin set in it's AssemblyInfo.fs
                    let seffRh = Rhino.PlugIns.PlugIn.Find(seffId)
                    seffAssembly <- seffRh.Assembly
                    seffRhinoSyncModule <- seffAssembly.GetType("Seff.Rhino.Sync") 
                    syncContext <- seffRhinoSyncModule.GetProperty("syncContext").GetValue(seffAssembly) :?> Threading.SynchronizationContext
                    if isNull syncContext then 
                        eprintfn "Seff.Rhino.Sync.syncContext is still -null-, Ensure all UI interactions form this assembly like rs.GetObject() are not done from an async thread!"
                with ex ->
                    eprintfn "Failed to get Seff.Rhino.Sync.syncContext via Reflection, Ensure all UI interactions form this assembly like rs.GetObject() are not done from an async thread! \r\nMessage: %A" ex
                                
                    
            if isNull seffWindow then 
                try   
                    seffWindow <- seffRhinoSyncModule.GetProperty("window").GetValue(seffAssembly)  :?> System.Windows.Window
                    if isNull seffWindow then 
                        eprintfn "Seff.Rhino.SeffPlugin.Instance.Window is still -null-., If you are not using the Seff Editor Plugin this is normal.\r\n If you are using Seff the editor window will not hide on UI interactions" 
                with ex ->
                    eprintfn "Failed to get Seff.Rhino.SeffPlugin.Instance.Window via Reflection, If you are not using the Seff Editor Plugin this is normal.\r\n If you are using Seff the editor window will not hide on UI interactions: \r\n%A" ex 
        else
            eprintfn "SyncRhino.init() not done because: HostUtils.RunningInRhino %b && HostUtils.RunningOnWindows: %b" HostUtils.RunningInRhino  HostUtils.RunningOnWindows
    
  
    // ---------------------------------
    // PUBLIC MEMBERS:
    // ---------------------------------
    
    /// Test if the current thread is the main UI thread
    /// just calls RhinoApp.InvokeRequired
    static member IsCurrrenThreadUIThread()=
        // Threading.Thread.CurrentThread = Windows.Threading.Dispatcher.CurrentDispatcher.Thread // fails ! if not in WPF ??
        RhinoApp.InvokeRequired
        //this calls: (via ILSpy)
        //[DllImport("rhcommon_c", CallingConvention = CallingConvention.Cdecl)]
        //[return: MarshalAs(UnmanagedType.U1)]
        //internal static extern bool CRhMainFrame_InvokeRequired();
   

    /// The SynchronizationContext of the currently Running Rhino Instance,
    /// This SynchronizationContext is loaded via reflection from the Seff.Rhino plugin      
    static member SyncContext = 
        if isNull syncContext then init()
        syncContext
    
    ///the WPF Window of currently running Seff Editor
    static member SeffWindow = seffWindow

    ///the Assembly currently running Seff Editor Window
    static member SeffRhinoAssembly = seffAssembly

    ///set up Sync Context and Refrence to Seff Window via reflection on Seff Plugin
    static member Initialize() = init() // called in ActiveDocument module

    ///Evaluates a function on UI Thread.
    static member DoSync (func:unit->'T) : 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        if RhinoApp.InvokeRequired then
             if isNull syncContext then SyncRhino.Initialize()
             if isNull syncContext then SyncRhinoException.Raise "Rhino.SyncRhino.syncContext is still null and not set up. UI code only works when started in sync mode."                
             async{
                    do! Async.SwitchToContext syncContext
                    return func()  
                    } |> Async.RunSynchronously        
        else
            func()

    ///Evaluates a function on UI Thread. 
    ///Also ensures that redraw is enabled and disabled afterwards again if it was disabled initailly.
    static member DoSyncRedraw (func:unit->'T) : 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        if RhinoApp.InvokeRequired then
             if isNull syncContext then SyncRhino.Initialize()
             if isNull syncContext then SyncRhinoException.Raise "Rhino.SyncRhino.syncContext is still null and not set up. UI code only works when started in sync mode."                
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
            
    ///Evaluates a function on UI Thread. 
    ///Also ensures that redraw is enabled and disabled afterwards again if it was disabled initailly.  
    // Hides Seff editor window if it exists. Shows it afterwards again
    static member DoSyncRedrawHideEditor (func:unit->'T) : 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        let isWin = if isNull seffWindow then false else seffWindow.Visibility = Windows.Visibility.Visible
        if RhinoApp.InvokeRequired then
             if isNull syncContext then SyncRhino.Initialize()
             if isNull syncContext then SyncRhinoException.Raise "Rhino.SyncRhino.syncContext is still null and not set up. UI code only works when started in sync mode."                
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