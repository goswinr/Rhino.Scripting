# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- UnitScale accepts only the destination unit system and uses the current model units as the source, like in RhinoPython.
- UnitAbsoluteTolerance, UnitAngleTolerance, UnitDistanceDisplayPrecision, UnitRelativeTolerance and UnitSystem have model/page selector overloads, restoring page-unit access without changing existing model-unit calls.
- AddDimStyle() creates an automatically named dimension style and returns its name, like in RhinoPython.
- LinetypeNames accepts a sort flag to return names in ordinal order, like in RhinoPython.
- LayerNames accepts a sort flag to return names in ordinal order, like in RhinoPython.
- DimStyleNames accepts a sort flag to return names in ordinal order, like in RhinoPython.
- BlockNames accepts a sort flag to return names in ordinal order, like in RhinoPython.
- Direct geometry overloads for methods that previously required object identifiers, including collection inputs; geometry-producing operations take and return raw geometry, while document-object operations retain their Guid APIs.
- ContextIsGrasshopper, it checks if the call stack of the current thread goes through the Grasshopper assembly
- MeshTextureCoordinates, like in RhinoPython
- XformScale overloads with a Plane, to scale about the Plane's origin along the Plane's axes, like in RhinoPython
### Changed
- IsHatch, IsBlockInstance, IsPoint, IsPointCloud, IsText, IsTextDot, IsClippingPlane, IsBrep, IsPolysurface and IsPolysurfaceClosed return false for missing object identifiers, like in RhinoPython.
- IsObjectSolid returns false for geometry that cannot be solid, like in RhinoPython.
- GetObjectGrip and GetObjectGrips raise RhinoUserInteractionException on cancellation, and ObjectGripLocations reports a point count mismatch instead of silently doing nothing, unlike RhinoPython.
- Breaking: Parameter names ending in Id or Ids are reserved for Guid inputs; direct geometry parameters use names such as curveGeo, surfaceGeo and geometryObject, and non-Guid coercion inputs use descriptive names.
- The API table lists each Make factory immediately after its matching Add methods.
- Breaking: AddHatch and AddHatches with Curve geometry, and AddPlanarSrf with a Polyline, a Curve or a Curve seq, are renamed to MakeHatch, MakeHatches and MakePlanarSrf; they added the result to the document and returned its Guid, now they return the new geometry without adding it, use e.g. rs.Add(rs.MakePlanarSrf(curve)) to add it; the Add methods with Guid inputs still add to the document.
- Guid overloads share geometry calculations with the direct overloads and handle document insertion, replacement, deletion, attributes and redraws.
- Breaking: Add, CreatePoint, CreateVector and the Coerce and TryCoerce functions for Guids, lines, arcs, circles, ellipses, polylines, views, points, vectors, planes, transforms and colors have one overload per accepted input type instead of a generic parameter that was checked at runtime, so a wrong input type is a compile error now
- Breaking: these functions take coordinates and color values as separate arguments, like Coerce3dPoint(1, 2, 3) or CoerceColor(255, 0, 0), a tuple value like CoerceColor(rgb) does not compile any more
- Breaking: Coerce3dPoint, Coerce3dVector, CreatePoint and CreateVector don't accept options, tuples of decimals, singles or strings, or sequences of ints or strings any more, and a sequence of floats or a string like "1, 2, 3" must have exactly three values
- Breaking: CoerceGuid doesn't accept a Guid option any more, and raises an exception for a string of the empty Guid
- Breaking: CoercePlane only accepts a Plane, a Point3d or three floats, not all inputs of Coerce3dPoint
- Coerce3dVector with a Guid accepts any curve that is linear within tolerance, not only a LineCurve
- CreateXform accepts nested lists and arrays of floats, like CoerceXform
- Hide AutoOpen modules from editor completion lists with EditorBrowsable(EditorBrowsableState.Never).
- DimStyleTextAlignment sets the text orientation and location for radial dimensions too, like in RhinoPython
- ShowGrid, ShowGridAxes and ShowWorldAxes get and set the grid settings of the view's display mode if it overrides the document grid settings, like in RhinoPython
- CopyObject(s), and TransformObject(s), MirrorObject(s), RotateObject(s), ScaleObject(s) and ShearObject(s) with copy=true, put copies of grouped objects into new groups like the Copy command does, instead of adding them to the original groups, like in RhinoPython
- IsDimension, IsLeader and the other dimension checks return false for objects that don't exist, instead of raising an exception, like in RhinoPython
- IsCone, IsCylinder, IsPlaneSurface and IsPointLight, IsSpotLight and the other light type checks return false for objects of another type or that don't exist, instead of raising an exception, like in RhinoPython
### Fixed
- AddLeader accepts omitted text to create a leader without text, like in RhinoPython.
- CurveFilletPoints defaults its radius to 1.0 when omitted, like in RhinoPython.
- CurveCurveIntersection uses the same overlap tolerance as RhinoPython instead of substituting the document absolute tolerance.
- StatusBarPoint can be called without a value, like in RhinoPython.
- StatusBarMessage can be called without a value, like in RhinoPython.
- StatusBarDistance can be called without a value, like in RhinoPython.
- SendKeystrokes can be called without a value, like in RhinoPython.
- Prompt can be called without a value, like in RhinoPython.
- GetObjects applies the custom geometry filter as well as the allowed-object list when both are supplied.
- GetObject preserves other preselected objects when select is false, like in RhinoPython.
- Optional view names and empty view names use the active view in view operations, CreatePreviewImage and screen/world transforms, instead of raising in CoerceView.
- SplitBrep, PullCurve, JoinCurves and MeshBooleanDifference, MeshBooleanIntersection and MeshBooleanSplit reject empty results and retain their inputs when adding results fails.
- MakeHatches raises when no hatches can be created, instead of returning an empty list.
- Material getters and setters validate indices before indexing the material table, which otherwise silently substitutes the current material, unlike RhinoPython.
- MatchMaterial rejects a source without an assigned material before changing destination objects, unlike RhinoPython.
- LineTransform with copy=true preserves the original object's attributes.
- NextObjectGrip and PrevObjectGrip raise a clear exception when there is no neighboring grip, instead of a NullReferenceException.
- GroupObjects validates the object count before creating a group, so invalid input no longer leaves an empty group.
- IsGroupEmpty returns true for an empty group, unlike RhinoPython.
- EdgeAnalysisMode accepts mode 0 to show all edges.
- PlugIns combines the requested type flags instead of treating combinations as a request for every plug-in.
- IsPlugIn accepts a plug-in identifier string as well as a name.
- ScreenSize returns physical pixels, accounting for the display scale.
- StatusBarProgressMeterShow, Update and Hide process UI messages so the meter repaints during UI-thread loops.
- AddLinearLight converts its default width from inches, like in RhinoPython, instead of millimeters.
- PullPoints accepts plain surfaces and single-face extrusions by converting them to Breps, like in RhinoPython.
- PointClosestObject includes extrusion caps when finding the closest point.
- PointsAreCoplanar uses the documented default tolerance of 1e-12.
- UnitSystem sets the model unit system, including unit system 0, and applies optional scaling; the setter previously did nothing.
- ExtractPreviewImage uses the supplied model filename and reports missing previews and unsaved documents clearly.
- RenderMeshSettings preserves the texture-range flag, and render mesh numeric setters accept 0 to disable a setting.
- RenderMeshQuality rejects unsupported values instead of silently disabling the meshing style.
- CurveDeviation returns the curve parameters and distances in their documented order.
- CurveFilletPoints evaluates the second fillet point on the second curve.
- ConvertCurveToPolyline uses the documented default angle tolerance of 5 degrees.
- CurveLineClosestPoint returns the curve point before the line point, as documented.
- AddTweenCurves and MakeTweenCurves distinguish control-point matching from refitting and accept method 1.
- FairCurve writes successful fairing results to the document instead of returning false on success.
- InsertCurveKnot returns the result of insertion and document replacement, including symmetrical insertion.
- ChangeCurveDegree accepts degree 2 when increasing a linear curve's degree.
- OffsetCurveOnSurfaceUV extends the offsets to the surface edges, like in RhinoPython, and curve offset methods redraw after adding results.
- ObjectMaterialIndex accepts valid indices and rejects invalid indices; index -1 resets the assignment.
- SelectObject and SelectObjects can unlock a locked layer when forceVisible is true.
- Object transformations, copies, moves, HideObject and LockObject redraw after modifying document objects.
- PointCoordinates writes the supplied new location instead of the old point.
- TextObjectFont no longer eagerly raises an exception while choosing a font fallback.
- TextObjectRichText with multiple objects and a style applies rich text before replacing each document object.
- PointCloudHidePoints and PointCloudPointColors replace the edited point cloud geometry so changes are saved reliably.
- Area supports hatches using the hatch-specific area calculation.
- AddClippingPlane rejects an empty or unmatched view list.
- AddText warns when a requested bold or italic face is unavailable and still creates the text, like in RhinoPython.
- InvertSelectedObjects selects unselected objects and deselects selected objects, like in RhinoPython.
- WindowPick returns the picked identifiers and transforms full 3D corners to screen coordinates before dropping Z.
- LastObject and ObjectsByName avoid enumerating Rhino's object iterator twice.
- LastCreatedObjects selects only existing objects and redraws once, including when a single object was created.
- GetCurveObject checks whether the picked curve was actually preselected when clearing selection.
- NextObject raises RhinoScriptingException for missing or filtered-out objects instead of an iterator exception.
- UnCollapseLayer expands the requested layer, and recursive layer collapse and expansion handle leaf layers.
- LayerLockedSetFalse unlocks the layer when parentsToo is false, and LayerVisibleSetTrue sets visibility when forceVisible is false.
- AddLayer honors visibility and locking when generating a layer name.
- CurrentLayer, DeleteLayer, ExpandLayer and IsLayer accept short names as well as full layer paths.
- ReplaceBlockObjects updates the block definition before deleting inputs and redraws after success.
- IsBlockEmbedded recognizes Static definitions, which Rhino displays as embedded.
- MeshVertices preserves double-precision vertex coordinates.
- MeshOutline uses the active viewport when no view is supplied.
- PlaneCurveIntersection returns the documented curve interval and plane UV coordinates, unlike RhinoPython.
- OffsetSurface preserves tolerance 0 for loose offsets.
- ShootRay includes the starting point when the ray hits a surface, like in RhinoPython.
- SurfaceVolume rejects open geometry, and SurfaceEvaluate handles evaluation with no derivatives.
- AddLoftSrf and MakeLoftSrf reject unsupported loft type 4 instead of silently creating a normal loft.
- TiltView converts the absolute angle from degrees to radians before rotating the viewport.
- IsDetail looks for the detail name within the requested layout instead of looking it up as a model view.
- DetailLock reports a missing detail identifier clearly instead of raising NullReferenceException.
- ViewProjection uses a 50 mm lens when changing to perspective and preserves the lens when the projection is already correct.
- RotateCamera rejects an unsupported direction before rotating the camera.
- CullDuplicateNumbers retains the first number and every subsequent number outside tolerance, including two-item inputs.
- Polar uses WorldXY when its plane is omitted.
- FxrangePython and FrangePython retain the last value below the stop for nonintegral step counts, while continuing to exclude the stop.
- CreatePlane defaults each omitted axis independently, preserving a supplied axis.
- GetColor returns the color written back by the dialog's reflection arguments.
- GetDistance reads the first point before disposing the getter and returns the supplied distance when Enter is pressed.
- GetPolyline defaults to two required points and honors an explicit maximum point count.
- GetLayer preselects layers by full path, and GetEdgeCurves duplicates line, arc and polycurve edges without an invalid NurbsCurve cast.
- BrowseForFolder displays both the title and message when both are supplied.
- IsDocumentData and IsDocumentUserText use their respective document string counts.
- SetUserText rejects empty values before changing any object, because Rhino treats empty strings as deletion; the documentation now requires a nonempty value.
- CullDuplicatePoints uses the documented default ZeroTolerance, and ColorHLSToRGB passes saturation and luminance in the correct order.
- CoerceColor rejects empty and whitespace-only color names, and TryCoerceArc with a curve rejects full circles as documented.
- Add with a layer index and collapseParents collapses the parent layers instead of the target layer and resolves index -1 to the current layer.
- AddLeader fits a plane through all points instead of relying on the first and last segments; two-point leaders use WorldXY with a vertical-plane fallback for vertical segments, and adding a leader redraws.
- AddAlignedDimension uses the active construction plane when the input points are collinear, like in RhinoPython.
- DimStyleExtension and DimStyleLeaderArrowSize accept 0, and dimension style setters report invalid negative values.
- AddHatchPatterns honors replace in Rhino 8; Rhino 7 reports that replacing an existing pattern is unsupported instead of ignoring it, unlike RhinoPython.
- LinetypeCount excludes deleted linetypes, unlike RhinoPython.
- Toolbar collection open, close and save operations and toolbar show and hide operations run on the UI thread.
- MirrorObject and MirrorObjects mirror about the vertical plane through the start and end point, like in RhinoPython, instead of about the plane perpendicular to the line between them; they raise an exception if the two points are vertically above each other
- ExtractSurface removes the extracted faces from the Polysurface only if copy is false, like in RhinoPython, it was the other way round; if all faces are extracted the Polysurface is deleted
- TextObjectStyle sets the font style of a text object, it changed nothing before
- CurveCurveIntersection without a second curve does a self-intersection test, like in RhinoPython, instead of reporting the whole curve as an overlap with itself
- LineTransform with copy=false transforms the line object in the document, before it returned the Guid without changing the object
- TrimBrep replaces the object with the trimmed piece also if only one piece is retained, like in RhinoPython, instead of keeping the untrimmed object and adding the piece as a new object without its attributes
- CurveFrame uses the segmentIndex of a PolyCurve, like in RhinoPython, it was ignored before
- ShrinkTrimmedSurface checks ShrinkFaces correctly, raising on failure instead of success.
- PointClosestObject uses the initialized result in the shared object implementation, avoiding a null-reference exception when comparing distances.
- DimStyleTextAlignment returns the documented values 1, 2 or 3, instead of the index of the text location
- IsLinetype returned true for any name in Rhino 8, and IsLinetypeReference used the Continuous linetype for unknown names
- ObjectPrintColorSource accepts 2 (print color by display)
- LayerLinetype and GetLinetype used the Continuous linetype for unknown names in Rhino 8
- ObjectLinetype can set the Continuous linetype
- AddMesh sets the given vertex colors, they were all empty before

