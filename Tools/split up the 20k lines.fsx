﻿//#r @"C:\GitHub\FsEx\bin\Release\netstandard2.0\FsEx.dll"
#r "nuget: FsEx, 0.6.4"

open System
open FsEx

let lines = IO.File.ReadAllLines(@"C:\GitHub\Rhino.Scripting\Src\Scripting.fs") 


let opens = """
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpen*&%&* =
  type Scripting with  
    //---The members below are in this file only for developemnt. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't chnage: {@$%^&*()*&^%$@}
"""

let mutable file = "Header"
let lns = Rarr() 
let flush() = 
    printfn "    <Compile Include=\"Src/Scripting_%s.fs\" />" file
    IO.File.WriteAllLines(@"C:\GitHub\Rhino.Scripting\Src\Scripting_"+file+".fs", lns, Text.Encoding.UTF8) 
    lns.Clear()


for ln in lines do 
    if ln.Contains ".. Module: " then 
        flush() 
        let modul = ln |> String.between ".. Module: " "......."  |> String.trim
        lns.Add <| opens.Replace("*&%&*", modul) 
        file <- modul
    
    elif ln.Contains "//............" then  
        () 
    
    else 
        lns.Add(ln) 
        
flush()         

printfn "*Done!"