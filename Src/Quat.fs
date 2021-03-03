namespace Rhino.Scripting

open Rhino
open Rhino.Geometry
open FsEx.SaveIgnore 
open FsEx
open System



// TODO verify results



//that is always normalized

/// a Quaternion 
type [<Struct>] Quat = 
    // http://www.codeproject.com/Articles/36868/Quaternion-Mathematics-and-3D-Library-with-C-and-G
    // http://physicsforgames.blogspot.co.at/2010/02/quaternions.html
    // http://www.ogre3d.org/tikiwiki/Quaternion+and+Rotation+Primer    
    // http://referencesource.microsoft.com/#PresentationCore/src/Core/CSharp/System/Windows/Media3D/Quaternion.cs#d7fe24535ba22e11

    // this implementation guarantees the Quat to be always normalized  !!!
    // TODO can a normalized quat be scales and stay normalized 

    val X:float
    val Y:float
    val Z:float  
    val W:float

    /// makes sure input is unitized
    private new (x, y, z, w) = // assume  unitized ??   // make private ?? 
        if abs(x*x+y*y+z*z+w*w-1.0) > 1e-9 then RhinoScriptingException.Raise "Rhino.Scripting.Quat constructors are not unitized: %g %g %g %g" x y z w // skip check ??
        {X = x; Y = y; Z = z; W = w} // assume  unitized ??    
    
    member q.MagnitudeSq = q.X*q.X + q.Y*q.Y + q.Z*q.Z + q.W*q.W

    member q.Magnitude = sqrt q.MagnitudeSq

    member q.ScaleBy(k)  =  Quat(k * q.X, k * q.Y, k * q.Z, k * q.W) // TODO can a normalized quat be scales and stay normalized 

    member q.Norm = let l = q.Magnitude in if l > 1e-8 then q.ScaleBy (1./l) else RhinoScriptingException.Raise "Quad failed to normalize %A" q
    
    /// returns Angle in Degree
    member q.AngleDeg  = 
        // NOT !!|> Math.radToDeg // Atan2 is better than "2. * acos q.W |> Math.radToDeg" . More precise and more efficient.
        Math.Atan2 (sqrt(q.X*q.X + q.Y*q.Y + q.Z*q.Z) , q.W) * (360.0 / Math.PI) 
        

    static member scaleBy  k (q:Quat)  =  q.ScaleBy k

    static member divideBy k (q:Quat)  =  if abs k > 1e-8 then q.ScaleBy (1./k) else RhinoScriptingException.Raise "Rhino.Scripting.Quat: Cannot devide Quat %A by %f" q k    

    static member magnitudeSq (q:Quat) = q.MagnitudeSq

    static member magnitude   (q:Quat) = q.Magnitude 

    static member norm (q:Quat) = q.Norm
    
    /// returns Angle in Degree    
    static member angelDeg (q:Quat) = q.AngleDeg 
    
    static member conjugate (q:Quat) = Quat (-q.X, -q.Y, -q.Z, q.W) //|> Quat.failIfNotNorm 

    static member isNormalized (q:Quat) = abs (1. - q.MagnitudeSq) < 1e-8 

    //static member failIfNotNorm (q:Quat) = if q |> Quat.isNormalized |> not then RhinoScriptingException.Raise "RhinoScriptSyntax.%A is not Unitized" q else q
         
    static member fromXYZW (x, y, z, w) = 
        let l = sqrt ( x*x + y*y + z*z + w*w )
        if l < 1e-9 then RhinoScriptingException.Raise "Rhino.Scripting.Quat.fromXYZW failed on : x:%g y:%g z:%g w:%g" x y z w
        let lq = 1. / l
        Quat(x*lq, y*lq, z*lq, w*lq)
        
    static member fromVectorAndW (v:Vector3d) w = //TODO add docstring
        Quat.fromXYZW (v.X, v.Y, v.Z, w)    
    
    static member fromVector   (v:Vector3d)   = 
        Quat.fromXYZW (v.X, v.Y, v.Z, 0.0)  

    static member fromPoint (v:Point3d)   = 
         Quat.fromXYZW (v.X, v.Y, v.Z, 0.0)  
    
    //static member inline ( * ) (q:Quat,k:float) =  Quat(k * q.X, k * q.Y, k * q.Z, k * q.W)
    //static member inline ( / ) (q:Quat,f:float) = if abs f < 1e-9 then RhinoScriptingException.Raise "RhinoScriptSyntax.Cannot devide Quat %A by %f" q f  else q * (1./f)
    //static member inline ( + ) (l:Quat,r:Quat)  = Quat( l.X + r.X, l.Y + r.Y, l.Z + r.Z, l.W + r.W )
    //static member inline ( - ) (l:Quat,r:Quat)  = Quat( l.X - r.X, l.Y - r.Y, l.Z - r.Z, l.W - r.W )
    //static member identity = Quat(0.,0.,0.,1.)
    //static member isIdentity (q:Quat) = (q.X = 0. && q.Y = 0. && q.Z = 0. && q.W = 1.)
    //static member invert (q:Quat) = q |> Quat.conjugate |> Quat.divideBy q.MagnitudeSq |> Quat.failIfNotNorm // Inverse = Conjugate / Norm Squared
    
    static member ( * ) (l:Quat,r:Quat)  = Quat(   l.W * r.X + l.X * r.W + l.Y * r.Z - l.Z * r.Y,
                                                    l.W * r.Y + l.Y * r.W + l.Z * r.X - l.X * r.Z,
                                                    l.W * r.Z + l.Z * r.W + l.X * r.Y - l.Y * r.X,
                                                    l.W * r.W - l.X * r.X - l.Y * r.Y - l.Z * r.Z ) 

    /// Given a Rotation Axis(Vec) and an Angle in Radian returns a Quaternion
    static member fromVecRad (axis:Vector3d, angleRadian) =
        let mutable li = axis.Length
        if li < 1e-8 then RhinoScriptingException.Raise "Rhino.Scripting.Quat.fromVecRad: Cannot create Quat to rotate %g radians around zero length vector %A" angleRadian axis //or return identity ?
        let sa = sin (angleRadian * 0.5)
        li <- 1. / li // inverse
        //printfn "Build with ang %g " (Math.radToDeg angleRadian)
        //printfn "Build with w %g "   (cos (angleRadian * 0.5))
        Quat ( axis.X * li * sa, //unitizing vector
               axis.Y * li * sa,
               axis.Z * li * sa,               
               cos (angleRadian * 0.5))

    /// Given a Rotation Axis(Vec) and an Angle in Deghree returns a Quaternion
    static member fromVecDeg (axis:Vector3d, angleDeg) = 
        Quat.fromVecRad (axis, angleDeg |> UtilMath.toRadians)
    
    /// finding-quaternion-representing-the-rotation-from-one-vector-to-another
    static member fromVecToVec (u:Vector3d,v:Vector3d) =
        // http://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another/1171995#1171995
        let n = Vec.cross u  v
        if n.SquareLength < 1e-12 then // vectors are paralell
            if u.IsTiny 1e-8  then RhinoScriptingException.Raise "Rhino.Scripting.Quat.fromVecToVec u: %A (+ %A) to short in Quat.fromVecToVec" u v
            if v.IsTiny 1e-8  then RhinoScriptingException.Raise "Rhino.Scripting.Quat.fromVecToVec v: %A (+ %A) to short in Quat.fromVecToVec" v u
            if u*v > 0. then
                Quat(0., 0., 0., 1.)   // Quat.fromVecAndW nn (cos ((Math.degToRad 0.) * 0.5))   
            else
                let nn = Vec.cross u  (if abs u.Z >= abs u.X && abs u.Z >= abs u.Y then Vector3d.XAxis else Vector3d.ZAxis) // use any axis to rotate around
                Quat.fromVectorAndW nn 0. // (cos ((Math.degToRad 180.) * 0.5))                        
        else
           Quat.fromVectorAndW n <| sqrt (u.SquareLength * v.SquareLength) + u * v 

    
    /// <summary>
    /// The quaternion expresses a relationship between two coordinate frames, A and B say. This relationship, if
    /// expressed using Euler angles, is as follows:
    /// 1) Rotate frame A about its z axis by angle gamma;
    /// 2) Rotate the resulting frame about its (new) y axis by angle beta;
    /// 3) Rotate the resulting frame about its (new) x axis by angle alpha, to arrive at frame B.
    /// .</summary>
    /// <returns>The EulerAngles in degrees: Alpha, Beta , Gamma.</returns>
    static member toEulerAngles(q:Quat)=
        // from https://github.com/mathnet/mathnet-spatial/blob/8f08be97b4b6d2ff676ee51dd91f88f7818bad3a/src/Spatial/Euclidean/Quaternion.cs#L499
        UtilMath.toDegrees (Math.Atan2(2.0 * ((q.W * q.X) + (q.Y * q.Z)), (q.W * q.W) + (q.Z * q.Z) - (q.X * q.X) - (q.Y * q.Y))),
        UtilMath.toDegrees (Math.Asin(2.0 *  ((q.W * q.Y) - (q.X * q.Z)))),
        UtilMath.toDegrees (Math.Atan2(2.0 * ((q.W * q.Z) + (q.X * q.Y)), (q.W * q.W) + (q.X * q.X) - (q.Y * q.Y) - (q.Z * q.Z)))

    
    // returns a rotated Point3d (about 0,0,0)
    static member rotatePoint (v:Point3d) (q:Quat) =        
        let  qNode = q * Quat.fromPoint v * Quat.conjugate q // kann man 0 oder W aus der multiplikation rauskürzen?
        Point3d ( qNode.X , qNode.Y , qNode.Z)


    override q.ToString() = 
        sprintf "Quat(x=%g; y=%g; z=%g; w=%g):(magnitude = %g; degrees = %g)" q.X q.Y q.Z q.W q.Magnitude q.AngleDeg
    
