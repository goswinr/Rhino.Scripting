# Rhino.Scripting

![code size](https://img.shields.io/github/languages/code-size/goswinr/Rhino.Scripting.svg)
[![license](https://img.shields.io/github/license/goswinr/Rhino.Scripting)](LICENSE)


![logo](https://raw.githubusercontent.com/goswinr/Rhino.Scripting/main/Doc/logo400.png)


Rhino.Scripting is an implementation of the **RhinoScript** syntax in and for F# (and C#).
Before this repo the high level Rhino-scripting API was only available for VBScript and Python.
This repo enables the use of the RhinoScriptSyntax in F# and C#
together with all the great coding experience and editor tooling that come with F# and C#, like:
- automatic code completion while typing.
- automatic error checking and highlighting in the background.
- type info on mouse over.
- type safety even without type annotation ( = type inference in F#).

## What is RhinoScript ?

RhinoScript provides application scripting for the [Rhino3D](https://www.rhino3d.com/) CAD app.
RhinoScript has [more than 900 functions](https://developer.rhino3d.com/api/RhinoScriptSyntax/) to control all kind of aspects of automating Rhino3D.
It was originally implemented in 2002 in VBScript.
Extensive Documentation on the original VBScript based version is available [here](https://developer.rhino3d.com/guides/rhinoscript/).


In 2010 all functions from [RhinoScript where reimplemented in IronPython](https://developer.rhino3d.com/guides/#rhinopython) (Python running on .NET).
This allowed the use of a modern, rich and dynamically typed programming language with a huge standard library and also access to all function of the underlying .NET Framework as well as the [RhinoCommon SDK](https://developer.rhino3d.com/guides/rhinocommon/).

## What is this repo?

This repo has [all](https://developer.rhino3d.com/api/RhinoScriptSyntax/) RhinoScript functions reimplemented in [F#](https://fsharp.org/)
It is literally a translation of the open source Ironpython [rhinoscriptsyntax](https://github.com/mcneel/rhinoscriptsyntax) implementation to F#.
Fuget.org is a good tool to explore the [900 methods in this repo](https://www.fuget.org/packages/Rhino.Scripting/0.8.0/lib/net48/Rhino.Scripting.dll/Rhino/Scripting).

A few minor bugs from the python implementation are fixed and a few extra methods and optional parameters where added.
I have been using this library extensively for my own professional scripting needs since 2019.
If you have problems, questions or find a bug, please open an [issue](https://github.com/goswinr/Rhino.Scripting/issues).

## Get started in C#
The recommended scripting use case is via the new [RhinoCode](https://discourse.mcneel.com/t/rhino-8-feature-rhinocode-cpython-csharp) Editor in Rhino 8.
However you can use this library just as well in compiled F#, C# or VB.net projects.
First reference the assemblies.

```csharp
#r "nuget: FSharp.Core, 6.0.7"// dependency of Rhino.Scripting
#r "nuget: FsEx, 0.15.0" // dependency of Rhino.Scripting
#r "nuget: Rhino.Scripting, 0.8.0"
```
The main namespace is  `Rhino.Scripting`.
The main class of this library is called `RhinoScriptSyntax` it has all ~900 functions as static methods.
In C# you can create an alias like this:

```csharp
using rs = Rhino.Scripting.RhinoScriptSyntax;
```

then you can use it like the RhinoScriptSyntax in Python:
```csharp
var pt =  rs.GetObject("Select an Object");
rs.ObjectColor(pt, System.Drawing.Color.Blue);
```

## Get started in F#
I will soon publish an F# scripting editor for Rhino. The prototype is working well.

First reference the assemblies.
```fsharp
#r "nuget: Rhino.Scripting, 0.8.0"
```

The main namespace is  `Rhino.Scripting`.
The main class of this library is called `RhinoScriptSyntax` it has all ~900 functions as static methods.
In F# you can create an alias like this:
```fsharp
open Rhino.Scripting
type rs = RhinoScriptSyntax
```

then use any of the RhinoScript functions like you would in Python or VBScript.
The `CoerceXXXX` functions will help you create types if you are too lazy to fully specify them.
```fsharp
let pl = rs.CoercePlane(0 , 80 , 0) // makes World XY plane at point
rs.AddText("Hello, Fesh", pl, height = 50.)
```

## How about the dynamic types and optional parameters from VBScript and Python?
Many RhinoScript function take variable types of input parameters. This is implemented with method overloads.
Many RhinoScript function have optional parameters. These are also implemented as optional method parameters.

## Change Log
`0.8.0`
- drop support for Rhino 6.0 ( 7.0 or higher is required)
- fix minor bugs about unused optional parameters

`0.7.0`
- renamed main static class from Rhino.Scripting to Rhino.Scripting.RhinoScriptSyntax !

`0.6.2`
- even better window sync with Fesh Editor

`0.6.1`
- better window sync with Fesh Editor
- fixes in docs

`0.6.0`
- don't check result of CommitChanges() anymore
- relax constraints on UserText values

`0.5.1`
- fix readme
- improve finding of SynchronizationContext

`0.4.0`
- fix threading bug in to make it work in RhinoCode
- fix typos

`0.3.0`
- remove WPF dependency
- don't return F# options anymore

`0.2.0`
- first public release
