namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsUtility =

    type RhinoScriptSyntax with

    [<EXT>]
    ///<summary>Return True if the script is being executed in the context of Rhino(currently always true)</summary>
    ///<returns>(bool) True if the script is being executed in the context of Rhino(currently always true)</returns>
    static member ContextIsRhino() : bool =
        true //TODO implement correctly


    [<EXT>]
    ///<summary>Return True if the script is being executed in a grasshopper component(currently always true)</summary>
    ///<returns>(bool) True if the script is being executed in a grasshopper component(currently always true)</returns>
    static member ContextIsGrasshopper() : bool =
        true //TODO implement correctly


    [<EXT>]
    ///<summary>Measures the angle between two points</summary>
    ///<param name="point1">(Point3d) Point1 of input points</param>
    ///<param name="point2">(Point3d) Point2 of input points</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane.WorldX</c>
    ///  If a plane is provided, angle calculation is with respect to this plane</param>
    ///<returns>(float * float * float * float * float * float) containing the following elements:
    ///  element 0 = the X,Y angle in degrees
    ///  element 1 = the elevation
    ///  element 2 = delta in the X direction
    ///  element 3 = delta in the Y direction
    ///  element 4 = delta in the Z direction</returns>
    static member Angle(point1:Point3d, point2:Point3d, [<OPT;DEF(Plane())>]plane:Plane) : float * float * float * float * float  =
        let plane = if plane.IsValid then plane else Plane.WorldXY
        let vector = point2 - point1
        let mutable x = vector.X
        let mutable y = vector.Y
        let mutable z = vector.Z
        let vfrom = point1 - plane.Origin
        let vto = point2 - plane.Origin
        x <- vto * plane.XAxis - vfrom * plane.XAxis
        y <- vto * plane.YAxis - vfrom * plane.YAxis
        z <- vto * plane.ZAxis - vfrom * plane.ZAxis
        let h = Math.Sqrt( x * x + y * y)
        let angleXY = toDegrees( Math.Atan2( y, x ) )
        let elevation = toDegrees( Math.Atan2( z, h ) )
        angleXY, elevation, x, y, z


    [<EXT>]
    ///<summary>Measures the angle between two lines</summary>
    ///<param name="line1">(Line) List of 6 numbers or 2 Point3d.</param>
    ///<param name="line2">(Line) List of 6 numbers or 2 Point3d.</param>
    ///<returns>(float * float) containing the following elements .
    ///  0 The angle in degrees.
    ///  1 The reflex angle in degrees.</returns>
    static member Angle2(line1:Line, line2:Line) : float * float =
        let vec0 = line1.To - line1.From
        let vec1 = line2.To - line2.From
        if not <| vec0.Unitize() || not <| vec1.Unitize() then  failwithf "angle two failed on %A and %A" line1 line2
        let mutable dot = vec0 * vec1
        dot <- max -1.0 (min 1.0 dot) // clamp for math errors
        let mutable angle = Math.Acos(dot)
        let mutable reflexAngle = 2.0*Math.PI - angle
        angle <- toDegrees(angle)
        reflexAngle <- toDegrees(reflexAngle)
        angle, reflexAngle


    [<EXT>]
    ///<summary>Returns a text string to the Windows clipboard</summary>
    ///<returns>(string) The current text in the clipboard</returns>
    static member ClipboardText() : string = //GET
        if Windows.Forms.Clipboard.ContainsText() then Windows.Forms.Clipboard.GetText() else ""

    ///<summary>Sets a text string to the Windows clipboard</summary>
    ///<param name="text">(string)Text to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member ClipboardText(text:string) : unit = //SET
        System.Windows.Forms.Clipboard.SetText(text)


    [<EXT>]
    ///<summary>Changes the luminance of a red-green-blue value. Hue and saturation are
    ///  not affected</summary>
    ///<param name="rgb">(Drawing.Color) Initial rgb value</param>
    ///<param name="luma">(float) The luminance in units of 0.1 percent of the total range. A
    ///  value of luma = 50 corresponds to 5 percent of the maximum luminance</param>
    ///<param name="isScaleRelative">(bool) Optional, Default Value: <c>false</c>
    ///If True, luma specifies how much to increment or decrement the
    ///  current luminance. If False, luma specified the absolute luminance.</param>
    ///<returns>(Drawing.Color) modified rgb value</returns>
    static member ColorAdjustLuma(rgb:Drawing.Color, luma:float, [<OPT;DEF(false)>]isScaleRelative:bool) : Drawing.Color =
        let mutable hsl = Display.ColorHSL(rgb)
        let mutable luma = luma / 1000.0
        if isScaleRelative then luma <- hsl.L + luma
        hsl.L <- luma
        hsl.ToArgbColor()


    [<EXT>]
    ///<summary>Retrieves intensity value for the blue component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(int) The blue component </returns>
    static member ColorBlueValue(rgb:Drawing.Color) : int =
       int rgb.B


    [<EXT>]
    ///<summary>Retrieves intensity value for the green component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(int) The green component</returns>
    static member ColorGreenValue(rgb:Drawing.Color) : int =
       int rgb.G


    [<EXT>]
    ///<summary>Converts colors from hue-lumanence-saturation to RGB</summary>
    ///<param name="hls">(Drawing.Color) The HLS color value</param>
    ///<returns>(Drawing.Color) The RGB color value</returns>
    static member ColorHLSToRGB(hls:Drawing.Color) : Drawing.Color =
        let hls = Rhino.Display.ColorHSL(hls.A.ToDouble/240.0, hls.R.ToDouble/240.0, hls.G.ToDouble/240.0, hls.B.ToDouble/240.0) // TODO test if correct with reverse function
        hls.ToArgbColor()


    [<EXT>]
    ///<summary>Retrieves intensity value for the red component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(int) The red color value </returns>
    static member ColorRedValue(rgb:Drawing.Color) : int =
        int rgb.R


    [<EXT>]
    ///<summary>Convert colors from RGB to  HSL ( Hue, Saturation and Luminance)</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(Rhino.Display.ColorHSL) The HLS color value </returns>
    static member ColorRGBToHLS(rgb:Drawing.Color) : Display.ColorHSL =
        let hsl = Rhino.Display.ColorHSL(rgb)
        hsl


    [<EXT>]
    ///<summary>Removes duplicates from an array of numbers.</summary>
    ///<param name="numbers">(float seq) List or tuple</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///The minimum distance between numbers.  Numbers that fall within this tolerance will be discarded.  If omitted, Rhino's internal zero tolerance is used.</param>
    ///<returns>(float seq) numbers with duplicates removed .</returns>
    static member CullDuplicateNumbers(numbers:float seq, [<OPT;DEF(0.0)>]tolerance:float) : float seq =
        if Seq.length numbers < 2 then numbers
        else
            let tol = max tolerance  RhinoMath.ZeroTolerance // or Doc.ModelAbsoluteTolerance
            let nums = numbers|> Seq.sort
            let first = Seq.head nums
            let second = (Seq.item 1 nums)
            let mutable lastOK = first
            seq{
                if abs(first-second) > tol then
                    yield first
                    lastOK <- second
                for n in Seq.skip 2 nums do
                    if abs(lastOK-n) > tol then
                        yield n
                        lastOK <- n
               }


    [<EXT>]
    ///<summary>Removes duplicates from a list of 3D points.</summary>
    ///<param name="points">(Point3d seq) A list of 3D points.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>-1</c>
    ///Minimum distance between points. Points within this
    ///  tolerance will be discarded. If omitted, Rhino's internal zero tolerance
    ///  is used.</param>
    ///<returns>(Point3d array) of 3D points with duplicates removed .</returns>
    static member CullDuplicatePoints(points:Point3d seq, [<OPT;DEF(-1)>]tolerance:float) : Point3d array =
        let tol = max tolerance Doc.ModelAbsoluteTolerance // RhinoMath.ZeroTolerance
        Geometry.Point3d.CullDuplicates(points, tolerance)


    [<EXT>]
    ///<summary>Measures Square distance between two 3D points. Does not validate input. </summary>
    ///<param name="point1">(Point3d) The first 3D point.</param>
    ///<param name="point2">(Point3d) The second 3D point </param>
    ///<returns>(float) the square distance.</returns>
    static member DistanceSquare(point1:Point3d, point2:Point3d) : float =
        (point1 - point2).SquareLength

    [<EXT>]
    ///<summary>Measures distance between two 3D points</summary>
    ///<param name="point1">(Point3d) The first 3D point.</param>
    ///<param name="point2">(Point3d) The second 3D point </param>
    ///<returns>(float) the distance .</returns>
    static member Distance(point1:Point3d, point2:Point3d) : float =
        (point1 - point2).Length


    [<EXT>]
    ///<summary>Returns string from a specified section in a initialization file. NOT IMPLEMENTED YET</summary>
    ///<param name="filename">(string) Name of the initialization file</param>
    ///<param name="section">(string) Optional, Section containing the entry</param>
    ///<param name="entry">(string) Optional, Entry whose associated string is to be returned</param>
    ///<returns>(string seq) A list containing all section names</returns>
    static member GetSettings(filename:string, [<OPT;DEF(null:string)>]section:string, [<OPT;DEF(null:string)>]entry:string) : string seq =
        failwithf "getSettings is missing implementation" // TODO!


    [<EXT>]
    ///<summary>Returns 3D point that is a specified angle and distance from a 3D point</summary>
    ///<param name="point">(Point3d) The point to transform</param>
    ///<param name="angleDegrees">(float) Angle in degrees</param>
    ///<param name="distance">(float) Distance from point</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>Plane()</c>
    ///Plane to base the transformation. If omitted, the world
    ///  x-y plane is used</param>
    ///<returns>(Point3d) resulting point is successful</returns>
    static member Polar(point:Point3d, angleDegrees:float, distance:float, [<OPT;DEF(Plane())>]plane:Plane) : Point3d =
        let angle = toRadians(angleDegrees)
        let mutable offset = plane.XAxis
        offset.Unitize() |> ignore
        offset <- distance * offset
        let rc = point+offset
        let xform = Transform.Rotation(angle, plane.ZAxis, point)
        rc.Transform(xform)
        rc


    [<EXT>]
    ///<summary>Flattens an array of 3-D points into a one-dimensional list of real numbers. For example, if you had an array containing three 3-D points, this method would return a one-dimensional array containing nine real numbers.</summary>
    ///<param name="points">(Point3d seq) Points to flatten</param>
    ///<returns>(float ResizeArray) A one-dimensional list containing real numbers, , otherwise None</returns>
    static member SimplifyArray(points:Point3d seq) : float ResizeArray =
        resizeArray { for  p in points do
                            yield p.X
                            yield p.Y
                            yield p.Z }


    [<EXT>]
    ///<summary>Suspends execution of a running script for the specified interval. Then refreshes Rhino UI</summary>
    ///<param name="milliseconds">(int) Thousands of a second</param>
    ///<returns>(unit) </returns>
    static member Sleep(milliseconds:int) : unit =
        Threading.Thread.Sleep(milliseconds)
        RhinoApp.Wait()


    [<EXT>]
    ///<summary>Sorts list of points so they will be connected in a "reasonable" polyline order</summary>
    ///<param name="points">(Point3d seq) The points to sort</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.ZeroTolerance</c>
    ///Minimum distance between points. Points that fall within this tolerance
    ///  will be discarded. If omitted, Rhino's internal zero tolerance is used.</param>
    ///<returns>(Point3d array) of sorted 3D points</returns>
    static member SortPointList(points:Point3d seq, [<OPT;DEF(0.0)>]tolerance:float) : Point3d array =
        let tol = max RhinoMath.ZeroTolerance tolerance
        Point3d.SortAndCullPointList(points, tol)


    [<EXT>]
    ///<summary>Sorts the components of an array of 3D points</summary>
    ///<param name="points">(Point3d seq) Points to sort</param>
    ///<param name="ascending">(bool) Optional, Default Value: <c>true</c>
    ///Ascendeing if omitted (True) or True, descending if False.</param>
    ///<param name="order">(int) Optional, Default Value: <c>0</c>
    ///The component sort order
    ///  Value       Component Sort Order
    ///  0 (default) X, Y, Z
    ///  1           X, Z, Y
    ///  2           Y, X, Z
    ///  3           Y, Z, X
    ///  4           Z, X, Y
    ///  5           Z, Y, X</param>
    ///<returns>(Point3d seq) sorted 3-D points</returns>
    static member SortPoints(points:Point3d seq, [<OPT;DEF(true)>]ascending:bool, [<OPT;DEF(0)>]order:int) : Point3d seq =
        let f =
            match order with
            |0 -> fun (p:Point3d) -> p.X, p.Y, p.Z
            |1 -> fun (p:Point3d) -> p.X, p.Z, p.Y
            |2 -> fun (p:Point3d) -> p.Y, p.X, p.Z
            |3 -> fun (p:Point3d) -> p.Y, p.Z, p.X
            |4 -> fun (p:Point3d) -> p.Z, p.X, p.Y
            |5 -> fun (p:Point3d) -> p.Z, p.Y, p.X
            |_ -> failwithf "sortPoints is missing implementation for order input %d, only 0 to 5 are valid inputs" order
        if ascending then points |>  Seq.sortBy           f
        else              points |>  Seq.sortByDescending f


    [<EXT>]
    ///<summary>convert a formatted string value into a 3D point value</summary>
    ///<param name="point">(string) A string that contains a delimited point like "1,2,3".</param>
    ///<returns>(Point3d) Point structure from the input string.</returns>
    static member Str2Pt(point:string) : Point3d =
        RhinoScriptSyntax.Coerce3dPoint point


    [<EXT>]
    ///<summary>Converts 'point' into a Rhino.Geometry.Point3d if possible.</summary>
    ///<param name="point">something that can be converted or parsed to a point</param>
    ///<returns>(Point3d) a Rhino.Geometry.Point3d</returns>
    static member CreatePoint(point:'T ) : Point3d =
        RhinoScriptSyntax.Coerce3dPoint point
    [<EXT>]
    ///<summary>Converts x,y and z into a Rhino.Geometry.Point3d if possible</summary>
    ///<param name="x">something that can be converted or parsed to X coordinate </param>
    ///<param name="y">something that can be converted or parsed to Y coordinate</param>
    ///<param name="z">something that can be converted or parsed to Z coordinate</param>
    ///<returns>(Point3d) a Rhino.Geometry.Point3d</returns>
    static member CreatePoint(x:'T, y:'T,z:'T ) : Point3d =
        RhinoScriptSyntax.Coerce3dPoint ((x,y,z))


    [<EXT>]
    ///<summary>Converts 'Vector' into a Rhino.Geometry.Vector3d if possible.</summary>
    ///<param name="vector">something that can be converted or parsed to a Vector</param>
    ///<returns>(Vector3d) a Rhino.Geometry.Vector3d</returns>
    static member CreateVector(vector:'T ) : Vector3d =
        RhinoScriptSyntax.Coerce3dVector vector
    [<EXT>]
    ///<summary>Converts x,y and z into a Rhino.Geometry.Vector3d if possible</summary>
    ///<param name="x">something that can be converted or parsed to X coordinate </param>
    ///<param name="y">something that can be converted or parsed to Y coordinate</param>
    ///<param name="z">something that can be converted or parsed to Z coordinate</param>
    ///<returns>(Vector3d) a Rhino.Geometry.Vector3d</returns>
    static member CreateVector(x:'T, y:'T,z:'T ) : Vector3d =
        RhinoScriptSyntax.Coerce3dVector ((x,y,z))



    [<EXT>]
    ///<summary>Converts input into a Rhino.Geometry.Plane object if possible.</summary>
    ///<param name="origin">(Point3d) the Plane Center or Origin </param>
    ///<param name="xAxis">(Vector3d) Optional, Default Value: <c>Vector3d.XAxis</c>
    ///Direction of X-Axis</param>
    ///<param name="yAxis">(Vector3d) Optional, Default Value: <c>Vector3d.YAxis</c>
    ///Direction of Y-Axis</param>
    ///<returns>(Plane) A Rhino.Geometry.Plane</returns>
    static member CreatePlane(origin:Point3d , [<OPT;DEF(Vector3d())>]xAxis:Vector3d, [<OPT;DEF(Vector3d())>]yAxis:Vector3d) : Plane =
        if xAxis.IsZero || yAxis.IsZero then
            Plane(origin,Vector3d.XAxis,Vector3d.YAxis)
        else
            Plane(origin, xAxis, yAxis)


    [<EXT>]
    ///<summary>Converts input into a Rhino.Geometry.Transform object if possible.</summary>
    ///<param name="xform">(seq<seq<float>>) The transform. This can be seen as a 4x4 matrix, given as nested lists.</param>
    ///<returns>(Transform) A Rhino.Geometry.Transform. result[0,3] gives access to the first row, last column.</returns>
    static member CreateXform(xform:seq<seq<float>>) : Transform =
        RhinoScriptSyntax.CoerceXform(xform) // TODO verify row, column order !!


    [<EXT>]
    ///<summary>Converts input into a native color object if possible.
    ///  The returned data is accessible by indexing, and that is the suggested method to interact with the type.
    ///  Red index is [0], Green index is [1], Blue index is [2] and Alpha index is [3].
    ///  If the provided object is already a color, its value is copied.
    ///  Alternatively, you can also pass three coordinates singularly for an RGB color, or four
    ///  for an RGBA color point.</summary>
    ///<param name="red">(int) Red Value</param>
    ///<param name="green">(int) Green value</param>
    ///<param name="blue">(int) Blue value</param>
    ///<param name="alpha">(int)Optional, Default Value: <c>0</c>
    ///Alpha value</param>
    ///<returns>(Drawing.Color) a Color</returns>
    static member CreateColor(red:int, green:int,blue:int, [<OPT;DEF(0)>]alpha:int) : Drawing.Color =
        RhinoScriptSyntax.CoerceColor((red,green,blue,alpha))


    [<EXT>]
    ///<summary>Converts input into a Rhino.Geometry.Interval.</summary>
    ///<param name="start">(float) The lower bound</param>
    ///<param name="ende">(float) Uper bound of interval</param>
    ///<returns>(Rhino.Geometry.Interval) This can be seen as an object made of two items:
    ///  [0] start of interval
    ///  [1] end of interval</returns>
    static member CreateInterval(start:float,ende:float) : Rhino.Geometry.Interval =
        Geometry.Interval(start , ende)
