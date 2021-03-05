#r @"D:\Git\FsEx\bin\Release\netstandard2.0\FsEx.dll"
//#r @"D:\OneDrive\10_Dev\01_Nuget\fparsec.1.1.1\lib\netstandard2.0\FParsecCS.dll"
//#r @"D:\OneDrive\10_Dev\01_Nuget\fparsec.1.1.1\lib\netstandard2.0\FParsec.dll"

open System
open FsEx

let dir = @"D:\Git\Rhino.Scripting\Src" 

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