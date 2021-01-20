namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open FsEx.Util
open FsEx.UtilMath
open System.Globalization
open System.Collections.Generic
open FsEx
open FsEx.SaveIgnore
open System.Runtime.CompilerServices

// to expose CLI-standard extension members that can be consumed from C# or VB,
// http://www.latkin.org/blog/2014/04/30/f-extension-methods-in-roslyn/
// declare all Extension attributes explicitly
[<assembly:Extension>] do () 

/// An Integer Enum of Object types to be use in object selection functions
/// Don't create an instance, use the instance in RhinoScriptSyntax.Filter
[<Sealed>] //AbstractClass;
type ObjectFilterEnum internal () =  
    /// retuns 0
    member _.AllObjects = 0
    /// retuns 1
    member _.Point = 1
    /// retuns 2
    member _.PointCloud = 2
    /// retuns 4
    member _.Curve = 4
    /// retuns 8
    member _.Surface = 8
    /// retuns 16
    member _.PolySurface = 16
    /// retuns 32
    member _.Mesh = 32
    /// retuns 256
    member _.Light = 256
    /// retuns 512, for Text, leaders, and dimension lines
    member _.Annotation = 512
    /// retuns 4096, for block instances
    member _.Instance = 4096
    /// retuns 8192
    member _.Textdot = 8192
    /// retuns 16384
    member _.Grip = 16384
    /// retuns 32768, for detail view objects
    member _.Detail = 32768
    /// retuns 65536
    member _.Hatch = 65536
    /// retuns 131072
    member _.Morph = 131072
    /// retuns 262144
    member _.SubD = 262144
    /// retuns 134217728
    member _.Cage = 134217728
    /// retuns 268435456
    member _.Phantom = 268435456
    /// retuns 536870912
    member _.ClippingPlane = 536870912
    /// retuns 1073741824
    member _.Extrusion = 1073741824 


