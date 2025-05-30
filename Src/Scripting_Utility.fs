﻿namespace Rhino.Scripting

open Rhino
open System
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils


[<AutoOpen>]
module AutoOpenUtility =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    /// <summary>Return true if the script is being executed in the context of Rhino (currently always true).</summary>
    /// <returns>(bool) True if the script is being executed in the context of Rhino (currently always true).</returns>
    static member ContextIsRhino() : bool =
        true //TODO implement correctly


    /// <summary>Return true if the script is being executed in a Grasshopper component (currently always false).</summary>
    /// <returns>(bool) True if the script is being executed in a Grasshopper component (currently always false).</returns>
    static member ContextIsGrasshopper() : bool =
        false //TODO implement correctly


    /// <summary>Measures the angle between two points.</summary>
    /// <param name="point1">(Point3d) Point1 of input points</param>
    /// <param name="point2">(Point3d) Point2 of input points</param>
    /// <param name="plane">(Plane) Optional, default value: <c>Plane.WorldX</c>
    ///    If a Plane is provided, angle calculation is with respect to this Plane</param>
    /// <returns>(float * float * float * float * float) containing the following elements:
    ///    element 0 = the X, Y angle in degrees
    ///    element 1 = the elevation
    ///    element 2 = delta in the X direction
    ///    element 3 = delta in the Y direction
    ///    element 4 = delta in the Z direction.</returns>
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
        let angleXY = toDegrees( Math.Atan2( y, x ))
        let elevation = toDegrees( Math.Atan2( z, h ))
        angleXY, elevation, x, y, z


    /// <summary>Measures the angle between two lines.</summary>
    /// <param name="line1">(Line) List of 6 numbers or 2 Point3d</param>
    /// <param name="line2">(Line) List of 6 numbers or 2 Point3d</param>
    /// <returns>(float * float) containing the following elements:
    ///    0 = The angle in degrees.
    ///    1 = The reflex angle in degrees.</returns>
    static member Angle2(line1:Line, line2:Line) : float * float =
        let vec0 = line1.To - line1.From
        let vec1 = line2.To - line2.From
        if not <| vec0.Unitize() || not <| vec1.Unitize() then  RhinoScriptingException.Raise "RhinoScriptSyntax.Angle2 two failed on %A and %A" line1 line2
        let mutable dot = vec0 * vec1
        dot <- max -1.0 (min 1.0 dot) // clamp for math errors
        let mutable angle = Math.Acos(dot)
        let mutable reflexAngle = 2.0*Math.PI - angle
        angle <- toDegrees(angle)
        reflexAngle <- toDegrees(reflexAngle)
        angle, reflexAngle


    /// <summary>Gets the text string in the Windows clipboard.</summary>
    /// <returns>(string) The current text in the clipboard
    /// or an empty string if the content of the clipboard is not a text</returns>
    static member ClipboardText() : string = //GET
        // Eto Forms fails:
        // https://discourse.mcneel.com/t/using-default-eto-control-over-rhino-variant/167138/8
        // let platform = Eto.Platform.Get(Eto.Platform.Instance.GetType().AssemblyQualifiedName) // Platform.Instance is null
        //let platform = Eto.Platform.Detect //Could not detect platform. Are you missing a platform assembly? --->  Error creating instance of platform type 'Eto.Wpf.Platform, Eto.Wpf' --->  Unable to cast object of type 'Eto.Wpf.Platform' to type 'Eto.Platform'.
        //use _ctx = platform.Context
        // let cl = Eto.Forms.Clipboard.Instance
        // if isNull cl then RhinoScriptingException.Raise "RhinoScriptSyntax.ClipboardText() failed to get clipboard instance"
        // if cl.ContainsText then cl.Text else ""

        // if Windows.Forms.Clipboard.ContainsText() then Windows.Forms.Clipboard.GetText() else ""
        // use reflection to avoid dependency on Windows.Forms
        let clipboardType = Type.GetType "System.Windows.Forms.Clipboard, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        match clipboardType with
        | null ->
            State.Warn "Could not access clipboard in RhinoScriptSyntax.ClipboardText. Could not load System.Windows.Forms.Clipboard type"
            ""
        | t ->
            let containsTextMethod = t.GetMethod("ContainsText", System.Type.EmptyTypes)
            let getTextMethod = t.GetMethod("GetText", System.Type.EmptyTypes)
            match containsTextMethod, getTextMethod with
            | null, _ ->
                State.Warn "Could not access clipboard in RhinoScriptSyntax.ClipboardText. Could not find ContainsText method on Clipboard type"
                ""
            | _, null ->
                State.Warn "Could not access clipboard in RhinoScriptSyntax.ClipboardText. Could not find GetText method on Clipboard type"
                ""
            | containsMethod, getText ->
                let containsText = containsMethod.Invoke(null, null) :?> bool
                if containsText then
                    getText.Invoke(null, null) :?> string
                else
                    ""

    /// <summary>Sets a text string to the Windows clipboard.</summary>
    /// <param name="text">(string) Text to set</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ClipboardText(text:string) : unit = //SET
        // does not work with Eto.Forms.Clipboard
        // let cl = Eto.Forms.Clipboard.Instance
        // if isNull cl then RhinoScriptingException.Raise "RhinoScriptSyntax.ClipboardText(text) failed to get clipboard instance"
        // cl.Text <- text //System.InvalidOperationException: Platform instance is null. Have you created your application?

        // Windows.Forms.Clipboard.SetText(text)
        // use reflection to avoid dependency on Windows.Forms
        let clipboardType = Type.GetType "System.Windows.Forms.Clipboard, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        match clipboardType with
        | null ->
            State.Warn "Could not access clipboard in RhinoScriptSyntax.ClipboardText. Could not load System.Windows.Forms.Clipboard type"
        | t ->
            let setTextMethod = t.GetMethod("SetText", [| typeof<string> |])
            match setTextMethod with
            | null ->
                State.Warn "Could not access clipboard in RhinoScriptSyntax.ClipboardText. Could not find SetText method on Clipboard type"
            | setText ->
                try
                    let _ = setText.Invoke(null, [| text :> obj |])
                    ()
                with ex ->
                    State.Warn $"Failed to set clipboard text: {ex.Message}"



    /// <summary>Changes the luminance of a red-green-blue value. Hue and saturation are
    ///    not affected.</summary>
    /// <param name="rgb">(Drawing.Color) Initial rgb value</param>
    /// <param name="luminance">(float) The luminance in units of 0.1 percent of the total range. A
    ///    value of luminance = 50 corresponds to 5 percent of the maximum luminance </param>
    /// <param name="isScaleRelative">(bool) Optional, default value: <c>false</c>
    ///    If True, luminance specifies how much to increment or decrement the
    ///    current luminance. If False, luminance specified the absolute luminance </param>
    /// <returns>(Drawing.Color) modified rgb value.</returns>
    static member ColorAdjustLuma(rgb:Drawing.Color, luminance:float, [<OPT;DEF(false)>]isScaleRelative:bool) : Drawing.Color =
        let mutable hsl = Display.ColorHSL(rgb)
        let mutable luminance = luminance / 1000.0
        if isScaleRelative then luminance <- hsl.L + luminance
        hsl.L <- luminance
        hsl.ToArgbColor()


    /// <summary>Retrieves intensity value for the blue component of an RGB color.</summary>
    /// <param name="rgb">(Drawing.Color) The RGB color value</param>
    /// <returns>(int) The blue component.</returns>
    static member ColorBlueValue(rgb:Drawing.Color) : int =
       int rgb.B


    /// <summary>Retrieves intensity value for the green component of an RGB color.</summary>
    /// <param name="rgb">(Drawing.Color) The RGB color value</param>
    /// <returns>(int) The green component.</returns>
    static member ColorGreenValue(rgb:Drawing.Color) : int =
       int rgb.G


    /// <summary>Converts colors from hue-luminance-saturation to RGB.</summary>
    /// <param name="hls">(Drawing.Color) The HLS color value</param>
    /// <returns>(Drawing.Color) The RGB color value.</returns>
    static member ColorHLSToRGB(hls:Drawing.Color) : Drawing.Color =
        let hls = Display.ColorHSL((float hls.A) /240.0, (float hls.R)/240.0, (float hls.G)/240.0, (float hls.B)/240.0) // TODO test if correct with reverse function
        hls.ToArgbColor()


    /// <summary>Retrieves intensity value for the red component of an RGB color.</summary>
    /// <param name="rgb">(Drawing.Color) The RGB color value</param>
    /// <returns>(int) The red color value.</returns>
    static member ColorRedValue(rgb:Drawing.Color) : int =
        int rgb.R


    /// <summary>Convert colors from RGB to  HSL ( Hue, Saturation and Luminance).</summary>
    /// <param name="rgb">(Drawing.Color) The RGB color value</param>
    /// <returns>(Display.ColorHSL) The HLS color value.</returns>
    static member ColorRGBToHLS(rgb:Drawing.Color) : Display.ColorHSL =
        let hsl = Display.ColorHSL(rgb)
        hsl


    /// <summary>Removes duplicates from an array of numbers.</summary>
    /// <param name="numbers">(float seq) List of Doubles</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c>
    ///    The minimum distance between numbers. Numbers that fall within this tolerance will be discarded</param>
    /// <returns>(float ResizeArray) numbers with duplicates removed.</returns>
    static member CullDuplicateNumbers(numbers:float seq, [<OPT;DEF(0.0)>]tolerance:float) : float ResizeArray =
        if Seq.length numbers < 2 then ResizeArray(numbers )
        else
            let tol = Util.ifZero1 tolerance  RhinoMath.ZeroTolerance // or State.Doc.ModelAbsoluteTolerance
            let nums = numbers|> Seq.sort
            let first = Seq.head nums
            let second = (Seq.item 1 nums)
            let mutable lastOK = first
            let r = ResizeArray()
            if abs(first-second) > tol then
                r.Add first
                lastOK <- second
            for n in Seq.skip 2 nums do
                if abs(lastOK-n) > tol then
                    r.Add n
                    lastOK <- n
            r



    /// <summary>Removes duplicates from a list of 3D points.</summary>
    /// <param name="points">(Point3d seq) A list of 3D points</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c> Minimum distance between points.
    /// Points within this tolerance will be discarded.</param>
    /// <returns>(Point3d array) Array of 3D points with duplicates removed.</returns>
    static member CullDuplicatePoints(points:Point3d seq, [<OPT;DEF(0.0)>]tolerance:float) : Point3d array =
        let tol = Util.ifZero1 tolerance State.Doc.ModelAbsoluteTolerance // RhinoMath.ZeroTolerance
        Geometry.Point3d.CullDuplicates(points, tol)


    /// <summary>Measures Square distance between two 3D points. Does not validate input.</summary>
    /// <param name="point1">(Point3d) The first 3D point</param>
    /// <param name="point2">(Point3d) The second 3D point</param>
    /// <returns>(float) The square distance.</returns>
    static member inline DistanceSquare(point1:Point3d, point2:Point3d) : float =
        let x = point1.X - point2.X
        let y = point1.Y - point2.Y
        let z = point1.Z - point2.Z
        x*x + y*y + z*z

    /// <summary>Measures distance between two 3D points.</summary>
    /// <param name="point1">(Point3d) The first 3D point</param>
    /// <param name="point2">(Point3d) The second 3D point</param>
    /// <returns>(float) The distance.</returns>
    static member inline Distance(point1:Point3d, point2:Point3d) : float =
        let x = point1.X - point2.X
        let y = point1.Y - point2.Y
        let z = point1.Z - point2.Z
        sqrt (x*x + y*y + z*z)

    /// <summary>Returns 3D point that is a specified angle and distance from a 3D point.</summary>
    /// <param name="point">(Point3d) The point to transform</param>
    /// <param name="angleDegrees">(float) Angle in degrees</param>
    /// <param name="distance">(float) Distance from point</param>
    /// <param name="plane">(Plane) Optional, Plane to base the transformation. If omitted, the world
    ///    x-y Plane is used</param>
    /// <returns>(Point3d) resulting point is successful.</returns>
    static member Polar(point:Point3d, angleDegrees:float, distance:float, [<OPT;DEF(Plane())>]plane:Plane) : Point3d =
        let angle = toRadians(angleDegrees)
        let mutable offset = plane.XAxis
        offset.Unitize() |> ignore<bool>
        offset <- distance * offset
        let rc = point + offset
        let xForm = Transform.Rotation(angle, plane.ZAxis, point)
        rc.Transform(xForm)
        rc


    /// <summary>Flattens an array of 3-D points into a one-dimensional list of real numbers. For example, if you had an array containing three 3-D points, this method would return a one-dimensional array containing nine real numbers.</summary>
    /// <param name="points">(Point3d seq) Points to flatten</param>
    /// <returns>(float ResizeArray) A one-dimensional list containing real numbers.</returns>
    static member SimplifyArray(points:Point3d seq) : float ResizeArray =
        let r = ResizeArray()
        for p in points do
            r.Add p.X
            r.Add p.Y
            r.Add p.Z
        r


    /// <summary>Suspends execution of a running script for the specified interval. Then refreshes Rhino UI.</summary>
    /// <param name="milliseconds">(int) Thousands of a second</param>
    /// <returns>(unit).</returns>
    static member Sleep(milliseconds:int) : unit =
        Threading.Thread.Sleep(milliseconds)
        RhinoApp.Wait()


    /// <summary>Sorts list of points so they will be connected in a "reasonable" Polyline order.</summary>
    /// <param name="points">(Point3d seq) The points to sort</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.ZeroTolerance</c>
    ///    Minimum distance between points. Points that fall within this tolerance
    ///    will be discarded.</param>
    /// <returns>(Point3d array) Array of sorted 3D points.</returns>
    static member SortPointList(points:Point3d seq, [<OPT;DEF(0.0)>]tolerance:float) : Point3d array =
        let tol = Util.ifZero2 RhinoMath.ZeroTolerance tolerance
        Point3d.SortAndCullPointList(points, tol)


    /// <summary>Sorts the components of an array of 3D points.</summary>
    /// <param name="points">(Point3d seq) Points to sort</param>
    /// <param name="ascending">(bool) Optional, default value: <c>true</c>
    ///    Ascending if omitted (True) or True, descending if False</param>
    /// <param name="order">(int) Optional, default value: <c>0</c>
    ///    The component sort order
    ///    Value       Component Sort Order
    ///    0 (default) X, Y, Z
    ///    1           X, Z, Y
    ///    2           Y, X, Z
    ///    3           Y, Z, X
    ///    4           Z, X, Y
    ///    5           Z, Y, X</param>
    /// <returns>(Point3d seq) sorted 3-D points.</returns>
    static member SortPoints(points:Point3d seq, [<OPT;DEF(true)>]ascending:bool, [<OPT;DEF(0)>]order:int) : Point3d seq =
        let f =
            match order with
            |0 -> fun (p:Point3d) -> p.X, p.Y, p.Z
            |1 -> fun (p:Point3d) -> p.X, p.Z, p.Y
            |2 -> fun (p:Point3d) -> p.Y, p.X, p.Z
            |3 -> fun (p:Point3d) -> p.Y, p.Z, p.X
            |4 -> fun (p:Point3d) -> p.Z, p.X, p.Y
            |5 -> fun (p:Point3d) -> p.Z, p.Y, p.X
            |_ -> RhinoScriptingException.Raise "RhinoScriptSyntax.SortPoints is missing implementation for order input %d, only 0 to 5 are valid inputs" order
        if ascending then points |>  Seq.sortBy           f
        else              points |>  Seq.sortByDescending f


    /// <summary>convert a formatted string value into a 3D point value.</summary>
    /// <param name="point">(string) A string that contains a delimited point like "1, 2, 3"</param>
    /// <returns>(Point3d) Point structure from the input string.</returns>
    static member Str2Pt(point:string) : Point3d =
        RhinoScriptSyntax.Coerce3dPoint point


    /// <summary>Converts 'point' into a Rhino.Geometry.Point3d if possible.</summary>
    /// <param name="point">('T) any value that can be converted or parsed to a point</param>
    /// <returns>(Point3d) a Rhino.Geometry.Point3d.</returns>
    static member CreatePoint(point:'T ) : Point3d =
        RhinoScriptSyntax.Coerce3dPoint point

    /// <summary>Converts x, y and z into a Rhino.Geometry.Point3d if possible.</summary>
    /// <param name="x">('T) any value that can be converted or parsed to X coordinate</param>
    /// <param name="y">('T) any value that can be converted or parsed to Y coordinate</param>
    /// <param name="z">('T) any value that can be converted or parsed to Z coordinate</param>
    /// <returns>(Point3d) a Rhino.Geometry.Point3d.</returns>
    static member CreatePoint(x:'T, y:'T, z:'T ) : Point3d =
        RhinoScriptSyntax.Coerce3dPoint ((x, y, z))


    /// <summary>Converts 'Vector' into a Rhino.Geometry.Vector3d if possible.</summary>
    /// <param name="vector">('T) any value that can be converted or parsed to a Vector</param>
    /// <returns>(Vector3d) a Rhino.Geometry.Vector3d.</returns>
    static member CreateVector(vector:'T ) : Vector3d =
        RhinoScriptSyntax.Coerce3dVector vector
    /// <summary>Converts x, y and z into a Rhino.Geometry.Vector3d if possible.</summary>
    /// <param name="x">('T) any value that can be converted or parsed to X coordinate</param>
    /// <param name="y">('T) any value that can be converted or parsed to Y coordinate</param>
    /// <param name="z">('T) any value that can be converted or parsed to Z coordinate</param>
    /// <returns>(Vector3d) a Rhino.Geometry.Vector3d.</returns>
    static member CreateVector(x:'T, y:'T, z:'T ) : Vector3d =
        RhinoScriptSyntax.Coerce3dVector ((x, y, z))



    /// <summary>Converts input into a Rhino.Geometry.Plane object if possible.</summary>
    /// <param name="origin">(Point3d) The Plane Center or Origin</param>
    /// <param name="xAxis">(Vector3d) Optional, default value: <c>Vector3d.XAxis</c>
    ///    Direction of X-Axis</param>
    /// <param name="yAxis">(Vector3d) Optional, default value: <c>Vector3d.YAxis</c>
    ///    Direction of Y-Axis</param>
    /// <returns>(Plane) A Rhino.Geometry.Plane.</returns>
    static member CreatePlane(origin:Point3d , [<OPT;DEF(Vector3d())>]xAxis:Vector3d, [<OPT;DEF(Vector3d())>]yAxis:Vector3d) : Plane =
        if xAxis.IsZero || yAxis.IsZero then
            Plane(origin, Vector3d.XAxis, Vector3d.YAxis)
        else
            Plane(origin, xAxis, yAxis)


    /// <summary>Converts input into a Rhino.Geometry.Transform object if possible.</summary>
    /// <param name="xForm">(float seq seq) The transform. This can be seen as a 4x4 matrix, given as nested lists</param>
    /// <returns>(Transform) A Rhino.Geometry.Transform. result[0, 3] gives access to the first row, last column.</returns>
    static member CreateXform(xForm:seq<seq<float>>) : Transform =
        RhinoScriptSyntax.CoerceXform(xForm) // TODO verify row, column order !!


    /// <summary>Creates a RGB color from red, green and  blue  values as Integers. Raise an exception if values are out of range</summary>
    /// <param name="red">(int) Red Value between 0 and 255 </param>
    /// <param name="green">(int) Green value between 0 and 255 </param>
    /// <param name="blue">(int) Blue value between 0 and 255 </param>
    /// <returns>(System.Drawing.Color) a Color.</returns>
    static member CreateColor(red:int, green:int, blue:int) : Drawing.Color =
        if red   < 0 || red > 255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CreateColor red value out of range 0 to 255: %d" red
        if green < 0 || green > 255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CreateColor green value out of range 0 to 255: %d" green
        if blue  < 0 || blue > 255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CreateColor blue value out of range 0 to 255: %d" blue
        Drawing.Color.FromArgb(red, green, blue)

    /// <summary>Creates a RGB color from  red, green and  blue  values as Bytes.</summary>
    /// <param name="red">(byte) Red Value</param>
    /// <param name="green">(byte) Green value</param>
    /// <param name="blue">(byte) Blue value</param>
    /// <returns>(System.Drawing.Color) a Color.</returns>
    static member CreateColor(red:byte, green:byte, blue:byte) : Drawing.Color =
        Drawing.Color.FromArgb(int red, int green, int blue)

    /// <summary>Converts input into a Rhino.Geometry.Interval. This interval can be increasing or decreasing</summary>
    /// <param name="start">(float) Start of interval</param>
    /// <param name="ende">(float) End of interval</param>
    /// <returns>(Rhino.Geometry.Interval) This can be seen as an object made of two items:
    ///    [0] start of interval
    ///    [1] end of interval.</returns>
    static member CreateInterval(start:float, ende:float) : Rhino.Geometry.Interval =
        Geometry.Interval(start , ende)


    (*
    manipulating ini files like in original Rhinoscript could be include via
    <PackageReference Include="ini-parser" Version="2.5.2" />
    however for now it is excluded to keep the dependencies at just FsEx.
    If ini-file reading and writing is needed I would suggest to use the "ini-parser" package directly and not the below functions.

    open IniParser
    open IniParser.Model

    /// <summary>Returns section names or keys in one section of an ini file.</summary>
    /// <param name="filename">(string) Name  and path of the ini-file</param>
    /// <param name="section">(string) Optional, Section to list keys from</param>
    /// <returns>(string array)
    ///    If section is NOT specified, a list containing all section names
    ///    If section is specified, a list containing all entry names for the given section.</returns>
    static member GetSettings(filename:string, [<OPT;DEF(null:string)>]section:string) : string ResizeArray =
        //https://github.com/rickyah/ini-parser

        //https://github.com/rickyah/ini-parser/wiki/Configuring-parser-behavior
        let parser = new FileIniDataParser()
        let data = parser.ReadFile(filename)
        data.Configuration.ThrowExceptionsOnError <-true
        if isNull section then
            resizeArray { for s in data.Sections do s.SectionName }
        else
            resizeArray { for k in data.[section] do k.KeyName}

    /// <summary>Returns string from a specified section and entry in an ini file.</summary>
    /// <param name="filename">(string) Name  and path of the ini file</param>
    /// <param name="section">(string) Section containing the entry,for keys without section use empty string</param>
    /// <param name="entry">(string) Entry whose associated string is to be returned</param>
    /// <returns>(string) a value for entry.</returns>
    static member GetSettings(filename:string, section:string, entry:string) : string =
        let parser = new FileIniDataParser()
        let data = parser.ReadFile(filename)
        data.Configuration.ThrowExceptionsOnError <-true
        let s =
            if section = "" then
                data.Global.[entry]
            else
                data.[section].[entry]
        if isNull s then RhinoScriptingException.Raise "RhinoScriptSyntax.GetSettings entry '%s' in section '%s' not found in file %s" entry section filename
        else s

    /// <summary>Saves a specified section and entry in an ini file.</summary>
    /// <param name="filename">(string) Name and path of the ini file</param>
    /// <param name="section">(string) Section containing the entry. if empty string key without section will be added</param>
    /// <param name="entry">(string) Entry whose associated string is to be returned</param>
    /// <param name="value">(string) The Value of this entry</param>
    /// <returns>(string) a value for entry.</returns>
    static member SaveSettings(filename:string, section:string, entry:string,value:string) : unit =
        if IO.File.Exists filename then
            let parser = new FileIniDataParser()
            parser.Parser.Configuration.ThrowExceptionsOnError <-true
            let data = parser.ReadFile(filename)
            data.Configuration.ThrowExceptionsOnError <-true
            if section = "" then
                data.Global.[entry]<- value
            else
                data.Configuration.AllowCreateSectionsOnFly <- true
                data.[section].[entry] <- value
            parser.WriteFile(filename,data)
        else
            let data = IniData()
            data.Configuration.ThrowExceptionsOnError <-true
            if section = "" then
                data.Global.[entry]<- value
            else
                data.Configuration.AllowCreateSectionsOnFly <- true
                data.[section].[entry] <- value
            IO.File.WriteAllText(filename,data.ToString())
    *)

