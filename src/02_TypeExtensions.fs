namespace Rhino.Scripting

open System
open Rhino.Scripting.Util

[<AutoOpen>]
module TypeExtensions =   
    
    type Object with 
        [<EXT>]  
        ///A property like the ToString() method, 
        ///but with richer formationg for collections.
        member obj.ToNiceString = NiceString.toNiceString obj

    type Int32 with  
        [<EXT>] member inline x.ToDouble = float(x)
        [<EXT>] member inline x.ToByte = byte(x)


    type Byte with  
        [<EXT>] member inline x.ToDouble = float(x)
        [<EXT>] member inline x.ToInt = int(x)
    
   
    type Double with  
        ///converts int to float including rounding: 
        ///int(round(x))
        [<EXT>] member inline x.ToInt = int(round(x))

        /// with automatic formating of display precision depending on float size
        [<EXT>] member x.ToNiceString = NiceString.floatToString x        
        

    type Single with  
        /// with automatic formating of display precision depending on float size
        [<EXT>] member x.ToNiceString = NiceString.singleToString x


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
    

    type Collections.Generic.Dictionary<'K,'V> with           
        [<EXT>]
        member inline  d.SetValue k v =
                    d.[k] <-v        
       
        [<EXT>] 
        member inline d.GetValue k  =
                    d.[k]
        
        /// get a value and remove it from Dictionary, like *.pop() in Python         
        [<EXT>] 
        member inline d.Pop k  =
                    let v= d.[k]
                    d.Remove(k)|> ignore
                    v

        /// Returns a seq of key and value tuples
        [<EXT>] 
        member inline d.Items =
                    seq { for KeyValue(k,v) in d -> k,v}

   
    type Collections.Generic.List<'T>  with        
        [<EXT>] 
        /// Allows for negtive slice index too ( -1 = last element), returns a shallow copy including the end index.
        member this.GetSlice(startIdx,endIdx) =    
            let count = this.Count
            let st  = match startIdx with None -> 0        | Some i -> if i<0 then count+i      else i
            let len = match endIdx   with None -> count-st | Some i -> if i<0 then count+i-st+1 else i-st+1
    
            if st < 0 || st > count-1 then 
                let err = sprintf "GetSlice: Start Index %d is out of Range. Allowed values are -%d upto %d for List of %d items" startIdx.Value count (count-1) count
                raise (IndexOutOfRangeException(err))
    
            if st+len > count then 
                let err = sprintf "GetSlice: End Index %d is out of Range. Allowed values are -%d upto %d for List of %d items" endIdx.Value count (count-1) count
                raise (IndexOutOfRangeException(err)) 
                
            if len < 0 then
                let en =  match endIdx  with None -> count-1 | Some i -> if i<0 then count+i else i
                let err = sprintf "GetSlice: Start Index '%A' (= %d) is bigger than End Index '%A'(= %d) for List of %d items" startIdx st startIdx en  count
                raise (IndexOutOfRangeException(err)) 
                
            this.GetRange(st,len)
        
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

        //member this.GetSlice(startIdx,endIdx) = // overides of existing methods are ignored / not possible