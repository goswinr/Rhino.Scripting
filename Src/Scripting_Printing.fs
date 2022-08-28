
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenPrinting =
  type Scripting with  
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't change: {@$%^&*()*&^%$@}


    ///<summary>
    /// Nice formatting for numbers including thousand Separator and (nested) sequences, first five items are printed out.
    /// Prints to Console.Out and to Rhino Commandline
    /// Shows numbers smaller than State.Doc.ModelAbsoluteTolerance * 0.1 as 0.0
    /// Settings are exposed in FsEx.NiceString.NiceStringSettings.defaultNicePrintSettings:
    /// maxDepth          = 3     : how deep the content of nested seq is printed 
    /// maxVertItems      = 6     : amount of lines printed.
    /// maxHorChars       = 120   : maximum amount of characters per line.
    /// maxCharsInString  = 2000  : after this the characters of a string are trimmed off.
    /// The function rs.PrintFull does not do this trimming.
    /// </summary>
    ///<param name="x">('T) the value or object to print</param>
    ///<returns>(unit) void, nothing.</returns>
    static member Print (x:'T) : unit = 
        if PrintSetup.initIsPending then PrintSetup.init()
        toNiceString(x)
        |>! RhinoApp.WriteLine
        |>  Console.WriteLine
        RhinoApp.Wait() // no switch to UI Thread needed !

    ///<summary>
    /// Nice formatting for numbers including thousand Separator, all items of sequences, including nested items, are printed out.
    /// Prints to Console.Out and to Rhino Commandline
    /// Shows numbers smaller than State.Doc.ModelAbsoluteTolerance * 0.1 as 0.0</summary>
    ///<param name="x">('T) the value or object to print</param>
    ///<returns>(unit) void, nothing.</returns>
    static member PrintFull (x:'T) : unit = 
        if PrintSetup.initIsPending then PrintSetup.init()
        toNiceStringFull(x)
        |>! RhinoApp.WriteLine
        |>  Console.WriteLine
        RhinoApp.Wait() // no switch to UI Thread needed !


    /// Like printf but in Red if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfRed msg = 
        Printf.kprintf (fun s ->
            RhinoApp.Write s
            Printf.red "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printfn but in Red if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfnRed msg = 
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.red "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like PrintColor.f but in Green if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfGreen msg = 
        Printf.kprintf (fun s ->
            RhinoApp.Write s
            Printf.green "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printfn but in Green if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfnGreen msg = 
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.green "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printf but in Light Blue if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfLightBlue msg = 
        Printf.kprintf (fun s ->
            RhinoApp.Write s
            Printf.lightBlue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printfn but in Light Blue if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfnLightBlue msg = 
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.lightBlue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printf but in Blue if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfBlue msg = 
        Printf.kprintf (fun s ->
            RhinoApp.Write s
            Printf.blue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printfn but in Blue if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfnBlue msg = 
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.blue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printf but in Gray if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfGray msg = 
        Printf.kprintf (fun s ->
            RhinoApp.Write s
            Printf.gray "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printfn but in Gray if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    static member PrintfnGray msg = 
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.gray "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    ///<summary>Like printf but in custom color if used from Seff Editor. Does not add a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.</summary>
    ///<param name="red">(int) Red Value between 0 and 255 </param>
    ///<param name="green">(int) Green value between 0 and 255 </param>
    ///<param name="blue">(int) Blue value between 0 and 255 </param>
    ///<param name="msg">The format string</param>
    ///<returns>(unit) void, nothing.</returns>
    static member PrintfColor (red:int) (green:int) (blue:int) msg  = 
        Printf.kprintf (fun s ->
            RhinoApp.Write s
            Printf.color red green blue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    ///<summary>Like printfn but in custom color if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.</summary>
    ///<param name="red">(int) Red Value between 0 and 255 </param>
    ///<param name="green">(int) Green value between 0 and 255 </param>
    ///<param name="blue">(int) Blue value between 0 and 255 </param>
    ///<param name="msg">The format string</param>
    ///<returns>(unit) void, nothing.</returns>
    static member PrintfnColor (red:int) (green:int) (blue:int) msg  = 
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.color red green blue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !



