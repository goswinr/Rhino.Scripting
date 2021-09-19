namespace Rhino.Scripting

open System.Runtime.CompilerServices
open Rhino.Geometry 

[<AutoOpen>]
/// This module provides type extensions for Points , Vector,  Lines
/// Mostly for conversion to other types
/// This module is automatically opened when Rhino.Scripting namespace is opened.
module AutoOpenRhinoTypeExtensions =  
       
    
    // Extensions for .ToNiceString are in Print Module !!

    type Point3d with 

        /// To convert a Point3d (as it is used in most other Rhino Geometries) to Point3f (as it is used in Meshes)
        member pt.ToPoint3f = Point3f(float32 pt.X, float32 pt.Y, float32 pt.Z)
     

     
    type Point3f with         
        
        /// To convert a Point3f (as it is used in Meshes) to Point3d (as it is used in most other Rhino Geometries)
        member pt.ToPoint3d = Point3d(pt)

     
    type Vector3d with  
        
        /// To convert Vector3d (as it is used in most other Rhino Geometries) 
        /// to a Vector3f (as it is used in Mesh noramls)
        member v.ToVector3f = Vector3f(float32 v.X, float32 v.Y, float32 v.Z) 
        
        /// Unitizes the vector. 
        /// Checks input length to be longer than  1e-9 units
        member v.Unitized = 
            let len = sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) // see Vec.unitze too
            if len > 1e-9 then v * (1./len) 
            else RhinoScriptingException.Raise "Vector3d.Unitized: %s is too small for unitizing, tol: 1e-9" v.ToNiceString
        
        //[<Extension>] 
        //Unitizes the vector , fails if input is of zero length
        //member inline v.UnitizedUnchecked = let f = 1. / sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) in Vector3d(v.X*f, v.Y*f, v.Z*f)


    type Vector3f with  
        
        /// To convert a Vector3f (as it is used in Mesh noramls) 
        /// to a Vector3d (as it is used in most other Rhino Geometries)
        member v.ToVector3d = Vector3d(v)

  
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
        
        /// Gets a seq (= IEnumerable) of the Points that make up the Polyline.
        member pl.Points = 
            seq { for i = 0 to pl.PointCount - 1 do pl.Point(i) }
    
    (*
    type Mesh with 
        static member join (meshes:Mesh seq) : Mesh =  // use rs.JoinMeshes overload
            let j = new Mesh()
            j.Append(meshes)
            j
        *)
