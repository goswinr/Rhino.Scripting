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
        failNotImpl () 


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
        failNotImpl () 


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
        failNotImpl () 


    ///<summary>Adds hatch patterns to the document by importing hatch pattern definitions
    ///  from a pattern file.</summary>
    ///<param name="filename">(string) Name of the hatch pattern file</param>
    ///<param name="replace">(bool) Optional, Default Value: <c>false</c>
    ///If hatch pattern names already in the document match hatch
    ///  pattern names in the pattern definition file, then the existing hatch
    ///  patterns will be redefined</param>
    ///<returns>(string seq) Names of the newly added hatch patterns</returns>
    static member AddHatchPatterns(filename:string, [<OPT;DEF(false)>]replace:bool) : string seq =
        failNotImpl () 


    ///<summary>Returns the current hatch pattern file</summary>
    ///<returns>(string) The current hatch pattern</returns>
    static member CurrentHatchPattern() : string =
        failNotImpl () 

    ///<summary>Sets the current hatch pattern file</summary>
    ///<param name="hatchPattern">(string)Name of an existing hatch pattern to make current</param>
    ///<returns>(unit) unit</returns>
    static member CurrentHatchPattern(hatchPattern:string) : unit =
        failNotImpl () 


    ///<summary>Explodes a hatch object into its component objects. The exploded objects
    ///  will be added to the document. If the hatch object uses a solid pattern,
    ///  then planar face Brep objects will be created. Otherwise, line curve objects
    ///  will be created</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="delete">(bool) Optional, Default Value: <c>false</c>
    ///Delete the hatch object</param>
    ///<returns>(Guid seq) list of identifiers for the newly created objects</returns>
    static member ExplodeHatch(hatchId:Guid, [<OPT;DEF(false)>]delete:bool) : Guid seq =
        failNotImpl () 


    ///<summary>Returns a hatch object's hatch pattern</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(string) The current hatch pattern</returns>
    static member HatchPattern(hatchId:Guid) : string =
        failNotImpl () 

    ///<summary>Changes a hatch object's hatch pattern</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="hatchPattern">(string)Name of an existing hatch pattern to replace the
    ///  current hatch pattern</param>
    ///<returns>(unit) unit</returns>
    static member HatchPattern(hatchId:Guid, hatchPattern:string) : unit =
        failNotImpl () 


    ///<summary>Returns the number of hatch patterns in the document</summary>
    ///<returns>(int) the number of hatch patterns in the document</returns>
    static member HatchPatternCount() : int =
        failNotImpl () 


    ///<summary>Returns the description of a hatch pattern. Note, not all hatch patterns
    ///  have descriptions</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(string) description of the hatch pattern on success otherwise None</returns>
    static member HatchPatternDescription(hatchPattern:string) : string =
        failNotImpl () 


    ///<summary>Returns the fill type of a hatch pattern.</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(int) hatch pattern's fill type
    ///  0 = solid, uses object color
    ///  1 = lines, uses pattern file definition
    ///  2 = gradient, uses fill color definition</returns>
    static member HatchPatternFillType(hatchPattern:string) : int =
        failNotImpl () 


    ///<summary>Returns the names of all of the hatch patterns in the document</summary>
    ///<returns>(string seq) the names of all of the hatch patterns in the document</returns>
    static member HatchPatternNames() : string seq =
        failNotImpl () 


    ///<summary>Returns the rotation applied to the hatch pattern when
    /// it is mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(float) if rotation is not defined, the current rotation angle</returns>
    static member HatchRotation(hatchId:Guid) : float =
        failNotImpl () 

    ///<summary>Modifies the rotation applied to the hatch pattern when
    /// it is mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="rotation">(float)Rotation angle in degrees</param>
    ///<returns>(unit) unit</returns>
    static member HatchRotation(hatchId:Guid, rotation:float) : unit =
        failNotImpl () 


    ///<summary>Returns the scale applied to the hatch pattern when it is
    /// mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<returns>(float) if scale is not defined, the current scale factor</returns>
    static member HatchScale(hatchId:Guid) : float =
        failNotImpl () 

    ///<summary>Modifies the scale applied to the hatch pattern when it is
    /// mapped to the hatch's plane</summary>
    ///<param name="hatchId">(Guid) Identifier of a hatch object</param>
    ///<param name="scale">(float)Scale factor</param>
    ///<returns>(unit) unit</returns>
    static member HatchScale(hatchId:Guid, scale:float) : unit =
        failNotImpl () 


    ///<summary>Verifies the existence of a hatch object in the document</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatch(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies the existence of a hatch pattern in the document</summary>
    ///<param name="name">(string) The name of a hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPattern(name:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a hatch pattern is the current hatch pattern</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternCurrent(hatchPattern:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a hatch pattern is from a reference file</summary>
    ///<param name="hatchPattern">(string) Name of an existing hatch pattern</param>
    ///<returns>(bool) True or False</returns>
    static member IsHatchPatternReference(hatchPattern:string) : bool =
        failNotImpl () 


