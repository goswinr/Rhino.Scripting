namespace Rhino.Scripting

open Rhino

open System

open Rhino.Geometry
open Rhino.ApplicationSettings

// open FsEx
// open FsEx.SaveIgnore

[<AutoOpen>]
module AutoOpenApplication =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    ///<summary>Add new command alias to Rhino Command aliases can be added manually by
    ///    using Rhino's Options command and modifying the contents of the Aliases tab.</summary>
    ///<param name="alias">(string) Name of new command alias. Cannot match command names or existing
    ///    aliases</param>
    ///<param name="macro">(string) The macro to run when the alias is executed</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member AddAlias( alias:string,
                            macro:string) : bool =
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.CommandAliasList.Add(alias, macro))


    ///<summary>Add new path to Rhino's search path list. Search paths can be added by
    ///    using Rhino's Options command and modifying the contents of the files tab.</summary>
    ///<param name="folder">(string) A valid folder, or path, to add</param>
    ///<param name="index">(int) Optional, Zero-based position in the search path list to insert.
    ///    If omitted, path will be appended to the end of the search path list.</param>
    ///<returns>(int) The index where the item was inserted.
    ///    -1 on failure.</returns>
    static member AddSearchPath(    folder:string,
                                    [<OPT;DEF(-1)>]index:int) : int =
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.FileSettings.AddSearchPath(folder, index))


    ///<summary>Returns number of command aliases in Rhino.</summary>
    ///<returns>(int) The number of command aliases in Rhino.</returns>
    static member AliasCount() : int =
        ApplicationSettings.CommandAliasList.Count


    ///<summary>Returns the macro of a command alias.</summary>
    ///<param name="alias">(string) The name of an existing command alias</param>
    ///<returns>(string) The existing macro.</returns>
    static member AliasMacro(alias:string) : string = //GET
        ApplicationSettings.CommandAliasList.GetMacro(alias)

    ///<summary>Modifies the macro of a command alias.</summary>
    ///<param name="alias">(string) The name of an existing command alias</param>
    ///<param name="macro">(string) The new macro to run when the alias is executed.</param>
    ///<returns>(unit) void, nothing.</returns>
    static member AliasMacro(alias:string, macro:string) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.CommandAliasList.SetMacro(alias, macro)
            |> ignore)


    ///<summary>Returns a array of command alias names.</summary>
    ///<returns>(string array) a array of command alias names.</returns>
    static member AliasNames() : array<string> =
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.CommandAliasList.GetNames())


    ///<summary>Returns an application interface item's color.</summary>
    ///<param name="item">(int) Item number to either query or modify
    ///    0  = View background
    ///    1  = Major grid line
    ///    2  = Minor grid line
    ///    3  = X-Axis line
    ///    4  = Y-Axis line
    ///    5  = Selected Objects
    ///    6  = Locked Objects
    ///    7  = New layers
    ///    8  = Feedback
    ///    9  = Tracking
    ///    10 = Crosshair
    ///    11 = Text
    ///    12 = Text Background
    ///    13 = Text hover</param>
    ///<returns>(Drawing.Color) The current item color.
    ///    0  = View background
    ///    1  = Major grid line
    ///    2  = Minor grid line
    ///    3  = X-Axis line
    ///    4  = Y-Axis line
    ///    5  = Selected Objects
    ///    6  = Locked Objects
    ///    7  = New layers
    ///    8  = Feedback
    ///    9  = Tracking
    ///    10 = Crosshair
    ///    11 = Text
    ///    12 = Text Background
    ///    13 = Text hover.</returns>
    static member AppearanceColor(item:int) : Drawing.Color = //GET
        if   item = 0 then AppearanceSettings.ViewportBackgroundColor
        elif item = 1 then AppearanceSettings.GridThickLineColor
        elif item = 2 then AppearanceSettings.GridThinLineColor
        elif item = 3 then AppearanceSettings.GridXAxisLineColor
        elif item = 4 then AppearanceSettings.GridYAxisLineColor
        elif item = 5 then AppearanceSettings.SelectedObjectColor
        elif item = 6 then AppearanceSettings.LockedObjectColor
        elif item = 7 then AppearanceSettings.DefaultLayerColor
        elif item = 8 then AppearanceSettings.FeedbackColor
        elif item = 9 then  AppearanceSettings.TrackingColor
        elif item = 10 then AppearanceSettings.CrosshairColor
        elif item = 11 then AppearanceSettings.CommandPromptTextColor
        elif item = 12 then AppearanceSettings.CommandPromptBackgroundColor
        elif item = 13 then AppearanceSettings.CommandPromptHypertextColor
        else RhinoScriptingException.Raise "RhinoScriptSyntax.AppearanceColor: get item %d is out of range" item

    ///<summary>Modifies an application interface item's color.</summary>
    ///<param name="item">(int) Item number to either query or modify
    ///    0  = View background
    ///    1  = Major grid line
    ///    2  = Minor grid line
    ///    3  = X-Axis line
    ///    4  = Y-Axis line
    ///    5  = Selected Objects
    ///    6  = Locked Objects
    ///    7  = New layers
    ///    8  = Feedback
    ///    9  = Tracking
    ///    10 = Crosshair
    ///    11 = Text
    ///    12 = Text Background
    ///    13 = Text hover</param>
    ///<param name="color">(Drawing.Color ) The new color value as System.Drawing.Color</param>
    ///<returns>(unit) void, nothing.</returns>
    static member AppearanceColor(item:int, color:Drawing.Color) : unit = //SET
        RhinoSync.DoSync (fun () ->
            if item = 0 then AppearanceSettings.ViewportBackgroundColor <- color
            elif item = 1 then AppearanceSettings.GridThickLineColor <- color
            elif item = 2 then AppearanceSettings.GridThinLineColor <- color
            elif item = 3 then AppearanceSettings.GridXAxisLineColor <- color
            elif item = 4 then AppearanceSettings.GridYAxisLineColor <- color
            elif item = 5 then AppearanceSettings.SelectedObjectColor <- color
            elif item = 6 then AppearanceSettings.LockedObjectColor <- color
            elif item = 7 then AppearanceSettings.DefaultLayerColor <- color
            elif item = 8 then AppearanceSettings.FeedbackColor <- color
            elif item = 9 then AppearanceSettings.TrackingColor <- color
            elif item = 10 then AppearanceSettings.CrosshairColor <- color
            elif item = 11 then AppearanceSettings.CommandPromptTextColor <- color
            elif item = 12 then AppearanceSettings.CommandPromptBackgroundColor <- color
            elif item = 13 then AppearanceSettings.CommandPromptHypertextColor <- color
            else RhinoScriptingException.Raise "RhinoScriptSyntax.AppearanceColor:: Setting item %d is out of range" item
            State.Doc.Views.Redraw()
            )


    ///<summary>Returns the file name used by Rhino's automatic file saving.</summary>
    ///<returns>(string) The name of the current autosave file.</returns>
    static member AutosaveFile() : string = //GET
        ApplicationSettings.FileSettings.AutoSaveFile

    ///<summary>Changes the file name used by Rhino's automatic file saving.</summary>
    ///<param name="filename">(string) Name of the new autosave file</param>
    ///<returns>(unit) void, nothing.</returns>
    static member AutosaveFile(filename:string) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.FileSettings.AutoSaveFile <- filename)


    ///<summary>Returns how often the document will be saved when Rhino's
    /// automatic file saving mechanism is enabled.</summary>
    ///<returns>(float) The current interval in minutes.</returns>
    static member AutosaveInterval() : float = //GET
        ApplicationSettings.FileSettings.AutoSaveInterval.TotalMinutes

    ///<summary>Changes how often the document will be saved when Rhino's
    /// automatic file saving mechanism is enabled.</summary>
    ///<param name="minutes">(float) The number of minutes between saves</param>
    ///<returns>(unit) void, nothing.</returns>
    static member AutosaveInterval(minutes:float) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.FileSettings.AutoSaveInterval <- TimeSpan.FromMinutes(minutes))


    ///<summary>Returns the build date of Rhino.</summary>
    ///<returns>(DateTime) The build date of Rhino. Will be converted to a string by most functions.</returns>
    static member BuildDate() : DateTime =
        RhinoApp.BuildDate


    ///<summary>Clears contents of Rhino's command history window. You can view the
    ///    command history window by using the CommandHistory command in Rhino.</summary>
    ///<returns>(unit) void, nothing.</returns>
    static member ClearCommandHistory() : unit =
        RhinoSync.DoSync (fun () ->
            RhinoApp.ClearCommandHistoryWindow())


    ///<summary>Runs a Rhino command macro / script. All Rhino commands can be used in command
    ///    scripts. The command can be a built-in Rhino command or one provided by a
    ///    3rd party plug-in.
    ///    Write command scripts just as you would type the command sequence at the
    ///    Commandline. A space or a new line acts like pressing 'Enter' at the
    ///    Commandline. For more information, see "Scripting" in Rhino help.
    ///    Note, this function is designed to run one command and one command only.
    ///    Do not combine multiple Rhino commands into a single call to this method.
    ///      WRONG:
    ///        rs.Command("_Line _SelLast _Invert")
    ///      CORRECT:
    ///        rs.Command("_Line")
    ///        rs.Command("_SelLast")
    ///        rs.Command("_Invert")
    ///    Also, the exclamation point and space character ( ! ) combination used by
    ///    button macros and batch-driven scripts to cancel the previous command is
    ///    not valid.
    ///      WRONG:
    ///        rs.Command("! _Line _Pause _Pause")
    ///      CORRECT:
    ///        rs.Command("_Line _Pause _Pause")
    ///    In a normal command, when the user enters a command beginning with a '!' , the command exits.
    ///    There is no documented way to get this behavior from within a script command.
    ///
    ///    After the command script has run, you can obtain the identifiers of most
    ///    recently created or changed object by calling RhinoScriptSyntax.LastCreatedObjects().
    ///
    ///    Warnings:
    ///    This kind of command can be very dangerous. Please be sure you understand the following:
    ///    If you are not very familiar with how references work, you should only call Rhino.RhinoApp.RunScript()
    ///     from within a RhinoScriptCommand derived command.
    ///    If you are very familiar with references, then please observe the following rules:
    ///    If you get a reference or pointer to any part of the Rhino run-time database, this reference or pointer
    ///    will not be valid after you call Rhino.RhinoApp.RunScript().
    ///    If you get a reference or a pointer, then call Rhino.RhinoApp.RunScript(), and then use the reference,
    ///    Rhino will probably crash.
    ///    All pointers and references used by the command should be scoped such that they are only valid for the
    ///    time between calls to Rhino.RhinoApp.RunScript().
    ///    This is because Rhino.RhinoApp.RunScript() can change the dynamic arrays in the run-time database.
    ///    The result is that all pointers and references become invalid.
    ///    Be sure to scope your variables between Rhino.RhinoApp.RunScript() calls.
    ///    Never allow references and pointers from one section to be used in another section.</summary>
    ///<param name="commandString">(string) A Rhino command including any arguments</param>
    ///<param name="echo">(bool) Optional, default value: <c>true</c>
    ///    The default command echo mode <c>true</c> will display the commands on the commandline.
    ///    If the command echo mode is set to <c>false</c> the command prompts will not be printed to the Rhino command line.</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member Command (commandString:string, [<OPT;DEF(true)>]echo:bool) : bool =
        RhinoSync.DoSync (fun () ->
            let start = DocObjects.RhinoObject.NextRuntimeSerialNumber
            // RhinoApp.RunScript:
            // Rhino acts as if each character in the script string had been typed in the command prompt.
            // When RunScript is called from a "script runner" command, it completely runs the
            // script before returning. When RunScript is called outside of a command, it returns and the
            // script is run. This way menus and buttons can use RunScript to execute complicated functions
            // see warnings from https://developer.rhino3d.com/guides/rhinocommon/run-rhino-command-from-plugin/
            let rc = RhinoApp.RunScript(commandString, echo)
            let ende = DocObjects.RhinoObject.NextRuntimeSerialNumber
            State.CommandSerialNumbers <- None
            if start<>ende then  State.CommandSerialNumbers <- Some(start, ende)
            rc
            )


    ///<summary>Returns the contents of Rhino's command history window.</summary>
    ///<returns>(string) The contents of Rhino's command history window.</returns>
    static member CommandHistory() : string =
        RhinoSync.DoSync (fun () ->
            RhinoApp.CommandHistoryWindowText)


    ///<summary>Returns the default render plug-in.</summary>
    ///<returns>(string) Name of default renderer.</returns>
    static member DefaultRenderer() : string = //GET
        let mutable objectId = Render.Utilities.DefaultRenderPlugInId
        let mutable plugins = PlugIns.PlugIn.GetInstalledPlugIns()
        plugins.[objectId]

    ///<summary>Changes the default render plug-in.</summary>
    ///<param name="renderer">(string) The name of the renderer to set as default renderer</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member DefaultRenderer(renderer:string) : bool = //SET
        RhinoSync.DoSync (fun () ->
            let objectId = Rhino.PlugIns.PlugIn.IdFromName(renderer)
            Rhino.Render.Utilities.SetDefaultRenderPlugIn(objectId)
            )


    ///<summary>Delete an existing alias from Rhino.</summary>
    ///<param name="alias">(string) The name of an existing alias</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member DeleteAlias(alias:string) : bool =
        ApplicationSettings.CommandAliasList.Delete(alias)


    ///<summary>Removes existing path from Rhino's search path list. Search path items
    ///    can be removed manually by using Rhino's options command and modifying the
    ///    contents of the files tab.</summary>
    ///<param name="folder">(string) A folder to remove</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member DeleteSearchPath(folder:string) : bool =
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.FileSettings.DeleteSearchPath(folder))


    ///<summary>Enables/disables OLE Server Busy/Not Responding dialog boxes.</summary>
    ///<param name="enable">(bool) Whether alerts should be visible (True or False)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DisplayOleAlerts(enable:bool) : unit =
        RhinoSync.DoSync (fun () ->
            Rhino.Runtime.HostUtils.DisplayOleAlerts( enable )
            )


    ///<summary>Returns edge analysis color displayed by the ShowEdges command.</summary>
    ///<returns>(Drawing.Color) The current edge analysis color.</returns>
    static member EdgeAnalysisColor() : Drawing.Color= //GET
        ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor

    ///<summary>Modifies edge analysis color displayed by the ShowEdges command.</summary>
    ///<param name="color">(Drawing.Color), optional) The new color for the analysis</param>
    ///<returns>(unit) void, nothing.</returns>
    static member EdgeAnalysisColor(color:Drawing.Color) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor <- color
            )


    ///<summary>Returns edge analysis mode displayed by the ShowEdges command.</summary>
    ///<returns>(int) The current edge analysis mode
    ///    0 - display all edges
    ///    1 - display naked edges.</returns>
    static member EdgeAnalysisMode() : int = //GET
        ApplicationSettings.EdgeAnalysisSettings.ShowEdges

    ///<summary>Modifies edge analysis mode displayed by the ShowEdges command.</summary>
    ///<param name="mode">(int) The new display mode. The available modes are
    ///    0 - display all edges
    ///    1 - display naked edges</param>
    ///<returns>(unit) void, nothing.</returns>
    static member EdgeAnalysisMode(mode:int) : unit = //SET
        RhinoSync.DoSync (fun () ->
            if mode = 1 || mode = 2 then
                ApplicationSettings.EdgeAnalysisSettings.ShowEdges <- mode
            else
                RhinoScriptingException.Raise "RhinoScriptSyntax.EdgeAnalysisMode bad edge analysisMode %d" mode
            )

    ///<summary>Enables or disables Rhino's automatic file saving mechanism.</summary>
    ///<param name="enable">(bool) Optional, default value: <c>true</c>
    ///    The autosave state. If omitted automatic saving is enabled (True)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member EnableAutosave([<OPT;DEF(true)>]enable:bool) : unit =
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.FileSettings.AutoSaveEnabled <- enable)


    ///<summary>Get status of a Rhino plug-in.</summary>
    ///<param name="plugin">(string) The name of the plugin</param>
    ///<returns>(bool) True if set to load silently otherwise False.</returns>
    static member EnablePlugIn(plugin:string) : bool = //GET
        RhinoSync.DoSync (fun () ->
            let objectId = PlugIns.PlugIn.IdFromName(plugin)
            let rc, loadSilent = PlugIns.PlugIn.GetLoadProtection(objectId)
            if rc then loadSilent
            else RhinoScriptingException.Raise "RhinoScriptSyntax.EnablePlugIn: %s GetLoadProtection failed" plugin
            )

    ///<summary>Enables or disables a Rhino plug-in.</summary>
    ///<param name="plugin">(string) The name of the plugin</param>
    ///<param name="enable">(bool) Load silently if True</param>
    ///<returns>(unit) void, nothing.</returns>
    static member EnablePlugIn(plugin:string, enable:bool) : unit = //SET
        RhinoSync.DoSync (fun () ->
            let objectId = Rhino.PlugIns.PlugIn.IdFromName(plugin)
            let rc, _ = Rhino.PlugIns.PlugIn.GetLoadProtection(objectId)
            if rc then PlugIns.PlugIn.SetLoadProtection(objectId, enable)
            else RhinoScriptingException.Raise "RhinoScriptSyntax.EnablePlugIn: %s GetLoadProtection failed" plugin
            )



    ///<summary>Returns the full path to Rhino's executable folder.</summary>
    ///<returns>(string) The full path to Rhino's executable folder.</returns>
    static member ExeFolder() : string =
        ApplicationSettings.FileSettings.ExecutableFolder


    ///<summary>Returns the platform of the Rhino executable , calls System.Environment.Is64BitProcess.</summary>
    ///<returns>(int) 1 for 64 bit, 0 for 32 bit.</returns>
    static member ExePlatform() : int =
        if System.Environment.Is64BitProcess then  1 else  0


    ///<summary>Returns the service release number of the Rhino executable.</summary>
    ///<returns>(int) The service release number of the Rhino executable.</returns>
    static member ExeServiceRelease() : int =
        RhinoApp.ExeServiceRelease


    ///<summary>Returns the major version number of the Rhino executable.</summary>
    ///<returns>(int) The major version number of the Rhino executable.</returns>
    static member ExeVersion() : int =
        RhinoApp.ExeVersion


    ///<summary>Closes the Rhino application.</summary>
    ///<returns>(unit) void, nothing.</returns>
    static member Exit() : unit =
        RhinoSync.DoSync (fun () ->
            RhinoApp.Exit())


    ///<summary>Searches for a file using Rhino's search path. Rhino will look for a file in the following locations:
    ///      1. The current document's folder.
    ///      2. Folder's specified in Options dialog, File tab.
    ///      3. Rhino's System folders.</summary>
    ///<param name="filename">(string) A short file name to search for</param>
    ///<returns>(string) a full path.</returns>
    static member FindFile(filename:string) : string =
        ApplicationSettings.FileSettings.FindFile(filename)


    ///<summary>Returns a scriptable object from a specified plug-in. Not all plug-ins
    ///    contain scriptable objects. Check with the manufacturer of your plug-in
    ///    to see if they support this capability.</summary>
    ///<param name="plugIn">(string) The name of a registered plug-in that supports scripting.
    ///    If the plug-in is registered but not loaded, it will be loaded</param>
    ///<returns>(object) a scriptable plugin object.</returns>
    static member GetPlugInObject(plugIn:string) : obj =
        RhinoApp.GetPlugInObject(plugIn)


    ///<summary>Determines if Rhino is currently running a command. Because Rhino allows
    ///    for transparent commands (commands run from inside of other commands), this
    ///    method returns the total number of active commands.</summary>
    ///<returns>(int) The number of active commands.</returns>
    static member InCommand() : int = // [<OPT;DEF(true)>]ignoreRunners:bool) : int =
        //<param name="ignoreRunners">(bool) Optional, default value: <c>true</c>
        //If True, script running commands, such as
        //  LoadScript, RunScript, and ReadCommandFile will not counted.
        //  If omitted the default is not to count script running command (True)</param>
        //let inCommand (ignoreRunners:bool) :int =
        //<param name="ignoreRunners">ignoreRunners If True, script running commands, such as
        //        LoadScript, RunScript, and ReadCommandFile will not counted.
        //        If omitted the default is not to count script running command (True)</param>
        Commands.Command.GetCommandStack()
        //|> Array.filter (fun i -> if ignoreRunners then  Commands.Command.GetInt(i) <> 1 else true)
        |> Array.length


    ///<summary>The full path to Rhino's installation folder.</summary>
    ///<returns>(string) The full path to Rhino's installation folder.</returns>
    static member InstallFolder() : string =
        ApplicationSettings.FileSettings.InstallFolder.FullName


    ///<summary>Checks if a command alias exists in Rhino.</summary>
    ///<param name="alias">(string) The name of an existing command alias</param>
    ///<returns>(bool) True if exists or False if the alias does not exist.</returns>
    static member IsAlias(alias:string) : bool =
        ApplicationSettings.CommandAliasList.IsAlias(alias)


    ///<summary>Checks if a command exists in Rhino. Useful when scripting commands
    ///    found in 3rd party plug-ins.</summary>
    ///<param name="commandName">(string) The command name to test</param>
    ///<returns>(bool) True if the string is a command or False if it is not a command.</returns>
    static member IsCommand(commandName:string) : bool =
        Commands.Command.IsCommand(commandName)


    ///<summary>Checks if a plug-in is registered.</summary>
    ///<param name="plugin">(string) The unique objectId of the plug-in</param>
    ///<returns>(bool) True if the Guid is registered or False if it is not.</returns>
    static member IsPlugIn(plugin:string) : bool =
        let objectId = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        if objectId = Guid.Empty then false
        else
            let rc, _ , _ = Rhino.PlugIns.PlugIn.PlugInExists(objectId)
            rc


    ///<summary>Returns True if this script is being executed on a Windows platform.</summary>
    ///<returns>(bool) True if currently running on the Widows platform. False if it is not Windows.</returns>
    static member IsRunningOnWindows() : bool =
        Rhino.Runtime.HostUtils.RunningOnWindows


    ///<summary>Returns the name of the last executed command.</summary>
    ///<returns>(string) The name of the last executed command.</returns>
    static member LastCommandName() : string =
        let mutable objectId = Commands.Command.LastCommandId
        Commands.Command.LookupCommandName(objectId, englishName=true)


    ///<summary>Returns the result code for the last executed command.</summary>
    ///<returns>(int) The result code for the last executed command.
    ///    0 = success (command successfully completed)
    ///    1 = cancel (command was cancelled by the user)
    ///    2 = nothing (command did nothing, but was not cancelled)
    ///    3 = failure (command failed due to bad input, computational problem...)
    ///    4 = unknown command (the command was not found).</returns>
    static member LastCommandResult() : int =
        RhinoSync.DoSync (fun () ->
            int(Commands.Command.LastCommandResult))


    ///<summary>Returns the current language used for the Rhino interface. The current
    ///    language is returned as a locale ID, or LCID, value.</summary>
    ///<returns>(int) The current language used for the Rhino interface as a locale ID, or LCID.
    ///    1029  Czech
    ///    1031  German-Germany
    ///    1033  English-United States
    ///    1034  Spanish-Spain
    ///    1036  French-France
    ///    1040  Italian-Italy
    ///    1041  Japanese
    ///    1042  Korean
    ///    1045  Polish.</returns>
    static member LocaleID() : int =
        ApplicationSettings.AppearanceSettings.LanguageIdentifier


    ///<summary>Get status of Rhino's ortho modeling aid.</summary>
    ///<returns>(bool) The current ortho status.</returns>
    static member Ortho() : bool = //GET
        ModelAidSettings.Ortho

    ///<summary>Enables or disables Rhino's ortho modeling aid.</summary>
    ///<param name="enable">(bool) The new enabled status</param>
    ///<returns>(unit) void, nothing.</returns>
    static member Ortho(enable:bool) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ModelAidSettings.Ortho <- enable)


    ///<summary>Get status of Rhino's object snap modeling aid.
    ///    Object snaps are tools for specifying points on existing objects.</summary>
    ///<returns>(bool) The current object snap status.</returns>
    static member Osnap() : bool = //GET
        ModelAidSettings.Osnap

    ///<summary>Enables or disables Rhino's object snap modeling aid.
    ///    Object snaps are tools for specifying points on existing objects.</summary>
    ///<param name="enable">(bool) The new enabled status</param>
    ///<returns>(unit) void, nothing.</returns>
    static member Osnap(enable:bool) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ModelAidSettings.Osnap <- enable)


    ///<summary>Get status of Rhino's dockable object snap bar.</summary>
    ///<returns>(bool) The current visible state.</returns>
    static member OsnapDialog() : bool = //GET
        ModelAidSettings.UseHorizontalDialog

    ///<summary>Shows or hides Rhino's dockable object snap bar.</summary>
    ///<param name="visible">(bool) The new visibility state.</param>
    ///<returns>(unit) void, nothing.</returns>
    static member OsnapDialog(visible:bool) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ModelAidSettings.UseHorizontalDialog <- visible)


    ///<summary>Returns the object snap mode. Object snaps are tools for
    /// specifying points on existing objects.</summary>
    ///<returns>(int) The current object snap mode(s)
    ///    0          None
    ///    2          Near
    ///    8          Focus
    ///    32         Center
    ///    64         Vertex
    ///    128        Knot
    ///    512        Quadrant
    ///    2048       Midpoint
    ///    8192       Intersection
    ///    131072     End
    ///    524288     Perpendicular
    ///    2097152    Tangent
    ///    134217728  Point
    ///    Object snap modes can be added together to set multiple modes.</returns>
    static member OsnapMode() : int = //GET
        int(ModelAidSettings.OsnapModes)

    ///<summary>Sets the object snap mode. Object snaps are tools for
    /// specifying points on existing objects.</summary>
    ///<param name="mode">(int) The object snap mode or modes to set.
    ///    0          None
    ///    2          Near
    ///    8          Focus
    ///    32         Center
    ///    64         Vertex
    ///    128        Knot
    ///    512        Quadrant
    ///    2048       Midpoint
    ///    8192       Intersection
    ///    131072     End
    ///    524288     Perpendicular
    ///    2097152    Tangent
    ///    134217728  Point
    ///    Object snap modes can be added together to set multiple modes</param>
    ///<returns>(unit) void, nothing.</returns>
    static member OsnapMode(mode:int) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ModelAidSettings.OsnapModes <- LanguagePrimitives.EnumOfValue mode)


    ///<summary>Get status of Rhino's planar modeling aid.</summary>
    ///<returns>(bool) The current planar status.</returns>
    static member Planar() : bool = //GET
        ModelAidSettings.Planar

    ///<summary>Enables or disables Rhino's planar modeling aid.</summary>
    ///<param name="enable">(bool) The new enable status.</param>
    ///<returns>(unit) void, nothing.</returns>
    static member Planar(enable:bool) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ModelAidSettings.Planar <- enable)


    ///<summary>Returns the identifier of a plug-in given the plug-in name.</summary>
    ///<param name="plugin">(string) The name  of the plug-in</param>
    ///<returns>(Guid) The  Unique Guid of the plug-in.</returns>
    static member PlugInId(plugin:string) : Guid =
        let objectId = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        if objectId<>Guid.Empty then  objectId
        else RhinoScriptingException.Raise "RhinoScriptSyntax.PlugInId: Plugin %s not found" plugin


    ///<summary>Returns a array of registered Rhino plug-ins.</summary>
    ///<param name="types">(int) Optional, default value: <c>0</c>
    ///    The type of plug-ins to return.
    ///    0 = all
    ///    1 = render
    ///    2 = file export
    ///    4 = file import
    ///    8 = digitizer
    ///    16 = utility.
    ///    If omitted, all are returned</param>
    ///<param name="status">(int) Optional, default value: <c>0</c>
    /// 0 = both loaded and unloaded,
    /// 1 = loaded,
    /// 2 = unloaded. If omitted both status is returned</param>
    ///<returns>(string array) array of registered Rhino plug-ins.</returns>
    static member PlugIns([<OPT;DEF(0)>]types:int, [<OPT;DEF(0)>]status:int) : array<string> =
        let mutable filter = Rhino.PlugIns.PlugInType.Any
        if types = 1 then  filter <- Rhino.PlugIns.PlugInType.Render
        if types = 2 then  filter <- Rhino.PlugIns.PlugInType.FileExport
        if types = 4 then  filter <- Rhino.PlugIns.PlugInType.FileImport
        if types = 8 then  filter <- Rhino.PlugIns.PlugInType.Digitizer
        if types = 16 then filter <- Rhino.PlugIns.PlugInType.Utility
        let loaded = status = 0 || status = 1
        let unloaded = status = 0 || status = 2
        RhinoSync.DoSync (fun () ->
            Rhino.PlugIns.PlugIn.GetInstalledPlugInNames(filter, loaded, unloaded))


    ///<summary>Get status of object snap projection.</summary>
    ///<returns>(bool) The current object snap projection status.</returns>
    static member ProjectOsnaps() : bool = //GET
        ModelAidSettings.ProjectSnapToCPlane

    ///<summary>Enables or disables object snap projection.</summary>
    ///<param name="enable">(bool) The new enabled status.</param>
    static member ProjectOsnaps(enable:bool) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ModelAidSettings.ProjectSnapToCPlane <- enable)


    ///<summary>Change Rhino's command window prompt.</summary>
    ///<param name="message">(string) The new prompt on the commandline</param>
    ///<returns>(unit) void, nothing.</returns>
    static member Prompt(message:string) : unit =
        RhinoSync.DoSync (fun () ->
            RhinoApp.SetCommandPrompt(message))



    ///<summary>Returns current width and height, of the screen of the primary monitor.</summary>
    ///<returns>(int * int) containing two numbers identifying the width and height in pixels.</returns>
    static member ScreenSize() : int * int =
        // let sz = System.Windows.Forms.Screen.PrimaryScreen.Bounds //  TODO: Windows Forms is supported on Mono Mac??  compile separate Assembly for Windows ???
        // sz.Width, sz.Height
        let sz = Eto.Forms.Screen.PrimaryScreen.Bounds
        int sz.Width, int sz.Height


    ///<summary>Returns version of the Rhino SDK supported by the executing Rhino.</summary>
    ///<returns>(int) The version of the Rhino SDK supported by the executing Rhino. Rhino SDK versions are 9 digit numbers in the form of YYYYMMDDn.</returns>
    static member SdkVersion() : int =
        RhinoApp.SdkVersion


    ///<summary>Returns the number of path items in Rhino's search path list.
    ///    See "Options Files settings" in the Rhino help file for more details.</summary>
    ///<returns>(int) The number of path items in Rhino's search path list.</returns>
    static member SearchPathCount() : int =
        ApplicationSettings.FileSettings.SearchPathCount


    ///<summary>Returns all of the path items in Rhino's search path list.
    ///    See "Options Files settings" in the Rhino help file for more details.</summary>
    ///<returns>(string array) list of search paths.</returns>
    static member SearchPathList() : array<string> =
        ApplicationSettings.FileSettings.GetSearchPaths()


    ///<summary>Sends a string of printable characters to Rhino's Commandline.</summary>
    ///<param name="keys">(string) A string of characters to send to the Commandline</param>
    ///<param name="addReturn">(bool) Optional, default value: <c>true</c>
    ///    Append a return character to the end of the string. If omitted an return character will be added (True)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SendKeystrokes(keys:string, [<OPT;DEF(true)>]addReturn:bool) : unit =
        RhinoSync.DoSync (fun () ->
            RhinoApp.SendKeystrokes(keys, addReturn))


    ///<summary>Get status of Rhino's grid snap modeling aid.</summary>
    ///<returns>(bool) The current grid snap status.</returns>
    static member Snap() : bool = //GET
        ModelAidSettings.GridSnap

    ///<summary>Enables or disables Rhino's grid snap modeling aid.</summary>
    ///<param name="enable">(bool) The new enabled status.</param>
    static member Snap(enable:bool) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ModelAidSettings.GridSnap <- enable)


    ///<summary>Sets Rhino's status bar distance pane.</summary>
    ///<param name="distance">(float) The distance to set the status bar</param>
    ///<returns>(unit) void, nothing.</returns>
    static member StatusBarDistance(distance:float) : unit =
        RhinoSync.DoSync (fun () ->
            UI.StatusBar.SetDistancePane(distance))


    ///<summary>Sets Rhino's status bar message pane.</summary>
    ///<param name="message">(string) The message to display</param>
    ///<returns>(unit) void, nothing.</returns>
    static member StatusBarMessage(message:string) : unit =
        RhinoSync.DoSync (fun () ->
            UI.StatusBar.SetMessagePane(message))


    ///<summary>Sets Rhino's status bar point coordinate pane.</summary>
    ///<param name="point">(Point3d) The 3d coordinates of the status bar</param>
    ///<returns>(unit) void, nothing.</returns>
    static member StatusBarPoint(point:Point3d) : unit =
        RhinoSync.DoSync (fun () ->
            UI.StatusBar.SetPointPane(point))


    ///<summary>Start the Rhino status bar progress meter.</summary>
    ///<param name="label">(string) Short description of the progesss</param>
    ///<param name="lower">(int) Lower limit of the progress meter's range</param>
    ///<param name="upper">(int) Upper limit of the progress meter's range</param>
    ///<param name="embedLabel">(bool) Optional, default value: <c>true</c>
    ///    If true, the label will show inside the meter.
    ///    If false, the label will show to the left of the meter</param>
    ///<param name="showPercent">(bool) Optional, default value: <c>true</c>
    ///    Show the percent complete if True</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member StatusBarProgressMeterShow(label:string, lower:int, upper:int, [<OPT;DEF(true)>]embedLabel:bool, [<OPT;DEF(true)>]showPercent:bool) : bool =
        RhinoSync.DoSync (fun () ->
            let mutable rc = UI.StatusBar.ShowProgressMeter(lower, upper, label, embedLabel, showPercent)
            rc = 1)


    ///<summary>Set the current position of the progress meter.</summary>
    ///<param name="position">(int) The new position in the progress meter</param>
    ///<param name="absolute">(bool) Optional, default value: <c>true</c>
    ///    The position is set absolute (True) or relative (False) to its current position. If omitted the absolute (True) is used</param>
    ///<returns>(unit) void, nothing.</returns>
    static member StatusBarProgressMeterUpdate(position:int, [<OPT;DEF(true)>]absolute:bool) : unit =
        RhinoSync.DoSync (fun () ->
            UI.StatusBar.UpdateProgressMeter(position, absolute)
            |> ignore)


    ///<summary>Hide the progress meter.</summary>
    ///<returns>(unit) void, nothing.</returns>
    static member StatusBarProgressMeterHide() : unit =
        RhinoSync.DoSync (fun () ->
            UI.StatusBar.HideProgressMeter())


    ///<summary>Returns Rhino's default template file. This is the file used
    /// when Rhino starts.</summary>
    ///<returns>(string) The current default template file.</returns>
    static member TemplateFile() : string = //GET
        ApplicationSettings.FileSettings.TemplateFile

    ///<summary>Sets Rhino's default template file. This is the file used
    /// when Rhino starts.</summary>
    ///<param name="filename">(string) The name of the new default template file.</param>
    ///<returns>(unit) void, nothing.</returns>
    static member TemplateFile(filename:string) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.FileSettings.TemplateFile <- filename)


    ///<summary>Returns the location of Rhino's template folder.</summary>
    ///<returns>(string) The current template file folder.</returns>
    static member TemplateFolder() : string = //GET
        ApplicationSettings.FileSettings.TemplateFolder

    ///<summary>Sets the location of Rhino's template folder.</summary>
    ///<param name="folder">(string) The location of Rhino's template files. Note, the location must exist</param>
    ///<returns>(unit) void, nothing.</returns>
    static member TemplateFolder(folder:string) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.FileSettings.TemplateFolder <- folder)


    ///<summary>Returns the windows handle of Rhino's main window.</summary>
    ///<returns>(IntPtr) The Window's handle of Rhino's main window. IntPtr is a platform-specific type that is used to represent a pointer or a handle.</returns>
    static member WindowHandle() : IntPtr =
        RhinoApp.MainWindowHandle()


    ///<summary>Returns Rhino's working folder (directory).
    /// The working folder is the default folder for all file operations.</summary>
    ///<returns>(string) The current working folder.</returns>
    static member WorkingFolder() : string = //GET
        ApplicationSettings.FileSettings.WorkingFolder

    ///<summary>Sets Rhino's working folder (directory).
    /// The working folder is the default folder for all file operations.</summary>
    ///<param name="folder">(string) The new working folder for the current Rhino session</param>
    ///<returns>(unit) void, nothing.</returns>
    static member WorkingFolder(folder:string) : unit = //SET
        RhinoSync.DoSync (fun () ->
            ApplicationSettings.FileSettings.WorkingFolder <- folder)



