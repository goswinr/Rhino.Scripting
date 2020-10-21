namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Runtime
open FsEx.SaveIgnore

type RhinoScriptingException (s:string) =
    inherit System.Exception(s)
    static member inline Raise msg = Printf.kprintf (fun s -> raise (new RhinoScriptingException(s))) msg 
    static member inline FailIfFalse s b = if not b then raise (new RhinoScriptingException(s))



/// A static classs to help access the UI thread from other threads
type Synchronisation private () =
    
    static let mutable seffRhinoSyncModule:Type = null

    static let mutable syncContext: Threading.SynchronizationContext  = null //set via reflection below from Seff.Rhino    
   
    static let mutable seffAssembly : Reflection.Assembly = null //set via reflection below from Seff.Rhino    
    
    static let mutable seffWindow : System.Windows.Window = null //set via reflection below from Seff.Rhino

    static let mutable colorLogger : int-> int -> int -> string -> unit = //set via reflection below from Seff.Rhino
        fun r g b s -> 
             RhinoApp.WriteLine s
             printfn "%s" s

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
                    "Failed to get Seff.Rhino.Sync.syncContext via Reflection, Ensure all UI interactions form this assembly like rs.GetObject() are not done from an async thread!"                     
                    |> eprintfn "%s"
                
                try   
                    let printModule = seffAssembly.GetType("Seff.Rhino.Print") 
                    colorLogger <- printModule.GetProperty("colorLogger").GetValue(seffAssembly) :?>  int-> int -> int -> string -> unit
                with _ ->
                    "Failed to get Seff.Rhino.Print.colorLogger  via Reflection, If you are not using the Seff Editor Plugin this is normal."                    
                    |> eprintfn "%s" 


                try   
                    seffWindow <- seffRhinoSyncModule.GetProperty("window").GetValue(seffAssembly)  :?> System.Windows.Window
                with _ ->
                    "Failed to get Seff.Rhino.SeffPlugin.Instance.Window via Reflection, If you are not using the Seff Editor Plugin this is normal.\r\n If you are using Seff the editor window will not hide on UI interactions"                    
                    |> eprintfn "%s" 
        
                if notNull syncContext && notNull seffWindow then ()
                    //"Rhino.Scripting SynchronizationContext and Seff Window refrence is set up."
                    //|>! RhinoApp.WriteLine 
                    //|> printfn "%s"
    
  
    // ---------------------------------
    // PUBLIC MEMBERS:
    // ---------------------------------

    /// print with rgb colors
    /// red -> green -> blue -> string -> unit
    static member ColorLogger = colorLogger

    /// The SynchronizationContext of the currently Running Rhino Instance,
    /// This SynchronizationContext is loaded via reflection from the Seff.Rhino plugin      
    static member SyncContext = syncContext
    
    ///the WPF Window of currently running Seff Editor
    static member SeffWindow = seffWindow

    ///the Assembly currently running Seff Editor Window
    static member SeffRhinoAssembly = seffAssembly

    ///set up Sync Context and Refrence to Seff Window via reflection on Seff Plugin
    static member Initialize() = init() // called in ActiveDocument module


    ///Evaluates a function on UI Thread. Optionally ensures that redraw is enabled . Optionally hides Seff editor window if it exists. 
    static member DoSync ensureRedrawEnabled hideEditor (func:unit->'T): 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        if not RhinoApp.InvokeRequired then 
            if hideEditor && notNull seffWindow then seffWindow.Hide()
            if ensureRedrawEnabled && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true
            let res = func()
            if ensureRedrawEnabled && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
            if hideEditor && notNull seffWindow then seffWindow.Show() 
            res
        else
            async{
                do! Async.SwitchToContext syncContext
                if hideEditor && notNull seffWindow then seffWindow.Hide()
                if ensureRedrawEnabled && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true                
                let res = func()
                if ensureRedrawEnabled && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
                if hideEditor && notNull seffWindow then seffWindow.Show() 
                return res
                } |> Async.RunSynchronously 
        

            
                
