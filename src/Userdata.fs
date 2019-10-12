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
    ///<param name="section">(string) Optional, Default Value: <c>null:string</c>
    ///Section name. If omitted, all sections and their corresponding
    ///  entries are removed</param>
    ///<param name="entry">(string) Optional, Default Value: <c>null:string</c>
    ///Entry name. If omitted, all entries for section are removed</param>
    ///<returns>(unit) void, nothing</returns>
    static member DeleteDocumentData([<OPT;DEF(null:string)>]section:string, [<OPT;DEF(null:string)>]entry:string) : unit =
        Doc.Strings.Delete(section, entry)
    (*
    def DeleteDocumentData(section=None, entry=None):
        '''Removes user data strings from the current document
        Parameters:
          section (str, optional): section name. If omitted, all sections and their corresponding
            entries are removed
          entry (str, optional): entry name. If omitted, all entries for section are removed
        Returns:
          bool: True or False indicating success or failure
        '''
        return Doc.Strings.Delete(section, entry)
    *)


    [<EXT>]
    ///<summary>Returns the number of user data strings in the current document</summary>
    ///<returns>(int) the number of user data strings in the current document</returns>
    static member DocumentDataCount() : int =
        Doc.Strings.DocumentDataCount 


    //<summary>Returns the number of user text strings in the current document</summary>
    //<returns>(float) The number of user text strings in the current document</returns>
    //let documentUserTextCount () :int = Doc.Strings.Count //DocumentUserTextCount //TODO same as Data count? only in Rhino 6?
    (*
    def DocumentDataCount():
        '''Returns the number of user data strings in the current document
        Returns:
          number: the number of user data strings in the current document
        '''
        return Doc.Strings.DocumentDataCount
    *)


    [<EXT>]
    ///<summary>Returns the number of user text strings in the current document</summary>
    ///<returns>(int) the number of user text strings in the current document</returns>
    static member DocumentUserTextCount() : int =
        Doc.Strings.DocumentUserTextCount
    (*
    def DocumentUserTextCount():
        '''Returns the number of user text strings in the current document
        Returns:
          number: the number of user text strings in the current document
        '''
        return Doc.Strings.DocumentUserTextCount
    *)


    [<EXT>]
    ///<summary>Returns a user data item from the current document</summary>
    ///<param name="section">(string) Optional, Default Value: <c>null:string</c>
    ///Section name. If omitted, all section names are returned</param
    ///<returns>(string array) of all section names if section name is omitted
    ///  list(str, ...) of all entry names for a section if entry is omitted</returns>
    static member GetDocumentData([<OPT;DEF(null:string)>]section:string) : string [] =
        if notNull section then
            Doc.Strings.GetSectionNames()        
        else 
            Doc.Strings.GetEntryNames(section)
    
    [<EXT>]
    ///<summary>Returns a user data item  entry from the current document</summary>
    ///<param name="section">(string) Section name.</param>
    ///<param name="entry">(string) Entry name.</param>
    ///<returns>(string) the entry value</returns>
    static member GetDocumentDataEntry(section:string, entry:string) : string =
        Doc.Strings.GetValue(section, entry)
    
    (*
    def GetDocumentData(section=None, entry=None):
        '''Returns a user data item from the current document
        Parameters:
          section (str, optional): section name. If omitted, all section names are returned
          entry (str, optional): entry name. If omitted, all entry names for section are returned
        Returns:
          list(str, ...): of all section names if section name is omitted
          list(str, ...) of all entry names for a section if entry is omitted
          str: value of the entry if both section and entry are specified
          None: if not successful
        '''
        if section is None:
            rc = Doc.Strings.GetSectionNames()
            return list(rc) if rc else None
        if entry is None:
            rc = Doc.Strings.GetEntryNames(section)
            return list(rc) if rc else None
        val = Doc.Strings.GetValue(section, entry)
        return val if val else None
    *)


    [<EXT>]
    ///<summary>Returns user text stored in the document</summary>
    ///<param name="key">(string) Key to use for retrieving user text.</param>
    ///<returns>(string) If key is specified, then the associated value .</returns>
    static member GetDocumentUserText(key:string) : string =
        Doc.Strings.GetValue(key)

    [<EXT>]
    ///<summary>Returns all document user text keys</summary>
    ///<returns>(string array) all document user text keys </returns>
    static member GetDocumentUserTextKeys() : string array =
        [| for i=0 to Doc.Strings.Count-1  do 
              let k = Doc.Strings.GetKey(i) 
              if not <| k.Contains "\\" then  yield k |]

        
    (*
    def GetDocumentUserText(key=None):
        '''Returns user text stored in the document
        Parameters:
          key (str, optional): key to use for retrieving user text. If empty, all keys are returned
        Returns:
          str: If key is specified, then the associated value if successful.
          list(str, ...):If key is not specified, then a list of key names if successful.
          None: If not successful, or on error.
        '''
        if key: 
          val =  Doc.Strings.GetValue(key)
          return val if val else None
        #todo: leaky abstraction: "\\" logic should be inside doc.Strings implementation
        keys = [Doc.Strings.GetKey(i) for i in range(Doc.Strings.Count) if not "\\" in Doc.Strings.GetKey(i)]
        return keys if keys else None
    *)

    [<EXT>]
    ///<summary>Returns all user text keys stored on an object.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to retrieve the user text</param>
    ///<returns>(string array) all keys</returns>
    static member GetUserTextKeys(objectId:Guid, [<OPT;DEF(false)>]attachedToGeometry:bool) : string array =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachedToGeometry then
            let uss = obj.Geometry.GetUserStrings()
            [| for i=0 to uss.Count-1 do yield uss.GetKey(i)|]  
        else
            let uss = obj.Attributes.GetUserStrings()
            [| for i=0 to uss.Count-1 do yield uss.GetKey(i)|]
            
    
    [<EXT>]    
    ///<summary>Returns user text stored on an object.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">The key name.</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to retrieve the user text</param>
    ///<returns>(string) if key is specified, the associated value</returns>
    static member GetUserText(objectId:Guid, key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachedToGeometry then
            obj.Geometry.GetUserString(key)
        else
            obj.Attributes.GetUserString(key)
    (*
    def GetUserText(object_id, key=None, attached_to_geometry=False):
        '''Returns user text stored on an object.
        Parameters:
          object_id (guid): the object's identifies
          key (str, optional): the key name. If omitted all key names for an object are returned
          attached_to_geometry (bool, optional): location on the object to retrieve the user text
        Returns:
          str: if key is specified, the associated value if successful
          list(str, ...): if key is not specified, a list of key names if successful
        '''
        obj = rhutil.coercerhinoobject(object_id, True, True)
        source = None
        if attached_to_geometry: source = obj.Geometry
        else: source = obj.Attributes
        rc = None
        if key: return source.GetUserString(key)
        userstrings = source.GetUserStrings()
        return [userstrings.GetKey(i) for i in range(userstrings.Count)]
    *)


    [<EXT>]
    ///<summary>Verifies the current document contains user data</summary>
    ///<returns>(bool) True or False indicating the presence of Script user data</returns>
    static member IsDocumentData() : bool =
        Doc.Strings.Count > 0 //DocumentDataCount > 0
    (*
    def IsDocumentData():
        '''Verifies the current document contains user data
        Returns:
          bool: True or False indicating the presence of Script user data
        '''
        return Doc.Strings.DocumentDataCount > 0
    *)


    [<EXT>]
    ///<summary>Verifies the current document contains user text</summary>
    ///<returns>(bool) True or False indicating the presence of Script user text</returns>
    static member IsDocumentUserText() : bool =
        Doc.Strings.Count > 0 //.DocumentUserTextCount > 0
    (*
    def IsDocumentUserText():
        '''Verifies the current document contains user text
        Returns:
          bool: True or False indicating the presence of Script user text
        '''
        return Doc.Strings.DocumentUserTextCount > 0
    *)


    [<EXT>]
    ///<summary>Verifies that an object contains user text</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(float) result of test:
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
    (*
    def IsUserText(object_id):
        '''Verifies that an object contains user text
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          number: result of test:
            0 = no user text
            1 = attribute user text
            2 = geometry user text
            3 = both attribute and geometry user text
        '''
        obj = rhutil.coercerhinoobject(object_id, True, True)
        rc = 0
        if obj.Attributes.UserStringCount: rc = rc|1
        if obj.Geometry.UserStringCount: rc = rc|2
        return rc
    *)


    [<EXT>]
    ///<summary>Adds or sets a user data string to the current document</summary>
    ///<param name="section">(string) The section name</param>
    ///<param name="entry">(string) The entry name</param>
    ///<param name="value">(string) The string value</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetDocumentData(section:string, entry:string, value:string) : unit =
        Doc.Strings.SetString(section, entry, value) |> ignore
    (*
    def SetDocumentData(section, entry, value):
        '''Adds or sets a user data string to the current document
        Parameters:
          section (str): the section name
          entry (str): the entry name
          value (str): the string value
        Returns:
          str: The previous value
        '''
        val = Doc.Strings.SetString(section, entry, value)
        return val if val else None
    *)


    [<EXT>]
    ///<summary>Sets or removes user text stored in the document</summary>
    ///<param name="key">(string) Key name to set</param>
    ///<param name="value">(string) Optional, Default Value: <c>null:string</c>
    ///The string value to set. If omitted the key/value pair
    ///  specified by key will be deleted</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetDocumentUserText(key:string, [<OPT;DEF(null:string)>]value:string) : unit =
        Doc.Strings.SetString(key,value) |> ignore
    (*
    def SetDocumentUserText(key, value=None):
        '''Sets or removes user text stored in the document
        Parameters:
          key (str): key name to set
          value (str): The string value to set. If omitted the key/value pair
            specified by key will be deleted
        Returns:
          bool: True or False indicating success
        '''
        if value: Doc.Strings.SetString(key,value)
        else: Doc.Strings.Delete(key)
        return True
    *)


    [<EXT>]
    ///<summary>Sets or removes user text stored on an object.</summary>
    ///<param name="objectId">(string) The object's identifier</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) Optional, Default Value: <c>null:string</c>
    ///  The string value to set. If omitted, the key/value pair
    ///  specified by key will be deleted</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to store the user text</param>
    ///<returns>(unit) void, nothing</returns>
    static member SetUserText(objectId:string, key:string, [<OPT;DEF(null:string)>]value:string, [<OPT;DEF(false)>]attachToGeometry:bool) : unit =
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if attachToGeometry then
            obj.Geometry.SetUserString(key, value)|> ignore
        else
            obj.Attributes.SetUserString(key, value)|> ignore
        
    (*
    def SetUserText(object_id, key, value=None, attach_to_geometry=False):
        '''Sets or removes user text stored on an object.
        Parameters:
          object_id (str): the object's identifier
          key (str): the key name to set
          value (str, optional) the string value to set. If omitted, the key/value pair
              specified by key will be deleted
          attach_to_geometry (bool, optional): location on the object to store the user text
        Returns:
          bool: True or False indicating success or failure
        '''
        obj = rhutil.coercerhinoobject(object_id, True, True)
        if type(key) is not str: key = str(key)
        if value and type(value) is not str: value = str(value)
        if attach_to_geometry: return obj.Geometry.SetUserString(key, value)
        return obj.Attributes.SetUserString(key, value)
    *)


