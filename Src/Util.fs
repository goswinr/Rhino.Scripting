namespace Rhino.Scripting


//[<AutoOpen>]
module internal Util =   

    /// If first value is 0.0 return second else first
    let inline ifZero1 a b = if a = 0.0 then b else a
    
    /// if second value is 0.0 return first else second
    let inline ifZero2 a b = if b = 0.0 then a else b    
