# Bugs found by the test scripts, with suggested fixes

> Progress: the failing cases in sections 1 and 2 and the hatch and knot cases have been fixed and checked in Rhino 9, along with the unset Notes and BlockDescription cases. RenameBlock already used quiet=true; a duplicate-name regression test now passes. The other observations below still need review. The historical descriptions below have not yet been reconciled.

Found while writing and running the suites in this folder (Rhino 8.36, `net8.0` build). Each entry names the failing test, so
`caddis_eval path=Tests/Test_<File>.fsx` shows the failure. The failing tests are left as they are on purpose: they pass once the bug is fixed.

Nothing in `Src/` has been changed yet. The suggested fixes were reasoned from the source and the observed behavior; the ones marked **(verified)** were also tried out in Rhino.
After a fix in a `Scripting_*.fs` file, follow AGENTS.md: rebuild `ForPublishing.fsproj`, commit the regenerated file, add a CHANGELOG entry (`Breaking:` if a return value changes),
and run `dotnet fsi API-Table/ApiTable.fsx` if a signature changed.

## 1. Wrong results

### IsGoodStringId accepts "" when allowEmpty = false
- Test: `Test_Object.fsx` / `IsGoodStringId` ("empty string is not good by default").
- Also breaks: `SetDocumentUserText("", "v")` is accepted instead of raising (`Test_UserData.fsx` / `SetDocumentUserText`, "empty key").
- Cause: [UtilString.fs](../Src/UtilString.fs) `isGoodStringId` and `isASCIIStringId` have `elif allowEmpty && name = "" then true`. For `""` and `allowEmpty = false` the code falls through, the loop ends at once and returns `true`.
- Fix, in both functions:
  ```fsharp
  if isNull name then false
  elif name = "" then allowEmpty
  else ...
  ```

### GroupCount counts deleted groups
- Test: `Test_Group.fsx` / `GroupCount` ("back to the old count": expected 2, got 3).
- Cause: [Scripting_Group.fs](../Src/Scripting_Group.fs) returns `State.Doc.Groups.Count`, which includes deleted table entries (deleting a group only marks it).
- Fix: `(RhinoScriptSyntax.GroupNames()).Length`; `GroupNames` already skips deleted groups through `Groups.GroupNames(true)`.

### DeleteDocumentUserText leaves the key in place
- Test: `Test_UserData.fsx` / `DeleteDocumentUserText` ("key is removed").
- Cause: [Scripting_UserData.fs](../Src/Scripting_UserData.fs) calls `State.Doc.Strings.SetString(key, null)`. That returns the old value, but does not remove the key.
- Fix: check `State.Doc.Strings.GetValue(key)` for null (raise "does not exist" as before), then call `State.Doc.Strings.Delete(key)`.

### ClosedCurveOrientation returns 0 with the default direction
- Test: `Test_Curve.fsx` / `ClosedCurveOrientation` ("counter clockwise": expected 1, got 0).
- Cause: [Scripting_Curve.fs](../Src/Scripting_Curve.fs), geometry overload: `if direction.IsZero then Vector3d.Unset`. **(verified)** `Curve.ClosedCurveOrientation(Vector3d.Unset)` returns `Undefined`, `Curve.ClosedCurveOrientation(Vector3d.ZAxis)` returns `CounterClockwise`. The doc string promises `Vector3d.ZAxis` as default.
- Fix: `let direction0 = if direction.IsZero then Vector3d.ZAxis else direction`.

### DimStyleFont getter returns the face, not the font
- Test: `Test_Dimension.fsx` / `DimStyleFont` ("font round trip": expected "Arial", got "Regular").
- Cause: [Scripting_Dimension.fs](../Src/Scripting_Dimension.fs) getter returns `ds.Font.FaceName`. **(verified)** For `Font("Arial")`, `FaceName = "Regular"`, `FamilyName = "Arial"`, `QuartetName = "Arial"`.
- Fix: return `ds.Font.QuartetName` (what the setter and `AddText` take as input) or `FamilyName`. Also adjust the doc string, which says "font face name".

