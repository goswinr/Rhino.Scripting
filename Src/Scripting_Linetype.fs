namespace Rhino.Scripting

open Rhino
open Rhino.Scripting.RhinoScriptingUtils

[<AutoOpen>]
module AutoOpenLinetype =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    /// <summary>Verifies the existence of a linetype in the document.</summary>
    /// <param name="name">(string) The name of an existing linetype</param>
    /// <returns>(bool) True or False.</returns>
    static member IsLinetype(name:string) : bool =
        notNull <| State.Doc.Linetypes.FindName(name)


    /// <summary>Checks if an existing linetype is from a reference file.</summary>
    /// <param name="name">(string) The name of an existing linetype</param>
    /// <returns>(bool) True or False.</returns>
    static member IsLinetypeReference(name:string) : bool =
        let lt = State.Doc.Linetypes.FindName(name)
        if isNull lt then RhinoScriptingException.Raise "RhinoScriptSyntax.IsLinetypeReference unable to find '%s' in a line-types" name
        lt.IsReference


    /// <summary>Returns the number of linetypes in the document.</summary>
    /// <returns>(int) The number of linetypes in the document.</returns>
    static member LinetypeCount() : int =
        State.Doc.Linetypes.Count


    /// <summary>Returns names of all linetypes in the document.</summary>
    /// <returns>(string ResizeArray) List of linetype names.</returns>
    static member LinetypeNames() : string ResizeArray =
        let count = State.Doc.Linetypes.Count
        let rc = ResizeArray()
        for i = 0 to count - 1 do
            let linetype = State.Doc.Linetypes.[i]
            if not linetype.IsDeleted then  rc.Add(linetype.Name)
        rc



