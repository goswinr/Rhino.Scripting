namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsGrips =
  type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Enables or disables an object's grips. For curves and surfaces, these are
    ///  also called control points.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the specified object's grips will be turned on.
    ///  Otherwise, they will be turned off</param>
    ///<returns>(bool) True on success, False on failure</returns>
    static member EnableObjectGrips(objectId:Guid, [<OPT;DEF(true)>]enable:bool) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Prompts the user to pick a single object grip</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for selection of pre-selected object grip.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked object grip.</param>
    ///<returns>(Guid * float * Point3d) defining a grip record.
    ///  [0] = identifier of the object that owns the grip
    ///  [1] = index value of the grip
    ///  [2] = location of the grip</returns>
    static member GetObjectGrip([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid * float * Point3d =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Prompts user to pick one or more object grips from one or more objects.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>null:string</c>
    ///Prompt for picking</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>false</c>
    ///Allow for selection of pre-selected object grips</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the picked object grips</param>
    ///<returns>(Guid seq) containing one or more grip records. Each grip record is a tuple
    ///  [n][0] = identifier of the object that owns the grip
    ///  [n][1] = index value of the grip
    ///  [n][2] = location of the grip</returns>
    static member GetObjectGrips([<OPT;DEF(null:string)>]message:string, [<OPT;DEF(false)>]preselect:bool, [<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    
    static member internal Neighborgrip() : obj =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the next grip index from a specified grip index of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Zero based grip index from which to get the next grip index</param>
    ///<param name="direction">(int * int) Optional, Default Value: <c>0</c>
    ///Direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the next grip index found will be selected</param>
    ///<returns>(float) index of the next grip on success</returns>
    static member NextObjectGrip(objectId:Guid, index:int, [<OPT;DEF(0)>]direction:int * int, [<OPT;DEF(true)>]enable:bool) : float =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns number of grips owned by an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int) number of grips</returns>
    static member ObjectGripCount(objectId:Guid) : int =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the location of an object's grip</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to either query or modify</param>
    ///<returns>(Point3d) The current location of the grip referenced by index</returns>
    static member ObjectGripLocation(objectId:Guid, index:int) : Point3d = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Modifies the location of an object's grip</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to either query or modify</param>
    ///<param name="point">(Point3d)3D point defining new location of the grip</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectGripLocation(objectId:Guid, index:int, point:Point3d) : unit = //SET
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the location of all grips owned by an object. The
    /// locations of the grips are returned in a list of Point3d with each position
    /// in the list corresponding to that grip's index. To modify the locations of
    /// the grips, you must provide a list of points that contain the same number
    /// of points at grips</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(Point3d seq) The current location of all grips</returns>
    static member ObjectGripLocations(objectId:Guid) : Point3d seq = //GET
        failNotImpl () // genreation temp disabled !!

    ///<summary>Modifies the location of all grips owned by an object. The
    /// locations of the grips are returned in a list of Point3d with each position
    /// in the list corresponding to that grip's index. To modify the locations of
    /// the grips, you must provide a list of points that contain the same number
    /// of points at grips</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="points">(Point3d seq)List of 3D points identifying the new grip locations</param>
    ///<returns>(unit) void, nothing</returns>
    static member ObjectGripLocations(objectId:Guid, points:Point3d seq) : unit = //SET
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies that an object's grips are turned on</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(bool) True or False indicating Grips state</returns>
    static member ObjectGripsOn(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Verifies that an object's grips are turned on and at least one grip
    ///  is selected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ObjectGripsSelected(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns the previous grip index from a specified grip index of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Zero based grip index from which to get the previous grip index</param>
    ///<param name="direction">(int * int) Optional, Default Value: <c>0</c>
    ///Direction to get the next grip index (0=U, 1=V)</param>
    ///<param name="enable">(bool) Optional, Default Value: <c>true</c>
    ///If True, the next grip index found will be selected</param>
    ///<returns>(float) index of the next grip on success</returns>
    static member PrevObjectGrip(objectId:Guid, index:int, [<OPT;DEF(0)>]direction:int * int, [<OPT;DEF(true)>]enable:bool) : float =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Returns a list of grip indices indentifying an object's selected grips</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(int seq) list of indices on success</returns>
    static member SelectedObjectGrips(objectId:Guid) : int seq =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Selects a single grip owned by an object. If the object's grips are
    ///  not turned on, the grips will not be selected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to select</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member SelectObjectGrip(objectId:Guid, index:int) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Selects an object's grips. If the object's grips are not turned on,
    ///  they will not be selected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) Number of grips selected on success</returns>
    static member SelectObjectGrips(objectId:Guid) : float =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Unselects a single grip owned by an object. If the object's grips are
    ///  not turned on, the grips will not be unselected</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="index">(int) Index of the grip to unselect</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member UnselectObjectGrip(objectId:Guid, index:int) : bool =
        failNotImpl () // genreation temp disabled !!


    [<EXT>]
    ///<summary>Unselects an object's grips. Note, the grips will not be turned off.</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) Number of grips unselected on success</returns>
    static member UnselectObjectGrips(objectId:Guid) : float =
        failNotImpl () // genreation temp disabled !!


