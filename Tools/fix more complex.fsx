#r @"D:\Git\FsEx\bin\Release\netstandard2.0\FsEx.dll"

open System
open FsEx

let folder = @"D:\Git\Rhino.Scripting\Src\"
let files = IO.getAllFilesByPattern folder "*.fs" |> Rarr.ofSeq


//let prevName, newName = "..</summary>" ,  ".</summary>"


let find = ") of"

for f in files do
    let shortPath = f.Replace(folder, "") 
    printfnColor 255 110 255 "%s:" shortPath
    let lines = IO.File.ReadAllLines(f)
    let mutable any  = false
    let code = stringBuffer{ 
        for ln in lines do    
            let t = ln.Trim() 
            if t.StartsWith "///" then 
                
                let i = t.IndexOf( find, StringComparison.Ordinal ) 
                let nc = t.[i + find.Length ]
                if Char.IsLower nc then 
                    printWithHighlight find ln
                    //yield! ln.Replace(prevName, newName)
                    any <- true
                else 
                    yield! ln
            else 
                yield! ln
                    
        }
    if false   && any then     
        if code.EndsWith(Environment.NewLine) then 
            IO.File.WriteAllText(f, code) 
        else 
            IO.File.WriteAllText(f, code + Environment.NewLine) 
    
     
        
          
