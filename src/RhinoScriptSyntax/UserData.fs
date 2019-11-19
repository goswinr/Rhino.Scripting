namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsUserdata =
    type RhinoScriptSyntax with

    [<EXT>]
    ///<summary>Removes user data strings from the current document</summary>
    ///<param name="section">(string) Optional, Section name. If omitted, all sections and their corresponding
    ///  entries are removed</param>
    ///<param name="entry">(string) Optional, Entry name. If omitted, all entries for section are removed</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteDocumentData([<OPT;DEF(null:string)>]section:string, [<OPT;DEF(null:string)>]entry:string) : unit =
        Doc.Strings.Delete(section, entry) //TODO check null case


    [<EXT>]
    ///<summary>Returns the number of user data strings in the current document</summary>
    ///<returns>(int) the number of user data strings in the current document</returns>
    static member DocumentDataCount() : int =
        Doc.Strings.DocumentDataCount

  
    [<EXT>]
    ///<summary>Returns the number of user text strings in the current document</summary>
    ///<returns>(int) the number of user text strings in the current document</returns>
    static member DocumentUserTextCount() : int =
        Doc.Strings.DocumentUserTextCount


    [<EXT>]
    ///<summary>Returns a user data item from the current document</summary>
    ///<param name="section">(string) Optional, Section name. If omitted, all section names are returned</param>
    ///<returns>(string array) of all section names if section name is omitted, 
    /// else all entry names in this  section</returns>
    static member GetDocumentData([<OPT;DEF(null:string)>]section:string) : array<string> =
        if notNull section then
            Doc.Strings.GetSectionNames()
        else
            Doc.Strings.GetEntryNames(section)

    [<EXT>]
    ///<summary>Returns a user data item  entry from the current document</summary>
    ///<param name="section">(string) Section name</param>
    ///<param name="entry">(string) Entry name</param>
    ///<returns>(string) the entry value</returns>
    static member GetDocumentDataEntry(section:string, entry:string) : string =
        Doc.Strings.GetValue(section, entry)



    [<EXT>]
    ///<summary>Returns user text stored in the document</summary>
    ///<param name="key">(string) Key to use for retrieving user text</param>
    ///<returns>(string) If key is specified, then the associated value </returns>
    static member GetDocumentUserText(key:string) : string =
        Doc.Strings.GetValue(key)

    [<EXT>]
    ///<summary>Returns all document user text keys</summary>
    ///<returns>(string ResizeArray) all document user text keys</returns>
    static member GetDocumentUserTextKeys() : string ResizeArray =
        resizeArray { for  i = 0 to Doc.Strings.Count-1  do
                          let k = Doc.Strings.GetKey(i)
                          if not <| k.Contains "\\" then  yield k }



    [<EXT>]
    ///<summary>Returns all user text keys stored on an object</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to retrieve the user text</param>
    ///<returns>(string ResizeArray) all keys</returns>
    static member GetUserTextKeys(objectId:Guid, [<OPT;DEF(false)>]attachedToGeometry:bool) : string ResizeArray =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachedToGeometry then
            let uss = obj.Geometry.GetUserStrings()
            resizeArray { for  i = 0 to uss.Count-1 do yield uss.GetKey(i)}
        else
            let uss = obj.Attributes.GetUserStrings()
            resizeArray { for  i = 0 to uss.Count-1 do yield uss.GetKey(i)}


    [<EXT>]
    ///<summary>Returns user text stored on an object</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to retrieve the user text</param>
    ///<returns>(string) if key is specified, the associated value</returns>
    static member GetUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachedToGeometry then
            obj.Geometry.GetUserString(key)
        else
            obj.Attributes.GetUserString(key)


    [<EXT>]
    ///<summary>Verifies the current document contains user data</summary>
    ///<returns>(bool) True or False indicating the presence of Script user data</returns>
    static member IsDocumentData() : bool =
        Doc.Strings.Count > 0 //DocumentDataCount > 0


    [<EXT>]
    ///<summary>Verifies the current document contains user text</summary>
    ///<returns>(bool) True or False indicating the presence of Script user text</returns>
    static member IsDocumentUserText() : bool =
        Doc.Strings.Count > 0 //.DocumentUserTextCount > 0


    [<EXT>]
    ///<summary>Verifies that an object contains user text</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(int) result of test:
    ///  0 = no user text
    ///  1 = attribute user text
    ///  2 = geometry user text
    ///  3 = both attribute and geometry user text</returns>
    static member IsUserText(objectId:Guid) : int =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let mutable rc = 0
        if obj.Attributes.UserStringCount > 0 then  rc <- rc ||| 1
        if obj.Geometry.UserStringCount > 0   then  rc <- rc ||| 2
        rc


    [<EXT>]
    ///<summary>Adds or sets a user data string to the current document</summary>
    ///<param name="section">(string) The section name</param>
    ///<param name="entry">(string) The entry name</param>
    ///<param name="value">(string) The string value</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetDocumentData(section:string, entry:string, value:string) : unit =
        Doc.Strings.SetString(section, entry, value) |> ignore


    [<EXT>]
    ///<summary>Sets or removes user text stored in the document</summary>
    ///<param name="key">(string) Key name to set</param>
    ///<param name="value">(string) Optional, The string value to set. If omitted the key/value pair
    ///  specified by key will be deleted</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetDocumentUserText(key:string, [<OPT;DEF(null:string)>]value:string) : unit =
        Doc.Strings.SetString(key, value) |> ignore
        //TODO check null case


    [<EXT>]
    ///<summary>Sets or removes user text stored on an object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) Optional, The string value to set. If omitted, the key/value pair
    ///  specified by key will be deleted</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to store the user text</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetUserText(objectId:Guid, key:string, [<OPT;DEF(null:string)>]value:string, [<OPT;DEF(false)>]attachToGeometry:bool) : unit =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachToGeometry then
            obj.Geometry.SetUserString(key, value)|> ignore
        else
            obj.Attributes.SetUserString(key, value)|> ignore



