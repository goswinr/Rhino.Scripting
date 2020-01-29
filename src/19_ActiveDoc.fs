namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Runtime
open FsEx.SaveIgnore



type internal OPT = Runtime.InteropServices.OptionalAttribute
type internal DEF = Runtime.InteropServices.DefaultParameterValueAttribute


[<AutoOpen>]
module ActiceDocument =
    
    /// to store last created object form executing a rs.Command(...)
    let mutable internal commandSerialNumbers : option<uint32*uint32> = None // to store last created object form executing a rs.Command(...)

    /// the current active Rhino document (= the file currently open)
    let mutable Doc = 
        if HostUtils.RunningInRhino then 
            Rhino.RhinoDoc.ActiveDoc 
        else 
            failwith "failed to find the active Rhino document, is this dll running inside Rhino? " 
    
    //Redraws all Rhino viewports
    //let redraw() = Doc.Views.Redraw()

    /// gets a localised descritipn on rhino object type (curve , point, surface ....)
    let internal rhtype (g:Guid)=
        if g = Guid.Empty then "-Guid.Empty-"
        else
            let o = Doc.Objects.FindId(g) 
            if isNull o then sprintf "Guid does not exits in current Rhino Document"
            else o.ShortDescription(false)

    ///so that python range expressions dont need top be translated to F#
    let internal range(l) = seq{0..(l-1)} 

    ///if first value is 0.0 return second else first
    let internal ifZero1 a b = if a = 0.0 then b else a
    
    ///if second value is 0.0 return first else second
    let internal ifZero2 a b = if b = 0.0 then a else b    


    let mutable internal escapePressed = false   

    do
        if HostUtils.RunningInRhino then 
            Rhino.RhinoDoc.EndOpenDocument.Add (fun args -> Doc <- args.Document)
            RhinoApp.EscapeKeyPressed.Add ( fun _ -> if not escapePressed &&  not <| Input.RhinoGet.InGet(Doc) then escapePressed <- true) 
            //RhinoApp.EscapeKeyPressed.Add ( fun _ -> RhinoApp.Wait(); if not <| Input.RhinoGet.InGet(Doc) then failwithf "immediate escape pressed fail") // this does not work on Synch thread
            

        
[<AutoOpen>]
/// To acces the UI therad from other therads
module Synchronisation =
    
    let mutable internal seffRhinoSyncModule:Type = null

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
            
    let internal mayFsiBeCancelled() = //To avoid that the next runn of an async script gets cancelled on the first occurence of rs.EscapeTest
        try 
            seffRhinoSyncModule.GetProperty("rhinoScriptSyntaxCanCancel").GetValue(seffRhinoAssembly)  :?> bool
        with _ ->
            "Failed to get Seff.Rhino.Sync.rhinoScriptSyntaxCanCancel via Reflection, cancel will repeat once if in async"
            |>> RhinoApp.WriteLine 
            |> printfn "%s" 
            true

    
    ///Evaluates a function on UI Thread. Optionally enables redraw . Optionally hides Seff editor window if it exists. 
    let doSync enableRedraw hideEditor (f:unit->'T): 'T =
        let redraw = Doc.Views.RedrawEnabled
        if not RhinoApp.InvokeRequired then 
            if hideEditor && notNull seffRhinoWindow then seffRhinoWindow.Hide()
            if enableRedraw && not redraw then Doc.Views.RedrawEnabled <- true
            let res = f()
            if enableRedraw && not redraw then Doc.Views.RedrawEnabled <- false
            if hideEditor && notNull seffRhinoWindow then seffRhinoWindow.Show() 
            res
        else
            async{
                do! Async.SwitchToContext syncContext
                if hideEditor && notNull seffRhinoWindow then seffRhinoWindow.Hide()
                if enableRedraw && not redraw then Doc.Views.RedrawEnabled <- true                
                let res = f()
                if enableRedraw && not redraw then Doc.Views.RedrawEnabled <- false
                if hideEditor && notNull seffRhinoWindow then seffRhinoWindow.Show() 
                return res
                } |> Async.RunSynchronously 
        
    do
        if HostUtils.RunningInRhino  then
            if isNull syncContext     then getSeffRhinoPluginSyncContext()
            if isNull seffRhinoWindow then getSeffRhinoPluginWindow()
            "Rhino.Scripting SynchronizationContext is set up."
            |>> RhinoApp.WriteLine 
            |> printfn "%s"
