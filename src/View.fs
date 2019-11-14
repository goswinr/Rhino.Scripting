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
    ///<returns>(Guid) id of new layout</returns>
    static member AddLayout([<OPT;DEF(null:string)>]title:string, [<OPT;DEF(null)>]size:float * float) : Guid =
        let page = 
            if Object.ReferenceEquals(size,null)  then Doc.Views.AddPageView(title)
            else  Doc.Views.AddPageView(title, size|> fst, size|> snd)
        if notNull page then page.MainViewport.Id
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
    static member AddNamedView(name:string, [<OPT;DEF(null:string)>]view:string) : unit =
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
    static member IsViewMaximized([<OPT;DEF(null:string)>]view:string) : bool =
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