## [0.15.0] - 2026-10-11
### Added
- RhinoScriptingException constructor with inner exception, and RhinoScriptingException.RaiseWith
- Builds for net10.0, in addition to net48 and net8.0
### Changed
- Breaking: CoerceColor reads a tuple of 4 integers as (red, green, blue, alpha), like in RhinoPython. It was (alpha, red, green, blue) before
- Add, AddPoints and AddLeader raise an exception if adding to the document fails, instead of returning Guid.Empty
- CoerceXform only accepts a 4x4 matrix
- FxrangePython and FrangePython return an empty range if the stop value cannot be reached, like in Python
- Without the Fesh editor, RhinoSync always uses Eto.Forms.Application.Instance.Invoke to get to the UI thread
- The assembly is built for AnyCPU instead of x64, so that it can load in Rhino running natively on ARM64
### Fixed
- TryCoerce functions never throw an exception, they return None on empty or unknown Guids
- GetLinetype pre-selects the defaultValLinetype (Rhino 8 only)
- RhinoSync.DoSyncRedraw and DoSyncRedrawHideEditor restore redraw and the Fesh editor even if an exception is raised, and work without an active Rhino document
- RhinoSync.HideEditor and ShowEditor can be called from any thread
- RhinoSync.PrintColor, PrintnColor, ClearLog, HideEditor and ShowEditor use the Fesh editor even if they are the first call to RhinoSync
- Clear error message if there is no active Rhino document (e.g. on Mac), event handlers are not added repeatedly anymore
- Thread safe initialization of RhinoSync
- Pretty printing of numbers above 1000 uses the invariant culture
- Build fails if combineIntoOneFile.fsx does not include all Scripting_*.fs files
- LastCreatedObjects returns the created objects even if Command was the first call to Rhino.Scripting
- No deadlock if the first call to Rhino.Scripting is on a worker thread that the UI thread waits for, e.g. in Array.Parallel.map; thread safe initialization of the internal State
- RhinoScriptSyntax.Doc follows a newly opened document even if the handler for the ActiveDocumentChanged event could not be added

