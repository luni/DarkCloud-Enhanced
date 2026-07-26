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
- Supported environments, build profiles, and CI artifacts for both hosts are documented in `docs/supported-environments.md`.
- The shared memory contract tests run for both the legacy host (`DarkCloudEnhancedMod.IntegrationTests`) and the modern host's memory layer (`DarkCloud.Memory.Windows.IntegrationTests`).
- Feature parity between hosts is tracked in `docs/modern-host-parity.md`.
- `ApplyChangesFeature` lives in `DarkCloud.Core.Features`; its implementation (`ApplyChangesService`) lives in `DarkCloud.Memory.Windows` and is shared by both hosts.
- `WeaponsFeature` lives in `DarkCloud.Memory.Windows` and uses `WeaponRerollService`; the legacy `Weapons.RerollWeaponSpecialAttributes` method has been removed.
- Shared utilities moved to support feature migration: `ThreadingHelper` in `DarkCloud.Core.Threading`, `MainMenuThread` and `Items` in `DarkCloud.Memory.Windows`.
- Phase 14.3 is closed with `ApplyChanges` and `Weapon Reroll` migrated to the modern host. `TownCharacter` and `Dungeon` remain blocked on the legacy static script graph (Player, Dialogues, Dayuppy, CustomEffects, Enemies, MiniBoss, SideQuestManager, Resources, etc.) and are deferred.
