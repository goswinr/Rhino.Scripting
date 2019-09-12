namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

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
        failNotImpl () 


    ///<summary>Hides a previously visible toolbar group in an open toolbar collection</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group to hide</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member HideToolbar(name:string, toolbarGroup:string) : bool =
        failNotImpl () 


    ///<summary>Verifies a toolbar (or toolbar group) exists in an open collection file</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbar">(string) Name of a toolbar group</param>
    ///<param name="group">(bool) Optional, Default Value: <c>false</c>
    ///If toolbar parameter is referring to a toolbar group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbar(name:string, toolbar:string, [<OPT;DEF(false)>]group:bool) : bool =
        failNotImpl () 


    ///<summary>Verifies that a toolbar collection is open</summary>
    ///<param name="file">(string) Full path to a toolbar collection file</param>
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member IsToolbarCollection(file:string) : string =
        failNotImpl () 


    ///<summary>Verifies that a toolbar group in an open toolbar collection is visible</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbarDocked(name:string, toolbarGroup:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that a toolbar group in an open toolbar collection is visible</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsToolbarVisible(name:string, toolbarGroup:string) : bool =
        failNotImpl () 


    ///<summary>Opens a toolbar collection file</summary>
    ///<param name="file">(string) Full path to the collection file</param>
    ///<returns>(string) Rhino-assigned name of the toolbar collection</returns>
    static member OpenToolbarCollection(file:string) : string =
        failNotImpl () 


    ///<summary>Saves an open toolbar collection to disk</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollection(name:string) : bool =
        failNotImpl () 


    ///<summary>Saves an open toolbar collection to a different disk file</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="file">(string) Full path to file name to save to</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SaveToolbarCollectionAs(name:string, file:string) : bool =
        failNotImpl () 


    ///<summary>Shows a previously hidden toolbar group in an open toolbar collection</summary>
    ///<param name="name">(string) Name of a currently open toolbar file</param>
    ///<param name="toolbarGroup">(string) Name of a toolbar group to show</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ShowToolbar(name:string, toolbarGroup:string) : bool =
        failNotImpl () 


    ///<summary>Returns number of currently open toolbar collections</summary>
    ///<returns>(int) the number of currently open toolbar collections</returns>
    static member ToolbarCollectionCount() : int =
        failNotImpl () 


    ///<summary>Returns names of all currently open toolbar collections</summary>
    ///<returns>(string seq) the names of all currently open toolbar collections</returns>
    static member ToolbarCollectionNames() : string seq =
        failNotImpl () 


    ///<summary>Returns full path to a currently open toolbar collection file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<returns>(string) full path on success</returns>
    static member ToolbarCollectionPath(name:string) : string =
        failNotImpl () 


    ///<summary>Returns the number of toolbars or groups in a currently open toolbar file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<param name="groups">(bool) Optional, Default Value: <c>false</c>
    ///If true, return the number of toolbar groups in the file</param>
    ///<returns>(int) number of toolbars on success</returns>
    static member ToolbarCount(name:string, [<OPT;DEF(false)>]groups:bool) : int =
        failNotImpl () 


    ///<summary>Returns the names of all toolbars (or toolbar groups) found in a
    ///  currently open toolbar file</summary>
    ///<param name="name">(string) Name of currently open toolbar collection</param>
    ///<param name="groups">(bool) Optional, Default Value: <c>false</c>
    ///If true, return the names of toolbar groups in the file</param>
    ///<returns>(string seq) names of all toolbars (or toolbar groups) on success</returns>
    static member ToolbarNames(name:string, [<OPT;DEF(false)>]groups:bool) : string seq =
        failNotImpl () 


