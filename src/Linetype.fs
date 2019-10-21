namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsLinetype =
  type RhinoScriptSyntax with 

    [<EXT>]
    ///<summary>Verifies the existance of a linetype in the document</summary>
    ///<param name="name">(string) The name of an existing linetype.</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinetype(name:string) : bool =
        notNull <| Doc.Linetypes.FindName(name)        


    [<EXT>]
    ///<summary>Verifies that an existing linetype is from a reference file</summary>
    ///<param name="name">(string) The name of an existing linetype.</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinetypeReference(name:string) : bool =
        let lt = Doc.Linetypes.FindName(name)
        if isNull lt then failwithf "isLinetypeReference unable to find '%s' in a linetypes" name
        lt.IsReference


    [<EXT>]
    ///<summary>Returns number of linetypes in the document</summary>
    ///<returns>(int) the number of linetypes in the document</returns>
    static member LinetypeCount() : int =
        Doc.Linetypes.Count


    [<EXT>]
    ///<summary>Returns names of all linetypes in the document</summary>
    ///<returns>(string seq) list of linetype names</returns>
    static member LinetypeNames() : string ResizeArray =
        let count = Doc.Linetypes.Count
        let rc = ResizeArray()
        for i = 0 to count - 1 do
            let linetype = Doc.Linetypes.[i]
            if not linetype.IsDeleted then  rc.Add(linetype.Name)        
        rc


