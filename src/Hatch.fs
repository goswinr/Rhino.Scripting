namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsHatch =
  type RhinoScriptSyntax with
    
    
    static member internal InitHatchPatterns() : obj =
    (*
    def __initHatchPatterns():
        if scriptcontext.doc.HatchPatterns.FindName(Rhino.DocObjects.HatchPattern.Defaults.Solid.Name) is None:
            scriptcontext.doc.HatchPatterns.Add(Rhino.DocObjects.HatchPattern.Defaults.Solid)
        if scriptcontext.doc.HatchPatterns.FindName(Rhino.DocObjects.HatchPattern.Defaults.Hatch1.Name) is None:
            scriptcontext.doc.HatchPatterns.Add(Rhino.DocObjects.HatchPattern.Defaults.Hatch1)
        if scriptcontext.doc.HatchPatterns.FindName(Rhino.DocObjects.HatchPattern.Defaults.Hatch2.Name) is None:
            scriptcontext.doc.HatchPatterns.Add(Rhino.DocObjects.HatchPattern.Defaults.Hatch2)
        if scriptcontext.doc.HatchPatterns.FindName(Rhino.DocObjects.HatchPattern.Defaults.Hatch3.Name) is None:
            scriptcontext.doc.HatchPatterns.Add(Rhino.DocObjects.HatchPattern.Defaults.Hatch3)
        if scriptcontext.doc.HatchPatterns.FindName(Rhino.DocObjects.HatchPattern.Defaults.Dash.Name) is None:
            scriptcontext.doc.HatchPatterns.Add(Rhino.DocObjects.HatchPattern.Defaults.Dash)
        if scriptcontext.doc.HatchPatterns.FindName(Rhino.DocObjects.HatchPattern.Defaults.Grid.Name) is None:
            scriptcontext.doc.HatchPatterns.Add(Rhino.DocObjects.HatchPattern.Defaults.Grid)
        if scriptcontext.doc.HatchPatterns.FindName(Rhino.DocObjects.HatchPattern.Defaults.Grid60.Name) is None:
            scriptcontext.doc.HatchPatterns.Add(Rhino.DocObjects.HatchPattern.Defaults.Grid60)
        if scriptcontext.doc.HatchPatterns.FindName(Rhino.DocObjects.HatchPattern.Defaults.Plus.Name) is None:
            scriptcontext.doc.HatchPatterns.Add(Rhino.DocObjects.HatchPattern.Defaults.Plus)
        if scriptcontext.doc.HatchPatterns.FindName(Rhino.DocObjects.HatchPattern.Defaults.Squares.Name) is None:
            scriptcontext.doc.HatchPatterns.Add(Rhino.DocObjects.HatchPattern.Defaults.Squares)
    *)


    ///<summary>Creates a new hatch object from a closed planar curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the closed planar curve that defines the
    ///  boundary of the hatch object</param>
    ///<param name="hatchPattern">(string) Optional, Default Value: <c>null</c>
    ///Name of the hatch pattern to be used by the hatch
    ///  object. If omitted, the current hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>
    ///Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>
    ///Hatch pattern rotation angle in degrees.</param>
    ///<returns>(Guid) identifier of the newly created hatch on success</returns>
    static member AddHatch(curveId:Guid, [<OPT;DEF(null)>]hatchPattern:string, [<OPT;DEF(1.0)>]scale:float, [<OPT;DEF(0.0)>]rotation:float) : Guid =
        let rc = AddHatches(curve_id, hatchPattern, scale, rotation)
        if rc then rc.[0]
        failwithf "Rhino.Scripting Error:AddHatch failed.  curveId:"%A" hatchPattern:"%A" scale:"%A" rotation:"%A"" curveId hatchPattern scale rotation
    (*
    def AddHatch(curve_id, hatch_pattern=None, scale=1.0, rotation=0.0):
        """Creates a new hatch object from a closed planar curve object
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
        Example:
          import rhinoscriptsyntax as rs
          circle = rs.AddCircle(rs.WorldXYPlane(), 10.0)
          if rs.IsHatchPattern("Grid"):
              rs.AddHatch( circle, "Grid" )
          else:
              rs.AddHatch( circle, rs.CurrentHatchPattern() )
        See Also:
          AddHatches
          CurrentHatchPattern
          HatchPatternNames
        """
        rc = AddHatches(curve_id, hatch_pattern, scale, rotation)
        if rc: return rc[0]
        return scriptcontext.errorhandler()
    *)


    ///<summary>Creates one or more new hatch objects a list of closed planar curves</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of the closed planar curves that defines the
    ///  boundary of the hatch objects</param>
    ///<param name="hatchPattern">(string) Optional, Default Value: <c>null</c>
    ///Name of the hatch pattern to be used by the hatch
    ///  object. If omitted, the current hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>
    ///Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>
    ///Hatch pattern rotation angle in degrees.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///Tolerance for hatch fills.</param>
    ///<returns>(Guid seq) identifiers of the newly created hatch on success</returns>
    static member AddHatches(curveIds:Guid seq, [<OPT;DEF(null)>]hatchPattern:string, [<OPT;DEF(1.0)>]scale:float, [<OPT;DEF(0.0)>]rotation:float, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        __initHatchPatterns()
        let id = Coerce.coerceguid(curve_ids, false)
        if id then curve_ids <- .[id]
        let index = Doc.HatchPatterns.CurrentHatchPatternIndex
        if hatchPattern && hatchPattern<>index then
            if isinstance(hatchPattern, int) then
                index <- hatchPattern
            else
                let pattern_instance = Doc.HatchPatterns.FindName(hatchPattern)
                index <- Rhino.RhinoMath.RhinoMath.UnsetIntIndex if pattern_instance = null else pattern_instance.Index
            if index<0 then failwithf "Rhino.Scripting Error:AddHatches failed.  curveIds:"%A" hatchPattern:"%A" scale:"%A" rotation:"%A" tolerance:"%A"" curveIds hatchPattern scale rotation tolerance
        let curves = [| for id in curve_ids -> Coerce.coercecurve(id, -1, true) |]
        let rotation = Rhino.RhinoMath.ToRadians(rotation)
        if tolerance = null || tolerance < 0 then
            let tolerance = Doc.ModelAbsoluteTolerance
        let hatches = Hatch.Create(curves, index, rotation, scale, tolerance)
        if not <| hatches then failwithf "Rhino.Scripting Error:AddHatches failed.  curveIds:"%A" hatchPattern:"%A" scale:"%A" rotation:"%A" tolerance:"%A"" curveIds hatchPattern scale rotation tolerance
        let ids = ResizeArray()
        for hatch in hatches do
            id <- Doc.Objects.AddHatch(hatch)
            if id=Guid.Empty then continue
            ids.Add(id)
        if not <| ids then failwithf "Rhino.Scripting Error:AddHatches failed.  curveIds:"%A" hatchPattern:"%A" scale:"%A" rotation:"%A" tolerance:"%A"" curveIds hatchPattern scale rotation tolerance
        Doc.Views.Redraw()
        ids
    (*
    def AddHatches(curve_ids, hatch_pattern=None, scale=1.0, rotation=0.0, tolerance=None):
        """Creates one or more new hatch objects a list of closed planar curves
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
        Example:
          import rhinoscriptsyntax as rs
          curves = rs.GetObjects("Select closed planar curves", rs.filter.curve)
          if curves:
              if rs.IsHatchPattern("Grid"):
                  rs.AddHatches( curves, "Grid" )
              else:
                  rs.AddHatches( curves, rs.CurrentHatchPattern() )
        See Also:
          AddHatch
          CurrentHatchPattern
          HatchPatternNames
        """
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


    ///<summary>Adds hatch patterns to the document by importing hatch pattern definitions
    ///  from a pattern file.</summary>
    ///<param name="filename">(string) Name of the hatch pattern file</param>
    ///<param name="replace">(bool) Optional, Default Value: <c>false</c>
    ///If hatch pattern names already in the document match hatch
    ///  pattern names in the pattern definition file, then the existing hatch
    ///  patterns will be redefined</param>
    ///<returns>(string seq) Names of the newly added hatch patterns</returns>
    static member AddHatchPatterns(filename:string, [<OPT;DEF(false)>]replace:bool) : string seq =
        let patterns = Rhino.DocObjects.HatchPattern.ReadFromFile(filename, true)
        if not <| patterns then failwithf "Rhino.Scripting Error:AddHatchPatterns failed.  filename:"%A" replace:"%A"" filename replace
        let rc = ResizeArray()
        for pattern in patterns do
             let index = Doc.HatchPatterns.Add(pattern)
             if index>=0 then
                 let pattern = Doc.HatchPatterns.[index]
                 rc.Add(pattern.Name)
        if not <| rc then failwithf "Rhino.Scripting Error:AddHatchPatterns failed.  filename:"%A" replace:"%A"" filename replace
        rc
    (*
    def AddHatchPatterns(filename, replace=False):
        """Adds hatch patterns to the document by importing hatch pattern definitions
        from a pattern file.
        Parameters:
          filename (str): name of the hatch pattern file
          replace (bool, optional): If hatch pattern names already in the document match hatch
              pattern names in the pattern definition file, then the existing hatch
              patterns will be redefined
        Returns:
          list(str, ...): Names of the newly added hatch patterns if successful
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          filename = rs.OpenFileName("Import", "Pattern Files (*.pat)|*.pat||")
          if filename:
              patterns = rs.AddHatchPatterns(filename)
              if patterns:
                  for pattern in patterns: print pattern
        See Also:
          HatchPatternCount
          HatchPatternNames
        """
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


    ///<summary>Returns the current hatch pattern file</summary>
    ///<returns>(string) The current hatch pattern</returns>
    static member CurrentHatchPattern() : string =
        let rc = Doc.HatchPatterns.CurrentHatchPatternIndex
        if hatchPattern then
            __initHatchPatterns()
            let pattern_instance = Doc.HatchPatterns.FindName(hatchPattern)
            if pattern_instance = null then failwithf "Rhino.Scripting Error:CurrentHatchPattern failed.  hatchPattern:"%A"" hatchPattern
            let Doc.HatchPatterns.CurrentHatchPatternIndex = pattern_instance.Index
        rc
    (*
    def CurrentHatchPattern(hatch_pattern=None):
        """Returns or sets the current hatch pattern file
        Parameters:
          hatch_pattern(str, optional):  name of an existing hatch pattern to make current
        Returns:
          str: if hatch_pattern is not specified, the current hatch pattern
          str: if hatch_pattern is specified, the previous hatch pattern
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          if rs.IsHatchPattern("Hatch2"): rs.CurrentHatchPattern("Hatch2")
        See Also:
          HatchPatternCount
          HatchPatternNames
        """
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
    ///<returns>(unit) unit</returns>
    static member CurrentHatchPattern(hatchPattern:string) : unit =
        let rc = Doc.HatchPatterns.CurrentHatchPatternIndex
        if hatchPattern then
            __initHatchPatterns()
            let pattern_instance = Doc.HatchPatterns.FindName(hatchPattern)
            if pattern_instance = null then failwithf "Rhino.Scripting Error:CurrentHatchPattern failed.  hatchPattern:"%A"" hatchPattern
            let Doc.HatchPatterns.CurrentHatchPatternIndex = pattern_instance.Index
        rc
    (*
    def CurrentHatchPattern(hatch_pattern=None):
        """Returns or sets the current hatch pattern file
        Parameters:
          hatch_pattern(str, optional):  name of an existing hatch pattern to make current
        Returns:
          str: if hatch_pattern is not specified, the current hatch pattern
          str: if hatch_pattern is specified, the previous hatch pattern
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          if rs.IsHatchPattern("Hatch2"): rs.CurrentHatchPattern("Hatch2")
        See Also:
          HatchPatternCount
          HatchPatternNames
        """
        rc = scriptcontext.doc.HatchPatterns.CurrentHatchPatternIndex
        if hatch_pattern:
            __initHatchPatterns()
            pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
            if pattern_instance is None: return scriptcontext.errorhandler()
            scriptcontext.doc.HatchPatterns.CurrentHatchPatternIndex = pattern_instance.Index
        return rc
    *)


    ///<summary>Explodes a hatch object into its component objects. The exploded objects
    ///  will be added to the document. If the hatch object uses a solid pattern,
    ///  then planar face Brep objects will be created. Otherwise, line curve objects
    ///  will be created</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///Delete the hatch object</param>
    ///<returns>(Guid seq) list of identifiers for the newly created objects</returns>
    static member ExplodeHatch(hatchId:Guid, [<OPT;DEF(false)>]delete:bool) : Guid seq =
        let rhobj = Coerce.coercerhinoobject(hatchId, true, true)
        let pieces = rhobj.HatchGeometry.Explode()
        if not <| pieces then failwithf "Rhino.Scripting Error:ExplodeHatch failed.  hatchId:"%A" delete:"%A"" hatchId delete
        let attr = rhobj.Attributes
        let rc = ResizeArray()
        for piece in pieces do
            let id = null
            if isinstance(piece, Curve) then
                id <- Doc.Objects.AddCurve(piece, attr)
            elif isinstance(piece, Brep) then
                id <- Doc.Objects.AddBrep(piece, attr)
            if id then rc.Add(id)
        if delete then Doc.Objects.Delete(rhobj)
        rc
    (*
    def ExplodeHatch(hatch_id, delete=False):
        """Explodes a hatch object into its component objects. The exploded objects
        will be added to the document. If the hatch object uses a solid pattern,
        then planar face Brep objects will be created. Otherwise, line curve objects
        will be created
        Parameters:
          hatch_id (guid): identifier of a hatch object
          delete (bool, optional): delete the hatch object
        Returns:
          list(guid, ...): list of identifiers for the newly created objects
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          id = rs.GetObject("Select object")
          if rs.IsHatch(id): rs.ExplodeHatch(id, True)
        See Also:
          IsHatch
          HatchPattern
          HatchRotation
          HatchScale
        """
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


    ///<summary>Returns a hatch object's hatch pattern</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(string) The current hatch pattern</returns>
    static member HatchPattern(hatchId:Guid) : string =
        let hatchobj = Coerce.coercerhinoobject(hatch_id, true, true)
        if not <| isinstance(hatchobj, Rhino.DocObjects.HatchObject) then
            failwithf "Rhino.Scripting Error:HatchPattern failed.  hatchId:"%A" hatchPattern:"%A"" hatchId hatchPattern
        let old_index = hatchobj.HatchGeometry.PatternIndex
        if hatchPattern then
            _initHatchPatterns(hatchPattern)
            let new_patt = Doc.HatchPatterns.FindName(hatchPattern)
            if new_patt = null then failwithf "Rhino.Scripting Error:HatchPattern failed.  hatchId:"%A" hatchPattern:"%A"" hatchId hatchPattern
            let hatchobj.HatchGeometry.PatternIndex = new_patt.Index
            hatchobj.CommitChanges()
            Doc.Views.Redraw()
        Doc.HatchPatterns.[old_index].Name
    (*
    def HatchPattern(hatch_id, hatch_pattern=None):
        """Returns or changes a hatch object's hatch pattern
        Parameters:
          hatch_id (guid): identifier of a hatch object
          hatch_pattern (str, optional): name of an existing hatch pattern to replace the
              current hatch pattern
        Returns:
          str: if hatch_pattern is not specified, the current hatch pattern
          str: if hatch_pattern is specified, the previous hatch pattern
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          objects = rs.AllObjects()
          if objects is not None:
              for obj in objects:
                  if rs.IsHatch(obj) and rs.HatchPattern(obj)=="Solid":
                      rs.SelectObject(obj)
        See Also:
          AddHatch
          AddHatches
          HatchRotation
          HatchScale
          IsHatch
        """
        hatchobj = rhutil.coercerhinoobject(hatch_id, True, True)
        if not isinstance(hatchobj, Rhino.DocObjects.HatchObject):
            return scriptcontext.errorhandler()
        old_index = hatchobj.HatchGeometry.PatternIndex
        if hatch_pattern:
            _initHatchPatterns(hatch_pattern)
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
    ///<returns>(unit) unit</returns>
    static member HatchPattern(hatchId:Guid, hatchPattern:string) : unit =
        let hatchobj = Coerce.coercerhinoobject(hatch_id, true, true)
        if not <| isinstance(hatchobj, Rhino.DocObjects.HatchObject) then
            failwithf "Rhino.Scripting Error:HatchPattern failed.  hatchId:"%A" hatchPattern:"%A"" hatchId hatchPattern
        let old_index = hatchobj.HatchGeometry.PatternIndex
        if hatchPattern then
            _initHatchPatterns(hatchPattern)
            let new_patt = Doc.HatchPatterns.FindName(hatchPattern)
            if new_patt = null then failwithf "Rhino.Scripting Error:HatchPattern failed.  hatchId:"%A" hatchPattern:"%A"" hatchId hatchPattern
            let hatchobj.HatchGeometry.PatternIndex = new_patt.Index
            hatchobj.CommitChanges()
            Doc.Views.Redraw()
        Doc.HatchPatterns.[old_index].Name
    (*
    def HatchPattern(hatch_id, hatch_pattern=None):
        """Returns or changes a hatch object's hatch pattern
        Parameters:
          hatch_id (guid): identifier of a hatch object
          hatch_pattern (str, optional): name of an existing hatch pattern to replace the
              current hatch pattern
        Returns:
          str: if hatch_pattern is not specified, the current hatch pattern
          str: if hatch_pattern is specified, the previous hatch pattern
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          objects = rs.AllObjects()
          if objects is not None:
              for obj in objects:
                  if rs.IsHatch(obj) and rs.HatchPattern(obj)=="Solid":
                      rs.SelectObject(obj)
        See Also:
          AddHatch
          AddHatches
          HatchRotation
          HatchScale
          IsHatch
        """
        hatchobj = rhutil.coercerhinoobject(hatch_id, True, True)
        if not isinstance(hatchobj, Rhino.DocObjects.HatchObject):
            return scriptcontext.errorhandler()
        old_index = hatchobj.HatchGeometry.PatternIndex
        if hatch_pattern:
            _initHatchPatterns(hatch_pattern)
            new_patt = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
            if new_patt is None: return scriptcontext.errorhandler()
            hatchobj.HatchGeometry.PatternIndex = new_patt.Index
            hatchobj.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return scriptcontext.doc.HatchPatterns[old_index].Name
    *)


    ///<summary>Returns the number of hatch patterns in the document</summary>
    ///<returns>(int) the number of hatch patterns in the document</returns>
    static member HatchPatternCount() : int =
        __initHatchPatterns()
        Doc.HatchPatterns.Count
    (*
    def HatchPatternCount():
        """Returns the number of hatch patterns in the document
        Returns:
          number: the number of hatch patterns in the document
        Example:
          import rhinoscriptsyntax as rs
          print "There are", rs.HatchPatternCount(), "hatch patterns."
        See Also:
          HatchPatternNames
        """
        __initHatchPatterns()
        return scriptcontext.doc.HatchPatterns.Count
    *)


    ///<summary>Returns the description of a hatch pattern. Note, not all hatch patterns
    ///  have descriptions</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(string) description of the hatch pattern on success otherwise None</returns>
    static member HatchPatternDescription(hatchPattern:string) : string =
        __initHatchPatterns()
        let pattern_instance = Doc.HatchPatterns.FindName(hatchPattern)
        if pattern_instance = null then failwithf "Rhino.Scripting Error:HatchPatternDescription failed.  hatchPattern:"%A"" hatchPattern
        pattern_instance.Description
    (*
    def HatchPatternDescription(hatch_pattern):
        """Returns the description of a hatch pattern. Note, not all hatch patterns
        have descriptions
        Parameters:
          hatch_pattern (str): name of an existing hatch pattern
        Returns:
          str: description of the hatch pattern on success otherwise None
        Example:
          import rhinoscriptsyntax as rs
          patterns = rs.HatchPatternNames()
          for pattern in patterns:
              description = rs.HatchPatternDescription(pattern)
              if description: print pattern, "-", description
              else: print pattern
        See Also:
          HatchPatternCount
          HatchPatternNames
        """
        __initHatchPatterns()
        pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
        if pattern_instance is None: return scriptcontext.errorhandler()
        return pattern_instance.Description
    *)


    ///<summary>Returns the fill type of a hatch pattern.</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(int) hatch pattern's fill type
    ///  0 = solid, uses object color
    ///  1 = lines, uses pattern file definition
    ///  2 = gradient, uses fill color definition</returns>
    static member HatchPatternFillType(hatchPattern:string) : int =
        __initHatchPatterns()
        let pattern_instance = Doc.HatchPatterns.FindName(hatchPattern)
        if pattern_instance = null then failwithf "Rhino.Scripting Error:HatchPatternFillType failed.  hatchPattern:"%A"" hatchPattern
        int(pattern_instance.FillType)
    (*
    def HatchPatternFillType(hatch_pattern):
        """Returns the fill type of a hatch pattern.
        Parameters:
          hatch_pattern (str): name of an existing hatch pattern
        Returns:
          number: hatch pattern's fill type if successful
                  0 = solid, uses object color
                  1 = lines, uses pattern file definition
                  2 = gradient, uses fill color definition
          None: if unsuccessful
        Example:
          import rhinoscriptsyntax as rs
          patterns = rs.HatchPatternNames()
          for pattern in patterns:
              fill = rs.HatchPatternFillType(pattern)
              print pattern, "-", fill
        See Also:
          HatchPatternCount
          HatchPatternNames
        """
        __initHatchPatterns()
        pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
        if pattern_instance is None: return scriptcontext.errorhandler()
        return int(pattern_instance.FillType)
    *)


    ///<summary>Returns the names of all of the hatch patterns in the document</summary>
    ///<returns>(string seq) the names of all of the hatch patterns in the document</returns>
    static member HatchPatternNames() : string seq =
        __initHatchPatterns()
        let rc = ResizeArray()
        for i=0 to Doc.HatchPatterns.Count) do
            let hatchpattern = Doc.HatchPatterns.[i]
            if hatchpattern.IsDeleted then continue
            rc.Add(hatchpattern.Name)
        rc
    (*
    def HatchPatternNames():
        """Returns the names of all of the hatch patterns in the document
        Returns:
          list(str, ...): the names of all of the hatch patterns in the document
        Example:
          import rhinoscriptsyntax as rs
          patterns = rs.HatchPatternNames()
          for pattern in patterns:
              description = rs.HatchPatternDescription(pattern)
              if description: print pattern, "-", description
              else: print pattern
        See Also:
          HatchPatternCount
        """
        __initHatchPatterns()
        rc = []
        for i in range(scriptcontext.doc.HatchPatterns.Count):
            hatchpattern = scriptcontext.doc.HatchPatterns[i]
            if hatchpattern.IsDeleted: continue
            rc.append(hatchpattern.Name)
        return rc
    *)


    ///<summary>Returns the rotation applied to the hatch pattern when
    /// it is mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(float) if rotation is not defined, the current rotation angle</returns>
    static member HatchRotation(hatchId:Guid) : float =
        let hatchobj = Coerce.coercerhinoobject(hatchId, true, true)
        if not <| isinstance(hatchobj, Rhino.DocObjects.HatchObject) then
            failwithf "Rhino.Scripting Error:HatchRotation failed.  hatchId:"%A" rotation:"%A"" hatchId rotation
        let rc = hatchobj.HatchGeometry.PatternRotation
        rc <- Rhino.RhinoMath.ToDegrees(rc)
        if rotation <> null && rotation<>rc then
            let rotation = Rhino.RhinoMath.ToRadians(rotation)
            let hatchobj.HatchGeometry.PatternRotation = rotation
            hatchobj.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def HatchRotation(hatch_id, rotation=None):
        """Returns or modifies the rotation applied to the hatch pattern when
        it is mapped to the hatch's plane
        Parameters:
          hatch_id (guid): identifier of a hatch object
          rotation (number, optional): rotation angle in degrees
        Returns:
          number: if rotation is not defined, the current rotation angle
          number: if rotation is specified, the previous rotation angle
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          objects = rs.AllObjects()
          if objects:
              for obj in objects:
                  if rs.IsHatch(obj) and rs.HatchRotation(obj)>0:
                      rs.HatchRotation(obj,0)
        See Also:
          AddHatch
          AddHatches
          HatchPattern
          HatchScale
          IsHatch
        """
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
    ///<returns>(unit) unit</returns>
    static member HatchRotation(hatchId:Guid, rotation:float) : unit =
        let hatchobj = Coerce.coercerhinoobject(hatchId, true, true)
        if not <| isinstance(hatchobj, Rhino.DocObjects.HatchObject) then
            failwithf "Rhino.Scripting Error:HatchRotation failed.  hatchId:"%A" rotation:"%A"" hatchId rotation
        let rc = hatchobj.HatchGeometry.PatternRotation
        rc <- Rhino.RhinoMath.ToDegrees(rc)
        if rotation <> null && rotation<>rc then
            let rotation = Rhino.RhinoMath.ToRadians(rotation)
            let hatchobj.HatchGeometry.PatternRotation = rotation
            hatchobj.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def HatchRotation(hatch_id, rotation=None):
        """Returns or modifies the rotation applied to the hatch pattern when
        it is mapped to the hatch's plane
        Parameters:
          hatch_id (guid): identifier of a hatch object
          rotation (number, optional): rotation angle in degrees
        Returns:
          number: if rotation is not defined, the current rotation angle
          number: if rotation is specified, the previous rotation angle
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          objects = rs.AllObjects()
          if objects:
              for obj in objects:
                  if rs.IsHatch(obj) and rs.HatchRotation(obj)>0:
                      rs.HatchRotation(obj,0)
        See Also:
          AddHatch
          AddHatches
          HatchPattern
          HatchScale
          IsHatch
        """
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


    ///<summary>Returns the scale applied to the hatch pattern when it is
    /// mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(float) if scale is not defined, the current scale factor</returns>
    static member HatchScale(hatchId:Guid) : float =
        let hatchobj = Coerce.coercerhinoobject(hatchId)
        if not <| isinstance(hatchobj, Rhino.DocObjects.HatchObject) then
            failwithf "Rhino.Scripting Error:HatchScale failed.  hatchId:"%A" scale:"%A"" hatchId scale
        let rc = hatchobj.HatchGeometry.PatternScale
        if scale && scale<>rc then
            let hatchobj.HatchGeometry.PatternScale = scale
            hatchobj.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def HatchScale(hatch_id, scale=None):
        """Returns or modifies the scale applied to the hatch pattern when it is
        mapped to the hatch's plane
        Parameters:
          hatch_id (guid): identifier of a hatch object
          scale (number, optional):  scale factor
        Returns:
          number: if scale is not defined, the current scale factor
          number: if scale is defined, the previous scale factor
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          objects = rs.NormalObjects()
          if objects:
              for obj in objects:
                  if rs.IsHatch(obj) and rs.HatchScale(obj)>1.0:
                      rs.HatchScale(obj, 1.0)
        See Also:
          HatchPattern
          HatchRotation
          IsHatch
        """
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
    ///<returns>(unit) unit</returns>
    static member HatchScale(hatchId:Guid, scale:float) : unit =
        let hatchobj = Coerce.coercerhinoobject(hatchId)
        if not <| isinstance(hatchobj, Rhino.DocObjects.HatchObject) then
            failwithf "Rhino.Scripting Error:HatchScale failed.  hatchId:"%A" scale:"%A"" hatchId scale
        let rc = hatchobj.HatchGeometry.PatternScale
        if scale && scale<>rc then
            let hatchobj.HatchGeometry.PatternScale = scale
            hatchobj.CommitChanges()
            Doc.Views.Redraw()
        rc
    (*
    def HatchScale(hatch_id, scale=None):
        """Returns or modifies the scale applied to the hatch pattern when it is
        mapped to the hatch's plane
        Parameters:
          hatch_id (guid): identifier of a hatch object
          scale (number, optional):  scale factor
        Returns:
          number: if scale is not defined, the current scale factor
          number: if scale is defined, the previous scale factor
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          objects = rs.NormalObjects()
          if objects:
              for obj in objects:
                  if rs.IsHatch(obj) and rs.HatchScale(obj)>1.0:
                      rs.HatchScale(obj, 1.0)
        See Also:
          HatchPattern
          HatchRotation
          IsHatch
        """
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


    ///<summary>Verifies the existence of a hatch object in the document</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatch(objectId:Guid) : bool =
        let rhobj = Coerce.coercerhinoobject(objectId, true, false)
        isinstance(rhobj, Rhino.DocObjects.HatchObject)
    (*
    def IsHatch(object_id):
        """Verifies the existence of a hatch object in the document
        Parameters:
          object_id (guid): identifier of an object
        Returns:
          bool: True or False
        Example:
          import rhinoscriptsyntax as rs
          obj = rs.GetObject("Select object")
          if rs.IsHatch(obj): print "Object is a hatch"
          else: print "Object is not a hatch"
        See Also:
          HatchPattern
          HatchRotation
          HatchScale
        """
        rhobj = rhutil.coercerhinoobject(object_id, True, False)
        return isinstance(rhobj, Rhino.DocObjects.HatchObject)
    *)


    ///<summary>Verifies the existence of a hatch pattern in the document</summary>
    ///<param name="name">(string) The name of a hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPattern(name:string) : bool =
        __initHatchPatterns()
        Doc.HatchPatterns.FindName(name) <> null
    (*
    def IsHatchPattern(name):
        """Verifies the existence of a hatch pattern in the document
        Parameters:
          name (str): the name of a hatch pattern
        Returns:
          bool: True or False
        Example:
          import rhinoscriptsyntax as rs
          hatch = rs.GetString("Hatch pattern name")
          if rs.IsHatchPattern(hatch): print "The hatch pattern exists."
          else: print "The hatch pattern does not exist."
        See Also:
          IsHatchPatternCurrent
          IsHatchPatternReference
        """
        __initHatchPatterns()
        return scriptcontext.doc.HatchPatterns.FindName(name) is not None
    *)


    ///<summary>Verifies that a hatch pattern is the current hatch pattern</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternCurrent(hatchPattern:string) : bool =
        __initHatchPatterns()
        let pattern_instance = Doc.HatchPatterns.FindName(hatchPattern)
        if pattern_instance = null then failwithf "Rhino.Scripting Error:IsHatchPatternCurrent failed.  hatchPattern:"%A"" hatchPattern
        pattern_instance.Index=Doc.HatchPatterns.CurrentHatchPatternIndex
    (*
    def IsHatchPatternCurrent(hatch_pattern):
        """Verifies that a hatch pattern is the current hatch pattern
        Parameters:
          hatch_pattern (str): name of an existing hatch pattern
        Returns:
          bool: True or False
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          hatch = rs.GetString("Hatch pattern name")
          if rs.IsHatchPattern(hatch):
              if rs.IsHatchPatternCurrent(hatch):
                  print "The hatch pattern is current."
              else:
                  print "The hatch pattern is not current."
          else: print "The hatch pattern does not exist."
        See Also:
          IsHatchPattern
          IsHatchPatternReference
        """
        __initHatchPatterns()
        pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
        if pattern_instance is None: return scriptcontext.errorhandler()
        return pattern_instance.Index==scriptcontext.doc.HatchPatterns.CurrentHatchPatternIndex
    *)


    ///<summary>Verifies that a hatch pattern is from a reference file</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternReference(hatchPattern:string) : bool =
        __initHatchPatterns()
        let pattern_instance = Doc.HatchPatterns.FindName(hatchPattern)
        if pattern_instance = null then failwithf "Rhino.Scripting Error:IsHatchPatternReference failed.  hatchPattern:"%A"" hatchPattern
        pattern_instance.IsReference
    (*
    def IsHatchPatternReference(hatch_pattern):
        """Verifies that a hatch pattern is from a reference file
        Parameters:
          hatch_pattern (str): name of an existing hatch pattern
        Returns:
          bool: True or False
          None: on error
        Example:
          import rhinoscriptsyntax as rs
          hatch = rs.GetString("Hatch pattern name")
          if rs.IsHatchPattern(hatch):
              if rs.IsHatchPatternReference(hatch):
                  print "The hatch pattern is reference."
              else:
                  print "The hatch pattern is not reference."
          else:
              print "The hatch pattern does not exist."
        See Also:
          IsHatchPattern
          IsHatchPatternCurrent
        """
        __initHatchPatterns()
        pattern_instance = scriptcontext.doc.HatchPatterns.FindName(hatch_pattern)
        if pattern_instance is None: return scriptcontext.errorhandler()
        return pattern_instance.IsReference
    *)


