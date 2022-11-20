
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings
open Rhino.ScriptingFSharp

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenHatch =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    static member private InitHatchPatterns() : unit = // TODO, optimize so that this init is ony done once ?
        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Solid.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Solid) |> ignore

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Hatch1.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Hatch1)|> ignore

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Hatch2.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Hatch2)|> ignore

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Hatch3.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Hatch3)|> ignore

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Dash.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Dash)|> ignore

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Grid.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Grid)|> ignore

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Grid60.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Grid60)|> ignore

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Plus.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Plus)|> ignore

        if isNull <| State.Doc.HatchPatterns.FindName(DocObjects.HatchPattern.Defaults.Squares.Name) then
            State.Doc.HatchPatterns.Add(DocObjects.HatchPattern.Defaults.Squares)|> ignore

    

    ///<summary>Creates one or more new Hatch objects from a list of closed planar Curves.</summary>
    ///<param name="curves">(Curve seq) Geometry of the closed planar Curves that defines the boundary of the Hatch objects</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch object.  If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>  Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c>  Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>State.Doc.ModelAbsoluteTolerance</c>  Tolerance for Hatch fills</param>
    ///<returns>(Guid Rarr) identifiers of the newly created Hatch.</returns>
    static member AddHatches( curves:Curve seq,
                              [<OPT;DEF(null:string)>]hatchPattern:string,
                              [<OPT;DEF(1.0)>]scale:float,
                              [<OPT;DEF(0.0)>]rotation:float,
                              [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr = 
        Scripting.InitHatchPatterns()
        let mutable index = State.Doc.HatchPatterns.CurrentHatchPatternIndex
        if notNull hatchPattern then
            let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
            index <-  if patternInstance|> isNull then RhinoMath.UnsetIntIndex else patternInstance.Index
            if index<0 then RhinoScriptingException.Raise "Rhino.Scripting.AddHatches failed to find hatchPattern:'%s'"  hatchPattern           
        let rotation = RhinoMath.ToRadians(rotation)

        let tolerance = if tolerance <= 0.0 then State.Doc.ModelAbsoluteTolerance else tolerance
        let hatches = Hatch.Create(curves, index, rotation, scale, tolerance)
        if isNull hatches then 
            RhinoScriptingException.Raise "Rhino.Scripting.AddHatches failed to create hatch from %d curves, not closed: %d, not planar %d, tolerance:'%g' " 
                (Seq.length curves) 
                (curves |> Seq.countIf ( fun c -> c.IsClosed   |> not )) 
                (curves |> Seq.countIf ( fun c -> c.IsPlanar() |> not )) 
                tolerance
        let ids = Rarr()
        for hatch in hatches do
            let objectId = State.Doc.Objects.AddHatch(hatch)
            if objectId <> Guid.Empty then
                ids.Add(objectId)
        if ids.Count = 0 then 
            RhinoScriptingException.Raise "Rhino.Scripting.AddHatches failed to add any hatches from %d curves, not closed: %d, not planar %d, tolerance:'%g' " 
                (Seq.length curves) 
                (curves |> Seq.countIf ( fun c -> c.IsClosed   |> not )) 
                (curves |> Seq.countIf ( fun c -> c.IsPlanar() |> not )) 
                tolerance
        State.Doc.Views.Redraw()
        ids
    
    ///<summary>Creates one  Hatch objects from one closed planar Curve.</summary>
    ///<param name="curve">(Curve) Curve Geometry of the closed planar Curve that defines the boundary of the Hatch object</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch object. If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>   Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c> Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>State.Doc.ModelAbsoluteTolerance</c> Tolerance for Hatch fills</param>
    ///<returns>(Guid) identifier of the newly created Hatch.</returns>
    static member AddHatch(   curve:Curve,
                              [<OPT;DEF(null:string)>]hatchPattern:string,
                              [<OPT;DEF(1.0)>]scale:float,
                              [<OPT;DEF(0.0)>]rotation:float,
                              [<OPT;DEF(0.0)>]tolerance:float) : Guid  =         
        try 
           let rc = Scripting.AddHatches([curve], hatchPattern, scale, rotation,tolerance) 
           if rc.Count = 1 then rc.[0]
           else RhinoScriptingException.Raise "Rhino.Scripting.AddHatch failed to create exactly on hatch from curve.  It created %d Hatches"  rc.Count 
        with e->
            let tolerance = if tolerance <= 0.0 then State.Doc.ModelAbsoluteTolerance else tolerance
            RhinoScriptingException.Raise "Rhino.Scripting.AddHatch failed on one curve \r\nMessage: %s"  e.Message
            
        
    ///<summary>Creates one or more new Hatch objects from a list of closed planar Curves.</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of the closed planar Curves that defines the   boundary of the Hatch objects</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch object. If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>   Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c> Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>State.Doc.ModelAbsoluteTolerance</c> Tolerance for Hatch fills</param>
    ///<returns>(Guid Rarr) identifiers of the newly created Hatch.</returns>
    static member AddHatches(  curveIds:Guid seq,
                              [<OPT;DEF(null:string)>]hatchPattern:string,
                              [<OPT;DEF(1.0)>]scale:float,
                              [<OPT;DEF(0.0)>]rotation:float,
                              [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr = 
        let curves =  rarr { for objectId in curveIds do yield Scripting.CoerceCurve(objectId) }
        try Scripting.AddHatches(curves, hatchPattern, scale, rotation) 
        with e->
            let tolerance = if tolerance <= 0.0 then State.Doc.ModelAbsoluteTolerance else tolerance
            RhinoScriptingException.Raise "Rhino.Scripting.AddHatches failed on curveIds:'%s' \r\nMessage: %s" (Nice.str curveIds)  e.Message
            
    ///<summary>Creates a new Hatch object from a closed planar Curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the closed planar Curve that defines the boundary of the Hatch object</param>
    ///<param name="hatchPattern">(string) Optional, Name of the Hatch pattern to be used by the Hatch object. If omitted, the current Hatch pattern will be used</param>
    ///<param name="scale">(float) Optional, Default Value: <c>1.0</c>  Hatch pattern scale factor</param>
    ///<param name="rotation">(float) Optional, Default Value: <c>0.0</c> Hatch pattern rotation angle in degrees</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>State.Doc.ModelAbsoluteTolerance</c> Tolerance for Hatch fills</param>
    ///<returns>(Guid) identifier of the newly created Hatch.</returns>
    static member AddHatch( curveId:Guid,
                            [<OPT;DEF(null:string)>]hatchPattern:string,
                            [<OPT;DEF(1.0)>]scale:float,
                            [<OPT;DEF(0.0)>]rotation:float,
                            [<OPT;DEF(0.0)>]tolerance:float) : Guid = 
        try Scripting.AddHatch(Scripting.CoerceCurve(curveId), hatchPattern, scale, rotation, tolerance)             
        with e->
            let tolerance = if tolerance <= 0.0 then State.Doc.ModelAbsoluteTolerance else tolerance
            RhinoScriptingException.Raise "Rhino.Scripting.AddHatch failed on one curve %s\r\nMessage: %s" (Nice.str curveId)  e.Message
       



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
        if isNull patterns then RhinoScriptingException.Raise "Rhino.Scripting.AddHatchPatterns failed.  filename:'%A' replace:'%A'" filename replace
        let rc = Rarr()
        for pattern in patterns do
             let index = State.Doc.HatchPatterns.Add(pattern)
             if index>=0 then
                 let pattern = State.Doc.HatchPatterns.[index]
                 rc.Add(pattern.Name)
        if  rc.Count = 0 then RhinoScriptingException.Raise "Rhino.Scripting.AddHatchPatterns failed.  filename:'%A' replace:'%A'" filename replace
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
        let rc = State.Doc.HatchPatterns.CurrentHatchPatternIndex
        Scripting.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.CurrentHatchPattern: Setting it failed. hatchPattern:'%A'" hatchPattern
        State.Doc.HatchPatterns.CurrentHatchPatternIndex <- patternInstance.Index


    ///<summary>Explodes a Hatch object into its component objects. The exploded objects
    ///    will be added to the document. If the Hatch object uses a solid pattern,
    ///    then planar face Brep objects will be created. Otherwise, line Curve objects
    ///    will be created.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the Hatch object</param>
    ///<returns>(Guid Rarr) list of identifiers for the newly created objects.</returns>
    static member ExplodeHatch(hatchId:Guid, [<OPT;DEF(false)>]delete:bool) : Guid Rarr = 
        let rhobj = Scripting.CoerceRhinoObject(hatchId)
        let geo =  Scripting.CoerceHatch(hatchId)
        let pieces = geo.Explode()
        if isNull pieces then RhinoScriptingException.Raise "Rhino.Scripting.ExplodeHatch failed.  hatchId:'%s' delete:'%A'" (Nice.str hatchId) delete
        let attr = rhobj.Attributes
        let rc = Rarr()
        for piece in pieces do
            match piece with
            | :? Curve as c->
                let g = State.Doc.Objects.AddCurve(c, attr)
                if g<>Guid.Empty then rc.Add(g)
            | :? Brep as c->
                let g = State.Doc.Objects.AddBrep(c, attr)
                if g<>Guid.Empty then rc.Add(g)
            | _ -> RhinoScriptingException.Raise "Rhino.Scripting.ExplodeHatch: drawing of %A objects after exploding not implemented" piece.ObjectType //TODO test with hatch patterns that have points
        if delete then State.Doc.Objects.Delete(rhobj)|> ignore
        rc


    ///<summary>Returns a Hatch object's Hatch pattern.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<returns>(string) The current Hatch pattern.</returns>
    static member HatchPattern(hatchId:Guid) : string = //GET
        let hatchObj = Scripting.CoerceHatchObject(hatchId)
        let oldIndex = hatchObj.HatchGeometry.PatternIndex
        State.Doc.HatchPatterns.[oldIndex].Name

    ///<summary>Changes a Hatch object's Hatch pattern.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern to replace the
    ///    current Hatch pattern</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchPattern(hatchId:Guid, hatchPattern:string) : unit = //SET
        let hatchObj = Scripting.CoerceHatchObject(hatchId)
        let oldIndex = hatchObj.HatchGeometry.PatternIndex
        Scripting.InitHatchPatterns()
        let newPattern = State.Doc.HatchPatterns.FindName(hatchPattern)
        if newPattern|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (Nice.str hatchId) hatchPattern
        hatchObj.HatchGeometry.PatternIndex <- newPattern.Index
        if not<| hatchObj.CommitChanges() then RhinoScriptingException.Raise "Rhino.Scripting.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (Nice.str hatchId) hatchPattern
        State.Doc.Views.Redraw()

    ///<summary>Changes multiple Hatch objects's Hatch pattern.</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple Hatch objects</param>
    ///<param name="hatchPattern">(string) Name of multiple existing Hatch pattern to replace the
    ///    current Hatch pattern</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchPattern(hatchIds:Guid seq, hatchPattern:string) : unit = //MULTISET
        Scripting.InitHatchPatterns()
        for hatchId in hatchIds do
            let hatchObj = Scripting.CoerceHatchObject(hatchId)
            let oldIndex = hatchObj.HatchGeometry.PatternIndex
            let newPattern = State.Doc.HatchPatterns.FindName(hatchPattern)
            if newPattern|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (Nice.str hatchId) hatchPattern
            hatchObj.HatchGeometry.PatternIndex <- newPattern.Index
            if not<| hatchObj.CommitChanges() then RhinoScriptingException.Raise "Rhino.Scripting.HatchPattern failed.  hatchId:'%s' hatchPattern:'%A'" (Nice.str hatchId) hatchPattern
        State.Doc.Views.Redraw()


    ///<summary>Returns the number of Hatch patterns in the document.</summary>
    ///<returns>(int) The number of Hatch patterns in the document.</returns>
    static member HatchPatternCount() : int = 
        Scripting.InitHatchPatterns()
        State.Doc.HatchPatterns.Count


    ///<summary>Returns the description of a Hatch pattern. Note, not all Hatch patterns
    ///    have descriptions.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(string) description of the Hatch pattern.</returns>
    static member HatchPatternDescription(hatchPattern:string) : string = 
        Scripting.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.HatchPatternDescription failed.  hatchPattern:'%A'" hatchPattern
        patternInstance.Description


    ///<summary>Returns the fill type of a Hatch pattern.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(int) Hatch pattern's fill type
    ///    0 = solid, uses object color
    ///    1 = lines, uses pattern file definition
    ///    2 = gradient, uses fill color definition.</returns>
    static member HatchPatternFillType(hatchPattern:string) : int = 
        Scripting.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.HatchPatternFillType failed.  hatchPattern:'%A'" hatchPattern
        int(patternInstance.FillType)


    ///<summary>Returns the names of all of the Hatch patterns in the document.</summary>
    ///<returns>(string Rarr) The names of all of the Hatch patterns in the document.</returns>
    static member HatchPatternNames() : string Rarr = 
        Scripting.InitHatchPatterns()
        let rc = Rarr()
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
        let hatchObj = Scripting.CoerceHatchObject(hatchId)
        let rc = hatchObj.HatchGeometry.PatternRotation
        RhinoMath.ToDegrees(rc)


    ///<summary>Modifies the rotation applied to the Hatch pattern when
    /// it is mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="rotation">(float) Rotation angle in degrees</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchRotation(hatchId:Guid, rotation:float) : unit = //SET
        let hatchObj = Scripting.CoerceHatchObject(hatchId)
        let mutable rc = hatchObj.HatchGeometry.PatternRotation
        rc <- RhinoMath.ToDegrees(rc)
        if rotation <> rc then
            let rotation = RhinoMath.ToRadians(rotation)
            hatchObj.HatchGeometry.PatternRotation <- rotation
            if not <| hatchObj.CommitChanges() then RhinoScriptingException.Raise "Rhino.Scripting.HatchRotation failed on rotation %f on %A" rotation hatchId
            State.Doc.Views.Redraw()

    ///<summary>Modifies the rotation applied to the Hatch pattern when
    /// it is mapped to the Hatch's Plane.</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple Hatch objects</param>
    ///<param name="rotation">(float) Rotation angle in degrees</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchRotation(hatchIds:Guid seq, rotation:float) : unit = //MULTISET
        for hatchId in hatchIds do
            let hatchObj = Scripting.CoerceHatchObject(hatchId)
            let mutable rc = hatchObj.HatchGeometry.PatternRotation
            rc <- RhinoMath.ToDegrees(rc)
            if rotation <> rc then
                let rotation = RhinoMath.ToRadians(rotation)
                hatchObj.HatchGeometry.PatternRotation <- rotation
                if not <| hatchObj.CommitChanges() then RhinoScriptingException.Raise "Rhino.Scripting.HatchRotation failed on rotation %f on %A" rotation hatchId
        State.Doc.Views.Redraw()


    ///<summary>Returns the scale applied to the Hatch pattern when it is
    /// mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<returns>(float) if scale is not defined, the current scale factor.</returns>
    static member HatchScale(hatchId:Guid) : float = //GET
        let hatchObj = Scripting.CoerceHatchObject(hatchId)
        hatchObj.HatchGeometry.PatternScale


    ///<summary>Modifies the scale applied to the Hatch pattern when it is
    /// mapped to the Hatch's Plane.</summary>
    ///<param name="hatchId">(Guid) Identifier of a Hatch object</param>
    ///<param name="scale">(float) Scale factor</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchScale(hatchId:Guid, scale:float) : unit = //SET
        let hatchObj = Scripting.CoerceHatchObject(hatchId)
        let rc = hatchObj.HatchGeometry.PatternScale
        if scale <> rc then
            hatchObj.HatchGeometry.PatternScale <- scale
            if not <| hatchObj.CommitChanges() then RhinoScriptingException.Raise "Rhino.Scripting.HatchScale failed on scale %f on %A" scale hatchId
            State.Doc.Views.Redraw()

    ///<summary>Modifies the scale applied to the Hatch pattern when it is
    /// mapped to the Hatch's Plane.</summary>
    ///<param name="hatchIds">(Guid seq) Identifiers of multiple Hatch objects</param>
    ///<param name="scale">(float) Scale factor</param>
    ///<returns>(unit) void, nothing.</returns>
    static member HatchScale(hatchIds:Guid seq, scale:float) : unit = //MULTISET
        for hatchId in hatchIds do
            let hatchObj = Scripting.CoerceHatchObject(hatchId)
            let rc = hatchObj.HatchGeometry.PatternScale
            if scale <> rc then
                hatchObj.HatchGeometry.PatternScale <- scale
                if not <| hatchObj.CommitChanges() then RhinoScriptingException.Raise "Rhino.Scripting.HatchScale failed on scale %f on %A" scale hatchId
        State.Doc.Views.Redraw()


    ///<summary>Verifies the existence of a Hatch object in the document.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatch(objectId:Guid) : bool = 
        let rhobj = Scripting.CoerceRhinoObject(objectId)
        match rhobj with :? DocObjects.HatchObject  -> true |_ -> false


    ///<summary>Verifies the existence of a Hatch pattern in the document.</summary>
    ///<param name="name">(string) The name of a Hatch pattern</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatchPattern(name:string) : bool = 
        Scripting.InitHatchPatterns()
        State.Doc.HatchPatterns.FindName(name) |> notNull


    ///<summary>Checks if a Hatch pattern is the current Hatch pattern.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatchPatternCurrent(hatchPattern:string) : bool = 
        Scripting.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.IsHatchPatternCurrent failed.  hatchPattern:'%A'" hatchPattern
        patternInstance.Index = State.Doc.HatchPatterns.CurrentHatchPatternIndex


    ///<summary>Checks if a Hatch pattern is from a reference file.</summary>
    ///<param name="hatchPattern">(string) Name of an existing Hatch pattern</param>
    ///<returns>(bool) True or False.</returns>
    static member IsHatchPatternReference(hatchPattern:string) : bool = 
        Scripting.InitHatchPatterns()
        let patternInstance = State.Doc.HatchPatterns.FindName(hatchPattern)
        if patternInstance|> isNull  then RhinoScriptingException.Raise "Rhino.Scripting.IsHatchPatternReference failed.  hatchPattern:'%A'" hatchPattern
        patternInstance.IsReference



