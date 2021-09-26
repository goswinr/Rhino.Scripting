namespace Rhino

open System
open System.Collections.Generic
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators
open System.Globalization

// This file and all other files with the name Scripting_**.fs will be combined into one large file called Scripting.fs before compiling.
// This is done via the script combineIntoOneFile.fsx that is invoked as part of the build process.
// This build process is needed because F# extension members dont work in C#, and C# extension members via Extension attribute show as instance mebers when useds in F#.
// Autocomplete would not work well if this file has 20k lines while beeing edited.

/// A static class with static methods providing functions identical to RhinoScript in Python or VBscript
[<AbstractClass; Sealed>]
type Scripting private () = 

    // static class, use these attributes [<AbstractClass; Sealed>] to match C# static class
    // and make in visible in C# // https://stackoverflow.com/questions/13101995/defining-static-classes-in-f
    

    /// A Dictionary to store state between scripting session.
    /// Use Rhino.Scripting.Sticky.Clear() to reset it.
    /// Similar to scriptingcontext.sticky in Rhino Python.
    static member val Sticky = new Dictionary<string, obj>() with get

    /// An Integer Enum of Object types.
    /// To be use in object selection functions such as rs.GetObjects().
    static member val Filter = new ObjectFilterEnum ()

    /// Tests to see if the user has pressed the escape key.
    /// Raises an OperationCanceledException.
    static member EscapeTest() : unit = // [<OPT;DEF(true)>]throwException:bool, [<OPT;DEF(false)>]reset:bool) : bool = 
        RhinoApp.Wait() //does not need to be on  UI thread
        if State.EscapePressed  then
            State.EscapePressed <- false //allways reset is needed otherwise in next run of sript will not be reset
            raise ( OperationCanceledException("Esc key was pressed and caught via Scripting.EscapeTest()"))


    ///<summary>Clamps a value between a lower and an upper bound.</summary>
    ///<param name="minVal">(float) The lower bound</param>
    ///<param name="maxVal">(float) The upper bound</param>
    ///<param name="value">(float) The value to clamp</param>
    ///<returns>(float) The clamped value.</returns>
    static member Clamp (minVal:float, maxVal:float, value:float) : float = 
        if minVal > maxVal then  RhinoScriptingException.Raise "Rhino.Scripting.Clamp: lowvalue %A must be less than highvalue %A" minVal maxVal
        max minVal (min maxVal value)


    ///<summary>Like the Python 'xrange' function for integers this creates a range of floating point values.
    ///    The last or stop value will NOT be included in range as per python semantics, this is different from F# semantics on range expressions.
    ///    Use FsEx.UtilMath.floatRange(...) to include stop value in range.</summary>
    ///<param name="start">(float) first value of range</param>
    ///<param name="stop">(float) end of range (The last value will not be included in range, Python semantics.)</param>
    ///<param name="step">(float) step size between two values</param>
    ///<returns>(float seq) a lazy seq of loats.</returns>
    static member FxrangePython (start:float, stop:float, step:float) : float seq = 
        if isNanOrInf start then RhinoScriptingException.Raise "Rhino.Scripting.FxrangePython: NaN or Infinity, start=%f, step=%f, stop=%f" start step stop
        if isNanOrInf step  then RhinoScriptingException.Raise "Rhino.Scripting.FxrangePython: NaN or Infinity, start=%f, step=%f, stop=%f" start step stop
        if isNanOrInf stop  then RhinoScriptingException.Raise "Rhino.Scripting.FxrangePython: NaN or Infinity, start=%f, step=%f, stop=%f" start step stop
        let range = stop - start
                    |> BitConverter.DoubleToInt64Bits
                    |> (+) 15L // to make sure stop value is included in Range, this will then explicitly be removed below to match python semantics
                    |> BitConverter.Int64BitsToDouble
        let steps = range/step - 1.0 // -1 to make sure stop value is not included(python semanticsis diffrent from F# semantics on range expressions)
        if isNanOrInf steps then RhinoScriptingException.Raise "Rhino.Scripting.FxrangePython range/step in frange: %f / %f is NaN Infinity, start=%f, stop=%f" range step start stop

        if steps < 0.0 then
            RhinoScriptingException.Raise "Rhino.Scripting.FxrangePython: Stop value cannot be reached: start=%f, step=%f, stop=%f (steps:%f)" start step stop steps //or Seq.empty
        else
            // the actual algorithm:
            let rec floatrange (start, i, steps) = 
                seq { if i <= steps then
                        yield start + i*step
                        yield! floatrange (start, (i + 1.0), steps) } // tail recursive ?
            floatrange (start, 0.0, steps)

    ///<summary>Like the Python 'range' function for integers this creates a range of floating point values.
    ///    This last or stop value will NOT be included in range as per python semantics, this is different from F# semantics on range expressions.
    ///    Use FsEx.UtilMath.floatRange(...) to include stop value in range.</summary>
    ///<param name="start">(float) first value of range</param>
    ///<param name="stop">(float) end of range( The last value will not be included in range, Python semantics.)</param>
    ///<param name="step">(float) step size between two values</param>
    ///<returns>(float Rarr).</returns>
    static member FrangePython (start:float, stop:float, step:float) : float Rarr = 
        Scripting.FxrangePython (start, stop, step) |> Rarr.ofSeq


    ///<summary>Adds any geometry object (stuct or class) to the Rhino document.
    /// works not only on any subclass of GeometryBase but also on Point3d, Line, Arc and similar structs </summary>
    ///<param name="geo">the Geometry</param>
    ///<returns>(Guid) The Guid of the added Object.</returns>
    static member Add (geo:'T) : Guid = 
        match box geo with
        | :? GeometryBase as g -> State.Doc.Objects.Add(g)
        // now the structs:
        | :? Point3d    as pt->   State.Doc.Objects.AddPoint pt
        | :? Point3f    as pt->   State.Doc.Objects.AddPoint(pt)
        | :? Line       as ln->   State.Doc.Objects.AddLine(ln)
        | :? Arc        as a->    State.Doc.Objects.AddArc(a)
        | :? Circle     as c->    State.Doc.Objects.AddCircle(c)
        | :? Ellipse    as e->    State.Doc.Objects.AddEllipse(e)
        | :? Polyline   as pl ->  State.Doc.Objects.AddPolyline(pl)
        | :? Box        as b ->   State.Doc.Objects.AddBox(b)
        | :? BoundingBox as b ->  State.Doc.Objects.AddBox(Box(b))
        | :? Sphere     as b ->   State.Doc.Objects.AddSphere(b)
        | :? Cylinder   as cl -> State.Doc.Objects.AddSurface (cl.ToNurbsSurface())
        | :? Cone       as c ->   State.Doc.Objects.AddSurface (c.ToNurbsSurface())
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.Add: object of type %A not implemented yet" (geo.GetType())


    /// The current active Rhino document (= the file currently open)
    static member Doc = State.Doc

    /// Object Table of the current active Rhino document
    static member Ot = State.Ot


