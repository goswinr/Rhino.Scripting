namespace Rhino.Scripting

open System
open Rhino.Geometry
open System.Runtime.CompilerServices
open Rhino.Scripting.Util

[<AutoOpen>]
module TypeExtensionsRhino =    

   
    [<Extension>]       
    type Point3d with  
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = NiceString.point3dToString pt

    [<Extension>]       
    type Point3f with  
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = NiceString.point3fToString pt


    [<Extension>]       
    type Vector3d with  
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  NiceString.vector3dToString v

