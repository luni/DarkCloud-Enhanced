# Migration Status

This document tracks progress through the migration plan defined in `migration.md`. Update it in every migration pull request.

## Current phase

Phase 6: Add platform integration tests

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

## In progress

- [ ] Phase 6 — Add platform integration tests

## Next

- Phase 7 — Windows integration tests
- Phase 8: Extract session state machine
- Phase 9: Decouple UI notifications

## Known blockers

- None

## Deferred work

- WinForms redesign
- Modern Windows host migration
- Emulator-level public CI
- Address-data generator and PNACH generation

---

*Last updated: 2026-07-25*
