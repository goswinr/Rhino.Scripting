namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.ActiceDocument
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
 

module ExtrasBrep =
    
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
        use c1 = new NurbsCurve(3,true,3,13)
        for i=0 to 12 do c1.Points.[i] <- ControlPoint( points.[i], weights.[i])
        for i=0 to 13 do c1.Knots.[i] <- knots.[i]
        use c2 = new NurbsCurve(3,true,3,13)
        for i=0 to 12 do c2.Points.[i] <- ControlPoint( Point3d(points.[i].X, points.[i].Y, height), weights.[i])
        for i=0 to 13 do c2.Knots.[i] <- knots.[i]
        Transform.PlaneToPlane (Plane.WorldXY, plane) |> c1.Transform |> ignore
        Transform.PlaneToPlane (Plane.WorldXY, plane) |> c2.Transform |> ignore    
        let rb = Brep.CreateFromLoft( [|c1;c2|], Point3d.Unset, Point3d.Unset, LoftType.Straight, false )
        if isNull rb || rb.Length <> 1  then 
            failwithf "*** Failed to Create loft part of  SlotedHole , at tolerance %f" Doc.ModelAbsoluteTolerance
        rb.[0].CapPlanarHoles(Doc.ModelAbsoluteTolerance)
    
