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

module ExtrasBrep = 
 
   
    ///Returns the needed trimming of two planar surfaces in order to fit a fillet of given radius.
    ///the Lines can be anywhere on plane ( except paralel to axis)
    let getTrimForFillet (radius:float) (direction:Vector3d) (lineA:Line) (lineB:Line): float  =         
        let ok,axis = 
           let pla = Plane(lineA.From, lineA.Direction, direction)
           let plb = Plane(lineB.From, lineB.Direction, direction)            
           Intersect.Intersection.PlanePlane(pla,plb)
        if not ok then failwithf "getTrimForFillet: Can't intersect Planes , are lineA and lineB  paralell?"


        let arcPl = Plane(axis.From,axis.Direction)
        let uA = (lineA.Mid - arcPl.Origin) |> projectToPlane arcPl |> unitize // vector of line A projected in arc plane 
        let uB = (lineB.Mid - arcPl.Origin) |> projectToPlane arcPl |> unitize // vector of line B projected in arc plane  

        // calculate trim       
        let alphaDouble = 
           let dot = uA*uB
           if abs(dot) > 0.999  then failwithf "getTrimForFillet: Can't fillet, lineA and lineB and direction vector are in same plane."
           acos dot
        let alpha = alphaDouble * 0.5
        let beta  = Math.PI * 0.5 - alpha
        tan(beta) * radius // the setback distance from intersection   
   
    ///<summary>Creates a fillet curve between two lines, 
    ///the fillet might be an ellipse or free form 
    ///but it always lies on the surface of a cylinder with the given direction and radius </summary>
    ///<param name="makeSCurve">(bool)only relevant if curves are skew: make S-curve if true or kink if false</param>
    ///<param name="radius">(float) radius of filleting zylinder</param>
    ///<param name="direction">(float) direction of filleting zylinder usually the intersection of the two  planes to fillet<e</param>
    ///<param name="lineA">(Line) First line to fillet, must not be parallel to direction </param> 
    ///<param name="lineB">(Line) Second line to fillet, must not be parallel to direction or first line </param> 
    ///<returns>(NurbsCurve)Fillet curve Geometry, 
    ///  the true fillet arc on cylinder(wrong ends), 
    ///  the point where fillet would be at radius 0, (same plane as arc) </returns>
    let freeFillet makeSCurve (radius:float)  (direction:Vector3d) (lineA:Line) (lineB:Line): NurbsCurve*Arc*Point3d   =         
        let ok,axis = 
           let pla = Plane(lineA.From, lineA.Direction, direction)
           let plb = Plane(lineB.From, lineB.Direction, direction)            
           Intersect.Intersection.PlanePlane(pla,plb)
        if not ok then failwithf "freeFillet: Can't intersect Planes , are lineA and lineB  paralell?"
    
    
        let arcPl = Plane(axis.From,axis.Direction)
        let uA = (lineA.Mid - arcPl.Origin) |> projectToPlane arcPl |> unitize // vector of line A projected in arc plane 
        let uB = (lineB.Mid - arcPl.Origin) |> projectToPlane arcPl |> unitize // vector of line B projected in arc plane  
    
        // calculate trim       
        let alphaDouble = 
           let dot = uA*uB
           if abs(dot) > 0.999  then failwithf "freeFillet: Can't fillet, lineA and lineB and direction vector are in same plane."
           acos dot
        let alpha = alphaDouble * 0.5
        let beta  = Math.PI * 0.5 - alpha
        let trim = tan(beta) * radius // the setback distance from intersection         
    
        let arcStart0 =  arcPl.Origin + uA * trim // still on arc plane
        let arcEnd0 =    arcPl.Origin + uB * trim
        let arcStart =  arcStart0 |> projectToLine lineA direction |> snapIfClose lineA.From |> snapIfClose lineA.To
        let arcEnd   =  arcEnd0   |> projectToLine lineB direction |> snapIfClose lineB.From |> snapIfClose lineB.To
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
   
   
   
    //[<Extension>] //Error 3246  
    type RhinoScriptSyntax with
   
        [<Extension>]
        ///<summary>Creates a Brep in the Shape of a Sloted Hole</summary>
        ///<param name="plane">(Plane)Origin = center of hole</param>
        ///<param name="length">(float) total length of sloted hole</param>
        ///<param name="breite">(float) width = radius of sloted hole</param>
        ///<param name="height">(float) height of sloted hole volume</param> 
        ///<returns>(Brep) Brep Geometry</returns>
        static member SlotedHole( plane:Plane, length, width, height):Brep  =
            if length<width then failwithf "SlotedHole: length= %g must be more than width= %g" length width
            let root05  = sqrt 0.5
            let y05 = 0.5 * width
            let x1 =  0.5 * length 
            let x05 = x1 - y05
            let knots = [|
                0.0
                0.0
                2.0
                2.0
                2.785398
                2.785398
                3.570796
                3.570796
                5.570796
                5.570796
                6.356194
                6.356194
                7.141593
                7.141593
                |]
            let weights = [|1.0; 1.0; 1.0; root05; 1.0; root05; 1.0; 1.0; 1.0; root05; 1.0; root05; 1.0 |]
            let points = [|
                Point3d(-x05, -y05, 0.0)
                Point3d(0.0,  -y05, 0.0)
                Point3d(x05,  -y05, 0.0)
                Point3d(x1,   -y05, 0.0)
                Point3d(x1,    0.0, 0.0)
                Point3d(x1,    y05, 0.0)
                Point3d(x05,   y05, 0.0)
                Point3d(0.0,   y05, 0.0)
                Point3d(-x05,  y05, 0.0)
                Point3d(-x1,   y05, 0.0)
                Point3d(-x1,   0.0, 0.0)
                Point3d(-x1,  -y05, 0.0)
                Point3d(-x05, -y05, 0.0)
                |]
            use c1 = new NurbsCurve(3, true, 3, 13)
            for i=0 to 12 do c1.Points.[i] <- ControlPoint( points.[i], weights.[i])
            for i=0 to 13 do c1.Knots.[i] <- knots.[i]
            use c2 = new NurbsCurve(3, true, 3, 13)
            for i=0 to 12 do c2.Points.[i] <- ControlPoint( Point3d(points.[i].X, points.[i].Y, height), weights.[i])
            for i=0 to 13 do c2.Knots.[i] <- knots.[i]
            Transform.PlaneToPlane (Plane.WorldXY, plane) |> c1.Transform |> ignore
            Transform.PlaneToPlane (Plane.WorldXY, plane) |> c2.Transform |> ignore    
            let rb = Brep.CreateFromLoft( [|c1;c2|], Point3d.Unset, Point3d.Unset, LoftType.Straight, false )
            if isNull rb || rb.Length <> 1  then 
                failwithf "*** Failed to Create loft part of  SlotedHole , at tolerance %f" Doc.ModelAbsoluteTolerance
            rb.[0].CapPlanarHoles(Doc.ModelAbsoluteTolerance)
      
