
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenLinetype =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>Verifies the existence of a line-type in the document.</summary>
    ///<param name="name">(string) The name of an existing line-type</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLinetype(name:string) : bool = 
        notNull <| State.Doc.Linetypes.FindName(name)


    ///<summary>Checks if an existing line-type is from a reference file.</summary>
    ///<param name="name">(string) The name of an existing line-type</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLinetypeReference(name:string) : bool = 
        let lt = State.Doc.Linetypes.FindName(name)
        if isNull lt then RhinoScriptingException.Raise "Rhino.Scripting.IsLinetypeReference unable to find '%s' in a line-types" name
        lt.IsReference


    ///<summary>Returns number of line-types in the document.</summary>
    ///<returns>(int) The number of line-types in the document.</returns>
    static member LinetypeCount() : int = 
        State.Doc.Linetypes.Count


    ///<summary>Returns names of all line-types in the document.</summary>
    ///<returns>(string Rarr) list of line-type names.</returns>
    static member LinetypeNames() : string Rarr = 
        let count = State.Doc.Linetypes.Count
        let rc = Rarr()
        for i = 0 to count - 1 do
            let linetype = State.Doc.Linetypes.[i]
            if not linetype.IsDeleted then  rc.Add(linetype.Name)
        rc



