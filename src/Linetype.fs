namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsLinetype =

  [<EXT>] 
  type RhinoScriptSyntax with 

    [<EXT>]
    ///<summary>Verifies the existance of a linetype in the document</summary>
    ///<param name="name">(string) The name of an existing linetype.</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinetype(name:string) : bool =
        notNull <| Doc.Linetypes.FindName(name)        
    (*
    def IsLinetype(name_or_id):
        '''Verifies the existance of a linetype in the document
        Parameters:
          name_or_id (guid|str): The name or identifier of an existing linetype.
        Returns: 
          bool: True or False
        '''
        lt = __getlinetype(name_or_id)
        return lt is not None
    *)


    [<EXT>]
    ///<summary>Verifies that an existing linetype is from a reference file</summary>
    ///<param name="name">(string) The name of an existing linetype.</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinetypeReference(name:string) : bool =
        let lt = Doc.Linetypes.FindName(name)
        if isNull lt then failwithf "isLinetypeReference unable to find '%s' in a linetypes" name
        lt.IsReference
    (*
    def IsLinetypeReference(name_or_id):
        '''Verifies that an existing linetype is from a reference file
        Parameters:
          name_or_id (guid|str): The name or identifier of an existing linetype.
        Returns: 
          bool: True or False
        '''
        lt = __getlinetype(name_or_id)
        if lt is None: raise ValueError("unable to coerce %s into linetype"%name_or_id)
        return lt.IsReference
    *)


    [<EXT>]
    ///<summary>Returns number of linetypes in the document</summary>
    ///<returns>(int) the number of linetypes in the document</returns>
    static member LinetypeCount() : int =
        Doc.Linetypes.Count
    (*
    def LinetypeCount():
        '''Returns number of linetypes in the document
        Returns:
          number: the number of linetypes in the document
        '''
        return scriptcontext.doc.Linetypes.Count
    *)


    [<EXT>]
    ///<summary>Returns names of all linetypes in the document</summary>
    ///<returns>(string seq) list of linetype names</returns>
    static member LinetypeNames() : string ResizeArray =
        let count = Doc.Linetypes.Count
        let rc = ResizeArray()
        for i = 0 to count - 1 do
            let linetype = Doc.Linetypes.[i]
            if not linetype.IsDeleted then  rc.Add(linetype.Name)        
        rc
    (*
    def LinetypeNames(sort=False):
        '''Returns names of all linetypes in the document
        Parameters:
          sort (bool, optional): return a sorted list of the linetype names
        Returns:
          list(str, ...): list of linetype names if successful
        '''
        count = scriptcontext.doc.Linetypes.Count
        rc = []
        for i in xrange(count):
            linetype = scriptcontext.doc.Linetypes[i]
            if not linetype.IsDeleted: rc.append(linetype.Name)
        if sort: rc.sort()
        return rc
    *)


