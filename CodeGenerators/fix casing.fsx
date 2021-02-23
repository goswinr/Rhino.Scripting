#r @"D:\Git\FsEx\bin\Release\netstandard2.0\FsEx.dll"

open System
open FsEx

let folder = @"D:\Git\Rhino.Scripting\Src\"
let files = IO.getAllFilesByPattern folder "*.fs" |> Rarr.ofSeq


let prevName,newName = " rhino" ,  " Rhino"


for f in files do
    let shortPath = f.Replace(folder, "") 
    printfnColor 0 170 0 "%s:" shortPath
    let lines = IO.File.ReadAllLines(f)
    let code = stringBuffer{ 
        for ln in lines do    
            let t = ln.Trim() 
            if t.StartsWith "///" then  
                if t.Contains prevName then 
                    //printfn "%s" ln
                    printWithHighlight prevName ln
                    
        }
    () 
        
          
