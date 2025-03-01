
namespace Rhino.Scripting

open Rhino

open System

open ResizeArray
// open FsEx
// open FsEx.SaveIgnore

[<AutoOpen>]
module AutoOpenToolbar =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>Closes a currently open tool-bar collection.</summary>
    ///<param name="name">(string) Name of a currently open tool-bar collection</param>
    ///<param name="prompt">(bool) Optional, default value: <c>false</c>
    ///    If True, user will be prompted to save the collection file
    ///    if it has been modified prior to closing</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member CloseToolbarCollection(name:string, [<OPT;DEF(false)>]prompt:bool) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.Close(prompt)
        else false


    ///<summary>Hides a previously visible tool-bar group in an open tool-bar collection.</summary>
    ///<param name="name">(string) Name of a currently open tool-bar file</param>
    ///<param name="toolbarGroup">(string) Name of a tool-bar group to hide</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member HideToolbar(name:string, toolbarGroup:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then
            let group = tbfile.GetGroup(toolbarGroup)
            if notNull group then
                group.Visible <- false
                true
            else
                false
        else
            false


    ///<summary>Verifies a tool-bar (or tool-bar group) exists in an open collection file.</summary>
    ///<param name="name">(string) Name of a currently open tool-bar file</param>
    ///<param name="toolbar">(string) Name of a tool-bar group</param>
    ///<param name="group">(bool) Optional, default value: <c>false</c>
    ///    If tool-bar parameter is referring to a tool-bar group</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsToolbar(name:string, toolbar:string, [<OPT;DEF(false)>]group:bool) : bool =
       let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
       if notNull tbfile then
           if group then
              tbfile.GetGroup(toolbar) |> notNull
           else
              seq { for i = 0 to tbfile.ToolbarCount-1 do yield tbfile.GetToolbar(i).Name }
              |> Seq.exists (fun n -> n= toolbar)
           //tbfile.GetToolbar(tool-bar) <> null // Fails in Rhino 5 with string
       else
           false


    ///<summary>Checks if a tool-bar collection is open.</summary>
    ///<param name="file">(string) Full path to a tool-bar collection file</param>
    ///<returns>(string) Rhino-assigned name of the tool-bar collection.</returns>
    static member IsToolbarCollection(file:string) : string =
        let tbfile = RhinoApp.ToolbarFiles.FindByPath(file)
        if notNull tbfile then  tbfile.Name
        else ""


    ///<summary>Checks if a tool-bar group in an open tool-bar collection is visible.</summary>
    ///<param name="name">(string) Name of a currently open tool-bar file</param>
    ///<param name="toolbarGroup">(string) Name of a tool-bar group</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsToolbarDocked(name:string, toolbarGroup:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then
            let group = tbfile.GetGroup(toolbarGroup)
            if notNull group then  group.IsDocked
            else RhinoScriptingException.Raise "RhinoScriptSyntax.IsToolbarDocked failed on name '%s'" name
        else RhinoScriptingException.Raise "RhinoScriptSyntax.IsToolbarDocked failed on name '%s'" name


    ///<summary>Checks if a tool-bar group in an open tool-bar collection is visible.</summary>
    ///<param name="name">(string) Name of a currently open tool-bar file</param>
    ///<param name="toolbarGroup">(string) Name of a tool-bar group</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsToolbarVisible(name:string, toolbarGroup:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then
            let group = tbfile.GetGroup(toolbarGroup)
            if notNull group then  group.Visible
            else RhinoScriptingException.Raise "RhinoScriptSyntax.IsToolbarVisible failed on name '%s'" name
        else RhinoScriptingException.Raise "RhinoScriptSyntax.IsToolbarVisible failed on name '%s'" name


    ///<summary>Opens a tool-bar collection file.</summary>
    ///<param name="file">(string) Full path to the collection file</param>
    ///<returns>(string) Rhino-assigned name of the tool-bar collection.</returns>
    static member OpenToolbarCollection(file:string) : string =
        let tbfile = RhinoApp.ToolbarFiles.Open(file)
        if notNull tbfile then  tbfile.Name
        else RhinoScriptingException.Raise "RhinoScriptSyntax.OpenToolbarCollection failed on file '%s'" file


    ///<summary>Saves an open tool-bar collection to disk.</summary>
    ///<param name="name">(string) Name of a currently open tool-bar file</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member SaveToolbarCollection(name:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.Save()
        else false


    ///<summary>Saves an open tool-bar collection to a different disk file.</summary>
    ///<param name="name">(string) Name of a currently open tool-bar file</param>
    ///<param name="file">(string) Full path to file name to save to</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member SaveToolbarCollectionAs(name:string, file:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.SaveAs(file)
        else false


    ///<summary>Shows a previously hidden tool-bar group in an open tool-bar collection.</summary>
    ///<param name="name">(string) Name of a currently open tool-bar file</param>
    ///<param name="toolbarGroup">(string) Name of a tool-bar group to show</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member ShowToolbar(name:string, toolbarGroup:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then
            let group = tbfile.GetGroup(toolbarGroup)
            if notNull group then
                group.Visible <- true
                true
            else
                false
        else
            false


    ///<summary>Returns number of currently open tool-bar collections.</summary>
    ///<returns>(int) The number of currently open tool-bar collections.</returns>
    static member ToolbarCollectionCount() : int =
        RhinoApp.ToolbarFiles.Count


    ///<summary>Returns names of all currently open tool-bar collections.</summary>
    ///<returns>(string ResizeArray) The names of all currently open tool-bar collections.</returns>
    static member ToolbarCollectionNames() : string ResizeArray =
        resizeArray { for tbfile in RhinoApp.ToolbarFiles -> tbfile.Name }


    ///<summary>Returns full path to a currently open tool-bar collection file.</summary>
    ///<param name="name">(string) Name of currently open tool-bar collection</param>
    ///<returns>(string) The full path.</returns>
    static member ToolbarCollectionPath(name:string) : string =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.Path
        else ""


    ///<summary>Returns the number of tool-bars or groups in a currently open tool-bar file.</summary>
    ///<param name="name">(string) Name of currently open tool-bar collection</param>
    ///<param name="groups">(bool) Optional, default value: <c>false</c>
    ///    If true, return the number of tool-bar groups in the file</param>
    ///<returns>(int) number of tool-bars.</returns>
    static member ToolbarCount(name:string, [<OPT;DEF(false)>]groups:bool) : int =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then
            if groups then  tbfile.GroupCount
            else tbfile.ToolbarCount
        else
            -1


    ///<summary>Returns the names of all tool-bars (or tool-bar groups) found in a
    ///    currently open tool-bar file.</summary>
    ///<param name="name">(string) Name of currently open tool-bar collection</param>
    ///<param name="groups">(bool) Optional, default value: <c>false</c>
    ///    If true, return the names of tool-bar groups in the file</param>
    ///<returns>(string ResizeArray) names of all tool-bars (or tool-bar groups).</returns>
    static member ToolbarNames(name:string, [<OPT;DEF(false)>]groups:bool) : string ResizeArray =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        let rc = ResizeArray()
        if notNull tbfile then
            if groups then
                for i = 0 to tbfile.GroupCount-1 do rc.Add(tbfile.GetGroup(i).Name)
            else
                for i = 0 to tbfile.ToolbarCount-1 do rc.Add(tbfile.GetToolbar(i).Name)
        rc



