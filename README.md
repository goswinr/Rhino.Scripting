![logo](https://raw.githubusercontent.com/goswinr/Rhino.Scripting/main/Docs/img/logo.png)
# Rhino.Scripting

[![Rhino.Scripting on nuget.org](https://img.shields.io/nuget/v/Rhino.Scripting)](https://www.nuget.org/packages/Rhino.Scripting/)
[![Build Status](https://github.com/goswinr/Rhino.Scripting/actions/workflows/build.yml/badge.svg)](https://github.com/goswinr/Rhino.Scripting/actions/workflows/build.yml)
[![Docs Build Status](https://github.com/goswinr/Rhino.Scripting/actions/workflows/docs.yml/badge.svg)](https://github.com/goswinr/Rhino.Scripting/actions/workflows/docs.yml)
[![Check dotnet tools](https://github.com/goswinr/Rhino.Scripting/actions/workflows/outdatedDotnetTool.yml/badge.svg)](https://github.com/goswinr/Rhino.Scripting/actions/workflows/outdatedDotnetTool.yml)
[![license](https://img.shields.io/github/license/goswinr/Rhino.Scripting)](LICENSE.md)
![code size](https://img.shields.io/github/languages/code-size/goswinr/Rhino.Scripting.svg)

Rhino.Scripting is a complete re-implementation of the original **RhinoScript syntax** in and for F# (and C#).<br>
Before this repo, the high-level RhinoScript API was only available for VBScript and (Iron-)Python.<br>
This repo enables the use of the RhinoScriptSyntax in F# and C#<br>
together with all the great coding experience and editor tooling that come with F# and C#, like:<br>
- automatic code completion while typing.<br>
- automatic error checking and highlighting in the background.<br>
- type info on mouse over.<br>
- type safety even without type annotation (= type inference in F#).<br>

## What is RhinoScript?

RhinoScript provides application scripting for the [Rhino3D](https://www.rhino3d.com/) CAD app.<br>
RhinoScript has [more than 900 functions](https://developer.rhino3d.com/api/RhinoScriptSyntax/) to control all kinds of aspects of automating Rhino3D.<br>
It was originally implemented in 2002 in VBScript.<br>
Extensive documentation on the original VBScript-based version is available [here](https://developer.rhino3d.com/guides/rhinoscript/).

In 2010, all functions from [RhinoScript were reimplemented in IronPython](https://developer.rhino3d.com/guides/#rhinopython) (Python running on .NET).<br>
This allowed the use of a modern, rich, and dynamically typed programming language with a huge standard library and <br>
also access to all functions of the underlying .NET Framework as well as the [RhinoCommon SDK](https://developer.rhino3d.com/guides/rhinocommon/).

## What is this repo?

This repo has [all original  RhinoScript functions](https://developer.rhino3d.com/api/RhinoScriptSyntax/) reimplemented in [F#](https://fsharp.org/).<br>
It is literally a translation of [the open-source IronPython rhinoscriptsyntax](https://github.com/mcneel/rhinoscriptsyntax) implementation to F#.<br>
[You can see all 900+ methods in this repo in the docs](https://goswinr.github.io/Rhino.Scripting/reference/rhino-scripting-rhinoscriptsyntax.html).

A few minor bugs from the Python implementation are fixed and a few extra methods and optional parameters were added.<br>
I have been using this library extensively for my own professional scripting needs since 2019.<br>
If you have problems, questions, or find a bug, please open an [issue](https://github.com/goswinr/Rhino.Scripting/issues).

## Get started

The recommended scripting use case is via [Fesh](https://github.com/goswinr/Fesh.Rhino), the dedicated F# scripting editor for Rhino.<br>
However, you can use this library just as well in the new Rhino 8 [ScriptEditor](https://www.rhino3d.com/features/developer/scripting/#refreshed-editor--new-in-8-) with C# or in independently compiled F#, C#, or VB.net projects.

### Get started in F#

First reference the assemblies.
```fsharp
#r "nuget: Rhino.Scripting"
```

The main namespace is  `Rhino.Scripting`.<br>
The main class of this library is called `RhinoScriptSyntax` it has all ~900 functions as static methods.<br>
In F# you can create an alias like this:

```fsharp
open Rhino.Scripting
type rs = RhinoScriptSyntax
```

then use any of the RhinoScript functions like you would in Python or VBScript.<br>
The `CoerceXXXX` functions will help you create types if you are too lazy to fully specify them.

```fsharp
let pl = rs.CoercePlane(0 , 80 , 0) // makes World XY plane at point
rs.AddText("Hello, Fesh", pl, height = 50.)
```

For F# scripting the [Rhino.Scripting.Fsharp](https://github.com/goswinr/Rhino.Scripting.Fsharp) provides useful extensions and curried functions for piping and partial application.

### Get started in C#
You can use it via the new Rhino 8 [ScriptEditor](https://www.rhino3d.com/features/developer/scripting/#refreshed-editor--new-in-8-).
First reference the assemblies.

```csharp
#r "nuget: Rhino.Scripting"
```
The main namespace is  `Rhino.Scripting`.<br>
The main class of this library is called `RhinoScriptSyntax` it has all ~900 functions as static methods.<br>
In C# you can create an alias like this:

```csharp
using rs = Rhino.Scripting.RhinoScriptSyntax;
```

then you can use it like the RhinoScriptSyntax in Python:
```csharp
var pt =  rs.GetObject("Select an Object");
rs.ObjectColor(pt, System.Drawing.Color.Blue);
```


## How about the dynamic types and optional parameters from VBScript and Python?
Many RhinoScript functions take variable types of input parameters.<br>
This is implemented with method overloads.<br>
Many RhinoScript functions have optional parameters.<br>
These are also implemented as optional method parameters.<br>
Many RhinoScript functions are getters and setters at the same time.<br>
Depending on if an argument is provided or not, the function acts as a getter or setter.<br>
This is also implemented with method overloads.

### Example
For example `rs.ObjectLayer` can be called in several ways:

To get the layer of one object, returns a string:<br>
```fsharp
rs.ObjectLayer(guid)
```
To set the layer of one object (fails if layer does not exist), no return value:<br>
```fsharp
rs.ObjectLayer(guid, string)
```
To set the layer of one object, and create the layer if it does not exist yet, no return value:<br>
```fsharp
rs.ObjectLayer(guid, string, createLayerIfMissing = true )
```
To set the layer of several objects (fails if layer does not exist), no return value:<br>
```fsharp
rs.ObjectLayer(list of guids, string)
```
To set the layer of several objects, and create the layer if it does not exist yet, no return value:<br>
```fsharp
rs.ObjectLayer(list of guids, string, createLayerIfMissing = true )
```

These are implemented with 3 overloads and  `Optional` and `DefaultParameterValue` parameters:
```fsharp
    ///<summary>Returns the full layer name of an object.
    /// parent layers are separated by <c>::</c>.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<returns>(string) The object's current layer.</returns>
    static member ObjectLayer(objectId:Guid) : string = //GET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = obj.Attributes.LayerIndex
        State.Doc.Layers.[index].FullPath

    ///<summary>Modifies the layer of an object ,
    ///     optionally creates layer if it does not exist yet.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="createLayerIfMissing">(bool) Optional,
    ///     Default Value: <c>false</c> Set true to create Layer
    ///     if it does not exist yet.</param>
    ///<param name="allowAllUnicode">(bool) Optional,
    ///     Allow Ambiguous Unicode characters too </param>
    ///<param name="collapseParents">(bool) Optional,
    ///     Collapse parent layers in Layer UI </param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectLayer( objectId:Guid
                             , layer:string
                             ,[<OPT;DEF(false)>]createLayerIfMissing:bool
                             ,[<OPT;DEF(false:bool)>]allowAllUnicode:bool
                             ,[<OPT;DEF(false:bool)>]collapseParents:bool) : unit = //SET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let layerIndex =
            if createLayerIfMissing then
                UtilLayer.getOrCreateLayer(
                    layer,
                    UtilLayer.randomLayerColor,
                    UtilLayer.ByParent,
                    UtilLayer.ByParent,
                    allowAllUnicode,
                    collapseParents
                    ).Index
            else
                RhinoScriptSyntax.CoerceLayer(layer).Index
        obj.Attributes.LayerIndex <- layerIndex
        obj.CommitChanges() |> ignore
        State.Doc.Views.Redraw()

    ///<summary>Modifies the layer of multiple objects, optionally creates
    ///     layer if it does not exist yet.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of the objects</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="createLayerIfMissing">(bool) Optional,
    ///     Default Value: <c>false</c> Set true to create Layer
    ///     if it does not exist yet.</param>
    ///<param name="allowUnicode">(bool) Optional,
    ///     Allow Ambiguous Unicode characters too </param>
    ///<param name="collapseParents">(bool) Optional,
    ///     Collapse parent layers in Layer UI </param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectLayer( objectIds:Guid seq
                             , layer:string
                             , [<OPT;DEF(false)>]createLayerIfMissing:bool
                             , [<OPT;DEF(false:bool)>]allowUnicode:bool
                             , [<OPT;DEF(false:bool)>]collapseParents:bool) : unit = //MULTISET
        let layerIndex =
            if createLayerIfMissing then
                UtilLayer.getOrCreateLayer(
                    layer,
                    UtilLayer.randomLayerColor,
                    UtilLayer.ByParent,
                    UtilLayer.ByParent,
                    allowUnicode,
                    collapseParents
                    ).Index
            else
                RhinoScriptSyntax.CoerceLayer(layer).Index
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            obj.Attributes.LayerIndex <- layerIndex
            obj.CommitChanges() |> ignore
        State.Doc.Views.Redraw()
```


## Full API Documentation

[goswinr.github.io/Rhino.Scripting](https://goswinr.github.io/Rhino.Scripting)

## .NET Framework or .NET Core?
On Rhino 8.19 or higher, you can use .NET 7.0 and .NET Framework 4.8.<br>
For Rhino 7 and lower versions of Rhino 8, only .NET Framework 4.8 is recommended.<br>

## Windows or Mac?
This library should work on Rhino for Mac just as well as on Windows.<br>


## Build from source
To build this library from source you need the .NET SDK 7 or higher installed<br>
Then just run in the root folder:
```bash
dotnet build ForPublishing.fsproj
```

This will first combine all `Scripting_*.fs` files into one file and compile it.<br>
This is needed because F# type extensions are not visible from C# editor tooling.

## Edit the source
While having all 900 methods on one class in one file is needed for publishing via `ForPublishing.fsproj` <br>
it is not ideal for editing.<br>
The source is split into several files imitating the structure of the original Python implementation.<br>
Open the project `ForEditing.fsproj` to edit the source.<br>


## Changelog

see [CHANGELOG.md](https://github.com/goswinr/Rhino.Scripting/blob/main/CHANGELOG.md)

## Thread Safety
While the main Rhino Document is officially not thread safe, this library can be used from any thread.<br>
If running async this library will automatically marshal all calls that affect the UI to the main Rhino UI thread <br>
and wait for switching back till completion on UI thread.<br>
Modifying the Rhino Document from a background thread is actually OK as long as there is only one thread doing it.<br>
The main reason to use this library async is to keep the Rhino UI and Fesh scripting editor UI responsive while doing long running operations.

## Contributing
Contributions are welcome even for small things like typos. If you have problems with this library please submit an issue.

## License
[MIT](https://github.com/goswinr/Rhino.Scripting/blob/main/LICENSE.md)
