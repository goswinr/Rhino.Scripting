namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsObject =
  type RhinoScriptSyntax with
    
    ///<summary>Copies object from one location to another, or in-place.</summary>
    ///<param name="objectId">(Guid) Object to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value: <c>null</c>
    ///Translation vector to apply</param>
    ///<returns>(Guid) id for the copy</returns>
    static member CopyObject(objectId:Guid, [<OPT;DEF(null)>]translation:Vector3d) : Guid =
        failNotImpl () 


    ///<summary>Copies one or more objects from one location to another, or in-place.</summary>
    ///<param name="objectIds">(Guid seq) List of objects to copy</param>
    ///<param name="translation">(Vector3d) Optional, Default Value: <c>null</c>
    ///List of three numbers or Vector3d representing
    ///  translation vector to apply to copied set</param>
    ///<returns>(Guid seq) identifiers for the copies</returns>
    static member CopyObjects(objectIds:Guid seq, [<OPT;DEF(null)>]translation:Vector3d) : Guid seq =
        failNotImpl () 


    ///<summary>Deletes a single object from the document</summary>
    ///<param name="objectId">(Guid) Identifier of object to delete</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member DeleteObject(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Deletes one or more objects from the document</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to delete</param>
    ///<returns>(float) Number of objects deleted</returns>
    static member DeleteObjects(objectIds:Guid seq) : float =
        failNotImpl () 


    ///<summary>Causes the selection state of one or more objects to change momentarily
    ///  so the object appears to flash on the screen</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to flash</param>
    ///<param name="style">(bool) Optional, Default Value: <c>true</c>
    ///If True, flash between object color and selection color.
    ///  If False, flash between visible and invisible</param>
    ///<returns>(unit) </returns>
    static member FlashObject(objectIds:Guid seq, [<OPT;DEF(true)>]style:bool) : unit =
        failNotImpl () 


    ///<summary>Hides a single object</summary>
    ///<param name="objectId">(Guid) Id of object to hide</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member HideObject(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Hides one or more objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to hide</param>
    ///<returns>(int) Number of objects hidden</returns>
    static member HideObjects(objectIds:Guid seq) : int =
        failNotImpl () 


    ///<summary>Verifies that an object is in either page layout space or model space</summary>
    ///<param name="objectId">(Guid) Id of an object to test</param>
    ///<returns>(bool) True if the object is in page layout space, False if the object is in model space</returns>
    static member IsLayoutObject(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies the existence of an object</summary>
    ///<param name="objectId">(Guid) An object to test</param>
    ///<returns>(bool) True if the object exists, False if the object does not exist</returns>
    static member IsObject(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies that an object is hidden. Hidden objects are not visible, cannot
    ///  be snapped to, and cannot be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is hidden, False if the object is not hidden</returns>
    static member IsObjectHidden(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies an object's bounding box is inside of another bounding box</summary>
    ///<param name="objectId">(Guid) Identifier of an object to be tested</param>
    ///<param name="box">(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) Bounding box to test for containment</param>
    ///<param name="testMode">(bool) Optional, Default Value: <c>true</c>
    ///If True, the object's bounding box must be contained by box
    ///  If False, the object's bounding box must be contained by or intersect box</param>
    ///<returns>(bool) True if object is inside box, False is object is not inside box</returns>
    static member IsObjectInBox(objectId:Guid, box:Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d, [<OPT;DEF(true)>]testMode:bool) : bool =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Verifies that an object is a member of a group</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="groupName">(string) Optional, Default Value: <c>null</c>
    ///The name of a group. If omitted, the function
    ///  verifies that the object is a member of any group</param>
    ///<returns>(bool) True if the object is a member of the specified group. If a group_name
    ///  was not specified, the object is a member of some group.
    ///  False if the object  is not a member of the specified group.
    ///  If a group_name was not specified, the object is not a member of any group</returns>
    static member IsObjectInGroup(objectId:Guid, [<OPT;DEF(null)>]groupName:string) : bool =
        failNotImpl () 


    ///<summary>Verifies that an object is locked. Locked objects are visible, and can
    ///  be snapped to, but cannot be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is locked, False if the object is not locked</returns>
    static member IsObjectLocked(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies that an object is normal. Normal objects are visible, can be
    ///  snapped to, and can be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to be tested</param>
    ///<returns>(bool) True if the object is normal, False if the object is not normal</returns>
    static member IsObjectNormal(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies that an object is a reference object. Reference objects are
    ///  objects that are not part of the current document</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is a reference object, False if the object is not a reference object</returns>
    static member IsObjectReference(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies that an object can be selected</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True or False</returns>
    static member IsObjectSelectable(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies that an object is currently selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(int) 0, the object is not selected
    ///  1, the object is selected
    ///  2, the object is entirely persistently selected
    ///  3, one or more proper sub-objects are selected</returns>
    static member IsObjectSelected(objectId:Guid) : int =
        failNotImpl () 


    ///<summary>Determines if an object is closed, solid</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is solid, or a mesh is closed., False otherwise.</returns>
    static member IsObjectSolid(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies an object's geometry is valid and without error</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<returns>(bool) True if the object is valid</returns>
    static member IsObjectValid(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is visible in a view</summary>
    ///<param name="objectId">(Guid) The identifier of an object to test</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///He title of the view.  If omitted, the current active view is used.</param>
    ///<returns>(bool) True if the object is visible in the specified view, otherwise False.</returns>
    static member IsVisibleInView(objectId:Guid, [<OPT;DEF(null)>]view:string) : bool =
        failNotImpl () 


    ///<summary>Locks a single object. Locked objects are visible, and they can be
    ///  snapped to. But, they cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member LockObject(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Locks one or more objects. Locked objects are visible, and they can be
    ///  snapped to. But, they cannot be selected.</summary>
    ///<param name="objectIds">(Guid seq) List of Strings or Guids. The identifiers of objects</param>
    ///<returns>(float) number of objects locked</returns>
    static member LockObjects(objectIds:Guid seq) : float =
        failNotImpl () 


    ///<summary>Matches, or copies the attributes of a source object to a target object</summary>
    ///<param name="targetIds">(Guid seq) Identifiers of objects to copy attributes to</param>
    ///<param name="sourceId">(Guid) Optional, Default Value: <c>null</c>
    ///Identifier of object to copy attributes from. If None,
    ///  then the default attributes are copied to the targetIds</param>
    ///<returns>(float) number of objects modified</returns>
    static member MatchObjectAttributes(targetIds:Guid seq, [<OPT;DEF(null)>]sourceId:Guid) : float =
        failNotImpl () 


    ///<summary>Mirrors a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to mirror</param>
    ///<param name="startPoint">(Point3d) Start of the mirror plane</param>
    ///<param name="endePoint">(Point3d) End of the mirror plane</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the mirrored object</returns>
    static member MirrorObject(objectId:Guid, startPoint:Point3d, endePoint:Point3d, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () 


    ///<summary>Mirrors a list of objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to mirror</param>
    ///<param name="startPoint">(Point3d) Start of the mirror plane</param>
    ///<param name="endePoint">(Point3d) End of the mirror plane</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid seq) List of identifiers of the mirrored objects</returns>
    static member MirrorObjects(objectIds:Guid seq, startPoint:Point3d, endePoint:Point3d, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () 


    ///<summary>Moves a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(Guid) Identifier of the moved object</returns>
    static member MoveObject(objectId:Guid, translation:Vector3d) : Guid =
        failNotImpl () 


    ///<summary>Moves one or more objects</summary>
    ///<param name="objectIds">(Guid seq) The identifiers objects to move</param>
    ///<param name="translation">(Vector3d) List of 3 numbers or Vector3d</param>
    ///<returns>(Guid seq) identifiers of the moved objects</returns>
    static member MoveObjects(objectIds:Guid seq, translation:Vector3d) : Guid seq =
        failNotImpl () 


    ///<summary>Returns the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectIds">(Guid) Id or ids of object(s)</param>
    ///<returns>(Drawing.Color) The current color value</returns>
    static member ObjectColor(objectIds:Guid) : Drawing.Color =
        failNotImpl () 

    ///<summary>Modifies the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectIds">(Guid) Id or ids of object(s)</param>
    ///<param name="color">(Drawing.Color)The new color value. If omitted, then current object
    ///  color is returned. If objectIds is a list, color is required</param>
    ///<returns>(unit) unit</returns>
    static member ObjectColor(objectIds:Guid, color:Drawing.Color) : unit =
        failNotImpl () 


    ///<summary>Modifies the color of an object. Object colors are represented
    /// as RGB colors. An RGB color specifies the relative intensity of red, green,
    /// and blue to cause a specific color to be displayed</summary>
    ///<param name="objectIds">(Guid seq) Id or ids of object(s)</param>
    ///<param name="color">(Drawing.Color)The new color value. If omitted, then current object
    ///  color is returned. If objectIds is a list, color is required</param>
    ///<returns>(unit) unit</returns>
    static member ObjectColor(objectIds:Guid, color:Drawing.Color seq) : unit =
        failNotImpl () 


    ///<summary>Returns the color source of an object.</summary>
    ///<param name="objectIds">(Guid) Single identifier of list of identifiers</param>
    ///<returns>(int) The current color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</returns>
    static member ObjectColorSource(objectIds:Guid) : int =
        failNotImpl () 

    ///<summary>Modifies the color source of an object.</summary>
    ///<param name="objectIds">(Guid) Single identifier of list of identifiers</param>
    ///<param name="source">(int)New color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectColorSource(objectIds:Guid, source:int) : unit =
        failNotImpl () 


    ///<summary>Modifies the color source of an object.</summary>
    ///<param name="objectIds">(Guid seq) Single identifier of list of identifiers</param>
    ///<param name="source">(int)New color source
    ///  0 = color from layer
    ///  1 = color from object
    ///  2 = color from material
    ///  3 = color from parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectColorSource(objectIds:Guid, source:int seq) : unit =
        failNotImpl () 


    ///<summary>Returns a short text description of an object</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(string) A short text description of the object .</returns>
    static member ObjectDescription(objectId:Guid) : string =
        failNotImpl () 


    ///<summary>Returns all of the group names that an object is assigned to</summary>
    ///<param name="objectId">(Guid) Identifier of an object(s)</param>
    ///<returns>(string seq) list of group names on success</returns>
    static member ObjectGroups(objectId:Guid) : string seq =
        failNotImpl () 


    ///<summary>Returns the layer of an object</summary>
    ///<param name="objectId">(Guid) The identifier of the object(s)</param>
    ///<returns>(string) The object's current layer</returns>
    static member ObjectLayer(objectId:Guid) : string =
        failNotImpl () 

    ///<summary>Modifies the layer of an object</summary>
    ///<param name="objectId">(Guid) The identifier of the object(s)</param>
    ///<param name="layer">(string)Name of an existing layer</param>
    ///<returns>(unit) unit</returns>
    static member ObjectLayer(objectId:Guid, layer:string) : unit =
        failNotImpl () 


    ///<summary>Modifies the layer of an object</summary>
    ///<param name="objectId">(Guid seq) The identifier of the object(s)</param>
    ///<param name="layer">(string)Name of an existing layer</param>
    ///<returns>(unit) unit</returns>
    static member ObjectLayer(objectId:Guid, layer:string seq) : unit =
        failNotImpl () 


    ///<summary>Returns the layout or model space of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The object's current page layout view</returns>
    static member ObjectLayout(objectId:Guid) : string =
        failNotImpl () 

    ///<summary>Changes the layout or model space of an object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="layout">(string)To change, or move, an object from model space to page
    ///  layout space, or from one page layout to another, then specify the
    ///  title or identifier of an existing page layout view. To move an object
    ///  from page layout space to model space, just specify None</param>
    ///<param name="returnName">(bool)If True, the name, or title, of the page layout view
    ///  is returned. If False, the identifier of the page layout view is returned</param>
    ///<returns>(unit) unit</returns>
    static member ObjectLayout(objectId:Guid, layout:string, [<OPT;DEF(true)>]returnName:bool) : unit =
        failNotImpl () 


    ///<summary>Returns the linetype of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(string) The object's current linetype</returns>
    static member ObjectLinetype(objectIds:Guid) : string =
        failNotImpl () 

    ///<summary>Modifies the linetype of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="linetyp">(string)Name of an existing linetyp. If omitted, the current
    ///  linetyp is returned. If objectIds is a list of identifiers, this parameter
    ///  is required</param>
    ///<returns>(unit) unit</returns>
    static member ObjectLinetype(objectIds:Guid, linetyp:string) : unit =
        failNotImpl () 


    ///<summary>Modifies the linetype of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="linetyp">(string)Name of an existing linetyp. If omitted, the current
    ///  linetyp is returned. If objectIds is a list of identifiers, this parameter
    ///  is required</param>
    ///<returns>(unit) unit</returns>
    static member ObjectLinetype(objectIds:Guid, linetyp:string seq) : unit =
        failNotImpl () 


    ///<summary>Returns the linetype source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(int) The object's current linetype source
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</returns>
    static member ObjectLinetypeSource(objectIds:Guid) : int =
        failNotImpl () 

    ///<summary>Modifies the linetype source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="source">(int)New linetype source. If omitted, the current source is returned.
    ///  If objectIds is a list of identifiers, this parameter is required
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectLinetypeSource(objectIds:Guid, source:int) : unit =
        failNotImpl () 


    ///<summary>Modifies the linetype source of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="source">(int)New linetype source. If omitted, the current source is returned.
    ///  If objectIds is a list of identifiers, this parameter is required
    ///    0 = By Layer
    ///    1 = By Object
    ///    3 = By Parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectLinetypeSource(objectIds:Guid, source:int seq) : unit =
        failNotImpl () 


    ///<summary>Returns the material index of an object. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(int) If the return value of ObjectMaterialSource is "material by object", then
    ///  the return value of this function is the index of the object's rendering
    ///  material. A material index of -1 indicates no material has been assigned,
    ///  and that Rhino's internal default material has been assigned to the object.</returns>
    static member ObjectMaterialIndex(objectId:Guid) : int =
        failNotImpl () 

    ///<summary>Changes the material index of an object. Rendering materials are stored in
    /// Rhino's rendering material table. The table is conceptually an array. Render
    /// materials associated with objects and layers are specified by zero based
    /// indices into this array.</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<param name="materialIndex">(int)The new material index</param>
    static member ObjectMaterialIndex(objectId:Guid, materialIndex:int) : unit =
        failNotImpl () 


    ///<summary>Returns the rendering material source of an object.</summary>
    ///<param name="objectIds">(Guid) One or more object identifiers</param>
    ///<returns>(int) The current rendering material source
    ///  0 = Material from layer
    ///  1 = Material from object
    ///  3 = Material from parent</returns>
    static member ObjectMaterialSource(objectIds:Guid) : int =
        failNotImpl () 

    ///<summary>Modifies the rendering material source of an object.</summary>
    ///<param name="objectIds">(Guid) One or more object identifiers</param>
    ///<param name="source">(int)The new rendering material source. If omitted and a single
    ///  object is provided in objectIds, then the current material source is
    ///  returned. This parameter is required if multiple objects are passed in
    ///  objectIds
    ///  0 = Material from layer
    ///  1 = Material from object
    ///  3 = Material from parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectMaterialSource(objectIds:Guid, source:int) : unit =
        failNotImpl () 


    ///<summary>Modifies the rendering material source of an object.</summary>
    ///<param name="objectIds">(Guid seq) One or more object identifiers</param>
    ///<param name="source">(int)The new rendering material source. If omitted and a single
    ///  object is provided in objectIds, then the current material source is
    ///  returned. This parameter is required if multiple objects are passed in
    ///  objectIds
    ///  0 = Material from layer
    ///  1 = Material from object
    ///  3 = Material from parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectMaterialSource(objectIds:Guid, source:int seq) : unit =
        failNotImpl () 


    ///<summary>Returns the name of an object</summary>
    ///<param name="objectId">(Guid) Id or ids of object(s)</param>
    ///<returns>(string) The current object name</returns>
    static member ObjectName(objectId:Guid) : string =
        failNotImpl () 

    ///<summary>Modifies the name of an object</summary>
    ///<param name="objectId">(Guid) Id or ids of object(s)</param>
    ///<param name="name">(string)The new object name. If omitted, the current name is returned</param>
    ///<returns>(unit) unit</returns>
    static member ObjectName(objectId:Guid, name:string) : unit =
        failNotImpl () 


    ///<summary>Modifies the name of an object</summary>
    ///<param name="objectId">(Guid seq) Id or ids of object(s)</param>
    ///<param name="name">(string)The new object name. If omitted, the current name is returned</param>
    ///<returns>(unit) unit</returns>
    static member ObjectName(objectId:Guid, name:string seq) : unit =
        failNotImpl () 


    ///<summary>Returns the print color of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(Drawing.Color) The object's current print color</returns>
    static member ObjectPrintColor(objectIds:Guid) : Drawing.Color =
        failNotImpl () 

    ///<summary>Modifies the print color of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="color">(Drawing.Color)New print color. If omitted, the current color is returned.</param>
    ///<returns>(unit) unit</returns>
    static member ObjectPrintColor(objectIds:Guid, color:Drawing.Color) : unit =
        failNotImpl () 


    ///<summary>Modifies the print color of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="color">(Drawing.Color)New print color. If omitted, the current color is returned.</param>
    ///<returns>(unit) unit</returns>
    static member ObjectPrintColor(objectIds:Guid, color:Drawing.Color seq) : unit =
        failNotImpl () 


    ///<summary>Returns the print color source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(int) The object's current print color source
    ///  0 = print color by layer
    ///  1 = print color by object
    ///  3 = print color by parent</returns>
    static member ObjectPrintColorSource(objectIds:Guid) : int =
        failNotImpl () 

    ///<summary>Modifies the print color source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="source">(int)New print color source
    ///  0 = print color by layer
    ///  1 = print color by object
    ///  3 = print color by parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectPrintColorSource(objectIds:Guid, source:int) : unit =
        failNotImpl () 


    ///<summary>Modifies the print color source of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="source">(int)New print color source
    ///  0 = print color by layer
    ///  1 = print color by object
    ///  3 = print color by parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectPrintColorSource(objectIds:Guid, source:int seq) : unit =
        failNotImpl () 


    ///<summary>Returns the print width of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(float) The object's current print width</returns>
    static member ObjectPrintWidth(objectIds:Guid) : float =
        failNotImpl () 

    ///<summary>Modifies the print width of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="width">(float)New print width value in millimeters, where width=0 means use
    ///  the default width, and width<0 means do not print (visible for screen display,
    ///  but does not show on print). If omitted, the current width is returned.</param>
    ///<returns>(unit) unit</returns>
    static member ObjectPrintWidth(objectIds:Guid, width:float) : unit =
        failNotImpl () 


    ///<summary>Modifies the print width of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="width">(float)New print width value in millimeters, where width=0 means use
    ///  the default width, and width<0 means do not print (visible for screen display,
    ///  but does not show on print). If omitted, the current width is returned.</param>
    ///<returns>(unit) unit</returns>
    static member ObjectPrintWidth(objectIds:Guid, width:float seq) : unit =
        failNotImpl () 


    ///<summary>Returns the print width source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<returns>(int) The object's current print width source
    ///  0 = print width by layer
    ///  1 = print width by object
    ///  3 = print width by parent</returns>
    static member ObjectPrintWidthSource(objectIds:Guid) : int =
        failNotImpl () 

    ///<summary>Modifies the print width source of an object</summary>
    ///<param name="objectIds">(Guid) Identifiers of object(s)</param>
    ///<param name="source">(int)New print width source
    ///  0 = print width by layer
    ///  1 = print width by object
    ///  3 = print width by parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectPrintWidthSource(objectIds:Guid, source:int) : unit =
        failNotImpl () 


    ///<summary>Modifies the print width source of an object</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of object(s)</param>
    ///<param name="source">(int)New print width source
    ///  0 = print width by layer
    ///  1 = print width by object
    ///  3 = print width by parent</param>
    ///<returns>(unit) unit</returns>
    static member ObjectPrintWidthSource(objectIds:Guid, source:int seq) : unit =
        failNotImpl () 


    ///<summary>Returns the object type</summary>
    ///<param name="objectId">(Guid) Identifier of an object</param>
    ///<returns>(int) The object type .
    ///  The valid object types are as follows:
    ///  Value   Description
    ///    0           Unknown object
    ///    1           Point
    ///    2           Point cloud
    ///    4           Curve
    ///    8           Surface or single-face brep
    ///    16          Polysurface or multiple-face
    ///    32          Mesh
    ///    256         Light
    ///    512         Annotation
    ///    4096        Instance or block reference
    ///    8192        Text dot object
    ///    16384       Grip object
    ///    32768       Detail
    ///    65536       Hatch
    ///    131072      Morph control
    ///    134217728   Cage
    ///    268435456   Phantom
    ///    536870912   Clipping plane
    ///    1073741824  Extrusion</returns>
    static member ObjectType(objectId:Guid) : int =
        failNotImpl () 


    ///<summary>Orients a single object based on input points.
    ///  If two 3-D points are specified, then this method will function similar to Rhino's Orient command.  If more than two 3-D points are specified, then the function will orient similar to Rhino's Orient3Pt command.
    ///  The orient flags values can be added together to specify multiple options.
    ///    Value   Description
    ///    1       Copy object.  The default is not to copy the object.
    ///    2       Scale object.  The default is not to scale the object.  Note, the scale option only applies if both reference and target contain only two 3-D points.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="reference">(Point3d seq) List of 3-D reference points.</param>
    ///<param name="target">(Point3d seq) List of 3-D target points</param>
    ///<param name="flags">(int) Optional, Default Value: <c>0</c>
    ///1 = copy object
    ///  2 = scale object
    ///  3 = copy and scale</param>
    ///<returns>(Guid) The identifier of the oriented object .</returns>
    static member OrientObject(objectId:Guid, reference:Point3d seq, target:Point3d seq, [<OPT;DEF(0)>]flags:int) : Guid =
        failNotImpl () 


    ///<summary>Rotates a single object</summary>
    ///<param name="objectId">(Guid) The identifier of an object to rotate</param>
    ///<param name="centerPoint">(Point3d) The center of rotation</param>
    ///<param name="rotationAngle">(float) In degrees</param>
    ///<param name="axis">(Plane) Optional, Default Value: <c>null</c>
    ///Axis of rotation, If omitted, the Z axis of the active
    ///  construction plane is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the rotated object</returns>
    static member RotateObject(objectId:Guid, centerPoint:Point3d, rotationAngle:float, [<OPT;DEF(null)>]axis:Plane, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () 


    ///<summary>Rotates multiple objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to rotate</param>
    ///<param name="centerPoint">(Point3d) The center of rotation</param>
    ///<param name="rotationAngle">(float) In degrees</param>
    ///<param name="axis">(Plane) Optional, Default Value: <c>null</c>
    ///Axis of rotation, If omitted, the Z axis of the active
    ///  construction plane is used as the rotation axis</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid seq) identifiers of the rotated objects</returns>
    static member RotateObjects(objectIds:Guid seq, centerPoint:Point3d, rotationAngle:float, [<OPT;DEF(null)>]axis:Plane, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () 


    ///<summary>Scales a single object. Can be used to perform a uniform or non-uniform
    ///  scale transformation. Scaling is based on the active construction plane.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object</param>
    ///<returns>(Guid) Identifier of the scaled object</returns>
    static member ScaleObject(objectId:Guid, origin:Point3d, scale:float*float*float, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () 


    ///<summary>Scales one or more objects. Can be used to perform a uniform or non-
    ///  uniform scale transformation. Scaling is based on the active construction plane.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects to scale</param>
    ///<param name="origin">(Point3d) The origin of the scale transformation</param>
    ///<param name="scale">(float*float*float) Three numbers that identify the X, Y, and Z axis scale factors to apply</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid seq) identifiers of the scaled objects</returns>
    static member ScaleObjects(objectIds:Guid seq, origin:Point3d, scale:float*float*float, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () 


    ///<summary>Selects a single object</summary>
    ///<param name="objectId">(Guid) The identifier of the object to select</param>
    ///<param name="redraw">(bool) Optional, Default Value: <c>true</c>
    ///Redraw view too</param>
    ///<returns>(bool) True on success</returns>
    static member SelectObject(objectId:Guid, [<OPT;DEF(true)>]redraw:bool) : bool =
        failNotImpl () 


    ///<summary>Selects one or more objects</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of the objects to select</param>
    ///<returns>(float) number of selected objects</returns>
    static member SelectObjects(objectIds:Guid seq) : float =
        failNotImpl () 


    ///<summary>Perform a shear transformation on a single object</summary>
    ///<param name="objectId">(Guid seq) The identifier of an object</param>
    ///<param name="origin">(Point3d) Origin point of the shear transformation</param>
    ///<param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    ///<param name="angleDegrees">(int) The shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid) Identifier of the sheared object</returns>
    static member ShearObject(objectId:Guid seq, origin:Point3d, referencePoint:Point3d, angleDegrees:int, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () 


    ///<summary>Shears one or more objects</summary>
    ///<param name="objectIds">(Guid seq) The identifiers objects to shear</param>
    ///<param name="origin">(Point3d) Origin point of the shear transformation</param>
    ///<param name="referencePoint">(Point3d) Reference point of the shear transformation</param>
    ///<param name="angleDegrees">(int) The shear angle in degrees</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid seq) identifiers of the sheared objects</returns>
    static member ShearObjects(objectIds:Guid seq, origin:Point3d, referencePoint:Point3d, angleDegrees:int, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () 


    ///<summary>Shows a previously hidden object. Hidden objects are not visible, cannot
    ///  be snapped to and cannot be selected</summary>
    ///<param name="objectId">(Guid) Representing id of object to show</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member ShowObject(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Shows one or more objects. Hidden objects are not visible, cannot be
    ///  snapped to and cannot be selected</summary>
    ///<param name="objectIds">(Guid seq) Ids of objects to show</param>
    ///<returns>(float) Number of objects shown</returns>
    static member ShowObjects(objectIds:Guid seq) : float =
        failNotImpl () 


    ///<summary>Moves, scales, or rotates an object given a 4x4 transformation matrix.
    ///  The matrix acts on the left.</summary>
    ///<param name="objectId">(Guid) The identifier of the object.</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers).</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the object.</param>
    ///<returns>(Guid) The identifier of the transformed object</returns>
    static member TransformObject(objectId:Guid, matrix:Transform, [<OPT;DEF(false)>]copy:bool) : Guid =
        failNotImpl () 


    ///<summary>Moves, scales, or rotates a list of objects given a 4x4 transformation
    ///  matrix. The matrix acts on the left.</summary>
    ///<param name="objectIds">(Guid seq) List of object identifiers.</param>
    ///<param name="matrix">(Transform) The transformation matrix (4x4 array of numbers).</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///Copy the objects</param>
    ///<returns>(Guid seq) ids identifying the newly transformed objects</returns>
    static member TransformObjects(objectIds:Guid seq, matrix:Transform, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () 


    ///<summary>Unlocks an object. Locked objects are visible, and can be snapped to,
    ///  but they cannot be selected.</summary>
    ///<param name="objectId">(Guid) The identifier of an object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member UnlockObject(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Unlocks one or more objects. Locked objects are visible, and can be
    ///  snapped to, but they cannot be selected.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of objects</param>
    ///<returns>(float) number of objects unlocked</returns>
    static member UnlockObjects(objectIds:Guid seq) : float =
        failNotImpl () 


    ///<summary>Unselects a single selected object</summary>
    ///<param name="objectId">(Guid) Id of object to unselect</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member UnselectObject(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Unselects one or more selected objects.</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of the objects to unselect.</param>
    ///<returns>(float) The number of objects unselected</returns>
    static member UnselectObjects(objectIds:Guid seq) : float =
        failNotImpl () 


