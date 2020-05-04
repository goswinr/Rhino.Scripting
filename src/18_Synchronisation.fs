namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Runtime
open FsEx.SaveIgnore


/// A static classs to hlp access the UI thread from other threads
type Synchronisation private () =
    
    static let mutable seffRhinoSyncModule:Type = null

    static let mutable syncContext: Threading.SynchronizationContext  = null //set via reflection below from Seff.Rhino    
   
    static let mutable seffAssembly : Reflection.Assembly = null //set via reflection below from Seff.Rhino    
    
    static let mutable seffWindow : System.Windows.Window = null //set via reflection below from Seff.Rhino

    static let init()=
        if HostUtils.RunningInRhino  && HostUtils.RunningOnWindows then 
            if isNull syncContext || isNull seffAssembly || isNull seffWindow then 
                // some reflection hacks because Rhinocommon does not expose a UI sync context
                // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130/7
                try
                    let seffId = System.Guid("01dab273-99ae-4760-8695-3f29f4887831") // the GUID of Seff.Rhino Plugin set in it's AssemblyInfo.fs
                    let seffRh = Rhino.PlugIns.PlugIn.Find(seffId)
                    seffAssembly <- seffRh.Assembly
                    seffRhinoSyncModule <- seffAssembly.GetType("Seff.Rhino.Sync") 
                    syncContext <- seffRhinoSyncModule.GetProperty("syncContext").GetValue(seffAssembly) :?> Threading.SynchronizationContext
                with _ ->
                    "Failed to get Seff.Rhino.Sync.syncContext via Reflection, Async UI interactions like selecting objects might crash Rhino!"
                    |>> RhinoApp.WriteLine 
                    |> printfn "%s"
    
                try   
                    seffWindow <- seffRhinoSyncModule.GetProperty("window").GetValue(seffAssembly)  :?> System.Windows.Window
                with _ ->
                    "Failed to get Seff.Rhino.SeffPlugin.Instance.Window via Reflection, editor window will not hide on UI interactions"
                    |>> RhinoApp.WriteLine 
                    |> printfn "%s" 
        
                if notNull syncContext && notNull seffWindow then 
                    "Rhino.Scripting SynchronizationContext and Seff Window refrence is set up."
                    |>> RhinoApp.WriteLine 
                    |> printfn "%s"
    
    do
        init()

    /// The SynchronizationContext of the currently Running Rhino Instance,
    /// This SynchronizationContext is loaded via reflection from the Seff.Rhino plugin      
    static member SyncContext = syncContext
    
    ///the WPF Window of currently running Seff Editor
    static member SeffWindow = seffWindow

    ///the Assembly currently running Seff Editor Window
    static member SeffRhinoAssembly = seffAssembly

    ///set up Sync Context and Refrenbec to Seff Window via reflection on Seff Plugin
    static member Initialize() = init()

    ///Evaluates a function on UI Thread. Optionally enables redraw . Optionally hides Seff editor window if it exists. 
    static member DoSync enableRedraw hideEditor (f:unit->'T): 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        if not RhinoApp.InvokeRequired then 
            if hideEditor && notNull seffWindow then seffWindow.Hide()
            if enableRedraw && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true
            let res = f()
            if enableRedraw && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
            if hideEditor && notNull seffWindow then seffWindow.Show() 
            res
        else
            async{
                do! Async.SwitchToContext syncContext
                if hideEditor && notNull seffWindow then seffWindow.Hide()
                if enableRedraw && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true                
                let res = f()
                if enableRedraw && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
                if hideEditor && notNull seffWindow then seffWindow.Show() 
                return res
                } |> Async.RunSynchronously 
        

            
                
