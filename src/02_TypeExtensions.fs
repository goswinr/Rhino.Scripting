namespace Rhino.Scripting

open System
open Rhino.Scripting.Util

[<AutoOpen>]
module TypeExtensions =    
    
    type Collections.Generic.Dictionary<'K,'V> with
        
        
        [<EXT>] member inline  d.SetValue k v =
                    d.[k] <-v
        
       
        [<EXT>] member inline d.GetValue k  =
                    d.[k]
        
        /// get a value and remove it from Dictionary, like *.pop() in Python         
        [<EXT>] member inline d.Pop k  =
                    let v= d.[k]
                    d.Remove(k)|> ignore
                    v

        /// Returns a seq of key and value tuples
        [<EXT>] member inline d.Items =
                    seq { for KeyValue(k,v) in d -> k,v}


    type Int32 with  
        [<EXT>] member inline x.ToDouble = float(x)
        [<EXT>] member inline x.ToByte = byte(x)

    type Byte with  
        [<EXT>] member inline x.ToDouble = float(x)
        [<EXT>] member inline x.ToInt = int(x)

    type Double with  
        ///converts int to float including rounding: int(round(x))
        [<EXT>] member inline x.ToInt = int(round(x))
        /// with automatic formating of display precision depending on float size
        [<EXT>] member x.ToNiceString = Util.floatToString x        
        

    type Single with  
        /// with automatic formating of display precision depending on float size
        [<EXT>] member x.ToNiceString = Util.singleToString x

    type Drawing.Color with        
        ///Compare to another color only by Alpha, Red, Green and Blue values ignoring other fields such as IsNamedColor        
        [<EXT>] 
        member inline this.EqualsARGB(other:Drawing.Color)=
            this.A = other.A && 
            this.R = other.R && 
            this.G = other.G && 
            this.B = other.B        
        
        ///Compare two colors only by Alpha, Red, Green and Blue values ignoring other fields such as IsNamedColor
        [<EXT>]
        static member inline AreEqualARGB (this:Drawing.Color)(other:Drawing.Color)=
            this.EqualsARGB(other)
    


   
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
    
    
    type ``[]``<'T>  with //Generic Array
        [<EXT>] 
        /// Allows for negtive index too (like python)
        member this.GetItem index = if index<0 then this.[this.Length+index]   else this.[index]
        
        [<EXT>] 
        /// Allows for negtive index too (like python)
        member this.SetItem index value = if index<0 then this.[this.Length+index]<-value   else this.[index]<-value 