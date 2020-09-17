namespace Rhino.Scripting

open Rhino
open Rhino.Geometry
open FsEx.SaveIgnore 


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
    let atZlevel (fromPt:Point3d)( toPt:Point3d) (z:float) =
        let v = toPt - fromPt
        if fromPt.Z < toPt.Z && z < fromPt.Z  then failwithf "Pnt.atZlevel  cannot be reached for fromPt:%A toPt:%A z:%f" fromPt toPt z
        if fromPt.Z > toPt.Z && z > fromPt.Z  then failwithf "Pnt.atZlevel  cannot be reached for fromPt:%A toPt:%A z:%f" fromPt toPt z
        let dot = abs ( v * Vector3d.ZAxis)
        if dot < 0.0001 then  failwithf "Pnt.atZlevel  cannot be reached for fromPt:%A toPt:%A  almost at same Z level. taget Z %f" fromPt toPt z
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

    /// Applies a transformation matrix
    let transform (xForm:Transform) (pt:Point3d ) =        
        pt.Transform(xForm)
        pt

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
    /// If line is vertical then XAxis
    /// result is unitized
    let normalOfTwoPointsInXY(fromPt:Point3d, toPt:Point3d) =
        let v = toPt - fromPt
        Vector3d.CrossProduct(v, Vector3d.ZAxis)
        |> Vec.unitizeWithAlternative Vector3d.XAxis
    
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
            let lp = Line(thisPt + sp , vp)  //|>> (Doc.Objects.AddLine>>ignore)
            let ln = Line(thisPt + sn , vn)  //|>> (Doc.Objects.AddLine>> ignore)               
            let ok, tp , tn = Intersect.Intersection.LineLine(lp, ln) //could also be solved with trigonometry functions            
            if not ok then failwithf "findOffsetCorner: Intersect.Intersection.LineLine failed on %s and %s" lp.ToNiceString ln.ToNiceString
            sp, sn, lp.PointAt(tp), n  //or ln.PointAt(tn), should be same

