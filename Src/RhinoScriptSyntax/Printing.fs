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


/// This module is automatically opened when Rhino.Scripting namespace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
[<AutoOpen>]
module ExtensionsPrinting =
  
  type RhinoScriptSyntax with    
    
    ///<summary>
    /// Nice formating for numbers including thousand Separator and (nested) sequences, first five items are printed out.
    /// Shows numbers smaller than State.Doc.ModelAbsoluteTolerance * 0.1 as 0.0
    /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
    ///   - thousandSeparator          = '\'' (this is just one quote: ')  ; set this to change the printing of floats and integers larger than 10'000
    ///   - toNiceStringMaxDepth       = 3                                 ; set this to change how deep the content of nested seq is printed (printFull ignores this)
    ///   - toNiceStringMaxItemsPerSeq = 5                                 ; set this to change how how many items per seq are printed (printFull ignores this)
    ///   - maxCharsInString           = 5000                              ; set this to change how many characters of a string might be printed at once.  </summary>
    ///<param name="x">('T) the value or object to represent as string</param>
    ///<param name="trim">(bool) Optional, Default Value: <c>true</c>
    /// Applicable if the value x is a Seq: If true  the string will only show the first 5 items per seq or nested seq. If false all itemes will be in the string</param>
    ///<returns>(string) The string.</returns>
    [<Extension>]
    static member ToNiceString (x:'T, [<OPT;DEF(true)>]trim:bool) : string = 
        if Print.doInit then Print.init()
        if trim then NiceString.toNiceString(x)
        else         NiceString.toNiceStringFull(x)       
    
    ///<summary>
    /// Nice formating for numbers including thousand Separator and (nested) sequences, first five items are printed out.
    /// Prints to Console.Out and to Rhino Commandline
    /// Shows numbers smaller than State.Doc.ModelAbsoluteTolerance * 0.1 as 0.0
    /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
    ///   - thousandSeparator          = '\'' (this is just one quote: ')  ; set this to change the printing of floats and integers larger than 10'000
    ///   - toNiceStringMaxDepth       = 3                                 ; set this to change how deep the content of nested seq is printed (printFull ignores this)
    ///   - toNiceStringMaxItemsPerSeq = 5                                 ; set this to change how how many items per seq are printed (printFull ignores this)
    ///   - maxCharsInString           = 5000                              ; set this to change how many characters of a string might be printed at once.  </summary>
    ///<param name="x">('T) the value or object to print</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member Print (x:'T) : unit =
        RhinoScriptSyntax.ToNiceString(x, true)
        |>! RhinoApp.WriteLine 
        |>  Console.WriteLine  
        RhinoApp.Wait() // no swith to UI Thread needed !
 
    ///<summary> 
    /// Nice formating for numbers including thousand Separator, all items of sequences, including nested items, are printed out.
    /// Prints to Console.Out and to Rhino Commandline
    /// Shows numbers smaller than State.Doc.ModelAbsoluteTolerance * 0.1 as 0.0
    /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
    ///   - thousandSeparator          = '\'' (this is just one quote: ')  ; set this to change the printing of floats and integers larger than 10'000
    ///   - maxCharsInString           = 5000                              ; set this to change how many characters of a string might be printed at once.</summary>
    ///<param name="x">('T) the value or object to print</param>   
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member PrintFull (x:'T) : unit =
        RhinoScriptSyntax.ToNiceString(x, false)
        |>! RhinoApp.WriteLine 
        |>  Console.WriteLine  
        RhinoApp.Wait() // no swith to UI Thread needed !
    
    (*
    //<summary>
    // Prints Sequence of objects or values separated by a space charcter or a custom value.
    // Prints to Console.Out and to Rhino Commandline.</summary>
    //<param name="xs">('T) the values or objects to print</param>
    //<param name="separator">(string) Optional, Default Value: a space character <c>" "</c></param>
    //<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member PrintSeq (xs:'T seq, [<OPT;DEF(" ")>]separator:string) : unit =
        xs
        |>  Seq.map RhinoScriptSyntax.ToNiceString
        |>  String.concat separator
        |>! RhinoApp.WriteLine 
        |>  Console.WriteLine
        RhinoApp.Wait()
    *)

    
    /// Like printf but in Red if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfRed msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            PrintColor.fColor 220 0 0 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
            
    /// Like printfn but in Red if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnRed msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            PrintColor.fnColor 220 0 0 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like PrintColor.f but in Green if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfGreen msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            PrintColor.fColor 0 180 0 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
            
    /// Like printfn but in Green if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnGreen msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            PrintColor.fnColor 0 180 0 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Light Blue if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfLightBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            PrintColor.fColor 173 216 230  "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printfn but in Light Blue if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnLightBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            PrintColor.fnColor 173 216 230  "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Blue if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            PrintColor.fColor 0 0 220 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printfn but in Blue if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            PrintColor.fnColor 0 0 220 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Grey if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfGrey msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            PrintColor.fColor 150 150 150 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printfn but in Grey if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnGrey msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            PrintColor.fnColor 150 150 150 "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
    
    ///<summary>Like printf but in custom color if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.</summary>
    ///<param name="red">(int) Red Value between 0 and 255 </param>
    ///<param name="green">(int) Green value between 0 and 255 </param>
    ///<param name="blue">(int) Blue value between 0 and 255 </param>
    ///<param name="msg">The format string</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member PrintfColor (red:int) (green:int) (blue:int) msg  =
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            PrintColor.fColor red green blue"%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
    
    ///<summary>Like printfn but in costom color if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.</summary>
    ///<param name="red">(int) Red Value between 0 and 255 </param>
    ///<param name="green">(int) Green value between 0 and 255 </param>
    ///<param name="blue">(int) Blue value between 0 and 255 </param>
    ///<param name="msg">The format string</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member PrintfnColor (red:int) (green:int) (blue:int) msg  =
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            PrintColor.fnColor red green blue"%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
  
  
  //---------------------------------------------------------------------------
  // Shadowing the built in print and printFull to inculde external formater
  //---------------------------------------------------------------------------


  /// Nice formating for numbers including thousand Separator and (nested) sequences, first five items are printed out.
  /// Only prints to Console.Out, NOT to Rhino Commandline
  /// Shows numbers smaller than State.Doc.ModelAbsoluteTolerance * 0.1 as 0.0
  /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
  /// - thousandSeparator          = '\'' (this is just one quote: ')  ; set this to change the printing of floats and integers larger than 10'000
  /// - toNiceStringMaxDepth       = 3                                 ; set this to change how deep the content of nested seq is printed (printFull ignores this)
  /// - toNiceStringMaxItemsPerSeq = 5                                 ; set this to change how how many items per seq are printed (printFull ignores this)
  /// - maxCharsInString           = 5000                              ; set this to change how many characters of a string might be printed at once.  
  let print x = 
    if Print.doInit then Print.init() 
    print x
  
  /// Nice formating for numbers including thousand Separator, all items of sequences, including nested items, are printed out.
  /// Only prints to Console.Out, NOT to Rhino Commandline
  /// Shows numbers smaller than State.Doc.ModelAbsoluteTolerance * 0.1 as 0.0
  /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
  /// - thousandSeparator          = '\'' (this is just one quote: ')  ; set this to change the printing of floats and integers larger than 10'000
  /// - maxCharsInString           = 5000                              ; set this to change how many characters of a string might be printed at once.
  let printFull x = 
    if Print.doInit then Print.init() 
    printFull x  

    (*
  /// Prints two values separated by a space using FsEx.NiceString.toNiceString
  /// Only prints to Console.Out, NOT to Rhino Commandline
  let print2 x y = 
    if Print.doInit then Print.init()
    print2 x y
  
  /// Prints three values separated by a space using FsEx.NiceString.toNiceString
  /// Only prints to Console.Out, NOT to Rhino Commandline
  let print3 x y z = 
    if Print.doInit then Print.init()
    print3 x y z
  
  /// Prints four values separated by a space using FsEx.NiceString.toNiceString
  /// Only prints to Console.Out, NOT to Rhino Commandline
  let print4 w x y z = 
    if Print.doInit then Print.init() 
    print4 w x y z
    *)


