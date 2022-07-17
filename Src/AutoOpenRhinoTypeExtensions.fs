namespace Rhino

open Rhino.Geometry

[<AutoOpen>]
/// This module provides type extensions for Points , Vector,  Lines
/// Mostly for conversion to other types
/// This module is automatically opened when Rhino.Scripting namespace is opened.
/// These type extensions are only visible in F#.
module AutoOpenRhinoTypeExtensions = 

    // NOTE: Extensions for .ToNiceString are in Print Module !!

    type Point3d with

        /// To convert a Point3d (as it is used in most other Rhino Geometries) to Point3f (as it is used in Meshes)
        member pt.ToPoint3f = Point3f(float32 pt.X, float32 pt.Y, float32 pt.Z)

        /// Take any object that has an X, Y and Z member and try to convert each to a float 
        static member inline ofXYZmembers pt  = 
            let x = ( ^T : (member X : _) pt)
            let y = ( ^T : (member Y : _) pt)
            let z = ( ^T : (member Z : _) pt)
            try
                Point3d(float x, float y, float z) 
            with e ->
                RhinoScriptingException.Raise "Point3d.ofXYZmembers: %A could not be converted to a Point3d:\r\n%A" pt e

    type Point3f with

        /// To convert a Point3f (as it is used in Meshes) to Point3d (as it is used in most other Rhino Geometries)
        member pt.ToPoint3d = Point3d(pt)
        
        /// Duck typing using SRTP.
        /// Take any object that has an X, Y and Z member and try to convert each to a float32 
        static member inline ofXYZmembers pt  = 
            let x = ( ^T : (member X : _) pt)
            let y = ( ^T : (member Y : _) pt)
            let z = ( ^T : (member Z : _) pt)
            try
                Point3f(float32 x, float32 y, float32 z) 
            with e ->
                RhinoScriptingException.Raise "Point3f.ofXYZmembers: %A could not be converted to a Point3f:\r\n%A" pt e

    type Vector3d with

        /// To convert Vector3d (as it is used in most other Rhino Geometries)
        /// to a Vector3f (as it is used in Mesh normals)
        member v.ToVector3f = Vector3f(float32 v.X, float32 v.Y, float32 v.Z)

        /// Duck typing using SRTP.
        /// Take any object that has an X, Y and Z member and try to convert each to a float 
        static member inline ofXYZmembers pt  = 
            let x = ( ^T : (member X : _) pt)
            let y = ( ^T : (member Y : _) pt)
            let z = ( ^T : (member Z : _) pt)
            try
                Vector3d(float x, float y, float z) 
            with e ->
                RhinoScriptingException.Raise "Vector3d.ofXYZmembers: %A could not be converted to a Vector3d:\r\n%A" pt e

        /// Unitizes the vector.
        /// Checks input length to be longer than  1e-9 units
        member v.Unitized = 
            let len = sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) // see Vec.unitize too
            if len > 1e-9 then v * (1./len)
            else RhinoScriptingException.Raise "Vector3d.Unitized: %s is too small for unitizing, tol: 1e-9" v.ToNiceString

        //[<Extension>]
        //Unitizes the vector , fails if input is of zero length
        //member inline v.UnitizedUnchecked = let f = 1. / sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) in Vector3d(v.X*f, v.Y*f, v.Z*f)


    type Vector3f with

        /// To convert a Vector3f (as it is used in Mesh normals)
        /// to a Vector3d (as it is used in most other Rhino Geometries)
        member v.ToVector3d = Vector3d(v)

        /// Duck typing using SRTP.
        /// Take any object that has an X, Y and Z member and try to convert each to a float 
        static member inline ofXYZmembers pt  = 
            let x = ( ^T : (member X : _) pt)
            let y = ( ^T : (member Y : _) pt)
            let z = ( ^T : (member Z : _) pt)
            try
                Vector3f(float32 x, float32 y, float32 z) 
            with e ->
                RhinoScriptingException.Raise "Vector3f.ofXYZmembers: %A could not be converted to a Vector3f:\r\n%A" pt e

    type Line with

        /// Get middle point of line
        member inline ln.Mid =  (ln.From + ln.To) * 0.5

        /// Returns a new instance of a reversed line
        member inline ln.Reversed =  Line(ln.To,ln.From)

    type Plane with

        /// WorldXY rotated 180 degrees round Z Axis
        static member WorldMinusXMinusY= 
            Plane(Point3d.Origin, -Vector3d.XAxis, -Vector3d.YAxis)

        /// WorldXY rotated 90 degrees round Z Axis counter clockwise from top
        static member WorldYMinusX= 
            Plane(Point3d.Origin, Vector3d.YAxis, -Vector3d.XAxis)

        /// WorldXY rotated 270 degrees round Z Axis counter clockwise from top
        static member WorldMinusYX= 
            Plane(Point3d.Origin, -Vector3d.YAxis, Vector3d.XAxis)

        /// WorldXY rotated 180 degrees round X Axis, Z points down now
        static member WorldXMinusY= 
            Plane(Point3d.Origin, Vector3d.XAxis, -Vector3d.YAxis)

    type PolylineCurve with

        /// Gets a lazy seq (= IEnumerable) of the Points that make up the Polyline.
        member pl.Points = 
            seq { for i = 0 to pl.PointCount - 1 do pl.Point(i) }

    (*
    type Mesh with
        static member join (meshes:Mesh seq) : Mesh =  // use rs.JoinMeshes overload
            let j = new Mesh()
            j.Append(meshes)
            j
        *)
