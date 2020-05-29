namespace Rhino.Scripting

open Rhino
open Rhino.Geometry

open FsEx.SaveIgnore 

/// This module provides curried functions to manipulate Rhino Vector3d
/// It is NOT automatically opened.
module Vec =
    
    
    /// Sets the X value and retuns new Vector3d
    let inline setX (v:Vector3d) x =  Vector3d(x, v.Y, v.Z)

    /// Sets the Y value and retuns new Vector3d
    let inline setY (v:Vector3d) y =  Vector3d(v.X, y, v.Z)

    /// Sets the Z value and retuns new Vector3d
    let inline setZ (v:Vector3d) z =  Vector3d(v.X, v.Y, z)

    /// scales the vector
    let inline scale (sc:float) (v:Vector3d) = v * sc  
    
    ///Same as reverse
    let inline flip  (v:Vector3d) = -v 
    
    ///Same as flip
    let inline reverse  (v:Vector3d) = -v     
    
    /// Unitizes the vector 
    /// fails if length is les than 1e-9 units
    let inline unitize (v:Vector3d) =
        let len = sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) // see v.Unitized() type extension too
        if len > 1e-9 then v * (1./len) 
        else failwithf "Vec.unitize: %s is too small for unitizing, tol: 1e-9" v.ToNiceString
    
    ///Unitize vector, if input vector is shorter than 1e-6 alternative vector is returned UN-unitized.
    let inline unitizeWithAlternative (unitVectorAlt:Vector3d) (v:Vector3d) =
        let l = v.SquareLength
        if l < RhinoMath.ZeroTolerance  then  //sqrt RhinoMath.ZeroTolerance = 1e-06
            unitVectorAlt //|> unitize
        else  
            let f = 1.0 / sqrt(l)
            Vector3d(v.X*f , v.Y*f , v.Z*f)
        
    /// set vector to a given Length
    let inline setLength (len:float) (v:Vector3d) =
        let f = len / sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) in Vector3d(v.X*f, v.Y*f, v.Z*f) 
    
    /// reverse vector if Z part is smaller than 0.0     
    let inline orientUp (v:Vector3d) =
        if v.Z < 0.0 then -v else v
    
    /// ensure vector has a positive dot product with given orientation vector
    let inline matchOrientation (orientToMatch:Vector3d) (v:Vector3d) =
        if orientToMatch * v < 0.0 then -v else v
       

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

        // for fsi: printfn "%.18f" (cos( 0.25 * (System.Math.PI / 180.)))
        
    /// Fails on zero length vectors
    let isAngleBelowQuaterDegree(a:Vector3d, b:Vector3d) = //(prevPt:Point3d, thisPt:Point3d, nextPt:Point3d) =                        
        //let a = prevPt - thisPt 
        //let b = nextPt - thisPt                       
        let sa = a.SquareLength
        if sa < RhinoMath.SqrtEpsilon then 
            failwithf "Duplicate points: isAngleBelowQuaterDegree: prevPt - thisPt: %s.SquareLength < RhinoMath.SqrtEpsilon; nextPt - thisPt:%s " a.ToNiceString b.ToNiceString
        let sb = b.SquareLength
        if sb < RhinoMath.SqrtEpsilon then 
            failwithf "Duplicate points: isAngleBelowQuaterDegree: nextPt - thisPt: %s.SquareLength < RhinoMath.SqrtEpsilon; prevPt - thisPt:%s " b.ToNiceString a.ToNiceString
        let lena = sqrt sa
        let lenb = sqrt sb
        if lena < Doc.ModelAbsoluteTolerance then 
            failwithf "Duplicate points: isAngleBelowQuaterDegree: prevPt - thisPt: %s < Doc.ModelAbsoluteTolerance: %f; nextPt - thisPt:%s " a.ToNiceString Doc.ModelAbsoluteTolerance b.ToNiceString
        if lenb < Doc.ModelAbsoluteTolerance then 
            failwithf "Duplicate points: isAngleBelowQuaterDegree: nextPt - thisPt: %s < Doc.ModelAbsoluteTolerance: %f; prevPt - thisPt:%s " b.ToNiceString Doc.ModelAbsoluteTolerance a.ToNiceString  
        let au = a * (1.0 / lena)
        let bu = b * (1.0 / lenb)
        abs(bu*au) > 0.999990480720734 // = cosine of 0.25 degree: printfn "%.18f" (cos( 0.25 * (System.Math.PI / 180.)))
                
    
             
                
       