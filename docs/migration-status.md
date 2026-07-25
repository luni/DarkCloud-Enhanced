# Migration Status

This document tracks progress through the migration plan defined in `migration.md`. Update it in every migration pull request.

## Current phase

Phase 1.3: Add repository security automation

## Completed

- [x] Baseline documented in `docs/migration-baseline.md`
- [x] Dependency map, call-site inventories, and WinForms coupling inventory produced
- [x] Existing validation commands executed and results recorded
- [x] First memory-abstraction pull request proposed
- [x] Phase 1.1 — Split `.github/workflows/build-and-release.yml` into `ci.yml` and `release.yml`
- [x] Phase 1.2 — Add `packages.lock.json`, locked restore, and test-result artifacts

## In progress

- [ ] Phase 1.3 — Add CodeQL, dependency review, and Dependabot

## Next

- Phase 2: Introduce memory abstractions
  - Create `DarkCloud.Memory.Abstractions`
  - Implement `InMemoryGameMemory`
  - Create `LegacyProcessGameMemory` adapter

## Known blockers

- None

## Deferred work

- WinForms redesign
- Modern Windows host migration
- Emulator-level public CI
- Address-data generator and PNACH generation

---

*Last updated: 2026-07-25*
