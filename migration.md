# DarkCloud-Enhanced Migration Plan

## Objective

Modernize DarkCloud-Enhanced so that:

1. Core gameplay behavior can be tested without launching PCSX2.
2. Windows and Linux memory implementations share a common contract.
3. Background execution is cancellable and deterministic.
4. Region addresses and patch data have one source of truth.
5. The application can migrate incrementally from .NET Framework 4.8 to modern .NET.
6. Existing behavior remains unchanged unless a task explicitly states otherwise.

The migration must be completed through small, reviewable pull requests. Do not combine architecture changes, runtime migration, UI redesign, and gameplay changes in the same pull request.

---

# Engineering constraints

## Preserve existing behavior

Unless explicitly instructed:

* Do not rebalance gameplay.
* Do not change memory addresses.
* Do not change PAL or NTSC behavior.
* Do not change generated patch output.
* Do not change supported emulator process names.
* Do not remove the existing WinForms application.
* Do not remove existing Linux smoke tests.
* Do not require copyrighted game assets in public CI.

## Keep the repository buildable

Every pull request must:

* Compile before merge.
* Pass all existing tests.
* Add tests for newly extracted behavior.
* Avoid leaving partially migrated call sites.
* Avoid introducing warnings without documenting them.
* Include a rollback-safe implementation.

## Refactoring rules

Prefer:

* Constructor injection over static global access.
* Interfaces at operating-system boundaries.
* Pure functions for address translation and gameplay rules.
* Small domain services over large static utility classes.
* Cancellation tokens over unmanaged background threads.
* Structured test fixtures over source-code regex parsing.

Avoid:

* Broad rewrites.
* Premature UI migration.
* Global service locators.
* New static mutable state.
* Mocking internal implementation details.
* Tests that depend on execution timing.
* Tests that require a real PCSX2 process unless marked as system tests.

---

# Proposed target structure

Create the following structure incrementally:

```text
src/
  DarkCloud.Core/
  DarkCloud.Memory.Abstractions/
  DarkCloud.Memory.Windows/
  DarkCloud.Memory.Linux/
  DarkCloud.App.WinForms/
  DarkCloud.App.Legacy/

tests/
  DarkCloud.Core.Tests/
  DarkCloud.Memory.ContractTests/
  DarkCloud.Memory.Windows.IntegrationTests/
  DarkCloud.Memory.Linux.IntegrationTests/
  DarkCloud.AddressData.Tests/
```

Do not create all projects in the first pull request. Add them only when their first implementation is ready.

During the transition:

* Shared libraries should initially target `netstandard2.0` when they must be consumed by the existing .NET Framework host.
* New test projects may target a current modern .NET version.
* The existing application remains on .NET Framework 4.8 until the extracted components are stable.
* A modern Windows host should be introduced only after memory access, domain behavior, and execution lifecycle are separated.

---

# Phase 0: Establish the baseline

## Goal

Document and lock down the current repository behavior before architecture changes begin.

## Tasks

1. Build the repository on Windows.
2. Run the current Linux smoke test.
3. Run PAL and PNACH verification scripts.
4. Record the current output artifacts.
5. Identify all static entry points that perform process-memory reads and writes.
6. Identify all direct references from gameplay code to:

   * `MemoryFunctions`
   * `Platform`
   * `ModWindow`
   * `Thread`
   * `Thread.Sleep`
   * `Process`
   * P/Invoke methods
7. Produce a dependency inventory for NuGet packages and native libraries.
8. Record existing compiler warnings.

## Deliverable

Add a document:

```text
docs/migration-baseline.md
```

It must contain:

* Current build commands.
* Current test commands.
* Supported operating systems.
* Current target frameworks.
* Native dependencies.
* Existing CI jobs.
* Known build warnings.
* Existing smoke-test behavior.
* A list of high-coupling static classes.

## Acceptance criteria

* No production behavior changes.
* Existing workflows continue to pass.
* Baseline commands are reproducible.

---

# Phase 1: Harden continuous integration

## Goal

Make CI deterministic and ensure every later migration step has reliable validation.

## Pull request 1.1: Separate CI and release workflows

Create:

```text
.github/workflows/ci.yml
.github/workflows/release.yml
```

