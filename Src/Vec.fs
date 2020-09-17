namespace Rhino.Scripting

open Rhino
open Rhino.Geometry
//open FsEx.UtilMath
open FsEx.SaveIgnore 
open System

/// This module provides curried functions to manipulate Rhino Vector3d
/// It is NOT automatically opened.
module Vec =    
    
    /// Converts Angels from Degrees to Radians
    let inline internal toRadians degrees = 0.0174532925199433 * degrees // 0.0174532925199433 = Math.PI / 180. 

    /// Converts Angels from Radians to Degrees
    let inline internal toDegrees radians = 57.2957795130823 * radians // 57.2957795130823 = 180. / Math.PI

    /// Gets the X value of  Vector3d
    let inline getX (v:Vector3d)  =  v.X

    /// Gets the Y value of  Vector3d
    let inline getY (v:Vector3d) =  v.Y

    /// Gets the Z value of  Vector3d
    let inline getZ (v:Vector3d) =  v.Z

    /// Sets the X value and retuns new Vector3d
    let inline setX x (v:Vector3d) =  Vector3d(x, v.Y, v.Z)

    /// Sets the Y value and retuns new Vector3d
    let inline setY y (v:Vector3d) =  Vector3d(v.X, y, v.Z)

    /// Sets the Z value and retuns new Vector3d
    let inline setZ z (v:Vector3d) =  Vector3d(v.X, v.Y, z)

    /// Scales the vector
    let inline scale (sc:float) (v:Vector3d) = v * sc  
    
    /// Same as reverse
    let inline flip  (v:Vector3d) = -v 
    
    /// Same as flip
    let inline reverse  (v:Vector3d) = -v     

    /// Dot product
    let inline dot (a:Vector3d) (b:Vector3d) = a * b 

    /// Checks if a vector is vertical by doing:
    /// abs(v.X) + abs(v.Y) < RhinoMath.SqrtEpsilon
    /// fails on tiny (shorter than RhinoMath.SqrtEpsilon) vectors
    let inline isVertical (v:Vector3d) =
        if v.IsTiny(RhinoMath.SqrtEpsilon) then failwithf "Cannot not check very tiny vector for verticality %A" v
        abs(v.X) + abs(v.Y) < RhinoMath.SqrtEpsilon

    /// Checks if a vector is horizontal  by doing:
    /// abs(v.Z) < RhinoMath.SqrtEpsilon
    /// fails on tiny (shorter than RhinoMath.SqrtEpsilon) vectors
    let inline isHorizontal (v:Vector3d) =
        if v.IsTiny(RhinoMath.SqrtEpsilon) then failwithf "Cannot not check very tiny vector for horizontality %A" v
        abs(v.Z) < RhinoMath.SqrtEpsilon

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
    
    /// Applies a transformation matrix
    let transform (xForm:Transform) (v:Vector3d ) =
        let t = Vector3d(v)
        v.Transform(xForm)
        v


    /// Returns positive angle between 2 Vectors in Radians , takes vector orientation into account, 
    /// Range 0.0 to PI( = 0 to 180 degree)
    /// Unitizes the input vectors
    let inline angle180Rad (a:Vector3d) (b:Vector3d) = 
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
    let inline angle180Deg (a:Vector3d) (b:Vector3d) = 
        angle180Rad a b |>  toDegrees


    /// Returns positive angle between 2 Vectors in Radians, 
    /// Ignores vector orientation, 
    /// Range: 0.0 to PI/2 ( = 0 to 90 degrees)
    /// Unitizes the input vectors
    let inline angle90Rad (a:Vector3d) (b:Vector3d) =   
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
    /// Range: 0 to 90 degrees
    /// Unitizes the input vectors
    let inline angle90Deg (a:Vector3d) (b:Vector3d) =   
        angle90Rad a b |>  toDegrees
    
    /// Returns positive angle between 2 Vectors in Radians
    /// Considering positve rotation round a planes ZAxis 
    /// Range: 0.0 to 2 PI ( = 0 to 360 degrees)
    /// Unitizes the input vectors
    let inline angle360Rad (pl:Plane) (a:Vector3d) (b:Vector3d)  = 
        let r = angle180Rad a b
        if dot (cross a b) pl.ZAxis > 0.0 then  r
        else                                    Math.PI * 2. - r
    
    /// Returns positive angle between 2 Vectors in Degrees
    /// Considering positve rotation round a planes ZAxis 
    /// Range:  0 to 360 degrees
    /// Unitizes the input vectors
    let inline angle360Deg (pl:Plane) (a:Vector3d) (b:Vector3d)  = 
        let r = angle180Deg a b
        if dot (cross a b) pl.ZAxis > 0.0 then  r
        else                                    360. - r

    /// Returns positive angle between 2 Vectors in Radians
    /// Considering positve rotation round the World ZAxis 
    /// Range: 0.0 to 2 PI ( = 0 to 360 degrees)
    /// Unitizes the input vectors
    let inline angle360RadXY (a:Vector3d) (b:Vector3d)  = 
        let r = angle180Rad a b
        if (cross a b).Z > 0.0 then  r
        else                         Math.PI * 2. - r
    
    /// Returns positive angle between 2 Vectors in Degrees
    /// Considering positve rotation round the World ZAxis 
    /// Range:  0 to 360 degrees
    /// Unitizes the input vectors
    let inline angle360DegXY  (a:Vector3d) (b:Vector3d)  = 
        let r = angle180Deg a b
        if (cross a b).Z  > 0.0 then  r
        else                          360. - r        
    
    /// Returns positive or negative  slope of a vector in Radians
    /// in relation to XY Plane    
    let slopeRad (v:Vector3d) =
        let f = Vector3d(v.X, v.Y, 0.0)
        if v.Z > 0.0 then   angle90Rad v f
        else              -(angle90Rad v f)

    /// Returns positive or negative slope of a vector in Degrees
    /// in relation to XY Plane    
    let slopeDeg (v:Vector3d) =
        let f = Vector3d(v.X, v.Y, 0.0)
        if v.Z > 0.0 then   angle90Deg v f
        else              -(angle90Deg v f)

    /// Returns positive or negative slope of a vector in Percent
    /// in relation to XY Plane    
    /// 100% = 45 degrees
    let slopePercent (v:Vector3d) =
        if abs(v.Z) < RhinoMath.SqrtEpsilon then failwithf " Can't get Slope from vertical vector %A" v
        let f = Vector3d(v.X, v.Y, 0.0)
        100.0 * (v.Z/f.Length)


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
    
    /// Returns a horizontal vector that is perpendicular to the given vector.
    /// just: Vector3d(v.Y, -v.X, 0.0)
    /// Rotated counter clockwise in top view.
    /// If input vector is vertical, result is a zero length vector.
    /// Fails on vertical input vector where resulting vector would be of almost zero length (RhinoMath.SqrtEpsilon)
    let perpendicularVecInXY (v:Vector3d) =         
        let r = Vector3d(v.Y, -v.X, 0.0) // this si the same as: Vec.cross v Vector3d.ZAxis
        if r.IsTiny(RhinoMath.SqrtEpsilon) then failwithf "Cannot find perpendicularVecInXY for vertical vector %A" v
        r

    /// Returns a vector that is perpendicular to the given vector an in the same vertical plane .
    /// Projected into the XY plane input and output vectors are parallell and of same orientation.
    /// Not of same length, not unitized
    /// Fails on vertical input vector where resulting vector would be of almost zero length (RhinoMath.SqrtEpsilon)
    let perpendicularVecInVerticalPlane (v:Vector3d) =         
        let hor = Vector3d(v.Y, -v.X, 0.0)
        let r = cross v hor
        if r.IsTiny(RhinoMath.SqrtEpsilon) then failwithf "Cannot find perpendicularVecInVerticalPlane for vertical vector %A" v 
        if v.Z < 0.0 then -r else r

    /// Project vector to plane
    /// Fails if resulting vector is of almost zero length (RhinoMath.SqrtEpsilon)
    let projectToPlane (pl:Plane) (v:Vector3d) =
        let pt = pl.Origin + v
        let clpt = pl.ClosestPoint(pt)
        let r = clpt-pl.Origin
        if r.IsTiny(RhinoMath.SqrtEpsilon) then failwithf "Cannot projectToPlane for perpendicular vector %A to given plane %A" v pl
        r

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
                
    
             
                
       