namespace Rhino.Scripting

open System.Collections.Generic
open System
open Rhino.Geometry

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module String =

    /// Returns everytrhing before a given splitting string
    /// Full strring if splitter not present
    let before (splitter:string) (s:string) = 
        let start = s.IndexOf(splitter) 
        if start = -1 then s
        else s.Substring(0, start )

    ///s.Split([| spliter |], StringSplitOptions.RemoveEmptyEntries)
    let split (spliter:string) (s:string) = s.Split([|spliter|], StringSplitOptions.RemoveEmptyEntries)
    
    ///s.Split([| spliter |], StringSplitOptions.None)  
    let splitkeep (spliter:string) (s:string) = s.Split([|spliter|], StringSplitOptions.None)    
    
    ///split string into maximum two elements
    ///.Split( [| spliter |],2, StringSplitOptions.RemoveEmptyEntries) in if xs.Length > 1 then xs.[0],xs.[1] else s,""
    let split2 (spliter:string) (s:string) = let xs = s.Split( [|spliter|],2, StringSplitOptions.None) in if xs.Length > 1 then xs.[0],xs.[1] else s,""
    

    let trim (s:string) = s.Trim()
    let trimEnd (s:string) = s.TrimEnd()
    let trimStart (s:string) = s.TrimStart()   

    ///s.Replace(a, b)
    let replace (a:string) (b:string) (s:string) = s.Replace(a, b)

    ///finds text betwween two strings
    ///between "((" ")" "c((ab)c" = ("c", "ab", "c")
    let between (a:string) (b:string) (s:string) = 
        
        let start = s.IndexOf(a) 
        if start = -1 then "","",""
        else 
            let ende = s.IndexOf(b, start + a.Length)
            if ende = -1 then "","",""
            else 
                s.Substring(0, start ),
                s.Substring(start + a.Length, ende - start - a.Length),// finds text betwween two chars
                s.Substring(ende + b.Length)
    
    /// finds text betwween two strings including delimiters on middle string 
    ///betweenIncl "((" "))" "c((ab))d" = "c", "((ab))", "d"
    let betweenIncl (a:string) (b:string) (s:string) = 
        
        let start = s.IndexOf(a) 
        if start = -1 then "","",""
        else 
            let ende = s.IndexOf(b, start + a.Length)
            if ende = -1 then "","",""
            else 
                s.Substring(0, start),
                s.Substring(start, ende - start + b.Length),// finds text betwween two chars
                s.Substring(ende + b.Length)
    
  
    
    let up1 (s:String)  = if s="" then s else Char.ToUpper(s.[0]).ToString() + s.Substring(1)
    let low1 (s:String) = if s="" then s else Char.ToLower(s.[0]).ToString() + s.Substring(1)
