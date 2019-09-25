namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsUserdata =
  [<EXT>] 
  type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Removes user data strings from the current document</summary>
    ///<param name="section">(string) Optional, Default Value: <c>null:string</c>
    ///Section name. If omitted, all sections and their corresponding
    ///  entries are removed</param>
    ///<param name="entry">(string) Optional, Default Value: <c>null:string</c>
    ///Entry name. If omitted, all entries for section are removed</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteDocumentData([<OPT;DEF(null:string)>]section:string, [<OPT;DEF(null:string)>]entry:string) : bool =
        failNotImpl () // genreation temp disabled !!
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
        return scriptcontext.doc.Strings.Delete(section, entry)
    *)


    [<EXT>]
    ///<summary>Returns the number of user data strings in the current document</summary>
    ///<returns>(int) the number of user data strings in the current document</returns>
    static member DocumentDataCount() : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def DocumentDataCount():
        '''Returns the number of user data strings in the current document
        Returns:
          number: the number of user data strings in the current document
        '''
        return scriptcontext.doc.Strings.DocumentDataCount
    *)


    [<EXT>]
    ///<summary>Returns the number of user text strings in the current document</summary>
    ///<returns>(int) the number of user text strings in the current document</returns>
    static member DocumentUserTextCount() : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def DocumentUserTextCount():
        '''Returns the number of user text strings in the current document
        Returns:
          number: the number of user text strings in the current document
        '''
        return scriptcontext.doc.Strings.DocumentUserTextCount
    *)


    //(FIXME) VarOutTypes
    [<EXT>]
    ///<summary>Returns a user data item from the current document</summary>
    ///<param name="section">(string) Optional, Default Value: <c>null:string</c>
    ///Section name. If omitted, all section names are returned</param>
    ///<param name="entry">(string) Optional, Default Value: <c>null:string</c>
    ///Entry name. If omitted, all entry names for section are returned</param>
    ///<returns>(string seq) of all section names if section name is omitted
    ///  list(str, ...) of all entry names for a section if entry is omitted</returns>
    static member GetDocumentData([<OPT;DEF(null:string)>]section:string, [<OPT;DEF(null:string)>]entry:string) : string seq =
        failNotImpl () // genreation temp disabled !!
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
            rc = scriptcontext.doc.Strings.GetSectionNames()
            return list(rc) if rc else None
        if entry is None:
            rc = scriptcontext.doc.Strings.GetEntryNames(section)
            return list(rc) if rc else None
        val = scriptcontext.doc.Strings.GetValue(section, entry)
        return val if val else None
    *)


    //(FIXME) VarOutTypes
    [<EXT>]
    ///<summary>Returns user text stored in the document</summary>
    ///<param name="key">(string) Optional, Default Value: <c>null:string</c>
    ///Key to use for retrieving user text. If empty, all keys are returned</param>
    ///<returns>(string) If key is specified, then the associated value .</returns>
    static member GetDocumentUserText([<OPT;DEF(null:string)>]key:string) : string =
        failNotImpl () // genreation temp disabled !!
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
          val =  scriptcontext.doc.Strings.GetValue(key)
          return val if val else None
        #todo: leaky abstraction: "\\" logic should be inside doc.Strings implementation
        keys = [scriptcontext.doc.Strings.GetKey(i) for i in range(scriptcontext.doc.Strings.Count) if not "\\" in scriptcontext.doc.Strings.GetKey(i)]
        return keys if keys else None
    *)


    //(FIXME) VarOutTypes
    [<EXT>]
    ///<summary>Returns user text stored on an object.</summary>
    ///<param name="objectId">(Guid) The object's identifies</param>
    ///<param name="key">(string) Optional, Default Value: <c>null:string</c>
    ///The key name. If omitted all key names for an object are returned</param>
    ///<param name="attachedToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to retrieve the user text</param>
    ///<returns>(string) if key is specified, the associated value</returns>
    static member GetUserText(objectId:Guid, [<OPT;DEF(null:string)>]key:string, [<OPT;DEF(false)>]attachedToGeometry:bool) : string =
        failNotImpl () // genreation temp disabled !!
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
        failNotImpl () // genreation temp disabled !!
    (*
    def IsDocumentData():
        '''Verifies the current document contains user data
        Returns:
          bool: True or False indicating the presence of Script user data
        '''
        return scriptcontext.doc.Strings.DocumentDataCount > 0
    *)


    [<EXT>]
    ///<summary>Verifies the current document contains user text</summary>
    ///<returns>(bool) True or False indicating the presence of Script user text</returns>
    static member IsDocumentUserText() : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsDocumentUserText():
        '''Verifies the current document contains user text
        Returns:
          bool: True or False indicating the presence of Script user text
        '''
        return scriptcontext.doc.Strings.DocumentUserTextCount > 0
    *)


    [<EXT>]
    ///<summary>Verifies that an object contains user text</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(float) result of test:
    ///  0 = no user text
    ///  1 = attribute user text
    ///  2 = geometry user text
    ///  3 = both attribute and geometry user text</returns>
    static member IsUserText(objectId:Guid) : float =
        failNotImpl () // genreation temp disabled !!
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
        failNotImpl () // genreation temp disabled !!
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
        val = scriptcontext.doc.Strings.SetString(section, entry, value)
        return val if val else None
    *)


    [<EXT>]
    ///<summary>Sets or removes user text stored in the document</summary>
    ///<param name="key">(string) Key name to set</param>
    ///<param name="value">(string) Optional, Default Value: <c>null:string</c>
    ///The string value to set. If omitted the key/value pair
    ///  specified by key will be deleted</param>
    ///<returns>(bool) True or False indicating success</returns>
    static member SetDocumentUserText(key:string, [<OPT;DEF(null:string)>]value:string) : bool =
        failNotImpl () // genreation temp disabled !!
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
        if value: scriptcontext.doc.Strings.SetString(key,value)
        else: scriptcontext.doc.Strings.Delete(key)
        return True
    *)


    [<EXT>]
    ///<summary>Sets or removes user text stored on an object.</summary>
    ///<param name="objectId">(string) The object's identifier</param>
    ///<param name="key">(string) The key name to set</param>
    ///<param name="value">(string) Optional, Default Value: <c>null:string</c>
    ///The string value to set. If omitted, the key/value pair
    ///  specified by key will be deleted</param>
    ///<param name="attachToGeometry">(bool) Optional, Default Value: <c>false</c>
    ///Location on the object to store the user text</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SetUserText(objectId:string, key:string, [<OPT;DEF(null:string)>]value:string, [<OPT;DEF(false)>]attachToGeometry:bool) : bool =
        failNotImpl () // genreation temp disabled !!
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


