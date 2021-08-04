namespace Rhino.Scripting

open FsEx
open System
open Rhino

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

 
/// This module is automatically opened when Rhino.Scripting namespace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
[<AutoOpen>]
module ExtensionsLinetype =

  
  type RhinoScriptSyntax with

    ///<summary>Verifies the existance of a linetype in the document.</summary>
    ///<param name="name">(string) The name of an existing linetype</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsLinetype(name:string) : bool =
        notNull <| State.Doc.Linetypes.FindName(name)


    ///<summary>Verifies that an existing linetype is from a reference file.</summary>
    ///<param name="name">(string) The name of an existing linetype</param>
    ///<returns>(bool) True or False.</returns>
    [<Extension>]
    static member IsLinetypeReference(name:string) : bool =
        let lt = State.Doc.Linetypes.FindName(name)
        if isNull lt then RhinoScriptingException.Raise "RhinoScriptSyntax.IsLinetypeReference unable to find '%s' in a linetypes" name
        lt.IsReference


    ///<summary>Returns number of linetypes in the document.</summary>
    ///<returns>(int) The number of linetypes in the document.</returns>
    [<Extension>]
    static member LinetypeCount() : int =
        State.Doc.Linetypes.Count


    ///<summary>Returns names of all linetypes in the document.</summary>
    ///<returns>(string Rarr) list of linetype names.</returns>
    [<Extension>]
    static member LinetypeNames() : string Rarr =
        let count = State.Doc.Linetypes.Count
        let rc = Rarr()
        for i = 0 to count - 1 do
            let linetype = State.Doc.Linetypes.[i]
            if not linetype.IsDeleted then  rc.Add(linetype.Name)
        rc


