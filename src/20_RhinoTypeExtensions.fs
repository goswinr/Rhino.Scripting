namespace Rhino.Scripting

open System.Runtime.CompilerServices

// to expose CLI-standard extension members that can be consumed from C# or VB,
// http://www.latkin.org/blog/2014/04/30/f-extension-methods-in-roslyn/
// declare all Extension attributes explicitly
[<assembly:Extension>] do () 


[<AutoOpen>]
/// This module provides  type extensions for Points , Vector,  Lines
/// Mostly for pretty printing and coneversion to other types
/// This module is automatically opened when Rhino.Scripting namspace is opened.
module ExtraRhinoTypeExtensions =  
    open Rhino.Geometry
    open FsEx

    type Point3d with  
        
        [<Extension>]     
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = 
            if pt = Point3d.Unset then  
                "Point3d.Unset"
            else
                sprintf "Point3d(%s, %s, %s)" (NiceString.floatToString  pt.X) (NiceString.floatToString  pt.Y) (NiceString.floatToString  pt.Z)
        
        
        /// To convert a Point3d (as it is used in most other Rhino Geometries) to Point3f (as it is used in meshes)
        [<Extension>] 
        member pt.ToPoint3f = Point3f(float32 pt.X, float32 pt.Y, float32 pt.Z)
     

     
    type Point3f with  
        
        [<Extension>]  
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = 
            if pt = Point3f.Unset then  
                "Point3f.Unset"
            else
                sprintf "Point3f(%s, %s, %s)" (NiceString.singleToString  pt.X) (NiceString.singleToString  pt.Y) (NiceString.singleToString  pt.Z)
        
        
        /// To convert a Point3f (as it is used in meshes) to Point3d (as it is used in most other Rhino Geometries)
        [<Extension>] 
        member pt.ToPoint3d = Point3d(pt)

     
    type Vector3d with  
        
        [<Extension>] 
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  
            if v = Vector3d.Unset then  
                "Vector3d.Unset"
            else
                sprintf "Vector3d(%s, %s, %s)" (NiceString.floatToString  v.X) (NiceString.floatToString  v.Y) (NiceString.floatToString  v.Z)
        
        /// To convert Vector3d (as it is used in most other Rhino Geometries) to a Vector3f (as it is used in mesh noramls)
        [<Extension>] 
        member v.ToVector3f = Vector3f(float32 v.X, float32 v.Y, float32 v.Z) 
        
        [<Extension>] 
        ///Unitizes the vector , checks input length to be longer than  1e-9 units
        member v.Unitized = 
            let len = sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) // see Vec.unitze too
            if len > 1e-9 then v * (1./len) 
            else failwithf "v.Unitized: %s is too small for unitizing, tol: 1e-9" v.ToNiceString
        
        //[<Extension>] 
        //Unitizes the vector , fails if input is of zero length
        //member inline v.UnitizedUnchecked = let f = 1. / sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) in Vector3d(v.X*f, v.Y*f, v.Z*f)


    type Vector3f with  
        
        [<Extension>] 
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  
            if v = Vector3f.Unset then  
                "Vector3f.Unset"
            else
                sprintf "Vector3f(%s, %s, %s)" (NiceString.singleToString  v.X) (NiceString.singleToString  v.Y) (NiceString.singleToString  v.Z)
   
        /// To convert a Vector3f (as it is used in mesh noramls) to a Vector3d (as it is used in most other Rhino Geometries)
        [<Extension>] 
        member v.ToVector3d = Vector3d(v)

  
    type Line with  
        
        [<Extension>]     
        ///Like the ToString function but with appropiate precision formating
        member ln.ToNiceString = 
            sprintf "Geometry.Line from %s, %s, %s to %s, %s, %s" (NiceString.floatToString  ln.From.X) (NiceString.floatToString  ln.From.Y) (NiceString.floatToString  ln.From.Z) (NiceString.floatToString  ln.To.X) (NiceString.floatToString  ln.To.Y) (NiceString.floatToString  ln.To.Z)
    
        [<Extension>]     
        ///Middle point of line
        member ln.Mid =  (ln.From + ln.To) * 0.5

             
    (*
    type Mesh with 
        [<Extension>]
        static member join (meshes:Mesh seq) : Mesh =  // use rs.JoinMeshes overload
            let j = new Mesh()
            j.Append(meshes)
            j
        *)