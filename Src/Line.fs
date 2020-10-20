namespace Rhino.Scripting

open FsEx
open System

open Rhino
open Rhino.Geometry
open FsEx.Util
open FsEx.UtilMath

open System.Collections.Generic
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore 

/// This module provides curried functions to manipulate Rhino Line structs
/// It is NOT automatically opened.
module Line =
    
    /// Reverse or flip  the Line (same as Line.flip)
    let reverse (ln:Line) = Line(ln.To,ln.From)
    
    /// Reverse or flip  the Line (same as Line.reverse)
    let flip (ln:Line) = Line(ln.To,ln.From)
    
    /// Get point at center of line
    let mid (ln:Line) = (ln.To + ln.From)* 0.5

    /// Move Line (same as Line.translate)
    let move (v:Vector3d) (ln:Line) = Line(ln.From + v, ln.To + v)

    /// Move Line (same as Line.move)
    let translate (v:Vector3d) (ln:Line) = Line(ln.From + v, ln.To + v)

    /// Finds intersection of two infinite lines.
    /// Fails if lines are paralell or skew by more than 1e-6 units
    /// Considers Lines infinte
    /// Returns point on lnB
    let intersectInOnePoint (lnA:Line) (lnB:Line):Point3d = 
        let ok, ta, tb = Intersect.Intersection.LineLine(lnA,lnB)
        if not ok then RhinoScriptingException.Raise "Rhino.Scripting.Line.intersect failed, paralell ?  on %s and %s" lnA.ToNiceString lnB.ToNiceString
        let a = lnA.PointAt(ta)
        let b = lnB.PointAt(tb)
        if (a-b).SquareLength > RhinoMath.ZeroTolerance then // = Length > 1e-6
            RhinoScriptingException.Raise "Rhino.Scripting.Line.intersect failed, they are skew. distance: %g  on %s and %s" (a-b).Length lnA.ToNiceString lnB.ToNiceString
        b
    
    /// Finds intersection of two infinite lines.
    /// Returns a point for each line where they are the closest to each other.
    /// (in same order as input)
    /// Fails if lines are paralell.
    /// Considers Lines infinte
    let intersectSkew (lnA:Line) (lnB:Line) :Point3d*Point3d= 
        let ok, ta, tb = Intersect.Intersection.LineLine(lnA,lnB)
        if not ok then RhinoScriptingException.Raise "Rhino.Scripting.Line.intersect failed, paralell ?  on %s and %s" lnA.ToNiceString lnB.ToNiceString
        let a = lnA.PointAt(ta)
        let b = lnB.PointAt(tb)        
        a,b
    
    /// Returns the distance between two infinite lines.
    /// At the point where they are closest to each other.
    /// Fails if lines are paralell.
    let distanceToLine (lnA:Line) (lnB:Line) :float= 
        let ok, ta, tb = Intersect.Intersection.LineLine(lnA,lnB)
        if not ok then RhinoScriptingException.Raise "Rhino.Scripting.Line.intersect failed, paralell ?  on %s and %s" lnA.ToNiceString lnB.ToNiceString
        let a = lnA.PointAt(ta)
        let b = lnB.PointAt(tb)
        (a-b).Length
    
    /// Returns the distance between a point and an infinite line.
    let distanceToPoint (pt:Point3d) (ln:Line) :float= 
        let cl = ln.ClosestPoint(pt,false)
        (cl-pt).Length
