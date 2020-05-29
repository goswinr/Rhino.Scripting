<!-- in VS Code press Ctrl+K and then V to see a preview-->
# Rhino.Scripting
Rhino.Scripting is an implementation of the **RhinoScript** syntax for F# (and C#).  
It enables RhinoScripts in F# and all the great coding experience that come with it, like: 
- automatic code completion while typing
- automatic error checking and highlighting in the background 
- type info on mouse over
- type certainty even without type annotaion (type inference)

## What is RhinoScript ?

RhinoScript provides application scripting for the [Rhino3D](https://www.rhino3d.com/) CAD app.  
RhinoScript has [more than 900 functions](https://developer.rhino3d.com/api/RhinoScriptSyntax/) to controll all kind of aspects of automating Rhino3D.  
It was orignally implemented in 2002 in VBScript.   
Extensive Documentation on the original VBScript based version is available [here](https://developer.rhino3d.com/guides/rhinoscript/).


In 2010 all functions from [RhinoScript where reimplemented in IronPython](https://developer.rhino3d.com/guides/#rhinopython) (Python running on .NET).  
This allowed the use of a modern, rich and dynamically typed programming language with a huge standard libray and also access to all function of the underlying .NET Framework as well as the [RhinoCommon SDK](https://developer.rhino3d.com/guides/rhinocommon/).

## What is this repro?

This repro has **all** RhinoScript functions reimpleneted in [F#](https://fsharp.org/)  
It is literaly a translation of the open scource Ironpython [rhinoscriptsyntax](https://github.com/mcneel/rhinoscriptsyntax) implementation to F#.  

## Get started 

The recomended scripting use case is via the [Seff.Rhino](https://github.com/goswinr/Seff.Rhino) Editor.   
However you can use this libray just as well in compiled F#, C# or VB.net projects.
Or even in Grasshopper C# VB.net scripting components.

First reference the assembiles. In an F# scripting editor do
```fsharp
#r @"C:\Program Files\Rhino 6\System\RhinoCommon.dll"  // adapt path if needed
#r @"D:\Git\Rhino.Scripting\src\bin\Debug\net472\Rhino.Scripting.dll"
```   
open modules 
```fsharp
open Rhino.Scripting  // to make extension members available 

type rs = RhinoScriptSyntax  // type abreviation (alias) for RhinoScriptSyntax
```
then use any of the RhinoScript functions like you would in Python or VBScript.  
The CoerceXXX functions will help you create types if you are too lazy to fully specify them.
```fsharp
let pl = rs.CoercePlane(0 , 80 , 0) // makes World XY plane at point
rs.AddText("Hello, Seff", pl, height = 50.)
```
![Seff Editor Screenshot](img/HelloSeff.png)


## How about the dynamic types from VBScript and Python?
The RhinoScript function that take variable types of input parameters are implemented with various method overloads.

## How about optional parameters?
Many RhinoScript function have optional parameters the are also implemented as optional method parameters