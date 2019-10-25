namespace Rhino.Scripting

open System
open Rhino.Geometry
open System.Runtime.CompilerServices
open Rhino.Scripting.Util

[<AutoOpen>]
module TypeExtensionsRhino =    

   
      
    type Point3d with  
        
        [<Extension>]     
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = NiceString.point3dToString pt
        
        [<Extension>] 
        member pt.ToPoint3f = Point3f(float32 pt.X, float32 pt.Y, float32 pt.Z)

         
    type Point3f with  
        
        [<Extension>]  
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = NiceString.point3fToString pt
        
        [<Extension>] 
        member pt.ToPoint3d = Point3d(pt)

          
    type Vector3d with  
        
        [<Extension>] 
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  NiceString.vector3dToString v
        
        [<Extension>] 
        member v.ToVector3f = Vector3f(float32 v.X, float32 v.Y, float32 v.Z) 
    
    type Vector3f with  
        
        [<Extension>] 
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  NiceString.vector3fToString v
        
        [<Extension>] 
        member v.ToVector3d = Vector3d(v)
