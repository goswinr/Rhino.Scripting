#r "C:/Program Files/Rhino 8/System/RhinoCommon.dll"
#r "C:/Program Files/Rhino 8/System/Eto.dll"
#r @"D:\Git\_Rhino.Scripting_\Rhino.Scripting\bin\Release\net48\FsEx"
#r @"D:\Git\_Rhino.Scripting_\Rhino.Scripting\bin\Release\net48\Rhino.Scripting.dll"

open System
// open FsEx
open Eto

type rs =  Rhino.Scripting.RhinoScriptSyntax

printfn $"{typeof<rs>.Assembly}"
printfn $"{typeof<Rhino.UI.RhinoHelp>.Assembly}"
printfn $"{typeof<FsEx.Rarr<int>>.Assembly}"


rs.ClipboardText("Hi from rs")


//let qn = Eto.Platform.Instance.GetType().AssemblyQualifiedName
//print qn
//let platform = Eto.Platform.Get(qn)

//do 
    //let pf = Eto.Platform.Detect
    //use x = pf.Context
    //print<|  pf.GetType().AssemblyQualifiedName


rs.ClipboardText()|> print