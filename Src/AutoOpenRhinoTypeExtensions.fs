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
module AutoOpenRhinoTypeExtensions =  
    open Rhino.Geometry
    open FsEx
    

    type Point3d with  
        
        [<Extension>]     
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = 
            if pt = Point3d.Unset then  
                "Point3d.Unset"
            else
                sprintf "Point3d(%s, %s, %s)" (NiceFormat.float  pt.X) (NiceFormat.float  pt.Y) (NiceFormat.float  pt.Z)
        
        
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
                sprintf "Point3f(%s, %s, %s)" (NiceFormat.single  pt.X) (NiceFormat.single  pt.Y) (NiceFormat.single  pt.Z)
        
        
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
                sprintf "Vector3d(%s, %s, %s)" (NiceFormat.float  v.X) (NiceFormat.float  v.Y) (NiceFormat.float  v.Z)
        
        /// To convert Vector3d (as it is used in most other Rhino Geometries) to a Vector3f (as it is used in mesh noramls)
        [<Extension>] 
        member v.ToVector3f = Vector3f(float32 v.X, float32 v.Y, float32 v.Z) 
        
        [<Extension>] 
        ///Unitizes the vector , checks input length to be longer than  1e-9 units
        member v.Unitized = 
            let len = sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z) // see Vec.unitze too
            if len > 1e-9 then v * (1./len) 
            else RhinoScriptingException.Raise "Vector3d.Unitized: %s is too small for unitizing, tol: 1e-9" v.ToNiceString
        
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
                sprintf "Vector3f(%s, %s, %s)" (NiceFormat.single  v.X) (NiceFormat.single  v.Y) (NiceFormat.single  v.Z)
   
        /// To convert a Vector3f (as it is used in mesh noramls) to a Vector3d (as it is used in most other Rhino Geometries)
        [<Extension>] 
        member v.ToVector3d = Vector3d(v)

  
    type Line with  
        
        [<Extension>]     
        ///Like the ToString function but with appropiate precision formating
        member ln.ToNiceString = 
            sprintf "Geometry.Line from %s, %s, %s to %s, %s, %s" (NiceFormat.float  ln.From.X) (NiceFormat.float  ln.From.Y) (NiceFormat.float  ln.From.Z) (NiceFormat.float  ln.To.X) (NiceFormat.float  ln.To.Y) (NiceFormat.float  ln.To.Z)
    
        [<Extension>]     
        ///Middle point of line
        member ln.Mid =  (ln.From + ln.To) * 0.5

    type Plane with  
      
         [<Extension>]     
         ///WorldXY rotate 180 degrees round Z Axis
         static member WorldMinusXMinusY=  
            Plane(Point3d.Origin, -Vector3d.XAxis, -Vector3d.YAxis)

         [<Extension>]     
         ///WorldXY rotate 90 degrees round Z Axis counter clockwise
         static member WorldYMinusX=  
            Plane(Point3d.Origin, Vector3d.YAxis, -Vector3d.XAxis)

         [<Extension>]     
         ///WorldXY rotate 270 degrees round Z Axis counter clockwise
         static member WorldMinusYX=  
            Plane(Point3d.Origin, -Vector3d.YAxis, Vector3d.XAxis)

         [<Extension>]     
         ///WorldXY rotate 180 degrees round X Axis, Z points down now
         static member WorldXMinusY=  
            Plane(Point3d.Origin, Vector3d.XAxis, -Vector3d.YAxis)


    type Transform with 
        
        [<Extension>] 
        /// returns a string showing the Transformation Matrix in an aligned 4x4 grid
        member m.ToNiceString =  
            let vs = 
                m.ToFloatArray(true)
                |> Array.map (sprintf "%g")
            let cols = 
                [| for i=0 to 3 do
                    [| vs.[0+i];vs.[4+i];vs.[8+i];vs.[12+i] |]
                    |> Array.map String.length
                    |> Array.max
                |]
                
            stringBuffer{
                yield! "Rhino.Geometry.Transform:"
                for i,s in Seq.indexed vs do
                    let coli = i%4
                    let len = cols.[coli]                
                    yield "| "
                    yield String.prefixToLength len ' ' s
                    if coli = 3 then yield! ""
                //yield! sprintf "Scale x: %g ; y: %g z: %g" m.M00 m.M11 m.M22
                }
    (*
    type Mesh with 
        [<Extension>]
        static member join (meshes:Mesh seq) : Mesh =  // use rs.JoinMeshes overload
            let j = new Mesh()
            j.Append(meshes)
            j
        *)
