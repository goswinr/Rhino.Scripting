namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.ActiceDocument
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore
 
[<AutoOpen>]
module ExtensionsUserdata =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    [<Extension>]
    ///<summary>Removes user data strings from the current document</summary>
    ///<param name="section">(string) Optional, Section name. If omitted, all sections and their corresponding
    ///    entries are removed</param>
    ///<param name="entry">(string) Optional, Entry name. If omitted, all entries for section are removed</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteDocumentData([<OPT;DEF(null:string)>]section:string, [<OPT;DEF(null:string)>]entry:string) : unit =
        Doc.Strings.Delete(section, entry) //TODO check null case


    [<Extension>]
    ///<summary>Returns the number of user data strings in the current document</summary>
    ///<returns>(int) the number of user data strings in the current document</returns>
    static member DocumentDataCount() : int =
        Doc.Strings.DocumentDataCount

  
    [<Extension>]
    ///<summary>Returns the number of user text strings in the current document</summary>
    ///<returns>(int) the number of user text strings in the current document</returns>
    static member DocumentUserTextCount() : int =
        Doc.Strings.DocumentUserTextCount


    [<Extension>]
    ///<summary>Returns a user data item from the current document</summary>
    ///<param name="section">(string) Optional, Section name. If omitted, all section names are returned</param>
    ///<returns>(string array) of all section names if section name is omitted, 
    /// else all entry names in this  section</returns>
    static member GetDocumentData([<OPT;DEF(null:string)>]section:string) : array<string> =
        if notNull section then
            Doc.Strings.GetSectionNames()
        else
            Doc.Strings.GetEntryNames(section)

    [<Extension>]
    ///<summary>Returns a user data item  entry from the current document</summary>
    ///<param name="section">(string) Section name</param>
    ///<param name="entry">(string) Entry name</param>
    ///<returns>(string) the entry value</returns>
    static member GetDocumentDataEntry(section:string, entry:string) : string =
        Doc.Strings.GetValue(section, entry)


    [<Extension>]
    ///<summary>Returns user text stored in the document</summary>
    ///<param name="key">(string) Key to use for retrieving user text</param>
    ///<returns>(string) If key is specified, then the associated value</returns>
    static member GetDocumentUserText(key:string) : string =
        Doc.Strings.GetValue(key) //TODO add null checking

    [<Extension>]
    ///<summary>Returns all document user text keys</summary>
    ///<returns>(string ResizeArray) all document user text keys</returns>
    static member GetDocumentUserTextKeys() : string ResizeArray =
        resizeArray { for  i = 0 to Doc.Strings.Count-1  do
                          let k = Doc.Strings.GetKey(i)
                          if not <| k.Contains "\\" then  yield k }


    [<Extension>]
    ///<summary>Returns all user text keys stored on an object</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(string ResizeArray) all keys</returns>
    static member GetUserTextKeys(objectId:Guid, [<OPT;DEF(false)>]attachedToGeometry:bool) : string ResizeArray =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachedToGeometry then
            let uss = obj.Geometry.GetUserStrings()
            resizeArray { for  i = 0 to uss.Count-1 do yield uss.GetKey(i)}
        else
            let uss = obj.Attributes.GetUserStrings()
            resizeArray { for  i = 0 to uss.Count-1 do yield uss.GetKey(i)}


    [<Extension>]
    ///<summary>Returns user text stored on an object, fails if non existing</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(string) if key is specified, the associated value,fails if non existing</returns>
    static member GetUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let s = 
            if attachedToGeometry then
                obj.Geometry.GetUserString(key)
            else
                obj.Attributes.GetUserString(key)
        
        if isNull s then 
            printfn "**Available keys on Object are:"
            RhinoScriptSyntax.GetUserTextKeys(objectId,false) |> Seq.iter (printfn "'%s'") 
            printfn "on Geometry:"
            RhinoScriptSyntax.GetUserTextKeys(objectId,true) |> Seq.iter (printfn "'%s'") 
            failwithf " GetUserText key: '%s' does not exist on %A: %A" key obj objectId
        s
    
    [<Extension>]
    ///<summary>Returns user text stored on an object, returns Option.None if non existing</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(string) if key is specified, the associated value,fails if non existing</returns>
    static member TryGetUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string Option=
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let s = 
            if attachedToGeometry then
                obj.Geometry.GetUserString(key)
            else
                obj.Attributes.GetUserString(key)
        if isNull s then None
        else Some s

    [<Extension>]
    ///<summary>Checks if a User Text key is stored on an object</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(bool) if key exist true</returns>
    static member HasUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : bool =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachedToGeometry then
            notNull <| obj.Geometry.GetUserString(key)
        else
            notNull <| obj.Attributes.GetUserString(key)


    [<Extension>]
    ///<summary>Verifies the current document contains user data</summary>
    ///<returns>(bool) True or False indicating the presence of Script user data</returns>
    static member IsDocumentData() : bool =
        Doc.Strings.Count > 0 //DocumentDataCount > 0


    [<Extension>]
    ///<summary>Verifies the current document contains user text</summary>
    ///<returns>(bool) True or False indicating the presence of Script user text</returns>
    static member IsDocumentUserText() : bool =
        Doc.Strings.Count > 0 //.DocumentUserTextCount > 0


    [<Extension>]
    ///<summary>Verifies that an object contains user text</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(int) result of test:
    ///    0 = no user text
    ///    1 = attribute user text
    ///    2 = geometry user text
    ///    3 = both attribute and geometry user text</returns>
    static member IsUserText(objectId:Guid) : int =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let mutable rc = 0
        if obj.Attributes.UserStringCount > 0 then  rc <- rc ||| 1
        if obj.Geometry.UserStringCount > 0   then  rc <- rc ||| 2
        rc


    [<Extension>]
    ///<summary>Adds or sets a user data string to the current document</summary>
    ///<param name="section">(string) The section name</param>
    ///<param name="entry">(string) The entry name</param>
    ///<param name="value">(string) The string value</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetDocumentData(section:string, entry:string, value:string) : unit =
        Doc.Strings.SetString(section, entry, value) |> ignoreObj


    [<Extension>]
    ///<summary>Sets a user text stored in the document</summary>
    ///<param name="key">(string) Key name to set</param>
    ///<param name="value">(string) The string value to set. Cannot be empty string. Use rs.DeleteDocumentUserText to delete keys</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetDocumentUserText(key:string, value:string) : unit =
        if isNull key || isNull value then failwithf "SetDocumentUserText failed on for null key and/or null value" 
        if value = "" then failwithf "SetDocumentUserText failed on for key '%s' and value \"\" (empty string)"  key 
        Doc.Strings.SetString(key, value) |> ignoreObj
        

    [<Extension>]
    ///<summary>Removes user text stored in the document</summary>
    ///<param name="key">(string) Key name to delete</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteDocumentUserText(key:string) : unit =
        if isNull key  then failwithf "DeleteDocumentUserText failed on for null key" 
        let p = Doc.Strings.SetString(key, null) 
        if isNull p then failwithf "DeleteDocumentUserText failed,  key '%s' does not exist"  key    

    [<Extension>]
    ///<summary>Sets a user text stored on an object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) The string value to set. Cannot be empty string. use rs.DeleteUserText to delete keys</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to store the user text</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetUserText(objectId:Guid, key:string, value:string, [<OPT;DEF(false)>]attachToGeometry:bool) : unit =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if value = "" then failwithf "SetUserText failed on %A for key '%s' and value \"\" (empty string)" objectId key 
        if attachToGeometry then
            if not <| obj.Geometry.SetUserString(key, value) then failwithf "SetUserText failed on %A for key '%s' value '%s'" objectId key value
        else
            if not <| obj.Attributes.SetUserString(key, value) then failwithf "SetUserText failed on %A for key '%s' value '%s'" objectId key value

    [<Extension>]
    ///<summary>Sets or removes user text stored on multiple objects</summary>
    ///<param name="objectIds">(Guid seq) The object identifiers</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) The string value to set. Cannot be empty string. use rs.DeleteUserText to delete keys</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to store the user text</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetUserText(objectIds:Guid seq, key:string, value:string, [<OPT;DEF(false)>]attachToGeometry:bool) : unit = //PLURAL
        if value = "" then failwithf "SetUserText failed on %A for key '%s' and value \"\" (empty string)" objectIds key 
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if attachToGeometry then
                if not <| obj.Geometry.SetUserString(key, value) then failwithf "SetUserText failed on %A for key '%s' value '%s'" objectId key value
            else
                if not <| obj.Attributes.SetUserString(key, value) then failwithf "SetUserText failed on %A for key '%s' value '%s'" objectId key value


    [<Extension>]
    ///<summary>Sets or removes user text stored on an object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) Optional, The string value to set. If omitted, the key/value pair
    ///    specified by key will be deleted</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to store the user text</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteUserText(objectId:Guid, key:string,  [<OPT;DEF(false)>]attachToGeometry:bool) : unit =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachToGeometry then
            if not <| obj.Geometry.SetUserString(key, null) then failwithf "DeleteUserText failed on %A for key '%s'" objectId key 
        else
            if not <| obj.Attributes.SetUserString(key, null) then failwithf "DeleteUserText failed on %A for key '%s'" objectId key 

    [<Extension>]
    ///<summary>Sets or removes user text stored on multiple objects</summary>
    ///<param name="objectIds">(Guid seq) The object identifiers</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) Optional, The string value to set. If omitted, the key/value pair
    ///    specified by key will be deleted</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to store the user text</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteUserText(objectIds:Guid seq, key:string,  [<OPT;DEF(false)>]attachToGeometry:bool) : unit = //PLURAL        
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if attachToGeometry then
                if not <| obj.Geometry.SetUserString(key, null) then failwithf "DeleteUserText failed on %A for key '%s'" objectId key 
            else
                if not <| obj.Attributes.SetUserString(key, null) then failwithf "DeleteUserText failed on %A for key '%s'" objectId key 