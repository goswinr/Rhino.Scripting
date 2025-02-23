![logo](https://raw.githubusercontent.com/goswinr/Rhino.Scripting/main/Docs/img/logo.png)
# Rhino.Scripting

[![Rhino.Scripting on nuget.org](https://img.shields.io/nuget/v/Rhino.Scripting)](https://www.nuget.org/packages/Rhino.Scripting/)
[![Build Status](https://github.com/goswinr/Rhino.Scripting/actions/workflows/build.yml/badge.svg)](https://github.com/goswinr/Rhino.Scripting/actions/workflows/build.yml)
[![Docs Build Status](https://github.com/goswinr/Rhino.Scripting/actions/workflows/docs.yml/badge.svg)](https://github.com/goswinr/Rhino.Scripting/actions/workflows/docs.yml)
[![Check dotnet tools](https://github.com/goswinr/Rhino.Scripting/actions/workflows/outdatedDotnetTool.yml/badge.svg)](https://github.com/goswinr/Rhino.Scripting/actions/workflows/outdatedDotnetTool.yml)
[![license](https://img.shields.io/github/license/goswinr/Rhino.Scripting)](LICENSE.md)
![code size](https://img.shields.io/github/languages/code-size/goswinr/Rhino.Scripting.svg)

Rhino.Scripting is a complete re-implementation of the original **RhinoScript syntax** in and for F# (and C#).\
Before this repo, the high-level RhinoScript API was only available for VBScript and (Iron-)Python.\
This repo enables the use of the RhinoScriptSyntax in F# and C#\
together with all the great coding experience and editor tooling that come with F# and C#, like:
- automatic code completion while typing.
- automatic error checking and highlighting in the background.
- type info on mouse over.
- type safety even without type annotation (= type inference in F#).

## What is RhinoScript?

RhinoScript provides application scripting for the [Rhino3D](https://www.rhino3d.com/) CAD app.\
RhinoScript has [more than 900 functions](https://developer.rhino3d.com/api/RhinoScriptSyntax/) to control all kinds of aspects of automating Rhino3D.\
It was originally implemented in 2002 in VBScript.\
Extensive documentation on the original VBScript-based version is available [here](https://developer.rhino3d.com/guides/rhinoscript/).

In 2010, all functions from [RhinoScript were reimplemented in IronPython](https://developer.rhino3d.com/guides/#rhinopython) (Python running on .NET).\
This allowed the use of a modern, rich, and dynamically typed programming language with a huge standard library and \
also access to all functions of the underlying .NET Framework as well as the [RhinoCommon SDK](https://developer.rhino3d.com/guides/rhinocommon/).

## What is this repo?

This repo has [all original  RhinoScript functions](https://developer.rhino3d.com/api/RhinoScriptSyntax/) reimplemented in [F#](https://fsharp.org/).\
It is literally a translation of [the open-source IronPython rhinoscriptsyntax](https://github.com/mcneel/rhinoscriptsyntax) implementation to F#.\
[You can see all 900+ methods in this repo in the docs](https://goswinr.github.io/Rhino.Scripting/reference/rhino-scripting-rhinoscriptsyntax.html).

A few minor bugs from the Python implementation are fixed and a few extra methods and optional parameters were added.\
I have been using this library extensively for my own professional scripting needs since 2019.\
If you have problems, questions, or find a bug, please open an [issue](https://github.com/goswinr/Rhino.Scripting/issues).

## Get started

The recommended scripting use case is via [Fesh](https://github.com/goswinr/Fesh.Rhino), the dedicated F# scripting editor for Rhino.\
However, you can use this library just as well in compiled F#, C#, or VB.net projects.

### Get started in F#

First reference the assemblies.
```fsharp
#r "nuget: Rhino.Scripting, 0.8.2"
```

The main namespace is  `Rhino.Scripting`.\
The main class of this library is called `RhinoScriptSyntax` it has all ~900 functions as static methods.\
In F# you can create an alias like this:

```fsharp
open Rhino.Scripting
type rs = RhinoScriptSyntax
```

then use any of the RhinoScript functions like you would in Python or VBScript.\
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
#r "nuget: FSharp.Core, 6.0.7"// dependency of Rhino.Scripting
#r "nuget: FsEx, 0.16.0" // dependency of Rhino.Scripting
#r "nuget: Rhino.Scripting, 0.8.2"
```
The main namespace is  `Rhino.Scripting`.\
The main class of this library is called `RhinoScriptSyntax` it has all ~900 functions as static methods.\
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
Many RhinoScript function take variable types of input parameters.\
This is implemented with method overloads.\
Many RhinoScript function have optional parameters.\
These are also implemented as optional method parameters.


### Example
for example `rs.ObjectLayer` can be called in several ways:

`rs.ObjectLayer(guid)` To get the layer of one object, returns a string.\
`rs.ObjectLayer(guid, string)` To set the layer of one object (fails if layer does not exist), no return value.\
`rs.ObjectLayer(guid, string, createLayerIfMissing = true )` To set the layer of one object, and create the layer if it does not exist yet, no return value.\
`rs.ObjectLayer(list of guids, string)` To set the layer of several objects (fails if layer does not exist), no return value.\
`rs.ObjectLayer(list of guids, string, createLayerIfMissing = true )` To set the layer of several objects, and create the layer if it does not exist yet, no return value.\

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
                UtilLayer.getOrCreateLayer(layer, UtilLayer.randomLayerColor,
                    UtilLayer.ByParent, UtilLayer.ByParent,
                    allowAllUnicode,collapseParents).Index
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
                UtilLayer.getOrCreateLayer(layer,
                    UtilLayer.randomLayerColor, UtilLayer.ByParent,
                    UtilLayer.ByParent, allowUnicode, collapseParents).Index
            else
                RhinoScriptSyntax.CoerceLayer(layer).Index
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            obj.Attributes.LayerIndex <- layerIndex
            obj.CommitChanges() |> ignore
        State.Doc.Views.Redraw()
```


### Full API Documentation

[goswinr.github.io/Rhino.Scripting](https://goswinr.github.io/Rhino.Scripting)

## Thread Safety
While the main Rhino Document is officially not thread safe, this library can be used from any thread.\
If running async this library will automatically marshal all calls that affect the UI to the main Rhino UI thread \
and wait for switching back till completion on UI thread.\
Modifying the Rhino Document from a background thread is actually OK as long as there is only one thread doing it.\
The main reason to use this library async is to keep the Rhino UI and Fesh scripting editor UI responsive while doing long running operations.

## Contributing
Contributions are welcome even for small things like typos. If you have problems with this library please submit an issue.

### License
[MIT](https://github.com/goswinr/Rhino.Scripting/blob/main/LICENSE.md)

### Changelog
see [CHANGELOG.md](https://github.com/goswinr/Rhino.Scripting/blob/main/CHANGELOG.md)