/// A static class with static members providing functions Identical to RhinoScript in Pyhton or VBscript 
[<AbstractClass; Sealed>]
type RhinoScriptSyntax private () =
    
   
    /// A Dictionary to store state between scripting session.
    /// use Sticky.Clear() to reset it.
    static member val Sticky = new Dictionary<string, obj>() with get
    
    /// An Integer Enum of Object types.
    /// to be use in object selection functions such as rs.GetObjects().
    static member val Filter = new ObjectFilterEnum ()
    
    ///Tests to see if the user has pressed the escape key.
    ///raises an OperationCanceledException.
    static member EscapeTest() : unit = //[<OPT;DEF(true)>]throwException:bool, [<OPT;DEF(false)>]reset:bool): bool =         
        RhinoApp.Wait() //does not need to be on  UI thread
        if escapePressed  then           
            escapePressed <- false //allways reset is needed otherwise in next run of sript will not be reset 
            raise (new OperationCanceledException("Esc key was pressed and caught via RhinoScriptSyntax.EscapeTest()"))

     


    ///<summary>Clamps a value between a lower and an upper bound</summary>
    ///<param name="minVal">(float): lower bound</param>
    ///<param name="maxVal">(float): upper bound</param>
    ///<param name="value">(float): the value to clamp</param>
    ///<returns>(float):clamped value</returns>
    static member Clamp (minVal:float, maxVal:float, value:float) : float =
        if minVal > maxVal then  RhinoScriptingException.Raise "RhinoScriptSyntax.Clamp: lowvalue %A must be less than highvalue %A" minVal maxVal
        max minVal (min maxVal value) 


    ///<summary>Like the Python 'xrange' function for integers this creates a range of floating point values.
    ///    The last or stop value will NOT be included in range as per python semantics, this is different from F# semantics on range expressions.
    ///    Use FsEx.UtilMath.floatRange(...) to include stop value in range</summary>
    ///<param name="start">(float): first value of range</param> 
    ///<param name="stop">(float): end of range (The last value will not be included in range, Python semantics.)</param>    
    ///<param name="step">(float): step size between two values</param>
    ///<returns>(float seq) a lazy seq of loats</returns>
    static member  FxrangePython (start:float, stop:float, step:float) : float seq =
        if isNanOrInf start then RhinoScriptingException.Raise "RhinoScriptSyntax.FxrangePython: NaN or Infinity, start=%f, step=%f, stop=%f" start step stop
        if isNanOrInf step  then RhinoScriptingException.Raise "RhinoScriptSyntax.FxrangePython: NaN or Infinity, start=%f, step=%f, stop=%f" start step stop
        if isNanOrInf stop  then RhinoScriptingException.Raise "RhinoScriptSyntax.FxrangePython: NaN or Infinity, start=%f, step=%f, stop=%f" start step stop
        let range = stop - start 
                    |> BitConverter.DoubleToInt64Bits 
                    |> (+) 5L // to make sure stop value is included in Range, will explicitly be removed below to match python semantics
                    |> BitConverter.Int64BitsToDouble
        let steps = range/step - 1.0 // -1 to make sure stop value is not included(python semanticsis diffrent from F# semantics on range expressions)
        if isNanOrInf steps then RhinoScriptingException.Raise "RhinoScriptSyntax.FxrangePython range/step in frange: %f / %f is NaN Infinity, start=%f, stop=%f" range step start stop
    
        if steps < 0.0 then 
            RhinoScriptingException.Raise "RhinoScriptSyntax.FxrangePython: Stop value cannot be reached: start=%f, step=%f, stop=%f (steps:%f)" start step stop steps //or Seq.empty
        else 
            // the actual algorithm: 
            let rec floatrange (start, i, steps) =
                seq { if i <= steps then 
                        yield start + i*step
                        yield! floatrange (start, (i + 1.0), steps) } // tail recursive ?
            floatrange (start, 0.0, steps) 

    ///<summary>Like the Python 'range' function for integers this creates a range of floating point values.
    ///    This last or stop value will NOT be included in range as per python semantics, this is different from F# semantics on range expressions.
    ///    Use FsEx.UtilMath.floatRange(...) to include stop value in range.</summary>
    ///<param name="start">(float): first value of range</param> 
    ///<param name="stop">(float): end of range( The last value will not be included in range, Python semantics.)</param>    
    ///<param name="step">(float): step size between two values</param>
    ///<returns>(float Rarr)</returns>
    static member FrangePython (start:float, stop:float, step:float) : float Rarr =
        RhinoScriptSyntax.FxrangePython (start, stop, step) |> Rarr.ofSeq
        
    
    ///<summary>Adds any geometry object (stuct or class) to the Rhino document.</summary>
    ///<param name="geo">the Geometry</param> 
    ///<returns>(Guid) The Guid of the added Object</returns>
    static member Add (geo:'AnyRhinoGeometry) : Guid = 
        match box geo with
        | :? GeometryBase as g -> Doc.Objects.Add(g)
        | :? Point3d    as pt->   Doc.Objects.AddPoint pt
        | :? Point3f    as pt->   Doc.Objects.AddPoint(pt)
        | :? Line       as ln->   Doc.Objects.AddLine(ln)
        | :? Arc        as a->    Doc.Objects.AddArc(a)
        | :? Circle     as c->    Doc.Objects.AddCircle(c)
        | :? Ellipse    as e->    Doc.Objects.AddEllipse(e)
        | :? Polyline   as pl ->  Doc.Objects.AddPolyline(pl)
        | :? Box        as b ->   Doc.Objects.AddBox(b)
        | :? BoundingBox as b ->  Doc.Objects.AddBox(Box(b))
        | :? Sphere     as b ->   Doc.Objects.AddSphere(b)
        | :? Cylinder    as cl -> Doc.Objects.AddSurface (cl.ToNurbsSurface())
        | :? Cone       as c ->   Doc.Objects.AddSurface (c.ToNurbsSurface())
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Add: object of type %A not implemented yet" (geo.GetType())
    

    //---------------------------------------------------------------
    //-------------------------------TRY COERCE-------------------
    //---------------------------------------------------------------


    ///<summary>Attempt to get a Guids from input</summary>
    ///<param name="objectId">objcts , Guid or string</param>
    ///<returns>a Guid Option</returns>
    static member TryCoerceGuid (objectId:Guid) : Guid option=
        match box objectId with
        | :? Guid  as g -> if Guid.Empty = g then None else Some g    
        | :? DocObjects.RhinoObject as o -> Some o.Id
        | :? DocObjects.ObjRef      as o -> Some o.ObjectId
        | :? string  as s -> let ok, g = Guid.TryParse s in  if ok then Some g else None
        | _ -> None

    ///<summary>Attempt to get RhinoObject from the document with a given objectId. Fails on empty Guid.</summary>
    ///<param name="objectId">object Identifier (Guid or string)</param>
    ///<returns>a RhinoObject Option</returns>
    static member TryCoerceRhinoObject (objectId:Guid): DocObjects.RhinoObject option =     
        if Guid.Empty = objectId then RhinoScriptingException.Raise "RhinoScriptSyntax.TryCoerceRhinoObject failed on empty Guid"
        else 
            let o = Doc.Objects.FindId(objectId) 
            if isNull o then None
            else Some o     
    


    //<summary>Attempt to get GeometryBase class from given Guid. Fails on empty Guid.</summary>   // it doe not make sens to have this just use rs.CoerceGeometry none is never returned
    //<param name="objectId">geometry Identifier (Guid)</param>
    //<returns>a Rhino.Geometry.GeometryBase Option</returns>
    //static member TryCoerceGeometry (objectId:Guid) :GeometryBase option =
    //    if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.TryCoerceGeometry failed on empty Guid"
    //    else
    //        match Doc.Objects.FindId(objectId) with 
    //        | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.TryCoerceGeometry: %A is not an object in Doc.Objects table" objectId
    //        | o -> Some o.Geometry   
            
    ///<summary>Attempt to get Rhino LightObject from the document with a given objectId</summary>
    ///<param name="objectId">(Guid): light Identifier</param>
    ///<returns>a Rhino.Geometry.Light. Option</returns>
    static member TryCoerceLight (objectId:Guid) : Light option =        
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :? Geometry.Light as l -> Some l
        | _ -> None

    ///<summary>Attempt to get Mesh class from given Guid. Fails on empty Guid.</summary>
    ///<param name="objectId">Mesh Identifier (Guid)</param>
    ///<returns>a Rhino.Geometry.Surface Option</returns>
    static member TryCoerceMesh (objectId:Guid) :Mesh option =
        if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.TryCoerceMesh failed on empty Guid"
        else
            match Doc.Objects.FindId(objectId) with 
            | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.TryCoerceMesh: %A is not an object in Doc.Objects table" objectId
            | o -> 
                match o.Geometry with 
                | :? Mesh as m -> Some m
                | _ -> None

    ///<summary>Attempt to get Surface class from given Guid. Fails on empty Guid.</summary>
    ///<param name="objectId">Surface Identifier (Guid)</param>
    ///<returns>a Rhino.Geometry.Surface Option</returns>
    static member TryCoerceSurface (objectId:Guid) :Surface option =
        if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.TryCoerceSurface failed on empty Guid"
        else
            match Doc.Objects.FindId(objectId) with 
            | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.TryCoerceSurface: %A is not an object in Doc.Objects table" objectId
            | o -> 
                match o.Geometry with          // TODO Extrusion of one curve too ?
                | :? Surface as c -> Some c
                | :? Brep as b -> 
                    if b.Faces.Count = 1 then Some (b.Faces.[0] :> Surface)
                    else None
                | _ -> None

    ///<summary>Attempt to get a Polysurface or Brep class from given Guid. Works on Extrusions too. Fails on empty Guid.</summary>
    ///<param name="objectId">Polysurface Identifier (Guid)</param>
    ///<returns>a Rhino.Geometry.Mesh Option</returns>
    static member TryCoerceBrep (objectId:Guid) :Brep option =
        if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.TryCoerceBrep failed on empty Guid"
        else
            match Doc.Objects.FindId(objectId) with 
            | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.TryCoerceBrep: %A is not an object in Doc.Objects table" objectId
            | o -> 
                match o.Geometry with
                | :? Brep as b ->  Some b
                | :? Extrusion as b -> Some (b.ToBrep(true))
                | _ -> None

    ///<summary>Attempt to get curve geometry from the document with a given objectId</summary>
    ///<param name="objectId">objectId (Guid or string) to be RhinoScriptSyntax.Coerced into a curve</param>
    ///<param name="segmentIndex">(int) Optional, index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<returns>a Rhino.Geometry.Curve Option</returns>
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

    
    ///<summary>Attempt to get Rhino Line Geometry using the current Documents Absolute Tolerance</summary>
    ///<param name="line">Line, two points or Guid</param>
    ///<returns>Geometry.Line) Fails on bad input</returns>
    static member TryCoerceLine(line:'T) : Line option =        
        match box line with
        | :? Line as l -> Some l
        | :? Curve as crv ->
            if crv.IsLinear(Doc.ModelAbsoluteTolerance) then Some <|  Line(crv.PointAtStart, crv.PointAtEnd)
                else None
        | :? Guid as g ->  
            match Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                if crv.IsLinear(Doc.ModelAbsoluteTolerance) then Some <| Line(crv.PointAtStart, crv.PointAtEnd)
                else None
            //| :? Line as l -> l
            | _ -> None
        | :? (Point3d*Point3d) as ab -> let a, b = ab in Some <| Line(a, b)
        |_ -> None
    
    ///<summary>Attempt to get Rhino Arc Geometry using the current Documents Absolute Tolerance.
    /// does not return circles as arcs.</summary>
    ///<param name="arc">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Arc Option</returns>
    static member TryCoerceArc(arc:'T) : Arc option=
        match box arc with
        | :? Arc as a -> Some(a)
        | :? Curve as crv ->
            let a = ref (new Arc())
            let ok = crv.TryGetArc(a,Doc.ModelAbsoluteTolerance)
            if ok then Some(!a )
            else None
        | :? Guid as g ->  
            match Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a = ref (new Arc())
                let ok = crv.TryGetArc(a,Doc.ModelAbsoluteTolerance)
                if ok then Some(!a )
                else None            
            | _ -> None        
        |_ -> None
    
    ///<summary>Attempt to get Rhino Circle Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="cir">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Circle Option</returns>
    static member TryCoerceCircle(cir:'T) : Circle option=
        match box cir with
        | :? Circle as a -> Some(a)
        | :? Curve as crv ->
            let a = ref (new Circle())
            let ok = crv.TryGetCircle(a,Doc.ModelAbsoluteTolerance)
            if ok then Some(!a )
            else None
        | :? Guid as g ->  
            match Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a = ref (new Circle())
                let ok = crv.TryGetCircle(a,Doc.ModelAbsoluteTolerance)
                if ok then Some(!a )
                else None            
            | _ -> None        
        |_ -> None

    ///<summary>Attempt to get Rhino Ellipse Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="cir">Guid, RhinoObject or Curve </param>
    ///<returns>a Geometry.Ellipse Option</returns>
    static member TryCoerceEllipse(cir:'T) :  Ellipse option=
        match box cir with
        | :? Ellipse as a -> Some(a)
        | :? Curve as crv ->
            let a = ref (new Ellipse())
            let ok = crv.TryGetEllipse(a,Doc.ModelAbsoluteTolerance)
            if ok then Some(!a )
            else None
        | :? Guid as g ->  
            match Doc.Objects.FindId(g).Geometry with
            | :? Curve as crv ->
                let a = ref (new  Ellipse())
                let ok = crv.TryGetEllipse(a,Doc.ModelAbsoluteTolerance)
                if ok then Some(!a )
                else None            
            | _ -> None        
        |_ -> None
        

    //-------------------------------------------------------
    //------------COERCE-----------------------------------
    //-------------------------------------------------------


    
    ///<summary>Attempt to get a Guids from input</summary>
    ///<param name="input">objects , Guid or string</param>
    ///<returns>Guid) Fails on bad input</returns>
    static member CoerceGuid(input:'T) : Guid =
        match box input with
        | :? Guid  as g -> if Guid.Empty = g then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuid: Guid is Empty"  else g
        | :? Option<Guid>  as go -> if go.IsNone || Guid.Empty = go.Value then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuid: Guid is Empty or None: %A" input else go.Value //from UI functions
        | :? string  as s -> try Guid.Parse s with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuid: string '%s' can not be converted to a Guid" s
        | :? DocObjects.RhinoObject as o -> o.Id
        | :? DocObjects.ObjRef      as o -> o.ObjectId
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuid: %A can not be converted to a Guid" input

    //<summary>Attempt to get a Sequence of Guids from input</summary>
    //<param name="Ids">list of Guids</param>
    //<returns>Guid seq) Fails on bad input</returns>
    //static member CoerceGuidList(Ids:'T):seq<Guid> =
    //    match box Ids with
    //    | :? Guid  as g -> if Guid.Empty = g then fail() else [|g|] :> seq<Guid>
    //    | :? seq<obj> as gs -> 
    //                        try    
    //                            gs |> Seq.map RhinoScriptSyntax.CoerceGuid
    //                        with _ -> 
    //                            RhinoScriptingException.Raise "RhinoScriptSyntax.: could not CoerceGuidList: %A can not be converted to a Sequence(IEnumerable) of Guids" Ids
    //    | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGuidList: could not CoerceGuidList: %A can not be converted to a Sequence(IEnumerable) of Guids" Ids
    
   
    ///<summary>Attempt to get a System.Drawing.Color also works on natrural language color strings see Drawing.ColorTranslator.FromHtml</summary>
    ///<param name="color">string, tuple with  or 3 or 4 items</param>
    ///<returns>System.Drawing.Color in ARGB form (not as named color) this will provIde better comparison to other colors.
    /// For example the named color Red is not equal to fromRGB(255, 0, 0) ) Fails on bad input</returns>
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
            if red  <0 || red  >255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green  %d aplpha %d" red green blue alpha
            if green<0 || green>255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green  %d aplpha %d" red green blue alpha
            if blue <0 || blue >255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green  %d aplpha %d" red green blue alpha
            if alpha <0 || alpha >255 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor: cannot create color form red %d, blue %d and green %d aplpha %d" red green blue alpha
            Drawing.Color.FromArgb(alpha, red, green, blue)        
        | :? string  as s -> 
            try 
                let c = Drawing.Color.FromName(s)
                Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
            with _ -> 
                try 
                    let c = Drawing.ColorTranslator.FromHtml(s)
                    Drawing.Color.FromArgb(int c.A, int c.R, int c.G, int c.B)// convert to unnamed color
                with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor:: could not Coerce %A to a Color" color
               //     try Windows.Media.ColorConverter.ConvertFromString(s)        
               //     with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.: could not Coerce %A to a Color" c
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceColor:: could not Coerce %A to a Color" color





    ///<summary>Attempt to get RhinoObject from the document with a given objectId</summary>
    ///<param name="objectId">(Guid) Object Identifier </param>
    ///<returns>a RhinoObject, Fails on bad input</returns>
    static member CoerceRhinoObject(objectId:Guid): DocObjects.RhinoObject =  
        //match box objectId with
        //| :? Guid  as g -> 
        //    if Guid.Empty = g then raise <|  RhinoScriptingException "RhinoScriptSyntax.CoerceRhinoObject: Empty Guid in RhinoScriptSyntax.CoerceRhinoObject" 
        //    else 
        //        let o = Doc.Objects.FindId(g) 
        //        if isNull o then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceRhinoObject: Guid %A not found in Object table (in RhinoScriptSyntax.CoerceRhinoObject)" g
        //        else o        
        //| :? DocObjects.RhinoObject as o -> o
        //| :? DocObjects.ObjRef as r -> r.Object()        
        //| :? string as s -> 
        //        let g = RhinoScriptSyntax.CoerceGuid(s)
        //        if Guid.Empty = g then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceRhinoObject: could not coerce %s to a RhinoObject" s
        //        else 
        //            let o = Doc.Objects.FindId(g) 
        //            if isNull o then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceRhinoObject: Guid %s not found in Object table (in RhinoScriptSyntax.CoerceRhinoObject)" s
        //            else o
        //| _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceRhinoObject: could not coerce %A to a RhinoObject" (rhType objectId)
        if Guid.Empty = objectId then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceRhinoObject failed on empty Guid"
        else 
            let o = Doc.Objects.FindId(objectId) 
            if isNull o then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceRhinoObject: The Guid %A was not found in the current Object table." objectId
            else o  


    ///<summary>Attempt to get Rhino LayerObject from the document with a given objectId or fullame</summary>
    ///<param name="nameOrId">(string or Guid or index): layers Identifier name</param>
    ///<returns>DocObjectys.Layer  Fails on bad input</returns>
    static member CoerceLayer (nameOrId:'T) : DocObjects.Layer=       
            match box nameOrId with
            | :? string as s -> 
                    let i = Doc.Layers.FindByFullPath(s, RhinoMath.UnsetIntIndex)
                    if i = RhinoMath.UnsetIntIndex then 
                        let lay = Doc.Layers.FindName s
                        if isNull lay then 
                            RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLayer: could find name '%s'" s
                        else
                            lay
                    else
                        Doc.Layers.[i]
            | :? Guid as g   -> 
                    let l = Doc.Layers.FindId(g)            
                    if isNull l then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLayer: could not Coerce Layer from Id '%A'" nameOrId  
                    l
            //| :? int as ix  -> // TODO better not allow ints here ??
            //        if ix<0 || ix >= Doc.Layers.Count then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLayer: could not find Layer at index %d from '%A'" ix nameOrId  
            //        Doc.Layers.[ix]

            | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLayer: could not Coerce Layer from '%A'" nameOrId    
    
    
        
    ///<summary>Returns the Rhino Block instance object for a given Id</summary>
    ///<param name="objectId">(Guid) Id of block instance</param>    
    ///<returns>(DocObjects.InstanceObject) block instance object</returns>
    static member CoerceBlockInstanceObject(objectId:Guid) : DocObjects.InstanceObject =
        match RhinoScriptSyntax.CoerceRhinoObject(objectId) with  
        | :? DocObjects.InstanceObject as b -> b
        | o -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceBlockInstanceObject: unable to find Block InstanceObject from %A '%A'" o.ObjectType objectId


    
    //-------------------views ---------------------

    ///<summary>Attempt to get Rhino View Object from the name of the view, can be a standart or page view</summary>
    ///<param name="nameOrId">(string or Guid): Name or Guid the view, empty string will return the Active view</param> 
    ///<returns>a Doc.View object) Fails on bad input</returns>
    static member CoerceView (nameOrId:'T) : Display.RhinoView =    
        match box nameOrId with
        | :? Guid as g ->
            let viewo = Doc.Views.Find(g)
            if isNull viewo then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceView: could not CoerceView  from '%A'" g
            else viewo
        
        | :? string as view ->       
            if isNull view then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceView: failed on null for view name input" // or Doc.Views.ActiveView
            elif view = "" then Doc.Views.ActiveView
            else 
                let allviews = Doc.Views.GetViewList(true, true) |> Array.filter (fun v-> v.MainViewport.Name = view)
                if allviews.Length = 1 then allviews.[0]
                else  RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceView: could not CoerceView '%s'" view
        
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Cannot get view from %A" nameOrId

    
    ///<summary>Attempt to get Rhino Page (or Layout) View Object from the name of the Layout</summary>
    ///<param name="nameOrId">(string): Name of the Layout</param> 
    ///<returns>a Doc.View object) Fails on bad input</returns>
    static member CoercePageView (nameOrId:'T) : Display.RhinoPageView =    
        match box nameOrId with
        | :? Guid as g ->
            let viewo = Doc.Views.Find(g)
            if isNull viewo then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceView: could not CoerceView  from '%A'" g
            else 
                try viewo :?> Display.RhinoPageView
                with _  -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceView: the view found '%A' is not a page view" viewo.MainViewport.Name
        
        | :? string as view ->       
            if isNull view then RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePageView: failed on null for view name input" // or Doc.Views.ActiveView
            else 
                let allviews = Doc.Views.GetPageViews()|> Array.filter (fun v-> v.PageName = view)
                let l = allviews.Length
                if allviews.Length = 1 then allviews.[0]
                elif allviews.Length > 1 then RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePageView: more than one page called '%s'" view
                else  RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePageView: Layout called '%s' not found" view
        
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Cannot get view from %A" nameOrId
    
    ///<summary>Attempt to get Detail view rectangle Geometry</summary>
    ///<param name="objectId">(Guid): objectId of Detail object</param> 
    ///<returns>a Geometry.DetailView) Fails on bad input</returns>
    static member CoerceDetailView (objectId:Guid) : DetailView =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  DetailView as a -> a
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceDetailView failed on %A : %A" g.ObjectType objectId            
    
    ///<summary>Attempt to get Detail view rectangle Object</summary>
    ///<param name="objectId">(Guid): objectId of Detail object</param> 
    ///<returns>a DocObjects.DetailViewObject) Fails on bad input</returns>
    static member CoerceDetailViewObject (objectId:Guid) : DocObjects.DetailViewObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.DetailViewObject as a -> a
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceDetailViewObject failed on %A : %A" g.ObjectType objectId
   
        
    //-------Annotation ----


    ///<summary>Attempt to get TextDot Geometry</summary>
    ///<param name="objectId">(Guid): objectId of TextDot object</param> 
    ///<returns>a Geometry.TextDot) Fails on bad input</returns>
    static member CoerceTextDot (objectId:Guid) : TextDot =
       match RhinoScriptSyntax.CoerceGeometry objectId with
       | :?  TextDot as a -> a
       | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceTextDot failed on: %s " (typeDescr objectId)

    ///<summary>Attempt to get TextEntity Geometry (for the text Object use rs.CoerceTextObject) </summary>
    ///<param name="objectId">(Guid): objectId of TextEntity object</param> 
    ///<returns>a Geometry.TextEntity) Fails on bad input</returns>
    static member CoerceTextEntity (objectId:Guid) : TextEntity =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  TextEntity as a -> a
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceTextEntity failed on: %s " (typeDescr objectId)


    ///<summary>Attempt to get Rhino TextObject Annotation Object</summary>
    ///<param name="objectId">(Guid): objectId of TextObject</param> 
    ///<returns>(DocObjects.TextObject) Fails on bad input</returns>
    static member CoerceTextObject (objectId:Guid): DocObjects.TextObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.TextObject as a -> a
        | o -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceTextObject failed on: %s " (typeDescr objectId)

    ///<summary>Attempt to get Hatch Geometry</summary>
    ///<param name="objectId">(Guid): objectId of Hatch object</param> 
    ///<returns>a Geometry.CoerceHatch) Fails on bad input</returns>
    static member CoerceHatch (objectId:Guid) : Hatch =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  Hatch as a -> a
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceHatch failed on: %s " (typeDescr objectId)


    ///<summary>Attempt to get Rhino Hatch Object</summary>
    ///<param name="objectId">(Guid): objectId of Hatch object</param> 
    ///<returns>(DocObjects.HatchObject) Fails on bad input</returns>
    static member CoerceHatchObject (objectId:Guid): DocObjects.HatchObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.HatchObject as a -> a
        | o -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceHatchObject failed on: %s " (typeDescr objectId)

    ///<summary>Attempt to get Rhino Annotation Base Object</summary>
    ///<param name="objectId">(Guid): objectId of annotation object</param> 
    ///<returns>(DocObjects.AnnotationObjectBase) Fails on bad input</returns>
    static member CoerceAnnotation (objectId:Guid): DocObjects.AnnotationObjectBase =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.AnnotationObjectBase as a -> a
        | o -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceAnnotation failed on: %s " (typeDescr objectId)


    ///<summary>Attempt to get Rhino Leader Annotation Object</summary>
    ///<param name="objectId">(Guid): objectId of Leader object</param> 
    ///<returns>(DocObjects.LeaderObject) Fails on bad input</returns>
    static member CoerceLeader (objectId:Guid): DocObjects.LeaderObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.LeaderObject as a -> a
        | o -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLeader failed on: %s " (typeDescr objectId)

    ///<summary>Attempt to get Rhino LinearDimension Annotation Object</summary>
    ///<param name="objectId">(Guid): objectId of LinearDimension object</param> 
    ///<returns>(DocObjects.LinearDimensionObject) Fails on bad input</returns>
    static member CoerceLinearDimension (objectId:Guid): DocObjects.LinearDimensionObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.LinearDimensionObject as a -> a
        | o -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLinearDimension failed on: %s " (typeDescr objectId)
    
    //---------Geometry (Base)------------


    ///<summary>Attempt to get GeometryBase class from given input</summary>
    ///<param name="objectId">(Guid) geometry Identifier </param>
    ///<returns>(Rhino.Geometry.GeometryBase) Fails on bad input</returns>
    static member CoerceGeometry(objectId:Guid) : GeometryBase =
        //match box object with
        //| :? GeometryBase as g -> g
        //| :? Guid  as g -> 
        //        if Guid.Empty = g then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGeometry failed on empty Guid"
        //        else 
        //            let o = Doc.Objects.FindId(g) 
        //            if isNull o then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGeometry failed: Guid %A not found in Object table." g else o.Geometry        
        //| :? option<Guid>  as go -> 
        //        if go.IsSome then 
        //            if Guid.Empty = go.Value then raise <|  RhinoScriptingException "RhinoScriptSyntax.CoerceGeometry failed on empty Guid"
        //            else 
        //                let o = Doc.Objects.FindId(go.Value) 
        //                if isNull o then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGeometry: Guid %A not found in Object table." go.Value else o.Geometry 
        //        else  RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGeometry: Could not coerce Option<Guid> None to a RhinoObject. This might be from cancelled UI interaction" 
        //| :? DocObjects.ObjRef as r -> r.Geometry()
        //| :? DocObjects.RhinoObject as o -> o.Geometry        
        //| _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGeometry: Could not Coerce %A to a Rhino.Geometry.GeometryBase base class. Is it a struct like Point3d or Plane? " object
        if Guid.Empty = objectId then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGeometry failed on empty Guid"        
        let o = Doc.Objects.FindId(objectId) 
        if isNull o then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceGeometry failed: Guid %A not found in Object table." objectId
        o.Geometry   
    
    ///<summary>Converts input into a Rhino.Geometry.Point3d if possible</summary>
    ///<param name="pt">Input to convert, Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    ///<returns>a Rhino.Geometry.Point3d, Fails on bad input</returns>
    static member Coerce3dPoint(pt:'T) : Point3d =               
        let inline  point3dOf3(x:^x, y:^y, z:^z) = 
            try Point3d(floatOfObj (x), floatOfObj(y), floatOfObj(z))
            with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dPoint: Could not Coerce the 3 values %A, %A and %A to a Point3d" x y z
        
        let b = box pt
        match b with
        | :? Point3d    as pt               -> pt
        | :? Point3f    as pt               -> Point3d(pt)
        //| :? Vector3d   as v                -> Point3d(v) //dont confuse vectors and points !
        | :? DocObjects.PointObject as po   -> po.PointGeometry.Location
        | :? TextDot as td                  -> td.Point
        | :? (float*float*float) as xyz     -> let x, y, z = xyz in Point3d(x, y, z)
        | :? (single*single*single) as xyz  -> let x, y, z = xyz in Point3d(float(x), float(y), float(z))        
        | :? (int*int*int) as xyz           -> let x, y, z = xyz in Point3d(float(x), float(y), float(z))        
        | _ ->
            try
                match b with
                | :? (Point3d option) as pto   -> pto.Value // from UI function
                | :? option<Guid> as go      -> ((Doc.Objects.FindId(go.Value).Geometry) :?> Point).Location
                | :? (string*string*string) as xyz  -> let x, y, z = xyz in Point3d(parseFloatEnDe(x), parseFloatEnDe(y), parseFloatEnDe(z)) 
                | :? Guid as g ->  ((Doc.Objects.FindId(g).Geometry) :?> Point).Location 
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
                |_ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dPoint failed on: %s " (typeDescr pt)
            with _ ->
                RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dPoint failed on: %s " (typeDescr pt)
    
    ///<summary>Attempt to get Rhino Point Object</summary>
    ///<param name="objectId">(Guid): objectId of Point object</param> 
    ///<returns>a DocObjects.PointObject, Fails on bad input</returns>
    static member CoercePointObject (objectId:Guid) : DocObjects.PointObject =
        match RhinoScriptSyntax.CoerceRhinoObject objectId with
        | :?  DocObjects.PointObject as a -> a
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePointObject failed on: %s " (rhType objectId)

    ///<summary>Convert input into a Rhino.Geometry.Point2d if possible</summary>
    ///<param name="point">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    ///<returns>a Rhino.Geometry.Point2d, Fails on bad input</returns>
    static member Coerce2dPoint(point:'T) : Point2d =
        match box point with
        | :? Point2d    as point -> point
        | :? Point3d    as point -> Point2d(point.X, point.Y)
        | :? (float*float) as xy  -> let x, y = xy in Point2d(x, y)
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce2dPoint: could not Coerce: Could not convert %A to a Point2d"  point
    
    ///<summary>Convert input into a Rhino.Geometry.Vector3d if possible</summary>
    ///<param name="vec">input to convert, Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>    
    ///<returns> aRhino.Geometry.Vector3d, Fails on bad input</returns>
    static member Coerce3dVector(vec:'T) : Vector3d =
        let inline vecOf3(x:^x, y:^y, z:^z) = 
            try Vector3d(floatOfObj (x), floatOfObj(y), floatOfObj(z))
            with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dVector: Could not Coerce %A, %A and %A to Vector3d" x y z
        
        let b = box vec
        match b with
        | :? Vector3d   as v                -> v
        | :? Vector3f   as v                -> Vector3d(v)
        | :? (float*float*float) as xyz     -> let x, y, z = xyz in Vector3d(x, y, z)
        | :? (single*single*single) as xyz  -> let x, y, z = xyz in Vector3d(float(x), float(y), float(z))        
        | :? (int*int*int) as xyz           -> let x, y, z = xyz in Vector3d(float(x), float(y), float(z))         
        | _ ->
            try
                match b with
                | :? Guid as g ->  (Doc.Objects.FindId(g).Geometry :?> LineCurve).Line.Direction
                | :? (Vector3d option) as v   -> v.Value // from UI function
                | :? option<Guid> as go      -> (Doc.Objects.FindId(go.Value).Geometry :?> LineCurve).Line.Direction
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
                |_ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dVector failed on: %s " (typeDescr vec)
            with _ ->
                RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dVector failed on: %s " (typeDescr vec)
        
            
    //<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible</summary>
    //<param name="ponits">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    //<returns>(Rhino.Geometry.Point3d seq) Fails on bad input</returns>
    //static member Coerce3dPointList(points:'T) : Point3d seq=
    //    try Seq.map RhinoScriptSyntax.Coerce3dPoint points //|> Seq.cache
    //    with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce3dPointList: could not Coerce: Could not convert %A to a list of 3d points"  points
        
    //<summary>Convert input into a Rhino.Geometry.Point3d sequence if possible</summary>
    //<param name="ponits">input to convert, list of , Point3d, Vector3d, Point3f, Vector3f, str, Guid, or seq</param>
    //<returns>(Rhino.Geometry.Point2d seq) Fails on bad input</returns>
    //static member Coerce2dPointList(points:'T) : Point2d seq=
    //    try Seq.map RhinoScriptSyntax.Coerce2dPoint points //|> Seq.cache
    //    with _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.Coerce2dPointList: could not Coerce: Could not convert %A to a list of 2d points"  points


    ///<summary>Convert input into a Rhino.Geometry.Plane if possible</summary>
    ///<param name="plane">Plane, point, list, tuple</param>
    ///<returns>(Rhino.Geometry.Plane) Fails on bad input</returns>
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
                RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePlane failed on: %s " (typeDescr plane) 
 
    ///<summary>Convert input into a Rhino.Geometry.Transform Transformation Matrix if possible</summary>
    ///<param name="xForm">object to convert</param>
    ///<returns>(Rhino.Geometry.Transform) Fails on bad input</returns>   
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
                | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceXform: seq<seq<float>> %s can not be converted to a Transformation Matrix" (NiceString.toNiceString xForm)
            t        
        | :? ``[,]``<float>  as xss -> // TODO verify row, column order !!
            let mutable t= Transform()           
            try
                xss|> Array2D.iteri (fun i j x -> t.[i, j]<-x)
            with
                | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceXform: Array2D %s can not be converted to a Transformation Matrix" (NiceString.toNiceString xForm)
            t
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceXform: could not CoerceXform %s can not be converted to a Transformation Matrix" (NiceString.toNiceString xForm)

    ///<summary>Attempt to get Rhino Line Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="line">Line, two points or Guid</param>
    ///<returns>Geometry.Line, Fails on bad input</returns>
    static member CoerceLine(line:'T) : Line=
        match RhinoScriptSyntax.TryCoerceLine(line) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLine failed on: %s " (typeDescr line)
        
    ///<summary>Attempt to get Rhino Arc Geometry using the current Documents Absolute Tolerance.
    /// does not return circles as arcs.</summary>
    ///<param name="arc">Guid, RhinoObject or Curve </param>
    ///<returns>Geometry.Arc, Fails on bad input</returns>
    static member CoerceArc(arc:'T) : Arc=
        match RhinoScriptSyntax.TryCoerceArc(arc) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceArc failed on: %s " (typeDescr arc)

    ///<summary>Attempt to get Rhino Circle Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="circ">Guid, RhinoObject or Curve </param>
    ///<returns>Geometry.Circle, Fails on bad input</returns>
    static member CoerceCircle(circ:'T) : Circle=
        match RhinoScriptSyntax.TryCoerceCircle(circ) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceCircle failed on: %s " (typeDescr circ)

    ///<summary>Attempt to get Rhino Ellips Geometry using the current Documents Absolute Tolerance.</summary>
    ///<param name="ellip">Guid, RhinoObject or Curve </param>
    ///<returns>Geometry.Ellipse, Fails on bad input</returns>
    static member CoerceEllips(ellip:'T) : Ellipse=
        match RhinoScriptSyntax.TryCoerceEllipse(ellip) with
        | Some a -> a
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceEllipse failed on: %s " (typeDescr ellip)      
    
    ///<summary>Attempt to get polysurface geometry from the document with a given objectId</summary>
    ///<param name="objectId">objectId (Guid or string) to be RhinoScriptSyntax.Coerced into a brep</param>
    ///<returns>(Rhino.Geometry.Brep) Fails on bad input</returns>
    static member CoerceBrep(objectId:Guid) : Brep  =
        match RhinoScriptSyntax.TryCoerceBrep(objectId) with 
        | Some b -> b
        | None -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceBrep failed on: %s " (typeDescr objectId)


    
    ///<summary>Attempt to get curve geometry from the document with a given objectId</summary>
    ///<param name="objectId">objectId (Guid or string) to be RhinoScriptSyntax.Coerced into a curve</param>
    ///<param name="segmentIndex">(int) Optional, index of segment to retrieve. To ignore segmentIndex give -1 as argument</param>
    ///<returns>(Rhino.Geometry.Curve) Fails on bad input</returns>
    static member CoerceCurve(objectId:Guid, [<OPT;DEF(-1)>]segmentIndex:int): Curve = 
        if segmentIndex < 0 then 
            match RhinoScriptSyntax.CoerceGeometry(objectId) with 
            | :? Curve as c -> c
            | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceCurve failed on: %s " (typeDescr objectId)
        else
            match RhinoScriptSyntax.CoerceGeometry(objectId) with 
            | :? PolyCurve as c -> 
                let crv = c.SegmentCurve(segmentIndex)
                if isNull crv then RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceCurve failed on segment index %d for %s" segmentIndex   (typeDescr crv)
                crv
            | :? Curve as c -> c
            | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceCurve failed for %s"  (typeDescr objectId)

  

    ///<summary>Attempt to get surface geometry from the document with a given objectId</summary>
    ///<param name="objectId">the object's Identifier</param>
    ///<returns>(Rhino.Geometry.Surface) Fails on bad input</returns>
    static member CoerceSurface(objectId:Guid): Surface =
        match RhinoScriptSyntax.CoerceGeometry(objectId) with 
        | :? Surface as c -> c
        | :? Brep as b -> 
            if b.Faces.Count = 1 then b.Faces.[0] :> Surface
            else RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceSurface failed on %A from Brep with %d Faces" objectId b.Faces.Count
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceSurface failed on: %A " objectId

    ///<summary>Attempt to get surface geometry from the document with a given objectId</summary>
    ///<param name="objectId">the object's Identifier</param>
    ///<returns>(Rhino.Geometry.Surface) Fails on bad input</returns>
    static member CoerceNurbsSurface(objectId:Guid): NurbsSurface =
        match RhinoScriptSyntax.CoerceGeometry(objectId) with 
        | :? NurbsSurface as s -> s
        | :? Surface as c -> c.ToNurbsSurface()
        | :? Brep as b -> 
            if b.Faces.Count = 1 then (b.Faces.[0] :> Surface).ToNurbsSurface()
            else RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceNurbsSurface failed on %s from Brep with %d Faces" (typeDescr objectId)   b.Faces.Count
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceNurbsSurface failed on: %A "  objectId


    ///<summary>Attempt to get mesh geometry from the document with a given objectId</summary>
    ///<param name="objectId">object Identifier (Guid or string)</param>
    ///<returns>(Rhino.Geometry.Mesh) Fails on bad input</returns>    
    static member CoerceMesh(objectId:Guid) : Mesh =
        match RhinoScriptSyntax.CoerceGeometry(objectId) with 
        | :? Mesh as m -> m        
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceMesh failed on: %s " (typeDescr objectId)


    ///<summary>Attempt to get Rhino LightObject from the document with a given objectId</summary>
    ///<param name="objectId">(Guid): light Identifier</param>
    ///<returns>a  Rhino.Geometry.Light) Fails on bad input</returns>
    static member CoerceLight (objectId:Guid) : Light =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :? Geometry.Light as l -> l
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoerceLight failed on: %s " (typeDescr objectId)


    ///<summary>Attempt to get Rhino PointCloud Geometry</summary>
    ///<param name="objectId">(Guid): objectId of PointCloud object</param> 
    ///<returns>a Geometry.PointCloud) Fails on bad input</returns>
    static member CoercePointCloud (objectId:Guid) : PointCloud =
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :?  PointCloud as a -> a
        | g -> RhinoScriptingException.Raise "RhinoScriptSyntax.CoercePointCloud failed on: %s " (typeDescr objectId)
                

    