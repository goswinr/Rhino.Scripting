---
name: rhino.scripting-usage
description: >
  Write and fix Rhino 3D scripts with Rhino.Scripting, the F# and C# version of the RhinoScript functions (RhinoScriptSyntax, about 900 functions).
  Use for scripts that add, select, change or query objects, layers, views, blocks, materials or user text in Rhino, and to look up exact names,
  overloads and optional parameters in ApiTable.md next to this file.
---

# Rhino.Scripting

Rhino.Scripting has the functions of the RhinoScript syntax as static methods of one class, `RhinoScriptSyntax`. Scripts call them through a short alias, `rs`. The functions act on the active Rhino document, so they only work inside a running Rhino process.

## Setup

F# script (.fsx), the default template of the Fesh editor in Rhino:

```fsharp
#r "nuget: Rhino.Scripting.FSharp"

open System
open Rhino.Geometry
open Rhino.Scripting
open Rhino.Scripting.FSharp // recommended for F#

type rs = RhinoScriptSyntax

let crv = rs.GetObject("Select a curve", rs.Filter.Curve)
let length = rs.CurveLength(crv)
printfn "Curve length: %f" length
```

C#, in the Rhino 8 ScriptEditor:

```csharp
using rs = Rhino.Scripting.RhinoScriptSyntax;

var id = rs.GetObject("Select an object");
rs.ObjectColor(id, System.Drawing.Color.Blue);
```

Key properties of `rs`:

- `rs.Doc`: the active Rhino document.
- `rs.Ot`: the object table of the document, a shortcut for `Doc.Objects`.
- `rs.Sticky`: a dictionary that keeps state between script runs.
- `rs.Filter`: object type flags for selection, e.g. `rs.Filter.Curve`.

## Finding functions

`ApiTable.md` lists every public member of `RhinoScriptSyntax` with its signature and the first sentence of its summary. Check it before you guess a name or a parameter. It is large, so search it instead of reading it whole: the Contents list at the top links to one section per topic, such as Curve, Layer or Document.

- Rows with the same name are overloads. Pick the one whose parameter types match your arguments.
- Some names are both getter and setter. The overload without the value returns the current value; the overload with the value sets it.
- `?name: type = value` is an optional parameter with its default value. Pass it by name, e.g. `createLayerIfMissing = true`.
- Instance members of the other types, such as `Plane` or `Transform`, start with `.` in their sections.

## Coercion

The `Coerce...` functions turn inputs into the types that the functions need, so you can pass plain numbers and strings. Each has one overload per accepted input type, so a wrong type is a compile error.

- Pass coordinates and color values as separate arguments: `rs.CoerceColor(255, 0, 0)`. A tuple value like `rs.CoerceColor(rgb)` does not compile.
- Many of them also have a `TryCoerce...` variant that returns an option and never throws.

## Common patterns

Add an object on a layer, with a name:

```fsharp
let guid = rs.Add(geometry, "LayerName", "ObjectName")

// For many objects on one layer, get the layer index once and reuse it
let layerIdx = rs.AddLayer("MyLayer")
let guid2 = rs.Add(geometry, layerIdx, "ObjectName")
```

Iterate the objects on a layer:

```fsharp
for curveId in rs.ObjectsByLayer("MyCurves") do
    printfn "Curve length: %f" (rs.CurveLength(curveId))
```

Store and read metadata on an object:

```fsharp
rs.SetUserText(objectId, "Material", "Steel")
let material = rs.GetUserText(objectId, "Material")
```

Select with filters:

```fsharp
let curves = rs.GetObjects("Select curves", rs.Filter.Curve)
let points = rs.GetObjects("Select points", rs.Filter.Point)
```

Transform an object:

```fsharp
let xform = rs.XformRotation(45.0, Vector3d.ZAxis, Point3d.Origin)
rs.TransformObject(objectId, xform)
```

## Thread safety

Methods can be called from any thread. Calls that touch the Rhino UI, such as creating layers or picking objects, are marshalled to the Rhino UI thread and wait there. Changing the document from one background thread at a time is fine.

```fsharp
async {
    let! _ = Async.SwitchToThreadPool()
    let pt = rs.AddPoint(0.0, 0.0, 0.0) // marshalled to the UI thread
    rs.Redraw()
}
```

## Error handling

The library raises these exceptions:

- `RhinoScriptingException`: a general scripting error.
- `RhinoUserInteractionException`: the user cancelled, e.g. with Escape.

```fsharp
try
    let obj = rs.GetObject("Select an object")
    // ... work with obj
with
| :? RhinoScriptingException as ex -> printfn "Script error: %s" ex.Message
| :? RhinoUserInteractionException -> printfn "User cancelled"
```

## Escape key

In long loops, check for Escape so the person can stop the script:

```fsharp
for i in 0 .. 10000 do
    rs.EscapeTest() // raises OperationCanceledException if Escape was pressed
    // ... do work
```

## Best practices

1. Turn off redraw for batch work: call `rs.EnableRedraw(false)` before many changes and `rs.EnableRedraw(true)` after.
2. Get a layer index once and reuse it when adding many objects to the same layer.
3. Call `rs.EscapeTest()` in loops so the person can interrupt.
4. Use the `Coerce...` functions for inputs. They accept several formats and give clear error messages.
5. Use `rs.Sticky` to keep state between script runs.

## Through an MCP server

In an agent connected to the Caddis MCP server, `caddis_describe` looks up the same API in the loaded assemblies, e.g. `RhinoScriptSyntax.ObjectLayer`. Use it for one name. This skill is also an MCP resource, but `ApiTable.md` is large, so reading it through the resource returns the whole table.
