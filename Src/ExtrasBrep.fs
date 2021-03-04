namespace Rhino.Scripting.Extra

open FsEx
open System
open Rhino.Geometry
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting
open FsEx.SaveIgnore

[<AutoOpen>]
/// This module provides functions to create or manipulate Rhino Breps/ Polysurface 
/// This module is automatically opened when Rhino.Scripting.Extra namspace is opened.
module ExtrasBrep = 
   
  type RhinoScriptSyntax with // TODO chnage to Brep type extensions ??!!
   
    ///<summary>Creates a Brep in the Shape of a Sloted Hole. Closed with caps. </summary>
    ///<param name="plane">(Plane)Origin = center of hole</param>
    ///<param name="length">(float) total length of sloted hole</param>
    ///<param name="width">(float) width = radius of sloted hole</param>
    ///<param name="height">(float) height of sloted hole volume</param> 
    ///<returns>(Brep) Closed Brep Geometry.</returns>
    [<Extension>]
    static member CreateSlotedHoleVolume( plane:Plane, length, width, height):Brep  =
        if length<width then RhinoScriptingException.Raise "RhinoScriptSyntax.SlotedHole: length= %g must be more than width= %g" length width
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
            RhinoScriptingException.Raise "RhinoScriptSyntax.*** Failed to Create loft part of  SlotedHole , at tolerance %f" Doc.ModelAbsoluteTolerance
        rb.[0].CapPlanarHoles(Doc.ModelAbsoluteTolerance)
        


    ///<summary>Creates a solid Brep in the Shape of a  cylinder. Closed with caps. </summary>
    ///<param name="plane">(Plane) Origin is center of base of cylinder</param>
    ///<param name="diameter">(float) Diameter of cylinder</param>
    ///<param name="length">(float) total length of the screw brep</param>
    ///<returns>(Brep) Brep Geometry.</returns>
    [<Extension>]
    static member CreateCylinder ( plane:Plane, diameter, length):Brep  =            
        let circ = Circle(plane,diameter*0.5)
        let cy = Cylinder(circ,length)
        Brep.CreateFromCylinder(cy, capBottom=true, capTop=true)

    ///<summary>Creates a Brep in the Shape of a Countersunk Screw Hole , 45 degrees slope
    ///    a caped cone and a cylinder. one closed polysurface </summary>
    ///<param name="plane">(Plane) Origin is center of conebase or head</param>
    ///<param name="outerDiameter">(float) diameter of cone base</param>
    ///<param name="innerDiameter">(float) Diameter of cylinder</param>
    ///<param name="length">(float) total length of the screw brep</param>
    ///<returns>(Brep) Brep Geometry.</returns>
    [<Extension>]
    static member CreateCounterSunkScrewVolume ( plane:Plane, outerDiameter, innerDiameter, length):Brep  =
        let r = outerDiameter*0.5
        let mutable plco = Plane(plane)
        plco.Origin <- plco.Origin + plco.ZAxis * r
        plco.Flip()
        let cone = Cone(plco, r, r)
        let coneSrf = Brep.CreateFromCone(cone, capBottom=true)
        plane.Rotate(Math.PI * 0.5, plane.ZAxis)|> RhinoScriptingException.FailIfFalse "rotate plane" // so that seam of cone an cylinder align
        let cySrf = RhinoScriptSyntax.CreateCylinder(plane, innerDiameter, length)
        let bs = Brep.CreateBooleanUnion( [coneSrf; cySrf], Doc.ModelAbsoluteTolerance)
        if bs.Length <> 1 then RhinoScriptingException.Raise "RhinoScriptSyntax.%d items as result from creating countersunc screw" bs.Length
        let brep = bs.[0]
        if brep.SolidOrientation = BrepSolidOrientation.Inward then brep.Flip()
        brep

    ///<summary>if brep.SolidOrientation is inward then flip brep .</summary>
    static member OrientBrep (brep:Brep):Brep  =
        if brep.SolidOrientation = BrepSolidOrientation.Inward then  
            brep.Flip()
        brep

    static member CreateExrusionAtPlane(curveToExtrudeInWorldXY:Curve, plane:Plane, height, [<OPT;DEF(0.0)>]extraHeightPerSide:float): Brep =
        let mutable pl = Plane(plane)
        if extraHeightPerSide <> 0.0 then 
            pl.Origin <- pl.Origin - pl.ZAxis*extraHeightPerSide
        let xForm = RhinoScriptSyntax.XformRotation1(Plane.WorldXY,pl)
        let c = curveToExtrudeInWorldXY.DuplicateCurve()
        c.Transform(xForm) |> RhinoScriptingException.FailIfFalse "xForm in CreateExrusionAtPlane"
        let h = extraHeightPerSide + height
        let brep = Surface.CreateExtrusion(c, pl.ZAxis * h )
                        .ToBrep()
                        .CapPlanarHoles(Doc.ModelAbsoluteTolerance)
        if brep.SolidOrientation = BrepSolidOrientation.Inward then brep.Flip()
        brep
            

    ///<summary>Subtracts trimmer from brep (= BooleanDifference), 
    /// so that a single brep is returned, 
    /// draws objects and zooms on them if an error occures.</summary>
    ///<param name="trimmer">(Brep)the volume to cut out</param>
    ///<param name="keep">(Brep) The volume to keep</param>
    ///<param name="subtractionLocations">(int) Optional, The amount of locations where the brep is expected to be cut
    ///  This is an optional safety check that makes it twice as slow. 
    ///  It ensures that the count of breps from  Brep.CreateBooleanIntersection is equal to subtractionLocations </param>
    ///<returns>(Brep) Brep Geometry.</returns>
    [<Extension>]
    static member SubstractBrep (keep:Brep,trimmer:Brep,[<OPT;DEF(0)>]subtractionLocations:int)  :Brep =
        if not trimmer.IsSolid then
            RhinoScriptSyntax.draw "debug trimmer" trimmer
            RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
            RhinoScriptingException.Raise "RhinoScriptSyntax.SubstractBrep:CreateBooleanDifference trimmer is NOT a closed polysurface" 
        if not keep.IsSolid then
            RhinoScriptSyntax.draw "debug keep" keep
            RhinoScriptSyntax.ZoomBoundingBox(keep.GetBoundingBox(false))
            RhinoScriptingException.Raise "RhinoScriptSyntax.SubstractBrep:CreateBooleanDifference keep Volume is NOT a closed polysurface" 
            
        if subtractionLocations <> 0 then 
            let xs = Brep.CreateBooleanIntersection (keep,trimmer,Doc.ModelAbsoluteTolerance) // TODO expensive extra check
            if isNull xs then
                RhinoScriptSyntax.draw "debug trimmer no Intersection" trimmer
                RhinoScriptSyntax.draw "debug keep no Intersection" keep
                RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
                RhinoScriptingException.Raise "RhinoScriptSyntax.SubstractBrep:CreateBooleanIntersection check isnull, no intersection found, tolerance = %g" Doc.ModelAbsoluteTolerance
            if xs.Length <> subtractionLocations then
                RhinoScriptSyntax.draw "debug trimer empty Intersection" trimmer
                RhinoScriptSyntax.draw "debug keep empty Intersection" keep
                RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
                RhinoScriptingException.Raise "RhinoScriptSyntax.SubstractBrep:CreateBooleanIntersection check returned %d breps instead of one , tolerance = %g" xs.Length Doc.ModelAbsoluteTolerance
            for x in xs do x.Dispose()

        let bs =  Brep.CreateBooleanDifference(keep,trimmer,Doc.ModelAbsoluteTolerance)
        if isNull bs then
            RhinoScriptSyntax.draw "debug trimmer" trimmer
            RhinoScriptSyntax.draw "debug keep" keep
            RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
            RhinoScriptingException.Raise "RhinoScriptSyntax.SubstractBrep:CreateBooleanDifference is null, tolerance = %g" Doc.ModelAbsoluteTolerance
        if bs.Length = 0 then
            RhinoScriptSyntax.draw "debug trimer for empty result" trimmer
            RhinoScriptSyntax.draw "debug keep for empty result" keep
            RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
            RhinoScriptingException.Raise "RhinoScriptSyntax.SubstractBrep:CreateBooleanDifference returned 0 breps instead of one , tolerance = %g" Doc.ModelAbsoluteTolerance
        if bs.Length <> 1 then 
            bs |> Seq.iter (RhinoScriptSyntax.draw "debug more than one")
            RhinoScriptSyntax.draw "debug trimer for more than one" trimmer
            RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
            RhinoScriptingException.Raise "RhinoScriptSyntax.SubstractBrep:CreateBooleanDifference returned %d breps instead of one , tolerance = %g" bs.Length Doc.ModelAbsoluteTolerance            
        let brep = bs.[0]
        if subtractionLocations = 0 && brep.Vertices.Count = keep.Vertices.Count then // extra test if 
            RhinoScriptSyntax.draw "debug trimmer same vertex count on  result" trimmer
            RhinoScriptSyntax.draw "debug keep same vertex count on  result" keep
            RhinoScriptSyntax.ZoomBoundingBox(trimmer.GetBoundingBox(false))
            RhinoScriptingException.Raise "RhinoScriptSyntax.SubstractBrep:CreateBooleanDifference returned same vertex count on input and output brep is this desired ?, tolerance = %g" Doc.ModelAbsoluteTolerance
        if brep.SolidOrientation = BrepSolidOrientation.Inward then  brep.Flip()
        brep
        
    ///<summary> Calls Mesh.CreateFromBrep, and Mesh.HealNakedEdges() to try to ensure Mesh is closed if input is closed.</summary>
    ///<param name="brep">(Brep)the Polysurface to extract Mesh from</param>
    ///<param name="meshingParameters">(MeshingParameters) Optional, The Meshing parameters , if omitted the current Meshing parameters are used </param>
    ///<returns>((Mesh Result) Ok Mesh Geometry or Error Mesh if input brep is closed but output Mesh not
    /// fails if no Meshes can be extracted .</returns>
    [<Extension>]
    static member ExtractRenderMesh (brep:Brep,[<OPT;DEF(null:MeshingParameters)>]meshingParameters:MeshingParameters) :Result<Mesh,Mesh> =            
        let meshing =                
            if notNull meshingParameters then 
                meshingParameters
            else
                Doc.GetCurrentMeshingParameters()
        meshing.ClosedObjectPostProcess <- true // not needed use heal instead
        let ms = Mesh.CreateFromBrep(brep,meshing)
        let m = new Mesh()
        for p in ms do 
            if notNull p then 
                m.Append p 
        if m.Vertices.Count < 3 then 
            RhinoScriptingException.Raise "RhinoScriptSyntax.ExtractRenderMesh: failed to extract a mesh from brep: %A of %d Faces" brep brep.Faces.Count
        
        //let g = ref Guid.Empty
        if brep.IsSolid && not m.IsClosed then // https://discourse.mcneel.com/t/failed-to-create-closed-mesh-with-mesh-createfrombrep-brep-meshing-params-while-sucessfull-with-rhino-command--mesh/35481/8
            m.HealNakedEdges(Doc.ModelAbsoluteTolerance * 100.0) |> ignore // see https://discourse.mcneel.com/t/mesh-createfrombrep-fails/93926
            if not m.IsClosed then 
                m.HealNakedEdges(Doc.ModelAbsoluteTolerance * 1000.0 + meshing.MinimumEdgeLength * 100.0) |> ignore             
        if  not m.IsValid then
            //Doc.Objects.AddBrep brep|> RhinoScriptSyntax.setLayer "rs.ExtractRenderMesh mesh from Brep invalid"                    
            RhinoScriptingException.Raise "RhinoScriptSyntax.ExtractRenderMesh: failed to create valid mesh from brep"
        elif brep.IsSolid && not m.IsClosed then 
            Result.Error m
            //Doc.Objects.AddMesh m |> RhinoScriptSyntax.setLayer "RhinoScriptSyntax.ExtractRenderMesh not closed"
            //printf "Mesh from closed Brep is not closed, see debug layer"
            //if  m0.IsValid && m0.IsClosed && ( g := Doc.Objects.AddMesh m0 ; !g <> Guid.Empty) then 
            //    Ok !g 
            //else                        //if it fails it uses ExtractRenderMesh command and returns both mesh and temporay created brep Guid</
            //    let mb = brep |> Doc.Objects.AddBrep 
            //    RhinoScriptSyntax.EnableRedraw(true)
            //    Doc.Views.Redraw()
            //    RhinoScriptSyntax.SelectObject(mb)
            //    RhinoScriptSyntax.Command("ExtractRenderMesh ") |> RhinoScriptingException.FailIfFalse "mesh render"
            //    let ms = RhinoScriptSyntax.LastCreatedObjects()
            //    if ms.Count <> 1 then RhinoScriptingException.Raise "getRenderMesh: %d in LastCreatedObjects" ms.Count 
            //    RhinoScriptSyntax.EnableRedraw(false)
            //    let k = RhinoScriptSyntax.UnselectAllObjects()
            //    if k <> 1 then RhinoScriptingException.Raise "getRenderMesh: %d Unselected" k
            //    Error (ms.[0],mb) 
        else
            Result.Ok m
           
