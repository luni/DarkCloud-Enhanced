# Migration Status

This document tracks progress through the migration plan defined in `migration.md`. Update it in every migration pull request.

## Current phase

Phase 2: Introduce memory abstractions

## Completed

- [x] Baseline documented in `docs/migration-baseline.md`
- [x] Dependency map, call-site inventories, and WinForms coupling inventory produced
- [x] Existing validation commands executed and results recorded
- [x] First memory-abstraction pull request proposed
- [x] Phase 1.1 — Split `.github/workflows/build-and-release.yml` into `ci.yml` and `release.yml`
- [x] Phase 1.2 — Add `packages.lock.json`, locked restore, and test-result artifacts
- [x] Phase 1.3 — Add CodeQL, dependency review, and Dependabot

## In progress

- [ ] Phase 2.1 — Create `DarkCloud.Memory.Abstractions`

## Next

- Phase 2.2 — Implement `InMemoryGameMemory` with boundary tests
- Phase 2.3 — Create `LegacyProcessGameMemory` adapter

## Known blockers

- None

## Deferred work

- WinForms redesign
- Modern Windows host migration
- Emulator-level public CI
- Address-data generator and PNACH generation

---

*Last updated: 2026-07-25*