## [0.14.0] - 2026-04-06
### Fixed
- no more auto fail on bad input for AddPolyline
### Changed
- net8 instead of net7


## [0.13.0] - 2026-01-17
### Fixed
- Fix DimStyleTextAlignment
- Fix Deprecation Warnings

### Changed
- CoerceView fails on empty string now instead of returning current view

## [0.12.1] - 2026-01-04
### Fixed
- Fix typos in documentation (README.md spacing issues)
- Fix typos in XML docstrings (Scripting_Header.fs, Scripting_Light.fs)
- Fix grammar errors in documentation comments

## [0.12.0] - 2025-05-25
### Fixed
- fix sync with Fesh.Rhino 0.27.4

## [0.11.0] - 2025-05-24
### Added
- build for net7.0 too
### Fixed
- many typos in docstrings via copilot
- create layers only on UI thread

## [0.10.1] - 2025-03-15
### Changed
- version number in exception messages

## [0.10.0] - 2025-03-07
### Changed
- rename .ToNiceString to .Pretty
- add rs.Print(..)
- expose more internals

## [0.9.0] - 2025-02-23
### Changed
- Remove dependency on FsEx
- Replace the heavily used custom list `Rarr<'T>` with `Collections.Generic.List<'T>`
- Use Eto.Forms instead of Windows.Forms

