namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Runtime

type internal OPT = Runtime.InteropServices.OptionalAttribute
type internal DEF = Runtime.InteropServices.DefaultParameterValueAttribute


[<AutoOpen>]
module ActiceDocument =
    

    /// the current active Rhino document (= the file currently open)
    let mutable Doc = 
        if HostUtils.RunningInRhino then 
            Rhino.RhinoDoc.ActiveDoc 
        else 
            failwith "failed to find the active Rhino document, is this dll running inside Rhino? " 
    
    ///Redraws all Rhino viewports
    let redraw() = Doc.Views.Redraw()
   

    ///SynchronizationContext  of the currently Running Rhino Instance, set via reflection on Seff.Rhino
    let mutable internal syncContext: Threading.SynchronizationContext  = null //Threading.SynchronizationContext.Current  //OLD:: to be set via RhinoScriptSyntax.SynchronizationContext from running script
    
    let private setSyncContectFromSeffRhinoPlugin() =
        try
            let seffId = System.Guid("01dab273-99ae-4760-8695-3f29f4887831")
            let seffRh = Rhino.PlugIns.PlugIn.Find(seffId)
            let ass = seffRh.Assembly
            let modul = ass.GetType("Seff.Rhino.Sync") 
            syncContext <- modul.GetProperty("syncContext").GetValue(ass) :?> Threading.SynchronizationContext
        with _ ->
            "Failed to get Seff.Rhino.Sync.syncContext via Reflection, UI interactions will crash Rhino!"
            |>> RhinoApp.WriteLine 
            |> printfn "%s"


    /// to store last created object form executing a rs.Command(...)
    let mutable internal commandSerialNumbers : option<uint32*uint32> = None // to store last created object form executing a rs.Command(...)

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


    


    do
        if HostUtils.RunningInRhino then 
            Rhino.RhinoDoc.EndOpenDocument.Add (fun args -> Doc <- args.Document)
            if isNull syncContext then setSyncContectFromSeffRhinoPlugin()
            
            
            //RhinoApp.EscapeKeyPressed.Add ( fun _ -> failwith "Esc Key was pressed, Exception raised via Rhino.Scripting") // done in Seff.Rhino

        
