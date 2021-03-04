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
    
    static member internal formatRhinoObject (o:obj)  = 
        match o with
        | :? Guid       as x -> Some <| NiceStringSettings.Element (rhType x)
        | :? Point3d    as x -> Some <| NiceStringSettings.Element x.ToNiceString
        | :? Vector3d   as x -> Some <| NiceStringSettings.Element x.ToNiceString
        | :? Line       as x -> Some <| NiceStringSettings.Element x.ToNiceString        
        | :? Point3f    as x -> Some <| NiceStringSettings.Element x.ToNiceString
        | :? Vector3f   as x -> Some <| NiceStringSettings.Element x.ToNiceString
        | _                  -> None        
    
    ///<summary>Returns a nice string for any kinds of objects or values, for most objects this is just calling *.ToString().</summary>
    ///<param name="x">('T): the value or object to represent as string</param>
    ///<param name="trim">(bool) Optional, Default Value: <c>true</c>
    /// Applicable if the value x is a Seq: If true  the string will only show the first 4 items per seq or nested seq. If false all itemes will be in the string</param>
    ///<returns>(string) The string.</returns>
    [<Extension>]
    static member ToNiceString (x:'T, [<OPT;DEF(true)>]trim:bool) : string = 
        if trim then NiceString.toNiceString(x)
        else         NiceString.toNiceStringFull(x)       
    
    ///<summary>Prints an object or value to Seff editor (if present,otherwise to StandardOut stream) and to Rhino Command line. 
    ///    If the value is a Seq the string will only show the first 4 items per seq or nested seq.</summary>
    ///<param name="x">('T): the value or object to print</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member Print (x:'T) : unit =
        RhinoScriptSyntax.ToNiceString(x, true)
        |>! RhinoApp.WriteLine 
        |>  Console.WriteLine  
        RhinoApp.Wait() // no swith to UI Thread needed !
 
    ///<summary>Prints an object or value to Seff editor (if present,otherwise to StandardOut stream) and to Rhino Command line.
    ///    If the value is a Seq the string will contain a line for each item and per nested item.</summary>
    ///<param name="x">('T): the value or object to print</param>   
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member PrintFull (x:'T) : unit =
        RhinoScriptSyntax.ToNiceString(x, false)
        |>! RhinoApp.WriteLine 
        |>  Console.WriteLine  
        RhinoApp.Wait() // no swith to UI Thread needed !

    ///<summary>Prints Sequence of objects or values separated by a space charcter or a custom value.</summary>
    ///<param name="xs">('T): the values or objects to print</param>
    ///<param name="separator">(string) Optional, Default Value: a space character <c>" "</c></param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member PrintSeq (xs:'T seq, [<OPT;DEF(" ")>]separator:string) : unit =
        xs
        |>  Seq.map RhinoScriptSyntax.ToNiceString
        |>  String.concat separator
        |>! RhinoApp.WriteLine 
        |>  Console.WriteLine
        RhinoApp.Wait()

    //printf:
    
    /// Like printf but in Red. Does not add a new line at end.
    [<Extension>]
    static member PrintfRed msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            printfColor 220 0 0 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
            
    /// Like printf but in Red. Adds a new line at end.
    [<Extension>]
    static member PrintfnRed msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            printfnColor 220 0 0 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Green. Does not add a new line at end.
    [<Extension>]
    static member PrintfGreen msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            printfColor 0 180 0 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
            
    /// Like printf but in Green.Adds a new line at end.
    [<Extension>]
    static member PrintfnGreen msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            printfnColor 0 180 0 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Light Blue. Does not add a new line at end.
    [<Extension>]
    static member PrintfLightBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            printfColor 173 216 230  "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Light Blue. Adds a new line at end.
    [<Extension>]
    static member PrintfnLightBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            printfnColor 173 216 230  "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Blue. Does not add a new line at end.
    [<Extension>]
    static member PrintfBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            printfColor 0 0 220 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Blue. Adds a new line at end.
    [<Extension>]
    static member PrintfnBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            printfnColor 0 0 220 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Grey. Does not add a new line at end.
    [<Extension>]
    static member PrintfGrey msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            printfColor 150 150 150 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Grey. Adds a new line at end.
    [<Extension>]
    static member PrintfnGrey msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            printfnColor 150 150 150 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
    
    ///<summary>Like printf but in costom color. Does not add a new line at end.</summary>
    ///<param name="red">(int) Red Value between 0 and 255 </param>
    ///<param name="green">(int) Green value between 0 and 255 </param>
    ///<param name="blue">(int) Blue value between 0 and 255 </param>
    ///<param name="msg">The format string</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member PrintfColor (red:int) (green:int) (blue:int) msg  =
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            printfColor red green blue"%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
    
    ///<summary>Like printfn but in costom color. Adds a new line at end. .</summary>
    ///<param name="red">(int) Red Value between 0 and 255 </param>
    ///<param name="green">(int) Green value between 0 and 255 </param>
    ///<param name="blue">(int) Blue value between 0 and 255 </param>
    ///<param name="msg">The format string</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member PrintfnColor (red:int) (green:int) (blue:int) msg  =
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            printfnColor red green blue"%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
  
  do
    NiceStringSettings.externalFormater                                              <- RhinoScriptSyntax.formatRhinoObject

    NiceStringSettings.roundToZeroBelow                                              <- Doc.ModelAbsoluteTolerance * 0.1 // so that any float small than Doc.ModelAbsoluteTolerance wil be shown as 0.0
    RhinoApp.AppSettingsChanged.Add    (fun _ -> NiceStringSettings.roundToZeroBelow <- Doc.ModelAbsoluteTolerance * 0.1 )
    RhinoDoc.ActiveDocumentChanged.Add (fun _ -> NiceStringSettings.roundToZeroBelow <- Doc.ModelAbsoluteTolerance * 0.1 )
    

  //TODO add these too becaue printcolorfn does not print to rhino command window?

  (*


  ///same as RhinoScriptSyntax.Print (shadows print from FsEx)
  let print x = RhinoScriptSyntax.Print x 
  
  /// shadowing the default printf to also print to Rhino Command line
  let printf   msg= Printf.kprintf (fun s -> s |>! RhinoApp.Write     |> printf "%s"   ; RhinoApp.Wait()) msg // no swith to UI Thread needed !
  
  /// shadowing the default printfn to also print to Rhino Command line
  let printfn  msg= Printf.kprintf (fun s -> s |>! RhinoApp.WriteLine |> Console.WriteLine  ; RhinoApp.Wait()) msg // no swith to UI Thread needed !
  
  /// shadowing the default eprintf to also print to Rhino Command line
  let eprintf  msg= Printf.kprintf (fun s -> s |>! RhinoApp.Write     |> eprintf "%s"  ; RhinoApp.Wait()) msg // no swith to UI Thread needed !
  
  /// shadowing the default eprintfn to also print to Rhino Command line
  let eprintfn msg= Printf.kprintf (fun s -> s |>! RhinoApp.WriteLine |> eConsole.WriteLine ; RhinoApp.Wait()) msg // no swith to UI Thread needed !
  
  /// prints two values separated by a space using FsEx.NiceString.toNiceString
  ///(shadows print2 from FsEx)
  let print2 x y = RhinoScriptSyntax.Print (sprintf "%s %s" (NiceString.toNiceString x) (NiceString.toNiceString y))
    
  /// prints three values separated by a space using FsEx.NiceString.toNiceString
  ///(shadows print3 from FsEx)
  let print3 x y z = RhinoScriptSyntax.Print (sprintf "%s %s %s" (NiceString.toNiceString x) (NiceString.toNiceString y) (NiceString.toNiceString z))
  
  /// prints four values separated by a space using FsEx.NiceString.toNiceString
  ///(shadows print4 from FsEx)
  let print4 w x y z = RhinoScriptSyntax.Print (sprintf "%s %s %s %s" (NiceString.toNiceString w) (NiceString.toNiceString x) (NiceString.toNiceString y) (NiceString.toNiceString z))
  
  ///RhinoScriptSyntax.PrintFull (shadows printFull from FsEx)
  let printFull x = RhinoScriptSyntax.PrintFull x //shadows FsEx.TypeExtensionsObject.printFull
    
  *)
