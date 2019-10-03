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
        member pt.ToNiceString =
            if pt = Point3d.Unset then  
                "Point3d.Unset"
            else
                sprintf "Point3d(%s, %s, %s)" (floatToString  pt.X) (floatToString  pt.Y) (floatToString  pt.Z)


    [<Extension>]       
    type Point3f with  
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString =
            if pt = Point3f.Unset then  
                "Point3f.Unset"
            else
                sprintf "Point3f(%s, %s, %s)" (singleToString  pt.X) (singleToString  pt.Y) (singleToString  pt.Z)


    [<Extension>]       
    type Vector3d with  
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =
            if v = Vector3d.Unset then  
                "Vector3d.Unset"
            else
                sprintf "Vector3d(%s, %s, %s)" (floatToString  v.X) (floatToString  v.Y) (floatToString  v.Z)