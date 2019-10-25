namespace Rhino.Scripting

open System.Collections.Generic
open System
open Rhino.Geometry


module NiceString =
    
    
    /// with automatic formationg of precision
    let floatToString  (x:float) =
        if Double.IsNaN x || Double.IsInfinity x then sprintf "%f"  x
        elif x = Rhino.RhinoMath.UnsetValue then "RhinoMath.UnsetValue"
        else
            let  a = abs x
            if   a > 1000. then sprintf "%.0f" x
            elif a > 100.  then sprintf "%.1f" x 
            elif a > 10.   then sprintf "%.2f" x 
            elif a > 1.    then sprintf "%.3f" x 
            else                sprintf "%f"   x  

    /// with automatic formationg of precision
    let singleToString  (x:float32) =
        if Single.IsNaN x || Single.IsInfinity x then sprintf "%f"  x
        elif x = Rhino.RhinoMath.UnsetSingle then "RhinoMath.UnsetSingle"
        else
            let  a = abs x
            if   a > 1000.f then sprintf "%.0f" x
            elif a > 100.f  then sprintf "%.1f" x 
            elif a > 10.f   then sprintf "%.2f" x 
            elif a > 1.f    then sprintf "%.3f" x 
            else                 sprintf "%f"   x

    ///Like the ToString function but with appropiate precision formating       
    let point3dToString (pt:Point3d) =
        if pt = Point3d.Unset then  
            "Point3d.Unset"
        else
            sprintf "Point3d(%s, %s, %s)" (floatToString  pt.X) (floatToString  pt.Y) (floatToString  pt.Z)


    ///Like the ToString function but with appropiate precision formating       
    let point3fToString (pt:Point3f) =
        if pt = Point3f.Unset then  
            "Point3f.Unset"
        else
            sprintf "Point3f(%s, %s, %s)" (singleToString  pt.X) (singleToString  pt.Y) (singleToString  pt.Z)


    ///Like the ToString function but with appropiate precision formating       
    let vector3dToString (v:Vector3d) =
        if v = Vector3d.Unset then  
            "Vector3d.Unset"
        else
            sprintf "Vector3d(%s, %s, %s)" (floatToString  v.X) (floatToString  v.Y) (floatToString  v.Z)
    
    ///Like the ToString function but with appropiate precision formating       
    let vector3fToString (v:Vector3f) =
        if v = Vector3f.Unset then  
            "Vector3f.Unset"
        else
            sprintf "Vector3f(%s, %s, %s)" (singleToString  v.X) (singleToString  v.Y) (singleToString  v.Z)

    
    //--------------------------------
    // -- generic pretty printer-----
    //-------------------------------

    let mutable toNiceStringMaxDepth= 2
    let mutable toNiceStringMaxItemsPerSeq = 4

    let private before (splitter:string) (s:string) = 
        let start = s.IndexOf(splitter) 
        if start = -1 then s
        else s.Substring(0,start )
    
    let private formatTypeName nameSpace name = 
        let name = name |> before "`" // Collections.Generic.List`1  -> Collections.Generic.List
        let fullPath = nameSpace  + "." + name 
        if fullPath.StartsWith "System." then 
            fullPath.Substring(7)
        else 
            fullPath
    

    let private (|IsSeq|_|) (xs : obj) =
        let typ = xs.GetType() 
        let interfaces= typ.GetInterfaces()
        let seqType = interfaces  |> Seq.tryFind( fun x -> x.IsGenericType && x.GetGenericTypeDefinition() = typedefof<IEnumerable<_>> )
        match seqType with
        |Some iet ->
            let iCollType = interfaces  |> Seq.tryFind( fun x -> x.IsGenericType && x.GetGenericTypeDefinition() = typedefof<ICollection<_>> )
            let count =
                match iCollType with
                |None -> -1
                |Some _ -> (xs :?> Collections.ICollection).Count
            
            let args = iet.GetGenericArguments()
            if args.Length = 1 then             
                let arg = args.[0]            
                let name =   formatTypeName typ.Namespace typ.Name           
                let elName = formatTypeName arg.Namespace arg.Name 
                if args.[0].IsValueType then //create new trimmed Colllection with values boxed
                    let rs = ResizeArray<obj>()
                    let ics = xs :?> Collections.IEnumerable
                    let enum = ics.GetEnumerator()
                    let mutable k=0
                    while enum.MoveNext() && k < toNiceStringMaxItemsPerSeq do
                        rs.Add (box enum.Current)
                        k <- k+1
                    Some (count,  rs:>IEnumerable<_>, name, elName)   // the retuened seq is trimmed!  to a length of maxItemsPerSeq              
                else                
                    let ics = xs :?> IEnumerable<_> 
                    Some (count, ics, name, elName)
            else
                None
        |None -> None
    
    let private sb = Text.StringBuilder()


    let rec private toNiceStringRec (x:obj,indent:int) : unit =
        let addn (s:string) =  sb.Append(String(' ', 4 *  indent )).AppendLine(s) |> ignore
        let add  (s:string) =  sb.Append(String(' ', 4 *  indent )).Append(s) |> ignore
        let adn  (s:string) =  sb.AppendLine(s) |> ignore
   
        match x with // boxed already
        | null -> "-null-" |> add
        | :? Point3d    as p   -> p |> point3dToString |> addn
        | :? Vector3d   as v   -> v |> vector3dToString |> addn
        | :? Point3f    as p   -> p |> point3fToString |> addn
        | :? Vector3f    as p  -> p |> vector3fToString |> addn
        | :? float      as v   -> v |> floatToString |> addn
        | :? single     as v   -> v |> singleToString |> addn        
        | :? Char       as c   -> c.ToString()   |> addn // "'" + c.ToString() + "'" // or add qotes?
        | :? string     as s   -> s |> add // to not have it in quotes
        | :? Guid       as g   -> sprintf "Guid[%O]" g |> addn
        | IsSeq (count,xs,name,elName) ->  
                    if count <0 then    sprintf "%s of %s" name elName  |> add  
                    else                sprintf "%s with %d items of %s" name count elName |> add     
                    if indent < toNiceStringMaxDepth  then 
                        adn ":"
                        for x in xs  |> Seq.truncate toNiceStringMaxItemsPerSeq do  
                            toNiceStringRec (x, indent+1)
                        if count > toNiceStringMaxItemsPerSeq then
                            toNiceStringRec ("...", indent+1)
                    else 
                        adn ""                
        | _ ->  x.ToString() |> add

    /// Nice formating for floats , some Rhino Objects and sequences of any kind, first four items are printed out.
    /// set NiceString.toNiceStringMaxItemsPerSeq to other value if more or less shall be shown (default is 4)
    /// set NiceString.toNiceStringMaxDepth to change how deep nested lists are printed (default is 2)
    let toNiceString (x:'T) = 
        sb.Clear() |> ignore
        toNiceStringRec(box x,0)
        let s = sb.ToString()
        let st = s.Trim()
        if st.Contains (Environment.NewLine) then s else st // trim new line on one line strings

    /// Nice formating for floats , some Rhino Objects and sequences of any kind, all items including nested items are printed out.
    let toNiceStringFull (x:'T) = 
        let maxDepthP = toNiceStringMaxDepth  
        let maxItemsPerSeqP = toNiceStringMaxItemsPerSeq 
        toNiceStringMaxDepth <- Int32.MaxValue
        toNiceStringMaxItemsPerSeq  <- Int32.MaxValue

        sb.Clear() |> ignore
        toNiceStringRec(box x,0)

        toNiceStringMaxDepth <- maxDepthP 
        toNiceStringMaxItemsPerSeq  <- maxItemsPerSeqP 
        let s = sb.ToString()
        let st = s.Trim()
        if st.Contains (Environment.NewLine) then s else st // trim new line on one line strings