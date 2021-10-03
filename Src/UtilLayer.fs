namespace Rhino

open System
open System.Globalization // for UnicodeCategory
open Rhino.DocObjects


module internal UtilLayer = 

  /// Raise exceptions if short layername is not valid
  /// it may not contain :: or control characters
  /// set allowSpecialChars to false to only have ASCII
  let internal ensureValidShortLayerName(name:string, allowSpecialChars) : unit= 
    if isNull name then RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an null string as layer name"
    if name.Length = 0 then RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an empty string as layer name"

    let tr = name.Trim()
    if tr.Length <> name.Length then
        RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an invalid layer name: '%s'. It has trailing or leading white space" name

    if name.Contains "::" then
        RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an invalid short layer name containing two colons(::)  '%s'. " name

    match Char.GetUnicodeCategory(name.[0]) with
    | UnicodeCategory.OpenPunctuation | UnicodeCategory.ClosePunctuation ->  // { [ ( } ] ) dont work at start of layer name
        RhinoScriptingException.Raise  "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an invalid layer name: '%s'. The name may not start with a '%c' " name name.[0]
    | _ -> ()

    for c in name do
        let cat = Char.GetUnicodeCategory(c)
        match cat with
        |UnicodeCategory.Control  ->
            RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an invalid short layer name containing UnicodeCategory.Control characters: '%s'. " name

        |UnicodeCategory.SpaceSeparator when c <> ' ' ->
            RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an invalid short layer name containing UnicodeCategory.SpaceSeparator other than regular space in '%s'. If you really want this character add the layer directly via Rhinocommon"  name

        |UnicodeCategory.ConnectorPunctuation when c <> '_' ->
            RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an not recommended charcater '%c' in short layer name containing %A  in '%s'. If you really want this character add the layer directly via Rhinocommon" c cat  name

        |UnicodeCategory.DashPunctuation when c <> '-'  ->       // only minus is allowed
            RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an not recommended charcater '%c' in short layer name containing %A  in '%s'. If you really want this character add the layer directly via Rhinocommon" c cat  name

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
            RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found a probably not supported charcater in short layer name containing %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" cat  name

        | _ -> ()


        if not allowSpecialChars then
             match cat with

             (*
             |UnicodeCategory.UppercaseLetter when c > 'Z' -> //always allow ?
                RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name

             |UnicodeCategory.LowercaseLetter when c > 'z' -> //always allow ß ä ö ü and similar
                 RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name

             |UnicodeCategory.CurrencySymbol when c > '¥' -> //Unicode	U+00A5 ¥ YEN SIGN //always allow ?
                 RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name
             *)

             |UnicodeCategory.OtherPunctuation when c > '\\' ->
                 RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name

             |UnicodeCategory.OpenPunctuation when c > '{' ->
                 RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name

             |UnicodeCategory.ClosePunctuation when c > '}' ->
                 RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name

             |UnicodeCategory.MathSymbol when c > '÷' -> //Unicode	U+00F7 ÷ DIVISION SIGN
                 RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name

             |UnicodeCategory.DecimalDigitNumber when c > '9' ->
                 RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name
             |UnicodeCategory.OtherSymbol
             |UnicodeCategory.InitialQuotePunctuation
             |UnicodeCategory.FinalQuotePunctuation
             |UnicodeCategory.OtherLetter ->
                 RhinoScriptingException.Raise "Rhino.Scripting.UtilLayer.ensureValidShortLayerName found an discouraged charcater '%c' in short layer name of category %A. in '%s'. If you really need this character add the layer directly via Rhinocommon" c cat  name

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
          lay.SetPersistentLocking(false) // does noy unlock parents

  type internal LayerState = Off | On | ByParent

    [<Struct>]
    type internal FoundOrCreatedIndex = 
        |LayerCreated of createdIdx :int
        |LayerFound   of foundIdx   :int
  /// Creates all parent layers too if they are missing, uses same locked state and colors for all new layers.
  /// Does not allow ambigous unicode characters in layername.
  /// Returns index
  /// Empty string returns current layer index
  let internal getOrCreateLayer(name, color, visible:LayerState, locked:LayerState) : int = 
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
                              ensureValidShortLayerName (branch, false)   // only check non existing layer names
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
                              layer.Color <- color() // delay creation of (random) color till actually needed ( so random colors are not created, in most cases layer exists)
                              let i = State.Doc.Layers.Add(layer)
                              let id = State.Doc.Layers.[i].Id //just using layer.Id would be empty guid
                              createLayer(rest , id , i,  fullpath)

                          | i ->
                              createLayer(rest , State.Doc.Layers.[i].Id , i ,fullpath)

                  createLayer( ns |> List.ofArray, Guid.Empty, 0, "")
      | i -> i


  /// creates layer with default name
  let internal createDefaultLayer(color, visible, locked) = 
      let layer = DocObjects.Layer.GetDefaultLayerProperties()
      layer.Color <- color() // delay creation of (random) color till actaully neded ( so random colors are not created, in most cases layer exists)
      if layer.ParentLayerId <> Guid.Empty then RhinoScriptingException.Raise "how can a new default layer have a parent ? %A" layer
      layer.IsVisible <- visible
      layer.IsLocked <- locked
      State.Doc.Layers.Add(layer)

