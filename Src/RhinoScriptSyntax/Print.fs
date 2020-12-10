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



/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
[<AutoOpen>]
module ExtensionsPrint =

  type RhinoScriptSyntax with
    
    [<Extension>]
    ///<summary>Returns a nice string for any kinds of objects or values, for most objects this is just calling *.ToString()</summary>
    ///<param name="x">('T): the value or object to represent as string</param>
    ///<param name="trim">(bool) Optional, Default Value: <c>true</c>
    /// Applicable if the value x is a Seq: If true  the string will only show the first 4 items per seq or nested seq. If false all itemes will be in the string</param>
    ///<returns>(string) the string</returns>
    static member ToNiceString (x:'T, [<OPT;DEF(true)>]trim:bool) : string = 
        let formatRhinoObject (o:obj)  = 
            match o with
            | :? Guid       as x -> Some (rhType x)
            | :? Point3d    as x -> Some x.ToNiceString
            | :? Vector3d   as x -> Some x.ToNiceString
            | :? Line       as x -> Some x.ToNiceString        
            | :? Point3f    as x -> Some x.ToNiceString
            | :? Vector3f   as x -> Some x.ToNiceString
            | _                  -> None        
        if trim then NiceString.toNiceStringWithFormater    (x, formatRhinoObject)
        else         NiceString.toNiceStringFullWithFormater(x, formatRhinoObject)       
    
    [<Extension>]
    ///<summary>Prints an object or value to Seff editor (if present,otherwise to StandardOut stream) and to Rhino Command line. 
    ///    If the value is a Seq the string will only show the first 4 items per seq or nested seq</summary>
    ///<param name="x">('T): the value or object to print</param>
    ///<returns>(unit) void, nothing</returns>
    static member Print (x:'T) : unit =
        RhinoScriptSyntax.ToNiceString(x, true)
        |>! RhinoApp.WriteLine 
        |> printfn "%s"  
        RhinoApp.Wait() // no swith to UI Thread needed !
 
    [<Extension>]
    ///<summary>Prints an object or value to Seff editor (if present,otherwise to StandardOut stream) and to Rhino Command line.
    ///    If the value is a Seq the string will contain a line for each item and per nested item.</summary>
    ///<param name="x">('T): the value or object to print</param>   
    ///<returns>(unit) void, nothing</returns>
    static member PrintFull (x:'T) : unit =
        RhinoScriptSyntax.ToNiceString(x, false)
        |>! RhinoApp.WriteLine 
        |> printfn "%s"  
        RhinoApp.Wait() // no swith to UI Thread needed !

    [<Extension>]
    ///<summary>Prints Sequence of objects or values separated by a space charcter or a custom value</summary>
    ///<param name="xs">('T): the values or objects to print</param>
    ///<param name="separator">(string) Optional, Default Value: a space character <c>" "</c></param>
    ///<returns>(unit) void, nothing</returns>
    static member PrintSeq (xs:'T seq, [<OPT;DEF(" ")>]separator:string) : unit =
        xs
        |> Seq.map RhinoScriptSyntax.ToNiceString
        |> String.concat separator
        |>! RhinoApp.WriteLine 
        |> printfn "%s"
        RhinoApp.Wait()

    //printf:

    [<Extension>]
    /// Like printf but in Red. Does not add a new line at end.
    static member PrintfRed msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Synchronisation.ColorLogger 220 0 0  s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !

    [<Extension>]
    /// Like printf but in Green. Does not add a new line at end.
    static member PrintfGreen msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Synchronisation.ColorLogger 0 220 0  s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !

    [<Extension>]
    /// Like printf but in Light Blue. Does not add a new line at end.
    static member PrintfLightBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Synchronisation.ColorLogger 173 216 230    s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !

    [<Extension>]
    /// Like printf but in Blue. Does not add a new line at end.
    static member PrintfBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Synchronisation.ColorLogger 0 0 220   s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !


    //printfn:

    [<Extension>]
    /// Like printf but in Red. Adds a new line at end.
    static member PrintfnRed msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Synchronisation.ColorLoggerNl 220 0 0  s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !

    [<Extension>]
    /// Like printf but in Green.Adds a new line at end.
    static member PrintfnGreen msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Synchronisation.ColorLoggerNl 0 220 0  s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !

    [<Extension>]
    /// Like printf but in Light Blue. Adds a new line at end.
    static member PrintfnLightBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Synchronisation.ColorLoggerNl 173 216 230    s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !

    [<Extension>]
    /// Like printf but in Blue. Adds a new line at end.
    static member PrintfnBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Synchronisation.ColorLoggerNl 0 0 220   s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !

    
    [<Extension>]
    ///<summary>Like printf but in costom color. Does not add a new line at end.</summary>
    ///<param name="red">(int) Red Value between 0 and 255 </param>
    ///<param name="green">(int) Green value between 0 and 255 </param>
    ///<param name="blue">(int) Blue value between 0 and 255 </param>
    ///<param name="msg">The format string</param>
    ///<returns>(unit) void, nothing</returns>
    static member PrintfColor (red:int) (green:int) (blue:int) msg  =
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Synchronisation.ColorLogger red green blue  s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !
    
    [<Extension>]
    ///<summary>Like printfn but in costom color. Adds a new line at end. </summary>
    ///<param name="red">(int) Red Value between 0 and 255 </param>
    ///<param name="green">(int) Green value between 0 and 255 </param>
    ///<param name="blue">(int) Blue value between 0 and 255 </param>
    ///<param name="msg">The format string</param>
    ///<returns>(unit) void, nothing</returns>
    static member PrintfnColor (red:int) (green:int) (blue:int) msg  =
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Synchronisation.ColorLoggerNl red green blue  s
            RhinoApp.Wait() )  msg // no swith to UI Thread needed !