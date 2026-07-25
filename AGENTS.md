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
