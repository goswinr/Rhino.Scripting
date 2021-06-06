namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore
 
[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namespace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsUserdata =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    ///<summary>Removes user data strings from the current document.</summary>
    ///<param name="section">(string) Optional, Section name. If omitted, all sections and their corresponding entries are removed</param>
    ///<param name="entry">(string) Optional, Entry name. If omitted, all entries for section are removed</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member DeleteDocumentData([<OPT;DEF(null:string)>]section:string, [<OPT;DEF(null:string)>]entry:string) : unit =
        State.Doc.Strings.Delete(section, entry) //TODO check null case


    ///<summary>Returns the number of user data strings in the current document.</summary>
    ///<returns>(int) The number of user data strings in the current document.</returns>
    [<Extension>]
    static member DocumentDataCount() : int =
        State.Doc.Strings.DocumentDataCount

  
    ///<summary>Returns the number of user text strings in the current document.</summary>
    ///<returns>(int) The number of user text strings in the current document.</returns>
    [<Extension>]
    static member DocumentUserTextCount() : int =
        State.Doc.Strings.DocumentUserTextCount


    ///<summary>Returns a user data item from the current document.</summary>
    ///<param name="section">(string) Optional, Section name. If omitted, all section names are returned</param>
    ///<returns>(string array) Array of all section names if section name is omitted, 
    /// else all entry names in this  section.</returns>
    [<Extension>]
    static member GetDocumentData([<OPT;DEF(null:string)>]section:string) : array<string> =
        if notNull section then
            State.Doc.Strings.GetSectionNames()
        else
            State.Doc.Strings.GetEntryNames(section)

    ///<summary>Returns a user data item  entry from the current document.</summary>
    ///<param name="section">(string) Section name</param>
    ///<param name="entry">(string) Entry name</param>
    ///<returns>(string) The entry value.</returns>
    [<Extension>]
    static member GetDocumentDataEntry(section:string, entry:string) : string =
        State.Doc.Strings.GetValue(section, entry)


    ///<summary>Returns user text stored in the document.</summary>
    ///<param name="key">(string) Key to use for retrieving user text</param>
    ///<returns>(string) If key is specified, then the associated value.</returns>
    [<Extension>]
    static member GetDocumentUserText(key:string) : string =
        State.Doc.Strings.GetValue(key) //TODO add null checking

    ///<summary>Returns all document user text keys.</summary>
    ///<returns>(string Rarr) all document user text keys.</returns>
    [<Extension>]
    static member GetDocumentUserTextKeys() : string Rarr =
        rarr { for i = 0 to State.Doc.Strings.Count-1  do
                    let k = State.Doc.Strings.GetKey(i)
                    if not <| k.Contains "\\" then  // TODO why ??
                        yield k }


    ///<summary>Returns all user text keys stored on an object.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(string Rarr) all keys.</returns>
    [<Extension>]
    static member GetUserTextKeys(objectId:Guid, [<OPT;DEF(false)>]attachedToGeometry:bool) : string Rarr =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachedToGeometry then
            let uss = obj.Geometry.GetUserStrings()
            rarr { for  i = 0 to uss.Count-1 do yield uss.GetKey(i)}
        else
            let uss = obj.Attributes.GetUserStrings()
            rarr { for  i = 0 to uss.Count-1 do yield uss.GetKey(i)}


    ///<summary>Returns user text stored on an object, fails if non existing.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(string) if key is specified, the associated value,fails if non existing.</returns>
    [<Extension>]
    static member GetUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let s = 
            if attachedToGeometry then
                obj.Geometry.GetUserString(key)
            else
                obj.Attributes.GetUserString(key)
        
        if isNull s then             
            let err = 
                stringBuffer{
                yield! sprintf "RhinoScriptSyntax.GetUserText key: '%s' does not exist on %s" key (Print.guid objectId)
                let ks = RhinoScriptSyntax.GetUserTextKeys(objectId, attachedToGeometry=false)
                if ks.Count = 0 then 
                    yield!  "This Object does not have any UserText."
                else                
                    yield!  "Available keys on Object are:"
                    for k in RhinoScriptSyntax.GetUserTextKeys(objectId, attachedToGeometry=false) do 
                        yield "    "
                        yield! k
                let gks = RhinoScriptSyntax.GetUserTextKeys(objectId, attachedToGeometry=true)
                if gks.Count > 0 then 
                    yield! "Available keys on Geometry:"
                    for k in RhinoScriptSyntax.GetUserTextKeys(objectId, attachedToGeometry=true) do 
                        yield "    "
                        yield! k
            }
            RhinoScriptingException.Raise "%s" err
        s
    
    ///<summary>Returns user text stored on an object, returns Option.None if non existing.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(string Option) if key is specified, Some(value) else None .</returns>
    [<Extension>]
    static member TryGetUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string Option=
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let s = 
            if attachedToGeometry then
                obj.Geometry.GetUserString(key)
            else
                obj.Attributes.GetUserString(key)
        if isNull s then None
        else Some s

    ///<summary>Checks if a User Text key is stored on an object.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(bool) if key exist true.</returns>
    [<Extension>]
    static member HasUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : bool =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachedToGeometry then
            notNull <| obj.Geometry.GetUserString(key)
        else
            notNull <| obj.Attributes.GetUserString(key)


    ///<summary>Verifies the current document contains user data.</summary>
    ///<returns>(bool) True or False indicating the presence of Script user data.</returns>
    [<Extension>]
    static member IsDocumentData() : bool =
        State.Doc.Strings.Count > 0 //DocumentDataCount > 0


    ///<summary>Verifies the current document contains user text.</summary>
    ///<returns>(bool) True or False indicating the presence of Script user text.</returns>
    [<Extension>]
    static member IsDocumentUserText() : bool =
        State.Doc.Strings.Count > 0 //.DocumentUserTextCount > 0


    ///<summary>Verifies that an object contains user text.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(int) result of test:
    ///    0 = no user text
    ///    1 = attribute user text
    ///    2 = geometry user text
    ///    3 = both attribute and geometry user text.</returns>
    [<Extension>]
    static member IsUserText(objectId:Guid) : int =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let mutable rc = 0
        if obj.Attributes.UserStringCount > 0 then  rc <- rc ||| 1
        if obj.Geometry.UserStringCount > 0   then  rc <- rc ||| 2
        rc


    ///<summary>Adds or sets a user data string to the current document.</summary>
    ///<param name="section">(string) The section name</param>
    ///<param name="entry">(string) The entry name</param>
    ///<param name="value">(string) The string value</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member SetDocumentData(section:string, entry:string, value:string) : unit =
        State.Doc.Strings.SetString(section, entry, value) |> ignoreObj


    ///<summary>Sets a user text stored in the document.</summary>
    ///<param name="key">(string) Key name to set</param>
    ///<param name="value">(string) The string value to set. Cannot be empty string. Use rs.DeleteDocumentUserText to delete keys</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member SetDocumentUserText(key:string, value:string) : unit =
        if isNull key || isNull value then RhinoScriptingException.Raise "RhinoScriptSyntax.SetDocumentUserText failed on for null key and/or null value" 
        if value = "" then RhinoScriptingException.Raise "RhinoScriptSyntax.SetDocumentUserText failed on for key '%s' and value \"\" (empty string)"  key 
        State.Doc.Strings.SetString(key, value) |> ignoreObj
        

    ///<summary>Removes user text stored in the document.</summary>
    ///<param name="key">(string) Key name to delete</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member DeleteDocumentUserText(key:string) : unit =
        if isNull key  then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteDocumentUserText failed on for null key" 
        let p = State.Doc.Strings.SetString(key, null) 
        if isNull p then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteDocumentUserText failed,  key '%s' does not exist"  key    

    ///<summary>Sets a user text stored on an object.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) The string value to set. Cannot be empty string. use rs.DeleteUserText to delete keys</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to store the user text</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member SetUserText(objectId:Guid, key:string, value:string, [<OPT;DEF(false)>]attachToGeometry:bool) : unit =
        //TODO add null check for key and value
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if value = "" then RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' and value \"\" (empty string)" (Print.guid objectId) key 
        if attachToGeometry then
            if not <| obj.Geometry.SetUserString(key, value) then RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' value '%s'" (Print.guid objectId) key value
        else
            if not <| obj.Attributes.SetUserString(key, value) then RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' value '%s'" (Print.guid objectId) key value

    ///<summary>Sets or removes user text stored on multiple objects.</summary>
    ///<param name="objectIds">(Guid seq) The object identifiers</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) The string value to set. Cannot be empty string. use rs.DeleteUserText to delete keys</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to store the user text</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member SetUserText(objectIds:Guid seq, key:string, value:string, [<OPT;DEF(false)>]attachToGeometry:bool) : unit = //PLURAL
        //TODO add null check for key and value
        if value = "" then RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' and value \"\" (empty string)" (RhinoScriptSyntax.ToNiceString objectIds) key 
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if attachToGeometry then
                if not <| obj.Geometry.SetUserString(key, value) then RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' value '%s'" (Print.guid objectId) key value
            else
                if not <| obj.Attributes.SetUserString(key, value) then RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' value '%s'" (Print.guid objectId) key value


    ///<summary>Removes user text stored on an object. If the key exists.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="key">(string) The key name to delete</param>    
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to delte the user text from</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member DeleteUserText(objectId:Guid, key:string,  [<OPT;DEF(false)>]attachToGeometry:bool) : unit =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachToGeometry then obj.Geometry.SetUserString  (key, null) |> ignore // returns false if key does not exist yet, otherwise true
        else                     obj.Attributes.SetUserString(key, null) |> ignore
        

    ///<summary>Removes user text stored on multiple objects.If the key exists.</summary>
    ///<param name="objectIds">(Guid seq) The object identifiers</param>
    ///<param name="key">(string) The key name to delete</param>    
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///    Location on the object to delete the user text from</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member DeleteUserText(objectIds:Guid seq, key:string,  [<OPT;DEF(false)>]attachToGeometry:bool) : unit = //PLURAL        
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if attachToGeometry then  obj.Geometry.SetUserString  (key, null) |> ignore // returns false if key does not exist yet, otherwise true
            else                      obj.Attributes.SetUserString(key, null) |> ignore
