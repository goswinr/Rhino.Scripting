namespace Rhino.Scripting

open Rhino
open Rhino.Geometry
open Rhino.Scripting.ActiceDocument
open FsEx.SaveIgnore 


/// Functions to manipulate Rhino Point3d 
module Pnt = //depends on Vec module
    
   
    
    let inline distance (a:Point3d) (b:Point3d) = let v = a-b in sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z)
    
    let inline distanceSq (a:Point3d) (b:Point3d) = let v = a-b in    v.X*v.X + v.Y*v.Y + v.Z*v.Z

    let inline setX (v:Point3d) x =  Point3d(x, v.Y, v.Z)

    let inline setY (v:Point3d) y =  Point3d(v.X, y, v.Z)

    let inline setZ (v:Point3d) z =  Point3d(v.X, v.Y, z)
    
    /// snap to point if within Doc.ModelAbsoluteTolerance
    let snapIfClose (snapTo:Point3d) (pt:Point3d) =
        if (snapTo-pt).Length < Doc.ModelAbsoluteTolerance then snapTo else pt
    ///Every line has a normal vector in XY Plane.
    ///If line is vertical then XAxis
    ///result is unitized
    let normalOfTwoPointsInXY(fromPt:Point3d, toPt:Point3d) =
        let v = toPt - fromPt
        Vector3d.CrossProduct(v, Vector3d.ZAxis)
        |> Vec.unitizeWithAlternative Vector3d.XAxis
        
    /// distHor in XY plane, 
    /// distNormal in free Normal ( ZAxis for Flat line)
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
            |> Vec.unitize        
    
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
        if Vec.isAngleBelow1Degree(vp, vn) then 
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

