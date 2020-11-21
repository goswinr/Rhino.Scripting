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
        if not ok then RhinoScriptingException.Raise "Rhino.Scripting.Line.intersectInOnePoint failed, paralell ?  on %s and %s" lnA.ToNiceString lnB.ToNiceString
        let a = lnA.PointAt(ta)
        let b = lnB.PointAt(tb)
        if (a-b).SquareLength > RhinoMath.ZeroTolerance then // = Length > 1e-6
            RhinoScriptingException.Raise "Rhino.Scripting.Line.intersect intersectInOnePoint, they are skew. distance: %g  on %s and %s" (a-b).Length lnA.ToNiceString lnB.ToNiceString
        b
    
    /// Finds intersection of two infinite lines.
    /// Returns a point for each line where they are the closest to each other.
    /// (in same order as input)
    /// Fails if lines are paralell.
    /// Considers Lines infinte
    let intersectSkew (lnA:Line) (lnB:Line) :Point3d*Point3d= 
        let ok, ta, tb = Intersect.Intersection.LineLine(lnA,lnB)
        if not ok then RhinoScriptingException.Raise "Rhino.Scripting.Line.intersectSkew failed, paralell ?  on %s and %s" lnA.ToNiceString lnB.ToNiceString
        let a = lnA.PointAt(ta)
        let b = lnB.PointAt(tb)        
        a,b
    

    /// Checks if two finite lines intersect.
    let doIntersectFinite (lnA:Line) (lnB:Line) : bool= 
        let ok, ta, tb = Intersect.Intersection.LineLine(lnA,lnB)
        if ok then 
            0.0 <= ta && ta <= 1.0 && 0.0 <= tb && tb <= 1.0 
        else
            false


    /// Finds intersection of two finite lines.
    /// Returns: 
    ///    an enpty array if they are paralell,
    ///    an array with one point if they intersect by Doc.ModelAbsoluteTolerance (point will be the average of the two points within the tolerance)
    ///    an array with two points where they are the closest to each other. (in same order as input)
    /// Fails if lines are paralell.
    /// Considers Lines finte
    let intersectFinite (lnA:Line) (lnB:Line) : Point3d[]= 
        let ok, ta, tb = Intersect.Intersection.LineLine(lnA,lnB)
        if not ok then [||] //RhinoScriptingException.Raise "Rhino.Scripting.Line.intersectFinite failed, paralell ?  on %s and %s" lnA.ToNiceString lnB.ToNiceString
        else
            let ca = clamp 0. 1. ta
            let cb = clamp 0. 1. tb
            let a = lnA.PointAt(ca)
            let b = lnB.PointAt(cb) 
            let d = Pnt.distance a b
            if  d < Doc.ModelAbsoluteTolerance * 0.5 then 
                if d < RhinoMath.ZeroTolerance then [|a|]
                else [| Pnt.divPt a b 0.5|]
            else [|a ; b|]

    /// Returns the distance between two infinite lines.
    /// At the point where they are closest to each other.
    /// works even if lines are paralell.
    let distanceToLine (lnA:Line) (lnB:Line) :float= 
        let ok, ta, tb = Intersect.Intersection.LineLine(lnA,lnB)
        if not ok then // paralell
            //RhinoScriptingException.Raise "Rhino.Scripting.Line.intersect failed, paralell ?  on %s and %s" lnA.ToNiceString lnB.ToNiceString
            let pt = lnA.ClosestPoint(lnB.From,false)
            (pt-lnB.From).Length
        else
            let a = lnA.PointAt(ta)
            let b = lnB.PointAt(tb)
            (a-b).Length
    
    /// Returns the distance between a point and an infinite line.
    let distanceToPoint (pt:Point3d) (ln:Line) :float= 
        let cl = ln.ClosestPoint(pt,false)
        (cl-pt).Length

    /// returns a new transformed line
    let transform(xForm:Transform) (line:Line) =
        let ln = Line(line.From,line.To)
        if ln.Transform(xForm) then 
            ln
        else  
            RhinoScriptingException.Raise "Line.transform failed on line %A with  %A" line xForm
        
