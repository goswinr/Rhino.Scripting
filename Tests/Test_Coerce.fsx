// Tests for Src/Scripting_Coerce.fs
#load "Harness.fsx"
open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting
open Harness

start "Coerce"

let vec x y z = Vector3d(x, y, z)
let obj (id:Guid) = doc().Objects.FindId id
let rgb (c:Drawing.Color) = (int c.A, int c.R, int c.G, int c.B)
let line1 () = rs.AddLine(pt 0. 0. 0., pt 10. 0. 0.)
let arc1 () = rs.AddArc(Plane.WorldXY, 5., 90.)
let circle1 () = rs.AddCircle(Plane.WorldXY, 5.)
let poly1 () = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 5. 5. 0. ]
let pointId () = rs.AddPoint(1., 2., 3.)
/// a polycurve object with two line segments (JoinCurves would make a polyline curve of two lines)
let polyCurve1 () =
    let pc = new PolyCurve()
    pc.Append(new LineCurve(pt 0. 0. 0., pt 5. 0. 0.)) |> ignore
    pc.Append(new LineCurve(pt 5. 0. 0., pt 5. 5. 0.)) |> ignore
    doc().Objects.AddCurve pc

// ------------------------------- Guid -------------------------------

test "TryCoerceGuid" (fun () ->
    let g = Guid.NewGuid()
    eq "Guid" (Some g) (rs.TryCoerceGuid g)
    eq "empty Guid" None (rs.TryCoerceGuid Guid.Empty)
    eq "Some Guid" (Some g) (rs.TryCoerceGuid (Some g))
    eq "Some empty Guid" None (rs.TryCoerceGuid (Some Guid.Empty))
    eq "None" None (rs.TryCoerceGuid (None : Guid option))
    eq "valid string" (Some g) (rs.TryCoerceGuid (g.ToString()))
    eq "invalid string" None (rs.TryCoerceGuid "not a guid")
    eq "empty Guid as string" None (rs.TryCoerceGuid (Guid.Empty.ToString()))
    eq "null string" None (rs.TryCoerceGuid (null : string))
    let p = pointId ()
    eq "RhinoObject" (Some p) (rs.TryCoerceGuid (obj p))
    eq "null RhinoObject" None (rs.TryCoerceGuid (null : DocObjects.RhinoObject))
    use r = new DocObjects.ObjRef(p)
    eq "ObjRef" (Some p) (rs.TryCoerceGuid r)
    eq "null ObjRef" None (rs.TryCoerceGuid (null : DocObjects.ObjRef))
    )

test "CoerceGuid" (fun () ->
    let g = Guid.NewGuid()
    eq "Guid" g (rs.CoerceGuid g)
    throwsRs "empty Guid" (fun () -> rs.CoerceGuid Guid.Empty)
    eq "Some Guid" g (rs.CoerceGuid (Some g))
    throwsRs "Some empty Guid" (fun () -> rs.CoerceGuid (Some Guid.Empty))
    throwsRs "None" (fun () -> rs.CoerceGuid (None : Guid option))
    eq "valid string" g (rs.CoerceGuid (g.ToString()))
    throwsRs "invalid string" (fun () -> rs.CoerceGuid "not a guid")
    throwsRs "empty Guid as string" (fun () -> rs.CoerceGuid (Guid.Empty.ToString()))
    let p = pointId ()
    eq "RhinoObject" p (rs.CoerceGuid (obj p))
    throwsRs "null RhinoObject" (fun () -> rs.CoerceGuid (null : DocObjects.RhinoObject))
    use r = new DocObjects.ObjRef(p)
    eq "ObjRef" p (rs.CoerceGuid r)
    throwsRs "null ObjRef" (fun () -> rs.CoerceGuid (null : DocObjects.ObjRef))
    )

// ------------------------------- objects and geometry -------------------------------

test "TryCoerceRhinoObject" (fun () ->
    let p = pointId ()
    eq "found" p (rs.TryCoerceRhinoObject p).Value.Id
    eq "unknown" None (rs.TryCoerceRhinoObject (Guid.NewGuid()))
    eq "empty" None (rs.TryCoerceRhinoObject Guid.Empty)
    )

