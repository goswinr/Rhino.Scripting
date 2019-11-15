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
    static member  Viewhelper() =() //TODO delete

   

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


    [<EXT>]
    ///<summary>Adds a new page layout view</summary>
    ///<param name="title">(string) Optional, Default Value: <c>null:string</c>
    ///Title of new layout</param>
    ///<param name="size">(float * float) Optional, Width and height of paper for the new layout</param>
    ///<returns>(Guid*string) Id and Name of new layout</returns>
    static member AddLayout([<OPT;DEF(null:string)>]title:string, [<OPT;DEF(null)>]size:float * float) : Guid*string =
        let page = 
            if Object.ReferenceEquals(size,null)  then Doc.Views.AddPageView(title)
            else  Doc.Views.AddPageView(title, size|> fst, size|> snd)
        if notNull page then page.MainViewport.Id,page.PageName
        else failwithf "AddLayout failed for %A %A" title size
    (*
    def AddLayout(title=None, size=None):
        '''Adds a new page layout view
        Parameters:
          title (str, optional): title of new layout
          size ([number, number], optional): width and height of paper for the new layout
        Returns:
          guid: id of new layout
        '''
    
        page = None
        if size is None: page = scriptcontext.doc.Views.AddPageView(title)
        else: page = scriptcontext.doc.Views.AddPageView(title, size[0], size[1])
        if page: return page.MainViewport.Id
    *)


    [<EXT>]
    ///<summary>Adds new named construction plane to the document</summary>
    ///<param name="cplaneName">(string) The name of the new named construction plane</param>
    ///<param name="plane">(Plane) The construction plane.</param>
    ///<returns>(unit) void, nothing</returns>
    static member AddNamedCPlane(cplaneName:string, plane:Plane) : unit =
        if isNull cplaneName then failwithf "Rhino.Scripting: CplaneName = null.  cplaneName:'%A' plane:'%A'" cplaneName plane       
        let index = Doc.NamedConstructionPlanes.Add(cplaneName, plane)
        if index<0 then failwithf "Rhino.Scripting: AddNamedCPlane failed.  cplaneName:'%A' plane:'%A'" cplaneName plane
        ()
    (*
    def AddNamedCPlane(cplane_name, view=None):
        '''Adds new named construction plane to the document
        Parameters:
          cplane_name (str): the name of the new named construction plane
          view (guid|str): Title or identifier of the view from which to save
                   the construction plane. If omitted, the current active view is used.
        Returns:
          str: name of the newly created construction plane if successful
          None: on error
        '''
    
        view = __viewhelper(view)
        if not cplane_name: raise ValueError("cplane_name is empty")
        plane = view.MainViewport.ConstructionPlane()
        index = scriptcontext.doc.NamedConstructionPlanes.Add(cplane_name, plane)
        if index<0: return scriptcontext.errorhandler()
        return cplane_name
    *)


    [<EXT>]
    ///<summary>Adds a new named view to the document</summary>
    ///<param name="name">(string) The name of the new named view</param>
    ///<param name="view">(string) Optional, The title or identifier of the view to save. If omitted, the current
    ///  active view is saved</param>
    ///<returns>(unit) void, nothing</returns>
    static member AddNamedView(name:string, [<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        if isNull name then failwithf "Rhino.Scripting: Name = empty.  name:'%A' view:'%A'" name view
        let viewportId = view.MainViewport.Id
        let index = Doc.NamedViews.Add(name, viewportId)
        if index<0 then failwithf "Rhino.Scripting: AddNamedView failed.  name:'%A' view:'%A'" name view
        
    (*
    def AddNamedView(name, view=None):
        '''Adds a new named view to the document
        Parameters:
          name (str): the name of the new named view
          view: (guid|str): the title or identifier of the view to save. If omitted, the current
                active view is saved
        Returns:
          str: name fo the newly created named view if successful
          None: on error
        '''
    
        view = __viewhelper(view)
        if not name: raise ValueError("name is empty")
        viewportId = view.MainViewport.Id
        index = scriptcontext.doc.NamedViews.Add(name, viewportId)
        if index<0: return scriptcontext.errorhandler()
        return name
    *)


    [<EXT>]
    ///<summary>Returns the current detail view in a page layout view</summary>
    ///<param name="layout">(string) Title or identifier of an existing page layout view</param>
    ///<returns>(string option) Option of The name  the current detail view, None if Page is current view</returns>
    static member CurrentDetail(layout:string) : string option = //GET
        let page = RhinoScriptSyntax.CoercePageView(layout)        
        if page.MainViewport.Id = page.ActiveViewport.Id then None
        else  Some  page.ActiveViewport.Name
    (*
    def CurrentDetail(layout, detail=None, return_name=True):
        '''Returns or changes the current detail view in a page layout view
        Parameters:
          layout (str|guid): title or identifier of an existing page layout view
          detail (str|guid, optional): title or identifier the the detail view to set
          return_name (bool, optional): return title if True, else return identifier
        Returns:
          str: if detail is not specified, the title or id of the current detail view
          str: if detail is specified, the title or id of the previous detail view
          None: on error
        '''
    
        layout_id = rhutil.coerceguid(layout)
        page = None
        if layout_id is None: page = scriptcontext.doc.Views.Find(layout, False)
        else: page = scriptcontext.doc.Views.Find(layout_id)
        if page is None: return scriptcontext.errorhandler()
        rc = None
        active_viewport = page.ActiveViewport
        if return_name: rc = active_viewport.Name
        else: rc = active_viewport.Id
        if detail:
            id = rhutil.coerceguid(detail)
            if( (id and id==page.MainViewport.Id) or (id is None and detail==page.MainViewport.Name) ):
                page.SetPageAsActive()
            else:
                if id: page.SetActiveDetail(id)
                else: page.SetActiveDetail(detail, False)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Changes the current detail view in a page layout view</summary>
    ///<param name="layout">(string) Title or identifier of an existing page layout view</param>
    ///<param name="detail">(string)Title or identifier the the detail view to set</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentDetail(layout:string, detail:string) : unit = //SET
        let page = RhinoScriptSyntax.CoercePageView(layout) 
        if layout = detail then page.SetPageAsActive()
        else 
            if not <| page.SetActiveDetail(detail,false) then failwithf "set CurrentDetail failed for %s to %s" layout detail
        Doc.Views.Redraw()
        
    (*
    def CurrentDetail(layout, detail=None, return_name=True):
        '''Returns or changes the current detail view in a page layout view
        Parameters:
          layout (str|guid): title or identifier of an existing page layout view
          detail (str|guid, optional): title or identifier the the detail view to set
          return_name (bool, optional): return title if True, else return identifier
        Returns:
          str: if detail is not specified, the title or id of the current detail view
          str: if detail is specified, the title or id of the previous detail view
          None: on error
        '''
    
        layout_id = rhutil.coerceguid(layout)
        page = None
        if layout_id is None: page = scriptcontext.doc.Views.Find(layout, False)
        else: page = scriptcontext.doc.Views.Find(layout_id)
        if page is None: return scriptcontext.errorhandler()
        rc = None
        active_viewport = page.ActiveViewport
        if return_name: rc = active_viewport.Name
        else: rc = active_viewport.Id
        if detail:
            id = rhutil.coerceguid(detail)
            if( (id and id==page.MainViewport.Id) or (id is None and detail==page.MainViewport.Name) ):
                page.SetPageAsActive()
            else:
                if id: page.SetActiveDetail(id)
                else: page.SetActiveDetail(detail, False)
        scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the currently active view</summary>
    ///<returns>(string) The title of the current view</returns>
    static member CurrentView() : string = //GET
        Doc.Views.ActiveView.MainViewport.Name
        
    (*
    def CurrentView(view=None, return_name=True):
        '''Returns or sets the currently active view
        Parameters:
          view (str|guid): Title or id of the view to set current.
            If omitted, only the title or identifier of the current view is returned
          return_name (bool, optional): If True, then the name, or title, of the view is returned.
            If False, then the identifier of the view is returned
        Returns:
          str: if the title is not specified, the title or id of the current view
          str: if the title is specified, the title or id of the previous current view
          None: on error
        '''
    
        rc = None
        if return_name: rc = scriptcontext.doc.Views.ActiveView.MainViewport.Name
        else: rc = scriptcontext.doc.Views.ActiveView.MainViewport.Id
        if view:
            id = rhutil.coerceguid(view)
            rhview = None
            if id: rhview = scriptcontext.doc.Views.Find(id)
            else: rhview = scriptcontext.doc.Views.Find(view, False)
            if rhview is None: return scriptcontext.errorhandler()
            scriptcontext.doc.Views.ActiveView = rhview
        return rc
    *)

    ///<summary>Sets the currently active view</summary>
    ///<param name="view">(string)Title of the view to set current</param>
    ///<returns>(unit) void, nothing</returns>
    static member CurrentView(view:string) : unit = //SET
        Doc.Views.ActiveView <- RhinoScriptSyntax.CoerceView(view)

    (*
    def CurrentView(view=None, return_name=True):
        '''Returns or sets the currently active view
        Parameters:
          view (str|guid): Title or id of the view to set current.
            If omitted, only the title or identifier of the current view is returned
          return_name (bool, optional): If True, then the name, or title, of the view is returned.
            If False, then the identifier of the view is returned
        Returns:
          str: if the title is not specified, the title or id of the current view
          str: if the title is specified, the title or id of the previous current view
          None: on error
        '''
    
        rc = None
        if return_name: rc = scriptcontext.doc.Views.ActiveView.MainViewport.Name
        else: rc = scriptcontext.doc.Views.ActiveView.MainViewport.Id
        if view:
            id = rhutil.coerceguid(view)
            rhview = None
            if id: rhview = scriptcontext.doc.Views.Find(id)
            else: rhview = scriptcontext.doc.Views.Find(view, False)
            if rhview is None: return scriptcontext.errorhandler()
            scriptcontext.doc.Views.ActiveView = rhview
        return rc
    *)


    [<EXT>]
    ///<summary>Removes a named construction plane from the document</summary>
    ///<param name="name">(string) Name of the construction plane to remove</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteNamedCPlane(name:string) : bool =
        Doc.NamedConstructionPlanes.Delete(name)
    (*
    def DeleteNamedCPlane(name):
        '''Removes a named construction plane from the document
        Parameters:
          name (str): name of the construction plane to remove
        Returns:
          bool: True or False indicating success or failure
        '''
    
        return scriptcontext.doc.NamedConstructionPlanes.Delete(name)
    *)


    [<EXT>]
    ///<summary>Removes a named view from the document</summary>
    ///<param name="name">(string) Name of the named view to remove</param>
    ///<returns>(bool) True or False indicating success or failure</returns>
    static member DeleteNamedView(name:string) : bool =
        Doc.NamedViews.Delete(name)
    (*
    def DeleteNamedView(name):
        '''Removes a named view from the document
        Parameters:
          name (str): name of the named view to remove
        Returns:
          bool: True or False indicating success or failure
        '''
    
        return scriptcontext.doc.NamedViews.Delete(name)
    *)


    [<EXT>]
    ///<summary>Returns the projection locked state of a detail viewport rectangle</summary>
    ///<param name="detailId">(Guid) Identifier of a detail rectangle object</param>
    ///<returns>(bool) the current detail projection locked state</returns>
    static member DetailLock(detailId:Guid) : bool = //GET
        let detail = RhinoScriptSyntax.CoerceDetailView(detailId)       
        detail.IsProjectionLocked
        
    (*
    def DetailLock(detail_id, lock=None):
        '''Returns or modifies the projection locked state of a detail
        Parameters:
          detail_id (guid): identifier of a detail object
          lock (bool, optional) the new lock state
        Returns:
          bool: if lock==None, the current detail projection locked state
          bool: if lock is True or False, the previous detail projection locked state
          None: on error
        '''
    
        detail_id = rhutil.coerceguid(detail_id, True)
        detail = scriptcontext.doc.Objects.FindId(detail_id)
        if not detail: return scriptcontext.errorhandler()
        rc = detail.DetailGeometry.IsProjectionLocked
        if lock is not None and lock!=rc:
            detail.DetailGeometry.IsProjectionLocked = lock
            detail.CommitChanges()
        return rc
    *)

    ///<summary>Modifies the projection locked state of a detail</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<param name="lock">(bool)The new lock state</param>
    ///<returns>(unit) void, nothing</returns>
    static member DetailLock(detailId:Guid, lock:bool) : unit = //SET
        let detail = 
            try Doc.Objects.FindId(detailId) :?> DocObjects.DetailViewObject
            with _ ->  failwithf "Rhino.Scripting: Set DetailLock failed. detailId is a %s '%A' lock:'%A'" (rhtype detailId) detailId lock        
        if lock <> detail.DetailGeometry.IsProjectionLocked then
            detail.DetailGeometry.IsProjectionLocked <- lock
            detail.CommitChanges() |> ignore
        
    (*
    def DetailLock(detail_id, lock=None):
        '''Returns or modifies the projection locked state of a detail
        Parameters:
          detail_id (guid): identifier of a detail object
          lock (bool, optional) the new lock state
        Returns:
          bool: if lock==None, the current detail projection locked state
          bool: if lock is True or False, the previous detail projection locked state
          None: on error
        '''
    
        detail_id = rhutil.coerceguid(detail_id, True)
        detail = scriptcontext.doc.Objects.FindId(detail_id)
        if not detail: return scriptcontext.errorhandler()
        rc = detail.DetailGeometry.IsProjectionLocked
        if lock is not None and lock!=rc:
            detail.DetailGeometry.IsProjectionLocked = lock
            detail.CommitChanges()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the scale of a detail object</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<returns>(float) current page to model scale ratio if model_length and page_length are both None</returns>
    static member DetailScale(detailId:Guid) : float = //GET
        let detail = RhinoScriptSyntax.CoerceDetailViewObject(detailId)
        detail.DetailGeometry.PageToModelRatio
        
    (*
    def DetailScale(detail_id, model_length=None, page_length=None):
        '''Returns or modifies the scale of a detail object
        Parameters:
          detail_id (guid): identifier of a detail object
          model_length (number, optional): a length in the current model units
          page_length (number, optional): a length in the current page units
        Returns:
          number: current page to model scale ratio if model_length and page_length are both None
          number: previous page to model scale ratio if model_length and page_length are values
          None: on error
        '''
    
        detail_id = rhutil.coerceguid(detail_id, True)
        detail = scriptcontext.doc.Objects.FindId(detail_id)
        if detail is None: return scriptcontext.errorhandler()
        rc = detail.DetailGeometry.PageToModelRatio
        if model_length or page_length:
            if model_length is None or page_length is None:
                return scriptcontext.errorhandler()
            model_units = scriptcontext.doc.ModelUnitSystem
            page_units = scriptcontext.doc.PageUnitSystem
            if detail.DetailGeometry.SetScale(model_length, model_units, page_length, page_units):
                detail.CommitChanges()
                scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Modifies the scale of a detail object</summary>
    ///<param name="detailId">(Guid) Identifier of a detail object</param>
    ///<param name="modelLength">(float)A length in the current model units</param>
    ///<param name="pageLength">(float)A length in the current page units</param>
    ///<returns>(unit) void, nothing</returns>
    static member DetailScale(detailId:Guid, modelLength:float,pageLength:float) : unit = //SET
        let detail = RhinoScriptSyntax.CoerceDetailViewObject(detailId)        
        let modelunits = Doc.ModelUnitSystem
        let pageunits = Doc.PageUnitSystem
        if detail.DetailGeometry.SetScale(modelLength, modelunits, pageLength, pageunits) then
            detail.CommitChanges()|> ignore
            Doc.Views.Redraw()
        else 
            failwithf "Rhino.Scripting: DetailScale failed.  detailId:'%A' modelLength:'%A' pageLength:'%A'" detailId modelLength pageLength
            
    (*
    def DetailScale(detail_id, model_length=None, page_length=None):
        '''Returns or modifies the scale of a detail object
        Parameters:
          detail_id (guid): identifier of a detail object
          model_length (number, optional): a length in the current model units
          page_length (number, optional): a length in the current page units
        Returns:
          number: current page to model scale ratio if model_length and page_length are both None
          number: previous page to model scale ratio if model_length and page_length are values
          None: on error
        '''
    
        detail_id = rhutil.coerceguid(detail_id, True)
        detail = scriptcontext.doc.Objects.FindId(detail_id)
        if detail is None: return scriptcontext.errorhandler()
        rc = detail.DetailGeometry.PageToModelRatio
        if model_length or page_length:
            if model_length is None or page_length is None:
                return scriptcontext.errorhandler()
            model_units = scriptcontext.doc.ModelUnitSystem
            page_units = scriptcontext.doc.PageUnitSystem
            if detail.DetailGeometry.SetScale(model_length, model_units, page_length, page_units):
                detail.CommitChanges()
                scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Verifies that a detail view exists on a page layout view</summary>
    ///<param name="layout">(string) Title of an existing page layout</param>
    ///<param name="detail">(string) Title of an existing detail view</param>
    ///<returns>(bool) True if detail is a detail view, False if detail is not a detail view</returns>
    static member IsDetail(layout:string, detail:string) : bool =
        let view = RhinoScriptSyntax.CoercePageView(layout)
        let det = RhinoScriptSyntax.CoerceView(detail)
        view.GetDetailViews()
        |> Array.exists (fun v -> v.Name = det.ActiveViewport.Name) // TODO test
    (*
    def IsDetail(layout, detail):
        '''Verifies that a detail view exists on a page layout view
        Parameters:
          layout (str|guid): title or identifier of an existing page layout
          detail (str|guid): title or identifier of an existing detail view
        Returns:
          bool: True if detail is a detail view, False if detail is not a detail view
          None: on error
        '''
    
        layout_id = rhutil.coerceguid(layout)
        views = scriptcontext.doc.Views.GetViewList(False, True)
        found_layout = None
        for view in views:
            if layout_id:
                if view.MainViewport.Id==layout_id:
                    found_layout = view
                    break
            elif view.MainViewport.Name==layout:
                found_layout = view
                break
        # if we couldn't find a layout, this is an error
        if found_layout is None: return scriptcontext.errorhandler()
        detail_id = rhutil.coerceguid(detail)
        details = view.GetDetailViews()
        if not details: return False
        for detail_view in details:
            if detail_id:
                if detail_view.Id==detail_id: return True
            else:
                if detail_view.Name==detail: return True
        return False
    *)


    [<EXT>]
    ///<summary>Verifies that a view is a page layout view</summary>
    ///<param name="layout">(string) Title of an existing page layout view</param>
    ///<returns>(bool) True if layout is a page layout view, False is layout is a standard model view</returns>
    static member IsLayout(layout:string) : bool =
        //layoutid = RhinoScriptSyntax.Coerceguid(layout)
        if   Doc.Views.GetViewList(false, true) |> Array.exists ( fun v -> v.MainViewport.Name = layout) then true
        elif Doc.Views.GetViewList(true, false) |> Array.exists ( fun v -> v.MainViewport.Name = layout) then false
        else failwithf "Rhino.Scripting: IsLayout View does not exist at all.  layout:'%A'" layout // or false
    (*
    def IsLayout(layout):
        '''Verifies that a view is a page layout view
        Parameters:
          layout (guid|str): title or identifier of an existing page layout view
        Returns:
          bool: True if layout is a page layout view, False is layout is a standard model view
          None: on error
        '''
    
        layout_id = rhutil.coerceguid(layout)
        alllayouts = scriptcontext.doc.Views.GetViewList(False, True)
        for layoutview in alllayouts:
            if layout_id:
                if layoutview.MainViewport.Id==layout_id: return True
            elif layoutview.MainViewport.Name==layout: return True
        allmodelviews = scriptcontext.doc.Views.GetViewList(True, False)
        for modelview in allmodelviews:
            if layout_id:
              if modelview.MainViewport.Id==layout_id: return False
            elif modelview.MainViewport.Name==layout: return False
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Verifies that the specified view exists</summary>
    ///<param name="view">(string) Title of the view</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member IsView(view:string) : bool =
        Doc.Views.GetViewList(false, true) |> Array.exists ( fun v -> v.MainViewport.Name = view) 
        
    (*
    def IsView(view):
        '''Verifies that the specified view exists
        Parameters:
          view (str|guid): title or identifier of the view
        Returns:
          bool: True of False indicating success or failure
        '''
    
        view_id = rhutil.coerceguid(view)
        if view_id is None and view is None: return False
        allviews = scriptcontext.doc.Views.GetViewList(True, True)
        for item in allviews:
            if view_id:
                if item.MainViewport.Id==view_id: return True
            elif item.MainViewport.Name==view: return True
        return False
    *)


    [<EXT>]
    ///<summary>Verifies that the specified view is the current, or active view</summary>
    ///<param name="view">(string) Title of the view</param>
    ///<returns>(bool) True of False indicating success or failure</returns>
    static member IsViewCurrent(view:string) : bool =
        let activeview = Doc.Views.ActiveView
        view = activeview.MainViewport.Name
    (*
    def IsViewCurrent(view):
        '''Verifies that the specified view is the current, or active view
        Parameters:
          view (str|guid): title or identifier of the view
        Returns:
          bool: True of False indicating success or failure
        '''
    
        activeview = scriptcontext.doc.Views.ActiveView
        view_id = rhutil.coerceguid(view)
        if view_id: return view_id==activeview.MainViewport.Id
        return view==activeview.MainViewport.Name
    *)


    [<EXT>]
    ///<summary>Verifies that the specified view is maximized (enlarged so as to fill
    ///  the entire Rhino window)</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null:string</c>
    ///Title or identifier of the view. If omitted, the current
    ///  view is used</param>
    ///<returns>(bool) True of False</returns>
    static member IsViewMaximized([<OPT;DEF("":string)>]view:string) : bool =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.Maximized
    (*
    def IsViewMaximized(view=None):
        '''Verifies that the specified view is maximized (enlarged so as to fill
        the entire Rhino window)
        Parameters:
          view: (str|guid): title or identifier of the view. If omitted, the current
                view is used
        Returns:
          bool: True of False
        '''
    
        view = __viewhelper(view)
        return view.Maximized
    *)


    [<EXT>]
    ///<summary>Verifies that the specified view's projection is set to perspective</summary>
    ///<param name="view">(string) Title or identifier of the view</param>
    ///<returns>(bool) True of False</returns>
    static member IsViewPerspective(view:string) : bool =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.MainViewport.IsPerspectiveProjection
    (*
    def IsViewPerspective(view):
        '''Verifies that the specified view's projection is set to perspective
        Parameters:
          view (str|guid): title or identifier of the view
        Returns:
          bool: True of False
        '''
    
        view = __viewhelper(view)
        return view.MainViewport.IsPerspectiveProjection
    *)


    [<EXT>]
    ///<summary>Verifies that the specified view's title window is visible</summary>
    ///<param name="view">(string) Optional, The title or identifier of the view. If omitted, the current
    ///  active view is used</param>
    ///<returns>(bool) True of False</returns>
    static member IsViewTitleVisible([<OPT;DEF("":string)>]view:string) : bool =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.TitleVisible
    (*
    def IsViewTitleVisible(view=None):
        '''Verifies that the specified view's title window is visible
        Parameters:
          view: (str|guid, optional): The title or identifier of the view. If omitted, the current
                active view is used
        Returns:
          bool: True of False
        '''
    
        view = __viewhelper(view)
        return view.MainViewport.TitleVisible
    *)


    [<EXT>]
    ///<summary>Verifies that the specified view contains a wallpaper image</summary>
    ///<param name="view">(string) View to verify</param>
    ///<returns>(bool) True or False</returns>
    static member IsWallpaper(view:string) : bool =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.MainViewport.WallpaperFilename.Length > 0
    (*
    def IsWallpaper(view):
        '''Verifies that the specified view contains a wallpaper image
        Parameters:
          view (str|guid): view to verify
        Returns:
          bool: True or False
        '''
    
        view = __viewhelper(view)
        return len(view.MainViewport.WallpaperFilename)>0
    *)


    [<EXT>]
    ///<summary>Toggles a view's maximized/restore window state of the specified view</summary>
    ///<param name="view">(string) Optional, The title or identifier of the view. If omitted, the current
    ///  active view is used</param>
    ///<returns>(unit) </returns>
    static member MaximizeRestoreView([<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        view.Maximized <- not view.Maximized
    (*
    def MaximizeRestoreView(view=None):
        '''Toggles a view's maximized/restore window state of the specified view
        Parameters:
          view: (str|guid, optional): the title or identifier of the view. If omitted, the current
                active view is used
        Returns:
          None
        '''
    
        view = __viewhelper(view)
        view.Maximized = not view.Maximized
    *)


    [<EXT>]
    ///<summary>Returns the plane geometry of the specified named construction plane</summary>
    ///<param name="name">(string) The name of the construction plane</param>
    ///<returns>(Plane) a plane on success</returns>
    static member NamedCPlane(name:string) : Plane =
        let index = Doc.NamedConstructionPlanes.Find(name)
        if index<0 then failwithf "Rhino.Scripting: NamedCPlane failed.  name:'%A'" name
        Doc.NamedConstructionPlanes.[index].Plane
    (*
    def NamedCPlane(name):
        '''Returns the plane geometry of the specified named construction plane
        Parameters:
          name (str): the name of the construction plane
        Returns:
          plane: a plane on success
          None: on error
        '''
    
        index = scriptcontext.doc.NamedConstructionPlanes.Find(name)
        if index<0: return scriptcontext.errorhandler()
        return scriptcontext.doc.NamedConstructionPlanes[index].Plane
    *)


    [<EXT>]
    ///<summary>Returns the names of all named construction planes in the document</summary>
    ///<returns>(string ResizeArray) the names of all named construction planes in the document</returns>
    static member NamedCPlanes() : string ResizeArray =
        let count = Doc.NamedConstructionPlanes.Count
        resizeArray {for i in range(count) do Doc.NamedConstructionPlanes.[i].Name }
        
    (*
    def NamedCPlanes():
        '''Returns the names of all named construction planes in the document
        Returns:
          list(str, ...): the names of all named construction planes in the document
        '''
    
        count = scriptcontext.doc.NamedConstructionPlanes.Count
        rc = [scriptcontext.doc.NamedConstructionPlanes[i].Name for i in range(count)]
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the names of all named views in the document</summary>
    ///<returns>(string ResizeArray) the names of all named views in the document</returns>
    static member NamedViews() : string ResizeArray =
        let count = Doc.NamedViews.Count
        resizeArray {for i in range(count) do Doc.NamedViews.[i].Name }
    (*
    def NamedViews():
        '''Returns the names of all named views in the document
        Returns:
          list(str, ...): the names of all named views in the document
        '''
    
        count = scriptcontext.doc.NamedViews.Count
        return [scriptcontext.doc.NamedViews[i].Name for i in range(count)]
    *)


    [<EXT>]
    ///<summary>Changes the title of the specified view</summary>
    ///<param name="oldTitle">(string) The title of the view to rename</param>
    ///<param name="newTitle">(string) The new title of the view</param>
    ///<returns>(unit) void, nothing</returns>
    static member RenameView(oldTitle:string, newTitle:string) : unit =
        if isNull oldTitle || isNull newTitle then failwithf "Rhino.Scripting: RenameView failed.  oldTitle:'%A' newTitle:'%A'" oldTitle newTitle        
        let foundview = RhinoScriptSyntax.CoerceView(oldTitle)        
        foundview.MainViewport.Name <- newTitle
        
    (*
    def RenameView(old_title, new_title):
        '''Changes the title of the specified view
        Parameters:
          old_title (str|guid): the title or identifier of the view to rename
          new_title (str): the new title of the view
        Returns:
          str: the view's previous title if successful
          None: on error
        '''
    
        if not old_title or not new_title: return scriptcontext.errorhandler()
        old_id = rhutil.coerceguid(old_title)
        foundview = None
        allviews = scriptcontext.doc.Views.GetViewList(True, True)
        for view in allviews:
            if old_id:
                if view.MainViewport.Id==old_id:
                    foundview = view
                    break
            elif view.MainViewport.Name==old_title:
                foundview = view
                break
        if foundview is None: return scriptcontext.errorhandler()
        old_title = foundview.MainViewport.Name
        foundview.MainViewport.Name = new_title
        return old_title
    *)


    [<EXT>]
    ///<summary>Restores a named construction plane to the specified view.</summary>
    ///<param name="cplaneName">(string) Name of the construction plane to restore</param>
    ///<param name="view">(string) Optional, The title of the view. If omitted, the current
    ///  active view is used</param>
    ///<returns>(string) name of the restored named construction plane</returns>
    static member RestoreNamedCPlane(cplaneName:string, [<OPT;DEF("":string)>]view:string) : string =
        let view = RhinoScriptSyntax.CoerceView(view)
        let index = Doc.NamedConstructionPlanes.Find(cplaneName)
        if index<0 then failwithf "Rhino.Scripting: RestoreNamedCPlane failed.  cplaneName:'%A' view:'%A'" cplaneName view
        let cplane = Doc.NamedConstructionPlanes.[index]
        view.MainViewport.PushConstructionPlane(cplane)
        view.Redraw()
        cplaneName
    (*
    def RestoreNamedCPlane(cplane_name, view=None):
        '''Restores a named construction plane to the specified view.
        Parameters:
          cplane_name (str): name of the construction plane to restore
          view: (str|guid, optional): the title or identifier of the view. If omitted, the current
                active view is used
        Returns:
          str: name of the restored named construction plane if successful
          None: on error
        '''
    
        view = __viewhelper(view)
        index = scriptcontext.doc.NamedConstructionPlanes.Find(cplane_name)
        if index<0: return scriptcontext.errorhandler()
        cplane = scriptcontext.doc.NamedConstructionPlanes[index]
        view.MainViewport.PushConstructionPlane(cplane)
        view.Redraw()
        return cplane_name
    *)


    [<EXT>]
    ///<summary>Restores a named view to the specified view</summary>
    ///<param name="namedView">(string) Name of the named view to restore</param>
    ///<param name="view">(string) Optional, Title of the view to restore the named view.
    ///  If omitted, the current active view is used</param>
    ///<param name="restoreBitmap">(bool) Optional, Default Value: <c>false</c>
    ///Restore the named view's background bitmap</param>
    ///<returns>(unit) void, nothing</returns>
    static member RestoreNamedView( namedView:string, 
                                    [<OPT;DEF("":string)>]view:string, 
                                    [<OPT;DEF(false)>]restoreBitmap:bool) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        let index = Doc.NamedViews.FindByName(namedView)
        if index<0 then failwithf "Rhino.Scripting: RestoreNamedView failed.  namedView:'%A' view:'%A' restoreBitmap:'%A'" namedView view restoreBitmap
        let viewinfo = Doc.NamedViews.[index]
        if view.MainViewport.PushViewInfo(viewinfo, restoreBitmap) then
            view.Redraw()
            //view.MainViewport.Name
        else
            failwithf "Rhino.Scripting: RestoreNamedView failed.  namedView:'%A' view:'%A' restoreBitmap:'%A'" namedView view restoreBitmap
    (*
    def RestoreNamedView(named_view, view=None, restore_bitmap=False):
        '''Restores a named view to the specified view
        Parameters:
          named_view (str): name of the named view to restore
          view (str|guid, optional):  title or id of the view to restore the named view.
               If omitted, the current active view is used
          restore_bitmap: (bool, optional): restore the named view's background bitmap
        Returns:
          str: name of the restored view if successful
          None: on error
        '''
    
        view = __viewhelper(view)
        index = scriptcontext.doc.NamedViews.FindByName(named_view)
        if index<0: return scriptcontext.errorhandler()
        viewinfo = scriptcontext.doc.NamedViews[index]
        if view.MainViewport.PushViewInfo(viewinfo, restore_bitmap):
            view.Redraw()
            return view.MainViewport.Name
        return scriptcontext.errorhandler()
    *)


    [<EXT>]
    ///<summary>Rotates a perspective-projection view's camera. See the RotateCamera
    ///  command in the Rhino help file for more details</summary>
    ///<param name="direction">(int) 
    ///The direction to rotate the camera where
    ///  0=right
    ///  1=left
    ///  2=down
    ///  3=up</param>
    ///<param name="angle">(float) The angle to rotate.</param>
    ///<param name="view">(string) Optional, Title or id of the view. If omitted, current active view is used</param>
    ///<returns>(unit) void, nothing</returns>
    static member RotateCamera( direction:int, 
                                angle:float,
                                [<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable angle = Rhino.RhinoMath.ToRadians( abs(angle) )
        let targetdistance = (viewport.CameraLocation-viewport.CameraTarget)*viewport.CameraZ
        let mutable axis = viewport.CameraY
        if direction = 0 || direction = 2 then angle <- -angle
        if direction = 0 || direction = 1 then
            if Rhino.ApplicationSettings.ViewSettings.RotateToView then
                axis <- viewport.CameraY
            else 
                axis <- Vector3d.ZAxis
        elif direction = 2 || direction = 3 then
            axis <- viewport.CameraX
        
        if Rhino.ApplicationSettings.ViewSettings.RotateReverseKeyboard then angle<- -angle
        let rot = Transform.Rotation(angle, axis, Point3d.Origin)
        let camUp = rot * viewport.CameraY
        let camDir = -(rot * viewport.CameraZ)
        let target = viewport.CameraLocation + targetdistance*camDir
        viewport.SetCameraLocations(target, viewport.CameraLocation)
        viewport.CameraUp <- camUp
        view.Redraw()
        
    (*
    def RotateCamera(view=None, direction=0, angle=None):
        '''Rotates a perspective-projection view's camera. See the RotateCamera
        command in the Rhino help file for more details
        Parameters:
          view (str|guid, optional):  title or id of the view. If omitted, current active view is used
          direction(number, optional): the direction to rotate the camera where
            0=right
            1=left
            2=down
            3=up
          angle: (number, optional): the angle to rotate. If omitted, the angle of rotation
                is specified by the "Increment in divisions of a circle" parameter
                specified in Options command's View tab
        Returns:
          bool: True or False indicating success or failure
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        if angle is None:
            angle = 2.0*math.pi/Rhino.ApplicationSettings.ViewSettings.RotateCircleIncrement
        else:
            angle = Rhino.RhinoMath.ToRadians( abs(angle) )
        target_distance = (viewport.CameraLocation-viewport.CameraTarget)*viewport.CameraZ
        axis = viewport.CameraY
        if direction==0 or direction==2: angle=-angle
        if direction==0 or direction==1:
            if Rhino.ApplicationSettings.ViewSettings.RotateToView:
                axis = viewport.CameraY
            else:
                axis = Rhino.Geometry.Vector3d.ZAxis
        elif direction==2 or direction==3:
            axis = viewport.CameraX
        else:
            return False
        if Rhino.ApplicationSettings.ViewSettings.RotateReverseKeyboard: angle=-angle
        rot = Rhino.Geometry.Transform.Rotation(angle, axis, Rhino.Geometry.Point3d.Origin)
        camUp = rot * viewport.CameraY
        camDir = -(rot * viewport.CameraZ)
        target = viewport.CameraLocation + target_distance*camDir
        viewport.SetCameraLocations(target, viewport.CameraLocation)
        viewport.CameraUp = camUp
        view.Redraw()
        return True
    *)


    [<EXT>]
    ///<summary>Rotates a view. See RotateView command in Rhino help for more information</summary>
    ///<param name="view">(string) Optional, Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="direction">(int) Optional, The direction to rotate the view where
    ///  0=right
    ///  1=left
    ///  2=down
    ///  3=up</param>
    ///<param name="angle">(float) Angle to rotate. If omitted, the angle of rotation is specified
    ///  by the "Increment in divisions of a circle" parameter specified in
    ///  Options command's View tab</param>
    ///<returns>(unit) void, nothing</returns>
    static member RotateView( direction:int, 
                              angle:float,
                              [<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable angle =  Rhino.RhinoMath.ToRadians( abs(angle) )
        if Rhino.ApplicationSettings.ViewSettings.RotateReverseKeyboard then angle <- -angle
        if direction = 0 then viewport.KeyboardRotate(true, angle)       |> ignore
        elif direction = 1 then viewport.KeyboardRotate(true, -angle)    |> ignore
        elif direction = 2 then viewport.KeyboardRotate(false, -angle)   |> ignore
        elif direction = 3 then viewport.KeyboardRotate(false, angle)    |> ignore    
        view.Redraw()
    (*
    def RotateView(view=None, direction=0, angle=None):
        '''Rotates a view. See RotateView command in Rhino help for more information
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          direction (number, optional): the direction to rotate the view where
                0=right
                1=left
                2=down
                3=up
          angle (number): angle to rotate. If omitted, the angle of rotation is specified
                by the "Increment in divisions of a circle" parameter specified in
                Options command's View tab
        Returns:
          bool: True or False indicating success or failure
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        if angle is None:
            angle = 2.0*math.pi/Rhino.ApplicationSettings.ViewSettings.RotateCircleIncrement
        else:
            angle = Rhino.RhinoMath.ToRadians( abs(angle) )
        if Rhino.ApplicationSettings.ViewSettings.RotateReverseKeyboard: angle = -angle
        if direction==0: viewport.KeyboardRotate(True, angle)
        elif direction==1: viewport.KeyboardRotate(True, -angle)
        elif direction==2: viewport.KeyboardRotate(False, -angle)
        elif direction==3: viewport.KeyboardRotate(False, angle)
        else: return False
        view.Redraw()
        return True
    *)


    [<EXT>]
    ///<summary>Get status of a view's construction plane grid</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(bool) The grid display state</returns>
    static member ShowGrid(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        viewport.ConstructionGridVisible
    (*
    def ShowGrid(view=None, show=None):
        '''Shows or hides a view's construction plane grid
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          show (bool, optional): The grid state to set. If omitted, the current grid display state is returned
        Returns:
          bool: If show is not specified, then the grid display state if successful
          bool: If show is specified, then the previous grid display state if successful
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        rc = viewport.ConstructionGridVisible
        if show is not None and rc!=show:
            viewport.ConstructionGridVisible = show
            view.Redraw()
        return rc
    *)

    ///<summary>Shows or hides a view's construction plane grid</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="show">(bool)The grid state to set. If omitted, the current grid display state is returned</param>
    ///<returns>(unit) void, nothing</returns>
    static member ShowGrid(view:string, show:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.ConstructionGridVisible
        if  rc <> show then
            viewport.ConstructionGridVisible <- show
            view.Redraw()
        
    (*
    def ShowGrid(view=None, show=None):
        '''Shows or hides a view's construction plane grid
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          show (bool, optional): The grid state to set. If omitted, the current grid display state is returned
        Returns:
          bool: If show is not specified, then the grid display state if successful
          bool: If show is specified, then the previous grid display state if successful
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        rc = viewport.ConstructionGridVisible
        if show is not None and rc!=show:
            viewport.ConstructionGridVisible = show
            view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Get status of a view's construction plane grid axes.</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(bool) The grid axes display state</returns>
    static member ShowGridAxes(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.ConstructionAxesVisible
        rc
    (*
    def ShowGridAxes(view=None, show=None):
        '''Shows or hides a view's construction plane grid axes.
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          show (bool, optional): The state to set. If omitted, the current grid axes display state is returned
        Returns:
          bool: If show is not specified, then the grid axes display state
          bool: If show is specified, then the previous grid axes display state
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        rc = viewport.ConstructionAxesVisible
        if show is not None and rc!=show:
            viewport.ConstructionAxesVisible = show
            view.Redraw()
        return rc
    *)

    ///<summary>Shows or hides a view's construction plane grid axes.</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="show">(bool)The state to set. If omitted, the current grid axes display state is returned</param>
    ///<returns>(unit) void, nothing</returns>
    static member ShowGridAxes(view:string, show:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.ConstructionAxesVisible
        if  rc <> show then
            viewport.ConstructionAxesVisible <- show
            view.Redraw()
        
    (*
    def ShowGridAxes(view=None, show=None):
        '''Shows or hides a view's construction plane grid axes.
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          show (bool, optional): The state to set. If omitted, the current grid axes display state is returned
        Returns:
          bool: If show is not specified, then the grid axes display state
          bool: If show is specified, then the previous grid axes display state
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        rc = viewport.ConstructionAxesVisible
        if show is not None and rc!=show:
            viewport.ConstructionAxesVisible = show
            view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Get status of the title window of a view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(unit) </returns>
    static member ShowViewTitle(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)        
        view.TitleVisible 
    (*
    def ShowViewTitle(view=None, show=True):
        '''Shows or hides the title window of a view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          show (bool, optional): The state to set.
        Returns:
          None
        '''
    
        view = __viewhelper(view)
        if view is None: return scriptcontext.errorhandler()
        view.TitleVisible = show
    *)

    ///<summary>Shows or hides the title window of a view</summary>
    ///<param name="view">(string)Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="show">(bool)The state to set.</param>
    static member ShowViewTitle(view:string, show:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)        
        view.TitleVisible <- show
    (*
    def ShowViewTitle(view=None, show=True):
        '''Shows or hides the title window of a view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          show (bool, optional): The state to set.
        Returns:
          None
        '''
    
        view = __viewhelper(view)
        if view is None: return scriptcontext.errorhandler()
        view.TitleVisible = show
    *)


    [<EXT>]
    ///<summary>Get status of a view's world axis icon</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(bool) The world axes display state</returns>
    static member ShowWorldAxes(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.WorldAxesVisible
        rc
    (*
    def ShowWorldAxes(view=None, show=None):
        '''Shows or hides a view's world axis icon
        Parameters:
          view (str|guid, optional):  title or id of the view. If omitted, the current active view is used
          show: (bool, optional): The state to set.
        Returns:
          bool: If show is not specified, then the world axes display state
          bool: If show is specified, then the previous world axes display state
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        rc = viewport.WorldAxesVisible
        if show is not None and rc!=show:
            viewport.WorldAxesVisible = show
            view.Redraw()
        return rc
    *)

    ///<summary>Shows or hides a view's world axis icon</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="show">(bool)The state to set.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ShowWorldAxes(view:string, show:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let rc = viewport.WorldAxesVisible
        if rc <> show then
            viewport.WorldAxesVisible <- show
            view.Redraw()
        
    (*
    def ShowWorldAxes(view=None, show=None):
        '''Shows or hides a view's world axis icon
        Parameters:
          view (str|guid, optional):  title or id of the view. If omitted, the current active view is used
          show: (bool, optional): The state to set.
        Returns:
          bool: If show is not specified, then the world axes display state
          bool: If show is specified, then the previous world axes display state
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        rc = viewport.WorldAxesVisible
        if show is not None and rc!=show:
            viewport.WorldAxesVisible = show
            view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Tilts a view by rotating the camera up vector. See the TiltView command in
    ///  the Rhino help file for more details.</summary>
    ///<param name="view">(string) Optional, Title or id of the view. If omitted, the current active view is used</param>
    ///<param name="direction">(int) The direction to rotate the view where
    ///  0=right
    ///  1=left</param>
    ///<param name="angle">(float) The angle in degrees to rotate</param>
    ///<returns>(unit) void, nothing</returns>
    static member TiltView( direction:int, 
                            angle:float,
                            [<OPT;DEF("":string)>]view:string) : unit =
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport        
        let mutable angle = angle
        if Rhino.ApplicationSettings.ViewSettings.RotateReverseKeyboard then angle <- -angle
        let axis = viewport.CameraLocation - viewport.CameraTarget
        if direction = 0 then viewport.Rotate(angle, axis, viewport.CameraLocation) |> ignore
        elif direction = 1 then viewport.Rotate(-angle, axis, viewport.CameraLocation)   |> ignore     
        view.Redraw()
        
    (*
    def TiltView(view=None, direction=0, angle=None):
        '''Tilts a view by rotating the camera up vector. See the TiltView command in
        the Rhino help file for more details.
        Parameters:
          view (str|guid, optional):  title or id of the view. If omitted, the current active view is used
          direction (number, optional): the direction to rotate the view where
            0=right
            1=left
          angle (number, optional): the angle to rotate. If omitted, the angle of rotation is
            specified by the "Increment in divisions of a circle" parameter specified
            in Options command's View tab
        Returns:
          bool: True or False indicating success or failure
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        if angle is None:
            angle = 2.0*math.pi/Rhino.ApplicationSettings.ViewSettings.RotateCircleIncrement
        else:
            angle = Rhino.RhinoMath.ToRadians( abs(angle) )
    
        if Rhino.ApplicationSettings.ViewSettings.RotateReverseKeyboard: angle = -angle
        axis = viewport.CameraLocation - viewport.CameraTarget
        if direction==0: viewport.Rotate(angle, axis, viewport.CameraLocation)
        elif direction==1: viewport.Rotate(-angle, axis, viewport.CameraLocation)
        else: return False
        view.Redraw()
        return True
    *)


    [<EXT>]
    ///<summary>Returns the camera location of the specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(Point3d) The current camera location</returns>
    static member ViewCamera(view:string) : Point3d = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.CameraLocation
        
    (*
    def ViewCamera(view=None, camera_location=None):
        '''Returns or sets the camera location of the specified view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          camera_location (point, optional): a 3D point identifying the new camera location.
            If omitted, the current camera location is returned
        Returns:
          point: If camera_location is not specified, the current camera location
          point: If camera_location is specified, the previous camera location
          None: on error
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.CameraLocation
        if camera_location is None: return rc
        camera_location = rhutil.coerce3dpoint(camera_location)
        if camera_location is None: return scriptcontext.errorhandler()
        view.ActiveViewport.SetCameraLocation(camera_location, True)
        view.Redraw()
        return rc
    *)

    ///<summary>Sets the camera location of the specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="cameraLocation">(Point3d)A 3D point identifying the new camera location.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCamera(view:string, cameraLocation:Point3d) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.SetCameraLocation(cameraLocation, true)
        view.Redraw()
        
    (*
    def ViewCamera(view=None, camera_location=None):
        '''Returns or sets the camera location of the specified view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          camera_location (point, optional): a 3D point identifying the new camera location.
            If omitted, the current camera location is returned
        Returns:
          point: If camera_location is not specified, the current camera location
          point: If camera_location is specified, the previous camera location
          None: on error
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.CameraLocation
        if camera_location is None: return rc
        camera_location = rhutil.coerce3dpoint(camera_location)
        if camera_location is None: return scriptcontext.errorhandler()
        view.ActiveViewport.SetCameraLocation(camera_location, True)
        view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the 35mm camera lens length of the specified perspective
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    /// projection view.</summary>
    ///<returns>(float) The current lens length</returns>
    static member ViewCameraLens(view:string) : float = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.Camera35mmLensLength
        
    (*
    def ViewCameraLens(view=None, length=None):
        '''Returns or sets the 35mm camera lens length of the specified perspective
        projection view.
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          length (number, optional): the new 35mm camera lens length. If omitted, the previous
            35mm camera lens length is returned
        Returns:
          number: If lens length is not specified, the current lens length
          number: If lens length is specified, the previous lens length
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.Camera35mmLensLength
        if not length: return rc
        view.ActiveViewport.Camera35mmLensLength = length
        view.Redraw()
        return rc
    *)

    ///<summary>Sets the 35mm camera lens length of the specified perspective
    /// projection view.</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="length">(float)The new 35mm camera lens length.</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCameraLens(view:string,length:float) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.Camera35mmLensLength <- length
        view.Redraw()
        
    (*
    def ViewCameraLens(view=None, length=None):
        '''Returns or sets the 35mm camera lens length of the specified perspective
        projection view.
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          length (number, optional): the new 35mm camera lens length. If omitted, the previous
            35mm camera lens length is returned
        Returns:
          number: If lens length is not specified, the current lens length
          number: If lens length is specified, the previous lens length
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.Camera35mmLensLength
        if not length: return rc
        view.ActiveViewport.Camera35mmLensLength = length
        view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the orientation of a view's camera.</summary>
    ///<param name="view">(string) Optional, Title or id of the view. If omitted, the current active view is used</param>
    ///<returns>(Plane) the view's camera plane</returns>
    static member ViewCameraPlane([<OPT;DEF("":string)>]view:string) : Plane =
        let view = RhinoScriptSyntax.CoerceView(view)
        let rc, frame = view.ActiveViewport.GetCameraFrame()
        if not rc then failwithf "Rhino.Scripting: ViewCameraPlane failed.  view:'%A'" view
        frame
    (*
    def ViewCameraPlane(view=None):
        '''Returns the orientation of a view's camera.
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
        Returns:
          plane: the view's camera plane if successful
          None: on error
        '''
    
        view = __viewhelper(view)
        rc, frame = view.ActiveViewport.GetCameraFrame()
        if not rc: return scriptcontext.errorhandler()
        return frame
    *)


    [<EXT>]
    ///<summary>Returns the camera and target positions of the specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(Point3d * Point3d) the 3d points containing the current camera and target locations</returns>
    static member ViewCameraTarget(view:string) : Point3d * Point3d = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.CameraLocation, view.ActiveViewport.CameraTarget
        
    (*
    def ViewCameraTarget(view=None, camera=None, target=None):
        '''Returns or sets the camera and target positions of the specified view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used
          camera (point): 3d point identifying the new camera location. If camera and
             target are not specified, current camera and target locations are returned
          target (point): 3d point identifying the new target location. If camera and
             target are not specified, current camera and target locations are returned
        Returns:
          list(point, point): if both camera and target are not specified, then the 3d points containing
            the current camera and target locations is returned
          point: if either camera or target are specified, then the 3d points containing the
            previous camera and target locations is returned
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.CameraLocation, view.ActiveViewport.CameraTarget
        if not camera and not target: return rc
        if camera: camera = rhutil.coerce3dpoint(camera, True)
        if target: target = rhutil.coerce3dpoint(target, True)
        if camera and target: view.ActiveViewport.SetCameraLocations(target, camera)
        elif camera is None: view.ActiveViewport.SetCameraTarget(target, True)
        else: view.ActiveViewport.SetCameraLocation(camera, True)
        view.Redraw()
        return rc
    *)

    ///<summary>Sets the camera and target positions of the specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="camera">(Point3d)3d point identifying the new camera location.</param>
    ///<param name="target">(Point3d)3d point identifying the new target location. </param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCameraTarget(view:string, camera:Point3d,target:Point3d) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.SetCameraLocations(target, camera)
        view.Redraw()
    (*
    def ViewCameraTarget(view=None, camera=None, target=None):
        '''Returns or sets the camera and target positions of the specified view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used
          camera (point): 3d point identifying the new camera location. If camera and
             target are not specified, current camera and target locations are returned
          target (point): 3d point identifying the new target location. If camera and
             target are not specified, current camera and target locations are returned
        Returns:
          list(point, point): if both camera and target are not specified, then the 3d points containing
            the current camera and target locations is returned
          point: if either camera or target are specified, then the 3d points containing the
            previous camera and target locations is returned
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.CameraLocation, view.ActiveViewport.CameraTarget
        if not camera and not target: return rc
        if camera: camera = rhutil.coerce3dpoint(camera, True)
        if target: target = rhutil.coerce3dpoint(target, True)
        if camera and target: view.ActiveViewport.SetCameraLocations(target, camera)
        elif camera is None: view.ActiveViewport.SetCameraTarget(target, True)
        else: view.ActiveViewport.SetCameraLocation(camera, True)
        view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the camera up direction of a specified</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(Vector3d) The current camera up direction</returns>
    static member ViewCameraUp(view:string) : Vector3d = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.CameraUp
        
    (*
    def ViewCameraUp(view=None, up_vector=None):
        '''Returns or sets the camera up direction of a specified
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          up_vector (vector): 3D vector identifying the new camera up direction
        Returns:
          vector: if up_vector is not specified, then the current camera up direction
          vector: if up_vector is specified, then the previous camera up direction
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.CameraUp
        if up_vector:
            view.ActiveViewport.CameraUp = rhutil.coerce3dvector(up_vector, True)
            view.Redraw()
        return rc
    *)

    ///<summary>Sets the camera up direction of a specified</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="upVector">(Vector3d)3D vector identifying the new camera up direction</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCameraUp(view:string, upVector:Vector3d) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.CameraUp <- upVector
        view.Redraw()
        
    (*
    def ViewCameraUp(view=None, up_vector=None):
        '''Returns or sets the camera up direction of a specified
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, the current active view is used
          up_vector (vector): 3D vector identifying the new camera up direction
        Returns:
          vector: if up_vector is not specified, then the current camera up direction
          vector: if up_vector is specified, then the previous camera up direction
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.CameraUp
        if up_vector:
            view.ActiveViewport.CameraUp = rhutil.coerce3dvector(up_vector, True)
            view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Return a view's construction plane</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(Plane) The current construction plane</returns>
    static member ViewCPlane(view:string) : Plane = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.ConstructionPlane()
       
    (*
    def ViewCPlane(view=None, plane=None):
        '''Return or set a view's construction plane
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used.
          plane (plane): the new construction plane if setting
        Returns:
          plane: If a construction plane is not specified, the current construction plane
          plane: If a construction plane is specified, the previous construction plane
        '''
    
        view = __viewhelper(view)
        cplane = view.ActiveViewport.ConstructionPlane()
        if plane:
            plane = rhutil.coerceplane(plane, True)
            view.ActiveViewport.SetConstructionPlane(plane)
            view.Redraw()
        return cplane
    *)

    ///<summary>Set a view's construction plane</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="plane">(Plane)The new construction plane if setting</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewCPlane(view:string, plane:Plane) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)        
        view.ActiveViewport.SetConstructionPlane(plane)
        view.Redraw()
        
    (*
    def ViewCPlane(view=None, plane=None):
        '''Return or set a view's construction plane
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used.
          plane (plane): the new construction plane if setting
        Returns:
          plane: If a construction plane is not specified, the current construction plane
          plane: If a construction plane is specified, the previous construction plane
        '''
    
        view = __viewhelper(view)
        cplane = view.ActiveViewport.ConstructionPlane()
        if plane:
            plane = rhutil.coerceplane(plane, True)
            view.ActiveViewport.SetConstructionPlane(plane)
            view.Redraw()
        return cplane
    *)


    [<EXT>]
    ///<summary>Return a view display mode</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(string)the current mode </returns>
    static member ViewDisplayMode(view:string) : string = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let current = view.ActiveViewport.DisplayMode
        current.EnglishName
       
    (*
    def ViewDisplayMode(view=None, mode=None, return_name=True):
        '''Return or set a view display mode
        Parameters:
          view (str|guid, optional): Title or id of a view. If omitted, active view is used
          mode (str|guid, optional): Name or id of a display mode
          return_name (bool, optional): If true, return display mode name. If False, display mode id
        Returns:
          str: If mode is specified, the previous mode
          str: If mode is not specified, the current mode
        '''
    
        view = __viewhelper(view)
        current = view.ActiveViewport.DisplayMode
        if return_name: rc = current.EnglishName
        else: rc = current.Id
        if mode:
            mode_id = rhutil.coerceguid(mode)
            if mode_id:
                desc = Rhino.Display.DisplayModeDescription.GetDisplayMode(mode_id)
            else:
                desc = Rhino.Display.DisplayModeDescription.FindByName(mode)
            if desc: view.ActiveViewport.DisplayMode = desc
            scriptcontext.doc.Views.Redraw()
        return rc
    *)

    ///<summary>Set a view display mode</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="mode">(string)Name of a display mode</param>
    ///<returns>(string) If mode is not specified, the current mode</returns>
    static member ViewDisplayMode(view:string, mode:string) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let desc = Rhino.Display.DisplayModeDescription.FindByName(mode)
        if notNull desc then 
            view.ActiveViewport.DisplayMode <- desc
            Doc.Views.Redraw()
        else
            failwithf "set ViewDisplayMode mode %s not found." mode
        
    (*
    def ViewDisplayMode(view=None, mode=None, return_name=True):
        '''Return or set a view display mode
        Parameters:
          view (str|guid, optional): Title or id of a view. If omitted, active view is used
          mode (str|guid, optional): Name or id of a display mode
          return_name (bool, optional): If true, return display mode name. If False, display mode id
        Returns:
          str: If mode is specified, the previous mode
          str: If mode is not specified, the current mode
        '''
    
        view = __viewhelper(view)
        current = view.ActiveViewport.DisplayMode
        if return_name: rc = current.EnglishName
        else: rc = current.Id
        if mode:
            mode_id = rhutil.coerceguid(mode)
            if mode_id:
                desc = Rhino.Display.DisplayModeDescription.GetDisplayMode(mode_id)
            else:
                desc = Rhino.Display.DisplayModeDescription.FindByName(mode)
            if desc: view.ActiveViewport.DisplayMode = desc
            scriptcontext.doc.Views.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Return id of a display mode given it's name</summary>
    ///<param name="name">(string) Name of the display mode</param>
    ///<returns>(Guid) The id of the display mode , otherwise None</returns>
    static member ViewDisplayModeId(name:string) : Guid =
        let desc = Rhino.Display.DisplayModeDescription.FindByName(name)
        if notNull desc then desc.Id
        else
            failwithf "set ViewDisplayModeId mode %s not found." name
    (*
    def ViewDisplayModeId(name):
        '''Return id of a display mode given it's name
        Parameters:
          name (str): name of the display mode
        Returns:
          guid: The id of the display mode if successful, otherwise None
        '''
    
        desc = Rhino.Display.DisplayModeDescription.FindByName(name)
        if desc: return desc.Id
    *)


    [<EXT>]
    ///<summary>Return name of a display mode given it's id</summary>
    ///<param name="modeId">(Guid) The identifier of the display mode obtained from the ViewDisplayModes method.</param>
    ///<returns>(string) The name of the display mode , otherwise None</returns>
    static member ViewDisplayModeName(modeId:Guid) : string =
        //modeId = RhinoScriptSyntax.Coerceguid(modeId)
        let desc = Rhino.Display.DisplayModeDescription.GetDisplayMode(modeId)
        if notNull desc then desc.EnglishName
        else
            failwithf "set ViewDisplayModeName Id %A not found." modeId
    (*
    def ViewDisplayModeName(mode_id):
        '''Return name of a display mode given it's id
        Parameters:
          mode_id (guid): The identifier of the display mode obtained from the ViewDisplayModes method.
        Returns:
          str: The name of the display mode if successful, otherwise None
        '''
    
        mode_id = rhutil.coerceguid(mode_id, True)
        desc = Rhino.Display.DisplayModeDescription.GetDisplayMode(mode_id)
        if desc: return desc.EnglishName
    *)


    [<EXT>]
    ///<summary>Return list of display modes</summary>    
    ///<returns>(string ResizeArray) strings identifying the display mode names </returns>
    static member ViewDisplayModes() : string ResizeArray =
        let modes = Rhino.Display.DisplayModeDescription.GetDisplayModes()
        resizeArray {for mode in modes do mode.EnglishName }
    (*
    def ViewDisplayModes(return_names=True):
        '''Return list of display modes
        Parameters:
          return_names (bool, optional): If True, return mode names. If False, return ids
        Returns:
          list(str|guid, ...): strings identifying the display mode names or identifiers if successful
        '''
    
        modes = Rhino.Display.DisplayModeDescription.GetDisplayModes()
        if return_names:
            return [mode.EnglishName for mode in modes]
        return [mode.Id for mode in modes]
    *)


    [<EXT>]
    ///<summary>Return the names, titles, or identifiers of all views in the document</summary>  
    ///<param name="viewType">(int) Optional, Default: standard model views: <c>0</c>
    ///The type of view to return
    ///  0 = standard model views
    ///  1 = page layout views
    ///  2 = both standard and page layout views</param>
    ///<returns>(string ResizeArray) of the view names or identifiers on success</returns>
    static member ViewNames([<OPT;DEF(0)>]viewType:int) : string ResizeArray =
        let views = Doc.Views.GetViewList(viewType <> 1, viewType>0)
        if views|> isNull  then failwithf "Rhino.Scripting: ViewNames failed. viewType:'%A'" viewType
        resizeArray { for view in views do view.MainViewport.Name}
        
    (*
    def ViewNames(return_names=True, view_type=0):
        '''Return the names, titles, or identifiers of all views in the document
        Parameters:
          return_names (bool, optional): if True then the names of the views are returned.
            If False, then the identifiers of the views are returned
          view_type: (number, optional): the type of view to return
                           0 = standard model views
                           1 = page layout views
                           2 = both standard and page layout views
        Returns:
          list(str|guid, ...): of the view names or identifiers on success
          None: on error
        '''
    
        views = scriptcontext.doc.Views.GetViewList(view_type!=1, view_type>0)
        if views is None: return scriptcontext.errorhandler()
        if return_names: return [view.MainViewport.Name for view in views]
        return [view.MainViewport.Id for view in views]
    *)


    [<EXT>]
    ///<summary>Return 3d corners of a view's near clipping plane rectangle. Useful
    ///  in determining the "real world" size of a parallel-projected view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(Point3d * Point3d * Point3d * Point3d) Four Point3d that define the corners of the rectangle (counter-clockwise order)</returns>
    static member ViewNearCorners([<OPT;DEF("":string)>]view:string) : Point3d * Point3d * Point3d * Point3d =
        let view = RhinoScriptSyntax.CoerceView(view)
        let rc = view.ActiveViewport.GetNearRect()
        rc.[0], rc.[1], rc.[3], rc.[2]
    (*
    def ViewNearCorners(view=None):
        '''Return 3d corners of a view's near clipping plane rectangle. Useful
        in determining the "real world" size of a parallel-projected view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used
        Returns:
          list(point, point, point, point): Four Point3d that define the corners of the rectangle (counter-clockwise order)
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.GetNearRect()
        return rc[0], rc[1], rc[3], rc[2]
    *)


    [<EXT>]
    ///<summary>Return a view's projection mode.</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(int) The current projection mode for the specified view
    ///  1 = parallel
    ///  2 = perspective
    ///  3 = two point perspective</returns>
    static member ViewProjection(view:string) : int = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        let mutable rc = 2
        if viewport.IsParallelProjection then rc <- 1
        elif viewport.IsTwoPointPerspectiveProjection then rc <- 3        
        rc
    (*
    def ViewProjection(view=None, mode=None):
        '''Return or set a view's projection mode.
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used
          mode (number, optional): the projection mode
            1 = parallel
            2 = perspective
            3 = two point perspective
        Returns:
          number: if mode is not specified, the current projection mode for the specified view
          number: if mode is specified, the previous projection mode for the specified view
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        rc = 2
        if viewport.IsParallelProjection: rc = 1
        elif viewport.IsTwoPointPerspectiveProjection: rc = 3
        if mode is None or mode==rc: return rc
        if mode==1: viewport.ChangeToParallelProjection(True)
        elif mode==2: viewport.ChangeToPerspectiveProjection(True, 50)
        elif mode==3: viewport.ChangeToTwoPointPerspectiveProjection(50)
        else: return
        view.Redraw()
        return rc
    *)

    ///<summary>Set a view's projection mode.</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="mode">(int)The projection mode
    ///  1 = parallel
    ///  2 = perspective
    ///  3 = two point perspective</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewProjection(view:string, mode:int) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        if mode = 1 then viewport.ChangeToParallelProjection(true) |> ignore
        elif mode = 2 then viewport.ChangeToPerspectiveProjection(true, 35.)|> ignore
        elif mode = 3 then viewport.ChangeToTwoPointPerspectiveProjection(35.)       |> ignore
        view.Redraw()
        
    (*
    def ViewProjection(view=None, mode=None):
        '''Return or set a view's projection mode.
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used
          mode (number, optional): the projection mode
            1 = parallel
            2 = perspective
            3 = two point perspective
        Returns:
          number: if mode is not specified, the current projection mode for the specified view
          number: if mode is specified, the previous projection mode for the specified view
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        rc = 2
        if viewport.IsParallelProjection: rc = 1
        elif viewport.IsTwoPointPerspectiveProjection: rc = 3
        if mode is None or mode==rc: return rc
        if mode==1: viewport.ChangeToParallelProjection(True)
        elif mode==2: viewport.ChangeToPerspectiveProjection(True, 50)
        elif mode==3: viewport.ChangeToTwoPointPerspectiveProjection(50)
        else: return
        view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the radius of a parallel-projected view. Useful
    /// when you need an absolute zoom factor for a parallel-projected view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(float) The current view radius for the specified view</returns>
    static member ViewRadius(view:string) : float = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        if not viewport.IsParallelProjection then failwithf "Rhino.Scripting: ViewRadius view is not ParallelProjection.  view:'%A' " view 
        let ok,a,b,c,d,e,f = viewport.GetFrustum()
        let frusright = b
        let frustop = d
        let oldradius = min frustop frusright
        oldradius
    (*
    def ViewRadius(view=None, radius=None, mode=False):
        '''Returns or sets the radius of a parallel-projected view. Useful
        when you need an absolute zoom factor for a parallel-projected view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used
          radius (number): the view radius
          mode (bool, optional): perform a "dolly" magnification by moving the camera
            towards/away from the target so that the amount of the screen
            subtended by an object changes.  true = perform a "zoom"
            magnification by adjusting the "lens" angle
        Returns:
          number: if radius is not specified, the current view radius for the specified view
          number: if radius is specified, the previous view radius for the specified view
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        if not viewport.IsParallelProjection: return scriptcontext.errorhandler()
        fr = viewport.GetFrustum()
        frus_right = fr[2]
        frus_top = fr[4]
        old_radius = min(frus_top, frus_right)
        if radius is None: return old_radius
        magnification_factor = radius / old_radius
        d = 1.0 / magnification_factor
        viewport.Magnify(d, mode)
        view.Redraw()
        return old_radius
    *)

    ///<summary>Sets the radius of a parallel-projected view. Useful
    /// when you need an absolute zoom factor for a parallel-projected view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="radius">(float)The view radius</param>
    ///<param name="mode">(bool)Perform a "dolly" magnification by moving the camera
    ///  towards/away from the target so that the amount of the screen
    ///  subtended by an object changes.  true = perform a "zoom"
    ///  magnification by adjusting the "lens" angle</param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewRadius(view:string, radius:float, mode:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        if not viewport.IsParallelProjection then failwithf "Rhino.Scripting: ViewRadius view is not ParallelProjection.  view:'%A' " view 
        let ok,a,b,c,d,e,f = viewport.GetFrustum()
        let frusright = b
        let frustop = d
        let oldradius = min frustop frusright        
        let magnificationfactor = radius / oldradius
        let d = 1.0 / magnificationfactor
        viewport.Magnify(d, mode) |> ignore
        view.Redraw()
        
    (*
    def ViewRadius(view=None, radius=None, mode=False):
        '''Returns or sets the radius of a parallel-projected view. Useful
        when you need an absolute zoom factor for a parallel-projected view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used
          radius (number): the view radius
          mode (bool, optional): perform a "dolly" magnification by moving the camera
            towards/away from the target so that the amount of the screen
            subtended by an object changes.  true = perform a "zoom"
            magnification by adjusting the "lens" angle
        Returns:
          number: if radius is not specified, the current view radius for the specified view
          number: if radius is specified, the previous view radius for the specified view
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        if not viewport.IsParallelProjection: return scriptcontext.errorhandler()
        fr = viewport.GetFrustum()
        frus_right = fr[2]
        frus_top = fr[4]
        old_radius = min(frus_top, frus_right)
        if radius is None: return old_radius
        magnification_factor = radius / old_radius
        d = 1.0 / magnification_factor
        viewport.Magnify(d, mode)
        view.Redraw()
        return old_radius
    *)


    [<EXT>]
    ///<summary>Returns the width and height in pixels of the specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(int * int ) of two numbers identifying width and height</returns>
    static member ViewSize([<OPT;DEF(null:string)>]view:string) : int * int =
        let view = RhinoScriptSyntax.CoerceView(view)
        let cr = view.ClientRectangle
        cr.Width, cr.Height
    (*
    def ViewSize(view=None):
        '''Returns the width and height in pixels of the specified view
        Parameters:
          view (str|guid): title or id of the view. If omitted, current active view is used
        Returns:
          tuple(number, number): of two numbers identifying width and height
        '''
    
        view = __viewhelper(view)
        cr = view.ClientRectangle
        return cr.Width, cr.Height
    *)


    [<EXT>]
    ///<summary>Test's Rhino's display performance</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null:string</c>
    ///The title or identifier of the view.  If omitted, the current active view is used</param>
    ///<param name="frames">(int) Optional, Default Value: <c>100</c>
    ///The number of frames, or times to regenerate the view. If omitted, the view will be regenerated 100 times.</param>
    ///<param name="freeze">(bool) Optional, Default Value: <c>true</c>
    ///If True (Default), then Rhino's display list will not be updated with every frame redraw. If False, then Rhino's display list will be updated with every frame redraw.</param>
    ///<param name="direction">(int) Optional, Default Value: <c>0</c>
    ///The direction to rotate the view. The default direction is Right (0). Modes:
    ///  0 = Right
    ///  1 = Left
    ///  2 = Down
    ///  3 = Up.</param>
    ///<param name="angleDegrees">(float) Optional, Default Value: <c>5</c>
    ///The angle to rotate. If omitted, the rotation angle of 5.0 degrees will be used.</param>
    ///<returns>(float) The number of seconds it took to regenerate the view frames number of times,</returns>
    static member ViewSpeedTest( [<OPT;DEF("":string)>]view:string, 
                                 [<OPT;DEF(100)>]frames:int, 
                                 [<OPT;DEF(true)>]freeze:bool, 
                                 [<OPT;DEF(0)>]direction:int, 
                                 [<OPT;DEF(5)>]angleDegrees:float) : float =
        let view = RhinoScriptSyntax.CoerceView(view)
        let angleradians = toRadians(angleDegrees)
        view.SpeedTest(frames, freeze, direction, angleradians)
    (*
    def ViewSpeedTest(view=None, frames=100, freeze=True, direction=0, angle_degrees=5):
        '''Test's Rhino's display performance
        Parameters:
          view (str|guid, optional): The title or identifier of the view.  If omitted, the current active view is used
          frames (number, optional): The number of frames, or times to regenerate the view. If omitted, the view will be regenerated 100 times.
          freeze (bool, optional): If True (Default), then Rhino's display list will not be updated with every frame redraw. If False, then Rhino's display list will be updated with every frame redraw.
          direction (number, optional): The direction to rotate the view. The default direction is Right (0). Modes:
            0 = Right
            1 = Left
            2 = Down
            3 = Up.
          angle_degrees (number, optional): The angle to rotate. If omitted, the rotation angle of 5.0 degrees will be used.
        Returns:
          number: The number of seconds it took to regenerate the view frames number of times, if successful
          None: if not successful
        '''
    
        view = __viewhelper(view)
        angle_radians = math.radians(angle_degrees)
        return view.SpeedTest(frames, freeze, direction, angle_radians)
    *)


    [<EXT>]
    ///<summary>Returns the target location of the specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(Point3d) The current target location</returns>
    static member ViewTarget(view:string) : Point3d = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        viewport.CameraTarget
    (*
    def ViewTarget(view=None, target=None):
        '''Returns or sets the target location of the specified view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used
          target (point, optional): 3d point identifying the new target location. If omitted,
            the current target location is returned
        Returns:
          point: is target is not specified, then the current target location
          point: is target is specified, then the previous target location
          None: on error
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        old_target = viewport.CameraTarget
        if target is None: return old_target
        target = rhutil.coerce3dpoint(target)
        if target is None: return scriptcontext.errorhandler()
        viewport.SetCameraTarget(target, True)
        view.Redraw()
        return old_target
    *)

    ///<summary>Sets the target location of the specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="target">(Point3d)3d point identifying the new target location. </param>
    ///<returns>(unit) void, nothing</returns>
    static member ViewTarget(view:string,target:Point3d) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)
        let viewport = view.ActiveViewport
        viewport.SetCameraTarget(target, true)
        view.Redraw()
        
    (*
    def ViewTarget(view=None, target=None):
        '''Returns or sets the target location of the specified view
        Parameters:
          view (str|guid, optional): title or id of the view. If omitted, current active view is used
          target (point, optional): 3d point identifying the new target location. If omitted,
            the current target location is returned
        Returns:
          point: is target is not specified, then the current target location
          point: is target is specified, then the previous target location
          None: on error
        '''
    
        view = __viewhelper(view)
        viewport = view.ActiveViewport
        old_target = viewport.CameraTarget
        if target is None: return old_target
        target = rhutil.coerce3dpoint(target)
        if target is None: return scriptcontext.errorhandler()
        viewport.SetCameraTarget(target, True)
        view.Redraw()
        return old_target
    *)


    [<EXT>]
    ///<summary>Returns the name, or title, of a given view's identifier</summary>
    ///<param name="viewId">(Guid) The identifier of the view</param>
    ///<returns>(string) name or title of the view on success</returns>
    static member ViewTitle(viewId:Guid) : string =        
        let view = RhinoScriptSyntax.CoerceView(viewId)
        view.MainViewport.Name
    (*
    def ViewTitle(view_id):
        '''Returns the name, or title, of a given view's identifier
        Parameters:
          view_id (str|guid): The identifier of the view
        Returns:
          str: name or title of the view on success
          None: on error
        '''
    
        view_id = rhutil.coerceguid(view_id)
        if view_id is None: return scriptcontext.errorhandler()
        view = scriptcontext.doc.Views.Find(view_id)
        if view is None: return scriptcontext.errorhandler()
        return view.MainViewport.Name
    *)


    [<EXT>]
    ///<summary>Returns the wallpaper bitmap of the specified view. To remove a
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    /// wallpaper bitmap, pass an empty string ""</summary>
    ///<returns>(string option) The current wallpaper bitmap filename</returns>
    static member Wallpaper(view:string) : string option= //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        let f=view.ActiveViewport.WallpaperFilename
        if isNull f then None else Some f
    (*
    def Wallpaper(view=None, filename=None):
        '''Returns or sets the wallpaper bitmap of the specified view. To remove a
        wallpaper bitmap, pass an empty string ""
        Parameters:
          view (str|guid, optional): The identifier of the view. If omitted, the
            active view is used
          filename (str): Name of the bitmap file to set as wallpaper
        Returns:
          str: If filename is not specified, the current wallpaper bitmap filename
          str: If filename is specified, the previous wallpaper bitmap filename
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.WallpaperFilename
        if filename is not None and filename!=rc:
            view.ActiveViewport.SetWallpaper(filename, False)
            view.Redraw()
        return rc
    *)

    ///<summary>Sets the wallpaper bitmap of the specified view. To remove a
    /// wallpaper bitmap, pass an empty string ""</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="filename">(string)Name of the bitmap file to set as wallpaper</param>
    ///<returns>(unit) void, nothing</returns>
    static member Wallpaper(view:string, filename:string) : unit = //SET
        let viewo = RhinoScriptSyntax.CoerceView(view)
        let rc = viewo.ActiveViewport.WallpaperFilename
        if not <| viewo.ActiveViewport.SetWallpaper(filename, false) then failwithf "Wallpaper faild to set walleper to %s in view %s" filename view
        viewo.Redraw()
        
    (*
    def Wallpaper(view=None, filename=None):
        '''Returns or sets the wallpaper bitmap of the specified view. To remove a
        wallpaper bitmap, pass an empty string ""
        Parameters:
          view (str|guid, optional): The identifier of the view. If omitted, the
            active view is used
          filename (str): Name of the bitmap file to set as wallpaper
        Returns:
          str: If filename is not specified, the current wallpaper bitmap filename
          str: If filename is specified, the previous wallpaper bitmap filename
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.WallpaperFilename
        if filename is not None and filename!=rc:
            view.ActiveViewport.SetWallpaper(filename, False)
            view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the grayscale display option of the wallpaper bitmap in a
    /// specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(bool) The current grayscale display option</returns>
    static member WallpaperGrayScale(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        view.ActiveViewport.WallpaperGrayscale
    (*
    def WallpaperGrayScale(view=None, grayscale=None):
        '''Returns or sets the grayscale display option of the wallpaper bitmap in a
        specified view
        Parameters:
          view (str|guid, optional):  The identifier of the view. If omitted, the
            active view is used
          grayscale (bool, optional): Display the wallpaper in gray(True) or color (False)
        Returns:
          bool: If grayscale is not specified, the current grayscale display option
          bool: If grayscale is specified, the previous grayscale display option
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.WallpaperGrayscale
        if grayscale is not None and grayscale!=rc:
            filename = view.ActiveViewport.WallpaperFilename
            view.ActiveViewport.SetWallpaper(filename, grayscale)
            view.Redraw()
        return rc
    *)

    ///<summary>Sets the grayscale display option of the wallpaper bitmap in a
    /// specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="grayscale">(bool)Display the wallpaper in gray(True) or color (False)</param>
    ///<returns>(unit) void, nothing</returns>
    static member WallpaperGrayScale(view:string, grayscale:bool) : unit = //SET
        let viewo = RhinoScriptSyntax.CoerceView(view)      
        let filename = viewo.ActiveViewport.WallpaperFilename
        if not <| viewo.ActiveViewport.SetWallpaper(filename, grayscale) then failwithf "WallpaperGrayScale faild to set walleper to %s in view %s" filename view        
        viewo.Redraw()
        
    (*
    def WallpaperGrayScale(view=None, grayscale=None):
        '''Returns or sets the grayscale display option of the wallpaper bitmap in a
        specified view
        Parameters:
          view (str|guid, optional):  The identifier of the view. If omitted, the
            active view is used
          grayscale (bool, optional): Display the wallpaper in gray(True) or color (False)
        Returns:
          bool: If grayscale is not specified, the current grayscale display option
          bool: If grayscale is specified, the previous grayscale display option
        '''
    
        view = __viewhelper(view)
        rc = view.ActiveViewport.WallpaperGrayscale
        if grayscale is not None and grayscale!=rc:
            filename = view.ActiveViewport.WallpaperFilename
            view.ActiveViewport.SetWallpaper(filename, grayscale)
            view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Returns the visibility of the wallpaper bitmap in a specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<returns>(bool) The current hidden state</returns>
    static member WallpaperHidden(view:string) : bool = //GET
        let view = RhinoScriptSyntax.CoerceView(view)
        not view.ActiveViewport.WallpaperVisible
       
    (*
    def WallpaperHidden(view=None, hidden=None):
        '''Returns or sets the visibility of the wallpaper bitmap in a specified view
        Parameters:
          view (str|guid, optional): The identifier of the view. If omitted, the
            active view is used
          hidden (bool, optional): Show or hide the wallpaper
        Returns:
          bool: If hidden is not specified, the current hidden state
          bool: If hidden is specified, the previous hidden state
        '''
    
        view = __viewhelper(view)
        rc = not view.ActiveViewport.WallpaperVisible
        if hidden is not None and hidden!=rc:
            filename = view.ActiveViewport.WallpaperFilename
            gray = view.ActiveViewport.WallpaperGrayscale
            view.ActiveViewport.SetWallpaper(filename, gray, not hidden)
            view.Redraw()
        return rc
    *)

    ///<summary>Sets the visibility of the wallpaper bitmap in a specified view</summary>
    ///<param name="view">(string)Title of the view. Use "" empty string for the current active view </param>
    ///<param name="hidden">(bool)Show or hide the wallpaper</param>
    ///<returns>(unit) void, nothing</returns>
    static member WallpaperHidden(view:string, hidden:bool) : unit = //SET
        let view = RhinoScriptSyntax.CoerceView(view)        
        let filename = view.ActiveViewport.WallpaperFilename
        let gray = view.ActiveViewport.WallpaperGrayscale
        view.ActiveViewport.SetWallpaper(filename, gray, not hidden) |> ignore
        view.Redraw()
        
    (*
    def WallpaperHidden(view=None, hidden=None):
        '''Returns or sets the visibility of the wallpaper bitmap in a specified view
        Parameters:
          view (str|guid, optional): The identifier of the view. If omitted, the
            active view is used
          hidden (bool, optional): Show or hide the wallpaper
        Returns:
          bool: If hidden is not specified, the current hidden state
          bool: If hidden is specified, the previous hidden state
        '''
    
        view = __viewhelper(view)
        rc = not view.ActiveViewport.WallpaperVisible
        if hidden is not None and hidden!=rc:
            filename = view.ActiveViewport.WallpaperFilename
            gray = view.ActiveViewport.WallpaperGrayscale
            view.ActiveViewport.SetWallpaper(filename, gray, not hidden)
            view.Redraw()
        return rc
    *)


    [<EXT>]
    ///<summary>Zooms to the extents of a specified bounding box in the specified view</summary>
    ///<param name="boundingBox">(Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d * Point3d) Eight points that define the corners
    ///  of a bounding box or a BoundingBox class instance</param>
    ///<param name="view">(string) Optional, Default Value: <c>null:string</c>
    ///Title or id of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///Zoom extents in all views</param>
    ///<returns>(unit) </returns>
    static member ZoomBoundingBox( boundingBox:BoundingBox, 
                                   [<OPT;DEF("":string)>]view:string, 
                                   [<OPT;DEF(false)>]all:bool) : unit =        
          if all then
              let views = Doc.Views.GetViewList(true, true)
              for view in views do view.ActiveViewport.ZoomBoundingBox(boundingBox) |> ignore
          else 
              let view = RhinoScriptSyntax.CoerceView(view)
              view.ActiveViewport.ZoomBoundingBox(boundingBox) |> ignore
          Doc.Views.Redraw()
    (*
    def ZoomBoundingBox(bounding_box, view=None, all=False):
        '''Zooms to the extents of a specified bounding box in the specified view
        Parameters:
          bounding_box ([point, point, point ,point, point, point, point, point]): eight points that define the corners
            of a bounding box or a BoundingBox class instance
          view  (str|guid, optional): title or id of the view. If omitted, current active view is used
          all (bool, optional): zoom extents in all views
        Returns:
          None
        '''
    
        bbox = rhutil.coerceboundingbox(bounding_box)
        if bbox:
          if all:
              views = scriptcontext.doc.Views.GetViewList(True, True)
              for view in views: view.ActiveViewport.ZoomBoundingBox(bbox)
          else:
              view = __viewhelper(view)
              view.ActiveViewport.ZoomBoundingBox(bbox)
          scriptcontext.doc.Views.Redraw()
    *)


    [<EXT>]
    ///<summary>Zooms to extents of visible objects in the specified view</summary>
    ///<param name="view">(string) Optional, Title or id of the view. If omitted, current active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///Zoom extents in all views</param>
    ///<returns>(unit) </returns>
    static member ZoomExtents([<OPT;DEF("":string)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        if  all then
            let views = Doc.Views.GetViewList(true, true)
            for view in views do view.ActiveViewport.ZoomExtents()|> ignore
        else 
            let view = RhinoScriptSyntax.CoerceView(view)
            view.ActiveViewport.ZoomExtents()|> ignore
        Doc.Views.Redraw()
    (*
    def ZoomExtents(view=None, all=False):
        '''Zooms to extents of visible objects in the specified view
        Parameters:
          view  (str|guid, optional): title or id of the view. If omitted, current active view is used
          all (bool, optional): zoom extents in all views
        Returns:
          None
        '''
    
        if all:
            views = scriptcontext.doc.Views.GetViewList(True, True)
            for view in views: view.ActiveViewport.ZoomExtents()
        else:
            view = __viewhelper(view)
            view.ActiveViewport.ZoomExtents()
        scriptcontext.doc.Views.Redraw()
    *)


    [<EXT>]
    ///<summary>Zoom to extents of selected objects in a view</summary>
    ///<param name="view">(string) Optional, Default Value: <c>null:string</c>
    ///Title or id of the view. If omitted, active view is used</param>
    ///<param name="all">(bool) Optional, Default Value: <c>false</c>
    ///Zoom extents in all views</param>
    ///<returns>(unit) </returns>
    static member ZoomSelected([<OPT;DEF("":string)>]view:string, [<OPT;DEF(false)>]all:bool) : unit =
        if all then
            let views = Doc.Views.GetViewList(true, true)
            for view in views do view.ActiveViewport.ZoomExtentsSelected()|> ignore
        else 
            let view = RhinoScriptSyntax.CoerceView(view)
            view.ActiveViewport.ZoomExtentsSelected()|> ignore
        Doc.Views.Redraw()
    (*
    def ZoomSelected(view=None, all=False):
        '''Zoom to extents of selected objects in a view
        Parameters:
          view  (str|guid, optional): title or id of the view. If omitted, active view is used
          all (bool, optional): zoom extents in all views
        Returns:
          None
        '''
    
        if all:
            views = scriptcontext.doc.Views.GetViewList(True, True)
            for view in views: view.ActiveViewport.ZoomExtentsSelected()
        else:
            view = __viewhelper(view)
            view.ActiveViewport.ZoomExtentsSelected()
        scriptcontext.doc.Views.Redraw()
    *)