Move release creation and tag packaging into `release.yml`.

Set default CI permissions to:

```yaml
permissions:
  contents: read
```

Grant write permissions only to the release job that requires them.

Add workflow concurrency:

```yaml
concurrency:
  group: ci-${{ github.workflow }}-${{ github.event.pull_request.number || github.ref }}
  cancel-in-progress: true
```

Use branch patterns that also match branch names containing slashes.

### Acceptance criteria

* Pull requests run CI but cannot publish releases.
* Tag pushes still create the expected release artifacts.
* Duplicate runs for the same branch are cancelled.

## Pull request 1.2: Add reproducibility checks

Add:

* NuGet lock files where supported.
* Locked restore mode in CI.
* Test result upload.
* Coverage artifact upload.
* Formatting verification for modern projects.
* Generated-file drift verification.

Do not fail the build on a global coverage threshold yet.

### Acceptance criteria

* Dependency changes appear as explicit lock-file changes.
* Test results are retained as CI artifacts.
* Generated files cannot silently diverge from their source data.

## Pull request 1.3: Add repository security automation

Add:

* CodeQL for C# and C/C++.
* Dependency review on pull requests.
* Dependabot for NuGet and GitHub Actions.

### Acceptance criteria

* Security analysis runs independently from the main build.
* Dependency updates are proposed automatically.
* No production code changes are included.

---

# Phase 2: Introduce memory abstractions

## Goal

Allow gameplay logic to operate against simulated PS2 memory without launching PCSX2.

## Pull request 2.1: Add memory abstraction project

Create:

```text
src/DarkCloud.Memory.Abstractions/
```

Initially target `netstandard2.0`.

Add interfaces similar to:

```csharp
public interface IGameMemory
{
    bool TryRead(long address, Span<byte> destination);
    bool TryWrite(long address, ReadOnlySpan<byte> source);
}
```

If `Span<T>` compatibility creates problems with the existing target framework, use array-based overloads initially:

```csharp
public interface IGameMemory
{
    bool TryRead(long address, byte[] destination, int offset, int count);
    bool TryWrite(long address, byte[] source, int offset, int count);
}
```

Add a typed wrapper:

```csharp
public interface IGameMemoryReader
{
    byte ReadByte(long address);
    ushort ReadUInt16(long address);
    uint ReadUInt32(long address);
    int ReadInt32(long address);
    float ReadSingle(long address);
    string ReadString(long address, int length);
}
```

Add a corresponding writer interface.

### Requirements

* Define byte ordering explicitly.
* Return or throw consistent errors.
* Validate invalid lengths.
* Validate arithmetic overflow.
* Avoid referencing WinForms, `Process`, or P/Invoke.

### Acceptance criteria

* The abstraction project contains no operating-system-specific code.
* The legacy application can reference it.
* Unit tests cover primitive conversions and failure handling.

## Pull request 2.2: Add in-memory implementation

Create a test-focused implementation:

```csharp
public sealed class InMemoryGameMemory : IGameMemory
```

It should:

* Use a fixed-size byte buffer.
* Support a configurable base address.
* Reject out-of-range reads and writes.
* Support fixture loading from byte arrays.
* Expose safe test helper methods when appropriate.
* Avoid production dependencies.

Recommended default capacity:

```text
32 MB
```

### Tests

Add tests for:

* Valid read.
* Valid write.
* Read/write round trip.
* First valid address.
* Last valid address.
* Negative address.
* Address below base.
* Address beyond buffer.
* Multi-byte operation crossing the buffer boundary.
* Empty operation.
* Overflowing address arithmetic.
* Primitive encoding.
* String encoding.

### Acceptance criteria

* Tests execute without PCSX2.
* Tests execute on Windows and Linux.
* No production call sites are changed yet.

## Pull request 2.3: Adapt current memory implementation

Wrap the existing static memory implementation with an adapter implementing `IGameMemory`.

Do not rewrite all existing callers yet.

Create a compatibility bridge such as:

```csharp
public sealed class LegacyProcessGameMemory : IGameMemory
```

The adapter may delegate to the current implementation while the architecture is being migrated.

### Acceptance criteria

* Existing process-memory behavior remains unchanged.
* The adapter passes the common memory contract where applicable.
* Existing application startup remains unchanged.

