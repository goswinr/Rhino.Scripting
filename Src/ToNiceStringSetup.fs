namespace Rhino

open System
open Rhino.Geometry
open FsEx

/// Part of Rhino.Scripting nuget.
/// An internal module to set up nice printing of Rhino Objects. 
/// (It is public only for access from Rhino.ScriptingFsharp project)
[<RequireQualifiedAccess>]
module InternalToNiceStringSetup = 
    open Rhino    
    open Rhino.ScriptingFsharp

    let mutable private initIsPending = true // to delay setup of printing till first print call

    /// Gets a description on Rhino object type (curve , point, Surface ....).
    /// Including Layer and object name
    let internal guid (g:Guid)= 
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
            // the Nice.str function gets injected into FsEx.NiceString, below
            // so that using the print function still works on other Guids if Rhino.Scripting is referenced from outside of rhino where there is no active Doc
            sprintf "Guid %O" g

    /// Rhino specific formatting function that is set to be the 
    /// externalFormatter in FsEx.NiceStringSettings. It is set in the init() function.
    let internal formatRhinoObject (o:obj) : NiceStringSettings.Lines option= 
          match o with
          | :? Guid        as x -> Some <| NiceStringSettings.Element (guid x)
          | :? Point3d     as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Vector3d    as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Line        as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Point3f     as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Vector3f    as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Transform   as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? Plane       as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | :? BoundingBox as x -> Some <| NiceStringSettings.Element x.ToNiceString
          | _                  -> None

    let init()= 
        if initIsPending then 
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
                eprintfn "Initializing NiceString pretty printing in Rhino.InternalToNiceStringSetup.init() via Rhino.Scripting.dll failed with:\r\n%A" e
            
    
    /// Like printfn but in Blue if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    let internal printfnBlue msg = 
        init()
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.blue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !

    /// Like printfn but in Red if used from Seff Editor. Adds a new line at end.
    /// Prints to Console.Out and to Rhino Commandline.
    let internal printfnRed msg = 
        init()
        Printf.kprintf (fun s ->
            RhinoApp.WriteLine s
            Printfn.blue "%s" s
            RhinoApp.Wait())  msg // no switch to UI Thread needed !


[<RequireQualifiedAccess>]
module internal Nice  = 

    /// Nice formatting for Rhino and .Net types, e.g. numbers including thousand Separator and (nested) sequences, first five items are printed out.
    /// Settings are exposed in FsEx.NiceString.NiceStringSettings:
    /// - thousandSeparator       = '     ; set this to change the printing of floats and integers larger than 10'000
    /// - maxNestingDepth         = 3     ; set this to change how deep the content of nested seq is printed (printFull ignores this)
    /// - maxNestingDepth         = 6     ; set this to change how how many items per seq are printed (printFull ignores this)
    /// - maxCharsInString        = 2000  ; set this to change how many characters of a string might be printed at once.
    let str (x:'T) :string = 
        InternalToNiceStringSetup.init() // the shadowing is only done to ensure init() is called once
        NiceString.toNiceString x


