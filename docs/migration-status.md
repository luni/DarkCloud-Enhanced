# Migration Status

This document tracks progress through the migration plan defined in `migration.md`. Update it in every migration pull request.

## Current phase

Phase 0: Establish the baseline

## Completed

- [x] Baseline documented in `docs/migration-baseline.md`
- [x] Dependency map, call-site inventories, and WinForms coupling inventory produced
- [x] Existing validation commands executed and results recorded
- [x] First memory-abstraction pull request proposed

## In progress

- [ ] Phase 0 review and approval

## Next

- Phase 1: Harden continuous integration
  - Separate CI and release workflows
  - Add concurrency, lock files, and test artifacts
  - Add CodeQL, dependency review, and Dependabot
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