---

# Phase 3: Extract address translation

## Goal

Make PAL and NTSC translation independently testable.

## Pull request 3.1: Add address translation interface

Add:

```csharp
public interface IAddressTranslator
{
    long Translate(GameRegion region, long ntscAddress);
}
```

Define:

```csharp
public enum GameRegion
{
    Unknown = 0,
    Ntsc = 1,
    Pal = 2
}
```

Extract translation logic from static process-memory or region classes into a dedicated implementation.

Do not alter the address table in this pull request.

### Tests

Cover:

* NTSC identity translation.
* Exact PAL translation.
* Offset translation within a mapped range.
* First mapping.
* Last mapping.
* Address below the first mapping.
* Address above the last mapping.
* Duplicate mappings.
* Unsorted mappings.
* Unknown region behavior.
* Arithmetic overflow.

### Acceptance criteria

* Translation tests require no process.
* Existing translated addresses remain byte-for-byte identical.
* Production code may continue to access translation through a temporary static facade.

## Pull request 3.2: Add strongly named addresses

Introduce symbolic address definitions gradually.

Example:

```csharp
public readonly struct GameAddress
{
    public GameAddress(string name, long ntscAddress)
    {
        Name = name;
        NtscAddress = ntscAddress;
    }

    public string Name { get; }

    public long NtscAddress { get; }
}
```

Optionally add typed addresses later:

```csharp
public readonly struct GameAddress<T>
{
    public string Name { get; }
    public long NtscAddress { get; }
}
```

Do not convert every address at once.

Start with addresses used by:

* Boot detection.
* Main-menu detection.
* Region detection.
* Current save-state detection.
* Player health or another simple read-only feature.

### Acceptance criteria

* Named addresses replace magic numbers in at least one end-to-end path.
* Existing behavior does not change.
* New address definitions have tests.

---

# Phase 4: Create a single source of truth for addresses

## Goal

Stop maintaining C# arrays, translation mappings, and PNACH data separately.

## Pull request 4.1: Define structured address data

Create a structured file such as:

```text
data/addresses.yaml
```

or:

```text
data/addresses.json
```

Recommended schema:

```yaml
addresses:
  - name: Game.BootMarker
    dataType: UInt32
    ntsc: 0x202A2518
    pal: 0x202A35A8
    description: Game boot marker

  - name: Player.Health
    dataType: UInt16
    ntsc: 0x21CDD8A2
    pal: 0x21CDE932
    description: Current player health
```

The source data must contain:

* Unique symbolic name.
* NTSC address.
* PAL address when applicable.
* Data type.
* Optional size.
* Optional description.
* Optional category.
* Optional patch metadata.

Do not delete the existing tables yet.

### Validation

Add tests for:

* Unique names.
* Valid hexadecimal values.
* Valid data types.
* Required PAL coverage.
* Valid PS2 memory range.
* Duplicate addresses.
* Overlapping incompatible fields.
* Stable ordering.

### Acceptance criteria

* Structured data parses successfully.
* The new data represents a small pilot subset.
* Existing production code still uses the old tables.

## Pull request 4.2: Generate C# address definitions

Create a deterministic generator that produces:

```text
src/DarkCloud.Memory.Abstractions/Generated/GameAddresses.g.cs
```

Requirements:

* Stable output ordering.
* Stable whitespace.
* Generated-file header.
* No timestamps.
* Clear generator version.
* Build or CI verification that generated output is current.

### Acceptance criteria

* Re-running the generator creates no diff.
* The pilot addresses are consumed from generated C#.
* Generated output matches the existing constants exactly.

## Pull request 4.3: Generate PNACH data

Extend the same source data and generator to produce PNACH output where appropriate.

The generator must preserve:

* Existing addresses.
* Existing patch values.
* Existing output ordering unless ordering is intentionally standardized.
* Existing region behavior.

### Acceptance criteria

* Generated PNACH output matches the existing expected output.
* CI fails when generated output is stale.
* Regex parsing of C# source is no longer required for migrated entries.

## Pull request 4.4: Complete address migration

Move the remaining address definitions into structured data in manageable batches.

Suggested sequence:

