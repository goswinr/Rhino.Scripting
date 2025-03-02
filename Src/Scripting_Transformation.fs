
namespace Rhino.Scripting

open Rhino

open System

open Rhino.Geometry




[<AutoOpen>]
module AutoOpenTransformation =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>Verifies a matrix is the identity matrix.</summary>
    ///<param name="xForm">(Transform) Rhino.Geometry.Transform. A 4x4 transformation matrix</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsXformIdentity(xForm:Transform) : bool =
        xForm = Transform.Identity


    ///<summary>Verifies a matrix is a similarity transformation. A similarity
    ///    transformation can be broken into a sequence of dilatations, translations,
    ///    rotations, and reflections.</summary>
    ///<param name="xForm">(Transform) Rhino.Geometry.Transform. A 4x4 transformation matrix</param>
    ///<returns>(bool) True if this transformation is an orientation preserving similarity, otherwise False.</returns>
    static member IsXformSimilarity(xForm:Transform) : bool =
        xForm.SimilarityType <> TransformSimilarityType.NotSimilarity


    ///<summary>Checks if a matrix is a zero transformation matrix.</summary>
    ///<param name="xForm">(Transform) Rhino.Geometry.Transform. A 4x4 transformation matrix</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsXformZero(xForm:Transform) : bool =
        xForm.IsZero4x4



    ///<summary>Returns a change of basis transformation matrix or None on error.</summary>
    ///<param name="initialPlane">(Plane) The initial Plane</param>
    ///<param name="finalPlane">(Plane) The final Plane</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformChangeBasis(initialPlane:Plane, finalPlane:Plane) : Transform =
        let xForm = Transform.ChangeBasis(initialPlane, finalPlane)
        if not xForm.IsValid then RhinoScriptingException.Raise "RhinoScriptSyntax.XformChangeBasis failed.  initialPlane:'%A' finalPlane:'%A'" initialPlane finalPlane
        xForm


    ///<summary>Returns a change of basis transformation matrix of None on error.</summary>
    ///<param name="x0">(Vector3d) X of initial basis</param>
    ///<param name="y0">(Vector3d) Y of initial basis</param>
    ///<param name="z0">(Vector3d) Z of initial basis</param>
    ///<param name="x1">(Vector3d) X of final basis</param>
    ///<param name="y1">(Vector3d) Y of final basis</param>
    ///<param name="z1">(Vector3d) Z of final basis</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformChangeBasis2( x0:Vector3d,
                                     y0:Vector3d,
                                     z0:Vector3d,
                                     x1:Vector3d,
                                     y1:Vector3d,
                                     z1:Vector3d) : Transform =
        let xForm = Transform.ChangeBasis(x0, y0, z0, x1, y1, z1)
        if not xForm.IsValid   then RhinoScriptingException.Raise "RhinoScriptSyntax.XformChangeBasis2 failed.  x0:'%A' y0:'%A' z0:'%A' x1:'%A' y1:'%A' z1:'%A'" x0 y0 z0 x1 y1 z1
        xForm


    ///<summary>Compares two transformation matrices.</summary>
    ///<param name="xForm1">(Transform) First matrix to compare</param>
    ///<param name="xForm2">(Transform) Second matrix to compare</param>
    ///<returns>(int) -1 if xForm1 is smaller than xForm2
    ///    1 if xForm1 bigger than xForm2
    ///    0 if xForm1 = xForm2.</returns>
    static member XformCompare(xForm1:Transform, xForm2:Transform) : int =
        xForm1.CompareTo(xForm2)


    ///<summary>Transform point from construction Plane coordinates to world coordinates.</summary>
    ///<param name="point">(Point3d) A 3D point in construction Plane coordinates</param>
    ///<param name="plane">(Plane) The construction Plane</param>
    ///<returns>(Point3d) A 3D point in world coordinates.</returns>
    static member XformCPlaneToWorld(point:Point3d, plane:Plane) : Point3d =
        plane.Origin + point.X*plane.XAxis + point.Y*plane.YAxis + point.Z*plane.ZAxis


    ///<summary>Returns the determinant of a transformation matrix. If the determinant
    ///    of a transformation matrix is 0, the matrix is said to be singular. Singular
    ///    matrices do not have inverses.</summary>
    ///<param name="xForm">(Transform) Rhino.Geometry.Transform. A 4x4 transformation matrix</param>
    ///<returns>(float) The determinant.</returns>
    static member XformDeterminant(xForm:Transform) : float =
        xForm.Determinant


    ///<summary>Returns a diagonal transformation matrix. Diagonal matrices are 3x3 with
    ///    the bottom row [0, 0, 0, 1].</summary>
    ///<param name="diagonalValue">(float) The diagonal value</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformDiagonal(diagonalValue:float) : Transform =
        Transform(diagonalValue)


    ///<summary>returns the identity transformation matrix.</summary>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformIdentity() : Transform =
        Transform.Identity


    ///<summary>Returns the inverse of a non-singular transformation matrix.</summary>
    ///<param name="xForm">(Transform) Rhino.Geometry.Transform. A 4x4 transformation matrix</param>
    ///<returns>(Transform) The inverted 4x4 transformation matrix.</returns>
    static member XformInverse(xForm:Transform) : Transform =
        let rc, inverse = xForm.TryGetInverse()
        if not rc then RhinoScriptingException.Raise "RhinoScriptSyntax.XformInverse failed.  xForm:'%A'" xForm
        inverse


    ///<summary>Creates a mirror transformation matrix.</summary>
    ///<param name="mirrorPlanePoint">(Point3d) Point on the mirror Plane</param>
    ///<param name="mirrorPlaneNormal">(Vector3d) A 3D vector that is normal to the mirror Plane</param>
    ///<returns>(Transform) mirror Transform matrix.</returns>
    static member XformMirror(mirrorPlanePoint:Point3d, mirrorPlaneNormal:Vector3d) : Transform =
        Transform.Mirror(mirrorPlanePoint, mirrorPlaneNormal)


    ///<summary>Multiplies two transformation matrices, where result = xForm1 * xForm2.</summary>
    ///<param name="xForm1">(Transform) Rhino.Geometry.Transform. The first 4x4 transformation matrix to multiply</param>
    ///<param name="xForm2">(Transform) Rhino.Geometry.Transform. The second 4x4 transformation matrix to multiply</param>
    ///<returns>(Transform) result transformation.</returns>
    static member XformMultiply(xForm1:Transform, xForm2:Transform) : Transform =
        xForm1*xForm2


    ///<summary>Returns a transformation matrix that projects to a Plane.</summary>
    ///<param name="plane">(Plane) The Plane to project to</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformPlanarProjection(plane:Plane) : Transform =
        Transform.PlanarProjection(plane)


    ///<summary>Returns a rotation transformation that maps initialPlane to finalPlane.
    ///    The Planes should be right hand orthonormal Planes.</summary>
    ///<param name="initialPlane">(Plane) Plane to rotate from</param>
    ///<param name="finalPlane">(Plane) Plane to rotate to</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation1(initialPlane:Plane, finalPlane:Plane) : Transform =
        let xForm = Transform.PlaneToPlane(initialPlane, finalPlane)
        if not xForm.IsValid   then RhinoScriptingException.Raise "RhinoScriptSyntax.XformRotation1 failed.  initialPlane:'%A' finalPlane:'%A'" initialPlane finalPlane
        xForm


    ///<summary>Returns a rotation transformation around an axis.</summary>
    ///<param name="angleDegrees">(float) Rotation angle in degrees</param>
    ///<param name="rotationAxis">(Vector3d) Rotation axis</param>
    ///<param name="centerPoint">(Point3d) Rotation center</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation2( angleDegrees:float,
                                  rotationAxis:Vector3d,
                                  centerPoint:Point3d) : Transform =
        let anglerad = toRadians(angleDegrees)
        let xForm = Transform.Rotation(anglerad, rotationAxis, centerPoint)
        if not xForm.IsValid   then RhinoScriptingException.Raise "RhinoScriptSyntax.XformRotation2 failed.  angleDegrees:'%A' rotationAxis:'%A' centerPoint:'%A'" angleDegrees rotationAxis centerPoint
        xForm


    ///<summary>Calculate the minimal transformation that rotates startDirection to
    ///    endDirection while fixing centerPoint.</summary>
    ///<param name="startDirection">(Vector3d) Start direction</param>
    ///<param name="endDirection">(Vector3d) End direction</param>
    ///<param name="centerPoint">(Point3d) The rotation center</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation3( startDirection:Vector3d,
                                  endDirection:Vector3d,
                                  centerPoint:Point3d) : Transform =
        let xForm = Transform.Rotation(startDirection, endDirection, centerPoint)
        if not xForm.IsValid   then RhinoScriptingException.Raise "RhinoScriptSyntax.XformRotation3 failed.  startDirection:'%A' endDirection:'%A' centerPoint:'%A'" startDirection endDirection centerPoint
        xForm


    ///<summary>Returns a rotation transformation.</summary>
    ///<param name="x0">(Vector3d) X of Vector defining the initial orthonormal frame</param>
    ///<param name="y0">(Vector3d) Y of Vector defining the initial orthonormal frame</param>
    ///<param name="z0">(Vector3d) Z of Vector defining the initial orthonormal frame</param>
    ///<param name="x1">(Vector3d) X of Vector defining the final orthonormal frame</param>
    ///<param name="y1">(Vector3d) Y of Vector defining the final orthonormal frame</param>
    ///<param name="z1">(Vector3d) Z of Vector defining the final orthonormal frame</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation4( x0:Vector3d,
                                  y0:Vector3d,
                                  z0:Vector3d,
                                  x1:Vector3d,
                                  y1:Vector3d,
                                  z1:Vector3d) : Transform =
        let xForm = Transform.Rotation(x0, y0, z0, x1, y1, z1)
        if not xForm.IsValid   then RhinoScriptingException.Raise "RhinoScriptSyntax.XformRotation4 failed.  x0:'%A' y0:'%A' z0:'%A' x1:'%A' y1:'%A' z1:'%A'" x0 y0 z0 x1 y1 z1
        xForm


    ///<summary>Creates a scale transformation.</summary>
    ///<param name="scaleX">(float) Scale in X direction</param>
    ///<param name="scaleY">(float) Scale in Y direction</param>
    ///<param name="scaleZ">(float) Scale in Z direction</param>
    ///<param name="point">(Point3d) Center of scale</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformScale(scaleX, scaleY, scaleZ, point:Point3d) : Transform =
        let plane = Plane(point, Vector3d.ZAxis)
        Transform.Scale(plane, scaleX, scaleY, scaleZ)

    ///<summary>Creates a scale transformation based on World Origin point.</summary>
    ///<param name="scale">(float) Scale in X , Y and Z direction</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformScale(scale) : Transform =
        Transform.Scale(Plane.WorldXY, scale, scale, scale)

    ///<summary>Creates a scale transformation.</summary>
    ///<param name="scale">(float) Scale in X , Y and Z direction</param>
    ///<param name="point">(Point3d) Center of scale</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformScale(scale, point:Point3d) : Transform =
        let plane = Plane(point, Vector3d.ZAxis)
        Transform.Scale(plane, scale, scale, scale)


    ///<summary>Transforms a point from either client-area coordinates of the specified view
    ///    or screen coordinates to world coordinates. The resulting coordinates are represented
    ///    as a 3-D point.</summary>
    ///<param name="point">(Point3d) 2D point</param>
    ///<param name="view">(string) Optional, Title of a view. If omitted, the active view is used</param>
    ///<param name="screenCoordinates">(bool) Optional, default value: <c>false</c>
    ///    If False, point is in client-area coordinates. If True,
    ///    point is in screen-area coordinates</param>
    ///<returns>(Point3d) The transformedPoint.</returns>
    static member XformScreenToWorld( point:Point3d,
                                      [<OPT;DEF(null:string)>]view:string,
                                      [<OPT;DEF(false)>]screenCoordinates:bool) : Point3d =
        let view = RhinoScriptSyntax.CoerceView(view |? "") // ""to get active view
        let viewport = view.MainViewport
        let xForm = viewport.GetTransform(DocObjects.CoordinateSystem.Screen, DocObjects.CoordinateSystem.World)
        let mutable point3d = Point3d(point.X, point.Y, 0.0)
        if  screenCoordinates then
            let screen = view.ScreenRectangle
            point3d.X <- point.X - (float screen.Left) //TODO check if correct?
            point3d.Y <- point.Y - (float screen.Top)
        point3d <- xForm * point3d
        point3d


    ///<summary>Returns a shear transformation matrix.</summary>
    ///<param name="plane">(Plane) Plane.Origin is the fixed point</param>
    ///<param name="x">(Vector3d) X axis scale vector</param>
    ///<param name="y">(Vector3d) Y axis scale vector</param>
    ///<param name="z">(Vector3d) Z axis scale vector</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformShear( plane:Plane,
                              x:Vector3d,
                              y:Vector3d,
                              z:Vector3d) : Transform =
        Transform.Shear(plane, x, y, z)


    ///<summary>Creates a translation transformation matrix.</summary>
    ///<param name="vector">(Vector3d) List of 3 numbers, Point3d, or Vector3d. A 3-D translation vector</param>
    ///<returns>(Transform) The 4x4 transformation matrix if successful.</returns>
    static member XformTranslation(vector:Vector3d) : Transform =
        Transform.Translation(vector)


    ///<summary>Transforms a point from world coordinates to construction Plane coordinates.</summary>
    ///<param name="point">(Point3d) A 3D point in world coordinates</param>
    ///<param name="plane">(Plane) The construction Plane</param>
    ///<returns>(Point3d) 3D point in construction Plane coordinates.</returns>
    static member XformWorldToCPlane(point:Point3d, plane:Plane) : Point3d =
        let v = point - plane.Origin;
        Point3d(v*plane.XAxis, v*plane.YAxis, v*plane.ZAxis)


    ///<summary>Transforms a point from world coordinates to either client-area coordinates of
    ///    the specified view or screen coordinates. The resulting coordinates are represented
    ///    as a 2D point.</summary>
    ///<param name="point">(Point3d) 3D point in world coordinates</param>
    ///<param name="view">(string) Optional, Title of a view. If omitted, the active view is used</param>
    ///<param name="screenCoordinates">(bool) Optional, default value: <c>false</c>
    ///    If False, the function returns the results as
    ///    client-area coordinates. If True, the result is in screen-area coordinates</param>
    ///<returns>(Point2d) 2D point.</returns>
    static member XformWorldToScreen( point:Point3d,
                                      [<OPT;DEF(null:string)>]view:string,
                                      [<OPT;DEF(false)>]screenCoordinates:bool) : Point2d =
        let view = RhinoScriptSyntax.CoerceView(view |? "")// to get active view
        let viewport = view.MainViewport
        let xForm = viewport.GetTransform(DocObjects.CoordinateSystem.World, DocObjects.CoordinateSystem.Screen)
        let mutable point3 = xForm * point
        let mutable point = Point2d(point3.X, point3.Y)
        if  screenCoordinates then
            let screen = view.ScreenRectangle
            point.X <- point.X + (float screen.Left)
            point.Y <- point.Y + (float screen.Top)
        point


    ///<summary>Returns a zero transformation matrix.</summary>
    ///<returns>(Transform) a zero transformation matrix.</returns>
    static member XformZero() : Transform =
        Transform()



