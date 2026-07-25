# Migration Status

This document tracks progress through the migration plan defined in `migration.md`. Update it in every migration pull request.

## Current phase

Phase 10: Extract domain logic (in progress)

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

## In progress

- None

## Next

- Phase 10.2 — Continue extracting domain logic (weapons, items, dungeons, etc.)
- Phase 11: Replace feature threads with modules

## Known blockers

- None

## Deferred work

- WinForms redesign
- Modern Windows host migration
- Emulator-level public CI
- Address-data generator and PNACH generation

---

*Last updated: 2026-07-25*
