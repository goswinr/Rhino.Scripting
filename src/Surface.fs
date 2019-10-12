namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsSurface =
    type RhinoScriptSyntax with
    
    [<EXT>]
    ///<summary>Adds a box shaped polysurface to the document</summary>
    ///<param name="corners">(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) 8 points that define the corners of the box. Points need to
    ///  be in counter-clockwise order starting with the bottom rectangle of the box</param>
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddBox(corners:Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddBox(corners):
        '''Adds a box shaped polysurface to the document
        Parameters:
          corners ([point, point, point ,point, point, point ,point,point]) 8 points that define the corners of the box. Points need to
            be in counter-clockwise order starting with the bottom rectangle of the box
        Returns:
          guid: identifier of the new object on success
        '''
        box = rhutil.coerce3dpointlist(corners, True)
        brep = Rhino.Geometry.Brep.CreateFromBox(box)
        if not brep: raise ValueError("unable to create brep from box")
        rc = scriptcontext.doc.Objects.AddBrep(brep)
        if rc==System.Guid.Empty: raise Exception("unable to add brep to document")
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds a cone shaped polysurface to the document</summary>
    ///<param name="basis">(Plane) 3D origin point of the cone or a plane with an apex at the origin
    ///  and normal along the plane's z-axis</param>
    ///<param name="height">(float) 3D height point of the cone if basis is a 3D point. The height
    ///  point defines the height and direction of the cone. If basis is a
    ///  plane, height is a numeric value</param>
    ///<param name="radius">(float) The radius at the basis of the cone</param>
    ///<param name="cap">(bool) Optional, Default Value: <c>true</c>
    ///Cap basis of the cone</param>
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddCone(basis:Plane, height:float, radius:float, [<OPT;DEF(true)>]cap:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddCone(base, height, radius, cap=True):
        '''Adds a cone shaped polysurface to the document
        Parameters:
          base (point|plane): 3D origin point of the cone or a plane with an apex at the origin
              and normal along the plane's z-axis
          height (point|number): 3D height point of the cone if base is a 3D point. The height
              point defines the height and direction of the cone. If base is a
              plane, height is a numeric value
          radius (number): the radius at the base of the cone
          cap (bool, optional): cap base of the cone
        Returns:
          guid: identifier of the new object on success
        '''
        plane = None
        height_point = rhutil.coerce3dpoint(height)
        if height_point is None:
            plane = rhutil.coerceplane(base, True)
        else:
            base_point = rhutil.coerce3dpoint(base, True)
            normal = base_point - height_point
            height = normal.Length
            plane = Rhino.Geometry.Plane(height_point, normal)
        cone = Rhino.Geometry.Cone(plane, height, radius)
        brep = Rhino.Geometry.Brep.CreateFromCone(cone, cap)
        rc = scriptcontext.doc.Objects.AddBrep(brep)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds a planar surface through objects at a designated location. For more
    ///  information, see the Rhino help file for the CutPlane command</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of objects that the cutting plane will
    ///  pass through</param>
    ///<param name="startPoint">(Line) Start point of 'line that defines the cutting plane' (FIXME 0)</param>
    ///<param name="endePoint">(Line) End point of 'line that defines the cutting plane' (FIXME 0)</param>
    ///<param name="normal">(Vector3d) Optional, Default Value: <c>Vector3d()</c>
    ///Vector that will be contained in the returned planar
    ///  surface. In the case of Rhino's CutPlane command, this is the
    ///  normal to, or Z axis of, the active view's construction plane.
    ///  If omitted, the world Z axis is used</param>
    ///<returns>(Guid) identifier of new object on success</returns>
    static member AddCutPlane(objectIds:Guid seq, startPoint:Line, endePoint:Line, [<OPT;DEF(Vector3d())>]normal:Vector3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddCutPlane(object_ids, start_point, end_point, normal=None):
        '''Adds a planar surface through objects at a designated location. For more
        information, see the Rhino help file for the CutPlane command
        Parameters:
          object_ids ([guid, ...]): identifiers of objects that the cutting plane will
              pass through
          start_point, end_point (line): line that defines the cutting plane
          normal (vector, optional): vector that will be contained in the returned planar
              surface. In the case of Rhino's CutPlane command, this is the
              normal to, or Z axis of, the active view's construction plane.
              If omitted, the world Z axis is used
        Returns:
          guid: identifier of new object on success
          None: on error
        '''
        objects = []
        bbox = Rhino.Geometry.BoundingBox.Unset
        for id in object_ids:
            rhobj = rhutil.coercerhinoobject(id, True, True)
            geometry = rhobj.Geometry
            bbox.Union( geometry.GetBoundingBox(True) )
        start_point = rhutil.coerce3dpoint(start_point, True)
        end_point = rhutil.coerce3dpoint(end_point, True)
        if not bbox.IsValid: return scriptcontext.errorhandler()
        line = Rhino.Geometry.Line(start_point, end_point)
        if normal: normal = rhutil.coerce3dvector(normal, True)
        else: normal = scriptcontext.doc.Views.ActiveView.ActiveViewport.ConstructionPlane().Normal
        surface = Rhino.Geometry.PlaneSurface.CreateThroughBox(line, normal, bbox)
        if surface is None: return scriptcontext.errorhandler()
        id = scriptcontext.doc.Objects.AddSurface(surface)
        if id==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return id
    *)


    [<EXT>]
    ///<summary>Adds a cylinder-shaped polysurface to the document</summary>
    ///<param name="basis">(Plane) The 3D basis point of the cylinder or the basis plane of the cylinder</param>
    ///<param name="height">(float) If basis is a point, then height is a 3D height point of the
    ///  cylinder. The height point defines the height and direction of the
    ///  cylinder. If basis is a plane, then height is the numeric height value
    ///  of the cylinder</param>
    ///<param name="radius">(float) Radius of the cylinder</param>
    ///<param name="cap">(bool) Optional, Default Value: <c>true</c>
    ///Cap the cylinder</param>
    ///<returns>(Guid) identifier of new object</returns>
    static member AddCylinder(basis:Plane, height:float, radius:float, [<OPT;DEF(true)>]cap:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddCylinder(base, height, radius, cap=True):
        '''Adds a cylinder-shaped polysurface to the document
        Parameters:
          base (point|plane): The 3D base point of the cylinder or the base plane of the cylinder
          height (point|number): if base is a point, then height is a 3D height point of the
            cylinder. The height point defines the height and direction of the
            cylinder. If base is a plane, then height is the numeric height value
            of the cylinder
          radius (number): radius of the cylinder
          cap (bool, optional): cap the cylinder
        Returns:
          guid: identifier of new object if successful
          None: on error
        '''
        cylinder=None
        height_point = rhutil.coerce3dpoint(height)
        if height_point:
            #base must be a point
            base = rhutil.coerce3dpoint(base, True)
            normal = height_point-base
            plane = Rhino.Geometry.Plane(base, normal)
            height = normal.Length
            circle = Rhino.Geometry.Circle(plane, radius)
            cylinder = Rhino.Geometry.Cylinder(circle, height)
        else:
            #base must be a plane
            if type(base) is Rhino.Geometry.Point3d: base = [base.X, base.Y, base.Z]
            base = rhutil.coerceplane(base, True)
            circle = Rhino.Geometry.Circle(base, radius)
            cylinder = Rhino.Geometry.Cylinder(circle, height)
        brep = cylinder.ToBrep(cap, cap)
        id = scriptcontext.doc.Objects.AddBrep(brep)
        if id==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return id
    *)


    [<EXT>]
    ///<summary>Creates a surface from 2, 3, or 4 edge curves</summary>
    ///<param name="curveIds">(Guid seq) List or tuple of curves</param>
    ///<returns>(Guid) identifier of new object</returns>
    static member AddEdgeSrf(curveIds:Guid seq) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddEdgeSrf(curve_ids):
        '''Creates a surface from 2, 3, or 4 edge curves
        Parameters:
          curve_ids ([guid, ...]): list or tuple of curves
        Returns:
          guid: identifier of new object if successful
          None: on error
        '''
        curves = [rhutil.coercecurve(id, -1, True) for id in curve_ids]
        brep = Rhino.Geometry.Brep.CreateEdgeSurface(curves)
        if brep is None: return scriptcontext.errorhandler()
        id = scriptcontext.doc.Objects.AddBrep(brep)
        if id==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return id
    *)


    [<EXT>]
    ///<summary>Creates a surface from a network of crossing curves</summary>
    ///<param name="curves">(Guid seq) Curves from which to create the surface</param>
    ///<param name="continuity">(int) Optional, Default Value: <c>1</c>
    ///How the edges match the input geometry
    ///  0 = loose
    ///  1 = position
    ///  2 = tangency
    ///  3 = curvature</param>
    ///<param name="edgeTolerance">(float) Optional, Default Value: <c>0</c>
    ///Edge tolerance</param>
    ///<param name="interiorTolerance">(float) Optional, Default Value: <c>0</c>
    ///Interior tolerance</param>
    ///<param name="angleTolerance">(float) Optional, Default Value: <c>0</c>
    ///Angle tolerance , in radians?</param>
    ///<returns>(Guid) identifier of new object</returns>
    static member AddNetworkSrf(curves:Guid seq, [<OPT;DEF(1)>]continuity:int, [<OPT;DEF(0)>]edgeTolerance:float, [<OPT;DEF(0)>]interiorTolerance:float, [<OPT;DEF(0)>]angleTolerance:float) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddNetworkSrf(curves, continuity=1, edge_tolerance=0, interior_tolerance=0, angle_tolerance=0):
        '''Creates a surface from a network of crossing curves
        Parameters:
          curves ([guid, ...]): curves from which to create the surface
          continuity (number, optional): how the edges match the input geometry
                     0 = loose
                     1 = position
                     2 = tangency
                     3 = curvature
          edge_tolerance(number,optional): edge tolerance
          interior_tolerance(number,optional): interior tolerance
          angle_tolerance(number,optional):angle tolerance , in radians?
        Returns:
          guid: identifier of new object if successful
        '''
        curves = [rhutil.coercecurve(curve, -1, True) for curve in curves]
        surf, err = Rhino.Geometry.NurbsSurface.CreateNetworkSurface(curves, continuity, edge_tolerance, interior_tolerance, angle_tolerance)
        if surf:
            rc = scriptcontext.doc.Objects.AddSurface(surf)
            scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Adds a NURBS surface object to the document</summary>
    ///<param name="pointCount">(int * int) Number of control points in the u and v direction</param>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<param name="knotsU">(float seq) Knot values for the surface in the u direction.
    ///  Must contain pointCount[0]+degree[0]-1 elements</param>
    ///<param name="knotsV">(float seq) Knot values for the surface in the v direction.
    ///  Must contain pointCount[1]+degree[1]-1 elements</param>
    ///<param name="degree">(int * int) Degree of the surface in the u and v directions.</param>
    ///<param name="weights">(float seq) Optional, Default Value: <c>null:float seq</c>
    ///Weight values for the surface. The number of elements in
    ///  weights must equal the number of elements in points. Values must be
    ///  greater than zero.</param>
    ///<returns>(Guid) identifier of new object</returns>
    static member AddNurbsSurface(pointCount:int * int, points:Point3d seq, knotsU:float seq, knotsV:float seq, degree:int * int, [<OPT;DEF(null:float seq)>]weights:float seq) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddNurbsSurface(point_count, points, knots_u, knots_v, degree, weights=None):
        '''Adds a NURBS surface object to the document
        Parameters:
          point_count ([number, number]) number of control points in the u and v direction
          points ([point, ...]): list of 3D points
          knots_u ([number, ...]): knot values for the surface in the u direction.
                    Must contain point_count[0]+degree[0]-1 elements
          knots_v ([number, ...]): knot values for the surface in the v direction.
                    Must contain point_count[1]+degree[1]-1 elements
          degree ([number, number]): degree of the surface in the u and v directions.
          weights ([(number, ...]): weight values for the surface. The number of elements in
            weights must equal the number of elements in points. Values must be
            greater than zero.
        Returns:
          guid: identifier of new object if successful
          None on error
        '''
        if len(points)<(point_count[0]*point_count[1]):
            return scriptcontext.errorhandler()
        ns = Rhino.Geometry.NurbsSurface.Create(3, weights!=None, degree[0]+1, degree[1]+1, point_count[0], point_count[1])
        #add the points and weights
        controlpoints = ns.Points
        index = 0
        for i in range(point_count[0]):
            for j in range(point_count[1]):
                if weights:
                    cp = Rhino.Geometry.ControlPoint(points[index], weights[index])
                    controlpoints.SetControlPoint(i,j,cp)
                else:
                    cp = Rhino.Geometry.ControlPoint(points[index])
                    controlpoints.SetControlPoint(i,j,cp)
                index += 1
        #add the knots
        for i in range(ns.KnotsU.Count): ns.KnotsU[i] = knots_u[i]
        for i in range(ns.KnotsV.Count): ns.KnotsV[i] = knots_v[i]
        if not ns.IsValid: return scriptcontext.errorhandler()
        id = scriptcontext.doc.Objects.AddSurface(ns)
        if id==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return id
    *)


    [<EXT>]
    ///<summary>Fits a surface through curve, point, point cloud, and mesh objects.</summary>
    ///<param name="objectIds">(Guid seq) A list of object identifiers that indicate the objects to use for the patch fitting.
    ///  Acceptable object types include curves, points, point clouds, and meshes.</param>
    ///<param name="uvSpansTupleORSurfaceObjectId">(float * Guid) The U and V direction span counts for the automatically generated surface OR
    ///  The identifier of the starting surface.  It is best if you create a starting surface that is similar in shape
    ///  to the surface you are trying to create.</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///The tolerance used by input analysis functions. If omitted, Rhino's document absolute tolerance is used.</param>
    ///<param name="trim">(bool) Optional, Default Value: <c>true</c>
    ///Try to find an outside curve and trims the surface to it.  The default value is True.</param>
    ///<param name="pointSpacing">(float) Optional, Default Value: <c>0.1</c>
    ///The basic distance between points sampled from input curves.  The default value is 0.1.</param>
    ///<param name="flexibility">(float) Optional, Default Value: <c>1.0</c>
    ///Determines the behavior of the surface in areas where its not otherwise controlled by the input.
    ///  Lower numbers make the surface behave more like a stiff material, higher, more like a flexible material.
    ///  That is, each span is made to more closely match the spans adjacent to it if there is no input geometry
    ///  mapping to that area of the surface when the flexibility value is low.  The scale is logarithmic.
    ///  For example, numbers around 0.001 or 0.1 make the patch pretty stiff and numbers around 10 or 100
    ///  make the surface flexible.  The default value is 1.0.</param>
    ///<param name="surfacePull">(float) Optional, Default Value: <c>1.0</c>
    ///Similar to stiffness, but applies to the starting surface. The bigger the pull, the closer
    ///  the resulting surface shape will be to the starting surface.  The default value is 1.0.</param>
    ///<param name="fixEdges">(bool) Optional, Default Value: <c>false</c>
    ///Clamps the edges of the starting surface in place. This option is useful if you are using a
    ///  curve or points for deforming an existing surface, and you do not want the edges of the starting surface
    ///  to move.  The default if False.</param>
    ///<returns>(Guid) Identifier of the new surface object .</returns>
    static member AddPatch(objectIds:Guid seq, uvSpansTupleORSurfaceObjectId:float * Guid, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(true)>]trim:bool, [<OPT;DEF(0.1)>]pointSpacing:float, [<OPT;DEF(1.0)>]flexibility:float, [<OPT;DEF(1.0)>]surfacePull:float, [<OPT;DEF(false)>]fixEdges:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddPatch(object_ids, uv_spans_tuple_OR_surface_object_id, tolerance=None, trim=True, point_spacing=0.1, flexibility=1.0, surface_pull=1.0, fix_edges=False):
        '''Fits a surface through curve, point, point cloud, and mesh objects.
        Parameters:
          object_ids ([guid, ...]): a list of object identifiers that indicate the objects to use for the patch fitting.
              Acceptable object types include curves, points, point clouds, and meshes.
          uv_spans_tuple_OR_surface_object_id ([number, number]|guid):  the U and V direction span counts for the automatically generated surface OR
              The identifier of the starting surface.  It is best if you create a starting surface that is similar in shape 
              to the surface you are trying to create.
          tolerance (number, optional): The tolerance used by input analysis functions. If omitted, Rhino's document absolute tolerance is used.
          trim (bool, optional): Try to find an outside curve and trims the surface to it.  The default value is True.
          point_spacing (number, optional): The basic distance between points sampled from input curves.  The default value is 0.1.
          flexibility (number, optional): Determines the behavior of the surface in areas where its not otherwise controlled by the input.
              Lower numbers make the surface behave more like a stiff material, higher, more like a flexible material.  
              That is, each span is made to more closely match the spans adjacent to it if there is no input geometry 
              mapping to that area of the surface when the flexibility value is low.  The scale is logarithmic.  
              For example, numbers around 0.001 or 0.1 make the patch pretty stiff and numbers around 10 or 100 
              make the surface flexible.  The default value is 1.0.
          surface_pull (number, optional): Similar to stiffness, but applies to the starting surface. The bigger the pull, the closer
              the resulting surface shape will be to the starting surface.  The default value is 1.0.
          fix_edges (bool, optional): Clamps the edges of the starting surface in place. This option is useful if you are using a
              curve or points for deforming an existing surface, and you do not want the edges of the starting surface 
              to move.  The default if False.
        Returns:
          guid: Identifier of the new surface object if successful.
          None: on error.
        '''
        # System.Collections.List instead of Python list because IronPython is
        # having problems converting a list to an IEnumerable<GeometryBase> which
        # is the 1st argument for Brep.CreatePatch
        geometry = List[Rhino.Geometry.GeometryBase]()
        u_span = 10
        v_span = 10
        rc = None
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        for object_id in object_ids:
            rhobj = rhutil.coercerhinoobject(object_id, False, False)
            if not rhobj: return None
            geometry.Add( rhobj.Geometry )
        if not geometry: return None
        
        surface = None
        if uv_spans_tuple_OR_surface_object_id:
          if type(uv_spans_tuple_OR_surface_object_id) is tuple:
            u_span = uv_spans_tuple_OR_surface_object_id[0]
            v_span = uv_spans_tuple_OR_surface_object_id[1]
          else:
            surface = rhutil.coercesurface(uv_spans_tuple_OR_surface_object_id, False)
        if not tolerance: tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        b = System.Array.CreateInstance(bool, 4)
        for i in range(4): b[i] = fix_edges
        brep = Rhino.Geometry.Brep.CreatePatch(geometry, surface, u_span, v_span, trim, False, point_spacing, flexibility, surface_pull, b, tolerance)
        if brep:
          rc = scriptcontext.doc.Objects.AddBrep(brep)
          scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Creates a single walled surface with a circular profile around a curve</summary>
    ///<param name="curveId">(Guid) Identifier of rail curve</param>
    ///<param name="parameters">(float seq) Parameters of 'list of radius values at normalized curve parameters' (FIXME 0)</param>
    ///<param name="radii">(float seq) Radii of 'list of radius values at normalized curve parameters' (FIXME 0)</param>
    ///<param name="blendeType">(int) Optional, Default Value: <c>0</c>
    ///0(local) or 1(global)</param>
    ///<param name="cap">(float) Optional, Default Value: <c>0</c>
    ///0(none), 1(flat), 2(round)</param>
    ///<param name="fit">(bool) Optional, Default Value: <c>false</c>
    ///Attempt to fit a single surface</param>
    ///<returns>(Guid seq) identifiers of new objects created</returns>
    static member AddPipe(curveId:Guid, parameters:float seq, radii:float seq, [<OPT;DEF(0)>]blendeType:int, [<OPT;DEF(0)>]cap:float, [<OPT;DEF(false)>]fit:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddPipe(curve_id, parameters, radii, blend_type=0, cap=0, fit=False):
        '''Creates a single walled surface with a circular profile around a curve
        Parameters:
          curve_id (guid): identifier of rail curve
          parameters, radii ([number, ...]): list of radius values at normalized curve parameters
          blend_type (number, optional): 0(local) or 1(global)
          cap (number, optional): 0(none), 1(flat), 2(round)
          fit (bool, optional): attempt to fit a single surface
        Returns:
          list(guid, ...): identifiers of new objects created
        '''
        rail = rhutil.coercecurve(curve_id, -1, True)
        abs_tol = scriptcontext.doc.ModelAbsoluteTolerance
        ang_tol = scriptcontext.doc.ModelAngleToleranceRadians
        if type(parameters) is int or type(parameters) is float: parameters = [parameters]
        if type(radii) is int or type(radii) is float: radii = [radii]
        parameters = map(float,parameters)
        radii = map(float,radii)
        cap = System.Enum.ToObject(Rhino.Geometry.PipeCapMode, cap)
        breps = Rhino.Geometry.Brep.CreatePipe(rail, parameters, radii, blend_type==0, cap, fit, abs_tol, ang_tol)
        rc = [scriptcontext.doc.Objects.AddBrep(brep) for brep in breps]
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Creates one or more surfaces from planar curves</summary>
    ///<param name="objectIds">(Guid seq) Curves to use for creating planar surfaces</param>
    ///<returns>(Guid seq) identifiers of surfaces created on success</returns>
    static member AddPlanarSrf(objectIds:Guid seq) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddPlanarSrf(object_ids):
        '''Creates one or more surfaces from planar curves
        Parameters:
          object_ids ([guid, ...]): curves to use for creating planar surfaces
        Returns:
          list(guid, ...): identifiers of surfaces created on success
          None: on error
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        curves = [rhutil.coercecurve(id,-1,True) for id in object_ids]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        breps = Rhino.Geometry.Brep.CreatePlanarBreps(curves, tolerance)
        if breps:
            rc = [scriptcontext.doc.Objects.AddBrep(brep) for brep in breps]
            scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Create a plane surface and add it to the document.</summary>
    ///<param name="plane">(Plane) The plane.</param>
    ///<param name="uDir">(float) The magnitude in the U direction.</param>
    ///<param name="vDir">(float) The magnitude in the V direction.</param>
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddPlaneSurface(plane:Plane, uDir:float, vDir:float) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddPlaneSurface(plane, u_dir, v_dir):
        '''Create a plane surface and add it to the document.
        Parameters:
          plane (plane): The plane.
          u_dir (number): The magnitude in the U direction.
          v_dir (number): The magnitude in the V direction.
        Returns:
          guid: The identifier of the new object if successful.
          None: if not successful, or on error.
        '''
        plane = rhutil.coerceplane(plane, True)
        u_interval = Rhino.Geometry.Interval(0, u_dir)
        v_interval = Rhino.Geometry.Interval(0, v_dir)
        plane_surface = Rhino.Geometry.PlaneSurface(plane, u_interval, v_interval) 
        if plane_surface is None: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddSurface(plane_surface)
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds a surface created by lofting curves to the document.
    ///  - no curve sorting performed. pass in curves in the order you want them sorted
    ///  - directions of open curves not adjusted. Use CurveDirectionsMatch and
    ///    ReverseCurve to adjust the directions of open curves
    ///  - seams of closed curves are not adjusted. Use CurveSeam to adjust the seam
    ///    of closed curves</summary>
    ///<param name="objectIds">(Guid seq) Ordered list of the curves to loft through</param>
    ///<param name="start">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///Starting point of the loft</param>
    ///<param name="ende">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///Ending point of the loft</param>
    ///<param name="loftType">(int) Optional, Default Value: <c>0</c>
    ///Type of loft. Possible options are:
    ///  0 = Normal. Uses chord-length parameterization in the loft direction
    ///  1 = Loose. The surface is allowed to move away from the original curves
    ///    to make a smoother surface. The surface control points are created
    ///    at the same locations as the control points of the loft input curves.
    ///  2 = Straight. The sections between the curves are straight. This is
    ///    also known as a ruled surface.
    ///  3 = Tight. The surface sticks closely to the original curves. Uses square
    ///    root of chord-length parameterization in the loft direction
    ///  4 = Developable. Creates a separate developable surface or polysurface
    ///    from each pair of curves.</param>
    ///<param name="simplifyMethod">(float) Optional, Default Value: <c>0</c>
    ///Possible options are:
    ///  0 = None. Does not simplify.
    ///  1 = Rebuild. Rebuilds the shape curves before lofting. modified by `value` below
    ///  2 = Refit. Refits the shape curves to a specified tolerance. modified by `value` below</param>
    ///<param name="value">(float) Optional, Default Value: <c>0</c>
    ///Additional value based on the specified `simplifyMethod`:
    ///  Simplify  -   Description
    ///  Rebuild(1) - then value is the number of control point used to rebuild
    ///  Rebuild(1) - is specified and this argument is omitted, then curves will be
    ///    rebuilt using 10 control points.
    ///  Refit(2) - then value is the tolerance used to rebuild.
    ///  Refit(2) - is specified and this argument is omitted, then the document's
    ///    absolute tolerance us used for refitting.</param>
    ///<param name="closed">(bool) Optional, Default Value: <c>false</c>
    ///Close the loft back to the first curve</param>
    ///<returns>(Guid seq) Array containing the identifiers of the new surface objects</returns>
    static member AddLoftSrf(objectIds:Guid seq, [<OPT;DEF(Point3d())>]start:Point3d, [<OPT;DEF(Point3d())>]ende:Point3d, [<OPT;DEF(0)>]loftType:int, [<OPT;DEF(0)>]simplifyMethod:float, [<OPT;DEF(0)>]value:float, [<OPT;DEF(false)>]closed:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddLoftSrf(object_ids, start=None, end=None, loft_type=0, simplify_method=0, value=0, closed=False):
        '''Adds a surface created by lofting curves to the document.
        - no curve sorting performed. pass in curves in the order you want them sorted
        - directions of open curves not adjusted. Use CurveDirectionsMatch and
          ReverseCurve to adjust the directions of open curves
        - seams of closed curves are not adjusted. Use CurveSeam to adjust the seam
          of closed curves
        Parameters:
          object_ids ([guid, guid, ...]): ordered list of the curves to loft through
          start (point, optional): starting point of the loft
          end (point, optional): ending point of the loft
          loft_type (number, optional): type of loft. Possible options are:
            0 = Normal. Uses chord-length parameterization in the loft direction
            1 = Loose. The surface is allowed to move away from the original curves
                to make a smoother surface. The surface control points are created
                at the same locations as the control points of the loft input curves.
            2 = Straight. The sections between the curves are straight. This is
                also known as a ruled surface.
            3 = Tight. The surface sticks closely to the original curves. Uses square
                root of chord-length parameterization in the loft direction
            4 = Developable. Creates a separate developable surface or polysurface
                from each pair of curves.
          simplify_method (number, optional): Possible options are:
            0 = None. Does not simplify.
            1 = Rebuild. Rebuilds the shape curves before lofting. modified by `value` below
            2 = Refit. Refits the shape curves to a specified tolerance. modified by `value` below
          value (number, optional): Additional value based on the specified `simplify_method`:
            Simplify  -   Description
            Rebuild(1) - then value is the number of control point used to rebuild
            Rebuild(1) - is specified and this argument is omitted, then curves will be
                         rebuilt using 10 control points.
            Refit(2) - then value is the tolerance used to rebuild.
            Refit(2) - is specified and this argument is omitted, then the document's
                         absolute tolerance us used for refitting.
          closed (bool, optional): close the loft back to the first curve
        Returns:
          list(guid, ...):Array containing the identifiers of the new surface objects if successful
          None: on error
        '''
        if loft_type<0 or loft_type>5: raise ValueError("loft_type must be 0-4")
        if simplify_method<0 or simplify_method>2: raise ValueError("simplify_method must be 0-2")
        # get set of curves from object_ids
        curves = [rhutil.coercecurve(id,-1,True) for id in object_ids]
        if len(curves)<2: return scriptcontext.errorhandler()
        if start is None: start = Rhino.Geometry.Point3d.Unset
        if end is None: end = Rhino.Geometry.Point3d.Unset
        start = rhutil.coerce3dpoint(start, True)
        end = rhutil.coerce3dpoint(end, True)
        
        lt = Rhino.Geometry.LoftType.Normal
        if loft_type==1: lt = Rhino.Geometry.LoftType.Loose
        elif loft_type==2: lt = Rhino.Geometry.LoftType.Straight
        elif loft_type==3: lt = Rhino.Geometry.LoftType.Tight
        elif loft_type==4: lt = Rhino.Geometry.LoftType.Developable
        breps = None
        if simplify_method==0:
            breps = Rhino.Geometry.Brep.CreateFromLoft(curves, start, end, lt, closed)
        elif simplify_method==1:
            value = abs(value)
            rebuild_count = int(value)
            breps = Rhino.Geometry.Brep.CreateFromLoftRebuild(curves, start, end, lt, closed, rebuild_count)
        elif simplify_method==2:
            refit = abs(value)
            if refit==0: refit = scriptcontext.doc.ModelAbsoluteTolerance
            breps = Rhino.Geometry.Brep.CreateFromLoftRefit(curves, start, end, lt, closed, refit)
        if not breps: return scriptcontext.errorhandler()
        idlist = []
        for brep in breps:
            id = scriptcontext.doc.Objects.AddBrep(brep)
            if id!=System.Guid.Empty: idlist.append(id)
        if idlist: scriptcontext.doc.Views.Redraw()
        return idlist
    *)


    [<EXT>]
    ///<summary>Create a surface by revolving a curve around an axis</summary>
    ///<param name="curveId">(Guid) Identifier of profile curve</param>
    ///<param name="axis">(Line) Line for the rail revolve axis</param>
    ///<param name="startAngle">(float) Optional, Default Value: <c>0.0</c>
    ///Start angles of revolve</param>
    ///<param name="endeAngle">(float) Optional, Default Value: <c>360.0</c>
    ///End angles of revolve</param>
    ///<returns>(Guid) identifier of new object</returns>
    static member AddRevSrf(curveId:Guid, axis:Line, [<OPT;DEF(0.0)>]startAngle:float, [<OPT;DEF(360.0)>]endeAngle:float) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddRevSrf(curve_id, axis, start_angle=0.0, end_angle=360.0):
        '''Create a surface by revolving a curve around an axis
        Parameters:
          curve_id (guid): identifier of profile curve
          axis (line): line for the rail revolve axis
          start_angle, end_angle (number, optional): start and end angles of revolve
        Returns:
          guid: identifier of new object if successful
          None: on error
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        axis = rhutil.coerceline(axis, True)
        start_angle = math.radians(start_angle)
        end_angle = math.radians(end_angle)
        srf = Rhino.Geometry.RevSurface.Create(curve, axis, start_angle, end_angle)
        if not srf: return scriptcontext.errorhandler()
        ns = srf.ToNurbsSurface()
        if not ns: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddSurface(ns)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Add a spherical surface to the document</summary>
    ///<param name="centerOrPlane">(Plane) Center point of the sphere. If a plane is input,
    ///  the origin of the plane will be the center of the sphere</param>
    ///<param name="radius">(float) Radius of the sphere in the current model units</param>
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddSphere(centerOrPlane:Plane, radius:float) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddSphere(center_or_plane, radius):
        '''Add a spherical surface to the document
        Parameters:
          center_or_plane (point|plane): center point of the sphere. If a plane is input,
            the origin of the plane will be the center of the sphere
          radius (number): radius of the sphere in the current model units
        Returns:
          guid: identifier of the new object on success
          None: on error
        '''
        c_or_p = rhutil.coerce3dpoint(center_or_plane)
        if c_or_p is None:
            c_or_p = rhutil.coerceplane(center_or_plane)
        if c_or_p is None: return None
        sphere = Rhino.Geometry.Sphere(c_or_p, radius)
        rc = scriptcontext.doc.Objects.AddSphere(sphere)
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds a spaced series of planar curves resulting from the intersection of
    ///  defined cutting planes through a surface or polysurface. For more
    ///  information, see Rhino help for details on the Contour command</summary>
    ///<param name="objectId">(Guid) Object identifier to contour</param>
    ///<param name="pointsOrPlane">(Point3d * Plane) Either a list/tuple of two points or a plane
    ///  if two points, they define the start and end points of a center line
    ///  if a plane, the plane defines the cutting plane</param>
    ///<param name="interval">(float) Optional, Default Value: <c>7e89</c>
    ///Distance between contour curves.</param>
    ///<returns>(Guid) ids of new contour curves on success</returns>
    static member AddSrfContourCrvs(objectId:Guid, pointsOrPlane:Point3d * Plane, [<OPT;DEF(7e89)>]interval:float) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddSrfContourCrvs(object_id, points_or_plane, interval=None):
        '''Adds a spaced series of planar curves resulting from the intersection of
        defined cutting planes through a surface or polysurface. For more
        information, see Rhino help for details on the Contour command
        Parameters:
          object_id (guid): object identifier to contour
          points_or_plane ([point,point]|plane): either a list/tuple of two points or a plane
            if two points, they define the start and end points of a center line
            if a plane, the plane defines the cutting plane
          interval (number, optional): distance between contour curves.
        Returns:
          guid: ids of new contour curves on success
          None: on error
        '''
        brep = rhutil.coercebrep(object_id)
        plane = rhutil.coerceplane(points_or_plane)
        curves = None
        if plane:
            curves = Rhino.Geometry.Brep.CreateContourCurves(brep, plane)
        else:
            start = rhutil.coerce3dpoint(points_or_plane[0], True)
            end = rhutil.coerce3dpoint(points_or_plane[1], True)
            if not interval:
                bbox = brep.GetBoundingBox(True)
                v = bbox.Max - bbox.Min
                interval = v.Length / 50.0
            curves = Rhino.Geometry.Brep.CreateContourCurves(brep, start, end, interval)
        rc = []
        for crv in curves:
            id = scriptcontext.doc.Objects.AddCurve(crv)
            if id!=System.Guid.Empty: rc.append(id)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Creates a surface from a grid of points</summary>
    ///<param name="count">(int * int) Tuple of two numbers defining number of points in the u,v directions</param>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<param name="degree">(int * int) Optional, Default Value: <c>3*3</c>
    ///Two numbers defining degree of the surface in the u,v directions</param>
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddSrfControlPtGrid(count:int * int, points:Point3d seq, [<OPT;DEF(null)>]degree:int * int) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddSrfControlPtGrid(count, points, degree=(3,3)):
        '''Creates a surface from a grid of points
        Parameters:
          count ([number, number])tuple of two numbers defining number of points in the u,v directions
          points ([point, ...]): list of 3D points
          degree ([number, number]): two numbers defining degree of the surface in the u,v directions
        Returns:
          guid: The identifier of the new object if successful.
          None: if not successful, or on error.
        '''
        points = rhutil.coerce3dpointlist(points, True)
        surf = Rhino.Geometry.NurbsSurface.CreateFromPoints(points, count[0], count[1], degree[0], degree[1])
        if not surf: return scriptcontext.errorhandler()
        id = scriptcontext.doc.Objects.AddSurface(surf)
        if id!=System.Guid.Empty:
            scriptcontext.doc.Views.Redraw()
            return id
    *)


    [<EXT>]
    ///<summary>Creates a new surface from either 3 or 4 corner points.</summary>
    ///<param name="points">(Point3d * Point3d * Point3d * Point3d) List of either 3 or 4 corner points</param>
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddSrfPt(points:Point3d * Point3d * Point3d * Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddSrfPt(points):
        '''Creates a new surface from either 3 or 4 corner points.
        Parameters:
          points ([point, point, point, point]): list of either 3 or 4 corner points
        Returns:
          guid: The identifier of the new object if successful.
          None: if not successful, or on error.
        '''
        points = rhutil.coerce3dpointlist(points, True)
        surface=None
        if len(points)==3:
            surface = Rhino.Geometry.NurbsSurface.CreateFromCorners(points[0], points[1], points[2])
        elif len(points)==4:
            surface = Rhino.Geometry.NurbsSurface.CreateFromCorners(points[0], points[1], points[2], points[3])
        if surface is None: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddSurface(surface)
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Creates a surface from a grid of points</summary>
    ///<param name="count">(int * int) Tuple of two numbers defining number of points in the u,v directions</param>
    ///<param name="points">(Point3d seq) List of 3D points</param>
    ///<param name="degree">(int * int) Optional, Default Value: <c>3*3</c>
    ///Two numbers defining degree of the surface in the u,v directions</param>
    ///<param name="closed">(bool * bool) Optional, Default Value: <c>false*false</c>
    ///Two booleans defining if the surface is closed in the u,v directions</param>
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddSrfPtGrid(count:int * int, points:Point3d seq, [<OPT;DEF(null)>]degree:int * int, [<OPT;DEF(null)>]closed:bool * bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddSrfPtGrid(count, points, degree=(3,3), closed=(False,False)):
        '''Creates a surface from a grid of points
        Parameters:
          count ([number, number]): tuple of two numbers defining number of points in the u,v directions
          points ([point, ...]): list of 3D points
          degree ([number, number], optional): two numbers defining degree of the surface in the u,v directions
          closed ([bool, bool], optional): two booleans defining if the surface is closed in the u,v directions
        Returns:
          guid: The identifier of the new object if successful.
          None: if not successful, or on error.
        '''
        points = rhutil.coerce3dpointlist(points, True)
        surf = Rhino.Geometry.NurbsSurface.CreateThroughPoints(points, count[0], count[1], degree[0], degree[1], closed[0], closed[1])
        if not surf: return scriptcontext.errorhandler()
        id = scriptcontext.doc.Objects.AddSurface(surf)
        if id!=System.Guid.Empty:
            scriptcontext.doc.Views.Redraw()
            return id
    *)


    [<EXT>]
    ///<summary>Adds a surface created through profile curves that define the surface
    ///  shape and one curve that defines a surface edge.</summary>
    ///<param name="rail">(Guid) Identifier of the rail curve</param>
    ///<param name="shapes">(Guid seq) One or more cross section shape curves</param>
    ///<param name="closed">(bool) Optional, Default Value: <c>false</c>
    ///If True, then create a closed surface</param>
    ///<returns>(Guid seq) of new surface objects</returns>
    static member AddSweep1(rail:Guid, shapes:Guid seq, [<OPT;DEF(false)>]closed:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddSweep1(rail, shapes, closed=False):
        '''Adds a surface created through profile curves that define the surface
        shape and one curve that defines a surface edge.
        Parameters:
          rail (guid): identifier of the rail curve
          shapes ([guid, ...]): one or more cross section shape curves
          closed (bool, optional): If True, then create a closed surface
        Returns:
          list(guid, ...): of new surface objects if successful
          None: if not successful, or on error
        '''
        rail = rhutil.coercecurve(rail, -1, True)
        shapes = [rhutil.coercecurve(shape, -1, True) for shape in shapes]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        breps = Rhino.Geometry.Brep.CreateFromSweep(rail, shapes, closed, tolerance)
        if not breps: return scriptcontext.errorhandler()
        rc = [scriptcontext.doc.Objects.AddBrep(brep) for brep in breps]
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds a surface created through profile curves that define the surface
    ///  shape and two curves that defines a surface edge.</summary>
    ///<param name="rails">(Guid * Guid) Identifiers of the two rail curve</param>
    ///<param name="shapes">(Guid seq) One or more cross section shape curves</param>
    ///<param name="closed">(bool) Optional, Default Value: <c>false</c>
    ///If True, then create a closed surface</param>
    ///<returns>(Guid seq) of new surface objects</returns>
    static member AddSweep2(rails:Guid * Guid, shapes:Guid seq, [<OPT;DEF(false)>]closed:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddSweep2(rails, shapes, closed=False):
        '''Adds a surface created through profile curves that define the surface
        shape and two curves that defines a surface edge.
        Parameters:
          rails ([guid, guid]): identifiers of the two rail curve
          shapes ([guid, ...]): one or more cross section shape curves
          closed (bool, optional): If True, then create a closed surface
        Returns:
          list(guid, ...): of new surface objects if successful
          None: if not successful, or on error
        '''
        rail1 = rhutil.coercecurve(rails[0], -1, True)
        rail2 = rhutil.coercecurve(rails[1], -1, True)
        shapes = [rhutil.coercecurve(shape, -1, True) for shape in shapes]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        breps = Rhino.Geometry.Brep.CreateFromSweep(rail1, rail2, shapes, closed, tolerance)
        if not breps: return scriptcontext.errorhandler()
        rc = [scriptcontext.doc.Objects.AddBrep(brep) for brep in breps]
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds a surface created through profile curves that define the surface
    ///  shape and two curves that defines a surface edge.</summary>
    ///<param name="profile">(Guid) Identifier of the profile curve</param>
    ///<param name="rail">(Guid) Identifier of the rail curve</param>
    ///<param name="axis">(Point3d * Point3d) A list of two 3-D points identifying the start point and end point of the rail revolve axis, or a Line</param>
    ///<param name="scaleHeight">(bool) Optional, Default Value: <c>false</c>
    ///If True, surface will be locally scaled. Defaults to False</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member AddRailRevSrf(profile:Guid, rail:Guid, axis:Point3d * Point3d, [<OPT;DEF(false)>]scaleHeight:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddRailRevSrf(profile, rail, axis, scale_height=False):
        '''Adds a surface created through profile curves that define the surface
        shape and two curves that defines a surface edge.
        Parameters:
          profile (guid): identifier of the profile curve
          rail (guid): identifier of the rail curve
          axis ([point, point]): A list of two 3-D points identifying the start point and end point of the rail revolve axis, or a Line
          scale_height (bool, optional): If True, surface will be locally scaled. Defaults to False
        Returns:
          guid: identifier of the new object if successful
          None: if not successful, or on error
        '''
        profile_inst = rhutil.coercecurve(profile, -1, True)
        rail_inst = rhutil.coercecurve(rail, -1, True)
        axis_start = rhutil.coerce3dpoint(axis[0], True)
        axis_end = rhutil.coerce3dpoint(axis[1], True)
        line = Rhino.Geometry.Line(axis_start, axis_end)
        surface = Rhino.Geometry.NurbsSurface.CreateRailRevolvedSurface(profile_inst, rail_inst, line, scale_height)
        if not surface: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddSurface(surface)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Adds a torus shaped revolved surface to the document</summary>
    ///<param name="basis">(Point3d) 3D origin point of the torus or the basis plane of the torus</param>
    ///<param name="majorRadius">(float) Major radius of 'the two radii of the torus' (FIXME 0)</param>
    ///<param name="minorRadius">(float) Minor radius of 'the two radii of the torus' (FIXME 0)</param>
    ///<param name="direction">(Point3d) Optional, Default Value: <c>Point3d()</c>
    ///A point that defines the direction of the torus when basis is a point.
    ///  If omitted, a torus that is parallel to the world XY plane is created</param>
    ///<returns>(Guid) The identifier of the new object .</returns>
    static member AddTorus(basis:Point3d, majorRadius:float, minorRadius:float, [<OPT;DEF(Point3d())>]direction:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def AddTorus(base, major_radius, minor_radius, direction=None):
        '''Adds a torus shaped revolved surface to the document
        Parameters:
          base (point): 3D origin point of the torus or the base plane of the torus
          major_radius, minor_radius (number): the two radii of the torus
          direction (point):  A point that defines the direction of the torus when base is a point.
            If omitted, a torus that is parallel to the world XY plane is created
        Returns:
          guid: The identifier of the new object if successful.
          None: if not successful, or on error.
        '''
        baseplane = None
        basepoint = rhutil.coerce3dpoint(base)
        if basepoint is None:
            baseplane = rhutil.coerceplane(base, True)
            if direction!=None: return scriptcontext.errorhandler()
        if baseplane is None:
            direction = rhutil.coerce3dpoint(direction, False)
            if direction: direction = direction-basepoint
            else: direction = Rhino.Geometry.Vector3d.ZAxis
            baseplane = Rhino.Geometry.Plane(basepoint, direction)
        torus = Rhino.Geometry.Torus(baseplane, major_radius, minor_radius)
        revsurf = torus.ToRevSurface()
        rc = scriptcontext.doc.Objects.AddSurface(revsurf)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Performs a boolean difference operation on two sets of input surfaces
    ///  and polysurfaces. For more details, see the BooleanDifference command in
    ///  the Rhino help file</summary>
    ///<param name="input0">(Guid seq) List of surfaces to subtract from</param>
    ///<param name="input1">(Guid seq) List of surfaces to be subtracted</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete all input objects</param>
    ///<returns>(Guid seq) of identifiers of newly created objects on success</returns>
    static member BooleanDifference(input0:Guid seq, input1:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def BooleanDifference(input0, input1, delete_input=True):
        '''Performs a boolean difference operation on two sets of input surfaces
        and polysurfaces. For more details, see the BooleanDifference command in
        the Rhino help file
        Parameters:
            input0 ([guid, ...]): list of surfaces to subtract from
            input1 ([guid, ...]): list of surfaces to be subtracted
            delete_input (bool, optional): delete all input objects
        Returns:
            list(guid, ...): of identifiers of newly created objects on success
            None: on error
        '''
        if type(input0) is list or type(input0) is tuple: pass
        else: input0 = [input0]
        
        if type(input1) is list or type(input1) is tuple: pass
        else: input1 = [input1]
        breps0 = [rhutil.coercebrep(id, True) for id in input0]
        breps1 = [rhutil.coercebrep(id, True) for id in input1]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        newbreps = Rhino.Geometry.Brep.CreateBooleanDifference(breps0, breps1, tolerance)
        if newbreps is None: return scriptcontext.errorhandler()
        
        rc = [scriptcontext.doc.Objects.AddBrep(brep) for brep in newbreps]
        if delete_input:
            for id in input0: scriptcontext.doc.Objects.Delete(id, True)
            for id in input1: scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Performs a boolean intersection operation on two sets of input surfaces
    ///  and polysurfaces. For more details, see the BooleanIntersection command in
    ///  the Rhino help file</summary>
    ///<param name="input0">(Guid seq) List of surfaces</param>
    ///<param name="input1">(Guid seq) List of surfaces</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete all input objects</param>
    ///<returns>(Guid seq) of identifiers of newly created objects on success</returns>
    static member BooleanIntersection(input0:Guid seq, input1:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def BooleanIntersection(input0, input1, delete_input=True):
        '''Performs a boolean intersection operation on two sets of input surfaces
        and polysurfaces. For more details, see the BooleanIntersection command in
        the Rhino help file
        Parameters:
            input0 ([guid, ...]): list of surfaces
            input1 ([guid, ...]): list of surfaces
            delete_input (bool, optional): delete all input objects
        Returns:
            list(guid, ...): of identifiers of newly created objects on success
            None: on error
        '''
        if type(input0) is list or type(input0) is tuple: pass
        else: input0 = [input0]
        
        if type(input1) is list or type(input1) is tuple: pass
        else: input1 = [input1]
        breps0 = [rhutil.coercebrep(id, True) for id in input0]
        breps1 = [rhutil.coercebrep(id, True) for id in input1]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        newbreps = Rhino.Geometry.Brep.CreateBooleanIntersection(breps0, breps1, tolerance)
        if newbreps is None: return scriptcontext.errorhandler()
        rc = [scriptcontext.doc.Objects.AddBrep(brep) for brep in newbreps]
        if delete_input:
            for id in input0: scriptcontext.doc.Objects.Delete(id, True)
            for id in input1: scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Performs a boolean union operation on a set of input surfaces and
    ///  polysurfaces. For more details, see the BooleanUnion command in the
    ///  Rhino help file</summary>
    ///<param name="input">(Guid seq) List of surfaces to union</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>true</c>
    ///Delete all input objects</param>
    ///<returns>(Guid seq) of identifiers of newly created objects on success</returns>
    static member BooleanUnion(input:Guid seq, [<OPT;DEF(true)>]deleteInput:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def BooleanUnion(input, delete_input=True):
        '''Performs a boolean union operation on a set of input surfaces and
        polysurfaces. For more details, see the BooleanUnion command in the
        Rhino help file
        Parameters:
            input ([guid, ...]): list of surfaces to union
            delete_input (bool, optional):  delete all input objects
        Returns:
            list(guid, ...): of identifiers of newly created objects on success
            None on error
        '''
        if len(input)<2: return scriptcontext.errorhandler()
        breps = [rhutil.coercebrep(id, True) for id in input]
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        newbreps = Rhino.Geometry.Brep.CreateBooleanUnion(breps, tolerance)
        if newbreps is None: return scriptcontext.errorhandler()
        
        rc = [scriptcontext.doc.Objects.AddBrep(brep) for brep in newbreps]
        if delete_input:
            for id in input: scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the point on a surface or polysurface that is closest to a test
    ///  point. This function works on both untrimmed and trimmed surfaces.</summary>
    ///<param name="objectId">(Guid) The object's identifier.</param>
    ///<param name="point">(Point3d) The test, or sampling point.</param>
    ///<returns>(Point3d * (float * float) * (float * float) * Vector3d) of closest point information . The list will
    ///  contain the following information:
    ///  Element     Type             Description
    ///    0        Point3d          The 3-D point at the parameter value of the
    ///      closest point.
    ///    1        (U, V)           Parameter values of closest point. Note, V
    ///      is 0 if the component index type is brep_edge
    ///      or brep_vertex.
    ///    2        (type, index)    The type and index of the brep component that
    ///      contains the closest point. Possible types are
    ///      brep_face, brep_edge or brep_vertex.
    ///    3        Vector3d         The normal to the brep_face, or the tangent
    ///      to the brep_edge.</returns>
    static member BrepClosestPoint(objectId:Guid, point:Point3d) : Point3d * (float * float) * (float * float) * Vector3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def BrepClosestPoint(object_id, point):
        '''Returns the point on a surface or polysurface that is closest to a test
        point. This function works on both untrimmed and trimmed surfaces.
        Parameters:
          object_id (guid): The object's identifier.
          point (point): The test, or sampling point.
        Returns:
          tuple(point, [number, number], [number, number], vector): of closest point information if successful. The list will
            contain the following information:
            Element     Type             Description
              0        Point3d          The 3-D point at the parameter value of the 
                                        closest point.
              1        (U, V)           Parameter values of closest point. Note, V 
                                        is 0 if the component index type is brep_edge
                                        or brep_vertex. 
              2        (type, index)    The type and index of the brep component that
                                        contains the closest point. Possible types are
                                        brep_face, brep_edge or brep_vertex.
              3        Vector3d         The normal to the brep_face, or the tangent
                                        to the brep_edge.  
          None: if not successful, or on error.
        '''
        brep = rhutil.coercebrep(object_id, True)
        point = rhutil.coerce3dpoint(point, True)
        rc = brep.ClosestPoint(point, 0.0)
        if rc[0]:
            type = int(rc[2].ComponentIndexType)
            index = rc[2].Index
            return rc[1], (rc[3], rc[4]), (type, index), rc[5]
    *)


    [<EXT>]
    ///<summary>Caps planar holes in a surface or polysurface</summary>
    ///<param name="surfaceId">(Guid) The identifier of the surface or polysurface to cap.</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member CapPlanarHoles(surfaceId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def CapPlanarHoles(surface_id):
        '''Caps planar holes in a surface or polysurface
        Parameters:
          surface_id (guid): The identifier of the surface or polysurface to cap.
        Returns:
          bool: True or False indicating success or failure
        '''
        brep = rhutil.coercebrep(surface_id, True)
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        newbrep = brep.CapPlanarHoles(tolerance)
        if newbrep:
            if newbrep.SolidOrientation == Rhino.Geometry.BrepSolidOrientation.Inward:
                newbrep.Flip()
            surface_id = rhutil.coerceguid(surface_id)
            if surface_id and scriptcontext.doc.Objects.Replace(surface_id, newbrep):
                scriptcontext.doc.Views.Redraw()
                return True
        return False
    *)


    [<EXT>]
    ///<summary>Duplicates the edge curves of a surface or polysurface. For more
    ///  information, see the Rhino help file for information on the DupEdge
    ///  command.</summary>
    ///<param name="objectId">(Guid) The identifier of the surface or polysurface object.</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///Select the duplicated edge curves. The default is not to select (False).</param>
    ///<returns>(Guid seq) identifying the newly created curve objects .</returns>
    static member DuplicateEdgeCurves(objectId:Guid, [<OPT;DEF(false)>]select:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def DuplicateEdgeCurves(object_id, select=False):
        '''Duplicates the edge curves of a surface or polysurface. For more
        information, see the Rhino help file for information on the DupEdge
        command.
        Parameters:
          object_id (guid): The identifier of the surface or polysurface object.
          select (bool, optional):  Select the duplicated edge curves. The default is not to select (False).
        Returns:
          list(guid, ..): identifying the newly created curve objects if successful.
          None: if not successful, or on error.
        '''
        brep = rhutil.coercebrep(object_id, True)
        out_curves = brep.DuplicateEdgeCurves()
        curves = []
        for curve in out_curves:
            if curve.IsValid:
                rc = scriptcontext.doc.Objects.AddCurve(curve)
                curve.Dispose()
                if rc==System.Guid.Empty: return None
                curves.append(rc)
                if select: rhobject.SelectObject(rc)
        if curves: scriptcontext.doc.Views.Redraw()
        return curves
    *)


    [<EXT>]
    ///<summary>Create curves that duplicate a surface or polysurface border</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface</param>
    ///<param name="typ">(int) Optional, Default Value: <c>0</c>
    ///The border curves to return
    ///  0=both exterior and interior,
    ///  1=exterior
    ///  2=interior</param>
    ///<returns>(Guid seq) list of curve ids on success</returns>
    static member DuplicateSurfaceBorder(surfaceId:Guid, [<OPT;DEF(0)>]typ:int) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def DuplicateSurfaceBorder(surface_id, type=0):
        '''Create curves that duplicate a surface or polysurface border
        Parameters:
          surface_id (guid): identifier of a surface
          type (number, optional): the border curves to return
             0=both exterior and interior,
             1=exterior
             2=interior
        Returns:
          list(guid, ...): list of curve ids on success
          None: on error
        '''
        brep = rhutil.coercebrep(surface_id, True)
        inner = type==0 or type==2
        outer = type==0 or type==1
        curves = brep.DuplicateNakedEdgeCurves(outer, inner)
        if curves is None: return scriptcontext.errorhandler()
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance * 2.1
        curves = Rhino.Geometry.Curve.JoinCurves(curves, tolerance)
        if curves is None: return scriptcontext.errorhandler()
        rc = [scriptcontext.doc.Objects.AddCurve(c) for c in curves]
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Evaluates a surface at a U,V parameter</summary>
    ///<param name="surfaceId">(Guid) The object's identifier.</param>
    ///<param name="u">(float * float) U of 'u, v parameters to evaluate.' (FIXME 0)</param>
    ///<param name="v">(float * float) V of 'u, v parameters to evaluate.' (FIXME 0)</param>
    ///<returns>(Point3d) a 3-D point</returns>
    static member EvaluateSurface(surfaceId:Guid, u:float * float, v:float * float) : Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def EvaluateSurface(surface_id, u, v):
        '''Evaluates a surface at a U,V parameter
        Parameters:
          surface_id (guid): the object's identifier.
          u, v ([number, number]): u, v parameters to evaluate.
        Returns:
          point: a 3-D point if successful
          None: if not successful
        '''
        surface = rhutil.coercesurface(surface_id, True)
        rc = surface.PointAt(u,v)
        if rc.IsValid: return rc
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Lengthens an untrimmed surface object</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface</param>
    ///<param name="parameter">(float * float) Tuple of two values definfing the U,V parameter to evaluate.
    ///  The surface edge closest to the U,V parameter will be the edge that is
    ///  extended</param>
    ///<param name="length">(float) Amount to extend to surface</param>
    ///<param name="smooth">(bool) Optional, Default Value: <c>true</c>
    ///If True, the surface is extended smoothly curving from the
    ///  edge. If False, the surface is extended in a straight line from the edge</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member ExtendSurface(surfaceId:Guid, parameter:float * float, length:float, [<OPT;DEF(true)>]smooth:bool) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExtendSurface(surface_id, parameter, length, smooth=True):
        '''Lengthens an untrimmed surface object
        Parameters:
          surface_id (guid): identifier of a surface
          parameter ([number, number]): tuple of two values definfing the U,V parameter to evaluate.
            The surface edge closest to the U,V parameter will be the edge that is
            extended
          length (number): amount to extend to surface
          smooth (bool, optional): If True, the surface is extended smoothly curving from the
            edge. If False, the surface is extended in a straight line from the edge
        Returns:
          bool: True or False indicating success or failure
        '''
        surface = rhutil.coercesurface(surface_id, True)
        edge = surface.ClosestSide(parameter[0], parameter[1])
        newsrf = surface.Extend(edge, length, smooth)
        if newsrf:
            surface_id = rhutil.coerceguid(surface_id)
            if surface_id: scriptcontext.doc.Objects.Replace(surface_id, newsrf)
            scriptcontext.doc.Views.Redraw()
        return newsrf is not None
    *)


    [<EXT>]
    ///<summary>Explodes, or unjoins, one or more polysurface objects. Polysurfaces
    ///  will be exploded into separate surfaces</summary>
    ///<param name="objectIds">(Guid seq) Identifiers of polysurfaces to explode</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input objects after exploding</param>
    ///<returns>(Guid seq) of identifiers of exploded pieces on success</returns>
    static member ExplodePolysurfaces(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExplodePolysurfaces(object_ids, delete_input=False):
        '''Explodes, or unjoins, one or more polysurface objects. Polysurfaces
        will be exploded into separate surfaces
        Parameters:
          object_ids ([guid, ...]): identifiers of polysurfaces to explode
          delete_input (bool, optional): delete input objects after exploding
        Returns:
          list(guid, ...): of identifiers of exploded pieces on success
        '''
        id = rhutil.coerceguid(object_ids, False)
        if id: object_ids = [id]
        ids = []
        for id in object_ids:
            brep = rhutil.coercebrep(id, True)
            if brep.Faces.Count>1:
                for i in range(brep.Faces.Count):
                    copyface = brep.Faces[i].DuplicateFace(False)
                    face_id = scriptcontext.doc.Objects.AddBrep(copyface)
                    if face_id!=System.Guid.Empty: ids.append(face_id)
                if delete_input: scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return ids
    *)


    [<EXT>]
    ///<summary>Extracts isoparametric curves from a surface</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface</param>
    ///<param name="parameter">(float * float) U,v parameter of the surface to evaluate</param>
    ///<param name="direction">(int) Direction to evaluate
    ///  0 = u
    ///  1 = v
    ///  2 = both</param>
    ///<returns>(Guid seq) of curve ids on success</returns>
    static member ExtractIsoCurve(surfaceId:Guid, parameter:float * float, direction:int) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExtractIsoCurve(surface_id, parameter, direction):
        '''Extracts isoparametric curves from a surface
        Parameters:
          surface_id (guid): identifier of a surface
          parameter ([number, number]): u,v parameter of the surface to evaluate
          direction (number): Direction to evaluate
            0 = u
            1 = v
            2 = both
        Returns:
          list(guid, ...): of curve ids on success
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        ids = []
        if direction==0 or direction==2:
            curves = None
            if type(surface) is Rhino.Geometry.BrepFace:
                curves = surface.TrimAwareIsoCurve(0, parameter[1])
            else:
                curves = [surface.IsoCurve(0,parameter[1])]
            if curves:
                for curve in curves:
                    id = scriptcontext.doc.Objects.AddCurve(curve)
                    if id!=System.Guid.Empty: ids.append(id)
        if direction==1 or direction==2:
            curves = None
            if type(surface) is Rhino.Geometry.BrepFace:
                curves = surface.TrimAwareIsoCurve(1, parameter[0])
            else:
                curves = [surface.IsoCurve(1,parameter[0])]
            if curves:
                for curve in curves:
                    id = scriptcontext.doc.Objects.AddCurve(curve)
                    if id!=System.Guid.Empty: ids.append(id)
        scriptcontext.doc.Views.Redraw()
        return ids
    *)


    [<EXT>]
    ///<summary>Separates or copies a surface or a copy of a surface from a polysurface</summary>
    ///<param name="objectId">(Guid) Polysurface identifier</param>
    ///<param name="faceIndices">(float seq) One or more numbers representing faces</param>
    ///<param name="copy">(bool) Optional, Default Value: <c>false</c>
    ///If True the faces are copied. If False, the faces are extracted</param>
    ///<returns>(Guid seq) identifiers of extracted surface objects on success</returns>
    static member ExtractSurface(objectId:Guid, faceIndices:float seq, [<OPT;DEF(false)>]copy:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExtractSurface(object_id, face_indices, copy=False):
        '''Separates or copies a surface or a copy of a surface from a polysurface
        Parameters:
          object_id (guid): polysurface identifier
          face_indices (number, ...): one or more numbers representing faces
          copy (bool, optional): If True the faces are copied. If False, the faces are extracted
        Returns:
          list(guid, ...): identifiers of extracted surface objects on success
        '''
        brep = rhutil.coercebrep(object_id, True)
        if hasattr(face_indices, "__getitem__"): pass
        else: face_indices = [face_indices]
        rc = []
        face_indices = sorted(face_indices, reverse=True)
        for index in face_indices:
            face = brep.Faces[index]
            newbrep = face.DuplicateFace(True)
            id = scriptcontext.doc.Objects.AddBrep(newbrep)
            rc.append(id)
        if not copy:
            for index in face_indices: brep.Faces.RemoveAt(index)
            id = rhutil.coerceguid(object_id)
            scriptcontext.doc.Objects.Replace(id, brep)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Creates a surface by extruding a curve along a path</summary>
    ///<param name="curveId">(Guid) Identifier of the curve to extrude</param>
    ///<param name="pathId">(Guid) Identifier of the path curve</param>
    ///<returns>(Guid) identifier of new surface on success</returns>
    static member ExtrudeCurve(curveId:Guid, pathId:Guid) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExtrudeCurve(curve_id, path_id):
        '''Creates a surface by extruding a curve along a path
        Parameters:
          curve_id (guid): identifier of the curve to extrude
          path_id (guid): identifier of the path curve
        Returns:
          guid: identifier of new surface on success
          None: on error
        '''
        curve1 = rhutil.coercecurve(curve_id, -1, True)
        curve2 = rhutil.coercecurve(path_id, -1, True)
        srf = Rhino.Geometry.SumSurface.Create(curve1, curve2)
        rc = scriptcontext.doc.Objects.AddSurface(srf)
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Creates a surface by extruding a curve to a point</summary>
    ///<param name="curveId">(Guid) Identifier of the curve to extrude</param>
    ///<param name="point">(Point3d) 3D point</param>
    ///<returns>(Guid) identifier of new surface on success</returns>
    static member ExtrudeCurvePoint(curveId:Guid, point:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExtrudeCurvePoint(curve_id, point):
        '''Creates a surface by extruding a curve to a point
        Parameters:
          curve_id (guid): identifier of the curve to extrude
          point (point): 3D point
        Returns:
          guid: identifier of new surface on success
          None: on error
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        point = rhutil.coerce3dpoint(point, True)
        srf = Rhino.Geometry.Surface.CreateExtrusionToPoint(curve, point)
        rc = scriptcontext.doc.Objects.AddSurface(srf)
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Create surface by extruding a curve along two points that define a line</summary>
    ///<param name="curveId">(Guid) Identifier of the curve to extrude</param>
    ///<param name="startPoint">(Point3d) Start point of '3D points that specify distance and direction' (FIXME 0)</param>
    ///<param name="endePoint">(Point3d) End point of '3D points that specify distance and direction' (FIXME 0)</param>
    ///<returns>(Guid) identifier of new surface on success</returns>
    static member ExtrudeCurveStraight(curveId:Guid, startPoint:Point3d, endePoint:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExtrudeCurveStraight(curve_id, start_point, end_point):
        '''Create surface by extruding a curve along two points that define a line
        Parameters:
          curve_id (guid): identifier of the curve to extrude
          start_point, end_point (point): 3D points that specify distance and direction
        Returns:
          guid: identifier of new surface on success
          None: on error
        '''
        curve = rhutil.coercecurve(curve_id, -1, True)
        start_point = rhutil.coerce3dpoint(start_point, True)
        end_point = rhutil.coerce3dpoint(end_point, True)
        vec = end_point - start_point
        srf = Rhino.Geometry.Surface.CreateExtrusion(curve, vec)
        rc = scriptcontext.doc.Objects.AddSurface(srf)
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Create surface by extruding along a path curve</summary>
    ///<param name="surface">(Guid) Identifier of the surface to extrude</param>
    ///<param name="curve">(Guid) Identifier of the path curve</param>
    ///<param name="cap">(bool) Optional, Default Value: <c>true</c>
    ///Extrusion is capped at both ends</param>
    ///<returns>(Guid) identifier of new surface on success</returns>
    static member ExtrudeSurface(surface:Guid, curve:Guid, [<OPT;DEF(true)>]cap:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def ExtrudeSurface(surface, curve, cap=True):
        '''Create surface by extruding along a path curve
        Parameters:
          surface (guid): identifier of the surface to extrude
          curve (guid): identifier of the path curve
          cap (bool, optional): extrusion is capped at both ends
        Returns:
          guid: identifier of new surface on success
        '''
        brep = rhutil.coercebrep(surface, True)
        curve = rhutil.coercecurve(curve, -1, True)
        newbrep = brep.Faces[0].CreateExtrusion(curve, cap)
        if newbrep:
            rc = scriptcontext.doc.Objects.AddBrep(newbrep)
            scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Create constant radius rolling ball fillets between two surfaces. Note,
    ///  this function does not trim the original surfaces of the fillets</summary>
    ///<param name="surface0">(Guid) Surface0 of 'identifiers of first and second surface' (FIXME 0)</param>
    ///<param name="surface1">(Guid) Surface1 of 'identifiers of first and second surface' (FIXME 0)</param>
    ///<param name="radius">(float) A positive fillet radius</param>
    ///<param name="uvparam0">(float * float) Optional, Default Value: <c>null:float * float</c>
    ///A u,v surface parameter of surface0 near where the fillet
    ///  is expected to hit the surface</param>
    ///<param name="uvparam1">(float * float) Optional, Default Value: <c>null:float * float</c>
    ///Same as uvparam0, but for surface1</param>
    ///<returns>(Guid) ids of surfaces created on success</returns>
    static member FilletSurfaces(surface0:Guid, surface1:Guid, radius:float, [<OPT;DEF(null)>]uvparam0:float * float, [<OPT;DEF(null)>]uvparam1:float * float) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def FilletSurfaces(surface0, surface1, radius, uvparam0=None, uvparam1=None):
        '''Create constant radius rolling ball fillets between two surfaces. Note,
        this function does not trim the original surfaces of the fillets
        Parameters:
          surface0, surface1 (guid): identifiers of first and second surface
          radius (number): a positive fillet radius
          uvparam0 ([number, number], optional): a u,v surface parameter of surface0 near where the fillet
            is expected to hit the surface
          uvparam1([number, number], optional): same as uvparam0, but for surface1
        Returns:
          guid: ids of surfaces created on success
          None: on error
        '''
        surface0 = rhutil.coercesurface(surface0, True)
        surface1 = rhutil.coercesurface(surface1, True)
        if uvparam0 is not None and uvparam1 is not None:   #SR9 error: "Could not convert None to a Point2d"
            uvparam0 = rhutil.coerce2dpoint(uvparam0, True)
            uvparam1 = rhutil.coerce2dpoint(uvparam1, True)
        surfaces = None
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        if uvparam0 and uvparam1:
            surfaces = Rhino.Geometry.Surface.CreateRollingBallFillet(surface0, uvparam0, surface1, uvparam1, radius, tol)
        else:
            surfaces = Rhino.Geometry.Surface.CreateRollingBallFillet(surface0, surface1, radius, tol)
        if not surfaces: return scriptcontext.errorhandler()
        rc = []
        for surf in surfaces:
            rc.append( scriptcontext.doc.Objects.AddSurface(surf) )
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the normal direction of a surface. This feature can
    /// also be found in Rhino's Dir command</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface object</param>
    ///<returns>(Vector3d) The current normal orientation</returns>
    static member FlipSurface(surfaceId:Guid) : Vector3d = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def FlipSurface(surface_id, flip=None):
        '''Returns or changes the normal direction of a surface. This feature can
        also be found in Rhino's Dir command
        Parameters:
          surface_id (guid): identifier of a surface object
          flip (bool, optional) new normal orientation, either flipped(True) or not flipped (False).
        Returns:
          vector: if flipped is not specified, the current normal orientation
          vector: if flipped is specified, the previous normal orientation
          None: on error
        '''
        brep = rhutil.coercebrep(surface_id, True)
        if brep.Faces.Count>1: return scriptcontext.errorhandler()
        face = brep.Faces[0]
        old_reverse = face.OrientationIsReversed
        if flip!=None and brep.IsSolid==False and old_reverse!=flip:
            brep.Flip()
            surface_id = rhutil.coerceguid(surface_id)
            if surface_id: scriptcontext.doc.Objects.Replace(surface_id, brep)
            scriptcontext.doc.Views.Redraw()
        return old_reverse
    *)

    ///<summary>Changes the normal direction of a surface. This feature can
    /// also be found in Rhino's Dir command</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface object</param>
    ///<param name="flip">(bool)New normal orientation, either flipped(True) or not flipped (False).</param>
    ///<returns>(unit) void, nothing</returns>
    static member FlipSurface(surfaceId:Guid, flip:bool) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def FlipSurface(surface_id, flip=None):
        '''Returns or changes the normal direction of a surface. This feature can
        also be found in Rhino's Dir command
        Parameters:
          surface_id (guid): identifier of a surface object
          flip (bool, optional) new normal orientation, either flipped(True) or not flipped (False).
        Returns:
          vector: if flipped is not specified, the current normal orientation
          vector: if flipped is specified, the previous normal orientation
          None: on error
        '''
        brep = rhutil.coercebrep(surface_id, True)
        if brep.Faces.Count>1: return scriptcontext.errorhandler()
        face = brep.Faces[0]
        old_reverse = face.OrientationIsReversed
        if flip!=None and brep.IsSolid==False and old_reverse!=flip:
            brep.Flip()
            surface_id = rhutil.coerceguid(surface_id)
            if surface_id: scriptcontext.doc.Objects.Replace(surface_id, brep)
            scriptcontext.doc.Views.Redraw()
        return old_reverse
    *)


    [<EXT>]
    ///<summary>Intersects a brep object with another brep object. Note, unlike the
    ///  SurfaceSurfaceIntersection function this function works on trimmed surfaces.</summary>
    ///<param name="brep1">(Guid) Identifier of first brep object</param>
    ///<param name="brep2">(Guid) Identifier of second brep object</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///Distance tolerance at segment midpoints. If omitted,
    ///  the current absolute tolerance is used.</param>
    ///<returns>(Guid seq) identifying the newly created intersection curve and point objects .</returns>
    static member IntersectBreps(brep1:Guid, brep2:Guid, [<OPT;DEF(0.0)>]tolerance:float) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def IntersectBreps(brep1, brep2, tolerance=None):
        '''Intersects a brep object with another brep object. Note, unlike the
        SurfaceSurfaceIntersection function this function works on trimmed surfaces.
        Parameters:
          brep1 (guid): identifier of first brep object
          brep2 (guid): identifier of second brep object
          tolerance (number): Distance tolerance at segment midpoints. If omitted,
                      the current absolute tolerance is used.
        Returns:
          list(guid, ...): identifying the newly created intersection curve and point objects if successful.
          None: if not successful, or on error.
        '''
        brep1 = rhutil.coercebrep(brep1, True)
        brep2 = rhutil.coercebrep(brep2, True)
        if tolerance is None or tolerance < 0.0:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        rc = Rhino.Geometry.Intersect.Intersection.BrepBrep(brep1, brep2, tolerance)
        if not rc[0]: return None
        out_curves = rc[1]
        out_points = rc[2]
        merged_curves = Rhino.Geometry.Curve.JoinCurves(out_curves, 2.1 * tolerance)
        
        ids = []
        if merged_curves:
            for curve in merged_curves:
                if curve.IsValid:
                    rc = scriptcontext.doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc==System.Guid.Empty: return scriptcontext.errorhandler()
                    ids.append(rc)
        else:
            for curve in out_curves:
                if curve.IsValid:
                    rc = scriptcontext.doc.Objects.AddCurve(curve)
                    curve.Dispose()
                    if rc==System.Guid.Empty: return scriptcontext.errorhandler()
                    ids.append(rc)
        for point in out_points:
            rc = scriptcontext.doc.Objects.AddPoint(point)
            if rc==System.Guid.Empty: return scriptcontext.errorhandler()
            ids.append(rc)
        if ids:
            scriptcontext.doc.Views.Redraw()
            return ids
    *)


    [<EXT>]
    ///<summary>Calculates intersections of two spheres</summary>
    ///<param name="spherePlane0">(Plane) An equatorial plane of the first sphere. The origin of the
    ///  plane will be the center point of the sphere</param>
    ///<param name="sphereRadius0">(float) Radius of the first sphere</param>
    ///<param name="spherePlane1">(Plane) Plane for second sphere</param>
    ///<param name="sphereRadius1">(float) Radius for second sphere</param>
    ///<returns>(float * Point3d * float) of intersection results
    ///  [0] = type of intersection (0=point, 1=circle, 2=spheres are identical)
    ///  [1] = Point of intersection or plane of circle intersection
    ///  [2] = radius of circle if circle intersection</returns>
    static member IntersectSpheres(spherePlane0:Plane, sphereRadius0:float, spherePlane1:Plane, sphereRadius1:float) : float * Point3d * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def IntersectSpheres(sphere_plane0, sphere_radius0, sphere_plane1, sphere_radius1):
        '''Calculates intersections of two spheres
        Parameters:
          sphere_plane0 (plane): an equatorial plane of the first sphere. The origin of the
            plane will be the center point of the sphere
          sphere_radius0 (number): radius of the first sphere
          sphere_plane1 (plane): plane for second sphere
          sphere_radius1 (number): radius for second sphere
        Returns:
          list(number, point, number): of intersection results
            [0] = type of intersection (0=point, 1=circle, 2=spheres are identical)
            [1] = Point of intersection or plane of circle intersection
            [2] = radius of circle if circle intersection
          None: on error
        '''
        plane0 = rhutil.coerceplane(sphere_plane0, True)
        plane1 = rhutil.coerceplane(sphere_plane1, True)
        sphere0 = Rhino.Geometry.Sphere(plane0, sphere_radius0)
        sphere1 = Rhino.Geometry.Sphere(plane1, sphere_radius1)
        rc, circle = Rhino.Geometry.Intersect.Intersection.SphereSphere(sphere0, sphere1)
        if rc==Rhino.Geometry.Intersect.SphereSphereIntersection.Point:
            return [0, circle.Center]
        if rc==Rhino.Geometry.Intersect.SphereSphereIntersection.Circle:
            return [1, circle.Plane, circle.Radius]
        if rc==Rhino.Geometry.Intersect.SphereSphereIntersection.Overlap:
            return [2]
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Verifies an object is a Brep, or a boundary representation model, object.</summary>
    ///<param name="objectId">(Guid) The object's identifier.</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsBrep(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsBrep(object_id):
        '''Verifies an object is a Brep, or a boundary representation model, object.
        Parameters:
          object_id (guid): The object's identifier.
        Returns:
          bool: True if successful, otherwise False.
          None: on error.
        '''
        return rhutil.coercebrep(object_id)!=None
    *)


    [<EXT>]
    ///<summary>Determines if a surface is a portion of a cone</summary>
    ///<param name="objectId">(Guid) The surface object's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsCone(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsCone(object_id):
        '''Determines if a surface is a portion of a cone
        Parameters:
          object_id (guid): the surface object's identifier
        Returns:
          bool: True if successful, otherwise False
        '''
        surface = rhutil.coercesurface(object_id, True)
        return surface.IsCone()
    *)


    [<EXT>]
    ///<summary>Determines if a surface is a portion of a cone</summary>
    ///<param name="objectId">(Guid) The cylinder object's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsCylinder(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsCylinder(object_id):
        '''Determines if a surface is a portion of a cone
        Parameters:
          object_id (guid): the cylinder object's identifier
        Returns:
          bool: True if successful, otherwise False
        '''
        surface = rhutil.coercesurface(object_id, True)
        return surface.IsCylinder()
    *)


    [<EXT>]
    ///<summary>Verifies an object is a plane surface. Plane surfaces can be created by
    ///  the Plane command. Note, a plane surface is not a planar NURBS surface</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsPlaneSurface(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsPlaneSurface(object_id):
        '''Verifies an object is a plane surface. Plane surfaces can be created by
        the Plane command. Note, a plane surface is not a planar NURBS surface
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if successful, otherwise False
        '''
        face = rhutil.coercesurface(object_id, True)
        if type(face) is Rhino.Geometry.BrepFace and face.IsSurface:
            return type(face.UnderlyingSurface()) is Rhino.Geometry.PlaneSurface
        return False
    *)


    [<EXT>]
    ///<summary>Verifies that a point is inside a closed surface or polysurface</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="point">(Point3d) The test, or sampling point</param>
    ///<param name="strictlyIn">(bool) Optional, Default Value: <c>false</c>
    ///If true, the test point must be inside by at least tolerance</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///Distance tolerance used for intersection and determining
    ///  strict inclusion. If omitted, Rhino's internal tolerance is used</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsPointInSurface(objectId:Guid, point:Point3d, [<OPT;DEF(false)>]strictlyIn:bool, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsPointInSurface(object_id, point, strictly_in=False, tolerance=None):
        '''Verifies that a point is inside a closed surface or polysurface
        Parameters:
          object_id (guid): the object's identifier
          point (point): The test, or sampling point
          strictly_in (bool, optional): If true, the test point must be inside by at least tolerance
          tolerance (number, optional): distance tolerance used for intersection and determining
            strict inclusion. If omitted, Rhino's internal tolerance is used
        Returns:
          bool: True if successful, otherwise False
        '''
        object_id = rhutil.coerceguid(object_id, True)
        point = rhutil.coerce3dpoint(point, True)
        if object_id==None or point==None: return scriptcontext.errorhandler()
        obj = scriptcontext.doc.Objects.Find(object_id)
        if tolerance is None: tolerance = Rhino.RhinoMath.SqrtEpsilon
        brep = None
        if type(obj)==Rhino.DocObjects.ExtrusionObject:
            brep = obj.ExtrusionGeometry.ToBrep(False)
        elif type(obj)==Rhino.DocObjects.BrepObject:
            brep = obj.BrepGeometry
        elif hasattr(obj, "Geometry"):
            brep = obj.Geometry
        return brep.IsPointInside(point, tolerance, strictly_in)
    *)


    [<EXT>]
    ///<summary>Verifies that a point lies on a surface</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<param name="point">(Point3d) The test, or sampling point</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsPointOnSurface(objectId:Guid, point:Point3d) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsPointOnSurface(object_id, point):
        '''Verifies that a point lies on a surface
        Parameters:
          object_id (guid): the object's identifier
          point (point): The test, or sampling point
        Returns:
          bool: True if successful, otherwise False
        '''
        surf = rhutil.coercesurface(object_id, True)
        point = rhutil.coerce3dpoint(point, True)
        rc, u, v = surf.ClosestPoint(point)
        if rc:
            srf_pt = surf.PointAt(u,v)
            if srf_pt.DistanceTo(point)>scriptcontext.doc.ModelAbsoluteTolerance:
                rc = False
            else:
                rc = surf.IsPointOnFace(u,v) != Rhino.Geometry.PointFaceRelation.Exterior
        return rc
    *)


    [<EXT>]
    ///<summary>Verifies an object is a polysurface. Polysurfaces consist of two or more
    ///  surfaces joined together. If the polysurface fully encloses a volume, it is
    ///  considered a solid.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True is successful, otherwise False</returns>
    static member IsPolysurface(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsPolysurface(object_id):
        '''Verifies an object is a polysurface. Polysurfaces consist of two or more
        surfaces joined together. If the polysurface fully encloses a volume, it is
        considered a solid.
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True is successful, otherwise False
        '''
        brep = rhutil.coercebrep(object_id)
        if brep is None: return False
        return brep.Faces.Count>1
    *)


    [<EXT>]
    ///<summary>Verifies a polysurface object is closed. If the polysurface fully encloses
    ///  a volume, it is considered a solid.</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True is successful, otherwise False</returns>
    static member IsPolysurfaceClosed(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsPolysurfaceClosed(object_id):
        '''Verifies a polysurface object is closed. If the polysurface fully encloses
        a volume, it is considered a solid.
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True is successful, otherwise False
        '''
        brep = rhutil.coercebrep(object_id, True)
        return brep.IsSolid
    *)


    [<EXT>]
    ///<summary>Determines if a surface is a portion of a sphere</summary>
    ///<param name="objectId">(Guid) The object's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsSphere(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsSphere(object_id):
        '''Determines if a surface is a portion of a sphere
        Parameters:
          object_id (guid): the object's identifier
        Returns:
          bool: True if successful, otherwise False
        '''
        surface = rhutil.coercesurface(object_id, True)
        return surface.IsSphere()
    *)


    [<EXT>]
    ///<summary>Verifies an object is a surface. Brep objects with only one face are
    ///  also considered surfaces.</summary>
    ///<param name="objectId">(Guid) The object's identifier.</param>
    ///<returns>(bool) True , otherwise False.</returns>
    static member IsSurface(objectId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsSurface(object_id):
        '''Verifies an object is a surface. Brep objects with only one face are
        also considered surfaces.
        Parameters:
          object_id (guid): the object's identifier.
        Returns:
          bool: True if successful, otherwise False.
        '''
        brep = rhutil.coercebrep(object_id)
        if brep and brep.Faces.Count==1: return True
        surface = rhutil.coercesurface(object_id)
        return (surface!=None)
    *)


    [<EXT>]
    ///<summary>Verifies a surface object is closed in the specified direction.  If the
    ///  surface fully encloses a volume, it is considered a solid</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface</param>
    ///<param name="direction">(int) 0=U direction check, 1=V direction check</param>
    ///<returns>(bool) True or False</returns>
    static member IsSurfaceClosed(surfaceId:Guid, direction:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsSurfaceClosed( surface_id, direction ):
        '''Verifies a surface object is closed in the specified direction.  If the
        surface fully encloses a volume, it is considered a solid
        Parameters:
          surface_id (guid): identifier of a surface
          direction (number): 0=U direction check, 1=V direction check
        Returns:
          bool: True or False
        '''
        surface = rhutil.coercesurface(surface_id, True)
        return surface.IsClosed(direction)
    *)


    [<EXT>]
    ///<summary>Verifies a surface object is periodic in the specified direction.</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface</param>
    ///<param name="direction">(int) 0=U direction check, 1=V direction check</param>
    ///<returns>(bool) True or False</returns>
    static member IsSurfacePeriodic(surfaceId:Guid, direction:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsSurfacePeriodic(surface_id, direction):
        '''Verifies a surface object is periodic in the specified direction.
        Parameters:
          surface_id (guid): identifier of a surface
          direction (number): 0=U direction check, 1=V direction check
        Returns:
          bool: True or False
        '''
        surface = rhutil.coercesurface(surface_id, True)
        return surface.IsPeriodic(direction)
    *)


    [<EXT>]
    ///<summary>Verifies a surface object is planar</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///Tolerance used when checked. If omitted, the current absolute
    ///  tolerance is used</param>
    ///<returns>(bool) True or False</returns>
    static member IsSurfacePlanar(surfaceId:Guid, [<OPT;DEF(0.0)>]tolerance:float) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsSurfacePlanar(surface_id, tolerance=None):
        '''Verifies a surface object is planar
        Parameters:
          surface_id (guid): identifier of a surface
          tolerance (number): tolerance used when checked. If omitted, the current absolute
            tolerance is used
        Returns:
          bool: True or False
        '''
        surface = rhutil.coercesurface(surface_id, True)
        if tolerance is None:
            tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        return surface.IsPlanar(tolerance)
    *)


    [<EXT>]
    ///<summary>Verifies a surface object is rational</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsSurfaceRational(surfaceId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsSurfaceRational(surface_id):
        '''Verifies a surface object is rational
        Parameters:
          surface_id (guid): the surface's identifier
        Returns:
          bool: True if successful, otherwise False
        '''
        surface = rhutil.coercesurface(surface_id, True)
        ns = surface.ToNurbsSurface()
        if ns is None: return False
        return ns.IsRational
    *)


    [<EXT>]
    ///<summary>Verifies a surface object is singular in the specified direction.
    ///  Surfaces are considered singular if a side collapses to a point.</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="direction">(int) 0=south
    ///  1=east
    ///  2=north
    ///  3=west</param>
    ///<returns>(bool) True or False</returns>
    static member IsSurfaceSingular(surfaceId:Guid, direction:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsSurfaceSingular(surface_id, direction):
        '''Verifies a surface object is singular in the specified direction.
        Surfaces are considered singular if a side collapses to a point.
        Parameters:
          surface_id (guid): the surface's identifier
          direction (number):
            0=south
            1=east
            2=north
            3=west
        Returns:
          bool: True or False
        '''
        surface = rhutil.coercesurface(surface_id, True)
        return surface.IsSingular(direction)
    *)


    [<EXT>]
    ///<summary>Verifies a surface object has been trimmed</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<returns>(bool) True or False</returns>
    static member IsSurfaceTrimmed(surfaceId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsSurfaceTrimmed(surface_id):
        '''Verifies a surface object has been trimmed
        Parameters:
          surface_id (guid): the surface's identifier
        Returns:
          bool: True or False
        '''
        brep = rhutil.coercebrep(surface_id, True)
        return not brep.IsSurface
    *)


    [<EXT>]
    ///<summary>Determines if a surface is a portion of a torus</summary>
    ///<param name="surfaceId">(Guid) The surface object's identifier</param>
    ///<returns>(bool) True , otherwise False</returns>
    static member IsTorus(surfaceId:Guid) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def IsTorus(surface_id):
        '''Determines if a surface is a portion of a torus
        Parameters:
          surface_id (guid): the surface object's identifier
        Returns:
          bool: True if successful, otherwise False
        '''
        surface = rhutil.coercesurface(surface_id, True)
        return surface.IsTorus()
    *)


    [<EXT>]
    ///<summary>Gets the sphere definition from a surface, if possible.</summary>
    ///<param name="surfaceId">(Guid) The identifier of the surface object</param>
    ///<returns>(Plane * float) The equatorial plane of the sphere, and its radius.</returns>
    static member SurfaceSphere(surfaceId:Guid) : Plane * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceSphere(surface_id):
        '''Gets the sphere definition from a surface, if possible.
        Parameters:
          surface_id (guid): the identifier of the surface object
        Returns:
          (plane, number): The equatorial plane of the sphere, and its radius.
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        is_sphere, sphere = surface.TryGetSphere(tol)
        rc = None
        if is_sphere: rc = (sphere.EquatorialPlane, sphere.Radius)
        return rc
    *)


    [<EXT>]
    ///<summary>Joins two or more surface or polysurface objects together to form one
    ///  polysurface object</summary>
    ///<param name="objectIds">(Guid seq) List of object identifiers</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete the original surfaces</param>
    ///<returns>(Guid) identifier of newly created object on success</returns>
    static member JoinSurfaces(objectIds:Guid seq, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def JoinSurfaces(object_ids, delete_input=False):
        '''Joins two or more surface or polysurface objects together to form one
        polysurface object
        Parameters:
          object_ids ([guid, ...]) list of object identifiers
          delete_input (bool, optional): Delete the original surfaces
        Returns:
          guid: identifier of newly created object on success
          None: on failure
        '''
        breps = [rhutil.coercebrep(id, True) for id in object_ids]
        if len(breps)<2: return scriptcontext.errorhandler()
        tol = scriptcontext.doc.ModelAbsoluteTolerance * 2.1
        joinedbreps = Rhino.Geometry.Brep.JoinBreps(breps, tol)
        if joinedbreps is None or len(joinedbreps)!=1:
            return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddBrep(joinedbreps[0])
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        if delete_input:
            for id in object_ids:
                id = rhutil.coerceguid(id)
                scriptcontext.doc.Objects.Delete(id, True)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    //(FIXME) VarOutTypes
    ///<summary>Makes an existing surface a periodic NURBS surface</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="direction">(int) The direction to make periodic, either 0=U or 1=V</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete the input surface</param>
    ///<returns>(Guid) if delete_input is False, identifier of the new surface</returns>
    static member MakeSurfacePeriodic(surfaceId:Guid, direction:int, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def MakeSurfacePeriodic(surface_id, direction, delete_input=False):
        '''Makes an existing surface a periodic NURBS surface
        Parameters:
          surface_id (guid): the surface's identifier
          direction (number): The direction to make periodic, either 0=U or 1=V
          delete_input (bool, optional): delete the input surface
        Returns:
          guid: if delete_input is False, identifier of the new surface
          guid: if delete_input is True, identifier of the modifier surface
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        newsurf = Rhino.Geometry.Surface.CreatePeriodicSurface(surface, direction)
        if newsurf is None: return scriptcontext.errorhandler()
        id = rhutil.coerceguid(surface_id)
        if delete_input:
            scriptcontext.doc.Objects.Replace(id, newsurf)
        else:
            id = scriptcontext.doc.Objects.AddSurface(newsurf)
        scriptcontext.doc.Views.Redraw()
        return id
    *)


    [<EXT>]
    ///<summary>Offsets a trimmed or untrimmed surface by a distance. The offset surface
    ///  will be added to Rhino.</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="distance">(float) The distance to offset</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///The offset tolerance. Use 0.0 to make a loose offset. Otherwise, the
    ///  document's absolute tolerance is usually sufficient.</param>
    ///<param name="bothSides">(bool) Optional, Default Value: <c>false</c>
    ///Offset to both sides of the input surface</param>
    ///<param name="createSolid">(bool) Optional, Default Value: <c>false</c>
    ///Make a solid object</param>
    ///<returns>(Guid) identifier of the new object</returns>
    static member OffsetSurface(surfaceId:Guid, distance:float, [<OPT;DEF(0.0)>]tolerance:float, [<OPT;DEF(false)>]bothSides:bool, [<OPT;DEF(false)>]createSolid:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def OffsetSurface(surface_id, distance, tolerance=None, both_sides=False, create_solid=False):
        '''Offsets a trimmed or untrimmed surface by a distance. The offset surface
        will be added to Rhino.
        Parameters:
          surface_id (guid): the surface's identifier
          distance (number): the distance to offset
          tolerance (number, optional): The offset tolerance. Use 0.0 to make a loose offset. Otherwise, the
            document's absolute tolerance is usually sufficient.
          both_sides (bool, optional): Offset to both sides of the input surface
          create_solid (bool, optional): Make a solid object
        Returns:
          guid: identifier of the new object if successful
          None: on error
        '''
        brep = rhutil.coercebrep(surface_id, True)
        face = None
        if (1 == brep.Faces.Count): face = brep.Faces[0]
        if face is None: return scriptcontext.errorhandler()
        if tolerance is None: tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        newbrep = Rhino.Geometry.Brep.CreateFromOffsetFace(face, distance, tolerance, both_sides, create_solid)
        if newbrep is None: return scriptcontext.errorhandler()
        rc = scriptcontext.doc.Objects.AddBrep(newbrep)
        if rc==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Pulls a curve object to a surface object</summary>
    ///<param name="surface">(Guid) The surface's identifier</param>
    ///<param name="curve">(Guid) The curve's identifier</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Should the input items be deleted</param>
    ///<returns>(Guid seq) of new curves</returns>
    static member PullCurve(surface:Guid, curve:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def PullCurve(surface, curve, delete_input=False):
        '''Pulls a curve object to a surface object
        Parameters:
          surface (guid): the surface's identifier
          curve (guid): the curve's identifier
          delete_input (bool, optional) should the input items be deleted
        Returns:
          list(guid, ...): of new curves if successful
          None: on error
        '''
        crvobj = rhutil.coercerhinoobject(curve, True, True)
        brep = rhutil.coercebrep(surface, True)
        curve = rhutil.coercecurve(curve, -1, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        curves = Rhino.Geometry.Curve.PullToBrepFace(curve, brep.Faces[0], tol)
        rc = [scriptcontext.doc.Objects.AddCurve(curve) for curve in curves]
        if rc:
            if delete_input and crvobj:
                scriptcontext.doc.Objects.Delete(crvobj, True)
            scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    ///<summary>Rebuilds a surface to a given degree and control point count. For more
    ///  information see the Rhino help file for the Rebuild command</summary>
    ///<param name="objectId">(Guid) The surface's identifier</param>
    ///<param name="degree">(int * int) Optional, Default Value: <c>3*3</c>
    ///Two numbers that identify surface degree in both U and V directions</param>
    ///<param name="pointcount">(int * int) Optional, Default Value: <c>10*10</c>
    ///Two numbers that identify the surface point count in both the U and V directions</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RebuildSurface(objectId:Guid, [<OPT;DEF(null)>]degree:int * int, [<OPT;DEF(null)>]pointcount:int * int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def RebuildSurface(object_id, degree=(3,3), pointcount=(10,10)):
        '''Rebuilds a surface to a given degree and control point count. For more
        information see the Rhino help file for the Rebuild command
        Parameters:
          object_id (guid): the surface's identifier
          degree ([number, number], optional): two numbers that identify surface degree in both U and V directions
          pointcount ([number, number], optional): two numbers that identify the surface point count in both the U and V directions
        Returns:
          bool: True of False indicating success or failure
        '''
        surface = rhutil.coercesurface(object_id, True)
        newsurf = surface.Rebuild( degree[0], degree[1], pointcount[0], pointcount[1] )
        if newsurf is None: return False
        object_id = rhutil.coerceguid(object_id)
        rc = scriptcontext.doc.Objects.Replace(object_id, newsurf)
        if rc: scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Deletes a knot from a surface object.</summary>
    ///<param name="surface">(Guid) The reference of the surface object</param>
    ///<param name="uvParameter">(float * float) ): An indexable item containing a U,V parameter on the surface. List, tuples and UVIntervals will work.
    ///  Note, if the parameter is not equal to one of the existing knots, then the knot closest to the specified parameter will be removed.</param>
    ///<param name="vDirection">(bool) If True, or 1, the V direction will be addressed. If False, or 0, the U direction.</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member RemoveSurfaceKnot(surface:Guid, uvParameter:float * float, vDirection:bool) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def RemoveSurfaceKnot(surface, uv_parameter, v_direction):
        '''Deletes a knot from a surface object.
        Parameters:
          surface (guid): The reference of the surface object
          uv_parameter (list(number, number)): An indexable item containing a U,V parameter on the surface. List, tuples and UVIntervals will work.
            Note, if the parameter is not equal to one of the existing knots, then the knot closest to the specified parameter will be removed.
          v_direction (bool): if True, or 1, the V direction will be addressed. If False, or 0, the U direction.
        Returns:
          bool: True of False indicating success or failure
        '''
        srf_inst = rhutil.coercesurface(surface, True)
        u_param = uv_parameter[0]
        v_param = uv_parameter[1]
        success, n_u_param, n_v_param = srf_inst.GetSurfaceParameterFromNurbsFormParameter(u_param, v_param)
        if not success: return False
        n_srf = srf_inst.ToNurbsSurface()
        if not n_srf: return False
        knots = n_srf.KnotsV if v_direction else n_srf.KnotsU
        success = knots.RemoveKnotsAt(n_u_param, n_v_param)
        if not success: return False
        scriptcontext.doc.Objects.Replace(surface, n_srf)
        scriptcontext.doc.Views.Redraw()
        return True
    *)


    [<EXT>]
    ///<summary>Reverses U or V directions of a surface, or swaps (transposes) U and V
    ///  directions.</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface object</param>
    ///<param name="direction">(int) As a bit coded flag to swap
    ///  1 = reverse U
    ///  2 = reverse V
    ///  4 = transpose U and V (values can be combined)</param>
    ///<returns>(bool) indicating success or failure</returns>
    static member ReverseSurface(surfaceId:Guid, direction:int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def ReverseSurface(surface_id, direction):
        '''Reverses U or V directions of a surface, or swaps (transposes) U and V
        directions.
        Parameters:
          surface_id (guid): identifier of a surface object
          direction (number): as a bit coded flag to swap
                1 = reverse U
                2 = reverse V
                4 = transpose U and V (values can be combined)
        Returns:
          bool: indicating success or failure
          None: on error
        '''
        brep = rhutil.coercebrep(surface_id, True)
        if not brep.Faces.Count==1: return scriptcontext.errorhandler()
        face = brep.Faces[0]
        if direction & 1:
            face.Reverse(0, True)
        if direction & 2:
            face.Reverse(1, True)
        if direction & 4:
            face.Transpose(True)
        scriptcontext.doc.Objects.Replace(surface_id, brep)
        scriptcontext.doc.Views.Redraw()
        return True
    *)


    [<EXT>]
    ///<summary>Shoots a ray at a collection of surfaces</summary>
    ///<param name="surfaceIds">(Guid seq) One of more surface identifiers</param>
    ///<param name="startPoint">(Point3d) Starting point of the ray</param>
    ///<param name="direction">(Vector3d) Vector identifying the direction of the ray</param>
    ///<param name="reflections">(float) Optional, Default Value: <c>10</c>
    ///The maximum number of times the ray will be reflected</param>
    ///<returns>(Point3d seq) of reflection points on success</returns>
    static member ShootRay(surfaceIds:Guid seq, startPoint:Point3d, direction:Vector3d, [<OPT;DEF(10)>]reflections:float) : Point3d seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def ShootRay(surface_ids, start_point, direction, reflections=10):
        '''Shoots a ray at a collection of surfaces
        Parameters:
          surface_ids ([guid, ...]): one of more surface identifiers
          start_point (point): starting point of the ray
          direction (vector):  vector identifying the direction of the ray
          reflections (number, optional): the maximum number of times the ray will be reflected
        Returns:
          list(point, ...): of reflection points on success
          None: on error
        '''
        start_point = rhutil.coerce3dpoint(start_point, True)
        direction = rhutil.coerce3dvector(direction, True)
        id = rhutil.coerceguid(surface_ids, False)
        if id: surface_ids = [id]
        ray = Rhino.Geometry.Ray3d(start_point, direction)
        breps = []
        for id in surface_ids:
            brep = rhutil.coercebrep(id)
            if brep: breps.append(brep)
            else:
                surface = rhutil.coercesurface(id, True)
                breps.append(surface)
        if not breps: return scriptcontext.errorhandler()
        points = Rhino.Geometry.Intersect.Intersection.RayShoot(ray, breps, reflections)
        if points:
            rc = []
            rc.append(start_point)
            rc = rc + list(points)
            return rc
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Creates the shortest possible curve(geodesic) between two points on a
    ///  surface. For more details, see the ShortPath command in Rhino help</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface</param>
    ///<param name="startPoint">(Point3d) Start point of 'start/end points of the short curve' (FIXME 0)</param>
    ///<param name="endePoint">(Point3d) End point of 'start/end points of the short curve' (FIXME 0)</param>
    ///<returns>(Guid) identifier of the new surface on success</returns>
    static member ShortPath(surfaceId:Guid, startPoint:Point3d, endePoint:Point3d) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def ShortPath(surface_id, start_point, end_point):
        '''Creates the shortest possible curve(geodesic) between two points on a
        surface. For more details, see the ShortPath command in Rhino help
        Parameters:
          surface_id (guid): identifier of a surface
          start_point, end_point (point): start/end points of the short curve
        Returns:
          guid: identifier of the new surface on success
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        start = rhutil.coerce3dpoint(start_point, True)
        end = rhutil.coerce3dpoint(end_point, True)
        rc_start, u_start, v_start = surface.ClosestPoint(start)
        rc_end, u_end, v_end = surface.ClosestPoint(end)
        if not rc_start or not rc_end: return scriptcontext.errorhandler()
        start = Rhino.Geometry.Point2d(u_start, v_start)
        end = Rhino.Geometry.Point2d(u_end, v_end)
        tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        curve = surface.ShortPath(start, end, tolerance)
        if curve is None: return scriptcontext.errorhandler()
        id = scriptcontext.doc.Objects.AddCurve(curve)
        if id==System.Guid.Empty: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return id
    *)


    [<EXT>]
    //(FIXME) VarOutTypes
    ///<summary>Shrinks the underlying untrimmed surfaces near to the trimming
    ///  boundaries. See the ShrinkTrimmedSrf command in the Rhino help.</summary>
    ///<param name="objectId">(Guid) The surface's identifier</param>
    ///<param name="createCopy">(bool) Optional, Default Value: <c>false</c>
    ///If True, the original surface is not deleted</param>
    ///<returns>(bool) If create_copy is False, True or False indicating success or failure</returns>
    static member ShrinkTrimmedSurface(objectId:Guid, [<OPT;DEF(false)>]createCopy:bool) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def ShrinkTrimmedSurface(object_id, create_copy=False):
        '''Shrinks the underlying untrimmed surfaces near to the trimming
        boundaries. See the ShrinkTrimmedSrf command in the Rhino help.
        Parameters:
          object_id (guid): the surface's identifier
          create_copy (bool, optional): If True, the original surface is not deleted
        Returns:
          bool: If create_copy is False, True or False indicating success or failure
          bool: If create_copy is True, the identifier of the new surface
          None: on error
        '''
        brep = rhutil.coercebrep(object_id, True)
        if not brep.Faces.ShrinkFaces(): return scriptcontext.errorhandler()
        rc = None
        object_id = rhutil.coerceguid(object_id)
        if create_copy:
            oldobj = scriptcontext.doc.Objects.Find(object_id)
            attr = oldobj.Attributes
            rc = scriptcontext.doc.Objects.AddBrep(brep, attr)
        else:
            rc = scriptcontext.doc.Objects.Replace(object_id, brep)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    
    static member internal GetMassProperties() : obj =
        failNotImpl () // genreation temp disabled !!
    (*
    def __GetMassProperties(object_id, area):
        ''''''
    *)


    [<EXT>]
    ///<summary>Splits a brep</summary>
    ///<param name="brepId">(Guid) Identifier of the brep to split</param>
    ///<param name="cutterId">(Guid) Identifier of the brep to split with</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Delete input breps</param>
    ///<returns>(Guid seq) identifiers of split pieces on success</returns>
    static member SplitBrep(brepId:Guid, cutterId:Guid, [<OPT;DEF(false)>]deleteInput:bool) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def SplitBrep(brep_id, cutter_id, delete_input=False):
        '''Splits a brep
        Parameters:
          brep_id (guid): identifier of the brep to split
          cutter_id (guid): identifier of the brep to split with
          delete_input(bool,optional): delete input breps
        Returns:
          list(guid, ...): identifiers of split pieces on success
          None: on error
        '''
        brep = rhutil.coercebrep(brep_id, True)
        cutter = rhutil.coercebrep(cutter_id, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        pieces = brep.Split(cutter, tol)
        if not pieces: return scriptcontext.errorhandler()
        if delete_input:
            brep_id = rhutil.coerceguid(brep_id)
            scriptcontext.doc.Objects.Delete(brep_id, True)
        rc = [scriptcontext.doc.Objects.AddBrep(piece) for piece in pieces]
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Calculate the area of a surface or polysurface object. The results are
    ///  based on the current drawing units</summary>
    ///<param name="objectId">(Guid) The surface's identifier</param>
    ///<returns>(float * float) of area information on success (area, absolute error bound)</returns>
    static member SurfaceArea(objectId:Guid) : float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceArea(object_id):
        '''Calculate the area of a surface or polysurface object. The results are
        based on the current drawing units
        Parameters:
          object_id (guid): the surface's identifier
        Returns:
          list(number, number): of area information on success (area, absolute error bound)
          None: on error
        '''
        amp = __GetMassProperties(object_id, True)
        if amp: return amp.Area, amp.AreaError
    *)


    [<EXT>]
    ///<summary>Calculates the area centroid of a surface or polysurface</summary>
    ///<param name="objectId">(Guid) The surface's identifier</param>
    ///<returns>(Point3d * float * float * float) Area centroid information (Area Centroid, Error bound in X, Y, Z)</returns>
    static member SurfaceAreaCentroid(objectId:Guid) : Point3d * float * float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceAreaCentroid(object_id):
        '''Calculates the area centroid of a surface or polysurface
        Parameters:
          object_id (guid): the surface's identifier
        Returns:
          list(point, tuple(number, number, number)): Area centroid information (Area Centroid, Error bound in X, Y, Z)
          None: on error
        '''
        amp = __GetMassProperties(object_id, True)
        if amp is None: return scriptcontext.errorhandler()
        return amp.Centroid, amp.CentroidError
    *)


    [<EXT>]
    
    static member internal AreaMomentsHelper() : obj =
        failNotImpl () // genreation temp disabled !!
    (*
    def __AreaMomentsHelper(surface_id, area):
        ''''''
    *)


    [<EXT>]
    ///<summary>Calculates area moments of inertia of a surface or polysurface object.
    ///  See the Rhino help for "Mass Properties calculation details"</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<returns>(float seq) of moments and error bounds in tuple(X, Y, Z) - see help topic
    ///  Index   Description
    ///  [0]     First Moments.
    ///  [1]     The absolute (+/-) error bound for the First Moments.
    ///  [2]     Second Moments.
    ///  [3]     The absolute (+/-) error bound for the Second Moments.
    ///  [4]     Product Moments.
    ///  [5]     The absolute (+/-) error bound for the Product Moments.
    ///  [6]     Area Moments of Inertia about the World Coordinate Axes.
    ///  [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
    ///  [8]     Area Radii of Gyration about the World Coordinate Axes.
    ///  [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///  [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///  [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///  [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///  [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.</returns>
    static member SurfaceAreaMoments(surfaceId:Guid) : float seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceAreaMoments(surface_id):
        '''Calculates area moments of inertia of a surface or polysurface object.
        See the Rhino help for "Mass Properties calculation details"
        Parameters:
          surface_id (guid): the surface's identifier
        Returns:
          list(tuple(number, number,number), ...): of moments and error bounds in tuple(X, Y, Z) - see help topic
            Index   Description
            [0]     First Moments.
            [1]     The absolute (+/-) error bound for the First Moments.
            [2]     Second Moments.
            [3]     The absolute (+/-) error bound for the Second Moments.
            [4]     Product Moments.
            [5]     The absolute (+/-) error bound for the Product Moments.
            [6]     Area Moments of Inertia about the World Coordinate Axes.
            [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
            [8]     Area Radii of Gyration about the World Coordinate Axes.
            [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
            [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
            [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
            [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
            [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.
          None: on error
        '''
        return __AreaMomentsHelper(surface_id, True)
    *)


    [<EXT>]
    ///<summary>Returns U,V parameters of point on a surface that is closest to a test point</summary>
    ///<param name="surfaceId">(Guid) Identifier of a surface object</param>
    ///<param name="testPoint">(Point3d) Sampling point</param>
    ///<returns>(float * float) The U,V parameters of the closest point on the surface .</returns>
    static member SurfaceClosestPoint(surfaceId:Guid, testPoint:Point3d) : float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceClosestPoint(surface_id, test_point):
        '''Returns U,V parameters of point on a surface that is closest to a test point
        Parameters:
          surface_id (guid): identifier of a surface object
          test_point (point): sampling point
        Returns:
          list(number, number): The U,V parameters of the closest point on the surface if successful.
          None: on error.
        '''
        surface = rhutil.coercesurface(surface_id, True)
        point = rhutil.coerce3dpoint(test_point, True)
        rc, u, v = surface.ClosestPoint(point)
        if not rc: return None
        return u,v
    *)


    [<EXT>]
    ///<summary>Returns the definition of a surface cone</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<returns>(Plane * float * float) containing the definition of the cone
    ///  [0]   the plane of the cone. The apex of the cone is at the
    ///    plane's origin and the axis of the cone is the plane's z-axis
    ///  [1]   the height of the cone
    ///  [2]   the radius of the cone</returns>
    static member SurfaceCone(surfaceId:Guid) : Plane * float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceCone(surface_id):
        '''Returns the definition of a surface cone
        Parameters:
          surface_id (guid): the surface's identifier
        Returns:
          tuple(plane, number, number): containing the definition of the cone if successful
            [0]   the plane of the cone. The apex of the cone is at the
                  plane's origin and the axis of the cone is the plane's z-axis
            [1]   the height of the cone
            [2]   the radius of the cone
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        rc, cone = surface.TryGetCone()
        if not rc: return scriptcontext.errorhandler()
        return cone.Plane, cone.Height, cone.Radius
    *)


    [<EXT>]
    ///<summary>Returns the curvature of a surface at a U,V parameter. See Rhino help
    ///  for details of surface curvature</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="parameter">(float * float) U,v parameter</param>
    ///<returns>(Point3d * Vector3d * float * float * float * float * float) of curvature information
    ///  [0]   point at specified U,V parameter
    ///  [1]   normal direction
    ///  [2]   maximum principal curvature
    ///  [3]   maximum principal curvature direction
    ///  [4]   minimum principal curvature
    ///  [5]   minimum principal curvature direction
    ///  [6]   gaussian curvature
    ///  [7]   mean curvature</returns>
    static member SurfaceCurvature(surfaceId:Guid, parameter:float * float) : Point3d * Vector3d * float * float * float * float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceCurvature(surface_id, parameter):
        '''Returns the curvature of a surface at a U,V parameter. See Rhino help
        for details of surface curvature
        Parameters:
          surface_id (guid): the surface's identifier
          parameter (number, number): u,v parameter
        Returns:
          tuple(point, vector, number, number, number, number, number): of curvature information
            [0]   point at specified U,V parameter
            [1]   normal direction
            [2]   maximum principal curvature
            [3]   maximum principal curvature direction
            [4]   minimum principal curvature
            [5]   minimum principal curvature direction
            [6]   gaussian curvature
            [7]   mean curvature
          None: if not successful, or on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        if len(parameter)<2: return scriptcontext.errorhandler()
        c = surface.CurvatureAt(parameter[0], parameter[1])
        if c is None: return scriptcontext.errorhandler()
        return c.Point, c.Normal, c.Kappa(0), c.Direction(0), c.Kappa(1), c.Direction(1), c.Gaussian, c.Mean
    *)


    [<EXT>]
    ///<summary>Returns the definition of a cylinder surface</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<returns>(Plane * float * float) of the cylinder plane, height, radius on success</returns>
    static member SurfaceCylinder(surfaceId:Guid) : Plane * float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceCylinder(surface_id):
        '''Returns the definition of a cylinder surface
        Parameters:
          surface_id (guid): the surface's identifier
        Returns:
          tuple(plane, number, number): of the cylinder plane, height, radius on success
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        tol = scriptcontext.doc.ModelAbsoluteTolerance
        rc, cylinder = surface.TryGetFiniteCylinder(tol)
        if rc:
            return cylinder.BasePlane, cylinder.TotalHeight, cylinder.Radius
    *)


    [<EXT>]
    //(FIXME) VarOutTypes
    ///<summary>Returns the degree of a surface object in the specified direction</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="direction">(int) Optional, Default Value: <c>2</c>
    ///The degree U, V direction
    ///  0 = U
    ///  1 = V
    ///  2 = both</param>
    ///<returns>(int) Single number if `direction` = 0 or 1</returns>
    static member SurfaceDegree(surfaceId:Guid, [<OPT;DEF(2)>]direction:int) : int =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceDegree(surface_id, direction=2):
        '''Returns the degree of a surface object in the specified direction
        Parameters:
          surface_id (guid): the surface's identifier
          direction (number, optional): The degree U, V direction
                    0 = U
                    1 = V
                    2 = both
        Returns:
          number: Single number if `direction` = 0 or 1
          tuple(number, number): of two values if `direction` = 2
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        if direction==0 or direction==1: return surface.Degree(direction)
        if direction==2: return surface.Degree(0), surface.Degree(1)
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Returns the domain of a surface object in the specified direction.</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="direction">(int) Domain direction 0 = U, or 1 = V</param>
    ///<returns>(float * float) containing the domain interval in the specified direction</returns>
    static member SurfaceDomain(surfaceId:Guid, direction:int) : float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceDomain(surface_id, direction):
        '''Returns the domain of a surface object in the specified direction.
        Parameters:
          surface_id (guid): the surface's identifier
          direction (number): domain direction 0 = U, or 1 = V
        Returns:
          list(number, number): containing the domain interval in the specified direction
          None: if not successful, or on error
        '''
        if direction!=0 and direction!=1: return scriptcontext.errorhandler()
        surface = rhutil.coercesurface(surface_id, True)
        domain = surface.Domain(direction)
        return domain.T0, domain.T1
    *)


    [<EXT>]
    //(FIXME) VarOutTypes
    ///<summary>Returns the edit, or Greville points of a surface object. For each
    ///  surface control point, there is a corresponding edit point</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="returnParameters">(bool) Optional, Default Value: <c>false</c>
    ///If False, edit points are returned as a list of
    ///  3D points. If True, edit points are returned as a list of U,V surface
    ///  parameters</param>
    ///<param name="returnAll">(bool) Optional, Default Value: <c>true</c>
    ///If True, all surface edit points are returned. If False,
    ///  the function will return surface edit points based on whether or not the
    ///  surface is closed or periodic</param>
    ///<returns>(Point3d seq) if return_parameters is False, a list of 3D points</returns>
    static member SurfaceEditPoints(surfaceId:Guid, [<OPT;DEF(false)>]returnParameters:bool, [<OPT;DEF(true)>]returnAll:bool) : Point3d seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceEditPoints(surface_id, return_parameters=False, return_all=True):
        '''Returns the edit, or Greville points of a surface object. For each
        surface control point, there is a corresponding edit point
        Parameters:
          surface_id (guid): the surface's identifier
          return_parameters (bool, optional): If False, edit points are returned as a list of
            3D points. If True, edit points are returned as a list of U,V surface
            parameters
          return_all (bool, optional): If True, all surface edit points are returned. If False,
            the function will return surface edit points based on whether or not the
            surface is closed or periodic
        Returns:
          list(point, ...): if return_parameters is False, a list of 3D points
          list((number, number), ...): if return_parameters is True, a list of U,V parameters
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        nurb = surface.ToNurbsSurface()
        if not nurb: return scriptcontext.errorhandler()
        ufirst = 0
        ulast = nurb.Points.CountU
        vfirst = 0
        vlast = nurb.Points.CountV
        if not return_all:
            if nurb.IsClosed(0): ulast = nurb.Points.CountU-1
            if nurb.IsPeriodic(0):
                degree = nurb.Degree(0)
                ufirst = degree/2
                ulast = nurb.Points.CountU-degree+ufirst
            if nurb.IsClosed(1): vlast = nurb.Points.CountV-1
            if nurb.IsPeriodic(1):
                degree = nurb.Degree(1)
                vfirst = degree/2
                vlast = nurb.Points.CountV-degree+vfirst
        rc = []
        for u in range(ufirst, ulast):
            for v in range(vfirst, vlast):
                pt = nurb.Points.GetGrevillePoint(u,v)
                if not return_parameters: pt = nurb.PointAt(pt.X, pt.Y)
                rc.append(pt)
        return rc
    *)


    [<EXT>]
    ///<summary>A general purpose surface evaluator</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="parameter">(float * float) U,v parameter to evaluate</param>
    ///<param name="derivative">(float) Number of derivatives to evaluate</param>
    ///<returns>(Point3d seq) list length (derivative+1)*(derivative+2)/2 .  The elements are as follows:
    ///  Element  Description
    ///  [0]      The 3-D point.
    ///  [1]      The first derivative.
    ///  [2]      The first derivative.
    ///  [3]      The second derivative.
    ///  [4]      The second derivative.
    ///  [5]      The second derivative.
    ///  [6]      etc...
    ///    None: If not successful, or on error.</returns>
    static member SurfaceEvaluate(surfaceId:Guid, parameter:float * float, derivative:float) : Point3d seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceEvaluate(surface_id, parameter, derivative):
        '''A general purpose surface evaluator
        Parameters:
          surface_id (guid): the surface's identifier
          parameter ([number, number]): u,v parameter to evaluate
          derivative (number): number of derivatives to evaluate
        Returns:
          list((point, vector, ...), ...): list length (derivative+1)*(derivative+2)/2 if successful.  The elements are as follows:
          Element  Description
          [0]      The 3-D point.
          [1]      The first derivative.
          [2]      The first derivative.
          [3]      The second derivative.
          [4]      The second derivative.
          [5]      The second derivative.
          [6]      etc...
        None: If not successful, or on error.
        '''
        surface = rhutil.coercesurface(surface_id, True)
        success, point, der = surface.Evaluate(parameter[0], parameter[1], derivative)
        if not success: return scriptcontext.errorhandler()
        rc = [point]
        if der:
          for d in der: rc.append(d)
        return rc
    *)


    [<EXT>]
    ///<summary>Returns a plane based on the normal, u, and v directions at a surface
    ///  U,V parameter</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="uvParameter">(float * float) U,v parameter to evaluate</param>
    ///<returns>(Plane) plane on success</returns>
    static member SurfaceFrame(surfaceId:Guid, uvParameter:float * float) : Plane =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceFrame(surface_id, uv_parameter):
        '''Returns a plane based on the normal, u, and v directions at a surface
        U,V parameter
        Parameters:
          surface_id (guid): the surface's identifier
          uv_parameter ([number, number]): u,v parameter to evaluate
        Returns:
          plane: plane on success
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        rc, frame = surface.FrameAt(uv_parameter[0], uv_parameter[1])
        if rc: return frame
    *)


    [<EXT>]
    ///<summary>Returns the isocurve density of a surface or polysurface object.
    /// An isoparametric curve is a curve of constant U or V value on a surface.
    /// Rhino uses isocurves and surface edge curves to visualize the shape of a
    /// NURBS surface</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<returns>(int) The current isocurve density
    ///  -1: Hides the surface isocurves
    ///    0: Display boundary and knot wires
    ///    1: Display boundary and knot wires and one interior wire if there
    ///      are no interior knots
    ///        >=2: Display boundary and knot wires and (N+1) interior wires</returns>
    static member SurfaceIsocurveDensity(surfaceId:Guid) : int = //GET
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceIsocurveDensity(surface_id, density=None):
        '''Returns or sets the isocurve density of a surface or polysurface object.
        An isoparametric curve is a curve of constant U or V value on a surface.
        Rhino uses isocurves and surface edge curves to visualize the shape of a
        NURBS surface
        Parameters:
          surface_id (guid): the surface's identifier
          density (number, optional): the isocurve wireframe density. The possible values are
              -1: Hides the surface isocurves
               0: Display boundary and knot wires
               1: Display boundary and knot wires and one interior wire if there
                  are no interior knots
             >=2: Display boundary and knot wires and (N+1) interior wires
        Returns:
          number: If density is not specified, then the current isocurve density if successful
          number: If density is specified, the the previous isocurve density if successful
          None: on error
        '''
        rhino_object = rhutil.coercerhinoobject(surface_id, True, True)
        if not isinstance(rhino_object, Rhino.DocObjects.BrepObject):
            return scriptcontext.errorhandler()
        rc = rhino_object.Attributes.WireDensity
        if density is not None:
            if density<0: density = -1
            rhino_object.Attributes.WireDensity = density
            rhino_object.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Sets the isocurve density of a surface or polysurface object.
    /// An isoparametric curve is a curve of constant U or V value on a surface.
    /// Rhino uses isocurves and surface edge curves to visualize the shape of a
    /// NURBS surface</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="density">(int)The isocurve wireframe density. The possible values are
    ///  -1: Hides the surface isocurves
    ///    0: Display boundary and knot wires
    ///    1: Display boundary and knot wires and one interior wire if there
    ///      are no interior knots
    ///        >=2: Display boundary and knot wires and (N+1) interior wires</param>
    ///<returns>(unit) void, nothing</returns>
    static member SurfaceIsocurveDensity(surfaceId:Guid, density:int) : unit = //SET
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceIsocurveDensity(surface_id, density=None):
        '''Returns or sets the isocurve density of a surface or polysurface object.
        An isoparametric curve is a curve of constant U or V value on a surface.
        Rhino uses isocurves and surface edge curves to visualize the shape of a
        NURBS surface
        Parameters:
          surface_id (guid): the surface's identifier
          density (number, optional): the isocurve wireframe density. The possible values are
              -1: Hides the surface isocurves
               0: Display boundary and knot wires
               1: Display boundary and knot wires and one interior wire if there
                  are no interior knots
             >=2: Display boundary and knot wires and (N+1) interior wires
        Returns:
          number: If density is not specified, then the current isocurve density if successful
          number: If density is specified, the the previous isocurve density if successful
          None: on error
        '''
        rhino_object = rhutil.coercerhinoobject(surface_id, True, True)
        if not isinstance(rhino_object, Rhino.DocObjects.BrepObject):
            return scriptcontext.errorhandler()
        rc = rhino_object.Attributes.WireDensity
        if density is not None:
            if density<0: density = -1
            rhino_object.Attributes.WireDensity = density
            rhino_object.CommitChanges()
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the control point count of a surface
    ///  surfaceId = the surface's identifier</summary>
    ///<param name="surfaceId">(Guid) The surface object's identifier</param>
    ///<returns>(int * int) a list containing (U count, V count) on success</returns>
    static member SurfaceKnotCount(surfaceId:Guid) : int * int =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceKnotCount(surface_id):
        '''Returns the control point count of a surface
          surface_id = the surface's identifier
        Parameters:
          surface_id (guid): the surface object's identifier
        Returns:
          list(number, number): a list containing (U count, V count) on success
        '''
        surface = rhutil.coercesurface(surface_id, True)
        ns = surface.ToNurbsSurface()
        return ns.KnotsU.Count, ns.KnotsV.Count
    *)


    [<EXT>]
    ///<summary>Returns the knots, or knot vector, of a surface object.</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<returns>(float * float) knot values of the surface . The list will
    ///  contain the following information:
    ///  Element   Description
    ///    [0]     Knot vector in U direction
    ///    [1]     Knot vector in V direction
    ///  None: if not successful, or on error.</returns>
    static member SurfaceKnots(surfaceId:Guid) : float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceKnots(surface_id):
        '''Returns the knots, or knot vector, of a surface object.
        Parameters:
          surface_id (guid): the surface's identifier
        Returns:
         list(number, number): knot values of the surface if successful. The list will
          contain the following information:
          Element   Description
            [0]     Knot vector in U direction
            [1]     Knot vector in V direction
          None: if not successful, or on error.
        '''
        surface = rhutil.coercesurface(surface_id, True)
        nurb_surf = surface.ToNurbsSurface()
        if nurb_surf is None: return scriptcontext.errorhandler()
        s_knots = [knot for knot in nurb_surf.KnotsU]
        t_knots = [knot for knot in nurb_surf.KnotsV]
        if not s_knots or not t_knots: return scriptcontext.errorhandler()
        return s_knots, t_knots
    *)


    [<EXT>]
    ///<summary>Returns 3D vector that is the normal to a surface at a parameter</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="uvParameter">(float * float) The uv parameter to evaluate</param>
    ///<returns>(Vector3d) Normal vector on success</returns>
    static member SurfaceNormal(surfaceId:Guid, uvParameter:float * float) : Vector3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceNormal(surface_id, uv_parameter):
        '''Returns 3D vector that is the normal to a surface at a parameter
        Parameters:
          surface_id (guid): the surface's identifier
          uv_parameter  ([number, number]): the uv parameter to evaluate
        Returns:
          vector: Normal vector on success
        '''
        surface = rhutil.coercesurface(surface_id, True)
        return surface.NormalAt(uv_parameter[0], uv_parameter[1])
    *)


    [<EXT>]
    ///<summary>Converts surface parameter to a normalized surface parameter; one that
    ///  ranges between 0.0 and 1.0 in both the U and V directions</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="parameter">(float * float) The surface parameter to convert</param>
    ///<returns>(float * float) normalized surface parameter</returns>
    static member SurfaceNormalizedParameter(surfaceId:Guid, parameter:float * float) : float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceNormalizedParameter(surface_id, parameter):
        '''Converts surface parameter to a normalized surface parameter; one that
        ranges between 0.0 and 1.0 in both the U and V directions
        Parameters:
          surface_id (guid) the surface's identifier
          parameter ([number, number]): the surface parameter to convert
        Returns:
          list(number, number): normalized surface parameter if successful
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        u_domain = surface.Domain(0)
        v_domain = surface.Domain(1)
        if parameter[0]<u_domain.Min or parameter[0]>u_domain.Max:
            return scriptcontext.errorhandler()
        if parameter[1]<v_domain.Min or parameter[1]>v_domain.Max:
            return scriptcontext.errorhandler()
        u = u_domain.NormalizedParameterAt(parameter[0])
        v = v_domain.NormalizedParameterAt(parameter[1])
        return u,v
    *)


    [<EXT>]
    ///<summary>Converts normalized surface parameter to a surface parameter; or
    ///  within the surface's domain</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="parameter">(float * float) The normalized parameter to convert</param>
    ///<returns>(float * float) surface parameter on success</returns>
    static member SurfaceParameter(surfaceId:Guid, parameter:float * float) : float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceParameter(surface_id, parameter):
        '''Converts normalized surface parameter to a surface parameter; or
        within the surface's domain
        Parameters:
          surface_id (guid): the surface's identifier
          parameter ([number, number]): the normalized parameter to convert
        Returns:
          tuple(number, number): surface parameter on success
        '''
        surface = rhutil.coercesurface(surface_id, True)
        x = surface.Domain(0).ParameterAt(parameter[0])
        y = surface.Domain(1).ParameterAt(parameter[1])
        return x, y
    *)


    [<EXT>]
    ///<summary>Returns the control point count of a surface
    ///  surfaceId = the surface's identifier</summary>
    ///<param name="surfaceId">(Guid) The surface object's identifier</param>
    ///<returns>(int * int) THe number of control points in UV direction. (U count, V count)</returns>
    static member SurfacePointCount(surfaceId:Guid) : int * int =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfacePointCount(surface_id):
        '''Returns the control point count of a surface
          surface_id = the surface's identifier
        Parameters:
          surface_id (guid): the surface object's identifier
        Returns:
          list(number, number): THe number of control points in UV direction. (U count, V count)
        '''
        surface = rhutil.coercesurface(surface_id, True)
        ns = surface.ToNurbsSurface()
        return ns.Points.CountU, ns.Points.CountV
    *)


    [<EXT>]
    ///<summary>Returns the control points, or control vertices, of a surface object</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="returnAll">(bool) Optional, Default Value: <c>true</c>
    ///If True all surface edit points are returned. If False,
    ///  the function will return surface edit points based on whether or not
    ///  the surface is closed or periodic</param>
    ///<returns>(Point3d seq) the control points</returns>
    static member SurfacePoints(surfaceId:Guid, [<OPT;DEF(true)>]returnAll:bool) : Point3d seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfacePoints(surface_id, return_all=True):
        '''Returns the control points, or control vertices, of a surface object
        Parameters:
          surface_id (guid): the surface's identifier
          return_all (bool, optional): If True all surface edit points are returned. If False,
            the function will return surface edit points based on whether or not
            the surface is closed or periodic
        Returns:
          list(point, ...): the control points if successful
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        ns = surface.ToNurbsSurface()
        if ns is None: return scriptcontext.errorhandler()
        rc = []
        for u in range(ns.Points.CountU):
            for v in range(ns.Points.CountV):
                pt = ns.Points.GetControlPoint(u,v)
                rc.append(pt.Location)
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the definition of a surface torus</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<returns>(Plane * float * float) containing the definition of the torus
    ///  [0]   the base plane of the torus
    ///  [1]   the major radius of the torus
    ///  [2]   the minor radius of the torus</returns>
    static member SurfaceTorus(surfaceId:Guid) : Plane * float * float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceTorus(surface_id):
        '''Returns the definition of a surface torus
        Parameters:
          surface_id (guid): the surface's identifier
        Returns:
          tuple(plane, number, number): containing the definition of the torus if successful
            [0]   the base plane of the torus
            [1]   the major radius of the torus
            [2]   the minor radius of the torus
          None: on error
        '''
        surface = rhutil.coercesurface(surface_id, True)
        rc, torus = surface.TryGetTorus()
        if rc: return torus.Plane, torus.MajorRadius, torus.MinorRadius
    *)


    [<EXT>]
    ///<summary>Calculates volume of a closed surface or polysurface</summary>
    ///<param name="objectId">(Guid) The surface's identifier</param>
    ///<returns>(float) volume data returned (Volume, Error bound) on success</returns>
    static member SurfaceVolume(objectId:Guid) : float =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceVolume(object_id):
        '''Calculates volume of a closed surface or polysurface
        Parameters:
          object_id (guid): the surface's identifier
        Returns:
          list[number, tuple(X, Y, Z)]: volume data returned (Volume, Error bound) on success
          None: on error
        '''
        vmp = __GetMassProperties(object_id, False)
        if vmp: return vmp.Volume, vmp.VolumeError
    *)


    [<EXT>]
    ///<summary>Calculates volume centroid of a closed surface or polysurface</summary>
    ///<param name="objectId">(Guid) The surface's identifier</param>
    ///<returns>(Point3d) volume data returned (Volume Centriod, Error bound) on success</returns>
    static member SurfaceVolumeCentroid(objectId:Guid) : Point3d =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceVolumeCentroid(object_id):
        '''Calculates volume centroid of a closed surface or polysurface
        Parameters:
          object_id (guid): the surface's identifier
        Returns:
          list[point, tuple(X, Y, Z)]: volume data returned (Volume Centriod, Error bound) on success
          None: on error
        '''
        vmp = __GetMassProperties(object_id, False)
        if vmp: return vmp.Centroid, vmp.CentroidError
    *)


    [<EXT>]
    ///<summary>Calculates volume moments of inertia of a surface or polysurface object.
    ///  For more information, see Rhino help for "Mass Properties calculation details"</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<returns>(float seq) of moments and error bounds in tuple(X, Y, Z) - see help topic
    ///  Index   Description
    ///  [0]     First Moments.
    ///  [1]     The absolute (+/-) error bound for the First Moments.
    ///  [2]     Second Moments.
    ///  [3]     The absolute (+/-) error bound for the Second Moments.
    ///  [4]     Product Moments.
    ///  [5]     The absolute (+/-) error bound for the Product Moments.
    ///  [6]     Area Moments of Inertia about the World Coordinate Axes.
    ///  [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
    ///  [8]     Area Radii of Gyration about the World Coordinate Axes.
    ///  [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
    ///  [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
    ///  [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
    ///  [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
    ///  [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.</returns>
    static member SurfaceVolumeMoments(surfaceId:Guid) : float seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceVolumeMoments(surface_id):
        '''Calculates volume moments of inertia of a surface or polysurface object.
        For more information, see Rhino help for "Mass Properties calculation details"
        Parameters:
          surface_id (guid): the surface's identifier
        Returns:
          list(tuple(number, number,number), ...): of moments and error bounds in tuple(X, Y, Z) - see help topic
            Index   Description
            [0]     First Moments.
            [1]     The absolute (+/-) error bound for the First Moments.
            [2]     Second Moments.
            [3]     The absolute (+/-) error bound for the Second Moments.
            [4]     Product Moments.
            [5]     The absolute (+/-) error bound for the Product Moments.
            [6]     Area Moments of Inertia about the World Coordinate Axes.
            [7]     The absolute (+/-) error bound for the Area Moments of Inertia about World Coordinate Axes.
            [8]     Area Radii of Gyration about the World Coordinate Axes.
            [9]     The absolute (+/-) error bound for the Area Radii of Gyration about World Coordinate Axes.
            [10]    Area Moments of Inertia about the Centroid Coordinate Axes.
            [11]    The absolute (+/-) error bound for the Area Moments of Inertia about the Centroid Coordinate Axes.
            [12]    Area Radii of Gyration about the Centroid Coordinate Axes.
            [13]    The absolute (+/-) error bound for the Area Radii of Gyration about the Centroid Coordinate Axes.
          None: on error
        '''
        return __AreaMomentsHelper(surface_id, False)
    *)


    [<EXT>]
    ///<summary>Returns list of weight values assigned to the control points of a surface.
    ///  The number of weights returned will be equal to the number of control points
    ///  in the U and V directions.</summary>
    ///<param name="objectId">(Guid) The surface's identifier</param>
    ///<returns>(float seq) point weights.</returns>
    static member SurfaceWeights(objectId:Guid) : float seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def SurfaceWeights(object_id):
        '''Returns list of weight values assigned to the control points of a surface.
        The number of weights returned will be equal to the number of control points
        in the U and V directions.
        Parameters:
          object_id (guid): the surface's identifier
        Returns:
          list(number, ...): point weights.
          None: on error
        '''
        surface = rhutil.coercesurface(object_id, True)
        ns = surface.ToNurbsSurface()
        if ns is None: return scriptcontext.errorhandler()
        rc = []
        for u in range(ns.Points.CountU):
            for v in range(ns.Points.CountV):
                pt = ns.Points.GetControlPoint(u,v)
                rc.append(pt.Weight)
        return rc
    *)


    [<EXT>]
    ///<summary>Trims a surface using an oriented cutter</summary>
    ///<param name="objectId">(Guid) Surface or polysurface identifier</param>
    ///<param name="cutter">(Plane) Surface, polysurface, or plane performing the trim</param>
    ///<param name="tolerance">(float) Optional, Default Value: <c>0.0</c>
    ///Trimming tolerance. If omitted, the document's absolute
    ///  tolerance is used</param>
    ///<returns>(Guid seq) identifiers of retained components on success</returns>
    static member TrimBrep(objectId:Guid, cutter:Plane, [<OPT;DEF(0.0)>]tolerance:float) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def TrimBrep(object_id, cutter, tolerance=None):
        '''Trims a surface using an oriented cutter
        Parameters:
          object_id (guid): surface or polysurface identifier
          cutter (guid|plane): surface, polysurface, or plane performing the trim
          tolerance (number, optional): trimming tolerance. If omitted, the document's absolute
            tolerance is used
        Returns:
          list(guid, ...): identifiers of retained components on success
        '''
        brep = rhutil.coercebrep(object_id, True)
        brep_cutter = rhutil.coercebrep(cutter, False)
        if brep_cutter: cutter = brep_cutter
        else: cutter = rhutil.coerceplane(cutter, True)
        if tolerance is None: tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        breps = brep.Trim(cutter, tolerance)
        rhid = rhutil.coerceguid(object_id, False)
        attrs = None
        if len(breps) > 1:
          rho = rhutil.coercerhinoobject(object_id, False)
          if rho: attrs = rho.Attributes
        if rhid:
            rc = []
            for i in range(len(breps)):
                if i==0:
                    scriptcontext.doc.Objects.Replace(rhid, breps[i])
                    rc.append(rhid)
                else:
                    rc.append(scriptcontext.doc.Objects.AddBrep(breps[i], attrs))
        else:
            rc = [scriptcontext.doc.Objects.AddBrep(brep) for brep in breps]
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Remove portions of the surface outside of the specified interval</summary>
    ///<param name="surfaceId">(Guid) Surface identifier</param>
    ///<param name="direction">(int) 0(U), 1(V), or 2(U and V)</param>
    ///<param name="interval">(float*float) Sub section of the surface to keep.
    ///  If both U and V then a list or tuple of 2 intervals</param>
    ///<param name="deleteInput">(bool) Optional, Default Value: <c>false</c>
    ///Should the input surface be deleted</param>
    ///<returns>(Guid) new surface identifier on success</returns>
    static member TrimSurface(surfaceId:Guid, direction:int, interval:float*float, [<OPT;DEF(false)>]deleteInput:bool) : Guid =
        failNotImpl () // genreation temp disabled !!
    (*
    def TrimSurface( surface_id, direction, interval, delete_input=False):
        '''Remove portions of the surface outside of the specified interval
        Parameters:
          surface_id (guid): surface identifier
          direction (number, optional): 0(U), 1(V), or 2(U and V)
          interval (interval): sub section of the surface to keep.
            If both U and V then a list or tuple of 2 intervals
          delete_input (bool, optional): should the input surface be deleted
        Returns:
          guid: new surface identifier on success
        '''
        surface = rhutil.coercesurface(surface_id, True)
        u = surface.Domain(0)
        v = surface.Domain(1)
        if direction==0:
            u[0] = interval[0]
            u[1] = interval[1]
        elif direction==1:
            v[0] = interval[0]
            v[1] = interval[1]
        else:
            u[0] = interval[0][0]
            u[1] = interval[0][1]
            v[0] = interval[1][0]
            v[1] = interval[1][1]
        new_surface = surface.Trim(u,v)
        if new_surface:
            rc = scriptcontext.doc.Objects.AddSurface(new_surface)
            if delete_input: scriptcontext.doc.Objects.Delete(rhutil.coerceguid(surface_id), True)
            scriptcontext.doc.Views.Redraw()
            return rc
    *)


    [<EXT>]
    //(FIXME) VarOutTypes
    ///<summary>Flattens a developable surface or polysurface</summary>
    ///<param name="surfaceId">(Guid) The surface's identifier</param>
    ///<param name="explode">(bool) Optional, Default Value: <c>false</c>
    ///If True, the resulting surfaces ar not joined</param>
    ///<param name="followingGeometry">(Guid seq) Optional, Default Value: <c>null:Guid seq</c>
    ///List of curves, dots, and points which
    ///  should be unrolled with the surface</param>
    ///<param name="absoluteTolerance">(float) Optional, Default Value: <c>0.0</c>
    ///Absolute tolerance</param>
    ///<param name="relativeTolerance">(float) Optional, Default Value: <c>0.0</c>
    ///Relative tolerance</param>
    ///<returns>(Guid seq) of unrolled surface ids</returns>
    static member UnrollSurface(surfaceId:Guid, [<OPT;DEF(false)>]explode:bool, [<OPT;DEF(null:Guid seq)>]followingGeometry:Guid seq, [<OPT;DEF(0.0)>]absoluteTolerance:float, [<OPT;DEF(0.0)>]relativeTolerance:float) : Guid seq =
        failNotImpl () // genreation temp disabled !!
    (*
    def UnrollSurface(surface_id, explode=False, following_geometry=None, absolute_tolerance=None, relative_tolerance=None):
        '''Flattens a developable surface or polysurface
        Parameters:
          surface_id (guid): the surface's identifier
          explode (bool, optional): If True, the resulting surfaces ar not joined
          following_geometry ([guid, ...], optional): List of curves, dots, and points which
            should be unrolled with the surface
          absolute_tolerance(number,optional): absolute tolerance
          relative_tolerance(number,optional): relative tolerance
        Returns:
          list(guid, ...): of unrolled surface ids
          tuple((guid, ...),(guid, ...)): if following_geometry is not None, a tuple
            [1] is the list of unrolled surface ids
            [2] is the list of unrolled following geometry
        '''
        brep = rhutil.coercebrep(surface_id, True)
        unroll = Rhino.Geometry.Unroller(brep)
        unroll.ExplodeOutput = explode
        if relative_tolerance is None: relative_tolerance = scriptcontext.doc.ModelRelativeTolerance
        if absolute_tolerance is None: absolute_tolerance = scriptcontext.doc.ModelAbsoluteTolerance
        unroll.AbsoluteTolerance = absolute_tolerance
        unroll.RelativeTolerance = relative_tolerance
        if following_geometry:
            for id in following_geometry:
                geom = rhutil.coercegeometry(id)
                unroll.AddFollowingGeometry(geom)
        breps, curves, points, dots = unroll.PerformUnroll()
        if not breps: return None
        rc = [scriptcontext.doc.Objects.AddBrep(brep) for brep in breps]
        new_following = []
        for curve in curves:
            id = scriptcontext.doc.Objects.AddCurve(curve)
            new_following.append(id)
        for point in points:
            id = scriptcontext.doc.Objects.AddPoint(point)
            new_following.append(id)
        for dot in dots:
            id = scriptcontext.doc.Objects.AddTextDot(dot)
            new_following.append(id)
        scriptcontext.doc.Views.Redraw()
        if following_geometry: return rc, new_following
        return rc
    *)


    [<EXT>]
    ///<summary>Changes the degree of a surface object.  For more information see the Rhino help file for the ChangeDegree command.</summary>
    ///<param name="objectId">(Guid) The object's identifier.</param>
    ///<param name="degree">(int * int) Two integers, specifying the degrees for the U  V directions</param>
    ///<returns>(bool) True of False indicating success or failure.</returns>
    static member ChangeSurfaceDegree(objectId:Guid, degree:int * int) : bool =
        failNotImpl () // genreation temp disabled !!
    (*
    def ChangeSurfaceDegree(object_id, degree):
        '''Changes the degree of a surface object.  For more information see the Rhino help file for the ChangeDegree command.
      Parameters:
        object_id (guid): the object's identifier.
        degree ([number, number]) two integers, specifying the degrees for the U  V directions
      Returns:
        bool: True of False indicating success or failure.
        None: on failure.
      '''
      object = rhutil.coercerhinoobject(object_id)
      if not object: return None
      obj_ref = Rhino.DocObjects.ObjRef(object)
      
      surface = obj_ref.Surface()
      if not surface: return None
      if not isinstance(surface, Rhino.Geometry.NurbsSurface):
        surface = surface.ToNurbsSurface() # could be a Surface or BrepFace
      max_nurbs_degree = 11
      if degree[0] < 1 or degree[0] > max_nurbs_degree or \
          degree[1] < 1 or degree[1] > max_nurbs_degree or \
          (surface.Degree(0) == degree[0] and surface.Degree(1) == degree[1]):
        return None
      r = False
      if surface.IncreaseDegreeU(degree[0]):
        if surface.IncreaseDegreeV(degree[1]):
          r = scriptcontext.doc.Objects.Replace(object_id, surface)
      return r
    *)


