
namespace Rhino.Scripting

open Rhino
open System
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils


[<AutoOpen>]
module AutoOpenHatch =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    static member private InitHatchPatterns() : unit = // TODO, optimize so that this init is ony done once ?
        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Solid.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Solid) |> ignore<int>

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Hatch1.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Hatch1)|> ignore<int>

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Hatch2.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Hatch2)|> ignore<int>

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Hatch3.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Hatch3)|> ignore<int>

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Dash.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Dash)|> ignore<int>

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Grid.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Grid)|> ignore<int>

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Grid60.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Grid60)|> ignore<int>

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Plus.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Plus)|> ignore<int>

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Squares.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Squares)|> ignore<int>



    ///<summary>Creates one or more new Hatch objects from a list of closed planar Curves.</summary>
    ///<param name="curves">(Curve seq) Geometry of the closed planar Curves that defines the boundary of the Hatch objects</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch object.  If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, default value: <c>1.0</c>  Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, default value: <c>0.0</c>  Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>  Tolerance for Hatch fills</param>
    ///<returns>(Guid ResizeArray) identifiers of the newly created Hatch.</returns>
    static member AddHatches( curves:Curve seq,
                              [<OPT;DEF(null:string)>]hatchPattern:string,
                              [<OPT;DEF(1.0)>]scale:float,
                              [<OPT;DEF(0.0)>]rotation:float,
                              [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        RhinoScriptSyntax.InitHatchPatterns()
        let mutable index = State.Doc.HatchPatterns.CurrentHatchPatternIndex
        if notNull hatchPattern then
            let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
            index <-  if patternInstance|> isNull then RhinoMath.UnsetIntIndex else patternInstance.Index
            if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatches failed to find hatchPattern:'%s'"  hatchPattern
        let rotation = RhinoMath.ToRadians(rotation)

        let tolerance = if tolerance <= 0.0 then State.Doc.ModelAbsoluteTolerance else tolerance
        let hatches = Hatch.Create(curves, index, rotation, scale, tolerance)
        if isNull hatches then
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatches failed to create hatch from %d curves, not closed: %d, not planar %d, tolerance:'%g' "
                (Seq.length curves)
                (curves |> Seq.countIf ( fun c -> c.IsClosed   |> not ))
                (curves |> Seq.countIf ( fun c -> c.IsPlanar() |> not ))
                tolerance
        let ids = ResizeArray()
        for hatch in hatches do
            let objectId = State.Doc.Objects.AddHatch(hatch)
            if objectId <> Guid.Empty then
                ids.Add(objectId)
        if ids.Count = 0 then
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatches failed to add any hatches from %d curves, not closed: %d, not planar %d, tolerance:'%g' "
                (Seq.length curves)
                (curves |> Seq.countIf ( fun c -> c.IsClosed   |> not ))
                (curves |> Seq.countIf ( fun c -> c.IsPlanar() |> not ))
                tolerance
        State.Doc.Views.Redraw()
        ids

    ///<summary>Creates one  Hatch objects from one closed planar Curve.</summary>
    ///<param name="curve">(Curve) Curve Geometry of the closed planar Curve that defines the boundary of the Hatch object</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch object. If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, default value: <c>1.0</c>   Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, default value: <c>0.0</c> Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c> Tolerance for Hatch fills</param>
    ///<returns>(Guid) identifier of the newly created Hatch.</returns>
    static member AddHatch(   curve:Curve,
                              [<OPT;DEF(null:string)>]hatchPattern:string,
                              [<OPT;DEF(1.0)>]scale:float,
                              [<OPT;DEF(0.0)>]rotation:float,
                              [<OPT;DEF(0.0)>]tolerance:float) : Guid  =
        try
           let rc = RhinoScriptSyntax.AddHatches([curve], hatchPattern, scale, rotation,tolerance)
           if rc.Count = 1 then rc.[0]
           else RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatch failed to create exactly on hatch from curve. It created %d Hatches"  rc.Count
        with e->
            let tolerance = if tolerance <= 0.0 then State.Doc.ModelAbsoluteTolerance else tolerance
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatch failed on one curve using tolerance %f %sMessage: %s" tolerance  Environment.NewLine e.Message


    ///<summary>Creates one or more new Hatch objects from a list of closed planar Curves.</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of the closed planar Curves that defines the   boundary of the Hatch objects</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch object. If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, default value: <c>1.0</c>   Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, default value: <c>0.0</c> Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c> Tolerance for Hatch fills</param>
    ///<returns>(Guid ResizeArray) identifiers of the newly created Hatch.</returns>
    static member AddHatches(  curveIds:Guid seq,
                              [<OPT;DEF(null:string)>]hatchPattern:string,
                              [<OPT;DEF(1.0)>]scale:float,
                              [<OPT;DEF(0.0)>]rotation:float,
                              [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let curves  = curveIds |> RArr.mapSeq RhinoScriptSyntax.CoerceCurve
        try RhinoScriptSyntax.AddHatches(curves, hatchPattern, scale, rotation)
        with e->
            let tolerance = if tolerance <= 0.0 then State.Doc.ModelAbsoluteTolerance else tolerance
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatches failed on curveIds using tolerance %f :'%s' %sMessage: %s" tolerance (Pretty.str curveIds) Environment.NewLine  e.Message

    ///<summary>Creates a new Hatch object from a closed planar Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the closed planar Curve that defines the boundary of the Hatch object</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch object. If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, default value: <c>1.0</c>  Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, default value: <c>0.0</c> Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c> Tolerance for Hatch fills</param>
    ///<returns>(Guid) identifier of the newly created Hatch.</returns>
    static member AddHatch( curveId:Guid,
                            [<OPT;DEF(null:string)>]hatchPattern:string,
                            [<OPT;DEF(1.0)>]scale:float,
                            [<OPT;DEF(0.0)>]rotation:float,
                            [<OPT;DEF(0.0)>]tolerance:float) : Guid =
        try RhinoScriptSyntax.AddHatch(RhinoScriptSyntax.CoerceCurve(curveId), hatchPattern, scale, rotation, tolerance)
        with e->
            let tolerance = if tolerance <= 0.0 then State.Doc.ModelAbsoluteTolerance else tolerance
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatch failed on one curve using tolerance %f : %s%sMessage: %s" tolerance (Pretty.str curveId) Environment.NewLine  e.Message




    ///<summary>Adds Hatch patterns to the document by importing Hatch pattern definitions
    ///    from a pattern file.</summary>
    ///<param name="filename">(string) Name of the Hatch pattern file</param>
    ///<param name="replace">(bool) Optional, default value: <c>false</c>
    ///    If Hatch pattern names already in the document match Hatch
    ///    pattern names in the pattern definition file, then the existing Hatch
    ///    patterns will be redefined</param>
    ///<returns>(string ResizeArray) Names of the newly added Hatch patterns.</returns>
    static member AddHatchPatterns(filename:string, [<OPT;DEF(false)>]replace:bool) : string ResizeArray =
        let patterns = DocObjects.HatchPattern.ReadFromFile(filename, true)
        if isNull patterns then RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatchPatterns failed. filename:'%s' replace:'%A'" filename replace
        let rc = ResizeArray()
        for pattern in patterns do
             let index = State.Doc.HatchPatterns.Add(pattern)
             if index>=0 then
                 let pattern = State.Doc.HatchPatterns.[index]
                 rc.Add(pattern.Name)
        if  rc.Count = 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatchPatterns failed. filename:'%A' replace:'%A'" filename replace
        rc


    ///<summary>Returns the current Hatch pattern file.</summary>
    ///<returns>(string) The current Hatch pattern.</returns>
    static member CurrentHatchPattern() : string = //GET
        let i = State.Doc.HatchPatterns.CurrentHatchPatternIndex
        let hp = State.Doc.HatchPatterns.[i]
        hp.Name



    ///<summary>Sets the current Hatch pattern file.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern to make current</param>
    ///<returns>(unit) void, nothing.</returns>
    static member CurrentHatchPattern(hatchPattern:string) : unit = //SET
        RhinoScriptSyntax.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.CurrentHatchPattern: Setting it failed. hatchPattern:'%A'" hatchPattern
        State.Doc.HatchPatterns.CurrentHatchPatternIndex <- patternInstance.Index


    ///<summary>Explodes a Hatch object into its component objects. The exploded objects
    ///    will be added to the document. If the Hatch object uses a solid pattern,
    ///    then planar face Brep objects will be created. Otherwise, line Curve objects
    ///    will be created.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="delete">(bool) Optional, default value: <c>false</c>
    ///    Delete the Hatch object</param>
    ///<returns>(Guid ResizeArray) list of identifiers for the newly created objects.</returns>
    static member ExplodeHatch(hatchId:Guid, [<OPT;DEF(false)>]delete:bool) : Guid ResizeArray =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(hatchId)
        let geo =  RhinoScriptSyntax.CoerceHatch(hatchId)
        let pieces = geo.Explode()
        if isNull pieces then RhinoScriptingException.Raise "RhinoScriptSyntax.ExplodeHatch failed.  hatchId:'%s' delete:'%A'" (Pretty.str hatchId) delete
        let attr = rhobj.Attributes
        let rc = ResizeArray()
        for piece in pieces do
            match piece with
            | :? Curve as c->
                let g = State.Doc.Objects.AddCurve(c, attr)
                if g<>Guid.Empty then rc.Add(g)
            | :? Brep as c->
                let g = State.Doc.Objects.AddBrep(c, attr)
                if g<>Guid.Empty then rc.Add(g)
            | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.ExplodeHatch: drawing of %A objects after exploding not implemented" piece.ObjectType //TODO test with hatch patterns that have points
        if delete then State.Doc.Objects.Delete(rhobj)|> ignore<bool>
        rc


    ///<summary>Returns a Hatch object's Hatch pattern.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<returns>(string) The current Hatch pattern.</returns>
    static member HatchPattern(hatchId:Guid) : string = //GET
        let hatchObj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let oldIndex = hatchObj.HatchGeometry.PatternIndex
        State.Doc.HatchPatterns.[oldIndex].Name

    ///<summary>Changes a Hatch object's Hatch pattern.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern to replace the
    ///    current Hatch pattern</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchPattern(hatchId:Guid, hatchPattern:string) : unit = //SET
        let hatchObj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        RhinoScriptSyntax.InitHatchPatterns()
        let newPattern = State.Doc.HatchPatterns.FindName(hatchPattern)
        if newPattern|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (Pretty.str hatchId) hatchPattern
        hatchObj.HatchGeometry.PatternIndex <- newPattern.Index
        hatchObj.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()

    ///<summary>Changes multiple Hatch objects's Hatch pattern.</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple Hatch objects</param>
    ///<param name="hatchPattern">(string) Name of multiple existing Hatch pattern to replace the
    ///    current Hatch pattern</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchPattern(hatchIds:Guid seq, hatchPattern:string) : unit = //MULTISET
        RhinoScriptSyntax.InitHatchPatterns()
        for hatchId in hatchIds do
            let hatchObj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
            let newPattern = State.Doc.HatchPatterns.FindName(hatchPattern)
            if newPattern|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (Pretty.str hatchId) hatchPattern
            hatchObj.HatchGeometry.PatternIndex <- newPattern.Index
            hatchObj.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()


    ///<summary>Returns the number of Hatch patterns in the document.</summary>
    ///<returns>(int) The number of Hatch patterns in the document.</returns>
    static member HatchPatternCount() : int =
        RhinoScriptSyntax.InitHatchPatterns()
        State.Doc.HatchPatterns.Count


    ///<summary>Returns the description of a Hatch pattern. Note, not all Hatch patterns
    ///    have descriptions.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(string) description of the Hatch pattern.</returns>
    static member HatchPatternDescription(hatchPattern:string) : string =
        RhinoScriptSyntax.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPatternDescription failed.  hatchPattern:'%A'" hatchPattern
        patternInstance.Description


    ///<summary>Returns the fill type of a Hatch pattern.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(int) Hatch pattern's fill type
    ///    0 = solid, uses object color
    ///    1 = lines, uses pattern file definition
    ///    2 = gradient, uses fill color definition.</returns>
    static member HatchPatternFillType(hatchPattern:string) : int =
        RhinoScriptSyntax.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPatternFillType failed.  hatchPattern:'%A'" hatchPattern
        int(patternInstance.FillType)


    ///<summary>Returns the names of all of the Hatch patterns in the document.</summary>
    ///<returns>(string ResizeArray) The names of all of the Hatch patterns in the document.</returns>
    static member HatchPatternNames() : string ResizeArray =
        RhinoScriptSyntax.InitHatchPatterns()
        let rc = ResizeArray()
        for i = 0 to State.Doc.HatchPatterns.Count - 1 do
            let hatchPattern = State.Doc.HatchPatterns.[i]
            if not hatchPattern.IsDeleted then
                rc.Add(hatchPattern.Name)
        rc


    ///<summary>Returns the rotation applied to the Hatch pattern when
    /// it is mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<returns>(float) if rotation is not defined, the current rotation angle.</returns>
    static member HatchRotation(hatchId:Guid) : float = //GET
        let hatchObj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let rc = hatchObj.HatchGeometry.PatternRotation
        RhinoMath.ToDegrees(rc)


    ///<summary>Modifies the rotation applied to the Hatch pattern when
    /// it is mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="rotation">(float) Rotation angle in degrees</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchRotation(hatchId:Guid, rotation:float) : unit = //SET
        let hatchObj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let mutable rc = hatchObj.HatchGeometry.PatternRotation
        rc <- RhinoMath.ToDegrees(rc)
        if rotation <> rc then
            let rotation = RhinoMath.ToRadians(rotation)
            hatchObj.HatchGeometry.PatternRotation <- rotation
            hatchObj.CommitChanges() |> ignore<bool>
            State.Doc.Views.Redraw()

    ///<summary>Modifies the rotation applied to the Hatch pattern when
    /// it is mapped to the Hatch's Plane.</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple Hatch objects</param>
    ///<param name="rotation">(float) Rotation angle in degrees</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchRotation(hatchIds:Guid seq, rotation:float) : unit = //MULTISET
        for hatchId in hatchIds do
            let hatchObj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
            let mutable rc = hatchObj.HatchGeometry.PatternRotation
            rc <- RhinoMath.ToDegrees(rc)
            if rotation <> rc then
                let rotation = RhinoMath.ToRadians(rotation)
                hatchObj.HatchGeometry.PatternRotation <- rotation
                hatchObj.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()


    ///<summary>Returns the scale applied to the Hatch pattern when it is
    /// mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<returns>(float) if scale is not defined, the current scale factor.</returns>
    static member HatchScale(hatchId:Guid) : float = //GET
        let hatchObj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        hatchObj.HatchGeometry.PatternScale


    ///<summary>Modifies the scale applied to the Hatch pattern when it is
    /// mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="scale">(float) Scale factor</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchScale(hatchId:Guid, scale:float) : unit = //SET
        let hatchObj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let rc = hatchObj.HatchGeometry.PatternScale
        if scale <> rc then
            hatchObj.HatchGeometry.PatternScale <- scale
            hatchObj.CommitChanges() |> ignore<bool>
            State.Doc.Views.Redraw()

    ///<summary>Modifies the scale applied to the Hatch pattern when it is
    /// mapped to the Hatch's Plane.</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple Hatch objects</param>
    ///<param name="scale">(float) Scale factor</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchScale(hatchIds:Guid seq, scale:float) : unit = //MULTISET
        for hatchId in hatchIds do
            let hatchObj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
            let rc = hatchObj.HatchGeometry.PatternScale
            if scale <> rc then
                hatchObj.HatchGeometry.PatternScale <- scale
                hatchObj.CommitChanges() |> ignore<bool>
        State.Doc.Views.Redraw()


    ///<summary>Verifies the existence of a Hatch object in the document.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatch(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        match rhobj with :? DocObjects.HatchObject  -> true |_ -> false


    ///<summary>Verifies the existence of a Hatch pattern in the document.</summary>
    ///<param name="name">(string) The name of a Hatch pattern</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatchPattern(name:string) : bool =
        RhinoScriptSyntax.InitHatchPatterns()
        State.Doc.HatchPatterns.FindName(name) |> notNull


    ///<summary>Checks if a Hatch pattern is the current Hatch pattern.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatchPatternCurrent(hatchPattern:string) : bool =
        RhinoScriptSyntax.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.IsHatchPatternCurrent failed.  hatchPattern:'%A'" hatchPattern
        patternInstance.Index = State.Doc.HatchPatterns.CurrentHatchPatternIndex


    ///<summary>Checks if a Hatch pattern is from a reference file.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatchPatternReference(hatchPattern:string) : bool =
        RhinoScriptSyntax.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.IsHatchPatternReference failed.  hatchPattern:'%A'" hatchPattern
        patternInstance.IsReference



