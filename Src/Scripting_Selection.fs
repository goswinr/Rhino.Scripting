
namespace Rhino

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.FSharp.Core.LanguagePrimitives

open Rhino.Geometry
open Rhino.ApplicationSettings

open FsEx
open FsEx.UtilMath
open FsEx.SaveIgnore
open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenSelection =
  type Scripting with  
    //---The members below are in this file only for developemnt. This brings acceptable tooling performance (e.g. autocomplete) 
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file. 
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file. 
    //---End of header marker: don't chnage: {@$%^&*()*&^%$@}



    ///<summary>Returns identifiers of all objects in the document.</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c> Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c> Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c> Include grips objects</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c> Include refrence objects such as work session objects</param>
    ///<returns>(Guid Rarr) Identifiers for all the objects in the document.</returns>
    static member AllObjects(  [<OPT;DEF(false)>]select:bool,
                               [<OPT;DEF(false)>]includeLights:bool,
                               [<OPT;DEF(false)>]includeGrips:bool,
                               [<OPT;DEF(false)>]includeReferences:bool) : Guid Rarr = 
            let it = DocObjects.ObjectEnumeratorSettings()
            it.IncludeLights <- includeLights
            it.IncludeGrips <- includeGrips
            it.NormalObjects <- true
            it.LockedObjects <- true
            it.HiddenObjects <- true
            it.ReferenceObjects <- includeReferences
            let es = State.Doc.Objects.GetObjectList(it)
            let objectIds = Rarr()
            for ob in es do
                objectIds.Add ob.Id
                if select then ob.Select(true) |> ignore   //TODO make sync ?
            if objectIds.Count > 0 && select then State.Doc.Views.Redraw()
            objectIds

    ///<summary>Returns identifiers of all objects in the current model or paper space that are not hidden or on turned off layers.</summary>
    ///<param name="filter">(int) Optional, Default Value: <c>0</c>
    ///    The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the Scripting.Filter enum to get values, they can be joinded with '+'</param>
    ///<param name="printCount">(bool) Optional, Default Value: <c>true</c> Print object count to command window</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c> Include refrence objects such as work session objects</param>
    ///<param name="includeLockedObjects">(bool) Optional, Default Value: <c>true</c> Include locked objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c> Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c> Include grips objects</param>
    ///<returns>(Guid Rarr) Identifiers for all the objects that are not hidden and who's layer is on and visible.</returns>
    static member ShownObjects(     [<OPT;DEF(0)>]filter:int,
                                    [<OPT;DEF(true)>]printCount:bool,
                                    [<OPT;DEF(false)>]includeReferences:bool,
                                    [<OPT;DEF(true)>] includeLockedObjects:bool,
                                    [<OPT;DEF(false)>]includeLights:bool,
                                    [<OPT;DEF(false)>]includeGrips:bool) : Guid Rarr = 
            let viewId = // only get object from model space if current or current page
                if State.Doc.Views.ActiveView :? Display.RhinoPageView then State.Doc.Views.ActiveView.MainViewport.Id
                else Guid.Empty
            let Vis = new Collections.Generic.HashSet<int>()
            for layer in State.Doc.Layers do
                if not layer.IsDeleted && layer.IsVisible then
                    Vis.Add(layer.Index) |> ignore
            let it = DocObjects.ObjectEnumeratorSettings()
            it.IncludeLights <- includeLights //TODO check what happens to layout objects !!! included ?
            it.IncludeGrips <- includeGrips
            it.NormalObjects <- true
            it.LockedObjects <- includeLockedObjects
            it.HiddenObjects <- false
            it.ReferenceObjects <- includeReferences
            it.ObjectTypeFilter <- ObjectFilterEnum.GetFilterEnum (filter)
            it.DeletedObjects <- false
            //it.VisibleFilter <- true
            let objects = State.Doc.Objects.GetObjectList(it)
            let objectIds = Rarr()
            for ob in objects do
                if ob.Attributes.ViewportId = viewId then // only get object from model space if current or current page
                    if Vis.Contains(ob.Attributes.LayerIndex) then
                         objectIds.Add(ob.Id)
            if printCount then
                Scripting.PrintfnBlue "ShownObjects found %s"  (Scripting.ObjectDescription(objectIds))
            objectIds


    ///<summary>Returns identifier of the first object in the document. The first
    ///    object is the last object created by the user.</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c> Select the object. If omitted, the object is not selected</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c> Include light objects. If omitted, light objects are not returned</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c> Include grips objects. If omitted, grips objects are not returned</param>
    ///<returns>(Guid) The identifier of the object.</returns>
    static member FirstObject(      [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(false)>]includeLights:bool,
                                    [<OPT;DEF(false)>]includeGrips:bool) : Guid = 
            let it = DocObjects.ObjectEnumeratorSettings()
            it.IncludeLights <- includeLights
            it.IncludeGrips <- includeGrips
            let e = State.Doc.Objects.GetObjectList(it).GetEnumerator()
            if not <| e.MoveNext() then RhinoScriptingException.Raise "Rhino.Scripting.FirstObject not found"
            let object = e.Current
            if isNull object then RhinoScriptingException.Raise "Rhino.Scripting.FirstObject not found(null)"
            if select then object.Select(true) |> ignore //TODO make sync ?
            object.Id




    ///<summary>Prompts user to pick or select a single Curve object.</summary>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>true</c> Allow for the selection of pre-selected objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c> Select the picked objects. If False, objects that are picked are not selected</param>
    ///<returns>(Guid * bool * int * Point3d * float * string) Tuple containing the following information
    ///    [0]  guid     identifier of the Curve object
    ///    [1]  bool     True if the Curve was preselected, otherwise False
    ///    [2]  Enum     DocObjects.SelectionMethod
    ///    [3]  point    selection point
    ///    [4]  number   the Curve parameter of the selection point
    ///    [5]  str      name of the view selection was made.</returns>
    static member GetCurveObject(   [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool) : Guid * bool * DocObjects.SelectionMethod * Point3d * float * string = 
        let get () =  // TODO Add check if already hidden, then dont even hide and show
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore
                State.Doc.Views.Redraw()
            let go = new Input.Custom.GetObject()
            if notNull message then go.SetCommandPrompt(message)
            go.GeometryFilter <- DocObjects.ObjectType.Curve
            go.SubObjectSelect <- false
            go.GroupSelect <- false
            go.AcceptNothing(true)
            let res = go.Get()
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in rs.GetCurveObject(message=%A), Interaction result: %A" message res
            else
                let objref = go.Object(0)
                let objectId = objref.ObjectId
                let presel = go.ObjectsWerePreselected
                let selmethod = objref.SelectionMethod()
                let point = objref.SelectionPoint()
                let crv, curveparameter = objref.CurveParameter()
                let viewname = go.View().ActiveViewport.Name
                let obj = go.Object(0).Object()
                go.Dispose()
                if not <| select && not <| preselect then
                    State.Doc.Objects.UnselectAll()|> ignore
                    State.Doc.Views.Redraw()
                obj.Select(select)  |> ignore
                (objectId, presel, selmethod, point, curveparameter, viewname)
            |>! fun _ -> if notNull RhinoSync.SeffWindow then RhinoSync.SeffWindow.Show()
        RhinoSync.DoSyncRedrawHideEditor get


    ///<summary>Prompts user to pick, or select, a single object.
    /// Raises a RhinoUserInteractionException if no object was selcted. For example when Esc key was pressed.</summary>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="filter">(int) Optional, The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the Scripting.Filter enum to get values, they can be joinded with '+'</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    ///<param name="customFilter">(Input.Custom.GetObjectGeometryFilter) Optional, A custom filter function</param>
    ///<param name="subObjects">(bool) Optional, Default Value: <c>false</c>
    ///    If True, subobjects can be selected. When this is the
    ///    case, for tracking  of the subobject go via the Object Ref</param>
    ///<returns>(Guid) Identifier of the picked object.</returns>
    static member GetObject(        [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(0)>]filter:int,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(null:Input.Custom.GetObjectGeometryFilter)>]customFilter:Input.Custom.GetObjectGeometryFilter,
                                    [<OPT;DEF(false)>]subObjects:bool) : Guid = 
        let get () = 
            if not  preselect then
                State.Doc.Objects.UnselectAll() |> ignore
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
                RhinoUserInteractionException.Raise "No Object was selected in rs.GetObject(message=%A), Interaction result: %A" message res
            else
                let objref = go.Object(0)
                let obj = objref.Object()
                //let presel = go.ObjectsWerePreselected
                go.Dispose()
                //if not <| select && not <| preselect then State.Doc.Objects.UnselectAll() |> ignore  State.Doc.Views.Redraw()
                if select then
                    obj.Select(select)  |> ignore
                else
                    State.Doc.Objects.UnselectAll() |> ignore
                State.Doc.Views.Redraw()
                obj.Id
        RhinoSync.DoSyncRedrawHideEditor get


    ///<summary>Prompts user to pick, or select a single object.
    /// Raises a RhinoUserInteractionException if no object was selcted. For example when Esc key was pressed.</summary>
    ///<param name="message">(string) Optional, A prompt or message</param>
    ///<param name="filter">(int) Optional, Default Value: <c>0</c>
    ///    The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the filter class to get values</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    ///<param name="objects">(Guid seq) Optional, List of object identifiers specifying objects that are
    ///    allowed to be selected</param>
    ///<returns>(Guid * bool * float * Point3d * string) Tuple containing the following information
    ///    [0] identifier of the object
    ///    [1] True if the object was preselected, otherwise False
    ///    [2] selection method Enum DocObjects.SelectionMethod
    ///         (0) selected by non-mouse method (SelAll, etc.).
    ///         (1) selected by mouse click on theobject.
    ///         (2) selected by being inside of amouse window.
    ///         (3) selected by intersecting a mousecrossing window.
    ///    [3] selection point
    ///    [4] name of the view selection was made.</returns>
    static member GetObjectEx(      [<OPT;DEF(null:string)>]message:string,
                                    [<OPT;DEF(0)>]filter:int,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(null:Guid seq)>]objects:Guid seq) : Guid * bool * DocObjects.SelectionMethod * Point3d * string = 
        let get () = 
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore
                State.Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull objects then
                let s = System.Collections.Generic.HashSet(objects)
                go.SetCustomGeometryFilter(fun rhinoobject _ _ -> s.Contains(rhinoobject.Id))
            if notNull message then
                go.SetCommandPrompt(message)
            if filter>0 then
                go.GeometryFilter <- ObjectFilterEnum.GetFilterEnum(filter)
            go.SubObjectSelect <- false
            go.GroupSelect <- false
            go.AcceptNothing(true)
            let res = go.Get()
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in rs.GetObjectEx(message=%A), Interaction result: %A" message res
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
                    State.Doc.Objects.UnselectAll() |> ignore
                    State.Doc.Views.Redraw()
                obj.Select(select) |> ignore
                (objectId, presel, selmethod, point, viewname)
            |>! fun _ -> if notNull RhinoSync.SeffWindow then RhinoSync.SeffWindow.Show()
        RhinoSync.DoSyncRedrawHideEditor get


    ///<summary>Prompts user to pick or select one or more objects.
    /// Raises a RhinoUserInteractionException if no object was selcted. For example when Esc key was pressed.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Select objects"</c>
    ///    A prompt or message</param>
    ///<param name="filter">(int) Optional, The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the Scripting.Filter enum to get values, they can be joinded with '+'</param>
    ///<param name="group">(bool) Optional, Default Value: <c>true</c>
    ///    Honor object grouping. If omitted and the user picks a group,
    ///    the entire group will be picked (True). Note, if filter is set to a
    ///    value other than 0 (All objects), then group selection will be disabled</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    ///<param name="objectsToSelectFrom">(Guid seq) Optional, List of objects that are allowed to be selected. If set customFilter will be ignored</param>
    ///<param name="minimumCount">(int) Optional, Default Value: <c>1</c>
    ///    Minimum count of objects allowed to be selected</param>
    ///<param name="maximumCount">(int) Optional, Default Value: <c>0</c>
    ///    Maximum count of objects allowed to be selected</param>
    ///<param name="printCount">(bool) Optional, Default Value: <c>true</c> Print object count to command window</param>
    ///<param name="customFilter">(Input.Custom.GetObjectGeometryFilter) Optional, Will be ignored if 'objects' are set. Calls a custom function in the script and passes
    ///    the Rhino Object, Geometry, and component index and returns true or false indicating if the object can be selected</param>
    ///<returns>(Guid Rarr) List of identifiers of the picked objects.</returns>
    static member GetObjects(       [<OPT;DEF("Select objects")>]message:string,
                                    [<OPT;DEF(0)>]filter:int,
                                    [<OPT;DEF(true)>]group:bool,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(null:Guid seq)>]objectsToSelectFrom:Guid seq,
                                    [<OPT;DEF(1)>]minimumCount:int,
                                    [<OPT;DEF(0)>]maximumCount:int,
                                    [<OPT;DEF(true)>]printCount:bool,
                                    [<OPT;DEF(null:Input.Custom.GetObjectGeometryFilter)>]customFilter:Input.Custom.GetObjectGeometryFilter)  : Rarr<Guid> = 
        let get () = 
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore
                State.Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull objectsToSelectFrom then
                let s = System.Collections.Generic.HashSet(objectsToSelectFrom)
                go.SetCustomGeometryFilter(fun rhinoobject _ _ -> s.Contains(rhinoobject.Id))
            elif notNull customFilter then
                go.SetCustomGeometryFilter(customFilter)
            go.SetCommandPrompt(message )
            let geometryfilter = ObjectFilterEnum.GetFilterEnum(filter)
            if filter>0 then go.GeometryFilter <- geometryfilter
            go.SubObjectSelect <- false
            go.GroupSelect <- group
            go.AcceptNothing(true)
            let res = go.GetMultiple(minimumCount, maximumCount)
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in rs.GetObjects(message=%A), Interaction result: %A" message res
            else
                if not <| select && not <| go.ObjectsWerePreselected then
                    State.Doc.Objects.UnselectAll() |> ignore
                    State.Doc.Views.Redraw()
                let rc = Rarr()
                let count = go.ObjectCount
                for i = 0 to count - 1 do
                    let objref = go.Object(i)
                    rc.Add(objref.ObjectId)
                    let obj = objref.Object()
                    if select && notNull obj then obj.Select(select) |> ignore
                if printCount then Scripting.PrintfnBlue "GetObjects got %s" (Scripting.ObjectDescription(rc))
                rc
            |>! fun _ -> if notNull RhinoSync.SeffWindow then RhinoSync.SeffWindow.Show()
        RhinoSync.DoSyncRedrawHideEditor get


    ///<summary>Returns the same objects as in the last user interaction with the same prompt message
    /// If none found, Prompts user to pick or select one or more objects and remembers them.</summary>
    ///<param name="message">(string) A prompt or message, should be unique, this will be the key in dictionary to remeber objects</param>
    ///<param name="filter">(int) Optional, The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the Scripting.Filter enum to get values, they can be joinded with '+'</param>
    ///<param name="group">(bool) Optional, Default Value: <c>true</c>
    ///    Honor object grouping. If omitted and the user picks a group,
    ///    the entire group will be picked (True). Note, if filter is set to a
    ///    value other than 0 (All objects), then group selection will be disabled</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    ///<param name="objects">(Guid seq) Optional, List of objects that are allowed to be selected. If set customFilter will be ignored</param>
    ///<param name="minimumCount">(int) Optional, Default Value: <c>1</c>
    ///    Minimum count of objects allowed to be selected</param>
    ///<param name="maximumCount">(int) Optional, Default Value: <c>0</c>
    ///    Maximum count of objects allowed to be selected</param>
    ///<param name="printCount">(bool) Optional, Default Value: <c>true</c> Print object count to command window</param>
    ///<param name="customFilter">(Input.Custom.GetObjectGeometryFilter) Optional, Will be ignored if 'objects' are set. Calls a custom function in the script and passes
    ///    the Rhino Object, Geometry, and component index and returns true or false indicating if the object can be selected</param>
    ///<returns>(Guid Rarr) List of identifiers of the picked objects.</returns>
    static member GetObjectsAndRemember(message:string,
                                        [<OPT;DEF(0)>]filter:int,
                                        [<OPT;DEF(true)>]group:bool,
                                        [<OPT;DEF(true)>]preselect:bool,
                                        [<OPT;DEF(false)>]select:bool,
                                        [<OPT;DEF(null:Guid seq)>]objects:Guid seq,
                                        [<OPT;DEF(1)>]minimumCount:int,
                                        [<OPT;DEF(0)>]maximumCount:int,
                                        [<OPT;DEF(true)>]printCount:bool,
                                        [<OPT;DEF(null:Input.Custom.GetObjectGeometryFilter)>]customFilter:Input.Custom.GetObjectGeometryFilter)  : Guid Rarr = 
        try
            let objectIds = Scripting.Sticky.[message] :?> Rarr<Guid>
            if printCount then  // this print statement also raises an exception if object does not exist to trigger reselection
                Scripting.PrintfnBlue "GetObjectsAndRemember for '%s': %s" message (Scripting.ObjectDescription(objectIds))
            objectIds
        with | _ ->
            let ids = Scripting.GetObjects(message, filter, group, preselect, select, objects, minimumCount, maximumCount, printCount, customFilter)
            Scripting.Sticky.[message] <- ids
            ids



    ///<summary>Returns the same object as in the last user interaction with the same prompt message
    /// If none found, Prompts user to pick one object and remembers it.</summary>
    ///<param name="message">(string) A prompt or message, should be unique, this will be the key in dictionary to remeber object</param>
    ///<param name="filter">(int) Optional, The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the Scripting.Filter enum to get values, they can be joinded with '+'</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    ///<param name="printCount">(bool) Optional, Default Value: <c>true</c> Print object count to command window</param>
    ///<param name="customFilter">(Input.Custom.GetObjectGeometryFilter) Optional, A custom filter function</param>
    ///<returns>(Guid) a identifier of the picked object.</returns>
    static member GetObjectAndRemember( message:string,
                                        [<OPT;DEF(0)>]filter:int,
                                        [<OPT;DEF(true)>]preselect:bool,
                                        [<OPT;DEF(false)>]select:bool,
                                        [<OPT;DEF(true)>]printCount:bool,
                                        [<OPT;DEF(null:Input.Custom.GetObjectGeometryFilter)>]customFilter:Input.Custom.GetObjectGeometryFilter)  : Guid = 
        try
            let objectId:Guid = Scripting.Sticky.[message] |> unbox
            if printCount then // this print statement also raises an exception if object does not exist to trigger reselection
                Scripting.PrintfnBlue "GetObjectAndRemember for '%s': one %s" message (Scripting.ObjectDescription(objectId))
            objectId
        with | _ ->
            let id = Scripting.GetObject(message, filter,  preselect, select,  customFilter, subObjects=false)
            Scripting.Sticky.[message] <- box id
            id

    ///<summary>Prompts user to pick, or select one or more objects.
    /// Raises a RhinoUserInteractionException if no object was selcted. For example when Esc key was pressed.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Select objects"</c>
    ///    A prompt or message</param>
    ///<param name="filter">(int) Optional, Default Value: <c>0</c>
    ///    The type(s) of geometry (points, Curves, Surfaces, Meshes,...)
    ///    that can be selected. Object types can be added together to filter
    ///    several different kinds of geometry. use the filter class to get values</param>
    ///<param name="group">(bool) Optional, Default Value: <c>true</c>
    ///    Honor object grouping. If omitted and the user picks a group,
    ///    the entire group will be picked (True). Note, if filter is set to a
    ///    value other than 0 (All objects), then group selection will be disabled</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>true</c>
    ///    Allow for the selection of pre-selected objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the picked objects. If False, the objects that are
    ///    picked are not selected</param>
    ///<param name="printCount">(bool) Optional, Default Value: <c>true</c> Print object count to command window</param>
    ///<param name="objectsToSelectFrom">(Guid seq) Optional, List of object identifiers specifying objects that are
    ///    allowed to be selected</param>
    ///<returns>((Guid*bool*int*Point3d*string) Rarr) List containing the following information
    ///    [n][0]  identifier of the object
    ///    [n][1]  True if the object was preselected, otherwise False
    ///    [n][2]  selection method (DocObjects.SelectionMethod)
    ///    [n][3]  selection point
    ///    [n][4]  name of the view selection was made.</returns>
    static member GetObjectsEx(     [<OPT;DEF("Select objects")>]message:string,
                                    [<OPT;DEF(0)>]filter:int,
                                    [<OPT;DEF(true)>]group:bool,
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool,
                                    [<OPT;DEF(true)>]printCount:bool,
                                    [<OPT;DEF(null:Guid seq)>]objectsToSelectFrom:Guid seq) : (Guid*bool*DocObjects.SelectionMethod*Point3d*string) Rarr = 
        let get () = 
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore
                State.Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            if notNull objectsToSelectFrom then
                let s = System.Collections.Generic.HashSet(objectsToSelectFrom)
                go.SetCustomGeometryFilter(fun rhinoobject _ _ -> s.Contains(rhinoobject.Id))
            go.SetCommandPrompt(message)
            let geometryfilter = ObjectFilterEnum.GetFilterEnum(filter)
            if filter>0 then go.GeometryFilter <- geometryfilter
            go.SubObjectSelect <- false
            go.GroupSelect <- group
            go.AcceptNothing(true)
            let res = go.GetMultiple(1, 0)
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in rs.GetObjectsEx(message=%A), Interaction result: %A" message res
            else
                if not <| select && not <| go.ObjectsWerePreselected then
                    State.Doc.Objects.UnselectAll() |> ignore
                    State.Doc.Views.Redraw()
                let rc = Rarr()
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
                    if select && notNull obj then obj.Select(select) |> ignore
                if printCount then
                    rc
                    |> Rarr.map ( fun (id, _, _, _, _) -> id )
                    |> Scripting.ObjectDescription
                    |> Scripting.PrintfnBlue "GetObjectsEx got %s"


                rc
            |>! fun _ -> if notNull RhinoSync.SeffWindow then RhinoSync.SeffWindow.Show()
        RhinoSync.DoSyncRedrawHideEditor get


    ///<summary>Prompts the user to select one or more point objects.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Select Point Objects"</c>
    ///    A prompt message</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>true</c>
    ///    Allow for the selection of pre-selected objects. If omitted, pre-selected objects are not accepted</param>
    ///<returns>(Point3d Rarr) List of 3d points.</returns>
    static member GetPointCoordinates(  [<OPT;DEF("Select Point Objects")>] message:string,
                                        [<OPT;DEF(false)>]                  preselect:bool) : Point3d Rarr = 
        let ids =  Scripting.GetObjects(message, Scripting.Filter.Point, preselect = preselect)
        let rc = Rarr<Point3d>()
        for objectId in ids do
            let pt = Scripting.Coerce3dPoint(objectId)
            rc.Add(pt)
        rc




    ///<summary>Prompts the user to select a single Surface.
    /// Raises a RhinoUserInteractionException if no object was selcted. For example when Esc key was pressed.</summary>
    ///<param name="message">(string) Optional, Default Value: <c>"Select Surface"</c>
    ///    Prompt displayed</param>
    ///<param name="preselect">(bool) Optional, Default Value: <c>true</c>
    ///    Allow for preselected objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the picked object</param>
    ///<returns>((Guid * bool * DocObjects.SelectionMethod * Point3d * (float * float) * string))
    ///    [0]  identifier of the Surface
    ///    [1]  True if the Surface was preselected, otherwise False
    ///    [2]  selection method ( DocObjects.SelectionMethod )
    ///    [3]  selection point
    ///    [4]  u, v Surface parameter of the selection point
    ///    [5]  name of the view in which the selection was made.</returns>
    static member GetSurfaceObject( [<OPT;DEF("Select surface")>]message:string, // TODO add selection method returmn value.  see help
                                    [<OPT;DEF(true)>]preselect:bool,
                                    [<OPT;DEF(false)>]select:bool) : Guid * bool * DocObjects.SelectionMethod * Point3d * (float * float) * string = 
        let get () = 
            if not <| preselect then
                State.Doc.Objects.UnselectAll() |> ignore
                State.Doc.Views.Redraw()
            use go = new Input.Custom.GetObject()
            go.SetCommandPrompt(message)
            go.GeometryFilter <- DocObjects.ObjectType.Surface
            go.SubObjectSelect <- false
            go.GroupSelect <- false
            go.AcceptNothing(true)
            let res = go.Get()
            if res <> Input.GetResult.Object then
                RhinoUserInteractionException.Raise "No Object was selected in rs.GetSurfaceObject(message=%A), Interaction result: %A" message res
            else
                let objref = go.Object(0)
                let rhobj = objref.Object()
                rhobj.Select(select) |> ignore
                State.Doc.Views.Redraw()
                let objectId = rhobj.Id
                let prepicked = go.ObjectsWerePreselected
                let selmethod = objref.SelectionMethod()
                let mutable point = objref.SelectionPoint()
                let surf, u, v = objref.SurfaceParameter()
                let mutable uv = (u, v)
                if not <| point.IsValid then
                    point <- Point3d.Unset
                    uv <- RhinoMath.UnsetValue, RhinoMath.UnsetValue
                let view = go.View()
                let name = view.ActiveViewport.Name
                go.Dispose()
                if not <| select && not <| prepicked then
                    State.Doc.Objects.UnselectAll() |> ignore
                    State.Doc.Views.Redraw()
                (objectId, prepicked, selmethod, point, uv, name)
            |>! fun _ -> if notNull RhinoSync.SeffWindow then RhinoSync.SeffWindow.Show()
        RhinoSync.DoSyncRedrawHideEditor get


    ///<summary>Returns identifiers of all locked objects in the document. Locked objects
    ///    cannot be snapped to, and cannot be selected.</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///    Include grip objects</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///    Include refrence objects such as work session objects</param>
    ///<returns>(Guid Rarr) identifiers the locked objects.</returns>
    static member LockedObjects(    [<OPT;DEF(false)>]includeLights:bool,
                                    [<OPT;DEF(false)>]includeGrips:bool,
                                    [<OPT;DEF(false)>]includeReferences:bool) : Guid Rarr = 
            let settings = DocObjects.ObjectEnumeratorSettings()
            settings.ActiveObjects <- true
            settings.NormalObjects <- true
            settings.LockedObjects <- true
            settings.HiddenObjects <- true
            settings.IncludeLights <- includeLights
            settings.IncludeGrips <- includeGrips
            settings.ReferenceObjects <- includeReferences
            rarr{
                for i in State.Doc.Objects.GetObjectList(settings) do
                    if i.IsLocked || (State.Doc.Layers.[i.Attributes.LayerIndex]).IsLocked then
                        yield i.Id }



    ///<summary>Returns identifiers of all hidden objects in the document. Hidden objects
    ///    are not visible, cannot be snapped to, and cannot be selected.</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///    Include grip objects</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///    Include refrence objects such as work session objects</param>
    ///<returns>(Guid Rarr) identifiers of the hidden objects.</returns>
    static member HiddenObjects(    [<OPT;DEF(false)>]includeLights:bool,
                                    [<OPT;DEF(false)>]includeGrips:bool,
                                    [<OPT;DEF(false)>]includeReferences:bool) : Guid Rarr = 
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.ActiveObjects <- true
        settings.NormalObjects <- true
        settings.LockedObjects <- true
        settings.HiddenObjects <- true
        settings.IncludeLights <- includeLights
        settings.IncludeGrips <- includeGrips
        settings.ReferenceObjects <- includeReferences
        rarr {for i in State.Doc.Objects.GetObjectList(settings) do
                        if i.IsHidden || not <| (State.Doc.Layers.[i.Attributes.LayerIndex]).IsVisible then
                            i.Id }


    ///<summary>Inverts the current object selection. The identifiers of the newly
    ///    selected objects are returned.</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include light objects. If omitted, light objects are not returned</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///    Include grips objects. If omitted, grips objects are not returned</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///    Include refrence objects such as work session objects</param>
    ///<returns>(Guid Rarr) identifiers of the newly selected objects.</returns>
    static member InvertSelectedObjects([<OPT;DEF(false)>]includeLights:bool,
                                        [<OPT;DEF(false)>]includeGrips:bool,
                                        [<OPT;DEF(false)>]includeReferences:bool) : Guid Rarr = 
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.IncludeLights <- includeLights
        settings.IncludeGrips <- includeGrips
        settings.IncludePhantoms <- true
        settings.ReferenceObjects <- includeReferences
        let rhobjs = State.Doc.Objects.GetObjectList(settings)
        let rc = Rarr()
        for obj in rhobjs do
            if obj.IsSelected(false) <> 0 && obj.IsSelectable() then
                rc.Add(obj.Id)
                obj.Select(true) |> ignore //TODO make sync ?
            else
                obj.Select(false) |> ignore
        State.Doc.Views.Redraw()
        rc


    ///<summary>Returns identifiers of the objects that were most recently created or changed
    ///    by scripting a Rhino command using the Command function. It is important to
    ///    call this function immediately after calling the Command function as only the
    ///    most recently created or changed object identifiers will be returned.</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the object. If omitted, the object is not selected</param>
    ///<returns>(Guid Rarr) identifiers of the most recently created or changed objects.</returns>
    static member LastCreatedObjects([<OPT;DEF(false)>]select:bool) : Guid Rarr = 
        match State.CommandSerialNumbers with
        |None -> Rarr()
        |Some (serialnum, ende) ->
            let mutable serialnumber = serialnum
            let rc = Rarr()
            while serialnumber < ende do
                let obj = State.Doc.Objects.Find(serialnumber)
                if notNull obj && not <| obj.IsDeleted then
                    rc.Add(obj.Id)
                if select then obj.Select(true) |> ignore //TODO make sync ?
                serialnumber <- serialnumber + 1u
                if select && rc.Count > 1 then State.Doc.Views.Redraw()
            rc


    ///<summary>Returns the identifier of the last object in the document. The last object
    ///    in the document is the first object created by the user.</summary>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the object</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include lights in the potential set</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///    Include grips in the potential set</param>
    ///<returns>(Guid) identifier of the object.</returns>
    static member LastObject( [<OPT;DEF(false)>]select:bool,
                              [<OPT;DEF(false)>]includeLights:bool,
                              [<OPT;DEF(false)>]includeGrips:bool) : Guid = 
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.IncludeLights <- includeLights
        settings.IncludeGrips <- includeGrips
        settings.DeletedObjects <- false
        let rhobjs = State.Doc.Objects.GetObjectList(settings)
        if isNull rhobjs || Seq.isEmpty rhobjs then
            RhinoScriptingException.Raise "Rhino.Scripting.LastObject failed.  select:'%A' includeLights:'%A' includeGrips:'%A'" select includeLights includeGrips
        let firstobj = Seq.last rhobjs
        if isNull firstobj then
            RhinoScriptingException.Raise "Rhino.Scripting.LastObject failed.  select:'%A' includeLights:'%A' includeGrips:'%A'" select includeLights includeGrips
        if select then
            firstobj.Select(true) |> ignore //TODO make sync ?
            State.Doc.Views.Redraw()
        firstobj.Id


    ///<summary>Returns the identifier of the next object in the document.</summary>
    ///<param name="objectId">(Guid) The identifier of the object from which to get the next object</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the object</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include lights in the potential set</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///    Include grips in the potential set</param>
    ///<returns>(Guid) identifier of the object.</returns>
    static member NextObject( objectId:Guid,
                              [<OPT;DEF(false)>]select:bool,
                              [<OPT;DEF(false)>]includeLights:bool,
                              [<OPT;DEF(false)>]includeGrips:bool) : Guid = 
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.IncludeLights <- includeLights
        settings.IncludeGrips <- includeGrips
        settings.DeletedObjects <- false
        State.Doc.Objects.GetObjectList(settings)
        |> Seq.thisNext
        |> Seq.skipLast // dont loop
        |> Seq.tryFind (fun (t, n) -> objectId = t.Id)
        |>  function
            |None -> RhinoScriptingException.Raise "Rhino.Scripting.NextObject not found for %A" (Print.guid objectId)
            |Some (t, n) ->
                if select then n.Select(true) |> ignore //TODO make sync ?
                n.Id


    ///<summary>Returns identifiers of all normal objects in the document. Normal objects
    ///    are visible, can be snapped to, and are independent of selection state.</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include light objects. If omitted, light objects are not returned</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///    Include grips objects. If omitted, grips objects are not returned</param>
    ///<returns>(Guid Rarr) identifier of normal objects.</returns>
    static member NormalObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid Rarr = 
        let iter = DocObjects.ObjectEnumeratorSettings()
        iter.NormalObjects <- true
        iter.LockedObjects <- false
        iter.IncludeLights <- includeLights
        iter.IncludeGrips <- includeGrips
        rarr {for obj in State.Doc.Objects.GetObjectList(iter) do yield obj.Id }


    ///<summary>Returns identifiers of all objects based on color.</summary>
    ///<param name="color">(Drawing.Color) Color to get objects by</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include lights in the set</param>
    ///<returns>(Guid Rarr) identifiers of objects of the selected color.</returns>
    static member ObjectsByColor( color:Drawing.Color,
                                  [<OPT;DEF(false)>]select:bool,
                                  [<OPT;DEF(false)>]includeLights:bool) : Guid Rarr = 
        let rhinoobjects = State.Doc.Objects.FindByDrawColor(color, includeLights)
        if select then
            for obj in rhinoobjects do obj.Select(true)|> ignore //TODO make sync ?
            State.Doc.Views.Redraw()
        rarr {for obj in rhinoobjects do yield obj.Id }


    ///<summary>Returns identifiers of all objects based on the objects' group name.</summary>
    ///<param name="groupName">(string) Name of the group</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the objects</param>
    ///<returns>(Guid Rarr) identifiers for objects in the group.</returns>
    static member ObjectsByGroup(groupName:string, [<OPT;DEF(false)>]select:bool) : Guid Rarr = 
        let groupinstance = State.Doc.Groups.FindName(groupName)
        if isNull groupinstance then
            RhinoScriptingException.Raise "Rhino.Scripting.%s does not exist in GroupTable" groupName
        let rhinoobjects = State.Doc.Groups.GroupMembers(groupinstance.Index)
        if isNull rhinoobjects then
            Rarr()
        else
            if select then
                for obj in rhinoobjects do obj.Select(true) |> ignore //TODO make sync ?
                State.Doc.Views.Redraw()
            rarr { for obj in rhinoobjects do yield obj.Id }


    ///<summary>Returns identifiers of all objects based on the objects' layer name.</summary>
    ///<param name="layerName">(string) Name of the layer</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the objects</param>
    ///<returns>(Guid Rarr) identifiers for objects in the specified layer.</returns>
    static member ObjectsByLayer(layerName:string, [<OPT;DEF(false)>]select:bool) : Guid Rarr = 
        let layer = Scripting.CoerceLayer(layerName)
        let rhinoobjects = State.Doc.Objects.FindByLayer(layer)
        if isNull rhinoobjects then Rarr()
        else
            if select then
                for rhobj in rhinoobjects do rhobj.Select(true) |> ignore //TODO make sync ?
                State.Doc.Views.Redraw()
            rarr {for obj in rhinoobjects do yield obj.Id }



    ///<summary>Returns identifiers of all objects based on user-assigned name.</summary>
    ///<param name="name">(string) Name of the object or objects</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include light objects</param>
    ///<param name="includeReferences">(bool) Optional, Default Value: <c>false</c>
    ///    Include refrence objects such as work session objects</param>
    ///<returns>(Guid Rarr) identifiers for objects with the specified name.</returns>
    static member ObjectsByName( name:string,
                                 [<OPT;DEF(false)>]select:bool,
                                 [<OPT;DEF(false)>]includeLights:bool,
                                 [<OPT;DEF(false)>]includeReferences:bool) : Guid Rarr = 
        let settings = DocObjects.ObjectEnumeratorSettings()
        settings.HiddenObjects <- true
        settings.DeletedObjects <- false
        settings.IncludeGrips <- false
        settings.IncludePhantoms <- true
        settings.IncludeLights <- includeLights
        settings.NameFilter <- name
        settings.ReferenceObjects <- includeReferences
        let objects = State.Doc.Objects.GetObjectList(settings)
        let ids = rarr{ for rhobj in objects do yield rhobj.Id }
        if ids.Count>0 && select then
            for rhobj in objects do rhobj.Select(true) |> ignore //TODO make sync ?
            State.Doc.Views.Redraw()
        ids

    ///<summary>Returns identifiers of all objects based on the objects' geometry type.</summary>
    ///<param name="geometryType">(int) The type(s) of geometry objects (points, Curves, Surfaces,
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
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the objects</param>
    ///<param name="state">(int) Optional, Default Value: <c>0</c>
    ///    The object state (normal, locked, and hidden). Object states can be
    ///    added together to filter several different states of geometry.
    ///      Value     Description
    ///      0         All objects
    ///      1         Normal objects
    ///      2         Locked objects
    ///      4         Hidden objects</param>
    ///<returns>(Guid Rarr) identifiers of object that fit the specified type(s).</returns>
    static member ObjectsByType( geometryType:int,
                                 [<OPT;DEF(false)>]select:bool,
                                 [<OPT;DEF(0)>]state:int) : Guid Rarr = 
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
        let objectIds = Rarr()
        let e = State.Doc.Objects.GetObjectList(it)
        for object in e do
            let  mutable bFound = false
            let objecttyp = object.ObjectType
            if objecttyp = DocObjects.ObjectType.Brep && (bSurface || bPolySurface) then
                let brep = Scripting.CoerceBrep(object.Id)
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
                if select then object.Select(true) |> ignore //TODO make sync ?
                objectIds.Add(object.Id)
        if objectIds.Count > 0 && select then State.Doc.Views.Redraw()
        objectIds


    ///<summary>Returns the identifiers of all objects that are currently selected.</summary>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///    Include grip objects</param>
    ///<returns>(Guid Rarr) identifiers of selected objects.</returns>
    static member SelectedObjects([<OPT;DEF(false)>]includeLights:bool, [<OPT;DEF(false)>]includeGrips:bool) : Guid Rarr = 
        let selobjects = State.Doc.Objects.GetSelectedObjects(includeLights, includeGrips)
        rarr {for obj in selobjects do obj.Id }


    ///<summary>Unselects all objects in the document.</summary>
    ///<returns>(int) The number of objects that were unselected.</returns>
    static member UnselectAllObjects() : int = 
        let rc = State.Doc.Objects.UnselectAll()
        if rc>0 then State.Doc.Views.Redraw()
        rc


    ///<summary>Return identifiers of all objects that are visible in a specified view.
    /// This function is the same as rs.VisibleObjects in Rhino Python.
    /// use rs.ShownObjects to get all objects that are not hidden or on turned-off layers. .</summary>
    ///<param name="view">(string) Optional, The view to use. If omitted, the current active view is used</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select the objects</param>
    ///<param name="includeLights">(bool) Optional, Default Value: <c>false</c>
    ///    Include light objects</param>
    ///<param name="includeGrips">(bool) Optional, Default Value: <c>false</c>
    ///    Include grip objects</param>
    ///<returns>(Guid Rarr) identifiers of the visible objects.</returns>
    static member VisibleObjectsInView(   [<OPT;DEF(null:string)>]view:string,
                                          [<OPT;DEF(false)>]select:bool,
                                          [<OPT;DEF(false)>]includeLights:bool,
                                          [<OPT;DEF(false)>]includeGrips:bool) : Guid Rarr = 
        let it = DocObjects.ObjectEnumeratorSettings()
        it.DeletedObjects <- false
        it.ActiveObjects <- true
        it.ReferenceObjects <- true
        it.IncludeLights <- includeLights
        it.IncludeGrips <- includeGrips
        it.VisibleFilter <- true
        let viewport = if notNull view then (Scripting.CoerceView(view)).MainViewport else State.Doc.Views.ActiveView.MainViewport
        it.ViewportFilter <- viewport
        let objectIds = Rarr()
        let e = State.Doc.Objects.GetObjectList(it)
        for object in e do
            let bbox = object.Geometry.GetBoundingBox(true)
            if viewport.IsVisible(bbox) then
                if select then object.Select(true) |> ignore //TODO make sync ? TEST !!!
                objectIds.Add(object.Id)
        if objectIds.Count>0 && select then State.Doc.Views.Redraw()
        objectIds


    ///<summary>Picks objects using either a window or crossing selection.</summary>
    ///<param name="corner1">(Point3d) Corner1 of selection window</param>
    ///<param name="corner2">(Point3d) Corner2 of selection window</param>
    ///<param name="view">(string) Optional, View to perform the selection in</param>
    ///<param name="select">(bool) Optional, Default Value: <c>false</c>
    ///    Select picked objects</param>
    ///<param name="inWindow">(bool) Optional, Default Value: <c>true</c>
    ///    If False, then a crossing window selection is performed</param>
    ///<returns>(Guid Rarr) identifiers of selected objects.</returns>
    static member WindowPick( corner1:Point3d,
                              corner2:Point3d,
                              [<OPT;DEF(null:string)>]view:string,
                              [<OPT;DEF(false)>]select:bool,
                              [<OPT;DEF(true)>]inWindow:bool) : Guid Rarr = 

        let pick () = 
            let view = if notNull view then Scripting.CoerceView(view) else State.Doc.Views.ActiveView
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

            let rc = Rarr()
            if notNull objects then
                let rc = Rarr()
                for rhobjr in objects do
                    let rhobj = rhobjr.Object()
                    rc.Add(rhobj.Id)
                    if select then rhobj.Select(true) |> ignore //TODO make sync ?
                if select then State.Doc.Views.Redraw()
            rc
        RhinoSync.DoSyncRedrawHideEditor pick



