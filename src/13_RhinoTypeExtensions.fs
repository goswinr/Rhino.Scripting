namespace Rhino.Scripting

open System
open Rhino.Geometry
open System.Runtime.CompilerServices
open FsEx
open FsEx.NiceString



[<assembly:Extension>] do() //http://www.latkin.org/blog/2014/04/30/f-extension-methods-in-roslyn/

[<AutoOpen>]
module TypeExtensionsRhino =  

    //[<Extension>] //Error 3246
    type Point3d with  
        
        [<Extension>]     
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = 
            if pt = Point3d.Unset then  
                "Point3d.Unset"
            else
                sprintf "Point3d(%s, %s, %s)" (floatToString  pt.X) (floatToString  pt.Y) (floatToString  pt.Z)
        
        [<Extension>] 
        member pt.ToPoint3f = Point3f(float32 pt.X, float32 pt.Y, float32 pt.Z)

        

    //[<Extension>] //Error 3246     
    type Point3f with  
        
        [<Extension>]  
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = 
            if pt = Point3f.Unset then  
                "Point3f.Unset"
            else
                sprintf "Point3f(%s, %s, %s)" (singleToString  pt.X) (singleToString  pt.Y) (singleToString  pt.Z)
        
        [<Extension>] 
        member pt.ToPoint3d = Point3d(pt)

    //[<Extension>] //Error 3246     
    type Vector3d with  
        
        [<Extension>] 
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  
            if v = Vector3d.Unset then  
                "Vector3d.Unset"
            else
                sprintf "Vector3d(%s, %s, %s)" (floatToString  v.X) (floatToString  v.Y) (floatToString  v.Z)
        
        [<Extension>] 
        member v.ToVector3f = Vector3f(float32 v.X, float32 v.Y, float32 v.Z) 
        
        [<Extension>] 
        ///Unitizes the vector , checks input length
        member v.Unitized = let l = sqrt(v.X*v.X+v.Y*v.Y+v.Z*v.Z) in (if l > 1e-9 then v * (1./l) else failwithf "v.Unitized: %s is too small for unitizing, tol: 1e-9" v.ToNiceString)
        
        [<Extension>] 
        ///Unitizes the vector , fails if input is of zero length
        member inline v.UnitizedUnchecked = let f = 1. / sqrt(v.X*v.X+v.Y*v.Y+v.Z*v.Z) in Vector3d(v.X*f, v.Y*f, v.Z*f)

    //[<Extension>] //Error 3246
    type Vector3f with  
        
        [<Extension>] 
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  
            if v = Vector3f.Unset then  
                "Vector3f.Unset"
            else
                sprintf "Vector3f(%s, %s, %s)" (singleToString  v.X) (singleToString  v.Y) (singleToString  v.Z)
   
        
        [<Extension>] 
        member v.ToVector3d = Vector3d(v)

    //[<Extension>] //Error 3246  
    type Line with  
        
        [<Extension>]     
        ///Like the ToString function but with appropiate precision formating
        member ln.ToNiceString = 
                sprintf "Geometry.Line from %s, %s, %s to %s, %s, %s" (floatToString  ln.From.X) (floatToString  ln.From.Y) (floatToString  ln.From.Z) (floatToString  ln.To.X) (floatToString  ln.To.Y) (floatToString  ln.To.Z)
    
    
    let internal formatRhinoObject (o:obj)  = 
        match o with
        | :? Point3d    as x -> Some x.ToNiceString
        | :? Vector3d   as x -> Some x.ToNiceString
        | :? Line       as x -> Some x.ToNiceString        
        | :? Point3f    as x -> Some x.ToNiceString
        | :? Vector3f   as x -> Some x.ToNiceString
        | _ -> None
             