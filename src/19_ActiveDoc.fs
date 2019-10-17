namespace Rhino.Scripting


open Rhino.Runtime

[<AutoOpen>]
module ActiceDocument =


    /// the current active Rhino document (= the file currently open)
    let mutable Doc = 
        if HostUtils.RunningInRhino then 
            Rhino.RhinoDoc.ActiveDoc 
        else 
            failwith "failed to find the active Rhino document, is this dll running inside Rhino? " 
    
    /// redraws all Rhino viewports
    let redraw() = Doc.Views.Redraw()
   

    ///of the currently Running Rhino Instance, to be set via RhinoScriptSyntax.SynchronizationContext from running script
    let mutable internal syncContext = System.Threading.SynchronizationContext.Current  
    
    /// to store last created object form executing a rs.Command(...)
    let mutable internal commandSerialNumbers : option<uint32*uint32> = None // to store last created object form executing a rs.Command(...)

    do
        if HostUtils.RunningInRhino then 
            Rhino.RhinoDoc.EndOpenDocument.Add (fun args -> Doc <- args.Document)
            
            //RhinoApp.EscapeKeyPressed.Add ( fun _ -> failwith "Esc Key was pressed, Exception raised via Rhino.Scripting")

        
