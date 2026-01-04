namespace Rhino.Scripting

open Rhino

/// An integer Enum of Object types to be used in object selection functions of Rhino.Scripting.
/// Don't create an instance, use the instance in Rhino.Scripting.RhinoScriptSyntax.Filter
[<Sealed>]
type ObjectFilterEnum internal () =  // not a static class, just internal, constructor is used once in static class Scripting
    /// returns 0
    member _.AllObjects = 0
    /// returns 1
    member _.Point = 1
    /// returns 2
    member _.PointCloud = 2
    /// returns 4
    member _.Curve = 4
    /// returns 8
    member _.Surface = 8
    /// returns 16
    member _.PolySurface = 16
    /// returns 32
    member _.Mesh = 32
    /// returns 256
    member _.Light = 256
    /// returns 512, for Text, leaders, and dimension lines
    member _.Annotation = 512
    /// returns 4096, for block instances
    member _.Instance = 4096
    /// returns 8192
    member _.Textdot = 8192
    /// returns 16384
    member _.Grip = 16384
    /// returns 32768, for detail view objects
    member _.Detail = 32768
    /// returns 65536
    member _.Hatch = 65536
    /// returns 131072
    member _.Morph = 131072
    /// returns 262144
    member _.SubD = 262144
    /// returns 134217728
    member _.Cage = 134217728
    /// returns 268435456
    member _.Phantom = 268435456
    /// returns 536870912
    member _.ClippingPlane = 536870912
    /// returns 1073741824
    member _.Extrusion = 1073741824

    /// A helper function to get a DocObjects.ObjectType Enum from an integer
    static member GetFilterEnum(i:int) : DocObjects.ObjectType = // not internal because also used in Rhino.Scripting.Fsharp
        let mutable e = DocObjects.ObjectType.None
        if 0 <> (i &&& 1 ) then          e  <- e ||| DocObjects.ObjectType.Point
        if 0 <> (i &&& 16384 ) then      e  <- e ||| DocObjects.ObjectType.Grip
        if 0 <> (i &&& 2 ) then          e  <- e ||| DocObjects.ObjectType.PointSet
        if 0 <> (i &&& 4 ) then          e  <- e ||| DocObjects.ObjectType.Curve
        if 0 <> (i &&& 8 ) then          e  <- e ||| DocObjects.ObjectType.Surface
        if 0 <> (i &&& 16 ) then         e  <- e ||| DocObjects.ObjectType.Brep
        if 0 <> (i &&& 32 ) then         e  <- e ||| DocObjects.ObjectType.Mesh
        if 0 <> (i &&& 512 ) then        e  <- e ||| DocObjects.ObjectType.Annotation
        if 0 <> (i &&& 256 ) then        e  <- e ||| DocObjects.ObjectType.Light
        if 0 <> (i &&& 4096 ) then       e  <- e ||| DocObjects.ObjectType.InstanceReference
        if 0 <> (i &&& 134217728 ) then  e  <- e ||| DocObjects.ObjectType.Cage
        if 0 <> (i &&& 65536 ) then      e  <- e ||| DocObjects.ObjectType.Hatch
        if 0 <> (i &&& 131072 ) then     e  <- e ||| DocObjects.ObjectType.MorphControl
        if 0 <> (i &&& 262144 ) then     e  <- e ||| DocObjects.ObjectType.SubD
        if 0 <> (i &&& 2097152 ) then    e  <- e ||| DocObjects.ObjectType.PolysrfFilter
        if 0 <> (i &&& 268435456 ) then  e  <- e ||| DocObjects.ObjectType.Phantom
        if 0 <> (i &&& 8192 ) then       e  <- e ||| DocObjects.ObjectType.TextDot
        if 0 <> (i &&& 32768 ) then      e  <- e ||| DocObjects.ObjectType.Detail
        if 0 <> (i &&& 536870912 ) then  e  <- e ||| DocObjects.ObjectType.ClipPlane
        if 0 <> (i &&& 1073741824 ) then e  <- e ||| DocObjects.ObjectType.Extrusion
        e