1. Process and boot addresses.
2. Region and save-state addresses.
3. Player addresses.
4. Inventory addresses.
5. Weapon addresses.
6. Dungeon addresses.
7. Feature-specific addresses.

### Acceptance criteria

* Existing handwritten parallel address arrays are removed only after full parity.
* Every migrated category has validation tests.
* No address change is hidden inside a refactoring commit.

---

# Phase 5: Add memory contract tests

## Goal

Ensure every memory backend behaves consistently.

## Pull request 5.1: Create shared contract suite

Create:

```text
tests/DarkCloud.Memory.ContractTests/
```

Define reusable tests:

```csharp
public abstract class GameMemoryContractTests
{
    protected abstract IGameMemory CreateMemory();

    [Fact]
    public void WrittenBytesCanBeReadBack()
    {
    }

    [Fact]
    public void InvalidAddressIsRejected()
    {
    }

    [Fact]
    public void BoundaryCrossingReadIsRejected()
    {
    }
}
```

Run the contract against:

* `InMemoryGameMemory`
* Existing Windows implementation
* Existing Linux implementation

Use conditional test execution only where the operating system requires it.

### Contract behavior to define

Specify exact semantics for:

* Invalid address.
* Zero-length read.
* Partial read.
* Process exit during operation.
* Unavailable memory mapping.
* Read-only memory failure.
* Concurrent access.
* Disposed implementation.
* Base-address translation.

### Acceptance criteria

* All implementations pass the same behavioral contract.
* Any operating-system-specific difference is explicitly documented.
* Tests do not use arbitrary delays.

---

# Phase 6: Convert Linux smoke testing into standard integration tests

## Goal

Preserve the existing synthetic-process coverage while making it easier to run and maintain.

## Pull request 6.1: Introduce Linux integration test project

Create:

```text
tests/DarkCloud.Memory.Linux.IntegrationTests/
```

Move testable behavior from shell and reflection-based invocation into normal test fixtures where practical.

Keep the existing shell script temporarily as a compatibility entry point.

The integration fixture should:

1. Launch a synthetic target process.
2. Allocate or expose a simulated `EEmem` region.
3. Populate known game-memory values.
4. Locate the target process.
5. Resolve `EEmem`.
6. Read expected bytes.
7. Write expected bytes.
8. Verify translated addresses.
9. Terminate the process.
10. Confirm graceful handling after process exit.

### Requirements

* Use deterministic process cleanup.
* Add a timeout to prevent hung CI runs.
* Capture target-process stdout and stderr.
* Avoid reflection when public or internal test seams are available.
* Use `InternalsVisibleTo` only when necessary.

### Acceptance criteria

* The test runs through `dotnet test`.
* The test passes on Linux CI.
* Failed tests produce useful diagnostics.
* The legacy shell wrapper may delegate to the test project.

## Pull request 6.2: Remove obsolete Linux test paths

Remove the old reflection harness only after the new integration project covers all prior assertions.

### Acceptance criteria

* No Linux coverage is lost.
* CI no longer maintains two independent implementations of the same smoke test.

---

# Phase 7: Add Windows process-memory integration tests

## Goal

Test Windows memory access without requiring PCSX2.

## Pull request 7.1: Create synthetic Windows target

Create a small helper executable that:

* Allocates a fixed-size memory buffer.
* Writes a known marker.
* Reports its process ID and buffer address.
* Remains alive until signaled.
* Exits cleanly.

Do not include game assets.

## Pull request 7.2: Create Windows integration test project

Create:

```text
tests/DarkCloud.Memory.Windows.IntegrationTests/
```

Test:

* Process discovery.
* Handle acquisition.
* Memory reads.
* Memory writes.
* Invalid handle behavior.
* Target exit during access.
* Correct cleanup.
* Native helper interaction when applicable.
* Base address and address translation.

### Acceptance criteria

* Tests run on `windows-latest`.
* Tests require no PCSX2 installation.
* Native handles are always released.
* Failures include Win32 error details.

---

# Phase 8: Extract the game session state machine

## Goal

Replace nested polling loops with deterministic state transitions.

## Proposed states

```csharp
public enum GameSessionState
{
    NoEmulator,
    EmulatorLocated,
    GameNotBooted,
    MainMenu,
    InGame,
    SaveStateLoaded,
    EmulatorExited,
    Faulted
}
```

