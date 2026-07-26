# Modern Host Feature Parity

This matrix tracks the migration of mod features from the legacy WinForms host (`DarkCloudEnhancedMod`) to the modern WinForms host (`DarkCloud.App.WinForms`). Phase 14.3 is closed: all legacy feature scripts have been moved to `DarkCloud.Memory.Windows` and are wired into both hosts.

## Matrix

| Feature | Legacy status | Modern status | Automated tests | Manual validation | Known differences |
|---|---|---|---|---|---|
| Apply Changes | Implemented in `DarkCloudEnhancedMod.ModWindowGameSessionObserver` via `ApplyChangesFeature` and legacy `ApplyChangesService` | Implemented in `DarkCloud.App.WinForms.ModernHostGameSessionObserver` via shared `ApplyChangesFeature` and `DarkCloud.Memory.Windows.ApplyChangesService` | `ApplyChangesFeatureTests` (Core/Integration), `DarkCloud.Memory.Windows.IntegrationTests` contract suite | Run both hosts and verify weapon stats and shop prices update once on new game | Uses the same `WeaponBalanceService`, `WeaponStatService`, and `Shop.UpdateShopPrices` logic. The shared service reads/writes through the static `Memory` backend, which both hosts initialize via `ModWindowGameMemoryProvider`. |
| Town Character | Implemented in `TownCharacterFeature` wrapping `TownCharacter.MainScript` | Implemented in `DarkCloud.Memory.Windows.TownCharacterFeature` driving the shared `TownCharacter.MainScript` | `TownCharacterFeature` integration tests in both hosts (`FeatureModuleTests`) | Verify ally switching, dialogues, and side-quest state in town | `TownCharacter` and its dependency graph (`Player`, `Dialogues`, `DailyShopItem`, etc.) were moved to `DarkCloud.Memory.Windows` so both hosts share the same script. |
| Dungeon | Implemented in `DungeonFeature` wrapping `Dungeon.InsideDungeonThread` | Implemented in `DarkCloud.Memory.Windows.DungeonFeature` driving the shared `Dungeon.InsideDungeonThread` | `DungeonFeature` integration tests in both hosts (`FeatureModuleTests`) | Verify dungeon floor scripts, escape powder, miniboss stamina, etc. | `Dungeon` and its dependency graph (`Dayuppy`, `CustomEffects`, `Enemies`, `MiniBoss`, etc.) were moved to `DarkCloud.Memory.Windows` so both hosts share the same script. |
| Weapon Reroll | Implemented in `WeaponsFeature` wrapping `Weapons.RerollWeaponSpecialAttributes` | Implemented in `DarkCloud.Memory.Windows.WeaponsFeature` using `WeaponRerollService` | `WeaponsFeature` integration tests in both hosts (`FeatureModuleTests`) | Verify weapon special-attribute rerolling | Domain logic extracted to `WeaponRerollService` in `DarkCloud.Memory.Windows` and reused by both hosts. Legacy `Weapons.RerollWeaponSpecialAttributes` removed. |
| Status Log | N/A (legacy host logs directly to `ModWindow`) | Implemented as `StatusLogFeature` in `DarkCloud.App.WinForms` | `StatusLogFeature` exercised by modern host integration tests | Verify log output appears in the modern host UI | Legacy host does not have a dedicated `StatusLogFeature`; logging is inline. This is a modern-only diagnostic feature. |

## Legend

- **Legacy status**: Where the feature is implemented in `DarkCloudEnhancedMod`.
- **Modern status**: Where the feature is implemented in `DarkCloud.App.WinForms`, or a note that it is not yet migrated.
- **Automated tests**: Existing test coverage and where it runs.
- **Manual validation**: Suggested manual checks once both hosts are run side by side.
- **Known differences**: Divergences between the two implementations, including technical debt or blockers.

## Blockers

Feature parity is complete. The remaining step for Phase 14 is to retire the legacy host (`Pull request 14.4`), which requires:

1. Validating supported Windows environments with the modern host.
2. Updating release packaging to produce the modern host artifacts.
3. Writing rollback instructions.
4. Shipping at least one stable release that has used the modern host successfully.

Until those conditions are met, `DarkCloudEnhancedMod` remains in the solution as a WinForms shell that consumes the same `DarkCloud.Memory.Windows` feature modules as `DarkCloud.App.WinForms`.
