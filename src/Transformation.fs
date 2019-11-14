namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsTransformation =
  
  type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Verifies a matrix is the identity matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsXformIdentity(xform:Transform) : bool =
        //xform = RhinoScriptSyntax.Coercexform(xform)
        xform = Transform.Identity
    (*
    def IsXformIdentity(xform):
        '''Verifies a matrix is the identity matrix
        Parameters:
          xform (transform): List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.
        Returns:
          bool: True or False indicating success or failure.
        '''
    
        xform = rhutil.coercexform(xform, True)
        return xform==Rhino.Geometry.Transform.Identity
    *)


    [<EXT>]
    ///<summary>Verifies a matrix is a similarity transformation. A similarity
    ///  transformation can be broken into a sequence of dialations, translations,
    ///  rotations, and reflections</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(bool) True if this transformation is an orientation preserving similarity, otherwise False.</returns>
    static member IsXformSimilarity(xform:Transform) : bool =
        //xform = RhinoScriptSyntax.Coercexform(xform)
        xform.SimilarityType <> TransformSimilarityType.NotSimilarity
    (*
    def IsXformSimilarity(xform):
        '''Verifies a matrix is a similarity transformation. A similarity
        transformation can be broken into a sequence of dialations, translations,
        rotations, and reflections
        Parameters:
          xform (transform): List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.
        Returns:
          bool: True if this transformation is an orientation preserving similarity, otherwise False.
        '''
    
        xform = rhutil.coercexform(xform, True)
        return xform.SimilarityType!=Rhino.Geometry.TransformSimilarityType.NotSimilarity
    *)


    [<EXT>]
    ///<summary>verifies that a matrix is a zero transformation matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member IsXformZero(xform:Transform) : bool =
        xform.IsZero4x4
        
    (*
    def IsXformZero(xform):
        '''verifies that a matrix is a zero transformation matrix
        Parameters:
          xform (transform): List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.
        Returns:
          bool: True or False indicating success or failure.
        '''
    
        xform = rhutil.coercexform(xform, True)
        for i in range(4):
            for j in range(4):
                if xform[i,j]!=0: return False
        return True
    *)


    [<EXT>]
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
    (*
    def XformChangeBasis(initial_plane, final_plane):
        '''Returns a change of basis transformation matrix or None on error
        Parameters:
          initial_plane (plane): the initial plane
          final_plane (plane): the final plane
        Returns:
          transform: The 4x4 transformation matrix if successful
          None: if not successful
        '''
    
        initial_plane = rhutil.coerceplane(initial_plane, True)
        final_plane = rhutil.coerceplane(final_plane, True)
        xform = Rhino.Geometry.Transform.ChangeBasis(initial_plane, final_plane)
        if not xform.IsValid: return scriptcontext.errorhandler()
        return xform
    *)


    [<EXT>]
    ///<summary>Returns a change of basis transformation matrix of None on error</summary>
    ///<param name="x0">(Vector3d) X0 of 'initial basis' (FIXME 0)</param>
    ///<param name="y0">(Vector3d) Y0 of 'initial basis' (FIXME 0)</param>
    ///<param name="z0">(Vector3d) Z0 of 'initial basis' (FIXME 0)</param>
    ///<param name="x1">(Vector3d) X1 of 'final basis' (FIXME 0)</param>
    ///<param name="y1">(Vector3d) Y1 of 'final basis' (FIXME 0)</param>
    ///<param name="z1">(Vector3d) Z1 of 'final basis' (FIXME 0)</param>
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
        let xform = Transform.ChangeBasis(x0,y0,z0,x1,y1,z1)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformChangeBasis2 failed.  x0:'%A' y0:'%A' z0:'%A' x1:'%A' y1:'%A' z1:'%A'" x0 y0 z0 x1 y1 z1
        xform
    (*
    def XformChangeBasis2(x0,y0,z0,x1,y1,z1):
        '''Returns a change of basis transformation matrix of None on error
        Parameters:
          x0,y0,z0 (vector): initial basis
          x1,y1,z1 (vector): final basis
        Returns:
          transform: The 4x4 transformation matrix if successful
          None: if not successful
        '''
    
        x0 = rhutil.coerce3dvector(x0, True)
        y0 = rhutil.coerce3dvector(y0, True)
        z0 = rhutil.coerce3dvector(z0, True)
        x1 = rhutil.coerce3dvector(x1, True)
        y1 = rhutil.coerce3dvector(y1, True)
        z1 = rhutil.coerce3dvector(z1, True)
        xform = Rhino.Geometry.Transform.ChangeBasis(x0,y0,z0,x1,y1,z1)
        if not xform.IsValid: return scriptcontext.errorhandler()
        return xform
    *)


    [<EXT>]
    ///<summary>Compares two transformation matrices</summary>
    ///<param name="xform1">(Transform) First matrix to compare</param>
    ///<param name="xform2">(Transform) Second matrix to compare</param>
    ///<returns>(int) -1 if xform1<xform2
    ///  1 if xform1>xform2
    ///  0 if xform1=xform2</returns>
    static member XformCompare(xform1:Transform, xform2:Transform) : int =
        //xform1 = RhinoScriptSyntax.Coercexform(xform1)
        //xform2 = RhinoScriptSyntax.Coercexform(xform2)
        xform1.CompareTo(xform2)
    (*
    def XformCompare(xform1, xform2):
        '''Compares two transformation matrices
        Parameters:
          xform1(transform): first matrix to compare
          xform2(transform): second matrix to compare
        Returns:
          int:
            -1 if xform1<xform2
             1 if xform1>xform2
             0 if xform1=xform2
        '''
    
        xform1 = rhutil.coercexform(xform1, True)
        xform2 = rhutil.coercexform(xform2, True)
        return xform1.CompareTo(xform2)
    *)


    [<EXT>]
    ///<summary>Transform point from construction plane coordinates to world coordinates</summary>
    ///<param name="point">(Point3d) A 3D point in construction plane coordinates.</param>
    ///<param name="plane">(Plane) The construction plane</param>
    ///<returns>(Point3d) A 3D point in world coordinates</returns>
    static member XformCPlaneToWorld(point:Point3d, plane:Plane) : Point3d =
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        plane.Origin + point.X*plane.XAxis + point.Y*plane.YAxis + point.Z*plane.ZAxis
    (*
    def XformCPlaneToWorld(point, plane):
        '''Transform point from construction plane coordinates to world coordinates
        Parameters:
          point (point): A 3D point in construction plane coordinates.
          plane (plane): The construction plane
        Returns:
          point: A 3D point in world coordinates
        '''
    
        point = rhutil.coerce3dpoint(point, True)
        plane = rhutil.coerceplane(plane, True)
        return plane.Origin + point.X*plane.XAxis + point.Y*plane.YAxis + point.Z*plane.ZAxis
    *)


    [<EXT>]
    ///<summary>Returns the determinant of a transformation matrix. If the determinant
    ///  of a transformation matrix is 0, the matrix is said to be singular. Singular
    ///  matrices do not have inverses.</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(float) The determinant</returns>
    static member XformDeterminant(xform:Transform) : float =
        //xform = RhinoScriptSyntax.Coercexform(xform)
        xform.Determinant
    (*
    def XformDeterminant(xform):
        '''Returns the determinant of a transformation matrix. If the determinant
        of a transformation matrix is 0, the matrix is said to be singular. Singular
        matrices do not have inverses.
        Parameters:
          xform (transform): List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.
        Returns:
          number: The determinant if successful
          None: if not successful
        '''
    
        xform = rhutil.coercexform(xform, True)
        return xform.Determinant
    *)


    [<EXT>]
    ///<summary>Returns a diagonal transformation matrix. Diagonal matrices are 3x3 with
    ///  the bottom row [0,0,0,1]</summary>
    ///<param name="diagonalValue">(float) The diagonal value</param>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformDiagonal(diagonalValue:float) : Transform =
        Transform(diagonalValue)
    (*
    def XformDiagonal(diagonal_value):
        '''Returns a diagonal transformation matrix. Diagonal matrices are 3x3 with
        the bottom row [0,0,0,1]
        Parameters:
          diagonal_value (number): the diagonal value
        Returns:
          transform: The 4x4 transformation matrix if successful
          None: if not successful
        '''
    
        return Rhino.Geometry.Transform(diagonal_value)
    *)


    [<EXT>]
    ///<summary>returns the identity transformation matrix</summary>
    ///<returns>(Transform) The 4x4 transformation matrix</returns>
    static member XformIdentity() : Transform =
        Transform.Identity
    (*
    def XformIdentity():
        '''returns the identity transformation matrix
        Returns:
          transform: The 4x4 transformation matrix
        '''
    
        return Rhino.Geometry.Transform.Identity
    *)


    [<EXT>]
    ///<summary>Returns the inverse of a non-singular transformation matrix</summary>
    ///<param name="xform">(Transform) List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.</param>
    ///<returns>(Transform) The inverted 4x4 transformation matrix .</returns>
    static member XformInverse(xform:Transform) : Transform =
        //xform = RhinoScriptSyntax.Coercexform(xform)
        let rc, inverse = xform.TryGetInverse()
        if not rc then failwithf "Rhino.Scripting: XformInverse failed.  xform:'%A'" xform
        inverse
    (*
    def XformInverse(xform):
        '''Returns the inverse of a non-singular transformation matrix
        Parameters:
          xform (transform): List or Rhino.Geometry.Transform.  A 4x4 transformation matrix.
        Returns:
          transform: The inverted 4x4 transformation matrix if successful.
          None: if matrix is non-singular or on error.
        '''
    
        xform = rhutil.coercexform(xform, True)
        rc, inverse = xform.TryGetInverse()
        if not rc: return scriptcontext.errorhandler()
        return inverse
    *)


    [<EXT>]
    ///<summary>Creates a mirror transformation matrix</summary>
    ///<param name="mirrorPlanePoint">(Point3d) Point on the mirror plane</param>
    ///<param name="mirrorPlaneNormal">(Vector3d) A 3D vector that is normal to the mirror plane</param>
    ///<returns>(Transform) mirror Transform matrix</returns>
    static member XformMirror(mirrorPlanePoint:Point3d, mirrorPlaneNormal:Vector3d) : Transform =
        //point = RhinoScriptSyntax.Coerce3dpoint(mirrorPlanePoint)
        //normal = RhinoScriptSyntax.Coerce3dvector(mirrorPlaneNormal)
        Transform.Mirror(mirrorPlanePoint, mirrorPlaneNormal)
    (*
    def XformMirror(mirror_plane_point, mirror_plane_normal):
        '''Creates a mirror transformation matrix
        Parameters:
          mirror_plane_point (point): point on the mirror plane
          mirror_plane_normal (vector): a 3D vector that is normal to the mirror plane
        Returns:
          transform: mirror Transform matrix
        '''
    
        point = rhutil.coerce3dpoint(mirror_plane_point, True)
        normal = rhutil.coerce3dvector(mirror_plane_normal, True)
        return Rhino.Geometry.Transform.Mirror(point, normal)
    *)


    [<EXT>]
    ///<summary>Multiplies two transformation matrices, where result = xform1 * xform2</summary>
    ///<param name="xform1">(Transform) List or Rhino.Geometry.Transform.  The first 4x4 transformation matrix to multiply.</param>
    ///<param name="xform2">(Transform) List or Rhino.Geometry.Transform.  The second 4x4 transformation matrix to multiply.</param>
    ///<returns>(Transform) result transformation on success</returns>
    static member XformMultiply(xform1:Transform, xform2:Transform) : Transform =
        //xform1 = RhinoScriptSyntax.Coercexform(xform1)
        //xform2 = RhinoScriptSyntax.Coercexform(xform2)
        xform1*xform2
    (*
    def XformMultiply(xform1, xform2):
        '''Multiplies two transformation matrices, where result = xform1 * xform2
        Parameters:
          xform1 (transform): List or Rhino.Geometry.Transform.  The first 4x4 transformation matrix to multiply.
          xform2 (transform): List or Rhino.Geometry.Transform.  The second 4x4 transformation matrix to multiply.
        Returns:
          transform: result transformation on success
        '''
    
        xform1 = rhutil.coercexform(xform1, True)
        xform2 = rhutil.coercexform(xform2, True)
        return xform1*xform2
    *)


    [<EXT>]
    ///<summary>Returns a transformation matrix that projects to a plane.</summary>
    ///<param name="plane">(Plane) The plane to project to.</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformPlanarProjection(plane:Plane) : Transform =
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        Transform.PlanarProjection(plane)
    (*
    def XformPlanarProjection(plane):
        '''Returns a transformation matrix that projects to a plane.
        Parameters:
          plane (plane): The plane to project to.
        Returns:
          transform: The 4x4 transformation matrix.
        '''
    
        plane = rhutil.coerceplane(plane, True)
        return Rhino.Geometry.Transform.PlanarProjection(plane)
    *)


    [<EXT>]
    ///<summary>Returns a rotation transformation that maps initialPlane to finalPlane.
    ///  The planes should be right hand orthonormal planes.</summary>
    ///<param name="initialPlane">(Plane) Plane to rotate from</param>
    ///<param name="finalPlane">(Plane) Plane to rotate to</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation1(initialPlane:Plane, finalPlane:Plane) : Transform =
        //initialPlane = RhinoScriptSyntax.Coerceplane(initialPlane)
        //finalPlane = RhinoScriptSyntax.Coerceplane(finalPlane)
        let xform = Transform.PlaneToPlane(initialPlane, finalPlane)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformRotation1 failed.  initialPlane:'%A' finalPlane:'%A'" initialPlane finalPlane
        xform
    (*
    def XformRotation1(initial_plane, final_plane):
        '''Returns a rotation transformation that maps initial_plane to final_plane.
        The planes should be right hand orthonormal planes.
        Parameters:
          initial_plane (plane): plane to rotate from
          final_plane (plane): plane to rotate to
        Returns:
          transform: The 4x4 transformation matrix.
          None: on error.
        '''
    
        initial_plane = rhutil.coerceplane(initial_plane, True)
        final_plane = rhutil.coerceplane(final_plane, True)
        xform = Rhino.Geometry.Transform.PlaneToPlane(initial_plane, final_plane)
        if not xform.IsValid: return scriptcontext.errorhandler()
        return xform
    *)


    [<EXT>]
    ///<summary>Returns a rotation transformation around an axis</summary>
    ///<param name="angleDegrees">(float) Rotation angle in degrees</param>
    ///<param name="rotationAxis">(Vector3d) Rotation axis</param>
    ///<param name="centerPoint">(Point3d) Rotation center</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation2( angleDegrees:float, 
                                  rotationAxis:Vector3d, 
                                  centerPoint:Point3d) : Transform =
        //axis = RhinoScriptSyntax.Coerce3dvector(rotationAxis)
        //center = RhinoScriptSyntax.Coerce3dpoint(centerPoint)
        let anglerad = toRadians(angleDegrees)
        let xform = Transform.Rotation(anglerad, rotationAxis, centerPoint)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformRotation2 failed.  angleDegrees:'%A' rotationAxis:'%A' centerPoint:'%A'" angleDegrees rotationAxis centerPoint
        xform
    (*
    def XformRotation2(angle_degrees, rotation_axis, center_point):
        '''Returns a rotation transformation around an axis
        Parameters:
          angle_degrees (number): rotation angle in degrees
          rotation_axis (vector): rotation axis
          center_point (point): rotation center
        Returns:
          transform: The 4x4 transformation matrix.
          None: on error.
        '''
    
        axis = rhutil.coerce3dvector(rotation_axis, True)
        center = rhutil.coerce3dpoint(center_point, True)
        angle_rad = math.radians(angle_degrees)
        xform = Rhino.Geometry.Transform.Rotation(angle_rad, axis, center)
        if not xform.IsValid: return scriptcontext.errorhandler()
        return xform
    *)


    [<EXT>]
    ///<summary>Calculate the minimal transformation that rotates startDirection to
    ///  endDirection while fixing centerPoint</summary>
    ///<param name="startDirection">(Vector3d) Start direction of '3d vectors' (FIXME 0)</param>
    ///<param name="endDirection">(Vector3d) End direction of '3d vectors' (FIXME 0)</param>
    ///<param name="centerPoint">(Point3d) The rotation center</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
    static member XformRotation3( startDirection:Vector3d, 
                                  endDirection:Vector3d, 
                                  centerPoint:Point3d) : Transform =
        //start = RhinoScriptSyntax.Coerce3dvector(startDirection)
        //end = RhinoScriptSyntax.Coerce3dvector(endDirection)
        //center = RhinoScriptSyntax.Coerce3dpoint(centerPoint)
        let xform = Transform.Rotation(startDirection, endDirection, centerPoint)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformRotation3 failed.  startDirection:'%A' endDirection:'%A' centerPoint:'%A'" startDirection endDirection centerPoint
        xform
    (*
    def XformRotation3( start_direction, end_direction, center_point ):
        '''Calculate the minimal transformation that rotates start_direction to
        end_direction while fixing center_point
        Parameters:
          start_direction, end_direction (vector): 3d vectors
          center_point (point): the rotation center
        Returns:
          transform: The 4x4 transformation matrix.
          None: on error.
        '''
    
        start = rhutil.coerce3dvector(start_direction, True)
        end = rhutil.coerce3dvector(end_direction, True)
        center = rhutil.coerce3dpoint(center_point, True)
        xform = Rhino.Geometry.Transform.Rotation(start, end, center)
        if not xform.IsValid: return scriptcontext.errorhandler()
        return xform
    *)


    [<EXT>]
    ///<summary>Returns a rotation transformation.</summary>
    ///<param name="x0">(Vector3d) X0 of 'Vectors defining the initial orthonormal frame' (FIXME 0)</param>
    ///<param name="y0">(Vector3d) Y0 of 'Vectors defining the initial orthonormal frame' (FIXME 0)</param>
    ///<param name="z0">(Vector3d) Z0 of 'Vectors defining the initial orthonormal frame' (FIXME 0)</param>
    ///<param name="x1">(Vector3d) X1 of 'Vectors defining the final orthonormal frame' (FIXME 0)</param>
    ///<param name="y1">(Vector3d) Y1 of 'Vectors defining the final orthonormal frame' (FIXME 0)</param>
    ///<param name="z1">(Vector3d) Z1 of 'Vectors defining the final orthonormal frame' (FIXME 0)</param>
    ///<returns>(Transform) The 4x4 transformation matrix.</returns>
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
        let xform = Transform.Rotation(x0,y0,z0,x1,y1,z1)
        if not xform.IsValid   then failwithf "Rhino.Scripting: XformRotation4 failed.  x0:'%A' y0:'%A' z0:'%A' x1:'%A' y1:'%A' z1:'%A'" x0 y0 z0 x1 y1 z1
        xform
    (*
    def XformRotation4(x0, y0, z0, x1, y1, z1):
        '''Returns a rotation transformation.
        Parameters:
          x0,y0,z0 (vector): Vectors defining the initial orthonormal frame
          x1,y1,z1 (vector): Vectors defining the final orthonormal frame
        Returns:
          transform: The 4x4 transformation matrix.
          None: on error.
        '''
    
        x0 = rhutil.coerce3dvector(x0, True)
        y0 = rhutil.coerce3dvector(y0, True)
        z0 = rhutil.coerce3dvector(z0, True)
        x1 = rhutil.coerce3dvector(x1, True)
        y1 = rhutil.coerce3dvector(y1, True)
        z1 = rhutil.coerce3dvector(z1, True)
        xform = Rhino.Geometry.Transform.Rotation(x0,y0,z0,x1,y1,z1)
        if not xform.IsValid: return scriptcontext.errorhandler()
        return xform
    *)


    [<EXT>]
    ///<summary>Creates a scale transformation</summary>
    ///<param name="scale">(float*float*float) list of 3 numbers for x , y andf z direction</param>
    ///<param name="point">(Point3d) Optional, Center of scale. If omitted, world origin is used</param>
    ///<returns>(Transform) The 4x4 transformation matrix on success</returns>
    static member XformScale(scale:float*float*float, [<OPT;DEF(Point3d())>]point:Point3d) : Transform =
        let plane = Plane(point, Vector3d.ZAxis);
        let xf = Transform.Scale(plane, scale|> t1, scale|> t2, t3 scale)
        xf
    (*
    def XformScale(scale, point=None):
        '''Creates a scale transformation
        Parameters:
          scale (number|point|vector|[number, number, number]): single number, list of 3 numbers, Point3d, or Vector3d
          point (point, optional): center of scale. If omitted, world origin is used
        Returns:
          transform: The 4x4 transformation matrix on success
          None: on error
        '''
    
        factor = rhutil.coerce3dpoint(scale)
        if factor is None:
            if type(scale) is int or type(scale) is float:
                factor = (scale,scale,scale)
            if factor is None: return scriptcontext.errorhandler()
        if point: point = rhutil.coerce3dpoint(point, True)
        else: point = Rhino.Geometry.Point3d.Origin
        plane = Rhino.Geometry.Plane(point, Rhino.Geometry.Vector3d.ZAxis);
        xf = Rhino.Geometry.Transform.Scale(plane, factor[0], factor[1], factor[2])
        return xf
    *)


    [<EXT>]
    ///<summary>Transforms a point from either client-area coordinates of the specified view
    ///  or screen coordinates to world coordinates. The resulting coordinates are represented
    ///  as a 3-D point</summary>
    ///<param name="point">(Point3d) 2D point</param>
    ///<param name="view">(string) Optional, Default Value: <c>null:string</c>
    ///Title or identifier of a view. If omitted, the active view is used</param>
    ///<param name="screenCoordinates">(bool) Optional, Default Value: <c>false</c>
    ///If False, point is in client-area coordinates. If True,
    ///  point is in screen-area coordinates</param>
    ///<returns>(Point3d) on success</returns>
    static member XformScreenToWorld( point:Point3d, 
                                      [<OPT;DEF(null:string)>]view:string, 
                                      [<OPT;DEF(false)>]screenCoordinates:bool) : Point3d =
        //point = RhinoScriptSyntax.Coerce2dpoint(point)
        let view = RhinoScriptSyntax.CoerceView(view |? "")// to get active view
        let viewport = view.MainViewport
        let xform = viewport.GetTransform(Rhino.DocObjects.CoordinateSystem.Screen, Rhino.DocObjects.CoordinateSystem.World)
        let mutable point3d = Point3d(point.X, point.Y, 0.0)
        if  screenCoordinates then
            let screen = view.ScreenRectangle
            point3d.X <- point.X - (float screen.Left)
            point3d.Y <- point.Y - (float screen.Top)
        point3d <- xform * point3d
        point3d
    (*
    def XformScreenToWorld(point, view=None, screen_coordinates=False):
        '''Transforms a point from either client-area coordinates of the specified view
        or screen coordinates to world coordinates. The resulting coordinates are represented
        as a 3-D point
        Parameters:
          point (point): 2D point
          view (str, optional): title or identifier of a view. If omitted, the active view is used
          screen_coordinates (bool, optional): if False, point is in client-area coordinates. If True,
          point is in screen-area coordinates
        Returns:
          point: on success
          None: on error
        '''
    
        point = rhutil.coerce2dpoint(point, True)
        view = rhview.__viewhelper(view)
        viewport = view.MainViewport
        xform = viewport.GetTransform(Rhino.DocObjects.CoordinateSystem.Screen, Rhino.DocObjects.CoordinateSystem.World)
        point3d = Rhino.Geometry.Point3d(point.X, point.Y, 0)
        if screen_coordinates:
            screen = view.ScreenRectangle
            point3d.X = point.X - screen.Left
            point3d.Y = point.Y - screen.Top
        point3d = xform * point3d
        return point3d
    *)


    [<EXT>]
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
        Transform.Shear(plane,x,y,z)
    (*
    def XformShear(plane, x, y, z):
        '''Returns a shear transformation matrix
        Parameters:
          plane (plane): plane[0] is the fixed point
          x,y,z (number): each axis scale factor
        Returns:
          transform: The 4x4 transformation matrix on success
        '''
    
        plane = rhutil.coerceplane(plane, True)
        x = rhutil.coerce3dvector(x, True)
        y = rhutil.coerce3dvector(y, True)
        z = rhutil.coerce3dvector(z, True)
        return Rhino.Geometry.Transform.Shear(plane,x,y,z)
    *)


    [<EXT>]
    ///<summary>Creates a translation transformation matrix</summary>
    ///<param name="vector">(Vector3d) List of 3 numbers, Point3d, or Vector3d.  A 3-D translation vector.</param>
    ///<returns>(Transform) The 4x4 transformation matrix is successful, otherwise None</returns>
    static member XformTranslation(vector:Vector3d) : Transform =
        //vector = RhinoScriptSyntax.Coerce3dvector(vector)
        Transform.Translation(vector)
    (*
    def XformTranslation(vector):
        '''Creates a translation transformation matrix
        Parameters:
          vector (vector): List of 3 numbers, Point3d, or Vector3d.  A 3-D translation vector.
        Returns:
          transform: The 4x4 transformation matrix is successful, otherwise None
        '''
    
        vector = rhutil.coerce3dvector(vector, True)
        return Rhino.Geometry.Transform.Translation(vector)
    *)


    [<EXT>]
    ///<summary>Transforms a point from world coordinates to construction plane coordinates.</summary>
    ///<param name="point">(Point3d) A 3D point in world coordinates.</param>
    ///<param name="plane">(Plane) The construction plane</param>
    ///<returns>(Point3d) 3D point in construction plane coordinates</returns>
    static member XformWorldToCPlane(point:Point3d, plane:Plane) : Point3d =
        //point = RhinoScriptSyntax.Coerce3dpoint(point)
        //plane = RhinoScriptSyntax.Coerceplane(plane)
        let v = point - plane.Origin;
        Point3d(v*plane.XAxis, v*plane.YAxis, v*plane.ZAxis)
    (*
    def XformWorldToCPlane(point, plane):
        '''Transforms a point from world coordinates to construction plane coordinates.
        Parameters:
          point (point): A 3D point in world coordinates.
          plane (plane): The construction plane
        Returns:
          (point): 3D point in construction plane coordinates
        '''
    
        point = rhutil.coerce3dpoint(point, True)
        plane = rhutil.coerceplane(plane, True)
        v = point - plane.Origin;
        return Rhino.Geometry.Point3d(v*plane.XAxis, v*plane.YAxis, v*plane.ZAxis)
    *)


    [<EXT>]
    ///<summary>Transforms a point from world coordinates to either client-area coordinates of
    ///  the specified view or screen coordinates. The resulting coordinates are represented
    ///  as a 2D point</summary>
    ///<param name="point">(Point3d) 3D point in world coordinates</param>
    ///<param name="view">(string) Optional, Default Value: <c>null:string</c>
    ///Title or identifier of a view. If omitted, the active view is used</param>
    ///<param name="screenCoordinates">(bool) Optional, Default Value: <c>false</c>
    ///If False, the function returns the results as
    ///  client-area coordinates. If True, the result is in screen-area coordinates</param>
    ///<returns>(Point2d) 2D point on success</returns>
    static member XformWorldToScreen( point:Point3d, 
                                      [<OPT;DEF(null:string)>]view:string, 
                                      [<OPT;DEF(false)>]screenCoordinates:bool) : Point2d =
        let view = RhinoScriptSyntax.CoerceView(view |? "")// to get active view
        let viewport = view.MainViewport
        let xform = viewport.GetTransform(DocObjects.CoordinateSystem.World,DocObjects.CoordinateSystem.Screen)
        let mutable point3 = xform * point
        let mutable point = Point2d(point3.X, point3.Y)
        if  screenCoordinates then
            let screen = view.ScreenRectangle
            point.X <- point.X + (float screen.Left)
            point.Y <- point.Y + (float screen.Top)       
        point
    (*
    def XformWorldToScreen(point, view=None, screen_coordinates=False):
        '''Transforms a point from world coordinates to either client-area coordinates of
        the specified view or screen coordinates. The resulting coordinates are represented
        as a 2D point
        Parameters:
          point (point): 3D point in world coordinates
          view (str, optional): title or identifier of a view. If omitted, the active view is used
          screen_coordinates (bool, optional): if False, the function returns the results as
            client-area coordinates. If True, the result is in screen-area coordinates
        Returns:
          (point): 2D point on success
          None: on error
        '''
    
        point = rhutil.coerce3dpoint(point, True)
        view = rhview.__viewhelper(view)
        viewport = view.MainViewport
        xform = viewport.GetTransform(Rhino.DocObjects.CoordinateSystem.World, Rhino.DocObjects.CoordinateSystem.Screen)
        point = xform * point
        point = Rhino.Geometry.Point2d(point.X, point.Y)
        if screen_coordinates:
            screen = view.ScreenRectangle
            point.X = point.X + screen.Left
            point.Y = point.Y + screen.Top
        return point
    *)


    [<EXT>]
    ///<summary>Returns a zero transformation matrix</summary>
    ///<returns>(Transform) a zero transformation matrix</returns>
    static member XformZero() : Transform =
        Transform()
    (*
    def XformZero():
        '''Returns a zero transformation matrix
        Returns:
          transform: a zero transformation matrix
        '''
    
        return Rhino.Geometry.Transform()
    *)


