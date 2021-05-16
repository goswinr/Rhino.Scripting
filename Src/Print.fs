namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open FsEx.Util
open FsEx.UtilMath
open System.Globalization
open System.Collections.Generic
open FsEx
open FsEx.SaveIgnore
open System.Runtime.CompilerServices



namespace Rhino.Scripting

open System
open System.Runtime.CompilerServices
open Rhino
open Rhino.Geometry
open FsEx  

[<AutoOpen>]
/// This module provides type extensions for pretty printing 
/// This module is automatically opened when Rhino.Scripting namespace is opened.
module AutoOpenToNiceStringExtensions = 
      
    type Point3d with          
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = 
            if pt = Point3d.Unset then  
                "Point3d.Unset"
            else
                sprintf "Point3d(%s, %s, %s)" (NiceFormat.float  pt.X) (NiceFormat.float  pt.Y) (NiceFormat.float  pt.Z) 
     
    type Point3f with          
        ///Like the ToString function but with appropiate precision formating
        member pt.ToNiceString = 
            if pt = Point3f.Unset then  
                "Point3f.Unset"
            else
                sprintf "Point3f(%s, %s, %s)" (NiceFormat.single  pt.X) (NiceFormat.single  pt.Y) (NiceFormat.single  pt.Z)
         
    type Vector3d with         
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  
            if v = Vector3d.Unset then  
                "Vector3d.Unset"
            else
                sprintf "Vector3d(%s, %s, %s)" (NiceFormat.float  v.X) (NiceFormat.float  v.Y) (NiceFormat.float  v.Z)
     
    type Vector3f with
        ///Like the ToString function but with appropiate precision formating
        member v.ToNiceString =  
            if v = Vector3f.Unset then  
                "Vector3f.Unset"
            else
                sprintf "Vector3f(%s, %s, %s)" (NiceFormat.single  v.X) (NiceFormat.single  v.Y) (NiceFormat.single  v.Z)
   
    type Line with
        ///Like the ToString function but with appropiate precision formating
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

[<RequireQualifiedAccess>]
module internal Print =
    
    let mutable doInit = true // to delay setup of printing till first print call, see Print.fs

    /// Gets a description on Rhino object type (curve , point, Surface ....)
    /// including Layer and object name
    let guid (g:Guid)=
        if g = Guid.Empty then 
            "-Guid.Empty-"
        elif Runtime.HostUtils.RunningInRhino  then 
            let o = State.Doc.Objects.FindId(g) 
            if isNull o then sprintf "Guid %O (not in State.Doc.Objects table of this Rhino file)." g
            else
                let name = o.Attributes.Name
                if name <> "" then 
                    sprintf "Guid %O (a %s on Layer %s named '%s')" g (o.ShortDescription(false)) (State.Doc.Layers.[o.Attributes.LayerIndex].FullPath) name
                else 
                    sprintf "Guid %O (an unnamed %s on Layer '%s')" g (o.ShortDescription(false)) (State.Doc.Layers.[o.Attributes.LayerIndex].FullPath)
        else 
            // the Print.guid function gets injected into FsEx.NiceString, below
            // so that using the print function still works on other Guids if Rhino.Scripting is referenced from outside of rhino wher there is no active Doc
            sprintf "Guid %O" g
        



    /// Gets a localized description on Rhino layer and object type (e.g. Curve , point, Surface ....)
    let any (x:'T) =
       match box x with 
       | :? Guid as g -> guid g
       | _ -> NiceString.toNiceString x      
    

    let formatRhinoObject (o:obj) : NiceStringSettings.Lines option= 
          match o with
          | :? Guid       as x -> Some <| NiceStringSettings.Element (guid x)
          | :? Point3d    as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Vector3d   as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Line       as x -> Some <| NiceStringSettings.Element x.ToNiceString        
          | :? Point3f    as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Vector3f   as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Transform  as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | _                  -> None   

    let init()=    
        doInit <- false
        NiceStringSettings.externalFormater  <- formatRhinoObject
        if Rhino.Runtime.HostUtils.RunningInRhino then 
            // these below fail if not running inside rhino.exe
            // scripts that refrence Rhino.Scripting from outside of rhino is still Ok , but all function that call the C++ API dont work
            // but accessing the current Doc obvioiusly does not work.
            NiceStringSettings.roundToZeroBelow                                              <- State.Doc.ModelAbsoluteTolerance  * 0.1 // so that any float smaller than State.Doc.ModelAbsoluteTolerance wil be shown as 0.0
            RhinoApp.AppSettingsChanged.Add    (fun _ -> NiceStringSettings.roundToZeroBelow <- State.Doc.ModelAbsoluteTolerance  * 0.1 )
            RhinoDoc.ActiveDocumentChanged.Add (fun a -> NiceStringSettings.roundToZeroBelow <- a.Document.ModelAbsoluteTolerance * 0.1 )
            RhinoDoc.EndOpenDocument.Add       (fun a -> NiceStringSettings.roundToZeroBelow <- a.Document.ModelAbsoluteTolerance * 0.1 )
