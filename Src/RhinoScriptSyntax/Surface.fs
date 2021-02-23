namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.UtilMath

open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ? 
open System.Collections.Generic
open FsEx.SaveIgnore


[<AutoOpen>]
/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
module ExtensionsSurface =

  //[<Extension>] //Error 3246
    type RhinoScriptSyntax with

    [<Extension>]
    ///<summary>Adds a box shaped Polysurface to the document</summary>
    ///<param name="corners">(Point3d seq) 8 points that define the corners of the box. Points need to
    ///    be in counter-clockwise order starting with the bottom rectangle of the box</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member AddBox(corners:Point3d seq) : Guid =
        //box = RhinoScriptSyntax.Coerce3dpointlist(corners)
        let brep = Brep.CreateFromBox(corners)
        if isNull brep then RhinoScriptingException.Raise "RhinoScriptSyntax.AddBox: Unable to create brep from box.  %d corners:'%A'" (Seq.length corners) corners
        let rc = Doc.Objects.AddBrep(brep)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddBox: Unable to add brep to document. corners:'%A'" corners
        Doc.Views.Redraw()
        rc

    [<Extension>]
    ///<summary>Adds a cone shaped Polysurface to the document</summary>
    ///<param name="basis">(Plane) 3D origin point of the cone or a Plane with an apex at the origin
    ///    and normal along the Plane's z-axis</param>
    ///<param name="height">(float)  height of cone </param>
    ///<param name="radius">(float) The radius at the basis of the cone</param>
    ///<param name="cap">(bool) Optional, Default Value: <c>true</c>
    ///    Cap basis of the cone</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member AddCone( basis:Plane,
                           height:float,
                           radius:float,
                           [<OPT;DEF(true)>]cap:bool) : Guid =
        let cone = Cone(basis, height, radius)
        let brep = Brep.CreateFromCone(cone, cap)// TODO cone is upside down??
        let rc = Doc.Objects.AddBrep(brep)
        Doc.Views.Redraw()
        rc
        //TODO add version with two points


    [<Extension>]
    ///<summary>Adds a planar Surface through objects at a designated location. For more
    ///    information, see the Rhino help file for the CutPlane command</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects that the cutting Plane will
    ///    pass through</param>
    ///<param name="startPoint">(Point3d) Start point of line that defines the cutting Plane</param>
    ///<param name="endPoint">(Point3d) End point of line that defines the cutting Plane</param>
    ///<param name="normal">(Vector3d) Optional, Default Value: <c>world Z axis</c>
    ///    Vector that will be contained in the returned planar
    ///    Surface.
    ///    If omitted, the world Z axis is used</param>
    ///<returns>(Guid) identifier of new object.</returns>
    static member AddCutPlane( objectIds:Guid seq,
                               startPoint:Point3d,
                               endPoint:Point3d,
                               [<OPT;DEF(Vector3d())>]normal:Vector3d) : Guid = //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
        let bbox = BoundingBox.Unset
        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let geometry = rhobj.Geometry
            bbox.Union( geometry.GetBoundingBox(true))
        //startPoint = RhinoScriptSyntax.Coerce3dpoint(startPoint)
        //endPoint = RhinoScriptSyntax.Coerce3dpoint(endPoint)
        if not bbox.IsValid then
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddCutPlane failed.  objectIds:'%A' startPoint:'%A' endPoint:'%A' normal:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) startPoint endPoint normal
        let line = Line(startPoint, endPoint)
        let normal = if normal.IsZero then Vector3d.ZAxis else normal
        let surface = PlaneSurface.CreateThroughBox(line, normal, bbox)
        if surface|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.AddCutPlane failed.  objectIds:'%A' startPoint:'%A' endPoint:'%A' normal:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) startPoint endPoint normal
        let objectId = Doc.Objects.AddSurface(surface)
        if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddCutPlane failed.  objectIds:'%A' startPoint:'%A' endPoint:'%A' normal:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) startPoint endPoint normal
        Doc.Views.Redraw()
        objectId


    [<Extension>]
    ///<summary>Adds a cylinder-shaped Polysurface to the document</summary>
    ///<param name="basis">(Plane) The 3D basis point of the cylinder or the basis Plane of the cylinder</param>
    ///<param name="height">(float) If basis is a point, then height is a 3D height point of the
    ///    cylinder. The height point defines the height and direction of the
    ///    cylinder. If basis is a Plane, then height is the numeric height value
    ///    of the cylinder</param>
    ///<param name="radius">(float) Radius of the cylinder</param>
    ///<param name="cap">(bool) Optional, Default Value: <c>true</c>
    ///    Cap the cylinder</param>
    ///<returns>(Guid) identifier of new object.</returns>
    static member AddCylinder( basis:Plane,
                               height:float,
                               radius:float,
                               [<OPT;DEF(true)>]cap:bool) : Guid =
        let circle = Circle(basis, radius)
        let cylinder = Cylinder(circle, height)
        let brep = cylinder.ToBrep(cap, cap)
        let objectId = Doc.Objects.AddBrep(brep)
        if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddCylinder failed.  basis:'%A' height:'%A' radius:'%A' cap:'%A'" basis height radius cap
        Doc.Views.Redraw()
        objectId


    [<Extension>]
    ///<summary>Creates a Surface from 2, 3, or 4 edge Curves</summary>
    ///<param name="curveIds">(Guid seq) List or tuple of Curves</param>
    ///<returns>(Guid) identifier of new object.</returns>
    static member AddEdgeSrf(curveIds:Guid seq) : Guid =
        let curves =  rarr { for objectId in curveIds do yield RhinoScriptSyntax.CoerceCurve(objectId) }
        let brep = Brep.CreateEdgeSurface(curves)
        if brep|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.AddEdgeSrf failed.  curveIds:'%s'" (RhinoScriptSyntax.ToNiceString curveIds)
        let objectId = Doc.Objects.AddBrep(brep)
        if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddEdgeSrf failed.  curveIds:'%s'" (RhinoScriptSyntax.ToNiceString curveIds)
        Doc.Views.Redraw()
        objectId


    [<Extension>]
    ///<summary>Creates a Surface from a network of crossing Curves</summary>
    ///<param name="curves">(Guid seq) Curves from which to create the Surface</param>
    ///<param name="continuity">(int) Optional, Default Value: <c>1</c>
    ///    How the edges match the input geometry
    ///    0 = loose
    ///    1 = position
    ///    2 = tangency
    ///    3 = curvature</param>
    ///<param name="edgeTolerance">(float) Optional, Edge tolerance</param>
    ///<param name="interiorTolerance">(float) Optional, Interior tolerance</param>
    ///<param name="angleTolerance">(float) Optional, Angle tolerance , in radians?</param>
    ///<returns>(Guid) identifier of new object.</returns>
    static member AddNetworkSrf( curves:Guid seq,
                                 [<OPT;DEF(1)>]continuity:int,
                                 [<OPT;DEF(0.0)>]edgeTolerance:float,
                                 [<OPT;DEF(0.0)>]interiorTolerance:float,
                                 [<OPT;DEF(0.0)>]angleTolerance:float) : Guid =
        let curves =  rarr { for curve in curves do yield RhinoScriptSyntax.CoerceCurve(curve) }
        let surf, err = NurbsSurface.CreateNetworkSurface(curves, continuity, edgeTolerance, interiorTolerance, angleTolerance)// 0.0 Tolerance OK ? TODO
        if notNull surf then
            let rc = Doc.Objects.AddSurface(surf)
            Doc.Views.Redraw()
            rc
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddNetworkSrf failed on %A" curves

    [<Extension>]
    ///<summary>Adds a NURBS Surface object to the document</summary>
    ///<param name="pointCount">(int * int) Number of control points in the u and v direction</param>
    ///<param name="points">(Point3d IList) List or Array of 3D points</param>
    ///<param name="knotsU">(float IList) List or Array of Knot values for the Surface in the u direction.
    ///    Must contain pointCount[0]+degree[0]-1 elements</param>
    ///<param name="knotsV">(float IList) List or Array of Knot values for the Surface in the v direction.
    ///    Must contain pointCount[1]+degree[1]-1 elements</param>
    ///<param name="degree">(int * int) Degree of the Surface in the u and v directions</param>
    ///<param name="weights">(float IList) Optional, List or Array ofWeight values for the Surface. The number of elements in
    ///    weights must equal the number of elements in points. Values must be
    ///    greater than zero</param>
    ///<returns>(Guid) identifier of new object.</returns>
    static member AddNurbsSurface( pointCount:int * int,
                                   points:Point3d IList,
                                   knotsU:float IList,
                                   knotsV:float IList,
                                   degree:int * int,
                                   [<OPT;DEF(null:float IList)>]weights:float IList) : Guid =
        let pu, pv = pointCount
        let du, dv = degree
        if points.Count < (pu*pv) then
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddNurbsSurface failed.  pointCount:'%A' points:'%A' knotsU:'%A' knotsV:'%A' degree:'%A' weights:'%A'" pointCount points knotsU knotsV degree weights
        let ns = NurbsSurface.Create(3, notNull weights , du + 1, dv + 1, pu, pv)
        //add the points && weights
        let controlpoints = ns.Points
        let mutable index = 0


        if notNull weights then
            if weights.Count < (pu*pv) then
                RhinoScriptingException.Raise "RhinoScriptSyntax.AddNurbsSurface failed.  pointCount:'%A' points:'%A' knotsU:'%A' knotsV:'%A' degree:'%A' weights:'%A'" pointCount points knotsU knotsV degree weights
            for i in range(pu) do
                for j in range(pv) do
                    let cp = ControlPoint(points.[index], weights.[index])
                    controlpoints.SetControlPoint(i, j, cp)|> ignore
                    index <- index + 1
        else
            for i in range(pu) do
                for j in range(pv) do
                    let cp = ControlPoint(points.[index])
                    controlpoints.SetControlPoint(i, j, cp)|> ignore
                    index <- index + 1

        for i in range(pu) do
            for j in range(pv) do
                if notNull weights then
                    let cp = ControlPoint(points.[index], weights.[index])
                    controlpoints.SetControlPoint(i, j, cp)|> ignore
                else
                    let cp = ControlPoint(points.[index])
                    controlpoints.SetControlPoint(i, j, cp)|> ignore
                index <- index + 1

        //add the knots
        for i in range(ns.KnotsU.Count) do
            ns.KnotsU.[i] <-  knotsU.[i]
        for i in range(ns.KnotsV.Count) do
            ns.KnotsV.[i] <-  knotsV.[i]
        if not ns.IsValid then RhinoScriptingException.Raise "RhinoScriptSyntax.AddNurbsSurface failed.  pointCount:'%A' points:'%A' knotsU:'%A' knotsV:'%A' degree:'%A' weights:'%A'" pointCount points knotsU knotsV degree weights
        let objectId = Doc.Objects.AddSurface(ns)
        if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddNurbsSurface failed.  pointCount:'%A' points:'%A' knotsU:'%A' knotsV:'%A' degree:'%A' weights:'%A'" pointCount points knotsU knotsV degree weights
        Doc.Views.Redraw()
        objectId

    [<Extension>]
    ///<summary>Fits a Surface through Curve, point, point cloud, and Mesh objects</summary>
    ///<param name="objectIds">(Guid seq) A list of object identifiers that indicate the objects to use for the patch fitting.
    ///    Acceptable object types include Curves, points, point clouds, and Meshes</param>
    ///<param name="startSurfaceId">(Guid) The identifier of the starting Surface.  It is best if you create a starting Surface that is similar in shape
    ///    to the Surface you are trying to create</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    The tolerance used by input analysis functions.</param>
    ///<param name="trim">(bool) Optional, Default Value: <c>true</c>
    ///    Try to find an outside Curve and trims the Surface to it.  The default value is True</param>
    ///<param name="pointSpacing">(float) Optional, Default Value: <c>0.1</c>
    ///    The basic distance between points sampled from input Curves.  The default value is 0.1</param>
    ///<param name="flexibility">(float) Optional, Default Value: <c>1.0</c>
    ///    Determines the behavior of the Surface in areas where its not otherwise controlled by the input.
    ///    Lower numbers make the Surface behave more like a stiff material, higher, more like a flexible material.
    ///    That is, each span is made to more closely match the spans adjacent to it if there is no input geometry
    ///    mapping to that area of the Surface when the flexibility value is low.  The scale is logarithmic.
    ///    For example, numbers around 0.001 or 0.1 make the patch pretty stiff and numbers around 10 or 100
    ///    make the Surface flexible.  The default value is 1.0</param>
    ///<param name="surfacePull">(float) Optional, Default Value: <c>1.0</c>
    ///    Similar to stiffness, but applies to the starting Surface. The bigger the pull, the closer
    ///    the resulting Surface shape will be to the starting Surface.  The default value is 1.0</param>
    ///<param name="fixEdges">(bool) Optional, Default Value: <c>false</c>
    ///    Clamps the edges of the starting Surface in place. This option is useful if you are using a
    ///    Curve or points for deforming an existing Surface, and you do not want the edges of the starting Surface
    ///    to move.  The default if False</param>
    ///<returns>(Guid) Identifier of the new Surface object.</returns>
    static member AddPatch( objectIds:Guid seq,
                              startSurfaceId: Guid,
                              [<OPT;DEF(0.0)>]tolerance:float,
                              [<OPT;DEF(true)>]trim:bool,
                              [<OPT;DEF(0.1)>]pointSpacing:float,
                              [<OPT;DEF(1.0)>]flexibility:float,
                              [<OPT;DEF(1.0)>]surfacePull:float,
                              [<OPT;DEF(false)>]fixEdges:bool) : Guid =
                    let uspan, vspan = 10, 10
                    let geometry =   rarr{for objectId in objectIds do RhinoScriptSyntax.CoerceRhinoObject(objectId).Geometry }
                    let surface = RhinoScriptSyntax.CoerceSurface(startSurfaceId)
                    let tolerance = if 0.0 = tolerance then Doc.ModelAbsoluteTolerance else tolerance
                    let b =  Array.create 4 fixEdges
                    let brep = Brep.CreatePatch(geometry, surface, uspan, vspan, trim, false, pointSpacing, flexibility, surfacePull, b, tolerance)
                    if notNull brep then
                        let rc =  Doc.Objects.AddBrep(brep)
                        Doc.Views.Redraw()
                        rc
                    else
                        RhinoScriptingException.Raise "RhinoScriptSyntax.AddPatch faild for %A and %A" (RhinoScriptSyntax.ToNiceString objectIds) startSurfaceId

    [<Extension>]
    ///<summary>Fits a Surface through Curve, point, point cloud, and Mesh objects</summary>
    ///<param name="objectIds">(Guid seq) A list of object identifiers that indicate the objects to use for the patch fitting.
    ///    Acceptable object types include Curves, points, point clouds, and Meshes</param>
    ///<param name="uvSpans">(int * int) The U and V direction span counts for the automatically generated Surface . however it is best if you create a starting Surface that is similar in shape
    ///    to the Surface you are trying to create an use the other overload of this method</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    The tolerance used by input analysis functions.</param>
    ///<param name="trim">(bool) Optional, Default Value: <c>true</c>
    ///    Try to find an outside Curve and trims the Surface to it.  The default value is True</param>
    ///<param name="pointSpacing">(float) Optional, Default Value: <c>0.1</c>
    ///    The basic distance between points sampled from input Curves.  The default value is 0.1</param>
    ///<param name="flexibility">(float) Optional, Default Value: <c>1.0</c>
    ///    Determines the behavior of the Surface in areas where its not otherwise controlled by the input.
    ///    Lower numbers make the Surface behave more like a stiff material, higher, more like a flexible material.
    ///    That is, each span is made to more closely match the spans adjacent to it if there is no input geometry
    ///    mapping to that area of the Surface when the flexibility value is low.  The scale is logarithmic.
    ///    For example, numbers around 0.001 or 0.1 make the patch pretty stiff and numbers around 10 or 100
    ///    make the Surface flexible.  The default value is 1.0</param>
    ///<param name="surfacePull">(float) Optional, Default Value: <c>1.0</c>
    ///    Similar to stiffness, but applies to the starting Surface. The bigger the pull, the closer
    ///    the resulting Surface shape will be to the starting Surface.  The default value is 1.0</param>
    ///<param name="fixEdges">(bool) Optional, Default Value: <c>false</c>
    ///    Clamps the edges of the starting Surface in place. This option is useful if you are using a
    ///    Curve or points for deforming an existing Surface, and you do not want the edges of the starting Surface
    ///    to move.  The default if False</param>
    ///<returns>(Guid) Identifier of the new Surface object.</returns>
    static member AddPatch( objectIds:Guid seq,
                            uvSpans: int * int ,
                            [<OPT;DEF(0.0)>]tolerance:float,
                            [<OPT;DEF(true)>]trim:bool,
                            [<OPT;DEF(0.1)>]pointSpacing:float,
                            [<OPT;DEF(1.0)>]flexibility:float,
                            [<OPT;DEF(1.0)>]surfacePull:float,
                            [<OPT;DEF(false)>]fixEdges:bool) : Guid =

        let uspan, vspan = uvSpans
        let geometry =   rarr{for objectId in objectIds do RhinoScriptSyntax.CoerceRhinoObject(objectId).Geometry }
        let tolerance = if 0.0 = tolerance then Doc.ModelAbsoluteTolerance else tolerance
        let b =  Array.create 4 fixEdges
        let brep = Brep.CreatePatch(geometry, null, uspan, vspan, trim, false, pointSpacing, flexibility, surfacePull, b, tolerance) //TODO test with null as srf
        if notNull brep then
            let rc =  Doc.Objects.AddBrep(brep)
            Doc.Views.Redraw()
            rc
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddPatch faild for %A and %A" (RhinoScriptSyntax.ToNiceString objectIds) uvSpans


    [<Extension>]
    ///<summary>Creates a single walled Surface with a circular profile around a Curve</summary>
    ///<param name="curveId">(Guid) Identifier of rail Curve</param>
    ///<param name="parameters">(float seq) normalized Curve parameters</param>
    ///<param name="radii">(float seq) radius values at normalized Curve parameters</param>
    ///<param name="blendType">(int) Optional, Default Value: <c>0</c>
    ///    0(local) or 1(global)</param>
    ///<param name="cap">(int) Optional, Default Value: <c>0</c>
    ///    0(none), 1(flat), 2(round)</param>
    ///<param name="fit">(bool) Optional, Default Value: <c>false</c>
    ///    Attempt to fit a single Surface</param>
    ///<returns>(Guid Rarr) identifiers of new objects created.</returns>
    static member AddPipe( curveId:Guid,
                           parameters:float seq,
                           radii:float seq,
                           [<OPT;DEF(0)>]blendType:int,
                           [<OPT;DEF(0)>]cap:int,
                           [<OPT;DEF(false)>]fit:bool) : Guid Rarr =
        let rail = RhinoScriptSyntax.CoerceCurve(curveId)
        let abstol = Doc.ModelAbsoluteTolerance
        let angtol = Doc.ModelAngleToleranceRadians
        let cap :PipeCapMode  = LanguagePrimitives.EnumOfValue  cap
        let breps = Brep.CreatePipe(rail, parameters, radii, (blendType = 0), cap, fit, abstol, angtol)
        let rc =  rarr { for brep in breps do yield Doc.Objects.AddBrep(brep) }
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Creates one or more Surfaces from planar Curves</summary>
    ///<param name="objectIds">(Guid seq) Curves to use for creating planar Surfaces</param>
    ///<returns>(Guid Rarr) identifiers of Surfaces created .</returns>
    static member AddPlanarSrf(objectIds:Guid seq) : Guid Rarr =
        let curves =  rarr { for objectId in objectIds do yield RhinoScriptSyntax.CoerceCurve(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let breps = Brep.CreatePlanarBreps(curves, tolerance)
        if notNull breps then
            let rc =  rarr { for brep in breps do yield Doc.Objects.AddBrep(brep) }
            Doc.Views.Redraw()
            rc
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddPlanarSrf faild on %A" (RhinoScriptSyntax.ToNiceString objectIds) 


    [<Extension>]
    ///<summary>Create a Plane Surface and add it to the document</summary>
    ///<param name="plane">(Plane) The Plane</param>
    ///<param name="uDir">(float) The magnitude in the U direction</param>
    ///<param name="vDir">(float) The magnitude in the V direction</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    static member AddPlaneSurface( plane:Plane,
                                   uDir:float,
                                   vDir:float) : Guid =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        let uinterval = Interval(0.0, uDir)
        let vinterval = Interval(0.0, vDir)
        let planesurface = new PlaneSurface(plane, uinterval, vinterval)
        if planesurface|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPlaneSurface failed.  plane:'%A' uDir:'%A' vDir:'%A'" plane uDir vDir
        let rc = Doc.Objects.AddSurface(planesurface)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddPlaneSurface failed.  plane:'%A' uDir:'%A' vDir:'%A'" plane uDir vDir
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a Surface created by lofting Curves to the document.
    ///    - no Curve sorting performed. pass in Curves in the order you want them sorted
    ///    - directions of open Curves not adjusted. Use CurveDirectionsMatch and
    ///      ReverseCurve to adjust the directions of open Curves
    ///    - seams of closed Curves are not adjusted. Use CurveSeam to adjust the seam
    ///      of closed Curves</summary>
    ///<param name="objectIds">(Guid seq) Ordered list of the Curves to loft through</param>
    ///<param name="start">(Point3d) Optional, Starting point of the loft</param>
    ///<param name="ende">(Point3d) Optional, Ending point of the loft</param>
    ///<param name="loftType">(int) Optional, Default Value: <c>0</c>
    ///    Type of loft. Possible options are:
    ///    0 = Normal. Uses chord-length parameterization in the loft direction
    ///    1 = Loose. The Surface is allowed to move away from the original Curves
    ///      to make a smoother Surface. The Surface control points are created
    ///      at the same locations as the control points of the loft input Curves.
    ///    2 = Straight. The sections between the Curves are straight. This is
    ///      also known as a ruled Surface.
    ///    3 = Tight. The Surface sticks closely to the original Curves. Uses square
    ///      root of chord-length parameterization in the loft direction</param>
    ///<param name="rebuild">(int) Optional, Default Value: <c>0</c>
    ///    If not 0 then Rebuilds the shape Curves before lofting with this control poin count</param>
    ///<param name="refit">(float) Optional, if given the loft is refitted, the value is the tolerance used to rebuild</param>
    ///<param name="closed">(bool) Optional, Default Value: <c>false</c>
    ///    Close the loft back to the first Curve</param>
    ///<returns>(Guid Rarr) Array containing the identifiers of the new Surface objects.</returns>
    static member AddLoftSrf( objectIds:Guid seq,
                              [<OPT;DEF(Point3d())>]start:Point3d, //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
                              [<OPT;DEF(Point3d())>]ende:Point3d,
                              [<OPT;DEF(0)>]loftType:int,
                              [<OPT;DEF(0)>]rebuild:int,
                              [<OPT;DEF(0.0)>]refit:float,
                              [<OPT;DEF(false)>]closed:bool) : Guid Rarr =
        if loftType<0 || loftType>4 then RhinoScriptingException.Raise "RhinoScriptSyntax.Rhino.Scripting.AddLoftSrf: LoftType must be 0-4.  objectIds:'%A' start:'%A' end:'%A' loftType:'%A' rebuild:'%A' refit:'%A' closed:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) start ende loftType rebuild refit closed
        if rebuild<>0 && refit<>0.0 then RhinoScriptingException.Raise "RhinoScriptSyntax.Rhino.Scripting.AddLoftSrf: set either rebuild or refit to a value ! not both.  objectIds:'%A' start:'%A' end:'%A' loftType:'%A' rebuild:'%A' refit:'%A' closed:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) start ende loftType rebuild refit closed
        let curves =  rarr { for objectId in objectIds do yield RhinoScriptSyntax.CoerceCurve(objectId) }
        if Seq.length(curves)<2 then RhinoScriptingException.Raise "RhinoScriptSyntax.AddLoftSrf failed.  objectIds:'%A' start:'%A' end:'%A' loftType:'%A' rebuild:'%A' refit:'%A' closed:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) start ende loftType rebuild refit closed
        let start = if start = Point3d.Origin  then Point3d.Unset else start
        let ende  = if ende  = Point3d.Origin  then Point3d.Unset else ende
        let mutable lt = LoftType.Normal
        if loftType = 1 then lt <- LoftType.Loose
        elif loftType = 2 then lt <- LoftType.Straight
        elif loftType = 3 then lt <- LoftType.Tight
        //elif loftType = 4 then lt <- LoftType.Developable
        let mutable breps = null
        if rebuild = 0 && refit = 0.0 then
            breps <- Brep.CreateFromLoft(curves, start, ende, lt, closed)
        elif rebuild > 0 then
            breps <- Brep.CreateFromLoftRebuild(curves, start, ende, lt, closed, rebuild)
        elif refit > 0.0 then
            breps <- Brep.CreateFromLoftRefit(curves, start, ende, lt, closed, refit)
        if isNull breps then RhinoScriptingException.Raise "RhinoScriptSyntax.AddLoftSrf failed.  objectIds:'%A' start:'%A' end:'%A' loftType:'%A' rebuild:'%A' refit:'%A' closed:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) start ende loftType rebuild refit closed
        let idlist = Rarr()
        for brep in breps do
            let objectId = Doc.Objects.AddBrep(brep)
            if objectId <> Guid.Empty then idlist.Add(objectId)
        if idlist.IsNotEmpty then Doc.Views.Redraw()
        idlist


    [<Extension>]
    ///<summary>Create a Surface by revolving a Curve around an axis</summary>
    ///<param name="curveId">(Guid) Identifier of profile Curve</param>
    ///<param name="axis">(Line) Line for the rail revolve axis</param>
    ///<param name="startAngle">(float) Optional, Default Value: <c>0.0</c>
    ///    Start angles of revolve</param>
    ///<param name="endAngle">(float) Optional, Default Value: <c>360.0</c>
    ///    End angles of revolve</param>
    ///<returns>(Guid) identifier of new object.</returns>
    static member AddRevSrf( curveId:Guid,
                             axis:Line,
                             [<OPT;DEF(0.0)>]startAngle:float,
                             [<OPT;DEF(360.0)>]endAngle:float) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        //axis = RhinoScriptSyntax.Coerceline(axis)
        let startAngle = toRadians(startAngle)
        let endAngle = toRadians(endAngle)
        let srf = RevSurface.Create(curve, axis, startAngle, endAngle)
        if isNull srf then RhinoScriptingException.Raise "RhinoScriptSyntax.AddRevSrf failed. curveId:'%s' axis:'%A' startAngle:'%A' endAngle:'%A'" (rhType curveId) axis startAngle endAngle
        let ns = srf.ToNurbsSurface()
        if isNull ns then RhinoScriptingException.Raise "RhinoScriptSyntax.AddRevSrf failed. curveId:'%s' axis:'%A' startAngle:'%A' endAngle:'%A'" (rhType curveId) axis startAngle endAngle
        let rc = Doc.Objects.AddSurface(ns)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Add a spherical Surface to the document</summary>
    ///<param name="center">(Point3d) Center point of the sphere</param>
    ///<param name="radius">(float) Radius of the sphere in the current model units</param>
    ///<returns>(Guid) identifier of the new object .</returns>
    static member AddSphere(center:Point3d, radius:float) : Guid =
        let sphere = Sphere(center, radius)
        let rc = Doc.Objects.AddSphere(sphere)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSphere failed.  centerOrPlane:'%A' radius:'%A'" center radius
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a spaced series of planar Curves resulting from the intersection of
    ///    defined cutting Planes through a Surface or Polysurface. For more
    ///    information, see Rhino help for details on the Contour command</summary>
    ///<param name="objectId">(Guid) Object identifier to contour</param>
    ///<param name="plane">(Plane) The Plane that defines the cutting Plane</param>    
    ///<returns>(Guid Rarr) ids of new contour Curves .</returns>
    static member AddSrfContourCrvs( objectId:Guid,
                                     plane:Plane) : Guid Rarr =
        let brep = RhinoScriptSyntax.CoerceBrep(objectId)
        //plane = RhinoScriptSyntax.Coerceplane(pointsOrPlane)
        let curves =  Brep.CreateContourCurves(brep, plane)
        let rc = Rarr()
        for crv in curves do
            let objectId = Doc.Objects.AddCurve(crv)
            if objectId <> Guid.Empty then rc.Add(objectId)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a spaced series of planar Curves resulting from the intersection of
    ///    defined cutting Planes through a Surface or Polysurface. For more
    ///    information, see Rhino help for details on the Contour command</summary>
    ///<param name="objectId">(Guid) Object identifier to contour</param>
    ///<param name="startPoint">(Point3d) The startpoint of a center line</param>
    ///<param name="endPoint">(Point3d)   the end point of a center line</param>
    ///<param name="interval">(float) Distance between contour Curves</param>
    ///<returns>(Guid Rarr) ids of new contour Curves .</returns>
    static member AddSrfContourCrvs( objectId:Guid,
                                     startPoint:Point3d,
                                     endPoint :Point3d ,
                                     interval:float) : Guid Rarr=
        let brep = RhinoScriptSyntax.CoerceBrep(objectId)
        let curves =  Brep.CreateContourCurves(brep, startPoint , endPoint, interval)
        let rc = Rarr()
        for crv in curves do
            let objectId = Doc.Objects.AddCurve(crv)
            if objectId <> Guid.Empty then rc.Add(objectId)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Creates a Surface from a grid of points</summary>
    ///<param name="count">(int * int) Tuple of two numbers defining number of points in the u, v directions</param>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<param name="degreeU">(int) Optional, Default Value: <c>3</c> degree of the Surface in the U directions</param>
    ///<param name="degreeV">(int) Optional, Default Value: <c>3</c> degree of the Surface in the V directions</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    static member AddSrfControlPtGrid( count:int * int,
                                       points:Point3d seq,                                       
                                       [<OPT;DEF(3)>]degreeU:int,
                                       [<OPT;DEF(3)>]degreeV:int           ) : Guid =
        //points = RhinoScriptSyntax.Coerce3dpointlist(points)
        let surf = NurbsSurface.CreateFromPoints(points, fst count, snd count,  degreeU,  degreeV)
        if isNull surf then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSrfControlPtGrid failed.  count:'%A' points:'%A' degree:'%A'" count points (degreeU,degreeV) 
        let objectId = Doc.Objects.AddSurface(surf)
        if objectId <> Guid.Empty then
            Doc.Views.Redraw()
            objectId
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddSrfControlPtGrid failed.  count:'%A' points:'%A' degree:'%A'" count points (degreeU,degreeV) 

    [<Extension>]
    ///<summary>Creates a new Surface from four corner points</summary>
    ///<param name="pointA">(Point3d) First corner point</param>
    ///<param name="pointB">(Point3d) Second  corner point</param>
    ///<param name="pointC">(Point3d) Third corner point</param>
    ///<param name="pointD">(Point3d) Fourth corner point</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    static member AddSrfPt(pointA:Point3d , pointB:Point3d , pointC: Point3d , pointD: Point3d) : Guid =
        let surface = NurbsSurface.CreateFromCorners(pointA , pointB , pointC , pointD)
        if surface|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSrfPt failed.  points:'%A, %A, %A and %A" pointA pointB pointC pointD
        let rc = Doc.Objects.AddSurface(surface)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSrfPt failed.  points:'%A, %A, %A and %A" pointA pointB pointC pointD
        Doc.Views.Redraw()
        rc

    [<Extension>]
    ///<summary>Creates a new Surface from three corner points</summary>
    ///<param name="pointA">(Point3d) First corner point</param>
    ///<param name="pointB">(Point3d) Second  corner point</param>
    ///<param name="pointC">(Point3d) Third corner point</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    static member AddSrfPt(pointA:Point3d , pointB:Point3d , pointC: Point3d ) : Guid =
        let surface = NurbsSurface.CreateFromCorners(pointA , pointB , pointC)
        if surface|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSrfPt failed.  points:'%A, %A and %A" pointA pointB pointC
        let rc = Doc.Objects.AddSurface(surface)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSrfPt failed.  points:'%A, %A and %A" pointA pointB pointC
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Creates a Surface from a grid of points</summary>
    ///<param name="count">(int * int) Tuple of two numbers defining number of points in the u, v directions</param>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<param name="degreeU">(int) Optional, Default Value: <c>3</c> degree of the Surface in the U directions</param>
    ///<param name="degreeV">(int) Optional, Default Value: <c>3</c> degree of the Surface in the V directions</param>
    ///<param name="closedU">(bool * bool) Optional, Default Value: <c>false</c>  boolean defining if the Surface is closed in the U directions</param>
    ///<param name="closedV">(bool * bool) Optional, Default Value: <c>false</c>  boolean defining if the Surface is closed in the V directions</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    static member AddSrfPtGrid( count:int * int,
                                points:Point3d seq,
                                [<OPT;DEF(3)>]degreeU:int,
                                [<OPT;DEF(3)>]degreeV:int,
                                [<OPT;DEF(false)>]closedU:bool,
                                [<OPT;DEF(false)>]closedV:bool) : Guid =
        //points = RhinoScriptSyntax.Coerce3dpointlist(points)
        let surf = NurbsSurface.CreateThroughPoints(points, fst count, snd count, degreeU, degreeV, closedU, closedV)
        if isNull surf then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSrfPtGrid failed.  count:'%A' points:'%A' degree:'%A' closed:'%A'" count points (degreeU,degreeV) (closedU,closedV)
        let objectId = Doc.Objects.AddSurface(surf)
        if objectId <> Guid.Empty then
            Doc.Views.Redraw()
            objectId
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddSrfPtGrid failed.  count:'%A' points:'%A' degree:'%A' closed:'%A'" count points (degreeU,degreeV) (closedU,closedV)


    [<Extension>]
    ///<summary>Adds a Surface created through profile Curves that define the Surface
    ///    shape and one Curve that defines a Surface edge</summary>
    ///<param name="rail">(Guid) Identifier of the rail Curve</param>
    ///<param name="shapes">(Guid seq) One or more cross section shape Curves</param>
    ///<param name="closed">(bool) Optional, Default Value: <c>false</c>
    ///    If True, then create a closed Surface</param>
    ///<returns>(Guid Rarr) of new Surface objects.</returns>
    static member AddSweep1( rail:Guid,
                             shapes:Guid seq,
                             [<OPT;DEF(false)>]closed:bool) : Guid Rarr =
        let rail = RhinoScriptSyntax.CoerceCurve(rail)
        let shapes =  rarr { for shape in shapes do yield RhinoScriptSyntax.CoerceCurve(shape) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let breps = Brep.CreateFromSweep(rail, shapes, closed, tolerance)
        if isNull breps then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSweep1 failed.  rail:'%A' shapes:'%A' closed:'%A'" rail shapes closed
        let rc =  rarr { for brep in breps do yield Doc.Objects.AddBrep(brep) }
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a Surface created through profile Curves that define the Surface
    ///    shape and two Curves that defines a Surface edge</summary>
    ///<param name="rails">(Guid * Guid) Identifiers of the two rail Curve</param>
    ///<param name="shapes">(Guid seq) One or more cross section shape Curves</param>
    ///<param name="closed">(bool) Optional, Default Value: <c>false</c>
    ///    If True, then create a closed Surface</param>
    ///<returns>(Guid Rarr) of new Surface objects.</returns>
    static member AddSweep2( rails:Guid * Guid,
                             shapes:Guid seq,
                             [<OPT;DEF(false)>]closed:bool) : Guid Rarr =
        let rail1 = RhinoScriptSyntax.CoerceCurve(fst rails)
        let rail2 = RhinoScriptSyntax.CoerceCurve(snd rails)
        let shapes =  rarr { for shape in shapes do yield RhinoScriptSyntax.CoerceCurve(shape) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let breps = Brep.CreateFromSweep(rail1, rail2, shapes, closed, tolerance)
        if isNull breps then RhinoScriptingException.Raise "RhinoScriptSyntax.AddSweep2 failed.  rails:'%A' shapes:'%A' closed:'%A'" rails shapes closed
        let rc =  rarr { for brep in breps do yield Doc.Objects.AddBrep(brep) }
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a Surface created through profile Curves that define the Surface
    ///    shape and two Curves that defines a Surface edge</summary>
    ///<param name="profile">(Guid) Identifier of the profile Curve</param>
    ///<param name="rail">(Guid) Identifier of the rail Curve</param>
    ///<param name="axis">(Line) A LIne identifying the start point and end point of the rail revolve axis</param>
    ///<param name="scaleHeight">(bool) Optional, Default Value: <c>false</c>
    ///    If True, Surface will be locally scaled. Defaults to False</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member AddRailRevSrf( profile:Guid,
                                 rail:Guid,
                                 axis:Line,
                                 [<OPT;DEF(false)>]scaleHeight:bool) : Guid =
        let profileinst = RhinoScriptSyntax.CoerceCurve(profile)
        let railinst = RhinoScriptSyntax.CoerceCurve(rail)
        let surface = NurbsSurface.CreateRailRevolvedSurface(profileinst, railinst, axis, scaleHeight)
        if isNull surface then RhinoScriptingException.Raise "RhinoScriptSyntax.AddRailRevSrf failed.  profile:'%A' rail:'%A' axis:'%A' scaleHeight:'%A'" profile rail axis scaleHeight
        let rc = Doc.Objects.AddSurface(surface)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Adds a torus shaped revolved Surface to the document</summary>
    ///<param name="basis">(Plane) The basis Plane of the torus</param>
    ///<param name="majorRadius">(float) Major radius of  the torus</param>
    ///<param name="minorRadius">(float) Minor radius of  torus</param>
    ///<returns>(Guid) The identifier of the new object.</returns>
    static member AddTorus( basis:Plane,
                            majorRadius:float,
                            minorRadius:float) : Guid = 
        let torus = Torus(basis, majorRadius, minorRadius) 
        let revsurf = torus.ToRevSurface()
        let rc = Doc.Objects.AddSurface(revsurf)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Performs a boolean difference operation on two sets of input Surfaces
    ///    and Polysurfaces. For more details, see the BooleanDifference command in
    ///    the Rhino help file</summary>
    ///<param name="input0">(Guid seq) List of Surfaces to subtract from</param>
    ///<param name="input1">(Guid seq) List of Surfaces to be subtracted</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete all input objects</param>
    ///<returns>(Guid Rarr) of identifiers of newly created objects .</returns>
    static member BooleanDifference( input0:Guid seq,
                                     input1:Guid seq,
                                     [<OPT;DEF(true)>]deleteInput:bool) : Guid Rarr =

        let breps0 =  rarr { for objectId in input0 do yield RhinoScriptSyntax.CoerceBrep(objectId) }
        let breps1 =  rarr { for objectId in input1 do yield RhinoScriptSyntax.CoerceBrep(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let newbreps = Brep.CreateBooleanDifference(breps0, breps1, tolerance)
        if newbreps|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.BooleanDifference failed.  input0:'%A' input1:'%A' deleteInput:'%A'" input0 input1 deleteInput
        let rc =  rarr { for brep in newbreps do yield Doc.Objects.AddBrep(brep) }
        if deleteInput then
            for objectId in input0 do Doc.Objects.Delete(objectId, true)|> ignore
            for objectId in input1 do Doc.Objects.Delete(objectId, true)|> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Performs a boolean intersection operation on two sets of input Surfaces
    ///    and Polysurfaces. For more details, see the BooleanIntersection command in
    ///    the Rhino help file</summary>
    ///<param name="input0">(Guid seq) List of Surfaces</param>
    ///<param name="input1">(Guid seq) List of Surfaces</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete all input objects</param>
    ///<returns>(Guid Rarr) of identifiers of newly created objects .</returns>
    static member BooleanIntersection( input0:Guid seq,
                                       input1:Guid seq,
                                       [<OPT;DEF(true)>]deleteInput:bool) : Guid Rarr =
        let breps0 =  rarr { for objectId in input0 do yield RhinoScriptSyntax.CoerceBrep(objectId) }
        let breps1 =  rarr { for objectId in input1 do yield RhinoScriptSyntax.CoerceBrep(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let newbreps = Brep.CreateBooleanIntersection(breps0, breps1, tolerance)
        if newbreps|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.BooleanIntersection failed.  input0:'%A' input1:'%A' deleteInput:'%A'" input0 input1 deleteInput
        let rc =  rarr { for brep in newbreps do yield Doc.Objects.AddBrep(brep) }
        if deleteInput then
            for objectId in input0 do Doc.Objects.Delete(objectId, true)|> ignore
            for objectId in input1 do Doc.Objects.Delete(objectId, true)|> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Performs a boolean union operation on a set of input Surfaces and
    ///    Polysurfaces. For more details, see the BooleanUnion command in the
    ///    Rhino help file</summary>
    ///<param name="input">(Guid seq) List of Surfaces to union</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///    Delete all input objects</param>
    ///<returns>(Guid Rarr) of identifiers of newly created objects .</returns>
    static member BooleanUnion(input:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid Rarr =
        if Seq.length(input)<2 then RhinoScriptingException.Raise "RhinoScriptSyntax.BooleanUnion failed.  input:'%A' deleteInput:'%A'" input deleteInput
        let breps =  rarr { for objectId in input do yield RhinoScriptSyntax.CoerceBrep(objectId) }
        let tolerance = Doc.ModelAbsoluteTolerance
        let newbreps = Brep.CreateBooleanUnion(breps, tolerance)
        if newbreps|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.BooleanUnion failed.  input:'%A' deleteInput:'%A'" input deleteInput
        let rc =  rarr { for brep in newbreps do yield Doc.Objects.AddBrep(brep) }
        if  deleteInput then
            for objectId in input do Doc.Objects.Delete(objectId, true)|> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Returns the point on a Surface or Polysurface that is closest to a test
    ///    point. This function works on both untrimmed and trimmed Surfaces</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="point">(Point3d) The test, or sampling point</param>
    ///<returns>(Point3d * float * float * ComponentIndexType * int * Vector3d) of closest point information . The list will
    ///    contain the following information:
    ///    Element     Type            Description
    ///      0        Point3d          The 3-D point at the parameter value of the closest point.
    ///      1        (U of U, V)      Parameter values of closest point.
    ///                                   Note, V is 0 if the component index type is brepEdge or brepVertex.
    ///      2        (V of U, V)      Parameter values of closest point.
    ///                                   Note, V is 0 if the component index type is brepEdge or brepVertex.
    ///      3        (type, index)   The type  the brep component that contains the closest point. Possible types are
    ///                                   BrepVertex 1 Targets a brep vertex index.
    ///                                   BrepEdge   2 Targets a brep edge index.
    ///                                   BrepFace   3 Targets a brep face index.
    ///                                   BrepTrim   4 Targets a brep trim index.
    ///                                   BrepLoop   5 Targets a brep loop index.
    ///      4        int             The index of the brep component
    ///      5        Vector3d        The normal to the brepFace, or the tangent to the brepEdge.</returns>
    static member BrepClosestPoint(objectId:Guid, point:Point3d) : Point3d * float * float * ComponentIndexType * int * Vector3d =
        let brep = RhinoScriptSyntax.CoerceBrep(objectId)
        let clpt = ref Point3d.Origin
        let ci = ref ComponentIndex.Unset
        let s = ref 0.0
        let t = ref 0.0
        let n = ref Vector3d.Zero
        let ok = brep.ClosestPoint(point, clpt, ci, s, t, 0.0, n)
        if ok then
            let typ = (!ci).ComponentIndexType
            let idx = (!ci).Index
            !clpt,!s,!t, typ, idx, !n
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.BrepClosestPoint faile for %A and %A" (rhType objectId) point


    [<Extension>]
    ///<summary>Caps planar holes in a Surface or Polysurface</summary>
    ///<param name="surfaceId">(Guid) The identifier of the Surface or Polysurface to cap</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member CapPlanarHoles(surfaceId:Guid) : bool =
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        let tolerance = Doc.ModelAbsoluteTolerance
        let newbrep = brep.CapPlanarHoles(tolerance)
        if notNull newbrep then
            if newbrep.SolidOrientation = BrepSolidOrientation.Inward then
                newbrep.Flip()
            //surfaceId = RhinoScriptSyntax.Coerceguid(surfaceId)
            if Doc.Objects.Replace(surfaceId, newbrep) then
                Doc.Views.Redraw()
                true
            else
                false
        else
            false


    [<Extension>]
    ///<summary>Duplicates the edge Curves of a Surface or Polysurface. For more
    ///    information, see the Rhino help file for information on the DupEdge
    ///    command</summary>
    ///<param name="objectId">(Guid) The identifier of the Surface or Polysurface object</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the duplicated edge Curves. The default is not to select (False)</param>
    ///<returns>(Guid Rarr) identifying the newly created Curve objects.</returns>
    static member DuplicateEdgeCurves(objectId:Guid, [<OPT;DEF(false)>]select:bool) : Guid Rarr =
        let brep = RhinoScriptSyntax.CoerceBrep(objectId)
        let outcurves = brep.DuplicateEdgeCurves()
        let curves = Rarr()
        for curve in outcurves do
            if curve.IsValid then
                let rc = Doc.Objects.AddCurve(curve)
                curve.Dispose()
                if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.DuplicateEdgeCurves faile on one of the edge curves"
                curves.Add(rc)
                if select then
                    let rhobject = RhinoScriptSyntax.CoerceRhinoObject(rc)
                    rhobject.Select(true)  |>  ignore //TODO make sync ?
        if curves.IsNotEmpty then Doc.Views.Redraw()
        curves


    [<Extension>]
    ///<summary>Create Curves that duplicate a Surface or Polysurface border</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface</param>
    ///<param name="typ">(int) Optional, Default Value: <c>0</c>
    ///    The border Curves to return
    ///    0 = both exterior and interior,
    ///    1 = exterior
    ///    2 = interior</param>
    ///<returns>(Guid Rarr) list of Curve ids .</returns>
    static member DuplicateSurfaceBorder(surfaceId:Guid, [<OPT;DEF(0)>]typ:int) : Guid Rarr =
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        let inner = typ = 0 || typ = 2
        let outer = typ = 0 || typ = 1
        let mutable curves = brep.DuplicateNakedEdgeCurves(outer, inner)
        if curves|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.DuplicateSurfaceBorder failed.  surfaceId:'%s' typ:'%d'" (rhType surfaceId) typ
        let tolerance = Doc.ModelAbsoluteTolerance * 2.1
        curves <- Curve.JoinCurves(curves, tolerance)
        if curves|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.DuplicateSurfaceBorder failed.  surfaceId:'%s' typ:'%d'" (rhType surfaceId) typ
        let rc =  rarr { for c in curves do yield Doc.Objects.AddCurve(c) }
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Evaluates a Surface at a U, V parameter</summary>
    ///<param name="surfaceId">(Guid) The object's identifier</param>
    ///<param name="u">(float) U of u, v parameters to evaluate</param>
    ///<param name="v">(float) V of u, v parameters to evaluate</param>
    ///<returns>(Point3d) a 3-D point.</returns>
    static member EvaluateSurface( surfaceId:Guid,
                                   u:float ,
                                   v:float ) : Point3d =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let rc = surface.PointAt(u, v)
        if rc.IsValid then rc
        else RhinoScriptingException.Raise "RhinoScriptSyntax.EvaluateSurface failed.  surfaceId:'%s' u:'%f' v:'%f'" (rhType surfaceId) u v


    [<Extension>]
    ///<summary>Lengthens an untrimmed Surface object</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface</param>
    ///<param name="parameter">(float * float) Tuple of two values definfing the U, V parameter to evaluate.
    ///    The Surface edge closest to the U, V parameter will be the edge that is
    ///    extended</param>
    ///<param name="length">(float) Amount to extend to Surface</param>
    ///<param name="smooth">(bool) Optional, Default Value: <c>true</c>
    ///    If True, the Surface is extended smoothly curving from the
    ///    edge. If False, the Surface is extended in a straight line from the edge</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member ExtendSurface( surfaceId:Guid,
                                 parameter:float * float,
                                 length:float,
                                 [<OPT;DEF(true)>]smooth:bool) : bool =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let edge = surface.ClosestSide(parameter|> fst, parameter|> snd)
        let newsrf = surface.Extend(edge, length, smooth)
        if notNull newsrf then
            Doc.Objects.Replace(surfaceId, newsrf)|> ignore
            Doc.Views.Redraw()
        else
            ()
        notNull newsrf


    [<Extension>]
    ///<summary>Explodes, or unjoins, one or more Polysurface objects. Polysurfaces
    ///    will be exploded into separate Surfaces</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of Polysurfaces to explode</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input objects after exploding</param>
    ///<returns>(Guid Rarr) of identifiers of exploded pieces .</returns>
    static member ExplodePolysurfaces(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid Rarr =
        let ids = Rarr()
        for objectId in objectIds do
            let brep = RhinoScriptSyntax.CoerceBrep(objectId)
            if brep.Faces.Count>1 then
                for i in range(brep.Faces.Count) do
                    let copyface = brep.Faces.[i].DuplicateFace(false)
                    let faceid = Doc.Objects.AddBrep(copyface)
                    if faceid <> Guid.Empty then ids.Add(faceid)
                if  deleteInput then Doc.Objects.Delete(objectId, true) |> ignore
        Doc.Views.Redraw()
        ids


    [<Extension>]
    ///<summary>Extracts isoparametric Curves from a Surface</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface</param>
    ///<param name="parameter">(float * float) U, v parameter of the Surface to evaluate</param>
    ///<param name="direction">(int) Direction to evaluate
    ///    0 = u
    ///    1 = v
    ///    2 = both</param>
    ///<returns>(Guid Rarr) of Curve ids .</returns>
    static member ExtractIsoCurve( surfaceId:Guid,
                                   parameter:float * float,
                                   direction:int) : Guid Rarr =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let ids = Rarr()
        let mutable curves = [| |]
        if direction = 0 || direction = 2 then

            match surface with
            | :? BrepFace as br ->
                curves <- br.TrimAwareIsoCurve(0, parameter|> snd)
            | _ ->
                curves <- [|surface.IsoCurve(0, parameter|> snd) |]
            if notNull curves then
                for curve in curves do
                    let objectId = Doc.Objects.AddCurve(curve)
                    if objectId <> Guid.Empty then ids.Add(objectId)

        if direction = 1 || direction = 2 then
            curves <- null
            match surface with
            | :? BrepFace as br ->
                curves <- br.TrimAwareIsoCurve(1, parameter|> fst)
            | _ ->
                curves <- [|surface.IsoCurve(1, parameter|> fst)|]
            if notNull curves then
                for curve in curves do
                    let objectId = Doc.Objects.AddCurve(curve)
                    if objectId <> Guid.Empty then ids.Add(objectId)
        Doc.Views.Redraw()
        ids


    [<Extension>]
    ///<summary>Separates or copies a Surface or a copy of a Surface from a Polysurface</summary>
    ///<param name="objectId">(Guid) Polysurface identifier</param>
    ///<param name="faceIndices">(int seq) One or more numbers representing faces</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///    If True the faces are copied. If False, the faces are extracted</param>
    ///<returns>(Guid Rarr) identifiers of extracted Surface objects .</returns>
    static member ExtractSurface( objectId:Guid,
                                  faceIndices:int seq,
                                  [<OPT;DEF(false)>]copy:bool) : Guid Rarr =
        let brep = RhinoScriptSyntax.CoerceBrep(objectId)
        let rc = Rarr()
        let faceIndices = Seq.sort(faceIndices)|> Seq.rev
        for index in faceIndices do
            let face = brep.Faces.[index]
            let newbrep = face.DuplicateFace(true)
            let objectId = Doc.Objects.AddBrep(newbrep)
            rc.Add(objectId)
        if copy then
            for index in faceIndices do brep.Faces.RemoveAt(index)
            Doc.Objects.Replace(objectId, brep)|> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Creates a Surface by extruding a Curve along a path</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve to extrude</param>
    ///<param name="pathId">(Guid) Identifier of the path Curve</param>
    ///<returns>(Guid) identifier of new Surface .</returns>
    static member ExtrudeCurve(curveId:Guid, pathId:Guid) : Guid =
        let curve1 = RhinoScriptSyntax.CoerceCurve(curveId)
        let curve2 = RhinoScriptSyntax.CoerceCurve(pathId)
        let srf = SumSurface.Create(curve1, curve2)
        let rc = Doc.Objects.AddSurface(srf)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ExtrudeCurve failed. curveId:'%s' pathId:'%s'" (rhType curveId) <| rhType pathId
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Creates a Surface by extruding a Curve to a point</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve to extrude</param>
    ///<param name="point">(Point3d) 3D point</param>
    ///<returns>(Guid) identifier of new Surface .</returns>
    static member ExtrudeCurvePoint(curveId:Guid, point:Point3d) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        let srf = Surface.CreateExtrusionToPoint(curve, point)
        let rc = Doc.Objects.AddSurface(srf)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ExtrudeCurvePoint failed. curveId:'%s' point:'%A'" (rhType curveId) point
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Create Surface by extruding a Curve along two points that define a line</summary>
    ///<param name="curveId">(Guid) Identifier of the Curve to extrude</param>
    ///<param name="startPoint">(Point3d) Start point</param>
    ///<param name="endPoint">(Point3d) End point, that specifyies distance and direction</param>
    ///<returns>(Guid) identifier of new Surface .</returns>
    static member ExtrudeCurveStraight( curveId:Guid,
                                        startPoint:Point3d,
                                        endPoint:Point3d) : Guid =
        let curve = RhinoScriptSyntax.CoerceCurve(curveId)
        //startPoint = RhinoScriptSyntax.Coerce3dpoint(startPoint)
        //endPoint = RhinoScriptSyntax.Coerce3dpoint(endPoint)
        let vec = endPoint - startPoint
        let srf = Surface.CreateExtrusion(curve, vec)
        let rc = Doc.Objects.AddSurface(srf)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ExtrudeCurveStraight failed. curveId:'%s' startPoint:'%A' endPoint:'%A'" (rhType curveId) startPoint endPoint
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Create Surface by extruding along a path Curve</summary>
    ///<param name="surface">(Guid) Identifier of the Surface to extrude</param>
    ///<param name="curve">(Guid) Identifier of the path Curve</param>
    ///<param name="cap">(bool) Optional, Default Value: <c>true</c>
    ///    Extrusion is capped at both ends</param>
    ///<returns>(Guid) identifier of new Surface .</returns>
    static member ExtrudeSurface( surface:Guid,
                                  curve:Guid,
                                  [<OPT;DEF(true)>]cap:bool) : Guid =
        let brep = RhinoScriptSyntax.CoerceBrep(surface)
        let curve = RhinoScriptSyntax.CoerceCurve(curve)
        let newbrep = brep.Faces.[0].CreateExtrusion(curve, cap)
        if notNull newbrep then
            let rc = Doc.Objects.AddBrep(newbrep)
            Doc.Views.Redraw()
            rc
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.ExtrudeSurface faild on %A and %A" surface curve


    [<Extension>]
    ///<summary>Create constant radius rolling ball fillets between two Surfaces. Note,
    ///    this function does not trim the original Surfaces of the fillets</summary>
    ///<param name="surface0">(Guid) first Surface</param>
    ///<param name="surface1">(Guid) second Surface</param>
    ///<param name="radius">(float) A positive fillet radius</param>
    ///<param name="uvparam0">(Point2d) Optional, A u, v Surface parameter of Surface0 near where the fillet
    ///    is expected to hit the Surface</param>
    ///<param name="uvparam1">(Point2d) Optional, Same as uvparam0, but for Surface1</param>
    ///<returns>(Guid Rarr) ids of Surfaces created .</returns>
    static member FilletSurfaces( surface0:Guid,
                                  surface1:Guid,
                                  radius:float,
                                  [<OPT;DEF(Point2d())>]uvparam0:Point2d,
                                  [<OPT;DEF(Point2d())>]uvparam1:Point2d) : Guid Rarr=
        let surface0 = RhinoScriptSyntax.CoerceSurface(surface0)
        let surface1 = RhinoScriptSyntax.CoerceSurface(surface1)
        let tol = Doc.ModelAbsoluteTolerance
        let surfaces =
            if uvparam0<>Point2d.Origin && uvparam1<>Point2d.Origin then
                Surface.CreateRollingBallFillet(surface0, uvparam0, surface1, uvparam1, radius, tol)
            else
                Surface.CreateRollingBallFillet(surface0, surface1, radius, tol)
        if isNull surfaces then RhinoScriptingException.Raise "RhinoScriptSyntax.FilletSurfaces failed.  surface0:'%A' surface1:'%A' radius:'%A' uvparam0:'%A' uvparam1:'%A'" surface0 surface1 radius uvparam0 uvparam1
        let rc = Rarr()
        for surf in surfaces do
            rc.Add( Doc.Objects.AddSurface(surf))
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Returns the normal direction of a Surface. This feature can
    /// also be found in Rhino's Dir command</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface object</param>
    ///<returns>(bool) The current normal orientation.</returns>
    static member FlipSurface(surfaceId:Guid) : bool = //GET
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        if brep.Faces.Count>1 then RhinoScriptingException.Raise "RhinoScriptSyntax.FlipSurface Get failed.  surfaceId:'%s'" (rhType surfaceId)
        let face = brep.Faces.[0]
        let oldreverse = face.OrientationIsReversed
        oldreverse

    [<Extension>]
    ///<summary>Changes the normal direction of a Surface. This feature can
    /// also be found in Rhino's Dir command</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface object</param>
    ///<param name="flip">(bool) New normal orientation, either flipped(True) or not flipped (False)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member FlipSurface(surfaceId:Guid, flip:bool) : unit = //SET
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        if brep.Faces.Count>1 then RhinoScriptingException.Raise "RhinoScriptSyntax.FlipSurface failed.  surfaceId:'%s' flip:'%A'" (rhType surfaceId) flip
        let face = brep.Faces.[0]
        let oldreverse = face.OrientationIsReversed
        if brep.IsSolid = false && oldreverse <> flip then
            brep.Flip()
            Doc.Objects.Replace(surfaceId, brep)|> ignore
            Doc.Views.Redraw()
    [<Extension>]
    ///<summary>Changes the normal direction of multiple Surface. This feature can
    /// also be found in Rhino's Dir command</summary>
    ///<param name="surfaceIds">(Guid seq) Identifiers of multiple Surface objects</param>
    ///<param name="flip">(bool) New normal orientation, either flipped(True) or not flipped (False)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member FlipSurface(surfaceIds:Guid seq, flip:bool) : unit = //MULTISET
        for surfaceId in surfaceIds do 
            let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
            if brep.Faces.Count>1 then RhinoScriptingException.Raise "RhinoScriptSyntax.FlipSurface failed.  surfaceId:'%s' flip:'%A'" (rhType surfaceId) flip
            let face = brep.Faces.[0]
            let oldreverse = face.OrientationIsReversed
            if brep.IsSolid = false && oldreverse <> flip then
                brep.Flip()
                Doc.Objects.Replace(surfaceId, brep)|> ignore
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Intersects a brep object with another brep object. Note, unlike the
    ///    SurfaceSurfaceIntersection function this function works on trimmed Surfaces</summary>
    ///<param name="brep1">(Guid) Identifier of first brep object</param>
    ///<param name="brep2">(Guid) Identifier of second brep object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Distance tolerance at segment midpoints. If omitted,
    ///    the current absolute tolerance is used</param>
    ///<returns>(Guid Rarr) identifying the newly created intersection Curve and point objects.</returns>
    static member IntersectBreps( brep1:Guid,
                                  brep2:Guid,
                                  [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr =
        let brep1 = RhinoScriptSyntax.CoerceBrep(brep1)
        let brep2 = RhinoScriptSyntax.CoerceBrep(brep2)
        let tolerance = ifZero2 Doc.ModelAbsoluteTolerance  tolerance
        let ok, outcurves, outpoints = Intersect.Intersection.BrepBrep(brep1, brep2, tolerance)
        let ids = Rarr()
        if not ok then ids // empty array TODO or fail ?
        else
            let mergedcurves = Curve.JoinCurves(outcurves, 2.1 * tolerance)

            if notNull mergedcurves then
                for curve in mergedcurves do
                    if curve.IsValid then
                        let rc = Doc.Objects.AddCurve(curve)
                        curve.Dispose()
                        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.IntersectBreps failed.  brep1:'%A' brep2:'%A' tolerance:'%A'" brep1 brep2 tolerance
                        ids.Add(rc)
            else
                for curve in outcurves do
                    if curve.IsValid then
                        let rc = Doc.Objects.AddCurve(curve)
                        curve.Dispose()
                        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.IntersectBreps failed.  brep1:'%A' brep2:'%A' tolerance:'%A'" brep1 brep2 tolerance
                        ids.Add(rc)
            for point in outpoints do
                let rc = Doc.Objects.AddPoint(point)
                if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.IntersectBreps failed.  brep1:'%A' brep2:'%A' tolerance:'%A'" brep1 brep2 tolerance
                ids.Add(rc)
            Doc.Views.Redraw()
            ids


    [<Extension>]
    ///<summary>Calculates intersections of two spheres</summary>
    ///<param name="spherePlane0">(Plane) An equatorial Plane of the first sphere. The origin of the
    ///    Plane will be the center point of the sphere</param>
    ///<param name="sphereRadius0">(float) Radius of the first sphere</param>
    ///<param name="spherePlane1">(Plane) Plane for second sphere</param>
    ///<param name="sphereRadius1">(float) Radius for second sphere</param>
    ///<returns>(int * Circle * float ) of intersection results
    ///    [0] = type of intersection (0 = point, 1 = circle, 2 = spheres are identical)
    ///    [1] = Circle of  intersection , if type is Point take Circle center
    ///    [2] = radius of circle if circle intersection.</returns>
    static member IntersectSpheres( spherePlane0:Plane,
                                    sphereRadius0:float,
                                    spherePlane1:Plane,
                                    sphereRadius1:float) : int * Circle * float =
        let sphere0 = Sphere(spherePlane0, sphereRadius0)
        let sphere1 = Sphere(spherePlane1, sphereRadius1)
        let rc, circle = Intersect.Intersection.SphereSphere(sphere0, sphere1)
        if rc = Intersect.SphereSphereIntersection.Point then
            0, circle, 0.0
        elif rc = Intersect.SphereSphereIntersection.Circle then
            1, circle, circle.Radius
        elif rc = Intersect.SphereSphereIntersection.Overlap then
            2, circle, sphere0.Radius
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.IntersectSpheres failed.  spherePlane0:'%A' sphereRadius0:'%A' spherePlane1:'%A' sphereRadius1:'%A'" spherePlane0 sphereRadius0 spherePlane1 sphereRadius1


    [<Extension>]
    ///<summary>Verifies an object is a Brep, or a boundary representation model, object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsBrep(objectId:Guid) : bool =
        (RhinoScriptSyntax.TryCoerceBrep(objectId)).IsSome


    [<Extension>]
    ///<summary>Determines if a Surface is a portion of a cone</summary>
    ///<param name="objectId">(Guid) The Surface object's identifier</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsCone(objectId:Guid) : bool =
        let surface = RhinoScriptSyntax.CoerceSurface(objectId)
        surface.IsCone()


    [<Extension>]
    ///<summary>Determines if a Surface is a portion of a cone</summary>
    ///<param name="objectId">(Guid) The cylinder object's identifier</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsCylinder(objectId:Guid) : bool =
        let surface = RhinoScriptSyntax.CoerceSurface(objectId)
        surface.IsCylinder()


    [<Extension>]
    ///<summary>Verifies an object is a Plane Surface. Plane Surfaces can be created by
    ///    the Plane command. Note, a Plane Surface is not a planar NURBS Surface</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsPlaneSurface(objectId:Guid) : bool =
        let face = RhinoScriptSyntax.CoerceSurface(objectId)
        match face with
        | :? BrepFace  as bface ->
            if bface.IsSurface then
                match bface.UnderlyingSurface() with
                | :?  PlaneSurface -> true
                | _ -> false
            else
                false
        | _ -> false


    [<Extension>]
    ///<summary>Verifies that a point is inside a closed Mesh , Surface or Polysurface</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="point">(Point3d) The test, or sampling point</param>
    ///<param name="strictlyIn">(bool) Optional, Default Value: <c>false</c>
    ///    If true, the test point must be inside by at least tolerance</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>RhinoMath.SqrtEpsilon</c>
    ///    Distance tolerance used for intersection and determining
    ///    strict inclusion.</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsPointInSurface( objectId:Guid,
                                    point:Point3d,
                                    [<OPT;DEF(false)>]strictlyIn:bool,
                                    [<OPT;DEF(0.0)>]tolerance:float) : bool =
        //objectId = RhinoScriptSyntax.Coerceguid(objectId)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        //if objectId|> isNull  || point|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.IsPointInSurface failed.  objectId:'%s' point:'%A' strictlyIn:'%A' tolerance:'%A'" (rhType objectId) point strictlyIn tolerance
        let obj = Doc.Objects.FindId(objectId)
        let  tolerance= ifZero1 tolerance RhinoMath.SqrtEpsilon
        match obj with
        | :? DocObjects.ExtrusionObject as es->
            let brep= es.ExtrusionGeometry.ToBrep(false)
            if not brep.IsSolid then RhinoScriptingException.Raise "RhinoScriptSyntax.IsPointInSurface failed on not closed Extrusion %A"  objectId
            brep.IsPointInside(point, tolerance, strictlyIn)
        | :? DocObjects.BrepObject as bo->
            let br= bo.BrepGeometry
            if not br.IsSolid then RhinoScriptingException.Raise "RhinoScriptSyntax.IsPointInSurface failed on not closed Brep %A"  objectId
            br.IsPointInside(point, tolerance, strictlyIn)
        | :? DocObjects.MeshObject as mo ->
            let me = mo.MeshGeometry
            if not me.IsClosed then RhinoScriptingException.Raise "RhinoScriptSyntax.IsPointInSurface failed on not closed Mesh %A"  objectId
            me.IsPointInside(point, tolerance, strictlyIn)
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.IsPointInSurface does not work  on %s %A" (RhinoScriptSyntax.ObjectDescription(objectId)) objectId

    [<Extension>]
    ///<summary>Verifies that a point lies on a Surface</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="point">(Point3d) The test, or sampling point</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsPointOnSurface(objectId:Guid, point:Point3d) : bool =
        let surf = RhinoScriptSyntax.CoerceSurface(objectId)
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        let mutable rc, u, v = surf.ClosestPoint(point)
        if rc  then
            let srfpt = surf.PointAt(u, v)
            if srfpt.DistanceTo(point) > Doc.ModelAbsoluteTolerance then
                rc <- false
            else
                match RhinoScriptSyntax.TryCoerceBrep(objectId) with
                | Some b ->
                    rc <- b.Faces.[0].IsPointOnFace(u, v) <> PointFaceRelation.Exterior
                | _ -> ()
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.IsPointOnSurface failed for surf.ClosestPoint on %A %A" (rhType objectId) point
        rc


    [<Extension>]
    ///<summary>Verifies an object is a Folysurface or Extrusion. Polysurfaces consist of two or more
    ///    Surfaces joined together. </summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if successful, otherwise False.</returns>
    static member IsPolysurface(objectId:Guid) : bool =
        match Doc.Objects.FindId(objectId) with 
        | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.IsPolysurface: %A is not an object in Doc.Objects table" objectId
        | o ->  match o.Geometry with          
                | :? Brep as b -> b.Faces.Count > 1 
                | :? Extrusion  -> true   
                | _ -> false

      

    [<Extension>]
    ///<summary>Verifies a Guid refers to a closed Polysurface or closed Extrtrusion. If the Polysurface fully encloses
    ///    a volume, it is considered a solid</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True if successful, otherwise False.</returns>
    static member IsPolysurfaceClosed(objectId:Guid) : bool =
        match Doc.Objects.FindId(objectId) with 
        | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.IsPolysurfaceClosed: %A is not an object in Doc.Objects table" objectId
        | o ->  match o.Geometry with          
                | :? Brep as b -> b.IsSolid
                | :? Extrusion as e -> e.IsSolid   
                | _ -> false        
       

    [<Extension>]
    ///<summary>Determines if a Surface is a portion of a Sphere</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsSphere(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(objectId) with
        | Some b ->  b.IsSphere()
        | _ -> false



    [<Extension>]
    ///<summary>Verifies an object is a Surface. Brep objects with only one face are
    ///    also considered Surfaces</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsSurface(objectId:Guid) : bool =
        RhinoScriptSyntax.TryCoerceSurface(objectId).IsSome


    [<Extension>]
    ///<summary>Verifies a Surface object is closed in the specified direction.  If the
    ///    Surface fully encloses a volume, it is considered a solid</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface</param>
    ///<param name="direction">(int) 
    ///    0 = U direction check, 
    ///    1 = V direction check</param>
    ///<returns>(bool) True or False.</returns>
    static member IsSurfaceClosed(surfaceId:Guid, direction:int) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with
        | Some surface ->  surface.IsClosed(direction)
        | _ -> false


    [<Extension>]
    ///<summary>Verifies a Surface object is periodic in the specified direction</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface</param>
    ///<param name="direction">(int) 
    ///    0 = U direction check, 
    ///    1 = V direction check</param>
    ///<returns>(bool) True or False.</returns>
    static member IsSurfacePeriodic(surfaceId:Guid, direction:int) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with
        | Some surface ->  surface.IsPeriodic(direction)
        | _ -> false



    [<Extension>]
    ///<summary>Verifies a Surface object is planar</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Tolerance used when checked. If omitted, the current absolute
    ///    tolerance is used</param>
    ///<returns>(bool) True or False.</returns>
    static member IsSurfacePlanar(surfaceId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance = ifZero1 tolerance Doc.ModelAbsoluteTolerance
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with
        | Some surface ->  surface.IsPlanar(tolerance)
        | _ -> false



    [<Extension>]
    ///<summary>Verifies a Surface object is rational</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsSurfaceRational(surfaceId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with //TODO better fail if input is not a surface ?? here and above functions
        | Some surface ->
            let ns = surface.ToNurbsSurface()
            if ns|> isNull  then false
            else ns.IsRational
        | _ -> false



    [<Extension>]
    ///<summary>Verifies a Surface object is singular in the specified direction.
    ///    Surfaces are considered singular if a side collapses to a point</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="direction">(int) 
    ///    0 = south
    ///    1 = east
    ///    2 = north
    ///    3 = west</param>
    ///<returns>(bool) True or False.</returns>
    static member IsSurfaceSingular(surfaceId:Guid, direction:int) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with    //TODO better fail if input is not a surface ?? here and above functions
        | Some surface ->
            surface.IsSingular(direction)
        | _ -> false


    [<Extension>]
    ///<summary>Verifies a Surface object has been trimmed</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>(bool) True or False.</returns>
    static member IsSurfaceTrimmed(surfaceId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceBrep(surfaceId) with
        | Some brep ->  not brep.IsSurface
        | _ -> false //TODO better fail if input is not a surface ?? here and above functions



    [<Extension>]
    ///<summary>Determines if a Surface is a portion of a torus</summary>
    ///<param name="surfaceId">(Guid) The Surface object's identifier</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsTorus(surfaceId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with //TODO better fail if input is not a surface ?? here and above functions
        | Some surface ->  surface.IsTorus()
        | _ -> false



    [<Extension>]
    ///<summary>Gets the sphere definition from a Surface, if possible</summary>
    ///<param name="surfaceId">(Guid) The identifier of the Surface object</param>
    ///<returns>(Plane * float) The equatorial Plane of the sphere, and its radius.</returns>
    static member SurfaceSphere(surfaceId:Guid) : Plane * float =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let tol = Doc.ModelAbsoluteTolerance
        let sphere = ref Sphere.Unset
        let issphere = surface.TryGetSphere(sphere, tol)
        if issphere then (!sphere).EquatorialPlane, (!sphere).Radius
        else RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceSphere input is not a sphere %A"surfaceId


    [<Extension>]
    ///<summary>Joins two or more Surface or Polysurface objects together to form one
    ///    Polysurface object</summary>
    ///<param name="objectIds">(Guid seq) List of object identifiers</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the original Surfaces</param>
    ///<returns>(Guid) identifier of newly created object.</returns>
    static member JoinSurfaces(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let breps =  rarr { for objectId in objectIds do yield RhinoScriptSyntax.CoerceBrep(objectId) }
        if breps.Count<2 then RhinoScriptingException.Raise "RhinoScriptSyntax.JoinSurfaces failed, less than two objects given.  objectIds:'%A' deleteInput:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) deleteInput
        let tol = Doc.ModelAbsoluteTolerance * 2.1
        let joinedbreps = Brep.JoinBreps(breps, tol)
        if joinedbreps|> isNull  then
            RhinoScriptingException.Raise "RhinoScriptSyntax.JoinSurfaces failed.  objectIds:'%A' deleteInput:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) deleteInput
        if joinedbreps.Length <> 1 then
            RhinoScriptingException.Raise "RhinoScriptSyntax.JoinSurfaces resulted in more than one object: %d  objectIds:'%A' deleteInput:'%A'" joinedbreps.Length objectIds deleteInput
        let rc = Doc.Objects.AddBrep(joinedbreps.[0])
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.JoinSurfaces failed.  objectIds:'%A' deleteInput:'%A'" (RhinoScriptSyntax.ToNiceString objectIds) deleteInput
        if  deleteInput then
            for objectId in objectIds do
                //id = RhinoScriptSyntax.Coerceguid(objectId)
                Doc.Objects.Delete(objectId, true) |> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Makes an existing Surface a periodic NURBS Surface</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="direction">(int) The direction to make periodic, either 
    ///    0 = U  
    ///    1 = V</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete the input Surface</param>
    ///<returns>(Guid) if deleteInput is False, identifier of the new Surface.</returns>
    static member MakeSurfacePeriodic( surfaceId:Guid,
                                       direction:int,
                                       [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let newsurf = Surface.CreatePeriodicSurface(surface, direction)
        if newsurf|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.MakeSurfacePeriodic failed.  surfaceId:'%s' direction:'%A' deleteInput:'%A'" (rhType surfaceId) direction deleteInput
        //id = RhinoScriptSyntax.Coerceguid(surfaceId)
        if deleteInput then
            Doc.Objects.Replace(surfaceId, newsurf)|> ignore
            Doc.Views.Redraw()
            surfaceId
        else
            let objectIdn = Doc.Objects.AddSurface(newsurf)
            Doc.Views.Redraw()
            objectIdn


    [<Extension>]
    ///<summary>Offsets a trimmed or untrimmed Surface by a distance. The offset Surface
    ///    will be added to Rhino</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="distance">(float) The distance to offset</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    The offset tolerance. Use 0.0 to make a loose offset. Otherwise, the
    ///    document's absolute tolerance is usually sufficient</param>
    ///<param name="bothSides">(bool) Optional, Default Value: <c>false</c>
    ///    Offset to both sides of the input Surface</param>
    ///<param name="createSolid">(bool) Optional, Default Value: <c>false</c>
    ///    Make a solid object</param>
    ///<returns>(Guid) identifier of the new object.</returns>
    static member OffsetSurface( surfaceId:Guid,
                                 distance:float,
                                 [<OPT;DEF(0.0)>]tolerance:float,
                                 [<OPT;DEF(false)>]bothSides:bool,
                                 [<OPT;DEF(false)>]createSolid:bool) : Guid =
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        let mutable face = null
        if (1 = brep.Faces.Count) then face <- brep.Faces.[0]
        if face|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetSurface failed.  surfaceId:'%s' distance:'%A' tolerance:'%A' bothSides:'%A' createSolid:'%A'" (rhType surfaceId) distance tolerance bothSides createSolid
        let tolerance= ifZero1 tolerance Doc.ModelAbsoluteTolerance
        let newbrep = Brep.CreateFromOffsetFace(face, distance, tolerance, bothSides, createSolid)
        if newbrep|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetSurface failed.  surfaceId:'%s' distance:'%A' tolerance:'%A' bothSides:'%A' createSolid:'%A'" (rhType surfaceId) distance tolerance bothSides createSolid
        let rc = Doc.Objects.AddBrep(newbrep)
        if rc = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.OffsetSurface failed.  surfaceId:'%s' distance:'%A' tolerance:'%A' bothSides:'%A' createSolid:'%A'" (rhType surfaceId) distance tolerance bothSides createSolid
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Pulls a Curve object to a Surface object</summary>
    ///<param name="surface">(Guid) The Surface's identifier</param>
    ///<param name="curve">(Guid) The Curve's identifier</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Should the input items be deleted</param>
    ///<returns>(Guid Rarr) of new Curves.</returns>
    static member PullCurve( surface:Guid,
                             curve:Guid,
                             [<OPT;DEF(false)>]deleteInput:bool) : Guid Rarr =
        let crvobj = RhinoScriptSyntax.CoerceRhinoObject(curve)
        let brep = RhinoScriptSyntax.CoerceBrep(surface)
        let curve = RhinoScriptSyntax.CoerceCurve(curve)
        let tol = Doc.ModelAbsoluteTolerance
        let curves = Curve.PullToBrepFace(curve, brep.Faces.[0], tol)
        let rc =  rarr { for curve in curves do yield Doc.Objects.AddCurve(curve) }
        if deleteInput  then
            Doc.Objects.Delete(crvobj, true) |> ignore
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Rebuilds a Surface to a given degree and control point count. For more
    ///    information see the Rhino help file for the Rebuild command</summary>
    ///<param name="objectId">(Guid) The Surface's identifier</param>
    ///<param name="degreeU">(int) Optional, Default Value: <c>3</c> degree of the Surface in the U directions</param>
    ///<param name="degreeV">(int) Optional, Default Value: <c>3</c> degree of the Surface in the V directions</param>
    ///<param name="pointcountU">(int) Optional, Default Value: <c>10*</c> the Surface point count in U  directions</param>
    ///<param name="pointcountV">(int) Optional, Default Value: <c>10*</c> the Surface point count in V  directions</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member RebuildSurface( objectId:Guid,
                                  [<OPT;DEF(3)>]degreeU:int,
                                  [<OPT;DEF(3)>]degreeV:int,
                                  [<OPT;DEF(10)>]pointcountU:int ,
                                  [<OPT;DEF(10)>]pointcountV:int ) : bool =        
        
        let surface = RhinoScriptSyntax.CoerceSurface(objectId)
        let newsurf = surface.Rebuild( degreeU, degreeV, pointcountU, pointcountV )
        if newsurf|> isNull  then false
        else
            //objectId = RhinoScriptSyntax.Coerceguid(objectId)
            let rc = Doc.Objects.Replace(objectId, newsurf)
            if rc then Doc.Views.Redraw()
            rc


    [<Extension>]
    ///<summary>Deletes a knot from a Surface object</summary>
    ///<param name="surface">(Guid) The reference of the Surface object</param>
    ///<param name="uvParameter">(float * float)): An indexable item containing a U, V parameter on the Surface. List, tuples and UVIntervals will work.
    ///    Note, if the parameter is not equal to one of the existing knots, then the knot closest to the specified parameter will be removed</param>
    ///<param name="vDirection">(bool) If True, or 1, the V direction will be addressed. If False, or 0, the U direction</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member RemoveSurfaceKnot( surface:Guid,
                                     uvParameter:float * float,
                                     vDirection:bool) : bool =
        let srfinst = RhinoScriptSyntax.CoerceSurface(surface)
        let uparam = uvParameter|> fst
        let vparam = uvParameter|> snd
        let success, nuparam, nvparam = srfinst.GetSurfaceParameterFromNurbsFormParameter(uparam, vparam)
        if not success then false
        else
            let nsrf = srfinst.ToNurbsSurface()
            if isNull nsrf then false
            else
                let knots = if vDirection then nsrf.KnotsV  else nsrf.KnotsU
                let success = knots.RemoveKnotsAt(nuparam, nvparam)
                if not success then false
                else
                    Doc.Objects.Replace(surface, nsrf)|> ignore
                    Doc.Views.Redraw()
                    true


    [<Extension>]
    ///<summary>Reverses U or V directions of a Surface, or swaps (transposes) U and V
    ///    directions</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface object</param>
    ///<param name="direction">(int) As a bit coded flag to swap
    ///    1 = reverse U
    ///    2 = reverse V
    ///    4 = transpose U and V (values can be combined)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ReverseSurface(surfaceId:Guid, direction:int) : unit =
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        if brep.Faces.Count <> 1 then RhinoScriptingException.Raise "RhinoScriptSyntax.ReverseSurface failed.  surfaceId:'%s' direction:'%A'" (rhType surfaceId) direction
        let face = brep.Faces.[0]
        if direction &&& 1 <> 0 then            face.Reverse(0, true)|> ignoreObj
        if direction &&& 2 <> 0 then            face.Reverse(1, true)|> ignoreObj
        if direction &&& 4 <> 0 then            face.Transpose(true) |> ignoreObj
        Doc.Objects.Replace(surfaceId, brep)|> ignore
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Shoots a ray at a collection of Surfaces or Polysurfaces</summary>
    ///<param name="surfaceIds">(Guid seq) One of more Surface identifiers</param>
    ///<param name="startPoint">(Point3d) Starting point of the ray</param>
    ///<param name="direction">(Vector3d) Vector identifying the direction of the ray</param>
    ///<param name="reflections">(int) Optional, Default Value: <c>10</c>
    ///    The maximum number of times the ray will be reflected</param>
    ///<returns>(Point3d array) of reflection points.</returns>
    static member ShootRay( surfaceIds:Guid seq,
                            startPoint:Point3d,
                            direction:Vector3d,
                            [<OPT;DEF(10)>]reflections:int) : Point3d array =
        //startPoint = RhinoScriptSyntax.Coerce3dpoint(startPoint)
        //direction = RhinoScriptSyntax.Coerce3dvector(direction)
        //id = RhinoScriptSyntax.Coerceguid(surfaceIds)
        //if notNull objectId then surfaceIds <- .[id]
        let ray = Ray3d(startPoint, direction)
        let breps = rarr{for objectId in surfaceIds do RhinoScriptSyntax.CoerceBrep(objectId)}
        if breps.Count = 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.ShootRay failed.  surfaceIds:'%A' startPoint:'%A' direction:'%A' reflections:'%A'"  surfaceIds startPoint direction reflections
        Intersect.Intersection.RayShoot(ray, Seq.cast breps, reflections)



    [<Extension>]
    ///<summary>Creates the shortest possible Curve(geodesic) between two points on a
    ///    Surface. For more details, see the ShortPath command in Rhino help</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface</param>
    ///<param name="startPoint">(Point3d) Start point the short Curve</param>
    ///<param name="endPoint">(Point3d) End point of the short Curve</param>
    ///<returns>(Guid) identifier of the new Surface.</returns>
    static member ShortPath( surfaceId:Guid,
                             startPoint:Point3d,
                             endPoint:Point3d) : Guid =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        //start = RhinoScriptSyntax.Coerce3dpoint(startPoint)
        //end = RhinoScriptSyntax.Coerce3dpoint(endPoint)
        let rcstart, ustart, vstart = surface.ClosestPoint(startPoint)
        let rcend, uend, vend = surface.ClosestPoint(endPoint)
        if not rcstart || not rcend then RhinoScriptingException.Raise "RhinoScriptSyntax.ShortPath failed.  surfaceId:'%s' startPoint:'%A' endPoint:'%A'" (rhType surfaceId) startPoint endPoint
        let start = Point2d(ustart, vstart)
        let ende = Point2d(uend, vend)
        let tolerance = Doc.ModelAbsoluteTolerance
        let curve = surface.ShortPath(start, ende, tolerance)
        if curve|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.ShortPath failed.  surfaceId:'%s' startPoint:'%A' endPoint:'%A'" (rhType surfaceId) startPoint endPoint
        let objectId = Doc.Objects.AddCurve(curve)
        if objectId = Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.ShortPath failed.  surfaceId:'%s' startPoint:'%A' endPoint:'%A'" (rhType surfaceId) startPoint endPoint
        Doc.Views.Redraw()
        objectId


    [<Extension>]
    ///<summary>Shrinks the underlying untrimmed Surfaces near to the trimming
    ///    boundaries. See the ShrinkTrimmedSrf command in the Rhino help</summary>
    ///<param name="objectId">(Guid) The Surface's identifier</param>
    ///<param name="createCopy">(bool) Optional, Default Value: <c>false</c>
    ///    If True, the original Surface is not deleted</param>
    ///<returns>(Guid) If createCopy is true the new Guid, else the input Guid.</returns>
    static member ShrinkTrimmedSurface(objectId:Guid, [<OPT;DEF(false)>]createCopy:bool) : Guid =
        let brep = RhinoScriptSyntax.CoerceBrep(objectId)
        if brep.Faces.ShrinkFaces() then RhinoScriptingException.Raise "RhinoScriptSyntax.ShrinkTrimmedSurface failed.  objectId:'%s' createCopy:'%A'" (rhType objectId) createCopy
        if  createCopy then
            let oldobj = Doc.Objects.FindId(objectId)
            let attr = oldobj.Attributes
            let rc = Doc.Objects.AddBrep(brep, attr)
            Doc.Views.Redraw()
            rc
        else
            Doc.Objects.Replace(objectId, brep)|> ignore
            Doc.Views.Redraw()
            objectId




    [<Extension>]
    ///<summary>Splits a brep</summary>
    ///<param name="brepId">(Guid) Identifier of the brep to split</param>
    ///<param name="cutterId">(Guid) Identifier of the brep to split with</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Delete input breps</param>
    ///<returns>(Guid Rarr) identifiers of split pieces.</returns>
    static member SplitBrep( brepId:Guid,
                             cutterId:Guid,
                             [<OPT;DEF(false)>]deleteInput:bool) : Guid Rarr =
        let brep = RhinoScriptSyntax.CoerceBrep(brepId)
        let cutter = RhinoScriptSyntax.CoerceBrep(cutterId)
        let tol = Doc.ModelAbsoluteTolerance
        let pieces = brep.Split(cutter, tol)
        if isNull pieces then RhinoScriptingException.Raise "RhinoScriptSyntax.SplitBrep failed.  brepId:'%s' cutterId:'%s' deleteInput:'%A'" (rhType brepId) (rhType cutterId) deleteInput
        if  deleteInput then
            //brepId = RhinoScriptSyntax.Coerceguid(brepId)
            Doc.Objects.Delete(brepId, true) |> ignore
        let rc =  rarr { for piece in pieces do yield Doc.Objects.AddBrep(piece) }
        Doc.Views.Redraw()
        rc
    

    [<Extension>]
    ///<summary>Calculate the area of a Surface Geometry. 
    /// The results are based on the current drawing units</summary>
    ///<param name="srf">(Geometry.Surface) The Surface geometry</param>
    ///<returns>(float) of area.</returns>
    static member SurfaceArea(srf:Surface) : float  =        
        let amp = AreaMassProperties.Compute(srf, true, false, false, false)
        if isNull amp then  RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceArea failed on Surface: %A" srf
        amp.Area

    [<Extension>]
    ///<summary>Calculate the area of a Brep / Polysurface  Geometry. 
    /// The results are based on the current drawing units</summary>
    ///<param name="brep">(Geometry.Brep) The Polysurface geometry</param>
    ///<returns>(float) of area.</returns>
    static member SurfaceArea(brep:Brep) : float  =        
        let amp = AreaMassProperties.Compute(brep, true, false, false, false)
        if isNull amp then  RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceArea failed on Brep: %A" brep
        amp.Area

    [<Extension>]
    ///<summary>Calculate the area of a Surface or Polysurface object. 
    /// The results are based on the current drawing units</summary>
    ///<param name="objectId">(Guid) The Surface's identifier</param>
    ///<returns>(float) of area.</returns>
    static member SurfaceArea(objectId:Guid) : float  =        
        match RhinoScriptSyntax.CoerceGeometry objectId with
        | :? Surface as s -> RhinoScriptSyntax.SurfaceArea s        
        | :? Brep    as b -> RhinoScriptSyntax.SurfaceArea b
        | x -> RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceArea doesnt work on on %A" (rhType  objectId)
   




    [<Extension>]
    ///<summary>Calculates the area centroid of a Surface or Polysurface</summary>
    ///<param name="objectId">(Guid) The Surface's identifier</param>
    ///<returns>(Point3d ) Area centroid.</returns>
    static member SurfaceAreaCentroid(objectId:Guid) : Point3d =
        objectId
        |> RhinoScriptSyntax.TryCoerceBrep
        |> Option.map AreaMassProperties.Compute
        |> Option.orElseWith (fun () ->
            objectId
            |> RhinoScriptSyntax.TryCoerceSurface
            |> Option.map AreaMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceAreaCentroid failed.  objectId:'%s'" (rhType objectId))
        |> fun amp -> amp.Centroid


    [<Extension>]
    ///<summary>Calculates area moments of inertia of a Surface or Polysurface object.
    ///    See the Rhino help for "Mass Properties calculation details"</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>((float*float*float) Rarr ) of moments and error bounds in tuples(X, Y, Z) - see help topic
    ///    Index   Description
    ///    [0]     First Moments.
    ///    [1]     The absolute (+/-) error bound for the First Moments.
    ///    [2]     Second Moments.
    ///    [3]     The absolute (+/-) error bound for the Second Moments.
    ///    [4]     Product Moments.
    ///    [5]     The absolute (+/-) error bound for the Product Moments.
    ///    [6]     Area Moments of Inertia about the World Coordinate Axes.
    ///    [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
    ///    [8]     Area Radii of Gyration about the World Coordinate Axes.
    ///    [9]     (Not impemented yet) The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///    [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///    [13]    (Not impemented yet) The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.</returns>
    static member SurfaceAreaMoments(surfaceId:Guid) : (float*float*float) Rarr =
        surfaceId
        |> RhinoScriptSyntax.TryCoerceBrep
        |> Option.map AreaMassProperties.Compute
        |> Option.orElseWith (fun () ->
            surfaceId
            |> RhinoScriptSyntax.TryCoerceSurface
            |> Option.map AreaMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceArea failed on %A" surfaceId)
        |> fun mp ->
            rarr{
                yield (mp.WorldCoordinatesFirstMoments.X, mp.WorldCoordinatesFirstMoments.Y, mp.WorldCoordinatesFirstMoments.Z)                                     //  [0]     First Moments.
                yield (mp.WorldCoordinatesFirstMomentsError.X, mp.WorldCoordinatesFirstMomentsError.Y, mp.WorldCoordinatesFirstMomentsError.Z)                      //  [1]     The absolute (+/-) error bound for the First Moments.
                yield (mp.WorldCoordinatesSecondMoments.X, mp.WorldCoordinatesSecondMoments.Y, mp.WorldCoordinatesSecondMoments.Z)                                  //  [2]     Second Moments.
                yield (mp.WorldCoordinatesSecondMomentsError.X, mp.WorldCoordinatesSecondMomentsError.Y, mp.WorldCoordinatesSecondMomentsError.Z)                   //  [3]     The absolute (+/-) error bound for the Second Moments.
                yield (mp.WorldCoordinatesProductMoments.X, mp.WorldCoordinatesProductMoments.Y, mp.WorldCoordinatesProductMoments.Z)                               //  [4]     Product Moments.
                yield (mp.WorldCoordinatesProductMomentsError.X, mp.WorldCoordinatesProductMomentsError.Y, mp.WorldCoordinatesProductMomentsError.Z)                //  [5]     The absolute (+/-) error bound for the Product Moments.
                yield (mp.WorldCoordinatesMomentsOfInertia.X, mp.WorldCoordinatesMomentsOfInertia.Y, mp.WorldCoordinatesMomentsOfInertia.Z)                         //  [6]     Area Moments of Inertia about the World Coordinate Axes.
                yield (mp.WorldCoordinatesMomentsOfInertiaError.X, mp.WorldCoordinatesMomentsOfInertiaError.Y, mp.WorldCoordinatesMomentsOfInertiaError.Z)          //  [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
                yield (mp.WorldCoordinatesRadiiOfGyration.X, mp.WorldCoordinatesRadiiOfGyration.Y, mp.WorldCoordinatesRadiiOfGyration.Z)                            //  [8]     Area Radii of Gyration about the World Coordinate Axes.
                yield (0., 0., 0.) // need to add error calc to RhinoCommon                                                                                           //  [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
                yield (mp.CentroidCoordinatesMomentsOfInertia.X, mp.CentroidCoordinatesMomentsOfInertia.Y, mp.CentroidCoordinatesMomentsOfInertia.Z)                //  [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
                yield (mp.CentroidCoordinatesMomentsOfInertiaError.X, mp.CentroidCoordinatesMomentsOfInertiaError.Y, mp.CentroidCoordinatesMomentsOfInertiaError.Z) //  [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
                yield (mp.CentroidCoordinatesRadiiOfGyration.X, mp.CentroidCoordinatesRadiiOfGyration.Y, mp.CentroidCoordinatesRadiiOfGyration.Z)                   //  [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
                yield (0., 0., 0.) //need to add error calc to RhinoCommon                                                                                            //  [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes</returns>
                }


    [<Extension>]
    ///<summary>Returns the point on a Surface that is closest to a test point</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface object</param>
    ///<param name="testPoint">(Point3d) Sampling point</param>
    ///<returns>(Point3d) The closest point on the Surface.</returns>
    static member SurfaceClosestPoint(surfaceId:Guid, testPoint:Point3d) : Point3d =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        //point = RhinoScriptSyntax.Coerce3dpoint(testPoint)
        let rc, u, v = surface.ClosestPoint(testPoint)
        if not rc then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceClosestPoint failed on %A and %A" surfaceId testPoint
        surface.PointAt(u, v)

    [<Extension>]
    ///<summary>Returns U, V parameters of point on a Surface that is closest to a test point</summary>
    ///<param name="surfaceId">(Guid) Identifier of a Surface object</param>
    ///<param name="testPoint">(Point3d) Sampling point</param>
    ///<returns>(float * float) The U, V parameters of the closest point on the Surface.</returns>
    static member SurfaceClosestParameter(surfaceId:Guid, testPoint:Point3d) : float * float =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        //point = RhinoScriptSyntax.Coerce3dpoint(testPoint)
        let rc, u, v = surface.ClosestPoint(testPoint)
        if not rc then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceClosestParameter failed on %A and %A" surfaceId testPoint
        u, v


    [<Extension>]
    ///<summary>Returns the definition of a Surface cone</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>(Plane * float * float) containing the definition of the cone
    ///    [0]   the Plane of the cone. The apex of the cone is at the
    ///      Plane's origin and the axis of the cone is the Plane's z-axis
    ///    [1]   the height of the cone
    ///    [2]   the radius of the cone.</returns>
    static member SurfaceCone(surfaceId:Guid) : Plane * float * float =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let rc, cone = surface.TryGetCone()
        if not rc then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceCone failed.  surfaceId:'%s'" (rhType surfaceId)
        cone.Plane, cone.Height, cone.Radius


    [<Extension>]
    ///<summary>Returns the curvature of a Surface at a U, V parameter. See Rhino help
    ///    for details of Surface curvature</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="parameter">(float * float) U, v parameter</param>
    ///<returns>(Point3d * Vector3d * float * Vector3d * float * Vector3d * float * float) of curvature information
    ///    [0]   point at specified U, V parameter
    ///    [1]   normal direction
    ///    [2]   maximum principal curvature
    ///    [3]   maximum principal curvature direction
    ///    [4]   minimum principal curvature
    ///    [5]   minimum principal curvature direction
    ///    [6]   gaussian curvature
    ///    [7]   mean curvature.</returns>
    static member SurfaceCurvature(surfaceId:Guid, parameter:float * float) : Point3d * Vector3d * float * Vector3d * float * Vector3d * float * float=
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        //if Seq.length(parameter)<2 then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceCurvature failed.  surfaceId:'%s' parameter:'%A'" (rhType surfaceId) parameter
        let c = surface.CurvatureAt(parameter|> fst, parameter|> snd)
        if c|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceCurvature failed.  surfaceId:'%s' parameter:'%A'" (rhType surfaceId) parameter
        c.Point, c.Normal, c.Kappa(0), c.Direction(0), c.Kappa(1), c.Direction(1), c.Gaussian, c.Mean


    [<Extension>]
    ///<summary>Returns the definition of a cylinder Surface</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>(Plane * float * float) of the cylinder Plane, height and radius.</returns>
    static member SurfaceCylinder(surfaceId:Guid) : Plane * float * float =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let tol = Doc.ModelAbsoluteTolerance
        let cy = ref Cylinder.Unset
        let rc = surface.TryGetFiniteCylinder(cy, tol)
        if rc  then
            let cylinder = !cy
            cylinder.BasePlane, cylinder.TotalHeight, cylinder.Radius
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceCylinder failed on %A" surfaceId


    [<Extension>]
    ///<summary>Returns the U and V degrees of a Surface</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>(int*int) The degree in U and V direction.</returns>
    static member SurfaceDegree(surfaceId:Guid ) : int*int =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        surface.Degree(0), surface.Degree(1)


    [<Extension>]
    ///<summary>Returns the domain of a Surface object in the specified direction</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="direction">(int) Domain direction 0 = U, or 1 = V</param>
    ///<returns>(float * float) containing the domain interval in the specified direction.</returns>
    static member SurfaceDomain(surfaceId:Guid, direction:int) : float * float =
        if direction <> 0 && direction <> 1 then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceDomain failed.  surfaceId:'%s' direction:'%A'" (rhType surfaceId) direction
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let domain = surface.Domain(direction)
        domain.T0, domain.T1


    [<Extension>]
    ///<summary>Returns the edit, or Greville points of a Surface object. For each
    ///    Surface control point, there is a corresponding edit point</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="returnAll">(bool) Optional, Default Value: <c>true</c>
    ///    If True, all Surface edit points are returned. If False,
    ///    the function will return Surface edit points based on whether or not the
    ///    Surface is closed or periodic</param>
    ///<returns>(Point3d Rarr) a list of 3D points.</returns>
    static member SurfaceEditPoints( surfaceId:Guid, [<OPT;DEF(true)>]returnAll:bool) : Point3d Rarr =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let nurb = surface.ToNurbsSurface()
        if isNull nurb then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceEditPoints failed.  surfaceId:'%s'  returnAll:'%A'" (rhType surfaceId) returnAll
        let mutable ufirst = 0
        let mutable ulast = nurb.Points.CountU
        let mutable vfirst = 0
        let mutable vlast = nurb.Points.CountV
        let mutable degree = -1
        if not returnAll then
            if nurb.IsClosed(0) then ulast <- nurb.Points.CountU-1
            if nurb.IsPeriodic(0) then
                degree <- nurb.Degree(0)
                ufirst <- degree/2
                ulast <- nurb.Points.CountU-degree + ufirst
            if nurb.IsClosed(1) then vlast <- nurb.Points.CountV-1
            if nurb.IsPeriodic(1) then
                degree <- nurb.Degree(1)
                vfirst <- degree/2
                vlast <- nurb.Points.CountV-degree + vfirst
        rarr{
            for u = ufirst to ulast-1 do
                for v = vfirst to  vlast-1 do
                    let pt = nurb.Points.GetGrevillePoint(u, v)
                    nurb.PointAt(pt.X, pt.Y)
                    }
    [<Extension>]
    ///<summary>Returns the parameters at edit, or Greville points of a Surface object. For each
    ///    Surface control point, there is a corresponding edit point</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="returnAll">(bool) Optional, Default Value: <c>true</c>
    ///    If True, all Surface edit points are returned. If False,
    ///    the function will return Surface edit points based on whether or not the
    ///    Surface is closed or periodic</param>
    ///<returns>((float*float) Rarr) a list of U and V parameters.</returns>
    static member SurfaceEditPointPrameters( surfaceId:Guid, [<OPT;DEF(true)>]returnAll:bool) : (float*float) Rarr =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let nurb = surface.ToNurbsSurface()
        if isNull nurb then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceEditPointParameterss failed.  surfaceId:'%s'  returnAll:'%A'" (rhType surfaceId) returnAll
        let mutable ufirst = 0
        let mutable ulast = nurb.Points.CountU
        let mutable vfirst = 0
        let mutable vlast = nurb.Points.CountV
        let mutable degree = -1
        if not returnAll then
            if nurb.IsClosed(0) then ulast <- nurb.Points.CountU-1
            if nurb.IsPeriodic(0) then
                degree <- nurb.Degree(0)
                ufirst <- degree/2
                ulast <- nurb.Points.CountU-degree + ufirst
            if nurb.IsClosed(1) then vlast <- nurb.Points.CountV-1
            if nurb.IsPeriodic(1) then
                degree <- nurb.Degree(1)
                vfirst <- degree/2
                vlast <- nurb.Points.CountV-degree + vfirst
        rarr{
            for u = ufirst to ulast-1 do
                for v = vfirst to  vlast-1 do
                    let pt = nurb.Points.GetGrevillePoint(u, v)
                    pt.X, pt.Y
                    }



    [<Extension>]
    ///<summary>A general purpose Surface evaluator</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="parameter">(float * float) U, v parameter to evaluate</param>
    ///<param name="derivative">(int) Number of derivatives to evaluate</param>
    ///<returns>(Point3d * Vector3d Rarr) list length (derivative + 1)*(derivative + 2)/2 .  The elements are as follows:
    ///    firts Element of tuple
    ///    [fst]      The 3-D point.
    ///    [snd]   Vectors in List:
    ///            [0]      The first derivative.
    ///            [1]      The first derivative.
    ///            [2]      The second derivative.
    ///            [3]      The second derivative.
    ///            [4]      The second derivative.
    ///            [5]      etc...
    ///.</returns>
    static member SurfaceEvaluate( surfaceId:Guid,
                                   parameter:float * float,
                                   derivative:int) : Point3d * Vector3d Rarr =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let success, point, der = surface.Evaluate(parameter|> fst, parameter|> snd, derivative)
        if not success then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceEvaluate failed.  surfaceId:'%s' parameter:'%A' derivative:'%A'" (rhType surfaceId) parameter derivative
        let rc = rarr{()}
        if der.Length > 0 then
          for d in der do rc.Add(d)
        point, rc


    [<Extension>]
    ///<summary>Returns a Plane based on the normal, u, and v directions at a Surface
    ///    U, V parameter</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="uvParameter">(float * float) U, v parameter to evaluate</param>
    ///<returns>(Plane) Plane.</returns>
    static member SurfaceFrame(surfaceId:Guid, uvParameter:float * float) : Plane =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let rc, frame = surface.FrameAt(uvParameter|> fst, uvParameter|> snd)
        if rc  then frame
        else RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceFrame failed on %A at %A" surfaceId uvParameter


    [<Extension>]
    ///<summary>Returns the isocurve density of a Surface or Polysurface object.
    /// An isoparametric Curve is a Curve of constant U or V value on a Surface.
    /// Rhino uses isocurves and Surface edge Curves to visualize the shape of a
    /// NURBS Surface</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>(int) The current isocurve density
    ///      -1: Hides the Surface isocurves
    ///      0: Display boundary and knot wires
    ///      1: Display boundary and knot wires and one interior wire if there are no interior knots
    ///      bigger than 1: Display boundary and knot wires and (N + 1) interior wires.</returns>
    static member SurfaceIsocurveDensity(surfaceId:Guid) : int = //GET
        match RhinoScriptSyntax.CoerceRhinoObject(surfaceId) with
        | :?  DocObjects.BrepObject as rhinoobject ->
                rhinoobject.Attributes.WireDensity
        | :?  DocObjects.SurfaceObject as rhinoobject ->
                rhinoobject.Attributes.WireDensity
        | :?  DocObjects.ExtrusionObject as rhinoobject ->
                rhinoobject.Attributes.WireDensity
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceIsocurveDensity Get failed.  surfaceId:'%s'" (rhType surfaceId)


    [<Extension>]
    ///<summary>Sets the isocurve density of a Surface or Polysurface object.
    /// An isoparametric Curve is a Curve of constant U or V value on a Surface.
    /// Rhino uses isocurves and Surface edge Curves to visualize the shape of a
    /// NURBS Surface</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="density">(int) The isocurve wireframe density. The possible values are
    ///      -1: Hides the Surface isocurves
    ///      0: Display boundary and knot wires
    ///      1: Display boundary and knot wires and one interior wire if there are no interior knots
    ///      bigger than 1: Display boundary and knot wires and (N + 1) interior wires</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SurfaceIsocurveDensity(surfaceId:Guid, density:int) : unit = //SET
        match RhinoScriptSyntax.CoerceRhinoObject(surfaceId) with
        | :?  DocObjects.BrepObject as rhinoobject ->
                let dens = if density<0 then -1 else density
                rhinoobject.Attributes.WireDensity <- dens
                rhinoobject.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 
                Doc.Views.Redraw()
        | :?  DocObjects.SurfaceObject as rhinoobject ->
                let dens = if density<0 then -1 else density
                rhinoobject.Attributes.WireDensity <- dens
                rhinoobject.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 
                Doc.Views.Redraw()
        | :?  DocObjects.ExtrusionObject as rhinoobject ->
                let dens = if density<0 then -1 else density
                rhinoobject.Attributes.WireDensity <- dens
                rhinoobject.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 
                Doc.Views.Redraw()
        | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceIsocurveDensity Get failed.  surfaceId:'%s' density:'%A'" (rhType surfaceId) density


    [<Extension>]
    ///<summary>Sets the isocurve density of multiple Surface or Polysurface objects.
    /// An isoparametric Curve is a Curve of constant U or V value on a Surface.
    /// Rhino uses isocurves and Surface edge Curves to visualize the shape of a
    /// NURBS Surface</summary>
    ///<param name="surfaceIds">(Guid seq) The Surface's identifiers</param>
    ///<param name="density">(int) The isocurve wireframe density. The possible values are
    ///      -1: Hides the Surface isocurves
    ///      0: Display boundary and knot wires
    ///      1: Display boundary and knot wires and one interior wire if there are no interior knots
    ///      bigger than 1: Display boundary and knot wires and (N + 1) interior wires</param>
    ///<returns>(unit) void, nothing.</returns>
    static member SurfaceIsocurveDensity(surfaceIds:Guid seq, density:int) : unit = //MULTISET
        for surfaceId in surfaceIds do 
            match RhinoScriptSyntax.CoerceRhinoObject(surfaceId) with
            | :?  DocObjects.BrepObject as rhinoobject ->
                    let dens = if density<0 then -1 else density
                    rhinoobject.Attributes.WireDensity <- dens
                    rhinoobject.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 
            | :?  DocObjects.SurfaceObject as rhinoobject ->
                    let dens = if density<0 then -1 else density
                    rhinoobject.Attributes.WireDensity <- dens
                    rhinoobject.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 
            | :?  DocObjects.ExtrusionObject as rhinoobject ->
                    let dens = if density<0 then -1 else density
                    rhinoobject.Attributes.WireDensity <- dens
                    rhinoobject.CommitChanges() |> RhinoScriptingException.FailIfFalse "CommitChanges failed" 
            | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceIsocurveDensity Get failed.  surfaceId:'%s' density:'%A'" (rhType surfaceId) density
        Doc.Views.Redraw()

    [<Extension>]
    ///<summary>Returns the control point count of a Surface
    ///    SurfaceId = the Surface's identifier</summary>
    ///<param name="surfaceId">(Guid) The Surface object's identifier</param>
    ///<returns>(int * int) a list containing (U count, V count).</returns>
    static member SurfaceKnotCount(surfaceId:Guid) : int * int =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let ns = surface.ToNurbsSurface()
        ns.KnotsU.Count, ns.KnotsV.Count


    [<Extension>]
    ///<summary>Returns the knots, or knot vector, of a Surface object</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>(NurbsSurfaceKnotList * NurbsSurfaceKnotList) knot values of the Surface. 
    ///    The list will contain the following information:
    ///    Element   Description
    ///      [0]     Knot vectors in U direction
    ///      [1]     Knot vectors in V direction.</returns>
    static member SurfaceKnots(surfaceId:Guid) : Collections.NurbsSurfaceKnotList * Collections.NurbsSurfaceKnotList=
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let nurbsurf = surface.ToNurbsSurface()
        if nurbsurf|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceKnots failed.  surfaceId:'%s'" (rhType surfaceId)
        nurbsurf.KnotsU , nurbsurf.KnotsV
        //let sknots =  rarr { for knot in nurbsurf.KnotsU do yield knot }
        //let tknots =  rarr { for knot in nurbsurf.KnotsV do yield knot }
        //if isNull sknots || not tknots then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceKnots failed.  surfaceId:'%s'" (rhType surfaceId)
        //sknots, tknots


    [<Extension>]
    ///<summary>Returns 3D vector that is the normal to a Surface at a parameter</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="uvParameter">(float * float) The uv parameter to evaluate</param>
    ///<returns>(Vector3d) Normal vector.</returns>
    static member SurfaceNormal(surfaceId:Guid, uvParameter:float * float) : Vector3d =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        surface.NormalAt(uvParameter|> fst, uvParameter|> snd)

    [<Extension>]
    ///<summary>Returns 3D vector that is the normal to a Surface at mid parameter</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>(Vector3d) Normal vector.</returns>
    static member SurfaceNormal(surfaceId:Guid) : Vector3d =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let u = surface.Domain(0).ParameterAt(0.5)
        let v = surface.Domain(1).ParameterAt(0.5)
        surface.NormalAt(u,v)        


    [<Extension>]
    ///<summary>Converts Surface parameter to a normalized Surface parameter; one that
    ///    ranges between 0.0 and 1.0 in both the U and V directions</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="parameter">(float * float) The Surface parameter to convert</param>
    ///<returns>(float * float) normalized Surface parameter.</returns>
    static member SurfaceNormalizedParameter(surfaceId:Guid, parameter:float * float) : float * float =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let udomain = surface.Domain(0)
        let vdomain = surface.Domain(1)
        if parameter|> fst<udomain.Min || parameter|> fst>udomain.Max then
            RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceNormalizedParameter failed.  surfaceId:'%s' parameter:'%A'" (rhType surfaceId) parameter
        if parameter|> snd<vdomain.Min || parameter|> snd>vdomain.Max then
            RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceNormalizedParameter failed.  surfaceId:'%s' parameter:'%A'" (rhType surfaceId) parameter
        let u = udomain.NormalizedParameterAt(parameter|> fst)
        let v = vdomain.NormalizedParameterAt(parameter|> snd)
        u, v


    [<Extension>]
    ///<summary>Converts normalized Surface parameter to a Surface domain specific (regular) parameter.
    /// or a paranter within the Surface's domain</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="parameter">(float * float) The normalized parameter to convert</param>
    ///<returns>(float * float) u and v Surface parameters.</returns>
    static member SurfaceParameter(surfaceId:Guid, parameter:float * float) : float * float =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let x = surface.Domain(0).ParameterAt(parameter|> fst)
        let y = surface.Domain(1).ParameterAt(parameter|> snd)
        x, y


    [<Extension>]
    ///<summary>Returns the control point count of a Surface
    ///    SurfaceId = the Surface's identifier</summary>
    ///<param name="surfaceId">(Guid) The Surface object's identifier</param>
    ///<returns>(int * int) THe number of control points in UV direction. (U count, V count).</returns>
    static member SurfacePointCount(surfaceId:Guid) : int * int =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let ns = surface.ToNurbsSurface()
        ns.Points.CountU, ns.Points.CountV


    [<Extension>]
    ///<summary>Returns the control points, or control vertices, of a Surface object</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="returnAll">(bool) Optional, Default Value: <c>true</c>
    ///    If True all Surface edit points are returned. If False,
    ///    the function will return Surface edit points based on whether or not
    ///    the Surface is closed or periodic</param>
    ///<returns>(Point3d Rarr) The control points.</returns>
    static member SurfacePoints(surfaceId:Guid, [<OPT;DEF(true)>]returnAll:bool) : Point3d Rarr =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let ns = surface.ToNurbsSurface()
        if ns|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfacePoints failed.  surfaceId:'%s' returnAll:'%A'" (rhType surfaceId) returnAll
        let rc = Rarr()
        for u in range(ns.Points.CountU) do
            for v in range(ns.Points.CountV) do
                let pt = ns.Points.GetControlPoint(u, v)
                rc.Add(pt.Location)
        rc


    [<Extension>]
    ///<summary>Returns the definition of a Surface torus</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<returns>(Plane * float * float) containing the definition of the torus
    ///    [0]   the base Plane of the torus
    ///    [1]   the major radius of the torus
    ///    [2]   the minor radius of the torus.</returns>
    static member SurfaceTorus(surfaceId:Guid) : Plane * float * float =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let rc, torus = surface.TryGetTorus()
        if rc then torus.Plane, torus.MajorRadius, torus.MinorRadius
        else RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceTorus failed for %A" surfaceId


    [<Extension>]
    ///<summary>Calculates volume of a closed Surface or Polysurface</summary>
    ///<param name="objectId">(Guid) The Surface's identifier</param>
    ///<returns>(float) The volume.</returns>
    static member SurfaceVolume(objectId:Guid) : float =
        objectId
        |> RhinoScriptSyntax.TryCoerceBrep
        |> Option.map VolumeMassProperties.Compute
        |> Option.orElseWith (fun () ->
            objectId
            |> RhinoScriptSyntax.TryCoerceSurface
            |> Option.map VolumeMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceVolume failed on %A" (rhType objectId))
        |> fun amp -> amp.Volume


    [<Extension>]
    ///<summary>Calculates volume centroid of a closed Surface or Polysurface</summary>
    ///<param name="objectId">(Guid) The Surface's identifier</param>
    ///<returns>(Point3d) Volume Centriod.</returns>
    static member SurfaceVolumeCentroid(objectId:Guid) : Point3d =
        objectId
        |> RhinoScriptSyntax.TryCoerceBrep
        |> Option.bind (fun b -> if b.IsSolid then Some b else RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceVolumeCentroid failed on  open Brep %A" (rhType objectId))
        |> Option.map VolumeMassProperties.Compute
        |> Option.orElseWith (fun () ->
            objectId
            |> RhinoScriptSyntax.TryCoerceSurface
            |> Option.bind (fun s -> if s.IsSolid then Some s else RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceVolumeCentroid failed on  open Surface %A" (rhType objectId))
            |> Option.map VolumeMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceVolumeCentroid failed on %A" (rhType objectId))
        |> fun amp -> amp.Centroid


    [<Extension>]
    ///<summary>Calculates volume moments of inertia of a Surface or Polysurface object.
    ///    For more information, see Rhino help for "Mass Properties calculation details"</summary>
    ///<param name="objectId">(Guid) The Surface's identifier</param>
    ///<returns>((float*float*float) Rarr) of moments and error bounds in tuple(X, Y, Z) - see help topic
    ///    Index   Description
    ///    [0]     First Moments.
    ///    [1]     The absolute (+/-) error bound for the First Moments.
    ///    [2]     Second Moments.
    ///    [3]     The absolute (+/-) error bound for the Second Moments.
    ///    [4]     Product Moments.
    ///    [5]     The absolute (+/-) error bound for the Product Moments.
    ///    [6]     Area Moments of Inertia about the World Coordinate Axes.
    ///    [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
    ///    [8]     Area Radii of Gyration about the World Coordinate Axes.
    ///    [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///    [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///    [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.</returns>
    static member SurfaceVolumeMoments(objectId:Guid) : (float*float*float) Rarr =
        objectId
        |> RhinoScriptSyntax.TryCoerceBrep
        |> Option.bind (fun b -> if b.IsSolid then Some b else RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceVolumeMoments failed on  open Brep %A" (rhType objectId))
        |> Option.map VolumeMassProperties.Compute
        |> Option.orElseWith (fun () ->
            objectId
            |> RhinoScriptSyntax.TryCoerceSurface
            |> Option.bind (fun s -> if s.IsSolid then Some s else RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceVolumeMoments failed on  open Surface %A" (rhType objectId))
            |> Option.map VolumeMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceVolumeMoments failed on %A" (rhType objectId))
        |> fun mp ->
            rarr{
                yield (mp.WorldCoordinatesFirstMoments.X, mp.WorldCoordinatesFirstMoments.Y, mp.WorldCoordinatesFirstMoments.Z)                                     //  [0]     First Moments.
                yield (mp.WorldCoordinatesFirstMomentsError.X, mp.WorldCoordinatesFirstMomentsError.Y, mp.WorldCoordinatesFirstMomentsError.Z)                      //  [1]     The absolute (+/-) error bound for the First Moments.
                yield (mp.WorldCoordinatesSecondMoments.X, mp.WorldCoordinatesSecondMoments.Y, mp.WorldCoordinatesSecondMoments.Z)                                  //  [2]     Second Moments.
                yield (mp.WorldCoordinatesSecondMomentsError.X, mp.WorldCoordinatesSecondMomentsError.Y, mp.WorldCoordinatesSecondMomentsError.Z)                   //  [3]     The absolute (+/-) error bound for the Second Moments.
                yield (mp.WorldCoordinatesProductMoments.X, mp.WorldCoordinatesProductMoments.Y, mp.WorldCoordinatesProductMoments.Z)                               //  [4]     Product Moments.
                yield (mp.WorldCoordinatesProductMomentsError.X, mp.WorldCoordinatesProductMomentsError.Y, mp.WorldCoordinatesProductMomentsError.Z)                //  [5]     The absolute (+/-) error bound for the Product Moments.
                yield (mp.WorldCoordinatesMomentsOfInertia.X, mp.WorldCoordinatesMomentsOfInertia.Y, mp.WorldCoordinatesMomentsOfInertia.Z)                         //  [6]     Area Moments of Inertia about the World Coordinate Axes.
                yield (mp.WorldCoordinatesMomentsOfInertiaError.X, mp.WorldCoordinatesMomentsOfInertiaError.Y, mp.WorldCoordinatesMomentsOfInertiaError.Z)          //  [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
                yield (mp.WorldCoordinatesRadiiOfGyration.X, mp.WorldCoordinatesRadiiOfGyration.Y, mp.WorldCoordinatesRadiiOfGyration.Z)                            //  [8]     Area Radii of Gyration about the World Coordinate Axes.
                yield (0., 0., 0.) // need to add error calc to RhinoCommon                                                                                           //  [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
                yield (mp.CentroidCoordinatesMomentsOfInertia.X, mp.CentroidCoordinatesMomentsOfInertia.Y, mp.CentroidCoordinatesMomentsOfInertia.Z)                //  [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
                yield (mp.CentroidCoordinatesMomentsOfInertiaError.X, mp.CentroidCoordinatesMomentsOfInertiaError.Y, mp.CentroidCoordinatesMomentsOfInertiaError.Z) //  [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
                yield (mp.CentroidCoordinatesRadiiOfGyration.X, mp.CentroidCoordinatesRadiiOfGyration.Y, mp.CentroidCoordinatesRadiiOfGyration.Z)                   //  [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
                yield (0., 0., 0.) //need to add error calc to RhinoCommon                                                                                            //  [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes</returns>
                }



    [<Extension>]
    ///<summary>Returns list of weight values assigned to the control points of a Surface.
    ///    The number of weights returned will be equal to the number of control points
    ///    in the U and V directions</summary>
    ///<param name="objectId">(Guid) The Surface's identifier</param>
    ///<returns>(float Rarr) point weights.</returns>
    static member SurfaceWeights(objectId:Guid) : float Rarr =
        let surface = RhinoScriptSyntax.CoerceSurface(objectId)
        let ns = surface.ToNurbsSurface()
        if ns|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.SurfaceWeights failed.  objectId:'%s'" (rhType objectId)
        let rc = Rarr()
        for u in range(ns.Points.CountU) do
            for v in range(ns.Points.CountV) do
                let pt = ns.Points.GetControlPoint(u, v)
                rc.Add(pt.Weight)
        rc

    [<Extension>]
    ///<summary>Trims a Surface or Polysurface using an oriented cutter brep or Surface</summary>
    ///<param name="objectId">(Guid) Surface or Polysurface identifier</param>
    ///<param name="cutter">(Guid) Surface or Polysurface  performing the trim</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(Guid Rarr) identifiers of retained components.</returns>
    static member TrimBrep( objectId:Guid,
                            cutter:Guid,
                            [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr =
        let brep = RhinoScriptSyntax.CoerceBrep(objectId)
        let cutter = RhinoScriptSyntax.CoerceBrep(cutter)
        let tolerance= ifZero1 tolerance  Doc.ModelAbsoluteTolerance
        let breps = brep.Trim(cutter, tolerance)
        let attrs = None
        if breps.Length > 1 then
            let rho = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let attrs = rho.Attributes
            let rc = Rarr()
            for i in range(breps.Length) do
                if i = 0 then
                    Doc.Objects.Replace(objectId, breps.[i]) |> ignore
                    rc.Add(objectId)
                else
                    rc.Add(Doc.Objects.AddBrep(breps.[i], attrs))
            Doc.Views.Redraw()
            rc
        else
            let rc =  rarr { for brep in breps do yield Doc.Objects.AddBrep(brep) }
            Doc.Views.Redraw()
            rc

    [<Extension>]
    ///<summary>Trims a Surface using an oriented cutter Plane</summary>
    ///<param name="objectId">(Guid) Surface or Polysurface identifier</param>
    ///<param name="cutter">(Plane) Plane performing the trim</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c></param>
    ///<returns>(Guid Rarr) identifiers of retained components.</returns>
    static member TrimBrep( objectId:Guid,
                            cutter:Plane,
                            [<OPT;DEF(0.0)>]tolerance:float) : Guid Rarr =
        let brep = RhinoScriptSyntax.CoerceBrep(objectId)
        let tolerance= ifZero1 tolerance  Doc.ModelAbsoluteTolerance
        let breps = brep.Trim(cutter, tolerance)
        let attrs = None
        if breps.Length > 1 then
            let rho = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let attrs = rho.Attributes
            let rc = Rarr()
            for i in range(breps.Length) do
                if i = 0 then
                    Doc.Objects.Replace(objectId, breps.[i]) |> ignore
                    rc.Add(objectId)
                else
                    rc.Add(Doc.Objects.AddBrep(breps.[i], attrs))
            Doc.Views.Redraw()
            rc
        else
            let rc =  rarr { for brep in breps do yield Doc.Objects.AddBrep(brep) }
            Doc.Views.Redraw()
            rc

    [<Extension>]
    ///<summary>Remove portions of the Surface outside of the specified interval in U direction</summary>
    ///<param name="surfaceId">(Guid) Surface identifier</param>
    ///<param name="interval">(float*float) Sub section of the Surface to keep.</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Should the input Surface be deleted</param>
    ///<returns>(Guid) new Surface identifier.</returns>
    static member TrimSurfaceU( surfaceId:Guid,
                                interval:float*float,
                                [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let mutable u = surface.Domain(0)
        let mutable v = surface.Domain(1)
        u.[0] <-  interval|> fst
        u.[1] <-  interval|> snd
        let newsurface = surface.Trim(u, v)
        if notNull newsurface then
            let rc = Doc.Objects.AddSurface(newsurface)
            if deleteInput then  Doc.Objects.Delete(surfaceId, true) |> ignore
            Doc.Views.Redraw()
            rc
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.TrimSurfaceU faild on %A with domain %A" surfaceId interval

    [<Extension>]
    ///<summary>Remove portions of the Surface outside of the specified interval in V direction</summary>
    ///<param name="surfaceId">(Guid) Surface identifier</param>
    ///<param name="interval">(float*float) Sub section of the Surface to keep.</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Should the input Surface be deleted</param>
    ///<returns>(Guid) new Surface identifier.</returns>
    static member TrimSurfaceV( surfaceId:Guid,
                               interval:float*float,
                               [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let mutable u = surface.Domain(0)
        let mutable v = surface.Domain(1)
        v.[0] <-  interval|> fst
        v.[1] <-  interval|> snd
        let newsurface = surface.Trim(u, v)
        if notNull newsurface then
            let rc = Doc.Objects.AddSurface(newsurface)
            if deleteInput then  Doc.Objects.Delete(surfaceId, true) |> ignore
            Doc.Views.Redraw()
            rc
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.TrimSurfaceV faild on %A with domain %A" surfaceId interval


    [<Extension>]
    ///<summary>Remove portions of the Surface outside of the specified interval ain U and V direction</summary>
    ///<param name="surfaceId">(Guid) Surface identifier</param>
    ///<param name="intervalU">(float*float) Sub section of the Surface to keep in U direction</param>
    ///<param name="intervalV">(float*float) Sub section of the Surface to keep in V direction</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///    Should the input Surface be deleted</param>
    ///<returns>(Guid) new Surface identifier.</returns>
    static member TrimSurfaceUV( surfaceId:Guid,
                               intervalU:float*float,
                               intervalV:float*float,
                               [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let mutable u = surface.Domain(0)
        let mutable v = surface.Domain(1)
        u.[0]  <- intervalU|> fst
        u.[1]  <- intervalU|> snd
        v.[0]  <- intervalV|> fst
        v.[1]  <- intervalV|> snd
        let newsurface = surface.Trim(u, v)
        if notNull newsurface then
            let rc = Doc.Objects.AddSurface(newsurface)
            if deleteInput then  Doc.Objects.Delete(surfaceId, true) |> ignore
            Doc.Views.Redraw()
            rc
        else
            RhinoScriptingException.Raise "RhinoScriptSyntax.TrimSurfaceUV faild on %A with domain %A and %A" surfaceId intervalU intervalV



    [<Extension>]
    ///<summary>Flattens a developable Surface or Polysurface</summary>
    ///<param name="surfaceId">(Guid) The Surface's identifier</param>
    ///<param name="explode">(bool) Optional, Default Value: <c>false</c>
    ///    If True, the resulting Surfaces are not joined</param>
    ///<param name="followingGeometry">(Guid seq) Optional, List of Curves, dots, and points which
    ///    should be unrolled with the Surface</param>
    ///<param name="absoluteTolerance">(float) Optional, Default Value: <c>Doc.ModelAbsoluteTolerance</c>
    ///    Absolute tolerance</param>
    ///<param name="relativeTolerance">(float) Optional, Default Value: <c>Doc.ModelRelativeTolerance</c>
    ///    Relative tolerance</param>
    ///<returns>(Guid Rarr * Guid Rarr) Two lists: List of unrolled Surface ids and list of following objects.</returns>
    static member UnrollSurface( surfaceId:Guid,
                                 [<OPT;DEF(false)>]explode:bool,
                                 [<OPT;DEF(null:Guid seq)>]followingGeometry:Guid seq,
                                 [<OPT;DEF(0.0)>]absoluteTolerance:float,
                                 [<OPT;DEF(0.0)>]relativeTolerance:float) : Guid Rarr * Guid Rarr =
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        let unroll = Unroller(brep)
        unroll.ExplodeOutput <- explode
        let relativeTolerance = ifZero1 relativeTolerance  Doc.ModelRelativeTolerance
        let  absoluteTolerance = ifZero1 absoluteTolerance  Doc.ModelAbsoluteTolerance
        unroll.AbsoluteTolerance <- absoluteTolerance
        unroll.RelativeTolerance <- relativeTolerance
        if notNull followingGeometry then
            for objectId in followingGeometry do
                let geom = RhinoScriptSyntax.CoerceGeometry(objectId)
                match geom with
                | :? Curve as g -> unroll.AddFollowingGeometry(g) //TODO verify order is correct ???
                | :? Point as g -> unroll.AddFollowingGeometry(g)
                | :? TextDot as g -> unroll.AddFollowingGeometry(g)
                | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.UnrollSurface: cannot add (a %s) as following Geometry" (rhType objectId)

        let breps, curves, points, dots = unroll.PerformUnroll()
        if isNull breps then RhinoScriptingException.Raise "RhinoScriptSyntax.UnrollSurface: failed on  %A" surfaceId
        let rc =  rarr { for brep in breps do yield Doc.Objects.AddBrep(brep) }
        let newfollowing = Rarr()
        for curve in curves do
            let objectId = Doc.Objects.AddCurve(curve) //TODO verify order is correct ???
            newfollowing.Add(objectId)
        for point in points do
            let objectId = Doc.Objects.AddPoint(point)
            newfollowing.Add(objectId)
        for dot in dots do
            let objectId = Doc.Objects.AddTextDot(dot)
            newfollowing.Add(objectId)
        Doc.Views.Redraw()
        rc, newfollowing


    [<Extension>]
    ///<summary>Changes the degree of a Surface object.  For more information see the Rhino help file for the ChangeDegree command</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="degree">(int * int) Two integers, specifying the degrees for the U  V directions</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member ChangeSurfaceDegree(objectId:Guid, degree:int * int) : bool =
        let surface = RhinoScriptSyntax.CoerceNurbsSurface(objectId)
        let u, v = degree
        let maxnurbsdegree = 11
        if u < 1 || u > maxnurbsdegree || v < 1 || v > maxnurbsdegree ||  (surface.Degree(0) = u && surface.Degree(1) = v) then RhinoScriptingException.Raise "RhinoScriptSyntax.ChangeSurfaceDegree failed on %A" (rhType objectId)
        let mutable r = false
        if surface.IncreaseDegreeU(u) then
            if surface.IncreaseDegreeV(v) then
                r <- Doc.Objects.Replace(objectId, surface)
        r


