namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open FsEx.SaveIgnore 
open FsEx
open Rhino.Geometry
open Rhino.Geometry
open System.Resources


/// This module provides curried functions to manipulate Rhino Point3d
/// It is NOT automatically opened.
module Pnt = 
    //depends on Vec module   
    
    /// Returns the distance between two points
    let inline distance (a:Point3d) (b:Point3d) = let v = a-b in sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z)
    
    /// Returns the squared distance bewteen two points. 
    /// This operation is slighty faster than the distance function, and sufficient for many algorithms like finding closest points.
    let inline distanceSq (a:Point3d) (b:Point3d) = let v = a-b in    v.X*v.X + v.Y*v.Y + v.Z*v.Z

    
    /// retuns a point that is at a given distance from a point in the direction of another point. 
    let inline distPt (fromPt:Point3d) ( dirPt:Point3d) ( distance:float) : Point3d  =
        let v = dirPt - fromPt
        let sc = distance/v.Length
        fromPt + v*sc
    
    
    /// Retuns a Point by evaluation a line between two point with a normalized patrameter.
    /// e.g. rel=0.5 will return the middle point, rel=1.0 the endPoint
    let inline divPt(fromPt:Point3d)( toPt:Point3d)(rel:float) : Point3d  =
        let v = toPt - fromPt
        fromPt + v*rel
    
    /// retuns a point that is at a given Z level, 
    /// going from a point in the direction of another point. 
    let extendToZLevel (fromPt:Point3d)( toPt:Point3d) (z:float) =
        let v = toPt - fromPt
        if fromPt.Z < toPt.Z && z < fromPt.Z  then RhinoScriptingException.Raise "Pnt.extendToZLevel  cannot be reached for fromPt:%A toPt:%A z:%f" fromPt toPt z
        if fromPt.Z > toPt.Z && z > fromPt.Z  then RhinoScriptingException.Raise "Pnt.extendToZLevel  cannot be reached for fromPt:%A toPt:%A z:%f" fromPt toPt z
        let dot = abs ( v * Vector3d.ZAxis)
        if dot < 0.0001 then  RhinoScriptingException.Raise "Pnt.extendToZLevel  cannot be reached for fromPt:%A toPt:%A  almost at same Z level. taget Z %f" fromPt toPt z
        let diffZ = abs (fromPt.Z - z)
        let fac = diffZ / dot 
        fromPt + v * fac
    

    /// Sets the X value and retuns new Point3d
    let inline setX x (pt:Point3d) =  Point3d(x, pt.Y, pt.Z)

    /// Sets the Y value and retuns new Point3d
    let inline setY y (pt:Point3d) =  Point3d(pt.X, y, pt.Z)

    /// Sets the Z value and retuns new Point3d
    let inline setZ z (pt:Point3d) =  Point3d(pt.X, pt.Y, z)

    /// Gets the X value of  Point3d
    let inline getX (pt:Point3d)  =  pt.X

    /// Gets the Y value of  Point3d
    let inline getY (pt:Point3d) =  pt.Y

    /// Gets the Z value of  Point3d
    let inline getZ (pt:Point3d) =  pt.Z

    /// Applies a transformation matrix, retun new point
    let transform (xForm:Transform) (pt:Point3d ) =        
        let p = Point3d(pt) //copy first !
        p.Transform(xForm)
        p

    /// Applies a translation vector
    let inline translate (shift:Vector3d) (pt:Point3d ) =
        pt + shift
    
    /// Add to X coordinat of point
    let inline translateX (xShift:float) (pt:Point3d ) =
        Point3d(pt.X+xShift, pt.Y, pt.Z)

    /// Add to Y coordinat of point
    let inline translateY (yShift:float) (pt:Point3d ) =
        Point3d(pt.X, pt.Y+yShift, pt.Z)

    /// Add to Z coordinat of point
    let inline translateZ (zShift:float) (pt:Point3d ) =
        Point3d(pt.X, pt.Y, pt.Z+zShift)
   
    /// Snap to point if within Doc.ModelAbsoluteTolerance
    let snapIfClose (snapTo:Point3d) (pt:Point3d) =
        if (snapTo-pt).Length < Doc.ModelAbsoluteTolerance then snapTo else pt
    
    /// Every line has a normal vector in XY Plane.
    /// If line is vertical then XAxis is returned
    /// Rotated counter clockwise in top view.  
    /// result is unitized
    /// see also : Vec.perpendicularVecInXY
    let normalOfTwoPointsInXY(fromPt:Point3d, toPt:Point3d) =        
        let x = toPt.Y - fromPt.Y
        let y = fromPt.X - toPt.X  // this is the same as: Vec.cross v Vector3d.ZAxis
        let len = sqrt(x*x + y*y)
        if len < RhinoMath.SqrtEpsilon then Vector3d.XAxis
        else Vector3d(x/len, y/len, 0.0)
    
    /// Offsets two points by two given distances.
    /// The fist distance (distHor) is applied in in XY Plane 
    /// The second distance (distNormal) is applied perpendicular to the line (made by the two points) and perpendicular to the horizontal offset direction.
    /// this is in Wolrd.Z direction if both points are at the same Z level. 
    /// If points are closer than than 1e-6 units the World.XAxis is used as first direction and World.ZAxis as second direction.
    let offsetTwoPt(    fromPt:Point3d, 
                        toPt:Point3d, 
                        distHor:float,
                        distNormal:float) : Point3d*Point3d= 
        let v = toPt - fromPt
        let normHor =
            Vector3d.CrossProduct(v, Vector3d.ZAxis)
            |> Vec.unitizeWithAlternative Vector3d.XAxis
            
        let normFree = 
            Vector3d.CrossProduct(v, normHor)
            |> Vec.unitizeWithAlternative Vector3d.ZAxis      
    
        let shift = distHor * normHor + distNormal * normFree
        fromPt +  shift, toPt + shift 
    
    

    /// Finds the inner offset point in a corner ( difind by a polyline from 3 points ( prevPt, thisPt and nextPt)
    /// The offset from first and second segment are given speratly and can vary (prevDist and nextDist).    
    /// Use negative distance for outer offset
    /// The orientation parameter is only aproximate, it might flip the output normal, so that the  dot-product is positive.
    /// Returns :
    ///   • the first segment offset vector in actual length  , 
    ///   • second segment offset vector, 
    ///   • the offsseted corner,
    ///   • and the unitized normal at the corner. fliped if needed to match orientation of the orintation input vector (positive dot product)
    /// If Points are  colinear returns: Vector3d.Zero, Vector3d.Zero, Point3d.Origin, Vector3d.Zero
    let findOffsetCorner(   prevPt:Point3d,
                            thisPt:Point3d, 
                            nextPt:Point3d, 
                            prevDist:float, 
                            nextDist:float,
                            orientation:Vector3d) : Vector3d* Vector3d * Point3d * Vector3d=
        let vp = prevPt - thisPt
        let vn = nextPt - thisPt    
        if Vec.isAngleBelowQuaterDegree(vp, vn) then // TODO refine erroe criteria
            Vector3d.Zero, Vector3d.Zero, Point3d.Origin, Vector3d.Zero
        else 
            let n = 
                Vector3d.CrossProduct(vp, vn)
                |> Vec.unitize
                |> Vec.matchOrientation orientation            
            
            let sp = Vector3d.CrossProduct(vp, n) |> Vec.setLength prevDist 
            let sn = Vector3d.CrossProduct(n, vn) |> Vec.setLength nextDist    
            let lp = Line(thisPt + sp , vp)  //|>! (Doc.Objects.AddLine>>ignore)
            let ln = Line(thisPt + sn , vn)  //|>! (Doc.Objects.AddLine>> ignore)               
            let ok, tp , tn = Intersect.Intersection.LineLine(lp, ln) //could also be solved with trigonometry functions            
            if not ok then RhinoScriptingException.Raise "Rhino.Scripting.Pnt.findOffsetCorner: Intersect.Intersection.LineLine failed on %s and %s" lp.ToNiceString ln.ToNiceString
            sp, sn, lp.PointAt(tp), n  //or ln.PointAt(tn), should be same

    /// returns the closest point form a Point list 
    let closestPoint (pt:Point3d) (pts:Rarr<Point3d>) : Point3d=
        if pts.Count = 0 then RhinoScriptingException.Raise "Pnt.closestPoint empty List of Points: pts"
        let mutable mip = Point3d.Unset
        let mutable mid = Double.MaxValue
        for i=0 to pts.LastIndex do
            let p = pts.[i]
            let d = distanceSq p pt
            if d < mid then 
                mid <- d
                mip <- p
        mip 

    /// returns the indices of the points that are closest to each other 
    let closestPointsIdx (xs:Rarr<Point3d>) (ys:Rarr<Point3d>) =
        if xs.Count = 0 then RhinoScriptingException.Raise "Pnt.closestPointsIdx empty List of Points: xs"
        if ys.Count = 0 then RhinoScriptingException.Raise "Pnt.closestPointsIdx empty List of Points: ys"
        let mutable xi = -1
        let mutable yj = -1
        let mutable mid = Double.MaxValue
        for i=0 to xs.LastIndex do
            let pt = xs.[i] 
            for j=0 to ys.LastIndex do
                let d = distanceSq pt ys.[j]
                if d < mid then 
                    mid <- d
                    xi <- i
                    yj <- j
        xi,yj    
    
    /// returns the smallest Distance between Point Sets
    let minDistBetweenPointSets (xs:Rarr<Point3d>) (ys:Rarr<Point3d>) =
        if xs.Count = 0 then RhinoScriptingException.Raise "Pnt.minDistBetweenPointSets empty List of Points: xs"
        if ys.Count = 0 then RhinoScriptingException.Raise "Pnt.minDistBetweenPointSets empty List of Points: ys"        
        let (i,j) = closestPointsIdx xs ys
        distance xs.[i]  ys.[j]
    
    /// find the indix of the point that has the biggest distance to any point from another set
    /// returns findPointFromIdx * checkAgainstIdx
    let mostDistantPointIdx (findPointFrom:Rarr<Point3d>) (checkAgainst:Rarr<Point3d>) : int*int=        
        if findPointFrom.Count = 0 then RhinoScriptingException.Raise "Pnt.mostDistantPoint empty List of Points: findPointFrom"
        if checkAgainst.Count = 0 then RhinoScriptingException.Raise "Pnt.mostDistantPoint empty List of Points: checkAgainst"
        let mutable maxd = Double.MinValue
        let mutable findPointFromIdx = -1
        let mutable checkAgainstTempIdx = -1
        let mutable checkAgainstIdx = -1
        for i=0 to findPointFrom.LastIndex do
            let pt = findPointFrom.[i] 
            let mutable mind = Double.MaxValue
            for j=0 to checkAgainst.LastIndex do
                let d = distanceSq pt checkAgainst.[j]
                if d < mind then 
                    mind <- d
                    checkAgainstTempIdx <-j
            if mind > maxd then
                maxd <- mind
                findPointFromIdx <-i
                checkAgainstIdx <-checkAgainstTempIdx
        findPointFromIdx, checkAgainstIdx

    /// find the point that has the biggest distance to any point from another set
    let mostDistantPoint (findPointFrom:Rarr<Point3d>) (checkAgainst:Rarr<Point3d>) =        
        let i,_ = mostDistantPointIdx findPointFrom checkAgainst
        findPointFrom.[i]
       


    /// Culls points if they are to close to previous or next item
    let cullDuplicatePointsInSeq (tolerance) (pts:Rarr<Point3d>)  = 
        if pts.Count = 0 then RhinoScriptingException.Raise "Pnt.cullDuplicatePointsInSeq empty List of Points: pts"
        if pts.Count = 1 then 
            pts
        else
            let tsq = tolerance*tolerance
            let res  =  Rarr(pts.Count)
            let mutable last  = pts.[0]
            res.Add last
            for i = 1  to pts.LastIndex do                
                let pt = pts.[i]
                if distanceSq last pt > tsq then 
                    last <- pt
                    res.Add last
            res

    /// similar to Join Polylines this tries to find continous sequences of points
    /// tolGap is the maximun allowable gap between the start and the endpoint of to segments
    /// search starts from the segment with the most points
    /// both start and ened point of each point list is checked for adjacency
    let findContinousPoints (tolGap:float)  (ptss: Rarr<Rarr<Point3d>>)  = 
        let i =  ptss |> Rarr.maxIndBy Rarr.length        
        let res = ptss.Pop(i)
        let mutable loop = true
        while loop && ptss.Count > 0 do
            //first try to append to end
            let ende = res.Last
            let si = ptss |> Rarr.minIndBy ( fun ps -> distanceSq ende ps.First) 
            let ei = ptss |> Rarr.minIndBy ( fun ps -> distanceSq ende ps.Last) 
            let sd = distance ende ptss.[si].First
            let ed = distance ende ptss.[ei].Last
            if   sd < tolGap && sd < ed then  res.AddRange(         ptss.Pop(si)) 
            elif ed < tolGap && ed < sd then  res.AddRange(Rarr.rev(ptss.Pop(ei)))
            else
                //search from start
                let start = res.First
                let si = ptss |> Rarr.minIndBy ( fun ps -> distanceSq start ps.First) 
                let ei = ptss |> Rarr.minIndBy ( fun ps -> distanceSq start ps.Last) 
                let sd = distance start ptss.[si].First
                let ed = distance start ptss.[ei].Last
                if   sd < tolGap && sd < ed then res.InsertRange(0, Rarr.rev(ptss.Pop(si))) 
                elif ed < tolGap && ed < sd then res.InsertRange(0,          ptss.Pop(ei))
                else
                    loop <- false
        res