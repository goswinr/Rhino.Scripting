namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsHatch =
 type RhinoScriptSyntax with
    
    [<EXT>]    
    static member InitHatchPatterns() : unit = //TODO make private
        if isNull <| Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Solid.Name) then
            Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Solid) |> ignore

        if isNull <| Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Hatch1.Name) then
            Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Hatch1)|> ignore

        if isNull <| Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Hatch2.Name) then
            Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Hatch2)|> ignore

        if isNull <| Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Hatch3.Name) then
            Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Hatch3)|> ignore

        if isNull <| Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Dash.Name) then
            Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Dash)|> ignore

        if isNull <| Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Grid.Name) then
            Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Grid)|> ignore

        if isNull <| Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Grid60.Name) then
            Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Grid60)|> ignore

        if isNull <| Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Plus.Name) then
            Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Plus)|> ignore

        if isNull <| Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Squares.Name) then
            Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Squares)|> ignore




    [<EXT>]
    ///<summary>Creates one or more new hatch objects a list of closed planar curves</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of the closed planar curves that defines the
    ///  boundary of the hatch objects</param>
    ///<param name="hatchPattern">(string) Optional, Name of the hatch pattern to be used by the hatch object. 
    ///  If omitted, the current hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>
    ///Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>
    ///Hatch pattern rotation angle in degrees.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///Tolerance for hatch fills.</param>
    ///<returns>(Guid ResizeArray) identifiers of the newly created hatch on success</returns>
    static member AddHatches( curveIds:Guid seq, 
                              [<OPT;DEF(null:string)>]hatchPattern:string, 
                              [<OPT;DEF(1.0)>]scale:float, 
                              [<OPT;DEF(0.0)>]rotation:float, 
                              [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        RhinoScriptSyntax.InitHatchPatterns()
        //id = RhinoScriptSyntax.Coerceguid(curveIds)        
        let mutable index = Doc.HatchPatterns.CurrentHatchPatternIndex
        if notNull hatchPattern then
            let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
            index <-  if patterninstance|> isNull then RhinoMath.UnsetIntIndex else patterninstance.Index
            if index<0 then failwithf "Rhino.Scripting: AddHatches failed.  curveIds:'%A' hatchPattern:'%A' scale:'%A' rotation:'%A' tolerance:'%A'" curveIds hatchPattern scale rotation tolerance
        let curves =  resizeArray { for objectId in curveIds do yield RhinoScriptSyntax.CoerceCurve(objectId) } 
        let rotation = Rhino.RhinoMath.ToRadians(rotation)
        
        let tolerance = if tolerance <= 0.0 then Doc.ModelAbsoluteTolerance else tolerance
        let hatches = Hatch.Create(curves, index, rotation, scale, tolerance)
        if isNull hatches then failwithf "Rhino.Scripting: AddHatches failed.  curveIds:'%A' hatchPattern:'%A' scale:'%A' rotation:'%A' tolerance:'%A'" curveIds hatchPattern scale rotation tolerance
        let ids = ResizeArray()
        for hatch in hatches do
            let objectId = Doc.Objects.AddHatch(hatch)
            if objectId <> Guid.Empty then 
                ids.Add(objectId)
        if ids.Count=0 then failwithf "Rhino.Scripting: AddHatches failed.  curveIds:'%A' hatchPattern:'%A' scale:'%A' rotation:'%A' tolerance:'%A'" curveIds hatchPattern scale rotation tolerance
        Doc.Views.Redraw()
        ids
    
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
    static member AddHatch( curveId:Guid, 
                            [<OPT;DEF(null:string)>]hatchPattern:string, 
                            [<OPT;DEF(1.0)>]scale:float, 
                            [<OPT;DEF(0.0)>]rotation:float) : Guid =
        let rc = RhinoScriptSyntax.AddHatches([curveId], hatchPattern, scale, rotation)
        if rc.Count=1 then rc.[0]
        else failwithf "Rhino.Scripting: AddHatch failed.  curveId:'%A' hatchPattern:'%A' scale:'%A' rotation:'%A'" curveId hatchPattern scale rotation

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
    ///<returns>(string ResizeArray) Names of the newly added hatch patterns</returns>
    static member AddHatchPatterns(filename:string, [<OPT;DEF(false)>]replace:bool) : string ResizeArray =
        let patterns = Rhino.DocObjects.HatchPattern.ReadFromFile(filename, true)
        if isNull patterns then failwithf "Rhino.Scripting: AddHatchPatterns failed.  filename:'%A' replace:'%A'" filename replace
        let rc = ResizeArray()
        for pattern in patterns do
             let index = Doc.HatchPatterns.Add(pattern)
             if index>=0 then
                 let pattern = Doc.HatchPatterns.[index]
                 rc.Add(pattern.Name)
        if  rc.Count=0 then failwithf "Rhino.Scripting: AddHatchPatterns failed.  filename:'%A' replace:'%A'" filename replace
        rc
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
        let i = Doc.HatchPatterns.CurrentHatchPatternIndex
        let hp = Doc.HatchPatterns.[i]
        hp.Name
        
        
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
        let rc = Doc.HatchPatterns.CurrentHatchPatternIndex        
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then failwithf "Rhino.Scripting: Set CurrentHatchPattern failed. hatchPattern:'%A'" hatchPattern
        Doc.HatchPatterns.CurrentHatchPatternIndex <- patterninstance.Index
        
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

