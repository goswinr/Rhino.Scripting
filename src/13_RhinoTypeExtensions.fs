namespace Rhino.Scripting

open System
open Rhino.Geometry
open System.Runtime.CompilerServices
open FsEx.Util
open FsEx
open FsEx.NiceString

[<AutoOpen>]
module TypeExtensionsRhino =  

    [<Extension>]  
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

    [<Extension>]     
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

    [<Extension>]      
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
