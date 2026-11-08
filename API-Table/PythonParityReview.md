# Python parity review

Reviewed on 2026-11-09 against the installed Rhino 9 IronPython files in
`C:\Users\ROGO\AppData\Roaming\McNeel\Rhinoceros\9.0\Plug-ins\IronPython (814d908a-e25c-493d-97e9-ee3861957f49)\settings\lib\rhinoscript`.
All 26 implementation modules and `__init__.py` were compared through subagent reviews, then the findings were checked against the current source, XML documentation and changelog before making changes.

This review preserves documented F# API choices where they provide useful static types, explicit operations or predictable behavior. A same-named method with a different signature does not by itself establish a porting defect. Compilation checks the Rhino 7 and Rhino 8 API branches; interactive behavior still requires a running Rhino process.

## Fixes

Each code change has its own commit, including its changelog entry and regenerated combined source. Public API changes also regenerate the API table. Existing call syntax remains available when adding collection or selector options. BlockNames, DimStyleNames, LinetypeNames and LayerNames each use one optional sort argument defaulting to false, supporting both omitted and explicit sort flags.

| Commit | Change |
| --- | --- |
| `02f7e69` | Preserve unrelated preselected objects in `GetObject`. |
| `3130f42` | Apply both the allowed-object list and custom geometry filter in `GetObjects`. |
| `7c222e9` | Add BlockNames sorting support, subsequently consolidated into one optional sort argument defaulting to false. |
| `bf83c78` | Add DimStyleNames sorting support, subsequently consolidated into one optional sort argument defaulting to false. |
| `4d5903d` | Add LayerNames sorting support, subsequently consolidated into one optional sort argument defaulting to false. |
| `8b9c68d` | Add LinetypeNames sorting support, subsequently consolidated into one optional sort argument defaulting to false. |
| `64b619e` | Restore omitted-message support for `Prompt`. |
| `557a1c7` | Restore omitted-key support for `SendKeystrokes`. |
| `ac9ae17` | Restore the zero default for `StatusBarDistance`. |
| `854a5b6` | Restore omitted-message support for `StatusBarMessage`. |
| `4ea4590` | Restore the origin default for `StatusBarPoint`. |
| `493d36a` | Match Python's curve/curve overlap tolerance. |
| `dc2fe71` | Restore the default fillet radius in `CurveFilletPoints`. |
| `94b3f71` | Add automatically named dimension styles and return the generated name. |
| `17e20dd` | Allow omitted leader text. |
| `c15db52` | Restore model/page selectors for tolerance, precision and unit-system operations. |
| `3dcd0dd` | Restore current-model-unit defaults for `UnitScale`. |
| `dc97ff2` | Distinguish omitted linear-light width from an explicit zero. |
| `8d4d338` | Restore the default magnification mode in the `ViewRadius` setter. |
| `c4b5b64` | Add correctly spelled `IsSurfaceFlipped` getters while preserving `IsSurfaceFliped`. |
| `48ad1ef` | Restore mesh-area collection aggregation. |
| `2639548` | Add `GetObjectRef` for selected subobject information and share picking with `GetObject`. |
| `4733496` | Use the textless Rhino overload when leader text is omitted or null. |
| `b437600` | Restore material matching by material-table index. |
| `5db1e26` | Add `JoinSurfacesAll` for Python's `return_all` capability; retain inputs if adding results fails. |

The sorting overloads use ordinal comparison, avoiding culture-dependent ordering. The new page-unit overloads take `modelUnits:bool`; existing calls continue to address model units. `GetObjectRef` returns an owned `ObjRef` that the caller must dispose.

## Retained API choices

The shared conventions are documented in [AGENTS.md](../AGENTS.md), the [changelog](../CHANGELOG.md), and each method's XML comments: getters and setters are separate overloads; many setters return `unit`; invalid operations raise `RhinoScriptingException`; cancelled interaction raises `RhinoUserInteractionException`; optional coercion uses `TryCoerce` options. These contracts avoid dynamic return types and null-based success/failure handling.

