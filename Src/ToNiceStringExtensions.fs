namespace Rhino.Scripting

open Rhino
open Rhino.Geometry
open FsEx

/// This module provides type extensions for pretty printing.
/// It adds a 'rhObj.ToNiceString' property to many Rhino geometry objects.
/// This module is automatically opened when Rhino namespace is opened.
/// These type extensions are only visible in F# (not in C#)
/// This auto-open module is under the 'Rhino.Scripting' namespace so that making these extensions available works 
/// with 'open Rhino.Scripting'. 
[<AutoOpen>]
module AutoOpenToNiceStringExtensions = 

    type Point3d with
        ///Like the ToString function but with appropriate precision formatting
        member pt.ToNiceString = 
            if pt = Point3d.Unset then
                "Point3d.Unset"
            else
                sprintf "Point3d(%s, %s, %s)" (NiceFormat.float  pt.X) (NiceFormat.float  pt.Y) (NiceFormat.float  pt.Z)

    type Point3f with
        ///Like the ToString function but with appropriate precision formatting
        member pt.ToNiceString = 
            if pt = Point3f.Unset then
                "Point3f.Unset"
            else
                sprintf "Point3f(%s, %s, %s)" (NiceFormat.single  pt.X) (NiceFormat.single  pt.Y) (NiceFormat.single  pt.Z)

    type Vector3d with
        ///Like the ToString function but with appropriate precision formatting
        member v.ToNiceString = 
            if v = Vector3d.Unset then
                "Vector3d.Unset"
            else
                sprintf "Vector3d(%s, %s, %s)" (NiceFormat.float  v.X) (NiceFormat.float  v.Y) (NiceFormat.float  v.Z)

    type Vector3f with
        ///Like the ToString function but with appropriate precision formatting
        member v.ToNiceString = 
            if v = Vector3f.Unset then
                "Vector3f.Unset"
            else
                sprintf "Vector3f(%s, %s, %s)" (NiceFormat.single  v.X) (NiceFormat.single  v.Y) (NiceFormat.single  v.Z)

    type Line with
        ///Like the ToString function but with appropriate precision formatting
        member ln.ToNiceString = 
            sprintf "Geometry.Line from %s, %s, %s to %s, %s, %s" (NiceFormat.float  ln.From.X) (NiceFormat.float  ln.From.Y) (NiceFormat.float  ln.From.Z) (NiceFormat.float  ln.To.X) (NiceFormat.float  ln.To.Y) (NiceFormat.float  ln.To.Z)

    type Transform with
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
            str{
                yield! "Rhino.Geometry.Transform:"
                for i,s in Seq.indexed vs do
                    let coli = i%4
                    let len = cols.[coli]
                    yield "| "
                    yield String.padLeftWith len ' ' s
                    if coli = 3 then yield! ""
                //yield! sprintf "Scale x: %g ; y: %g z: %g" m.M00 m.M11 m.M22
                }

    type Plane with
        /// returns a string showing the Transformation Matrix in an aligned 4x4 grid
        member p.ToNiceString = 
            if not p.IsValid then "invalid Rhino.Geometry.Plane"
            else
                str{
                    yield! "Rhino.Geometry.Plane:"
                    yield! sprintf "Origin X=%s Y=%s Z=%s" (NiceFormat.float  p.Origin.X)(NiceFormat.float  p.Origin.Y) (NiceFormat.float  p.Origin.Z)
                    yield! sprintf "X-Axis X=%s Y=%s Z=%s" (NiceFormat.float  p.XAxis.X) (NiceFormat.float  p.XAxis.Y) (NiceFormat.float  p.XAxis.Z)
                    yield! sprintf "Y-Axis X=%s Y=%s Z=%s" (NiceFormat.float  p.YAxis.X) (NiceFormat.float  p.YAxis.Y) (NiceFormat.float  p.YAxis.Z)                
                    }
                
    type BoundingBox with
        /// returns a string showing the Transformation Matrix in an aligned 4x4 grid
        member b.ToNiceString =            
            str{ 
                if   b.IsDegenerate(Rhino.Scripting.State.Doc.ModelAbsoluteTolerance) > 0 then yield! "flat Rhino.Geometry.BoundingBox"
                elif not b.IsValid then yield! "invalid (decreasing?) Rhino.Geometry.BoundingBox"
                else                    yield! "Rhino.Geometry.BoundingBox:"
                yield! sprintf "Size X=%s Y=%s Z=%s" (NiceFormat.float b.Diagonal .X) (NiceFormat.float  b.Diagonal .Y) (NiceFormat.float  b.Diagonal .Z)
                yield! sprintf "from X=%s Y=%s Z=%s" (NiceFormat.float  b.Min.X) (NiceFormat.float  b.Min.Y) (NiceFormat.float  b.Min.Z)
                yield! sprintf "till X=%s Y=%s Z=%s" (NiceFormat.float  b.Max.X) (NiceFormat.float  b.Max.Y) (NiceFormat.float  b.Max.Z)                
                }


