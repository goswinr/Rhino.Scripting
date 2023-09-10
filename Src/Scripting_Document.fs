
namespace Rhino.Scripting 

open Rhino 

open System
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore

[<AutoOpen>]
module AutoOpenDocument =
  type RhinoScriptSyntax with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>Create a bitmap preview image of the current model.</summary>
    ///<param name="fileName">(string) Name of the bitmap file to create</param>
    ///<param name="view">(string) Optional, Title of the view. If omitted, the active view is used</param>
    ///<param name="width">(int) Optional, default value: <c>0</c>
    /// integer that specifies width of the bitmap in pixel. if only width given height will be scaled to keep screen ratio</param>
    ///<param name="height">(int) Optional, default value: <c>0</c>
    /// integer that specifies height of the bitmap in pixel. if only height given width will be scaled to keep screen ratio</param>
    ///<param name="flags">(int) Optional, default value: <c>0</c>
    ///    Bitmap creation flags. Can be the combination of:
    ///    1 = honor object highlighting
    ///    2 = draw construction Plane
    ///    4 = use ghosted shading</param>
    ///<param name="wireframe">(bool) Optional, default value: <c>false</c>
    ///    If True then a wire-frame preview image. If False,
    ///    a rendered image will be created</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member CreatePreviewImage(   fileName:string,
                                        [<OPT;DEF("")>]view:string,
                                        [<OPT;DEF(0)>]width:int,
                                        [<OPT;DEF(0)>]height:int,
                                        [<OPT;DEF(0)>]flags:int,
                                        [<OPT;DEF(false)>]wireframe:bool) : bool = 
        let rhview = RhinoScriptSyntax.CoerceView(view)
        let inline  ( ./. ) (i:int) (j:int) = (float(i)) / (float(j))
        let inline  ( *. ) ( i:int) (f:float) = int(round(float(i) * f))
        let rhsize = 
            match width, height with
            | 0, 0 -> rhview.ClientRectangle.Size
            | x, 0 ->
                let sc = x ./. rhview.ClientRectangle.Size.Width
                Drawing.Size(x, rhview.ClientRectangle.Size.Height *. sc)
            | 0, y ->
                let sc = y ./. rhview.ClientRectangle.Size.Height
                Drawing.Size(rhview.ClientRectangle.Size.Width *. sc , y)
            | x, y -> Drawing.Size(x, y)
        let ignoreHighlights =  (flags &&& 1) <> 1
        let drawcplane =        (flags &&& 2)  = 2
        let useghostedshading = (flags &&& 4)  = 4
        if wireframe then
            rhview.CreateWireframePreviewImage(fileName, rhsize, ignoreHighlights, drawcplane)
        else
            rhview.CreateShadedPreviewImage(fileName, rhsize, ignoreHighlights, drawcplane, useghostedshading)


    ///<summary>Returns the document's modified flag. This flag indicates whether
    /// or not any changes to the current document have been made. NOTE: setting the
    /// document modified flag to False will prevent the "Do you want to save this
    /// file..." from displaying when you close Rhino.</summary>
    ///<returns>(bool) if no modified state is specified, the current modified state.</returns>
    static member DocumentModified() : bool = //GET
        State.Doc.Modified

    ///<summary>Sets the document's modified flag. This flag indicates whether
    /// or not any changes to the current document have been made. NOTE: setting the
    /// document modified flag to False will prevent the "Do you want to save this
    /// file..." from displaying when you close Rhino.</summary>
    ///<param name="modified">(bool) The modified state, either True or False</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DocumentModified(modified:bool) : unit = //SET
        State.Doc.Modified <- modified



    ///<summary>Returns the name of the currently loaded Rhino document (3dm file).</summary>
    ///<returns>(string) The name of the currently loaded Rhino document (3dm file).</returns>
    static member DocumentName() : string = 
        State.Doc.Name |? ""



    ///<summary>Returns full path of the currently loaded Rhino document including the file name (3dm file).</summary>
    ///<returns>(string) The path of the currently loaded Rhino document  including the file name(3dm file).</returns>
    static member DocumentPath() : string = 
        let p = State.Doc.Path
        if isNull p then ""
        else p
            //let slash = string Path.DirectorySeparatorChar
            //if p.EndsWith slash then p
            //else p + slash // add \ or / at the ende to be consistent with RhinoScript



    ///<summary>Enables or disables screen redrawing.
    ///  All UI interacting functions (such as rs.GetObject) of Rhino.Scripting
    ///  will automatically enable redraw if needed
    ///  and afterwards disable it again if it was disabled before.
    ///  At the end of a script run in Seff Editor Redraw will be automatically enabled again.</summary>
    ///<param name="enable">(bool) Optional, default value: <c>true</c>
    ///    True to enable, False to disable</param>
    ///<returns>(unit) void, nothing.</returns>
    static member EnableRedraw([<OPT;DEF(true)>]enable:bool) : unit = 
        State.Doc.Views.RedrawEnabled <- enable

    ///<summary>Disables screen redrawing.
    ///  All UI interacting functions (such as rs.GetObject) of Rhino.Scripting
    ///  will automatically enable redraw if needed
    ///  and afterwards disable it again if it was disabled before.
    ///  At the end of a script run in Seff Editor Redraw will be automatically enabled again.</summary>
    ///<returns>(unit) void, nothing.</returns>
    static member DisableRedraw() : unit = 
        State.Doc.Views.RedrawEnabled <- false


    ///<summary>Extracts the bitmap preview image from the specified model (.3dm).</summary>
    ///<param name="fileName">(string) Name of the bitmap file to create. The extension of
    ///    the fileName controls the format of the bitmap file created.
    ///    (.bmp, .tga, .jpg, .jpeg, .pcx, .png, .tif, .tiff)</param>
    ///<param name="modelName">(string) Optional, The model (.3dm) from which to extract the
    ///    preview image. If omitted, the currently loaded model is used</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ExtractPreviewImage(fileName:string, [<OPT;DEF(null:string)>]modelName:string) : unit = 
        let bmp = 
            if notNull modelName  then
                if notNull State.Doc.Path then RhinoDoc.ExtractPreviewImage(State.Doc.Path) // TODO test this works ok
                else RhinoScriptingException.Raise "RhinoScriptSyntax.ExtractPreviewImage failed on unsaved file"
            else
                RhinoDoc.ExtractPreviewImage(modelName)
        bmp.Save(fileName)


    ///<summary>Checks if the current document has been modified in some way.</summary>
    ///<returns>(bool) True or False.</returns>
    static member IsDocumentModified() : bool = 
        State.Doc.Modified


    ///<summary>Returns the document's notes. Notes are generally created
    /// using Rhino's Notes command.</summary>
    ///<returns>(string) The current notes.</returns>
    static member Notes() : string = //GET
        State.Doc.Notes

    ///<summary>Sets the document's notes. Notes are generally created
    /// using Rhino's Notes command.</summary>
    ///<param name="newNotes">(string) New notes to set</param>
    ///<returns>(unit) void, nothing.</returns>
    static member Notes(newNotes:string) : unit = //SET
        State.Doc.Notes <- newNotes



    ///<summary>Returns the file version of the current document. Use this function to
    ///    determine which version of Rhino last saved the document. Note, this
    ///    function will not return values from referenced or merged files.</summary>
    ///<returns>(int) The file version of the current document.</returns>
    static member ReadFileVersion() : int = 
        State.Doc.ReadFileVersion()


    ///<summary>Redraws all views.</summary>
    ///<returns>(unit).</returns>
    static member Redraw() : unit = 
        let old = State.Doc.Views.RedrawEnabled
        State.Doc.Views.RedrawEnabled <- true
        State.Doc.Views.Redraw()
        RhinoApp.Wait()
        State.Doc.Views.RedrawEnabled <- old


    ///<summary>Returns render antialiasing style.</summary>
    ///<returns>(int) The current antialiasing style (0 = none, 1 = normal, 2 = best).</returns>
    static member RenderAntialias() : int = //GET
        int(State.Doc.RenderSettings.AntialiasLevel) // TODO check

    ///<summary>Sets render antialiasing style.</summary>
    ///<param name="style">(int) Level of antialiasing (0 = none, 1 = normal, 2 = best)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderAntialias(style:int) : unit = //SET
        if style = 0 || style = 1 || style = 2 then
            let settings = State.Doc.RenderSettings
            settings.AntialiasLevel <- EnumOfValue (style)
            State.Doc.RenderSettings <- settings


    ///<summary>Returns the render ambient light or background color.</summary>
    ///<param name="item">(int)
    ///   0 = ambient light color,
    ///   1 = background color</param>
    ///<returns>(Drawing.Color) The current item color.</returns>
    static member RenderColor(item:int) : Drawing.Color = //GET
        if item<>0 && item<>1 then  RhinoScriptingException.Raise "RhinoScriptSyntax.RenderColor Item must be 0 or 1.  item:'%A'" item
        if item = 0 then  State.Doc.RenderSettings.AmbientLight
        else State.Doc.RenderSettings.BackgroundColorTop

    ///<summary>Sets the render ambient light or background color.</summary>
    ///<param name="item">(int)
    ///    0 = ambient light color,
    ///    1 = background color</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderColor(item:int, color:Drawing.Color) : unit = //SET
        if item<>0 && item<>1 then  RhinoScriptingException.Raise "RhinoScriptSyntax.RenderColor Item must be 0 || 1.  item:'%A' color:'%A'" item color
        let settings = State.Doc.RenderSettings
        if item = 0 then  settings.AmbientLight <- color
        else            settings.BackgroundColorTop <- color
        State.Doc.RenderSettings <- settings
        State.Doc.Views.Redraw()


    ///<summary>Returns the render resolution.</summary>
    ///<returns>(int * int) The current resolution width, height.</returns>
    static member RenderResolution() : int * int = //GET
        let rc = State.Doc.RenderSettings.ImageSize
        rc.Width, rc.Height

    ///<summary>Sets the render resolution.</summary>
    ///<param name="width">(int) width and height of render</param>
    ///<param name="height">(int) height of render</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderResolution(width:int, height:int) : unit = //SET
            let settings = State.Doc.RenderSettings
            settings.ImageSize <- Drawing.Size(width , height)
            State.Doc.RenderSettings <- settings



    ///<summary>Returns the render Mesh density property of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render Mesh density.</returns>
    static member RenderMeshDensity() : float = //GET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        current.RelativeTolerance

    ///<summary>Sets the render Mesh density property of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="density">(float) The new render Mesh density, which is a number between 0.0 and 1.0</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderMeshDensity(density:float) : unit = //SET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        if RhinoMath.Clamp(density, 0.0, 1.0) = density then
            current.RelativeTolerance <- density
            State.Doc.SetCustomMeshingParameters(current)
            State.Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom


    ///<summary>Returns the render Mesh maximum angle property of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current maximum angle.</returns>
    static member RenderMeshMaxAngle() : float = //GET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        toDegrees(current.RefineAngle)

    ///<summary>Sets the render Mesh maximum angle property of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="angleDegrees">(float) The new maximum angle, which is a positive number in degrees</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderMeshMaxAngle(angleDegrees:float) : unit = //SET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        if angleDegrees > 0. then
                current.RefineAngle <- toRadians(angleDegrees)
                State.Doc.SetCustomMeshingParameters(current)
                State.Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom


    ///<summary>Returns the render Mesh maximum aspect ratio property of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render Mesh maximum aspect ratio.</returns>
    static member RenderMeshMaxAspectRatio() : float = //GET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        let rc = current.GridAspectRatio
        rc

    ///<summary>Sets the render Mesh maximum aspect ratio property of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="ratio">(float) The render Mesh maximum aspect ratio. The suggested range, when not zero, is from 1 to 100</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderMeshMaxAspectRatio(ratio:float) : unit = //SET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        if ratio <> 0.0 then
            current.GridAspectRatio <- ratio
            State.Doc.SetCustomMeshingParameters(current)
            State.Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom


    ///<summary>Returns the render Mesh maximum distance, edge to Surface parameter of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render Mesh maximum distance, edge to Surface.</returns>
    static member RenderMeshMaxDistEdgeToSrf() : float = //GET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        let rc = current.Tolerance
        rc

    ///<summary>Sets the render Mesh maximum distance, edge to Surface parameter of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="distance">(float) The render Mesh maximum distance, edge to Surface</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderMeshMaxDistEdgeToSrf(distance:float) : unit = //SET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        if distance > 0. then
            current.Tolerance <- distance
            State.Doc.SetCustomMeshingParameters(current)
            State.Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom


    ///<summary>Returns the render Mesh maximum edge length parameter of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render Mesh maximum edge length.</returns>
    static member RenderMeshMaxEdgeLength() : float = //GET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        let rc = current.MaximumEdgeLength
        rc

    ///<summary>Sets the render Mesh maximum edge length parameter of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="distance">(float) The render Mesh maximum edge length</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderMeshMaxEdgeLength(distance:float) : unit = //SET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        if distance > 0.0 then
            current.MaximumEdgeLength <- distance
            State.Doc.SetCustomMeshingParameters(current)
            State.Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom


    ///<summary>Returns the render Mesh minimum edge length parameter of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render Mesh minimum edge length.</returns>
    static member RenderMeshMinEdgeLength() : float = //GET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        let rc = current.MinimumEdgeLength
        rc

    ///<summary>Sets the render Mesh minimum edge length parameter of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="distance">(float) The render Mesh minimum edge length</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderMeshMinEdgeLength(distance:float) : unit = //SET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        let rc = current.MinimumEdgeLength
        if distance > 0.0 then
            current.MinimumEdgeLength <- distance
            State.Doc.SetCustomMeshingParameters(current)
            State.Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom


    ///<summary>Returns the render Mesh minimum initial grid quads parameter of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(int) The current render Mesh minimum initial grid quads.</returns>
    static member RenderMeshMinInitialGridQuads() : int = //GET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        current.GridMinCount


    ///<summary>Sets the render Mesh minimum initial grid quads parameter of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="quads">(int) The render Mesh minimum initial grid quads. The suggested range is from 0 to 10000</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderMeshMinInitialGridQuads(quads:int) : unit = //SET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        let rc = current.GridMinCount
        if quads > 0 then
            current.GridMinCount <- quads
            State.Doc.SetCustomMeshingParameters(current)
            State.Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom


    ///<summary>Returns the render Mesh quality of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(int) The current render Mesh quality .
    ///    0: Jagged and faster. Objects may look jagged, but they should shade and render relatively quickly.
    ///    1: Smooth and slower. Objects should look smooth, but they may take a very long time to shade and render.
    ///    2: Custom.</returns>
    static member RenderMeshQuality() : int = //GET
        let current = State.Doc.MeshingParameterStyle
        if current = MeshingParameterStyle.Fast then 0
        elif current = MeshingParameterStyle.Quality then 1
        elif current = MeshingParameterStyle.Custom then  2
        else -1

    ///<summary>Sets the render Mesh quality of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="quality">(int) The render Mesh quality, either:
    ///    0: Jagged and faster. Objects may look jagged, but they should shade and render relatively quickly.
    ///    1: Smooth and slower. Objects should look smooth, but they may take a very long time to shade and render.
    ///    2: Custom</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderMeshQuality(quality:int) : unit = //SET
        let newValue = 
            if quality = 0 then
                MeshingParameterStyle.Fast
            elif quality = 1 then
                MeshingParameterStyle.Quality
            elif quality = 2 then
                MeshingParameterStyle.Custom
            else
                MeshingParameterStyle.None
        State.Doc.MeshingParameterStyle <- newValue


    ///<summary>Returns the render Mesh settings of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(int) The current render Mesh settings .
    ///      0: No settings enabled.
    ///      1: Refine Mesh enabled.
    ///      2: Jagged seams enabled.
    ///      4: Simple Planes enabled.
    ///      8: Texture is packed, scaled and normalized; otherwise unpacked, unscaled and normalized.</returns>
    static member RenderMeshSettings() : int = //GET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        let mutable rc = 0
        if current.RefineGrid then  rc <- rc +  1
        if current.JaggedSeams then  rc <- rc +  2
        if current.SimplePlanes then  rc <- rc +  4
        rc

    ///<summary>Sets the render Mesh settings of the active document.
    /// For more information on render Meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="settings">(int) The render Mesh settings, which is a bit-coded number that allows or disallows certain features.
    ///    The bits can be added together in any combination to form a value between 0 and 7. The bit values are as follows:
    ///      0: No settings enabled.
    ///      1: Refine Mesh enabled.
    ///      2: Jagged seams enabled.
    ///      4: Simple Planes enabled.
    ///      8: Texture is packed, scaled and normalized; otherwise unpacked, unscaled and normalized</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderMeshSettings(settings:int) : unit = //SET
        let current = State.Doc.GetMeshingParameters(State.Doc.MeshingParameterStyle)
        current.RefineGrid <- (settings &&& 1)   <> 0
        current.JaggedSeams <- (settings &&& 2)  <> 0
        current.SimplePlanes <- (settings &&& 4) <> 0
        State.Doc.SetCustomMeshingParameters(current)
        State.Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom


    ///<summary>Returns render settings.</summary>
    ///<returns>(int) if settings are not specified, the current render settings in bit-coded flags
    ///    0 = none,
    ///    1 = create shadows,
    ///    2 = use lights on layers that are off,
    ///    4 = render Curves and iso-curves,
    ///    8 = render dimensions and text.</returns>
    static member RenderSettings() : int = //GET
        let mutable rc = 0
        let rendersettings = State.Doc.RenderSettings
        if 0 <> rendersettings.ShadowmapLevel   then  rc <- rc + 1
        if rendersettings.UseHiddenLights       then  rc <- rc + 2
        if rendersettings.RenderCurves          then  rc <- rc + 4
        if rendersettings.RenderAnnotations     then  rc <- rc + 8
        rc

    ///<summary>Sets render settings.</summary>
    ///<param name="settings">(int) Bit-coded flags of render settings to modify.
    ///    0 = none,
    ///    1 = create shadows,
    ///    2 = use lights on layers that are off,
    ///    4 = render Curves and iso-curves,
    ///    8 = render dimensions and text</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenderSettings(settings:int) : unit = //SET
        let rendersettings = State.Doc.RenderSettings
        rendersettings.ShadowmapLevel <-    (settings &&& 1)
        rendersettings.UseHiddenLights <-   (settings &&& 2) = 2
        rendersettings.RenderCurves <-      (settings &&& 4) = 4
        rendersettings.RenderAnnotations <- (settings &&& 8) = 8
        State.Doc.RenderSettings <- rendersettings


    ///<summary>Returns the document's absolute tolerance. Absolute tolerance
    /// is measured in drawing units. See Rhino's document properties command
    /// (Units and Page Units Window) for details.</summary>
    ///<returns>(float) The current absolute tolerance.</returns>
    static member UnitAbsoluteTolerance() : float = //GET
        State.Doc.ModelAbsoluteTolerance

    ///<summary>Sets the document's absolute tolerance. Absolute tolerance
    /// is measured in drawing units. See Rhino's document properties command
    /// (Units and Page Units Window) for details.</summary>
    ///<param name="tolerance">(float) The absolute tolerance to set</param>
    ///<returns>(unit) void, nothing.</returns>
    static member UnitAbsoluteTolerance(tolerance:float) : unit = //SET
        if tolerance > 0.0 then
            State.Doc.ModelAbsoluteTolerance <- tolerance
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.UnitAbsoluteTolerance failed.  tolerance:'%A'" tolerance



    ///<summary>Return the document's angle tolerance. Angle tolerance is
    /// measured in degrees. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details.</summary>
    ///<returns>(float) The current angle tolerance.</returns>
    static member UnitAngleTolerance() : float = //GET
        State.Doc.ModelAngleToleranceDegrees

    ///<summary>Set the document's angle tolerance. Angle tolerance is
    /// measured in degrees. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details.</summary>
    ///<param name="angleToleranceDegrees">(float) The angle tolerance to set</param>
    ///<returns>(unit) void, nothing.</returns>
    static member UnitAngleTolerance(angleToleranceDegrees:float) : unit = //SET
            if angleToleranceDegrees > 0. then
                State.Doc.ModelAngleToleranceDegrees <- angleToleranceDegrees
            else
                RhinoScriptingException.Raise "RhinoScriptSyntax.UnitAngleTolerance failed.  angleToleranceDegrees:'%A'" angleToleranceDegrees


    ///<summary>Return the document's distance display precision.</summary>
    ///<returns>(int) The current distance display precision.</returns>
    static member UnitDistanceDisplayPrecision() : int = //GET
        State.Doc.ModelDistanceDisplayPrecision

    ///<summary>Set the document's distance display precision.</summary>
    ///<param name="precision">(int) The distance display precision. If the current distance display mode is Decimal, then precision is the number of decimal places.
    ///    If the current distance display mode is Fractional (including Feet and Inches), then the denominator = (1/2)^precision.
    ///    Use UnitDistanceDisplayMode to get the current distance display mode</param>
    ///<returns>(unit) void, nothing.</returns>
    static member UnitDistanceDisplayPrecision(precision:int) : unit = //SET
            State.Doc.ModelDistanceDisplayPrecision <- precision


    ///<summary>Return the document's relative tolerance. Relative tolerance
    /// is measured in percent. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details.</summary>
    ///<returns>(float) The current tolerance in percent.</returns>
    static member UnitRelativeTolerance() : float = //GET
        State.Doc.ModelRelativeTolerance

    ///<summary>Set the document's relative tolerance. Relative tolerance
    /// is measured in percent. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details.</summary>
    ///<param name="relativeTolerance">(float) The relative tolerance in percent</param>
    ///<returns>(unit) void, nothing.</returns>
    static member UnitRelativeTolerance(relativeTolerance:float) : unit = //SET
            if relativeTolerance > 0.0 then
                State.Doc.ModelRelativeTolerance <- relativeTolerance
            else
                RhinoScriptingException.Raise "RhinoScriptSyntax.UnitRelativeTolerance failed.  relativeTolerance:'%A'" relativeTolerance


    ///<summary>Return the scale factor for changing between unit systems.</summary>
    ///<param name="toSystem">(int) The unit system to convert to. The unit systems are are:
    ///    0 - No unit system
    ///    1 - Microns (1.0e-6 meters)
    ///    2 - Millimeters (1.0e-3 meters)
    ///    3 - Centimeters (1.0e-2 meters)
    ///    4 - Meters
    ///    5 - Kilometers (1.0e + 3 meters)
    ///    6 - Microinches (2.54e-8 meters, 1.0e-6 inches)
    ///    7 - Mils (2.54e-5 meters, 0.001 inches)
    ///    8 - Inches (0.0254 meters)
    ///    9 - Feet (0.3048 meters, 12 inches)
    ///   10 - Miles (1609.344 meters, 5280 feet)
    ///   11 - *Reserved for custom Unit System*
    ///   12 - Angstroms (1.0e-10 meters)
    ///   13 - Nanometers (1.0e-9 meters)
    ///   14 - Decimeters (1.0e-1 meters)
    ///   15 - Dekameters (1.0e + 1 meters)
    ///   16 - Hectometers (1.0e + 2 meters)
    ///   17 - Megameters (1.0e + 6 meters)
    ///   18 - Gigameters (1.0e + 9 meters)
    ///   19 - Yards (0.9144  meters, 36 inches)
    ///   20 - Printer point (1/72 inches, computer points)
    ///   21 - Printer pica (1/6 inches, (computer picas)
    ///   22 - Nautical mile (1852 meters)
    ///   23 - Astronomical (1.4959787e + 11)
    ///   24 - Lightyears (9.46073e + 15 meters)
    ///   25 - Parsecs (3.08567758e + 16)</param>
    ///<param name="fromSystem">(int) The unit system to convert from (see above)</param>
    ///<returns>(float) scale factor for changing between unit systems.</returns>
    static member UnitScale(toSystem:int, fromSystem:int) : float = // https://github.com/mcneel/rhinoscriptsyntax/pull/198/files
        let toSystem:UnitSystem   = LanguagePrimitives.EnumOfValue  (byte toSystem)
        let fromSystem:UnitSystem  = LanguagePrimitives.EnumOfValue (byte fromSystem)
        RhinoMath.UnitScale(fromSystem, toSystem)


    ///<summary>Return the document's unit system. See Rhino's DocumentProperties
    /// command (Units and Page Units Window) for details.</summary>
    ///<returns>(int) The current unit system
    ///    0 - No unit system
    ///    1 - Microns (1.0e-6 meters)
    ///    2 - Millimeters (1.0e-3 meters)
    ///    3 - Centimeters (1.0e-2 meters)
    ///    4 - Meters
    ///    5 - Kilometers (1.0e + 3 meters)
    ///    6 - MicroInches (2.54e-8 meters, 1.0e-6 inches)
    ///    7 - Mils (2.54e-5 meters, 0.001 inches)
    ///    8 - Inches (0.0254 meters)
    ///    9 - Feet (0.3048 meters, 12 inches)
    ///   10 - Miles (1609.344 meters, 5280 feet)
    ///   11 - *Reserved for custom Unit System*
    ///   12 - Angstroms (1.0e-10 meters)
    ///   13 - Nanometers (1.0e-9 meters)
    ///   14 - Decimeters (1.0e-1 meters)
    ///   15 - Dekameters (1.0e + 1 meters)
    ///   16 - Hectometers (1.0e + 2 meters)
    ///   17 - Megameters (1.0e + 6 meters)
    ///   18 - Gigameters (1.0e + 9 meters)
    ///   19 - Yards (0.9144  meters, 36 inches)
    ///   20 - Printer point (1/72 inches, computer points)
    ///   21 - Printer pica (1/6 inches, (computer picas)
    ///   22 - Nautical mile (1852 meters)
    ///   23 - Astronomical (1.4959787e + 11)
    ///   24 - Lightyears (9.46073e + 15 meters)
    ///   25 - Parsecs (3.08567758e + 16).</returns>
    static member UnitSystem() : int = //GET
            int(State.Doc.ModelUnitSystem)

    ///<summary>Set the document's unit system. See Rhino's DocumentProperties
    /// command (Units and Page Units Window) for details.</summary>
    ///<param name="unitSystem">(int) The unit system to set the document to. The unit systems are:
    ///    0 - No unit system
    ///    1 - Microns (1.0e-6 meters)
    ///    2 - Millimeters (1.0e-3 meters)
    ///    3 - Centimeters (1.0e-2 meters)
    ///    4 - Meters
    ///    5 - Kilometers (1.0e + 3 meters)
    ///    6 - MicroInches (2.54e-8 meters, 1.0e-6 inches)
    ///    7 - Mils (2.54e-5 meters, 0.001 inches)
    ///    8 - Inches (0.0254 meters)
    ///    9 - Feet (0.3048 meters, 12 inches)
    ///   10 - Miles (1609.344 meters, 5280 feet)
    ///   11 - *Reserved for custom Unit System*
    ///   12 - Angstroms (1.0e-10 meters)
    ///   13 - Nanometers (1.0e-9 meters)
    ///   14 - Decimeters (1.0e-1 meters)
    ///   15 - Dekameters (1.0e + 1 meters)
    ///   16 - Hectometers (1.0e + 2 meters)
    ///   17 - Megameters (1.0e + 6 meters)
    ///   18 - Gigameters (1.0e + 9 meters)
    ///   19 - Yards (0.9144  meters, 36 inches)
    ///   20 - Printer point (1/72 inches, computer points)
    ///   21 - Printer pica (1/6 inches, (computer picas)
    ///   22 - Nautical mile (1852 meters)
    ///   23 - Astronomical (1.4959787e + 11)
    ///   24 - Lightyears (9.46073e + 15 meters)
    ///   25 - Parsecs (3.08567758e + 16)</param>
    ///<param name="scale">(bool) Optional, default value: <c>false</c>
    ///    Scale existing geometry based on the new unit system.
    ///    If not specified, any existing geometry is not scaled (False)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member UnitSystem(unitSystem:int, [<OPT;DEF(false)>]scale:bool) : unit = //SET
        if unitSystem < 1 || unitSystem > 25 then
            RhinoScriptingException.Raise "RhinoScriptSyntax.UnitSystem value of %d is not  valid" unitSystem
            let unitSystem : UnitSystem  = LanguagePrimitives.EnumOfValue (byte unitSystem)
            State.Doc.AdjustPageUnitSystem(unitSystem, scale)



    ///<summary>Returns the name of the current unit system.</summary>
    ///<param name="capitalize">(bool) Optional, default value: <c>false</c>
    ///    Capitalize the first character of the units system name (e.g. return "Millimeter" instead of "millimeter"). The default is not to capitalize the first character (false)</param>
    ///<param name="singular">(bool) Optional, default value: <c>true</c>
    ///    Return the singular form of the units system name (e.g. "millimeter" instead of "millimeters"). The default is to return the singular form of the name (true)</param>
    ///<param name="abbreviate">(bool) Optional, default value: <c>false</c>
    ///    Abbreviate the name of the units system (e.g. return "mm" instead of "millimeter"). The default is not to abbreviate the name (false)</param>
    ///<param name="modelUnits">(bool) Optional, default value: <c>true</c>
    ///    Return the document's model units (True) or the document's page units (False). The default is True</param>
    ///<returns>(string) The name of the current units system.</returns>
    static member UnitSystemName([<OPT;DEF(false)>]capitalize:bool, [<OPT;DEF(true)>]singular:bool, [<OPT;DEF(false)>]abbreviate:bool, [<OPT;DEF(true)>]modelUnits:bool) : string = 
        State.Doc.GetUnitSystemName(modelUnits, capitalize, singular, abbreviate)



