namespace Rhino.Scripting

open FsEx
open System
open Rhino

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore

 
[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsLinetype =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    [<Extension>]
    ///<summary>Verifies the existance of a linetype in the document.</summary>
    ///<param name="name">(string) The name of an existing linetype</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLinetype(name:string) : bool =
        notNull <| Doc.Linetypes.FindName(name)


    [<Extension>]
    ///<summary>Verifies that an existing linetype is from a reference file.</summary>
    ///<param name="name">(string) The name of an existing linetype</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLinetypeReference(name:string) : bool =
        let lt = Doc.Linetypes.FindName(name)
        if isNull lt then RhinoScriptingException.Raise "RhinoScriptSyntax.IsLinetypeReference unable to find '%s' in a linetypes" name
        lt.IsReference


    [<Extension>]
    ///<summary>Returns number of linetypes in the document.</summary>
    ///<returns>(int) The number of linetypes in the document.</returns>
    static member LinetypeCount() : int =
        Doc.Linetypes.Count


    [<Extension>]
    ///<summary>Returns names of all linetypes in the document.</summary>
    ///<returns>(string Rarr) list of linetype names.</returns>
    static member LinetypeNames() : string Rarr =
        let count = Doc.Linetypes.Count
        let rc = Rarr()
        for i = 0 to count - 1 do
            let linetype = Doc.Linetypes.[i]
            if not linetype.IsDeleted then  rc.Add(linetype.Name)
        rc


