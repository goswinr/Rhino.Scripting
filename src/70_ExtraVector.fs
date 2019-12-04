namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.Util
open FsEx.UtilMath
open Rhino.Scripting.ActiceDocument
open System.Collections.Generic


module ExtrasVector =
  
  let private normalOfTwoPt(a:Point3d, b:Point3d) =
    let v = b-a
    let p = Vector3d.CrossProduct(v,Vector3d.ZAxis)
    if p.Length<0.00001 then 
        Vector3d.XAxis
    else  
        let mutable tot = Vector3d.CrossProduct(v,p)
        tot.Unitize() |> ignore
        if tot.Z < 0.0 then 
            tot <- -tot
        tot


  type RhinoScriptSyntax with


    [<EXT>]
    static member DrawVector(   vector:Vector3d, 
                                [<OPT;DEF(Point3d())>]fromPoint:Point3d, 
                                [<OPT;DEF("")>]layer:string ) : unit  = 
        let l = RhinoScriptSyntax.AddLine(fromPoint,fromPoint+vector)
        RhinoScriptSyntax.CurveArrows(l,2)
        if layer<>"" then  RhinoScriptSyntax.ObjectLayer(l,layer)
         
    
    [<EXT>]
    static member DrawPlane(    pl:Plane,
                                [<OPT;DEF(1000.0)>]scale:float,
                                [<OPT;DEF("")>]suffixInDot:string,
                                [<OPT;DEF("")>]layer:string ) : unit  = 
        
        let a=RhinoScriptSyntax.AddLine(pl.Origin, pl.Origin+pl.XAxis*scale)
        let b=RhinoScriptSyntax.AddLine(pl.Origin, pl.Origin+pl.YAxis*scale)
        let c=RhinoScriptSyntax.AddLine(pl.Origin, pl.Origin+pl.ZAxis*scale*0.5)
        let e=RhinoScriptSyntax.AddTextDot("x"+suffixInDot, pl.Origin+pl.XAxis*scale)
        let f=RhinoScriptSyntax.AddTextDot("y"+suffixInDot, pl.Origin+pl.YAxis*scale)
        let g=RhinoScriptSyntax.AddTextDot("z"+suffixInDot, pl.Origin+ pl.ZAxis*scale*0.5)
        let gg=RhinoScriptSyntax.AddGroup()
        if layer <>"" then  RhinoScriptSyntax.ObjectLayer([a;b;c;e;f;g],layer)
        RhinoScriptSyntax.AddObjectToGroup([a;b;c;e;f;g],gg)
       

    [<EXT>]
    static member DistPt(from:Point3d, dir:Point3d, distance:float) : Point3d  =
        let v = dir - from
        let sc = distance/v.Length
        from + v*sc
    
    [<EXT>]
    static member DivPt(from:Point3d, dir:Point3d, rel:float) : Point3d  =
        let v = dir - from
        from + v*rel
        

    [<EXT>]
    static member MeanPoint(pts:Point3d seq) : Point3d  =
        let mutable p = Point3d.Origin
        let mutable k = 0.0
        for pt in pts do
            k <- k + 1.0
            p <- p + pt
        p/k

    
    ///considers current order of points too, counterclockwise in xy plane is z
    [<EXT>]
    static member NormalOfPoints(pts:Point3d IList) : Vector3d  =
        let k = Seq.length pts
        if k < 2 then 
            failwithf "NormalOfPoints can't find normal of two or less points %s" pts.ToNiceString
        elif k = 3   then  
            let a = pts.[0] - pts.[1]
            let b = pts.[2] - pts.[1]
            let v= Vector3d.CrossProduct(b,a)
            if v.IsTiny() then failwithf "NormalOfPoints: three points are in a line  %s" pts.ToNiceString
            else
                v.UnitizedUnchecked
        else
            let cen = RhinoScriptSyntax.MeanPoint(pts)
            let mutable v = Vector3d.Zero
            for t,n in Seq.thisNext pts do
                let a = t-cen
                let b = n-cen
                v <- v + Vector3d.CrossProduct(a,b)                
            if v.IsTiny() then failwithf "NormalOfPoints: points are in a line  %s"  pts.ToNiceString
            else
                v.UnitizedUnchecked


    ///auto detects points from closed polylines and loops them
    [<EXT>]
    static member OffsetPoints(     points:Point3d IList, 
                                    offsetDistances: float seq, 
                                    normalDistances: float seq,
                                    [<OPT;DEF(false)>]loop:bool) :Point3d  ResizeArray  =           
        
        if points.Count < 2 then failwithf "OffsetPoints needs at least two points but %s given" points.ToNiceString

        //auto detect closed polyline points:
        let lastIsFirst = 
            if (points.[0] - points.Last).Length < Doc.ModelAbsoluteTolerance then true         
            else false

        let arrD =
            let l = Seq.length offsetDistances
            if l = 0 then ResizeArray.create points.Count 0.0
            elif l = 1 then ResizeArray.create points.Count (Seq.head offsetDistances)
            elif l = points.Count then ResizeArray(offsetDistances)  
            else failwithf "OffsetPoints: offsetDistances has %d items but should have %d or 1" l points.Count

            
        let epts,arrD = 
            if lastIsFirst then  // always loop in this case
                resizeArray{ yield points.GetItem(-2) ; yield! points ; yield points.[1]  },  //end and strat exist twice
                resizeArray{ yield arrD.Last      ; yield! arrD ; yield arrD.[0] }

            elif loop then
                resizeArray{ yield points.Last    ; yield! points ; yield points.[0]  }, 
                resizeArray{ yield arrD.Last  ; yield! arrD ; yield arrD.[0] }
            else
                ResizeArray(points),arrD 

        // core
        let mutable ops = ResizeArray<Point3d>(points.Count) 
        let mutable N =   ResizeArray<Vector3d>(points.Count)  // normals

        if points.Count = 2 then 
            let norm=normalOfTwoPt(points.[0], points.[1])
            N.[0]<-norm
            N.[1]<-norm
            let v = points.[1]-points.[0]
            let mutable p = Vector3d.CrossProduct(v,Vector3d.ZAxis)
            if p.Length < 0.00001 then
                p <- Vector3d.XAxis
            p.Unitize() |> ignore
            ops.[0] <- points.[0] + arrD.[0] * p
            ops.[1] <- points.[1] + arrD.[0] * p
        else        

            let norm = RhinoScriptSyntax.NormalOfPoints(points)

            let lns = resizeArray { for t,n in Seq.thisNext(epts) do  Line(t,n)}
            let lasti = epts.LastIndex

            let mutable nPrev=Vector3d.ZAxis // Z as default ? for lines ?
            Seq.iPrevThisNext(epts)
            |> Seq.tryFind ( fun (i,p,t,n) ->   // this loop is only for having an inital value of nPrev
                    if i=0 || i = lasti then
                        false
                    else
                        let vn = n-t
                        let vp = p-t
                        let n = Vector3d.CrossProduct(vp,vn)
                        if n.Length > 0.00001 then  // not 3 points in Line
                            n.Unitize() |> ignore
                            nPrev<-n // side effect
                            true
                        else 
                            false ) |> ignore


            for i,p,t,n in Seq.iPrevThisNext(epts) do    
                if i=0 || i =lasti then
                    ()
                else
                    let vn = n-t
                    let vp = p-t
        
                    if vn.Length < Doc.ModelAbsoluteTolerance || vp.Length < Doc.ModelAbsoluteTolerance then
                        failwithf "Dulicate points in OffsetPoints"
        
        
                    let mutable n = Vector3d.CrossProduct(vp,vn)
                    if n.Length < 0.0001 then //3 points in Line
                        n <- nPrev
                    else                    
                        nPrev <- n.UnitizedUnchecked
        
                     //print n
                    if n * norm < 0.0 then
                        n <- -n
        
                    let mutable sp = Vector3d.CrossProduct(vp,n)  //direction for offset
                    let mutable sn = Vector3d.CrossProduct(n,vn)  //direction for offset
                    sp.Unitize() |> ignore
                    sn.Unitize() |> ignore
                    sn <- sn *  arrD.[i]
                    sp <- sn *  arrD.[i-1]
                    let lp = Line(t+sp,vp) 
                    let ln = Line(t+sn,vn)
                    let rc,a,b = Intersect.Intersection.LineLine(lp,ln)
                    let pt =
                        if rc then
                            lp.PointAt(a)  //or on b, should be same
                        else  // no intersection, paralell lines
                            if arrD.[i] = arrD.[i-1] then  //can offset coliniear points with same distance
                                t + sp  //or then + sn, should be same
                            else
                                failwithf "cannot offset coliniear points with variable distance"
                
                    ops.[i] <- pt
                    N.[i] <- n
        
                    if i=1 then
                        ops.[0] <- epts.[0] + sp
                        N.[0] <- n
        
                    if i=lasti-1 then
                        ops.[lasti] <- epts.[lasti] + sn
                        N.[lasti] <- n
        
       
        // end core
        if lastIsFirst || loop then 
            ops <- ops.[1 .. -1]
            N <- N.[1 .. -1]



        let lN = Seq.length normalDistances
        if lN>0 then
            let arrN =                
                if lN = 1 then ResizeArray.create points.Count (Seq.head normalDistances)
                elif lN = points.Count then ResizeArray(normalDistances)
                else failwithf "OffsetPoints: normalDistances has %d items but should have %d or 1" lN points.Count            
            for i in range(lN) do
                let mutable  n = N.[i] 
                n <- n *  arrN.[i]
                ops.[i] <- ops.[i] +  n 

        ops

        

