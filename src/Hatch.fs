namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsHatch =
  [<EXT>] 
  type RhinoScriptSyntax with
    
    [<EXT>]
    
    static member internal InitHatchPatterns() : obj =
        failNotImpl () // genreation temp disabled !!
    (*
    def __initHatchPatterns():
        ''''''
    *)


    [<EXT>]
    ///<summary>Creates a new hatch object from a closed planar curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the closed planar curve that defines the
    ///  boundary of the hatch object</param>
    ///<param name="hatchPattern">(string) Optional, Default Value: <c>null:string</c>
    ///Name of the hatch pattern to be used by the hatch
    ///  object. If omitted, the current hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>
    ///Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>
    ///Hatch pattern rotation angle in degrees.</param>
    ///<returns>(Guid) identifier of the newly created hatch on success</returns>
    static member AddHatch(curveId:Guid, [<OPT;DEF(null:string)>]hatchPattern:string, [<OPT;DEF(1.0)>]scale:float, [<OPT;DEF(0.0)>]rotation:float) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddHatch(curve_id, hatch_pattern=None, scale=1.0, rotation=0.0):
        '''Creates a new hatch object from a closed planar curve object
        Parameters:
          curve_id (guid): identifier of the closed planar curve that defines the
              boundary of the hatch object
          hatch_pattern (str, optional): name of the hatch pattern to be used by the hatch
              object. If omitted, the current hatch pattern will be used
          scale (number, optional): hatch pattern scale factor
          rotation (number, optional): hatch pattern rotation angle in degrees.
        Returns:
          guid:identifier of the newly created hatch on success
          None on error
        '''
        rc = AddHatches(curve_id, hatch_pattern, scale, rotation)
        if rc: return rc[0]
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Creates one or more new hatch objects a list of closed planar curves</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of the closed planar curves that defines the
    ///  boundary of the hatch objects</param>
    ///<param name="hatchPattern">(string) Optional, Default Value: <c>null:string</c>
    ///Name of the hatch pattern to be used by the hatch
    ///  object. If omitted, the current hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>
    ///Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>
    ///Hatch pattern rotation angle in degrees.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///Tolerance for hatch fills.</param>
    ///<returns>(Guid seq) identifiers of the newly created hatch on success</returns>
    static member AddHatches(curveIds:Guid seq, [<OPT;DEF(null:string)>]hatchPattern:string, [<OPT;DEF(1.0)>]scale:float, [<OPT;DEF(0.0)>]rotation:float, [<OPT;DEF(0.0)>]tolerance:float) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddHatches(curve_ids, hatch_pattern=None, scale=1.0, rotation=0.0, tolerance=None):
        '''Creates one or more new hatch objects a list of closed planar curves
        Parameters:
          curve_ids ([guid, ...]): identifiers of the closed planar curves that defines the
              boundary of the hatch objects
          hatch_pattern (str, optional):  name of the hatch pattern to be used by the hatch
              object. If omitted, the current hatch pattern will be used
          scale (number, optional): hatch pattern scale factor
          rotation (number, optional): hatch pattern rotation angle in degrees.
          tolerance (number, optional): tolerance for hatch fills.
        Returns:
          list(guid, ...): identifiers of the newly created hatch on success
          None: on error
        '''
        __initHatchPatterns()
        id = rhutil.coerceguid(curve_ids, False)
        if id: curve_ids = [id]
        index = scriptcontext.doc.HatchPatterns.CurrentHatchPatternIndex
        if hatch_pattern and hatch_pattern!=index:
            if isinstance(hatch_pattern, int):
                index = hatch_pattern
            else:
                pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
                index = Rhino.RhinoMath.UnsetIntIndex if pattern_instance is None else pattern_instance.Index
            if index<0: return scriptcontext.errorhandler()
        curves = [rhutil.coercecurve(id, -1, True) for id in curve_ids]
        rotation = Rhino.RhinoMath.ToRadians(rotation)
        if tolerance is None or tolerance < 0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        hatches = Rhino.Geometry.Hatch.Create(curves, index, rotation, scale, tolerance)
        if not hatches: return scriptcontext.errorhandler()
        ids = []
        for hatch in hatches:
            id = scriptcontext.doc.Objects.AddHatch(hatch)
            if id==System.Guid.Empty: continue
            ids.append(id)
        if not ids: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return ids
    *)


    [<EXT>]
    ///<summary>Adds hatch patterns to the document by importing hatch pattern definitions
    ///  from a pattern file.</summary>
    ///<param name="filename">(string) Name of the hatch pattern file</param>
    ///<param name="replace">(bool) Optional, Default Value: <c>false</c>
    ///If hatch pattern names already in the document match hatch
    ///  pattern names in the pattern definition file, then the existing hatch
    ///  patterns will be redefined</param>
    ///<returns>(string seq) Names of the newly added hatch patterns</returns>
    static member AddHatchPatterns(filename:string, [<OPT;DEF(false)>]replace:bool) : string seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddHatchPatterns(filename, replace=False):
        '''Adds hatch patterns to the document by importing hatch pattern definitions
        from a pattern file.
        Parameters:
          filename (str): name of the hatch pattern file
          replace (bool, optional): If hatch pattern names already in the document match hatch
              pattern names in the pattern definition file, then the existing hatch
              patterns will be redefined
        Returns:
          list(str, ...): Names of the newly added hatch patterns if successful
          None: on error
        '''
        patterns = Rhino.DocObjects.HatchPattern.ReadFromFile(filename, True)
        if not patterns: return scriptcontext.errorhandler()
        rc = []
        for pattern in patterns:
             index = scriptcontext.doc.HatchPatterns.Add(pattern)
             if index>=0:
                 pattern = scriptcontext.doc.HatchPatterns[index]
                 rc.append(pattern.Name)
        if not rc: return scriptcontext.errorhandler()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the current hatch pattern file</summary>
    ///<returns>(string) The current hatch pattern</returns>
    static member CurrentHatchPattern() : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def CurrentHatchPattern(hatch_pattern=None):
        '''Returns or sets the current hatch pattern file
        Parameters:
          hatch_pattern(str, optional):  name of an existing hatch pattern to make current
        Returns:
          str: if hatch_pattern is not specified, the current hatch pattern
          str: if hatch_pattern is specified, the previous hatch pattern
          None: on error
        '''
        rc = scriptcontext.doc.HatchPatterns.CurrentHatchPatternIndex
        if hatch_pattern:
            __initHatchPatterns()
            pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
            if pattern_instance is None: return scriptcontext.errorhandler()
            scriptcontext.doc.HatchPatterns.CurrentHatchPatternIndex = pattern_instance.Index
        return rc
    *)

    ///<summary>Sets the current hatch pattern file</summary>
    ///<param name="hatchPattern">(string)Name of an existing hatch pattern to make current</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentHatchPattern(hatchPattern:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def CurrentHatchPattern(hatch_pattern=None):
        '''Returns or sets the current hatch pattern file
        Parameters:
          hatch_pattern(str, optional):  name of an existing hatch pattern to make current
        Returns:
          str: if hatch_pattern is not specified, the current hatch pattern
          str: if hatch_pattern is specified, the previous hatch pattern
          None: on error
        '''
        rc = scriptcontext.doc.HatchPatterns.CurrentHatchPatternIndex
        if hatch_pattern:
            __initHatchPatterns()
            pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
            if pattern_instance is None: return scriptcontext.errorhandler()
            scriptcontext.doc.HatchPatterns.CurrentHatchPatternIndex = pattern_instance.Index
        return rc
    *)


    [<EXT>]
    ///<summary>Explodes a hatch object into its component objects. The exploded objects
    ///  will be added to the document. If the hatch object uses a solid pattern,
    ///  then planar face Brep objects will be created. Otherwise, line curve objects
    ///  will be created</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///Delete the hatch object</param>
    ///<returns>(Guid seq) list of identifiers for the newly created objects</returns>
    static member ExplodeHatch(hatchId:Guid, [<OPT;DEF(false)>]delete:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExplodeHatch(hatch_id, delete=False):
        '''Explodes a hatch object into its component objects. The exploded objects
        will be added to the document. If the hatch object uses a solid pattern,
        then planar face Brep objects will be created. Otherwise, line curve objects
        will be created
        Parameters:
          hatch_id (guid): identifier of a hatch object
          delete (bool, optional): delete the hatch object
        Returns:
          list(guid, ...): list of identifiers for the newly created objects
          None: on error
        '''
        rhobj = rhutil.coercerhinoobject(hatch_id, True, True)
        pieces = rhobj.HatchGeometry.Explode()
        if not pieces: return scriptcontext.errorhandler()
        attr = rhobj.Attributes
        rc = []
        for piece in pieces:
            id = None
            if isinstance(piece, Rhino.Geometry.Curve):
                id = scriptcontext.doc.Objects.AddCurve(piece, attr)
            elif isinstance(piece, Rhino.Geometry.Brep):
                id = scriptcontext.doc.Objects.AddBrep(piece, attr)
            if id: rc.append(id)
        if delete: scriptcontext.doc.Objects.Delete(rhobj)
        return rc
    *)


    [<EXT>]
    ///<summary>Returns a hatch object's hatch pattern</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(string) The current hatch pattern</returns>
    static member HatchPattern(hatchId:Guid) : string = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchPattern(hatch_id, hatch_pattern=None):
        '''Returns or changes a hatch object's hatch pattern
        Parameters:
          hatch_id (guid): identifier of a hatch object
          hatch_pattern (str, optional): name of an existing hatch pattern to replace the
              current hatch pattern
        Returns:
          str: if hatch_pattern is not specified, the current hatch pattern
          str: if hatch_pattern is specified, the previous hatch pattern
          None: on error
        '''
        hatchobj = rhutil.coercerhinoobject(hatch_id, True, True)
        if not isinstance(hatchobj, Rhino.DocObjects.HatchObject):
            return scriptcontext.errorhandler()
        old_index = hatchobj.HatchGeometry.PatternIndex
        if hatch_pattern:
            __initHatchPatterns()
            new_patt = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
            if new_patt is None: return scriptcontext.errorhandler()
            hatchobj.HatchGeometry.PatternIndex = new_patt.Index
            hatchobj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return scriptcontext.doc.HatchPatterns[old_index].Name
    *)

    ///<summary>Changes a hatch object's hatch pattern</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="hatchPattern">(string)Name of an existing hatch pattern to replace the
    ///  current hatch pattern</param>
    ///<returns>(unit) void, nothing</returns>
    static member HatchPattern(hatchId:Guid, hatchPattern:string) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchPattern(hatch_id, hatch_pattern=None):
        '''Returns or changes a hatch object's hatch pattern
        Parameters:
          hatch_id (guid): identifier of a hatch object
          hatch_pattern (str, optional): name of an existing hatch pattern to replace the
              current hatch pattern
        Returns:
          str: if hatch_pattern is not specified, the current hatch pattern
          str: if hatch_pattern is specified, the previous hatch pattern
          None: on error
        '''
        hatchobj = rhutil.coercerhinoobject(hatch_id, True, True)
        if not isinstance(hatchobj, Rhino.DocObjects.HatchObject):
            return scriptcontext.errorhandler()
        old_index = hatchobj.HatchGeometry.PatternIndex
        if hatch_pattern:
            __initHatchPatterns()
            new_patt = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
            if new_patt is None: return scriptcontext.errorhandler()
            hatchobj.HatchGeometry.PatternIndex = new_patt.Index
            hatchobj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return scriptcontext.doc.HatchPatterns[old_index].Name
    *)


    [<EXT>]
    ///<summary>Returns the number of hatch patterns in the document</summary>
    ///<returns>(int) the number of hatch patterns in the document</returns>
    static member HatchPatternCount() : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchPatternCount():
        '''Returns the number of hatch patterns in the document
        Returns:
          number: the number of hatch patterns in the document
        '''
        __initHatchPatterns()
        return scriptcontext.doc.HatchPatterns.Count
    *)


    [<EXT>]
    ///<summary>Returns the description of a hatch pattern. Note, not all hatch patterns
    ///  have descriptions</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(string) description of the hatch pattern on success otherwise None</returns>
    static member HatchPatternDescription(hatchPattern:string) : string =
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchPatternDescription(hatch_pattern):
        '''Returns the description of a hatch pattern. Note, not all hatch patterns
        have descriptions
        Parameters:
          hatch_pattern (str): name of an existing hatch pattern
        Returns:
          str: description of the hatch pattern on success otherwise None
        '''
        __initHatchPatterns()
        pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
        if pattern_instance is None: return scriptcontext.errorhandler()
        return pattern_instance.Description
    *)


    [<EXT>]
    ///<summary>Returns the fill type of a hatch pattern.</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(int) hatch pattern's fill type
    ///  0 = solid, uses object color
    ///  1 = lines, uses pattern file definition
    ///  2 = gradient, uses fill color definition</returns>
    static member HatchPatternFillType(hatchPattern:string) : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchPatternFillType(hatch_pattern):
        '''Returns the fill type of a hatch pattern.
        Parameters:
          hatch_pattern (str): name of an existing hatch pattern
        Returns:
          number: hatch pattern's fill type if successful
                  0 = solid, uses object color
                  1 = lines, uses pattern file definition
                  2 = gradient, uses fill color definition
          None: if unsuccessful
        '''
        __initHatchPatterns()
        pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
        if pattern_instance is None: return scriptcontext.errorhandler()
        return int(pattern_instance.FillType)
    *)


    [<EXT>]
    ///<summary>Returns the names of all of the hatch patterns in the document</summary>
    ///<returns>(string seq) the names of all of the hatch patterns in the document</returns>
    static member HatchPatternNames() : string seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchPatternNames():
        '''Returns the names of all of the hatch patterns in the document
        Returns:
          list(str, ...): the names of all of the hatch patterns in the document
        '''
        __initHatchPatterns()
        rc = []
        for i in range(scriptcontext.doc.HatchPatterns.Count):
            hatchpattern = scriptcontext.doc.HatchPatterns[i]
            if hatchpattern.IsDeleted: continue
            rc.append(hatchpattern.Name)
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the rotation applied to the hatch pattern when
    /// it is mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(float) if rotation is not defined, the current rotation angle</returns>
    static member HatchRotation(hatchId:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchRotation(hatch_id, rotation=None):
        '''Returns or modifies the rotation applied to the hatch pattern when
        it is mapped to the hatch's plane
        Parameters:
          hatch_id (guid): identifier of a hatch object
          rotation (number, optional): rotation angle in degrees
        Returns:
          number: if rotation is not defined, the current rotation angle
          number: if rotation is specified, the previous rotation angle
          None: on error
        '''
        hatchobj = rhutil.coercerhinoobject(hatch_id, True, True)
        if not isinstance(hatchobj, Rhino.DocObjects.HatchObject):
            return scriptcontext.errorhandler()
        rc = hatchobj.HatchGeometry.PatternRotation
        rc = Rhino.RhinoMath.ToDegrees(rc)
        if rotation is not None and rotation!=rc:
            rotation = Rhino.RhinoMath.ToRadians(rotation)
            hatchobj.HatchGeometry.PatternRotation = rotation
            hatchobj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the rotation applied to the hatch pattern when
    /// it is mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="rotation">(float)Rotation angle in degrees</param>
    ///<returns>(unit) void, nothing</returns>
    static member HatchRotation(hatchId:Guid, rotation:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchRotation(hatch_id, rotation=None):
        '''Returns or modifies the rotation applied to the hatch pattern when
        it is mapped to the hatch's plane
        Parameters:
          hatch_id (guid): identifier of a hatch object
          rotation (number, optional): rotation angle in degrees
        Returns:
          number: if rotation is not defined, the current rotation angle
          number: if rotation is specified, the previous rotation angle
          None: on error
        '''
        hatchobj = rhutil.coercerhinoobject(hatch_id, True, True)
        if not isinstance(hatchobj, Rhino.DocObjects.HatchObject):
            return scriptcontext.errorhandler()
        rc = hatchobj.HatchGeometry.PatternRotation
        rc = Rhino.RhinoMath.ToDegrees(rc)
        if rotation is not None and rotation!=rc:
            rotation = Rhino.RhinoMath.ToRadians(rotation)
            hatchobj.HatchGeometry.PatternRotation = rotation
            hatchobj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the scale applied to the hatch pattern when it is
    /// mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(float) if scale is not defined, the current scale factor</returns>
    static member HatchScale(hatchId:Guid) : float = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchScale(hatch_id, scale=None):
        '''Returns or modifies the scale applied to the hatch pattern when it is
        mapped to the hatch's plane
        Parameters:
          hatch_id (guid): identifier of a hatch object
          scale (number, optional):  scale factor
        Returns:
          number: if scale is not defined, the current scale factor
          number: if scale is defined, the previous scale factor
          None: on error
        '''
        hatchobj = rhutil.coercerhinoobject(hatch_id)
        if not isinstance(hatchobj, Rhino.DocObjects.HatchObject):
            return scriptcontext.errorhandler()
        rc = hatchobj.HatchGeometry.PatternScale
        if scale and scale!=rc:
            hatchobj.HatchGeometry.PatternScale = scale
            hatchobj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the scale applied to the hatch pattern when it is
    /// mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="scale">(float)Scale factor</param>
    ///<returns>(unit) void, nothing</returns>
    static member HatchScale(hatchId:Guid, scale:float) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def HatchScale(hatch_id, scale=None):
        '''Returns or modifies the scale applied to the hatch pattern when it is
        mapped to the hatch's plane
        Parameters:
          hatch_id (guid): identifier of a hatch object
          scale (number, optional):  scale factor
        Returns:
          number: if scale is not defined, the current scale factor
          number: if scale is defined, the previous scale factor
          None: on error
        '''
        hatchobj = rhutil.coercerhinoobject(hatch_id)
        if not isinstance(hatchobj, Rhino.DocObjects.HatchObject):
            return scriptcontext.errorhandler()
        rc = hatchobj.HatchGeometry.PatternScale
        if scale and scale!=rc:
            hatchobj.HatchGeometry.PatternScale = scale
            hatchobj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Verifies the existence of a hatch object in the document</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatch(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsHatch(object_id):
        '''Verifies the existence of a hatch object in the document
        Parameters:
          object_id (guid): identifier of an object
        Returns:
          bool: True or False
        '''
        rhobj = rhutil.coercerhinoobject(object_id, True, False)
        return isinstance(rhobj, Rhino.DocObjects.HatchObject)
    *)


    [<EXT>]
    ///<summary>Verifies the existence of a hatch pattern in the document</summary>
    ///<param name="name">(string) The name of a hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPattern(name:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsHatchPattern(name):
        '''Verifies the existence of a hatch pattern in the document
        Parameters:
          name (str): the name of a hatch pattern
        Returns:
          bool: True or False
        '''
        __initHatchPatterns()
        return scriptcontext.doc.HatchPatterns.FindName(name) is not None
    *)


    [<EXT>]
    ///<summary>Verifies that a hatch pattern is the current hatch pattern</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternCurrent(hatchPattern:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsHatchPatternCurrent(hatch_pattern):
        '''Verifies that a hatch pattern is the current hatch pattern
        Parameters:
          hatch_pattern (str): name of an existing hatch pattern
        Returns:
          bool: True or False
          None: on error
        '''
        __initHatchPatterns()
        pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
        if pattern_instance is None: return scriptcontext.errorhandler()
        return pattern_instance.Index==scriptcontext.doc.HatchPatterns.CurrentHatchPatternIndex
    *)


    [<EXT>]
    ///<summary>Verifies that a hatch pattern is from a reference file</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternReference(hatchPattern:string) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsHatchPatternReference(hatch_pattern):
        '''Verifies that a hatch pattern is from a reference file
        Parameters:
          hatch_pattern (str): name of an existing hatch pattern
        Returns:
          bool: True or False
          None: on error
        '''
        __initHatchPatterns()
        pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
        if pattern_instance is None: return scriptcontext.errorhandler()
        return pattern_instance.IsReference
    *)


