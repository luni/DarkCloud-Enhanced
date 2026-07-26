# Migration Status

This document tracks progress through the migration plan defined in `migration.md`. Update it in every migration pull request.

## Current phase

Phase 14: Introduce the modern Windows host (in progress)

## Completed

- [x] Baseline documented in `docs/migration-baseline.md`
- [x] Dependency map, call-site inventories, and WinForms coupling inventory produced
- [x] Existing validation commands executed and results recorded
- [x] First memory-abstraction pull request proposed
- [x] Phase 1.1 — Split `.github/workflows/build-and-release.yml` into `ci.yml` and `release.yml`
- [x] Phase 1.2 — Add `packages.lock.json`, locked restore, and test-result artifacts
- [x] Phase 1.3 — Add CodeQL, dependency review, and Dependabot
- [x] Phase 2.1 — Create `DarkCloud.Memory.Abstractions` with `IGameMemory` and typed reader/writer
- [x] Phase 2.2 — Implement `InMemoryGameMemory` with boundary tests
- [x] Phase 2.3 — Create `LegacyProcessGameMemory` adapter
- [x] Phase 3.1 — Extract `IAddressTranslator` and `RegionAddressTranslator`
- [x] Phase 3.2 — Add symbolic `GameAddress` pilot in `RegionAddresses.DetectRegion`
- [x] Phase 4.1 — Define `data/addresses.json` schema and pilot addresses
- [x] Phase 4.2 — Create `AddressGenerator` and `GameAddresses.g.cs`
- [x] Phase 5 — Add shared `DarkCloud.Memory.ContractTests`
- [x] Phase 6 — Add `IMemoryBackend` abstraction, `SnapshotMemoryBackend`, and `DarkCloudEnhancedMod.IntegrationTests` covering Linux synthetic and snapshot-based scenarios
- [x] Phase 7 — Add Windows synthetic process target and `DarkCloud.Memory.Windows.IntegrationTests`
- [x] Phase 8 — Extract session state machine
- [x] Phase 9.1 — Introduce `IModStatusSink` and `WinFormsModStatusSink` adapter
- [x] Phase 9.2 — Remove `ModWindow.NightlyVersionCheck` from `Memory.Initialize` and route all UI updates through `IModStatusSink`
- [x] Phase 10.1 — Extract player presence/identity and character state (HP/MaxHP/defense/thirst/max thirst/status) into `DarkCloud.Core/Players` repository and service
- [x] Phase 10.1 review fixes — `PlayerStateRepository` requires layout, `PlayerPresenceService` reads directly, `SetMaxHp` validates range, `GetStatus` validates status flags
- [x] Phase 10.2 pilot — Extract active item and bag capacity logic into `DarkCloud.Core/Inventory` (`InventoryItem`, `InventorySnapshot`, `IInventoryRepository`, `InventoryRepository`, `IInventoryService`, `InventoryService`) with tests and thin `Player.Inventory` facades
- [x] Phase 10.2 completion — Extract bag items, bag weapons, and attachments into `DarkCloud.Core/Inventory` (`IInventoryMemoryLayout`, `InventoryRepository`, `InventoryService`) and update `Player.Inventory` facades
- [x] Phase 10.2 review fixes — `PlayerPresenceService` uses `IPlayerPresenceRepository`/`IPlayerPresenceMemoryLayout`, `ModFeatureRunner` isolates feature faults and shuts down cleanly, `InventoryItem.IsEmpty` handles zero IDs, `InventoryRepository` validates ranges before casting
- [x] Phase 11 pilot — Add `DarkCloud.Core/Features` abstractions (`IModFeature`, `GameFeatureContext`, `GameSnapshot`, `ModFeatureRunner`) with tests
- [x] Phase 11 first feature — Convert `ApplyNewChanges` to `ApplyChangesFeature` `IModFeature` and wire `ModFeatureRunner` into `ModWindowGameSessionObserver`
- [x] Phase 11 completion — Migrate remaining feature threads to `IModFeature` modules
- [x] Phase 12 — Add `IModLogger` abstraction, `NullModLogger`/`ConsoleModLogger`, and log session transitions, feature init/shutdown, process attach, and errors
- [x] Phase 13 — Add `ModConfiguration`, `IModConfigurationStore`, `JsonModConfigurationStore` with versioning/unknown-key preservation, and wire configuration into `ModWindow`/`GameSessionRunner`/`ModWindowGameSessionObserver`
- [x] Phase 10.4 pilot — Extract dungeon gate-key, back-floor key, event-floor, enemy key-drop, and bone-door services into `DarkCloud.Core/Dungeon`
- [x] Phase 10.3 — Extract weapons domain logic (special-attribute rolls, synth-sphere upgrade rules, balance table) into `DarkCloud.Core/Weapons`
- [x] Phase 10.4 — Extract remaining dungeon domain behavior into `DarkCloud.Core/Dungeon`: Ungaga door/swap, clown, escape powder, miniboss stamina, Sword of Zeus, side-quest state, floor selection, spawn detection, mini-boss message, active item usage, weapon level-up, monster-kill quests, and Samba/Mayor side-quest challenges.
- [x] Phase 14.1 — Create `DarkCloud.App.WinForms` modern host skeleton, extract shared `DarkCloud.Memory.Windows` process-memory library, and wire pilot session runner, status display, and `StatusLogFeature`.
- [x] Phase 14.2 — Add build/CI profiles for legacy and modern hosts, document supported environments, and run shared memory contract suites for both host test projects.

## In progress

- Phase 14.3 — Reach feature parity; first batch migrated `ApplyChangesFeature` to `DarkCloud.Core.Features` and `ApplyChangesService` to `DarkCloud.Memory.Windows`. Remaining features blocked on legacy domain extraction.

## Next

- Phase 14.4 — Retire the legacy host
- Phase 15 — Add optional emulator-level system tests

## Known blockers

- None

## Deferred work

- WinForms redesign
- Modern Windows host migration
- Emulator-level public CI
- Address-data generator and PNACH generation

---

## Notes

- `Dungeon.cs` still contains `InsideDungeonThread`, which is the legacy feature-thread orchestrator that wires the newly extracted Core dungeon services together with the remaining `CustomEffects` weapon-specific feature threads. This orchestrator is intentionally left in the legacy host layer; migrating it will happen as part of the broader feature-thread/host modernization rather than as additional Phase 10.4 domain extraction.

*Last updated: 2026-07-26*
