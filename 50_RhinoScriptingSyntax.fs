
namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry

[<Sealed>]
type RhinoScriptingSyntax private () = 

    ///<summary>Add new command alias to Rhino. Command aliases can be added manually by
    ///    using Rhino's Options command and modifying the contents of the Aliases tab.</summary>
    ///<param name="alias">(string) Name of new command alias. Cannot match command names or existing
    ///              aliases.</param>
    ///<param name="macro">(string) The macro to run when the alias is executed.</param>
    ///
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member AddAlias(alias:string, macro:string) : bool =
        Application.addAlias macro alias

    ///<summary>Add new path to Rhino's search path list. Search paths can be added by
    ///    using Rhino's Options command and modifying the contents of the files tab.</summary>
    ///<param name="folder">(string) A valid folder, or path, to add.</param>
    ///<param name="index">(int) Optional, Default Value:-1, Zero-based position in the search path list to insert.
    ///                             If omitted, path will be appended to the end of the
    ///                             search path list.</param>
    ///
    ///<returns>(float) The index where the item was inserted if success.
    ///           -1 on failure.</returns>
    static member AddSearchPath(folder:string, [<OPT;DEF(-1)>]index:int) : float =
        Application.addSearchPath index folder

    ///<summary>Returns number of command aliases in Rhino.</summary>
    ///
    ///<returns>(int) the number of command aliases in Rhino.</returns>
    static member AliasCount() : int =
        Application.aliasCount ()

    ///<summary>Returns a list of command alias names.</summary>
    ///
    ///<returns>(string seq) a list of command alias names.</returns>
    static member AliasNames() : string seq =
        Application.aliasNames ()

    ///<summary>Returns the build date of Rhino</summary>
    ///
    ///<returns>(DateTime) the build date of Rhino. Will be converted to a string by most functions.</returns>
    static member BuildDate() : DateTime =
        Application.buildDate ()

    ///<summary>Clears contents of Rhino's command history window. You can view the
    ///    command history window by using the CommandHistory command in Rhino.</summary>
    ///
    ///<returns>(unit) </returns>
    static member ClearCommandHistory() : unit =
        Application.clearCommandHistory ()

    ///<summary>Runs a Rhino command script. All Rhino commands can be used in command
    ///    scripts. The command can be a built-in Rhino command or one provided by a
    ///    3rd party plug-in.
    ///    Write command scripts just as you would type the command sequence at the
    ///    command line. A space or a new line acts like pressing <Enter> at the
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
    ///    recently created or changed object by calling LastCreatedObjects.</summary>
    ///<param name="commandString">(string) A Rhino command including any arguments</param>
    ///<param name="echo">(bool) Optional, Default Value:True, The command echo mode True will display the commands on the commandline. If omitted, command prompts are echoed (True)</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member Command(commandString:string, [<OPT;DEF(true)>]echo:bool) : bool =
        Application.command echo commandString

    ///<summary>Returns the contents of Rhino's command history window</summary>
    ///
    ///<returns>(string) the contents of Rhino's command history window</returns>
    static member CommandHistory() : string =
        Application.commandHistory ()

    ///<summary>Delete an existing alias from Rhino.</summary>
    ///<param name="alias">(string) The name of an existing alias.</param>
    ///
    ///<returns>(bool) True or False indicating success</returns>
    static member DeleteAlias(alias:string) : bool =
        Application.deleteAlias alias

    ///<summary>Removes existing path from Rhino's search path list. Search path items
    ///    can be removed manually by using Rhino's options command and modifying the
    ///    contents of the files tab</summary>
    ///<param name="folder">(string) A folder to remove</param>
    ///
    ///<returns>(bool) True or False indicating success</returns>
    static member DeleteSearchPath(folder:string) : bool =
        Application.deleteSearchPath folder

    ///<summary>Enables/disables OLE Server Busy/Not Responding dialog boxes</summary>
    ///<param name="enable">(bool) Whether alerts should be visible (True or False)</param>
    ///
    ///<returns>(unit) </returns>
    static member DisplayOleAlerts(enable:bool) : unit =
        Application.displayOleAlerts enable

    ///<summary>Enables or disables Rhino's automatic file saving mechanism</summary>
    ///<param name="enable">(bool) Optional, Default Value:True, The autosave state. If omitted automatic saving is enabled (True)</param>
    ///
    ///<returns>(unit) unit</returns>
    static member EnableAutosave([<OPT;DEF(true)>]enable:bool) : unit =
        Application.enableAutosave enable

    ///<summary>Enables or disables a Rhino plug-in</summary>
    ///<param name="plugin">(Guid) The unique Guid id of the plugin.</param>
    ///<param name="enable">(bool) Optional, Default Value:None, Load silently if True. If omitted Load silently is False.</param>
    ///
    ///<returns>(bool) True if set to load silently otherwise False</returns>
    static member EnablePlugIn(plugin:Guid, [<OPT;DEF(null)>]enable:bool) : bool =
        Application.enablePlugIn enable plugin

    ///<summary>Returns the full path to Rhino's executable folder.</summary>
    ///
    ///<returns>(string) the full path to Rhino's executable folder.</returns>
    static member ExeFolder() : string =
        Application.exeFolder ()

    ///<summary>Returns the platform of the Rhino executable</summary>
    ///
    ///<returns>(string) the platform of the Rhino executable</returns>
    static member ExePlatform() : string =
        Application.exePlatform ()

    ///<summary>Returns the service release number of the Rhino executable</summary>
    ///
    ///<returns>(string) the service release number of the Rhino executable</returns>
    static member ExeServiceRelease() : string =
        Application.exeServiceRelease ()

    ///<summary>Returns the major version number of the Rhino executable</summary>
    ///
    ///<returns>(string) the major version number of the Rhino executable</returns>
    static member ExeVersion() : string =
        Application.exeVersion ()

    ///<summary>Closes the rhino application</summary>
    ///
    ///<returns>(unit) </returns>
    static member Exit() : unit =
        Application.exit ()

    ///<summary>Searches for a file using Rhino's search path. Rhino will look for a
    ///    file in the following locations:
    ///      1. The current document's folder.
    ///      2. Folder's specified in Options dialog, File tab.
    ///      3. Rhino's System folders</summary>
    ///<param name="filename">(string) A short file name to search for</param>
    ///
    ///<returns>(string) full path on success</returns>
    static member FindFile(filename:string) : string =
        Application.findFile filename

    ///<summary>Returns a scriptable object from a specified plug-in. Not all plug-ins
    ///    contain scriptable objects. Check with the manufacturer of your plug-in
    ///    to see if they support this capability.</summary>
    ///<param name="plug_in">(string) The name or Id of a registered plug-in that supports scripting.
    ///                             If the plug-in is registered but not loaded, it will be loaded</param>
    ///
    ///<returns>(Guid) scriptable object</returns>
    static member GetPlugInObject(plug_in:string) : Guid =
        Application.getPlugInObject plug_in

    ///<summary>Determines if Rhino is currently running a command. Because Rhino allows
    ///    for transparent commands (commands run from inside of other commands), this
    ///    method returns the total number of active commands.</summary>
    ///<param name="ignore_runners">(bool) Optional, Default Value:True, If True, script running commands, such as
    ///                                       LoadScript, RunScript, and ReadCommandFile will not counted.
    ///                                       If omitted the default is not to count script running command (True).</param>
    ///
    ///<returns>(float) the number of active commands</returns>
    static member InCommand([<OPT;DEF(true)>]ignore_runners:bool) : float =
        Application.inCommand ignore_runners

    ///<summary>The full path to Rhino's installation folder</summary>
    ///
    ///<returns>(string) the full path to Rhino's installation folder</returns>
    static member InstallFolder() : string =
        Application.installFolder ()

    ///<summary>Verifies that a command alias exists in Rhino</summary>
    ///<param name="alias">(string) The name of an existing command alias</param>
    ///
    ///<returns>(bool) True if exists or False if the alias does not exist.</returns>
    static member IsAlias(alias:string) : bool =
        Application.isAlias alias

    ///<summary>Verifies that a command exists in Rhino. Useful when scripting commands
    ///    found in 3rd party plug-ins.</summary>
    ///<param name="command_name">(string) The command name to test</param>
    ///
    ///<returns>(bool) True if the string is a command or False if it is not a command.</returns>
    static member IsCommand(command_name:string) : bool =
        Application.isCommand command_name

    ///<summary>Verifies that a plug-in is registered</summary>
    ///<param name="plugin">(Guid) The unique id of the plug-in</param>
    ///
    ///<returns>(bool) True if the Guid is registered or False if it is not.</returns>
    static member IsPlugIn(plugin:Guid) : bool =
        Application.isPlugIn plugin

    ///<summary>Returns True if this script is being executed on a Windows platform</summary>
    ///
    ///<returns>(bool) True if currently running on the Widows platform. False if it is not Windows.</returns>
    static member IsRunningOnWindows() : bool =
        Application.isRunningOnWindows ()

    ///<summary>Returns the name of the last executed command</summary>
    ///
    ///<returns>(string) the name of the last executed command</returns>
    static member LastCommandName() : string =
        Application.lastCommandName ()

    ///<summary>Returns the result code for the last executed command</summary>
    ///
    ///<returns>(float) the result code for the last executed command.
    ///                0 = success (command successfully completed)
    ///                1 = cancel (command was cancelled by the user)
    ///                2 = nothing (command did nothing, but was not cancelled)
    ///                3 = failure (command failed due to bad input, computational problem...)
    ///                4 = unknown command (the command was not found)</returns>
    static member LastCommandResult() : float =
        Application.lastCommandResult ()

    ///<summary>Returns the current language used for the Rhino interface.  The current
    ///    language is returned as a locale ID, or LCID, value.</summary>
    ///
    ///<returns>(float) the current language used for the Rhino interface as a locale ID, or LCID.
    ///                1029  Czech
    ///                1031  German-Germany
    ///                1033  English-United States
    ///                1034  Spanish-Spain
    ///                1036  French-France
    ///                1040  Italian-Italy
    ///                1041  Japanese
    ///                1042  Korean
    ///                1045  Polish</returns>
    static member LocaleID() : float =
        Application.localeID ()

    ///<summary>Returns the identifier of a plug-in given the plug-in name</summary>
    ///<param name="plugin">(Guid) Unique id of the plug-in</param>
    ///
    ///<returns>(Guid) the id of the plug-in</returns>
    static member PlugInId(plugin:Guid) : Guid =
        Application.plugInId plugin

    ///<summary>Returns a list of registered Rhino plug-ins</summary>
    ///<param name="types">(int) Optional, Default Value:0, The type of plug-ins to return.
    ///                                0=all
    ///                                1=render
    ///                                2=file export
    ///                                4=file import
    ///                                8=digitizer
    ///                                16=utility.
    ///                                If omitted, all are returned.</param>
    ///<param name="status">(int) Optional, Default Value:0, 0=both loaded and unloaded, 1=loaded, 2=unloaded.  If omitted both status is returned.</param>
    ///
    ///<returns>(string seq) list of registered Rhino plug-ins</returns>
    static member PlugIns([<OPT;DEF(0)>]types:int, [<OPT;DEF(0)>]status:int) : string seq =
        Application.plugIns status types

    ///<summary>Enables or disables object snap projection</summary>
    ///<param name="enable">(bool) Optional, Default Value:None, The new enabled status.  If omitted the current status is returned.</param>
    ///
    ///<returns>(bool) the current object snap projection status</returns>
    static member ProjectOsnaps([<OPT;DEF(null)>]enable:bool) : bool =
        Application.projectOsnaps enable

    ///<summary>Change Rhino's command window prompt</summary>
    ///<param name="message">(string) Optional, Default Value:None, The new prompt on the commandline.  If omitted the prompt will be blank.</param>
    ///
    ///<returns>(unit) </returns>
    static member Prompt([<OPT;DEF(null)>]message:string) : unit =
        Application.prompt message

    ///<summary>Returns current width and height, of the screen of the primary monitor.</summary>
    ///
    ///<returns>(float * float) containing two numbers identifying the width and height in pixels</returns>
    static member ScreenSize() : float * float =
        Application.screenSize ()

    ///<summary>Returns version of the Rhino SDK supported by the executing Rhino.</summary>
    ///
    ///<returns>(string) the version of the Rhino SDK supported by the executing Rhino. Rhino SDK versions are 9 digit numbers in the form of YYYYMMDDn.</returns>
    static member SdkVersion() : string =
        Application.sdkVersion ()

    ///<summary>Returns the number of path items in Rhino's search path list.
    ///    See "Options Files settings" in the Rhino help file for more details.</summary>
    ///
    ///<returns>(int) the number of path items in Rhino's search path list</returns>
    static member SearchPathCount() : int =
        Application.searchPathCount ()

    ///<summary>Returns all of the path items in Rhino's search path list.
    ///    See "Options Files settings" in the Rhino help file for more details.</summary>
    ///
    ///<returns>(string seq) list of search paths</returns>
    static member SearchPathList() : string seq =
        Application.searchPathList ()

    ///<summary>Sends a string of printable characters to Rhino's command line</summary>
    ///<param name="keys">(string) Optional, Default Value:None, A string of characters to send to the command line.</param>
    ///<param name="add_return">(bool) Optional, Default Value:True, Append a return character to the end of the string. If omitted an return character will be added (True)</param>
    ///
    ///<returns>(unit) </returns>
    static member SendKeystrokes([<OPT;DEF(null)>]keys:string, [<OPT;DEF(true)>]add_return:bool) : unit =
        Application.sendKeystrokes add_return keys

    ///<summary>Enables or disables Rhino's grid snap modeling aid</summary>
    ///<param name="enable">(bool) Optional, Default Value:None, The new enabled status. If omitted the current status is returned.</param>
    ///
    ///<returns>(bool) the current grid snap status</returns>
    static member Snap([<OPT;DEF(null)>]enable:bool) : bool =
        Application.snap enable

    ///<summary>Sets Rhino's status bar distance pane</summary>
    ///<param name="distance">(int) Optional, Default Value:0, The distance to set the status bar.  If omitted the distance will be set to 0.</param>
    ///
    ///<returns>(unit) </returns>
    static member StatusBarDistance([<OPT;DEF(0)>]distance:int) : unit =
        Application.statusBarDistance distance

    ///<summary>Sets Rhino's status bar message pane</summary>
    ///<param name="message">(string) Optional, Default Value:None, The message to display.</param>
    ///
    ///<returns>(unit) </returns>
    static member StatusBarMessage([<OPT;DEF(null)>]message:string) : unit =
        Application.statusBarMessage message

    ///<summary>Sets Rhino's status bar point coordinate pane</summary>
    ///<param name="point">(Point3d) Optional, Default Value:None, The 3d coordinates of the status bar.  If omitted the current poition is set to (0,0,0).</param>
    ///
    ///<returns>(unit) </returns>
    static member StatusBarPoint([<OPT;DEF(null)>]point:Point3d) : unit =
        Application.statusBarPoint point

    ///<summary>Start the Rhino status bar progress meter</summary>
    ///<param name="label">(string) Short description of the progesss</param>
    ///<param name="lower">(string) Lower limit of the progress meter's range</param>
    ///<param name="upper">(string) Upper limit of the progress meter's range</param>
    ///<param name="embed_label">(bool) Optional, Default Value:True, If True, the label will show inside the meter.
    ///                                    If false, the label will show to the left of the meter.
    ///                                    If omitted the label will show inside the meter (True)</param>
    ///<param name="show_percent">(bool) Optional, Default Value:True, Show the percent complete if True. If omitted the percnetage will be shown (True)</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member StatusBarProgressMeterShow(label:string, lower:string, upper:string, [<OPT;DEF(true)>]embed_label:bool, [<OPT;DEF(true)>]show_percent:bool) : bool =
        Application.statusBarProgressMeterShow show_percent embed_label upper lower label

    ///<summary>Set the current position of the progress meter</summary>
    ///<param name="position">(int) The new position in the progress meter</param>
    ///<param name="absolute">(bool) Optional, Default Value:True, The position is set absolute (True) or relative (False) to its current position. If omitted the absolute (True) is used.</param>
    ///
    ///<returns>(unit) unit</returns>
    static member StatusBarProgressMeterUpdate(position:int, [<OPT;DEF(true)>]absolute:bool) : unit =
        Application.statusBarProgressMeterUpdate absolute position

    ///<summary>Hide the progress meter</summary>
    ///
    ///<returns>(unit) </returns>
    static member StatusBarProgressMeterHide() : unit =
        Application.statusBarProgressMeterHide ()

    ///<summary>Returns the windows handle of Rhino's main window</summary>
    ///
    ///<returns>(IntPtr) the Window's handle of Rhino's main window. IntPtr is a platform-specific type that is used to represent a pointer or a handle.</returns>
    static member WindowHandle() : IntPtr =
        Application.windowHandle ()

    ///<summary>Returns the Rhino Block instance object for a given Id</summary>
    ///<param name="id">(Guid) Id of block instance</param>
    ///<param name="raise_if_missing">(bool) Optional, Default Value:False, raise error if id is missing?</param>
    ///
    ///<returns>(Rhino.DocObjects.InstanceObject) block instance object</returns>
    static member InstanceObjectFromId(id:Guid, [<OPT;DEF(false)>]raise_if_missing:bool) : Rhino.DocObjects.InstanceObject =
        Block.instanceObjectFromId raise_if_missing id

    ///<summary>Adds a new block definition to the document</summary>
    ///<param name="object_ids">(Guid seq) objects that will be included in the block</param>
    ///<param name="base_point">(Point3d) 3D base point for the block definition</param>
    ///<param name="name">(string) Optional, Default Value:None, name of the block definition. If omitted a name will be
    ///        automatically generated</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, if True, the object_ids will be deleted</param>
    ///
    ///<returns>(string) name of new block definition on success</returns>
    static member AddBlock(object_ids:Guid seq, base_point:Point3d, [<OPT;DEF(null)>]name:string, [<OPT;DEF(false)>]delete_input:bool) : string =
        Block.addBlock delete_input name base_point object_ids

    ///<summary>Returns number of block definitions that contain a specified
    ///    block definition</summary>
    ///<param name="block_name">(string) the name of an existing block definition</param>
    ///
    ///<returns>(int) the number of block definitions that contain a specified block definition</returns>
    static member BlockContainerCount(block_name:string) : int =
        Block.blockContainerCount block_name

    ///<summary>Returns names of the block definitions that contain a specified block
    ///    definition.</summary>
    ///<param name="block_name">(string) the name of an existing block definition</param>
    ///
    ///<returns>(string seq) A list of block definition names</returns>
    static member BlockContainers(block_name:string) : string seq =
        Block.blockContainers block_name

    ///<summary>Returns the number of block definitions in the document</summary>
    ///
    ///<returns>(int) the number of block definitions in the document</returns>
    static member BlockCount() : int =
        Block.blockCount ()

    ///<summary>Counts number of instances of the block in the document.
    ///    Nested instances are not included in the count.</summary>
    ///<param name="block_name">(string) the name of an existing block definition</param>
    ///<param name="where_to_look">(int) Optional, Default Value:0, 0 = get top level references in active document.
    ///        1 = get top level and nested references in active document.
    ///        2 = check for references from other instance definitions</param>
    ///
    ///<returns>(int) the number of instances of the block in the document</returns>
    static member BlockInstanceCount(block_name:string, [<OPT;DEF(0)>]where_to_look:int) : int =
        Block.blockInstanceCount where_to_look block_name

    ///<summary>Returns the insertion point of a block instance.</summary>
    ///<param name="object_id">(Guid) The identifier of an existing block insertion object</param>
    ///
    ///<returns>(Point3d) The insertion 3D point</returns>
    static member BlockInstanceInsertPoint(object_id:Guid) : Point3d =
        Block.blockInstanceInsertPoint object_id

    ///<summary>Returns the block name of a block instance</summary>
    ///<param name="object_id">(Guid) The identifier of an existing block insertion object</param>
    ///
    ///<returns>(string) the block name of a block instance</returns>
    static member BlockInstanceName(object_id:Guid) : string =
        Block.blockInstanceName object_id

    ///<summary>Returns the identifiers of the inserted instances of a block.</summary>
    ///<param name="block_name">(string) the name of an existing block definition</param>
    ///
    ///<returns>(Guid seq) Ids identifying the instances of a block in the model.</returns>
    static member BlockInstances(block_name:string) : Guid seq =
        Block.blockInstances block_name

    ///<summary>Returns the location of a block instance relative to the world coordinate
    ///    system origin (0,0,0). The position is returned as a 4x4 transformation
    ///    matrix</summary>
    ///<param name="object_id">(Guid) The identifier of an existing block insertion object</param>
    ///
    ///<returns>(Transform) the location, as a transform matrix, of a block instance relative to the world coordinate
    ///    system origin</returns>
    static member BlockInstanceXform(object_id:Guid) : Transform =
        Block.blockInstanceXform object_id

    ///<summary>Returns the names of all block definitions in the document</summary>
    ///
    ///<returns>(string seq) the names of all block definitions in the document</returns>
    static member BlockNames() : string seq =
        Block.blockNames ()

    ///<summary>Returns number of objects that make up a block definition</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///
    ///<returns>(int) the number of objects that make up a block definition</returns>
    static member BlockObjectCount(block_name:string) : int =
        Block.blockObjectCount block_name

    ///<summary>Returns identifiers of the objects that make up a block definition</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///
    ///<returns>(Guid seq) list of identifiers on success</returns>
    static member BlockObjects(block_name:string) : Guid seq =
        Block.blockObjects block_name

    ///<summary>Returns path to the source of a linked or embedded block definition.
    ///    A linked or embedded block definition is a block definition that was
    ///    inserted from an external file.</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///
    ///<returns>(string) path to the linked block on success</returns>
    static member BlockPath(block_name:string) : string =
        Block.blockPath block_name

    ///<summary>Returns the status of a linked block</summary>
    ///<param name="block_name">(string) Name of an existing block</param>
    ///
    ///<returns>(int) the status of a linked block
    ///        Value Description
    ///        -3    Not a linked block definition.
    ///        -2    The linked block definition's file could not be opened or could not be read.
    ///        -1    The linked block definition's file could not be found.
    ///         0    The linked block definition is up-to-date.
    ///         1    The linked block definition's file is newer than definition.
    ///         2    The linked block definition's file is older than definition.
    ///         3    The linked block definition's file is different than definition.</returns>
    static member BlockStatus(block_name:string) : int =
        Block.blockStatus block_name

    ///<summary>Deletes a block definition and all of it's inserted instances.</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteBlock(block_name:string) : bool =
        Block.deleteBlock block_name

    ///<summary>Explodes a block instance into it's geometric components. The
    ///    exploded objects are added to the document</summary>
    ///<param name="object_id">(Guid) The identifier of an existing block insertion object</param>
    ///<param name="explode_nested_instances">(bool) Optional, Default Value:False, By default nested blocks are not exploded.</param>
    ///
    ///<returns>(Guid seq) identifiers for the newly exploded objects on success</returns>
    static member ExplodeBlockInstance(object_id:Guid, [<OPT;DEF(false)>]explode_nested_instances:bool) : Guid seq =
        Block.explodeBlockInstance explode_nested_instances object_id

    ///<summary>Inserts a block whose definition already exists in the document</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///<param name="insertion_point">(Point3d) insertion point for the block</param>
    ///<param name="scale">(float*float*float) Optional, Default Value:1*1*1, x,y,z scale factors</param>
    ///<param name="angle_degrees">(int) Optional, Default Value:0, rotation angle in degrees</param>
    ///<param name="rotation_normal">(Vector3d) Optional, Default Value:0*0*1, the axis of rotation.</param>
    ///
    ///<returns>(Guid) id for the block that was added to the doc</returns>
    static member InsertBlock(block_name:string, insertion_point:Point3d, [<OPT;DEF(1*1*1)>]scale:float*float*float, [<OPT;DEF(0)>]angle_degrees:int, [<OPT;DEF(0*0*1)>]rotation_normal:Vector3d) : Guid =
        Block.insertBlock rotation_normal angle_degrees scale insertion_point block_name

    ///<summary>Inserts a block whose definition already exists in the document</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///<param name="xform">(Transform) 4x4 transformation matrix to apply</param>
    ///
    ///<returns>(Guid) id for the block that was added to the doc on success</returns>
    static member InsertBlock2(block_name:string, xform:Transform) : Guid =
        Block.insertBlock2 xform block_name

    ///<summary>Verifies the existence of a block definition in the document.</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsBlock(block_name:string) : bool =
        Block.isBlock block_name

    ///<summary>Verifies a block definition is embedded, or linked, from an external file.</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsBlockEmbedded(block_name:string) : bool =
        Block.isBlockEmbedded block_name

    ///<summary>Verifies an object is a block instance</summary>
    ///<param name="object_id">(Guid) The identifier of an existing block insertion object</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsBlockInstance(object_id:Guid) : bool =
        Block.isBlockInstance object_id

    ///<summary>Verifies that a block definition is being used by an inserted instance</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///<param name="where_to_look">(float) Optional, Default Value:0, One of the following values
    ///           0 = Check for top level references in active document
    ///           1 = Check for top level and nested references in active document
    ///           2 = Check for references in other instance definitions</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsBlockInUse(block_name:string, [<OPT;DEF(0)>]where_to_look:float) : bool =
        Block.isBlockInUse where_to_look block_name

    ///<summary>Verifies that a block definition is from a reference file.</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsBlockReference(block_name:string) : bool =
        Block.isBlockReference block_name

    ///<summary>Renames an existing block definition</summary>
    ///<param name="block_name">(string) name of an existing block definition</param>
    ///<param name="new_name">(string) name to change to</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RenameBlock(block_name:string, new_name:string) : bool =
        Block.renameBlock new_name block_name

    ///<summary>Adds an arc curve to the document</summary>
    ///<param name="plane">(Plane) plane on which the arc will lie. The origin of the plane will be
    ///        the center point of the arc. x-axis of the plane defines the 0 angle
    ///        direction.</param>
    ///<param name="radius">(float) radius of the arc</param>
    ///<param name="angle_degrees">(int) interval of arc in degrees</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddArc(plane:Plane, radius:float, angle_degrees:int) : Guid =
        Curve.addArc angle_degrees radius plane

    ///<summary>Adds a 3-point arc curve to the document</summary>
    ///<param name="start">(Point3d) start of 'endpoints of the arc' (FIXME 0)</param>
    ///<param name="end">(Point3d) end of 'endpoints of the arc' (FIXME 0)</param>
    ///<param name="point_on_arc">(Point3d) a point on the arc</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddArc3Pt(start:Point3d, end:Point3d, point_on_arc:Point3d) : Guid =
        Curve.addArc3Pt point_on_arc end start

    ///<summary>Adds an arc curve, created from a start point, a start direction, and an
    ///    end point, to the document</summary>
    ///<param name="start">(Point3d) the starting point of the arc</param>
    ///<param name="direction">(Vector3d) the arc direction at start</param>
    ///<param name="end">(Point3d) the ending point of the arc</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddArcPtTanPt(start:Point3d, direction:Vector3d, end:Point3d) : Guid =
        Curve.addArcPtTanPt end direction start

    ///<summary>Makes a curve blend between two curves</summary>
    ///<param name="curves">(Guid * Guid) list of two curves</param>
    ///<param name="parameters">(float * float) list of two curve parameters defining the blend end points</param>
    ///<param name="reverses">(bool * bool) list of two boolean values specifying to use the natural or opposite direction of the curve</param>
    ///<param name="continuities">(int * int) list of two numbers specifying continuity at end points
    ///                                            0 = position
    ///                                            1 = tangency
    ///                                            2 = curvature</param>
    ///
    ///<returns>(Guid) identifier of new curve on success</returns>
    static member AddBlendCurve(curves:Guid * Guid, parameters:float * float, reverses:bool * bool, continuities:int * int) : Guid =
        Curve.addBlendCurve continuities reverses parameters curves

    ///<summary>Adds a circle curve to the document</summary>
    ///<param name="plane_or_center">(Plane) plane on which the circle will lie. If a point is
    ///        passed, this will be the center of the circle on the active
    ///        construction plane</param>
    ///<param name="radius">(float) the radius of the circle</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddCircle(plane_or_center:Plane, radius:float) : Guid =
        Curve.addCircle radius plane_or_center

    ///<summary>Adds a 3-point circle curve to the document</summary>
    ///<param name="first">(Point3d) first of 'points on the circle' (FIXME 0)</param>
    ///<param name="second">(Point3d) second of 'points on the circle' (FIXME 0)</param>
    ///<param name="third">(Point3d) third of 'points on the circle' (FIXME 0)</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddCircle3Pt(first:Point3d, second:Point3d, third:Point3d) : Guid =
        Curve.addCircle3Pt third second first

    ///<summary>Adds a control points curve object to the document</summary>
    ///<param name="points">(Point3d seq) a list of points</param>
    ///<param name="degree">(int) Optional, Default Value:3, degree of the curve</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddCurve(points:Point3d seq, [<OPT;DEF(3)>]degree:int) : Guid =
        Curve.addCurve degree points

    ///<summary>Adds an elliptical curve to the document</summary>
    ///<param name="plane">(Plane) the plane on which the ellipse will lie. The origin of
    ///              the plane will be the center of the ellipse</param>
    ///<param name="radiusX">(float) radiusX of 'radius in the X and Y axis directions' (FIXME 0)</param>
    ///<param name="radiusY">(float) radiusY of 'radius in the X and Y axis directions' (FIXME 0)</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddEllipse(plane:Plane, radiusX:float, radiusY:float) : Guid =
        Curve.addEllipse radiusY radiusX plane

    ///<summary>Adds a 3-point elliptical curve to the document</summary>
    ///<param name="center">(Point3d) center point of the ellipse</param>
    ///<param name="second">(Point3d) end point of the x axis</param>
    ///<param name="third">(Point3d) end point of the y axis</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddEllipse3Pt(center:Point3d, second:Point3d, third:Point3d) : Guid =
        Curve.addEllipse3Pt third second center

    ///<summary>Adds a fillet curve between two curve objects</summary>
    ///<param name="curve0id">(Guid) identifier of the first curve object</param>
    ///<param name="curve1id">(Guid) identifier of the second curve object</param>
    ///<param name="radius">(float) Optional, Default Value:1.0, fillet radius</param>
    ///<param name="base_point0">(Point3d) Optional, Default Value:None, base point of the first curve. If omitted,
    ///                          starting point of the curve is used</param>
    ///<param name="base_point1">(Point3d) Optional, Default Value:None, base point of the second curve. If omitted,
    ///                          starting point of the curve is used</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddFilletCurve(curve0id:Guid, curve1id:Guid, [<OPT;DEF(1.0)>]radius:float, [<OPT;DEF(null)>]base_point0:Point3d, [<OPT;DEF(null)>]base_point1:Point3d) : Guid =
        Curve.addFilletCurve base_point1 base_point0 radius curve1id curve0id

    ///<summary>Adds an interpolated curve object that lies on a specified
    ///    surface.  Note, this function will not create periodic curves,
    ///    but it will create closed curves.</summary>
    ///<param name="surface_id">(Guid) identifier of the surface to create the curve on</param>
    ///<param name="points">(Point3d seq) list of 3D points that lie on the specified surface.
    ///               The list must contain at least 2 points</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddInterpCrvOnSrf(surface_id:Guid, points:Point3d seq) : Guid =
        Curve.addInterpCrvOnSrf points surface_id

    ///<summary>Adds an interpolated curve object based on surface parameters,
    ///    that lies on a specified surface. Note, this function will not
    ///    create periodic curves, but it will create closed curves.</summary>
    ///<param name="surface_id">(Guid) identifier of the surface to create the curve on</param>
    ///<param name="points">(float seq) a list of 2D surface parameters. The list must contain
    ///                                                         at least 2 sets of parameters</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddInterpCrvOnSrfUV(surface_id:Guid, points:float seq) : Guid =
        Curve.addInterpCrvOnSrfUV points surface_id

    ///<summary>Adds an interpolated curve object to the document. Options exist to make
    ///    a periodic curve or to specify the tangent at the endpoints. The resulting
    ///    curve is a non-rational NURBS curve of the specified degree.</summary>
    ///<param name="points">(Point3d seq) a list containing 3D points to interpolate. For periodic curves,
    ///          if the final point is a duplicate of the initial point, it is
    ///          ignored. The number of control points must be >= (degree+1).</param>
    ///<param name="degree">(int) Optional, Default Value:3, The degree of the curve (must be >=1).
    ///          Periodic curves must have a degree >= 2. For knotstyle = 1 or 2,
    ///          the degree must be 3. For knotstyle = 4 or 5, the degree must be odd</param>
    ///<param name="knotstyle">(int) Optional, Default Value:0, 0 Uniform knots.  Parameter spacing between consecutive knots is 1.0.
    ///          1 Chord length spacing.  Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///          2 Sqrt (chord length).  Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///          3 Periodic with uniform spacing.
    ///          4 Periodic with chord length spacing.  Requires an odd degree value.
    ///          5 Periodic with sqrt (chord length) spacing.  Requires an odd degree value.</param>
    ///<param name="start_tangent">(Vector3d) Optional, Default Value:None, a vector that specifies a tangency condition at the
    ///          beginning of the curve. If the curve is periodic, this argument must be omitted.</param>
    ///<param name="end_tangent">(Vector3d) Optional, Default Value:None, 3d vector that specifies a tangency condition at the
    ///          end of the curve. If the curve is periodic, this argument must be omitted.</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddInterpCurve(points:Point3d seq, [<OPT;DEF(3)>]degree:int, [<OPT;DEF(0)>]knotstyle:int, [<OPT;DEF(null)>]start_tangent:Vector3d, [<OPT;DEF(null)>]end_tangent:Vector3d) : Guid =
        Curve.addInterpCurve end_tangent start_tangent knotstyle degree points

    ///<summary>Adds a line curve to the current model.</summary>
    ///<param name="start">(Point3d) start of 'end points of the line' (FIXME 0)</param>
    ///<param name="end">(Point3d) end of 'end points of the line' (FIXME 0)</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddLine(start:Point3d, end:Point3d) : Guid =
        Curve.addLine end start

    ///<summary>Adds a NURBS curve object to the document</summary>
    ///<param name="points">(Point3d seq) a list containing 3D control points</param>
    ///<param name="knots">(float seq) Knot values for the curve. The number of elements in knots must
    ///          equal the number of elements in points plus degree minus 1</param>
    ///<param name="degree">(int) degree of the curve. must be greater than of equal to 1</param>
    ///<param name="weights">(float seq) Optional, Default Value:None, weight values for the curve. Number of elements should
    ///          equal the number of elements in points. Values must be greater than 0</param>
    ///
    ///<returns>(Guid) the identifier of the new object , otherwise None</returns>
    static member AddNurbsCurve(points:Point3d seq, knots:float seq, degree:int, [<OPT;DEF(null)>]weights:float seq) : Guid =
        Curve.addNurbsCurve weights degree knots points

    ///<summary>Adds a polyline curve to the current model</summary>
    ///<param name="points">(Point3d seq) list of 3D points. Duplicate, consecutive points will be
    ///               removed. The list must contain at least two points. If the
    ///               list contains less than four points, then the first point and
    ///               last point must be different.</param>
    ///<param name="replace_id">(Guid) Optional, Default Value:None, If set to the id of an existing object, the object
    ///               will be replaced by this polyline</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddPolyline(points:Point3d seq, [<OPT;DEF(null)>]replace_id:Guid) : Guid =
        Curve.addPolyline replace_id points

    ///<summary>Add a rectangular curve to the document</summary>
    ///<param name="plane">(Plane) plane on which the rectangle will lie</param>
    ///<param name="width">(float) width of rectangle as measured along the plane's
    /// x and y axes</param>
    ///<param name="height">(float) height of rectangle as measured along the plane's
    /// x and y axes</param>
    ///
    ///<returns>(Guid) id of new rectangle</returns>
    static member AddRectangle(plane:Plane, width:float, height:float) : Guid =
        Curve.addRectangle height width plane

    ///<summary>Adds a spiral or helical curve to the document</summary>
    ///<param name="point0">(Point3d) helix axis start point or center of spiral</param>
    ///<param name="point1">(Point3d) helix axis end point or point normal on spiral plane</param>
    ///<param name="pitch">(float) distance between turns. If 0, then a spiral. If > 0 then the
    ///              distance between helix "threads"</param>
    ///<param name="turns">(float) number of turns</param>
    ///<param name="radius0">(float) starting radius of spiral</param>
    ///<param name="radius1">(float) Optional, Default Value:None, ending radius of spiral. If omitted, the starting radius is used for the complete spiral.</param>
    ///
    ///<returns>(Guid) id of new curve on success</returns>
    static member AddSpiral(point0:Point3d, point1:Point3d, pitch:float, turns:float, radius0:float, [<OPT;DEF(null)>]radius1:float) : Guid =
        Curve.addSpiral radius1 radius0 turns pitch point1 point0

    ///<summary>Add a curve object based on a portion, or interval of an existing curve
    ///    object. Similar in operation to Rhino's SubCrv command</summary>
    ///<param name="curve_id">(Guid) identifier of a closed planar curve object</param>
    ///<param name="param0">(float) first parameters on the source curve</param>
    ///<param name="param1">(float) second parameters on the source curve</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddSubCrv(curve_id:Guid, param0:float, param1:float) : Guid =
        Curve.addSubCrv param1 param0 curve_id

    ///<summary>Returns the angle of an arc curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, identifies the curve segment if curve_id identifies a polycurve</param>
    ///
    ///<returns>(float) The angle in degrees .</returns>
    static member ArcAngle(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : float =
        Curve.arcAngle segment_index curve_id

    ///<summary>Returns the center point of an arc curve object</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Point3d) The 3D center point of the arc .</returns>
    static member ArcCenterPoint(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : Point3d =
        Curve.arcCenterPoint segment_index curve_id

    ///<summary>Returns the mid point of an arc curve object</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Point3d) The 3D mid point of the arc .</returns>
    static member ArcMidPoint(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : Point3d =
        Curve.arcMidPoint segment_index curve_id

    ///<summary>Returns the radius of an arc curve object</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(float) The radius of the arc .</returns>
    static member ArcRadius(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : float =
        Curve.arcRadius segment_index curve_id

    ///<summary>Returns the circumference of a circle curve object</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(float) The circumference of the circle .</returns>
    static member CircleCircumference(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : float =
        Curve.circleCircumference segment_index curve_id

    ///<summary>Returns the radius of a circle curve object</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(float) The radius of the circle .</returns>
    static member CircleRadius(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : float =
        Curve.circleRadius segment_index curve_id

    ///<summary>Closes an open curve object by making adjustments to the end points so
    ///    they meet at a point</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value:-1.0, maximum allowable distance between start and end
    ///                                    point. If omitted, the current absolute tolerance is used</param>
    ///
    ///<returns>(Guid) id of the new curve object</returns>
    static member CloseCurve(curve_id:Guid, [<OPT;DEF(-1.0)>]tolerance:float) : Guid =
        Curve.closeCurve tolerance curve_id

    ///<summary>Determine the orientation (counter-clockwise or clockwise) of a closed,
    ///    planar curve</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="direction">(Vector3d) Optional, Default Value:0*0*1, 3d vector that identifies up, or Z axs, direction of
    ///                                    the plane to test against</param>
    ///
    ///<returns>(float) 1 if the curve's orientation is clockwise
    ///             -1 if the curve's orientation is counter-clockwise
    ///              0 if unable to compute the curve's orientation</returns>
    static member ClosedCurveOrientation(curve_id:Guid, [<OPT;DEF(0*0*1)>]direction:Vector3d) : float =
        Curve.closedCurveOrientation direction curve_id

    ///<summary>Convert curve to a polyline curve</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="angle_tolerance">(float) Optional, Default Value:5.0, The maximum angle between curve tangents at line endpoints.
    ///                                          If omitted, the angle tolerance is set to 5.0.</param>
    ///<param name="tolerance">(float) Optional, Default Value:0.01, The distance tolerance at segment midpoints. If omitted, the tolerance is set to 0.01.</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, Delete the curve object specified by curve_id. If omitted, curve_id will not be deleted.</param>
    ///<param name="min_edge_length">(float) Optional, Default Value:0, Minimum segment length</param>
    ///<param name="max_edge_length">(float) Optional, Default Value:0, Maximum segment length</param>
    ///
    ///<returns>(Guid) The new curve .</returns>
    static member ConvertCurveToPolyline(curve_id:Guid, [<OPT;DEF(5.0)>]angle_tolerance:float, [<OPT;DEF(0.01)>]tolerance:float, [<OPT;DEF(false)>]delete_input:bool, [<OPT;DEF(0)>]min_edge_length:float, [<OPT;DEF(0)>]max_edge_length:float) : Guid =
        Curve.convertCurveToPolyline max_edge_length min_edge_length delete_input tolerance angle_tolerance curve_id

    ///<summary>Returns the point on the curve that is a specified arc length
    ///    from the start of the curve.</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="length">(float) The arc length from the start of the curve to evaluate.</param>
    ///<param name="from_start">(bool) Optional, Default Value:True, If not specified or True, then the arc length point is
    ///          calculated from the start of the curve. If False, the arc length
    ///          point is calculated from the end of the curve.</param>
    ///
    ///<returns>(Point3d) on curve</returns>
    static member CurveArcLengthPoint(curve_id:Guid, length:float, [<OPT;DEF(true)>]from_start:bool) : Point3d =
        Curve.curveArcLengthPoint from_start length curve_id

    ///<summary>Returns area of closed planar curves. The results are based on the
    ///    current drawing units.</summary>
    ///<param name="curve_id">(Guid) The identifier of a closed, planar curve object.</param>
    ///
    ///<returns>(float * float) List of area information. The list will contain the following information:
    ///        Element  Description
    ///        [0]      The area. If more than one curve was specified, the
    ///                   value will be the cumulative area.
    ///        [1]      The absolute (+/-) error bound for the area.</returns>
    static member CurveArea(curve_id:Guid) : float * float =
        Curve.curveArea curve_id

    ///<summary>Returns area centroid of closed, planar curves. The results are based
    ///    on the current drawing units.</summary>
    ///<param name="curve_id">(Guid) The identifier of a closed, planar curve object.</param>
    ///
    ///<returns>(Point3d * Vector3d) of area centroid information containing the following information:
    ///        Element  Description
    ///        [0]        The 3d centroid point. If more than one curve was specified,
    ///                 the value will be the cumulative area.
    ///        [1]        A 3d vector with the absolute (+/-) error bound for the area
    ///                 centroid.</returns>
    static member CurveAreaCentroid(curve_id:Guid) : Point3d * Vector3d =
        Curve.curveAreaCentroid curve_id

    ///<summary>Calculates the difference between two closed, planar curves and
    ///    adds the results to the document. Note, curves must be coplanar.</summary>
    ///<param name="curve_id_0">(Guid) identifier of the first curve object.</param>
    ///<param name="curve_id_1">(Guid) identifier of the second curve object.</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, a positive tolerance value, or None for the doc default.</param>
    ///
    ///<returns>(Guid seq) The identifiers of the new objects , .</returns>
    static member CurveBooleanDifference(curve_id_0:Guid, curve_id_1:Guid, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        Curve.curveBooleanDifference tolerance curve_id_1 curve_id_0

    ///<summary>Calculates the intersection of two closed, planar curves and adds
    ///    the results to the document. Note, curves must be coplanar.</summary>
    ///<param name="curve_id_0">(Guid) identifier of the first curve object.</param>
    ///<param name="curve_id_1">(Guid) identifier of the second curve object.</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, a positive tolerance value, or None for the doc default.</param>
    ///
    ///<returns>(Guid seq) The identifiers of the new objects.</returns>
    static member CurveBooleanIntersection(curve_id_0:Guid, curve_id_1:Guid, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        Curve.curveBooleanIntersection tolerance curve_id_1 curve_id_0

    ///<summary>Calculate the union of two or more closed, planar curves and
    ///    add the results to the document. Note, curves must be coplanar.</summary>
    ///<param name="curve_id">(Guid seq) list of two or more close planar curves identifiers</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, a positive tolerance value, or None for the doc default.</param>
    ///
    ///<returns>(Guid seq) The identifiers of the new objects.</returns>
    static member CurveBooleanUnion(curve_id:Guid seq, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        Curve.curveBooleanUnion tolerance curve_id

    ///<summary>Intersects a curve object with a brep object. Note, unlike the
    ///    CurveSurfaceIntersection function, this function works on trimmed surfaces.</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="brep_id">(Guid) identifier of a brep object</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, distance tolerance at segment midpoints.
    ///                        If omitted, the current absolute tolerance is used.</param>
    ///
    ///<returns>(Guid seq) identifiers for the newly created intersection objects .</returns>
    static member CurveBrepIntersect(curve_id:Guid, brep_id:Guid, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        Curve.curveBrepIntersect tolerance brep_id curve_id

    ///<summary>Returns the 3D point locations on two objects where they are closest to
    ///    each other. Note, this function provides similar functionality to that of
    ///    Rhino's ClosestPt command.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object to test</param>
    ///<param name="object_ids">(Guid seq) list of identifiers of point cloud, curve, surface, or
    ///        polysurface to test against</param>
    ///
    ///<returns>(Guid * Point3d * Point3d) containing the results of the closest point calculation.
    ///        The elements are as follows:
    ///          [0]    The identifier of the closest object.
    ///          [1]    The 3-D point that is closest to the closest object.
    ///          [2]    The 3-D point that is closest to the test curve.</returns>
    static member CurveClosestObject(curve_id:Guid, object_ids:Guid seq) : Guid * Point3d * Point3d =
        Curve.curveClosestObject object_ids curve_id

    ///<summary>Returns parameter of the point on a curve that is closest to a test point.</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="point">(Point3d) sampling point</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(float) The parameter of the closest point on the curve</returns>
    static member CurveClosestPoint(curve_id:Guid, point:Point3d, [<OPT;DEF(-1)>]segment_index:int) : float =
        Curve.curveClosestPoint segment_index point curve_id

    ///<summary>Returns the 3D point locations calculated by contouring a curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object.</param>
    ///<param name="start_point">(Point3d) 3D starting point of a center line.</param>
    ///<param name="end_point">(Point3d) 3D ending point of a center line.</param>
    ///<param name="interval">(float) Optional, Default Value:None, The distance between contour curves. If omitted,
    ///      the interval will be equal to the diagonal distance of the object's
    ///      bounding box divided by 50.</param>
    ///
    ///<returns>(Point3d seq) A list of 3D points, one for each contour</returns>
    static member CurveContourPoints(curve_id:Guid, start_point:Point3d, end_point:Point3d, [<OPT;DEF(null)>]interval:float) : Point3d seq =
        Curve.curveContourPoints interval end_point start_point curve_id

    ///<summary>Returns the curvature of a curve at a parameter. See the Rhino help for
    ///    details on curve curvature</summary>
    ///<param name="curve_id">(Guid) identifier of the curve</param>
    ///<param name="parameter">(float) parameter to evaluate</param>
    ///
    ///<returns>(Point3d * Vector3d * Point3d * float * Vector3d) of curvature information on success
    ///        [0] = point at specified parameter
    ///        [1] = tangent vector
    ///        [2] = center of radius of curvature
    ///        [3] = radius of curvature
    ///        [4] = curvature vector</returns>
    static member CurveCurvature(curve_id:Guid, parameter:float) : Point3d * Vector3d * Point3d * float * Vector3d =
        Curve.curveCurvature parameter curve_id

    ///<summary>Calculates intersection of two curve objects.</summary>
    ///<param name="curveA">(Guid) identifier of the first curve object.</param>
    ///<param name="curveB">(Guid) Optional, Default Value:None, identifier of the second curve object. If omitted, then a
    ///               self-intersection test will be performed on curveA.</param>
    ///<param name="tolerance">(float) Optional, Default Value:-1, absolute tolerance in drawing units. If omitted,
    ///                        the document's current absolute tolerance is used.</param>
    ///
    ///<returns>((int*Point3d*Point3d*Point3d*int*int*int*int*int*int) array) list of tuples: containing intersection information .
    ///        The list will contain one or more of the following elements:
    ///          Element Type     Description
    ///          [n][0]  Number   The intersection event type, either Point (1) or Overlap (2).
    ///          [n][1]  Point3d  If the event type is Point (1), then the intersection point
    ///                          on the first curve. If the event type is Overlap (2), then
    ///                          intersection start point on the first curve.
    ///          [n][2]  Point3d  If the event type is Point (1), then the intersection point
    ///                          on the first curve. If the event type is Overlap (2), then
    ///                          intersection end point on the first curve.
    ///          [n][3]  Point3d  If the event type is Point (1), then the intersection point
    ///                          on the second curve. If the event type is Overlap (2), then
    ///                          intersection start point on the second curve.
    ///          [n][4]  Point3d  If the event type is Point (1), then the intersection point
    ///                          on the second curve. If the event type is Overlap (2), then
    ///                          intersection end point on the second curve.
    ///          [n][5]  Number   If the event type is Point (1), then the first curve parameter.
    ///                          If the event type is Overlap (2), then the start value of the
    ///                          first curve parameter range.
    ///          [n][6]  Number   If the event type is Point (1), then the first curve parameter.
    ///                          If the event type is Overlap (2), then the end value of the
    ///                          first curve parameter range.
    ///          [n][7]  Number   If the event type is Point (1), then the second curve parameter.
    ///                          If the event type is Overlap (2), then the start value of the
    ///                          second curve parameter range.
    ///          [n][8]  Number   If the event type is Point (1), then the second curve parameter.
    ///                          If the event type is Overlap (2), then the end value of the
    ///                          second curve parameter range.</returns>
    static member CurveCurveIntersection(curveA:Guid, [<OPT;DEF(null)>]curveB:Guid, [<OPT;DEF(-1)>]tolerance:float) : (int*Point3d*Point3d*Point3d*int*int*int*int*int*int) array =
        Curve.curveCurveIntersection tolerance curveB curveA

    ///<summary>Returns the degree of a curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object.</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve.</param>
    ///
    ///<returns>(int) The degree of the curve .</returns>
    static member CurveDegree(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : int =
        Curve.curveDegree segment_index curve_id

    ///<summary>Returns the minimum and maximum deviation between two curve objects</summary>
    ///<param name="curve_a">(Guid) curve a of 'identifiers of two curves' (FIXME 0)</param>
    ///<param name="curve_b">(Guid) curve b of 'identifiers of two curves' (FIXME 0)</param>
    ///
    ///<returns>(float * float * float * float * float * float) of deviation information on success
    ///        [0] = curve_a parameter at maximum overlap distance point
    ///        [1] = curve_b parameter at maximum overlap distance point
    ///        [2] = maximum overlap distance
    ///        [3] = curve_a parameter at minimum overlap distance point
    ///        [4] = curve_b parameter at minimum overlap distance point
    ///        [5] = minimum distance between curves
    ///      </returns>
    static member CurveDeviation(curve_a:Guid, curve_b:Guid) : float * float * float * float * float * float =
        Curve.curveDeviation curve_b curve_a

    ///<summary>Returns the dimension of a curve object</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object.</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment if `curve_id` identifies a polycurve.</param>
    ///
    ///<returns>(float) The dimension of the curve . .</returns>
    static member CurveDim(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : float =
        Curve.curveDim segment_index curve_id

    ///<summary>Tests if two curve objects are generally in the same direction or if they
    ///    would be more in the same direction if one of them were flipped. When testing
    ///    curve directions, both curves must be either open or closed - you cannot test
    ///    one open curve and one closed curve.</summary>
    ///<param name="curve_id_0">(Guid) identifier of first curve object</param>
    ///<param name="curve_id_1">(Guid) identifier of second curve object</param>
    ///
    ///<returns>(bool) True if the curve directions match, otherwise False.</returns>
    static member CurveDirectionsMatch(curve_id_0:Guid, curve_id_1:Guid) : bool =
        Curve.curveDirectionsMatch curve_id_1 curve_id_0

    ///<summary>Search for a derivatitive, tangent, or curvature discontinuity in
    ///    a curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of curve object</param>
    ///<param name="style">(int) The type of continuity to test for. The types of
    ///          continuity are as follows:
    ///          Value    Description
    ///          1        C0 - Continuous function
    ///          2        C1 - Continuous first derivative
    ///          3        C2 - Continuous first and second derivative
    ///          4        G1 - Continuous unit tangent
    ///          5        G2 - Continuous unit tangent and curvature</param>
    ///
    ///<returns>(Point3d seq) 3D points where the curve is discontinuous</returns>
    static member CurveDiscontinuity(curve_id:Guid, style:int) : Point3d seq =
        Curve.curveDiscontinuity style curve_id

    ///<summary>Returns the domain of a curve object
    ///    as an indexable object with two elements.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve.</param>
    ///
    ///<returns>(float * float) the domain of the curve .
    ///         [0] Domain minimum
    ///         [1] Domain maximum</returns>
    static member CurveDomain(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : float * float =
        Curve.curveDomain segment_index curve_id

    ///<summary>Returns the edit, or Greville, points of a curve object. 
    ///    For each curve control point, there is a corresponding edit point.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="return_parameters">(bool) Optional, Default Value:False, if True, return as a list of curve parameters.
    ///                                          If False, return as a list of 3d points</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index is `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Point3d seq) curve edit points on success
    ///      </returns>
    static member CurveEditPoints(curve_id:Guid, [<OPT;DEF(false)>]return_parameters:bool, [<OPT;DEF(-1)>]segment_index:int) : Point3d seq =
        Curve.curveEditPoints segment_index return_parameters curve_id

    ///<summary>Returns the end point of a curve object</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Point3d) The 3d endpoint of the curve .</returns>
    static member CurveEndPoint(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : Point3d =
        Curve.curveEndPoint segment_index curve_id

    ///<summary>Returns the plane at a parameter of a curve. The plane is based on the
    ///    tangent and curvature vectors at a parameter.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object.</param>
    ///<param name="parameter">(float) parameter to evaluate.</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Plane) The plane at the specified parameter .</returns>
    static member CurveFrame(curve_id:Guid, parameter:float, [<OPT;DEF(-1)>]segment_index:int) : Plane =
        Curve.curveFrame segment_index parameter curve_id

    ///<summary>Returns the knot count of a curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object.</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment if `curve_id` identifies a polycurve.</param>
    ///
    ///<returns>(int) The number of knots .</returns>
    static member CurveKnotCount(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : int =
        Curve.curveKnotCount segment_index curve_id

    ///<summary>Returns the knots, or knot vector, of a curve object</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object.</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve.</param>
    ///
    ///<returns>(float seq) knot values .</returns>
    static member CurveKnots(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : float seq =
        Curve.curveKnots segment_index curve_id

    ///<summary>Returns the length of a curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///<param name="sub_domain">(float * float) Optional, Default Value:None, list of two numbers identifying the sub-domain of the
    ///          curve on which the calculation will be performed. The two parameters
    ///          (sub-domain) must be non-decreasing. If omitted, the length of the
    ///          entire curve is returned.</param>
    ///
    ///<returns>(float) The length of the curve .</returns>
    static member CurveLength(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int, [<OPT;DEF(null)>]sub_domain:float * float) : float =
        Curve.curveLength sub_domain segment_index curve_id

    ///<summary>Returns the mid point of a curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Point3d) The 3D midpoint of the curve .</returns>
    static member CurveMidPoint(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : Point3d =
        Curve.curveMidPoint segment_index curve_id

    ///<summary>Returns the normal direction of the plane in which a planar curve object lies.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment if curve_id identifies a polycurve</param>
    ///
    ///<returns>(Vector3d) The 3D normal vector .</returns>
    static member CurveNormal(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : Vector3d =
        Curve.curveNormal segment_index curve_id

    ///<summary>Converts a curve parameter to a normalized curve parameter;
    ///    one that ranges between 0-1</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="parameter">(float) the curve parameter to convert</param>
    ///
    ///<returns>(float) normalized curve parameter</returns>
    static member CurveNormalizedParameter(curve_id:Guid, parameter:float) : float =
        Curve.curveNormalizedParameter parameter curve_id

    ///<summary>Converts a normalized curve parameter to a curve parameter;
    ///    one within the curve's domain</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="parameter">(float) the normalized curve parameter to convert</param>
    ///
    ///<returns>(float) curve parameter</returns>
    static member CurveParameter(curve_id:Guid, parameter:float) : float =
        Curve.curveParameter parameter curve_id

    ///<summary>Returns the perpendicular plane at a parameter of a curve. The result
    ///    is relatively parallel (zero-twisting) plane</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="parameter">(float) parameter to evaluate</param>
    ///
    ///<returns>(Plane) Plane on success</returns>
    static member CurvePerpFrame(curve_id:Guid, parameter:float) : Plane =
        Curve.curvePerpFrame parameter curve_id

    ///<summary>Returns the plane in which a planar curve lies. Note, this function works
    ///    only on planar curves.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Plane) The plane in which the curve lies .</returns>
    static member CurvePlane(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : Plane =
        Curve.curvePlane segment_index curve_id

    ///<summary>Returns the control points count of a curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(int) Number of control points .</returns>
    static member CurvePointCount(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : int =
        Curve.curvePointCount segment_index curve_id

    ///<summary>Returns the control points, or control vertices, of a curve object.
    ///    If the curve is a rational NURBS curve, the euclidean control vertices
    ///    are returned.</summary>
    ///<param name="curve_id">(Guid) the object's identifier</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Point3d seq) the control points, or control vertices, of a curve object</returns>
    static member CurvePoints(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : Point3d seq =
        Curve.curvePoints segment_index curve_id

    ///<summary>Returns the radius of curvature at a point on a curve.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="test_point">(Point3d) sampling point</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment if curve_id identifies a polycurve</param>
    ///
    ///<returns>(float) The radius of curvature at the point on the curve .</returns>
    static member CurveRadius(curve_id:Guid, test_point:Point3d, [<OPT;DEF(-1)>]segment_index:int) : float =
        Curve.curveRadius segment_index test_point curve_id

    ///<summary>Adjusts the seam, or start/end, point of a closed curve.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="parameter">(float) The parameter of the new start/end point.
    ///                  Note, if successful, the resulting curve's
    ///                  domain will start at `parameter`.</param>
    ///
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member CurveSeam(curve_id:Guid, parameter:float) : bool =
        Curve.curveSeam parameter curve_id

    ///<summary>Returns the start point of a curve object</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///<param name="point">(Point3d) Optional, Default Value:None, new start point</param>
    ///
    ///<returns>(Point3d) The 3D starting point of the curve .</returns>
    static member CurveStartPoint(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int, [<OPT;DEF(null)>]point:Point3d) : Point3d =
        Curve.curveStartPoint point segment_index curve_id

    ///<summary>Calculates intersection of a curve object with a surface object.
    ///    Note, this function works on the untrimmed portion of the surface.</summary>
    ///<param name="curve_id">(Guid) The identifier of the first curve object.</param>
    ///<param name="surface_id">(Guid) The identifier of the second curve object. If omitted,
    ///          the a self-intersection test will be performed on curve.</param>
    ///<param name="tolerance">(float) Optional, Default Value:-1, The absolute tolerance in drawing units. If omitted,
    ///          the document's current absolute tolerance is used.</param>
    ///<param name="angle_tolerance">(float) Optional, Default Value:-1, angle tolerance in degrees. The angle
    ///          tolerance is used to determine when the curve is tangent to the
    ///          surface. If omitted, the document's current angle tolerance is used.</param>
    ///
    ///<returns>((int*Point3d*Point3d*Point3d*int*int*int*int*int*int) array) of intersection information .
    ///        The list will contain one or more of the following elements:
    ///          Element Type     Description
    ///          [n][0]  Number   The intersection event type, either Point(1) or Overlap(2).
    ///          [n][1]  Point3d  If the event type is Point(1), then the intersection point
    ///                          on the first curve. If the event type is Overlap(2), then
    ///                          intersection start point on the first curve.
    ///          [n][2]  Point3d  If the event type is Point(1), then the intersection point
    ///                          on the first curve. If the event type is Overlap(2), then
    ///                          intersection end point on the first curve.
    ///          [n][3]  Point3d  If the event type is Point(1), then the intersection point
    ///                          on the second curve. If the event type is Overlap(2), then
    ///                          intersection start point on the surface.
    ///          [n][4]  Point3d  If the event type is Point(1), then the intersection point
    ///                          on the second curve. If the event type is Overlap(2), then
    ///                          intersection end point on the surface.
    ///          [n][5]  Number   If the event type is Point(1), then the first curve parameter.
    ///                          If the event type is Overlap(2), then the start value of the
    ///                          first curve parameter range.
    ///          [n][6]  Number   If the event type is Point(1), then the first curve parameter.
    ///                          If the event type is Overlap(2), then the end value of the
    ///                          curve parameter range.
    ///          [n][7]  Number   If the event type is Point(1), then the U surface parameter.
    ///                          If the event type is Overlap(2), then the U surface parameter
    ///                          for curve at (n, 5).
    ///          [n][8]  Number   If the event type is Point(1), then the V surface parameter.
    ///                          If the event type is Overlap(2), then the V surface parameter
    ///                          for curve at (n, 5).
    ///          [n][9]  Number   If the event type is Point(1), then the U surface parameter.
    ///                          If the event type is Overlap(2), then the U surface parameter
    ///                          for curve at (n, 6).
    ///          [n][10] Number   If the event type is Point(1), then the V surface parameter.
    ///                          If the event type is Overlap(2), then the V surface parameter
    ///                          for curve at (n, 6).</returns>
    static member CurveSurfaceIntersection(curve_id:Guid, surface_id:Guid, [<OPT;DEF(-1)>]tolerance:float, [<OPT;DEF(-1)>]angle_tolerance:float) : (int*Point3d*Point3d*Point3d*int*int*int*int*int*int) array =
        Curve.curveSurfaceIntersection angle_tolerance tolerance surface_id curve_id

    ///<summary>Returns a 3D vector that is the tangent to a curve at a parameter.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="parameter">(float) parameter to evaluate</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Vector3d) A 3D vector .</returns>
    static member CurveTangent(curve_id:Guid, parameter:float, [<OPT;DEF(-1)>]segment_index:int) : Vector3d =
        Curve.curveTangent segment_index parameter curve_id

    ///<summary>Returns list of weights that are assigned to the control points of a curve</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(float) The weight values of the curve .</returns>
    static member CurveWeights(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : float =
        Curve.curveWeights segment_index curve_id

    ///<summary>Divides a curve such that the linear distance between the points is equal.</summary>
    ///<param name="curve_id">(Guid) the object's identifier</param>
    ///<param name="distance">(int) linear distance between division points</param>
    ///<param name="create_points">(bool) Optional, Default Value:False, create the division points if True.</param>
    ///<param name="return_points">(bool) Optional, Default Value:True, If True, return a list of points.
    ///                                      If False, return a list of curve parameters</param>
    ///
    ///<returns>(point|number seq) points or curve parameters based on the value of return_points
    ///      none on error</returns>
    static member DivideCurveEquidistant(curve_id:Guid, distance:int, [<OPT;DEF(false)>]create_points:bool, [<OPT;DEF(true)>]return_points:bool) : point|number seq =
        Curve.divideCurveEquidistant return_points create_points distance curve_id

    ///<summary>Returns the center point of an elliptical-shaped curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object.</param>
    ///
    ///<returns>(Point3d) The 3D center point of the ellipse .</returns>
    static member EllipseCenterPoint(curve_id:Guid) : Point3d =
        Curve.ellipseCenterPoint curve_id

    ///<summary>Returns the quadrant points of an elliptical-shaped curve object.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object.</param>
    ///
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four points identifying the quadrants of the ellipse</returns>
    static member EllipseQuadPoints(curve_id:Guid) : Point3d * Point3d * Point3d * Point3d =
        Curve.ellipseQuadPoints curve_id

    ///<summary>Evaluates a curve at a parameter and returns a 3D point</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="t">(float) the parameter to evaluate</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(Point3d) a 3-D point</returns>
    static member EvaluateCurve(curve_id:Guid, t:float, [<OPT;DEF(-1)>]segment_index:int) : Point3d =
        Curve.evaluateCurve segment_index t curve_id

    ///<summary>Explodes, or un-joins, one curves. Polycurves will be exploded into curve
    ///    segments. Polylines will be exploded into line segments. ExplodeCurves will
    ///    return the curves in topological order. </summary>
    ///<param name="curve_ids">(Guid) the curve object(s) to explode.</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, Delete input objects after exploding if True.</param>
    ///
    ///<returns>(Guid seq) identifying the newly created curve objects</returns>
    static member ExplodeCurves(curve_ids:Guid, [<OPT;DEF(false)>]delete_input:bool) : Guid seq =
        Curve.explodeCurves delete_input curve_ids

    ///<summary>Extends a non-closed curve object by a line, arc, or smooth extension
    ///    until it intersects a collection of objects.</summary>
    ///<param name="curve_id">(Guid) identifier of curve to extend</param>
    ///<param name="extension_type">(int) 0 = line
    ///        1 = arc
    ///        2 = smooth</param>
    ///<param name="side">(int) 0=extend from the start of the curve
    ///        1=extend from the end of the curve
    ///        2=extend from both the start and the end of the curve</param>
    ///<param name="boundary_object_ids">(Guid) curve, surface, and polysurface objects to extend to</param>
    ///
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member ExtendCurve(curve_id:Guid, extension_type:int, side:int, boundary_object_ids:Guid) : Guid =
        Curve.extendCurve boundary_object_ids side extension_type curve_id

    ///<summary>Extends a non-closed curve by a line, arc, or smooth extension for a
    ///    specified distance</summary>
    ///<param name="curve_id">(Guid) curve to extend</param>
    ///<param name="extension_type">(int) 0 = line
    ///        1 = arc
    ///        2 = smooth</param>
    ///<param name="side">(int) 0=extend from start of the curve
    ///        1=extend from end of the curve
    ///        2=Extend from both ends</param>
    ///<param name="length">(float) distance to extend</param>
    ///
    ///<returns>(Guid) The identifier of the new object</returns>
    static member ExtendCurveLength(curve_id:Guid, extension_type:int, side:int, length:float) : Guid =
        Curve.extendCurveLength length side extension_type curve_id

    ///<summary>Extends a non-closed curve by smooth extension to a point</summary>
    ///<param name="curve_id">(Guid) curve to extend</param>
    ///<param name="side">(int) 0=extend from start of the curve
    ///        1=extend from end of the curve</param>
    ///<param name="point">(Point3d) point to extend to</param>
    ///
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member ExtendCurvePoint(curve_id:Guid, side:int, point:Point3d) : Guid =
        Curve.extendCurvePoint point side curve_id

    ///<summary>Fairs a curve. Fair works best on degree 3 (cubic) curves. Fair attempts
    ///    to remove large curvature variations while limiting the geometry changes to
    ///    be no more than the specified tolerance. Sometimes several applications of
    ///    this method are necessary to remove nasty curvature problems.</summary>
    ///<param name="curve_id">(Guid) curve to fair</param>
    ///<param name="tolerance">(float) Optional, Default Value:1.0, fairing tolerance</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member FairCurve(curve_id:Guid, [<OPT;DEF(1.0)>]tolerance:float) : bool =
        Curve.fairCurve tolerance curve_id

    ///<summary>Reduces number of curve control points while maintaining the curve's same
    ///    general shape. Use this function for replacing curves with many control
    ///    points. For more information, see the Rhino help for the FitCrv command.</summary>
    ///<param name="curve_id">(Guid) Identifier of the curve object</param>
    ///<param name="degree">(int) Optional, Default Value:3, The curve degree, which must be greater than 1.
    ///                     The default is 3.</param>
    ///<param name="distance_tolerance">(float) Optional, Default Value:-1, The fitting tolerance. If distance_tolerance
    ///          is not specified or <= 0.0, the document absolute tolerance is used.</param>
    ///<param name="angle_tolerance">(float) Optional, Default Value:-1, The kink smoothing tolerance in degrees. If
    ///          angle_tolerance is 0.0, all kinks are smoothed. If angle_tolerance
    ///          is > 0.0, kinks smaller than angle_tolerance are smoothed. If
    ///          angle_tolerance is not specified or < 0.0, the document angle
    ///          tolerance is used for the kink smoothing.</param>
    ///
    ///<returns>(Guid) The identifier of the new object</returns>
    static member FitCurve(curve_id:Guid, [<OPT;DEF(3)>]degree:int, [<OPT;DEF(-1)>]distance_tolerance:float, [<OPT;DEF(-1)>]angle_tolerance:float) : Guid =
        Curve.fitCurve angle_tolerance distance_tolerance degree curve_id

    ///<summary>Inserts a knot into a curve object</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="parameter">(float) parameter on the curve</param>
    ///<param name="symmetrical">(bool) Optional, Default Value:False, if True, then knots are added on both sides of
    ///          the center of the curve</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member InsertCurveKnot(curve_id:Guid, parameter:float, [<OPT;DEF(false)>]symmetrical:bool) : bool =
        Curve.insertCurveKnot symmetrical parameter curve_id

    ///<summary>Verifies an object is an open arc curve</summary>
    ///<param name="curve_id">(Guid) Identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsArc(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isArc segment_index curve_id

    ///<summary>Verifies an object is a circle curve</summary>
    ///<param name="curve_id">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, If the curve is not a circle, then the tolerance used
    ///        to determine whether or not the NURBS form of the curve has the
    ///        properties of a circle. If omitted, Rhino's internal zero tolerance is used</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsCircle(curve_id:Guid, [<OPT;DEF(null)>]tolerance:float) : bool =
        Curve.isCircle tolerance curve_id

    ///<summary>Verifies an object is a curve</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsCurve(object_id:Guid) : bool =
        Curve.isCurve object_id

    ///<summary>Decide if it makes sense to close off the curve by moving the end point
    ///    to the start point based on start-end gap size and length of curve as
    ///    approximated by chord defined by 6 points</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, maximum allowable distance between start point and end
    ///        point. If omitted, the document's current absolute tolerance is used</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsCurveClosable(curve_id:Guid, [<OPT;DEF(null)>]tolerance:float) : bool =
        Curve.isCurveClosable tolerance curve_id

    ///<summary>Verifies an object is a closed curve object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True  otherwise False.</returns>
    static member IsCurveClosed(object_id:Guid) : bool =
        Curve.isCurveClosed object_id

    ///<summary>Test a curve to see if it lies in a specific plane</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///<param name="plane">(Plane) Optional, Default Value:None, plane to test. If omitted, the active construction plane is used</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsCurveInPlane(object_id:Guid, [<OPT;DEF(null)>]plane:Plane) : bool =
        Curve.isCurveInPlane plane object_id

    ///<summary>Verifies an object is a linear curve</summary>
    ///<param name="object_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurveLinear(object_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isCurveLinear segment_index object_id

    ///<summary>Verifies an object is a periodic curve object</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsCurvePeriodic(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isCurvePeriodic segment_index curve_id

    ///<summary>Verifies an object is a planar curve</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurvePlanar(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isCurvePlanar segment_index curve_id

    ///<summary>Verifies an object is a rational NURBS curve</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurveRational(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isCurveRational segment_index curve_id

    ///<summary>Verifies an object is an elliptical-shaped curve</summary>
    ///<param name="object_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsEllipse(object_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isEllipse segment_index object_id

    ///<summary>Verifies an object is a line curve</summary>
    ///<param name="object_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsLine(object_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isLine segment_index object_id

    ///<summary>Verifies that a point is on a curve</summary>
    ///<param name="object_id">(Guid) identifier of the curve object</param>
    ///<param name="point">(Point3d) the test point</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsPointOnCurve(object_id:Guid, point:Point3d, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isPointOnCurve segment_index point object_id

    ///<summary>Verifies an object is a PolyCurve curve</summary>
    ///<param name="object_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsPolyCurve(object_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isPolyCurve segment_index object_id

    ///<summary>Verifies an object is a Polyline curve object</summary>
    ///<param name="object_id">(Guid) identifier of the curve object</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, the curve segment index if `curve_id` identifies a polycurve</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsPolyline(object_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : bool =
        Curve.isPolyline segment_index object_id

    ///<summary>Joins multiple curves together to form one or more curves or polycurves</summary>
    ///<param name="object_ids">(Guid) list of multiple curves</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, delete input objects after joining</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, join tolerance. If omitted, 2.1 * document absolute
    ///          tolerance is used</param>
    ///
    ///<returns>(Guid seq) Object id representing the new curves</returns>
    static member JoinCurves(object_ids:Guid, [<OPT;DEF(false)>]delete_input:bool, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        Curve.joinCurves tolerance delete_input object_ids

    ///<summary>Returns a line that was fit through an array of 3D points</summary>
    ///<param name="points">(Point3d seq) a list of at least two 3D points</param>
    ///
    ///<returns>(Line) line on success</returns>
    static member LineFitFromPoints(points:Point3d seq) : Line =
        Curve.lineFitFromPoints points

    ///<summary>Makes a periodic curve non-periodic. Non-periodic curves can develop
    ///    kinks when deformed</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, delete the input curve. If omitted, the input curve will not be deleted.</param>
    ///
    ///<returns>(Guid) id of the new or modified curve</returns>
    static member MakeCurveNonPeriodic(curve_id:Guid, [<OPT;DEF(false)>]delete_input:bool) : Guid =
        Curve.makeCurveNonPeriodic delete_input curve_id

    ///<summary>Creates an average curve from two curves</summary>
    ///<param name="curve0">(Guid) curve0 of 'identifiers of two curves' (FIXME 0)</param>
    ///<param name="curve1">(Guid) curve1 of 'identifiers of two curves' (FIXME 0)</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, angle tolerance used to match kinks between curves</param>
    ///
    ///<returns>(Guid) id of the new or modified curve</returns>
    static member MeanCurve(curve0:Guid, curve1:Guid, [<OPT;DEF(null)>]tolerance:float) : Guid =
        Curve.meanCurve tolerance curve1 curve0

    ///<summary>Creates a polygon mesh object based on a closed polyline curve object.
    ///    The created mesh object is added to the document</summary>
    ///<param name="polyline_id">(Guid) identifier of the polyline curve object</param>
    ///
    ///<returns>(Guid) identifier of the new mesh object</returns>
    static member MeshPolyline(polyline_id:Guid) : Guid =
        Curve.meshPolyline polyline_id

    ///<summary>Offsets a curve by a distance. The offset curve will be added to Rhino</summary>
    ///<param name="object_id">(Guid) identifier of a curve object</param>
    ///<param name="direction">(Point3d) point describing direction of the offset</param>
    ///<param name="distance">(float) distance of the offset</param>
    ///<param name="normal">(Vector3d) Optional, Default Value:None, normal of the plane in which the offset will occur.
    ///          If omitted, the normal of the active construction plane will be used</param>
    ///<param name="style">(int) Optional, Default Value:1, the corner style. If omitted, the style is sharp.
    ///                                0 = None
    ///                                1 = Sharp
    ///                                2 = Round
    ///                                3 = Smooth
    ///                                4 = Chamfer</param>
    ///
    ///<returns>(Guid seq) list of ids for the new curves on success</returns>
    static member OffsetCurve(object_id:Guid, direction:Point3d, distance:float, [<OPT;DEF(null)>]normal:Vector3d, [<OPT;DEF(1)>]style:int) : Guid seq =
        Curve.offsetCurve style normal distance direction object_id

    ///<summary>Offset a curve on a surface. The source curve must lie on the surface.
    ///    The offset curve or curves will be added to Rhino</summary>
    ///<param name="curve_id">(Guid) curve identifiers</param>
    ///<param name="surface_id">(Guid) surface identifiers</param>
    ///<param name="distance_or_parameter">(float) ): If a single number is passed, then this is the
    ///        distance of the offset. Based on the curve's direction, a positive value
    ///        will offset to the left and a negative value will offset to the right.
    ///        If a tuple of two values is passed, this is interpreted as the surface
    ///        U,V parameter that the curve will be offset through</param>
    ///
    ///<returns>(Guid seq) identifiers of the new curves</returns>
    static member OffsetCurveOnSurface(curve_id:Guid, surface_id:Guid, distance_or_parameter:float) : Guid seq =
        Curve.offsetCurveOnSurface distance_or_parameter surface_id curve_id

    ///<summary>Determines the relationship between the regions bounded by two coplanar
    ///    simple closed curves</summary>
    ///<param name="curve_a">(Guid) curve a of 'identifiers of two planar, closed curves' (FIXME 0)</param>
    ///<param name="curve_b">(Guid) curve b of 'identifiers of two planar, closed curves' (FIXME 0)</param>
    ///<param name="plane">(Plane) Optional, Default Value:None, test plane. If omitted, the currently active construction
    ///        plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, if omitted, the document absolute tolerance is used</param>
    ///
    ///<returns>(float) a number identifying the relationship 
    ///        0 = the regions bounded by the curves are disjoint
    ///        1 = the two curves intersect
    ///        2 = the region bounded by curve_a is inside of curve_b
    ///        3 = the region bounded by curve_b is inside of curve_a</returns>
    static member PlanarClosedCurveContainment(curve_a:Guid, curve_b:Guid, [<OPT;DEF(null)>]plane:Plane, [<OPT;DEF(null)>]tolerance:float) : float =
        Curve.planarClosedCurveContainment tolerance plane curve_b curve_a

    ///<summary>Determines if two coplanar curves intersect</summary>
    ///<param name="curve_a">(Guid) curve a of 'identifiers of two planar curves' (FIXME 0)</param>
    ///<param name="curve_b">(Guid) curve b of 'identifiers of two planar curves' (FIXME 0)</param>
    ///<param name="plane">(Plane) Optional, Default Value:None, test plane. If omitted, the currently active construction
    ///        plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, if omitted, the document absolute tolerance is used</param>
    ///
    ///<returns>(bool) True if the curves intersect; otherwise False</returns>
    static member PlanarCurveCollision(curve_a:Guid, curve_b:Guid, [<OPT;DEF(null)>]plane:Plane, [<OPT;DEF(null)>]tolerance:float) : bool =
        Curve.planarCurveCollision tolerance plane curve_b curve_a

    ///<summary>Determines if a point is inside of a closed curve, on a closed curve, or
    ///    outside of a closed curve</summary>
    ///<param name="point">(Point3d) text point</param>
    ///<param name="curve">(Guid) identifier of a curve object</param>
    ///<param name="plane">(Plane) Optional, Default Value:None, plane containing the closed curve and point. If omitted,
    ///          the currently active construction plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, it omitted, the document abosulte tolerance is used</param>
    ///
    ///<returns>(float) number identifying the result 
    ///              0 = point is outside of the curve
    ///              1 = point is inside of the curve
    ///              2 = point in on the curve</returns>
    static member PointInPlanarClosedCurve(point:Point3d, curve:Guid, [<OPT;DEF(null)>]plane:Plane, [<OPT;DEF(null)>]tolerance:float) : float =
        Curve.pointInPlanarClosedCurve tolerance plane curve point

    ///<summary>Returns the number of curve segments that make up a polycurve</summary>
    ///<param name="curve_id">(Guid) the object's identifier</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, if `curve_id` identifies a polycurve object, then `segment_index` identifies the curve segment of the polycurve to query.</param>
    ///
    ///<returns>(int) the number of curve segments in a polycurve</returns>
    static member PolyCurveCount(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : int =
        Curve.polyCurveCount segment_index curve_id

    ///<summary>Returns the vertices of a polyline curve on success</summary>
    ///<param name="curve_id">(Guid) the object's identifier</param>
    ///<param name="segment_index">(int) Optional, Default Value:-1, if curve_id identifies a polycurve object, then segment_index identifies the curve segment of the polycurve to query.</param>
    ///
    ///<returns>(Point3d seq) an list of Point3d vertex points</returns>
    static member PolylineVertices(curve_id:Guid, [<OPT;DEF(-1)>]segment_index:int) : Point3d seq =
        Curve.polylineVertices segment_index curve_id

    ///<summary>Projects one or more curves onto one or more surfaces or meshes</summary>
    ///<param name="curve_ids">(Guid seq) identifiers of curves to project</param>
    ///<param name="mesh_ids">(Guid seq) identifiers of meshes to project onto</param>
    ///<param name="direction">(Vector3d) projection direction</param>
    ///
    ///<returns>(Guid seq) list of identifiers for the resulting curves.</returns>
    static member ProjectCurveToMesh(curve_ids:Guid seq, mesh_ids:Guid seq, direction:Vector3d) : Guid seq =
        Curve.projectCurveToMesh direction mesh_ids curve_ids

    ///<summary>Projects one or more curves onto one or more surfaces or polysurfaces</summary>
    ///<param name="curve_ids">(Guid seq) identifiers of curves to project</param>
    ///<param name="surface_ids">(Guid seq) identifiers of surfaces to project onto</param>
    ///<param name="direction">(Vector3d) projection direction</param>
    ///
    ///<returns>(Guid seq) list of identifiers</returns>
    static member ProjectCurveToSurface(curve_ids:Guid seq, surface_ids:Guid seq, direction:Vector3d) : Guid seq =
        Curve.projectCurveToSurface direction surface_ids curve_ids

    ///<summary>Rebuilds a curve to a given degree and control point count. For more
    ///    information, see the Rhino help for the Rebuild command.</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///<param name="degree">(int) Optional, Default Value:3, new degree (must be greater than 0)</param>
    ///<param name="point_count">(int) Optional, Default Value:10, new point count, which must be bigger than degree.</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RebuildCurve(curve_id:Guid, [<OPT;DEF(3)>]degree:int, [<OPT;DEF(10)>]point_count:int) : bool =
        Curve.rebuildCurve point_count degree curve_id

    ///<summary>Deletes a knot from a curve object.</summary>
    ///<param name="curve">(Guid) The reference of the source object</param>
    ///<param name="parameter">(float) The parameter on the curve. Note, if the parameter is not equal to one
    ///                      of the existing knots, then the knot closest to the specified parameter
    ///                      will be removed.</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RemoveCurveKnot(curve:Guid, parameter:float) : bool =
        Curve.removeCurveKnot parameter curve

    ///<summary>Reverses the direction of a curve object. Same as Rhino's Dir command</summary>
    ///<param name="curve_id">(Guid) identifier of the curve object</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ReverseCurve(curve_id:Guid) : bool =
        Curve.reverseCurve curve_id

    ///<summary>Replace a curve with a geometrically equivalent polycurve.
    ///    The polycurve will have the following properties:
    ///     - All the polycurve segments are lines, polylines, arcs, or NURBS curves.
    ///     - The NURBS curves segments do not have fully multiple interior knots.
    ///     - Rational NURBS curves do not have constant weights.
    ///     - Any segment for which IsCurveLinear or IsArc is True is a line, polyline segment, or an arc.
    ///     - Adjacent co-linear or co-circular segments are combined.
    ///     - Segments that meet with G1-continuity have there ends tuned up so that they meet with G1-continuity to within machine precision.
    ///     - If the polycurve is a polyline, a polyline will be created</summary>
    ///<param name="curve_id">(Guid) the object's identifier</param>
    ///<param name="flags">(int) Optional, Default Value:0, the simplification methods to use. By default, all methods are used (flags = 0)
    ///        Value Description
    ///        0     Use all methods.
    ///        1     Do not split NURBS curves at fully multiple knots.
    ///        2     Do not replace segments with IsCurveLinear = True with line curves.
    ///        4     Do not replace segments with IsArc = True with arc curves.
    ///        8     Do not replace rational NURBS curves with constant denominator with an equivalent non-rational NURBS curve.
    ///        16    Do not adjust curves at G1-joins.
    ///        32    Do not merge adjacent co-linear lines or co-circular arcs or combine consecutive line segments into a polyline.</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member SimplifyCurve(curve_id:Guid, [<OPT;DEF(0)>]flags:int) : bool =
        Curve.simplifyCurve flags curve_id

    ///<summary>Splits, or divides, a curve at a specified parameter. The parameter must
    ///    be in the interior of the curve's domain</summary>
    ///<param name="curve_id">(Guid) the curve to split</param>
    ///<param name="parameter">(float seq) one or more parameters to split the curve at</param>
    ///<param name="delete_input">(bool) Optional, Default Value:True, delete the input curve</param>
    ///
    ///<returns>(Guid seq) list of new curves on success</returns>
    static member SplitCurve(curve_id:Guid, parameter:float seq, [<OPT;DEF(true)>]delete_input:bool) : Guid seq =
        Curve.splitCurve delete_input parameter curve_id

    ///<summary>Trims a curve by removing portions of the curve outside a specified interval</summary>
    ///<param name="curve_id">(Guid) the curve to trim</param>
    ///<param name="interval">(float * float) two numbers identifying the interval to keep. Portions of
    ///        the curve before domain[0] and after domain[1] will be removed. If the
    ///        input curve is open, the interval must be increasing. If the input
    ///        curve is closed and the interval is decreasing, then the portion of
    ///        the curve across the start and end of the curve is returned</param>
    ///<param name="delete_input">(bool) Optional, Default Value:True, delete the input curve. If omitted the input curve is deleted.</param>
    ///
    ///<returns>(Guid seq) identifier of the new curve on success</returns>
    static member TrimCurve(curve_id:Guid, interval:float * float, [<OPT;DEF(true)>]delete_input:bool) : Guid seq =
        Curve.trimCurve delete_input interval curve_id

    ///<summary>Changes the degree of a curve object. For more information see the Rhino help file for the ChangeDegree command.</summary>
    ///<param name="object_id">(Guid) the object's identifier.</param>
    ///<param name="degree">(int) the new degree.</param>
    ///
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member ChangeCurveDegree(object_id:Guid, degree:int) : bool =
        Curve.changeCurveDegree degree object_id

    ///<summary>Creates curves between two open or closed input curves.</summary>
    ///<param name="from_curve_id">(Guid) identifier of the first curve object.</param>
    ///<param name="to_curve_id">(Guid) identifier of the second curve object.</param>
    ///<param name="number_of_curves">(float) Optional, Default Value:1, The number of curves to create. The default is 1.</param>
    ///<param name="method">(float) Optional, Default Value:0, The method for refining the output curves, where:
    ///        0: (Default) Uses the control points of the curves for matching. So the first control point of first curve is matched to first control point of the second curve.
    ///        1: Refits the output curves like using the FitCurve method.  Both the input curve and the output curve will have the same structure. The resulting curves are usually more complex than input unless input curves are compatible.
    ///        2: Input curves are divided to the specified number of points on the curve, corresponding points define new points that output curves go through. If you are making one tween curve, the method essentially does the following: divides the two curves into an equal number of points, finds the midpoint between the corresponding points on the curves, and interpolates the tween curve through those points.</param>
    ///<param name="sample_number">(float) Optional, Default Value:10, The number of samples points to use if method is 2. The default is 10.</param>
    ///
    ///<returns>(Guid seq) The identifiers of the new tween objects , .</returns>
    static member AddTweenCurves(from_curve_id:Guid, to_curve_id:Guid, [<OPT;DEF(1)>]number_of_curves:float, [<OPT;DEF(0)>]method:float, [<OPT;DEF(10)>]sample_number:float) : Guid seq =
        Curve.addTweenCurves sample_number method number_of_curves to_curve_id from_curve_id

    ///<summary>Adds an aligned dimension object to the document. An aligned dimension
    ///    is a linear dimension lined up with two points</summary>
    ///<param name="start_point">(Point3d) first point of dimension</param>
    ///<param name="end_point">(Point3d) second point of dimension</param>
    ///<param name="point_on_dimension_line">(Point3d) location point of dimension line</param>
    ///<param name="style">(string) Optional, Default Value:None, name of dimension style</param>
    ///
    ///<returns>(Guid) identifier of new dimension on success</returns>
    static member AddAlignedDimension(start_point:Point3d, end_point:Point3d, point_on_dimension_line:Point3d, [<OPT;DEF(null)>]style:string) : Guid =
        Dimension.addAlignedDimension style point_on_dimension_line end_point start_point

    ///<summary>Adds a new dimension style to the document. The new dimension style will
    ///    be initialized with the current default dimension style properties.</summary>
    ///<param name="dimstyle_name">(string) Optional, Default Value:None, name of the new dimension style. If omitted, Rhino automatically generates the dimension style name</param>
    ///
    ///<returns>(string) name of the new dimension style on success</returns>
    static member AddDimStyle([<OPT;DEF(null)>]dimstyle_name:string) : string =
        Dimension.addDimStyle dimstyle_name

    ///<summary>Adds a leader to the document. Leader objects are planar.
    ///    The 3D points passed to this function should be co-planar</summary>
    ///<param name="points">(Point3d seq) list of (at least 2) 3D points</param>
    ///<param name="view_or_plane">(string) Optional, Default Value:None, If a view name is specified, points will be constrained
    ///        to the view's construction plane. If a view is not specified, points
    ///        will be constrained to a plane fit through the list of points</param>
    ///<param name="text">(string) Optional, Default Value:None, leader's text string</param>
    ///
    ///<returns>(Guid) identifier of the new leader on success</returns>
    static member AddLeader(points:Point3d seq, [<OPT;DEF(null)>]view_or_plane:string, [<OPT;DEF(null)>]text:string) : Guid =
        Dimension.addLeader text view_or_plane points

    ///<summary>Adds a linear dimension to the document</summary>
    ///<param name="plane">(Plane) The plane on which the dimension will lie.</param>
    ///<param name="start_point">(Point3d) The origin, or first point of the dimension.</param>
    ///<param name="end_point">(Point3d) The offset, or second point of the dimension.</param>
    ///<param name="point_on_dimension_line">(Point3d) A point that lies on the dimension line.</param>
    ///
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddLinearDimension(plane:Plane, start_point:Point3d, end_point:Point3d, point_on_dimension_line:Point3d) : Guid =
        Dimension.addLinearDimension point_on_dimension_line end_point start_point plane

    ///<summary>Removes an existing dimension style from the document. The dimension style
    ///    to be removed cannot be referenced by any dimension objects.</summary>
    ///<param name="dimstyle_name">(string) the name of an unreferenced dimension style</param>
    ///
    ///<returns>(string) The name of the deleted dimension style</returns>
    static member DeleteDimStyle(dimstyle_name:string) : string =
        Dimension.deleteDimStyle dimstyle_name

    ///<summary>Returns the text displayed by a dimension object</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///
    ///<returns>(string) the text displayed by a dimension object</returns>
    static member DimensionText(object_id:Guid) : string =
        Dimension.dimensionText object_id

    ///<summary>Returns the value of a dimension object</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///
    ///<returns>(float) numeric value of the dimension</returns>
    static member DimensionValue(object_id:Guid) : float =
        Dimension.dimensionValue object_id

    ///<summary>Returns the number of dimension styles in the document</summary>
    ///
    ///<returns>(int) the number of dimension styles in the document</returns>
    static member DimStyleCount() : int =
        Dimension.dimStyleCount ()

    ///<summary>Returns the names of all dimension styles in the document</summary>
    ///
    ///<returns>(string seq) the names of all dimension styles in the document</returns>
    static member DimStyleNames() : string seq =
        Dimension.dimStyleNames ()

    ///<summary>Verifies an object is an aligned dimension object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsAlignedDimension(object_id:Guid) : bool =
        Dimension.isAlignedDimension object_id

    ///<summary>Verifies an object is an angular dimension object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsAngularDimension(object_id:Guid) : bool =
        Dimension.isAngularDimension object_id

    ///<summary>Verifies an object is a diameter dimension object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsDiameterDimension(object_id:Guid) : bool =
        Dimension.isDiameterDimension object_id

    ///<summary>Verifies an object is a dimension object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsDimension(object_id:Guid) : bool =
        Dimension.isDimension object_id

    ///<summary>Verifies the existance of a dimension style in the document</summary>
    ///<param name="dimstyle">(string) the name of a dimstyle to test for</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsDimStyle(dimstyle:string) : bool =
        Dimension.isDimStyle dimstyle

    ///<summary>Verifies that an existing dimension style is from a reference file</summary>
    ///<param name="dimstyle">(string) the name of an existing dimension style</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsDimStyleReference(dimstyle:string) : bool =
        Dimension.isDimStyleReference dimstyle

    ///<summary>Verifies an object is a dimension leader object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsLeader(object_id:Guid) : bool =
        Dimension.isLeader object_id

    ///<summary>Verifies an object is a linear dimension object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsLinearDimension(object_id:Guid) : bool =
        Dimension.isLinearDimension object_id

    ///<summary>Verifies an object is an ordinate dimension object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsOrdinateDimension(object_id:Guid) : bool =
        Dimension.isOrdinateDimension object_id

    ///<summary>Verifies an object is a radial dimension object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsRadialDimension(object_id:Guid) : bool =
        Dimension.isRadialDimension object_id

    ///<summary>Renames an existing dimension style</summary>
    ///<param name="oldstyle">(string) the name of an existing dimension style</param>
    ///<param name="newstyle">(string) the new dimension style name</param>
    ///
    ///<returns>(string) the new dimension style name</returns>
    static member RenameDimStyle(oldstyle:string, newstyle:string) : string =
        Dimension.renameDimStyle newstyle oldstyle

    ///<summary>Create a bitmap preview image of the current model</summary>
    ///<param name="filename">(string) name of the bitmap file to create</param>
    ///<param name="view">(string) Optional, Default Value:None, title of the view. If omitted, the active view is used</param>
    ///<param name="size">(float) Optional, Default Value:None, two integers that specify width and height of the bitmap</param>
    ///<param name="flags">(int) Optional, Default Value:0, Bitmap creation flags. Can be the combination of:
    ///          1 = honor object highlighting
    ///          2 = draw construction plane
    ///          4 = use ghosted shading</param>
    ///<param name="wireframe">(bool) Optional, Default Value:False, If True then a wireframe preview image. If False,
    ///          a rendered image will be created</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CreatePreviewImage(filename:string, [<OPT;DEF(null)>]view:string, [<OPT;DEF(null)>]size:float, [<OPT;DEF(0)>]flags:int, [<OPT;DEF(false)>]wireframe:bool) : bool =
        Document.createPreviewImage wireframe flags size view filename

    ///<summary>Returns the name of the currently loaded Rhino document (3DM file)</summary>
    ///
    ///<returns>(string) the name of the currently loaded Rhino document (3DM file)</returns>
    static member DocumentName() : string =
        Document.documentName ()

    ///<summary>Returns path of the currently loaded Rhino document (3DM file)</summary>
    ///
    ///<returns>(string) the path of the currently loaded Rhino document (3DM file)</returns>
    static member DocumentPath() : string =
        Document.documentPath ()

    ///<summary>Enables or disables screen redrawing</summary>
    ///<param name="enable">(bool) Optional, Default Value:True, True to enable, False to disable</param>
    ///
    ///<returns>(unit) unit</returns>
    static member EnableRedraw([<OPT;DEF(true)>]enable:bool) : unit =
        Document.enableRedraw enable

    ///<summary>Extracts the bitmap preview image from the specified model (.3dm)</summary>
    ///<param name="filename">(string) name of the bitmap file to create. The extension of
    ///         the filename controls the format of the bitmap file created.
    ///         (.bmp, .tga, .jpg, .jpeg, .pcx, .png, .tif, .tiff)</param>
    ///<param name="modelname">(string) Optional, Default Value:None, The model (.3dm) from which to extract the
    ///         preview image. If omitted, the currently loaded model is used.</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ExtractPreviewImage(filename:string, [<OPT;DEF(null)>]modelname:string) : bool =
        Document.extractPreviewImage modelname filename

    ///<summary>Verifies that the current document has been modified in some way</summary>
    ///
    ///<returns>(bool) True or False.</returns>
    static member IsDocumentModified() : bool =
        Document.isDocumentModified ()

    ///<summary>Returns the file version of the current document. Use this function to
    ///    determine which version of Rhino last saved the document. Note, this
    ///    function will not return values from referenced or merged files.</summary>
    ///
    ///<returns>(string) the file version of the current document</returns>
    static member ReadFileVersion() : string =
        Document.readFileVersion ()

    ///<summary>Redraws all views</summary>
    ///
    ///<returns>(unit) </returns>
    static member Redraw() : unit =
        Document.redraw ()

    ///<summary>Return the scale factor for changing between unit systems.</summary>
    ///<param name="to_system">(int) The unit system to convert to. The unit systems are are:
    ///       0 - No unit system
    ///       1 - Microns (1.0e-6 meters)
    ///       2 - Millimeters (1.0e-3 meters)
    ///       3 - Centimeters (1.0e-2 meters)
    ///       4 - Meters
    ///       5 - Kilometers (1.0e+3 meters)
    ///       6 - Microinches (2.54e-8 meters, 1.0e-6 inches)
    ///       7 - Mils (2.54e-5 meters, 0.001 inches)
    ///       8 - Inches (0.0254 meters)
    ///       9 - Feet (0.3408 meters, 12 inches)
    ///      10 - Miles (1609.344 meters, 5280 feet)
    ///      11 - *Reserved for custom Unit System*
    ///      12 - Angstroms (1.0e-10 meters)
    ///      13 - Nanometers (1.0e-9 meters)
    ///      14 - Decimeters (1.0e-1 meters)
    ///      15 - Dekameters (1.0e+1 meters)
    ///      16 - Hectometers (1.0e+2 meters)
    ///      17 - Megameters (1.0e+6 meters)
    ///      18 - Gigameters (1.0e+9 meters)
    ///      19 - Yards (0.9144  meters, 36 inches)
    ///      20 - Printer point (1/72 inches, computer points)
    ///      21 - Printer pica (1/6 inches, (computer picas)
    ///      22 - Nautical mile (1852 meters)
    ///      23 - Astronomical (1.4959787e+11)
    ///      24 - Lightyears (9.46073e+15 meters)
    ///      25 - Parsecs (3.08567758e+16)</param>
    ///<param name="from_system">(int) Optional, Default Value:None, the unit system to convert from (see above). If omitted,
    ///        the document's current unit system is used</param>
    ///
    ///<returns>(float) scale factor for changing between unit systems</returns>
    static member UnitScale(to_system:int, [<OPT;DEF(null)>]from_system:int) : float =
        Document.unitScale from_system to_system

    ///<summary>Returns the name of the current unit system</summary>
    ///<param name="capitalize">(bool) Optional, Default Value:False, Capitalize the first character of the units system name (e.g. return "Millimeter" instead of "millimeter"). The default is not to capitalize the first character (false).</param>
    ///<param name="singular">(bool) Optional, Default Value:True, Return the singular form of the units system name (e.g. "millimeter" instead of "millimeters"). The default is to return the singular form of the name (true).</param>
    ///<param name="abbreviate">(bool) Optional, Default Value:False, Abbreviate the name of the units system (e.g. return "mm" instead of "millimeter"). The default is not to abbreviate the name (false).</param>
    ///<param name="model_units">(bool) Optional, Default Value:True, Return the document's model units (True) or the document's page units (False). The default is True.</param>
    ///
    ///<returns>(string) The name of the current units system .</returns>
    static member UnitSystemName([<OPT;DEF(false)>]capitalize:bool, [<OPT;DEF(true)>]singular:bool, [<OPT;DEF(false)>]abbreviate:bool, [<OPT;DEF(true)>]model_units:bool) : string =
        Document.unitSystemName model_units abbreviate singular capitalize

    ///<summary>Create a clipping plane for visibly clipping away geometry in a specific
    ///    view. Note, clipping planes are infinite</summary>
    ///<param name="plane">(Plane) the plane</param>
    ///<param name="u_magnitude">(float) u magnitude of 'size of the plane' (FIXME 0)</param>
    ///<param name="v_magnitude">(float) v magnitude of 'size of the plane' (FIXME 0)</param>
    ///<param name="views">(string seq) Optional, Default Value:None, Titles or ids the the view(s) to clip. If omitted, the active
    ///        view is used.</param>
    ///
    ///<returns>(Guid) object identifier on success</returns>
    static member AddClippingPlane(plane:Plane, u_magnitude:float, v_magnitude:float, [<OPT;DEF(null)>]views:string seq) : Guid =
        Geometry.addClippingPlane views v_magnitude u_magnitude plane

    ///<summary>Creates a picture frame and adds it to the document.</summary>
    ///<param name="plane">(Plane) The plane in which the PictureFrame will be created.  The bottom-left corner of picture will be at plane's origin. The width will be in the plane's X axis direction, and the height will be in the plane's Y axis direction.</param>
    ///<param name="filename">(string) The path to a bitmap or image file.</param>
    ///<param name="width">(float) Optional, Default Value:0.0, If both dblWidth and dblHeight = 0, then the width and height of the PictureFrame will be the width and height of the image. If dblWidth = 0 and dblHeight is > 0, or if dblWidth > 0 and dblHeight = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, which ever one is = 0. If both dblWidth and dblHeight are > 0, then these are assumed to be the width and height of in the current unit system.</param>
    ///<param name="height">(float) Optional, Default Value:0.0, If both dblWidth and dblHeight = 0, then the width and height of the PictureFrame will be the width and height of the image. If dblWidth = 0 and dblHeight is > 0, or if dblWidth > 0 and dblHeight = 0, then the non-zero value is assumed to be an aspect ratio of the image's width or height, which ever one is = 0. If both dblWidth and dblHeight are > 0, then these are assumed to be the width and height of in the current unit system.</param>
    ///<param name="self_illumination">(bool) Optional, Default Value:True, If True, then the image mapped to the picture frame plane always displays at full intensity and is not affected by light or shadow.</param>
    ///<param name="embed">(bool) Optional, Default Value:False, If True, then the function adds the image to Rhino's internal bitmap table, thus making the document self-contained.</param>
    ///<param name="use_alpha">(bool) Optional, Default Value:False, If False, the picture frame is created without any transparency texture.  If True, a transparency texture is created with a "mask texture" set to alpha, and an instance of the diffuse texture in the source texture slot.</param>
    ///<param name="make_mesh">(bool) Optional, Default Value:False, If True, the function will make a PictureFrame object from a mesh rather than a plane surface.</param>
    ///
    ///<returns>(Guid) object identifier on success</returns>
    static member AddPictureFrame(plane:Plane, filename:string, [<OPT;DEF(0.0)>]width:float, [<OPT;DEF(0.0)>]height:float, [<OPT;DEF(true)>]self_illumination:bool, [<OPT;DEF(false)>]embed:bool, [<OPT;DEF(false)>]use_alpha:bool, [<OPT;DEF(false)>]make_mesh:bool) : Guid =
        Geometry.addPictureFrame make_mesh use_alpha embed self_illumination height width filename plane

    ///<summary>Adds point object to the document.</summary>
    ///<param name="pointOrX">(point|number) a point3d or X location of point to add</param>
    ///<param name="y">(float) Optional, Default Value:None, Y location of point to add</param>
    ///<param name="z">(float) Optional, Default Value:None, Z location of point to add</param>
    ///
    ///<returns>(Guid) identifier for the object that was added to the doc</returns>
    static member AddPoint(pointOrX:point|number, [<OPT;DEF(null)>]y:float, [<OPT;DEF(null)>]z:float) : Guid =
        Geometry.addPoint z y pointOrX

    ///<summary>Adds point cloud object to the document</summary>
    ///<param name="points">(Point3d seq) list of values where every multiple of three represents a point</param>
    ///<param name="colors">(Drawing.Color seq) Optional, Default Value:None, list of colors to apply to each point</param>
    ///
    ///<returns>(Guid) identifier of point cloud on success</returns>
    static member AddPointCloud(points:Point3d seq, [<OPT;DEF(null)>]colors:Drawing.Color seq) : Guid =
        Geometry.addPointCloud colors points

    ///<summary>Adds one or more point objects to the document</summary>
    ///<param name="points">(Point3d seq) list of points</param>
    ///
    ///<returns>(Guid seq) identifiers of the new objects on success</returns>
    static member AddPoints(points:Point3d seq) : Guid seq =
        Geometry.addPoints points

    ///<summary>Adds a text string to the document</summary>
    ///<param name="text">(string) the text to display</param>
    ///<param name="point_or_plane">(Plane) a 3-D point or the plane on which the text will lie.
    ///          The origin of the plane will be the origin point of the text</param>
    ///<param name="height">(float) Optional, Default Value:1.0, the text height</param>
    ///<param name="font">(string) Optional, Default Value:"Arial", the text font</param>
    ///<param name="font_style">(int) Optional, Default Value:0, any of the following flags
    ///         0 = normal
    ///         1 = bold
    ///         2 = italic
    ///         3 = bold and italic</param>
    ///<param name="justification">(float) Optional, Default Value:None, text justification. Values may be added to create combinations.
    ///         1 = Left
    ///         2 = Center (horizontal)
    ///         4 = Right
    ///         65536 = Bottom
    ///         131072 = Middle (vertical)
    ///         262144 = Top</param>
    ///
    ///<returns>(Guid) identifier for the object that was added to the doc on success</returns>
    static member AddText(text:string, point_or_plane:Plane, [<OPT;DEF(1.0)>]height:float, [<OPT;DEF("Arial")>]font:string, [<OPT;DEF(0)>]font_style:int, [<OPT;DEF(null)>]justification:float) : Guid =
        Geometry.addText justification font_style font height point_or_plane text

    ///<summary>Add a text dot to the document.</summary>
    ///<param name="text">(string) string in dot</param>
    ///<param name="point">(Point3d) A 3D point identifying the origin point.</param>
    ///
    ///<returns>(Guid) The identifier of the new object</returns>
    static member AddTextDot(text:string, point:Point3d) : Guid =
        Geometry.addTextDot point text

    ///<summary>Compute the area of a closed curve, hatch, surface, polysurface, or mesh</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(float) area</returns>
    static member Area(object_id:Guid) : float =
        Geometry.area object_id

    ///<summary>Returns either world axis-aligned or a construction plane axis-aligned
    ///    bounding box of an object or of several objects</summary>
    ///<param name="objects">(Guid seq) The identifiers of the objects</param>
    ///<param name="view_or_plane">(string) Optional, Default Value:None, Title or id of the view that contains the
    ///          construction plane to which the bounding box should be aligned -or-
    ///          user defined plane. If omitted, a world axis-aligned bounding box
    ///          will be calculated</param>
    ///<param name="in_world_coords">(bool) Optional, Default Value:True, return the bounding box as world coordinates or
    ///          construction plane coordinates. Note, this option does not apply to
    ///          world axis-aligned bounding boxes.</param>
    ///
    ///<returns>(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) Eight 3D points that define the bounding box.
    ///           Points returned in counter-clockwise order starting with the bottom rectangle of the box.</returns>
    static member BoundingBox(objects:Guid seq, [<OPT;DEF(null)>]view_or_plane:string, [<OPT;DEF(true)>]in_world_coords:bool) : Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d =
        Geometry.boundingBox in_world_coords view_or_plane objects

    ///<summary>Compares two objects to determine if they are geometrically identical.</summary>
    ///<param name="first">(Guid) The identifier of the first object to compare.</param>
    ///<param name="second">(Guid) The identifier of the second object to compare.</param>
    ///
    ///<returns>(bool) True if the objects are geometrically identical, otherwise False.</returns>
    static member CompareGeometry(first:Guid, second:Guid) : bool =
        Geometry.compareGeometry second first

    ///<summary>Creates outline curves for a given text entity</summary>
    ///<param name="text_id">(Guid) identifier of Text object to explode</param>
    ///<param name="delete">(bool) Optional, Default Value:False, delete the text object after the curves have been created</param>
    ///
    ///<returns>(list(guid)) of outline curves</returns>
    static member ExplodeText(text_id:Guid, [<OPT;DEF(false)>]delete:bool) : list(guid) =
        Geometry.explodeText delete text_id

    ///<summary>Verifies that an object is a clipping plane object</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True if the object with a given id is a clipping plane</returns>
    static member IsClippingPlane(object_id:Guid) : bool =
        Geometry.isClippingPlane object_id

    ///<summary>Verifies an object is a point object.</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True if the object with a given id is a point</returns>
    static member IsPoint(object_id:Guid) : bool =
        Geometry.isPoint object_id

    ///<summary>Verifies an object is a point cloud object.</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True if the object with a given id is a point cloud</returns>
    static member IsPointCloud(object_id:Guid) : bool =
        Geometry.isPointCloud object_id

    ///<summary>Verifies an object is a text object.</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True if the object with a given id is a text object</returns>
    static member IsText(object_id:Guid) : bool =
        Geometry.isText object_id

    ///<summary>Verifies an object is a text dot object.</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True if the object with a given id is a text dot object</returns>
    static member IsTextDot(object_id:Guid) : bool =
        Geometry.isTextDot object_id

    ///<summary>Returns the point count of a point cloud object</summary>
    ///<param name="object_id">(Guid) the point cloud object's identifier</param>
    ///
    ///<returns>(int) number of points</returns>
    static member PointCloudCount(object_id:Guid) : int =
        Geometry.pointCloudCount object_id

    ///<summary>Verifies that a point cloud has hidden points</summary>
    ///<param name="object_id">(Guid) the point cloud object's identifier</param>
    ///
    ///<returns>(bool) True if cloud has hidden points, otherwise False</returns>
    static member PointCloudHasHiddenPoints(object_id:Guid) : bool =
        Geometry.pointCloudHasHiddenPoints object_id

    ///<summary>Verifies that a point cloud has point colors</summary>
    ///<param name="object_id">(Guid) the point cloud object's identifier</param>
    ///
    ///<returns>(bool) True if cloud has point colors, otherwise False</returns>
    static member PointCloudHasPointColors(object_id:Guid) : bool =
        Geometry.pointCloudHasPointColors object_id

    ///<summary>Returns the points of a point cloud object</summary>
    ///<param name="object_id">(Guid) the point cloud object's identifier</param>
    ///
    ///<returns>(Guid seq) list of points</returns>
    static member PointCloudPoints(object_id:Guid) : Guid seq =
        Geometry.pointCloudPoints object_id

    ///<summary>Returns amount indices of points in a point cloud that are near needle_points.</summary>
    ///<param name="pt_cloud">(Point3d seq) the point cloud to be searched, or the "hay stack". This can also be a list of points.</param>
    ///<param name="needle_points">(Point3d seq) a list of points to search in the point_cloud. This can also be specified as a point cloud.</param>
    ///<param name="amount">(int) Optional, Default Value:1, the amount of required closest points. Defaults to 1.</param>
    ///
    ///<returns>(int seq) a list of indices of the found points, if amount equals 1.
    ///        [[int, ...], ...]: nested lists with amount items within a list, with the indices of the found points.</returns>
    static member PointCloudKNeighbors(pt_cloud:Point3d seq, needle_points:Point3d seq, [<OPT;DEF(1)>]amount:int) : int seq =
        Geometry.pointCloudKNeighbors amount needle_points pt_cloud

    ///<summary>Returns a list of lists of point indices in a point cloud that are
    ///    closest to needle_points. Each inner list references all points within or on the surface of a sphere of distance radius.</summary>
    ///<param name="pt_cloud">(Point3d seq) the point cloud to be searched, or the "hay stack". This can also be a list of points.</param>
    ///<param name="needle_points">(Point3d seq) a list of points to search in the point_cloud. This can also be specified as a point cloud.</param>
    ///<param name="distance">(float) the included limit for listing points.</param>
    ///
    ///<returns>(int seq) a list of lists with the indices of the found points.</returns>
    static member PointCloudClosestPoints(pt_cloud:Point3d seq, needle_points:Point3d seq, distance:float) : int seq =
        Geometry.pointCloudClosestPoints distance needle_points pt_cloud

    ///<summary>Enables or disables an object's grips. For curves and surfaces, these are
    ///    also called control points.</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///<param name="enable">(bool) Optional, Default Value:True, if True, the specified object's grips will be turned on.
    ///        Otherwise, they will be turned off</param>
    ///
    ///<returns>(bool) True on success, False on failure</returns>
    static member EnableObjectGrips(object_id:Guid, [<OPT;DEF(true)>]enable:bool) : bool =
        Grips.enableObjectGrips enable object_id

    ///<summary>Prompts the user to pick a single object grip</summary>
    ///<param name="message">(string) Optional, Default Value:None, prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value:False, allow for selection of pre-selected object grip.</param>
    ///<param name="select">(bool) Optional, Default Value:False, select the picked object grip.</param>
    ///
    ///<returns>(Guid * float * Point3d) defining a grip record.
    ///         [0] = identifier of the object that owns the grip
    ///         [1] = index value of the grip
    ///         [2] = location of the grip</returns>
    static member GetObjectGrip([<OPT;DEF(null)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid * float * Point3d =
        Grips.getObjectGrip select preselect message

    ///<summary>Prompts user to pick one or more object grips from one or more objects.</summary>
    ///<param name="message">(string) Optional, Default Value:None, prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value:False, allow for selection of pre-selected object grips</param>
    ///<param name="select">(bool) Optional, Default Value:False, select the picked object grips</param>
    ///
    ///<returns>(Guid seq) containing one or more grip records. Each grip record is a tuple
    ///        [n][0] = identifier of the object that owns the grip
    ///        [n][1] = index value of the grip
    ///        [n][2] = location of the grip</returns>
    static member GetObjectGrips([<OPT;DEF(null)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid seq =
        Grips.getObjectGrips select preselect message

    ///<summary>Returns the next grip index from a specified grip index of an object</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///<param name="index">(int) zero based grip index from which to get the next grip index</param>
    ///<param name="direction">(float * float) Optional, Default Value:0, direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value:True, if True, the next grip index found will be selected</param>
    ///
    ///<returns>(float) index of the next grip on success</returns>
    static member NextObjectGrip(object_id:Guid, index:int, [<OPT;DEF(0)>]direction:float * float, [<OPT;DEF(true)>]enable:bool) : float =
        Grips.nextObjectGrip enable direction index object_id

    ///<summary>Returns number of grips owned by an object</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///
    ///<returns>(int) number of grips</returns>
    static member ObjectGripCount(object_id:Guid) : int =
        Grips.objectGripCount object_id

    ///<summary>Verifies that an object's grips are turned on</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///
    ///<returns>(bool) True or False indicating Grips state</returns>
    static member ObjectGripsOn(object_id:Guid) : bool =
        Grips.objectGripsOn object_id

    ///<summary>Verifies that an object's grips are turned on and at least one grip
    ///    is selected</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ObjectGripsSelected(object_id:Guid) : bool =
        Grips.objectGripsSelected object_id

    ///<summary>Returns the previous grip index from a specified grip index of an object</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///<param name="index">(int) zero based grip index from which to get the previous grip index</param>
    ///<param name="direction">(float * float) Optional, Default Value:0, direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value:True, if True, the next grip index found will be selected</param>
    ///
    ///<returns>(float) index of the next grip on success</returns>
    static member PrevObjectGrip(object_id:Guid, index:int, [<OPT;DEF(0)>]direction:float * float, [<OPT;DEF(true)>]enable:bool) : float =
        Grips.prevObjectGrip enable direction index object_id

    ///<summary>Returns a list of grip indices indentifying an object's selected grips</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///
    ///<returns>(int seq) list of indices on success</returns>
    static member SelectedObjectGrips(object_id:Guid) : int seq =
        Grips.selectedObjectGrips object_id

    ///<summary>Selects a single grip owned by an object. If the object's grips are
    ///    not turned on, the grips will not be selected</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///<param name="index">(int) index of the grip to select</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SelectObjectGrip(object_id:Guid, index:int) : bool =
        Grips.selectObjectGrip index object_id

    ///<summary>Selects an object's grips. If the object's grips are not turned on,
    ///    they will not be selected</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///
    ///<returns>(float) Number of grips selected on success</returns>
    static member SelectObjectGrips(object_id:Guid) : float =
        Grips.selectObjectGrips object_id

    ///<summary>Unselects a single grip owned by an object. If the object's grips are
    ///    not turned on, the grips will not be unselected</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///<param name="index">(int) index of the grip to unselect</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member UnselectObjectGrip(object_id:Guid, index:int) : bool =
        Grips.unselectObjectGrip index object_id

    ///<summary>Unselects an object's grips. Note, the grips will not be turned off.</summary>
    ///<param name="object_id">(Guid) identifier of the object</param>
    ///
    ///<returns>(float) Number of grips unselected on success</returns>
    static member UnselectObjectGrips(object_id:Guid) : float =
        Grips.unselectObjectGrips object_id

    ///<summary>Adds a new empty group to the document</summary>
    ///<param name="group_name">(string) Optional, Default Value:None, name of the new group. If omitted, rhino automatically
    ///          generates the group name</param>
    ///
    ///<returns>(string) name of the new group</returns>
    static member AddGroup([<OPT;DEF(null)>]group_name:string) : string =
        Group.addGroup group_name

    ///<summary>Adds one or more objects to an existing group.</summary>
    ///<param name="object_ids">(Guid seq) list of Strings or Guids representing the object identifiers</param>
    ///<param name="group_name">(string) the name of an existing group</param>
    ///
    ///<returns>(float) number of objects added to the group</returns>
    static member AddObjectsToGroup(object_ids:Guid seq, group_name:string) : float =
        Group.addObjectsToGroup group_name object_ids

    ///<summary>Adds a single object to an existing group.</summary>
    ///<param name="object_id">(Guid) String or Guid representing the object identifier</param>
    ///<param name="group_name">(string) the name of an existing group</param>
    ///
    ///<returns>(bool) True or False representing success or failure</returns>
    static member AddObjectToGroup(object_id:Guid, group_name:string) : bool =
        Group.addObjectToGroup group_name object_id

    ///<summary>Removes an existing group from the document. Reference groups cannot be
    ///    removed. Deleting a group does not delete the member objects</summary>
    ///<param name="group_name">(string) the name of an existing group</param>
    ///
    ///<returns>(bool) True or False representing success or failure</returns>
    static member DeleteGroup(group_name:string) : bool =
        Group.deleteGroup group_name

    ///<summary>Returns the number of groups in the document</summary>
    ///
    ///<returns>(int) the number of groups in the document</returns>
    static member GroupCount() : int =
        Group.groupCount ()

    ///<summary>Returns the names of all the groups in the document
    ///    None if no names exist in the document</summary>
    ///
    ///<returns>(string seq) the names of all the groups in the document.  None if no names exist in the document</returns>
    static member GroupNames() : string seq =
        Group.groupNames ()

    ///<summary>Hides a group of objects. Hidden objects are not visible, cannot be
    ///    snapped to, and cannot be selected</summary>
    ///<param name="group_name">(string) the name of an existing group</param>
    ///
    ///<returns>(int) The number of objects that were hidden</returns>
    static member HideGroup(group_name:string) : int =
        Group.hideGroup group_name

    ///<summary>Verifies the existance of a group</summary>
    ///<param name="group_name">(string) the name of the group to check for</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsGroup(group_name:string) : bool =
        Group.isGroup group_name

    ///<summary>Locks a group of objects. Locked objects are visible and they can be
    ///    snapped to. But, they cannot be selected</summary>
    ///<param name="group_name">(string) the name of an existing group</param>
    ///
    ///<returns>(float) Number of objects that were locked</returns>
    static member LockGroup(group_name:string) : float =
        Group.lockGroup group_name

    ///<summary>Removes a single object from any and all groups that it is a member.
    ///    Neither the object nor the group can be reference objects</summary>
    ///<param name="object_id">(Guid) the object identifier</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RemoveObjectFromAllGroups(object_id:Guid) : bool =
        Group.removeObjectFromAllGroups object_id

    ///<summary>Remove a single object from an existing group</summary>
    ///<param name="object_id">(Guid) the object identifier</param>
    ///<param name="group_name">(string) the name of an existing group</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RemoveObjectFromGroup(object_id:Guid, group_name:string) : bool =
        Group.removeObjectFromGroup group_name object_id

    ///<summary>Removes one or more objects from an existing group</summary>
    ///<param name="object_ids">(Guid seq) a list of object identifiers</param>
    ///<param name="group_name">(string) the name of an existing group</param>
    ///
    ///<returns>(float) The number of objects removed from the group is successful</returns>
    static member RemoveObjectsFromGroup(object_ids:Guid seq, group_name:string) : float =
        Group.removeObjectsFromGroup group_name object_ids

    ///<summary>Renames an existing group</summary>
    ///<param name="old_name">(string) the name of an existing group</param>
    ///<param name="new_name">(string) the new group name</param>
    ///
    ///<returns>(string) the new group name</returns>
    static member RenameGroup(old_name:string, new_name:string) : string =
        Group.renameGroup new_name old_name

    ///<summary>Shows a group of previously hidden objects. Hidden objects are not
    ///    visible, cannot be snapped to, and cannot be selected</summary>
    ///<param name="group_name">(string) the name of an existing group</param>
    ///
    ///<returns>(float) The number of objects that were shown</returns>
    static member ShowGroup(group_name:string) : float =
        Group.showGroup group_name

    ///<summary>Unlocks a group of previously locked objects. Lockes objects are visible,
    ///    can be snapped to, but cannot be selected</summary>
    ///<param name="group_name">(string) the name of an existing group</param>
    ///
    ///<returns>(float) The number of objects that were unlocked</returns>
    static member UnlockGroup(group_name:string) : float =
        Group.unlockGroup group_name

    ///<summary>Creates a new hatch object from a closed planar curve object</summary>
    ///<param name="curve_id">(Guid) identifier of the closed planar curve that defines the
    ///          boundary of the hatch object</param>
    ///<param name="hatch_pattern">(string) Optional, Default Value:None, name of the hatch pattern to be used by the hatch
    ///          object. If omitted, the current hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value:1.0, hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value:0.0, hatch pattern rotation angle in degrees.</param>
    ///
    ///<returns>(Guid) identifier of the newly created hatch on success
    ///      </returns>
    static member AddHatch(curve_id:Guid, [<OPT;DEF(null)>]hatch_pattern:string, [<OPT;DEF(1.0)>]scale:float, [<OPT;DEF(0.0)>]rotation:float) : Guid =
        Hatch.addHatch rotation scale hatch_pattern curve_id

    ///<summary>Creates one or more new hatch objects a list of closed planar curves</summary>
    ///<param name="curve_ids">(Guid seq) identifiers of the closed planar curves that defines the
    ///          boundary of the hatch objects</param>
    ///<param name="hatch_pattern">(string) Optional, Default Value:None, name of the hatch pattern to be used by the hatch
    ///          object. If omitted, the current hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value:1.0, hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value:0.0, hatch pattern rotation angle in degrees.</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, tolerance for hatch fills.</param>
    ///
    ///<returns>(Guid seq) identifiers of the newly created hatch on success</returns>
    static member AddHatches(curve_ids:Guid seq, [<OPT;DEF(null)>]hatch_pattern:string, [<OPT;DEF(1.0)>]scale:float, [<OPT;DEF(0.0)>]rotation:float, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        Hatch.addHatches tolerance rotation scale hatch_pattern curve_ids

    ///<summary>Adds hatch patterns to the document by importing hatch pattern definitions
    ///    from a pattern file.</summary>
    ///<param name="filename">(string) name of the hatch pattern file</param>
    ///<param name="replace">(bool) Optional, Default Value:False, If hatch pattern names already in the document match hatch
    ///          pattern names in the pattern definition file, then the existing hatch
    ///          patterns will be redefined</param>
    ///
    ///<returns>(string seq) Names of the newly added hatch patterns</returns>
    static member AddHatchPatterns(filename:string, [<OPT;DEF(false)>]replace:bool) : string seq =
        Hatch.addHatchPatterns replace filename

    ///<summary>Explodes a hatch object into its component objects. The exploded objects
    ///    will be added to the document. If the hatch object uses a solid pattern,
    ///    then planar face Brep objects will be created. Otherwise, line curve objects
    ///    will be created</summary>
    ///<param name="hatch_id">(Guid) identifier of a hatch object</param>
    ///<param name="delete">(bool) Optional, Default Value:False, delete the hatch object</param>
    ///
    ///<returns>(Guid seq) list of identifiers for the newly created objects</returns>
    static member ExplodeHatch(hatch_id:Guid, [<OPT;DEF(false)>]delete:bool) : Guid seq =
        Hatch.explodeHatch delete hatch_id

    ///<summary>Returns the number of hatch patterns in the document</summary>
    ///
    ///<returns>(int) the number of hatch patterns in the document</returns>
    static member HatchPatternCount() : int =
        Hatch.hatchPatternCount ()

    ///<summary>Returns the description of a hatch pattern. Note, not all hatch patterns
    ///    have descriptions</summary>
    ///<param name="hatch_pattern">(string) name of an existing hatch pattern</param>
    ///
    ///<returns>(string) description of the hatch pattern on success otherwise None</returns>
    static member HatchPatternDescription(hatch_pattern:string) : string =
        Hatch.hatchPatternDescription hatch_pattern

    ///<summary>Returns the fill type of a hatch pattern.</summary>
    ///<param name="hatch_pattern">(string) name of an existing hatch pattern</param>
    ///
    ///<returns>(int) hatch pattern's fill type 
    ///              0 = solid, uses object color
    ///              1 = lines, uses pattern file definition
    ///              2 = gradient, uses fill color definition</returns>
    static member HatchPatternFillType(hatch_pattern:string) : int =
        Hatch.hatchPatternFillType hatch_pattern

    ///<summary>Returns the names of all of the hatch patterns in the document</summary>
    ///
    ///<returns>(string seq) the names of all of the hatch patterns in the document</returns>
    static member HatchPatternNames() : string seq =
        Hatch.hatchPatternNames ()

    ///<summary>Verifies the existence of a hatch object in the document</summary>
    ///<param name="object_id">(Guid) identifier of an object</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsHatch(object_id:Guid) : bool =
        Hatch.isHatch object_id

    ///<summary>Verifies the existence of a hatch pattern in the document</summary>
    ///<param name="name">(string) the name of a hatch pattern</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsHatchPattern(name:string) : bool =
        Hatch.isHatchPattern name

    ///<summary>Verifies that a hatch pattern is the current hatch pattern</summary>
    ///<param name="hatch_pattern">(string) name of an existing hatch pattern</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternCurrent(hatch_pattern:string) : bool =
        Hatch.isHatchPatternCurrent hatch_pattern

    ///<summary>Verifies that a hatch pattern is from a reference file</summary>
    ///<param name="hatch_pattern">(string) name of an existing hatch pattern</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternReference(hatch_pattern:string) : bool =
        Hatch.isHatchPatternReference hatch_pattern

    ///<summary>Add a new layer to the document</summary>
    ///<param name="name">(string) Optional, Default Value:None, The name of the new layer. If omitted, Rhino automatically
    ///          generates the layer name.</param>
    ///<param name="color">(Drawing.Color) Optional, Default Value:None, A Red-Green-Blue color value. If omitted, the color Black is assigned.</param>
    ///<param name="visible">(bool) Optional, Default Value:True, layer's visibility</param>
    ///<param name="locked">(bool) Optional, Default Value:False, layer's locked state</param>
    ///<param name="parent">(string) Optional, Default Value:None, name of the new layer's parent layer. If omitted, the new
    ///          layer will not have a parent layer.</param>
    ///
    ///<returns>(string) The full name of the new layer .</returns>
    static member AddLayer([<OPT;DEF(null)>]name:string, [<OPT;DEF(null)>]color:Drawing.Color, [<OPT;DEF(true)>]visible:bool, [<OPT;DEF(false)>]locked:bool, [<OPT;DEF(null)>]parent:string) : string =
        Layer.addLayer parent locked visible color name

    ///<summary>Removes an existing layer from the document. The layer to be removed
    ///    cannot be the current layer. Unlike the PurgeLayer method, the layer must
    ///    be empty, or contain no objects, before it can be removed. Any layers that
    ///    are children of the specified layer will also be removed if they are also
    ///    empty.</summary>
    ///<param name="layer">(string) the name or id of an existing empty layer</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteLayer(layer:string) : bool =
        Layer.deleteLayer layer

    ///<summary>Expands a layer. Expanded layers can be viewed in Rhino's layer dialog</summary>
    ///<param name="layer">(string) name of the layer to expand</param>
    ///<param name="expand">(bool) True to expand, False to collapse</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ExpandLayer(layer:string, expand:bool) : bool =
        Layer.expandLayer expand layer

    ///<summary>Verifies the existance of a layer in the document</summary>
    ///<param name="layer">(string) the name or id of a layer to search for</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayer(layer:string) : bool =
        Layer.isLayer layer

    ///<summary>Verifies that the objects on a layer can be changed (normal)</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerChangeable(layer:string) : bool =
        Layer.isLayerChangeable layer

    ///<summary>Verifies that a layer is a child of another layer</summary>
    ///<param name="layer">(string) the name or id of the layer to test against</param>
    ///<param name="test">(string) the name or id to the layer to test</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerChildOf(layer:string, test:string) : bool =
        Layer.isLayerChildOf test layer

    ///<summary>Verifies that a layer is the current layer</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerCurrent(layer:string) : bool =
        Layer.isLayerCurrent layer

    ///<summary>Verifies that an existing layer is empty, or contains no objects</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerEmpty(layer:string) : bool =
        Layer.isLayerEmpty layer

    ///<summary>Verifies that a layer is expanded. Expanded layers can be viewed in
    ///    Rhino's layer dialog</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerExpanded(layer:string) : bool =
        Layer.isLayerExpanded layer

    ///<summary>Verifies that a layer is locked.</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerLocked(layer:string) : bool =
        Layer.isLayerLocked layer

    ///<summary>Verifies that a layer is on.</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerOn(layer:string) : bool =
        Layer.isLayerOn layer

    ///<summary>Verifies that an existing layer is selectable (normal and reference)</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerSelectable(layer:string) : bool =
        Layer.isLayerSelectable layer

    ///<summary>Verifies that a layer is a parent of another layer</summary>
    ///<param name="layer">(string) the name or id of the layer to test against</param>
    ///<param name="test">(string) the name or id to the layer to test</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerParentOf(layer:string, test:string) : bool =
        Layer.isLayerParentOf test layer

    ///<summary>Verifies that a layer is from a reference file.</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerReference(layer:string) : bool =
        Layer.isLayerReference layer

    ///<summary>Verifies that a layer is visible (normal, locked, and reference)</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(bool) True on success otherwise False</returns>
    static member IsLayerVisible(layer:string) : bool =
        Layer.isLayerVisible layer

    ///<summary>Returns the number of immediate child layers of a layer</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(int) the number of immediate child layers</returns>
    static member LayerChildCount(layer:string) : int =
        Layer.layerChildCount layer

    ///<summary>Returns the immediate child layers of a layer</summary>
    ///<param name="layer">(string) the name or id of an existing layer</param>
    ///
    ///<returns>(string seq) List of children layer names</returns>
    static member LayerChildren(layer:string) : string seq =
        Layer.layerChildren layer

    ///<summary>Returns the number of layers in the document</summary>
    ///
    ///<returns>(int) the number of layers in the document</returns>
    static member LayerCount() : int =
        Layer.layerCount ()

    ///<summary>Return identifiers of all layers in the document</summary>
    ///
    ///<returns>(Guid seq) the identifiers of all layers in the document</returns>
    static member LayerIds() : Guid seq =
        Layer.layerIds ()

    ///<summary>Returns the identifier of a layer given the layer's name.</summary>
    ///<param name="layer">(string) name of existing layer</param>
    ///
    ///<returns>(Guid) The layer's identifier .</returns>
    static member LayerId(layer:string) : Guid =
        Layer.layerId layer

    ///<summary>Return the name of a layer given it's identifier</summary>
    ///<param name="layer_id">(Guid) layer identifier</param>
    ///<param name="fullpath">(bool) Optional, Default Value:True, return the full path name `True` or short name `False`</param>
    ///
    ///<returns>(string) the layer's name</returns>
    static member LayerName(layer_id:Guid, [<OPT;DEF(true)>]fullpath:bool) : string =
        Layer.layerName fullpath layer_id

    ///<summary>Returns the names of all layers in the document.</summary>
    ///
    ///<returns>(string seq) list of layer names</returns>
    static member LayerNames() : string seq =
        Layer.layerNames ()

    ///<summary>Returns the current display order index of a layer as displayed in Rhino's
    ///    layer dialog box. A display order index of -1 indicates that the current
    ///    layer dialog filter does not allow the layer to appear in the layer list</summary>
    ///<param name="layer">(string) name of existing layer</param>
    ///
    ///<returns>(float) 0 based index of layer</returns>
    static member LayerOrder(layer:string) : float =
        Layer.layerOrder layer

    ///<summary>Removes an existing layer from the document. The layer will be removed
    ///    even if it contains geometry objects. The layer to be removed cannot be the
    ///    current layer
    ///    empty.</summary>
    ///<param name="layer">(string) the name or id of an existing empty layer</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member PurgeLayer(layer:string) : bool =
        Layer.purgeLayer layer

    ///<summary>Renames an existing layer</summary>
    ///<param name="oldname">(string) original layer name</param>
    ///<param name="newname">(string) new layer name</param>
    ///
    ///<returns>(string) The new layer name  otherwise None</returns>
    static member RenameLayer(oldname:string, newname:string) : string =
        Layer.renameLayer newname oldname

    ///<summary>Adds a new directional light object to the document</summary>
    ///<param name="start_point">(Point3d) starting point of the light</param>
    ///<param name="end_point">(Point3d) ending point and direction of the light</param>
    ///
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddDirectionalLight(start_point:Point3d, end_point:Point3d) : Guid =
        Light.addDirectionalLight end_point start_point

    ///<summary>Adds a new linear light object to the document</summary>
    ///<param name="start_point">(Point3d) starting point of the light</param>
    ///<param name="end_point">(Point3d) ending point and direction of the light</param>
    ///<param name="width">(float) Optional, Default Value:None, width of the light</param>
    ///
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddLinearLight(start_point:Point3d, end_point:Point3d, [<OPT;DEF(null)>]width:float) : Guid =
        Light.addLinearLight width end_point start_point

    ///<summary>Adds a new point light object to the document</summary>
    ///<param name="point">(Point3d) the 3d location of the point</param>
    ///
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddPointLight(point:Point3d) : Guid =
        Light.addPointLight point

    ///<summary>Adds a new rectangular light object to the document</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="width_point">(Point3d) 3d width and direction point of the light</param>
    ///<param name="height_point">(Point3d) 3d height and direction point of the light</param>
    ///
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddRectangularLight(origin:Point3d, width_point:Point3d, height_point:Point3d) : Guid =
        Light.addRectangularLight height_point width_point origin

    ///<summary>Adds a new spot light object to the document</summary>
    ///<param name="origin">(Point3d) 3d origin point of the light</param>
    ///<param name="radius">(float) radius of the cone</param>
    ///<param name="apex_point">(Point3d) 3d apex point of the light</param>
    ///
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddSpotLight(origin:Point3d, radius:float, apex_point:Point3d) : Guid =
        Light.addSpotLight apex_point radius origin

    ///<summary>Verifies a light object is a directional light</summary>
    ///<param name="object_id">(Guid) the light object's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsDirectionalLight(object_id:Guid) : bool =
        Light.isDirectionalLight object_id

    ///<summary>Verifies an object is a light object</summary>
    ///<param name="object_id">(Guid) the light object's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsLight(object_id:Guid) : bool =
        Light.isLight object_id

    ///<summary>Verifies a light object is enabled</summary>
    ///<param name="object_id">(Guid) the light object's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsLightEnabled(object_id:Guid) : bool =
        Light.isLightEnabled object_id

    ///<summary>Verifies a light object is referenced from another file</summary>
    ///<param name="object_id">(Guid) the light object's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsLightReference(object_id:Guid) : bool =
        Light.isLightReference object_id

    ///<summary>Verifies a light object is a linear light</summary>
    ///<param name="object_id">(Guid) the light object's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsLinearLight(object_id:Guid) : bool =
        Light.isLinearLight object_id

    ///<summary>Verifies a light object is a point light</summary>
    ///<param name="object_id">(Guid) the light object's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsPointLight(object_id:Guid) : bool =
        Light.isPointLight object_id

    ///<summary>Verifies a light object is a rectangular light</summary>
    ///<param name="object_id">(Guid) the light object's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsRectangularLight(object_id:Guid) : bool =
        Light.isRectangularLight object_id

    ///<summary>Verifies a light object is a spot light</summary>
    ///<param name="object_id">(Guid) the light object's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsSpotLight(object_id:Guid) : bool =
        Light.isSpotLight object_id

    ///<summary>Returns the number of light objects in the document</summary>
    ///
    ///<returns>(int) the number of light objects in the document</returns>
    static member LightCount() : int =
        Light.lightCount ()

    ///<summary>Returns list of identifiers of light objects in the document</summary>
    ///
    ///<returns>(Guid seq) the list of identifiers of light objects in the document</returns>
    static member LightObjects() : Guid seq =
        Light.lightObjects ()

    ///<summary>Returns the plane of a rectangular light object</summary>
    ///<param name="object_id">(Guid) the light object's identifier</param>
    ///
    ///<returns>(Plane) the plane</returns>
    static member RectangularLightPlane(object_id:Guid) : Plane =
        Light.rectangularLightPlane object_id

    ///<summary>Finds the point on an infinite line that is closest to a test point</summary>
    ///<param name="line">(Point3d * Point3d) List of 6 numbers or 2 Point3d.  Two 3-D points identifying the starting and ending points of the line.</param>
    ///<param name="testpoint">(Point3d) List of 3 numbers or Point3d.  The test point.</param>
    ///
    ///<returns>(Point3d) the point on the line that is closest to the test point , otherwise None</returns>
    static member LineClosestPoint(line:Point3d * Point3d, testpoint:Point3d) : Point3d =
        Line.lineClosestPoint testpoint line

    ///<summary>Calculates the intersection of a line and a cylinder</summary>
    ///<param name="line">(Line) the line to intersect</param>
    ///<param name="cylinder_plane">(Plane) base plane of the cylinder</param>
    ///<param name="cylinder_height">(float) height of the cylinder</param>
    ///<param name="cylinder_radius">(float) radius of the cylinder</param>
    ///
    ///<returns>(Point3d seq) list of intersection points (0, 1, or 2 points)</returns>
    static member LineCylinderIntersection(line:Line, cylinder_plane:Plane, cylinder_height:float, cylinder_radius:float) : Point3d seq =
        Line.lineCylinderIntersection cylinder_radius cylinder_height cylinder_plane line

    ///<summary>Determines if the shortest distance from a line to a point or another
    ///    line is greater than a specified distance</summary>
    ///<param name="line">(Line) List of 6 numbers, 2 Point3d, or Line.</param>
    ///<param name="distance">(float) the distance</param>
    ///<param name="point_or_line">(Point3d) the test point or the test line</param>
    ///
    ///<returns>(bool) True if the shortest distance from the line to the other project is
    ///            greater than distance, False otherwise</returns>
    static member LineIsFartherThan(line:Line, distance:float, point_or_line:Point3d) : bool =
        Line.lineIsFartherThan point_or_line distance line

    ///<summary>Calculates the intersection of two non-parallel lines. Note, the two
    ///    lines do not have to intersect for an intersection to be found. (see help)</summary>
    ///<param name="lineA">(Line) lineA of 'lines to intersect' (FIXME 0)</param>
    ///<param name="lineB">(Line) lineB of 'lines to intersect' (FIXME 0)</param>
    ///
    ///<returns>(Point3d * Point3d) containing a point on the first line and a point on the second line</returns>
    static member LineLineIntersection(lineA:Line, lineB:Line) : Point3d * Point3d =
        Line.lineLineIntersection lineB lineA

    ///<summary>Finds the longest distance between a line as a finite chord, and a point
    ///    or another line</summary>
    ///<param name="line">(Line) List of 6 numbers, two Point3d, or Line.</param>
    ///<param name="point_or_line">(Point3d) the test point or test line.</param>
    ///
    ///<returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object, then D >= Rhino.Distance(Q, P).</returns>
    static member LineMaxDistanceTo(line:Line, point_or_line:Point3d) : float =
        Line.lineMaxDistanceTo point_or_line line

    ///<summary>Finds the shortest distance between a line as a finite chord, and a point
    ///    or another line</summary>
    ///<param name="line">(Line) List of 6 numbers, two Point3d, or Line.</param>
    ///<param name="point_or_line">(Point3d) the test point or test line.</param>
    ///
    ///<returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object, then D <= Rhino.Distance(Q, P).</returns>
    static member LineMinDistanceTo(line:Line, point_or_line:Point3d) : float =
        Line.lineMinDistanceTo point_or_line line

    ///<summary>Returns a plane that contains the line. The origin of the plane is at the start of
    ///    the line. If possible, a plane parallel to the world XY, YZ, or ZX plane is returned</summary>
    ///<param name="line">(Line) List of 6 numbers, two Point3d, or Line.</param>
    ///
    ///<returns>(Plane) the plane</returns>
    static member LinePlane(line:Line) : Plane =
        Line.linePlane line

    ///<summary>Calculates the intersection of a line and a plane.</summary>
    ///<param name="line">(Point3d * Point3d) Two 3D points identifying the starting and ending points of the line to intersect.</param>
    ///<param name="plane">(Plane) The plane to intersect.</param>
    ///
    ///<returns>(Point3d) The 3D point of intersection is successful.</returns>
    static member LinePlaneIntersection(line:Point3d * Point3d, plane:Plane) : Point3d =
        Line.linePlaneIntersection plane line

    ///<summary>Calculates the intersection of a line and a sphere</summary>
    ///<param name="line">(Line) the line</param>
    ///<param name="sphere_center">(Point3d) the center point of the sphere</param>
    ///<param name="sphere_radius">(float) the radius of the sphere</param>
    ///
    ///<returns>(Point3d seq) list of intersection points , otherwise None</returns>
    static member LineSphereIntersection(line:Line, sphere_center:Point3d, sphere_radius:float) : Point3d seq =
        Line.lineSphereIntersection sphere_radius sphere_center line

    ///<summary>Transforms a line</summary>
    ///<param name="line">(Guid) the line to transform</param>
    ///<param name="xform">(Transform) the transformation to apply</param>
    ///
    ///<returns>(Guid) transformed line</returns>
    static member LineTransform(line:Guid, xform:Transform) : Guid =
        Line.lineTransform xform line

    ///<summary>Verifies the existance of a linetype in the document</summary>
    ///<param name="name_or_id">(Guid) The name or identifier of an existing linetype.</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsLinetype(name_or_id:Guid) : bool =
        Linetype.isLinetype name_or_id

    ///<summary>Verifies that an existing linetype is from a reference file</summary>
    ///<param name="name_or_id">(Guid) The name or identifier of an existing linetype.</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsLinetypeReference(name_or_id:Guid) : bool =
        Linetype.isLinetypeReference name_or_id

    ///<summary>Returns number of linetypes in the document</summary>
    ///
    ///<returns>(int) the number of linetypes in the document</returns>
    static member LinetypeCount() : int =
        Linetype.linetypeCount ()

    ///<summary>Returns names of all linetypes in the document</summary>
    ///
    ///<returns>(string seq) list of linetype names</returns>
    static member LinetypeNames() : string seq =
        Linetype.linetypeNames ()

    ///<summary>Add material to a layer and returns the new material's index. If the
    ///    layer already has a material, then the layer's current material index is
    ///    returned</summary>
    ///<param name="layer">(string) name of an existing layer.</param>
    ///
    ///<returns>(float) Material index of the layer</returns>
    static member AddMaterialToLayer(layer:string) : float =
        Material.addMaterialToLayer layer

    ///<summary>Adds material to an object and returns the new material's index. If the
    ///    object already has a material, the the object's current material index is
    ///    returned.</summary>
    ///<param name="object_id">(Guid) identifier of an object</param>
    ///
    ///<returns>(float) material index of the object</returns>
    static member AddMaterialToObject(object_id:Guid) : float =
        Material.addMaterialToObject object_id

    ///<summary>Copies definition of a source material to a destination material</summary>
    ///<param name="source_index">(int) source index of 'indices of materials to copy' (FIXME 0)</param>
    ///<param name="destination_index">(int) destination index of 'indices of materials to copy' (FIXME 0)</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CopyMaterial(source_index:int, destination_index:int) : bool =
        Material.copyMaterial destination_index source_index

    ///<summary>Verifies a material is a copy of Rhino's built-in "default" material.
    ///    The default material is used by objects and layers that have not been
    ///    assigned a material.</summary>
    ///<param name="material_index">(int) the zero-based material index</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialDefault(material_index:int) : bool =
        Material.isMaterialDefault material_index

    ///<summary>Verifies a material is referenced from another file</summary>
    ///<param name="material_index">(int) the zero-based material index</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsMaterialReference(material_index:int) : bool =
        Material.isMaterialReference material_index

    ///<summary>Copies the material definition from one material to one or more objects</summary>
    ///<param name="source">(Guid) source material index -or- identifier of the source object.
    ///        The object must have a material assigned</param>
    ///<param name="destination">(Guid seq) identifiers(s) of the destination object(s)</param>
    ///
    ///<returns>(float) number of objects that were modified</returns>
    static member MatchMaterial(source:Guid, destination:Guid seq) : float =
        Material.matchMaterial destination source

    ///<summary>Resets a material to Rhino's default material</summary>
    ///<param name="material_index">(int) zero based material index</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ResetMaterial(material_index:int) : bool =
        Material.resetMaterial material_index

    ///<summary>Add a mesh object to the document</summary>
    ///<param name="vertices">(Point3d seq) list of 3D points defining the vertices of the mesh</param>
    ///<param name="face_vertices">(float seq) list containing lists of 3 or 4 numbers that define the
    ///                    vertex indices for each face of the mesh. If the third a fourth vertex
    ///                     indices of a face are identical, a triangular face will be created.</param>
    ///<param name="vertex_normals">(Vector3d seq) Optional, Default Value:None, list of 3D vectors defining the vertex normals of
    ///        the mesh. Note, for every vertex, there must be a corresponding vertex
    ///        normal</param>
    ///<param name="texture_coordinates">(float seq) Optional, Default Value:None, list of 2D texture coordinates. For every
    ///        vertex, there must be a corresponding texture coordinate</param>
    ///<param name="vertex_colors">(Drawing.Color seq) Optional, Default Value:None, a list of color values. For every vertex,
    ///        there must be a corresponding vertex color</param>
    ///
    ///<returns>(Guid) Identifier of the new object</returns>
    static member AddMesh(vertices:Point3d seq, face_vertices:float seq, [<OPT;DEF(null)>]vertex_normals:Vector3d seq, [<OPT;DEF(null)>]texture_coordinates:float seq, [<OPT;DEF(null)>]vertex_colors:Drawing.Color seq) : Guid =
        Mesh.addMesh vertex_colors texture_coordinates vertex_normals face_vertices vertices

    ///<summary>Creates a planar mesh from a closed, planar curve</summary>
    ///<param name="object_id">(Guid) identifier of a closed, planar curve</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, if True, delete the input curve defined by object_id</param>
    ///
    ///<returns>(Guid) id of the new mesh on success</returns>
    static member AddPlanarMesh(object_id:Guid, [<OPT;DEF(false)>]delete_input:bool) : Guid =
        Mesh.addPlanarMesh delete_input object_id

    ///<summary>Calculates the intersection of a curve object and a mesh object</summary>
    ///<param name="curve_id">(Guid) identifier of a curve object</param>
    ///<param name="mesh_id">(Guid) identifier or a mesh object</param>
    ///<param name="return_faces">(bool) Optional, Default Value:False, return both intersection points and face indices.
    ///        If False, then just the intersection points are returned</param>
    ///
    ///<returns>(Point3d seq) if return_false is omitted or False, then a list of intersection points
    ///        list([point, number], ...): if return_false is True, the a one-dimensional list containing information
    ///          about each intersection. Each element contains the following two elements
    ///            [0] = point of intersection
    ///            [1] = mesh face index where intersection lies</returns>
    static member CurveMeshIntersection(curve_id:Guid, mesh_id:Guid, [<OPT;DEF(false)>]return_faces:bool) : Point3d seq =
        Mesh.curveMeshIntersection return_faces mesh_id curve_id

    ///<summary>Returns number of meshes that could be created by calling SplitDisjointMesh</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(int) The number of meshes that could be created</returns>
    static member DisjointMeshCount(object_id:Guid) : int =
        Mesh.disjointMeshCount object_id

    ///<summary>Creates curves that duplicates a mesh border</summary>
    ///<param name="mesh_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(Guid seq) list of curve ids on success</returns>
    static member DuplicateMeshBorder(mesh_id:Guid) : Guid seq =
        Mesh.duplicateMeshBorder mesh_id

    ///<summary>Explodes a mesh object, or mesh objects int submeshes. A submesh is a
    ///    collection of mesh faces that are contained within a closed loop of
    ///    unwelded mesh edges. Unwelded mesh edges are where the mesh faces that
    ///    share the edge have unique mesh vertices (not mesh topology vertices)
    ///    at both ends of the edge</summary>
    ///<param name="mesh_ids">(Guid seq) list of mesh identifiers</param>
    ///<param name="delete">(bool) Optional, Default Value:False, delete the input meshes</param>
    ///
    ///<returns>(Guid seq) List of resulting objects after explode.</returns>
    static member ExplodeMeshes(mesh_ids:Guid seq, [<OPT;DEF(false)>]delete:bool) : Guid seq =
        Mesh.explodeMeshes delete mesh_ids

    ///<summary>Verifies if an object is a mesh</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True , otherwise False</returns>
    static member IsMesh(object_id:Guid) : bool =
        Mesh.isMesh object_id

    ///<summary>Verifies a mesh object is closed</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsMeshClosed(object_id:Guid) : bool =
        Mesh.isMeshClosed object_id

    ///<summary>Verifies a mesh object is manifold. A mesh for which every edge is shared
    ///    by at most two faces is called manifold. If a mesh has at least one edge
    ///    that is shared by more than two faces, then that mesh is called non-manifold</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsMeshManifold(object_id:Guid) : bool =
        Mesh.isMeshManifold object_id

    ///<summary>Verifies a point is on a mesh</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///<param name="point">(Point3d) test point</param>
    ///
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsPointOnMesh(object_id:Guid, point:Point3d) : bool =
        Mesh.isPointOnMesh point object_id

    ///<summary>Joins two or or more mesh objects together</summary>
    ///<param name="object_ids">(Guid seq) identifiers of two or more mesh objects</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, delete input after joining</param>
    ///
    ///<returns>(Guid) identifier of newly created mesh on success</returns>
    static member JoinMeshes(object_ids:Guid seq, [<OPT;DEF(false)>]delete_input:bool) : Guid =
        Mesh.joinMeshes delete_input object_ids

    ///<summary>Returns approximate area of one or more mesh objects</summary>
    ///<param name="object_ids">(Guid seq) identifiers of one or more mesh objects</param>
    ///
    ///<returns>(float * float * float) where
    ///       [0] = number of meshes used in calculation
    ///       [1] = total area of all meshes
    ///       [2] = the error estimate</returns>
    static member MeshArea(object_ids:Guid seq) : float * float * float =
        Mesh.meshArea object_ids

    ///<summary>Calculates the area centroid of a mesh object</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(Point3d) representing the area centroid</returns>
    static member MeshAreaCentroid(object_id:Guid) : Point3d =
        Mesh.meshAreaCentroid object_id

    ///<summary>Performs boolean difference operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid) input0 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="input1">(Guid) input1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="delete_input">(bool) Optional, Default Value:True, delete the input meshes</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, a positive tolerance value, or None to use the default of the document.</param>
    ///
    ///<returns>(Guid seq) identifiers of newly created meshes</returns>
    static member MeshBooleanDifference(input0:Guid, input1:Guid, [<OPT;DEF(true)>]delete_input:bool, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        Mesh.meshBooleanDifference tolerance delete_input input1 input0

    ///<summary>Performs boolean intersection operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid) input0 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="input1">(Guid) input1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="delete_input">(bool) Optional, Default Value:True, delete the input meshes</param>
    ///
    ///<returns>(Guid seq) identifiers of new meshes on success</returns>
    static member MeshBooleanIntersection(input0:Guid, input1:Guid, [<OPT;DEF(true)>]delete_input:bool) : Guid seq =
        Mesh.meshBooleanIntersection delete_input input1 input0

    ///<summary>Performs boolean split operation on two sets of input meshes</summary>
    ///<param name="input0">(Guid) input0 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="input1">(Guid) input1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="delete_input">(bool) Optional, Default Value:True, delete the input meshes</param>
    ///
    ///<returns>(Guid seq) identifiers of new meshes on success</returns>
    static member MeshBooleanSplit(input0:Guid, input1:Guid, [<OPT;DEF(true)>]delete_input:bool) : Guid seq =
        Mesh.meshBooleanSplit delete_input input1 input0

    ///<summary>Performs boolean union operation on a set of input meshes</summary>
    ///<param name="mesh_ids">(Guid seq) identifiers of meshes</param>
    ///<param name="delete_input">(bool) Optional, Default Value:True, delete the input meshes</param>
    ///
    ///<returns>(Guid seq) identifiers of new meshes</returns>
    static member MeshBooleanUnion(mesh_ids:Guid seq, [<OPT;DEF(true)>]delete_input:bool) : Guid seq =
        Mesh.meshBooleanUnion delete_input mesh_ids

    ///<summary>Returns the point on a mesh that is closest to a test point</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///<param name="point">(Point3d) point to test</param>
    ///<param name="maximum_distance">(float) Optional, Default Value:None, upper bound used for closest point calculation.
    ///        If you are only interested in finding a point Q on the mesh when
    ///        point.DistanceTo(Q) < maximum_distance, then set maximum_distance to
    ///        that value</param>
    ///
    ///<returns>(Point3d * float) containing the results of the calculation where
    ///                            [0] = the 3-D point on the mesh
    ///                            [1] = the index of the mesh face on which the 3-D point lies</returns>
    static member MeshClosestPoint(object_id:Guid, point:Point3d, [<OPT;DEF(null)>]maximum_distance:float) : Point3d * float =
        Mesh.meshClosestPoint maximum_distance point object_id

    ///<summary>Returns the center of each face of the mesh object</summary>
    ///<param name="mesh_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(Point3d seq) points defining the center of each face</returns>
    static member MeshFaceCenters(mesh_id:Guid) : Point3d seq =
        Mesh.meshFaceCenters mesh_id

    ///<summary>Returns total face count of a mesh object</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(int) the number of mesh faces</returns>
    static member MeshFaceCount(object_id:Guid) : int =
        Mesh.meshFaceCount object_id

    ///<summary>Returns the face unit normal for each face of a mesh object</summary>
    ///<param name="mesh_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(Vector3d seq) 3D vectors that define the face unit normals of the mesh</returns>
    static member MeshFaceNormals(mesh_id:Guid) : Vector3d seq =
        Mesh.meshFaceNormals mesh_id

    ///<summary>Returns face vertices of a mesh</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///<param name="face_type">(bool) Optional, Default Value:True, The face type to be returned. True = both triangles
    ///        and quads. False = only triangles</param>
    ///
    ///<returns>(Point3d seq) 3D points that define the face vertices of the mesh. If
    ///      face_type is True, then faces are returned as both quads and triangles
    ///      (4 3D points). For triangles, the third and fourth vertex will be
    ///      identical. If face_type is False, then faces are returned as only
    ///      triangles(3 3D points). Quads will be converted to triangles.</returns>
    static member MeshFaces(object_id:Guid, [<OPT;DEF(true)>]face_type:bool) : Point3d seq =
        Mesh.meshFaces face_type object_id

    ///<summary>Returns the vertex indices of all faces of a mesh object</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(float seq) containing tuples of 4 numbers that define the vertex indices for
    ///      each face of the mesh. Both quad and triangle faces are returned. If the
    ///      third and fourth vertex indices are identical, the face is a triangle.</returns>
    static member MeshFaceVertices(object_id:Guid) : float seq =
        Mesh.meshFaceVertices object_id

    ///<summary>Verifies a mesh object has face normals</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasFaceNormals(object_id:Guid) : bool =
        Mesh.meshHasFaceNormals object_id

    ///<summary>Verifies a mesh object has texture coordinates</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasTextureCoordinates(object_id:Guid) : bool =
        Mesh.meshHasTextureCoordinates object_id

    ///<summary>Verifies a mesh object has vertex colors</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasVertexColors(object_id:Guid) : bool =
        Mesh.meshHasVertexColors object_id

    ///<summary>Verifies a mesh object has vertex normals</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(bool) True , otherwise False.</returns>
    static member MeshHasVertexNormals(object_id:Guid) : bool =
        Mesh.meshHasVertexNormals object_id

    ///<summary>Calculates the intersections of a mesh object with another mesh object</summary>
    ///<param name="mesh1">(Guid) mesh1 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="mesh2">(Guid) mesh2 of 'identifiers of meshes' (FIXME 0)</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, the intersection tolerance</param>
    ///
    ///<returns>(Point3d seq) of points that define the vertices of the intersection curves</returns>
    static member MeshMeshIntersection(mesh1:Guid, mesh2:Guid, [<OPT;DEF(null)>]tolerance:float) : Point3d seq =
        Mesh.meshMeshIntersection tolerance mesh2 mesh1

    ///<summary>Identifies the naked edge points of a mesh object. This function shows
    ///    where mesh vertices are not completely surrounded by faces. Joined
    ///    meshes, such as are made by MeshBox, have naked mesh edge points where
    ///    the sub-meshes are joined</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(bool seq) of boolean values that represent whether or not a mesh vertex is
    ///      naked or not. The number of elements in the list will be equal to
    ///      the value returned by MeshVertexCount. In which case, the list will
    ///      identify the naked status for each vertex returned by MeshVertices</returns>
    static member MeshNakedEdgePoints(object_id:Guid) : bool seq =
        Mesh.meshNakedEdgePoints object_id

    ///<summary>Makes a new mesh with vertices offset at a distance in the opposite
    ///    direction of the existing vertex normals</summary>
    ///<param name="mesh_id">(Guid) identifier of a mesh object</param>
    ///<param name="distance">(float) the distance to offset</param>
    ///
    ///<returns>(Guid) identifier of the new mesh object</returns>
    static member MeshOffset(mesh_id:Guid, distance:float) : Guid =
        Mesh.meshOffset distance mesh_id

    ///<summary>Creates polyline curve outlines of mesh objects</summary>
    ///<param name="object_ids">(Guid seq) identifiers of meshes to outline</param>
    ///<param name="view">(string) Optional, Default Value:None, view to use for outline direction</param>
    ///
    ///<returns>(Guid seq) polyline curve identifiers on success</returns>
    static member MeshOutline(object_ids:Guid seq, [<OPT;DEF(null)>]view:string) : Guid seq =
        Mesh.meshOutline view object_ids

    ///<summary>Returns the number of quad faces of a mesh object</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(int) the number of quad mesh faces</returns>
    static member MeshQuadCount(object_id:Guid) : int =
        Mesh.meshQuadCount object_id

    ///<summary>Converts a mesh object's quad faces to triangles</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member MeshQuadsToTriangles(object_id:Guid) : bool =
        Mesh.meshQuadsToTriangles object_id

    ///<summary>Duplicates each polygon in a mesh with a NURBS surface. The resulting
    ///    surfaces are then joined into a polysurface and added to the document</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///<param name="trimmed_triangles">(bool) Optional, Default Value:True, if True, triangles in the mesh will be
    ///        represented by a trimmed plane</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, delete input object</param>
    ///
    ///<returns>(Guid seq) identifiers for the new breps on success</returns>
    static member MeshToNurb(object_id:Guid, [<OPT;DEF(true)>]trimmed_triangles:bool, [<OPT;DEF(false)>]delete_input:bool) : Guid seq =
        Mesh.meshToNurb delete_input trimmed_triangles object_id

    ///<summary>Returns number of triangular faces of a mesh</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(int) The number of triangular mesh faces</returns>
    static member MeshTriangleCount(object_id:Guid) : int =
        Mesh.meshTriangleCount object_id

    ///<summary>Returns the vertex count of a mesh</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(int) The number of mesh vertices .</returns>
    static member MeshVertexCount(object_id:Guid) : int =
        Mesh.meshVertexCount object_id

    ///<summary>Returns the mesh faces that share a specified mesh vertex</summary>
    ///<param name="mesh_id">(Guid) identifier of a mesh object</param>
    ///<param name="vertex_index">(int) index of the mesh vertex to find faces for</param>
    ///
    ///<returns>(float seq) face indices on success</returns>
    static member MeshVertexFaces(mesh_id:Guid, vertex_index:int) : float seq =
        Mesh.meshVertexFaces vertex_index mesh_id

    ///<summary>Returns the vertex unit normal for each vertex of a mesh</summary>
    ///<param name="mesh_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(Vector3d seq) of vertex normals, (empty list if no normals exist)</returns>
    static member MeshVertexNormals(mesh_id:Guid) : Vector3d seq =
        Mesh.meshVertexNormals mesh_id

    ///<summary>Returns the vertices of a mesh</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(Point3d seq) vertex points in the mesh</returns>
    static member MeshVertices(object_id:Guid) : Point3d seq =
        Mesh.meshVertices object_id

    ///<summary>Returns the approximate volume of one or more closed meshes</summary>
    ///<param name="object_ids">(Guid seq) identifiers of one or more mesh objects</param>
    ///
    ///<returns>(float * float * float) containing 3 velues  where
    ///           [0] = number of meshes used in volume calculation
    ///           [1] = total volume of all meshes
    ///           [2] = the error estimate</returns>
    static member MeshVolume(object_ids:Guid seq) : float * float * float =
        Mesh.meshVolume object_ids

    ///<summary>Calculates the volume centroid of a mesh</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(Point3d) Point3d representing the volume centroid</returns>
    static member MeshVolumeCentroid(object_id:Guid) : Point3d =
        Mesh.meshVolumeCentroid object_id

    ///<summary>Pulls a curve to a mesh. The function makes a polyline approximation of
    ///    the input curve and gets the closest point on the mesh for each point on
    ///    the polyline. Then it "connects the points" to create a polyline on the mesh</summary>
    ///<param name="mesh_id">(Guid) identifier of mesh that pulls</param>
    ///<param name="curve_id">(Guid) identifier of curve to pull</param>
    ///
    ///<returns>(Guid) identifier new curve on success</returns>
    static member PullCurveToMesh(mesh_id:Guid, curve_id:Guid) : Guid =
        Mesh.pullCurveToMesh curve_id mesh_id

    ///<summary>Splits up a mesh into its unconnected pieces</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, delete the input object</param>
    ///
    ///<returns>(Guid seq) identifiers for the new meshes</returns>
    static member SplitDisjointMesh(object_id:Guid, [<OPT;DEF(false)>]delete_input:bool) : Guid seq =
        Mesh.splitDisjointMesh delete_input object_id

    ///<summary>Fixes inconsistencies in the directions of faces of a mesh</summary>
    ///<param name="object_id">(Guid) identifier of a mesh object</param>
    ///
    ///<returns>(float) the number of faces that were modified</returns>
    static member UnifyMeshNormals(object_id:Guid) : float =
        Mesh.unifyMeshNormals object_id

    ///<summary>Copies object from one location to another, or in-place.</summary>
    ///<param name="object_id">(Guid) object to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value:None, translation vector to apply</param>
    ///
    ///<returns>(Guid) id for the copy</returns>
    static member CopyObject(object_id:Guid, [<OPT;DEF(null)>]translation:Vector3d) : Guid =
        Object.copyObject translation object_id

    ///<summary>Copies one or more objects from one location to another, or in-place.</summary>
    ///<param name="object_ids">(Guid seq) list of objects to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value:None, list of three numbers or Vector3d representing
    ///                         translation vector to apply to copied set</param>
    ///
    ///<returns>(Guid seq) identifiers for the copies</returns>
    static member CopyObjects(object_ids:Guid seq, [<OPT;DEF(null)>]translation:Vector3d) : Guid seq =
        Object.copyObjects translation object_ids

    ///<summary>Deletes a single object from the document</summary>
    ///<param name="object_id">(Guid) identifier of object to delete</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member DeleteObject(object_id:Guid) : bool =
        Object.deleteObject object_id

    ///<summary>Deletes one or more objects from the document</summary>
    ///<param name="object_ids">(Guid seq) identifiers of objects to delete</param>
    ///
    ///<returns>(float) Number of objects deleted</returns>
    static member DeleteObjects(object_ids:Guid seq) : float =
        Object.deleteObjects object_ids

    ///<summary>Causes the selection state of one or more objects to change momentarily
    ///    so the object appears to flash on the screen</summary>
    ///<param name="object_ids">(Guid seq) identifiers of objects to flash</param>
    ///<param name="style">(bool) Optional, Default Value:True, If True, flash between object color and selection color.
    ///        If False, flash between visible and invisible</param>
    ///
    ///<returns>(unit) </returns>
    static member FlashObject(object_ids:Guid seq, [<OPT;DEF(true)>]style:bool) : unit =
        Object.flashObject style object_ids

    ///<summary>Hides a single object</summary>
    ///<param name="object_id">(Guid) id of object to hide</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member HideObject(object_id:Guid) : bool =
        Object.hideObject object_id

    ///<summary>Hides one or more objects</summary>
    ///<param name="object_ids">(Guid seq) identifiers of objects to hide</param>
    ///
    ///<returns>(int) Number of objects hidden</returns>
    static member HideObjects(object_ids:Guid seq) : int =
        Object.hideObjects object_ids

    ///<summary>Verifies that an object is in either page layout space or model space</summary>
    ///<param name="object_id">(Guid) id of an object to test</param>
    ///
    ///<returns>(bool) True if the object is in page layout space, False if the object is in model space</returns>
    static member IsLayoutObject(object_id:Guid) : bool =
        Object.isLayoutObject object_id

    ///<summary>Verifies the existence of an object</summary>
    ///<param name="object_id">(Guid) an object to test</param>
    ///
    ///<returns>(bool) True if the object exists, False if the object does not exist</returns>
    static member IsObject(object_id:Guid) : bool =
        Object.isObject object_id

    ///<summary>Verifies that an object is hidden. Hidden objects are not visible, cannot
    ///    be snapped to, and cannot be selected</summary>
    ///<param name="object_id">(Guid) The identifier of an object to test</param>
    ///
    ///<returns>(bool) True if the object is hidden, False if the object is not hidden</returns>
    static member IsObjectHidden(object_id:Guid) : bool =
        Object.isObjectHidden object_id

    ///<summary>Verifies an object's bounding box is inside of another bounding box</summary>
    ///<param name="object_id">(Guid) identifier of an object to be tested</param>
    ///<param name="box">(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) bounding box to test for containment</param>
    ///<param name="test_mode">(bool) Optional, Default Value:True, If True, the object's bounding box must be contained by box
    ///        If False, the object's bounding box must be contained by or intersect box</param>
    ///
    ///<returns>(bool) True if object is inside box, False is object is not inside box</returns>
    static member IsObjectInBox(object_id:Guid, box:Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d, [<OPT;DEF(true)>]test_mode:bool) : bool =
        Object.isObjectInBox test_mode box object_id

    ///<summary>Verifies that an object is a member of a group</summary>
    ///<param name="object_id">(Guid) The identifier of an object</param>
    ///<param name="group_name">(string) Optional, Default Value:None, The name of a group. If omitted, the function
    ///        verifies that the object is a member of any group</param>
    ///
    ///<returns>(bool) True if the object is a member of the specified group. If a group_name
    ///        was not specified, the object is a member of some group.
    ///        False if the object  is not a member of the specified group.
    ///        If a group_name was not specified, the object is not a member of any group</returns>
    static member IsObjectInGroup(object_id:Guid, [<OPT;DEF(null)>]group_name:string) : bool =
        Object.isObjectInGroup group_name object_id

    ///<summary>Verifies that an object is locked. Locked objects are visible, and can
    ///    be snapped to, but cannot be selected</summary>
    ///<param name="object_id">(Guid) The identifier of an object to be tested</param>
    ///
    ///<returns>(bool) True if the object is locked, False if the object is not locked</returns>
    static member IsObjectLocked(object_id:Guid) : bool =
        Object.isObjectLocked object_id

    ///<summary>Verifies that an object is normal. Normal objects are visible, can be
    ///    snapped to, and can be selected</summary>
    ///<param name="object_id">(Guid) The identifier of an object to be tested</param>
    ///
    ///<returns>(bool) True if the object is normal, False if the object is not normal</returns>
    static member IsObjectNormal(object_id:Guid) : bool =
        Object.isObjectNormal object_id

    ///<summary>Verifies that an object is a reference object. Reference objects are
    ///    objects that are not part of the current document</summary>
    ///<param name="object_id">(Guid) The identifier of an object to test</param>
    ///
    ///<returns>(bool) True if the object is a reference object, False if the object is not a reference object</returns>
    static member IsObjectReference(object_id:Guid) : bool =
        Object.isObjectReference object_id

    ///<summary>Verifies that an object can be selected</summary>
    ///<param name="object_id">(Guid) The identifier of an object to test</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsObjectSelectable(object_id:Guid) : bool =
        Object.isObjectSelectable object_id

    ///<summary>Verifies that an object is currently selected.</summary>
    ///<param name="object_id">(Guid) The identifier of an object to test</param>
    ///
    ///<returns>(int) 0, the object is not selected
    ///        1, the object is selected
    ///        2, the object is entirely persistently selected
    ///        3, one or more proper sub-objects are selected</returns>
    static member IsObjectSelected(object_id:Guid) : int =
        Object.isObjectSelected object_id

    ///<summary>Determines if an object is closed, solid</summary>
    ///<param name="object_id">(Guid) The identifier of an object to test</param>
    ///
    ///<returns>(bool) True if the object is solid, or a mesh is closed., False otherwise.</returns>
    static member IsObjectSolid(object_id:Guid) : bool =
        Object.isObjectSolid object_id

    ///<summary>Verifies an object's geometry is valid and without error</summary>
    ///<param name="object_id">(Guid) The identifier of an object to test</param>
    ///
    ///<returns>(bool) True if the object is valid</returns>
    static member IsObjectValid(object_id:Guid) : bool =
        Object.isObjectValid object_id

    ///<summary>Verifies an object is visible in a view</summary>
    ///<param name="object_id">(Guid) the identifier of an object to test</param>
    ///<param name="view">(string) Optional, Default Value:None, he title of the view.  If omitted, the current active view is used.</param>
    ///
    ///<returns>(bool) True if the object is visible in the specified view, otherwise False.</returns>
    static member IsVisibleInView(object_id:Guid, [<OPT;DEF(null)>]view:string) : bool =
        Object.isVisibleInView view object_id

    ///<summary>Locks a single object. Locked objects are visible, and they can be
    ///    snapped to. But, they cannot be selected.</summary>
    ///<param name="object_id">(Guid) The identifier of an object</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member LockObject(object_id:Guid) : bool =
        Object.lockObject object_id

    ///<summary>Locks one or more objects. Locked objects are visible, and they can be
    ///    snapped to. But, they cannot be selected.</summary>
    ///<param name="object_ids">(Guid seq) list of Strings or Guids. The identifiers of objects</param>
    ///
    ///<returns>(float) number of objects locked</returns>
    static member LockObjects(object_ids:Guid seq) : float =
        Object.lockObjects object_ids

    ///<summary>Matches, or copies the attributes of a source object to a target object</summary>
    ///<param name="target_ids">(Guid seq) identifiers of objects to copy attributes to</param>
    ///<param name="source_id">(Guid) Optional, Default Value:None, identifier of object to copy attributes from. If None,
    ///        then the default attributes are copied to the target_ids</param>
    ///
    ///<returns>(float) number of objects modified</returns>
    static member MatchObjectAttributes(target_ids:Guid seq, [<OPT;DEF(null)>]source_id:Guid) : float =
        Object.matchObjectAttributes source_id target_ids

    ///<summary>Mirrors a single object</summary>
    ///<param name="object_id">(Guid) The identifier of an object to mirror</param>
    ///<param name="start_point">(Point3d) start of the mirror plane</param>
    ///<param name="end_point">(Point3d) end of the mirror plane</param>
    ///<param name="copy">(bool) Optional, Default Value:False, copy the object</param>
    ///
    ///<returns>(Guid) Identifier of the mirrored object</returns>
    static member MirrorObject(object_id:Guid, start_point:Point3d, end_point:Point3d, [<OPT;DEF(false)>]copy:bool) : Guid =
        Object.mirrorObject copy end_point start_point object_id

    ///<summary>Mirrors a list of objects</summary>
    ///<param name="object_ids">(Guid seq) identifiers of objects to mirror</param>
    ///<param name="start_point">(Point3d) start of the mirror plane</param>
    ///<param name="end_point">(Point3d) end of the mirror plane</param>
    ///<param name="copy">(bool) Optional, Default Value:False, copy the objects</param>
    ///
    ///<returns>(Guid seq) List of identifiers of the mirrored objects</returns>
    static member MirrorObjects(object_ids:Guid seq, start_point:Point3d, end_point:Point3d, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        Object.mirrorObjects copy end_point start_point object_ids

    ///<summary>Moves a single object</summary>
    ///<param name="object_id">(Guid) The identifier of an object to move</param>
    ///<param name="translation">(Vector3d) list of 3 numbers or Vector3d</param>
    ///
    ///<returns>(Guid) Identifier of the moved object</returns>
    static member MoveObject(object_id:Guid, translation:Vector3d) : Guid =
        Object.moveObject translation object_id

    ///<summary>Moves one or more objects</summary>
    ///<param name="object_ids">(Guid seq) The identifiers objects to move</param>
    ///<param name="translation">(Vector3d) list of 3 numbers or Vector3d</param>
    ///
    ///<returns>(Guid seq) identifiers of the moved objects</returns>
    static member MoveObjects(object_ids:Guid seq, translation:Vector3d) : Guid seq =
        Object.moveObjects translation object_ids

    ///<summary>Returns a short text description of an object</summary>
    ///<param name="object_id">(Guid) identifier of an object</param>
    ///
    ///<returns>(string) A short text description of the object .</returns>
    static member ObjectDescription(object_id:Guid) : string =
        Object.objectDescription object_id

    ///<summary>Returns all of the group names that an object is assigned to</summary>
    ///<param name="object_id">(Guid) identifier of an object(s)</param>
    ///
    ///<returns>(string seq) list of group names on success</returns>
    static member ObjectGroups(object_id:Guid) : string seq =
        Object.objectGroups object_id

    ///<summary>Returns the object type</summary>
    ///<param name="object_id">(Guid) identifier of an object</param>
    ///
    ///<returns>(int) The object type .
    ///        The valid object types are as follows:
    ///        Value   Description
    ///          0           Unknown object
    ///          1           Point
    ///          2           Point cloud
    ///          4           Curve
    ///          8           Surface or single-face brep
    ///          16          Polysurface or multiple-face
    ///          32          Mesh
    ///          256         Light
    ///          512         Annotation
    ///          4096        Instance or block reference
    ///          8192        Text dot object
    ///          16384       Grip object
    ///          32768       Detail
    ///          65536       Hatch
    ///          131072      Morph control
    ///          134217728   Cage
    ///          268435456   Phantom
    ///          536870912   Clipping plane
    ///          1073741824  Extrusion</returns>
    static member ObjectType(object_id:Guid) : int =
        Object.objectType object_id

    ///<summary>Orients a single object based on input points.  
    ///    If two 3-D points are specified, then this method will function similar to Rhino's Orient command.  If more than two 3-D points are specified, then the function will orient similar to Rhino's Orient3Pt command.
    ///    The orient flags values can be added together to specify multiple options.
    ///        Value   Description
    ///        1       Copy object.  The default is not to copy the object.
    ///        2       Scale object.  The default is not to scale the object.  Note, the scale option only applies if both reference and target contain only two 3-D points.</summary>
    ///<param name="object_id">(Guid) The identifier of an object</param>
    ///<param name="reference">(Point3d seq) list of 3-D reference points.</param>
    ///<param name="target">(Point3d seq) list of 3-D target points</param>
    ///<param name="flags">(int) Optional, Default Value:0, 1 = copy object
    ///                         2 = scale object
    ///                         3 = copy and scale</param>
    ///
    ///<returns>(Guid) The identifier of the oriented object .</returns>
    static member OrientObject(object_id:Guid, reference:Point3d seq, target:Point3d seq, [<OPT;DEF(0)>]flags:int) : Guid =
        Object.orientObject flags target reference object_id

    ///<summary>Rotates a single object</summary>
    ///<param name="object_id">(Guid) The identifier of an object to rotate</param>
    ///<param name="center_point">(Point3d) the center of rotation</param>
    ///<param name="rotation_angle">(float) in degrees</param>
    ///<param name="axis">(Plane) Optional, Default Value:None, axis of rotation, If omitted, the Z axis of the active
    ///        construction plane is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value:False, copy the object</param>
    ///
    ///<returns>(Guid) Identifier of the rotated object</returns>
    static member RotateObject(object_id:Guid, center_point:Point3d, rotation_angle:float, [<OPT;DEF(null)>]axis:Plane, [<OPT;DEF(false)>]copy:bool) : Guid =
        Object.rotateObject copy axis rotation_angle center_point object_id

    ///<summary>Rotates multiple objects</summary>
    ///<param name="object_ids">(Guid seq) Identifiers of objects to rotate</param>
    ///<param name="center_point">(Point3d) the center of rotation</param>
    ///<param name="rotation_angle">(float) in degrees</param>
    ///<param name="axis">(Plane) Optional, Default Value:None, axis of rotation, If omitted, the Z axis of the active
    ///        construction plane is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value:False, copy the object</param>
    ///
    ///<returns>(Guid seq) identifiers of the rotated objects</returns>
    static member RotateObjects(object_ids:Guid seq, center_point:Point3d, rotation_angle:float, [<OPT;DEF(null)>]axis:Plane, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        Object.rotateObjects copy axis rotation_angle center_point object_ids

    ///<summary>Scales a single object. Can be used to perform a uniform or non-uniform
    ///    scale transformation. Scaling is based on the active construction plane.</summary>
    ///<param name="object_id">(Guid) The identifier of an object</param>
    ///<param name="origin">(Point3d) the origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value:False, copy the object</param>
    ///
    ///<returns>(Guid) Identifier of the scaled object</returns>
    static member ScaleObject(object_id:Guid, origin:Point3d, scale:float*float*float, [<OPT;DEF(false)>]copy:bool) : Guid =
        Object.scaleObject copy scale origin object_id

    ///<summary>Scales one or more objects. Can be used to perform a uniform or non-
    ///    uniform scale transformation. Scaling is based on the active construction plane.</summary>
    ///<param name="object_ids">(Guid seq) Identifiers of objects to scale</param>
    ///<param name="origin">(Point3d) the origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value:False, copy the objects</param>
    ///
    ///<returns>(Guid seq) identifiers of the scaled objects</returns>
    static member ScaleObjects(object_ids:Guid seq, origin:Point3d, scale:float*float*float, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        Object.scaleObjects copy scale origin object_ids

    ///<summary>Selects a single object</summary>
    ///<param name="object_id">(Guid) the identifier of the object to select</param>
    ///<param name="redraw">(bool) Optional, Default Value:True, redraw view too</param>
    ///
    ///<returns>(bool) True on success</returns>
    static member SelectObject(object_id:Guid, [<OPT;DEF(true)>]redraw:bool) : bool =
        Object.selectObject redraw object_id

    ///<summary>Selects one or more objects</summary>
    ///<param name="object_ids">(Guid seq) identifiers of the objects to select</param>
    ///
    ///<returns>(float) number of selected objects</returns>
    static member SelectObjects(object_ids:Guid seq) : float =
        Object.selectObjects object_ids

    ///<summary>Perform a shear transformation on a single object</summary>
    ///<param name="object_id">(Guid seq) The identifier of an object</param>
    ///<param name="origin">(Point3d) origin point of the shear transformation</param>
    ///<param name="reference_point">(Point3d) reference point of the shear transformation</param>
    ///<param name="angle_degrees">(int) the shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value:False, copy the objects</param>
    ///
    ///<returns>(Guid) Identifier of the sheared object</returns>
    static member ShearObject(object_id:Guid seq, origin:Point3d, reference_point:Point3d, angle_degrees:int, [<OPT;DEF(false)>]copy:bool) : Guid =
        Object.shearObject copy angle_degrees reference_point origin object_id

    ///<summary>Shears one or more objects</summary>
    ///<param name="object_ids">(Guid seq) The identifiers objects to shear</param>
    ///<param name="origin">(Point3d) origin point of the shear transformation</param>
    ///<param name="reference_point">(Point3d) reference point of the shear transformation</param>
    ///<param name="angle_degrees">(int) the shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value:False, copy the objects</param>
    ///
    ///<returns>(Guid seq) identifiers of the sheared objects</returns>
    static member ShearObjects(object_ids:Guid seq, origin:Point3d, reference_point:Point3d, angle_degrees:int, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        Object.shearObjects copy angle_degrees reference_point origin object_ids

    ///<summary>Shows a previously hidden object. Hidden objects are not visible, cannot
    ///    be snapped to and cannot be selected</summary>
    ///<param name="object_id">(Guid) representing id of object to show</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member ShowObject(object_id:Guid) : bool =
        Object.showObject object_id

    ///<summary>Shows one or more objects. Hidden objects are not visible, cannot be
    ///    snapped to and cannot be selected</summary>
    ///<param name="object_ids">(Guid seq) ids of objects to show</param>
    ///
    ///<returns>(float) Number of objects shown</returns>
    static member ShowObjects(object_ids:Guid seq) : float =
        Object.showObjects object_ids

    ///<summary>Moves, scales, or rotates an object given a 4x4 transformation matrix.
    ///    The matrix acts on the left.</summary>
    ///<param name="object_id">(Guid) The identifier of the object.</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers).</param>
    ///<param name="copy">(bool) Optional, Default Value:False, Copy the object.</param>
    ///
    ///<returns>(Guid) The identifier of the transformed object</returns>
    static member TransformObject(object_id:Guid, matrix:Transform, [<OPT;DEF(false)>]copy:bool) : Guid =
        Object.transformObject copy matrix object_id

    ///<summary>Moves, scales, or rotates a list of objects given a 4x4 transformation
    ///    matrix. The matrix acts on the left.</summary>
    ///<param name="object_ids">(Guid seq) List of object identifiers.</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers).</param>
    ///<param name="copy">(bool) Optional, Default Value:False, Copy the objects</param>
    ///
    ///<returns>(Guid seq) ids identifying the newly transformed objects</returns>
    static member TransformObjects(object_ids:Guid seq, matrix:Transform, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        Object.transformObjects copy matrix object_ids

    ///<summary>Unlocks an object. Locked objects are visible, and can be snapped to,
    ///    but they cannot be selected.</summary>
    ///<param name="object_id">(Guid) The identifier of an object</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member UnlockObject(object_id:Guid) : bool =
        Object.unlockObject object_id

    ///<summary>Unlocks one or more objects. Locked objects are visible, and can be
    ///    snapped to, but they cannot be selected.</summary>
    ///<param name="object_ids">(Guid seq) The identifiers of objects</param>
    ///
    ///<returns>(float) number of objects unlocked</returns>
    static member UnlockObjects(object_ids:Guid seq) : float =
        Object.unlockObjects object_ids

    ///<summary>Unselects a single selected object</summary>
    ///<param name="object_id">(Guid) id of object to unselect</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member UnselectObject(object_id:Guid) : bool =
        Object.unselectObject object_id

    ///<summary>Unselects one or more selected objects.</summary>
    ///<param name="object_ids">(Guid seq) identifiers of the objects to unselect.</param>
    ///
    ///<returns>(float) The number of objects unselected</returns>
    static member UnselectObjects(object_ids:Guid seq) : float =
        Object.unselectObjects object_ids

    ///<summary>Returns the distance from a 3D point to a plane</summary>
    ///<param name="plane">(Plane) the plane</param>
    ///<param name="point">(Point3d) List of 3 numbers or Point3d</param>
    ///
    ///<returns>(float) The distance , otherwise None</returns>
    static member DistanceToPlane(plane:Plane, point:Point3d) : float =
        Plane.distanceToPlane point plane

    ///<summary>Evaluates a plane at a U,V parameter</summary>
    ///<param name="plane">(Plane) the plane to evaluate</param>
    ///<param name="parameter">(float * float) list of two numbers defining the U,V parameter to evaluate</param>
    ///
    ///<returns>(Point3d) Point3d on success</returns>
    static member EvaluatePlane(plane:Plane, parameter:float * float) : Point3d =
        Plane.evaluatePlane parameter plane

    ///<summary>Calculates the intersection of three planes</summary>
    ///<param name="plane1">(Plane) the 1st plane to intersect</param>
    ///<param name="plane2">(Plane) the 2nd plane to intersect</param>
    ///<param name="plane3">(Plane) the 3rd plane to intersect</param>
    ///
    ///<returns>(Point3d) the intersection point between the 3 planes on success</returns>
    static member IntersectPlanes(plane1:Plane, plane2:Plane, plane3:Plane) : Point3d =
        Plane.intersectPlanes plane3 plane2 plane1

    ///<summary>Moves the origin of a plane</summary>
    ///<param name="plane">(Plane) Plane or ConstructionPlane</param>
    ///<param name="origin">(Point3d) Point3d or list of three numbers</param>
    ///
    ///<returns>(Plane) moved plane</returns>
    static member MovePlane(plane:Plane, origin:Point3d) : Plane =
        Plane.movePlane origin plane

    ///<summary>Intersect an infinite plane and a curve object</summary>
    ///<param name="plane">(Plane) The plane to intersect.</param>
    ///<param name="curve">(Guid) The identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, The intersection tolerance. If omitted, the document's absolute tolerance is used.</param>
    ///
    ///<returns>(list of intersection information tuple if successful.  The list will contain one or more of the following tuple) Element Type        Description
    ///        [0]       Number      The intersection event type, either Point (1) or Overlap (2).
    ///        [1]       Point3d     If the event type is Point (1), then the intersection point on the curve.
    ///                            If the event type is Overlap (2), then intersection start point on the curve.
    ///        [2]       Point3d     If the event type is Point (1), then the intersection point on the curve.
    ///                            If the event type is Overlap (2), then intersection end point on the curve.
    ///        [3]       Point3d     If the event type is Point (1), then the intersection point on the plane.
    ///                            If the event type is Overlap (2), then intersection start point on the plane.
    ///        [4]       Point3d     If the event type is Point (1), then the intersection point on the plane.
    ///                            If the event type is Overlap (2), then intersection end point on the plane.
    ///        [5]       Number      If the event type is Point (1), then the curve parameter.
    ///                            If the event type is Overlap (2), then the start value of the curve parameter range.
    ///        [6]       Number      If the event type is Point (1), then the curve parameter.
    ///                            If the event type is Overlap (2),  then the end value of the curve parameter range.
    ///        [7]       Number      If the event type is Point (1), then the U plane parameter.
    ///                            If the event type is Overlap (2), then the U plane parameter for curve at (n, 5).
    ///        [8]       Number      If the event type is Point (1), then the V plane parameter.
    ///                            If the event type is Overlap (2), then the V plane parameter for curve at (n, 5).
    ///        [9]       Number      If the event type is Point (1), then the U plane parameter.
    ///                            If the event type is Overlap (2), then the U plane parameter for curve at (n, 6).
    ///        [10]      Number      If the event type is Point (1), then the V plane parameter.
    ///                            If the event type is Overlap (2), then the V plane parameter for curve at (n, 6).</returns>
    static member PlaneCurveIntersection(plane:Plane, curve:Guid, [<OPT;DEF(null)>]tolerance:float) : list of intersection information tuple if successful.  The list will contain one or more of the following tuple =
        Plane.planeCurveIntersection tolerance curve plane

    ///<summary>Returns the equation of a plane as a tuple of four numbers. The standard
    ///    equation of a plane with a non-zero vector is Ax+By+Cz+D=0</summary>
    ///<param name="plane">(Plane) the plane to deconstruct</param>
    ///
    ///<returns>(float * float * float * float) containing four numbers that represent the coefficients of the equation  (A, B, C, D)</returns>
    static member PlaneEquation(plane:Plane) : float * float * float * float =
        Plane.planeEquation plane

    ///<summary>Returns a plane that was fit through an array of 3D points.</summary>
    ///<param name="points">(Point3d) An array of 3D points.</param>
    ///
    ///<returns>(Plane) The plane</returns>
    static member PlaneFitFromPoints(points:Point3d) : Plane =
        Plane.planeFitFromPoints points

    ///<summary>Construct a plane from a point, and two vectors in the plane.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the plane.</param>
    ///<param name="x_axis">(Vector3d) A non-zero 3D vector in the plane that determines the X axis
    ///               direction.</param>
    ///<param name="y_axis">(Vector3d) A non-zero 3D vector not parallel to x_axis that is used
    ///               to determine the Y axis direction. Note, y_axis does not
    ///               have to be perpendicular to x_axis.</param>
    ///
    ///<returns>(Plane) The plane .</returns>
    static member PlaneFromFrame(origin:Point3d, x_axis:Vector3d, y_axis:Vector3d) : Plane =
        Plane.planeFromFrame y_axis x_axis origin

    ///<summary>Creates a plane from an origin point and a normal direction vector.</summary>
    ///<param name="origin">(Point3d) A 3D point identifying the origin of the plane.</param>
    ///<param name="normal">(Vector3d) A 3D vector identifying the normal direction of the plane.</param>
    ///<param name="xaxis">(Vector3d) Optional, Default Value:None, optional vector defining the plane's x-axis</param>
    ///
    ///<returns>(Plane) The plane .</returns>
    static member PlaneFromNormal(origin:Point3d, normal:Vector3d, [<OPT;DEF(null)>]xaxis:Vector3d) : Plane =
        Plane.planeFromNormal xaxis normal origin

    ///<summary>Creates a plane from three non-colinear points</summary>
    ///<param name="origin">(Point3d) origin point of the plane</param>
    ///<param name="x">(Point3d) x of 'points on the plane's x and y axes' (FIXME 0)</param>
    ///<param name="y">(Point3d) y of 'points on the plane's x and y axes' (FIXME 0)</param>
    ///
    ///<returns>(Plane) The plane , otherwise None</returns>
    static member PlaneFromPoints(origin:Point3d, x:Point3d, y:Point3d) : Plane =
        Plane.planeFromPoints y x origin

    ///<summary>Calculates the intersection of two planes</summary>
    ///<param name="plane1">(Plane) the 1st plane to intersect</param>
    ///<param name="plane2">(Plane) the 2nd plane to intersect</param>
    ///
    ///<returns>(Line) a line with two 3d points identifying the starting/ending points of the intersection</returns>
    static member PlanePlaneIntersection(plane1:Plane, plane2:Plane) : Line =
        Plane.planePlaneIntersection plane2 plane1

    ///<summary>Calculates the intersection of a plane and a sphere</summary>
    ///<param name="plane">(Plane) the plane to intersect</param>
    ///<param name="sphere_plane">(Plane) equatorial plane of the sphere. origin of the plane is
    ///        the center of the sphere</param>
    ///<param name="sphere_radius">(float) radius of the sphere</param>
    ///
    ///<returns>(float * Plane * float) of intersection results
    ///          Element    Type      Description
    ///          [0]       number     The type of intersection, where 0 = point and 1 = circle.
    ///          [1]   point or plane If a point intersection, the a Point3d identifying the 3-D intersection location.
    ///                               If a circle intersection, then the circle's plane. The origin of the plane will be the center point of the circle
    ///          [2]       number     If a circle intersection, then the radius of the circle.</returns>
    static member PlaneSphereIntersection(plane:Plane, sphere_plane:Plane, sphere_radius:float) : float * Plane * float =
        Plane.planeSphereIntersection sphere_radius sphere_plane plane

    ///<summary>Transforms a plane</summary>
    ///<param name="plane">(Plane) Plane to transform</param>
    ///<param name="xform">(Transform) Transformation to apply</param>
    ///
    ///<returns>(Plane) the resulting plane</returns>
    static member PlaneTransform(plane:Plane, xform:Transform) : Plane =
        Plane.planeTransform xform plane

    ///<summary>Rotates a plane</summary>
    ///<param name="plane">(Plane) Plane to rotate</param>
    ///<param name="angle_degrees">(int) rotation angle in degrees</param>
    ///<param name="axis">(Vector3d) Axis of rotation or list of three numbers</param>
    ///
    ///<returns>(Plane) rotated plane on success</returns>
    static member RotatePlane(plane:Plane, angle_degrees:int, axis:Vector3d) : Plane =
        Plane.rotatePlane axis angle_degrees plane

    ///<summary>Returns Rhino's world XY plane</summary>
    ///
    ///<returns>(Plane) Rhino's world XY plane</returns>
    static member WorldXYPlane() : Plane =
        Plane.worldXYPlane ()

    ///<summary>Returns Rhino's world YZ plane</summary>
    ///
    ///<returns>(Plane) Rhino's world YZ plane</returns>
    static member WorldYZPlane() : Plane =
        Plane.worldYZPlane ()

    ///<summary>Returns Rhino's world ZX plane</summary>
    ///
    ///<returns>(Plane) Rhino's world ZX plane</returns>
    static member WorldZXPlane() : Plane =
        Plane.worldZXPlane ()

    ///<summary>Compares two vectors to see if they are parallel</summary>
    ///<param name="vector1">(Vector3d) vector1 of 'the vectors to compare' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) vector2 of 'the vectors to compare' (FIXME 0)</param>
    ///
    ///<returns>(float) the value represents
    ///            -1 = the vectors are anti-parallel
    ///             0 = the vectors are not parallel
    ///             1 = the vectors are parallel</returns>
    static member IsVectorParallelTo(vector1:Vector3d, vector2:Vector3d) : float =
        Pointvector.isVectorParallelTo vector2 vector1

    ///<summary>Compares two vectors to see if they are perpendicular</summary>
    ///<param name="vector1">(Vector3d) vector1 of 'the vectors to compare' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) vector2 of 'the vectors to compare' (FIXME 0)</param>
    ///
    ///<returns>(bool) True if vectors are perpendicular, otherwise False</returns>
    static member IsVectorPerpendicularTo(vector1:Vector3d, vector2:Vector3d) : bool =
        Pointvector.isVectorPerpendicularTo vector2 vector1

    ///<summary>Verifies that a vector is very short. The X,Y,Z elements are <= 1.0e-12</summary>
    ///<param name="vector">(Vector3d) the vector to check</param>
    ///
    ///<returns>(bool) True if the vector is tiny, otherwise False</returns>
    static member IsVectorTiny(vector:Vector3d) : bool =
        Pointvector.isVectorTiny vector

    ///<summary>Verifies that a vector is zero, or tiny. The X,Y,Z elements are equal to 0.0</summary>
    ///<param name="vector">(Vector3d) the vector to check</param>
    ///
    ///<returns>(bool) True if the vector is zero, otherwise False</returns>
    static member IsVectorZero(vector:Vector3d) : bool =
        Pointvector.isVectorZero vector

    ///<summary>Adds a 3D point or a 3D vector to a 3D point</summary>
    ///<param name="point1">(Point3d) point1 of 'the points to add' (FIXME 0)</param>
    ///<param name="point2">(Point3d) point2 of 'the points to add' (FIXME 0)</param>
    ///
    ///<returns>(Point3d) the resulting 3D point</returns>
    static member PointAdd(point1:Point3d, point2:Point3d) : Point3d =
        Pointvector.pointAdd point2 point1

    ///<summary>Finds the point in a list of 3D points that is closest to a test point</summary>
    ///<param name="points">(Point3d seq) list of points</param>
    ///<param name="test_point">(Point3d) the point to compare against</param>
    ///
    ///<returns>(float) index of the element in the point list that is closest to the test point</returns>
    static member PointArrayClosestPoint(points:Point3d seq, test_point:Point3d) : float =
        Pointvector.pointArrayClosestPoint test_point points

    ///<summary>Transforms a list of 3D points</summary>
    ///<param name="points">(Point3d seq) list of 3D points</param>
    ///<param name="xform">(Transform) transformation to apply</param>
    ///
    ///<returns>(Point3d seq) transformed points on success</returns>
    static member PointArrayTransform(points:Point3d seq, xform:Transform) : Point3d seq =
        Pointvector.pointArrayTransform xform points

    ///<summary>Finds the object that is closest to a test point</summary>
    ///<param name="point">(Point3d) point to test</param>
    ///<param name="object_ids">(Guid seq) identifiers of one or more objects</param>
    ///
    ///<returns>(Guid * Point3d) closest [0] object_id and [1] point on object on success</returns>
    static member PointClosestObject(point:Point3d, object_ids:Guid seq) : Guid * Point3d =
        Pointvector.pointClosestObject object_ids point

    ///<summary>Compares two 3D points</summary>
    ///<param name="point1">(Point3d) point1 of 'the points to compare' (FIXME 0)</param>
    ///<param name="point2">(Point3d) point2 of 'the points to compare' (FIXME 0)</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, tolerance to use for comparison. If omitted,
    ///                                    Rhino's internal zero tolerance is used</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member PointCompare(point1:Point3d, point2:Point3d, [<OPT;DEF(null)>]tolerance:float) : bool =
        Pointvector.pointCompare tolerance point2 point1

    ///<summary>Divides a 3D point by a value</summary>
    ///<param name="point">(Point3d) the point to divide</param>
    ///<param name="divide">(int) a non-zero value to divide</param>
    ///
    ///<returns>(Point3d) resulting point</returns>
    static member PointDivide(point:Point3d, divide:int) : Point3d =
        Pointvector.pointDivide divide point

    ///<summary>Verifies that a list of 3D points are coplanar</summary>
    ///<param name="points">(Point3d seq) 3D points to test</param>
    ///<param name="tolerance">(float) Optional, Default Value:1.0e-12, tolerance to use when verifying</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member PointsAreCoplanar(points:Point3d seq, [<OPT;DEF(1.0e-12)>]tolerance:float) : bool =
        Pointvector.pointsAreCoplanar tolerance points

    ///<summary>Scales a 3D point by a value</summary>
    ///<param name="point">(Point3d) the point to divide</param>
    ///<param name="scale">(float) scale factor to apply</param>
    ///
    ///<returns>(Point3d) resulting point on success</returns>
    static member PointScale(point:Point3d, scale:float) : Point3d =
        Pointvector.pointScale scale point

    ///<summary>Subtracts a 3D point or a 3D vector from a 3D point</summary>
    ///<param name="point1">(Point3d) point1 of 'the points to subtract' (FIXME 0)</param>
    ///<param name="point2">(Point3d) point2 of 'the points to subtract' (FIXME 0)</param>
    ///
    ///<returns>(Point3d) the resulting 3D point</returns>
    static member PointSubtract(point1:Point3d, point2:Point3d) : Point3d =
        Pointvector.pointSubtract point2 point1

    ///<summary>Transforms a 3D point</summary>
    ///<param name="point">(Point3d) the point to transform</param>
    ///<param name="xform">(Transform) a valid 4x4 transformation matrix</param>
    ///
    ///<returns>(Vector3d) transformed vector on success</returns>
    static member PointTransform(point:Point3d, xform:Transform) : Vector3d =
        Pointvector.pointTransform xform point

    ///<summary>Projects one or more points onto one or more meshes</summary>
    ///<param name="points">(Point3d seq) one or more 3D points</param>
    ///<param name="mesh_ids">(Guid seq) identifiers of one or more meshes</param>
    ///<param name="direction">(Vector3d) direction vector to project the points</param>
    ///
    ///<returns>(Point3d seq) projected points on success</returns>
    static member ProjectPointToMesh(points:Point3d seq, mesh_ids:Guid seq, direction:Vector3d) : Point3d seq =
        Pointvector.projectPointToMesh direction mesh_ids points

    ///<summary>Projects one or more points onto one or more surfaces or polysurfaces</summary>
    ///<param name="points">(Point3d seq) one or more 3D points</param>
    ///<param name="surface_ids">(Guid seq) identifiers of one or more surfaces/polysurfaces</param>
    ///<param name="direction">(Vector3d) direction vector to project the points</param>
    ///
    ///<returns>(Point3d seq) projected points on success</returns>
    static member ProjectPointToSurface(points:Point3d seq, surface_ids:Guid seq, direction:Vector3d) : Point3d seq =
        Pointvector.projectPointToSurface direction surface_ids points

    ///<summary>Pulls an array of points to a surface or mesh object. For more
    ///    information, see the Rhino help file Pull command</summary>
    ///<param name="object_id">(Guid) the identifier of the surface or mesh object that pulls</param>
    ///<param name="points">(Point3d seq) list of 3D points</param>
    ///
    ///<returns>(Point3d seq) 3D points pulled onto surface or mesh</returns>
    static member PullPoints(object_id:Guid, points:Point3d seq) : Point3d seq =
        Pointvector.pullPoints points object_id

    ///<summary>Adds two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) vector1 of 'the vectors to add' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) vector2 of 'the vectors to add' (FIXME 0)</param>
    ///
    ///<returns>(Vector3d) the resulting 3D vector</returns>
    static member VectorAdd(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        Pointvector.vectorAdd vector2 vector1

    ///<summary>Returns the angle, in degrees, between two 3-D vectors</summary>
    ///<param name="vector1">(Vector3d) The first 3-D vector.</param>
    ///<param name="vector2">(Vector3d) The second 3-D vector.</param>
    ///
    ///<returns>(float) The angle in degrees</returns>
    static member VectorAngle(vector1:Vector3d, vector2:Vector3d) : float =
        Pointvector.vectorAngle vector2 vector1

    ///<summary>Compares two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) vector1 of 'the two vectors to compare' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) vector2 of 'the two vectors to compare' (FIXME 0)</param>
    ///
    ///<returns>(float) result of comparing the vectors.
    ///              -1 if vector1 is less than vector2
    ///              0 if vector1 is equal to vector2
    ///              1 if vector1 is greater than vector2</returns>
    static member VectorCompare(vector1:Vector3d, vector2:Vector3d) : float =
        Pointvector.vectorCompare vector2 vector1

    ///<summary>Creates a vector from two 3D points</summary>
    ///<param name="to_point">(Point3d) to point of 'the points defining the vector' (FIXME 0)</param>
    ///<param name="from_point">(Point3d) from point of 'the points defining the vector' (FIXME 0)</param>
    ///
    ///<returns>(Vector3d) the resulting vector</returns>
    static member VectorCreate(to_point:Point3d, from_point:Point3d) : Vector3d =
        Pointvector.vectorCreate from_point to_point

    ///<summary>Calculates the cross product of two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) vector1 of 'the vectors to perform cross product on' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) vector2 of 'the vectors to perform cross product on' (FIXME 0)</param>
    ///
    ///<returns>(Vector3d) the resulting cross product direction</returns>
    static member VectorCrossProduct(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        Pointvector.vectorCrossProduct vector2 vector1

    ///<summary>Divides a 3D vector by a value</summary>
    ///<param name="vector">(Vector3d) the vector to divide</param>
    ///<param name="divide">(int) a non-zero value to divide</param>
    ///
    ///<returns>(Vector3d) resulting vector on success</returns>
    static member VectorDivide(vector:Vector3d, divide:int) : Vector3d =
        Pointvector.vectorDivide divide vector

    ///<summary>Calculates the dot product of two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) vector1 of 'the vectors to perform the dot product on' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) vector2 of 'the vectors to perform the dot product on' (FIXME 0)</param>
    ///
    ///<returns>(Vector3d) the resulting dot product</returns>
    static member VectorDotProduct(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        Pointvector.vectorDotProduct vector2 vector1

    ///<summary>Returns the length of a 3D vector</summary>
    ///<param name="vector">(Vector3d) The 3-D vector.</param>
    ///
    ///<returns>(float) The length of the vector , otherwise None</returns>
    static member VectorLength(vector:Vector3d) : float =
        Pointvector.vectorLength vector

    ///<summary>Multiplies two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) vector1 of 'the vectors to multiply' (FIXME 0)</param>
    ///<param name="vector2">(Vector3d) vector2 of 'the vectors to multiply' (FIXME 0)</param>
    ///
    ///<returns>(Vector3d) the resulting inner (dot) product</returns>
    static member VectorMultiply(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        Pointvector.vectorMultiply vector2 vector1

    ///<summary>Reverses the direction of a 3D vector</summary>
    ///<param name="vector">(Vector3d) the vector to reverse</param>
    ///
    ///<returns>(Vector3d) reversed vector on success</returns>
    static member VectorReverse(vector:Vector3d) : Vector3d =
        Pointvector.vectorReverse vector

    ///<summary>Rotates a 3D vector</summary>
    ///<param name="vector">(Vector3d) the vector to rotate</param>
    ///<param name="angle_degrees">(int) rotation angle</param>
    ///<param name="axis">(Vector3d) axis of rotation</param>
    ///
    ///<returns>(Vector3d) rotated vector on success</returns>
    static member VectorRotate(vector:Vector3d, angle_degrees:int, axis:Vector3d) : Vector3d =
        Pointvector.vectorRotate axis angle_degrees vector

    ///<summary>Scales a 3-D vector</summary>
    ///<param name="vector">(Vector3d) the vector to scale</param>
    ///<param name="scale">(float) scale factor to apply</param>
    ///
    ///<returns>(Vector3d) resulting vector on success</returns>
    static member VectorScale(vector:Vector3d, scale:float) : Vector3d =
        Pointvector.vectorScale scale vector

    ///<summary>Subtracts two 3D vectors</summary>
    ///<param name="vector1">(Vector3d) the vector to subtract from</param>
    ///<param name="vector2">(Vector3d) the vector to subtract</param>
    ///
    ///<returns>(Vector3d) the resulting 3D vector</returns>
    static member VectorSubtract(vector1:Vector3d, vector2:Vector3d) : Vector3d =
        Pointvector.vectorSubtract vector2 vector1

    ///<summary>Transforms a 3D vector</summary>
    ///<param name="vector">(Vector3d) the vector to transform</param>
    ///<param name="xform">(Transform) a valid 4x4 transformation matrix</param>
    ///
    ///<returns>(Vector3d) transformed vector on success</returns>
    static member VectorTransform(vector:Vector3d, xform:Transform) : Vector3d =
        Pointvector.vectorTransform xform vector

    ///<summary>Unitizes, or normalizes a 3D vector. Note, zero vectors cannot be unitized</summary>
    ///<param name="vector">(Vector3d) the vector to unitize</param>
    ///
    ///<returns>(unit) unitized vector on success</returns>
    static member VectorUnitize(vector:Vector3d) : unit =
        Pointvector.vectorUnitize vector

    ///<summary>Returns either a world axis-aligned or a construction plane axis-aligned 
    ///    bounding box of an array of 3-D point locations.</summary>
    ///<param name="points">(Point3d seq) A list of 3-D points</param>
    ///<param name="view_or_plane">(Plane) Optional, Default Value:None, Title or id of the view that contains the
    ///          construction plane to which the bounding box should be aligned -or-
    ///          user defined plane. If omitted, a world axis-aligned bounding box
    ///          will be calculated</param>
    ///<param name="in_world_coords">(bool) Optional, Default Value:True, return the bounding box as world coordinates or
    ///          construction plane coordinates. Note, this option does not apply to
    ///          world axis-aligned bounding boxes.</param>
    ///
    ///<returns>(Point3d seq) Eight points that define the bounding box. Points returned in counter-
    ///      clockwise order starting with the bottom rectangle of the box.</returns>
    static member PointArrayBoundingBox(points:Point3d seq, [<OPT;DEF(null)>]view_or_plane:Plane, [<OPT;DEF(true)>]in_world_coords:bool) : Point3d seq =
        Pointvector.pointArrayBoundingBox in_world_coords view_or_plane points

    ///<summary>Returns identifiers of all objects in the document.</summary>
    ///<param name="select">(bool) Optional, Default Value:False, Select the objects</param>
    ///<param name="include_lights">(bool) Optional, Default Value:False, Include light objects</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, Include grips objects</param>
    ///<param name="include_references">(bool) Optional, Default Value:False, Include refrence objects such as work session objects</param>
    ///
    ///<returns>(Guid seq) identifiers for all the objects in the document</returns>
    static member AllObjects([<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool, [<OPT;DEF(false)>]include_references:bool) : Guid seq =
        Selection.allObjects include_references include_grips include_lights select

    ///<summary>Returns identifier of the first object in the document. The first
    ///    object is the last object created by the user.</summary>
    ///<param name="select">(bool) Optional, Default Value:False, Select the object.  If omitted (False), the object is not selected.</param>
    ///<param name="include_lights">(bool) Optional, Default Value:False, Include light objects.  If omitted (False), light objects are not returned.</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, Include grips objects.  If omitted (False), grips objects are not returned.</param>
    ///
    ///<returns>(Guid) The identifier of the object .</returns>
    static member FirstObject([<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool) : Guid =
        Selection.firstObject include_grips include_lights select

    ///<summary>A helper Function for Rhino.DocObjects.ObjectType Enum</summary>
    ///<param name="filter">(int) int representing one or several Enums as used ion Rhinopython for object types.</param>
    ///
    ///<returns>(Rhino.DocObjects.ObjectType) translated Rhino.DocObjects.ObjectType Enum</returns>
    static member __FilterHelper(filter:int) : Rhino.DocObjects.ObjectType =
        Selection.__FilterHelper filter

    ///<summary>Prompts user to pick or select a single curve object</summary>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message.</param>
    ///<param name="preselect">(bool) Optional, Default Value:False, Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value:False, Select the picked objects. If False, objects that
    ///        are picked are not selected.</param>
    ///
    ///<returns>(Guid * bool * int * Point3d * float * string) Tuple containing the following information
    ///        [0]  guid     identifier of the curve object
    ///        [1]  bool     True if the curve was preselected, otherwise False
    ///        [2]  number   selection method
    ///                         0 = selected by non-mouse method (SelAll, etc.).
    ///                         1 = selected by mouse click on the object.
    ///                         2 = selected by being inside of a mouse window.
    ///                         3 = selected by intersecting a mouse crossing window.
    ///        [3]  point    selection point
    ///        [4]  number   the curve parameter of the selection point
    ///        [5]  str      name of the view selection was made</returns>
    static member GetCurveObject([<OPT;DEF(null)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid * bool * int * Point3d * float * string =
        Selection.getCurveObject select preselect message

    ///<summary>Prompts user to pick, or select, a single object.</summary>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message.</param>
    ///<param name="filter">(float) Optional, Default Value:0, The type(s) of geometry (points, curves, surfaces, meshes,...)
    ///          that can be selected. Object types can be added together to filter
    ///          several different kinds of geometry. use the filter class to get values</param>
    ///<param name="preselect">(bool) Optional, Default Value:False, Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value:False, Select the picked objects.  If False, the objects that are
    ///          picked are not selected.</param>
    ///<param name="custom_filter">(function) Optional, Default Value:None, a custom filter function</param>
    ///<param name="subobjects">(bool) Optional, Default Value:False, If True, subobjects can be selected. When this is the
    ///          case, an ObjRef is returned instead of a Guid to allow for tracking
    ///          of the subobject when passed into other functions</param>
    ///
    ///<returns>(Guid) Identifier of the picked object</returns>
    static member GetObject([<OPT;DEF(null)>]message:string, [<OPT;DEF(0)>]filter:float, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null)>]custom_filter:function, [<OPT;DEF(false)>]subobjects:bool) : Guid =
        Selection.getObject subobjects custom_filter select preselect filter message

    ///<summary>Prompts user to pick or select one or more objects.</summary>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message.</param>
    ///<param name="filter">(float) Optional, Default Value:0, The type(s) of geometry (points, curves, surfaces, meshes,...)
    ///          that can be selected. Object types can be added together to filter
    ///          several different kinds of geometry. use the filter class to get values
    ///              Value         Description
    ///              0             All objects (default)
    ///              1             Point
    ///              2             Point cloud
    ///              4             Curve
    ///              8             Surface or single-face brep
    ///              16            Polysurface or multiple-face
    ///              32            Mesh
    ///              256           Light
    ///              512           Annotation
    ///              4096          Instance or block reference
    ///              8192          Text dot object
    ///              16384         Grip object
    ///              32768         Detail
    ///              65536         Hatch
    ///              131072        Morph control
    ///              134217728     Cage
    ///              268435456     Phantom
    ///              536870912     Clipping plane
    ///              1073741824    Extrusion</param>
    ///<param name="group">(bool) Optional, Default Value:True, Honor object grouping.  If omitted and the user picks a group,
    ///          the entire group will be picked (True). Note, if filter is set to a
    ///          value other than 0 (All objects), then group selection will be disabled.</param>
    ///<param name="preselect">(bool) Optional, Default Value:False, Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value:False, Select the picked objects.  If False, the objects that are
    ///          picked are not selected.</param>
    ///<param name="objects">(Guid seq) Optional, Default Value:None, list of objects that are allowed to be selected</param>
    ///<param name="minimum_count">(int) Optional, Default Value:1, minimum count of 'limits on number of objects allowed to be selected' (FIXME 0)</param>
    ///<param name="maximum_count">(int) Optional, Default Value:0, maximum count of 'limits on number of objects allowed to be selected' (FIXME 0)</param>
    ///<param name="custom_filter">(string) Optional, Default Value:None, Calls a custom function in the script and passes the Rhino Object, Geometry, and component index and returns true or false indicating if the object can be selected</param>
    ///
    ///<returns>(Guid seq) identifiers of the picked objects</returns>
    static member GetObjects([<OPT;DEF(null)>]message:string, [<OPT;DEF(0)>]filter:float, [<OPT;DEF(true)>]group:bool, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null)>]objects:Guid seq, [<OPT;DEF(1)>]minimum_count:int, [<OPT;DEF(0)>]maximum_count:int, [<OPT;DEF(null)>]custom_filter:string) : Guid seq =
        Selection.getObjects custom_filter maximum_count minimum_count objects select preselect group filter message

    ///<summary>Prompts user to pick, or select one or more objects</summary>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message.</param>
    ///<param name="filter">(float) Optional, Default Value:0, The type(s) of geometry (points, curves, surfaces, meshes,...)
    ///          that can be selected. Object types can be added together to filter
    ///          several different kinds of geometry. use the filter class to get values</param>
    ///<param name="group">(bool) Optional, Default Value:True, Honor object grouping.  If omitted and the user picks a group,
    ///          the entire group will be picked (True). Note, if filter is set to a
    ///          value other than 0 (All objects), then group selection will be disabled.</param>
    ///<param name="preselect">(bool) Optional, Default Value:False, Allow for the selection of pre-selected objects.</param>
    ///<param name="select">(bool) Optional, Default Value:False, Select the picked objects. If False, the objects that are
    ///          picked are not selected.</param>
    ///<param name="objects">(Guid seq) Optional, Default Value:None, list of object identifiers specifying objects that are
    ///          allowed to be selected</param>
    ///
    ///<returns>((Guid*bool*int*Point3d*string) seq) containing the following information
    ///        [n][0]  identifier of the object
    ///        [n][1]  True if the object was preselected, otherwise False
    ///        [n][2]  selection method (see help)
    ///        [n][3]  selection point
    ///        [n][4]  name of the view selection was made</returns>
    static member GetObjectsEx([<OPT;DEF(null)>]message:string, [<OPT;DEF(0)>]filter:float, [<OPT;DEF(true)>]group:bool, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(null)>]objects:Guid seq) : (Guid*bool*int*Point3d*string) seq =
        Selection.getObjectsEx objects select preselect group filter message

    ///<summary>Prompts the user to select one or more point objects.</summary>
    ///<param name="message">(string) Optional, Default Value:"Select points", a prompt message.</param>
    ///<param name="preselect">(bool) Optional, Default Value:False, Allow for the selection of pre-selected objects.  If omitted (False), pre-selected objects are not accepted.</param>
    ///
    ///<returns>(Point3d seq) 3d coordinates of point objects on success</returns>
    static member GetPointCoordinates([<OPT;DEF("Select points")>]message:string, [<OPT;DEF(false)>]preselect:bool) : Point3d seq =
        Selection.getPointCoordinates preselect message

    ///<summary>Prompts the user to select a single surface</summary>
    ///<param name="message">(string) Optional, Default Value:"Select surface", prompt displayed</param>
    ///<param name="preselect">(bool) Optional, Default Value:False, allow for preselected objects</param>
    ///<param name="select">(bool) Optional, Default Value:False, select the picked object</param>
    ///
    ///<returns>(Guid * bool * float * Point3d * (float * float) * string) of information on success
    ///        [0]  identifier of the surface
    ///        [1]  True if the surface was preselected, otherwise False
    ///        [2]  selection method ( see help )
    ///        [3]  selection point
    ///        [4]  u,v surface parameter of the selection point
    ///        [5]  name of the view in which the selection was made</returns>
    static member GetSurfaceObject([<OPT;DEF("Select surface")>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid * bool * float * Point3d * (float * float) * string =
        Selection.getSurfaceObject select preselect message

    ///<summary>Returns identifiers of all locked objects in the document. Locked objects
    ///    cannot be snapped to, and cannot be selected</summary>
    ///<param name="include_lights">(bool) Optional, Default Value:False, include light objects</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, include grip objects</param>
    ///<param name="include_references">(bool) Optional, Default Value:False, Include refrence objects such as work session objects</param>
    ///
    ///<returns>(Guid seq) identifiers the locked objects .</returns>
    static member LockedObjects([<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool, [<OPT;DEF(false)>]include_references:bool) : Guid seq =
        Selection.lockedObjects include_references include_grips include_lights

    ///<summary>Returns identifiers of all hidden objects in the document. Hidden objects
    ///    are not visible, cannot be snapped to, and cannot be selected</summary>
    ///<param name="include_lights">(bool) Optional, Default Value:False, include light objects</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, include grip objects</param>
    ///<param name="include_references">(bool) Optional, Default Value:False, Include refrence objects such as work session objects</param>
    ///
    ///<returns>(Guid seq) identifiers of the hidden objects .</returns>
    static member HiddenObjects([<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool, [<OPT;DEF(false)>]include_references:bool) : Guid seq =
        Selection.hiddenObjects include_references include_grips include_lights

    ///<summary>Inverts the current object selection. The identifiers of the newly
    ///    selected objects are returned</summary>
    ///<param name="include_lights">(bool) Optional, Default Value:False, Include light objects.  If omitted (False), light objects are not returned.</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, Include grips objects.  If omitted (False), grips objects are not returned.</param>
    ///<param name="include_references">(bool) Optional, Default Value:False, Include refrence objects such as work session objects</param>
    ///
    ///<returns>(Guid seq) identifiers of the newly selected objects .</returns>
    static member InvertSelectedObjects([<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool, [<OPT;DEF(false)>]include_references:bool) : Guid seq =
        Selection.invertSelectedObjects include_references include_grips include_lights

    ///<summary>Returns identifiers of the objects that were most recently created or changed
    ///    by scripting a Rhino command using the Command function. It is important to
    ///    call this function immediately after calling the Command function as only the
    ///    most recently created or changed object identifiers will be returned</summary>
    ///<param name="select">(bool) Optional, Default Value:False, Select the object.  If omitted (False), the object is not selected.</param>
    ///
    ///<returns>(Guid seq) identifiers of the most recently created or changed objects .</returns>
    static member LastCreatedObjects([<OPT;DEF(false)>]select:bool) : Guid seq =
        Selection.lastCreatedObjects select

    ///<summary>Returns the identifier of the last object in the document. The last object
    ///    in the document is the first object created by the user</summary>
    ///<param name="select">(bool) Optional, Default Value:False, select the object</param>
    ///<param name="include_lights">(bool) Optional, Default Value:False, include lights in the potential set</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, include grips in the potential set</param>
    ///
    ///<returns>(Guid) identifier of the object on success</returns>
    static member LastObject([<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool) : Guid =
        Selection.lastObject include_grips include_lights select

    ///<summary>Returns the identifier of the next object in the document</summary>
    ///<param name="object_id">(Guid) the identifier of the object from which to get the next object</param>
    ///<param name="select">(bool) Optional, Default Value:False, select the object</param>
    ///<param name="include_lights">(bool) Optional, Default Value:False, include lights in the potential set</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, include grips in the potential set</param>
    ///
    ///<returns>(Guid) identifier of the object on success</returns>
    static member NextObject(object_id:Guid, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool) : Guid =
        Selection.nextObject include_grips include_lights select object_id

    ///<summary>Returns identifiers of all normal objects in the document. Normal objects
    ///    are visible, can be snapped to, and are independent of selection state</summary>
    ///<param name="include_lights">(bool) Optional, Default Value:False, Include light objects.  If omitted (False), light objects are not returned.</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, Include grips objects.  If omitted (False), grips objects are not returned.</param>
    ///
    ///<returns>(Guid seq) identifier of normal objects .</returns>
    static member NormalObjects([<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool) : Guid seq =
        Selection.normalObjects include_grips include_lights

    ///<summary>Returns identifiers of all objects based on color</summary>
    ///<param name="color">(Drawing.Color) color to get objects by</param>
    ///<param name="select">(bool) Optional, Default Value:False, select the objects</param>
    ///<param name="include_lights">(bool) Optional, Default Value:False, include lights in the set</param>
    ///
    ///<returns>(Guid seq) identifiers of objects of the selected color.</returns>
    static member ObjectsByColor(color:Drawing.Color, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]include_lights:bool) : Guid seq =
        Selection.objectsByColor include_lights select color

    ///<summary>Returns identifiers of all objects based on the objects' group name</summary>
    ///<param name="group_name">(string) name of the group</param>
    ///<param name="select">(bool) Optional, Default Value:False, select the objects</param>
    ///
    ///<returns>(Guid seq) identifiers for objects in the group on success</returns>
    static member ObjectsByGroup(group_name:string, [<OPT;DEF(false)>]select:bool) : Guid seq =
        Selection.objectsByGroup select group_name

    ///<summary>Returns identifiers of all objects based on the objects' layer name</summary>
    ///<param name="layer_name">(string) name of the layer</param>
    ///<param name="select">(bool) Optional, Default Value:False, select the objects</param>
    ///
    ///<returns>(Guid seq) identifiers for objects in the specified layer</returns>
    static member ObjectsByLayer(layer_name:string, [<OPT;DEF(false)>]select:bool) : Guid seq =
        Selection.objectsByLayer select layer_name

    ///<summary>Returns identifiers of all objects based on user-assigned name</summary>
    ///<param name="name">(string) name of the object or objects</param>
    ///<param name="select">(bool) Optional, Default Value:False, select the objects</param>
    ///<param name="include_lights">(bool) Optional, Default Value:False, include light objects</param>
    ///<param name="include_references">(bool) Optional, Default Value:False, Include refrence objects such as work session objects</param>
    ///
    ///<returns>(Guid seq) identifiers for objects with the specified name.</returns>
    static member ObjectsByName(name:string, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_references:bool) : Guid seq =
        Selection.objectsByName include_references include_lights select name

    ///<summary>Returns identifiers of all objects based on the objects' geometry type.</summary>
    ///<param name="geometry_type">(int) The type(s) of geometry objects (points, curves, surfaces,
    ///             meshes, etc.) that can be selected. Object types can be
    ///             added together as bit-coded flags to filter several different kinds of geometry.
    ///              Value        Description
    ///               0           All objects
    ///               1           Point
    ///               2           Point cloud
    ///               4           Curve
    ///               8           Surface or single-face brep
    ///               16          Polysurface or multiple-face
    ///               32          Mesh
    ///               256         Light
    ///               512         Annotation
    ///               4096        Instance or block reference
    ///               8192        Text dot object
    ///               16384       Grip object
    ///               32768       Detail
    ///               65536       Hatch
    ///               131072      Morph control
    ///               134217728   Cage
    ///               268435456   Phantom
    ///               536870912   Clipping plane
    ///               1073741824  Extrusion</param>
    ///<param name="select">(bool) Optional, Default Value:False, Select the objects</param>
    ///<param name="state">(bool) Optional, Default Value:0, The object state (normal, locked, and hidden). Object states can be
    ///        added together to filter several different states of geometry.
    ///              Value     Description
    ///              0         All objects
    ///              1         Normal objects
    ///              2         Locked objects
    ///              4         Hidden objects</param>
    ///
    ///<returns>(Guid seq) identifiers of object that fit the specified type(s).</returns>
    static member ObjectsByType(geometry_type:int, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(0)>]state:bool) : Guid seq =
        Selection.objectsByType state select geometry_type

    ///<summary>Returns the identifiers of all objects that are currently selected</summary>
    ///<param name="include_lights">(bool) Optional, Default Value:False, include light objects</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, include grip objects</param>
    ///
    ///<returns>(Guid * ...identifiersofselectedobjects) </returns>
    static member SelectedObjects([<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool) : Guid * ...identifiersofselectedobjects =
        Selection.selectedObjects include_grips include_lights

    ///<summary>Unselects all objects in the document</summary>
    ///
    ///<returns>(float) the number of objects that were unselected</returns>
    static member UnselectAllObjects() : float =
        Selection.unselectAllObjects ()

    ///<summary>Return identifiers of all objects that are visible in a specified view</summary>
    ///<param name="view">(bool) Optional, Default Value:None, the view to use. If omitted, the current active view is used</param>
    ///<param name="select">(bool) Optional, Default Value:False, Select the objects</param>
    ///<param name="include_lights">(bool) Optional, Default Value:False, include light objects</param>
    ///<param name="include_grips">(bool) Optional, Default Value:False, include grip objects</param>
    ///
    ///<returns>(Guid seq) identifiers of the visible objects</returns>
    static member VisibleObjects([<OPT;DEF(null)>]view:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(false)>]include_lights:bool, [<OPT;DEF(false)>]include_grips:bool) : Guid seq =
        Selection.visibleObjects include_grips include_lights select view

    ///<summary>Picks objects using either a window or crossing selection</summary>
    ///<param name="corner1">(Point3d) corner1 of 'corners of selection window' (FIXME 0)</param>
    ///<param name="corner2">(Point3d) corner2 of 'corners of selection window' (FIXME 0)</param>
    ///<param name="view">(bool) Optional, Default Value:None, view to perform the selection in</param>
    ///<param name="select">(bool) Optional, Default Value:False, select picked objects</param>
    ///<param name="in_window">(bool) Optional, Default Value:True, if False, then a crossing window selection is performed</param>
    ///
    ///<returns>(Guid seq) identifiers of selected objects on success</returns>
    static member WindowPick(corner1:Point3d, corner2:Point3d, [<OPT;DEF(null)>]view:bool, [<OPT;DEF(false)>]select:bool, [<OPT;DEF(true)>]in_window:bool) : Guid seq =
        Selection.windowPick in_window select view corner2 corner1

    ///<summary>Adds a box shaped polysurface to the document</summary>
    ///<param name="corners">(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) 8 points that define the corners of the box. Points need to
    ///        be in counter-clockwise order starting with the bottom rectangle of the box</param>
    ///
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddBox(corners:Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) : Guid =
        Surface.addBox corners

    ///<summary>Adds a cone shaped polysurface to the document</summary>
    ///<param name="base">(Plane) 3D origin point of the cone or a plane with an apex at the origin
    ///          and normal along the plane's z-axis</param>
    ///<param name="height">(point|number) 3D height point of the cone if base is a 3D point. The height
    ///          point defines the height and direction of the cone. If base is a
    ///          plane, height is a numeric value</param>
    ///<param name="radius">(float) the radius at the base of the cone</param>
    ///<param name="cap">(bool) Optional, Default Value:True, cap base of the cone</param>
    ///
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddCone(base:Plane, height:point|number, radius:float, [<OPT;DEF(true)>]cap:bool) : Guid =
        Surface.addCone cap radius height base

    ///<summary>Adds a planar surface through objects at a designated location. For more
    ///    information, see the Rhino help file for the CutPlane command</summary>
    ///<param name="object_ids">(Guid seq) identifiers of objects that the cutting plane will
    ///          pass through</param>
    ///<param name="start_point">(Line) start point of 'line that defines the cutting plane' (FIXME 0)</param>
    ///<param name="end_point">(Line) end point of 'line that defines the cutting plane' (FIXME 0)</param>
    ///<param name="normal">(Vector3d) Optional, Default Value:None, vector that will be contained in the returned planar
    ///          surface. In the case of Rhino's CutPlane command, this is the
    ///          normal to, or Z axis of, the active view's construction plane.
    ///          If omitted, the world Z axis is used</param>
    ///
    ///<returns>(Guid) identifier of new object on success</returns>
    static member AddCutPlane(object_ids:Guid seq, start_point:Line, end_point:Line, [<OPT;DEF(null)>]normal:Vector3d) : Guid =
        Surface.addCutPlane normal end_point start_point object_ids

    ///<summary>Adds a cylinder-shaped polysurface to the document</summary>
    ///<param name="base">(Plane) The 3D base point of the cylinder or the base plane of the cylinder</param>
    ///<param name="height">(point|number) if base is a point, then height is a 3D height point of the
    ///        cylinder. The height point defines the height and direction of the
    ///        cylinder. If base is a plane, then height is the numeric height value
    ///        of the cylinder</param>
    ///<param name="radius">(float) radius of the cylinder</param>
    ///<param name="cap">(bool) Optional, Default Value:True, cap the cylinder</param>
    ///
    ///<returns>(Guid) identifier of new object</returns>
    static member AddCylinder(base:Plane, height:point|number, radius:float, [<OPT;DEF(true)>]cap:bool) : Guid =
        Surface.addCylinder cap radius height base

    ///<summary>Creates a surface from 2, 3, or 4 edge curves</summary>
    ///<param name="curve_ids">(Guid seq) list or tuple of curves</param>
    ///
    ///<returns>(Guid) identifier of new object</returns>
    static member AddEdgeSrf(curve_ids:Guid seq) : Guid =
        Surface.addEdgeSrf curve_ids

    ///<summary>Creates a surface from a network of crossing curves</summary>
    ///<param name="curves">(Guid seq) curves from which to create the surface</param>
    ///<param name="continuity">(float) Optional, Default Value:1, how the edges match the input geometry
    ///                 0 = loose
    ///                 1 = position
    ///                 2 = tangency
    ///                 3 = curvature</param>
    ///<param name="edge_tolerance">(float) Optional, Default Value:0, edge tolerance</param>
    ///<param name="interior_tolerance">(float) Optional, Default Value:0, interior tolerance</param>
    ///<param name="angle_tolerance">(float) Optional, Default Value:0, angle tolerance , in radians?</param>
    ///
    ///<returns>(Guid) identifier of new object</returns>
    static member AddNetworkSrf(curves:Guid seq, [<OPT;DEF(1)>]continuity:float, [<OPT;DEF(0)>]edge_tolerance:float, [<OPT;DEF(0)>]interior_tolerance:float, [<OPT;DEF(0)>]angle_tolerance:float) : Guid =
        Surface.addNetworkSrf angle_tolerance interior_tolerance edge_tolerance continuity curves

    ///<summary>Adds a NURBS surface object to the document</summary>
    ///<param name="point_count">(int * int) number of control points in the u and v direction</param>
    ///<param name="points">(Point3d seq) list of 3D points</param>
    ///<param name="knots_u">(float seq) knot values for the surface in the u direction.
    ///                Must contain point_count[0]+degree[0]-1 elements</param>
    ///<param name="knots_v">(float seq) knot values for the surface in the v direction.
    ///                Must contain point_count[1]+degree[1]-1 elements</param>
    ///<param name="degree">(int * int) degree of the surface in the u and v directions.</param>
    ///<param name="weights">(float seq) Optional, Default Value:None, weight values for the surface. The number of elements in
    ///        weights must equal the number of elements in points. Values must be
    ///        greater than zero.</param>
    ///
    ///<returns>(Guid) identifier of new object 
    ///      </returns>
    static member AddNurbsSurface(point_count:int * int, points:Point3d seq, knots_u:float seq, knots_v:float seq, degree:int * int, [<OPT;DEF(null)>]weights:float seq) : Guid =
        Surface.addNurbsSurface weights degree knots_v knots_u points point_count

    ///<summary>Fits a surface through curve, point, point cloud, and mesh objects.</summary>
    ///<param name="object_ids">(Guid seq) a list of object identifiers that indicate the objects to use for the patch fitting.
    ///          Acceptable object types include curves, points, point clouds, and meshes.</param>
    ///<param name="uv_spans_tuple_OR_surface_object_id">(float * Guid) the U and V direction span counts for the automatically generated surface OR
    ///          The identifier of the starting surface.  It is best if you create a starting surface that is similar in shape
    ///          to the surface you are trying to create.</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, The tolerance used by input analysis functions. If omitted, Rhino's document absolute tolerance is used.</param>
    ///<param name="trim">(bool) Optional, Default Value:True, Try to find an outside curve and trims the surface to it.  The default value is True.</param>
    ///<param name="point_spacing">(float) Optional, Default Value:0.1, The basic distance between points sampled from input curves.  The default value is 0.1.</param>
    ///<param name="flexibility">(float) Optional, Default Value:1.0, Determines the behavior of the surface in areas where its not otherwise controlled by the input.
    ///          Lower numbers make the surface behave more like a stiff material, higher, more like a flexible material.
    ///          That is, each span is made to more closely match the spans adjacent to it if there is no input geometry
    ///          mapping to that area of the surface when the flexibility value is low.  The scale is logarithmic.
    ///          For example, numbers around 0.001 or 0.1 make the patch pretty stiff and numbers around 10 or 100
    ///          make the surface flexible.  The default value is 1.0.</param>
    ///<param name="surface_pull">(float) Optional, Default Value:1.0, Similar to stiffness, but applies to the starting surface. The bigger the pull, the closer
    ///          the resulting surface shape will be to the starting surface.  The default value is 1.0.</param>
    ///<param name="fix_edges">(bool) Optional, Default Value:False, Clamps the edges of the starting surface in place. This option is useful if you are using a
    ///          curve or points for deforming an existing surface, and you do not want the edges of the starting surface
    ///          to move.  The default if False.</param>
    ///
    ///<returns>(Guid) Identifier of the new surface object .</returns>
    static member AddPatch(object_ids:Guid seq, uv_spans_tuple_OR_surface_object_id:float * Guid, [<OPT;DEF(null)>]tolerance:float, [<OPT;DEF(true)>]trim:bool, [<OPT;DEF(0.1)>]point_spacing:float, [<OPT;DEF(1.0)>]flexibility:float, [<OPT;DEF(1.0)>]surface_pull:float, [<OPT;DEF(false)>]fix_edges:bool) : Guid =
        Surface.addPatch fix_edges surface_pull flexibility point_spacing trim tolerance uv_spans_tuple_OR_surface_object_id object_ids

    ///<summary>Creates a single walled surface with a circular profile around a curve</summary>
    ///<param name="curve_id">(Guid) identifier of rail curve</param>
    ///<param name="parameters">(float seq) parameters of 'list of radius values at normalized curve parameters' (FIXME 0)</param>
    ///<param name="radii">(float seq) radii of 'list of radius values at normalized curve parameters' (FIXME 0)</param>
    ///<param name="blend_type">(int) Optional, Default Value:0, 0(local) or 1(global)</param>
    ///<param name="cap">(float) Optional, Default Value:0, 0(none), 1(flat), 2(round)</param>
    ///<param name="fit">(bool) Optional, Default Value:False, attempt to fit a single surface</param>
    ///
    ///<returns>(Guid seq) identifiers of new objects created</returns>
    static member AddPipe(curve_id:Guid, parameters:float seq, radii:float seq, [<OPT;DEF(0)>]blend_type:int, [<OPT;DEF(0)>]cap:float, [<OPT;DEF(false)>]fit:bool) : Guid seq =
        Surface.addPipe fit cap blend_type radii parameters curve_id

    ///<summary>Creates one or more surfaces from planar curves</summary>
    ///<param name="object_ids">(Guid seq) curves to use for creating planar surfaces</param>
    ///
    ///<returns>(Guid seq) identifiers of surfaces created on success</returns>
    static member AddPlanarSrf(object_ids:Guid seq) : Guid seq =
        Surface.addPlanarSrf object_ids

    ///<summary>Create a plane surface and add it to the document.</summary>
    ///<param name="plane">(Plane) The plane.</param>
    ///<param name="u_dir">(float) The magnitude in the U direction.</param>
    ///<param name="v_dir">(float) The magnitude in the V direction.</param>
    ///
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddPlaneSurface(plane:Plane, u_dir:float, v_dir:float) : Guid =
        Surface.addPlaneSurface v_dir u_dir plane

    ///<summary>Adds a surface created by lofting curves to the document.
    ///    - no curve sorting performed. pass in curves in the order you want them sorted
    ///    - directions of open curves not adjusted. Use CurveDirectionsMatch and
    ///      ReverseCurve to adjust the directions of open curves
    ///    - seams of closed curves are not adjusted. Use CurveSeam to adjust the seam
    ///      of closed curves</summary>
    ///<param name="object_ids">(Guid seq) ordered list of the curves to loft through</param>
    ///<param name="start">(Point3d) Optional, Default Value:None, starting point of the loft</param>
    ///<param name="end">(Point3d) Optional, Default Value:None, ending point of the loft</param>
    ///<param name="loft_type">(int) Optional, Default Value:0, type of loft. Possible options are:
    ///        0 = Normal. Uses chord-length parameterization in the loft direction
    ///        1 = Loose. The surface is allowed to move away from the original curves
    ///            to make a smoother surface. The surface control points are created
    ///            at the same locations as the control points of the loft input curves.
    ///        2 = Straight. The sections between the curves are straight. This is
    ///            also known as a ruled surface.
    ///        3 = Tight. The surface sticks closely to the original curves. Uses square
    ///            root of chord-length parameterization in the loft direction
    ///        4 = Developable. Creates a separate developable surface or polysurface
    ///            from each pair of curves.</param>
    ///<param name="simplify_method">(float) Optional, Default Value:0, Possible options are:
    ///        0 = None. Does not simplify.
    ///        1 = Rebuild. Rebuilds the shape curves before lofting. modified by `value` below
    ///        2 = Refit. Refits the shape curves to a specified tolerance. modified by `value` below</param>
    ///<param name="value">(float) Optional, Default Value:0, Additional value based on the specified `simplify_method`:
    ///        Simplify  -   Description
    ///        Rebuild(1) - then value is the number of control point used to rebuild
    ///        Rebuild(1) - is specified and this argument is omitted, then curves will be
    ///                     rebuilt using 10 control points.
    ///        Refit(2) - then value is the tolerance used to rebuild.
    ///        Refit(2) - is specified and this argument is omitted, then the document's
    ///                     absolute tolerance us used for refitting.</param>
    ///<param name="closed">(bool) Optional, Default Value:False, close the loft back to the first curve</param>
    ///
    ///<returns>(Guid seq) Array containing the identifiers of the new surface objects</returns>
    static member AddLoftSrf(object_ids:Guid seq, [<OPT;DEF(null)>]start:Point3d, [<OPT;DEF(null)>]end:Point3d, [<OPT;DEF(0)>]loft_type:int, [<OPT;DEF(0)>]simplify_method:float, [<OPT;DEF(0)>]value:float, [<OPT;DEF(false)>]closed:bool) : Guid seq =
        Surface.addLoftSrf closed value simplify_method loft_type end start object_ids

    ///<summary>Create a surface by revolving a curve around an axis</summary>
    ///<param name="curve_id">(Guid) identifier of profile curve</param>
    ///<param name="axis">(Line) line for the rail revolve axis</param>
    ///<param name="start_angle">(float) Optional, Default Value:0.0, start angles of revolve</param>
    ///<param name="end_angle">(float) Optional, Default Value:360.0, end angles of revolve</param>
    ///
    ///<returns>(Guid) identifier of new object</returns>
    static member AddRevSrf(curve_id:Guid, axis:Line, [<OPT;DEF(0.0)>]start_angle:float, [<OPT;DEF(360.0)>]end_angle:float) : Guid =
        Surface.addRevSrf end_angle start_angle axis curve_id

    ///<summary>Add a spherical surface to the document</summary>
    ///<param name="center_or_plane">(Plane) center point of the sphere. If a plane is input,
    ///        the origin of the plane will be the center of the sphere</param>
    ///<param name="radius">(float) radius of the sphere in the current model units</param>
    ///
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddSphere(center_or_plane:Plane, radius:float) : Guid =
        Surface.addSphere radius center_or_plane

    ///<summary>Adds a spaced series of planar curves resulting from the intersection of
    ///    defined cutting planes through a surface or polysurface. For more
    ///    information, see Rhino help for details on the Contour command</summary>
    ///<param name="object_id">(Guid) object identifier to contour</param>
    ///<param name="points_or_plane">(Point3d * Plane) either a list/tuple of two points or a plane
    ///        if two points, they define the start and end points of a center line
    ///        if a plane, the plane defines the cutting plane</param>
    ///<param name="interval">(float) Optional, Default Value:None, distance between contour curves.</param>
    ///
    ///<returns>(Guid) ids of new contour curves on success</returns>
    static member AddSrfContourCrvs(object_id:Guid, points_or_plane:Point3d * Plane, [<OPT;DEF(null)>]interval:float) : Guid =
        Surface.addSrfContourCrvs interval points_or_plane object_id

    ///<summary>Creates a surface from a grid of points</summary>
    ///<param name="count">(int * int) tuple of two numbers defining number of points in the u,v directions</param>
    ///<param name="points">(Point3d seq) list of 3D points</param>
    ///<param name="degree">(int * int) Optional, Default Value:3*3, two numbers defining degree of the surface in the u,v directions</param>
    ///
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddSrfControlPtGrid(count:int * int, points:Point3d seq, [<OPT;DEF(3*3)>]degree:int * int) : Guid =
        Surface.addSrfControlPtGrid degree points count

    ///<summary>Creates a new surface from either 3 or 4 corner points.</summary>
    ///<param name="points">(Point3d * Point3d * Point3d * Point3d) list of either 3 or 4 corner points</param>
    ///
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddSrfPt(points:Point3d * Point3d * Point3d * Point3d) : Guid =
        Surface.addSrfPt points

    ///<summary>Creates a surface from a grid of points</summary>
    ///<param name="count">(int * int) tuple of two numbers defining number of points in the u,v directions</param>
    ///<param name="points">(Point3d seq) list of 3D points</param>
    ///<param name="degree">(int * int) Optional, Default Value:3*3, two numbers defining degree of the surface in the u,v directions</param>
    ///<param name="closed">(bool * bool) Optional, Default Value:false*false, two booleans defining if the surface is closed in the u,v directions</param>
    ///
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddSrfPtGrid(count:int * int, points:Point3d seq, [<OPT;DEF(3*3)>]degree:int * int, [<OPT;DEF(false*false)>]closed:bool * bool) : Guid =
        Surface.addSrfPtGrid closed degree points count

    ///<summary>Adds a surface created through profile curves that define the surface
    ///    shape and one curve that defines a surface edge.</summary>
    ///<param name="rail">(Guid) identifier of the rail curve</param>
    ///<param name="shapes">(Guid seq) one or more cross section shape curves</param>
    ///<param name="closed">(bool) Optional, Default Value:False, If True, then create a closed surface</param>
    ///
    ///<returns>(Guid seq) of new surface objects</returns>
    static member AddSweep1(rail:Guid, shapes:Guid seq, [<OPT;DEF(false)>]closed:bool) : Guid seq =
        Surface.addSweep1 closed shapes rail

    ///<summary>Adds a surface created through profile curves that define the surface
    ///    shape and two curves that defines a surface edge.</summary>
    ///<param name="rails">(Guid * Guid) identifiers of the two rail curve</param>
    ///<param name="shapes">(Guid seq) one or more cross section shape curves</param>
    ///<param name="closed">(bool) Optional, Default Value:False, If True, then create a closed surface</param>
    ///
    ///<returns>(Guid seq) of new surface objects</returns>
    static member AddSweep2(rails:Guid * Guid, shapes:Guid seq, [<OPT;DEF(false)>]closed:bool) : Guid seq =
        Surface.addSweep2 closed shapes rails

    ///<summary>Adds a surface created through profile curves that define the surface
    ///    shape and two curves that defines a surface edge.</summary>
    ///<param name="profile">(Guid) identifier of the profile curve</param>
    ///<param name="rail">(Guid) identifier of the rail curve</param>
    ///<param name="axis">(Point3d * Point3d) A list of two 3-D points identifying the start point and end point of the rail revolve axis, or a Line</param>
    ///<param name="scale_height">(bool) Optional, Default Value:False, If True, surface will be locally scaled. Defaults to False</param>
    ///
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddRailRevSrf(profile:Guid, rail:Guid, axis:Point3d * Point3d, [<OPT;DEF(false)>]scale_height:bool) : Guid =
        Surface.addRailRevSrf scale_height axis rail profile

    ///<summary>Adds a torus shaped revolved surface to the document</summary>
    ///<param name="base">(Point3d) 3D origin point of the torus or the base plane of the torus</param>
    ///<param name="major_radius">(float) major radius of 'the two radii of the torus' (FIXME 0)</param>
    ///<param name="minor_radius">(float) minor radius of 'the two radii of the torus' (FIXME 0)</param>
    ///<param name="direction">(Point3d) Optional, Default Value:None, A point that defines the direction of the torus when base is a point.
    ///        If omitted, a torus that is parallel to the world XY plane is created</param>
    ///
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddTorus(base:Point3d, major_radius:float, minor_radius:float, [<OPT;DEF(null)>]direction:Point3d) : Guid =
        Surface.addTorus direction minor_radius major_radius base

    ///<summary>Performs a boolean difference operation on two sets of input surfaces
    ///    and polysurfaces. For more details, see the BooleanDifference command in
    ///    the Rhino help file</summary>
    ///<param name="input0">(Guid seq) list of surfaces to subtract from</param>
    ///<param name="input1">(Guid seq) list of surfaces to be subtracted</param>
    ///<param name="delete_input">(bool) Optional, Default Value:True, delete all input objects</param>
    ///
    ///<returns>(Guid seq) of identifiers of newly created objects on success</returns>
    static member BooleanDifference(input0:Guid seq, input1:Guid seq, [<OPT;DEF(true)>]delete_input:bool) : Guid seq =
        Surface.booleanDifference delete_input input1 input0

    ///<summary>Performs a boolean intersection operation on two sets of input surfaces
    ///    and polysurfaces. For more details, see the BooleanIntersection command in
    ///    the Rhino help file</summary>
    ///<param name="input0">(Guid seq) list of surfaces</param>
    ///<param name="input1">(Guid seq) list of surfaces</param>
    ///<param name="delete_input">(bool) Optional, Default Value:True, delete all input objects</param>
    ///
    ///<returns>(Guid seq) of identifiers of newly created objects on success</returns>
    static member BooleanIntersection(input0:Guid seq, input1:Guid seq, [<OPT;DEF(true)>]delete_input:bool) : Guid seq =
        Surface.booleanIntersection delete_input input1 input0

    ///<summary>Performs a boolean union operation on a set of input surfaces and
    ///    polysurfaces. For more details, see the BooleanUnion command in the
    ///    Rhino help file</summary>
    ///<param name="input">(Guid seq) list of surfaces to union</param>
    ///<param name="delete_input">(bool) Optional, Default Value:True, delete all input objects</param>
    ///
    ///<returns>(Guid seq) of identifiers of newly created objects on success
    ///        </returns>
    static member BooleanUnion(input:Guid seq, [<OPT;DEF(true)>]delete_input:bool) : Guid seq =
        Surface.booleanUnion delete_input input

    ///<summary>Returns the point on a surface or polysurface that is closest to a test
    ///    point. This function works on both untrimmed and trimmed surfaces.</summary>
    ///<param name="object_id">(Guid) The object's identifier.</param>
    ///<param name="point">(Point3d) The test, or sampling point.</param>
    ///
    ///<returns>(Point3d * (float * float) * (float * float) * Vector3d) of closest point information . The list will
    ///        contain the following information:
    ///        Element     Type             Description
    ///          0        Point3d          The 3-D point at the parameter value of the
    ///                                    closest point.
    ///          1        (U, V)           Parameter values of closest point. Note, V
    ///                                    is 0 if the component index type is brep_edge
    ///                                    or brep_vertex.
    ///          2        (type, index)    The type and index of the brep component that
    ///                                    contains the closest point. Possible types are
    ///                                    brep_face, brep_edge or brep_vertex.
    ///          3        Vector3d         The normal to the brep_face, or the tangent
    ///                                    to the brep_edge.</returns>
    static member BrepClosestPoint(object_id:Guid, point:Point3d) : Point3d * (float * float) * (float * float) * Vector3d =
        Surface.brepClosestPoint point object_id

    ///<summary>Caps planar holes in a surface or polysurface</summary>
    ///<param name="surface_id">(Guid) The identifier of the surface or polysurface to cap.</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CapPlanarHoles(surface_id:Guid) : bool =
        Surface.capPlanarHoles surface_id

    ///<summary>Duplicates the edge curves of a surface or polysurface. For more
    ///    information, see the Rhino help file for information on the DupEdge
    ///    command.</summary>
    ///<param name="object_id">(Guid) The identifier of the surface or polysurface object.</param>
    ///<param name="select">(bool) Optional, Default Value:False, Select the duplicated edge curves. The default is not to select (False).</param>
    ///
    ///<returns>(Guid seq) identifying the newly created curve objects .</returns>
    static member DuplicateEdgeCurves(object_id:Guid, [<OPT;DEF(false)>]select:bool) : Guid seq =
        Surface.duplicateEdgeCurves select object_id

    ///<summary>Create curves that duplicate a surface or polysurface border</summary>
    ///<param name="surface_id">(Guid) identifier of a surface</param>
    ///<param name="type">(int) Optional, Default Value:0, the border curves to return
    ///         0=both exterior and interior,
    ///         1=exterior
    ///         2=interior</param>
    ///
    ///<returns>(Guid seq) list of curve ids on success</returns>
    static member DuplicateSurfaceBorder(surface_id:Guid, [<OPT;DEF(0)>]type:int) : Guid seq =
        Surface.duplicateSurfaceBorder type surface_id

    ///<summary>Evaluates a surface at a U,V parameter</summary>
    ///<param name="surface_id">(Guid) the object's identifier.</param>
    ///<param name="u">(float * float) u of 'u, v parameters to evaluate.' (FIXME 0)</param>
    ///<param name="v">(float * float) v of 'u, v parameters to evaluate.' (FIXME 0)</param>
    ///
    ///<returns>(Point3d) a 3-D point</returns>
    static member EvaluateSurface(surface_id:Guid, u:float * float, v:float * float) : Point3d =
        Surface.evaluateSurface v u surface_id

    ///<summary>Lengthens an untrimmed surface object</summary>
    ///<param name="surface_id">(Guid) identifier of a surface</param>
    ///<param name="parameter">(float * float) tuple of two values definfing the U,V parameter to evaluate.
    ///        The surface edge closest to the U,V parameter will be the edge that is
    ///        extended</param>
    ///<param name="length">(float) amount to extend to surface</param>
    ///<param name="smooth">(bool) Optional, Default Value:True, If True, the surface is extended smoothly curving from the
    ///        edge. If False, the surface is extended in a straight line from the edge</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ExtendSurface(surface_id:Guid, parameter:float * float, length:float, [<OPT;DEF(true)>]smooth:bool) : bool =
        Surface.extendSurface smooth length parameter surface_id

    ///<summary>Explodes, or unjoins, one or more polysurface objects. Polysurfaces
    ///    will be exploded into separate surfaces</summary>
    ///<param name="object_ids">(Guid seq) identifiers of polysurfaces to explode</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, delete input objects after exploding</param>
    ///
    ///<returns>(Guid seq) of identifiers of exploded pieces on success</returns>
    static member ExplodePolysurfaces(object_ids:Guid seq, [<OPT;DEF(false)>]delete_input:bool) : Guid seq =
        Surface.explodePolysurfaces delete_input object_ids

    ///<summary>Extracts isoparametric curves from a surface</summary>
    ///<param name="surface_id">(Guid) identifier of a surface</param>
    ///<param name="parameter">(float * float) u,v parameter of the surface to evaluate</param>
    ///<param name="direction">(float) Direction to evaluate
    ///        0 = u
    ///        1 = v
    ///        2 = both</param>
    ///
    ///<returns>(Guid seq) of curve ids on success</returns>
    static member ExtractIsoCurve(surface_id:Guid, parameter:float * float, direction:float) : Guid seq =
        Surface.extractIsoCurve direction parameter surface_id

    ///<summary>Separates or copies a surface or a copy of a surface from a polysurface</summary>
    ///<param name="object_id">(Guid) polysurface identifier</param>
    ///<param name="face_indices">(float seq) one or more numbers representing faces</param>
    ///<param name="copy">(bool) Optional, Default Value:False, If True the faces are copied. If False, the faces are extracted</param>
    ///
    ///<returns>(Guid seq) identifiers of extracted surface objects on success</returns>
    static member ExtractSurface(object_id:Guid, face_indices:float seq, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        Surface.extractSurface copy face_indices object_id

    ///<summary>Creates a surface by extruding a curve along a path</summary>
    ///<param name="curve_id">(Guid) identifier of the curve to extrude</param>
    ///<param name="path_id">(Guid) identifier of the path curve</param>
    ///
    ///<returns>(Guid) identifier of new surface on success</returns>
    static member ExtrudeCurve(curve_id:Guid, path_id:Guid) : Guid =
        Surface.extrudeCurve path_id curve_id

    ///<summary>Creates a surface by extruding a curve to a point</summary>
    ///<param name="curve_id">(Guid) identifier of the curve to extrude</param>
    ///<param name="point">(Point3d) 3D point</param>
    ///
    ///<returns>(Guid) identifier of new surface on success</returns>
    static member ExtrudeCurvePoint(curve_id:Guid, point:Point3d) : Guid =
        Surface.extrudeCurvePoint point curve_id

    ///<summary>Create surface by extruding a curve along two points that define a line</summary>
    ///<param name="curve_id">(Guid) identifier of the curve to extrude</param>
    ///<param name="start_point">(Point3d) start point of '3D points that specify distance and direction' (FIXME 0)</param>
    ///<param name="end_point">(Point3d) end point of '3D points that specify distance and direction' (FIXME 0)</param>
    ///
    ///<returns>(Guid) identifier of new surface on success</returns>
    static member ExtrudeCurveStraight(curve_id:Guid, start_point:Point3d, end_point:Point3d) : Guid =
        Surface.extrudeCurveStraight end_point start_point curve_id

    ///<summary>Create surface by extruding along a path curve</summary>
    ///<param name="surface">(Guid) identifier of the surface to extrude</param>
    ///<param name="curve">(Guid) identifier of the path curve</param>
    ///<param name="cap">(bool) Optional, Default Value:True, extrusion is capped at both ends</param>
    ///
    ///<returns>(Guid) identifier of new surface on success</returns>
    static member ExtrudeSurface(surface:Guid, curve:Guid, [<OPT;DEF(true)>]cap:bool) : Guid =
        Surface.extrudeSurface cap curve surface

    ///<summary>Create constant radius rolling ball fillets between two surfaces. Note,
    ///    this function does not trim the original surfaces of the fillets</summary>
    ///<param name="surface0">(Guid) surface0 of 'identifiers of first and second surface' (FIXME 0)</param>
    ///<param name="surface1">(Guid) surface1 of 'identifiers of first and second surface' (FIXME 0)</param>
    ///<param name="radius">(float) a positive fillet radius</param>
    ///<param name="uvparam0">(float * float) Optional, Default Value:None, a u,v surface parameter of surface0 near where the fillet
    ///        is expected to hit the surface</param>
    ///<param name="uvparam1">(float * float) Optional, Default Value:None, same as uvparam0, but for surface1</param>
    ///
    ///<returns>(Guid) ids of surfaces created on success</returns>
    static member FilletSurfaces(surface0:Guid, surface1:Guid, radius:float, [<OPT;DEF(null)>]uvparam0:float * float, [<OPT;DEF(null)>]uvparam1:float * float) : Guid =
        Surface.filletSurfaces uvparam1 uvparam0 radius surface1 surface0

    ///<summary>Intersects a brep object with another brep object. Note, unlike the
    ///    SurfaceSurfaceIntersection function this function works on trimmed surfaces.</summary>
    ///<param name="brep1">(Guid) identifier of first brep object</param>
    ///<param name="brep2">(Guid) identifier of second brep object</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, Distance tolerance at segment midpoints. If omitted,
    ///                  the current absolute tolerance is used.</param>
    ///
    ///<returns>(Guid seq) identifying the newly created intersection curve and point objects .</returns>
    static member IntersectBreps(brep1:Guid, brep2:Guid, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        Surface.intersectBreps tolerance brep2 brep1

    ///<summary>Calculates intersections of two spheres</summary>
    ///<param name="sphere_plane0">(Plane) an equatorial plane of the first sphere. The origin of the
    ///        plane will be the center point of the sphere</param>
    ///<param name="sphere_radius0">(float) radius of the first sphere</param>
    ///<param name="sphere_plane1">(Plane) plane for second sphere</param>
    ///<param name="sphere_radius1">(float) radius for second sphere</param>
    ///
    ///<returns>(float * Point3d * float) of intersection results
    ///        [0] = type of intersection (0=point, 1=circle, 2=spheres are identical)
    ///        [1] = Point of intersection or plane of circle intersection
    ///        [2] = radius of circle if circle intersection</returns>
    static member IntersectSpheres(sphere_plane0:Plane, sphere_radius0:float, sphere_plane1:Plane, sphere_radius1:float) : float * Point3d * float =
        Surface.intersectSpheres sphere_radius1 sphere_plane1 sphere_radius0 sphere_plane0

    ///<summary>Verifies an object is a Brep, or a boundary representation model, object.</summary>
    ///<param name="object_id">(Guid) The object's identifier.</param>
    ///
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsBrep(object_id:Guid) : bool =
        Surface.isBrep object_id

    ///<summary>Determines if a surface is a portion of a cone</summary>
    ///<param name="object_id">(Guid) the surface object's identifier</param>
    ///
    ///<returns>(bool) True , otherwise False</returns>
    static member IsCone(object_id:Guid) : bool =
        Surface.isCone object_id

    ///<summary>Determines if a surface is a portion of a cone</summary>
    ///<param name="object_id">(Guid) the cylinder object's identifier</param>
    ///
    ///<returns>(bool) True , otherwise False</returns>
    static member IsCylinder(object_id:Guid) : bool =
        Surface.isCylinder object_id

    ///<summary>Verifies an object is a plane surface. Plane surfaces can be created by
    ///    the Plane command. Note, a plane surface is not a planar NURBS surface</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True , otherwise False</returns>
    static member IsPlaneSurface(object_id:Guid) : bool =
        Surface.isPlaneSurface object_id

    ///<summary>Verifies that a point is inside a closed surface or polysurface</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///<param name="point">(Point3d) The test, or sampling point</param>
    ///<param name="strictly_in">(bool) Optional, Default Value:False, If true, the test point must be inside by at least tolerance</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, distance tolerance used for intersection and determining
    ///        strict inclusion. If omitted, Rhino's internal tolerance is used</param>
    ///
    ///<returns>(bool) True , otherwise False</returns>
    static member IsPointInSurface(object_id:Guid, point:Point3d, [<OPT;DEF(false)>]strictly_in:bool, [<OPT;DEF(null)>]tolerance:float) : bool =
        Surface.isPointInSurface tolerance strictly_in point object_id

    ///<summary>Verifies that a point lies on a surface</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///<param name="point">(Point3d) The test, or sampling point</param>
    ///
    ///<returns>(bool) True , otherwise False</returns>
    static member IsPointOnSurface(object_id:Guid, point:Point3d) : bool =
        Surface.isPointOnSurface point object_id

    ///<summary>Verifies an object is a polysurface. Polysurfaces consist of two or more
    ///    surfaces joined together. If the polysurface fully encloses a volume, it is
    ///    considered a solid.</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True is successful, otherwise False</returns>
    static member IsPolysurface(object_id:Guid) : bool =
        Surface.isPolysurface object_id

    ///<summary>Verifies a polysurface object is closed. If the polysurface fully encloses
    ///    a volume, it is considered a solid.</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True is successful, otherwise False</returns>
    static member IsPolysurfaceClosed(object_id:Guid) : bool =
        Surface.isPolysurfaceClosed object_id

    ///<summary>Determines if a surface is a portion of a sphere</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(bool) True , otherwise False</returns>
    static member IsSphere(object_id:Guid) : bool =
        Surface.isSphere object_id

    ///<summary>Verifies an object is a surface. Brep objects with only one face are
    ///    also considered surfaces.</summary>
    ///<param name="object_id">(Guid) the object's identifier.</param>
    ///
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsSurface(object_id:Guid) : bool =
        Surface.isSurface object_id

    ///<summary>Verifies a surface object is closed in the specified direction.  If the
    ///    surface fully encloses a volume, it is considered a solid</summary>
    ///<param name="surface_id">(Guid) identifier of a surface</param>
    ///<param name="direction">(float) 0=U direction check, 1=V direction check</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsSurfaceClosed(surface_id:Guid, direction:float) : bool =
        Surface.isSurfaceClosed direction surface_id

    ///<summary>Verifies a surface object is periodic in the specified direction.</summary>
    ///<param name="surface_id">(Guid) identifier of a surface</param>
    ///<param name="direction">(float) 0=U direction check, 1=V direction check</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsSurfacePeriodic(surface_id:Guid, direction:float) : bool =
        Surface.isSurfacePeriodic direction surface_id

    ///<summary>Verifies a surface object is planar</summary>
    ///<param name="surface_id">(Guid) identifier of a surface</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, tolerance used when checked. If omitted, the current absolute
    ///        tolerance is used</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsSurfacePlanar(surface_id:Guid, [<OPT;DEF(null)>]tolerance:float) : bool =
        Surface.isSurfacePlanar tolerance surface_id

    ///<summary>Verifies a surface object is rational</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(bool) True , otherwise False</returns>
    static member IsSurfaceRational(surface_id:Guid) : bool =
        Surface.isSurfaceRational surface_id

    ///<summary>Verifies a surface object is singular in the specified direction.
    ///    Surfaces are considered singular if a side collapses to a point.</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="direction">(float) 0=south
    ///        1=east
    ///        2=north
    ///        3=west</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsSurfaceSingular(surface_id:Guid, direction:float) : bool =
        Surface.isSurfaceSingular direction surface_id

    ///<summary>Verifies a surface object has been trimmed</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsSurfaceTrimmed(surface_id:Guid) : bool =
        Surface.isSurfaceTrimmed surface_id

    ///<summary>Determines if a surface is a portion of a torus</summary>
    ///<param name="surface_id">(Guid) the surface object's identifier</param>
    ///
    ///<returns>(bool) True , otherwise False</returns>
    static member IsTorus(surface_id:Guid) : bool =
        Surface.isTorus surface_id

    ///<summary>Gets the sphere definition from a surface, if possible.</summary>
    ///<param name="surface_id">(Guid) the identifier of the surface object</param>
    ///
    ///<returns>(Plane * float) The equatorial plane of the sphere, and its radius.</returns>
    static member SurfaceSphere(surface_id:Guid) : Plane * float =
        Surface.surfaceSphere surface_id

    ///<summary>Joins two or more surface or polysurface objects together to form one
    ///    polysurface object</summary>
    ///<param name="object_ids">(Guid seq) list of object identifiers</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, Delete the original surfaces</param>
    ///
    ///<returns>(Guid) identifier of newly created object on success</returns>
    static member JoinSurfaces(object_ids:Guid seq, [<OPT;DEF(false)>]delete_input:bool) : Guid =
        Surface.joinSurfaces delete_input object_ids

    ///<summary>Offsets a trimmed or untrimmed surface by a distance. The offset surface
    ///    will be added to Rhino.</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="distance">(float) the distance to offset</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, The offset tolerance. Use 0.0 to make a loose offset. Otherwise, the
    ///        document's absolute tolerance is usually sufficient.</param>
    ///<param name="both_sides">(bool) Optional, Default Value:False, Offset to both sides of the input surface</param>
    ///<param name="create_solid">(bool) Optional, Default Value:False, Make a solid object</param>
    ///
    ///<returns>(Guid) identifier of the new object</returns>
    static member OffsetSurface(surface_id:Guid, distance:float, [<OPT;DEF(null)>]tolerance:float, [<OPT;DEF(false)>]both_sides:bool, [<OPT;DEF(false)>]create_solid:bool) : Guid =
        Surface.offsetSurface create_solid both_sides tolerance distance surface_id

    ///<summary>Pulls a curve object to a surface object</summary>
    ///<param name="surface">(Guid) the surface's identifier</param>
    ///<param name="curve">(Guid) the curve's identifier</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, should the input items be deleted</param>
    ///
    ///<returns>(Guid seq) of new curves</returns>
    static member PullCurve(surface:Guid, curve:Guid, [<OPT;DEF(false)>]delete_input:bool) : Guid seq =
        Surface.pullCurve delete_input curve surface

    ///<summary>Rebuilds a surface to a given degree and control point count. For more
    ///    information see the Rhino help file for the Rebuild command</summary>
    ///<param name="object_id">(Guid) the surface's identifier</param>
    ///<param name="degree">(int * int) Optional, Default Value:3*3, two numbers that identify surface degree in both U and V directions</param>
    ///<param name="pointcount">(int * int) Optional, Default Value:10*10, two numbers that identify the surface point count in both the U and V directions</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RebuildSurface(object_id:Guid, [<OPT;DEF(3*3)>]degree:int * int, [<OPT;DEF(10*10)>]pointcount:int * int) : bool =
        Surface.rebuildSurface pointcount degree object_id

    ///<summary>Deletes a knot from a surface object.</summary>
    ///<param name="surface">(Guid) The reference of the surface object</param>
    ///<param name="uv_parameter">(float * float) ): An indexable item containing a U,V parameter on the surface. List, tuples and UVIntervals will work.
    ///        Note, if the parameter is not equal to one of the existing knots, then the knot closest to the specified parameter will be removed.</param>
    ///<param name="v_direction">(bool) if True, or 1, the V direction will be addressed. If False, or 0, the U direction.</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RemoveSurfaceKnot(surface:Guid, uv_parameter:float * float, v_direction:bool) : bool =
        Surface.removeSurfaceKnot v_direction uv_parameter surface

    ///<summary>Reverses U or V directions of a surface, or swaps (transposes) U and V
    ///    directions.</summary>
    ///<param name="surface_id">(Guid) identifier of a surface object</param>
    ///<param name="direction">(float) as a bit coded flag to swap
    ///            1 = reverse U
    ///            2 = reverse V
    ///            4 = transpose U and V (values can be combined)</param>
    ///
    ///<returns>(bool) indicating success or failure</returns>
    static member ReverseSurface(surface_id:Guid, direction:float) : bool =
        Surface.reverseSurface direction surface_id

    ///<summary>Shoots a ray at a collection of surfaces</summary>
    ///<param name="surface_ids">(Guid seq) one of more surface identifiers</param>
    ///<param name="start_point">(Point3d) starting point of the ray</param>
    ///<param name="direction">(Vector3d) vector identifying the direction of the ray</param>
    ///<param name="reflections">(float) Optional, Default Value:10, the maximum number of times the ray will be reflected</param>
    ///
    ///<returns>(Point3d seq) of reflection points on success</returns>
    static member ShootRay(surface_ids:Guid seq, start_point:Point3d, direction:Vector3d, [<OPT;DEF(10)>]reflections:float) : Point3d seq =
        Surface.shootRay reflections direction start_point surface_ids

    ///<summary>Creates the shortest possible curve(geodesic) between two points on a
    ///    surface. For more details, see the ShortPath command in Rhino help</summary>
    ///<param name="surface_id">(Guid) identifier of a surface</param>
    ///<param name="start_point">(Point3d) start point of 'start/end points of the short curve' (FIXME 0)</param>
    ///<param name="end_point">(Point3d) end point of 'start/end points of the short curve' (FIXME 0)</param>
    ///
    ///<returns>(Guid) identifier of the new surface on success</returns>
    static member ShortPath(surface_id:Guid, start_point:Point3d, end_point:Point3d) : Guid =
        Surface.shortPath end_point start_point surface_id

    ///<summary>Splits a brep</summary>
    ///<param name="brep_id">(Guid) identifier of the brep to split</param>
    ///<param name="cutter_id">(Guid) identifier of the brep to split with</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, delete input breps</param>
    ///
    ///<returns>(Guid seq) identifiers of split pieces on success</returns>
    static member SplitBrep(brep_id:Guid, cutter_id:Guid, [<OPT;DEF(false)>]delete_input:bool) : Guid seq =
        Surface.splitBrep delete_input cutter_id brep_id

    ///<summary>Calculate the area of a surface or polysurface object. The results are
    ///    based on the current drawing units</summary>
    ///<param name="object_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(float * float) of area information on success (area, absolute error bound)</returns>
    static member SurfaceArea(object_id:Guid) : float * float =
        Surface.surfaceArea object_id

    ///<summary>Calculates the area centroid of a surface or polysurface</summary>
    ///<param name="object_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(Point3d * float * float * float) Area centroid information (Area Centroid, Error bound in X, Y, Z)</returns>
    static member SurfaceAreaCentroid(object_id:Guid) : Point3d * float * float * float =
        Surface.surfaceAreaCentroid object_id

    ///<summary>Calculates area moments of inertia of a surface or polysurface object.
    ///    See the Rhino help for "Mass Properties calculation details"</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(float seq) of moments and error bounds in tuple(X, Y, Z) - see help topic
    ///        Index   Description
    ///        [0]     First Moments.
    ///        [1]     The absolute (+/-) error bound for the First Moments.
    ///        [2]     Second Moments.
    ///        [3]     The absolute (+/-) error bound for the Second Moments.
    ///        [4]     Product Moments.
    ///        [5]     The absolute (+/-) error bound for the Product Moments.
    ///        [6]     Area Moments of Inertia about the World Coordinate Axes.
    ///        [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
    ///        [8]     Area Radii of Gyration about the World Coordinate Axes.
    ///        [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///        [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///        [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///        [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///        [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.</returns>
    static member SurfaceAreaMoments(surface_id:Guid) : float seq =
        Surface.surfaceAreaMoments surface_id

    ///<summary>Returns U,V parameters of point on a surface that is closest to a test point</summary>
    ///<param name="surface_id">(Guid) identifier of a surface object</param>
    ///<param name="test_point">(Point3d) sampling point</param>
    ///
    ///<returns>(float * float) The U,V parameters of the closest point on the surface .</returns>
    static member SurfaceClosestPoint(surface_id:Guid, test_point:Point3d) : float * float =
        Surface.surfaceClosestPoint test_point surface_id

    ///<summary>Returns the definition of a surface cone</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(Plane * float * float) containing the definition of the cone 
    ///        [0]   the plane of the cone. The apex of the cone is at the
    ///              plane's origin and the axis of the cone is the plane's z-axis
    ///        [1]   the height of the cone
    ///        [2]   the radius of the cone</returns>
    static member SurfaceCone(surface_id:Guid) : Plane * float * float =
        Surface.surfaceCone surface_id

    ///<summary>Returns the curvature of a surface at a U,V parameter. See Rhino help
    ///    for details of surface curvature</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="parameter">(float * float) u,v parameter</param>
    ///
    ///<returns>(Point3d * Vector3d * float * float * float * float * float) of curvature information
    ///        [0]   point at specified U,V parameter
    ///        [1]   normal direction
    ///        [2]   maximum principal curvature
    ///        [3]   maximum principal curvature direction
    ///        [4]   minimum principal curvature
    ///        [5]   minimum principal curvature direction
    ///        [6]   gaussian curvature
    ///        [7]   mean curvature</returns>
    static member SurfaceCurvature(surface_id:Guid, parameter:float * float) : Point3d * Vector3d * float * float * float * float * float =
        Surface.surfaceCurvature parameter surface_id

    ///<summary>Returns the definition of a cylinder surface</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(Plane * float * float) of the cylinder plane, height, radius on success</returns>
    static member SurfaceCylinder(surface_id:Guid) : Plane * float * float =
        Surface.surfaceCylinder surface_id

    ///<summary>Returns the domain of a surface object in the specified direction.</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="direction">(float) domain direction 0 = U, or 1 = V</param>
    ///
    ///<returns>(float * float) containing the domain interval in the specified direction</returns>
    static member SurfaceDomain(surface_id:Guid, direction:float) : float * float =
        Surface.surfaceDomain direction surface_id

    ///<summary>A general purpose surface evaluator</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="parameter">(float * float) u,v parameter to evaluate</param>
    ///<param name="derivative">(float) number of derivatives to evaluate</param>
    ///
    ///<returns>(Point3d seq) list length (derivative+1)*(derivative+2)/2 .  The elements are as follows:
    ///      Element  Description
    ///      [0]      The 3-D point.
    ///      [1]      The first derivative.
    ///      [2]      The first derivative.
    ///      [3]      The second derivative.
    ///      [4]      The second derivative.
    ///      [5]      The second derivative.
    ///      [6]      etc...
    ///    None: If not successful, or on error.</returns>
    static member SurfaceEvaluate(surface_id:Guid, parameter:float * float, derivative:float) : Point3d seq =
        Surface.surfaceEvaluate derivative parameter surface_id

    ///<summary>Returns a plane based on the normal, u, and v directions at a surface
    ///    U,V parameter</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="uv_parameter">(float * float) u,v parameter to evaluate</param>
    ///
    ///<returns>(Plane) plane on success</returns>
    static member SurfaceFrame(surface_id:Guid, uv_parameter:float * float) : Plane =
        Surface.surfaceFrame uv_parameter surface_id

    ///<summary>Returns the control point count of a surface
    ///      surface_id = the surface's identifier</summary>
    ///<param name="surface_id">(Guid) the surface object's identifier</param>
    ///
    ///<returns>(int * int) a list containing (U count, V count) on success</returns>
    static member SurfaceKnotCount(surface_id:Guid) : int * int =
        Surface.surfaceKnotCount surface_id

    ///<summary>Returns the knots, or knot vector, of a surface object.</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(float * float) knot values of the surface . The list will
    ///      contain the following information:
    ///      Element   Description
    ///        [0]     Knot vector in U direction
    ///        [1]     Knot vector in V direction
    ///      None: if not successful, or on error.</returns>
    static member SurfaceKnots(surface_id:Guid) : float * float =
        Surface.surfaceKnots surface_id

    ///<summary>Returns 3D vector that is the normal to a surface at a parameter</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="uv_parameter">(float * float) the uv parameter to evaluate</param>
    ///
    ///<returns>(Vector3d) Normal vector on success</returns>
    static member SurfaceNormal(surface_id:Guid, uv_parameter:float * float) : Vector3d =
        Surface.surfaceNormal uv_parameter surface_id

    ///<summary>Converts surface parameter to a normalized surface parameter; one that
    ///    ranges between 0.0 and 1.0 in both the U and V directions</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="parameter">(float * float) the surface parameter to convert</param>
    ///
    ///<returns>(float * float) normalized surface parameter</returns>
    static member SurfaceNormalizedParameter(surface_id:Guid, parameter:float * float) : float * float =
        Surface.surfaceNormalizedParameter parameter surface_id

    ///<summary>Converts normalized surface parameter to a surface parameter; or
    ///    within the surface's domain</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="parameter">(float * float) the normalized parameter to convert</param>
    ///
    ///<returns>(float * float) surface parameter on success</returns>
    static member SurfaceParameter(surface_id:Guid, parameter:float * float) : float * float =
        Surface.surfaceParameter parameter surface_id

    ///<summary>Returns the control point count of a surface
    ///      surface_id = the surface's identifier</summary>
    ///<param name="surface_id">(Guid) the surface object's identifier</param>
    ///
    ///<returns>(int * int) THe number of control points in UV direction. (U count, V count)</returns>
    static member SurfacePointCount(surface_id:Guid) : int * int =
        Surface.surfacePointCount surface_id

    ///<summary>Returns the control points, or control vertices, of a surface object</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///<param name="return_all">(bool) Optional, Default Value:True, If True all surface edit points are returned. If False,
    ///        the function will return surface edit points based on whether or not
    ///        the surface is closed or periodic</param>
    ///
    ///<returns>(Point3d seq) the control points</returns>
    static member SurfacePoints(surface_id:Guid, [<OPT;DEF(true)>]return_all:bool) : Point3d seq =
        Surface.surfacePoints return_all surface_id

    ///<summary>Returns the definition of a surface torus</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(Plane * float * float) containing the definition of the torus 
    ///        [0]   the base plane of the torus
    ///        [1]   the major radius of the torus
    ///        [2]   the minor radius of the torus</returns>
    static member SurfaceTorus(surface_id:Guid) : Plane * float * float =
        Surface.surfaceTorus surface_id

    ///<summary>Calculates volume of a closed surface or polysurface</summary>
    ///<param name="object_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(list[number, tuple(X, Y, Z)]) volume data returned (Volume, Error bound) on success</returns>
    static member SurfaceVolume(object_id:Guid) : list[number, tuple(X, Y, Z)] =
        Surface.surfaceVolume object_id

    ///<summary>Calculates volume centroid of a closed surface or polysurface</summary>
    ///<param name="object_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(list[point, tuple(X, Y, Z)]) volume data returned (Volume Centriod, Error bound) on success</returns>
    static member SurfaceVolumeCentroid(object_id:Guid) : list[point, tuple(X, Y, Z)] =
        Surface.surfaceVolumeCentroid object_id

    ///<summary>Calculates volume moments of inertia of a surface or polysurface object.
    ///    For more information, see Rhino help for "Mass Properties calculation details"</summary>
    ///<param name="surface_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(float seq) of moments and error bounds in tuple(X, Y, Z) - see help topic
    ///        Index   Description
    ///        [0]     First Moments.
    ///        [1]     The absolute (+/-) error bound for the First Moments.
    ///        [2]     Second Moments.
    ///        [3]     The absolute (+/-) error bound for the Second Moments.
    ///        [4]     Product Moments.
    ///        [5]     The absolute (+/-) error bound for the Product Moments.
    ///        [6]     Area Moments of Inertia about the World Coordinate Axes.
    ///        [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
    ///        [8]     Area Radii of Gyration about the World Coordinate Axes.
    ///        [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///        [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///        [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///        [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///        [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.</returns>
    static member SurfaceVolumeMoments(surface_id:Guid) : float seq =
        Surface.surfaceVolumeMoments surface_id

    ///<summary>Returns list of weight values assigned to the control points of a surface.
    ///    The number of weights returned will be equal to the number of control points
    ///    in the U and V directions.</summary>
    ///<param name="object_id">(Guid) the surface's identifier</param>
    ///
    ///<returns>(float seq) point weights.</returns>
    static member SurfaceWeights(object_id:Guid) : float seq =
        Surface.surfaceWeights object_id

    ///<summary>Trims a surface using an oriented cutter</summary>
    ///<param name="object_id">(Guid) surface or polysurface identifier</param>
    ///<param name="cutter">(Plane) surface, polysurface, or plane performing the trim</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, trimming tolerance. If omitted, the document's absolute
    ///        tolerance is used</param>
    ///
    ///<returns>(Guid seq) identifiers of retained components on success</returns>
    static member TrimBrep(object_id:Guid, cutter:Plane, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        Surface.trimBrep tolerance cutter object_id

    ///<summary>Remove portions of the surface outside of the specified interval</summary>
    ///<param name="surface_id">(Guid) surface identifier</param>
    ///<param name="direction">(float) 0(U), 1(V), or 2(U and V)</param>
    ///<param name="interval">(float*float) sub section of the surface to keep.
    ///        If both U and V then a list or tuple of 2 intervals</param>
    ///<param name="delete_input">(bool) Optional, Default Value:False, should the input surface be deleted</param>
    ///
    ///<returns>(Guid) new surface identifier on success</returns>
    static member TrimSurface(surface_id:Guid, direction:float, interval:float*float, [<OPT;DEF(false)>]delete_input:bool) : Guid =
        Surface.trimSurface delete_input interval direction surface_id

    ///<summary>Changes the degree of a surface object.  For more information see the Rhino help file for the ChangeDegree command.</summary>
    ///<param name="object_id">(Guid) the object's identifier.</param>
    ///<param name="degree">(int * int) two integers, specifying the degrees for the U  V directions</param>
    ///
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member ChangeSurfaceDegree(object_id:Guid, degree:int * int) : bool =
        Surface.changeSurfaceDegree degree object_id

    ///<summary>Closes a currently open toolbar collection</summary>
    ///<param name="name">(string) name of a currently open toolbar collection</param>
    ///<param name="prompt">(bool) Optional, Default Value:False, if True, user will be prompted to save the collection file
    ///        if it has been modified prior to closing</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CloseToolbarCollection(name:string, [<OPT;DEF(false)>]prompt:bool) : bool =
        Toolbar.closeToolbarCollection prompt name

    ///<summary>Hides a previously visible toolbar group in an open toolbar collection</summary>
    ///<param name="name">(string) name of a currently open toolbar file</param>
    ///<param name="toolbar_group">(string) name of a toolbar group to hide</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member HideToolbar(name:string, toolbar_group:string) : bool =
        Toolbar.hideToolbar toolbar_group name

    ///<summary>Verifies a toolbar (or toolbar group) exists in an open collection file</summary>
    ///<param name="name">(string) name of a currently open toolbar file</param>
    ///<param name="toolbar">(string) name of a toolbar group</param>
    ///<param name="group">(bool) Optional, Default Value:False, if toolbar parameter is referring to a toolbar group</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbar(name:string, toolbar:string, [<OPT;DEF(false)>]group:bool) : bool =
        Toolbar.isToolbar group toolbar name

    ///<summary>Verifies that a toolbar collection is open</summary>
    ///<param name="file">(string) full path to a toolbar collection file</param>
    ///
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member IsToolbarCollection(file:string) : string =
        Toolbar.isToolbarCollection file

    ///<summary>Verifies that a toolbar group in an open toolbar collection is visible</summary>
    ///<param name="name">(string) name of a currently open toolbar file</param>
    ///<param name="toolbar_group">(string) name of a toolbar group</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbarDocked(name:string, toolbar_group:string) : bool =
        Toolbar.isToolbarDocked toolbar_group name

    ///<summary>Verifies that a toolbar group in an open toolbar collection is visible</summary>
    ///<param name="name">(string) name of a currently open toolbar file</param>
    ///<param name="toolbar_group">(string) name of a toolbar group</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbarVisible(name:string, toolbar_group:string) : bool =
        Toolbar.isToolbarVisible toolbar_group name

    ///<summary>Opens a toolbar collection file</summary>
    ///<param name="file">(string) full path to the collection file</param>
    ///
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member OpenToolbarCollection(file:string) : string =
        Toolbar.openToolbarCollection file

    ///<summary>Saves an open toolbar collection to disk</summary>
    ///<param name="name">(string) name of a currently open toolbar file</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollection(name:string) : bool =
        Toolbar.saveToolbarCollection name

    ///<summary>Saves an open toolbar collection to a different disk file</summary>
    ///<param name="name">(string) name of a currently open toolbar file</param>
    ///<param name="file">(string) full path to file name to save to</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollectionAs(name:string, file:string) : bool =
        Toolbar.saveToolbarCollectionAs file name

    ///<summary>Shows a previously hidden toolbar group in an open toolbar collection</summary>
    ///<param name="name">(string) name of a currently open toolbar file</param>
    ///<param name="toolbar_group">(string) name of a toolbar group to show</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ShowToolbar(name:string, toolbar_group:string) : bool =
        Toolbar.showToolbar toolbar_group name

    ///<summary>Returns number of currently open toolbar collections</summary>
    ///
    ///<returns>(int) the number of currently open toolbar collections</returns>
    static member ToolbarCollectionCount() : int =
        Toolbar.toolbarCollectionCount ()

    ///<summary>Returns names of all currently open toolbar collections</summary>
    ///
    ///<returns>(string seq) the names of all currently open toolbar collections</returns>
    static member ToolbarCollectionNames() : string seq =
        Toolbar.toolbarCollectionNames ()

    ///<summary>Returns full path to a currently open toolbar collection file</summary>
    ///<param name="name">(string) name of currently open toolbar collection</param>
    ///
    ///<returns>(string) full path on success</returns>
    static member ToolbarCollectionPath(name:string) : string =
        Toolbar.toolbarCollectionPath name

    ///<summary>Returns the number of toolbars or groups in a currently open toolbar file</summary>
    ///<param name="name">(string) name of currently open toolbar collection</param>
    ///<param name="groups">(bool) Optional, Default Value:False, If true, return the number of toolbar groups in the file</param>
    ///
    ///<returns>(int) number of toolbars on success</returns>
    static member ToolbarCount(name:string, [<OPT;DEF(false)>]groups:bool) : int =
        Toolbar.toolbarCount groups name

    ///<summary>Returns the names of all toolbars (or toolbar groups) found in a
    ///    currently open toolbar file</summary>
    ///<param name="name">(string) name of currently open toolbar collection</param>
    ///<param name="groups">(bool) Optional, Default Value:False, If true, return the names of toolbar groups in the file</param>
    ///
    ///<returns>(string seq) names of all toolbars (or toolbar groups) on success</returns>
    static member ToolbarNames(name:string, [<OPT;DEF(false)>]groups:bool) : string seq =
        Toolbar.toolbarNames groups name

    ///<summary>Verifies a matrix is the identity matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsXformIdentity(xform:Transform) : bool =
        Transformation.isXformIdentity xform

    ///<summary>Verifies a matrix is a similarity transformation. A similarity
    ///    transformation can be broken into a sequence of dialations, translations,
    ///    rotations, and reflections</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///
    ///<returns>(bool) True if this transformation is an orientation preserving similarity, otherwise False.</returns>
    static member IsXformSimilarity(xform:Transform) : bool =
        Transformation.isXformSimilarity xform

    ///<summary>verifies that a matrix is a zero transformation matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsXformZero(xform:Transform) : bool =
        Transformation.isXformZero xform

    ///<summary>Returns a change of basis transformation matrix or None on error</summary>
    ///<param name="initial_plane">(Plane) the initial plane</param>
    ///<param name="final_plane">(Plane) the final plane</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformChangeBasis(initial_plane:Plane, final_plane:Plane) : Transform =
        Transformation.xformChangeBasis final_plane initial_plane

    ///<summary>Returns a change of basis transformation matrix of None on error</summary>
    ///<param name="x0">(Vector3d) x0 of 'initial basis' (FIXME 0)</param>
    ///<param name="y0">(Vector3d) y0 of 'initial basis' (FIXME 0)</param>
    ///<param name="z0">(Vector3d) z0 of 'initial basis' (FIXME 0)</param>
    ///<param name="x1">(Vector3d) x1 of 'final basis' (FIXME 0)</param>
    ///<param name="y1">(Vector3d) y1 of 'final basis' (FIXME 0)</param>
    ///<param name="z1">(Vector3d) z1 of 'final basis' (FIXME 0)</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformChangeBasis2(x0:Vector3d, y0:Vector3d, z0:Vector3d, x1:Vector3d, y1:Vector3d, z1:Vector3d) : Transform =
        Transformation.xformChangeBasis2 z1 y1 x1 z0 y0 x0

    ///<summary>Compares two transformation matrices</summary>
    ///<param name="xform1">(Transform) first matrix to compare</param>
    ///<param name="xform2">(Transform) second matrix to compare</param>
    ///
    ///<returns>(int) -1 if xform1<xform2
    ///         1 if xform1>xform2
    ///         0 if xform1=xform2</returns>
    static member XformCompare(xform1:Transform, xform2:Transform) : int =
        Transformation.xformCompare xform2 xform1

    ///<summary>Transform point from construction plane coordinates to world coordinates</summary>
    ///<param name="point">(Point3d) A 3D point in construction plane coordinates.</param>
    ///<param name="plane">(Plane) The construction plane</param>
    ///
    ///<returns>(Point3d) A 3D point in world coordinates</returns>
    static member XformCPlaneToWorld(point:Point3d, plane:Plane) : Point3d =
        Transformation.xformCPlaneToWorld plane point

    ///<summary>Returns the determinant of a transformation matrix. If the determinant
    ///    of a transformation matrix is 0, the matrix is said to be singular. Singular
    ///    matrices do not have inverses.</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///
    ///<returns>(float) The determinant</returns>
    static member XformDeterminant(xform:Transform) : float =
        Transformation.xformDeterminant xform

    ///<summary>Returns a diagonal transformation matrix. Diagonal matrices are 3x3 with
    ///    the bottom row [0,0,0,1]</summary>
    ///<param name="diagonal_value">(float) the diagonal value</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformDiagonal(diagonal_value:float) : Transform =
        Transformation.xformDiagonal diagonal_value

    ///<summary>returns the identity transformation matrix</summary>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformIdentity() : Transform =
        Transformation.xformIdentity ()

    ///<summary>Returns the inverse of a non-singular transformation matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///
    ///<returns>(Transform) The inverted 4x4 transformation matrix .</returns>
    static member XformInverse(xform:Transform) : Transform =
        Transformation.xformInverse xform

    ///<summary>Creates a mirror transformation matrix</summary>
    ///<param name="mirror_plane_point">(Point3d) point on the mirror plane</param>
    ///<param name="mirror_plane_normal">(Vector3d) a 3D vector that is normal to the mirror plane</param>
    ///
    ///<returns>(Transform) mirror Transform matrix</returns>
    static member XformMirror(mirror_plane_point:Point3d, mirror_plane_normal:Vector3d) : Transform =
        Transformation.xformMirror mirror_plane_normal mirror_plane_point

    ///<summary>Multiplies two transformation matrices, where result = xform1 * xform2</summary>
    ///<param name="xform1">(Transform) List or Rhino.Geometry.Transform.  The first 4x4 transformation matrix to multiply.</param>
    ///<param name="xform2">(Transform) List or Rhino.Geometry.Transform.  The second 4x4 transformation matrix to multiply.</param>
    ///
    ///<returns>(Transform) result transformation on success</returns>
    static member XformMultiply(xform1:Transform, xform2:Transform) : Transform =
        Transformation.xformMultiply xform2 xform1

    ///<summary>Returns a transformation matrix that projects to a plane.</summary>
    ///<param name="plane">(Plane) The plane to project to.</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformPlanarProjection(plane:Plane) : Transform =
        Transformation.xformPlanarProjection plane

    ///<summary>Returns a rotation transformation that maps initial_plane to final_plane.
    ///    The planes should be right hand orthonormal planes.</summary>
    ///<param name="initial_plane">(Plane) plane to rotate from</param>
    ///<param name="final_plane">(Plane) plane to rotate to</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation1(initial_plane:Plane, final_plane:Plane) : Transform =
        Transformation.xformRotation1 final_plane initial_plane

    ///<summary>Returns a rotation transformation around an axis</summary>
    ///<param name="angle_degrees">(int) rotation angle in degrees</param>
    ///<param name="rotation_axis">(Vector3d) rotation axis</param>
    ///<param name="center_point">(Point3d) rotation center</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation2(angle_degrees:int, rotation_axis:Vector3d, center_point:Point3d) : Transform =
        Transformation.xformRotation2 center_point rotation_axis angle_degrees

    ///<summary>Calculate the minimal transformation that rotates start_direction to
    ///    end_direction while fixing center_point</summary>
    ///<param name="start_direction">(Vector3d) start direction of '3d vectors' (FIXME 0)</param>
    ///<param name="end_direction">(Vector3d) end direction of '3d vectors' (FIXME 0)</param>
    ///<param name="center_point">(Point3d) the rotation center</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation3(start_direction:Vector3d, end_direction:Vector3d, center_point:Point3d) : Transform =
        Transformation.xformRotation3 center_point end_direction start_direction

    ///<summary>Returns a rotation transformation.</summary>
    ///<param name="x0">(Vector3d) x0 of 'Vectors defining the initial orthonormal frame' (FIXME 0)</param>
    ///<param name="y0">(Vector3d) y0 of 'Vectors defining the initial orthonormal frame' (FIXME 0)</param>
    ///<param name="z0">(Vector3d) z0 of 'Vectors defining the initial orthonormal frame' (FIXME 0)</param>
    ///<param name="x1">(Vector3d) x1 of 'Vectors defining the final orthonormal frame' (FIXME 0)</param>
    ///<param name="y1">(Vector3d) y1 of 'Vectors defining the final orthonormal frame' (FIXME 0)</param>
    ///<param name="z1">(Vector3d) z1 of 'Vectors defining the final orthonormal frame' (FIXME 0)</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation4(x0:Vector3d, y0:Vector3d, z0:Vector3d, x1:Vector3d, y1:Vector3d, z1:Vector3d) : Transform =
        Transformation.xformRotation4 z1 y1 x1 z0 y0 x0

    ///<summary>Creates a scale transformation</summary>
    ///<param name="scale">(float*float*float) single number, list of 3 numbers, Point3d, or Vector3d</param>
    ///<param name="point">(Point3d) Optional, Default Value:None, center of scale. If omitted, world origin is used</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix on success</returns>
    static member XformScale(scale:float*float*float, [<OPT;DEF(null)>]point:Point3d) : Transform =
        Transformation.xformScale point scale

    ///<summary>Transforms a point from either client-area coordinates of the specified view
    ///    or screen coordinates to world coordinates. The resulting coordinates are represented
    ///    as a 3-D point</summary>
    ///<param name="point">(Point3d) 2D point</param>
    ///<param name="view">(string) Optional, Default Value:None, title or identifier of a view. If omitted, the active view is used</param>
    ///<param name="screen_coordinates">(bool) Optional, Default Value:False, if False, point is in client-area coordinates. If True,
    ///      point is in screen-area coordinates</param>
    ///
    ///<returns>(Point3d) on success</returns>
    static member XformScreenToWorld(point:Point3d, [<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]screen_coordinates:bool) : Point3d =
        Transformation.xformScreenToWorld screen_coordinates view point

    ///<summary>Returns a shear transformation matrix</summary>
    ///<param name="plane">(Plane) plane[0] is the fixed point</param>
    ///<param name="x">(float) x of 'each axis scale factor' (FIXME 0)</param>
    ///<param name="y">(float) y of 'each axis scale factor' (FIXME 0)</param>
    ///<param name="z">(float) z of 'each axis scale factor' (FIXME 0)</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix on success</returns>
    static member XformShear(plane:Plane, x:float, y:float, z:float) : Transform =
        Transformation.xformShear z y x plane

    ///<summary>Creates a translation transformation matrix</summary>
    ///<param name="vector">(Vector3d) List of 3 numbers, Point3d, or Vector3d.  A 3-D translation vector.</param>
    ///
    ///<returns>(Transform) The 4x4 transformation matrix is successful, otherwise None</returns>
    static member XformTranslation(vector:Vector3d) : Transform =
        Transformation.xformTranslation vector

    ///<summary>Transforms a point from world coordinates to construction plane coordinates.</summary>
    ///<param name="point">(Point3d) A 3D point in world coordinates.</param>
    ///<param name="plane">(Plane) The construction plane</param>
    ///
    ///<returns>(Point3d) 3D point in construction plane coordinates</returns>
    static member XformWorldToCPlane(point:Point3d, plane:Plane) : Point3d =
        Transformation.xformWorldToCPlane plane point

    ///<summary>Transforms a point from world coordinates to either client-area coordinates of
    ///    the specified view or screen coordinates. The resulting coordinates are represented
    ///    as a 2D point</summary>
    ///<param name="point">(Point3d) 3D point in world coordinates</param>
    ///<param name="view">(string) Optional, Default Value:None, title or identifier of a view. If omitted, the active view is used</param>
    ///<param name="screen_coordinates">(bool) Optional, Default Value:False, if False, the function returns the results as
    ///        client-area coordinates. If True, the result is in screen-area coordinates</param>
    ///
    ///<returns>(Point3d) 2D point on success</returns>
    static member XformWorldToScreen(point:Point3d, [<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]screen_coordinates:bool) : Point3d =
        Transformation.xformWorldToScreen screen_coordinates view point

    ///<summary>Returns a zero transformation matrix</summary>
    ///
    ///<returns>(Transform) a zero transformation matrix</returns>
    static member XformZero() : Transform =
        Transformation.xformZero ()

    ///<summary>Removes user data strings from the current document</summary>
    ///<param name="section">(string) Optional, Default Value:None, section name. If omitted, all sections and their corresponding
    ///        entries are removed</param>
    ///<param name="entry">(string) Optional, Default Value:None, entry name. If omitted, all entries for section are removed</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteDocumentData([<OPT;DEF(null)>]section:string, [<OPT;DEF(null)>]entry:string) : bool =
        Userdata.deleteDocumentData entry section

    ///<summary>Returns the number of user data strings in the current document</summary>
    ///
    ///<returns>(int) the number of user data strings in the current document</returns>
    static member DocumentDataCount() : int =
        Userdata.documentDataCount ()

    ///<summary>Returns the number of user text strings in the current document</summary>
    ///
    ///<returns>(int) the number of user text strings in the current document</returns>
    static member DocumentUserTextCount() : int =
        Userdata.documentUserTextCount ()

    ///<summary>Verifies the current document contains user data</summary>
    ///
    ///<returns>(bool) True or False indicating the presence of Script user data</returns>
    static member IsDocumentData() : bool =
        Userdata.isDocumentData ()

    ///<summary>Verifies the current document contains user text</summary>
    ///
    ///<returns>(bool) True or False indicating the presence of Script user text</returns>
    static member IsDocumentUserText() : bool =
        Userdata.isDocumentUserText ()

    ///<summary>Verifies that an object contains user text</summary>
    ///<param name="object_id">(Guid) the object's identifier</param>
    ///
    ///<returns>(float) result of test:
    ///        0 = no user text
    ///        1 = attribute user text
    ///        2 = geometry user text
    ///        3 = both attribute and geometry user text</returns>
    static member IsUserText(object_id:Guid) : float =
        Userdata.isUserText object_id

    ///<summary>Adds or sets a user data string to the current document</summary>
    ///<param name="section">(string) the section name</param>
    ///<param name="entry">(string) the entry name</param>
    ///<param name="value">(string) the string value</param>
    ///
    ///<returns>(unit) unit</returns>
    static member SetDocumentData(section:string, entry:string, value:string) : unit =
        Userdata.setDocumentData value entry section

    ///<summary>Sets or removes user text stored in the document</summary>
    ///<param name="key">(string) key name to set</param>
    ///<param name="value">(string) Optional, Default Value:None, The string value to set. If omitted the key/value pair
    ///        specified by key will be deleted</param>
    ///
    ///<returns>(bool) True or False indicating success</returns>
    static member SetDocumentUserText(key:string, [<OPT;DEF(null)>]value:string) : bool =
        Userdata.setDocumentUserText value key

    ///<summary>Sets or removes user text stored on an object.</summary>
    ///<param name="object_id">(string) the object's identifier</param>
    ///<param name="key">(string) the key name to set</param>
    ///<param name="value">(string) Optional, Default Value:None, the string value to set. If omitted, the key/value pair
    ///          specified by key will be deleted</param>
    ///<param name="attach_to_geometry">(bool) Optional, Default Value:False, location on the object to store the user text</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SetUserText(object_id:string, key:string, [<OPT;DEF(null)>]value:string, [<OPT;DEF(false)>]attach_to_geometry:bool) : bool =
        Userdata.setUserText attach_to_geometry value key object_id

    ///<summary>Display browse-for-folder dialog allowing the user to select a folder</summary>
    ///<param name="folder">(string) Optional, Default Value:None, a default folder</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message</param>
    ///<param name="title">(string) Optional, Default Value:None, a dialog box title</param>
    ///
    ///<returns>(string) selected folder</returns>
    static member BrowseForFolder([<OPT;DEF(null)>]folder:string, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string =
        Userinterface.browseForFolder title message folder

    ///<summary>Displays a list of items in a checkable-style list dialog box</summary>
    ///<param name="items">((string*bool) seq) a list of tuples containing a string and a boolean check state</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message</param>
    ///<param name="title">(string) Optional, Default Value:None, a dialog box title</param>
    ///
    ///<returns>(string seq) of tuples containing the input string in items along with their new boolean check value</returns>
    static member CheckListBox(items:(string*bool) seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string seq =
        Userinterface.checkListBox title message items

    ///<summary>Displays a list of items in a combo-style list box dialog.</summary>
    ///<param name="items">(string seq) a list of string</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt of message</param>
    ///<param name="title">(string) Optional, Default Value:None, a dialog box title</param>
    ///
    ///<returns>(string) The selected item</returns>
    static member ComboListBox(items:string seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string =
        Userinterface.comboListBox title message items

    ///<summary>Display dialog prompting the user to enter a string. The
    ///    string value may span multiple lines</summary>
    ///<param name="default_string">(string) Optional, Default Value:None, a default string value.</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt message.</param>
    ///<param name="title">(string) Optional, Default Value:None, a dialog box title.</param>
    ///
    ///<returns>(string) Multiple lines that are separated by carriage return-linefeed combinations</returns>
    static member EditBox([<OPT;DEF(null)>]default_string:string, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string =
        Userinterface.editBox title message default_string

    ///<summary>Pause for user input of an angle</summary>
    ///<param name="point">(Point3d) Optional, Default Value:None, starting, or base point</param>
    ///<param name="reference_point">(Point3d) Optional, Default Value:None, if specified, the reference angle is calculated
    ///        from it and the base point</param>
    ///<param name="default_angle_degrees">(int) Optional, Default Value:0, a default angle value specified</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt to display</param>
    ///
    ///<returns>(float) angle in degree</returns>
    static member GetAngle([<OPT;DEF(null)>]point:Point3d, [<OPT;DEF(null)>]reference_point:Point3d, [<OPT;DEF(0)>]default_angle_degrees:int, [<OPT;DEF(null)>]message:string) : float =
        Userinterface.getAngle message default_angle_degrees reference_point point

    ///<summary>Pauses for user input of one or more boolean values. Boolean values are
    ///    displayed as click-able command line option toggles</summary>
    ///<param name="message">(string) a prompt</param>
    ///<param name="items">(string seq) list or tuple of options. Each option is a tuple of three strings
    ///        [n][1]    description of the boolean value. Must only consist of letters
    ///                  and numbers. (no characters like space, period, or dash
    ///        [n][2]    string identifying the false value
    ///        [n][3]    string identifying the true value</param>
    ///<param name="defaults">(bool seq) list of boolean values used as default or starting values</param>
    ///
    ///<returns>(bool seq) a list of values that represent the boolean values</returns>
    static member GetBoolean(message:string, items:string seq, defaults:bool seq) : bool seq =
        Userinterface.getBoolean defaults items message

    ///<summary>Pauses for user input of a box</summary>
    ///<param name="mode">(float) Optional, Default Value:0, The box selection mode.
    ///         0 = All modes
    ///         1 = Corner. The base rectangle is created by picking two corner points
    ///         2 = 3-Point. The base rectangle is created by picking three points
    ///         3 = Vertical. The base vertical rectangle is created by picking three points.
    ///         4 = Center. The base rectangle is created by picking a center point and a corner point</param>
    ///<param name="base_point">(Point3d) Optional, Default Value:None, optional 3D base point</param>
    ///<param name="prompt1">(string) Optional, Default Value:None, prompt1 of 'optional prompts to set' (FIXME 0)</param>
    ///<param name="prompt2">(string) Optional, Default Value:None, prompt2 of 'optional prompts to set' (FIXME 0)</param>
    ///<param name="prompt3">(string) Optional, Default Value:None, prompt3 of 'optional prompts to set' (FIXME 0)</param>
    ///
    ///<returns>(Point3d seq) list of eight Point3d that define the corners of the box on success</returns>
    static member GetBox([<OPT;DEF(0)>]mode:float, [<OPT;DEF(null)>]base_point:Point3d, [<OPT;DEF(null)>]prompt1:string, [<OPT;DEF(null)>]prompt2:string, [<OPT;DEF(null)>]prompt3:string) : Point3d seq =
        Userinterface.getBox prompt3 prompt2 prompt1 base_point mode

    ///<summary>Display the Rhino color picker dialog allowing the user to select an RGB color</summary>
    ///<param name="color">(Drawing.Color) Optional, Default Value:None, default RGB value. If omitted, the default color is black</param>
    ///
    ///<returns>(Drawing.Color) RGB tuple of three numbers on success</returns>
    static member GetColor([<OPT;DEF(null)>]color:Drawing.Color) : Drawing.Color =
        Userinterface.getColor color

    ///<summary>Retrieves the cursor's position</summary>
    ///
    ///<returns>(Point3d * Point3d * Guid * pointcontainingthefollowinginformation
    ///0cursorpositioninworldcoordinates
    ///1cursorpositioninscreencoordinates
    ///2idoftheactiveviewport
    ///3cursorpositioninclientcoordinates) </returns>
    static member GetCursorPos() : Point3d * Point3d * Guid * pointcontainingthefollowinginformation
0cursorpositioninworldcoordinates
1cursorpositioninscreencoordinates
2idoftheactiveviewport
3cursorpositioninclientcoordinates =
        Userinterface.getCursorPos ()

    ///<summary>Pauses for user input of a distance.</summary>
    ///<param name="first_pt">(Point3d) Optional, Default Value:None, First distance point</param>
    ///<param name="distance">(float) Optional, Default Value:None, Default distance</param>
    ///<param name="first_pt_msg">(string) Optional, Default Value:'First distance point', Prompt for the first distance point</param>
    ///<param name="second_pt_msg">(string) Optional, Default Value:'Second distance point', Prompt for the second distance point</param>
    ///
    ///<returns>(float) The distance between the two points .</returns>
    static member GetDistance([<OPT;DEF(null)>]first_pt:Point3d, [<OPT;DEF(null)>]distance:float, [<OPT;DEF('First distance point')>]first_pt_msg:string, [<OPT;DEF('Second distance point')>]second_pt_msg:string) : float =
        Userinterface.getDistance second_pt_msg first_pt_msg distance first_pt

    ///<summary>Prompt the user to pick one or more surface or polysurface edge curves</summary>
    ///<param name="message">(string) Optional, Default Value:None, A prompt or message.</param>
    ///<param name="min_count">(int) Optional, Default Value:1, minimum number of edges to select.</param>
    ///<param name="max_count">(int) Optional, Default Value:0, maximum number of edges to select.</param>
    ///<param name="select">(bool) Optional, Default Value:False, Select the duplicated edge curves.</param>
    ///
    ///<returns>(Guid seq) of selection prompts (curve id, parent id, selection point)</returns>
    static member GetEdgeCurves([<OPT;DEF(null)>]message:string, [<OPT;DEF(1)>]min_count:int, [<OPT;DEF(0)>]max_count:int, [<OPT;DEF(false)>]select:bool) : Guid seq =
        Userinterface.getEdgeCurves select max_count min_count message

    ///<summary>Pauses for user input of a whole number.</summary>
    ///<param name="message">(string) Optional, Default Value:None, A prompt or message.</param>
    ///<param name="number">(float) Optional, Default Value:None, A default whole number value.</param>
    ///<param name="minimum">(float) Optional, Default Value:None, A minimum allowable value.</param>
    ///<param name="maximum">(float) Optional, Default Value:None, A maximum allowable value.</param>
    ///
    ///<returns>(float) The whole number input by the user .</returns>
    static member GetInteger([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]number:float, [<OPT;DEF(null)>]minimum:float, [<OPT;DEF(null)>]maximum:float) : float =
        Userinterface.getInteger maximum minimum number message

    ///<summary>Displays dialog box prompting the user to select a layer</summary>
    ///<param name="title">(string) Optional, Default Value:"Select Layer", dialog box title</param>
    ///<param name="layer">(string) Optional, Default Value:None, name of a layer to preselect. If omitted, the current layer will be preselected</param>
    ///<param name="show_new_button">(bool) Optional, Default Value:False, show new button of 'Optional buttons to show on the dialog' (FIXME 0)</param>
    ///<param name="show_set_current">(bool) Optional, Default Value:False, show set current of 'Optional buttons to show on the dialog' (FIXME 0)</param>
    ///
    ///<returns>(string) name of selected layer</returns>
    static member GetLayer([<OPT;DEF("Select Layer")>]title:string, [<OPT;DEF(null)>]layer:string, [<OPT;DEF(false)>]show_new_button:bool, [<OPT;DEF(false)>]show_set_current:bool) : string =
        Userinterface.getLayer show_set_current show_new_button layer title

    ///<summary>Displays a dialog box prompting the user to select one or more layers</summary>
    ///<param name="title">(string) Optional, Default Value:"Select Layers", dialog box title</param>
    ///<param name="show_new_button">(bool) Optional, Default Value:False, Optional button to show on the dialog</param>
    ///
    ///<returns>(string) The names of selected layers</returns>
    static member GetLayers([<OPT;DEF("Select Layers")>]title:string, [<OPT;DEF(false)>]show_new_button:bool) : string =
        Userinterface.getLayers show_new_button title

    ///<summary>Prompts the user to pick points that define a line</summary>
    ///<param name="mode">(float) Optional, Default Value:0, line definition mode.
    ///        0  Default - Show all modes, start in two-point mode
    ///        1  Two-point - Defines a line from two points.
    ///        2  Normal - Defines a line normal to a location on a surface.
    ///        3  Angled - Defines a line at a specified angle from a reference line.
    ///        4  Vertical - Defines a line vertical to the construction plane.
    ///        5  Four-point - Defines a line using two points to establish direction and two points to establish length.
    ///        6  Bisector - Defines a line that bisects a specified angle.
    ///        7  Perpendicular - Defines a line perpendicular to or from a curve
    ///        8  Tangent - Defines a line tangent from a curve.
    ///        9  Extension - Defines a line that extends from a curve.</param>
    ///<param name="point">(Point3d) Optional, Default Value:None, optional starting point</param>
    ///<param name="message1">(string) Optional, Default Value:None, message1 of 'optional prompts' (FIXME 0)</param>
    ///<param name="message2">(string) Optional, Default Value:None, message2 of 'optional prompts' (FIXME 0)</param>
    ///<param name="message3">(string) Optional, Default Value:None, message3 of 'optional prompts' (FIXME 0)</param>
    ///
    ///<returns>(Line) Tuple of two points on success</returns>
    static member GetLine([<OPT;DEF(0)>]mode:float, [<OPT;DEF(null)>]point:Point3d, [<OPT;DEF(null)>]message1:string, [<OPT;DEF(null)>]message2:string, [<OPT;DEF(null)>]message3:string) : Line =
        Userinterface.getLine message3 message2 message1 point mode

    ///<summary>Displays a dialog box prompting the user to select one linetype</summary>
    ///<param name="default_linetype">(string) Optional, Default Value:None, Optional. The name of the linetype to select. If omitted, the current linetype will be selected.</param>
    ///<param name="show_by_layer">(bool) Optional, Default Value:False, If True, the "by Layer" linetype will show. Defaults to False.</param>
    ///
    ///<returns>(string) The names of selected linetype</returns>
    static member GetLinetype([<OPT;DEF(null)>]default_linetype:string, [<OPT;DEF(false)>]show_by_layer:bool) : string =
        Userinterface.getLinetype show_by_layer default_linetype

    ///<summary>Prompts the user to pick one or more mesh faces</summary>
    ///<param name="object_id">(Guid) the mesh object's identifier</param>
    ///<param name="message">(string) Optional, Default Value:"", a prompt of message</param>
    ///<param name="min_count">(int) Optional, Default Value:1, the minimum number of faces to select</param>
    ///<param name="max_count">(int) Optional, Default Value:0, the maximum number of faces to select.
    ///        If 0, the user must press enter to finish selection.
    ///        If -1, selection stops as soon as there are at least min_count faces selected.</param>
    ///
    ///<returns>(float seq) of mesh face indices on success</returns>
    static member GetMeshFaces(object_id:Guid, [<OPT;DEF("")>]message:string, [<OPT;DEF(1)>]min_count:int, [<OPT;DEF(0)>]max_count:int) : float seq =
        Userinterface.getMeshFaces max_count min_count message object_id

    ///<summary>Prompts the user to pick one or more mesh vertices</summary>
    ///<param name="object_id">(Guid) the mesh object's identifier</param>
    ///<param name="message">(string) Optional, Default Value:"", a prompt of message</param>
    ///<param name="min_count">(int) Optional, Default Value:1, the minimum number of vertices to select</param>
    ///<param name="max_count">(int) Optional, Default Value:0, the maximum number of vertices to select. If 0, the user must
    ///        press enter to finish selection. If -1, selection stops as soon as there
    ///        are at least min_count vertices selected.</param>
    ///
    ///<returns>(float seq) of mesh vertex indices on success</returns>
    static member GetMeshVertices(object_id:Guid, [<OPT;DEF("")>]message:string, [<OPT;DEF(1)>]min_count:int, [<OPT;DEF(0)>]max_count:int) : float seq =
        Userinterface.getMeshVertices max_count min_count message object_id

    ///<summary>Pauses for user input of a point.</summary>
    ///<param name="message">(string) Optional, Default Value:None, A prompt or message.</param>
    ///<param name="base_point">(Point3d) Optional, Default Value:None, list of 3 numbers or Point3d identifying a starting, or base point</param>
    ///<param name="distance">(float) Optional, Default Value:None, constraining distance. If distance is specified, basePoint must also be specified.</param>
    ///<param name="in_plane">(bool) Optional, Default Value:False, constrains the point selections to the active construction plane.</param>
    ///
    ///<returns>(Point3d) point on success</returns>
    static member GetPoint([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]base_point:Point3d, [<OPT;DEF(null)>]distance:float, [<OPT;DEF(false)>]in_plane:bool) : Point3d =
        Userinterface.getPoint in_plane distance base_point message

    ///<summary>Pauses for user input of a point constrainted to a curve object</summary>
    ///<param name="curve_id">(Guid) identifier of the curve to get a point on</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt of message</param>
    ///
    ///<returns>(Point3d) 3d point</returns>
    static member GetPointOnCurve(curve_id:Guid, [<OPT;DEF(null)>]message:string) : Point3d =
        Userinterface.getPointOnCurve message curve_id

    ///<summary>Pauses for user input of a point constrained to a mesh object</summary>
    ///<param name="mesh_id">(Guid) identifier of the mesh to get a point on</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message</param>
    ///
    ///<returns>(Point3d) 3d point</returns>
    static member GetPointOnMesh(mesh_id:Guid, [<OPT;DEF(null)>]message:string) : Point3d =
        Userinterface.getPointOnMesh message mesh_id

    ///<summary>Pauses for user input of a point constrained to a surface or polysurface
    ///    object</summary>
    ///<param name="surface_id">(Guid) identifier of the surface to get a point on</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message</param>
    ///
    ///<returns>(Point3d) 3d point</returns>
    static member GetPointOnSurface(surface_id:Guid, [<OPT;DEF(null)>]message:string) : Point3d =
        Userinterface.getPointOnSurface message surface_id

    ///<summary>Pauses for user input of one or more points</summary>
    ///<param name="draw_lines">(bool) Optional, Default Value:False, Draw lines between points</param>
    ///<param name="in_plane">(bool) Optional, Default Value:False, Constrain point selection to the active construction plane</param>
    ///<param name="message1">(string) Optional, Default Value:None, A prompt or message for the first point</param>
    ///<param name="message2">(string) Optional, Default Value:None, A prompt or message for the next points</param>
    ///<param name="max_points">(float) Optional, Default Value:None, maximum number of points to pick. If not specified, an
    ///                        unlimited number of points can be picked.</param>
    ///<param name="base_point">(Point3d) Optional, Default Value:None, a starting or base point</param>
    ///
    ///<returns>(Point3d seq) of 3d points</returns>
    static member GetPoints([<OPT;DEF(false)>]draw_lines:bool, [<OPT;DEF(false)>]in_plane:bool, [<OPT;DEF(null)>]message1:string, [<OPT;DEF(null)>]message2:string, [<OPT;DEF(null)>]max_points:float, [<OPT;DEF(null)>]base_point:Point3d) : Point3d seq =
        Userinterface.getPoints base_point max_points message2 message1 in_plane draw_lines

    ///<summary>Prompts the user to pick points that define a polyline.</summary>
    ///<param name="flags">(int) Optional, Default Value:3, The options are bit coded flags. Values can be added together to specify more than one option. The default is 3.
    ///      value description
    ///      1     Permit close option. If specified, then after 3 points have been picked, the user can type "Close" and a closed polyline will be returned.
    ///      2     Permit close snap. If specified, then after 3 points have been picked, the user can pick near the start point and a closed polyline will be returned.
    ///      4     Force close. If specified, then the returned polyline is always closed. If specified, then intMax must be 0 or >= 4.
    ///      Note: the default is 3, or "Permit close option = True", "Permit close snap = True", and "Force close = False".</param>
    ///<param name="message1">(string) Optional, Default Value:None, A prompt or message for the first point.</param>
    ///<param name="message2">(string) Optional, Default Value:None, A prompt or message for the second point.</param>
    ///<param name="message3">(string) Optional, Default Value:None, A prompt or message for the third point.</param>
    ///<param name="message4">(string) Optional, Default Value:None, A prompt or message for the 'next' point.</param>
    ///<param name="min">(float) Optional, Default Value:2, The minimum number of points to require. The default is 2.</param>
    ///<param name="max">(float) Optional, Default Value:0, The maximum number of points to require; 0 for no limit.  The default is 0.</param>
    ///
    ///<returns>(Point3d seq) A list of 3-D points that define the polyline .</returns>
    static member GetPolyline([<OPT;DEF(3)>]flags:int, [<OPT;DEF(null)>]message1:string, [<OPT;DEF(null)>]message2:string, [<OPT;DEF(null)>]message3:string, [<OPT;DEF(null)>]message4:string, [<OPT;DEF(2)>]min:float, [<OPT;DEF(0)>]max:float) : Point3d seq =
        Userinterface.getPolyline max min message4 message3 message2 message1 flags

    ///<summary>Pauses for user input of a number.</summary>
    ///<param name="message">(string) Optional, Default Value:"Number", A prompt or message.</param>
    ///<param name="number">(float) Optional, Default Value:None, A default number value.</param>
    ///<param name="minimum">(float) Optional, Default Value:None, A minimum allowable value.</param>
    ///<param name="maximum">(float) Optional, Default Value:None, A maximum allowable value.</param>
    ///
    ///<returns>(float) The number input by the user .</returns>
    static member GetReal([<OPT;DEF("Number")>]message:string, [<OPT;DEF(null)>]number:float, [<OPT;DEF(null)>]minimum:float, [<OPT;DEF(null)>]maximum:float) : float =
        Userinterface.getReal maximum minimum number message

    ///<summary>Pauses for user input of a rectangle</summary>
    ///<param name="mode">(float) Optional, Default Value:0, The rectangle selection mode. The modes are as follows
    ///          0 = All modes
    ///          1 = Corner - a rectangle is created by picking two corner points
    ///          2 = 3Point - a rectangle is created by picking three points
    ///          3 = Vertical - a vertical rectangle is created by picking three points
    ///          4 = Center - a rectangle is created by picking a center point and a corner point</param>
    ///<param name="base_point">(Point3d) Optional, Default Value:None, a 3d base point</param>
    ///<param name="prompt1">(string) Optional, Default Value:None, prompt1 of 'optional prompts' (FIXME 0)</param>
    ///<param name="prompt2">(string) Optional, Default Value:None, prompt2 of 'optional prompts' (FIXME 0)</param>
    ///<param name="prompt3">(string) Optional, Default Value:None, prompt3 of 'optional prompts' (FIXME 0)</param>
    ///
    ///<returns>(Point3d * Point3d * Point3d * Point3d) four 3d points that define the corners of the rectangle</returns>
    static member GetRectangle([<OPT;DEF(0)>]mode:float, [<OPT;DEF(null)>]base_point:Point3d, [<OPT;DEF(null)>]prompt1:string, [<OPT;DEF(null)>]prompt2:string, [<OPT;DEF(null)>]prompt3:string) : Point3d * Point3d * Point3d * Point3d =
        Userinterface.getRectangle prompt3 prompt2 prompt1 base_point mode

    ///<summary>Pauses for user input of a string value</summary>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message</param>
    ///<param name="defaultString">(string) Optional, Default Value:None, a default value</param>
    ///<param name="strings">(string seq) Optional, Default Value:None, list of strings to be displayed as a click-able command options.
    ///                                      Note, strings cannot begin with a numeric character</param>
    ///
    ///<returns>(string) The string either input or selected by the user .
    ///           If the user presses the Enter key without typing in a string, an empty string "" is returned.</returns>
    static member GetString([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]defaultString:string, [<OPT;DEF(null)>]strings:string seq) : string =
        Userinterface.getString strings defaultString message

    ///<summary>Display a list of items in a list box dialog.</summary>
    ///<param name="items">(string seq) a list of values to select</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt of message</param>
    ///<param name="title">(string) Optional, Default Value:None, a dialog box title</param>
    ///<param name="default">(string) Optional, Default Value:None, selected item in the list</param>
    ///
    ///<returns>(string) he selected item</returns>
    static member ListBox(items:string seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]default:string) : string =
        Userinterface.listBox default title message items

    ///<summary>Displays a message box. A message box contains a message and
    ///    title, plus any combination of predefined icons and push buttons.</summary>
    ///<param name="message">(string) A prompt or message.</param>
    ///<param name="buttons">(float) Optional, Default Value:0, buttons and icon to display as a bit coded flag. Can be a combination of the
    ///        following flags. If omitted, an OK button and no icon is displayed
    ///        0      Display OK button only.
    ///        1      Display OK and Cancel buttons.
    ///        2      Display Abort, Retry, and Ignore buttons.
    ///        3      Display Yes, No, and Cancel buttons.
    ///        4      Display Yes and No buttons.
    ///        5      Display Retry and Cancel buttons.
    ///        16     Display Critical Message icon.
    ///        32     Display Warning Query icon.
    ///        48     Display Warning Message icon.
    ///        64     Display Information Message icon.
    ///        0      First button is the default.
    ///        256    Second button is the default.
    ///        512    Third button is the default.
    ///        768    Fourth button is the default.
    ///        0      Application modal. The user must respond to the message box
    ///               before continuing work in the current application.
    ///        4096   System modal. The user must respond to the message box
    ///               before continuing work in any application.</param>
    ///<param name="title">(string) Optional, Default Value:"", the dialog box title</param>
    ///
    ///<returns>(float) indicating which button was clicked:
    ///        1      OK button was clicked.
    ///        2      Cancel button was clicked.
    ///        3      Abort button was clicked.
    ///        4      Retry button was clicked.
    ///        5      Ignore button was clicked.
    ///        6      Yes button was clicked.
    ///        7      No button was clicked.</returns>
    static member MessageBox(message:string, [<OPT;DEF(0)>]buttons:float, [<OPT;DEF("")>]title:string) : float =
        Userinterface.messageBox title buttons message

    ///<summary>Displays list of items and their values in a property-style list box dialog</summary>
    ///<param name="items">(string seq) items of 'list of string items and their corresponding values' (FIXME 0)</param>
    ///<param name="values">(string seq) values of 'list of string items and their corresponding values' (FIXME 0)</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message</param>
    ///<param name="title">(string) Optional, Default Value:None, a dialog box title</param>
    ///
    ///<returns>(string seq) of new values on success</returns>
    static member PropertyListBox(items:string seq, values:string seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string seq =
        Userinterface.propertyListBox title message values items

    ///<summary>Displays a list of items in a multiple-selection list box dialog</summary>
    ///<param name="items">(string seq) a zero-based list of string items</param>
    ///<param name="message">(string) Optional, Default Value:None, a prompt or message</param>
    ///<param name="title">(string) Optional, Default Value:None, a dialog box title</param>
    ///<param name="defaults">(string seq) Optional, Default Value:None, either a string representing the pre-selected item in the list
    ///                                          or a list if multiple items are pre-selected</param>
    ///
    ///<returns>(string seq) containing the selected items</returns>
    static member MultiListBox(items:string seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]defaults:string seq) : string seq =
        Userinterface.multiListBox defaults title message items

    ///<summary>Displays file open dialog box allowing the user to enter a file name.
    ///    Note, this function does not open the file.</summary>
    ///<param name="title">(string) Optional, Default Value:None, A dialog box title.</param>
    ///<param name="filter">(string) Optional, Default Value:None, A filter string. The filter must be in the following form:
    ///        "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///        If omitted, the filter (*.*) is used.</param>
    ///<param name="folder">(string) Optional, Default Value:None, A default folder.</param>
    ///<param name="filename">(string) Optional, Default Value:None, a default file name</param>
    ///<param name="extension">(string) Optional, Default Value:None, a default file extension</param>
    ///
    ///<returns>(string) file name is successful</returns>
    static member OpenFileName([<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]filter:string, [<OPT;DEF(null)>]folder:string, [<OPT;DEF(null)>]filename:string, [<OPT;DEF(null)>]extension:string) : string =
        Userinterface.openFileName extension filename folder filter title

    ///<summary>Displays file open dialog box allowing the user to select one or more file names.
    ///    Note, this function does not open the file.</summary>
    ///<param name="title">(string) Optional, Default Value:None, A dialog box title.</param>
    ///<param name="filter">(string) Optional, Default Value:None, A filter string. The filter must be in the following form:
    ///        "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///        If omitted, the filter (*.*) is used.</param>
    ///<param name="folder">(string) Optional, Default Value:None, A default folder.</param>
    ///<param name="filename">(string) Optional, Default Value:None, a default file name</param>
    ///<param name="extension">(string) Optional, Default Value:None, a default file extension</param>
    ///
    ///<returns>(string seq) of selected file names</returns>
    static member OpenFileNames([<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]filter:string, [<OPT;DEF(null)>]folder:string, [<OPT;DEF(null)>]filename:string, [<OPT;DEF(null)>]extension:string) : string seq =
        Userinterface.openFileNames extension filename folder filter title

    ///<summary>Display a context-style popup menu. The popup menu can appear almost
    ///    anywhere, and can be dismissed by clicking the left or right mouse buttons</summary>
    ///<param name="items">(string seq) list of strings representing the menu items. An empty string or None
    ///        will create a separator</param>
    ///<param name="modes">(float seq) Optional, Default Value:None, List of numbers identifying the display modes. If omitted, all
    ///        modes are enabled.
    ///          0 = menu item is enabled
    ///          1 = menu item is disabled
    ///          2 = menu item is checked
    ///          3 = menu item is disabled and checked</param>
    ///<param name="point">(Point3d) Optional, Default Value:None, a 3D point where the menu item will appear. If omitted, the menu
    ///        will appear at the current cursor position</param>
    ///<param name="view">(string) Optional, Default Value:None, if point is specified, the view in which the point is computed.
    ///        If omitted, the active view is used</param>
    ///
    ///<returns>(float) index of the menu item picked or -1 if no menu item was picked</returns>
    static member PopupMenu(items:string seq, [<OPT;DEF(null)>]modes:float seq, [<OPT;DEF(null)>]point:Point3d, [<OPT;DEF(null)>]view:string) : float =
        Userinterface.popupMenu view point modes items

    ///<summary>Display a dialog box prompting the user to enter a number</summary>
    ///<param name="message">(string) Optional, Default Value:"", a prompt message.</param>
    ///<param name="default_number">(float) Optional, Default Value:None, a default number.</param>
    ///<param name="title">(string) Optional, Default Value:"", a dialog box title.</param>
    ///<param name="minimum">(float) Optional, Default Value:None, a minimum allowable value.</param>
    ///<param name="maximum">(float) Optional, Default Value:None, a maximum allowable value.</param>
    ///
    ///<returns>(float) The newly entered number on success</returns>
    static member RealBox([<OPT;DEF("")>]message:string, [<OPT;DEF(null)>]default_number:float, [<OPT;DEF("")>]title:string, [<OPT;DEF(null)>]minimum:float, [<OPT;DEF(null)>]maximum:float) : float =
        Userinterface.realBox maximum minimum title default_number message

    ///<summary>Display a save dialog box allowing the user to enter a file name.
    ///    Note, this function does not save the file.</summary>
    ///<param name="title">(string) Optional, Default Value:None, A dialog box title.</param>
    ///<param name="filter">(string) Optional, Default Value:None, A filter string. The filter must be in the following form:
    ///        "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///        If omitted, the filter (*.*) is used.</param>
    ///<param name="folder">(string) Optional, Default Value:None, A default folder.</param>
    ///<param name="filename">(string) Optional, Default Value:None, a default file name</param>
    ///<param name="extension">(string) Optional, Default Value:None, a default file extension</param>
    ///
    ///<returns>(string) the file name is successful</returns>
    static member SaveFileName([<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]filter:string, [<OPT;DEF(null)>]folder:string, [<OPT;DEF(null)>]filename:string, [<OPT;DEF(null)>]extension:string) : string =
        Userinterface.saveFileName extension filename folder filter title

    ///<summary>Display a dialog box prompting the user to enter a string value.</summary>
    ///<param name="message">(string) Optional, Default Value:None, a prompt message</param>
    ///<param name="default_value">(string) Optional, Default Value:None, a default string value</param>
    ///<param name="title">(string) Optional, Default Value:None, a dialog box title</param>
    ///
    ///<returns>(string) the newly entered string value</returns>
    static member StringBox([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]default_value:string, [<OPT;DEF(null)>]title:string) : string =
        Userinterface.stringBox title default_value message

    ///<summary>Display a text dialog box similar to the one used by the _What command.</summary>
    ///<param name="message">(string) Optional, Default Value:None, a message</param>
    ///<param name="title">(string) Optional, Default Value:None, the message title</param>
    ///
    ///<returns>(unit) in any case</returns>
    static member TextOut([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : unit =
        Userinterface.textOut title message

    ///<summary>Return True if the script is being executed in the context of Rhino</summary>
    ///
    ///<returns>(bool) True if the script is being executed in the context of Rhino</returns>
    static member ContextIsRhino() : bool =
        Utility.contextIsRhino ()

    ///<summary>Return True if the script is being executed in a grasshopper component</summary>
    ///
    ///<returns>(bool) True if the script is being executed in a grasshopper component</returns>
    static member ContextIsGrasshopper() : bool =
        Utility.contextIsGrasshopper ()

    ///<summary>Measures the angle between two points</summary>
    ///<param name="point1">(Point3d) point1 of 'the input points' (FIXME 0)</param>
    ///<param name="point2">(Point3d) point2 of 'the input points' (FIXME 0)</param>
    ///<param name="plane">(bool) Optional, Default Value:True, Boolean or Plane
    ///        If True, angle calculation is based on the world coordinate system.
    ///        If False, angle calculation is based on the active construction plane
    ///        If a plane is provided, angle calculation is with respect to this plane</param>
    ///
    ///<returns>(float * float * float * float * float * float) containing the following elements 
    ///        element 0 = the X,Y angle in degrees
    ///        element 1 = the elevation
    ///        element 2 = delta in the X direction
    ///        element 3 = delta in the Y direction
    ///        element 4 = delta in the Z direction</returns>
    static member Angle(point1:Point3d, point2:Point3d, [<OPT;DEF(true)>]plane:bool) : float * float * float * float * float * float =
        Utility.angle plane point2 point1

    ///<summary>Measures the angle between two lines</summary>
    ///<param name="line1">(Line) List of 6 numbers or 2 Point3d.</param>
    ///<param name="line2">(Line) List of 6 numbers or 2 Point3d.</param>
    ///
    ///<returns>(float * float) containing the following elements .
    ///        0 The angle in degrees.
    ///        1 The reflex angle in degrees.</returns>
    static member Angle2(line1:Line, line2:Line) : float * float =
        Utility.angle2 line2 line1

    ///<summary>Changes the luminance of a red-green-blue value. Hue and saturation are
    ///    not affected</summary>
    ///<param name="rgb">(Drawing.Color) initial rgb value</param>
    ///<param name="luma">(float) The luminance in units of 0.1 percent of the total range. A
    ///          value of luma = 50 corresponds to 5 percent of the maximum luminance</param>
    ///<param name="scale">(bool) Optional, Default Value:False, if True, luma specifies how much to increment or decrement the
    ///          current luminance. If False, luma specified the absolute luminance.</param>
    ///
    ///<returns>(Drawing.Color) modified rgb value</returns>
    static member ColorAdjustLuma(rgb:Drawing.Color, luma:float, [<OPT;DEF(false)>]scale:bool) : Drawing.Color =
        Utility.colorAdjustLuma scale luma rgb

    ///<summary>Retrieves intensity value for the blue component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) the RGB color value</param>
    ///
    ///<returns>(float) The blue component , otherwise None</returns>
    static member ColorBlueValue(rgb:Drawing.Color) : float =
        Utility.colorBlueValue rgb

    ///<summary>Retrieves intensity value for the green component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) the RGB color value</param>
    ///
    ///<returns>(float) The green component , otherwise None</returns>
    static member ColorGreenValue(rgb:Drawing.Color) : float =
        Utility.colorGreenValue rgb

    ///<summary>Converts colors from hue-lumanence-saturation to RGB</summary>
    ///<param name="hls">(Drawing.Color) the HLS color value</param>
    ///
    ///<returns>(Drawing.Color) The RGB color value , otherwise False</returns>
    static member ColorHLSToRGB(hls:Drawing.Color) : Drawing.Color =
        Utility.colorHLSToRGB hls

    ///<summary>Retrieves intensity value for the red component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) the RGB color value</param>
    ///
    ///<returns>(Drawing.Color) The red color value , otherwise False</returns>
    static member ColorRedValue(rgb:Drawing.Color) : Drawing.Color =
        Utility.colorRedValue rgb

    ///<summary>Convert colors from RGB to HLS</summary>
    ///<param name="rgb">(Drawing.Color) the RGB color value</param>
    ///
    ///<returns>(Drawing.Color) The HLS color value , otherwise False</returns>
    static member ColorRGBToHLS(rgb:Drawing.Color) : Drawing.Color =
        Utility.colorRGBToHLS rgb

    ///<summary>Removes duplicates from an array of numbers.</summary>
    ///<param name="numbers">(float seq) list or tuple</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, The minimum distance between numbers.  Numbers that fall within this tolerance will be discarded.  If omitted, Rhino's internal zero tolerance is used.</param>
    ///
    ///<returns>(float seq) numbers with duplicates removed .</returns>
    static member CullDuplicateNumbers(numbers:float seq, [<OPT;DEF(null)>]tolerance:float) : float seq =
        Utility.cullDuplicateNumbers tolerance numbers

    ///<summary>Removes duplicates from a list of 3D points.</summary>
    ///<param name="points">(Point3d seq) A list of 3D points.</param>
    ///<param name="tolerance">(float) Optional, Default Value:-1, Minimum distance between points. Points within this
    ///        tolerance will be discarded. If omitted, Rhino's internal zero tolerance
    ///        is used.</param>
    ///
    ///<returns>(Point3d seq) of 3D points with duplicates removed .</returns>
    static member CullDuplicatePoints(points:Point3d seq, [<OPT;DEF(-1)>]tolerance:float) : Point3d seq =
        Utility.cullDuplicatePoints tolerance points

    ///<summary>Returns 3D point that is a specified angle and distance from a 3D point</summary>
    ///<param name="point">(Point3d) the point to transform</param>
    ///<param name="angle_degrees">(int) angle in degrees</param>
    ///<param name="distance">(float) distance from point</param>
    ///<param name="plane">(Plane) Optional, Default Value:None, plane to base the transformation. If omitted, the world
    ///        x-y plane is used</param>
    ///
    ///<returns>(Point3d) resulting point is successful</returns>
    static member Polar(point:Point3d, angle_degrees:int, distance:float, [<OPT;DEF(null)>]plane:Plane) : Point3d =
        Utility.polar plane distance angle_degrees point

    ///<summary>Flattens an array of 3-D points into a one-dimensional list of real numbers. For example, if you had an array containing three 3-D points, this method would return a one-dimensional array containing nine real numbers.</summary>
    ///<param name="points">(Point3d seq) Points to flatten</param>
    ///
    ///<returns>(float seq) A one-dimensional list containing real numbers, , otherwise None</returns>
    static member SimplifyArray(points:Point3d seq) : float seq =
        Utility.simplifyArray points

    ///<summary>Suspends execution of a running script for the specified interval</summary>
    ///<param name="milliseconds">(float) thousands of a second</param>
    ///
    ///<returns>(unit) </returns>
    static member Sleep(milliseconds:float) : unit =
        Utility.sleep milliseconds

    ///<summary>Sorts list of points so they will be connected in a "reasonable" polyline order</summary>
    ///<param name="points">(Point3d seq) the points to sort</param>
    ///<param name="tolerance">(float) Optional, Default Value:None, minimum distance between points. Points that fall within this tolerance
    ///        will be discarded. If omitted, Rhino's internal zero tolerance is used.</param>
    ///
    ///<returns>(Point3d seq) of sorted 3D points</returns>
    static member SortPointList(points:Point3d seq, [<OPT;DEF(null)>]tolerance:float) : Point3d seq =
        Utility.sortPointList tolerance points

    ///<summary>Sorts the components of an array of 3D points</summary>
    ///<param name="points">(Point3d seq) points to sort</param>
    ///<param name="ascending">(bool) Optional, Default Value:True, ascending if omitted (True) or True, descending if False.</param>
    ///<param name="order">(float) Optional, Default Value:0, the component sort order
    ///        Value       Component Sort Order
    ///        0 (default) X, Y, Z
    ///        1           X, Z, Y
    ///        2           Y, X, Z
    ///        3           Y, Z, X
    ///        4           Z, X, Y
    ///        5           Z, Y, X</param>
    ///
    ///<returns>(Point3d seq) sorted 3-D points</returns>
    static member SortPoints(points:Point3d seq, [<OPT;DEF(true)>]ascending:bool, [<OPT;DEF(0)>]order:float) : Point3d seq =
        Utility.sortPoints order ascending points

    ///<summary>convert a formatted string value into a 3D point value</summary>
    ///<param name="point">(string) A string that contains a delimited point like "1,2,3".</param>
    ///
    ///<returns>(Point3d) Point structure from the input string.</returns>
    static member Str2Pt(point:string) : Point3d =
        Utility.str2Pt point

    ///<summary>Converts 'point' into a Rhino.Geometry.Point3d if possible.
    ///    If the provided object is already a point, it value is copied.
    ///    In case the conversion fails, an error is raised.
    ///    Alternatively, you can also pass two coordinates singularly for a
    ///    point on the XY plane, or three for a 3D point.</summary>
    ///<param name="point">('T * float * float) </param>
    ///<param name="y">(float) Optional, Default Value:None, y position</param>
    ///<param name="z">(float) Optional, Default Value:None, z position</param>
    ///
    ///<returns>(Point3d) a Rhino.Geometry.Point3d. This can be seen as an object with three indices:
    ///        [0]  X coordinate
    ///        [1]  Y coordinate
    ///        [2]  Z coordinate.</returns>
    static member CreatePoint(point:'T * float * float, [<OPT;DEF(null)>]y:float, [<OPT;DEF(null)>]z:float) : Point3d =
        Utility.createPoint z y point

    ///<summary>Converts 'vector' into a Rhino.Geometry.Vector3d if possible.
    ///    If the provided object is already a vector, it value is copied.
    ///    If the conversion fails, an error is raised.
    ///    Alternatively, you can also pass two coordinates singularly for a
    ///    vector on the XY plane, or three for a 3D vector.</summary>
    ///<param name="vector">('T * float * float) </param>
    ///<param name="y">(float) Optional, Default Value:None, y position</param>
    ///<param name="z">(float) Optional, Default Value:None, z position</param>
    ///
    ///<returns>(Rhino.Geometry.Vector3d. This can be seen as an object with three indices) result[0]: X component, result[1]: Y component, and result[2] Z component.</returns>
    static member CreateVector(vector:'T * float * float, [<OPT;DEF(null)>]y:float, [<OPT;DEF(null)>]z:float) : Rhino.Geometry.Vector3d. This can be seen as an object with three indices =
        Utility.createVector z y vector

    ///<summary>Converts input into a Rhino.Geometry.Plane object if possible.
    ///    If the provided object is already a plane, its value is copied.
    ///    The returned data is accessible by indexing[origin, X axis, Y axis, Z axis], and that is the suggested method to interact with the type.
    ///    The Z axis is in any case computed from the X and Y axes, so providing it is possible but not required.
    ///    If the conversion fails, an error is raised.</summary>
    ///<param name="plane_or_origin">(Point3d * Vector3d * Vector3d) </param>
    ///<param name="x_axis">(Vector3d) Optional, Default Value:None, direction of X-Axis</param>
    ///<param name="y_axis">(Vector3d) Optional, Default Value:None, direction of Y-Axis</param>
    ///<param name="ignored">(ignored) Optional, Default Value:None, this paramneter is always ignored</param>
    ///
    ///<returns>(Plane) A Rhino.Geometry.plane.</returns>
    static member CreatePlane(plane_or_origin:Point3d * Vector3d * Vector3d, [<OPT;DEF(null)>]x_axis:Vector3d, [<OPT;DEF(null)>]y_axis:Vector3d, [<OPT;DEF(null)>]ignored:ignored) : Plane =
        Utility.createPlane ignored y_axis x_axis plane_or_origin

    ///<summary>Converts input into a Rhino.Geometry.Transform object if possible.
    ///    If the provided object is already a transform, its value is copied.
    ///    The returned data is accessible by indexing[row, column], and that is the suggested method to interact with the type.
    ///    If the conversion fails, an error is raised.</summary>
    ///<param name="xform">(nested list) the transform. This can be seen as a 4x4 matrix, given as nested lists or tuples.</param>
    ///
    ///<returns>(Transform) A Rhino.Geometry.Transform. result[0,3] gives access to the first row, last column.</returns>
    static member CreateXform(xform:nested list) : Transform =
        Utility.createXform xform

    ///<summary>Converts 'color' into a native color object if possible.
    ///    The returned data is accessible by indexing, and that is the suggested method to interact with the type.
    ///    Red index is [0], Green index is [1], Blue index is [2] and Alpha index is [3].
    ///    If the provided object is already a color, its value is copied.
    ///    Alternatively, you can also pass three coordinates singularly for an RGB color, or four
    ///    for an RGBA color point.</summary>
    ///<param name="color">(float*float*float) list or 3 or 4 items. Also, a single int can be passed and it will be bitwise-parsed.</param>
    ///<param name="g">(int) Optional, Default Value:None, green value</param>
    ///<param name="b">(int) Optional, Default Value:None, blue value</param>
    ///<param name="a">(int) Optional, Default Value:None, alpha value</param>
    ///
    ///<returns>(Drawing.Color) An object that can be indexed for red, green, blu, alpha. Item[0] is red.</returns>
    static member CreateColor(color:float*float*float, [<OPT;DEF(null)>]g:int, [<OPT;DEF(null)>]b:int, [<OPT;DEF(null)>]a:int) : Drawing.Color =
        Utility.createColor a b g color

    ///<summary>Converts 'interval' into a Rhino.Geometry.Interval.
    ///    If the provided object is already an interval, its value is copied.
    ///    In case the conversion fails, an error is raised.
    ///    In case a single number is provided, it will be translated to an increasing interval that includes
    ///    the provided input and 0. If two values are provided, they will be used instead.</summary>
    ///<param name="interval">(float * float) or any item that can be accessed at index 0 and 1; an Interval or just the lower bound</param>
    ///<param name="y">(float) Optional, Default Value:None, uper bound of interval</param>
    ///
    ///<returns>(float*float) a Rhino.Geometry.Interval. This can be seen as an object made of two items:
    ///        [0] start of interval
    ///        [1] end of interval</returns>
    static member CreateInterval(interval:float * float, [<OPT;DEF(null)>]y:float) : float*float =
        Utility.createInterval y interval

    ///<summary>Add new detail view to an existing layout view</summary>
    ///<param name="layout_id">(Guid) identifier of an existing layout</param>
    ///<param name="corner1">(Point3d) corner1 of '2d corners of the detail in the layout's unit system' (FIXME 0)</param>
    ///<param name="corner2">(Point3d) corner2 of '2d corners of the detail in the layout's unit system' (FIXME 0)</param>
    ///<param name="title">(string) Optional, Default Value:None, title of the new detail</param>
    ///<param name="projection">(float) Optional, Default Value:1, type of initial view projection for the detail
    ///          1 = parallel top view
    ///          2 = parallel bottom view
    ///          3 = parallel left view
    ///          4 = parallel right view
    ///          5 = parallel front view
    ///          6 = parallel back view
    ///          7 = perspective view</param>
    ///
    ///<returns>(Guid) identifier of the newly created detail on success</returns>
    static member AddDetail(layout_id:Guid, corner1:Point3d, corner2:Point3d, [<OPT;DEF(null)>]title:string, [<OPT;DEF(1)>]projection:float) : Guid =
        View.addDetail projection title corner2 corner1 layout_id

    ///<summary>Adds a new page layout view</summary>
    ///<param name="title">(string) Optional, Default Value:None, title of new layout</param>
    ///<param name="size">(float * float) Optional, Default Value:None, width and height of paper for the new layout</param>
    ///
    ///<returns>(Guid) id of new layout</returns>
    static member AddLayout([<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]size:float * float) : Guid =
        View.addLayout size title

    ///<summary>Adds new named construction plane to the document</summary>
    ///<param name="cplane_name">(string) the name of the new named construction plane</param>
    ///<param name="view">(Guid) Optional, Default Value:None, Title or identifier of the view from which to save
    ///               the construction plane. If omitted, the current active view is used.</param>
    ///
    ///<returns>(string) name of the newly created construction plane</returns>
    static member AddNamedCPlane(cplane_name:string, [<OPT;DEF(null)>]view:Guid) : string =
        View.addNamedCPlane view cplane_name

    ///<summary>Adds a new named view to the document</summary>
    ///<param name="name">(string) the name of the new named view</param>
    ///<param name="view">(Guid) Optional, Default Value:None, the title or identifier of the view to save. If omitted, the current
    ///            active view is saved</param>
    ///
    ///<returns>(string) name fo the newly created named view</returns>
    static member AddNamedView(name:string, [<OPT;DEF(null)>]view:Guid) : string =
        View.addNamedView view name

    ///<summary>Removes a named construction plane from the document</summary>
    ///<param name="name">(string) name of the construction plane to remove</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteNamedCPlane(name:string) : bool =
        View.deleteNamedCPlane name

    ///<summary>Removes a named view from the document</summary>
    ///<param name="name">(string) name of the named view to remove</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteNamedView(name:string) : bool =
        View.deleteNamedView name

    ///<summary>Verifies that a detail view exists on a page layout view</summary>
    ///<param name="layout">(string) title or identifier of an existing page layout</param>
    ///<param name="detail">(string) title or identifier of an existing detail view</param>
    ///
    ///<returns>(bool) True if detail is a detail view, False if detail is not a detail view</returns>
    static member IsDetail(layout:string, detail:string) : bool =
        View.isDetail detail layout

    ///<summary>Verifies that a view is a page layout view</summary>
    ///<param name="layout">(Guid) title or identifier of an existing page layout view</param>
    ///
    ///<returns>(bool) True if layout is a page layout view, False is layout is a standard model view</returns>
    static member IsLayout(layout:Guid) : bool =
        View.isLayout layout

    ///<summary>Verifies that the specified view exists</summary>
    ///<param name="view">(string) title or identifier of the view</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member IsView(view:string) : bool =
        View.isView view

    ///<summary>Verifies that the specified view is the current, or active view</summary>
    ///<param name="view">(string) title or identifier of the view</param>
    ///
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member IsViewCurrent(view:string) : bool =
        View.isViewCurrent view

    ///<summary>Verifies that the specified view is maximized (enlarged so as to fill
    ///    the entire Rhino window)</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or identifier of the view. If omitted, the current
    ///            view is used</param>
    ///
    ///<returns>(bool) True of False</returns>
    static member IsViewMaximized([<OPT;DEF(null)>]view:string) : bool =
        View.isViewMaximized view

    ///<summary>Verifies that the specified view's projection is set to perspective</summary>
    ///<param name="view">(string) title or identifier of the view</param>
    ///
    ///<returns>(bool) True of False</returns>
    static member IsViewPerspective(view:string) : bool =
        View.isViewPerspective view

    ///<summary>Verifies that the specified view's title window is visible</summary>
    ///<param name="view">(string) Optional, Default Value:None, The title or identifier of the view. If omitted, the current
    ///            active view is used</param>
    ///
    ///<returns>(bool) True of False</returns>
    static member IsViewTitleVisible([<OPT;DEF(null)>]view:string) : bool =
        View.isViewTitleVisible view

    ///<summary>Verifies that the specified view contains a wallpaper image</summary>
    ///<param name="view">(string) view to verify</param>
    ///
    ///<returns>(bool) True or False</returns>
    static member IsWallpaper(view:string) : bool =
        View.isWallpaper view

    ///<summary>Toggles a view's maximized/restore window state of the specified view</summary>
    ///<param name="view">(string) Optional, Default Value:None, the title or identifier of the view. If omitted, the current
    ///            active view is used</param>
    ///
    ///<returns>(unit) </returns>
    static member MaximizeRestoreView([<OPT;DEF(null)>]view:string) : unit =
        View.maximizeRestoreView view

    ///<summary>Returns the plane geometry of the specified named construction plane</summary>
    ///<param name="name">(string) the name of the construction plane</param>
    ///
    ///<returns>(Plane) a plane on success</returns>
    static member NamedCPlane(name:string) : Plane =
        View.namedCPlane name

    ///<summary>Returns the names of all named construction planes in the document</summary>
    ///
    ///<returns>(string seq) the names of all named construction planes in the document</returns>
    static member NamedCPlanes() : string seq =
        View.namedCPlanes ()

    ///<summary>Returns the names of all named views in the document</summary>
    ///
    ///<returns>(string seq) the names of all named views in the document</returns>
    static member NamedViews() : string seq =
        View.namedViews ()

    ///<summary>Changes the title of the specified view</summary>
    ///<param name="old_title">(string) the title or identifier of the view to rename</param>
    ///<param name="new_title">(string) the new title of the view</param>
    ///
    ///<returns>(unit) unit</returns>
    static member RenameView(old_title:string, new_title:string) : unit =
        View.renameView new_title old_title

    ///<summary>Restores a named construction plane to the specified view.</summary>
    ///<param name="cplane_name">(string) name of the construction plane to restore</param>
    ///<param name="view">(string) Optional, Default Value:None, the title or identifier of the view. If omitted, the current
    ///            active view is used</param>
    ///
    ///<returns>(string) name of the restored named construction plane</returns>
    static member RestoreNamedCPlane(cplane_name:string, [<OPT;DEF(null)>]view:string) : string =
        View.restoreNamedCPlane view cplane_name

    ///<summary>Restores a named view to the specified view</summary>
    ///<param name="named_view">(string) name of the named view to restore</param>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view to restore the named view.
    ///           If omitted, the current active view is used</param>
    ///<param name="restore_bitmap">(bool) Optional, Default Value:False, restore the named view's background bitmap</param>
    ///
    ///<returns>(string) name of the restored view</returns>
    static member RestoreNamedView(named_view:string, [<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]restore_bitmap:bool) : string =
        View.restoreNamedView restore_bitmap view named_view

    ///<summary>Rotates a perspective-projection view's camera. See the RotateCamera
    ///    command in the Rhino help file for more details</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, current active view is used</param>
    ///<param name="direction">(float) Optional, Default Value:0, the direction to rotate the camera where
    ///        0=right
    ///        1=left
    ///        2=down
    ///        3=up</param>
    ///<param name="angle">(float) Optional, Default Value:None, the angle to rotate. If omitted, the angle of rotation
    ///            is specified by the "Increment in divisions of a circle" parameter
    ///            specified in Options command's View tab</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RotateCamera([<OPT;DEF(null)>]view:string, [<OPT;DEF(0)>]direction:float, [<OPT;DEF(null)>]angle:float) : bool =
        View.rotateCamera angle direction view

    ///<summary>Rotates a view. See RotateView command in Rhino help for more information</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, the current active view is used</param>
    ///<param name="direction">(float) Optional, Default Value:0, the direction to rotate the view where
    ///            0=right
    ///            1=left
    ///            2=down
    ///            3=up</param>
    ///<param name="angle">(float) Optional, Default Value:None, angle to rotate. If omitted, the angle of rotation is specified
    ///            by the "Increment in divisions of a circle" parameter specified in
    ///            Options command's View tab</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member RotateView([<OPT;DEF(null)>]view:string, [<OPT;DEF(0)>]direction:float, [<OPT;DEF(null)>]angle:float) : bool =
        View.rotateView angle direction view

    ///<summary>Shows or hides the title window of a view</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, the current active view is used</param>
    ///<param name="show">(bool) Optional, Default Value:True, The state to set.</param>
    ///
    ///<returns>(unit) </returns>
    static member ShowViewTitle([<OPT;DEF(null)>]view:string, [<OPT;DEF(true)>]show:bool) : unit =
        View.showViewTitle show view

    ///<summary>Tilts a view by rotating the camera up vector. See the TiltView command in
    ///    the Rhino help file for more details.</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, the current active view is used</param>
    ///<param name="direction">(float) Optional, Default Value:0, the direction to rotate the view where
    ///        0=right
    ///        1=left</param>
    ///<param name="angle">(float) Optional, Default Value:None, the angle to rotate. If omitted, the angle of rotation is
    ///        specified by the "Increment in divisions of a circle" parameter specified
    ///        in Options command's View tab</param>
    ///
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member TiltView([<OPT;DEF(null)>]view:string, [<OPT;DEF(0)>]direction:float, [<OPT;DEF(null)>]angle:float) : bool =
        View.tiltView angle direction view

    ///<summary>Returns the orientation of a view's camera.</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, the current active view is used</param>
    ///
    ///<returns>(Plane) the view's camera plane</returns>
    static member ViewCameraPlane([<OPT;DEF(null)>]view:string) : Plane =
        View.viewCameraPlane view

    ///<summary>Return id of a display mode given it's name</summary>
    ///<param name="name">(string) name of the display mode</param>
    ///
    ///<returns>(Guid) The id of the display mode , otherwise None</returns>
    static member ViewDisplayModeId(name:string) : Guid =
        View.viewDisplayModeId name

    ///<summary>Return name of a display mode given it's id</summary>
    ///<param name="mode_id">(Guid) The identifier of the display mode obtained from the ViewDisplayModes method.</param>
    ///
    ///<returns>(string) The name of the display mode , otherwise None</returns>
    static member ViewDisplayModeName(mode_id:Guid) : string =
        View.viewDisplayModeName mode_id

    ///<summary>Return list of display modes</summary>
    ///<param name="return_names">(bool) Optional, Default Value:True, If True, return mode names. If False, return ids</param>
    ///
    ///<returns>(string seq) strings identifying the display mode names or identifiers</returns>
    static member ViewDisplayModes([<OPT;DEF(true)>]return_names:bool) : string seq =
        View.viewDisplayModes return_names

    ///<summary>Return the names, titles, or identifiers of all views in the document</summary>
    ///<param name="return_names">(bool) Optional, Default Value:True, if True then the names of the views are returned.
    ///        If False, then the identifiers of the views are returned</param>
    ///<param name="view_type">(int) Optional, Default Value:0, the type of view to return
    ///                       0 = standard model views
    ///                       1 = page layout views
    ///                       2 = both standard and page layout views</param>
    ///
    ///<returns>(string seq) of the view names or identifiers on success</returns>
    static member ViewNames([<OPT;DEF(true)>]return_names:bool, [<OPT;DEF(0)>]view_type:int) : string seq =
        View.viewNames view_type return_names

    ///<summary>Return 3d corners of a view's near clipping plane rectangle. Useful
    ///    in determining the "real world" size of a parallel-projected view</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, current active view is used</param>
    ///
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four Point3d that define the corners of the rectangle (counter-clockwise order)</returns>
    static member ViewNearCorners([<OPT;DEF(null)>]view:string) : Point3d * Point3d * Point3d * Point3d =
        View.viewNearCorners view

    ///<summary>Returns the width and height in pixels of the specified view</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, current active view is used</param>
    ///
    ///<returns>(float * float) of two numbers identifying width and height</returns>
    static member ViewSize([<OPT;DEF(null)>]view:string) : float * float =
        View.viewSize view

    ///<summary>Test's Rhino's display performance</summary>
    ///<param name="view">(string) Optional, Default Value:None, The title or identifier of the view.  If omitted, the current active view is used</param>
    ///<param name="frames">(float) Optional, Default Value:100, The number of frames, or times to regenerate the view. If omitted, the view will be regenerated 100 times.</param>
    ///<param name="freeze">(bool) Optional, Default Value:True, If True (Default), then Rhino's display list will not be updated with every frame redraw. If False, then Rhino's display list will be updated with every frame redraw.</param>
    ///<param name="direction">(float) Optional, Default Value:0, The direction to rotate the view. The default direction is Right (0). Modes:
    ///        0 = Right
    ///        1 = Left
    ///        2 = Down
    ///        3 = Up.</param>
    ///<param name="angle_degrees">(int) Optional, Default Value:5, The angle to rotate. If omitted, the rotation angle of 5.0 degrees will be used.</param>
    ///
    ///<returns>(float) The number of seconds it took to regenerate the view frames number of times,</returns>
    static member ViewSpeedTest([<OPT;DEF(null)>]view:string, [<OPT;DEF(100)>]frames:float, [<OPT;DEF(true)>]freeze:bool, [<OPT;DEF(0)>]direction:float, [<OPT;DEF(5)>]angle_degrees:int) : float =
        View.viewSpeedTest angle_degrees direction freeze frames view

    ///<summary>Returns the name, or title, of a given view's identifier</summary>
    ///<param name="view_id">(string) The identifier of the view</param>
    ///
    ///<returns>(string) name or title of the view on success</returns>
    static member ViewTitle(view_id:string) : string =
        View.viewTitle view_id

    ///<summary>Zooms to the extents of a specified bounding box in the specified view</summary>
    ///<param name="bounding_box">(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) eight points that define the corners
    ///        of a bounding box or a BoundingBox class instance</param>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value:False, zoom extents in all views</param>
    ///
    ///<returns>(unit) </returns>
    static member ZoomBoundingBox(bounding_box:Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d, [<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        View.zoomBoundingBox all view bounding_box

    ///<summary>Zooms to extents of visible objects in the specified view</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value:False, zoom extents in all views</param>
    ///
    ///<returns>(unit) </returns>
    static member ZoomExtents([<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        View.zoomExtents all view

    ///<summary>Zoom to extents of selected objects in a view</summary>
    ///<param name="view">(string) Optional, Default Value:None, title or id of the view. If omitted, active view is used</param>
    ///<param name="all">(bool) Optional, Default Value:False, zoom extents in all views</param>
    ///
    ///<returns>(unit) </returns>
    static member ZoomSelected([<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        View.zoomSelected all view

