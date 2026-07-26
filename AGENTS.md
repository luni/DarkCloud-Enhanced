# Project Notes

## Build

```bash
dotnet build "DarkCloud-Enhanced.sln"
```

## Test

```bash
dotnet test "DarkCloud-Enhanced.sln" --no-build
```

## Verification

- Build with `dotnet build`.
- Run `dotnet test` for Core, Memory Abstractions, and integration test projects.
- The WinForms project targets .NET Framework and builds on Linux with Mono/.NET reference assemblies.

## Domain migration notes

- Place new domain abstractions in `src/DarkCloud.Core` and keep legacy WinForms/process-specific code in `src/DarkCloudEnhancedMod`.
- `DarkCloud.Core` targets .NET Standard 2.0 with `LangVersion` 7.3; avoid capturing `ref`/`out` parameters in lambdas.
- The legacy `Player` static methods are being preserved as thin facades that delegate to `DarkCloud.Core.Players` services and repositories.
- New inventory abstractions live in `DarkCloud.Core/Inventory`; memory layouts remain in `DarkCloudEnhancedMod`.
- New mod-feature abstractions live in `DarkCloud.Core/Features`; legacy feature threads in `DarkCloudEnhancedMod` should be migrated to `IModFeature` implementations and driven by `ModFeatureRunner`.

## Modern host (Phase 14)

- Process-memory and platform code lives in `src/DarkCloud.Memory.Windows` (shared `netstandard2.0`) so both the legacy and modern hosts can use it.
- `FileLockModInstanceProvider` lives in `DarkCloud.Core.Session`.
- The modern WinForms host is `src/DarkCloud.App.WinForms` and targets `net8.0-windows` with `<EnableWindowsTargeting>true</EnableWindowsTargeting>` so it builds on Linux too.
- The legacy `DarkCloudEnhancedMod` still targets .NET Framework and builds on Linux; both hosts share `DarkCloud.Memory.Windows` and `DarkCloud.Core`.
