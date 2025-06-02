namespace Rhino.Scripting

open Rhino

open System

// -------------Custom exceptions used across this library including printf formatting:

module internal VersionInfo =

    type Dummy = class end

    let versionInfo =
        let v = typeof<Dummy>.Assembly.GetName().Version
        lazy
            $"{Environment.NewLine}Rhino:{RhinoApp.Version}{Environment.NewLine}Rhino.Scripting:{v}{Environment.NewLine}"


open VersionInfo

/// Rhino.Scripting Exception for Errors in script execution
type RhinoScriptingException (s:string) =
    inherit Exception(s)
    static member Raise msg =
        Printf.kprintf (fun s -> raise (new RhinoScriptingException(s+versionInfo.Value))) msg

    static member FailIfFalse s b =
        if not b then raise (new RhinoScriptingException(s+versionInfo.Value))


/// Rhino.Scripting Exception for aborted user interactions, such as canceling to pick an object
type RhinoUserInteractionException (s:string) =
    inherit Exception(s)

    static member Raise msg =
        Printf.kprintf (fun s -> raise (new RhinoUserInteractionException(s+versionInfo.Value
        ))) msg

// if syncContext is null, then Eto.Forms.Application.Instance.Invoke is used.
// Rhino.Scripting Exception for UI thread synchronization problems
// type RhinoSyncException (s:string) =
//     inherit Exception(s)
//     static member inline Raise msg =
//         Printf.kprintf (fun s -> raise (new RhinoSyncException(s))) msg
//