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

type UserInteractionException (s:string) =
    inherit System.Exception(s)
    static member inline Raise msg = Printf.kprintf (fun s -> raise (new UserInteractionException(s))) msg 

/// A static classs to help access the UI thread from other threads
type Synchronisation private () = // no public constructor
    
    static let mutable seffRhinoSyncModule:Type = null

    static let mutable syncContext: Threading.SynchronizationContext  = null //set via reflection below from Seff.Rhino    
   
    static let mutable seffAssembly : Reflection.Assembly = null //set via reflection below from Seff.Rhino    
    
    static let mutable seffWindow : System.Windows.Window = null //set via reflection below from Seff.Rhino

    (* in fsEx now
    static let mutable colorLogger : int-> int -> int -> string -> unit = //set via reflection below from Seff.Rhino
        fun r g b s -> 
             RhinoApp.Write s
             printf "%s" s    
    
    static let mutable colorLoggerNl : int-> int -> int -> string -> unit = //set via reflection below from Seff.Rhino
        fun r g b s -> 
             RhinoApp.WriteLine s
             printfn "%s" s
    *)
   
    static let init()=
        if HostUtils.RunningInRhino  && HostUtils.RunningOnWindows then 
            if isNull syncContext || isNull seffAssembly then
                try
                // some reflection hacks because Rhinocommon does not expose a UI sync context
                // https://discourse.mcneel.com/t/use-rhino-ui-dialogs-from-worker-threads/90130/7 
                    let seffId = System.Guid("01dab273-99ae-4760-8695-3f29f4887831") // the GUID of Seff.Rhino Plugin set in it's AssemblyInfo.fs
                    let seffRh = Rhino.PlugIns.PlugIn.Find(seffId)
                    seffAssembly <- seffRh.Assembly
                    seffRhinoSyncModule <- seffAssembly.GetType("Seff.Rhino.Sync") 
                    syncContext <- seffRhinoSyncModule.GetProperty("syncContext").GetValue(seffAssembly) :?> Threading.SynchronizationContext
                with ex ->
                    eprintfn "Failed to get Seff.Rhino.Sync.syncContext via Reflection, Ensure all UI interactions form this assembly like rs.GetObject() are not done from an async thread! \r\nMessage: %A" ex
                if isNull syncContext then 
                    eprintfn "Seff.Rhino.Sync.syncContext is still -null-, Ensure all UI interactions form this assembly like rs.GetObject() are not done from an async thread!" 
                if isNull seffAssembly then  
                    eprintfn "Seff.Rhino.Print.colorLogger is still -null-. If you are not using the Seff Editor Plugin this is normal." 
                
                (*
                try   
                    let printModule = seffAssembly.GetType("Seff.Rhino.Print") 
                    colorLogger   <- printModule.GetProperty("colorLogger").GetValue(seffAssembly) :?>  int-> int -> int -> string -> unit
                    colorLoggerNl <- printModule.GetProperty("colorLoggerNl").GetValue(seffAssembly) :?>  int-> int -> int -> string -> unit
                with ex ->
                    eprintfn "Failed to get Seff.Rhino.Print.colorLogger or colorLoggerNl via Reflection, If you are not using the Seff Editor Plugin this is normal.\r\nMessage: %A" ex                    
                *)
                
                    
            if isNull seffWindow then 
                try   
                    seffWindow <- seffRhinoSyncModule.GetProperty("window").GetValue(seffAssembly)  :?> System.Windows.Window
                with ex ->
                    eprintfn "Failed to get Seff.Rhino.SeffPlugin.Instance.Window via Reflection, If you are not using the Seff Editor Plugin this is normal.\r\n If you are using Seff the editor window will not hide on UI interactions: \r\n%A" ex 
                if isNull seffWindow then 
                    "Seff.Rhino.SeffPlugin.Instance.Window is still -null-., If you are not using the Seff Editor Plugin this is normal.\r\n If you are using Seff the editor window will not hide on UI interactions"                    
                    |> eprintfn "%s"

            //if notNull syncContext && notNull seffWindow then 
                //"Rhino.Scripting SynchronizationContext and Seff Window refrence is set up."
                //|>! RhinoApp.WriteLine 
                //|> printfn "%s"
        else
            eprintfn "Synchronisation.init() not done because HostUtils.RunningInRhino %b HostUtils.RunningOnWindows: %b" HostUtils.RunningInRhino  HostUtils.RunningOnWindows
    
  
    // ---------------------------------
    // PUBLIC MEMBERS:
    // ---------------------------------
    
    /// test if the current thread is the main WPF UI thread
    /// just calls RhinoApp.InvokeRequired
    static member isCurrrenThreadUI()=
        // Threading.Thread.CurrentThread = Windows.Threading.Dispatcher.CurrentDispatcher.Thread // fails
        RhinoApp.InvokeRequired
    (*
    /// print with rgb colors. does not add a new line
    /// red -> green -> blue -> string -> unit
    static member ColorLogger = colorLogger

    /// print with rgb colors. adds a new line
    /// red -> green -> blue -> string -> unit
    static member ColorLoggerNl = colorLoggerNl
    *)

    /// The SynchronizationContext of the currently Running Rhino Instance,
    /// This SynchronizationContext is loaded via reflection from the Seff.Rhino plugin      
    static member SyncContext = 
        if isNull syncContext then 
            Synchronisation.Initialize()
        syncContext
    
    ///the WPF Window of currently running Seff Editor
    static member SeffWindow = seffWindow

    ///the Assembly currently running Seff Editor Window
    static member SeffRhinoAssembly = seffAssembly

    ///set up Sync Context and Refrence to Seff Window via reflection on Seff Plugin
    static member Initialize() = init() // called in ActiveDocument module

    ///Evaluates a function on UI Thread. Optionally ensures that redraw is enabled . Optionally hides Seff editor window if it exists. 
    static member DoSync ensureRedrawEnabled hideEditor (func:unit->'T): 'T =
        let redraw = RhinoDoc.ActiveDoc.Views.RedrawEnabled
        if RhinoApp.InvokeRequired then
             if isNull syncContext then 
                Synchronisation.Initialize()
             if isNull syncContext then 
                RhinoScriptingException.Raise "Rhino.Synchronisation.syncContext is still null and not set up. UI code only works when started in sync mode."
                
             async{
                    do! Async.SwitchToContext syncContext
                    if hideEditor && notNull seffWindow then seffWindow.Hide()
                    if ensureRedrawEnabled && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true                
                    let res = func()
                    if ensureRedrawEnabled && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
                    if hideEditor && notNull seffWindow then seffWindow.Show() 
                    return res
                    } |> Async.RunSynchronously        
        else
            if hideEditor && notNull seffWindow then seffWindow.Hide()
            if ensureRedrawEnabled && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- true
            let res = func()
            if ensureRedrawEnabled && not redraw then RhinoDoc.ActiveDoc.Views.RedrawEnabled <- false
            if hideEditor && notNull seffWindow then seffWindow.Show() 
            res
       

            
                
