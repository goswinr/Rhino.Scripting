
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenCoerce =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    /// TODO all the functions taking a generic paramnter should not be called coerce but rather try convert ?? no ?
    /// TODO none of the coerce function should take generic paramter, just a few overloads


  //---------------------------------------------------
  //-----------------Coerce and TryCoerce pairs -------
  //---------------------------------------------------



    ///<summary>Attempt to get a Guids from input.</summary>
    ///<param name="objectId">objcts , Guid or string</param>
    ///<returns>a Guid Option.</returns>
    static member TryCoerceGuid (objectId:'T) : Guid option= 
        match box objectId with
        | :? Guid  as g -> if Guid.Empty = g then None else Some g
        | :? DocObjects.RhinoObject as o -> Some o.Id
        | :? DocObjects.ObjRef      as o -> Some o.ObjectId
        | :? string  as s -> let ok, g = Guid.TryParse s in  if ok then Some g else None
        | _ -> None

    ///<summary>Attempt to get a Guids from input.</summary>
    ///<param name="input">objects , Guid or string</param>
    ///<returns>Guid) Fails on bad input.</returns>
    static member CoerceGuid(input:'T) : Guid = 
        match box input with
        | :? Guid  as g -> if Guid.Empty = g then RhinoScriptingException.Raise "Rhino.Scripting.CoerceGuid: Guid is Empty"  else g
        | :? Option<Guid>  as go -> if go.IsNone || Guid.Empty = go.Value then RhinoScriptingException.Raise "Rhino.Scripting.CoerceGuid: Guid is Empty or None: %A" input else go.Value //from UI functions
        | :? string  as s -> try Guid.Parse s with _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceGuid: string '%s' can not be converted to a Guid" s
        | :? DocObjects.RhinoObject as o -> o.Id
        | :? DocObjects.ObjRef      as o -> o.ObjectId
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceGuid: %A can not be converted to a Guid" input


    ///<summary>Attempt to get RhinoObject from the document with a given objectId.</summary>
    ///<param name="objectId">object Identifier (Guid or string)</param>
    ///<returns>a RhinoObject Option.</returns>
    static member TryCoerceRhinoObject (objectId:Guid) : DocObjects.RhinoObject option = 
        match State.Doc.Objects.FindId(objectId) with
        | null -> None
        | o    -> Some o        
    
    ///<summary>Attempt to get RhinoObject from the document with a given objectId.</summary>
    ///<param name="objectId">(Guid) Object Identifier </param>
    ///<returns>a RhinoObject, Fails on bad input.</returns>
    static member CoerceRhinoObject(objectId:Guid) : DocObjects.RhinoObject = 
       match Scripting.TryCoerceRhinoObject objectId with 
       |Some o -> o
       |None -> 
            if Guid.Empty = objectId then    RhinoScriptingException.Raise "Rhino.Scripting.CoerceRhinoObject failed on empty Guid"
            else                             RhinoScriptingException.Raise "Rhino.Scripting.CoerceRhinoObject: The Guid %O was not found in the Current Object table." objectId

    ///<summary>Attempt to get GeometryBase class from given Guid. Fails on empty Guid.</summary>   
    ///<param name="objectId">geometry Identifier (Guid)</param>
    ///<returns>a Rhino.Geometry.GeometryBase Option</returns>
    static member TryCoerceGeometry (objectId:Guid) :GeometryBase option = 
        match State.Doc.Objects.FindId(objectId) with
        | null -> None
        | o -> Some o.Geometry
    
    ///<summary>Attempt to get GeometryBase class from given input.</summary>
    ///<param name="objectId">(Guid) geometry Identifier </param>
    ///<returns>(Rhino.Geometry.GeometryBase) Fails on bad input.</returns>
    static member CoerceGeometry(objectId:Guid) : GeometryBase =         
        if Guid.Empty = objectId then RhinoScriptingException.Raise "Rhino.Scripting.CoerceGeometry failed on empty Guid"
        let o = State.Doc.Objects.FindId(objectId)
        if isNull o then RhinoScriptingException.Raise "Rhino.Scripting.CoerceGeometry failed: Guid %O not found in Object table." objectId
        o.Geometry

    ///<summary>Attempt to get Rhino LightObject from the document with a given objectId.</summary>
    ///<param name="objectId">(Guid) light Identifier</param>
    ///<returns>a Rhino.Geometry.Light. Option.</returns>
    static member TryCoerceLight (objectId:Guid) : Light option = 
        match Scripting.CoerceGeometry objectId with
        | :? Geometry.Light as l -> Some l
        | _ -> None

    ///<summary>Attempt to get Rhino LightObject from the document with a given objectId.</summary>
    ///<param name="objectId">(Guid) light Identifier</param>
    ///<returns>a  Rhino.Geometry.Light) Fails on bad input.</returns>
    static member CoerceLight (objectId:Guid) : Light = 
        match Scripting.CoerceGeometry objectId with
        | :? Geometry.Light as l -> l
        | g -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceLight failed on: %s " (Print.guid objectId)


    ///<summary>Attempt to get Mesh class from given Guid. Fails on empty Guid.</summary>
    ///<param name="objectId">Mesh Identifier (Guid)</param>
    ///<returns>a Rhino.Geometry.Surface Option.</returns>
    static member TryCoerceMesh (objectId:Guid) : Mesh option = 
        match State.Doc.Objects.FindId(objectId) with
            | null -> None
            | o ->
                match o.Geometry with
                | :? Mesh as m -> Some m
                | _ -> None

    ///<summary>Attempt to get Mesh geometry from the document with a given objectId.</summary>
    ///<param name="objectId">object Identifier (Guid or string)</param>
    ///<returns>(Rhino.Geometry.Mesh) Fails on bad input.</returns>
    static member CoerceMesh(objectId:Guid) : Mesh = 
        match Scripting.TryCoerceMesh(objectId) with
        | Some m -> m
        | None -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceMesh failed on: %s " (Print.guid objectId)

    ///<summary>Attempt to get Surface class from given Guid. Fails on empty Guid.</summary>
    ///<param name="objectId">Surface Identifier (Guid)</param>
    ///<returns>a Rhino.Geometry.Surface Option.</returns>
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
    
    ///<summary>Attempt to get Surface geometry from the document with a given objectId.</summary>
    ///<param name="objectId">the object's Identifier</param>
    ///<returns>(Rhino.Geometry.Surface) Fails on bad input.</returns>
    static member CoerceSurface(objectId:Guid) : Surface = 
        match Scripting.CoerceGeometry(objectId) with
        | :? Surface as c -> c
        | :? Brep as b ->
            if b.Faces.Count = 1 then b.Faces.[0] :> Surface
            else RhinoScriptingException.Raise "Rhino.Scripting.CoerceSurface failed on %O from Brep with %d Faces" objectId b.Faces.Count
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceSurface failed on: %O " objectId

    ///<summary>Attempt to get a Polysurface or Brep class from given Guid. Works on Extrusions too. Fails on empty Guid.</summary>
    ///<param name="objectId">Polysurface Identifier (Guid)</param>
    ///<returns>a Rhino.Geometry.Mesh Option.</returns>
    static member TryCoerceBrep (objectId:Guid) : Brep option =
        match State.Doc.Objects.FindId(objectId) with
            | null -> None
            | o ->
                match o.Geometry with
                | :? Brep as b ->  Some b
                | :? Extrusion as b -> Some (b.ToBrep(true))
                | _ -> None
                
    ///<summary>Attempt to get Polysurface geometry from the document with a given objectId.</summary>
    ///<param name="objectId">objectId (Guid or string) to be Scripting.Coerced into a brep</param>
    ///<returns>(Rhino.Geometry.Brep) Fails on bad input.</returns>
    static member CoerceBrep(objectId:Guid) : Brep  = 
        match Scripting.TryCoerceBrep(objectId) with
        | Some b -> b
        | None -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceBrep failed on: %s " (Print.guid objectId)

    ///<summary>Attempt to get Curve geometry from the document with a given objectId.</summary>
    ///<param name="objectId">objectId (Guid or string) to be Scripting.Coerced into a Curve</param>
    ///<param name="segmentIndex">(int) Optional, index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<returns>a Rhino.Geometry.Curve Option.</returns>
    static member TryCoerceCurve(objectId:Guid,[<OPT;DEF(-1)>]segmentIndex:int) : Curve option = 
        let geo = Scripting.CoerceGeometry objectId
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

    ///<summary>Attempt to get Curve geometry from the document with a given objectId.</summary>
    ///<param name="objectId">objectId (Guid or string) to be Scripting.Coerced into a Curve</param>
    ///<param name="segmentIndex">(int) Optional, index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<returns>(Rhino.Geometry.Curve) Fails on bad input.</returns>
    static member CoerceCurve(objectId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Curve = 
        if segmentIndex < 0 then
            match Scripting.CoerceGeometry(objectId) with
            | :? Curve as c -> c
            | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceCurve failed on: %s " (Print.guid objectId)
        else
            match Scripting.CoerceGeometry(objectId) with
            | :? PolyCurve as c ->
                let crv = c.SegmentCurve(segmentIndex)
                if isNull crv then RhinoScriptingException.Raise "Rhino.Scripting.CoerceCurve failed on segment index %d for %s" segmentIndex  (Print.guid objectId)
                crv
            | :? Curve as c -> c
            | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceCurve failed for %s"  (Print.guid objectId)


    ///<summary>Attempt to get Rhino Line Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="line">Line, two points or Guid</param>
    ///<returns>Geometry.Line) Fails on bad input.</returns>
    static member TryCoerceLine(line:'T) : Line option = 
        match box line with
        | :? Line as l -> Some l
        | :? Curve as crv ->
            if crv.IsLinear(State.Doc.ModelAbsoluteTolerance) then Some <|  Line(crv.PointAtStart, crv.PointAtEnd)
                else None
        | :? Guid as g ->
            match State.Doc.Objects.FindId(g).Geometry with
            | :? LineCurve as l -> Some l.Line
            | :? Curve as crv ->
                if crv.IsLinear(State.Doc.ModelAbsoluteTolerance) then Some <| Line(crv.PointAtStart, crv.PointAtEnd)
                else None
            | _ -> None
        | :? (Point3d*Point3d) as ab -> let a, b = ab in Some <| Line(a, b)
        |_ -> None
    
    ///<summary>Attempt to get Rhino Line Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="line">Line, two points or Guid</param>
    ///<returns>Geometry.Line, Fails on bad input.</returns>
    static member CoerceLine(line:'T) : Line= 
        match Scripting.TryCoerceLine(line) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceLine failed on: %s " (Print.nice line)

    ///<summary>Attempt to get Rhino Arc Geometry using the current Documents Absolute Tolerance.
    /// does not return circles as arcs.</summary>
    ///<param name="arc">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Arc Option.</returns>
    static member TryCoerceArc(arc:'T) : Arc option= 
        match box arc with
        | :? Arc as a -> Some(a)
        | :? Curve as crv ->
            let a = ref (new Arc())
            let ok = crv.TryGetArc(a,State.Doc.ModelAbsoluteTolerance)
            if ok then Some(!a )
            else None
        | :? Guid as g ->
            match State.Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a = ref (new Arc())
                let ok = crv.TryGetArc(a,State.Doc.ModelAbsoluteTolerance)
                if ok then Some(!a )
                else None
            | _ -> None
        |_ -> None

    ///<summary>Attempt to get Rhino Arc Geometry using the current Documents Absolute Tolerance.
    /// does not return circles as arcs.</summary>
    ///<param name="arc">Guid, RhinoObject or Curve </param>
    ///<returns>Geometry.Arc, Fails on bad input.</returns>
    static member CoerceArc(arc:'T) : Arc= 
        match Scripting.TryCoerceArc(arc) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceArc failed on: %s " (Print.nice arc)


    ///<summary>Attempt to get Rhino Circle Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="cir">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Circle Option.</returns>
    static member TryCoerceCircle(cir:'T) : Circle option= 
        match box cir with
        | :? Circle as a -> Some(a)
        | :? Curve as crv ->
            let a = ref (new Circle())
            let ok = crv.TryGetCircle(a,State.Doc.ModelAbsoluteTolerance)
            if ok then Some(!a )
            else None
        | :? Guid as g ->
            match State.Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a = ref (new Circle())
                let ok = crv.TryGetCircle(a,State.Doc.ModelAbsoluteTolerance)
                if ok then Some(!a )
                else None
            | _ -> None
        |_ -> None

    ///<summary>Attempt to get Rhino Circle Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="circ">Guid, RhinoObject or Curve </param>
    ///<returns>Geometry.Circle, Fails on bad input.</returns>
    static member CoerceCircle(circ:'T) : Circle= 
        match Scripting.TryCoerceCircle(circ) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceCircle failed on: %s " (Print.nice circ)

    ///<summary>Attempt to get Rhino Ellipse Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="cir">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Ellipse Option.</returns>
    static member TryCoerceEllipse(cir:'T) : Ellipse option= 
        match box cir with
        | :? Ellipse as a -> Some(a)
        | :? Curve as crv ->
            let a = ref (new Ellipse())
            let ok = crv.TryGetEllipse(a,State.Doc.ModelAbsoluteTolerance)
            if ok then Some(!a )
            else None
        | :? Guid as g ->
            match State.Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a = ref (new  Ellipse())
                let ok = crv.TryGetEllipse(a,State.Doc.ModelAbsoluteTolerance)
                if ok then Some(!a )
                else None
            | _ -> None
        |_ -> None

    ///<summary>Attempt to get Rhino Ellips Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="ellip">Guid, RhinoObject or Curve </param>
    ///<returns>Geometry.Ellipse, Fails on bad input.</returns>
    static member CoerceEllips(ellip:'T) : Ellipse= 
        match Scripting.TryCoerceEllipse(ellip) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceEllipse failed on: %s " (Print.nice ellip)

    //----------------------------------------------------
    //---------------Coerce only (no TryCoerce) ----------  TODO: add TryCorece ?
    //----------------------------------------------------



    ///<summary>Attempt to get Rhino LayerObject from the document for a given fullame.</summary>
    ///<param name="name">(string) The layer's name.</param>
    ///<returns>DocObjectys.Layer </returns>
    static member CoerceLayer (name:string) : DocObjects.Layer = 
        let i = State.Doc.Layers.FindByFullPath(name, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then
            let lay = State.Doc.Layers.FindName name
            if isNull lay then
                RhinoScriptingException.Raise "Rhino.Scripting.CoerceLayer: could find name '%s'" name
            else
                lay
        else
            State.Doc.Layers.[i]


    ///<summary>Attempt to get Rhino LayerObject from the document with a given objectId.</summary>
    ///<param name="layerId">(Guid) The layer's Guid.</param>
    ///<returns>DocObjectys.Layer</returns>
    static member CoerceLayer (layerId:Guid) : DocObjects.Layer= 
        if layerId = Guid.Empty then RhinoScriptingException.Raise "Rhino.Scripting.CoerceLayer: input Guid is Guid.Empty" 
        let l = State.Doc.Layers.FindId(layerId)
        if isNull l then
            if notNull (State.Doc.Objects.FindId(layerId)) then
                RhinoScriptingException.Raise "Rhino.Scripting.CoerceLayer works on  Guid of a Layer Object, not the Guid of a Documnt Object (with Geometry) '%s'" (Print.guid layerId)
            else
                RhinoScriptingException.Raise "Rhino.Scripting.CoerceLayer: could not find Guid %O in State.Doc.Layer table'" layerId
        l

    ///<summary>Returns the Rhino Block instance object for a given Id.</summary>
    ///<param name="objectId">(Guid) Id of block instance</param>
    ///<returns>(DocObjects.InstanceObject) block instance object.</returns>
    static member CoerceBlockInstanceObject(objectId:Guid) : DocObjects.InstanceObject = 
        match Scripting.CoerceRhinoObject(objectId) with
        | :? DocObjects.InstanceObject as b -> b
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceBlockInstanceObject: unable to coerce Block InstanceObject from '%s'" (Print.guid objectId)



    //-------------------views ---------------------

    ///<summary>Attempt to get Rhino View Object from the name of the view, can be a standart or page view.</summary>
    ///<param name="nameOrId">(string or Guid) Name or Guid the view, empty string will return the Active view</param>
    ///<returns>a State.Doc.View object) Fails on bad input.</returns>
    static member CoerceView (nameOrId:'T) : Display.RhinoView = 
        match box nameOrId with
        | :? Guid as g ->
            let viewo = State.Doc.Views.Find(g)
            if isNull viewo then RhinoScriptingException.Raise "Rhino.Scripting.CoerceView: could not CoerceView  from '%O'" g
            else viewo

        | :? string as view ->
            if isNull view then RhinoScriptingException.Raise "Rhino.Scripting.CoerceView: failed on null for view name input" // or State.Doc.Views.ActiveView
            elif view = "" then State.Doc.Views.ActiveView
            else
                let allviews = 
                    State.Doc.Views.GetViewList(includeStandardViews=true, includePageViews=true)
                    |> Array.filter (fun v-> v.MainViewport.Name = view)
                if allviews.Length = 1 then allviews.[0]
                else  RhinoScriptingException.Raise "Rhino.Scripting.CoerceView: could not CoerceView '%s'" view

        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.Cannot get view from %O" nameOrId


    ///<summary>Attempt to get Rhino Page (or Layout) View Object from the name of the Layout.</summary>
    ///<param name="nameOrId">(string) Name of the Layout</param>
    ///<returns>a State.Doc.View object) Fails on bad input.</returns>
    static member CoercePageView (nameOrId:'T) : Display.RhinoPageView = 
        match box nameOrId with
        | :? Guid as g ->
            let viewo = State.Doc.Views.Find(g)
            if isNull viewo then RhinoScriptingException.Raise "Rhino.Scripting.CoerceView: could not CoerceView  from '%O'" g
            else
                try viewo :?> Display.RhinoPageView
                with _  -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceView: the view found '%s' is not a page view" viewo.MainViewport.Name

        | :? string as view ->
            if isNull view then RhinoScriptingException.Raise "Rhino.Scripting.CoercePageView: failed on null for view name input" // or State.Doc.Views.ActiveView
            else
                let allviews = State.Doc.Views.GetPageViews()|> Array.filter (fun v-> v.PageName = view)
                let l = allviews.Length
                if allviews.Length = 1 then allviews.[0]
                elif allviews.Length > 1 then RhinoScriptingException.Raise "Rhino.Scripting.CoercePageView: more than one page called '%s'" view
                else  RhinoScriptingException.Raise "Rhino.Scripting.CoercePageView: Layout called '%s' not found" view

        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.Cannot get view from %O" nameOrId

    ///<summary>Attempt to get Detail view rectangle Geometry.</summary>
    ///<param name="objectId">(Guid) objectId of Detail object</param>
    ///<returns>a Geometry.DetailView) Fails on bad input.</returns>
    static member CoerceDetailView (objectId:Guid) : DetailView = 
        match Scripting.CoerceGeometry objectId with
        | :?  DetailView as a -> a
        | g -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceDetailView failed on %s"  (Print.guid objectId)

    ///<summary>Attempt to get Detail view rectangle Object.</summary>
    ///<param name="objectId">(Guid) objectId of Detail object</param>
    ///<returns>a DocObjects.DetailViewObject) Fails on bad input.</returns>
    static member CoerceDetailViewObject (objectId:Guid) : DocObjects.DetailViewObject = 
        match Scripting.CoerceRhinoObject objectId with
        | :?  DocObjects.DetailViewObject as a -> a
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceDetailViewObject failed on %s"  (Print.guid objectId)


    //-------Annotation ----


    ///<summary>Attempt to get TextDot Geometry.</summary>
    ///<param name="objectId">(Guid) objectId of TextDot object</param>
    ///<returns>a Geometry.TextDot) Fails on bad input.</returns>
    static member CoerceTextDot (objectId:Guid) : TextDot = 
        match Scripting.CoerceGeometry objectId with
        | :?  TextDot as a -> a
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceTextDot failed on: %s " (Print.guid objectId)

    ///<summary>Attempt to get TextEntity Geometry (for the text Object use rs.CoerceTextObject) .</summary>
    ///<param name="objectId">(Guid) objectId of TextEntity object</param>
    ///<returns>a Geometry.TextEntity) Fails on bad input.</returns>
    static member CoerceTextEntity (objectId:Guid) : TextEntity = 
        match Scripting.CoerceGeometry objectId with
        | :?  TextEntity as a -> a
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceTextEntity failed on: %s " (Print.guid objectId)


    ///<summary>Attempt to get Rhino TextObject Annotation Object.</summary>
    ///<param name="objectId">(Guid) objectId of TextObject</param>
    ///<returns>(DocObjects.TextObject) Fails on bad input.</returns>
    static member CoerceTextObject (objectId:Guid) : DocObjects.TextObject = 
        match Scripting.CoerceRhinoObject objectId with
        | :?  DocObjects.TextObject as a -> a
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceTextObject failed on: %s " (Print.guid objectId)

    ///<summary>Attempt to get Hatch Geometry.</summary>
    ///<param name="objectId">(Guid) objectId of Hatch object</param>
    ///<returns>a Geometry.CoerceHatch) Fails on bad input.</returns>
    static member CoerceHatch (objectId:Guid) : Hatch = 
        match Scripting.CoerceGeometry objectId with
        | :?  Hatch as a -> a
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceHatch failed on: %s " (Print.guid objectId)


    ///<summary>Attempt to get Rhino Hatch Object.</summary>
    ///<param name="objectId">(Guid) objectId of Hatch object</param>
    ///<returns>(DocObjects.HatchObject) Fails on bad input.</returns>
    static member CoerceHatchObject (objectId:Guid) : DocObjects.HatchObject = 
        match Scripting.CoerceRhinoObject objectId with
        | :?  DocObjects.HatchObject as a -> a
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceHatchObject failed on: %s " (Print.guid objectId)

    ///<summary>Attempt to get Rhino Annotation Base Object.</summary>
    ///<param name="objectId">(Guid) objectId of annotation object</param>
    ///<returns>(DocObjects.AnnotationObjectBase) Fails on bad input.</returns>
    static member CoerceAnnotation (objectId:Guid) : DocObjects.AnnotationObjectBase = 
        match Scripting.CoerceRhinoObject objectId with
        | :?  DocObjects.AnnotationObjectBase as a -> a
        | o -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceAnnotation failed on: %s " (Print.guid objectId)


    ///<summary>Attempt to get Rhino Leader Annotation Object.</summary>
    ///<param name="objectId">(Guid) objectId of Leader object</param>
    ///<returns>(DocObjects.LeaderObject) Fails on bad input.</returns>
    static member CoerceLeader (objectId:Guid) : DocObjects.LeaderObject = 
        match Scripting.CoerceRhinoObject objectId with
        | :?  DocObjects.LeaderObject as a -> a
        | o -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceLeader failed on: %s " (Print.guid objectId)

    ///<summary>Attempt to get Rhino LinearDimension Annotation Object.</summary>
    ///<param name="objectId">(Guid) objectId of LinearDimension object</param>
    ///<returns>(DocObjects.LinearDimensionObject) Fails on bad input.</returns>
    static member CoerceLinearDimension (objectId:Guid) : DocObjects.LinearDimensionObject = 
        match Scripting.CoerceRhinoObject objectId with
        | :?  DocObjects.LinearDimensionObject as a -> a
        | o -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceLinearDimension failed on: %s " (Print.guid objectId)

    //---------Geometry ------------
    

    ///<summary>Converts input into a Rhino.Geometry.Point3d if possible.</summary>
    ///<param name="pt">Input to convert, Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    ///<returns>a Rhino.Geometry.Point3d, Fails on bad input.</returns>
    static member Coerce3dPoint(pt:'T) : Point3d = 
        let inline  point3dOf3(x:^x, y:^y, z:^z) = 
            try Point3d(float (x), float(y), float(z))
            with _ -> RhinoScriptingException.Raise "Rhino.Scripting.Coerce3dPoint: Could not Coerce the 3 values %O, %O and %O to a Point3d" x y z
        let b = box pt
        match b with
        | :? Point3d    as pt                 -> pt
        | :? Point3f    as pt                 -> Point3d(pt)
        //| :? Vector3d   as v                  -> Point3d(v) //dont confuse vectors and points !
        | :? DocObjects.PointObject as po     -> po.PointGeometry.Location
        | :? TextDot as td                    -> td.Point
        | :? (float*float*float) as xyz       -> let x, y, z = xyz in Point3d(x, y, z)
        | :? (decimal*decimal*decimal) as xyz -> let x, y, z = xyz in Point3d(float(x), float(y), float(z))
        | :? (single*single*single) as xyz    -> let x, y, z = xyz in Point3d(float(x), float(y), float(z))
        | :? (int*int*int) as xyz             -> let x, y, z = xyz in Point3d(float(x), float(y), float(z))
        | _ ->
            try
                match b with
                | :? (Point3d option) as pto   -> pto.Value // from UI function
                | :? option<Guid> as go      -> ((State.Doc.Objects.FindId(go.Value).Geometry) :?> Point).Location
                | :? (string*string*string) as xyz  -> let x, y, z = xyz in Point3d(parseFloatEnDe(x), parseFloatEnDe(y), parseFloatEnDe(z))
                | :? Guid as g ->  ((State.Doc.Objects.FindId(g).Geometry) :?> Point).Location
                | :? seq<float>  as xyz  ->  point3dOf3(Seq.item 0 xyz, Seq.item 3 xyz, Seq.item 2 xyz)
                | :? seq<int>  as xyz  ->    point3dOf3(Seq.item 0 xyz, Seq.item 3 xyz, Seq.item 2 xyz)
                | :? seq<string> as xyz  ->  point3dOf3(Seq.item 0 xyz, Seq.item 3 xyz, Seq.item 2 xyz)
                | :? string as s  ->
                    let xs = s.Split(';')
                    if Seq.length xs > 2 then
                        Point3d(parseFloatEnDe(Seq.item 0 xs), parseFloatEnDe(Seq.item 1 xs), parseFloatEnDe(Seq.item 2 xs))
                    else
                        let ys = s.Split(',')
                        Point3d(parseFloatEnDe(Seq.item 0 ys), parseFloatEnDe(Seq.item 1 ys), parseFloatEnDe(Seq.item 2 ys))
                |_ -> RhinoScriptingException.Raise "Rhino.Scripting.Coerce3dPoint failed on: %s " (Print.nice pt)
            with _ ->
                RhinoScriptingException.Raise "Rhino.Scripting.Coerce3dPoint failed on: %s " (Print.nice pt)

    ///<summary>Attempt to get Rhino Point Object.</summary>
    ///<param name="objectId">(Guid) objectId of Point object</param>
    ///<returns>a DocObjects.PointObject, Fails on bad input.</returns>
    static member CoercePointObject (objectId:Guid) : DocObjects.PointObject = 
        match Scripting.CoerceRhinoObject objectId with
        | :?  DocObjects.PointObject as a -> a
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoercePointObject failed on: %s " (Print.guid objectId)

    ///<summary>Convert input into a Rhino.Geometry.Point2d if possible.</summary>
    ///<param name="point">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    ///<returns>a Rhino.Geometry.Point2d, Fails on bad input.</returns>
    static member Coerce2dPoint(point:'T) : Point2d = 
        match box point with
        | :? Point2d    as point -> point
        | :? Point3d    as point -> Point2d(point.X, point.Y)
        | :? (float*float) as xy  -> let x, y = xy in Point2d(x, y)
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.Coerce2dPoint: could not Coerce: Could not convert %A to a Point2d"  point

    ///<summary>Convert input into a Rhino.Geometry.Vector3d if possible.</summary>
    ///<param name="vec">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    ///<returns> a Rhino.Geometry.Vector3d, Fails on bad input.</returns>
    static member Coerce3dVector(vec:'T) : Vector3d = 
        let inline vecOf3(x:^x, y:^y, z:^z) = 
            try Vector3d(float (x), float(y), float(z))
            with _ -> RhinoScriptingException.Raise "Rhino.Scripting.Coerce3dVector: Could not Coerce %O, %O and %O to Vector3d" x y z
        let b = box vec
        match b with
        | :? Vector3d   as v                  -> v
        | :? Vector3f   as v                  -> Vector3d(v)
        | :? (float*float*float) as xyz       -> let x, y, z = xyz in Vector3d(x, y, z)
        | :? (decimal*decimal*decimal) as xyz -> let x, y, z = xyz in Vector3d(float(x), float(y), float(z))
        | :? (single*single*single) as xyz    -> let x, y, z = xyz in Vector3d(float(x), float(y), float(z))
        | :? (int*int*int) as xyz             -> let x, y, z = xyz in Vector3d(float(x), float(y), float(z))
        | _ ->
            try
                match b with
                | :? Guid as g ->  (State.Doc.Objects.FindId(g).Geometry :?> LineCurve).Line.Direction
                | :? (Vector3d option) as v   -> v.Value // from UI function
                | :? option<Guid> as go      -> (State.Doc.Objects.FindId(go.Value).Geometry :?> LineCurve).Line.Direction
                | :? (string*string*string) as xyz  -> let x, y, z = xyz in Vector3d(parseFloatEnDe(x), parseFloatEnDe(y), parseFloatEnDe(z))
                | :? seq<float>  as xyz  ->  vecOf3(Seq.item 0 xyz, Seq.item 3 xyz, Seq.item 2 xyz)
                | :? seq<int>  as xyz  ->    vecOf3(Seq.item 0 xyz, Seq.item 3 xyz, Seq.item 2 xyz)
                | :? seq<string> as xyz  ->  vecOf3(Seq.item 0 xyz, Seq.item 3 xyz, Seq.item 2 xyz)
                | :? string as s  ->
                    let xs = s.Split(';')
                    if Seq.length xs > 2 then
                        Vector3d(parseFloatEnDe(Seq.item 0 xs), parseFloatEnDe(Seq.item 1 xs), parseFloatEnDe(Seq.item 2 xs))
                    else
                        let ys = s.Split(',')
                        Vector3d(parseFloatEnDe(Seq.item 0 ys), parseFloatEnDe(Seq.item 1 ys), parseFloatEnDe(Seq.item 2 ys))
                |_ -> RhinoScriptingException.Raise "Rhino.Scripting.Coerce3dVector failed on: %s " (Print.nice vec)
            with _ ->
                RhinoScriptingException.Raise "Rhino.Scripting.Coerce3dVector failed on: %s " (Print.nice vec)



    ///<summary>Convert input into a Rhino.Geometry.Plane if possible.</summary>
    ///<param name="plane">Plane, point, list, tuple</param>
    ///<returns>(Rhino.Geometry.Plane) Fails on bad input.</returns>
    static member CoercePlane(plane:'T) : Plane = 
        match box plane with
        | :? Plane  as plane -> plane
        | _ ->
            try
                let pt = Scripting.Coerce3dPoint(plane)
                let mutable pl = Plane.WorldXY
                pl.Origin <- pt
                pl
            with e ->
                RhinoScriptingException.Raise "Rhino.Scripting.CoercePlane failed on: %s " (Print.nice plane)

    ///<summary>Convert input into a Rhino.Geometry.Transform Transformation Matrix if possible.</summary>
    ///<param name="xForm">object to convert</param>
    ///<returns>(Rhino.Geometry.Transform) Fails on bad input.</returns>
    static member CoerceXform(xForm:'T) : Transform = 
        match box xForm with
        | :? Transform  as xForm -> xForm
        | :? seq<seq<float>>  as xss -> // TODO verify row, column order !!
                let mutable t= Transform()
                try
                    for c, xs in Seq.indexed xss do
                        for r, x in Seq.indexed xs do
                            t.[c, r] <- x
                with
                    | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceXform: seq<seq<float>> %s can not be converted to a Transformation Matrix" (toNiceString xForm)
                t
        | :? ``[,]``<float>  as xss -> // TODO verify row, column order !!
                let mutable t= Transform()
                try
                    xss|> Array2D.iteri (fun i j x -> t.[i, j]<-x)
                with
                    | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceXform: Array2D %s can not be converted to a Transformation Matrix" (toNiceString xForm)
                t
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceXform: could not CoerceXform %s can not be converted to a Transformation Matrix" (toNiceString xForm)
    


    ///<summary>Attempt to get Surface geometry from the document with a given objectId.</summary>
    ///<param name="objectId">the object's Identifier</param>
    ///<returns>(Rhino.Geometry.Surface) Fails on bad input.</returns>
    static member CoerceNurbsSurface(objectId:Guid) : NurbsSurface = 
        match Scripting.CoerceGeometry(objectId) with
        | :? NurbsSurface as s -> s
        | :? Surface as c -> c.ToNurbsSurface()
        | :? Brep as b ->
            if b.Faces.Count = 1 then (b.Faces.[0] :> Surface).ToNurbsSurface()
            else RhinoScriptingException.Raise "Rhino.Scripting.CoerceNurbsSurface failed on %s from Brep with %d Faces" (Print.guid objectId)   b.Faces.Count
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceNurbsSurface failed on: %O "  objectId



    ///<summary>Attempt to get Rhino PointCloud Geometry.</summary>
    ///<param name="objectId">(Guid) objectId of PointCloud object</param>
    ///<returns>a Geometry.PointCloud) Fails on bad input.</returns>
    static member CoercePointCloud (objectId:Guid) : PointCloud = 
        match Scripting.CoerceGeometry objectId with
        | :?  PointCloud as a -> a
        | g -> RhinoScriptingException.Raise "Rhino.Scripting.CoercePointCloud failed on: %s " (Print.guid objectId)
            
    
    ///<summary>Attempt to get a System.Drawing.Color also works on natrural language color strings see Drawing.ColorTranslator.FromHtml.</summary>
    ///<param name="color">string, tuple with  or 3 or 4 items</param>
    ///<returns>System.Drawing.Color in ARGB form (not as named color) this will provIde better comparison to other colors.
    /// For example the named color Red is not equal to fromRGB(255, 0, 0)) Fails on bad input.</returns>
    static member CoerceColor(color:'T) : Drawing.Color = 
        match box color with
        | :? Drawing.Color  as c -> Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B) //https://stackoverflow.com/questions/20994753/compare-two-color-objects
        | :? (int*int*int) as rgb       ->
            let red , green, blue   = rgb
            if red  <0 || red  >255 then RhinoScriptingException.Raise "Rhino.Scripting.CoerceColor: cannot create color form red %d, blue %d and green %d" red green blue
            if green<0 || green>255 then RhinoScriptingException.Raise "Rhino.Scripting.CoerceColor: cannot create color form red %d, blue %d and green %d" red green blue
            if blue <0 || blue >255 then RhinoScriptingException.Raise "Rhino.Scripting.CoerceColor: cannot create color form red %d, blue %d and green %d" red green blue
            Drawing.Color.FromArgb( red, green, blue)

        | :? (int*int*int*int) as argb  ->
            let alpha, red , green, blue   = argb
            if red  <0 || red  >255 then RhinoScriptingException.Raise "Rhino.Scripting.CoerceColor: cannot create color form red %d, blue %d and green  %d aplpha %d" red green blue alpha
            if green<0 || green>255 then RhinoScriptingException.Raise "Rhino.Scripting.CoerceColor: cannot create color form red %d, blue %d and green  %d aplpha %d" red green blue alpha
            if blue <0 || blue >255 then RhinoScriptingException.Raise "Rhino.Scripting.CoerceColor: cannot create color form red %d, blue %d and green  %d aplpha %d" red green blue alpha
            if alpha<0 || alpha >255 then RhinoScriptingException.Raise "Rhino.Scripting.CoerceColor: cannot create color form red %d, blue %d and green %d aplpha %d" red green blue alpha
            Drawing.Color.FromArgb(alpha, red, green, blue)
        | :? string  as s ->
            try
                let c = Drawing.Color.FromName(s)
                Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
            with _ ->
                try
                    let c = Drawing.ColorTranslator.FromHtml(s)
                    Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
                with _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceColor:: could not Coerce %A to a Color" color
              //     try Windows.Media.ColorConverter.ConvertFromString(s)
              //     with _ -> RhinoScriptingException.Raise "Rhino.Scripting.: could not Coerce %A to a Color" c
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceColor:: could not Coerce %A to a Color" color

    //<summary>Attempt to get a Sequence of Guids from input</summary>
    //<param name="Ids">list of Guids</param>
    //<returns>Guid seq) Fails on bad input</returns>
    //static member CoerceGuidList(Ids:'T) : seq<Guid> = 
    //    match box Ids with
    //    | :? Guid  as g -> if Guid.Empty = g then fail() else [|g|] :> seq<Guid>
    //    | :? seq<obj> as gs ->
    //                        try
    //                            gs |> Seq.map Scripting.CoerceGuid
    //                        with _ ->
    //                            RhinoScriptingException.Raise "Rhino.Scripting.: could not CoerceGuidList: %A can not be converted to a Sequence(IEnumerable) of Guids" Ids
    //    | _ -> RhinoScriptingException.Raise "Rhino.Scripting.CoerceGuidList: could not CoerceGuidList: %A can not be converted to a Sequence(IEnumerable) of Guids" Ids

        //<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible</summary>
    //<param name="ponits">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    //<returns>(Rhino.Geometry.Point3d seq) Fails on bad input</returns>
    //static member Coerce3dPointList(points:'T) : Point3d seq= 
    //    try Seq.map Scripting.Coerce3dPoint points //|> Seq.cache
    //    with _ -> RhinoScriptingException.Raise "Rhino.Scripting.Coerce3dPointList: could not Coerce: Could not convert %A to a list of 3d points"  points

    //<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible</summary>
    //<param name="ponits">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    //<returns>(Rhino.Geometry.Point2d seq) Fails on bad input</returns>
    //static member Coerce2dPointList(points:'T) : Point2d seq= 
    //    try Seq.map Scripting.Coerce2dPoint points //|> Seq.cache
    //    with _ -> RhinoScriptingException.Raise "Rhino.Scripting.Coerce2dPointList: could not Coerce: Could not convert %A to a list of 2d points"  points

