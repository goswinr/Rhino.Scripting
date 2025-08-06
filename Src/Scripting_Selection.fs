namespace Rhino.Scripting

open Rhino
open System
open Rhino.Geometry
open Rhino.Scripting.RhinoScriptingUtils

[<AutoOpen>]
module AutoOpenSelection =
  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}

    // moved to Rhino.RhinoScriptSyntax.Fsharp project:
    // static member ShownObjects(
    // static member GetObjectsAndRemember(
    // static member GetObjectAndRemember(

    /// <summary>Returns identifiers of all objects in the document.</summary>
    /// <param name="select">(bool) Optional, default value: <c>false</c> Select the objects</param>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c> Include light objects</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c> Include grips objects</param>
    /// <param name="includeReferences">(bool) Optional, default value: <c>false</c> Include reference objects such as work session objects</param>
    /// <returns>(Guid ResizeArray) Identifiers for all the objects in the document.</returns>
    static member AllObjects(  [<OPT;DEF(false)>]select:bool,
                               [<OPT;DEF(false)>]includeLights:bool,
                               [<OPT;DEF(false)>]includeGrips:bool,
                               [<OPT;DEF(false)>]includeReferences:bool) : Guid ResizeArray =
            let it = DocObjects.ObjectEnumeratorSettings()
            it.IncludeLights <- includeLights
            it.IncludeGrips <- includeGrips
            it.NormalObjects <- true
            it.LockedObjects <- true
            it.HiddenObjects <- true
            it.ReferenceObjects <- includeReferences
            let es = State.Doc.Objects.GetObjectList(it)
            let objectIds = ResizeArray()
            for ob in es do
                objectIds.Add ob.Id
                if select then ob.Select(true) |> ignore<int>   // TODO needs sync ? apparently not needed!
            if objectIds.Count > 0 && select then State.Doc.Views.Redraw()
            objectIds


    /// <summary>Returns identifier of the first object in the document. The first
    ///    object is the last object created by the user.</summary>
    /// <param name="select">(bool) Optional, default value: <c>false</c> Select the object. If omitted, the object is not selected</param>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c> Include light objects. If omitted, light objects are not returned</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c> Include grips objects. If omitted, grips objects are not returned</param>
    /// <returns>(Guid) The identifier of the object.</returns>
    static member FirstObject(      [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(false)>]includeLights:bool,
                                    [<OPT;DEF(false)>]includeGrips:bool) : Guid =
            let it = DocObjects.ObjectEnumeratorSettings()
            it.IncludeLights <- includeLights
            it.IncludeGrips <- includeGrips
            let e = State.Doc.Objects.GetObjectList(it).GetEnumerator()
            if not <| e.MoveNext() then RhinoScriptingException.Raise "RhinoScriptSyntax.FirstObject not found"
            let object = e.Current
            if isNull object then RhinoScriptingException.Raise "RhinoScriptSyntax.FirstObject not found(null)"
            if select then object.Select(true) |> ignore<int> // TODO needs sync ? apparently not needed!
            object.Id




    /// <summary>Prompts user to pick or select a single Curve object.</summary>
    /// <param name="message">(string) Optional, A prompt or message</param>
    /// <param name="preselect">(bool) Optional, default value: <c>true</c> Allow for the selection of pre-selected objects</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c> Select the picked objects. If False, objects that are picked are not selected</param>
    /// <returns>(Guid * bool * int * Point3d * float * string) Tuple containing the following information
    ///    [0]  guid     identifier of the Curve object
    ///    [1]  bool     True if the Curve was preselected, otherwise False
    ///    [2]  Enum     DocObjects.SelectionMethod
    ///    [3]  point    selection point
    ///    [4]  number   the Curve parameter of the selection point
    ///    [5]  str      name of the view selection was made.
    /// A RhinoUserInteractionException is raised if input is cancelled via Esc Key.</returns>
    static member GetCurveObject(   [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool) : Guid * bool * DocObjects.SelectionMethod * Point3d * float * string =
        let get () =  // TODO Add check if already hidden, then don't even hide and show
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore<int>
                State.Doc.Views.Redraw()
            let go = new Input.Custom.GetObject()
            if notNull message then go.SetCommandPrompt(message)
            go.GeometryFilter <- DocObjects.ObjectType.Curve
            go.SubObjectSelect <- false
            go.GroupSelect <- false
            go.AcceptNothing(true)
            let res = go.Get()
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in RhinoScriptSyntax.GetCurveObject(message=%A), Interaction result: %A" message res
            else
                let objref = go.Object(0)
                let objectId = objref.ObjectId
                let presel = go.ObjectsWerePreselected
                let selmethod = objref.SelectionMethod()
                let point = objref.SelectionPoint()
                let _, curveparameter = objref.CurveParameter() // _ = curve
                let viewname = go.View().ActiveViewport.Name
                let obj = go.Object(0).Object()
                go.Dispose()
                if not <| select && not <| preselect then
                    State.Doc.Objects.UnselectAll()|> ignore<int>
                    State.Doc.Views.Redraw()
                obj.Select(select)  |> ignore<int>
                (objectId, presel, selmethod, point, curveparameter, viewname)
        RhinoSync.DoSyncRedrawHideEditor get


    /// <summary>Prompts user to pick, or select, a single object.
    /// Raises a RhinoUserInteractionException if no object was selected. For example when Esc key was pressed.</summary>
    /// <param name="message">(string) Optional, A prompt or message</param>
    /// <param name="filter">(int) Optional, The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the RhinoScriptSyntax.Filter enum to get values, they can be joined with '+'</param>
    /// <param name="preselect">(bool) Optional, default value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    /// <param name="customFilter">(Input.Custom.GetObjectGeometryFilter) Optional, A custom filter function</param>
    /// <param name="subObjects">(bool) Optional, default value: <c>false</c>
    ///    If True, sub-objects can be selected. When this is the
    ///    case, for tracking  of the sub-object go via the Object Ref</param>
    /// <returns>(Guid) Identifier of the picked object.
    /// A RhinoUserInteractionException is raised if input is cancelled via Esc Key.</returns>
    static member GetObject(        [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(0)>]filter:int,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(null:Input.Custom.GetObjectGeometryFilter)>]customFilter:Input.Custom.GetObjectGeometryFilter,
                                    [<OPT;DEF(false)>]subObjects:bool) : Guid =
        let get () =
            if not  preselect then
                State.Doc.Objects.UnselectAll() |> ignore<int>
                State.Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull customFilter then go.SetCustomGeometryFilter(customFilter)
            if notNull message then go.SetCommandPrompt(message)
            if filter>0 then go.GeometryFilter <- ObjectFilterEnum.GetFilterEnum(filter)
            go.SubObjectSelect <- subObjects
            go.GroupSelect <- false
            go.AcceptNothing(true)
            let res = go.Get()
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in RhinoScriptSyntax.GetObject(message=%A), Interaction result: %A" message res
            else
                let objref = go.Object(0)
                let obj = objref.Object()
                //let presel = go.ObjectsWerePreselected
                go.Dispose()
                //if not <| select && not <| preselect then State.Doc.Objects.UnselectAll() |> ignore<int>  State.Doc.Views.Redraw()
                if select then
                    obj.Select(select)  |> ignore<int>
                else
                    State.Doc.Objects.UnselectAll() |> ignore<int>
                State.Doc.Views.Redraw()
                obj.Id
        RhinoSync.DoSyncRedrawHideEditor get


    /// <summary>Prompts user to pick, or select a single object.
    /// Raises a RhinoUserInteractionException if no object was selected. For example when Esc key was pressed.</summary>
    /// <param name="message">(string) Optional, A prompt or message</param>
    /// <param name="filter">(int) Optional, default value: <c>0</c>
    ///    The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the filter class to get values</param>
    /// <param name="preselect">(bool) Optional, default value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    /// <param name="objects">(Guid seq) Optional, List of object identifiers specifying objects that are
    ///    allowed to be selected</param>
    /// <returns>(Guid * bool * float * Point3d * string) Tuple containing the following information
    ///    [0] identifier of the object
    ///    [1] True if the object was preselected, otherwise False
    ///    [2] selection method Enum DocObjects.SelectionMethod
    ///         (0) selected by non-mouse method (SelAll, etc.).
    ///         (1) selected by mouse click on the object.
    ///         (2) selected by being inside of a mouse window.
    ///         (3) selected by intersecting a mouse crossing window.
    ///    [3] selection point
    ///    [4] name of the view selection was made.
    /// A RhinoUserInteractionException is raised if input is cancelled via Esc Key.</returns>
    static member GetObjectEx(      [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(0)>]filter:int,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(null:Guid seq)>]objects:Guid seq) : Guid * bool * DocObjects.SelectionMethod * Point3d * string =
        let get () =
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore<int>
                State.Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull objects then
                let s = System.Collections.Generic.HashSet(objects)
                go.SetCustomGeometryFilter(fun rhinoObject _ _ -> s.Contains(rhinoObject.Id))
            if notNull message then
                go.SetCommandPrompt(message)
            if filter>0 then
                go.GeometryFilter <- ObjectFilterEnum.GetFilterEnum(filter)
            go.SubObjectSelect <- false
            go.GroupSelect <- false
            go.AcceptNothing(true)
            let res = go.Get()
            if res <> Input.GetResult.Object then

                RhinoUserInteractionException.Raise "No Object was selected in RhinoScriptSyntax.GetObjectEx(message=%A), Interaction result: %A" message res
            else
                let objref = go.Object(0)
                let objectId = objref.ObjectId
                let presel = go.ObjectsWerePreselected
                let selmethod = objref.SelectionMethod()
                let point = objref.SelectionPoint()
                let viewname = go.View().ActiveViewport.Name
                let obj = go.Object(0).Object()
                go.Dispose()
                if not <| select && not <| presel then
                    State.Doc.Objects.UnselectAll() |> ignore<int>
                    State.Doc.Views.Redraw()
                obj.Select(select) |> ignore<int>
                (objectId, presel, selmethod, point, viewname)
        RhinoSync.DoSyncRedrawHideEditor get


    /// <summary>Prompts user to pick or select one or more objects.
    /// Raises a RhinoUserInteractionException if no object was selected. For example when Esc key was pressed.</summary>
    /// <param name="message">(string) Optional, default value: <c>"Select objects"</c>
    ///    A prompt or message</param>
    /// <param name="filter">(int) Optional, The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the RhinoScriptSyntax.Filter enum to get values, they can be joined with '+'</param>
    /// <param name="group">(bool) Optional, default value: <c>true</c>
    ///    Honor object grouping. If omitted and the user picks a group,
    ///    the entire group will be picked (True). Note, if filter is set to a
    ///    value other than 0 (All objects), then group selection will be disabled</param>
    /// <param name="preselect">(bool) Optional, default value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    /// <param name="objectsToSelectFrom">(Guid seq) Optional, List of objects that are allowed to be selected. If set customFilter will be ignored</param>
    /// <param name="minimumCount">(int) Optional, default value: <c>1</c>
    ///    Minimum count of objects allowed to be selected</param>
    /// <param name="maximumCount">(int) Optional, default value: <c>0</c>
    ///    Maximum count of objects allowed to be selected</param>
    /// <param name="printCount">(bool) Optional, default value: <c>false</c> Print object count to command window.</param>
    /// <param name="customFilter">(Input.Custom.GetObjectGeometryFilter) Optional, Will be ignored if 'objects' are set. Calls a custom function in the script and passes
    ///    the Rhino Object, Geometry, and component index and returns true or false indicating if the object can be selected</param>
    /// <returns>(Guid ResizeArray) List of identifiers of the picked objects.
    /// A RhinoUserInteractionException is raised if input is cancelled via Esc Key.</returns>
    static member GetObjects(       [<OPT;DEF("Select objects")>]message:string,
                                    [<OPT;DEF(0)>]filter:int,
                                    [<OPT;DEF(true)>]group:bool,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(null:Guid seq)>]objectsToSelectFrom:Guid seq,
                                    [<OPT;DEF(1)>]minimumCount:int,
                                    [<OPT;DEF(0)>]maximumCount:int,
                                    [<OPT;DEF(false)>]printCount:bool,
                                    [<OPT;DEF(null:Input.Custom.GetObjectGeometryFilter)>]customFilter:Input.Custom.GetObjectGeometryFilter)  : ResizeArray<Guid> =
        let get () =
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore<int>
                State.Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull objectsToSelectFrom then
                let s = System.Collections.Generic.HashSet(objectsToSelectFrom)
                go.SetCustomGeometryFilter(fun rhinoObject _ _ -> s.Contains(rhinoObject.Id))
            elif notNull customFilter then
                go.SetCustomGeometryFilter(customFilter)
            go.SetCommandPrompt(message )
            let geometryFilter = ObjectFilterEnum.GetFilterEnum(filter)
            if filter>0 then go.GeometryFilter <- geometryFilter
            go.SubObjectSelect <- false
            go.GroupSelect <- group
            go.AcceptNothing(true)
            let res = go.GetMultiple(minimumCount, maximumCount)
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in RhinoScriptSyntax.GetObjects(message=%A), Interaction result: %A" message res
            else
                if not <| select && not <| go.ObjectsWerePreselected then
                    State.Doc.Objects.UnselectAll() |> ignore<int>
                    State.Doc.Views.Redraw()
                let rc = ResizeArray()
                let count = go.ObjectCount
                for i = 0 to count - 1 do
                    let objref = go.Object(i)
                    rc.Add(objref.ObjectId)
                    let obj = objref.Object()
                    if select && notNull obj then obj.Select(select) |> ignore<int>
                if printCount then PrettySetup.printfnBlue $"RhinoScriptSyntax.GetObjects(\"{message}\") returned {RhinoScriptSyntax.ObjectDescription rc}"
                rc
        RhinoSync.DoSyncRedrawHideEditor get



    /// <summary>Prompts user to pick, or select one or more objects.
    /// Raises a RhinoUserInteractionException if no object was selected. For example when Esc key was pressed.</summary>
    /// <param name="message">(string) Optional, default value: <c>"Select objects"</c>
    ///    A prompt or message</param>
    /// <param name="filter">(int) Optional, default value: <c>0</c>
    ///    The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the filter class to get values</param>
    /// <param name="group">(bool) Optional, default value: <c>true</c>
    ///    Honor object grouping. If omitted and the user picks a group,
    ///    the entire group will be picked (True). Note, if filter is set to a
    ///    value other than 0 (All objects), then group selection will be disabled</param>
    /// <param name="preselect">(bool) Optional, default value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    /// <param name="printCount">(bool) Optional, default value: <c>false</c> Print object count to command window</param>
    /// <param name="objectsToSelectFrom">(Guid seq) Optional, List of object identifiers specifying objects that are
    ///    allowed to be selected</param>
    /// <returns>((Guid*bool*int*Point3d*string) ResizeArray) List containing the following information
    ///    [n][0]  identifier of the object
    ///    [n][1]  True if the object was preselected, otherwise False
    ///    [n][2]  selection method (DocObjects.SelectionMethod)
    ///    [n][3]  selection point
    ///    [n][4]  name of the view selection was made.
    /// A RhinoUserInteractionException is raised if input is cancelled via Esc Key.</returns>
    static member GetObjectsEx(     [<OPT;DEF("Select objects")>]message:string,
                                    [<OPT;DEF(0)>]filter:int,
                                    [<OPT;DEF(true)>]group:bool,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(false)>]printCount:bool,
                                    [<OPT;DEF(null:Guid seq)>]objectsToSelectFrom:Guid seq) : (Guid*bool*DocObjects.SelectionMethod*Point3d*string) ResizeArray =
        let get () =
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore<int>
                State.Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull objectsToSelectFrom then
                let s = System.Collections.Generic.HashSet(objectsToSelectFrom)
                go.SetCustomGeometryFilter(fun rhinoObject _ _ -> s.Contains(rhinoObject.Id))
            go.SetCommandPrompt(message)
            let geometryfilter = ObjectFilterEnum.GetFilterEnum(filter)
            if filter>0 then go.GeometryFilter <- geometryfilter
            go.SubObjectSelect <- false
            go.GroupSelect <- group
            go.AcceptNothing(true)
            let res = go.GetMultiple(1, 0)
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in RhinoScriptSyntax.GetObjectsEx(message=%A), Interaction result: %A" message res
            else
                if not <| select && not <| go.ObjectsWerePreselected then
                    State.Doc.Objects.UnselectAll() |> ignore<int>
                    State.Doc.Views.Redraw()
                let rc = ResizeArray()
                let count = go.ObjectCount
                for i = 0 to count - 1 do
                    let objref = go.Object(i)
                    let objectId = objref.ObjectId
                    let presel = go.ObjectsWerePreselected
                    let selmethod = objref.SelectionMethod()
                    let point = objref.SelectionPoint()
                    let viewname = go.View().ActiveViewport.Name
                    rc.Add( (objectId, presel, selmethod, point, viewname))
                    let obj = objref.Object()
                    if select && notNull obj then obj.Select(select) |> ignore<int>
                if printCount then
                    rc
                    |> RArr.map ( fun (id, _, _, _, _) -> id )
                    |> RhinoScriptSyntax.ObjectDescription
                    |> PrettySetup.printfnBlue "RhinoScriptSyntax.GetObjectsEx(\"%s\") returned %s" message
                rc
        RhinoSync.DoSyncRedrawHideEditor get


    /// <summary>Prompts the user to select one or more point objects.</summary>
    /// <param name="message">(string) Optional, default value: <c>"Select Point Objects"</c>
    ///    A prompt message</param>
    /// <param name="preselect">(bool) Optional, default value: <c>true</c>
    ///    Allow for the selection of pre-selected objects. If omitted, pre-selected objects are not accepted</param>
    /// <returns>(Point3d ResizeArray) List of 3d points.</returns>
    static member GetPointCoordinates(  [<OPT;DEF("Select Point Objects")>] message:string,
                                        [<OPT;DEF(false)>]                  preselect:bool) : Point3d ResizeArray =
        let ids =  RhinoScriptSyntax.GetObjects(message, RhinoScriptSyntax.Filter.Point, preselect = preselect)
        let rc = ResizeArray<Point3d>()
        for objectId in ids do
            let pt = RhinoScriptSyntax.Coerce3dPoint(objectId)
            rc.Add(pt)
        rc




    /// <summary>Prompts the user to select a single Surface.
    /// Raises a RhinoUserInteractionException if no object was selected. For example when Esc key was pressed.</summary>
    /// <param name="message">(string) Optional, default value: <c>"Select Surface"</c>
    ///    Prompt displayed</param>
    /// <param name="preselect">(bool) Optional, default value: <c>true</c>
    ///    Allow for preselected objects</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the picked object</param>
    /// <returns>((Guid * bool * DocObjects.SelectionMethod * Point3d * (float * float) * string))
    ///    [0]  identifier of the Surface
    ///    [1]  True if the Surface was preselected, otherwise False
    ///    [2]  selection method ( DocObjects.SelectionMethod )
    ///    [3]  selection point
    ///    [4]  u, v Surface parameter of the selection point
    ///    [5]  name of the view in which the selection was made.
    /// A RhinoUserInteractionException is raised if input is cancelled via Esc Key.</returns>
    static member GetSurfaceObject( [<OPT;DEF("Select surface")>]message:string, // TODO add selection method return value.  see help
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool) : Guid * bool * DocObjects.SelectionMethod * Point3d * (float * float) * string =
        let get () =
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore<int>
                State.Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            go.SetCommandPrompt(message)
            go.GeometryFilter <- DocObjects.ObjectType.Surface
            go.SubObjectSelect <- false
            go.GroupSelect <- false
            go.AcceptNothing(true)
            let res = go.Get()
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in RhinoScriptSyntax.GetSurfaceObject(message=%A), Interaction result: %A" message res
            else
                let objref = go.Object(0)
                let rhobj = objref.Object()
                rhobj.Select(select) |> ignore<int>
                State.Doc.Views.Redraw()
                let objectId = rhobj.Id
                let prePicked = go.ObjectsWerePreselected
                let selmethod = objref.SelectionMethod()
                let mutable point = objref.SelectionPoint()
                let _, u, v = objref.SurfaceParameter()
                let mutable uv = (u, v)
                if not <| point.IsValid then
                    point <- Point3d.Unset
                    uv <- RhinoMath.UnsetValue, RhinoMath.UnsetValue
                let view = go.View()
                let name = view.ActiveViewport.Name
                go.Dispose()
                if not <| select && not <| prePicked then
                    State.Doc.Objects.UnselectAll() |> ignore<int>
                    State.Doc.Views.Redraw()
                (objectId, prePicked, selmethod, point, uv, name)
        RhinoSync.DoSyncRedrawHideEditor get


    /// <summary>Returns identifiers of all locked objects in the document. Locked objects
    ///    cannot be snapped to, and cannot be selected.</summary>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include light objects</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c>
    ///    Include grip objects</param>
    /// <param name="includeReferences">(bool) Optional, default value: <c>false</c>
    ///    Include reference objects such as work session objects</param>
    /// <returns>(Guid ResizeArray) identifiers the locked objects.</returns>
    static member LockedObjects(    [<OPT;DEF(false)>]includeLights:bool,
                                    [<OPT;DEF(false)>]includeGrips:bool,
                                    [<OPT;DEF(false)>]includeReferences:bool) : Guid ResizeArray =
            let settings = DocObjects.ObjectEnumeratorSettings()
            settings.ActiveObjects <- true
            settings.NormalObjects <- true
            settings.LockedObjects <- true
            settings.HiddenObjects <- true
            settings.IncludeLights <- includeLights
            settings.IncludeGrips <- includeGrips
            settings.ReferenceObjects <- includeReferences
            let rc = ResizeArray()
            for i in State.Doc.Objects.GetObjectList(settings) do
                if i.IsLocked || (State.Doc.Layers.[i.Attributes.LayerIndex]).IsLocked then
                    rc.Add(i.Id)
            rc

    /// <summary>Returns identifiers of all hidden objects in the document. Hidden objects
    ///    are not visible, cannot be snapped to, and cannot be selected.</summary>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include light objects</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c>
    ///    Include grip objects</param>
    /// <param name="includeReferences">(bool) Optional, default value: <c>false</c>
    ///    Include reference objects such as work session objects</param>
    /// <returns>(Guid ResizeArray) identifiers of the hidden objects.</returns>
    static member HiddenObjects(    [<OPT;DEF(false)>]includeLights:bool,
                                    [<OPT;DEF(false)>]includeGrips:bool,
                                    [<OPT;DEF(false)>]includeReferences:bool) : Guid ResizeArray =
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.ActiveObjects <- true
        settings.NormalObjects <- true
        settings.LockedObjects <- true
        settings.HiddenObjects <- true
        settings.IncludeLights <- includeLights
        settings.IncludeGrips <- includeGrips
        settings.ReferenceObjects <- includeReferences
        let rc = ResizeArray()
        for i in State.Doc.Objects.GetObjectList(settings) do
            if i.IsHidden || not <| (State.Doc.Layers.[i.Attributes.LayerIndex]).IsVisible then
                rc.Add(i.Id)
        rc

    /// <summary>Inverts the current object selection. The identifiers of the newly
    ///    selected objects are returned.</summary>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include light objects. If omitted, light objects are not returned</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c>
    ///    Include grips objects. If omitted, grips objects are not returned</param>
    /// <param name="includeReferences">(bool) Optional, default value: <c>false</c>
    ///    Include reference objects such as work session objects</param>
    /// <returns>(Guid ResizeArray) identifiers of the newly selected objects.</returns>
    static member InvertSelectedObjects([<OPT;DEF(false)>]includeLights:bool,
                                        [<OPT;DEF(false)>]includeGrips:bool,
                                        [<OPT;DEF(false)>]includeReferences:bool) : Guid ResizeArray =
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.IncludeLights <- includeLights
        settings.IncludeGrips <- includeGrips
        settings.IncludePhantoms <- true
        settings.ReferenceObjects <- includeReferences
        let rhobjs = State.Doc.Objects.GetObjectList(settings)
        let rc = ResizeArray()
        for obj in rhobjs do
            if obj.IsSelected(false) <> 0 && obj.IsSelectable() then
                rc.Add(obj.Id)
                obj.Select(true) |> ignore<int> // TODO needs sync ? apparently not needed!
            else
                obj.Select(false) |> ignore<int>
        State.Doc.Views.Redraw()
        rc


    /// <summary>Returns identifiers of the objects that were most recently created or changed
    ///    by scripting a Rhino command using the Command function. It is important to
    ///    call this function immediately after calling the Command function as only the
    ///    most recently created or changed object identifiers will be returned.</summary>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the object. If omitted, the object is not selected</param>
    /// <returns>(Guid ResizeArray) identifiers of the most recently created or changed objects.</returns>
    static member LastCreatedObjects([<OPT;DEF(false)>]select:bool) : Guid ResizeArray =
        match State.CommandSerialNumbers with
        |None -> ResizeArray()
        |Some (serialnum, ende) ->
            let mutable serialnumber = serialnum
            let rc = ResizeArray()
            while serialnumber < ende do
                let obj = State.Doc.Objects.Find(serialnumber)
                if notNull obj && not <| obj.IsDeleted then
                    rc.Add(obj.Id)
                if select then obj.Select(true) |> ignore<int> // TODO needs sync ? apparently not needed!
                serialnumber <- serialnumber + 1u
                if select && rc.Count > 1 then State.Doc.Views.Redraw()
            rc


    /// <summary>Returns the identifier of the last object in the document. The last object
    ///    in the document is the first object created by the user.</summary>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the object</param>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include lights in the potential set</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c>
    ///    Include grips in the potential set</param>
    /// <returns>(Guid) identifier of the object.</returns>
    static member LastObject( [<OPT;DEF(false)>]select:bool,
                              [<OPT;DEF(false)>]includeLights:bool,
                              [<OPT;DEF(false)>]includeGrips:bool) : Guid =
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.IncludeLights <- includeLights
        settings.IncludeGrips <- includeGrips
        settings.DeletedObjects <- false
        let rhobjs = State.Doc.Objects.GetObjectList(settings)
        if isNull rhobjs || Seq.isEmpty rhobjs then
            RhinoScriptingException.Raise "RhinoScriptSyntax.LastObject failed.  select:'%A' includeLights:'%A' includeGrips:'%A'" select includeLights includeGrips
        let firstobj = Seq.last rhobjs
        if isNull firstobj then
            RhinoScriptingException.Raise "RhinoScriptSyntax.LastObject failed.  select:'%A' includeLights:'%A' includeGrips:'%A'" select includeLights includeGrips
        if select then
            firstobj.Select(true) |> ignore<int> // TODO needs sync ? apparently not needed!
            State.Doc.Views.Redraw()
        firstobj.Id


    /// <summary>Returns the identifier of the next object in the document.</summary>
    /// <param name="objectId">(Guid) The identifier of the object from which to get the next object</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the object</param>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include lights in the potential set</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c>
    ///    Include grips in the potential set</param>
    /// <returns>(Guid) identifier of the object.</returns>
    static member NextObject( objectId:Guid,
                              [<OPT;DEF(false)>]select:bool,
                              [<OPT;DEF(false)>]includeLights:bool,
                              [<OPT;DEF(false)>]includeGrips:bool) : Guid =
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.IncludeLights <- includeLights
        settings.IncludeGrips <- includeGrips
        settings.DeletedObjects <- false
        State.Doc.Objects.GetObjectList(settings)
        |> Seq.skipWhile (fun obj -> obj.Id <> objectId)
        |> Seq.skip 1
        |> Seq.tryHead
        |> Option.defaultWith ( fun () -> RhinoScriptingException.Raise "RhinoScriptSyntax.NextObject not found for %A" (Pretty.str objectId))
        |> fun obj ->
            if select then
                obj.Select(true) |> ignore<int> // TODO needs sync ? apparently not needed!
                State.Doc.Views.Redraw()
            obj.Id



    /// <summary>Returns identifiers of all normal objects in the document. Normal objects
    ///    are visible, can be snapped to, and are independent of selection state.</summary>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include light objects. If omitted, light objects are not returned</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c>
    ///    Include grips objects. If omitted, grips objects are not returned</param>
    /// <returns>(Guid ResizeArray) identifier of normal objects.</returns>
    static member NormalObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid ResizeArray =
        let iter = DocObjects.ObjectEnumeratorSettings()
        iter.NormalObjects <- true
        iter.LockedObjects <- false
        iter.IncludeLights <- includeLights
        iter.IncludeGrips <- includeGrips
        State.Doc.Objects.GetObjectList(iter)
        |> RArr.mapSeq _.Id



    /// <summary>Returns identifiers of all objects based on color.</summary>
    /// <param name="color">(Drawing.Color) Color to get objects by</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the objects</param>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include lights in the set</param>
    /// <returns>(Guid ResizeArray) identifiers of objects of the selected color.</returns>
    static member ObjectsByColor( color:Drawing.Color,
                                  [<OPT;DEF(false)>]select:bool,
                                  [<OPT;DEF(false)>]includeLights:bool) : Guid ResizeArray =
        let rhinoobjects = State.Doc.Objects.FindByDrawColor(color, includeLights)
        if select then
            for obj in rhinoobjects do obj.Select(true)|> ignore<int> // TODO needs sync ? apparently not needed!
            State.Doc.Views.Redraw()
        rhinoobjects |> RArr.mapSeq _.Id


    /// <summary>Returns identifiers of all objects based on the objects' group name.</summary>
    /// <param name="groupName">(string) Name of the group</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the objects</param>
    /// <returns>(Guid ResizeArray) identifiers for objects in the group.</returns>
    static member ObjectsByGroup(groupName:string, [<OPT;DEF(false)>]select:bool) : Guid ResizeArray =
        let groupinstance = State.Doc.Groups.FindName(groupName)
        if isNull groupinstance then
            RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectsByGroup: '%s' does not exist in GroupTable" groupName
        let rhinoobjects = State.Doc.Groups.GroupMembers(groupinstance.Index)
        if isNull rhinoobjects then
            ResizeArray()
        else
            if select then
                for obj in rhinoobjects do obj.Select(true) |> ignore<int> // TODO needs sync ? apparently not needed!
                State.Doc.Views.Redraw()
            rhinoobjects  |> RArr.mapArr _.Id


    /// <summary>Returns identifiers of all objects based on the objects' layer name.</summary>
    /// <param name="layerName">(string) Name of the layer</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the objects</param>
    /// <returns>(Guid ResizeArray) identifiers for objects in the specified layer.</returns>
    static member ObjectsByLayer(layerName:string, [<OPT;DEF(false)>]select:bool) : Guid ResizeArray =
        let layer = RhinoScriptSyntax.CoerceLayer(layerName)
        let rhinoobjects = State.Doc.Objects.FindByLayer(layer)
        if isNull rhinoobjects then ResizeArray()
        else
            if select then
                for rhobj in rhinoobjects do rhobj.Select(true) |> ignore<int> // TODO needs sync ? apparently not needed!
                State.Doc.Views.Redraw()
            rhinoobjects  |> RArr.mapArr _.Id



    /// <summary>Returns identifiers of all objects based on user-assigned name.</summary>
    /// <param name="name">(string) Name of the object or objects</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the objects</param>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include light objects</param>
    /// <param name="includeReferences">(bool) Optional, default value: <c>false</c>
    ///    Include reference objects such as work session objects</param>
    /// <returns>(Guid ResizeArray) identifiers for objects with the specified name.</returns>
    static member ObjectsByName( name:string,
                                 [<OPT;DEF(false)>]select:bool,
                                 [<OPT;DEF(false)>]includeLights:bool,
                                 [<OPT;DEF(false)>]includeReferences:bool) : Guid ResizeArray =
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.HiddenObjects <- true
        settings.DeletedObjects <- false
        settings.IncludeGrips <- false
        settings.IncludePhantoms <- true
        settings.IncludeLights <- includeLights
        settings.NameFilter <- name
        settings.ReferenceObjects <- includeReferences
        let objects = State.Doc.Objects.GetObjectList(settings)
        let ids = objects  |> RArr.mapSeq _.Id
        if ids.Count>0 && select then
            for rhobj in objects do rhobj.Select(true) |> ignore<int> // TODO needs sync ? apparently not needed!
            State.Doc.Views.Redraw()
        ids

    /// <summary>Returns identifiers of all objects based on the objects' geometry type.</summary>
    /// <param name="geometryType">(int) The type(s) of geometry objects (points, Curves, Surfaces,
    ///    Meshes, etc.) that can be selected. Object types can be
    ///    added together as bit-coded flags to filter several different kinds of geometry.
    ///      Value        Description
    ///        0           All objects
    ///        1           Point
    ///        2           Point cloud
    ///        4           Curve
    ///        8           Surface or single-face brep
    ///        16          Polysurface or multiple-face
    ///        32          Mesh
    ///        256         Light
    ///        512         Annotation
    ///        4096        Instance or block reference
    ///        8192        Text dot object
    ///        16384       Grip object
    ///        32768       Detail
    ///        65536       Hatch
    ///        131072      Morph control
    ///        262144      SubD
    ///        134217728   Cage
    ///        268435456   Phantom
    ///        536870912   Clipping Plane
    ///        1073741824  Extrusion</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the objects</param>
    /// <param name="state">(int) Optional, default value: <c>0</c>
    ///    The object state (normal, locked, and hidden). Object states can be
    ///    added together to filter several different states of geometry.
    ///      Value     Description
    ///      0         All objects
    ///      1         Normal objects
    ///      2         Locked objects
    ///      4         Hidden objects</param>
    /// <returns>(Guid ResizeArray) identifiers of object that fit the specified type(s).</returns>
    static member ObjectsByType( geometryType:int,
                                 [<OPT;DEF(false)>]select:bool,
                                 [<OPT;DEF(0)>]state:int) : Guid ResizeArray =
        let mutable state = state
        if state = 0 then state <- 7
        let mutable bSurface = false
        let mutable bPolySurface = false
        let mutable bLights = false
        let mutable bGrips = false
        let mutable bPhantoms = false
        let mutable geometryfilter = ObjectFilterEnum.GetFilterEnum(geometryType)
        if geometryType = 0 then geometryfilter <- DocObjects.ObjectType.AnyObject
        if DocObjects.ObjectType.None <>(geometryfilter &&& DocObjects.ObjectType.Surface) then bSurface <- true // TODO verify this works OK !
        if DocObjects.ObjectType.None <>(geometryfilter &&& DocObjects.ObjectType.Brep ) then bPolySurface <- true
        if DocObjects.ObjectType.None <>(geometryfilter &&& DocObjects.ObjectType.Light ) then bLights <- true
        if DocObjects.ObjectType.None <>(geometryfilter &&& DocObjects.ObjectType.Grip ) then bGrips <- true
        if DocObjects.ObjectType.None <>(geometryfilter &&& DocObjects.ObjectType.Phantom ) then bPhantoms <- true
        let it = DocObjects.ObjectEnumeratorSettings()
        it.DeletedObjects <- false
        it.ActiveObjects <- true
        it.ReferenceObjects <- true
        it.IncludeLights <- bLights
        it.IncludeGrips <- bGrips
        it.IncludePhantoms <- bPhantoms
        if 0 <> state then
            it.NormalObjects <- false
            it.LockedObjects <- false
        if (state &&& 1) <> 0 then it.NormalObjects <- true
        if (state &&& 2) <> 0 then it.LockedObjects <- true
        if (state &&& 4) <> 0 then it.HiddenObjects <- true
        let objectIds = ResizeArray()
        let e = State.Doc.Objects.GetObjectList(it)
        for object in e do
            let  mutable bFound = false
            let objecttyp = object.ObjectType
            if objecttyp = DocObjects.ObjectType.Brep && (bSurface || bPolySurface) then
                let brep = RhinoScriptSyntax.CoerceBrep(object.Id)
                if notNull brep then
                    if brep.Faces.Count = 1 then
                        if bSurface then bFound <- true
                    else
                        if bPolySurface then bFound <- true
            elif objecttyp = DocObjects.ObjectType.Extrusion && (bSurface || bPolySurface) then
                let extrusion = object.Geometry :?> Extrusion
                let profilecount = extrusion.ProfileCount
                let capcount = extrusion.CapCount
                if profilecount = 1 && capcount = 0 && bSurface then
                    bFound <- true
                elif profilecount>0 && capcount>0 && bPolySurface then
                    bFound <- true
            elif objecttyp &&& geometryfilter <> DocObjects.ObjectType.None then
                bFound <- true
            if bFound then
                if select then object.Select(true) |> ignore<int> // TODO needs sync ? apparently not needed!
                objectIds.Add(object.Id)
        if objectIds.Count > 0 && select then State.Doc.Views.Redraw()
        objectIds


    /// <summary>Returns the identifiers of all objects that are currently selected.</summary>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include light objects</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c>
    ///    Include grip objects</param>
    /// <returns>(Guid ResizeArray) identifiers of selected objects.</returns>
    static member SelectedObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid ResizeArray =
        State.Doc.Objects.GetSelectedObjects(includeLights, includeGrips)
        |> RArr.mapSeq _.Id


    /// <summary>Unselects all objects in the document.</summary>
    /// <returns>(int) The number of objects that were unselected.</returns>
    static member UnselectAllObjects() : int =
        let rc = State.Doc.Objects.UnselectAll()
        if rc>0 then State.Doc.Views.Redraw()
        rc


    /// <summary>Return identifiers of all objects that are visible in a specified view.
    /// This function is the same as rs.VisibleObjects in Rhino Python.
    /// use rs.ShownObjects to get all objects that are not hidden or on turned-off layers. .</summary>
    /// <param name="view">(string) Optional, The view to use. If omitted, the current active view is used</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select the objects</param>
    /// <param name="includeLights">(bool) Optional, default value: <c>false</c>
    ///    Include light objects</param>
    /// <param name="includeGrips">(bool) Optional, default value: <c>false</c>
    ///    Include grip objects</param>
    /// <returns>(Guid ResizeArray) identifiers of the visible objects.</returns>
    static member VisibleObjectsInView(   [<OPT;DEF(null:string)>]view:string,
                                          [<OPT;DEF(false)>]select:bool,
                                          [<OPT;DEF(false)>]includeLights:bool,
                                          [<OPT;DEF(false)>]includeGrips:bool) : Guid ResizeArray =
        let get () =
            let it = DocObjects.ObjectEnumeratorSettings()
            it.DeletedObjects <- false
            it.ActiveObjects <- true
            it.ReferenceObjects <- true
            it.IncludeLights <- includeLights
            it.IncludeGrips <- includeGrips
            it.VisibleFilter <- true
            let viewport = if notNull view then (RhinoScriptSyntax.CoerceView(view)).MainViewport else State.Doc.Views.ActiveView.MainViewport
            it.ViewportFilter <- viewport
            let objectIds = ResizeArray()
            let e = State.Doc.Objects.GetObjectList(it)
            for object in e do
                let bbox = object.Geometry.GetBoundingBox(true)
                if viewport.IsVisible(bbox) then
                    if select then object.Select(true) |> ignore<int>
                    objectIds.Add(object.Id)
            if objectIds.Count>0 && select then State.Doc.Views.Redraw()
            objectIds
        RhinoSync.DoSync get


    /// <summary>Picks objects using either a window or crossing selection.</summary>
    /// <param name="corner1">(Point3d) Corner1 of selection window</param>
    /// <param name="corner2">(Point3d) Corner2 of selection window</param>
    /// <param name="view">(string) Optional, View to perform the selection in</param>
    /// <param name="select">(bool) Optional, default value: <c>false</c>
    ///    Select picked objects</param>
    /// <param name="inWindow">(bool) Optional, default value: <c>true</c>
    ///    If False, then a crossing window selection is performed</param>
    /// <returns>(Guid ResizeArray) identifiers of selected objects.</returns>
    static member WindowPick( corner1:Point3d,
                              corner2:Point3d,
                              [<OPT;DEF(null:string)>]view:string,
                              [<OPT;DEF(false)>]select:bool,
                              [<OPT;DEF(true)>]inWindow:bool) : Guid ResizeArray =

        let pick () =
            let view = if notNull view then RhinoScriptSyntax.CoerceView(view) else State.Doc.Views.ActiveView
            let viewport = view.MainViewport
            let screen1 = Point2d(corner1)
            let screen2 = Point2d(corner2)
            let xf = viewport.GetTransform(DocObjects.CoordinateSystem.World, DocObjects.CoordinateSystem.Screen)
            screen1.Transform(xf)
            screen2.Transform(xf)


            let objects =
                // updated from https://github.com/mcneel/rhinoscriptsyntax/pull/185
                let pc = new Input.Custom.PickContext()
                pc.View <- view
                pc.PickStyle <- if inWindow then Input.Custom.PickStyle.WindowPick else Input.Custom.PickStyle.CrossingPick
                pc.PickGroupsEnabled <- if inWindow then true else false
                let _, frustumLine = viewport.GetFrustumLine((screen1.X + screen2.X) / 2.0, (screen1.Y + screen2.Y) / 2.0)
                pc.PickLine <- frustumLine

                let leftX = min screen1.X  screen2.X   |> round |> int
                let topY =  min screen1.Y screen2.Y    |> round |> int
                let w    =  abs(screen1.X - screen2.X) |> round |> int
                let h    =  abs(screen1.Y - screen2.Y) |> round |> int
                let rect = Drawing.Rectangle(leftX, topY, w, h)

                pc.SetPickTransform(viewport.GetPickTransform(rect))
                pc.UpdateClippingPlanes()

                State.Doc.Objects.PickObjects(pc)

            let rc = ResizeArray()
            if notNull objects then
                let rc = ResizeArray()
                for rhobjr in objects do
                    let rhobj = rhobjr.Object()
                    rc.Add(rhobj.Id)
                    if select then rhobj.Select(true) |> ignore<int>
                if select then State.Doc.Views.Redraw()
            rc
        RhinoSync.DoSyncRedrawHideEditor pick