| Python file / F# source | Retained choice and reason |
| --- | --- |
| `__init__.py` | Packaging imports only. It contains no functions. Its omission of `material` from `__all__` is a Python packaging detail; F# includes material methods. |
| [Application](../Src/Scripting_Application.fs) | Keep typed plug-in arguments and physical-pixel screen dimensions. Python's `ignore_runners` argument is unused in its implementation, so omitting that flag loses no behavior. |
| [Block](../Src/Scripting_Block.fs) | Keep explicit setter overloads and exceptions. Setting an empty block description is useful and should clear it rather than reproduce Python's truthiness no-op. |
| [Curve](../Src/Scripting_Curve.fs) | Keep separate point/parameter division methods and the fillet `Plane` result, which packages its center and axes. Division computes values; `AddPoints` explicitly adds document objects. This composes the Python `create_points` capability without a hidden document side effect. |
| [Dimension](../Src/Scripting_Dimension.fs) | Keep the documented argument order and explicit `Plane` input for linear dimensions. Two-point leaders use the documented deterministic WorldXY/vertical-segment plane rule rather than depending on the active viewport. Named-style setters retain `unit` returns. |
| [Document](../Src/Scripting_Document.fs) | `DocumentPath` deliberately returns the full filename, as its XML docs state. Keep explicit redraw control, preview exceptions and `unit` setters. Unit system 0 is supported intentionally; the pre-existing changelog records that correction. |
| [Geometry](../Src/Scripting_Geometry.fs) | Keep typed `Box`/`BoundingBox` results, point-sequence search inputs and separate text alignment values. Point-based text uses the documented WorldXY orientation, so scripts are independent of viewport state. Empty point-cloud color/hidden-state sequences explicitly clear that state. |
| [Grips](../Src/Scripting_Grips.fs) | Keep validation of negative indices and point-count mismatches, explicit cancellation exceptions and empty collections. These avoid Python indexing accidents and silently ignored edits; mismatch/cancellation behavior is recorded in the pre-existing changelog. |
| [Group](../Src/Scripting_Group.fs) | Keep the corrected `IsGroupEmpty` logic: the supplied Python implementation is inverted. Keep mutation returns and missing-group errors consistent with the F# conventions. |
| [Hatch](../Src/Scripting_Hatch.fs) | Keep stable pattern-name inputs, typed single/collection methods and geometry helpers. Setting scale 0 and honoring replacement are useful corrections to Python behavior, rather than bugs to reproduce. |
| [Layer](../Src/Scripting_Layer.fs) | Keep documented name-based operations, random default colors, parent-aware integer visibility/locking states and the returned layer-table index. Keep explicit visibility/locking helpers; these expose parent and persistence behavior clearly. `LayerId` returns a typed `Guid`. |
| [Light](../Src/Scripting_Light.fs) | Keep the flat typed `(Plane, length, width)` result. Zero-valued spotlight setters remain effective instead of reproducing Python truthiness no-ops. |
| [Line](../Src/Scripting_Line.fs) | Keep typed `Line` inputs and explicit failure exceptions. The geometry `LineTransform` overload returns geometry; its `Guid` overload performs a document-object operation and preserves document attributes. |
| [Linetype](../Src/Scripting_Linetype.fs) | Keep name-based lookup and `ActiveCount`, which counts non-deleted linetypes consistently with the name listing. Deleted table slots should not inflate the public count. |
| [Material](../Src/Scripting_Material.fs) | Keep typed colors, effective zero/empty-name setters and local-file validation for bitmap setters. Early missing-file errors are deliberate; callers needing Rhino's broader resolution can use RhinoCommon. Copying an in-range material index to itself is a successful no-op. |
| [Mesh](../Src/Scripting_Mesh.fs) | Keep separate intersection point/index arrays and scalar area/volume returns. Scalar totals compose directly with arithmetic; input counts are known, and detailed error estimates remain available through RhinoCommon mass-property objects. Empty vertex-color sequences clear colors. The initially reported boolean input-deletion issue was already fixed in the existing source and changelog. |
| [Object](../Src/Scripting_Object.fs) | Keep layout names and the empty-name model-space convention, typed transformations, `forceVisible`/`ignoreErrors` controls, and documented mutation returns. `ObjectLayer` and `ObjectGroups` are implemented in the Layer and Group files. |
| [Plane](../Src/Scripting_Plane.fs) | Keep typed `Line` and `(intersectionType, Plane, radius)` results and the separate `PlaneClosestParameter` method. For a sphere point intersection, the point is the returned plane's origin; the earlier audit's claim that this loses the point was incorrect. |
| [PointVector](../Src/Scripting_PointVector.fs) | Keep closest-object distance in the result, typed bounding boxes, sequence-based projection inputs and explicit division validation. An unsuccessful closest-array lookup raises a scripting exception rather than returning Python's None. |
| [Selection](../Src/Scripting_Selection.fs) | Keep documented `preselect=true`, typed return values, named F# options and cancellation exceptions. `VisibleObjectsInView` is an intentional clearer name and already documents its Python correspondence. Subobject references are now available through `GetObjectRef`. |
| [Surface](../Src/Scripting_Surface.fs) | Keep explicit Plane/Point3d construction inputs and the documented WorldZ cut-plane default. Keep `SurfaceClosestParameter`, `SurfaceEditPointParameters` and `TrimSurfaceU/V/UV` as typed query/operation splits. Scalar mass-property results follow the same design as Mesh. `JoinSurfaces` guarantees one result; `JoinSurfacesAll` now covers multiple results. |
| [Toolbar](../Src/Scripting_Toolbar.fs) | Keep typed empty/missing-resource results and exceptions consistent with the F# method documentation. |
| [Transformation](../Src/Scripting_Transformation.fs) | Keep typed transforms, planes and points, and explicit view names. Nonuniform scaling about the origin is available by passing `Point3d.Origin`; no geometry operation is missing. |
| [UserData](../Src/Scripting_UserData.fs) | Keep separate key listing, entry lookup, set and delete methods. This avoids dynamic return shapes and ambiguous deletion through an empty setter value. Keep documented string/Unicode validation. |
| [UserInterface](../Src/Scripting_UserInterface.fs) | Keep typed `Line` returns and cancellation exceptions. Python's `GetPolyline.flags` and `GetLinetype.show_by_layer` parameters are unused by its implementation, so their omission loses no implemented mode. |
| [Utility](../Src/Scripting_Utility.fs), [Coerce](../Src/Scripting_Coerce.fs), [Header](../Src/Scripting_Header.fs) | Keep the documented typed coercion redesign. `Clamp`, `FxrangePython` and `FrangePython` already exist in Header; the earlier missing-helper finding was incorrect. `GetSettings` is deliberately excluded to keep dependencies small, with a source comment recommending direct use of `ini-parser`. Map scalar `Distance` over a sequence when needed; pass a construction plane explicitly to `Angle`. |
| [Views](../Src/Scripting_Views.fs) | Keep explicit view-name/Plane inputs, display-mode ID helpers, richer layout results and setter overloads. `AddNamedCPlane` accepts an explicit plane, allowing construction independently of the active viewport. The `ViewRadius` setter's omitted mode is now supported. |

