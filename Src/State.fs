namespace Rhino.Scripting

open Rhino
open System
open Rhino.Scripting.RhinoScriptingUtils

/// An internal static class to hold current state like active Rhino document.
[<AbstractClass; Sealed>] //static class, use these attributes to match C# static class and make it visible in C# // https://stackoverflow.com/questions/13101995/defining-static-classes-in-f
type internal State private () =

    /// Rhino.Runtime.HostUtils.RunningInRhino
    [<VolatileField>]
    static let mutable isRunningInRhino = false

    /// was escape key pressed
    static let mutable escapePressed = false // will be reset in EndOpenDocument Event

    /// To store last created object from executing a rs.Command(...)
    static let mutable commandSerialNumbers : option<uint32*uint32> = None // will be reset in EndOpenDocument Event

    /// The current active Rhino document (= the file currently open)
    static let mutable doc : RhinoDoc = null

    /// Object Table of the current active Rhino document
    static let mutable ot : DocObjects.Tables.ObjectTable = null

    /// Is set once the ActiveDocumentChanged event handler is added.
    /// Until then getDoc checks for a changed active document itself.
    [<VolatileField>]
    static let mutable docEventIsSetUp = false

    //----------------------------------------------------------------
    //-----------------Update state:----------------------------------
    //----------------------------------------------------------------


    /// keep the reference to the active Document (3d file) updated.
    /// The document may be null, e.g. on Mac when the last document is closed.
    static let updateDoc (document:RhinoDoc) =
        doc <- document //Rhino.RhinoDoc.ActiveDoc
        ot  <- if isNull document then null else document.Objects //Rhino.RhinoDoc.ActiveDoc.Objects
        commandSerialNumbers <- None
        escapePressed <- false

    /// Calls updateDoc only if the active document is a different one,
    /// so that the Esc and Command state is not reset needlessly.
    static let updateDocIfChanged() =
        let active = RhinoDoc.ActiveDoc
        let isSame =
            if isNull active then isNull doc
            else notNull doc && doc.RuntimeSerialNumber = active.RuntimeSerialNumber
        if not isSame then
            updateDoc active

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


    /// Adds the event handlers on the UI thread.
    /// From any other thread they are added asynchronously, without waiting for the UI thread,
    /// because the UI thread might be waiting for this thread, e.g. in Array.Parallel.map. That would deadlock.
    static let setupEvents() =
        let setup () =
            try
                // keep the reference to the active Document (3d file ) updated:
                RhinoDoc.ActiveDocumentChanged.Add (fun args ->  updateDoc args.Document)
                docEventIsSetUp <- true
                // RhinoDoc.EndOpenDocument.Add  // used here in the past. why ?
                // RhinoDoc.BeginOpenDocument.Add //Don't use since it is called on temp pasting files too

                // listen to Esc Key press.
                // doing this "Add" on UI thread is only required if no handler has been added in sync before.
                // Adding the first handler to this from async thread causes an Access violation exception that can only be seen with the windows event log.
                // This handler does not work on Sync evaluation-mode, TODO: test!
                RhinoApp.EscapeKeyPressed.Add( fun _ ->
                    if not escapePressed && notNull doc && not <| Input.RhinoGet.InGet(doc) then
                        escapePressed <- true
                    )
            with
                // | :? RhinoSyncException -> warnAboutFailedEventSetup()
                | e ->
                    //raise e
                    let txt = sprintf "%A" e
                    RhinoApp.WriteLine txt
                    eprintfn "%s" txt
        if RhinoApp.InvokeRequired then
            RhinoApp.InvokeOnUiThread(Action setup) // does not wait, unlike RhinoSync.DoSync
        else
            setup()

    static let initLock = obj()

    /// Runs only once, sets up the event handlers.
    /// Thread safe, isRunningInRhino is only set to true once the setup is done.
    static let initState()=
        if not isRunningInRhino then
            lock initLock (fun () ->
                if not isRunningInRhino then
                    if not Rhino.Runtime.HostUtils.RunningInRhino then
                        RhinoScriptingException.Raise "State.initState Failed to find the active Rhino document, is this dll running hosted inside the Rhino process? "
                    else
                        //RhinoSync.Initialize() // don't do yet, only try to get sync context when actually needed, if on UI thread this might be never.
                        updateDoc(RhinoDoc.ActiveDoc )  // do first
                        setupEvents()                   // do after Doc is set up
                        isRunningInRhino <- true        // do last
                )

    /// Returns the current document, fails with a clear error if there is none, e.g. on Mac when all documents are closed.
    static let getDoc() =
        initState()
        if isNull doc || not docEventIsSetUp then
            updateDocIfChanged() // in case the ActiveDocumentChanged event was missed, or its handler is not added (yet)
            if isNull doc then
                RhinoScriptingException.Raise "State.Doc: There is no active Rhino document. Open or create a document first."
        doc


    //----------------------------------------------------------------
    //-----------------internally public members------------------------
    //----------------------------------------------------------------

    /// The current active Rhino document (= the file currently open)
    static member Doc
        with get()=
            getDoc()

    /// Object Table of the current active Rhino document
    static member Ot
        with get()=
            getDoc().Objects

    /// Was escape key pressed
    static member EscapePressed
        with get() =
            initState()
            escapePressed
        and set v =
            initState() // otherwise a later first initState would reset the value
            escapePressed <- v

    /// To store last created object from executing a rs.Command(...)
    static member CommandSerialNumbers
        with get() =
            initState()
            commandSerialNumbers
        and set v =
            initState() // otherwise a later first initState would reset the value
            commandSerialNumbers <- v


    /// does RhinoApp.WriteLine
    /// and eprintfn
    static member Warn(msg:string) =
        RhinoApp.WriteLine msg
        eprintfn "%s" msg
