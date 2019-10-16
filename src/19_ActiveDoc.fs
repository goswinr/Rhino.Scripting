namespace Rhino.Scripting


open Rhino.Runtime

[<AutoOpen>]
module ActiceDocument =


    /// the current active Rhino document (= the file currently open)
    let mutable Doc = 
        if HostUtils.RunningInRhino then 
            Rhino.RhinoDoc.ActiveDoc 
        else 
            failwith "failed to find the active Rhino document, is this dll running inside Rhino? " 
    
    /// redraws all Rhino viewports
    let redraw() = Doc.Views.Redraw()
   

    ///of the currently Running Rhino Instance, to be set via RhinoScriptSyntax.SynchronizationContext from running script
    let mutable internal syncContext = System.Threading.SynchronizationContext.Current  
    
    /// to store last created object form executing a rs.Command(...)
    let mutable internal commandSerialNumbers : option<uint32*uint32> = None // to store last created object form executing a rs.Command(...)

    do
        if HostUtils.RunningInRhino then 
            Rhino.RhinoDoc.EndOpenDocument.Add (fun args -> Doc <- args.Document)
            
            //RhinoApp.EscapeKeyPressed.Add ( fun _ -> failwith "Esc Key was pressed, Exception raised via Rhino.Scripting")


module internal NiceString=
open System.Collections.Generic
open System

let maxDepth= 3
let maxItemsPerSeq = 3

let (|IsICollection|_|) (xs : obj) =
    let interfaces= xs.GetType().GetInterfaces()
    let ICollType = interfaces  |> Seq.tryFind( fun x -> x.IsGenericType && x.GetGenericTypeDefinition() = typedefof<ICollection<_>> )
    match ICollType with
    |Some t ->
        let args = t.GetGenericArguments()
        if args.Length = 1 then 
            if args.[0].IsValueType then //create new trimmed Colllection with values boxed
                let rs = ResizeArray<obj>()
                let ics = xs :?> Collections.ICollection
                let enum = ics.GetEnumerator()
                let mutable k=0
                while enum.MoveNext() && k < maxItemsPerSeq do
                    rs.Add (box enum.Current)
                    k <- k+1
                Some (ics.Count,  rs :> ICollection<_>)                 
            else                
                let ics = xs :?> ICollection<_> 
                Some (ics.Count, ics )
        else
            None
    |None -> None
    
let sb = Text.StringBuilder()

let add (s:string) =  s|> sb.Append |> ignore
let addn (s:string) = s|> sb.AppendLine |> ignore

