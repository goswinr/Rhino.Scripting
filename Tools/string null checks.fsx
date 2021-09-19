#r "nuget: FsEx"
//#r @"D:\OneDrive\10_Dev\01_Nuget\fparsec.1.1.1\lib\netstandard2.0\FParsecCS.dll"
//#r @"D:\OneDrive\10_Dev\01_Nuget\fparsec.1.1.1\lib\netstandard2.0\FParsec.dll"

open System
open FsEx

let dir = @"D:\Git\Rhino.Scripting\Src\RhinoScriptSyntax" 
//let dir = @"D:\Git\Rhino.Scripting\Src" 

type Arg =  
    |Str of string
    |Cha of string
    |ChAr of string
    |Int of string 
    |Oth of string 


type ArgKind =  
    |Infer of string //  name
    |Typed of string* string //  name, typ
    |Opt of string*string*string // def, name, typ
    
    static member parse (s:string)  =  
        match String.splitMaybeOnce ")>]" s with 
        | s, "" ->  
            match String.splitMaybeOnce ":" s with 
            | n, "" ->  Infer (n.Trim() ) 
            | n, t -> Typed (n.Trim(), t.Trim())  
        | d, nt ->  
            let d = String.after "(" d |> String.before ":" // to find default value
            let n, t = String.splitOnce ":" nt 
            Opt (d.Trim() , n.Trim(), t.Trim()) 
        
        
        

let argTyps = Hashset<string>() 

let doAddNullcheck(path) =
    printfnColor 0 155 0 "doing %s ..." path 
    
    
    
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
            
    let doDef(txt:string) = 
        let doc,code = String.splitOnce "static member" txt 
        let decl,body = String.splitOnce "=" code
        let name,args = String.splitOnce "(" decl
        try
            let args, retType = String.splitOnce " : " args 
            let name = name.Trim() 
            let args = 
                args
                |> String.trimEnd [| ' '; ')'|]
                |> String.split ","
                |> Array.map (ArgKind.parse)
            
            for arg in args do   
                match arg with
                |Typed(n, t)  -> argTyps.Add t  |> ignore 
                |Opt _ |Infer _ -> () 
                
        with e ->  
            printfn "static member %s" name
            eprintfn "%A" e
        
        //printfn "name:%s args:%s" name (String.concat "|" args) 
        () 
        
        
        
    
    let code = IO.File.ReadAllText(path) 
    
    let es = code |> String.split "///<summary>"
    
    let head = es.First
    
    let defs = es |> Array.skip 1 
    
    for def in defs do 
        doDef( "///<summary>" + def) 
    
    
    
   
    
    
    
let files = IO.getAllFilesByPattern dir "*.fs"

for file in files |> Seq.truncate 99 do
    if not (file.EndsWith "Print.fs")  then 
        doAddNullcheck file
        
    
argTyps
|> Seq.sort
|> printFull

printfn "*Done!"
