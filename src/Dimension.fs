namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsDimension =
  type RhinoScriptSyntax with
    
    ///<summary>Adds an aligned dimension object to the document. An aligned dimension
    ///  is a linear dimension lined up with two points</summary>
    ///<param name="startPoint">(Point3d) First point of dimension</param>
    ///<param name="endePoint">(Point3d) Second point of dimension</param>
    ///<param name="pointOnDimensionLine">(Point3d) Location point of dimension line</param>
    ///<param name="style">(string) Optional, Default Value: <c>null</c>
    ///Name of dimension style</param>
    ///<returns>(Guid) identifier of new dimension on success</returns>
    static member AddAlignedDimension(startPoint:Point3d, endePoint:Point3d, pointOnDimensionLine:Point3d, [<OPT;DEF(null)>]style:string) : Guid =
        failNotImpl () // done in 2018


    ///<summary>Adds a new dimension style to the document. The new dimension style will
    ///  be initialized with the current default dimension style properties.</summary>
    ///<param name="dimstyleName">(string) Optional, Default Value: <c>null</c>
    ///Name of the new dimension style. If omitted, Rhino automatically generates the dimension style name</param>
    ///<returns>(string) name of the new dimension style on success</returns>
    static member AddDimStyle([<OPT;DEF(null)>]dimstyleName:string) : string =
        failNotImpl () // done in 2018


    ///<summary>Adds a leader to the document. Leader objects are planar.
    ///  The 3D points passed to this function should be co-planar</summary>
    ///<param name="points">(Point3d seq) List of (at least 2) 3D points</param>
    ///<param name="viewOrPlane">(string) Optional, Default Value: <c>null</c>
    ///If a view name is specified, points will be constrained
    ///  to the view's construction plane. If a view is not specified, points
    ///  will be constrained to a plane fit through the list of points</param>
    ///<param name="text">(string) Optional, Default Value: <c>null</c>
    ///Leader's text string</param>
    ///<returns>(Guid) identifier of the new leader on success</returns>
    static member AddLeader(points:Point3d seq, [<OPT;DEF(null)>]viewOrPlane:string, [<OPT;DEF(null)>]text:string) : Guid =
        failNotImpl () // done in 2018


    ///<summary>Adds a linear dimension to the document</summary>
    ///<param name="plane">(Plane) The plane on which the dimension will lie.</param>
    ///<param name="startPoint">(Point3d) The origin, or first point of the dimension.</param>
    ///<param name="endePoint">(Point3d) The offset, or second point of the dimension.</param>
    ///<param name="pointOnDimensionLine">(Point3d) A point that lies on the dimension line.</param>
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddLinearDimension(plane:Plane, startPoint:Point3d, endePoint:Point3d, pointOnDimensionLine:Point3d) : Guid =
        failNotImpl () // done in 2018


    ///<summary>Returns the current default dimension style</summary>
    ///<returns>(string) Name of the current dimension style</returns>
    static member CurrentDimStyle() : string =
        failNotImpl () // done in 2018

    ///<summary>Changes the current default dimension style</summary>
    ///<param name="dimstyleName">(string)Name of an existing dimension style to make current</param>
    ///<returns>(unit) unit</returns>
    static member CurrentDimStyle(dimstyleName:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Removes an existing dimension style from the document. The dimension style
    ///  to be removed cannot be referenced by any dimension objects.</summary>
    ///<param name="dimstyleName">(string) The name of an unreferenced dimension style</param>
    ///<returns>(string) The name of the deleted dimension style</returns>
    static member DeleteDimStyle(dimstyleName:string) : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the dimension style of a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The object's current dimension style name</returns>
    static member DimensionStyle(objectId:Guid) : string =
        failNotImpl () // done in 2018

    ///<summary>Modifies the dimension style of a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="dimstyleName">(string)The name of an existing dimension style</param>
    ///<returns>(unit) unit</returns>
    static member DimensionStyle(objectId:Guid, dimstyleName:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the text displayed by a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) the text displayed by a dimension object</returns>
    static member DimensionText(objectId:Guid) : string =
        failNotImpl () // done in 2018


    ///<summary>Returns the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(string) The current usertext string</returns>
    static member DimensionUserText(objectId:Guid) : string =
        failNotImpl () // done in 2018

    ///<summary>Modifies the user text string of a dimension object. The user
    /// text is the string that gets printed when the dimension is defined</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<param name="usertext">(string)The new user text string value</param>
    ///<returns>(unit) unit</returns>
    static member DimensionUserText(objectId:Guid, usertext:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the value of a dimension object</summary>
    ///<param name="objectId">(Guid) Identifier of the object</param>
    ///<returns>(float) numeric value of the dimension</returns>
    static member DimensionValue(objectId:Guid) : float =
        failNotImpl () // done in 2018


    ///<summary>Returns the angle display precision of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current angle precision</returns>
    static member DimStyleAnglePrecision(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the angle display precision of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="precision">(int)The new angle precision value. If omitted, the current angle
    ///  precision is returned</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleAnglePrecision(dimstyle:string, precision:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the arrow size of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current arrow size</returns>
    static member DimStyleArrowSize(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the arrow size of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="size">(int)The new arrow size. If omitted, the current arrow size is returned</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleArrowSize(dimstyle:string, size:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the number of dimension styles in the document</summary>
    ///<returns>(int) the number of dimension styles in the document</returns>
    static member DimStyleCount() : int =
        failNotImpl () // done in 2018


    ///<summary>Returns the extension line extension of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current extension line extension</returns>
    static member DimStyleExtension(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the extension line extension of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="extension">(int)The new extension line extension</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleExtension(dimstyle:string, extension:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the font used by a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(string) The current font</returns>
    static member DimStyleFont(dimstyle:string) : string =
        failNotImpl () // done in 2018

    ///<summary>Changes the font used by a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="font">(string)The new font face name</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleFont(dimstyle:string, font:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the leader arrow size of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current leader arrow size</returns>
    static member DimStyleLeaderArrowSize(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the leader arrow size of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="size">(int)The new leader arrow size</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleLeaderArrowSize(dimstyle:string, size:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) if factor is not defined, the current length factor</returns>
    static member DimStyleLengthFactor(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the length factor of a dimension style. Length factor
    /// is the conversion between Rhino units and dimension units</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="factor">(int)The new length factor</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleLengthFactor(dimstyle:string, factor:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the linear display precision of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current linear precision value</returns>
    static member DimStyleLinearPrecision(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the linear display precision of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="precision">(int)The new linear precision value</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleLinearPrecision(dimstyle:string, precision:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the names of all dimension styles in the document</summary>
    ///<returns>(string seq) the names of all dimension styles in the document</returns>
    static member DimStyleNames() : string seq =
        failNotImpl () // done in 2018


    ///<summary>Returns the number display format of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current display format
    ///  0 = Decimal
    ///  1 = Fractional
    ///  2 = Feet and inches</returns>
    static member DimStyleNumberFormat(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the number display format of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="format">(int)The new number format
    ///  0 = Decimal
    ///  1 = Fractional
    ///  2 = Feet and inches</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleNumberFormat(dimstyle:string, format:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the extension line offset of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current extension line offset</returns>
    static member DimStyleOffset(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the extension line offset of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="offset">(int)The new extension line offset</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleOffset(dimstyle:string, offset:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the prefix of a dimension style - the text to
    /// prefix to the dimension text.</summary>
    ///<param name="dimstyle">(string) The name of an existing dimstyle</param>
    ///<returns>(string) The current prefix</returns>
    static member DimStylePrefix(dimstyle:string) : string =
        failNotImpl () // done in 2018

    ///<summary>Changes the prefix of a dimension style - the text to
    /// prefix to the dimension text.</summary>
    ///<param name="dimstyle">(string) The name of an existing dimstyle</param>
    ///<param name="prefix">(string)The new prefix</param>
    ///<returns>(unit) unit</returns>
    static member DimStylePrefix(dimstyle:string, prefix:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the suffix of a dimension style - the text to
    /// append to the dimension text.</summary>
    ///<param name="dimstyle">(string) The name of an existing dimstyle</param>
    ///<returns>(string) The current suffix</returns>
    static member DimStyleSuffix(dimstyle:string) : string =
        failNotImpl () // done in 2018

    ///<summary>Changes the suffix of a dimension style - the text to
    /// append to the dimension text.</summary>
    ///<param name="dimstyle">(string) The name of an existing dimstyle</param>
    ///<param name="suffix">(string)The new suffix</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleSuffix(dimstyle:string, suffix:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the text alignment mode of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current text alignment
    ///  0 = Normal (same as 2)
    ///  1 = Horizontal to view
    ///  2 = Above the dimension line
    ///  3 = In the dimension line</returns>
    static member DimStyleTextAlignment(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the text alignment mode of a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="alignment">(int)The new text alignment
    ///  0 = Normal (same as 2)
    ///  1 = Horizontal to view
    ///  2 = Above the dimension line
    ///  3 = In the dimension line</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleTextAlignment(dimstyle:string, alignment:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the text gap used by a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current text gap</returns>
    static member DimStyleTextGap(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the text gap used by a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="gap">(int)The new text gap</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleTextGap(dimstyle:string, gap:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Returns the text height used by a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(int) The current text height</returns>
    static member DimStyleTextHeight(dimstyle:string) : int =
        failNotImpl () // done in 2018

    ///<summary>Changes the text height used by a dimension style</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<param name="height">(int)The new text height</param>
    ///<returns>(unit) unit</returns>
    static member DimStyleTextHeight(dimstyle:string, height:int) : unit =
        failNotImpl () // done in 2018


    ///<summary>Verifies an object is an aligned dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsAlignedDimension(objectId:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies an object is an angular dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsAngularDimension(objectId:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies an object is a diameter dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDiameterDimension(objectId:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies an object is a dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDimension(objectId:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies the existance of a dimension style in the document</summary>
    ///<param name="dimstyle">(string) The name of a dimstyle to test for</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDimStyle(dimstyle:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies that an existing dimension style is from a reference file</summary>
    ///<param name="dimstyle">(string) The name of an existing dimension style</param>
    ///<returns>(bool) True or False.</returns>
    static member IsDimStyleReference(dimstyle:string) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies an object is a dimension leader object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLeader(objectId:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies an object is a linear dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsLinearDimension(objectId:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies an object is an ordinate dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsOrdinateDimension(objectId:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Verifies an object is a radial dimension object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsRadialDimension(objectId:Guid) : bool =
        failNotImpl () // done in 2018


    ///<summary>Returns the text string of a dimension leader object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(string) The current text string</returns>
    static member LeaderText(objectId:Guid) : string =
        failNotImpl () // done in 2018

    ///<summary>Modifies the text string of a dimension leader object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="text">(string)The new text string</param>
    ///<returns>(unit) unit</returns>
    static member LeaderText(objectId:Guid, text:string) : unit =
        failNotImpl () // done in 2018


    ///<summary>Renames an existing dimension style</summary>
    ///<param name="oldstyle">(string) The name of an existing dimension style</param>
    ///<param name="newstyle">(string) The new dimension style name</param>
    ///<returns>(string) the new dimension style name</returns>
    static member RenameDimStyle(oldstyle:string, newstyle:string) : string =
        failNotImpl () // done in 2018


