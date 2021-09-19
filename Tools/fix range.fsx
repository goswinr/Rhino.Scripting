#r "nuget: FsEx"

open System
open FsEx

let dir = @"C:\GitHub\Rhino.Scripting\Src"

let doFixAttr (path) = 
    //printfn "doing %s" path
    rarr{
        for ln in IO.File.ReadAllLines(path)  do

            if ln.Contains "Util.range" then
                let p, t, n = String.splitTwice "(" ")" ln

                printWithHighlight "Util.range" ln

                let  a = p.Replace("in Util.range", "= 0 to ")
                let b =  t + " - 1"
                let n = a + b + n
                printWithHighlight "= 0 to " n
                printfn ""
                yield n

            else
                yield ln
                ()
        //yield ""
        }


    //|>! Seq.iter (printfn "%s")
    |> fun xs -> IO.File.WriteAllLines(path, xs)





let files = IO.getAllFilesByPattern dir "*.fs"

for file in files |> Seq.truncate 99 do
    doFixAttr file

printfn "*Done!"
