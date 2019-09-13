namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
[<AutoOpen>]
module ExtensionsLinetype =
  type RhinoScriptSyntax with
    
    
    static member internal Getlinetype() : obj =
        failNotImpl () // genreation temp disabled !!
    (*
    def __getlinetype(name_or_id):
        ''''''
    *)


    ///<summary>Verifies the existance of a linetype in the document</summary>
    ///<param name="nameOrId">(Guid) The name or identifier of an existing linetype.</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinetype(nameOrId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Verifies that an existing linetype is from a reference file</summary>
    ///<param name="nameOrId">(Guid) The name or identifier of an existing linetype.</param>
    ///<returns>(bool) True or False</returns>
    static member IsLinetypeReference(nameOrId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
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


    ///<summary>Returns number of linetypes in the document</summary>
    ///<returns>(int) the number of linetypes in the document</returns>
    static member LinetypeCount() : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def LinetypeCount():
        '''Returns number of linetypes in the document
        Returns:
          number: the number of linetypes in the document
        '''
        return scriptcontext.doc.Linetypes.Count
    *)


    ///<summary>Returns names of all linetypes in the document</summary>
    ///<returns>(string seq) list of linetype names</returns>
    static member LinetypeNames() : string seq =
        failNotImpl () // genreation temp disabled !!
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


