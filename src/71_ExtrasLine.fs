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
open FsEx.SaveIgnore 


module ExtrasLine =
    
    /// same as flip
    let reverse (ln:Line) = Line(ln.To,ln.From)
    
    /// same as reverse
    let flip (ln:Line) = Line(ln.To,ln.From)
    
    /// Mid point
    let mid (ln:Line) = (ln.To + ln.From)* 0.5

    /// same as translate
    let move (v:Vector3d) (ln:Line) = Line(ln.From + v, ln.To + v)

    /// same as move
    let translate (v:Vector3d) (ln:Line) = Line(ln.From + v, ln.To + v)

    ///Fails if lines are skew or paralell
    ///Considers Lines infinte
    ///Retuns point on lnB
    let intersect (lnA:Line) (lnB:Line) = 
        let ok, ta, tb = Intersect.Intersection.LineLine(lnA,lnB)
        if not ok then failwithf "Line.intersect failed, paralell ?  on %s and %s" lnA.ToNiceString lnB.ToNiceString
        let a = lnA.PointAt(ta)
        let b = lnB.PointAt(tb)
        if (a-b).SquareLength > RhinoMath.ZeroTolerance then // = Length > 1e-6
            failwithf "Line.intersect failed, they are skew. distance: %g  on %s and %s" (a-b).Length lnA.ToNiceString lnB.ToNiceString
        b