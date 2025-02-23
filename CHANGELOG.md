# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.8.1] - 2025-02-23
## Changed
- build and deploy with GitHub Actions

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

[Unreleased]: https://github.com/goswinr/Str/compare/0.8.0...HEAD
[0.8.0]: https://github.com/goswinr/Str/compare/0.7.0...0.8.0
[0.7.0]: https://github.com/goswinr/Str/compare/0.6.2...0.7.0
[0.6.2]: https://github.com/goswinr/Str/compare/0.6.1...0.6.2
[0.6.1]: https://github.com/goswinr/Str/compare/0.6.0...0.6.1
[0.6.0]: https://github.com/goswinr/Str/compare/0.5.1...0.6.0
[0.5.1]: https://github.com/goswinr/Str/compare/0.4.0...0.5.1
[0.4.0]: https://github.com/goswinr/Str/compare/0.3.0...0.4.0
[0.3.0]: https://github.com/goswinr/Str/compare/0.2.0...0.3.0
[0.2.0]: https://github.com/goswinr/Str/releases/tag/0.2.0

<!--
use to get tag dates:
git log --tags --simplify-by-decoration --pretty="format:%ci %d"

-->