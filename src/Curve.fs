namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsCurve =
  type RhinoScriptSyntax with
    
    ///<summary>Adds an arc curve to the document</summary>
    ///<param name="plane">(Plane) Plane on which the arc will lie. The origin of the plane will be
    ///  the center point of the arc. x-axis of the plane defines the 0 angle
    ///  direction.</param>
    ///<param name="radius">(float) Radius of the arc</param>
    ///<param name="angleDegrees">(int) Interval of arc in degrees</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddArc(plane:Plane, radius:float, angleDegrees:int) : Guid =
        failNotImpl () 


    ///<summary>Adds a 3-point arc curve to the document</summary>
    ///<param name="start">(Point3d) Start of 'endpoints of the arc' (FIXME 0)</param>
    ///<param name="ende">(Point3d) End of 'endpoints of the arc' (FIXME 0)</param>
    ///<param name="pointOnArc">(Point3d) A point on the arc</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddArc3Pt(start:Point3d, ende:Point3d, pointOnArc:Point3d) : Guid =
        failNotImpl () 


    ///<summary>Adds an arc curve, created from a start point, a start direction, and an
    ///  end point, to the document</summary>
    ///<param name="start">(Point3d) The starting point of the arc</param>
    ///<param name="direction">(Vector3d) The arc direction at start</param>
    ///<param name="ende">(Point3d) The ending point of the arc</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddArcPtTanPt(start:Point3d, direction:Vector3d, ende:Point3d) : Guid =
        failNotImpl () 


    ///<summary>Makes a curve blend between two curves</summary>
    ///<param name="curves">(Guid * Guid) List of two curves</param>
    ///<param name="parameters">(float * float) List of two curve parameters defining the blend end points</param>
    ///<param name="reverses">(bool * bool) List of two boolean values specifying to use the natural or opposite direction of the curve</param>
    ///<param name="continuities">(int * int) List of two numbers specifying continuity at end points
    ///  0 = position
    ///  1 = tangency
    ///  2 = curvature</param>
    ///<returns>(Guid) identifier of new curve on success</returns>
    static member AddBlendCurve(curves:Guid * Guid, parameters:float * float, reverses:bool * bool, continuities:int * int) : Guid =
        failNotImpl () 


    ///<summary>Adds a circle curve to the document</summary>
    ///<param name="planeOrCenter">(Plane) Plane on which the circle will lie. If a point is
    ///  passed, this will be the center of the circle on the active
    ///  construction plane</param>
    ///<param name="radius">(float) The radius of the circle</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddCircle(planeOrCenter:Plane, radius:float) : Guid =
        failNotImpl () 


    ///<summary>Adds a 3-point circle curve to the document</summary>
    ///<param name="first">(Point3d) First of 'points on the circle' (FIXME 0)</param>
    ///<param name="second">(Point3d) Second of 'points on the circle' (FIXME 0)</param>
    ///<param name="third">(Point3d) Third of 'points on the circle' (FIXME 0)</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddCircle3Pt(first:Point3d, second:Point3d, third:Point3d) : Guid =
        failNotImpl () 


    ///<summary>Adds a control points curve object to the document</summary>
    ///<param name="points">(Point3d seq) A list of points</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///Degree of the curve</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddCurve(points:Point3d seq, [<OPT;DEF(3)>]degree:int) : Guid =
        failNotImpl () 


    ///<summary>Adds an elliptical curve to the document</summary>
    ///<param name="plane">(Plane) The plane on which the ellipse will lie. The origin of
    ///  the plane will be the center of the ellipse</param>
    ///<param name="radiusX">(float) RadiusX of 'radius in the X and Y axis directions' (FIXME 0)</param>
    ///<param name="radiusY">(float) RadiusY of 'radius in the X and Y axis directions' (FIXME 0)</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddEllipse(plane:Plane, radiusX:float, radiusY:float) : Guid =
        failNotImpl () 


    ///<summary>Adds a 3-point elliptical curve to the document</summary>
    ///<param name="center">(Point3d) Center point of the ellipse</param>
    ///<param name="second">(Point3d) End point of the x axis</param>
    ///<param name="third">(Point3d) End point of the y axis</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddEllipse3Pt(center:Point3d, second:Point3d, third:Point3d) : Guid =
        failNotImpl () 


    ///<summary>Adds a fillet curve between two curve objects</summary>
    ///<param name="curve0id">(Guid) Identifier of the first curve object</param>
    ///<param name="curve1id">(Guid) Identifier of the second curve object</param>
    ///<param name="radius">(float) Optional, Default Value: <c>1.0</c>
    ///Fillet radius</param>
    ///<param name="basisPointA">(Point3d) Optional, Default Value: <c>null</c>
    ///Base point of the first curve. If omitted,
    ///  starting point of the curve is used</param>
    ///<param name="basisPointB">(Point3d) Optional, Default Value: <c>null</c>
    ///Base point of the second curve. If omitted,
    ///  starting point of the curve is used</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddFilletCurve(curve0id:Guid, curve1id:Guid, [<OPT;DEF(1.0)>]radius:float, [<OPT;DEF(null)>]basisPointA:Point3d, [<OPT;DEF(null)>]basisPointB:Point3d) : Guid =
        failNotImpl () 


    ///<summary>Adds an interpolated curve object that lies on a specified
    ///  surface.  Note, this function will not create periodic curves,
    ///  but it will create closed curves.</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to create the curve on</param>
    ///<param name="points">(Point3d seq) List of 3D points that lie on the specified surface.
    ///  The list must contain at least 2 points</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddInterpCrvOnSrf(surfaceId:Guid, points:Point3d seq) : Guid =
        failNotImpl () 


    ///<summary>Adds an interpolated curve object based on surface parameters,
    ///  that lies on a specified surface. Note, this function will not
    ///  create periodic curves, but it will create closed curves.</summary>
    ///<param name="surfaceId">(Guid) Identifier of the surface to create the curve on</param>
    ///<param name="points">(float seq) A list of 2D surface parameters. The list must contain
    ///  at least 2 sets of parameters</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddInterpCrvOnSrfUV(surfaceId:Guid, points:float seq) : Guid =
        failNotImpl () 


    ///<summary>Adds an interpolated curve object to the document. Options exist to make
    ///  a periodic curve or to specify the tangent at the endpoints. The resulting
    ///  curve is a non-rational NURBS curve of the specified degree.</summary>
    ///<param name="points">(Point3d seq) A list containing 3D points to interpolate. For periodic curves,
    ///  if the final point is a duplicate of the initial point, it is
    ///  ignored. The number of control points must be >= (degree+1).</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///The degree of the curve (must be >=1).
    ///  Periodic curves must have a degree >= 2. For knotstyle = 1 or 2,
    ///  the degree must be 3. For knotstyle = 4 or 5, the degree must be odd</param>
    ///<param name="knotstyle">(int) Optional, Default Value: <c>0</c>
    ///0 Uniform knots.  Parameter spacing between consecutive knots is 1.0.
    ///  1 Chord length spacing.  Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///  2 Sqrt (chord length).  Requires degree = 3 with arrCV1 and arrCVn1 specified.
    ///  3 Periodic with uniform spacing.
    ///  4 Periodic with chord length spacing.  Requires an odd degree value.
    ///  5 Periodic with sqrt (chord length) spacing.  Requires an odd degree value.</param>
    ///<param name="startTangent">(Vector3d) Optional, Default Value: <c>null</c>
    ///A vector that specifies a tangency condition at the
    ///  beginning of the curve. If the curve is periodic, this argument must be omitted.</param>
    ///<param name="endeTangent">(Vector3d) Optional, Default Value: <c>null</c>
    ///3d vector that specifies a tangency condition at the
    ///  end of the curve. If the curve is periodic, this argument must be omitted.</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddInterpCurve(points:Point3d seq, [<OPT;DEF(3)>]degree:int, [<OPT;DEF(0)>]knotstyle:int, [<OPT;DEF(null)>]startTangent:Vector3d, [<OPT;DEF(null)>]endeTangent:Vector3d) : Guid =
        failNotImpl () 


    ///<summary>Adds a line curve to the current model.</summary>
    ///<param name="start">(Point3d) Start of 'end points of the line' (FIXME 0)</param>
    ///<param name="ende">(Point3d) End of 'end points of the line' (FIXME 0)</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddLine(start:Point3d, ende:Point3d) : Guid =
        failNotImpl () 


    ///<summary>Adds a NURBS curve object to the document</summary>
    ///<param name="points">(Point3d seq) A list containing 3D control points</param>
    ///<param name="knots">(float seq) Knot values for the curve. The number of elements in knots must
    ///  equal the number of elements in points plus degree minus 1</param>
    ///<param name="degree">(int) Degree of the curve. must be greater than of equal to 1</param>
    ///<param name="weights">(float seq) Optional, Default Value: <c>null</c>
    ///Weight values for the curve. Number of elements should
    ///  equal the number of elements in points. Values must be greater than 0</param>
    ///<returns>(Guid) the identifier of the new object , otherwise None</returns>
    static member AddNurbsCurve(points:Point3d seq, knots:float seq, degree:int, [<OPT;DEF(null)>]weights:float seq) : Guid =
        failNotImpl () 


    ///<summary>Adds a polyline curve to the current model</summary>
    ///<param name="points">(Point3d seq) List of 3D points. Duplicate, consecutive points will be
    ///  removed. The list must contain at least two points. If the
    ///  list contains less than four points, then the first point and
    ///  last point must be different.</param>
    ///<param name="replaceId">(Guid) Optional, Default Value: <c>null</c>
    ///If set to the id of an existing object, the object
    ///  will be replaced by this polyline</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddPolyline(points:Point3d seq, [<OPT;DEF(null)>]replaceId:Guid) : Guid =
        failNotImpl () 


    ///<summary>Add a rectangular curve to the document</summary>
    ///<param name="plane">(Plane) Plane on which the rectangle will lie</param>
    ///<param name="width">(float) Width of rectangle as measured along the plane's
    ///  x and y axes</param>
    ///<param name="height">(float) Height of rectangle as measured along the plane's
    ///  x and y axes</param>
    ///<returns>(Guid) id of new rectangle</returns>
    static member AddRectangle(plane:Plane, width:float, height:float) : Guid =
        failNotImpl () 


    ///<summary>Adds a spiral or helical curve to the document</summary>
    ///<param name="point0">(Point3d) Helix axis start point or center of spiral</param>
    ///<param name="point1">(Point3d) Helix axis end point or point normal on spiral plane</param>
    ///<param name="pitch">(float) Distance between turns. If 0, then a spiral. If > 0 then the
    ///  distance between helix "threads"</param>
    ///<param name="turns">(float) Number of turns</param>
    ///<param name="radius0">(float) Starting radius of spiral</param>
    ///<param name="radius1">(float) Optional, Default Value: <c>null</c>
    ///Ending radius of spiral. If omitted, the starting radius is used for the complete spiral.</param>
    ///<returns>(Guid) id of new curve on success</returns>
    static member AddSpiral(point0:Point3d, point1:Point3d, pitch:float, turns:float, radius0:float, [<OPT;DEF(null)>]radius1:float) : Guid =
        failNotImpl () 


    ///<summary>Add a curve object based on a portion, or interval of an existing curve
    ///  object. Similar in operation to Rhino's SubCrv command</summary>
    ///<param name="curveId">(Guid) Identifier of a closed planar curve object</param>
    ///<param name="param0">(float) First parameters on the source curve</param>
    ///<param name="param1">(float) Second parameters on the source curve</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member AddSubCrv(curveId:Guid, param0:float, param1:float) : Guid =
        failNotImpl () 


    ///<summary>Returns the angle of an arc curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///Identifies the curve segment if curveId identifies a polycurve</param>
    ///<returns>(float) The angle in degrees .</returns>
    static member ArcAngle(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        failNotImpl () 


    ///<summary>Returns the center point of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D center point of the arc .</returns>
    static member ArcCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        failNotImpl () 


    ///<summary>Returns the mid point of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D mid point of the arc .</returns>
    static member ArcMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        failNotImpl () 


    ///<summary>Returns the radius of an arc curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The radius of the arc .</returns>
    static member ArcRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Returns the center point of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="returnPlane">(bool) Optional, Default Value: <c>false</c>
    ///If True, the circle's plane is returned. If omitted the plane is not returned.</param>
    ///<returns>(Point3d) The 3D center point of the circle .</returns>
    static member CircleCenterPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int, [<OPT;DEF(false)>]returnPlane:bool) : Point3d =
        failNotImpl () 


    ///<summary>Returns the circumference of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The circumference of the circle .</returns>
    static member CircleCircumference(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        failNotImpl () 


    ///<summary>Returns the radius of a circle curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The radius of the circle .</returns>
    static member CircleRadius(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        failNotImpl () 


    ///<summary>Closes an open curve object by making adjustments to the end points so
    ///  they meet at a point</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>-1.0</c>
    ///Maximum allowable distance between start and end
    ///  point. If omitted, the current absolute tolerance is used</param>
    ///<returns>(Guid) id of the new curve object</returns>
    static member CloseCurve(curveId:Guid, [<OPT;DEF(-1.0)>]tolerance:float) : Guid =
        failNotImpl () 


    ///<summary>Determine the orientation (counter-clockwise or clockwise) of a closed,
    ///  planar curve</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="direction">(Vector3d) Optional, Default Value: <c>0*0*1</c>
    ///3d vector that identifies up, or Z axs, direction of
    ///  the plane to test against</param>
    ///<returns>(float) 1 if the curve's orientation is clockwise
    ///  -1 if the curve's orientation is counter-clockwise
    ///    0 if unable to compute the curve's orientation</returns>
    static member ClosedCurveOrientation(curveId:Guid, [<OPT;DEF(null)>]direction:Vector3d) : float =
        failNotImpl () 


    ///<summary>Convert curve to a polyline curve</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>5.0</c>
    ///The maximum angle between curve tangents at line endpoints.
    ///  If omitted, the angle tolerance is set to 5.0.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.01</c>
    ///The distance tolerance at segment midpoints. If omitted, the tolerance is set to 0.01.</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete the curve object specified by curveId. If omitted, curveId will not be deleted.</param>
    ///<param name="minEdgeLength">(float) Optional, Default Value: <c>0</c>
    ///Minimum segment length</param>
    ///<param name="maxEdgeLength">(float) Optional, Default Value: <c>0</c>
    ///Maximum segment length</param>
    ///<returns>(Guid) The new curve .</returns>
    static member ConvertCurveToPolyline(curveId:Guid, [<OPT;DEF(5.0)>]angleTolerance:float, [<OPT;DEF(0.01)>]tolerance:float, [<OPT;DEF(false)>]deleteInput:bool, [<OPT;DEF(0)>]minEdgeLength:float, [<OPT;DEF(0)>]maxEdgeLength:float) : Guid =
        failNotImpl () 


    ///<summary>Returns the point on the curve that is a specified arc length
    ///  from the start of the curve.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="length">(float) The arc length from the start of the curve to evaluate.</param>
    ///<param name="fromStart">(bool) Optional, Default Value: <c>true</c>
    ///If not specified or True, then the arc length point is
    ///  calculated from the start of the curve. If False, the arc length
    ///  point is calculated from the end of the curve.</param>
    ///<returns>(Point3d) on curve</returns>
    static member CurveArcLengthPoint(curveId:Guid, length:float, [<OPT;DEF(true)>]fromStart:bool) : Point3d =
        failNotImpl () 


    ///<summary>Returns area of closed planar curves. The results are based on the
    ///  current drawing units.</summary>
    ///<param name="curveId">(Guid) The identifier of a closed, planar curve object.</param>
    ///<returns>(float * float) List of area information. The list will contain the following information:
    ///  Element  Description
    ///  [0]      The area. If more than one curve was specified, the
    ///    value will be the cumulative area.
    ///  [1]      The absolute (+/-) error bound for the area.</returns>
    static member CurveArea(curveId:Guid) : float * float =
        failNotImpl () 


    ///<summary>Returns area centroid of closed, planar curves. The results are based
    ///  on the current drawing units.</summary>
    ///<param name="curveId">(Guid) The identifier of a closed, planar curve object.</param>
    ///<returns>(Point3d * Vector3d) of area centroid information containing the following information:
    ///  Element  Description
    ///  [0]        The 3d centroid point. If more than one curve was specified,
    ///    the value will be the cumulative area.
    ///  [1]        A 3d vector with the absolute (+/-) error bound for the area
    ///    centroid.</returns>
    static member CurveAreaCentroid(curveId:Guid) : Point3d * Vector3d =
        failNotImpl () 


    ///<summary>Get status of a curve object's annotation arrows</summary>
    ///<param name="curveId">(Guid) Identifier of a curve</param>
    ///<returns>(int) The current annotation arrow style
    ///  0 = no arrows
    ///  1 = display arrow at start of curve
    ///  2 = display arrow at end of curve
    ///  3 = display arrow at both start and end of curve</returns>
    static member CurveArrows(curveId:Guid) : int =
        failNotImpl () 

    ///<summary>Enables or disables a curve object's annotation arrows</summary>
    ///<param name="curveId">(Guid) Identifier of a curve</param>
    ///<param name="arrowStyle">(int)The style of annotation arrow to be displayed. If omitted the current type is returned.
    ///  0 = no arrows
    ///  1 = display arrow at start of curve
    ///  2 = display arrow at end of curve
    ///  3 = display arrow at both start and end of curve</param>
    ///<returns>(unit) unit</returns>
    static member CurveArrows(curveId:Guid, arrowStyle:int) : unit =
        failNotImpl () 


    ///<summary>Calculates the difference between two closed, planar curves and
    ///  adds the results to the document. Note, curves must be coplanar.</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object.</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///A positive tolerance value, or None for the doc default.</param>
    ///<returns>(Guid seq) The identifiers of the new objects , .</returns>
    static member CurveBooleanDifference(curveA:Guid, curveB:Guid, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        failNotImpl () 


    ///<summary>Calculates the intersection of two closed, planar curves and adds
    ///  the results to the document. Note, curves must be coplanar.</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object.</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///A positive tolerance value, or None for the doc default.</param>
    ///<returns>(Guid seq) The identifiers of the new objects.</returns>
    static member CurveBooleanIntersection(curveA:Guid, curveB:Guid, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        failNotImpl () 


    ///<summary>Calculate the union of two or more closed, planar curves and
    ///  add the results to the document. Note, curves must be coplanar.</summary>
    ///<param name="curveId">(Guid seq) List of two or more close planar curves identifiers</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///A positive tolerance value, or None for the doc default.</param>
    ///<returns>(Guid seq) The identifiers of the new objects.</returns>
    static member CurveBooleanUnion(curveId:Guid seq, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        failNotImpl () 


    ///<summary>Intersects a curve object with a brep object. Note, unlike the
    ///  CurveSurfaceIntersection function, this function works on trimmed surfaces.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="brepId">(Guid) Identifier of a brep object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///Distance tolerance at segment midpoints.
    ///  If omitted, the current absolute tolerance is used.</param>
    ///<returns>(Guid seq) identifiers for the newly created intersection objects .</returns>
    static member CurveBrepIntersect(curveId:Guid, brepId:Guid, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        failNotImpl () 


    ///<summary>Returns the 3D point locations on two objects where they are closest to
    ///  each other. Note, this function provides similar functionality to that of
    ///  Rhino's ClosestPt command.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object to test</param>
    ///<param name="objectIds">(Guid seq) List of identifiers of point cloud, curve, surface, or
    ///  polysurface to test against</param>
    ///<returns>(Guid * Point3d * Point3d) containing the results of the closest point calculation.
    ///  The elements are as follows:
    ///    [0]    The identifier of the closest object.
    ///    [1]    The 3-D point that is closest to the closest object.
    ///    [2]    The 3-D point that is closest to the test curve.</returns>
    static member CurveClosestObject(curveId:Guid, objectIds:Guid seq) : Guid * Point3d * Point3d =
        failNotImpl () 


    ///<summary>Returns parameter of the point on a curve that is closest to a test point.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object</param>
    ///<param name="point">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///Curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The parameter of the closest point on the curve</returns>
    static member CurveClosestPoint(curveId:Guid, point:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        failNotImpl () 


    ///<summary>Returns the 3D point locations calculated by contouring a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object.</param>
    ///<param name="startPoint">(Point3d) 3D starting point of a center line.</param>
    ///<param name="endePoint">(Point3d) 3D ending point of a center line.</param>
    ///<param name="interval">(float) Optional, Default Value: <c>null</c>
    ///The distance between contour curves. If omitted,
    ///  the interval will be equal to the diagonal distance of the object's
    ///  bounding box divided by 50.</param>
    ///<returns>(Point3d seq) A list of 3D points, one for each contour</returns>
    static member CurveContourPoints(curveId:Guid, startPoint:Point3d, endePoint:Point3d, [<OPT;DEF(null)>]interval:float) : Point3d seq =
        failNotImpl () 


    ///<summary>Returns the curvature of a curve at a parameter. See the Rhino help for
    ///  details on curve curvature</summary>
    ///<param name="curveId">(Guid) Identifier of the curve</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<returns>(Point3d * Vector3d * Point3d * float * Vector3d) of curvature information on success
    ///  [0] = point at specified parameter
    ///  [1] = tangent vector
    ///  [2] = center of radius of curvature
    ///  [3] = radius of curvature
    ///  [4] = curvature vector</returns>
    static member CurveCurvature(curveId:Guid, parameter:float) : Point3d * Vector3d * Point3d * float * Vector3d =
        failNotImpl () 


    ///<summary>Calculates intersection of two curve objects.</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object.</param>
    ///<param name="curveB">(Guid) Optional, Default Value: <c>null</c>
    ///Identifier of the second curve object. If omitted, then a
    ///  self-intersection test will be performed on curveA.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>-1</c>
    ///Absolute tolerance in drawing units. If omitted,
    ///  the document's current absolute tolerance is used.</param>
    ///<returns>((int*Point3d*Point3d*Point3d*int*int*int*int*int*int) array) list of tuples: containing intersection information .
    ///  The list will contain one or more of the following elements:
    ///    Element Type     Description
    ///    [n][0]  Number   The intersection event type, either Point (1) or Overlap (2).
    ///    [n][1]  Point3d  If the event type is Point (1), then the intersection point
    ///      on the first curve. If the event type is Overlap (2), then
    ///      intersection start point on the first curve.
    ///    [n][2]  Point3d  If the event type is Point (1), then the intersection point
    ///      on the first curve. If the event type is Overlap (2), then
    ///      intersection end point on the first curve.
    ///    [n][3]  Point3d  If the event type is Point (1), then the intersection point
    ///      on the second curve. If the event type is Overlap (2), then
    ///      intersection start point on the second curve.
    ///    [n][4]  Point3d  If the event type is Point (1), then the intersection point
    ///      on the second curve. If the event type is Overlap (2), then
    ///      intersection end point on the second curve.
    ///    [n][5]  Number   If the event type is Point (1), then the first curve parameter.
    ///      If the event type is Overlap (2), then the start value of the
    ///      first curve parameter range.
    ///    [n][6]  Number   If the event type is Point (1), then the first curve parameter.
    ///      If the event type is Overlap (2), then the end value of the
    ///      first curve parameter range.
    ///    [n][7]  Number   If the event type is Point (1), then the second curve parameter.
    ///      If the event type is Overlap (2), then the start value of the
    ///      second curve parameter range.
    ///    [n][8]  Number   If the event type is Point (1), then the second curve parameter.
    ///      If the event type is Overlap (2), then the end value of the
    ///      second curve parameter range.</returns>
    static member CurveCurveIntersection(curveA:Guid, [<OPT;DEF(null)>]curveB:Guid, [<OPT;DEF(-1)>]tolerance:float) : (int*Point3d*Point3d*Point3d*int*int*int*int*int*int) array =
        failNotImpl () 


    ///<summary>Returns the degree of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve.</param>
    ///<returns>(int) The degree of the curve .</returns>
    static member CurveDegree(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        failNotImpl () 


    ///<summary>Returns the minimum and maximum deviation between two curve objects</summary>
    ///<param name="curveA">(Guid) Curve a of 'identifiers of two curves' (FIXME 0)</param>
    ///<param name="curveB">(Guid) Curve b of 'identifiers of two curves' (FIXME 0)</param>
    ///<returns>(float * float * float * float * float * float) of deviation information on success
    ///  [0] = curve_a parameter at maximum overlap distance point
    ///  [1] = curve_b parameter at maximum overlap distance point
    ///  [2] = maximum overlap distance
    ///  [3] = curve_a parameter at minimum overlap distance point
    ///  [4] = curve_b parameter at minimum overlap distance point
    ///  [5] = minimum distance between curves</returns>
    static member CurveDeviation(curveA:Guid, curveB:Guid) : float * float * float * float * float * float =
        failNotImpl () 


    ///<summary>Returns the dimension of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of a curve object.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if `curveId` identifies a polycurve.</param>
    ///<returns>(float) The dimension of the curve . .</returns>
    static member CurveDim(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        failNotImpl () 


    ///<summary>Tests if two curve objects are generally in the same direction or if they
    ///  would be more in the same direction if one of them were flipped. When testing
    ///  curve directions, both curves must be either open or closed - you cannot test
    ///  one open curve and one closed curve.</summary>
    ///<param name="curveA">(Guid) Identifier of first curve object</param>
    ///<param name="curveB">(Guid) Identifier of second curve object</param>
    ///<returns>(bool) True if the curve directions match, otherwise False.</returns>
    static member CurveDirectionsMatch(curveA:Guid, curveB:Guid) : bool =
        failNotImpl () 


    ///<summary>Search for a derivatitive, tangent, or curvature discontinuity in
    ///  a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of curve object</param>
    ///<param name="style">(int) The type of continuity to test for. The types of
    ///  continuity are as follows:
    ///  Value    Description
    ///  1        C0 - Continuous function
    ///  2        C1 - Continuous first derivative
    ///  3        C2 - Continuous first and second derivative
    ///  4        G1 - Continuous unit tangent
    ///  5        G2 - Continuous unit tangent and curvature</param>
    ///<returns>(Point3d seq) 3D points where the curve is discontinuous</returns>
    static member CurveDiscontinuity(curveId:Guid, style:int) : Point3d seq =
        failNotImpl () 


    ///<summary>Returns the domain of a curve object
    ///  as an indexable object with two elements.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve.</param>
    ///<returns>(float * float) the domain of the curve .
    ///  [0] Domain minimum
    ///  [1] Domain maximum</returns>
    static member CurveDomain(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float * float =
        failNotImpl () 


    ///<summary>Returns the edit, or Greville, points of a curve object.
    ///  For each curve control point, there is a corresponding edit point.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="returnParameters">(bool) Optional, Default Value: <c>false</c>
    ///If True, return as a list of curve parameters.
    ///  If False, return as a list of 3d points</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index is `curveId` identifies a polycurve</param>
    ///<returns>(Point3d seq) curve edit points on success</returns>
    static member CurveEditPoints(curveId:Guid, [<OPT;DEF(false)>]returnParameters:bool, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d seq =
        failNotImpl () 


    ///<summary>Returns the end point of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3d endpoint of the curve .</returns>
    static member CurveEndPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Find points at which to cut a pair of curves so that a fillet of a
    ///  specified radius fits. A fillet point is a pair of points (point0, point1)
    ///  such that there is a circle of radius tangent to curve curve0 at point0 and
    ///  tangent to curve curve1 at point1. Of all possible fillet points, this
    ///  function returns the one which is the closest to the base point base_point_0,
    ///  base_point_1. Distance from the base point is measured by the sum of arc
    ///  lengths along the two curves.</summary>
    ///<param name="curveA">(Guid) Identifier of the first curve object.</param>
    ///<param name="curveB">(Guid) Identifier of the second curve object.</param>
    ///<param name="radius">(float) Optional, Default Value: <c>1.0</c>
    ///The fillet radius. If omitted, a radius
    ///  of 1.0 is specified.</param>
    ///<param name="basisPointA">(Point3d) Optional, Default Value: <c>null</c>
    ///The base point on the first curve.
    ///  If omitted, the starting point of the curve is used.</param>
    ///<param name="basisPointB">(Point3d) Optional, Default Value: <c>null</c>
    ///The base point on the second curve. If omitted,
    ///  the starting point of the curve is used.</param>
    ///<param name="returnPoints">(bool) Optional, Default Value: <c>true</c>
    ///If True (Default), then fillet points are
    ///  returned. Otherwise, a fillet curve is created and
    ///  it's identifier is returned.</param>
    ///<returns>(Point3d * Point3d * Point3d * Vector3d * Vector3d * Vector3d) If return_points is True, then a list of point and vector values
    ///  . The list elements are as follows:
    ///    [0]    A point on the first curve at which to cut (point).
    ///    [1]    A point on the second curve at which to cut (point).
    ///    [2]    The fillet plane's origin (point). This point is also
    ///      the center point of the fillet
    ///    [3]    The fillet plane's X axis (vector).
    ///    [4]    The fillet plane's Y axis (vector).
    ///    [5]    The fillet plane's Z axis (vector).</returns>
    static member CurveFilletPoints(curveA:Guid, curveB:Guid, [<OPT;DEF(1.0)>]radius:float, [<OPT;DEF(null)>]basisPointA:Point3d, [<OPT;DEF(null)>]basisPointB:Point3d, [<OPT;DEF(true)>]returnPoints:bool) : Point3d * Point3d * Point3d * Vector3d * Vector3d * Vector3d =
        failNotImpl () 


    ///<summary>Returns the plane at a parameter of a curve. The plane is based on the
    ///  tangent and curvature vectors at a parameter.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<param name="parameter">(float) Parameter to evaluate.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The plane at the specified parameter .</returns>
    static member CurveFrame(curveId:Guid, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        failNotImpl () 


    ///<summary>Returns the knot count of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if `curveId` identifies a polycurve.</param>
    ///<returns>(int) The number of knots .</returns>
    static member CurveKnotCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        failNotImpl () 


    ///<summary>Returns the knots, or knot vector, of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve.</param>
    ///<returns>(float seq) knot values .</returns>
    static member CurveKnots(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float seq =
        failNotImpl () 


    ///<summary>Returns the length of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="subDomain">(float * float) Optional, Default Value: <c>null</c>
    ///List of two numbers identifying the sub-domain of the
    ///  curve on which the calculation will be performed. The two parameters
    ///  (sub-domain) must be non-decreasing. If omitted, the length of the
    ///  entire curve is returned.</param>
    ///<returns>(float) The length of the curve .</returns>
    static member CurveLength(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int, [<OPT;DEF(null)>]subDomain:float * float) : float =
        failNotImpl () 


    ///<summary>Returns the mid point of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) The 3D midpoint of the curve .</returns>
    static member CurveMidPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        failNotImpl () 


    ///<summary>Returns the normal direction of the plane in which a planar curve object lies.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if curveId identifies a polycurve</param>
    ///<returns>(Vector3d) The 3D normal vector .</returns>
    static member CurveNormal(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        failNotImpl () 


    ///<summary>Converts a curve parameter to a normalized curve parameter;
    ///  one that ranges between 0-1</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) The curve parameter to convert</param>
    ///<returns>(float) normalized curve parameter</returns>
    static member CurveNormalizedParameter(curveId:Guid, parameter:float) : float =
        failNotImpl () 


    ///<summary>Converts a normalized curve parameter to a curve parameter;
    ///  one within the curve's domain</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) The normalized curve parameter to convert</param>
    ///<returns>(float) curve parameter</returns>
    static member CurveParameter(curveId:Guid, parameter:float) : float =
        failNotImpl () 


    ///<summary>Returns the perpendicular plane at a parameter of a curve. The result
    ///  is relatively parallel (zero-twisting) plane</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<returns>(Plane) Plane on success</returns>
    static member CurvePerpFrame(curveId:Guid, parameter:float) : Plane =
        failNotImpl () 


    ///<summary>Returns the plane in which a planar curve lies. Note, this function works
    ///  only on planar curves.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Plane) The plane in which the curve lies .</returns>
    static member CurvePlane(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Plane =
        failNotImpl () 


    ///<summary>Returns the control points count of a curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(int) Number of control points .</returns>
    static member CurvePointCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        failNotImpl () 


    ///<summary>Returns the control points, or control vertices, of a curve object.
    ///  If the curve is a rational NURBS curve, the euclidean control vertices
    ///  are returned.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d seq) the control points, or control vertices, of a curve object</returns>
    static member CurvePoints(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d seq =
        failNotImpl () 


    ///<summary>Returns the radius of curvature at a point on a curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="testPoint">(Point3d) Sampling point</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment if curveId identifies a polycurve</param>
    ///<returns>(float) The radius of curvature at the point on the curve .</returns>
    static member CurveRadius(curveId:Guid, testPoint:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        failNotImpl () 


    ///<summary>Adjusts the seam, or start/end, point of a closed curve.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) The parameter of the new start/end point.
    ///  Note, if successful, the resulting curve's
    ///  domain will start at `parameter`.</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member CurveSeam(curveId:Guid, parameter:float) : bool =
        failNotImpl () 


    ///<summary>Returns the start point of a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<param name="point">(Point3d) Optional, Default Value: <c>null</c>
    ///New start point</param>
    ///<returns>(Point3d) The 3D starting point of the curve .</returns>
    static member CurveStartPoint(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int, [<OPT;DEF(null)>]point:Point3d) : Point3d =
        failNotImpl () 


    ///<summary>Calculates intersection of a curve object with a surface object.
    ///  Note, this function works on the untrimmed portion of the surface.</summary>
    ///<param name="curveId">(Guid) The identifier of the first curve object.</param>
    ///<param name="surfaceId">(Guid) The identifier of the second curve object. If omitted,
    ///  the a self-intersection test will be performed on curve.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>-1</c>
    ///The absolute tolerance in drawing units. If omitted,
    ///  the document's current absolute tolerance is used.</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>-1</c>
    ///Angle tolerance in degrees. The angle
    ///  tolerance is used to determine when the curve is tangent to the
    ///  surface. If omitted, the document's current angle tolerance is used.</param>
    ///<returns>((int*Point3d*Point3d*Point3d*int*int*int*int*int*int) array) of intersection information .
    ///  The list will contain one or more of the following elements:
    ///    Element Type     Description
    ///    [n][0]  Number   The intersection event type, either Point(1) or Overlap(2).
    ///    [n][1]  Point3d  If the event type is Point(1), then the intersection point
    ///      on the first curve. If the event type is Overlap(2), then
    ///      intersection start point on the first curve.
    ///    [n][2]  Point3d  If the event type is Point(1), then the intersection point
    ///      on the first curve. If the event type is Overlap(2), then
    ///      intersection end point on the first curve.
    ///    [n][3]  Point3d  If the event type is Point(1), then the intersection point
    ///      on the second curve. If the event type is Overlap(2), then
    ///      intersection start point on the surface.
    ///    [n][4]  Point3d  If the event type is Point(1), then the intersection point
    ///      on the second curve. If the event type is Overlap(2), then
    ///      intersection end point on the surface.
    ///    [n][5]  Number   If the event type is Point(1), then the first curve parameter.
    ///      If the event type is Overlap(2), then the start value of the
    ///      first curve parameter range.
    ///    [n][6]  Number   If the event type is Point(1), then the first curve parameter.
    ///      If the event type is Overlap(2), then the end value of the
    ///      curve parameter range.
    ///    [n][7]  Number   If the event type is Point(1), then the U surface parameter.
    ///      If the event type is Overlap(2), then the U surface parameter
    ///      for curve at (n, 5).
    ///    [n][8]  Number   If the event type is Point(1), then the V surface parameter.
    ///      If the event type is Overlap(2), then the V surface parameter
    ///      for curve at (n, 5).
    ///    [n][9]  Number   If the event type is Point(1), then the U surface parameter.
    ///      If the event type is Overlap(2), then the U surface parameter
    ///      for curve at (n, 6).
    ///    [n][10] Number   If the event type is Point(1), then the V surface parameter.
    ///      If the event type is Overlap(2), then the V surface parameter
    ///      for curve at (n, 6).</returns>
    static member CurveSurfaceIntersection(curveId:Guid, surfaceId:Guid, [<OPT;DEF(-1)>]tolerance:float, [<OPT;DEF(-1)>]angleTolerance:float) : (int*Point3d*Point3d*Point3d*int*int*int*int*int*int) array =
        failNotImpl () 


    ///<summary>Returns a 3D vector that is the tangent to a curve at a parameter.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Vector3d) A 3D vector .</returns>
    static member CurveTangent(curveId:Guid, parameter:float, [<OPT;DEF(-1)>]segmentIndex:int) : Vector3d =
        failNotImpl () 


    ///<summary>Returns list of weights that are assigned to the control points of a curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(float) The weight values of the curve .</returns>
    static member CurveWeights(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : float =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Divides a curve object into a specified number of segments.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segments">(int) The number of segments.</param>
    ///<param name="createPoints">(bool) Optional, Default Value: <c>false</c>
    ///Create the division points. If omitted or False,
    ///  points are not created.</param>
    ///<param name="returnPoints">(bool) Optional, Default Value: <c>true</c>
    ///If omitted or True, points are returned.
    ///  If False, then a list of curve parameters are returned.</param>
    ///<returns>(float seq) If `return_points` is not specified or True, then a list containing 3D division points.</returns>
    static member DivideCurve(curveId:Guid, segments:int, [<OPT;DEF(false)>]createPoints:bool, [<OPT;DEF(true)>]returnPoints:bool) : float seq =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Divides a curve such that the linear distance between the points is equal.</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="distance">(int) Linear distance between division points</param>
    ///<param name="createPoints">(bool) Optional, Default Value: <c>false</c>
    ///Create the division points if True.</param>
    ///<param name="returnPoints">(bool) Optional, Default Value: <c>true</c>
    ///If True, return a list of points.
    ///  If False, return a list of curve parameters</param>
    ///<returns>(float seq) points or curve parameters based on the value of return_points
    ///  none on error</returns>
    static member DivideCurveEquidistant(curveId:Guid, distance:int, [<OPT;DEF(false)>]createPoints:bool, [<OPT;DEF(true)>]returnPoints:bool) : float seq =
        failNotImpl () 


    //(FIXME) VarOutTypes
    ///<summary>Divides a curve object into segments of a specified length.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="length">(float) The length of each segment.</param>
    ///<param name="createPoints">(bool) Optional, Default Value: <c>false</c>
    ///Create the division points. If omitted or False,
    ///  points are not created.</param>
    ///<param name="returnPoints">(bool) Optional, Default Value: <c>true</c>
    ///If omitted or True, points are returned.
    ///  If False, then a list of curve parameters are returned.</param>
    ///<returns>(Point3d seq) If return_points is not specified or True, then a list containing division points.</returns>
    static member DivideCurveLength(curveId:Guid, length:float, [<OPT;DEF(false)>]createPoints:bool, [<OPT;DEF(true)>]returnPoints:bool) : Point3d seq =
        failNotImpl () 


    ///<summary>Returns the center point of an elliptical-shaped curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<returns>(Point3d) The 3D center point of the ellipse .</returns>
    static member EllipseCenterPoint(curveId:Guid) : Point3d =
        failNotImpl () 


    ///<summary>Returns the quadrant points of an elliptical-shaped curve object.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object.</param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four points identifying the quadrants of the ellipse</returns>
    static member EllipseQuadPoints(curveId:Guid) : Point3d * Point3d * Point3d * Point3d =
        failNotImpl () 


    ///<summary>Evaluates a curve at a parameter and returns a 3D point</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="t">(float) The parameter to evaluate</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(Point3d) a 3-D point</returns>
    static member EvaluateCurve(curveId:Guid, t:float, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d =
        failNotImpl () 


    ///<summary>Explodes, or un-joins, one curves. Polycurves will be exploded into curve
    ///  segments. Polylines will be exploded into line segments. ExplodeCurves will
    ///  return the curves in topological order.</summary>
    ///<param name="curveIds">(Guid) The curve object(s) to explode.</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input objects after exploding if True.</param>
    ///<returns>(Guid seq) identifying the newly created curve objects</returns>
    static member ExplodeCurves(curveIds:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid seq =
        failNotImpl () 


    ///<summary>Extends a non-closed curve object by a line, arc, or smooth extension
    ///  until it intersects a collection of objects.</summary>
    ///<param name="curveId">(Guid) Identifier of curve to extend</param>
    ///<param name="extensionType">(int) 0 = line
    ///  1 = arc
    ///  2 = smooth</param>
    ///<param name="side">(int) 0=extend from the start of the curve
    ///  1=extend from the end of the curve
    ///  2=extend from both the start and the end of the curve</param>
    ///<param name="boundaryObjectIds">(Guid) Curve, surface, and polysurface objects to extend to</param>
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member ExtendCurve(curveId:Guid, extensionType:int, side:int, boundaryObjectIds:Guid) : Guid =
        failNotImpl () 


    ///<summary>Extends a non-closed curve by a line, arc, or smooth extension for a
    ///  specified distance</summary>
    ///<param name="curveId">(Guid) Curve to extend</param>
    ///<param name="extensionType">(int) 0 = line
    ///  1 = arc
    ///  2 = smooth</param>
    ///<param name="side">(int) 0=extend from start of the curve
    ///  1=extend from end of the curve
    ///  2=Extend from both ends</param>
    ///<param name="length">(float) Distance to extend</param>
    ///<returns>(Guid) The identifier of the new object</returns>
    static member ExtendCurveLength(curveId:Guid, extensionType:int, side:int, length:float) : Guid =
        failNotImpl () 


    ///<summary>Extends a non-closed curve by smooth extension to a point</summary>
    ///<param name="curveId">(Guid) Curve to extend</param>
    ///<param name="side">(int) 0=extend from start of the curve
    ///  1=extend from end of the curve</param>
    ///<param name="point">(Point3d) Point to extend to</param>
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member ExtendCurvePoint(curveId:Guid, side:int, point:Point3d) : Guid =
        failNotImpl () 


    ///<summary>Fairs a curve. Fair works best on degree 3 (cubic) curves. Fair attempts
    ///  to remove large curvature variations while limiting the geometry changes to
    ///  be no more than the specified tolerance. Sometimes several applications of
    ///  this method are necessary to remove nasty curvature problems.</summary>
    ///<param name="curveId">(Guid) Curve to fair</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>1.0</c>
    ///Fairing tolerance</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member FairCurve(curveId:Guid, [<OPT;DEF(1.0)>]tolerance:float) : bool =
        failNotImpl () 


    ///<summary>Reduces number of curve control points while maintaining the curve's same
    ///  general shape. Use this function for replacing curves with many control
    ///  points. For more information, see the Rhino help for the FitCrv command.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///The curve degree, which must be greater than 1.
    ///  The default is 3.</param>
    ///<param name="distanceTolerance">(float) Optional, Default Value: <c>-1</c>
    ///The fitting tolerance. If distanceTolerance
    ///  is not specified or <= 0.0, the document absolute tolerance is used.</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>-1</c>
    ///The kink smoothing tolerance in degrees. If
    ///  angleTolerance is 0.0, all kinks are smoothed. If angleTolerance
    ///  is > 0.0, kinks smaller than angleTolerance are smoothed. If
    ///  angleTolerance is not specified or < 0.0, the document angle
    ///  tolerance is used for the kink smoothing.</param>
    ///<returns>(Guid) The identifier of the new object</returns>
    static member FitCurve(curveId:Guid, [<OPT;DEF(3)>]degree:int, [<OPT;DEF(-1)>]distanceTolerance:float, [<OPT;DEF(-1)>]angleTolerance:float) : Guid =
        failNotImpl () 


    ///<summary>Inserts a knot into a curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="parameter">(float) Parameter on the curve</param>
    ///<param name="symmetrical">(bool) Optional, Default Value: <c>false</c>
    ///If True, then knots are added on both sides of
    ///  the center of the curve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member InsertCurveKnot(curveId:Guid, parameter:float, [<OPT;DEF(false)>]symmetrical:bool) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is an open arc curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsArc(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a circle curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///If the curve is not a circle, then the tolerance used
    ///  to determine whether or not the NURBS form of the curve has the
    ///  properties of a circle. If omitted, Rhino's internal zero tolerance is used</param>
    ///<returns>(bool) True or False</returns>
    static member IsCircle(curveId:Guid, [<OPT;DEF(null)>]tolerance:float) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a curve</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurve(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Decide if it makes sense to close off the curve by moving the end point
    ///  to the start point based on start-end gap size and length of curve as
    ///  approximated by chord defined by 6 points</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///Maximum allowable distance between start point and end
    ///  point. If omitted, the document's current absolute tolerance is used</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurveClosable(curveId:Guid, [<OPT;DEF(null)>]tolerance:float) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a closed curve object</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True  otherwise False.</returns>
    static member IsCurveClosed(objectId:Guid) : bool =
        failNotImpl () 


    ///<summary>Test a curve to see if it lies in a specific plane</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>null</c>
    ///Plane to test. If omitted, the active construction plane is used</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurveInPlane(objectId:Guid, [<OPT;DEF(null)>]plane:Plane) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a linear curve</summary>
    ///<param name="objectId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurveLinear(objectId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a periodic curve object</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsCurvePeriodic(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a planar curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurvePlanar(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a rational NURBS curve</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curveId` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsCurveRational(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is an elliptical-shaped curve</summary>
    ///<param name="objectId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsEllipse(objectId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a line curve</summary>
    ///<param name="objectId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsLine(objectId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Verifies that a point is on a curve</summary>
    ///<param name="objectId">(Guid) Identifier of the curve object</param>
    ///<param name="point">(Point3d) The test point</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsPointOnCurve(objectId:Guid, point:Point3d, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a PolyCurve curve</summary>
    ///<param name="objectId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsPolyCurve(objectId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Verifies an object is a Polyline curve object</summary>
    ///<param name="objectId">(Guid) Identifier of the curve object</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///The curve segment index if `curve_id` identifies a polycurve</param>
    ///<returns>(bool) True or False</returns>
    static member IsPolyline(objectId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : bool =
        failNotImpl () 


    ///<summary>Joins multiple curves together to form one or more curves or polycurves</summary>
    ///<param name="objectIds">(Guid) List of multiple curves</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input objects after joining</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///Join tolerance. If omitted, 2.1 * document absolute
    ///  tolerance is used</param>
    ///<returns>(Guid seq) Object id representing the new curves</returns>
    static member JoinCurves(objectIds:Guid, [<OPT;DEF(false)>]deleteInput:bool, [<OPT;DEF(null)>]tolerance:float) : Guid seq =
        failNotImpl () 


    ///<summary>Returns a line that was fit through an array of 3D points</summary>
    ///<param name="points">(Point3d seq) A list of at least two 3D points</param>
    ///<returns>(Line) line on success</returns>
    static member LineFitFromPoints(points:Point3d seq) : Line =
        failNotImpl () 


    ///<summary>Makes a periodic curve non-periodic. Non-periodic curves can develop
    ///  kinks when deformed</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete the input curve. If omitted, the input curve will not be deleted.</param>
    ///<returns>(Guid) id of the new or modified curve</returns>
    static member MakeCurveNonPeriodic(curveId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        failNotImpl () 


    ///<summary>Creates an average curve from two curves</summary>
    ///<param name="curve0">(Guid) Curve0 of 'identifiers of two curves' (FIXME 0)</param>
    ///<param name="curve1">(Guid) Curve1 of 'identifiers of two curves' (FIXME 0)</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///Angle tolerance used to match kinks between curves</param>
    ///<returns>(Guid) id of the new or modified curve</returns>
    static member MeanCurve(curve0:Guid, curve1:Guid, [<OPT;DEF(null)>]tolerance:float) : Guid =
        failNotImpl () 


    ///<summary>Creates a polygon mesh object based on a closed polyline curve object.
    ///  The created mesh object is added to the document</summary>
    ///<param name="polylineId">(Guid) Identifier of the polyline curve object</param>
    ///<returns>(Guid) identifier of the new mesh object</returns>
    static member MeshPolyline(polylineId:Guid) : Guid =
        failNotImpl () 


    ///<summary>Offsets a curve by a distance. The offset curve will be added to Rhino</summary>
    ///<param name="objectId">(Guid) Identifier of a curve object</param>
    ///<param name="direction">(Point3d) Point describing direction of the offset</param>
    ///<param name="distance">(float) Distance of the offset</param>
    ///<param name="normal">(Vector3d) Optional, Default Value: <c>null</c>
    ///Normal of the plane in which the offset will occur.
    ///  If omitted, the normal of the active construction plane will be used</param>
    ///<param name="style">(int) Optional, Default Value: <c>1</c>
    ///The corner style. If omitted, the style is sharp.
    ///  0 = None
    ///  1 = Sharp
    ///  2 = Round
    ///  3 = Smooth
    ///  4 = Chamfer</param>
    ///<returns>(Guid seq) list of ids for the new curves on success</returns>
    static member OffsetCurve(objectId:Guid, direction:Point3d, distance:float, [<OPT;DEF(null)>]normal:Vector3d, [<OPT;DEF(1)>]style:int) : Guid seq =
        failNotImpl () 


    ///<summary>Offset a curve on a surface. The source curve must lie on the surface.
    ///  The offset curve or curves will be added to Rhino</summary>
    ///<param name="curveId">(Guid) Curve identifiers</param>
    ///<param name="surfaceId">(Guid) Surface identifiers</param>
    ///<param name="distanceOrParameter">(float) ): If a single number is passed, then this is the
    ///  distance of the offset. Based on the curve's direction, a positive value
    ///  will offset to the left and a negative value will offset to the right.
    ///  If a tuple of two values is passed, this is interpreted as the surface
    ///  U,V parameter that the curve will be offset through</param>
    ///<returns>(Guid seq) identifiers of the new curves</returns>
    static member OffsetCurveOnSurface(curveId:Guid, surfaceId:Guid, distanceOrParameter:float) : Guid seq =
        failNotImpl () 


    ///<summary>Determines the relationship between the regions bounded by two coplanar
    ///  simple closed curves</summary>
    ///<param name="curveA">(Guid) Curve a of 'identifiers of two planar, closed curves' (FIXME 0)</param>
    ///<param name="curveB">(Guid) Curve b of 'identifiers of two planar, closed curves' (FIXME 0)</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>null</c>
    ///Test plane. If omitted, the currently active construction
    ///  plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///If omitted, the document absolute tolerance is used</param>
    ///<returns>(float) a number identifying the relationship
    ///  0 = the regions bounded by the curves are disjoint
    ///  1 = the two curves intersect
    ///  2 = the region bounded by curve_a is inside of curve_b
    ///  3 = the region bounded by curve_b is inside of curve_a</returns>
    static member PlanarClosedCurveContainment(curveA:Guid, curveB:Guid, [<OPT;DEF(null)>]plane:Plane, [<OPT;DEF(null)>]tolerance:float) : float =
        failNotImpl () 


    ///<summary>Determines if two coplanar curves intersect</summary>
    ///<param name="curveA">(Guid) Curve a of 'identifiers of two planar curves' (FIXME 0)</param>
    ///<param name="curveB">(Guid) Curve b of 'identifiers of two planar curves' (FIXME 0)</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>null</c>
    ///Test plane. If omitted, the currently active construction
    ///  plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///If omitted, the document absolute tolerance is used</param>
    ///<returns>(bool) True if the curves intersect; otherwise False</returns>
    static member PlanarCurveCollision(curveA:Guid, curveB:Guid, [<OPT;DEF(null)>]plane:Plane, [<OPT;DEF(null)>]tolerance:float) : bool =
        failNotImpl () 


    ///<summary>Determines if a point is inside of a closed curve, on a closed curve, or
    ///  outside of a closed curve</summary>
    ///<param name="point">(Point3d) Text point</param>
    ///<param name="curve">(Guid) Identifier of a curve object</param>
    ///<param name="plane">(Plane) Optional, Default Value: <c>null</c>
    ///Plane containing the closed curve and point. If omitted,
    ///  the currently active construction plane is used</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>null</c>
    ///It omitted, the document abosulte tolerance is used</param>
    ///<returns>(float) number identifying the result
    ///  0 = point is outside of the curve
    ///  1 = point is inside of the curve
    ///  2 = point in on the curve</returns>
    static member PointInPlanarClosedCurve(point:Point3d, curve:Guid, [<OPT;DEF(null)>]plane:Plane, [<OPT;DEF(null)>]tolerance:float) : float =
        failNotImpl () 


    ///<summary>Returns the number of curve segments that make up a polycurve</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///If `curveId` identifies a polycurve object, then `segmentIndex` identifies the curve segment of the polycurve to query.</param>
    ///<returns>(int) the number of curve segments in a polycurve</returns>
    static member PolyCurveCount(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : int =
        failNotImpl () 


    ///<summary>Returns the vertices of a polyline curve on success</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="segmentIndex">(int) Optional, Default Value: <c>-1</c>
    ///If curveId identifies a polycurve object, then segmentIndex identifies the curve segment of the polycurve to query.</param>
    ///<returns>(Point3d seq) an list of Point3d vertex points</returns>
    static member PolylineVertices(curveId:Guid, [<OPT;DEF(-1)>]segmentIndex:int) : Point3d seq =
        failNotImpl () 


    ///<summary>Projects one or more curves onto one or more surfaces or meshes</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of curves to project</param>
    ///<param name="meshIds">(Guid seq) Identifiers of meshes to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid seq) list of identifiers for the resulting curves.</returns>
    static member ProjectCurveToMesh(curveIds:Guid seq, meshIds:Guid seq, direction:Vector3d) : Guid seq =
        failNotImpl () 


    ///<summary>Projects one or more curves onto one or more surfaces or polysurfaces</summary>
    ///<param name="curveIds">(Guid seq) Identifiers of curves to project</param>
    ///<param name="surfaceIds">(Guid seq) Identifiers of surfaces to project onto</param>
    ///<param name="direction">(Vector3d) Projection direction</param>
    ///<returns>(Guid seq) list of identifiers</returns>
    static member ProjectCurveToSurface(curveIds:Guid seq, surfaceIds:Guid seq, direction:Vector3d) : Guid seq =
        failNotImpl () 


    ///<summary>Rebuilds a curve to a given degree and control point count. For more
    ///  information, see the Rhino help for the Rebuild command.</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<param name="degree">(int) Optional, Default Value: <c>3</c>
    ///New degree (must be greater than 0)</param>
    ///<param name="pointCount">(int) Optional, Default Value: <c>10</c>
    ///New point count, which must be bigger than degree.</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RebuildCurve(curveId:Guid, [<OPT;DEF(3)>]degree:int, [<OPT;DEF(10)>]pointCount:int) : bool =
        failNotImpl () 


    ///<summary>Deletes a knot from a curve object.</summary>
    ///<param name="curve">(Guid) The reference of the source object</param>
    ///<param name="parameter">(float) The parameter on the curve. Note, if the parameter is not equal to one
    ///  of the existing knots, then the knot closest to the specified parameter
    ///  will be removed.</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RemoveCurveKnot(curve:Guid, parameter:float) : bool =
        failNotImpl () 


    ///<summary>Reverses the direction of a curve object. Same as Rhino's Dir command</summary>
    ///<param name="curveId">(Guid) Identifier of the curve object</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ReverseCurve(curveId:Guid) : bool =
        failNotImpl () 


    ///<summary>Replace a curve with a geometrically equivalent polycurve.
    ///  The polycurve will have the following properties:
    ///    - All the polycurve segments are lines, polylines, arcs, or NURBS curves.
    ///    - The NURBS curves segments do not have fully multiple interior knots.
    ///    - Rational NURBS curves do not have constant weights.
    ///    - Any segment for which IsCurveLinear or IsArc is True is a line, polyline segment, or an arc.
    ///    - Adjacent co-linear or co-circular segments are combined.
    ///    - Segments that meet with G1-continuity have there ends tuned up so that they meet with G1-continuity to within machine precision.
    ///    - If the polycurve is a polyline, a polyline will be created</summary>
    ///<param name="curveId">(Guid) The object's identifier</param>
    ///<param name="flags">(int) Optional, Default Value: <c>0</c>
    ///The simplification methods to use. By default, all methods are used (flags = 0)
    ///  Value Description
    ///  0     Use all methods.
    ///  1     Do not split NURBS curves at fully multiple knots.
    ///  2     Do not replace segments with IsCurveLinear = True with line curves.
    ///  4     Do not replace segments with IsArc = True with arc curves.
    ///  8     Do not replace rational NURBS curves with constant denominator with an equivalent non-rational NURBS curve.
    ///  16    Do not adjust curves at G1-joins.
    ///  32    Do not merge adjacent co-linear lines or co-circular arcs or combine consecutive line segments into a polyline.</param>
    ///<returns>(bool) True or False</returns>
    static member SimplifyCurve(curveId:Guid, [<OPT;DEF(0)>]flags:int) : bool =
        failNotImpl () 


    ///<summary>Splits, or divides, a curve at a specified parameter. The parameter must
    ///  be in the interior of the curve's domain</summary>
    ///<param name="curveId">(Guid) The curve to split</param>
    ///<param name="parameter">(float seq) One or more parameters to split the curve at</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input curve</param>
    ///<returns>(Guid seq) list of new curves on success</returns>
    static member SplitCurve(curveId:Guid, parameter:float seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        failNotImpl () 


    ///<summary>Trims a curve by removing portions of the curve outside a specified interval</summary>
    ///<param name="curveId">(Guid) The curve to trim</param>
    ///<param name="interval">(float * float) Two numbers identifying the interval to keep. Portions of
    ///  the curve before domain[0] and after domain[1] will be removed. If the
    ///  input curve is open, the interval must be increasing. If the input
    ///  curve is closed and the interval is decreasing, then the portion of
    ///  the curve across the start and end of the curve is returned</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete the input curve. If omitted the input curve is deleted.</param>
    ///<returns>(Guid seq) identifier of the new curve on success</returns>
    static member TrimCurve(curveId:Guid, interval:float * float, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        failNotImpl () 


    ///<summary>Changes the degree of a curve object. For more information see the Rhino help file for the ChangeDegree command.</summary>
    ///<param name="objectId">(Guid) The object's identifier.</param>
    ///<param name="degree">(int) The new degree.</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member ChangeCurveDegree(objectId:Guid, degree:int) : bool =
        failNotImpl () 


    ///<summary>Creates curves between two open or closed input curves.</summary>
    ///<param name="fromCurveId">(Guid) Identifier of the first curve object.</param>
    ///<param name="toCurveId">(Guid) Identifier of the second curve object.</param>
    ///<param name="numberOfCurves">(float) Optional, Default Value: <c>1</c>
    ///The number of curves to create. The default is 1.</param>
    ///<param name="method">(float) Optional, Default Value: <c>0</c>
    ///The method for refining the output curves, where:
    ///  0: (Default) Uses the control points of the curves for matching. So the first control point of first curve is matched to first control point of the second curve.
    ///  1: Refits the output curves like using the FitCurve method.  Both the input curve and the output curve will have the same structure. The resulting curves are usually more complex than input unless input curves are compatible.
    ///  2: Input curves are divided to the specified number of points on the curve, corresponding points define new points that output curves go through. If you are making one tween curve, the method essentially does the following: divides the two curves into an equal number of points, finds the midpoint between the corresponding points on the curves, and interpolates the tween curve through those points.</param>
    ///<param name="sampleNumber">(float) Optional, Default Value: <c>10</c>
    ///The number of samples points to use if method is 2. The default is 10.</param>
    ///<returns>(Guid seq) The identifiers of the new tween objects , .</returns>
    static member AddTweenCurves(fromCurveId:Guid, toCurveId:Guid, [<OPT;DEF(1)>]numberOfCurves:float, [<OPT;DEF(0)>]method:float, [<OPT;DEF(10)>]sampleNumber:float) : Guid seq =
        failNotImpl () 


