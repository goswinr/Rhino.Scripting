# Rhino.Scripting API

All public types, members and functions of Rhino.Scripting, with the first sentence of their summary.

- The 1664 static members of `RhinoScriptSyntax`, 1038 names with their overloads, have one table per source file,
  e.g. [Curve](#curve) for `Src/Scripting_Curve.fs`. Scripts usually call them through an alias, `type rs = RhinoScriptSyntax`.
- Overloads of the same name are in the order of the source.
- Geometry-only `Make...` methods are listed immediately after their corresponding document-adding `Add...` methods.
- `?name: type = value` marks an optional parameter and its default value.
- In the other tables, instance members and fields start with `.`, e.g. `.Pretty`. Static members are qualified with their type name.
- A property shows only its type. `with get, set` marks a property that can be set.

Generated from the sources by `API-Table/ApiTable.fsx`. Do not edit by hand.

## Contents

**RhinoScriptSyntax**

- [Header](#header), 34 members
- [Application](#application), 79 members
- [Block](#block), 29 members
- [Coerce](#coerce), 103 members
- [Curve](#curve), 247 members
- [Dimension](#dimension), 71 members
- [Document](#document), 51 members
- [Geometry](#geometry), 101 members
- [Grips](#grips), 17 members
- [Group](#group), 20 members
- [Hatch](#hatch), 30 members
- [Layer](#layer), 61 members
- [Light](#light), 56 members
- [Line](#line), 15 members
- [Linetype](#linetype), 5 members
- [Material](#material), 25 members
- [Mesh](#mesh), 97 members
- [Object](#object), 104 members
- [Plane](#plane), 21 members
- [PointVector](#pointvector), 37 members
- [Selection](#selection), 25 members
- [Surface](#surface), 210 members
- [Toolbar](#toolbar), 15 members
- [Transformation](#transformation), 29 members
- [UserData](#userdata), 21 members
- [UserInterface](#userinterface), 42 members
- [Utility](#utility), 41 members
- [Views](#views), 78 members

**Other types, modules and extensions**

- [RhinoScriptingException](#rhinoscriptingexception), 5 members
- [RhinoUserInteractionException](#rhinouserinteractionexception), 2 members
- [module RhinoScriptingUtils.PrettySettings](#module-rhinoscriptingutilsprettysettings), 2 members
- [module RhinoScriptingUtils.PrettyFormat](#module-rhinoscriptingutilsprettyformat), 2 members
- [module RhinoScriptingUtils.AutoOpenRsUtils](#module-rhinoscriptingutilsautoopenrsutils), 14 members
- [Array extensions](#array-extensions), 1 member
- [HashSet extensions](#hashset-extensions), 1 member
- [ResizeArray extensions](#resizearray-extensions), 1 member
- [RhinoSync](#rhinosync), 11 members
- [Point3d extensions](#point3d-extensions), 1 member
- [Point3f extensions](#point3f-extensions), 1 member
- [Vector3d extensions](#vector3d-extensions), 1 member
- [Vector3f extensions](#vector3f-extensions), 1 member
- [Line extensions](#line-extensions), 1 member
- [Transform extensions](#transform-extensions), 1 member
- [Plane extensions](#plane-extensions), 1 member
- [BoundingBox extensions](#boundingbox-extensions), 1 member
- [ObjectFilterEnum](#objectfilterenum), 21 members

## Header

From [Src/Scripting_Header.fs](../Src/Scripting_Header.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `Add` | `geo: GeometryBase * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds any geometry object that is a subclass of GeometryBase to the Rhino document, such as a Curve, Brep, Mesh or Point. |
| `Add` | `geo: GeometryBase * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds any geometry object that is a subclass of GeometryBase to the Rhino document, such as a Curve, Brep, Mesh or Point. |
| `Add` | `geo: Point3d * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Point3d to the Rhino document as a point object. |
| `Add` | `geo: Point3d * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Point3d to the Rhino document as a point object. |
| `Add` | `geo: Point3f * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Point3f to the Rhino document as a point object. |
| `Add` | `geo: Point3f * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Point3f to the Rhino document as a point object. |
| `Add` | `geo: Line * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Line to the Rhino document as a line curve. |
| `Add` | `geo: Line * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Line to the Rhino document as a line curve. |
| `Add` | `geo: Arc * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds an Arc to the Rhino document as an arc curve. |
| `Add` | `geo: Arc * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds an Arc to the Rhino document as an arc curve. |
| `Add` | `geo: Circle * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Circle to the Rhino document as a curve. |
| `Add` | `geo: Circle * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Circle to the Rhino document as a curve. |
| `Add` | `geo: Ellipse * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds an Ellipse to the Rhino document as a curve. |
| `Add` | `geo: Ellipse * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds an Ellipse to the Rhino document as a curve. |
| `Add` | `geo: Polyline * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Polyline to the Rhino document as a polyline curve. |
| `Add` | `geo: Polyline * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Polyline to the Rhino document as a polyline curve. |
| `Add` | `geo: Box * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Box to the Rhino document. |
| `Add` | `geo: Box * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Box to the Rhino document. |
| `Add` | `geo: BoundingBox * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a BoundingBox to the Rhino document as a box. |
| `Add` | `geo: BoundingBox * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a BoundingBox to the Rhino document as a box. |
| `Add` | `geo: Sphere * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Sphere to the Rhino document. |
| `Add` | `geo: Sphere * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Sphere to the Rhino document. |
| `Add` | `geo: Cylinder * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Cylinder to the Rhino document as a NURBS surface. |
| `Add` | `geo: Cylinder * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Cylinder to the Rhino document as a NURBS surface. |
| `Add` | `geo: Cone * layerIndex: int * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Cone to the Rhino document as a NURBS surface. |
| `Add` | `geo: Cone * ?layer: string = "" * ?objectName: string = "" * ?userTextKeysAndValues: (string * string) seq = null * ?layerColor: Color = Drawing.Color() * ?stringSafetyCheck: bool = true * ?collapseParents: bool = false -> Guid` | Adds a Cone to the Rhino document as a NURBS surface. |
| `Clamp` | `minVal: float * maxVal: float * value: float -> float` | Clamps a value between a lower and an upper bound. |
| `Doc` | `RhinoDoc` | The current active Rhino document (= the file currently open) |
| `EscapeTest` | `unit -> unit` | Tests to see if the user has pressed the escape key. |
| `Filter` | `ObjectFilterEnum` | An Integer Enum of Object types. |
| `FrangePython` | `start: float * stop: float * step: float -> ResizeArray<float>` | Like the Python 'range' function for integers, this creates a range of floating point values. |
| `FxrangePython` | `start: float * stop: float * step: float -> float seq` | Like the Python 'xrange' function for integers, this creates a range of floating point values. |
| `Ot` | `ObjectTable` | Object Table of the current active Rhino document |
| `Sticky` | `Dict<string,obj>` | A Dictionary to store state between scripting session. |

## Application

From [Src/Scripting_Application.fs](../Src/Scripting_Application.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddAlias` | `alias: string * macro: string -> bool` | Add new command alias to Rhino. |
| `AddSearchPath` | `folder: string * ?index: int = -1 -> int` | Add new path to Rhino's search path list. |
| `AliasCount` | `unit -> int` | Returns number of command aliases in Rhino. |
| `AliasMacro` | `alias: string -> string` | Returns the macro of a command alias. |
| `AliasMacro` | `alias: string * macro: string -> unit` | Modifies the macro of a command alias. |
| `AliasNames` | `unit -> string array` | Returns an array of command alias names. |
| `AppearanceColor` | `item: int -> Color` | Returns an application interface item's color. |
| `AppearanceColor` | `item: int * color: Color -> unit` | Modifies an application interface item's color. |
| `AutosaveFile` | `unit -> string` | Returns the file name used by Rhino's automatic file saving. |
| `AutosaveFile` | `filename: string -> unit` | Changes the file name used by Rhino's automatic file saving. |
| `AutosaveInterval` | `unit -> float` | Returns how often the document will be saved when Rhino's automatic file saving mechanism is enabled. |
| `AutosaveInterval` | `minutes: float -> unit` | Changes how often the document will be saved when Rhino's automatic file saving mechanism is enabled. |
| `BuildDate` | `unit -> DateTime` | Returns the build date of Rhino. |
| `ClearCommandHistory` | `unit -> unit` | Clears contents of Rhino's command history window. |
| `Command` | `commandString: string * ?echo: bool = true -> bool` | Runs a Rhino command macro / script. |
| `CommandHistory` | `unit -> string` | Returns the contents of Rhino's command history window. |
| `DefaultRenderer` | `unit -> string` | Returns the default render plug-in. |
| `DefaultRenderer` | `renderer: string -> bool` | Changes the default render plug-in. |
| `DeleteAlias` | `alias: string -> bool` | Delete an existing alias from Rhino. |
| `DeleteSearchPath` | `folder: string -> bool` | Removes existing path from Rhino's search path list. |
| `DisplayOleAlerts` | `enable: bool -> unit` | Enables/disables OLE Server Busy/Not Responding dialog boxes. |
| `EdgeAnalysisColor` | `unit -> Color` | Returns edge analysis color displayed by the ShowEdges command. |
| `EdgeAnalysisColor` | `color: Color -> unit` | Modifies edge analysis color displayed by the ShowEdges command. |
| `EdgeAnalysisMode` | `unit -> int` | Returns edge analysis mode displayed by the ShowEdges command. |
| `EdgeAnalysisMode` | `mode: int -> unit` | Modifies edge analysis mode displayed by the ShowEdges command. |
| `EnableAutosave` | `?enable: bool = true -> unit` | Enables or disables Rhino's automatic file saving mechanism. |
| `EnablePlugIn` | `plugin: string -> bool` | Get status of a Rhino plug-in. |
| `EnablePlugIn` | `plugin: string * enable: bool -> unit` | Enables or disables a Rhino plug-in. |
| `ExeFolder` | `unit -> string` | Returns the full path to Rhino's executable folder. |
| `ExePlatform` | `unit -> int` | Returns the platform of the Rhino executable , calls System.Environment.Is64BitProcess. |
| `ExeServiceRelease` | `unit -> int` | Returns the service release number of the Rhino executable. |
| `ExeVersion` | `unit -> int` | Returns the major version number of the Rhino executable. |
| `Exit` | `unit -> unit` | Closes the Rhino application. |
| `FindFile` | `filename: string -> string` | Searches for a file using Rhino's search path. |
| `GetPlugInObject` | `plugIn: string -> obj` | Returns a scriptable object from a specified plug-in. |
| `InCommand` | `unit -> int` | Determines if Rhino is currently running a command. |
| `InstallFolder` | `unit -> string` | The full path to Rhino's installation folder. |
| `IsAlias` | `alias: string -> bool` | Checks if a command alias exists in Rhino. |
| `IsCommand` | `commandName: string -> bool` | Checks if a command exists in Rhino. |
| `IsPlugIn` | `plugin: string -> bool` | Checks if a plug-in is registered. |
| `IsRunningOnWindows` | `unit -> bool` | Returns True if this script is being executed on a Windows platform. |
| `LastCommandName` | `unit -> string` | Returns the name of the last executed command. |
| `LastCommandResult` | `unit -> int` | Returns the result code for the last executed command. |
| `LocaleID` | `unit -> int` | Returns the current language used for the Rhino interface. |
| `Ortho` | `unit -> bool` | Get status of Rhino's ortho modeling aid. |
| `Ortho` | `enable: bool -> unit` | Enables or disables Rhino's ortho modeling aid. |
| `Osnap` | `unit -> bool` | Get status of Rhino's object snap modeling aid. |
| `Osnap` | `enable: bool -> unit` | Enables or disables Rhino's object snap modeling aid. |
| `OsnapDialog` | `unit -> bool` | Get status of Rhino's dockable object snap bar. |
| `OsnapDialog` | `visible: bool -> unit` | Shows or hides Rhino's dockable object snap bar. |
| `OsnapMode` | `unit -> int` | Returns the object snap mode. |
| `OsnapMode` | `mode: int -> unit` | Sets the object snap mode. |
| `Planar` | `unit -> bool` | Get status of Rhino's planar modeling aid. |
| `Planar` | `enable: bool -> unit` | Enables or disables Rhino's planar modeling aid. |
| `PlugInId` | `plugin: string -> Guid` | Returns the identifier of a plug-in given the plug-in name. |
| `PlugIns` | `?types: int = 0 * ?status: int = 0 -> string array` | Returns an array of registered Rhino plug-ins. |
| `ProjectOsnaps` | `unit -> bool` | Get status of object snap projection. |
| `ProjectOsnaps` | `enable: bool -> unit` | Enables or disables object snap projection. |
| `Prompt` | `?message: string = "" -> unit` | Change Rhino's command window prompt. |
| `ScreenSize` | `unit -> int * int` | Returns current width and height, of the screen of the primary monitor. |
| `SdkVersion` | `unit -> int` | Returns version of the Rhino SDK supported by the executing Rhino. |
| `SearchPathCount` | `unit -> int` | Returns the number of path items in Rhino's search path list. |
| `SearchPathList` | `unit -> string array` | Returns all of the path items in Rhino's search path list. |
| `SendKeystrokes` | `?keys: string = "" * ?addReturn: bool = true -> unit` | Sends a string of printable characters to Rhino's Commandline. |
| `Snap` | `unit -> bool` | Get status of Rhino's grid snap modeling aid. |
| `Snap` | `enable: bool -> unit` | Enables or disables Rhino's grid snap modeling aid. |
| `StatusBarDistance` | `?distance: float = 0.0 -> unit` | Sets Rhino's status bar distance pane. |
| `StatusBarMessage` | `?message: string = "" -> unit` | Sets Rhino's status bar message pane. |
| `StatusBarPoint` | `?point: Point3d = Point3d() -> unit` | Sets Rhino's status bar point coordinate pane. |
| `StatusBarProgressMeterHide` | `unit -> unit` | Hide the progress meter. |
| `StatusBarProgressMeterShow` | `label: string * lower: int * upper: int * ?embedLabel: bool = true * ?showPercent: bool = true -> bool` | Start the Rhino status bar progress meter. |
| `StatusBarProgressMeterUpdate` | `position: int * ?absolute: bool = true -> unit` | Set the current position of the progress meter. |
| `TemplateFile` | `unit -> string` | Returns Rhino's default template file. |
| `TemplateFile` | `filename: string -> unit` | Sets Rhino's default template file. |
| `TemplateFolder` | `unit -> string` | Returns the location of Rhino's template folder. |
| `TemplateFolder` | `folder: string -> unit` | Sets the location of Rhino's template folder. |
| `WindowHandle` | `unit -> IntPtr` | Returns the windows handle of Rhino's main window. |
| `WorkingFolder` | `unit -> string` | Returns Rhino's working folder (directory). |
| `WorkingFolder` | `folder: string -> unit` | Sets Rhino's working folder (directory). |

## Block

From [Src/Scripting_Block.fs](../Src/Scripting_Block.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddBlock` | `objectIds: Guid seq * basePoint: Point3d * ?name: string = "" * ?deleteInput: bool = false -> string` | Adds a new block definition to the document. |
| `BlockContainerCount` | `blockName: string -> int` | Returns number of block definitions that contain a specified block definition. |
| `BlockContainers` | `blockName: string -> ResizeArray<string>` | Returns names of the block definitions that contain a specified block definition. |
| `BlockCount` | `unit -> int` | Returns the number of block definitions in the document. |
| `BlockDescription` | `blockName: string -> string` | Returns the description of a block definition. |
| `BlockDescription` | `blockName: string * description: string -> unit` | Sets the description of a block definition. |
| `BlockInstanceCount` | `blockName: string * ?whereToLook: int = 0 -> int` | Counts number of instances of the block in the document. |
| `BlockInstanceInsertPoint` | `objectId: Guid -> Point3d` | Returns the insertion point of a block instance. |
| `BlockInstanceName` | `objectId: Guid -> string` | Returns the block name of a block instance. |
| `BlockInstances` | `blockName: string * ?whereToLook: int = 0 -> ResizeArray<Guid>` | Returns the identifiers of the inserted instances of a block. |
| `BlockInstanceXform` | `objectId: Guid -> Transform` | Returns the location of a block instance relative to the world coordinate system origin (0, 0, 0). |
| `BlockNames` | `unit -> ResizeArray<string>` | Returns the names of all block definitions in the document. |
| `BlockNames` | `sort: bool -> ResizeArray<string>` | Returns the names, optionally sorted in ordinal order, like in RhinoPython. |
| `BlockObjectCount` | `blockName: string -> int` | Returns number of objects that make up a block definition. |
| `BlockObjects` | `blockName: string -> ResizeArray<Guid>` | Returns identifiers of the objects that make up a block definition. |
| `BlockPath` | `blockName: string -> string` | Returns path to the source of a linked or embedded block definition. |
| `BlockStatus` | `blockName: string -> int` | Returns the status of a linked block. |
| `DeleteBlock` | `blockName: string -> bool` | Deletes a block definition and all of it's inserted instances. |
| `ExplodeBlockInstance` | `objectId: Guid * ?explodeNestedInstances: bool = false -> Guid array` | Explodes a block instance into it's geometric components. |
| `ExplodeBlockInstance` | `geometryObject: InstanceReferenceGeometry * ?explodeNestedInstances: bool = false -> GeometryBase array` | Explodes block reference geometry into transformed copies of its component geometry. |
| `InsertBlock` | `blockName: string * insertionPoint: Point3d * ?scale: Vector3d = Vector3d() * ?angleDegrees: float = 0.0 * ?rotationNormal: Vector3d = Vector3d() -> Guid` | Inserts a block whose definition already exists in the document. |
| `InsertBlock2` | `blockName: string * xForm: Transform -> Guid` | Inserts a block whose definition already exists in the document. |
| `IsBlock` | `blockName: string -> bool` | Checks if the existence of a block definition in the document. |
| `IsBlockEmbedded` | `blockName: string -> bool` | Checks if a block definition is embedded, or linked, from an external file. |
| `IsBlockInstance` | `objectId: Guid -> bool` | Checks if an object is a block instance. |
| `IsBlockInUse` | `blockName: string * ?whereToLook: int = 0 -> bool` | Checks if a block definition is being used by an inserted instance. |
| `IsBlockReference` | `blockName: string -> bool` | Checks if that a block definition is from a reference file. |
| `RenameBlock` | `blockName: string * newName: string -> bool` | Renames an existing block definition. |
| `ReplaceBlockObjects` | `blockName: string * newObjects: Guid seq * ?deleteInput: bool = true -> bool` | Replaces the objects inside an existing block definition. |

## Coerce

From [Src/Scripting_Coerce.fs](../Src/Scripting_Coerce.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `Coerce2dPoint` | `point: Point2d -> Point2d` | Returns the given Point2d. |
| `Coerce2dPoint` | `point: Point3d -> Point2d` | Converts a Point3d into a Rhino.Geometry.Point2d by dropping its Z coordinate. |
| `Coerce2dPoint` | `x: float * y: float -> Point2d` | Converts two floats into a Rhino.Geometry.Point2d. |
| `Coerce3dPoint` | `point: Point3d -> Point3d` | Returns the given Point3d. |
| `Coerce3dPoint` | `point: Point3f -> Point3d` | Converts a Point3f into a Rhino.Geometry.Point3d. |
| `Coerce3dPoint` | `x: float * y: float * z: float -> Point3d` | Converts three floats into a Rhino.Geometry.Point3d. |
| `Coerce3dPoint` | `xyz: float seq -> Point3d` | Converts a sequence of exactly three floats into a Rhino.Geometry.Point3d. |
| `Coerce3dPoint` | `point: string -> Point3d` | Parses a string into a Rhino.Geometry.Point3d. |
| `Coerce3dPoint` | `objectId: Guid -> Point3d` | Gets the location of a point object in the document. |
| `Coerce3dPoint` | `pointObject: PointObject -> Point3d` | Gets the location of a PointObject. |
| `Coerce3dPoint` | `textDot: TextDot -> Point3d` | Gets the location of a TextDot. |
| `Coerce3dVector` | `vector: Vector3d -> Vector3d` | Returns the given Vector3d. |
| `Coerce3dVector` | `vector: Vector3f -> Vector3d` | Converts a Vector3f into a Rhino.Geometry.Vector3d. |
| `Coerce3dVector` | `x: float * y: float * z: float -> Vector3d` | Converts three floats into a Rhino.Geometry.Vector3d. |
| `Coerce3dVector` | `xyz: float seq -> Vector3d` | Converts a sequence of exactly three floats into a Rhino.Geometry.Vector3d. |
| `Coerce3dVector` | `vector: string -> Vector3d` | Parses a string into a Rhino.Geometry.Vector3d. |
| `Coerce3dVector` | `lineId: Guid -> Vector3d` | Gets the direction of a linear curve object in the document, from its start to its end point. |
| `CoerceAnnotation` | `objectId: Guid -> AnnotationObjectBase` | Attempt to get Rhino Annotation Base Object. |
| `CoerceArc` | `arc: Arc -> Arc` | Returns the given Arc. |
| `CoerceArc` | `curve: Curve -> Arc` | Gets an Arc from a Curve using the current Documents Absolute Tolerance. |
| `CoerceArc` | `objectId: Guid -> Arc` | Gets an Arc from a curve object using the current Documents Absolute Tolerance. |
| `CoerceBlockInstanceObject` | `objectId: Guid -> InstanceObject` | Returns the Rhino Block instance object for a given Id. |
| `CoerceBrep` | `objectId: Guid -> Brep` | Attempt to get Polysurface geometry from the document with a given objectId. |
| `CoerceCircle` | `circle: Circle -> Circle` | Returns the given Circle. |
| `CoerceCircle` | `curve: Curve -> Circle` | Gets a Circle from a Curve using the current Documents Absolute Tolerance. |
| `CoerceCircle` | `objectId: Guid -> Circle` | Gets a Circle from a curve object using the current Documents Absolute Tolerance. |
| `CoerceColor` | `color: Color -> Color` | Converts a System.Drawing.Color into its ARGB form, so that it is not a named color any more. |
| `CoerceColor` | `red: int * green: int * blue: int -> Color` | Creates a System.Drawing.Color from red, green and blue values. |
| `CoerceColor` | `red: int * green: int * blue: int * alpha: int -> Color` | Creates a System.Drawing.Color from red, green, blue and alpha values. |
| `CoerceColor` | `color: string -> Color` | Parses a System.Drawing.Color from a string, also works on natural language color strings, see Drawing.ColorTranslator.FromHtml. |
| `CoerceCurve` | `objectId: Guid * ?segmentIndex: int = -1 -> Curve` | Attempt to get Curve geometry from the document with a given objectId. |
| `CoerceDetailView` | `objectId: Guid -> DetailView` | Attempt to get Detail view rectangle Geometry. |
| `CoerceDetailViewObject` | `objectId: Guid -> DetailViewObject` | Attempt to get Detail view rectangle Object. |
| `CoerceEllipse` | `ellipse: Ellipse -> Ellipse` | Returns the given Ellipse. |
| `CoerceEllipse` | `curve: Curve -> Ellipse` | Gets an Ellipse from a Curve using the current Documents Absolute Tolerance. |
| `CoerceEllipse` | `objectId: Guid -> Ellipse` | Gets an Ellipse from a curve object using the current Documents Absolute Tolerance. |
| `CoerceGeometry` | `objectId: Guid -> GeometryBase` | Attempt to get GeometryBase from a given input. |
| `CoerceGuid` | `objectId: Guid -> Guid` | Checks that a Guid is not empty. |
| `CoerceGuid` | `input: string -> Guid` | Parses a Guid from a string. |
| `CoerceGuid` | `rhinoObject: RhinoObject -> Guid` | Gets the Guid of a RhinoObject. |
| `CoerceGuid` | `objRef: ObjRef -> Guid` | Gets the Guid of the object an ObjRef refers to. |
| `CoerceHatch` | `objectId: Guid -> Hatch` | Attempt to get Hatch Geometry. |
| `CoerceHatchObject` | `objectId: Guid -> HatchObject` | Attempt to get Rhino Hatch Object. |
| `CoerceLayer` | `name: string -> Layer` | Attempt to get Rhino LayerObject from the document for a given full name. |
| `CoerceLayer` | `layerId: Guid -> Layer` | Attempt to get Rhino LayerObject from the document with a given objectId. |
| `CoerceLeader` | `objectId: Guid -> LeaderObject` | Attempt to get Rhino Leader Annotation Object. |
| `CoerceLight` | `objectId: Guid -> Light` | Attempt to get Rhino LightObject from the document with a given objectId. |
| `CoerceLine` | `line: Line -> Line` | Returns the given Line. |
| `CoerceLine` | `curve: Curve -> Line` | Gets a Line from a Curve that is linear within the current Documents Absolute Tolerance. |
| `CoerceLine` | `objectId: Guid -> Line` | Gets a Line from a curve object that is linear within the current Documents Absolute Tolerance. |
| `CoerceLine` | `startPoint: Point3d * endPoint: Point3d -> Line` | Gets a Line from its start and end point. |
| `CoerceLinearDimension` | `objectId: Guid -> LinearDimensionObject` | Attempt to get Rhino LinearDimension Annotation Object. |
| `CoerceMesh` | `objectId: Guid -> Mesh` | Attempt to get Mesh geometry from the document with a given objectId. |
| `CoerceNurbsSurface` | `objectId: Guid -> NurbsSurface` | Attempt to get Surface geometry from the document with a given objectId. |
| `CoercePageView` | `viewId: Guid -> RhinoPageView` | Gets a Rhino Page (or Layout) View from its Guid. |
| `CoercePageView` | `pageName: string -> RhinoPageView` | Gets a Rhino Page (or Layout) View from the name of the Layout. |
| `CoercePlane` | `plane: Plane -> Plane` | Returns the given Plane. |
| `CoercePlane` | `origin: Point3d -> Plane` | Creates a Plane parallel to the World XY plane at the given origin. |
| `CoercePlane` | `x: float * y: float * z: float -> Plane` | Creates a Plane parallel to the World XY plane at the given origin. |
| `CoercePointCloud` | `objectId: Guid -> PointCloud` | Attempt to get Rhino PointCloud Geometry. |
| `CoercePointObject` | `objectId: Guid -> PointObject` | Attempt to get Rhino Point Object. |
| `CoercePolyline` | `polyline: Polyline -> Polyline` | Returns the given Polyline. |
| `CoercePolyline` | `curve: Curve -> Polyline` | Gets a Polyline from a Curve. |
| `CoercePolyline` | `objectId: Guid -> Polyline` | Gets a Polyline from a curve object. |
| `CoerceRhinoObject` | `objectId: Guid -> RhinoObject` | Attempt to get a RhinoObject from the document with a given objectId. |
| `CoerceSurface` | `objectId: Guid -> Surface` | Attempt to get Surface geometry from the document with a given objectId. |
| `CoerceTextDot` | `objectId: Guid -> TextDot` | Attempt to get TextDot Geometry. |
| `CoerceTextEntity` | `objectId: Guid -> TextEntity` | Attempt to get TextEntity Geometry (for the text Object use rs.CoerceTextObject) . |
| `CoerceTextObject` | `objectId: Guid -> TextObject` | Attempt to get Rhino TextObject Annotation Object. |
| `CoerceView` | `viewId: Guid -> RhinoView` | Gets a Rhino View from its Guid, it can be a standard or page view. |
| `CoerceView` | `viewName: string -> RhinoView` | Gets a Rhino View from its name, it can be a standard or page view. |
| `CoerceXform` | `xForm: Transform -> Transform` | Returns the given Transform. |
| `CoerceXform` | `xForm: 'a seq -> Transform` | Converts a 4x4 matrix of floats into a Rhino.Geometry.Transform Transformation Matrix. |
| `CoerceXform` | `xForm: float array2d -> Transform` | Converts a 4x4 matrix of floats into a Rhino.Geometry.Transform Transformation Matrix. |
| `TryCoerceArc` | `arc: Arc -> Arc option` | Returns the given Arc. |
| `TryCoerceArc` | `curve: Curve -> Arc option` | Attempt to get an Arc from a Curve using the current Documents Absolute Tolerance. |
| `TryCoerceArc` | `objectId: Guid -> Arc option` | Attempt to get an Arc from a curve object using the current Documents Absolute Tolerance. |
| `TryCoerceBrep` | `objectId: Guid -> Brep option` | Attempt to get a Polysurface or Brep class from given Guid. |
| `TryCoerceCircle` | `circle: Circle -> Circle option` | Returns the given Circle. |
| `TryCoerceCircle` | `curve: Curve -> Circle option` | Attempt to get a Circle from a Curve using the current Documents Absolute Tolerance. |
| `TryCoerceCircle` | `objectId: Guid -> Circle option` | Attempt to get a Circle from a curve object using the current Documents Absolute Tolerance. |
| `TryCoerceCurve` | `objectId: Guid * ?segmentIndex: int = -1 -> Curve option` | Attempt to get Curve geometry from the document with a given objectId. |
| `TryCoerceEllipse` | `ellipse: Ellipse -> Ellipse option` | Returns the given Ellipse. |
| `TryCoerceEllipse` | `curve: Curve -> Ellipse option` | Attempt to get an Ellipse from a Curve using the current Documents Absolute Tolerance. |
| `TryCoerceEllipse` | `objectId: Guid -> Ellipse option` | Attempt to get an Ellipse from a curve object using the current Documents Absolute Tolerance. |
| `TryCoerceGeometry` | `objectId: Guid -> GeometryBase option` | Attempt to get GeometryBase from a given Guid. |
| `TryCoerceGuid` | `objectId: Guid -> Guid option` | Attempt to get a Guid that is not empty. |
| `TryCoerceGuid` | `input: string -> Guid option` | Attempt to parse a Guid from a string. |
| `TryCoerceGuid` | `rhinoObject: RhinoObject -> Guid option` | Attempt to get the Guid of a RhinoObject. |
| `TryCoerceGuid` | `objRef: ObjRef -> Guid option` | Attempt to get the Guid of the object an ObjRef refers to. |
| `TryCoerceLight` | `objectId: Guid -> Light option` | Attempt to get Rhino LightObject from the document with a given objectId. |
| `TryCoerceLine` | `line: Line -> Line option` | Returns the given Line. |
| `TryCoerceLine` | `curve: Curve -> Line option` | Attempt to get a Line from a Curve that is linear within the current Documents Absolute Tolerance. |
| `TryCoerceLine` | `objectId: Guid -> Line option` | Attempt to get a Line from a curve object that is linear within the current Documents Absolute Tolerance. |
| `TryCoerceLine` | `startPoint: Point3d * endPoint: Point3d -> Line option` | Gets a Line from its start and end point. |
| `TryCoerceMesh` | `objectId: Guid -> Mesh option` | Attempt to get Mesh class from given Guid. |
| `TryCoercePolyline` | `polyline: Polyline -> Polyline option` | Returns the given Polyline. |
| `TryCoercePolyline` | `curve: Curve -> Polyline option` | Attempt to get a Polyline from a Curve. |
| `TryCoercePolyline` | `objectId: Guid -> Polyline option` | Attempt to get a Polyline from a curve object. |
| `TryCoerceRhinoObject` | `objectId: Guid -> RhinoObject option` | Attempt to get a RhinoObject from the document with a given objectId. |
| `TryCoerceSurface` | `objectId: Guid -> Surface option` | Attempt to get Surface class from given Guid. |
| `TryCoerceView` | `viewId: Guid -> Option<RhinoView>` | Attempt to get a Rhino View from its Guid, it can be a standard or page view. |
| `TryCoerceView` | `viewName: string -> Option<RhinoView>` | Attempt to get a Rhino View from its name, it can be a standard or page view. |

## Curve

From [Src/Scripting_Curve.fs](../Src/Scripting_Curve.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddArc` | `plane: Plane * radius: float * angleDegrees: float -> Guid` | Adds an arc curve to the document. |
| `AddArc3Pt` | `start: Point3d * ende: Point3d * pointOnArc: Point3d -> Guid` | Adds a 3-point arc curve to the document. |
| `AddArcPtTanPt` | `start: Point3d * direction: Vector3d * ende: Point3d -> Guid` | Adds an arc curve, created from a start point, a start direction, and an end point, to the document. |
| `AddBlendCurve` | `curves: (Guid * Guid) * parameters: (float * float) * reverses: (bool * bool) * continuities: (int * int) -> Guid` | Makes a curve blend between two curves. |
| `MakeBlendCurve` | `curves: (Curve * Curve) * parameters: (float * float) * reverses: (bool * bool) * continuities: (int * int) -> Curve` | Computes MakeBlendCurve from geometry without modifying the document. |
| `AddCircle` | `plane: Plane * radius: float -> Guid` | Adds a circle curve to the document. |
| `AddCircle` | `center: Point3d * radius: float -> Guid` | Adds a circle curve to the document. |
| `AddCircle3Pt` | `first: Point3d * second: Point3d * third: Point3d -> Guid` | Adds a 3-point circle curve to the document. |
| `AddCurve` | `points: Point3d seq * ?degree: int = 3 -> Guid` | Adds a control points curve object to the document. |
| `AddEllipse` | `plane: Plane * radiusX: float * radiusY: float -> Guid` | Adds an elliptical curve to the document. |
| `AddEllipse3Pt` | `center: Point3d * second: Point3d * third: Point3d -> Guid` | Adds a 3-point elliptical curve to the document. |
| `AddFilletCurve` | `curveA: Guid * curveB: Guid * ?radius: float = 1.0 * ?basePointA: Point3d = Point3d() * ?basePointB: Point3d = Point3d() -> Guid` | Adds a fillet curve between two curve objects. |
| `MakeFilletCurve` | `curveA: Curve * curveB: Curve * ?radius: float = 1.0 * ?basePointA: Point3d = Point3d() * ?basePointB: Point3d = Point3d() -> ArcCurve` | Computes MakeFilletCurve from geometry without modifying the document. |
| `AddInterpCrvOnSrf` | `surfaceId: Guid * points: Point3d seq -> Guid` | Adds an interpolated curve object that lies on a specified surface. |
| `MakeInterpCrvOnSrf` | `surfaceGeo: Surface * points: Point3d seq -> Curve` | Computes MakeInterpCrvOnSrf from geometry without modifying the document. |
| `AddInterpCrvOnSrfUV` | `surfaceId: Guid * points: Point2d seq -> Guid` | Adds an interpolated curve object based on surface parameters, that lies on a specified surface. |
| `MakeInterpCrvOnSrfUV` | `surfaceGeo: Surface * points: Point2d seq -> Curve` | Computes MakeInterpCrvOnSrfUV from geometry without modifying the document. |
| `AddInterpCurve` | `points: Point3d seq * ?degree: int = 3 * ?knotStyle: int = 0 * ?startTangent: Vector3d = Vector3d() * ?endTangent: Vector3d = Vector3d() -> Guid` | Adds an interpolated Curve object to the document. |
| `AddLine` | `start: Point3d * ende: Point3d -> Guid` | Adds a line curve to the current model. |
| `AddLine` | `startX: float * startY: float * startZ: float * endX: float * endY: float * endZ: float -> Guid` | Adds a line curve to the current model. |
| `AddLine2D` | `startX: float * startY: float * endX: float * endY: float -> Guid` | Adds a 2D line curve to the current model at z level 0.0 |
| `AddNurbsCurve` | `points: Point3d seq * knots: float seq * degree: int * ?weights: float seq = null -> Guid` | Adds a NURBS Curve object to the document. |
| `AddPolyline` | `points: Point3d seq * ?deleteShortSegments: bool = true * ?drawDotsAndRaiseIfFailed: bool = false -> Guid` | Adds a Polyline Curve. |
| `AddPolylineClosed` | `points: Point3d seq * ?deleteShortSegments: bool = true * ?drawDotsAndRaiseIfFailed: bool = false -> Guid` | Adds a closed Polyline Curve. |
| `AddRectangle` | `plane: Plane * width: float * height: float -> Guid` | Add a rectangular Curve to the document. |
| `AddSpiral` | `point0: Point3d * point1: Point3d * pitch: float * turns: float * radius0: float * ?radius1: float = 0.0 -> Guid` | Adds a spiral or helical curve to the document. |
| `AddSubCrv` | `curveId: Guid * param0: float * param1: float -> Guid` | Add a Curve object based on a portion, or interval of an existing Curve object. |
| `MakeSubCrv` | `curveGeo: Curve * param0: float * param1: float -> Curve` | Computes MakeSubCrv from geometry without modifying the document. |
| `AddTweenCurves` | `fromCurveId: Guid * toCurveId: Guid * ?numberOfCurves: int = 1 * ?method: int = 0 * ?sampleNumber: int = 10 -> ResizeArray<Guid>` | Creates Curves between two open or closed input Curves. |
| `MakeTweenCurves` | `fromCurve: Curve * toCurve: Curve * ?numberOfCurves: int = 1 * ?method: int = 0 * ?sampleNumber: int = 10 -> ResizeArray<Curve>` | Computes MakeTweenCurves from geometry without modifying the document. |
| `ArcAngle` | `curveId: Guid * ?segmentIndex: int = -1 -> float` | Returns the angle of an arc Curve object. |
| `ArcAngle` | `curveGeo: Curve * ?segmentIndex: int = -1 -> float` | Returns the angle of an arc Curve object. |
| `ArcCenterPoint` | `curveId: Guid * ?segmentIndex: int = -1 -> Point3d` | Returns the center point of an arc Curve object. |
| `ArcCenterPoint` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Point3d` | Returns the center point of an arc Curve object. |
| `ArcMidPoint` | `curveId: Guid * ?segmentIndex: int = -1 -> Point3d` | Returns the mid point of an arc Curve object. |
| `ArcMidPoint` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Point3d` | Returns the mid point of an arc Curve object. |
| `ArcRadius` | `curveId: Guid * ?segmentIndex: int = -1 -> float` | Returns the radius of an arc Curve object. |
| `ArcRadius` | `curveGeo: Curve * ?segmentIndex: int = -1 -> float` | Returns the radius of an arc Curve object. |
| `ChangeCurveDegree` | `curveId: Guid * degree: int -> bool` | Changes the degree of a Curve object. |
| `CircleCenterPlane` | `curveId: Guid * ?segmentIndex: int = -1 -> Plane` | Returns the center Plane of a circle Curve object. |
| `CircleCenterPlane` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Plane` | Returns the center Plane of a circle Curve object. |
| `CircleCenterPoint` | `curveId: Guid * ?segmentIndex: int = -1 -> Point3d` | Returns the center point of a circle Curve object. |
| `CircleCenterPoint` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Point3d` | Returns the center point of a circle Curve object. |
| `CircleCircumference` | `curveId: Guid * ?segmentIndex: int = -1 -> float` | Returns the circumference of a circle Curve object. |
| `CircleCircumference` | `curveGeo: Curve * ?segmentIndex: int = -1 -> float` | Returns the circumference of a circle Curve object. |
| `CircleRadius` | `curveId: Guid * ?segmentIndex: int = -1 -> float` | Returns the radius of a circle Curve object. |
| `CircleRadius` | `curveGeo: Curve * ?segmentIndex: int = -1 -> float` | Returns the radius of a circle Curve object. |
| `CloseCurve` | `curveId: Guid * ?tolerance: float = 0.0 -> Guid` | Closes an open Curve object by making adjustments to the end points so they meet at a point. |
| `CloseCurve` | `curveGeo: Curve * ?tolerance: float = 0.0 -> Curve` | Computes CloseCurve from geometry without modifying the document. |
| `ClosedCurveOrientation` | `curveId: Guid * ?direction: Vector3d = Vector3d() -> int` | Determine the orientation (counter-clockwise or clockwise) of a closed, planar Curve. |
| `ClosedCurveOrientation` | `curveGeo: Curve * ?direction: Vector3d = Vector3d() -> int` | Determine the orientation (counter-clockwise or clockwise) of a closed, planar Curve. |
| `ConvertCurveToPolyline` | `curveId: Guid * ?angleTolerance: float = 0.0 * ?tolerance: float = 0.0 * ?deleteInput: bool = false * ?minEdgeLength: float = 0.0 * ?maxEdgeLength: float = 0.0 -> Guid` | Convert Curve to a Polyline Curve. |
| `ConvertCurveToPolyline` | `curveGeo: Curve * ?angleTolerance: float = 0.0 * ?tolerance: float = 0.0 * ?minEdgeLength: float = 0.0 * ?maxEdgeLength: float = 0.0 -> PolylineCurve` | Computes ConvertCurveToPolyline from geometry without modifying the document. |
| `CreateNurbsCurve` | `points: Point3d seq * knots: float seq * degree: int * ?weights: float seq = null -> NurbsCurve` | Creates a NURBS Curve geometry, but does not add or draw it to the document. |
| `CurveArcLengthPoint` | `curveId: Guid * length: float * ?fromStart: bool = true -> Point3d` | Returns the point on the Curve that is a specified arc length from the start of the Curve. |
| `CurveArcLengthPoint` | `curveGeo: Curve * length: float * ?fromStart: bool = true -> Point3d` | Returns the point on the Curve that is a specified arc length from the start of the Curve. |
| `CurveArea` | `curveId: Guid -> float` | Returns area of closed planar Curves. |
| `CurveArea` | `curveGeo: Curve -> float` | Returns area of closed planar Curves. |
| `CurveAreaCentroid` | `curveId: Guid -> Point3d` | Returns area centroid of closed, planar Curves. |
| `CurveAreaCentroid` | `curveGeo: Curve -> Point3d` | Returns area centroid of closed, planar Curves. |
| `CurveArrows` | `curveId: Guid -> int` | Get status of a Curve object's annotation arrows. |
| `CurveArrows` | `curveId: Guid * arrowStyle: int -> unit` | Enables or disables a Curve object's annotation arrows. |
| `CurveArrows` | `curveIds: Guid seq * arrowStyle: int -> unit` | Enables or disables multiple Curve objects's annotation arrows. |
| `CurveBooleanDifference` | `curveA: Guid * curveB: Guid * ?tolerance: float = 0.0 -> ResizeArray<Guid>` | Calculates the difference between two closed, planar Curves and adds the results to the document. |
| `CurveBooleanDifference` | `curveA: Curve * curveB: Curve * ?tolerance: float = 0.0 -> ResizeArray<Curve>` | Computes CurveBooleanDifference from geometry without modifying the document. |
| `CurveBooleanIntersection` | `curveA: Guid * curveB: Guid * ?tolerance: float = 0.0 -> ResizeArray<Guid>` | Calculates the intersection of two closed, planar Curves and adds the results to the document. |
| `CurveBooleanIntersection` | `curveA: Curve * curveB: Curve * ?tolerance: float = 0.0 -> ResizeArray<Curve>` | Computes CurveBooleanIntersection from geometry without modifying the document. |
| `CurveBooleanUnion` | `curveIds: Guid seq * ?tolerance: float = 0.0 -> ResizeArray<Guid>` | Calculate the union of two or more closed, planar Curves and add the results to the document. |
| `CurveBooleanUnion` | `curveInputs: Curve seq * ?tolerance: float = 0.0 -> ResizeArray<Curve>` | Computes CurveBooleanUnion from geometry without modifying the document. |
| `CurveBrepIntersect` | `curveId: Guid * brepId: Guid * ?tolerance: float = 0.0 -> ResizeArray<Point3d> * ResizeArray<Curve>` | Intersects a Curve object with a brep object. |
| `CurveBrepIntersect` | `curveGeo: Curve * brepInput: Brep * ?tolerance: float = 0.0 -> ResizeArray<Point3d> * ResizeArray<Curve>` | Intersects a Curve object with a brep object. |
| `CurveClosestObject` | `curveId: Guid * objectIds: Guid seq -> Guid * Point3d * Point3d` | Returns the 3D point locations on two objects where they are closest to each other. |
| `CurveClosestObject` | `curveGeo: Curve * objectInputs: GeometryBase seq -> GeometryBase * Point3d * Point3d` | Computes CurveClosestObject from geometry without modifying the document. |
| `CurveClosestParameter` | `curveId: Guid * point: Point3d * ?segmentIndex: int = -1 -> float` | Returns parameter of the point on a Curve that is closest to a test point. |
| `CurveClosestParameter` | `curveGeo: Curve * point: Point3d * segmentIndex: int -> float` | Returns parameter of the point on a Curve that is closest to a test point. |
| `CurveClosestParameter` | `curve: Curve * point: Point3d -> float` | Returns parameter of the point on a Curve that is closest to a test point. |
| `CurveClosestPoint` | `curveId: Guid * point: Point3d * ?segmentIndex: int = -1 -> Point3d` | Returns the point on a Curve that is closest to a test point. |
| `CurveClosestPoint` | `curveGeo: Curve * point: Point3d * segmentIndex: int -> Point3d` | Returns the point on a Curve that is closest to a test point. |
| `CurveClosestPoint` | `curve: Curve * point: Point3d -> Point3d` | Returns the point on a Curve that is closest to a test point. |
| `CurveContourPoints` | `curveId: Guid * startPoint: Point3d * endPoint: Point3d * interval: float -> Point3d array` | Returns the 3D point locations calculated by contouring a Curve object. |
| `CurveContourPoints` | `curveGeo: Curve * startPoint: Point3d * endPoint: Point3d * interval: float -> Point3d array` | Returns the 3D point locations calculated by contouring a Curve object. |
| `CurveCurvature` | `curveId: Guid * parameter: float -> Point3d * Vector3d * Point3d * float * Vector3d` | Returns the curvature of a Curve at a parameter. |
| `CurveCurvature` | `curveGeo: Curve * parameter: float -> Point3d * Vector3d * Point3d * float * Vector3d` | Returns the curvature of a Curve at a parameter. |
| `CurveCurveIntersection` | `curveA: Guid * ?curveB: Guid = Guid() * ?tolerance: float = 0.0 -> ResizeArray<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float>` | Calculates intersection of two Curve objects. |
| `CurveCurveIntersection` | `curveA: Curve * ?curveB: Curve = null * ?tolerance: float = 0.0 -> ResizeArray<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float>` | Calculates intersection of two Curve objects. |
| `CurveDegree` | `curveId: Guid * ?segmentIndex: int = -1 -> int` | Returns the degree of a Curve object. |
| `CurveDegree` | `curveGeo: Curve * ?segmentIndex: int = -1 -> int` | Returns the degree of a Curve object. |
| `CurveDeviation` | `curveA: Guid * curveB: Guid -> float * float * float * float * float * float` | Returns the minimum and maximum deviation between two Curve objects. |
| `CurveDeviation` | `curveA: Curve * curveB: Curve -> float * float * float * float * float * float` | Returns the minimum and maximum deviation between two Curve objects. |
| `CurveDim` | `curveId: Guid * ?segmentIndex: int = -1 -> int` | Returns the dimension of a Curve object. |
| `CurveDim` | `curveGeo: Curve * ?segmentIndex: int = -1 -> int` | Returns the dimension of a Curve object. |
| `CurveDirection` | `curveId: Guid * ?allowNonLinear: bool = false * ?segmentIndex: int = -1 -> Vector3d` | Returns the average unitized direction of a Curve object between start and end point, Optionally allows non linear Curves too. |
| `CurveDirection` | `curveGeo: Curve * ?allowNonLinear: bool = false * ?segmentIndex: int = -1 -> Vector3d` | Returns the average unitized direction of a Curve object between start and end point, Optionally allows non linear Curves too. |
| `CurveDirectionsMatch` | `curveA: Guid * curveB: Guid -> bool` | Tests if two Curve objects are generally in the same direction or if they would be more in the same direction if one of them were flipped. |
| `CurveDirectionsMatch` | `curveA: Curve * curveB: Curve -> bool` | Tests if two Curve objects are generally in the same direction or if they would be more in the same direction if one of them were flipped. |
| `CurveDiscontinuity` | `curveId: Guid * style: int -> ResizeArray<Point3d>` | Search for a derivative, tangent, or curvature discontinuity in a Curve object. |
| `CurveDiscontinuity` | `curveGeo: Curve * style: int -> ResizeArray<Point3d>` | Search for a derivative, tangent, or curvature discontinuity in a Curve object. |
| `CurveDomain` | `curveId: Guid * ?segmentIndex: int = -1 -> Interval` | Returns the domain of a Curve object as an indexable object with two elements. |
| `CurveDomain` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Interval` | Returns the domain of a Curve object as an indexable object with two elements. |
| `CurveEditParameters` | `curveId: Guid * ?segmentIndex: int = -1 -> float array` | Returns the Edit-, or Greville-Parameters of a Curve object. |
| `CurveEditParameters` | `curveGeo: Curve * ?segmentIndex: int = -1 -> float array` | Returns the Edit-, or Greville-Parameters of a Curve object. |
| `CurveEditPoints` | `curveId: Guid * ?segmentIndex: int = -1 -> Point3dList` | Returns the Edit-, or Greville-Points of a Curve object. |
| `CurveEditPoints` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Point3dList` | Returns the Edit-, or Greville-Points of a Curve object. |
| `CurveEndPoint` | `curveId: Guid * ?segmentIndex: int = -1 -> Point3d` | Returns the end point of a Curve object. |
| `CurveEndPoint` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Point3d` | Returns the end point of a Curve object. |
| `CurveEndTangent` | `curveId: Guid * ?segmentIndex: int = -1 -> Vector3d` | Returns the tangent at end point of a Curve object pointing away from the Curve . |
| `CurveEndTangent` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Vector3d` | Returns the tangent at end point of a Curve object pointing away from the Curve . |
| `CurveFilletPoints` | `curveA: Guid * curveB: Guid * ?radius: float = 1.0 * ?basePointA: Point3d = Point3d() * ?basePointB: Point3d = Point3d() -> Point3d * Point3d * Plane` | Find points at which to cut a pair of Curves so that a fillet of a specified radius fits. |
| `CurveFilletPoints` | `curveA: Curve * curveB: Curve * ?radius: float = 1.0 * ?basePointA: Point3d = Point3d() * ?basePointB: Point3d = Point3d() -> Point3d * Point3d * Plane` | Find points at which to cut a pair of Curves so that a fillet of a specified radius fits. |
| `CurveFrame` | `curveId: Guid * parameter: float * ?segmentIndex: int = -1 -> Plane` | Returns the Plane at a parameter of a Curve. |
| `CurveFrame` | `curveGeo: Curve * parameter: float * ?segmentIndex: int = -1 -> Plane` | Returns the Plane at a parameter of a Curve. |
| `CurveKnotCount` | `curveId: Guid * ?segmentIndex: int = -1 -> int` | Returns the knot count of a Curve object. |
| `CurveKnotCount` | `curveGeo: Curve * ?segmentIndex: int = -1 -> int` | Returns the knot count of a Curve object. |
| `CurveKnots` | `curveId: Guid * ?segmentIndex: int = -1 -> ResizeArray<float>` | Returns the knots, or knot vector, of a Curve object. |
| `CurveKnots` | `curveGeo: Curve * ?segmentIndex: int = -1 -> ResizeArray<float>` | Returns the knots, or knot vector, of a Curve object. |
| `CurveLength` | `curveId: Guid * ?segmentIndex: int = -1 * ?subDomain: Interval = Interval() -> float` | Returns the length of a Curve object. |
| `CurveLength` | `curveGeo: Curve * ?segmentIndex: int = -1 * ?subDomain: Interval = Interval() -> float` | Returns the length of a Curve object. |
| `CurveLineClosestPoint` | `curveId: Guid * line: Line -> Point3d * Point3d` | Returns the 3D point locations on the Curve and finite line where they are closest to each other. |
| `CurveLineClosestPoint` | `curveGeo: Curve * line: Line -> Point3d * Point3d` | Returns the 3D point locations on the Curve and finite line where they are closest to each other. |
| `CurveMidPoint` | `curveId: Guid * ?segmentIndex: int = -1 -> Point3d` | Returns the mid point of a Curve object. |
| `CurveMidPoint` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Point3d` | Returns the mid point of a Curve object. |
| `CurveNormal` | `curveId: Guid * ?segmentIndex: int = -1 -> Vector3d` | Returns the normal direction of the Plane in which a planar Curve object lies. |
| `CurveNormal` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Vector3d` | Returns the normal direction of the Plane in which a planar Curve object lies. |
| `CurveNormalizedParameter` | `curveId: Guid * parameter: float -> float` | Converts a Curve parameter to a normalized Curve parameter; one that ranges between 0-1. |
| `CurveNormalizedParameter` | `curveGeo: Curve * parameter: float -> float` | Converts a Curve parameter to a normalized Curve parameter; one that ranges between 0-1. |
| `CurveParameter` | `curveId: Guid * parameter: float -> float` | Converts a normalized Curve parameter to a Curve parameter; one within the Curve's domain. |
| `CurveParameter` | `curveGeo: Curve * parameter: float -> float` | Converts a normalized Curve parameter to a Curve parameter; one within the Curve's domain. |
| `CurvePerpFrame` | `curveId: Guid * parameter: float -> Plane` | Returns the perpendicular Plane at a parameter of a Curve. |
| `CurvePerpFrame` | `curveGeo: Curve * parameter: float -> Plane` | Returns the perpendicular Plane at a parameter of a Curve. |
| `CurvePlane` | `curveId: Guid * ?segmentIndex: int = -1 -> Plane` | Returns the Plane in which a planar Curve lies. |
| `CurvePlane` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Plane` | Returns the Plane in which a planar Curve lies. |
| `CurvePointCount` | `curveId: Guid * ?segmentIndex: int = -1 -> int` | Returns the control points count of a Curve object. |
| `CurvePointCount` | `curveGeo: Curve * ?segmentIndex: int = -1 -> int` | Returns the control points count of a Curve object. |
| `CurvePoints` | `curve: Curve -> ResizeArray<Point3d>` | Returns the control points, or control vertices, of a Curve object. |
| `CurvePoints` | `curveId: Guid * ?segmentIndex: int = -1 -> ResizeArray<Point3d>` | Returns the control points, or control vertices, of a Curve object. |
| `CurvePoints` | `curveGeo: Curve * segmentIndex: int -> ResizeArray<Point3d>` | Returns the control points, or control vertices, of a Curve object. |
| `CurveRadius` | `curveId: Guid * testPoint: Point3d * ?segmentIndex: int = -1 -> float` | Returns the radius of curvature at a point on a Curve. |
| `CurveRadius` | `curveGeo: Curve * testPoint: Point3d * ?segmentIndex: int = -1 -> float` | Returns the radius of curvature at a point on a Curve. |
| `CurveSeam` | `curveId: Guid * parameter: float -> bool` | Adjusts the seam, or start/end, point of a closed Curve. |
| `CurveStartPoint` | `curveId: Guid * ?segmentIndex: int = -1 -> Point3d` | Returns the start point of a Curve object. |
| `CurveStartPoint` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Point3d` | Returns the start point of a Curve object. |
| `CurveStartPoint` | `curveId: Guid * point: Point3d -> unit` | Sets the start point of a Curve object. |
| `CurveStartTangent` | `curveId: Guid * ?segmentIndex: int = -1 -> Vector3d` | Returns the tangent at start point of a Curve object pointing in direction of the Curve . |
| `CurveStartTangent` | `curveGeo: Curve * ?segmentIndex: int = -1 -> Vector3d` | Returns the tangent at start point of a Curve object pointing in direction of the Curve . |
| `CurveSurfaceIntersection` | `curveId: Guid * surfaceId: Guid * ?tolerance: float = 0.0 * ?angleTolerance: float = 0.0 -> ResizeArray<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float * float * float>` | Calculates intersection of a Curve object with a Surface object. |
| `CurveSurfaceIntersection` | `curveGeo: Curve * surfaceGeo: Surface * ?tolerance: float = 0.0 * ?angleTolerance: float = 0.0 -> ResizeArray<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float * float * float>` | Calculates intersection of a Curve object with a Surface object. |
| `CurveTangent` | `curveId: Guid * parameter: float * ?segmentIndex: int = -1 -> Vector3d` | Returns a 3D vector that is the tangent to a Curve at a parameter. |
| `CurveTangent` | `curveGeo: Curve * parameter: float * ?segmentIndex: int = -1 -> Vector3d` | Returns a 3D vector that is the tangent to a Curve at a parameter. |
| `CurveWeights` | `curveId: Guid * ?segmentIndex: int = -1 -> ResizeArray<float>` | Returns list of weights that are assigned to the control points of a Curve. |
| `CurveWeights` | `curveGeo: Curve * ?segmentIndex: int = -1 -> ResizeArray<float>` | Returns list of weights that are assigned to the control points of a Curve. |
| `DivideCurve` | `curve: Curve * segments: int -> float array` | Divides a Curve Geometry into a specified number of segments. |
| `DivideCurve` | `curveId: Guid * segments: int -> float array` | Divides a Curve object into a specified number of segments. |
| `DivideCurveEquidistant` | `curve: Curve * distance: float -> Point3d array` | Divides a Curve Geometry such that the linear distance between the points is equal. |
| `DivideCurveEquidistant` | `curveId: Guid * distance: float -> Point3d array` | Divides a Curve object such that the linear distance between the points is equal. |
| `DivideCurveIntoPoints` | `curve: Curve * segments: int -> Point3d array` | Divides a Curve Geometry into a specified number of segments, including start and end point. |
| `DivideCurveIntoPoints` | `curveId: Guid * segments: int -> Point3d array` | Divides a Curve object into a specified number of segments, including start and end point. |
| `DivideCurveLength` | `curve: Curve * length: float -> float array` | Divides a Curve Geometry into segments of a specified length. |
| `DivideCurveLength` | `curveId: Guid * length: float -> float array` | Divides a Curve object into segments of a specified length. |
| `DivideCurveLengthIntoPoints` | `curve: Curve * length: float -> ResizeArray<Point3d>` | Divides a Curve Geometry into segments of a specified length. |
| `DivideCurveLengthIntoPoints` | `curveId: Guid * length: float -> ResizeArray<Point3d>` | Divides a Curve object into segments of a specified length. |
| `EllipseCenterPoint` | `curveId: Guid -> Point3d` | Returns the center point of an elliptical-shaped Curve object. |
| `EllipseCenterPoint` | `curveGeo: Curve -> Point3d` | Returns the center point of an elliptical-shaped Curve object. |
| `EllipseQuadPoints` | `curveId: Guid -> Point3d * Point3d * Point3d * Point3d` | Returns the quadrant points of an elliptical-shaped Curve object. |
| `EllipseQuadPoints` | `curveGeo: Curve -> Point3d * Point3d * Point3d * Point3d` | Returns the quadrant points of an elliptical-shaped Curve object. |
| `EvaluateCurve` | `curveId: Guid * t: float * ?segmentIndex: int = -1 -> Point3d` | Evaluates a Curve at a parameter and returns a 3D point. |
| `EvaluateCurve` | `curveGeo: Curve * t: float * ?segmentIndex: int = -1 -> Point3d` | Evaluates a Curve at a parameter and returns a 3D point. |
| `ExplodeCurve` | `curveId: Guid * ?deleteInput: bool = false -> ResizeArray<Guid>` | Explodes, or un-joins, one Curve. |
| `ExplodeCurve` | `curveGeo: Curve -> ResizeArray<Curve>` | Computes ExplodeCurve from geometry without modifying the document. |
| `ExplodeCurves` | `curveIds: Guid seq * ?deleteInput: bool = false -> ResizeArray<Guid>` | Explodes, or un-joins, several curves Curve. |
| `ExplodeCurves` | `curveInputs: Curve seq -> ResizeArray<Curve>` | Computes ExplodeCurves from geometry without modifying the document. |
| `ExtendCurve` | `curveId: Guid * extensionType: int * side: int * boundaryCurveIds: Guid seq * ?replaceInput: bool = false -> Guid` | Extends a non-closed Curve object by a line, arc, or smooth extension until it intersects a collection of objects. |
| `ExtendCurve` | `curveGeo: Curve * extensionType: int * side: int * boundaryCurves: GeometryBase seq -> Curve` | Computes ExtendCurve from geometry without modifying the document. |
| `ExtendCurveLength` | `curveId: Guid * extensionType: int * side: int * length: float -> Guid` | Extends a non-closed Curve by a line, arc, or smooth extension for a specified distance. |
| `ExtendCurveLength` | `curveGeo: Curve * extensionType: int * side: int * length: float -> Curve` | Computes ExtendCurveLength from geometry without modifying the document. |
| `ExtendCurvePoint` | `curveId: Guid * side: int * point: Point3d * ?extensionType: int = -1 -> Guid` | Extends a non-closed Curve by smooth extension to a point. |
| `ExtendCurvePoint` | `curveGeo: Curve * side: int * point: Point3d * ?extensionType: int = -1 -> Curve` | Computes ExtendCurvePoint from geometry without modifying the document. |
| `FairCurve` | `curveId: Guid * tolerance: float -> bool` | Fairs a Curve. |
| `FitCurve` | `curveId: Guid * ?degree: int = 3 * ?distanceTolerance: float = 0.0 * ?angleTolerance: float = -1.0 -> Guid` | Reduces number of Curve control points while maintaining the Curve's same general shape. |
| `FitCurve` | `curveGeo: Curve * ?degree: int = 3 * ?distanceTolerance: float = 0.0 * ?angleTolerance: float = -1.0 -> Curve` | Computes FitCurve from geometry without modifying the document. |
| `InsertCurveKnot` | `curveId: Guid * parameter: float * ?symmetrical: bool = false -> bool` | Inserts a knot into a Curve object. |
| `IsArc` | `curveId: Guid * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is an open arc Curve. |
| `IsArc` | `curveGeo: GeometryBase * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is an open arc Curve. |
| `IsCircle` | `curveId: Guid * ?tolerance: float = 0.0 -> bool` | Checks if an object is a circle Curve. |
| `IsCircle` | `curveGeo: GeometryBase * ?tolerance: float = 0.0 -> bool` | Checks if an object is a circle Curve. |
| `IsCurve` | `curveId: Guid -> bool` | Checks if an object is a Curve. |
| `IsCurve` | `curveGeo: GeometryBase -> bool` | Checks if an object is a Curve. |
| `IsCurveClosable` | `curveId: Guid * ?tolerance: float = 0.0 -> bool` | Decide if it makes sense to close off the Curve by moving the end point to the start point based on start-end gap size and length of Curve as approximated by chord defined by 6 points. |
| `IsCurveClosable` | `curveGeo: GeometryBase * ?tolerance: float = 0.0 -> bool` | Decide if it makes sense to close off the Curve by moving the end point to the start point based on start-end gap size and length of Curve as approximated by chord defined by 6 points. |
| `IsCurveClosed` | `curveId: Guid -> bool` | Checks if an object is a closed Curve object. |
| `IsCurveClosed` | `curveGeo: GeometryBase -> bool` | Checks if an object is a closed Curve object. |
| `IsCurveInPlane` | `curveId: Guid * plane: Plane * ?tolerance: float = 0.0 -> bool` | Test a Curve to see if it lies in a specific Plane. |
| `IsCurveInPlane` | `curveGeo: GeometryBase * plane: Plane * ?tolerance: float = 0.0 -> bool` | Test a Curve to see if it lies in a specific Plane. |
| `IsCurveLinear` | `curveId: Guid * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is a linear Curve. |
| `IsCurveLinear` | `curveGeo: GeometryBase * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is a linear Curve. |
| `IsCurvePeriodic` | `curveId: Guid * ?segmentIndex: int = -1 -> bool` | Checks if an object is a periodic Curve object. |
| `IsCurvePeriodic` | `curveGeo: GeometryBase * ?segmentIndex: int = -1 -> bool` | Checks if an object is a periodic Curve object. |
| `IsCurvePlanar` | `curveId: Guid * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is a planar Curve. |
| `IsCurvePlanar` | `curveGeo: GeometryBase * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is a planar Curve. |
| `IsCurveRational` | `curveId: Guid * ?segmentIndex: int = -1 -> bool` | Checks if an object is a rational NURBS Curve. |
| `IsCurveRational` | `curveGeo: GeometryBase * ?segmentIndex: int = -1 -> bool` | Checks if an object is a rational NURBS Curve. |
| `IsEllipse` | `curveId: Guid * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is an elliptical-shaped Curve. |
| `IsEllipse` | `curveGeo: GeometryBase * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is an elliptical-shaped Curve. |
| `IsLine` | `curveId: Guid * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is a line Curve. |
| `IsLine` | `curveGeo: GeometryBase * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if an object is a line Curve. |
| `IsPointOnCurve` | `curveId: Guid * point: Point3d * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if a point is on a Curve. |
| `IsPointOnCurve` | `curveGeo: Curve * point: Point3d * ?tolerance: float = 0.0 * ?segmentIndex: int = -1 -> bool` | Checks if a point is on a Curve. |
| `IsPolyCurve` | `curveId: Guid * ?segmentIndex: int = -1 -> bool` | Checks if an object is a PolyCurve Curve. |
| `IsPolyCurve` | `curveGeo: GeometryBase * ?segmentIndex: int = -1 -> bool` | Checks if an object is a PolyCurve Curve. |
| `IsPolyline` | `curveId: Guid * ?segmentIndex: int = -1 -> bool` | Checks if an object is a Polyline Curve object or a nurbs cure with degree 1 and moer than 2 points Lines return false. |
| `IsPolyline` | `curveGeo: GeometryBase * ?segmentIndex: int = -1 -> bool` | Checks if an object is a Polyline Curve object or a nurbs cure with degree 1 and moer than 2 points Lines return false. |
| `JoinCurves` | `curveIds: Guid seq * ?deleteInput: bool = false * ?tolerance: float = 0.0 -> ResizeArray<Guid>` | Joins multiple Curves together to form one or more Curves or polycurves. |
| `JoinCurves` | `curveInputs: Curve seq * ?tolerance: float = 0.0 -> ResizeArray<Curve>` | Computes JoinCurves from geometry without modifying the document. |
| `LineFitFromPoints` | `points: Point3d seq -> Line` | Returns a line that was fit through an array of 3D points. |
| `MakeCurveNonPeriodic` | `curveId: Guid * ?deleteInput: bool = false -> Guid` | Makes a periodic Curve non-periodic. |
| `MakeCurveNonPeriodic` | `curveGeo: Curve -> NurbsCurve` | Computes MakeCurveNonPeriodic from geometry without modifying the document. |
| `MeanCurve` | `curve0: Guid * curve1: Guid * ?tolerance: float = 0.0 -> Guid` | Creates an average Curve from two Curves. |
| `MeanCurve` | `curve0: Curve * curve1: Curve * ?tolerance: float = 0.0 -> Curve` | Computes MeanCurve from geometry without modifying the document. |
| `MeshPolyline` | `polylineId: Guid -> Guid` | Creates a polygon Mesh object based on a closed Polyline Curve object. |
| `MeshPolyline` | `polylineInput: Curve -> Mesh` | Computes MeshPolyline from geometry without modifying the document. |
| `OffsetCurve` | `curveId: Guid * direction: Point3d * distance: float * ?normal: Vector3d = Vector3d() * ?style: int = 1 -> ResizeArray<Guid>` | Offsets a Curve by a distance. |
| `OffsetCurve` | `curveGeo: Curve * direction: Point3d * distance: float * ?normal: Vector3d = Vector3d() * ?style: int = 1 -> ResizeArray<Curve>` | Computes OffsetCurve from geometry without modifying the document. |
| `OffsetCurveOnSurface` | `curveId: Guid * surfaceId: Guid * distance: float -> ResizeArray<Guid>` | Offset a Curve on a Surface. |
| `OffsetCurveOnSurface` | `curveGeo: Curve * surfaceGeo: Surface * distance: float -> ResizeArray<Curve>` | Computes OffsetCurveOnSurface from geometry without modifying the document. |
| `OffsetCurveOnSurfaceUV` | `curveId: Guid * surfaceId: Guid * parameter: Point2d -> ResizeArray<Guid>` | Offset a Curve on a Surface. |
| `OffsetCurveOnSurfaceUV` | `curveGeo: Curve * surfaceGeo: Surface * parameter: Point2d -> ResizeArray<Curve>` | Computes OffsetCurveOnSurfaceUV from geometry without modifying the document. |
| `PlanarClosedCurveContainment` | `curveA: Guid * curveB: Guid * ?plane: Plane = Plane() * ?tolerance: float = 0.0 -> int` | Determines the relationship between the regions bounded by two coplanar simple closed Curves. |
| `PlanarClosedCurveContainment` | `curveA: Curve * curveB: Curve * ?plane: Plane = Plane() * ?tolerance: float = 0.0 -> int` | Determines the relationship between the regions bounded by two coplanar simple closed Curves. |
| `PlanarCurveCollision` | `curveA: Guid * curveB: Guid * ?plane: Plane = Plane() * ?tolerance: float = 0.0 -> bool` | Determines if two coplanar Curves intersect. |
| `PlanarCurveCollision` | `curveA: Curve * curveB: Curve * ?plane: Plane = Plane() * ?tolerance: float = 0.0 -> bool` | Determines if two coplanar Curves intersect. |
| `PointInPlanarClosedCurve` | `point: Point3d * curve: Guid * ?plane: Plane = Plane() * ?tolerance: float = 0.0 -> int` | Determines if a point is inside of a closed Curve, on a closed Curve, or outside of a closed Curve. |
| `PointInPlanarClosedCurve` | `point: Point3d * curve: Curve * ?plane: Plane = Plane() * ?tolerance: float = 0.0 -> int` | Determines if a point is inside of a closed Curve, on a closed Curve, or outside of a closed Curve. |
| `PolyCurveCount` | `curveId: Guid * ?segmentIndex: int = -1 -> int` | Returns the number of Curve segments that make up a PolyCurve. |
| `PolyCurveCount` | `curveGeo: Curve * ?segmentIndex: int = -1 -> int` | Returns the number of Curve segments that make up a PolyCurve. |
| `PolylineVertices` | `curveId: Guid * ?segmentIndex: int = -1 -> ResizeArray<Point3d>` | Returns the vertices of a Polyline Curve. |
| `PolylineVertices` | `curveGeo: Curve * ?segmentIndex: int = -1 -> ResizeArray<Point3d>` | Returns the vertices of a Polyline Curve. |
| `ProjectCurveToMesh` | `curveIds: Guid seq * meshIds: Guid seq * direction: Vector3d -> ResizeArray<Guid>` | Projects one or more Curves onto one or more Surfaces or Meshes. |
| `ProjectCurveToMesh` | `curveInputs: Curve seq * meshInputs: Mesh seq * direction: Vector3d -> ResizeArray<Curve>` | Computes ProjectCurveToMesh from geometry without modifying the document. |
| `ProjectCurveToSurface` | `curveIds: Guid seq * surfaceIds: Guid seq * direction: Vector3d -> ResizeArray<Guid>` | Projects one or more Curves onto one or more Surfaces or Polysurfaces. |
| `ProjectCurveToSurface` | `curveInputs: Curve seq * surfaceInputs: Brep seq * direction: Vector3d -> ResizeArray<Curve>` | Computes ProjectCurveToSurface from geometry without modifying the document. |
| `RebuildCurve` | `curveId: Guid * degree: int * pointCount: int -> bool` | Rebuilds a Curve to a given degree and control point count. |
| `RemoveCurveKnot` | `curve: Guid * parameter: float -> bool` | Deletes a knot from a Curve object. |
| `ReverseCurve` | `curveId: Guid -> bool` | Reverses the direction of a Curve object. |
| `SimplifyCurve` | `curveId: Guid * ?flags: int = 0 -> bool` | Replace a Curve with a geometrically equivalent PolyCurve. |
| `SplitCurve` | `curveId: Guid * parameter: float seq * ?deleteInput: bool = true -> ResizeArray<Guid>` | Splits, or divides, a Curve at a specified parameter. |
| `SplitCurve` | `curveGeo: Curve * parameter: float seq -> ResizeArray<Curve>` | Computes SplitCurve from geometry without modifying the document. |
| `TrimCurve` | `curveId: Guid * interval: (float * float) * ?deleteInput: bool = true -> Guid` | Trims a Curve by removing portions of the Curve outside a specified interval. |
| `TrimCurve` | `curveGeo: Curve * interval: (float * float) -> Curve` | Computes TrimCurve from geometry without modifying the document. |

## Dimension

From [Src/Scripting_Dimension.fs](../Src/Scripting_Dimension.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddAlignedDimension` | `startPoint: Point3d * endPoint: Point3d * pointOnDimensionLine: Point3d * ?style: string = "" -> Guid` | Adds an aligned dimension object to the document. |
| `AddDimStyle` | `dimStyleName: string -> unit` | Adds a new dimension style to the document. |
| `AddLeader` | `points: Point3d seq * text: string * ?plane: Plane = Plane() -> Guid` | Adds a leader to the document. |
| `AddLinearDimension` | `startPoint: Point3d * endPoint: Point3d * pointOnDimensionLine: Point3d * ?plane: Plane = Plane() -> Guid` | Adds a linear dimension to the document. |
| `CurrentDimStyle` | `unit -> string` | Returns the current default dimension style. |
| `CurrentDimStyle` | `dimStyleName: string -> unit` | Changes the current default dimension style. |
| `DeleteDimStyle` | `dimStyleName: string -> unit` | Removes an existing dimension style from the document. |
| `DimensionStyle` | `objectId: Guid -> string` | Returns the dimension style of a dimension object. |
| `DimensionStyle` | `objectId: Guid * dimStyleName: string -> unit` | Modifies the dimension style of a dimension object. |
| `DimensionStyle` | `objectIds: Guid seq * dimStyleName: string -> unit` | Modifies the dimension style of multiple dimension objects. |
| `DimensionText` | `objectId: Guid -> string` | Returns the text displayed by a dimension object. |
| `DimensionUserText` | `objectId: Guid -> string` | Returns the user text string of a dimension object. |
| `DimensionUserText` | `objectId: Guid * usertext: string -> unit` | Modifies the user text string of a dimension object. |
| `DimensionUserText` | `objectIds: Guid seq * usertext: string -> unit` | Modifies the user text string of multiple dimension objects. |
| `DimensionValue` | `objectId: Guid -> float` | Returns the value of a dimension object. |
| `DimStyleAnglePrecision` | `dimStyle: string -> int` | Returns the angle display precision of a dimension style. |
| `DimStyleAnglePrecision` | `dimStyle: string * precision: int -> unit` | Changes the angle display precision of a dimension style. |
| `DimStyleArrowSize` | `dimStyle: string -> float` | Returns the arrow size of a dimension style. |
| `DimStyleArrowSize` | `dimStyle: string * size: float -> unit` | Changes the arrow size of a dimension style. |
| `DimStyleAvailableFonts` | `unit -> string array` | Gets all Available Font Face Names. |
| `DimStyleCount` | `unit -> int` | Returns the number of dimension styles in the document. |
| `DimStyleExtension` | `dimStyle: string -> float` | Returns the extension line extension of a dimension style. |
| `DimStyleExtension` | `dimStyle: string * extension: float -> unit` | Changes the extension line extension of a dimension style. |
| `DimStyleFont` | `dimStyle: string -> string` | Returns the font used by a dimension style. |
| `DimStyleFont` | `dimStyle: string * font: string -> unit` | Changes the font used by a dimension style. |
| `DimStyleLeaderArrowSize` | `dimStyle: string -> float` | Returns the leader arrow size of a dimension style. |
| `DimStyleLeaderArrowSize` | `dimStyle: string * size: float -> unit` | Changes the leader arrow size of a dimension style. |
| `DimStyleLengthFactor` | `dimStyle: string -> float` | Returns the length factor of a dimension style. |
| `DimStyleLengthFactor` | `dimStyle: string * factor: float -> unit` | Changes the length factor of a dimension style. |
| `DimStyleLinearPrecision` | `dimStyle: string -> int` | Returns the linear display precision of a dimension style. |
| `DimStyleLinearPrecision` | `dimStyle: string * precision: int -> unit` | Changes the linear display precision of a dimension style. |
| `DimStyleNames` | `unit -> ResizeArray<string>` | Returns the names of all dimension styles in the document. |
| `DimStyleNames` | `sort: bool -> ResizeArray<string>` | Returns the names, optionally sorted in ordinal order, like in RhinoPython. |
| `DimStyleNumberFormat` | `dimStyle: string -> int` | Returns the number display format of a dimension style. |
| `DimStyleNumberFormat` | `dimStyle: string * format: int -> unit` | Changes the number display format of a dimension style. |
| `DimStyleOffset` | `dimStyle: string -> float` | Returns the extension line offset of a dimension style. |
| `DimStyleOffset` | `dimStyle: string * offset: float -> unit` | Changes the extension line offset of a dimension style. |
| `DimStylePrefix` | `dimStyle: string -> string` | Returns the prefix of a dimension style - the text to prefix to the dimension text. |
| `DimStylePrefix` | `dimStyle: string * prefix: string -> unit` | Changes the prefix of a dimension style - the text to prefix to the dimension text. |
| `DimStyleScale` | `dimStyle: string -> float` | Returns the scale of a dimension style . |
| `DimStyleScale` | `dimStyle: string * scale: float -> unit` | Changes the scale of a dimension style . |
| `DimStyleSuffix` | `dimStyle: string -> string` | Returns the suffix of a dimension style - the text to append to the dimension text. |
| `DimStyleSuffix` | `dimStyle: string * suffix: string -> unit` | Changes the suffix of a dimension style - the text to append to the dimension text. |
| `DimStyleTextAlignment` | `dimStyle: string -> int` | Returns the text alignment mode of a dimension style. |
| `DimStyleTextAlignment` | `dimStyle: string * alignment: int -> unit` | Changes the text alignment mode of a dimension style. |
| `DimStyleTextGap` | `dimStyle: string -> float` | Returns the text gap used by a dimension style. |
| `DimStyleTextGap` | `dimStyle: string * gap: float -> unit` | Changes the text gap used by a dimension style. |
| `DimStyleTextHeight` | `dimStyle: string -> float` | Returns the text height used by a dimension style. |
| `DimStyleTextHeight` | `dimStyle: string * height: float -> unit` | Changes the text height used by a dimension style. |
| `IsAlignedDimension` | `objectId: Guid -> bool` | Checks if an object is an aligned dimension object. |
| `IsAlignedDimension` | `geometryObject: GeometryBase -> bool` | Checks if an object is an aligned dimension object. |
| `IsAngularDimension` | `objectId: Guid -> bool` | Checks if an object is an angular dimension object. |
| `IsAngularDimension` | `geometryObject: GeometryBase -> bool` | Checks if an object is an angular dimension object. |
| `IsDiameterDimension` | `objectId: Guid -> bool` | Checks if an object is a diameter dimension object. |
| `IsDiameterDimension` | `geometryObject: GeometryBase -> bool` | Checks if an object is a diameter dimension object. |
| `IsDimension` | `objectId: Guid -> bool` | Checks if an object is a dimension object. |
| `IsDimension` | `geometryObject: GeometryBase -> bool` | Checks if an object is a dimension object. |
| `IsDimStyle` | `dimStyle: string -> bool` | Checks if the existence of a dimension style in the document. |
| `IsDimStyleReference` | `dimStyle: string -> bool` | Checks if that an existing dimension style is from a reference file. |
| `IsLeader` | `objectId: Guid -> bool` | Checks if an object is a dimension leader object. |
| `IsLeader` | `geometryObject: GeometryBase -> bool` | Checks if an object is a dimension leader object. |
| `IsLinearDimension` | `objectId: Guid -> bool` | Checks if an object is a linear dimension object. |
| `IsLinearDimension` | `geometryObject: GeometryBase -> bool` | Checks if an object is a linear dimension object. |
| `IsOrdinateDimension` | `objectId: Guid -> bool` | Checks if an object is an ordinate dimension object. |
| `IsOrdinateDimension` | `geometryObject: GeometryBase -> bool` | Checks if an object is an ordinate dimension object. |
| `IsRadialDimension` | `objectId: Guid -> bool` | Checks if an object is a radial dimension object. |
| `IsRadialDimension` | `geometryObject: GeometryBase -> bool` | Checks if an object is a radial dimension object. |
| `LeaderText` | `objectId: Guid -> string` | Returns the text string of a dimension leader object. |
| `LeaderText` | `objectId: Guid * text: string -> unit` | Modifies the text string of a dimension leader object. |
| `LeaderText` | `objectIds: Guid seq * text: string -> unit` | Modifies the text string of multiple dimension leader objects. |
| `RenameDimStyle` | `oldStyle: string * newStyle: string -> unit` | Renames an existing dimension style. |

## Document

From [Src/Scripting_Document.fs](../Src/Scripting_Document.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `CreatePreviewImage` | `fileName: string * ?view: string = "" * ?width: int = 0 * ?height: int = 0 * ?flags: int = 0 * ?wireframe: bool = false -> bool` | Creates a bitmap preview image of the current model. |
| `DisableRedraw` | `unit -> unit` | Disables screen redrawing. |
| `DocumentModified` | `unit -> bool` | Returns the document's modified flag. |
| `DocumentModified` | `modified: bool -> unit` | Sets the document's modified flag. |
| `DocumentName` | `unit -> string` | Returns the name of the currently loaded Rhino document (3dm file). |
| `DocumentPath` | `unit -> string` | Returns full path of the currently loaded Rhino document including the file name (3dm file). |
| `EnableRedraw` | `?enable: bool = true -> unit` | Enables or disables screen redrawing. |
| `ExtractPreviewImage` | `fileName: string * ?modelName: string = null -> unit` | Extracts the bitmap preview image from the specified model (.3dm). |
| `IsDocumentModified` | `unit -> bool` | Checks if the current document has been modified in some way. |
| `Notes` | `unit -> string` | Returns the document's notes. |
| `Notes` | `newNotes: string -> unit` | Sets the document's notes. |
| `ReadFileVersion` | `unit -> int` | Returns the file version of the current document. |
| `Redraw` | `unit -> unit` | Redraws all views. |
| `RenderAntialias` | `unit -> int` | Returns render antialiasing style. |
| `RenderAntialias` | `style: int -> unit` | Sets render antialiasing style. |
| `RenderColor` | `item: int -> Color` | Returns the render ambient light or background color. |
| `RenderColor` | `item: int * color: Color -> unit` | Sets the render ambient light or background color. |
| `RenderMeshDensity` | `unit -> float` | Returns the render Mesh density property of the active document. |
| `RenderMeshDensity` | `density: float -> unit` | Sets the render Mesh density property of the active document. |
| `RenderMeshMaxAngle` | `unit -> float` | Returns the render Mesh maximum angle property of the active document. |
| `RenderMeshMaxAngle` | `angleDegrees: float -> unit` | Sets the render Mesh maximum angle property of the active document. |
| `RenderMeshMaxAspectRatio` | `unit -> float` | Returns the render Mesh maximum aspect ratio property of the active document. |
| `RenderMeshMaxAspectRatio` | `ratio: float -> unit` | Sets the render Mesh maximum aspect ratio property of the active document. |
| `RenderMeshMaxDistEdgeToSrf` | `unit -> float` | Returns the render Mesh maximum distance, edge to Surface parameter of the active document. |
| `RenderMeshMaxDistEdgeToSrf` | `distance: float -> unit` | Sets the render Mesh maximum distance, edge to Surface parameter of the active document. |
| `RenderMeshMaxEdgeLength` | `unit -> float` | Returns the render Mesh maximum edge length parameter of the active document. |
| `RenderMeshMaxEdgeLength` | `distance: float -> unit` | Sets the render Mesh maximum edge length parameter of the active document. |
| `RenderMeshMinEdgeLength` | `unit -> float` | Returns the render Mesh minimum edge length parameter of the active document. |
| `RenderMeshMinEdgeLength` | `distance: float -> unit` | Sets the render Mesh minimum edge length parameter of the active document. |
| `RenderMeshMinInitialGridQuads` | `unit -> int` | Returns the render Mesh minimum initial grid quads parameter of the active document. |
| `RenderMeshMinInitialGridQuads` | `quads: int -> unit` | Sets the render Mesh minimum initial grid quads parameter of the active document. |
| `RenderMeshQuality` | `unit -> int` | Returns the render Mesh quality of the active document. |
| `RenderMeshQuality` | `quality: int -> unit` | Sets the render Mesh quality of the active document. |
| `RenderMeshSettings` | `unit -> int` | Returns the render Mesh settings of the active document. |
| `RenderMeshSettings` | `settings: int -> unit` | Sets the render Mesh settings of the active document. |
| `RenderResolution` | `unit -> int * int` | Returns the render resolution. |
| `RenderResolution` | `width: int * height: int -> unit` | Sets the render resolution. |
| `RenderSettings` | `unit -> int` | Returns render settings. |
| `RenderSettings` | `settings: int -> unit` | Sets render settings. |
| `UnitAbsoluteTolerance` | `unit -> float` | Returns the document's absolute tolerance. |
| `UnitAbsoluteTolerance` | `tolerance: float -> unit` | Sets the document's absolute tolerance. |
| `UnitAngleTolerance` | `unit -> float` | Return the document's angle tolerance. |
| `UnitAngleTolerance` | `angleToleranceDegrees: float -> unit` | Set the document's angle tolerance. |
| `UnitDistanceDisplayPrecision` | `unit -> int` | Return the document's distance display precision. |
| `UnitDistanceDisplayPrecision` | `precision: int -> unit` | Set the document's distance display precision. |
| `UnitRelativeTolerance` | `unit -> float` | Return the document's relative tolerance. |
| `UnitRelativeTolerance` | `relativeTolerance: float -> unit` | Set the document's relative tolerance. |
| `UnitScale` | `toSystem: int * fromSystem: int -> float` | Return the scale factor for changing between unit systems. |
| `UnitSystem` | `unit -> int` | Return the document's unit system. |
| `UnitSystem` | `unitSystem: int * ?scale: bool = false -> unit` | Set the document's unit system. |
| `UnitSystemName` | `?capitalize: bool = false * ?singular: bool = true * ?abbreviate: bool = false * ?modelUnits: bool = true -> string` | Returns the name of the current unit system. |

## Geometry

From [Src/Scripting_Geometry.fs](../Src/Scripting_Geometry.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddClippingPlane` | `plane: Plane * uMagnitude: float * vMagnitude: float * ?views: string seq = null -> Guid` | Create a clipping Plane for visibly clipping away geometry in a specific view. |
| `AddPictureFrame` | `plane: Plane * filename: string * ?width: float = 0.0 * ?height: float = 0.0 * ?selfIllumination: bool = true * ?embed: bool = false * ?useAlpha: bool = false * ?makeMesh: bool = false -> Guid` | Creates a picture frame and adds it to the document. |
| `AddPoint` | `x: float * y: float * z: float -> Guid` | Adds point object to the document. |
| `AddPoint` | `point: Point3d -> Guid` | Adds point object to the document. |
| `AddPointCloud` | `points: Point3d array * ?colors: IList<Color> = null -> Guid` | Adds point cloud object to the document. |
| `AddPoints` | `points: Point3d seq -> ResizeArray<Guid>` | Adds one or more point objects to the document. |
| `AddText` | `text: string * plane: Plane * ?height: float = 1.0 * ?font: string = null * ?fontStyle: int = 0 * ?horizontalAlignment: byte = 1uy * ?verticalAlignment: byte = 3uy -> Guid` | Adds a text string to the document. |
| `AddText` | `text: string * pt: Point3d * ?height: float = 1.0 * ?font: string = null * ?fontStyle: int = 0 * ?horizontalAlignment: byte = 1uy * ?verticalAlignment: byte = 3uy -> Guid` | Adds a text string to the document. |
| `AddTextDot` | `text: string * point: Point3d -> Guid` | Add a text dot to the document. |
| `AddTextDot` | `text: string * x: float * y: float * z: float -> Guid` | Add a text dot to the document. |
| `Area` | `geometry: GeometryBase -> float` | Compute the area of a closed Curve, Hatch, Surface, Polysurface, or Mesh. |
| `Area` | `objectId: Guid -> float` | Compute the area of a closed Curve, Hatch, Surface, Polysurface, or Mesh. |
| `BoundingBox` | `geos: 'a seq -> BoundingBox` | Returns a world axis-aligned bounding box of several geometry objects. |
| `BoundingBox` | `objects: Guid seq -> BoundingBox` | Returns a world axis-aligned bounding box of several objects. |
| `BoundingBox` | `object: Guid -> BoundingBox` | Returns a world axis-aligned bounding box of one object. |
| `BoundingBox` | `objects: Guid seq * plane: Plane * ?inWorldCoords: bool = true -> Box` | Returns a custom Plane axis-aligned bounding box of several objects. |
| `BoundingBox` | `object: Guid * plane: Plane * ?inWorldCoords: bool = true -> Box` | Returns a custom Plane axis-aligned bounding box of one object. |
| `BoundingBox` | `object: GeometryBase -> BoundingBox` | Returns a world axis-aligned bounding box of one object. |
| `BoundingBox` | `objects: GeometryBase seq * plane: Plane * ?inWorldCoords: bool = true -> Box` | Returns a custom Plane axis-aligned bounding box of several objects. |
| `BoundingBox` | `object: GeometryBase * plane: Plane * ?inWorldCoords: bool = true -> Box` | Returns a custom Plane axis-aligned bounding box of one object. |
| `BoundingBoxEstimate` | `geometries: 'a seq -> BoundingBox` | Returns a world axis-aligned bounding box of several objects. |
| `BoundingBoxEstimate` | `objects: Guid seq -> BoundingBox` | Returns a world axis-aligned bounding box of several objects. |
| `BoundingBoxEstimate` | `object: Guid -> BoundingBox` | Returns a world axis-aligned bounding box of one object. |
| `BoundingBoxEstimate` | `object: GeometryBase -> BoundingBox` | Returns a world axis-aligned bounding box of one object. |
| `BoundingBoxInflate` | `bbox: BoundingBox * amount: float -> BoundingBox` | Returns a new inflated the box with equal amounts in all directions. |
| `BoundingBoxInflate` | `bbox: BoundingBox * amountX: float * amountY: float * amountZ: float -> BoundingBox` | Returns a new inflated box with custom x, y and z amounts in their directions. |
| `CompareGeometry` | `first: Guid * second: Guid -> bool` | Compares two objects to determine if they are geometrically identical. |
| `CompareGeometry` | `first: GeometryBase * second: GeometryBase -> bool` | Compares two objects to determine if they are geometrically identical. |
| `ExplodeText` | `textId: Guid * ?delete: bool = false -> ResizeArray<Guid>` | Creates outline Curves for a given text entity. |
| `ExplodeText` | `text: TextEntity -> ResizeArray<Curve>` | Creates outline curves from text geometry without adding them to the document. |
| `IsClippingPlane` | `objectId: Guid -> bool` | Checks if that an object is a clipping Plane object. |
| `IsClippingPlane` | `geometryObject: GeometryBase -> bool` | Checks if that an object is a clipping Plane object. |
| `IsPoint` | `objectId: Guid -> bool` | Checks if an object is a point object. |
| `IsPoint` | `geometryObject: GeometryBase -> bool` | Checks if an object is a point object. |
| `IsPointCloud` | `objectId: Guid -> bool` | Checks if an object is a point cloud object. |
| `IsPointCloud` | `geometryObject: GeometryBase -> bool` | Checks if an object is a point cloud object. |
| `IsText` | `objectId: Guid -> bool` | Checks if an object is a text object. |
| `IsText` | `geometryObject: GeometryBase -> bool` | Checks if an object is a text object. |
| `IsTextDot` | `objectId: Guid -> bool` | Checks if an object is a text dot object. |
| `IsTextDot` | `geometryObject: GeometryBase -> bool` | Checks if an object is a text dot object. |
| `PointCloudClosestPoints` | `ptCloud: Point3d seq * needlePoints: Point3d seq * distance: float -> int array seq` | Returns a list of lists of point indices in a point cloud that are closest to needlePoints. |
| `PointCloudCount` | `objectId: Guid -> int` | Returns the point count of a point cloud object. |
| `PointCloudCount` | `geometryObject: PointCloud -> int` | Returns the point count of a point cloud object. |
| `PointCloudHasHiddenPoints` | `objectId: Guid -> bool` | Checks if a point cloud has hidden points. |
| `PointCloudHasHiddenPoints` | `geometryObject: PointCloud -> bool` | Checks if a point cloud has hidden points. |
| `PointCloudHasPointColors` | `objectId: Guid -> bool` | Checks if a point cloud has point colors. |
| `PointCloudHasPointColors` | `geometryObject: PointCloud -> bool` | Checks if a point cloud has point colors. |
| `PointCloudHidePoints` | `objectId: Guid -> ResizeArray<bool>` | Returns the hidden state of points in a point cloud object. |
| `PointCloudHidePoints` | `objectId: Guid * hidden: bool seq -> unit` | Modifies the hidden points of a point cloud object. |
| `PointCloudHidePoints` | `geometryObject: PointCloud -> ResizeArray<bool>` | Returns the hidden state of points in a point cloud object. |
| `PointCloudKNeighbors` | `ptCloud: Point3d seq * needlePoints: Point3d seq * ?amount: int = 1 -> int array seq` | Returns amount indices of points in a point cloud that are near needlePoints. |
| `PointCloudPointColors` | `objectId: Guid -> ResizeArray<Color>` | Returns the point colors of a point cloud object. |
| `PointCloudPointColors` | `objectId: Guid * colors: Color seq -> unit` | Modifies the point colors of a point cloud object. |
| `PointCloudPointColors` | `geometryObject: PointCloud -> ResizeArray<Color>` | Returns the point colors of a point cloud object. |
| `PointCloudPoints` | `objectId: Guid -> Point3d array` | Returns the points of a point cloud object. |
| `PointCloudPoints` | `geometryObject: PointCloud -> Point3d array` | Returns the points of a point cloud object. |
| `PointCoordinates` | `objectId: Guid -> Point3d` | Returns the X, Y, and Z coordinates of a point object. |
| `PointCoordinates` | `objectId: Guid * point: Point3d -> unit` | Modifies the X, Y, and Z coordinates of a point object. |
| `PointCoordinates` | `point: Point3d -> Point3d` | Returns the X, Y, and Z coordinates of a point object. |
| `PointCoordinates` | `point: Point -> Point3d` | Returns the X, Y, and Z coordinates of a point geometry object. |
| `TextDotFont` | `objectId: Guid -> string` | Returns the font of a text dot. |
| `TextDotFont` | `objectId: Guid * fontFace: string -> unit` | Modifies the font of a text dot. |
| `TextDotFont` | `objectIds: Guid seq * fontFace: string -> unit` | Modifies the font of multiple text dots. |
| `TextDotFont` | `geometryObject: TextDot -> string` | Returns the font of a text dot. |
| `TextDotHeight` | `objectId: Guid -> int` | Returns the font height of a text dot. |
| `TextDotHeight` | `objectId: Guid * height: int -> unit` | Modifies the font height of a text dot. |
| `TextDotHeight` | `objectIds: Guid seq * height: int -> unit` | Modifies the font height of multiple text dots. |
| `TextDotHeight` | `geometryObject: TextDot -> int` | Returns the font height of a text dot. |
| `TextDotPoint` | `objectId: Guid -> Point3d` | Returns the location, or insertion point, on a text dot object. |
| `TextDotPoint` | `objectId: Guid * point: Point3d -> unit` | Modifies the location, or insertion point, on a text dot object. |
| `TextDotPoint` | `geometryObject: TextDot -> Point3d` | Returns the location, or insertion point, on a text dot object. |
| `TextDotText` | `objectId: Guid -> string` | Returns the text on a text dot object. |
| `TextDotText` | `objectId: Guid * text: string -> unit` | Modifies the text on a text dot object. |
| `TextDotText` | `objectIds: Guid seq * text: string -> unit` | Modifies the text on multiple text dot objects. |
| `TextDotText` | `geometryObject: TextDot -> string` | Returns the text on a text dot object. |
| `TextObjectFont` | `objectId: Guid -> string` | Returns the font used by a text object. |
| `TextObjectFont` | `objectId: Guid * font: string -> unit` | Modifies the font used by a text object. |
| `TextObjectFont` | `objectIds: Guid seq * font: string -> unit` | Modifies the font used by multiple text objects. |
| `TextObjectFont` | `geometryObject: TextEntity -> string` | Returns the font used by a text object. |
| `TextObjectHeight` | `objectId: Guid -> float` | Returns the height of a text object. |
| `TextObjectHeight` | `objectId: Guid * height: float -> unit` | Modifies the height of a text object. |
| `TextObjectHeight` | `objectIds: Guid seq * height: float -> unit` | Modifies the height of multiple text objects. |
| `TextObjectHeight` | `geometryObject: TextEntity -> float` | Returns the height of a text object. |
| `TextObjectPlane` | `objectId: Guid -> Plane` | Returns the Plane used by a text object. |
| `TextObjectPlane` | `objectId: Guid * plane: Plane -> unit` | Modifies the Plane used by a text object. |
| `TextObjectPlane` | `geometryObject: TextEntity -> Plane` | Returns the Plane used by a text object. |
| `TextObjectPoint` | `objectId: Guid -> Point3d` | Returns the location of a text object. |
| `TextObjectPoint` | `objectId: Guid * point: Point3d -> unit` | Modifies the location of a text object. |
| `TextObjectPoint` | `geometryObject: TextEntity -> Point3d` | Returns the location of a text object. |
| `TextObjectRichText` | `objectId: Guid -> string` | Returns the RichText formatting string of a text object. |
| `TextObjectRichText` | `objectId: Guid * rtfString: string * ?style: string = "" -> unit` | Modifies the RichText formatting string of a text object. |
| `TextObjectRichText` | `objectIds: Guid seq * rtfString: string * ?style: string = "" -> unit` | Modifies the RichText formatting string of multiple text objects. |
| `TextObjectRichText` | `geometryObject: TextEntity -> string` | Returns the RichText formatting string of a text object. |
| `TextObjectStyle` | `objectId: Guid -> int` | Returns the font style of a text object. |
| `TextObjectStyle` | `objectId: Guid * style: int -> unit` | Modifies the font style of a text object. |
| `TextObjectStyle` | `objectIds: Guid seq * style: int -> unit` | Modifies the font style of multiple text objects. |
| `TextObjectStyle` | `geometryObject: TextEntity -> int` | Returns the font style of a text object. |
| `TextObjectText` | `objectId: Guid -> string` | Returns the plain text string of a text object. |
| `TextObjectText` | `objectId: Guid * text: string -> unit` | Modifies the plain text string of a text object. |
| `TextObjectText` | `objectIds: Guid seq * text: string -> unit` | Modifies the plain text string of multiple text objects. |
| `TextObjectText` | `geometryObject: TextEntity -> string` | Returns the plain text string of a text object. |

## Grips

From [Src/Scripting_Grips.fs](../Src/Scripting_Grips.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `EnableObjectGrips` | `objectId: Guid * ?enable: bool = true -> bool` | Enables or disables an object's grips. |
| `GetObjectGrip` | `?message: string = null * ?preselect: bool = false * ?select: bool = false -> Guid * int * Point3d` | Prompts the user to pick a single object grip. |
| `GetObjectGrips` | `?message: string = null * ?preselect: bool = false * ?select: bool = false -> ResizeArray<Guid * int * Point3d>` | Prompts the user to pick one or more object grips from one or more objects. |
| `NextObjectGrip` | `objectId: Guid * index: int * ?direction: int = 0 * ?enable: bool = true -> int` | Returns the next grip index from a specified grip index of an object. |
| `ObjectGripCount` | `objectId: Guid -> int` | Returns number of grips owned by an object. |
| `ObjectGripLocation` | `objectId: Guid * index: int -> Point3d` | Returns the location of an object's grip. |
| `ObjectGripLocation` | `objectId: Guid * index: int * point: Point3d -> unit` | Modifies the location of an object's grip. |
| `ObjectGripLocations` | `objectId: Guid -> ResizeArray<Point3d>` | Returns the location of all grips owned by an object. |
| `ObjectGripLocations` | `objectId: Guid * points: Point3d seq -> unit` | Modifies the location of all grips owned by an object. |
| `ObjectGripsOn` | `objectId: Guid -> bool` | Checks if an object's grips are turned on. |
| `ObjectGripsSelected` | `objectId: Guid -> bool` | Checks if an object's grips are turned on and at least one grip is selected. |
| `PrevObjectGrip` | `objectId: Guid * index: int * ?direction: int = 0 * ?enable: bool = true -> int` | Returns the previous grip index from a specified grip index of an object. |
| `SelectedObjectGrips` | `objectId: Guid -> ResizeArray<int>` | Returns a list of grip indices identifying an object's selected grips. |
| `SelectObjectGrip` | `objectId: Guid * index: int -> bool` | Selects a single grip owned by an object. |
| `SelectObjectGrips` | `objectId: Guid -> int` | Selects an object's grips. |
| `UnselectObjectGrip` | `objectId: Guid * index: int -> bool` | Unselects a single grip owned by an object. |
| `UnselectObjectGrips` | `objectId: Guid -> int` | Unselects an object's grips. |

## Group

From [Src/Scripting_Group.fs](../Src/Scripting_Group.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddGroup` | `?groupName: string = null -> string` | Adds a new empty group to the document. |
| `AddObjectsToGroup` | `objectIds: Guid seq * groupName: string -> unit` | Adds one or more objects to an existing group. |
| `AddObjectToGroup` | `objectId: Guid * groupName: string -> unit` | Adds a single object to an existing group. |
| `DeleteGroup` | `groupName: string -> unit` | Removes an existing group from the document. |
| `GroupCount` | `unit -> int` | Returns the number of groups in the document. |
| `GroupNames` | `unit -> string array` | Returns the names of all the groups in the document. |
| `GroupObjects` | `objectIds: Guid seq -> unit` | Adds two or more objects to a new group. |
| `GroupObjects` | `objectIds: Guid seq * groupName: string -> unit` | Adds two or more objects to a new group, sets group name. |
| `HideGroup` | `groupName: string -> int` | Hides a group of objects. |
| `IsGroup` | `groupName: string -> bool` | Verifies the existence of a group. |
| `IsGroupEmpty` | `groupName: string -> bool` | Checks if an existing group is empty, or contains no object members. |
| `LockGroup` | `groupName: string -> int` | Locks a group of objects. |
| `ObjectGroups` | `objId: Guid -> ResizeArray<string>` | Returns the names of all groups that an object is part of . |
| `ObjectTopGroup` | `objId: Guid -> string` | Returns the top most group name that an object is assigned. |
| `RemoveObjectFromAllGroups` | `objectId: Guid -> bool` | Removes a single object from any and all groups that it is a member. |
| `RemoveObjectFromGroup` | `objectId: Guid * groupName: string -> unit` | Remove a single object from an existing group. |
| `RemoveObjectsFromGroup` | `objectIds: Guid seq * groupName: string -> unit` | Removes multiple objects from an existing group. |
| `RenameGroup` | `oldName: string * newName: string -> unit` | Renames an existing group. |
| `ShowGroup` | `groupName: string -> int` | Shows a group of previously hidden objects. |
| `UnlockGroup` | `groupName: string -> int` | Unlocks a group of previously locked objects. |

## Hatch

From [Src/Scripting_Hatch.fs](../Src/Scripting_Hatch.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddHatch` | `curveId: Guid * ?hatchPattern: string = null * ?scale: float = 1.0 * ?rotation: float = 0.0 * ?tolerance: float = 0.0 -> Guid` | Creates a new Hatch object from a closed planar Curve object. |
| `MakeHatch` | `curve: Curve * ?hatchPattern: string = null * ?scale: float = 1.0 * ?rotation: float = 0.0 * ?tolerance: float = 0.0 -> Hatch` | Creates one Hatch object from one closed planar Curve. |
| `AddHatches` | `curveIds: Guid seq * ?hatchPattern: string = null * ?scale: float = 1.0 * ?rotation: float = 0.0 * ?tolerance: float = 0.0 -> ResizeArray<Guid>` | Creates one or more new Hatch objects from a list of closed planar Curves. |
| `MakeHatches` | `curves: Curve seq * ?hatchPattern: string = null * ?scale: float = 1.0 * ?rotation: float = 0.0 * ?tolerance: float = 0.0 -> ResizeArray<Hatch>` | Creates one or more new Hatch objects from a list of closed planar Curves. |
| `AddHatchPatterns` | `filename: string * ?replace: bool = false -> ResizeArray<string>` | Adds Hatch patterns to the document by importing Hatch pattern definitions from a pattern file. |
| `CurrentHatchPattern` | `unit -> string` | Returns the current Hatch pattern file. |
| `CurrentHatchPattern` | `hatchPattern: string -> unit` | Sets the current Hatch pattern file. |
| `ExplodeHatch` | `hatchId: Guid * ?delete: bool = false -> ResizeArray<Guid>` | Explodes a Hatch object into its component objects. |
| `ExplodeHatch` | `hatch: Hatch -> ResizeArray<GeometryBase>` | Explodes hatch geometry into curves or planar breps without adding them to the document. |
| `HatchPattern` | `hatchId: Guid -> string` | Returns a Hatch object's Hatch pattern. |
| `HatchPattern` | `hatchId: Guid * hatchPattern: string -> unit` | Changes a Hatch object's Hatch pattern. |
| `HatchPattern` | `hatchIds: Guid seq * hatchPattern: string -> unit` | Changes multiple Hatch objects' Hatch pattern. |
| `HatchPattern` | `hatchGeo: Hatch -> string` | Returns a Hatch object's Hatch pattern. |
| `HatchPatternCount` | `unit -> int` | Returns the number of Hatch patterns in the document. |
| `HatchPatternDescription` | `hatchPattern: string -> string` | Returns the description of a Hatch pattern. |
| `HatchPatternFillType` | `hatchPattern: string -> int` | Returns the fill type of a Hatch pattern. |
| `HatchPatternNames` | `unit -> ResizeArray<string>` | Returns the names of all of the Hatch patterns in the document. |
| `HatchRotation` | `hatchId: Guid -> float` | Returns the rotation applied to the Hatch pattern when it is mapped to the Hatch's Plane. |
| `HatchRotation` | `hatchId: Guid * rotation: float -> unit` | Modifies the rotation applied to the Hatch pattern when it is mapped to the Hatch's Plane. |
| `HatchRotation` | `hatchIds: Guid seq * rotation: float -> unit` | Modifies the rotation applied to the Hatch pattern when it is mapped to the Hatch's Plane. |
| `HatchRotation` | `hatchGeo: Hatch -> float` | Returns the rotation applied to the Hatch pattern when it is mapped to the Hatch's Plane. |
| `HatchScale` | `hatchId: Guid -> float` | Returns the scale applied to the Hatch pattern when it is mapped to the Hatch's Plane. |
| `HatchScale` | `hatchId: Guid * scale: float -> unit` | Modifies the scale applied to the Hatch pattern when it is mapped to the Hatch's Plane. |
| `HatchScale` | `hatchIds: Guid seq * scale: float -> unit` | Modifies the scale applied to the Hatch pattern when it is mapped to the Hatch's Plane. |
| `HatchScale` | `hatchGeo: Hatch -> float` | Returns the scale applied to the Hatch pattern when it is mapped to the Hatch's Plane. |
| `IsHatch` | `objectId: Guid -> bool` | Verifies the existence of a Hatch object in the document. |
| `IsHatch` | `geometry: GeometryBase -> bool` | Tests whether the supplied geometry is a Hatch. |
| `IsHatchPattern` | `name: string -> bool` | Verifies the existence of a Hatch pattern in the document. |
| `IsHatchPatternCurrent` | `hatchPattern: string -> bool` | Checks if a Hatch pattern is the current Hatch pattern. |
| `IsHatchPatternReference` | `hatchPattern: string -> bool` | Checks if a Hatch pattern is from a reference file. |

## Layer

From [Src/Scripting_Layer.fs](../Src/Scripting_Layer.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddLayer` | `?name: string = null * ?color: Color = Drawing.Color() * ?visible: int = 2 * ?locked: int = 2 * ?parent: string = null * ?allowAllUnicode: bool = false * ?collapseParents: bool = false -> int` | Adds a new layer to the document. |
| `ChangeLayerName` | `currentLayerName: string * newLayerName: string * ?allowUnicode: bool = false -> unit` | Changes the Name of a layer if than name is yet non existing. |
| `CollapseLayer` | `layerName: string * ?childrenToo: bool = false -> unit` | Collapse a layer in UI if it has children. |
| `CurrentLayer` | `unit -> string` | Returns the current layer. |
| `CurrentLayer` | `layer: string -> unit` | Changes the current layer. |
| `DeleteLayer` | `layer: string -> bool` | Removes an existing layer from the document. |
| `ExpandLayer` | `layer: string * expand: bool -> unit` | Expands or Collapses a layer. |
| `IsLayer` | `layer: string -> bool` | Verifies the existence of a layer in the document. |
| `IsLayerChangeable` | `layer: string -> bool` | Checks if the objects on a layer can be changed (normal). |
| `IsLayerChildOf` | `layer: string * test: string -> bool` | Checks if a layer is a child of another layer. |
| `IsLayerCurrent` | `layer: string -> bool` | Checks if a layer is the current layer. |
| `IsLayerEmpty` | `layer: string -> bool` | Checks if an existing layer is empty, or contains no objects. |
| `IsLayerExpanded` | `layer: string -> bool` | Checks if a layer is expanded. |
| `IsLayerLocked` | `layer: string -> bool` | Checks if a layer is locked persistent or non persistent locking return true via layer.IsLocked. |
| `IsLayerOn` | `layer: string -> bool` | Checks if a layer is on. |
| `IsLayerParentOf` | `layer: string * test: string -> bool` | Checks if a layer is a parent of another layer. |
| `IsLayerReference` | `layer: string -> bool` | Checks if a layer is from a reference file. |
| `IsLayerSelectable` | `layer: string -> bool` | Checks if an existing layer is selectable (normal and reference). |
| `IsLayerVisible` | `layer: string -> bool` | Checks if a layer is visible. |
| `LayerChildCount` | `layer: string -> int` | Returns the number of immediate child layers of a layer. |
| `LayerChildren` | `layer: string -> ResizeArray<string>` | Returns the immediate child layers of a layer. |
| `LayerChildrenAll` | `layer: string -> ResizeArray<string>` | Returns all (immediate and descendent) child and grand child layers of a layer. |
| `LayerColor` | `layer: string -> Color` | Returns the color of a layer. |
| `LayerColor` | `layer: string * color: Color -> unit` | Changes the color of a layer. |
| `LayerCount` | `unit -> int` | Returns the number of layers in the document. |
| `LayerId` | `layer: string -> Guid` | Returns the identifier of a layer given the layer's name. |
| `LayerIds` | `unit -> ResizeArray<Guid>` | Return identifiers of all layers in the document. |
| `LayerLinetype` | `layer: string -> string` | Returns the line type of a layer. |
| `LayerLinetype` | `layer: string * lineType: string -> unit` | Changes the line type of a layer. |
| `LayerLock` | `layer: string -> unit` | Locks a layer if it is not already locked via a parent. |
| `LayerLocked` | `layer: string -> bool` | Returns the locked property of a layer, if layer is unlocked but parent layer is locked this still returns true. |
| `LayerLockedSetFalse` | `layer: string * ?parentsToo: bool = true -> unit` | Makes a layer unlocked. |
| `LayerLockedSetTrue` | `layer: string * ?forceLocked: bool = false -> unit` | Makes a layer locked. |
| `LayerMaterialIndex` | `layer: string -> int` | Returns the material index of a layer. |
| `LayerMaterialIndex` | `layer: string * index: int -> unit` | Changes the material index of a layer. |
| `LayerName` | `layerId: Guid * ?fullPath: bool = true -> string` | Return the name of a layer given it's identifier. |
| `LayerNames` | `unit -> ResizeArray<string>` | Returns the names of all layers in the document. |
| `LayerNames` | `sort: bool -> ResizeArray<string>` | Returns the names, optionally sorted in ordinal order, like in RhinoPython. |
| `LayerOff` | `layer: string -> unit` | Turn a layer off if visible, does nothing if parent layer is already invisible. . |
| `LayerOn` | `layer: string -> unit` | Turn a layer on if not visible , enforces visibility of parents. |
| `LayerOrder` | `layer: string -> int` | Returns the current display order index of a layer as displayed in Rhino's layer dialog box. |
| `LayerPrintColor` | `layer: string -> Color` | Returns the print color of a layer. |
| `LayerPrintColor` | `layer: string * color: Color -> unit` | Changes the print color of a layer. |
| `LayerPrintWidth` | `layer: string -> float` | Returns the print width of a layer. |
| `LayerPrintWidth` | `layer: string * width: float -> unit` | Changes the print width of a layer. |
| `LayerUnlock` | `layer: string -> unit` | Unlocks a layer and all parents if needed . |
| `LayerVisible` | `layer: string -> bool` | Returns the visible property of a layer, if layer is on but invisible because of a parent that is off this returns false. |
| `LayerVisibleSetFalse` | `layer: string * ?persist: bool = false -> unit` | Makes a layer invisible. |
| `LayerVisibleSetTrue` | `layer: string * ?forceVisible: bool = true -> unit` | Makes a layer visible. |
| `ObjectLayer` | `objectId: Guid -> string` | Returns the full layer name of an object. parent layers are separated by `::`. |
| `ObjectLayer` | `objectId: Guid * layer: string * ?createLayerIfMissing: bool = false * ?allowAllUnicode: bool = false * ?collapseParents: bool = false -> unit` | Modifies the layer of an object , optionally creates layer if it does not exist yet. |
| `ObjectLayer` | `objectIds: Guid seq * layer: string * ?createLayerIfMissing: bool = false * ?allowUnicode: bool = false * ?collapseParents: bool = false -> unit` | Modifies the layer of multiple objects, optionally creates layer if it does not exist yet. |
| `ObjectLayer` | `objectId: Guid * layerIndex: int -> unit` | Modifies the layer of an object. |
| `ObjectLayer` | `objectIds: Guid seq * layerIndex: int -> unit` | Modifies the layer of multiple objects, optionally creates layer if it does not exist yet. |
| `ParentLayer` | `layer: string -> string` | Return the parent layer of a layer or empty string if no parent present. |
| `ParentLayer` | `layer: string * parent: string -> unit` | Modify the parent layer of a layer. |
| `PurgeEmptyLayers` | `?keepCurrent: bool = true -> unit` | Removes all empty layers from the document. |
| `PurgeLayer` | `layer: string -> bool` | Removes an existing layer and all its objects from the document. |
| `RenameLayer` | `oldName: string * newName: string -> unit` | Renames an existing layer. |
| `ShowLayer` | `layerName: string -> unit` | Expand all parent layers in UI if this layer is hidden by a collapsed parent layer. |
| `UnCollapseLayer` | `layerName: string * ?childrenToo: bool = false -> unit` | Expand a layer in UI if it has children. |

## Light

From [Src/Scripting_Light.fs](../Src/Scripting_Light.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddDirectionalLight` | `startPoint: Point3d * endPoint: Point3d -> Guid` | Adds a new directional light object to the document. |
| `AddLinearLight` | `startPoint: Point3d * endPoint: Point3d * ?lightWidth: float = 0.0 -> Guid` | Adds a new linear light object to the document. |
| `AddPointLight` | `point: Point3d -> Guid` | Adds a new point light object to the document. |
| `AddRectangularLight` | `origin: Point3d * widthPoint: Point3d * heightPoint: Point3d -> Guid` | Adds a new rectangular light object to the document. |
| `AddSpotLight` | `origin: Point3d * radius: float * apexPoint: Point3d -> Guid` | Adds a new spot light object to the document. |
| `EnableLight` | `objectId: Guid -> bool` | Get On / Off status of a light object. |
| `EnableLight` | `objectId: Guid * enable: bool -> unit` | Enables or disables a light object. |
| `EnableLight` | `objectIds: Guid seq * enable: bool -> unit` | Enables or disables multiple light objects. |
| `EnableLight` | `geometryObject: Light -> bool` | Get On / Off status of a light object. |
| `IsDirectionalLight` | `objectId: Guid -> bool` | Verifies a light object is a directional light. |
| `IsDirectionalLight` | `geometryObject: GeometryBase -> bool` | Verifies a light object is a directional light. |
| `IsLight` | `objectId: Guid -> bool` | Verifies an object is a light object. |
| `IsLight` | `geometryObject: GeometryBase -> bool` | Verifies an object is a light object. |
| `IsLightEnabled` | `objectId: Guid -> bool` | Verifies a light object is enabled. |
| `IsLightEnabled` | `geometryObject: Light -> bool` | Verifies a light object is enabled. |
| `IsLightReference` | `objectId: Guid -> bool` | Verifies a light object is referenced from another file. |
| `IsLinearLight` | `objectId: Guid -> bool` | Verifies a light object is a linear light. |
| `IsLinearLight` | `geometryObject: GeometryBase -> bool` | Verifies a light object is a linear light. |
| `IsPointLight` | `objectId: Guid -> bool` | Verifies a light object is a point light. |
| `IsPointLight` | `geometryObject: GeometryBase -> bool` | Verifies a light object is a point light. |
| `IsRectangularLight` | `objectId: Guid -> bool` | Verifies a light object is a rectangular light. |
| `IsRectangularLight` | `geometryObject: GeometryBase -> bool` | Verifies a light object is a rectangular light. |
| `IsSpotLight` | `objectId: Guid -> bool` | Verifies a light object is a spot light. |
| `IsSpotLight` | `geometryObject: GeometryBase -> bool` | Verifies a light object is a spot light. |
| `LightColor` | `objectId: Guid -> Color` | Returns the color of a light. |
| `LightColor` | `objectId: Guid * color: Color -> unit` | Changes the color of a light. |
| `LightColor` | `objectIds: Guid seq * color: Color -> unit` | Changes the color of multiple lights. |
| `LightColor` | `geometryObject: Light -> Color` | Returns the color of a light. |
| `LightCount` | `unit -> int` | Returns the number of light objects in the document. |
| `LightDirection` | `objectId: Guid -> Vector3d` | Returns the direction of a light object. |
| `LightDirection` | `objectId: Guid * direction: Vector3d -> unit` | Changes the direction of a light object. |
| `LightDirection` | `objectIds: Guid seq * direction: Vector3d -> unit` | Changes the direction of multiple light objects. |
| `LightDirection` | `geometryObject: Light -> Vector3d` | Returns the direction of a light object. |
| `LightLocation` | `objectId: Guid -> Point3d` | Returns the location of a light object. |
| `LightLocation` | `objectId: Guid * location: Point3d -> unit` | Changes the location of a light object. |
| `LightLocation` | `objectIds: Guid seq * location: Point3d -> unit` | Changes the location of multiple light objects. |
| `LightLocation` | `geometryObject: Light -> Point3d` | Returns the location of a light object. |
| `LightName` | `objectId: Guid -> string` | Returns the name of a light object. |
| `LightName` | `objectId: Guid * name: string -> unit` | Changes the name of a light object. |
| `LightName` | `objectIds: Guid seq * name: string -> unit` | Changes the name of multiple light objects. |
| `LightName` | `geometryObject: Light -> string` | Returns the name of a light object. |
| `LightObjects` | `unit -> ResizeArray<Guid>` | Returns list of identifiers of light objects in the document. |
| `RectangularLightPlane` | `objectId: Guid -> Plane * float * float` | Returns the Plane of a rectangular light object. |
| `RectangularLightPlane` | `geometryObject: Light -> Plane * float * float` | Returns the Plane of a rectangular light object. |
| `SpotLightHardness` | `objectId: Guid -> float` | Returns the hardness of a spot light. |
| `SpotLightHardness` | `objectId: Guid * hardness: float -> unit` | Changes the hardness of a spot light. |
| `SpotLightHardness` | `objectIds: Guid seq * hardness: float -> unit` | Changes the hardness of multiple spot lights. |
| `SpotLightHardness` | `geometryObject: Light -> float` | Returns the hardness of a spot light. |
| `SpotLightRadius` | `objectId: Guid -> float` | Returns the radius of a spot light. |
| `SpotLightRadius` | `objectId: Guid * radius: float -> unit` | Changes the radius of a spot light. |
| `SpotLightRadius` | `objectIds: Guid seq * radius: float -> unit` | Changes the radius of multiple spot lights. |
| `SpotLightRadius` | `geometryObject: Light -> float` | Returns the radius of a spot light. |
| `SpotLightShadowIntensity` | `objectId: Guid -> float` | Returns the shadow intensity of a spot light. |
| `SpotLightShadowIntensity` | `objectId: Guid * intensity: float -> unit` | Changes the shadow intensity of a spot light. |
| `SpotLightShadowIntensity` | `objectIds: Guid seq * intensity: float -> unit` | Changes the shadow intensity of multiple spot lights. |
| `SpotLightShadowIntensity` | `geometryObject: Light -> float` | Returns the shadow intensity of a spot light. |

## Line

From [Src/Scripting_Line.fs](../Src/Scripting_Line.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `LineClosestPoint` | `line: Line * testPoint: Point3d -> Point3d` | Finds the point on an infinite line (ray) that is closest to a test point. |
| `LineClosestPointFinite` | `line: Line * testPoint: Point3d -> Point3d` | Finds the point on a finite line that is closest to a test point. |
| `LineCylinderIntersection` | `line: Line * cylinderPlane: Plane * cylinderHeight: float * cylinderRadius: float -> Point3d array` | Calculates the intersection of a line and a cylinder. |
| `LineIsFartherThan` | `line: Line * distance: float * point: Point3d -> bool` | Determines if the shortest distance from a line to a point or another line is greater than a specified distance. |
| `LineIsFartherThan` | `line: Line * distance: float * line2: Line -> bool` | Determines if the shortest distance from a line to a point or another line is greater than a specified distance. |
| `LineLineIntersection` | `lineA: Line * lineB: Line -> Point3d * Point3d` | Calculates the intersection of two non-parallel lines. |
| `LineMaxDistanceTo` | `line: Line * point: Point3d -> float` | Finds the longest distance between a line as a finite chord, and a point. |
| `LineMaxDistanceTo` | `line: Line * line2: Line -> float` | Finds the longest distance between a line as a finite chord, and a line. |
| `LineMinDistanceTo` | `line: Line * point: Point3d -> float` | Finds the shortest distance between a line as a finite chord, and a point or another line. |
| `LineMinDistanceTo` | `line: Line * line2: Line -> float` | Finds the shortest distance between a line as a finite chord, and a point or another line. |
| `LinePlane` | `line: Line -> Plane` | Returns a Plane that contains the line. |
| `LinePlaneIntersection` | `line: Line * plane: Plane -> Point3d` | Calculates the intersection of a line and a Plane. |
| `LineSphereIntersection` | `line: Line * sphereCenter: Point3d * sphereRadius: float -> Point3d array` | Calculates the intersection of a line and a sphere. |
| `LineTransform` | `lineId: Guid * xForm: Transform * ?copy: bool = false -> Guid` | Transforms a line. |
| `LineTransform` | `line: Line * xForm: Transform -> Line` | Transforms a line geometry and returns the transformed line. |

## Linetype

From [Src/Scripting_Linetype.fs](../Src/Scripting_Linetype.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `IsLinetype` | `name: string -> bool` | Verifies the existence of a linetype in the document. |
| `IsLinetypeReference` | `name: string -> bool` | Checks if an existing linetype is from a reference file. |
| `LinetypeCount` | `unit -> int` | Returns the number of linetypes in the document. |
| `LinetypeNames` | `unit -> ResizeArray<string>` | Returns names of all linetypes in the document. |
| `LinetypeNames` | `sort: bool -> ResizeArray<string>` | Returns the names, optionally sorted in ordinal order, like in RhinoPython. |

## Material

From [Src/Scripting_Material.fs](../Src/Scripting_Material.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddMaterialToLayer` | `layer: string -> int` | Adds a material to a layer and returns the new material's index. |
| `AddMaterialToObject` | `objectId: Guid -> int` | Adds a material to an object and returns the new material's index. |
| `CopyMaterial` | `sourceIndex: int * destinationIndex: int -> bool` | Copies the definition of a source material to a destination material. |
| `IsMaterialDefault` | `materialIndex: int -> bool` | Verifies a material is a copy of Rhino's built-in "default" material. |
| `IsMaterialReference` | `materialIndex: int -> bool` | Verifies a material is referenced from another file. |
| `MatchMaterial` | `source: Guid * destination: Guid seq -> unit` | Copies the material definition from one material to one or more objects. |
| `MaterialBump` | `materialIndex: int -> string` | Returns a material's bump bitmap filename. |
| `MaterialBump` | `materialIndex: int * filename: string -> unit` | Modifies a material's bump bitmap filename. |
| `MaterialColor` | `materialIndex: int -> Color` | Returns a material's diffuse color. |
| `MaterialColor` | `materialIndex: int * color: Color -> unit` | Modifies a material's diffuse color. |
| `MaterialEnvironmentMap` | `materialIndex: int -> string` | Returns a material's environment bitmap filename. |
| `MaterialEnvironmentMap` | `materialIndex: int * filename: string -> unit` | Modifies a material's environment bitmap filename. |
| `MaterialName` | `materialIndex: int -> string` | Returns a material's user defined name. |
| `MaterialName` | `materialIndex: int * name: string -> unit` | Modifies a material's user defined name. |
| `MaterialReflectiveColor` | `materialIndex: int -> Color` | Returns a material's reflective color. |
| `MaterialReflectiveColor` | `materialIndex: int * color: Color -> unit` | Modifies a material's reflective color. |
| `MaterialShine` | `materialIndex: int -> float` | Returns a material's shine value. |
| `MaterialShine` | `materialIndex: int * shine: float -> unit` | Modifies a material's shine value. |
| `MaterialTexture` | `materialIndex: int -> string` | Returns a material's texture bitmap filename. |
| `MaterialTexture` | `materialIndex: int * filename: string -> unit` | Modifies a material's texture bitmap filename. |
| `MaterialTransparency` | `materialIndex: int -> float` | Returns a material's transparency value. |
| `MaterialTransparency` | `materialIndex: int * transparency: float -> unit` | Modifies a material's transparency value. |
| `MaterialTransparencyMap` | `materialIndex: int -> string` | Returns a material's transparency bitmap filename. |
| `MaterialTransparencyMap` | `materialIndex: int * filename: string -> unit` | Modifies a material's transparency bitmap filename. |
| `ResetMaterial` | `materialIndex: int -> bool` | Resets a material to Rhino's default material. |

## Mesh

From [Src/Scripting_Mesh.fs](../Src/Scripting_Mesh.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddMesh` | `vertices: Point3d seq * faceVertices: 'a seq * ?vertexNormals: Vector3f seq = null * ?textureCoordinates: Point2f seq = null * ?vertexColors: Color seq = null -> Guid` | Adds a Mesh object to the document. |
| `AddMesh` | `vertices: Point3d seq * faceVertices: (int * int * int * int) seq * ?vertexNormals: Vector3f seq = null * ?textureCoordinates: Point2f seq = null * ?vertexColors: Color seq = null -> Guid` | Adds a Mesh object to the document. |
| `AddMeshQuad` | `pointA: Point3d * pointB: Point3d * pointC: Point3d * pointD: Point3d -> Guid` | Creates a new Mesh with just one quad face . |
| `AddMeshTriangle` | `pointA: Point3d * pointB: Point3d * pointC: Point3d -> Guid` | Creates a new Mesh with just one triangle face . |
| `AddPlanarMesh` | `objectId: Guid * ?deleteInput: bool = false -> Guid` | Creates a planar Mesh from a closed, planar Curve. |
| `MakePlanarMesh` | `curve: Curve -> Mesh` | Creates a planar Mesh from a closed, planar Curve. |
| `CurveMeshIntersection` | `curveId: Guid * meshId: Guid -> Point3d array * int array` | Calculates the intersection of a Curve object and a Mesh object. |
| `CurveMeshIntersection` | `curve: Curve * mesh: Mesh -> Point3d array * int array` | Calculates the intersection of a Curve object and a Mesh object. |
| `DisjointMeshCount` | `objectId: Guid -> int` | Returns number of Meshes that could be created by calling SplitDisjointMesh. |
| `DisjointMeshCount` | `mesh: Mesh -> int` | Returns number of Meshes that could be created by calling SplitDisjointMesh. |
| `DuplicateMeshBorder` | `meshId: Guid -> ResizeArray<Guid>` | Creates Curves that duplicates a Mesh border. |
| `DuplicateMeshBorder` | `mesh: Mesh -> ResizeArray<PolylineCurve>` | Duplicates the naked borders of a Mesh as Polyline Curves. |
| `ExplodeMeshes` | `meshIds: Guid seq * ?delete: bool = false -> ResizeArray<Guid>` | Explodes a Mesh object, or Mesh objects int subMeshes. |
| `ExplodeMeshes` | `meshes: Mesh seq -> ResizeArray<Mesh>` | Splits Meshes into submeshes along unwelded edges. |
| `IsMesh` | `objectId: Guid -> bool` | Verifies if an object is a Mesh. |
| `IsMesh` | `geometry: GeometryBase -> bool` | Verifies if an object is a Mesh. |
| `IsMeshClosed` | `objectId: Guid -> bool` | Verifies a Mesh object is closed. |
| `IsMeshClosed` | `mesh: Mesh -> bool` | Verifies a Mesh object is closed. |
| `IsMeshManifold` | `objectId: Guid -> bool` | Verifies a Mesh object is manifold. |
| `IsMeshManifold` | `mesh: Mesh -> bool` | Verifies a Mesh object is manifold. |
| `IsPointOnMesh` | `objectId: Guid * point: Point3d * ?tolerance: float = 0.0 -> bool` | Verifies a point is on a Mesh. |
| `IsPointOnMesh` | `mesh: Mesh * point: Point3d * ?tolerance: float = 0.0 -> bool` | Verifies a point is on a Mesh. |
| `JoinMeshes` | `meshes: Mesh seq -> Mesh` | Joins two or or more Mesh objects together. |
| `JoinMeshes` | `objectIds: Guid seq * ?deleteInput: bool = false -> Guid` | Joins two or or more Mesh objects together. |
| `MeshArea` | `objectId: Guid -> float` | Returns approximate area of one mesh object. |
| `MeshArea` | `mesh: Mesh -> float` | Returns approximate area of one mesh object. |
| `MeshAreaCentroid` | `objectId: Guid -> Point3d` | Calculates the area centroid of a Mesh object. |
| `MeshAreaCentroid` | `mesh: Mesh -> Point3d` | Calculates the area centroid of a Mesh object. |
| `MeshBooleanDifference` | `input0: Guid seq * input1: Guid seq * ?deleteInput: bool = true -> ResizeArray<Guid>` | Performs boolean difference operation on two sets of input Meshes. |
| `MeshBooleanDifference` | `input0: Mesh seq * input1: Mesh seq -> ResizeArray<Mesh>` | Performs a boolean difference on two sets of Meshes. |
| `MeshBooleanIntersection` | `input0: Guid seq * input1: Guid seq * ?deleteInput: bool = true -> ResizeArray<Guid>` | Performs boolean intersection operation on two sets of input Meshes. |
| `MeshBooleanIntersection` | `input0: Mesh seq * input1: Mesh seq -> ResizeArray<Mesh>` | Performs a boolean intersection on two sets of Meshes. |
| `MeshBooleanSplit` | `input0: Guid seq * input1: Guid seq * ?deleteInput: bool = true -> ResizeArray<Guid>` | Performs boolean split operation on two sets of input Meshes. |
| `MeshBooleanSplit` | `input0: Mesh seq * input1: Mesh seq -> ResizeArray<Mesh>` | Performs a boolean split on two sets of Meshes. |
| `MeshBooleanUnion` | `meshIds: Guid seq * ?deleteInput: bool = true -> ResizeArray<Guid>` | Performs boolean union operation on a set of input Meshes. |
| `MeshBooleanUnion` | `meshes: Mesh seq -> ResizeArray<Mesh>` | Performs a boolean union on a set of Meshes. |
| `MeshClosestPoint` | `objectId: Guid * point: Point3d * ?maximumDistance: float = 0.0 -> Point3d * int` | Returns the point on a Mesh that is closest to a test point. |
| `MeshClosestPoint` | `mesh: Mesh * point: Point3d * ?maximumDistance: float = 0.0 -> Point3d * int` | Returns the point on a Mesh that is closest to a test point. |
| `MeshFaceCenters` | `meshId: Guid -> ResizeArray<Point3d>` | Returns the center of each face of the Mesh object. |
| `MeshFaceCenters` | `mesh: Mesh -> ResizeArray<Point3d>` | Returns the center of each face of the Mesh object. |
| `MeshFaceCount` | `objectId: Guid -> int` | Returns total face count of a Mesh object. |
| `MeshFaceCount` | `mesh: Mesh -> int` | Returns total face count of a Mesh object. |
| `MeshFaceNormals` | `meshId: Guid -> ResizeArray<Vector3d>` | Returns the face unit normal for each face of a Mesh object. |
| `MeshFaceNormals` | `mesh: Mesh -> ResizeArray<Vector3d>` | Returns the face unit normal for each face of a Mesh object. |
| `MeshFacePoints` | `objectId: Guid -> ResizeArray<Point3d * Point3d * Point3d * Point3d>` | Returns vertices of each face in a Mesh as tuple of 4 points. |
| `MeshFacePoints` | `mesh: Mesh -> ResizeArray<Point3d * Point3d * Point3d * Point3d>` | Returns vertices of each face in a Mesh as tuple of 4 points. |
| `MeshFaces` | `objectId: Guid * ?faceType: bool = true -> ResizeArray<Point3d>` | Returns face vertices of a Mesh. |
| `MeshFaces` | `mesh: Mesh * ?faceType: bool = true -> ResizeArray<Point3d>` | Returns face vertices of a Mesh. |
| `MeshFaceVertices` | `objectId: Guid -> ResizeArray<int * int * int * int>` | Returns the vertex indices of all faces of a Mesh object, Does not suport Ngons yet. |
| `MeshFaceVertices` | `mesh: Mesh -> ResizeArray<int * int * int * int>` | Returns the vertex indices of all faces of a Mesh object, Does not suport Ngons yet. |
| `MeshHasFaceNormals` | `objectId: Guid -> bool` | Verifies a Mesh object has face normals. |
| `MeshHasFaceNormals` | `mesh: Mesh -> bool` | Verifies a Mesh object has face normals. |
| `MeshHasTextureCoordinates` | `objectId: Guid -> bool` | Verifies a Mesh object has texture coordinates. |
| `MeshHasTextureCoordinates` | `mesh: Mesh -> bool` | Verifies a Mesh object has texture coordinates. |
| `MeshHasVertexColors` | `objectId: Guid -> bool` | Verifies a Mesh object has vertex colors. |
| `MeshHasVertexColors` | `mesh: Mesh -> bool` | Verifies a Mesh object has vertex colors. |
| `MeshHasVertexNormals` | `objectId: Guid -> bool` | Verifies a Mesh object has vertex normals. |
| `MeshHasVertexNormals` | `mesh: Mesh -> bool` | Verifies a Mesh object has vertex normals. |
| `MeshMeshIntersection` | `mesh1: Guid * mesh2: Guid * ?tolerance: float = 0.0 -> Polyline array` | Calculates the intersections of a Mesh object with another Mesh object. |
| `MeshMeshIntersection` | `mesh1: Mesh * mesh2: Mesh * ?tolerance: float = 0.0 -> Polyline array` | Calculates the intersections of a Mesh object with another Mesh object. |
| `MeshNakedEdgePoints` | `objectId: Guid -> bool array` | Identifies the naked edge points of a Mesh object. |
| `MeshNakedEdgePoints` | `mesh: Mesh -> bool array` | Identifies the naked edge points of a Mesh object. |
| `MeshNgonFaceVertices` | `objectId: Guid -> ResizeArray<ResizeArray<int>>` | Returns the vertex indices of all faces of a Ngon Mesh object. |
| `MeshNgonFaceVertices` | `mesh: Mesh -> ResizeArray<ResizeArray<int>>` | Returns the vertex indices of all faces of a Ngon Mesh object. |
| `MeshOffset` | `meshId: Guid * distance: float -> Guid` | Makes a new Mesh with vertices offset at a distance in the opposite direction of the existing vertex normals. |
| `MeshOffset` | `mesh: Mesh * distance: float -> Mesh` | Creates an offset copy of a Mesh. |
| `MeshOutline` | `objectIds: Guid seq * ?view: string = null -> ResizeArray<Guid>` | Creates Polyline Curve outlines of Mesh objects. |
| `MeshOutline` | `meshes: Mesh seq * ?view: RhinoViewport = null -> ResizeArray<PolylineCurve>` | Creates Polyline Curve outlines of Meshes. |
| `MeshQuadCount` | `objectId: Guid -> int` | Returns the number of quad faces of a Mesh object. |
| `MeshQuadCount` | `mesh: Mesh -> int` | Returns the number of quad faces of a Mesh object. |
| `MeshQuadsToTriangles` | `objectId: Guid -> bool` | Converts a Mesh object's quad faces to triangles. |
| `MeshTextureCoordinates` | `objectId: Guid -> ResizeArray<Point2f>` | Returns the texture coordinates of a Mesh. |
| `MeshTextureCoordinates` | `mesh: Mesh -> ResizeArray<Point2f>` | Returns the texture coordinates of a Mesh. |
| `MeshToNurb` | `objectId: Guid * ?trimmedTriangles: bool = true * ?deleteInput: bool = false -> ResizeArray<Guid>` | Duplicates each polygon in a Mesh with a NURBS Surface. |
| `MeshToNurb` | `mesh: Mesh * ?trimmedTriangles: bool = true -> ResizeArray<Brep>` | Duplicates each Mesh polygon with a NURBS Surface and joins the Surfaces for each connected piece. |
| `MeshTriangleCount` | `objectId: Guid -> int` | Returns number of triangular faces of a Mesh. |
| `MeshTriangleCount` | `mesh: Mesh -> int` | Returns number of triangular faces of a Mesh. |
| `MeshVertexColors` | `meshId: Guid -> ResizeArray<Color>` | Returns vertex colors of a Mesh. |
| `MeshVertexColors` | `mesh: Mesh -> ResizeArray<Color>` | Returns vertex colors of a Mesh. |
| `MeshVertexColors` | `meshId: Guid * colors: Color seq -> unit` | Modifies vertex colors of a Mesh. |
| `MeshVertexCount` | `objectId: Guid -> int` | Returns the vertex count of a Mesh. |
| `MeshVertexCount` | `mesh: Mesh -> int` | Returns the vertex count of a Mesh. |
| `MeshVertexFaces` | `meshId: Guid * vertexIndex: int -> int array` | Returns the Mesh faces that share a specified Mesh vertex. |
| `MeshVertexFaces` | `mesh: Mesh * vertexIndex: int -> int array` | Returns the Mesh faces that share a specified Mesh vertex. |
| `MeshVertexNormals` | `meshId: Guid -> ResizeArray<Vector3d>` | Returns the vertex unit normal for each vertex of a Mesh. |
| `MeshVertexNormals` | `mesh: Mesh -> ResizeArray<Vector3d>` | Returns the vertex unit normal for each vertex of a Mesh. |
| `MeshVertices` | `objectId: Guid -> ResizeArray<Point3d>` | Returns the vertices of a Mesh. |
| `MeshVertices` | `mesh: Mesh -> ResizeArray<Point3d>` | Returns the vertices of a Mesh. |
| `MeshVolume` | `objectIds: Guid seq -> float` | Returns the approximate volume of one or more closed Meshes. |
| `MeshVolume` | `meshes: Mesh seq -> float` | Returns the approximate volume of one or more closed Meshes. |
| `MeshVolumeCentroid` | `objectId: Guid -> Point3d` | Calculates the volume centroid of a Mesh. |
| `MeshVolumeCentroid` | `mesh: Mesh -> Point3d` | Calculates the volume centroid of a Mesh. |
| `PullCurveToMesh` | `meshId: Guid * curveId: Guid -> Guid` | Pulls a Curve to a Mesh. |
| `PullCurveToMesh` | `mesh: Mesh * curve: Curve -> PolylineCurve` | Pulls a polyline approximation of a Curve onto a Mesh. |
| `SplitDisjointMesh` | `objectId: Guid * ?deleteInput: bool = false -> ResizeArray<Guid>` | Splits up a Mesh into its unconnected pieces. |
| `SplitDisjointMesh` | `mesh: Mesh -> ResizeArray<Mesh>` | Splits a Mesh into its unconnected pieces. |
| `UnifyMeshNormals` | `objectId: Guid -> int` | Fixes inconsistencies in the directions of faces of a Mesh. |

## Object

From [Src/Scripting_Object.fs](../Src/Scripting_Object.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `CopyObject` | `objectId: Guid * ?translation: Vector3d = Vector3d() -> Guid` | Copies an object from one location to another, or in-place. |
| `CopyObject` | `geometry: GeometryBase * ?translation: Vector3d = Vector3d() -> GeometryBase` | Returns a transformed duplicate of the input geometry. |
| `CopyObjects` | `objectIds: Guid seq * ?translation: Vector3d = Vector3d() -> ResizeArray<Guid>` | Copies one or more objects from one location to another, or in-place. |
| `CopyObjects` | `geometries: GeometryBase seq * ?translation: Vector3d = Vector3d() -> ResizeArray<GeometryBase>` | Returns transformed duplicates of the input geometries. |
| `DeleteObject` | `objectId: Guid -> unit` | Deletes a single object from the document. |
| `DeleteObjects` | `objectIds: Guid seq -> unit` | Deletes one or more objects from the document, Fails if not all objects can be deleted. |
| `FlashObject` | `objectIds: Guid seq * ?style: bool = true -> unit` | Causes the selection state of one or more objects to change momentarily so the object appears to flash on the screen. |
| `HideObject` | `objectId: Guid -> bool` | Hides a single object. |
| `HideObjects` | `objectIds: Guid seq -> int` | Hides one or more objects. |
| `IsGoodStringId` | `name: string * ?allowEmpty: bool = false * ?limitToAscii: bool = false -> bool` | Checks if a string is a good string for use in Rhino Object Names or User Dictionary key as and values. |
| `IsLayoutObject` | `objectId: Guid -> bool` | Checks if an object is in on a page layout space. |
| `IsObject` | `objectId: Guid -> bool` | Verifies the existence of an object in the State.Doc.Objects table. |
| `IsObjectHidden` | `objectId: Guid -> bool` | Checks if an object is hidden. |
| `IsObjectInBox` | `objectId: Guid * box: BoundingBox * ?testMode: bool = true -> bool` | Verifies an object's bounding box is inside of another bounding box. |
| `IsObjectInBox` | `geometryObject: GeometryBase * box: BoundingBox * ?testMode: bool = true -> bool` | Verifies an object's bounding box is inside of another bounding box. |
| `IsObjectInGroup` | `objectId: Guid * ?groupName: string = null -> bool` | Checks if an object is a member of a group. |
| `IsObjectLocked` | `objectId: Guid -> bool` | Checks if an object is locked. |
| `IsObjectNormal` | `objectId: Guid -> bool` | Checks if an object is normal. |
| `IsObjectReference` | `objectId: Guid -> bool` | Checks if an object is a reference object. |
| `IsObjectSelectable` | `objectId: Guid -> bool` | Checks if an object can be selected. |
| `IsObjectSelected` | `objectId: Guid -> int` | Checks if an object is currently selected. |
| `IsObjectSolid` | `objectId: Guid -> bool` | Determines if an object is closed or solid. |
| `IsObjectSolid` | `geometryObject: GeometryBase -> bool` | Determines if an object is closed or solid. |
| `IsObjectValid` | `objectId: Guid -> bool` | Verifies an object's geometry is valid and without error. |
| `IsVisibleInView` | `objectId: Guid * ?view: string = null -> bool` | Verifies an object is visible in a view. |
| `LockObject` | `objectId: Guid -> bool` | Locks a single object. |
| `LockObjects` | `objectIds: Guid seq -> int` | Locks multiple objects. |
| `MatchObjectAttributes` | `targetIds: Guid seq * ?sourceId: Guid = Guid() -> int` | Matches, or copies the attributes of a source object to a target object. |
| `MirrorObject` | `objectId: Guid * startPoint: Point3d * endPoint: Point3d * ?copy: bool = false -> Guid` | Mirrors a single object on World XY Plane. |
| `MirrorObject` | `geometry: GeometryBase * startPoint: Point3d * endPoint: Point3d -> GeometryBase` | Returns a transformed duplicate of the input geometry. |
| `MirrorObjects` | `objectIds: Guid seq * startPoint: Point3d * endPoint: Point3d * ?copy: bool = false -> ResizeArray<Guid>` | Mirrors a list of objects on World XY Plane. |
| `MirrorObjects` | `geometries: GeometryBase seq * startPoint: Point3d * endPoint: Point3d -> ResizeArray<GeometryBase>` | Returns transformed duplicates of the input geometries. |
| `MoveObject` | `objectId: Guid * translation: Vector3d -> unit` | Moves a single object. |
| `MoveObjects` | `objectIds: Guid seq * translation: Vector3d -> unit` | Moves one or more objects. |
| `ObjectColor` | `objectId: Guid -> Color` | Returns the color of an object. |
| `ObjectColor` | `objectId: Guid * color: Color -> unit` | Modifies the color of an object. |
| `ObjectColor` | `objectIds: Guid seq * color: Color -> unit` | Modifies the color of multiple objects. |
| `ObjectColorSource` | `objectId: Guid -> int` | Returns the color source of an object. |
| `ObjectColorSource` | `objectId: Guid * source: int -> unit` | Modifies the color source of an object. |
| `ObjectColorSource` | `objectIds: Guid seq * source: int -> unit` | Modifies the color source of multiple objects. |
| `ObjectDescription` | `objectId: Guid -> string` | Returns a description of the object type (e.g. Line, Surface, Text,...). |
| `ObjectDescription` | `objectIds: Guid seq -> string` | Returns the count for each object type in a List of objects. |
| `ObjectLayerShort` | `objectId: Guid -> string` | Returns the short layer of an object. |
| `ObjectLayout` | `objectId: Guid -> string` | Returns the layout or model space of an object. |
| `ObjectLayout` | `objectId: Guid * layout: string -> unit` | Changes the layout or model space of an object. |
| `ObjectLayout` | `objectIds: Guid seq * layout: string -> unit` | Changes the layout or model space of an objects. |
| `ObjectLinetype` | `objectId: Guid -> string` | Returns the linetype of an object. |
| `ObjectLinetype` | `objectId: Guid * linetype: string -> unit` | Modifies the linetype of an object. |
| `ObjectLinetype` | `objectIds: Guid seq * linetype: string -> unit` | Modifies the linetype of multiple object. |
| `ObjectLinetypeSource` | `objectId: Guid -> int` | Returns the linetype source of an object. |
| `ObjectLinetypeSource` | `objectId: Guid * source: int -> unit` | Modifies the linetype source of an object. |
| `ObjectLinetypeSource` | `objectIds: Guid seq * source: int -> unit` | Modifies the linetype source of multiple objects. |
| `ObjectMaterialIndex` | `objectId: Guid -> int` | Returns the material index of an object. |
| `ObjectMaterialIndex` | `objectId: Guid * materialIndex: int -> unit` | Changes the material index of an object. |
| `ObjectMaterialIndex` | `objectIds: Guid seq * materialIndex: int -> unit` | Changes the material index multiple objects. |
| `ObjectMaterialSource` | `objectId: Guid -> int` | Returns the rendering material source of an object. |
| `ObjectMaterialSource` | `objectId: Guid * source: int -> unit` | Modifies the rendering material source of an object. |
| `ObjectMaterialSource` | `objectIds: Guid seq * source: int -> unit` | Modifies the rendering material source of multiple objects. |
| `ObjectName` | `objectId: Guid -> string` | Returns the name of an object or "" if none given. |
| `ObjectName` | `objectId: Guid * name: string -> unit` | Modifies the name of an object. |
| `ObjectName` | `objectIds: Guid seq * name: string -> unit` | Modifies the name of multiple objects. |
| `ObjectPrintColor` | `objectId: Guid -> Color` | Returns the print color of an object. |
| `ObjectPrintColor` | `objectId: Guid * color: Color -> unit` | Modifies the print color of an object. |
| `ObjectPrintColor` | `objectIds: Guid seq * color: Color -> unit` | Modifies the print color of multiple objects. |
| `ObjectPrintColorSource` | `objectId: Guid -> int` | Returns the print color source of an object. |
| `ObjectPrintColorSource` | `objectId: Guid * source: int -> unit` | Modifies the print color source of an object. |
| `ObjectPrintColorSource` | `objectIds: Guid seq * source: int -> unit` | Modifies the print color source of multiple objects. |
| `ObjectPrintWidth` | `objectId: Guid -> float` | Returns the print width of an object. |
| `ObjectPrintWidth` | `objectId: Guid * width: float -> unit` | Modifies the print width of an object. |
| `ObjectPrintWidth` | `objectIds: Guid seq * width: float -> unit` | Modifies the print width of multiple objects. |
| `ObjectPrintWidthSource` | `objectId: Guid -> int` | Returns the print width source of an object. |
| `ObjectPrintWidthSource` | `objectId: Guid * source: int -> unit` | Modifies the print width source of an object. |
| `ObjectPrintWidthSource` | `objectIds: Guid seq * source: int -> unit` | Modifies the print width source of multiple objects. |
| `ObjectType` | `objectId: Guid -> int` | Returns the object type. |
| `OrientObject` | `objectId: Guid * referencePts: IList<Point3d> * targetPts: IList<Point3d> * ?flags: int = 0 -> Guid` | Orients a single object based on input points. |
| `OrientObject` | `geometry: GeometryBase * referencePts: IList<Point3d> * targetPts: IList<Point3d> * ?scale: bool = false -> GeometryBase` | Returns a transformed duplicate of the input geometry. |
| `RotateObject` | `objectId: Guid * centerPoint: Point3d * rotationAngle: float * ?axis: Vector3d = Vector3d() * ?copy: bool = false -> Guid` | Rotates a single object. |
| `RotateObject` | `geometry: GeometryBase * centerPoint: Point3d * rotationAngle: float * ?axis: Vector3d = Vector3d() -> GeometryBase` | Returns a transformed duplicate of the input geometry. |
| `RotateObjects` | `objectIds: Guid seq * centerPoint: Point3d * rotationAngle: float * ?axis: Vector3d = Vector3d() * ?copy: bool = false -> ResizeArray<Guid>` | Rotates multiple objects. |
| `RotateObjects` | `geometries: GeometryBase seq * centerPoint: Point3d * rotationAngle: float * ?axis: Vector3d = Vector3d() -> ResizeArray<GeometryBase>` | Returns transformed duplicates of the input geometries. |
| `ScaleObject` | `objectId: Guid * origin: Point3d * scale: (float * float * float) * ?copy: bool = false -> Guid` | Scales a single object. |
| `ScaleObject` | `geometry: GeometryBase * origin: Point3d * scale: (float * float * float) -> GeometryBase` | Returns a transformed duplicate of the input geometry. |
| `ScaleObject` | `objectId: Guid * origin: Point3d * scale: float * ?copy: bool = false -> Guid` | Scales a single object. |
| `ScaleObject` | `geometry: GeometryBase * origin: Point3d * scale: float -> GeometryBase` | Returns a transformed duplicate of the input geometry. |
| `ScaleObjects` | `objectIds: Guid seq * origin: Point3d * scale: (float * float * float) * ?copy: bool = false -> ResizeArray<Guid>` | Scales one or more objects. |
| `ScaleObjects` | `geometries: GeometryBase seq * origin: Point3d * scale: (float * float * float) -> ResizeArray<GeometryBase>` | Returns transformed duplicates of the input geometries. |
| `ScaleObjects` | `objectIds: Guid seq * origin: Point3d * scale: float * ?copy: bool = false -> ResizeArray<Guid>` | Scales one or more objects. |
| `ScaleObjects` | `geometries: GeometryBase seq * origin: Point3d * scale: float -> ResizeArray<GeometryBase>` | Returns transformed duplicates of the input geometries. |
| `SelectObject` | `objectId: Guid * ?forceVisible: bool = false * ?ignoreErrors: bool = false -> unit` | Selects a single object. |
| `SelectObjects` | `objectIds: Guid seq * ?forceVisible: bool = false * ?ignoreErrors: bool = false -> unit` | Selects one or more objects Throws an exception if object can't be selected for some reason e.g. when locked , hidden, or on invisible layer . |
| `ShearObject` | `objectId: Guid * origin: Point3d * referencePoint: Point3d * angleDegrees: float * ?copy: bool = false -> Guid` | Perform a shear transformation on a single object. |
| `ShearObject` | `geometry: GeometryBase * origin: Point3d * referencePoint: Point3d * angleDegrees: float -> GeometryBase` | Returns a transformed duplicate of the input geometry. |
| `ShearObjects` | `objectIds: Guid seq * origin: Point3d * referencePoint: Point3d * angleDegrees: float * ?copy: bool = false -> ResizeArray<Guid>` | Shears one or more objects. |
| `ShearObjects` | `geometries: GeometryBase seq * origin: Point3d * referencePoint: Point3d * angleDegrees: float -> ResizeArray<GeometryBase>` | Returns transformed duplicates of the input geometries. |
| `ShowObject` | `objectId: Guid -> unit` | Shows a previously hidden object. |
| `ShowObjects` | `objectIds: Guid seq -> unit` | Shows one or more objects. |
| `TransformObject` | `objectId: Guid * matrix: Transform * ?copy: bool = false -> Guid` | Moves, scales, or rotates an object given a 4x4 transformation matrix. |
| `TransformObject` | `geometry: GeometryBase * matrix: Transform -> GeometryBase` | Returns a transformed duplicate of the input geometry. |
| `TransformObjects` | `objectIds: Guid seq * matrix: Transform * ?copy: bool = false -> ResizeArray<Guid>` | Moves, scales, or rotates a list of objects given a 4x4 transformation matrix. |
| `TransformObjects` | `geometries: GeometryBase seq * matrix: Transform -> ResizeArray<GeometryBase>` | Returns transformed duplicates of the input geometries. |
| `UnlockObject` | `objectId: Guid -> unit` | Unlocks an object. |
| `UnlockObjects` | `objectIds: Guid seq -> unit` | Unlocks one or more objects. |
| `UnselectObject` | `objectId: Guid -> unit` | Unselects a single selected object. |
| `UnselectObjects` | `objectIds: Guid seq -> unit` | Unselects multiple selected objects. |

## Plane

From [Src/Scripting_Plane.fs](../Src/Scripting_Plane.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `DistanceToPlane` | `plane: Plane * point: Point3d -> float` | Returns the distance from a 3D point to a plane. |
| `EvaluatePlane` | `plane: Plane * u: float * v: float -> Point3d` | Evaluates a plane at a U, V parameter. |
| `FlipPlane` | `plane: Plane -> Plane` | Flips this plane by swapping out the X and Y axes and inverting the Z axis. |
| `IntersectPlanes` | `plane1: Plane * plane2: Plane * plane3: Plane -> Point3d` | Calculates the intersection of three planes. |
| `MovePlane` | `plane: Plane * origin: Point3d -> Plane` | Moves the origin of a plane. |
| `PlaneClosestParameter` | `plane: Plane * point: Point3d -> float * float` | Returns the point on a Plane that is closest to a test point. |
| `PlaneClosestPoint` | `plane: Plane * point: Point3d -> Point3d` | Returns the point on a Plane that is closest to a test point. |
| `PlaneCurveIntersection` | `plane: Plane * curve: Guid * ?tolerance: float = 0.0 -> ResizeArray<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float * float * float>` | Intersect an infinite Plane and a Curve object. |
| `PlaneCurveIntersection` | `plane: Plane * curve: Curve * ?tolerance: float = 0.0 -> ResizeArray<int * Point3d * Point3d * Point3d * Point3d * float * float * float * float * float * float>` | Intersect an infinite Plane and a Curve object. |
| `PlaneEquation` | `plane: Plane -> float * float * float * float` | Returns the equation of a Plane as a tuple of four numbers. |
| `PlaneFitFromPoints` | `points: Point3d seq -> Plane` | Returns a Plane that was fit through an array of 3D points. |
| `PlaneFromFrame` | `origin: Point3d * xAxis: Vector3d * yAxis: Vector3d -> Plane` | Construct a Plane from a point, and two vectors in the Plane. |
| `PlaneFromNormal` | `origin: Point3d * normal: Vector3d * ?xAxis: Vector3d = Vector3d() -> Plane` | Creates a Plane from an origin point and a normal direction vector. |
| `PlaneFromPoints` | `origin: Point3d * x: Point3d * y: Point3d -> Plane` | Creates a Plane from three non-collinear points. |
| `PlanePlaneIntersection` | `plane1: Plane * plane2: Plane -> Line` | Calculates the intersection of two Planes. |
| `PlaneSphereIntersection` | `plane: Plane * spherePlane: Plane * sphereRadius: float -> int * Plane * float` | Calculates the intersection of a Plane and a sphere. |
| `PlaneTransform` | `plane: Plane * xForm: Transform -> Plane` | Transforms a Plane. |
| `RotatePlane` | `plane: Plane * angleDegrees: float * axis: Vector3d -> Plane` | Rotates a Plane. |
| `WorldXYPlane` | `unit -> Plane` | Returns Rhino's world XY Plane. |
| `WorldYZPlane` | `unit -> Plane` | Returns Rhino's world YZ Plane. |
| `WorldZXPlane` | `unit -> Plane` | Returns Rhino's world ZX Plane. |

## PointVector

From [Src/Scripting_PointVector.fs](../Src/Scripting_PointVector.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `IsVectorParallelTo` | `vector1: Vector3d * vector2: Vector3d * ?toleranceDegree: float = 0.0 -> int` | Compares two vectors to see if they are parallel within one degree or custom tolerance. |
| `IsVectorPerpendicularTo` | `vector1: Vector3d * vector2: Vector3d * ?toleranceDegree: float = 0.0 -> bool` | Compares two vectors to see if they are perpendicular. |
| `IsVectorTiny` | `vector: Vector3d -> bool` | Checks if a vector is very short. |
| `IsVectorZero` | `vector: Vector3d -> bool` | Checks if a vector is zero. |
| `PointAdd` | `point1: Point3d * point2: Point3d -> Point3d` | Adds a 3D point or a 3D vector to a 3D point. |
| `PointArrayBoundingBox` | `points: Point3d seq * ?plane: Plane = Plane() -> Box` | Returns either a world axis-aligned or a construction Plane axis-aligned bounding box of an array of 3-D point locations. |
| `PointArrayClosestPoint` | `points: IList<Point3d> * testPoint: Point3d -> int` | Finds the point in a list of 3D points that is closest to a test point. |
| `PointArrayTransform` | `points: IList<Point3d> * xForm: Transform -> ResizeArray<Point3d>` | Transforms a list of 3D points. |
| `PointClosestObject` | `point: Point3d * objectIds: Guid seq -> Guid * Point3d * float` | Finds the object that is closest to a test point. |
| `PointClosestObject` | `point: Point3d * geometries: GeometryBase seq -> GeometryBase * Point3d * float` | Finds the object that is closest to a test point. |
| `PointCompare` | `point1: Point3d * point2: Point3d * ?tolerance: float = 0.0 -> bool` | Compares two 3D points. |
| `PointDivide` | `point: Point3d * divide: float -> Point3d` | Divides a 3D point by a value. |
| `PointsAreCoplanar` | `points: Point3d seq * ?tolerance: float = 0.0 -> bool` | Checks if a list of 3D points are coplanar. |
| `PointScale` | `point: Point3d * scale: float -> Point3d` | Scales a 3D point by a value. |
| `PointSubtract` | `point1: Point3d * point2: Point3d -> Point3d` | Subtracts a 3D point or a 3D vector from a 3D point. |
| `PointTransform` | `point: Point3d * xForm: Transform -> Point3d` | Transforms a 3D point. |
| `ProjectPointToMesh` | `points: Point3d seq * meshIds: Guid seq * direction: Vector3d -> Point3d array` | Projects one or more points onto one or more Meshes. |
| `ProjectPointToMesh` | `points: Point3d seq * meshGeos: Mesh seq * direction: Vector3d -> Point3d array` | Projects one or more points onto one or more Meshes. |
| `ProjectPointToSurface` | `points: Point3d seq * surfaceIds: Guid seq * direction: Vector3d -> Point3d array` | Projects one or more points onto one or more Surfaces or Polysurfaces. |
| `ProjectPointToSurface` | `points: Point3d seq * surfaceGeos: Brep seq * direction: Vector3d -> Point3d array` | Projects one or more points onto one or more Surfaces or Polysurfaces. |
| `PullPoints` | `objectId: Guid * points: Point3d seq -> Point3d array` | Pulls an array of points to a Surface or Mesh object. |
| `PullPoints` | `geometryObject: GeometryBase * points: Point3d seq -> Point3d array` | Pulls an array of points to a Surface or Mesh object. |
| `VectorAdd` | `vector1: Vector3d * vector2: Vector3d -> Vector3d` | Adds two 3D vectors. |
| `VectorAngle` | `vector1: Vector3d * vector2: Vector3d -> float` | Returns the angle, in degrees, between two 3-D vectors. |
| `VectorCompare` | `vector1: Vector3d * vector2: Vector3d -> int` | Compares two 3D vectors. |
| `VectorCreate` | `fromPoint: Point3d * toPoint: Point3d -> Vector3d` | Creates a vector from two 3D points. |
| `VectorCrossProduct` | `vector1: Vector3d * vector2: Vector3d -> Vector3d` | Calculates the cross product of two 3D vectors. |
| `VectorDivide` | `vector: Vector3d * divide: float -> Vector3d` | Divides a 3D vector by a value. |
| `VectorDotProduct` | `vector1: Vector3d * vector2: Vector3d -> float` | Calculates the dot product of two 3D vectors. |
| `VectorLength` | `vector: Vector3d -> float` | Returns the length of a 3D vector. |
| `VectorMultiply` | `vector1: Vector3d * vector2: Vector3d -> float` | Multiplies two 3D vectors, same as Dot Product. |
| `VectorReverse` | `vector: Vector3d -> Vector3d` | Reverses the direction of a 3D vector. |
| `VectorRotate` | `vector: Vector3d * angleDegrees: float * axis: Vector3d -> Vector3d` | Rotates a 3D vector. |
| `VectorScale` | `vector: Vector3d * scale: float -> Vector3d` | Scales a 3-D vector. |
| `VectorSubtract` | `vector1: Vector3d * vector2: Vector3d -> Vector3d` | Subtracts two 3D vectors. |
| `VectorTransform` | `vector: Vector3d * xForm: Transform -> Vector3d` | Transforms a 3D vector. |
| `VectorUnitize` | `vector: Vector3d -> Vector3d` | Unitizes, or normalizes a 3D vector. |

## Selection

From [Src/Scripting_Selection.fs](../Src/Scripting_Selection.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AllObjects` | `?select: bool = false * ?includeLights: bool = false * ?includeGrips: bool = false * ?includeReferences: bool = false -> ResizeArray<Guid>` | Returns identifiers of all objects in the document. |
| `FirstObject` | `?select: bool = false * ?includeLights: bool = false * ?includeGrips: bool = false -> Guid` | Returns identifier of the first object in the document. |
| `GetCurveObject` | `?message: string = null * ?preselect: bool = true * ?select: bool = false -> Guid * bool * SelectionMethod * Point3d * float * string` | Prompts user to pick or select a single Curve object. |
| `GetObject` | `?message: string = null * ?filter: int = 0 * ?preselect: bool = true * ?select: bool = false * ?customFilter: GetObjectGeometryFilter = null * ?subObjects: bool = false -> Guid` | Prompts user to pick, or select, a single object. |
| `GetObjectEx` | `?message: string = null * ?filter: int = 0 * ?preselect: bool = true * ?select: bool = false * ?objects: Guid seq = null -> Guid * bool * SelectionMethod * Point3d * string` | Prompts user to pick, or select a single object. |
| `GetObjects` | `?message: string = "Select objects" * ?filter: int = 0 * ?group: bool = true * ?preselect: bool = true * ?select: bool = false * ?objectsToSelectFrom: Guid seq = null * ?minimumCount: int = 1 * ?maximumCount: int = 0 * ?printCount: bool = false * ?customFilter: GetObjectGeometryFilter = null -> ResizeArray<Guid>` | Prompts user to pick or select one or more objects. |
| `GetObjectsEx` | `?message: string = "Select objects" * ?filter: int = 0 * ?group: bool = true * ?preselect: bool = true * ?select: bool = false * ?printCount: bool = false * ?objectsToSelectFrom: Guid seq = null -> ResizeArray<Guid * bool * SelectionMethod * Point3d * string>` | Prompts user to pick, or select one or more objects. |
| `GetPointCoordinates` | `?message: string = "Select Point Objects" * ?preselect: bool = false -> ResizeArray<Point3d>` | Prompts the user to select one or more point objects. |
| `GetSurfaceObject` | `?message: string = "Select surface" * ?preselect: bool = true * ?select: bool = false -> Guid * bool * SelectionMethod * Point3d * (float * float) * string` | Prompts the user to select a single Surface. |
| `HiddenObjects` | `?includeLights: bool = false * ?includeGrips: bool = false * ?includeReferences: bool = false -> ResizeArray<Guid>` | Returns identifiers of all hidden objects in the document. |
| `InvertSelectedObjects` | `?includeLights: bool = false * ?includeGrips: bool = false * ?includeReferences: bool = false -> ResizeArray<Guid>` | Inverts the current object selection. |
| `LastCreatedObjects` | `?select: bool = false -> ResizeArray<Guid>` | Returns identifiers of the objects that were most recently created or changed by scripting a Rhino command using the Command function. |
| `LastObject` | `?select: bool = false * ?includeLights: bool = false * ?includeGrips: bool = false -> Guid` | Returns the identifier of the last object in the document. |
| `LockedObjects` | `?includeLights: bool = false * ?includeGrips: bool = false * ?includeReferences: bool = false -> ResizeArray<Guid>` | Returns identifiers of all locked objects in the document. |
| `NextObject` | `objectId: Guid * ?select: bool = false * ?includeLights: bool = false * ?includeGrips: bool = false -> Guid` | Returns the identifier of the next object in the document. |
| `NormalObjects` | `?includeLights: bool = false * ?includeGrips: bool = false -> ResizeArray<Guid>` | Returns identifiers of all normal objects in the document. |
| `ObjectsByColor` | `color: Color * ?select: bool = false * ?includeLights: bool = false -> ResizeArray<Guid>` | Returns identifiers of all objects based on color. |
| `ObjectsByGroup` | `groupName: string * ?select: bool = false -> ResizeArray<Guid>` | Returns identifiers of all objects based on the objects' group name. |
| `ObjectsByLayer` | `layerName: string * ?select: bool = false -> ResizeArray<Guid>` | Returns identifiers of all objects based on the objects' layer name. |
| `ObjectsByName` | `name: string * ?select: bool = false * ?includeLights: bool = false * ?includeReferences: bool = false -> ResizeArray<Guid>` | Returns identifiers of all objects based on user-assigned name. |
| `ObjectsByType` | `geometryType: int * ?select: bool = false * ?state: int = 0 -> ResizeArray<Guid>` | Returns identifiers of all objects based on the objects' geometry type. |
| `SelectedObjects` | `?includeLights: bool = false * ?includeGrips: bool = false -> ResizeArray<Guid>` | Returns the identifiers of all objects that are currently selected. |
| `UnselectAllObjects` | `unit -> int` | Unselects all objects in the document. |
| `VisibleObjectsInView` | `?view: string = null * ?select: bool = false * ?includeLights: bool = false * ?includeGrips: bool = false -> ResizeArray<Guid>` | Return identifiers of all objects that are visible in a specified view. |
| `WindowPick` | `corner1: Point3d * corner2: Point3d * ?view: string = null * ?select: bool = false * ?inWindow: bool = true -> ResizeArray<Guid>` | Picks objects using either a window or crossing selection. |

## Surface

From [Src/Scripting_Surface.fs](../Src/Scripting_Surface.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddBox` | `corners: Point3d seq -> Guid` | Adds a box shaped Polysurface to the document. |
| `AddCone` | `basis: Plane * height: float * radius: float * ?cap: bool = true -> Guid` | Adds a cone shaped Polysurface to the document. |
| `AddCone` | `baseCenter: Point3d * tip: Point3d * radius: float * ?cap: bool = true -> Guid` | Adds a cone shaped Polysurface to the document. |
| `AddCutPlane` | `objectIds: Guid seq * startPoint: Point3d * endPoint: Point3d * ?normal: Vector3d = Vector3d() -> Guid` | Adds a planar Surface through objects at a designated location. |
| `MakeCutPlane` | `geometryObjects: GeometryBase seq * startPoint: Point3d * endPoint: Point3d * ?normal: Vector3d = Vector3d() -> PlaneSurface` | Creates a planar Surface through objects at a designated location. |
| `AddCylinder` | `basis: Plane * height: float * radius: float * ?cap: bool = true -> Guid` | Adds a cylinder-shaped Polysurface to the document. |
| `AddEdgeSrf` | `curveIds: Guid seq -> Guid` | Creates a Surface from 2, 3, or 4 edge Curves. |
| `MakeEdgeSrf` | `curveGeos: Curve seq -> Brep` | Creates a Surface from 2, 3, or 4 edge Curves. |
| `AddLoftSrf` | `objectIds: Guid seq * ?start: Point3d = Point3d() * ?ende: Point3d = Point3d() * ?loftType: int = 0 * ?rebuild: int = 0 * ?refit: float = 0.0 * ?closed: bool = false -> ResizeArray<Guid>` | Adds a Surface created by lofting Curves to the document. - no Curve sorting performed. pass in Curves in the order you want them sorted - directions of open Curves not adjusted. |
| `MakeLoftSrf` | `geometryObjects: Curve seq * ?start: Point3d = Point3d() * ?ende: Point3d = Point3d() * ?loftType: int = 0 * ?rebuild: int = 0 * ?refit: float = 0.0 * ?closed: bool = false -> ResizeArray<Brep>` | Creates a Surface created by lofting Curves. - no Curve sorting performed. pass in Curves in the order you want them sorted - directions of open Curves not adjusted. |
| `AddNetworkSrf` | `curves: Guid seq * ?continuity: int = 1 * ?edgeTolerance: float = 0.0 * ?interiorTolerance: float = 0.0 * ?angleTolerance: float = 0.0 -> Guid` | Creates a Surface from a network of crossing Curves. |
| `MakeNetworkSrf` | `curves: Curve seq * ?continuity: int = 1 * ?edgeTolerance: float = 0.0 * ?interiorTolerance: float = 0.0 * ?angleTolerance: float = 0.0 -> NurbsSurface` | Creates a Surface from a network of crossing Curves. |
| `AddNurbsSurface` | `pointCount: (int * int) * points: IList<Point3d> * knotsU: float seq * knotsV: float seq * degree: (int * int) * ?weights: IList<float> = null -> Guid` | Adds a NURBS Surface object to the document. |
| `AddPatch` | `objectIds: Guid seq * startSurfaceId: Guid * ?tolerance: float = 0.0 * ?trim: bool = true * ?pointSpacing: float = 0.1 * ?flexibility: float = 1.0 * ?surfacePull: float = 1.0 * ?fixEdges: bool = false -> Guid` | Fits a Surface through Curve, point, point cloud, and Mesh objects. |
| `AddPatch` | `objectIds: Guid seq * uvSpans: (int * int) * ?tolerance: float = 0.0 * ?trim: bool = true * ?pointSpacing: float = 0.1 * ?flexibility: float = 1.0 * ?surfacePull: float = 1.0 * ?fixEdges: bool = false -> Guid` | Fits a Surface through Curve, point, point cloud, and Mesh objects. |
| `MakePatch` | `geometryObjects: GeometryBase seq * startSurfaceGeo: Surface * ?tolerance: float = 0.0 * ?trim: bool = true * ?pointSpacing: float = 0.1 * ?flexibility: float = 1.0 * ?surfacePull: float = 1.0 * ?fixEdges: bool = false -> Brep` | Fits a Surface through Curve, point, point cloud, and Mesh objects. |
| `MakePatch` | `geometryObjects: GeometryBase seq * uvSpans: (int * int) * ?tolerance: float = 0.0 * ?trim: bool = true * ?pointSpacing: float = 0.1 * ?flexibility: float = 1.0 * ?surfacePull: float = 1.0 * ?fixEdges: bool = false -> Brep` | Fits a Surface through Curve, point, point cloud, and Mesh objects. |
| `AddPipe` | `curveId: Guid * parameters: float seq * radii: float seq * ?blendType: int = 0 * ?cap: int = 0 * ?fit: bool = false -> ResizeArray<Guid>` | Creates a single walled Surface with a circular profile around a Curve. |
| `MakePipe` | `curveGeo: Curve * parameters: float seq * radii: float seq * ?blendType: int = 0 * ?cap: int = 0 * ?fit: bool = false -> ResizeArray<Brep>` | Creates a single walled Surface with a circular profile around a Curve. |
| `AddPlanarSrf` | `objectIds: Guid seq -> ResizeArray<Guid>` | Creates one or more Surfaces from planar Curves. |
| `MakePlanarSrf` | `polyline: Polyline -> Brep` | Creates one Surface from one Polyline Geometry. |
| `MakePlanarSrf` | `curve: Curve -> Brep` | Creates one Surface from one planar Curve. |
| `MakePlanarSrf` | `curves: Curve seq -> ResizeArray<Brep>` | Creates one or more Surfaces from planar Curves. |
| `AddPlaneSurface` | `plane: Plane * uDir: float * vDir: float -> Guid` | Create a Plane Surface and add it to the document. |
| `AddRailRevSrf` | `profile: Guid * rail: Guid * axis: Line * ?scaleHeight: bool = false -> Guid` | Adds a Surface created through profile Curves that define the Surface shape and two Curves that defines a Surface edge. |
| `MakeRailRevSrf` | `profile: Curve * rail: Curve * axis: Line * ?scaleHeight: bool = false -> NurbsSurface` | Creates a Surface created through profile Curves that define the Surface shape and two Curves that defines a Surface edge. |
| `AddRevSrf` | `curveId: Guid * axis: Line * ?startAngle: float = 0.0 * ?endAngle: float = 360.0 -> Guid` | Create a Surface by revolving a Curve around an axis. |
| `MakeRevSrf` | `curveGeo: Curve * axis: Line * ?startAngle: float = 0.0 * ?endAngle: float = 360.0 -> NurbsSurface` | Create a Surface by revolving a Curve around an axis. |
| `AddSphere` | `center: Point3d * radius: float -> Guid` | Add a spherical Surface to the document. |
| `AddSrfContourCrvs` | `objectId: Guid * plane: Plane -> ResizeArray<Guid>` | Adds a spaced series of planar Curves resulting from the intersection of defined cutting Planes through a Surface or Polysurface. |
| `AddSrfContourCrvs` | `objectId: Guid * startPoint: Point3d * endPoint: Point3d * interval: float -> ResizeArray<Guid>` | Adds a spaced series of planar Curves resulting from the intersection of defined cutting Planes through a Surface or Polysurface. |
| `MakeSrfContourCrvs` | `geometryObject: Brep * plane: Plane -> ResizeArray<Curve>` | Creates a spaced series of planar Curves resulting from the intersection of defined cutting Planes through a Surface or Polysurface. |
| `MakeSrfContourCrvs` | `geometryObject: Brep * startPoint: Point3d * endPoint: Point3d * interval: float -> ResizeArray<Curve>` | Creates a spaced series of planar Curves resulting from the intersection of defined cutting Planes through a Surface or Polysurface. |
| `AddSrfControlPtGrid` | `count: (int * int) * points: Point3d seq * ?degreeU: int = 3 * ?degreeV: int = 3 -> Guid` | Creates a Surface from a grid of points. |
| `AddSrfPt` | `pointA: Point3d * pointB: Point3d * pointC: Point3d * pointD: Point3d -> Guid` | Creates a new Surface from four corner points. |
| `AddSrfPt` | `pointA: Point3d * pointB: Point3d * pointC: Point3d -> Guid` | Creates a new Surface from three corner points. |
| `AddSrfPtGrid` | `count: (int * int) * points: Point3d seq * ?degreeU: int = 3 * ?degreeV: int = 3 * ?closedU: bool = false * ?closedV: bool = false -> Guid` | Creates a Surface from a grid of points. |
| `AddSweep1` | `rail: Guid * shapes: Guid seq * ?closed: bool = false -> ResizeArray<Guid>` | Adds a Surface created through profile Curves that define the Surface shape and one Curve that defines a Surface edge. |
| `MakeSweep1` | `rail: Curve * shapes: Curve seq * ?closed: bool = false -> ResizeArray<Brep>` | Creates a Surface created through profile Curves that define the Surface shape and one Curve that defines a Surface edge. |
| `AddSweep2` | `rails: (Guid * Guid) * shapes: Guid seq * ?closed: bool = false -> ResizeArray<Guid>` | Adds a Surface created through profile Curves that define the Surface shape and two Curves that defines a Surface edge. |
| `MakeSweep2` | `rails: (Curve * Curve) * shapes: Curve seq * ?closed: bool = false -> ResizeArray<Brep>` | Creates a Surface created through profile Curves that define the Surface shape and two Curves that defines a Surface edge. |
| `AddTorus` | `basis: Plane * majorRadius: float * minorRadius: float -> Guid` | Adds a torus shaped revolved Surface to the document. |
| `BooleanDifference` | `input0: Guid seq * input1: Guid seq * ?deleteInput: bool = true -> ResizeArray<Guid>` | Performs a boolean difference operation on two sets of input Surfaces and Polysurfaces. |
| `BooleanDifference` | `input0: Brep seq * input1: Brep seq -> ResizeArray<Brep>` | Performs a boolean difference operation on two sets of input Surfaces and Polysurfaces. |
| `BooleanIntersection` | `input0: Guid seq * input1: Guid seq * ?deleteInput: bool = true -> ResizeArray<Guid>` | Performs a boolean intersection operation on two sets of input Surfaces and Polysurfaces. |
| `BooleanIntersection` | `input0: Brep seq * input1: Brep seq -> ResizeArray<Brep>` | Performs a boolean intersection operation on two sets of input Surfaces and Polysurfaces. |
| `BooleanUnion` | `input: Guid seq * ?deleteInput: bool = true -> ResizeArray<Guid>` | Performs a boolean union operation on a set of input Surfaces and Polysurfaces. |
| `BooleanUnion` | `input: Brep seq -> ResizeArray<Brep>` | Performs a boolean union operation on a set of input Surfaces and Polysurfaces. |
| `BrepClosestPoint` | `objectId: Guid * point: Point3d -> Point3d * float * float * ComponentIndexType * int * Vector3d` | Returns the point on a Surface or Polysurface that is closest to a test point. |
| `BrepClosestPoint` | `geometryObject: Brep * point: Point3d -> Point3d * float * float * ComponentIndexType * int * Vector3d` | Returns the point on a Surface or Polysurface that is closest to a test point. |
| `CapPlanarHoles` | `surfaceId: Guid -> bool` | Caps planar holes in a Surface or Polysurface. |
| `ChangeSurfaceDegree` | `objectId: Guid * degree: (int * int) -> bool` | Changes the degree of a Surface object. |
| `DuplicateEdgeCurves` | `objectId: Guid * ?select: bool = false -> ResizeArray<Guid>` | Duplicates the edge Curves of a Surface or Polysurface. |
| `DuplicateEdgeCurves` | `geometryObject: Brep -> ResizeArray<Curve>` | Duplicates the edge Curves of a Surface or Polysurface. |
| `DuplicateSurfaceBorder` | `surfaceId: Guid * ?typ: int = 0 -> ResizeArray<Guid>` | Create Curves that duplicate a Surface or Polysurface border. |
| `DuplicateSurfaceBorder` | `surfaceGeo: Brep * ?typ: int = 0 -> ResizeArray<Curve>` | Create Curves that duplicate a Surface or Polysurface border. |
| `EvaluateSurface` | `surfaceId: Guid * u: float * v: float -> Point3d` | Evaluates a Surface at a U, V parameter. |
| `EvaluateSurface` | `surfaceGeo: Surface * u: float * v: float -> Point3d` | Evaluates a Surface at a U, V parameter. |
| `ExplodePolysurfaces` | `objectIds: Guid seq * ?deleteInput: bool = false -> ResizeArray<Guid>` | Explodes, or un-joins, one or more Polysurface objects. |
| `ExplodePolysurfaces` | `geometryObjects: Brep seq -> ResizeArray<Brep>` | Explodes, or un-joins, one or more Polysurface objects. |
| `ExtendSurface` | `surfaceId: Guid * parameter: (float * float) * length: float * ?smooth: bool = true -> bool` | Lengthens an untrimmed Surface object. |
| `ExtractIsoCurve` | `surfaceId: Guid * parameter: (float * float) * direction: int -> ResizeArray<Guid>` | Extracts iso-parametric Curves from a Surface. |
| `ExtractIsoCurve` | `surfaceGeo: Surface * parameter: (float * float) * direction: int -> ResizeArray<Curve>` | Extracts iso-parametric Curves from a Surface. |
| `ExtractSurface` | `objectId: Guid * faceIndices: int seq * ?copy: bool = false -> ResizeArray<Guid>` | Separates or copies a Surface or a copy of a Surface from a Polysurface. |
| `ExtractSurface` | `geometryObject: Brep * faceIndices: int seq -> ResizeArray<Brep>` | Duplicates the selected faces of a Polysurface as independent Breps. |
| `ExtrudeCurve` | `curveId: Guid * pathId: Guid -> Guid` | Creates a Surface by extruding a Curve along a path. |
| `ExtrudeCurve` | `curveGeo: Curve * pathGeo: Curve -> SumSurface` | Creates a Surface by extruding a Curve along a path. |
| `ExtrudeCurvePoint` | `curveId: Guid * point: Point3d -> Guid` | Creates a Surface by extruding a Curve to a point. |
| `ExtrudeCurvePoint` | `curveGeo: Curve * point: Point3d -> Surface` | Creates a Surface by extruding a Curve to a point. |
| `ExtrudeCurveStraight` | `curveId: Guid * startPoint: Point3d * endPoint: Point3d -> Guid` | Create Surface by extruding a Curve along two points that define a line. |
| `ExtrudeCurveStraight` | `curveGeo: Curve * startPoint: Point3d * endPoint: Point3d -> Surface` | Create Surface by extruding a Curve along two points that define a line. |
| `ExtrudeSurface` | `surfaceId: Guid * curveId: Guid * ?cap: bool = true -> Guid` | Create Surface by extruding along a path Curve. |
| `ExtrudeSurface` | `surfaceGeo: Brep * curveGeo: Curve * ?cap: bool = true -> Brep` | Create Surface by extruding along a path Curve. |
| `FilletSurfaces` | `surface0: Guid * surface1: Guid * radius: float * ?uvparam0: Point2d = Point2d() * ?uvparam1: Point2d = Point2d() -> ResizeArray<Guid>` | Create constant radius rolling ball fillets between two Surfaces. |
| `FilletSurfaces` | `surface0: Surface * surface1: Surface * radius: float * ?uvparam0: Point2d = Point2d() * ?uvparam1: Point2d = Point2d() -> ResizeArray<Surface>` | Create constant radius rolling ball fillets between two Surfaces. |
| `FlipSurface` | `surfaceId: Guid * flip: bool -> unit` | Changes the normal direction of a Surface. |
| `FlipSurface` | `surfaceIds: Guid seq * flip: bool -> unit` | Changes the normal direction of multiple Surface. |
| `IntersectBreps` | `brep1: Guid * brep2: Guid * ?tolerance: float = 0.0 -> ResizeArray<Guid>` | Intersects a brep object with another brep object. |
| `IntersectBreps` | `brep1: Brep * brep2: Brep * ?tolerance: float = 0.0 -> ResizeArray<GeometryBase>` | Intersects a brep object with another brep object. |
| `IntersectSpheres` | `spherePlane0: Plane * sphereRadius0: float * spherePlane1: Plane * sphereRadius1: float -> int * Circle * float` | Calculates intersections of two spheres. |
| `IsBrep` | `objectId: Guid -> bool` | Verifies an object is a Brep. |
| `IsBrep` | `geometryObject: GeometryBase -> bool` | Verifies an object is a Brep. |
| `IsCone` | `objectId: Guid -> bool` | Determines if a Surface is a portion of a cone. |
| `IsCone` | `geometryObject: Surface -> bool` | Determines if a Surface is a portion of a cone. |
| `IsCylinder` | `objectId: Guid -> bool` | Determines if a Surface is a portion of a cylinder. |
| `IsCylinder` | `geometryObject: Surface -> bool` | Determines if a Surface is a portion of a cylinder. |
| `IsExtrusion` | `objectId: Guid -> bool` | Verifies an object is a or Extrusion. |
| `IsExtrusion` | `geometryObject: GeometryBase -> bool` | Verifies an object is a or Extrusion. |
| `IsPlaneSurface` | `objectId: Guid -> bool` | Verifies an object is a Plane Surface. |
| `IsPlaneSurface` | `geometryObject: Surface -> bool` | Verifies an object is a Plane Surface. |
| `IsPointInSurface` | `objectId: Guid * point: Point3d * ?strictlyIn: bool = false * ?tolerance: float = 0.0 -> bool` | Checks if a point is inside a closed Mesh , Surface or Polysurface. |
| `IsPointInSurface` | `geometryObject: GeometryBase * point: Point3d * ?strictlyIn: bool = false * ?tolerance: float = 0.0 -> bool` | Checks if a point is inside a closed Mesh , Surface or Polysurface. |
| `IsPointOnSurface` | `objectId: Guid * point: Point3d -> bool` | Checks if a point lies on a Surface. |
| `IsPointOnSurface` | `geometryObject: GeometryBase * point: Point3d -> bool` | Checks if a point lies on a Surface. |
| `IsPolysurface` | `objectId: Guid -> bool` | Verifies an object is a Polysurface or Extrusion. |
| `IsPolysurface` | `geometryObject: GeometryBase -> bool` | Verifies an object is a Polysurface or Extrusion. |
| `IsPolysurfaceClosed` | `objectId: Guid -> bool` | Verifies a Guid refers to a closed Polysurface or closed Extrusion. |
| `IsPolysurfaceClosed` | `geometryObject: GeometryBase -> bool` | Verifies a Guid refers to a closed Polysurface or closed Extrusion. |
| `IsSphere` | `objectId: Guid -> bool` | Determines if a Surface is a portion of a Sphere. |
| `IsSphere` | `geometryObject: Surface -> bool` | Determines if a Surface is a portion of a Sphere. |
| `IsSurface` | `objectId: Guid -> bool` | Verifies an object is a Surface, Extrusion or Brep objects with only one face. |
| `IsSurface` | `geometryObject: Surface -> bool` | Verifies an object is a Surface, Extrusion or Brep objects with only one face. |
| `IsSurfaceClosed` | `surfaceId: Guid * direction: int -> bool` | Verifies a Surface object is closed in the specified direction. |
| `IsSurfaceClosed` | `surfaceGeo: Surface * direction: int -> bool` | Verifies a Surface object is closed in the specified direction. |
| `IsSurfaceFliped` | `surfaceId: Guid -> bool` | Returns the normal direction of a Surface. |
| `IsSurfaceFliped` | `surfaceGeo: Brep -> bool` | Returns the normal direction of a Surface. |
| `IsSurfacePeriodic` | `surfaceId: Guid * direction: int -> bool` | Verifies a Surface object is periodic in the specified direction. |
| `IsSurfacePeriodic` | `surfaceGeo: Surface * direction: int -> bool` | Verifies a Surface object is periodic in the specified direction. |
| `IsSurfacePlanar` | `surfaceId: Guid * ?tolerance: float = 0.0 -> bool` | Verifies a Surface object is planar. |
| `IsSurfacePlanar` | `surfaceGeo: Surface * ?tolerance: float = 0.0 -> bool` | Verifies a Surface object is planar. |
| `IsSurfaceRational` | `surfaceId: Guid -> bool` | Verifies a Surface object is rational. |
| `IsSurfaceRational` | `surfaceGeo: Surface -> bool` | Verifies a Surface object is rational. |
| `IsSurfaceSingular` | `surfaceId: Guid * direction: int -> bool` | Verifies a Surface object is singular in the specified direction. |
| `IsSurfaceSingular` | `surfaceGeo: Surface * direction: int -> bool` | Verifies a Surface object is singular in the specified direction. |
| `IsSurfaceTrimmed` | `surfaceId: Guid -> bool` | Verifies a Surface object has been trimmed. |
| `IsSurfaceTrimmed` | `surfaceGeo: Brep -> bool` | Verifies a Surface object has been trimmed. |
| `IsTorus` | `surfaceId: Guid -> bool` | Determines if a Surface is a portion of a torus. |
| `IsTorus` | `surfaceGeo: Surface -> bool` | Determines if a Surface is a portion of a torus. |
| `JoinSurfaces` | `objectIds: Guid seq * ?deleteInput: bool = false -> Guid` | Joins two or more Surface or Polysurface objects together to form one Polysurface object. |
| `JoinSurfaces` | `geometryObjects: Brep seq -> Brep` | Joins two or more Surface or Polysurface objects together to form one Polysurface object. |
| `MakeSurfacePeriodic` | `surfaceId: Guid * direction: int * ?deleteInput: bool = false -> Guid` | Makes an existing Surface a periodic NURBS Surface. |
| `MakeSurfacePeriodic` | `surfaceGeo: Surface * direction: int -> Surface` | Makes an existing Surface a periodic NURBS Surface. |
| `OffsetSurface` | `surfaceId: Guid * distance: float * ?tolerance: float = 0.0 * ?bothSides: bool = false * ?createSolid: bool = false -> Guid` | Offsets a trimmed or untrimmed Surface by a distance. |
| `OffsetSurface` | `surfaceGeo: Brep * distance: float * ?tolerance: float = 0.0 * ?bothSides: bool = false * ?createSolid: bool = false -> Brep` | Offsets a trimmed or untrimmed Surface by a distance. |
| `PullCurve` | `surface: Guid * curve: Guid * ?deleteInput: bool = false -> ResizeArray<Guid>` | Pulls a Curve object to a Surface object. |
| `PullCurve` | `surface: Brep * curve: Curve -> ResizeArray<Curve>` | Pulls a Curve object to a Surface object. |
| `RebuildSurface` | `objectId: Guid * ?degreeU: int = 3 * ?degreeV: int = 3 * ?pointcountU: int = 10 * ?pointcountV: int = 10 -> bool` | Rebuilds a Surface to a given degree and control point count. |
| `RemoveSurfaceKnot` | `surface: Guid * uvParameter: (float * float) * vDirection: bool -> bool` | Deletes a knot from a Surface object. |
| `ReverseSurface` | `surfaceId: Guid * direction: int -> unit` | Reverses U or V directions of a Surface, or swaps (transposes) U and V directions. |
| `ShootRay` | `surfaceIds: Guid seq * startPoint: Point3d * direction: Vector3d * ?reflections: int = 10 -> Point3d array` | Shoots a ray at a collection of Surfaces or Polysurfaces. |
| `ShootRay` | `surfaceGeos: Brep seq * startPoint: Point3d * direction: Vector3d * ?reflections: int = 10 -> Point3d array` | Shoots a ray at a collection of Surfaces or Polysurfaces. |
| `ShortPath` | `surfaceId: Guid * startPoint: Point3d * endPoint: Point3d -> Guid` | Creates the shortest possible Curve(geodesic) between two points on a Surface. |
| `ShortPath` | `surfaceGeo: Surface * startPoint: Point3d * endPoint: Point3d -> Curve` | Creates the shortest possible Curve(geodesic) between two points on a Surface. |
| `ShrinkTrimmedSurface` | `objectId: Guid * ?createCopy: bool = false -> Guid` | Shrinks the underlying untrimmed Surfaces near to the trimming boundaries. |
| `ShrinkTrimmedSurface` | `geometryObject: Brep -> Brep` | Shrinks the underlying untrimmed Surfaces near to the trimming boundaries. |
| `SplitBrep` | `brepId: Guid * cutterId: Guid * ?deleteInput: bool = false -> ResizeArray<Guid>` | Splits a brep. |
| `SplitBrep` | `brepGeo: Brep * cutterGeo: Brep -> ResizeArray<Brep>` | Splits a brep. |
| `SurfaceArea` | `srf: Surface -> float` | Calculate the area of a Surface Geometry. |
| `SurfaceArea` | `brep: Brep -> float` | Calculate the area of a Brep / Polysurface Geometry. |
| `SurfaceArea` | `objectId: Guid -> float` | Calculate the area of a Surface or Polysurface object. |
| `SurfaceArea` | `geometryObject: GeometryBase -> float` | Calculate the area of a Surface or Polysurface object. |
| `SurfaceAreaCentroid` | `objectId: Guid -> Point3d` | Calculates the area centroid of a Surface or Polysurface. |
| `SurfaceAreaCentroid` | `geometryObject: GeometryBase -> Point3d` | Calculates the area centroid of a Surface or Polysurface. |
| `SurfaceAreaMoments` | `surfaceId: Guid -> ResizeArray<float * float * float>` | Calculates area moments of inertia of a Surface or Polysurface object. |
| `SurfaceAreaMoments` | `surfaceGeo: GeometryBase -> ResizeArray<float * float * float>` | Calculates area moments of inertia of a Surface or Polysurface object. |
| `SurfaceClosestParameter` | `surfaceId: Guid * testPoint: Point3d -> float * float` | Returns U, V parameters of point on a Surface that is closest to a test point. |
| `SurfaceClosestParameter` | `surfaceGeo: Surface * testPoint: Point3d -> float * float` | Returns U, V parameters of point on a Surface that is closest to a test point. |
| `SurfaceClosestPoint` | `surfaceId: Guid * testPoint: Point3d -> Point3d` | Returns the point on a Surface that is closest to a test point. |
| `SurfaceClosestPoint` | `surfaceGeo: Surface * testPoint: Point3d -> Point3d` | Returns the point on a Surface that is closest to a test point. |
| `SurfaceCone` | `surfaceId: Guid -> Plane * float * float` | Returns the definition of a Surface cone. |
| `SurfaceCone` | `surfaceGeo: Surface -> Plane * float * float` | Returns the definition of a Surface cone. |
| `SurfaceCurvature` | `surfaceId: Guid * parameter: (float * float) -> Point3d * Vector3d * float * Vector3d * float * Vector3d * float * float` | Returns the curvature of a Surface at a U, V parameter. |
| `SurfaceCurvature` | `surfaceGeo: Surface * parameter: (float * float) -> Point3d * Vector3d * float * Vector3d * float * Vector3d * float * float` | Returns the curvature of a Surface at a U, V parameter. |
| `SurfaceCylinder` | `surfaceId: Guid -> Plane * float * float` | Returns the definition of a cylinder Surface. |
| `SurfaceCylinder` | `surfaceGeo: Surface -> Plane * float * float` | Returns the definition of a cylinder Surface. |
| `SurfaceDegree` | `surfaceId: Guid -> int * int` | Returns the U and V degrees of a Surface. |
| `SurfaceDegree` | `surfaceGeo: Surface -> int * int` | Returns the U and V degrees of a Surface. |
| `SurfaceDomain` | `surfaceId: Guid * direction: int -> float * float` | Returns the domain of a Surface object in the specified direction. |
| `SurfaceDomain` | `surfaceGeo: Surface * direction: int -> float * float` | Returns the domain of a Surface object in the specified direction. |
| `SurfaceEditPointParameters` | `surfaceId: Guid * ?returnAll: bool = true -> ResizeArray<float * float>` | Returns the parameters at edit, or Greville points of a Surface object. |
| `SurfaceEditPointParameters` | `surfaceGeo: Surface * ?returnAll: bool = true -> ResizeArray<float * float>` | Returns the parameters at edit, or Greville points of a Surface object. |
| `SurfaceEditPoints` | `surfaceId: Guid * ?returnAll: bool = true -> ResizeArray<Point3d>` | Returns the edit, or Greville points of a Surface object. |
| `SurfaceEditPoints` | `surfaceGeo: Surface * ?returnAll: bool = true -> ResizeArray<Point3d>` | Returns the edit, or Greville points of a Surface object. |
| `SurfaceEvaluate` | `surfaceId: Guid * parameter: (float * float) * derivative: int -> Point3d * ResizeArray<Vector3d>` | A general purpose Surface evaluator. |
| `SurfaceEvaluate` | `surfaceGeo: Surface * parameter: (float * float) * derivative: int -> Point3d * ResizeArray<Vector3d>` | A general purpose Surface evaluator. |
| `SurfaceFrame` | `surfaceId: Guid * uvParameter: (float * float) -> Plane` | Returns a Plane based on the normal, u, and v directions at a Surface U, V parameter. |
| `SurfaceFrame` | `surfaceGeo: Surface * uvParameter: (float * float) -> Plane` | Returns a Plane based on the normal, u, and v directions at a Surface U, V parameter. |
| `SurfaceIsocurveDensity` | `surfaceId: Guid -> int` | Returns the isocurve density of a Surface or Polysurface object. |
| `SurfaceIsocurveDensity` | `surfaceId: Guid * density: int -> unit` | Sets the isocurve density of a Surface or Polysurface object. |
| `SurfaceIsocurveDensity` | `surfaceIds: Guid seq * density: int -> unit` | Sets the isocurve density of multiple Surface or Polysurface objects. |
| `SurfaceKnotCount` | `surfaceId: Guid -> int * int` | Returns the knot count of a Surface SurfaceId = the Surface's identifier. |
| `SurfaceKnotCount` | `surfaceGeo: Surface -> int * int` | Returns the knot count of a Surface SurfaceId = the Surface's identifier. |
| `SurfaceKnots` | `surfaceId: Guid -> NurbsSurfaceKnotList * NurbsSurfaceKnotList` | Returns the knots, or knot vector, of a Surface object. |
| `SurfaceKnots` | `surfaceGeo: Surface -> NurbsSurfaceKnotList * NurbsSurfaceKnotList` | Returns the knots, or knot vector, of a Surface object. |
| `SurfaceNormal` | `surfaceId: Guid * uvParameter: (float * float) -> Vector3d` | Returns 3D vector that is the normal to a Surface at a parameter. |
| `SurfaceNormal` | `surfaceId: Guid -> Vector3d` | Returns 3D vector that is the normal to a Surface at mid parameter. |
| `SurfaceNormal` | `surfaceGeo: Surface * uvParameter: (float * float) -> Vector3d` | Returns 3D vector that is the normal to a Surface at a parameter. |
| `SurfaceNormal` | `surfaceGeo: Surface -> Vector3d` | Returns 3D vector that is the normal to a Surface at mid parameter. |
| `SurfaceNormalizedParameter` | `surfaceId: Guid * parameter: (float * float) -> float * float` | Converts Surface parameter to a normalized Surface parameter; one that ranges between 0.0 and 1.0 in both the U and V directions. |
| `SurfaceNormalizedParameter` | `surfaceGeo: Surface * parameter: (float * float) -> float * float` | Converts Surface parameter to a normalized Surface parameter; one that ranges between 0.0 and 1.0 in both the U and V directions. |
| `SurfaceParameter` | `surfaceId: Guid * parameter: (float * float) -> float * float` | Converts normalized Surface parameter to a Surface domain specific (regular) parameter. or a parameter within the Surface's domain. |
| `SurfaceParameter` | `surfaceGeo: Surface * parameter: (float * float) -> float * float` | Converts normalized Surface parameter to a Surface domain specific (regular) parameter. or a parameter within the Surface's domain. |
| `SurfacePointCount` | `surfaceId: Guid -> int * int` | Returns the control point count of a Surface SurfaceId = the Surface's identifier. |
| `SurfacePointCount` | `surfaceGeo: Surface -> int * int` | Returns the control point count of a Surface SurfaceId = the Surface's identifier. |
| `SurfacePoints` | `surfaceId: Guid * ?returnAll: bool = true -> ResizeArray<Point3d>` | Returns the control points, or control vertices, of a Surface object. |
| `SurfacePoints` | `surfaceGeo: Surface * ?returnAll: bool = true -> ResizeArray<Point3d>` | Returns the control points, or control vertices, of a Surface object. |
| `SurfaceSphere` | `surfaceId: Guid -> Plane * float` | Gets the sphere definition from a Surface, if possible. |
| `SurfaceSphere` | `surfaceGeo: Surface -> Plane * float` | Gets the sphere definition from a Surface, if possible. |
| `SurfaceTorus` | `surfaceId: Guid -> Plane * float * float` | Returns the definition of a Surface torus. |
| `SurfaceTorus` | `surfaceGeo: Surface -> Plane * float * float` | Returns the definition of a Surface torus. |
| `SurfaceVolume` | `objectId: Guid -> float` | Calculates volume of a closed Surface or Polysurface. |
| `SurfaceVolume` | `geometryObject: GeometryBase -> float` | Calculates volume of a closed Surface or Polysurface. |
| `SurfaceVolumeCentroid` | `objectId: Guid -> Point3d` | Calculates volume centroid of a closed Surface or Polysurface. |
| `SurfaceVolumeCentroid` | `geometryObject: GeometryBase -> Point3d` | Calculates volume centroid of a closed Surface or Polysurface. |
| `SurfaceVolumeMoments` | `objectId: Guid -> ResizeArray<float * float * float>` | Calculates volume moments of inertia of a Surface or Polysurface object. |
| `SurfaceVolumeMoments` | `geometryObject: GeometryBase -> ResizeArray<float * float * float>` | Calculates volume moments of inertia of a Surface or Polysurface object. |
| `SurfaceWeights` | `objectId: Guid -> ResizeArray<float>` | Returns list of weight values assigned to the control points of a Surface. |
| `SurfaceWeights` | `geometryObject: Surface -> ResizeArray<float>` | Returns list of weight values assigned to the control points of a Surface. |
| `TrimBrep` | `objectId: Guid * cutter: Guid * ?tolerance: float = 0.0 -> ResizeArray<Guid>` | Trims a Surface or Polysurface using an oriented cutter brep or Surface. |
| `TrimBrep` | `objectId: Guid * cutter: Plane * ?tolerance: float = 0.0 -> ResizeArray<Guid>` | Trims a Surface using an oriented cutter Plane. |
| `TrimBrep` | `geometryObject: Brep * cutter: Brep * ?tolerance: float = 0.0 -> ResizeArray<Brep>` | Trims a Surface or Polysurface using an oriented cutter brep or Surface. |
| `TrimBrep` | `geometryObject: Brep * cutter: Plane * ?tolerance: float = 0.0 -> ResizeArray<Brep>` | Trims a Surface using an oriented cutter Plane. |
| `TrimSurfaceU` | `surfaceId: Guid * interval: (float * float) * ?deleteInput: bool = false -> Guid` | Remove portions of the Surface outside of the specified interval in U direction. |
| `TrimSurfaceU` | `surfaceGeo: Surface * interval: (float * float) -> Surface` | Remove portions of the Surface outside of the specified interval in U direction. |
| `TrimSurfaceUV` | `surfaceId: Guid * intervalU: (float * float) * intervalV: (float * float) * ?deleteInput: bool = false -> Guid` | Remove portions of the Surface outside of the specified interval ain U and V direction. |
| `TrimSurfaceUV` | `surfaceGeo: Surface * intervalU: (float * float) * intervalV: (float * float) -> Surface` | Remove portions of the Surface outside of the specified interval ain U and V direction. |
| `TrimSurfaceV` | `surfaceId: Guid * interval: (float * float) * ?deleteInput: bool = false -> Guid` | Remove portions of the Surface outside of the specified interval in V direction. |
| `TrimSurfaceV` | `surfaceGeo: Surface * interval: (float * float) -> Surface` | Remove portions of the Surface outside of the specified interval in V direction. |
| `UnrollSurface` | `surfaceId: Guid * ?explode: bool = false * ?followingGeometry: Guid seq = null * ?absoluteTolerance: float = 0.0 * ?relativeTolerance: float = 0.0 -> ResizeArray<Guid> * ResizeArray<Guid>` | Flattens a develop-able Surface or Polysurface. |
| `UnrollSurface` | `surfaceGeo: Brep * ?explode: bool = false * ?followingGeometry: GeometryBase seq = null * ?absoluteTolerance: float = 0.0 * ?relativeTolerance: float = 0.0 -> ResizeArray<Brep> * ResizeArray<GeometryBase>` | Flattens a develop-able Surface or Polysurface. |

## Toolbar

From [Src/Scripting_Toolbar.fs](../Src/Scripting_Toolbar.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `CloseToolbarCollection` | `name: string * ?prompt: bool = false -> bool` | Closes a currently open toolbar collection. |
| `HideToolbar` | `name: string * toolbarGroup: string -> bool` | Hides a previously visible toolbar group in an open toolbar collection. |
| `IsToolbar` | `name: string * toolbar: string * ?group: bool = false -> bool` | Verifies a toolbar (or toolbar group) exists in an open collection file. |
| `IsToolbarCollection` | `file: string -> string` | Checks if a toolbar collection is open. |
| `IsToolbarDocked` | `name: string * toolbarGroup: string -> bool` | Checks if a toolbar group in an open toolbar collection is docked. |
| `IsToolbarVisible` | `name: string * toolbarGroup: string -> bool` | Checks if a toolbar group in an open toolbar collection is visible. |
| `OpenToolbarCollection` | `file: string -> string` | Opens a toolbar collection file. |
| `SaveToolbarCollection` | `name: string -> bool` | Saves an open toolbar collection to disk. |
| `SaveToolbarCollectionAs` | `name: string * file: string -> bool` | Saves an open toolbar collection to a different disk file. |
| `ShowToolbar` | `name: string * toolbarGroup: string -> bool` | Shows a previously hidden toolbar group in an open toolbar collection. |
| `ToolbarCollectionCount` | `unit -> int` | Returns number of currently open toolbar collections. |
| `ToolbarCollectionNames` | `unit -> ResizeArray<string>` | Returns names of all currently open toolbar collections. |
| `ToolbarCollectionPath` | `name: string -> string` | Returns full path to a currently open toolbar collection file. |
| `ToolbarCount` | `name: string * ?groups: bool = false -> int` | Returns the number of toolbars or groups in a currently open toolbar file. |
| `ToolbarNames` | `name: string * ?groups: bool = false -> ResizeArray<string>` | Returns the names of all toolbars (or toolbar groups) found in a currently open toolbar file. |

## Transformation

From [Src/Scripting_Transformation.fs](../Src/Scripting_Transformation.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `IsXformIdentity` | `xForm: Transform -> bool` | Verifies a matrix is the identity matrix. |
| `IsXformSimilarity` | `xForm: Transform -> bool` | Verifies a matrix is a similarity transformation. |
| `IsXformZero` | `xForm: Transform -> bool` | Checks if a matrix is a zero transformation matrix. |
| `XformChangeBasis` | `initialPlane: Plane * finalPlane: Plane -> Transform` | Returns a change of basis transformation matrix or None on error. |
| `XformChangeBasis2` | `x0: Vector3d * y0: Vector3d * z0: Vector3d * x1: Vector3d * y1: Vector3d * z1: Vector3d -> Transform` | Returns a change of basis transformation matrix or None on error. |
| `XformCompare` | `xForm1: Transform * xForm2: Transform -> int` | Compares two transformation matrices. |
| `XformCPlaneToWorld` | `point: Point3d * plane: Plane -> Point3d` | Transform point from construction Plane coordinates to world coordinates. |
| `XformDeterminant` | `xForm: Transform -> float` | Returns the determinant of a transformation matrix. |
| `XformDiagonal` | `diagonalValue: float -> Transform` | Returns a diagonal transformation matrix. |
| `XformIdentity` | `unit -> Transform` | returns the identity transformation matrix. |
| `XformInverse` | `xForm: Transform -> Transform` | Returns the inverse of a non-singular transformation matrix. |
| `XformMirror` | `mirrorPlanePoint: Point3d * mirrorPlaneNormal: Vector3d -> Transform` | Creates a mirror transformation matrix. |
| `XformMultiply` | `xForm1: Transform * xForm2: Transform -> Transform` | Multiplies two transformation matrices, where result = xForm1 \* xForm2. |
| `XformPlanarProjection` | `plane: Plane -> Transform` | Returns a transformation matrix that projects to a Plane. |
| `XformRotation1` | `initialPlane: Plane * finalPlane: Plane -> Transform` | Returns a rotation transformation that maps initialPlane to finalPlane. |
| `XformRotation2` | `angleDegrees: float * rotationAxis: Vector3d * centerPoint: Point3d -> Transform` | Returns a rotation transformation around an axis. |
| `XformRotation3` | `startDirection: Vector3d * endDirection: Vector3d * centerPoint: Point3d -> Transform` | Calculate the minimal transformation that rotates startDirection to endDirection while fixing centerPoint. |
| `XformRotation4` | `x0: Vector3d * y0: Vector3d * z0: Vector3d * x1: Vector3d * y1: Vector3d * z1: Vector3d -> Transform` | Returns a rotation transformation. |
| `XformScale` | `scaleX: float * scaleY: float * scaleZ: float * point: Point3d -> Transform` | Creates a scale transformation. |
| `XformScale` | `scale: float -> Transform` | Creates a scale transformation based on World Origin point. |
| `XformScale` | `scale: float * point: Point3d -> Transform` | Creates a scale transformation. |
| `XformScale` | `scaleX: float * scaleY: float * scaleZ: float * plane: Plane -> Transform` | Creates a scale transformation about the origin of a Plane and along the Plane's axes. |
| `XformScale` | `scale: float * plane: Plane -> Transform` | Creates a uniform scale transformation about the origin of a Plane. |
| `XformScreenToWorld` | `point: Point3d * ?view: string = null * ?screenCoordinates: bool = false -> Point3d` | Transforms a point from either client-area coordinates of the specified view or screen coordinates to world coordinates. |
| `XformShear` | `plane: Plane * x: Vector3d * y: Vector3d * z: Vector3d -> Transform` | Returns a shear transformation matrix. |
| `XformTranslation` | `vector: Vector3d -> Transform` | Creates a translation transformation matrix. |
| `XformWorldToCPlane` | `point: Point3d * plane: Plane -> Point3d` | Transforms a point from world coordinates to construction Plane coordinates. |
| `XformWorldToScreen` | `point: Point3d * ?view: string = null * ?screenCoordinates: bool = false -> Point2d` | Transforms a point from world coordinates to either client-area coordinates of the specified view or screen coordinates. |
| `XformZero` | `unit -> Transform` | Returns a zero transformation matrix. |

## UserData

From [Src/Scripting_UserData.fs](../Src/Scripting_UserData.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `DeleteDocumentData` | `?section: string = null * ?entry: string = null -> unit` | Removes user data strings from the current document. |
| `DeleteDocumentUserText` | `key: string -> unit` | Removes user text stored in the document. |
| `DeleteUserText` | `objectId: Guid * key: string * ?attachToGeometry: bool = false -> unit` | Removes user text stored on an object. |
| `DeleteUserText` | `objectIds: Guid seq * key: string * ?attachToGeometry: bool = false -> unit` | Removes user text stored on multiple objects. |
| `DocumentDataCount` | `unit -> int` | Returns the number of user data strings in the current document. |
| `DocumentUserTextCount` | `unit -> int` | Returns the number of user text strings in the current document. |
| `GetDocumentData` | `?sectionName: string = null -> string array` | Returns a user data item from the current document. |
| `GetDocumentDataEntry` | `section: string * entry: string -> string` | Returns a user data item entry from the current document. |
| `GetDocumentUserText` | `key: string -> string` | Returns user text stored in the document. |
| `GetDocumentUserTextKeys` | `unit -> ResizeArray<string>` | Returns all document user text keys. |
| `GetUserText` | `objectId: Guid * key: string * ?attachedToGeometry: bool = false -> string` | Returns user text stored on an object, fails if non-existing. |
| `GetUserTextKeys` | `objectId: Guid * ?attachedToGeometry: bool = false -> ResizeArray<string>` | Returns all user text keys stored on an object. |
| `HasUserText` | `objectId: Guid * key: string * ?attachedToGeometry: bool = false -> bool` | Checks if a User Text key is stored on an object. |
| `IsDocumentData` | `unit -> bool` | Checks if the current document contains user data. |
| `IsDocumentUserText` | `unit -> bool` | Checks if the current document contains user text. |
| `IsUserText` | `objectId: Guid -> int` | Checks if an object contains user text. |
| `SetDocumentData` | `section: string * entry: string * value: string -> unit` | Adds or sets a user data string to the current document. |
| `SetDocumentUserText` | `key: string * value: string * ?allowAllUnicode: bool = false -> unit` | Sets a user text stored in the document. |
| `SetUserText` | `objectId: Guid * key: string * value: string * ?attachToGeometry: bool = false * ?allowAllUnicode: bool = false -> unit` | Sets a user text stored on an object. |
| `SetUserText` | `objectIds: Guid seq * key: string * value: string * ?attachToGeometry: bool = false * ?allowAllUnicode: bool = false -> unit` | Sets or removes user text stored on multiple objects. |
| `TryGetUserText` | `objectId: Guid * key: string * ?attachedToGeometry: bool = false -> Option<string>` | Returns user text stored on an object, returns Option.None if non-existing. |

## UserInterface

From [Src/Scripting_UserInterface.fs](../Src/Scripting_UserInterface.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `BrowseForFolder` | `?folder: string = null * ?message: string = null * ?title: string = null -> string` | Display browse-for-folder dialog allowing the user to select a folder. |
| `CheckListBox` | `items: (string * bool) seq * ?message: string = null * ?title: string = null -> ResizeArray<string * bool>` | Displays a list of items in a checkable-style list dialog box. |
| `ComboListBox` | `items: string seq * ?message: string = null * ?title: string = null -> string` | Displays a list of items in a combo-style list box dialog. |
| `EditBox` | `?defaultValString: string = null * ?message: string = null * ?title: string = null -> string` | Display dialog prompting the user to enter a string. |
| `GetAngle` | `?point: Point3d = Point3d() * ?referencePoint: Point3d = Point3d() * ?defaultValAngleDegrees: float = 0.0 * ?message: string = null -> float` | Pause for user input of an angle. |
| `GetBoolean` | `message: string * items: (string * string * string) array * defaultVals: bool array -> ResizeArray<bool>` | Pauses for user input of one or more boolean values. |
| `GetBox` | `?mode: int = 0 * ?basePoint: Point3d = Point3d() * ?prompt1: string = null * ?prompt2: string = null * ?prompt3: string = null -> Point3d array` | Pauses for user input of a box. |
| `GetColor` | `?color: Color = Drawing.Color() -> Color` | Display the Rhino color picker dialog allowing the user to select an RGB color. |
| `GetCursorPos` | `unit -> Point3d * Point2d * Guid * Point2d` | Retrieves the cursor's position. |
| `GetDistance` | `?firstPt: Point3d = Point3d() * ?distance: float = 0.0 * ?firstPtMsg: string = "First distance point" * ?secondPtMsg: string = "Second distance point" -> float` | Pauses for user input of a distance. |
| `GetEdgeCurves` | `?message: string = "Select Edges" * ?minCount: int = 1 * ?maxCount: int = 0 * ?select: bool = false -> ResizeArray<Guid * Guid * Point3d>` | Prompt the user to pick one or more Surface or Polysurface edge Curves. |
| `GetInteger` | `?message: string = null * ?number: int = 2147482999 * ?minimum: int = 2147482999 * ?maximum: int = 2147482999 -> int` | Pauses for user input of a whole number. |
| `GetLayer` | `?title: string = "Select Layer" * ?layer: string = null * ?showNewButton: bool = false * ?showSetCurrent: bool = false -> string` | Displays dialog box prompting the user to select a layer. |
| `GetLayers` | `?title: string = "Select Layers" * ?showNewButton: bool = false -> ResizeArray<string>` | Displays a dialog box prompting the user to select one or more layers. |
| `GetLine` | `?mode: int = 0 * ?point: Point3d = Point3d() * ?message1: string = null * ?message2: string = null * ?message3: string = null -> Line` | Prompts the user to pick points that define a line. |
| `GetLinetype` | `?defaultValLinetype: string = null -> string` | Displays a dialog box prompting the user to select one line-type. |
| `GetMeshFaces` | `objectId: Guid * ?message: string = "Select Mesh Faces" * ?minCount: int = 1 * ?maxCount: int = 0 -> ResizeArray<int>` | Prompts the user to pick one or more Mesh faces. |
| `GetMeshVertices` | `objectId: Guid * ?message: string = "Select Mesh Vertices" * ?minCount: int = 1 * ?maxCount: int = 0 -> ResizeArray<int>` | Prompts the user to pick one or more Mesh vertices. |
| `GetPoint` | `?message: string = null * ?basePoint: Point3d = Point3d() * ?distance: float = 0.0 * ?inPlane: bool = false -> Point3d` | Pauses for user input of a point. |
| `GetPointOnCurve` | `curveId: Guid * ?message: string = "Pick Point On Curve" -> Point3d` | Pauses for user input of a point constrained to a Curve object. |
| `GetPointOnCurve` | `curveGeo: Curve * ?message: string = "Pick Point On Curve" -> Point3d` | Pauses for user input of a point constrained to a Curve object. |
| `GetPointOnMesh` | `meshId: Guid * ?message: string = "Pick Point On Mesh" -> Point3d` | Pauses for user input of a point constrained to a Mesh object. |
| `GetPointOnSurface` | `surfaceId: Guid * ?message: string = "Pick Point on Surface or Polysurface" -> Point3d` | Pauses for user input of a point constrained to a Surface or Polysurface object. |
| `GetPointOnSurface` | `surfaceGeo: GeometryBase * ?message: string = "Pick Point on Surface or Polysurface" -> Point3d` | Pauses for user input of a point constrained to a Surface or Polysurface object. |
| `GetPoints` | `?drawLines: bool = false * ?inPlane: bool = false * ?message1: string = null * ?message2: string = null * ?maxPoints: int = 0 -> ResizeArray<Point3d>` | Pauses for user input of one or more points. |
| `GetPolyline` | `?message1: string = null * ?message2: string = null * ?message3: string = null * ?message4: string = null * ?min: int = 2 * ?max: int = 0 -> Polyline` | Prompts the user to pick points that define a polyline. |
| `GetReal` | `?message: string = "Number" * ?number: float = 7e89 * ?minimum: float = 7e89 * ?maximum: float = 7e89 -> float` | Pauses for user input of a number. |
| `GetRectangle` | `?mode: int = 0 * ?basePoint: Point3d = Point3d() * ?prompt1: string = null * ?prompt2: string = null * ?prompt3: string = null -> Point3d * Point3d * Point3d * Point3d` | Pauses for user input of a rectangle. |
| `GetString` | `?message: string = null * ?defaultValString: string = null * ?strings: string seq = null -> string` | Pauses for user input of a string value. |
| `ListBox` | `items: IList<string> * ?message: string = null * ?title: string = null * ?defaultVal: string = null -> string` | Display a list of items in a list box dialog. |
| `MessageBox` | `message: string * ?buttons: int = 0 * ?title: string = "" -> int` | Displays a message box. |
| `MultiListBox` | `items: IList<string> * ?message: string = null * ?title: string = null * ?defaultVals: IList<string> = null -> string array` | Displays a list of items in a multiple-selection list box dialog. |
| `OpenFileName` | `?title: string = null * ?filter: string = null * ?folder: string = null * ?filename: string = null * ?extension: string = null -> string` | Displays file open dialog box allowing the user to enter a file name. |
| `OpenFileNames` | `?title: string = null * ?filter: string = null * ?folder: string = null * ?filename: string = null * ?extension: string = null -> string array` | Displays file open dialog box allowing the user to select one or more file names. |
| `PopupMenu` | `items: string seq * ?modes: int seq = null * ?point: Point3d = Point3d() * ?view: string = null -> int` | Display a context-style popup menu. |
| `Print` | `obj: 'T -> unit` | Prints any object to the console and to the Rhino command line using F#'s %A formatting. |
| `Print` | `txt: string -> unit` | Prints a string to the console and to the Rhino command line using F#'s %A formatting. |
| `PropertyListBox` | `items: IList<string> * values: IList<string> * ?message: string = null * ?title: string = null -> string array` | Displays list of items and their values in a property-style list box dialog. |
| `RealBox` | `?message: string = "" * ?defaultValNumber: float = 7e89 * ?title: string = "" * ?minimum: float = 7e89 * ?maximum: float = 7e89 -> float` | Display a dialog box prompting the user to enter a number. |
| `SaveFileName` | `?title: string = null * ?filter: string = null * ?folder: string = null * ?filename: string = null * ?extension: string = null -> string` | Display a save dialog box allowing the user to enter a file name. |
| `StringBox` | `?message: string = null * ?defaultValValue: string = null * ?title: string = null -> string` | Display a dialog box prompting the user to enter a string value. |
| `TextOut` | `message: string * ?title: string = null -> unit` | Display a text dialog box similar to the one used by the \_What command. |

## Utility

From [Src/Scripting_Utility.fs](../Src/Scripting_Utility.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `Angle` | `point1: Point3d * point2: Point3d * ?plane: Plane = Plane() -> float * float * float * float * float` | Measures the angle between two points. |
| `Angle2` | `line1: Line * line2: Line -> float * float` | Measures the angle between two lines. |
| `ClipboardText` | `unit -> string` | Gets the text string in the Windows clipboard. |
| `ClipboardText` | `text: string -> unit` | Sets a text string to the Windows clipboard. |
| `ColorAdjustLuma` | `rgb: Color * luminance: float * ?isScaleRelative: bool = false -> Color` | Changes the luminance of a red-green-blue value. |
| `ColorBlueValue` | `rgb: Color -> int` | Retrieves intensity value for the blue component of an RGB color. |
| `ColorGreenValue` | `rgb: Color -> int` | Retrieves intensity value for the green component of an RGB color. |
| `ColorHLSToRGB` | `hls: Color -> Color` | Converts colors from hue-luminance-saturation to RGB. |
| `ColorRedValue` | `rgb: Color -> int` | Retrieves intensity value for the red component of an RGB color. |
| `ColorRGBToHLS` | `rgb: Color -> ColorHSL` | Convert colors from RGB to HSL ( Hue, Saturation and Luminance). |
| `ContextIsGrasshopper` | `unit -> bool` | Return true if the script is being executed in a Grasshopper component. |
| `ContextIsRhino` | `unit -> bool` | Return true if the script is being executed in the context of Rhino (Rhino.Runtime.HostUtils.RunningInRhino). |
| `CreateColor` | `red: int * green: int * blue: int -> Color` | Creates a RGB color from red, green and blue values as Integers. |
| `CreateColor` | `red: byte * green: byte * blue: byte -> Color` | Creates a RGB color from red, green and blue values as Bytes. |
| `CreateInterval` | `start: float * ende: float -> Interval` | Converts input into a Rhino.Geometry.Interval. |
| `CreatePlane` | `origin: Point3d * ?xAxis: Vector3d = Vector3d() * ?yAxis: Vector3d = Vector3d() -> Plane` | Converts input into a Rhino.Geometry.Plane object if possible. |
| `CreatePoint` | `point: Point3d -> Point3d` | Returns the given Point3d. |
| `CreatePoint` | `point: Point3f -> Point3d` | Converts a Point3f into a Rhino.Geometry.Point3d. |
| `CreatePoint` | `x: float * y: float * z: float -> Point3d` | Converts x, y and z into a Rhino.Geometry.Point3d. |
| `CreatePoint` | `xyz: float seq -> Point3d` | Converts a sequence of exactly three floats into a Rhino.Geometry.Point3d. |
| `CreatePoint` | `point: string -> Point3d` | Parses a string into a Rhino.Geometry.Point3d. |
| `CreatePoint` | `objectId: Guid -> Point3d` | Gets the location of a point object in the document. |
| `CreatePoint` | `pointObject: PointObject -> Point3d` | Gets the location of a PointObject. |
| `CreatePoint` | `textDot: TextDot -> Point3d` | Gets the location of a TextDot. |
| `CreateVector` | `vector: Vector3d -> Vector3d` | Returns the given Vector3d. |
| `CreateVector` | `vector: Vector3f -> Vector3d` | Converts a Vector3f into a Rhino.Geometry.Vector3d. |
| `CreateVector` | `x: float * y: float * z: float -> Vector3d` | Converts x, y and z into a Rhino.Geometry.Vector3d. |
| `CreateVector` | `xyz: float seq -> Vector3d` | Converts a sequence of exactly three floats into a Rhino.Geometry.Vector3d. |
| `CreateVector` | `vector: string -> Vector3d` | Parses a string into a Rhino.Geometry.Vector3d. |
| `CreateVector` | `lineId: Guid -> Vector3d` | Gets the direction of a linear curve object in the document, from its start to its end point. |
| `CreateXform` | `xForm: 'a seq -> Transform` | Converts input into a Rhino.Geometry.Transform object if possible. |
| `CullDuplicateNumbers` | `numbers: float seq * ?tolerance: float = 0.0 -> ResizeArray<float>` | Removes duplicates from an array of numbers. |
| `CullDuplicatePoints` | `points: Point3d seq * ?tolerance: float = 0.0 -> Point3d array` | Removes duplicates from a list of 3D points. |
| `Distance` | `point1: Point3d * point2: Point3d -> float` | Measures distance between two 3D points. |
| `DistanceSquare` | `point1: Point3d * point2: Point3d -> float` | Measures Square distance between two 3D points. |
| `Polar` | `point: Point3d * angleDegrees: float * distance: float * ?plane: Plane = Plane() -> Point3d` | Returns 3D point that is a specified angle and distance from a 3D point. |
| `SimplifyArray` | `points: Point3d seq -> ResizeArray<float>` | Flattens an array of 3-D points into a one-dimensional list of real numbers. |
| `Sleep` | `milliseconds: int -> unit` | Suspends execution of a running script for the specified interval. |
| `SortPointList` | `points: Point3d seq * ?tolerance: float = 0.0 -> Point3d array` | Sorts list of points so they will be connected in a "reasonable" Polyline order. |
| `SortPoints` | `points: Point3d seq * ?ascending: bool = true * ?order: int = 0 -> Point3d seq` | Sorts the components of an array of 3D points. |
| `Str2Pt` | `point: string -> Point3d` | convert a formatted string value into a 3D point value. |

## Views

From [Src/Scripting_Views.fs](../Src/Scripting_Views.fs).

| Name | Signature | Summary |
|:--|:--|:--|
| `AddDetail` | `layoutName: string * corner1: Point2d * corner2: Point2d * ?title: string = null * ?projection: int = 1 -> Guid` | Add new detail view to an existing layout view. |
| `AddLayout` | `?title: string = null * ?width: float = 0.0 * ?height: float = 0.0 -> Guid * string` | Adds a new page layout view. |
| `AddNamedCPlane` | `cPlaneName: string * plane: Plane -> unit` | Adds new named construction plane to the document. |
| `AddNamedView` | `name: string * ?view: string = "" -> unit` | Adds a new named view to the document. |
| `CurrentDetail` | `layout: string -> string` | Returns the current detail view in a page layout view. |
| `CurrentDetail` | `layout: string * detail: string -> unit` | Changes the current detail view in a page layout view. |
| `CurrentView` | `unit -> string` | Returns the currently active view. |
| `CurrentView` | `view: string -> unit` | Sets the currently active view. |
| `DeleteNamedCPlane` | `name: string -> bool` | Removes a named construction Plane from the document. |
| `DeleteNamedView` | `name: string -> bool` | Removes a named view from the document. |
| `DetailLock` | `detailId: Guid -> bool` | Returns the projection locked state of a detail view-port rectangle. |
| `DetailLock` | `detailId: Guid * lock: bool -> unit` | Modifies the projection locked state of a detail. |
| `DetailLock` | `detailGeo: DetailView -> bool` | Returns the projection locked state of a detail view-port rectangle. |
| `DetailScale` | `detailId: Guid -> float` | Returns the scale of a detail object. |
| `DetailScale` | `detailId: Guid * modelLength: float * pageLength: float -> unit` | Modifies the scale of a detail object. |
| `DetailScale` | `detailGeo: DetailView -> float` | Returns the scale of a detail geometry object. |
| `IsDetail` | `layout: string * detail: string -> bool` | Checks if a detail view exists on a page layout view. |
| `IsLayout` | `layout: string -> bool` | Checks if a view is a page layout view. |
| `IsView` | `view: string -> bool` | Checks if the specified view exists. |
| `IsViewCurrent` | `view: string -> bool` | Checks if the specified view is the current, or active view. |
| `IsViewMaximized` | `?view: string = "" -> bool` | Checks if the specified view is maximized (enlarged so as to fill the entire Rhino window). |
| `IsViewPerspective` | `view: string -> bool` | Checks if the specified view's projection is set to perspective. |
| `IsViewTitleVisible` | `?view: string = "" -> bool` | Checks if the specified view's title window is visible. |
| `IsWallpaper` | `view: string -> bool` | Checks if the specified view contains a wallpaper image. |
| `MaximizeRestoreView` | `?view: string = "" -> unit` | Toggles a view's maximized/restore window state of the specified view. |
| `NamedCPlane` | `name: string -> Plane` | Returns the Plane geometry of the specified named construction Plane. |
| `NamedCPlanes` | `unit -> ResizeArray<string>` | Returns the names of all named construction Planes in the document. |
| `NamedViews` | `unit -> ResizeArray<string>` | Returns the names of all named views in the document. |
| `RenameView` | `oldTitle: string * newTitle: string -> unit` | Changes the title of the specified view. |
| `RestoreNamedCPlane` | `cplaneName: string * ?view: string = "" -> string` | Restores a named construction Plane to the specified view. |
| `RestoreNamedView` | `namedView: string * ?view: string = "" * ?restoreBitmap: bool = false -> unit` | Restores a named view to the specified view. |
| `RotateCamera` | `direction: int * angle: float * ?view: string = "" -> unit` | Rotates a perspective-projection view's camera. |
| `RotateView` | `direction: int * angle: float * ?view: string = "" -> unit` | Rotates a view. |
| `ShowGrid` | `view: string -> bool` | Get status of a view's construction Plane grid. |
| `ShowGrid` | `view: string * show: bool -> unit` | Shows or hides a view's construction Plane grid. |
| `ShowGridAxes` | `view: string -> bool` | Get status of a view's construction Plane grid axes. |
| `ShowGridAxes` | `view: string * show: bool -> unit` | Shows or hides a view's construction Plane grid axes. |
| `ShowViewTitle` | `view: string -> bool` | Get status of the title window of a view. |
| `ShowViewTitle` | `view: string * show: bool -> unit` | Shows or hides the title window of a view. |
| `ShowWorldAxes` | `view: string -> bool` | Get status of a view's world axis icon. |
| `ShowWorldAxes` | `view: string * show: bool -> unit` | Shows or hides a view's world axis icon. |
| `TiltView` | `direction: int * angle: float * ?view: string = "" -> unit` | Tilts a view by rotating the camera up vector. |
| `ViewCamera` | `view: string -> Point3d` | Returns the camera location of the specified view. |
| `ViewCamera` | `view: string * cameraLocation: Point3d -> unit` | Sets the camera location of the specified view. |
| `ViewCameraLens` | `view: string -> float` | Returns the 35mm camera lens length of the specified perspective projection view. |
| `ViewCameraLens` | `view: string * length: float -> unit` | Sets the 35mm camera lens length of the specified perspective projection view. |
| `ViewCameraPlane` | `?view: string = "" -> Plane` | Returns the orientation of a view's camera. |
| `ViewCameraTarget` | `view: string -> Point3d * Point3d` | Returns the camera and target positions of the specified view. |
| `ViewCameraTarget` | `view: string * camera: Point3d * target: Point3d -> unit` | Sets the camera and target positions of the specified view. |
| `ViewCameraUp` | `view: string -> Vector3d` | Returns the camera up direction of a specified. |
| `ViewCameraUp` | `view: string * upVector: Vector3d -> unit` | Sets the camera up direction of a specified. |
| `ViewCPlane` | `view: string -> Plane` | Return a view's construction Plane. |
| `ViewCPlane` | `view: string * plane: Plane -> unit` | Set a view's construction Plane. |
| `ViewDisplayMode` | `view: string -> string` | Return a view display mode. |
| `ViewDisplayMode` | `view: string * mode: string -> unit` | Set a view display mode. |
| `ViewDisplayModeId` | `name: string -> Guid` | Return id of a display mode given it's name. |
| `ViewDisplayModeName` | `modeId: Guid -> string` | Return name of a display mode given it's id. |
| `ViewDisplayModes` | `unit -> ResizeArray<string>` | Return list of display modes. |
| `ViewNames` | `?viewType: int = 0 -> ResizeArray<string>` | Return the names/titles, of all views in the document. |
| `ViewNearCorners` | `?view: string = "" -> Point3d * Point3d * Point3d * Point3d` | Return 3d corners of a view's near clipping Plane rectangle. |
| `ViewProjection` | `view: string -> int` | Return a view's projection mode. |
| `ViewProjection` | `view: string * mode: int -> unit` | Set a view's projection mode. |
| `ViewRadius` | `view: string -> float` | Returns the radius of a parallel-projected view. |
| `ViewRadius` | `view: string * radius: float * mode: bool -> unit` | Sets the radius of a parallel-projected view. |
| `ViewSize` | `?view: string = null -> int * int` | Returns the width and height in pixels of the specified view. |
| `ViewSpeedTest` | `?view: string = "" * ?frames: int = 100 * ?freeze: bool = true * ?direction: int = 0 * ?angleDegrees: float = 5.0 -> float` | Test's Rhino's display performance. |
| `ViewTarget` | `view: string -> Point3d` | Returns the target location of the specified view. |
| `ViewTarget` | `view: string * target: Point3d -> unit` | Sets the target location of the specified view. |
| `ViewTitle` | `viewId: Guid -> string` | Returns the name, or title, of a given view's identifier. |
| `Wallpaper` | `view: string -> string` | Returns the wallpaper bitmap of the specified view. |
| `Wallpaper` | `view: string * filename: string -> unit` | Sets the wallpaper bitmap of the specified view. |
| `WallpaperGrayScale` | `view: string -> bool` | Returns the gray-scale display option of the wallpaper bitmap in a specified view. |
| `WallpaperGrayScale` | `view: string * grayscale: bool -> unit` | Sets the gray-scale display option of the wallpaper bitmap in a specified view. |
| `WallpaperHidden` | `view: string -> bool` | Returns the visibility of the wallpaper bitmap in a specified view. |
| `WallpaperHidden` | `view: string * hidden: bool -> unit` | Sets the visibility of the wallpaper bitmap in a specified view. |
| `ZoomBoundingBox` | `boundingBox: BoundingBox * ?view: string = "" * ?all: bool = false -> unit` | Zooms to the extents of a specified bounding box in the specified view. |
| `ZoomExtents` | `?view: string = "" * ?all: bool = false -> unit` | Zooms to extents of visible objects in the specified view. |
| `ZoomSelected` | `?view: string = "" * ?all: bool = false -> unit` | Zoom to extents of selected objects in a view. |

## RhinoScriptingException

Rhino.Scripting Exception for Errors in script execution

| Name | Signature | Summary |
|:--|:--|:--|
| `new` | `s: string -> RhinoScriptingException` |  |
| `new` | `s: string * inner: exn -> RhinoScriptingException` | Keeps the original exception as InnerException, including its stack trace. |
| `RhinoScriptingException.FailIfFalse` | `s: 'a -> b: bool -> unit` |  |
| `RhinoScriptingException.Raise` | `msg: StringFormat<'a,'b> -> 'a` |  |
| `RhinoScriptingException.RaiseWith` | `inner: exn -> msg: StringFormat<'a,'b> -> 'a` | Like Raise, but keeps the given exception as InnerException. |

## RhinoUserInteractionException

Rhino.Scripting Exception for aborted user interactions, such as canceling to pick an object

| Name | Signature | Summary |
|:--|:--|:--|
| `new` | `s: string -> RhinoUserInteractionException` |  |
| `RhinoUserInteractionException.Raise` | `msg: StringFormat<'a,'b> -> 'a` |  |

## module RhinoScriptingUtils.PrettySettings

Exposes the settings used in toPretty pretty printing function

| Name | Signature | Summary |
|:--|:--|:--|
| `PrettySettings.thousandSeparator` | `char` | Set this to change the printing of floats larger than 10'000 |
| `PrettySettings.userZeroTolerance` | `float` | If the absolute value of a float is below this, display ±0.0 Default = 1e-24. |

## module RhinoScriptingUtils.PrettyFormat

| Name | Signature | Summary |
|:--|:--|:--|
| `PrettyFormat.float` | `x: float -> string` | Formatting with automatic precision e.g.: 0 digits behind comma if above 1000 if there are more than 15 zeros behind the comma just '~0.0' will be displayed if the value is smaller than PrettySettings.roundToZeroBelow '0.0' will be shown. this is Double.Epsilon by default |
| `PrettyFormat.single` | `x: float32 -> string` | Formatting with automatic precision e.g.: 0 digits behind comma if above 1000 |

## module RhinoScriptingUtils.AutoOpenRsUtils

| Name | Signature | Summary |
|:--|:--|:--|
| `Dict` | `= Dictionary<'K,'V>` |  |
| `( \|>! )` | `x: 'a -> f: ('a -> bool) -> 'a` | Apply function, like \|&gt; , but ignore result. |
| `( \|? )` | `a: 'T -> b: 'T -> 'T` | Null coalescing: Returns the value on the left unless it is null, then it returns the value on the right. |
| `AutoOpenRsUtils.deAt` | `CultureInfo` | German culture (used for float parsing). |
| `AutoOpenRsUtils.enUs` | `CultureInfo` | American English culture (used for float parsing). |
| `AutoOpenRsUtils.isNanOrInf` | `f: float<'T> -> bool` | Test is a floating point number (with Measure) is NaN (Not a Number) or Infinity. |
| `AutoOpenRsUtils.notNull` | `value: 'T -> bool` | Returns false if the value is null. |
| `AutoOpenRsUtils.parseFloatEnDe` | `x: string -> float` | First tries to parse float with En-Us CultureInfo (period as decimal separator). |
| `AutoOpenRsUtils.t1` | `a: 'a * 'b * 'c -> 'a` | Get first element of Triple (Tuple of three elements) |
| `AutoOpenRsUtils.t2` | `'a * b: 'b * 'c -> 'b` | Get second element of Triple (Tuple of three elements) |
| `AutoOpenRsUtils.t3` | `'a * 'b * c: 'c -> 'c` | Get third element of Triple (Tuple of three elements) |
| `AutoOpenRsUtils.toDegrees` | `radians: float<'T> -> float` | Converts Angles from Radians to Degrees. |
| `AutoOpenRsUtils.toRadians` | `degrees: float<'T> -> float` | Converts Angles from Degrees to Radians. |
| `AutoOpenRsUtils.tryParseFloatEnDe` | `x: string -> float option` | First tries to parse float with En-Us CultureInfo (period as decimal separator), if this fails tries to parse float with De-At CultureInfo (comma as decimal separator). |

## Array extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.Last` | `'T with get, set` | Get (or set) the last item in the Array. equal to this.[this.Length - 1] |

## HashSet extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.DoesNotContain` | `item: 'T -> bool` |  |

## ResizeArray extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.GetNeg` | `index: int -> 'T` | Gets an item in the ResizeArray by index. |

## RhinoSync

All methods in Rhino.Scripting.dll are thread safe and can be called from any thread. However concurrent writing to Rhino Object Tables might corrupt its state. This class provides a way to run all UI methods on the UI thread. It tries to use the Fesh Editor UI thread if it is running. If you are not running from the Fesh Editor, it will use Eto.Forms.Application.Instance.Invoke.

| Name | Signature | Summary |
|:--|:--|:--|
| `RhinoSync.ClearLog` | `unit -> unit` | Clears the Fesh log window and the Rhino command history window. |
| `RhinoSync.DoSync` | `func: (unit -> 'T) -> 'T` | Evaluates a function on UI Thread. |
| `RhinoSync.DoSyncRedraw` | `func: (unit -> 'T) -> 'T` | Evaluates a function on UI Thread. |
| `RhinoSync.DoSyncRedrawHideEditor` | `func: (unit -> 'T) -> 'T` | Evaluates a function on UI Thread. |
| `RhinoSync.HideEditor` | `unit -> unit` | Hide the WPF Window of currently running Fesh Editor. |
| `RhinoSync.LogErrors` | `bool with get, set` | Set to false to disable the logging of errors to RhinoApp.WriteLine and the error stream (eprintfn). |
| `RhinoSync.PrettyFormatters` | `ResizeArray<(obj -> string option)>` | This is a collection of pretty formatters that the library Pretty is using if present. |
| `RhinoSync.PrintColor` | `r: int -> g: int -> b: int -> s: string -> unit` | Prints in both RhinoApp.WriteLine and Console.WriteLine, or Fesh Editor with color if running. |
| `RhinoSync.PrintnColor` | `r: int -> g: int -> b: int -> s: string -> unit` | Prints in both RhinoApp.WriteLine and Console.WriteLine, or Fesh Editor with color if running. |
| `RhinoSync.ShowEditor` | `unit -> unit` | Show the WPF Window of currently running Fesh Editor. |
| `RhinoSync.SyncContext` | `SynchronizationContext with get, set` | The SynchronizationContext of the currently Running Rhino Instance, This SynchronizationContext is loaded via reflection from the Fesh.Rhino plugin. |

## Point3d extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.Pretty` | `string` | Like the ToString function but with appropriate precision formatting |

## Point3f extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.Pretty` | `string` | Like the ToString function but with appropriate precision formatting |

## Vector3d extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.Pretty` | `string` | Like the ToString function but with appropriate precision formatting |

## Vector3f extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.Pretty` | `string` | Like the ToString function but with appropriate precision formatting |

## Line extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.Pretty` | `string` | Like the ToString function but with appropriate precision formatting |

## Transform extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.Pretty` | `string` | returns a string showing the Transformation Matrix in an aligned 4x4 grid |

## Plane extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.Pretty` | `string` | returns a string showing the Plane origin and axes |

## BoundingBox extensions

| Name | Signature | Summary |
|:--|:--|:--|
| `.Pretty` | `string` | returns a string showing the BoundingBox size and extents |

## ObjectFilterEnum

An integer Enum of Object types to be used in object selection functions of Rhino.Scripting. Don't create an instance, use the instance in Rhino.Scripting.RhinoScriptSyntax.Filter

| Name | Signature | Summary |
|:--|:--|:--|
| `.AllObjects` | `int` | returns 0 |
| `.Annotation` | `int` | returns 512, for Text, leaders, and dimension lines |
| `.Cage` | `int` | returns 134217728 |
| `.ClippingPlane` | `int` | returns 536870912 |
| `.Curve` | `int` | returns 4 |
| `.Detail` | `int` | returns 32768, for detail view objects |
| `.Extrusion` | `int` | returns 1073741824 |
| `ObjectFilterEnum.GetFilterEnum` | `i: int -> ObjectType` | A helper function to get a DocObjects.ObjectType Enum from an integer |
| `.Grip` | `int` | returns 16384 |
| `.Hatch` | `int` | returns 65536 |
| `.Instance` | `int` | returns 4096, for block instances |
| `.Light` | `int` | returns 256 |
| `.Mesh` | `int` | returns 32 |
| `.Morph` | `int` | returns 131072 |
| `.Phantom` | `int` | returns 268435456 |
| `.Point` | `int` | returns 1 |
| `.PointCloud` | `int` | returns 2 |
| `.PolySurface` | `int` | returns 16 |
| `.SubD` | `int` | returns 262144 |
| `.Surface` | `int` | returns 8 |
| `.Textdot` | `int` | returns 8192 |
