namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsUserdata =
  type RhinoScriptSyntax with
    
    ///<summary>Removes user data strings from the current document</summary>
    ///<param name="section">(string) Optional, Default Value: <c>null</c>
    ///Section name. If omitted, all sections and their corresponding
    ///  entries are removed</param>
    ///<param name="entry">(string) Optional, Default Value: <c>null</c>
    ///Entry name. If omitted, all entries for section are removed</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteDocumentData([<OPT;DEF(null)>]section:string, [<OPT;DEF(null)>]entry:string) : bool =
        failNotImpl () 


    ///<summary>Returns the number of user data strings in the current document</summary>
    ///<returns>(int) the number of user data strings in the current document</returns>
    static member DocumentDataCount() : int =
        failNotImpl () 


    ///<summary>Returns the number of user text strings in the current document</summary>
    ///<returns>(int) the number of user text strings in the current document</returns>
    static member DocumentUserTextCount() : int =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Returns a user data item from the current document</summary>
    ///<param name="section">(string) Optional, Default Value: <c>null</c>
    ///Section name. If omitted, all section names are returned</param>
    ///<param name="entry">(string) Optional, Default Value: <c>null</c>
    ///Entry name. If omitted, all entry names for section are returned</param>
    ///<returns>(string seq) of all section names if section name is omitted
    ///  list(str, ...) of all entry names for a section if entry is omitted</returns>
    static member GetDocumentData([<OPT;DEF(null)>]section:string, [<OPT;DEF(null)>]entry:string) : string seq =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Returns user text stored in the document</summary>
    ///<param name="key">(string) Optional, Default Value: <c>null</c>
    ///Key to use for retrieving user text. If empty, all keys are returned</param>
    ///<returns>(string) If key is specified, then the associated value .</returns>
    static member GetDocumentUserText([<OPT;DEF(null)>]key:string) : string =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Returns user text stored on an object.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) Optional, Default Value: <c>null</c>
    ///The key name. If omitted all key names for an object are returned</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to retrieve the user text</param>
    ///<returns>(string) if key is specified, the associated value</returns>
    static member GetUserText(objectId:Guid, [<OPT;DEF(null)>]key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string =
        failNotImpl () 


    ///<summary>Verifies the current document contains user data</summary>
    ///<returns>(bool) True or False indicating the presence of Script user data</returns>
    static member IsDocumentData() : bool =
        failNotImpl () 


    ///<summary>Verifies the current document contains user text</summary>
    ///<returns>(bool) True or False indicating the presence of Script user text</returns>
    static member IsDocumentUserText() : bool =
        failNotImpl () 


    ///<summary>Verifies that an object contains user text</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(float) result of test:
    ///  0 = no user text
    ///  1 = attribute user text
    ///  2 = geometry user text
    ///  3 = both attribute and geometry user text</returns>
    static member IsUserText(objectId:Guid) : float =
        failNotImpl () 


    ///<summary>Adds or sets a user data string to the current document</summary>
    ///<param name="section">(string) The section name</param>
    ///<param name="entry">(string) The entry name</param>
    ///<param name="value">(string) The string value</param>
    ///<returns>(unit) unit</returns>
    static member SetDocumentData(section:string, entry:string, value:string) : unit =
        failNotImpl () 


    ///<summary>Sets or removes user text stored in the document</summary>
    ///<param name="key">(string) Key name to set</param>
    ///<param name="value">(string) Optional, Default Value: <c>null</c>
    ///The string value to set. If omitted the key/value pair
    ///  specified by key will be deleted</param>
    ///<returns>(bool) True or False indicating success</returns>
    static member SetDocumentUserText(key:string, [<OPT;DEF(null)>]value:string) : bool =
        failNotImpl () 


    ///<summary>Sets or removes user text stored on an object.</summary>
    ///<param name="objectId">(string) The object's identifier</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) Optional, Default Value: <c>null</c>
    ///The string value to set. If omitted, the key/value pair
    ///  specified by key will be deleted</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to store the user text</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SetUserText(objectId:string, key:string, [<OPT;DEF(null)>]value:string, [<OPT;DEF(false)>]attachToGeometry:bool) : bool =
        failNotImpl () 


