namespace Rhino.Scripting

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
type UserInteractionException (s:string) =
    inherit Exception(s)
    
    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new UserInteractionException(s))) msg 

 /// Exception for UI thread synchronisation problems
type SyncRhinoException (s:string) =
    inherit Exception(s)
    
    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new SyncRhinoException(s))) msg 
