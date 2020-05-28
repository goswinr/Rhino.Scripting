namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.UtilMath
open Rhino.Scripting.ActiceDocument
open System.Collections.Generic
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

 

[<AutoOpen>]
module ExtensionsUserinterface =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with
    


    [<Extension>]
    ///<summary>Display browse-for-folder dialog allowing the user to select a folder</summary>
    ///<param name="folder">(string) Optional, A default folder</param>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<returns>(string option) selected folder option or None if selection was canceled</returns>
    static member BrowseForFolder([<OPT;DEF(null:string)>]folder:string, [<OPT;DEF(null:string)>]message:string) : string option =
        let getKeepEditor () = 
            use dlg = new System.Windows.Forms.FolderBrowserDialog()
            dlg.ShowNewFolderButton <- true
            if notNull folder then
                if IO.Directory.Exists(folder) then
                    dlg.SelectedPath <-  folder
            if notNull message then
                dlg.Description <- message
            if dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK then
                Some(dlg.SelectedPath)
            else
                None
        Synchronisation.DoSync false false getKeepEditor
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


    [<Extension>]
    ///<summary>Displays a list of items in a checkable-style list dialog box</summary>
    ///<param name="items">((string*bool) seq) A list of tuples containing a string and a boolean check state</param>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>((string*bool) ResizeArray option) Option of tuples containing the input string in items along with their new boolean check value</returns>
    static member CheckListBox( items:(string*bool) seq,
                                [<OPT;DEF(null:string)>]message:string,
                                [<OPT;DEF(null:string)>]title:string) : option<ResizeArray<string*bool>> =
        let checkstates = resizeArray { for  item in items -> snd item }
        let itemstrs =    resizeArray { for item in items -> fst item}

        let newcheckstates =
            let getKeepEditor () = UI.Dialogs.ShowCheckListBox(title, message, itemstrs, checkstates)
            Synchronisation.DoSync false false getKeepEditor

        if notNull newcheckstates then
            Some (Seq.zip itemstrs newcheckstates |>  ResizeArray.ofSeq)
        else
            //failwithf "Rhino.Scripting: CheckListBox failed.  items:'%A' message:'%A' title:'%A'" items message title
            None


    [<Extension>]
    ///<summary>Displays a list of items in a combo-style list box dialog</summary>
    ///<param name="items">(string seq) A list of string</param>
    ///<param name="message">(string) Optional, A prompt of message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>(string option) Option of The selected item</returns>
    static member ComboListBox(items:string seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : string option=
        let getKeepEditor () = 
            match UI.Dialogs.ShowComboListBox(title, message, items|> Array.ofSeq) with
            | null -> None
            | :? string as s -> Some s
            | _ -> None
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Display dialog prompting the user to enter a string. The
    ///    string value may span multiple lines</summary>
    ///<param name="defaultValString">(string) Optional, A default string value</param>
    ///<param name="message">(string) Optional, A prompt message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>(string Option) Option of Multiple lines that are separated by carriage return-linefeed combinations</returns>
    static member EditBox(  [<OPT;DEF(null:string)>]defaultValString:string,
                            [<OPT;DEF(null:string)>]message:string,
                            [<OPT;DEF(null:string)>]title:string) : string option =
        let getKeepEditor () = 
            let rc, text = UI.Dialogs.ShowEditBox(title, message, defaultValString, true)
            if rc then Some text else None
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Pause for user input of an angle</summary>
    ///<param name="point">(Point3d) Optional, Default Value: <c>Point3d.Unset</c>
    ///    Starting, or base point</param>
    ///<param name="referencePoint">(Point3d) Optional, Default Value: <c>Point3d.Unset</c>
    ///    If specified, the reference angle is calculated from it and the base point</param>
    ///<param name="defaultValAngleDegrees">(float) Optional, A default angle value specified</param>
    ///<param name="message">(string) Optional, A prompt to display</param>
    ///<returns>(float option) Option of angle in degree</returns>
    static member GetAngle( [<OPT;DEF(Point3d())>]point:Point3d,
                            [<OPT;DEF(Point3d())>]referencePoint:Point3d,
                            [<OPT;DEF(0.0)>]defaultValAngleDegrees:float,
                            [<OPT;DEF(null:string)>]message:string) : float option=
        let get () = 
            let point = if point = Point3d.Origin then Point3d.Unset else point
            let referencepoint = if referencePoint = Point3d.Origin then Point3d.Unset else referencePoint
            let defaultangle = toRadians(defaultValAngleDegrees)
            let rc, angle = Input.RhinoGet.GetAngle(message, point, referencepoint, defaultangle)
            if rc = Commands.Result.Success then Some(toDegrees(angle))
            else None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of one or more boolean values. Boolean values are
    ///    displayed as click-able command line option toggles</summary>
    ///<param name="message">(string) A prompt</param>
    ///<param name="items">((string*string*string) array) List of options. Each option is a tuple of three strings
    ///    [n][1]    description of the boolean value. Must only consist of letters and numbers. (no characters like space, period, or dash)
    ///    [n][2]    string identifying the false value
    ///    [n][3]    string identifying the true value</param>
    ///<param name="defaultVals">(bool seq) List of boolean values used as default or starting values</param>
    ///<returns>(bool ResizeArray) Option of a list of values that represent the boolean values</returns>
    static member GetBoolean(message:string, items:(string*string*string) array, defaultVals:bool array) :option<ResizeArray<bool>> =
        let get () = 
            use go = new Input.Custom.GetOption()
            go.AcceptNothing(true)
            go.SetCommandPrompt( message )
            let count = Seq.length(items)
            if count < 1 || count <> Seq.length(defaultVals) then failwithf "Rhino.Scripting: GetBoolean failed.  message:'%A' items:'%A' defaultVals:'%A'" message items defaultVals
            let toggles = ResizeArray()
            for i in range(count) do
                let initial = defaultVals.[i]
                let item = items.[i]
                let name, off, on = item
                let t = new Input.Custom.OptionToggle( initial, off, on )
                toggles.Add(t)
                go.AddOptionToggle(name, ref t) |> ignore
            let mutable getrc = go.Get()
            while getrc = Input.GetResult.Option do
                getrc <- go.Get()

            let res =
                if getrc <> Input.GetResult.Nothing then
                    None
                else
                    Some (ResizeArray.map (fun (t:Input.Custom.OptionToggle) ->  t.CurrentValue) toggles)
            if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
            res
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of a box</summary>
    ///<param name="mode">(int) Optional, Default Value: <c>0</c>
    ///    The box selection mode.
    ///    0 = All modes
    ///    1 = Corner. The base rectangle is created by picking two corner points
    ///    2 = 3-Point. The base rectangle is created by picking three points
    ///    3 = Vertical. The base vertical rectangle is created by picking three points.
    ///    4 = Center. The base rectangle is created by picking a center point and a corner point</param>
    ///<param name="basePoint">(Point3d) Optional, Optional 3D base point</param>
    ///<param name="prompt1">(string) Optional, Prompt1 of 'optional prompts to set'</param>
    ///<param name="prompt2">(string) Optional, Prompt2 of 'optional prompts to set'</param>
    ///<param name="prompt3">(string) Optional, Prompt3 of 'optional prompts to set'</param>
    ///<returns>(Point3d array) option) array of eight Point3d that define the corners of the box on success</returns>
    static member GetBox(   [<OPT;DEF(0)>]mode:int,
                            [<OPT;DEF(Point3d())>]basePoint:Point3d,
                            [<OPT;DEF(null:string)>]prompt1:string,
                            [<OPT;DEF(null:string)>]prompt2:string,
                            [<OPT;DEF(null:string)>]prompt3:string) : (Point3d []) option=
        let get () = 
            let basePoint = if basePoint <> Point3d.Origin then basePoint else  Point3d.Unset
            let m =
                match mode with
                |0 -> Input.GetBoxMode.All
                |1 -> Input.GetBoxMode.Corner
                |2 -> Input.GetBoxMode.ThreePoint
                |3 -> Input.GetBoxMode.Vertical
                |4 -> Input.GetBoxMode.Center
                |_ -> failwithf "GetBox:Bad mode %A" mode

            let box = ref (Box())
            let rc= Input.RhinoGet.GetBox(box, m, basePoint, prompt1, prompt2, prompt3)
            if rc = Commands.Result.Success then Some ((!box).GetCorners())
            else None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Display the Rhino color picker dialog allowing the user to select an RGB color</summary>
    ///<param name="color">(Drawing.Color) Optional, Default Value: <c>Drawing.Color.Black</c></param>
    ///<returns>(Drawing.Color option) an Option of RGB color</returns>
    static member GetColor([<OPT;DEF(Drawing.Color())>]color:Drawing.Color) : option<Drawing.Color> =
        let get () = 
            let zero = Drawing.Color()
            let col = ref(if color = zero then  Drawing.Color.Black else color)
            let rc = UI.Dialogs.ShowColorDialog(col)
            if rc then Some (!col) else None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Retrieves the cursor's position</summary>
    ///<returns>(Point3d * Point2d * Guid * Point2d) a Tuple of containing the following information
    ///    0  Point3d: cursor position in world coordinates
    ///    1  Point2d: cursor position in screen coordinates
    ///    2  Guid:    objectId of the active viewport
    ///    3  Point2d: cursor position in client coordinates</returns>
    static member GetCursorPos() : Point3d * Point2d * Guid * Point2d =
        let get () =   //or skip ?
            let view = Doc.Views.ActiveView
            let screenpt = UI.MouseCursor.Location
            let clientpt = view.ScreenToClient(screenpt)
            let viewport = view.ActiveViewport
            let xf = viewport.GetTransform(DocObjects.CoordinateSystem.Screen, DocObjects.CoordinateSystem.World)
            let worldpt = Point3d(clientpt.X, clientpt.Y, 0.0)
            worldpt.Transform(xf)
            if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show() //or skip ?
            worldpt, screenpt, viewport.Id, clientpt
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of a distance</summary>
    ///<param name="firstPt">(Point3d) Optional, First distance point</param>
    ///<param name="distance">(float) Optional, Default distance</param>
    ///<param name="firstPtMsg">(string) Optional, Default Value: <c>"First distance point"</c>
    ///    Prompt for the first distance point</param>
    ///<param name="secondPtMsg">(string) Optional, Default Value: <c>"Second distance point"</c>
    ///    Prompt for the second distance point</param>
    ///<returns>(float option) an Option of The distance between the two points</returns>
    static member GetDistance(  [<OPT;DEF(Point3d())>]firstPt:Point3d,
                                [<OPT;DEF(0.0)>]distance:float,
                                [<OPT;DEF("First distance point")>]firstPtMsg:string,
                                [<OPT;DEF("Second distance point")>]secondPtMsg:string) : float option =
        let get () = 
            let pt1 =
                if firstPt = Point3d.Origin then
                    let gp1 = new Input.Custom.GetPoint()
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
            
            match pt1 with
            | Some pt ->
                let gp2 = new Input.Custom.GetPoint()
                if distance <> 0.0 then
                    gp2.AcceptNothing(true)
                    gp2.SetCommandPrompt(sprintf "%s<%f>" secondPtMsg distance)
                else
                    gp2.SetCommandPrompt(secondPtMsg)
                gp2.DrawLineFromPoint(pt, true)
                gp2.EnableDrawLineFromPoint(true)
                match gp2.Get() with
                | Input.GetResult.Point ->
                    let d = gp2.Point().DistanceTo(pt)
                    RhinoScriptSyntax.Print ("Distance: " + d.ToNiceString + " " + Doc.GetUnitSystemName(true, true, false, false) )
                    gp2.Dispose()
                    Some d
                | _ ->
                    gp2.Dispose()
                    None
            | _ -> None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Prompt the user to pick one or more surface or polysurface edge curves</summary>
    ///<param name="message">(string) Optional, Default Value: <c>Select Edges</c>
    ///    A prompt or message</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///    Minimum number of edges to select</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///    Maximum number of edges to select</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the duplicated edge curves</param>
    ///<returns>((Guid*Guid*Point3d) ResizeArray) an Option of a List of selection prompts (curve objectId, parent objectId, selection point)</returns>
    static member GetEdgeCurves(    [<OPT;DEF("Select Edges")>]message:string,
                                    [<OPT;DEF(1)>]minCount:int,
                                    [<OPT;DEF(0)>]maxCount:int,
                                    [<OPT;DEF(false)>]select:bool) : option<ResizeArray<Guid*Guid*Point3d>> =
        let get () = 
            if maxCount > 0 && minCount > maxCount then failwithf "GetEdgeCurves: minCount %d is bigger than  maxCount %d" minCount  maxCount
            use go = new Input.Custom.GetObject()
            go.SetCommandPrompt(message)
            go.GeometryFilter <- DocObjects.ObjectType.Curve
            go.GeometryAttributeFilter <- Input.Custom.GeometryAttributeFilter.EdgeCurve
            go.EnablePreSelect(false, true)
            let rc = go.GetMultiple(minCount, maxCount)            
            if rc <> Input.GetResult.Object then None
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
                        rhobj.Select(true)|> ignore //TODO make sync ?
                    Doc.Views.Redraw()
                Some r
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of a whole number</summary>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="number">(int) Optional, A default whole number value</param>
    ///<param name="minimum">(int) Optional, A minimum allowable value</param>
    ///<param name="maximum">(int) Optional, A maximum allowable value</param>
    ///<returns>(int option) an Option of The whole number input by the user</returns>
    static member GetInteger(   [<OPT;DEF(null:string)>]message:string,
                                [<OPT;DEF(2147482999)>]number:int,
                                [<OPT;DEF(2147482999)>]minimum:int,
                                [<OPT;DEF(2147482999)>]maximum:int) : int option =
        let get () = 
            use gi = new Input.Custom.GetInteger()
            if notNull message then gi.SetCommandPrompt(message)
            if number  <> 2147482999 then gi.SetDefaultInteger(number)
            if minimum <> 2147482999 then gi.SetLowerLimit(minimum, false)
            if maximum <> 2147482999 then gi.SetUpperLimit(maximum, false)
            if gi.Get() <> Input.GetResult.Number then
                gi.Dispose()
                None
            else
                let rc = gi.Number()
                gi.Dispose()
                Some rc
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Displays dialog box prompting the user to select a layer</summary>
    ///<param name="title">(string) Optional, Default Value: <c>"Select Layer"</c>
    ///    Dialog box title</param>
    ///<param name="layer">(string) Optional, Name of a layer to preselect. If omitted, the current layer will be preselected</param>
    ///<param name="showNewButton">(bool) Optional, Default Value: <c>false</c>
    ///    Show new button of on the dialog</param>
    ///<param name="showSetCurrent">(bool) Optional, Default Value: <c>false</c>
    ///    Show set current  button on the dialog</param>
    ///<returns>(string option) an Option of name of selected layer</returns>
    static member GetLayer( [<OPT;DEF("Select Layer")>]title:string,
                            [<OPT;DEF(null:string)>]layer:string,
                            [<OPT;DEF(false)>]showNewButton:bool,
                            [<OPT;DEF(false)>]showSetCurrent:bool) : string option =
        let getKeepEditor () = 
            let layerindex = ref Doc.Layers.CurrentLayerIndex
            if notNull layer then
                let layerinstance = Doc.Layers.FindName(layer)
                if notNull layerinstance then layerindex := layerinstance.Index
            let rc = UI.Dialogs.ShowSelectLayerDialog(layerindex, title, showNewButton, showSetCurrent, ref true)
            if not rc then None
            else
                let layer = Doc.Layers.[!layerindex]
                Some layer.FullPath
        Synchronisation.DoSync false false getKeepEditor



    [<Extension>]
    ///<summary>Displays a dialog box prompting the user to select one or more layers</summary>
    ///<param name="title">(string) Optional, Default Value: <c>"Select Layers"</c>
    ///    Dialog box title</param>
    ///<param name="showNewButton">(bool) Optional, Default Value: <c>false</c>
    ///    Optional button to show on the dialog</param>
    ///<returns>(string ResizeArray) an Option of The names of selected layers</returns>
    static member GetLayers([<OPT;DEF("Select Layers")>]title:string, [<OPT;DEF(false)>]showNewButton:bool) : option<string ResizeArray> =
        let getKeepEditor () = 
            let rc, layerindices = UI.Dialogs.ShowSelectMultipleLayersDialog(null, title, showNewButton)
            if rc then
                Some (resizeArray { for index in layerindices do yield  Doc.Layers.[index].FullPath })
            else
                None
        Synchronisation.DoSync false false getKeepEditor




    [<Extension>]
    ///<summary>Prompts the user to pick points that define a line</summary>
    ///<param name="mode">(int) Optional, Default Value: <c>0</c>
    ///    Line definition mode.
    ///    0  Default - Show all modes, start in two-point mode
    ///    1  Two-point - Defines a line from two points.
    ///    2  Normal - Defines a line normal to a location on a surface.
    ///    3  Angled - Defines a line at a specified angle from a reference line.
    ///    4  Vertical - Defines a line vertical to the construction plane.
    ///    5  Four-point - Defines a line using two points to establish direction and two points to establish length.
    ///    6  Bisector - Defines a line that bisects a specified angle.
    ///    7  Perpendicular - Defines a line perpendicular to or from a curve
    ///    8  Tangent - Defines a line tangent from a curve.
    ///    9  Extension - Defines a line that extends from a curve</param>
    ///<param name="point">(Point3d) Optional, Optional starting point</param>
    ///<param name="message1">(string) Optional, Message1 of optional prompts</param>
    ///<param name="message2">(string) Optional, Message2 of optional prompts</param>
    ///<param name="message3">(string) Optional, Message3 of optional prompts</param>
    ///<returns>(Line option) an Option of a Line</returns>
    static member GetLine(  [<OPT;DEF(0)>]mode:int,
                            [<OPT;DEF(Point3d())>]point:Point3d,
                            [<OPT;DEF(null:string)>]message1:string,
                            [<OPT;DEF(null:string)>]message2:string,
                            [<OPT;DEF(null:string)>]message3:string) : option<Line> =
        let get () = 
            use gl = new Input.Custom.GetLine()
            if mode = 0 then gl.EnableAllVariations(true)
            else  gl.GetLineMode <- LanguagePrimitives.EnumOfValue( mode-1)
            if point <> Point3d.Origin then
                gl.SetFirstPoint(point)
            if notNull message1 then gl.FirstPointPrompt <- message1
            if notNull message2 then gl.MidPointPrompt <- message2
            if notNull message3 then gl.SecondPointPrompt <- message3
            let rc, line = gl.Get()
            if rc = Commands.Result.Success then
                Some line
            else
                None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Displays a dialog box prompting the user to select one linetype</summary>
    ///<param name="defaultValLinetype">(string) Optional, Optional. The name of the linetype to select. If omitted, the current linetype will be selected</param>
    ///<param name="showByLayer">(bool) Optional, Default Value: <c>false</c>
    ///    If True, the "by Layer" linetype will show. Defaults to False</param>
    ///<returns>(string option) an Option of The names of selected linetype</returns>
    static member GetLinetype(  [<OPT;DEF(null:string)>]defaultValLinetype:string,
                                [<OPT;DEF(false)>]showByLayer:bool) : string option =
        let getKeepEditor () = 
            let mutable ltinstance = Doc.Linetypes.CurrentLinetype
            if notNull defaultValLinetype then
                let ltnew = Doc.Linetypes.FindName(defaultValLinetype)
                if notNull ltnew  then ltinstance <- ltnew
            try
                let objectId = UI.Dialogs.ShowLineTypes("Select Linetype", "Select Linetype", Doc) :?> Guid  // this fails if clicking in void
                let linetype = Doc.Linetypes.FindId(objectId)
                Some linetype.Name
            with _ ->
                None
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Prompts the user to pick one or more mesh faces</summary>
    ///<param name="objectId">(Guid) The mesh object's identifier</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Select Mesh Faces"</c>
    ///    A prompt of message</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///    The minimum number of faces to select</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///    The maximum number of faces to select.
    ///    If 0, the user must press enter to finish selection.
    ///    If -1, selection stops as soon as there are at least minCount faces selected</param>
    ///<returns>(int ResizeArray) an Option of of mesh face indices on success</returns>
    static member GetMeshFaces( objectId:Guid,
                                [<OPT;DEF("Select Mesh Faces")>]message:string,
                                [<OPT;DEF(1)>]minCount:int,
                                [<OPT;DEF(0)>]maxCount:int) : option<ResizeArray<int>> =
        let get () = 
            Doc.Objects.UnselectAll() |> ignore
            Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            go.SetCustomGeometryFilter(fun rhinoobject _ _ -> objectId = rhinoobject.Id)
            go.SetCommandPrompt(message)
            go.GeometryFilter <- DocObjects.ObjectType.MeshFace
            go.AcceptNothing(true)
            if go.GetMultiple(minCount, maxCount) <> Input.GetResult.Object then
                None
            else
                let objrefs = go.Objects()
                let rc = resizeArray { for  item in objrefs do yield item.GeometryComponentIndex.Index }
                Some rc
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Prompts the user to pick one or more mesh vertices</summary>
    ///<param name="objectId">(Guid) The mesh object's identifier</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Select Mesh Vertices"</c>
    ///    A prompt of message</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///    The minimum number of vertices to select</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///    The maximum number of vertices to select. If 0, the user must
    ///    press enter to finish selection. If -1, selection stops as soon as there
    ///    are at least minCount vertices selected</param>
    ///<returns>(int ResizeArray) an Option of of mesh vertex indices on success</returns>
    static member GetMeshVertices(  objectId:Guid,
                                    [<OPT;DEF("Select Mesh Vertices")>]message:string,
                                    [<OPT;DEF(1)>]minCount:int,
                                    [<OPT;DEF(0)>]maxCount:int) : option<ResizeArray<int>> =
        let get () = 
            Doc.Objects.UnselectAll() |> ignore
            Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            go.SetCustomGeometryFilter(fun rhinoobject _ _ -> objectId = rhinoobject.Id)
            go.SetCommandPrompt(message)
            go.GeometryFilter <-  DocObjects.ObjectType.MeshVertex
            go.AcceptNothing(true)
            if go.GetMultiple(minCount, maxCount) <> Input.GetResult.Object then
                None
            else
                let objrefs = go.Objects()
                let rc = resizeArray { for  item in objrefs do yield item.GeometryComponentIndex.Index }
                Some rc
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of a point</summary>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="basePoint">(Point3d) Optional, Point3d identifying a starting, or base point</param>
    ///<param name="distance">(float) Optional, Constraining distance. If distance is specified, basePoint must also be specified</param>
    ///<param name="inPlane">(bool) Optional, Default Value: <c>false</c>
    ///    Constrains the point selections to the active construction plane</param>
    ///<returns>(Point3d option) an Option of point on success</returns>
    static member GetPoint( [<OPT;DEF(null:string)>]message:string,
                            [<OPT;DEF(Point3d())>]basePoint:Point3d,
                            [<OPT;DEF(0.0)>]distance:float,
                            [<OPT;DEF(false)>]inPlane:bool) : Point3d option =
        let get () = 
            use gp = new Input.Custom.GetPoint()
            if notNull message then gp.SetCommandPrompt(message)
            if basePoint <> Point3d.Origin then
                gp.DrawLineFromPoint(basePoint, true)
                gp.EnableDrawLineFromPoint(true)
                if distance<>0.0 then gp.ConstrainDistanceFromBasePoint(distance)
            if inPlane then gp.ConstrainToConstructionPlane(true)|> ignore
            gp.Get() |> ignore
            if gp.CommandResult() <> Commands.Result.Success then
                None
            else
                let pt = gp.Point()
                Some pt
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get



    [<Extension>]
    ///<summary>Pauses for user input of a point constrainted to a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Pick Point On Curve"</c>
    ///    A prompt of message</param>
    ///<returns>(Point3d option) an Option of 3d point</returns>
    static member GetPointOnCurve(curveId:Guid, [<OPT;DEF("Pick Point On Curve")>]message:string) : Point3d option =
        let get () = 
            let curve = RhinoScriptSyntax.CoerceCurve(curveId)
            use gp = new Input.Custom.GetPoint()
            gp.SetCommandPrompt(message)
            gp.Constrain(curve, false) |> ignore
            gp.Get() |> ignore
            if gp.CommandResult() <> Commands.Result.Success then
                None
            else
                let pt = gp.Point()
                Some pt
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of a point constrained to a mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of the mesh to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Pick Point On Mesh"</c>
    ///    A prompt or message</param>
    ///<returns>(Point3d option) an Option of 3d point</returns>
    static member GetPointOnMesh(meshId:Guid, [<OPT;DEF("Pick Point On Mesh")>]message:string) : Point3d option =
        let get () = 
            let cmdrc, point = Input.RhinoGet.GetPointOnMesh(meshId, message, false)
            if cmdrc = Commands.Result.Success then Some point
            else None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of a point constrained to a surface or polysurface
    ///    object</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>"Pick Point on Surface or Polysurface"</c>
    ///    A prompt or message</param>
    ///<returns>(Point3d option) an Option of 3d point</returns>
    static member GetPointOnSurface(surfaceId:Guid, [<OPT;DEF("Pick Point on Surface or Polysurface")>]message:string) : Point3d option =
        let get () = 
            use gp = new Input.Custom.GetPoint()
            gp.SetCommandPrompt(message)
            match RhinoScriptSyntax.CoerceGeometry surfaceId with
            | :? Surface as srf ->
                gp.Constrain(srf, false) |> ignore

            | :? Brep as brep ->
                gp.Constrain(brep, -1, -1, false) |> ignore

            | _ ->
                failwithf "Rhino.Scripting: GetPointOnSurface failed input is not surface or polysurface.  surfaceId:'%A' message:'%A'" surfaceId message

            gp.Get() |>ignore
            if gp.CommandResult() <> Commands.Result.Success then
                None
            else
                let pt = gp.Point()
                Some pt
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of one or more points</summary>
    ///<param name="drawLines">(bool) Optional, Default Value: <c>false</c>
    ///    Draw lines between points</param>
    ///<param name="inPlane">(bool) Optional, Default Value: <c>false</c>
    ///    Constrain point selection to the active construction plane</param>
    ///<param name="message1">(string) Optional, A prompt or message for the first point</param>
    ///<param name="message2">(string) Optional, A prompt or message for the next points</param>
    ///<param name="maxPoints">(int) Optional, Maximum number of points to pick. If not specified, an
    ///    unlimited number of points can be picked</param>
    ///<returns>(Point3d array) an Option of of 3d points</returns>
    static member GetPoints(    [<OPT;DEF(false)>]drawLines:bool,
                                [<OPT;DEF(false)>]inPlane:bool,
                                [<OPT;DEF(null:string)>]message1:string,
                                [<OPT;DEF(null:string)>]message2:string,
                                [<OPT;DEF(0)>]maxPoints:int) : option<Point3d ResizeArray> =
                                //[<OPT;DEF(Point3d())>]basePoint:Point3d) // Ignored here because ignored in python too

        let get () = 
            use gp = new Input.Custom.GetPoint()
            if notNull message1 then gp.SetCommandPrompt(message1)
            gp.EnableDrawLineFromPoint( drawLines )
            if inPlane then
                gp.ConstrainToConstructionPlane(true) |> ignore
                let plane = Doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
                gp.Constrain(plane, false) |> ignore
            let mutable getres = gp.Get()
            if gp.CommandResult() <> Commands.Result.Success then None
            else
                let mutable prevPoint = gp.Point()
                let rc = ResizeArray([prevPoint])
                if maxPoints = 0 || maxPoints > 1 then
                    let mutable currentpoint = 1
                    if notNull message2 then gp.SetCommandPrompt(message2)

                    if drawLines then
                        gp.DynamicDraw.Add (fun args  -> if rc.Count > 1 then
                                                            let c = ApplicationSettings.AppearanceSettings.FeedbackColor
                                                            args.Display.DrawPolyline(rc, c))
                    let mutable cont = true
                    while cont do
                        if maxPoints <> 0 && currentpoint>=maxPoints then cont <- false
                        if cont && drawLines then
                            gp.DrawLineFromPoint(prevPoint, true)
                        if cont then
                            gp.SetBasePoint(prevPoint, true)
                            currentpoint <- currentpoint + 1
                            getres <- gp.Get()
                            if getres = Input.GetResult.Cancel then
                                cont <- false
                                //RhinoApp.WriteLine "GetPoints canceled"
                            if cont && gp.CommandResult() <> Commands.Result.Success then
                                rc.Clear()
                                cont <- false
                                RhinoScriptSyntax.Print "GetPoints no Success"
                            if cont then
                                prevPoint <- gp.Point()
                                rc.Add(prevPoint)
                if rc.Count>0 then
                    RhinoScriptSyntax.Print (rc.Count, "Points picked")
                    Some rc
                else
                    None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()

        Synchronisation.DoSync true true get




    [<Extension>]
    ///<summary>Prompts the user to pick points that define a polyline</summary>
    ///<param name="flags">(int) Optional, Default Value: <c>3</c>
    ///    The options are bit coded flags. Values can be added together to specify more than one option. 
    ///    value description
    ///    1     Permit close option. If specified, then after 3 points have been picked, the user can type "Close" and a closed polyline will be returned.
    ///    2     Permit close snap. If specified, then after 3 points have been picked, the user can pick near the start point and a closed polyline will be returned.
    ///    4     Force close. If specified, then the returned polyline is always closed. If specified, then max must bebet 0 or bigger than 4.
    ///    Note: the default is 3, or "Permit close option = True", "Permit close snap = True", and "Force close = False"</param>
    ///<param name="message1">(string) Optional, A prompt or message for the first point</param>
    ///<param name="message2">(string) Optional, A prompt or message for the second point</param>
    ///<param name="message3">(string) Optional, A prompt or message for the third point</param>
    ///<param name="message4">(string) Optional, A prompt or message for the 'next' point</param>
    ///<param name="min">(int) Optional, Default Value: <c>2</c>
    ///    The minimum number of points to require. The default is 2</param>
    ///<param name="max">(int) Optional, Default Value: <c>0</c>
    ///    The maximum number of points to require; 0 for no limit.</param>
    ///<returns>(Polyline option) an Option of a  polyline</returns>
    static member GetPolyline(          [<OPT;DEF(3)>]flags:int,
                                        [<OPT;DEF(null:string)>]message1:string,
                                        [<OPT;DEF(null:string)>]message2:string,
                                        [<OPT;DEF(null:string)>]message3:string,
                                        [<OPT;DEF(null:string)>]message4:string,
                                        [<OPT;DEF(2147482999)>]min:int,
                                        [<OPT;DEF(0)>]max:int) : option<Polyline> =
        let get () = 
            let gpl = new Input.Custom.GetPolyline()
            if notNull message1 then gpl.FirstPointPrompt <- message1
            if notNull message2 then gpl.SecondPointPrompt <- message2
            if notNull message3 then gpl.ThirdPointPrompt <- message3
            if notNull message4 then gpl.FourthPointPrompt <- message4
            gpl.MinPointCount <- min
            if max>min then gpl.MaxPointCount <- max
            let rc, polyline = gpl.Get()
            Doc.Views.Redraw()
            if rc = Commands.Result.Success then Some polyline
            else None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of a number</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Number"</c>
    ///    A prompt or message</param>
    ///<param name="number">(float) Optional, A default number value</param>
    ///<param name="minimum">(float) Optional, A minimum allowable value</param>
    ///<param name="maximum">(float) Optional, A maximum allowable value</param>
    ///<returns>(float option) an Option of The number input by the user</returns>
    static member GetReal(              [<OPT;DEF("Number")>]message:string,
                                        [<OPT;DEF(7e89)>]number:float,
                                        [<OPT;DEF(7e89)>]minimum:float,
                                        [<OPT;DEF(7e89)>]maximum:float) : float option =
        let get () = 
            let gn = new Input.Custom.GetNumber()
            if notNull message then gn.SetCommandPrompt(message)
            if number <> 7e89 then gn.SetDefaultNumber(number)
            if minimum <> 7e89 then gn.SetLowerLimit(minimum, false)
            if maximum <> 7e89 then gn.SetUpperLimit(maximum, false)
            if gn.Get() <> Input.GetResult.Number then None
            else
                let rc = gn.Number()
                gn.Dispose()
                Some rc
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of a rectangle</summary>
    ///<param name="mode">(int) Optional, Default Value: <c>0</c>
    ///    The rectangle selection mode. The modes are as follows
    ///    0 = All modes
    ///    1 = Corner - a rectangle is created by picking two corner points
    ///    2 = 3Point - a rectangle is created by picking three points
    ///    3 = Vertical - a vertical rectangle is created by picking three points
    ///    4 = Center - a rectangle is created by picking a center point and a corner point</param>
    ///<param name="basePoint">(Point3d) Optional, A 3d base point</param>
    ///<param name="prompt1">(string) Optional, Prompt1 of optional prompts</param>
    ///<param name="prompt2">(string) Optional, Prompt2 of optional prompts</param>
    ///<param name="prompt3">(string) Optional, Prompt3 of optional prompts</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) an Option of four 3d points that define the corners of the rectangle</returns>
    static member GetRectangle(
                                        [<OPT;DEF(0)>]mode:int,
                                        [<OPT;DEF(Point3d())>]basePoint:Point3d,
                                        [<OPT;DEF(null:string)>]prompt1:string,
                                        [<OPT;DEF(null:string)>]prompt2:string,
                                        [<OPT;DEF(null:string)>]prompt3:string) : option<Point3d * Point3d * Point3d * Point3d> =
        let get () = 
            let mode : Input.GetBoxMode = LanguagePrimitives.EnumOfValue mode

            let basePoint = if basePoint = Point3d.Origin then Point3d.Unset else basePoint
            let prompts = ResizeArray([""; ""; ""])
            if notNull prompt1 then prompts.[0] <- prompt1
            if notNull prompt2 then prompts.[1] <- prompt2
            if notNull prompt3 then prompts.[2] <- prompt3
            let rc, corners = Input.RhinoGet.GetRectangle(mode, basePoint, prompts)
            if rc = Commands.Result.Success then Some (corners.[0], corners.[1], corners.[2], corners.[3])
            else None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Pauses for user input of a string value</summary>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="defaultValString">(string) Optional, A default value</param>
    ///<param name="strings">(string seq) Optional, List of strings to be displayed as a click-able command options.
    ///    Note, strings cannot begin with a numeric character</param>
    ///<returns>(string option) an Option of The string either input or selected by the user .
    ///    If the user presses the Enter key without typing in a string, an empty string "" is returned</returns>
    static member GetString(
                                        [<OPT;DEF(null:string)>]message:string,
                                        [<OPT;DEF(null:string)>]defaultValString:string,
                                        [<OPT;DEF(null:string seq)>]strings:string seq) : string option =
        let get () = 
            let gs = new Input.Custom.GetString()
            gs.AcceptNothing(true)
            if notNull message then gs.SetCommandPrompt(message)
            if notNull defaultValString then gs.SetDefaultString(defaultValString)
            if notNull strings then
                for s in strings do gs.AddOption(s) |> ignore
            let result = gs.Get()
            if result = Input.GetResult.Cancel then
                None
            elif( result = Input.GetResult.Option ) then
                Some <| gs.Option().EnglishName
            else
                Some <| gs.StringResult()
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Display a list of items in a list box dialog</summary>
    ///<param name="items">(string IList) A list of values to select</param>
    ///<param name="message">(string) Optional, A prompt of message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="defaultVal">(string) Optional, Selected item in the list</param>
    ///<returns>(string option) an Option of he selected item</returns>
    static member ListBox(              items:string IList,
                                        [<OPT;DEF(null:string)>]message:string,
                                        [<OPT;DEF(null:string)>]title:string,
                                        [<OPT;DEF(null:string)>]defaultVal:string) : string option =
        let getKeepEditor () = 
            match UI.Dialogs.ShowListBox(title, message, Array.ofSeq items , defaultVal) with
            | null ->  None
            |  :? string as s -> Some s
            | _ -> None

        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Displays a message box. A message box contains a message and
    ///    title, plus any combination of predefined icons and push buttons</summary>
    ///<param name="message">(string) A prompt or message</param>
    ///<param name="buttons">(int) Optional, Default Value: <c>0</c>
    ///    Buttons and icon to display as a bit coded flag. Can be a combination of the
    ///    following flags. If omitted, an OK button and no icon is displayed
    ///    0      Display OK button only.
    ///    1      Display OK and Cancel buttons.
    ///    2      Display Abort, Retry, and Ignore buttons.
    ///    3      Display Yes, No, and Cancel buttons.
    ///    4      Display Yes and No buttons.
    ///    5      Display Retry and Cancel buttons.
    ///    16     Display Critical Message icon.
    ///    32     Display Warning Query icon.
    ///    48     Display Warning Message icon.
    ///    64     Display Information Message icon.
    ///    0      First button is the default.
    ///    256    Second button is the default.
    ///    512    Third button is the default.
    ///    768    Fourth button is the default.
    ///    0      Application modal. The user must respond to the message box
    ///      before continuing work in the current application.
    ///    4096   System modal. The user must respond to the message box
    ///      before continuing work in any application</param>
    ///<param name="title">(string) Optional, Default Value: <c>""</c>
    ///    The dialog box title</param>
    ///<returns>(int option) an Option of indicating which button was clicked:
    ///    1      OK button was clicked.
    ///    2      Cancel button was clicked.
    ///    3      Abort button was clicked.
    ///    4      Retry button was clicked.
    ///    5      Ignore button was clicked.
    ///    6      Yes button was clicked.
    ///    7      No button was clicked</returns>
    static member MessageBox(           message:string,
                                        [<OPT;DEF(0)>]buttons:int,
                                        [<OPT;DEF("")>]title:string) : int option =
        let getKeepEditor () = 
            let mutable buttontyp =  buttons &&& 0x00000007 //111 in binary
            let mutable btn = UI.ShowMessageButton.OK
            if   buttontyp = 1 then btn <- UI.ShowMessageButton.OKCancel
            elif buttontyp = 2 then btn <- UI.ShowMessageButton.AbortRetryIgnore
            elif buttontyp = 3 then btn <- UI.ShowMessageButton.YesNoCancel
            elif buttontyp = 4 then btn <- UI.ShowMessageButton.YesNo
            elif buttontyp = 5 then btn <- UI.ShowMessageButton.RetryCancel
            let icontyp = buttons &&& 0x00000070
            let mutable icon = UI.ShowMessageIcon.None
            if   icontyp = 16 then icon <- UI.ShowMessageIcon.Error
            elif icontyp = 32 then icon <- UI.ShowMessageIcon.Question
            elif icontyp = 48 then icon <- UI.ShowMessageIcon.Warning
            elif icontyp = 64 then icon <- UI.ShowMessageIcon.Information
            ////// 15 Sep 2014 Alain - default button not supported in new version of RC
            ////// that isn"t tied to Windows.Forms but it probably will so I"m commenting
            ////// the old code instead of deleting it.
            //defbtntyp = buttons & 0x00000300
            //defbtn = Windows.Forms.MessageDefaultButton.Button1
            //if defbtntyp = 256:
            //    defbtn = Windows.Forms.MessageDefaultButton.Button2
            //elif defbtntyp = 512:
            //    defbtn <- Windows.Forms.MessageDefaultButton.Button3
            let dlgresult = UI.Dialogs.ShowMessage(message, title, btn, icon)
            if   dlgresult = UI.ShowMessageResult.OK then     Some 1
            elif dlgresult = UI.ShowMessageResult.Cancel then Some 2
            elif dlgresult = UI.ShowMessageResult.Abort then  Some 3
            elif dlgresult = UI.ShowMessageResult.Retry then  Some 4
            elif dlgresult = UI.ShowMessageResult.Ignore then Some 5
            elif dlgresult = UI.ShowMessageResult.Yes then    Some 6
            elif dlgresult = UI.ShowMessageResult.No then     Some 7
            else None
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Displays list of items and their values in a property-style list box dialog</summary>
    ///<param name="items">(string IList) list of string items</param>
    ///<param name="values">(string seq) the corresponding values to the items</param>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>(string array option) an Option of of new values on success</returns>
    static member PropertyListBox(  items:string IList,
                                    values:string seq,
                                    [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(null:string)>]title:string) : string array option =
        let getKeepEditor () = 
            let values = resizeArray { for  v in values do yield v.ToString() }
            match UI.Dialogs.ShowPropertyListBox(title, message, Array.ofSeq items , values) with
            | null ->  None
            | s -> Some s
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Displays a list of items in a multiple-selection list box dialog</summary>
    ///<param name="items">(string IList) A zero-based list of string items</param>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="defaultVals">(string IList) Optional, a list if multiple items that are pre-selected</param>
    ///<returns>(string array option) an Option of containing the selected items</returns>
    static member MultiListBox(     items:string IList,
                                    [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(null:string)>]title:string,
                                    [<OPT;DEF(null:string IList)>]defaultVals:string IList) : string array option =
        let getKeepEditor () = 
            let r =  UI.Dialogs.ShowMultiListBox(title, message, items, defaultVals)
            if notNull r then Some r else None
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Displays file open dialog box allowing the user to enter a file name.
    ///    Note, this function does not open the file</summary>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="filter">(string) Optional, A filter string. The filter must be in the following form:
    ///    "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///    If omitted, the filter (*.*) is used</param>
    ///<param name="folder">(string) Optional, A default folder</param>
    ///<param name="filename">(string) Optional, A default file name</param>
    ///<param name="extension">(string) Optional, A default file extension</param>
    ///<returns>(string option) an Option of file name is successful</returns>
    static member OpenFileName(     [<OPT;DEF(null:string)>]title:string,
                                    [<OPT;DEF(null:string)>]filter:string,
                                    [<OPT;DEF(null:string)>]folder:string,
                                    [<OPT;DEF(null:string)>]filename:string,
                                    [<OPT;DEF(null:string)>]extension:string) : string option =
        let getKeepEditor () = 
            let fd = UI.OpenFileDialog()
            if notNull title then fd.Title <- title
            if notNull filter then fd.Filter <- filter
            if notNull folder then fd.InitialDirectory <- folder
            if notNull filename then fd.FileName <- filename
            if notNull extension then fd.DefaultExt <- extension
            if fd.ShowOpenDialog() then Some fd.FileName
            else None
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Displays file open dialog box allowing the user to select one or more file names.
    ///    Note, this function does not open the file</summary>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="filter">(string) Optional, A filter string. The filter must be in the following form:
    ///    "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///    If omitted, the filter (*.*) is used</param>
    ///<param name="folder">(string) Optional, A default folder</param>
    ///<param name="filename">(string) Optional, A default file name</param>
    ///<param name="extension">(string) Optional, A default file extension</param>
    ///<returns>(string array option) an Option of of selected file names</returns>
    static member OpenFileNames(    [<OPT;DEF(null:string)>]title:string,
                                    [<OPT;DEF(null:string)>]filter:string,
                                    [<OPT;DEF(null:string)>]folder:string,
                                    [<OPT;DEF(null:string)>]filename:string,
                                    [<OPT;DEF(null:string)>]extension:string) : string array option =
        let getKeepEditor () = 
            let fd = UI.OpenFileDialog()
            if notNull title then fd.Title <- title
            if notNull filter then fd.Filter <- filter
            if notNull folder then fd.InitialDirectory <- folder
            if notNull filename then fd.FileName <- filename
            if notNull extension then fd.DefaultExt <- extension
            fd.MultiSelect <- true
            if fd.ShowOpenDialog() then Some fd.FileNames
            else None
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Display a context-style popup menu. The popup menu can appear almost
    ///    anywhere, and can be dismissed by clicking the left or right mouse buttons</summary>
    ///<param name="items">(string seq) List of strings representing the menu items. An empty string or None
    ///    will create a separator</param>
    ///<param name="modes">(int seq) Optional, List of numbers identifying the display modes. If omitted, all
    ///    modes are enabled.
    ///      0 = menu item is enabled
    ///      1 = menu item is disabled
    ///      2 = menu item is checked
    ///      3 = menu item is disabled and checked</param>
    ///<param name="point">(Point3d) Optional, A 3D point where the menu item will appear. If omitted, the menu
    ///    will appear at the current cursor position</param>
    ///<param name="view">(string) Optional, If point is specified, the view in which the point is computed.
    ///    If omitted, the active view is used</param>
    ///<returns>(int) index of the menu item picked or -1 if no menu item was picked</returns>
    static member PopupMenu(        items:string seq,
                                    [<OPT;DEF(null:int seq)>]modes:int seq,
                                    [<OPT;DEF(Point3d())>]point:Point3d,
                                    [<OPT;DEF(null:string)>]view:string) : int =
        let getKeepEditor () = 
            let mutable screenpoint = Windows.Forms.Cursor.Position
            if Point3d.Origin <> point then
                let view = RhinoScriptSyntax.CoerceView(view)
                let viewport = view.ActiveViewport
                let point2d = viewport.WorldToClient(point)
                screenpoint <- viewport.ClientToScreen(point2d)
            UI.Dialogs.ShowContextMenu(items, screenpoint, modes)
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Display a dialog box prompting the user to enter a number</summary>
    ///<param name="message">(string) Optional, Default Value: <c>""</c>
    ///    A prompt message</param>
    ///<param name="defaultValNumber">(float) Optional, A default number</param>
    ///<param name="title">(string) Optional, Default Value: <c>""</c>
    ///    A dialog box title</param>
    ///<param name="minimum">(float) Optional, A minimum allowable value</param>
    ///<param name="maximum">(float) Optional, A maximum allowable value</param>
    ///<returns>(float option) an Option of The newly entered number on success</returns>
    static member RealBox(          [<OPT;DEF("")>]message:string,
                                    [<OPT;DEF(7e89)>]defaultValNumber:float,
                                    [<OPT;DEF("")>]title:string,
                                    [<OPT;DEF(7e89)>]minimum:float,
                                    [<OPT;DEF(7e89)>]maximum:float) : float option =
        let get () = 
            let defaultValNumber = ref <| if defaultValNumber = 7e89 then RhinoMath.UnsetValue else defaultValNumber
            let minimum = if minimum = 7e89 then RhinoMath.UnsetValue else minimum
            let maximum = if maximum = 7e89 then RhinoMath.UnsetValue else maximum

            let rc = UI.Dialogs.ShowNumberBox(title, message, defaultValNumber, minimum, maximum)            
            if  rc then Some (!defaultValNumber)
            else None
            |>> fun _ -> if notNull Synchronisation.SeffWindow then Synchronisation.SeffWindow.Show()
        Synchronisation.DoSync true true get


    [<Extension>]
    ///<summary>Display a save dialog box allowing the user to enter a file name.
    ///    Note, this function does not save the file</summary>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<param name="filter">(string) Optional, A filter string. The filter must be in the following form:
    ///    "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///    If omitted, the filter (*.*) is used</param>
    ///<param name="folder">(string) Optional, A default folder</param>
    ///<param name="filename">(string) Optional, A default file name</param>
    ///<param name="extension">(string) Optional, A default file extension</param>
    ///<returns>(string option) an Option of the file name is successful</returns>
    static member SaveFileName(     [<OPT;DEF(null:string)>]title:string,
                                    [<OPT;DEF(null:string)>]filter:string,
                                    [<OPT;DEF(null:string)>]folder:string,
                                    [<OPT;DEF(null:string)>]filename:string,
                                    [<OPT;DEF(null:string)>]extension:string) : string option =
        let getKeepEditor () = 
            let fd = UI.SaveFileDialog()
            if notNull title then fd.Title <- title
            if notNull filter then fd.Filter <- filter
            if notNull folder then fd.InitialDirectory <- folder
            if notNull filename then fd.FileName <- filename
            if notNull extension then fd.DefaultExt <- extension
            if fd.ShowSaveDialog() then Some fd.FileName else None
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Display a dialog box prompting the user to enter a string value</summary>
    ///<param name="message">(string) Optional, A prompt message</param>
    ///<param name="defaultValValue">(string) Optional, A default string value</param>
    ///<param name="title">(string) Optional, A dialog box title</param>
    ///<returns>(string option) an Option of the newly entered string value</returns>
    static member StringBox(        [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(null:string)>]defaultValValue:string,
                                    [<OPT;DEF(null:string)>]title:string) : string option =
        let getKeepEditor () = 
            let rc, text = UI.Dialogs.ShowEditBox(title, message, defaultValValue, false)
            if rc then Some text else None
        Synchronisation.DoSync false false getKeepEditor


    [<Extension>]
    ///<summary>Display a text dialog box similar to the one used by the _What command</summary>
    ///<param name="message">(string) The message</param>
    ///<param name="title">(string) Optional, The message title</param>
    ///<returns>(unit) void, nothing</returns>
    static member TextOut(message:string,
                          [<OPT;DEF(null:string)>]title:string) : unit =
        let getKeepEditor () = 
            UI.Dialogs.ShowTextDialog(message, title)
        Synchronisation.DoSync false false getKeepEditor


