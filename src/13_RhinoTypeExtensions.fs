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

         
    type Point3f with  
        [<Extension>]  
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = NiceString.point3fToString pt

          
    type Vector3d with  
        [<Extension>] 
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  NiceString.vector3dToString v

