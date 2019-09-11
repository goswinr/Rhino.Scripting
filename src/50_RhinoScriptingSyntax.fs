
namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument 

/// A static class with static mebres providing functions very similar to RhinoScript in Pyhton and VBscript 
type RhinoScriptSyntax () = //private () = 

    ///<summary>Converts input into a Rhino.Geometry.Point3d if possible.</summary>
    ///<param name="point">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>
    ///<returns>a Rhino.Geometry.Point3d. Fails on bad input</returns>
    static member Coerce3dPoint(pt:'point) : Point3d=
        let b = box pt
        match b with
        | :? Point3d    as pt               -> pt
        | :? Vector3d   as v                -> Point3d(v)
        | :? Point3f    as pt               -> Point3d(pt)
        | :? (float*float*float) as xyz     -> let x,y,z = xyz in Point3d(x,y,z)
        | :? (int*int*int) as xyz           -> let x,y,z = xyz in Point3d(float(x),float(y),float(z))
        | _ ->
            try
                match b with
                | :? (string*string*string) as xyz  -> let x,y,z = xyz in Point3d(float(x),float(y),float(z)) 
                | :? Guid as g ->  ((Doc.Objects.Find(g).Geometry) :?> Point).Location 
                | :? seq<float>  as xyz  ->  point3dOf3(Seq.item 0 xyz,Seq.item 3 xyz,Seq.item 2 xyz)
                | :? seq<int>    as xyz  ->  point3dOf3(Seq.item 0 xyz,Seq.item 3 xyz,Seq.item 2 xyz)
                | :? seq<string> as xyz  ->  point3dOf3(Seq.item 0 xyz,Seq.item 3 xyz,Seq.item 2 xyz)
                | :? string as s  -> 
                    let xs = s.Split(',')
                    if Seq.length xs = 3 then Point3d(Double.Parse(Seq.item 0 xs),Double.Parse(Seq.item 1 xs),Double.Parse(Seq.item 2 xs)) else fail()                
                | _ -> 
                    let xs = b :?> seq<_>
                    let x = floatOfObj(Seq.item 0 xs)
                    let y = floatOfObj(Seq.item 1 xs)
                    let z = floatOfObj(Seq.item 2 xs)
                    Point3d(x,y,z)                    
            with _ ->
                failwithf "*** could not coerce %A to a Point3d" pt
    
    ///<summary>Convert input into a Rhino.Geometry.Point2d if possible.</summary>
    ///<param name="point">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>
    ///<returns>a Rhino.Geometry.Point2d. Fails on bad input</returns>
    static member Coerce2dPoint(point:'point2d) =
        match box point with
        | :? Point2d    as point -> point
        | :? Point3d    as point -> Point2d(point.X, point.Y)
        | :? (float*float) as xy  -> let x,y = xy in Point2d(x,y)
        | _ -> failwithf "*** could not coerce: Could not convert %A to a Point2d"  point
    
    ///<summary>Convert input into a Rhino.Geometry.Vector3d if possible.</summary>
    ///<param name="vector">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>
    ///<returns>a Rhino.Geometry.Vector3d. Fails on bad input</returns>
    static member Coerce3dVector(vector:'vector) =
        try Vector3d(RhinoScriptSyntax.Coerce3dPoint(vector))
        with _ -> failwithf "*** could not coerce: Could not convert %A to a Vector3d" vector
    
        
    ///<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible.</summary>
    ///<param name="ponits">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>
    ///<returns>Rhino.Geometry.Point3d seq. Fails on bad input</returns>
    static member Coerce3dPointList(points:'points) : Point3d seq=
        try Seq.map RhinoScriptSyntax.Coerce3dPoint points //|> Seq.cache
        with _ -> failwithf "*** could not coerce: Could not convert %A to a list of 3d points"  points
        
    ///<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible.</summary>
    ///<param name="ponits">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>
    ///<returns>Rhino.Geometry.Point2d seq. Fails on bad input</returns>
    static member Coerce2dPointList(points:'points) : Point2d seq=
        try Seq.map RhinoScriptSyntax.Coerce2dPoint points //|> Seq.cache
        with _ -> failwithf "*** could not coerce: Could not convert %A to a list of 2d points"  points

    ///<summary>Convert input into a Rhino.Geometry.Plane if possible.</summary>
    ///<param name="plane">Plane,point, list, tuple</param>
    ///<returns>a Rhino.Geometry.Plane. Fails on bad input</returns>
    static member CoercePlane(plane:'plane) =
        match box plane with // TODO add more
        | :? Plane  as plane -> plane // TODO add more
        | _ -> failwithf "*** could not coerce: %A can not be converted to a Plane" plane
    
 
    ///<summary>Convert input into a Rhino.Geometry.Transform Transformation Matrix if possible.</summary>
    ///<param name="xform">object to convert</param>
    ///<returns>Rhino.Geometry.Transform. Fails on bad input</returns>   
    static member CoerceXform(xform:'xform) =
        match box xform with
        | :? Transform  as xform -> xform // TODO add more
        //| :? seq<float>  as xs ->  let m = Seq.toArray sx
        | _ -> failwithf "*** could not coercexform %A can not be converted to a Transformation Matrix" xform
    
    ///<summary>attempt to get a Guids from input</summary>
    ///<param name="id">objcts , guid or string</param>
    ///<returns>Guid. Fails on bad input</returns>
    static member CoerceGuid(id:'id) =
        match box id with
        | :? Guid  as g -> if Guid.Empty = g then fail() else g
        | :? string  as s -> try Guid.Parse s with _ -> failwithf "*** could not coerceguid: string '%s' can not be converted to a Guid" s
        | :? DocObjects.RhinoObject as o -> o.Id
        | :? DocObjects.ObjRef      as o -> o.ObjectId
        | _ -> failwithf "*** could not coerceguid:%A can not be converted to a Guid" id

    ///<summary>attempt to get a Sequence of Guids from input</summary>
    ///<param name="ids">list of guids</param>
    ///<returns>Guid seq. Fails on bad input</returns>
    static member CoerceGuidList(ids:'ids) =
        match box ids with
        | :? Guid  as g -> if Guid.Empty = g then fail() else [|g|] :> seq<Guid>
        | :? seq<obj> as gs -> 
            try gs |> Seq.map RhinoScriptSyntax.CoerceGuid
            with _ -> failwithf "*** could not coerceguidlist: %A can not be converted to a Sequence(IEnumerable) of Guids" ids
        | _ -> failwithf "*** could not coerceguidlist: %A can not be converted to a Sequence(IEnumerable) of Guids" ids
    
   
    ///<summary>attempt to get a System.Drawing.Color also works on natrural language color strings see Drawing.ColorTranslator.FromHtml </summary>
    ///<param name="color">string, tuple with  or 3 or 4 items.</param>
    ///<returns>a System.Drawing.Color in ARGB form (not as named color) this will provide better comparison to other colors.
    /// For example thye named color Red is not equal to fromRGB(255,0,0) . Fails on bad input</returns>
    static member CoerceColor(color:'color) =
        match box color with
        | :? Drawing.Color  as c -> Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B) //https://stackoverflow.com/questions/20994753/compare-two-color-objects
        | :? (int*int*int) as rgb       -> let r,g,b    = rgb in Drawing.Color.FromArgb(r,g,b)
        | :? (int*int*int*int) as argb  -> let a,r,g,b = argb in Drawing.Color.FromArgb(a,r,g,b)
        | :? string  as s -> 
            try 
                let c = Drawing.Color.FromName(s)
                Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
            with _ -> 
                try 
                    let c = Drawing.ColorTranslator.FromHtml(s)
                    Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
                with _ -> failwithf "*** could not coerce %A to a Color" color
               //     try Windows.Media.ColorConverter.ConvertFromString(s)        
               //     with _ -> failwithf "*** could not coerce %A to a Color" c
        | _ -> failwithf "*** could not coerce %A to a Color" color

    ///<summary>attempt to get Rhino Line Geometry</summary>
    ///<param name="line">Line, two points or Guid</param>
    ///<returns>Geometry.Line. Fails on bad input</returns>
    static member Coerceline(line:'line) =
        match box line with
        | :? Line as l -> l
        | :? Guid as g ->  
            match Doc.Objects.Find(g).Geometry with
            | :? Curve as crv ->
                if crv.IsLinear() then Line(crv.PointAtStart,crv.PointAtEnd)
                else failwithf "*** could not coerce %A to a Line" line
            //| :? Line as l -> l
            | _ -> failwithf "*** could not coerce %A to a Line" line
        | :? (Point3d*Point3d) as ab -> let a,b = ab in Line(a,b)
        // TODO parse 6 numbers
        |_ -> failwithf "*** could not coerce %A to a Line" line
    
    
    ///<summary>attempt to get RhinoObject from the document with a given id</summary>
    ///<param name="objectId">object identifier (Guid or string)</param>
    ///<returns>a RhinoObject. Fails on bad input</returns>
    static member Coercerhinoobject(objectId:'id) =
        match box objectId with
        | :? Guid  as g -> 
            if Guid.Empty = g then failwith "*** could not coerceObject: Empty Guid in RhinoScriptSyntax.Coercerhinoobject" 
            else 
                let o = Doc.Objects.Find(g) 
                if isNull o then failwithf "*** could not coerceObject: Guid %A not found in Object table (in RhinoScriptSyntax.Coercerhinoobject)" g
                else o        
        | :? DocObjects.RhinoObject as o -> o
        | :? DocObjects.ObjRef as r -> r.Object()        
        | :? string as s -> 
                let g = RhinoScriptSyntax.CoerceGuid(s)
                if Guid.Empty = g then failwithf "*** could not coerceObject: %s to a RhinoObject" s
                else 
                    let o = Doc.Objects.Find(g) 
                    if isNull o then failwithf "*** could not coerceObject:Guid %s not found in Object table (in RhinoScriptSyntax.Coercerhinoobject)" s
                    else o
        | _ -> failwithf "*** could not coerceObject:  %A to a RhinoObject" objectId


    ///<summary>attempt to get GeometryBase class from given input</summary>
    ///<param name="id">geometry identifier (Guid or string)</param>
    ///<returns>Rhino.Geometry.GeometryBase. Fails on bad input</returns>
    static member Coercegeometry(id:'id) =
        match box id with
        | :? GeometryBase as g -> g
        | :? Guid  as g -> (RhinoScriptSyntax.Coercerhinoobject g).Geometry        
        | :? DocObjects.ObjRef as r -> r.Geometry()
        | :? DocObjects.RhinoObject as o -> o.Geometry
        | :? string as s -> (RhinoScriptSyntax.Coercerhinoobject s).Geometry
        | _ -> failwithf "*** could not coerce %A to a RhinoObject" id

    ///<summary>attempt to get polysurface geometry from the document with a given id</summary>
    ///<param name="id">id (Guid or string) to be RhinoScriptSyntax.Coerced into a brep</param>
    ///<returns>a Rhino.Geometry.Brep. Fails on bad input</returns>
    static member Coercebrep(id:'id) =
        match RhinoScriptSyntax.Coercegeometry(id) with 
        | :? Brep as b -> b
        | :? Extrusion as b -> b.ToBrep(true)
        | _ -> failwithf "*** could not coerce %A to a Brep" id

    ///<summary>attempt to get curve geometry from the document with a given id.</summary>
    ///<param name="segmentIndex">index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<param name="id">id (Guid or string) to be RhinoScriptSyntax.Coerced into a curve</param>
    ///<param name="segmentIndex">index of segment to retrieve</param>
    ///<returns>Rhino.Geometry.Curve. Fails on bad input</returns>
    static member Coercecurve(segmentIndex:int) (id:'id) = 
        if segmentIndex < 0 then 
            match RhinoScriptSyntax.Coercegeometry(id) with 
            | :? Curve as c -> c
            | _ -> failwithf "*** could not coerce %A to a Curve" id
        else
            match RhinoScriptSyntax.Coercegeometry(id) with 
            | :? PolyCurve as c -> 
                let crv = c.SegmentCurve(segmentIndex)
                if isNull crv then failwithf "*** could not coerce segment index %d from curve %A" segmentIndex id
                crv
            | :? Curve as c -> c
            | _ -> failwithf "*** could not coerce %A to a Curve with segment index %d" id segmentIndex

  

    ///<summary>attempt to get surface geometry from the document with a given id</summary>
    ///<param name="objectId">objectId = the object's identifier</param>
    ///<returns>a Rhino.Geometry.Surface. Fails on bad input</returns>
    static member Coercesurface(id:'id) =
        match RhinoScriptSyntax.Coercegeometry(id) with 
        | :? Surface as c -> c
        | :? Brep as b -> 
            if b.Faces.Count = 1 then b.Faces.[0] :> Surface
            else failwithf "*** could not coerce %A to a Surface" id
        | _ -> failwithf "*** could not coerce %A to a Surface" id

    ///<summary>attempt to get mesh geometry from the document with a given id</summary>
    ///<param name="objectId">object identifier (Guid or string)</param>
    ///<returns>a Rhino.Geometry.Mesh. Fails on bad input</returns>    
    static member Coercemesh(id:'id) =
        match RhinoScriptSyntax.Coercegeometry(id) with 
        | :? Mesh as m -> m        
        | _ -> failwithf "*** could not coerce %A to a Mesh" id

    ///<summary>attempt to get Rhino LayerObject from the document with a given id or fullame</summary>
    ///<param name="nameOrId">(string or guid or index): layers identifier name</param>
    ///<returns>DocObjectys.Layer  Fails on bad input</returns>
    static member CoerceLayer (nameOrId:'nameOrId) : Rhino.DocObjects.Layer=
        try
            match box nameOrId with 
            | :? Guid as g   -> Doc.Layers.FindId(g)
            | :? string as s -> Doc.Layers.[Doc.Layers.FindByFullPath(s, Rhino.RhinoMath.UnsetIntIndex)]
            | :? int as ix   -> Doc.Layers.[ix]
            | _ -> fail()            
        with _ ->
            failwithf "*** could not coerce Layer from '%A'" nameOrId    
    
    ///<summary>attempt to get Rhino LightObject from the document with a given id</summary>
    ///<param name="id">(guid): light identifier </param>
    ///<returns>a  Rhino.Geometry.Light. Fails on bad input</returns>
    static member CoerceLight (id:'id) =
        match RhinoScriptSyntax.Coercegeometry id with
        | :? Geometry.Light as l -> l
        |_ -> failwithf "*** could not coerceLight: %A is not a Geometry.Light " id
    
    
    ///<summary>attempt to get Rhino View Object from the name of the view </summary>
    ///<param name="view">(string): name of the view, empty string will return the Active view</param> 
    ///<returns>a Doc.View object. Fails on bad input</returns>
    static member CoerceView view =    
        if view = "" then Doc.Views.ActiveView
        else 
            let allviews = Doc.Views.GetViewList(true, true) |> Seq.filter (fun v-> v.MainViewport.Name = view)
            if Seq.length allviews = 1 then Seq.head allviews
            else  failwithf "*** could not coerceView '%s'" view
    
    
    ///<summary>attempt to get Rhino Annotation Object</summary>
    ///<param name="id">(guid): id of annotation object</param> 
    ///<returns>a Rhino.DocObjects.AnnotationObjectBase. Fails on bad input.</returns>
    static member CoerceAnnotation (id:'id) =
        match RhinoScriptSyntax.Coercerhinoobject id with
        | :? Rhino.DocObjects.AnnotationObjectBase as a -> a
        |_ -> failwithf "*** could not coerceAnnotation: %A is not a Rhino.DocObjects.AnnotationObjectBase " id
        
    ///<summary>attempt to get GeometryBase class from given Guid</summary>
    ///<param name="id">geometry identifier (Guid)</param>
    ///<returns>Rhino.Geometry.GeometryBase Option. </returns>
    static member TryCoerceGeometry (id:Guid) :GeometryBase option =
        if id = Guid.Empty then None
        else
            match Doc.Objects.Find(id) with 
            | null -> None
            | o -> Some o.Geometry 


    ///<summary>attempt to get curve geometry from the document with a given id.</summary>
    ///<param name="segmentIndex">index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<param name="id">id (Guid or string) to be RhinoScriptSyntax.Coerced into a curve</param>
    ///<param name="segmentIndex">index of segment to retrieve</param>
    ///<returns>Rhino.Geometry.Curve. Option</returns>
    static member TryCoerceCurve(segmentIndex:int) (id:Guid) = 
        match RhinoScriptSyntax.TryCoerceGeometry id with 
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