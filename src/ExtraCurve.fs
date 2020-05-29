namespace Rhino.Scripting.Extra

open FsEx
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open System.Collections.Generic
open FsEx.SaveIgnore

[<AutoOpen>]
/// This module provides functions to create or manipulate Rhino Curves
/// This module is automatically opened when Rhino.Scripting namspace is opened.
module ExtrasCurve =

  open Line
  open Vec
   
  type RhinoScriptSyntax with 
    
    [<Extension>]
    ///<summary>Returns the fillet arc if it fits within three points describing two connected lines (= a polyline). Fails otherwise</summary>
    ///<param name="prevPt">(Point3d)The first point of polyline</param>
    ///<param name="midPt">(Point3d)The middle point of polyline, that will get the fillet</param>    
    ///<param name="nextPt">(Point3d)The last (or third) point of polyline</param>
    ///<param name="radius">(float)The radius of the fillet to atempt to creat</param>
    ///<returns>An Arc Geometry</returns>
    static member FilletArc  (prevPt:Point3d, midPt:Point3d, nextPt:Point3d, radius:float)  : Arc   = 
        let A = prevPt-midPt
        let B = nextPt-midPt
        let uA = A |> Vec.unitize 
        let uB = B |> Vec.unitize      
        // calculate trim       
        let alphaDouble = 
            let dot = uA*uB
            if abs(dot) > 0.999  then failwithf "rs.FilletArc: Can't fillet points that are colinear %s,%s,%s" prevPt.ToNiceString midPt.ToNiceString nextPt.ToNiceString
            acos dot
        let alpha = alphaDouble * 0.5
        let beta  = Math.PI * 0.5 - alpha
        let trim = tan(beta) * radius // the setback distance from intersection  
        if trim > A.Length then failwithf "rs.FilletArc: Fillet Radius %g is too big for prev %s and  %s" radius prevPt.ToNiceString midPt.ToNiceString
        if trim > B.Length then failwithf "rs.FilletArc: Fillet Radius %g is too big for next %s and  %s" radius nextPt.ToNiceString midPt.ToNiceString    
        let arcStart =  midPt + uA * trim // still on arc plane
        let arcEnd =    midPt + uB * trim
        Arc(arcStart, - uA , arcEnd)
    
    [<Extension>]
    ///<summary>Fillest some corners of polyline</summary>
    ///<param name="fillets">(int*float ResizeArray)The index of the cornes to filet and the fillet radius</param>
    ///<param name="polyline">(Point3d ResizeArray) The polyline as pointlist </param> 
    ///<returns>a PolyCurve object</returns>
    static member FilletPolyline (fillets: IDictionary<int,float>, polyline:IList<Point3d>): PolyCurve =            
        for i in fillets.Keys do 
            if i >= polyline.LastIndex then failwithf "rs.FilletPolyline: cannot fillet corner %d . in polyline of %d points" i polyline.Count                
        
        let closed = RhinoScriptSyntax.Distance(polyline.[0], polyline.Last) < Doc.ModelAbsoluteTolerance 
        let mutable prevPt = polyline.[0]
        let mutable endPt = polyline.Last
        let plc = new PolyCurve()        
        if fillets.ContainsKey 0 then
            if closed then 
                let arc = RhinoScriptSyntax.FilletArc (polyline.Last, polyline.[0], polyline.[1], fillets.[0])
                plc.Append arc  |> ignore 
                prevPt <- arc.EndPoint
                endPt <- arc.StartPoint
                if fillets.ContainsKey polyline.LastIndex then failwithf "rs.FilletPolyline:Cannot set last and first radius on closed polyline fillet"
            else
                failwithf "rs.FilletPolyline: Cannot set radius at index 0 on open polyline"
        
        for i = 1 to polyline.Count - 2 do  
            let pt = polyline.[i]
            if fillets.ContainsKey i then 
                let ptn = polyline.[i+1]
                let arc = RhinoScriptSyntax.FilletArc (prevPt, pt, ptn, fillets.[i])
                plc.Append (Line (prevPt,arc.StartPoint)) |> ignore 
                plc.Append arc |> ignore 
                prevPt <- arc.EndPoint
            else
                plc.Append (Line (prevPt,pt)) |> ignore 
                prevPt <- pt

        plc.Append(Line(prevPt,endPt))  |> ignore 
        plc

            


    [<Extension>]
    ///<summary>Returns the needed trimming of two planar surfaces in order to fit a fillet of given radius.
    ///    the Lines can be anywhere on plane ( except paralel to axis)</summary>
    ///<param name="makeSCurve">(bool)only relevant if curves are skew: make S-curve if true or kink if false</param>
    ///<param name="radius">(float) radius of filleting zylinder</param>
    ///<param name="direction">(float) direction of filleting zylinder usually the intersection of the two  planes to fillet, this might be the cross profuct of the two lines, but the lines might also be skew </param>
    ///<param name="lineA">(Line) First line to fillet, must not be prependicular to direction, the lines might also be skew  </param> 
    ///<param name="lineB">(Line) Second line to fillet, must not be prependicular to direction or first line, the lines might also be skew  </param> 
    ///<returns>The needed trimming of two planar surfaces in order to fit a fillet of given radius.
    ///    the Lines can be anywhere on plane ( except paralel to axis)</returns>
    static member filletSkewLinesTrims (radius:float) (direction:Vector3d) (lineA:Line) (lineB:Line): float  =         
        let ok,axis = 
            let pla = Plane(lineA.From, lineA.Direction, direction)
            let plb = Plane(lineB.From, lineB.Direction, direction)            
            Intersect.Intersection.PlanePlane(pla,plb)
        if not ok then failwithf "rs.filletSkewLinesTrims: Can't intersect Planes , are lineA and lineB  paralell?"


        let arcPl = Plane(axis.From,axis.Direction)
        let uA = (lineA.Mid - arcPl.Origin) |> Vec.projectToPlane arcPl |> unitize // vector of line A projected in arc plane 
        let uB = (lineB.Mid - arcPl.Origin) |> Vec.projectToPlane arcPl |> unitize // vector of line B projected in arc plane  

        // calculate trim       
        let alphaDouble = 
            let dot = uA*uB
            if abs(dot) > 0.999  then failwithf "rs.filletSkewLinesTrims: Can't fillet, lineA and lineB and direction vector are in same plane."
            acos dot
        let alpha = alphaDouble * 0.5
        let beta  = Math.PI * 0.5 - alpha
        tan(beta) * radius // the setback distance from intersection   
    
    [<Extension>]
    ///<summary>Creates a fillet curve between two lines, 
    ///    the fillet might be an ellipse or free form 
    ///    but it always lies on the surface of a cylinder with the given direction and radius </summary>
    ///<param name="makeSCurve">(bool)only relevant if curves are skew: make S-curve if true or kink if false</param>
    ///<param name="radius">(float) radius of filleting zylinder</param>
    ///<param name="direction">(float) direction of filleting zylinder usually the intersection of the two  planes to fillet, this might be the cross profuct of the two lines, but the lines might also be skew </param>
    ///<param name="lineA">(Line) First line to fillet, must not be prependicular to direction, the lines might also be skew  </param> 
    ///<param name="lineB">(Line) Second line to fillet, must not be prependicular to direction or first line, the lines might also be skew  </param> 
    ///<returns>(NurbsCurve)Fillet curve Geometry, 
    ///    the true fillet arc on cylinder(wrong ends), 
    ///    the point where fillet would be at radius 0, (same plane as arc) </returns>
    static member filletSkewLines makeSCurve (radius:float)  (direction:Vector3d) (lineA:Line) (lineB:Line): NurbsCurve*Arc*Point3d   = 
        let ok,axis = 
            let pla = Plane(lineA.From, lineA.Direction, direction)
            let plb = Plane(lineB.From, lineB.Direction, direction)            
            Intersect.Intersection.PlanePlane(pla,plb)
        if not ok then failwithf "rs.filletSkewLines: Can't intersect Planes , are lineA and lineB  paralell?"
    
    
        let arcPl = Plane(axis.From,axis.Direction)
        let uA = (lineA.Mid - arcPl.Origin) |> projectToPlane arcPl |> unitize // vector of line A projected in arc plane 
        let uB = (lineB.Mid - arcPl.Origin) |> projectToPlane arcPl |> unitize // vector of line B projected in arc plane  
    
        // calculate trim       
        let alphaDouble = 
            let dot = uA*uB
            if abs(dot) > 0.999  then failwithf "rs.filletSkewLines: Can't fillet, lineA and lineB and direction vector are in same plane."
            acos dot
        let alpha = alphaDouble * 0.5
        let beta  = Math.PI * 0.5 - alpha
        let trim = tan(beta) * radius // the setback distance from intersection         
    
        let arcStart0 =  arcPl.Origin + uA * trim // still on arc plane
        let arcEnd0 =    arcPl.Origin + uB * trim
        let arcStart =  arcStart0 |> projectToLine lineA direction |> Pnt.snapIfClose lineA.From |> Pnt.snapIfClose lineA.To
        let arcEnd   =  arcEnd0   |> projectToLine lineB direction |> Pnt.snapIfClose lineB.From |> Pnt.snapIfClose lineB.To
        let arc = Arc(arcStart0, - uA , arcEnd0)
    
        if alphaDouble > Math.PI * 0.49999 && not makeSCurve then // fillet bigger than 89.999 degrees, one arc from 3 points
            let miA = intersectInOnePoint lineA axis
            let miB = intersectInOnePoint lineB axis
            let miPt  = (miA + miB) * 0.5 // if lines are skew
            let midWei = sin alpha
            let knots=    [| 0. ; 0. ; 1. ; 1.|]
            let weights = [| 1. ; midWei; 1.|]             
            let pts =     [| arcStart; miPt ; arcEnd |]
            RhinoScriptSyntax.CreateNurbsCurve(pts, knots, 2, weights), arc, arcPl.Origin
    
        else // fillet smaller than 89.999 degrees, two arc from 5 points
            let betaH = beta*0.5
            let trim2 = trim - radius * tan(betaH)
            let ma, mb = 
                if makeSCurve then
                    arcPl.Origin + uA * trim2 |> projectToLine lineA direction ,
                    arcPl.Origin + uB * trim2 |> projectToLine lineB direction 
                else
                    let miA = intersectInOnePoint lineA axis
                    let miB = intersectInOnePoint lineB axis
                    let miPt  = (miA + miB) * 0.5 // if lines are skew
                    arcPl.Origin + uA * trim2 |> projectToLine (Line(miPt,arcStart)) direction ,
                    arcPl.Origin + uB * trim2 |> projectToLine (Line(miPt,arcEnd  )) direction              
        
            let gamma = Math.PI*0.5 - betaH
            let midw= sin(gamma)
            let knots= [| 0. ; 0. ; 1. ; 1. ; 2. ; 2.|]
            let weights = [|1. ; midw ; 1. ; midw ; 1.|]             
            let mid = (ma + mb)*0.5
            let pts = [|arcStart; ma; mid; mb; arcEnd|]
            RhinoScriptSyntax.CreateNurbsCurve(pts, knots, 2, weights),arc,arcPl.Origin
   
   
   

               
