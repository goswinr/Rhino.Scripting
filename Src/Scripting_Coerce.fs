namespace Rhino.Scripting

open Rhino
open System.ComponentModel
open System
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils

[<AutoOpen; EditorBrowsable(EditorBrowsableState.Never)>]
module AutoOpenCoerce =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


 
    //---------------------------------------------
    //----------- Coerce and TryCoerce pairs -----
    //---------------------------------------------



    /// <summary>Attempt to get a Guid that is not empty.</summary>
    /// <param name="objectId">(Guid) The Guid.</param>
    /// <returns>A Guid option. None if the Guid is empty.</returns>
    static member TryCoerceGuid (objectId:Guid) : Guid option =
        if Guid.Empty = objectId then None else Some objectId

    /// <summary>Attempt to parse a Guid from a string.</summary>
    /// <param name="objectId">(string) The Guid as a string.</param>
    /// <returns>A Guid option. None if the string is not a Guid or is the empty Guid.</returns>
    static member TryCoerceGuid (objectId:string) : Guid option =
        match Guid.TryParse objectId with
        | true, g when g <> Guid.Empty -> Some g
        | _ -> None

    /// <summary>Attempt to get the Guid of a RhinoObject.</summary>
    /// <param name="rhinoObject">(DocObjects.RhinoObject) The RhinoObject.</param>
    /// <returns>A Guid option. None if the RhinoObject is null.</returns>
    static member TryCoerceGuid (rhinoObject:DocObjects.RhinoObject) : Guid option =
        if isNull rhinoObject then None else Some rhinoObject.Id

    /// <summary>Attempt to get the Guid of the object an ObjRef refers to.</summary>
    /// <param name="objRef">(DocObjects.ObjRef) The object reference.</param>
    /// <returns>A Guid option. None if the ObjRef is null.</returns>
    static member TryCoerceGuid (objRef:DocObjects.ObjRef) : Guid option =
        if isNull objRef then None else Some objRef.ObjectId

    /// <summary>Checks that a Guid is not empty.</summary>
    /// <param name="objectId">(Guid) The Guid.</param>
    /// <returns>(Guid) The same Guid. Raises a RhinoScriptingException if the Guid is empty.</returns>
    static member CoerceGuid(objectId:Guid) : Guid =
        if Guid.Empty = objectId then RhinoScriptingException.Raise "CoerceGuid: Guid is Empty"
        objectId

    /// <summary>Parses a Guid from a string.</summary>
    /// <param name="objectId">(string) The Guid as a string.</param>
    /// <returns>(Guid) The Guid. Raises a RhinoScriptingException if the string is not a Guid or is the empty Guid.</returns>
    static member CoerceGuid(objectId:string) : Guid =
        match Guid.TryParse objectId with
        | true, g when g <> Guid.Empty -> g
        | true, _ -> RhinoScriptingException.Raise "CoerceGuid: string '%s' is the empty Guid" objectId
        | _ -> RhinoScriptingException.Raise "CoerceGuid: string '%s' can not be converted to a Guid" objectId

    /// <summary>Gets the Guid of a RhinoObject.</summary>
    /// <param name="rhinoObject">(DocObjects.RhinoObject) The RhinoObject.</param>
    /// <returns>(Guid) The Guid of the RhinoObject. Raises a RhinoScriptingException if the RhinoObject is null.</returns>
    static member CoerceGuid(rhinoObject:DocObjects.RhinoObject) : Guid =
        if isNull rhinoObject then RhinoScriptingException.Raise "CoerceGuid: RhinoObject is null"
        rhinoObject.Id

    /// <summary>Gets the Guid of the object an ObjRef refers to.</summary>
    /// <param name="objRef">(DocObjects.ObjRef) The object reference.</param>
    /// <returns>(Guid) The Guid of the referenced object. Raises a RhinoScriptingException if the ObjRef is null.</returns>
    static member CoerceGuid(objRef:DocObjects.ObjRef) : Guid =
        if isNull objRef then RhinoScriptingException.Raise "CoerceGuid: ObjRef is null"
        objRef.ObjectId


    /// <summary>Attempt to get a RhinoObject from the document with a given objectId.</summary>
    /// <param name="objectId">Object identifier (Guid or string).</param>
    /// <returns>A RhinoObject option.</returns>
    static member TryCoerceRhinoObject (objectId:Guid) : DocObjects.RhinoObject option =
        match State.Doc.Objects.FindId(objectId) with
        | null -> None
        | o    -> Some o

    /// <summary>Attempt to get a RhinoObject from the document with a given objectId.</summary>
    /// <param name="objectId">(Guid) Object identifier.</param>
    /// <returns>A RhinoObject. Raises a RhinoScriptingException if coercion failed.</returns>
    static member CoerceRhinoObject(objectId:Guid) : DocObjects.RhinoObject =
        match RhinoScriptSyntax.TryCoerceRhinoObject objectId with
        |Some o -> o
        |None ->
            if Guid.Empty = objectId then    RhinoScriptingException.Raise "CoerceRhinoObject failed on empty Guid"
            else                             RhinoScriptingException.Raise "CoerceRhinoObject: The Guid %O was not found in the Current Object table." objectId

    /// <summary>Attempt to get GeometryBase from a given Guid. Returns None on empty or unknown Guid.</summary>
    /// <param name="objectId">Geometry identifier (Guid).</param>
    /// <returns>A Rhino.Geometry.GeometryBase option.</returns>
    static member TryCoerceGeometry (objectId:Guid) :GeometryBase option =
        match State.Doc.Objects.FindId(objectId) with
        | null -> None
        | o -> Some o.Geometry

    /// <summary>Attempt to get GeometryBase from a given input.</summary>
    /// <param name="objectId">(Guid) geometry Identifier </param>
    /// <returns>(Rhino.Geometry.GeometryBase. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceGeometry(objectId:Guid) : GeometryBase =
        if Guid.Empty = objectId then RhinoScriptingException.Raise "CoerceGeometry failed on empty Guid"
        let o = State.Doc.Objects.FindId(objectId)
        if isNull o then RhinoScriptingException.Raise "CoerceGeometry failed: Guid %O not found in Object table." objectId
        o.Geometry

    /// <summary>Attempt to get Rhino LightObject from the document with a given objectId.</summary>
    /// <param name="objectId">(Guid) light Identifier</param>
    /// <returns>A Rhino.Geometry.Light. Option.</returns>
    static member TryCoerceLight (objectId:Guid) : Light option =
        match RhinoScriptSyntax.TryCoerceGeometry objectId with
        | Some (:? Geometry.Light as l) -> Some l
        | _ -> None

    /// <summary>Attempt to get Rhino LightObject from the document with a given objectId.</summary>
    /// <param name="objectId">(Guid) light Identifier</param>
    /// <returns>A  Rhino.Geometry.Light. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceLight (objectId:Guid) : Light =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :? Geometry.Light as l -> l
        | _ -> RhinoScriptingException.Raise "CoerceLight failed on: %s " (Pretty.str objectId)


    /// <summary>Attempt to get Mesh class from given Guid. Returns None on empty or unknown Guid.</summary>
    /// <param name="objectId">Mesh Identifier (Guid)</param>
    /// <returns>A Rhino.Geometry.Surface Option.</returns>
    static member TryCoerceMesh (objectId:Guid) : Mesh option =
        match State.Doc.Objects.FindId(objectId) with
            | null -> None
            | o ->
                match o.Geometry with
                | :? Mesh as m -> Some m
                | _ -> None

    /// <summary>Attempt to get Mesh geometry from the document with a given objectId.</summary>
    /// <param name="objectId">object Identifier (Guid or string)</param>
    /// <returns>(Rhino.Geometry.Mesh. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceMesh(objectId:Guid) : Mesh =
        match RhinoScriptSyntax.TryCoerceMesh(objectId) with
        | Some m -> m
        | None -> RhinoScriptingException.Raise "CoerceMesh failed on: %s " (Pretty.str objectId)

    /// <summary>Attempt to get Surface class from given Guid. Returns None on empty or unknown Guid.</summary>
    /// <param name="objectId">Surface Identifier (Guid)</param>
    /// <returns>A Rhino.Geometry.Surface Option.</returns>
    static member TryCoerceSurface (objectId:Guid) : Surface option =
        match State.Doc.Objects.FindId(objectId) with
        | null -> None
        | o ->
            match o.Geometry with
            | :? Surface as c -> Some c
            | :? Brep as b ->
                if b.Faces.Count = 1 then Some (b.Faces.[0] :> Surface)
                else None
            //| :? Extrusion as e -> // covered by Surface
            //     let b = e.ToBrep()
            //     if b.Faces.Count = 1 then Some (b.Faces.[0] :> Surface)
            //     else None
            | _ -> None

    /// <summary>Attempt to get Surface geometry from the document with a given objectId.</summary>
    /// <param name="objectId">the object's Identifier</param>
    /// <returns>(Rhino.Geometry.Surface. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceSurface(objectId:Guid) : Surface =
        match RhinoScriptSyntax.CoerceGeometry(objectId) with
        | :? Surface as c -> c
        | :? Brep as b ->
            if b.Faces.Count = 1 then b.Faces.[0] :> Surface
            else RhinoScriptingException.Raise "CoerceSurface failed on %O from Brep with %d Faces" objectId b.Faces.Count
        | _ -> RhinoScriptingException.Raise "CoerceSurface failed on: %O " objectId

    /// <summary>Attempt to get a Polysurface or Brep class from given Guid. Works on Extrusions too. Returns None on empty or unknown Guid.</summary>
    /// <param name="objectId">Polysurface Identifier (Guid)</param>
    /// <returns>A Rhino.Geometry.Mesh Option.</returns>
    static member TryCoerceBrep (objectId:Guid) : Brep option =
        match State.Doc.Objects.FindId(objectId) with
            | null -> None
            | o ->
                match o.Geometry with
                | :? Brep as b ->  Some b
                | :? Extrusion as b -> Some (b.ToBrep(true))
                | _ -> None

    /// <summary>Attempt to get Polysurface geometry from the document with a given objectId.</summary>
    /// <param name="objectId">objectId (Guid or string) to be RhinoScriptSyntax.Coerced into a brep</param>
    /// <returns>(Rhino.Geometry.Brep. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceBrep(objectId:Guid) : Brep  =
        match RhinoScriptSyntax.TryCoerceBrep(objectId) with
        | Some b -> b
        | None -> RhinoScriptingException.Raise "CoerceBrep failed on: %s " (Pretty.str objectId)

    /// <summary>Attempt to get Curve geometry from the document with a given objectId.</summary>
    /// <param name="objectId">objectId (Guid or string) to be RhinoScriptSyntax.Coerced into a Curve</param>
    /// <param name="segmentIndex">(int) Optional, index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    /// <returns>A Rhino.Geometry.Curve Option.</returns>
    static member TryCoerceCurve(objectId:Guid,[<OPT;DEF(-1)>]segmentIndex:int) : Curve option =
        match RhinoScriptSyntax.TryCoerceGeometry objectId with
        | None -> None
        | Some geo ->
            if segmentIndex < 0 then
                match geo with
                | :? Curve as c -> Some c
                | _ -> None
            else
                match geo with
                | :? PolyCurve as c ->
                    let crv = c.SegmentCurve(segmentIndex)
                    if isNull crv then None
                    else Some crv
                | :? Curve as c -> Some c
                | _ -> None

    /// <summary>Attempt to get Curve geometry from the document with a given objectId.</summary>
    /// <param name="objectId">objectId (Guid or string) to be RhinoScriptSyntax.Coerced into a Curve</param>
    /// <param name="segmentIndex">(int) Optional, index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    /// <returns>(Rhino.Geometry.Curve. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceCurve(objectId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Curve =
        if segmentIndex < 0 then
            match RhinoScriptSyntax.CoerceGeometry(objectId) with
            | :? Curve as c -> c
            | _ -> RhinoScriptingException.Raise "CoerceCurve failed on: %s " (Pretty.str objectId)
        else
            match RhinoScriptSyntax.CoerceGeometry(objectId) with
            | :? PolyCurve as c ->
                let crv = c.SegmentCurve(segmentIndex)
                if isNull crv then RhinoScriptingException.Raise "CoerceCurve failed on segment index %d for %s" segmentIndex  (Pretty.str objectId)
                crv
            | :? Curve as c -> c
            | _ -> RhinoScriptingException.Raise "CoerceCurve failed for %s"  (Pretty.str objectId)


    /// <summary>Returns the given Line. This overload exists so that TryCoerceLine accepts a Line too.</summary>
    /// <param name="line">(Line) The line.</param>
    /// <returns>A Geometry.Line Option, always Some.</returns>
    static member TryCoerceLine(line:Line) : Line option =
        Some line

    /// <summary>Attempt to get a Line from a Curve that is linear within the current Documents Absolute Tolerance.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>A Geometry.Line Option. None if the curve is null or not linear.</returns>
    static member TryCoerceLine(curve:Curve) : Line option =
        match curve with
        | null -> None
        | :? LineCurve as l -> Some l.Line
        | crv ->
            if crv.IsLinear(State.Doc.ModelAbsoluteTolerance) then Some <| Line(crv.PointAtStart, crv.PointAtEnd)
            else None

    /// <summary>Attempt to get a Line from a curve object that is linear within the current Documents Absolute Tolerance.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>A Geometry.Line Option. None if the object does not exist, is not a curve, or is not linear.</returns>
    static member TryCoerceLine(objectId:Guid) : Line option =
        match RhinoScriptSyntax.TryCoerceGeometry objectId with
        | Some (:? Curve as crv) -> RhinoScriptSyntax.TryCoerceLine(crv)
        | _ -> None

    /// <summary>Gets a Line from its start and end point.</summary>
    /// <param name="startPoint">(Point3d) The start point of the line.</param>
    /// <param name="endPoint">(Point3d) The end point of the line.</param>
    /// <returns>A Geometry.Line Option, always Some.</returns>
    static member TryCoerceLine(startPoint:Point3d, endPoint:Point3d) : Line option =
        Some <| Line(startPoint, endPoint)

    /// <summary>Returns the given Line. This overload exists so that CoerceLine accepts a Line too.</summary>
    /// <param name="line">(Line) The line.</param>
    /// <returns>(Geometry.Line) The same line.</returns>
    static member CoerceLine(line:Line) : Line =
        line

    /// <summary>Gets a Line from a Curve that is linear within the current Documents Absolute Tolerance.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>(Geometry.Line) The line. Raises a RhinoScriptingException if the curve is null or not linear.</returns>
    static member CoerceLine(curve:Curve) : Line =
        match RhinoScriptSyntax.TryCoerceLine(curve) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "CoerceLine failed on: %s " (Pretty.str curve)

    /// <summary>Gets a Line from a curve object that is linear within the current Documents Absolute Tolerance.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>(Geometry.Line) The line. Raises a RhinoScriptingException if the object does not exist, is not a curve, or is not linear.</returns>
    static member CoerceLine(objectId:Guid) : Line =
        match RhinoScriptSyntax.TryCoerceLine(objectId) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "CoerceLine failed on: %s " (Pretty.str objectId)

    /// <summary>Gets a Line from its start and end point.</summary>
    /// <param name="startPoint">(Point3d) The start point of the line.</param>
    /// <param name="endPoint">(Point3d) The end point of the line.</param>
    /// <returns>(Geometry.Line) The line.</returns>
    static member CoerceLine(startPoint:Point3d, endPoint:Point3d) : Line =
        Line(startPoint, endPoint)

    /// <summary>Returns the given Arc. This overload exists so that TryCoerceArc accepts an Arc too.</summary>
    /// <param name="arc">(Arc) The arc.</param>
    /// <returns>A Geometry.Arc Option, always Some.</returns>
    static member TryCoerceArc(arc:Arc) : Arc option =
        Some arc

    /// <summary>Attempt to get an Arc from a Curve using the current Documents Absolute Tolerance.
    /// Does not return circles as arcs.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>A Geometry.Arc Option. None if the curve is null or not an arc.</returns>
    static member TryCoerceArc(curve:Curve) : Arc option =
        if isNull curve then
            None
        else
            let a = ref (new Arc())
            if curve.TryGetArc(a, State.Doc.ModelAbsoluteTolerance) then Some a.Value
            else None

    /// <summary>Attempt to get an Arc from a curve object using the current Documents Absolute Tolerance.
    /// Does not return circles as arcs.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>A Geometry.Arc Option. None if the object does not exist, is not a curve, or is not an arc.</returns>
    static member TryCoerceArc(objectId:Guid) : Arc option =
        match RhinoScriptSyntax.TryCoerceGeometry objectId with
        | Some (:? Curve as crv) -> RhinoScriptSyntax.TryCoerceArc(crv)
        | _ -> None

    /// <summary>Returns the given Arc. This overload exists so that CoerceArc accepts an Arc too.</summary>
    /// <param name="arc">(Arc) The arc.</param>
    /// <returns>(Geometry.Arc) The same arc.</returns>
    static member CoerceArc(arc:Arc) : Arc =
        arc

    /// <summary>Gets an Arc from a Curve using the current Documents Absolute Tolerance.
    /// Does not return circles as arcs.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>(Geometry.Arc) The arc. Raises a RhinoScriptingException if the curve is null or not an arc.</returns>
    static member CoerceArc(curve:Curve) : Arc =
        match RhinoScriptSyntax.TryCoerceArc(curve) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "CoerceArc failed on: %s " (Pretty.str curve)

    /// <summary>Gets an Arc from a curve object using the current Documents Absolute Tolerance.
    /// Does not return circles as arcs.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>(Geometry.Arc) The arc. Raises a RhinoScriptingException if the object does not exist, is not a curve, or is not an arc.</returns>
    static member CoerceArc(objectId:Guid) : Arc =
        match RhinoScriptSyntax.TryCoerceArc(objectId) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "CoerceArc failed on: %s " (Pretty.str objectId)

    /// <summary>Returns the given Circle. This overload exists so that TryCoerceCircle accepts a Circle too.</summary>
    /// <param name="circle">(Circle) The circle.</param>
    /// <returns>A Geometry.Circle Option, always Some.</returns>
    static member TryCoerceCircle(circle:Circle) : Circle option =
        Some circle

    /// <summary>Attempt to get a Circle from a Curve using the current Documents Absolute Tolerance.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>A Geometry.Circle Option. None if the curve is null or not a circle.</returns>
    static member TryCoerceCircle(curve:Curve) : Circle option =
        if isNull curve then
            None
        else
            let a = ref (new Circle())
            if curve.TryGetCircle(a, State.Doc.ModelAbsoluteTolerance) then Some a.Value
            else None

    /// <summary>Attempt to get a Circle from a curve object using the current Documents Absolute Tolerance.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>A Geometry.Circle Option. None if the object does not exist, is not a curve, or is not a circle.</returns>
    static member TryCoerceCircle(objectId:Guid) : Circle option =
        match RhinoScriptSyntax.TryCoerceGeometry objectId with
        | Some (:? Curve as crv) -> RhinoScriptSyntax.TryCoerceCircle(crv)
        | _ -> None

    /// <summary>Returns the given Circle. This overload exists so that CoerceCircle accepts a Circle too.</summary>
    /// <param name="circle">(Circle) The circle.</param>
    /// <returns>(Geometry.Circle) The same circle.</returns>
    static member CoerceCircle(circle:Circle) : Circle =
        circle

    /// <summary>Gets a Circle from a Curve using the current Documents Absolute Tolerance.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>(Geometry.Circle) The circle. Raises a RhinoScriptingException if the curve is null or not a circle.</returns>
    static member CoerceCircle(curve:Curve) : Circle =
        match RhinoScriptSyntax.TryCoerceCircle(curve) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "CoerceCircle failed on: %s " (Pretty.str curve)

    /// <summary>Gets a Circle from a curve object using the current Documents Absolute Tolerance.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>(Geometry.Circle) The circle. Raises a RhinoScriptingException if the object does not exist, is not a curve, or is not a circle.</returns>
    static member CoerceCircle(objectId:Guid) : Circle =
        match RhinoScriptSyntax.TryCoerceCircle(objectId) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "CoerceCircle failed on: %s " (Pretty.str objectId)

    /// <summary>Returns the given Ellipse. This overload exists so that TryCoerceEllipse accepts an Ellipse too.</summary>
    /// <param name="ellipse">(Ellipse) The ellipse.</param>
    /// <returns>A Geometry.Ellipse Option, always Some.</returns>
    static member TryCoerceEllipse(ellipse:Ellipse) : Ellipse option =
        Some ellipse

    /// <summary>Attempt to get an Ellipse from a Curve using the current Documents Absolute Tolerance.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>A Geometry.Ellipse Option. None if the curve is null or not an ellipse.</returns>
    static member TryCoerceEllipse(curve:Curve) : Ellipse option =
        if isNull curve then
            None
        else
            let a = ref (new Ellipse())
            if curve.TryGetEllipse(a, State.Doc.ModelAbsoluteTolerance) then Some a.Value
            else None

    /// <summary>Attempt to get an Ellipse from a curve object using the current Documents Absolute Tolerance.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>A Geometry.Ellipse Option. None if the object does not exist, is not a curve, or is not an ellipse.</returns>
    static member TryCoerceEllipse(objectId:Guid) : Ellipse option =
        match RhinoScriptSyntax.TryCoerceGeometry objectId with
        | Some (:? Curve as crv) -> RhinoScriptSyntax.TryCoerceEllipse(crv)
        | _ -> None

    /// <summary>Returns the given Ellipse. This overload exists so that CoerceEllipse accepts an Ellipse too.</summary>
    /// <param name="ellipse">(Ellipse) The ellipse.</param>
    /// <returns>(Geometry.Ellipse) The same ellipse.</returns>
    static member CoerceEllipse(ellipse:Ellipse) : Ellipse =
        ellipse

    /// <summary>Gets an Ellipse from a Curve using the current Documents Absolute Tolerance.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>(Geometry.Ellipse) The ellipse. Raises a RhinoScriptingException if the curve is null or not an ellipse.</returns>
    static member CoerceEllipse(curve:Curve) : Ellipse =
        match RhinoScriptSyntax.TryCoerceEllipse(curve) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "CoerceEllipse failed on: %s " (Pretty.str curve)

    /// <summary>Gets an Ellipse from a curve object using the current Documents Absolute Tolerance.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>(Geometry.Ellipse) The ellipse. Raises a RhinoScriptingException if the object does not exist, is not a curve, or is not an ellipse.</returns>
    static member CoerceEllipse(objectId:Guid) : Ellipse =
        match RhinoScriptSyntax.TryCoerceEllipse(objectId) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "CoerceEllipse failed on: %s " (Pretty.str objectId)

    /// <summary>Returns the given Polyline. This overload exists so that TryCoercePolyline accepts a Polyline too.</summary>
    /// <param name="polyline">(Polyline) The polyline.</param>
    /// <returns>A Geometry.Polyline Option, always Some.</returns>
    static member TryCoercePolyline(polyline:Polyline) : Polyline option =
        Some polyline

    /// <summary>Attempt to get a Polyline from a Curve.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>A Geometry.Polyline Option. None if the curve is null or not a polyline.</returns>
    static member TryCoercePolyline(curve:Curve) : Polyline option =
        match curve with
        | null -> None
        | :? PolylineCurve as crv -> Some <| crv.ToPolyline()
        | crv ->
            let a : ref<Polyline> = ref null
            if crv.TryGetPolyline(a) then Some a.Value
            else None

    /// <summary>Attempt to get a Polyline from a curve object.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>A Geometry.Polyline Option. None if the object does not exist, is not a curve, or is not a polyline.</returns>
    static member TryCoercePolyline(objectId:Guid) : Polyline option =
        match RhinoScriptSyntax.TryCoerceGeometry objectId with
        | Some (:? Curve as crv) -> RhinoScriptSyntax.TryCoercePolyline(crv)
        | _ -> None

    /// <summary>Returns the given Polyline. This overload exists so that CoercePolyline accepts a Polyline too.</summary>
    /// <param name="polyline">(Polyline) The polyline.</param>
    /// <returns>(Geometry.Polyline) The same polyline.</returns>
    static member CoercePolyline(polyline:Polyline) : Polyline =
        polyline

    /// <summary>Gets a Polyline from a Curve.</summary>
    /// <param name="curve">(Curve) The curve.</param>
    /// <returns>(Geometry.Polyline) The polyline. Raises a RhinoScriptingException if the curve is null or not a polyline.</returns>
    static member CoercePolyline(curve:Curve) : Polyline =
        match RhinoScriptSyntax.TryCoercePolyline(curve) with
        | Some pl -> pl
        | None -> RhinoScriptingException.Raise "CoercePolyline failed on: %s " (Pretty.str curve)

    /// <summary>Gets a Polyline from a curve object.</summary>
    /// <param name="objectId">(Guid) The identifier of a curve object.</param>
    /// <returns>(Geometry.Polyline) The polyline. Raises a RhinoScriptingException if the object does not exist, is not a curve, or is not a polyline.</returns>
    static member CoercePolyline(objectId:Guid) : Polyline =
        match RhinoScriptSyntax.TryCoercePolyline(objectId) with
        | Some pl -> pl
        | None -> RhinoScriptingException.Raise "CoercePolyline failed on: %s " (Pretty.str objectId)


    //----------------------------------------------------
    //---------------Coerce only (no TryCoerce) ----------  TODO: add TryCoerce ?
    //----------------------------------------------------


    /// <summary>Attempt to get Rhino LayerObject from the document for a given full name.</summary>
    /// <param name="name">(string) The layer's name.</param>
    /// <returns>DocObjects.Layer </returns>
    static member CoerceLayer (name:string) : DocObjects.Layer =
        let i = State.Doc.Layers.FindByFullPath(name, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then
            let lay = State.Doc.Layers.FindName name
            if isNull lay then
                RhinoScriptingException.Raise "CoerceLayer: could not find a layer named '%s'" name
            else
                lay
        else
            State.Doc.Layers.[i]


    /// <summary>Attempt to get Rhino LayerObject from the document with a given objectId.</summary>
    /// <param name="layerId">(Guid) The layer's Guid.</param>
    /// <returns>DocObjects.Layer</returns>
    static member CoerceLayer (layerId:Guid) : DocObjects.Layer=
        if layerId = Guid.Empty then RhinoScriptingException.Raise "CoerceLayer: input Guid is Guid.Empty"
        let l = State.Doc.Layers.FindId(layerId)
        if isNull l then
            if notNull (State.Doc.Objects.FindId(layerId)) then
                RhinoScriptingException.Raise "CoerceLayer works on  Guid of a Layer Object, not the Guid of a Document Object (with Geometry) '%s'" (Pretty.str layerId)
            else
                RhinoScriptingException.Raise "CoerceLayer: could not find Guid %O in State.Doc.Layer table'" layerId
        l

    /// <summary>Returns the Rhino Block instance object for a given Id.</summary>
    /// <param name="objectId">(Guid) Id of block instance</param>
    /// <returns>(DocObjects.InstanceObject) block instance object.</returns>
    static member CoerceBlockInstanceObject(objectId:Guid) : DocObjects.InstanceObject =
        match RhinoScriptSyntax.CoerceRhinoObject(objectId) with
        | :? DocObjects.InstanceObject as b -> b
        | _ -> RhinoScriptingException.Raise "CoerceBlockInstanceObject: unable to coerce Block InstanceObject from '%s'" (Pretty.str objectId)



    //-------------------views ---------------------


    /// <summary>Attempt to get a Rhino View from its Guid, it can be a standard or page view.</summary>
    /// <param name="viewId">(Guid) The Guid of the view.</param>
    /// <returns>A Display.RhinoView Option. None if no view has this Guid.</returns>
    static member TryCoerceView (viewId:Guid) : Option<Display.RhinoView> =
        match State.Doc.Views.Find(viewId) with
        | null -> None
        | viewObj -> Some viewObj

    /// <summary>Attempt to get a Rhino View from its name, it can be a standard or page view.</summary>
    /// <param name="viewName">(string) The name of the view.</param>
    /// <returns>A Display.RhinoView Option. None if the name is null or no view has this name.</returns>
    static member TryCoerceView (viewName:string) : Option<Display.RhinoView> =
        if isNull viewName then
            None
        else
            #if RH7
            State.Doc.Views.GetViewList(includeStandardViews=true, includePageViews=true)
            #else
            State.Doc.Views.GetViewList(Display.ViewTypeFilter.ModelStyleViews ||| Display.ViewTypeFilter.Page)
            #endif
            |> Array.tryFind (fun v -> v.MainViewport.Name = viewName)


    /// <summary>Gets a Rhino View from its Guid, it can be a standard or page view.</summary>
    /// <param name="viewId">(Guid) The Guid of the view.</param>
    /// <returns>(Display.RhinoView) The view. Raises a RhinoScriptingException if no view has this Guid.</returns>
    static member CoerceView (viewId:Guid) : Display.RhinoView =
        match State.Doc.Views.Find(viewId) with
        | null -> RhinoScriptingException.Raise "CoerceView: could not CoerceView from '%O'" viewId
        | viewObj -> viewObj

    /// <summary>Gets a Rhino View from its name, it can be a standard or page view.</summary>
    /// <param name="viewName">(string) The name of the view.</param>
    /// <returns>(Display.RhinoView) The view. Raises a RhinoScriptingException if the name is null or no view has this name.</returns>
    static member CoerceView (viewName:string) : Display.RhinoView =
        if isNull viewName then
            RhinoScriptingException.Raise "CoerceView: failed on null for view name input" // or State.Doc.Views.ActiveView
        else
            #if RH7
            State.Doc.Views.GetViewList(includeStandardViews=true, includePageViews=true)
            #else
            State.Doc.Views.GetViewList(Display.ViewTypeFilter.ModelStyleViews ||| Display.ViewTypeFilter.Page) // ModelStyleViews, see: https://github.com/mcneel/rhinoscriptsyntax/blob/86c9ab1e97df384662a8548b4accee45dfd77a8c/Scripts/rhinoscript/view.py#L26
            #endif
            |> Array.tryFind (fun v -> v.MainViewport.Name = viewName)
            |> Option.defaultWith (fun () ->  RhinoScriptingException.Raise "CoerceView: could not find view called '%s'" viewName)


    /// <summary>Gets a Rhino Page (or Layout) View from its Guid.</summary>
    /// <param name="viewId">(Guid) The Guid of the page view.</param>
    /// <returns>(Display.RhinoPageView) The page view. Raises a RhinoScriptingException if no view has this Guid or if the view is not a page view.</returns>
    static member CoercePageView (viewId:Guid) : Display.RhinoPageView =
        match State.Doc.Views.Find(viewId) with
        | null -> RhinoScriptingException.Raise "CoercePageView: could not CoerceView from '%O'" viewId
        | :? Display.RhinoPageView as page -> page
        | view -> RhinoScriptingException.Raise "CoercePageView: the view found '%s' is not a page view" view.MainViewport.Name

    /// <summary>Gets a Rhino Page (or Layout) View from the name of the Layout.</summary>
    /// <param name="pageName">(string) The name of the Layout.</param>
    /// <returns>(Display.RhinoPageView) The page view. Raises a RhinoScriptingException if the name is null, or if not exactly one Layout has this name.</returns>
    static member CoercePageView (pageName:string) : Display.RhinoPageView =
        if isNull pageName then RhinoScriptingException.Raise "CoercePageView: failed on null for view name input" // or State.Doc.Views.ActiveView
        let allViews = State.Doc.Views.GetPageViews()|> Array.filter (fun v-> v.PageName = pageName)
        if allViews.Length = 1 then allViews.[0]
        elif allViews.Length > 1 then RhinoScriptingException.Raise "CoercePageView: more than one page called '%s'" pageName
        else  RhinoScriptingException.Raise "CoercePageView: Layout called '%s' not found" pageName

    /// <summary>Attempt to get Detail view rectangle Geometry.</summary>
    /// <param name="objectId">(Guid) objectId of Detail object</param>
    /// <returns>A Geometry.DetailView. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceDetailView (objectId:Guid) : DetailView =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  DetailView as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceDetailView failed on %s"  (Pretty.str objectId)

    /// <summary>Attempt to get Detail view rectangle Object.</summary>
    /// <param name="objectId">(Guid) objectId of Detail object</param>
    /// <returns>A DocObjects.DetailViewObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceDetailViewObject (objectId:Guid) : DocObjects.DetailViewObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.DetailViewObject as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceDetailViewObject failed on %s"  (Pretty.str objectId)


    //-------Annotation ----


    /// <summary>Attempt to get TextDot Geometry.</summary>
    /// <param name="objectId">(Guid) objectId of TextDot object</param>
    /// <returns>A Geometry.TextDot. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceTextDot (objectId:Guid) : TextDot =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  TextDot as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceTextDot failed on: %s " (Pretty.str objectId)

    /// <summary>Attempt to get TextEntity Geometry (for the text Object use rs.CoerceTextObject) .</summary>
    /// <param name="objectId">(Guid) objectId of TextEntity object</param>
    /// <returns>A Geometry.TextEntity. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceTextEntity (objectId:Guid) : TextEntity =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  TextEntity as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceTextEntity failed on: %s " (Pretty.str objectId)


    /// <summary>Attempt to get Rhino TextObject Annotation Object.</summary>
    /// <param name="objectId">(Guid) objectId of TextObject</param>
    /// <returns>(DocObjects.TextObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceTextObject (objectId:Guid) : DocObjects.TextObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.TextObject as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceTextObject failed on: %s " (Pretty.str objectId)

    /// <summary>Attempt to get Hatch Geometry.</summary>
    /// <param name="objectId">(Guid) objectId of Hatch object</param>
    /// <returns>A Geometry.Hatch. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceHatch (objectId:Guid) : Hatch =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  Hatch as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceHatch failed on: %s " (Pretty.str objectId)


    /// <summary>Attempt to get Rhino Hatch Object.</summary>
    /// <param name="objectId">(Guid) objectId of Hatch object</param>
    /// <returns>(DocObjects.HatchObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceHatchObject (objectId:Guid) : DocObjects.HatchObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.HatchObject as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceHatchObject failed on: %s " (Pretty.str objectId)

    /// <summary>Attempt to get Rhino Annotation Base Object.</summary>
    /// <param name="objectId">(Guid) objectId of annotation object</param>
    /// <returns>(DocObjects.AnnotationObjectBase. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceAnnotation (objectId:Guid) : DocObjects.AnnotationObjectBase =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.AnnotationObjectBase as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceAnnotation failed on: %s " (Pretty.str objectId)


    /// <summary>Attempt to get Rhino Leader Annotation Object.</summary>
    /// <param name="objectId">(Guid) objectId of Leader object</param>
    /// <returns>(DocObjects.LeaderObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceLeader (objectId:Guid) : DocObjects.LeaderObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.LeaderObject as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceLeader failed on: %s " (Pretty.str objectId)

    /// <summary>Attempt to get Rhino LinearDimension Annotation Object.</summary>
    /// <param name="objectId">(Guid) objectId of LinearDimension object</param>
    /// <returns>(DocObjects.LinearDimensionObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceLinearDimension (objectId:Guid) : DocObjects.LinearDimensionObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.LinearDimensionObject as a -> a
        | _ -> RhinoScriptingException.Raise "CoerceLinearDimension failed on: %s " (Pretty.str objectId)

    //---------Geometry ------------


    /// <summary>Returns the given Point3d. This overload exists so that Coerce3dPoint accepts a Point3d too.</summary>
    /// <param name="point">(Point3d) The point.</param>
    /// <returns>(Point3d) The same point.</returns>
    static member Coerce3dPoint(point:Point3d) : Point3d =
        point

    /// <summary>Converts a Point3f into a Rhino.Geometry.Point3d.</summary>
    /// <param name="point">(Point3f) The single precision point.</param>
    /// <returns>(Point3d) The double precision point.</returns>
    static member Coerce3dPoint(point:Point3f) : Point3d =
        Point3d(point)

    /// <summary>Converts three floats into a Rhino.Geometry.Point3d.</summary>
    /// <param name="x">(float) The X coordinate.</param>
    /// <param name="y">(float) The Y coordinate.</param>
    /// <param name="z">(float) The Z coordinate.</param>
    /// <returns>(Point3d) The point.</returns>
    static member Coerce3dPoint(x:float, y:float, z:float) : Point3d =
        Point3d(x, y, z)

    /// <summary>Converts a sequence of exactly three floats into a Rhino.Geometry.Point3d.</summary>
    /// <param name="xyz">(float seq) The X, Y and Z coordinates.</param>
    /// <returns>(Point3d) The point. Raises a RhinoScriptingException if the sequence is null or does not have exactly three items.</returns>
    static member Coerce3dPoint(xyz:seq<float>) : Point3d =
        if isNull xyz then RhinoScriptingException.Raise "Coerce3dPoint failed on null sequence"
        match Array.ofSeq xyz with
        | [| x; y; z |] -> Point3d(x, y, z)
        | a -> RhinoScriptingException.Raise "Coerce3dPoint: a sequence of exactly 3 floats is needed, not %d: %s" a.Length (Pretty.str xyz)

    /// <summary>Parses a string into a Rhino.Geometry.Point3d.</summary>
    /// <param name="point">(string) Three numbers separated by commas, like "1, 2, 3",
    ///    or by semicolons if the decimal separator is a comma, like "1,5; 2,5; 3,5".</param>
    /// <returns>(Point3d) The point. Raises a RhinoScriptingException if the string can not be parsed.</returns>
    static member Coerce3dPoint(point:string) : Point3d =
        match tryParseXYZ point with
        | Some (x, y, z) -> Point3d(x, y, z)
        | None -> RhinoScriptingException.Raise "Coerce3dPoint: could not parse the string '%s' into a Point3d" point

    /// <summary>Gets the location of a point object in the document.</summary>
    /// <param name="objectId">(Guid) The identifier of a point object.</param>
    /// <returns>(Point3d) The location of the point. Raises a RhinoScriptingException if the object does not exist or is not a point.</returns>
    static member Coerce3dPoint(objectId:Guid) : Point3d =
        match RhinoScriptSyntax.CoerceGeometry(objectId) with
        | :? Point as p -> p.Location
        | g -> RhinoScriptingException.Raise "Coerce3dPoint: the object %s is a %s, not a point" (Pretty.str objectId) (g.GetType().Name)

    /// <summary>Gets the location of a PointObject.</summary>
    /// <param name="pointObject">(DocObjects.PointObject) The point object.</param>
    /// <returns>(Point3d) The location of the point. Raises a RhinoScriptingException if the PointObject is null.</returns>
    static member Coerce3dPoint(pointObject:DocObjects.PointObject) : Point3d =
        if isNull pointObject then RhinoScriptingException.Raise "Coerce3dPoint failed on null PointObject"
        pointObject.PointGeometry.Location

    /// <summary>Gets the location of a TextDot.</summary>
    /// <param name="textDot">(TextDot) The TextDot.</param>
    /// <returns>(Point3d) The location of the TextDot. Raises a RhinoScriptingException if the TextDot is null.</returns>
    static member Coerce3dPoint(textDot:TextDot) : Point3d =
        if isNull textDot then RhinoScriptingException.Raise "Coerce3dPoint failed on null TextDot"
        textDot.Point

    /// <summary>Attempt to get Rhino Point Object.</summary>
    /// <param name="objectId">(Guid) objectId of Point object</param>
    /// <returns>A DocObjects.PointObject, Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoercePointObject (objectId:Guid) : DocObjects.PointObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.PointObject as a -> a
        | _ -> RhinoScriptingException.Raise "CoercePointObject failed on: %s " (Pretty.str objectId)

    /// <summary>Returns the given Point2d. This overload exists so that Coerce2dPoint accepts a Point2d too.</summary>
    /// <param name="point">(Point2d) The point.</param>
    /// <returns>(Point2d) The same point.</returns>
    static member Coerce2dPoint(point:Point2d) : Point2d =
        point

    /// <summary>Converts a Point3d into a Rhino.Geometry.Point2d by dropping its Z coordinate.</summary>
    /// <param name="point">(Point3d) The 3D point.</param>
    /// <returns>(Point2d) The 2D point.</returns>
    static member Coerce2dPoint(point:Point3d) : Point2d =
        Point2d(point.X, point.Y)

    /// <summary>Converts two floats into a Rhino.Geometry.Point2d.</summary>
    /// <param name="x">(float) The X coordinate.</param>
    /// <param name="y">(float) The Y coordinate.</param>
    /// <returns>(Point2d) The 2D point.</returns>
    static member Coerce2dPoint(x:float, y:float) : Point2d =
        Point2d(x, y)

    /// <summary>Returns the given Vector3d. This overload exists so that Coerce3dVector accepts a Vector3d too.</summary>
    /// <param name="vector">(Vector3d) The vector.</param>
    /// <returns>(Vector3d) The same vector.</returns>
    static member Coerce3dVector(vector:Vector3d) : Vector3d =
        vector

    /// <summary>Converts a Vector3f into a Rhino.Geometry.Vector3d.</summary>
    /// <param name="vector">(Vector3f) The single precision vector.</param>
    /// <returns>(Vector3d) The double precision vector.</returns>
    static member Coerce3dVector(vector:Vector3f) : Vector3d =
        Vector3d(vector)

    /// <summary>Converts three floats into a Rhino.Geometry.Vector3d.</summary>
    /// <param name="x">(float) The X component.</param>
    /// <param name="y">(float) The Y component.</param>
    /// <param name="z">(float) The Z component.</param>
    /// <returns>(Vector3d) The vector.</returns>
    static member Coerce3dVector(x:float, y:float, z:float) : Vector3d =
        Vector3d(x, y, z)

    /// <summary>Converts a sequence of exactly three floats into a Rhino.Geometry.Vector3d.</summary>
    /// <param name="xyz">(float seq) The X, Y and Z components.</param>
    /// <returns>(Vector3d) The vector. Raises a RhinoScriptingException if the sequence is null or does not have exactly three items.</returns>
    static member Coerce3dVector(xyz:seq<float>) : Vector3d =
        if isNull xyz then RhinoScriptingException.Raise "Coerce3dVector failed on null sequence"
        match Array.ofSeq xyz with
        | [| x; y; z |] -> Vector3d(x, y, z)
        | a -> RhinoScriptingException.Raise "Coerce3dVector: a sequence of exactly 3 floats is needed, not %d: %s" a.Length (Pretty.str xyz)

    /// <summary>Parses a string into a Rhino.Geometry.Vector3d.</summary>
    /// <param name="vector">(string) Three numbers separated by commas, like "1, 2, 3",
    ///    or by semicolons if the decimal separator is a comma, like "1,5; 2,5; 3,5".</param>
    /// <returns>(Vector3d) The vector. Raises a RhinoScriptingException if the string can not be parsed.</returns>
    static member Coerce3dVector(vector:string) : Vector3d =
        match tryParseXYZ vector with
        | Some (x, y, z) -> Vector3d(x, y, z)
        | None -> RhinoScriptingException.Raise "Coerce3dVector: could not parse the string '%s' into a Vector3d" vector

    /// <summary>Gets the direction of a linear curve object in the document, from its start to its end point.</summary>
    /// <param name="lineId">(Guid) The identifier of a curve object that is linear within the current Documents Absolute Tolerance.</param>
    /// <returns>(Vector3d) The direction of the line, its length is the length of the line.
    ///    Raises a RhinoScriptingException if the object does not exist, is not a curve, or is not linear.</returns>
    static member Coerce3dVector(lineId:Guid) : Vector3d =
        RhinoScriptSyntax.CoerceLine(lineId).Direction



    /// <summary>Returns the given Plane. This overload exists so that CoercePlane accepts a Plane too.</summary>
    /// <param name="plane">(Plane) The plane.</param>
    /// <returns>(Plane) The same plane.</returns>
    static member CoercePlane(plane:Plane) : Plane =
        plane

    /// <summary>Creates a Plane parallel to the World XY plane at the given origin.</summary>
    /// <param name="origin">(Point3d) The origin of the plane.</param>
    /// <returns>(Plane) A plane with the axes of the World XY plane.</returns>
    static member CoercePlane(origin:Point3d) : Plane =
        let mutable pl = Plane.WorldXY
        pl.Origin <- origin
        pl

    /// <summary>Creates a Plane parallel to the World XY plane at the given origin.</summary>
    /// <param name="x">(float) The X coordinate of the origin.</param>
    /// <param name="y">(float) The Y coordinate of the origin.</param>
    /// <param name="z">(float) The Z coordinate of the origin.</param>
    /// <returns>(Plane) A plane with the axes of the World XY plane.</returns>
    static member CoercePlane(x:float, y:float, z:float) : Plane =
        RhinoScriptSyntax.CoercePlane(Point3d(x, y, z))

    /// <summary>Returns the given Transform. This overload exists so that CoerceXform accepts a Transform too.</summary>
    /// <param name="xForm">(Transform) The transformation matrix.</param>
    /// <returns>(Transform) The same transformation matrix.</returns>
    static member CoerceXform(xForm:Transform) : Transform =
        xForm

    /// <summary>Converts a 4x4 matrix of floats into a Rhino.Geometry.Transform Transformation Matrix.</summary>
    /// <param name="xForm">(float seq seq) The matrix given row by row, for example as a list of lists or an array of arrays.
    ///    Smaller or larger matrices are not accepted.</param>
    /// <returns>(Transform) The transformation matrix. Raises a RhinoScriptingException if the input is not a 4x4 matrix.</returns>
    static member CoerceXform(xForm:seq<#seq<float>>) : Transform =
        if isNull xForm then RhinoScriptingException.Raise "CoerceXform failed on null"
        let rows = xForm |> Seq.map (fun row -> if isNull (box row) then [||] else Array.ofSeq row) |> Array.ofSeq
        if rows.Length <> 4 || rows |> Array.exists (fun r -> r.Length <> 4) then
            RhinoScriptingException.Raise "CoerceXform: %s is not a 4x4 matrix, it can not be converted to a Transformation Matrix" (Pretty.str xForm)
        let mutable t = Transform()
        for row = 0 to 3 do
            for col = 0 to 3 do
                t.[row, col] <- rows.[row].[col]
        t

    /// <summary>Converts a 4x4 matrix of floats into a Rhino.Geometry.Transform Transformation Matrix.</summary>
    /// <param name="xForm">(float[,]) The matrix as a 2D array, the first index is the row.
    ///    Smaller or larger matrices are not accepted.</param>
    /// <returns>(Transform) The transformation matrix. Raises a RhinoScriptingException if the input is not a 4x4 matrix.</returns>
    static member CoerceXform(xForm:float[,]) : Transform =
        if isNull xForm then RhinoScriptingException.Raise "CoerceXform failed on null"
        if Array2D.length1 xForm <> 4 || Array2D.length2 xForm <> 4 then
            RhinoScriptingException.Raise "CoerceXform: Array2D %s is not a 4x4 matrix, it can not be converted to a Transformation Matrix" (Pretty.str xForm)
        let mutable t = Transform()
        for row = 0 to 3 do
            for col = 0 to 3 do
                t.[row, col] <- xForm.[Array2D.base1 xForm + row, Array2D.base2 xForm + col]
        t



    /// <summary>Attempt to get Surface geometry from the document with a given objectId.</summary>
    /// <param name="objectId">the object's Identifier</param>
    /// <returns>(Rhino.Geometry.Surface. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceNurbsSurface(objectId:Guid) : NurbsSurface =
        match RhinoScriptSyntax.CoerceGeometry(objectId) with
        | :? NurbsSurface as s -> s
        | :? Surface as c -> c.ToNurbsSurface()
        | :? Brep as b ->
            if b.Faces.Count = 1 then (b.Faces.[0] :> Surface).ToNurbsSurface()
            else RhinoScriptingException.Raise "CoerceNurbsSurface failed on %s from Brep with %d Faces" (Pretty.str objectId)   b.Faces.Count
        | _ -> RhinoScriptingException.Raise "CoerceNurbsSurface failed on: %O "  objectId



    /// <summary>Attempt to get Rhino PointCloud Geometry.</summary>
    /// <param name="objectId">(Guid) objectId of PointCloud object</param>
    /// <returns>A Geometry.PointCloud. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoercePointCloud (objectId:Guid) : PointCloud =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  PointCloud as a -> a
        | _ -> RhinoScriptingException.Raise "CoercePointCloud failed on: %s " (Pretty.str objectId)


    /// <summary>Converts a System.Drawing.Color into its ARGB form, so that it is not a named color any more.</summary>
    /// <param name="color">(Drawing.Color) The color.</param>
    /// <returns>(Drawing.Color) The color in ARGB form (not as named color) this will provide better comparison to other colors.
    /// For example the named color Red is not equal to fromRGB(255, 0, 0).</returns>
    static member CoerceColor(color:Drawing.Color) : Drawing.Color =
        Drawing.Color.FromArgb(int color.A, int color.R, int color.G, int color.B) //https://stackoverflow.com/questions/20994753/compare-two-color-objects

    /// <summary>Creates a System.Drawing.Color from red, green and blue values.</summary>
    /// <param name="red">(int) Red value from 0 to 255.</param>
    /// <param name="green">(int) Green value from 0 to 255.</param>
    /// <param name="blue">(int) Blue value from 0 to 255.</param>
    /// <returns>(Drawing.Color) The opaque color in ARGB form (not as named color).
    ///    Raises a RhinoScriptingException if a value is not between 0 and 255.</returns>
    static member CoerceColor(red:int, green:int, blue:int) : Drawing.Color =
        if red  <0 || red  >255 then RhinoScriptingException.Raise "CoerceColor: cannot create color from red %d, green %d and blue %d" red green blue
        if green<0 || green>255 then RhinoScriptingException.Raise "CoerceColor: cannot create color from red %d, green %d and blue %d" red green blue
        if blue <0 || blue >255 then RhinoScriptingException.Raise "CoerceColor: cannot create color from red %d, green %d and blue %d" red green blue
        Drawing.Color.FromArgb( red, green, blue)

    /// <summary>Creates a System.Drawing.Color from red, green, blue and alpha values.
    ///    Like in RhinoPython, alpha is the last argument, not the first one as in Drawing.Color.FromArgb.</summary>
    /// <param name="red">(int) Red value from 0 to 255.</param>
    /// <param name="green">(int) Green value from 0 to 255.</param>
    /// <param name="blue">(int) Blue value from 0 to 255.</param>
    /// <param name="alpha">(int) Alpha value from 0 (transparent) to 255 (opaque).</param>
    /// <returns>(Drawing.Color) The color in ARGB form (not as named color).
    ///    Raises a RhinoScriptingException if a value is not between 0 and 255.</returns>
    static member CoerceColor(red:int, green:int, blue:int, alpha:int) : Drawing.Color =
        if red  <0 || red  >255 then RhinoScriptingException.Raise "CoerceColor: cannot create color from red %d, green %d, blue %d and alpha %d" red green blue alpha
        if green<0 || green>255 then RhinoScriptingException.Raise "CoerceColor: cannot create color from red %d, green %d, blue %d and alpha %d" red green blue alpha
        if blue <0 || blue >255 then RhinoScriptingException.Raise "CoerceColor: cannot create color from red %d, green %d, blue %d and alpha %d" red green blue alpha
        if alpha<0 || alpha>255 then RhinoScriptingException.Raise "CoerceColor: cannot create color from red %d, green %d, blue %d and alpha %d" red green blue alpha
        Drawing.Color.FromArgb(alpha, red, green, blue)

    /// <summary>Parses a System.Drawing.Color from a string, also works on natural language color strings, see Drawing.ColorTranslator.FromHtml.</summary>
    /// <param name="color">(string) A color name like "Red", or a hex string like "#FF0000".</param>
    /// <returns>(Drawing.Color) The color in ARGB form (not as named color) this will provide better comparison to other colors.
    /// For example the named color Red is not equal to fromRGB(255, 0, 0). Raises a RhinoScriptingException if the string can not be parsed.</returns>
    static member CoerceColor(color:string) : Drawing.Color =
        try
            let c = Drawing.ColorTranslator.FromHtml(color)
            Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
        with _ ->
            //try
            //    let c = Drawing.Color.FromName(s) // for invalid names ( a hex string) this still returns named color black !!
            //    Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
            //with _ ->
                RhinoScriptingException.Raise "CoerceColor:: could not Coerce '%s' to a Color" color

            //     try Windows.Media.ColorConverter.ConvertFromString(s)
            //     with _ -> RhinoScriptingException.Raise ": could not Coerce %A to a Color" c

    //<summary>Attempt to get a Sequence of Guids from input</summary>
    //<param name="Ids">list of Guids</param>
    //<returns>Guid seq) Fails on bad input</returns>
    //static member CoerceGuidList(Ids:'T) : seq<Guid> =
    //    match box Ids with
    //    | :? Guid  as g -> if Guid.Empty = g then fail() else [|g|] :> seq<Guid>
    //    | :? seq<obj> as gs ->
    //                        try
    //                            gs |> Seq.map RhinoScriptSyntax.CoerceGuid
    //                        with _ ->
    //                            RhinoScriptingException.Raise ": could not CoerceGuidList: %A can not be converted to a Sequence(IEnumerable) of Guids" Ids
    //    | _ -> RhinoScriptingException.Raise "CoerceGuidList: could not CoerceGuidList: %A can not be converted to a Sequence(IEnumerable) of Guids" Ids

        //<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible</summary>
    //<param name="points">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    //<returns>(Rhino.Geometry.Point3d seq) Fails on bad input</returns>
    //static member Coerce3dPointList(points:'T) : Point3d seq=
    //    try Seq.map RhinoScriptSyntax.Coerce3dPoint points //|> Seq.cache
    //    with _ -> RhinoScriptingException.Raise "Coerce3dPointList: could not Coerce: Could not convert %A to a list of 3d points"  points

    //<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible</summary>
    //<param name="points">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    //<returns>(Rhino.Geometry.Point2d seq) Fails on bad input</returns>
    //static member Coerce2dPointList(points:'T) : Point2d seq=
    //    try Seq.map RhinoScriptSyntax.Coerce2dPoint points //|> Seq.cache
    //    with _ -> RhinoScriptingException.Raise "Coerce2dPointList: could not Coerce: Could not convert %A to a list of 2d points"  points

