namespace Rhino.Scripting




[<AutoOpen>]
module StringBufferBuilder =    
    // adapted from https://github.com/fsharp/fslang-suggestions/issues/775

    open System.Text

    type StringBufferBuilder () =
 
        
        member inline _.Yield (txt: string) =  fun (b: StringBuilder) -> b.Append  txt |> ignore
        member inline _.Yield (c: char) =      fun (b: StringBuilder) -> b.Append  c   |> ignore
        member inline _.Yield (f: float) =     fun (b: StringBuilder) -> b.Append f.ToNiceString  |> ignore
        member inline _.Yield (i: int) =       fun (b: StringBuilder)  -> b.Append (i.ToString())  |> ignore
        //member inline _.Yield (x: 'T) =        fun (b: StringBuilder)  -> b.Append (x.ToString())  |> ignore


        member inline _.YieldFrom (txt: string) =  fun (b: StringBuilder) -> b.AppendLine txt |> ignore // 
        
        //member inline _.YieldFrom (f: StringBuffer) = f // use for new line instead
        
        member inline _.Yield (strings: #seq<string>) =
            fun (b: StringBuilder) -> 
                for s in strings do 
                    Printf.bprintf b "%s\n" s 

        
        
        member _.Combine (f, g) = fun (b: StringBuilder) -> f b; g b
        
        member _.Delay f = fun (b: StringBuilder) -> (f()) b
        
        member _.Zero () = ignore
        
        member _.For (xs: 'T seq, f: 'T -> StringBuilder -> unit) =
            fun (b: StringBuilder) ->
                use e = xs.GetEnumerator ()
                while e.MoveNext() do
                    (f e.Current) b
        
        member _.While (p: unit -> bool, f: StringBuilder -> unit) =
            fun (b: StringBuilder) -> 
                while p () do 
                    f b
            
        member _.Run (f: StringBuilder -> unit) =
            let b = StringBuilder()
            do f b
            b.ToString()
    
    /// Computational Expression:  use 'yield' to append text and 'yield!' (with an exclamation mark)  to append text followed by a new line character.
    let stringBuffer = new StringBufferBuilder ()



[<AutoOpen>]
module ResizeArrayBuilder =

    type ResizeArrayBuilder<'T> () =
        member inline _.Yield (x: 'T) =  
            fun (r: ResizeArray<'T>) -> r.Add(x)      
        
        member inline _.YieldFrom (xs: #seq<'T>) =
            fun (r: ResizeArray<'T>) -> r.AddRange(xs) 
        
        member _.Combine (f, g) = 
            fun (r: ResizeArray<'T>) -> f r; g r
        
        member _.Delay f = 
            fun (r: ResizeArray<'T>) -> (f()) r
        
        member _.Zero () = ignore
        
        member _.For (xs: 'U seq, f: 'U -> ResizeArray<'T> -> unit) =
            fun (r: ResizeArray<'T>) ->
                use e = xs.GetEnumerator()
                while e.MoveNext() do
                    (f e.Current) r
        
        member _.While (p: unit -> bool, f: ResizeArray<'T> -> unit) =
            fun (r: ResizeArray<'T>) -> while p () do  f r
            
        member _.Run (f: ResizeArray<'T> -> unit) =
            let r = ResizeArray<'T>()
            do f r
            r  
    // Computational Expression:  use 'yield' to add alements to a ResizeArray (= Collections.Generic.List).
    let resizeArray<'T> = new ResizeArrayBuilder<'T> ()


   
    