### AddText ignores fontStyle
- Test: `Test_Geometry.fsx` / `AddText (font style)` ("the font style 1 (bold) is applied": expected 1, got 0, read back with `TextObjectStyle`).
- Where: [Scripting_Geometry.fs](../Src/Scripting_Geometry.fs) `AddText(text, plane, ...)`. The bold/italic flags are only applied through `te.SetBold` / `SetItalic` when `FromQuartetProperties(qn, bold, false)` is not null, and `SetBold(...)`'s result is ignored.
- Not yet diagnosed which step fails (font lookup, `SetBold`, or `TextObjectStyle` reading it back). Next step: in a scratch document add text with `font="Arial", fontStyle=1`, then inspect `te.Font.Bold`, `te.Font.QuartetName` and the result of `te.SetBold(true)`.
  If `SetBold` works but the entity does not report it, create the font with `Font.FromQuartetProperties(qn, bold, italic)` directly and assign `te.Font <- f` (note that the current code only passes one flag at a time, so bold+italic would also lose one).

### UnifyMeshNormals counts but does not change the document mesh
- Test: `Test_Mesh.fsx` / `UnifyMeshNormals` ("all face normals point in the same direction afterwards": expected 1 direction, got 2). The returned count (1) is right.
- Likely cause: [Scripting_Mesh.fs](../Src/Scripting_Mesh.fs) modifies the mesh returned by `CoerceMesh`, which is the object's own geometry, and then calls `Objects.Replace(id, mesh)` with that same instance, which does not take it.
- Suggested fix (untested): `let mesh = (RhinoScriptSyntax.CoerceMesh objectId).DuplicateMesh()` before `UnifyNormals()`, then `Replace`. Check other in-place `Replace(id, sameInstance)` patterns for the same problem.

### RestoreNamedView renames the target view
- Test: `Test_Views.fsx` / `RestoreNamedView (the view keeps its title)`.
- Observed: after `RestoreNamedView("n", "Front")` the view is called "n" and `IsView "Front"` is false. `ViewportInfo.Name` is overwritten by `PushViewInfo`.
- Suggested fix (untested): remember `view.MainViewport.Name` before `PushViewInfo` and set it back afterwards. Note that the original rhinoscriptsyntax calls the same RhinoCommon function, so this may be a deliberate Rhino behavior; if so, document it in the doc string instead.

## 2. Null instead of empty, or exceptions of the wrong kind

| Function | Test | Observed | Suggested fix |
|---|---|---|---|
| `CullDuplicatePoints []` | `Test_Utility.fsx` | returns null (`Point3d.CullDuplicates` returns null for no points) | `let r = Point3d.CullDuplicates(points, tol) in if isNull r then [\|\|] else r` |
| `MeshMeshIntersection` of meshes that do not touch | `Test_Mesh.fsx` | returns null | map null to `[\|\|]` (doc says "Array of points") |
| `MeshVertexFaces(mesh, badIndex)` | `Test_Mesh.fsx` | returns null | raise `RhinoScriptingException` when `vertexIndex` is outside `0 .. mesh.Vertices.Count-1`, and map a null result to `[\|\|]` |
| `BlockDescription` getter, `Notes` getter | no failing test: `Test_Block.fsx` and `Test_Document.fsx` accept null or empty, a comment says so | return null when unset | `if isNull d then "" else d`, as `ObjectName` already does |
| `AddCone`, `AddTorus` with zero radius / zero height | `Test_Surface.fsx` | return `Guid.Empty` (RhinoCommon returns a null Brep, `Objects.AddBrep null` gives `Guid.Empty`) | raise like `AddCylinder`/`AddSphere` do: check the brep for null and `Guid.Empty` after adding |
| `ExplodeText` on a non-text object | `Test_Geometry.fsx` | `InvalidCastException` from `:?> TextEntity` | `match geometry with :? TextEntity as t -> ... \| _ -> RhinoScriptingException.Raise "ExplodeText: object %s is not a text" ...` |
| `IsWallpaper` with no wallpaper | `Test_Views.fsx` | `NullReferenceException`, `WallpaperFilename` is null | `not (String.IsNullOrEmpty view.MainViewport.WallpaperFilename)` |
| `InCommand` | `Test_Application.fsx` | `ArgumentNullException`, **(verified)** `Command.GetCommandStack()` is null when no command runs | `let s = Commands.Command.GetCommandStack() in if isNull s then 0 else s.Length` |

## 3. Other issues

