namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.ActiceDocument
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.ApplicationSettings
open FsEx.SaveIgnore

[<AutoOpen>]
module ExtensionsApplication =

  type RhinoScriptSyntax with

    [<Extension>]
    ///<summary>Add new command alias to Rhino. Command aliases can be added manually by
    ///    using Rhino's Options command and modifying the contents of the Aliases tab</summary>
    ///<param name="alias">(string) Name of new command alias. Cannot match command names or existing
    ///    aliases</param>
    ///<param name="macro">(string) The macro to run when the alias is executed</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member AddAlias(alias:string, macro:string) : bool =
        ApplicationSettings.CommandAliasList.Add(alias, macro)


    [<Extension>]
    ///<summary>Add new path to Rhino's search path list. Search paths can be added by
    ///    using Rhino's Options command and modifying the contents of the files tab</summary>
    ///<param name="folder">(string) A valid folder, or path, to add</param>
    ///<param name="index">(int) Optional, Zero-based position in the search path list to insert.
    ///    If omitted, path will be appended to the end of the search path list.</param>
    ///<returns>(int) The index where the item was inserted.
    ///    -1 on failure</returns>
    static member AddSearchPath(folder:string, [<OPT;DEF(-1)>]index:int) : int =
        ApplicationSettings.FileSettings.AddSearchPath(folder, index)


    [<Extension>]
    ///<summary>Returns number of command aliases in Rhino</summary>
    ///<returns>(int) the number of command aliases in Rhino</returns>
    static member AliasCount() : int =
        ApplicationSettings.CommandAliasList.Count


    [<Extension>]
    ///<summary>Returns the macro of a command alias</summary>
    ///<param name="alias">(string) The name of an existing command alias</param>
    ///<returns>(string) The existing macro</returns>
    static member AliasMacro(alias:string) : string = //GET
        ApplicationSettings.CommandAliasList.GetMacro(alias)

    [<Extension>]
    ///<summary>Modifies the macro of a command alias</summary>
    ///<param name="alias">(string) The name of an existing command alias</param>
    ///<param name="macro">(string) The new macro to run when the alias is executed.</param>
    ///<returns>(unit) void, nothing</returns>
    static member AliasMacro(alias:string, macro:string) : unit = //SET
        ApplicationSettings.CommandAliasList.SetMacro(alias, macro)
        |> ignore


    [<Extension>]
    ///<summary>Returns a array of command alias names</summary>
    ///<returns>(string array) a array of command alias names</returns>
    static member AliasNames() : array<string> =
        ApplicationSettings.CommandAliasList.GetNames()


    [<Extension>]
    ///<summary>Returns an application interface item's color</summary>
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
    ///    13 = Text hover</returns>
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
        else failwithf "getAppearanceColor: item %d is out of range" item

    [<Extension>]
    ///<summary>Modifies an application interface item's color</summary>
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
    ///<returns>(unit) void, nothing</returns>
    static member AppearanceColor(item:int, color:Drawing.Color) : unit = //SET
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
        else failwithf "setAppearanceColor: item %d is out of range" item
        Doc.Views.Redraw()







    [<Extension>]
    ///<summary>Returns the file name used by Rhino's automatic file saving</summary>
    ///<returns>(string) The name of the current autosave file</returns>
    static member AutosaveFile() : string = //GET
        ApplicationSettings.FileSettings.AutoSaveFile

    [<Extension>]
    ///<summary>Changes the file name used by Rhino's automatic file saving</summary>
    ///<param name="filename">(string) Name of the new autosave file</param>
    ///<returns>(unit) void, nothing</returns>
    static member AutosaveFile(filename:string) : unit = //SET
        ApplicationSettings.FileSettings.AutoSaveFile <- filename


    [<Extension>]
    ///<summary>Returns how often the document will be saved when Rhino's
    /// automatic file saving mechanism is enabled</summary>
    ///<returns>(float) The current interval in minutes</returns>
    static member AutosaveInterval() : float = //GET
        ApplicationSettings.FileSettings.AutoSaveInterval.TotalMinutes

    [<Extension>]
    ///<summary>Changes how often the document will be saved when Rhino's
    /// automatic file saving mechanism is enabled</summary>
    ///<param name="minutes">(float) The number of minutes between saves</param>
    ///<returns>(unit) void, nothing</returns>
    static member AutosaveInterval(minutes:float) : unit = //SET
        ApplicationSettings.FileSettings.AutoSaveInterval <- TimeSpan.FromMinutes(minutes)


    [<Extension>]
    ///<summary>Returns the build date of Rhino</summary>
    ///<returns>(DateTime) the build date of Rhino. Will be converted to a string by most functions</returns>
    static member BuildDate() : DateTime =
        RhinoApp.BuildDate


    [<Extension>]
    ///<summary>Clears contents of Rhino's command history window. You can view the
    ///    command history window by using the CommandHistory command in Rhino</summary>
    ///<returns>(unit) void, nothing</returns>
    static member ClearCommandHistory() : unit =
        RhinoApp.ClearCommandHistoryWindow()


    [<Extension>]
    ///<summary>Runs a Rhino command script. All Rhino commands can be used in command
    ///    scripts. The command can be a built-in Rhino command or one provided by a
    ///    3rd party plug-in.
    ///    Write command scripts just as you would type the command sequence at the
    ///    command line. A space or a new line acts like pressing 'Enter' at the
    ///    command line. For more information, see "Scripting" in Rhino help.
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
    ///    After the command script has run, you can obtain the identifiers of most
    ///    recently created or changed object by calling LastCreatedObjects</summary>
    ///<param name="commandString">(string) A Rhino command including any arguments</param>
    ///<param name="echo">(bool) Optional, Default Value: <c>true</c>
    ///    The command echo mode True will display the commands on the commandline. If omitted, command prompts are echoed (True)</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member Command(commandString:string, [<OPT;DEF(true)>]echo:bool) : bool =
        let getKeepEditor () = 
            //if notNull SeffRhinoWindow then SeffRhinoWindow.Hide() // TODO Add check if already hidden, then dont even hide and show
            let start = DocObjects.RhinoObject.NextRuntimeSerialNumber
            let rc = RhinoApp.RunScript(commandString, echo)
            let ende = DocObjects.RhinoObject.NextRuntimeSerialNumber
            commandSerialNumbers <- None
            if start<>ende then  commandSerialNumbers <- Some(start, ende)
            rc
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Returns the contents of Rhino's command history window</summary>
    ///<returns>(string) the contents of Rhino's command history window</returns>
    static member CommandHistory() : string =
        RhinoApp.CommandHistoryWindowText


    [<Extension>]
    ///<summary>Returns the default render plug-in</summary>
    ///<returns>(string) Name of default renderer</returns>
    static member DefaultRenderer() : string = //GET
        let mutable objectId = Render.Utilities.DefaultRenderPlugInId
        let mutable plugins = PlugIns.PlugIn.GetInstalledPlugIns()
        plugins.[objectId]

    [<Extension>]
    ///<summary>Changes the default render plug-in</summary>
    ///<param name="renderer">(string) The name of the renderer to set as default renderer</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DefaultRenderer(renderer:string) : bool = //SET
        let objectId = Rhino.PlugIns.PlugIn.IdFromName(renderer)
        Rhino.Render.Utilities.SetDefaultRenderPlugIn(objectId)


    [<Extension>]
    ///<summary>Delete an existing alias from Rhino</summary>
    ///<param name="alias">(string) The name of an existing alias</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteAlias(alias:string) : bool =
        ApplicationSettings.CommandAliasList.Delete(alias)


    [<Extension>]
    ///<summary>Removes existing path from Rhino's search path list. Search path items
    ///    can be removed manually by using Rhino's options command and modifying the
    ///    contents of the files tab</summary>
    ///<param name="folder">(string) A folder to remove</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteSearchPath(folder:string) : bool =
        ApplicationSettings.FileSettings.DeleteSearchPath(folder)


    [<Extension>]
    ///<summary>Enables/disables OLE Server Busy/Not Responding dialog boxes</summary>
    ///<param name="enable">(bool) Whether alerts should be visible (True or False)</param>
    ///<returns>(unit) void, nothing</returns>
    static member DisplayOleAlerts(enable:bool) : unit =
        Rhino.Runtime.HostUtils.DisplayOleAlerts( enable )


    [<Extension>]
    ///<summary>Returns edge analysis color displayed by the ShowEdges command</summary>
    ///<returns>(Drawing.Color) The current edge analysis color</returns>
    static member EdgeAnalysisColor() : Drawing.Color= //GET
        ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor

    [<Extension>]
    ///<summary>Modifies edge analysis color displayed by the ShowEdges command</summary>
    ///<param name="color">(Drawing.Color), optional): The new color for the analysis</param>
    ///<returns>(unit) void, nothing</returns>
    static member EdgeAnalysisColor(color:Drawing.Color) : unit = //SET
        ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor <- color


    [<Extension>]
    ///<summary>Returns edge analysis mode displayed by the ShowEdges command</summary>
    ///<returns>(int) The current edge analysis mode
    ///    0 - display all edges
    ///    1 - display naked edges</returns>
    static member EdgeAnalysisMode() : int = //GET
        ApplicationSettings.EdgeAnalysisSettings.ShowEdges

    [<Extension>]
    ///<summary>Modifies edge analysis mode displayed by the ShowEdges command</summary>
    ///<param name="mode">(int) The new display mode. The available modes are
    ///    0 - display all edges
    ///    1 - display naked edges</param>
    ///<returns>(unit) void, nothing</returns>
    static member EdgeAnalysisMode(mode:int) : unit = //SET
        if mode = 1 || mode = 2 then
            ApplicationSettings.EdgeAnalysisSettings.ShowEdges <- mode
        else
            failwithf "bad edge analysisMode %d" mode


    [<Extension>]
    ///<summary>Enables or disables Rhino's automatic file saving mechanism</summary>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///    The autosave state. If omitted automatic saving is enabled (True)</param>
    ///<returns>(unit) void, nothing</returns>
    static member EnableAutosave([<OPT;DEF(true)>]enable:bool) : unit =
        ApplicationSettings.FileSettings.AutoSaveEnabled <- enable


    [<Extension>]
    ///<summary>Get status of a Rhino plug-in</summary>
    ///<param name="plugin">(string) The name of the plugin</param>
    ///<returns>(bool) True if set to load silently otherwise False</returns>
    static member EnablePlugIn(plugin:string) : bool = //GET
        let objectId = PlugIns.PlugIn.IdFromName(plugin)
        let rc, loadSilent = PlugIns.PlugIn.GetLoadProtection(objectId)
        if rc then loadSilent
        else failwithf "EnablePlugIn: %s GetLoadProtection failed" plugin

    [<Extension>]
    ///<summary>Enables or disables a Rhino plug-in</summary>
    ///<param name="plugin">(string) The name of the plugin</param>
    ///<param name="enable">(bool) Load silently if True</param>
    ///<returns>(unit) void, nothing</returns>
    static member EnablePlugIn(plugin:string, enable:bool) : unit = //SET
        let objectId = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        let rc, loadSilent = Rhino.PlugIns.PlugIn.GetLoadProtection(objectId)
        if rc then PlugIns.PlugIn.SetLoadProtection(objectId, enable)
        else failwithf "EnablePlugIn: %s GetLoadProtection failed" plugin




    [<Extension>]
    ///<summary>Returns the full path to Rhino's executable folder</summary>
    ///<returns>(string) the full path to Rhino's executable folder</returns>
    static member ExeFolder() : string =
        ApplicationSettings.FileSettings.ExecutableFolder


    [<Extension>]
    ///<summary>Returns the platform of the Rhino executable</summary>
    ///<returns>(int) 1 for 64 bit, 0 for 32 bit</returns>
    static member ExePlatform() : int =
        if System.Environment.Is64BitProcess then  1 else  0


    [<Extension>]
    ///<summary>Returns the service release number of the Rhino executable</summary>
    ///<returns>(int) the service release number of the Rhino executable</returns>
    static member ExeServiceRelease() : int =
        RhinoApp.ExeServiceRelease


    [<Extension>]
    ///<summary>Returns the major version number of the Rhino executable</summary>
    ///<returns>(int) the major version number of the Rhino executable</returns>
    static member ExeVersion() : int =
        RhinoApp.ExeVersion


    [<Extension>]
    ///<summary>Closes the rhino application</summary>
    ///<returns>(unit) void, nothing</returns>
    static member Exit() : unit =
        RhinoApp.Exit()


    [<Extension>]
    ///<summary>Searches for a file using Rhino's search path. Rhino will look for a file in the following locations:
    ///      1. The current document's folder.
    ///      2. Folder's specified in Options dialog, File tab.
    ///      3. Rhino's System folders</summary>
    ///<param name="filename">(string) A short file name to search for</param>
    ///<returns>(string) full path on success</returns>
    static member FindFile(filename:string) : string =
        ApplicationSettings.FileSettings.FindFile(filename)


    [<Extension>]
    ///<summary>Returns a scriptable object from a specified plug-in. Not all plug-ins
    ///    contain scriptable objects. Check with the manufacturer of your plug-in
    ///    to see if they support this capability</summary>
    ///<param name="plugIn">(string) The name of a registered plug-in that supports scripting.
    ///    If the plug-in is registered but not loaded, it will be loaded</param>
    ///<returns>(object) a scriptable plugin object</returns>
    static member GetPlugInObject(plugIn:string) : obj =
        RhinoApp.GetPlugInObject(plugIn)


    [<Extension>]
    ///<summary>Determines if Rhino is currently running a command. Because Rhino allows
    ///    for transparent commands (commands run from inside of other commands), this
    ///    method returns the total number of active commands</summary>
    ///<returns>(int) the number of active commands</returns>
    static member InCommand() : int = // [<OPT;DEF(true)>]ignoreRunners:bool) : int =
        //<param name="ignoreRunners">(bool) Optional, Default Value: <c>true</c>
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


    [<Extension>]
    ///<summary>The full path to Rhino's installation folder</summary>
    ///<returns>(string) the full path to Rhino's installation folder</returns>
    static member InstallFolder() : string =
        ApplicationSettings.FileSettings.InstallFolder.FullName


    [<Extension>]
    ///<summary>Verifies that a command alias exists in Rhino</summary>
    ///<param name="alias">(string) The name of an existing command alias</param>
    ///<returns>(bool) True if exists or False if the alias does not exist</returns>
    static member IsAlias(alias:string) : bool =
        ApplicationSettings.CommandAliasList.IsAlias(alias)


    [<Extension>]
    ///<summary>Verifies that a command exists in Rhino. Useful when scripting commands
    ///    found in 3rd party plug-ins</summary>
    ///<param name="commandName">(string) The command name to test</param>
    ///<returns>(bool) True if the string is a command or False if it is not a command</returns>
    static member IsCommand(commandName:string) : bool =
        Commands.Command.IsCommand(commandName)


    [<Extension>]
    ///<summary>Verifies that a plug-in is registered</summary>
    ///<param name="plugin">(string) The unique objectId of the plug-in</param>
    ///<returns>(bool) True if the Guid is registered or False if it is not</returns>
    static member IsPlugIn(plugin:string) : bool =
        let objectId = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        if objectId = Guid.Empty then false
        else
            let rc, loaded, loadprot = Rhino.PlugIns.PlugIn.PlugInExists(objectId)
            rc


    [<Extension>]
    ///<summary>Returns True if this script is being executed on a Windows platform</summary>
    ///<returns>(bool) True if currently running on the Widows platform. False if it is not Windows</returns>
    static member IsRunningOnWindows() : bool =
        Rhino.Runtime.HostUtils.RunningOnWindows


    [<Extension>]
    ///<summary>Returns the name of the last executed command</summary>
    ///<returns>(string) the name of the last executed command</returns>
    static member LastCommandName() : string =
        let mutable objectId = Commands.Command.LastCommandId
        Commands.Command.LookupCommandName(objectId, true)


    [<Extension>]
    ///<summary>Returns the result code for the last executed command</summary>
    ///<returns>(int) the result code for the last executed command.
    ///    0 = success (command successfully completed)
    ///    1 = cancel (command was cancelled by the user)
    ///    2 = nothing (command did nothing, but was not cancelled)
    ///    3 = failure (command failed due to bad input, computational problem...)
    ///    4 = unknown command (the command was not found)</returns>
    static member LastCommandResult() : int =
        int(Commands.Command.LastCommandResult)


    [<Extension>]
    ///<summary>Returns the current language used for the Rhino interface.  The current
    ///    language is returned as a locale ID, or LCID, value</summary>
    ///<returns>(int) the current language used for the Rhino interface as a locale ID, or LCID.
    ///    1029  Czech
    ///    1031  German-Germany
    ///    1033  English-United States
    ///    1034  Spanish-Spain
    ///    1036  French-France
    ///    1040  Italian-Italy
    ///    1041  Japanese
    ///    1042  Korean
    ///    1045  Polish</returns>
    static member LocaleID() : int =
        ApplicationSettings.AppearanceSettings.LanguageIdentifier


    [<Extension>]
    ///<summary>Get status of Rhino's ortho modeling aid</summary>
    ///<returns>(bool) The current ortho status</returns>
    static member Ortho() : bool = //GET
        ModelAidSettings.Ortho

    [<Extension>]
    ///<summary>Enables or disables Rhino's ortho modeling aid</summary>
    ///<param name="enable">(bool) The new enabled status</param>
    ///<returns>(unit) void, nothing</returns>
    static member Ortho(enable:bool) : unit = //SET
        ModelAidSettings.Ortho <- enable


    [<Extension>]
    ///<summary>Get status of Rhino's object snap modeling aid.
    ///    Object snaps are tools for specifying points on existing objects</summary>
    ///<returns>(bool) The current osnap status</returns>
    static member Osnap() : bool = //GET
        ModelAidSettings.Osnap

    [<Extension>]
    ///<summary>Enables or disables Rhino's object snap modeling aid.
    ///    Object snaps are tools for specifying points on existing objects</summary>
    ///<param name="enable">(bool) The new enabled status</param>
    ///<returns>(unit) void, nothing</returns>
    static member Osnap(enable:bool) : unit = //SET
        ModelAidSettings.Osnap <- enable


    [<Extension>]
    ///<summary>Get status of Rhino's dockable object snap bar</summary>
    ///<returns>(bool) The current visible state</returns>
    static member OsnapDialog() : bool = //GET
        ModelAidSettings.UseHorizontalDialog

    [<Extension>]
    ///<summary>Shows or hides Rhino's dockable object snap bar</summary>
    ///<param name="visible">(bool) The new visibility state.</param>
    ///<returns>(unit) void, nothing</returns>
    static member OsnapDialog(visible:bool) : unit = //SET
        ModelAidSettings.UseHorizontalDialog <- visible


    [<Extension>]
    ///<summary>Returns the object snap mode. Object snaps are tools for
    /// specifying points on existing objects</summary>
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
    ///    Object snap modes can be added together to set multiple modes</returns>
    static member OsnapMode() : int = //GET
        int(ModelAidSettings.OsnapModes)

    [<Extension>]
    ///<summary>Sets the object snap mode. Object snaps are tools for
    /// specifying points on existing objects</summary>
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
    ///<returns>(unit) void, nothing</returns>
    static member OsnapMode(mode:int) : unit = //SET
        ModelAidSettings.OsnapModes <- LanguagePrimitives.EnumOfValue mode


    [<Extension>]
    ///<summary>Get status of Rhino's planar modeling aid</summary>
    ///<returns>(bool) The current planar status</returns>
    static member Planar() : bool = //GET
        ModelAidSettings.Planar

    [<Extension>]
    ///<summary>Enables or disables Rhino's planar modeling aid</summary>
    ///<param name="enable">(bool) The new enable status.</param>
    ///<returns>(unit) void, nothing</returns>
    static member Planar(enable:bool) : unit = //SET
        ModelAidSettings.Planar <- enable


    [<Extension>]
    ///<summary>Returns the identifier of a plug-in given the plug-in name</summary>
    ///<param name="plugin">(string) the name  of the plug-in</param>
    ///<returns>(Guid) the  Unique Guid of the plug-in</returns>
    static member PlugInId(plugin:string) : Guid =
        let objectId = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        if objectId<>Guid.Empty then  objectId
        else failwithf "Plugin %s not found" plugin


    [<Extension>]
    ///<summary>Returns a array of registered Rhino plug-ins</summary>
    ///<param name="types">(int) Optional, Default Value: <c>0</c>
    ///    The type of plug-ins to return.
    ///    0= all
    ///    1= render
    ///    2= file export
    ///    4= file import
    ///    8= digitizer
    ///    16= utility.
    ///    If omitted, all are returned</param>
    ///<param name="status">(int) Optional, Default Value: <c>0</c>
    /// 0= both loaded and unloaded, 
    /// 1= loaded, 
    /// 2= unloaded.  If omitted both status is returned</param>
    ///<returns>(string array) array of registered Rhino plug-ins</returns>
    static member PlugIns([<OPT;DEF(0)>]types:int, [<OPT;DEF(0)>]status:int) : array<string> =
        let mutable filter = Rhino.PlugIns.PlugInType.Any
        if types = 1 then  filter <- Rhino.PlugIns.PlugInType.Render
        if types = 2 then  filter <- Rhino.PlugIns.PlugInType.FileExport
        if types = 4 then  filter <- Rhino.PlugIns.PlugInType.FileImport
        if types = 8 then  filter <- Rhino.PlugIns.PlugInType.Digitizer
        if types = 16 then filter <- Rhino.PlugIns.PlugInType.Utility
        let loaded = status = 0 || status = 1
        let unloaded = status = 0 || status = 2
        Rhino.PlugIns.PlugIn.GetInstalledPlugInNames(filter, loaded, unloaded)


    [<Extension>]
    ///<summary>Get status of object snap projection</summary>
    ///<returns>(bool) the current object snap projection status</returns>
    static member ProjectOsnaps() : bool = //GET
        ModelAidSettings.ProjectSnapToCPlane

    [<Extension>]
    ///<summary>Enables or disables object snap projection</summary>
    ///<param name="enable">(bool) The new enabled status.</param>
    static member ProjectOsnaps(enable:bool) : unit = //SET
        ModelAidSettings.ProjectSnapToCPlane <- enable


    [<Extension>]
    ///<summary>Change Rhino's command window prompt</summary>
    ///<param name="message">(string) The new prompt on the commandline</param>
    ///<returns>(unit) void, nothing</returns>
    static member Prompt(message:string) : unit =
        RhinoApp.SetCommandPrompt(message)


    [<Extension>]
    ///<summary>Returns current width and height, of the screen of the primary monitor</summary>
    ///<returns>(int * int) containing two numbers identifying the width and height in pixels</returns>
    static member ScreenSize() : int * int =
        let sz = System.Windows.Forms.Screen.PrimaryScreen.Bounds
        sz.Width, sz.Height


    [<Extension>]
    ///<summary>Returns version of the Rhino SDK supported by the executing Rhino</summary>
    ///<returns>(int) the version of the Rhino SDK supported by the executing Rhino. Rhino SDK versions are 9 digit numbers in the form of YYYYMMDDn</returns>
    static member SdkVersion() : int =
        RhinoApp.SdkVersion


    [<Extension>]
    ///<summary>Returns the number of path items in Rhino's search path list.
    ///    See "Options Files settings" in the Rhino help file for more details</summary>
    ///<returns>(int) the number of path items in Rhino's search path list</returns>
    static member SearchPathCount() : int =
        ApplicationSettings.FileSettings.SearchPathCount


    [<Extension>]
    ///<summary>Returns all of the path items in Rhino's search path list.
    ///    See "Options Files settings" in the Rhino help file for more details</summary>
    ///<returns>(string array) list of search paths</returns>
    static member SearchPathList() : array<string> =
        ApplicationSettings.FileSettings.GetSearchPaths()


    [<Extension>]
    ///<summary>Sends a string of printable characters to Rhino's command line</summary>
    ///<param name="keys">(string) A string of characters to send to the command line</param>
    ///<param name="addReturn">(bool) Optional, Default Value: <c>true</c>
    ///    Append a return character to the end of the string. If omitted an return character will be added (True)</param>
    ///<returns>(unit) void, nothing</returns>
    static member SendKeystrokes(keys:string, [<OPT;DEF(true)>]addReturn:bool) : unit =
        RhinoApp.SendKeystrokes(keys, addReturn)


    [<Extension>]
    ///<summary>Get status of Rhino's grid snap modeling aid</summary>
    ///<returns>(bool) the current grid snap status</returns>
    static member Snap() : bool = //GET
        ModelAidSettings.GridSnap

    [<Extension>]
    ///<summary>Enables or disables Rhino's grid snap modeling aid</summary>
    ///<param name="enable">(bool) The new enabled status.</param>
    static member Snap(enable:bool) : unit = //SET
        ModelAidSettings.GridSnap <- enable


    [<Extension>]
    ///<summary>Sets Rhino's status bar distance pane</summary>
    ///<param name="distance">(float) The distance to set the status bar</param>
    ///<returns>(unit) void, nothing</returns>
    static member StatusBarDistance(distance:float) : unit =
        UI.StatusBar.SetDistancePane(distance)


    [<Extension>]
    ///<summary>Sets Rhino's status bar message pane</summary>
    ///<param name="message">(string) The message to display</param>
    ///<returns>(unit) void, nothing</returns>
    static member StatusBarMessage(message:string) : unit =
        UI.StatusBar.SetMessagePane(message)


    [<Extension>]
    ///<summary>Sets Rhino's status bar point coordinate pane</summary>
    ///<param name="point">(Point3d) The 3d coordinates of the status bar</param>
    ///<returns>(unit) void, nothing</returns>
    static member StatusBarPoint(point:Point3d) : unit =
        UI.StatusBar.SetPointPane(point)


    [<Extension>]
    ///<summary>Start the Rhino status bar progress meter</summary>
    ///<param name="label">(string) Short description of the progesss</param>
    ///<param name="lower">(int) Lower limit of the progress meter's range</param>
    ///<param name="upper">(int) Upper limit of the progress meter's range</param>
    ///<param name="embedLabel">(bool) Optional, Default Value: <c>true</c>
    ///    If true, the label will show inside the meter.
    ///    If false, the label will show to the left of the meter</param>
    ///<param name="showPercent">(bool) Optional, Default Value: <c>true</c>
    ///    Show the percent complete if True</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member StatusBarProgressMeterShow(label:string, lower:int, upper:int, [<OPT;DEF(true)>]embedLabel:bool, [<OPT;DEF(true)>]showPercent:bool) : bool =
        let mutable rc = UI.StatusBar.ShowProgressMeter(lower, upper, label, embedLabel, showPercent)
        rc = 1


    [<Extension>]
    ///<summary>Set the current position of the progress meter</summary>
    ///<param name="position">(int) The new position in the progress meter</param>
    ///<param name="absolute">(bool) Optional, Default Value: <c>true</c>
    ///    The position is set absolute (True) or relative (False) to its current position. If omitted the absolute (True) is used</param>
    ///<returns>(unit) void, nothing</returns>
    static member StatusBarProgressMeterUpdate(position:int, [<OPT;DEF(true)>]absolute:bool) : unit =
        UI.StatusBar.UpdateProgressMeter(position, absolute)
        |> ignore


    [<Extension>]
    ///<summary>Hide the progress meter</summary>
    ///<returns>(unit) void, nothing</returns>
    static member StatusBarProgressMeterHide() : unit =
        UI.StatusBar.HideProgressMeter()


    [<Extension>]
    ///<summary>Returns Rhino's default template file. This is the file used
    /// when Rhino starts</summary>
    ///<returns>(string) The current default template file</returns>
    static member TemplateFile() : string = //GET
        ApplicationSettings.FileSettings.TemplateFile

    [<Extension>]
    ///<summary>Sets Rhino's default template file. This is the file used
    /// when Rhino starts</summary>
    ///<param name="filename">(string) The name of the new default template file.</param>
    ///<returns>(unit) void, nothing</returns>
    static member TemplateFile(filename:string) : unit = //SET
        ApplicationSettings.FileSettings.TemplateFile <- filename


    [<Extension>]
    ///<summary>Returns the location of Rhino's template folder</summary>
    ///<returns>(string) The current template file folder</returns>
    static member TemplateFolder() : string = //GET
        ApplicationSettings.FileSettings.TemplateFolder

    [<Extension>]
    ///<summary>Sets the location of Rhino's template folder</summary>
    ///<param name="folder">(string) The location of Rhino's template files. Note, the location must exist</param>
    ///<returns>(unit) void, nothing</returns>
    static member TemplateFolder(folder:string) : unit = //SET
        ApplicationSettings.FileSettings.TemplateFolder <- folder


    [<Extension>]
    ///<summary>Returns the windows handle of Rhino's main window</summary>
    ///<returns>(IntPtr) the Window's handle of Rhino's main window. IntPtr is a platform-specific type that is used to represent a pointer or a handle</returns>
    static member WindowHandle() : IntPtr =
        RhinoApp.MainWindowHandle()


    [<Extension>]
    ///<summary>Returns Rhino's working folder (directory).
    /// The working folder is the default folder for all file operations</summary>
    ///<returns>(string) The current working folder</returns>
    static member WorkingFolder() : string = //GET
        ApplicationSettings.FileSettings.WorkingFolder

    [<Extension>]
    ///<summary>Sets Rhino's working folder (directory).
    /// The working folder is the default folder for all file operations</summary>
    ///<param name="folder">(string) The new working folder for the current Rhino session</param>
    ///<returns>(unit) void, nothing</returns>
    static member WorkingFolder(folder:string) : unit = //SET
        ApplicationSettings.FileSettings.WorkingFolder <- folder


