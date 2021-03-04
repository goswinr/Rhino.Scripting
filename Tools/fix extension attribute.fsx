#r @"D:\Git\FsEx\bin\Release\netstandard2.0\FsEx.dll"
//#r @"D:\OneDrive\10_Dev\01_Nuget\fparsec.1.1.1\lib\netstandard2.0\FParsecCS.dll"
//#r @"D:\OneDrive\10_Dev\01_Nuget\fparsec.1.1.1\lib\netstandard2.0\FParsec.dll"

open System
open FsEx

let dir = @"D:\Git\Rhino.Scripting\Src" 

type Arg =  
    |Str of string
    |Cha of string
    |ChAr of string
    |Int of string 
    |Oth of string 
    
let doAddNullcheck(path) =
    let ilns = IO.File.ReadAllLines(path) 
    
    let getRaiseFail (a:Arg) (n:String) (decl:string)  (args:Rarr<Arg>) = 
        stringBuffer{ 
            yield String(' ', 8) 
            yield "if isNull "
            yield n
            yield " then FsExStringException.Raise \"String."
            yield decl
            yield ": "
            yield n
            yield " is null."
            for o in args do  
                if o<>a then  
                    yield " ("
                    match o with 
                    |Str n -> yield n;  yield ":%s) "
                    |Cha n -> yield n;  yield ":%c) "
                    |ChAr n -> yield n; yield ":%A) "
                    |Int n -> yield n;  yield ":%d) "
                    |Oth n ->  
                        yield n;  
                        if   n="culture" then   yield ":%A) "
                        elif n="options" then   yield ":%A) "
                        else                    yield ":%d) "
            yield "\"" 
            for o in args do  
                if o<>a then 
                    match o with 
                    |Str  n -> yield " (exnf "; yield n;  yield ")"
                    |Cha  n -> yield " "; yield n
                    |ChAr n -> yield " "; yield n
                    |Int  n -> yield " "; yield n
                    |Oth  n -> yield " "; yield n
            } 
            
            
    let getChecks(s:string) =
        let dec0, body = String.splitOnce "=" s
        let body = String(' ', 8) + body.Trim() 
        let _, dec = String.splitOnce "(*inline*) " dec0
        let des =  
            dec 
            |> String.replace " :" ":"
            |> String.replace ": " ":"
            |> String.split " "
        let name = des.First.Trim().Trim( [| '('; ')' |])  
        let args = Array.skip 1 des
        
        
        let args =  
            rarr{ for a in args do  
                    if   a.Contains ":string[]" then  Oth (String.beforeChar ':'      (a.Trim( [| '('; ')' |]) ) ) 
                    elif a.Contains ":string"   then  Str (String.beforeChar ':'      (a.Trim( [| '('; ')' |]) ) ) 
                    elif a.Contains ":char[]"   then  ChAr(String.beforeChar ':'      (a.Trim( [| '('; ')' |]) ) )  
                    elif a.Contains ":char"     then  Cha (String.beforeChar ':'      (a.Trim( [| '('; ')' |]) ) )  
                    elif a.Contains ":int"      then  Int (String.beforeCharMaybe ':' (a.Trim( [| '('; ')' |]) ) ) 
                    else                              Oth (String.beforeCharMaybe ':' (a.Trim( [| '('; ')' |]) ) ) 
                }                               
        
        
        rarr{  
            dec0 + " ="
            for a in args do
                match a with 
                |Str n -> getRaiseFail a n name args
                |_ -> () 
            body
            }
            
        
        
       
    
    
    
    rarr{
        for ln in ilns do 
            let tln = ln.Trim() 
            if tln.StartsWith "let (*inl" && not<| tln.EndsWith "=" then  
                yield! getChecks ln
            else 
                () 
                yield ln
        }
                
    |> Seq.iter (printfn "%s") 
    //|> fun xs -> IO.File.WriteAllLines(file, xs) 

let doFixAttr (path) = 
    printfn "doing %s" path 
    let mutable addExt = null
    rarr{
        for prev, this, next in IO.File.ReadAllLines(path) |> Seq.prevThisNext do 
            let p = prev.Trim() 
            let t = this.Trim() 
            let n = next.Trim() 
            
            if t = "[<Extension>]" && n.StartsWith "///" then  
                addExt <- this
            
            elif notNull addExt && t.StartsWith "static member" then  
                yield addExt
                yield this
                addExt <- null
            else 
                yield this
           
        }
                
    //|>! Seq.iter (printfn "%s") 
    |> fun xs -> IO.File.WriteAllLines(path, xs) 
    
    
    
    
    
let files = IO.getAllFilesByPattern dir "*.fs"

for file in files |> Seq.truncate 99 do
    doFixAttr file

printfn "*Done!"