let rec toNiceStringRec (x:'T,indent:int) =
    //let ind  = System.String(' ', 4 * (indent-1))
    let ind = System.String(' ', 4 *  indent )
    
    match box x with
    | null -> "-null-"
    | :? float      as v   -> v.ToString() |> addn
    | :? single     as v   -> v.ToString() |> addn
    | :? Char       as c   -> c.ToString() |> addn // "'" + c.ToString() + "'"
    | :? string     as s   -> s |> addn // to not have it in quotes
    | :? Guid       as g   -> sprintf "Guid[%O]" g |> addn
    | IsICollection (count,xs) -> 
                let sb = Text.StringBuilder()
                //let ind  = System.String(' ', 4 * (indent-1))
                //let indn = System.String(' ', 4 *  indent   )
                //printfn "ind %d"  ind.Length
                //printfn "indn %d" indn.Length
                sprintf "%A with %d items:(%d indent)" xs count indent |> sb.AppendLine |> ignore        
                if indent < maxDepth + 1 then 
                    for x in xs  |> Seq.truncate maxItemsPerSeq do  
                        //indn                             |> sb.Append      |> ignore
                        let s = toNiceStringRec (x, indent+1)    
                        sprintf "%A (%d indentt)" x  (indent+1) |> sb.AppendLine |> ignore    
                    if count > maxItemsPerSeq then 
                        //System.String(' ', 4*indent) |> sb.Append      |> ignore
                        "..." |> sb.AppendLine  |> ignore
                
                sb.ToString()
                
    | _ ->  x.ToString()

let toNiceString x = 
    sb.Clear() |> ignore
    toNiceStringRec(x,0)
    sb.ToString()
        
printfn "%s" <| toNiceString [| [| [| 9;8;8;4;5;6|];[| 1;2;3|] |] ;[| [| 9;8;8;4;5|];[| 1;2|]|] |]

printfn "%s" <| toNiceString 99.99

module internal NiceString = 
    open System.Collections.Generic
    open Rhino.Geometry
    open System

    let maxDepth= 3
    let maxItemsPerSeq = 3

    let (|IsICollection|_|) (xs : obj) =
        let interfaces= xs.GetType().GetInterfaces()
        let ICollType = interfaces  |> Seq.tryFind( fun x -> x.IsGenericType && x.GetGenericTypeDefinition() = typedefof<ICollection<_>> )
        match ICollType with
        |Some t ->
            let args = t.GetGenericArguments()
            if args.Length = 1 then 
                if args.[0].IsValueType then //create new trimmed Colllection with values boxed
                    let rs = ResizeArray<obj>()
                    let ics = xs :?> Collections.ICollection
                    let enum = ics.GetEnumerator()
                    let mutable k=0
                    while enum.MoveNext() && k < maxItemsPerSeq do
                        rs.Add (box enum.Current)
                        k <- k+1
                    Some ( rs :> ICollection<_>)                 
                else                
                    Some ( xs :?> ICollection<_> )
            else
                None
        |None -> None
        

    let rec  formatICollection (xs:ICollection<'T>,indent:int) =
        let sb = Text.StringBuilder()
        let ind= max 0 (4*(indent-1))
        sprintf "%s%A with %d items:" (System.String(' ',ind)) xs xs.Count |> sb.AppendLine |> ignore        
        if indent < maxDepth then 
            for x in xs  |> Seq.truncate maxItemsPerSeq do // add sorting ? print 3 lines??                                            
                System.String(' ',4*indent)     |> sb.Append      |> ignore
                toNiceStringRec (x,indent+1)        |> sb.AppendLine |> ignore                
                if xs.Count > maxItemsPerSeq then 
                        "..." |> sb.AppendLine  |> ignore
        sb.ToString()


    and toNiceStringRec (x:'T,indent:int) =
        match box x with
        | null -> "-null-"
        | :? Point3d    as p   -> p.ToNiceString
        | :? Vector3d   as v   -> v.ToNiceString
        | :? Point3f    as p   -> p.ToNiceString
        | :? float      as v   -> v.ToNiceString
        | :? single     as v   -> v.ToNiceString
        | :? Char       as c   -> c.ToString()  // "'" + c.ToString() + "'"
        | :? string     as s   -> s // to not have it in quotes
        | :? Guid       as g   -> sprintf "Guid[%O]" g
        | IsICollection xs -> formatICollection(xs,indent)
        | _ ->  x.ToString()

    let toNiceString x = toNiceStringRec(x,0)

module internal NiceStringNONREcusrive = 
    open System.Collections.Generic
    open Rhino.Geometry
    open System
        
    let  internal maxItemsPerSeq = 3
        
    // cant use recusion on thes function becaus inline is neded to keep them generic 
        
    let inline internal toNiceString3 (x:'T) =
        match box x with
        | null -> "NULL"
        | :? Point3d    as p   -> p.ToNiceString
        | :? Vector3d   as v   -> v.ToNiceString
        | :? Point3f    as p   -> p.ToNiceString
        | :? float      as v   -> v.ToNiceString
        | :? single     as v   -> v.ToNiceString
        | :? Char       as c   -> c.ToString()  // "'" + c.ToString() + "'"
        | :? string     as s   -> s // to not have it in quotes     
        | _ ->  x.ToString()   
        
    let inline internal formatICollection2 (xs:ICollection<'T>) =
        let sb = Text.StringBuilder()
        sprintf "%s%A with %d items:" (System.String(' ',4))xs xs.Count |> sb.AppendLine |> ignore        
        for x in xs  |> Seq.truncate maxItemsPerSeq do // add sorting ? print 3 lines??                                            
            System.String(' ',8)     |> sb.Append |> ignore
            toNiceString3 x          |> sb.AppendLine |> ignore                
            if xs.Count > maxItemsPerSeq then 
                    "..." |> sb.AppendLine  |> ignore
        sb.ToString()
                    
        
    let  internal toNiceString2 (x:'T) =
        match box x with
        | null -> "NULL"
        | :? Point3d    as p   -> p.ToNiceString
        | :? Vector3d   as v   -> v.ToNiceString
        | :? Point3f    as p   -> p.ToNiceString
        | :? float      as v   -> v.ToNiceString
        | :? single     as v   -> v.ToNiceString
        | :? Char       as c   -> c.ToString()  // "'" + c.ToString() + "'"
        | :? string     as s   -> s // to not have it in quotes
        | :? ICollection<int>   as xs -> formatICollection2(xs)
        | :? ICollection<float> as xs -> formatICollection2(xs)
        | :? ICollection<Guid>  as xs -> formatICollection2(xs)
        | :? ICollection<obj>   as xs -> formatICollection2(xs)        
        | _ ->  x.ToString()    
            
    let inline internal formatICollection (xs:ICollection<'T>) =
        let sb = Text.StringBuilder()
        sprintf "%A with %d items:" xs xs.Count |> sb.AppendLine |> ignore        
        for x in xs  |> Seq.truncate maxItemsPerSeq do // add sorting ? print 3 lines??                                            
            System.String(' ',4)     |> sb.Append |> ignore
            toNiceString2 x                 |> sb.AppendLine |> ignore                
            if xs.Count > maxItemsPerSeq then 
                    "..." |> sb.AppendLine  |> ignore
        sb.ToString()
                    
        
    let toNiceString (x:'T) =
        match box x with
        | null -> "NULL"
        | :? Point3d    as p   -> p.ToNiceString
        | :? Vector3d   as v   -> v.ToNiceString
        | :? Point3f    as p   -> p.ToNiceString
        | :? float      as v   -> v.ToNiceString
        | :? single     as v   -> v.ToNiceString
        | :? Char       as c   -> c.ToString()  // "'" + c.ToString() + "'"
        | :? string     as s   -> s // to not have it in quotes
        | :? ICollection<int>   as xs -> formatICollection(xs)
        | :? ICollection<float> as xs -> formatICollection(xs)
        | :? ICollection<Guid>  as xs -> formatICollection(xs)
        | :? ICollection<obj>   as xs -> formatICollection(xs)        
        | _ ->  x.ToString()