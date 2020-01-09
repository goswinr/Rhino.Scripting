﻿namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.Util
open FsEx.UtilMath
open Rhino.Scripting.ActiceDocument
open System.Collections.Generic
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore 


module ExtrasVector =
               
    let inline scale (sc:float) (v:Vector3d) = v * sc  
    
    ///Same as reverse
    let inline flip  (v:Vector3d) = -v 
    
    ///Same as flip
    let inline reverse  (v:Vector3d) = -v     
    
    let inline distance (a:Point3d) (b:Point3d) = let v = a-b in sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z)
    
    let inline distanceSq (a:Point3d) (b:Point3d) = let v = a-b in    v.X*v.X + v.Y*v.Y + v.Z*v.Z

    let inline unitize (v:Vector3d) =
        let f = 1. / sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) in Vector3d(v.X*f, v.Y*f, v.Z*f)   
        
    let inline setLength (len:float) (v:Vector3d) =
        let f = len / sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) in Vector3d(v.X*f, v.Y*f, v.Z*f) 
            
    let inline orientUp (v:Vector3d) =
        if v.Z < 0.0 then -v else v
        
    let inline matchOrientation (orientToMatch:Vector3d) (v:Vector3d) =
        if orientToMatch * v < 0.0 then -v else v
    
    /// computes the signed Volume of a Tetrahedron
    let inline tetrahedronVolumeSigned(a:Point3d, b:Point3d, c:Point3d, d:Point3d) =
        ((Vector3d.CrossProduct( b-a, c-a)) * (d-a)) / 6.0

    ///project vector to plane
    let projectToPlane (pl:Plane) (v:Vector3d) =
        let pt = pl.Origin + v
        let clpt = pl.ClosestPoint(pt)
        clpt-pl.Origin
    
    /// project point onto line in directin of v
    /// fails if line is missed
    let projectToLine (ln:Line) (v:Vector3d) (pt:Point3d) =
        let h = Line(pt,v)
        let ok,tln,th = Intersect.Intersection.LineLine(ln,h)
        if not ok then failwithf "projectToLine: project in direction faild. (paralell?)"
        let a = ln.PointAt(tln)
        let b = h.PointAt(th)
        if (a-b).SquareLength > RhinoMath.ZeroTolerance then 
            Doc.Objects.AddLine ln   |> RhinoScriptSyntax.setLayer "projectToLine"
            Doc.Objects.AddLine h    |> RhinoScriptSyntax.setLayer "projectToLineDirection"
            Doc.Objects.AddPoint pt  |> RhinoScriptSyntax.setLayer "projectToLineFrom"            
            failwithf "projectToLine: missed Line by: %g " (a-b).Length
        a
    
    /// snap to point if within Doc.ModelAbsoluteTolerance
    let snapIfClose (snapTo:Point3d) (pt:Point3d) =
        if (snapTo-pt).Length < Doc.ModelAbsoluteTolerance then snapTo else pt

    /// Fails on zero length vectors
    let isAngleBelow1Degree(a:Vector3d, b:Vector3d) = //(prevPt:Point3d, thisPt:Point3d, nextPt:Point3d) =                        
        //let a = prevPt - thisPt 
        //let b = nextPt - thisPt                       
        let sa = a.SquareLength
        if sa < RhinoMath.SqrtEpsilon then 
            failwithf "Duplicate points: isAngleBelow1Degree: prevPt - thisPt: %s.SquareLength < RhinoMath.SqrtEpsilon; nextPt - thisPt:%s " a.ToNiceString b.ToNiceString
        let sb = b.SquareLength
        if sb < RhinoMath.SqrtEpsilon then 
            failwithf "Duplicate points: isAngleBelow1Degree: nextPt - thisPt: %s.SquareLength < RhinoMath.SqrtEpsilon; prevPt - thisPt:%s " b.ToNiceString a.ToNiceString
        let lena = sqrt sa
        let lenb = sqrt sb
        if lena < Doc.ModelAbsoluteTolerance then 
            failwithf "Duplicate points: isAngleBelow1Degree: prevPt - thisPt: %s < Doc.ModelAbsoluteTolerance: %f; nextPt - thisPt:%s " a.ToNiceString Doc.ModelAbsoluteTolerance b.ToNiceString
        if lenb < Doc.ModelAbsoluteTolerance then 
            failwithf "Duplicate points: isAngleBelow1Degree: nextPt - thisPt: %s < Doc.ModelAbsoluteTolerance: %f; prevPt - thisPt:%s " b.ToNiceString Doc.ModelAbsoluteTolerance a.ToNiceString  
        let au = a * (1.0 / lena)
        let bu = b * (1.0 / lenb)
        abs(bu*au) > 0.999847695156391 // = cosine of 1 degree (2 degrees would be =  0.999390827019096)
        
    ///Unitize vector, if input vector is shorter than 1e-6 alternative vector is returned UN-unitized.
    let inline unitizeWithAlternative (unitVectorAlt:Vector3d) (v:Vector3d) =
        let l = v.SquareLength
        if l < RhinoMath.ZeroTolerance  then  //sqrt RhinoMath.ZeroTolerance = 1e-06
            unitVectorAlt //|> unitize
        else  
            let f = 1.0 / sqrt(l)
            Vector3d(v.X*f , v.Y*f , v.Z*f)
                
    ///Every line has a normal vector in XY Plane.
    ///If line is vertical then XAxis
    ///result is unitized
    let normalOfTwoPointsInXY(fromPt:Point3d, toPt:Point3d) =
        let v = toPt - fromPt
        Vector3d.CrossProduct(v, Vector3d.ZAxis)
        |> unitizeWithAlternative Vector3d.XAxis
            
    /// distHor in XY plane, 
    /// distNormal in free Normal ( ZAxis for Flat line)
    let offsetTwoPt(    fromPt:Point3d, 
                        toPt:Point3d, 
                        distHor:float,
                        distNormal:float) : Point3d*Point3d= 
        let v = toPt - fromPt
        let normHor =
            Vector3d.CrossProduct(v, Vector3d.ZAxis)
            |> unitizeWithAlternative Vector3d.XAxis
                
        let normFree = 
            Vector3d.CrossProduct(v, normHor)
            |> unitize        
        
        let shift = distHor * normHor + distNormal * normFree
        fromPt +  shift, toPt + shift 
        
        
    ///Points  must not be colinear !
    let findOffsetCorner(   prevPt:Point3d,
                            thisPt:Point3d, 
                            nextPt:Point3d, 
                            prevDist:float, 
                            nextDist:float,
                            orientation:Vector3d) : Vector3d* Vector3d * Point3d * Vector3d=
        let vp = prevPt - thisPt
        let vn = nextPt - thisPt    
        if isAngleBelow1Degree(vp, vn) then 
            Vector3d.Zero, Vector3d.Zero, Point3d.Origin, Vector3d.Zero
        else 
            let n = 
                Vector3d.CrossProduct(vp, vn)
                |> unitize
                |> matchOrientation orientation            
                
            let sp = Vector3d.CrossProduct(vp, n) |> setLength prevDist 
            let sn = Vector3d.CrossProduct(n, vn) |> setLength nextDist    
            let lp = Line(thisPt + sp , vp)  //|>> (Doc.Objects.AddLine>>ignore)
            let ln = Line(thisPt + sn , vn)  //|>> (Doc.Objects.AddLine>> ignore)               
            let ok, tp , tn = Intersect.Intersection.LineLine(lp, ln) //could also be solved with trigonometry functions            
            if not ok then failwithf "findOffsetCorner: Intersect.Intersection.LineLine failed on %s and %s" lp.ToNiceString ln.ToNiceString
            sp, sn, lp.PointAt(tp), n  //or ln.PointAt(tn), should be same
             
                
        
    let rec internal searchBack i (ns:ResizeArray<Vector3d>) = 
        let ii = saveIdx (i) ns.Count
        let v = ns.[ii]
        if v <> Vector3d.Zero || i < -ns.Count then ii
        else searchBack (i-1) ns
        
    let rec internal searchForward i (ns:ResizeArray<Vector3d>) = 
        let ii = saveIdx (i) ns.Count
        let v = ns.[ii]
        if v <> Vector3d.Zero || i > (2 * ns.Count) then ii
        else searchForward (i + 1) ns        
        
            

    //[<Extension>] //Error 3246
    type RhinoScriptSyntax with
        
        ///projects to plane an retuns angle in degrees in plane betwen -180 and + 180
        [<Extension>]        
        static member AngleInPlane180( plane:Plane, vector:Vector3d):float  = 
            let v = projectToPlane plane vector |> unitize
            let dot = v * plane.XAxis 
            let ang = acos dot  |> toDegrees
            if v*plane.YAxis < 0.0 then -ang else ang

        ///projects to plane an retuns angle in degrees in plane betwen 0 and 360
        [<Extension>]        
        static member AngleInPlane360( plane:Plane, vector:Vector3d):float  = 
            let v = projectToPlane plane vector |> unitize
            let dot = v * plane.XAxis 
            let ang = acos dot  |> toDegrees
            if v*plane.YAxis < 0.0 then 360.0-ang else ang

        [<Extension>]
        static member DrawVector(   vector:Vector3d, 
                                    [<OPT;DEF(Point3d())>]fromPoint:Point3d, 
                                    [<OPT;DEF("")>]layer:string ) : unit  = 
            let l = RhinoScriptSyntax.AddLine(fromPoint, fromPoint + vector)
            RhinoScriptSyntax.CurveArrows(l, 2)
            if layer<>"" then  RhinoScriptSyntax.ObjectLayer(l, layer)
         
    
        [<Extension>]
        static member DrawPlane(    pl:Plane,
                                    [<OPT;DEF(1000.0)>]scale:float,
                                    [<OPT;DEF("")>]suffixInDot:string,
                                    [<OPT;DEF("")>]layer:string ) : unit  =         
            let a=RhinoScriptSyntax.AddLine(pl.Origin, pl.Origin + pl.XAxis*scale)
            let b=RhinoScriptSyntax.AddLine(pl.Origin, pl.Origin + pl.YAxis*scale)
            let c=RhinoScriptSyntax.AddLine(pl.Origin, pl.Origin + pl.ZAxis*scale*0.5)
            let e=RhinoScriptSyntax.AddTextDot("x"+suffixInDot, pl.Origin + pl.XAxis*scale)
            let f=RhinoScriptSyntax.AddTextDot("y"+suffixInDot, pl.Origin + pl.YAxis*scale)
            let g=RhinoScriptSyntax.AddTextDot("z"+suffixInDot, pl.Origin+ pl.ZAxis*scale*0.5)
            let gg=RhinoScriptSyntax.AddGroup()
            if layer <>"" then  RhinoScriptSyntax.ObjectLayer([a;b;c;e;f;g], layer)
            RhinoScriptSyntax.AddObjectToGroup([a;b;c;e;f;g], gg)
       

        [<Extension>]
        static member DistPt(from:Point3d, dir:Point3d, distance:float) : Point3d  =
            let v = dir - from
            let sc = distance/v.Length
            from + v*sc
    
        [<Extension>] //TODO add xml doc
        static member DivPt(from:Point3d, dir:Point3d, [<OPT;DEF(0.5)>]rel:float) : Point3d  =
            let v = dir - from
            from + v*rel
        

        [<Extension>]
        static member MeanPoint(pts:Point3d seq) : Point3d  =
            let mutable p = Point3d.Origin
            let mutable k = 0.0
            for pt in pts do
                k <- k + 1.0
                p <- p + pt
            p/k

    
        ///considers current order of points too, counterclockwise in xy plane is z
        [<Extension>]
        static member NormalOfPoints(pts:Point3d IList) : Vector3d  =
            let k = Seq.length pts
            if k < 2 then 
                failwithf "NormalOfPoints can't find normal of two or less points %s" pts.ToNiceString
            elif k = 3   then  
                let a = pts.[0] - pts.[1]
                let b = pts.[2] - pts.[1]
                let v= Vector3d.CrossProduct(b, a)
                if v.IsTiny() then failwithf "NormalOfPoints: three points are in a line  %s" pts.ToNiceString
                else
                    v.UnitizedUnchecked
            else
                let cen = RhinoScriptSyntax.MeanPoint(pts)
                let mutable v = Vector3d.Zero
                for t, n in Seq.thisNext pts do
                    let a = t-cen
                    let b = n-cen
                    v <- v + Vector3d.CrossProduct(a, b)                
                if v.IsTiny() then failwithf "NormalOfPoints: points are in a line  %s"  pts.ToNiceString
                else
                    v.UnitizedUnchecked

        
        ///Auto detects points from closed polylines and loops them
        ///Distance must have exact length, be singelton or empty seq
        [<Extension>] //TODO docstring
        static member OffsetPoints(     points:Point3d IList, 
                                        offsetDistances: float seq, 
                                        [<OPT;DEF(null:seq<float>)>] normalDistances: float seq,
                                        [<OPT;DEF(false)>]loop:bool) :Point3d  ResizeArray  =                                         
            let offDists0  = Array.ofSeq offsetDistances           
            let normDists0 = Array.ofSeq (normalDistances |? Seq.empty<float> )
            let pointk = points.Count
            let lastIndex = pointk - 1
            let lenDist = offDists0.Length
            let lenDistNorm = normDists0.Length                                        
            if pointk < 2 then 
                failwithf "OffsetPoints needs at least two points but %s given" points.ToNiceString                                        
            elif pointk = 2 then
                let offDist = 
                    if   lenDist = 0 then 0.0
                    elif lenDist = 1 then offDists0.[0]
                    else failwithf "OffsetPoints: offsetDistances has %d items but should have 1 or 0 for 2 given points %s" lenDist points.ToNiceString 
                let normDist = 
                    if   lenDistNorm = 0 then 0.0
                    elif lenDistNorm = 1 then normDists0.[0]
                    else failwithf "OffsetPoints: normalDistances has %d items but should have 1 or 0 for 2 given points %s" lenDistNorm points.ToNiceString         
                let a, b = offsetTwoPt(points.[0], points.[1] , offDist, normDist)
                resizeArray { a; b}                                        
            else // regular case more than 2 points
                let lastIsFirst = (points.[0] - points.Last).Length < Doc.ModelAbsoluteTolerance //auto detect closed polyline points:                                            
                let distsNeeded = 
                    if lastIsFirst then pointk - 1
                    elif loop      then pointk
                    else                pointk - 1                                            
                let distsNeededNorm = 
                    if lastIsFirst then pointk - 1
                    elif loop      then pointk
                    else                pointk   // not -1 !! 
                let  offDists = 
                    if   lenDist = 0 then             Array.create distsNeeded 0.0
                    elif lenDist = 1 then             Array.create distsNeeded offDists0.[0]
                    elif lenDist = distsNeeded then   offDists0
                    else failwithf"OffsetPoints: offsetDistances has %d items but should have %d (lastIsFirst=%b) (loop=%b)" lenDist distsNeeded lastIsFirst loop                                                
                let normDists = 
                    if   lenDistNorm = 0 then                 Array.create distsNeededNorm 0.0
                    elif lenDistNorm = 1 then                 Array.create distsNeededNorm normDists0.[0]
                    elif lenDistNorm = distsNeededNorm then   normDists0
                    else failwithf "OffsetPoints: normalDistances has %d items but should have %d (lastIsFirst=%b) (loop=%b)" lenDist distsNeededNorm lastIsFirst loop  
                let refNormal = RhinoScriptSyntax.NormalOfPoints(points) //to have good starting direction, first kink might be in bad direction   
                let Pts = ResizeArray<Point3d>(pointk) 
                let Ns = ResizeArray<Vector3d>(pointk)         
                for i, p, t, n in Seq.iPrevThisNext(points) do                                                 
                    // first one:
                    if i=0 then 
                        if lastIsFirst then 
                            let prev = points.GetItem(-2) // because -1 is same as 0                    
                            let _, _, pt, N = findOffsetCorner(prev, t, n, offDists.Last, offDists.[0], refNormal)
                            Pts.Add pt
                            Ns.Add N
                        else
                            let _, sn, pt, N = findOffsetCorner(p, t, n, offDists.Last, offDists.[0], refNormal)
                            Ns.Add N 
                            if loop then Pts.Add pt
                            else         Pts.Add (t + sn)                                                 
                    // last one:
                    elif i = lastIndex  then 
                        if lastIsFirst then
                            let _, _, pt, N = findOffsetCorner(p, t, points.[1], offDists.[i-1], offDists.[0], refNormal)
                            Pts.Add pt
                            Ns.Add N
                        elif loop then 
                            let _, _, pt, N = findOffsetCorner(p, t, n, offDists.[i-1], offDists.[i], refNormal)
                            Pts.Add pt
                            Ns.Add N
                        else 
                            let sp, _, _, N = findOffsetCorner(p, t, n, offDists.[i-1], offDists.[i-1], refNormal) // or any next off dist since only sp is used    
                            Pts.Add (t + sp)
                            Ns.Add N 
                    else
                        let _, _, pt, N = findOffsetCorner(p, t, n, offDists.[i-1], offDists.[i], refNormal)
                        Pts.Add pt
                        Ns.Add N                                            
                if lenDistNorm > 0 then 
                    for i=0 to  distsNeededNorm-1 do // ns might be shorter than pts if lastIsFirst= true
                        let n = Ns.[i]
                        if n <> Vector3d.Zero then 
                            Pts.[i] <- Pts.[i] + n * normDists.[i]
                // fix colinear segments by nearest neigbours that are ok
                for i, n in Seq.indexed Ns do // ns might be shorter than pts if lastIsFirst= true
                    if n = Vector3d.Zero then                 
                        let pi = searchBack (i-1) Ns
                        let ppt = Pts.[pi]
                        let pln = Line(points.[pi], points.[saveIdx (pi + 1) pointk])                
                        let pclp = pln.ClosestPoint(ppt, false)
                        let pv = ppt - pclp
                                                    
                        let ni = searchForward (i + 1) Ns
                        let npt = Pts.[ni]
                        let nln = Line(points.[ni], points.[saveIdx (ni-1) pointk])
                        let nclp = nln.ClosestPoint(npt, false)
                        let nv = npt - nclp
                        print (pi,"prev i")
                        print (i,"is colinear")
                        print (ni,"next i")
                        if offDists.[pi] <> offDists.[saveIdx (ni-1) distsNeeded] then 
                            failwithf "OffsetPoints: cant fix colinear at index %d with index %d and %d because offset distances are missmatching: %f, %f" i pi ni offDists.[pi] offDists.[saveIdx (ni-1) pointk]                                                     
                        Pts.[i] <- points.[i] + (nv + pv)*0.5                                            
                if lastIsFirst then Pts.[lastIndex] <- Pts.[0]
                Pts

        [<Extension>]
        ///auto detects points from closed polylines and loops them
        static member OffsetPoints(     points:Point3d IList, 
                                        offsetDistance: float, 
                                        [<OPT;DEF(0.0)>]normalDistance: float ,
                                        [<OPT;DEF(false)>]loop:bool) :Point3d  ResizeArray  = 
            if normalDistance = 0.0 then RhinoScriptSyntax.OffsetPoints(points,[offsetDistance],[]              , loop)
            else                         RhinoScriptSyntax.OffsetPoints(points,[offsetDistance],[normalDistance], loop)
                                        
        
        [<Extension>]     
        ///Calculates the intersection of a finite line with a triangle (without using Rhinocommon)        
        static member LineTriangleIntersect(line:Line, p1 :Point3d ,p2 :Point3d, p3 :Point3d):  Point3d option  = 
            // https://stackoverflow.com/questions/42740765/intersection-between-line-and-triangle-in-3d
            let q1 = line.From
            let q2 = line.To
            let s1 = sign (tetrahedronVolumeSigned(q1,p1,p2,p3))
            let s2 = sign (tetrahedronVolumeSigned(q2,p1,p2,p3))
            if s1 <> s2 then
                let s3 = sign (tetrahedronVolumeSigned(q1,q2,p1,p2))
                let s4 = sign (tetrahedronVolumeSigned(q1,q2,p2,p3))
                let s5 = sign (tetrahedronVolumeSigned(q1,q2,p3,p1))
                if s3 = s4 && s4 = s5 then                
                    let n = Vector3d.CrossProduct(p2-p1,p3-p1)
                    let t = ((p1-q1) * n) / ((q2-q1) * n)
                    Some (q1 + t * (q2-q1))
                else None
            else None


