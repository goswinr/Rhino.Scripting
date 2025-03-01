#r "nuget: FsEx"

open System
// open FsEx

let folder = @"D:\Git\Rhino.Scripting\Src\"
let files = IO.getAllFilesByPattern folder "*.fs" |> Rarr.ofSeq



let code = Hashset()

for f in files do
    let shortPath = f.Replace(folder, "")
    printfnColor 255 110 255 "%s:" shortPath
    let lines = IO.File.ReadAllLines(f)
    let mutable any  = false

    for ln in lines do
        let t = ln.Trim()
        if t.StartsWith "///" then
            for w in String.split " " ln do
                w
                |> Seq.skipWhile (not<<Char.IsLetterOrDigit)
                |> Seq.takeWhile (Char.IsLetterOrDigit)
                |> String.Concat
                |> code.Add
                |> ignore

code
|> Seq.sort
|> Seq.iter (printfn "%s" )




