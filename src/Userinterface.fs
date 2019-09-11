namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsUserinterface =
  type RhinoScriptSyntax with
    ///<summary>Display browse-for-folder dialog allowing the user to select a folder</summary>
    ///<param name="folder">(string) Optional, Default Value: <c>null</c>
    ///A default folder</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title</param>
    ///<returns>(string) selected folder</returns>
    static member BrowseForFolder([<OPT;DEF(null)>]folder:string, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string =
        failNotImpl()

    ///<summary>Displays a list of items in a checkable-style list dialog box</summary>
    ///<param name="items">((string*bool) seq) A list of tuples containing a string and a boolean check state</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title</param>
    ///<returns>(string seq) of tuples containing the input string in items along with their new boolean check value</returns>
    static member CheckListBox(items:(string*bool) seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string seq =
        failNotImpl()

    ///<summary>Displays a list of items in a combo-style list box dialog.</summary>
    ///<param name="items">(string seq) A list of string</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt of message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title</param>
    ///<returns>(string) The selected item</returns>
    static member ComboListBox(items:string seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string =
        failNotImpl()

    ///<summary>Display dialog prompting the user to enter a string. The
    ///  string value may span multiple lines</summary>
    ///<param name="defaultValString">(string) Optional, Default Value: <c>null</c>
    ///A default string value.</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt message.</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title.</param>
    ///<returns>(string) Multiple lines that are separated by carriage return-linefeed combinations</returns>
    static member EditBox([<OPT;DEF(null)>]defaultValString:string, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string =
        failNotImpl()

    ///<summary>Pause for user input of an angle</summary>
    ///<param name="point">(Point3d) Optional, Default Value: <c>null</c>
    ///Starting, or base point</param>
    ///<param name="referencePoint">(Point3d) Optional, Default Value: <c>null</c>
    ///If specified, the reference angle is calculated
    ///  from it and the base point</param>
    ///<param name="defaultValAngleDegrees">(int) Optional, Default Value: <c>0</c>
    ///A default angle value specified</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt to display</param>
    ///<returns>(float) angle in degree</returns>
    static member GetAngle([<OPT;DEF(null)>]point:Point3d, [<OPT;DEF(null)>]referencePoint:Point3d, [<OPT;DEF(0)>]defaultValAngleDegrees:int, [<OPT;DEF(null)>]message:string) : float =
        failNotImpl()

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
        failNotImpl()

    ///<summary>Pauses for user input of a box</summary>
    ///<param name="mode">(float) Optional, Default Value: <c>0</c>
    ///The box selection mode.
    ///  0 = All modes
    ///  1 = Corner. The base rectangle is created by picking two corner points
    ///  2 = 3-Point. The base rectangle is created by picking three points
    ///  3 = Vertical. The base vertical rectangle is created by picking three points.
    ///  4 = Center. The base rectangle is created by picking a center point and a corner point</param>
    ///<param name="basisPoint">(Point3d) Optional, Default Value: <c>null</c>
    ///Optional 3D base point</param>
    ///<param name="prompt1">(string) Optional, Default Value: <c>null</c>
    ///Prompt1 of 'optional prompts to set' (FIXME 0)</param>
    ///<param name="prompt2">(string) Optional, Default Value: <c>null</c>
    ///Prompt2 of 'optional prompts to set' (FIXME 0)</param>
    ///<param name="prompt3">(string) Optional, Default Value: <c>null</c>
    ///Prompt3 of 'optional prompts to set' (FIXME 0)</param>
    ///<returns>(Point3d seq) list of eight Point3d that define the corners of the box on success</returns>
    static member GetBox([<OPT;DEF(0)>]mode:float, [<OPT;DEF(null)>]basisPoint:Point3d, [<OPT;DEF(null)>]prompt1:string, [<OPT;DEF(null)>]prompt2:string, [<OPT;DEF(null)>]prompt3:string) : Point3d seq =
        failNotImpl()

    ///<summary>Display the Rhino color picker dialog allowing the user to select an RGB color</summary>
    ///<param name="color">(Drawing.Color) Optional, Default Value: <c>null</c>
    ///Default RGB value. If omitted, the default color is black</param>
    ///<returns>(Drawing.Color) RGB tuple of three numbers on success</returns>
    static member GetColor([<OPT;DEF(null)>]color:Drawing.Color) : Drawing.Color =
        failNotImpl()

    ///<summary>Retrieves the cursor's position</summary>
    ///<returns>(Point3d * Point3d * Guid * Point3d) containing the following information
    ///  0  cursor position in world coordinates
    ///  1  cursor position in screen coordinates
    ///  2  id of the active viewport
    ///  3  cursor position in client coordinates</returns>
    static member GetCursorPos() : Point3d * Point3d * Guid * Point3d =
        failNotImpl()

    ///<summary>Pauses for user input of a distance.</summary>
    ///<param name="firstPt">(Point3d) Optional, Default Value: <c>null</c>
    ///First distance point</param>
    ///<param name="distance">(float) Optional, Default Value: <c>null</c>
    ///Default distance</param>
    ///<param name="firstPtMsg">(string) Optional, Default Value: <c>"First distance point"</c>
    ///Prompt for the first distance point</param>
    ///<param name="secondPtMsg">(string) Optional, Default Value: <c>"Second distance point"</c>
    ///Prompt for the second distance point</param>
    ///<returns>(float) The distance between the two points .</returns>
    static member GetDistance([<OPT;DEF(null)>]firstPt:Point3d, [<OPT;DEF(null)>]distance:float, [<OPT;DEF("First distance point")>]firstPtMsg:string, [<OPT;DEF("Second distance point")>]secondPtMsg:string) : float =
        failNotImpl()

    ///<summary>Prompt the user to pick one or more surface or polysurface edge curves</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message.</param>
    ///<param name="minCount">(int) Optional, Default Value: <c>1</c>
    ///Minimum number of edges to select.</param>
    ///<param name="maxCount">(int) Optional, Default Value: <c>0</c>
    ///Maximum number of edges to select.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the duplicated edge curves.</param>
    ///<returns>(Guid seq) of selection prompts (curve id, parent id, selection point)</returns>
    static member GetEdgeCurves([<OPT;DEF(null)>]message:string, [<OPT;DEF(1)>]minCount:int, [<OPT;DEF(0)>]maxCount:int, [<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl()

    ///<summary>Pauses for user input of a whole number.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message.</param>
    ///<param name="number">(float) Optional, Default Value: <c>null</c>
    ///A default whole number value.</param>
    ///<param name="minimum">(float) Optional, Default Value: <c>null</c>
    ///A minimum allowable value.</param>
    ///<param name="maximum">(float) Optional, Default Value: <c>null</c>
    ///A maximum allowable value.</param>
    ///<returns>(float) The whole number input by the user .</returns>
    static member GetInteger([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]number:float, [<OPT;DEF(null)>]minimum:float, [<OPT;DEF(null)>]maximum:float) : float =
        failNotImpl()

    ///<summary>Displays dialog box prompting the user to select a layer</summary>
    ///<param name="title">(string) Optional, Default Value: <c>"Select Layer"</c>
    ///Dialog box title</param>
    ///<param name="layer">(string) Optional, Default Value: <c>null</c>
    ///Name of a layer to preselect. If omitted, the current layer will be preselected</param>
    ///<param name="showNewButton">(bool) Optional, Default Value: <c>false</c>
    ///Show new button of 'Optional buttons to show on the dialog' (FIXME 0)</param>
    ///<param name="showSetCurrent">(bool) Optional, Default Value: <c>false</c>
    ///Show set current of 'Optional buttons to show on the dialog' (FIXME 0)</param>
    ///<returns>(string) name of selected layer</returns>
    static member GetLayer([<OPT;DEF("Select Layer")>]title:string, [<OPT;DEF(null)>]layer:string, [<OPT;DEF(false)>]showNewButton:bool, [<OPT;DEF(false)>]showSetCurrent:bool) : string =
        failNotImpl()

    ///<summary>Displays a dialog box prompting the user to select one or more layers</summary>
    ///<param name="title">(string) Optional, Default Value: <c>"Select Layers"</c>
    ///Dialog box title</param>
    ///<param name="showNewButton">(bool) Optional, Default Value: <c>false</c>
    ///Optional button to show on the dialog</param>
    ///<returns>(string) The names of selected layers</returns>
    static member GetLayers([<OPT;DEF("Select Layers")>]title:string, [<OPT;DEF(false)>]showNewButton:bool) : string =
        failNotImpl()

    ///<summary>Prompts the user to pick points that define a line</summary>
    ///<param name="mode">(float) Optional, Default Value: <c>0</c>
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
    ///<param name="point">(Point3d) Optional, Default Value: <c>null</c>
    ///Optional starting point</param>
    ///<param name="message1">(string) Optional, Default Value: <c>null</c>
    ///Message1 of 'optional prompts' (FIXME 0)</param>
    ///<param name="message2">(string) Optional, Default Value: <c>null</c>
    ///Message2 of 'optional prompts' (FIXME 0)</param>
    ///<param name="message3">(string) Optional, Default Value: <c>null</c>
    ///Message3 of 'optional prompts' (FIXME 0)</param>
    ///<returns>(Line) Tuple of two points on success</returns>
    static member GetLine([<OPT;DEF(0)>]mode:float, [<OPT;DEF(null)>]point:Point3d, [<OPT;DEF(null)>]message1:string, [<OPT;DEF(null)>]message2:string, [<OPT;DEF(null)>]message3:string) : Line =
        failNotImpl()

    ///<summary>Displays a dialog box prompting the user to select one linetype</summary>
    ///<param name="defaultValLinetyp">(string) Optional, Default Value: <c>null</c>
    ///Optional. The name of the linetype to select. If omitted, the current linetype will be selected.</param>
    ///<param name="showByLayer">(bool) Optional, Default Value: <c>false</c>
    ///If True, the "by Layer" linetype will show. Defaults to False.</param>
    ///<returns>(string) The names of selected linetype</returns>
    static member GetLinetype([<OPT;DEF(null)>]defaultValLinetyp:string, [<OPT;DEF(false)>]showByLayer:bool) : string =
        failNotImpl()

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
        failNotImpl()

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
        failNotImpl()

    ///<summary>Pauses for user input of a point.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message.</param>
    ///<param name="basisPoint">(Point3d) Optional, Default Value: <c>null</c>
    ///List of 3 numbers or Point3d identifying a starting, or base point</param>
    ///<param name="distance">(float) Optional, Default Value: <c>null</c>
    ///Constraining distance. If distance is specified, basePoint must also be specified.</param>
    ///<param name="inPlane">(bool) Optional, Default Value: <c>false</c>
    ///Constrains the point selections to the active construction plane.</param>
    ///<returns>(Point3d) point on success</returns>
    static member GetPoint([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]basisPoint:Point3d, [<OPT;DEF(null)>]distance:float, [<OPT;DEF(false)>]inPlane:bool) : Point3d =
        failNotImpl()

    ///<summary>Pauses for user input of a point constrainted to a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt of message</param>
    ///<returns>(Point3d) 3d point</returns>
    static member GetPointOnCurve(curveId:Guid, [<OPT;DEF(null)>]message:string) : Point3d =
        failNotImpl()

    ///<summary>Pauses for user input of a point constrained to a mesh object</summary>
    ///<param name="meshId">(Guid) Identifier of the mesh to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message</param>
    ///<returns>(Point3d) 3d point</returns>
    static member GetPointOnMesh(meshId:Guid, [<OPT;DEF(null)>]message:string) : Point3d =
        failNotImpl()

    ///<summary>Pauses for user input of a point constrained to a surface or polysurface
    ///  object</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to get a point on</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message</param>
    ///<returns>(Point3d) 3d point</returns>
    static member GetPointOnSurface(surfaceId:Guid, [<OPT;DEF(null)>]message:string) : Point3d =
        failNotImpl()

    ///<summary>Pauses for user input of one or more points</summary>
    ///<param name="drawLines">(bool) Optional, Default Value: <c>false</c>
    ///Draw lines between points</param>
    ///<param name="inPlane">(bool) Optional, Default Value: <c>false</c>
    ///Constrain point selection to the active construction plane</param>
    ///<param name="message1">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message for the first point</param>
    ///<param name="message2">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message for the next points</param>
    ///<param name="maxPoints">(float) Optional, Default Value: <c>null</c>
    ///Maximum number of points to pick. If not specified, an
    ///  unlimited number of points can be picked.</param>
    ///<param name="basisPoint">(Point3d) Optional, Default Value: <c>null</c>
    ///A starting or base point</param>
    ///<returns>(Point3d seq) of 3d points</returns>
    static member GetPoints([<OPT;DEF(false)>]drawLines:bool, [<OPT;DEF(false)>]inPlane:bool, [<OPT;DEF(null)>]message1:string, [<OPT;DEF(null)>]message2:string, [<OPT;DEF(null)>]maxPoints:float, [<OPT;DEF(null)>]basisPoint:Point3d) : Point3d seq =
        failNotImpl()

    ///<summary>Prompts the user to pick points that define a polyline.</summary>
    ///<param name="flags">(int) Optional, Default Value: <c>3</c>
    ///The options are bit coded flags. Values can be added together to specify more than one option. The default is 3.
    ///  value description
    ///  1     Permit close option. If specified, then after 3 points have been picked, the user can type "Close" and a closed polyline will be returned.
    ///  2     Permit close snap. If specified, then after 3 points have been picked, the user can pick near the start point and a closed polyline will be returned.
    ///  4     Force close. If specified, then the returned polyline is always closed. If specified, then intMax must be 0 or >= 4.
    ///  Note: the default is 3, or "Permit close option = True", "Permit close snap = True", and "Force close = False".</param>
    ///<param name="message1">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message for the first point.</param>
    ///<param name="message2">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message for the second point.</param>
    ///<param name="message3">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message for the third point.</param>
    ///<param name="message4">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message for the 'next' point.</param>
    ///<param name="min">(float) Optional, Default Value: <c>2</c>
    ///The minimum number of points to require. The default is 2.</param>
    ///<param name="max">(float) Optional, Default Value: <c>0</c>
    ///The maximum number of points to require; 0 for no limit.  The default is 0.</param>
    ///<returns>(Point3d seq) A list of 3-D points that define the polyline .</returns>
    static member GetPolyline([<OPT;DEF(3)>]flags:int, [<OPT;DEF(null)>]message1:string, [<OPT;DEF(null)>]message2:string, [<OPT;DEF(null)>]message3:string, [<OPT;DEF(null)>]message4:string, [<OPT;DEF(2)>]min:float, [<OPT;DEF(0)>]max:float) : Point3d seq =
        failNotImpl()

    ///<summary>Pauses for user input of a number.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Number"</c>
    ///A prompt or message.</param>
    ///<param name="number">(float) Optional, Default Value: <c>null</c>
    ///A default number value.</param>
    ///<param name="minimum">(float) Optional, Default Value: <c>null</c>
    ///A minimum allowable value.</param>
    ///<param name="maximum">(float) Optional, Default Value: <c>null</c>
    ///A maximum allowable value.</param>
    ///<returns>(float) The number input by the user .</returns>
    static member GetReal([<OPT;DEF("Number")>]message:string, [<OPT;DEF(null)>]number:float, [<OPT;DEF(null)>]minimum:float, [<OPT;DEF(null)>]maximum:float) : float =
        failNotImpl()

    ///<summary>Pauses for user input of a rectangle</summary>
    ///<param name="mode">(float) Optional, Default Value: <c>0</c>
    ///The rectangle selection mode. The modes are as follows
    ///  0 = All modes
    ///  1 = Corner - a rectangle is created by picking two corner points
    ///  2 = 3Point - a rectangle is created by picking three points
    ///  3 = Vertical - a vertical rectangle is created by picking three points
    ///  4 = Center - a rectangle is created by picking a center point and a corner point</param>
    ///<param name="basisPoint">(Point3d) Optional, Default Value: <c>null</c>
    ///A 3d base point</param>
    ///<param name="prompt1">(string) Optional, Default Value: <c>null</c>
    ///Prompt1 of 'optional prompts' (FIXME 0)</param>
    ///<param name="prompt2">(string) Optional, Default Value: <c>null</c>
    ///Prompt2 of 'optional prompts' (FIXME 0)</param>
    ///<param name="prompt3">(string) Optional, Default Value: <c>null</c>
    ///Prompt3 of 'optional prompts' (FIXME 0)</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) four 3d points that define the corners of the rectangle</returns>
    static member GetRectangle([<OPT;DEF(0)>]mode:float, [<OPT;DEF(null)>]basisPoint:Point3d, [<OPT;DEF(null)>]prompt1:string, [<OPT;DEF(null)>]prompt2:string, [<OPT;DEF(null)>]prompt3:string) : Point3d * Point3d * Point3d * Point3d =
        failNotImpl()

    ///<summary>Pauses for user input of a string value</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message</param>
    ///<param name="defaultValString">(string) Optional, Default Value: <c>null</c>
    ///A default value</param>
    ///<param name="strings">(string seq) Optional, Default Value: <c>null</c>
    ///List of strings to be displayed as a click-able command options.
    ///  Note, strings cannot begin with a numeric character</param>
    ///<returns>(string) The string either input or selected by the user .
    ///  If the user presses the Enter key without typing in a string, an empty string "" is returned.</returns>
    static member GetString([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]defaultValString:string, [<OPT;DEF(null)>]strings:string seq) : string =
        failNotImpl()

    ///<summary>Display a list of items in a list box dialog.</summary>
    ///<param name="items">(string seq) A list of values to select</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt of message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title</param>
    ///<param name="defaultVal">(string) Optional, Default Value: <c>null</c>
    ///Selected item in the list</param>
    ///<returns>(string) he selected item</returns>
    static member ListBox(items:string seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]defaultVal:string) : string =
        failNotImpl()

    ///<summary>Displays a message box. A message box contains a message and
    ///  title, plus any combination of predefined icons and push buttons.</summary>
    ///<param name="message">(string) A prompt or message.</param>
    ///<param name="buttons">(float) Optional, Default Value: <c>0</c>
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
    static member MessageBox(message:string, [<OPT;DEF(0)>]buttons:float, [<OPT;DEF("")>]title:string) : float =
        failNotImpl()

    ///<summary>Displays list of items and their values in a property-style list box dialog</summary>
    ///<param name="items">(string seq) Items of 'list of string items and their corresponding values' (FIXME 0)</param>
    ///<param name="values">(string seq) Values of 'list of string items and their corresponding values' (FIXME 0)</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title</param>
    ///<returns>(string seq) of new values on success</returns>
    static member PropertyListBox(items:string seq, values:string seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : string seq =
        failNotImpl()

    ///<summary>Displays a list of items in a multiple-selection list box dialog</summary>
    ///<param name="items">(string seq) A zero-based list of string items</param>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt or message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title</param>
    ///<param name="defaultVals">(string seq) Optional, Default Value: <c>null</c>
    ///Either a string representing the pre-selected item in the list
    ///  or a list if multiple items are pre-selected</param>
    ///<returns>(string seq) containing the selected items</returns>
    static member MultiListBox(items:string seq, [<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]defaultVals:string seq) : string seq =
        failNotImpl()

    ///<summary>Displays file open dialog box allowing the user to enter a file name.
    ///  Note, this function does not open the file.</summary>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title.</param>
    ///<param name="filter">(string) Optional, Default Value: <c>null</c>
    ///A filter string. The filter must be in the following form:
    ///  "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///  If omitted, the filter (*.*) is used.</param>
    ///<param name="folder">(string) Optional, Default Value: <c>null</c>
    ///A default folder.</param>
    ///<param name="filename">(string) Optional, Default Value: <c>null</c>
    ///A default file name</param>
    ///<param name="extension">(string) Optional, Default Value: <c>null</c>
    ///A default file extension</param>
    ///<returns>(string) file name is successful</returns>
    static member OpenFileName([<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]filter:string, [<OPT;DEF(null)>]folder:string, [<OPT;DEF(null)>]filename:string, [<OPT;DEF(null)>]extension:string) : string =
        failNotImpl()

    ///<summary>Displays file open dialog box allowing the user to select one or more file names.
    ///  Note, this function does not open the file.</summary>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title.</param>
    ///<param name="filter">(string) Optional, Default Value: <c>null</c>
    ///A filter string. The filter must be in the following form:
    ///  "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///  If omitted, the filter (*.*) is used.</param>
    ///<param name="folder">(string) Optional, Default Value: <c>null</c>
    ///A default folder.</param>
    ///<param name="filename">(string) Optional, Default Value: <c>null</c>
    ///A default file name</param>
    ///<param name="extension">(string) Optional, Default Value: <c>null</c>
    ///A default file extension</param>
    ///<returns>(string seq) of selected file names</returns>
    static member OpenFileNames([<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]filter:string, [<OPT;DEF(null)>]folder:string, [<OPT;DEF(null)>]filename:string, [<OPT;DEF(null)>]extension:string) : string seq =
        failNotImpl()

    ///<summary>Display a context-style popup menu. The popup menu can appear almost
    ///  anywhere, and can be dismissed by clicking the left or right mouse buttons</summary>
    ///<param name="items">(string seq) List of strings representing the menu items. An empty string or None
    ///  will create a separator</param>
    ///<param name="modes">(float seq) Optional, Default Value: <c>null</c>
    ///List of numbers identifying the display modes. If omitted, all
    ///  modes are enabled.
    ///    0 = menu item is enabled
    ///    1 = menu item is disabled
    ///    2 = menu item is checked
    ///    3 = menu item is disabled and checked</param>
    ///<param name="point">(Point3d) Optional, Default Value: <c>null</c>
    ///A 3D point where the menu item will appear. If omitted, the menu
    ///  will appear at the current cursor position</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///If point is specified, the view in which the point is computed.
    ///  If omitted, the active view is used</param>
    ///<returns>(float) index of the menu item picked or -1 if no menu item was picked</returns>
    static member PopupMenu(items:string seq, [<OPT;DEF(null)>]modes:float seq, [<OPT;DEF(null)>]point:Point3d, [<OPT;DEF(null)>]view:string) : float =
        failNotImpl()

    ///<summary>Display a dialog box prompting the user to enter a number</summary>
    ///<param name="message">(string) Optional, Default Value: <c>""</c>
    ///A prompt message.</param>
    ///<param name="defaultValNumber">(float) Optional, Default Value: <c>null</c>
    ///A default number.</param>
    ///<param name="title">(string) Optional, Default Value: <c>""</c>
    ///A dialog box title.</param>
    ///<param name="minimum">(float) Optional, Default Value: <c>null</c>
    ///A minimum allowable value.</param>
    ///<param name="maximum">(float) Optional, Default Value: <c>null</c>
    ///A maximum allowable value.</param>
    ///<returns>(float) The newly entered number on success</returns>
    static member RealBox([<OPT;DEF("")>]message:string, [<OPT;DEF(null)>]defaultValNumber:float, [<OPT;DEF("")>]title:string, [<OPT;DEF(null)>]minimum:float, [<OPT;DEF(null)>]maximum:float) : float =
        failNotImpl()

    ///<summary>Display a save dialog box allowing the user to enter a file name.
    ///  Note, this function does not save the file.</summary>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title.</param>
    ///<param name="filter">(string) Optional, Default Value: <c>null</c>
    ///A filter string. The filter must be in the following form:
    ///  "Description1|Filter1|Description2|Filter2||", where "||" terminates filter string.
    ///  If omitted, the filter (*.*) is used.</param>
    ///<param name="folder">(string) Optional, Default Value: <c>null</c>
    ///A default folder.</param>
    ///<param name="filename">(string) Optional, Default Value: <c>null</c>
    ///A default file name</param>
    ///<param name="extension">(string) Optional, Default Value: <c>null</c>
    ///A default file extension</param>
    ///<returns>(string) the file name is successful</returns>
    static member SaveFileName([<OPT;DEF(null)>]title:string, [<OPT;DEF(null)>]filter:string, [<OPT;DEF(null)>]folder:string, [<OPT;DEF(null)>]filename:string, [<OPT;DEF(null)>]extension:string) : string =
        failNotImpl()

    ///<summary>Display a dialog box prompting the user to enter a string value.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A prompt message</param>
    ///<param name="defaultValValue">(string) Optional, Default Value: <c>null</c>
    ///A default string value</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///A dialog box title</param>
    ///<returns>(string) the newly entered string value</returns>
    static member StringBox([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]defaultValValue:string, [<OPT;DEF(null)>]title:string) : string =
        failNotImpl()

    ///<summary>Display a text dialog box similar to the one used by the _What command.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null</c>
    ///A message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null</c>
    ///The message title</param>
    ///<returns>(unit) in any case</returns>
    static member TextOut([<OPT;DEF(null)>]message:string, [<OPT;DEF(null)>]title:string) : unit =
        failNotImpl()

