namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.ApplicationSettings
open Rhino.Commands
open System.Windows.Forms

[<AutoOpen>]
module ExtensionsApplication =
  
  let mutable internal commandSerialNumbers = None // to store last created object form executing a rs.Command(...)
  
  type RhinoScriptSyntax with
    
    ///<summary>Add new command alias to Rhino. Command aliases can be added manually by
    ///  using Rhino's Options command and modifying the contents of the Aliases tab.</summary>
    ///<param name="alias">(string) Name of new command alias. Cannot match command names or existing
    ///  aliases.</param>
    ///<param name="macro">(string) The macro to run when the alias is executed.</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member AddAlias(alias:string, macro:string) : bool =
        ApplicationSettings.CommandAliasList.Add(alias, macro)
    (*
    def AddAlias(alias, macro):
        '''Add new command alias to Rhino. Command aliases can be added manually by
        using Rhino's Options command and modifying the contents of the Aliases tab.
        Parameters:
          alias (str): Name of new command alias. Cannot match command names or existing
                  aliases.
          macro (str): The macro to run when the alias is executed.
        Returns:
          bool: True or False indicating success or failure.
        '''
        return Rhino.ApplicationSettings.CommandAliasList.Add(alias, macro)
    *)


    ///<summary>Add new path to Rhino's search path list. Search paths can be added by
    ///  using Rhino's Options command and modifying the contents of the files tab.</summary>
    ///<param name="folder">(string) A valid folder, or path, to add.</param>
    ///<param name="index">(int) Optional, Default Value: <c>-1</c>
    ///  Zero-based position in the search path list to insert. 
    ///  If omitted, path will be appended to the end of the search path list. 
    ///<returns>(int)The index where the item was inserted.
    ///  -1 on failure.</returns>
    static member AddSearchPath(folder:string, [<OPT;DEF(-1)>]index:int) : int =
        ApplicationSettings.FileSettings.AddSearchPath(folder, index)
    (*
    def AddSearchPath(folder, index=-1):
        '''Add new path to Rhino's search path list. Search paths can be added by
        using Rhino's Options command and modifying the contents of the files tab.
        Parameters:
          folder (str): A valid folder, or path, to add.
          index (number, optional): Zero-based position in the search path list to insert.
                                 If omitted, path will be appended to the end of the
                                 search path list.
        Returns:
          int: The index where the item was inserted if success.
               -1 on failure.
        '''
        return Rhino.ApplicationSettings.FileSettings.AddSearchPath(folder, index)
    *)


    ///<summary>Returns number of command aliases in Rhino.</summary>
    ///<returns>(int) the number of command aliases in Rhino.</returns>
    static member AliasCount() : int =
        ApplicationSettings.CommandAliasList.Count
    (*
    def AliasCount():
        '''Returns number of command aliases in Rhino.
        Returns:
          number: the number of command aliases in Rhino.
        '''
        return Rhino.ApplicationSettings.CommandAliasList.Count
    *)


    ///<summary>Returns the macro of a command alias.</summary>
    ///<param name="alias">(string) The name of an existing command alias.</param>
    ///<returns>(string) The existing macro .</returns>
    static member AliasMacro(alias:string) : string = //GET
        ApplicationSettings.CommandAliasList.GetMacro(alias)
    (*
    def AliasMacro(alias, macro=None):
        '''Returns or modifies the macro of a command alias.
        Parameters:
          alias (str): The name of an existing command alias.
          macro (str, optional): The new macro to run when the alias is executed. If omitted, the current alias macro is returned.
        Returns:
          str:  If a new macro is not specified, the existing macro if successful.
          str:  If a new macro is specified, the previous macro if successful.
          null:  None on error
        '''
        rc = Rhino.ApplicationSettings.CommandAliasList.GetMacro(alias)
        if macro:
            Rhino.ApplicationSettings.CommandAliasList.SetMacro(alias, macro)
        if rc is None: return scriptcontext.errorhandler()
        return rc
    *)

    ///<summary>Modifies the macro of a command alias.</summary>
    ///<param name="alias">(string) The name of an existing command alias.</param>
    ///<param name="macro">(string)The new macro to run when the alias is executed. If omitted, the current alias macro is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member AliasMacro(alias:string, macro:string) : unit = //SET
        ApplicationSettings.CommandAliasList.SetMacro(alias, macro)
        |> ignore
    (*
    def AliasMacro(alias, macro=None):
        '''Returns or modifies the macro of a command alias.
        Parameters:
          alias (str): The name of an existing command alias.
          macro (str, optional): The new macro to run when the alias is executed. If omitted, the current alias macro is returned.
        Returns:
          str:  If a new macro is not specified, the existing macro if successful.
          str:  If a new macro is specified, the previous macro if successful.
          null:  None on error
        '''
        rc = Rhino.ApplicationSettings.CommandAliasList.GetMacro(alias)
        if macro:
            Rhino.ApplicationSettings.CommandAliasList.SetMacro(alias, macro)
        if rc is None: return scriptcontext.errorhandler()
        return rc
    *)


    ///<summary>Returns a array of command alias names.</summary>
    ///<returns>(string array) a array of command alias names.</returns>
    static member AliasNames() : string [] =
        ApplicationSettings.CommandAliasList.GetNames()
    (*
    def AliasNames():
        '''Returns a list of command alias names.
        Returns:
          list(str, ...): a list of command alias names.
        '''
        return Rhino.ApplicationSettings.CommandAliasList.GetNames()
    *)


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
    ///<returns>(Drawing.Color) The current item color.
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
    static member AppearanceColor(item:int) : Drawing.Color = //GET
        if   item=0 then AppearanceSettings.ViewportBackgroundColor  
        elif item=1 then AppearanceSettings.GridThickLineColor  
        elif item=2 then AppearanceSettings.GridThinLineColor  
        elif item=3 then AppearanceSettings.GridXAxisLineColor  
        elif item=4 then AppearanceSettings.GridYAxisLineColor  
        elif item=5 then AppearanceSettings.SelectedObjectColor  
        elif item=6 then AppearanceSettings.LockedObjectColor  
        elif item=7 then AppearanceSettings.DefaultLayerColor  
        elif item=8 then AppearanceSettings.FeedbackColor  
        elif item=9 then  AppearanceSettings.TrackingColor  
        elif item=10 then AppearanceSettings.CrosshairColor  
        elif item=11 then AppearanceSettings.CommandPromptTextColor  
        elif item=12 then AppearanceSettings.CommandPromptBackgroundColor  
        elif item=13 then AppearanceSettings.CommandPromptHypertextColor  
        else failwithf "getAppearanceColor: item %d is out of range" item
    (*
    def AppearanceColor(item, color=None):
        '''Returns or modifies an application interface item's color.
        Parameters:
          item (number): Item number to either query or modify
                 0  = View background
                 1  = Major grid line
                 2  = Minor grid line
                 3  = X-Axis line
                 4  = Y-Axis line
                 5  = Selected Objects
                 6  = Locked Objects
                 7  = New layers
                 8  = Feedback
                 9  = Tracking
                 10 = Crosshair
                 11 = Text
                 12 = Text Background
                 13 = Text hover
          color ([r255,g255,b255], optional): The new color value in (r255,g255,b255). If omitted, the current item color is returned.
        Returns:
          color (r255,g255,b255): if color is not specified, the current item color.
          color (r255,g255,b255): if color is specified, the previous item color.
        '''
        rc = None
        color = rhutil.coercecolor(color)
        appearance = Rhino.ApplicationSettings.AppearanceSettings
        if item==0:
            rc = appearance.ViewportBackgroundColor
            if color: appearance.ViewportBackgroundColor = color
        elif item==1:
            rc = appearance.GridThickLineColor
            if color: appearance.GridThickLineColor = color
        elif item==2:
            rc = appearance.GridThinLineColor
            if color: appearance.GridThinLineColor = color
        elif item==3:
            rc = appearance.GridXAxisLineColor
            if color: appearance.GridXAxisLineColor = color
        elif item==4:
            rc = appearance.GridYAxisLineColor
            if color: appearance.GridYAxisLineColor = color
        elif item==5:
            rc = appearance.SelectedObjectColor
            if color: appearance.SelectedObjectColor = color
        elif item==6:
            rc = appearance.LockedObjectColor
            if color: appearance.LockedObjectColor = color
        elif item==7:
            rc = appearance.DefaultLayerColor
            if color: appearance.DefaultLayerColor = color
        elif item==8:
            rc = appearance.FeedbackColor
            if color: appearance.FeedbackColor = color
        elif item==9:
            rc = appearance.TrackingColor
            if color: appearance.TrackingColor = color
        elif item==10:
            rc = appearance.CrosshairColor
            if color: appearance.CrosshairColor = color
        elif item==11:
            rc = appearance.CommandPromptTextColor
            if color: appearance.CommandPromptTextColor = color
        elif item==12:
            rc = appearance.CommandPromptBackgroundColor
            if color: appearance.CommandPromptBackgroundColor = color
        elif item==13:
            rc = appearance.CommandPromptHypertextColor
            if color: appearance.CommandPromptHypertextColor = color
        if rc is None: raise ValueError("item is out of range")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)

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
    ///<param name="color">(Drawing.Color )The new color value as System.Drawing.Color . </param>
    ///<returns>(unit) void, nothing</returns>
    static member AppearanceColor(item:int, color:Drawing.Color) : unit = //SET
        if item=0 then AppearanceSettings.ViewportBackgroundColor <- color
        elif item=1 then AppearanceSettings.GridThickLineColor <- color
        elif item=2 then AppearanceSettings.GridThinLineColor <- color
        elif item=3 then AppearanceSettings.GridXAxisLineColor <- color
        elif item=4 then AppearanceSettings.GridYAxisLineColor <- color
        elif item=5 then AppearanceSettings.SelectedObjectColor <- color
        elif item=6 then AppearanceSettings.LockedObjectColor <- color
        elif item=7 then AppearanceSettings.DefaultLayerColor <- color
        elif item=8 then AppearanceSettings.FeedbackColor <- color
        elif item=9 then AppearanceSettings.TrackingColor <- color
        elif item=10 then AppearanceSettings.CrosshairColor <- color
        elif item=11 then AppearanceSettings.CommandPromptTextColor <- color
        elif item=12 then AppearanceSettings.CommandPromptBackgroundColor <- color
        elif item=13 then AppearanceSettings.CommandPromptHypertextColor <- color
        else failwithf "setAppearanceColor: item %d is out of range" item
        Doc.Views.Redraw()





    (*
    def AppearanceColor(item, color=None):
        '''Returns or modifies an application interface item's color.
        Parameters:
          item (number): Item number to either query or modify
                 0  = View background
                 1  = Major grid line
                 2  = Minor grid line
                 3  = X-Axis line
                 4  = Y-Axis line
                 5  = Selected Objects
                 6  = Locked Objects
                 7  = New layers
                 8  = Feedback
                 9  = Tracking
                 10 = Crosshair
                 11 = Text
                 12 = Text Background
                 13 = Text hover
          color ([r255,g255,b255], optional): The new color value in (r255,g255,b255). If omitted, the current item color is returned.
        Returns:
          color (r255,g255,b255): if color is not specified, the current item color.
          color (r255,g255,b255): if color is specified, the previous item color.
        '''
        rc = None
        color = rhutil.coercecolor(color)
        appearance = Rhino.ApplicationSettings.AppearanceSettings
        if item==0:
            rc = appearance.ViewportBackgroundColor
            if color: appearance.ViewportBackgroundColor = color
        elif item==1:
            rc = appearance.GridThickLineColor
            if color: appearance.GridThickLineColor = color
        elif item==2:
            rc = appearance.GridThinLineColor
            if color: appearance.GridThinLineColor = color
        elif item==3:
            rc = appearance.GridXAxisLineColor
            if color: appearance.GridXAxisLineColor = color
        elif item==4:
            rc = appearance.GridYAxisLineColor
            if color: appearance.GridYAxisLineColor = color
        elif item==5:
            rc = appearance.SelectedObjectColor
            if color: appearance.SelectedObjectColor = color
        elif item==6:
            rc = appearance.LockedObjectColor
            if color: appearance.LockedObjectColor = color
        elif item==7:
            rc = appearance.DefaultLayerColor
            if color: appearance.DefaultLayerColor = color
        elif item==8:
            rc = appearance.FeedbackColor
            if color: appearance.FeedbackColor = color
        elif item==9:
            rc = appearance.TrackingColor
            if color: appearance.TrackingColor = color
        elif item==10:
            rc = appearance.CrosshairColor
            if color: appearance.CrosshairColor = color
        elif item==11:
            rc = appearance.CommandPromptTextColor
            if color: appearance.CommandPromptTextColor = color
        elif item==12:
            rc = appearance.CommandPromptBackgroundColor
            if color: appearance.CommandPromptBackgroundColor = color
        elif item==13:
            rc = appearance.CommandPromptHypertextColor
            if color: appearance.CommandPromptHypertextColor = color
        if rc is None: raise ValueError("item is out of range")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    ///<summary>Returns the file name used by Rhino's automatic file saving</summary>
    ///<returns>(string) The name of the current autosave file</returns>
    static member AutosaveFile() : string = //GET
        ApplicationSettings.FileSettings.AutoSaveFile
    (*
    def AutosaveFile(filename=None):
        '''Returns or changes the file name used by Rhino's automatic file saving
        Parameters:
          filename (str, optional): Name of the new autosave file
        Returns:
          str: if filename is not specified, the name of the current autosave file
          str: if filename is specified, the name of the previous autosave file
        '''
        rc = Rhino.ApplicationSettings.FileSettings.AutoSaveFile
        if filename: Rhino.ApplicationSettings.FileSettings.AutoSaveFile = filename
        return rc
    *)

    ///<summary>Changes the file name used by Rhino's automatic file saving</summary>
    ///<param name="filename">(string)Name of the new autosave file</param>
    ///<returns>(unit) void, nothing</returns>
    static member AutosaveFile(filename:string) : unit = //SET
        ApplicationSettings.FileSettings.AutoSaveFile <- filename
    (*
    def AutosaveFile(filename=None):
        '''Returns or changes the file name used by Rhino's automatic file saving
        Parameters:
          filename (str, optional): Name of the new autosave file
        Returns:
          str: if filename is not specified, the name of the current autosave file
          str: if filename is specified, the name of the previous autosave file
        '''
        rc = Rhino.ApplicationSettings.FileSettings.AutoSaveFile
        if filename: Rhino.ApplicationSettings.FileSettings.AutoSaveFile = filename
        return rc
    *)


    ///<summary>Returns how often the document will be saved when Rhino's
    /// automatic file saving mechanism is enabled</summary>
    ///<returns>(float) The current interval in minutes</returns>
    static member AutosaveInterval() : float = //GET
        ApplicationSettings.FileSettings.AutoSaveInterval.TotalMinutes
    (*
    def AutosaveInterval(minutes=None):
        '''Returns or changes how often the document will be saved when Rhino's
        automatic file saving mechanism is enabled
        Parameters:
          minutes (number, optional): The number of minutes between saves
        Returns:
          number: if minutes is not specified, the current interval in minutes
          number: if minutes is specified, the previous interval in minutes
        '''
        rc = Rhino.ApplicationSettings.FileSettings.AutoSaveInterval.TotalMinutes
        if minutes:
            timespan = System.TimeSpan.FromMinutes(minutes)
            Rhino.ApplicationSettings.FileSettings.AutoSaveInterval = timespan
        return rc
    *)

    ///<summary>Changes how often the document will be saved when Rhino's
    /// automatic file saving mechanism is enabled</summary>
    ///<param name="minutes">(float)The number of minutes between saves</param>
    ///<returns>(unit) void, nothing</returns>
    static member AutosaveInterval(minutes:float) : unit = //SET
        ApplicationSettings.FileSettings.AutoSaveInterval <- TimeSpan.FromMinutes(minutes)
    (*
    def AutosaveInterval(minutes=None):
        '''Returns or changes how often the document will be saved when Rhino's
        automatic file saving mechanism is enabled
        Parameters:
          minutes (number, optional): The number of minutes between saves
        Returns:
          number: if minutes is not specified, the current interval in minutes
          number: if minutes is specified, the previous interval in minutes
        '''
        rc = Rhino.ApplicationSettings.FileSettings.AutoSaveInterval.TotalMinutes
        if minutes:
            timespan = System.TimeSpan.FromMinutes(minutes)
            Rhino.ApplicationSettings.FileSettings.AutoSaveInterval = timespan
        return rc
    *)


    ///<summary>Returns the build date of Rhino</summary>
    ///<returns>(DateTime) the build date of Rhino. Will be converted to a string by most functions.</returns>
    static member BuildDate() : DateTime =
        RhinoApp.BuildDate
    (*
    def BuildDate():
        '''Returns the build date of Rhino
        Returns:
          Datetime.date: the build date of Rhino. Will be converted to a string by most functions.
        '''
        build = RhinoApp.BuildDate
        return datetime.date(build.Year, build.Month, build.Day)
    *)


    ///<summary>Clears contents of Rhino's command history window. You can view the
    ///  command history window by using the CommandHistory command in Rhino.</summary>
    ///<returns>(unit) </returns>
    static member ClearCommandHistory() : unit =
        RhinoApp.ClearCommandHistoryWindow()
    (*
    def ClearCommandHistory():
        '''Clears contents of Rhino's command history window. You can view the
        command history window by using the CommandHistory command in Rhino.
        Returns:
          none
        '''
        RhinoApp.ClearCommandHistoryWindow()
    __command_serial_numbers = None
    *)


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
        let start = DocObjects.RhinoObject.NextRuntimeSerialNumber
        let rc = RhinoApp.RunScript(commandString, echo)
        let ende = DocObjects.RhinoObject.NextRuntimeSerialNumber
        commandSerialNumbers <- None
        if start<>ende then  commandSerialNumbers <- Some(start,ende)
        rc
    (*
    def Command(commandString, echo=True):
        '''Runs a Rhino command script. All Rhino commands can be used in command
        scripts. The command can be a built-in Rhino command or one provided by a
        3rd party plug-in.
        
        Write command scripts just as you would type the command sequence at the
        command line. A space or a new line acts like pressing <Enter> at the
        command line. For more information, see "Scripting" in Rhino help.
        Note, this function is designed to run one command and one command only.
        Do not combine multiple Rhino commands into a single call to this method.
          WRONG:
            rs.Command("_Line _SelLast _Invert")
          CORRECT:
            rs.Command("_Line")
            rs.Command("_SelLast")
            rs.Command("_Invert")
        Also, the exclamation point and space character ( ! ) combination used by
        button macros and batch-driven scripts to cancel the previous command is
        not valid.
          WRONG:
            rs.Command("! _Line _Pause _Pause")
          CORRECT:
            rs.Command("_Line _Pause _Pause")
        After the command script has run, you can obtain the identifiers of most
        recently created or changed object by calling LastCreatedObjects.
        
        Parameters:
          commandString (str): A Rhino command including any arguments
          echo (bool, optional): The command echo mode True will display the commands on the commandline. If omitted, command prompts are echoed (True)
        Returns:
          bool: True or False indicating success or failure
        
        '''
        start = DocObjects.RhinoObject.NextRuntimeSerialNumber
        rc = RhinoApp.RunScript(commandString, echo)
        end = DocObjects.RhinoObject.NextRuntimeSerialNumber
        global __command_serial_numbers
        __command_serial_numbers = None
        if start!=end: __command_serial_numbers = (start,end)
        return rc
    *)


    ///<summary>Returns the contents of Rhino's command history window</summary>
    ///<returns>(string) the contents of Rhino's command history window</returns>
    static member CommandHistory() : string =
        RhinoApp.CommandHistoryWindowText
    (*
    def CommandHistory():
        '''Returns the contents of Rhino's command history window
        Returns:
          str: the contents of Rhino's command history window
        '''
        return RhinoApp.CommandHistoryWindowText
    *)


    ///<summary>Returns the default render plug-in</summary>
    ///<returns>(string) Name of default renderer</returns>
    static member DefaultRenderer() : string = //GET
        let mutable id = Render.Utilities.DefaultRenderPlugInId
        let mutable plugins = PlugIns.PlugIn.GetInstalledPlugIns()
        plugins.[id]
    (*
    def DefaultRenderer(renderer=None):
        '''Returns or changes the default render plug-in
        Parameters:
          renderer (str, optional): The name of the renderer to set as default renderer.  If omitted the Guid of the current renderer is returned.
        Returns:
          guid: Unique identifier of default renderer
          guid: Unique identifier of default renderer
        '''
        id = Rhino.Render.Utilities.DefaultRenderPlugInId
        plugins = Rhino.PlugIns.PlugIn.GetInstalledPlugIns()
        rc = plugins[id]
        if renderer:
            id = Rhino.PlugIns.PlugIn.IdFromName(renderer)
            Rhino.Render.Utilities.SetDefaultRenderPlugIn(id)
        return rc
    *)

    ///<summary>Changes the default render plug-in</summary>
    ///<param name="renderer">(string)The name of the renderer to set as default renderer. </param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DefaultRenderer(renderer:string) : bool = //SET
        let id = Rhino.PlugIns.PlugIn.IdFromName(renderer)
        Rhino.Render.Utilities.SetDefaultRenderPlugIn(id)
    (*
    def DefaultRenderer(renderer=None):
        '''Returns or changes the default render plug-in
        Parameters:
          renderer (str, optional): The name of the renderer to set as default renderer.  If omitted the Guid of the current renderer is returned.
        Returns:
          guid: Unique identifier of default renderer
          guid: Unique identifier of default renderer
        '''
        id = Rhino.Render.Utilities.DefaultRenderPlugInId
        plugins = Rhino.PlugIns.PlugIn.GetInstalledPlugIns()
        rc = plugins[id]
        if renderer:
            id = Rhino.PlugIns.PlugIn.IdFromName(renderer)
            Rhino.Render.Utilities.SetDefaultRenderPlugIn(id)
        return rc
    *)


    ///<summary>Delete an existing alias from Rhino.</summary>
    ///<param name="alias">(string) The name of an existing alias.</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteAlias(alias:string) : bool =
        ApplicationSettings.CommandAliasList.Delete(alias)
    (*
    def DeleteAlias(alias):
        '''Delete an existing alias from Rhino.
        Parameters:
          alias (str): The name of an existing alias.
        Returns:
          bool: True or False indicating success
        '''
        return Rhino.ApplicationSettings.CommandAliasList.Delete(alias)
    *)


    ///<summary>Removes existing path from Rhino's search path list. Search path items
    ///  can be removed manually by using Rhino's options command and modifying the
    ///  contents of the files tab</summary>
    ///<param name="folder">(string) A folder to remove</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteSearchPath(folder:string) : bool =
        ApplicationSettings.FileSettings.DeleteSearchPath(folder)
    (*
    def DeleteSearchPath(folder):
        '''Removes existing path from Rhino's search path list. Search path items
        can be removed manually by using Rhino's options command and modifying the
        contents of the files tab
        Parameters:
          folder (str): A folder to remove
        Returns:
          bool: True or False indicating success
        '''
        return Rhino.ApplicationSettings.FileSettings.DeleteSearchPath(folder)
    *)


    ///<summary>Enables/disables OLE Server Busy/Not Responding dialog boxes</summary>
    ///<param name="enable">(bool) Whether alerts should be visible (True or False)</param>
    ///<returns>(unit) </returns>
    static member DisplayOleAlerts(enable:bool) : unit =
        Rhino.Runtime.HostUtils.DisplayOleAlerts( enable )
    (*
    def DisplayOleAlerts(enable):
        '''Enables/disables OLE Server Busy/Not Responding dialog boxes
        Parameters:
          enable (bool): Whether alerts should be visible (True or False)
        Returns:
          none
        '''
        Rhino.Runtime.HostUtils.DisplayOleAlerts( enable )
    *)


    ///<summary>Returns edge analysis color displayed by the ShowEdges command</summary>
    ///<returns>(Drawing.Color) The current edge analysis color</returns>
    static member EdgeAnalysisColor() : Drawing.Color= //GET
        ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor
    (*
    def EdgeAnalysisColor(color=None):
        '''Returns or modifies edge analysis color displayed by the ShowEdges command
        Parameters:
          color (tuple (r255,g255,b255), optional): The new color for the analysis.
        Returns:
          tuple (r255,g255,b255): if color is not specified, the current edge analysis color
          tuple (r255,g255,b255): if color is specified, the previous edge analysis color
        '''
        rc = Rhino.ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor
        if color:
            color = rhutil.coercecolor(color, True)
            Rhino.ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor = color
        return rc
    *)

    ///<summary>Modifies edge analysis color displayed by the ShowEdges command</summary>
    ///<param name="color">(Drawing.Color), optional): The new color for the analysis.</param>
    ///<returns>(unit) void, nothing</returns>
    static member EdgeAnalysisColor(color:Drawing.Color) : unit = //SET
        ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor <- color
    (*
    def EdgeAnalysisColor(color=None):
        '''Returns or modifies edge analysis color displayed by the ShowEdges command
        Parameters:
          color (tuple (r255,g255,b255), optional): The new color for the analysis.
        Returns:
          tuple (r255,g255,b255): if color is not specified, the current edge analysis color
          tuple (r255,g255,b255): if color is specified, the previous edge analysis color
        '''
        rc = Rhino.ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor
        if color:
            color = rhutil.coercecolor(color, True)
            Rhino.ApplicationSettings.EdgeAnalysisSettings.ShowEdgeColor = color
        return rc
    *)


    ///<summary>Returns edge analysis mode displayed by the ShowEdges command</summary>
    ///<returns>(int) The current edge analysis mode
    ///  0 - display all edges
    ///  1 - display naked edges</returns>
    static member EdgeAnalysisMode() : int = //GET
        ApplicationSettings.EdgeAnalysisSettings.ShowEdges
    (*
    def EdgeAnalysisMode(mode=None):
        '''Returns or modifies edge analysis mode displayed by the ShowEdges command
        Parameters:
          mode (number, optional): The new display mode. The available modes are
                       0 - display all edges
                       1 - display naked edges
        Returns:
          number: if mode is not specified, the current edge analysis mode
          number: if mode is specified, the previous edge analysis mode
        '''
        rc = Rhino.ApplicationSettings.EdgeAnalysisSettings.ShowEdges
        if mode==1 or mode==2:
            Rhino.ApplicationSettings.EdgeAnalysisSettings.ShowEdges = mode
        return rc
    *)

    ///<summary>Modifies edge analysis mode displayed by the ShowEdges command</summary>
    ///<param name="mode">(int)The new display mode. The available modes are
    ///  0 - display all edges
    ///  1 - display naked edges</param>
    ///<returns>(unit) void, nothing</returns>
    static member EdgeAnalysisMode(mode:int) : unit = //SET
        if mode=1 || mode=2 then 
            ApplicationSettings.EdgeAnalysisSettings.ShowEdges <- mode
        else
            failwithf "bad edge analysisMode %d" mode
    (*
    def EdgeAnalysisMode(mode=None):
        '''Returns or modifies edge analysis mode displayed by the ShowEdges command
        Parameters:
          mode (number, optional): The new display mode. The available modes are
                       0 - display all edges
                       1 - display naked edges
        Returns:
          number: if mode is not specified, the current edge analysis mode
          number: if mode is specified, the previous edge analysis mode
        '''
        rc = Rhino.ApplicationSettings.EdgeAnalysisSettings.ShowEdges
        if mode==1 or mode==2:
            Rhino.ApplicationSettings.EdgeAnalysisSettings.ShowEdges = mode
        return rc
    *)


    ///<summary>Enables or disables Rhino's automatic file saving mechanism</summary>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///The autosave state. If omitted automatic saving is enabled (True)</param>
    ///<returns>(unit) void, nothing</returns>
    static member EnableAutosave([<OPT;DEF(true)>]enable:bool) : unit =
        ApplicationSettings.FileSettings.AutoSaveEnabled <- enable
    (*
    def EnableAutosave(enable=True):
        '''Enables or disables Rhino's automatic file saving mechanism
        Parameters:
          enable (bool, optional): The autosave state. If omitted automatic saving is enabled (True)
        Returns:
          bool: the previous autosave state
        '''
        rc = Rhino.ApplicationSettings.FileSettings.AutoSaveEnabled
        if rc!=enable: Rhino.ApplicationSettings.FileSettings.AutoSaveEnabled = enable
        return rc
    *)


    ///<summary>Get status of a Rhino plug-in</summary>
    ///<param name="plugin">(string) The name of the plugin.</param>
    ///<returns>(bool) True if set to load silently otherwise False</returns>
    static member EnablePlugIn(plugin:string) : bool = //GET
        let id = PlugIns.PlugIn.IdFromName(plugin)
        let rc, loadSilent = PlugIns.PlugIn.GetLoadProtection(id)        
        if rc then loadSilent 
        else failwithf "EnablePlugIn: %s GetLoadProtection failed" plugin
    (*
    def EnablePlugIn(plugin, enable=None):
        '''Enables or disables a Rhino plug-in
          Parameters:
            plugin (guid): The nameof the plugin.
            enable (bool, optional): Load silently if True. If omitted Load silently is False.
          Returns:
            bool: True if set to load silently otherwise False
          '''
        id = rhutil.coerceguid(plugin)
        if not id: id = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        rc, loadSilent = Rhino.PlugIns.PlugIn.GetLoadProtection(id)
        if enable is not None:
            Rhino.PlugIns.PlugIn.SetLoadProtection(id, enable)
        return loadSilent
    *)

    ///<summary>Enables or disables a Rhino plug-in</summary>
    ///<param name="plugin">(string) The name of the plugin.</param>
    ///<param name="enable">(bool) Load silently if True. </param>
    ///<returns>(unit) void, nothing</returns>
    static member EnablePlugIn(plugin:string, enable:bool) : unit = //SET
        let id = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        let rc, loadSilent = Rhino.PlugIns.PlugIn.GetLoadProtection(id)
        if rc then PlugIns.PlugIn.SetLoadProtection(id, enable)
        else failwithf "EnablePlugIn: %s GetLoadProtection failed" plugin
        

    (*
    def EnablePlugIn(plugin, enable=None):
        '''Enables or disables a Rhino plug-in
          Parameters:
            plugin (guid): The nameof the plugin.
            enable (bool, optional): Load silently if True. If omitted Load silently is False.
          Returns:
            bool: True if set to load silently otherwise False
          '''
        id = rhutil.coerceguid(plugin)
        if not id: id = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        rc, loadSilent = Rhino.PlugIns.PlugIn.GetLoadProtection(id)
        if enable is not None:
            Rhino.PlugIns.PlugIn.SetLoadProtection(id, enable)
        return loadSilent
    *)


    ///<summary>Returns the full path to Rhino's executable folder.</summary>
    ///<returns>(string) the full path to Rhino's executable folder.</returns>
    static member ExeFolder() : string =
        ApplicationSettings.FileSettings.ExecutableFolder
    (*
    def ExeFolder():
        '''Returns the full path to Rhino's executable folder.
        Returns:
          str: the full path to Rhino's executable folder.
        '''
        return Rhino.ApplicationSettings.FileSettings.ExecutableFolder
    *)


    ///<summary>Returns the platform of the Rhino executable</summary>
    ///<returns>(string) the platform of the Rhino executable</returns>
    static member ExePlatform() : string =
        if System.Environment.Is64BitProcess then  1 else  0
    (*
    def ExePlatform():
        '''Returns the platform of the Rhino executable
        Returns:
          str: the platform of the Rhino executable
        '''
        if System.Environment.Is64BitProcess: return 1
        return 0
    *)


    ///<summary>Returns the service release number of the Rhino executable</summary>
    ///<returns>(int) the service release number of the Rhino executable</returns>
    static member ExeServiceRelease() : int =
        RhinoApp.ExeServiceRelease
    (*
    def ExeServiceRelease():
        '''Returns the service release number of the Rhino executable
        Returns:
          str: the service release number of the Rhino executable
        '''
        return RhinoApp.ExeServiceRelease
    *)


    ///<summary>Returns the major version number of the Rhino executable</summary>
    ///<returns>(int) the major version number of the Rhino executable</returns>
    static member ExeVersion() : int =
        RhinoApp.ExeVersion
    (*
    def ExeVersion():
        '''Returns the major version number of the Rhino executable
        Returns:
          str: the major version number of the Rhino executable
        '''
        return RhinoApp.ExeVersion
    *)


    ///<summary>Closes the rhino application</summary>
    ///<returns>(unit) </returns>
    static member Exit() : unit =
        RhinoApp.Exit()
    (*
    def Exit():
        '''Closes the rhino application
        Returns:
          none
        '''
        RhinoApp.Exit()
    *)


    ///<summary>Searches for a file using Rhino's search path. Rhino will look for a file in the following locations:
    ///    1. The current document's folder.
    ///    2. Folder's specified in Options dialog, File tab.
    ///    3. Rhino's System folders</summary>
    ///<param name="filename">(string) A short file name to search for</param>
    ///<returns>(string) full path on success</returns>
    static member FindFile(filename:string) : string =
        ApplicationSettings.FileSettings.FindFile(filename)
    (*
    def FindFile(filename):
        '''Searches for a file using Rhino's search path. Rhino will look for a
        file in the following locations:
          1. The current document's folder.
          2. Folder's specified in Options dialog, File tab.
          3. Rhino's System folders
        Parameters:
          filename (str): A short file name to search for
        Returns:
          str: full path on success
        '''
        return Rhino.ApplicationSettings.FileSettings.FindFile(filename)
    *)


    ///<summary>Returns a scriptable object from a specified plug-in. Not all plug-ins
    ///  contain scriptable objects. Check with the manufacturer of your plug-in
    ///  to see if they support this capability.</summary>
    ///<param name="plugIn">(string) The name or Id of a registered plug-in that supports scripting.
    ///  If the plug-in is registered but not loaded, it will be loaded</param>
    ///<returns>(object) a scriptable object</returns>
    static member GetPlugInObject(plugIn:string) : obj =
        RhinoApp.GetPlugInObject(plugIn)
    (*
    def GetPlugInObject(plug_in):
        '''Returns a scriptable object from a specified plug-in. Not all plug-ins
        contain scriptable objects. Check with the manufacturer of your plug-in
        to see if they support this capability.
        Parameters:
          plug_in (str or guid): The name or Id of a registered plug-in that supports scripting.
                                 If the plug-in is registered but not loaded, it will be loaded
        Returns:
          guid: scriptable object if successful
          null: None on error
        '''
        return RhinoApp.GetPlugInObject(plug_in)
    *)


    ///<summary>Determines if Rhino is currently running a command. Because Rhino allows
    ///  for transparent commands (commands run from inside of other commands), this
    ///  method returns the total number of active commands.</summary>
    ///<param name="ignoreRunners">(bool) Optional, Default Value: <c>true</c>
    ///If True, script running commands, such as
    ///  LoadScript, RunScript, and ReadCommandFile will not counted.
    ///  If omitted the default is not to count script running command (True).</param>
    ///<returns>(float) the number of active commands</returns>
    static member InCommand([<OPT;DEF(true)>]ignoreRunners:bool) : float =
        //let inCommand (ignoreRunners:bool) :int =
        //<param name="ignoreRunners">ignoreRunners If True, script running commands, such as
        //        LoadScript, RunScript, and ReadCommandFile will not counted.
        //        If omitted the default is not to count script running command (True).</param>
        Rhino.Commands.Command.GetCommandStack()
        //|> Array.filter (fun i -> if ignoreRunners then  Commands.Command.GetInt(i) <> 1 else true)
        |> Array.length
    (*
    def InCommand(ignore_runners=True):
        '''Determines if Rhino is currently running a command. Because Rhino allows
        for transparent commands (commands run from inside of other commands), this
        method returns the total number of active commands.
        Parameters:
          ignore_runners (bool, optional): If True, script running commands, such as
                                           LoadScript, RunScript, and ReadCommandFile will not counted.
                                           If omitted the default is not to count script running command (True).
        Returns:
          number: the number of active commands
        '''
        ids = rhcommand.GetCommandStack()
        return len(ids)
    *)


    ///<summary>The full path to Rhino's installation folder</summary>
    ///<returns>(string) the full path to Rhino's installation folder</returns>
    static member InstallFolder() : string =
        ApplicationSettings.FileSettings.InstallFolder.FullName
    (*
    def InstallFolder():
        '''The full path to Rhino's installation folder
        Returns:
          str: the full path to Rhino's installation folder
        '''
        return Rhino.ApplicationSettings.FileSettings.InstallFolder
    *)


    ///<summary>Verifies that a command alias exists in Rhino</summary>
    ///<param name="alias">(string) The name of an existing command alias</param>
    ///<returns>(bool) True if exists or False if the alias does not exist.</returns>
    static member IsAlias(alias:string) : bool =
        ApplicationSettings.CommandAliasList.IsAlias(alias)
    (*
    def IsAlias(alias):
        '''Verifies that a command alias exists in Rhino
        Parameters:
          alias (str): The name of an existing command alias
        Returns:
          bool: True if exists or False if the alias does not exist.
        '''
        return Rhino.ApplicationSettings.CommandAliasList.IsAlias(alias)
    *)


    ///<summary>Verifies that a command exists in Rhino. Useful when scripting commands
    ///  found in 3rd party plug-ins.</summary>
    ///<param name="commandName">(string) The command name to test</param>
    ///<returns>(bool) True if the string is a command or False if it is not a command.</returns>
    static member IsCommand(commandName:string) : bool =
        Rhino.Commands.Command.IsCommand(commandName)
    (*
    def IsCommand(command_name):
        '''Verifies that a command exists in Rhino. Useful when scripting commands
        found in 3rd party plug-ins.
        Parameters:
          command_name (str): The command name to test
        Returns:
          bool: True if the string is a command or False if it is not a command.
        '''
        return rhcommand.IsCommand(command_name)
    *)


    ///<summary>Verifies that a plug-in is registered</summary>
    ///<param name="plugin">(Guid) The unique id of the plug-in</param>
    ///<returns>(bool) True if the Guid is registered or False if it is not.</returns>
    static member IsPlugIn(plugin:Guid) : bool =
        let id = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        if id = Guid.Empty then false
        else
            let rc, loaded, loadprot = Rhino.PlugIns.PlugIn.PlugInExists(id)
            rc
    (*
    def IsPlugIn(plugin):
        '''Verifies that a plug-in is registered
        Parameters:
          plugin (guid): The unique id of the plug-in
        Returns:
          bool: True if the Guid is registered or False if it is not.
        '''
        id = rhutil.coerceguid(plugin)
        if not id: id = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        if id:
            rc, loaded, loadprot = Rhino.PlugIns.PlugIn.PlugInExists(id)
            return rc
    *)


    ///<summary>Returns True if this script is being executed on a Windows platform</summary>
    ///<returns>(bool) True if currently running on the Widows platform. False if it is not Windows.</returns>
    static member IsRunningOnWindows() : bool =
        Rhino.Runtime.HostUtils.RunningOnWindows
    (*
    def IsRunningOnWindows():
        '''Returns True if this script is being executed on a Windows platform
        Returns:
          bool: True if currently running on the Widows platform. False if it is not Windows.
        '''
        return Rhino.Runtime.HostUtils.RunningOnWindows
    *)


    ///<summary>Returns the name of the last executed command</summary>
    ///<returns>(string) the name of the last executed command</returns>
    static member LastCommandName() : string =
        let mutable id = Rhino.Commands.Command.LastCommandId
        Rhino.Commands.Command.LookupCommandName(id, true)
    (*
    def LastCommandName():
        '''Returns the name of the last executed command
        Returns:
          str: the name of the last executed command
        '''
        id = rhcommand.LastCommandId
        return rhcommand.LookupCommandName(id, True)
    *)


    ///<summary>Returns the result code for the last executed command</summary>
    ///<returns>(float) the result code for the last executed command.
    ///  0 = success (command successfully completed)
    ///  1 = cancel (command was cancelled by the user)
    ///  2 = nothing (command did nothing, but was not cancelled)
    ///  3 = failure (command failed due to bad input, computational problem...)
    ///  4 = unknown command (the command was not found)</returns>
    static member LastCommandResult() : float =
        int(Rhino.Commands.Command.LastCommandResult)
    (*
    def LastCommandResult():
        '''Returns the result code for the last executed command
        Returns:
          number: the result code for the last executed command.
                    0 = success (command successfully completed)
                    1 = cancel (command was cancelled by the user)
                    2 = nothing (command did nothing, but was not cancelled)
                    3 = failure (command failed due to bad input, computational problem...)
                    4 = unknown command (the command was not found)
        '''
        return int(rhcommand.LastCommandResult)
    *)


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
        ApplicationSettings.AppearanceSettings.LanguageIdentifier
    (*
    def LocaleID():
        '''Returns the current language used for the Rhino interface.  The current
        language is returned as a locale ID, or LCID, value.
        Returns:
          number: the current language used for the Rhino interface as a locale ID, or LCID.
                    1029  Czech
                    1031  German-Germany
                    1033  English-United States
                    1034  Spanish-Spain
                    1036  French-France
                    1040  Italian-Italy
                    1041  Japanese
                    1042  Korean
                    1045  Polish
        '''
        return Rhino.ApplicationSettings.AppearanceSettings.LanguageIdentifier
    *)


    ///<summary>Get status of Rhino's ortho modeling aid.</summary>
    ///<returns>(bool) The current ortho status</returns>
    static member Ortho() : bool = //GET
        ModelAidSettings.Ortho <- enable
    (*
    def Ortho(enable=None):
        '''Enables or disables Rhino's ortho modeling aid.
        Parameters:
          enable (bool, optional): The new enabled status (True or False). If omitted the current state is returned.
        Returns:
          bool: if enable is not specified, then the current ortho status
          bool: if enable is specified, then the previous ortho status
        '''
        rc = modelaid.Ortho
        if enable!=None: modelaid.Ortho = enable
        return rc
    *)

    ///<summary>Enables or disables Rhino's ortho modeling aid.</summary>
    ///<param name="enable">(bool)The new enabled status (True or False). If omitted the current state is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member Ortho(enable:bool) : unit = //SET
        ModelAidSettings.Ortho <- enable
    (*
    def Ortho(enable=None):
        '''Enables or disables Rhino's ortho modeling aid.
        Parameters:
          enable (bool, optional): The new enabled status (True or False). If omitted the current state is returned.
        Returns:
          bool: if enable is not specified, then the current ortho status
          bool: if enable is specified, then the previous ortho status
        '''
        rc = modelaid.Ortho
        if enable!=None: modelaid.Ortho = enable
        return rc
    *)


    ///<summary>Get status of Rhino's object snap modeling aid.
    ///  Object snaps are tools for specifying points on existing objects.</summary>
    ///<returns>(bool) The current osnap status</returns>
    static member Osnap() : bool = //GET
        ModelAidSettings.Osnap <- enable
    (*
    def Osnap(enable=None):
        '''Enables or disables Rhino's object snap modeling aid.
        Object snaps are tools for specifying points on existing objects.
        Parameters:
          enable (bool, optional): The new enabled status.
        Returns:
          bool: if enable is not specified, then the current osnap status
          bool: if enable is specified, then the previous osnap status
        '''
        rc = modelaid.Osnap
        if enable!=None: modelaid.Osnap = enable
        return rc
    *)

    ///<summary>Enables or disables Rhino's object snap modeling aid.
    ///  Object snaps are tools for specifying points on existing objects.</summary>
    ///<param name="enable">(bool)The new enabled status.</param>
    ///<returns>(unit) void, nothing</returns>
    static member Osnap(enable:bool) : unit = //SET
        ModelAidSettings.Osnap <- enable
    (*
    def Osnap(enable=None):
        '''Enables or disables Rhino's object snap modeling aid.
        Object snaps are tools for specifying points on existing objects.
        Parameters:
          enable (bool, optional): The new enabled status.
        Returns:
          bool: if enable is not specified, then the current osnap status
          bool: if enable is specified, then the previous osnap status
        '''
        rc = modelaid.Osnap
        if enable!=None: modelaid.Osnap = enable
        return rc
    *)


    ///<summary>Get status of Rhino's dockable object snap bar</summary>
    ///<returns>(bool) The current visible state</returns>
    static member OsnapDialog() : bool = //GET
        ModelAidSettings.UseHorizontalDialog <- visible
    (*
    def OsnapDialog(visible=None):
        '''Shows or hides Rhino's dockable object snap bar
        Parameters:
          visible (bool, optional): The new visibility state. If omitted then the current state is returned.
        Returns:
          bool: if visible is not specified, then the current visible state
          bool: if visible is specified, then the previous visible state
        '''
        rc = modelaid.UseHorizontalDialog
        if visible is not None: modelaid.UseHorizontalDialog = visible
        return rc
    *)

    ///<summary>Shows or hides Rhino's dockable object snap bar</summary>
    ///<param name="visible">(bool)The new visibility state. If omitted then the current state is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member OsnapDialog(visible:bool) : unit = //SET
        ModelAidSettings.UseHorizontalDialog <- visible
    (*
    def OsnapDialog(visible=None):
        '''Shows or hides Rhino's dockable object snap bar
        Parameters:
          visible (bool, optional): The new visibility state. If omitted then the current state is returned.
        Returns:
          bool: if visible is not specified, then the current visible state
          bool: if visible is specified, then the previous visible state
        '''
        rc = modelaid.UseHorizontalDialog
        if visible is not None: modelaid.UseHorizontalDialog = visible
        return rc
    *)


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
    static member OsnapMode() : int = //GET
        ModelAidSettings.OsnapModes
    (*
    def OsnapMode(mode=None):
        '''Returns or sets the object snap mode. Object snaps are tools for
        specifying points on existing objects
        Parameters:
          mode (number, optional): The object snap mode or modes to set. 
            0          None
            2          Near
            8          Focus
            32         Center
            64         Vertex
            128        Knot
            512        Quadrant
            2048       Midpoint
            8192       Intersection
            131072     End
            524288     Perpendicular
            2097152    Tangent
            134217728  Point
            Object snap modes can be added together to set multiple modes
        Returns:
          number: if mode is not specified, then the current object snap mode(s)
          number: if mode is specified, then the previous object snap mode(s)
        '''
        rc = int(modelaid.OsnapModes)
        # RH-39062 reverts RH-31758
        #m = [(0,0), (1,2), (2,8), (4,0x20), (8,0x80), (16,0x200), (32,0x800), (64,0x2000),
        #      (128,0x20000), (256,0x80000), (512,0x200000), (1024,0x8000000), (2048, 0x40)]
        #rc = sum([x[0] for x in m if x[1] & rc])
        if mode is not None:
            #mode = sum([x[1] for x in m if x[0] & int(mode)])
            modelaid.OsnapModes = System.Enum.ToObject(Rhino.ApplicationSettings.OsnapModes, mode)
        return rc
    *)

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
    ///<returns>(unit) void, nothing</returns>
    static member OsnapMode(mode:float) : unit = //SET
        failwith "setOsnapMode is not implemented"
        //rc <- int(ModelAidSettings.OsnapModes)
        //m <- .[(0,0), (1,2), (2,8), (4,0x20), (8,0x80), (16,0x200), (32,0x800), (64,0x2000),
        //      (128,0x20000), (256,0x80000), (512,0x200000), (1024,0x8000000), (2048, 0x40)]
        //rc <- sum(.[x.[0] for x in m if x.[1] & rc])
        //if mode = not <| None then 
        //    mode <- sum(.[x.[1] for x in m if x.[0] & int(mode)])
        //ModelAidSettings.OsnapModes <- System.Enum.ToObject( typeof<ApplicationSettings.OsnapModes> , mode)
    (*
    def OsnapMode(mode=None):
        '''Returns or sets the object snap mode. Object snaps are tools for
        specifying points on existing objects
        Parameters:
          mode (number, optional): The object snap mode or modes to set. 
            0          None
            2          Near
            8          Focus
            32         Center
            64         Vertex
            128        Knot
            512        Quadrant
            2048       Midpoint
            8192       Intersection
            131072     End
            524288     Perpendicular
            2097152    Tangent
            134217728  Point
            Object snap modes can be added together to set multiple modes
        Returns:
          number: if mode is not specified, then the current object snap mode(s)
          number: if mode is specified, then the previous object snap mode(s)
        '''
        rc = int(modelaid.OsnapModes)
        # RH-39062 reverts RH-31758
        #m = [(0,0), (1,2), (2,8), (4,0x20), (8,0x80), (16,0x200), (32,0x800), (64,0x2000),
        #      (128,0x20000), (256,0x80000), (512,0x200000), (1024,0x8000000), (2048, 0x40)]
        #rc = sum([x[0] for x in m if x[1] & rc])
        if mode is not None:
            #mode = sum([x[1] for x in m if x[0] & int(mode)])
            modelaid.OsnapModes = System.Enum.ToObject(Rhino.ApplicationSettings.OsnapModes, mode)
        return rc
    *)


    ///<summary>Get status of Rhino's planar modeling aid</summary>
    ///<returns>(bool) The current planar status</returns>
    static member Planar() : bool = //GET
        ModelAidSettings.Planar <- enable
    (*
    def Planar(enable=None):
        '''Enables or disables Rhino's planar modeling aid
        Parameters:
          enable (bool, optional): The new enable status.  If omitted the current state is returned.
        Returns:
          bool: if enable is not specified, then the current planar status
          bool: if enable is secified, then the previous planar status
        '''
        rc = modelaid.Planar
        if enable is not None: modelaid.Planar = enable
        return rc
    *)

    ///<summary>Enables or disables Rhino's planar modeling aid</summary>
    ///<param name="enable">(bool)The new enable status.  If omitted the current state is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member Planar(enable:bool) : unit = //SET
        ModelAidSettings.Planar <- enable
    (*
    def Planar(enable=None):
        '''Enables or disables Rhino's planar modeling aid
        Parameters:
          enable (bool, optional): The new enable status.  If omitted the current state is returned.
        Returns:
          bool: if enable is not specified, then the current planar status
          bool: if enable is secified, then the previous planar status
        '''
        rc = modelaid.Planar
        if enable is not None: modelaid.Planar = enable
        return rc
    *)


    ///<summary>Returns the identifier of a plug-in given the plug-in name</summary>
    ///<param name="plugin">(Guid) Unique id of the plug-in</param>
    ///<returns>(Guid) the id of the plug-in</returns>
    static member PlugInId(plugin:Guid) : Guid =
        let id = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        if id<>System.Guid.Empty then  id 
        else failwithf "Plugin %s not found" plugin
    (*
    def PlugInId(plugin):
        '''Returns the identifier of a plug-in given the plug-in name
        Parameters:
          plugin (guid): Unique id of the plug-in
        Returns:
          guid: the id of the plug-in
          None: None if the plug-in isn't valid
        '''
        id = Rhino.PlugIns.PlugIn.IdFromName(plugin)
        if id!=System.Guid.Empty: return id
    *)


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
        let mutable filter = Rhino.PlugIns.PlugInType.Any
        if types=1 then  filter <- Rhino.PlugIns.PlugInType.Render
        if types=2 then  filter <- Rhino.PlugIns.PlugInType.FileExport
        if types=4 then  filter <- Rhino.PlugIns.PlugInType.FileImport
        if types=8 then  filter <- Rhino.PlugIns.PlugInType.Digitizer
        if types=16 then filter <- Rhino.PlugIns.PlugInType.Utility        
        let loaded = status=0 || status=1
        let unloaded = status=0 || status=2
        Rhino.PlugIns.PlugIn.GetInstalledPlugInNames(filter, loaded, unloaded)
    (*
    def PlugIns(types=0, status=0):
        '''Returns a list of registered Rhino plug-ins
        Parameters:
          types (number, optional): The type of plug-ins to return.
                                    0=all
                                    1=render
                                    2=file export
                                    4=file import
                                    8=digitizer
                                    16=utility.
                                    If omitted, all are returned.
          status (number, optional): 0=both loaded and unloaded, 1=loaded, 2=unloaded.  If omitted both status is returned.
        Returns:
          list of str: list of registered Rhino plug-ins
        '''
        filter = Rhino.PlugIns.PlugInType.None
        if types&1: filter |= Rhino.PlugIns.PlugInType.Render
        if types&2: filter |= Rhino.PlugIns.PlugInType.FileExport
        if types&4: filter |= Rhino.PlugIns.PlugInType.FileImport
        if types&8: filter |= Rhino.PlugIns.PlugInType.Digitiger
        if types&16: filter |= Rhino.PlugIns.PlugInType.Utility
        if types==0: filter = Rhino.PlugIns.PlugInType.Any
        loaded = (status==0 or status==1)
        unloaded = (status==0 or status==2)
        names = Rhino.PlugIns.PlugIn.GetInstalledPlugInNames(filter, loaded, unloaded)
        return list(names)
    *)


    ///<summary>Get status of object snap projection</summary>
    ///<returns>(bool) the current object snap projection status</returns>
    static member ProjectOsnaps() : bool = //GET
        ModelAidSettings.ProjectSnapToCPlane <- enable
    (*
    def ProjectOsnaps(enable=None):
        '''Enables or disables object snap projection
        Parameters:
          enable (bool, optional): The new enabled status.  If omitted the current status is returned.
        Returns:
          bool: the current object snap projection status
        '''
        rc = modelaid.ProjectSnapToCPlane
        if enable is not None: modelaid.ProjectSnapToCPlane = enable
        return rc
    *)

    ///<summary>Enables or disables object snap projection</summary>
    ///<param name="enable">(bool)The new enabled status.  If omitted the current status is returned.</param>
    static member ProjectOsnaps(enable:bool) : unit = //SET
        ModelAidSettings.ProjectSnapToCPlane <- enable
    (*
    def ProjectOsnaps(enable=None):
        '''Enables or disables object snap projection
        Parameters:
          enable (bool, optional): The new enabled status.  If omitted the current status is returned.
        Returns:
          bool: the current object snap projection status
        '''
        rc = modelaid.ProjectSnapToCPlane
        if enable is not None: modelaid.ProjectSnapToCPlane = enable
        return rc
    *)


    ///<summary>Change Rhino's command window prompt</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///The new prompt on the commandline.  If omitted the prompt will be blank.</param>
    ///<returns>(unit) </returns>
    static member Prompt([<OPT;DEF(null)>]message:string) : unit =
        RhinoApp.SetCommandPrompt(message)
    (*
    def Prompt(message=None):
        '''Change Rhino's command window prompt
        Parameters:
          message (str, optional): The new prompt on the commandline.  If omitted the prompt will be blank.
        Returns:
          none
        '''
        if message and type(message) is not str:
            strList = [str(item) for item in message]
            message = "".join(strList)
        RhinoApp.SetCommandPrompt(message)
    *)


    ///<summary>Returns current width and height, of the screen of the primary monitor.</summary>
    ///<returns>(float * float) containing two numbers identifying the width and height in pixels</returns>
    static member ScreenSize() : float * float =
        let sz = System.Windows.Forms.Screen.PrimaryScreen.Bounds
        sz.Width, sz.Height
    (*
    def ScreenSize():
        '''Returns current width and height, of the screen of the primary monitor.
        Returns:
          tuple (width, height): containing two numbers identifying the width and height in pixels
        '''
        sz = System.Windows.Forms.Screen.PrimaryScreen.Bounds
        return sz.Width, sz.Height
    *)


    ///<summary>Returns version of the Rhino SDK supported by the executing Rhino.</summary>
    ///<returns>(string) the version of the Rhino SDK supported by the executing Rhino. Rhino SDK versions are 9 digit numbers in the form of YYYYMMDDn.</returns>
    static member SdkVersion() : string =
        RhinoApp.SdkVersion
    (*
    def SdkVersion():
        '''Returns version of the Rhino SDK supported by the executing Rhino.
        Returns:
          str: the version of the Rhino SDK supported by the executing Rhino. Rhino SDK versions are 9 digit numbers in the form of YYYYMMDDn.
        '''
        return RhinoApp.SdkVersion
    *)


    ///<summary>Returns the number of path items in Rhino's search path list.
    ///  See "Options Files settings" in the Rhino help file for more details.</summary>
    ///<returns>(int) the number of path items in Rhino's search path list</returns>
    static member SearchPathCount() : int =
        ApplicationSettings.FileSettings.SearchPathCount
    (*
    def SearchPathCount():
        '''Returns the number of path items in Rhino's search path list.
        See "Options Files settings" in the Rhino help file for more details.
        Returns:
          number: the number of path items in Rhino's search path list
        '''
        return Rhino.ApplicationSettings.FileSettings.SearchPathCount
    *)


    ///<summary>Returns all of the path items in Rhino's search path list.
    ///  See "Options Files settings" in the Rhino help file for more details.</summary>
    ///<returns>(string seq) list of search paths</returns>
    static member SearchPathList() : string seq =
        ApplicationSettings.FileSettings.GetSearchPaths()
    (*
    def SearchPathList():
        '''Returns all of the path items in Rhino's search path list.
        See "Options Files settings" in the Rhino help file for more details.
        Returns:
          list of str: list of search paths
        '''
        return Rhino.ApplicationSettings.FileSettings.GetSearchPaths()
    *)


    ///<summary>Sends a string of printable characters to Rhino's command line</summary>
    ///<param name="keys">(string) Optional, Default Value: <c>null</c>
    ///A string of characters to send to the command line.</param>
    ///<param name="addReturn">(bool) Optional, Default Value: <c>true</c>
    ///Append a return character to the end of the string. If omitted an return character will be added (True)</param>
    ///<returns>(unit) </returns>
    static member SendKeystrokes([<OPT;DEF(null)>]keys:string, [<OPT;DEF(true)>]addReturn:bool) : unit =
        RhinoApp.SendKeystrokes(keys, addReturn)
    (*
    def SendKeystrokes(keys=None, add_return=True):
        '''Sends a string of printable characters to Rhino's command line
        Parameters:
          keys (str, optional): A string of characters to send to the command line.
          add_return (bool, optional): Append a return character to the end of the string. If omitted an return character will be added (True)
        Returns:
          none
        '''
        RhinoApp.SendKeystrokes(keys, add_return)
    *)


    ///<summary>Get status of Rhino's grid snap modeling aid</summary>
    ///<returns>(bool) the current grid snap status</returns>
    static member Snap() : bool = //GET
        ModelAidSettings.GridSnap <- enable
    (*
    def Snap(enable=None):
        '''Enables or disables Rhino's grid snap modeling aid
        Parameters:
          enable (bool, optional): The new enabled status. If omitted the current status is returned.
        Returns:
          bool: the current grid snap status
        '''
        rc = modelaid.GridSnap
        if enable is not None and enable <> rc:
            modelaid.GridSnap = enable
        return rc
    *)

    ///<summary>Enables or disables Rhino's grid snap modeling aid</summary>
    ///<param name="enable">(bool)The new enabled status. If omitted the current status is returned.</param>
    static member Snap(enable:bool) : unit = //SET
        ModelAidSettings.GridSnap <- enable
    (*
    def Snap(enable=None):
        '''Enables or disables Rhino's grid snap modeling aid
        Parameters:
          enable (bool, optional): The new enabled status. If omitted the current status is returned.
        Returns:
          bool: the current grid snap status
        '''
        rc = modelaid.GridSnap
        if enable is not None and enable <> rc:
            modelaid.GridSnap = enable
        return rc
    *)


    ///<summary>Sets Rhino's status bar distance pane</summary>
    ///<param name="distance">(int) Optional, Default Value: <c>0</c>
    ///The distance to set the status bar.  If omitted the distance will be set to 0.</param>
    ///<returns>(unit) </returns>
    static member StatusBarDistance([<OPT;DEF(0)>]distance:int) : unit =
        Rhino.UI.StatusBar.SetDistancePane(distance)
    (*
    def StatusBarDistance(distance=0):
        '''Sets Rhino's status bar distance pane
        Parameters:
          distance (number, optional): The distance to set the status bar.  If omitted the distance will be set to 0.
        Returns:
          none
        '''
        Rhino.UI.StatusBar.SetDistancePane(distance)
    *)


    ///<summary>Sets Rhino's status bar message pane</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///The message to display.</param>
    ///<returns>(unit) </returns>
    static member StatusBarMessage([<OPT;DEF(null)>]message:string) : unit =
        Rhino.UI.StatusBar.SetMessagePane(message)
    (*
    def StatusBarMessage(message=None):
        '''Sets Rhino's status bar message pane
          Parameters:
            message (str, optional): The message to display.
          Returns:
            none
          '''
        Rhino.UI.StatusBar.SetMessagePane(message)
    *)


    ///<summary>Sets Rhino's status bar point coordinate pane</summary>
    ///<param name="point">(Point3d) Optional, Default Value: <c>null</c>
    ///The 3d coordinates of the status bar.  If omitted the current poition is set to (0,0,0).</param>
    ///<returns>(unit) </returns>
    static member StatusBarPoint([<OPT;DEF(null)>]point:Point3d) : unit =
        Rhino.UI.StatusBar.SetPointPane(point)
    (*
    def StatusBarPoint(point=None):
        '''Sets Rhino's status bar point coordinate pane
        Parameters:
          point (point3d, optional): The 3d coordinates of the status bar.  If omitted the current poition is set to (0,0,0).
        Returns:
          none
        '''
        point = rhutil.coerce3dpoint(point)
        if not point: point = Rhino.Geometry.Point3d(0,0,0)
        Rhino.UI.StatusBar.SetPointPane(point)
    *)


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
        let mutable rc = Rhino.UI.StatusBar.ShowProgressMeter(lower, upper, label, embedLabel, showPercent)
        rc=1
    (*
    def StatusBarProgressMeterShow(label, lower, upper, embed_label=True, show_percent=True):
        '''Start the Rhino status bar progress meter
        Parameters:
          label (str): Short description of the progesss
          lower (str): Lower limit of the progress meter's range
          upper (str): Upper limit of the progress meter's range
          embed_label (bool, optional): If True, the label will show inside the meter.
                                        If false, the label will show to the left of the meter.
                                        If omitted the label will show inside the meter (True)
          show_percent (bool): Show the percent complete if True. If omitted the percnetage will be shown (True)
        Returns:
          bool: True or False indicating success or failure
        '''
        rc = Rhino.UI.StatusBar.ShowProgressMeter(lower, upper, label, embed_label, show_percent)
        return rc==1
    *)


    ///<summary>Set the current position of the progress meter</summary>
    ///<param name="position">(int) The new position in the progress meter</param>
    ///<param name="absolute">(bool) Optional, Default Value: <c>true</c>
    ///The position is set absolute (True) or relative (False) to its current position. If omitted the absolute (True) is used.</param>
    ///<returns>(unit) void, nothing</returns>
    static member StatusBarProgressMeterUpdate(position:int, [<OPT;DEF(true)>]absolute:bool) : unit =
        Rhino.UI.StatusBar.UpdateProgressMeter(position, absolute)
        |> ignore
    (*
    def StatusBarProgressMeterUpdate(position, absolute=True):
        '''Set the current position of the progress meter
        Parameters:
          position (number): The new position in the progress meter
          absolute (bool, optional): The position is set absolute (True) or relative (False) to its current position. If omitted the absolute (True) is used.
        Returns:
          number: previous position setting.
        '''
        return Rhino.UI.StatusBar.UpdateProgressMeter(position, absolute)
    *)


    ///<summary>Hide the progress meter</summary>
    ///<returns>(unit) </returns>
    static member StatusBarProgressMeterHide() : unit =
        Rhino.UI.StatusBar.HideProgressMeter()
    (*
    def StatusBarProgressMeterHide():
        '''Hide the progress meter
        Returns:
          none
        '''
        Rhino.UI.StatusBar.HideProgressMeter()
    *)


    ///<summary>Returns Rhino's default template file. This is the file used
    /// when Rhino starts.</summary>
    ///<returns>(string) The current default template file</returns>
    static member TemplateFile() : string = //GET
        ApplicationSettings.FileSettings.TemplateFile
    (*
    def TemplateFile(filename=None):
        '''Returns or sets Rhino's default template file. This is the file used
        when Rhino starts.
        Parameters:
          filename (str, optional): The name of the new default template file. If omitted the current default template name is returned.
        Returns:
          str: if filename is not specified, then the current default template file
          str: if filename is specified, then the previous default template file
        '''
        rc = Rhino.ApplicationSettings.FileSettings.TemplateFile
        if filename: Rhino.ApplicationSettings.FileSettings.TemplateFile = filename
        return rc
    *)

    ///<summary>Sets Rhino's default template file. This is the file used
    /// when Rhino starts.</summary>
    ///<param name="filename">(string)The name of the new default template file. If omitted the current default template name is returned.</param>
    ///<returns>(unit) void, nothing</returns>
    static member TemplateFile(filename:string) : unit = //SET
        ApplicationSettings.FileSettings.TemplateFile <- filename
    (*
    def TemplateFile(filename=None):
        '''Returns or sets Rhino's default template file. This is the file used
        when Rhino starts.
        Parameters:
          filename (str, optional): The name of the new default template file. If omitted the current default template name is returned.
        Returns:
          str: if filename is not specified, then the current default template file
          str: if filename is specified, then the previous default template file
        '''
        rc = Rhino.ApplicationSettings.FileSettings.TemplateFile
        if filename: Rhino.ApplicationSettings.FileSettings.TemplateFile = filename
        return rc
    *)


    ///<summary>Returns the location of Rhino's template folder</summary>
    ///<returns>(string) The current template file folder</returns>
    static member TemplateFolder() : string = //GET
        ApplicationSettings.FileSettings.TemplateFolder
    (*
    def TemplateFolder(folder=None):
        '''Returns or sets the location of Rhino's template folder
        Parameters:
          folder (str, optional): The location of Rhino's template files. Note, the location must exist.
        Returns:
          str: if folder is not specified, then the current template file folder
          str: if folder is specified, then the previous template file folder
        '''
        rc = Rhino.ApplicationSettings.FileSettings.TemplateFolder
        if folder is not None: Rhino.ApplicationSettings.FileSettings.TemplateFolder = folder
        return rc
    *)

    ///<summary>Sets the location of Rhino's template folder</summary>
    ///<param name="folder">(string)The location of Rhino's template files. Note, the location must exist.</param>
    ///<returns>(unit) void, nothing</returns>
    static member TemplateFolder(folder:string) : unit = //SET
        ApplicationSettings.FileSettings.TemplateFolder <- folder
    (*
    def TemplateFolder(folder=None):
        '''Returns or sets the location of Rhino's template folder
        Parameters:
          folder (str, optional): The location of Rhino's template files. Note, the location must exist.
        Returns:
          str: if folder is not specified, then the current template file folder
          str: if folder is specified, then the previous template file folder
        '''
        rc = Rhino.ApplicationSettings.FileSettings.TemplateFolder
        if folder is not None: Rhino.ApplicationSettings.FileSettings.TemplateFolder = folder
        return rc
    *)


    ///<summary>Returns the windows handle of Rhino's main window</summary>
    ///<returns>(IntPtr) the Window's handle of Rhino's main window. IntPtr is a platform-specific type that is used to represent a pointer or a handle.</returns>
    static member WindowHandle() : IntPtr =
        RhinoApp.MainWindowHandle()
    (*
    def WindowHandle():
        '''Returns the windows handle of Rhino's main window
        Returns:
          IntPt: the Window's handle of Rhino's main window. IntPtr is a platform-specific type that is used to represent a pointer or a handle.
        '''
        return RhinoApp.MainWindowHandle()
    *)


    ///<summary>Returns Rhino's working folder (directory).
    /// The working folder is the default folder for all file operations.</summary>
    ///<returns>(string) The current working folder</returns>
    static member WorkingFolder() : string = //GET
        ApplicationSettings.FileSettings.WorkingFolder
    (*
    def WorkingFolder(folder=None):
        '''Returns or sets Rhino's working folder (directory).
        The working folder is the default folder for all file operations.
        Parameters:
          folder (str, optional): The new working folder for the current Rhino session.
        Returns:
          str: if folder is not specified, then the current working folder
          str: if folder is specified, then the previous working folder
        '''
        rc = Rhino.ApplicationSettings.FileSettings.WorkingFolder
        if folder is not None: Rhino.ApplicationSettings.FileSettings.WorkingFolder = folder
        return rc
    *)

    ///<summary>Sets Rhino's working folder (directory).
    /// The working folder is the default folder for all file operations.</summary>
    ///<param name="folder">(string)The new working folder for the current Rhino session.</param>
    ///<returns>(unit) void, nothing</returns>
    static member WorkingFolder(folder:string) : unit = //SET
        ApplicationSettings.FileSettings.WorkingFolder <- folder
    (*
    def WorkingFolder(folder=None):
        '''Returns or sets Rhino's working folder (directory).
        The working folder is the default folder for all file operations.
        Parameters:
          folder (str, optional): The new working folder for the current Rhino session.
        Returns:
          str: if folder is not specified, then the current working folder
          str: if folder is specified, then the previous working folder
        '''
        rc = Rhino.ApplicationSettings.FileSettings.WorkingFolder
        if folder is not None: Rhino.ApplicationSettings.FileSettings.WorkingFolder = folder
        return rc
    *)


