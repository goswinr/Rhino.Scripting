namespace Rhino.Scripting

open System
open Rhino.Scripting.Util

[<AutoOpen>]
module TypeExtensions =    
    

    [<EXT>] 
    type Collections.Generic.Dictionary<'K,'V> with
        [<EXT>]
        member d.SetValue k v =
            d.[k] <-v
        
        [<EXT>] 
        member d.GetValue k  =
            d.[k]
        
        /// get a value and remove it from Dictionary, like *.pop() in Python 
        [<EXT>] 
        member d.PopValue k  =
            let v= d.[k]
            d.Remove(k)|> ignore
            v


    [<EXT>]       
    type Double with  
        [<EXT>] member x.AsNiceString = Util.floatToString x

    [<EXT>]       
    type Single with  
        [<EXT>] member x.AsNiceString = Util.singleToString x

    [<EXT>]
    type Drawing.Color with
        
        ///Compare to another color only by Alpha, Red, Green and Blue values ignoring other fields such as IsNamedColor        
        [<EXT>] 
        member this.EqualsARGB(other:Drawing.Color)=
            this.A = other.A && 
            this.R = other.R && 
            this.G = other.G && 
            this.B = other.B        
        
        ///Compare two colors only by Alpha, Red, Green and Blue values ignoring other fields such as IsNamedColor
        [<EXT>]
        static member AreEqualARGB (this:Drawing.Color)(other:Drawing.Color)=
            this.EqualsARGB(other)
    


   
    [<EXT>]
    type Collections.Generic.List<'T>  with        
        [<EXT>] 
        /// Allows for negtive slice index too (-1 = last element), returns a shallow copy
        member this.GetSlice = function
            | None  , None   -> this
            | Some a, None   -> this.GetRange(a,this.Count-a)
            | None  , Some b -> if b<0 then this.GetRange(0,this.Count+b)   else this.GetRange(0,b+2)
            | Some a, Some b -> if b<0 then this.GetRange(a,this.Count+b-a) else this.GetRange(a,b+2-a)
        
        [<EXT>] 
        /// Allows for negtive index too (like python)
        member this.GetItem index = if index<0 then this.[this.Count+index]   else this.[index]
        
        [<EXT>] 
        /// Allows for negtive index too (like python)
        member this.SetItem index value = if index<0 then this.[this.Count+index]<-value   else this.[index]<-value 
    
    
    [<EXT>]
    type ``[]``<'T>  with //Generic Array
        [<EXT>] 
        /// Allows for negtive index too (like python)
        member this.GetItem index = if index<0 then this.[this.Length+index]   else this.[index]
        
        [<EXT>] 
        /// Allows for negtive index too (like python)
        member this.SetItem index value = if index<0 then this.[this.Length+index]<-value   else this.[index]<-value 