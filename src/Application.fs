namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsApplication =
  type RhinoScriptSyntax with
    
    ///<summary>Add new command alias to Rhino. Command aliases can be added manually by
    ///  using Rhino's Options command and modifying the contents of the Aliases tab.</summary>
    ///<param name="alias">(string) Name of new command alias. Cannot match command names or existing
    ///  aliases.</param>
    ///<param name="macro">(string) The macro to run when the alias is executed.</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member AddAlias(alias:string, macro:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Add new path to Rhino's search path list. Search paths can be added by
    ///  using Rhino's Options command and modifying the contents of the files tab.</summary>
    ///<param name="folder">(string) A valid folder, or path, to add.</param>
    ///<param name="index">(int) Optional, Default Value: <c>-1</c>
    ///Zero-based position in the search path list to insert.
    ///  If omitted, path will be appended to the end of the
    ///  search path list.</param>
    ///<returns>(float) The index where the item was inserted if success.
    ///  -1 on failure.</returns>
    static member AddSearchPath(folder:string, [<OPT;DEF(-1)>]index:int) : float =
        failNotImpl () // done in 2018


    ///<summary>Returns number of command aliases in Rhino.</summary>
    ///<returns>(int) the number of command aliases in Rhino.</returns>
    static member AliasCount() : int =
        failNotImpl () // done in 2018


    ///<summary>Returns the macro of a command alias.</summary>
    ///<param name="alias">(string) The name of an existing command alias.</param>
    ///<returns>(string) The existing macro .</returns>
    static member AliasMacro(alias:string) : string =
        failNotImpl () // done in 2018

    ///<summary>Modifies the macro of a command alias.</summary>
    ///<param name="alias">(string) The name of an existing command alias.</param>
    ///<param name="macro">(string)The new macro to run when the alias is executed. If omitted, the current alias macro is returned.</param>
    ///<returns>(unit) unit</returns>
    static member AliasMacro(alias:string, macro:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns a list of command alias names.</summary>
    ///<returns>(string seq) a list of command alias names.</returns>
    static member AliasNames() : string seq =
        failNotImpl () // done in 2018


    ///<summary>Returns an application interface item's color.</summary>
    ///<param name="item">(int) Item number to either query or modify
    ///  0  = View background
    ///  1  = Major grid line
    ///  2  = Minor grid line
    ///  3  = X-Axis line
    ///  4  = Y-Axis line
    ///  5  = Selected Objects
    ///  6  = Locked Objects
    ///  7  = New layers
    ///  8  = Feedback
    ///  9  = Tracking
    ///  10 = Crosshair
    ///  11 = Text
    ///  12 = Text Background
    ///  13 = Text hover</param>
    ///<returns>(int) The current item color.
    ///  0  = View background
    ///  1  = Major grid line
    ///  2  = Minor grid line
    ///  3  = X-Axis line
    ///  4  = Y-Axis line
    ///  5  = Selected Objects
    ///  6  = Locked Objects
    ///  7  = New layers
    ///  8  = Feedback
    ///  9  = Tracking
    ///  10 = Crosshair
    ///  11 = Text
    ///  12 = Text Background
    ///  13 = Text hover</returns>
    static member AppearanceColor(item:int) : int =
        failNotImpl () // done in 2018

    ///<summary>Modifies an application interface item's color.</summary>
    ///<param name="item">(int) Item number to either query or modify
    ///  0  = View background
    ///  1  = Major grid line
    ///  2  = Minor grid line
    ///  3  = X-Axis line
    ///  4  = Y-Axis line
    ///  5  = Selected Objects
    ///  6  = Locked Objects
    ///  7  = New layers
    ///  8  = Feedback
    ///  9  = Tracking
    ///  10 = Crosshair
    ///  11 = Text
    ///  12 = Text Background
    ///  13 = Text hover</param>
    ///<param name="color">(int * int * int)The new color value in (r255,g255,b255). If omitted, the current item color is returned.</param>
    ///<returns>(unit) unit</returns>
    static member AppearanceColor(item:int, color:int * int * int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the file name used by Rhino's automatic file saving</summary>
    ///<returns>(string) The name of the current autosave file</returns>
    static member AutosaveFile() : string =
        failNotImpl () // done in 2018

    ///<summary>Changes the file name used by Rhino's automatic file saving</summary>
    ///<param name="filename">(string)Name of the new autosave file</param>
    ///<returns>(unit) unit</returns>
    static member AutosaveFile(filename:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns how often the document will be saved when Rhino's
    /// automatic file saving mechanism is enabled</summary>
    ///<returns>(float) The current interval in minutes</returns>
    static member AutosaveInterval() : float =
        failNotImpl () // done in 2018

    ///<summary>Changes how often the document will be saved when Rhino's
    /// automatic file saving mechanism is enabled</summary>
    ///<param name="minutes">(float)The number of minutes between saves</param>
    ///<returns>(unit) unit</returns>
    static member AutosaveInterval(minutes:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the build date of Rhino</summary>
    ///<returns>(DateTime) the build date of Rhino. Will be converted to a string by most functions.</returns>
    static member BuildDate() : DateTime =
        failNotImpl () // done in 2018


    ///<summary>Clears contents of Rhino's command history window. You can view the
    ///  command history window by using the CommandHistory command in Rhino.</summary>
    ///<returns>(unit) </returns>
    static member ClearCommandHistory() : unit =
        failNotImpl () // done in 2018


    ///<summary>Runs a Rhino command script. All Rhino commands can be used in command
    ///  scripts. The command can be a built-in Rhino command or one provided by a
    ///  3rd party plug-in.
    ///  Write command scripts just as you would type the command sequence at the
    ///  command line. A space or a new line acts like pressing <Enter> at the
    ///  command line. For more information, see "Scripting" in Rhino help.
    ///  Note, this function is designed to run one command and one command only.
    ///  Do not combine multiple Rhino commands into a single call to this method.
    ///    WRONG:
    ///      rs.Command("_Line _SelLast _Invert")
    ///    CORRECT:
    ///      rs.Command("_Line")
    ///      rs.Command("_SelLast")
    ///      rs.Command("_Invert")
    ///  Also, the exclamation point and space character ( ! ) combination used by
    ///  button macros and batch-driven scripts to cancel the previous command is
    ///  not valid.
    ///    WRONG:
    ///      rs.Command("! _Line _Pause _Pause")
    ///    CORRECT:
    ///      rs.Command("_Line _Pause _Pause")
    ///  After the command script has run, you can obtain the identifiers of most
    ///  recently created or changed object by calling LastCreatedObjects.</summary>
    ///<param name="commandString">(string) A Rhino command including any arguments</param>
    ///<param name="echo">(bool) Optional, Default Value: <c>true</c>
    ///The command echo mode True will display the commands on the commandline. If omitted, command prompts are echoed (True)</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member Command(commandString:string, [<OPT;DEF(true)>]echo:bool) : bool =
        failNotImpl () // done in 2018


    ///<summary>Returns the contents of Rhino's command history window</summary>
    ///<returns>(string) the contents of Rhino's command history window</returns>
    static member CommandHistory() : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the default render plug-in</summary>
    ///<returns>(Guid) Unique identifier of default renderer</returns>
    static member DefaultRenderer() : Guid =
        failNotImpl () // done in 2018

    ///<summary>Changes the default render plug-in</summary>
    ///<param name="rendeerer">(string)The name of the rendeerer to set as default rendeerer.  If omitted the Guid of the current rendeerer is returned.</param>
    ///<returns>(Guid) Unique identifier of default renderer</returns>
    static member DefaultRenderer(rendeerer:string) : Guid =
        failNotImpl () // done in 2018


    ///<summary>Delete an existing alias from Rhino.</summary>
    ///<param name="alias">(string) The name of an existing alias.</param>
    ///<returns>(bool) True or False indicating success</returns>
    static member DeleteAlias(alias:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Removes existing path from Rhino's search path list. Search path items
    ///  can be removed manually by using Rhino's options command and modifying the
    ///  contents of the files tab</summary>
    ///<param name="folder">(string) A folder to remove</param>
    ///<returns>(bool) True or False indicating success</returns>
    static member DeleteSearchPath(folder:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Enables/disables OLE Server Busy/Not Responding dialog boxes</summary>
    ///<param name="enable">(bool) Whether alerts should be visible (True or False)</param>
    ///<returns>(unit) </returns>
    static member DisplayOleAlerts(enable:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns edge analysis color displayed by the ShowEdges command</summary>
    ///<returns>(int * int * int) The current edge analysis color</returns>
    static member EdgeAnalysisColor() : int * int * int =
        failNotImpl () // done in 2018

    ///<summary>Modifies edge analysis color displayed by the ShowEdges command</summary>
    ///<param name="color">(int * int * int), optional): The new color for the analysis.</param>
    ///<returns>(unit) unit</returns>
    static member EdgeAnalysisColor(color:int * int * int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns edge analysis mode displayed by the ShowEdges command</summary>
    ///<returns>(int) The current edge analysis mode
    ///  0 - display all edges
    ///  1 - display naked edges</returns>
    static member EdgeAnalysisMode() : int =
        failNotImpl () // done in 2018

    ///<summary>Modifies edge analysis mode displayed by the ShowEdges command</summary>
    ///<param name="mode">(float)The new display mode. The available modes are
    ///  0 - display all edges
    ///  1 - display naked edges</param>
    ///<returns>(unit) unit</returns>
    static member EdgeAnalysisMode(mode:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Enables or disables Rhino's automatic file saving mechanism</summary>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///The autosave state. If omitted automatic saving is enabled (True)</param>
    ///<returns>(unit) unit</returns>
    static member EnableAutosave([<OPT;DEF(true)>]enable:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Get status of a Rhino plug-in</summary>
    ///<param name="plugin">(Guid) The unique Guid id of the plugin.</param>
    ///<returns>(bool) True if set to load silently otherwise False</returns>
    static member EnablePlugIn(plugin:Guid) : bool =
        failNotImpl () // done in 2018

    ///<summary>Enables or disables a Rhino plug-in</summary>
    ///<param name="plugin">(Guid) The unique Guid id of the plugin.</param>
    ///<param name="enable">(bool)Load silently if True. If omitted Load silently is False.</param>
    static member EnablePlugIn(plugin:Guid, enable:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the full path to Rhino's executable folder.</summary>
    ///<returns>(string) the full path to Rhino's executable folder.</returns>
    static member ExeFolder() : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the platform of the Rhino executable</summary>
    ///<returns>(string) the platform of the Rhino executable</returns>
    static member ExePlatform() : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the service release number of the Rhino executable</summary>
    ///<returns>(string) the service release number of the Rhino executable</returns>
    static member ExeServiceRelease() : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the major version number of the Rhino executable</summary>
    ///<returns>(string) the major version number of the Rhino executable</returns>
    static member ExeVersion() : string =
        failNotImpl () // done in 2018


    ///<summary>Closes the rhino application</summary>
    ///<returns>(unit) </returns>
    static member Exit() : unit =
        failNotImpl () // done in 2018


    ///<summary>Searches for a file using Rhino's search path. Rhino will look for a
    ///  file in the following locations:
    ///    1. The current document's folder.
    ///    2. Folder's specified in Options dialog, File tab.
    ///    3. Rhino's System folders</summary>
    ///<param name="filename">(string) A short file name to search for</param>
    ///<returns>(string) full path on success</returns>
    static member FindFile(filename:string) : string =
        failNotImpl () // done in 2018


    ///<summary>Returns a scriptable object from a specified plug-in. Not all plug-ins
    ///  contain scriptable objects. Check with the manufacturer of your plug-in
    ///  to see if they support this capability.</summary>
    ///<param name="plugIn">(string) The name or Id of a registered plug-in that supports scripting.
    ///  If the plug-in is registered but not loaded, it will be loaded</param>
    ///<returns>(Guid) scriptable object</returns>
    static member GetPlugInObject(plugIn:string) : Guid =
        failNotImpl () // done in 2018


    ///<summary>Determines if Rhino is currently running a command. Because Rhino allows
    ///  for transparent commands (commands run from inside of other commands), this
    ///  method returns the total number of active commands.</summary>
    ///<param name="ignoreRunners">(bool) Optional, Default Value: <c>true</c>
    ///If True, script running commands, such as
    ///  LoadScript, RunScript, and ReadCommandFile will not counted.
    ///  If omitted the default is not to count script running command (True).</param>
    ///<returns>(float) the number of active commands</returns>
    static member InCommand([<OPT;DEF(true)>]ignoreRunners:bool) : float =
        failNotImpl () // done in 2018


    ///<summary>The full path to Rhino's installation folder</summary>
    ///<returns>(string) the full path to Rhino's installation folder</returns>
    static member InstallFolder() : string =
        failNotImpl () // done in 2018


    ///<summary>Verifies that a command alias exists in Rhino</summary>
    ///<param name="alias">(string) The name of an existing command alias</param>
    ///<returns>(bool) True if exists or False if the alias does not exist.</returns>
    static member IsAlias(alias:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies that a command exists in Rhino. Useful when scripting commands
    ///  found in 3rd party plug-ins.</summary>
    ///<param name="commandName">(string) The command name to test</param>
    ///<returns>(bool) True if the string is a command or False if it is not a command.</returns>
    static member IsCommand(commandName:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies that a plug-in is registered</summary>
    ///<param name="plugin">(Guid) The unique id of the plug-in</param>
    ///<returns>(bool) True if the Guid is registered or False if it is not.</returns>
    static member IsPlugIn(plugin:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Returns True if this script is being executed on a Windows platform</summary>
    ///<returns>(bool) True if currently running on the Widows platform. False if it is not Windows.</returns>
    static member IsRunningOnWindows() : bool =
        failNotImpl () // done in 2018


    ///<summary>Returns the name of the last executed command</summary>
    ///<returns>(string) the name of the last executed command</returns>
    static member LastCommandName() : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the result code for the last executed command</summary>
    ///<returns>(float) the result code for the last executed command.
    ///  0 = success (command successfully completed)
    ///  1 = cancel (command was cancelled by the user)
    ///  2 = nothing (command did nothing, but was not cancelled)
    ///  3 = failure (command failed due to bad input, computational problem...)
    ///  4 = unknown command (the command was not found)</returns>
    static member LastCommandResult() : float =
        failNotImpl () // done in 2018


    ///<summary>Returns the current language used for the Rhino interface.  The current
    ///  language is returned as a locale ID, or LCID, value.</summary>
    ///<returns>(float) the current language used for the Rhino interface as a locale ID, or LCID.
    ///  1029  Czech
    ///  1031  German-Germany
    ///  1033  English-United States
    ///  1034  Spanish-Spain
    ///  1036  French-France
    ///  1040  Italian-Italy
    ///  1041  Japanese
    ///  1042  Korean
    ///  1045  Polish</returns>
    static member LocaleID() : float =
        failNotImpl () // done in 2018


    ///<summary>Get status of Rhino's ortho modeling aid.</summary>
    ///<returns>(bool) The current ortho status</returns>
    static member Ortho() : bool =
        failNotImpl () // done in 2018

    ///<summary>Enables or disables Rhino's ortho modeling aid.</summary>
    ///<param name="enable">(bool)The new enabled status (True or False). If omitted the current state is returned.</param>
    ///<returns>(unit) unit</returns>
    static member Ortho(enable:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Get status of Rhino's object snap modeling aid.
    ///  Object snaps are tools for specifying points on existing objects.</summary>
    ///<returns>(bool) The current osnap status</returns>
    static member Osnap() : bool =
        failNotImpl () // done in 2018

    ///<summary>Enables or disables Rhino's object snap modeling aid.
    ///  Object snaps are tools for specifying points on existing objects.</summary>
    ///<param name="enable">(bool)The new enabled status.</param>
    ///<returns>(unit) unit</returns>
    static member Osnap(enable:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Get status of Rhino's dockable object snap bar</summary>
    ///<returns>(bool) The current visible state</returns>
    static member OsnapDialog() : bool =
        failNotImpl () // done in 2018

    ///<summary>Shows or hides Rhino's dockable object snap bar</summary>
    ///<param name="visible">(bool)The new visibility state. If omitted then the current state is returned.</param>
    ///<returns>(unit) unit</returns>
    static member OsnapDialog(visible:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the object snap mode. Object snaps are tools for
    /// specifying points on existing objects</summary>
    ///<returns>(int) The current object snap mode(s)
    ///  0          None
    ///  2          Near
    ///  8          Focus
    ///  32         Center
    ///  64         Vertex
    ///  128        Knot
    ///  512        Quadrant
    ///  2048       Midpoint
    ///  8192       Intersection
    ///  131072     End
    ///  524288     Perpendicular
    ///  2097152    Tangent
    ///  134217728  Point
    ///  Object snap modes can be added together to set multiple modes</returns>
    static member OsnapMode() : int =
        failNotImpl () // done in 2018

    ///<summary>Sets the object snap mode. Object snaps are tools for
    /// specifying points on existing objects</summary>
    ///<param name="mode">(float)The object snap mode or modes to set.
    ///  0          None
    ///  2          Near
    ///  8          Focus
    ///  32         Center
    ///  64         Vertex
    ///  128        Knot
    ///  512        Quadrant
    ///  2048       Midpoint
    ///  8192       Intersection
    ///  131072     End
    ///  524288     Perpendicular
    ///  2097152    Tangent
    ///  134217728  Point
    ///  Object snap modes can be added together to set multiple modes</param>
    ///<returns>(unit) unit</returns>
    static member OsnapMode(mode:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Get status of Rhino's planar modeling aid</summary>
    ///<returns>(bool) The current planar status</returns>
    static member Planar() : bool =
        failNotImpl () // done in 2018

    ///<summary>Enables or disables Rhino's planar modeling aid</summary>
    ///<param name="enable">(bool)The new enable status.  If omitted the current state is returned.</param>
    ///<returns>(unit) unit</returns>
    static member Planar(enable:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the identifier of a plug-in given the plug-in name</summary>
    ///<param name="plugin">(Guid) Unique id of the plug-in</param>
    ///<returns>(Guid) the id of the plug-in</returns>
    static member PlugInId(plugin:Guid) : Guid =
        failNotImpl () // done in 2018


    ///<summary>Returns a list of registered Rhino plug-ins</summary>
    ///<param name="typs">(int) Optional, Default Value: <c>0</c>
    ///The type of plug-ins to return.
    ///  0=all
    ///  1=render
    ///  2=file export
    ///  4=file import
    ///  8=digitizer
    ///  16=utility.
    ///  If omitted, all are returned.</param>
    ///<param name="status">(int) Optional, Default Value: <c>0</c>
    ///0=both loaded and unloaded, 1=loaded, 2=unloaded.  If omitted both status is returned.</param>
    ///<returns>(string seq) list of registered Rhino plug-ins</returns>
    static member PlugIns([<OPT;DEF(0)>]typs:int, [<OPT;DEF(0)>]status:int) : string seq =
        failNotImpl () // done in 2018


    ///<summary>Get status of object snap projection</summary>
    ///<returns>(bool) the current object snap projection status</returns>
    static member ProjectOsnaps() : bool =
        failNotImpl () // done in 2018

    ///<summary>Enables or disables object snap projection</summary>
    ///<param name="enable">(bool)The new enabled status.  If omitted the current status is returned.</param>
    static member ProjectOsnaps(enable:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Change Rhino's command window prompt</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///The new prompt on the commandline.  If omitted the prompt will be blank.</param>
    ///<returns>(unit) </returns>
    static member Prompt([<OPT;DEF(null)>]message:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns current width and height, of the screen of the primary monitor.</summary>
    ///<returns>(float * float) containing two numbers identifying the width and height in pixels</returns>
    static member ScreenSize() : float * float =
        failNotImpl () // done in 2018


    ///<summary>Returns version of the Rhino SDK supported by the executing Rhino.</summary>
    ///<returns>(string) the version of the Rhino SDK supported by the executing Rhino. Rhino SDK versions are 9 digit numbers in the form of YYYYMMDDn.</returns>
    static member SdkVersion() : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the number of path items in Rhino's search path list.
    ///  See "Options Files settings" in the Rhino help file for more details.</summary>
    ///<returns>(int) the number of path items in Rhino's search path list</returns>
    static member SearchPathCount() : int =
        failNotImpl () // done in 2018


    ///<summary>Returns all of the path items in Rhino's search path list.
    ///  See "Options Files settings" in the Rhino help file for more details.</summary>
    ///<returns>(string seq) list of search paths</returns>
    static member SearchPathList() : string seq =
        failNotImpl () // done in 2018


    ///<summary>Sends a string of printable characters to Rhino's command line</summary>
    ///<param name="keys">(string) Optional, Default Value: <c>null</c>
    ///A string of characters to send to the command line.</param>
    ///<param name="addReturn">(bool) Optional, Default Value: <c>true</c>
    ///Append a return character to the end of the string. If omitted an return character will be added (True)</param>
    ///<returns>(unit) </returns>
    static member SendKeystrokes([<OPT;DEF(null)>]keys:string, [<OPT;DEF(true)>]addReturn:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Get status of Rhino's grid snap modeling aid</summary>
    ///<returns>(bool) the current grid snap status</returns>
    static member Snap() : bool =
        failNotImpl () // done in 2018

    ///<summary>Enables or disables Rhino's grid snap modeling aid</summary>
    ///<param name="enable">(bool)The new enabled status. If omitted the current status is returned.</param>
    static member Snap(enable:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Sets Rhino's status bar distance pane</summary>
    ///<param name="distance">(int) Optional, Default Value: <c>0</c>
    ///The distance to set the status bar.  If omitted the distance will be set to 0.</param>
    ///<returns>(unit) </returns>
    static member StatusBarDistance([<OPT;DEF(0)>]distance:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Sets Rhino's status bar message pane</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///The message to display.</param>
    ///<returns>(unit) </returns>
    static member StatusBarMessage([<OPT;DEF(null)>]message:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Sets Rhino's status bar point coordinate pane</summary>
    ///<param name="point">(Point3d) Optional, Default Value: <c>null</c>
    ///The 3d coordinates of the status bar.  If omitted the current poition is set to (0,0,0).</param>
    ///<returns>(unit) </returns>
    static member StatusBarPoint([<OPT;DEF(null)>]point:Point3d) : unit =
        failNotImpl () // done in 2018


    ///<summary>Start the Rhino status bar progress meter</summary>
    ///<param name="label">(string) Short description of the progesss</param>
    ///<param name="lower">(string) Lower limit of the progress meter's range</param>
    ///<param name="upper">(string) Upper limit of the progress meter's range</param>
    ///<param name="embedLabel">(bool) Optional, Default Value: <c>true</c>
    ///If True, the label will show inside the meter.
    ///  If false, the label will show to the left of the meter.
    ///  If omitted the label will show inside the meter (True)</param>
    ///<param name="showPercent">(bool) Optional, Default Value: <c>true</c>
    ///Show the percent complete if True. If omitted the percnetage will be shown (True)</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member StatusBarProgressMeterShow(label:string, lower:string, upper:string, [<OPT;DEF(true)>]embedLabel:bool, [<OPT;DEF(true)>]showPercent:bool) : bool =
        failNotImpl () // done in 2018


    ///<summary>Set the current position of the progress meter</summary>
    ///<param name="position">(int) The new position in the progress meter</param>
    ///<param name="absolute">(bool) Optional, Default Value: <c>true</c>
    ///The position is set absolute (True) or relative (False) to its current position. If omitted the absolute (True) is used.</param>
    ///<returns>(unit) unit</returns>
    static member StatusBarProgressMeterUpdate(position:int, [<OPT;DEF(true)>]absolute:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Hide the progress meter</summary>
    ///<returns>(unit) </returns>
    static member StatusBarProgressMeterHide() : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns Rhino's default template file. This is the file used
    /// when Rhino starts.</summary>
    ///<returns>(string) The current default template file</returns>
    static member TemplateFile() : string =
        failNotImpl () // done in 2018

    ///<summary>Sets Rhino's default template file. This is the file used
    /// when Rhino starts.</summary>
    ///<param name="filename">(string)The name of the new default template file. If omitted the current default template name is returned.</param>
    ///<returns>(unit) unit</returns>
    static member TemplateFile(filename:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the location of Rhino's template folder</summary>
    ///<returns>(string) The current template file folder</returns>
    static member TemplateFolder() : string =
        failNotImpl () // done in 2018

    ///<summary>Sets the location of Rhino's template folder</summary>
    ///<param name="folder">(string)The location of Rhino's template files. Note, the location must exist.</param>
    ///<returns>(unit) unit</returns>
    static member TemplateFolder(folder:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the windows handle of Rhino's main window</summary>
    ///<returns>(IntPtr) the Window's handle of Rhino's main window. IntPtr is a platform-specific type that is used to represent a pointer or a handle.</returns>
    static member WindowHandle() : IntPtr =
        failNotImpl () // done in 2018


    ///<summary>Returns Rhino's working folder (directory).
    /// The working folder is the default folder for all file operations.</summary>
    ///<returns>(string) The current working folder</returns>
    static member WorkingFolder() : string =
        failNotImpl () // done in 2018

    ///<summary>Sets Rhino's working folder (directory).
    /// The working folder is the default folder for all file operations.</summary>
    ///<param name="folder">(string)The new working folder for the current Rhino session.</param>
    ///<returns>(unit) unit</returns>
    static member WorkingFolder(folder:string) : unit =
        failNotImpl () // done in 2018


