
namespace Rhino.Scripting 

open Rhino 

open System

open Rhino.Geometry

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenCoerce =
  type RhinoScriptSyntax with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    // TODO all the functions taking a generic parameter should not be called coerce but rather try convert ?? no ?
    // TODO none of the coerce function should take generic parameter, just a few overloads


    //---------------------------------------------------
    //-----------------Coerce and TryCoerce pairs -------
    //---------------------------------------------------



    ///<summary>Attempt to get a Guids from input.</summary>
    ///<param name="objectId">object , Guid or string</param>
    ///<returns>a Guid Option.</returns>
    static member TryCoerceGuid (objectId:'T) : Guid option= 
        match box objectId with
        | :? Guid  as g -> if Guid.Empty = g then None else Some g
        | :? DocObjects.RhinoObject as o -> Some o.Id
        | :? DocObjects.ObjRef      as o -> Some o.ObjectId
        | :? string  as s -> let ok, g = Guid.TryParse s in  if ok then Some g else None
        | _ -> None

    ///<summary>Attempt to get a Guids from input.</summary>
    ///<param name="input">object , Guid or string</param>
    ///<returns>Guid. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceGuid(input:'T) : Guid = 
        match box input with
        | :? Guid  as g -> if Guid.Empty = g then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuid: Guid is Empty"  else g
        | :? Option<Guid>  as go -> if go.IsNone || Guid.Empty = go.Value then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuid: Guid is Empty or None: %A" input else go.Value //from UI functions
        | :? string  as s -> try Guid.Parse s with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuid: string '%s' can not be converted to a Guid" s
        | :? DocObjects.RhinoObject as o -> o.Id
        | :? DocObjects.ObjRef      as o -> o.ObjectId
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuid: %A can not be converted to a Guid" input


    ///<summary>Attempt to get RhinoObject from the document with a given objectId.</summary>
    ///<param name="objectId">object Identifier (Guid or string)</param>
    ///<returns>a RhinoObject Option</returns>
    static member TryCoerceRhinoObject (objectId:Guid) : DocObjects.RhinoObject option = 
        match State.Doc.Objects.FindId(objectId) with
        | null -> None
        | o    -> Some o        
    
    ///<summary>Attempt to get RhinoObject from the document with a given objectId.</summary>
    ///<param name="objectId">(Guid) Object Identifier </param>
    ///<returns>a RhinoObject, Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceRhinoObject(objectId:Guid) : DocObjects.RhinoObject = 
        match RhinoScriptSyntax.TryCoerceRhinoObject objectId with 
        |Some o -> o
        |None -> 
            if Guid.Empty = objectId then    RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceRhinoObject failed on empty Guid"
            else                             RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceRhinoObject: The Guid %O was not found in the Current Object table." objectId

    ///<summary>Attempt to get GeometryBase class from given Guid. Fails on empty Guid.</summary>   
    ///<param name="objectId">geometry Identifier (Guid)</param>
    ///<returns>a Rhino.Geometry.GeometryBase Option</returns>
    static member TryCoerceGeometry (objectId:Guid) :GeometryBase option = 
        match State.Doc.Objects.FindId(objectId) with
        | null -> None
        | o -> Some o.Geometry
    
    ///<summary>Attempt to get GeometryBase class from given input.</summary>
    ///<param name="objectId">(Guid) geometry Identifier </param>
    ///<returns>(Rhino.Geometry.GeometryBase. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceGeometry(objectId:Guid) : GeometryBase =         
        if Guid.Empty = objectId then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGeometry failed on empty Guid"
        let o = State.Doc.Objects.FindId(objectId)
        if isNull o then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGeometry failed: Guid %O not found in Object table." objectId
        o.Geometry

    ///<summary>Attempt to get Rhino LightObject from the document with a given objectId.</summary>
    ///<param name="objectId">(Guid) light Identifier</param>
    ///<returns>a Rhino.Geometry.Light. Option.</returns>
    static member TryCoerceLight (objectId:Guid) : Light option = 
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :? Geometry.Light as l -> Some l
        | _ -> None

    ///<summary>Attempt to get Rhino LightObject from the document with a given objectId.</summary>
    ///<param name="objectId">(Guid) light Identifier</param>
    ///<returns>a  Rhino.Geometry.Light. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceLight (objectId:Guid) : Light = 
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :? Geometry.Light as l -> l
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLight failed on: %s " (Nice.str objectId)


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
    ///<returns>(Rhino.Geometry.Mesh. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceMesh(objectId:Guid) : Mesh = 
        match RhinoScriptSyntax.TryCoerceMesh(objectId) with
        | Some m -> m
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceMesh failed on: %s " (Nice.str objectId)

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
    ///<returns>(Rhino.Geometry.Surface. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceSurface(objectId:Guid) : Surface = 
        match RhinoScriptSyntax.CoerceGeometry(objectId) with
        | :? Surface as c -> c
        | :? Brep as b ->
            if b.Faces.Count = 1 then b.Faces.[0] :> Surface
            else RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceSurface failed on %O from Brep with %d Faces" objectId b.Faces.Count
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceSurface failed on: %O " objectId

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
    ///<param name="objectId">objectId (Guid or string) to be RhinoScriptSyntax.Coerced into a brep</param>
    ///<returns>(Rhino.Geometry.Brep. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceBrep(objectId:Guid) : Brep  = 
        match RhinoScriptSyntax.TryCoerceBrep(objectId) with
        | Some b -> b
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceBrep failed on: %s " (Nice.str objectId)

    ///<summary>Attempt to get Curve geometry from the document with a given objectId.</summary>
    ///<param name="objectId">objectId (Guid or string) to be RhinoScriptSyntax.Coerced into a Curve</param>
    ///<param name="segmentIndex">(int) Optional, index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<returns>a Rhino.Geometry.Curve Option.</returns>
    static member TryCoerceCurve(objectId:Guid,[<OPT;DEF(-1)>]segmentIndex:int) : Curve option = 
        let geo = RhinoScriptSyntax.CoerceGeometry objectId
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
    ///<param name="objectId">objectId (Guid or string) to be RhinoScriptSyntax.Coerced into a Curve</param>
    ///<param name="segmentIndex">(int) Optional, index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<returns>(Rhino.Geometry.Curve. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceCurve(objectId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Curve = 
        if segmentIndex < 0 then
            match RhinoScriptSyntax.CoerceGeometry(objectId) with
            | :? Curve as c -> c
            | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceCurve failed on: %s " (Nice.str objectId)
        else
            match RhinoScriptSyntax.CoerceGeometry(objectId) with
            | :? PolyCurve as c ->
                let crv = c.SegmentCurve(segmentIndex)
                if isNull crv then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceCurve failed on segment index %d for %s" segmentIndex  (Nice.str objectId)
                crv
            | :? Curve as c -> c
            | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceCurve failed for %s"  (Nice.str objectId)


    ///<summary>Attempt to get Rhino Line Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="line">Line, two points or Guid</param>
    ///<returns>Geometry.Line. Raises a RhinoScriptingException if coerce failed.</returns>
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
    ///<returns>Geometry.Line, Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceLine(line:'T) : Line= 
        match RhinoScriptSyntax.TryCoerceLine(line) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLine failed on: %s " (Nice.str line)

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
            if ok then Some( !a )
            else None
        | :? Guid as g ->
            match State.Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a = ref (new Arc())
                let ok = crv.TryGetArc(a,State.Doc.ModelAbsoluteTolerance)
                if ok then Some( !a )
                else None
            | _ -> None
        |_ -> None

    ///<summary>Attempt to get Rhino Arc Geometry using the current Documents Absolute Tolerance.
    /// does not return circles as arcs.</summary>
    ///<param name="arc">Guid, RhinoObject or Curve </param>
    ///<returns>Geometry.Arc, Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceArc(arc:'T) : Arc= 
        match RhinoScriptSyntax.TryCoerceArc(arc) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceArc failed on: %s " (Nice.str arc)


    ///<summary>Attempt to get Rhino Circle Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="cir">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Circle Option.</returns>
    static member TryCoerceCircle(cir:'T) : Circle option= 
        match box cir with
        | :? Circle as a -> Some(a)
        | :? Curve as crv ->
            let a = ref (new Circle())
            let ok = crv.TryGetCircle(a,State.Doc.ModelAbsoluteTolerance)
            if ok then Some( !a )
            else None
        | :? Guid as g ->
            match State.Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a = ref (new Circle())
                let ok = crv.TryGetCircle(a,State.Doc.ModelAbsoluteTolerance)
                if ok then Some( !a )
                else None
            | _ -> None
        |_ -> None

    ///<summary>Attempt to get Rhino Circle Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="circ">Guid, RhinoObject or Curve </param>
    ///<returns>Geometry.Circle, Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceCircle(circ:'T) : Circle= 
        match RhinoScriptSyntax.TryCoerceCircle(circ) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceCircle failed on: %s " (Nice.str circ)

    ///<summary>Attempt to get Rhino Ellipse Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="cir">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Ellipse Option.</returns>
    static member TryCoerceEllipse(cir:'T) : Ellipse option= 
        match box cir with
        | :? Ellipse as a -> Some(a)
        | :? Curve as crv ->
            let a = ref (new Ellipse())
            let ok = crv.TryGetEllipse(a,State.Doc.ModelAbsoluteTolerance)
            if ok then Some( !a )
            else None
        | :? Guid as g ->
            match State.Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a = ref (new  Ellipse())
                let ok = crv.TryGetEllipse(a,State.Doc.ModelAbsoluteTolerance)
                if ok then Some( !a )
                else None
            | _ -> None
        |_ -> None

    ///<summary>Attempt to get Rhino Ellipse Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="ellipse">Guid, RhinoObject or Curve </param>
    ///<returns>Geometry.Ellipse, Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceEllipse(ellipse:'T) : Ellipse= 
        match RhinoScriptSyntax.TryCoerceEllipse(ellipse) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceEllipse failed on: %s " (Nice.str ellipse)

    ///<summary>Attempt to get Rhino Polyline Geometry.</summary>
    ///<param name="poly">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Polyline Option.</returns>
    static member TryCoercePolyline(poly:'T) : Polyline option= 
        match box poly with
        | :? Polyline as a -> Some(a)
        | :? PolylineCurve as crv -> Some <| crv.ToPolyline()
        | :? Curve as crv ->                 
                let a : ref<Polyline> = ref null
                let ok = crv.TryGetPolyline(a)
                if ok then Some( !a )
                else None            
        | :? Guid as g ->
            match State.Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a : ref<Polyline> = ref null
                let ok = crv.TryGetPolyline(a)
                if ok then Some( !a )
                else None
            | _ -> None
        |_ -> None

    ///<summary>Attempt to get Rhino Polyline Geometry.</summary>
    ///<param name="poly">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Polyline Option.</returns>
    static member CoercePolyline(poly:'T) : Polyline = 
        match RhinoScriptSyntax.TryCoercePolyline poly with
        |None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePolyline failed on: %s " (Nice.str poly)
        |Some pl -> pl


    //----------------------------------------------------
    //---------------Coerce only (no TryCoerce) ----------  TODO: add TryCoerce ?
    //----------------------------------------------------


    ///<summary>Attempt to get Rhino LayerObject from the document for a given full name.</summary>
    ///<param name="name">(string) The layer's name.</param>
    ///<returns>DocObjects.Layer </returns>
    static member CoerceLayer (name:string) : DocObjects.Layer = 
        let i = State.Doc.Layers.FindByFullPath(name, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then
            let lay = State.Doc.Layers.FindName name
            if isNull lay then
                RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLayer: could not find a layer named '%s'" name
            else
                lay
        else
            State.Doc.Layers.[i]


    ///<summary>Attempt to get Rhino LayerObject from the document with a given objectId.</summary>
    ///<param name="layerId">(Guid) The layer's Guid.</param>
    ///<returns>DocObjects.Layer</returns>
    static member CoerceLayer (layerId:Guid) : DocObjects.Layer= 
        if layerId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLayer: input Guid is Guid.Empty" 
        let l = State.Doc.Layers.FindId(layerId)
        if isNull l then
            if notNull (State.Doc.Objects.FindId(layerId)) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLayer works on  Guid of a Layer Object, not the Guid of a Document Object (with Geometry) '%s'" (Nice.str layerId)
            else
                RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLayer: could not find Guid %O in State.Doc.Layer table'" layerId
        l

    ///<summary>Returns the Rhino Block instance object for a given Id.</summary>
    ///<param name="objectId">(Guid) Id of block instance</param>
    ///<returns>(DocObjects.InstanceObject) block instance object.</returns>
    static member CoerceBlockInstanceObject(objectId:Guid) : DocObjects.InstanceObject = 
        match RhinoScriptSyntax.CoerceRhinoObject(objectId) with
        | :? DocObjects.InstanceObject as b -> b
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceBlockInstanceObject: unable to coerce Block InstanceObject from '%s'" (Nice.str objectId)



    //-------------------views ---------------------

    ///<summary>Attempt to get Rhino View Object from the name of the view, can be a standard or page view.</summary>
    ///<param name="nameOrId">(string or Guid) Name or Guid the view, empty string will return the Active view</param>
    ///<returns>a State.Doc.View object. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceView (nameOrId:'T) : Display.RhinoView = 
        match box nameOrId with
        | :? Guid as g ->
            let viewo = State.Doc.Views.Find(g)
            if isNull viewo then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceView: could not CoerceView from '%O'" g
            else viewo

        | :? string as view ->
            if isNull view then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceView: failed on null for view name input" // or State.Doc.Views.ActiveView
            elif view = "" then State.Doc.Views.ActiveView
            else
                let allviews = 
                    State.Doc.Views.GetViewList(includeStandardViews=true, includePageViews=true)
                    |> Array.filter (fun v-> v.MainViewport.Name = view)
                if allviews.Length = 1 then allviews.[0]
                else  RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceView: could not CoerceView '%s'" view

        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceView: Cannot get view from %A" nameOrId


    ///<summary>Attempt to get Rhino Page (or Layout) View Object from the name of the Layout.</summary>
    ///<param name="nameOrId">(string) Name of the Layout</param>
    ///<returns>a State.Doc.View object. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoercePageView (nameOrId:'T) : Display.RhinoPageView = 
        match box nameOrId with
        | :? Guid as g ->
            let viewo = State.Doc.Views.Find(g)
            if isNull viewo then RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePageView: could not CoerceView  from '%O'" g
            else
                try viewo :?> Display.RhinoPageView
                with _  -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePageView: the view found '%s' is not a page view" viewo.MainViewport.Name

        | :? string as view ->
            if isNull view then RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePageView: failed on null for view name input" // or State.Doc.Views.ActiveView
            else
                let allviews = State.Doc.Views.GetPageViews()|> Array.filter (fun v-> v.PageName = view)
                let l = allviews.Length
                if allviews.Length = 1 then allviews.[0]
                elif allviews.Length > 1 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePageView: more than one page called '%s'" view
                else  RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePageView: Layout called '%s' not found" view

        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePageView: Cannot get view from %O" nameOrId

    ///<summary>Attempt to get Detail view rectangle Geometry.</summary>
    ///<param name="objectId">(Guid) objectId of Detail object</param>
    ///<returns>a Geometry.DetailView. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceDetailView (objectId:Guid) : DetailView = 
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  DetailView as a -> a
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceDetailView failed on %s"  (Nice.str objectId)

    ///<summary>Attempt to get Detail view rectangle Object.</summary>
    ///<param name="objectId">(Guid) objectId of Detail object</param>
    ///<returns>a DocObjects.DetailViewObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceDetailViewObject (objectId:Guid) : DocObjects.DetailViewObject = 
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.DetailViewObject as a -> a
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceDetailViewObject failed on %s"  (Nice.str objectId)


    //-------Annotation ----


    ///<summary>Attempt to get TextDot Geometry.</summary>
    ///<param name="objectId">(Guid) objectId of TextDot object</param>
    ///<returns>a Geometry.TextDot. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceTextDot (objectId:Guid) : TextDot = 
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  TextDot as a -> a
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceTextDot failed on: %s " (Nice.str objectId)

    ///<summary>Attempt to get TextEntity Geometry (for the text Object use rs.CoerceTextObject) .</summary>
    ///<param name="objectId">(Guid) objectId of TextEntity object</param>
    ///<returns>a Geometry.TextEntity. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceTextEntity (objectId:Guid) : TextEntity = 
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  TextEntity as a -> a
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceTextEntity failed on: %s " (Nice.str objectId)


    ///<summary>Attempt to get Rhino TextObject Annotation Object.</summary>
    ///<param name="objectId">(Guid) objectId of TextObject</param>
    ///<returns>(DocObjects.TextObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceTextObject (objectId:Guid) : DocObjects.TextObject = 
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.TextObject as a -> a
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceTextObject failed on: %s " (Nice.str objectId)

    ///<summary>Attempt to get Hatch Geometry.</summary>
    ///<param name="objectId">(Guid) objectId of Hatch object</param>
    ///<returns>a Geometry.CoerceHatch. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceHatch (objectId:Guid) : Hatch = 
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  Hatch as a -> a
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceHatch failed on: %s " (Nice.str objectId)


    ///<summary>Attempt to get Rhino Hatch Object.</summary>
    ///<param name="objectId">(Guid) objectId of Hatch object</param>
    ///<returns>(DocObjects.HatchObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceHatchObject (objectId:Guid) : DocObjects.HatchObject = 
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.HatchObject as a -> a
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceHatchObject failed on: %s " (Nice.str objectId)

    ///<summary>Attempt to get Rhino Annotation Base Object.</summary>
    ///<param name="objectId">(Guid) objectId of annotation object</param>
    ///<returns>(DocObjects.AnnotationObjectBase. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceAnnotation (objectId:Guid) : DocObjects.AnnotationObjectBase = 
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.AnnotationObjectBase as a -> a
        | o -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceAnnotation failed on: %s " (Nice.str objectId)


    ///<summary>Attempt to get Rhino Leader Annotation Object.</summary>
    ///<param name="objectId">(Guid) objectId of Leader object</param>
    ///<returns>(DocObjects.LeaderObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceLeader (objectId:Guid) : DocObjects.LeaderObject = 
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.LeaderObject as a -> a
        | o -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLeader failed on: %s " (Nice.str objectId)

    ///<summary>Attempt to get Rhino LinearDimension Annotation Object.</summary>
    ///<param name="objectId">(Guid) objectId of LinearDimension object</param>
    ///<returns>(DocObjects.LinearDimensionObject. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceLinearDimension (objectId:Guid) : DocObjects.LinearDimensionObject = 
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.LinearDimensionObject as a -> a
        | o -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLinearDimension failed on: %s " (Nice.str objectId)

    //---------Geometry ------------
    

    ///<summary>Converts input into a Rhino.Geometry.Point3d if possible.</summary>
    ///<param name="pt">Input to convert, Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    ///<returns>a Rhino.Geometry.Point3d, Raises a RhinoScriptingException if coerce failed.</returns>
    static member Coerce3dPoint(pt:'T) : Point3d = 
        let inline  point3dOf3(x:^x, y:^y, z:^z) = 
            try Point3d(float (x), float(y), float(z))
            with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dPoint: Could not Coerce the 3 values %O, %O and %O to a Point3d" x y z
        let b = box pt
        match b with
        | :? Point3d    as pt                 -> pt
        | :? Point3f    as pt                 -> Point3d(pt)
        //| :? Vector3d   as v                  -> Point3d(v) //don't confuse vectors and points !
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
                |_ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dPoint failed on: %s " (Nice.str pt)
            with _ ->
                RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dPoint failed on: %s " (Nice.str pt)

    ///<summary>Attempt to get Rhino Point Object.</summary>
    ///<param name="objectId">(Guid) objectId of Point object</param>
    ///<returns>a DocObjects.PointObject, Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoercePointObject (objectId:Guid) : DocObjects.PointObject = 
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.PointObject as a -> a
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePointObject failed on: %s " (Nice.str objectId)

    ///<summary>Convert input into a Rhino.Geometry.Point2d if possible.</summary>
    ///<param name="point">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    ///<returns>a Rhino.Geometry.Point2d, Raises a RhinoScriptingException if coerce failed.</returns>
    static member Coerce2dPoint(point:'T) : Point2d = 
        match box point with
        | :? Point2d    as point -> point
        | :? Point3d    as point -> Point2d(point.X, point.Y)
        | :? (float*float) as xy  -> let x, y = xy in Point2d(x, y)
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce2dPoint: could not Coerce: Could not convert %A to a Point2d"  point

    ///<summary>Convert input into a Rhino.Geometry.Vector3d if possible.</summary>
    ///<param name="vec">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    ///<returns> a Rhino.Geometry.Vector3d, Raises a RhinoScriptingException if coerce failed.</returns>
    static member Coerce3dVector(vec:'T) : Vector3d = 
        let inline vecOf3(x:^x, y:^y, z:^z) = 
            try Vector3d(float (x), float(y), float(z))
            with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dVector: Could not Coerce %O, %O and %O to Vector3d" x y z
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
                |_ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dVector failed on: %s " (Nice.str vec)
            with _ ->
                RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dVector failed on: %s " (Nice.str vec)



    ///<summary>Convert input into a Rhino.Geometry.Plane if possible.</summary>
    ///<param name="plane">Plane, point, list, tuple</param>
    ///<returns>(Rhino.Geometry.Plane. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoercePlane(plane:'T) : Plane = 
        match box plane with
        | :? Plane  as plane -> plane
        | _ ->
            try
                let pt = RhinoScriptSyntax.Coerce3dPoint(plane)
                let mutable pl = Plane.WorldXY
                pl.Origin <- pt
                pl
            with e ->
                RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePlane failed on: %s " (Nice.str plane)

    ///<summary>Convert input into a Rhino.Geometry.Transform Transformation Matrix if possible.</summary>
    ///<param name="xForm">object to convert</param>
    ///<returns>(Rhino.Geometry.Transform. Raises a RhinoScriptingException if coerce failed.</returns>
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
                    | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceXform: seq<seq<float>> %s can not be converted to a Transformation Matrix" (Nice.str xForm)
                t
        | :? ``[,]``<float>  as xss -> // TODO verify row, column order !!
                let mutable t= Transform()
                try
                    xss|> Array2D.iteri (fun i j x -> t.[i, j]<-x)
                with
                    | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceXform: Array2D %s can not be converted to a Transformation Matrix" (Nice.str xForm)
                t
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceXform: could not CoerceXform %s can not be converted to a Transformation Matrix" (Nice.str xForm)
    


    ///<summary>Attempt to get Surface geometry from the document with a given objectId.</summary>
    ///<param name="objectId">the object's Identifier</param>
    ///<returns>(Rhino.Geometry.Surface. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceNurbsSurface(objectId:Guid) : NurbsSurface = 
        match RhinoScriptSyntax.CoerceGeometry(objectId) with
        | :? NurbsSurface as s -> s
        | :? Surface as c -> c.ToNurbsSurface()
        | :? Brep as b ->
            if b.Faces.Count = 1 then (b.Faces.[0] :> Surface).ToNurbsSurface()
            else RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceNurbsSurface failed on %s from Brep with %d Faces" (Nice.str objectId)   b.Faces.Count
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceNurbsSurface failed on: %O "  objectId



    ///<summary>Attempt to get Rhino PointCloud Geometry.</summary>
    ///<param name="objectId">(Guid) objectId of PointCloud object</param>
    ///<returns>a Geometry.PointCloud. Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoercePointCloud (objectId:Guid) : PointCloud = 
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  PointCloud as a -> a
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePointCloud failed on: %s " (Nice.str objectId)
            
    
    ///<summary>Attempt to get a System.Drawing.Color also works on natural language color strings see Drawing.ColorTranslator.FromHtml.</summary>
    ///<param name="color">string, tuple with  or 3 or 4 items</param>
    ///<returns>System.Drawing.Color in ARGB form (not as named color) this will provide better comparison to other colors.
    /// For example the named color Red is not equal to fromRGB(255, 0, 0). Raises a RhinoScriptingException if coerce failed.</returns>
    static member CoerceColor(color:'T) : Drawing.Color = 
        match box color with
        | :? Drawing.Color  as c -> Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B) //https://stackoverflow.com/questions/20994753/compare-two-color-objects
        | :? (int*int*int) as rgb       ->
            let red , green, blue   = rgb
            if red  <0 || red  >255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green %d" red green blue
            if green<0 || green>255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green %d" red green blue
            if blue <0 || blue >255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green %d" red green blue
            Drawing.Color.FromArgb( red, green, blue)

        | :? (int*int*int*int) as argb  ->
            let alpha, red , green, blue   = argb
            if red  <0 || red  >255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green  %d alpha %d" red green blue alpha
            if green<0 || green>255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green  %d alpha %d" red green blue alpha
            if blue <0 || blue >255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green  %d alpha %d" red green blue alpha
            if alpha<0 || alpha >255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green %d alpha %d" red green blue alpha
            Drawing.Color.FromArgb(alpha, red, green, blue)
        | :? string  as s ->            
            try
                let c = Drawing.ColorTranslator.FromHtml(s)
                Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
            with _ -> 
                //try
                //    let c = Drawing.Color.FromName(s) // for invalid names ( a hex string) this still returns named color black !!
                //    Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
                //with _ -> 
                    RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor:: could not Coerce %A to a Color" color

                //     try Windows.Media.ColorConverter.ConvertFromString(s)
                //     with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.: could not Coerce %A to a Color" c
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor:: could not Coerce %A to a Color" color

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
    //                            RhinoScriptingException.Raise "RhinoScriptSyntax.: could not CoerceGuidList: %A can not be converted to a Sequence(IEnumerable) of Guids" Ids
    //    | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuidList: could not CoerceGuidList: %A can not be converted to a Sequence(IEnumerable) of Guids" Ids

        //<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible</summary>
    //<param name="points">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    //<returns>(Rhino.Geometry.Point3d seq) Fails on bad input</returns>
    //static member Coerce3dPointList(points:'T) : Point3d seq= 
    //    try Seq.map RhinoScriptSyntax.Coerce3dPoint points //|> Seq.cache
    //    with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dPointList: could not Coerce: Could not convert %A to a list of 3d points"  points

    //<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible</summary>
    //<param name="points">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    //<returns>(Rhino.Geometry.Point2d seq) Fails on bad input</returns>
    //static member Coerce2dPointList(points:'T) : Point2d seq= 
    //    try Seq.map RhinoScriptSyntax.Coerce2dPoint points //|> Seq.cache
    //    with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce2dPointList: could not Coerce: Could not convert %A to a list of 2d points"  points

