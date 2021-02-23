namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore
 

[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namspace is opened.
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
    ///<summary>Creates one or more new Hatch objects a list of closed planar Curves.</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of the closed planar Curves that defines the
    ///    boundary of the Hatch objects</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch object.
    ///    If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>
    ///    Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>
    ///    Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Tolerance for Hatch fills</param>
    ///<returns>(Guid Rarr) identifiers of the newly created Hatch.</returns>
    static member AddHatches( curveIds:Guid seq,
                              [<OPT;DEF(null:string)>]hatchPattern:string,
                              [<OPT;DEF(1.0)>]scale:float,
                              [<OPT;DEF(0.0)>]rotation:float,
                              [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr =
        RhinoScriptSyntax.InitHatchPatterns()
        //id = RhinoScriptSyntax.Coerceguid(curveIds)
        let mutable index = Doc.HatchPatterns.CurrentHatchPatternIndex
        if notNull hatchPattern then
            let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
            index <-  if patterninstance|> isNull then RhinoMath.UnsetIntIndex else patterninstance.Index
            if index<0 then RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatches failed.  curveIds:'%s' hatchPattern:'%A' scale:'%A' rotation:'%A' tolerance:'%A'" (RhinoScriptSyntax.ToNiceString curveIds) hatchPattern scale rotation tolerance
        let curves =  rarr { for objectId in curveIds do yield RhinoScriptSyntax.CoerceCurve(objectId) }
        let rotation = RhinoMath.ToRadians(rotation)

        let tolerance = if tolerance <= 0.0 then Doc.ModelAbsoluteTolerance else tolerance
        let hatches = Hatch.Create(curves, index, rotation, scale, tolerance)
        if isNull hatches then RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatches failed.  curveIds:'%s' hatchPattern:'%A' scale:'%A' rotation:'%A' tolerance:'%A'" (RhinoScriptSyntax.ToNiceString curveIds) hatchPattern scale rotation tolerance
        let ids = Rarr()
        for hatch in hatches do
            let objectId = Doc.Objects.AddHatch(hatch)
            if objectId <> Guid.Empty then
                ids.Add(objectId)
        if ids.Count = 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatches failed.  curveIds:'%s' hatchPattern:'%A' scale:'%A' rotation:'%A' tolerance:'%A'" (RhinoScriptSyntax.ToNiceString curveIds) hatchPattern scale rotation tolerance
        Doc.Views.Redraw()
        ids

    [<Extension>]
    ///<summary>Creates a new Hatch object from a closed planar Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the closed planar Curve that defines the
    ///    boundary of the Hatch object</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch
    ///    object. If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>
    ///    Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>
    ///    Hatch pattern rotation angle in degrees</param>
    ///<returns>(Guid) identifier of the newly created Hatch.</returns>
    static member AddHatch( curveId:Guid,
                            [<OPT;DEF(null:string)>]hatchPattern:string,
                            [<OPT;DEF(1.0)>]scale:float,
                            [<OPT;DEF(0.0)>]rotation:float) : Guid =
        let rc = RhinoScriptSyntax.AddHatches([curveId], hatchPattern, scale, rotation) //TODO Test ok with null
        if rc.Count = 1 then rc.[0]
        else RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatch failed. curveId:'%s' hatchPattern:'%A' scale:'%A' rotation:'%A'" (rhType curveId) hatchPattern scale rotation



    [<Extension>]
    ///<summary>Adds Hatch patterns to the document by importing Hatch pattern definitions
    ///    from a pattern file.</summary>
    ///<param name="filename">(string) Name of the Hatch pattern file</param>
    ///<param name="replace">(bool) Optional, Default Value: <c>false</c>
    ///    If Hatch pattern names already in the document match Hatch
    ///    pattern names in the pattern definition file, then the existing Hatch
    ///    patterns will be redefined</param>
    ///<returns>(string Rarr) Names of the newly added Hatch patterns.</returns>
    static member AddHatchPatterns(filename:string, [<OPT;DEF(false)>]replace:bool) : string Rarr =
        let patterns = DocObjects.HatchPattern.ReadFromFile(filename, true)
        if isNull patterns then RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatchPatterns failed.  filename:'%A' replace:'%A'" filename replace
        let rc = Rarr()
        for pattern in patterns do
             let index = Doc.HatchPatterns.Add(pattern)
             if index>=0 then
                 let pattern = Doc.HatchPatterns.[index]
                 rc.Add(pattern.Name)
        if  rc.Count = 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.AddHatchPatterns failed.  filename:'%A' replace:'%A'" filename replace
        rc


    [<Extension>]
    ///<summary>Returns the current Hatch pattern file.</summary>
    ///<returns>(string) The current Hatch pattern.</returns>
    static member CurrentHatchPattern() : string = //GET
        let i = Doc.HatchPatterns.CurrentHatchPatternIndex
        let hp = Doc.HatchPatterns.[i]
        hp.Name



    [<Extension>]
    ///<summary>Sets the current Hatch pattern file.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern to make current</param>
    ///<returns>(unit) void, nothing.</returns>
    static member CurrentHatchPattern(hatchPattern:string) : unit = //SET
        let rc = Doc.HatchPatterns.CurrentHatchPatternIndex
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.Set CurrentHatchPattern failed. hatchPattern:'%A'" hatchPattern
        Doc.HatchPatterns.CurrentHatchPatternIndex <- patterninstance.Index


    [<Extension>]
    ///<summary>Explodes a Hatch object into its component objects. The exploded objects
    ///    will be added to the document. If the Hatch object uses a solid pattern,
    ///    then planar face Brep objects will be created. Otherwise, line Curve objects
    ///    will be created.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the Hatch object</param>
    ///<returns>(Guid Rarr) list of identifiers for the newly created objects.</returns>
    static member ExplodeHatch(hatchId:Guid, [<OPT;DEF(false)>]delete:bool) : Guid Rarr =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(hatchId)
        let geo =  RhinoScriptSyntax.CoerceHatch(hatchId)
        let pieces = geo.Explode()
        if isNull pieces then RhinoScriptingException.Raise "RhinoScriptSyntax.ExplodeHatch failed.  hatchId:'%s' delete:'%A'" (rhType hatchId) delete
        let attr = rhobj.Attributes
        let rc = Rarr()
        for piece in pieces do
            match piece with
            | :? Curve as c->
                let g = Doc.Objects.AddCurve(c, attr)
                if g<>Guid.Empty then rc.Add(g)
            | :? Brep as c->
                let g = Doc.Objects.AddBrep(c, attr)
                if g<>Guid.Empty then rc.Add(g)
            | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.ExplodeHatch: darwing of %A objects after exploding not implemented" piece.ObjectType //TODO test with hatch patterns that have points
        if delete then Doc.Objects.Delete(rhobj)|> ignore
        rc


    [<Extension>]
    ///<summary>Returns a Hatch object's Hatch pattern.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<returns>(string) The current Hatch pattern.</returns>
    static member HatchPattern(hatchId:Guid) : string = //GET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let oldindex = hatchobj.HatchGeometry.PatternIndex
        Doc.HatchPatterns.[oldindex].Name

    [<Extension>]
    ///<summary>Changes a Hatch object's Hatch pattern.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern to replace the
    ///    current Hatch pattern</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchPattern(hatchId:Guid, hatchPattern:string) : unit = //SET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let oldindex = hatchobj.HatchGeometry.PatternIndex
        RhinoScriptSyntax.InitHatchPatterns()
        let newpatt = Doc.HatchPatterns.FindName(hatchPattern)
        if newpatt|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (rhType hatchId) hatchPattern
        hatchobj.HatchGeometry.PatternIndex <- newpatt.Index
        if not<| hatchobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (rhType hatchId) hatchPattern
        Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Changes multiple Hatch objects's Hatch pattern.</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple Hatch objects</param>
    ///<param name="hatchPattern">(string) Name of multiple existing Hatch pattern to replace the
    ///    current Hatch pattern</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchPattern(hatchIds:Guid seq, hatchPattern:string) : unit = //MULTISET
        RhinoScriptSyntax.InitHatchPatterns()
        for hatchId in hatchIds do 
            let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
            let oldindex = hatchobj.HatchGeometry.PatternIndex            
            let newpatt = Doc.HatchPatterns.FindName(hatchPattern)
            if newpatt|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (rhType hatchId) hatchPattern
            hatchobj.HatchGeometry.PatternIndex <- newpatt.Index
            if not<| hatchobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (rhType hatchId) hatchPattern
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the number of Hatch patterns in the document.</summary>
    ///<returns>(int) The number of Hatch patterns in the document.</returns>
    static member HatchPatternCount() : int =
        RhinoScriptSyntax.InitHatchPatterns()
        Doc.HatchPatterns.Count


    [<Extension>]
    ///<summary>Returns the description of a Hatch pattern. Note, not all Hatch patterns
    ///    have descriptions.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(string) description of the Hatch pattern.</returns>
    static member HatchPatternDescription(hatchPattern:string) : string =
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPatternDescription failed.  hatchPattern:'%A'" hatchPattern
        patterninstance.Description


    [<Extension>]
    ///<summary>Returns the fill type of a Hatch pattern.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(int) Hatch pattern's fill type
    ///    0 = solid, uses object color
    ///    1 = lines, uses pattern file definition
    ///    2 = gradient, uses fill color definition.</returns>
    static member HatchPatternFillType(hatchPattern:string) : int =
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchPatternFillType failed.  hatchPattern:'%A'" hatchPattern
        int(patterninstance.FillType)


    [<Extension>]
    ///<summary>Returns the names of all of the Hatch patterns in the document.</summary>
    ///<returns>(string Rarr) The names of all of the Hatch patterns in the document.</returns>
    static member HatchPatternNames() : string Rarr =
        RhinoScriptSyntax.InitHatchPatterns()
        let rc = Rarr()
        for i in range(Doc.HatchPatterns.Count) do
            let hatchpattern = Doc.HatchPatterns.[i]
            if not hatchpattern.IsDeleted then
                rc.Add(hatchpattern.Name)
        rc


    [<Extension>]
    ///<summary>Returns the rotation applied to the Hatch pattern when
    /// it is mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<returns>(float) if rotation is not defined, the current rotation angle.</returns>
    static member HatchRotation(hatchId:Guid) : float = //GET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let rc = hatchobj.HatchGeometry.PatternRotation
        RhinoMath.ToDegrees(rc)


    [<Extension>]
    ///<summary>Modifies the rotation applied to the Hatch pattern when
    /// it is mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="rotation">(float) Rotation angle in degrees</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchRotation(hatchId:Guid, rotation:float) : unit = //SET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let mutable rc = hatchobj.HatchGeometry.PatternRotation
        rc <- RhinoMath.ToDegrees(rc)
        if rotation <> rc then
            let rotation = RhinoMath.ToRadians(rotation)
            hatchobj.HatchGeometry.PatternRotation <- rotation
            if not <| hatchobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchRotation failed on rotation %f on %A" rotation hatchId
            Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Modifies the rotation applied to the Hatch pattern when
    /// it is mapped to the Hatch's Plane.</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple Hatch objects</param>
    ///<param name="rotation">(float) Rotation angle in degrees</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchRotation(hatchIds:Guid seq, rotation:float) : unit = //MULTISET
        for hatchId in hatchIds do  
            let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
            let mutable rc = hatchobj.HatchGeometry.PatternRotation
            rc <- RhinoMath.ToDegrees(rc)
            if rotation <> rc then
                let rotation = RhinoMath.ToRadians(rotation)
                hatchobj.HatchGeometry.PatternRotation <- rotation
                if not <| hatchobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchRotation failed on rotation %f on %A" rotation hatchId
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the scale applied to the Hatch pattern when it is
    /// mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<returns>(float) if scale is not defined, the current scale factor.</returns>
    static member HatchScale(hatchId:Guid) : float = //GET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        hatchobj.HatchGeometry.PatternScale


    [<Extension>]
    ///<summary>Modifies the scale applied to the Hatch pattern when it is
    /// mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="scale">(float) Scale factor</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchScale(hatchId:Guid, scale:float) : unit = //SET
        let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
        let rc = hatchobj.HatchGeometry.PatternScale
        if scale <> rc then
            hatchobj.HatchGeometry.PatternScale <- scale
            if not <| hatchobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchScale failed on scale %f on %A" scale hatchId
            Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Modifies the scale applied to the Hatch pattern when it is
    /// mapped to the Hatch's Plane.</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple Hatch objects</param>
    ///<param name="scale">(float) Scale factor</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchScale(hatchIds:Guid seq, scale:float) : unit = //MULTISET
        for hatchId in hatchIds do  
            let hatchobj = RhinoScriptSyntax.CoerceHatchObject(hatchId)
            let rc = hatchobj.HatchGeometry.PatternScale
            if scale <> rc then
                hatchobj.HatchGeometry.PatternScale <- scale
                if not <| hatchobj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.HatchScale failed on scale %f on %A" scale hatchId
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Verifies the existence of a Hatch object in the document.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatch(objectId:Guid) : bool =
        let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        match rhobj with :? DocObjects.HatchObject  -> true |_ -> false


    [<Extension>]
    ///<summary>Verifies the existence of a Hatch pattern in the document.</summary>
    ///<param name="name">(string) The name of a Hatch pattern</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatchPattern(name:string) : bool =
        RhinoScriptSyntax.InitHatchPatterns()
        Doc.HatchPatterns.FindName(name) |> notNull


    [<Extension>]
    ///<summary>Verifies that a Hatch pattern is the current Hatch pattern.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatchPatternCurrent(hatchPattern:string) : bool =
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.IsHatchPatternCurrent failed.  hatchPattern:'%A'" hatchPattern
        patterninstance.Index = Doc.HatchPatterns.CurrentHatchPatternIndex


    [<Extension>]
    ///<summary>Verifies that a Hatch pattern is from a reference file.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatchPatternReference(hatchPattern:string) : bool =
        RhinoScriptSyntax.InitHatchPatterns()
        let patterninstance = Doc.HatchPatterns.FindName(hatchPattern)
        if patterninstance|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.IsHatchPatternReference failed.  hatchPattern:'%A'" hatchPattern
        patterninstance.IsReference


