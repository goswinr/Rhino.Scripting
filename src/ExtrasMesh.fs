namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.ActiceDocument
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.ExtrasVector
open Rhino.Scripting.ExtrasLine
open FsEx.SaveIgnore


module ExtrasMesh =

    //[<Extension>] //Error 3246
    type Mesh with //TODO or RhinoScriptSyntax ??

        [<Extension>]
        static member join (meshes:Mesh seq) : Mesh = 
            let j = new Mesh()
            j.Append(meshes)
            j