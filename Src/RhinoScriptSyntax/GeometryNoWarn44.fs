namespace Rhino.Scripting
    
open FsEx
open System
open Rhino
open Rhino.Geometry
    
open System.Runtime.CompilerServices // [<Extension>] Attribute not needed for intrinsic (same dll) type augmentations ?
open FsEx.SaveIgnore
open System.Collections.Generic
    

// *** This file has only functions that require a nowarn for deprecated implementations. Is thert a non deprecated implementeation for RhinoCommon 6.0 ??

#if RHINO6   // only for Rh6.0, would not be needed for latest releases of Rh6      
#nowarn "44" // for using Doc.Fonts.FindOrCreate
#endif
    
/// This module is automatically opened when Rhino.Scripting namespace is opened.
/// it only contaions static extension member on RhinoScriptSyntax
[<AutoOpen>]
module ExtensionsGeometryNoWarn44 =
        
  
  type RhinoScriptSyntax with     
    
    
    ///<summary>Modifies the font style of a text object.</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="style">(int) The font style. Can be any of the following flags
    ///    0 = Normal
    ///    1 = Bold
    ///    2 = Italic
    ///    3 = Bold and Italic</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member TextObjectStyle(objectId:Guid, style:int) : unit = //SET
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let fontdata = annotation.Font
        #if RHINO6   // only for Rh6.0, would not be needed for latest releases of Rh6
        let index =
            match style with
            |3 -> State.Doc.Fonts.FindOrCreate(fontdata.FaceName, bold=true , italic=true)
            |2 -> State.Doc.Fonts.FindOrCreate(fontdata.FaceName, bold=false, italic=true)
            |1 -> State.Doc.Fonts.FindOrCreate(fontdata.FaceName, bold=true , italic=false)
            |0 -> State.Doc.Fonts.FindOrCreate(fontdata.FaceName, bold=false, italic=false)
            |_ -> (RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectStyle failed.  objectId:'%s' bad style:%d" (Print.guid objectId) style)
        if index < 0 then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectStyle failed.  objectId:'%s' style:%d not availabe for %s" (Print.guid objectId) style fontdata.FaceName        
        let f = State.Doc.Fonts.[index]
        #else
        let f =          
            match style with
            |3 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=true , italic=true)
            |2 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=false, italic=true)
            |1 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=true , italic=false)
            |0 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=false, italic=false)
            |_ -> (RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectStyle failed.  objectId:'%s' bad style:%d" (Print.guid objectId) style)
        if isNull f then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectStyle failed.  objectId:'%s' style:%d not availabe for %s" (Print.guid objectId) style fontdata.QuartetName
        #endif       
 
        if not <| State.Doc.Objects.Replace(objectId, annotation) then 
            RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectStyle failed.  objectId:'%s' bad style:%d" (Print.guid objectId) style
        State.Doc.Views.Redraw()

    ///<summary>Modifies the font style of multiple text objects. Keeps the font face</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of multiple text objects</param>
    ///<param name="style">(int) The font style. Can be any of the following flags
    ///    0 = Normal
    ///    1 = Bold
    ///    2 = Italic
    ///    3 = Bold and Italic</param>
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member TextObjectStyle(objectIds:Guid seq, style:int) : unit = //MULTISET
        for objectId in objectIds do 
            RhinoScriptSyntax.TextObjectStyle(objectId,style)
        State.Doc.Views.Redraw()


    
    
    ///<summary>Modifies the font used by a text object. Keeps the current state of bold and italic when possible.</summary>
    ///<param name="objectId">(Guid) The identifier of a text object</param>
    ///<param name="font">(string) The new Font Name</param> 
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member TextObjectFont(objectId:Guid, font:string) : unit = //SET        
        let annotation = RhinoScriptSyntax.CoerceTextEntity(objectId)
        let fontdata = annotation.Font
        #if RHINO6   // only for Rh6.0, would not be needed for latest releases of Rh6      
        let index = State.Doc.Fonts.FindOrCreate( font, fontdata.Bold, fontdata.Italic )
        let f = State.Doc.Fonts.[index]
        #else
        let f =
            DocObjects.Font.FromQuartetProperties(font, fontdata.Bold, fontdata.Italic)
            // normally calls will not  go further than FromQuartetProperties(font, false, false)
            // but there are a few rare fonts that don"t have a regular font
            |? DocObjects.Font.FromQuartetProperties(font, bold=false, italic=false)
            |? DocObjects.Font.FromQuartetProperties(font, bold=true , italic=false)
            |? DocObjects.Font.FromQuartetProperties(font, bold=false, italic=true )
            |? DocObjects.Font.FromQuartetProperties(font, bold=true , italic=true )
            |? (RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectFont failed.  objectId:'%s' font:''%s''" (Print.guid objectId) font)
        #endif
        annotation.Font <- f
        if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "RhinoScriptSyntax.TextObjectFont failed.  objectId:'%s' font:''%s''" (Print.guid objectId) font
        State.Doc.Views.Redraw()
    
    ///<summary>Modifies the font used by multiple text objects. Keeps the current state of bold and italic when possible.</summary>
    ///<param name="objectIds">(Guid seq) The identifiers of multiple text objects</param>
    ///<param name="font">(string) The new Font Name</param> 
    ///<returns>(unit) void, nothing.</returns>
    [<Extension>]
    static member TextObjectFont(objectIds:Guid seq, font:string) : unit = //MULTISET        
        for objectId in objectIds do         
            RhinoScriptSyntax.TextObjectFont(objectId, font)