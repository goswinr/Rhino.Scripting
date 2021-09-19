﻿namespace Rhino

open System

// -------------Custom exceptions used across this library including printf formating:

/// Exception for Errors in script execution
type RhinoScriptingException (s:string) = 
    inherit Exception(s)

    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new RhinoScriptingException(s))) msg

    static member inline FailIfFalse s b = 
        if not b then raise (new RhinoScriptingException(s))


 /// Exception for aborted user interactions, such as cancelling to pick an object
type RhinoUserInteractionException (s:string) = 
    inherit Exception(s)

    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new RhinoUserInteractionException(s))) msg

 /// Exception for UI thread synchronisation problems
type RhinoSyncException (s:string) = 
    inherit Exception(s)

    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new RhinoSyncException(s))) msg
