namespace Rhino.Scripting

open Rhino
open Rhino.Geometry
open FsEx.UtilMath
open FsEx.SaveIgnore 
open System

/// This module provides curried functions to manipulate Rhino Vector3d
/// It is NOT automatically opened.
module Vec =    
    
    /// Sets the X value and retuns new Vector3d
    let inline setX (v:Vector3d) x =  Vector3d(x, v.Y, v.Z)

    /// Sets the Y value and retuns new Vector3d
    let inline setY (v:Vector3d) y =  Vector3d(v.X, y, v.Z)

    /// Sets the Z value and retuns new Vector3d
    let inline setZ (v:Vector3d) z =  Vector3d(v.X, v.Y, z)

    /// Scales the vector
    let inline scale (sc:float) (v:Vector3d) = v * sc  
    
    /// Same as reverse
    let inline flip  (v:Vector3d) = -v 
    
    /// Same as flip
    let inline reverse  (v:Vector3d) = -v     

    /// Dot product
    let inline dot (a:Vector3d) (b:Vector3d) = a * b 

    /// Cross product 
    /// A x B = |A|*|B|*sin(angle), direction follow right-hand rule
    let inline cross (a:Vector3d) (b:Vector3d) =
        Vector3d (  a.Y * b.Z - a.Z * b.Y ,  
                    a.Z * b.X - a.X * b.Z ,  
                    a.X * b.Y - a.Y * b.X ) 
    
    /// Unitizes the vector 
    /// fails if length is les than 1e-9 units
    let inline unitize (v:Vector3d) =
        let len = sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) // see v.Unitized() type extension too
        if len > 1e-9 then v * (1./len) 
        else failwithf "Vec.unitize: %s is too small for unitizing, tol: 1e-9" v.ToNiceString
    
    /// Unitize vector, if input vector is shorter than 1e-6 alternative vector is returned UN-unitized.
    let inline unitizeWithAlternative (unitVectorAlt:Vector3d) (v:Vector3d) =
        let l = v.SquareLength
        if l < RhinoMath.ZeroTolerance  then  //sqrt RhinoMath.ZeroTolerance = 1e-06
            unitVectorAlt //|> unitize
        else  
            let f = 1.0 / sqrt(l)
            Vector3d(v.X*f , v.Y*f , v.Z*f)

    /// Returns positive angle between 2 Vectors in Radians , takes vector orientation into account, 
    /// Range 0.0 to PI( = 0 to 180 degree)
    /// Unitizes the input vectors
    let inline angleRad (a:Vector3d) (b:Vector3d) = 
        // The "straight forward" method of acos(u.v) has large precision
        // issues when the dot product is near +/-1.  This is due to the
        // steep slope of the acos function as we approach +/- 1.  Slight
        // precision errors in the dot product calculation cause large
        // variation in the output value.
        // To avoid this we use an alternative method which finds the
        // angle bisector by (u-v)/2    
        // Because u and v and unit vectors, (u-v)/2 forms a right angle
        // with the angle bisector.  The hypotenuse is 1, therefore
        // 2*asin(|u-v|/2) gives us the angle between u and v.       
        // The largest possible value of |u-v| occurs with perpendicular
        // vectors and is sqrt(2)/2 which is well away from extreme slope
        // at +/-1. (See Windows OS Bug #1706299 for details) (form WPF refrence scource code)
        let a0 = unitize a
        let b0 = unitize b
        let dot = a0 * b0
        if -0.98 < dot && dot < 0.98 then acos dot // TODO test and compare this to real Acossafe function !!! also to find thershold for switching !!  0.98 ??               
        else
            if dot < 0. then System.Math.PI - 2.0 * asin((-a0 - b0).Length * 0.5)
            else                              2.0 * asin(( a0 - b0).Length * 0.5)    
            
    /// Returns positive angle between 2 Vectors in Degrees , takes vector orientation into account, 
    /// Range 0 to 180 degrees
    /// Unitizes the input vectors
    let inline angleDeg (a:Vector3d) (b:Vector3d) = 
        angleRad a b |>  toDegrees


    /// Returns positive angle between 2 Vectors in Radians, 
    /// Ignores vector orientation, 
    /// Range 0.0 to PI/2 ( = 0 to 90 degrees)
    /// Unitizes the input vectors
    let inline angleRadIgnoreOrient (a:Vector3d) (b:Vector3d) =   
        let a0 = unitize a
        let b0 = unitize b
        let dot =  a0 * b0
        let dotAbs = abs dot
        if dotAbs < 0.98 then acos dotAbs 
        else
            if dot < 0. then 2.0 * asin((-a0 - b0).Length * 0.5)
            else             2.0 * asin(( a0 - b0).Length * 0.5)
    
    /// Returns positive angle between 2 Vectors in Degrees, 
    /// Ignores vector orientation, 
    /// Range 0 to 90 degrees
    /// Unitizes the input vectors
    let inline angleDegIgnoreOrient (a:Vector3d) (b:Vector3d) =   
        angleRadIgnoreOrient a b |>  toDegrees

    /// Set vector to a given Length
    /// Fails on tiny vectors (v.SquareLength < RhinoMath.SqrtEpsilon)
    let inline setLength (len:float) (v:Vector3d) =
        let l  = v.SquareLength
        if l < RhinoMath.SqrtEpsilon then failwithf "Cant set length of tiny vector %A" v
        let f = len / sqrt(l) in Vector3d(v.X*f, v.Y*f, v.Z*f) 
    
    /// Reverse vector if Z part is smaller than 0.0     
    let inline orientUp (v:Vector3d) =
        if v.Z < 0.0 then -v else v
    
    /// Reverse vector if Z part is bigger than 0.0     
    let inline orientDown (v:Vector3d) =
        if v.Z < 0.0 then v else -v
    
    /// Ensure vector has a positive dot product with given orientation vector
    let inline matchOrientation (orientToMatch:Vector3d) (v:Vector3d) =
        if orientToMatch * v < 0.0 then -v else v
    
    /// Returns a horizontal perpendicular vector : Vector3d(v.Y, -v.X, 0.0)
    /// Not of same length, not unitized, rotated from X to Y axis.
    /// If input vector is vertical, result is Zero length vector
    let inline perpendicularVecInXY (v:Vector3d) = 
        Vector3d(v.Y, -v.X, 0.0)

    /// Project vector to plane
    let projectToPlane (pl:Plane) (v:Vector3d) =
        let pt = pl.Origin + v
        let clpt = pl.ClosestPoint(pt)
        clpt-pl.Origin
    
    /// Project point onto line in directin of v
    /// Fails if line is missed , tolerance 1E-6
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
    
    /// Checks if Angle between two vectors is Below one Degree
    /// Ignores vector orientation 
    /// Fails on zero length vectors, tolerance 0.00012
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
        
    /// Checks if Angle between two vectors is Below 0.25 Degrees
    /// Ignores vector orientation 
    /// Fails on zero length vectors, tolerance 0.00012
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
                
    
             
                
       