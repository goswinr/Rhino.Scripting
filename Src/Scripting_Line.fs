namespace Rhino.Scripting

open Rhino

open System

open Rhino.Geometry


[<AutoOpen>]
module AutoOpenLine =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    /// <summary>Finds the point on a finite line that is closest to a test point.</summary>
    /// <param name="line">(Geometry.Line) The finite line</param>
    /// <param name="testPoint">(Point3d) List of 3 numbers or Point3d. The test point</param>
    /// <returns>(Point3d) The point on the finite line that is closest to the test point.</returns>
    static member LineClosestPointFinite(line:Line, testPoint:Point3d) : Point3d =
        line.ClosestPoint(testPoint, limitToFiniteSegment=true)



    /// <summary>Finds the point on an infinite line (ray) that is closest to a test point.</summary>
    /// <param name="line">(Geometry.Line) The line to be considered infinite</param>
    /// <param name="testPoint">(Point3d) The test point</param>
    /// <returns>(Point3d) The point on the infinite line (ray) that is closest to the test point.</returns>
    static member LineClosestPoint(line:Line, testPoint:Point3d) : Point3d =
        line.ClosestPoint(testPoint, limitToFiniteSegment=false)



    /// <summary>Calculates the intersection of a line and a cylinder.</summary>
    /// <param name="line">(Geometry.Line) The line to intersect</param>
    /// <param name="cylinderPlane">(Plane) Base plane of the cylinder</param>
    /// <param name="cylinderHeight">(float) Height of the cylinder</param>
    /// <param name="cylinderRadius">(float) Radius of the cylinder</param>
    /// <returns>(Point3d array) List of intersection points (0, 1, or 2 points).</returns>
    static member LineCylinderIntersection(line:Line, cylinderPlane:Plane, cylinderHeight:float, cylinderRadius:float) : Point3d array =
        let circle = Geometry.Circle( cylinderPlane, cylinderRadius )
        if not <| circle.IsValid then  RhinoScriptingException.Raise "RhinoScriptSyntax.LineCylinderIntersection: Unable to create valid circle with given plane && radius.  line:'%A' cylinderPlane:'%A' cylinderHeight:'%A' cylinderRadius:'%A'" line cylinderPlane cylinderHeight cylinderRadius
        let cyl = Geometry.Cylinder( circle, cylinderHeight )
        if not <| cyl.IsValid then  RhinoScriptingException.Raise "RhinoScriptSyntax.LineCylinderIntersection: Unable to create valid cylinder with given circle && height.  line:'%A' cylinderPlane:'%A' cylinderHeight:'%A' cylinderRadius:'%A'" line cylinderPlane cylinderHeight cylinderRadius
        let rc, pt1, pt2 = Intersect.Intersection.LineCylinder(line, cyl)
        if rc= Intersect.LineCylinderIntersection.None then
            [| |]
        elif rc= Intersect.LineCylinderIntersection.Single then
            [|pt1|]
        else
            [|pt1; pt2|]




    /// <summary>Determines if the shortest distance from a line to a point or another
    ///    line is greater than a specified distance.</summary>
    /// <param name="line">(Geometry.Line) A Geometry.Line</param>
    /// <param name="distance">(float) The distance</param>
    /// <param name="point">(Point3d) The test point</param>
    /// <returns>(bool) True if the shortest distance from the line to the other object is
    ///    greater than distance, False otherwise.</returns>
    static member LineIsFartherThan(line:Line, distance:float, point:Point3d) : bool =
        let minDist = line.MinimumDistanceTo(point)
        minDist > distance
    /// <summary>Determines if the shortest distance from a line to a point or another
    ///    line is greater than a specified distance.</summary>
    /// <param name="line">(Geometry.Line) a Geometry.Line</param>
    /// <param name="distance">(float) The distance</param>
    /// <param name="line2">(Geometry.Line) The test line</param>
    /// <returns>(bool) True if the shortest distance from the line to the other project is
    ///    greater than distance, False otherwise.</returns>
    static member LineIsFartherThan(line:Line, distance:float, line2:Line) : bool =
        let minDist = line.MinimumDistanceTo(line2)
        minDist > distance



    /// <summary>Calculates the intersection of two non-parallel lines. The lines are considered endless.
    /// If the two lines do not actually intersect the closest point on each is returned.
    /// Fails on parallel or collinear lines</summary>
    /// <param name="lineA">(Geometry.Line) LineA of lines to intersect</param>
    /// <param name="lineB">(Geometry.Line) LineB of lines to intersect</param>
    /// <returns>(Point3d * Point3d) containing a point on the first line and a point on the second line.</returns>
    static member LineLineIntersection(lineA:Line, lineB:Line) : Point3d * Point3d =
        let rc, a, b = Intersect.Intersection.LineLine(lineA, lineB)
        if not <| rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.LineLineIntersection failed on lineA:%A lineB:%A , are they parallel?" lineA lineB
        lineA.PointAt(a), lineB.PointAt(b)

    /// <summary>Finds the longest distance between a line as a finite chord, and a point.</summary>
    /// <param name="line">(Geometry.Line) Line</param>
    /// <param name="point">(Point3d) The test point or test line</param>
    /// <returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object,
    /// then D is bigger than Rhino.Distance(Q, P).</returns>
    static member LineMaxDistanceTo(line:Line, point:Point3d) : float =
        line.MaximumDistanceTo(point)
    /// <summary>Finds the longest distance between a line as a finite chord, and a line.</summary>
    /// <param name="line">(Geometry.Line) Line</param>
    /// <param name="line2">(Geometry.Line) The test line</param>
    /// <returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object,
    /// then D is bigger than Rhino.Distance(Q, P).</returns>
    static member LineMaxDistanceTo(line:Line, line2:Line) : float =
        line.MaximumDistanceTo(line2)


    /// <summary>Finds the shortest distance between a line as a finite chord, and a point or another line.</summary>
    /// <param name="line">(Geometry.Line) Line</param>
    /// <param name="point">(Point3d) The test point</param>
    /// <returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object,
    /// then D is smaller than Rhino.Distance(Q, P).</returns>
    static member LineMinDistanceTo(line:Line, point:Point3d) : float =
        line.MinimumDistanceTo(point)

    /// <summary>Finds the shortest distance between a line as a finite chord, and a point or another line.</summary>
    /// <param name="line">(Geometry.Line) Line</param>
    /// <param name="line2">(Geometry.Line) The test line</param>
    /// <returns>(float) A distance (D) such that if Q is any point on the line and P is any point on the other object,
    /// then D is smaller than Rhino.Distance(Q, P).</returns>
    static member LineMinDistanceTo(line:Line, line2:Line) : float =
        line.MinimumDistanceTo(line2)



    /// <summary>Returns a Plane that contains the line. The origin of the Plane is at the start of
    ///    the line. If possible, a Plane parallel to the world XY, YZ, or ZX Plane is returned.</summary>
    /// <param name="line">(Geometry.Line) a Line</param>
    /// <returns>(Plane) The Plane.</returns>
    static member LinePlane(line:Line) : Plane =
        let rc, plane = line.TryGetPlane()
        if not <| rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.LinePlane failed.  line:'%A'" line
        plane


    /// <summary>Calculates the intersection of a line and a Plane.</summary>
    /// <param name="line">(Line) The line to intersect</param>
    /// <param name="plane">(Plane) The Plane to intersect</param>
    /// <returns>(Point3d) The 3D point of intersection is successful.</returns>
    static member LinePlaneIntersection(line:Line, plane:Plane) : Point3d =
        let rc, t = Intersect.Intersection.LinePlane(line, plane)
        if  not <| rc then  RhinoScriptingException.Raise "RhinoScriptSyntax.LinePlaneIntersection failed. Parallel? line:'%A' plane:'%A'" line plane
        line.PointAt(t)


    /// <summary>Calculates the intersection of a line and a sphere.</summary>
    /// <param name="line">(Geometry.Line) The line</param>
    /// <param name="sphereCenter">(Point3d) The center point of the sphere</param>
    /// <param name="sphereRadius">(float) The radius of the sphere</param>
    /// <returns>(Point3d array) list of intersection points.</returns>
    static member LineSphereIntersection(line:Line, sphereCenter:Point3d, sphereRadius:float) : Point3d array =
        let sphere = Sphere(sphereCenter, sphereRadius)
        let rc, pt1, pt2 = Intersect.Intersection.LineSphere(line, sphere)
        if rc= Intersect.LineSphereIntersection.None then  [||]
        elif rc= Intersect.LineSphereIntersection.Single then  [|pt1|]
        else [|pt1; pt2|]


    /// <summary>Transforms a line.</summary>
    /// <param name="lineId">(Guid) The line to transform</param>
    /// <param name="xForm">(Transform) The transformation to apply</param>
    /// <param name="copy">(bool) Optional, default value: <c>false</c>. Copy the Line object</param>
    /// <returns>(Guid) The same Guid, or a new Guid if copy=true.</returns>
    static member LineTransform(    lineId:Guid,
                                    xForm:Transform,
                                    [<OPT;DEF(false)>]copy:bool)  : Guid =

        // the original python  implementation has a bug, does not return Guid: https://github.com/mcneel/rhinoscriptsyntax/pull/204
        let line = RhinoScriptSyntax.CoerceLine lineId
        if copy then
            let ln = Line(line.From,line.To)
            let success = ln.Transform(xForm)
            if not <| success then  RhinoScriptingException.Raise "RhinoScriptSyntax.LineTransform unable to transform line %A with  %A" line xForm
            State.Ot.AddLine(ln)
        else
            // if not <| State.Ot.Replace(lineId,ln) then  RhinoScriptingException.Raise "RhinoScriptSyntax.LineTransform unable to replace geometry: line %A with  %A" line xForm
            let success =line.Transform(xForm)
            if not <| success then  RhinoScriptingException.Raise "RhinoScriptSyntax.LineTransform unable to transform line %A with  %A" line xForm
            lineId


