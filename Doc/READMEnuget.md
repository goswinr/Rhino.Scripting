<!-- in VS Code press Ctrl + Shift + V to see a preview-->
# Rhino.Scripting

![logo](https://raw.githubusercontent.com/goswinr/Rhino.Scripting/main/Doc/logo400.png)

Rhino.Scripting is an implementation of the **RhinoScript** syntax in and for F# (and C#).  
It enables the use of RhinoScript in F# and all the great coding experience that come with F# and C#, like: 
- automatic code completion while typing
- automatic error checking and highlighting in the background 
- type info on mouse over
- type certainty even without type annotation (type inference)

## What is RhinoScript ?

RhinoScript provides application scripting for the [Rhino3D](https://www.rhino3d.com/) CAD app.  
RhinoScript has [more than 900 functions](https://developer.rhino3d.com/api/RhinoScriptSyntax/) to control all kind of aspects of automating Rhino3D.  
It was originally implemented in 2002 in VBScript.   
Extensive Documentation on the original VBScript based version is available [here](https://developer.rhino3d.com/guides/rhinoscript/).


In 2010 all functions from [RhinoScript where reimplemented in IronPython](https://developer.rhino3d.com/guides/#rhinopython) (Python running on .NET).  
This allowed the use of a modern, rich and dynamically typed programming language with a huge standard library and also access to all function of the underlying .NET Framework as well as the [RhinoCommon SDK](https://developer.rhino3d.com/guides/rhinocommon/).

## What is this repro?

This repro has [all](https://developer.rhino3d.com/api/RhinoScriptSyntax/) RhinoScript functions reimplemented in [F#](https://fsharp.org/)  
It is literally a translation of the open source Ironpython [rhinoscriptsyntax](https://github.com/mcneel/rhinoscriptsyntax) implementation to F#.  

## Get started 

The recommended scripting use case is via the [RhinoCode](https://discourse.mcneel.com/t/rhino-8-feature-rhinocode-cpython-csharp) Editor.   
However you can use this library just as well in compiled F#, C# or VB.net projects.
Or even in Grasshopper C# VB.net scripting components.

First reference the assemblies. 
In an F# or C# scripting editor do
```fsharp
#r "nuget: Rhino.Scripting"
```   

The main class of this library is called `Rhino.Scripting` it has all ~900 functions as static methods.
In F# you can create a shortcut like this: 
```fsharp
type rs = Rhino.Scripting  // type abbreviation  (alias) for RhinoScriptSyntax
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

`rs.ObjectLayer(guid)` to get the layer of one object, returns a string  
`rs.ObjectLayer(guid, string)` to set the layer of one object (fails if layer does not exist), no return value  
`rs.ObjectLayer(guid, string, createLayerIfMissing = true )` to set the layer of one object, and create the layer if it does not exist yet, no return value  
`rs.ObjectLayer(list of guids, string)` to set the layer of several objects (fails if layer does not exist), no return value    
`rs.ObjectLayer(list of guids, string, createLayerIfMissing = true )` to set the layer of several objects, and create the layer if it does not exist yet , no return value

these are implemented with 3 overloads and  `Optional` and `DefaultParameterValue` parameters:
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
