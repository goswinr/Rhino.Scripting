namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsDocument =
  type RhinoScriptSyntax with
    
    ///<summary>Create a bitmap preview image of the current model</summary>
    ///<param name="filename">(string) Name of the bitmap file to create</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title of the view. If omitted, the active view is used</param>
    ///<param name="size">(float) Optional, Default Value: <c>null</c>
    ///Two integers that specify width and height of the bitmap</param>
    ///<param name="flags">(int) Optional, Default Value: <c>0</c>
    ///Bitmap creation flags. Can be the combination of:
    ///  1 = honor object highlighting
    ///  2 = draw construction plane
    ///  4 = use ghosted shading</param>
    ///<param name="wireframe">(bool) Optional, Default Value: <c>false</c>
    ///If True then a wireframe preview image. If False,
    ///  a rendered image will be created</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CreatePreviewImage(filename:string, [<OPT;DEF(null)>]view:string, [<OPT;DEF(null)>]size:float, [<OPT;DEF(0)>]flags:int, [<OPT;DEF(false)>]wireframe:bool) : bool =
        failNotImpl () // done in 2018


    ///<summary>Returns the document's modified flag. This flag indicates whether
    /// or not any changes to the current document have been made. NOTE: setting the
    /// document modified flag to False will prevent the "Do you want to save this
    /// file..." from displaying when you close Rhino.</summary>
    ///<returns>(bool) if no modified state is specified, the current modified state</returns>
    static member DocumentModified() : bool =
        failNotImpl () // done in 2018

    ///<summary>Sets the document's modified flag. This flag indicates whether
    /// or not any changes to the current document have been made. NOTE: setting the
    /// document modified flag to False will prevent the "Do you want to save this
    /// file..." from displaying when you close Rhino.</summary>
    ///<param name="modified">(bool)The modified state, either True or False</param>
    ///<returns>(unit) unit</returns>
    static member DocumentModified(modified:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the name of the currently loaded Rhino document (3DM file)</summary>
    ///<returns>(string) the name of the currently loaded Rhino document (3DM file)</returns>
    static member DocumentName() : string =
        failNotImpl () // done in 2018


    ///<summary>Returns path of the currently loaded Rhino document (3DM file)</summary>
    ///<returns>(string) the path of the currently loaded Rhino document (3DM file)</returns>
    static member DocumentPath() : string =
        failNotImpl () // done in 2018


    ///<summary>Enables or disables screen redrawing</summary>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///True to enable, False to disable</param>
    ///<returns>(unit) unit</returns>
    static member EnableRedraw([<OPT;DEF(true)>]enable:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Extracts the bitmap preview image from the specified model (.3dm)</summary>
    ///<param name="filename">(string) Name of the bitmap file to create. The extension of
    ///  the filename controls the format of the bitmap file created.
    ///  (.bmp, .tga, .jpg, .jpeg, .pcx, .png, .tif, .tiff)</param>
    ///<param name="modelname">(string) Optional, Default Value: <c>null</c>
    ///The model (.3dm) from which to extract the
    ///  preview image. If omitted, the currently loaded model is used.</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ExtractPreviewImage(filename:string, [<OPT;DEF(null)>]modelname:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies that the current document has been modified in some way</summary>
    ///<returns>(bool) True or False.</returns>
    static member IsDocumentModified() : bool =
        failNotImpl () // done in 2018


    ///<summary>Returns the document's notes. Notes are generally created
    /// using Rhino's Notes command</summary>
    ///<returns>(string) if `newnotes` is omitted, the current notes</returns>
    static member Notes() : string =
        failNotImpl () // done in 2018

    ///<summary>Sets the document's notes. Notes are generally created
    /// using Rhino's Notes command</summary>
    ///<param name="newnotes">(string)New notes to set</param>
    ///<returns>(unit) unit</returns>
    static member Notes(newnotes:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the file version of the current document. Use this function to
    ///  determine which version of Rhino last saved the document. Note, this
    ///  function will not return values from referenced or merged files.</summary>
    ///<returns>(string) the file version of the current document</returns>
    static member ReadFileVersion() : string =
        failNotImpl () // done in 2018


    ///<summary>Redraws all views</summary>
    ///<returns>(unit) </returns>
    static member Redraw() : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns render antialiasing style</summary>
    ///<returns>(float) The current antialiasing style</returns>
    static member RenderAntialias() : float =
        failNotImpl () // done in 2018

    ///<summary>Sets render antialiasing style</summary>
    ///<param name="style">(int)Level of antialiasing (0=none, 1=normal, 2=best)</param>
    ///<returns>(unit) unit</returns>
    static member RenderAntialias(style:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render ambient light or background color</summary>
    ///<param name="item">(int) 0=ambient light color, 1=background color</param>
    ///<returns>(int) The current item color
    ///0=ambient light color, 1=background color</returns>
    static member RenderColor(item:int) : int =
        failNotImpl () // done in 2018

    ///<summary>Sets the render ambient light or background color</summary>
    ///<param name="item">(int) 0=ambient light color, 1=background color</param>
    ///<param name="color">(Drawing.Color)The new color value. If omitted, the current item color is returned</param>
    ///<returns>(unit) unit</returns>
    static member RenderColor(item:int, color:Drawing.Color) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render resolution</summary>
    ///<returns>(float * float) The current resolution width,height</returns>
    static member RenderResolution() : float * float =
        failNotImpl () // done in 2018

    ///<summary>Sets the render resolution</summary>
    ///<param name="resolution">(float * float)Width and height of render</param>
    ///<returns>(unit) unit</returns>
    static member RenderResolution(resolution:float * float) : unit =
        failNotImpl () // done in 2018


    
    static member internal SetRenderMeshAndUpdateStyle() : obj =
        failNotImpl () // done in 2018


    ///<summary>Returns the render mesh density property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh density .</returns>
    static member RenderMeshDensity() : float =
        failNotImpl () // done in 2018

    ///<summary>Sets the render mesh density property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="density">(float)The new render mesh density, which is a number between 0.0 and 1.0.</param>
    ///<returns>(unit) unit</returns>
    static member RenderMeshDensity(density:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render mesh maximum angle property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current maximum angle .</returns>
    static member RenderMeshMaxAngle() : float =
        failNotImpl () // done in 2018

    ///<summary>Sets the render mesh maximum angle property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="angleDegrees">(int)The new maximum angle, which is a positive number in degrees.</param>
    ///<returns>(unit) unit</returns>
    static member RenderMeshMaxAngle(angleDegrees:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render mesh maximum aspect ratio property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh maximum aspect ratio .</returns>
    static member RenderMeshMaxAspectRatio() : float =
        failNotImpl () // done in 2018

    ///<summary>Sets the render mesh maximum aspect ratio property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="ratio">(float)The render mesh maximum aspect ratio.  The suggested range, when not zero, is from 1 to 100.</param>
    ///<returns>(unit) unit</returns>
    static member RenderMeshMaxAspectRatio(ratio:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render mesh maximum distance, edge to surface parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh maximum distance, edge to surface .</returns>
    static member RenderMeshMaxDistEdgeToSrf() : float =
        failNotImpl () // done in 2018

    ///<summary>Sets the render mesh maximum distance, edge to surface parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="distance">(float)The render mesh maximum distance, edge to surface.</param>
    ///<returns>(unit) unit</returns>
    static member RenderMeshMaxDistEdgeToSrf(distance:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render mesh maximum edge length parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh maximum edge length .</returns>
    static member RenderMeshMaxEdgeLength() : float =
        failNotImpl () // done in 2018

    ///<summary>Sets the render mesh maximum edge length parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="distance">(float)The render mesh maximum edge length.</param>
    ///<returns>(unit) unit</returns>
    static member RenderMeshMaxEdgeLength(distance:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render mesh minimum edge length parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh minimum edge length .</returns>
    static member RenderMeshMinEdgeLength() : float =
        failNotImpl () // done in 2018

    ///<summary>Sets the render mesh minimum edge length parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="distance">(float)The render mesh minimum edge length.</param>
    ///<returns>(unit) unit</returns>
    static member RenderMeshMinEdgeLength(distance:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render mesh minimum initial grid quads parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh minimum initial grid quads .</returns>
    static member RenderMeshMinInitialGridQuads() : float =
        failNotImpl () // done in 2018

    ///<summary>Sets the render mesh minimum initial grid quads parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="quads">(float)The render mesh minimum initial grid quads. The suggested range is from 0 to 10000.</param>
    ///<returns>(unit) unit</returns>
    static member RenderMeshMinInitialGridQuads(quads:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render mesh quality of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(int) The current render mesh quality .
    ///  0: Jagged and faster.  Objects may look jagged, but they should shade and render relatively quickly.
    ///  1: Smooth and slower.  Objects should look smooth, but they may take a very long time to shade and render.
    ///  2: Custom.</returns>
    static member RenderMeshQuality() : int =
        failNotImpl () // done in 2018

    ///<summary>Sets the render mesh quality of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="quality">(float)The render mesh quality, either:
    ///  0: Jagged and faster.  Objects may look jagged, but they should shade and render relatively quickly.
    ///  1: Smooth and slower.  Objects should look smooth, but they may take a very long time to shade and render.
    ///  2: Custom.</param>
    ///<returns>(unit) unit</returns>
    static member RenderMeshQuality(quality:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the render mesh settings of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(int) The current render mesh settings .
    ///    0: No settings enabled.
    ///    1: Refine mesh enabled.
    ///    2: Jagged seams enabled.
    ///    4: Simple planes enabled.
    ///    8: Texture is packed, scaled and normalized; otherwise unpacked, unscaled and normalized.</returns>
    static member RenderMeshSettings() : int =
        failNotImpl () // done in 2018

    ///<summary>Sets the render mesh settings of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="settings">(int)The render mesh settings, which is a bit-coded number that allows or disallows certain features.
    ///  The bits can be added together in any combination to form a value between 0 and 7.  The bit values are as follows:
    ///    0: No settings enabled.
    ///    1: Refine mesh enabled.
    ///    2: Jagged seams enabled.
    ///    4: Simple planes enabled.
    ///    8: Texture is packed, scaled and normalized; otherwise unpacked, unscaled and normalized.</param>
    ///<returns>(unit) unit</returns>
    static member RenderMeshSettings(settings:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns render settings</summary>
    ///<returns>(int) if settings are not specified, the current render settings in bit-coded flags
    ///  0=none,
    ///  1=create shadows,
    ///  2=use lights on layers that are off,
    ///  4=render curves and isocurves,
    ///  8=render dimensions and text</returns>
    static member RenderSettings() : int =
        failNotImpl () // done in 2018

    ///<summary>Sets render settings</summary>
    ///<param name="settings">(int)Bit-coded flags of render settings to modify.
    ///  0=none,
    ///  1=create shadows,
    ///  2=use lights on layers that are off,
    ///  4=render curves and isocurves,
    ///  8=render dimensions and text</param>
    ///<returns>(unit) unit</returns>
    static member RenderSettings(settings:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the document's absolute tolerance. Absolute tolerance
    /// is measured in drawing units. See Rhino's document properties command
    /// (Units and Page Units Window) for details</summary>
    ///<returns>(float) The current absolute tolerance</returns>
    static member UnitAbsoluteTolerance() : float =
        failNotImpl () // done in 2018

    ///<summary>Sets the document's absolute tolerance. Absolute tolerance
    /// is measured in drawing units. See Rhino's document properties command
    /// (Units and Page Units Window) for details</summary>
    ///<param name="tolerance">(float)The absolute tolerance to set</param>
    ///<param name="inModelUnits">(bool)Return or modify the document's model units (True)
    ///  or the document's page units (False)</param>
    ///<returns>(unit) unit</returns>
    static member UnitAbsoluteTolerance(tolerance:float, [<OPT;DEF(true)>]inModelUnits:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Return the document's angle tolerance. Angle tolerance is
    /// measured in degrees. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details</summary>
    ///<returns>(float) The current angle tolerance</returns>
    static member UnitAngleTolerance() : float =
        failNotImpl () // done in 2018

    ///<summary>Set the document's angle tolerance. Angle tolerance is
    /// measured in degrees. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details</summary>
    ///<param name="angleToleranceDegrees">(float)The angle tolerance to set</param>
    ///<param name="inModelUnits">(float)Return or modify the document's model units (True)
    ///  or the document's page units (False)</param>
    ///<returns>(unit) unit</returns>
    static member UnitAngleTolerance(angleToleranceDegrees:float, [<OPT;DEF(true)>]inModelUnits:float) : unit =
        failNotImpl () // done in 2018


    ///<summary>Return the document's distance display precision</summary>
    ///<returns>(float) The current distance display precision .</returns>
    static member UnitDistanceDisplayPrecision() : float =
        failNotImpl () // done in 2018

    ///<summary>Set the document's distance display precision</summary>
    ///<param name="precision">(float)The distance display precision.  If the current distance display mode is Decimal, then precision is the number of decimal places.
    ///  If the current distance display mode is Fractional (including Feet and Inches), then the denominator = (1/2)^precision.
    ///  Use UnitDistanceDisplayMode to get the current distance display mode.</param>
    ///<param name="modelUnits">(bool)Return or modify the document's model units (True) or the document's page units (False). The default is True.</param>
    ///<returns>(unit) unit</returns>
    static member UnitDistanceDisplayPrecision(precision:float, [<OPT;DEF(true)>]modelUnits:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Return the document's relative tolerance. Relative tolerance
    /// is measured in percent. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details</summary>
    ///<returns>(float) The current tolerance in percent</returns>
    static member UnitRelativeTolerance() : float =
        failNotImpl () // done in 2018

    ///<summary>Set the document's relative tolerance. Relative tolerance
    /// is measured in percent. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details</summary>
    ///<param name="relativeTolerance">(float)The relative tolerance in percent</param>
    ///<param name="inModelUnits">(bool)Return or modify the document's model units (True)
    ///  or the document's page units (False)</param>
    ///<returns>(unit) unit</returns>
    static member UnitRelativeTolerance(relativeTolerance:float, [<OPT;DEF(true)>]inModelUnits:bool) : unit =
        failNotImpl () // done in 2018


    ///<summary>Return the scale factor for changing between unit systems.</summary>
    ///<param name="toSystem">(int) The unit system to convert to. The unit systems are are:
    ///  0 - No unit system
    ///  1 - Microns (1.0e-6 meters)
    ///  2 - Millimeters (1.0e-3 meters)
    ///  3 - Centimeters (1.0e-2 meters)
    ///  4 - Meters
    ///  5 - Kilometers (1.0e+3 meters)
    ///  6 - Microinches (2.54e-8 meters, 1.0e-6 inches)
    ///  7 - Mils (2.54e-5 meters, 0.001 inches)
    ///  8 - Inches (0.0254 meters)
    ///  9 - Feet (0.3408 meters, 12 inches)
    ///    10 - Miles (1609.344 meters, 5280 feet)
    ///    11 - *Reserved for custom Unit System*
    ///    12 - Angstroms (1.0e-10 meters)
    ///    13 - Nanometers (1.0e-9 meters)
    ///    14 - Decimeters (1.0e-1 meters)
    ///    15 - Dekameters (1.0e+1 meters)
    ///    16 - Hectometers (1.0e+2 meters)
    ///    17 - Megameters (1.0e+6 meters)
    ///    18 - Gigameters (1.0e+9 meters)
    ///    19 - Yards (0.9144  meters, 36 inches)
    ///    20 - Printer point (1/72 inches, computer points)
    ///    21 - Printer pica (1/6 inches, (computer picas)
    ///    22 - Nautical mile (1852 meters)
    ///    23 - Astronomical (1.4959787e+11)
    ///    24 - Lightyears (9.46073e+15 meters)
    ///    25 - Parsecs (3.08567758e+16)</param>
    ///<param name="fromSystem">(int) Optional, Default Value: <c>null</c>
    ///The unit system to convert from (see above). If omitted,
    ///  the document's current unit system is used</param>
    ///<returns>(float) scale factor for changing between unit systems</returns>
    static member UnitScale(toSystem:int, [<OPT;DEF(null)>]fromSystem:int) : float =
        failNotImpl () // done in 2018


    ///<summary>Return the document's unit system. See Rhino's DocumentProperties
    /// command (Units and Page Units Window) for details</summary>
    ///<returns>(int) The current unit system
    ///  0 - No unit system
    ///  1 - Microns (1.0e-6 meters)
    ///  2 - Millimeters (1.0e-3 meters)
    ///  3 - Centimeters (1.0e-2 meters)
    ///  4 - Meters
    ///  5 - Kilometers (1.0e+3 meters)
    ///  6 - Microinches (2.54e-8 meters, 1.0e-6 inches)
    ///  7 - Mils (2.54e-5 meters, 0.001 inches)
    ///  8 - Inches (0.0254 meters)
    ///  9 - Feet (0.3408 meters, 12 inches)
    ///    10 - Miles (1609.344 meters, 5280 feet)
    ///    11 - *Reserved for custom Unit System*
    ///    12 - Angstroms (1.0e-10 meters)
    ///    13 - Nanometers (1.0e-9 meters)
    ///    14 - Decimeters (1.0e-1 meters)
    ///    15 - Dekameters (1.0e+1 meters)
    ///    16 - Hectometers (1.0e+2 meters)
    ///    17 - Megameters (1.0e+6 meters)
    ///    18 - Gigameters (1.0e+9 meters)
    ///    19 - Yards (0.9144  meters, 36 inches)
    ///    20 - Printer point (1/72 inches, computer points)
    ///    21 - Printer pica (1/6 inches, (computer picas)
    ///    22 - Nautical mile (1852 meters)
    ///    23 - Astronomical (1.4959787e+11)
    ///    24 - Lightyears (9.46073e+15 meters)
    ///    25 - Parsecs (3.08567758e+16)</returns>
    static member UnitSystem() : int =
        failNotImpl () // done in 2018

    ///<summary>Set the document's unit system. See Rhino's DocumentProperties
    /// command (Units and Page Units Window) for details</summary>
    ///<param name="unitSystem">(int)The unit system to set the document to. The unit systems are:
    ///  0 - No unit system
    ///  1 - Microns (1.0e-6 meters)
    ///  2 - Millimeters (1.0e-3 meters)
    ///  3 - Centimeters (1.0e-2 meters)
    ///  4 - Meters
    ///  5 - Kilometers (1.0e+3 meters)
    ///  6 - Microinches (2.54e-8 meters, 1.0e-6 inches)
    ///  7 - Mils (2.54e-5 meters, 0.001 inches)
    ///  8 - Inches (0.0254 meters)
    ///  9 - Feet (0.3408 meters, 12 inches)
    ///    10 - Miles (1609.344 meters, 5280 feet)
    ///    11 - *Reserved for custom Unit System*
    ///    12 - Angstroms (1.0e-10 meters)
    ///    13 - Nanometers (1.0e-9 meters)
    ///    14 - Decimeters (1.0e-1 meters)
    ///    15 - Dekameters (1.0e+1 meters)
    ///    16 - Hectometers (1.0e+2 meters)
    ///    17 - Megameters (1.0e+6 meters)
    ///    18 - Gigameters (1.0e+9 meters)
    ///    19 - Yards (0.9144  meters, 36 inches)
    ///    20 - Printer point (1/72 inches, computer points)
    ///    21 - Printer pica (1/6 inches, (computer picas)
    ///    22 - Nautical mile (1852 meters)
    ///    23 - Astronomical (1.4959787e+11)
    ///    24 - Lightyears (9.46073e+15 meters)
    ///    25 - Parsecs (3.08567758e+16)</param>
    ///<param name="scale">(bool)Scale existing geometry based on the new unit system.
    ///  If not specified, any existing geometry is not scaled (False)</param>
    ///<param name="inModelUnits">(int)Return or modify the document's model units (True)
    ///  or the document's page units (False). The default is True.</param>
    ///<returns>(unit) unit</returns>
    static member UnitSystem(unitSystem:int, [<OPT;DEF(false)>]scale:bool, [<OPT;DEF(true)>]inModelUnits:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the name of the current unit system</summary>
    ///<param name="capitalize">(bool) Optional, Default Value: <c>false</c>
    ///Capitalize the first character of the units system name (e.g. return "Millimeter" instead of "millimeter"). The default is not to capitalize the first character (false).</param>
    ///<param name="singular">(bool) Optional, Default Value: <c>true</c>
    ///Return the singular form of the units system name (e.g. "millimeter" instead of "millimeters"). The default is to return the singular form of the name (true).</param>
    ///<param name="abbreviate">(bool) Optional, Default Value: <c>false</c>
    ///Abbreviate the name of the units system (e.g. return "mm" instead of "millimeter"). The default is not to abbreviate the name (false).</param>
    ///<param name="modelUnits">(bool) Optional, Default Value: <c>true</c>
    ///Return the document's model units (True) or the document's page units (False). The default is True.</param>
    ///<returns>(string) The name of the current units system .</returns>
    static member UnitSystemName([<OPT;DEF(false)>]capitalize:bool, [<OPT;DEF(true)>]singular:bool, [<OPT;DEF(false)>]abbreviate:bool, [<OPT;DEF(true)>]modelUnits:bool) : string =
        failNotImpl () // done in 2018


