namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
open Rhino
open Rhino.UI
open System.Drawing
//open System.Windows.Forms

[<AutoOpen>]
module ExtensionsUserinterface =
  
  ///of the currently Running Rhino Instance, to be set via RhinoScriptSyntax.SynchronizationContext from running script
  let mutable internal syncContext = System.Threading.SynchronizationContext.Current  

  [<EXT>] 
  type RhinoScriptSyntax with
    

    [<EXT>]
    ///<summary>The Synchronization Context of the Rhino UI Therad.
    ///This MUST be set at the  beginning of every Script if using UI dialogs and not running on UI thread</summary>
    static member SynchronizationContext 
        with get() = syncContext
        and set v  = syncContext <- v


    [<EXT>]
    ///<summary>Display browse-for-folder dialog allowing the user to select a folder</summary>
    ///<param name="folder">(string) Optional, Default Value: <c>null:string</c>
    ///A default folder</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message</param>   
    ///<returns>(string option) selected folder option or None if selection was canceled</returns>
    static member BrowseForFolder([<OPT;DEF(null:string)>]folder:string, [<OPT;DEF(null:string)>]message:string) : string option =
        async{            
            do! Async.SwitchToContext syncContext        
            use dlg = new System.Windows.Forms.FolderBrowserDialog()
            dlg.ShowNewFolderButton <- true
            if notNull folder then
                if IO.Directory.Exists(folder) then
                    dlg.SelectedPath <-  folder
            if notNull message then
                dlg.Description <- message
            if dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK then 
                return Some(dlg.SelectedPath)
            else
                return None
        }  |> Async.RunSynchronously
        // or use ETO ??
        //let dlg = Eto.Forms.SelectFolderDialog()
        //if notNull folder then
        //    if not <| isinstance(folder, str) then folder <- string(folder)
        //    let dlg.Directory = folder
        //if notNull message then
        //    if not <| isinstance(message, str) then message <- string(message)
        //    let dlg.Title = message
        //if dlg.ShowDialog(null) = Eto.Forms.DialogResult.Ok then
        //    dlg.Directory
    (*
    def BrowseForFolder(folder=None, message=None, title=None):
        '''Display browse-for-folder dialog allowing the user to select a folder
        Parameters:
          folder (str, optional): a default folder
          message (str, optional): a prompt or message
          title (str, optional): a dialog box title
        Returns:
          str: selected folder
          None: on error
        '''
    
        dlg = Eto.Forms.SelectFolderDialog()
        if folder:
            if not isinstance(folder, str): folder = str(folder)
            dlg.Directory = folder
        if message:
            if not isinstance(message, str): message = str(message)
            dlg.Title = message
        if dlg.ShowDialog(None)==Eto.Forms.DialogResult.Ok:
            return dlg.Directory
    *)


    [<EXT>]
    ///<summary>Displays a list of items in a checkable-style list dialog box</summary>
    ///<param name="items">((string*bool) seq) A list of tuples containing a string and a boolean check state</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title</param>
    ///<returns>((string*bool) [] option) Option of tuples containing the input string in items along with their new boolean check value</returns>
    static member CheckListBox(items:(string*bool) seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : (string*bool) [] option=
        let checkstates = [| for item in items -> snd item |]
        let itemstrs = [|for item in items -> fst item|]
                
        let newcheckstates =
            async{
                do! Async.SwitchToContext syncContext 
                return Rhino.UI.Dialogs.ShowCheckListBox(title, message, itemstrs, checkstates)
                } |> Async.RunSynchronously

        if notNull newcheckstates then
            Some (Array.zip itemstrs newcheckstates)
        else
            //failwithf "Rhino.Scripting: CheckListBox failed.  items:'%A' message:'%A' title:'%A'" items message title
            None
    (*
    def CheckListBox(items, message=None, title=None):
        '''Displays a list of items in a checkable-style list dialog box
        Parameters:
          items ([[str, bool], ...]): a list of tuples containing a string and a boolean check state
          message (str, optional):  a prompt or message
          title (str, optional):  a dialog box title
        Returns:
          list((str, bool), ...): of tuples containing the input string in items along with their new boolean check value
          None: on error
        '''
    
        checkstates = [item[1] for item in items]
        itemstrs = [str(item[0]) for item in items]
        newcheckstates = Rhino.UI.Dialogs.ShowCheckListBox(title, message, itemstrs, checkstates)
        if newcheckstates:
            rc = zip(itemstrs, newcheckstates)
            return rc
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Displays a list of items in a combo-style list box dialog.</summary>
    ///<param name="items">(string seq) A list of string</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt of message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title</param>
    ///<returns>(string Option) Option of  The selected item</returns>
    static member ComboListBox(items:string seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : string option=
        async{
            do! Async.SwitchToContext syncContext 
            return 
                match Rhino.UI.Dialogs.ShowComboListBox(title, message, items|> Array.ofSeq) with
                | null -> None
                | :? string as s -> Some s
                | _ -> None
            } |> Async.RunSynchronously
    (*
    def ComboListBox(items, message=None, title=None):
        '''Displays a list of items in a combo-style list box dialog.
        Parameters:
          items ([str, ...]): a list of string
          message (str, optional):  a prompt of message
          title (str, optional):  a dialog box title
        Returns:
          str: The selected item if successful
          None: if not successful or on error
        '''
    
        return Rhino.UI.Dialogs.ShowComboListBox(title, message, items)
    *)


    [<EXT>]
    ///<summary>Display dialog prompting the user to enter a string. The
    ///  string value may span multiple lines</summary>
    ///<param name="defaultValString">(string) Optional, Default Value: <c>null:string</c>
    ///A default string value.</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt message.</param>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title.</param>
    ///<returns>(string Option) Option of  Multiple lines that are separated by carriage return-linefeed combinations</returns>
    static member EditBox([<OPT;DEF(null:string)>]defaultValString:string, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : string option =
        async{
            do! Async.SwitchToContext syncContext 
            let rc, text = Rhino.UI.Dialogs.ShowEditBox(title, message, defaultValString, true)
            return if rc then Some text else None
            } |> Async.RunSynchronously
    (*
    def EditBox(default_string=None, message=None, title=None):
        '''Display dialog prompting the user to enter a string. The
        string value may span multiple lines
        Parameters:
          default_string  (str, optional):  a default string value.
          message (str, optional): a prompt message.
          title (str, optional): a dialog box title.
        Returns:
          str: Multiple lines that are separated by carriage return-linefeed combinations if successful
          None: if not successful
        '''
    
        rc, text = Rhino.UI.Dialogs.ShowEditBox(title, message, default_string, True)
        return text
    *)


    [<EXT>]
    ///<summary>Pause for user input of an angle</summary>
    ///<param name="point">(Point3d ref) Optional, Default Value: <c>null</c>
    ///Starting, or base point</param>
    ///<param name="referencePoint">(Point3d ref) Optional, Default Value: <c>null</c>
    ///If specified, the reference angle is calculated from it and the base point</param>
    ///<param name="defaultValAngleDegrees">(float) Optional, Default Value: <c>0.0</c>
    /// A default angle value specified</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    /// A prompt to display</param>
    ///<returns>(float option)Option of angle in degree</returns>
    static member GetAngle( [<OPT;DEF(null)>]point:Point3d ref, 
                            [<OPT;DEF(null)>]referencePoint:Point3d ref, 
                            [<OPT;DEF(0.0)>]defaultValAngleDegrees:float, 
                            [<OPT;DEF(null:string)>]message:string) : float option=
        async{
            do! Async.SwitchToContext syncContext 
            let point = !(point|? ref Point3d.Unset)
            let referencepoint = !(referencePoint|? ref Point3d.Unset)
            let defaultangle = toRadians(defaultValAngleDegrees)
            let rc, angle = Rhino.Input.RhinoGet.GetAngle(message, point, referencepoint, defaultangle)
            return 
                if rc = Rhino.Commands.Result.Success then Some(toDegrees(angle))
                else None
            } |> Async.RunSynchronously
    (*
    def GetAngle(point=None, reference_point=None, default_angle_degrees=0, message=None):
        '''Pause for user input of an angle
        Parameters:
          point (point): starting, or base point
          reference_point (point, optional): if specified, the reference angle is calculated
            from it and the base point
          default_angle_degrees (number, optional): a default angle value specified
          message (str, optional): a prompt to display
        Returns:
          number: angle in degree if successful
          None: on error
        '''
    
        point = rhutil.coerce3dpoint(point)
        if not point: point = Rhino.Geometry.Point3d.Unset
        reference_point = rhutil.coerce3dpoint(reference_point)
        if not reference_point: reference_point = Rhino.Geometry.Point3d.Unset
        default_angle = math.radians(default_angle_degrees)
        rc, angle = Rhino.Input.RhinoGet.GetAngle(message, point, reference_point, default_angle)
        if rc==Rhino.Commands.Result.Success: return math.degrees(angle)
    *)


    [<EXT>]
    ///<summary>Pauses for user input of one or more boolean values. Boolean values are
    ///  displayed as click-able command line option toggles</summary>
    ///<param name="message">(string) A prompt</param>
    ///<param name="items">(string seq) List or tuple of options. Each option is a tuple of three strings
    ///  [n][1]    description of the boolean value. Must only consist of letters and numbers. (no characters like space, period, or dash)
    ///  [n][2]    string identifying the false value
    ///  [n][3]    string identifying the true value</param>
    ///<param name="defaultVals">(bool seq) List of boolean values used as default or starting values</param>
    ///<returns>(bool seq) a list of values that represent the boolean values</returns>
    static member GetBoolean(message:string, items:(string*string*string) seq, defaultVals:bool seq) : bool array option =
        async{
            do! Async.SwitchToContext syncContext             
            let go = Rhino.Input.Custom.GetOption()
            go.AcceptNothing(true)
            go.SetCommandPrompt( message )
            let count = Seq.length(items)
            if count < 1 || count <> Seq.length(defaultVals) then failwithf "Rhino.Scripting: GetBoolean failed.  message:'%A' items:'%A' defaultVals:'%A'" message items defaultVals
            let toggles = ResizeArray()
            for i in range(count) do
                let initial = defaultVals.[i]
                let item = items.[i]
                let offVal = item.[1]
                let t = Rhino.Input.Custom.OptionToggle( initial, item.[1], item.[2] )
                toggles.Add(t)
                go.AddOptionToggle(item.[0], t)
            
            let mutable getrc = go.Get()
            while getrc = Rhino.Input.GetResult.Option do
            return 
                if getrc <> Rhino.Input.GetResult.Nothing then 
                    None
                else
                    Some [| for t in toggles do yield t.CurrentValue |]
            } |> Async.RunSynchronously
    (*
    def GetBoolean(message, items, defaults):
        '''Pauses for user input of one or more boolean values. Boolean values are
        displayed as click-able command line option toggles
        Parameters:
          message (str): a prompt
          items ([str, str, str], ...): list or tuple of options. Each option is a tuple of three strings
            [n][1]    description of the boolean value. Must only consist of letters
                      and numbers. (no characters like space, period, or dash
            [n][2]    string identifying the false value
            [n][3]    string identifying the true value
          defaults ([bool, ...]): list of boolean values used as default or starting values
        Returns:
          list(bool, ...): a list of values that represent the boolean values if successful
          None: on error
        '''
    
        go = Rhino.Input.Custom.GetOption()
        go.AcceptNothing(True)
        go.SetCommandPrompt( message )
        if type(defaults) is list or type(defaults) is tuple: pass
        else: defaults = [defaults]
        # special case for single list. Wrap items into a list
        if len(items)==3 and len(defaults)==1: items = [items]
        count = len(items)
        if count<1 or count!=len(defaults): return scriptcontext.errorhandler()
        toggles = []
        for i in range(count):
            initial = defaults[i]
            item = items[i]
            offVal = item[1]
            t = Rhino.Input.Custom.OptionToggle( initial, item[1], item[2] )
            toggles.append(t)
            go.AddOptionToggle(item[0], t)
        while True:
            getrc = go.Get()
            if getrc==Rhino.Input.GetResult.Option: continue
            if getrc!=Rhino.Input.GetResult.Nothing: return None
            break
        return [t.CurrentValue for t in toggles]
    *)
