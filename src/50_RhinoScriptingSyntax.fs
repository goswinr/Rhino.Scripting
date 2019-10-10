
namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument 

[<AbstractClass; Sealed>]
/// A static class with static members providing functions very similar to RhinoScript in Pyhton and VBscript 
type RhinoScriptSyntax private () = // no constructor?
    

    ///<summary>Returns a nice string for any kinds of objects or values, for most objects this is just calling *.ToString() </summary>
    ///<param name="x">('T): the value or object to represent as string</param>
    ///<returns>(stirng) the string</returns>
    static member ConvertToString (x:'T) : string =
        match box x with
        | :? Point3d    as p   -> p.ToNiceString
        | :? Vector3d   as v   -> v.ToNiceString
        | :? Point3f    as p   -> p.ToNiceString
        | :? float      as v   -> v.ToNiceString
        | :? single     as v   -> v.ToNiceString
        | :? Char       as c   -> c.ToString()  // "'" + c.ToString() + "'"
        | :? string     as s   -> s // to not have it in quotes
        | _ ->          x.ToString()
        

    ///<summary>Prints an object or value to Rhino Command line </summary>
    ///<param name="x">('T): the value or object to print</param>
    ///<returns>(unit) void, nothing/returns>
    static member Print (x:'T) : unit =
        let s = RhinoScriptSyntax.ConvertToString(x)
        RhinoApp.WriteLine s   
        RhinoApp.Wait() // no swith to UI Thread needed !
    
    ///<summary>Prints two objects or values to Rhino Command line, separated by a space character</summary>
    ///<param name="x1">('T): the first value or object to print</param>
    ///<param name="x2">('T): the second value or object to print</param>
    ///<returns>(unit) void, nothing/returns>
    static member Print (x1:'T, x2:'U) : unit =
        let s1 = RhinoScriptSyntax.ConvertToString(x1)
        let s2 = RhinoScriptSyntax.ConvertToString(x2)
        RhinoApp.WriteLine (s1 + " " + s2)
        RhinoApp.Wait()

    ///<summary>Prints three objects or values to Rhino Command line, separated by a space character</summary>
    ///<param name="x1">('T): the first value or object to print</param>
    ///<param name="x2">('U): the second value or object to print</param>
    ///<param name="x2">('V): the third value or object to print</param>
    ///<returns>(unit) void, nothing/returns>
    static member Print (x1:'T, x2:'U, x3:'v) : unit =
        let s1 = RhinoScriptSyntax.ConvertToString(x1)
        let s2 = RhinoScriptSyntax.ConvertToString(x2)
        let s3 = RhinoScriptSyntax.ConvertToString(x3)
        RhinoApp.WriteLine (s1 + " " + s2+ " " + s3)
        RhinoApp.Wait()


    ///<summary>Prints Sequence of objects or values separated by a space charcter or a custom value </summary>
    ///<param name="xs">('T): the values or objects to print</param>
    ///<param name="separator">(string) Optional, Default Value: a space charcater <c>" "</c></param>
    ///<returns>(unit) void, nothing/returns>
    static member Print (xs:'T seq, [<OPT;DEF(null:string)>]separator:string) : unit =
        xs
        |> Seq.map RhinoScriptSyntax.ConvertToString
        |> String.concat (separator |? " ")
        |> RhinoApp.WriteLine 
        RhinoApp.Wait()



    ///<summary>clamps a value between a lower and an upper bound</summary>
    ///<param name="minVal">(float): lower bound</param>
    ///<param name="maxVal">(float): upper bound</param>
    ///<param name="value">(float): the value to clamp</param>
    ///<returns>(float):clamped value</returns>
    static member Clamp (minVal:float,maxVal:float,value:float) : float =
        if minVal > maxVal then  failwithf "Clamp: lowvalue %A must be less than highvalue %A" minVal maxVal
        max minVal (min maxVal value) 


    ///<summary>Like the Python 'xrange' function for integers this creates a range of floating point values.
    ///this last or stop value will NOT be included in range as per python semanticsis, this is diffrent from F# semantics on range expressions</summary>
    ///<param name="start">(float): first value of range</param> 
    ///<param name="stop">(float): end of range( this last value will not be included in range,Python semantics)</param>    
    ///<param name="step">(float): step size between two values</param>
    ///<returns> a seq of floats </returns>
    static member  Fxrange (start:float, stop:float, step:float) : float seq =
        if isNanOrInf start then failwithf "Frange: NaN or Infinity, start=%f, step=%f, stop=%f" start step stop
        if isNanOrInf step  then failwithf "Frange: NaN or Infinity, start=%f, step=%f, stop=%f" start step stop
        if isNanOrInf stop  then failwithf "Frange: NaN or Infinity, start=%f, step=%f, stop=%f" start step stop
        let range = stop - start 
                    |> BitConverter.DoubleToInt64Bits 
                    |> (+) 5L // to make sure stop value is included in Range, will explicitly be removed below
                    |> BitConverter.Int64BitsToDouble
        let steps = range/step - 1.0 // -1 to make sure stop value is not included(python semanticsis diffrent from F# semantics on range expressions)
        if isNanOrInf steps then failwithf "*** range/step in frange: %f / %f is NaN Infinity, start=%f, stop=%f" range step start stop
    
        if steps < 0.0 then 
            failwithf "Frange: Stop value cannot be reached: start=%f, step=%f, stop=%f (steps:%f)" start step stop steps //or Seq.empty
        else 
            // the actual alogorithm: 
            let rec floatrange (start, i, steps) =
                seq { if i <= steps then 
                        yield start + i*step
                        yield! floatrange (start, (i + 1.0), steps) } // tail recursive ?
            floatrange (start, 0.0, steps) 

    ///<summary>Like the Python 'range' function for integers this creates a range of floating point values.
    ///this last or stop value will NOT be included in range as per python semanticsis, this is diffrent from F# semantics on range expressions</summary>
    ///<param name="start">(float): first value of range</param> 
    ///<param name="stop">(float): end of range( this last value will not be included in range,Python semantics)</param>    
    ///<param name="step">(float): step size between two values</param>
    ///<returns> an array of floats </returns>
    static member Frange (start:float, stop:float, step:float) : float [] =
        RhinoScriptSyntax.Fxrange (start, stop, step) |> Array.ofSeq
    
    
    ///<summary>Converts input into a Rhino.Geometry.Point3d if possible.</summary>
    ///<param name="pt">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>
    ///<returns>a Rhino.Geometry.Point3d. Fails on bad input</returns>
    static member Coerce3dPoint(pt:'T) : Point3d =               
        let inline  point3dOf3(x:^x, y:^y, z:^z) = 
            try Geometry.Point3d(floatOfObj (x),floatOfObj(y),floatOfObj(z))
            with _ -> failwithf "*** could not Coerce %A, %A and %A to Point3d" x y z
        
        let b = box pt
        match b with
        | :? Point3d    as pt               -> pt
        | :? Vector3d   as v                -> Point3d(v)
        | :? Point3f    as pt               -> Point3d(pt)
        | :? (float*float*float) as xyz     -> let x,y,z = xyz in Point3d(x,y,z)
        | :? (single*single*single) as xyz  -> let x,y,z = xyz in Point3d(float(x),float(y),float(z))        
        | :? (int*int*int) as xyz           -> let x,y,z = xyz in Point3d(float(x),float(y),float(z))
        | _ ->
            try
                match b with
                | :? (string*string*string) as xyz  -> let x,y,z = xyz in Point3d(parseFloatEnDe(x),parseFloatEnDe(y),parseFloatEnDe(z)) 
                | :? Guid as g ->  ((Doc.Objects.Find(g).Geometry) :?> Point).Location 
                | :? seq<float>  as xyz  ->  point3dOf3(Seq.item 0 xyz,Seq.item 3 xyz,Seq.item 2 xyz)
                | :? seq<int>  as xyz  ->    point3dOf3(Seq.item 0 xyz,Seq.item 3 xyz,Seq.item 2 xyz)
                | :? seq<string> as xyz  ->  point3dOf3(Seq.item 0 xyz,Seq.item 3 xyz,Seq.item 2 xyz)
                | :? string as s  -> 
                    let xs = s.Split(';')
                    if Seq.length xs > 2 then 
                        Point3d(parseFloatEnDe(Seq.item 0 xs),parseFloatEnDe(Seq.item 1 xs),parseFloatEnDe(Seq.item 2 xs))
                    else
                        let ys = s.Split(',') 
                        Point3d(parseFloatEnDe(Seq.item 0 ys),parseFloatEnDe(Seq.item 1 ys),parseFloatEnDe(Seq.item 2 ys))   
                |_ -> failwithf "*** could not Coerce %A to a Point3d" pt
            with _ ->
                failwithf "*** could not Coerce %A to a Point3d" pt
    
    
    ///<summary>Convert input into a Rhino.Geometry.Point2d if possible.</summary>
    ///<param name="point">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>
    ///<returns>a Rhino.Geometry.Point2d. Fails on bad input</returns>
    static member Coerce2dPoint(point:'point2d) =
        match box point with
        | :? Point2d    as point -> point
        | :? Point3d    as point -> Point2d(point.X, point.Y)
        | :? (float*float) as xy  -> let x,y = xy in Point2d(x,y)
        | _ -> failwithf "*** could not Coerce: Could not convert %A to a Point2d"  point
    
    ///<summary>Convert input into a Rhino.Geometry.Vector3d if possible.</summary>
    ///<param name="vector">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>    
    ///<returns>a Rhino.Geometry.Vector3d. Fails on bad input</returns>
    static member Coerce3dVector(vector:'T) : Vector3d =
        match box vector with
        | :? Vector3d   as v  -> v   
        | _ ->         
            try Vector3d(RhinoScriptSyntax.Coerce3dPoint(vector))
            with _ -> failwithf "*** could not Coerce: Could not convert %A to a Vector3d" vector
        
            
    ///<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible.</summary>
    ///<param name="ponits">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>
    ///<returns>Rhino.Geometry.Point3d seq. Fails on bad input</returns>
    static member Coerce3dPointList(points:'points) : Point3d seq=
        try Seq.map RhinoScriptSyntax.Coerce3dPoint points //|> Seq.cache
        with _ -> failwithf "*** could not Coerce: Could not convert %A to a list of 3d points"  points
        
    ///<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible.</summary>
    ///<param name="ponits">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, guid, or seq </param>
    ///<returns>Rhino.Geometry.Point2d seq. Fails on bad input</returns>
    static member Coerce2dPointList(points:'points) : Point2d seq=
        try Seq.map RhinoScriptSyntax.Coerce2dPoint points //|> Seq.cache
        with _ -> failwithf "*** could not Coerce: Could not convert %A to a list of 2d points"  points

    ///<summary>Convert input into a Rhino.Geometry.Plane if possible.</summary>
    ///<param name="plane">Plane,point, list, tuple</param>
    ///<returns>a Rhino.Geometry.Plane. Fails on bad input</returns>
    static member CoercePlane(plane:'plane) =
        match box plane with 
        | :? Plane  as plane -> plane // TODO add more
        | _ -> failwithf "*** could not Coerce: %A can not be converted to a Plane" plane
    
 
    ///<summary>Convert input into a Rhino.Geometry.Transform Transformation Matrix if possible.</summary>
    ///<param name="xform">object to convert</param>
    ///<returns>Rhino.Geometry.Transform. Fails on bad input</returns>   
    static member CoerceXform(xform:'xform) =
        match box xform with
        | :? Transform  as xform -> xform 
        | :? seq<seq<float>>  as xss -> // TODO verify row, column order !!
            let mutable t=Transform()
            for c,xs in Seq.indexed xss do
                for r,x in Seq.indexed xs do
                    t.[c,r] <- x
            t

        | :? ``[,]``<float>  as xss -> // TODO verify row, column order !!
            let mutable t=Transform()           
            xss|> Array2D.iteri (fun i j x -> t.[i,j]<-x)
            t

        | _ -> failwithf "*** could not Coercexform %A can not be converted to a Transformation Matrix" xform
    
    ///<summary>attempt to get a Guids from input</summary>
    ///<param name="id">objcts , guid or string</param>
    ///<returns>Guid. Fails on bad input</returns>
    static member CoerceGuid(id:'id) =
        match box id with
        | :? Guid  as g -> if Guid.Empty = g then fail() else g
        | :? string  as s -> try Guid.Parse s with _ -> failwithf "*** could not Coerceguid: string '%s' can not be converted to a Guid" s
        | :? DocObjects.RhinoObject as o -> o.Id
        | :? DocObjects.ObjRef      as o -> o.ObjectId
        | _ -> failwithf "*** could not Coerceguid:%A can not be converted to a Guid" id

    ///<summary>attempt to get a Sequence of Guids from input</summary>
    ///<param name="ids">list of guids</param>
    ///<returns>Guid seq. Fails on bad input</returns>
    static member CoerceGuidList(ids:'ids) =
        match box ids with
        | :? Guid  as g -> if Guid.Empty = g then fail() else [|g|] :> seq<Guid>
        | :? seq<obj> as gs -> 
            try gs |> Seq.map RhinoScriptSyntax.CoerceGuid
            with _ -> failwithf "*** could not Coerceguidlist: %A can not be converted to a Sequence(IEnumerable) of Guids" ids
        | _ -> failwithf "*** could not Coerceguidlist: %A can not be converted to a Sequence(IEnumerable) of Guids" ids
    
   
    ///<summary>attempt to get a System.Drawing.Color also works on natrural language color strings see Drawing.ColorTranslator.FromHtml </summary>
    ///<param name="color">string, tuple with  or 3 or 4 items.</param>
    ///<returns>System.Drawing.Color in ARGB form (not as named color) this will provide better comparison to other colors.
    /// For example the named color Red is not equal to fromRGB(255,0,0) . Fails on bad input</returns>
    static member CoerceColor(color:'color) =
        match box color with
        | :? Drawing.Color  as c -> Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B) //https://stackoverflow.com/questions/20994753/compare-two-color-objects
        | :? (int*int*int) as rgb       -> 
            let red , green, blue   = rgb 
            if red  <0 || red  >255 then failwithf " cannot create color form red %d, blue %d and green %d" red green blue
            if green<0 || green>255 then failwithf " cannot create color form red %d, blue %d and green %d" red green blue
            if blue <0 || blue >255 then failwithf " cannot create color form red %d, blue %d and green %d" red green blue
            Drawing.Color.FromArgb(255  ,red, green, blue)
            
        | :? (int*int*int*int) as argb  -> 
            let alpha, red , green, blue   = argb 
            if red  <0 || red  >255 then failwithf " cannot create color form red %d, blue %d and green  %d aplpha %d" red green blue alpha
            if green<0 || green>255 then failwithf " cannot create color form red %d, blue %d and green  %d aplpha %d" red green blue alpha
            if blue <0 || blue >255 then failwithf " cannot create color form red %d, blue %d and green  %d aplpha %d" red green blue alpha
            if alpha <0 || alpha >255 then failwithf " cannot create color form red %d, blue %d and green %d aplpha %d" red green blue alpha
            Drawing.Color.FromArgb(alpha  ,red, green, blue)        
        | :? string  as s -> 
            try 
                let c = Drawing.Color.FromName(s)
                Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
            with _ -> 
                try 
                    let c = Drawing.ColorTranslator.FromHtml(s)
                    Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
                with _ -> failwithf "*** could not Coerce %A to a Color" color
               //     try Windows.Media.ColorConverter.ConvertFromString(s)        
               //     with _ -> failwithf "*** could not Coerce %A to a Color" c
        | _ -> failwithf "*** could not Coerce %A to a Color" color

    ///<summary>attempt to get Rhino Line Geometry</summary>
    ///<param name="line">Line, two points or Guid</param>
    ///<returns>Geometry.Line. Fails on bad input</returns>
    static member CoerceLine(line:'line) =
        match box line with
        | :? Line as l -> l
        | :? Guid as g ->  
            match Doc.Objects.Find(g).Geometry with
            | :? Curve as crv ->
                if crv.IsLinear() then Line(crv.PointAtStart,crv.PointAtEnd)
                else failwithf "*** could not Coerce %A to a Line" line
            //| :? Line as l -> l
            | _ -> failwithf "*** could not Coerce %A to a Line" line
        | :? (Point3d*Point3d) as ab -> let a,b = ab in Line(a,b)
        // TODO parse 6 numbers, convert form polyline
        |_ -> failwithf "*** could not Coerce %A to a Line" line
    
    
    ///<summary>attempt to get RhinoObject from the document with a given id</summary>
    ///<param name="objectId">object identifier (Guid or string)</param>
    ///<returns>a RhinoObject. Fails on bad input</returns>
    static member CoerceRhinoObject(objectId:'id) =
        match box objectId with
        | :? Guid  as g -> 
            if Guid.Empty = g then failwith "*** could not CoerceObject: Empty Guid in RhinoScriptSyntax.CoerceRhinoObject" 
            else 
                let o = Doc.Objects.Find(g) 
                if isNull o then failwithf "*** could not CoerceObject: Guid %A not found in Object table (in RhinoScriptSyntax.CoerceRhinoObject)" g
                else o        
        | :? DocObjects.RhinoObject as o -> o
        | :? DocObjects.ObjRef as r -> r.Object()        
        | :? string as s -> 
                let g = RhinoScriptSyntax.CoerceGuid(s)
                if Guid.Empty = g then failwithf "*** could not CoerceObject: %s to a RhinoObject" s
                else 
                    let o = Doc.Objects.Find(g) 
                    if isNull o then failwithf "*** could not CoerceObject:Guid %s not found in Object table (in RhinoScriptSyntax.CoerceRhinoObject)" s
                    else o
        | _ -> failwithf "*** could not CoerceObject:  %A to a RhinoObject" objectId


    ///<summary>attempt to get GeometryBase class from given input</summary>
    ///<param name="id">geometry identifier (Guid or string)</param>
    ///<returns>Rhino.Geometry.GeometryBase. Fails on bad input</returns>
    static member CoerceGeometry(id:'id) =
        match box id with
        | :? GeometryBase as g -> g
        | :? Guid  as g -> (RhinoScriptSyntax.CoerceRhinoObject g).Geometry        
        | :? DocObjects.ObjRef as r -> r.Geometry()
        | :? DocObjects.RhinoObject as o -> o.Geometry
        | :? string as s -> (RhinoScriptSyntax.CoerceRhinoObject s).Geometry
        | _ -> failwithf "*** could not Coerce %A to a RhinoObject" id

    ///<summary>attempt to get polysurface geometry from the document with a given id</summary>
    ///<param name="id">id (Guid or string) to be RhinoScriptSyntax.Coerced into a brep</param>
    ///<returns>a Rhino.Geometry.Brep. Fails on bad input</returns>
    static member CoerceBrep(id:'id) =
        match RhinoScriptSyntax.CoerceGeometry(id) with 
        | :? Brep as b -> b
        | :? Extrusion as b -> b.ToBrep(true)
        | _ -> failwithf "*** could not Coerce %A to a Brep" id


    
    ///<summary>attempt to get curve geometry from the document with a given id.</summary>
    ///<param name="id">id (Guid or string) to be RhinoScriptSyntax.Coerced into a curve</param>
    ///<param name="segmentIndex">(int, optional) index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<returns>Rhino.Geometry.Curve. Fails on bad input</returns>
    static member CoerceCurve(id:'id, [<OPT;DEF(-1)>]segmentIndex:int): Curve = 
        if segmentIndex < 0 then 
            match RhinoScriptSyntax.CoerceGeometry(id) with 
            | :? Curve as c -> c
            | _ -> failwithf "*** could not Coerce %A to a Curve" id
        else
            match RhinoScriptSyntax.CoerceGeometry(id) with 
            | :? PolyCurve as c -> 
                let crv = c.SegmentCurve(segmentIndex)
                if isNull crv then failwithf "*** could not Coerce segment index %d from curve %A" segmentIndex id
                crv
            | :? Curve as c -> c
            | _ -> failwithf "*** could not Coerce %A to a Curve with segment index %d" id segmentIndex

  

    ///<summary>attempt to get surface geometry from the document with a given id</summary>
    ///<param name="objectId">objectId = the object's identifier</param>
    ///<returns>a Rhino.Geometry.Surface. Fails on bad input</returns>
    static member CoerceSurface(id:'id): Surface =
        match RhinoScriptSyntax.CoerceGeometry(id) with 
        | :? Surface as c -> c
        | :? Brep as b -> 
            if b.Faces.Count = 1 then b.Faces.[0] :> Surface
            else failwithf "*** could not Coerce %A to a Surface" id
        | _ -> failwithf "*** could not Coerce %A to a Surface" id

    ///<summary>attempt to get mesh geometry from the document with a given id</summary>
    ///<param name="objectId">object identifier (Guid or string)</param>
    ///<returns>a Rhino.Geometry.Mesh. Fails on bad input</returns>    
    static member CoerceMesh(id:'id) =
        match RhinoScriptSyntax.CoerceGeometry(id) with 
        | :? Mesh as m -> m        
        | _ -> failwithf "*** could not Coerce %A to a Mesh" id

    ///<summary>attempt to get Rhino LayerObject from the document with a given id or fullame</summary>
    ///<param name="nameOrId">(string or guid or index): layers identifier name</param>
    ///<returns>DocObjectys.Layer  Fails on bad input</returns>
    static member CoerceLayer (nameOrId:'nameOrId) : DocObjects.Layer=
        try
            match box nameOrId with 
            | :? Guid as g   -> Doc.Layers.FindId(g)
            | :? string as s -> Doc.Layers.[Doc.Layers.FindByFullPath(s, RhinoMath.UnsetIntIndex)]
            | :? int as ix   -> Doc.Layers.[ix]
            | _ -> fail()            
        with _ ->
            failwithf "*** could not Coerce Layer from '%A'" nameOrId    
    
    ///<summary>attempt to get Rhino LightObject from the document with a given id</summary>
    ///<param name="id">(guid): light identifier </param>
    ///<returns>a  Rhino.Geometry.Light. Fails on bad input</returns>
    static member CoerceLight (id:'id) =
        match RhinoScriptSyntax.CoerceGeometry id with
        | :? Geometry.Light as l -> l
        |_ -> failwithf "*** could not CoerceLight: %A is not a Geometry.Light " id
    
    
    ///<summary>attempt to get Rhino View Object from the name of the view </summary>
    ///<param name="view">(string): name of the view, empty string will return the Active view</param> 
    ///<returns>a Doc.View object. Fails on bad input</returns>
    static member CoerceView (view) =    
        if view = "" then Doc.Views.ActiveView
        else 
            let allviews = Doc.Views.GetViewList(true, true) |> Seq.filter (fun v-> v.MainViewport.Name = view)
            if Seq.length allviews = 1 then Seq.head allviews
            else  failwithf "*** could not CoerceView '%s'" view
    
    
    ///<summary>attempt to get Rhino Annotation Object</summary>
    ///<param name="id">(guid): id of annotation object</param> 
    ///<returns>a Rhino.DocObjects.AnnotationObjectBase. Fails on bad input.</returns>
    static member CoerceAnnotation (id:'id): DocObjects.AnnotationObjectBase =
        match RhinoScriptSyntax.CoerceRhinoObject id with
        | :?  DocObjects.AnnotationObjectBase as a -> a
        |_ -> failwithf "*** could not CoerceAnnotation: %A is not a Rhino.DocObjects.AnnotationObjectBase " id
        
    ///<summary>Returns the Rhino Block instance object for a given Id</summary>
    ///<param name="id">(Guid) Id of block instance</param>    
    ///<returns>(Rhino.DocObjects.InstanceObject) block instance object</returns>
    static member CoerceBlockInstanceObject(id:Guid) : Rhino.DocObjects.InstanceObject =
        match RhinoScriptSyntax.CoerceRhinoObject(id) with  
        | :? DocObjects.InstanceObject as b -> b
        | _ -> failwithf "unable to find Block InstanceObject for '%A'" id
    
    ///<summary>attempt to get Rhino PointCloud Geometry</summary>
    ///<param name="id">(guid): id of PointCloud object</param> 
    ///<returns>a Geometry.PointCloud. Fails on bad input.</returns>
    static member CoercePointCloud (id:'id) : PointCloud =
        match RhinoScriptSyntax.CoerceGeometry id with
        | :?  PointCloud as a -> a
        |_ -> failwithf "*** could not CoercePointCloud: %A =" id
                

    ///<summary>attempt to get TextDot Geometry</summary>
    ///<param name="id">(guid): id of TextDot object</param> 
    ///<returns>a Geometry.TextDot. Fails on bad input.</returns>
    static member CoerceTextDot (id:'id) : TextDot =
        match RhinoScriptSyntax.CoerceGeometry id with
        | :?  TextDot as a -> a
        |_ -> failwithf "*** could not TextDot: %A =" id


    ///<summary>attempt to get TextEntity Geometry</summary>
    ///<param name="id">(guid): id of TextEntity object</param> 
    ///<returns>a Geometry.TextEntity. Fails on bad input.</returns>
    static member CoerceTextEntity (id:'id) : TextEntity =
        match RhinoScriptSyntax.CoerceGeometry id with
        | :?  TextEntity as a -> a
        |_ -> failwithf "*** could not TextEntity: %A =" id






    //------try----


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
    ///<param name="id">id (Guid or string) to be RhinoScriptSyntax.Coerced into a curve</param>
    ///<param name="segmentIndex">(int, optional) index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<returns>Rhino.Geometry.Curve. Option</returns>
    static member TryCoerceCurve(id:Guid,[<OPT;DEF(-1)>]segmentIndex:int) : Curve option= 
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