namespace Rhino.Scripting 

open Rhino 

open System
open System.Globalization // for UnicodeCategory
open Rhino.DocObjects


module internal UtilLayer = 
    
    let randomLayerColor()=
        let c = FsEx.Color.randomForRhino()
        Drawing.Color.FromArgb(int c.Red, int c.Green, int c.Blue)


    let eVSLN = "RhinoScriptSyntax.UtilLayer.failOnBadShortLayerName"

    /// Raise exceptions if short layer-name is not valid
    /// it may not contain :: or control characters
    let internal failOnBadShortLayerName(name:string, fullPath:string, allowAllUnicode) : unit= 
        if isNull name then RhinoScriptingException.Raise "%s: null string as layer name in '%s'" eVSLN fullPath

        if String.IsNullOrWhiteSpace name then // to cover for StringSplitOptions.None
            RhinoScriptingException.Raise "%s Empty or just whitespace string as layer name in '%s'" eVSLN fullPath
        
        if name.Contains "::" then
            RhinoScriptingException.Raise "%s: The layer name '%s' shall not contains two colons (::).  in '%s'" eVSLN name fullPath
        
        let trimmed = name.Trim()
        
        if trimmed.Length <> name.Length then 
            RhinoScriptingException.Raise "The layer name may not start or end with whitespace: '%s'" name
        
        if allowAllUnicode then 
            if not<| Util.isAcceptableStringId(trimmed, false) then 
                RhinoScriptingException.Raise "%s: The layer name '%s' is invalid. It may not include line returns, tabs, and leading or trailing whitespace. in '%s'" eVSLN  name fullPath
        else
            if not<| Util.isGoodStringId(trimmed, false) then             
                RhinoScriptingException.Raise "%s: The layer name '%s' has invalid or not recommended characters.\r\nYou may be able to use this layer name by setting the optional 'allowUnicode' parameter in '%s' to 'true'" eVSLN  name fullPath
        
        match Char.GetUnicodeCategory(trimmed.[0]) with
        | UnicodeCategory.OpenPunctuation 
        | UnicodeCategory.ClosePunctuation ->  // { [ ( } ] ) don't work at start of layer name
            RhinoScriptingException.Raise  "%s: The layer name '%s' may not start with a '%c' in '%s'" eVSLN name name.[0] fullPath
        | _ -> ()

    let internal getParents(lay:Layer) = 
        let rec find (l:Layer) ps = 
            if l.ParentLayerId = Guid.Empty then ps
            else
            let pl = State.Doc.Layers.FindId(l.ParentLayerId)
            if isNull pl then RhinoScriptingException.Raise "RhinoScriptSyntax.UtilLayer.getParents : ParentLayerId not found in layers"
            find pl (pl::ps)
        find lay []

    let internal visibleSetTrue(lay:Layer, forceVisible:bool) : unit = 
        if not lay.IsVisible then
            if forceVisible then
                if not (State.Doc.Layers.ForceLayerVisible(lay.Id)) then RhinoScriptingException.Raise "RhinoScriptSyntax.UtilLayer.visibleSetTrue Failed to turn on sub-layers of layer  %s"  lay.FullPath
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
    /// The collapseParents parameter only has an effect if layer is created, not if it exists already  
    let internal getOrCreateLayer(name, colorForNewLayers, visible:LayerState, locked:LayerState, allowAllUnicode:bool, collapseParents:bool) : FoundOrCreatedIndex = 
        // TODO trim off leading and trailing '::' from name string to be more tolerant ?
        match State.Doc.Layers.FindByFullPath(name, RhinoMath.UnsetIntIndex) with
        | RhinoMath.UnsetIntIndex ->
            match name with
            | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.UtilLayer.getOrCreateLayer: Cannot get or create layer from null string"
            | ""   -> RhinoScriptingException.Raise "RhinoScriptSyntax.UtilLayer.getOrCreateLayer: Cannot get or create layer from empty string"
            | _ ->
                match name.Split( [|"::"|], StringSplitOptions.None) with // TODO or use StringSplitOptions.RemoveEmptyEntries ??
                | [| |] -> RhinoScriptingException.Raise "RhinoScriptSyntax.UtilLayer.getOrCreateLayer: Cannot get or create layer for name: '%s'" name
                | ns ->
                    let rec createLayer(nameList, prevId, prevIdx, root) : int = 
                        match nameList with
                        | [] -> prevIdx // exit recursion
                        | branch :: rest ->
                            if String.IsNullOrWhiteSpace branch then // to cover for StringSplitOptions.None
                                RhinoScriptingException.Raise "RhinoScriptSyntax.UtilLayer.getOrCreateLayer: A segment falls into String.IsNullOrWhiteSpace. Cannot get or create layer for name: '%s'" name
                            let fullPath = if root="" then branch else root + "::" + branch
                            match State.Doc.Layers.FindByFullPath(fullPath, RhinoMath.UnsetIntIndex) with
                            | RhinoMath.UnsetIntIndex -> // actually create layer:
                                failOnBadShortLayerName (branch, name, allowAllUnicode)   // only check non existing sub layer names
                                let layer = DocObjects.Layer.GetDefaultLayerProperties()
                                if prevId <> Guid.Empty then 
                                    layer.ParentLayerId <- prevId
                                    if collapseParents then 
                                        State.Doc.Layers.[prevIdx].IsExpanded <- false

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
                                let id = State.Doc.Layers.[i].Id // just using layer.Id would be empty guid
                                createLayer(rest , id , i,  fullPath)

                            | i ->
                                createLayer(rest , State.Doc.Layers.[i].Id , i ,fullPath)

                    LayerCreated (createLayer( ns |> List.ofArray, Guid.Empty, 0, ""))
        | i -> LayerFound i


    /// creates layer with default name
    let internal createDefaultLayer(color, visible, locked) = 
        let layer = DocObjects.Layer.GetDefaultLayerProperties()
        layer.Color <- color() // delay creation of (random) color till actually needed ( so random colors are not created, in most cases layer exists)
        if layer.ParentLayerId <> Guid.Empty then RhinoScriptingException.Raise "RhinoScriptSyntax.createDefaultLayer how can a new default layer have a parent ? %A" layer
        layer.IsVisible <- visible
        layer.IsLocked <- locked
        State.Doc.Layers.Add(layer)

