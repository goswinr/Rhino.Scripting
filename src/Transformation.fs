namespace Rhino.Scripting

open System
open Rhino.Geometry
//open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open Rhino.Scripting.Util
open Rhino.Scripting.ActiceDocument

[<AutoOpen>]
module ExtensionsTransformation =
  type RhinoScriptSyntax with
    ///<summary>Verifies a matrix is the identity matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsXformIdentity(xform:Transform) : bool =
        failNotImpl()

    ///<summary>Verifies a matrix is a similarity transformation. A similarity
    ///  transformation can be broken into a sequence of dialations, translations,
    ///  rotations, and reflections</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(bool) True if this transformation is an orientation preserving similarity, otherwise False.</returns>
    static member IsXformSimilarity(xform:Transform) : bool =
        failNotImpl()

    ///<summary>verifies that a matrix is a zero transformation matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsXformZero(xform:Transform) : bool =
        failNotImpl()

    ///<summary>Returns a change of basis transformation matrix or None on error</summary>
    ///<param name="initialPlane">(Plane) The initial plane</param>
    ///<param name="finalPlane">(Plane) The final plane</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformChangeBasis(initialPlane:Plane, finalPlane:Plane) : Transform =
        failNotImpl()

    ///<summary>Returns a change of basis transformation matrix of None on error</summary>
    ///<param name="x0">(Vector3d) X0 of 'initial basis' (FIXME 0)</param>
    ///<param name="y0">(Vector3d) Y0 of 'initial basis' (FIXME 0)</param>
    ///<param name="z0">(Vector3d) Z0 of 'initial basis' (FIXME 0)</param>
    ///<param name="x1">(Vector3d) X1 of 'final basis' (FIXME 0)</param>
    ///<param name="y1">(Vector3d) Y1 of 'final basis' (FIXME 0)</param>
    ///<param name="z1">(Vector3d) Z1 of 'final basis' (FIXME 0)</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformChangeBasis2(x0:Vector3d, y0:Vector3d, z0:Vector3d, x1:Vector3d, y1:Vector3d, z1:Vector3d) : Transform =
        failNotImpl()

    ///<summary>Compares two transformation matrices</summary>
    ///<param name="xform1">(Transform) First matrix to compare</param>
    ///<param name="xform2">(Transform) Second matrix to compare</param>
    ///<returns>(int) -1 if xform1<xform2
    ///  1 if xform1>xform2
    ///  0 if xform1=xform2</returns>
    static member XformCompare(xform1:Transform, xform2:Transform) : int =
        failNotImpl()

    ///<summary>Transform point from construction plane coordinates to world coordinates</summary>
    ///<param name="point">(Point3d) A 3D point in construction plane coordinates.</param>
    ///<param name="plane">(Plane) The construction plane</param>
    ///<returns>(Point3d) A 3D point in world coordinates</returns>
    static member XformCPlaneToWorld(point:Point3d, plane:Plane) : Point3d =
        failNotImpl()

    ///<summary>Returns the determinant of a transformation matrix. If the determinant
    ///  of a transformation matrix is 0, the matrix is said to be singular. Singular
    ///  matrices do not have inverses.</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(float) The determinant</returns>
    static member XformDeterminant(xform:Transform) : float =
        failNotImpl()

    ///<summary>Returns a diagonal transformation matrix. Diagonal matrices are 3x3 with
    ///  the bottom row [0,0,0,1]</summary>
    ///<param name="diagonalValue">(float) The diagonal value</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformDiagonal(diagonalValue:float) : Transform =
        failNotImpl()

    ///<summary>returns the identity transformation matrix</summary>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformIdentity() : Transform =
        failNotImpl()

    ///<summary>Returns the inverse of a non-singular transformation matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(Transform) The inverted 4x4 transformation matrix .</returns>
    static member XformInverse(xform:Transform) : Transform =
        failNotImpl()

    ///<summary>Creates a mirror transformation matrix</summary>
    ///<param name="mirrorPlanePoint">(Point3d) Point on the mirror plane</param>
    ///<param name="mirrorPlaneNormal">(Vector3d) A 3D vector that is normal to the mirror plane</param>
    ///<returns>(Transform) mirror Transform matrix</returns>
    static member XformMirror(mirrorPlanePoint:Point3d, mirrorPlaneNormal:Vector3d) : Transform =
        failNotImpl()

    ///<summary>Multiplies two transformation matrices, where result = xform1 * xform2</summary>
    ///<param name="xform1">(Transform) List or Rhino.Geometry.Transform.  The first 4x4 transformation matrix to multiply.</param>
    ///<param name="xform2">(Transform) List or Rhino.Geometry.Transform.  The second 4x4 transformation matrix to multiply.</param>
    ///<returns>(Transform) result transformation on success</returns>
    static member XformMultiply(xform1:Transform, xform2:Transform) : Transform =
        failNotImpl()

    ///<summary>Returns a transformation matrix that projects to a plane.</summary>
    ///<param name="plane">(Plane) The plane to project to.</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformPlanarProjection(plane:Plane) : Transform =
        failNotImpl()

    ///<summary>Returns a rotation transformation that maps initial_plane to finalPlane.
    ///  The planes should be right hand orthonormal planes.</summary>
    ///<param name="initialPlane">(Plane) Plane to rotate from</param>
    ///<param name="finalPlane">(Plane) Plane to rotate to</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation1(initialPlane:Plane, finalPlane:Plane) : Transform =
        failNotImpl()

    ///<summary>Returns a rotation transformation around an axis</summary>
    ///<param name="angleDegrees">(int) Rotation angle in degrees</param>
    ///<param name="rotationAxis">(Vector3d) Rotation axis</param>
    ///<param name="centerPoint">(Point3d) Rotation center</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation2(angleDegrees:int, rotationAxis:Vector3d, centerPoint:Point3d) : Transform =
        failNotImpl()

    ///<summary>Calculate the minimal transformation that rotates start_direction to
    ///  end_direction while fixing centerPoint</summary>
    ///<param name="startDirection">(Vector3d) Start direction of '3d vectors' (FIXME 0)</param>
    ///<param name="endeDirection">(Vector3d) End direction of '3d vectors' (FIXME 0)</param>
    ///<param name="centerPoint">(Point3d) The rotation center</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation3(startDirection:Vector3d, endeDirection:Vector3d, centerPoint:Point3d) : Transform =
        failNotImpl()

    ///<summary>Returns a rotation transformation.</summary>
    ///<param name="x0">(Vector3d) X0 of 'Vectors defining the initial orthonormal frame' (FIXME 0)</param>
    ///<param name="y0">(Vector3d) Y0 of 'Vectors defining the initial orthonormal frame' (FIXME 0)</param>
    ///<param name="z0">(Vector3d) Z0 of 'Vectors defining the initial orthonormal frame' (FIXME 0)</param>
    ///<param name="x1">(Vector3d) X1 of 'Vectors defining the final orthonormal frame' (FIXME 0)</param>
    ///<param name="y1">(Vector3d) Y1 of 'Vectors defining the final orthonormal frame' (FIXME 0)</param>
    ///<param name="z1">(Vector3d) Z1 of 'Vectors defining the final orthonormal frame' (FIXME 0)</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation4(x0:Vector3d, y0:Vector3d, z0:Vector3d, x1:Vector3d, y1:Vector3d, z1:Vector3d) : Transform =
        failNotImpl()

    ///<summary>Creates a scale transformation</summary>
    ///<param name="scale">(float*float*float) Single number, list of 3 numbers, Point3d, or Vector3d</param>
    ///<param name="point">(Point3d) Optional, Default Value: <c>null</c>
    ///Center of scale. If omitted, world origin is used</param>
    ///<returns>(Transform) The 4x4 transformation matrix on success</returns>
    static member XformScale(scale:float*float*float, [<OPT;DEF(null)>]point:Point3d) : Transform =
        failNotImpl()

    ///<summary>Transforms a point from either client-area coordinates of the specified view
    ///  or screen coordinates to world coordinates. The resulting coordinates are represented
    ///  as a 3-D point</summary>
    ///<param name="point">(Point3d) 2D point</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or identifier of a view. If omitted, the active view is used</param>
    ///<param name="screenCoordinates">(bool) Optional, Default Value: <c>false</c>
    ///If False, point is in client-area coordinates. If True,
    ///  point is in screen-area coordinates</param>
    ///<returns>(Point3d) on success</returns>
    static member XformScreenToWorld(point:Point3d, [<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]screenCoordinates:bool) : Point3d =
        failNotImpl()

    ///<summary>Returns a shear transformation matrix</summary>
    ///<param name="plane">(Plane) Plane[0] is the fixed point</param>
    ///<param name="x">(float) X of 'each axis scale factor' (FIXME 0)</param>
    ///<param name="y">(float) Y of 'each axis scale factor' (FIXME 0)</param>
    ///<param name="z">(float) Z of 'each axis scale factor' (FIXME 0)</param>
    ///<returns>(Transform) The 4x4 transformation matrix on success</returns>
    static member XformShear(plane:Plane, x:float, y:float, z:float) : Transform =
        failNotImpl()

    ///<summary>Creates a translation transformation matrix</summary>
    ///<param name="vector">(Vector3d) List of 3 numbers, Point3d, or Vector3d.  A 3-D translation vector.</param>
    ///<returns>(Transform) The 4x4 transformation matrix is successful, otherwise None</returns>
    static member XformTranslation(vector:Vector3d) : Transform =
        failNotImpl()

    ///<summary>Transforms a point from world coordinates to construction plane coordinates.</summary>
    ///<param name="point">(Point3d) A 3D point in world coordinates.</param>
    ///<param name="plane">(Plane) The construction plane</param>
    ///<returns>(Point3d) 3D point in construction plane coordinates</returns>
    static member XformWorldToCPlane(point:Point3d, plane:Plane) : Point3d =
        failNotImpl()

    ///<summary>Transforms a point from world coordinates to either client-area coordinates of
    ///  the specified view or screen coordinates. The resulting coordinates are represented
    ///  as a 2D point</summary>
    ///<param name="point">(Point3d) 3D point in world coordinates</param>
    ///<param name="view">(string) Optional, Default Value: <c>null</c>
    ///Title or identifier of a view. If omitted, the active view is used</param>
    ///<param name="screenCoordinates">(bool) Optional, Default Value: <c>false</c>
    ///If False, the function returns the results as
    ///  client-area coordinates. If True, the result is in screen-area coordinates</param>
    ///<returns>(Point3d) 2D point on success</returns>
    static member XformWorldToScreen(point:Point3d, [<OPT;DEF(null)>]view:string, [<OPT;DEF(false)>]screenCoordinates:bool) : Point3d =
        failNotImpl()

    ///<summary>Returns a zero transformation matrix</summary>
    ///<returns>(Transform) a zero transformation matrix</returns>
    static member XformZero() : Transform =
        failNotImpl()

