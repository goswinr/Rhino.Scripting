﻿namespace Rhino

open System
open Rhino.Geometry
open FsEx

/// Internal module to set up nice printing.(public only for access from Rhino.Scripting.Extension)
[<RequireQualifiedAccess>]
module PrintSetup = 

    let mutable initIsPending = true // to delay setup of printing till first print call

    /// Gets a description on Rhino object type (curve , point, Surface ....).
    /// Including Layer and object name
    let guid (g:Guid)= 
        if g = Guid.Empty then
            "-Guid.Empty-"
        elif Runtime.HostUtils.RunningInRhino  then // because Rhino.Scripting might be referenced from outside of Rhino too
            let o = State.Doc.Objects.FindId(g)
            if isNull o then sprintf "Guid %O (not in State.Doc.Objects table of this Rhino file)." g
            else
                let name = o.Attributes.Name // null if unset
                if String.IsNullOrWhiteSpace name then
                    sprintf "Guid %O (a %s%s%s on Layer '%s')" g  (if o.IsDeleted then "deleted " else "") (if o.IsHidden then "hidden " else "") (o.ShortDescription(false)) (State.Doc.Layers.[o.Attributes.LayerIndex].FullPath)
                else
                    sprintf "Guid %O (a %s%s%s named '%s' on Layer '%s')" g (if o.IsDeleted then "deleted " else "") (if o.IsHidden then "hidden " else "") (o.ShortDescription(false)) name (State.Doc.Layers.[o.Attributes.LayerIndex].FullPath)
        else
            // the toNiceString function gets injected into FsEx.NiceString, below
            // so that using the print function still works on other Guids if Rhino.Scripting is referenced from outside of rhino where there is no active Doc
            sprintf "Guid %O" g

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
        initIsPending <- false
        try
            NiceStringSettings.externalFormatter  <- formatRhinoObject
            if Rhino.Runtime.HostUtils.RunningInRhino then
                // these below fail if not running inside rhino.exe
                // scripts that reference Rhino.Scripting from outside of rhino is still Ok , but all function that call the C++ API don't work
                // but accessing the current Doc obviously does not work.
                NiceStringSettings.userZeroTolerance                                              <- State.Doc.ModelAbsoluteTolerance  * 0.1 // so that any float smaller than State.Doc.ModelAbsoluteTolerance wil be shown as 0.0
                RhinoApp.AppSettingsChanged.Add    (fun _ -> NiceStringSettings.userZeroTolerance <- State.Doc.ModelAbsoluteTolerance  * 0.1 )
                RhinoDoc.ActiveDocumentChanged.Add (fun a -> NiceStringSettings.userZeroTolerance <- a.Document.ModelAbsoluteTolerance * 0.1 )
                RhinoDoc.EndOpenDocument.Add       (fun a -> NiceStringSettings.userZeroTolerance <- a.Document.ModelAbsoluteTolerance * 0.1 )
        with e -> 
            // try to log errors to error stream:
            eprintfn "Initializing NiceString pretty printing from Rhino.Scripting in FsEx failed with:\r\n%A" e
            
    
    /// Like printfn but in Blue if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    let printfnBlue msg = 
        if initIsPending then init()
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.blue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printfn but in Red if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    let printfnRed msg = 
        if initIsPending then init()
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.blue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !


/// This module shadows the NiceString module from FsEx to include the special formatting for Rhino types.
[<AutoOpen>]
module NiceString  = 

    /// Nice formatting for Rhino and .Net types, e.g. numbers including thousand Separator and (nested) sequences, first five items are printed out.
    /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
    /// - thousandSeparator       = '     ; set this to change the printing of floats and integers larger than 10'000
    /// - maxNestingDepth         = 3     ; set this to change how deep the content of nested seq is printed (printFull ignores this)
    /// - maxNestingDepth         = 6     ; set this to change how how many items per seq are printed (printFull ignores this)
    /// - maxCharsInString        = 2000  ; set this to change how many characters of a string might be printed at once.
    let toNiceString (x:'T) :string = 
        if PrintSetup.initIsPending then PrintSetup.init() // the shadowing is only done to ensure init() is called once
        NiceString.toNiceString x

    /// Nice formatting for Rhino and .Net types, e.g. numbers including thousand Separator,
    /// all items of sequences, including nested items, are printed out.
    /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
    /// - thousandSeparator       = '      ; set this to change the printing of floats and integers larger than 10'000
    /// - maxCharsInString        = 2000   ; set this to change how many characters of a string might be printed at once.
    let toNiceStringFull (x:'T) :string = 
        if PrintSetup.initIsPending then PrintSetup.init() // the shadowing is only done to ensure init() is called once
        toNiceString x


/// Shadowing the print and printFull from FsEx to include external formatter for Rhino
[<AutoOpen>]
module AutoOpenPrint = 

    /// Print to standard out including nice formatting for Rhino Objects, numbers including thousand Separator and (nested) sequences, first five items are printed out.
    /// Only prints to Console.Out, NOT to Rhino Commandline
    /// Shows numbers smaller than State.Doc.ModelAbsoluteTolerance * 0.1 as ~0.0
    /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
    /// - thousandSeparator       = '     ; set this to change the printing of floats and integers larger than 10'000
    /// - maxNestingDepth         = 3     ; set this to change how deep the content of nested seq is printed (printFull ignores this)
    /// - maxNestingDepth         = 6     ; set this to change how how many items per seq are printed (printFull ignores this)
    /// - maxCharsInString        = 2000  ; set this to change how many characters of a string might be printed at once.
    let print x = 
        if PrintSetup.initIsPending then PrintSetup.init()
        print x

    /// Print to standard out including nice formatting for Rhino Objects, numbers including thousand Separator, all items of sequences, including nested items, are printed out.
    /// Only prints to Console.Out, NOT to Rhino Commandline
    /// Shows numbers smaller than State.Doc.ModelAbsoluteTolerance * 0.1 as ~0.0
    /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
    /// - thousandSeparator       = '      ; set this to change the printing of floats and integers larger than 10'000
    /// - maxCharsInString        = 2000   ; set this to change how many characters of a string might be printed at once.
    let printFull x = 
        if PrintSetup.initIsPending then PrintSetup.init()
        printFull x