### RemoveSurfaceKnot works on the wrong direction
- Test: `Test_Surface.fsx` / `RemoveSurfaceKnot`.
- **(verified)** On a 6x7 point cubic surface (knot counts u=8, v=9): `vDirection=false` leaves u at 8 and reduces v to 8; `vDirection=true` reduces u to 7. Directions are swapped.
- Where: [Scripting_Surface.fs](../Src/Scripting_Surface.fs), `knots.RemoveKnotsAt(nuparam, nvparam)` is called on `KnotsU` or `KnotsV` with the pair (u, v) as if it were a parameter range, and RhinoCommon applies it to the other direction.
- Suggested fix: pass the parameter that belongs to the chosen direction as both ends (`KnotsU.RemoveKnotsAt(nu, nu)`, `KnotsV.RemoveKnotsAt(nv, nv)`), and re-check with the test. The original rhinoscriptsyntax has the same call, so compare against Rhino's `RemoveKnot` command.

### Hatch creation fails when the document has no current hatch pattern
- Test: `Test_Hatch.fsx` / `AddHatch (document without a current hatch pattern)`.
- Cause: [Scripting_Hatch.fs](../Src/Scripting_Hatch.fs) `MakeHatches` uses `HatchPatterns.CurrentHatchPatternIndex`, which is -1 in a document created from the default template, then `Hatch.Create` fails with that index.
- Fix: after `InitHatchPatterns()`, if the index is below 0 use the first pattern of the table (index 0), or raise a message that says "no current hatch pattern" instead of the generic "failed to create hatch".

### Other `quiet=false` calls can open dialogs
- `RenameBlock` used to freeze Rhino behind a hidden "name already exists" message box; it now passes `quiet=true`, and `Test_Block.fsx` checks a duplicate name. Audit the other `quiet=false` calls the same way:
  `DeleteBlock` (`InstanceDefinitions.Delete(..., quiet=false)`), `Scripting_Curve.fs` around line 3018 (`Objects.Delete(id, quiet=false)`), and the many `DimStyles.Modify(ds, ds.Id, quiet=false)` calls in `Scripting_Dimension.fs`.
  Rule for all scripted code: never let RhinoCommon ask a person; use `quiet=true` and report the failure through the return value or an exception.

### ChangeSurfaceDegree can only raise the degree
- Test: `Test_Surface.fsx` / `ChangeSurfaceDegree` documents the current behavior (lowering returns `false`).
- Cause: only `IncreaseDegreeU/V` are called; the doc string says "Changes the degree".
- Fix options: state "raises the degree" in the doc string, or rebuild the surface (`Surface.Rebuild(degreeU, degreeV, pointCountU, pointCountV)`) when a lower degree is requested.

### SdkVersion doc string is outdated
- Test: `Test_Application.fsx` / `SdkVersion` now only checks the pass through.
- **(verified)** Rhino 8.36 returns the `Int32` -1946131191 (RhinoCommon's own value), not a 9 digit `YYYYMMDDn` number. Fix the doc string; if a positive value is wanted, return `uint32` (breaking, needs a CHANGELOG `Breaking:` entry).

### Smaller observations (behavior is arguably right, doc could say it)
- `ShrinkTrimmedSurface` raises "failed" when there is nothing to shrink (`Faces.ShrinkFaces()` returns false). Consider returning the unchanged copy instead (not compared against the Python original).
- `ShootRay` returns the start point as the first element of the result.
- `SurfaceAreaMoments` / `SurfaceVolumeMoments` return 14 tuples (value and error bound pairs), the summary line of the doc strings does not say how many.
- `IsPointOnSurface` raises for polysurfaces (matches the original, which needs a surface); the doc string says "Surface" only.
- `AddPipe` parameters are normalized (0..1), as documented, but it throws a plain `ArgumentException` for unequal parameter and radius counts instead of `RhinoScriptingException`.
- `BooleanIntersection` of non-overlapping breps returns an empty list instead of raising, while `AddSweep1/2` and `FilletSurfaces` also return empty lists on failure. Decide on one rule (AGENTS.md prefers raising).
- `MeshBooleanDifference` etc. raise when RhinoCommon returns an invalid ngon mesh (seen with coplanar 6-division boxes). Fine, but a clearer message would help.
