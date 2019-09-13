namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
[<AutoOpen>]
module ExtensionsToolbar =
  type RhinoScriptSyntax with
    
    ///<summary>Closes a currently open toolbar collection</summary>
    ///<param name="name">(string) Name of a currently open toolbar collection</param>
    ///<param name="prompt">(bool) Optional, Default Value: <c>false</c>
    ///If True, user will be prompted to save the collection file
    ///  if it has been modified prior to closing</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CloseToolbarCollection(name:string, [<OPT;DEF(false)>]prompt:bool) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Hides a previously visible toolbar group in an open toolbar collection</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group to hide</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member HideToolbar(name:string, toolbarGroup:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Verifies a toolbar (or toolbar group) exists in an open collection file</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbar">(string) Name of a toolbar group</param>
    ///<param name="group">(bool) Optional, Default Value: <c>false</c>
    ///If toolbar parameter is referring to a toolbar group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbar(name:string, toolbar:string, [<OPT;DEF(false)>]group:bool) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Verifies that a toolbar collection is open</summary>
    ///<param name="file">(string) Full path to a toolbar collection file</param>
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member IsToolbarCollection(file:string) : string =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Verifies that a toolbar group in an open toolbar collection is visible</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbarDocked(name:string, toolbarGroup:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Verifies that a toolbar group in an open toolbar collection is visible</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbarVisible(name:string, toolbarGroup:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Opens a toolbar collection file</summary>
    ///<param name="file">(string) Full path to the collection file</param>
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member OpenToolbarCollection(file:string) : string =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Saves an open toolbar collection to disk</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollection(name:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Saves an open toolbar collection to a different disk file</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="file">(string) Full path to file name to save to</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollectionAs(name:string, file:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Shows a previously hidden toolbar group in an open toolbar collection</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group to show</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ShowToolbar(name:string, toolbarGroup:string) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns number of currently open toolbar collections</summary>
    ///<returns>(int) the number of currently open toolbar collections</returns>
    static member ToolbarCollectionCount() : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def ToolbarCollectionCount():
        '''Returns number of currently open toolbar collections
        Returns:
          number: the number of currently open toolbar collections
        '''
        return Rhino.RhinoApp.ToolbarFiles.Count
    *)


    ///<summary>Returns names of all currently open toolbar collections</summary>
    ///<returns>(string seq) the names of all currently open toolbar collections</returns>
    static member ToolbarCollectionNames() : string seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ToolbarCollectionNames():
        '''Returns names of all currently open toolbar collections
        Returns:
          list(str, ...): the names of all currently open toolbar collections
        '''
        return [tbfile.Name for tbfile in Rhino.RhinoApp.ToolbarFiles]
    *)


    ///<summary>Returns full path to a currently open toolbar collection file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<returns>(string) full path on success</returns>
    static member ToolbarCollectionPath(name:string) : string =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns the number of toolbars or groups in a currently open toolbar file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<param name="groups">(bool) Optional, Default Value: <c>false</c>
    ///If true, return the number of toolbar groups in the file</param>
    ///<returns>(int) number of toolbars on success</returns>
    static member ToolbarCount(name:string, [<OPT;DEF(false)>]groups:bool) : int =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns the names of all toolbars (or toolbar groups) found in a
    ///  currently open toolbar file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<param name="groups">(bool) Optional, Default Value: <c>false</c>
    ///If true, return the names of toolbar groups in the file</param>
    ///<returns>(string seq) names of all toolbars (or toolbar groups) on success</returns>
    static member ToolbarNames(name:string, [<OPT;DEF(false)>]groups:bool) : string seq =
        failNotImpl () // genreation temp disabled !!
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


