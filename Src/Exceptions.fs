namespace Rhino.Scripting

open System


// to expose CLI-standard extension members that can be consumed from C# or VB,
// http://www.latkin.org/blog/2014/04/30/f-extension-methods-in-roslyn/
// declare all Extension attributes explicitly
[<assembly:Runtime.CompilerServices.Extension>] do () 


// ------- Abreviations so that declarations are not so long:

///OptionalAttribute for member parameters
type internal OPT = Runtime.InteropServices.OptionalAttribute

/// DefaultParameterValueAttribute for member parameters
type internal DEF = Runtime.InteropServices.DefaultParameterValueAttribute


// -------------Custom exceptions used across this library including printf formating:

/// Exception for Errors in script execution
type RhinoScriptingException (s:string) =
    inherit System.Exception(s)

    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new RhinoScriptingException(s))) msg 

    static member inline FailIfFalse s b = 
        if not b then raise (new RhinoScriptingException(s))


 /// Exception for aborted user interactions, such as cancelling to pick an object
type UserInteractionException (s:string) =
    inherit System.Exception(s)
    
    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new UserInteractionException(s))) msg 

 /// Exception for UI thread synchronisation problems
type SyncRhinoException (s:string) =
    inherit System.Exception(s)
    
    static member inline Raise msg = 
        Printf.kprintf (fun s -> raise (new SyncRhinoException(s))) msg 
