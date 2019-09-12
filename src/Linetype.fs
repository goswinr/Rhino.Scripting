namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsLinetype =
  type RhinoScriptSyntax with
    
    
    static member internal Getlinetype() : obj =
        failNotImpl () 


    ///<summary>Verifies the existance of a linetype in the document</summary>
    ///<param name="nameOrId">(Guid) The name or identifier of an existing linetype.</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinetype(nameOrId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies that an existing linetype is from a reference file</summary>
    ///<param name="nameOrId">(Guid) The name or identifier of an existing linetype.</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinetypeReference(nameOrId:Guid) : bool =
        failNotImpl () 


    ///<summary>Returns number of linetypes in the document</summary>
    ///<returns>(int) the number of linetypes in the document</returns>
    static member LinetypeCount() : int =
        failNotImpl () 


    ///<summary>Returns names of all linetypes in the document</summary>
    ///<returns>(string seq) list of linetype names</returns>
    static member LinetypeNames() : string seq =
        failNotImpl () 


