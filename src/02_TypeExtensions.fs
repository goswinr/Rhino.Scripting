
namespace Rhino.Scripting

open System
open System.Runtime.CompilerServices
open Rhino.Scripting.Util

[<AutoOpen>]
module TypeExtensions =
    
    [<Extension>] 
    type Collections.Generic.Dictionary<'K,'V> with
        member d.SetValue k v =
            d.[k] <-v
        
        member d.GetValue k  =
            d.[k]
        
        /// get a value and remove it from Dictionary, like *.pop() in Python 
        member d.PopValue k  =
            let v= d.[k]
            d.Remove(k)|> ignore
            v


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
        static member AreEqualARGB (this:Drawing.Color)(other:Drawing.Color)=
            this.EqualsARGB(other)
    