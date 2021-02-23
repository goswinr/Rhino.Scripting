namespace Rhino.Scripting

open FsEx
open System
open Rhino
open System.Globalization
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore
open Rhino.DocObjects


// -------------------- Layer related functions moved in fist position sinsce they are used in other modules. -------------------

 

/// This module is automatically opened when Rhino.Scripting namspace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
[<AutoOpen>]
module ExtensionsLayer =
  
  /// Raise exceptions if short layername is not valid
  /// it may not contain :: or control characters
  /// set allowSpecialChars to false to only have ASCII
  let internal ensureValidShortLayerName(name:string, allowSpecialChars) : unit= 
    if isNull name then RhinoScriptingException.Raise "RhinoScriptSyntax found an null string as layer name" 
    if name.Length = 0 then RhinoScriptingException.Raise "RhinoScriptSyntax found an empty string as layer name" 
   
    let tr = name.Trim()
    if tr.Length <> name.Length then 
        RhinoScriptingException.Raise "RhinoScriptSyntax found an invalid layer name: '%s'. It has trailing or leading white space" name
   
    if name.Contains "::" then 
        RhinoScriptingException.Raise "RhinoScriptSyntax found an invalid short layer name containing two colons(::)  '%s'. " name        
   
    match Char.GetUnicodeCategory(name.First) with
    | UnicodeCategory.OpenPunctuation | UnicodeCategory.ClosePunctuation ->  // { [ ( } ] ) dont work at start of layer name
        RhinoScriptingException.Raise  "RhinoScriptSyntax found an invalid layer name: '%s'. The name may not start with a '%c' " name name.First
    | _ -> ()      
   
    for c in name do
        let cat = Char.GetUnicodeCategory(c)
        match cat with
        |UnicodeCategory.Control  ->                     
            RhinoScriptingException.Raise "RhinoScriptSyntax found an invalid short layer name containing UnicodeCategory.Control characters: '%s'. " name       
        
        |UnicodeCategory.SpaceSeparator when c <> ' ' -> 
            RhinoScriptingException.Raise "RhinoScriptSyntax found an invalid short layer name containing UnicodeCategory.SpaceSeparator other than regular space in '%s'. If you really want this character add the layer directly via Rhinocommon"  name
        
        |UnicodeCategory.ConnectorPunctuation when c <> '_' -> 
            RhinoScriptingException.Raise "RhinoScriptSyntax found an not recommended charcater '%c' in short layer name containing %A  in '%s'. If you really want this character add the layer directly via Rhinocommon" c cat  name
        
        |UnicodeCategory.DashPunctuation when c <> '-'  ->       // only minus is allowed
            RhinoScriptingException.Raise "RhinoScriptSyntax found an not recommended charcater '%c' in short layer name containing %A  in '%s'. If you really want this character add the layer directly via Rhinocommon" c cat  name
        
        |UnicodeCategory.TitlecaseLetter            
        |UnicodeCategory.ModifierLetter             
        |UnicodeCategory.NonSpacingMark             
        |UnicodeCategory.SpacingCombiningMark       
        |UnicodeCategory.EnclosingMark              
        |UnicodeCategory.LetterNumber               
        |UnicodeCategory.OtherNumber                
        |UnicodeCategory.LineSeparator              
        |UnicodeCategory.ParagraphSeparator         
        |UnicodeCategory.Format 
        |UnicodeCategory.OtherNotAssigned 
        |UnicodeCategory.ModifierSymbol ->
            RhinoScriptingException.Raise "RhinoScriptSyntax found a probably not supported charcater in short layer name containing %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" cat  name
                   
        | _ -> ()
   
        if not allowSpecialChars then
             match cat with
             |UnicodeCategory.OtherPunctuation when c > '\\' -> 
                 RhinoScriptingException.Raise "RhinoScriptSyntax found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name
             
             |UnicodeCategory.CurrencySymbol when c > '¥' -> //Unicode	U+00A5 ¥ YEN SIGN
                 RhinoScriptingException.Raise "RhinoScriptSyntax found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name

             |UnicodeCategory.OpenPunctuation when c > '{' -> 
                 RhinoScriptingException.Raise "RhinoScriptSyntax found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name

             |UnicodeCategory.ClosePunctuation when c > '}' -> 
                 RhinoScriptingException.Raise "RhinoScriptSyntax found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name
             
             |UnicodeCategory.MathSymbol when c > '÷' -> //Unicode	U+00F7 ÷ DIVISION SIGN
                 RhinoScriptingException.Raise "RhinoScriptSyntax found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name
             
             |UnicodeCategory.DecimalDigitNumber when c > '9' -> 
                 RhinoScriptingException.Raise "RhinoScriptSyntax found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name
             
             |UnicodeCategory.UppercaseLetter when c > 'Z' -> 
                 RhinoScriptingException.Raise "RhinoScriptSyntax found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name
             
             |UnicodeCategory.LowercaseLetter when c > 'z' ->
                 RhinoScriptingException.Raise "RhinoScriptSyntax found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name
             
             |UnicodeCategory.OtherSymbol 
             |UnicodeCategory.InitialQuotePunctuation 
             |UnicodeCategory.FinalQuotePunctuation 
             |UnicodeCategory.OtherLetter ->
                 RhinoScriptingException.Raise "RhinoScriptSyntax found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name
             
             | _ -> ()

  let internal getParents(lay:Layer) = 
      let rec find (l:Layer) ps = 
          if l.ParentLayerId = Guid.Empty then ps
          else
            let pl = Doc.Layers.FindId(l.ParentLayerId)
            if isNull pl then RhinoScriptingException.Raise "RhinoScriptSyntax let internal getParents: ParentLayerId not found in layers"   
            find pl (pl::ps)
      find lay []
  
  let internal visibleSetTrue(lay:Layer, forceVisible:bool) : unit = 
    if not lay.IsVisible then
        if forceVisible then
            if not (Doc.Layers.ForceLayerVisible(lay.Id)) then RhinoScriptingException.Raise "rs.LayerVisibleSetTrue Failed to turn on sublayers of layer  %s"  lay.FullPath        
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
          lay.SetPersistentLocking(false) // does noy unlock parents 

  type internal LayerState = Off | On | ByParent
          
  /// Creates all parent layers too if they are missing, uses same locked state and colors for all new layers.
  /// returns index
  /// empty string returns current layer
  let internal getOrCreateLayer(name, color, visible:LayerState, locked:LayerState) : int = 
      match Doc.Layers.FindByFullPath(name, RhinoMath.UnsetIntIndex) with
      | RhinoMath.UnsetIntIndex -> 
          match name with 
          | null -> RhinoScriptingException.Raise "RhinoScriptSyntax.getOrCreateLayer Cannot get or create layer from null string" 
          | "" -> RhinoScriptingException.Raise "RhinoScriptSyntax.getOrCreateLayer Cannot get or create layer from empty string"        
          | _ -> 
              match name.Split( [|"::"|], StringSplitOptions.RemoveEmptyEntries) with 
              | [| |] -> RhinoScriptingException.Raise "RhinoScriptSyntax.getOrCreateLayer Cannot get or create layer for name: '%s'" name
              | ns -> 
              
                  let rec createLayer(nameList, prevId, prevIdx, root) : int = 
                      match nameList with 
                      | [] -> prevIdx // exit recursion
                      | branch :: rest -> 
                          let fullpath = if root="" then branch else root + "::" + branch 
                          match Doc.Layers.FindByFullPath(fullpath, RhinoMath.UnsetIntIndex) with
                          | RhinoMath.UnsetIntIndex -> // actually create layer:                          
                              ensureValidShortLayerName (branch,false)   // only check non existing layer names                          
                              let layer = DocObjects.Layer.GetDefaultLayerProperties()
                              if prevId <> Guid.Empty then layer.ParentLayerId <- prevId

                              match visible with 
                              |ByParent -> ()
                              |On  -> visibleSetTrue(layer,true)
                              |Off -> visibleSetFalse(layer,true)

                              match locked with 
                              |ByParent -> ()
                              |On  -> lockedSetTrue(layer,true)
                              |Off -> lockedSetFalse(layer,true)
                              
                              layer.Name <- branch
                              layer.Color <- color() // delay creation of (random) color till actually needed ( so random colors are not created, in most cases layer exists)                              
                              let i = Doc.Layers.Add(layer)
                              let id = Doc.Layers.[i].Id //just using layer.Id would be empty guid                                
                              createLayer(rest , id , i,  fullpath)
                          
                          | i -> 
                              createLayer(rest , Doc.Layers.[i].Id , i ,fullpath)
              
                  createLayer( ns |> List.ofArray, Guid.Empty, 0, "")
      | i -> i
  
  
  /// creates layer with default name
  let internal createDefaultLayer(color, visible, locked) = 
      let layer = DocObjects.Layer.GetDefaultLayerProperties() 
      layer.Color <- color() // delay creation of (random) color till actaully neded ( so random colors are not created, in most cases layer exists)
      if layer.ParentLayerId <> Guid.Empty then RhinoScriptingException.Raise "how can a new default layer have a parent ? %A" layer      
      layer.IsVisible <- visible 
      layer.IsLocked <- locked 
      Doc.Layers.Add(layer)        


  //[<Extension>] //Error 3246
  type RhinoScriptSyntax with
     
    
    [<Extension>]
    ///<summary>Add a new layer to the document. If it does not exist yet. Currently anly ASCII characters are allowed.
    /// If layers or parent layers exist already color, visibility and locking parameters are  ignored.</summary>
    ///<param name="name">(string) Optional, The name of the new layer. If omitted, Rhino automatically  generates the layer name.</param>
    ///<param name="color">(Drawing.Color) Optional, A Red-Green-Blue color value.  If omitted a random (non yellow)  color wil be choosen.</param>
    ///<param name="visible">(int) Optional, Default Value: <c>2</c>  
    ///   Layer visibility:
    ///   0 = explicitly Off (even if parent is already Off)
    ///   1 = On and turn parents on too
    ///   2 = inherited from parent, or On by default</param>
    ///<param name="locked">(int) Optional, Default Value: <c>2</c>  
    ///   Layer locking state:
    ///   0 = Unlocked this and parents 
    ///   1 = explicitly Locked (even if parent is already Locked)
    ///   2 = inherited from parent, or Unlocked default</param>
    ///<param name="parent">(string) Optional, Name of existing or non existing parent layer. </param>
    ///<returns>(string) The full name of the new layer.</returns>
    static member AddLayer( [<OPT;DEF(null:string)>]name:string,
                            [<OPT;DEF(Drawing.Color())>]color:Drawing.Color,
                            [<OPT;DEF(2)>]visible:int,
                            [<OPT;DEF(2)>]locked:int,
                            [<OPT;DEF(null:string)>]parent:string) : string = 

        let col   = if color.IsEmpty then Color.randomColorForRhino else (fun () -> color)
        if notNull parent && isNull name then  
            RhinoScriptingException.Raise "RhinoScriptSyntax.AddLayer if parent name is given (%s) the child name must be given too." parent
        
        let vis =    match visible with 
                     | 0 ->  Off
                     | 1 ->  On
                     | 2 ->  ByParent
                     | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.AddLayer Bad value vor Visibility: %d, it may be 0, 1 or 2" visible
        let loc =    match locked with 
                     | 0 ->  Off
                     | 1 ->  On
                     | 2 ->  ByParent
                     | _ -> RhinoScriptingException.Raise "RhinoScriptSyntax.AddLayer Bad value vor Locked: %d, it may be 0, 1 or 2" locked

        if isNull name then 
            Doc.Layers.[createDefaultLayer(col, true, false)].FullPath
        else
            let names = if isNull parent then name else parent+ "::" + name            
            let i     = getOrCreateLayer(names, col, vis, loc)
            Doc.Layers.[i].FullPath
  

    [<Extension>]
    ///<summary>Returns the full layername of an object. 
    /// parent layers are separated by <c>::</c>.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<returns>(string) The object's current layer.</returns>
    static member ObjectLayer(objectId:Guid) : string = //GET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
        let index = obj.Attributes.LayerIndex
        Doc.Layers.[index].FullPath

    [<Extension>]
    ///<summary>Modifies the layer of an object , optionaly creates layer if it does not exist yet.</summary>
    ///<param name="objectId">(Guid) The identifier of the object</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="createLayerIfMissing">(bool) Optional, Default Value: <c>false</c>
    ///     Set true to create Layer if it does not exist yet.</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectLayer(objectId:Guid, layer:string, [<OPT;DEF(false)>]createLayerIfMissing:bool) : unit = //SET
        let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)   
        let layerIndex =
            if createLayerIfMissing then  getOrCreateLayer(layer, Color.randomColorForRhino, ByParent, ByParent)
            else                          RhinoScriptSyntax.CoerceLayer(layer).Index                 
        obj.Attributes.LayerIndex <- layerIndex
        if not <| obj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLayer failed for layer '%s' on: %s " layer (typeDescr objectId)
        Doc.Views.Redraw()
   
    [<Extension>]
    ///<summary>Modifies the layer of multiple objects, optionaly creates layer if it does not exist yet.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of the objects</param>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="createLayerIfMissing">(bool) Optional, Default Value: <c>false</c>
    ///     Set true to create Layer if it does not exist yet.</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ObjectLayer(objectIds:Guid seq, layer:string, [<OPT;DEF(false)>]createLayerIfMissing:bool) : unit = //MULTISET
        let layerIndex =
            if createLayerIfMissing then  getOrCreateLayer(layer, Color.randomColorForRhino, ByParent, ByParent)
            else                          RhinoScriptSyntax.CoerceLayer(layer).Index   
        for objectId in objectIds do
            let obj = RhinoScriptSyntax.CoerceRhinoObject(objectId)
            obj.Attributes.LayerIndex <- layerIndex
            if not <| obj.CommitChanges() then RhinoScriptingException.Raise "RhinoScriptSyntax.Set ObjectLayer failed for layer '%s' and '%A' of %d objects"  layer objectId (Seq.length objectIds)
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Changes the Name of a layer if than name is yet non existing. Fails if layer exists already. Currently anly ASCII characters are allowed.</summary>
    ///<param name="currentLayerName">(string) The name an existing layer to rename</param>
    ///<param name="newLayerName">(string) The new name</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ChangeLayerName(currentLayerName:string, newLayerName:string) : unit = 
        let i = Doc.Layers.FindByFullPath(currentLayerName, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then RhinoScriptingException.Raise "rs.ChangeLayerName: could not FindByFullPath Layer from currentLayerName: '%A'" currentLayerName
        else 
            ensureValidShortLayerName (newLayerName, false)
            let lay = Doc.Layers.[i]
            let ps= lay.FullPath |> String.split "::" |> Rarr.ofArray
            ps.Last <- newLayerName
            let np = String.concat "::" ps
            let ni = Doc.Layers.FindByFullPath(np, RhinoMath.UnsetIntIndex)
            if i >= 0 then 
                RhinoScriptingException.Raise "rs.ChangeLayerName: could not rename Layer '%s' to '%s', it already exists." currentLayerName np
            else
                lay.Name <- newLayerName
       

    [<Extension>]
    ///<summary>Returns the current layer.</summary>
    ///<returns>(string) The full name of the current layer.</returns>
    static member CurrentLayer() : string = //GET
        Doc.Layers.CurrentLayer.FullPath


    [<Extension>]
    ///<summary>Changes the current layer.</summary>
    ///<param name="layer">(string) The name of an existing layer to make current</param>
    ///<returns>(unit) void, nothing.</returns>
    static member CurrentLayer(layer:string) : unit = //SET
        let rc = Doc.Layers.CurrentLayer.FullPath
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then RhinoScriptingException.Raise "RhinoScriptSyntax.CurrentLayer: could not FindByFullPath Layer from name'%s'" layer
        if not<|  Doc.Layers.SetCurrentLayerIndex(i, true) then RhinoScriptingException.Raise "RhinoScriptSyntax.CurrentLayer Set CurrentLayer to %s failed" layer



    [<Extension>]
    ///<summary>Removes an existing layer from the document. The layer to be removed
    ///    cannot be the current layer. Unlike the PurgeLayer method, the layer must
    ///    be empty, or contain no objects, before it can be removed. Any layers that
    ///    are children of the specified layer will also be removed if they are also
    ///    empty.</summary>
    ///<param name="layer">(string) The name of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member DeleteLayer(layer:string) : bool =
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then RhinoScriptingException.Raise "RhinoScriptSyntax.DeleteLayer: could not FindByFullPath Layer from name'%s'" layer
        Doc.Layers.Delete(i, true)


    [<Extension>]
    ///<summary>Expands a layer. Expanded layers can be viewed in Rhino's layer dialog.</summary>
    ///<param name="layer">(string) Name of the layer to expand</param>
    ///<param name="expand">(bool) True to expand, False to collapse</param>
    ///<returns>(unit) void, nothing.</returns>
    static member ExpandLayer(layer:string, expand:bool) : unit =
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if i = RhinoMath.UnsetIntIndex then RhinoScriptingException.Raise "RhinoScriptSyntax.ExpandLayer: could not FindByFullPath Layer from name'%s'" layer
        let layer = Doc.Layers.[i]
        if layer.IsExpanded <> expand then
            layer.IsExpanded <- expand




    [<Extension>]
    ///<summary>Verifies the existance of a layer in the document.</summary>
    ///<param name="layer">(string) The name of a layer to search for</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayer(layer:string) : bool =
        let i = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        i <> RhinoMath.UnsetIntIndex


    [<Extension>]
    ///<summary>Verifies that the objects on a layer can be changed (normal).</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerChangeable(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.IsVisible && not layer.IsLocked
        rc


    [<Extension>]
    ///<summary>Verifies that a layer is a child of another layer.</summary>
    ///<param name="layer">(string) The name of the layer to test against</param>
    ///<param name="test">(string) The name to the layer to test</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerChildOf(layer:string, test:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let test = RhinoScriptSyntax.CoerceLayer(test)
        test.IsChildOf(layer)


    [<Extension>]
    ///<summary>Verifies that a layer is the current layer.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerCurrent(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.Index = Doc.Layers.CurrentLayerIndex


    [<Extension>]
    ///<summary>Verifies that an existing layer is empty, or contains no objects.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerEmpty(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rhobjs = Doc.Objects.FindByLayer(layer)
        isNull rhobjs || rhobjs.Length = 0


    [<Extension>]
    ///<summary>Verifies that a layer is expanded. Expanded layers can be viewed in
    ///    Rhino's layer dialog.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerExpanded(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsExpanded


    [<Extension>]
    ///<summary>Verifies that a layer is locked 
    /// persistent or non persitent locking return true
    /// via layer.IsLocked.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerLocked(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsLocked


    [<Extension>]
    ///<summary>Verifies that a layer is on.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerOn(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible


    [<Extension>]
    ///<summary>Verifies that an existing layer is selectable (normal and reference).</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerSelectable(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible && not layer.IsLocked


    [<Extension>]
    ///<summary>Verifies that a layer is a parent of another layer.</summary>
    ///<param name="layer">(string) The name of the layer to test against</param>
    ///<param name="test">(string) The name to the layer to test</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerParentOf(layer:string, test:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let test = RhinoScriptSyntax.CoerceLayer(test)
        test.IsParentOf(layer)


    [<Extension>]
    ///<summary>Verifies that a layer is from a reference file.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerReference(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsReference


    [<Extension>]
    ///<summary>Verifies that a layer is visible.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(bool) True on success, otherwise False.</returns>
    static member IsLayerVisible(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.IsVisible


    [<Extension>]
    ///<summary>Returns the number of immediate child layers of a layer.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(int) The number of immediate child layers.</returns>
    static member LayerChildCount(layer:string) : int =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let children = layer.GetChildren()
        if notNull children then children.Length
        else 0


    [<Extension>]
    ///<summary>Returns the immediate child layers of a layer.</summary>
    ///<param name="layer">(string) The name of an existing layer</param>
    ///<returns>(string Rarr) List of children layer names.</returns>
    static member LayerChildren(layer:string) : string Rarr =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let children = layer.GetChildren()
        if notNull children then rarr {for child in children do child.FullPath }
        else rarr { () } //empty list


    [<Extension>]
    ///<summary>Returns the color of a layer.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(Drawing.Color) The current color value .</returns>
    static member LayerColor(layer:string) : Drawing.Color = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.Color

    [<Extension>]
    ///<summary>Changes the color of a layer.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="color">(Drawing.Color) The new color value</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerColor(layer:string, color:Drawing.Color) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.Color <- color
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the number of layers in the document.</summary>
    ///<returns>(int) The number of layers in the document.</returns>
    static member LayerCount() : int =
        Doc.Layers.ActiveCount


    [<Extension>]
    ///<summary>Return identifiers of all layers in the document.</summary>
    ///<returns>(Guid Rarr) The identifiers of all layers in the document.</returns>
    static member LayerIds() : Guid Rarr =
        rarr {for layer in Doc.Layers do
                        if not layer.IsDeleted then
                            layer.Id }


    [<Extension>]
    ///<summary>Returns the linetype of a layer.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) Name of the current linetype.</returns>
    static member LayerLinetype(layer:string) : string = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let index = layer.LinetypeIndex
        Doc.Linetypes.[index].Name


    [<Extension>]
    ///<summary>Changes the linetype of a layer.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="linetyp">(string) Name of a linetype</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLinetype(layer:string, linetyp:string) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let mutable index = layer.LinetypeIndex
        if linetyp = Doc.Linetypes.ContinuousLinetypeName then
            index <- -1
        else
            let lt = Doc.Linetypes.FindName(linetyp)
            if lt|> isNull  then RhinoScriptingException.Raise "RhinoScriptSyntax.LayerLinetype not found.  layer:'%A' linetyp:'%A'" layer linetyp
            index <- lt.LinetypeIndex
        layer.LinetypeIndex <- index
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Returns the visible property of a layer, 
    ///  if layer is on but invisble because of a parent that is off this returns false.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(bool) The current layer visibility.</returns>
    static member LayerVisible(layer:string) : bool = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.IsVisible
        rc
    

    [<Extension>]
    ///<summary>Makes a layer visible.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="forceVisible">(bool) Optional, Default Value: <c>true</c>
    ///     Turn on parent layers too if needed. True by default</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerVisibleSetTrue(layer:string, [<OPT;DEF(true)>]forceVisible:bool) : unit = 
        let lay = RhinoScriptSyntax.CoerceLayer(layer)
        visibleSetTrue(lay,forceVisible)
        Doc.Views.Redraw()
    
    [<Extension>]
    ///<summary>Makes a layer invisible.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="persist">(bool) Optional, Default Value: <c>false</c>
    ///     Turn layer persitently off? even if it is already invisible because of a parent layer that is turned off.
    ///     By default alreaday invisibe layers are not changed</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerVisibleSetFalse(layer:string,  [<OPT;DEF(false)>]persist:bool) : unit = 
        let lay = RhinoScriptSyntax.CoerceLayer(layer)
        visibleSetFalse(lay,persist)
        Doc.Views.Redraw()

    (*
    [<Extension>]
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
            Doc.Layers.ForceLayerVisible(layer.Id) |> ignore
        if not visible && not forcevisibleOrDonotpersist then
            if layer.ParentLayerId <> Guid.Empty then
                layer.SetPersistentVisibility(visible)
            // layer.CommitChanges() |> ignore //obsolete !!
        Doc.Views.Redraw()
    *)
    

    [<Extension>]
    ///<summary>Turn a layer off if visible, does nothing if parent layer is already invisible. .</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerOff(layer:string) : unit = 
        RhinoScriptSyntax.LayerVisibleSetFalse(layer,false)
    
    [<Extension>]
    ///<summary>Turn a layer on if not  visible , enforces visibility  of parents.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerOn(layer:string) : unit = //SET
        RhinoScriptSyntax.LayerVisibleSetTrue(layer, true)
    
    [<Extension>]
    ///<summary>Returns the locked property of a layer, 
    ///  if layer is unlocked but parent layer is locked this still returns true.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(bool) The current layer visibility.</returns>
    static member LayerLocked(layer:string) : bool =       
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.IsLocked //not same as // https://github.com/mcneel/rhinoscriptsyntax/pull/193 // TODO ??
        rc    

    [<Extension>]
    ///<summary>Makes a layer locked.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="forceLocked">(bool) Optional, Default Value: <c>false</c>
    ///     Lock layer even if it is already locked via a parent layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLockedSetTrue(layer:string, [<OPT;DEF(false)>]forceLocked:bool) : unit = 
        let lay = RhinoScriptSyntax.CoerceLayer(layer)
        lockedSetTrue(lay,forceLocked)       
        Doc.Views.Redraw()
    
    [<Extension>]
    ///<summary>Makes a layer unlocked.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="parentsToo">(bool) Optional, Default Value: <c>true</c>
    ///     Unlock parent layers to if needed</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLockedSetFalse(layer:string,  [<OPT;DEF(true)>]parentsToo:bool) : unit = 
        let lay = RhinoScriptSyntax.CoerceLayer(layer)
        lockedSetFalse(lay,parentsToo)                      
        Doc.Views.Redraw()


    [<Extension>]
    ///<summary>Unlocks a layer and all parents if needed .</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerUnlock(layer:string) : unit = 
        RhinoScriptSyntax.LayerLockedSetFalse(layer,true)
    
    [<Extension>]
    ///<summary>Locks a layer if it is not already locked via a parent.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLock(layer:string) : unit = //SET
        RhinoScriptSyntax.LayerLockedSetTrue(layer, false)

    (*
    [<Extension>]
    ///<summary>Changes the locked mode of a layer, enforces presistent locking.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<param name="locked">(bool) New layer locked mode</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerLocked(layer:string, locked:bool) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if layer.ParentLayerId <> Guid.Empty then 
            let l = layer.GetPersistentLocking()
            if l <> locked then 
                layer.SetPersistentLocking(locked)
                Doc.Views.Redraw()
        else
            if locked <> layer.IsLocked then
                layer.IsLocked <- locked 
                Doc.Views.Redraw()
    *)


    [<Extension>]
    ///<summary>Returns the material index of a layer. A material index of -1
    /// indicates that no material has been assigned to the layer. Thus, the layer
    /// will use Rhino's default layer material.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) a zero-based material index.</returns>
    static member LayerMaterialIndex(layer:string) : int = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.RenderMaterialIndex

    [<Extension>]
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
            Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the identifier of a layer given the layer's name.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Guid) The layer's identifier.</returns>
    static member LayerId(layer:string) : Guid =
        let idx = Doc.Layers.FindByFullPath(layer, RhinoMath.UnsetIntIndex)
        if idx = RhinoMath.UnsetIntIndex then RhinoScriptingException.Raise "RhinoScriptSyntax.LayerId not found for name %s" layer
        Doc.Layers.[idx].Id



    [<Extension>]
    ///<summary>Return the name of a layer given it's identifier.</summary>
    ///<param name="layerId">(Guid) Layer identifier</param>
    ///<param name="fullpath">(bool) Optional, Default Value: <c>true</c>
    ///    Return the full path name `True` or short name `False`</param>
    ///<returns>(string) The layer's name.</returns>
    static member LayerName(layerId:Guid, [<OPT;DEF(true)>]fullpath:bool) : string =
        let layer = RhinoScriptSyntax.CoerceLayer(layerId)
        if fullpath then layer.FullPath
        else layer.Name


    [<Extension>]
    ///<summary>Returns the names of all layers in the document.</summary>
    ///<returns>(string Rarr) list of layer names.</returns>
    static member LayerNames() : string Rarr =
        let rc = Rarr()
        for layer in Doc.Layers do
            if not layer.IsDeleted then rc.Add(layer.FullPath)
        rc


    [<Extension>]
    ///<summary>Returns the current display order index of a layer as displayed in Rhino's
    ///    layer dialog box. A display order index of -1 indicates that the current
    ///    layer dialog filter does not allow the layer to appear in the layer list.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(int) zero based index of layer.</returns>
    static member LayerOrder(layer:string) : int =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        layer.SortIndex


    [<Extension>]
    ///<summary>Returns the print color of a layer. Layer print colors are
    /// represented as RGB colors.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(Drawing.Color) The current layer print color.</returns>
    static member LayerPrintColor(layer:string) : Drawing.Color = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotColor
        rc

    [<Extension>]
    ///<summary>Changes the print color of a layer. Layer print colors are
    /// represented as RGB colors.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<param name="color">(Drawing.Color) New print color</param>
    ///<returns>(unit) void, nothing.</returns>
    static member LayerPrintColor(layer:string, color:Drawing.Color) : unit = //SET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotColor
        //color = RhinoScriptSyntax.Coercecolor(color)
        layer.PlotColor <- color
        Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Returns the print width of a layer. Print width is specified
    /// in millimeters. A print width of 0.0 denotes the "default" print width.</summary>
    ///<param name="layer">(string) Name of existing layer</param>
    ///<returns>(float) The current layer print width.</returns>
    static member LayerPrintWidth(layer:string) : float = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = layer.PlotWeight
        rc

    [<Extension>]
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
            Doc.Views.Redraw()



    [<Extension>]
    ///<summary>Return the parent layer of a layer or mepty string if no parent present.</summary>
    ///<param name="layer">(string) Name of an existing layer</param>
    ///<returns>(string) The name of the current parent layer.</returns>
    static member ParentLayer(layer:string) : string = //GET
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        if layer.ParentLayerId = Guid.Empty then ""
        else
            let oldparentlayer = Doc.Layers.FindId(layer.ParentLayerId)
            if isNull oldparentlayer then "" // TODO test! is fail better ?
            else
                oldparentlayer.FullPath


    [<Extension>]
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



    [<Extension>]
    ///<summary>Removes an existing layer from the document. The layer will be removed
    ///    even if it contains geometry objects. The layer to be removed cannot be the
    ///    current layer
    ///    empty.</summary>
    ///<param name="layer">(string) The name of an existing empty layer</param>
    ///<returns>(bool) True or False indicating success or failure.</returns>
    static member PurgeLayer(layer:string) : bool =
        let layer = RhinoScriptSyntax.CoerceLayer(layer)
        let rc = Doc.Layers.Purge( layer.Index, true)
        Doc.Views.Redraw()
        rc


    [<Extension>]
    ///<summary>Renames an existing layer.</summary>
    ///<param name="oldname">(string) Original layer name</param>
    ///<param name="newname">(string) New layer name</param>
    ///<returns>(unit) void, nothing.</returns>
    static member RenameLayer(oldname:string, newname:string) : unit =
        if oldname <> newname then
            let layer = RhinoScriptSyntax.CoerceLayer(oldname)
            layer.Name <- newname // TODO test with bad chars in layer string



