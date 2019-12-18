﻿namespace Rhino.Scripting

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
        static member CreateSlotedHoleVolume( plane:Plane, length, width, height):Brep  =
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
        


        [<Extension>]
        ///<summary>Creates a solid Brep in the Shape of a  cylinder</summary>
        ///<param name="plane">(Plane) Origin is center of base of cylinder</param>
        ///<param name="diameter">(float) Diameter of cylinder</param>
        ///<param name="length">(float) total length of the screw brep</param>
        ///<returns>(Brep) Brep Geometry</returns>
        static member CreateCylinder ( plane:Plane, diameter, length):Brep  =            
            let circ = Circle(plane,diameter*0.5)
            let cy = Cylinder(circ,length)
            Brep.CreateFromCylinder(cy, capBottom=true, capTop=true)

        [<Extension>]
        ///<summary>Creates a Brep in the Shape of a Countersunk Screw Hole , 45 degrees slope
        ///a caped cone and a cylinder</summary>
        ///<param name="plane">(Plane) Origin is center of conebase or head</param>
        ///<param name="outerDiameter">(float) diameter of cone base</param>
        ///<param name="innerDiameter">(float) Diameter of cylinder</param>
        ///<param name="length">(float) total length of the screw brep</param>
        ///<returns>(Brep) Brep Geometry</returns>
        static member CreateCounterSunkScrewVolume ( plane:Plane, outerDiameter, innerDiameter, length):Brep  =
            let r = outerDiameter*0.5
            let mutable plco = Plane(plane)
            plco.Origin <- plco.Origin + plco.ZAxis * r
            plco.Flip()
            let cone = Cone(plco, r, r)
            let coneSrf = Brep.CreateFromCone(cone, capBottom=true)
            plane.Rotate(Math.PI * 0.5, plane.ZAxis)|> failIfFalse "rotate plane" // so that seam of cone an cylinder align
            let cySrf = RhinoScriptSyntax.CreateCylinder(plane, innerDiameter, length)
            let bs = Brep.CreateBooleanUnion( [coneSrf; cySrf], Doc.ModelAbsoluteTolerance)
            if bs.Length <> 1 then failwithf "%d items as result from creating countersunc screw" bs.Length
            let brep = bs.[0]
            if brep.SolidOrientation = BrepSolidOrientation.Inward then brep.Flip()
            brep

        ///<summary>if brep.SolidOrientation is inward then flip brep </summary>
        static member OrientBrep (brep:Brep):Brep  =
            if brep.SolidOrientation = BrepSolidOrientation.Inward then  
                brep.Flip()
            brep

        static member CreateExrusionAtPlane(curveToExtrudeInWorldXY:Curve, plane:Plane, height, [<OPT;DEF(0.0)>]extraHeightPerSide:float): Brep =
            let mutable pl = Plane(plane)
            if extraHeightPerSide <> 0.0 then 
                pl.Origin <- pl.Origin - pl.ZAxis*extraHeightPerSide
            let xform = RhinoScriptSyntax.XformRotation1(Plane.WorldXY,pl)
            let c = curveToExtrudeInWorldXY.DuplicateCurve()
            c.Transform(xform) |> failIfFalse "xform in CreateExrusionAtPlane"
            let h = extraHeightPerSide + height
            let brep = Surface.CreateExtrusion(c, pl.ZAxis * h )
                            .ToBrep()
                            .CapPlanarHoles(Doc.ModelAbsoluteTolerance)
            if brep.SolidOrientation = BrepSolidOrientation.Inward then brep.Flip()
            brep
            

        [<Extension>]
        ///<summary>Subtracts trimmer from brep (= BooleanDifference), 
        /// so that a single brep is returned, 
        /// draws objects and zooms on them if an error occures</summary>
        ///<param name="trimmer">(Brep)the volume to cut out</param>
        ///<param name="keep">(Brep) the volume to keep</param>
        ///<param name="subtractionLocations">(int) Optional, The amount of locations where the brep is expected to be cut
        /// This is an optional safety check that makes it twice as slow. 
        //  It ensures that the count of breps from  Brep.CreateBooleanIntersection is equal to subtractionLocations </param>
        ///<returns>(Brep) Brep Geometry</returns>
        static member substractBrep (keep:Brep,trimmer:Brep,[<OPT;DEF(0)>]subtractionLocations:int)  :Brep =
            if not trimmer.IsSolid then
                RhinoScriptSyntax.draw "debug trimmer" trimmer
                RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
                failwithf "substractBrep:CreateBooleanDifference trimmer is NOT a closed polysurface" 
            if not keep.IsSolid then
                RhinoScriptSyntax.draw "debug keep" keep
                RhinoScriptSyntax.ZoomBoundingBox(keep.GetBoundingBox(false))
                failwithf "substractBrep:CreateBooleanDifference keep Volume is NOT a closed polysurface" 
            
            if subtractionLocations <> 0 then 
                let xs = Brep.CreateBooleanIntersection (keep,trimmer,Doc.ModelAbsoluteTolerance) // TODO expensive extra check
                if isNull xs then
                    RhinoScriptSyntax.draw "debug trimmer no Intersection" trimmer
                    RhinoScriptSyntax.draw "debug keep no Intersection" keep
                    RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
                    failwithf "substractBrep:CreateBooleanIntersection check isnull, no intersection found, tolerance = %g" Doc.ModelAbsoluteTolerance
                if xs.Length <> subtractionLocations then
                    RhinoScriptSyntax.draw "debug trimer empty Intersection" trimmer
                    RhinoScriptSyntax.draw "debug keep empty Intersection" keep
                    RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
                    failwithf "substractBrep:CreateBooleanIntersection check returned %d breps instead of one , tolerance = %g" xs.Length Doc.ModelAbsoluteTolerance
                for x in xs do x.Dispose()

            let bs =  Brep.CreateBooleanDifference(keep,trimmer,Doc.ModelAbsoluteTolerance)
            if isNull bs then
                RhinoScriptSyntax.draw "debug trimmer" trimmer
                RhinoScriptSyntax.draw "debug keep" keep
                RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
                failwithf "substractBrep:CreateBooleanDifference is null, tolerance = %g" Doc.ModelAbsoluteTolerance
            if bs.Length = 0 then
                RhinoScriptSyntax.draw "debug trimer for empty result" trimmer
                RhinoScriptSyntax.draw "debug keep for empty result" keep
                RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
                failwithf "substractBrep:CreateBooleanDifference returned 0 breps instead of one , tolerance = %g" Doc.ModelAbsoluteTolerance
            if bs.Length <> 1 then 
                bs |> Seq.iter (RhinoScriptSyntax.draw "debug more than one")
                RhinoScriptSyntax.draw "debug trimer for more than one" trimmer
                RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
                failwithf "substractBrep:CreateBooleanDifference returned %d breps instead of one , tolerance = %g" bs.Length Doc.ModelAbsoluteTolerance            
            let brep = bs.[0]
            if subtractionLocations = 0 && brep.Vertices.Count = keep.Vertices.Count then // extra test if 
                RhinoScriptSyntax.draw "debug trimmer same vertex count on  result" trimmer
                RhinoScriptSyntax.draw "debug keep same vertex count on  result" keep
                RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
                failwithf "substractBrep:CreateBooleanDifference returned same vertex count on input and output brep is this desired ?, tolerance = %g" Doc.ModelAbsoluteTolerance
            if brep.SolidOrientation = BrepSolidOrientation.Inward then  brep.Flip()
            brep
      
