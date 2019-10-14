namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
open Microsoft.FSharp.Core.LanguagePrimitives
open System.IO

[<AutoOpen>]
module ExtensionsDocument =
  
  type RhinoScriptSyntax with
    
    [<EXT>]
     ///<summary>Create a bitmap preview image of the current model</summary>
    ///<param name="fileName">(string) Name of the bitmap file to create</param>
    ///<param name="view">(string) Optional,   Title of the view. If omitted, the active view is used</param>
    ///<param name="width">(int) Optional, Default Value: <c>0</c>
    /// integer that specifies width of the bitmap in pixel. if only width given height will be scaled to kepp screen ratio</param>
    ///<param name="height">(int) Optional, Default Value: <c>0</c>
    /// integer that specifies height of the bitmap in pixel. if only height given width will be scaled to kepp screen ratio</param>
    ///<param name="flags">(int) Optional, Default Value: <c>0</c>
    ///  Bitmap creation flags. Can be the combination of:
    ///  1 = honor object highlighting
    ///  2 = draw construction plane
    ///  4 = use ghosted shading</param>
    ///<param name="wireframe">(bool) Optional, Default Value: <c>false</c>
    ///  If True then a wireframe preview image. If False,
    ///  a rendered image will be created</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CreatePreviewImage(fileName:string, [<OPT;DEF(null:string)>]view:string, [<OPT;DEF(0:int)>]width:int,[<OPT;DEF(0:int)>]height:int, [<OPT;DEF(0)>]flags:int, [<OPT;DEF(false)>]wireframe:bool) : bool =
        let rhview =
            if view = "" then Doc.Views.ActiveView
            else Doc.Views.Find(view, false)
        let rhsize =
            match width,height with
            | 0,0 -> rhview.ClientRectangle.Size
            | x,0 -> 
                let sc = x /. rhview.ClientRectangle.Size.Width
                Drawing.Size(x, rhview.ClientRectangle.Size.Height *. sc)
            | 0,y -> 
                let sc = y /. rhview.ClientRectangle.Size.Height
                Drawing.Size(rhview.ClientRectangle.Size.Width *. sc , y)   
            | x,y -> Drawing.Size(x,y)
        let ignoreHighlights =  (flags &&& 1) <> 1
        let drawcplane =        (flags &&& 2)  = 2
        let useghostedshading = (flags &&& 4)  = 4
        if wireframe then
            rhview.CreateWireframePreviewImage(fileName, rhsize, ignoreHighlights, drawcplane)
        else
            rhview.CreateShadedPreviewImage(fileName, rhsize, ignoreHighlights, drawcplane, useghostedshading)
    (*
    def CreatePreviewImage(fileName, view=None, size=None, flags=0, wireframe=False):
        '''Create a bitmap preview image of the current model
        Parameters:
          fileName (str): name of the bitmap file to create
          view (str, optional): title of the view. If omitted, the active view is used
          size (number, optional): two integers that specify width and height of the bitmap
          flags (number, optional): Bitmap creation flags. Can be the combination of:
              1 = honor object highlighting
              2 = draw construction plane
              4 = use ghosted shading
          wireframe (bool, optional): If True then a wireframe preview image. If False,
              a rendered image will be created
        Returns:
          bool: True or False indicating success or failure
        '''
        rhview = scriptcontext.doc.Views.ActiveView
        if view is not None:
            rhview = scriptcontext.doc.Views.Find(view, False)
            if rhview is None: return False
        rhsize = rhview.ClientRectangle.Size
        if size: rhsize = System.Drawing.Size(size[0], size[1])
        ignore_highlights = (flags&1)!=1
        drawcplane = (flags&2)==2
        useghostedshading = (flags&4)==4
        if wireframe:
            return rhview.CreateWireframePreviewImage(fileName, rhsize, ignore_highlights, drawcplane)
        else:
            return rhview.CreateShadedPreviewImage(fileName, rhsize, ignore_highlights, drawcplane, useghostedshading)
    *)


    [<EXT>]
     ///<summary>Returns the document's modified flag. This flag indicates whether
    /// or not any changes to the current document have been made. NOTE: setting the
    /// document modified flag to False will prevent the "Do you want to save this
    /// file..." from displaying when you close Rhino.</summary>
    ///<returns>(bool) if no modified state is specified, the current modified state</returns>
    static member DocumentModified() : bool = //GET
        Doc.Modified
    (*
    def DocumentModified(modified=None):
        '''Returns or sets the document's modified flag. This flag indicates whether
        or not any changes to the current document have been made. NOTE: setting the
        document modified flag to False will prevent the "Do you want to save this
        file..." from displaying when you close Rhino.
        Parameters:
          modified (bool): the modified state, either True or False
        Returns:
          bool: if no modified state is specified, the current modified state
          bool: if a modified state is specified, the previous modified state
        '''
        oldstate = scriptcontext.doc.Modified
        if modified is not None and modified!=oldstate:
            scriptcontext.doc.Modified = modified
        return oldstate
    *)

    [<EXT>]
     ///<summary>Sets the document's modified flag. This flag indicates whether
    /// or not any changes to the current document have been made. NOTE: setting the
    /// document modified flag to False will prevent the "Do you want to save this
    /// file..." from displaying when you close Rhino.</summary>
    ///<param name="modified">(bool)The modified state, either True or False</param>
    ///<returns>(unit) void, nothing</returns>
    static member DocumentModified(modified:bool) : unit = //SET
        Doc.Modified <- modified
    (*
    def DocumentModified(modified=None):
        '''Returns or sets the document's modified flag. This flag indicates whether
        or not any changes to the current document have been made. NOTE: setting the
        document modified flag to False will prevent the "Do you want to save this
        file..." from displaying when you close Rhino.
        Parameters:
          modified (bool): the modified state, either True or False
        Returns:
          bool: if no modified state is specified, the current modified state
          bool: if a modified state is specified, the previous modified state
        '''
        oldstate = scriptcontext.doc.Modified
        if modified is not None and modified!=oldstate:
            scriptcontext.doc.Modified = modified
        return oldstate
    *)


  
    [<EXT>]
     ///<summary>Returns the name of the currently loaded Rhino document (3DM file)</summary>
    ///<returns>(string) the name of the currently loaded Rhino document (3DM file)</returns>
    static member DocumentName() : string =
        Doc.Name |? ""
    (*
    def DocumentName():
        '''Returns the name of the currently loaded Rhino document (3DM file)
        Returns:
          str: the name of the currently loaded Rhino document (3DM file)
        '''
        return scriptcontext.doc.Name or None
    *)



    [<EXT>]
     ///<summary>Returns path of the currently loaded Rhino document (3DM file)</summary>
    ///<returns>(string) the path of the currently loaded Rhino document (3DM file)</returns>
    static member DocumentPath() : string =
        let p = Doc.Path
        if isNull p then ""
        else
            let slash = string Path.DirectorySeparatorChar
            if p.EndsWith slash then p
            else p + slash // add \ or / at the ende to be consistent with RhinoScript
    (*
    def DocumentPath():
        '''Returns path of the currently loaded Rhino document (3DM file)
        Returns:
          str: the path of the currently loaded Rhino document (3DM file)
        '''
        # GetDirectoryName throws an exception if an empty string is passed hence the 'or None'
        path = System.IO.Path.GetDirectoryName(scriptcontext.doc.Path or None)
        # add \ or / at the end to be consistent with RhinoScript
        if path and not path.endswith(str(System.IO.Path.DirectorySeparatorChar)):
          path += System.IO.Path.DirectorySeparatorChar
        return path
    *)



    [<EXT>]
     ///<summary>Enables or disables screen redrawing</summary>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///True to enable, False to disable</param>
    ///<returns>(unit) void, nothing</returns>
    static member EnableRedraw([<OPT;DEF(true)>]enable:bool) : unit =
        Doc.Views.RedrawEnabled <- enable
    (*
    def EnableRedraw(enable=True):
        '''Enables or disables screen redrawing
        Parameters:
          enable (bool, optional): True to enable, False to disable
        Returns: 
          bool: previous screen redrawing state
        '''
        old = scriptcontext.doc.Views.RedrawEnabled
        if old!=enable: scriptcontext.doc.Views.RedrawEnabled = enable
        return old
    *)


    [<EXT>]
     ///<summary>Extracts the bitmap preview image from the specified model (.3dm)</summary>
    ///<param name="fileName">(string) Name of the bitmap file to create. The extension of
    ///  the fileName controls the format of the bitmap file created.
    ///  (.bmp, .tga, .jpg, .jpeg, .pcx, .png, .tif, .tiff)</param>
    ///<param name="modelname">(string) Optional, The model (.3dm) from which to extract the
    ///  preview image. If omitted, the currently loaded model is used.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ExtractPreviewImage(fileName:string, [<OPT;DEF(null:string)>]modelname:string) : unit =
        let bmp =
            if notNull modelname  then
                if notNull Doc.Path then RhinoDoc.ExtractPreviewImage(Doc.Path) // TODO test this works ok
                else failwithf "extractPreviewImage failed on unsaved file"
            else
                RhinoDoc.ExtractPreviewImage(modelname)
        bmp.Save(fileName)
    (*
    def ExtractPreviewImage(fileName, modelname=None):
        '''Extracts the bitmap preview image from the specified model (.3dm)
        Parameters:
          fileName (str): name of the bitmap file to create. The extension of
             the fileName controls the format of the bitmap file created.
             (.bmp, .tga, .jpg, .jpeg, .pcx, .png, .tif, .tiff)
          modelname (str, optional): The model (.3dm) from which to extract the
             preview image. If omitted, the currently loaded model is used.
        Returns:
          bool: True or False indicating success or failure
        '''
        return scriptcontext.doc.ExtractPreviewImage(fileName, modelname)
    *)


    [<EXT>]
     ///<summary>Verifies that the current document has been modified in some way</summary>
    ///<returns>(bool) True or False.</returns>
    static member IsDocumentModified() : bool =
        Doc.Modified
    (*
    def IsDocumentModified():
        '''Verifies that the current document has been modified in some way
        Returns:
          bool: True or False. None on error
        '''
        return scriptcontext.doc.Modified
    *)


    [<EXT>]
     ///<summary>Returns the document's notes. Notes are generally created
    /// using Rhino's Notes command</summary>
    ///<returns>(string) if `newnotes` is omitted, the current notes</returns>
    static member Notes() : string = //GET
        Doc.Notes
    (*
    def Notes(newnotes=None):
        '''Returns or sets the document's notes. Notes are generally created
        using Rhino's Notes command
        Parameters:
          newnotes (str): new notes to set
        Returns:
          str: if `newnotes` is omitted, the current notes if successful
          str: if `newnotes` is specified, the previous notes if successful
        '''
        old = scriptcontext.doc.Notes
        if newnotes is not None: scriptcontext.doc.Notes = newnotes
        return old
    *)

    [<EXT>]
     ///<summary>Sets the document's notes. Notes are generally created
    /// using Rhino's Notes command</summary>
    ///<param name="newnotes">(string)New notes to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member Notes(newnotes:string) : unit = //SET
        Doc.Notes <- newnotes
    (*
    def Notes(newnotes=None):
        '''Returns or sets the document's notes. Notes are generally created
        using Rhino's Notes command
        Parameters:
          newnotes (str): new notes to set
        Returns:
          str: if `newnotes` is omitted, the current notes if successful
          str: if `newnotes` is specified, the previous notes if successful
        '''
        old = scriptcontext.doc.Notes
        if newnotes is not None: scriptcontext.doc.Notes = newnotes
        return old
    *)



    [<EXT>]
     ///<summary>Returns the file version of the current document. Use this function to
    ///  determine which version of Rhino last saved the document. Note, this
    ///  function will not return values from referenced or merged files.</summary>
    ///<returns>(int) the file version of the current document</returns>
    static member ReadFileVersion() : int =
        Doc.ReadFileVersion()
    (*
    def ReadFileVersion():
        '''Returns the file version of the current document. Use this function to
        determine which version of Rhino last saved the document. Note, this
        function will not return values from referenced or merged files.
        Returns:
          str: the file version of the current document
        '''
        return scriptcontext.doc.ReadFileVersion()
    *)


    [<EXT>]
     ///<summary>Redraws all views</summary>
    ///<returns>(unit) </returns>
    static member Redraw() : unit =
        let old = Doc.Views.RedrawEnabled
        Doc.Views.RedrawEnabled <- true
        Doc.Views.Redraw()
        RhinoApp.Wait()
        Doc.Views.RedrawEnabled <- old
    (*
    def Redraw():
        '''Redraws all views
        Returns:
          None 
        '''
        old = scriptcontext.doc.Views.RedrawEnabled
        scriptcontext.doc.Views.RedrawEnabled = True
        scriptcontext.doc.Views.Redraw()
        Rhino.RhinoApp.Wait()
        scriptcontext.doc.Views.RedrawEnabled = old
    *)


    [<EXT>]
     ///<summary>Returns render antialiasing style</summary>
    ///<returns>(int) The current antialiasing style (0=none, 1=normal, 2=best)</returns>
    static member RenderAntialias() : int = //GET
        int(Doc.RenderSettings.AntialiasLevel) // TODO check
    (*
    def RenderAntialias(style=None):
        '''Returns or sets render antialiasing style
        Parameters:
          style (number, optional): level of antialiasing (0=none, 1=normal, 2=best)
        Returns:
          number: if style is not specified, the current antialiasing style
          number: if style is specified, the previous antialiasing style
        '''
        rc = scriptcontext.doc.RenderSettings.AntialiasLevel
        if style==0 or style==1 or style==2:
            settings = scriptcontext.doc.RenderSettings
            settings.AntialiasLevel = style
            scriptcontext.doc.RenderSettings = settings
        return rc
    *)

    [<EXT>]
     ///<summary>Sets render antialiasing style</summary>
    ///<param name="style">(int)Level of antialiasing (0=none, 1=normal, 2=best)</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderAntialias(style:int) : unit = //SET
        if style=0 || style=1 || style=2 then
            let settings = Doc.RenderSettings
            settings.AntialiasLevel <- EnumOfValue (style)
            Doc.RenderSettings <- settings
    (*
    def RenderAntialias(style=None):
        '''Returns or sets render antialiasing style
        Parameters:
          style (number, optional): level of antialiasing (0=none, 1=normal, 2=best)
        Returns:
          number: if style is not specified, the current antialiasing style
          number: if style is specified, the previous antialiasing style
        '''
        rc = scriptcontext.doc.RenderSettings.AntialiasLevel
        if style==0 or style==1 or style==2:
            settings = scriptcontext.doc.RenderSettings
            settings.AntialiasLevel = style
            scriptcontext.doc.RenderSettings = settings
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render ambient light or background color</summary>
    ///<param name="item">(int) 0=ambient light color, 1=background color</param>
    ///<returns>(Drawing.Color) The current item color</returns>
    static member RenderColor(item:int) : Drawing.Color = //GET
        if item<>0 && item<>1 then  failwithf "Item must be 0 or 1.  item:'%A'" item
        if item=0 then  Doc.RenderSettings.AmbientLight
        else Doc.RenderSettings.BackgroundColorTop
    (*
    def RenderColor(item, color=None):
        '''Returns or sets the render ambient light or background color
        Parameters:
          item (number): 0=ambient light color, 1=background color
          color (color, optional): the new color value. If omitted, the current item color is returned
        Returns:
          color: if color is not specified, the current item color
          color: if color is specified, the previous item color
        '''
        if item!=0 and item!=1: raise ValueError("item must be 0 or 1")
        if item==0: rc = scriptcontext.doc.RenderSettings.AmbientLight
        else: rc = scriptcontext.doc.RenderSettings.BackgroundColorTop
        if color is not None:
            color = rhutil.coercecolor(color, True)
            settings = scriptcontext.doc.RenderSettings
            if item==0: settings.AmbientLight = color
            else: settings.BackgroundColorTop = color
            scriptcontext.doc.RenderSettings = settings
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render ambient light or background color</summary>
    ///<param name="item">(int) 0=ambient light color, 1=background color</param>
    ///<param name="color">(Drawing.Color)The new color value. If omitted, the current item color is returned</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderColor(item:int, color:Drawing.Color) : unit = //SET
        if item<>0 && item<>1 then  failwithf "Item must be 0 || 1.  item:'%A' color:'%A'" item color
        let settings = Doc.RenderSettings
        if item=0 then  settings.AmbientLight <- color
        else            settings.BackgroundColorTop <- color
        Doc.RenderSettings <- settings
        Doc.Views.Redraw()
    (*
    def RenderColor(item, color=None):
        '''Returns or sets the render ambient light or background color
        Parameters:
          item (number): 0=ambient light color, 1=background color
          color (color, optional): the new color value. If omitted, the current item color is returned
        Returns:
          color: if color is not specified, the current item color
          color: if color is specified, the previous item color
        '''
        if item!=0 and item!=1: raise ValueError("item must be 0 or 1")
        if item==0: rc = scriptcontext.doc.RenderSettings.AmbientLight
        else: rc = scriptcontext.doc.RenderSettings.BackgroundColorTop
        if color is not None:
            color = rhutil.coercecolor(color, True)
            settings = scriptcontext.doc.RenderSettings
            if item==0: settings.AmbientLight = color
            else: settings.BackgroundColorTop = color
            scriptcontext.doc.RenderSettings = settings
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render resolution</summary>
    ///<returns>(int * int) The current resolution width,height</returns>
    static member RenderResolution() : int * int = //GET
        let rc = Doc.RenderSettings.ImageSize
        rc.Width, rc.Height
    (*
    def RenderResolution(resolution=None):
        '''Returns or sets the render resolution
        Parameters:
          resolution ([number, number], optional): width and height of render
        Returns:
          tuple(number, number): if resolution is not specified, the current resolution width,height
          tuple(number, number): if resolution is specified, the previous resolution width, height
        '''
        rc = scriptcontext.doc.RenderSettings.ImageSize
        if resolution:
            settings = scriptcontext.doc.RenderSettings
            settings.ImageSize = System.Drawing.Size(resolution[0], resolution[1])
            scriptcontext.doc.RenderSettings = settings
        return rc.Width, rc.Height
    *)

    [<EXT>]
     ///<summary>Sets the render resolution</summary>
    ///<param name="width">(int) width and height of render</param>
    ///<param name="height">(int) height of render</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderResolution(width:int, height:int) : unit = //SET
            let settings = Doc.RenderSettings
            settings.ImageSize <- Drawing.Size(width ,height)
            Doc.RenderSettings <- settings
    (*
    def RenderResolution(resolution=None):
        '''Returns or sets the render resolution
        Parameters:
          resolution ([number, number], optional): width and height of render
        Returns:
          tuple(number, number): if resolution is not specified, the current resolution width,height
          tuple(number, number): if resolution is specified, the previous resolution width, height
        '''
        rc = scriptcontext.doc.RenderSettings.ImageSize
        if resolution:
            settings = scriptcontext.doc.RenderSettings
            settings.ImageSize = System.Drawing.Size(resolution[0], resolution[1])
            scriptcontext.doc.RenderSettings = settings
        return rc.Width, rc.Height
    *)



    [<EXT>]
     ///<summary>Returns the render mesh density property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh density .</returns>
    static member RenderMeshDensity() : float = //GET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        current.RelativeTolerance
    (*
    def RenderMeshDensity(density=None):
        '''Returns or sets the render mesh density property of the active document.
            For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          density (number, optional): the new render mesh density, which is a number between 0.0 and 1.0.
        Returns:
          number: if density is not specified, the current render mesh density if successful.
          number: if density is specified, the previous render mesh density if successful.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.RelativeTolerance
        if density is not None:
            if Rhino.RhinoMath.Clamp(density, 0.0, 1.0) != density: return None
            current.RelativeTolerance = density
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render mesh density property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="density">(float)The new render mesh density, which is a number between 0.0 and 1.0.</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderMeshDensity(density:float) : unit = //SET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        if RhinoMath.Clamp(density, 0.0, 1.0) = density then
            current.RelativeTolerance <- density
            Doc.SetCustomMeshingParameters(current)
            Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom
    (*
    def RenderMeshDensity(density=None):
        '''Returns or sets the render mesh density property of the active document.
            For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          density (number, optional): the new render mesh density, which is a number between 0.0 and 1.0.
        Returns:
          number: if density is not specified, the current render mesh density if successful.
          number: if density is specified, the previous render mesh density if successful.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.RelativeTolerance
        if density is not None:
            if Rhino.RhinoMath.Clamp(density, 0.0, 1.0) != density: return None
            current.RelativeTolerance = density
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render mesh maximum angle property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current maximum angle .</returns>
    static member RenderMeshMaxAngle() : float = //GET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        toDegrees(current.RefineAngle)
    (*
    def RenderMeshMaxAngle(angle_degrees=None):
        '''Returns or sets the render mesh maximum angle property of the active document.  
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          angle_degrees (number, optional): the new maximum angle, which is a positive number in degrees.
        Returns:
          number: if angle_degrees is not specified, the current maximum angle if successful.
          number: if angle_degrees is specified, the previous maximum angle if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = math.degrees(current.RefineAngle)
        if angle_degrees is not None:
            if angle_degrees < 0: return None
            current.RefineAngle = math.radians(angle_degrees)
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render mesh maximum angle property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="angleDegrees">(float)The new maximum angle, which is a positive number in degrees.</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderMeshMaxAngle(angleDegrees:float) : unit = //SET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        if angleDegrees > 0. then
                current.RefineAngle <- toRadians(angleDegrees)
                Doc.SetCustomMeshingParameters(current)
                Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom
    (*
    def RenderMeshMaxAngle(angle_degrees=None):
        '''Returns or sets the render mesh maximum angle property of the active document.  
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          angle_degrees (number, optional): the new maximum angle, which is a positive number in degrees.
        Returns:
          number: if angle_degrees is not specified, the current maximum angle if successful.
          number: if angle_degrees is specified, the previous maximum angle if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = math.degrees(current.RefineAngle)
        if angle_degrees is not None:
            if angle_degrees < 0: return None
            current.RefineAngle = math.radians(angle_degrees)
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render mesh maximum aspect ratio property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh maximum aspect ratio .</returns>
    static member RenderMeshMaxAspectRatio() : float = //GET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        let rc = current.GridAspectRatio
        rc
    (*
    def RenderMeshMaxAspectRatio(ratio=None):
        '''Returns or sets the render mesh maximum aspect ratio property of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
         Parameters:
          ratio (number, optional): the render mesh maximum aspect ratio.  The suggested range, when not zero, is from 1 to 100.
        Returns:
          number: if ratio is not specified, the current render mesh maximum aspect ratio if successful.
          number: if ratio is specified, the previous render mesh maximum aspect ratio if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.GridAspectRatio
        if ratio is not None:
            current.GridAspectRatio = ratio
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render mesh maximum aspect ratio property of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="ratio">(float)The render mesh maximum aspect ratio.  The suggested range, when not zero, is from 1 to 100.</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderMeshMaxAspectRatio(ratio:float) : unit = //SET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        if ratio <> 0.0 then
            current.GridAspectRatio <- ratio
            Doc.SetCustomMeshingParameters(current)
            Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom
    (*
    def RenderMeshMaxAspectRatio(ratio=None):
        '''Returns or sets the render mesh maximum aspect ratio property of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
         Parameters:
          ratio (number, optional): the render mesh maximum aspect ratio.  The suggested range, when not zero, is from 1 to 100.
        Returns:
          number: if ratio is not specified, the current render mesh maximum aspect ratio if successful.
          number: if ratio is specified, the previous render mesh maximum aspect ratio if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.GridAspectRatio
        if ratio is not None:
            current.GridAspectRatio = ratio
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render mesh maximum distance, edge to surface parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh maximum distance, edge to surface .</returns>
    static member RenderMeshMaxDistEdgeToSrf() : float = //GET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        let rc = current.Tolerance
        rc
    (*
    def RenderMeshMaxDistEdgeToSrf(distance=None):
        '''Returns or sets the render mesh maximum distance, edge to surface parameter of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          distance (number, optional): the render mesh maximum distance, edge to surface.
        Returns:
          number: if distance is not specified, the current render mesh maximum distance, edge to surface if successful.
          number: if distance is specified, the previous render mesh maximum distance, edge to surface if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.Tolerance
        if distance is not None:
            current.Tolerance = distance
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render mesh maximum distance, edge to surface parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="distance">(float)The render mesh maximum distance, edge to surface.</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderMeshMaxDistEdgeToSrf(distance:float) : unit = //SET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        if distance > 0. then
            current.Tolerance <- distance
            Doc.SetCustomMeshingParameters(current)
            Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom
    (*
    def RenderMeshMaxDistEdgeToSrf(distance=None):
        '''Returns or sets the render mesh maximum distance, edge to surface parameter of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          distance (number, optional): the render mesh maximum distance, edge to surface.
        Returns:
          number: if distance is not specified, the current render mesh maximum distance, edge to surface if successful.
          number: if distance is specified, the previous render mesh maximum distance, edge to surface if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.Tolerance
        if distance is not None:
            current.Tolerance = distance
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render mesh maximum edge length parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh maximum edge length .</returns>
    static member RenderMeshMaxEdgeLength() : float = //GET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        let rc = current.MaximumEdgeLength
        rc
    (*
    def RenderMeshMaxEdgeLength(distance=None):
        '''Returns or sets the render mesh maximum edge length parameter of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          distance (number, optional): the render mesh maximum edge length.
        Returns:
          number: if distance is not specified, the current render mesh maximum edge length if successful.
          number: if distance is specified, the previous render mesh maximum edge length if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.MaximumEdgeLength
        if distance is not None:
            current.MaximumEdgeLength = distance
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render mesh maximum edge length parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="distance">(float)The render mesh maximum edge length.</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderMeshMaxEdgeLength(distance:float) : unit = //SET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        if distance > 0.0 then
            current.MaximumEdgeLength <- distance
            Doc.SetCustomMeshingParameters(current)
            Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom
    (*
    def RenderMeshMaxEdgeLength(distance=None):
        '''Returns or sets the render mesh maximum edge length parameter of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          distance (number, optional): the render mesh maximum edge length.
        Returns:
          number: if distance is not specified, the current render mesh maximum edge length if successful.
          number: if distance is specified, the previous render mesh maximum edge length if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.MaximumEdgeLength
        if distance is not None:
            current.MaximumEdgeLength = distance
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render mesh minimum edge length parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(float) The current render mesh minimum edge length .</returns>
    static member RenderMeshMinEdgeLength() : float = //GET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        let rc = current.MinimumEdgeLength
        rc
    (*
    def RenderMeshMinEdgeLength(distance=None):
        '''Returns or sets the render mesh minimum edge length parameter of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
            Parameters:
          distance (number, optional): the render mesh minimum edge length.
        Returns:
          number: if distance is not specified, the current render mesh minimum edge length if successful.
          number: if distance is specified, the previous render mesh minimum edge length if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.MinimumEdgeLength
        if distance is not None:
            current.MinimumEdgeLength = distance
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render mesh minimum edge length parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="distance">(float)The render mesh minimum edge length.</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderMeshMinEdgeLength(distance:float) : unit = //SET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        let rc = current.MinimumEdgeLength
        if distance > 0.0 then
            current.MinimumEdgeLength <- distance
            Doc.SetCustomMeshingParameters(current)
            Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom
    (*
    def RenderMeshMinEdgeLength(distance=None):
        '''Returns or sets the render mesh minimum edge length parameter of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
            Parameters:
          distance (number, optional): the render mesh minimum edge length.
        Returns:
          number: if distance is not specified, the current render mesh minimum edge length if successful.
          number: if distance is specified, the previous render mesh minimum edge length if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.MinimumEdgeLength
        if distance is not None:
            current.MinimumEdgeLength = distance
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render mesh minimum initial grid quads parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(int) The current render mesh minimum initial grid quads .</returns>
    static member RenderMeshMinInitialGridQuads() : int = //GET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        current.GridMinCount
        
    (*
    def RenderMeshMinInitialGridQuads(quads=None):
        '''Returns or sets the render mesh minimum initial grid quads parameter of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          quads (number, optional): the render mesh minimum initial grid quads. The suggested range is from 0 to 10000.
        Returns:
          number: if quads is not specified, the current render mesh minimum initial grid quads if successful.
          number: if quads is specified, the previous render mesh minimum initial grid quads if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.GridMinCount
        if quads is not None:
            current.GridMinCount = quads
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render mesh minimum initial grid quads parameter of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="quads">(int)The render mesh minimum initial grid quads. The suggested range is from 0 to 10000.</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderMeshMinInitialGridQuads(quads:int) : unit = //SET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        let rc = current.GridMinCount
        if quads > 0 then
            current.GridMinCount <- quads
            Doc.SetCustomMeshingParameters(current)
            Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom
    (*
    def RenderMeshMinInitialGridQuads(quads=None):
        '''Returns or sets the render mesh minimum initial grid quads parameter of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          quads (number, optional): the render mesh minimum initial grid quads. The suggested range is from 0 to 10000.
        Returns:
          number: if quads is not specified, the current render mesh minimum initial grid quads if successful.
          number: if quads is specified, the previous render mesh minimum initial grid quads if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = current.GridMinCount
        if quads is not None:
            current.GridMinCount = quads
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render mesh quality of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(int) The current render mesh quality .
    ///  0: Jagged and faster.  Objects may look jagged, but they should shade and render relatively quickly.
    ///  1: Smooth and slower.  Objects should look smooth, but they may take a very long time to shade and render.
    ///  2: Custom.</returns>
    static member RenderMeshQuality() : int = //GET
        let current = Doc.MeshingParameterStyle
        if current = MeshingParameterStyle.Fast then 0
        elif current = MeshingParameterStyle.Quality then 1
        elif current = MeshingParameterStyle.Custom then  2
        else -1
    (*
    def RenderMeshQuality(quality=None):
        '''Returns or sets the render mesh quality of the active document.
            For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
          Parameters:
          quality (number, optional): the render mesh quality, either:
            0: Jagged and faster.  Objects may look jagged, but they should shade and render relatively quickly.
            1: Smooth and slower.  Objects should look smooth, but they may take a very long time to shade and render.
            2: Custom.
        Returns:
          number: if quality is not specified, the current render mesh quality if successful.
          number: if quality is specified, the previous render mesh quality if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.MeshingParameterStyle
        if current == Rhino.Geometry.MeshingParameterStyle.Fast:
            rc = 0
        elif current == Rhino.Geometry.MeshingParameterStyle.Quality:
            rc = 1
        elif current == Rhino.Geometry.MeshingParameterStyle.Custom:
            rc = 2
        else:
            rc = None
        if quality is not None:
            if quality == 0:
                new_value = Rhino.Geometry.MeshingParameterStyle.Fast
            elif quality == 1:
                new_value = Rhino.Geometry.MeshingParameterStyle.Quality
            elif quality == 2:
                new_value = Rhino.Geometry.MeshingParameterStyle.Custom
            else:
                return None
            scriptcontext.doc.MeshingParameterStyle = new_value
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render mesh quality of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="quality">(int)The render mesh quality, either:
    ///  0: Jagged and faster.  Objects may look jagged, but they should shade and render relatively quickly.
    ///  1: Smooth and slower.  Objects should look smooth, but they may take a very long time to shade and render.
    ///  2: Custom.</param>
    ///<returns>(unit) void, nothing</returns>
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
        Doc.MeshingParameterStyle <- newValue
    (*
    def RenderMeshQuality(quality=None):
        '''Returns or sets the render mesh quality of the active document.
            For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
          Parameters:
          quality (number, optional): the render mesh quality, either:
            0: Jagged and faster.  Objects may look jagged, but they should shade and render relatively quickly.
            1: Smooth and slower.  Objects should look smooth, but they may take a very long time to shade and render.
            2: Custom.
        Returns:
          number: if quality is not specified, the current render mesh quality if successful.
          number: if quality is specified, the previous render mesh quality if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.MeshingParameterStyle
        if current == Rhino.Geometry.MeshingParameterStyle.Fast:
            rc = 0
        elif current == Rhino.Geometry.MeshingParameterStyle.Quality:
            rc = 1
        elif current == Rhino.Geometry.MeshingParameterStyle.Custom:
            rc = 2
        else:
            rc = None
        if quality is not None:
            if quality == 0:
                new_value = Rhino.Geometry.MeshingParameterStyle.Fast
            elif quality == 1:
                new_value = Rhino.Geometry.MeshingParameterStyle.Quality
            elif quality == 2:
                new_value = Rhino.Geometry.MeshingParameterStyle.Custom
            else:
                return None
            scriptcontext.doc.MeshingParameterStyle = new_value
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the render mesh settings of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<returns>(int) The current render mesh settings .
    ///    0: No settings enabled.
    ///    1: Refine mesh enabled.
    ///    2: Jagged seams enabled.
    ///    4: Simple planes enabled.
    ///    8: Texture is packed, scaled and normalized; otherwise unpacked, unscaled and normalized.</returns>
    static member RenderMeshSettings() : int = //GET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        let mutable rc = 0
        if current.RefineGrid then  rc <- rc +  1
        if current.JaggedSeams then  rc <- rc +  2
        if current.SimplePlanes then  rc <- rc +  4
        rc
    (*
    def RenderMeshSettings(settings=None):
        '''Returns or sets the render mesh settings of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          settings (number, optional): the render mesh settings, which is a bit-coded number that allows or disallows certain features.
          The bits can be added together in any combination to form a value between 0 and 7.  The bit values are as follows:
            0: No settings enabled.
            1: Refine mesh enabled.
            2: Jagged seams enabled.
            4: Simple planes enabled.
            8: Texture is packed, scaled and normalized; otherwise unpacked, unscaled and normalized.
        Returns:
          number: if settings is not specified, the current render mesh settings if successful.
          number: if settings is specified, the previous render mesh settings if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = 0
        if current.RefineGrid: rc += 1
        if current.JaggedSeams: rc += 2
        if current.SimplePlanes: rc += 4
        if current.TextureRange == Rhino.Geometry.MeshingParameterTextureRange.PackedScaledNormalized: rc += 8
        if settings is not None:
            current.RefineGrid = (settings & 1)
            current.JaggedSeams = (settings & 2)
            current.SimplePlanes = (settings & 4)
            current.TextureRange = Rhino.Geometry.MeshingParameterTextureRange.PackedScaledNormalized if (settings & 8) else Rhino.Geometry.MeshingParameterTextureRange.UnpackedUnscaledNormalized
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the render mesh settings of the active document.
    /// For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.</summary>
    ///<param name="settings">(int)The render mesh settings, which is a bit-coded number that allows or disallows certain features.
    ///  The bits can be added together in any combination to form a value between 0 and 7.  The bit values are as follows:
    ///    0: No settings enabled.
    ///    1: Refine mesh enabled.
    ///    2: Jagged seams enabled.
    ///    4: Simple planes enabled.
    ///    8: Texture is packed, scaled and normalized; otherwise unpacked, unscaled and normalized.</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderMeshSettings(settings:int) : unit = //SET
        let current = Doc.GetMeshingParameters(Doc.MeshingParameterStyle)
        current.RefineGrid <- (settings &&& 1)   <> 0
        current.JaggedSeams <- (settings &&& 2)  <> 0
        current.SimplePlanes <- (settings &&& 4) <> 0
        Doc.SetCustomMeshingParameters(current)
        Doc.MeshingParameterStyle <- Rhino.Geometry.MeshingParameterStyle.Custom
    (*
    def RenderMeshSettings(settings=None):
        '''Returns or sets the render mesh settings of the active document.
          For more information on render meshes, see the Document Properties: Mesh topic in the Rhino help file.
        Parameters:
          settings (number, optional): the render mesh settings, which is a bit-coded number that allows or disallows certain features.
          The bits can be added together in any combination to form a value between 0 and 7.  The bit values are as follows:
            0: No settings enabled.
            1: Refine mesh enabled.
            2: Jagged seams enabled.
            4: Simple planes enabled.
            8: Texture is packed, scaled and normalized; otherwise unpacked, unscaled and normalized.
        Returns:
          number: if settings is not specified, the current render mesh settings if successful.
          number: if settings is specified, the previous render mesh settings if successful.
          None: if not successful, or on error.
        '''
        current = scriptcontext.doc.GetCurrentMeshingParameters()
        rc = 0
        if current.RefineGrid: rc += 1
        if current.JaggedSeams: rc += 2
        if current.SimplePlanes: rc += 4
        if current.TextureRange == Rhino.Geometry.MeshingParameterTextureRange.PackedScaledNormalized: rc += 8
        if settings is not None:
            current.RefineGrid = (settings & 1)
            current.JaggedSeams = (settings & 2)
            current.SimplePlanes = (settings & 4)
            current.TextureRange = Rhino.Geometry.MeshingParameterTextureRange.PackedScaledNormalized if (settings & 8) else Rhino.Geometry.MeshingParameterTextureRange.UnpackedUnscaledNormalized
            _SetRenderMeshAndUpdateStyle(current)
        return rc
    *)


    [<EXT>]
     ///<summary>Returns render settings</summary>
    ///<returns>(int) if settings are not specified, the current render settings in bit-coded flags
    ///  0=none,
    ///  1=create shadows,
    ///  2=use lights on layers that are off,
    ///  4=render curves and isocurves,
    ///  8=render dimensions and text</returns>
    static member RenderSettings() : int = //GET
        let mutable rc = 0
        let rendersettings = Doc.RenderSettings
        if 0 <> rendersettings.ShadowmapLevel   then  rc <- rc + 1
        if rendersettings.UseHiddenLights       then  rc <- rc + 2
        if rendersettings.RenderCurves          then  rc <- rc + 4
        if rendersettings.RenderAnnotations     then  rc <- rc + 8
        rc
    (*
    def RenderSettings(settings=None):
        '''Returns or sets render settings
        Parameters:
          settings (number, optional): bit-coded flags of render settings to modify.
            0=none,
            1=create shadows,
            2=use lights on layers that are off,
            4=render curves and isocurves,
            8=render dimensions and text
        Returns:
          number: if settings are not specified, the current render settings in bit-coded flags
          number: if settings are specified, the previous render settings in bit-coded flags
        '''
        rc = 0
        rendersettings = scriptcontext.doc.RenderSettings
        if rendersettings.ShadowmapLevel: rc+=1
        if rendersettings.UseHiddenLights: rc+=2
        if rendersettings.RenderCurves: rc+=4
        if rendersettings.RenderAnnotations: rc+=8
        if settings is not None:
            rendersettings.ShadowmapLevel = (settings & 1)
            rendersettings.UseHiddenLights = (settings & 2)==2
            rendersettings.RenderCurves = (settings & 4)==4
            rendersettings.RenderAnnotations = (settings & 8)==8
            scriptcontext.doc.RenderSettings = rendersettings
        return rc
    *)

    [<EXT>]
     ///<summary>Sets render settings</summary>
    ///<param name="settings">(int)Bit-coded flags of render settings to modify.
    ///  0=none,
    ///  1=create shadows,
    ///  2=use lights on layers that are off,
    ///  4=render curves and isocurves,
    ///  8=render dimensions and text</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenderSettings(settings:int) : unit = //SET
        let rendersettings = Doc.RenderSettings
        rendersettings.ShadowmapLevel <-    (settings &&& 1)
        rendersettings.UseHiddenLights <-   (settings &&& 2) = 2
        rendersettings.RenderCurves <-      (settings &&& 4) = 4
        rendersettings.RenderAnnotations <- (settings &&& 8) = 8
        Doc.RenderSettings <- rendersettings
    (*
    def RenderSettings(settings=None):
        '''Returns or sets render settings
        Parameters:
          settings (number, optional): bit-coded flags of render settings to modify.
            0=none,
            1=create shadows,
            2=use lights on layers that are off,
            4=render curves and isocurves,
            8=render dimensions and text
        Returns:
          number: if settings are not specified, the current render settings in bit-coded flags
          number: if settings are specified, the previous render settings in bit-coded flags
        '''
        rc = 0
        rendersettings = scriptcontext.doc.RenderSettings
        if rendersettings.ShadowmapLevel: rc+=1
        if rendersettings.UseHiddenLights: rc+=2
        if rendersettings.RenderCurves: rc+=4
        if rendersettings.RenderAnnotations: rc+=8
        if settings is not None:
            rendersettings.ShadowmapLevel = (settings & 1)
            rendersettings.UseHiddenLights = (settings & 2)==2
            rendersettings.RenderCurves = (settings & 4)==4
            rendersettings.RenderAnnotations = (settings & 8)==8
            scriptcontext.doc.RenderSettings = rendersettings
        return rc
    *)


    [<EXT>]
     ///<summary>Returns the document's absolute tolerance. Absolute tolerance
    /// is measured in drawing units. See Rhino's document properties command
    /// (Units and Page Units Window) for details</summary>
    ///<returns>(float) The current absolute tolerance</returns>
    static member UnitAbsoluteTolerance() : float = //GET
        Doc.ModelAbsoluteTolerance 
    (*
    def UnitAbsoluteTolerance(tolerance=None, in_model_units=True):
        '''Returns or sets the document's absolute tolerance. Absolute tolerance
        is measured in drawing units. See Rhino's document properties command
        (Units and Page Units Window) for details
        Parameters:
          tolerance (number, optional): the absolute tolerance to set
          in_model_units (bool, optional): Return or modify the document's model units (True)
                                or the document's page units (False)
        Returns:
          number: if tolerance is not specified, the current absolute tolerance
          number: if tolerance is specified, the previous absolute tolerance
        '''
        if in_model_units:
            rc = scriptcontext.doc.ModelAbsoluteTolerance
            if tolerance is not None:
                scriptcontext.doc.ModelAbsoluteTolerance = tolerance
        else:
            rc = scriptcontext.doc.PageAbsoluteTolerance
            if tolerance is not None:
                scriptcontext.doc.PageAbsoluteTolerance = tolerance
        return rc
    *)

    [<EXT>]
     ///<summary>Sets the document's absolute tolerance. Absolute tolerance
    /// is measured in drawing units. See Rhino's document properties command
    /// (Units and Page Units Window) for details</summary>
    ///<param name="tolerance">(float)The absolute tolerance to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member UnitAbsoluteTolerance(tolerance:float) : unit = //SET
        if tolerance > 0.0 then
            Doc.ModelAbsoluteTolerance <- tolerance
        else
            failwithf "UnitAbsoluteTolerance failed.  tolerance:'%A'" tolerance
            
    (*
    def UnitAbsoluteTolerance(tolerance=None, in_model_units=True):
        '''Returns or sets the document's absolute tolerance. Absolute tolerance
        is measured in drawing units. See Rhino's document properties command
        (Units and Page Units Window) for details
        Parameters:
          tolerance (number, optional): the absolute tolerance to set
          in_model_units (bool, optional): Return or modify the document's model units (True)
                                or the document's page units (False)
        Returns:
          number: if tolerance is not specified, the current absolute tolerance
          number: if tolerance is specified, the previous absolute tolerance
        '''
        if in_model_units:
            rc = scriptcontext.doc.ModelAbsoluteTolerance
            if tolerance is not None:
                scriptcontext.doc.ModelAbsoluteTolerance = tolerance
        else:
            rc = scriptcontext.doc.PageAbsoluteTolerance
            if tolerance is not None:
                scriptcontext.doc.PageAbsoluteTolerance = tolerance
        return rc
    *)


    [<EXT>]
     ///<summary>Return the document's angle tolerance. Angle tolerance is
    /// measured in degrees. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details</summary>
    ///<returns>(float) The current angle tolerance</returns>
    static member UnitAngleTolerance() : float = //GET
       Doc.ModelAngleToleranceDegrees 
    (*
    def UnitAngleTolerance(angle_tolerance_degrees=None, in_model_units=True):
        '''Return or set the document's angle tolerance. Angle tolerance is
        measured in degrees. See Rhino's DocumentProperties command
        (Units and Page Units Window) for details
        Parameters:
          angle_tolerance_degrees (number, optional): the angle tolerance to set
          in_model_units (number, optional): Return or modify the document's model units (True)
                                 or the document's page units (False)
        Returns:
          number: if angle_tolerance_degrees is not specified, the current angle tolerance
          number: if angle_tolerance_degrees is specified, the previous angle tolerance
        '''
        if in_model_units:
            rc = scriptcontext.doc.ModelAngleToleranceDegrees
            if angle_tolerance_degrees is not None:
                scriptcontext.doc.ModelAngleToleranceDegrees = angle_tolerance_degrees
        else:
            rc = scriptcontext.doc.PageAngleToleranceDegrees
            if angle_tolerance_degrees is not None:
                scriptcontext.doc.PageAngleToleranceDegrees = angle_tolerance_degrees
        return rc
    *)

    [<EXT>]
     ///<summary>Set the document's angle tolerance. Angle tolerance is
    /// measured in degrees. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details</summary>
    ///<param name="angleToleranceDegrees">(float)The angle tolerance to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member UnitAngleTolerance(angleToleranceDegrees:float) : unit = //SET
            if angleToleranceDegrees > 0. then
                Doc.ModelAngleToleranceDegrees <- angleToleranceDegrees
            else
                failwithf "UnitAngleTolerance failed.  angleToleranceDegrees:'%A'" angleToleranceDegrees
    (*
    def UnitAngleTolerance(angle_tolerance_degrees=None, in_model_units=True):
        '''Return or set the document's angle tolerance. Angle tolerance is
        measured in degrees. See Rhino's DocumentProperties command
        (Units and Page Units Window) for details
        Parameters:
          angle_tolerance_degrees (number, optional): the angle tolerance to set
          in_model_units (number, optional): Return or modify the document's model units (True)
                                 or the document's page units (False)
        Returns:
          number: if angle_tolerance_degrees is not specified, the current angle tolerance
          number: if angle_tolerance_degrees is specified, the previous angle tolerance
        '''
        if in_model_units:
            rc = scriptcontext.doc.ModelAngleToleranceDegrees
            if angle_tolerance_degrees is not None:
                scriptcontext.doc.ModelAngleToleranceDegrees = angle_tolerance_degrees
        else:
            rc = scriptcontext.doc.PageAngleToleranceDegrees
            if angle_tolerance_degrees is not None:
                scriptcontext.doc.PageAngleToleranceDegrees = angle_tolerance_degrees
        return rc
    *)


    [<EXT>]
     ///<summary>Return the document's distance display precision</summary>
    ///<returns>(int) The current distance display precision .</returns>
    static member UnitDistanceDisplayPrecision() : int = //GET
        Doc.ModelDistanceDisplayPrecision 
    (*
    def UnitDistanceDisplayPrecision(precision=None, model_units=True):
        '''Return or set the document's distance display precision
        Parameters:
          precision (number, optional): The distance display precision.  If the current distance display mode is Decimal, then precision is the number of decimal places.
                                        If the current distance display mode is Fractional (including Feet and Inches), then the denominator = (1/2)^precision.
                                        Use UnitDistanceDisplayMode to get the current distance display mode.
          model_units (bool, optional): Return or modify the document's model units (True) or the document's page units (False). The default is True.
        Returns:
         number: If precision is not specified, the current distance display precision if successful. 
         number: If precision is specified, the previous distance display precision if successful. 
        '''
        if model_units:
            rc = scriptcontext.doc.ModelDistanceDisplayPrecision
            if precision: scriptcontext.doc.ModelDistanceDisplayPrecision = precision
            return rc
        rc = scriptcontext.doc.PageDistanceDisplayPrecision
        if precision: scriptcontext.doc.PageDistanceDisplayPrecision = precision
        return rc
    *)

    [<EXT>]
     ///<summary>Set the document's distance display precision</summary>
    ///<param name="precision">(int)The distance display precision.  If the current distance display mode is Decimal, then precision is the number of decimal places.
    ///  If the current distance display mode is Fractional (including Feet and Inches), then the denominator = (1/2)^precision.
    ///  Use UnitDistanceDisplayMode to get the current distance display mode.</param>
    ///<returns>(unit) void, nothing</returns>
    static member UnitDistanceDisplayPrecision(precision:int) : unit = //SET
            Doc.ModelDistanceDisplayPrecision <- precision
    (*
    def UnitDistanceDisplayPrecision(precision=None, model_units=True):
        '''Return or set the document's distance display precision
        Parameters:
          precision (number, optional): The distance display precision.  If the current distance display mode is Decimal, then precision is the number of decimal places.
                                        If the current distance display mode is Fractional (including Feet and Inches), then the denominator = (1/2)^precision.
                                        Use UnitDistanceDisplayMode to get the current distance display mode.
          model_units (bool, optional): Return or modify the document's model units (True) or the document's page units (False). The default is True.
        Returns:
         number: If precision is not specified, the current distance display precision if successful. 
         number: If precision is specified, the previous distance display precision if successful. 
        '''
        if model_units:
            rc = scriptcontext.doc.ModelDistanceDisplayPrecision
            if precision: scriptcontext.doc.ModelDistanceDisplayPrecision = precision
            return rc
        rc = scriptcontext.doc.PageDistanceDisplayPrecision
        if precision: scriptcontext.doc.PageDistanceDisplayPrecision = precision
        return rc
    *)


    [<EXT>]
     ///<summary>Return the document's relative tolerance. Relative tolerance
    /// is measured in percent. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details</summary>
    ///<returns>(float) The current tolerance in percent</returns>
    static member UnitRelativeTolerance() : float = //GET
         Doc.ModelRelativeTolerance 
    (*
    def UnitRelativeTolerance(relative_tolerance=None, in_model_units=True):
        '''Return or set the document's relative tolerance. Relative tolerance
        is measured in percent. See Rhino's DocumentProperties command
        (Units and Page Units Window) for details
        Parameters:
          relative_tolerance (number, optional) the relative tolerance in percent
          in_model_units (bool, optional): Return or modify the document's model units (True)
                                 or the document's page units (False)
        Returns:
          number: if relative_tolerance is not specified, the current tolerance in percent
          number: if relative_tolerance is specified, the previous tolerance in percent
        '''
        if in_model_units:
            rc = scriptcontext.doc.ModelRelativeTolerance
            if relative_tolerance is not None:
                scriptcontext.doc.ModelRelativeTolerance = relative_tolerance
        else:
            rc = scriptcontext.doc.PageRelativeTolerance
            if relative_tolerance is not None:
                scriptcontext.doc.PageRelativeTolerance = relative_tolerance
        return rc
    *)

    [<EXT>]
     ///<summary>Set the document's relative tolerance. Relative tolerance
    /// is measured in percent. See Rhino's DocumentProperties command
    /// (Units and Page Units Window) for details</summary>
    ///<param name="relativeTolerance">(float)The relative tolerance in percent</param>    
    ///<returns>(unit) void, nothing</returns>
    static member UnitRelativeTolerance(relativeTolerance:float) : unit = //SET
            if relativeTolerance > 0.0 then
                Doc.ModelRelativeTolerance <- relativeTolerance
            else
              failwithf "UnitRelativeTolerance failed.  relativeTolerance:'%A'" relativeTolerance
    (*
    def UnitRelativeTolerance(relative_tolerance=None, in_model_units=True):
        '''Return or set the document's relative tolerance. Relative tolerance
        is measured in percent. See Rhino's DocumentProperties command
        (Units and Page Units Window) for details
        Parameters:
          relative_tolerance (number, optional) the relative tolerance in percent
          in_model_units (bool, optional): Return or modify the document's model units (True)
                                 or the document's page units (False)
        Returns:
          number: if relative_tolerance is not specified, the current tolerance in percent
          number: if relative_tolerance is specified, the previous tolerance in percent
        '''
        if in_model_units:
            rc = scriptcontext.doc.ModelRelativeTolerance
            if relative_tolerance is not None:
                scriptcontext.doc.ModelRelativeTolerance = relative_tolerance
        else:
            rc = scriptcontext.doc.PageRelativeTolerance
            if relative_tolerance is not None:
                scriptcontext.doc.PageRelativeTolerance = relative_tolerance
        return rc
    *)


    [<EXT>]
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
    ///<param name="fromSystem">(int) The unit system to convert from (see above)</param>
    ///<returns>(float) scale factor for changing between unit systems</returns>
    static member UnitScale(toSystem:int, fromSystem:int) : float =
      let toSystemt:UnitSystem   = LanguagePrimitives.EnumOfValue  (byte toSystem)
      let fromSystemt:UnitSystem  = LanguagePrimitives.EnumOfValue (byte fromSystem)
      RhinoMath.UnitScale(fromSystemt, toSystemt)
    (*
    def UnitScale(to_system, from_system=None):
        '''Return the scale factor for changing between unit systems.
      Parameters:
        to_system (number): The unit system to convert to. The unit systems are are:
           0 - No unit system
           1 - Microns (1.0e-6 meters)
           2 - Millimeters (1.0e-3 meters)
           3 - Centimeters (1.0e-2 meters)
           4 - Meters
           5 - Kilometers (1.0e+3 meters)
           6 - Microinches (2.54e-8 meters, 1.0e-6 inches)
           7 - Mils (2.54e-5 meters, 0.001 inches)
           8 - Inches (0.0254 meters)
           9 - Feet (0.3408 meters, 12 inches)
          10 - Miles (1609.344 meters, 5280 feet)
          11 - *Reserved for custom Unit System*
          12 - Angstroms (1.0e-10 meters)
          13 - Nanometers (1.0e-9 meters)
          14 - Decimeters (1.0e-1 meters)
          15 - Dekameters (1.0e+1 meters)
          16 - Hectometers (1.0e+2 meters)
          17 - Megameters (1.0e+6 meters)
          18 - Gigameters (1.0e+9 meters)
          19 - Yards (0.9144  meters, 36 inches)
          20 - Printer point (1/72 inches, computer points)
          21 - Printer pica (1/6 inches, (computer picas)
          22 - Nautical mile (1852 meters)
          23 - Astronomical (1.4959787e+11)
          24 - Lightyears (9.46073e+15 meters)
          25 - Parsecs (3.08567758e+16)
        from_system (number, optional): the unit system to convert from (see above). If omitted,
            the document's current unit system is used
      Returns:
        number: scale factor for changing between unit systems
      '''
      if from_system is None:
          from_system = scriptcontext.doc.ModelUnitSystem
      if type(from_system) is int:
          from_system = System.Enum.ToObject(Rhino.UnitSystem, from_system)
      if type(to_system) is int:
          to_system = System.Enum.ToObject(Rhino.UnitSystem, to_system)
      return Rhino.RhinoMath.UnitScale(from_system, to_system)
    *)


    [<EXT>]
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
    static member UnitSystem() : int = //GET        
            int(Doc.ModelUnitSystem)
    (*
    def UnitSystem(unit_system=None, scale=False, in_model_units=True):
        '''Return or set the document's unit system. See Rhino's DocumentProperties
        command (Units and Page Units Window) for details
        Parameters:
          unit_system (number, optional): The unit system to set the document to. The unit systems are:
             0 - No unit system
             1 - Microns (1.0e-6 meters)
             2 - Millimeters (1.0e-3 meters)
             3 - Centimeters (1.0e-2 meters)
             4 - Meters
             5 - Kilometers (1.0e+3 meters)
             6 - Microinches (2.54e-8 meters, 1.0e-6 inches)
             7 - Mils (2.54e-5 meters, 0.001 inches)
             8 - Inches (0.0254 meters)
             9 - Feet (0.3408 meters, 12 inches)
            10 - Miles (1609.344 meters, 5280 feet)
            11 - *Reserved for custom Unit System*
            12 - Angstroms (1.0e-10 meters)
            13 - Nanometers (1.0e-9 meters)
            14 - Decimeters (1.0e-1 meters)
            15 - Dekameters (1.0e+1 meters)
            16 - Hectometers (1.0e+2 meters)
            17 - Megameters (1.0e+6 meters)
            18 - Gigameters (1.0e+9 meters)
            19 - Yards (0.9144  meters, 36 inches)
            20 - Printer point (1/72 inches, computer points)
            21 - Printer pica (1/6 inches, (computer picas)
            22 - Nautical mile (1852 meters)
            23 - Astronomical (1.4959787e+11)
            24 - Lightyears (9.46073e+15 meters)
            25 - Parsecs (3.08567758e+16)
          scale (bool, optional): Scale existing geometry based on the new unit system.
              If not specified, any existing geometry is not scaled (False)
          in_model_units (bool, optional): Return or modify the document's model units (True)
              or the document's page units (False). The default is True.
        Returns:
          number: if unit_system is not specified, the current unit system
          number: if unit_system is specified, the previous unit system
          None: on error
        '''
        if (unit_system is not None and (unit_system<1 or unit_system>25)):
            raise ValueError("unit_system value of %s is not valid"%unit_system)
        if in_model_units:
            rc = int(scriptcontext.doc.ModelUnitSystem)
            if unit_system is not None:
                unit_system = System.Enum.ToObject(Rhino.UnitSystem, unit_system)
                scriptcontext.doc.AdjustModelUnitSystem(unit_system, scale)
        else:
            rc = int(scriptcontext.doc.PageUnitSystem)
            if unit_system is not None:
                unit_system = System.Enum.ToObject(Rhino.UnitSystem, unit_system)
                scriptcontext.doc.AdjustPageUnitSystem(unit_system, scale)
        return rc
    *)

    [<EXT>]
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
    ///<returns>(unit) void, nothing</returns>
    static member UnitSystem(unitSystem:int, [<OPT;DEF(false)>]scale:bool) : unit = //SET
        if unitSystem < 1 || unitSystem > 25 then
            failwithf "unitSystem value of %d is not  valid" unitSystem        
            let unitSystem : UnitSystem  = LanguagePrimitives.EnumOfValue (byte unitSystem)
            Doc.AdjustPageUnitSystem(unitSystem, scale)
    (*
    def UnitSystem(unit_system=None, scale=False, in_model_units=True):
        '''Return or set the document's unit system. See Rhino's DocumentProperties
        command (Units and Page Units Window) for details
        Parameters:
          unit_system (number, optional): The unit system to set the document to. The unit systems are:
             0 - No unit system
             1 - Microns (1.0e-6 meters)
             2 - Millimeters (1.0e-3 meters)
             3 - Centimeters (1.0e-2 meters)
             4 - Meters
             5 - Kilometers (1.0e+3 meters)
             6 - Microinches (2.54e-8 meters, 1.0e-6 inches)
             7 - Mils (2.54e-5 meters, 0.001 inches)
             8 - Inches (0.0254 meters)
             9 - Feet (0.3408 meters, 12 inches)
            10 - Miles (1609.344 meters, 5280 feet)
            11 - *Reserved for custom Unit System*
            12 - Angstroms (1.0e-10 meters)
            13 - Nanometers (1.0e-9 meters)
            14 - Decimeters (1.0e-1 meters)
            15 - Dekameters (1.0e+1 meters)
            16 - Hectometers (1.0e+2 meters)
            17 - Megameters (1.0e+6 meters)
            18 - Gigameters (1.0e+9 meters)
            19 - Yards (0.9144  meters, 36 inches)
            20 - Printer point (1/72 inches, computer points)
            21 - Printer pica (1/6 inches, (computer picas)
            22 - Nautical mile (1852 meters)
            23 - Astronomical (1.4959787e+11)
            24 - Lightyears (9.46073e+15 meters)
            25 - Parsecs (3.08567758e+16)
          scale (bool, optional): Scale existing geometry based on the new unit system.
              If not specified, any existing geometry is not scaled (False)
          in_model_units (bool, optional): Return or modify the document's model units (True)
              or the document's page units (False). The default is True.
        Returns:
          number: if unit_system is not specified, the current unit system
          number: if unit_system is specified, the previous unit system
          None: on error
        '''
        if (unit_system is not None and (unit_system<1 or unit_system>25)):
            raise ValueError("unit_system value of %s is not valid"%unit_system)
        if in_model_units:
            rc = int(scriptcontext.doc.ModelUnitSystem)
            if unit_system is not None:
                unit_system = System.Enum.ToObject(Rhino.UnitSystem, unit_system)
                scriptcontext.doc.AdjustModelUnitSystem(unit_system, scale)
        else:
            rc = int(scriptcontext.doc.PageUnitSystem)
            if unit_system is not None:
                unit_system = System.Enum.ToObject(Rhino.UnitSystem, unit_system)
                scriptcontext.doc.AdjustPageUnitSystem(unit_system, scale)
        return rc
    *)


   
    [<EXT>]
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
        Doc.GetUnitSystemName(modelUnits, capitalize, singular, abbreviate)
    (*
    def UnitSystemName(capitalize=False, singular=True, abbreviate=False, model_units=True):
        '''Returns the name of the current unit system
        Parameters:
          capitalize (bool, optional): Capitalize the first character of the units system name (e.g. return "Millimeter" instead of "millimeter"). The default is not to capitalize the first character (false).
          singular (bool, optional): Return the singular form of the units system name (e.g. "millimeter" instead of "millimeters"). The default is to return the singular form of the name (true).
          abbreviate (bool, optional): Abbreviate the name of the units system (e.g. return "mm" instead of "millimeter"). The default is not to abbreviate the name (false).
          model_units (bool, optional): Return the document's model units (True) or the document's page units (False). The default is True.
        Returns:
          str: The name of the current units system if successful.
        '''
        return scriptcontext.doc.GetUnitSystemName(model_units, capitalize, singular, abbreviate)
    *)


