namespace Rhino.Scripting

open System
open Rhino
open Rhino.Geometry
open Rhino.Scripting.Util
open Rhino.Scripting.UtilMath
open Rhino.Scripting.ActiceDocument
[<AutoOpen>]
module ExtensionsView =
    type RhinoScriptSyntax with
    
    [<EXT>]    
    static member  Viewhelper() : obj =
        failNotImpl () // genreation temp disabled !!

   

    [<EXT>]
    ///<summary>Add new detail view to an existing layout view</summary>
    ///<param name="layoutName">(string) Identifier of an existing layout</param>
    ///<param name="corner1">(Point2d) Corner1 of the detail in the layout's unit system</param>
    ///<param name="corner2">(Point2d) Corner2 of the detail in the layout's unit system</param>
    ///<param name="title">(string) Optional, Title of the new detail</param>
    ///<param name="projection">(int) Optional, Default Value: <c>1</c>
    ///Type of initial view projection for the detail
    ///  1 = parallel top view
    ///  2 = parallel bottom view
    ///  3 = parallel left view
    ///  4 = parallel right view
    ///  5 = parallel front view
    ///  6 = parallel back view
    ///  7 = perspective view</param>
    ///<returns>(Guid) identifier of the newly created detail on success</returns>
    static member AddDetail( layoutName:string, 
                             corner1:Point2d, 
                             corner2:Point2d, 
                             [<OPT;DEF(null:string)>]title:string, 
                             [<OPT;DEF(1)>]projection:int) : Guid = 
        if projection<1 || projection>7 then failwithf "Rhino.Scripting: Projection must be a value between 1-7.  layoutId:'%A' corner1:'%A' corner2:'%A' title:'%A' projection:'%A'" layoutName corner1 corner2 title projection
        let layout = RhinoScriptSyntax.CoercePageView(layoutName)//TODO test this
        if isNull layout then failwithf "Rhino.Scripting: No layout found for given layoutId.  layoutId:'%A' corner1:'%A' corner2:'%A' title:'%A' projection:'%A'" layoutName corner1 corner2 title projection
        let projection : Display.DefinedViewportProjection = LanguagePrimitives.EnumOfValue  projection        
        let detail = layout.AddDetailView(title, corner1, corner2, projection)
        if isNull detail then failwithf "Rhino.Scripting: AddDetail failed.  layoutId:'%A' corner1:'%A' corner2:'%A' title:'%A' projection:'%A'" layoutName corner1 corner2 title projection
        Doc.Views.Redraw()
        detail.Id
    (*
    def AddDetail(layout_id, corner1, corner2, title=None, projection=1):
        '''Add new detail view to an existing layout view
        Parameters:
          layout_id (guid): identifier of an existing layout
          corner1, corner2 (point): 2d corners of the detail in the layout's unit system
          title (str, optional): title of the new detail
          projection (number, optional): type of initial view projection for the detail
              1 = parallel top view
              2 = parallel bottom view
              3 = parallel left view
              4 = parallel right view
              5 = parallel front view
              6 = parallel back view
              7 = perspective view
        Returns:
          guid: identifier of the newly created detail on success
          None: on error
        '''
    
        layout_id = rhutil.coerceguid(layout_id, True)
        corner1 = rhutil.coerce2dpoint(corner1, True)
        corner2 = rhutil.coerce2dpoint(corner2, True)
        if projection<1 or projection>7: raise ValueError("projection must be a value between 1-7")
        layout = scriptcontext.doc.Views.Find(layout_id)
        if not layout: raise ValueError("no layout found for given layout_id")
        projection = System.Enum.ToObject(Rhino.Display.DefinedViewportProjection, projection)
        detail = layout.AddDetailView(title, corner1, corner2, projection)
        if not detail: return scriptcontext.errorhandler()
        scriptcontext.doc.Views.Redraw()
        return detail.Id
    *)


