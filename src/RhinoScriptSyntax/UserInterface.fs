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
open System.Collections.Generic
//open System.Windows.Forms

[<AutoOpen>]
module ExtensionsUserinterface =

  type RhinoScriptSyntax with

    [<EXT>]
    ///<summary>The Synchronization Context of the Rhino UI Therad.
    ///This MUST be set at the  beginning of every Script if using UI dialogs and not running on UI thread</summary>
    static member SynchronizationContext
        with get() = syncContext
        and set v  = syncContext <- v


    [<EXT>]
    ///<summary>Display browse-for-folder dialog allowing the user to select a folder</summary>
    ///<param name="folder">(string) Optional, A default folder</param>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<returns>(string option) selected folder option or None if selection was canceled</returns>
    static member BrowseForFolder([<OPT;DEF(null:string)>]folder:string, [<OPT;DEF(null:string)>]message:string) : string option =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
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


    [<EXT>]
    ///<summary>Displays a list of items in a checkable-style list dialog box</summary>
    ///<param name="items">((string*bool) seq) A list of tuples containing a string and a boolean check state</param>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>((string*bool) ResizeArray option) Option of tuples containing the input string in items along with their new boolean check value</returns>
    static member CheckListBox(items:(string*bool) seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : option<ResizeArray<string*bool>> =
        let checkstates = resizeArray { for  item in items -> snd item }
        let itemstrs =    resizeArray { for item in items -> fst item}

        let newcheckstates =
            async{
                if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
                return Rhino.UI.Dialogs.ShowCheckListBox(title, message, itemstrs, checkstates)
                } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread

        if notNull newcheckstates then
            Some (Seq.zip itemstrs newcheckstates |>  ResizeArray.ofSeq)
        else
            //failwithf "Rhino.Scripting: CheckListBox failed.  items:'%A' message:'%A' title:'%A'" items message title
            None


    [<EXT>]
    ///<summary>Displays a list of items in a combo-style list box dialog</summary>
    ///<param name="items">(string seq) A list of string</param>
    ///<param name="message">(string) Optional, A prompt of message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>(string Option) Option of The selected item</returns>
    static member ComboListBox(items:string seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : string option=
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            return
                match Rhino.UI.Dialogs.ShowComboListBox(title, message, items|> Array.ofSeq) with
                | null -> None
                | :? string as s -> Some s
                | _ -> None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Display dialog prompting the user to enter a string. The
    ///  string value may span multiple lines</summary>
    ///<param name="defaultValString">(string) Optional, A default string value</param>
    ///<param name="message">(string) Optional, A prompt message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>(string Option) Option of Multiple lines that are separated by carriage return-linefeed combinations</returns>
    static member EditBox([<OPT;DEF(null:string)>]defaultValString:string, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : string option =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let rc, text = Rhino.UI.Dialogs.ShowEditBox(title, message, defaultValString, true)
            return if rc then Some text else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pause for user input of an angle</summary>
    ///<param name="point">(Point3d) Optional, Default Value: <c>Point3d.Unset</c>
    ///Starting, or base point</param>
    ///<param name="referencePoint">(Point3d) Optional, Default Value: <c>Point3d.Unset</c>
    ///If specified, the reference angle is calculated from it and the base point</param>
    ///<param name="defaultValAngleDegrees">(float) Optional, Default Value: <c>0.0</c>
    /// A default angle value specified</param>
    ///<param name="message">(string) Optional,  A prompt to display</param>
    ///<returns>(float option) Option of angle in degree</returns>
    static member GetAngle( [<OPT;DEF(Point3d())>]point:Point3d,
                            [<OPT;DEF(Point3d())>]referencePoint:Point3d,
                            [<OPT;DEF(0.0)>]defaultValAngleDegrees:float,
                            [<OPT;DEF(null:string)>]message:string) : float option=
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let point = if point = Point3d.Origin then Point3d.Unset else point
            let referencepoint = if referencePoint = Point3d.Origin then Point3d.Unset else referencePoint
            let defaultangle = toRadians(defaultValAngleDegrees)
            let rc, angle = Rhino.Input.RhinoGet.GetAngle(message, point, referencepoint, defaultangle)
            return
                if rc = Rhino.Commands.Result.Success then Some(toDegrees(angle))
                else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pauses for user input of one or more boolean values. Boolean values are
    ///  displayed as click-able command line option toggles</summary>
    ///<param name="message">(string) A prompt</param>
    ///<param name="items">(string seq) List or tuple of options. Each option is a tuple of three strings
    ///  [n][1]    description of the boolean value. Must only consist of letters and numbers. (no characters like space, period, or dash)
    ///  [n][2]    string identifying the false value
    ///  [n][3]    string identifying the true value</param>
    ///<param name="defaultVals">(bool seq) List of boolean values used as default or starting values</param>
    ///<returns>(bool ResizeArray) a list of values that represent the boolean values</returns>
    static member GetBoolean(message:string, items:(string*string*string) array, defaultVals:bool array) :option<ResizeArray<bool>> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            use go = new Input.Custom.GetOption()
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
                    Some (ResizeArray.map (fun (t:Input.Custom.OptionToggle) ->  t.CurrentValue) toggles)
            return res
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


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
    ///<param name="prompt1">(string) Optional, Prompt1 of 'optional prompts to set'</param>
    ///<param name="prompt2">(string) Optional, Prompt2 of 'optional prompts to set'</param>
    ///<param name="prompt3">(string) Optional, Prompt3 of 'optional prompts to set'</param>
    ///<returns>(Point3d array) option) array of eight Point3d that define the corners of the box on success</returns>
    static member GetBox(   [<OPT;DEF(0)>]mode:int,
                            [<OPT;DEF(Point3d())>]basisPoint:Point3d,
                            [<OPT;DEF(null:string)>]prompt1:string,
                            [<OPT;DEF(null:string)>]prompt2:string,
                            [<OPT;DEF(null:string)>]prompt3:string) : (Point3d []) option=
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let basisPoint = if basisPoint <> Point3d.Origin then basisPoint else  Point3d.Unset
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
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Display the Rhino color picker dialog allowing the user to select an RGB color</summary>
    ///<param name="color">(Drawing.Color) Optional, Default Value: <c>Drawing.Color()</c>
    ///Default RGB value. If omitted, the default color is black</param>
    ///<returns>(option<Drawing.Color>) an Option of RGB color</returns>
    static member GetColor([<OPT;DEF(Drawing.Color())>]color:Drawing.Color) : option<Drawing.Color> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let zero = Drawing.Color()
            let col = ref(if color = zero then  Drawing.Color.Black else color)
            let rc = Rhino.UI.Dialogs.ShowColorDialog(col)
            return
                if rc then Some (!col) else None
        } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Retrieves the cursor's position</summary>
    ///<returns>(Point3d * Point3d * Guid * Point3d) a Tuple of containing the following information
    ///  0  cursor position in world coordinates
    ///  1  cursor position in screen coordinates
    ///  2  objectId of the active viewport
    ///  3  cursor position in client coordinates</returns>
    static member GetCursorPos() : Point3d * Point2d * Guid * Point2d =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let view = Doc.Views.ActiveView
            let screenpt = Rhino.UI.MouseCursor.Location
            let clientpt = view.ScreenToClient(screenpt)
            let viewport = view.ActiveViewport
            let xf = viewport.GetTransform(Rhino.DocObjects.CoordinateSystem.Screen, Rhino.DocObjects.CoordinateSystem.World)
            let worldpt = Point3d(clientpt.X, clientpt.Y, 0.0)
            worldpt.Transform(xf)
            return worldpt, screenpt, viewport.Id, clientpt
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pauses for user input of a distance</summary>
    ///<param name="firstPt">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///First distance point</param>
    ///<param name="distance">(float) Optional, Default Value: <c>7e89</c>
    ///Default distance</param>
    ///<param name="firstPtMsg">(string) Optional, Default Value: <c>"First distance point"</c>
    ///Prompt for the first distance point</param>
    ///<param name="secondPtMsg">(string) Optional, Default Value: <c>"Second distance point"</c>
    ///Prompt for the second distance point</param>
    ///<returns>(option<float>) an Option of The distance between the two points </returns>
    static member GetDistance(  [<OPT;DEF(Point3d())>]firstPt:Point3d,
                                [<OPT;DEF(0.0)>]distance:float,
                                [<OPT;DEF("First distance point")>]firstPtMsg:string,
                                [<OPT;DEF("Second distance point")>]secondPtMsg:string) : option<float> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let pt1 =
                if firstPt = Point3d.Origin then
                    let gp1 = new Rhino.Input.Custom.GetPoint()
                    gp1.SetCommandPrompt(firstPtMsg)
                    match gp1.Get() with
                    | Input.GetResult.Point ->
                        gp1.Dispose()
                        Some (gp1.Point())
                    | _ ->
                        gp1.Dispose()
                        None
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
                        RhinoApp.WriteLine ("Distance: " + d.ToNiceString + " " + Doc.GetUnitSystemName(true,true,false,false) )
                        gp2.Dispose()
                        Some d
                    | _ ->
                        gp2.Dispose()
                        None
                | _ -> None
                } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Prompt the user to pick one or more surface or polysurface edge curves</summary>
    ///<param name="message">(string) Optional, Default Value: <c>Select Edges</c>
    ///A prompt or message</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///Minimum number of edges to select</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///Maximum number of edges to select</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the duplicated edge curves</param>
    ///<returns>(option<ResizeArray<Guid*Point3d*Point3d>>) an Option of a List of selection prompts (curve objectId, parent objectId, selection point)</returns>
    static member GetEdgeCurves(    [<OPT;DEF("Select Edges":string)>]message:string,
                                    [<OPT;DEF(1)>]minCount:int,
                                    [<OPT;DEF(0)>]maxCount:int,
                                    [<OPT;DEF(false)>]select:bool) : option<ResizeArray<Guid*Guid*Point3d>> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            if maxCount > 0 && minCount > maxCount then failwithf "GetEdgeCurves: minCount %d is bigger than  maxCount %d" minCount  maxCount
            use go = new Rhino.Input.Custom.GetObject()
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
                            let rhobj = Doc.Objects.FindId(t1 item)
                            rhobj.Select(true)|> ignore
                        Doc.Views.Redraw()
                    Some r
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pauses for user input of a whole number</summary>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="number">(int) Optional, A default whole number value</param>
    ///<param name="minimum">(int) Optional, A minimum allowable value</param>
    ///<param name="maximum">(int) Optional, A maximum allowable value</param>
    ///<returns>(option<int>) an Option of The whole number input by the user </returns>
    static member GetInteger([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(2147482999)>]number:int, [<OPT;DEF(2147482999)>]minimum:int, [<OPT;DEF(2147482999)>]maximum:int) : option<int> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            use gi = new Rhino.Input.Custom.GetInteger()
            if notNull message then gi.SetCommandPrompt(message)
            if number  <> 2147482999 then gi.SetDefaultInteger(number)
            if minimum <> 2147482999 then gi.SetLowerLimit(minimum, false)
            if maximum <> 2147482999 then gi.SetUpperLimit(maximum, false)
            return
                if gi.Get() <> Rhino.Input.GetResult.Number then
                    gi.Dispose()
                    None
                else
                    let rc = gi.Number()
                    gi.Dispose()
                    Some rc
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Displays dialog box prompting the user to select a layer</summary>
    ///<param name="title">(string) Optional, Default Value: <c>"Select Layer"</c>
    ///Dialog box title</param>
    ///<param name="layer">(string) Optional, Name of a layer to preselect. If omitted, the current layer will be preselected</param>
    ///<param name="showNewButton">(bool) Optional, Default Value: <c>false</c>
    ///Show new button of on the dialog</param>
    ///<param name="showSetCurrent">(bool) Optional, Default Value: <c>false</c>
    ///Show set current  button on the dialog</param>
    ///<returns>(option<string>) an Option of name of selected layer</returns>
    static member GetLayer([<OPT;DEF("Select Layer")>]title:string, [<OPT;DEF(null:string)>]layer:string, [<OPT;DEF(false)>]showNewButton:bool, [<OPT;DEF(false)>]showSetCurrent:bool) : option<string> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let layerindex = ref Doc.Layers.CurrentLayerIndex
            if notNull layer then
                let layerinstance = Doc.Layers.FindName(layer)
                if notNull layerinstance then layerindex := layerinstance.Index
            let rc = Rhino.UI.Dialogs.ShowSelectLayerDialog(layerindex, title, showNewButton, showSetCurrent, ref true)
            return
                if not rc then None
                else
                    let layer = Doc.Layers.[!layerindex]
                    Some layer.FullPath
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread



    [<EXT>]
    ///<summary>Displays a dialog box prompting the user to select one or more layers</summary>
    ///<param name="title">(string) Optional, Default Value: <c>"Select Layers"</c>
    ///Dialog box title</param>
    ///<param name="showNewButton">(bool) Optional, Default Value: <c>false</c>
    ///Optional button to show on the dialog</param>
    ///<returns>(option<string array>) an Option of The names of selected layers</returns>
    static member GetLayers([<OPT;DEF("Select Layers")>]title:string, [<OPT;DEF(false)>]showNewButton:bool) : option<ResizeArray<string>> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let rc, layerindices = Rhino.UI.Dialogs.ShowSelectMultipleLayersDialog(null, title, showNewButton)
            return
                if rc then
                    Some (resizeArray { for index in layerindices do yield  Doc.Layers.[index].FullPath })
                else
                    None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread




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
    ///  9  Extension - Defines a line that extends from a curve</param>
    ///<param name="point">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///Optional starting point</param>
    ///<param name="message1">(string) Optional, Message1 of optional prompts</param>
    ///<param name="message2">(string) Optional, Message2 of optional prompts</param>
    ///<param name="message3">(string) Optional, Message3 of optional prompts</param>
    ///<returns>(option<Line>) an Option of A Line</returns>
    static member GetLine([<OPT;DEF(0)>]mode:int, [<OPT;DEF(Point3d())>]point:Point3d, [<OPT;DEF(null:string)>]message1:string, [<OPT;DEF(null:string)>]message2:string, [<OPT;DEF(null:string)>]message3:string) : option<Line> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            use gl = new Input.Custom.GetLine()
            if mode = 0 then gl.EnableAllVariations(true)
            else  gl.GetLineMode <- LanguagePrimitives.EnumOfValue( mode-1)
            if point <> Point3d.Origin then
                gl.SetFirstPoint(point)
            if notNull message1 then gl.FirstPointPrompt <- message1
            if notNull message2 then gl.MidPointPrompt <- message2
            if notNull message3 then gl.SecondPointPrompt <- message3
            let rc, line = gl.Get()
            return
                if rc = Rhino.Commands.Result.Success then
                    Some line
                else
                    None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Displays a dialog box prompting the user to select one linetype</summary>
    ///<param name="defaultValLinetype">(string) Optional, Optional. The name of the linetype to select. If omitted, the current linetype will be selected</param>
    ///<param name="showByLayer">(bool) Optional, Default Value: <c>false</c>
    ///If True, the "by Layer" linetype will show. Defaults to False</param>
    ///<returns>(option<string>) an Option of The names of selected linetype</returns>
    static member GetLinetype([<OPT;DEF(null:string)>]defaultValLinetype:string, [<OPT;DEF(false)>]showByLayer:bool) : option<string> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let mutable ltinstance = Doc.Linetypes.CurrentLinetype
            if notNull defaultValLinetype then
                let ltnew = Doc.Linetypes.FindName(defaultValLinetype)
                if notNull ltnew  then ltinstance <- ltnew
            return
                try
                    let objectId = Rhino.UI.Dialogs.ShowLineTypes("Select Linetype", "Select Linetype", Doc) :?> Guid  // this fails if clicking in void
                    let linetype = Doc.Linetypes.FindId(objectId)
                    Some linetype.Name
                with _ ->
                    None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Prompts the user to pick one or more mesh faces</summary>
    ///<param name="objectId">(Guid) The mesh object's identifier</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Select Mesh Faces"</c>
    ///A prompt of message</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///The minimum number of faces to select</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///The maximum number of faces to select.
    ///  If 0, the user must press enter to finish selection.
    ///  If -1, selection stops as soon as there are at least minCount faces selected</param>
    ///<returns>(option<int ResizeArray>) an Option of of mesh face indices on success</returns>
    static member GetMeshFaces(objectId:Guid, [<OPT;DEF("Select Mesh Faces")>]message:string, [<OPT;DEF(1)>]minCount:int, [<OPT;DEF(0)>]maxCount:int) : option<ResizeArray<int>> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            Doc.Objects.UnselectAll() |> ignore
            Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            go.SetCustomGeometryFilter(fun rhinoobject _ _ -> objectId = rhinoobject.Id)
            go.SetCommandPrompt(message)
            go.GeometryFilter <- Rhino.DocObjects.ObjectType.MeshFace
            go.AcceptNothing(true)

            return
                if go.GetMultiple(minCount,maxCount) <> Rhino.Input.GetResult.Object then
                    None
                else
                    let objrefs = go.Objects()
                    let rc = resizeArray { for  item in objrefs do yield item.GeometryComponentIndex.Index }
                    Some rc
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously


    [<EXT>]
    ///<summary>Prompts the user to pick one or more mesh vertices</summary>
    ///<param name="objectId">(Guid) The mesh object's identifier</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Select Mesh Vertices"</c>
    ///A prompt of message</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///The minimum number of vertices to select</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///The maximum number of vertices to select. If 0, the user must
    ///  press enter to finish selection. If -1, selection stops as soon as there
    ///  are at least minCount vertices selected</param>
    ///<returns>(option<int ResizeArray>) an Option of of mesh vertex indices on success</returns>
    static member GetMeshVertices(objectId:Guid, [<OPT;DEF("Select Mesh Vertices")>]message:string, [<OPT;DEF(1)>]minCount:int, [<OPT;DEF(0)>]maxCount:int) : option<ResizeArray<int>> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            Doc.Objects.UnselectAll() |> ignore
            Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            go.SetCustomGeometryFilter(fun rhinoobject _ _ -> objectId = rhinoobject.Id)
            go.SetCommandPrompt(message)
            go.GeometryFilter <-  Rhino.DocObjects.ObjectType.MeshVertex
            go.AcceptNothing(true)
            return
                if go.GetMultiple(minCount,maxCount) <> Rhino.Input.GetResult.Object then
                    None
                else
                    let objrefs = go.Objects()
                    let rc = resizeArray { for  item in objrefs do yield item.GeometryComponentIndex.Index }
                    Some rc
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pauses for user input of a point</summary>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="basisPoint">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///Point3d identifying a starting, or base point</param>
    ///<param name="distance">(float) Optional, Default Value: <c>0.0</c>
    ///Constraining distance. If distance is specified, basePoint must also be specified</param>
    ///<param name="inPlane">(bool) Optional, Default Value: <c>false</c>
    ///Constrains the point selections to the active construction plane</param>
    ///<returns>(option<Point3d>) an Option of point on success</returns>
    static member GetPoint( [<OPT;DEF(null:string)>]message:string,
                            [<OPT;DEF(Point3d())>]basisPoint:Point3d,
                            [<OPT;DEF(0.0)>]distance:float,
                            [<OPT;DEF(false)>]inPlane:bool) : option<Point3d> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            use gp = new Input.Custom.GetPoint()
            if notNull message then gp.SetCommandPrompt(message)
            if basisPoint <> Point3d.Origin then
                gp.DrawLineFromPoint(basisPoint,true)
                gp.EnableDrawLineFromPoint(true)
                if distance<>0.0 then gp.ConstrainDistanceFromBasePoint(distance)
            if inPlane then gp.ConstrainToConstructionPlane(true)|> ignore
            gp.Get() |> ignore
            return
                if gp.CommandResult() <> Rhino.Commands.Result.Success then
                    None
                else
                    let pt = gp.Point()
                    Some pt
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread



    [<EXT>]
    ///<summary>Pauses for user input of a point constrainted to a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Pick Point On Curve"</c>
    ///A prompt of message</param>
    ///<returns>(option<Point3d>) an Option of 3d point</returns>
    static member GetPointOnCurve(curveId:Guid, [<OPT;DEF("Pick Point On Curve":string)>]message:string) : option<Point3d> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let curve = RhinoScriptSyntax.CoerceCurve(curveId)
            use gp = new Input.Custom.GetPoint()
            gp.SetCommandPrompt(message)
            gp.Constrain(curve, false) |> ignore
            gp.Get() |> ignore
            return
                if gp.CommandResult() <> Rhino.Commands.Result.Success then
                    None
                else
                    let pt = gp.Point()
                    Some pt
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pauses for user input of a point constrained to a mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of the mesh to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Pick Point On Mesh"</c>
    ///A prompt or message</param>
    ///<returns>(option<Point3d>) an Option of 3d point</returns>
    static member GetPointOnMesh(meshId:Guid, [<OPT;DEF("Pick Point On Mesh":string)>]message:string) : option<Point3d> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let cmdrc, point = Rhino.Input.RhinoGet.GetPointOnMesh(meshId, message, false)
            return
                if cmdrc = Rhino.Commands.Result.Success then Some point
                else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pauses for user input of a point constrained to a surface or polysurface
    ///  object</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Pick Point on Surface or Polysurface"</c>
    ///A prompt or message</param>
    ///<returns>(option<Point3d>) an Option of 3d point</returns>
    static member GetPointOnSurface(surfaceId:Guid, [<OPT;DEF("Pick Point on Surface or Polysurface":string)>]message:string) : option<Point3d> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            use gp = new Input.Custom.GetPoint()
            gp.SetCommandPrompt(message)
            match RhinoScriptSyntax.CoerceGeometry surfaceId with
            | :? Surface as srf ->
                gp.Constrain(srf,false) |> ignore

            | :? Brep as brep ->
                gp.Constrain(brep, -1, -1, false) |> ignore

            | _ ->
                failwithf "Rhino.Scripting: GetPointOnSurface failed input is not surface or polysurface.  surfaceId:'%A' message:'%A'" surfaceId message

            gp.Get() |>ignore
            return
                if gp.CommandResult() <> Rhino.Commands.Result.Success then
                    None
                else
                    let pt = gp.Point()
                    Some pt
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pauses for user input of one or more points</summary>
    ///<param name="drawLines">(bool) Optional, Default Value: <c>false</c>
    ///Draw lines between points</param>
    ///<param name="inPlane">(bool) Optional, Default Value: <c>false</c>
    ///Constrain point selection to the active construction plane</param>
    ///<param name="message1">(string) Optional, A prompt or message for the first point</param>
    ///<param name="message2">(string) Optional, A prompt or message for the next points</param>
    ///<param name="maxPoints">(float) Optional, Default Value: <c>None</c>
    ///  Maximum number of points to pick. If not specified, an
    ///  unlimited number of points can be picked</param>
    ///<returns>(option<Point3d array>) an Option of of 3d points</returns>
    static member GetPoints(    [<OPT;DEF(false)>]drawLines:bool,
                                [<OPT;DEF(false)>]inPlane:bool,
                                [<OPT;DEF(null:string)>]message1:string,
                                [<OPT;DEF(null:string)>]message2:string,
                                [<OPT;DEF(2147482999)>]maxPoints:int) : option<Point3d ResizeArray> =
                                //[<OPT;DEF(Point3d())>]basisPoint:Point3d) // Ignoredhere because ignored in python too TODO <param name="basisPoint">(Point3d) Optional, Default Value: <c>Point3d()</c>   A starting or base point</param>

        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            use gp = new Input.Custom.GetPoint()
            if notNull message1 then gp.SetCommandPrompt(message1)
            gp.EnableDrawLineFromPoint( drawLines )
            if inPlane then
                gp.ConstrainToConstructionPlane(true) |> ignore
                let plane = Doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
                gp.Constrain(plane, false) |> ignore
            let mutable getres = gp.Get()
            return
                if gp.CommandResult() <> Rhino.Commands.Result.Success then None
                else
                    let mutable prevPoint = gp.Point()
                    let rc = ResizeArray([prevPoint])
                    if maxPoints = 2147482999 || maxPoints>1 then
                        let mutable currentpoint = 1
                        if notNull message2 then gp.SetCommandPrompt(message2)

                        if drawLines then
                            gp.DynamicDraw.Add (fun args  -> if rc.Count > 1 then
                                                                let c = Rhino.ApplicationSettings.AppearanceSettings.FeedbackColor
                                                                args.Display.DrawPolyline(rc, c)
                                                                |>  ignore)
                        let mutable cont = true
                        while cont do
                            if maxPoints <> 2147482999 && currentpoint>=maxPoints then cont <- false
                            if cont && drawLines then
                                gp.DrawLineFromPoint(prevPoint, true)
                            if cont then
                                gp.SetBasePoint(prevPoint, true)
                                currentpoint <- currentpoint + 1
                                getres <- gp.Get()
                                if getres = Rhino.Input.GetResult.Cancel then
                                    cont <- false
                                    //RhinoApp.WriteLine "GetPoints canceled"
                                if cont && gp.CommandResult() <> Rhino.Commands.Result.Success then
                                    rc.Clear()
                                    cont <- false
                                    RhinoApp.WriteLine "GetPoints no Success"
                                if cont then
                                    prevPoint <- gp.Point()
                                    rc.Add(prevPoint)
                    if rc.Count>0 then
                        RhinoScriptSyntax.Print (rc.Count, "Points picked")
                        Some rc
                    else
                        None

            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread




    [<EXT>]
    ///<summary>Prompts the user to pick points that define a polyline</summary>
    ///<param name="flags">(int) Optional, Default Value: <c>3</c>
    ///The options are bit coded flags. Values can be added together to specify more than one option. The default is 3.
    ///  value description
    ///  1     Permit close option. If specified, then after 3 points have been picked, the user can type "Close" and a closed polyline will be returned.
    ///  2     Permit close snap. If specified, then after 3 points have been picked, the user can pick near the start point and a closed polyline will be returned.
    ///  4     Force close. If specified, then the returned polyline is always closed. If specified, then intMax must be 0 or >= 4.
    ///  Note: the default is 3, or "Permit close option = True", "Permit close snap = True", and "Force close = False"</param>
    ///<param name="message1">(string) Optional, A prompt or message for the first point</param>
    ///<param name="message2">(string) Optional, A prompt or message for the second point</param>
    ///<param name="message3">(string) Optional, A prompt or message for the third point</param>
    ///<param name="message4">(string) Optional, A prompt or message for the 'next' point</param>
    ///<param name="min">(int) Optional, Default Value: <c>2</c>
    ///The minimum number of points to require. The default is 2</param>
    ///<param name="max">(int) Optional, Default Value: <c>0</c>
    ///The maximum number of points to require; 0 for no limit.  The default is 0</param>
    ///<returns>(option<Polyline>) an Option of A  polyline </returns>
    static member GetPolyline(          [<OPT;DEF(3)>]flags:int,
                                        [<OPT;DEF(null:string)>]message1:string,
                                        [<OPT;DEF(null:string)>]message2:string,
                                        [<OPT;DEF(null:string)>]message3:string,
                                        [<OPT;DEF(null:string)>]message4:string,
                                        [<OPT;DEF(2147482999)>]min:int,
                                        [<OPT;DEF(0)>]max:int) : option<Polyline> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let gpl = new Input.Custom.GetPolyline()
            if notNull message1 then gpl.FirstPointPrompt <- message1
            if notNull message2 then gpl.SecondPointPrompt <- message2
            if notNull message3 then gpl.ThirdPointPrompt <- message3
            if notNull message4 then gpl.FourthPointPrompt <- message4
            gpl.MinPointCount <- min
            if max>min then gpl.MaxPointCount <- max
            let rc, polyline = gpl.Get()
            Doc.Views.Redraw()
            return
              if rc = Rhino.Commands.Result.Success then Some polyline
              else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pauses for user input of a number</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Number"</c>
    ///A prompt or message</param>
    ///<param name="number">(float) Optional, A default number value</param>
    ///<param name="minimum">(float) Optional, A minimum allowable value</param>
    ///<param name="maximum">(float) Optional, A maximum allowable value</param>
    ///<returns>(option<float>) an Option of The number input by the user </returns>
    static member GetReal(              [<OPT;DEF("Number")>]message:string,
                                        [<OPT;DEF(7e89)>]number:float,
                                        [<OPT;DEF(7e89)>]minimum:float,
                                        [<OPT;DEF(7e89)>]maximum:float) : option<float> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let gn = new Input.Custom.GetNumber()
            if notNull message then gn.SetCommandPrompt(message)
            if number <> 7e89 then gn.SetDefaultNumber(number)
            if minimum <> 7e89 then gn.SetLowerLimit(minimum, false)
            if maximum <> 7e89 then gn.SetUpperLimit(maximum, false)
            return
                if gn.Get() <> Rhino.Input.GetResult.Number then None
                else
                    let rc = gn.Number()
                    gn.Dispose()
                    Some rc
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


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
    ///<param name="prompt1">(string) Optional, Prompt1 of optional prompts</param>
    ///<param name="prompt2">(string) Optional, Prompt2 of optional prompts</param>
    ///<param name="prompt3">(string) Optional, Prompt3 of optional prompts</param>
    ///<returns>(option<Point3d * Point3d * Point3d * Point3d>) an Option of four 3d points that define the corners of the rectangle</returns>
    static member GetRectangle(
                                        [<OPT;DEF(0)>]mode:int,
                                        [<OPT;DEF(Point3d())>]basisPoint:Point3d,
                                        [<OPT;DEF(null:string)>]prompt1:string,
                                        [<OPT;DEF(null:string)>]prompt2:string,
                                        [<OPT;DEF(null:string)>]prompt3:string) : option<Point3d * Point3d * Point3d * Point3d> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let mode : Input.GetBoxMode = LanguagePrimitives.EnumOfValue mode

            let basisPoint = if basisPoint = Point3d.Origin then Point3d.Unset else basisPoint
            let prompts = ResizeArray([""; ""; ""])
            if notNull prompt1 then prompts.[0] <- prompt1
            if notNull prompt2 then prompts.[1] <- prompt2
            if notNull prompt3 then prompts.[2] <- prompt3
            let rc, corners = Rhino.Input.RhinoGet.GetRectangle(mode, basisPoint, prompts)
            return
                if rc = Rhino.Commands.Result.Success then Some (corners.[0],corners.[1],corners.[2],corners.[3])
                else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Pauses for user input of a string value</summary>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="defaultValString">(string) Optional, A default value</param>
    ///<param name="strings">(string seq) Optional, List of strings to be displayed as a click-able command options.
    ///  Note, strings cannot begin with a numeric character</param>
    ///<returns>(option<string>) an Option of The string either input or selected by the user .
    ///  If the user presses the Enter key without typing in a string, an empty string "" is returned</returns>
    static member GetString(
                                        [<OPT;DEF(null:string)>]message:string,
                                        [<OPT;DEF(null:string)>]defaultValString:string,
                                        [<OPT;DEF(null:string seq)>]strings:string seq) : option<string> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let gs = new Input.Custom.GetString()
            gs.AcceptNothing(true)
            if notNull message then gs.SetCommandPrompt(message)
            if notNull defaultValString then gs.SetDefaultString(defaultValString)
            if notNull strings then
                for s in strings do gs.AddOption(s) |> ignore
            let result = gs.Get()
            return
                if result = Rhino.Input.GetResult.Cancel then
                    None
                elif( result = Rhino.Input.GetResult.Option ) then
                    Some <| gs.Option().EnglishName
                else
                    Some <| gs.StringResult()
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Display a list of items in a list box dialog</summary>
    ///<param name="items">(string IList) A list of values to select</param>
    ///<param name="message">(string) Optional, A prompt of message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="defaultVal">(string) Optional, Selected item in the list</param>
    ///<returns>(option<string>) an Option of he selected item</returns>
    static member ListBox(              items:string IList,
                                        [<OPT;DEF(null:string)>]message:string,
                                        [<OPT;DEF(null:string)>]title:string,
                                        [<OPT;DEF(null:string)>]defaultVal:string) : option<string> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            return
                match Rhino.UI.Dialogs.ShowListBox(title, message, Array.ofSeq items , defaultVal) with
                | null ->  None
                |  :? string as s -> Some s
                | _ -> None

            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Displays a message box. A message box contains a message and
    ///  title, plus any combination of predefined icons and push buttons</summary>
    ///<param name="message">(string) A prompt or message</param>
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
    ///    before continuing work in any application</param>
    ///<param name="title">(string) Optional, Default Value: <c>""</c>
    ///The dialog box title</param>
    ///<returns>(option<int>) an Option of indicating which button was clicked:
    ///  1      OK button was clicked.
    ///  2      Cancel button was clicked.
    ///  3      Abort button was clicked.
    ///  4      Retry button was clicked.
    ///  5      Ignore button was clicked.
    ///  6      Yes button was clicked.
    ///  7      No button was clicked</returns>
    static member MessageBox(           message:string,
                                        [<OPT;DEF(0)>]buttons:int,
                                        [<OPT;DEF("")>]title:string) : option<int> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let mutable buttontyp =  buttons &&& 0x00000007 //111 in binary
            let mutable btn = Rhino.UI.ShowMessageButton.OK
            if   buttontyp = 1 then btn <- Rhino.UI.ShowMessageButton.OKCancel
            elif buttontyp = 2 then btn <- Rhino.UI.ShowMessageButton.AbortRetryIgnore
            elif buttontyp = 3 then btn <- Rhino.UI.ShowMessageButton.YesNoCancel
            elif buttontyp = 4 then btn <- Rhino.UI.ShowMessageButton.YesNo
            elif buttontyp = 5 then btn <- Rhino.UI.ShowMessageButton.RetryCancel
            let icontyp = buttons &&& 0x00000070
            let mutable icon = Rhino.UI.ShowMessageIcon.None
            if   icontyp = 16 then icon <- Rhino.UI.ShowMessageIcon.Error
            elif icontyp = 32 then icon <- Rhino.UI.ShowMessageIcon.Question
            elif icontyp = 48 then icon <- Rhino.UI.ShowMessageIcon.Warning
            elif icontyp = 64 then icon <- Rhino.UI.ShowMessageIcon.Information
            ////// 15 Sep 2014 Alain - default button not supported in new version of RC
            ////// that isn"t tied to Windows.Forms but it probably will so I"m commenting
            ////// the old code instead of deleting it.
            //defbtntyp = buttons & 0x00000300
            //defbtn = Windows.Forms.MessageDefaultButton.Button1
            //if defbtntyp = 256:
            //    defbtn = Windows.Forms.MessageDefaultButton.Button2
            //elif defbtntyp = 512:
            //    defbtn <- Windows.Forms.MessageDefaultButton.Button3
            let dlgresult = Rhino.UI.Dialogs.ShowMessage(message, title, btn, icon)
            return
                if   dlgresult = Rhino.UI.ShowMessageResult.OK then     Some 1
                elif dlgresult = Rhino.UI.ShowMessageResult.Cancel then Some 2
                elif dlgresult = Rhino.UI.ShowMessageResult.Abort then  Some 3
                elif dlgresult = Rhino.UI.ShowMessageResult.Retry then  Some 4
                elif dlgresult = Rhino.UI.ShowMessageResult.Ignore then Some 5
                elif dlgresult = Rhino.UI.ShowMessageResult.Yes then    Some 6
                elif dlgresult = Rhino.UI.ShowMessageResult.No then     Some 7
                else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Displays list of items and their values in a property-style list box dialog</summary>
    ///<param name="items">(string IList) list of string items</param>
    ///<param name="values">(string seq) the corresponding values to the items</param>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>(option<string array>) an Option of of new values on success</returns>
    static member PropertyListBox(  items:string IList,
                                    values:string seq,
                                    [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(null:string)>]title:string) : option<string array> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let values = resizeArray { for  v in values do yield v.ToString() }
            return
                match Rhino.UI.Dialogs.ShowPropertyListBox(title, message, Array.ofSeq items , values) with
                | null ->  None
                | s -> Some s
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Displays a list of items in a multiple-selection list box dialog</summary>
    ///<param name="items">(string IList) A zero-based list of string items</param>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="defaultVals">(string seq) Optional, Either a string representing the pre-selected item in the list
    ///  or a list if multiple items are pre-selected</param>
    ///<returns>(option<string array>) an Option of containing the selected items</returns>
    static member MultiListBox(     items:string IList,
                                    [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(null:string)>]title:string,
                                    [<OPT;DEF(null:string IList)>]defaultVals:string IList) : option<string array> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let r =  Rhino.UI.Dialogs.ShowMultiListBox(title, message, items, defaultVals)
            return
                if notNull r then Some r else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Displays file open dialog box allowing the user to enter a file name.
    ///  Note, this function does not open the file</summary>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="filter">(string) Optional, A filter string. The filter must be in the following form:
    ///  "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///  If omitted, the filter (*.*) is used</param>
    ///<param name="folder">(string) Optional, A default folder</param>
    ///<param name="filename">(string) Optional, A default file name</param>
    ///<param name="extension">(string) Optional, A default file extension</param>
    ///<returns>(option<string>) an Option of file name is successful</returns>
    static member OpenFileName(     [<OPT;DEF(null:string)>]title:string,
                                    [<OPT;DEF(null:string)>]filter:string,
                                    [<OPT;DEF(null:string)>]folder:string,
                                    [<OPT;DEF(null:string)>]filename:string,
                                    [<OPT;DEF(null:string)>]extension:string) : option<string> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let fd = Rhino.UI.OpenFileDialog()
            if notNull title then fd.Title <- title
            if notNull filter then fd.Filter <- filter
            if notNull folder then fd.InitialDirectory <- folder
            if notNull filename then fd.FileName <- filename
            if notNull extension then fd.DefaultExt <- extension
            return
                if fd.ShowOpenDialog() then Some fd.FileName
                else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Displays file open dialog box allowing the user to select one or more file names.
    ///  Note, this function does not open the file</summary>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="filter">(string) Optional, A filter string. The filter must be in the following form:
    ///  "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///  If omitted, the filter (*.*) is used</param>
    ///<param name="folder">(string) Optional, A default folder</param>
    ///<param name="filename">(string) Optional, A default file name</param>
    ///<param name="extension">(string) Optional, A default file extension</param>
    ///<returns>(option<string array>) an Option of of selected file names</returns>
    static member OpenFileNames(    [<OPT;DEF(null:string)>]title:string,
                                    [<OPT;DEF(null:string)>]filter:string,
                                    [<OPT;DEF(null:string)>]folder:string,
                                    [<OPT;DEF(null:string)>]filename:string,
                                    [<OPT;DEF(null:string)>]extension:string) : option<string array> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let fd = Rhino.UI.OpenFileDialog()
            if notNull title then fd.Title <- title
            if notNull filter then fd.Filter <- filter
            if notNull folder then fd.InitialDirectory <- folder
            if notNull filename then fd.FileName <- filename
            if notNull extension then fd.DefaultExt <- extension
            fd.MultiSelect <- true
            return
                if fd.ShowOpenDialog() then Some fd.FileNames
                else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Display a context-style popup menu. The popup menu can appear almost
    ///  anywhere, and can be dismissed by clicking the left or right mouse buttons</summary>
    ///<param name="items">(string seq) List of strings representing the menu items. An empty string or None
    ///  will create a separator</param>
    ///<param name="modes">(int seq) Optional, List of numbers identifying the display modes. If omitted, all
    ///  modes are enabled.
    ///    0 = menu item is enabled
    ///    1 = menu item is disabled
    ///    2 = menu item is checked
    ///    3 = menu item is disabled and checked</param>
    ///<param name="point">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///A 3D point where the menu item will appear. If omitted, the menu
    ///  will appear at the current cursor position</param>
    ///<param name="view">(string) Optional, If point is specified, the view in which the point is computed.
    ///  If omitted, the active view is used</param>
    ///<returns>(int) index of the menu item picked or -1 if no menu item was picked</returns>
    static member PopupMenu(        items:string seq,
                                    [<OPT;DEF(null:int seq)>]modes:int seq,
                                    [<OPT;DEF(Point3d())>]point:Point3d,
                                    [<OPT;DEF(null:string)>]view:string) : int =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let mutable screenpoint = Windows.Forms.Cursor.Position
            if Point3d.Origin <> point then
                let view = RhinoScriptSyntax.CoerceView(view)
                let viewport = view.ActiveViewport
                let point2d = viewport.WorldToClient(point)
                screenpoint <- viewport.ClientToScreen(point2d)
            return  Rhino.UI.Dialogs.ShowContextMenu(items, screenpoint, modes)
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Display a dialog box prompting the user to enter a number</summary>
    ///<param name="message">(string) Optional, Default Value: <c>""</c>
    ///A prompt message</param>
    ///<param name="defaultValNumber">(float) Optional, Default Value: <c>7e89</c>
    ///A default number</param>
    ///<param name="title">(string) Optional, Default Value: <c>""</c>
    ///A dialog box title</param>
    ///<param name="minimum">(float) Optional, Default Value: <c>7e89</c>
    ///A minimum allowable value</param>
    ///<param name="maximum">(float) Optional, Default Value: <c>7e89</c>
    ///A maximum allowable value</param>
    ///<returns>(option<float>) an Option of The newly entered number on success</returns>
    static member RealBox(          [<OPT;DEF("")>]message:string,
                                    [<OPT;DEF(7e89)>]defaultValNumber:float,
                                    [<OPT;DEF("")>]title:string,
                                    [<OPT;DEF(7e89)>]minimum:float,
                                    [<OPT;DEF(7e89)>]maximum:float) : option<float> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let defaultValNumber = ref <| if defaultValNumber = 7e89 then Rhino.RhinoMath.UnsetValue else defaultValNumber
            let minimum = if minimum = 7e89 then Rhino.RhinoMath.UnsetValue else minimum
            let maximum = if maximum = 7e89 then Rhino.RhinoMath.UnsetValue else maximum

            let rc = Rhino.UI.Dialogs.ShowNumberBox(title, message, defaultValNumber, minimum, maximum)
            return
                if  rc then Some (!defaultValNumber)
                else None

            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Display a save dialog box allowing the user to enter a file name.
    ///  Note, this function does not save the file</summary>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="filter">(string) Optional, A filter string. The filter must be in the following form:
    ///  "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///  If omitted, the filter (*.*) is used</param>
    ///<param name="folder">(string) Optional, A default folder</param>
    ///<param name="filename">(string) Optional, A default file name</param>
    ///<param name="extension">(string) Optional, A default file extension</param>
    ///<returns>(option<string>) an Option of the file name is successful</returns>
    static member SaveFileName(     [<OPT;DEF(null:string)>]title:string,
                                    [<OPT;DEF(null:string)>]filter:string,
                                    [<OPT;DEF(null:string)>]folder:string,
                                    [<OPT;DEF(null:string)>]filename:string,
                                    [<OPT;DEF(null:string)>]extension:string) : option<string> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let fd = Rhino.UI.SaveFileDialog()
            if notNull title then fd.Title <- title
            if notNull filter then fd.Filter <- filter
            if notNull folder then fd.InitialDirectory <- folder
            if notNull filename then fd.FileName <- filename
            if notNull extension then fd.DefaultExt <- extension
            return if fd.ShowSaveDialog() then Some fd.FileName else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Display a dialog box prompting the user to enter a string value</summary>
    ///<param name="message">(string) Optional, A prompt message</param>
    ///<param name="defaultValValue">(string) Optional, A default string value</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>(option<string>) an Option of the newly entered string value</returns>
    static member StringBox(        [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(null:string)>]defaultValValue:string,
                                    [<OPT;DEF(null:string)>]title:string) : option<string> =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            let rc, text = Rhino.UI.Dialogs.ShowEditBox(title, message, defaultValValue, false)
            return if rc then Some text else None
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread


    [<EXT>]
    ///<summary>Display a text dialog box similar to the one used by the _What command</summary>
    ///<param name="message">(string) Optional, A message</param>
    ///<param name="title">(string) Optional, The message title</param>
    ///<returns>(option<unit>) an Option of in any case</returns>
    static member TextOut(
                                    [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(null:string)>]title:string) : unit =
        async{
            if RhinoApp.InvokeRequired then do! Async.SwitchToContext syncContext
            return Rhino.UI.Dialogs.ShowTextDialog(message, title)
            } |> Async.StartImmediateAsTask |> Async.AwaitTask |> Async.RunSynchronously // to start on current thread

