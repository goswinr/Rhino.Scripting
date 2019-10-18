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
    (*
    def ContextIsRhino():
        '''Return True if the script is being executed in the context of Rhino
        Returns:
          bool: True if the script is being executed in the context of Rhino
        '''
        return scriptcontext.objectId == 1
    *)


    [<EXT>]
    ///<summary>Return True if the script is being executed in a grasshopper component(currently always true)</summary>
    ///<returns>(bool) True if the script is being executed in a grasshopper component(currently always true)</returns>
    static member ContextIsGrasshopper() : bool =
        true //TODO implement correctly
    (*
    def ContextIsGrasshopper():
        '''Return True if the script is being executed in a grasshopper component
        Returns:
          bool: True if the script is being executed in a grasshopper component
        '''
        return scriptcontext.objectId == 2
    *)


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
    (*
    def Angle(point1, point2, plane=True):
        '''Measures the angle between two points
        Parameters:
          point1, point2 (point): the input points
          plane (bool, optional): Boolean or Plane
            If True, angle calculation is based on the world coordinate system.
            If False, angle calculation is based on the active construction plane
            If a plane is provided, angle calculation is with respect to this plane
        Returns:
          tuple(tuple(number, number), number, number, number, number): containing the following elements if successful
            element 0 = the X,Y angle in degrees
            element 1 = the elevation
            element 2 = delta in the X direction
            element 3 = delta in the Y direction
            element 4 = delta in the Z direction
          None: if not successful
        '''
        pt1 = coerce3dpoint(point1)
        if pt1 is None:
            pt1 = coercerhinoobject(point1)
            if isinstance(pt1, Rhino.DocObjects.PointObject): pt1 = pt1.Geometry.Location
            else: pt1=None
        pt2 = coerce3dpoint(point2)
        if pt2 is None:
            pt2 = coercerhinoobject(point2)
            if isinstance(pt2, Rhino.DocObjects.PointObject): pt2 = pt2.Geometry.Location
            else: pt2=None
        point1 = pt1
        point2 = pt2
        if point1 is None or point2 is None: return scriptcontext.errorhandler()
        vector = point2 - point1
        x = vector.X
        y = vector.Y
        z = vector.Z
        if plane!=True:
            plane = coerceplane(plane)
            if plane is None:
                plane = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane()
            vfrom = point1 - plane.Origin
            vto = point2 - plane.Origin
            x = vto * plane.XAxis - vfrom * plane.XAxis
            y = vto * plane.YAxis - vfrom * plane.YAxis
            z = vto * plane.ZAxis - vfrom * plane.ZAxis
        h = math.sqrt( x * x + y * y)
        angle_xy = math.degrees( math.atan2( y, x ) )
        elevation = math.degrees( math.atan2( z, h ) )
        return angle_xy, elevation, x, y, z
    *)


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
    (*
    def Angle2(line1, line2):
        '''Measures the angle between two lines
        Parameters:
          line1 (line): List of 6 numbers or 2 Point3d.
          line2 (line): List of 6 numbers or 2 Point3d.
        Returns:
          tuple(number, number): containing the following elements if successful.
            0 The angle in degrees.
            1 The reflex angle in degrees.
          None: If not successful, or on error.
        '''
        line1 = coerceline(line1, True)
        line2 = coerceline(line2, True)
        vec0 = line1.To - line1.From
        vec1 = line2.To - line2.From
        if not vec0.Unitize() or not vec1.Unitize(): return scriptcontext.errorhandler()
        dot = vec0 * vec1
        dot = clamp(-1,1,dot)
        angle = math.acos(dot)
        reflex_angle = 2.0*math.pi - angle
        angle = math.degrees(angle)
        reflex_angle = math.degrees(reflex_angle)
        return angle, reflex_angle
    *)


    [<EXT>]
    ///<summary>Returns a text string to the Windows clipboard</summary>
    ///<returns>(string) The current text in the clipboard</returns>
    static member ClipboardText() : string = //GET
        if Windows.Forms.Clipboard.ContainsText() then Windows.Forms.Clipboard.GetText() else ""
    (*
    def ClipboardText(text=None):
        '''Returns or sets a text string to the Windows clipboard
        Parameters:
          text (str, optional): text to set
        Returns:
          str: if text is not specified, the current text in the clipboard
          str: if text is specified, the previous text in the clipboard
          None: if not successful
        '''
        rc = None
        if System.Windows.Forms.Clipboard.ContainsText():
            rc = System.Windows.Forms.Clipboard.GetText()
        if text:
            if not isinstance(text, str): text = str(text)
            System.Windows.Forms.Clipboard.SetText(text)
        return rc
    *)

    ///<summary>Sets a text string to the Windows clipboard</summary>
    ///<param name="text">(string)Text to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member ClipboardText(text:string) : unit = //SET
        System.Windows.Forms.Clipboard.SetText(text)
    (*
    def ClipboardText(text=None):
        '''Returns or sets a text string to the Windows clipboard
        Parameters:
          text (str, optional): text to set
        Returns:
          str: if text is not specified, the current text in the clipboard
          str: if text is specified, the previous text in the clipboard
          None: if not successful
        '''
        rc = None
        if System.Windows.Forms.Clipboard.ContainsText():
            rc = System.Windows.Forms.Clipboard.GetText()
        if text:
            if not isinstance(text, str): text = str(text)
            System.Windows.Forms.Clipboard.SetText(text)
        return rc
    *)


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
    (*
    def ColorAdjustLuma(rgb, luma, scale=False):
        '''Changes the luminance of a red-green-blue value. Hue and saturation are
        not affected
        Parameters:
          rgb (color): initial rgb value
          luma (number): The luminance in units of 0.1 percent of the total range. A
              value of luma = 50 corresponds to 5 percent of the maximum luminance
          scale (bool, optional): if True, luma specifies how much to increment or decrement the
              current luminance. If False, luma specified the absolute luminance.
        Returns:
          color: modified rgb value if successful
        '''
        rgb = coercecolor(rgb, True)
        hsl = Rhino.Display.ColorHSL(rgb)
        luma = luma / 1000.0
        if scale: luma = hsl.L + luma
        hsl.L = luma
        return hsl.ToArgbColor()
    *)


    [<EXT>]
    ///<summary>Retrieves intensity value for the blue component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(int) The blue component </returns>
    static member ColorBlueValue(rgb:Drawing.Color) : int =
       int rgb.B
    (*
    def ColorBlueValue(rgb):
        '''Retrieves intensity value for the blue component of an RGB color
        Parameters:
          rgb (color): the RGB color value
        Returns:
          number: The blue component if successful, otherwise None
        '''
        return coercecolor(rgb, True).B
    *)


    [<EXT>]
    ///<summary>Retrieves intensity value for the green component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(int) The green component</returns>
    static member ColorGreenValue(rgb:Drawing.Color) : int =
       int rgb.G
    (*
    def ColorGreenValue(rgb):
        '''Retrieves intensity value for the green component of an RGB color
        Parameters:
          rgb (color): the RGB color value
        Returns:
          number: The green component if successful, otherwise None
        '''
        return coercecolor(rgb, True).G
    *)


    [<EXT>]
    ///<summary>Converts colors from hue-lumanence-saturation to RGB</summary>
    ///<param name="hls">(Drawing.Color) The HLS color value</param>
    ///<returns>(Drawing.Color) The RGB color value</returns>
    static member ColorHLSToRGB(hls:Drawing.Color) : Drawing.Color =
        let hls = Rhino.Display.ColorHSL(hls.A.ToDouble/240.0, hls.R.ToDouble/240.0, hls.G.ToDouble/240.0, hls.B.ToDouble/240.0) // TODO test if correct with reverse function
        hls.ToArgbColor()
    (*
    def ColorHLSToRGB(hls):
        '''Converts colors from hue-lumanence-saturation to RGB
        Parameters:
          hls (color): the HLS color value
        Returns:
          color: The RGB color value if successful, otherwise False
        '''
        if len(hls)==3:
            hls = Rhino.Display.ColorHSL(hls[0]/240.0, hls[2]/240.0, hls[1]/240.0)
        elif len(hls)==4:
            hls = Rhino.Display.ColorHSL(hls[3]/240.0, hls[0]/240.0, hls[2]/240.0, hls[1]/240.0)
        return hls.ToArgbColor()
    *)


    [<EXT>]
    ///<summary>Retrieves intensity value for the red component of an RGB color</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(int) The red color value </returns>
    static member ColorRedValue(rgb:Drawing.Color) : int =
        int rgb.R
    (*
    def ColorRedValue(rgb):
        '''Retrieves intensity value for the red component of an RGB color
        Parameters:
          rgb (color): the RGB color value
        Returns:
          color: The red color value if successful, otherwise False
        '''
        return coercecolor(rgb, True).R
    *)


    [<EXT>]
    ///<summary>Convert colors from RGB to  HSL ( Hue, Saturation and Luminance)</summary>
    ///<param name="rgb">(Drawing.Color) The RGB color value</param>
    ///<returns>(Rhino.Display.ColorHSL) The HLS color value </returns>
    static member ColorRGBToHLS(rgb:Drawing.Color) : Display.ColorHSL =
        let hsl = Rhino.Display.ColorHSL(rgb)
        hsl
    (*
    def ColorRGBToHLS(rgb):
        '''Convert colors from RGB to HLS
        Parameters:
          rgb (color): the RGB color value
        Returns:
          color: The HLS color value if successful, otherwise False
        '''
        rgb = coercecolor(rgb, True)
        hsl = Rhino.Display.ColorHSL(rgb)
        return hsl.H, hsl.S, hsl.L
    *)


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
    (*
    def CullDuplicateNumbers(numbers, tolerance=None):
        '''Removes duplicates from an array of numbers.
        Parameters:
          numbers ([number, ...]): list or tuple
          tolerance (number, optional): The minimum distance between numbers.  Numbers that fall within this tolerance will be discarded.  If omitted, Rhino's internal zero tolerance is used.
        Returns:
          list(number, ...): numbers with duplicates removed if successful.
        '''
        count = len(numbers)
        if count < 2: return numbers
        if tolerance is None: tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        numbers = sorted(numbers)
        d = numbers[0]
        index = 1
        for step in range(1,count):
            test_value = numbers[index]
            if math.fabs(d-test_value)<=tolerance:
                numbers.pop(index)
            else:
                d = test_value
                index += 1
        return numbers
    *)


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
    (*
    def CullDuplicatePoints(points, tolerance=-1):
        '''Removes duplicates from a list of 3D points.
        Parameters:
          points ([point, ...]): A list of 3D points.
          tolerance (number): Minimum distance between points. Points within this
            tolerance will be discarded. If omitted, Rhino's internal zero tolerance
            is used.
        Returns:
          list(point, ...): of 3D points with duplicates removed if successful.
          None: if not successful
        '''
        points = coerce3dpointlist(points, True)
        if tolerance is None or tolerance < 0:
            tolerance = Rhino.RhinoMath.ZeroTolerance
        return list(Rhino.Geometry.Point3d.CullDuplicates(points, tolerance))
    *)


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
    (*
    def Distance(point1, point2):
        '''Measures distance between two 3D points, or between a 3D point and
        an array of 3D points.
        Parameters:
          point1 (point): The first 3D point.
          point2 (point): The second 3D point or list of 3-D points.
        Returns:
          point: If point2 is a 3D point then the distance if successful.
          point: If point2 is a list of points, then an list of distances if successful.
          None: if not successful
        '''
        from_pt = coerce3dpoint(point1, True)
        to_pt = coerce3dpoint(point2)
        if to_pt: return (to_pt - from_pt).Length
        # check if we have a list of points
        to_pt = coerce3dpointlist(point2, True)
        distances = [(point - from_pt).Length for point in to_pt]
        if distances: return distances
    *)


    [<EXT>]
    ///<summary>Returns string from a specified section in a initialization file. NOT IMPLEMENTED YET</summary> 
    ///<param name="filename">(string) Name of the initialization file</param>
    ///<param name="section">(string) Optional, Section containing the entry</param>
    ///<param name="entry">(string) Optional, Entry whose associated string is to be returned</param>
    ///<returns>(string seq) A list containing all section names</returns>
    static member GetSettings(filename:string, [<OPT;DEF(null:string)>]section:string, [<OPT;DEF(null:string)>]entry:string) : string seq =
        failwithf "getSettings is missing implementation" // TODO!
    (*
    def GetSettings(filename, section=None, entry=None):
        '''Returns string from a specified section in a initialization file.
        Parameters:
          filename (str): name of the initialization file
          section (str, optional): section containing the entry
          entry (str, optional): entry whose associated string is to be returned
        Returns:
          list(str, ...): If section is not specified, a list containing all section names
          list(str, ...): If entry is not specified, a list containing all entry names for a given section
          str: If section and entry are specified, a value for entry
          None: if not successful
        '''
        import ConfigParser
        try:
            cp = ConfigParser.ConfigParser()
            cp.read(filename)
            if not section: return cp.sections()
            section = string.lower(section)
            if not entry: return cp.options(section)
            entry = string.lower(entry)
            return cp.get(section, entry)
        except IOError:
            return scriptcontext.errorhander()
        return scriptcontext.errorhandler()
    *)


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
    (*
    def Polar(point, angle_degrees, distance, plane=None):
        '''Returns 3D point that is a specified angle and distance from a 3D point
        Parameters:
          point (point): the point to transform
          angle_degrees(number,optional):angle in degrees
          distance(number,optional):distance from point
          plane (plane, optional): plane to base the transformation. If omitted, the world
            x-y plane is used
        Returns:
          point: resulting point is successful
          None: on error
        '''
        point = coerce3dpoint(point, True)
        angle = math.radians(angle_degrees)
        if plane: plane = coerceplane(plane)
        else: plane = Rhino.Geometry.Plane.WorldXY
        offset = plane.XAxis
        offset.Unitize()
        offset *= distance
        rc = point+offset
        xform = Rhino.Geometry.Transform.Rotation(angle, plane.ZAxis, point)
        rc.Transform(xform)
        return rc
    *)


    [<EXT>]
    ///<summary>Flattens an array of 3-D points into a one-dimensional list of real numbers. For example, if you had an array containing three 3-D points, this method would return a one-dimensional array containing nine real numbers.</summary>
    ///<param name="points">(Point3d seq) Points to flatten</param>
    ///<returns>(float array) A one-dimensional list containing real numbers, , otherwise None</returns>
    static member SimplifyArray(points:Point3d seq) : float array =
        [| for p in points do
                yield p.X
                yield p.Y
                yield p.Z |]
    (*
    def SimplifyArray(points):
        '''Flattens an array of 3-D points into a one-dimensional list of real numbers. For example, if you had an array containing three 3-D points, this method would return a one-dimensional array containing nine real numbers.
        Parameters:
          points ([point, ...]): Points to flatten
        Returns:
          list(number, ...): A one-dimensional list containing real numbers, if successful, otherwise None
        '''
        rc = []
        for point in points:
            point = coerce3dpoint(point, True)
            rc.append(point.X)
            rc.append(point.Y)
            rc.append(point.Z)
        return rc
    *)


    [<EXT>]
    ///<summary>Suspends execution of a running script for the specified interval. Then refreshes Rhino UI</summary>
    ///<param name="milliseconds">(int) Thousands of a second</param>
    ///<returns>(unit) </returns>
    static member Sleep(milliseconds:int) : unit =
        Threading.Thread.Sleep(milliseconds)
        RhinoApp.Wait()
    (*
    def Sleep(milliseconds):
        '''Suspends execution of a running script for the specified interval
        Parameters:
          milliseconds (number): thousands of a second
        Returns:
          None
        '''
        time.sleep( milliseconds / 1000.0 )
        Rhino.RhinoApp.Wait() #keep the message pump alive
    *)


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
    (*
    def SortPointList(points, tolerance=None):
        '''Sorts list of points so they will be connected in a "reasonable" polyline order
        Parameters:
          points ([point, ...])the points to sort
          tolerance (number, optional): minimum distance between points. Points that fall within this tolerance
            will be discarded. If omitted, Rhino's internal zero tolerance is used.
        Returns:
          list(point, ...): of sorted 3D points if successful
          None: on error
        '''
        points = coerce3dpointlist(points, True)
        if tolerance is None: tolerance = Rhino.RhinoMath.ZeroTolerance
        return list(Rhino.Geometry.Point3d.SortAndCullPointList(points, tolerance))
    *)


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
    (*
    def SortPoints(points, ascending=True, order=0):
        '''Sorts the components of an array of 3D points
        Parameters:
          points ([point, ...]): points to sort
          ascending (bool, optional): ascending if omitted (True) or True, descending if False.
          order (number, optional): the component sort order
            Value       Component Sort Order
            0 (default) X, Y, Z
            1           X, Z, Y
            2           Y, X, Z
            3           Y, Z, X
            4           Z, X, Y
            5           Z, Y, X
        Returns:
          list(point, ...): sorted 3-D points if successful
          None: if not successful
        '''
        def __cmpXYZ( a, b ):
            rc = cmp(a.X, b.X)
            if rc==0: rc = cmp(a.Y, b.Y)
            if rc==0: rc = cmp(a.Z, b.Z)
            return rc
        def __cmpXZY( a, b ):
            rc = cmp(a.X, b.X)
            if rc==0: rc = cmp(a.Z, b.Z)
            if rc==0: rc = cmp(a.Y, b.Y)
            return rc
        def __cmpYXZ( a, b ):
            rc = cmp(a.Y, b.Y)
            if rc==0: rc = cmp(a.X, b.X)
            if rc==0: rc = cmp(a.Z, b.Z)
            return rc
        def __cmpYZX( a, b ):
            rc = cmp(a.Y, b.Y)
            if rc==0: rc = cmp(a.Z, b.Z)
            if rc==0: rc = cmp(a.X, b.X)
            return rc
        def __cmpZXY( a, b ):
            rc = cmp(a.Z, b.Z)
            if rc==0: rc = cmp(a.X, b.X)
            if rc==0: rc = cmp(a.Y, b.Y)
            return rc
        def __cmpZYX( a, b ):
            rc = cmp(a.Z, b.Z)
            if rc==0: rc = cmp(a.Y, b.Y)
            if rc==0: rc = cmp(a.X, b.X)
            return rc
        sortfunc = (__cmpXYZ, __cmpXZY, __cmpYXZ, __cmpYZX, __cmpZXY, __cmpZYX)[order]
        return sorted(points, sortfunc, None, not ascending)
    *)


    [<EXT>]
    ///<summary>convert a formatted string value into a 3D point value</summary>
    ///<param name="point">(string) A string that contains a delimited point like "1,2,3".</param>
    ///<returns>(Point3d) Point structure from the input string.</returns>
    static member Str2Pt(point:string) : Point3d =
        RhinoScriptSyntax.Coerce3dPoint point
    (*
    def Str2Pt(point):
        '''convert a formatted string value into a 3D point value
        Parameters:
          point (str): A string that contains a delimited point like "1,2,3".
        Returns:
          point: Point structure from the input string.
          None: error on invalid format
        '''
        return coerce3dpoint(point, True)
    *)


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
    (*
    def CreatePoint(point, y=None, z=None):
        '''Converts 'point' into a Rhino.Geometry.Point3d if possible.
        If the provided object is already a point, it value is copied.
        In case the conversion fails, an error is raised.
        Alternatively, you can also pass two coordinates singularly for a
        point on the XY plane, or three for a 3D point.
        Parameters:
          point (Point3d|Vector3d|Point3f|Vector3f|str|guid| [number, number, number]])
          y (number,optional):y position
          z (number,optional):z position
        Returns:
          point: a Rhino.Geometry.Point3d. This can be seen as an object with three indices:
            [0]  X coordinate
            [1]  Y coordinate
            [2]  Z coordinate.
        '''
        if y is not None: return Rhino.Geometry.Point3d(float(point), float(y), float(z or 0.0))
        if type(point) is System.Drawing.Color: return Rhino.Geometry.Point3d(point)
        return coerce3dpoint(point, True)
    *)


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
   
    (*
    def CreateVector(vector, y=None, z=None):
        '''Converts 'vector' into a Rhino.Geometry.Vector3d if possible.
        If the provided object is already a vector, it value is copied.
        If the conversion fails, an error is raised.
        Alternatively, you can also pass two coordinates singularly for a
        vector on the XY plane, or three for a 3D vector.
        Parameters:
          vector (Vector3d|Point3d|Point3f|Vector3f\str|guid|[number, number, number])
          y (number,optional):y position
          z (number,optional):z position
        Returns:
          vector: a Rhino.Geometry.Vector3d. This can be seen as an object with three indices:
            result[0]: X component, result[1]: Y component, and result[2] Z component.
        '''
        if y is not None: return Rhino.Geometry.Vector3d(float(vector), float(y), float(z or 0.0))
        if type(vector) is Rhino.Geometry.Vector3d: return Rhino.Geometry.Vector3d(vector)
        return coerce3dvector(vector, True)
    *)


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
    (*
    def CreatePlane(plane_or_origin, x_axis=None, y_axis=None):
        '''Converts input into a Rhino.Geometry.Plane object if possible.
        If the provided object is already a plane, its value is copied.
        The returned data is accessible by indexing[origin, X axis, Y axis, Z axis], and that is the suggested method to interact with the type.
        The Z axis is in any case computed from the X and Y axes, so providing it is possible but not required.
        If the conversion fails, an error is raised.
        Parameters:
          plane_or_origin (plane|point|point, vector, vector|[point, vector, vector])
          x_axis (vector,optional):direction of X-Axis
          y_axis (vector,optional):direction of Y-Axis
        Returns:
          plane: A Rhino.Geometry.Plane
        '''
        if type(plane_or_origin) is Rhino.Geometry.Plane: return plane_or_origin.Clone()
        if x_axis != None:
            if y_axis == None: raise Exception("A value for the Y axis is expected if the X axis is specified.")
            origin = coerce3dpoint(plane_or_origin, True)
            x_axis = coerce3dvector(x_axis, True)
            y_axis = coerce3dvector(y_axis, True)
            return Rhino.Geometry.Plane(origin, x_axis, y_axis)
        return coerceplane(plane_or_origin, True)
    *)


    [<EXT>]
    ///<summary>Converts input into a Rhino.Geometry.Transform object if possible.</summary>
    ///<param name="xform">(seq<seq<float>>) The transform. This can be seen as a 4x4 matrix, given as nested lists.</param>
    ///<returns>(Transform) A Rhino.Geometry.Transform. result[0,3] gives access to the first row, last column.</returns>
    static member CreateXform(xform:seq<seq<float>>) : Transform =
        RhinoScriptSyntax.CoerceXform(xform) // TODO verify row, column order !!
    (*
    def CreateXform(xform):
        '''Converts input into a Rhino.Geometry.Transform object if possible.
        If the provided object is already a transform, its value is copied.
        The returned data is accessible by indexing[row, column], and that is the suggested method to interact with the type.
        If the conversion fails, an error is raised.
        Parameters:
          xform (nested list): the transform. This can be seen as a 4x4 matrix, given as nested lists or tuples.
        Returns:
          transform: A Rhino.Geometry.Transform. result[0,3] gives access to the first row, last column.
        '''
        if type(xform) is Rhino.Geometry.Transform: return xform.Clone()
        return coercexform(xform, True)
    *)


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
    (*
    def CreateColor(color, g=None, b=None, a=None):
        '''Converts 'color' into a native color object if possible.
        The returned data is accessible by indexing, and that is the suggested method to interact with the type.
        Red index is [0], Green index is [1], Blue index is [2] and Alpha index is [3].
        If the provided object is already a color, its value is copied.
        Alternatively, you can also pass three coordinates singularly for an RGB color, or four
        for an RGBA color point.
        Parameters:
          color ([number, number, number]): list or 3 or 4 items. Also, a single int can be passed and it will be bitwise-parsed.
          g (int,optional): green value
          b (int,optional): blue value
          a (int,optional): alpha value
        Returns:
          color: An object that can be indexed for red, green, blu, alpha. Item[0] is red.
        '''
        if g is not None and b is not None: return System.Drawing.Color.FromArgb(int(a or 255), int(color), int(g), int(b))
        if type(color) is System.Drawing.Color: return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)
        return coercecolor(color, True)
    *)


    [<EXT>]
    ///<summary>Converts input into a Rhino.Geometry.Interval.</summary>
    ///<param name="start">(float) The lower bound</param>
    ///<param name="ende">(float) Uper bound of interval</param>
    ///<returns>(Rhino.Geometry.Interval) This can be seen as an object made of two items:
    ///  [0] start of interval
    ///  [1] end of interval</returns>
    static member CreateInterval(start:float,ende:float) : Rhino.Geometry.Interval =
        Geometry.Interval(start , ende)
    (*
    def CreateInterval(interval, y=None):
        '''Converts 'interval' into a Rhino.Geometry.Interval.
        If the provided object is already an interval, its value is copied.
        In case the conversion fails, an error is raised.
        In case a single number is provided, it will be translated to an increasing interval that includes
        the provided input and 0. If two values are provided, they will be used instead.
        Parameters:
          interval ([number, number]): or any item that can be accessed at index 0 and 1; an Interval or just the lower bound
          y (number,optional): uper bound of interval
        Returns:
          a Rhino.Geometry.Interval: This can be seen as an object made of two items:
            [0] start of interval
            [1] end of interval
        '''
        if y is not None: return Rhino.Geometry.Interval(float(interval), float(y))
        if isinstance(interval, numbers.Number):
            return Rhino.Geometry.Interval(interval if interval < 0 else 0, interval if interval > 0 else 0)
        try:
            return Rhino.Geometry.Interval(interval[0], interval[1])
        except:
            raise ValueError("unable to convert %s into an Interval: it cannot be indexed."%interval)
    *)
