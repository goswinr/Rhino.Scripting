namespace Rhino

open System
open System.Globalization // for UnicodeCategory
open Rhino.DocObjects


module internal UtilLayer = 
    
    let eVSLN = "Rhino.Scripting.UtilLayer.ensureValidShortLayerName"

    /// Raise exceptions if short layer-name is not valid
    /// it may not contain :: or control characters
    /// set allowSpecialChars to false to only have ASCII
    let internal failOnBadShortLayerName(name:string, limitToAscii) : unit= 
        if isNull name then RhinoScriptingException.Raise "%s: null string as layer name" eVSLN

        if name.Contains "::" then
            RhinoScriptingException.Raise "%s: Short layer name '%s' shall not contains two colons (::) . " eVSLN name

        if not<| Util.isGoodStringId(name, false, limitToAscii) then             
            if limitToAscii then 
                RhinoScriptingException.Raise "%s: Short layer name '%s' has invalid or not recommended characters. Only Unicode points 32 till 126 are allowed. If you really want this layer name add the layer directly via Rhinocommon" eVSLN  name 
            else
                RhinoScriptingException.Raise "%s: Short layer name '%s' has invalid or not recommended characters. If you really want this layer name add the layer directly via Rhinocommon" eVSLN  name 
        
        match Char.GetUnicodeCategory(name.[0]) with
        | UnicodeCategory.OpenPunctuation 
        | UnicodeCategory.ClosePunctuation ->  // { [ ( } ] ) don't work at start of layer name
            RhinoScriptingException.Raise  "%s: Short layer name '%s' may not start with a '%c' " eVSLN name name.[0]
        | _ -> ()

    let internal getParents(lay:Layer) = 
        let rec find (l:Layer) ps = 
            if l.ParentLayerId = Guid.Empty then ps
            else
            let pl = State.Doc.Layers.FindId(l.ParentLayerId)
            if isNull pl then RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.getParents let internal getParents: ParentLayerId not found in layers"
            find pl (pl::ps)
        find lay []

    let internal visibleSetTrue(lay:Layer, forceVisible:bool) : unit = 
        if not lay.IsVisible then
            if forceVisible then
                if not (State.Doc.Layers.ForceLayerVisible(lay.Id)) then RhinoScriptingException.Raise "rs.LayerVisibleSetTrue Failed to turn on sublayers of layer  %s"  lay.FullPath
            else
                lay.SetPersistentVisibility(true)

    let internal visibleSetFalse(lay:Layer, persist:bool) : unit = 
        if lay.IsVisible then
            lay.IsVisible <- false
        if persist then
            if lay.ParentLayerId <> Guid.Empty then
                lay.SetPersistentVisibility(false)

    let internal lockedSetTrue(lay:Layer,forceLocked:bool) : unit = 
        if not lay.IsLocked then
            lay.IsLocked <- true
        if forceLocked then
            lay.SetPersistentLocking(true)


    let internal lockedSetFalse(lay:Layer,  parentsToo:bool) : unit = 
        if lay.IsLocked then
            //lay.IsLocked <- false // fails with msg box if parents are locked
            if parentsToo then
                for l in getParents(lay) do
                    l.SetPersistentLocking(false)
                lay.SetPersistentLocking(false) // does not unlock parents

    type internal LayerState = Off | On | ByParent

    [<Struct>]
    type internal FoundOrCreatedIndex = 
        |LayerCreated of createdIdx :int
        |LayerFound   of foundIdx   :int

        member this.Index = 
            match this with 
            |LayerCreated i -> i
            |LayerFound   i -> i
      

    /// Creates all parent layers too if they are missing, uses same locked state and colors for all new layers.
    /// Does not allow ambiguous Unicode characters in layer-name.
    /// Returns index
    /// Empty string returns current layer index
    let internal getOrCreateLayer(name, colorForNewLayers, visible:LayerState, locked:LayerState) : FoundOrCreatedIndex = 
        match State.Doc.Layers.FindByFullPath(name, RhinoMath.UnsetIntIndex) with
        | RhinoMath.UnsetIntIndex ->
            match name with
            | null -> RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.getOrCreateLayer Cannot get or create layer from null string"
            | "" -> RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.getOrCreateLayer Cannot get or create layer from empty string"
            | _ ->
                match name.Split( [|"::"|], StringSplitOptions.RemoveEmptyEntries) with
                | [| |] -> RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.getOrCreateLayer Cannot get or create layer for name: '%s'" name
                | ns ->
                    let rec createLayer(nameList, prevId, prevIdx, root) : int = 
                        match nameList with
                        | [] -> prevIdx // exit recursion
                        | branch :: rest ->
                            let fullpath = if root="" then branch else root + "::" + branch
                            match State.Doc.Layers.FindByFullPath(fullpath, RhinoMath.UnsetIntIndex) with
                            | RhinoMath.UnsetIntIndex -> // actually create layer:
                                failOnBadShortLayerName (branch, false)   // only check non existing sub layer names
                                let layer = DocObjects.Layer.GetDefaultLayerProperties()
                                if prevId <> Guid.Empty then layer.ParentLayerId <- prevId

                                match visible with
                                |ByParent -> ()
                                |On  -> visibleSetTrue(layer, true)
                                |Off -> visibleSetFalse(layer, true)

                                match locked with
                                |ByParent -> ()
                                |On  -> lockedSetTrue(layer, true)
                                |Off -> lockedSetFalse(layer, true)

                                layer.Name <- branch
                                layer.Color <- colorForNewLayers() // delay creation of (random) color till actually needed ( so random colors are not created, in most cases layer exists)
                                let i = State.Doc.Layers.Add(layer)
                                let id = State.Doc.Layers.[i].Id //just using layer.Id would be empty guid
                                createLayer(rest , id , i,  fullpath)

                            | i ->
                                createLayer(rest , State.Doc.Layers.[i].Id , i ,fullpath)

                    LayerCreated (createLayer( ns |> List.ofArray, Guid.Empty, 0, ""))
        | i -> LayerFound i


    /// creates layer with default name
    let internal createDefaultLayer(color, visible, locked) = 
        let layer = DocObjects.Layer.GetDefaultLayerProperties()
        layer.Color <- color() // delay creation of (random) color till actually needed ( so random colors are not created, in most cases layer exists)
        if layer.ParentLayerId <> Guid.Empty then RhinoScriptingException.Raise "how can a new default layer have a parent ? %A" layer
        layer.IsVisible <- visible
        layer.IsLocked <- locked
        State.Doc.Layers.Add(layer)

