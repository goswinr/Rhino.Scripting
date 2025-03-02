#r "nuget: FsEx.IO"
#r "nuget: Fesher"
#r "nuget: Str"
open Str
open System
open FsEx
open Fesher
open Fesher

clearFeshLog()
for p in IO.getAllFilesByPattern "*.fs" "D:/Git/_Rhino.Scripting_/Rhino.Scripting/Src" do 
    if p.Contains "\Scripting_" then
        Printfn.blue $"{p}"
        let lns = IO.File.ReadAllLines(p) 
        for i, ln in Seq.indexed lns do 
            if ln.Contains "=  resizeArray" && ln.Contains "do" then  
                try
                    Printfn.darkGray $"{ln}"
                    let arg  = ln |> Str.between " for " " in "
                    let coll = ln |> Str.between " in " " do yield "
                    let dec  = ln |> Str.before "=  resizeArray"
                    let act  = ln |> Str.after " do yield " |> Str.trimEnd |> Str.trimChar '}'
                    
                    let actArg = act |> Str.between "(" ")"
                    
                    let nln =  
                        if actArg = arg then   
                            let f = act |> Str.before "("
                            $"{dec} = {coll} |> ResizeArray.mapArr {f}"
                        else 
                            $"{dec} = {coll} |> ResizeArray.mapArr (fun {arg} -> {act})"
                            
                    Printfn.green $"{nln}"
                    lns[i] <- nln
                with e -> 
                    eprintfn $"{e}"
        IO.File.WriteAllLines(p, lns, Text.Encoding.UTF8)
                    
                
                
                
                
        










