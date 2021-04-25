namespace Rhino.Scripting


type RhinoScriptingException (s:string) =
    inherit System.Exception(s)
    static member inline Raise msg = Printf.kprintf (fun s -> raise (new RhinoScriptingException(s))) msg 
    static member inline FailIfFalse s b = if not b then raise (new RhinoScriptingException(s))

type UserInteractionException (s:string) =
    inherit System.Exception(s)
    static member inline Raise msg = Printf.kprintf (fun s -> raise (new UserInteractionException(s))) msg 

