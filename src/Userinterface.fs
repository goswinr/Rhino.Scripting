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
    ///<returns>(string) The selected item</returns>
    static member ComboListBox(items:string seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : string =
        async{
            do! Async.SwitchToContext syncContext 
            return Rhino.UI.Dialogs.ShowComboListBox(title, message, items) :?> string
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
    ///<returns>(string) Multiple lines that are separated by carriage return-linefeed combinations</returns>
    static member EditBox([<OPT;DEF(null:string)>]defaultValString:string, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : string =
        async{
            do! Async.SwitchToContext syncContext 
            let rc, text = Rhino.UI.Dialogs.ShowEditBox(title, message, defaultValString, true)
            return text
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
    ///<param name="point">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///Starting, or base point</param>
    ///<param name="referencePoint">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///If specified, the reference angle is calculated from it and the base point</param>
    ///<param name="defaultValAngleDegrees">(float) Optional, Default Value: <c>0</c>
    /// A default angle value specified</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    /// A prompt to display</param>
    ///<returns>(float) angle in degree</returns>
    static member GetAngle([<OPT;DEF(Point3d())>]point:Point3d, [<OPT;DEF(Point3d())>]referencePoint:Point3d, [<OPT;DEF(0)>]defaultValAngleDegrees:float, [<OPT;DEF(null:string)>]message:string) : float =
        async{
            do! Async.SwitchToContext syncContext 
            
            let point = RhinoScriptSyntax.Coerce3dPoint(point)
            if not <| point then point <- Point3d.Unset
            let reference_point = RhinoScriptSyntax.Coerce3dPoint(reference_point)
            if not <| reference_point then reference_point <- Point3d.Unset
            let default_angle = toRadians(defaultValAngleDegrees)
            let rc, angle = Rhino.Input.RhinoGet.GetAngle(message, point, reference_point, default_angle)
            if rc = Rhino.Commands.Result.Success then toDegrees(angle)
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
    ///  [n][1]    description of the boolean value. Must only consist of letters
    ///    and numbers. (no characters like space, period, or dash
    ///  [n][2]    string identifying the false value
    ///  [n][3]    string identifying the true value</param>
    ///<param name="defaultVals">(bool seq) List of boolean values used as default or starting values</param>
    ///<returns>(bool seq) a list of values that represent the boolean values</returns>
    static member GetBoolean(message:string, items:string seq, defaultVals:bool seq) : bool seq =
        let go = Rhino.Input.Custom.GetOption()
        go.AcceptNothing(true)
        go.SetCommandPrompt( message )
        if typ(defaultVals) = list || typ(defaultVals) = tuple then pass
        else  defaultVals <- .[defaultVals]
        // special case for single list. Wrap items into a list
        if Seq.length(items) = 3 && Seq.length(defaultVals) = 1 then items <- .[items]
        let count = Seq.length(items)
        if count<1 || count <> Seq.length(defaultVals) then failwithf "Rhino.Scripting: GetBoolean failed.  message:'%A' items:'%A' defaultVals:'%A'" message items defaultVals
        let toggles = ResizeArray()
        for i in range(count) do
            let initial = defaultVals.[i]
            let item = items.[i]
            let offVal = item.[1]
            let t = Rhino.Input.Custom.OptionToggle( initial, item.[1], item.[2] )
            toggles.Add(t)
            go.AddOptionToggle(item.[0], t)
        while true:
            let getrc = go.Get()
            if getrc = Rhino.Input.GetResult.Option then continue
            if getrc <> Rhino.Input.GetResult.Nothing then null
            break
        [| for t in toggles do yield t.CurrentValue |]
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


    [<EXT>]
    ///<summary>Pauses for user input of a box</summary>
    ///<param name="mode">(int) Optional, Default Value: <c>0</c>
    ///The box selection mode.
    ///  0 = All modes
    ///  1 = Corner. The base rectangle is created by picking two corner points
    ///  2 = 3-Point. The base rectangle is created by picking three points
    ///  3 = Vertical. The base vertical rectangle is created by picking three points.
    ///  4 = Center. The base rectangle is created by picking a center point and a corner point</param>
    ///<param name="basisPoint">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///Optional 3D base point</param>
    ///<param name="prompt1">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt1 of 'optional prompts to set' (FIXME 0)</param>
    ///<param name="prompt2">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt2 of 'optional prompts to set' (FIXME 0)</param>
    ///<param name="prompt3">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt3 of 'optional prompts to set' (FIXME 0)</param>
    ///<returns>(Point3d seq) list of eight Point3d that define the corners of the box on success</returns>
    static member GetBox([<OPT;DEF(0)>]mode:int, [<OPT;DEF(Point3d())>]basisPoint:Point3d, [<OPT;DEF(null:string)>]prompt1:string, [<OPT;DEF(null:string)>]prompt2:string, [<OPT;DEF(null:string)>]prompt3:string) : Point3d seq =
        let basisPoint = RhinoScriptSyntax.Coerce3dPoint(basisPoint)
        if basisPoint = null then basisPoint <- Point3d.Unset
        def intToEnum(m):
          if m not <| in range(1,5) then
            let m = 0
          {
            0 : Rhino.Input.GetBoxMode.All,
            1 : Rhino.Input.GetBoxMode.Corner,
            2 : Rhino.Input.GetBoxMode.ThreePoint,
            3 : Rhino.Input.GetBoxMode.Vertical,
            4 : Rhino.Input.GetBoxMode.Center
          }.[m]
        let rc, box = Rhino.Input.RhinoGet.GetBox(intToEnum(mode), basisPoint, prompt1, prompt2, prompt3)
        if rc = Rhino.Commands.Result.Success then tuple(box.GetCorners())
    (*
    def GetBox(mode=0, base_point=None, prompt1=None, prompt2=None, prompt3=None):
        '''Pauses for user input of a box
        Parameters:
          mode (number): The box selection mode.
             0 = All modes
             1 = Corner. The base rectangle is created by picking two corner points
             2 = 3-Point. The base rectangle is created by picking three points
             3 = Vertical. The base vertical rectangle is created by picking three points.
             4 = Center. The base rectangle is created by picking a center point and a corner point
          base_point (point, optional): optional 3D base point
          prompt1, prompt2, prompt3 (str, optional): optional prompts to set
        Returns:
          list(point, ...): list of eight Point3d that define the corners of the box on success
          None: is not successful, or on error
        '''
    
        base_point = rhutil.coerce3dpoint(base_point)
        if base_point is None: base_point = Rhino.Geometry.Point3d.Unset
        def intToEnum(m):
          if m not in range(1,5):
            m = 0
          return {
            0 : Rhino.Input.GetBoxMode.All,
            1 : Rhino.Input.GetBoxMode.Corner,
            2 : Rhino.Input.GetBoxMode.ThreePoint,
            3 : Rhino.Input.GetBoxMode.Vertical,
            4 : Rhino.Input.GetBoxMode.Center
          }[m]
        rc, box = Rhino.Input.RhinoGet.GetBox(intToEnum(mode), base_point, prompt1, prompt2, prompt3)
        if rc==Rhino.Commands.Result.Success: return tuple(box.GetCorners())
    *)


    [<EXT>]
    ///<summary>Display the Rhino color picker dialog allowing the user to select an RGB color</summary>
    ///<param name="color">(Drawing.Color) Optional, Default Value: <c>Drawing.Color()</c>
    ///Default RGB value. If omitted, the default color is black</param>
    ///<returns>(Drawing.Color) RGB tuple of three numbers on success</returns>
    static member GetColor([<OPT;DEF(Drawing.Color())>]color:Drawing.Color) : Drawing.Color =
        let color = RhinoScriptSyntax.CoerceColor(color)
        if color = null then color <- Drawing.Color.Black
        let rc, color = Rhino.UI.Dialogs.ShowColorDialog(color)
        if notNull rc then color.R, color.G, color.B
        failwithf "Rhino.Scripting: GetColor failed.  color:'%A'" color
    (*
    def GetColor(color=None):
        '''Display the Rhino color picker dialog allowing the user to select an RGB color
        Parameters:
          color (color, optional): default RGB value. If omitted, the default color is black
        Returns:
          color: RGB tuple of three numbers on success
          None: on error
        '''
    
        color = rhutil.coercecolor(color)
        if color is None: color = System.Drawing.Color.Black
        rc, color = Rhino.UI.Dialogs.ShowColorDialog(color)
        if rc: return color.R, color.G, color.B
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Retrieves the cursor's position</summary>
    ///<returns>(Point3d * Point3d * Guid * Point3d) containing the following information
    ///  0  cursor position in world coordinates
    ///  1  cursor position in screen coordinates
    ///  2  id of the active viewport
    ///  3  cursor position in client coordinates</returns>
    static member GetCursorPos() : Point3d * Point3d * Guid * Point3d =
        let view = Doc.Views.ActiveView
        let screen_pt = Rhino.UI.MouseCursor.Location
        let client_pt = view.ScreenToClient(screen_pt)
        let viewport = view.ActiveViewport
        let xf = viewport.GetTransform(Rhino.DocObjects.CoordinateScreen, Rhino.DocObjects.CoordinateWorld)
        let world_pt = Point3d(client_pt.X, client_pt.Y, 0)
        world_pt.Transform(xf)
        world_pt, screen_pt, viewport.Id, client_pt
    (*
    def GetCursorPos():
        '''Retrieves the cursor's position
        Returns:
          tuple(point, point, guid, point): containing the following information
            0  cursor position in world coordinates
            1  cursor position in screen coordinates
            2  id of the active viewport
            3  cursor position in client coordinates
        '''
    
        view = scriptcontext.doc.Views.ActiveView
        screen_pt = Rhino.UI.MouseCursor.Location
        client_pt = view.ScreenToClient(screen_pt)
        viewport = view.ActiveViewport
        xf = viewport.GetTransform(Rhino.DocObjects.CoordinateSystem.Screen, Rhino.DocObjects.CoordinateSystem.World)
        world_pt = Rhino.Geometry.Point3d(client_pt.X, client_pt.Y, 0)
        world_pt.Transform(xf)
        return world_pt, screen_pt, viewport.Id, client_pt
    *)


    [<EXT>]
    ///<summary>Pauses for user input of a distance.</summary>
    ///<param name="firstPt">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///First distance point</param>
    ///<param name="distance">(float) Optional, Default Value: <c>7e89</c>
    ///Default distance</param>
    ///<param name="firstPtMsg">(string) Optional, Default Value: <c>"First distance point"</c>
    ///Prompt for the first distance point</param>
    ///<param name="secondPtMsg">(string) Optional, Default Value: <c>"Second distance point"</c>
    ///Prompt for the second distance point</param>
    ///<returns>(float) The distance between the two points .</returns>
    static member GetDistance([<OPT;DEF(Point3d())>]firstPt:Point3d, [<OPT;DEF(7e89)>]distance:float, [<OPT;DEF("First distance point")>]firstPtMsg:string, [<OPT;DEF("Second distance point")>]secondPtMsg:string) : float =
        if distance <> null && first_pt = null then
            failwithf "Rhino.Scripting: The "first_pt" parameter needs a value if "distance" is not null..  firstPt:'%A' distance:'%A' firstPtMsg:'%A' secondPtMsg:'%A'" firstPt distance firstPtMsg secondPtMsg
        if distance <> null && not <| (isinstance(distance, int) || isinstance(distance, float)) then null
        if first_pt_msg = null || not <| isinstance(first_pt_msg, str) then null
        if secondPtMsg = null || not <| isinstance(secondPtMsg, str) then null
        if first_pt <> null then
          if first_pt = 0 then first_pt <- (0,0,0)
          let first_pt = RhinoScriptSyntax.Coerce3dPoint(first_pt)
          if first_pt = null then null
        if first_pt = null then
          first_pt <- GetPoint(first_pt_msg)
          if first_pt = null then null
        // cannot <| use GetPoint for 2nd point because of the need do differentiate
        // between the user accepting none vs cancelling to exactly mimic RhinoScript
        let gp = Rhino.Input.Custom.GetPoint()
        if distance <> null then
          gp.AcceptNothing(true)
          let secondPtMsg = "{0}<{1}>".format(secondPtMsg, distance)
        gp.SetCommandPrompt(secondPtMsg)
        gp.DrawLineFromPoint(first_pt,true)
        gp.EnableDrawLineFromPoint(true)
        let r = gp.Get()
        if r not <| in [Rhino.Input.GetResult.Cancel, Rhino.Input.GetResult.Point, then
          Rhino.Input.GetResult.Nothing]: errorHandler()
        if r = Rhino.Input.GetResult.Cancel then null
        if r = Rhino.Input.GetResult.Point then
          let second_pt = gp.Point()
          let distance = second_pt.DistanceTo(first_pt)
        gp.Dispose()
        print "Distance: {0}".format(distance)
        distance
    (*
    def GetDistance(first_pt=None, distance=None, first_pt_msg="First distance point", second_pt_msg="Second distance point"):
        '''Pauses for user input of a distance.
        Parameters:
          first_pt (point, optional): First distance point
          distance (number, optional): Default distance
          first_pt_msg (str, optional): Prompt for the first distance point
          second_pt_msg (str, optional): Prompt for the second distance point
        Returns:
          number: The distance between the two points if successful.
          None: if not successful, or on error.
        '''
    
        if distance is not None and first_pt is None:
            raise Exception("The 'first_pt' parameter needs a value if 'distance' is not None.")
        if distance is not None and not (isinstance(distance, int) or isinstance(distance, float)): return None
        if first_pt_msg is None or not isinstance(first_pt_msg, str): return None
        if second_pt_msg is None or not isinstance(second_pt_msg, str): return None
    
        if first_pt is not None:
          if first_pt == 0: first_pt = (0,0,0)
          first_pt = rhutil.coerce3dpoint(first_pt)
          if first_pt is None: return None
    
        if first_pt is None:
          first_pt = GetPoint(first_pt_msg)
          if first_pt is None: return None
    
        # cannot use GetPoint for 2nd point because of the need do differentiate
        # between the user accepting none vs cancelling to exactly mimic RhinoScript
        gp = Rhino.Input.Custom.GetPoint()
        if distance is not None:
          gp.AcceptNothing(True)
          second_pt_msg = "{0}<{1}>".format(second_pt_msg, distance)
        gp.SetCommandPrompt(second_pt_msg)
        gp.DrawLineFromPoint(first_pt,True)
        gp.EnableDrawLineFromPoint(True)
        r = gp.Get()
        if r not in [Rhino.Input.GetResult.Cancel, Rhino.Input.GetResult.Point,
          Rhino.Input.GetResult.Nothing]: return scriptcontext.errorHandler()
        if r == Rhino.Input.GetResult.Cancel: return None
        if r == Rhino.Input.GetResult.Point:
          second_pt = gp.Point()
          distance = second_pt.DistanceTo(first_pt)
        gp.Dispose()
    
        print "Distance: {0}".format(distance)
        return distance
    *)


    [<EXT>]
    ///<summary>Prompt the user to pick one or more surface or polysurface edge curves</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message.</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///Minimum number of edges to select.</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///Maximum number of edges to select.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the duplicated edge curves.</param>
    ///<returns>(Guid seq) of selection prompts (curve id, parent id, selection point)</returns>
    static member GetEdgeCurves([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(1)>]minCount:int, [<OPT;DEF(0)>]maxCount:int, [<OPT;DEF(false)>]select:bool) : Guid seq =
        if min_count<0 || (maxCount>0 && min_count>maxCount) then 
        if not <| message then message <- "Select Edges"
        let go = Rhino.Input.Custom.GetObject()
        go.SetCommandPrompt(message)
        let go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve
        let go.GeometryAttributeFilter = Rhino.Input.Custom.GeometryAttributeFilter.EdgeCurve
        go.EnablePreSelect(false, true)
        let rc = go.GetMultiple(min_count, maxCount)
        if rc <> Rhino.Input.GetResult.Object then 
        rc <- []
        for i in range(go.ObjectCount) do
            let edge = go.Object(i).Edge()
            if not <| edge then continue
            edge <- edge.Duplicate()
            let curve_id = Doc.Objects.AddCurve(edge)
            let parent_id = go.Object(i).ObjectId
            let pt = go.Object(i).SelectionPoint()
            rc.Add( (curve_id, parent_id, pt) )
        if notNull select then
            for item in rc do
                let rhobj = Doc.Objects.Find(item.[0])
                rhobj.Select(true)
            Doc.Views.Redraw()
        rc
    (*
    def GetEdgeCurves(message=None, min_count=1, max_count=0, select=False):
        '''Prompt the user to pick one or more surface or polysurface edge curves
        Parameters:
          message  (str, optional):  A prompt or message.
          min_count (number, optional): minimum number of edges to select.
          max_count (number, optional): maximum number of edges to select.
          select (bool, optional): Select the duplicated edge curves.
        Returns:
          list(tuple[guid, point, point], ...): of selection prompts (curve id, parent id, selection point)
          None: if not successful
        '''
    
        if min_count<0 or (max_count>0 and min_count>max_count): return
        if not message: message = "Select Edges"
        go = Rhino.Input.Custom.GetObject()
        go.SetCommandPrompt(message)
        go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve
        go.GeometryAttributeFilter = Rhino.Input.Custom.GeometryAttributeFilter.EdgeCurve
        go.EnablePreSelect(False, True)
        rc = go.GetMultiple(min_count, max_count)
        if rc!=Rhino.Input.GetResult.Object: return
        rc = []
        for i in range(go.ObjectCount):
            edge = go.Object(i).Edge()
            if not edge: continue
            edge = edge.Duplicate()
            curve_id = scriptcontext.doc.Objects.AddCurve(edge)
            parent_id = go.Object(i).ObjectId
            pt = go.Object(i).SelectionPoint()
            rc.append( (curve_id, parent_id, pt) )
        if select:
            for item in rc:
                rhobj = scriptcontext.doc.Objects.Find(item[0])
                rhobj.Select(True)
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Pauses for user input of a whole number.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message.</param>
    ///<param name="number">(float) Optional, Default Value: <c>7e89</c>
    ///A default whole number value.</param>
    ///<param name="minimum">(float) Optional, Default Value: <c>7e89</c>
    ///A minimum allowable value.</param>
    ///<param name="maximum">(float) Optional, Default Value: <c>7e89</c>
    ///A maximum allowable value.</param>
    ///<returns>(float) The whole number input by the user .</returns>
    static member GetInteger([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(7e89)>]number:float, [<OPT;DEF(7e89)>]minimum:float, [<OPT;DEF(7e89)>]maximum:float) : float =
        let gi = Rhino.Input.Custom.GetInteger()
        if notNull message then gi.SetCommandPrompt(message)
        if number <> null then gi.SetDefaultInteger(number)
        if minimum <> null then gi.SetLowerLimit(minimum, false)
        if maximum <> null then gi.SetUpperLimit(maximum, false)
        if gi.Get() <> Rhino.Input.GetResult.Number then failwithf "Rhino.Scripting: GetInteger failed.  message:'%A' number:'%A' minimum:'%A' maximum:'%A'" message number minimum maximum
        let rc = gi.Number()
        gi.Dispose()
        rc
    (*
    def GetInteger(message=None, number=None, minimum=None, maximum=None):
        '''Pauses for user input of a whole number.
        Parameters:
          message (str, optional): A prompt or message.
          number (number, optional): A default whole number value.
          minimum (number, optional): A minimum allowable value.
          maximum (number, optional): A maximum allowable value.
        Returns:
           number: The whole number input by the user if successful.
           None: if not successful, or on error
        '''
    
        gi = Rhino.Input.Custom.GetInteger()
        if message: gi.SetCommandPrompt(message)
        if number is not None: gi.SetDefaultInteger(number)
        if minimum is not None: gi.SetLowerLimit(minimum, False)
        if maximum is not None: gi.SetUpperLimit(maximum, False)
        if gi.Get()!=Rhino.Input.GetResult.Number: return scriptcontext.errorhandler()
        rc = gi.Number()
        gi.Dispose()
        return rc
    *)


    [<EXT>]
    ///<summary>Displays dialog box prompting the user to select a layer</summary>
    ///<param name="title">(string) Optional, Default Value: <c>"Select Layer"</c>
    ///Dialog box title</param>
    ///<param name="layer">(string) Optional, Default Value: <c>null:string</c>
    ///Name of a layer to preselect. If omitted, the current layer will be preselected</param>
    ///<param name="showNewButton">(bool) Optional, Default Value: <c>false</c>
    ///Show new button of 'Optional buttons to show on the dialog' (FIXME 0)</param>
    ///<param name="showSetCurrent">(bool) Optional, Default Value: <c>false</c>
    ///Show set current of 'Optional buttons to show on the dialog' (FIXME 0)</param>
    ///<returns>(string) name of selected layer</returns>
    static member GetLayer([<OPT;DEF("Select Layer")>]title:string, [<OPT;DEF(null:string)>]layer:string, [<OPT;DEF(false)>]showNewButton:bool, [<OPT;DEF(false)>]showSetCurrent:bool) : string =
        let layer_index = Doc.Layers.CurrentLayerIndex
        if notNull layer then
            let layer_instance = Doc.Layers.FindName(layer)
            if layer_instance <> null then layer_index <- layer_instance.Index
        let rc = Rhino.UI.Dialogs.ShowSelectLayerDialog(layer_index, title, show_new_button, showSetCurrent, true)
        if rc.[0] <> true then null
        let layer = Doc.Layers.[rc.[1]]
        layer.FullPath
    (*
    def GetLayer(title="Select Layer", layer=None, show_new_button=False, show_set_current=False):
        '''Displays dialog box prompting the user to select a layer
        Parameters:
          title (str, optional): dialog box title
          layer (str, optional): name of a layer to preselect. If omitted, the current layer will be preselected
          show_new_button, show_set_current (bool, optional): Optional buttons to show on the dialog
        Returns:
          str: name of selected layer if successful
          None: on error
        '''
    
        layer_index = scriptcontext.doc.Layers.CurrentLayerIndex
        if layer:
            layer_instance = scriptcontext.doc.Layers.FindName(layer)
            if layer_instance is not None: layer_index = layer_instance.Index
        rc = Rhino.UI.Dialogs.ShowSelectLayerDialog(layer_index, title, show_new_button, show_set_current, True)
        if rc[0]!=True: return None
        layer = scriptcontext.doc.Layers[rc[1]]
        return layer.FullPath
    *)


    [<EXT>]
    ///<summary>Displays a dialog box prompting the user to select one or more layers</summary>
    ///<param name="title">(string) Optional, Default Value: <c>"Select Layers"</c>
    ///Dialog box title</param>
    ///<param name="showNewButton">(bool) Optional, Default Value: <c>false</c>
    ///Optional button to show on the dialog</param>
    ///<returns>(string) The names of selected layers</returns>
    static member GetLayers([<OPT;DEF("Select Layers")>]title:string, [<OPT;DEF(false)>]showNewButton:bool) : string =
        let rc, layer_indices = Rhino.UI.Dialogs.ShowSelectMultipleLayersDialog(null, title, showNewButton)
        if notNull rc then
            .[Doc.Layers.[index].FullPath for index in layer_indices]
    (*
    def GetLayers(title="Select Layers", show_new_button=False):
        '''Displays a dialog box prompting the user to select one or more layers
        Parameters:
          title (str, optional):  dialog box title
          show_new_button (bool, optional): Optional button to show on the dialog
        Returns:
          str: The names of selected layers if successful
        '''
    
        rc, layer_indices = Rhino.UI.Dialogs.ShowSelectMultipleLayersDialog(None, title, show_new_button)
        if rc:
            return [scriptcontext.doc.Layers[index].FullPath for index in layer_indices]
    *)


    [<EXT>]
    ///<summary>Prompts the user to pick points that define a line</summary>
    ///<param name="mode">(int) Optional, Default Value: <c>0</c>
    ///Line definition mode.
    ///  0  Default - Show all modes, start in two-point mode
    ///  1  Two-point - Defines a line from two points.
    ///  2  Normal - Defines a line normal to a location on a surface.
    ///  3  Angled - Defines a line at a specified angle from a reference line.
    ///  4  Vertical - Defines a line vertical to the construction plane.
    ///  5  Four-point - Defines a line using two points to establish direction and two points to establish length.
    ///  6  Bisector - Defines a line that bisects a specified angle.
    ///  7  Perpendicular - Defines a line perpendicular to or from a curve
    ///  8  Tangent - Defines a line tangent from a curve.
    ///  9  Extension - Defines a line that extends from a curve.</param>
    ///<param name="point">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///Optional starting point</param>
    ///<param name="message1">(string) Optional, Default Value: <c>null:string</c>
    ///Message1 of 'optional prompts' (FIXME 0)</param>
    ///<param name="message2">(string) Optional, Default Value: <c>null:string</c>
    ///Message2 of 'optional prompts' (FIXME 0)</param>
    ///<param name="message3">(string) Optional, Default Value: <c>null:string</c>
    ///Message3 of 'optional prompts' (FIXME 0)</param>
    ///<returns>(Line) Tuple of two points on success</returns>
    static member GetLine([<OPT;DEF(0)>]mode:int, [<OPT;DEF(Point3d())>]point:Point3d, [<OPT;DEF(null:string)>]message1:string, [<OPT;DEF(null:string)>]message2:string, [<OPT;DEF(null:string)>]message3:string) : Line =
        let gl = Rhino.Input.Custom.GetLine()
        if mode = 0 then gl.EnableAllVariations(true)
        else  gl.GetLineMode <- Enum.ToObject( Rhino.Input.Custom.GetLineMode, mode-1 )
        if notNull point then
            let point = RhinoScriptSyntax.Coerce3dPoint(point)
            gl.SetFirstPoint(point)
        if notNull message1 then gl.FirstPointPrompt <- message1
        if notNull message2 then gl.MidPointPrompt <- message2
        if notNull message3 then gl.SecondPointPrompt <- message3
        let rc, line = gl.Get()
        if rc = Rhino.Commands.Result.Success then line.From, line.To
    (*
    def GetLine(mode=0, point=None, message1=None, message2=None, message3=None):
        '''Prompts the user to pick points that define a line
        Parameters:
          mode (number, optional): line definition mode.
            0  Default - Show all modes, start in two-point mode
            1  Two-point - Defines a line from two points.
            2  Normal - Defines a line normal to a location on a surface.
            3  Angled - Defines a line at a specified angle from a reference line.
            4  Vertical - Defines a line vertical to the construction plane.
            5  Four-point - Defines a line using two points to establish direction and two points to establish length.
            6  Bisector - Defines a line that bisects a specified angle.
            7  Perpendicular - Defines a line perpendicular to or from a curve
            8  Tangent - Defines a line tangent from a curve.
            9  Extension - Defines a line that extends from a curve.
          point (point, optional): optional starting point
          message1, message2, message3 (str, optional): optional prompts
        Returns:
          line: Tuple of two points on success
          None: on error
        '''
    
        gl = Rhino.Input.Custom.GetLine()
        if mode==0: gl.EnableAllVariations(True)
        else: gl.GetLineMode = System.Enum.ToObject( Rhino.Input.Custom.GetLineMode, mode-1 )
        if point:
            point = rhutil.coerce3dpoint(point)
            gl.SetFirstPoint(point)
        if message1: gl.FirstPointPrompt = message1
        if message2: gl.MidPointPrompt = message2
        if message3: gl.SecondPointPrompt = message3
        rc, line = gl.Get()
        if rc==Rhino.Commands.Result.Success: return line.From, line.To
    *)


    [<EXT>]
    ///<summary>Displays a dialog box prompting the user to select one linetype</summary>
    ///<param name="defaultValLinetyp">(string) Optional, Default Value: <c>null:string</c>
    ///Optional. The name of the linetype to select. If omitted, the current linetype will be selected.</param>
    ///<param name="showByLayer">(bool) Optional, Default Value: <c>false</c>
    ///If True, the "by Layer" linetype will show. Defaults to False.</param>
    ///<returns>(string) The names of selected linetype</returns>
    static member GetLinetype([<OPT;DEF(null:string)>]defaultValLinetyp:string, [<OPT;DEF(false)>]showByLayer:bool) : string =
        let lt_instance = Doc.Linetyps.CurrentLinetyp
        if default_linetyp then
            let lt_new = Doc.Linetyps.FindName(default_linetyp)
            if lt_new <> null then lt_instance <- lt_new
        let id = Rhino.UI.Dialogs.ShowLineTypes("Select Linetyp", "", doc)
        if id = "" then null
        let linetyp = Doc.Linetyps.FindId(id)
        linetyp.Name
    (*
    def GetLinetype(default_linetype=None, show_by_layer=False):
        '''Displays a dialog box prompting the user to select one linetype
        Parameters:
          default_linetype (str, optional):  Optional. The name of the linetype to select. If omitted, the current linetype will be selected.
          show_by_layer (bool, optional): If True, the "by Layer" linetype will show. Defaults to False.
        Returns:
          str: The names of selected linetype if successful
        '''
    
        lt_instance = scriptcontext.doc.Linetypes.CurrentLinetype
        if default_linetype:
            lt_new = scriptcontext.doc.Linetypes.FindName(default_linetype)
            if lt_new is not None: lt_instance = lt_new
        id = Rhino.UI.Dialogs.ShowLineTypes("Select Linetype", "", scriptcontext.doc)
        if id == "": return None
        linetype = scriptcontext.doc.Linetypes.FindId(id)
        return linetype.Name
    *)


    [<EXT>]
    ///<summary>Prompts the user to pick one or more mesh faces</summary>
    ///<param name="objectId">(Guid) The mesh object's identifier</param>
    ///<param name="message">(string) Optional, Default Value: <c>""</c>
    ///A prompt of message</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///The minimum number of faces to select</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///The maximum number of faces to select.
    ///  If 0, the user must press enter to finish selection.
    ///  If -1, selection stops as soon as there are at least minCount faces selected.</param>
    ///<returns>(float seq) of mesh face indices on success</returns>
    static member GetMeshFaces(objectId:Guid, [<OPT;DEF("")>]message:string, [<OPT;DEF(1)>]minCount:int, [<OPT;DEF(0)>]maxCount:int) : float seq =
        Doc.Objects.UnselectAll()
        Doc.Views.Redraw()
        let object_id = RhinoScriptSyntax.CoerceGuid(object_id)
        def FilterById( rhino_object, geometry, component_index ):
            object_id = rhino_object.Id
        let go = Rhino.Input.Custom.GetObject()
        go.SetCustomGeometryFilter(FilterById)
        if notNull message then go.SetCommandPrompt(message)
        let go.GeometryFilter = Rhino.DocObjects.ObjectType.MeshFace
        go.AcceptNothing(true)
        if go.GetMultiple(min_count,maxCount) <> Rhino.Input.GetResult.Object then null
        let objrefs = go.Objects()
        let rc = [| for item in objrefs do yield item.GeometryComponentIndex.Index |]
        go.Dispose()
        rc
    (*
    def GetMeshFaces(object_id, message="", min_count=1, max_count=0):
        '''Prompts the user to pick one or more mesh faces
        Parameters:
          object_id (guid): the mesh object's identifier
          message (str, optional): a prompt of message
          min_count (number, optional): the minimum number of faces to select
          max_count (number, optional): the maximum number of faces to select.
            If 0, the user must press enter to finish selection.
            If -1, selection stops as soon as there are at least min_count faces selected.
        Returns:
          list(number, ...): of mesh face indices on success
          None: on error
        '''
    
        scriptcontext.doc.Objects.UnselectAll()
        scriptcontext.doc.Views.Redraw()
        object_id = rhutil.coerceguid(object_id, True)
        def FilterById( rhino_object, geometry, component_index ):
            return object_id == rhino_object.Id
        go = Rhino.Input.Custom.GetObject()
        go.SetCustomGeometryFilter(FilterById)
        if message: go.SetCommandPrompt(message)
        go.GeometryFilter = Rhino.DocObjects.ObjectType.MeshFace
        go.AcceptNothing(True)
        if go.GetMultiple(min_count,max_count)!=Rhino.Input.GetResult.Object: return None
        objrefs = go.Objects()
        rc = [item.GeometryComponentIndex.Index for item in objrefs]
        go.Dispose()
        return rc
    *)


    [<EXT>]
    ///<summary>Prompts the user to pick one or more mesh vertices</summary>
    ///<param name="objectId">(Guid) The mesh object's identifier</param>
    ///<param name="message">(string) Optional, Default Value: <c>""</c>
    ///A prompt of message</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///The minimum number of vertices to select</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///The maximum number of vertices to select. If 0, the user must
    ///  press enter to finish selection. If -1, selection stops as soon as there
    ///  are at least minCount vertices selected.</param>
    ///<returns>(float seq) of mesh vertex indices on success</returns>
    static member GetMeshVertices(objectId:Guid, [<OPT;DEF("")>]message:string, [<OPT;DEF(1)>]minCount:int, [<OPT;DEF(0)>]maxCount:int) : float seq =
        Doc.Objects.UnselectAll()
        Doc.Views.Redraw()
        let object_id = RhinoScriptSyntax.CoerceGuid(object_id)
        class CustomGetObject(Rhino.Input.Custom.GetObject):
            def CustomGeometryFilter( self, rhino_object, geometry, component_index ):
                object_id = rhino_object.Id
        let go = CustomGetObject()
        if notNull message then go.SetCommandPrompt(message)
        let go.GeometryFilter = Rhino.DocObjects.ObjectType.MeshVertex
        go.AcceptNothing(true)
        if go.GetMultiple(min_count,maxCount) <> Rhino.Input.GetResult.Object then null
        let objrefs = go.Objects()
        let rc = [| for item in objrefs do yield item.GeometryComponentIndex.Index |]
        go.Dispose()
        rc
    (*
    def GetMeshVertices(object_id, message="", min_count=1, max_count=0):
        '''Prompts the user to pick one or more mesh vertices
        Parameters:
          object_id (guid): the mesh object's identifier
          message (str, optional): a prompt of message
          min_count (number, optional): the minimum number of vertices to select
          max_count (number, optional): the maximum number of vertices to select. If 0, the user must
            press enter to finish selection. If -1, selection stops as soon as there
            are at least min_count vertices selected.
        Returns:
          list(number, ...): of mesh vertex indices on success
          None: on error
        '''
    
        scriptcontext.doc.Objects.UnselectAll()
        scriptcontext.doc.Views.Redraw()
        object_id = rhutil.coerceguid(object_id, True)
        class CustomGetObject(Rhino.Input.Custom.GetObject):
            def CustomGeometryFilter( self, rhino_object, geometry, component_index ):
                return object_id == rhino_object.Id
        go = CustomGetObject()
        if message: go.SetCommandPrompt(message)
        go.GeometryFilter = Rhino.DocObjects.ObjectType.MeshVertex
        go.AcceptNothing(True)
        if go.GetMultiple(min_count,max_count)!=Rhino.Input.GetResult.Object: return None
        objrefs = go.Objects()
        rc = [item.GeometryComponentIndex.Index for item in objrefs]
        go.Dispose()
        return rc
    *)


    [<EXT>]
    ///<summary>Pauses for user input of a point.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message.</param>
    ///<param name="basisPoint">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///List of 3 numbers or Point3d identifying a starting, or base point</param>
    ///<param name="distance">(float) Optional, Default Value: <c>7e89</c>
    ///Constraining distance. If distance is specified, basePoint must also be specified.</param>
    ///<param name="inPlane">(bool) Optional, Default Value: <c>false</c>
    ///Constrains the point selections to the active construction plane.</param>
    ///<returns>(Point3d) point on success</returns>
    static member GetPoint([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(Point3d())>]basisPoint:Point3d, [<OPT;DEF(7e89)>]distance:float, [<OPT;DEF(false)>]inPlane:bool) : Point3d =
        let gp = Rhino.Input.Custom.GetPoint()
        if notNull message then gp.SetCommandPrompt(message)
        let basis_point = RhinoScriptSyntax.Coerce3dPoint(basis_point)
        if basis_point then
            gp.DrawLineFromPoint(basis_point,true)
            gp.EnableDrawLineFromPoint(true)
            if notNull distance then gp.ConstrainDistanceFromBasePoint(distance)
        if notNull inPlane then gp.ConstrainToConstructionPlane(true)
        gp.Get()
        if gp.CommandResult() <> Rhino.Commands.Result.Success then
            failwithf "Rhino.Scripting: GetPoint failed.  message:'%A' basisPoint:'%A' distance:'%A' inPlane:'%A'" message basisPoint distance inPlane
        let pt = gp.Point()
        gp.Dispose()
        pt
    (*
    def GetPoint(message=None, base_point=None, distance=None, in_plane=False):
        '''Pauses for user input of a point.
        Parameters:
          message (str, optional): A prompt or message.
          base_point (point, optional): list of 3 numbers or Point3d identifying a starting, or base point
          distance  (number, optional): constraining distance. If distance is specified, basePoint must also be specified.
          in_plane (bool, optional): constrains the point selections to the active construction plane.
        Returns:
          point: point on success
          None: if no point picked or user canceled
        '''
    
        gp = Rhino.Input.Custom.GetPoint()
        if message: gp.SetCommandPrompt(message)
        base_point = rhutil.coerce3dpoint(base_point)
        if base_point:
            gp.DrawLineFromPoint(base_point,True)
            gp.EnableDrawLineFromPoint(True)
            if distance: gp.ConstrainDistanceFromBasePoint(distance)
        if in_plane: gp.ConstrainToConstructionPlane(True)
        gp.Get()
        if gp.CommandResult()!=Rhino.Commands.Result.Success:
            return scriptcontext.errorhandler()
        pt = gp.Point()
        gp.Dispose()
        return pt
    *)


    [<EXT>]
    ///<summary>Pauses for user input of a point constrainted to a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt of message</param>
    ///<returns>(Point3d) 3d point</returns>
    static member GetPointOnCurve(curveId:Guid, [<OPT;DEF(null:string)>]message:string) : Point3d =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId, -1)
        let gp = Rhino.Input.Custom.GetPoint()
        if notNull message then gp.SetCommandPrompt(message)
        gp.Constrain(curve, false)
        gp.Get()
        if gp.CommandResult() <> Rhino.Commands.Result.Success then
            failwithf "Rhino.Scripting: GetPointOnCurve failed.  curveId:'%A' message:'%A'" curveId message
        let pt = gp.Point()
        gp.Dispose()
        pt
    (*
    def GetPointOnCurve(curve_id, message=None):
        '''Pauses for user input of a point constrainted to a curve object
        Parameters:
          curve_id (guid): identifier of the curve to get a point on
          message (str, optional): a prompt of message
        Returns:
          point: 3d point if successful
          None: on error
        '''
    
        curve = rhutil.coercecurve(curve_id, -1, True)
        gp = Rhino.Input.Custom.GetPoint()
        if message: gp.SetCommandPrompt(message)
        gp.Constrain(curve, False)
        gp.Get()
        if gp.CommandResult()!=Rhino.Commands.Result.Success:
            return scriptcontext.errorhandler()
        pt = gp.Point()
        gp.Dispose()
        return pt
    *)


    [<EXT>]
    ///<summary>Pauses for user input of a point constrained to a mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of the mesh to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message</param>
    ///<returns>(Point3d) 3d point</returns>
    static member GetPointOnMesh(meshId:Guid, [<OPT;DEF(null:string)>]message:string) : Point3d =
        let meshId = RhinoScriptSyntax.CoerceGuid(meshId)
        if not <| message then message <- "Point"
        let cmdrc, point = Rhino.Input.RhinoGet.GetPointOnMesh(meshId, message, false)
        if cmdrc = Rhino.Commands.Result.Success then point
    (*
    def GetPointOnMesh(mesh_id, message=None):
        '''Pauses for user input of a point constrained to a mesh object
        Parameters:
          mesh_id (guid): identifier of the mesh to get a point on
          message (str, optional): a prompt or message
        Returns:
          point: 3d point if successful
          None: on error
        '''
    
        mesh_id = rhutil.coerceguid(mesh_id, True)
        if not message: message = "Point"
        cmdrc, point = Rhino.Input.RhinoGet.GetPointOnMesh(mesh_id, message, False)
        if cmdrc==Rhino.Commands.Result.Success: return point
    *)


    [<EXT>]
    ///<summary>Pauses for user input of a point constrained to a surface or polysurface
    ///  object</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message</param>
    ///<returns>(Point3d) 3d point</returns>
    static member GetPointOnSurface(surfaceId:Guid, [<OPT;DEF(null:string)>]message:string) : Point3d =
        let surfOrBrep = RhinoScriptSyntax.CoerceSurface(surfaceId)
        if not <| surfOrBrep then
            surfOrBrep <- RhinoScriptSyntax.CoerceBrep(surfaceId)
        let gp = Rhino.Input.Custom.GetPoint()
        if notNull message then gp.SetCommandPrompt(message)
        if isinstance(surfOrBrep,Surface) then
            gp.Constrain(surfOrBrep,false)
        else 
            gp.Constrain(surfOrBrep, -1, -1, false)
        gp.Get()
        if gp.CommandResult() <> Rhino.Commands.Result.Success then
            failwithf "Rhino.Scripting: GetPointOnSurface failed.  surfaceId:'%A' message:'%A'" surfaceId message
        let pt = gp.Point()
        gp.Dispose()
        pt
    (*
    def GetPointOnSurface(surface_id, message=None):
        '''Pauses for user input of a point constrained to a surface or polysurface
        object
        Parameters:
          surface_id (guid): identifier of the surface to get a point on
          message (str, optional): a prompt or message
        Returns:
          point: 3d point if successful
          None: on error
        '''
    
        surfOrBrep = rhutil.coercesurface(surface_id)
        if not surfOrBrep:
            surfOrBrep = rhutil.coercebrep(surface_id, True)
        gp = Rhino.Input.Custom.GetPoint()
        if message: gp.SetCommandPrompt(message)
        if isinstance(surfOrBrep,Rhino.Geometry.Surface):
            gp.Constrain(surfOrBrep,False)
        else:
            gp.Constrain(surfOrBrep, -1, -1, False)
        gp.Get()
        if gp.CommandResult()!=Rhino.Commands.Result.Success:
            return scriptcontext.errorhandler()
        pt = gp.Point()
        gp.Dispose()
        return pt
    *)


    [<EXT>]
    ///<summary>Pauses for user input of one or more points</summary>
    ///<param name="drawLines">(bool) Optional, Default Value: <c>false</c>
    ///Draw lines between points</param>
    ///<param name="inPlane">(bool) Optional, Default Value: <c>false</c>
    ///Constrain point selection to the active construction plane</param>
    ///<param name="message1">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message for the first point</param>
    ///<param name="message2">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message for the next points</param>
    ///<param name="maxPoints">(float) Optional, Default Value: <c>7e89</c>
    ///Maximum number of points to pick. If not specified, an
    ///  unlimited number of points can be picked.</param>
    ///<param name="basisPoint">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///A starting or base point</param>
    ///<returns>(Point3d seq) of 3d points</returns>
    static member GetPoints([<OPT;DEF(false)>]drawLines:bool, [<OPT;DEF(false)>]inPlane:bool, [<OPT;DEF(null:string)>]message1:string, [<OPT;DEF(null:string)>]message2:string, [<OPT;DEF(7e89)>]maxPoints:float, [<OPT;DEF(Point3d())>]basisPoint:Point3d) : Point3d seq =
        let gp = Rhino.Input.Custom.GetPoint()
        if notNull message1 then gp.SetCommandPrompt(message1)
        gp.EnableDrawLineFromPoint( draw_lines )
        if in_plane then
            gp.ConstrainToConstructionPlane(true)
            let plane = Doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
            gp.Constrain(plane, false)
        let getres = gp.Get()
        if gp.CommandResult() <> Rhino.Commands.Result.Success then null
        let prevPoint = gp.Point()
        let rc = .[prevPoint]
        if max_points = null || max_points>1 then
            let current_point = 1
            if notNull message2 then gp.SetCommandPrompt(message2)
            def GetPointDynamicDrawFunc( sender, args ):
                if Seq.length(rc)>1 then
                    let c = Rhino.ApplicationSettings.AppearanceSettings.FeedbackColor
                    args.Display.DrawPolyline(rc, c)
            if draw_lines then gp.DynamicDraw +<- GetPointDynamicDrawFunc
            while true:
                if max_points && current_point>=max_points then break
                if draw_lines then gp.DrawLineFromPoint(prevPoint, true)
                gp.SetBasePoint(prevPoint, true)
                current_point += 1
                getres <- gp.Get()
                if getres = Rhino.Input.GetResult.Cancel then break
                if gp.CommandResult() <> Rhino.Commands.Result.Success then null
                prevPoint <- gp.Point()
                rc.Add(prevPoint)
        rc
    (*
    def GetPoints(draw_lines=False, in_plane=False, message1=None, message2=None, max_points=None, base_point=None):
        '''Pauses for user input of one or more points
        Parameters:
          draw_lines (bool, optional): Draw lines between points
          in_plane (bool, optional): Constrain point selection to the active construction plane
          message1 (str, optional): A prompt or message for the first point
          message2 (str, optional): A prompt or message for the next points
          max_points (number, optional): maximum number of points to pick. If not specified, an
                            unlimited number of points can be picked.
          base_point (point, optional): a starting or base point
        Returns:
          list(point, ...): of 3d points if successful
          None: if not successful or on error
        '''
    
        gp = Rhino.Input.Custom.GetPoint()
        if message1: gp.SetCommandPrompt(message1)
        gp.EnableDrawLineFromPoint( draw_lines )
        if in_plane:
            gp.ConstrainToConstructionPlane(True)
            plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
            gp.Constrain(plane, False)
        getres = gp.Get()
        if gp.CommandResult()!=Rhino.Commands.Result.Success: return None
        prevPoint = gp.Point()
        rc = [prevPoint]
        if max_points is None or max_points>1:
            current_point = 1
            if message2: gp.SetCommandPrompt(message2)
            def GetPointDynamicDrawFunc( sender, args ):
                if len(rc)>1:
                    c = Rhino.ApplicationSettings.AppearanceSettings.FeedbackColor
                    args.Display.DrawPolyline(rc, c)
            if draw_lines: gp.DynamicDraw += GetPointDynamicDrawFunc
            while True:
                if max_points and current_point>=max_points: break
                if draw_lines: gp.DrawLineFromPoint(prevPoint, True)
                gp.SetBasePoint(prevPoint, True)
                current_point += 1
                getres = gp.Get()
                if getres==Rhino.Input.GetResult.Cancel: break
                if gp.CommandResult()!=Rhino.Commands.Result.Success: return None
                prevPoint = gp.Point()
                rc.append(prevPoint)
        return rc
    *)


    [<EXT>]
    ///<summary>Prompts the user to pick points that define a polyline.</summary>
    ///<param name="flags">(int) Optional, Default Value: <c>3</c>
    ///The options are bit coded flags. Values can be added together to specify more than one option. The default is 3.
    ///  value description
    ///  1     Permit close option. If specified, then after 3 points have been picked, the user can type "Close" and a closed polyline will be returned.
    ///  2     Permit close snap. If specified, then after 3 points have been picked, the user can pick near the start point and a closed polyline will be returned.
    ///  4     Force close. If specified, then the returned polyline is always closed. If specified, then intMax must be 0 or >= 4.
    ///  Note: the default is 3, or "Permit close option = True", "Permit close snap = True", and "Force close = False".</param>
    ///<param name="message1">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message for the first point.</param>
    ///<param name="message2">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message for the second point.</param>
    ///<param name="message3">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message for the third point.</param>
    ///<param name="message4">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message for the 'next' point.</param>
    ///<param name="min">(float) Optional, Default Value: <c>2</c>
    ///The minimum number of points to require. The default is 2.</param>
    ///<param name="max">(float) Optional, Default Value: <c>0</c>
    ///The maximum number of points to require; 0 for no limit.  The default is 0.</param>
    ///<returns>(Point3d seq) A list of 3-D points that define the polyline .</returns>
    static member GetPolyline([<OPT;DEF(3)>]flags:int, [<OPT;DEF(null:string)>]message1:string, [<OPT;DEF(null:string)>]message2:string, [<OPT;DEF(null:string)>]message3:string, [<OPT;DEF(null:string)>]message4:string, [<OPT;DEF(2)>]min:float, [<OPT;DEF(0)>]max:float) : Point3d seq =
      let gpl = Rhino.Input.Custom.GetPolyline()
      if notNull message1 then gpl.FirstPointPrompt <- message1
      if notNull message2 then gpl.SecondPointPrompt <- message2
      if notNull message3 then gpl.ThirdPointPrompt <- message3
      if notNull message4 then gpl.FourthPointPrompt <- message4
      if notNull min then gpl.MinPointCount <- min
      if notNull max then gpl.MaxPointCount <- max
      let rc, polyline = gpl.Get()
      Doc.Views.Redraw()
      if rc = Rhino.Commands.Result.Success then polyline
      null
    (*
    def GetPolyline(flags=3, message1=None, message2=None, message3=None, message4=None, min=2, max=0):
        '''Prompts the user to pick points that define a polyline.
      Parameters:
        flags (number, optional) The options are bit coded flags. Values can be added together to specify more than one option. The default is 3.
          value description
          1     Permit close option. If specified, then after 3 points have been picked, the user can type "Close" and a closed polyline will be returned.
          2     Permit close snap. If specified, then after 3 points have been picked, the user can pick near the start point and a closed polyline will be returned.
          4     Force close. If specified, then the returned polyline is always closed. If specified, then intMax must be 0 or >= 4.
          Note: the default is 3, or "Permit close option = True", "Permit close snap = True", and "Force close = False".
        message1 (str, optional): A prompt or message for the first point.
        message2 (str, optional): A prompt or message for the second point.
        message3 (str, optional): A prompt or message for the third point.
        message4 (str, optional): A prompt or message for the 'next' point.
        min (number, optional): The minimum number of points to require. The default is 2.
        max (number, optional): The maximum number of points to require; 0 for no limit.  The default is 0.
      Returns:
        list(point, ...): A list of 3-D points that define the polyline if successful.
        None: if not successful or on error
      '''
    
      gpl = Rhino.Input.Custom.GetPolyline()
      if message1: gpl.FirstPointPrompt = message1
      if message2: gpl.SecondPointPrompt = message2
      if message3: gpl.ThirdPointPrompt = message3
      if message4: gpl.FourthPointPrompt = message4
      if min: gpl.MinPointCount = min
      if max: gpl.MaxPointCount = max
      rc, polyline = gpl.Get()
      scriptcontext.doc.Views.Redraw()
      if rc==Rhino.Commands.Result.Success: return polyline
      return None
    *)


    [<EXT>]
    ///<summary>Pauses for user input of a number.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Number"</c>
    ///A prompt or message.</param>
    ///<param name="number">(float) Optional, Default Value: <c>7e89</c>
    ///A default number value.</param>
    ///<param name="minimum">(float) Optional, Default Value: <c>7e89</c>
    ///A minimum allowable value.</param>
    ///<param name="maximum">(float) Optional, Default Value: <c>7e89</c>
    ///A maximum allowable value.</param>
    ///<returns>(float) The number input by the user .</returns>
    static member GetReal([<OPT;DEF("Number")>]message:string, [<OPT;DEF(7e89)>]number:float, [<OPT;DEF(7e89)>]minimum:float, [<OPT;DEF(7e89)>]maximum:float) : float =
        let gn = Rhino.Input.Custom.GetNumber()
        if notNull message then gn.SetCommandPrompt(message)
        if number <> null then gn.SetDefaultNumber(number)
        if minimum <> null then gn.SetLowerLimit(minimum, false)
        if maximum <> null then gn.SetUpperLimit(maximum, false)
        if gn.Get() <> Rhino.Input.GetResult.Number then null
        let rc = gn.Number()
        gn.Dispose()
        rc
    (*
    def GetReal(message="Number", number=None, minimum=None, maximum=None):
        '''Pauses for user input of a number.
        Parameters:
          message (str, optional): A prompt or message.
          number (number, optional): A default number value.
          minimum (number, optional): A minimum allowable value.
          maximum (number, optional): A maximum allowable value.
        Returns:
          number: The number input by the user if successful.
          None: if not successful, or on error
        '''
    
        gn = Rhino.Input.Custom.GetNumber()
        if message: gn.SetCommandPrompt(message)
        if number is not None: gn.SetDefaultNumber(number)
        if minimum is not None: gn.SetLowerLimit(minimum, False)
        if maximum is not None: gn.SetUpperLimit(maximum, False)
        if gn.Get()!=Rhino.Input.GetResult.Number: return None
        rc = gn.Number()
        gn.Dispose()
        return rc
    *)


    [<EXT>]
    ///<summary>Pauses for user input of a rectangle</summary>
    ///<param name="mode">(int) Optional, Default Value: <c>0</c>
    ///The rectangle selection mode. The modes are as follows
    ///  0 = All modes
    ///  1 = Corner - a rectangle is created by picking two corner points
    ///  2 = 3Point - a rectangle is created by picking three points
    ///  3 = Vertical - a vertical rectangle is created by picking three points
    ///  4 = Center - a rectangle is created by picking a center point and a corner point</param>
    ///<param name="basisPoint">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///A 3d base point</param>
    ///<param name="prompt1">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt1 of 'optional prompts' (FIXME 0)</param>
    ///<param name="prompt2">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt2 of 'optional prompts' (FIXME 0)</param>
    ///<param name="prompt3">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt3 of 'optional prompts' (FIXME 0)</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) four 3d points that define the corners of the rectangle</returns>
    static member GetRectangle([<OPT;DEF(0)>]mode:int, [<OPT;DEF(Point3d())>]basisPoint:Point3d, [<OPT;DEF(null:string)>]prompt1:string, [<OPT;DEF(null:string)>]prompt2:string, [<OPT;DEF(null:string)>]prompt3:string) : Point3d * Point3d * Point3d * Point3d =
        let mode :  = LanguagePrimitives.EnumOfValue  Rhino.Input.GetBoxMode, mode )
        let basisPoint = RhinoScriptSyntax.Coerce3dPoint(basisPoint)
        if( basisPoint = null ) then basisPoint <- Point3d.Unset
        let prompts = .["", "", ""]
        if notNull prompt1 then prompts.[0] <- prompt1
        if notNull prompt2 then prompts.[1] <- prompt2
        if notNull prompt3 then prompts.[2] <- prompt3
        let rc, corners = Rhino.Input.RhinoGet.GetRectangle(mode, basisPoint, prompts)
        if rc = Rhino.Commands.Result.Success then corners
        null
    (*
    def GetRectangle(mode=0, base_point=None, prompt1=None, prompt2=None, prompt3=None):
        '''Pauses for user input of a rectangle
        Parameters:
          mode (number, optional): The rectangle selection mode. The modes are as follows
              0 = All modes
              1 = Corner - a rectangle is created by picking two corner points
              2 = 3Point - a rectangle is created by picking three points
              3 = Vertical - a vertical rectangle is created by picking three points
              4 = Center - a rectangle is created by picking a center point and a corner point
          base_point (point, optional): a 3d base point
          prompt1, prompt2, prompt3 (str, optional): optional prompts
        Returns:
          tuple(point, point, point, point): four 3d points that define the corners of the rectangle
          None: on error
        '''
    
        mode = System.Enum.ToObject( Rhino.Input.GetBoxMode, mode )
        base_point = rhutil.coerce3dpoint(base_point)
        if( base_point==None ): base_point = Rhino.Geometry.Point3d.Unset
        prompts = ["", "", ""]
        if prompt1: prompts[0] = prompt1
        if prompt2: prompts[1] = prompt2
        if prompt3: prompts[2] = prompt3
        rc, corners = Rhino.Input.RhinoGet.GetRectangle(mode, base_point, prompts)
        if rc==Rhino.Commands.Result.Success: return corners
        return None
    *)


    [<EXT>]
    ///<summary>Pauses for user input of a string value</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message</param>
    ///<param name="defaultValString">(string) Optional, Default Value: <c>null:string</c>
    ///A default value</param>
    ///<param name="strings">(string seq) Optional, Default Value: <c>null:string seq</c>
    ///List of strings to be displayed as a click-able command options.
    ///  Note, strings cannot begin with a numeric character</param>
    ///<returns>(string) The string either input or selected by the user .
    ///  If the user presses the Enter key without typing in a string, an empty string "" is returned.</returns>
    static member GetString([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]defaultValString:string, [<OPT;DEF(null:string seq)>]strings:string seq) : string =
        let gs = Rhino.Input.Custom.GetString()
        gs.AcceptNothing(true)
        if notNull message then gs.SetCommandPrompt(message)
        if notNull defaultValString then gs.SetDefaultString(defaultValString)
        if notNull strings then
            for s in strings do gs.AddOption(s)
        let result = gs.Get()
        if result = Rhino.Input.GetResult.Cancel then null
        if( result = Rhino.Input.GetResult.Option ) then
            gs.Option().EnglishName
        gs.StringResult()
    (*
    def GetString(message=None, defaultString=None, strings=None):
        '''Pauses for user input of a string value
        Parameters:
          message (str, optional): a prompt or message
          defaultString (str, optional): a default value
          strings ([str, ...], optional): list of strings to be displayed as a click-able command options.
                                          Note, strings cannot begin with a numeric character
        Returns:
          str: The string either input or selected by the user if successful.
               If the user presses the Enter key without typing in a string, an empty string "" is returned.
          None: if not successful, on error, or if the user pressed cancel.
        '''
    
        gs = Rhino.Input.Custom.GetString()
        gs.AcceptNothing(True)
        if message: gs.SetCommandPrompt(message)
        if defaultString: gs.SetDefaultString(defaultString)
        if strings:
            for s in strings: gs.AddOption(s)
        result = gs.Get()
        if result==Rhino.Input.GetResult.Cancel: return None
        if( result == Rhino.Input.GetResult.Option ):
            return gs.Option().EnglishName
        return gs.StringResult()
    *)


    [<EXT>]
    ///<summary>Display a list of items in a list box dialog.</summary>
    ///<param name="items">(string seq) A list of values to select</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt of message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title</param>
    ///<param name="defaultVal">(string) Optional, Default Value: <c>null:string</c>
    ///Selected item in the list</param>
    ///<returns>(string) he selected item</returns>
    static member ListBox(items:string seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string, [<OPT;DEF(null:string)>]defaultVal:string) : string =
        Rhino.UI.Dialogs.ShowListBox(title, message, items, defaultVal)
    (*
    def ListBox(items, message=None, title=None, default=None):
        '''Display a list of items in a list box dialog.
        Parameters:
          items ([str, ...]): a list of values to select
          message (str, optional): a prompt of message
          title (str, optional): a dialog box title
          default (str, optional): selected item in the list
        Returns:
          str: he selected item if successful
          None: if not successful or on error
        '''
    
        return Rhino.UI.Dialogs.ShowListBox(title, message, items, default)
    *)


    [<EXT>]
    ///<summary>Displays a message box. A message box contains a message and
    ///  title, plus any combination of predefined icons and push buttons.</summary>
    ///<param name="message">(string) A prompt or message.</param>
    ///<param name="buttons">(int) Optional, Default Value: <c>0</c>
    ///Buttons and icon to display as a bit coded flag. Can be a combination of the
    ///  following flags. If omitted, an OK button and no icon is displayed
    ///  0      Display OK button only.
    ///  1      Display OK and Cancel buttons.
    ///  2      Display Abort, Retry, and Ignore buttons.
    ///  3      Display Yes, No, and Cancel buttons.
    ///  4      Display Yes and No buttons.
    ///  5      Display Retry and Cancel buttons.
    ///  16     Display Critical Message icon.
    ///  32     Display Warning Query icon.
    ///  48     Display Warning Message icon.
    ///  64     Display Information Message icon.
    ///  0      First button is the default.
    ///  256    Second button is the default.
    ///  512    Third button is the default.
    ///  768    Fourth button is the default.
    ///  0      Application modal. The user must respond to the message box
    ///    before continuing work in the current application.
    ///  4096   System modal. The user must respond to the message box
    ///    before continuing work in any application.</param>
    ///<param name="title">(string) Optional, Default Value: <c>""</c>
    ///The dialog box title</param>
    ///<returns>(float) indicating which button was clicked:
    ///  1      OK button was clicked.
    ///  2      Cancel button was clicked.
    ///  3      Abort button was clicked.
    ///  4      Retry button was clicked.
    ///  5      Ignore button was clicked.
    ///  6      Yes button was clicked.
    ///  7      No button was clicked.</returns>
    static member MessageBox(message:string, [<OPT;DEF(0)>]buttons:int, [<OPT;DEF("")>]title:string) : float =
        let buttontyp = buttons & 0x00000007 //111 in binary
        let btn = Rhino.UI.ShowMessageButton.OK
        if buttontyp = 1 then btn <- Rhino.UI.ShowMessageButton.OKCancel
        elif buttontyp = 2 then btn <- Rhino.UI.ShowMessageButton.AbortRetryIgnore
        elif buttontyp = 3 then btn <- Rhino.UI.ShowMessageButton.YesNoCancel
        elif buttontyp = 4 then btn <- Rhino.UI.ShowMessageButton.YesNo
        elif buttontyp = 5 then btn <- Rhino.UI.ShowMessageButton.RetryCancel
        let icontyp = buttons & 0x00000070
        let icon = Rhino.UI.ShowMessageIcon.null
        if icontyp = 16 then icon <- Rhino.UI.ShowMessageIcon.Error
        elif icontyp = 32 then icon <- Rhino.UI.ShowMessageIcon.Question
        elif icontyp = 48 then icon <- Rhino.UI.ShowMessageIcon.Warning
        elif icontyp = 64 then icon <- Rhino.UI.ShowMessageIcon.Information
        ////// 15 Sep 2014 Alain - default button not <| supported in new version of RC
        ////// that isn"t tied to Windows.Forms but it probably will so I"m commenting
        ////// the old code instead of deleting it.
        let //defbtntyp = buttons & 0x00000300
        let //defbtn = Windows.Forms.MessageDefaultButton.Button1
        //if defbtntyp = 256:
        let //    defbtn = Windows.Forms.MessageDefaultButton.Button2
        //elif defbtntyp = 512:
        //    defbtn <- Windows.Forms.MessageDefaultButton.Button3
        if not <| isinstance(message, str) then message <- string(message)
        let dlg_result = Rhino.UI.Dialogs.ShowMessage(message, title, btn, icon)
        if dlg_result = Rhino.UI.ShowMessageResult.OK then     1
        if dlg_result = Rhino.UI.ShowMessageResult.Cancel then 2
        if dlg_result = Rhino.UI.ShowMessageResult.Abort then  3
        if dlg_result = Rhino.UI.ShowMessageResult.Retry then  4
        if dlg_result = Rhino.UI.ShowMessageResult.Ignore then 5
        if dlg_result = Rhino.UI.ShowMessageResult.Yes then    6
        if dlg_result = Rhino.UI.ShowMessageResult.No then     7
    (*
    def MessageBox(message, buttons=0, title=""):
        '''Displays a message box. A message box contains a message and
        title, plus any combination of predefined icons and push buttons.
        Parameters:
          message (str): A prompt or message.
          buttons (number, optional): buttons and icon to display as a bit coded flag. Can be a combination of the
            following flags. If omitted, an OK button and no icon is displayed
            0      Display OK button only.
            1      Display OK and Cancel buttons.
            2      Display Abort, Retry, and Ignore buttons.
            3      Display Yes, No, and Cancel buttons.
            4      Display Yes and No buttons.
            5      Display Retry and Cancel buttons.
            16     Display Critical Message icon.
            32     Display Warning Query icon.
            48     Display Warning Message icon.
            64     Display Information Message icon.
            0      First button is the default.
            256    Second button is the default.
            512    Third button is the default.
            768    Fourth button is the default.
            0      Application modal. The user must respond to the message box
                   before continuing work in the current application.
            4096   System modal. The user must respond to the message box
                   before continuing work in any application.
          title(str, optional): the dialog box title
        Returns:
          number: indicating which button was clicked:
            1      OK button was clicked.
            2      Cancel button was clicked.
            3      Abort button was clicked.
            4      Retry button was clicked.
            5      Ignore button was clicked.
            6      Yes button was clicked.
            7      No button was clicked.
        '''
    
        buttontype = buttons & 0x00000007 #111 in binary
        btn = Rhino.UI.ShowMessageButton.OK
        if buttontype==1: btn = Rhino.UI.ShowMessageButton.OKCancel
        elif buttontype==2: btn = Rhino.UI.ShowMessageButton.AbortRetryIgnore
        elif buttontype==3: btn = Rhino.UI.ShowMessageButton.YesNoCancel
        elif buttontype==4: btn = Rhino.UI.ShowMessageButton.YesNo
        elif buttontype==5: btn = Rhino.UI.ShowMessageButton.RetryCancel
    
        icontype = buttons & 0x00000070
        icon = Rhino.UI.ShowMessageIcon.None
        if icontype==16: icon = Rhino.UI.ShowMessageIcon.Error
        elif icontype==32: icon = Rhino.UI.ShowMessageIcon.Question
        elif icontype==48: icon = Rhino.UI.ShowMessageIcon.Warning
        elif icontype==64: icon = Rhino.UI.ShowMessageIcon.Information
    
        ### 15 Sep 2014 Alain - default button not supported in new version of RC
        ### that isn't tied to Windows.Forms but it probably will so I'm commenting
        ### the old code instead of deleting it.
        #defbtntype = buttons & 0x00000300
        #defbtn = System.Windows.Forms.MessageDefaultButton.Button1
        #if defbtntype==256:
        #    defbtn = System.Windows.Forms.MessageDefaultButton.Button2
        #elif defbtntype==512:
        #    defbtn = System.Windows.Forms.MessageDefaultButton.Button3
    
        if not isinstance(message, str): message = str(message)
        dlg_result = Rhino.UI.Dialogs.ShowMessage(message, title, btn, icon)
        if dlg_result==Rhino.UI.ShowMessageResult.OK:     return 1
        if dlg_result==Rhino.UI.ShowMessageResult.Cancel: return 2
        if dlg_result==Rhino.UI.ShowMessageResult.Abort:  return 3
        if dlg_result==Rhino.UI.ShowMessageResult.Retry:  return 4
        if dlg_result==Rhino.UI.ShowMessageResult.Ignore: return 5
        if dlg_result==Rhino.UI.ShowMessageResult.Yes:    return 6
        if dlg_result==Rhino.UI.ShowMessageResult.No:     return 7
    *)


    [<EXT>]
    ///<summary>Displays list of items and their values in a property-style list box dialog</summary>
    ///<param name="items">(string seq) Items of 'list of string items and their corresponding values' (FIXME 0)</param>
    ///<param name="values">(string seq) Values of 'list of string items and their corresponding values' (FIXME 0)</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title</param>
    ///<returns>(string seq) of new values on success</returns>
    static member PropertyListBox(items:string seq, values:string seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : string seq =
        let values = [| for v in values do yield v.ToString() |]
        Rhino.UI.Dialogs.ShowPropertyListBox(title, message, items, values)
    (*
    def PropertyListBox(items, values, message=None, title=None):
        '''Displays list of items and their values in a property-style list box dialog
        Parameters:
          items, values ([str, ...]): list of string items and their corresponding values
          message (str, optional): a prompt or message
          title (str, optional): a dialog box title
        Returns:
          list(str, ..): of new values on success
          None: on error
        '''
    
        values = [v.ToString() for v in values]
        return Rhino.UI.Dialogs.ShowPropertyListBox(title, message, items, values)
    *)


    [<EXT>]
    ///<summary>Displays a list of items in a multiple-selection list box dialog</summary>
    ///<param name="items">(string seq) A zero-based list of string items</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title</param>
    ///<param name="defaultVals">(string seq) Optional, Default Value: <c>null:string seq</c>
    ///Either a string representing the pre-selected item in the list
    ///  or a list if multiple items are pre-selected</param>
    ///<returns>(string seq) containing the selected items</returns>
    static member MultiListBox(items:string seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string, [<OPT;DEF(null:string seq)>]defaultVals:string seq) : string seq =
        if isinstance(defaultVals, str) then
          let defaultVals = .[defaultVals]
        Rhino.UI.Dialogs.ShowMultiListBox(title, message, items, defaultVals)
    (*
    def MultiListBox(items, message=None, title=None, defaults=None):
        '''Displays a list of items in a multiple-selection list box dialog
        Parameters:
          items ([str, ...]) a zero-based list of string items
          message (str, optional): a prompt or message
          title (str, optional): a dialog box title
          defaults (str|[str,...], optional): either a string representing the pre-selected item in the list
                                              or a list if multiple items are pre-selected
        Returns:
          list(str, ...): containing the selected items if successful
          None: on error
        '''
    
        if isinstance(defaults, str):
          defaults = [defaults]
        return Rhino.UI.Dialogs.ShowMultiListBox(title, message, items, defaults)
    *)


    [<EXT>]
    ///<summary>Displays file open dialog box allowing the user to enter a file name.
    ///  Note, this function does not open the file.</summary>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title.</param>
    ///<param name="filter">(string) Optional, Default Value: <c>null:string</c>
    ///A filter string. The filter must be in the following form:
    ///  "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///  If omitted, the filter (*.*) is used.</param>
    ///<param name="folder">(string) Optional, Default Value: <c>null:string</c>
    ///A default folder.</param>
    ///<param name="filename">(string) Optional, Default Value: <c>null:string</c>
    ///A default file name</param>
    ///<param name="extension">(string) Optional, Default Value: <c>null:string</c>
    ///A default file extension</param>
    ///<returns>(string) file name is successful</returns>
    static member OpenFileName([<OPT;DEF(null:string)>]title:string, [<OPT;DEF(null:string)>]filter:string, [<OPT;DEF(null:string)>]folder:string, [<OPT;DEF(null:string)>]filename:string, [<OPT;DEF(null:string)>]extension:string) : string =
        let fd = Rhino.UI.OpenFileDialog()
        if notNull title then fd.Title <- title
        if notNull filter then fd.Filter <- filter
        if notNull folder then fd.InitialDirectory <- folder
        if notNull filename then fd.FileName <- filename
        if notNull extension then fd.DefaultExt <- extension
        if fd.ShowOpenDialog() then fd.FileName
    (*
    def OpenFileName(title=None, filter=None, folder=None, filename=None, extension=None):
        '''Displays file open dialog box allowing the user to enter a file name.
        Note, this function does not open the file.
        Parameters:
          title (str, optional): A dialog box title.
          filter (str, optional): A filter string. The filter must be in the following form:
            "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
            If omitted, the filter (*.*) is used.
          folder (str, optional): A default folder.
          filename (str, optional): a default file name
          extension (str, optional): a default file extension
        Returns:
          str: file name is successful
          None: if not successful, or on error
        '''
    
        fd = Rhino.UI.OpenFileDialog()
        if title: fd.Title = title
        if filter: fd.Filter = filter
        if folder: fd.InitialDirectory = folder
        if filename: fd.FileName = filename
        if extension: fd.DefaultExt = extension
        if fd.ShowOpenDialog(): return fd.FileName
    *)


    [<EXT>]
    ///<summary>Displays file open dialog box allowing the user to select one or more file names.
    ///  Note, this function does not open the file.</summary>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title.</param>
    ///<param name="filter">(string) Optional, Default Value: <c>null:string</c>
    ///A filter string. The filter must be in the following form:
    ///  "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///  If omitted, the filter (*.*) is used.</param>
    ///<param name="folder">(string) Optional, Default Value: <c>null:string</c>
    ///A default folder.</param>
    ///<param name="filename">(string) Optional, Default Value: <c>null:string</c>
    ///A default file name</param>
    ///<param name="extension">(string) Optional, Default Value: <c>null:string</c>
    ///A default file extension</param>
    ///<returns>(string seq) of selected file names</returns>
    static member OpenFileNames([<OPT;DEF(null:string)>]title:string, [<OPT;DEF(null:string)>]filter:string, [<OPT;DEF(null:string)>]folder:string, [<OPT;DEF(null:string)>]filename:string, [<OPT;DEF(null:string)>]extension:string) : string seq =
        let fd = Rhino.UI.OpenFileDialog()
        if notNull title then fd.Title <- title
        if notNull filter then fd.Filter <- filter
        if notNull folder then fd.InitialDirectory <- folder
        if notNull filename then fd.FileName <- filename
        if notNull extension then fd.DefaultExt <- extension
        let fd.MultiSelect = true
        if fd.ShowOpenDialog() then fd.FileNames
        []
    (*
    def OpenFileNames(title=None, filter=None, folder=None, filename=None, extension=None):
        '''Displays file open dialog box allowing the user to select one or more file names.
        Note, this function does not open the file.
        Parameters:
          title (str, optional): A dialog box title.
          filter (str, optional): A filter string. The filter must be in the following form:
            "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
            If omitted, the filter (*.*) is used.
          folder (str, optional): A default folder.
          filename (str, optional): a default file name
          extension (str, optional): a default file extension
        Returns:
          list(str, ...): of selected file names
        '''
    
        fd = Rhino.UI.OpenFileDialog()
        if title: fd.Title = title
        if filter: fd.Filter = filter
        if folder: fd.InitialDirectory = folder
        if filename: fd.FileName = filename
        if extension: fd.DefaultExt = extension
        fd.MultiSelect = True
        if fd.ShowOpenDialog(): return fd.FileNames
        return []
    *)


    [<EXT>]
    ///<summary>Display a context-style popup menu. The popup menu can appear almost
    ///  anywhere, and can be dismissed by clicking the left or right mouse buttons</summary>
    ///<param name="items">(string seq) List of strings representing the menu items. An empty string or None
    ///  will create a separator</param>
    ///<param name="modes">(int seq) Optional, Default Value: <c>null:int seq</c>
    ///List of numbers identifying the display modes. If omitted, all
    ///  modes are enabled.
    ///    0 = menu item is enabled
    ///    1 = menu item is disabled
    ///    2 = menu item is checked
    ///    3 = menu item is disabled and checked</param>
    ///<param name="point">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///A 3D point where the menu item will appear. If omitted, the menu
    ///  will appear at the current cursor position</param>
    ///<param name="view">(string) Optional, Default Value: <c>null:string</c>
    ///If point is specified, the view in which the point is computed.
    ///  If omitted, the active view is used</param>
    ///<returns>(float) index of the menu item picked or -1 if no menu item was picked</returns>
    static member PopupMenu(items:string seq, [<OPT;DEF(null:int seq)>]modes:int seq, [<OPT;DEF(Point3d())>]point:Point3d, [<OPT;DEF(null:string)>]view:string) : float =
        let screen_point = Windows.Forms.Cursor.Position
        if notNull point then
            let point = RhinoScriptSyntax.Coerce3dPoint(point)
            let view = __viewhelper(view)
            let viewport = view.ActiveViewport
            let point2d = viewport.WorldToClient(point)
            screen_point <- viewport.ClientToScreen(point2d)
        Rhino.UI.Dialogs.ShowContextMenu(items, screen_point, modes);
    (*
    def PopupMenu(items, modes=None, point=None, view=None):
        '''Display a context-style popup menu. The popup menu can appear almost
        anywhere, and can be dismissed by clicking the left or right mouse buttons
        Parameters:
          items ([str, ...]): list of strings representing the menu items. An empty string or None
            will create a separator
          modes ([number, ...]): List of numbers identifying the display modes. If omitted, all
            modes are enabled.
              0 = menu item is enabled
              1 = menu item is disabled
              2 = menu item is checked
              3 = menu item is disabled and checked
          point (point, optional): a 3D point where the menu item will appear. If omitted, the menu
            will appear at the current cursor position
          view (str, optional): if point is specified, the view in which the point is computed.
            If omitted, the active view is used
        Returns:
          number: index of the menu item picked or -1 if no menu item was picked
        '''
    
        screen_point = System.Windows.Forms.Cursor.Position
        if point:
            point = rhutil.coerce3dpoint(point)
            view = __viewhelper(view)
            viewport = view.ActiveViewport
            point2d = viewport.WorldToClient(point)
            screen_point = viewport.ClientToScreen(point2d)
        return Rhino.UI.Dialogs.ShowContextMenu(items, screen_point, modes);
    *)


    [<EXT>]
    ///<summary>Display a dialog box prompting the user to enter a number</summary>
    ///<param name="message">(string) Optional, Default Value: <c>""</c>
    ///A prompt message.</param>
    ///<param name="defaultValNumber">(float) Optional, Default Value: <c>7e89</c>
    ///A default number.</param>
    ///<param name="title">(string) Optional, Default Value: <c>""</c>
    ///A dialog box title.</param>
    ///<param name="minimum">(float) Optional, Default Value: <c>7e89</c>
    ///A minimum allowable value.</param>
    ///<param name="maximum">(float) Optional, Default Value: <c>7e89</c>
    ///A maximum allowable value.</param>
    ///<returns>(float) The newly entered number on success</returns>
    static member RealBox([<OPT;DEF("")>]message:string, [<OPT;DEF(7e89)>]defaultValNumber:float, [<OPT;DEF("")>]title:string, [<OPT;DEF(7e89)>]minimum:float, [<OPT;DEF(7e89)>]maximum:float) : float =
        if defaultValNumber = null then defaultValNumber <- Rhino.RhinoMath.UnsetValue
        if minimum = null then minimum <- Rhino.RhinoMath.UnsetValue
        if maximum = null then maximum <- Rhino.RhinoMath.UnsetValue
        let rc, number = Rhino.UI.Dialogs.ShowNumberBox(title, message, defaultValNumber, minimum, maximum)
        if notNull rc then number
    (*
    def RealBox(message="", default_number=None, title="", minimum=None, maximum=None):
        '''Display a dialog box prompting the user to enter a number
        Parameters:
          message (str, optional): a prompt message.
          default_number (number, optional):  a default number.
          title (str, optional):  a dialog box title.
          minimum (number, optional):  a minimum allowable value.
          maximum (number, optional):  a maximum allowable value.
        Returns:
          number: The newly entered number on success
          None: on error
        '''
    
        if default_number is None: default_number = Rhino.RhinoMath.UnsetValue
        if minimum is None: minimum = Rhino.RhinoMath.UnsetValue
        if maximum is None: maximum = Rhino.RhinoMath.UnsetValue
        rc, number = Rhino.UI.Dialogs.ShowNumberBox(title, message, default_number, minimum, maximum)
        if rc: return number
    *)


    [<EXT>]
    ///<summary>Display a save dialog box allowing the user to enter a file name.
    ///  Note, this function does not save the file.</summary>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title.</param>
    ///<param name="filter">(string) Optional, Default Value: <c>null:string</c>
    ///A filter string. The filter must be in the following form:
    ///  "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///  If omitted, the filter (*.*) is used.</param>
    ///<param name="folder">(string) Optional, Default Value: <c>null:string</c>
    ///A default folder.</param>
    ///<param name="filename">(string) Optional, Default Value: <c>null:string</c>
    ///A default file name</param>
    ///<param name="extension">(string) Optional, Default Value: <c>null:string</c>
    ///A default file extension</param>
    ///<returns>(string) the file name is successful</returns>
    static member SaveFileName([<OPT;DEF(null:string)>]title:string, [<OPT;DEF(null:string)>]filter:string, [<OPT;DEF(null:string)>]folder:string, [<OPT;DEF(null:string)>]filename:string, [<OPT;DEF(null:string)>]extension:string) : string =
        let fd = Rhino.UI.SaveFileDialog()
        if notNull title then fd.Title <- title
        if notNull filter then fd.Filter <- filter
        if notNull folder then fd.InitialDirectory <- folder
        if notNull filename then fd.FileName <- filename
        if notNull extension then fd.DefaultExt <- extension
        if fd.ShowSaveDialog() then fd.FileName
    (*
    def SaveFileName(title=None, filter=None, folder=None, filename=None, extension=None):
        '''Display a save dialog box allowing the user to enter a file name.
        Note, this function does not save the file.
        Parameters:
          title (str, optional): A dialog box title.
          filter(str, optional): A filter string. The filter must be in the following form:
            "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
            If omitted, the filter (*.*) is used.
          folder (str, optional): A default folder.
          filename (str, optional): a default file name
          extension (str, optional):  a default file extension
        Returns:
          str: the file name is successful
          None: if not successful, or on error
        '''
    
        fd = Rhino.UI.SaveFileDialog()
        if title: fd.Title = title
        if filter: fd.Filter = filter
        if folder: fd.InitialDirectory = folder
        if filename: fd.FileName = filename
        if extension: fd.DefaultExt = extension
        if fd.ShowSaveDialog(): return fd.FileName
    *)


    [<EXT>]
    ///<summary>Display a dialog box prompting the user to enter a string value.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt message</param>
    ///<param name="defaultValValue">(string) Optional, Default Value: <c>null:string</c>
    ///A default string value</param>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title</param>
    ///<returns>(string) the newly entered string value</returns>
    static member StringBox([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]defaultValValue:string, [<OPT;DEF(null:string)>]title:string) : string =
        let rc, text = Rhino.UI.Dialogs.ShowEditBox(title, message, defaultValValue, false)
        if notNull rc then text
    (*
    def StringBox(message=None, default_value=None, title=None):
        '''Display a dialog box prompting the user to enter a string value.
        Parameters:
          message (str, optional): a prompt message
          default_value (str, optional): a default string value
          title (str, optional): a dialog box title
        Returns:
          str: the newly entered string value if successful
          None: if not successful
        '''
    
        rc, text = Rhino.UI.Dialogs.ShowEditBox(title, message, default_value, False)
        if rc: return text
    *)


    [<EXT>]
    ///<summary>Display a text dialog box similar to the one used by the _What command.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///The message title</param>
    ///<returns>(unit) in any case</returns>
    static member TextOut([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : unit =
        Rhino.UI.Dialogs.ShowTextDialog(message, title)
    (*
    def TextOut(message=None, title=None):
        '''Display a text dialog box similar to the one used by the _What command.
        Parameters:
          message (str): a message
          title (str, optional): the message title
        Returns:
          None: in any case
        '''
    
        Rhino.UI.Dialogs.ShowTextDialog(message, title)
    *)


