
namespace Rhino.Scripting

open System
open Rhino.Geometry
open System.Runtime.CompilerServices
open Rhino.Scripting.Util

[<AutoOpen>]
module TypeExtensions =
    
    [<Extension>]       
    type Double with  
        member x.AsNiceString = Util.floatToString x

    [<Extension>]       
    type Single with  
        member x.AsNiceString = Util.singleToString x

    [<Extension>]
    type Drawing.Color with
        
        ///Compare to another color only by Alpha, Red, Green and Blue values ignoring other fields such as IsNamedColor
        [<Extension>]
        member this.EqualsARGB(other:Drawing.Color)=
            this.A = other.A && 
            this.R = other.R && 
            this.G = other.G && 
            this.B = other.B        
        
        ///Compare two colors only by Alpha, Red, Green and Blue values ignoring other fields such as IsNamedColor
        [<Extension>]
        static member areEqualARGB (this:Drawing.Color)(other:Drawing.Color)=
            this.EqualsARGB(other)
    

    [<Extension>]       
    type Point3d with  
        ///Like the ToString function but with appropiate precision formating
        member pt.AsNiceString =
            if pt = Point3d.Unset then  
                "Point3d.Unset"
            else
                sprintf "Point3d(%s, %s, %s)" (floatToString  pt.X) (floatToString  pt.Y) (floatToString  pt.Z)


    [<Extension>]       
    type Point3f with  
        ///Like the ToString function but with appropiate precision formating
        member pt.AsNiceString =
            if pt = Point3f.Unset then  
                "Point3f.Unset"
            else
                sprintf "Point3f(%s, %s, %s)" (singleToString  pt.X) (singleToString  pt.Y) (singleToString  pt.Z)


    [<Extension>]       
    type Vector3d with  
        ///Like the ToString function but with appropiate precision formating
        member v.AsNiceString =
            if v = Vector3d.Unset then  
                "Vector3d.Unset"
            else
                sprintf "Vector3d(%s, %s, %s)" (floatToString  v.X) (floatToString  v.Y) (floatToString  v.Z)