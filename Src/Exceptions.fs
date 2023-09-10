namespace Rhino.Scripting 

open Rhino 

open System

// -------------Custom exceptions used across this library including printf formatting:


/// Rhino.Scripting Exception for Errors in script execution
type RhinoScriptingException (s:string) = 
    inherit Exception(s)

    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new RhinoScriptingException(s))) msg

    static member inline FailIfFalse s b = 
        if not b then raise (new RhinoScriptingException(s))


/// Rhino.Scripting Exception for aborted user interactions, such as canceling to pick an object
type RhinoUserInteractionException (s:string) = 
    inherit Exception(s)

    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new RhinoUserInteractionException(s))) msg

/// Rhino.Scripting Exception for UI thread synchronization problems
type RhinoSyncException (s:string) = 
    inherit Exception(s)

    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new RhinoSyncException(s))) msg
