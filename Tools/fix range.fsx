#r @"C:\GitHub\FsEx\bin\Release\netstandard2.0\FsEx.dll"

open System
open FsEx

let dir = @"C:\GitHub\Rhino.Scripting\Src" 

let doFixAttr (path) = 
    //printfn "doing %s" path 
    //rarr{
    for ln in IO.File.ReadAllLines(path)  do 
        
        if ln.Contains "Util.range" then
            let p, t, n = String.splitTwice "(" ")" ln
         
            printWithHighlight "Util.range" ln
            
            let  a = ""
            () 
            
        else 
            //yield ln
            () 
           
        //}
  
                
    //|>! Seq.iter (printfn "%s") 
    //|> fun xs -> IO.File.WriteAllLines(path, xs) 
    
    
    
    
    
let files = IO.getAllFilesByPattern dir "*.fs"

for file in files |> Seq.truncate 99 do
    doFixAttr file

printfn "*Done!"