test "CoerceRhinoObject" (fun () ->
    let p = pointId ()
    eq "found" p (rs.CoerceRhinoObject p).Id
    throwsRs "unknown" (fun () -> rs.CoerceRhinoObject (Guid.NewGuid()))
    throwsRs "empty" (fun () -> rs.CoerceRhinoObject Guid.Empty)
    )

test "TryCoerceGeometry" (fun () ->
    ok "found" ((rs.TryCoerceGeometry (pointId ())).Value :? Point)
    eq "unknown" None (rs.TryCoerceGeometry (Guid.NewGuid()))
    )

test "CoerceGeometry" (fun () ->
    ok "found" (rs.CoerceGeometry (line1 ()) :? LineCurve)
    throwsRs "unknown" (fun () -> rs.CoerceGeometry (Guid.NewGuid()))
    throwsRs "empty" (fun () -> rs.CoerceGeometry Guid.Empty)
    )

test "TryCoerceLight" (fun () ->
    ok "light" ((rs.TryCoerceLight (rs.AddPointLight(pt 0. 0. 1.))).IsSome)
    eq "not a light" None (rs.TryCoerceLight (pointId ()))
    eq "unknown" None (rs.TryCoerceLight (Guid.NewGuid()))
    )

test "CoerceLight" (fun () ->
    ok "light" (not (isNull (rs.CoerceLight (rs.AddPointLight(pt 0. 0. 1.)))))
    throwsRs "not a light" (fun () -> rs.CoerceLight (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceLight (Guid.NewGuid()))
    )

test "TryCoerceMesh" (fun () ->
    ok "mesh" ((rs.TryCoerceMesh (addMeshBox 1. 1. 1.)).IsSome)
    eq "not a mesh" None (rs.TryCoerceMesh (pointId ()))
    eq "unknown" None (rs.TryCoerceMesh (Guid.NewGuid()))
    )

test "CoerceMesh" (fun () ->
    ok "mesh" ((rs.CoerceMesh (addMeshBox 1. 1. 1.)).Faces.Count > 0)
    throwsRs "not a mesh" (fun () -> rs.CoerceMesh (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceMesh (Guid.NewGuid()))
    )

test "TryCoerceSurface" (fun () ->
    ok "single face brep" ((rs.TryCoerceSurface (addPlaneSurface 2. 2.)).IsSome)
    eq "polysurface" None (rs.TryCoerceSurface (addBox 1. 1. 1.))
    eq "not a surface" None (rs.TryCoerceSurface (pointId ()))
    eq "unknown" None (rs.TryCoerceSurface (Guid.NewGuid()))
    )

test "CoerceSurface" (fun () ->
    ok "single face brep" (not (isNull (rs.CoerceSurface (addPlaneSurface 2. 2.))))
    ok "sphere" (not (isNull (rs.CoerceSurface (addSphere 3.))))
    throwsRs "polysurface" (fun () -> rs.CoerceSurface (addBox 1. 1. 1.))
    throwsRs "not a surface" (fun () -> rs.CoerceSurface (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceSurface (Guid.NewGuid()))
    )

test "TryCoerceBrep" (fun () ->
    ok "brep" ((rs.TryCoerceBrep (addBox 1. 1. 1.)).IsSome)
    let ex = doc().Objects.AddExtrusion(Extrusion.Create(Polyline([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0.; pt 0. 0. 0. ]).ToPolylineCurve(), 2., true))
    ok "extrusion is converted to a brep" ((rs.TryCoerceBrep ex).IsSome)
    eq "not a brep" None (rs.TryCoerceBrep (pointId ()))
    eq "unknown" None (rs.TryCoerceBrep (Guid.NewGuid()))
    )

test "CoerceBrep" (fun () ->
    eq "box has six faces" 6 (rs.CoerceBrep (addBox 1. 1. 1.)).Faces.Count
    throwsRs "not a brep" (fun () -> rs.CoerceBrep (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceBrep (Guid.NewGuid()))
    )

test "TryCoerceCurve" (fun () ->
    ok "curve" ((rs.TryCoerceCurve(line1 ())).IsSome)
    eq "not a curve" None (rs.TryCoerceCurve(pointId ()))
    eq "unknown" None (rs.TryCoerceCurve(Guid.NewGuid()))
    // segments of a polycurve
    let c = polyCurve1 ()
    let seg0 = rs.TryCoerceCurve(c, 0)
    let seg1 = rs.TryCoerceCurve(c, 1)
    ok "segment 0" seg0.IsSome
    nearPt "segment 1 starts where segment 0 ends" seg0.Value.PointAtEnd seg1.Value.PointAtStart
    eq "segment index out of range" None (rs.TryCoerceCurve(c, 5))
    ok "a segment index on a simple curve returns the curve" ((rs.TryCoerceCurve(line1 (), 3)).IsSome)
    )

test "CoerceCurve" (fun () ->
    ok "curve" (not (isNull (rs.CoerceCurve(line1 ()))))
    throwsRs "not a curve" (fun () -> rs.CoerceCurve(pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceCurve(Guid.NewGuid()))
    let pc = polyCurve1 ()
    near "segment length" 5. ((rs.CoerceCurve(pc, 1)).GetLength())
    throwsRs "segment index out of range" (fun () -> rs.CoerceCurve(pc, 5))
    throwsRs "segment index on a non curve" (fun () -> rs.CoerceCurve(pointId (), 0))
    )

// ------------------------------- curve shapes -------------------------------

test "TryCoerceLine" (fun () ->
    let l = Line(pt 0. 0. 0., pt 1. 0. 0.)
    eq "Line" (Some l) (rs.TryCoerceLine l)
    eq "two points" (Some l) (rs.TryCoerceLine(pt 0. 0. 0., pt 1. 0. 0.))
    near "line curve" 10. (rs.TryCoerceLine (line1 ())).Value.Length
    near "from LineCurve geometry" 10. (rs.TryCoerceLine (new LineCurve(pt 0. 0. 0., pt 10. 0. 0.) :> Curve)).Value.Length
    eq "arc is no line" None (rs.TryCoerceLine (arc1 ()))
    eq "null curve" None (rs.TryCoerceLine (null : Curve))
    // a straight polyline is a line within the tolerance
    let straight = rs.AddPolyline [ pt 0. 0. 0.; pt 5. 0. 0.; pt 10. 0. 0. ]
    ok "a straight polyline counts as a line" ((rs.TryCoerceLine straight).IsSome)
    eq "a bent polyline is no line" None (rs.TryCoerceLine (poly1 ()))
    eq "a point is no line" None (rs.TryCoerceLine (pointId ()))
    )

test "CoerceLine" (fun () ->
    let l = Line(pt 0. 0. 0., pt 1. 0. 0.)
    eq "Line" l (rs.CoerceLine l)
    eq "two points" l (rs.CoerceLine(pt 0. 0. 0., pt 1. 0. 0.))
    near "line curve" 10. (rs.CoerceLine (line1 ())).Length
    near "from curve geometry" 10. (rs.CoerceLine (new LineCurve(pt 0. 0. 0., pt 10. 0. 0.) :> Curve)).Length
    throwsRs "arc" (fun () -> rs.CoerceLine (arc1 ()))
    throwsRs "unknown" (fun () -> rs.CoerceLine (Guid.NewGuid()))
    throwsRs "curve geometry that is no line" (fun () -> rs.CoerceLine (new ArcCurve(Circle(5.)) :> Curve))
    )

test "TryCoerceArc" (fun () ->
    let a = Arc(Plane.WorldXY, 5., Math.PI / 2.)
    eq "Arc" (Some a) (rs.TryCoerceArc a)
    near "arc object" 5. (rs.TryCoerceArc (arc1 ())).Value.Radius
    eq "a circle is no arc" None (rs.TryCoerceArc (circle1 ()))
    eq "a line is no arc" None (rs.TryCoerceArc (line1 ()))
    eq "null curve" None (rs.TryCoerceArc (null : Curve))
    ok "from curve geometry" ((rs.TryCoerceArc (new ArcCurve(a) :> Curve)).IsSome)
    )

test "CoerceArc" (fun () ->
    let a = Arc(Plane.WorldXY, 5., Math.PI / 2.)
    eq "Arc" a (rs.CoerceArc a)
    near "arc object" 5. (rs.CoerceArc (arc1 ())).Radius
    near "from curve geometry" 5. (rs.CoerceArc (new ArcCurve(a) :> Curve)).Radius
    throwsRs "circle" (fun () -> rs.CoerceArc (circle1 ()))
    throwsRs "line" (fun () -> rs.CoerceArc (line1 ()))
    throwsRs "unknown" (fun () -> rs.CoerceArc (Guid.NewGuid()))
    )

test "TryCoerceCircle" (fun () ->
    let c = Circle(Plane.WorldXY, 5.)
    eq "Circle" (Some c) (rs.TryCoerceCircle c)
    near "circle object" 5. (rs.TryCoerceCircle (circle1 ())).Value.Radius
    eq "an arc is no circle" None (rs.TryCoerceCircle (arc1 ()))
    eq "null curve" None (rs.TryCoerceCircle (null : Curve))
    ok "from curve geometry" ((rs.TryCoerceCircle (new ArcCurve(c) :> Curve)).IsSome)
    eq "a point is no circle" None (rs.TryCoerceCircle (pointId ()))
    )

test "CoerceCircle" (fun () ->
    let c = Circle(Plane.WorldXY, 5.)
    eq "Circle" c (rs.CoerceCircle c)
    near "circle object" 5. (rs.CoerceCircle (circle1 ())).Radius
    near "from curve geometry" 5. (rs.CoerceCircle (new ArcCurve(c) :> Curve)).Radius
    throwsRs "arc" (fun () -> rs.CoerceCircle (arc1 ()))
    throwsRs "unknown" (fun () -> rs.CoerceCircle (Guid.NewGuid()))
    )

test "TryCoerceEllipse" (fun () ->
    let e = Ellipse(Plane.WorldXY, 5., 3.)
    eq "Ellipse" (Some e) (rs.TryCoerceEllipse e)
    let id = doc().Objects.AddCurve(e.ToNurbsCurve())
    near "ellipse object" 5. (rs.TryCoerceEllipse id).Value.Radius1
    eq "a line is no ellipse" None (rs.TryCoerceEllipse (line1 ()))
    eq "null curve" None (rs.TryCoerceEllipse (null : Curve))
    ok "from curve geometry" ((rs.TryCoerceEllipse (e.ToNurbsCurve() :> Curve)).IsSome)
    )

test "CoerceEllipse" (fun () ->
    let e = Ellipse(Plane.WorldXY, 5., 3.)
    eq "Ellipse" e (rs.CoerceEllipse e)
    let id = doc().Objects.AddCurve(e.ToNurbsCurve())
    near "ellipse object" 5. (rs.CoerceEllipse id).Radius1
    near "from curve geometry" 5. (rs.CoerceEllipse (e.ToNurbsCurve() :> Curve)).Radius1
    throwsRs "line" (fun () -> rs.CoerceEllipse (line1 ()))
    throwsRs "unknown" (fun () -> rs.CoerceEllipse (Guid.NewGuid()))
    )

test "TryCoercePolyline" (fun () ->
    let pl = Polyline([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ])
    ok "Polyline" (Object.ReferenceEquals((rs.TryCoercePolyline pl).Value, pl))
    eq "polyline object has 3 points" 3 (rs.TryCoercePolyline (poly1 ())).Value.Count
    eq "from PolylineCurve" 3 (rs.TryCoercePolyline (pl.ToPolylineCurve() :> Curve)).Value.Count
    eq "a circle is no polyline" None (rs.TryCoercePolyline (circle1 ()))
    eq "null curve" None (rs.TryCoercePolyline (null : Curve))
    eq "a point is no polyline" None (rs.TryCoercePolyline (pointId ()))
    )

test "CoercePolyline" (fun () ->
    let pl = Polyline([ pt 0. 0. 0.; pt 1. 0. 0.; pt 1. 1. 0. ])
    ok "Polyline" (Object.ReferenceEquals(rs.CoercePolyline pl, pl))
    eq "polyline object" 3 (rs.CoercePolyline (poly1 ())).Count
    eq "from curve geometry" 3 (rs.CoercePolyline (pl.ToPolylineCurve() :> Curve)).Count
    throwsRs "circle" (fun () -> rs.CoercePolyline (circle1 ()))
    throwsRs "unknown" (fun () -> rs.CoercePolyline (Guid.NewGuid()))
    )

// ------------------------------- layers, views, annotation -------------------------------

test "CoerceLayer" (fun () ->
    let i = rs.AddLayer "clP::clC"
    let id = doc().Layers.[i].Id
    eq "by full path" id (rs.CoerceLayer "clP::clC").Id
    eq "by short name" id (rs.CoerceLayer "clC").Id
    eq "by Guid" id (rs.CoerceLayer id).Id
    throwsRs "unknown name" (fun () -> rs.CoerceLayer "clMissing")
    throwsRs "unknown Guid" (fun () -> rs.CoerceLayer (Guid.NewGuid()))
    throwsRs "empty Guid" (fun () -> rs.CoerceLayer Guid.Empty)
    throwsRs "the Guid of a document object" (fun () -> rs.CoerceLayer (pointId ()))
    )

test "CoerceBlockInstanceObject" (fun () ->
    let name = rs.AddBlock([ pointId () ], pt 0. 0. 0., "cbBlock", true)
    let inst = rs.InsertBlock(name, pt 0. 0. 0.)
    ok "instance" (not (isNull (rs.CoerceBlockInstanceObject inst)))
    throwsRs "not an instance" (fun () -> rs.CoerceBlockInstanceObject (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceBlockInstanceObject (Guid.NewGuid()))
    )

test "TryCoerceView" (fun () ->
    ok "by name" ((rs.TryCoerceView "Top").IsSome)
    eq "unknown name" None (rs.TryCoerceView "no such view")
    eq "null name" None (rs.TryCoerceView (null : string))
    let id = (rs.TryCoerceView "Top").Value.MainViewport.Id
    ok "by Guid" ((rs.TryCoerceView id).IsSome)
    eq "unknown Guid" None (rs.TryCoerceView (Guid.NewGuid()))
    )

test "CoerceView" (fun () ->
    eq "by name" "Top" (rs.CoerceView "Top").MainViewport.Name
    let id = (rs.CoerceView "Front").MainViewport.Id
    eq "by Guid" "Front" (rs.CoerceView id).MainViewport.Name
    throwsRs "unknown name" (fun () -> rs.CoerceView "no such view")
    throwsRs "null name" (fun () -> rs.CoerceView (null : string))
    throwsRs "unknown Guid" (fun () -> rs.CoerceView (Guid.NewGuid()))
    )

test "CoercePageView" (fun () ->
    let id, name = rs.AddLayout "cpLayout"
    eq "by name" name (rs.CoercePageView name).PageName
    eq "by Guid" name (rs.CoercePageView id).PageName
    throwsRs "a standard view is no page view (by Guid)" (fun () -> rs.CoercePageView ((rs.CoerceView "Top").MainViewport.Id))
    throwsRs "unknown name" (fun () -> rs.CoercePageView "no such layout")
    throwsRs "null name" (fun () -> rs.CoercePageView (null : string))
    throwsRs "unknown Guid" (fun () -> rs.CoercePageView (Guid.NewGuid()))
    )

test "CoerceDetailView" (fun () ->
    let _, layout = rs.AddLayout("cdvLayout", 297., 210.)
    let d = rs.AddDetail(layout, Point2d(10., 10.), Point2d(100., 100.), "cdv")
    ok "detail view geometry" (not (isNull (rs.CoerceDetailView d)))
    throwsRs "not a detail" (fun () -> rs.CoerceDetailView (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceDetailView (Guid.NewGuid()))
    )

test "CoerceDetailViewObject" (fun () ->
    let _, layout = rs.AddLayout("cdoLayout", 297., 210.)
    let d = rs.AddDetail(layout, Point2d(10., 10.), Point2d(100., 100.), "cdo")
    ok "detail view object" (not (isNull (rs.CoerceDetailViewObject d)))
    throwsRs "not a detail" (fun () -> rs.CoerceDetailViewObject (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceDetailViewObject (Guid.NewGuid()))
    )

test "CoerceTextDot" (fun () ->
    let d = rs.AddTextDot("dot", pt 1. 2. 3.)
    eq "text dot" "dot" (rs.CoerceTextDot d).Text
    throwsRs "not a text dot" (fun () -> rs.CoerceTextDot (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceTextDot (Guid.NewGuid()))
    )

test "CoerceTextEntity" (fun () ->
    let t = rs.AddText("hello", Plane.WorldXY, 2.)
    eq "text entity" "hello" (rs.CoerceTextEntity t).PlainText
    throwsRs "not a text" (fun () -> rs.CoerceTextEntity (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceTextEntity (Guid.NewGuid()))
    )

test "CoerceTextObject" (fun () ->
    let t = rs.AddText("hello", Plane.WorldXY, 2.)
    ok "text object" (not (isNull (rs.CoerceTextObject t)))
    throwsRs "not a text" (fun () -> rs.CoerceTextObject (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceTextObject (Guid.NewGuid()))
    )

test "CoerceHatch" (fun () ->
    rs.CurrentHatchPattern "Solid"
    let h = rs.AddHatch (addSquare 5.)
    ok "hatch geometry" (not (isNull (rs.CoerceHatch h)))
    throwsRs "not a hatch" (fun () -> rs.CoerceHatch (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceHatch (Guid.NewGuid()))
    )

test "CoerceHatchObject" (fun () ->
    rs.CurrentHatchPattern "Solid"
    let h = rs.AddHatch (addSquare 5.)
    ok "hatch object" (not (isNull (rs.CoerceHatchObject h)))
    throwsRs "not a hatch" (fun () -> rs.CoerceHatchObject (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceHatchObject (Guid.NewGuid()))
    )

test "CoerceAnnotation" (fun () ->
    let d = rs.AddLinearDimension(pt 0. 0. 0., pt 5. 0. 0., pt 2. 1. 0.)
    ok "dimension" (not (isNull (rs.CoerceAnnotation d)))
    let t = rs.AddText("hello", Plane.WorldXY, 2.)
    ok "text" (not (isNull (rs.CoerceAnnotation t)))
    throwsRs "not an annotation" (fun () -> rs.CoerceAnnotation (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceAnnotation (Guid.NewGuid()))
    )

test "CoerceLeader" (fun () ->
    let l = rs.AddLeader([ pt 0. 0. 0.; pt 5. 5. 0. ], "x")
    ok "leader" (not (isNull (rs.CoerceLeader l)))
    throwsRs "not a leader" (fun () -> rs.CoerceLeader (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceLeader (Guid.NewGuid()))
    )

test "CoerceLinearDimension" (fun () ->
    let d = rs.AddLinearDimension(pt 0. 0. 0., pt 5. 0. 0., pt 2. 1. 0.)
    ok "linear dimension" (not (isNull (rs.CoerceLinearDimension d)))
    throwsRs "not a dimension" (fun () -> rs.CoerceLinearDimension (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceLinearDimension (Guid.NewGuid()))
    )

// ------------------------------- points, vectors, planes -------------------------------

test "Coerce3dPoint" (fun () ->
    nearPt "Point3d" (pt 1. 2. 3.) (rs.Coerce3dPoint(pt 1. 2. 3.))
    nearPt "Point3f" (pt 1. 2. 3.) (rs.Coerce3dPoint(Point3f(1.f, 2.f, 3.f)))
    nearPt "three floats" (pt 1. 2. 3.) (rs.Coerce3dPoint(1., 2., 3.))
    nearPt "sequence" (pt 1. 2. 3.) (rs.Coerce3dPoint([ 1.; 2.; 3. ] : float seq))
    nearPt "string" (pt 1. 2. 3.) (rs.Coerce3dPoint("1, 2, 3"))
    nearPt "string without spaces" (pt 1.5 -2. 3.) (rs.Coerce3dPoint("1.5,-2,3"))
    nearPt "point object Guid" (pt 1. 2. 3.) (rs.Coerce3dPoint(pointId ()))
    nearPt "PointObject" (pt 1. 2. 3.) (rs.Coerce3dPoint(obj (pointId ()) :?> DocObjects.PointObject))
    nearPt "TextDot" (pt 4. 5. 6.) (rs.Coerce3dPoint(rs.CoerceTextDot (rs.AddTextDot("d", pt 4. 5. 6.))))
    throwsRs "sequence with two items" (fun () -> rs.Coerce3dPoint([ 1.; 2. ] : float seq))
    throwsRs "null sequence" (fun () -> rs.Coerce3dPoint(null : float seq))
    throwsRs "string that is no point" (fun () -> rs.Coerce3dPoint("abc"))
    throwsRs "string with two numbers" (fun () -> rs.Coerce3dPoint("1, 2"))
    throwsRs "Guid of a line" (fun () -> rs.Coerce3dPoint(line1 ()))
    throwsRs "unknown Guid" (fun () -> rs.Coerce3dPoint(Guid.NewGuid()))
    throwsRs "null PointObject" (fun () -> rs.Coerce3dPoint(null : DocObjects.PointObject))
    throwsRs "null TextDot" (fun () -> rs.Coerce3dPoint(null : TextDot))
    )

test "CoercePointObject" (fun () ->
    ok "point object" (not (isNull (rs.CoercePointObject (pointId ()))))
    throwsRs "not a point" (fun () -> rs.CoercePointObject (line1 ()))
    throwsRs "unknown" (fun () -> rs.CoercePointObject (Guid.NewGuid()))
    )

test "Coerce2dPoint" (fun () ->
    eq "Point2d" (Point2d(1., 2.)) (rs.Coerce2dPoint(Point2d(1., 2.)))
    eq "Point3d drops z" (Point2d(1., 2.)) (rs.Coerce2dPoint(pt 1. 2. 3.))
    eq "two floats" (Point2d(1., 2.)) (rs.Coerce2dPoint(1., 2.))
    )

test "Coerce3dVector" (fun () ->
    nearVec "Vector3d" (vec 1. 2. 3.) (rs.Coerce3dVector(vec 1. 2. 3.))
    nearVec "Vector3f" (vec 1. 2. 3.) (rs.Coerce3dVector(Vector3f(1.f, 2.f, 3.f)))
    nearVec "three floats" (vec 1. 2. 3.) (rs.Coerce3dVector(1., 2., 3.))
    nearVec "sequence" (vec 1. 2. 3.) (rs.Coerce3dVector([ 1.; 2.; 3. ] : float seq))
    nearVec "string" (vec 1. 2. 3.) (rs.Coerce3dVector("1, 2, 3"))
    nearVec "line Guid: from start to end" (vec 10. 0. 0.) (rs.Coerce3dVector(line1 ()))
    throwsRs "sequence with four items" (fun () -> rs.Coerce3dVector([ 1.; 2.; 3.; 4. ] : float seq))
    throwsRs "null sequence" (fun () -> rs.Coerce3dVector(null : float seq))
    throwsRs "string that is no vector" (fun () -> rs.Coerce3dVector("abc"))
    throwsRs "Guid of an arc" (fun () -> rs.Coerce3dVector(arc1 ()))
    throwsRs "unknown Guid" (fun () -> rs.Coerce3dVector(Guid.NewGuid()))
    )

test "CoercePlane" (fun () ->
    let p = Plane(pt 1. 2. 3., vec 0. 0. 1.)
    eq "Plane" p (rs.CoercePlane p)
    let fromPoint = rs.CoercePlane(pt 1. 2. 3.)
    nearPt "from a point: origin" (pt 1. 2. 3.) fromPoint.Origin
    nearVec "from a point: world xy orientation" Vector3d.ZAxis fromPoint.Normal
    nearPt "from three floats" (pt 4. 5. 6.) (rs.CoercePlane(4., 5., 6.)).Origin
    )

test "CoerceXform" (fun () ->
    let t = Transform.Translation(1., 2., 3.)
    ok "Transform" (rs.CoerceXform t = t)
    let rows = [ [ 1.; 0.; 0.; 5. ]; [ 0.; 1.; 0.; 6. ]; [ 0.; 0.; 1.; 7. ]; [ 0.; 0.; 0.; 1. ] ]
    near "nested lists, row 0 last column" 5. (rs.CoerceXform rows).[0, 3]
    near "nested lists, row 2 last column" 7. (rs.CoerceXform rows).[2, 3]
    let arr = Array2D.init 4 4 (fun r c -> if r = c then 1. else 0.)
    ok "2D array: identity" (rs.CoerceXform arr = Transform.Identity)
    let arr1 = Array2D.zeroCreate 4 4
    arr1.[0, 3] <- 9.
    near "2D array, position" 9. (rs.CoerceXform arr1).[0, 3]
    throwsRs "only three rows" (fun () -> rs.CoerceXform [ [ 1.; 0.; 0.; 0. ]; [ 0.; 1.; 0.; 0. ]; [ 0.; 0.; 1.; 0. ] ])
    throwsRs "a row with three numbers" (fun () -> rs.CoerceXform [ [ 1.; 0.; 0. ]; [ 0.; 1.; 0.; 0. ]; [ 0.; 0.; 1.; 0. ]; [ 0.; 0.; 0.; 1. ] ])
    throwsRs "2D array 3x3" (fun () -> rs.CoerceXform (Array2D.zeroCreate<float> 3 3))
    throwsRs "null 2D array" (fun () -> rs.CoerceXform (null : float[,]))
    )

test "CoerceNurbsSurface" (fun () ->
    ok "single face brep" (not (isNull (rs.CoerceNurbsSurface (addPlaneSurface 2. 2.))))
    ok "sphere" (not (isNull (rs.CoerceNurbsSurface (addSphere 2.))))
    throwsRs "polysurface" (fun () -> rs.CoerceNurbsSurface (addBox 1. 1. 1.))
    throwsRs "not a surface" (fun () -> rs.CoerceNurbsSurface (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoerceNurbsSurface (Guid.NewGuid()))
    )

test "CoercePointCloud" (fun () ->
    let pc = rs.AddPointCloud [| pt 0. 0. 0.; pt 1. 1. 1. |]
    eq "point cloud" 2 (rs.CoercePointCloud pc).Count
    throwsRs "not a point cloud" (fun () -> rs.CoercePointCloud (pointId ()))
    throwsRs "unknown" (fun () -> rs.CoercePointCloud (Guid.NewGuid()))
    )

test "CoerceColor" (fun () ->
    eq "Color" (255, 10, 20, 30) (rgb (rs.CoerceColor (Drawing.Color.FromArgb(10, 20, 30))))
    eq "unnamed color from a named one" (255, 255, 0, 0) (rgb (rs.CoerceColor Drawing.Color.Red))
    ok "a named color becomes an unnamed one" (not (rs.CoerceColor Drawing.Color.Red).IsNamedColor)
    eq "three ints" (255, 1, 2, 3) (rgb (rs.CoerceColor(1, 2, 3)))
    eq "four ints with alpha" (128, 1, 2, 3) (rgb (rs.CoerceColor(1, 2, 3, 128)))
    eq "html color" (255, 255, 0, 0) (rgb (rs.CoerceColor "#FF0000"))
    eq "named html color" (255, 0, 0, 255) (rgb (rs.CoerceColor "Blue"))
    throwsRs "red too big" (fun () -> rs.CoerceColor(256, 0, 0))
    throwsRs "green negative" (fun () -> rs.CoerceColor(0, -1, 0))
    throwsRs "blue too big" (fun () -> rs.CoerceColor(0, 0, 300))
    throwsRs "alpha too big" (fun () -> rs.CoerceColor(0, 0, 0, 256))
    throwsRs "alpha negative" (fun () -> rs.CoerceColor(0, 0, 0, -1))
    throwsRs "empty string" (fun () -> rs.CoerceColor "")
    throwsRs "whitespace string" (fun () -> rs.CoerceColor "   ")
    throwsRs "string that is no color" (fun () -> rs.CoerceColor "notacolor")
    )

skip "CoerceViewOrActive" "internal member, tested through every function that takes an optional view name (Views tests)"

printfn "%s" (finish ())