## Pull request 8.1: Extract detection logic

Create pure or near-pure detectors for:

* Emulator presence.
* Game boot state.
* Main-menu state.
* Active gameplay state.
* Save-state load.
* Emulator exit.

Each detector should consume abstractions rather than static process APIs.

Example:

```csharp
public sealed class GameSessionDetector
{
    private readonly IGameMemory _memory;
    private readonly IAddressTranslator _translator;

    public GameSessionState Detect(GameSessionState previous)
    {
        // Deterministic state evaluation.
    }
}
```

### Tests

Cover every valid transition and invalid transition.

Use table-driven tests where possible.

### Acceptance criteria

* State detection does not call `Thread.Sleep`.
* Tests can advance states through in-memory fixtures.
* Existing production polling still invokes the extracted detector.

## Pull request 8.2: Introduce a cancellable session runner

Create:

```csharp
public sealed class GameSessionRunner
{
    public Task RunAsync(CancellationToken cancellationToken);
}
```

Inject:

* Session detector.
* Clock or delay abstraction.
* State observer.
* Logger.

Define:

```csharp
public interface IClock
{
    Task DelayAsync(
        TimeSpan delay,
        CancellationToken cancellationToken);
}
```

Use a fake clock in tests.

### Acceptance criteria

* Runner shuts down through cancellation.
* No new raw threads are introduced.
* Tests do not wait for real polling intervals.
* Exceptions are surfaced or logged consistently.

## Pull request 8.3: Replace the main menu thread

Replace one legacy thread at a time.

Start with the main menu or top-level emulator session thread.

Do not migrate all feature threads in the same pull request.

### Acceptance criteria

* Existing UI behavior remains unchanged.
* Application shutdown no longer depends on forcibly terminating the thread.
* Session lifecycle is covered by deterministic tests.

---

# Phase 9: Decouple UI notifications

## Goal

Prevent core, memory, and feature logic from invoking WinForms directly.

## Pull request 9.1: Introduce status interfaces

Add interfaces such as:

```csharp
public interface IModStatusSink
{
    void ReportStatus(ModStatus status);
    void ReportWarning(string message);
    void ReportError(string message, Exception exception);
}

public interface IUserNotificationService
{
    void ShowInformation(string message);
    void ShowWarning(string message);
}
```

Create a WinForms adapter that marshals updates onto the UI thread.

Create a test adapter that records events.

### Acceptance criteria

* At least one non-UI component no longer references `ModWindow`.
* UI updates remain thread-safe.
* Tests can assert emitted statuses.

## Pull request 9.2: Remove UI dependencies from memory code

Memory implementations must not:

* Open message boxes.
* Modify labels.
* Update tray icons.
* Invoke controls.
* Depend on WinForms assemblies.

They should return structured errors or throw documented exceptions.

### Acceptance criteria

* Memory projects compile without WinForms references.
* The WinForms host converts technical failures into user-facing notifications.

---

# Phase 10: Extract domain logic

## Goal

Move gameplay rules out of process-memory and UI classes.

## General extraction pattern

For each feature:

1. Identify its memory reads.
2. Identify its calculations and decisions.
3. Write characterization tests around current behavior.
4. Extract calculations into a domain service.
5. Keep memory reads and writes in an adapter or repository.
6. Compare old and new output using the same fixture.
7. Switch production calls to the extracted service.
8. Remove obsolete static logic.

## Suggested order

### 10.1 Player state

Extract:

* Health calculations.
* Status values.
* Character selection.
* Character-specific conditions.

Create:

```text
src/DarkCloud.Core/Players/
tests/DarkCloud.Core.Tests/Players/
```

### 10.2 Inventory

Extract:

* Slot selection.
* Empty-slot detection.
* Item insertion.
* Item removal.
* Capacity validation.
* Invalid item handling.

Create a memory-backed repository:

```csharp
public interface IInventoryRepository
{
    InventorySnapshot Read();
    IReadOnlyList<InventoryItem> ReadBagItems();
    IReadOnlyList<InventoryItem> ReadBagWeapons(int character = -1);
    IReadOnlyList<InventoryItem> ReadBagAttachments();
    bool TryWriteActiveItem(int slot, InventoryItem item);
    bool TryWriteBagItem(int slot, InventoryItem item);
    bool TryCopyAttachment(int slot, int attachmentId);
}
```

