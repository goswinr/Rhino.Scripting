namespace Rhino.Scripting




[<AutoOpen>]
module StringBuffer =    
    // adapted from https://github.com/fsharp/fslang-suggestions/issues/775

    open System.Text

    type StringBufferBuilder () =
        member inline __.Yield (txt: string) =  fun (b: StringBuilder) -> b.Append  txt |> ignore
        member inline __.Yield (c: char) =      fun (b: StringBuilder) -> b.Append  c   |> ignore
        member inline __.Yield (f: float) =      fun (b: StringBuilder) -> b.Append f.AsNiceString  |> ignore
        member inline __.Yield (i: int) =      fun (b: StringBuilder) ->  b.Append (i.ToString())   |> ignore

        member inline __.YieldFrom (txt: string) =  fun (b: StringBuilder) -> b.AppendLine txt |> ignore // 
        
        
        
        member inline __.Yield (strings: #seq<string>) =
            fun (b: StringBuilder) -> 
                for s in strings do 
                    Printf.bprintf b "%s\n" s
        
        //member inline __.YieldFrom (f: StringBuffer) = f // use for new line instead
        
        member __.Combine (f, g) = fun (b: StringBuilder) -> f b; g b
        
        member __.Delay f = fun (b: StringBuilder) -> (f()) b
        
        member __.Zero () = ignore
        
        member __.For (xs: 'T seq, f: 'T -> StringBuilder -> unit) =
            fun (b: StringBuilder) ->
                use e = xs.GetEnumerator ()
                while e.MoveNext() do
                    (f e.Current) b
        
        member __.While (p: unit -> bool, f: StringBuilder -> unit) =
            fun (b: StringBuilder) -> 
                while p () do 
                    f b
            
        member __.Run (f: StringBuilder -> unit) =
            let b = StringBuilder()
            do f b
            b.ToString()
    
    /// Computational Expression:  use 'yield' to append text and 'yield!' (with an !)  to append text followed by new line.
    let stringBuffer = new StringBufferBuilder ()



[<AutoOpen>]
module ResizeArrayBuilder =

    type ResizeArrayBuilder<'T> () =
        member inline __.Yield (x: 'T) =  
            fun (r: ResizeArray<'T>) -> r.Add(x)      
        
        member inline __.YieldFrom (xs: #seq<'T>) =
            fun (r: ResizeArray<'T>) -> r.AddRange(xs) 
        
        member __.Combine (f, g) = 
            fun (r: ResizeArray<'T>) -> f r; g r
        
        member __.Delay f = 
            fun (r: ResizeArray<'T>) -> (f()) r
        
        member __.Zero () = ignore
        
        member __.For (xs: 'U seq, f: 'U -> ResizeArray<'T> -> unit) =
            fun (r: ResizeArray<'T>) ->
                use e = xs.GetEnumerator()
                while e.MoveNext() do
                    (f e.Current) r
        
        member __.While (p: unit -> bool, f: ResizeArray<'T> -> unit) =
            fun (r: ResizeArray<'T>) -> while p () do  f r
            
        member __.Run (f: ResizeArray<'T> -> unit) =
            let r = ResizeArray<'T>()
            do f r
            r  
    // Computational Expression:  use 'yield' to add alements to ResizeArray (= Collections.Generic.List).
    let resizeArray<'T> = new ResizeArrayBuilder<'T> ()


   
    