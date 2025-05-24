namespace Rhino.Scripting

open Rhino
open System
open Rhino.Scripting.RhinoScriptingUtils


[<AutoOpen>]
module AutoOpenUserData =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>Removes user data strings from the current document.</summary>
    ///<param name="section">(string) Optional, Section name. If omitted, all sections and their corresponding entries are removed</param>
    ///<param name="entry">(string) Optional, Entry name. If omitted, all entries for section are removed</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DeleteDocumentData([<OPT;DEF(null:string)>]section:string, [<OPT;DEF(null:string)>]entry:string) : unit =
        State.Doc.Strings.Delete(section, entry) //TODO check null case


    ///<summary>Returns the number of user data strings in the current document.</summary>
    ///<returns>(int) The number of user data strings in the current document.</returns>
    static member DocumentDataCount() : int =
        State.Doc.Strings.DocumentDataCount


    ///<summary>Returns the number of user text strings in the current document.</summary>
    ///<returns>(int) The number of user text strings in the current document.</returns>
    static member DocumentUserTextCount() : int =
        State.Doc.Strings.DocumentUserTextCount


    ///<summary>Returns a user data item from the current document.</summary>
    ///<param name="sectionName">(string) Optional, Section name. If omitted, all section names are returned</param>
    ///<returns>(string array) Array of all section names if sectionName is omitted,
    /// else all entry names in this section.</returns>
    static member GetDocumentData([<OPT;DEF(null:string)>]sectionName:string) : array<string> =
        if notNull sectionName then
            State.Doc.Strings.GetSectionNames()
        else
            State.Doc.Strings.GetEntryNames(sectionName)

    ///<summary>Returns a user data item  entry from the current document.</summary>
    ///<param name="section">(string) Section name</param>
    ///<param name="entry">(string) Entry name</param>
    ///<returns>(string) The entry value.</returns>
    static member GetDocumentDataEntry(section:string, entry:string) : string =
        State.Doc.Strings.GetValue(section, entry)


    ///<summary>Returns user text stored in the document.</summary>
    ///<param name="key">(string) Key to use for retrieving user text</param>
    ///<returns>(string) If key is specified, then the associated value.</returns>
    static member GetDocumentUserText(key:string) : string =
        State.Doc.Strings.GetValue(key) //TODO add null checking

    ///<summary>Returns all document user text keys.</summary>
    ///<returns>(string ResizeArray) all document user text keys.</returns>
    static member GetDocumentUserTextKeys() : string ResizeArray =
        let r = ResizeArray(State.Doc.Strings.Count)
        for i = 0 to State.Doc.Strings.Count-1  do
            let k = State.Doc.Strings.GetKey(i)
            if not <| k.Contains "\\" then  // TODO why ??
                r.Add k
        r


    ///<summary>Returns all user text keys stored on an object.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="attachedToGeometry">(bool) Optional, default value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(string ResizeArray) all keys.</returns>
    static member GetUserTextKeys(objectId:Guid, [<OPT;DEF(false)>]attachedToGeometry:bool) : string ResizeArray =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let uss =
            if attachedToGeometry then
                obj.Geometry.GetUserStrings()
            else
                obj.Attributes.GetUserStrings()
        let r = ResizeArray(uss.Count)
        for i = 0 to uss.Count-1  do
            r.Add <| uss.GetKey(i)
        r


    ///<summary>Returns user text stored on an object, fails if non existing.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, default value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(string) if key is specified, the associated value,fails if non existing.</returns>
    static member GetUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let s =
            if attachedToGeometry then  obj.Geometry.GetUserString(key)
            else                        obj.Attributes.GetUserString(key)

        if isNull s then
            let err = Text.StringBuilder()
            let addLn (s:String) = err.AppendLine s |> ignore<Text.StringBuilder>
            let add (s:String) = err.Append s |> ignore<Text.StringBuilder>
            addLn <| sprintf "RhinoScriptSyntax.GetUserText key: '%s' does not exist on %s" key (Pretty.str objectId)
            let ks = RhinoScriptSyntax.GetUserTextKeys(objectId, attachedToGeometry=false)
            if ks.Count = 0 then
                addLn  "This Object does not have any UserText."
            else
                addLn  "Available keys on Object are:"
                for k in RhinoScriptSyntax.GetUserTextKeys(objectId, attachedToGeometry=false) do
                    add "    "
                    addLn k
            let gks = RhinoScriptSyntax.GetUserTextKeys(objectId, attachedToGeometry=true)
            if gks.Count > 0 then
                addLn "Available keys on Geometry:"
                for k in RhinoScriptSyntax.GetUserTextKeys(objectId, attachedToGeometry=true) do
                    add "    "
                    addLn k

            RhinoScriptingException.Raise "%s" (err.ToString())
        s

    ///<summary>Returns user text stored on an object, returns Option.None if non existing.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, default value: <c>false</c>
    ///    Location on the object to retrieve the user text</param>
    ///<returns>(string Option) if key is specified, Some(value) else None .</returns>
    static member TryGetUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string Option=
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let s =
            if attachedToGeometry then  obj.Geometry.GetUserString(key)
            else                        obj.Attributes.GetUserString(key)
        if isNull s then None
        else Some s

    ///<summary>Checks if a User Text key is stored on an object.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) The key name</param>
    ///<param name="attachedToGeometry">(bool) Optional, default value: <c>false</c> Location on the object to retrieve the user text</param>
    ///<returns>(bool) if key exist true.</returns>
    static member HasUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : bool =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachedToGeometry then
            notNull <| obj.Geometry.GetUserString(key)
        else
            notNull <| obj.Attributes.GetUserString(key)


    ///<summary>Checks if the current document contains user data.</summary>
    ///<returns>(bool) True or False indicating the presence of Script user data.</returns>
    static member IsDocumentData() : bool =
        State.Doc.Strings.Count > 0 //DocumentDataCount > 0


    ///<summary>Checks if the current document contains user text.</summary>
    ///<returns>(bool) True or False indicating the presence of Script user text.</returns>
    static member IsDocumentUserText() : bool =
        State.Doc.Strings.Count > 0 //.DocumentUserTextCount > 0


    ///<summary>Checks if an object contains user text.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(int) result of test:
    ///    0 = no user text
    ///    1 = attribute user text
    ///    2 = geometry user text
    ///    3 = both attribute and geometry user text.</returns>
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
    static member SetDocumentData(section:string, entry:string, value:string) : unit =
        // TODO verify input strings
        State.Doc.Strings.SetString(section, entry, value) |> ignore<string>


    ///<summary>Sets a user text stored in the document.</summary>
    ///<param name="key">(string) Key name to set. Cannot be empty string.</param>
    ///<param name="value">(string) The string value to set. Can be empty string. To delete a key use rs.DeleteDocumentUserText</param>
    ///<param name="allowAllUnicode">(bool) Optional, default value: <c>false</c> , set to true to allow all Unicode characters,
    ///     (even the ones that look like ASCII characters but are not ASCII) in the value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SetDocumentUserText(key:string, value:string, [<OPT;DEF(false)>]allowAllUnicode:bool) : unit =
        if not <|  RhinoScriptSyntax.IsGoodStringId( key, allowEmpty=false) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.SetDocumentUserText the string '%s' cannot be used as key. See RhinoScriptSyntax.IsGoodStringId. You may be able bypass this restrictions in Rhino.Scripting by using RhinoCommon directly." key

        if allowAllUnicode then
            if not <| Util.isAcceptableStringId( value, true) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.SetDocumentUserText the string '%s' cannot be used as value. See RhinoScriptSyntax.IsGoodStringId. You can use RhinoCommon to bypass some of these restrictions." value
        else
            if not <|  RhinoScriptSyntax.IsGoodStringId( value, allowEmpty=true) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.SetDocumentUserText the string '%s' cannot be used as value. You may be able bypass this restrictions by using the optional argument: allowAllUnicode=true" value
        State.Doc.Strings.SetString(key, value) |> ignore<string>


    ///<summary>Removes user text stored in the document.</summary>
    ///<param name="key">(string) Key name to delete</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DeleteDocumentUserText(key:string) : unit =
        if isNull key  then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteDocumentUserText failed on for null key"
        let p = State.Doc.Strings.SetString(key, null)
        if isNull p then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteDocumentUserText failed,  key '%s' does not exist"  key

    ///<summary>Sets a user text stored on an object. Key and value must noy contain ambiguous Unicode characters.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="key">(string) The key name to set. Cannot be an empty string.</param>
    ///<param name="value">(string) The string value to set. Can be an empty string. Use rs.DeleteUserText to delete keys</param>
    ///<param name="attachToGeometry">(bool) Optional, default value: <c>false</c> Location on the object to store the user text</param>
    ///<param name="allowAllUnicode">(bool) Optional, default value: <c>false</c> , set to true to allow all Unicode characters,
    ///     (even the ones that look like ASCII characters but are not ASCII) in the value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SetUserText(objectId:Guid, key:string, value:string, [<OPT;DEF(false)>]attachToGeometry:bool, [<OPT;DEF(false)>]allowAllUnicode:bool) : unit =
        if not <| RhinoScriptSyntax.IsGoodStringId( key, allowEmpty=false) then
            RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText the string '%s' cannot be used as key. See RhinoScriptSyntax.IsGoodStringId. You can use RhinoCommon to bypass some of these restrictions." key

        if allowAllUnicode then
            if not <| Util.isAcceptableStringId( value, true) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText the string '%s' cannot be used as value. See RhinoScriptSyntax.IsGoodStringId. You can use RhinoCommon to bypass some of these restrictions." value
        else
            if not <|  RhinoScriptSyntax.IsGoodStringId( value, allowEmpty=true) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText the string '%s' cannot be used as value. You may be able bypass this restrictions by using the optional argument: allowAllUnicode=true" value
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachToGeometry then
            if not <| obj.Geometry.SetUserString(key, value) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' value '%s'" (Pretty.str objectId) key value
        else
            if not <| obj.Attributes.SetUserString(key, value) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' value '%s'" (Pretty.str objectId) key value

        obj.CommitChanges() |> ignore<bool> // should not be needed but still do it because of this potential bug: https://mcneel.myjetbrains.com/youtrack/issue/RH-71536

    ///<summary>Sets or removes user text stored on multiple objects. Key and value must noy contain ambiguous Unicode characters.</summary>
    ///<param name="objectIds">(Guid seq) The object identifiers</param>
    ///<param name="key">(string) The key name to set. Cannot be an empty string.</param>
    ///<param name="value">(string) The string value to set. Can be an empty string. Use rs.DeleteUserText to delete keys</param>
    ///<param name="attachToGeometry">(bool) Optional, default value: <c>false</c> Location on the object to store the user text</param>
    ///<param name="allowAllUnicode">(bool) Optional, default value: <c>false</c> , set to true to allow Unicode characters,
    ///     (even the ones that look like ASCII characters but are not ASCII) in the value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SetUserText(objectIds:Guid seq, key:string, value:string, [<OPT;DEF(false)>]attachToGeometry:bool, [<OPT;DEF(false)>]allowAllUnicode:bool) : unit = //PLURAL
        if not <|  RhinoScriptSyntax.IsGoodStringId( key, allowEmpty=false) then
            RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText the string '%s' cannot be used as key. See RhinoScriptSyntax.IsGoodStringId. You can use RhinoCommon to bypass some of these restrictions." key

        if allowAllUnicode then
            if not <| Util.isAcceptableStringId( value, true) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText the string '%s' cannot be used as value. See RhinoScriptSyntax.IsGoodStringId. You can use RhinoCommon to bypass some of these restrictions." value
        else
            if not <|  RhinoScriptSyntax.IsGoodStringId( value, allowEmpty=true) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText the string '%s' cannot be used as value. You may be able bypass this restrictions by using the optional argument: allowAllUnicode=true" value

        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if attachToGeometry then
                if not <| obj.Geometry.SetUserString(key, value) then
                    RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' value '%s'" (Pretty.str objectId) key value
            else
                if not <| obj.Attributes.SetUserString(key, value) then
                    RhinoScriptingException.Raise "RhinoScriptSyntax.SetUserText failed on %s for key '%s' value '%s'" (Pretty.str objectId) key value
            obj.CommitChanges() |> ignore<bool>  // should not be needed but still do it because of this potential bug: https://mcneel.myjetbrains.com/youtrack/issue/RH-71536

    ///<summary>Removes user text stored on an object. If the key exists.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="key">(string) The key name to delete</param>
    ///<param name="attachToGeometry">(bool) Optional, default value: <c>false</c> Location on the object to delete the user text from</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DeleteUserText(objectId:Guid, key:string,  [<OPT;DEF(false)>]attachToGeometry:bool) : unit =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachToGeometry then obj.Geometry.SetUserString  (key, null) |> ignore<bool> // returns false if key does not exist yet, otherwise true
        else                     obj.Attributes.SetUserString(key, null) |> ignore<bool>
        obj.CommitChanges() |> ignore<bool>  // should not be needed but still do it because of this potential bug: https://mcneel.myjetbrains.com/youtrack/issue/RH-71536


    ///<summary>Removes user text stored on multiple objects.If the key exists.</summary>
    ///<param name="objectIds">(Guid seq) The object identifiers</param>
    ///<param name="key">(string) The key name to delete</param>
    ///<param name="attachToGeometry">(bool) Optional, default value: <c>false</c> Location on the object to delete the user text from</param>
    ///<returns>(unit) void, nothing.</returns>
    static member DeleteUserText(objectIds:Guid seq, key:string,  [<OPT;DEF(false)>]attachToGeometry:bool) : unit = //PLURAL
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            if attachToGeometry then  obj.Geometry.SetUserString  (key, null) |> ignore<bool> // returns false if key does not exist yet, otherwise true
            else                      obj.Attributes.SetUserString(key, null) |> ignore<bool>
            obj.CommitChanges() |> ignore<bool>  // should not be needed but still do it because of this potential bug: https://mcneel.myjetbrains.com/youtrack/issue/RH-71536