Keep rules in a separate service:

```csharp
public sealed class InventoryService
{
}
```

### 10.3 Weapons

Extract:

* Stat calculations.
* Upgrade rules.
* Attachment rules.
* Validation.
* Serialization and memory layout.

Split large classes by responsibility.

### 10.4 Dungeon behavior

Extract:

* Progression rules.
* Floor-state rules.
* Enemy or event state calculations.
* Reward logic.

### Acceptance criteria for each feature

* Rules can execute against in-memory objects.
* Memory layout code is isolated.
* Tests cover normal and boundary cases.
* Existing behavior is preserved.
* Feature migration occurs in multiple small pull requests.

---

# Phase 11: Convert features into modules

## Goal

Replace independent static feature threads with lifecycle-managed modules.

## Interface

Introduce:

```csharp
public interface IModFeature
{
    string Id { get; }

    Task InitializeAsync(
        GameFeatureContext context,
        CancellationToken cancellationToken);

    Task OnGameTickAsync(
        GameSnapshot snapshot,
        CancellationToken cancellationToken);

    Task ShutdownAsync(
        CancellationToken cancellationToken);
}
```

Define feature metadata separately:

```csharp
public sealed class ModFeatureDescriptor
{
    public string Id { get; init; }
    public string DisplayName { get; init; }
    public bool EnabledByDefault { get; init; }
}
```

## Pull request sequence

Migrate one low-risk feature first.

Recommended first candidate:

* A read-only or simple toggle feature.
* A feature with few memory locations.
* A feature without UI-specific behavior.

Then migrate larger features one by one.

### Module tests

For every feature:

* Initialization is idempotent.
* Tick behavior is deterministic.
* Disabled feature performs no writes.
* Cancellation stops execution.
* Shutdown restores required state.
* Read failure does not cause uncontrolled writes.
* Unsupported region is handled explicitly.
* Emulator exit is handled gracefully.

### Acceptance criteria

* Features no longer own raw threads.
* Scheduling is centralized.
* One failing feature does not terminate all features unless configured.
* Feature enablement can be tested without WinForms.

---

# Phase 12: Add logging and diagnostics

## Goal

Replace ad hoc console output and UI error reporting with structured diagnostics.

## Tasks

Introduce an abstraction compatible with modern logging:

```csharp
public interface IModLogger
{
    void Debug(string message);
    void Information(string message);
    void Warning(string message);
    void Error(Exception exception, string message);
}
```

Alternatively, use `Microsoft.Extensions.Logging` where target compatibility permits.

Log:

* Process attachment.
* Region detection.
* Base-address resolution.
* Memory access failures.
* Session transitions.
* Feature initialization.
* Feature failure.
* Save-state detection.
* Shutdown.

Do not log:

* Sensitive file-system information unless needed.
* Repeated polling success on every tick.
* Large memory dumps by default.

### Acceptance criteria

* Integration-test failures include actionable diagnostics.
* Logs are rate-limited or deduplicated for repeated failures.
* Core domain tests do not require logging.

---

# Phase 13: Modernize configuration

## Goal

Move feature settings and runtime configuration out of static UI state.

## Tasks

Create configuration objects:

```csharp
public sealed class ModConfiguration
{
    public TimeSpan PollInterval { get; init; }
    public IReadOnlyDictionary<string, bool> Features { get; init; }
}
```

Add:

* Validation.
* Default values.
* Versioned persistence.
* Migration from existing settings.
* Unknown-setting preservation when practical.

### Tests

Cover:

* Missing configuration.
* Invalid values.
* Older versions.
* Newer unknown fields.
* Feature defaults.
* Corrupt files.
* Read-only location.
* Atomic save behavior.

### Acceptance criteria

* Feature code receives configuration through constructors or context.
* Feature code does not read controls directly.
* Configuration migration preserves current user settings.

---

# Phase 14: Introduce the modern Windows host

## Goal

Create a modern .NET WinForms host without removing the legacy host prematurely.

## Preconditions

Do not begin this phase until:

