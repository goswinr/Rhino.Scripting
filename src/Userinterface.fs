namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
open Rhino
open Rhino.UI
open System.Drawing
//open System.Windows.Forms

[<AutoOpen>]
module ExtensionsUserinterface =
  
  [<EXT>] 
  type RhinoScriptSyntax with
    


    [<EXT>]
    ///<summary>Display browse-for-folder dialog allowing the user to select a folder</summary>
    ///<param name="folder">(string) Optional, Default Value: <c>null:string</c>
    ///A default folder</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message</param>   
    ///<returns>(string option) selected folder option or None if selection was canceled</returns>
    static member BrowseForFolder([<OPT;DEF(null:string)>]folder:string, [<OPT;DEF(null:string)>]message:string) : string option =
        async{
            do! Async.SwitchToContext System.Windows.Forms.WindowsFormsSynchronizationContext.Current         
            use dlg = new System.Windows.Forms.FolderBrowserDialog()
            dlg.ShowNewFolderButton <- true
            if notNull folder then
                if IO.Directory.Exists(folder) then
                    dlg.SelectedPath <-  folder
            if notNull message then
                dlg.Description <- message
            if dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK then 
                return Some(dlg.SelectedPath)
            else
                return None
        } 
        |> Async.RunSynchronously

        //let dlg = Eto.Forms.SelectFolderDialog()
        //if notNull folder then
        //    if not <| isinstance(folder, str) then folder <- string(folder)
        //    let dlg.Directory = folder
        //if notNull message then
        //    if not <| isinstance(message, str) then message <- string(message)
        //    let dlg.Title = message
        //if dlg.ShowDialog(null) = Eto.Forms.DialogResult.Ok then
        //    dlg.Directory


    (*
    def BrowseForFolder(folder=None, message=None, title=None):
        '''Display browse-for-folder dialog allowing the user to select a folder
        Parameters:
          folder (str, optional): a default folder
          message (str, optional): a prompt or message
          title (str, optional): a dialog box title
        Returns:
          str: selected folder
          None: on error
        '''
    
        dlg = Eto.Forms.SelectFolderDialog()
        if folder:
            if not isinstance(folder, str): folder = str(folder)
            dlg.Directory = folder
        if message:
            if not isinstance(message, str): message = str(message)
            dlg.Title = message
        if dlg.ShowDialog(None)==Eto.Forms.DialogResult.Ok:
            return dlg.Directory
    *)


    [<EXT>]
    ///<summary>Displays a list of items in a checkable-style list dialog box</summary>
    ///<param name="items">((string*bool) seq) A list of tuples containing a string and a boolean check state</param>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///A prompt or message</param>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///A dialog box title</param>
    ///<returns>((string*bool) [] option) Option of tuples containing the input string in items along with their new boolean check value</returns>
    static member CheckListBox(items:(string*bool) seq, [<OPT;DEF(null:string)>]message:string, [<OPT;DEF(null:string)>]title:string) : (string*bool) [] option=
        let checkstates = [| for item in items -> snd item |]
        let itemstrs = [|for item in items -> fst item|]
        let 
        
        RhinoApp.InvokeOnUiThread
        
        
        let newcheckstates =
            async{
                //let currentContext = System.Threading.SynchronizationContext.Current 
                //do! Async.SwitchToContext(guiContext)
                do! Async.SwitchToContext System.Threading.SynchronizationContext.Current  
                let res = Rhino.UI.Dialogs.ShowCheckListBox(title, message, itemstrs, checkstates)
                //do! Async.SwitchToContext(currentContext) 
                return res
                } 
                |> Async.RunSynchronously

        if notNull newcheckstates then
            Some (Array.zip itemstrs newcheckstates)
        else
            //failwithf "Rhino.Scripting: CheckListBox failed.  items:'%A' message:'%A' title:'%A'" items message title
            None
    (*
    def CheckListBox(items, message=None, title=None):
        '''Displays a list of items in a checkable-style list dialog box
        Parameters:
          items ([[str, bool], ...]): a list of tuples containing a string and a boolean check state
          message (str, optional):  a prompt or message
          title (str, optional):  a dialog box title
        Returns:
          list((str, bool), ...): of tuples containing the input string in items along with their new boolean check value
          None: on error
        '''
    
        checkstates = [item[1] for item in items]
        itemstrs = [str(item[0]) for item in items]
        newcheckstates = Rhino.UI.Dialogs.ShowCheckListBox(title, message, itemstrs, checkstates)
        if newcheckstates:
            rc = zip(itemstrs, newcheckstates)
            return rc
        return scriptcontext.errorhandler()
    *)

