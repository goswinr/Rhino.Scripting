namespace Rhino.Scripting


open System
open Rhino
open Rhino.Runtime
open FsEx
open Rhino
open Rhino



// abreviations so that declarations are not so long
type internal OPT = Runtime.InteropServices.OptionalAttribute
type internal DEF = Runtime.InteropServices.DefaultParameterValueAttribute


[<AutoOpen>]
/// This Module contains 'Doc' the active document, and 'Ot' the current Object Table
/// This module is automatically opened when Rhino.Scripting namspace is opened.
module AutoOpenActiveDocument =
  
    /// The current active Rhino document (= the file currently open)
    let mutable Doc = 
        if HostUtils.RunningInRhino then
            Rhino.RhinoDoc.ActiveDoc  // will be reset in EndOpenDocument Event
        else 
            raise <|  RhinoScriptingException "failed to find the active Rhino document, is this dll running hosted inside the Rhino process? " 
    
    /// Object Table of the current active Rhino documents
    let mutable Ot = Doc.Objects  // will be reset in EndOpenDocument Event
    
    //------------------------------------------------------------------------------------
    // --------------- all below values and functions are internal : ------------------
    //------------------------------------------------------------------------------------
    let mutable internal escapePressed = false // will be reset in EndOpenDocument Event

    /// To store last created object form executing a rs.Command(...)
    let mutable internal commandSerialNumbers : option<uint32*uint32> = None // will be reset in EndOpenDocument Event
   
    /// Gets a description on Rhino object type (curve , point, Surface ....)
    /// including Layer and object name
    let internal rhType (g:Guid)=
        if g = Guid.Empty then "-Guid.Empty-"
        else
            let o = Doc.Objects.FindId(g) 
            if isNull o then sprintf "Guid %A is not in Doc.Objects table of this Rhino file." g
            else
                let name = o.Attributes.Name
                if name <> "" then 
                    sprintf "Guid %A (a %s on Layer '%s' named '%s')" g (o.ShortDescription(false)) (Doc.Layers.[o.Attributes.LayerIndex].FullPath) name
                else 
                    sprintf "Guid %A (an unnamed %s on Layer '%s')" g (o.ShortDescription(false)) (Doc.Layers.[o.Attributes.LayerIndex].FullPath)

    /// Gets a localized description on Rhino layer and  object type (e.g. Curve , point, Surface ....)
    let internal typeDescr (x:'a)=
       match box x with 
       | :? Guid as g -> rhType g
       | _ -> sprintf "%A" x
    

    /// So that python range expressions dont need top be translated to F#
    let internal range(l) = seq{ 0 .. (l-1) } 

    /// If first value is 0.0 return second else first
    let internal ifZero1 a b = if a = 0.0 then b else a
    
    /// if second value is 0.0 return first else second
    let internal ifZero2 a b = if b = 0.0 then a else b    

    do
        if HostUtils.RunningInRhino then
            Synchronisation.Initialize() //declared in Synchronisation static class
            
            // keep the reference to the active Document (3d file ) updated.  
            let updateDoc (doc:RhinoDoc) = 
                Doc <- doc //Rhino.RhinoDoc.ActiveDoc 
                Ot  <- doc.Objects //Rhino.RhinoDoc.ActiveDoc.Objects 
                commandSerialNumbers <- None
                escapePressed <- false

            let setup() = 
                
                // keep the reference to the active Document (3d file ) updated.  
                RhinoDoc.EndOpenDocument.Add (fun args -> updateDoc args.Document)
                //RhinoDoc.BeginOpenDocument.Add //Dont use since it is called on temp pasting files too
                //RhinoDoc.ActiveDocumentChanged.Add (fun args -> updateDoc args.Document) // seems to not work ??             
                                    
                
                // listen to Esc Key press.
                // doing this "Add" in sync is only required if no handler has been added in sync before. Adding the first handler to this from async thread cause a Access violation exeption that can only be seen with the window event log.
                // This his handler does not work on Sync evaluationmode , TODO: test!               
                RhinoApp.EscapeKeyPressed.Add    ( fun _ -> 
                    if not escapePressed  &&  not <| Input.RhinoGet.InGet(Doc) then 
                        escapePressed <- true
                    ) 
                
            
            if RhinoApp.InvokeRequired then 
                if isNull Synchronisation.SyncContext then                    
                    "Synchronisation.syncContext could not be set via reflection.\r\n"+
                    "Rhino.Scripting.dll is not loaded from main thread \r\n"+
                    "it needs to set up callbacks for pressing Esc key and changing the active document. \r\n"+
                    "Setting up these event handlers async can trigger a fatal access violation exception \r\n"+
                    "if its the first time you access the event.\r\n"+
                    "Try to set them up yourself on main thread \r\n"+
                    "or after you have already attached a dummy handler from main thread:\r\n"+
                    "Rhino.RhinoDoc.EndOpenDocument.Add (fun args -> Doc <- args.Document)"+
                    "RhinoApp.EscapeKeyPressed.Add     ( fun _    ->  \r\n"+
                    "          if not escapePressed  &&  not <| Input.RhinoGet.InGet(Doc) then escapePressed <- true)" 
                    |>! RhinoApp.WriteLine 
                    |> eprintfn "%s"  
                else
                    async{ 
                        do! Async.SwitchToContext Synchronisation.SyncContext // ensure its done on UI thread
                        setup() 
                        } |> Async.RunSynchronously
            else
                setup()
        

   
