# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- RhinoScriptingException constructor with inner exception, and RhinoScriptingException.RaiseWith
### Changed
- Breaking: CoerceColor reads a tuple of 4 integers as (red, green, blue, alpha), like in RhinoPython. It was (alpha, red, green, blue) before
- Add, AddPoints and AddLeader raise an exception if adding to the document fails, instead of returning Guid.Empty
- CoerceXform only accepts a 4x4 matrix
- FxrangePython and FrangePython return an empty range if the stop value cannot be reached, like in Python
- Without the Fesh editor, RhinoSync always uses Eto.Forms.Application.Instance.Invoke to get to the UI thread
### Fixed
- TryCoerce functions never throw an exception, they return None on empty or unknown Guids
- GetLinetype pre-selects the defaultValLinetype (Rhino 8 only)
- RhinoSync.DoSyncRedraw and DoSyncRedrawHideEditor restore redraw and the Fesh editor even if an exception is raised
- Clear error message if there is no active Rhino document (e.g. on Mac), event handlers are not added repeatedly anymore
- Thread safe initialization of RhinoSync
- Pretty printing of numbers above 1000 uses the invariant culture
- Build fails if combineIntoOneFile.fsx does not include all Scripting_*.fs files

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

[Unreleased]: https://github.com/goswinr/Rhino.Scripting/compare/0.13.0...HEAD
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
