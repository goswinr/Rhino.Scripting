namespace Rhino.Scripting


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Seq =   
    
    ///considers sequence cirular and move elements up or down
    /// e.g.: rotate +1 [a,b,c,d] = [d,a,b,c]
    /// e.g.: rotate -1 [a,b,c,d] = [b,c,d,a]
    let rotate r (xs:seq<_>) = xs |> ResizeArray.ofSeq |> ResizeArray.rotate r

    
    ///* yields Seq of (this, next) from (first, second)  upto (second-last, last)
    let thisAndNext (xs:seq<_>) = seq{ 
        use e = xs.GetEnumerator()        
        if e.MoveNext() then
            let prev = ref e.Current
            if e.MoveNext() then
                    yield !prev,e.Current 
                    prev := e.Current 
                    while e.MoveNext() do 
                        yield  !prev, e.Current 
                        prev := e.Current
            else
                failwith "thisNext: Input Sequence only had one element"
        else
            failwith "thisNext: Empty Input Sequence"}
    
    ///* yields Seq of (index this, this, next) from (first, second)  upto (second-last, last)
    let iThisAndNext (xs:seq<_>) = seq{
        use e = xs.GetEnumerator()
        let kk = ref 0              
        if e.MoveNext() then
            let prev = ref e.Current
            if e.MoveNext() then
                    yield !kk,!prev,e.Current 
                    prev := e.Current 
                    while e.MoveNext() do
                        incr kk 
                        yield !kk,!prev, e.Current 
                        prev := e.Current
            else
                failwith "* thisNext: Input Sequence only had one element"
        else
            failwith "* thisNext: Empty Input Sequence"}


    ///* yields looped Seq of (this, next) from (first, second)  upto (last, first)
    let thisAndNextLooped (xs:seq<_>) =  seq{ 
        use e = xs.GetEnumerator()
        if e.MoveNext() then
            let prev = ref e.Current
            let first = e.Current
            if e.MoveNext() then
                    yield !prev,e.Current 
                    prev := e.Current 
                    while e.MoveNext() do 
                        yield  !prev, e.Current 
                        prev := e.Current
                    yield !prev, first
            else
                failwith "thisNextLooped: Input Sequence only had one element"
        else
            failwith "thisNextLooped: Empty Input Sequence"}
    
    ///* yields looped Seq of (this, next) from (first, second)  upto (last, first)
    let iThisAndNextLooped (xs:seq<_>) =  seq{ 
        use e = xs.GetEnumerator()
        let kk = ref 0  
        if e.MoveNext() then
            let prev = ref e.Current
            let first = e.Current
            if e.MoveNext() then
                    yield !kk, !prev,e.Current 
                    prev := e.Current 
                    while e.MoveNext() do
                        incr kk 
                        yield  !kk, !prev, e.Current 
                        prev := e.Current
                    incr kk 
                    yield !kk, !prev, first
            else
                failwith "thisNextLooped: Input Sequence only had one element"
        else
            failwith "thisNextLooped: Empty Input Sequence"}

    ///* yields Seq of (this, next, Nextafter): from (first, second, third)  upto (third-last, second-last, last)
    let thisNextAndNextafter (xs:seq<_>) =  seq{ 
        use e = xs.GetEnumerator()
        if e.MoveNext() then
            let prev = ref e.Current
            if e.MoveNext() then
                let this = ref e.Current
                if e.MoveNext() then
                    yield !prev, !this, e.Current 
                    prev := !this 
                    this := e.Current                        
                    while e.MoveNext() do 
                        yield  !prev, !this, e.Current
                        prev := !this  
                        this := e.Current                              
                else
                    failwith "thisNextNextaftert: Input Sequence only had two elements"
            else
                failwith "thisNextNextaftert: Input Sequence only had one element"
        else
            failwith "thisNextNextaftert: Empty Input Sequence"}
    
    ///* yields looped Seq of (this, next, Nextafter): from (first, second, third)  upto (last, first, second)
    let thisNextAndNextafterLooped (xs:seq<_>) =  seq{ 
        use e = xs.GetEnumerator()
        if e.MoveNext() then
            let prev = ref e.Current
            let first = e.Current
            if e.MoveNext() then
                let this = ref e.Current
                let second = e.Current
                if e.MoveNext() then
                    yield !prev, !this, e.Current
                    prev := !this  
                    this := e.Current
                    while e.MoveNext() do 
                        yield  !prev, !this, e.Current
                        prev := !this 
                        this := e.Current                            
                    yield !prev, !this,  first
                    yield !this, first, second
                else
                    failwithf "thisNextNextaftertLooped: Input Sequence %A only had two elements" xs
            else
                failwithf "thisNextNextaftertLooped: Input Sequence %A only had one element" xs
        else
            failwithf "thisNextNextaftertLooped: Empty Input Sequence %A" xs}
    
    ///* yields looped Seq of (Index of next, this, next, Nextafter): from (1,first, second, third)  upto (0,last, first, second)
    let iThisNextAndNextafterLooped (xs:seq<_>) =  seq{ 
        use e = xs.GetEnumerator()
        let kk = ref 2
        if e.MoveNext() then
            let prev = ref e.Current
            let first = e.Current
            if e.MoveNext() then
                let this = ref e.Current
                let second = e.Current
                if e.MoveNext() then
                    yield 1, !prev, !this, e.Current                    
                    prev := !this  
                    this := e.Current
                    while e.MoveNext() do 
                        yield  !kk,!prev, !this, e.Current
                        incr kk
                        prev := !this 
                        this := e.Current                            
                    yield !kk, !prev, !this,  first
                    yield 0, !this, first, second
                else
                    failwithf "thisNextNextaftertLooped: Input Sequence %A only had two elements" xs
            else
                failwithf "thisNextNextaftertLooped: Input Sequence %A only had one element" xs
        else
            failwithf "thisNextNextaftertLooped: Empty Input Sequence %A" xs}    

    ///* yields looped Seq of (previous, this, next): from (last, first, second)  upto (second-last, last, first)
    ///* Consider "thisNextNextafterLooped" as faster since the last element is not required from the start on.
    let prevThisNextLooped (xs:seq<_>) =  seq { 
        use e = xs.GetEnumerator()
        if e.MoveNext() then
            let prev = ref e.Current
            let first = e.Current
            if e.MoveNext() then
                let this = ref e.Current
                if e.MoveNext() then
                    yield Seq.last xs ,!prev, !this
                    yield !prev, !this, e.Current
                    prev := !this  
                    this := e.Current
                    while e.MoveNext() do 
                        yield  !prev, !this, e.Current
                        prev := !this 
                        this := e.Current                            
                    yield !prev, !this,  first
                else                     
                    failwithf "prevThisNextLooped: Input Sequence %A only had two elements" xs
            else
                failwithf "prevThisNextLooped: Input Sequence %A only had one element" xs
        else
            failwithf "prevThisNextLooped: Empty Input Sequence %A" xs} 

    ///* yields looped Seq of (index,previous, this, next): from (0,last, first, second)  upto (lastIndex, second-last, last, first)
    ///* Consider "iThisNextNextafterLooped" as faster since the last element is not required from the start on.
    let iPrevThisNextLooped (xs:seq<_>) =  seq { 
        use e = xs.GetEnumerator()
        let kk = ref 2
        if e.MoveNext() then
            let prev = ref e.Current
            let first = e.Current
            if e.MoveNext() then
                let this = ref e.Current
                if e.MoveNext() then
                    yield  0, Seq.last xs ,!prev, !this
                    yield  1, !prev, !this, e.Current
                    prev := !this  
                    this := e.Current
                    while e.MoveNext() do 
                        yield !kk, !prev, !this, e.Current
                        incr kk
                        prev := !this 
                        this := e.Current                            
                    yield !kk, !prev, !this,  first
                else                     
                    failwithf "prevThisNextLooped: Input Sequence %A only had two elements" xs
            else
                failwithf "prevThisNextLooped: Input Sequence %A only had one element" xs
        else
            failwithf "prevThisNextLooped: Empty Input Sequence %A" xs} 