## [0.8.2] - 2025-02-23
### Changed
- build and deploy with GitHub Actions
- update FsEx to 0.16.0

## [0.8.0] - 2024-09-28
### Changed
- Drop support for Rhino 6.0 (7.0 or higher is required)
### Fixed
- Fix minor bugs about unused optional parameters

## [0.7.0] - 2023-09-10
### Changed
- Renamed main static class from Rhino.Scripting to Rhino.Scripting.RhinoScriptSyntax

## [0.6.2] - 2023-07-09
### Changed
- Even better window sync with Fesh Editor

## [0.6.1] - 2023-06-18
### Changed
- Better window sync with Fesh Editor
### Fixed
- Fixes in docs

## [0.6.0] - 2023-05-07
### Changed
- Don't check result of CommitChanges() anymore
- Relax constraints on UserText values

## [0.5.1] - 2023-02-18
### Fixed
- Fix readme
- Improve finding of SynchronizationContext

## [0.4.0] - 2023-01-21
### Fixed
- Fix threading bug to make it work in RhinoCode
- Fix typos

## [0.3.0] - 2022-12-03
### Changed
- Remove WPF dependency
- Don't return F# options anymore

## [0.2.0] - 2022-11-26
### Added
- First public release

[Unreleased]: https://github.com/goswinr/Rhino.Scripting/compare/0.15.0...HEAD
[0.15.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.14.0...0.15.0
[0.14.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.13.0...0.14.0
[0.13.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.12.1...0.13.0
[0.12.1]: https://github.com/goswinr/Rhino.Scripting/compare/0.12.0...0.12.1
[0.12.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.11.0...0.12.0
[0.11.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.10.1...0.11.0
[0.10.1]: https://github.com/goswinr/Rhino.Scripting/compare/0.10.0...0.10.1
[0.10.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.9.0...0.10.0
[0.9.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.8.2...0.9.0
[0.8.2]: https://github.com/goswinr/Rhino.Scripting/compare/0.8.0...0.8.2
[0.8.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.7.0...0.8.0
[0.7.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.6.2...0.7.0
[0.6.2]: https://github.com/goswinr/Rhino.Scripting/compare/0.6.1...0.6.2
[0.6.1]: https://github.com/goswinr/Rhino.Scripting/compare/0.6.0...0.6.1
[0.6.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.5.1...0.6.0
[0.5.1]: https://github.com/goswinr/Rhino.Scripting/compare/0.4.0...0.5.1
[0.4.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.3.0...0.4.0
[0.3.0]: https://github.com/goswinr/Rhino.Scripting/compare/0.2.0...0.3.0
[0.2.0]: https://github.com/goswinr/Rhino.Scripting/releases/tag/0.2.0
