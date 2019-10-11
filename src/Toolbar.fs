namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsToolbar =
  [<EXT>] 
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
    (*
    def CloseToolbarCollection(name, prompt=False):
        '''Closes a currently open toolbar collection
        Parameters:
          name (str): name of a currently open toolbar collection
          prompt  (bool, optional): if True, user will be prompted to save the collection file
            if it has been modified prior to closing
        Returns:
          bool: True or False indicating success or failure
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile: return tbfile.Close(prompt)
        return False
    *)


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
    (*
    def HideToolbar(name, toolbar_group):
        '''Hides a previously visible toolbar group in an open toolbar collection
        Parameters:
          name (str): name of a currently open toolbar file
          toolbar_group (str): name of a toolbar group to hide
        Returns:
          bool: True or False indicating success or failure
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile:
            group = tbfile.GetGroup(toolbar_group)
            if group:
                group.Visible = False
                return True
        return False
    *)


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
           if group then  tbfile.GetGroup(toolbar) <> null
           else
           seq { for i=0 to tbfile.ToolbarCount-1 do yield tbfile.GetToolbar(i).Name }
           |> Seq.exists (fun n -> n=toolbar)
           //tbfile.GetToolbar(toolbar) <> null // Fails in Rhino 5 with string
       else
           false
    (*
    def IsToolbar(name, toolbar, group=False):
        '''Verifies a toolbar (or toolbar group) exists in an open collection file
        Parameters:
          name (str): name of a currently open toolbar file
          toolbar (str): name of a toolbar group
          group (bool, optional): if toolbar parameter is referring to a toolbar group
        Returns:
          bool: True or False indicating success or failure
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile:
            if group: return tbfile.GetGroup(toolbar) != None
            return tbfile.GetToolbar(toolbar) != None
        return False
    *)


    [<EXT>]
    ///<summary>Verifies that a toolbar collection is open</summary>
    ///<param name="file">(string) Full path to a toolbar collection file</param>
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member IsToolbarCollection(file:string) : string =
        let tbfile = RhinoApp.ToolbarFiles.FindByPath(file)
        if notNull tbfile then  tbfile.Name
        else ""
    (*
    def IsToolbarCollection(file):
        '''Verifies that a toolbar collection is open
        Parameters:
          file (str): full path to a toolbar collection file
        Returns:
          str: Rhino-assigned name of the toolbar collection if successful
          None: if not successful
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByPath(file)
        if tbfile: return tbfile.Name
    *)


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
    (*
    def IsToolbarDocked(name, toolbar_group):
        '''Verifies that a toolbar group in an open toolbar collection is visible
        Parameters:
          name (str): name of a currently open toolbar file
          toolbar_group (str): name of a toolbar group
        Returns:
          boolean: True or False indicating success or failure
          None: on error
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile:
            group = tbfile.GetGroup(toolbar_group)
            if group: return group.IsDocked
    *)


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
    (*
    def IsToolbarVisible(name, toolbar_group):
        '''Verifies that a toolbar group in an open toolbar collection is visible
        Parameters:
          name (str): name of a currently open toolbar file
          toolbar_group (str): name of a toolbar group
        Returns:
          bool:True or False indicating success or failure
          None: on error
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile:
            group = tbfile.GetGroup(toolbar_group)
            if group: return group.Visible
    *)


    [<EXT>]
    ///<summary>Opens a toolbar collection file</summary>
    ///<param name="file">(string) Full path to the collection file</param>
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member OpenToolbarCollection(file:string) : string =
        let tbfile = RhinoApp.ToolbarFiles.Open(file)
        if notNull tbfile then  tbfile.Name
        else failwithf "openToolbarCollection failed on file '%s'" file
    (*
    def OpenToolbarCollection(file):
        '''Opens a toolbar collection file
        Parameters:
          file (str): full path to the collection file
        Returns:
          str: Rhino-assigned name of the toolbar collection if successful
          None: if not successful
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.Open(file)
        if tbfile: return tbfile.Name
    *)


    [<EXT>]
    ///<summary>Saves an open toolbar collection to disk</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollection(name:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.Save()
        else false
    (*
    def SaveToolbarCollection(name):
        '''Saves an open toolbar collection to disk
        Parameters:
          name (str): name of a currently open toolbar file
        Returns:
          bool: True or False indicating success or failure
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile: return tbfile.Save()
        return False
    *)


    [<EXT>]
    ///<summary>Saves an open toolbar collection to a different disk file</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="file">(string) Full path to file name to save to</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollectionAs(name:string, file:string) : bool =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.SaveAs(file)
        else false
    (*
    def SaveToolbarCollectionAs(name, file):
        '''Saves an open toolbar collection to a different disk file
        Parameters:
          name (str): name of a currently open toolbar file
          file (str): full path to file name to save to
        Returns:
          bool: True or False indicating success or failure
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile: return tbfile.SaveAs(file)
        return False
    *)


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
    (*
    def ShowToolbar(name, toolbar_group):
        '''Shows a previously hidden toolbar group in an open toolbar collection
        Parameters:
          name (str): name of a currently open toolbar file
          toolbar_group (str): name of a toolbar group to show
        Returns:
          bool: True or False indicating success or failure
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile:
            group = tbfile.GetGroup(toolbar_group)
            if group:
                group.Visible = True
                return True
        return False
    *)


    [<EXT>]
    ///<summary>Returns number of currently open toolbar collections</summary>
    ///<returns>(int) the number of currently open toolbar collections</returns>
    static member ToolbarCollectionCount() : int =
        RhinoApp.ToolbarFiles.Count
    (*
    def ToolbarCollectionCount():
        '''Returns number of currently open toolbar collections
        Returns:
          number: the number of currently open toolbar collections
        '''
        return Rhino.RhinoApp.ToolbarFiles.Count
    *)


    [<EXT>]
    ///<summary>Returns names of all currently open toolbar collections</summary>
    ///<returns>(string array) the names of all currently open toolbar collections</returns>
    static member ToolbarCollectionNames() : string array =
        [| for tbfile in RhinoApp.ToolbarFiles -> tbfile.Name |]
    (*
    def ToolbarCollectionNames():
        '''Returns names of all currently open toolbar collections
        Returns:
          list(str, ...): the names of all currently open toolbar collections
        '''
        return [tbfile.Name for tbfile in Rhino.RhinoApp.ToolbarFiles]
    *)


    [<EXT>]
    ///<summary>Returns full path to a currently open toolbar collection file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<returns>(string) full path on success</returns>
    static member ToolbarCollectionPath(name:string) : string =
        let tbfile = RhinoApp.ToolbarFiles.FindByName(name, true)
        if notNull tbfile then  tbfile.Path
        else ""
    (*
    def ToolbarCollectionPath(name):
        '''Returns full path to a currently open toolbar collection file
        Parameters:
          name (str): name of currently open toolbar collection
        Returns:
          str: full path on success
          None: on error
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile: return tbfile.Path
    *)


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
    (*
    def ToolbarCount(name, groups=False):
        '''Returns the number of toolbars or groups in a currently open toolbar file
        Parameters:
          name (str): name of currently open toolbar collection
          groups (bool, optional): If true, return the number of toolbar groups in the file
        Returns:
          number: number of toolbars on success
          None: on error
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile:
            if groups: return tbfile.GroupCount
            return tbfile.ToolbarCount
    *)


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
                for i=0 to tbfile.GroupCount-1 do rc.Add(tbfile.GetGroup(i).Name)
            else
                for i=0 to tbfile.ToolbarCount-1 do rc.Add(tbfile.GetToolbar(i).Name)
        rc
    (*
    def ToolbarNames(name, groups=False):
        '''Returns the names of all toolbars (or toolbar groups) found in a
        currently open toolbar file
        Parameters:
          name (str): name of currently open toolbar collection
          groups (bool, optional): If true, return the names of toolbar groups in the file
        Returns:
          list(str, ...): names of all toolbars (or toolbar groups) on success
          None: on error
        '''
        tbfile = Rhino.RhinoApp.ToolbarFiles.FindByName(name, True)
        if tbfile:
            rc = []
            if groups:
                for i in range(tbfile.GroupCount): rc.append(tbfile.GetGroup(i).Name)
            else:
                for i in range(tbfile.ToolbarCount): rc.append(tbfile.GetToolbar(i).Name)
            return rc;
    *)


