namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsToolbar =
    type RhinoScriptSyntax with

    [<EXT>]
    ///<summary>Closes a currently open toolbar collection</summary>
    ///<param name="name">(string) Name of a currently open toolbar collection</param>
    ///<param name="prompt">(bool) Optional, Default Value: <c>false</c>
    ///If True, user will be prompted to save the collection file
    ///  if it has been modified prior to closing</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CloseToolbarCollection(name:string, [<OPT;DEF(false)>]prompt:bool) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.Close(prompt)
        else false


    [<EXT>]
    ///<summary>Hides a previously visible toolbar group in an open toolbar collection</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group to hide</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
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


    [<EXT>]
    ///<summary>Verifies a toolbar (or toolbar group) exists in an open collection file</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbar">(string) Name of a toolbar group</param>
    ///<param name="group">(bool) Optional, Default Value: <c>false</c>
    ///If toolbar parameter is referring to a toolbar group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbar(name:string, toolbar:string, [<OPT;DEF(false)>]group:bool) : bool =
       let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
       if notNull tbfile then
           if group then
              tbfile.GetGroup(toolbar) |> notNull
           else
              seq { for i = 0 to tbfile.ToolbarCount-1 do yield tbfile.GetToolbar(i).Name }
              |> Seq.exists (fun n -> n= toolbar)
           //tbfile.GetToolbar(toolbar) <> null // Fails in Rhino 5 with string
       else
           false


    [<EXT>]
    ///<summary>Verifies that a toolbar collection is open</summary>
    ///<param name="file">(string) Full path to a toolbar collection file</param>
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member IsToolbarCollection(file:string) : string =
        let tbfile = RhinoApp.ToolbarFiles.FindByPath(file)
        if notNull tbfile then  tbfile.Name
        else ""


    [<EXT>]
    ///<summary>Verifies that a toolbar group in an open toolbar collection is visible</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbarDocked(name:string, toolbarGroup:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then
            let group = tbfile.GetGroup(toolbarGroup)
            if notNull group then  group.IsDocked
            else failwithf "isToolbarDocked failed on name '%s'" name
        else failwithf "isToolbarDocked failed on name '%s'" name


    [<EXT>]
    ///<summary>Verifies that a toolbar group in an open toolbar collection is visible</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbarVisible(name:string, toolbarGroup:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then
            let group = tbfile.GetGroup(toolbarGroup)
            if notNull group then  group.Visible
            else failwithf "isToolbarVisible failed on name '%s'" name
        else failwithf "isToolbarVisible failed on name '%s'" name


    [<EXT>]
    ///<summary>Opens a toolbar collection file</summary>
    ///<param name="file">(string) Full path to the collection file</param>
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member OpenToolbarCollection(file:string) : string =
        let tbfile = RhinoApp.ToolbarFiles.Open(file)
        if notNull tbfile then  tbfile.Name
        else failwithf "openToolbarCollection failed on file '%s'" file


    [<EXT>]
    ///<summary>Saves an open toolbar collection to disk</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollection(name:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.Save()
        else false


    [<EXT>]
    ///<summary>Saves an open toolbar collection to a different disk file</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="file">(string) Full path to file name to save to</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollectionAs(name:string, file:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.SaveAs(file)
        else false


    [<EXT>]
    ///<summary>Shows a previously hidden toolbar group in an open toolbar collection</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group to show</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
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


    [<EXT>]
    ///<summary>Returns number of currently open toolbar collections</summary>
    ///<returns>(int) the number of currently open toolbar collections</returns>
    static member ToolbarCollectionCount() : int =
        RhinoApp.ToolbarFiles.Count


    [<EXT>]
    ///<summary>Returns names of all currently open toolbar collections</summary>
    ///<returns>(string ResizeArray) the names of all currently open toolbar collections</returns>
    static member ToolbarCollectionNames() : string ResizeArray =
        resizeArray { for tbfile in RhinoApp.ToolbarFiles -> tbfile.Name }


    [<EXT>]
    ///<summary>Returns full path to a currently open toolbar collection file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<returns>(string) full path on success</returns>
    static member ToolbarCollectionPath(name:string) : string =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.Path
        else ""


    [<EXT>]
    ///<summary>Returns the number of toolbars or groups in a currently open toolbar file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<param name="groups">(bool) Optional, Default Value: <c>false</c>
    ///If true, return the number of toolbar groups in the file</param>
    ///<returns>(int) number of toolbars on success</returns>
    static member ToolbarCount(name:string, [<OPT;DEF(false)>]groups:bool) : int =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then
            if groups then  tbfile.GroupCount
            else tbfile.ToolbarCount
        else
            -1


    [<EXT>]
    ///<summary>Returns the names of all toolbars (or toolbar groups) found in a
    ///  currently open toolbar file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<param name="groups">(bool) Optional, Default Value: <c>false</c>
    ///If true, return the names of toolbar groups in the file</param>
    ///<returns>(string ResizeArray) names of all toolbars (or toolbar groups) on success</returns>
    static member ToolbarNames(name:string, [<OPT;DEF(false)>]groups:bool) : string ResizeArray =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        let rc = ResizeArray()
        if notNull tbfile then
            if groups then
                for i = 0 to tbfile.GroupCount-1 do rc.Add(tbfile.GetGroup(i).Name)
            else
                for i = 0 to tbfile.ToolbarCount-1 do rc.Add(tbfile.GetToolbar(i).Name)
        rc


