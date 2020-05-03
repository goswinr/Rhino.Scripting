namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Runtime
open FsEx.SaveIgnore


/// To acces the UI therad from other threads
module Synchronisation =
    
    let mutable private seffRhinoSyncModule:Type = null

    ///the SynchronizationContext of the currently Running Rhino Instance, 
    let mutable syncContext: Threading.SynchronizationContext  = null //set via reflection below from Seff.Rhino
    
    ///the Assembly currently running Seff Editor Window
    let mutable seffRhinoAssembly : Reflection.Assembly = null //set via reflection below from Seff.Rhino
    
    ///the WPF Window of currently running Seff Editor
    let mutable seffRhinoWindow : System.Windows.Window = null //set via reflection below from Seff.Rhino

    
    let private getSeffRhinoPluginSyncContext() =
        // some reflection hacks because Rhinocommon does not expose a UI sync context
        // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130/7
        try
            let seffId = System.Guid("01dab273-99ae-4760-8695-3f29f4887831")
            let seffRh = Rhino.PlugIns.PlugIn.Find(seffId)
            seffRhinoAssembly <- seffRh.Assembly
            seffRhinoSyncModule <- seffRhinoAssembly.GetType("Seff.Rhino.Sync") 
            syncContext <- seffRhinoSyncModule.GetProperty("syncContext").GetValue(seffRhinoAssembly) :?> Threading.SynchronizationContext
        with _ ->
            "Failed to get Seff.Rhino.Sync.syncContext via Reflection, Async UI interactions like selecting objects might crash Rhino!"
            |>> RhinoApp.WriteLine 
            |> printfn "%s"

    let private getSeffRhinoPluginWindow() =
        try   
            seffRhinoWindow <- seffRhinoSyncModule.GetProperty("window").GetValue(seffRhinoAssembly)  :?> System.Windows.Window
        with _ ->
            "Failed to get Seff.Rhino.SeffPlugin.Instance.Window via Reflection, editor window will not hide on UI interactions"
            |>> RhinoApp.WriteLine 
            |> printfn "%s"    
            
    
    ///Evaluates a function on UI Thread. Optionally enables redraw . Optionally hides Seff editor window if it exists. 
    let doSync enableRedraw hideEditor (f:unit->'T): 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        if not RhinoApp.InvokeRequired then 
            if hideEditor && notNull seffRhinoWindow then seffRhinoWindow.Hide()
            if enableRedraw && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true
            let res = f()
            if enableRedraw && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
            if hideEditor && notNull seffRhinoWindow then seffRhinoWindow.Show() 
            res
        else
            async{
                do! Async.SwitchToContext syncContext
                if hideEditor && notNull seffRhinoWindow then seffRhinoWindow.Hide()
                if enableRedraw && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true                
                let res = f()
                if enableRedraw && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
                if hideEditor && notNull seffRhinoWindow then seffRhinoWindow.Show() 
                return res
                } |> Async.RunSynchronously 
        
    do
        if HostUtils.RunningInRhino  then 
            if isNull syncContext     then getSeffRhinoPluginSyncContext()
            if isNull seffRhinoWindow then getSeffRhinoPluginWindow()
            "Rhino.Scripting SynchronizationContext and Seff Window refrence is set up."
            |>> RhinoApp.WriteLine 
            |> printfn "%s"
           
            
                