* Memory access is behind interfaces.
* Address translation is testable.
* Core rules are extracted for at least the primary features.
* Session execution is cancellable.
* UI notifications are abstracted.
* CI runs unit and integration tests reliably.

## Pull request 14.1: Create modern host project

Create:

```text
src/DarkCloud.App.WinForms/
```

Target a current Windows-specific modern .NET target.

The initial host should:

* Reuse extracted libraries.
* Reproduce the existing startup path.
* Support process attachment.
* Display session status.
* Enable a limited pilot feature set.
* Use modern dependency injection where useful.

Do not redesign the user interface yet.

## Pull request 14.2: Run legacy and modern hosts in parallel

Add packaging or build profiles for:

* Legacy host.
* Modern host.

Document supported environments.

Run the same memory and domain contract suites for both hosts where applicable.

## Pull request 14.3: Reach feature parity

Migrate features in batches.

Track parity in:

```text
docs/modern-host-parity.md
```

The parity matrix should include:

* Feature.
* Legacy status.
* Modern status.
* Automated tests.
* Manual validation.
* Known differences.

## Pull request 14.4: Retire the legacy host

Remove the legacy host only after:

* Feature parity is complete.
* Supported Windows environments are validated.
* Release packaging is updated.
* Rollback instructions exist.
* At least one stable release has used the modern host successfully.

---

# Phase 15: Add optional emulator-level system tests

## Goal

Validate a limited number of end-to-end workflows using PCSX2 in a controlled private environment.

## Constraints

Public CI must not distribute:

* BIOS files.
* Game disc images.
* Copyrighted game data.
* User save files without explicit permission.

Use a private or self-hosted runner.

## Scenarios

Implement only a small critical suite:

1. Launch PCSX2.
2. Boot or attach to a prepared environment.
3. Detect game region.
4. Reach the main menu.
5. Load a known save state.
6. Enable one feature.
7. Verify expected memory changes.
8. Reload the save state.
9. Verify feature recovery.
10. Shut down PCSX2.
11. Verify clean detachment.

## Test classification

Use categories:

```text
Unit
Contract
Integration
System
```

System tests must not run on every pull request.

Recommended triggers:

* Manual workflow dispatch.
* Nightly schedule.
* Release candidate tags.
* Changes to memory backends or state detection.

### Acceptance criteria

* Tests are isolated from public assets.
* Failures retain emulator logs and screenshots where legally permissible.
* The system suite remains small and focused.

---

# Recommended test matrix

## Every pull request

Run:

* Build.
* Unit tests.
* Address-data validation.
* Generated-file drift check.
* Linux synthetic memory integration.
* Windows synthetic memory integration.
* Static analysis.
* Dependency review.
* Formatting check for modern projects.

## Main branch

Additionally run:

* Full coverage reporting.
* Packaging smoke tests.
* Native library validation.
* Extended integration tests.

## Nightly or manual

Run:

* Emulator-level system tests.
* Long-running memory stability tests.
* Repeated attach/detach tests.
* Save-state stress tests.
* Feature interaction tests.

---

# Coverage policy

Do not start with an arbitrary repository-wide threshold.

Use this sequence:

1. Record baseline coverage.
2. Require new projects to publish coverage.
3. Prevent coverage regression in extracted projects.
4. Require high coverage for pure domain and translation code.
5. Use lower expectations for operating-system adapters.
6. Exclude generated code from coverage.
7. Do not count trivial property accessors as meaningful coverage.

Suggested targets after extraction:

```text
DarkCloud.Core:                  85% or higher
Address translation:            95% or higher
Memory codecs:                  95% or higher
Session state machine:          90% or higher
OS-specific memory adapters:    60–75%
WinForms host:                   No strict initial target
Generated code:                 Excluded
```

Mutation testing may be introduced later for address translation and domain rules.

---

# Pull request sizing policy

Each pull request should generally satisfy all of the following:

* One primary architectural purpose.
* Fewer than approximately 500 changed production lines where practical.
* Tests included in the same pull request.
* No unrelated formatting churn.
* No gameplay changes mixed with refactoring.
* No address changes mixed with generator changes.
* No runtime migration mixed with feature migration.

Large mechanical moves should be separated from behavioral edits.

Example:

