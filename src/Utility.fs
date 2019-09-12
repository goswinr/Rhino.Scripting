namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsUtility =
  type RhinoScriptSyntax with
    
    ///<summary>Return True if the script is being executed in the context of Rhino</summary>
    ///<returns>(bool) True if the script is being executed in the context of Rhino</returns>
    static member ContextIsRhino() : bool =
        failNotImpl () // done in 2018


    ///<summary>Return True if the script is being executed in a grasshopper component</summary>
    ///<returns>(bool) True if the script is being executed in a grasshopper component</returns>
    static member ContextIsGrasshopper() : bool =
        failNotImpl () // done in 2018


    ///<summary>Measures the angle between two points</summary>
    ///<param name="point1">(Point3d) Point1 of 'the input points' (FIXME 0)</param>
    ///<param name="point2">(Point3d) Point2 of 'the input points' (FIXME 0)</param>
    ///<param name="plane">(bool) Optional, Default Value: <c>true</c>
    ///Boolean or Plane
    ///  If True, angle calculation is based on the world coordinate system.
    ///  If False, angle calculation is based on the active construction plane
    ///  If a plane is provided, angle calculation is with respect to this plane</param>
    ///<returns>(float * float * float * float * float * float) containing the following elements
    ///  element 0 = the X,Y angle in degrees
    ///  element 1 = the elevation
    ///  element 2 = delta in the X direction
    ///  element 3 = delta in the Y direction
    ///  element 4 = delta in the Z direction</returns>
    static member Angle(point1:Point3d, point2:Point3d, [<OPT;DEF(true)>]plane:bool) : float * float * float * float * float * float =
        failNotImpl () // done in 2018


    ///<summary>Measures the angle between two lines</summary>
    ///<param name="line1">(Line) List of 6 numbers or 2 Point3d.</param>
    ///<param name="line2">(Line) List of 6 numbers or 2 Point3d.</param>
    ///<returns>(float * float) containing the following elements .
    ///  0 The angle in degrees.
    ///  1 The reflex angle in degrees.</returns>
    static member Angle2(line1:Line, line2:Line) : float * float =
        failNotImpl () // done in 2018


    ///<summary>Returns a text string to the Windows clipboard</summary>
    ///<returns>(string) The current text in the clipboard</returns>
    static member ClipboardText() : string =
        failNotImpl () // done in 2018

    ///<summary>Sets a text string to the Windows clipboard</summary>
    ///<param name="text">(string)Text to set</param>
    ///<returns>(unit) unit</returns>
    static member ClipboardText(text:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Changes the luminance of a red-green-blue value. Hue and saturation are
    ///  not affected</summary>
    ///<param name="rgb">(Drawing.Color) Initial rgb value</param>
    ///<param name="luma">(float) The luminance in units of 0.1 percent of the total range. A
    ///  value of luma = 50 corresponds to 5 percent of the maximum luminance</param>
    ///<param name="scale">(bool) Optional, Default Value: <c>false</c>
    ///If True, luma specifies how much to increment or decrement the
    ///  current luminance. If False, luma specified the absolute luminance.</param>
    ///<returns>(Drawing.Color) modified rgb value</returns>
    static member ColorAdjustLuma(rgb:Drawing.Color, luma:float, [<OPT;DEF(false)>]scale:bool) : Drawing.Color =
        failNotImpl () // done in 2018


    ///<summary>Retrieves intensity value for the blue component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(float) The blue component , otherwise None</returns>
    static member ColorBlueValue(rgb:Drawing.Color) : float =
        failNotImpl () // done in 2018


    ///<summary>Retrieves intensity value for the green component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(float) The green component , otherwise None</returns>
    static member ColorGreenValue(rgb:Drawing.Color) : float =
        failNotImpl () // done in 2018


    ///<summary>Converts colors from hue-lumanence-saturation to RGB</summary>
    ///<param name="hls">(Drawing.Color) The HLS color value</param>
    ///<returns>(Drawing.Color) The RGB color value , otherwise False</returns>
    static member ColorHLSToRGB(hls:Drawing.Color) : Drawing.Color =
        failNotImpl () // done in 2018


    ///<summary>Retrieves intensity value for the red component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(Drawing.Color) The red color value , otherwise False</returns>
    static member ColorRedValue(rgb:Drawing.Color) : Drawing.Color =
        failNotImpl () // done in 2018


    ///<summary>Convert colors from RGB to HLS</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(Drawing.Color) The HLS color value , otherwise False</returns>
    static member ColorRGBToHLS(rgb:Drawing.Color) : Drawing.Color =
        failNotImpl () // done in 2018


    ///<summary>Removes duplicates from an array of numbers.</summary>
    ///<param name="numbers">(float seq) List or tuple</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///The minimum distance between numbers.  Numbers that fall within this tolerance will be discarded.  If omitted, Rhino's internal zero tolerance is used.</param>
    ///<returns>(float seq) numbers with duplicates removed .</returns>
    static member CullDuplicateNumbers(numbers:float seq, [<OPT;DEF(null)>]tolerance:float) : float seq =
        failNotImpl () // done in 2018


    ///<summary>Removes duplicates from a list of 3D points.</summary>
    ///<param name="points">(Point3d seq) A list of 3D points.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>-1</c>
    ///Minimum distance between points. Points within this
    ///  tolerance will be discarded. If omitted, Rhino's internal zero tolerance
    ///  is used.</param>
    ///<returns>(Point3d seq) of 3D points with duplicates removed .</returns>
    static member CullDuplicatePoints(points:Point3d seq, [<OPT;DEF(-1)>]tolerance:float) : Point3d seq =
        failNotImpl () // done in 2018


    //(FIXME) VarOutTypes
    ///<summary>Measures distance between two 3D points, or between a 3D point and
    ///  an array of 3D points.</summary>
    ///<param name="point1">(Point3d) The first 3D point.</param>
    ///<param name="point2">(Point3d) The second 3D point or list of 3-D points.</param>
    ///<returns>(Point3d) If point2 is a 3D point then the distance .</returns>
    static member Distance(point1:Point3d, point2:Point3d) : Point3d =
        failNotImpl () // done in 2018


    //(FIXME) VarOutTypes
    ///<summary>Returns string from a specified section in a initialization file.</summary>
    ///<param name="filename">(string) Name of the initialization file</param>
    ///<param name="section">(string) Optional, Default Value: <c>null</c>
    ///Section containing the entry</param>
    ///<param name="entry">(string) Optional, Default Value: <c>null</c>
    ///Entry whose associated string is to be returned</param>
    ///<returns>(string seq) A list containing all section names</returns>
    static member GetSettings(filename:string, [<OPT;DEF(null)>]section:string, [<OPT;DEF(null)>]entry:string) : string seq =
        failNotImpl () // done in 2018


    ///<summary>Returns 3D point that is a specified angle and distance from a 3D point</summary>
    ///<param name="point">(Point3d) The point to transform</param>
    ///<param name="angleDegrees">(int) Angle in degrees</param>
    ///<param name="distance">(float) Distance from point</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>null</c>
    ///Plane to base the transformation. If omitted, the world
    ///  x-y plane is used</param>
    ///<returns>(Point3d) resulting point is successful</returns>
    static member Polar(point:Point3d, angleDegrees:int, distance:float, [<OPT;DEF(null)>]plane:Plane) : Point3d =
        failNotImpl () // done in 2018


    ///<summary>Flattens an array of 3-D points into a one-dimensional list of real numbers. For example, if you had an array containing three 3-D points, this method would return a one-dimensional array containing nine real numbers.</summary>
    ///<param name="points">(Point3d seq) Points to flatten</param>
    ///<returns>(float seq) A one-dimensional list containing real numbers, , otherwise None</returns>
    static member SimplifyArray(points:Point3d seq) : float seq =
        failNotImpl () // done in 2018


    ///<summary>Suspends execution of a running script for the specified interval</summary>
    ///<param name="milliseconds">(float) Thousands of a second</param>
    ///<returns>(unit) </returns>
    static member Sleep(milliseconds:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Sorts list of points so they will be connected in a "reasonable" polyline order</summary>
    ///<param name="points">(Point3d seq) The points to sort</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///Minimum distance between points. Points that fall within this tolerance
    ///  will be discarded. If omitted, Rhino's internal zero tolerance is used.</param>
    ///<returns>(Point3d seq) of sorted 3D points</returns>
    static member SortPointList(points:Point3d seq, [<OPT;DEF(null)>]tolerance:float) : Point3d seq =
        failNotImpl () // done in 2018


    ///<summary>Sorts the components of an array of 3D points</summary>
    ///<param name="points">(Point3d seq) Points to sort</param>
    ///<param name="ascendeing">(bool) Optional, Default Value: <c>true</c>
    ///Ascendeing if omitted (True) or True, descending if False.</param>
    ///<param name="order">(float) Optional, Default Value: <c>0</c>
    ///The component sort order
    ///  Value       Component Sort Order
    ///  0 (default) X, Y, Z
    ///  1           X, Z, Y
    ///  2           Y, X, Z
    ///  3           Y, Z, X
    ///  4           Z, X, Y
    ///  5           Z, Y, X</param>
    ///<returns>(Point3d seq) sorted 3-D points</returns>
    static member SortPoints(points:Point3d seq, [<OPT;DEF(true)>]ascendeing:bool, [<OPT;DEF(0)>]order:float) : Point3d seq =
        failNotImpl () // done in 2018


    ///<summary>convert a formatted string value into a 3D point value</summary>
    ///<param name="point">(string) A string that contains a delimited point like "1,2,3".</param>
    ///<returns>(Point3d) Point structure from the input string.</returns>
    static member Str2Pt(point:string) : Point3d =
        failNotImpl () // done in 2018


    ///<summary>Converts 'point' into a Rhino.Geometry.Point3d if possible.
    ///  If the provided object is already a point, it value is copied.
    ///  In case the conversion fails, an error is raised.
    ///  Alternatively, you can also pass two coordinates singularly for a
    ///  point on the XY plane, or three for a 3D point.</summary>
    ///<param name="point">('T * float * float) </param>
    ///<param name="y">(float) Optional, Default Value: <c>null</c>
    ///Y position</param>
    ///<param name="z">(float) Optional, Default Value: <c>null</c>
    ///Z position</param>
    ///<returns>(Point3d) a Rhino.Geometry.Point3d. This can be seen as an object with three indices:
    ///  [0]  X coordinate
    ///  [1]  Y coordinate
    ///  [2]  Z coordinate.</returns>
    static member CreatePoint(point:'T * float * float, [<OPT;DEF(null)>]y:float, [<OPT;DEF(null)>]z:float) : Point3d =
        failNotImpl () // done in 2018


    ///<summary>Converts 'vector' into a Rhino.Geometry.Vector3d if possible.
    ///  If the provided object is already a vector, it value is copied.
    ///  If the conversion fails, an error is raised.
    ///  Alternatively, you can also pass two coordinates singularly for a
    ///  vector on the XY plane, or three for a 3D vector.</summary>
    ///<param name="vector">('T * float * float) </param>
    ///<param name="y">(float) Optional, Default Value: <c>null</c>
    ///Y position</param>
    ///<param name="z">(float) Optional, Default Value: <c>null</c>
    ///Z position</param>
    ///<returns>(Vector3d) a Rhino.Geometry.Vector3d. This can be seen as an object with three indices:
    ///  result[0]: X component, result[1]: Y component, and result[2] Z component.</returns>
    static member CreateVector(vector:'T * float * float, [<OPT;DEF(null)>]y:float, [<OPT;DEF(null)>]z:float) : Vector3d =
        failNotImpl () // done in 2018


    ///<summary>Converts input into a Rhino.Geometry.Plane object if possible.
    ///  If the provided object is already a plane, its value is copied.
    ///  The returned data is accessible by indexing[origin, X axis, Y axis, Z axis], and that is the suggested method to interact with the type.
    ///  The Z axis is in any case computed from the X and Y axes, so providing it is possible but not required.
    ///  If the conversion fails, an error is raised.</summary>
    ///<param name="planeOrOrigin">(Point3d * Vector3d * Vector3d) </param>
    ///<param name="xAxis">(Vector3d) Optional, Default Value: <c>null</c>
    ///Direction of X-Axis</param>
    ///<param name="yAxis">(Vector3d) Optional, Default Value: <c>null</c>
    ///Direction of Y-Axis</param>
    ///<returns>(Plane) A Rhino.Geometry.Plane</returns>
    static member CreatePlane(planeOrOrigin:Point3d * Vector3d * Vector3d, [<OPT;DEF(null)>]xAxis:Vector3d, [<OPT;DEF(null)>]yAxis:Vector3d) : Plane =
        failNotImpl () // done in 2018


    ///<summary>Converts input into a Rhino.Geometry.Transform object if possible.
    ///  If the provided object is already a transform, its value is copied.
    ///  The returned data is accessible by indexing[row, column], and that is the suggested method to interact with the type.
    ///  If the conversion fails, an error is raised.</summary>
    ///<param name="xform">(seq<seq<float>>) The transform. This can be seen as a 4x4 matrix, given as nested lists or tuples.</param>
    ///<returns>(Transform) A Rhino.Geometry.Transform. result[0,3] gives access to the first row, last column.</returns>
    static member CreateXform(xform:seq<seq<float>>) : Transform =
        failNotImpl () // done in 2018


    ///<summary>Converts 'color' into a native color object if possible.
    ///  The returned data is accessible by indexing, and that is the suggested method to interact with the type.
    ///  Red index is [0], Green index is [1], Blue index is [2] and Alpha index is [3].
    ///  If the provided object is already a color, its value is copied.
    ///  Alternatively, you can also pass three coordinates singularly for an RGB color, or four
    ///  for an RGBA color point.</summary>
    ///<param name="color">(float*float*float) List or 3 or 4 items. Also, a single int can be passed and it will be bitwise-parsed.</param>
    ///<param name="g">(int) Optional, Default Value: <c>null</c>
    ///Green value</param>
    ///<param name="b">(int) Optional, Default Value: <c>null</c>
    ///Blue value</param>
    ///<param name="a">(int) Optional, Default Value: <c>null</c>
    ///Alpha value</param>
    ///<returns>(Drawing.Color) An object that can be indexed for red, green, blu, alpha. Item[0] is red.</returns>
    static member CreateColor(color:float*float*float, [<OPT;DEF(null)>]g:int, [<OPT;DEF(null)>]b:int, [<OPT;DEF(null)>]a:int) : Drawing.Color =
        failNotImpl () // done in 2018


    ///<summary>Converts 'interval' into a Rhino.Geometry.Interval.
    ///  If the provided object is already an interval, its value is copied.
    ///  In case the conversion fails, an error is raised.
    ///  In case a single number is provided, it will be translated to an increasing interval that includes
    ///  the provided input and 0. If two values are provided, they will be used instead.</summary>
    ///<param name="interval">(float * float) Or any item that can be accessed at index 0 and 1; an Interval or just the lower bound</param>
    ///<param name="y">(float) Optional, Default Value: <c>null</c>
    ///Uper bound of interval</param>
    ///<returns>(Rhino.Geometry.Interval) This can be seen as an object made of two items:
    ///  [0] start of interval
    ///  [1] end of interval</returns>
    static member CreateInterval(interval:float * float, [<OPT;DEF(null)>]y:float) : Rhino.Geometry.Interval =
        failNotImpl () // done in 2018


