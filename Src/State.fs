﻿namespace Rhino.Scripting

open Rhino
open System
open Rhino.Scripting.RhinoScriptingUtils

/// An internal static class to hold current state like active Rhino document.
[<AbstractClass; Sealed>] //static class, use these attributes to match C# static class and make in visible in C# // https://stackoverflow.com/questions/13101995/defining-static-classes-in-f
type internal State private () =

    /// Rhino.Runtime.HostUtils.RunningInRhino
    static let mutable isRunningInRhino = false

    /// was escape key pressed
    static let mutable escapePressed = false // will be reset in EndOpenDocument Event

    /// To store last created object form executing a rs.Command(...)
    static let mutable commandSerialNumbers : option<uint32*uint32> = None // will be reset in EndOpenDocument Event

    /// The current active Rhino document (= the file currently open)
    static let mutable doc : RhinoDoc = null

    /// Object Table of the current active Rhino document
    static let mutable ot : DocObjects.Tables.ObjectTable = null

    //----------------------------------------------------------------
    //-----------------Update state:----------------------------------
    //----------------------------------------------------------------


    /// keep the reference to the active Document (3d file ) updated.
    static let updateDoc (document:RhinoDoc) =
        doc <- document //Rhino.RhinoDoc.ActiveDoc
        ot  <- document.Objects //Rhino.RhinoDoc.ActiveDoc.Objects
        commandSerialNumbers <- None
        escapePressed <- false

    // -------Events: --------------------


    // static let warnAboutFailedEventSetup () =
    //     [
    //     "***"
    //     "RhinoSync.syncContext could not be set via reflection."
    //     "Rhino.Scripting.dll is not loaded from main thread"
    //     "it needs to set up callbacks for pressing Esc key and changing the active document."
    //     "Setting up these event handlers async can trigger a fatal access violation exception"
    //     "if its the first time you access the event."
    //     "Try to set them up yourself on main thread"
    //     "or after you have already attached a dummy handler from main thread:"
    //     "   Rhino.RhinoDoc.EndOpenDocument.Add (fun args -> Doc <- args.Document)"
    //     "   RhinoApp.EscapeKeyPressed.Add ( fun _  -> if not escapePressed  &&  not <| Input.RhinoGet.InGet(Doc) then escapePressed <- true)"
    //     "***"
    //     ]
    //     |> String.concat Environment.NewLine
    //     |>! RhinoApp.WriteLine
    //     |> eprintfn "%s"


    static let setupEventsInSync() =
        try
            RhinoSync.DoSync (fun () ->
                // keep the reference to the active Document (3d file ) updated:
                RhinoDoc.ActiveDocumentChanged.Add (fun args ->  updateDoc args.Document)
                // RhinoDoc.EndOpenDocument.Add  // used here in the past. why ?
                // RhinoDoc.BeginOpenDocument.Add //Don't use since it is called on temp pasting files too

                // listen to Esc Key press.
                // doing this "Add" in on UI thread is only required if no handler has been added in sync before.
                // Adding the first handler to this from async thread cause a Access violation exception that can only be seen with the windows event log.
                // This his handler does not work on Sync evaluation-mode , TODO: test!
                RhinoApp.EscapeKeyPressed.Add( fun _ ->
                    if not escapePressed  &&  not <| Input.RhinoGet.InGet(doc) then
                        escapePressed <- true
                    )
                )
        with
            // | :? RhinoSyncException -> warnAboutFailedEventSetup()
            | e ->
                //raise e
                let txt = sprintf "%A" e
                RhinoApp.WriteLine txt
                eprintfn "%s" txt


    static let initState()=
        if not Rhino.Runtime.HostUtils.RunningInRhino then
            RhinoScriptingException.Raise "RhinoScriptSyntax.State.initState Failed to find the active Rhino document, is this dll running hosted inside the Rhino process? "
        else
            //RhinoSync.Initialize() // don't do yet, only try to get sync context when actually needed, if on UI thread this might be never.
            updateDoc(RhinoDoc.ActiveDoc )  // do first
            setupEventsInSync()             // do after Doc is set up
            isRunningInRhino <- true        // do last


    //----------------------------------------------------------------
    //-----------------internally public members------------------------
    //----------------------------------------------------------------

    /// The current active Rhino document (= the file currently open)
    static member Doc
        with get()=
            if isNull doc then initState()
            doc

    /// Object Table of the current active Rhino document
    static member Ot
        with get()=
            if isNull doc then initState()
            doc.Objects

    /// Was escape key pressed
    static member EscapePressed
        with get() =
            if isNull doc then initState()
            escapePressed
        and set v =
            escapePressed <- v

    /// To store last created object form executing a rs.Command(...)
    static member CommandSerialNumbers
        with get() =
            if isNull doc then initState()
            commandSerialNumbers
        and set v =
            commandSerialNumbers <- v


    /// does RhinoApp.WriteLine
    /// and eprintfn
    static member Warn(msg:string) =
        RhinoApp.WriteLine msg
        eprintfn "%s" msg
