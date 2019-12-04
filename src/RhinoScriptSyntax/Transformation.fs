namespace Rhino.Scripting

open FsEx
open System
open Rhino
open Rhino.Geometry
open FsEx.UtilMath
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
 
[<AutoOpen>]
module ExtensionsTransformation =

  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with

    [<Extension>]
    ///<summary>Verifies a matrix is the identity matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsXformIdentity(xform:Transform) : bool =
        //xform = RhinoScriptSyntax.Coercexform(xform)
        xform = Transform.Identity


    [<Extension>]
    ///<summary>Verifies a matrix is a similarity transformation. A similarity
    ///  transformation can be broken into a sequence of dialations, translations,
    ///  rotations, and reflections</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix</param>
    ///<returns>(bool) True if this transformation is an orientation preserving similarity, otherwise False</returns>
    static member IsXformSimilarity(xform:Transform) : bool =
        //xform = RhinoScriptSyntax.Coercexform(xform)
        xform.SimilarityType <> TransformSimilarityType.NotSimilarity


    [<Extension>]
    ///<summary>verifies that a matrix is a zero transformation matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member IsXformZero(xform:Transform) : bool =
        xform.IsZero4x4



    [<Extension>]
    ///<summary>Returns a change of basis transformation matrix or None on error</summary>
    ///<param name="initialPlane">(Plane) The initial plane</param>
    ///<param name="finalPlane">(Plane) The final plane</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformChangeBasis(initialPlane:Plane, finalPlane:Plane) : Transform =
        //initialPlane = RhinoScriptSyntax.Coerceplane(initialPlane)
        //finalPlane = RhinoScriptSyntax.Coerceplane(finalPlane)
        let xform = Transform.ChangeBasis(initialPlane, finalPlane)
        if not xform.IsValid then failwithf "Rhino.Scripting: XformChangeBasis failed.  initialPlane:'%A' finalPlane:'%A'" initialPlane finalPlane
        xform


    [<Extension>]
    ///<summary>Returns a change of basis transformation matrix of None on error</summary>
    ///<param name="x0">(Vector3d) X of initial basis</param>
    ///<param name="y0">(Vector3d) Y of initial basis</param>
    ///<param name="z0">(Vector3d) Z of initial basis</param>
    ///<param name="x1">(Vector3d) X of final basis</param>
    ///<param name="y1">(Vector3d) Y of final basis</param>
    ///<param name="z1">(Vector3d) Z of final basis</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformChangeBasis2( x0:Vector3d,
                                     y0:Vector3d,
                                     z0:Vector3d,
                                     x1:Vector3d,
                                     y1:Vector3d,
                                     z1:Vector3d) : Transform =
        //x0 = RhinoScriptSyntax.Coerce3dvector(x0)
        //y0 = RhinoScriptSyntax.Coerce3dvector(y0)
        //z0 = RhinoScriptSyntax.Coerce3dvector(z0)
        //x1 = RhinoScriptSyntax.Coerce3dvector(x1)
        //y1 = RhinoScriptSyntax.Coerce3dvector(y1)
        //z1 = RhinoScriptSyntax.Coerce3dvector(z1)
        let xform = Transform.ChangeBasis(x0, y0, z0, x1, y1, z1)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformChangeBasis2 failed.  x0:'%A' y0:'%A' z0:'%A' x1:'%A' y1:'%A' z1:'%A'" x0 y0 z0 x1 y1 z1
        xform


    [<Extension>]
    ///<summary>Compares two transformation matrices</summary>
    ///<param name="xform1">(Transform) First matrix to compare</param>
    ///<param name="xform2">(Transform) Second matrix to compare</param>
    ///<returns>(int) -1 if xform1 is smaller than xform2
    ///  1 if xform1 bigger than xform2
    ///  0 if xform1 = xform2</returns>
    static member XformCompare(xform1:Transform, xform2:Transform) : int =
        //xform1 = RhinoScriptSyntax.Coercexform(xform1)
        //xform2 = RhinoScriptSyntax.Coercexform(xform2)
        xform1.CompareTo(xform2)


    [<Extension>]
    ///<summary>Transform point from construction plane coordinates to world coordinates</summary>
    ///<param name="point">(Point3d) A 3D point in construction plane coordinates</param>
    ///<param name="plane">(Plane) The construction plane</param>
    ///<returns>(Point3d) A 3D point in world coordinates</returns>
    static member XformCPlaneToWorld(point:Point3d, plane:Plane) : Point3d =
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        plane.Origin + point.X*plane.XAxis + point.Y*plane.YAxis + point.Z*plane.ZAxis


    [<Extension>]
    ///<summary>Returns the determinant of a transformation matrix. If the determinant
    ///  of a transformation matrix is 0, the matrix is said to be singular. Singular
    ///  matrices do not have inverses</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix</param>
    ///<returns>(float) The determinant</returns>
    static member XformDeterminant(xform:Transform) : float =
        //xform = RhinoScriptSyntax.Coercexform(xform)
        xform.Determinant


    [<Extension>]
    ///<summary>Returns a diagonal transformation matrix. Diagonal matrices are 3x3 with
    ///  the bottom row [0, 0, 0, 1]</summary>
    ///<param name="diagonalValue">(float) The diagonal value</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformDiagonal(diagonalValue:float) : Transform =
        Transform(diagonalValue)


    [<Extension>]
    ///<summary>returns the identity transformation matrix</summary>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformIdentity() : Transform =
        Transform.Identity


    [<Extension>]
    ///<summary>Returns the inverse of a non-singular transformation matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix</param>
    ///<returns>(Transform) The inverted 4x4 transformation matrix </returns>
    static member XformInverse(xform:Transform) : Transform =
        //xform = RhinoScriptSyntax.Coercexform(xform)
        let rc, inverse = xform.TryGetInverse()
        if not rc then failwithf "Rhino.Scripting: XformInverse failed.  xform:'%A'" xform
        inverse


    [<Extension>]
    ///<summary>Creates a mirror transformation matrix</summary>
    ///<param name="mirrorPlanePoint">(Point3d) Point on the mirror plane</param>
    ///<param name="mirrorPlaneNormal">(Vector3d) A 3D vector that is normal to the mirror plane</param>
    ///<returns>(Transform) mirror Transform matrix</returns>
    static member XformMirror(mirrorPlanePoint:Point3d, mirrorPlaneNormal:Vector3d) : Transform =
        //point = RhinoScriptSyntax.Coerce3dpoint(mirrorPlanePoint)
        //normal = RhinoScriptSyntax.Coerce3dvector(mirrorPlaneNormal)
        Transform.Mirror(mirrorPlanePoint, mirrorPlaneNormal)


    [<Extension>]
    ///<summary>Multiplies two transformation matrices, where result = xform1 * xform2</summary>
    ///<param name="xform1">(Transform) List or Rhino.Geometry.Transform.  The first 4x4 transformation matrix to multiply</param>
    ///<param name="xform2">(Transform) List or Rhino.Geometry.Transform.  The second 4x4 transformation matrix to multiply</param>
    ///<returns>(Transform) result transformation on success</returns>
    static member XformMultiply(xform1:Transform, xform2:Transform) : Transform =
        //xform1 = RhinoScriptSyntax.Coercexform(xform1)
        //xform2 = RhinoScriptSyntax.Coercexform(xform2)
        xform1*xform2


    [<Extension>]
    ///<summary>Returns a transformation matrix that projects to a plane</summary>
    ///<param name="plane">(Plane) The plane to project to</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformPlanarProjection(plane:Plane) : Transform =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        Transform.PlanarProjection(plane)


    [<Extension>]
    ///<summary>Returns a rotation transformation that maps initialPlane to finalPlane.
    ///  The planes should be right hand orthonormal planes</summary>
    ///<param name="initialPlane">(Plane) Plane to rotate from</param>
    ///<param name="finalPlane">(Plane) Plane to rotate to</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformRotation1(initialPlane:Plane, finalPlane:Plane) : Transform =
        //initialPlane = RhinoScriptSyntax.Coerceplane(initialPlane)
        //finalPlane = RhinoScriptSyntax.Coerceplane(finalPlane)
        let xform = Transform.PlaneToPlane(initialPlane, finalPlane)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformRotation1 failed.  initialPlane:'%A' finalPlane:'%A'" initialPlane finalPlane
        xform


    [<Extension>]
    ///<summary>Returns a rotation transformation around an axis</summary>
    ///<param name="angleDegrees">(float) Rotation angle in degrees</param>
    ///<param name="rotationAxis">(Vector3d) Rotation axis</param>
    ///<param name="centerPoint">(Point3d) Rotation center</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformRotation2( angleDegrees:float,
                                  rotationAxis:Vector3d,
                                  centerPoint:Point3d) : Transform =
        //axis = RhinoScriptSyntax.Coerce3dvector(rotationAxis)
        //center = RhinoScriptSyntax.Coerce3dpoint(centerPoint)
        let anglerad = toRadians(angleDegrees)
        let xform = Transform.Rotation(anglerad, rotationAxis, centerPoint)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformRotation2 failed.  angleDegrees:'%A' rotationAxis:'%A' centerPoint:'%A'" angleDegrees rotationAxis centerPoint
        xform


    [<Extension>]
    ///<summary>Calculate the minimal transformation that rotates startDirection to
    ///  endDirection while fixing centerPoint</summary>
    ///<param name="startDirection">(Vector3d) Start direction</param>
    ///<param name="endDirection">(Vector3d) End direction</param>
    ///<param name="centerPoint">(Point3d) The rotation center</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformRotation3( startDirection:Vector3d,
                                  endDirection:Vector3d,
                                  centerPoint:Point3d) : Transform =
        //start = RhinoScriptSyntax.Coerce3dvector(startDirection)
        //end = RhinoScriptSyntax.Coerce3dvector(endDirection)
        //center = RhinoScriptSyntax.Coerce3dpoint(centerPoint)
        let xform = Transform.Rotation(startDirection, endDirection, centerPoint)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformRotation3 failed.  startDirection:'%A' endDirection:'%A' centerPoint:'%A'" startDirection endDirection centerPoint
        xform


    [<Extension>]
    ///<summary>Returns a rotation transformation</summary>
    ///<param name="x0">(Vector3d) X of Vector defining the initial orthonormal frame</param>
    ///<param name="y0">(Vector3d) Y of Vector defining the initial orthonormal frame</param>
    ///<param name="z0">(Vector3d) Z of Vector defining the initial orthonormal frame</param>
    ///<param name="x1">(Vector3d) X of Vector defining the final orthonormal frame</param>
    ///<param name="y1">(Vector3d) Y of Vector defining the final orthonormal frame</param>
    ///<param name="z1">(Vector3d) Z of Vector defining the final orthonormal frame</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformRotation4( x0:Vector3d,
                                  y0:Vector3d,
                                  z0:Vector3d,
                                  x1:Vector3d,
                                  y1:Vector3d,
                                  z1:Vector3d) : Transform =
        //x0 = RhinoScriptSyntax.Coerce3dvector(x0)
        //y0 = RhinoScriptSyntax.Coerce3dvector(y0)
        //z0 = RhinoScriptSyntax.Coerce3dvector(z0)
        //x1 = RhinoScriptSyntax.Coerce3dvector(x1)
        //y1 = RhinoScriptSyntax.Coerce3dvector(y1)
        //z1 = RhinoScriptSyntax.Coerce3dvector(z1)
        let xform = Transform.Rotation(x0, y0, z0, x1, y1, z1)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformRotation4 failed.  x0:'%A' y0:'%A' z0:'%A' x1:'%A' y1:'%A' z1:'%A'" x0 y0 z0 x1 y1 z1
        xform


    [<Extension>]
    ///<summary>Creates a scale transformation</summary>
    ///<param name="scale">(float*float*float) list of 3 numbers for x , y andf z direction</param>
    ///<param name="point">(Point3d) Optional, Center of scale. If omitted, world origin is used</param>
    ///<returns>(Transform) The 4x4 transformation matrix on success</returns>
    static member XformScale(scale:float*float*float, [<OPT;DEF(Point3d())>]point:Point3d) : Transform =
        let plane = Plane(point, Vector3d.ZAxis);
        let xf = Transform.Scale(plane, scale|> t1, scale|> t2, t3 scale)
        xf


    [<Extension>]
    ///<summary>Transforms a point from either client-area coordinates of the specified view
    ///  or screen coordinates to world coordinates. The resulting coordinates are represented
    ///  as a 3-D point</summary>
    ///<param name="point">(Point3d) 2D point</param>
    ///<param name="view">(string) Optional, Title of a view. If omitted, the active view is used</param>
    ///<param name="screenCoordinates">(bool) Optional, Default Value: <c>false</c>
    ///If False, point is in client-area coordinates. If True,
    ///  point is in screen-area coordinates</param>
    ///<returns>(Point3d) on success</returns>
    static member XformScreenToWorld( point:Point3d,
                                      [<OPT;DEF(null:string)>]view:string,
                                      [<OPT;DEF(false)>]screenCoordinates:bool) : Point3d =
        //point = RhinoScriptSyntax.Coerce2dpoint(point)
        let view = RhinoScriptSyntax.CoerceView(view |? "") // ""to get active view
        let viewport = view.MainViewport
        let xform = viewport.GetTransform(DocObjects.CoordinateSystem.Screen, DocObjects.CoordinateSystem.World)
        let mutable point3d = Point3d(point.X, point.Y, 0.0)
        if  screenCoordinates then
            let screen = view.ScreenRectangle
            point3d.X <- point.X - (float screen.Left) //TODO check if correct?
            point3d.Y <- point.Y - (float screen.Top)
        point3d <- xform * point3d
        point3d


    [<Extension>]
    ///<summary>Returns a shear transformation matrix</summary>
    ///<param name="plane">(Plane) Plane.Origin is the fixed point</param>
    ///<param name="x">(Vector3d) X axis scale vecto</param>
    ///<param name="y">(Vector3d) Y axis scale vecto</param>
    ///<param name="z">(Vector3d) Z axis scale vecto</param>
    ///<returns>(Transform) The 4x4 transformation matrix on success</returns>
    static member XformShear( plane:Plane,
                              x:Vector3d,
                              y:Vector3d,
                              z:Vector3d) : Transform =
        Transform.Shear(plane, x, y, z)


    [<Extension>]
    ///<summary>Creates a translation transformation matrix</summary>
    ///<param name="vector">(Vector3d) List of 3 numbers, Point3d, or Vector3d.  A 3-D translation vector</param>
    ///<returns>(Transform) The 4x4 transformation matrix is successful, otherwise None</returns>
    static member XformTranslation(vector:Vector3d) : Transform =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        Transform.Translation(vector)


    [<Extension>]
    ///<summary>Transforms a point from world coordinates to construction plane coordinates</summary>
    ///<param name="point">(Point3d) A 3D point in world coordinates</param>
    ///<param name="plane">(Plane) The construction plane</param>
    ///<returns>(Point3d) 3D point in construction plane coordinates</returns>
    static member XformWorldToCPlane(point:Point3d, plane:Plane) : Point3d =
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        let v = point - plane.Origin;
        Point3d(v*plane.XAxis, v*plane.YAxis, v*plane.ZAxis)


    [<Extension>]
    ///<summary>Transforms a point from world coordinates to either client-area coordinates of
    ///  the specified view or screen coordinates. The resulting coordinates are represented
    ///  as a 2D point</summary>
    ///<param name="point">(Point3d) 3D point in world coordinates</param>
    ///<param name="view">(string) Optional, Title of a view. If omitted, the active view is used</param>
    ///<param name="screenCoordinates">(bool) Optional, Default Value: <c>false</c>
    ///If False, the function returns the results as
    ///  client-area coordinates. If True, the result is in screen-area coordinates</param>
    ///<returns>(Point2d) 2D point on success</returns>
    static member XformWorldToScreen( point:Point3d,
                                      [<OPT;DEF(null:string)>]view:string,
                                      [<OPT;DEF(false)>]screenCoordinates:bool) : Point2d =
        let view = RhinoScriptSyntax.CoerceView(view |? "")// to get active view
        let viewport = view.MainViewport
        let xform = viewport.GetTransform(DocObjects.CoordinateSystem.World, DocObjects.CoordinateSystem.Screen)
        let mutable point3 = xform * point
        let mutable point = Point2d(point3.X, point3.Y)
        if  screenCoordinates then
            let screen = view.ScreenRectangle
            point.X <- point.X + (float screen.Left)
            point.Y <- point.Y + (float screen.Top)
        point


    [<Extension>]
    ///<summary>Returns a zero transformation matrix</summary>
    ///<returns>(Transform) a zero transformation matrix</returns>
    static member XformZero() : Transform =
        Transform()


