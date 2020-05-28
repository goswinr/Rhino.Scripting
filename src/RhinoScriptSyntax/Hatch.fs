namespace Rhino.Scripting.Modules

open FsEx
open System
open Rhino
open Rhino.Geometry

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore
 

[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting Namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsHatch =

  //[<Extension>] //Error 3246
 type RhinoScriptSyntax with

    [<Extension>]
    static member private InitHatchPatterns() : unit =
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




    [<Extension>]
    ///<summary>Creates one or more new hatch objects a list of closed planar curves</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of the closed planar curves that defines the
    ///    boundary of the hatch objects</param>
    ///<param name="hatchPattern">(string) Optional, Name of the hatch pattern to be used by the hatch object.
    ///    If omitted, the current hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>
    ///    Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>
    ///    Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Tolerance for hatch fills</param>
    ///<returns>(Guid ResizeArray) identifiers of the newly created hatch</returns>
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
        let rotation = RhinoMath.ToRadians(rotation)

        let tolerance = if tolerance <= 0.0 then Doc.ModelAbsoluteTolerance else tolerance
        let hatches = Hatch.Create(curves, index, rotation, scale, tolerance)
        if isNull hatches then failwithf "Rhino.Scripting: AddHatches failed.  curveIds:'%A' hatchPattern:'%A' scale:'%A' rotation:'%A' tolerance:'%A'" curveIds hatchPattern scale rotation tolerance
        let ids = ResizeArray()
        for hatch in hatches do
            let objectId = Doc.Objects.AddHatch(hatch)
            if objectId <> Guid.Empty then
                ids.Add(objectId)
        if ids.Count = 0 then failwithf "Rhino.Scripting: AddHatches failed.  curveIds:'%A' hatchPattern:'%A' scale:'%A' rotation:'%A' tolerance:'%A'" curveIds hatchPattern scale rotation tolerance
        Doc.Views.Redraw()
        ids

    [<Extension>]
    ///<summary>Creates a new hatch object from a closed planar curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the closed planar curve that defines the
    ///    boundary of the hatch object</param>
    ///<param name="hatchPattern">(string) Optional, Name of the hatch pattern to be used by the hatch
    ///    object. If omitted, the current hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>
    ///    Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>
    ///    Hatch pattern rotation angle in degrees</param>
    ///<returns>(Guid) identifier of the newly created hatch</returns>
    static member AddHatch( curveId:Guid,
                            [<OPT;DEF(null:string)>]hatchPattern:string,
                            [<OPT;DEF(1.0)>]scale:float,
                            [<OPT;DEF(0.0)>]rotation:float) : Guid =
        let rc = RhinoScriptSyntax.AddHatches([curveId], hatchPattern, scale, rotation) //TODO Test ok with null
        if rc.Count = 1 then rc.[0]
        else failwithf "Rhino.Scripting: AddHatch failed.  curveId:'%A' hatchPattern:'%A' scale:'%A' rotation:'%A'" curveId hatchPattern scale rotation



    [<Extension>]
    ///<summary>Adds hatch patterns to the document by importing hatch pattern definitions
    ///    from a pattern file</summary>
    ///<param name="filename">(string) Name of the hatch pattern file</param>
    ///<param name="replace">(bool) Optional, Default Value: <c>false</c>
    ///    If hatch pattern names already in the document match hatch
    ///    pattern names in the pattern definition file, then the existing hatch
    ///    patterns will be redefined</param>
    ///<returns>(string ResizeArray) Names of the newly added hatch patterns</returns>
    static member AddHatchPatterns(filename:string, [<OPT;DEF(false)>]replace:bool) : string ResizeArray =
        let patterns = DocObjects.HatchPattern.ReadFromFile(filename, true)
        if isNull patterns then failwithf "Rhino.Scripting: AddHatchPatterns failed.  filename:'%A' replace:'%A'" filename replace
        let rc = ResizeArray()
        for pattern in patterns do
             let index = Doc.HatchPatterns.Add(pattern)
             if index>=0 then
                 let pattern = Doc.HatchPatterns.[index]
                 rc.Add(pattern.Name)
        if  rc.Count = 0 then failwithf "Rhino.Scripting: AddHatchPatterns failed.  filename:'%A' replace:'%A'" filename replace
        rc


    [<Extension>]
    ///<summary>Returns the current hatch pattern file</summary>
    ///<returns>(string) The current hatch pattern</returns>
    static member CurrentHatchPattern() : string = //GET
        let i = Doc.HatchPatterns.CurrentHatchPatternIndex
        let hp = Doc.HatchPatterns.[i]
        hp.Name



    [<Extension>]
    ///<summary>Sets the current hatch pattern file</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern to make current</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentHatchPattern(hatchPattern:string) : unit = //SET
        let rc = Doc.HatchPatterns.CurrentHatchPatternIndex
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then failwithf "Rhino.Scripting: Set CurrentHatchPattern failed. hatchPattern:'%A'" hatchPattern
        Doc.HatchPatterns.CurrentHatchPatternIndex <- patterninstance.Index


    [<Extension>]
    ///<summary>Explodes a hatch object into its component objects. The exploded objects
    ///    will be added to the document. If the hatch object uses a solid pattern,
    ///    then planar face Brep objects will be created. Otherwise, line curve objects
    ///    will be created</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the hatch object</param>
    ///<returns>(Guid ResizeArray) list of identifiers for the newly created objects</returns>
    static member ExplodeHatch(hatchId:Guid, [<OPT;DEF(false)>]delete:bool) : Guid ResizeArray =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(hatchId)
        let geo =  RhinoScriptSyntax.CoerceHatch(hatchId)
        let pieces = geo.Explode()
        if isNull pieces then failwithf "Rhino.Scripting: ExplodeHatch failed.  hatchId:'%A' delete:'%A'" hatchId delete
        let attr = rhobj.Attributes
        let rc = ResizeArray()
        for piece in pieces do
            match piece with
            | :? Curve as c->
                let g = Doc.Objects.AddCurve(c, attr)
                if g<>Guid.Empty then rc.Add(g)
            | :? Brep as c->
                let g = Doc.Objects.AddBrep(c, attr)
                if g<>Guid.Empty then rc.Add(g)
            | _ -> failwithf "ExplodeHatch: darwing of %A objects after exploding not implemented" piece.ObjectType //TODO test with hatch patterns that have points
        if delete then Doc.Objects.Delete(rhobj)|> ignore
        rc


    [<Extension>]
    ///<summary>Returns a hatch object's hatch pattern</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(string) The current hatch pattern</returns>
    static member HatchPattern(hatchId:Guid) : string = //GET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let oldindex = hatchobj.HatchGeometry.PatternIndex
        Doc.HatchPatterns.[oldindex].Name

    [<Extension>]
    ///<summary>Changes a hatch object's hatch pattern</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern to replace the
    ///    current hatch pattern</param>
    ///<returns>(unit) void, nothing</returns>
    static member HatchPattern(hatchId:Guid, hatchPattern:string) : unit = //SET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let oldindex = hatchobj.HatchGeometry.PatternIndex
        RhinoScriptSyntax.InitHatchPatterns()
        let newpatt = Doc.HatchPatterns.FindName(hatchPattern)
        if newpatt|> isNull  then failwithf "Rhino.Scripting: HatchPattern failed.  hatchId:'%A' hatchPattern:'%A'" hatchId hatchPattern
        hatchobj.HatchGeometry.PatternIndex <- newpatt.Index
        if not<| hatchobj.CommitChanges() then failwithf "Rhino.Scripting: HatchPattern failed.  hatchId:'%A' hatchPattern:'%A'" hatchId hatchPattern
        Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Changes multiple hatch objects's hatch pattern</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple hatch objects</param>
    ///<param name="hatchPattern">(string) Name of multiple existing hatch pattern to replace the
    ///    current hatch pattern</param>
    ///<returns>(unit) void, nothing</returns>
    static member HatchPattern(hatchIds:Guid seq, hatchPattern:string) : unit = //MULTISET
        RhinoScriptSyntax.InitHatchPatterns()
        for hatchId in hatchIds do 
            let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
            let oldindex = hatchobj.HatchGeometry.PatternIndex            
            let newpatt = Doc.HatchPatterns.FindName(hatchPattern)
            if newpatt|> isNull  then failwithf "Rhino.Scripting: HatchPattern failed.  hatchId:'%A' hatchPattern:'%A'" hatchId hatchPattern
            hatchobj.HatchGeometry.PatternIndex <- newpatt.Index
            if not<| hatchobj.CommitChanges() then failwithf "Rhino.Scripting: HatchPattern failed.  hatchId:'%A' hatchPattern:'%A'" hatchId hatchPattern
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the number of hatch patterns in the document</summary>
    ///<returns>(int) the number of hatch patterns in the document</returns>
    static member HatchPatternCount() : int =
        RhinoScriptSyntax.InitHatchPatterns()
        Doc.HatchPatterns.Count


    [<Extension>]
    ///<summary>Returns the description of a hatch pattern. Note, not all hatch patterns
    ///    have descriptions</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(string) description of the hatch pattern</returns>
    static member HatchPatternDescription(hatchPattern:string) : string =
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then failwithf "Rhino.Scripting: HatchPatternDescription failed.  hatchPattern:'%A'" hatchPattern
        patterninstance.Description


    [<Extension>]
    ///<summary>Returns the fill type of a hatch pattern</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(int) hatch pattern's fill type
    ///    0 = solid, uses object color
    ///    1 = lines, uses pattern file definition
    ///    2 = gradient, uses fill color definition</returns>
    static member HatchPatternFillType(hatchPattern:string) : int =
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then failwithf "Rhino.Scripting: HatchPatternFillType failed.  hatchPattern:'%A'" hatchPattern
        int(patterninstance.FillType)


    [<Extension>]
    ///<summary>Returns the names of all of the hatch patterns in the document</summary>
    ///<returns>(string ResizeArray) the names of all of the hatch patterns in the document</returns>
    static member HatchPatternNames() : string ResizeArray =
        RhinoScriptSyntax.InitHatchPatterns()
        let rc = ResizeArray()
        for i in range(Doc.HatchPatterns.Count) do
            let hatchpattern = Doc.HatchPatterns.[i]
            if not hatchpattern.IsDeleted then
                rc.Add(hatchpattern.Name)
        rc


    [<Extension>]
    ///<summary>Returns the rotation applied to the hatch pattern when
    /// it is mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(float) if rotation is not defined, the current rotation angle</returns>
    static member HatchRotation(hatchId:Guid) : float = //GET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let rc = hatchobj.HatchGeometry.PatternRotation
        RhinoMath.ToDegrees(rc)


    [<Extension>]
    ///<summary>Modifies the rotation applied to the hatch pattern when
    /// it is mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="rotation">(float) Rotation angle in degrees</param>
    ///<returns>(unit) void, nothing</returns>
    static member HatchRotation(hatchId:Guid, rotation:float) : unit = //SET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let mutable rc = hatchobj.HatchGeometry.PatternRotation
        rc <- RhinoMath.ToDegrees(rc)
        if rotation <> rc then
            let rotation = RhinoMath.ToRadians(rotation)
            hatchobj.HatchGeometry.PatternRotation <- rotation
            if not <| hatchobj.CommitChanges() then failwithf "HatchRotation failed on rotation %f on %A" rotation hatchId
            Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Modifies the rotation applied to the hatch pattern when
    /// it is mapped to the hatch's plane</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple hatch objects</param>
    ///<param name="rotation">(float) Rotation angle in degrees</param>
    ///<returns>(unit) void, nothing</returns>
    static member HatchRotation(hatchIds:Guid seq, rotation:float) : unit = //MULTISET
        for hatchId in hatchIds do  
            let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
            let mutable rc = hatchobj.HatchGeometry.PatternRotation
            rc <- RhinoMath.ToDegrees(rc)
            if rotation <> rc then
                let rotation = RhinoMath.ToRadians(rotation)
                hatchobj.HatchGeometry.PatternRotation <- rotation
                if not <| hatchobj.CommitChanges() then failwithf "HatchRotation failed on rotation %f on %A" rotation hatchId
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the scale applied to the hatch pattern when it is
    /// mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(float) if scale is not defined, the current scale factor</returns>
    static member HatchScale(hatchId:Guid) : float = //GET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        hatchobj.HatchGeometry.PatternScale


    [<Extension>]
    ///<summary>Modifies the scale applied to the hatch pattern when it is
    /// mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="scale">(float) Scale factor</param>
    ///<returns>(unit) void, nothing</returns>
    static member HatchScale(hatchId:Guid, scale:float) : unit = //SET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let rc = hatchobj.HatchGeometry.PatternScale
        if scale <> rc then
            hatchobj.HatchGeometry.PatternScale <- scale
            if not <| hatchobj.CommitChanges() then failwithf "HatchScale failed on scale %f on %A" scale hatchId
            Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Modifies the scale applied to the hatch pattern when it is
    /// mapped to the hatch's plane</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple hatch objects</param>
    ///<param name="scale">(float) Scale factor</param>
    ///<returns>(unit) void, nothing</returns>
    static member HatchScale(hatchIds:Guid seq, scale:float) : unit = //MULTISET
        for hatchId in hatchIds do  
            let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
            let rc = hatchobj.HatchGeometry.PatternScale
            if scale <> rc then
                hatchobj.HatchGeometry.PatternScale <- scale
                if not <| hatchobj.CommitChanges() then failwithf "HatchScale failed on scale %f on %A" scale hatchId
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Verifies the existence of a hatch object in the document</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatch(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        match rhobj with :? DocObjects.HatchObject  -> true |_ -> false


    [<Extension>]
    ///<summary>Verifies the existence of a hatch pattern in the document</summary>
    ///<param name="name">(string) The name of a hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPattern(name:string) : bool =
        RhinoScriptSyntax.InitHatchPatterns()
        Doc.HatchPatterns.FindName(name) |> notNull


    [<Extension>]
    ///<summary>Verifies that a hatch pattern is the current hatch pattern</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternCurrent(hatchPattern:string) : bool =
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then failwithf "Rhino.Scripting: IsHatchPatternCurrent failed.  hatchPattern:'%A'" hatchPattern
        patterninstance.Index = Doc.HatchPatterns.CurrentHatchPatternIndex


    [<Extension>]
    ///<summary>Verifies that a hatch pattern is from a reference file</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternReference(hatchPattern:string) : bool =
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then failwithf "Rhino.Scripting: IsHatchPatternReference failed.  hatchPattern:'%A'" hatchPattern
        patterninstance.IsReference


