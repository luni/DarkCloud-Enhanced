# Modern Host Feature Parity

This matrix tracks the migration of mod features from the legacy WinForms host (`DarkCloudEnhancedMod`) to the modern WinForms host (`DarkCloud.App.WinForms`). It is updated as features are moved in Phase 14.3.

## Matrix

| Feature | Legacy status | Modern status | Automated tests | Manual validation | Known differences |
|---|---|---|---|---|---|
| Apply Changes | Implemented in `DarkCloudEnhancedMod.ModWindowGameSessionObserver` via `ApplyChangesFeature` and legacy `ApplyChangesService` | Implemented in `DarkCloud.App.WinForms.ModernHostGameSessionObserver` via shared `ApplyChangesFeature` and `DarkCloud.Memory.Windows.ApplyChangesService` | `ApplyChangesFeatureTests` (Core/Integration), `DarkCloud.Memory.Windows.IntegrationTests` contract suite | Run both hosts and verify weapon stats and shop prices update once on new game | Uses the same `WeaponBalanceService`, `WeaponStatService`, and `Shop.UpdateShopPrices` logic. The shared service reads/writes through the static `Memory` backend, which both hosts initialize via `ModWindowGameMemoryProvider`. |
| Town Character | Implemented in `TownCharacterFeature` wrapping `TownCharacter.MainScript` | Not yet migrated | `TownCharacterFeature` integration tests in legacy host | Verify ally switching, dialogues, and side-quest state in town | Domain logic (`TownCharacter`) still lives in the legacy host and is not accessible to the modern target. Migration is blocked on extracting `TownCharacter` to a shared layer. |
| Dungeon | Implemented in `DungeonFeature` wrapping `Dungeon.InsideDungeonThread` | Not yet migrated | `DungeonFeature` integration tests in legacy host | Verify dungeon floor scripts, escape powder, miniboss stamina, etc. | Domain logic (`Dungeon`) still lives in the legacy host. Migration is blocked on extracting the remaining dungeon orchestration to `DarkCloud.Core`/`DarkCloud.Memory.Windows`. |
| Weapon Reroll | Implemented in `WeaponsFeature` wrapping `Weapons.RerollWeaponSpecialAttributes` | Implemented in `DarkCloud.Memory.Windows.WeaponsFeature` using `WeaponRerollService` | `WeaponsFeature` integration tests in both hosts (`FeatureModuleTests`) | Verify weapon special-attribute rerolling | Domain logic extracted to `WeaponRerollService` in `DarkCloud.Memory.Windows` and reused by both hosts. Legacy `Weapons.RerollWeaponSpecialAttributes` removed. |
| Status Log | N/A (legacy host logs directly to `ModWindow`) | Implemented as `StatusLogFeature` in `DarkCloud.App.WinForms` | `StatusLogFeature` exercised by modern host integration tests | Verify log output appears in the modern host UI | Legacy host does not have a dedicated `StatusLogFeature`; logging is inline. This is a modern-only diagnostic feature. |

## Legend

- **Legacy status**: Where the feature is implemented in `DarkCloudEnhancedMod`.
- **Modern status**: Where the feature is implemented in `DarkCloud.App.WinForms`, or a note that it is not yet migrated.
- **Automated tests**: Existing test coverage and where it runs.
- **Manual validation**: Suggested manual checks once both hosts are run side by side.
- **Known differences**: Divergences between the two implementations, including technical debt or blockers.

## Blockers

The remaining features (`Town Character` and `Dungeon`) depend on legacy static classes (`TownCharacter` and `Dungeon`) that are still in `DarkCloudEnhancedMod`. Migrating them requires either:

1. Extracting the domain logic into `DarkCloud.Core` services with `DarkCloud.Memory.Windows` memory layouts, or
2. Moving the static orchestration classes to `DarkCloud.Memory.Windows` so the modern host can reference them.

`Apply Changes` and `Weapon Reroll` have been migrated because their logic could be extracted to shared services (`ApplyChangesService` and `WeaponRerollService`) without taking dependencies on the legacy UI or player-static classes.
