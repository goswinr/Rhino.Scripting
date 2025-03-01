
namespace Rhino.Scripting

open Rhino

open System

// open FsEx
// open FsEx.UtilMath
// open FsEx.SaveIgnore
// open FsEx.CompareOperators

[<AutoOpen>]
module AutoOpenLayer =

  type RhinoScriptSyntax with
    //---The members below are in this file only for development. This brings acceptable tooling performance (e.g. autocomplete)
    //---Before compiling the script combineIntoOneFile.fsx is run to combine them all into one file.
    //---So that all members are visible in C# and Ironpython too.
    //---This happens as part of the <Targets> in the *.fsproj file.
    //---End of header marker: don't change: {@$%^&*()*&^%$@}



    ///<summary>Add a new layer to the document. If it does not exist yet.
    /// By default Ambiguous Unicode characters are not allowed in layer name. This can be changed via optional parameter.
    /// If layers or parent layers exist already color, visibility and locking parameters are ignored.</summary>
    ///<param name="name">(string) Optional, The name of the new layer. If omitted, Rhino automatically  generates the layer name.</param>
    ///<param name="color">(Drawing.Color) Optional, A Red-Green-Blue color value. If omitted a random (non yellow)  color wil be chosen.</param>
    ///<param name="visible">(int) Optional, default value: <c>2</c>
    ///   Layer visibility:
    ///   0 = explicitly Off (even if parent is already Off)
    ///   1 = On and turn parents on too
    ///   2 = inherited from parent, or On by default</param>
    ///<param name="locked">(int) Optional, default value: <c>2</c>
    ///   Layer locking state:
    ///   0 = Unlocked this and parents
    ///   1 = explicitly Locked (even if parent is already Locked)
    ///   2 = inherited from parent, or Unlocked default</param>
    ///<param name="parent">(string) Optional, Name of existing or non existing parent layer. </param>
    ///<param name="allowAllUnicode">(bool) Optional, Allow ambiguous Unicode characters too </param>
    ///<param name="collapseParents">(bool) Optional, Collapse parent layers in Layer UI </param>
    ///<returns>(int) The index in the layer table. Do Doc.Layers.[i].FullPath to get the full name of the new layer.
    /// E.g. The function rs.Add can then take this layer index.</returns>
    static member AddLayer( [<OPT;DEF(null:string)>]name:string
                          , [<OPT;DEF(Drawing.Color())>]color:Drawing.Color
                          , [<OPT;DEF(2)>]visible:int
                          , [<OPT;DEF(2)>]locked:int
                          , [<OPT;DEF(null:string)>]parent:string
                          , [<OPT;DEF(false:bool)>]allowAllUnicode:bool
                          , [<OPT;DEF(false:bool)>]collapseParents:bool) : int =

        let col = if color.IsEmpty then UtilLayer.randomLayerColor else (fun () -> color)
        if notNull parent && isNull name then
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddLayer if parent name is given (%s) the child name must be given too." parent

        let vis =   match visible with
                    | 0 ->  UtilLayer.Off
                    | 1 ->  UtilLayer.On
                    | 2 ->  UtilLayer.ByParent
                    | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.AddLayer Bad value vor Visibility: %d, it may be 0, 1 or 2" visible
        let loc =   match locked with
                    | 0 ->  UtilLayer.Off
                    | 1 ->  UtilLayer.On
                    | 2 ->  UtilLayer.ByParent
                    | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.AddLayer Bad value vor Locked: %d, it may be 0, 1 or 2" locked

        if isNull name then
            //State.Doc.Layers.[UtilLayer.createDefaultLayer(col, true, false)].FullPath
            UtilLayer.createDefaultLayer(col, true, false)
        else
            let names = if isNull parent then name else parent+ "::" + name
            let fOrC  = UtilLayer.getOrCreateLayer(names, col, vis, loc, allowAllUnicode, collapseParents)
            //State.Doc.Layers.[i].FullPath
            fOrC.Index


    ///<summary>Returns the full layer name of an object.
    /// parent layers are separated by <c>::</c>.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<returns>(string) The object's current layer.</returns>
    static member ObjectLayer(objectId:Guid) : string = //GET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = obj.Attributes.LayerIndex
        State.Doc.Layers.[index].FullPath

    ///<summary>Modifies the layer of an object , optionally creates layer if it does not exist yet.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="createLayerIfMissing">(bool) Optional, default value: <c>false</c> Set true to create Layer if it does not exist yet.</param>
    ///<param name="allowAllUnicode">(bool) Optional, Allow Ambiguous Unicode characters too </param>
    ///<param name="collapseParents">(bool) Optional, Collapse parent layers in Layer UI </param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectLayer( objectId:Guid
                             , layer:string
                             ,[<OPT;DEF(false)>]createLayerIfMissing:bool
                             ,[<OPT;DEF(false:bool)>]allowAllUnicode:bool
                             ,[<OPT;DEF(false:bool)>]collapseParents:bool) : unit = //SET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let layerIndex =
            if createLayerIfMissing then  UtilLayer.getOrCreateLayer(layer, UtilLayer.randomLayerColor, UtilLayer.ByParent, UtilLayer.ByParent, allowAllUnicode,collapseParents).Index
            else                          RhinoScriptSyntax.CoerceLayer(layer).Index
        obj.Attributes.LayerIndex <- layerIndex
        obj.CommitChanges() |> ignore
        State.Doc.Views.Redraw()

    ///<summary>Modifies the layer of multiple objects, optionally creates layer if it does not exist yet.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of the objects</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="createLayerIfMissing">(bool) Optional, default value: <c>false</c> Set true to create Layer if it does not exist yet.</param>
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
            else                          RhinoScriptSyntax.CoerceLayer(layer).Index
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            obj.Attributes.LayerIndex <- layerIndex
            obj.CommitChanges() |> ignore
        State.Doc.Views.Redraw()

    ///<summary>Modifies the layer of an object.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<param name="layerIndex">(int) Index of layer in layer table</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectLayer(objectId:Guid, layerIndex:int) : unit = //SET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        if layerIndex < 0 || layerIndex >= State.Doc.Layers.Count then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLayer: Setting it via index failed. bad index '%d' (max %d) on: %s " layerIndex State.Doc.Layers.Count (Nice.str objectId)
        if State.Doc.Layers.[layerIndex].IsDeleted then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLayer: Setting it via index failed.  index '%d' is deleted.  on: %s " layerIndex  (Nice.str objectId)
        obj.Attributes.LayerIndex <- layerIndex
        obj.CommitChanges() |> ignore
        State.Doc.Views.Redraw()

    ///<summary>Modifies the layer of multiple objects, optionally creates layer if it does not exist yet.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of the objects</param>
    ///<param name="layerIndex">(int) Index of layer in layer table</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectLayer(objectIds:Guid seq, layerIndex:int) : unit = //MULTISET
        if layerIndex < 0 || layerIndex >= State.Doc.Layers.Count then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLayer: Setting it via index failed. bad index '%d' (max %d) on %d objects " layerIndex State.Doc.Layers.Count (Seq.length objectIds)
        if State.Doc.Layers.[layerIndex].IsDeleted then RhinoScriptingException.Raise "RhinoScriptSyntax.ObjectLayer: Setting it via index failed.  index '%d' is deleted.  on %d objects" layerIndex  (Seq.length objectIds)
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            obj.Attributes.LayerIndex <- layerIndex
            obj.CommitChanges() |> ignore
        State.Doc.Views.Redraw()

    ///<summary>Changes the Name of a layer if than name is yet non existing. Fails if layer exists already. Currently only ASCII characters are allowed.</summary>
    ///<param name="currentLayerName">(string) The name an existing layer to rename</param>
    ///<param name="newLayerName">(string) The new name</param>
    ///<param name="allowUnicode">(bool) Optional, Allow ambiguous Unicode characters too </param>
    ///<returns>(unit) void, nothing.</returns>
    static member ChangeLayerName( currentLayerName:string
                                 , newLayerName:string
                                 , [<OPT;DEF(false:bool)>]allowUnicode:bool ) : unit =
        let i = State.Doc.Layers.FindByFullPath(currentLayerName, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then
            RhinoScriptingException.Raise "RhinoScriptSyntax.ChangeLayerName: could not FindByFullPath Layer from currentLayerName: '%s'" currentLayerName
        else
            UtilLayer.failOnBadShortLayerName (newLayerName, newLayerName, allowUnicode)
            let lay = State.Doc.Layers.[i]
            let ps = lay.FullPath.Split( [|"::"|], StringSplitOptions.RemoveEmptyEntries) //|> ResizeArray.ofArray
            ps.Last <- newLayerName
            let np = String.concat "::" ps
            let ni = State.Doc.Layers.FindByFullPath(np, RhinoMath.UnsetIntIndex)
            if ni >= 0 then
                RhinoScriptingException.Raise "RhinoScriptSyntax.ChangeLayerName: could not rename Layer '%s' to '%s', it already exists." currentLayerName np
            else
                lay.Name <- newLayerName


    ///<summary>Returns the current layer.</summary>
    ///<returns>(string) The full name of the current layer.</returns>
    static member CurrentLayer() : string = //GET
        State.Doc.Layers.CurrentLayer.FullPath


    ///<summary>Changes the current layer.</summary>
    ///<param name="layer">(string) The name of an existing layer to make current</param>
    ///<returns>(unit) void, nothing.</returns>
    static member CurrentLayer(layer:string) : unit = //SET
        let i = State.Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then RhinoScriptingException.Raise "RhinoScriptSyntax.CurrentLayer: could not FindByFullPath Layer from name'%s'" layer
        if not<|  State.Doc.Layers.SetCurrentLayerIndex(i, quiet=true) then RhinoScriptingException.Raise "RhinoScriptSyntax.CurrentLayer Set CurrentLayer to %s failed" layer

    ///<summary>Removes an existing layer from the document. The layer to be removed
    ///    cannot be the current layer. Unlike the PurgeLayer method, the layer must
    ///    be empty, or contain no objects, before it can be removed. Any layers that
    ///    are children of the specified layer will also be removed if they are also
    ///    empty.</summary>
    ///<param name="layer">(string) The name of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member DeleteLayer(layer:string) : bool =
        let i = State.Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteLayer: could not FindByFullPath Layer from name'%s'" layer
        State.Doc.Layers.Delete(i, quiet=true)


    ///<summary>Expands or Collapses a layer. Expanded layers can be viewed in Rhino's layer dialog.
    /// Use the functions rs.UnCollapseLayer and rs.CollapseLayer to expand and collapse their children too.</summary>
    ///<param name="layer">(string) Name of the layer to expand</param>
    ///<param name="expand">(bool) True to expand, False to collapse</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ExpandLayer(layer:string, expand:bool) : unit =
        let i = State.Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then RhinoScriptingException.Raise "RhinoScriptSyntax.ExpandLayer: could not FindByFullPath Layer from name'%s'" layer
        let layer = State.Doc.Layers.[i]
        if layer.IsExpanded <> expand then
            layer.IsExpanded <- expand



    ///<summary>Verifies the existence of a layer in the document.</summary>
    ///<param name="layer">(string) The name of a layer to search for</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayer(layer:string) : bool =
        let i = State.Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        i <> RhinoMath.UnsetIntIndex


    ///<summary>Checks if the objects on a layer can be changed (normal).</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerChangeable(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.IsVisible && not layer.IsLocked
        rc


    ///<summary>Checks if a layer is a child of another layer.</summary>
    ///<param name="layer">(string) The name of the layer to test against</param>
    ///<param name="test">(string) The name to the layer to test</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerChildOf(layer:string, test:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let test = RhinoScriptSyntax.CoerceLayer(test)
        test.IsChildOf(layer)


    ///<summary>Checks if a layer is the current layer.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerCurrent(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.Index = State.Doc.Layers.CurrentLayerIndex


    ///<summary>Checks if an existing layer is empty, or contains no objects.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerEmpty(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rhobjs = State.Doc.Objects.FindByLayer(layer)
        isNull rhobjs || rhobjs.Length = 0


    ///<summary>Checks if a layer is expanded. Expanded layers can be viewed in
    ///    Rhino's layer dialog.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerExpanded(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsExpanded


    ///<summary>Checks if a layer is locked
    /// persistent or non persistent locking return true
    /// via layer.IsLocked.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerLocked(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsLocked


    ///<summary>Checks if a layer is on.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerOn(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible


    ///<summary>Checks if an existing layer is selectable (normal and reference).</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerSelectable(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible && not layer.IsLocked


    ///<summary>Checks if a layer is a parent of another layer.</summary>
    ///<param name="layer">(string) The name of the layer to test against</param>
    ///<param name="test">(string) The name to the layer to test</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerParentOf(layer:string, test:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let test = RhinoScriptSyntax.CoerceLayer(test)
        test.IsParentOf(layer)


    ///<summary>Checks if a layer is from a reference file.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerReference(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsReference


    ///<summary>Checks if a layer is visible.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerVisible(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible


    ///<summary>Returns the number of immediate child layers of a layer.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(int) The number of immediate child layers.</returns>
    static member LayerChildCount(layer:string) : int =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let children = layer.GetChildren()
        if notNull children then children.Length
        else 0


    ///<summary>Returns the immediate child layers of a layer. ( excluding children of children) </summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(string ResizeArray) List of children layer names.</returns>
    static member LayerChildren(layer:string) : string ResizeArray =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let children = layer.GetChildren()
        if notNull children then
            let ls = ResizeArray()
            for child in children do
                ls.Add(child.FullPath)
            ls
        else ResizeArray() //empty list


    ///<summary>Returns all (immediate and descendent) child and grand child layers of a layer.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(string ResizeArray) List of children layer names.</returns>
    static member LayerChildrenAll(layer:string) : string ResizeArray =
        let rec loop (l:DocObjects.Layer) =
            seq{
                let children = l.GetChildren()
                if notNull children then
                    for child in children do
                        yield child
                        yield! loop child } //recurse
        layer
        |> RhinoScriptSyntax.CoerceLayer
        |> loop
        |> Seq.map ( fun l -> l.FullPath)
        |> ResizeArray

    ///<summary>Returns the color of a layer.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(Drawing.Color) The current color value .</returns>
    static member LayerColor(layer:string) : Drawing.Color = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.Color

    ///<summary>Changes the color of a layer.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerColor(layer:string, color:Drawing.Color) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.Color <- color
        State.Doc.Views.Redraw()


    ///<summary>Returns the number of layers in the document.</summary>
    ///<returns>(int) The number of layers in the document.</returns>
    static member LayerCount() : int =
        State.Doc.Layers.ActiveCount


    ///<summary>Return identifiers of all layers in the document.</summary>
    ///<returns>(Guid ResizeArray) The identifiers of all layers in the document.</returns>
    static member LayerIds() : Guid ResizeArray =
        State.Doc.Layers
        |> Seq.filter (fun layer -> not layer.IsDeleted)
        |> Seq.map (fun layer -> layer.Id)
        |> ResizeArray


    ///<summary>Returns the line type of a layer.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) Name of the current line type.</returns>
    static member LayerLinetype(layer:string) : string = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let index = layer.LinetypeIndex
        State.Doc.Linetypes.[index].Name


    ///<summary>Changes the line type of a layer.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="lineType">(string) Name of a line type</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLinetype(layer:string, lineType:string) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let mutable index = layer.LinetypeIndex
        if lineType = State.Doc.Linetypes.ContinuousLinetypeName then
            index <- -1
        else
            let lt = State.Doc.Linetypes.FindName(lineType)
            if lt|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.LayerLinetype not found. layer:'%s' line typ:'%s'" layer.FullPath lineType
            index <- lt.LinetypeIndex
        layer.LinetypeIndex <- index
        State.Doc.Views.Redraw()


    ///<summary>Returns the visible property of a layer,
    ///  if layer is on but invisible because of a parent that is off this returns false.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(bool) The current layer visibility.</returns>
    static member LayerVisible(layer:string) : bool = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.IsVisible
        rc


    ///<summary>Makes a layer visible.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="forceVisible">(bool) Optional, default value: <c>true</c>
    ///     Turn on parent layers too if needed. True by default</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerVisibleSetTrue(layer:string, [<OPT;DEF(true)>]forceVisible:bool) : unit =
        let lay = RhinoScriptSyntax.CoerceLayer(layer)
        UtilLayer.visibleSetTrue(lay,forceVisible)
        State.Doc.Views.Redraw()

    ///<summary>Makes a layer invisible.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="persist">(bool) Optional, default value: <c>false</c>
    ///     Turn layer persistently off? even if it is already invisible because of a parent layer that is turned off.
    ///     By default already invisible layers are not changed</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerVisibleSetFalse(layer:string,  [<OPT;DEF(false)>]persist:bool) : unit =
        let lay = RhinoScriptSyntax.CoerceLayer(layer)
        UtilLayer.visibleSetFalse(lay,persist)
        State.Doc.Views.Redraw()

    (*
    ///<summary>Changes the visible property of a layer.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="visible">(bool) New visible state</param>
    ///<param name="forcevisibleOrDonotpersist">(bool)
    ///    If visible is True then turn parent layers on if True.
    ///    If visible is False then do not persist if True</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerVisible(layer:string, visible:bool, [<OPT;DEF(false)>]forcevisibleOrDonotpersist:bool) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible <- visible
        if visible && forcevisibleOrDonotpersist then
            State.Doc.Layers.ForceLayerVisible(layer.Id) |> ignore
        if not visible && not forcevisibleOrDonotpersist then
            if layer.ParentLayerId <> Guid.Empty then
                layer.SetPersistentVisibility(visible)
            // layer.CommitChanges() |> ignore //obsolete !!
        State.Doc.Views.Redraw()
    *)


    ///<summary>Turn a layer off if visible, does nothing if parent layer is already invisible. .</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerOff(layer:string) : unit =
        RhinoScriptSyntax.LayerVisibleSetFalse(layer, persist=false)

    ///<summary>Turn a layer on if not  visible , enforces visibility  of parents.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerOn(layer:string) : unit = //SET
        RhinoScriptSyntax.LayerVisibleSetTrue(layer, forceVisible=true)

    ///<summary>Returns the locked property of a layer,
    ///  if layer is unlocked but parent layer is locked this still returns true.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(bool) The current layer visibility.</returns>
    static member LayerLocked(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.IsLocked //not same as // https://github.com/mcneel/rhinoscriptsyntax/pull/193 // TODO ??
        rc

    ///<summary>Makes a layer locked.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="forceLocked">(bool) Optional, default value: <c>false</c>
    ///     Lock layer even if it is already locked via a parent layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLockedSetTrue(layer:string, [<OPT;DEF(false)>]forceLocked:bool) : unit =
        let lay = RhinoScriptSyntax.CoerceLayer(layer)
        UtilLayer.lockedSetTrue(lay,forceLocked)
        State.Doc.Views.Redraw()

    ///<summary>Makes a layer unlocked.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="parentsToo">(bool) Optional, default value: <c>true</c>
    ///     Unlock parent layers to if needed</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLockedSetFalse(layer:string,  [<OPT;DEF(true)>]parentsToo:bool) : unit =
        let lay = RhinoScriptSyntax.CoerceLayer(layer)
        UtilLayer.lockedSetFalse(lay,parentsToo)
        State.Doc.Views.Redraw()


    ///<summary>Unlocks a layer and all parents if needed .</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerUnlock(layer:string) : unit =
        RhinoScriptSyntax.LayerLockedSetFalse(layer, parentsToo=true)

    ///<summary>Locks a layer if it is not already locked via a parent.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLock(layer:string) : unit = //SET
        RhinoScriptSyntax.LayerLockedSetTrue(layer, forceLocked=false)

    (*
    ///<summary>Changes the locked mode of a layer, enforces persistent locking.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="locked">(bool) New layer locked mode</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLocked(layer:string, locked:bool) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if layer.ParentLayerId <> Guid.Empty then
            let l = layer.GetPersistentLocking()
            if l <> locked then
                layer.SetPersistentLocking(locked)
                State.Doc.Views.Redraw()
        else
            if locked <> layer.IsLocked then
                layer.IsLocked <- locked
                State.Doc.Views.Redraw()
    *)


    ///<summary>Returns the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) a zero-based material index.</returns>
    static member LayerMaterialIndex(layer:string) : int = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.RenderMaterialIndex

    ///<summary>Changes the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="index">(int) The new material index</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerMaterialIndex(layer:string, index:int) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if  index >= -1 then
            layer.RenderMaterialIndex <- index
            State.Doc.Views.Redraw()



    ///<summary>Returns the identifier of a layer given the layer's name.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Guid) The layer's identifier.</returns>
    static member LayerId(layer:string) : Guid =
        let idx = State.Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if idx = RhinoMath.UnsetIntIndex then RhinoScriptingException.Raise "RhinoScriptSyntax.LayerId not found for name %s" layer
        State.Doc.Layers.[idx].Id



    ///<summary>Return the name of a layer given it's identifier.</summary>
    ///<param name="layerId">(Guid) Layer identifier</param>
    ///<param name="fullPath">(bool) Optional, default value: <c>true</c>
    ///    Return the full path name `True` or short name `False`</param>
    ///<returns>(string) The layer's name.</returns>
    static member LayerName(layerId:Guid, [<OPT;DEF(true)>]fullPath:bool) : string =
        let layer = RhinoScriptSyntax.CoerceLayer(layerId)
        if fullPath then layer.FullPath
        else layer.Name


    ///<summary>Returns the names of all layers in the document.</summary>
    ///<returns>(string ResizeArray) list of layer names.</returns>
    static member LayerNames() : string ResizeArray =
        let rc = ResizeArray()
        for layer in State.Doc.Layers do
            if not layer.IsDeleted then rc.Add(layer.FullPath)
        rc


    ///<summary>Returns the current display order index of a layer as displayed in Rhino's
    ///    layer dialog box. A display order index of -1 indicates that the current
    ///    layer dialog filter does not allow the layer to appear in the layer list.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) zero based index of layer.</returns>
    static member LayerOrder(layer:string) : int =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.SortIndex


    ///<summary>Returns the print color of a layer. Layer print colors are
    /// represented as RGB colors.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Drawing.Color) The current layer print color.</returns>
    static member LayerPrintColor(layer:string) : Drawing.Color = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotColor
        rc

    ///<summary>Changes the print color of a layer. Layer print colors are
    /// represented as RGB colors.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="color">(Drawing.Color) New print color</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerPrintColor(layer:string, color:Drawing.Color) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        //color = RhinoScriptSyntax.CoerceColor(color)
        layer.PlotColor <- color
        State.Doc.Views.Redraw()



    ///<summary>Returns the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(float) The current layer print width.</returns>
    static member LayerPrintWidth(layer:string) : float = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotWeight
        rc

    ///<summary>Changes the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="width">(float) New print width</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerPrintWidth(layer:string, width:float) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotWeight
        if width <> rc then
            layer.PlotWeight <- width
            State.Doc.Views.Redraw()



    ///<summary>Return the parent layer of a layer or empty string if no parent present.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) The name of the current parent layer or empty string if no parent present.</returns>
    static member ParentLayer(layer:string) : string = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if layer.ParentLayerId = Guid.Empty then ""
        else
            let oldparentlayer = State.Doc.Layers.FindId(layer.ParentLayerId)
            if isNull oldparentlayer then "" // TODO test! is fail better ?
            else
                oldparentlayer.FullPath


    ///<summary>Modify the parent layer of a layer.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="parent">(string) Name of new parent layer. To remove the parent layer,
    ///    thus making a root-level layer, specify an empty string</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ParentLayer(layer:string, parent:string) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if parent = "" then
            layer.ParentLayerId <- Guid.Empty
        else
            let parent = RhinoScriptSyntax.CoerceLayer(parent)
            layer.ParentLayerId <- parent.Id


    ///<summary>Removes an existing layer and all its objects from the document.
    /// The layer will be removed even if it contains geometry objects.
    /// The layer to be removed cannot be the current layer.</summary>
    ///<param name="layer">(string) The name of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member PurgeLayer(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = State.Doc.Layers.Purge( layer.Index, quiet=true)
        State.Doc.Views.Redraw()
        rc

    ///<summary>Removes all empty layers from the document. Even if it is current</summary>
    ///<param name="keepCurrent">(bool) Optional, default value: <c>true</c>
    ///    'true' to keep the current Layer even if empty
    ///    'false' to remove current layer too if its empty. Any other non empty layer might be the new current</param>
    ///<returns>(unit) void, nothing.</returns>
    static member PurgeEmptyLayers([<OPT;DEF(true)>]keepCurrent:bool) : unit =
        let taken=Collections.Generic.HashSet<Guid>()
        let Layers = State.Doc.Layers
        let mutable nonEmptyIndex = -1
        let rec addLoop(g:Guid)=
            if g <> Guid.Empty then
                taken.Add(g)|> ignore
                addLoop (Layers.FindId(g).ParentLayerId) // recurse

        for o in State.Doc.Objects do
            if not o.IsDeleted then
                let i = o.Attributes.LayerIndex
                nonEmptyIndex <- i
                let l = Layers.[i]
                addLoop l.Id

        let c = Layers.CurrentLayer.Id
        // deal with current layers
        // either keep
        if keepCurrent then
            addLoop c
        //or try to set it to another one thats not empty
        elif taken.Count > 0  && taken.DoesNotContain c && nonEmptyIndex > -1 then
            // change current layer to a non empty one:
            Layers.SetCurrentLayerIndex(nonEmptyIndex, quiet=true ) |> ignore

        // now delete them all
        for i = Layers.Count - 1 downto 0 do
            let l = Layers.[i]
            if not l.IsDeleted then
                if taken.DoesNotContain (l.Id) then
                    Layers.Delete(i, quiet=true) |> ignore // more than one layer might fail to delete if current layer is nested in parents
                    //if i <> Layers.CurrentLayerIndex then
                    //    if not <| Layers.Delete(i, quiet=true) then
                    //        //if Layers |> Seq.filter ( fun l -> not l.IsDeleted) |> Seq.length > 1 then
                    //        RhinoScriptingException.Raise "RhinoScriptSyntax.PurgeEmptyLayers: failed to delete layer '%s' " l.FullPath // the last layer can't be deleted so don't raise exception
        State.Doc.Views.Redraw()


    ///<summary>Renames an existing layer.</summary>
    ///<param name="oldName">(string) Original layer name</param>
    ///<param name="newName">(string) New layer name</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenameLayer(oldName:string, newName:string) : unit =
        if oldName <> newName then
            let layer = RhinoScriptSyntax.CoerceLayer(oldName)
            layer.Name <- newName // TODO test with bad chars in layer string

    ///<summary>Collapse a layer in UI if it has children. This is the opposite of rs.ExpandLayer(..) </summary>
    ///<param name="layerName">(string) full or short layer name</param>
    ///<param name="childrenToo">(bool) Optional, default value: <c>false</c>
    ///    'true' to collapse any child layers too.
    ///    'false' to not change the parents layers collapse state.(default)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member CollapseLayer(layerName:string, [<OPT;DEF(false)>]childrenToo:bool) : unit =
        let layer = RhinoScriptSyntax.CoerceLayer(layerName)
        if childrenToo then
            let rec collapse (l:DocObjects.Layer) =
                if l.IsExpanded then
                    l.IsExpanded <- false
                for child in l.GetChildren() do
                    collapse child
            collapse layer
        else
            if layer.IsExpanded then
                layer.IsExpanded <- false

    ///<summary>Expand a layer in UI if it has children. This is the opposite of rs.CollapseLayer(..) </summary>
    ///<param name="layerName">(string) full or short layer name</param>
    ///<param name="childrenToo">(bool) Optional, default value: <c>false</c>
    ///    'true' to expand any child layers too.
    ///    'false' to not change the child layers collapse state.(default)</param>
    ///<returns>(unit) void, nothing.</returns>
    static member UnCollapseLayer(layerName:string,[<OPT;DEF(false)>]childrenToo:bool) : unit =
        let layer = RhinoScriptSyntax.CoerceLayer(layerName)
        if childrenToo then
            let rec expand (l:DocObjects.Layer) =
                if not l.IsExpanded then
                    l.IsExpanded <- true
                for child in l.GetChildren() do
                    expand child
            expand layer
        else
            if layer.IsExpanded then
                layer.IsExpanded <- false

    ///<summary>Expand all parent layers in UI if this layer is hidden by a collapsed parent layer.</summary>
    ///<param name="layerName">(string) full or short layer name</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ShowLayer(layerName:string) : unit =
        let rec expand (l:DocObjects.Layer) =
            if not l.IsExpanded then
                l.IsExpanded <- true
            if l.ParentLayerId <> Guid.Empty then
                expand <| State.Doc.Layers.FindId(l.ParentLayerId)

        let layer = RhinoScriptSyntax.CoerceLayer(layerName)
        if layer.ParentLayerId <> Guid.Empty then
            expand <| State.Doc.Layers.FindId(layer.ParentLayerId)

