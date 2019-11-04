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
    ///<param name="corners">(Point3d seq) 8 points that define the corners of the box. Points need to
    ///  be in counter-clockwise order starting with the bottom rectangle of the box</param>
    ///<returns>(Guid) identifier of the new object on success</returns>
    static member AddBox(corners:Point3d seq) : Guid =
        //box = RhinoScriptSyntax.Coerce3dpointlist(corners)
        let brep = Brep.CreateFromBox(corners)
        if isNull brep then failwithf "Rhino.Scripting: Unable to create brep from box.  %d corners:'%A'" (Seq.length corners) corners
        let rc = Doc.Objects.AddBrep(brep)
        if rc = Guid.Empty then failwithf "Rhino.Scripting: Unable to add brep to document.  corners:'%A'" corners
        Doc.Views.Redraw()
        rc
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
    static member AddCone( basis:Plane, 
                           height:float, 
                           radius:float, 
                           [<OPT;DEF(true)>]cap:bool) : Guid =
        let cone = Cone(basis, height, radius)
        let brep = Brep.CreateFromCone(cone, cap)
        let rc = Doc.Objects.AddBrep(brep)
        Doc.Views.Redraw()
        rc
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
    ///<param name="startPoint">(Point3d) Start point of line that defines the cutting plane</param>
    ///<param name="endPoint">(Point3d) End point of line that defines the cutting plane</param>
    ///<param name="normal">(Vector3d) Optional, Default Value: <c>world Z axis</c>
    ///  Vector that will be contained in the returned planar
    ///  surface. 
    ///  If omitted, the world Z axis is used</param>
    ///<returns>(Guid) identifier of new object on success</returns>
    static member AddCutPlane( objectIds:Guid seq, 
                               startPoint:Point3d, 
                               endPoint:Point3d, 
                               [<OPT;DEF(Vector3d())>]normal:Vector3d) : Guid =
        let bbox = BoundingBox.Unset
        for objectId in objectIds do
            let rhobj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            let geometry = rhobj.Geometry
            bbox.Union( geometry.GetBoundingBox(true) )
        //startPoint = RhinoScriptSyntax.Coerce3dpoint(startPoint)
        //endPoint = RhinoScriptSyntax.Coerce3dpoint(endPoint)
        if not bbox.IsValid then 
            failwithf "Rhino.Scripting: AddCutPlane failed.  objectIds:'%A' startPoint:'%A' endPoint:'%A' normal:'%A'" objectIds startPoint endPoint normal
        let line = Line(startPoint, endPoint)
        let normal = if normal.IsZero then Vector3d.ZAxis else normal
        let surface = PlaneSurface.CreateThroughBox(line, normal, bbox)
        if surface|> isNull  then failwithf "Rhino.Scripting: AddCutPlane failed.  objectIds:'%A' startPoint:'%A' endPoint:'%A' normal:'%A'" objectIds startPoint endPoint normal
        let objectId = Doc.Objects.AddSurface(surface)
        if objectId = Guid.Empty then failwithf "Rhino.Scripting: AddCutPlane failed.  objectIds:'%A' startPoint:'%A' endPoint:'%A' normal:'%A'" objectIds startPoint endPoint normal
        Doc.Views.Redraw()
        objectId
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
    static member AddCylinder( basis:Plane, 
                               height:float, 
                               radius:float, 
                               [<OPT;DEF(true)>]cap:bool) : Guid =
        let circle = Circle(basis, radius)
        let cylinder = Cylinder(circle, height)          
        let brep = cylinder.ToBrep(cap, cap)
        let objectId = Doc.Objects.AddBrep(brep)
        if objectId = Guid.Empty then failwithf "Rhino.Scripting: AddCylinder failed.  basis:'%A' height:'%A' radius:'%A' cap:'%A'" basis height radius cap
        Doc.Views.Redraw()
        objectId
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
        let curves =  resizeArray { for objectId in curveIds do yield RhinoScriptSyntax.CoerceCurve(objectId) } 
        let brep = Brep.CreateEdgeSurface(curves)
        if brep|> isNull  then failwithf "Rhino.Scripting: AddEdgeSrf failed.  curveIds:'%A'" curveIds
        let objectId = Doc.Objects.AddBrep(brep)
        if objectId = Guid.Empty then failwithf "Rhino.Scripting: AddEdgeSrf failed.  curveIds:'%A'" curveIds
        Doc.Views.Redraw()
        objectId 
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
    ///<param name="edgeTolerance">(float) Optional, Edge tolerance</param>
    ///<param name="interiorTolerance">(float) Optional, Interior tolerance</param>
    ///<param name="angleTolerance">(float) Optional, Angle tolerance , in radians?</param>
    ///<returns>(Guid) identifier of new object</returns>
    static member AddNetworkSrf( curves:Guid seq, 
                                 [<OPT;DEF(1)>]continuity:int, 
                                 [<OPT;DEF(0.0)>]edgeTolerance:float, 
                                 [<OPT;DEF(0.0)>]interiorTolerance:float, 
                                 [<OPT;DEF(0.0)>]angleTolerance:float) : Guid =
        let curves =  resizeArray { for curve in curves do yield RhinoScriptSyntax.CoerceCurve(curve) } 
        let surf, err = NurbsSurface.CreateNetworkSurface(curves, continuity, edgeTolerance, interiorTolerance, angleTolerance)// 0.0 Tolerance OK ? TODO
        if notNull surf then
            let rc = Doc.Objects.AddSurface(surf)
            Doc.Views.Redraw()
            rc
        else
            failwithf "AddNetworkSrf failed on %A" curves
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

