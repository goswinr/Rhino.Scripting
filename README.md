# Rhino.Scripting

[![Rhino.Scripting on nuget.org](https://img.shields.io/nuget/v/Rhino.Scripting.svg)](https://nuget.org/packages/Rhino.Scripting) 
[![Rhino.Scripting on fuget.org](https://www.fuget.org/packages/Rhino.Scripting/badge.svg)](https://www.fuget.org/packages/Rhino.Scripting)
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
Fuget.org is a good tool to explore the [900 methods in this repo](https://www.fuget.org/packages/Rhino.Scripting/0.5.1/lib/net48/Rhino.Scripting.dll/Rhino/Scripting).

A few minor bugs from the python implementation are fixed and a few extra methods and optional parameters where added.
I have been using this library extensively for my own professional scripting needs since 2019.
If you have problems, questions or find a bug, please open an [issue](https://github.com/goswinr/Rhino.Scripting/issues).

## Get started in C#
The recommended scripting use case is via the new [RhinoCode](https://discourse.mcneel.com/t/rhino-8-feature-rhinocode-cpython-csharp) Editor in Rhino 8.   
However you can use this library just as well in compiled F#, C# or VB.net projects.
First reference the assemblies. 

```csharp
#r "nuget: FSharp.Core, 7.0.0"
#r "nuget: FsEx, 0.13.1"
#r "nuget: Rhino.Scripting, 0.5.1"
```   
The main class of this library is called `Rhino.Scripting` it has all ~900 functions as static methods.
In C# you can create an alias like this: 

```csharp
using rs = Rhino.Scripting;
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
#r "nuget: Rhino.Scripting, 0.5.1"
```   

The main class of this library is called `Rhino.Scripting` it has all ~900 functions as static methods.
In F# you can create an alias like this: 
```fsharp
type rs = Rhino.Scripting  
```
then use any of the RhinoScript functions like you would in Python or VBScript.  
The `CoerceXXXX` functions will help you create types if you are too lazy to fully specify them.
```fsharp
let pl = rs.CoercePlane(0 , 80 , 0) // makes World XY plane at point
rs.AddText("Hello, Seff", pl, height = 50.)
```

## How about the dynamic types and optional parameters from VBScript and Python?
Many RhinoScript function take variable types of input parameters. This is implemented with method overloads.
Many RhinoScript function have optional parameters. These are also implemented as optional method parameters.


### Example
for example `rs.ObjectLayer` can be called in several ways:

`rs.ObjectLayer(guid)` To get the layer of one object, returns a string.  
`rs.ObjectLayer(guid, string)` To set the layer of one object (fails if layer does not exist), no return value.  
`rs.ObjectLayer(guid, string, createLayerIfMissing = true )` To set the layer of one object, and create the layer if it does not exist yet, no return value.  
`rs.ObjectLayer(list of guids, string)` To set the layer of several objects (fails if layer does not exist), no return value.    
`rs.ObjectLayer(list of guids, string, createLayerIfMissing = true )` To set the layer of several objects, and create the layer if it does not exist yet , no return value.

These are implemented with 3 overloads and  `Optional` and `DefaultParameterValue` parameters:
```fsharp   
    ///<summary>Returns the full layer name of an object.
    /// parent layers are separated by <c>::</c>.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<returns>(string) The object's current layer.</returns>
    static member ObjectLayer(objectId:Guid) : string = //GET
        let obj = Scripting.CoerceRhinoObject(objectId)
        let index = obj.Attributes.LayerIndex
        State.Doc.Layers.[index].FullPath

    ///<summary>Modifies the layer of an object , optionally creates layer if it does not exist yet.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="createLayerIfMissing">(bool) Optional, Default Value: <c>false</c> Set true to create Layer if it does not exist yet.</param>
    ///<param name="allowAllUnicode">(bool) Optional, Allow Ambiguous Unicode characters too </param>
    ///<param name="collapseParents">(bool) Optional, Collapse parent layers in Layer UI </param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectLayer( objectId:Guid
                             , layer:string
                             ,[<OPT;DEF(false)>]createLayerIfMissing:bool
                             ,[<OPT;DEF(false:bool)>]allowAllUnicode:bool
                             ,[<OPT;DEF(false:bool)>]collapseParents:bool) : unit = //SET
        let obj = Scripting.CoerceRhinoObject(objectId)
        let layerIndex = 
            if createLayerIfMissing then  UtilLayer.getOrCreateLayer(layer, UtilLayer.randomLayerColor, UtilLayer.ByParent, UtilLayer.ByParent, allowAllUnicode,collapseParents).Index
            else                          Scripting.CoerceLayer(layer).Index
        obj.Attributes.LayerIndex <- layerIndex
        if not <| obj.CommitChanges() then RhinoScriptingException.Raise "Rhino.Scripting.ObjectLayer: Setting it failed for layer '%s' on: %s " layer (Nice.str objectId)
        State.Doc.Views.Redraw()

    ///<summary>Modifies the layer of multiple objects, optionally creates layer if it does not exist yet.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of the objects</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="createLayerIfMissing">(bool) Optional, Default Value: <c>false</c> Set true to create Layer if it does not exist yet.</param>
    ///<param name="allowUnicode">(bool) Optional, Allow Ambiguous Unicode characters too </param>
    ///<param name="collapseParents">(bool) Optional, Collapse parent layers in Layer UI </param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectLayer( objectIds:Guid seq
                             , layer:string
                             , [<OPT;DEF(false)>]createLayerIfMissing:bool
                             , [<OPT;DEF(false:bool)>]allowUnicode:bool
                             , [<OPT;DEF(false:bool)>]collapseParents:bool) : unit = //MULTISET
        let layerIndex = 
            if createLayerIfMissing then  UtilLayer.getOrCreateLayer(layer, UtilLayer.randomLayerColor, UtilLayer.ByParent, UtilLayer.ByParent, allowUnicode, collapseParents).Index
            else                          Scripting.CoerceLayer(layer).Index
        for objectId in objectIds do
            let obj = Scripting.CoerceRhinoObject(objectId)
            obj.Attributes.LayerIndex <- layerIndex
            if not <| obj.CommitChanges() then RhinoScriptingException.Raise "Rhino.Scripting.ObjectLayer: Setting it failed for layer '%s' and '%s' of %d objects"  layer (Nice.str objectId) (Seq.length objectIds)
        State.Doc.Views.Redraw()
```


## Contributing
Contributions are welcome even for small things like typos. If you have problems with this library please submit an issue.

## Change Log

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
