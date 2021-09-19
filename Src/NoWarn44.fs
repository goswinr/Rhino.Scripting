namespace Rhino
open System
open Rhino.Geometry
open FsEx.SaveIgnore
open FsEx // for |? op

// *** This file has only functions that require a nowarn for deprecated implementations. Is there a non deprecated implementeation for RhinoCommon 6.0 ??

#if RHINO6   // only for Rh6.0, would not be needed for latest releases of Rh6
#nowarn "44" // for using Doc.Fonts.FindOrCreate
#endif


module internal NoWarnUtil = 

    let CoerceGeometry(objectId:Guid) : GeometryBase = 
        if Guid.Empty = objectId then RhinoScriptingException.Raise "Rhino.Scripting..CoerceGeometry failed on empty Guid"
        let o = State.Doc.Objects.FindId(objectId)
        if isNull o then RhinoScriptingException.Raise "Rhino.Scripting..CoerceGeometry failed: Guid %A not found in Object table." objectId
        o.Geometry

    let CoerceTextEntity (objectId:Guid) : TextEntity = 
        match CoerceGeometry objectId with
        | :?  TextEntity as a -> a
        | _ -> RhinoScriptingException.Raise "Rhino.Scripting..CoerceTextEntity failed on: %s " (Print.guid objectId)



type internal NoWarnGeometry() = 

    static member TextObjectStyle(objectId:Guid, style:int) : unit = //SET
        let annotation = NoWarnUtil.CoerceTextEntity(objectId)
        let fontdata = annotation.Font
        #if RHINO6   // only for Rh6.0, would not be needed for latest releases of Rh6
        let index = 
            match style with
            |3 -> State.Doc.Fonts.FindOrCreate(fontdata.FaceName, bold=true , italic=true)
            |2 -> State.Doc.Fonts.FindOrCreate(fontdata.FaceName, bold=false, italic=true)
            |1 -> State.Doc.Fonts.FindOrCreate(fontdata.FaceName, bold=true , italic=false)
            |0 -> State.Doc.Fonts.FindOrCreate(fontdata.FaceName, bold=false, italic=false)
            |_ -> (RhinoScriptingException.Raise "Rhino.Scripting..TextObjectStyle failed.  objectId:'%s' bad style:%d" (Print.guid objectId) style)
        if index < 0 then RhinoScriptingException.Raise "Rhino.Scripting..TextObjectStyle failed.  objectId:'%s' style:%d not availabe for %s" (Print.guid objectId) style fontdata.FaceName
        let f = State.Doc.Fonts.[index]
        #else
        let f = 
            match style with
            |3 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=true , italic=true)
            |2 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=false, italic=true)
            |1 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=true , italic=false)
            |0 -> DocObjects.Font.FromQuartetProperties(fontdata.QuartetName, bold=false, italic=false)
            |_ -> (RhinoScriptingException.Raise "Rhino.Scripting..TextObjectStyle failed.  objectId:'%s' bad style:%d" (Print.guid objectId) style)
        if isNull f then RhinoScriptingException.Raise "Rhino.Scripting..TextObjectStyle failed.  objectId:'%s' style:%d not availabe for %s" (Print.guid objectId) style fontdata.QuartetName
        #endif

        if not <| State.Doc.Objects.Replace(objectId, annotation) then
            RhinoScriptingException.Raise "Rhino.Scripting..TextObjectStyle failed.  objectId:'%s' bad style:%d" (Print.guid objectId) style



    static member TextObjectFont(objectId:Guid, font:string) : unit = //SET
        let annotation = NoWarnUtil.CoerceTextEntity(objectId)
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
            |? (RhinoScriptingException.Raise "Rhino.Scripting..TextObjectFont failed.  objectId:'%s' font:''%s''" (Print.guid objectId) font)
        #endif
        annotation.Font <- f
        if not <| State.Doc.Objects.Replace(objectId, annotation) then RhinoScriptingException.Raise "Rhino.Scripting..TextObjectFont failed.  objectId:'%s' font:''%s''" (Print.guid objectId) font
        State.Doc.Views.Redraw()