Examples of the retained composable operations:

```fsharp
// Add document points only when that side effect is wanted.
let pointIds = rs.AddPoints(rs.DivideCurveIntoPoints(curveId, 10))

// Apply the scalar distance operation to many points.
let distances = points |> Seq.map (fun point -> rs.Distance(origin, point))

// Nonuniform scaling about the world origin.
let transform = rs.XformScale(2.0, 3.0, 4.0, Point3d.Origin)

// Detailed mass-property error estimates remain available through RhinoCommon.
use properties = AreaMassProperties.Compute(rs.CoerceMesh(meshId))
let area, errorEstimate = properties.Area, properties.AreaError
```

The examples use `type rs = Rhino.Scripting.RhinoScriptSyntax` and `open Rhino.Geometry`. Mass-property computation can fail for unsuitable input; check its result in production code.

## Validation

- `dotnet fsi API-Table/ApiTable.fsx` successfully type-checked the public API and regenerated its table after the public changes.
- `dotnet build ForEditing.fsproj -c Release --nologo` passed with 0 warnings and 0 errors.
- `dotnet build ForPublishing.fsproj -c Release --nologo` passed for net48, net8.0 and net10.0 with 0 warnings and 0 errors, and produced the NuGet packages.
- The publishing build confirmed that the combined source was current. `git diff --check` passed.
- Independent source reviews checked the new overloads, selection-reference ownership, join rollback and disposal. No Rhino viewport interaction was executed; that requires validation inside Rhino.
