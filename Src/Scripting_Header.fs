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

    /// The current active Rhino document (= the file currently open)
    static member Doc = State.Doc      

    /// Object Table of the current active Rhino document
    static member Ot = State.Ot

    /// A Dictionary to store state between scripting session.
    /// Use Rhino.Scripting.Sticky.Clear() to reset it.
    /// Similar to scriptingcontext.sticky in Rhino Python.
    static member val Sticky = new Dict<string, obj>() with get

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
    ///<param name="layerIndex">(int) LayerIndex</param>
    ///<param name="objectName">(string) Optional, object name</param>
    ///<param name="userTextKeysAndValues">(string*string seq) Optional, list of key value pairs for user text</param>
    ///<param name="stringSafetyCheck">(bool) Optional, Default: true. Check object name and usertext do not include line returns, tabs, and leading or trailing whitespace.</param>
    ///<param name="collapseParents">(bool) Optional, Collapse parent layers in Layer UI </param>
    ///<returns>(Guid) The Guid of the added Object.</returns>
    static member Add (  geo:'T
                      ,  layerIndex:int // dont make it  optional , so that method overload resolution works for rs.Add(..)
                      ,  [<OPT;DEF("")>]objectName:string
                      ,  [<OPT;DEF(null:seq<string*string>)>]userTextKeysAndValues:seq<string*string>
                      ,  [<OPT;DEF(true)>]stringSafetyCheck:bool 
                      ,  [<OPT;DEF(false:bool)>]collapseParents:bool
                      ) : Guid = 
        let attr =
            if layerIndex = -1 && objectName="" && isNull userTextKeysAndValues  then 
                null
            else
                let a = new DocObjects.ObjectAttributes()
                a.LayerIndex <- layerIndex
                if objectName <> "" then 
                    if stringSafetyCheck && not <|  Util.isAcceptableStringId( objectName, false) then // TODO or enforce goodStringID ?
                        RhinoScriptingException.Raise "Rhino.Scripting.Add objectName the string '%s' cannot be used as key. See Scripting.IsGoodStringId. You can use checkStrings=false parameter to bypass some of these restrictions." objectName
                    a.Name <- objectName
                if notNull userTextKeysAndValues then
                    for k,v in userTextKeysAndValues do 
                        if stringSafetyCheck then 
                            if not <|  Util.isAcceptableStringId( k, false) then // TODO or enforce goodStringID ?
                                RhinoScriptingException.Raise "Rhino.Scripting.Add SetUserText the string '%s' cannot be used as key. See Scripting.IsGoodStringId. You can use checkStrings=false parameter to bypass some of these restrictions." k
                            if not <|  Util.isAcceptableStringId( v, false) then
                                RhinoScriptingException.Raise "Rhino.Scripting.Add SetUserText the string '%s' cannot be used as value. See Scripting.IsGoodStringId. You can use checkStrings=false parameter to bypass some of these restrictions." v
                        if not <| a.SetUserString(k,v) then
                            RhinoScriptingException.Raise "Rhino.Scripting.Add: failed to set key value pair '%s' and '%s' " k v 
                a
                
        match box geo with
        | :? GeometryBase as g ->  State.Doc.Objects.Add(g,attr)
        // now the structs:
        | :? Point3d     as pt->   State.Doc.Objects.AddPoint(pt,attr)
        | :? Point3f     as pt->   State.Doc.Objects.AddPoint(pt,attr)
        | :? Line        as ln->   State.Doc.Objects.AddLine(ln,attr)
        | :? Arc         as a->    State.Doc.Objects.AddArc(a,attr)
        | :? Circle      as c->    State.Doc.Objects.AddCircle(c,attr)
        | :? Ellipse     as e->    State.Doc.Objects.AddEllipse(e,attr)
        | :? Polyline    as pl ->  State.Doc.Objects.AddPolyline(pl,attr)
        | :? Box         as b ->   State.Doc.Objects.AddBox(b,attr)
        | :? BoundingBox as b ->   State.Doc.Objects.AddBox(Box(b),attr)
        | :? Sphere      as b ->   State.Doc.Objects.AddSphere(b,attr)
        | :? Cylinder    as cl ->  State.Doc.Objects.AddSurface (cl.ToNurbsSurface(),attr)
        | :? Cone        as c ->   State.Doc.Objects.AddSurface (c.ToNurbsSurface(),attr)
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting.Add: object of type %A not implemented yet" (geo.GetType())

    ///<summary>Adds any geometry object (stuct or class) to the Rhino document.
    ///   Works not only on any subclass of GeometryBase but also on Point3d, Line, Arc and similar structs </summary>
    ///<param name="geo">the Geometry</param>
    ///<param name="layer">(string) Optional, Layername, parent layer separated by '::' </param>
    ///<param name="objectName">(string) Optional, object name</param>
    ///<param name="userTextKeysAndValues">(string*string seq) Optional, list of key value pairs for user text</param>
    ///<param name="layerColor">(Drawing.Color) Optional, Color for layer. The layer color will NOT be changed even if the layer exists already</param>
    ///<param name="stringSafetyCheck">(bool) Optional, Default: true. Check object name and usertext do not include line returns, tabs, and leading or trailing whitespace.</param>
    ///<param name="collapseParents">(bool) Optional, Collapse parent layers in Layer UI </param>
    ///<returns>(Guid) The Guid of the added Object.</returns>
    static member Add (  geo:'T
                      ,  [<OPT;DEF("")>]layer:string 
                      ,  [<OPT;DEF("")>]objectName:string
                      ,  [<OPT;DEF(null:seq<string*string>)>]userTextKeysAndValues:seq<string*string>
                      ,  [<OPT;DEF(Drawing.Color():Drawing.Color)>]layerColor:Drawing.Color    
                      ,  [<OPT;DEF(true)>]stringSafetyCheck:bool                      
                      ,  [<OPT;DEF(false:bool)>]collapseParents:bool
                      ) : Guid = 
        let layCorF =
            if layer<>""then 
                if layerColor.IsEmpty then                         
                    UtilLayer.getOrCreateLayer(layer, Color.randomForRhino, UtilLayer.ByParent, UtilLayer.ByParent, true, collapseParents) // TODO or disallow all unicode ?
                else
                    UtilLayer.getOrCreateLayer(layer, (fun () -> layerColor), UtilLayer.ByParent, UtilLayer.ByParent, true, collapseParents)// TODO or disallow all unicode ?
            else
                UtilLayer.LayerFound State.Doc.Layers.CurrentLayerIndex
        
        let layIdx =
            match layCorF with 
            |UtilLayer.LayerCreated ci -> ci
            |UtilLayer.LayerFound fi ->  fi
                (*
                // now update the layer color if one is given, even if the layer exists already
                if layerColor.IsEmpty then 
                    fi
                else
                    let lay = State.Doc.Layers.[fi]                    
                    if not <| lay.Color.EqualsARGB(layerColor) then 
                        lay.Color <- layerColor
                    fi                        
                *)

        let g = Scripting.Add( geo, layIdx, objectName, userTextKeysAndValues, stringSafetyCheck)  
        g



