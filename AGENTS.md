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
- The WinForms project targets `net8.0-windows` with `<EnableWindowsTargeting>true</EnableWindowsTargeting>` so it builds on Linux too.

## Domain migration notes

- Place new domain abstractions in `src/DarkCloud.Core` and keep process-specific code in `src/DarkCloud.Memory.Windows`.
- `DarkCloud.Core` targets .NET Standard 2.0 with `LangVersion` 7.3; avoid capturing `ref`/`out` parameters in lambdas.
- The legacy `Player` static methods are preserved as thin facades that delegate to `DarkCloud.Core.Players` services and repositories.
- New inventory abstractions live in `DarkCloud.Core/Inventory`; memory layouts remain in the `DarkCloudEnhancedMod` namespace inside `DarkCloud.Memory.Windows`.
- New mod-feature abstractions live in `DarkCloud.Core/Features`; legacy feature threads are replaced by `IModFeature` implementations driven by `ModFeatureRunner`.

## Modern host

- The legacy `DarkCloudEnhancedMod` WinForms host has been retired; `src/DarkCloud.App.WinForms` is the only remaining host.
- Process-memory and platform code lives in `src/DarkCloud.Memory.Windows` (shared `netstandard2.0`) and is used by the modern host.
- `FileLockModInstanceProvider` lives in `DarkCloud.Core.Session`.
- The modern WinForms host is `src/DarkCloud.App.WinForms` and targets `net8.0-windows` with `<EnableWindowsTargeting>true</EnableWindowsTargeting>` so it builds on Linux too.
- Runtime resources (`Resources/PNACH/*.pnach`, `Resources/pcsx2_offsetreader.dll`, and the application icon) are packaged by `DarkCloud.App.WinForms`.
- Supported environments, build profiles, and CI artifacts for the modern host are documented in `docs/supported-environments.md`.
- The integration test project `tests/DarkCloudEnhancedMod.IntegrationTests` now tests the modern host and shared memory layer.
- `ApplyChangesFeature` lives in `DarkCloud.Core.Features`; its implementation (`ApplyChangesService`) lives in `DarkCloud.Memory.Windows`.
- `WeaponsFeature` lives in `DarkCloud.Memory.Windows` and uses `WeaponRerollService`; the legacy `Weapons.RerollWeaponSpecialAttributes` method has been removed.
- Shared utilities moved to support migration: `ThreadingHelper` in `DarkCloud.Core.Threading`, `MainMenuThread` and `Items` in `DarkCloud.Memory.Windows`.
- `ConsoleModLogger` and `Resources` live in `DarkCloud.Memory.Windows.*` namespaces.
- Phase 14 is closed: parity is achieved and the legacy host has been retired from the solution. `ModernHostGameSessionObserver` lives in `DarkCloud.Memory.Windows`, `StatusLogFeature` lives in `DarkCloud.Core.Features`, and the modern host in `DarkCloud.App.WinForms` loads `JsonModConfigurationStore` and wires the observer. The runtime script graph (`TownCharacter`, `Dungeon`, `Player`, `Dialogues`, `Dayuppy`, `CustomEffects`, `Enemies`, `MiniBoss`, `SideQuestManager`, `Items`, `Shop`, `Weapons`, `Memory`, `Addresses`, etc.) still uses the `DarkCloudEnhancedMod` namespace while living in `DarkCloud.Memory.Windows`; renaming or extracting these into `DarkCloud.Core` is deferred.