1. Move file without changes.
2. Verify build.
3. Refactor internals.
4. Add tests.
5. Update callers.

---

# Required agent workflow for each task

For every migration task, follow this sequence.

## 1. Inspect

Identify:

* Existing callers.
* Static dependencies.
* Threading dependencies.
* Memory reads and writes.
* Region-specific behavior.
* Existing test coverage.
* Native boundary interactions.

## 2. Characterize

Before changing behavior:

* Add tests around current observable behavior.
* Capture representative memory fixtures.
* Document any ambiguous behavior.
* Preserve confirmed quirks unless explicitly instructed otherwise.

## 3. Extract

Move one responsibility behind an interface or pure function.

## 4. Adapt

Add a compatibility adapter so existing callers continue to work.

## 5. Migrate

Switch a limited number of call sites.

## 6. Verify

Run:

* Relevant unit tests.
* Full build.
* Existing smoke tests.
* Generated-data validation.
* Platform-specific tests.

## 7. Clean up

Remove obsolete code only after no callers remain.

## 8. Document

Update:

* Migration status.
* New architecture.
* Test commands.
* Any known limitations.

---

# Definition of done for each pull request

A migration pull request is complete only when:

* The repository builds.
* Existing behavior is preserved.
* New behavior is tested.
* Test names describe behavior, not implementation.
* Public APIs are documented.
* Cancellation and cleanup paths are tested where applicable.
* No new static mutable global state is introduced.
* No arbitrary sleeps are added to tests.
* Generated files are current.
* Platform-specific code is isolated.
* CI passes on all supported platforms.
* The migration tracking document is updated.

---

# Migration tracking document

Create:

```text
docs/migration-status.md
```

Use this structure:

```markdown
# Migration Status

## Current phase

Phase 2: Memory abstractions

## Completed

- [x] Baseline documented
- [x] CI and release workflows separated
- [x] In-memory game memory implemented

## In progress

- [ ] Legacy Windows memory adapter

## Next

- [ ] Address translation extraction
- [ ] Shared memory contract tests

## Known blockers

- None

## Deferred work

- WinForms redesign
- Modern host migration
- Emulator-level public CI
```

Update this document in every migration pull request.

---

# Initial backlog

Execute these tasks in order.

## Task 1

Document the existing build, test, memory, threading, address, and UI coupling baseline.

## Task 2

Separate CI and release workflows and reduce workflow permissions.

## Task 3

Add deterministic restore, test artifacts, and generated-file checks.

## Task 4

Create `DarkCloud.Memory.Abstractions`.

## Task 5

Create `InMemoryGameMemory` with boundary tests.

## Task 6

Wrap the existing memory implementation with `IGameMemory`.

## Task 7

Extract `IAddressTranslator` and add characterization tests.

## Task 8

Create shared memory contract tests.

## Task 9

Migrate the Linux synthetic-process smoke test to a standard integration test project.

## Task 10

Add a Windows synthetic-process integration test.

## Task 11

Create the structured address-data pilot.

## Task 12

Generate C# address definitions from structured data.

## Task 13

Generate PNACH output from the same source.

## Task 14

Extract the session state detector.

## Task 15

Introduce cancellation and a fake clock.

## Task 16

Remove direct UI calls from memory components.

## Task 17

Extract one low-risk gameplay feature into `DarkCloud.Core`.

## Task 18

Introduce the feature module lifecycle.

## Task 19

Migrate remaining gameplay features in small batches.

## Task 20

Create the modern Windows host after architectural prerequisites are met.

---

# Instructions for the first implementation task

Begin with Phase 0 only.

Do not modify production behavior.

Produce:

1. `docs/migration-baseline.md`
2. `docs/migration-status.md`
3. A repository dependency map.
4. A list of static memory-access call sites.
5. A list of raw thread and sleep call sites.
6. A list of direct WinForms dependencies outside UI classes.
7. The exact commands needed to reproduce the current Windows and Linux builds.
8. A concise proposal for the first memory-abstraction pull request.

After completing the inspection, run all existing validation commands and report:

* Commands executed.
* Commands that passed.
* Commands that failed.
* Failures caused by the local environment.
* Existing failures unrelated to the proposed migration.

Do not proceed to Phase 1 until the baseline documents are complete.
