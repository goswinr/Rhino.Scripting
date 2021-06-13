namespace Rhino.Scripting

open System
open System.Runtime.CompilerServices

open Rhino

open FsEx
open FsEx.SaveIgnore


/// This module is automatically opened when Rhino.Scripting namespace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
[<AutoOpen>]
module ExtensionsPrinting =
  
  type RhinoScriptSyntax with    
       
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
        if Print.doInit then Print.init()
        NiceString.toNiceString(x)
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
        if Print.doInit then Print.init()
        NiceString.toNiceStringFull(x) 
        |>! RhinoApp.WriteLine 
        |>  Console.WriteLine  
        RhinoApp.Wait() // no swith to UI Thread needed !
    
    
    /// Like printf but in Red if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfRed msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Printf.red "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
            
    /// Like printfn but in Red if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnRed msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Printfn.red "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like PrintColor.f but in Green if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfGreen msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Printf.green "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
            
    /// Like printfn but in Green if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnGreen msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Printfn.green "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Light Blue if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfLightBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Printf.lightBlue "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printfn but in Light Blue if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnLightBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Printfn.lightBlue "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Blue if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Printf.blue "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printfn but in Blue if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnBlue msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Printfn.blue "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printf but in Gray if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfGray msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.Write s
            Printf.gray "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !

    /// Like printfn but in Gray if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    [<Extension>]
    static member PrintfnGray msg =  
        Printf.kprintf (fun s -> 
            RhinoApp.WriteLine s
            Printfn.gray "%s" s
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
            Printf.color red green blue "%s" s
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
            Printfn.color red green blue "%s" s
            RhinoApp.Wait())  msg // no swith to UI Thread needed !
  
  
