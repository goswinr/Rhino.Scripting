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
            let setup() = 
                Rhino.RhinoDoc.EndOpenDocument.Add (fun args -> Doc <- args.Document)
                RhinoApp.EscapeKeyPressed.Add     ( fun _    -> if not escapePressed  &&  not <| Input.RhinoGet.InGet(Doc) then escapePressed <- true) // can only be done synchronous
                //RhinoApp.EscapeKeyPressed.Add ( fun _ -> RhinoApp.Wait(); if not <| Input.RhinoGet.InGet(Doc) then failwithf "immediate escape pressed fail") // this does not work on Sync thread
            
            if RhinoApp.InvokeRequired then 
                if isNull Synchronisation.syncContext then                    
                    "Rhino.Scripting.dll is not loaded from main thread, it failed to set up callbacks for pressing Esc key"
                    |>> RhinoApp.WriteLine 
                    |> printfn "%s"  
                else
                    async{
                        do! Async.SwitchToContext Synchronisation.syncContext // ensure its done on UI thread
                        setup() 
                        } |> Async.RunSynchronously
            else
                setup()
        

   