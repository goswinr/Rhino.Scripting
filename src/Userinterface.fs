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
    ///<param name="point">(Point3d) Optional, Default Value: <c>Point3d.Origin</c>
    ///Starting, or base point</param>
    ///<param name="referencePoint">(Point3d) Optional, Default Value: <c>Point3d.Origin</c>
    ///If specified, the reference angle is calculated from it and the base point</param>
    ///<param name="defaultValAngleDegrees">(float) Optional, Default Value: <c>0.0</c>
    /// A default angle value specified</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    /// A prompt to display</param>
    ///<returns>(float option)Option of angle in degree</returns>
    static member GetAngle( [<OPT;DEF(Point3d())>]point:Point3d, 
                            [<OPT;DEF(Point3d())>]referencePoint:Point3d, 
                            [<OPT;DEF(0.0)>]defaultValAngleDegrees:float, 
                            [<OPT;DEF(null:string)>]message:string) : float option=
        async{
            do! Async.SwitchToContext syncContext 
            let point = if point = Point3d.Origin then Point3d.Unset else point
            let referencepoint = if referencePoint = Point3d.Origin then Point3d.Unset else referencePoint
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
    static member GetBoolean(message:string, items:(string*string*string) array, defaultVals:bool array) : (bool array) option =
        async{
            do! Async.SwitchToContext syncContext             
            let go = new Input.Custom.GetOption()
            go.AcceptNothing(true)
            go.SetCommandPrompt( message )
            let count = Seq.length(items)
            if count < 1 || count <> Seq.length(defaultVals) then failwithf "Rhino.Scripting: GetBoolean failed.  message:'%A' items:'%A' defaultVals:'%A'" message items defaultVals
            let toggles = ResizeArray()
            for i in range(count) do
                let initial = defaultVals.[i]
                let item = items.[i]
                let name,off,on = item
                let t = new Input.Custom.OptionToggle( initial, off, on )
                toggles.Add(t)
                go.AddOptionToggle(name, ref t) |> ignore
            let mutable getrc = go.Get()
            while getrc = Rhino.Input.GetResult.Option do
                getrc <- go.Get()

            let res =  
                if getrc <> Rhino.Input.GetResult.Nothing then 
                    None
                else
                    Some [| for t in toggles do yield t.CurrentValue |]
            return res
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
    ///<param name="prompt1">(string) Optional, Default Value: <c>null</c>
    ///Prompt1 of 'optional prompts to set' </param>
    ///<param name="prompt2">(string) Optional, Default Value: <c>null</c>
    ///Prompt2 of 'optional prompts to set' </param>
    ///<param name="prompt3">(string) Optional, Default Value: <c>null</c>
    ///Prompt3 of 'optional prompts to set' </param>
    ///<returns>(Point3d []) option) array of eight Point3d that define the corners of the box on success</returns>
    static member GetBox(   [<OPT;DEF(0)>]mode:int, 
                            [<OPT;DEF(Point3d())>]basisPoint:Point3d, 
                            [<OPT;DEF(null:string)>]prompt1:string, 
                            [<OPT;DEF(null:string)>]prompt2:string, 
                            [<OPT;DEF(null:string)>]prompt3:string) : (Point3d []) option=
        async{
            do! Async.SwitchToContext syncContext 
            let basisPoint = basisPoint |?? Point3d.Unset 
            let m =
                match mode with
                |0 -> Rhino.Input.GetBoxMode.All
                |1 -> Rhino.Input.GetBoxMode.Corner
                |2 -> Rhino.Input.GetBoxMode.ThreePoint
                |3 -> Rhino.Input.GetBoxMode.Vertical
                |4 -> Rhino.Input.GetBoxMode.Center
                |_ -> failwithf "GetBox:Bad mode %A" mode
            
            let box = ref (Box())
            let rc= Rhino.Input.RhinoGet.GetBox(box, m, basisPoint, prompt1, prompt2, prompt3)
            return 
                if rc = Rhino.Commands.Result.Success then Some ((!box).GetCorners())
                else None
            } |> Async.RunSynchronously

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
    ///<returns>(option<Drawing.Color>) an Option of RGB color</returns>
    static member GetColor([<OPT;DEF(Drawing.Color())>]color:Drawing.Color) : option<Drawing.Color> =
        async{
            do! Async.SwitchToContext syncContext
            let zero = Drawing.Color()
            let col = ref(if color = zero then  Drawing.Color.Black else color)
            let rc = Rhino.UI.Dialogs.ShowColorDialog(col)
            return
                if rc then Some (!col) else None
        } |> Async.RunSynchronously
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
    ///<returns>(Point3d * Point3d * Guid * Point3d) a Tuple of containing the following information
    ///  0  cursor position in world coordinates
    ///  1  cursor position in screen coordinates
    ///  2  id of the active viewport
    ///  3  cursor position in client coordinates</returns>
    static member GetCursorPos() : Point3d * Point2d * Guid * Point2d =
        async{
            do! Async.SwitchToContext syncContext
            let view = Doc.Views.ActiveView
            let screen_pt = Rhino.UI.MouseCursor.Location
            let client_pt = view.ScreenToClient(screen_pt)
            let viewport = view.ActiveViewport
            let xf = viewport.GetTransform(Rhino.DocObjects.CoordinateSystem.Screen, Rhino.DocObjects.CoordinateSystem.World)
            let world_pt = Point3d(client_pt.X, client_pt.Y, 0.0)
            world_pt.Transform(xf)
            return world_pt, screen_pt, viewport.Id, client_pt
            } |> Async.RunSynchronously
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
    ///<returns>(option<float>) an Option of The distance between the two points .</returns>
    static member GetDistance(  [<OPT;DEF(Point3d())>]firstPt:Point3d, 
                                [<OPT;DEF(0.0)>]distance:float, 
                                [<OPT;DEF("First distance point")>]firstPtMsg:string, 
                                [<OPT;DEF("Second distance point")>]secondPtMsg:string) : option<float> =
        async{
            do! Async.SwitchToContext syncContext
            let pt1 = 
                if firstPt = Point3d.Origin then 
                    let gp1 = new Rhino.Input.Custom.GetPoint()            
                    gp1.SetCommandPrompt(firstPtMsg)
                    match gp1.Get() with 
                    | Input.GetResult.Point -> Some (gp1.Point())
                    | _ -> None
                else
                    Some firstPt
            
            return
                match pt1 with 
                | Some pt ->
                    let gp2 = new Rhino.Input.Custom.GetPoint()
                    if distance <> 0.0 then
                        gp2.AcceptNothing(true)                        
                        gp2.SetCommandPrompt(sprintf "%s<%f>" secondPtMsg distance)
                    else
                        gp2.SetCommandPrompt(secondPtMsg)
                    gp2.DrawLineFromPoint(pt,true)
                    gp2.EnableDrawLineFromPoint(true)
                    match gp2.Get() with 
                    | Input.GetResult.Point ->
                        let d = gp2.Point().DistanceTo(pt)
                        RhinoApp.WriteLine ("Distance: " + d.ToNiceString + " " + RhinoScriptSyntax.UnitSystemName() )
                        Some d
                    | _ -> None
                | _ -> None
                } |> Async.RunSynchronously
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
    ///<param name="message">(string) Optional, Default Value: <c>Select Edges</c>
    ///A prompt or message.</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///Minimum number of edges to select.</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///Maximum number of edges to select.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the duplicated edge curves.</param>
    ///<returns>(option<ResizeArray<Guid*Point3d*Point3d>>) an Option of a List of selection prompts (curve id, parent id, selection point)</returns>
    static member GetEdgeCurves(    [<OPT;DEF("Select Edges":string)>]message:string, 
                                    [<OPT;DEF(1)>]minCount:int, 
                                    [<OPT;DEF(0)>]maxCount:int, 
                                    [<OPT;DEF(false)>]select:bool) : option<ResizeArray<Guid*Guid*Point3d>> =
        async{
            do! Async.SwitchToContext syncContext
            if maxCount > 0 && minCount > maxCount then failwithf "GetEdgeCurves: minCount %d is bigger than  maxCount %d" minCount  maxCount
            let go = new Rhino.Input.Custom.GetObject()
            go.SetCommandPrompt(message)
            go.GeometryFilter <- Rhino.DocObjects.ObjectType.Curve
            go.GeometryAttributeFilter <- Rhino.Input.Custom.GeometryAttributeFilter.EdgeCurve
            go.EnablePreSelect(false, true)
            let rc = go.GetMultiple(minCount, maxCount)
            return
                if rc <> Rhino.Input.GetResult.Object then None
                else
                    let r = ResizeArray()
                    for i in range(go.ObjectCount) do
                        let edge = go.Object(i).Edge()
                        if notNull edge then 
                            let crv = edge.Duplicate() :?> NurbsCurve
                            let curveid = Doc.Objects.AddCurve(crv)
                            let parentid = go.Object(i).ObjectId
                            let pt = go.Object(i).SelectionPoint()
                            r.Add( (curveid, parentid, pt) )
                    if  select then
                        for item in r do
                            let rhobj = Doc.Objects.Find(t1 item)
                            rhobj.Select(true)|> ignore
                        Doc.Views.Redraw()
                    Some r
            } |> Async.RunSynchronously
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

