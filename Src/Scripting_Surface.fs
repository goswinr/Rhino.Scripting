
namespace Rhino.Scripting

open Rhino
open System.ComponentModel
open System
open System.Collections.Generic
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils


[<AutoOpen; EditorBrowsable(EditorBrowsableState.Never)>]
module AutoOpenSurface =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    /// <summary>Adds a box shaped Polysurface to the document.</summary>
    /// <param name="corners">(Point3d seq) 8 points that define the corners of the box. Points need to
    ///    be in counter-clockwise order starting with the bottom rectangle of the box</param>
    /// <returns>(Guid) identifier of the new object.</returns>
    static member AddBox(corners:Point3d seq) : Guid =
        let brep = Brep.CreateFromBox(corners)
        if isNull brep then RhinoScriptingException.Raise "AddBox: Unable to create brep from box.  %d corners:'%A'" (Seq.length corners) corners
        let rc = State.Doc.Objects.AddBrep(brep)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddBox: Unable to add brep to document. corners:'%A'" corners
        State.Doc.Views.Redraw()
        rc

    /// <summary>Adds a cone shaped Polysurface to the document.
    /// The cone will have it's tip at the origin of the given plane. And grow along its Z axis.
    /// So it will be an upside down cone on world XY plane</summary>
    /// <param name="basis">(Plane) The plane at the tip of the cone. From this tip the cone wil expand along the Z axis. </param>
    /// <param name="height">(float) The height of cone. </param>
    /// <param name="radius">(float) The radius at the basis of the cone.</param>
    /// <param name="cap">(bool) Optional, default value: <c>true</c>. Add capping surface at basis of the cone?</param>
    /// <returns>(Guid) identifier of the new object.</returns>
    static member AddCone( basis:Plane,
                           height:float,
                           radius:float,
                           [<OPT;DEF(true)>]cap:bool) : Guid =
        let cone = Cone(basis, height, radius)
        let brep = Brep.CreateFromCone(cone, cap)// cone is upside down
        let rc = State.Doc.Objects.AddBrep(brep)
        State.Doc.Views.Redraw()
        rc

    /// <summary>Adds a cone shaped Polysurface to the document.
    /// The cone will have it's base center at the first point and the tip at the second point</summary>
    /// <param name="baseCenter">(Point3d) The point in the center of the base.</param>
    /// <param name="tip">(Point3d) The tip of cone. </param>
    /// <param name="radius">(float) The radius at the basis of the cone.</param>
    /// <param name="cap">(bool) Optional, default value: <c>true</c>. Add capping surface at basis of the cone?</param>
    /// <returns>(Guid) identifier of the new object.</returns>
    static member AddCone( baseCenter:Point3d,
                           tip:Point3d,
                           radius:float,
                           [<OPT;DEF(true)>]cap:bool) : Guid =
        let n = baseCenter-tip
        let pl = RhinoScriptSyntax.PlaneFromNormal(tip,n)
        let cone = Cone(pl, n.Length, radius)
        let brep = Brep.CreateFromCone(cone, cap)// cone is upside down
        let rc = State.Doc.Objects.AddBrep(brep)
        State.Doc.Views.Redraw()
        rc



    /// <summary>Adds a planar Surface through objects at a designated location. For more
    ///    information, see the Rhino help file for the CutPlane command.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of objects that the cutting Plane will
    ///    pass through</param>
    /// <param name="startPoint">(Point3d) Start point of line that defines the cutting Plane</param>
    /// <param name="endPoint">(Point3d) End point of line that defines the cutting Plane</param>
    /// <param name="normal">(Vector3d) Optional, default value: <c>world Z axis</c>
    ///    Vector that will be contained in the returned planar Surface.
    ///    If omitted, the world Z axis is used (NOT the normal to, or Z axis of, the active view's construction plane as in rhinopython).</param>
    /// <returns>(Guid) identifier of new object.</returns>
    static member AddCutPlane( objectIds:Guid seq,
                               startPoint:Point3d,
                               endPoint:Point3d,
                               [<OPT;DEF(Vector3d())>]normal:Vector3d) : Guid =
        let objectIdsGeometry = objectIds |> RArr.mapSeq RhinoScriptSyntax.CoerceGeometry
        let geometry = RhinoScriptSyntax.MakeCutPlane(objectIdsGeometry, startPoint, endPoint, normal)
        let rc = State.Doc.Objects.AddSurface(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddCutPlane: unable to add geometry to the document."
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a planar Surface through objects at a designated location. For more
    ///    information, see the Rhino help file for the CutPlane command. Returns geometry without adding it.</summary>
    /// <param name="geometryObjects">(GeometryBase seq) Objects of objects that the cutting Plane will
    ///    pass through</param>
    /// <param name="startPoint">(Point3d) Start point of line that defines the cutting Plane</param>
    /// <param name="endPoint">(Point3d) End point of line that defines the cutting Plane</param>
    /// <param name="normal">(Vector3d) Optional, default value: <c>world Z axis</c>
    ///    Vector that will be contained in the returned planar Surface.
    ///    If omitted, the world Z axis is used (NOT the normal to, or Z axis of, the active view's construction plane as in rhinopython).</param>
    /// <returns>(PlaneSurface) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeCutPlane( geometryObjects:GeometryBase seq,
                               startPoint:Point3d,
                               endPoint:Point3d,
                               [<OPT;DEF(Vector3d())>]normal:Vector3d) : PlaneSurface =
        let bbox =
            let mutable bbox0 = BoundingBox.Unset
            for geometry in geometryObjects do bbox0.Union(geometry.GetBoundingBox(true))
            if not bbox0.IsValid then
                RhinoScriptingException.Raise "MakeCutPlane GetTightBoundingBox failed.startPoint:'%A' endPoint:'%A' normal:'%A' geometryObjects:%s "startPoint endPoint normal (Pretty.str geometryObjects)
            let mutable bboxMin = bbox0.Min
            let mutable bboxMax = bbox0.Max
            for i=0 to 2 do
                if (abs(bboxMin.[i] - bboxMax.[i]) < RhinoMath.SqrtEpsilon)then
                    bboxMin.[i] <- bboxMin.[i] - 1.0
                    bboxMax.[i] <- bboxMax.[i] + 1.0
            let mutable v = bboxMax - bboxMin
            v <- v * 1.1
            let p = bboxMin + v
            bboxMin <- bboxMax - v
            bboxMax <- p
            Rhino.Geometry.BoundingBox(bboxMin, bboxMax)
        let line = Geometry.Line(startPoint, endPoint)
        let normal = if normal.IsZero then Vector3d.ZAxis else normal // TODO or use original ?? : scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane().Normal
        let surface = Rhino.Geometry.PlaneSurface.CreateThroughBox(line, normal, bbox)
        if surface|> isNull  then RhinoScriptingException.Raise "MakeCutPlane failed.  geometryObjects:'%A' startPoint:'%A' endPoint:'%A' normal:'%A'" (Pretty.str geometryObjects) startPoint endPoint normal
        surface


    /// <summary>Adds a cylinder-shaped Polysurface to the document.</summary>
    /// <param name="basis">(Plane) The 3D basis point of the cylinder or the basis Plane of the cylinder</param>
    /// <param name="height">(float) If basis is a point, then height is a 3D height point of the
    ///    cylinder. The height point defines the height and direction of the
    ///    cylinder. If basis is a Plane, then height is the numeric height value
    ///    of the cylinder</param>
    /// <param name="radius">(float) Radius of the cylinder</param>
    /// <param name="cap">(bool) Optional, default value: <c>true</c>
    ///    Cap the cylinder</param>
    /// <returns>(Guid) identifier of new object.</returns>
    static member AddCylinder( basis:Plane,
                               height:float,
                               radius:float,
                               [<OPT;DEF(true)>]cap:bool) : Guid =
        let circle = Circle(basis, radius)
        let cylinder = Cylinder(circle, height)
        let brep = cylinder.ToBrep(cap, cap)
        let objectId = State.Doc.Objects.AddBrep(brep)
        if objectId = Guid.Empty then RhinoScriptingException.Raise "AddCylinder failed.  basis:'%A' height:'%A' radius:'%A' cap:'%A'" basis height radius cap
        State.Doc.Views.Redraw()
        objectId


    /// <summary>Creates a Surface from 2, 3, or 4 edge Curves.</summary>
    /// <param name="curveIds">(Guid seq) List of Curves</param>
    /// <returns>(Guid) identifier of new object.</returns>
    static member AddEdgeSrf(curveIds:Guid seq) : Guid =
        let curveIdsGeometry = curveIds |> RArr.mapSeq RhinoScriptSyntax.CoerceCurve
        let geometry = RhinoScriptSyntax.MakeEdgeSrf(curveIdsGeometry)
        let rc = State.Doc.Objects.AddBrep(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddEdgeSrf: unable to add geometry to the document."
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a Surface from 2, 3, or 4 edge Curves. Returns geometry without adding it.</summary>
    /// <param name="curveGeos">(Curve seq) List of Curves</param>
    /// <returns>(Brep) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeEdgeSrf(curveGeos:Curve seq) : Brep =
        let curves  = curveGeos |> RArr.mapSeq (fun geometry -> geometry)
        let brep = Brep.CreateEdgeSurface(curves)
        if brep|> isNull  then RhinoScriptingException.Raise "MakeEdgeSrf failed.  curveGeos:'%s'" (Pretty.str curveGeos)
        brep


    /// <summary>Creates a Surface from a network of crossing Curves.</summary>
    /// <param name="curves">(Guid seq) Curves from which to create the Surface</param>
    /// <param name="continuity">(int) Optional, default value: <c>1</c>
    ///    How the edges match the input geometry
    ///    0 = loose
    ///    1 = position
    ///    2 = tangency
    ///    3 = curvature</param>
    /// <param name="edgeTolerance">(float) Optional, Edge tolerance</param>
    /// <param name="interiorTolerance">(float) Optional, Interior tolerance</param>
    /// <param name="angleTolerance">(float) Optional, Angle tolerance , in radians?</param>
    /// <returns>(Guid) identifier of new object.</returns>
    static member AddNetworkSrf( curves:Guid seq,
                                 [<OPT;DEF(1)>]continuity:int,
                                 [<OPT;DEF(0.0)>]edgeTolerance:float,
                                 [<OPT;DEF(0.0)>]interiorTolerance:float,
                                 [<OPT;DEF(0.0)>]angleTolerance:float) : Guid =
        let curvesGeometry = curves |> RArr.mapSeq RhinoScriptSyntax.CoerceCurve
        let geometry = RhinoScriptSyntax.MakeNetworkSrf(curvesGeometry, continuity, edgeTolerance, interiorTolerance, angleTolerance)
        let rc = State.Doc.Objects.AddSurface(geometry)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a Surface from a network of crossing Curves. Returns geometry without adding it.</summary>
    /// <param name="curves">(Curve seq) Curves from which to create the Surface</param>
    /// <param name="continuity">(int) Optional, default value: <c>1</c>
    ///    How the edges match the input geometry
    ///    0 = loose
    ///    1 = position
    ///    2 = tangency
    ///    3 = curvature</param>
    /// <param name="edgeTolerance">(float) Optional, Edge tolerance</param>
    /// <param name="interiorTolerance">(float) Optional, Interior tolerance</param>
    /// <param name="angleTolerance">(float) Optional, Angle tolerance , in radians?</param>
    /// <returns>(NurbsSurface) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeNetworkSrf( curves:Curve seq,
                                 [<OPT;DEF(1)>]continuity:int,
                                 [<OPT;DEF(0.0)>]edgeTolerance:float,
                                 [<OPT;DEF(0.0)>]interiorTolerance:float,
                                 [<OPT;DEF(0.0)>]angleTolerance:float) : NurbsSurface =
        let curves  = curves |> RArr.mapSeq (fun geometry -> geometry)
        let surf, _ = NurbsSurface.CreateNetworkSurface(curves, continuity, edgeTolerance, interiorTolerance, angleTolerance)// 0.0 Tolerance OK ? TODO
        if notNull surf then
            surf
        else
            RhinoScriptingException.Raise "MakeNetworkSrf failed on %A" curves


    /// <summary>Adds a NURBS Surface object to the document.</summary>
    /// <param name="pointCount">(int * int) Number of control points in the u and v direction</param>
    /// <param name="points">(Point3d IList) List of 3D points</param>
    /// <param name="knotsU">(float IList) List of Knot values for the Surface in the u direction.
    ///    Must contain pointCount[0]+degree[0]-1 elements</param>
    /// <param name="knotsV">(float IList) List of Knot values for the Surface in the v direction.
    ///    Must contain pointCount[1]+degree[1]-1 elements</param>
    /// <param name="degree">(int * int) Degree of the Surface in the u and v directions</param>
    /// <param name="weights">(float IList) Optional, List of Weight values for the Surface. The number of elements in
    ///    weights must equal the number of elements in points. Values must be greater than zero</param>
    /// <returns>(Guid) identifier of new object.</returns>
    static member AddNurbsSurface( pointCount:int * int,
                                   points:Point3d IList,
                                   knotsU:float seq,
                                   knotsV:float seq,
                                   degree:int * int,
                                   [<OPT;DEF(null:float IList)>]weights:float IList) : Guid =
        let pu, pv = pointCount
        let du, dv = degree
        if points.Count < (pu*pv) then
            RhinoScriptingException.Raise "AddNurbsSurface failed.  pointCount:'%A' points:'%A' knotsU:'%A' knotsV:'%A' degree:'%A' weights:'%A'" pointCount points knotsU knotsV degree weights
        let ns = NurbsSurface.Create(3, notNull weights , du + 1, dv + 1, pu, pv)
        //add the points && weights
        let controlpoints = ns.Points
        let mutable index = 0

        if notNull weights then
            if weights.Count < (pu*pv) then
                RhinoScriptingException.Raise "AddNurbsSurface failed.  pointCount:'%A' points:'%A' knotsU:'%A' knotsV:'%A' degree:'%A' weights:'%A'" pointCount points knotsU knotsV degree weights
            for i = 0 to pu - 1 do
                for j = 0 to pv - 1 do
                    let cp = ControlPoint(points.[index], weights.[index])
                    controlpoints.SetControlPoint(i, j, cp)|> ignore<bool>
                    index <- index + 1
        else
            for i = 0 to pu - 1 do
                for j = 0 to pv - 1 do
                    let cp = ControlPoint(points.[index])
                    controlpoints.SetControlPoint(i, j, cp)|> ignore<bool>
                    index <- index + 1
        index <- 0
        for i = 0 to pu - 1 do
            for j = 0 to pv - 1 do
                if notNull weights then
                    let cp = ControlPoint(points.[index], weights.[index])
                    controlpoints.SetControlPoint(i, j, cp)|> ignore<bool>
                else
                    let cp = ControlPoint(points.[index])
                    controlpoints.SetControlPoint(i, j, cp)|> ignore<bool>
                index <- index + 1

        //add the knots
        let mutable i = 0
        for ku in knotsU do
            ns.KnotsU.[i] <-  ku
            i<-i+1
        if i<> ns.KnotsU.Count then RhinoScriptingException.Raise "AddNurbsSurface failed.  knotsU expected %d knots but input had %d" ns.KnotsU.Count i
        let mutable j = 0
        for kv in knotsV do
            ns.KnotsV.[j] <-  kv
            j<-j+1
        if j<> ns.KnotsV.Count then RhinoScriptingException.Raise "AddNurbsSurface failed.  knotsV expected %d knots but input had %d" ns.KnotsV.Count j


        if not ns.IsValid then RhinoScriptingException.Raise "AddNurbsSurface failed.  pointCount:'%A' points:'%A' knotsU:'%A' knotsV:'%A' degree:'%A' weights:'%A'" pointCount points knotsU knotsV degree weights
        let objectId = State.Doc.Objects.AddSurface(ns)
        if objectId = Guid.Empty then RhinoScriptingException.Raise "AddNurbsSurface failed.  pointCount:'%A' points:'%A' knotsU:'%A' knotsV:'%A' degree:'%A' weights:'%A'" pointCount points knotsU knotsV degree weights
        State.Doc.Views.Redraw()
        objectId

    /// <summary>Fits a Surface through Curve, point, point cloud, and Mesh objects.</summary>
    /// <param name="objectIds">(Guid seq) A list of object identifiers that indicate the objects to use for the patch fitting.
    ///    Acceptable object types include Curves, points, point clouds, and Meshes</param>
    /// <param name="startSurfaceId">(Guid) The identifier of the starting Surface. It is best if you create a starting Surface that is similar in shape
    ///    to the Surface you are trying to create</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The tolerance used by input analysis functions.</param>
    /// <param name="trim">(bool) Optional, default value: <c>true</c>
    ///    Try to find an outside Curve and trims the Surface to it. The default value is True</param>
    /// <param name="pointSpacing">(float) Optional, default value: <c>0.1</c>
    ///    The basic distance between points sampled from input Curves. The default value is 0.1</param>
    /// <param name="flexibility">(float) Optional, default value: <c>1.0</c>
    ///    Determines the behavior of the Surface in areas where its not otherwise controlled by the input.
    ///    Lower numbers make the Surface behave more like a stiff material, higher, more like a flexible material.
    ///    That is, each span is made to more closely match the spans adjacent to it if there is no input geometry
    ///    mapping to that area of the Surface when the flexibility value is low. The scale is logarithmic.
    ///    For example, numbers around 0.001 or 0.1 make the patch pretty stiff and numbers around 10 or 100
    ///    make the Surface flexible. The default value is 1.0</param>
    /// <param name="surfacePull">(float) Optional, default value: <c>1.0</c>
    ///    Similar to stiffness, but applies to the starting Surface. The bigger the pull, the closer
    ///    the resulting Surface shape will be to the starting Surface. The default value is 1.0</param>
    /// <param name="fixEdges">(bool) Optional, default value: <c>false</c>
    ///    Clamps the edges of the starting Surface in place. This option is useful if you are using a
    ///    Curve or points for deforming an existing Surface, and you do not want the edges of the starting Surface
    ///    to move. The default is False</param>
    /// <returns>(Guid) Identifier of the new Surface object.</returns>
    static member AddPatch( objectIds:Guid seq,
                              startSurfaceId: Guid,
                              [<OPT;DEF(0.0)>]tolerance:float,
                              [<OPT;DEF(true)>]trim:bool,
                              [<OPT;DEF(0.1)>]pointSpacing:float,
                              [<OPT;DEF(1.0)>]flexibility:float,
                              [<OPT;DEF(1.0)>]surfacePull:float,
                              [<OPT;DEF(false)>]fixEdges:bool) : Guid =
        let objectIdsGeometry = objectIds |> RArr.mapSeq RhinoScriptSyntax.CoerceGeometry
        let startSurfaceIdGeometry = RhinoScriptSyntax.CoerceSurface(startSurfaceId)
        let geometry = RhinoScriptSyntax.MakePatch(objectIdsGeometry, startSurfaceIdGeometry, tolerance, trim, pointSpacing, flexibility, surfacePull, fixEdges)
        let rc = State.Doc.Objects.AddBrep(geometry)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Fits a Surface through Curve, point, point cloud, and Mesh objects.</summary>
    /// <param name="objectIds">(Guid seq) A list of object identifiers that indicate the objects to use for the patch fitting.
    ///    Acceptable object types include Curves, points, point clouds, and Meshes</param>
    /// <param name="uvSpans">(int * int) The U and V direction span counts for the automatically generated Surface . however it is best if you create a starting Surface that is similar in shape
    ///    to the Surface you are trying to create an use the other overload of this method</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The tolerance used by input analysis functions.</param>
    /// <param name="trim">(bool) Optional, default value: <c>true</c>
    ///    Try to find an outside Curve and trims the Surface to it. The default value is True</param>
    /// <param name="pointSpacing">(float) Optional, default value: <c>0.1</c>
    ///    The basic distance between points sampled from input Curves. The default value is 0.1</param>
    /// <param name="flexibility">(float) Optional, default value: <c>1.0</c>
    ///    Determines the behavior of the Surface in areas where its not otherwise controlled by the input.
    ///    Lower numbers make the Surface behave more like a stiff material, higher, more like a flexible material.
    ///    That is, each span is made to more closely match the spans adjacent to it if there is no input geometry
    ///    mapping to that area of the Surface when the flexibility value is low. The scale is logarithmic.
    ///    For example, numbers around 0.001 or 0.1 make the patch pretty stiff and numbers around 10 or 100
    ///    make the Surface flexible. The default value is 1.0</param>
    /// <param name="surfacePull">(float) Optional, default value: <c>1.0</c>
    ///    Similar to stiffness, but applies to the starting Surface. The bigger the pull, the closer
    ///    the resulting Surface shape will be to the starting Surface. The default value is 1.0</param>
    /// <param name="fixEdges">(bool) Optional, default value: <c>false</c>
    ///    Clamps the edges of the starting Surface in place. This option is useful if you are using a
    ///    Curve or points for deforming an existing Surface, and you do not want the edges of the starting Surface
    ///    to move. The default is False</param>
    /// <returns>(Guid) Identifier of the new Surface object.</returns>
    static member AddPatch( objectIds:Guid seq,
                            uvSpans: int * int ,
                            [<OPT;DEF(0.0)>]tolerance:float,
                            [<OPT;DEF(true)>]trim:bool,
                            [<OPT;DEF(0.1)>]pointSpacing:float,
                            [<OPT;DEF(1.0)>]flexibility:float,
                            [<OPT;DEF(1.0)>]surfacePull:float,
                            [<OPT;DEF(false)>]fixEdges:bool) : Guid =
        let objectIdsGeometry = objectIds |> RArr.mapSeq RhinoScriptSyntax.CoerceGeometry
        let geometry = RhinoScriptSyntax.MakePatch(objectIdsGeometry, uvSpans, tolerance, trim, pointSpacing, flexibility, surfacePull, fixEdges)
        let rc = State.Doc.Objects.AddBrep(geometry)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Fits a Surface through Curve, point, point cloud, and Mesh objects. Returns geometry without adding it.</summary>
    /// <param name="geometryObjects">(GeometryBase seq) A list of objects that indicate the objects to use for the patch fitting.
    ///    Acceptable object types include Curves, points, point clouds, and Meshes</param>
    /// <param name="startSurfaceGeo">(Surface) The starting Surface. It is best if you create a starting Surface that is similar in shape
    ///    to the Surface you are trying to create</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The tolerance used by input analysis functions.</param>
    /// <param name="trim">(bool) Optional, default value: <c>true</c>
    ///    Try to find an outside Curve and trims the Surface to it. The default value is True</param>
    /// <param name="pointSpacing">(float) Optional, default value: <c>0.1</c>
    ///    The basic distance between points sampled from input Curves. The default value is 0.1</param>
    /// <param name="flexibility">(float) Optional, default value: <c>1.0</c>
    ///    Determines the behavior of the Surface in areas where its not otherwise controlled by the input.
    ///    Lower numbers make the Surface behave more like a stiff material, higher, more like a flexible material.
    ///    That is, each span is made to more closely match the spans adjacent to it if there is no input geometry
    ///    mapping to that area of the Surface when the flexibility value is low. The scale is logarithmic.
    ///    For example, numbers around 0.001 or 0.1 make the patch pretty stiff and numbers around 10 or 100
    ///    make the Surface flexible. The default value is 1.0</param>
    /// <param name="surfacePull">(float) Optional, default value: <c>1.0</c>
    ///    Similar to stiffness, but applies to the starting Surface. The bigger the pull, the closer
    ///    the resulting Surface shape will be to the starting Surface. The default value is 1.0</param>
    /// <param name="fixEdges">(bool) Optional, default value: <c>false</c>
    ///    Clamps the edges of the starting Surface in place. This option is useful if you are using a
    ///    Curve or points for deforming an existing Surface, and you do not want the edges of the starting Surface
    ///    to move. The default is False</param>
    /// <returns>(Brep) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakePatch( geometryObjects:GeometryBase seq,
                              startSurfaceGeo:Surface,
                              [<OPT;DEF(0.0)>]tolerance:float,
                              [<OPT;DEF(true)>]trim:bool,
                              [<OPT;DEF(0.1)>]pointSpacing:float,
                              [<OPT;DEF(1.0)>]flexibility:float,
                              [<OPT;DEF(1.0)>]surfacePull:float,
                              [<OPT;DEF(false)>]fixEdges:bool) : Brep =
                    let uspan, vspan = 10, 10
                    let geometry =  geometryObjects |> RArr.mapSeq (fun geometry -> geometry)
                    let surface = startSurfaceGeo
                    let tolerance = if 0.0 = tolerance then State.Doc.ModelAbsoluteTolerance else tolerance
                    let b =  Array.create 4 fixEdges
                    let brep = Brep.CreatePatch(geometry, surface, uspan, vspan, trim, false, pointSpacing, flexibility, surfacePull, b, tolerance)
                    if notNull brep then
                        brep
                    else
                        RhinoScriptingException.Raise "MakePatch failed for %A and %A" (Pretty.str geometryObjects) startSurfaceGeo


    /// <summary>Fits a Surface through Curve, point, point cloud, and Mesh objects. Returns geometry without adding it.</summary>
    /// <param name="geometryObjects">(GeometryBase seq) A list of objects that indicate the objects to use for the patch fitting.
    ///    Acceptable object types include Curves, points, point clouds, and Meshes</param>
    /// <param name="uvSpans">(int * int) The U and V direction span counts for the automatically generated Surface . however it is best if you create a starting Surface that is similar in shape
    ///    to the Surface you are trying to create an use the other overload of this method</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The tolerance used by input analysis functions.</param>
    /// <param name="trim">(bool) Optional, default value: <c>true</c>
    ///    Try to find an outside Curve and trims the Surface to it. The default value is True</param>
    /// <param name="pointSpacing">(float) Optional, default value: <c>0.1</c>
    ///    The basic distance between points sampled from input Curves. The default value is 0.1</param>
    /// <param name="flexibility">(float) Optional, default value: <c>1.0</c>
    ///    Determines the behavior of the Surface in areas where its not otherwise controlled by the input.
    ///    Lower numbers make the Surface behave more like a stiff material, higher, more like a flexible material.
    ///    That is, each span is made to more closely match the spans adjacent to it if there is no input geometry
    ///    mapping to that area of the Surface when the flexibility value is low. The scale is logarithmic.
    ///    For example, numbers around 0.001 or 0.1 make the patch pretty stiff and numbers around 10 or 100
    ///    make the Surface flexible. The default value is 1.0</param>
    /// <param name="surfacePull">(float) Optional, default value: <c>1.0</c>
    ///    Similar to stiffness, but applies to the starting Surface. The bigger the pull, the closer
    ///    the resulting Surface shape will be to the starting Surface. The default value is 1.0</param>
    /// <param name="fixEdges">(bool) Optional, default value: <c>false</c>
    ///    Clamps the edges of the starting Surface in place. This option is useful if you are using a
    ///    Curve or points for deforming an existing Surface, and you do not want the edges of the starting Surface
    ///    to move. The default is False</param>
    /// <returns>(Brep) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakePatch( geometryObjects:GeometryBase seq,
                            uvSpans: int * int ,
                            [<OPT;DEF(0.0)>]tolerance:float,
                            [<OPT;DEF(true)>]trim:bool,
                            [<OPT;DEF(0.1)>]pointSpacing:float,
                            [<OPT;DEF(1.0)>]flexibility:float,
                            [<OPT;DEF(1.0)>]surfacePull:float,
                            [<OPT;DEF(false)>]fixEdges:bool) : Brep =
        let uspan, vspan = uvSpans
        let geometry =   geometryObjects |> RArr.mapSeq (fun geometry -> geometry)
        let tolerance = if 0.0 = tolerance then State.Doc.ModelAbsoluteTolerance else tolerance
        let b =  Array.create 4 fixEdges
        let brep = Brep.CreatePatch(geometry, null, uspan, vspan, trim, false, pointSpacing, flexibility, surfacePull, b, tolerance) //TODO test with null as srf
        if notNull brep then
            brep
        else
            RhinoScriptingException.Raise "MakePatch failed for %A and %A" (Pretty.str geometryObjects) uvSpans


    /// <summary>Creates a single walled Surface with a circular profile around a Curve.</summary>
    /// <param name="curveId">(Guid) Identifier of rail Curve</param>
    /// <param name="parameters">(float seq) normalized Curve parameters</param>
    /// <param name="radii">(float seq) radius values at normalized Curve parameters</param>
    /// <param name="blendType">(int) Optional, default value: <c>0</c>
    ///    0(local) or 1(global)</param>
    /// <param name="cap">(int) Optional, default value: <c>0</c>
    ///    0(none), 1(flat), 2(round)</param>
    /// <param name="fit">(bool) Optional, default value: <c>false</c>
    ///    Attempt to fit a single Surface</param>
    /// <returns>(Guid ResizeArray) identifiers of new objects created.</returns>
    static member AddPipe( curveId:Guid,
                           parameters:float seq,
                           radii:float seq,
                           [<OPT;DEF(0)>]blendType:int,
                           [<OPT;DEF(0)>]cap:int,
                           [<OPT;DEF(false)>]fit:bool) : Guid ResizeArray =
        let curveIdGeometry = RhinoScriptSyntax.CoerceCurve(curveId)
        let geometry = RhinoScriptSyntax.MakePipe(curveIdGeometry, parameters, radii, blendType, cap, fit)
        let rc = geometry |> RArr.mapSeq State.Doc.Objects.AddBrep
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a single walled Surface with a circular profile around a Curve. Returns geometry without adding it.</summary>
    /// <param name="curveGeo">(Curve) Object of rail Curve</param>
    /// <param name="parameters">(float seq) normalized Curve parameters</param>
    /// <param name="radii">(float seq) radius values at normalized Curve parameters</param>
    /// <param name="blendType">(int) Optional, default value: <c>0</c>
    ///    0(local) or 1(global)</param>
    /// <param name="cap">(int) Optional, default value: <c>0</c>
    ///    0(none), 1(flat), 2(round)</param>
    /// <param name="fit">(bool) Optional, default value: <c>false</c>
    ///    Attempt to fit a single Surface</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakePipe( curveGeo:Curve,
                           parameters:float seq,
                           radii:float seq,
                           [<OPT;DEF(0)>]blendType:int,
                           [<OPT;DEF(0)>]cap:int,
                           [<OPT;DEF(false)>]fit:bool) : Brep ResizeArray =
        let rail = curveGeo
        let abstol = State.Doc.ModelAbsoluteTolerance
        let angtol = State.Doc.ModelAngleToleranceRadians
        let cap :PipeCapMode  = LanguagePrimitives.EnumOfValue  cap
        let breps = Brep.CreatePipe(rail, parameters, radii, (blendType = 0), cap, fit, abstol, angtol)
        ResizeArray(breps)


    /// <summary>Creates one Surface from one Polyline Geometry.</summary>
    /// <param name="polyline">(Polyline) one Polyline Geometry to use for creating planar Surfaces</param>
    /// <returns>(Brep) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakePlanarSrf(polyline:Polyline) : Brep =
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let breps = Brep.CreatePlanarBreps(new PolylineCurve(polyline), tolerance)
        if notNull breps then
            if breps.Length <> 1 then RhinoScriptingException.Raise "MakePlanarSrf created more then one surface on one input curve, use the seq overload instead on the same function on %s" (Pretty.str polyline)
            breps.[0]
        else
            RhinoScriptingException.Raise "MakePlanarSrf(polyline:Polyline) failed on  %s" (Pretty.str polyline)


    /// <summary>Creates one Surface from one planar Curve.</summary>
    /// <param name="curve">(Curve) one Curve Geometry to use for creating planar Surfaces</param>
    /// <returns>(Brep) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakePlanarSrf(curve:Curve) : Brep =
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let breps = Brep.CreatePlanarBreps(curve, tolerance)
        if notNull breps then
            if breps.Length <> 1 then RhinoScriptingException.Raise "MakePlanarSrf created more then one surface on one input curve, use the seq overload instead on the same function on %s" (Pretty.str curve)
            breps.[0]
        else
            RhinoScriptingException.Raise "MakePlanarSrf(curve:Curve) failed on %s" (Pretty.str curve)


    /// <summary>Creates one or more Surfaces from planar Curves.</summary>
    /// <param name="curves">(Curve seq) several Curves Geometries to use for creating planar Surfaces</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakePlanarSrf(curves:Curve seq) : Brep ResizeArray =
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let breps = Brep.CreatePlanarBreps(curves, tolerance)
        if notNull breps then
            ResizeArray(breps)
        else
            RhinoScriptingException.Raise "MakePlanarSrf(curves:Curve seq) failed on %s" (Pretty.str curves)


    /// <summary>Creates one or more Surfaces from planar Curves.</summary>
    /// <param name="objectIds">(Guid seq) Curves to use for creating planar Surfaces</param>
    /// <returns>(Guid ResizeArray) identifiers of Surfaces created .</returns>
    static member AddPlanarSrf(objectIds:Guid seq) : Guid ResizeArray =
        let curvesGeometry = objectIds |> RArr.mapSeq RhinoScriptSyntax.CoerceCurve
        let geometry = RhinoScriptSyntax.MakePlanarSrf(curvesGeometry)
        let rc = geometry |> RArr.mapSeq State.Doc.Objects.AddBrep
        State.Doc.Views.Redraw()
        rc


    /// <summary>Create a Plane Surface and add it to the document.</summary>
    /// <param name="plane">(Plane) The Plane</param>
    /// <param name="uDir">(float) The magnitude in the U direction</param>
    /// <param name="vDir">(float) The magnitude in the V direction</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member AddPlaneSurface( plane:Plane,
                                   uDir:float,
                                   vDir:float) : Guid =
        //plane = RhinoScriptSyntax.CoercePlane(plane)
        let uinterval = Interval(0.0, uDir)
        let vinterval = Interval(0.0, vDir)
        let planesurface = new PlaneSurface(plane, uinterval, vinterval)
        if planesurface|> isNull  then RhinoScriptingException.Raise "AddPlaneSurface failed.  plane:'%A' uDir:'%A' vDir:'%A'" plane uDir vDir
        let rc = State.Doc.Objects.AddSurface(planesurface)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddPlaneSurface failed.  plane:'%A' uDir:'%A' vDir:'%A'" plane uDir vDir
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a Surface created by lofting Curves to the document.
    ///    - no Curve sorting performed. pass in Curves in the order you want them sorted
    ///    - directions of open Curves not adjusted. Use CurveDirectionsMatch and
    ///      ReverseCurve to adjust the directions of open Curves
    ///    - seams of closed Curves are not adjusted. Use CurveSeam to adjust the seam
    ///      of closed Curves.</summary>
    /// <param name="objectIds">(Guid seq) Ordered list of the Curves to loft through</param>
    /// <param name="start">(Point3d) Optional, Starting point of the loft</param>
    /// <param name="ende">(Point3d) Optional, Ending point of the loft</param>
    /// <param name="loftType">(int) Optional, default value: <c>0</c>
    ///    Type of loft. Possible options are:
    ///    0 = Normal. Uses chord-length parameterization in the loft direction
    ///    1 = Loose. The Surface is allowed to move away from the original Curves
    ///      to make a smoother Surface. The Surface control points are created
    ///      at the same locations as the control points of the loft input Curves.
    ///    2 = Straight. The sections between the Curves are straight. This is
    ///      also known as a ruled Surface.
    ///    3 = Tight. The Surface sticks closely to the original Curves. Uses square
    ///      root of chord-length parameterization in the loft direction</param>
    /// <param name="rebuild">(int) Optional, default value: <c>0</c>
    ///    If not 0 then Rebuilds the shape Curves before lofting with this control point count</param>
    /// <param name="refit">(float) Optional, if given the loft is refitted, the value is the tolerance used to rebuild</param>
    /// <param name="closed">(bool) Optional, default value: <c>false</c>
    ///    Close the loft back to the first Curve</param>
    /// <returns>(Guid ResizeArray) Array containing the identifiers of the new Surface objects.</returns>
    static member AddLoftSrf( objectIds:Guid seq,
                              [<OPT;DEF(Point3d())>]start:Point3d, //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
                              [<OPT;DEF(Point3d())>]ende:Point3d,
                              [<OPT;DEF(0)>]loftType:int,
                              [<OPT;DEF(0)>]rebuild:int,
                              [<OPT;DEF(0.0)>]refit:float,
                              [<OPT;DEF(false)>]closed:bool) : Guid ResizeArray =
        let objectIdsGeometry = objectIds |> RArr.mapSeq RhinoScriptSyntax.CoerceCurve
        let geometry = RhinoScriptSyntax.MakeLoftSrf(objectIdsGeometry, start, ende, loftType, rebuild, refit, closed)
        let rc = ResizeArray<Guid>()
        for item in geometry do
            let id = State.Doc.Objects.AddBrep(item)
            if id <> Guid.Empty then rc.Add(id)
        if rc.Count > 0 then State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a Surface created by lofting Curves.
    ///    - no Curve sorting performed. pass in Curves in the order you want them sorted
    ///    - directions of open Curves not adjusted. Use CurveDirectionsMatch and
    ///      ReverseCurve to adjust the directions of open Curves
    ///    - seams of closed Curves are not adjusted. Use CurveSeam to adjust the seam
    ///      of closed Curves. Returns geometry without adding it.</summary>
    /// <param name="geometryObjects">(Curve seq) Ordered list of the Curves to loft through</param>
    /// <param name="start">(Point3d) Optional, Starting point of the loft</param>
    /// <param name="ende">(Point3d) Optional, Ending point of the loft</param>
    /// <param name="loftType">(int) Optional, default value: <c>0</c>
    ///    Type of loft. Possible options are:
    ///    0 = Normal. Uses chord-length parameterization in the loft direction
    ///    1 = Loose. The Surface is allowed to move away from the original Curves
    ///      to make a smoother Surface. The Surface control points are created
    ///      at the same locations as the control points of the loft input Curves.
    ///    2 = Straight. The sections between the Curves are straight. This is
    ///      also known as a ruled Surface.
    ///    3 = Tight. The Surface sticks closely to the original Curves. Uses square
    ///      root of chord-length parameterization in the loft direction</param>
    /// <param name="rebuild">(int) Optional, default value: <c>0</c>
    ///    If not 0 then Rebuilds the shape Curves before lofting with this control point count</param>
    /// <param name="refit">(float) Optional, if given the loft is refitted, the value is the tolerance used to rebuild</param>
    /// <param name="closed">(bool) Optional, default value: <c>false</c>
    ///    Close the loft back to the first Curve</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeLoftSrf( geometryObjects:Curve seq,
                              [<OPT;DEF(Point3d())>]start:Point3d, //TODO make overload instead,[<OPT;DEF(Point3d())>] may leak  see draw vector and transform point!
                              [<OPT;DEF(Point3d())>]ende:Point3d,
                              [<OPT;DEF(0)>]loftType:int,
                              [<OPT;DEF(0)>]rebuild:int,
                              [<OPT;DEF(0.0)>]refit:float,
                              [<OPT;DEF(false)>]closed:bool) : Brep ResizeArray =
        if loftType<0 || loftType>4 then RhinoScriptingException.Raise "MakeLoftSrf: LoftType must be 0-4.  geometryObjects:'%A' start:'%A' end:'%A' loftType:'%A' rebuild:'%A' refit:'%A' closed:'%A'" (Pretty.str geometryObjects) start ende loftType rebuild refit closed
        if rebuild<>0 && refit<>0.0 then RhinoScriptingException.Raise "MakeLoftSrf: set either rebuild or refit to a value ! not both.  geometryObjects:'%A' start:'%A' end:'%A' loftType:'%A' rebuild:'%A' refit:'%A' closed:'%A'" (Pretty.str geometryObjects) start ende loftType rebuild refit closed
        let curves  = geometryObjects |> RArr.mapSeq (fun geometry -> geometry)
        if Seq.length(curves)<2 then RhinoScriptingException.Raise "MakeLoftSrf failed.  geometryObjects:'%A' start:'%A' end:'%A' loftType:'%A' rebuild:'%A' refit:'%A' closed:'%A'" (Pretty.str geometryObjects) start ende loftType rebuild refit closed
        let start = if start = Point3d.Origin  then Point3d.Unset else start
        let ende  = if ende  = Point3d.Origin  then Point3d.Unset else ende
        let mutable lt = LoftType.Normal
        if loftType = 1 then lt <- LoftType.Loose
        elif loftType = 2 then lt <- LoftType.Straight
        elif loftType = 3 then lt <- LoftType.Tight
        let mutable breps = null
        if rebuild = 0 && refit = 0.0 then
            breps <- Brep.CreateFromLoft(curves, start, ende, lt, closed)
        elif rebuild > 0 then
            breps <- Brep.CreateFromLoftRebuild(curves, start, ende, lt, closed, rebuild)
        elif refit > 0.0 then
            breps <- Brep.CreateFromLoftRefit(curves, start, ende, lt, closed, refit)
        if isNull breps then RhinoScriptingException.Raise "MakeLoftSrf failed.  geometryObjects:'%A' start:'%A' end:'%A' loftType:'%A' rebuild:'%A' refit:'%A' closed:'%A'" (Pretty.str geometryObjects) start ende loftType rebuild refit closed
        ResizeArray(breps)


    /// <summary>Create a Surface by revolving a Curve around an axis.</summary>
    /// <param name="curveId">(Guid) Identifier of profile Curve</param>
    /// <param name="axis">(Line) Line for the rail revolve axis</param>
    /// <param name="startAngle">(float) Optional, default value: <c>0.0</c>
    ///    Start angles of revolve</param>
    /// <param name="endAngle">(float) Optional, default value: <c>360.0</c>
    ///    End angles of revolve</param>
    /// <returns>(Guid) identifier of new object.</returns>
    static member AddRevSrf( curveId:Guid,
                             axis:Line,
                             [<OPT;DEF(0.0)>]startAngle:float,
                             [<OPT;DEF(360.0)>]endAngle:float) : Guid =
        let curveIdGeometry = RhinoScriptSyntax.CoerceCurve(curveId)
        let geometry = RhinoScriptSyntax.MakeRevSrf(curveIdGeometry, axis, startAngle, endAngle)
        let rc = State.Doc.Objects.AddSurface(geometry)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Create a Surface by revolving a Curve around an axis. Returns geometry without adding it.</summary>
    /// <param name="curveGeo">(Curve) Object of profile Curve</param>
    /// <param name="axis">(Line) Line for the rail revolve axis</param>
    /// <param name="startAngle">(float) Optional, default value: <c>0.0</c>
    ///    Start angles of revolve</param>
    /// <param name="endAngle">(float) Optional, default value: <c>360.0</c>
    ///    End angles of revolve</param>
    /// <returns>(NurbsSurface) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeRevSrf( curveGeo:Curve,
                             axis:Line,
                             [<OPT;DEF(0.0)>]startAngle:float,
                             [<OPT;DEF(360.0)>]endAngle:float) : NurbsSurface =
        let curve = curveGeo
        let startAngle = toRadians(startAngle)
        let endAngle = toRadians(endAngle)
        let srf = RevSurface.Create(curve, axis, startAngle, endAngle)
        if isNull srf then RhinoScriptingException.Raise "MakeRevSrf failed. curveGeo:'%s' axis:'%A' startAngle:'%A' endAngle:'%A'" (Pretty.str curveGeo) axis startAngle endAngle
        let ns = srf.ToNurbsSurface()
        if isNull ns then RhinoScriptingException.Raise "MakeRevSrf failed. curveGeo:'%s' axis:'%A' startAngle:'%A' endAngle:'%A'" (Pretty.str curveGeo) axis startAngle endAngle
        ns


    /// <summary>Add a spherical Surface to the document.</summary>
    /// <param name="center">(Point3d) Center point of the sphere</param>
    /// <param name="radius">(float) Radius of the sphere in the current model units</param>
    /// <returns>(Guid) identifier of the new object .</returns>
    static member AddSphere(center:Point3d, radius:float) : Guid =
        let sphere = Sphere(center, radius)
        let rc = State.Doc.Objects.AddSphere(sphere)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddSphere failed.  centerOrPlane:'%A' radius:'%A'" center radius
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a spaced series of planar Curves resulting from the intersection of
    ///    defined cutting Planes through a Surface or Polysurface. For more
    ///    information, see Rhino help for details on the Contour command.</summary>
    /// <param name="objectId">(Guid) Object identifier to contour</param>
    /// <param name="plane">(Plane) The Plane that defines the cutting Plane</param>
    /// <returns>(Guid ResizeArray) ids of new contour Curves .</returns>
    static member AddSrfContourCrvs( objectId:Guid,
                                     plane:Plane) : Guid ResizeArray =
        let objectIdGeometry = RhinoScriptSyntax.CoerceBrep(objectId)
        let geometry = RhinoScriptSyntax.MakeSrfContourCrvs(objectIdGeometry, plane)
        let rc = ResizeArray<Guid>()
        for item in geometry do
            let id = State.Doc.Objects.AddCurve(item)
            if id <> Guid.Empty then rc.Add(id)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Adds a spaced series of planar Curves resulting from the intersection of
    ///    defined cutting Planes through a Surface or Polysurface. For more
    ///    information, see Rhino help for details on the Contour command.</summary>
    /// <param name="objectId">(Guid) Object identifier to contour</param>
    /// <param name="startPoint">(Point3d) The start point of a center line</param>
    /// <param name="endPoint">(Point3d)   the end point of a center line</param>
    /// <param name="interval">(float) Distance between contour Curves</param>
    /// <returns>(Guid ResizeArray) ids of new contour Curves .</returns>
    static member AddSrfContourCrvs( objectId:Guid,
                                     startPoint:Point3d,
                                     endPoint :Point3d ,
                                     interval:float) : Guid ResizeArray=
        let objectIdGeometry = RhinoScriptSyntax.CoerceBrep(objectId)
        let geometry = RhinoScriptSyntax.MakeSrfContourCrvs(objectIdGeometry, startPoint, endPoint, interval)
        let rc = ResizeArray<Guid>()
        for item in geometry do
            let id = State.Doc.Objects.AddCurve(item)
            if id <> Guid.Empty then rc.Add(id)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a spaced series of planar Curves resulting from the intersection of
    ///    defined cutting Planes through a Surface or Polysurface. For more
    ///    information, see Rhino help for details on the Contour command. Returns geometry without adding it.</summary>
    /// <param name="geometryObject">(Brep) Object to contour</param>
    /// <param name="plane">(Plane) The Plane that defines the cutting Plane</param>
    /// <returns>(Curve ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeSrfContourCrvs( geometryObject:Brep,
                                     plane:Plane) : Curve ResizeArray =
        let brep = geometryObject
        let curves =  Brep.CreateContourCurves(brep, plane)
        ResizeArray(curves)


    /// <summary>Creates a spaced series of planar Curves resulting from the intersection of
    ///    defined cutting Planes through a Surface or Polysurface. For more
    ///    information, see Rhino help for details on the Contour command. Returns geometry without adding it.</summary>
    /// <param name="geometryObject">(Brep) Object to contour</param>
    /// <param name="startPoint">(Point3d) The start point of a center line</param>
    /// <param name="endPoint">(Point3d)   the end point of a center line</param>
    /// <param name="interval">(float) Distance between contour Curves</param>
    /// <returns>(Curve ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeSrfContourCrvs( geometryObject:Brep,
                                     startPoint:Point3d,
                                     endPoint :Point3d ,
                                     interval:float) : Curve ResizeArray =
        let brep = geometryObject
        let curves =  Brep.CreateContourCurves(brep, startPoint , endPoint, interval)
        ResizeArray(curves)


    /// <summary>Creates a Surface from a grid of points.</summary>
    /// <param name="count">(int * int) Tuple of two numbers defining number of points in the u, v directions</param>
    /// <param name="points">(Point3d seq) List of 3D points</param>
    /// <param name="degreeU">(int) Optional, default value: <c>3</c> degree of the Surface in the U directions</param>
    /// <param name="degreeV">(int) Optional, default value: <c>3</c> degree of the Surface in the V directions</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member AddSrfControlPtGrid( count:int * int,
                                       points:Point3d seq,
                                       [<OPT;DEF(3)>]degreeU:int,
                                       [<OPT;DEF(3)>]degreeV:int           ) : Guid =
        //points = RhinoScriptSyntax.Coerce3dPointlist(points)
        let surf = NurbsSurface.CreateFromPoints(points, fst count, snd count,  degreeU,  degreeV)
        if isNull surf then RhinoScriptingException.Raise "AddSrfControlPtGrid failed.  count:'%A' points:'%A' degree:'%A'" count points (degreeU,degreeV)
        let objectId = State.Doc.Objects.AddSurface(surf)
        if objectId <> Guid.Empty then
            State.Doc.Views.Redraw()
            objectId
        else
            RhinoScriptingException.Raise "AddSrfControlPtGrid failed.  count:'%A' points:'%A' degree:'%A'" count points (degreeU,degreeV)

    /// <summary>Creates a new Surface from four corner points.</summary>
    /// <param name="pointA">(Point3d) First corner point</param>
    /// <param name="pointB">(Point3d) Second  corner point</param>
    /// <param name="pointC">(Point3d) Third corner point</param>
    /// <param name="pointD">(Point3d) Fourth corner point</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member AddSrfPt(pointA:Point3d , pointB:Point3d , pointC: Point3d , pointD: Point3d) : Guid =
        let surface = NurbsSurface.CreateFromCorners(pointA , pointB , pointC , pointD)
        if surface|> isNull  then RhinoScriptingException.Raise "AddSrfPt failed.  points:'%A, %A, %A and %A" pointA pointB pointC pointD
        let rc = State.Doc.Objects.AddSurface(surface)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddSrfPt failed.  points:'%A, %A, %A and %A" pointA pointB pointC pointD
        State.Doc.Views.Redraw()
        rc

    /// <summary>Creates a new Surface from three corner points.</summary>
    /// <param name="pointA">(Point3d) First corner point</param>
    /// <param name="pointB">(Point3d) Second  corner point</param>
    /// <param name="pointC">(Point3d) Third corner point</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member AddSrfPt(pointA:Point3d , pointB:Point3d , pointC: Point3d ) : Guid =
        let surface = NurbsSurface.CreateFromCorners(pointA , pointB , pointC)
        if surface|> isNull  then RhinoScriptingException.Raise "AddSrfPt failed.  points:'%A, %A and %A" pointA pointB pointC
        let rc = State.Doc.Objects.AddSurface(surface)
        if rc = Guid.Empty then RhinoScriptingException.Raise "AddSrfPt failed.  points:'%A, %A and %A" pointA pointB pointC
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a Surface from a grid of points.</summary>
    /// <param name="count">(int * int) Tuple of two numbers defining number of points in the u, v directions</param>
    /// <param name="points">(Point3d seq) List of 3D points</param>
    /// <param name="degreeU">(int) Optional, default value: <c>3</c> degree of the Surface in the U directions</param>
    /// <param name="degreeV">(int) Optional, default value: <c>3</c> degree of the Surface in the V directions</param>
    /// <param name="closedU">(bool * bool) Optional, default value: <c>false</c>  boolean defining if the Surface is closed in the U directions</param>
    /// <param name="closedV">(bool * bool) Optional, default value: <c>false</c>  boolean defining if the Surface is closed in the V directions</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member AddSrfPtGrid( count:int * int,
                                points:Point3d seq,
                                [<OPT;DEF(3)>]degreeU:int,
                                [<OPT;DEF(3)>]degreeV:int,
                                [<OPT;DEF(false)>]closedU:bool,
                                [<OPT;DEF(false)>]closedV:bool) : Guid =
        //points = RhinoScriptSyntax.Coerce3dPointlist(points)
        let surf = NurbsSurface.CreateThroughPoints(points, fst count, snd count, degreeU, degreeV, closedU, closedV)
        if isNull surf then RhinoScriptingException.Raise "AddSrfPtGrid failed.  count:'%A' points:'%A' degree:'%A' closed:'%A'" count points (degreeU,degreeV) (closedU,closedV)
        let objectId = State.Doc.Objects.AddSurface(surf)
        if objectId <> Guid.Empty then
            State.Doc.Views.Redraw()
            objectId
        else
            RhinoScriptingException.Raise "AddSrfPtGrid failed.  count:'%A' points:'%A' degree:'%A' closed:'%A'" count points (degreeU,degreeV) (closedU,closedV)


    /// <summary>Adds a Surface created through profile Curves that define the Surface
    ///    shape and one Curve that defines a Surface edge.</summary>
    /// <param name="rail">(Guid) Identifier of the rail Curve</param>
    /// <param name="shapes">(Guid seq) One or more cross section shape Curves</param>
    /// <param name="closed">(bool) Optional, default value: <c>false</c>
    ///    If True, then create a closed Surface</param>
    /// <returns>(Guid ResizeArray) List of new Surface objects.</returns>
    static member AddSweep1( rail:Guid,
                             shapes:Guid seq,
                             [<OPT;DEF(false)>]closed:bool) : Guid ResizeArray =
        let railGeometry = RhinoScriptSyntax.CoerceCurve(rail)
        let shapesGeometry = shapes |> RArr.mapSeq RhinoScriptSyntax.CoerceCurve
        let geometry = RhinoScriptSyntax.MakeSweep1(railGeometry, shapesGeometry, closed)
        let rc = geometry |> RArr.mapSeq State.Doc.Objects.AddBrep
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a Surface created through profile Curves that define the Surface
    ///    shape and one Curve that defines a Surface edge. Returns geometry without adding it.</summary>
    /// <param name="rail">(Curve) Object of the rail Curve</param>
    /// <param name="shapes">(Curve seq) One or more cross section shape Curves</param>
    /// <param name="closed">(bool) Optional, default value: <c>false</c>
    ///    If True, then create a closed Surface</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeSweep1( rail:Curve,
                             shapes:Curve seq,
                             [<OPT;DEF(false)>]closed:bool) : Brep ResizeArray =
        let rail = rail
        let shapes  = shapes |> RArr.mapSeq (fun geometry -> geometry)
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let breps = Brep.CreateFromSweep(rail, shapes, closed, tolerance)
        if isNull breps then RhinoScriptingException.Raise "MakeSweep1 failed.  rail:'%A' shapes:'%A' closed:'%A'" rail shapes closed
        ResizeArray(breps)


    /// <summary>Adds a Surface created through profile Curves that define the Surface
    ///    shape and two Curves that defines a Surface edge.</summary>
    /// <param name="rails">(Guid * Guid) Identifiers of the two rail Curve</param>
    /// <param name="shapes">(Guid seq) One or more cross section shape Curves</param>
    /// <param name="closed">(bool) Optional, default value: <c>false</c>
    ///    If True, then create a closed Surface</param>
    /// <returns>(Guid ResizeArray) List of new Surface objects.</returns>
    static member AddSweep2( rails:Guid * Guid,
                             shapes:Guid seq,
                             [<OPT;DEF(false)>]closed:bool) : Guid ResizeArray =
        let railsGeometry = (RhinoScriptSyntax.CoerceCurve(fst rails), RhinoScriptSyntax.CoerceCurve(snd rails))
        let shapesGeometry = shapes |> RArr.mapSeq RhinoScriptSyntax.CoerceCurve
        let geometry = RhinoScriptSyntax.MakeSweep2(railsGeometry, shapesGeometry, closed)
        let rc = geometry |> RArr.mapSeq State.Doc.Objects.AddBrep
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a Surface created through profile Curves that define the Surface
    ///    shape and two Curves that defines a Surface edge. Returns geometry without adding it.</summary>
    /// <param name="rails">(Curve * Curve) Objects of the two rail Curve</param>
    /// <param name="shapes">(Curve seq) One or more cross section shape Curves</param>
    /// <param name="closed">(bool) Optional, default value: <c>false</c>
    ///    If True, then create a closed Surface</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeSweep2( rails:Curve * Curve,
                             shapes:Curve seq,
                             [<OPT;DEF(false)>]closed:bool) : Brep ResizeArray =
        let rail1 = fst rails
        let rail2 = snd rails
        let shapes  = shapes |> RArr.mapSeq (fun geometry -> geometry)
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let breps = Brep.CreateFromSweep(rail1, rail2, shapes, closed, tolerance)
        if isNull breps then RhinoScriptingException.Raise "MakeSweep2 failed.  rails:'%A' shapes:'%A' closed:'%A'" rails shapes closed
        ResizeArray(breps)


    /// <summary>Adds a Surface created through profile Curves that define the Surface
    ///    shape and two Curves that defines a Surface edge.</summary>
    /// <param name="profile">(Guid) Identifier of the profile Curve</param>
    /// <param name="rail">(Guid) Identifier of the rail Curve</param>
    /// <param name="axis">(Line) A Line identifying the start point and end point of the rail revolve axis</param>
    /// <param name="scaleHeight">(bool) Optional, default value: <c>false</c>
    ///    If True, Surface will be locally scaled. Defaults to False</param>
    /// <returns>(Guid) identifier of the new object.</returns>
    static member AddRailRevSrf( profile:Guid,
                                 rail:Guid,
                                 axis:Line,
                                 [<OPT;DEF(false)>]scaleHeight:bool) : Guid =
        let profileGeometry = RhinoScriptSyntax.CoerceCurve(profile)
        let railGeometry = RhinoScriptSyntax.CoerceCurve(rail)
        let geometry = RhinoScriptSyntax.MakeRailRevSrf(profileGeometry, railGeometry, axis, scaleHeight)
        let rc = State.Doc.Objects.AddSurface(geometry)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a Surface created through profile Curves that define the Surface
    ///    shape and two Curves that defines a Surface edge. Returns geometry without adding it.</summary>
    /// <param name="profile">(Curve) Object of the profile Curve</param>
    /// <param name="rail">(Curve) Object of the rail Curve</param>
    /// <param name="axis">(Line) A Line identifying the start point and end point of the rail revolve axis</param>
    /// <param name="scaleHeight">(bool) Optional, default value: <c>false</c>
    ///    If True, Surface will be locally scaled. Defaults to False</param>
    /// <returns>(NurbsSurface) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeRailRevSrf( profile:Curve,
                                 rail:Curve,
                                 axis:Line,
                                 [<OPT;DEF(false)>]scaleHeight:bool) : NurbsSurface =
        let profileinst = profile
        let railinst = rail
        let surface = NurbsSurface.CreateRailRevolvedSurface(profileinst, railinst, axis, scaleHeight)
        if isNull surface then RhinoScriptingException.Raise "MakeRailRevSrf failed.  profile:'%A' rail:'%A' axis:'%A' scaleHeight:'%A'" profile rail axis scaleHeight
        surface


    /// <summary>Adds a torus shaped revolved Surface to the document.</summary>
    /// <param name="basis">(Plane) The basis Plane of the torus</param>
    /// <param name="majorRadius">(float) Major radius of  the torus</param>
    /// <param name="minorRadius">(float) Minor radius of  torus</param>
    /// <returns>(Guid) The identifier of the new object.</returns>
    static member AddTorus( basis:Plane,
                            majorRadius:float,
                            minorRadius:float) : Guid =
        let torus = Torus(basis, majorRadius, minorRadius)
        let revsurf = torus.ToRevSurface()
        let rc = State.Doc.Objects.AddSurface(revsurf)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Performs a boolean difference operation on two sets of input Surfaces
    ///    and Polysurfaces. For more details, see the BooleanDifference command in
    ///    the Rhino help file.</summary>
    /// <param name="input0">(Guid seq) List of Surfaces to subtract from</param>
    /// <param name="input1">(Guid seq) List of Surfaces to be subtracted</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>true</c>
    ///    Delete all input objects</param>
    /// <returns>(Guid ResizeArray) List of identifiers of newly created objects .</returns>
    static member BooleanDifference( input0:Guid seq,
                                     input1:Guid seq,
                                     [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let input0 = input0 |> Seq.toArray
        let input1 = input1 |> Seq.toArray
        let input0Geometry = input0 |> RArr.mapSeq RhinoScriptSyntax.CoerceBrep
        let input1Geometry = input1 |> RArr.mapSeq RhinoScriptSyntax.CoerceBrep
        let geometry = RhinoScriptSyntax.BooleanDifference(input0Geometry, input1Geometry)
        let rc = geometry |> RArr.mapSeq State.Doc.Objects.AddBrep
        if deleteInput then
            for id in input0 do State.Doc.Objects.Delete(id, true) |> ignore<bool>
            for id in input1 do State.Doc.Objects.Delete(id, true) |> ignore<bool>
        State.Doc.Views.Redraw()
        rc


    /// <summary>Performs a boolean difference operation on two sets of input Surfaces
    ///    and Polysurfaces. For more details, see the BooleanDifference command in
    ///    the Rhino help file. Returns geometry without adding it.</summary>
    /// <param name="input0">(Brep seq) List of Surfaces to subtract from</param>
    /// <param name="input1">(Brep seq) List of Surfaces to be subtracted</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member BooleanDifference( input0:Brep seq,
                                     input1:Brep seq) : Brep ResizeArray =
        let breps0 = input0 |> Seq.toArray
        let breps1 = input1 |> Seq.toArray
        let breps = Brep.CreateBooleanDifference(breps0, breps1, State.Doc.ModelAbsoluteTolerance)
        if isNull breps then RhinoScriptingException.Raise "BooleanDifference: operation failed."
        ResizeArray(breps)


    /// <summary>Performs a boolean intersection operation on two sets of input Surfaces
    ///    and Polysurfaces. For more details, see the BooleanIntersection command in
    ///    the Rhino help file.</summary>
    /// <param name="input0">(Guid seq) List of Surfaces</param>
    /// <param name="input1">(Guid seq) List of Surfaces</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>true</c>
    ///    Delete all input objects</param>
    /// <returns>(Guid ResizeArray) List of identifiers of newly created objects .</returns>
    static member BooleanIntersection( input0:Guid seq,
                                       input1:Guid seq,
                                       [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let input0 = input0 |> Seq.toArray
        let input1 = input1 |> Seq.toArray
        let input0Geometry = input0 |> RArr.mapSeq RhinoScriptSyntax.CoerceBrep
        let input1Geometry = input1 |> RArr.mapSeq RhinoScriptSyntax.CoerceBrep
        let geometry = RhinoScriptSyntax.BooleanIntersection(input0Geometry, input1Geometry)
        let rc = geometry |> RArr.mapSeq State.Doc.Objects.AddBrep
        if deleteInput then
            for id in input0 do State.Doc.Objects.Delete(id, true) |> ignore<bool>
            for id in input1 do State.Doc.Objects.Delete(id, true) |> ignore<bool>
        State.Doc.Views.Redraw()
        rc


    /// <summary>Performs a boolean intersection operation on two sets of input Surfaces
    ///    and Polysurfaces. For more details, see the BooleanIntersection command in
    ///    the Rhino help file. Returns geometry without adding it.</summary>
    /// <param name="input0">(Brep seq) List of Surfaces</param>
    /// <param name="input1">(Brep seq) List of Surfaces</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member BooleanIntersection( input0:Brep seq,
                                       input1:Brep seq) : Brep ResizeArray =
        let breps0 = input0 |> Seq.toArray
        let breps1 = input1 |> Seq.toArray
        let breps = Brep.CreateBooleanIntersection(breps0, breps1, State.Doc.ModelAbsoluteTolerance)
        if isNull breps then RhinoScriptingException.Raise "BooleanIntersection: operation failed."
        ResizeArray(breps)


    /// <summary>Performs a boolean union operation on a set of input Surfaces and
    ///    Polysurfaces. For more details, see the BooleanUnion command in the
    ///    Rhino help file.</summary>
    /// <param name="input">(Guid seq) List of Surfaces to union</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>true</c>
    ///    Delete all input objects</param>
    /// <returns>(Guid ResizeArray) List of identifiers of newly created objects .</returns>
    static member BooleanUnion(input:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid ResizeArray =
        let input = input |> Seq.toArray
        let inputGeometry = input |> RArr.mapSeq RhinoScriptSyntax.CoerceBrep
        let geometry = RhinoScriptSyntax.BooleanUnion(inputGeometry)
        let rc = geometry |> RArr.mapSeq State.Doc.Objects.AddBrep
        if deleteInput then
            for id in input do State.Doc.Objects.Delete(id, true) |> ignore<bool>
        State.Doc.Views.Redraw()
        rc


    /// <summary>Performs a boolean union operation on a set of input Surfaces and
    ///    Polysurfaces. For more details, see the BooleanUnion command in the
    ///    Rhino help file. Returns geometry without adding it.</summary>
    /// <param name="input">(Brep seq) List of Surfaces to union</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member BooleanUnion(input:Brep seq) : Brep ResizeArray =
        let breps = input |> Seq.toArray
        if breps.Length < 2 then RhinoScriptingException.Raise "BooleanUnion: at least two Breps are required."
        let breps = Brep.CreateBooleanUnion(breps, State.Doc.ModelAbsoluteTolerance)
        if isNull breps then RhinoScriptingException.Raise "BooleanUnion: operation failed."
        ResizeArray(breps)


    /// <summary>Returns the point on a Surface or Polysurface that is closest to a test
    ///    point. This function works on both untrimmed and trimmed Surfaces.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <param name="point">(Point3d) The test, or sampling point</param>
    /// <returns>(Point3d * float * float * ComponentIndexType * int * Vector3d) of closest point information . The list will
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
        let objectIdObject = RhinoScriptSyntax.CoerceBrep(objectId)
        RhinoScriptSyntax.BrepClosestPoint(objectIdObject, point)

    /// <summary>Returns the point on a Surface or Polysurface that is closest to a test
    ///    point. This function works on both untrimmed and trimmed Surfaces. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(Brep) The object</param>
    /// <param name="point">(Point3d) The test, or sampling point</param>
    /// <returns>(Point3d * float * float * ComponentIndexType * int * Vector3d) of closest point information . The list will
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
    static member BrepClosestPoint(geometryObject:Brep, point:Point3d) : Point3d * float * float * ComponentIndexType * int * Vector3d =
        let brep = geometryObject
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
            RhinoScriptingException.Raise "BrepClosestPoint failed for %A and %A" (Pretty.str geometryObject) point


    /// <summary>Caps planar holes in a Surface or Polysurface.</summary>
    /// <param name="surfaceId">(Guid) The identifier of the Surface or Polysurface to cap</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member CapPlanarHoles(surfaceId:Guid) : bool =
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let newbrep = brep.CapPlanarHoles(tolerance)
        if notNull newbrep then
            if newbrep.SolidOrientation = BrepSolidOrientation.Inward then
                newbrep.Flip()
            //surfaceId = RhinoScriptSyntax.CoerceGuid(surfaceId)
            if State.Doc.Objects.Replace(surfaceId, newbrep) then
                State.Doc.Views.Redraw()
                true
            else
                false
        else
            false


    /// <summary>Duplicates the edge Curves of a Surface or Polysurface. For more
    ///    information, see the Rhino help file for information on the DupEdge
    ///    command.</summary>
    /// <param name="objectId">(Guid) The identifier of the Surface or Polysurface object</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the duplicated edge Curves. The default is not to select (False)</param>
    /// <returns>(Guid ResizeArray) identifying the newly created Curve objects.</returns>
    static member DuplicateEdgeCurves(objectId:Guid, [<OPT;DEF(false)>]select:bool) : Guid ResizeArray =
        let objectIdGeometry = RhinoScriptSyntax.CoerceBrep(objectId)
        let curves = RhinoScriptSyntax.DuplicateEdgeCurves(objectIdGeometry)
        let ids = ResizeArray<Guid>()
        for curve in curves do
            let id = State.Doc.Objects.AddCurve(curve)
            curve.Dispose()
            if id = Guid.Empty then RhinoScriptingException.Raise "DuplicateEdgeCurves: unable to add curve to the document."
            ids.Add(id)
            if select then (RhinoScriptSyntax.CoerceRhinoObject(id)).Select(true) |> ignore
        if ids.Count > 0 then State.Doc.Views.Redraw()
        ids


    /// <summary>Duplicates the edge Curves of a Surface or Polysurface. For more
    ///    information, see the Rhino help file for information on the DupEdge
    ///    command. Returns geometry without adding it.</summary>
    /// <param name="geometryObject">(Brep) The Surface or Polysurface object</param>
    /// <returns>(Curve ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member DuplicateEdgeCurves(geometryObject:Brep) : Curve ResizeArray =
        geometryObject.DuplicateEdgeCurves() |> Seq.filter (fun curve -> curve.IsValid) |> ResizeArray


    /// <summary>Create Curves that duplicate a Surface or Polysurface border.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface</param>
    /// <param name="typ">(int) Optional, default value: <c>0</c>
    ///    The border Curves to return
    ///    0 = both exterior and interior,
    ///    1 = exterior
    ///    2 = interior</param>
    /// <returns>(Guid ResizeArray) list of Curve ids .</returns>
    static member DuplicateSurfaceBorder(surfaceId:Guid, [<OPT;DEF(0)>]typ:int) : Guid ResizeArray =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceBrep(surfaceId)
        let geometry = RhinoScriptSyntax.DuplicateSurfaceBorder(surfaceIdGeometry, typ)
        let rc = geometry |> RArr.mapSeq State.Doc.Objects.AddCurve
        State.Doc.Views.Redraw()
        rc


    /// <summary>Create Curves that duplicate a Surface or Polysurface border. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Brep) Object of a Surface</param>
    /// <param name="typ">(int) Optional, default value: <c>0</c>
    ///    The border Curves to return
    ///    0 = both exterior and interior,
    ///    1 = exterior
    ///    2 = interior</param>
    /// <returns>(Curve ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member DuplicateSurfaceBorder(surfaceGeo:Brep, [<OPT;DEF(0)>]typ:int) : Curve ResizeArray =
        let brep = surfaceGeo
        let inner = typ = 0 || typ = 2
        let outer = typ = 0 || typ = 1
        let mutable curves = brep.DuplicateNakedEdgeCurves(outer, inner)
        if curves|> isNull  then RhinoScriptingException.Raise "DuplicateSurfaceBorder failed.  surfaceGeo:'%s' type:'%d'" (Pretty.str surfaceGeo) typ
        let tolerance = State.Doc.ModelAbsoluteTolerance * 2.1
        curves <- Curve.JoinCurves(curves, tolerance)
        if curves|> isNull  then RhinoScriptingException.Raise "DuplicateSurfaceBorder failed.  surfaceGeo:'%s' type:'%d'" (Pretty.str surfaceGeo) typ
        ResizeArray(curves)


    /// <summary>Evaluates a Surface at a U, V parameter.</summary>
    /// <param name="surfaceId">(Guid) The object's identifier</param>
    /// <param name="u">(float) U of u, v parameters to evaluate</param>
    /// <param name="v">(float) V of u, v parameters to evaluate</param>
    /// <returns>(Point3d) a 3-D point.</returns>
    static member EvaluateSurface( surfaceId:Guid,
                                   u:float ,
                                   v:float ) : Point3d =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.EvaluateSurface(surfaceIdObject, u, v)

    /// <summary>Evaluates a Surface at a U, V parameter. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The object</param>
    /// <param name="u">(float) U of u, v parameters to evaluate</param>
    /// <param name="v">(float) V of u, v parameters to evaluate</param>
    /// <returns>(Point3d) a 3-D point.</returns>
    static member EvaluateSurface( surfaceGeo:Surface,
                                   u:float ,
                                   v:float ) : Point3d =
        let surface = surfaceGeo
        let rc = surface.PointAt(u, v)
        if rc.IsValid then rc
        else RhinoScriptingException.Raise "EvaluateSurface failed.  surfaceGeo:'%s' u:'%f' v:'%f'" (Pretty.str surfaceGeo) u v


    /// <summary>Lengthens an untrimmed Surface object.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface</param>
    /// <param name="parameter">(float * float) Tuple of two values defining the U, V parameter to evaluate.
    ///    The Surface edge closest to the U, V parameter will be the edge that is
    ///    extended</param>
    /// <param name="length">(float) Amount to extend to Surface</param>
    /// <param name="smooth">(bool) Optional, default value: <c>true</c>
    ///    If True, the Surface is extended smoothly curving from the
    ///    edge. If False, the Surface is extended in a straight line from the edge</param>
    /// <returns>(bool) True or False indicating success or failure.</returns>
    static member ExtendSurface( surfaceId:Guid,
                                 parameter:float * float,
                                 length:float,
                                 [<OPT;DEF(true)>]smooth:bool) : bool =
        let surface = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let edge = surface.ClosestSide(parameter|> fst, parameter|> snd)
        let newsrf = surface.Extend(edge, length, smooth)
        if notNull newsrf then
            State.Doc.Objects.Replace(surfaceId, newsrf)|> ignore<bool>
            State.Doc.Views.Redraw()
        else
            ()
        notNull newsrf

    /// <summary>Explodes, or un-joins, one or more Polysurface objects. Polysurfaces
    ///    will be exploded into separate Surfaces.</summary>
    /// <param name="objectIds">(Guid seq) Identifiers of Polysurfaces to explode</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Delete input objects after exploding</param>
    /// <returns>(Guid ResizeArray) List of identifiers of exploded pieces .</returns>
    static member ExplodePolysurfaces(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid ResizeArray =
        let ids = ResizeArray<Guid>()
        for objectId in objectIds do
            let brep = RhinoScriptSyntax.CoerceBrep(objectId)
            let faces = RhinoScriptSyntax.ExplodePolysurfaces([brep])
            for face in faces do
                let id = State.Doc.Objects.AddBrep(face)
                if id <> Guid.Empty then ids.Add(id)
            if deleteInput && brep.Faces.Count > 1 then State.Doc.Objects.Delete(objectId, true) |> ignore<bool>
        State.Doc.Views.Redraw()
        ids


    /// <summary>Explodes, or un-joins, one or more Polysurface objects. Polysurfaces
    ///    will be exploded into separate Surfaces. Returns geometry without adding it.</summary>
    /// <param name="geometryObjects">(Brep seq) Polysurfaces to explode</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member ExplodePolysurfaces(geometryObjects:Brep seq) : Brep ResizeArray =
        let faces = ResizeArray<Brep>()
        for brep in geometryObjects do
            if brep.Faces.Count > 1 then
                for face in brep.Faces do faces.Add(face.DuplicateFace(false))
        faces


    /// <summary>Extracts iso-parametric Curves from a Surface.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface</param>
    /// <param name="parameter">(float * float) U, v parameter of the Surface to evaluate</param>
    /// <param name="direction">(int) Direction to evaluate
    ///    0 = u
    ///    1 = v
    ///    2 = both</param>
    /// <returns>(Guid ResizeArray) List of Curve ids .</returns>
    static member ExtractIsoCurve( surfaceId:Guid,
                                   parameter:float * float,
                                   direction:int) : Guid ResizeArray =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let geometry = RhinoScriptSyntax.ExtractIsoCurve(surfaceIdGeometry, parameter, direction)
        let rc = ResizeArray<Guid>()
        for item in geometry do
            let id = State.Doc.Objects.AddCurve(item)
            if id <> Guid.Empty then rc.Add(id)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Extracts iso-parametric Curves from a Surface. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Surface) Object of a Surface</param>
    /// <param name="parameter">(float * float) U, v parameter of the Surface to evaluate</param>
    /// <param name="direction">(int) Direction to evaluate
    ///    0 = u
    ///    1 = v
    ///    2 = both</param>
    /// <returns>(Curve ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member ExtractIsoCurve( surfaceGeo:Surface,
                                   parameter:float * float,
                                   direction:int) : Curve ResizeArray =
        let surface = surfaceGeo
        let ids = ResizeArray<Curve>()
        let mutable curves = [| |]
        if direction = 0 || direction = 2 then

            match surface with
            | :? BrepFace as br ->
                curves <- br.TrimAwareIsoCurve(0, parameter|> snd)
            | _ ->
                curves <- [|surface.IsoCurve(0, parameter|> snd) |]
            if notNull curves then
                for curve in curves do
                    ids.Add(curve)

        if direction = 1 || direction = 2 then
            curves <- null
            match surface with
            | :? BrepFace as br ->
                curves <- br.TrimAwareIsoCurve(1, parameter|> fst)
            | _ ->
                curves <- [|surface.IsoCurve(1, parameter|> fst)|]
            if notNull curves then
                for curve in curves do
                    ids.Add(curve)
        ids


    /// <summary>Separates or copies a Surface or a copy of a Surface from a Polysurface.</summary>
    /// <param name="objectId">(Guid) Polysurface identifier</param>
    /// <param name="faceIndices">(int seq) One or more numbers representing faces</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>
    ///    If True the faces are copied. If False, the faces are extracted</param>
    /// <returns>(Guid ResizeArray) identifiers of extracted Surface objects .</returns>
    static member ExtractSurface( objectId:Guid,
                                  faceIndices:int seq,
                                  [<OPT;DEF(false)>]copy:bool) : Guid ResizeArray =
        let brep = RhinoScriptSyntax.CoerceBrep(objectId)
        let faceIndices = faceIndices |> Seq.sortDescending |> Seq.toArray
        let faces = RhinoScriptSyntax.ExtractSurface(brep, faceIndices)
        let ids = faces |> RArr.mapSeq State.Doc.Objects.AddBrep
        if copy then
            let remaining = brep.DuplicateBrep()
            for index in faceIndices do remaining.Faces.RemoveAt(index)
            State.Doc.Objects.Replace(objectId, remaining) |> ignore<bool>
        State.Doc.Views.Redraw()
        ids


    /// <summary>Duplicates the selected faces of a Polysurface as independent Breps. Returns geometry without adding it to the document.</summary>
    /// <param name="geometryObject">(Brep) Polysurface object</param>
    /// <param name="faceIndices">(int seq) One or more numbers representing faces</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member ExtractSurface( geometryObject:Brep,
                                  faceIndices:int seq) : Brep ResizeArray =
        faceIndices |> Seq.sortDescending |> Seq.map (fun index -> geometryObject.Faces.[index].DuplicateFace(true)) |> ResizeArray


    /// <summary>Creates a Surface by extruding a Curve along a path.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve to extrude</param>
    /// <param name="pathId">(Guid) Identifier of the path Curve</param>
    /// <returns>(Guid) identifier of new Surface .</returns>
    static member ExtrudeCurve(curveId:Guid, pathId:Guid) : Guid =
        let curveIdGeometry = RhinoScriptSyntax.CoerceCurve(curveId)
        let pathIdGeometry = RhinoScriptSyntax.CoerceCurve(pathId)
        let geometry = RhinoScriptSyntax.ExtrudeCurve(curveIdGeometry, pathIdGeometry)
        let rc = State.Doc.Objects.AddSurface(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "ExtrudeCurve: unable to add geometry to the document."
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a Surface by extruding a Curve along a path. Returns geometry without adding it.</summary>
    /// <param name="curveGeo">(Curve) Object of the Curve to extrude</param>
    /// <param name="pathGeo">(Curve) Object of the path Curve</param>
    /// <returns>(SumSurface) Newly created geometry. The caller owns the returned objects.</returns>
    static member ExtrudeCurve(curveGeo:Curve, pathGeo:Curve) : SumSurface =
        let curve1 = curveGeo
        let curve2 = pathGeo
        let srf = SumSurface.Create(curve1, curve2)
        if isNull srf then RhinoScriptingException.Raise "ExtrudeCurve: extrusion failed."
        srf


    /// <summary>Creates a Surface by extruding a Curve to a point.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve to extrude</param>
    /// <param name="point">(Point3d) 3D point</param>
    /// <returns>(Guid) identifier of new Surface .</returns>
    static member ExtrudeCurvePoint(curveId:Guid, point:Point3d) : Guid =
        let curveIdGeometry = RhinoScriptSyntax.CoerceCurve(curveId)
        let geometry = RhinoScriptSyntax.ExtrudeCurvePoint(curveIdGeometry, point)
        let rc = State.Doc.Objects.AddSurface(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "ExtrudeCurvePoint: unable to add geometry to the document."
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates a Surface by extruding a Curve to a point. Returns geometry without adding it.</summary>
    /// <param name="curveGeo">(Curve) Object of the Curve to extrude</param>
    /// <param name="point">(Point3d) 3D point</param>
    /// <returns>(Surface) Newly created geometry. The caller owns the returned objects.</returns>
    static member ExtrudeCurvePoint(curveGeo:Curve, point:Point3d) : Surface =
        let curve = curveGeo
        let srf = Surface.CreateExtrusionToPoint(curve, point)
        if isNull srf then RhinoScriptingException.Raise "ExtrudeCurvePoint: extrusion failed."
        srf


    /// <summary>Create Surface by extruding a Curve along two points that define a line.</summary>
    /// <param name="curveId">(Guid) Identifier of the Curve to extrude</param>
    /// <param name="startPoint">(Point3d) Start point</param>
    /// <param name="endPoint">(Point3d) End point, that specifies distance and direction</param>
    /// <returns>(Guid) identifier of new Surface .</returns>
    static member ExtrudeCurveStraight( curveId:Guid,
                                        startPoint:Point3d,
                                        endPoint:Point3d) : Guid =
        let curveIdGeometry = RhinoScriptSyntax.CoerceCurve(curveId)
        let geometry = RhinoScriptSyntax.ExtrudeCurveStraight(curveIdGeometry, startPoint, endPoint)
        let rc = State.Doc.Objects.AddSurface(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "ExtrudeCurveStraight: unable to add geometry to the document."
        State.Doc.Views.Redraw()
        rc


    /// <summary>Create Surface by extruding a Curve along two points that define a line. Returns geometry without adding it.</summary>
    /// <param name="curveGeo">(Curve) Object of the Curve to extrude</param>
    /// <param name="startPoint">(Point3d) Start point</param>
    /// <param name="endPoint">(Point3d) End point, that specifies distance and direction</param>
    /// <returns>(Surface) Newly created geometry. The caller owns the returned objects.</returns>
    static member ExtrudeCurveStraight( curveGeo:Curve,
                                        startPoint:Point3d,
                                        endPoint:Point3d) : Surface =
        let curve = curveGeo
        let vec = endPoint - startPoint
        let srf = Surface.CreateExtrusion(curve, vec)
        if isNull srf then RhinoScriptingException.Raise "ExtrudeCurveStraight: extrusion failed."
        srf


    /// <summary>Create Surface by extruding along a path Curve.</summary>
    /// <param name="surfaceId">(Guid) Identifier of the Surface to extrude</param>
    /// <param name="curveId">(Guid) Identifier of the path Curve</param>
    /// <param name="cap">(bool) Optional, default value: <c>true</c>
    ///    Extrusion is capped at both ends</param>
    /// <returns>(Guid) identifier of new Surface .</returns>
    static member ExtrudeSurface( surfaceId:Guid,
                                  curveId:Guid,
                                  [<OPT;DEF(true)>]cap:bool) : Guid =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceBrep(surfaceId)
        let curveIdGeometry = RhinoScriptSyntax.CoerceCurve(curveId)
        let geometry = RhinoScriptSyntax.ExtrudeSurface(surfaceIdGeometry, curveIdGeometry, cap)
        let rc = State.Doc.Objects.AddBrep(geometry)
        State.Doc.Views.Redraw()
        rc


    /// <summary>Create Surface by extruding along a path Curve. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Brep) Object of the Surface to extrude</param>
    /// <param name="curveGeo">(Curve) Object of the path Curve</param>
    /// <param name="cap">(bool) Optional, default value: <c>true</c>
    ///    Extrusion is capped at both ends</param>
    /// <returns>(Brep) Newly created geometry. The caller owns the returned objects.</returns>
    static member ExtrudeSurface( surfaceGeo:Brep,
                                  curveGeo:Curve,
                                  [<OPT;DEF(true)>]cap:bool) : Brep =
        let brep = surfaceGeo
        let curve = curveGeo
        let newbrep = brep.Faces.[0].CreateExtrusion(curve, cap)
        if notNull newbrep then
            newbrep
        else
            RhinoScriptingException.Raise "ExtrudeSurface failed on Surface: %s and Curve %s" (Pretty.str surfaceGeo)  (Pretty.str curveGeo)


    /// <summary>Create constant radius rolling ball fillets between two Surfaces. Note,
    ///    this function does not trim the original Surfaces of the fillets.</summary>
    /// <param name="surface0">(Guid) first Surface</param>
    /// <param name="surface1">(Guid) second Surface</param>
    /// <param name="radius">(float) A positive fillet radius</param>
    /// <param name="uvparam0">(Point2d) Optional, A u, v Surface parameter of Surface0 near where the fillet
    ///    is expected to hit the Surface</param>
    /// <param name="uvparam1">(Point2d) Optional, Same as uvparam0, but for Surface1</param>
    /// <returns>(Guid ResizeArray) ids of Surfaces created .</returns>
    static member FilletSurfaces( surface0:Guid,
                                  surface1:Guid,
                                  radius:float,
                                  [<OPT;DEF(Point2d())>]uvparam0:Point2d,
                                  [<OPT;DEF(Point2d())>]uvparam1:Point2d) : Guid ResizeArray=
        let surface0Geometry = RhinoScriptSyntax.CoerceSurface(surface0)
        let surface1Geometry = RhinoScriptSyntax.CoerceSurface(surface1)
        let geometry = RhinoScriptSyntax.FilletSurfaces(surface0Geometry, surface1Geometry, radius, uvparam0, uvparam1)
        let rc = geometry |> RArr.mapSeq State.Doc.Objects.AddSurface
        State.Doc.Views.Redraw()
        rc


    /// <summary>Create constant radius rolling ball fillets between two Surfaces. Note,
    ///    this function does not trim the original Surfaces of the fillets. Returns geometry without adding it.</summary>
    /// <param name="surface0">(Surface) first Surface</param>
    /// <param name="surface1">(Surface) second Surface</param>
    /// <param name="radius">(float) A positive fillet radius</param>
    /// <param name="uvparam0">(Point2d) Optional, A u, v Surface parameter of Surface0 near where the fillet
    ///    is expected to hit the Surface</param>
    /// <param name="uvparam1">(Point2d) Optional, Same as uvparam0, but for Surface1</param>
    /// <returns>(Surface ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member FilletSurfaces( surface0:Surface,
                                  surface1:Surface,
                                  radius:float,
                                  [<OPT;DEF(Point2d())>]uvparam0:Point2d,
                                  [<OPT;DEF(Point2d())>]uvparam1:Point2d) : Surface ResizeArray =
        let surface0 = surface0
        let surface1 = surface1
        let tol = State.Doc.ModelAbsoluteTolerance
        let surfaces =
            if uvparam0<>Point2d.Origin && uvparam1<>Point2d.Origin then
                Surface.CreateRollingBallFillet(surface0, uvparam0, surface1, uvparam1, radius, tol)
            else
                Surface.CreateRollingBallFillet(surface0, surface1, radius, tol)
        if isNull surfaces then RhinoScriptingException.Raise "FilletSurfaces failed.  surface0:'%A' surface1:'%A' radius:'%A' uvparam0:'%A' uvparam1:'%A'" surface0 surface1 radius uvparam0 uvparam1
        ResizeArray(surfaces)


    /// <summary>Returns the normal direction of a Surface. This feature can
    /// also be found in Rhino's Dir command.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface object</param>
    /// <returns>(bool) The current normal orientation.</returns>
    static member IsSurfaceFliped(surfaceId:Guid) : bool = //GET
        let surfaceIdObject = RhinoScriptSyntax.CoerceBrep(surfaceId)
        RhinoScriptSyntax.IsSurfaceFliped(surfaceIdObject)

    /// <summary>Returns the normal direction of a Surface. This feature can
    /// also be found in Rhino's Dir command. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Brep) Object of a Surface object</param>
    /// <returns>(bool) The current normal orientation.</returns>
    static member IsSurfaceFliped(surfaceGeo:Brep) : bool = //GET
        let brep = surfaceGeo
        if brep.Faces.Count>1 then RhinoScriptingException.Raise "IsSurfaceFliped failed. surfaceGeo:'%s'" (Pretty.str surfaceGeo)
        let face = brep.Faces.[0]
        face.OrientationIsReversed


    /// <summary>Changes the normal direction of a Surface. This feature can
    /// also be found in Rhino's Dir command.
    /// Use RhinoScriptSyntax.IsSurfaceFliped to get State.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface object</param>
    /// <param name="flip">(bool) New normal orientation, either flipped(True) or not flipped (False)</param>
    /// <returns>(unit) void, nothing.</returns>
    static member FlipSurface(surfaceId:Guid, flip:bool) : unit = //SET
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        if brep.Faces.Count>1 then RhinoScriptingException.Raise "FlipSurface failed. surfaceId:'%s' flip:'%A'" (Pretty.str surfaceId) flip
        let face = brep.Faces.[0]
        let oldreverse = face.OrientationIsReversed
        if brep.IsSolid = false && oldreverse <> flip then
            brep.Flip()
            State.Doc.Objects.Replace(surfaceId, brep)|> ignore<bool>
            State.Doc.Views.Redraw()

    /// <summary>Changes the normal direction of multiple Surface. This feature can
    /// also be found in Rhino's Dir command.
    /// Use RhinoScriptSyntax.IsSurfaceFliped to get State.</summary>
    /// <param name="surfaceIds">(Guid seq) Identifiers of multiple Surface objects</param>
    /// <param name="flip">(bool) New normal orientation, either flipped(True) or not flipped (False)</param>
    /// <returns>(unit) void, nothing.</returns>
    static member FlipSurface(surfaceIds:Guid seq, flip:bool) : unit = //MULTISET
        for surfaceId in surfaceIds do
            let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
            if brep.Faces.Count>1 then RhinoScriptingException.Raise "FlipSurface failed.  surfaceId:'%s' flip:'%A'" (Pretty.str surfaceId) flip
            let face = brep.Faces.[0]
            let oldreverse = face.OrientationIsReversed
            if brep.IsSolid = false && oldreverse <> flip then
                brep.Flip()
                State.Doc.Objects.Replace(surfaceId, brep)|> ignore<bool>
        State.Doc.Views.Redraw()


    /// <summary>Intersects a brep object with another brep object. Note, unlike the
    ///    SurfaceSurfaceIntersection function this function works on trimmed Surfaces.</summary>
    /// <param name="brep1">(Guid) Identifier of first brep object</param>
    /// <param name="brep2">(Guid) Identifier of second brep object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Distance tolerance at segment midpoints. If omitted,
    ///    the current absolute tolerance is used</param>
    /// <returns>(Guid ResizeArray) identifying the newly created intersection Curve and point objects.</returns>
    static member IntersectBreps( brep1:Guid,
                                  brep2:Guid,
                                  [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let brep1Geometry = RhinoScriptSyntax.CoerceBrep(brep1)
        let brep2Geometry = RhinoScriptSyntax.CoerceBrep(brep2)
        let geometry = RhinoScriptSyntax.IntersectBreps(brep1Geometry, brep2Geometry, tolerance)
        let ids = ResizeArray<Guid>()
        for item in geometry do
            let id =
                match item with
                | :? Curve as curve ->
                    let id = State.Doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    id
                | :? Point as point -> State.Doc.Objects.AddPoint(point.Location)
                | _ -> RhinoScriptingException.Raise "IntersectBreps: unexpected intersection geometry."
            if id = Guid.Empty then RhinoScriptingException.Raise "IntersectBreps: unable to add geometry to the document."
            ids.Add(id)
        if ids.Count > 0 then State.Doc.Views.Redraw()
        ids


    /// <summary>Intersects a brep object with another brep object. Note, unlike the
    ///    SurfaceSurfaceIntersection function this function works on trimmed Surfaces. Returns geometry without adding it.</summary>
    /// <param name="brep1">(Brep) Object of first brep object</param>
    /// <param name="brep2">(Brep) Object of second brep object</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Distance tolerance at segment midpoints. If omitted,
    ///    the current absolute tolerance is used</param>
    /// <returns>(GeometryBase ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member IntersectBreps( brep1:Brep,
                                  brep2:Brep,
                                  [<OPT;DEF(0.0)>]tolerance:float) : GeometryBase ResizeArray =
        let tolerance = Util.ifZero2 State.Doc.ModelAbsoluteTolerance tolerance
        let ok, curves, points = Intersect.Intersection.BrepBrep(brep1, brep2, tolerance)
        let geometry = ResizeArray<GeometryBase>()
        if ok then
            let joined = Curve.JoinCurves(curves, 2.1 * tolerance)
            let curves = if isNull joined then curves else joined
            for curve in curves do
                if curve.IsValid then geometry.Add(curve)
            for point in points do geometry.Add(new Point(point))
        geometry


    /// <summary>Calculates intersections of two spheres.</summary>
    /// <param name="spherePlane0">(Plane) An equatorial Plane of the first sphere. The origin of the
    ///    Plane will be the center point of the sphere</param>
    /// <param name="sphereRadius0">(float) Radius of the first sphere</param>
    /// <param name="spherePlane1">(Plane) Plane for second sphere</param>
    /// <param name="sphereRadius1">(float) Radius for second sphere</param>
    /// <returns>(int * Circle * float ) of intersection results
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
            RhinoScriptingException.Raise "IntersectSpheres failed.  spherePlane0:'%A' sphereRadius0:'%A' spherePlane1:'%A' sphereRadius1:'%A'" spherePlane0 sphereRadius0 spherePlane1 sphereRadius1



    /// <summary>Determines if a Surface is a portion of a cone.
    /// Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The Surface object's identifier</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsCone(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(objectId) with
        | Some geometry -> RhinoScriptSyntax.IsCone(geometry)
        | None -> false

    /// <summary>Determines if a Surface is a portion of a cone.
    /// Returns false for any other Rhino object or if the object does not exist. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(Surface) The Surface object</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsCone(geometryObject:Surface) : bool =
        match (Option.ofObj geometryObject) with
        | Some surface -> surface.IsCone()
        | None -> false


    /// <summary>Determines if a Surface is a portion of a cylinder.
    /// Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The cylinder object's identifier</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsCylinder(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(objectId) with
        | Some geometry -> RhinoScriptSyntax.IsCylinder(geometry)
        | None -> false

    /// <summary>Determines if a Surface is a portion of a cylinder.
    /// Returns false for any other Rhino object or if the object does not exist. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(Surface) The cylinder object</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsCylinder(geometryObject:Surface) : bool =
        match (Option.ofObj geometryObject) with
        | Some surface -> surface.IsCylinder()
        | None -> false


    /// <summary>Verifies an object is a Plane Surface. Plane Surfaces can be created by
    ///    the Plane command. Note, a Plane Surface is not a planar NURBS Surface.
    ///    Returns false for any other Rhino object or if the object does not exist.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsPlaneSurface(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(objectId) with
        | Some geometry -> RhinoScriptSyntax.IsPlaneSurface(geometry)
        | None -> false

    /// <summary>Verifies an object is a Plane Surface. Plane Surfaces can be created by
    ///    the Plane command. Note, a Plane Surface is not a planar NURBS Surface.
    ///    Returns false for any other Rhino object or if the object does not exist. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(Surface) The object</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsPlaneSurface(geometryObject:Surface) : bool =
        match (Option.ofObj geometryObject) with
        | Some (:? BrepFace  as bface) ->
            if bface.IsSurface then
                match bface.UnderlyingSurface() with
                | :?  PlaneSurface -> true
                | _ -> false
            else
                false
        | _ -> false


    /// <summary>Checks if a point is inside a closed Mesh , Surface or Polysurface.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <param name="point">(Point3d) The test, or sampling point</param>
    /// <param name="strictlyIn">(bool) Optional, default value: <c>false</c>
    ///    If true, the test point must be inside by at least tolerance</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.SqrtEpsilon</c>
    ///    Distance tolerance used for intersection and determining
    ///    strict inclusion.</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsPointInSurface( objectId:Guid,
                                    point:Point3d,
                                    [<OPT;DEF(false)>]strictlyIn:bool,
                                    [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let objectIdObject = RhinoScriptSyntax.CoerceGeometry(objectId)
        RhinoScriptSyntax.IsPointInSurface(objectIdObject, point, strictlyIn, tolerance)

    /// <summary>Checks if a point is inside a closed Mesh , Surface or Polysurface. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The object</param>
    /// <param name="point">(Point3d) The test, or sampling point</param>
    /// <param name="strictlyIn">(bool) Optional, default value: <c>false</c>
    ///    If true, the test point must be inside by at least tolerance</param>
    /// <param name="tolerance">(float) Optional, default value: <c>RhinoMath.SqrtEpsilon</c>
    ///    Distance tolerance used for intersection and determining
    ///    strict inclusion.</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsPointInSurface( geometryObject:GeometryBase,
                                    point:Point3d,
                                    [<OPT;DEF(false)>]strictlyIn:bool,
                                    [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let obj = geometryObject
        let  tolerance= Util.ifZero1 tolerance RhinoMath.SqrtEpsilon
        match obj with
        | :? Extrusion as es->
            let brep= es.ToBrep(false)
            if not brep.IsSolid then RhinoScriptingException.Raise "IsPointInSurface failed on not closed Extrusion %A"  geometryObject
            brep.IsPointInside(point, tolerance, strictlyIn)
        | :? Brep as bo->
            let br= bo
            if not br.IsSolid then RhinoScriptingException.Raise "IsPointInSurface failed on not closed Brep %A"  geometryObject
            br.IsPointInside(point, tolerance, strictlyIn)
        | :? Mesh as mo ->
            let me = mo
            if not me.IsClosed then RhinoScriptingException.Raise "IsPointInSurface failed on not closed Mesh %A"  geometryObject
            me.IsPointInside(point, tolerance, strictlyIn)
        | _ -> RhinoScriptingException.Raise "IsPointInSurface does not work  on %s %A" (Pretty.str geometryObject) geometryObject


    /// <summary>Checks if a point lies on a Surface.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <param name="point">(Point3d) The test, or sampling point</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsPointOnSurface(objectId:Guid, point:Point3d) : bool =
        let geometry = RhinoScriptSyntax.CoerceGeometry(objectId)
        RhinoScriptSyntax.IsPointOnSurface(geometry, point)

    /// <summary>Checks if a point lies on a Surface. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The object</param>
    /// <param name="point">(Point3d) The test, or sampling point</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsPointOnSurface(geometryObject:GeometryBase, point:Point3d) : bool =
        let tryGetSurface (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Surface as surface -> Some surface | :? Brep as brep when brep.Faces.Count = 1 -> Some (brep.Faces.[0] :> Surface) | _ -> None
        let getSurface value =
            match tryGetSurface value with
            | Some geometry -> geometry
            | None -> RhinoScriptingException.Raise "IsPointOnSurface: expected Surface geometry."
        let surf = getSurface(geometryObject)
        let mutable rc, u, v = surf.ClosestPoint(point)
        if rc  then
            let srfpt = surf.PointAt(u, v)
            if srfpt.DistanceTo(point) > State.Doc.ModelAbsoluteTolerance then
                rc <- false
            else
                match surf with
                | :? BrepFace as face ->
                    rc <- face.IsPointOnFace(u, v) <> PointFaceRelation.Exterior
                | _ -> ()
        else
            RhinoScriptingException.Raise "IsPointOnSurface failed for surf.ClosestPoint on %A %A" (Pretty.str geometryObject) point
        rc


    /// <summary>Verifies an object is a Polysurface or Extrusion. Polysurfaces consist of two or more Surfaces joined together.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True if successful, otherwise False.</returns>
    static member IsPolysurface(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | Some geometry -> RhinoScriptSyntax.IsPolysurface(geometry)
        | None -> RhinoScriptingException.Raise "IsPolysurface: %A is not an object in State.Doc.Objects table" objectId

    /// <summary>Verifies an object is a Polysurface or Extrusion. Polysurfaces consist of two or more Surfaces joined together. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The object</param>
    /// <returns>(bool) True if successful, otherwise False.</returns>
    static member IsPolysurface(geometryObject:GeometryBase) : bool =
        match geometryObject with
        | :? Brep as b -> b.Faces.Count > 1
        | :? Extrusion  -> true
        | _ -> false


    /// <summary>Verifies an object is a or Extrusion. </summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True if successful, otherwise False.</returns>
    static member IsExtrusion(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | Some geometry -> RhinoScriptSyntax.IsExtrusion(geometry)
        | None -> RhinoScriptingException.Raise "IsExtrusion: %A is not an object in State.Doc.Objects table" objectId

    /// <summary>Verifies an object is a or Extrusion.  Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The object</param>
    /// <returns>(bool) True if successful, otherwise False.</returns>
    static member IsExtrusion(geometryObject:GeometryBase) : bool =
        match geometryObject with
        | :? Extrusion  -> true
        | _ -> false


    /// <summary>Verifies an object is a Brep. That is a trimmed surface or a polysurface but not an extrusion.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True if successful, otherwise False.</returns>
    static member IsBrep(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | Some geometry -> RhinoScriptSyntax.IsBrep(geometry)
        | None -> RhinoScriptingException.Raise "IsBrep: %A is not an object in State.Doc.Objects table" objectId

    /// <summary>Verifies an object is a Brep. That is a trimmed surface or a polysurface but not an extrusion. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The object</param>
    /// <returns>(bool) True if successful, otherwise False.</returns>
    static member IsBrep(geometryObject:GeometryBase) : bool =
        match geometryObject with
        | :? Brep  -> true
        | _ -> false


    /// <summary>Verifies a Guid refers to a closed Polysurface or closed Extrusion. If the Polysurface fully encloses
    ///    a volume, it is considered a solid.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True if successful, otherwise False.</returns>
    static member IsPolysurfaceClosed(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | Some geometry -> RhinoScriptSyntax.IsPolysurfaceClosed(geometry)
        | None -> RhinoScriptingException.Raise "IsPolysurfaceClosed: %A is not an object in State.Doc.Objects table" objectId

    /// <summary>Verifies a Guid refers to a closed Polysurface or closed Extrusion. If the Polysurface fully encloses
    ///    a volume, it is considered a solid. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The object</param>
    /// <returns>(bool) True if successful, otherwise False.</returns>
    static member IsPolysurfaceClosed(geometryObject:GeometryBase) : bool =
        match geometryObject with
        | :? Brep as b -> b.IsSolid
        | :? Extrusion as e -> e.IsSolid
        | _ -> false


    /// <summary>Determines if a Surface is a portion of a Sphere.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsSphere(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(objectId) with
        | Some geometry -> RhinoScriptSyntax.IsSphere(geometry)
        | None -> false

    /// <summary>Determines if a Surface is a portion of a Sphere. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(Surface) The object</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsSphere(geometryObject:Surface) : bool =
        match (Option.ofObj geometryObject) with
        | Some b ->  b.IsSphere()
        | _ -> false


    /// <summary>Verifies an object is a Surface, Extrusion or Brep objects with only one face.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsSurface(objectId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(objectId) with
        | Some geometry -> RhinoScriptSyntax.IsSurface(geometry)
        | None -> false

    /// <summary>Verifies an object is a Surface, Extrusion or Brep objects with only one face. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(Surface) The object</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsSurface(geometryObject:Surface) : bool =
        (Option.ofObj geometryObject).IsSome


    /// <summary>Verifies a Surface object is closed in the specified direction. If the
    ///    Surface fully encloses a volume, it is considered a solid.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface</param>
    /// <param name="direction">(int)
    ///    0 = U direction check,
    ///    1 = V direction check</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfaceClosed(surfaceId:Guid, direction:int) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with
        | Some geometry -> RhinoScriptSyntax.IsSurfaceClosed(geometry, direction)
        | None -> false

    /// <summary>Verifies a Surface object is closed in the specified direction. If the
    ///    Surface fully encloses a volume, it is considered a solid. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) Object of a Surface</param>
    /// <param name="direction">(int)
    ///    0 = U direction check,
    ///    1 = V direction check</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfaceClosed(surfaceGeo:Surface, direction:int) : bool =
        match (Option.ofObj surfaceGeo) with
        | Some surface ->  surface.IsClosed(direction)
        | _ -> false


    /// <summary>Verifies a Surface object is periodic in the specified direction.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface</param>
    /// <param name="direction">(int)
    ///    0 = U direction check,
    ///    1 = V direction check</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfacePeriodic(surfaceId:Guid, direction:int) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with
        | Some geometry -> RhinoScriptSyntax.IsSurfacePeriodic(geometry, direction)
        | None -> false

    /// <summary>Verifies a Surface object is periodic in the specified direction. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) Object of a Surface</param>
    /// <param name="direction">(int)
    ///    0 = U direction check,
    ///    1 = V direction check</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfacePeriodic(surfaceGeo:Surface, direction:int) : bool =
        match (Option.ofObj surfaceGeo) with
        | Some surface ->  surface.IsPeriodic(direction)
        | _ -> false


    /// <summary>Verifies a Surface object is planar.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Tolerance used when checked. If omitted, the current absolute
    ///    tolerance is used</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfacePlanar(surfaceId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance = Util.ifZero1 tolerance State.Doc.ModelAbsoluteTolerance
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with
        | Some geometry -> RhinoScriptSyntax.IsSurfacePlanar(geometry, tolerance)
        | None -> false

    /// <summary>Verifies a Surface object is planar. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) Object of a Surface</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Tolerance used when checked. If omitted, the current absolute
    ///    tolerance is used</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfacePlanar(surfaceGeo:Surface, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        let tolerance = Util.ifZero1 tolerance State.Doc.ModelAbsoluteTolerance
        match (Option.ofObj surfaceGeo) with
        | Some surface ->  surface.IsPlanar(tolerance)
        | _ -> false


    /// <summary>Verifies a Surface object is rational.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsSurfaceRational(surfaceId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with
        | Some geometry -> RhinoScriptSyntax.IsSurfaceRational(geometry)
        | None -> false

    /// <summary>Verifies a Surface object is rational. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsSurfaceRational(surfaceGeo:Surface) : bool =
        match (Option.ofObj surfaceGeo) with //TODO better fail if input is not a surface ?? here and above functions
        | Some surface ->
            let ns = surface.ToNurbsSurface()
            if ns|> isNull  then false
            else ns.IsRational
        | _ -> false


    /// <summary>Verifies a Surface object is singular in the specified direction.
    ///    Surfaces are considered singular if a side collapses to a point.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="direction">(int)
    ///    0 = south
    ///    1 = east
    ///    2 = north
    ///    3 = west</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfaceSingular(surfaceId:Guid, direction:int) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with
        | Some geometry -> RhinoScriptSyntax.IsSurfaceSingular(geometry, direction)
        | None -> false

    /// <summary>Verifies a Surface object is singular in the specified direction.
    ///    Surfaces are considered singular if a side collapses to a point. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="direction">(int)
    ///    0 = south
    ///    1 = east
    ///    2 = north
    ///    3 = west</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfaceSingular(surfaceGeo:Surface, direction:int) : bool =
        match (Option.ofObj surfaceGeo) with    //TODO better fail if input is not a surface ?? here and above functions
        | Some surface ->
            surface.IsSingular(direction)
        | _ -> false


    /// <summary>Verifies a Surface object has been trimmed.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfaceTrimmed(surfaceId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceBrep(surfaceId) with
        | Some geometry -> RhinoScriptSyntax.IsSurfaceTrimmed(geometry)
        | None -> false

    /// <summary>Verifies a Surface object has been trimmed. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Brep) The Surface</param>
    /// <returns>(bool) True or False.</returns>
    static member IsSurfaceTrimmed(surfaceGeo:Brep) : bool =
        match (Option.ofObj surfaceGeo) with
        | Some brep ->  not brep.IsSurface
        | _ -> false //TODO better fail if input is not a surface ?? here and above functions


    /// <summary>Determines if a Surface is a portion of a torus.</summary>
    /// <param name="surfaceId">(Guid) The Surface object's identifier</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsTorus(surfaceId:Guid) : bool =
        match RhinoScriptSyntax.TryCoerceSurface(surfaceId) with
        | Some geometry -> RhinoScriptSyntax.IsTorus(geometry)
        | None -> false

    /// <summary>Determines if a Surface is a portion of a torus. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface object</param>
    /// <returns>(bool) True , otherwise False.</returns>
    static member IsTorus(surfaceGeo:Surface) : bool =
        match (Option.ofObj surfaceGeo) with //TODO better fail if input is not a surface ?? here and above functions
        | Some surface ->  surface.IsTorus()
        | _ -> false


    /// <summary>Gets the sphere definition from a Surface, if possible.</summary>
    /// <param name="surfaceId">(Guid) The identifier of the Surface object</param>
    /// <returns>(Plane * float) The equatorial Plane of the sphere, and its radius.</returns>
    static member SurfaceSphere(surfaceId:Guid) : Plane * float =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceSphere(surfaceIdObject)

    /// <summary>Gets the sphere definition from a Surface, if possible. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface object</param>
    /// <returns>(Plane * float) The equatorial Plane of the sphere, and its radius.</returns>
    static member SurfaceSphere(surfaceGeo:Surface) : Plane * float =
        let surface = surfaceGeo
        let tol = State.Doc.ModelAbsoluteTolerance
        let sphere = ref Sphere.Unset
        let issphere = surface.TryGetSphere(sphere, tol)
        if issphere then (!sphere).EquatorialPlane, (!sphere).Radius
        else RhinoScriptingException.Raise "SurfaceSphere input is not a sphere %A"surfaceGeo


    /// <summary>Joins two or more Surface or Polysurface objects together to form one
    ///    Polysurface object.</summary>
    /// <param name="objectIds">(Guid seq) List of object identifiers</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Delete the original Surfaces</param>
    /// <returns>(Guid) identifier of newly created object.</returns>
    static member JoinSurfaces(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let objectIds = objectIds |> Seq.toArray
        let objectIdsGeometry = objectIds |> RArr.mapSeq RhinoScriptSyntax.CoerceBrep
        let geometry = RhinoScriptSyntax.JoinSurfaces(objectIdsGeometry)
        let rc = State.Doc.Objects.AddBrep(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "JoinSurfaces: unable to add geometry to the document."
        if deleteInput then
            for id in objectIds do State.Doc.Objects.Delete(id, true) |> ignore<bool>
        State.Doc.Views.Redraw()
        rc


    /// <summary>Joins two or more Surface or Polysurface objects together to form one
    ///    Polysurface object. Returns geometry without adding it.</summary>
    /// <param name="geometryObjects">(Brep seq) List of objects</param>
    /// <returns>(Brep) Newly created geometry. The caller owns the returned objects.</returns>
    static member JoinSurfaces(geometryObjects:Brep seq) : Brep =
        let breps = geometryObjects |> Seq.toArray
        if breps.Length < 2 then RhinoScriptingException.Raise "JoinSurfaces: at least two Breps are required."
        let joined = Brep.JoinBreps(breps, State.Doc.ModelAbsoluteTolerance * 2.1)
        if isNull joined then RhinoScriptingException.Raise "JoinSurfaces: operation failed."
        if joined.Length <> 1 then RhinoScriptingException.Raise "JoinSurfaces resulted in more than one object: %d" joined.Length
        joined.[0]


    /// <summary>Makes an existing Surface a periodic NURBS Surface.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="direction">(int) The direction to make periodic, either
    ///    0 = U
    ///    1 = V</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Delete the input Surface</param>
    /// <returns>(Guid) if deleteInput is False, identifier of the new Surface.</returns>
    static member MakeSurfacePeriodic( surfaceId:Guid,
                                       direction:int,
                                       [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let surface = RhinoScriptSyntax.MakeSurfacePeriodic(surfaceIdGeometry, direction)
        let id =
            if deleteInput then
                State.Doc.Objects.Replace(surfaceId, surface) |> ignore<bool>
                surfaceId
            else State.Doc.Objects.AddSurface(surface)
        State.Doc.Views.Redraw()
        id


    /// <summary>Makes an existing Surface a periodic NURBS Surface. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="direction">(int) The direction to make periodic, either
    ///    0 = U
    ///    1 = V</param>
    /// <returns>(Surface) Newly created geometry. The caller owns the returned objects.</returns>
    static member MakeSurfacePeriodic( surfaceGeo:Surface,
                                       direction:int) : Surface =
        let surface = Surface.CreatePeriodicSurface(surfaceGeo, direction)
        if isNull surface then RhinoScriptingException.Raise "MakeSurfacePeriodic: operation failed."
        surface


    /// <summary>Offsets a trimmed or untrimmed Surface by a distance. The offset Surface
    ///    will be added to Rhino.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="distance">(float) The distance to offset</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The offset tolerance. Use 0.0 to make a loose offset. Otherwise, the
    ///    document's absolute tolerance is usually sufficient</param>
    /// <param name="bothSides">(bool) Optional, default value: <c>false</c>
    ///    Offset to both sides of the input Surface</param>
    /// <param name="createSolid">(bool) Optional, default value: <c>false</c>
    ///    Make a solid object</param>
    /// <returns>(Guid) identifier of the new object.</returns>
    static member OffsetSurface( surfaceId:Guid,
                                 distance:float,
                                 [<OPT;DEF(0.0)>]tolerance:float,
                                 [<OPT;DEF(false)>]bothSides:bool,
                                 [<OPT;DEF(false)>]createSolid:bool) : Guid =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceBrep(surfaceId)
        let geometry = RhinoScriptSyntax.OffsetSurface(surfaceIdGeometry, distance, tolerance, bothSides, createSolid)
        let rc = State.Doc.Objects.AddBrep(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "OffsetSurface: unable to add geometry to the document."
        State.Doc.Views.Redraw()
        rc


    /// <summary>Offsets a trimmed or untrimmed Surface by a distance. The result is returned as a new Brep. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Brep) The Surface</param>
    /// <param name="distance">(float) The distance to offset</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    The offset tolerance. Use 0.0 to make a loose offset. Otherwise, the
    ///    document's absolute tolerance is usually sufficient</param>
    /// <param name="bothSides">(bool) Optional, default value: <c>false</c>
    ///    Offset to both sides of the input Surface</param>
    /// <param name="createSolid">(bool) Optional, default value: <c>false</c>
    ///    Make a solid object</param>
    /// <returns>(Brep) Newly created geometry. The caller owns the returned objects.</returns>
    static member OffsetSurface( surfaceGeo:Brep,
                                 distance:float,
                                 [<OPT;DEF(0.0)>]tolerance:float,
                                 [<OPT;DEF(false)>]bothSides:bool,
                                 [<OPT;DEF(false)>]createSolid:bool) : Brep =
        let brep = surfaceGeo
        let mutable face = null
        if (1 = brep.Faces.Count) then face <- brep.Faces.[0]
        if face|> isNull  then RhinoScriptingException.Raise "OffsetSurface failed.  surfaceGeo:'%s' distance:'%A' tolerance:'%A' bothSides:'%A' createSolid:'%A'" (Pretty.str surfaceGeo) distance tolerance bothSides createSolid
        let tolerance= Util.ifZero1 tolerance State.Doc.ModelAbsoluteTolerance
        let newbrep = Brep.CreateFromOffsetFace(face, distance, tolerance, bothSides, createSolid)
        if newbrep|> isNull  then RhinoScriptingException.Raise "OffsetSurface failed.  surfaceGeo:'%s' distance:'%A' tolerance:'%A' bothSides:'%A' createSolid:'%A'" (Pretty.str surfaceGeo) distance tolerance bothSides createSolid
        newbrep


    /// <summary>Pulls a Curve object to a Surface object.</summary>
    /// <param name="surface">(Guid) The Surface's identifier</param>
    /// <param name="curve">(Guid) The Curve's identifier</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Should the input items be deleted</param>
    /// <returns>(Guid ResizeArray) List of new Curves.</returns>
    static member PullCurve( surface:Guid,
                             curve:Guid,
                             [<OPT;DEF(false)>]deleteInput:bool) : Guid ResizeArray =
        let surfaceGeometry = RhinoScriptSyntax.CoerceBrep(surface)
        let curveGeometry = RhinoScriptSyntax.CoerceCurve(curve)
        let curves = RhinoScriptSyntax.PullCurve(surfaceGeometry, curveGeometry)
        let ids = curves |> RArr.mapSeq State.Doc.Objects.AddCurve
        if deleteInput then State.Doc.Objects.Delete(curve, true) |> ignore<bool>
        State.Doc.Views.Redraw()
        ids


    /// <summary>Pulls a Curve object to a Surface object. Returns geometry without adding it.</summary>
    /// <param name="surface">(Brep) The Surface</param>
    /// <param name="curve">(Curve) The Curve</param>
    /// <returns>(Curve ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member PullCurve( surface:Brep,
                             curve:Curve) : Curve ResizeArray =
        let curves = Curve.PullToBrepFace(curve, surface.Faces.[0], State.Doc.ModelAbsoluteTolerance)
        if isNull curves then RhinoScriptingException.Raise "PullCurve: operation failed."
        ResizeArray(curves)


    /// <summary>Rebuilds a Surface to a given degree and control point count. For more
    ///    information see the Rhino help file for the Rebuild command.</summary>
    /// <param name="objectId">(Guid) The Surface's identifier</param>
    /// <param name="degreeU">(int) Optional, default value: <c>3</c> degree of the Surface in the U directions</param>
    /// <param name="degreeV">(int) Optional, default value: <c>3</c> degree of the Surface in the V directions</param>
    /// <param name="pointcountU">(int) Optional, default value: <c>10*</c> the Surface point count in U  directions</param>
    /// <param name="pointcountV">(int) Optional, default value: <c>10*</c> the Surface point count in V  directions</param>
    /// <returns>(bool) True of False indicating success or failure.</returns>
    static member RebuildSurface( objectId:Guid,
                                  [<OPT;DEF(3)>]degreeU:int,
                                  [<OPT;DEF(3)>]degreeV:int,
                                  [<OPT;DEF(10)>]pointcountU:int ,
                                  [<OPT;DEF(10)>]pointcountV:int ) : bool =

        let surface = RhinoScriptSyntax.CoerceSurface(objectId)
        let newsrf = surface.Rebuild( degreeU, degreeV, pointcountU, pointcountV )
        if newsrf|> isNull  then false
        else
            //objectId = RhinoScriptSyntax.CoerceGuid(objectId)
            let rc = State.Doc.Objects.Replace(objectId, newsrf)
            if rc then State.Doc.Views.Redraw()
            rc


    /// <summary>Deletes a knot from a Surface object.</summary>
    /// <param name="surface">(Guid) The reference of the Surface object</param>
    /// <param name="uvParameter">(float * float)) An indexable item containing a U, V parameter on the Surface. List, tuples and UVIntervals will work.
    ///    Note, if the parameter is not equal to one of the existing knots, then the knot closest to the specified parameter will be removed</param>
    /// <param name="vDirection">(bool) If True, or 1, the V direction will be addressed. If False, or 0, the U direction</param>
    /// <returns>(bool) True of False indicating success or failure.</returns>
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
                    State.Doc.Objects.Replace(surface, nsrf)|> ignore<bool>
                    State.Doc.Views.Redraw()
                    true


    /// <summary>Reverses U or V directions of a Surface, or swaps (transposes) U and V
    ///    directions.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface object</param>
    /// <param name="direction">(int) As a bit coded flag to swap
    ///    1 = reverse U
    ///    2 = reverse V
    ///    4 = transpose U and V (values can be combined)</param>
    /// <returns>(unit) void, nothing.</returns>
    static member ReverseSurface(surfaceId:Guid, direction:int) : unit =
        let brep = RhinoScriptSyntax.CoerceBrep(surfaceId)
        if brep.Faces.Count <> 1 then RhinoScriptingException.Raise "ReverseSurface failed.  surfaceId:'%s' direction:'%A'" (Pretty.str surfaceId) direction
        let face = brep.Faces.[0]
        if direction &&& 1 <> 0 then            face.Reverse(0, true)|> ignore<Surface>
        if direction &&& 2 <> 0 then            face.Reverse(1, true)|> ignore<Surface>
        if direction &&& 4 <> 0 then            face.Transpose(true) |> ignore<Surface>
        State.Doc.Objects.Replace(surfaceId, brep)|> ignore<bool>
        State.Doc.Views.Redraw()



    /// <summary>Shoots a ray at a collection of Surfaces or Polysurfaces.</summary>
    /// <param name="surfaceIds">(Guid seq) One of more Surface identifiers</param>
    /// <param name="startPoint">(Point3d) Starting point of the ray</param>
    /// <param name="direction">(Vector3d) Vector identifying the direction of the ray</param>
    /// <param name="reflections">(int) Optional, default value: <c>10</c>
    ///    The maximum number of times the ray will be reflected</param>
    /// <returns>(Point3d array) Array of reflection points.</returns>
    static member ShootRay( surfaceIds:Guid seq,
                            startPoint:Point3d,
                            direction:Vector3d,
                            [<OPT;DEF(10)>]reflections:int) : Point3d array =
        let surfaceIdsObjects = surfaceIds |> Seq.map RhinoScriptSyntax.CoerceBrep
        RhinoScriptSyntax.ShootRay(surfaceIdsObjects, startPoint, direction, reflections)

    /// <summary>Shoots a ray at a collection of Surfaces or Polysurfaces. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeos">(Brep seq) One of more Surface objects</param>
    /// <param name="startPoint">(Point3d) Starting point of the ray</param>
    /// <param name="direction">(Vector3d) Vector identifying the direction of the ray</param>
    /// <param name="reflections">(int) Optional, default value: <c>10</c>
    ///    The maximum number of times the ray will be reflected</param>
    /// <returns>(Point3d array) Array of reflection points.</returns>
    static member ShootRay( surfaceGeos:Brep seq,
                            startPoint:Point3d,
                            direction:Vector3d,
                            [<OPT;DEF(10)>]reflections:int) : Point3d array =
        let ray = Ray3d(startPoint, direction)
        let breps = surfaceGeos |> RArr.mapSeq (fun geometry -> geometry)
        if breps.Count = 0 then RhinoScriptingException.Raise "ShootRay failed.  surfaceGeos:'%A' startPoint:'%A' direction:'%A' reflections:'%A'"  surfaceGeos startPoint direction reflections
        Intersect.Intersection.RayShoot(ray, Seq.cast breps, reflections)


    /// <summary>Creates the shortest possible Curve(geodesic) between two points on a
    ///    Surface. For more details, see the ShortPath command in Rhino help.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface</param>
    /// <param name="startPoint">(Point3d) Start point the short Curve</param>
    /// <param name="endPoint">(Point3d) End point of the short Curve</param>
    /// <returns>(Guid) identifier of the new Surface.</returns>
    static member ShortPath( surfaceId:Guid,
                             startPoint:Point3d,
                             endPoint:Point3d) : Guid =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let geometry = RhinoScriptSyntax.ShortPath(surfaceIdGeometry, startPoint, endPoint)
        let rc = State.Doc.Objects.AddCurve(geometry)
        if rc = Guid.Empty then RhinoScriptingException.Raise "ShortPath: unable to add geometry to the document."
        State.Doc.Views.Redraw()
        rc


    /// <summary>Creates the shortest possible Curve(geodesic) between two points on a
    ///    Surface. For more details, see the ShortPath command in Rhino help. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Surface) Object of a Surface</param>
    /// <param name="startPoint">(Point3d) Start point the short Curve</param>
    /// <param name="endPoint">(Point3d) End point of the short Curve</param>
    /// <returns>(Curve) Newly created geometry. The caller owns the returned objects.</returns>
    static member ShortPath( surfaceGeo:Surface,
                             startPoint:Point3d,
                             endPoint:Point3d) : Curve =
        let surface = surfaceGeo
        let rcstart, ustart, vstart = surface.ClosestPoint(startPoint)
        let rcend, uend, vend = surface.ClosestPoint(endPoint)
        if not rcstart || not rcend then RhinoScriptingException.Raise "ShortPath failed.  surfaceGeo:'%s' startPoint:'%A' endPoint:'%A'" (Pretty.str surfaceGeo) startPoint endPoint
        let start = Point2d(ustart, vstart)
        let ende = Point2d(uend, vend)
        let tolerance = State.Doc.ModelAbsoluteTolerance
        let curve = surface.ShortPath(start, ende, tolerance)
        if curve|> isNull  then RhinoScriptingException.Raise "ShortPath failed.  surfaceGeo:'%s' startPoint:'%A' endPoint:'%A'" (Pretty.str surfaceGeo) startPoint endPoint
        curve


    /// <summary>Shrinks the underlying untrimmed Surfaces near to the trimming
    ///    boundaries. See the ShrinkTrimmedSrf command in the Rhino help.</summary>
    /// <param name="objectId">(Guid) The Surface's identifier</param>
    /// <param name="createCopy">(bool) Optional, default value: <c>false</c>
    ///    If True, the original Surface is not deleted</param>
    /// <returns>(Guid) If createCopy is true the new Guid, else the input Guid.</returns>
    static member ShrinkTrimmedSurface(objectId:Guid, [<OPT;DEF(false)>]createCopy:bool) : Guid =
        let objectIdGeometry = RhinoScriptSyntax.CoerceBrep(objectId)
        let brep = RhinoScriptSyntax.ShrinkTrimmedSurface(objectIdGeometry)
        let id =
            if createCopy then
                let attributes = (RhinoScriptSyntax.CoerceRhinoObject(objectId)).Attributes
                State.Doc.Objects.AddBrep(brep, attributes)
            else
                State.Doc.Objects.Replace(objectId, brep) |> ignore<bool>
                objectId
        State.Doc.Views.Redraw()
        id


    /// <summary>Shrinks the underlying untrimmed Surfaces near to the trimming
    ///    boundaries. See the ShrinkTrimmedSrf command in the Rhino help. Returns geometry without adding it.</summary>
    /// <param name="geometryObject">(Brep) The Surface</param>
    /// <returns>(Brep) Newly created geometry. The caller owns the returned objects.</returns>
    static member ShrinkTrimmedSurface(geometryObject:Brep) : Brep =
        let brep = geometryObject.DuplicateBrep()
        if not (brep.Faces.ShrinkFaces()) then RhinoScriptingException.Raise "ShrinkTrimmedSurface failed."
        brep


    /// <summary>Splits a brep.</summary>
    /// <param name="brepId">(Guid) Identifier of the brep to split</param>
    /// <param name="cutterId">(Guid) Identifier of the brep to split with</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Delete input breps</param>
    /// <returns>(Guid ResizeArray) identifiers of split pieces.</returns>
    static member SplitBrep( brepId:Guid,
                             cutterId:Guid,
                             [<OPT;DEF(false)>]deleteInput:bool) : Guid ResizeArray =
        let brepIdGeometry = RhinoScriptSyntax.CoerceBrep(brepId)
        let cutterIdGeometry = RhinoScriptSyntax.CoerceBrep(cutterId)
        let pieces = RhinoScriptSyntax.SplitBrep(brepIdGeometry, cutterIdGeometry)
        if deleteInput then State.Doc.Objects.Delete(brepId, true) |> ignore<bool>
        let ids = pieces |> RArr.mapSeq State.Doc.Objects.AddBrep
        State.Doc.Views.Redraw()
        ids


    /// <summary>Splits a brep. Returns geometry without adding it.</summary>
    /// <param name="brepGeo">(Brep) Object of the brep to split</param>
    /// <param name="cutterGeo">(Brep) Object of the brep to split with</param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member SplitBrep( brepGeo:Brep,
                             cutterGeo:Brep) : Brep ResizeArray =
        let pieces = brepGeo.Split(cutterGeo, State.Doc.ModelAbsoluteTolerance)
        if isNull pieces then RhinoScriptingException.Raise "SplitBrep: operation failed."
        ResizeArray(pieces)


    /// <summary>Calculate the area of a Surface Geometry.
    /// The results are based on the current drawing units.</summary>
    /// <param name="srf">(Geometry.Surface) The Surface geometry</param>
    /// <returns>(float) of area.</returns>
    static member SurfaceArea(srf:Surface) : float  =
        let amp =AreaMassProperties.Compute(srf, area=true, firstMoments=false, secondMoments=false, productMoments=false)
        if isNull amp then  RhinoScriptingException.Raise "SurfaceArea failed on Surface: %A" srf
        amp.Area


    /// <summary>Calculate the area of a Brep / Polysurface  Geometry.
    /// The results are based on the current drawing units.</summary>
    /// <param name="brep">(Geometry.Brep) The Polysurface geometry</param>
    /// <returns>(float) of area.</returns>
    static member SurfaceArea(brep:Brep) : float  =
        let amp = AreaMassProperties.Compute(brep, area=true, firstMoments=false, secondMoments=false, productMoments=false)
        if isNull amp then  RhinoScriptingException.Raise "SurfaceArea failed on Brep: %A" brep
        amp.Area

    /// <summary>Calculate the area of a Surface or Polysurface object.
    /// The results are based on the current drawing units.</summary>
    /// <param name="objectId">(Guid) The Surface's identifier</param>
    /// <returns>(float) of area.</returns>
    static member SurfaceArea(objectId:Guid) : float  =
        let objectIdObject = RhinoScriptSyntax.CoerceGeometry(objectId)
        RhinoScriptSyntax.SurfaceArea(objectIdObject)

    /// <summary>Calculate the area of a Surface or Polysurface object.
    /// The results are based on the current drawing units. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The Surface</param>
    /// <returns>(float) of area.</returns>
    static member SurfaceArea(geometryObject:GeometryBase) : float  =
        match geometryObject with
        | :? Surface as s -> RhinoScriptSyntax.SurfaceArea s
        | :? Brep    as b -> RhinoScriptSyntax.SurfaceArea b
        | _ -> RhinoScriptingException.Raise "SurfaceArea doesnt work on on %A" (Pretty.str  geometryObject)


    /// <summary>Calculates the area centroid of a Surface or Polysurface.</summary>
    /// <param name="objectId">(Guid) The Surface's identifier</param>
    /// <returns>(Point3d ) Area centroid.</returns>
    static member SurfaceAreaCentroid(objectId:Guid) : Point3d =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | Some geometry -> RhinoScriptSyntax.SurfaceAreaCentroid(geometry)
        | None -> RhinoScriptingException.Raise "SurfaceAreaCentroid failed.  objectId:'%s'" (Pretty.str objectId)

    /// <summary>Calculates the area centroid of a Surface or Polysurface. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The Surface</param>
    /// <returns>(Point3d ) Area centroid.</returns>
    static member SurfaceAreaCentroid(geometryObject:GeometryBase) : Point3d =
        let tryGetBrep (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Brep as brep -> Some brep | :? Extrusion as extrusion -> Some (extrusion.ToBrep(true)) | _ -> None
        let tryGetSurface (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Surface as surface -> Some surface | :? Brep as brep when brep.Faces.Count = 1 -> Some (brep.Faces.[0] :> Surface) | _ -> None
        geometryObject
        |> tryGetBrep
        |> Option.map AreaMassProperties.Compute
        |> Option.orElseWith (fun () ->
            geometryObject
            |> tryGetSurface
            |> Option.map AreaMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "SurfaceAreaCentroid failed.  geometryObject:'%s'" (Pretty.str geometryObject))
        |> fun amp -> amp.Centroid


    /// <summary>Calculates area moments of inertia of a Surface or Polysurface object.
    ///    See the Rhino help for "Mass Properties calculation details".</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>((float*float*float) ResizeArray ) of moments and error bounds in tuples(X, Y, Z) - see help topic
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
    ///    [9]     (Not implemented yet) The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///    [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///    [13]    (Not implemented yet) The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.</returns>
    static member SurfaceAreaMoments(surfaceId:Guid) : (float*float*float) ResizeArray =
        match RhinoScriptSyntax.TryCoerceGeometry(surfaceId) with
        | Some geometry -> RhinoScriptSyntax.SurfaceAreaMoments(geometry)
        | None -> RhinoScriptingException.Raise "SurfaceAreaMoments failed on %A" surfaceId

    /// <summary>Calculates area moments of inertia of a Surface or Polysurface object.
    ///    See the Rhino help for "Mass Properties calculation details". Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(GeometryBase) The Surface</param>
    /// <returns>((float*float*float) ResizeArray ) of moments and error bounds in tuples(X, Y, Z) - see help topic
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
    ///    [9]     (Not implemented yet) The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///    [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///    [13]    (Not implemented yet) The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.</returns>
    static member SurfaceAreaMoments(surfaceGeo:GeometryBase) : (float*float*float) ResizeArray =
        let tryGetBrep (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Brep as brep -> Some brep | :? Extrusion as extrusion -> Some (extrusion.ToBrep(true)) | _ -> None
        let tryGetSurface (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Surface as surface -> Some surface | :? Brep as brep when brep.Faces.Count = 1 -> Some (brep.Faces.[0] :> Surface) | _ -> None
        surfaceGeo
        |> tryGetBrep
        |> Option.map AreaMassProperties.Compute
        |> Option.orElseWith (fun () ->
            surfaceGeo
            |> tryGetSurface
            |> Option.map AreaMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "SurfaceAreaMoments failed on %A" surfaceGeo)
        |> fun mp ->
            let r = ResizeArray(15)
            r.Add (mp.WorldCoordinatesFirstMoments.X, mp.WorldCoordinatesFirstMoments.Y, mp.WorldCoordinatesFirstMoments.Z)                                     //  [0]     First Moments.
            r.Add (mp.WorldCoordinatesFirstMomentsError.X, mp.WorldCoordinatesFirstMomentsError.Y, mp.WorldCoordinatesFirstMomentsError.Z)                      //  [1]     The absolute (+/-) error bound for the First Moments.
            r.Add (mp.WorldCoordinatesSecondMoments.X, mp.WorldCoordinatesSecondMoments.Y, mp.WorldCoordinatesSecondMoments.Z)                                  //  [2]     Second Moments.
            r.Add (mp.WorldCoordinatesSecondMomentsError.X, mp.WorldCoordinatesSecondMomentsError.Y, mp.WorldCoordinatesSecondMomentsError.Z)                   //  [3]     The absolute (+/-) error bound for the Second Moments.
            r.Add (mp.WorldCoordinatesProductMoments.X, mp.WorldCoordinatesProductMoments.Y, mp.WorldCoordinatesProductMoments.Z)                               //  [4]     Product Moments.
            r.Add (mp.WorldCoordinatesProductMomentsError.X, mp.WorldCoordinatesProductMomentsError.Y, mp.WorldCoordinatesProductMomentsError.Z)                //  [5]     The absolute (+/-) error bound for the Product Moments.
            r.Add (mp.WorldCoordinatesMomentsOfInertia.X, mp.WorldCoordinatesMomentsOfInertia.Y, mp.WorldCoordinatesMomentsOfInertia.Z)                         //  [6]     Area Moments of Inertia about the World Coordinate Axes.
            r.Add (mp.WorldCoordinatesMomentsOfInertiaError.X, mp.WorldCoordinatesMomentsOfInertiaError.Y, mp.WorldCoordinatesMomentsOfInertiaError.Z)          //  [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
            r.Add (mp.WorldCoordinatesRadiiOfGyration.X, mp.WorldCoordinatesRadiiOfGyration.Y, mp.WorldCoordinatesRadiiOfGyration.Z)                            //  [8]     Area Radii of Gyration about the World Coordinate Axes.
            r.Add (0., 0., 0.) // need to add error calc to RhinoCommon                                                                                         //  [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
            r.Add (mp.CentroidCoordinatesMomentsOfInertia.X, mp.CentroidCoordinatesMomentsOfInertia.Y, mp.CentroidCoordinatesMomentsOfInertia.Z)                //  [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
            r.Add (mp.CentroidCoordinatesMomentsOfInertiaError.X, mp.CentroidCoordinatesMomentsOfInertiaError.Y, mp.CentroidCoordinatesMomentsOfInertiaError.Z) //  [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
            r.Add (mp.CentroidCoordinatesRadiiOfGyration.X, mp.CentroidCoordinatesRadiiOfGyration.Y, mp.CentroidCoordinatesRadiiOfGyration.Z)                   //  [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
            r.Add (0., 0., 0.) //need to add error calc to RhinoCommon                                                                                          //  [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes</returns>
            r


    /// <summary>Returns the point on a Surface that is closest to a test point.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface object</param>
    /// <param name="testPoint">(Point3d) Sampling point</param>
    /// <returns>(Point3d) The closest point on the Surface.</returns>
    static member SurfaceClosestPoint(surfaceId:Guid, testPoint:Point3d) : Point3d =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceClosestPoint(surfaceIdObject, testPoint)

    /// <summary>Returns the point on a Surface that is closest to a test point. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) Object of a Surface object</param>
    /// <param name="testPoint">(Point3d) Sampling point</param>
    /// <returns>(Point3d) The closest point on the Surface.</returns>
    static member SurfaceClosestPoint(surfaceGeo:Surface, testPoint:Point3d) : Point3d =
        let surface = surfaceGeo
        let rc, u, v = surface.ClosestPoint(testPoint)
        if not rc then RhinoScriptingException.Raise "SurfaceClosestPoint failed on %A and %A" surfaceGeo testPoint
        surface.PointAt(u, v)


    /// <summary>Returns U, V parameters of point on a Surface that is closest to a test point.</summary>
    /// <param name="surfaceId">(Guid) Identifier of a Surface object</param>
    /// <param name="testPoint">(Point3d) Sampling point</param>
    /// <returns>(float * float) The U, V parameters of the closest point on the Surface.</returns>
    static member SurfaceClosestParameter(surfaceId:Guid, testPoint:Point3d) : float * float =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceClosestParameter(surfaceIdObject, testPoint)

    /// <summary>Returns U, V parameters of point on a Surface that is closest to a test point. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) Object of a Surface object</param>
    /// <param name="testPoint">(Point3d) Sampling point</param>
    /// <returns>(float * float) The U, V parameters of the closest point on the Surface.</returns>
    static member SurfaceClosestParameter(surfaceGeo:Surface, testPoint:Point3d) : float * float =
        let surface = surfaceGeo
        let rc, u, v = surface.ClosestPoint(testPoint)
        if not rc then RhinoScriptingException.Raise "SurfaceClosestParameter failed on %A and %A" surfaceGeo testPoint
        u, v


    /// <summary>Returns the definition of a Surface cone.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>(Plane * float * float) containing the definition of the cone
    ///    [0]   the Plane of the cone. The apex of the cone is at the
    ///      Plane's origin and the axis of the cone is the Plane's z-axis
    ///    [1]   the height of the cone
    ///    [2]   the radius of the cone.</returns>
    static member SurfaceCone(surfaceId:Guid) : Plane * float * float =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceCone(surfaceIdObject)

    /// <summary>Returns the definition of a Surface cone. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <returns>(Plane * float * float) containing the definition of the cone
    ///    [0]   the Plane of the cone. The apex of the cone is at the
    ///      Plane's origin and the axis of the cone is the Plane's z-axis
    ///    [1]   the height of the cone
    ///    [2]   the radius of the cone.</returns>
    static member SurfaceCone(surfaceGeo:Surface) : Plane * float * float =
        let surface = surfaceGeo
        let rc, cone = surface.TryGetCone()
        if not rc then RhinoScriptingException.Raise "SurfaceCone failed.  surfaceGeo:'%s'" (Pretty.str surfaceGeo)
        cone.Plane, cone.Height, cone.Radius


    /// <summary>Returns the curvature of a Surface at a U, V parameter. See Rhino help
    ///    for details of Surface curvature.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="parameter">(float * float) U, v parameter</param>
    /// <returns>(Point3d * Vector3d * float * Vector3d * float * Vector3d * float * float) of curvature information
    ///    [0]   point at specified U, V parameter
    ///    [1]   normal direction
    ///    [2]   maximum principal curvature
    ///    [3]   maximum principal curvature direction
    ///    [4]   minimum principal curvature
    ///    [5]   minimum principal curvature direction
    ///    [6]   gaussian curvature
    ///    [7]   mean curvature.</returns>
    static member SurfaceCurvature(surfaceId:Guid, parameter:float * float) : Point3d * Vector3d * float * Vector3d * float * Vector3d * float * float=
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceCurvature(surfaceIdObject, parameter)

    /// <summary>Returns the curvature of a Surface at a U, V parameter. See Rhino help
    ///    for details of Surface curvature. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="parameter">(float * float) U, v parameter</param>
    /// <returns>(Point3d * Vector3d * float * Vector3d * float * Vector3d * float * float) of curvature information
    ///    [0]   point at specified U, V parameter
    ///    [1]   normal direction
    ///    [2]   maximum principal curvature
    ///    [3]   maximum principal curvature direction
    ///    [4]   minimum principal curvature
    ///    [5]   minimum principal curvature direction
    ///    [6]   gaussian curvature
    ///    [7]   mean curvature.</returns>
    static member SurfaceCurvature(surfaceGeo:Surface, parameter:float * float) : Point3d * Vector3d * float * Vector3d * float * Vector3d * float * float=
        let surface = surfaceGeo
        let c = surface.CurvatureAt(parameter|> fst, parameter|> snd)
        if c|> isNull  then RhinoScriptingException.Raise "SurfaceCurvature failed.  surfaceGeo:'%s' parameter:'%A'" (Pretty.str surfaceGeo) parameter
        c.Point, c.Normal, c.Kappa(0), c.Direction(0), c.Kappa(1), c.Direction(1), c.Gaussian, c.Mean


    /// <summary>Returns the definition of a cylinder Surface.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>(Plane * float * float) of the cylinder Plane, height and radius.</returns>
    static member SurfaceCylinder(surfaceId:Guid) : Plane * float * float =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceCylinder(surfaceIdObject)

    /// <summary>Returns the definition of a cylinder Surface. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <returns>(Plane * float * float) of the cylinder Plane, height and radius.</returns>
    static member SurfaceCylinder(surfaceGeo:Surface) : Plane * float * float =
        let surface = surfaceGeo
        let tol = State.Doc.ModelAbsoluteTolerance
        let cy = ref Cylinder.Unset
        let rc = surface.TryGetFiniteCylinder(cy, tol)
        if rc  then
            let cylinder = !cy
            cylinder.BasePlane, cylinder.TotalHeight, cylinder.Radius
        else
            RhinoScriptingException.Raise "SurfaceCylinder failed on %A" surfaceGeo


    /// <summary>Returns the U and V degrees of a Surface.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>(int*int) The degree in U and V direction.</returns>
    static member SurfaceDegree(surfaceId:Guid ) : int*int =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceDegree(surfaceIdObject)

    /// <summary>Returns the U and V degrees of a Surface. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <returns>(int*int) The degree in U and V direction.</returns>
    static member SurfaceDegree(surfaceGeo:Surface ) : int*int =
        let surface = surfaceGeo
        surface.Degree(0), surface.Degree(1)


    /// <summary>Returns the domain of a Surface object in the specified direction.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="direction">(int) Domain direction 0 = U, or 1 = V</param>
    /// <returns>(float * float) containing the domain interval in the specified direction.</returns>
    static member SurfaceDomain(surfaceId:Guid, direction:int) : float * float =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceDomain(surfaceIdObject, direction)

    /// <summary>Returns the domain of a Surface object in the specified direction. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="direction">(int) Domain direction 0 = U, or 1 = V</param>
    /// <returns>(float * float) containing the domain interval in the specified direction.</returns>
    static member SurfaceDomain(surfaceGeo:Surface, direction:int) : float * float =
        if direction <> 0 && direction <> 1 then RhinoScriptingException.Raise "SurfaceDomain failed.  surfaceGeo:'%s' direction:'%A'" (Pretty.str surfaceGeo) direction
        let surface = surfaceGeo
        let domain = surface.Domain(direction)
        domain.T0, domain.T1


    /// <summary>Returns the edit, or Greville points of a Surface object. For each
    ///    Surface control point, there is a corresponding edit point.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="returnAll">(bool) Optional, default value: <c>true</c>
    ///    If True, all Surface edit points are returned. If False,
    ///    the function will return Surface edit points based on whether or not the
    ///    Surface is closed or periodic</param>
    /// <returns>(Point3d ResizeArray) a list of 3D points.</returns>
    static member SurfaceEditPoints( surfaceId:Guid, [<OPT;DEF(true)>]returnAll:bool) : Point3d ResizeArray =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceEditPoints(surfaceIdObject, returnAll)

    /// <summary>Returns the edit, or Greville points of a Surface object. For each
    ///    Surface control point, there is a corresponding edit point. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="returnAll">(bool) Optional, default value: <c>true</c>
    ///    If True, all Surface edit points are returned. If False,
    ///    the function will return Surface edit points based on whether or not the
    ///    Surface is closed or periodic</param>
    /// <returns>(Point3d ResizeArray) a list of 3D points.</returns>
    static member SurfaceEditPoints( surfaceGeo:Surface, [<OPT;DEF(true)>]returnAll:bool) : Point3d ResizeArray =
        let surface = surfaceGeo
        let nurb = surface.ToNurbsSurface()
        if isNull nurb then RhinoScriptingException.Raise "SurfaceEditPoints failed.  surfaceGeo:'%s'  returnAll:'%A'" (Pretty.str surfaceGeo) returnAll
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
        let r = ResizeArray((ulast-1)*(vlast-1))
        for u = ufirst to ulast-1 do
            for v = vfirst to  vlast-1 do
                let pt = nurb.Points.GetGrevillePoint(u, v)
                r.Add <| nurb.PointAt(pt.X, pt.Y)
        r


    /// <summary>Returns the parameters at edit, or Greville points of a Surface object. For each
    ///    Surface control point, there is a corresponding edit point.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="returnAll">(bool) Optional, default value: <c>true</c>
    ///    If True, all Surface edit points are returned. If False,
    ///    the function will return Surface edit points based on whether or not the
    ///    Surface is closed or periodic</param>
    /// <returns>((float*float) ResizeArray) a list of U and V parameters.</returns>
    static member SurfaceEditPointParameters( surfaceId:Guid, [<OPT;DEF(true)>]returnAll:bool) : (float*float) ResizeArray =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceEditPointParameters(surfaceIdObject, returnAll)

    /// <summary>Returns the parameters at edit, or Greville points of a Surface object. For each
    ///    Surface control point, there is a corresponding edit point. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="returnAll">(bool) Optional, default value: <c>true</c>
    ///    If True, all Surface edit points are returned. If False,
    ///    the function will return Surface edit points based on whether or not the
    ///    Surface is closed or periodic</param>
    /// <returns>((float*float) ResizeArray) a list of U and V parameters.</returns>
    static member SurfaceEditPointParameters( surfaceGeo:Surface, [<OPT;DEF(true)>]returnAll:bool) : (float*float) ResizeArray =
        let surface = surfaceGeo
        let nurb = surface.ToNurbsSurface()
        if isNull nurb then RhinoScriptingException.Raise "SurfaceEditPointParameters failed.  surfaceGeo:'%s'  returnAll:'%A'" (Pretty.str surfaceGeo) returnAll
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
        let r = ResizeArray((ulast-1)*(vlast-1))
        for u = ufirst to ulast-1 do
            for v = vfirst to  vlast-1 do
                let pt = nurb.Points.GetGrevillePoint(u, v)
                r.Add(pt.X, pt.Y)
        r


    /// <summary>A general purpose Surface evaluator.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="parameter">(float * float) U, v parameter to evaluate</param>
    /// <param name="derivative">(int) Number of derivatives to evaluate</param>
    /// <returns>(Point3d * Vector3d ResizeArray) list length (derivative + 1)*(derivative + 2)/2
    ///    Elements of tuple:
    ///    [fst] The 3-D point.
    ///    [snd] List of Vectors:
    ///            [0]      The first derivative.
    ///            [1]      The first derivative.
    ///            [2]      The second derivative.
    ///            [3]      The second derivative.
    ///            [4]      The second derivative.
    ///            [5]      etc...</returns>
    static member SurfaceEvaluate( surfaceId:Guid,
                                   parameter:float * float,
                                   derivative:int) : Point3d * Vector3d ResizeArray =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceEvaluate(surfaceIdObject, parameter, derivative)

    /// <summary>A general purpose Surface evaluator. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="parameter">(float * float) U, v parameter to evaluate</param>
    /// <param name="derivative">(int) Number of derivatives to evaluate</param>
    /// <returns>(Point3d * Vector3d ResizeArray) list length (derivative + 1)*(derivative + 2)/2
    ///    Elements of tuple:
    ///    [fst] The 3-D point.
    ///    [snd] List of Vectors:
    ///            [0]      The first derivative.
    ///            [1]      The first derivative.
    ///            [2]      The second derivative.
    ///            [3]      The second derivative.
    ///            [4]      The second derivative.
    ///            [5]      etc...</returns>
    static member SurfaceEvaluate( surfaceGeo:Surface,
                                   parameter:float * float,
                                   derivative:int) : Point3d * Vector3d ResizeArray =
        let surface = surfaceGeo
        let success, point, der = surface.Evaluate(parameter|> fst, parameter|> snd, derivative)
        if not success then RhinoScriptingException.Raise "SurfaceEvaluate failed.  surfaceGeo:'%s' parameter:'%A' derivative:'%A'" (Pretty.str surfaceGeo) parameter derivative
        let rc = ResizeArray(der.Length)
        if der.Length > 0 then
          for d in der do rc.Add(d)
        point, rc


    /// <summary>Returns a Plane based on the normal, u, and v directions at a Surface
    ///    U, V parameter.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="uvParameter">(float * float) U, v parameter to evaluate</param>
    /// <returns>(Plane) Plane.</returns>
    static member SurfaceFrame(surfaceId:Guid, uvParameter:float * float) : Plane =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceFrame(surfaceIdObject, uvParameter)

    /// <summary>Returns a Plane based on the normal, u, and v directions at a Surface
    ///    U, V parameter. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="uvParameter">(float * float) U, v parameter to evaluate</param>
    /// <returns>(Plane) Plane.</returns>
    static member SurfaceFrame(surfaceGeo:Surface, uvParameter:float * float) : Plane =
        let surface = surfaceGeo
        let rc, frame = surface.FrameAt(uvParameter|> fst, uvParameter|> snd)
        if rc  then frame
        else RhinoScriptingException.Raise "SurfaceFrame failed on %A at %A" surfaceGeo uvParameter


    /// <summary>Returns the isocurve density of a Surface or Polysurface object.
    /// An isoparametric Curve is a Curve of constant U or V value on a Surface.
    /// Rhino uses isocurves and Surface edge Curves to visualize the shape of a
    /// NURBS Surface.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>(int) The current isocurve density
    ///      -1: Hides the Surface isocurves
    ///      0: Display boundary and knot wires
    ///      1: Display boundary and knot wires and one interior wire if there are no interior knots
    ///      bigger than 1: Display boundary and knot wires and (N + 1) interior wires.</returns>
    static member SurfaceIsocurveDensity(surfaceId:Guid) : int = //GET
        match RhinoScriptSyntax.CoerceRhinoObject(surfaceId) with
        | :?  DocObjects.BrepObject as rhinoObject ->
                rhinoObject.Attributes.WireDensity
        | :?  DocObjects.SurfaceObject as rhinoObject ->
                rhinoObject.Attributes.WireDensity
        | :?  DocObjects.ExtrusionObject as rhinoObject ->
                rhinoObject.Attributes.WireDensity
        | _ -> RhinoScriptingException.Raise "SurfaceIsocurveDensity Get failed.  surfaceId:'%s'" (Pretty.str surfaceId)


    /// <summary>Sets the isocurve density of a Surface or Polysurface object.
    /// An isoparametric Curve is a Curve of constant U or V value on a Surface.
    /// Rhino uses isocurves and Surface edge Curves to visualize the shape of a
    /// NURBS Surface.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="density">(int) The isocurve wireframe density. The possible values are
    ///      -1: Hides the Surface isocurves
    ///      0: Display boundary and knot wires
    ///      1: Display boundary and knot wires and one interior wire if there are no interior knots
    ///      bigger than 1: Display boundary and knot wires and (N + 1) interior wires</param>
    /// <returns>(unit) void, nothing.</returns>
    static member SurfaceIsocurveDensity(surfaceId:Guid, density:int) : unit = //SET
        match RhinoScriptSyntax.CoerceRhinoObject(surfaceId) with
        | :?  DocObjects.BrepObject as rhinoObject ->
                let dens = if density<0 then -1 else density
                rhinoObject.Attributes.WireDensity <- dens
                rhinoObject.CommitChanges() |> ignore<bool>
                State.Doc.Views.Redraw()
        | :?  DocObjects.SurfaceObject as rhinoObject ->
                let dens = if density<0 then -1 else density
                rhinoObject.Attributes.WireDensity <- dens
                rhinoObject.CommitChanges() |> ignore<bool>
                State.Doc.Views.Redraw()
        | :?  DocObjects.ExtrusionObject as rhinoObject ->
                let dens = if density<0 then -1 else density
                rhinoObject.Attributes.WireDensity <- dens
                rhinoObject.CommitChanges() |> ignore<bool>
                State.Doc.Views.Redraw()
        | _ -> RhinoScriptingException.Raise "SurfaceIsocurveDensity Get failed.  surfaceId:'%s' density:'%A'" (Pretty.str surfaceId) density


    /// <summary>Sets the isocurve density of multiple Surface or Polysurface objects.
    /// An isoparametric Curve is a Curve of constant U or V value on a Surface.
    /// Rhino uses isocurves and Surface edge Curves to visualize the shape of a
    /// NURBS Surface.</summary>
    /// <param name="surfaceIds">(Guid seq) The Surface's identifiers</param>
    /// <param name="density">(int) The isocurve wireframe density. The possible values are
    ///      -1: Hides the Surface isocurves
    ///      0: Display boundary and knot wires
    ///      1: Display boundary and knot wires and one interior wire if there are no interior knots
    ///      bigger than 1: Display boundary and knot wires and (N + 1) interior wires</param>
    /// <returns>(unit) void, nothing.</returns>
    static member SurfaceIsocurveDensity(surfaceIds:Guid seq, density:int) : unit = //MULTISET
        for surfaceId in surfaceIds do
            match RhinoScriptSyntax.CoerceRhinoObject(surfaceId) with
            | :?  DocObjects.BrepObject as rhinoObject ->
                    let dens = if density<0 then -1 else density
                    rhinoObject.Attributes.WireDensity <- dens
                    rhinoObject.CommitChanges() |> ignore<bool>
            | :?  DocObjects.SurfaceObject as rhinoObject ->
                    let dens = if density<0 then -1 else density
                    rhinoObject.Attributes.WireDensity <- dens
                    rhinoObject.CommitChanges() |> ignore<bool>
            | :?  DocObjects.ExtrusionObject as rhinoObject ->
                    let dens = if density<0 then -1 else density
                    rhinoObject.Attributes.WireDensity <- dens
                    rhinoObject.CommitChanges() |> ignore<bool>
            | _ -> RhinoScriptingException.Raise "SurfaceIsocurveDensity Get failed.  surfaceId:'%s' density:'%A'" (Pretty.str surfaceId) density
        State.Doc.Views.Redraw()

    /// <summary>Returns the control point count of a Surface
    ///    SurfaceId = the Surface's identifier.</summary>
    /// <param name="surfaceId">(Guid) The Surface object's identifier</param>
    /// <returns>(int * int) a list containing (U count, V count).</returns>
    static member SurfaceKnotCount(surfaceId:Guid) : int * int =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceKnotCount(surfaceIdObject)

    /// <summary>Returns the control point count of a Surface
    ///    SurfaceId = the Surface's identifier. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface object</param>
    /// <returns>(int * int) a list containing (U count, V count).</returns>
    static member SurfaceKnotCount(surfaceGeo:Surface) : int * int =
        let surface = surfaceGeo
        let ns = surface.ToNurbsSurface()
        ns.KnotsU.Count, ns.KnotsV.Count


    /// <summary>Returns the knots, or knot vector, of a Surface object.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>(NurbsSurfaceKnotList * NurbsSurfaceKnotList) knot values of the Surface.
    ///    The list will contain the following information:
    ///    Element   Description
    ///      [0]     Knot vectors in U direction
    ///      [1]     Knot vectors in V direction.</returns>
    static member SurfaceKnots(surfaceId:Guid) : Collections.NurbsSurfaceKnotList * Collections.NurbsSurfaceKnotList=
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceKnots(surfaceIdObject)

    /// <summary>Returns the knots, or knot vector, of a Surface object. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <returns>(NurbsSurfaceKnotList * NurbsSurfaceKnotList) knot values of the Surface.
    ///    The list will contain the following information:
    ///    Element   Description
    ///      [0]     Knot vectors in U direction
    ///      [1]     Knot vectors in V direction.</returns>
    static member SurfaceKnots(surfaceGeo:Surface) : Collections.NurbsSurfaceKnotList * Collections.NurbsSurfaceKnotList=
        let surface = surfaceGeo
        let nurbsurf = surface.ToNurbsSurface()
        if nurbsurf|> isNull  then RhinoScriptingException.Raise "SurfaceKnots failed.  surfaceGeo:'%s'" (Pretty.str surfaceGeo)
        nurbsurf.KnotsU , nurbsurf.KnotsV


    /// <summary>Returns 3D vector that is the normal to a Surface at a parameter.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="uvParameter">(float * float) The uv parameter to evaluate</param>
    /// <returns>(Vector3d) Normal vector.</returns>
    static member SurfaceNormal(surfaceId:Guid, uvParameter:float * float) : Vector3d =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceNormal(surfaceIdObject, uvParameter)


    /// <summary>Returns 3D vector that is the normal to a Surface at mid parameter.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>(Vector3d) Normal vector.</returns>
    static member SurfaceNormal(surfaceId:Guid) : Vector3d =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceNormal(surfaceIdObject)

    /// <summary>Returns 3D vector that is the normal to a Surface at a parameter. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="uvParameter">(float * float) The uv parameter to evaluate</param>
    /// <returns>(Vector3d) Normal vector.</returns>
    static member SurfaceNormal(surfaceGeo:Surface, uvParameter:float * float) : Vector3d =
        let surface = surfaceGeo
        surface.NormalAt(uvParameter|> fst, uvParameter|> snd)

    /// <summary>Returns 3D vector that is the normal to a Surface at mid parameter. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <returns>(Vector3d) Normal vector.</returns>
    static member SurfaceNormal(surfaceGeo:Surface) : Vector3d =
        let surface = surfaceGeo
        let u = surface.Domain(0).ParameterAt(0.5)
        let v = surface.Domain(1).ParameterAt(0.5)
        surface.NormalAt(u,v)


    /// <summary>Converts Surface parameter to a normalized Surface parameter; one that
    ///    ranges between 0.0 and 1.0 in both the U and V directions.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="parameter">(float * float) The Surface parameter to convert</param>
    /// <returns>(float * float) normalized Surface parameter.</returns>
    static member SurfaceNormalizedParameter(surfaceId:Guid, parameter:float * float) : float * float =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceNormalizedParameter(surfaceIdObject, parameter)

    /// <summary>Converts Surface parameter to a normalized Surface parameter; one that
    ///    ranges between 0.0 and 1.0 in both the U and V directions. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="parameter">(float * float) The Surface parameter to convert</param>
    /// <returns>(float * float) normalized Surface parameter.</returns>
    static member SurfaceNormalizedParameter(surfaceGeo:Surface, parameter:float * float) : float * float =
        let surface = surfaceGeo
        let udomain = surface.Domain(0)
        let vdomain = surface.Domain(1)
        if parameter|> fst<udomain.Min || parameter|> fst>udomain.Max then
            RhinoScriptingException.Raise "SurfaceNormalizedParameter failed.  surfaceGeo:'%s' parameter:'%A'" (Pretty.str surfaceGeo) parameter
        if parameter|> snd<vdomain.Min || parameter|> snd>vdomain.Max then
            RhinoScriptingException.Raise "SurfaceNormalizedParameter failed.  surfaceGeo:'%s' parameter:'%A'" (Pretty.str surfaceGeo) parameter
        let u = udomain.NormalizedParameterAt(parameter|> fst)
        let v = vdomain.NormalizedParameterAt(parameter|> snd)
        u, v


    /// <summary>Converts normalized Surface parameter to a Surface domain specific (regular) parameter.
    /// or a parameter within the Surface's domain.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="parameter">(float * float) The normalized parameter to convert</param>
    /// <returns>(float * float) u and v Surface parameters.</returns>
    static member SurfaceParameter(surfaceId:Guid, parameter:float * float) : float * float =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceParameter(surfaceIdObject, parameter)

    /// <summary>Converts normalized Surface parameter to a Surface domain specific (regular) parameter.
    /// or a parameter within the Surface's domain. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="parameter">(float * float) The normalized parameter to convert</param>
    /// <returns>(float * float) u and v Surface parameters.</returns>
    static member SurfaceParameter(surfaceGeo:Surface, parameter:float * float) : float * float =
        let surface = surfaceGeo
        let x = surface.Domain(0).ParameterAt(parameter|> fst)
        let y = surface.Domain(1).ParameterAt(parameter|> snd)
        x, y


    /// <summary>Returns the control point count of a Surface
    ///    SurfaceId = the Surface's identifier.</summary>
    /// <param name="surfaceId">(Guid) The Surface object's identifier</param>
    /// <returns>(int * int) THe number of control points in UV direction. (U count, V count).</returns>
    static member SurfacePointCount(surfaceId:Guid) : int * int =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfacePointCount(surfaceIdObject)

    /// <summary>Returns the control point count of a Surface
    ///    SurfaceId = the Surface's identifier. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface object</param>
    /// <returns>(int * int) THe number of control points in UV direction. (U count, V count).</returns>
    static member SurfacePointCount(surfaceGeo:Surface) : int * int =
        let surface = surfaceGeo
        let ns = surface.ToNurbsSurface()
        ns.Points.CountU, ns.Points.CountV


    /// <summary>Returns the control points, or control vertices, of a Surface object.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="returnAll">(bool) Optional, default value: <c>true</c>
    ///    If True all Surface edit points are returned. If False,
    ///    the function will return Surface edit points based on whether or not
    ///    the Surface is closed or periodic</param>
    /// <returns>(Point3d ResizeArray) The control points.</returns>
    static member SurfacePoints(surfaceId:Guid, [<OPT;DEF(true)>]returnAll:bool) : Point3d ResizeArray =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfacePoints(surfaceIdObject, returnAll)

    /// <summary>Returns the control points, or control vertices, of a Surface object. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <param name="returnAll">(bool) Optional, default value: <c>true</c>
    ///    If True all Surface edit points are returned. If False,
    ///    the function will return Surface edit points based on whether or not
    ///    the Surface is closed or periodic</param>
    /// <returns>(Point3d ResizeArray) The control points.</returns>
    static member SurfacePoints(surfaceGeo:Surface, [<OPT;DEF(true)>]returnAll:bool) : Point3d ResizeArray =
        let surface = surfaceGeo
        let ns = surface.ToNurbsSurface()
        if ns|> isNull  then RhinoScriptingException.Raise "SurfacePoints failed.  surfaceGeo:'%s' returnAll:'%A'" (Pretty.str surfaceGeo) returnAll
        let rc = ResizeArray()
        for u = 0 to ns.Points.CountU - 1 do
            for v = 0 to ns.Points.CountV - 1 do
                let pt = ns.Points.GetControlPoint(u, v)
                rc.Add(pt.Location)
        rc


    /// <summary>Returns the definition of a Surface torus.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <returns>(Plane * float * float) containing the definition of the torus
    ///    [0]   the base Plane of the torus
    ///    [1]   the major radius of the torus
    ///    [2]   the minor radius of the torus.</returns>
    static member SurfaceTorus(surfaceId:Guid) : Plane * float * float =
        let surfaceIdObject = RhinoScriptSyntax.CoerceSurface(surfaceId)
        RhinoScriptSyntax.SurfaceTorus(surfaceIdObject)

    /// <summary>Returns the definition of a Surface torus. Uses the supplied objects directly.</summary>
    /// <param name="surfaceGeo">(Surface) The Surface</param>
    /// <returns>(Plane * float * float) containing the definition of the torus
    ///    [0]   the base Plane of the torus
    ///    [1]   the major radius of the torus
    ///    [2]   the minor radius of the torus.</returns>
    static member SurfaceTorus(surfaceGeo:Surface) : Plane * float * float =
        let surface = surfaceGeo
        let rc, torus = surface.TryGetTorus()
        if rc then torus.Plane, torus.MajorRadius, torus.MinorRadius
        else RhinoScriptingException.Raise "SurfaceTorus failed for %A" surfaceGeo


    /// <summary>Calculates volume of a closed Surface or Polysurface.</summary>
    /// <param name="objectId">(Guid) The Surface's identifier</param>
    /// <returns>(float) The volume.</returns>
    static member SurfaceVolume(objectId:Guid) : float =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | Some geometry -> RhinoScriptSyntax.SurfaceVolume(geometry)
        | None -> RhinoScriptingException.Raise "SurfaceVolume failed on %A" (Pretty.str objectId)

    /// <summary>Calculates volume of a closed Surface or Polysurface. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The Surface</param>
    /// <returns>(float) The volume.</returns>
    static member SurfaceVolume(geometryObject:GeometryBase) : float =
        let tryGetBrep (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Brep as brep -> Some brep | :? Extrusion as extrusion -> Some (extrusion.ToBrep(true)) | _ -> None
        let tryGetSurface (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Surface as surface -> Some surface | :? Brep as brep when brep.Faces.Count = 1 -> Some (brep.Faces.[0] :> Surface) | _ -> None
        geometryObject
        |> tryGetBrep
        |> Option.map VolumeMassProperties.Compute
        |> Option.orElseWith (fun () ->
            geometryObject
            |> tryGetSurface
            |> Option.map VolumeMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "SurfaceVolume failed on %A" (Pretty.str geometryObject))
        |> fun amp -> amp.Volume


    /// <summary>Calculates volume centroid of a closed Surface or Polysurface.</summary>
    /// <param name="objectId">(Guid) The Surface's identifier</param>
    /// <returns>(Point3d) Volume Centroid.</returns>
    static member SurfaceVolumeCentroid(objectId:Guid) : Point3d =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | Some geometry -> RhinoScriptSyntax.SurfaceVolumeCentroid(geometry)
        | None -> RhinoScriptingException.Raise "SurfaceVolumeCentroid failed on %A" (Pretty.str objectId)

    /// <summary>Calculates volume centroid of a closed Surface or Polysurface. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The Surface</param>
    /// <returns>(Point3d) Volume Centroid.</returns>
    static member SurfaceVolumeCentroid(geometryObject:GeometryBase) : Point3d =
        let tryGetBrep (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Brep as brep -> Some brep | :? Extrusion as extrusion -> Some (extrusion.ToBrep(true)) | _ -> None
        let tryGetSurface (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Surface as surface -> Some surface | :? Brep as brep when brep.Faces.Count = 1 -> Some (brep.Faces.[0] :> Surface) | _ -> None
        geometryObject
        |> tryGetBrep
        |> Option.bind (fun b -> if b.IsSolid then Some b else RhinoScriptingException.Raise "SurfaceVolumeCentroid failed on  open Brep %A" (Pretty.str geometryObject))
        |> Option.map VolumeMassProperties.Compute
        |> Option.orElseWith (fun () ->
            geometryObject
            |> tryGetSurface
            |> Option.bind (fun s -> if s.IsSolid then Some s else RhinoScriptingException.Raise "SurfaceVolumeCentroid failed on  open Surface %A" (Pretty.str geometryObject))
            |> Option.map VolumeMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "SurfaceVolumeCentroid failed on %A" (Pretty.str geometryObject))
        |> fun amp -> amp.Centroid


    /// <summary>Calculates volume moments of inertia of a Surface or Polysurface object.
    ///    For more information, see Rhino help for "Mass Properties calculation details".</summary>
    /// <param name="objectId">(Guid) The Surface's identifier</param>
    /// <returns>((float*float*float) ResizeArray) A List of moments and error bounds in tuple(X, Y, Z) - see help topic
    ///    Index   Description
    ///    [ 0]    First Moments.
    ///    [ 1]    The absolute (+/-) error bound for the First Moments.
    ///    [ 2]    Second Moments.
    ///    [ 3]    The absolute (+/-) error bound for the Second Moments.
    ///    [ 4]    Product Moments.
    ///    [ 5]    The absolute (+/-) error bound for the Product Moments.
    ///    [ 6]    Area Moments of Inertia about the World Coordinate Axes.
    ///    [ 7]    The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
    ///    [ 8]    Area Radii of Gyration about the World Coordinate Axes.
    ///    [ 9]    (0,0,0) NOT WORKING , but should be: The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///    [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///    [13]    (0,0,0) NOT WORKING , but should be: The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.
    /// </returns>
    static member SurfaceVolumeMoments(objectId:Guid) : (float*float*float) ResizeArray =
        match RhinoScriptSyntax.TryCoerceGeometry(objectId) with
        | Some geometry -> RhinoScriptSyntax.SurfaceVolumeMoments(geometry)
        | None -> RhinoScriptingException.Raise "SurfaceVolumeMoments failed on %A" (Pretty.str objectId)

    /// <summary>Calculates volume moments of inertia of a Surface or Polysurface object.
    ///    For more information, see Rhino help for "Mass Properties calculation details". Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(GeometryBase) The Surface</param>
    /// <returns>((float*float*float) ResizeArray) A List of moments and error bounds in tuple(X, Y, Z) - see help topic
    ///    Index   Description
    ///    [ 0]    First Moments.
    ///    [ 1]    The absolute (+/-) error bound for the First Moments.
    ///    [ 2]    Second Moments.
    ///    [ 3]    The absolute (+/-) error bound for the Second Moments.
    ///    [ 4]    Product Moments.
    ///    [ 5]    The absolute (+/-) error bound for the Product Moments.
    ///    [ 6]    Area Moments of Inertia about the World Coordinate Axes.
    ///    [ 7]    The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
    ///    [ 8]    Area Radii of Gyration about the World Coordinate Axes.
    ///    [ 9]    (0,0,0) NOT WORKING , but should be: The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///    [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///    [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///    [13]    (0,0,0) NOT WORKING , but should be: The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.
    /// </returns>
    static member SurfaceVolumeMoments(geometryObject:GeometryBase) : (float*float*float) ResizeArray =
        let tryGetBrep (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Brep as brep -> Some brep | :? Extrusion as extrusion -> Some (extrusion.ToBrep(true)) | _ -> None
        let tryGetSurface (value:GeometryBase) =
            let geometry = value
            match geometry with | :? Surface as surface -> Some surface | :? Brep as brep when brep.Faces.Count = 1 -> Some (brep.Faces.[0] :> Surface) | _ -> None
        geometryObject
        |> tryGetBrep
        |> Option.bind (fun b -> if b.IsSolid then Some b else RhinoScriptingException.Raise "SurfaceVolumeMoments failed on  open Brep %A" (Pretty.str geometryObject))
        |> Option.map VolumeMassProperties.Compute
        |> Option.orElseWith (fun () ->
            geometryObject
            |> tryGetSurface
            |> Option.bind (fun s -> if s.IsSolid then Some s else RhinoScriptingException.Raise "SurfaceVolumeMoments failed on  open Surface %A" (Pretty.str geometryObject))
            |> Option.map VolumeMassProperties.Compute
            )
        |> Option.defaultWith (fun () -> RhinoScriptingException.Raise "SurfaceVolumeMoments failed on %A" (Pretty.str geometryObject))
        |> fun mp ->
            let r = ResizeArray(15)
            r.Add (mp.WorldCoordinatesFirstMoments.X, mp.WorldCoordinatesFirstMoments.Y, mp.WorldCoordinatesFirstMoments.Z)                                     //  [0]     First Moments.
            r.Add (mp.WorldCoordinatesFirstMomentsError.X, mp.WorldCoordinatesFirstMomentsError.Y, mp.WorldCoordinatesFirstMomentsError.Z)                      //  [1]     The absolute (+/-) error bound for the First Moments.
            r.Add (mp.WorldCoordinatesSecondMoments.X, mp.WorldCoordinatesSecondMoments.Y, mp.WorldCoordinatesSecondMoments.Z)                                  //  [2]     Second Moments.
            r.Add (mp.WorldCoordinatesSecondMomentsError.X, mp.WorldCoordinatesSecondMomentsError.Y, mp.WorldCoordinatesSecondMomentsError.Z)                   //  [3]     The absolute (+/-) error bound for the Second Moments.
            r.Add (mp.WorldCoordinatesProductMoments.X, mp.WorldCoordinatesProductMoments.Y, mp.WorldCoordinatesProductMoments.Z)                               //  [4]     Product Moments.
            r.Add (mp.WorldCoordinatesProductMomentsError.X, mp.WorldCoordinatesProductMomentsError.Y, mp.WorldCoordinatesProductMomentsError.Z)                //  [5]     The absolute (+/-) error bound for the Product Moments.
            r.Add (mp.WorldCoordinatesMomentsOfInertia.X, mp.WorldCoordinatesMomentsOfInertia.Y, mp.WorldCoordinatesMomentsOfInertia.Z)                         //  [6]     Area Moments of Inertia about the World Coordinate Axes.
            r.Add (mp.WorldCoordinatesMomentsOfInertiaError.X, mp.WorldCoordinatesMomentsOfInertiaError.Y, mp.WorldCoordinatesMomentsOfInertiaError.Z)          //  [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
            r.Add (mp.WorldCoordinatesRadiiOfGyration.X, mp.WorldCoordinatesRadiiOfGyration.Y, mp.WorldCoordinatesRadiiOfGyration.Z)                            //  [8]     Area Radii of Gyration about the World Coordinate Axes.
            r.Add (0., 0., 0.) // need to add error calc to RhinoCommon                                                                                         //  [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
            r.Add (mp.CentroidCoordinatesMomentsOfInertia.X, mp.CentroidCoordinatesMomentsOfInertia.Y, mp.CentroidCoordinatesMomentsOfInertia.Z)                //  [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
            r.Add (mp.CentroidCoordinatesMomentsOfInertiaError.X, mp.CentroidCoordinatesMomentsOfInertiaError.Y, mp.CentroidCoordinatesMomentsOfInertiaError.Z) //  [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
            r.Add (mp.CentroidCoordinatesRadiiOfGyration.X, mp.CentroidCoordinatesRadiiOfGyration.Y, mp.CentroidCoordinatesRadiiOfGyration.Z)                   //  [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
            r.Add (0., 0., 0.) //need to add error calc to RhinoCommon                                                                                          //  [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.
            r


    /// <summary>Returns list of weight values assigned to the control points of a Surface.
    ///    The number of weights returned will be equal to the number of control points
    ///    in the U and V directions.</summary>
    /// <param name="objectId">(Guid) The Surface's identifier</param>
    /// <returns>(float ResizeArray) point weights.</returns>
    static member SurfaceWeights(objectId:Guid) : float ResizeArray =
        let objectIdObject = RhinoScriptSyntax.CoerceSurface(objectId)
        RhinoScriptSyntax.SurfaceWeights(objectIdObject)

    /// <summary>Returns list of weight values assigned to the control points of a Surface.
    ///    The number of weights returned will be equal to the number of control points
    ///    in the U and V directions. Uses the supplied objects directly.</summary>
    /// <param name="geometryObject">(Surface) The Surface</param>
    /// <returns>(float ResizeArray) point weights.</returns>
    static member SurfaceWeights(geometryObject:Surface) : float ResizeArray =
        let surface = geometryObject
        let ns = surface.ToNurbsSurface()
        if ns|> isNull  then RhinoScriptingException.Raise "SurfaceWeights failed.  geometryObject:'%s'" (Pretty.str geometryObject)
        let rc = ResizeArray()
        for u = 0 to ns.Points.CountU - 1 do
            for v = 0 to ns.Points.CountV - 1 do
                let pt = ns.Points.GetControlPoint(u, v)
                rc.Add(pt.Weight)
        rc


    /// <summary>Trims a Surface or Polysurface using an oriented cutter brep or Surface.</summary>
    /// <param name="objectId">(Guid) Surface or Polysurface identifier</param>
    /// <param name="cutter">(Guid) Surface or Polysurface  performing the trim</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    /// <returns>(Guid ResizeArray) identifiers of retained components.</returns>
    static member TrimBrep( objectId:Guid,
                            cutter:Guid,
                            [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let objectIdGeometry = RhinoScriptSyntax.CoerceBrep(objectId)
        let cutterGeometry = RhinoScriptSyntax.CoerceBrep(cutter)
        let breps = RhinoScriptSyntax.TrimBrep(objectIdGeometry, cutterGeometry, tolerance)
        let ids = ResizeArray<Guid>()
        if breps.Count > 1 then
            let attributes = (RhinoScriptSyntax.CoerceRhinoObject(objectId)).Attributes
            State.Doc.Objects.Replace(objectId, breps.[0]) |> ignore<bool>
            ids.Add(objectId)
            for index = 1 to breps.Count - 1 do ids.Add(State.Doc.Objects.AddBrep(breps.[index], attributes))
        else
            for brep in breps do ids.Add(State.Doc.Objects.AddBrep(brep))
        State.Doc.Views.Redraw()
        ids


    /// <summary>Trims a Surface using an oriented cutter Plane.</summary>
    /// <param name="objectId">(Guid) Surface or Polysurface identifier</param>
    /// <param name="cutter">(Plane) Plane performing the trim</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    /// <returns>(Guid ResizeArray) identifiers of retained components.</returns>
    static member TrimBrep( objectId:Guid,
                            cutter:Plane,
                            [<OPT;DEF(0.0)>]tolerance:float) : Guid ResizeArray =
        let objectIdGeometry = RhinoScriptSyntax.CoerceBrep(objectId)
        let breps = RhinoScriptSyntax.TrimBrep(objectIdGeometry, cutter, tolerance)
        let ids = ResizeArray<Guid>()
        if breps.Count > 1 then
            let attributes = (RhinoScriptSyntax.CoerceRhinoObject(objectId)).Attributes
            State.Doc.Objects.Replace(objectId, breps.[0]) |> ignore<bool>
            ids.Add(objectId)
            for index = 1 to breps.Count - 1 do ids.Add(State.Doc.Objects.AddBrep(breps.[index], attributes))
        else
            for brep in breps do ids.Add(State.Doc.Objects.AddBrep(brep))
        State.Doc.Views.Redraw()
        ids


    /// <summary>Trims a Surface or Polysurface using an oriented cutter brep or Surface. Returns geometry without adding it.</summary>
    /// <param name="geometryObject">(Brep) Surface or Polysurface object</param>
    /// <param name="cutter">(Brep) Surface or Polysurface  performing the trim</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member TrimBrep( geometryObject:Brep,
                            cutter:Brep,
                            [<OPT;DEF(0.0)>]tolerance:float) : Brep ResizeArray =
        let tolerance = Util.ifZero1 tolerance State.Doc.ModelAbsoluteTolerance
        let pieces = geometryObject.Trim(cutter, tolerance)
        if isNull pieces then RhinoScriptingException.Raise "TrimBrep: operation failed."
        ResizeArray(pieces)


    /// <summary>Trims a Surface using an oriented cutter Plane. Returns geometry without adding it.</summary>
    /// <param name="geometryObject">(Brep) Surface or Polysurface object</param>
    /// <param name="cutter">(Plane) Plane performing the trim</param>
    /// <param name="tolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c></param>
    /// <returns>(Brep ResizeArray) Newly created geometry. The caller owns the returned objects.</returns>
    static member TrimBrep( geometryObject:Brep,
                            cutter:Plane,
                            [<OPT;DEF(0.0)>]tolerance:float) : Brep ResizeArray =
        let tolerance = Util.ifZero1 tolerance State.Doc.ModelAbsoluteTolerance
        let pieces = geometryObject.Trim(cutter, tolerance)
        if isNull pieces then RhinoScriptingException.Raise "TrimBrep: operation failed."
        ResizeArray(pieces)


    /// <summary>Remove portions of the Surface outside of the specified interval in U direction.</summary>
    /// <param name="surfaceId">(Guid) Surface identifier</param>
    /// <param name="interval">(float*float) Sub section of the Surface to keep.</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Should the input Surface be deleted</param>
    /// <returns>(Guid) new Surface identifier.</returns>
    static member TrimSurfaceU( surfaceId:Guid,
                                interval:float*float,
                                [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let surface = RhinoScriptSyntax.TrimSurfaceU(surfaceIdGeometry, interval)
        let id = State.Doc.Objects.AddSurface(surface)
        if deleteInput then State.Doc.Objects.Delete(surfaceId, true) |> ignore<bool>
        State.Doc.Views.Redraw()
        id


    /// <summary>Remove portions of the Surface outside of the specified interval in U direction. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Surface) Surface object</param>
    /// <param name="interval">(float*float) Sub section of the Surface to keep.</param>
    /// <returns>(Surface) Newly created geometry. The caller owns the returned objects.</returns>
    static member TrimSurfaceU( surfaceGeo:Surface,
                                interval:float*float) : Surface =
        let surface = surfaceGeo
        let mutable u = surface.Domain(0)
        let mutable v = surface.Domain(1)
        u.[0] <-  interval|> fst
        u.[1] <-  interval|> snd
        let newsurface = surface.Trim(u, v)
        if isNull newsurface then RhinoScriptingException.Raise "TrimSurfaceU: operation failed."
        newsurface


    /// <summary>Remove portions of the Surface outside of the specified interval in V direction.</summary>
    /// <param name="surfaceId">(Guid) Surface identifier</param>
    /// <param name="interval">(float*float) Sub section of the Surface to keep.</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Should the input Surface be deleted</param>
    /// <returns>(Guid) new Surface identifier.</returns>
    static member TrimSurfaceV( surfaceId:Guid,
                               interval:float*float,
                               [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let surface = RhinoScriptSyntax.TrimSurfaceV(surfaceIdGeometry, interval)
        let id = State.Doc.Objects.AddSurface(surface)
        if deleteInput then State.Doc.Objects.Delete(surfaceId, true) |> ignore<bool>
        State.Doc.Views.Redraw()
        id


    /// <summary>Remove portions of the Surface outside of the specified interval in V direction. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Surface) Surface object</param>
    /// <param name="interval">(float*float) Sub section of the Surface to keep.</param>
    /// <returns>(Surface) Newly created geometry. The caller owns the returned objects.</returns>
    static member TrimSurfaceV( surfaceGeo:Surface,
                               interval:float*float) : Surface =
        let surface = surfaceGeo
        let mutable u = surface.Domain(0)
        let mutable v = surface.Domain(1)
        v.[0] <-  interval|> fst
        v.[1] <-  interval|> snd
        let newsurface = surface.Trim(u, v)
        if isNull newsurface then RhinoScriptingException.Raise "TrimSurfaceV: operation failed."
        newsurface


    /// <summary>Remove portions of the Surface outside of the specified interval ain U and V direction.</summary>
    /// <param name="surfaceId">(Guid) Surface identifier</param>
    /// <param name="intervalU">(float*float) Sub section of the Surface to keep in U direction</param>
    /// <param name="intervalV">(float*float) Sub section of the Surface to keep in V direction</param>
    /// <param name="deleteInput">(bool) Optional, default value: <c>false</c>
    ///    Should the input Surface be deleted</param>
    /// <returns>(Guid) new Surface identifier.</returns>
    static member TrimSurfaceUV( surfaceId:Guid,
                               intervalU:float*float,
                               intervalV:float*float,
                               [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceSurface(surfaceId)
        let surface = RhinoScriptSyntax.TrimSurfaceUV(surfaceIdGeometry, intervalU, intervalV)
        let id = State.Doc.Objects.AddSurface(surface)
        if deleteInput then State.Doc.Objects.Delete(surfaceId, true) |> ignore<bool>
        State.Doc.Views.Redraw()
        id


    /// <summary>Remove portions of the Surface outside of the specified interval ain U and V direction. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Surface) Surface object</param>
    /// <param name="intervalU">(float*float) Sub section of the Surface to keep in U direction</param>
    /// <param name="intervalV">(float*float) Sub section of the Surface to keep in V direction</param>
    /// <returns>(Surface) Newly created geometry. The caller owns the returned objects.</returns>
    static member TrimSurfaceUV( surfaceGeo:Surface,
                               intervalU:float*float,
                               intervalV:float*float) : Surface =
        let surface = surfaceGeo
        let mutable u = surface.Domain(0)
        let mutable v = surface.Domain(1)
        u.[0]  <- intervalU|> fst
        u.[1]  <- intervalU|> snd
        v.[0]  <- intervalV|> fst
        v.[1]  <- intervalV|> snd
        let newsurface = surface.Trim(u, v)
        if isNull newsurface then RhinoScriptingException.Raise "TrimSurfaceUV: operation failed."
        newsurface


    /// <summary>Flattens a develop-able Surface or Polysurface.</summary>
    /// <param name="surfaceId">(Guid) The Surface's identifier</param>
    /// <param name="explode">(bool) Optional, default value: <c>false</c>
    ///    If True, the resulting Surfaces are not joined</param>
    /// <param name="followingGeometry">(Guid seq) Optional, List of Curves, dots, and points which
    ///    should be unrolled with the Surface</param>
    /// <param name="absoluteTolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Absolute tolerance</param>
    /// <param name="relativeTolerance">(float) Optional, default value: <c>State.Doc.ModelRelativeTolerance</c>
    ///    Relative tolerance</param>
    /// <returns>(Guid ResizeArray * Guid ResizeArray) Two lists: List of unrolled Surface ids and list of following objects.</returns>
    static member UnrollSurface( surfaceId:Guid,
                                 [<OPT;DEF(false)>]explode:bool,
                                 [<OPT;DEF(null:Guid seq)>]followingGeometry:Guid seq,
                                 [<OPT;DEF(0.0)>]absoluteTolerance:float,
                                 [<OPT;DEF(0.0)>]relativeTolerance:float) : Guid ResizeArray * Guid ResizeArray =
        let surfaceIdGeometry = RhinoScriptSyntax.CoerceBrep(surfaceId)
        let followingGeometryGeometry = if isNull followingGeometry then null else followingGeometry |> Seq.map RhinoScriptSyntax.CoerceGeometry
        let breps, following = RhinoScriptSyntax.UnrollSurface(surfaceIdGeometry, explode, followingGeometryGeometry, absoluteTolerance, relativeTolerance)
        let ids = breps |> RArr.mapSeq State.Doc.Objects.AddBrep
        let followingIds = ResizeArray<Guid>()
        for geometry in following do
            let id =
                match geometry with
                | :? Curve as curve -> State.Doc.Objects.AddCurve(curve)
                | :? Point as point -> State.Doc.Objects.AddPoint(point.Location)
                | :? TextDot as dot -> State.Doc.Objects.AddTextDot(dot)
                | _ -> RhinoScriptingException.Raise "UnrollSurface: unexpected following geometry."
            followingIds.Add(id)
        State.Doc.Views.Redraw()
        ids, followingIds


    /// <summary>Flattens a develop-able Surface or Polysurface. Returns geometry without adding it.</summary>
    /// <param name="surfaceGeo">(Brep) The Surface</param>
    /// <param name="explode">(bool) Optional, default value: <c>false</c>
    ///    If True, the resulting Surfaces are not joined</param>
    /// <param name="followingGeometry">(GeometryBase seq) Optional, List of Curves, dots, and points which
    ///    should be unrolled with the Surface</param>
    /// <param name="absoluteTolerance">(float) Optional, default value: <c>State.Doc.ModelAbsoluteTolerance</c>
    ///    Absolute tolerance</param>
    /// <param name="relativeTolerance">(float) Optional, default value: <c>State.Doc.ModelRelativeTolerance</c>
    ///    Relative tolerance</param>
    /// <returns>(Brep ResizeArray * GeometryBase ResizeArray) Unrolled Breps and following geometry, in curve, point, then text-dot order. The caller owns the returned objects.</returns>
    static member UnrollSurface( surfaceGeo:Brep,
                                 [<OPT;DEF(false)>]explode:bool,
                                 [<OPT;DEF(null:GeometryBase seq)>]followingGeometry:GeometryBase seq,
                                 [<OPT;DEF(0.0)>]absoluteTolerance:float,
                                 [<OPT;DEF(0.0)>]relativeTolerance:float) : Brep ResizeArray * GeometryBase ResizeArray =
        let brep = surfaceGeo
        let unroll = Unroller(brep)
        unroll.ExplodeOutput <- explode
        let relativeTolerance = Util.ifZero1 relativeTolerance  State.Doc.ModelRelativeTolerance
        let  absoluteTolerance = Util.ifZero1 absoluteTolerance  State.Doc.ModelAbsoluteTolerance
        unroll.AbsoluteTolerance <- absoluteTolerance
        unroll.RelativeTolerance <- relativeTolerance
        if notNull followingGeometry then
            for objectId in followingGeometry do
                let geom = objectId
                match geom with
                | :? Curve as g -> unroll.AddFollowingGeometry(g) //TODO verify order is correct ???
                | :? Point as g -> unroll.AddFollowingGeometry(g)
                | :? TextDot as g -> unroll.AddFollowingGeometry(g)
                | _ -> RhinoScriptingException.Raise "UnrollSurface: cannot add (a %s) as following Geometry" (Pretty.str objectId)

        let breps, curves, points, dots = unroll.PerformUnroll()
        if isNull breps then RhinoScriptingException.Raise "UnrollSurface: failed on  %A" surfaceGeo
        let following = ResizeArray<GeometryBase>()
        for curve in curves do following.Add(curve)
        for point in points do following.Add(new Point(point))
        for dot in dots do following.Add(dot)
        ResizeArray(breps), following


    /// <summary>Changes the degree of a Surface object. For more information see the Rhino help file for the ChangeDegree command.</summary>
    /// <param name="objectId">(Guid) The object's identifier</param>
    /// <param name="degree">(int * int) Two integers, specifying the degrees for the U  V directions</param>
    /// <returns>(bool) True of False indicating success or failure.</returns>
    static member ChangeSurfaceDegree(objectId:Guid, degree:int * int) : bool =
        let surface = RhinoScriptSyntax.CoerceNurbsSurface(objectId)
        let u, v = degree
        let maxnurbsdegree = 11
        if u < 1 || u > maxnurbsdegree || v < 1 || v > maxnurbsdegree ||  (surface.Degree(0) = u && surface.Degree(1) = v) then
            RhinoScriptingException.Raise "ChangeSurfaceDegree failed on %A" (Pretty.str objectId)
        let mutable r = false
        if surface.IncreaseDegreeU(u) then
            if surface.IncreaseDegreeV(v) then
                r <- State.Doc.Objects.Replace(objectId, surface)
        r



