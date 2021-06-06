namespace Rhino.Scripting

open System
open System.Collections.Generic
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore 
open FsEx.ExtensionsIList

open Rhino
open Rhino.Geometry



[<AutoOpen>]
/// This module provides functions to manipulate Rhino Vector3d
/// This module is automatically opened when Rhino.Scripting namespace is opened.
module AutoOpenVector =
    
    open Vec

    type RhinoScriptSyntax with
        (*
        [<Extension>] 
        [<Obsolete>]
        /// Use Vec.angle .. instead
        ///projects to Plane an returns angle in degrees in Plane between -180 and + 180               
        static member AngleInPlane180( plane:Plane, vector:Vector3d) : float  = 
            let v = projectToPlane plane vector |> unitize
            let dot = v * plane.XAxis 
            let ang = acos dot  |> toDegrees
            if v*plane.YAxis < 0.0 then -ang else ang
        
        [<Extension>]
        [<Obsolete>]
        /// Use Vec.angle .. instead
        ///projects to Plane an returns angle in degrees in Plane between 0 and 360               
        static member AngleInPlane360( plane:Plane, vector:Vector3d) : float  = 
            let v = projectToPlane plane vector |> unitize
            let dot = v * plane.XAxis 
            let ang = acos dot  |> toDegrees
            if v*plane.YAxis < 0.0 then 360.0-ang else ang
        *)

        /// Draws a line with a Curve Arrows
        [<Extension>]
        static member DrawVector(   vector:Vector3d, 
                                    fromPoint:Point3d, 
                                    [<OPT;DEF("")>]layer:string ) : unit  =             
            let l =RhinoScriptSyntax.AddLine(fromPoint, fromPoint + vector )
            RhinoScriptSyntax.CurveArrows(l, 2)
            if layer<>"" then
                RhinoScriptSyntax.ObjectLayer(l, layer, createLayerIfMissing=true)
        
        /// Draws a line with a Curve Arrows
        [<Extension>]
        static member DrawVector( vector:Vector3d) : unit  =             
            let l = RhinoScriptSyntax.AddLine(Point3d.Origin, Point3d.Origin + vector )
            RhinoScriptSyntax.CurveArrows(l, 2)
            
    
        /// Draws the axes of a Plane and adds TextDots to lable them.
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
       

        /// returns a point that is at a given distance from a point in the direction of another point. 
        [<Extension>]
        static member DistPt(fromPt:Point3d, dirPt:Point3d, distance:float) : Point3d  =
            let v = dirPt - fromPt
            let sc = distance/v.Length
            fromPt + v*sc
    
        /// returns a Point by evaluation a line between two point with a normalized patrameter.
        /// e.g. rel=0.5 will return the middle point, rel=1.0 the endPoint
        /// if the rel parameter is omitted it is set to 0.5
        [<Extension>] 
        static member DivPt(fromPt:Point3d, toPt:Point3d, [<OPT;DEF(0.5)>]rel:float) : Point3d  =
            let v = toPt - fromPt
            fromPt + v*rel
        

        /// returns the averge of many points
        [<Extension>]
        static member MeanPoint(pts:Point3d seq) : Point3d  =
            let mutable p = Point3d.Origin
            let mutable k = 0.0
            for pt in pts do
                k <- k + 1.0
                p <- p + pt
            p/k

        /// Finds the mean normal of many points.
        /// It finds the center point and then takes corossproducts iterating all points in pairs of two.
        /// The first two points define the orientation of the normal.
        /// Considers current order of points too, counterclockwise in xy Plane is z        
        [<Extension>]
        static member NormalOfPoints(pts:Point3d IList) : Vector3d  =            
            if Seq.hasMaximumItems 2 pts then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.NormalOfPoints can't find normal of two or less points %s" (Print.nice pts)
            elif Seq.hasItems 3 pts   then  
                let a = pts.[0] - pts.[1]
                let b = pts.[2] - pts.[1]
                let v= Vector3d.CrossProduct(b, a)
                if v.IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.NormalOfPoints: three points are in a line  %s" (Print.nice pts)
                else
                    v.Unitized
            else
                let cen = RhinoScriptSyntax.MeanPoint(pts)
                let mutable v = Vector3d.Zero
                for t, n in Seq.thisNext pts do
                    let a = t-cen
                    let b = n-cen
                    let x = Vector3d.CrossProduct(a, b)  |> Vec.matchOrientation v // TODO do this matching?
                    v <- v + x              
                if v.IsTiny() then RhinoScriptingException.Raise "RhinoScriptSyntax.NormalOfPoints: points are in a line  %s" (Print.nice pts)
                else
                    v.Unitized

        /// Calculates the intersection of a finite line with a triangle (without using Rhinocommon) 
        /// Returns Some(Point3d) or None if no intersection found
        [<Extension>]     
        static member LineTriangleIntersect(line:Line, p1 :Point3d ,p2 :Point3d, p3 :Point3d) : Point3d option  = 
            
            // https://stackoverflow.com/questions/42740765/intersection-between-line-and-triangle-in-3d
            /// computes the signed Volume of a Tetrahedron
            let inline tetrahedronVolumeSigned(a:Point3d, b:Point3d, c:Point3d, d:Point3d) =
                ((Vector3d.CrossProduct( b-a, c-a)) * (d-a)) / 6.0

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
        
        /// Offsets a Polyline in 3D space by finding th local offest in each corner.
        /// Offset distances can vary per segment, Positive distance is offset inwards, negative outwards.
        /// Normal distances define a perpendicular offset at each corner.
        /// Auto detects if given points are from a closed Polyline (first point = last point) and loops them
        /// Distances Sequence  must have exact count , be a singelton ( for repeating) or empty seq ( for ignoring)
        /// Auto detects points from closed polylines and loops them
        [<Extension>] 
        static member OffsetPoints(     points: IList<Point3d>,  // IList so it can take a Point3dList class too
                                        offsetDistances: float seq, 
                                        [<OPT;DEF(null:seq<float>)>] normalDistances: float seq,
                                        [<OPT;DEF(false)>]loop:bool) :Point3d  Rarr  =       
                                        
            let offDists0  = Array.ofSeq offsetDistances           
            let normDists0 = Array.ofSeq (normalDistances |? Seq.empty<float> )
            let pointk = points.Count
            let lastIndex = pointk - 1
            let lenDist = offDists0.Length
            let lenDistNorm = normDists0.Length                                        
            if pointk < 2 then 
                RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetPoints needs at least two points but %s given" (Print.nice points)                                       
            elif pointk = 2 then
                let offDist = 
                    if   lenDist = 0 then 0.0
                    elif lenDist = 1 then offDists0.[0]
                    else RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetPoints: offsetDistances has %d items but should have 1 or 0 for 2 given points %s" lenDist (Print.nice points)
                let normDist = 
                    if   lenDistNorm = 0 then 0.0
                    elif lenDistNorm = 1 then normDists0.[0]
                    else RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetPoints: normalDistances has %d items but should have 1 or 0 for 2 given points %s" lenDistNorm (Print.nice points)
                let a, b = Pnt.offsetTwoPt(points.[0], points.[1] , offDist, normDist)
                rarr { a; b}                                        
            else // regular case more than 2 points
                let lastIsFirst = (points.[0] - points.Last).Length < State.Doc.ModelAbsoluteTolerance //auto detect closed polyline points:                                            
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
                    else RhinoScriptingException.Raise "OffsetPoints: offsetDistances has %d items but should have %d (lastIsFirst=%b) (loop=%b)" lenDist distsNeeded lastIsFirst loop                                                
                let normDists = 
                    if   lenDistNorm = 0 then                 Array.create distsNeededNorm 0.0
                    elif lenDistNorm = 1 then                 Array.create distsNeededNorm normDists0.[0]
                    elif lenDistNorm = distsNeededNorm then   normDists0
                    else RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetPoints: normalDistances has %d items but should have %d (lastIsFirst=%b) (loop=%b)" lenDist distsNeededNorm lastIsFirst loop  
                let refNormal = RhinoScriptSyntax.NormalOfPoints(points) //to have good starting direction, first kink might be in bad direction   
                let Pts = Rarr<Point3d>(pointk) 
                let Ns = Rarr<Vector3d>(pointk)         
                for i, p, t, n in Seq.iPrevThisNext(points) do                                                 
                    // first one:
                    if i=0 then 
                        if lastIsFirst then 
                            let prev = points.GetNeg(-2) // because -1 is same as 0                    
                            let struct( _, _, pt, N) = Pnt.findOffsetCorner(prev, t, n, offDists.Last, offDists.[0], refNormal)
                            Pts.Add pt
                            Ns.Add N
                        else
                            let struct( _, sn, pt, N) = Pnt.findOffsetCorner(p, t, n, offDists.Last, offDists.[0], refNormal)
                            Ns.Add N 
                            if loop then Pts.Add pt
                            else         Pts.Add (t + sn)                                                 
                    // last one:
                    elif i = lastIndex  then 
                        if lastIsFirst then
                            let struct(_, _, pt, N) = Pnt.findOffsetCorner(p, t, points.[1], offDists.[i-1], offDists.[0], refNormal)
                            Pts.Add pt
                            Ns.Add N
                        elif loop then 
                            let struct( _, _, pt, N) = Pnt.findOffsetCorner(p, t, n, offDists.[i-1], offDists.[i], refNormal)
                            Pts.Add pt
                            Ns.Add N
                        else 
                            let struct( sp, _, _, N) = Pnt.findOffsetCorner(p, t, n, offDists.[i-1], offDists.[i-1], refNormal) // or any next off dist since only sp is used    
                            Pts.Add (t + sp)
                            Ns.Add N 
                    else
                        let struct( _, _, pt, N ) = Pnt.findOffsetCorner(p, t, n, offDists.[i-1], offDists.[i], refNormal)
                        Pts.Add pt
                        Ns.Add N                                            
                if lenDistNorm > 0 then 
                    for i=0 to  distsNeededNorm-1 do // ns might be shorter than pts if lastIsFirst= true
                        let n = Ns.[i]
                        if n <> Vector3d.Zero then 
                            Pts.[i] <- Pts.[i] + n * normDists.[i]
                
                let rec searchBack i (ns:Rarr<Vector3d>) = 
                    let ii = saveIdx (i) ns.Count
                    let v = ns.[ii]
                    if v <> Vector3d.Zero || i < -ns.Count then ii
                    else searchBack (i-1) ns
                    
                let rec  searchForward i (ns:Rarr<Vector3d>) = 
                    let ii = saveIdx (i) ns.Count
                    let v = ns.[ii]
                    if v <> Vector3d.Zero || i > (2 * ns.Count) then ii
                    else searchForward (i + 1) ns  

                // fix colinear segments by nearest neigbours that are ok
                for i, n in Seq.indexed Ns do // ns might be shorter than pts if lastIsFirst= true
                    if n = Vector3d.Zero then                 
                        let pi = searchBack (i-1) Ns
                        let ppt = Pts.[pi]
                        let pln = Line(points.[pi], points.[saveIdx (pi + 1) pointk])                
                        let pclp = pln.ClosestPoint(ppt, limitToFiniteSegment=false)
                        let pv = ppt - pclp
                                                    
                        let ni = searchForward (i + 1) Ns
                        let npt = Pts.[ni]
                        let nln = Line(points.[ni], points.[saveIdx (ni-1) pointk])
                        let nclp = nln.ClosestPoint(npt, limitToFiniteSegment=false)
                        let nv = npt - nclp
                        //print (pi,"prev i")
                        //print (i,"is colinear")
                        //print (ni,"next i")
                        if offDists.[pi] <> offDists.[saveIdx (ni-1) distsNeeded] then 
                            RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetPoints: can't fix colinear at index %d with index %d and %d because offset distances are missmatching: %f, %f" i pi ni offDists.[pi] offDists.[saveIdx (ni-1) pointk]                                                     
                        Pts.[i] <- points.[i] + (nv + pv)*0.5                                            
                if lastIsFirst then Pts.[lastIndex] <- Pts.[0]
                Pts
        
        
        /// Offsets a Polyline in 3D space by finding th local offest in each corner.
        /// Positive distance is offset inwards, negative outwards.
        /// Normal distances define a perpendicular offset at each corner.
        /// Auto detects if given points are from a closed Polyline (first point = last point) and loops them
        /// Auto detects points from closed polylines and loops them
        [<Extension>]
        static member OffsetPoints(     points:Point3d IList, 
                                        offsetDistance: float, 
                                        [<OPT;DEF(0.0)>]normalDistance: float ,
                                        [<OPT;DEF(false)>]loop:bool) :Point3d  Rarr  = 

            if normalDistance = 0.0 then RhinoScriptSyntax.OffsetPoints(points,[offsetDistance],[]              , loop)
            else                         RhinoScriptSyntax.OffsetPoints(points,[offsetDistance],[normalDistance], loop)
                                     
        



