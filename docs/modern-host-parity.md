# Modern Host Feature Parity

Feature parity between the legacy `DarkCloudEnhancedMod` WinForms host and the modern `DarkCloud.App.WinForms` host was reached in Phase 14.3. The legacy host was retired from the solution in Phase 14.4; `DarkCloud.App.WinForms` is now the only host.

## Feature ownership

| Feature | Implementation | Automated tests |
|---|---|---|
| Apply Changes | `DarkCloud.Core.Features.ApplyChangesFeature` with `DarkCloud.Memory.Windows.ApplyChangesService` | `ApplyChangesFeatureTests` |
| Town Character | `DarkCloud.Memory.Windows.TownCharacterFeature` wrapping `TownCharacter.MainScript` | `FeatureModuleTests` |
| Dungeon | `DarkCloud.Memory.Windows.DungeonFeature` wrapping `Dungeon.InsideDungeonThread` | `FeatureModuleTests` |
| Weapon Reroll | `DarkCloud.Memory.Windows.WeaponsFeature` using `WeaponRerollService` | `FeatureModuleTests` |
| Status Log | `DarkCloud.Core.Features.StatusLogFeature` | Modern host integration tests |
| Session boot / reset | `DarkCloud.Memory.Windows.ModernHostGameSessionObserver` | `ModernHostGameSessionObserverTests` |

## Notes

- The modern host calls `TownCharacter.InitializeCharacterOffsetValues()` on first boot, `Dialogues.IntroTextAtNorune()` for new games, and sets `MainMenuThread.userMode` on active state transitions, matching legacy behavior.
- Runtime resources (`Resources/PNACH/*.pnach`, `Resources/pcsx2_offsetreader.dll`, and the application icon) are now packaged with `DarkCloud.App.WinForms`.
- `JsonModConfigurationStore` was moved to `DarkCloud.App.WinForms.Configuration` and is loaded by `MainForm`; feature enablement and the poll interval are passed to `ModernHostGameSessionObserver`.
- The remaining `DarkCloudEnhancedMod` namespace static script graph (`TownCharacter`, `Dungeon`, `Dialogues`, `Player`, `Items`, `Shop`, `Weapons`, `Memory`, `Addresses`, etc.) still lives in `DarkCloud.Memory.Windows`; renaming or extracting these into `DarkCloud.Core` is deferred work.
