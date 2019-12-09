namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.Util
open FsEx.UtilMath
open Rhino.Scripting.ActiceDocument
open System.Collections.Generic
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
 


module ExtrasLine =
    
    // same as flip
    let reverse (ln:Line) = Line(ln.To,ln.From)
    
    // same as reverse
    let flip (ln:Line) = Line(ln.To,ln.From)
