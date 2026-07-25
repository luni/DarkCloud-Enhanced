# Migration Baseline

This document records the state of the repository **before** the migration described in `migration.md` begins. It contains no production behavior changes and is used to verify that later pull requests preserve existing functionality.

## 1. Current build commands

### Windows

The primary build uses MSBuild against the top-level solution:

```powershell
msbuild -restore "DarkCloud-Enhanced.sln" /p:Configuration=Release /p:Platform=x64
```

This builds:

* `src/DarkCloudEnhancedMod/DarkCloudEnhancedMod.csproj` (.NET Framework 4.8 WinForms executable)
* `native/pcsx2_offsetreader/pcsx2_offsetreader.vcxproj` (C++ helper DLL, Visual Studio 2022 / v143 toolset)

Output is written to `src/DarkCloudEnhancedMod/bin/Release/`.

### Linux

The C# project can be built with the .NET SDK because it targets `net48` and the SDK uses Mono reference assemblies:

```bash
dotnet build "src/DarkCloudEnhancedMod/DarkCloudEnhancedMod.csproj" -c Release
```

Building the full solution on Linux fails because the C++ project requires MSBuild with Visual C++ targets:

```bash
dotnet build "DarkCloud-Enhanced.sln" -c Release /p:Platform=x64
```

Error observed:

```text
/home/calvin/DarkCloud-Enhanced/native/pcsx2_offsetreader/pcsx2_offsetreader.vcxproj(28,3): error MSB4019: The imported project "/Microsoft.Cpp.Default.props" was not found.
```

AOT verification (optional) works after the Linux C# build:

```bash
mono --aot "src/DarkCloudEnhancedMod/bin/Release/DarkCloudEnhancedMod.exe"
```

## 2. Current test commands

### PAL port verification

```bash
python3 tests/pal/test_pal.py
```

Verifies that `RegionAddresses.cs` NTSC/PAL arrays match `tests/pal/pal_address_map.json` and that all `.pnach` entries are inside the EE RAM range.

### Linux memory smoke test

```bash
tests/linux_smoke/run.sh
```

Compiles a synthetic PIE PCSX2 process, builds `SmokeTest.cs` with `mcs`, and uses reflection to call `Platform.GetEEMem`, `Platform.ReadMemory`, `RegionAddresses.DetectRegion`, `RegionAddresses.Translate`, `Memory.ReadByte`, and `Memory.WriteByte`.

### Additional repository artifacts

There is no `dotnet test` or xUnit/NUnit harness today. Tests are the PAL Python script and the Linux shell smoke test.

## 3. Supported operating systems

* **Windows** — primary target; full build (C# + C++ helper DLL).
* **Linux** — secondary compatibility layer; C# builds and runs under Mono; memory access uses `/proc/<pid>/mem` and ELF symbol reading.

## 4. Current target frameworks

| Project | Target |
| --- | --- |
| `src/DarkCloudEnhancedMod/DarkCloudEnhancedMod.csproj` | `net48` |
| `native/pcsx2_offsetreader/pcsx2_offsetreader.vcxproj` | `v143` toolset, Windows 10 SDK, `Release|x64` produces a DLL |
| Smoke test | `mcs` against the built `DarkCloudEnhancedMod.exe` |
| PAL tests | `python3` |

Shared MSBuild properties in `Directory.Build.props`:

* `LangVersion` = `7.2`
* `Deterministic` = `true`
* `Nullable` = `disable`

`global.json` pins the .NET SDK to `8.0.100` with `latestMajor` roll-forward.

## 5. Native dependencies

### Windows

* `kernel32.dll` — `ReadProcessMemory`, `WriteProcessMemory`, `VirtualProtectEx`, `OpenProcess`, `DebugActiveProcess`, `DebugSetProcessKillOnExit`, `DebugActiveProcessStop`, `GetLastError`, `FormatMessage`.
* `user32.dll` — `ShowWindow`, `GetConsoleWindow`.
* `pcsx2_offsetreader.dll` — custom C++ DLL exported as `?GetEEMem@@YAJH@Z` (Cdecl) to read the PCSX2 `EEmem` symbol address.

### Linux

* `libc.so.6` — `kill` (used for `SIGSTOP`/`SIGCONT`).
* `/proc/<pid>/mem` — read/write process memory.
* `/proc/<pid>/maps` — locate executable mappings and the largest `rw-p`/`rw-s` region.
* `/proc/<pid>/exe` and `/proc/<pid>/root/<path>` — Flatpak/Snap sandbox support.
* `/proc/<pid>/cmdline` — identify wrapper processes.

### Native source

`native/pcsx2_offsetreader/main.cpp` uses `EnumProcessModules`, `GetProcAddressEx`, and `OpenProcess` to locate `EEmem` in a PCSX2 process.

## 6. NuGet and assembly dependency map

### Direct package references

* `System.Resources.Extensions` `4.6.0` (transitively brings in `System.Buffers`, `System.Memory`, `System.Numerics.Vectors`, `System.Runtime.CompilerServices.Unsafe`).

### Framework references

`System`, `System.Core`, `System.Data`, `System.Data.DataSetExtensions`, `System.Deployment`, `System.Drawing`, `System.Net.Http`, `System.Windows.Forms`, `System.Xml`, `System.Xml.Linq`, `Microsoft.CSharp`.

### Project reference graph

```text
DarkCloud-Enhanced.sln
├── src/DarkCloudEnhancedMod/DarkCloudEnhancedMod.csproj
│   ├── System.Resources.Extensions (NuGet)
│   └── .NET Framework 4.8 reference assemblies
└── native/pcsx2_offsetreader/pcsx2_offsetreader.vcxproj
    └── Windows-only Visual C++ runtime / PSAPI / TlHelp32
```

## 7. Existing CI jobs

`.github/workflows/build-and-release.yml`:

1. **Build (Windows)** — MSBuild Release/x64, upload `src/DarkCloudEnhancedMod/bin/Release`.
2. **Build and Test (Linux)** — `dotnet build` C# project, `mono --aot`, `python3 tests/pal/test_pal.py`, `tests/linux_smoke/run.sh`, upload Linux build artifacts.
3. **Create GitHub Release** — runs only on `refs/tags/v*`, downloads Windows and Linux artifacts, creates zip files, publishes with `softprops/action-gh-release@v2`.

Permissions are currently `contents: write` for the entire workflow and there is no concurrency control.

## 8. Known build warnings

The Release C# build on Linux produces **15 warnings** (CS0169 / CS0414). These are all unused private fields:

| File | Line | Warning |
| --- | --- | --- |
| `SideQuestManager.cs` | 76 | `CS0169` `rolledbackfloornumber` is never used |
| `SideQuestManager.cs` | 77 | `CS0169` `backfloornumber` is never used |
| `TownCharacter.cs` | 26 | `CS0169` `checkByte` is never used |
| `TownCharacter.cs` | 36 | `CS0169` `charSelected` is never used |
| `TownCharacter.cs` | 37 | `CS0169` `indungeon` is never used |
| `Dialogues.cs` | 97 | `CS0169` `prevDialogue` is never used |
| `Dialogues.cs` | 172 | `CS0414` `obtainedAttachments` is assigned but never used |
| `SideQuestManager.cs` | 66 | `CS0414` `DEFreward` is assigned but never used |
| `SideQuestManager.cs` | 85 | `CS0414` `currentAddressFishingQuestType` is assigned but never used |
| `SideQuestManager.cs` | 93 | `CS0414` `currentAddressQueensQuestsCompleteCount` is assigned but never used |
| `TownCharacter.cs` | 38 | `CS0414` `charaSwitchFunctionsRestored` is assigned but never used |
| `TownCharacter.cs` | 42 | `CS0414` `dialogueWritten` is assigned but never used |
| `TownCharacter.cs` | 44 | `CS0414` `nearNPCSD` is assigned but never used |
| `Dialogues.cs` | 13 | `CS0414` `brownbooPickleData` is assigned but never used |
| `Items.cs` | 355 | `CS0414` `item81._dropRate` is assigned but never used |

## 9. Existing smoke-test behavior

### PAL verification

`test_pal.py` parses `RegionAddresses.cs` NTSC/PAL arrays and the two `.pnach` files. Current result:

```text
PASS: all 872 mapped entries match pal_address_map.json
PASS: NTSC .pnach has 317 entries and all addresses are in EE RAM
PASS: PAL .pnach has 317 entries and all addresses are in EE RAM
```

### Linux smoke test

`run.sh` performs the following reflection-based checks:

1. Launches `fake_pcsx2` (a PIE executable that exports an `EEmem` symbol).
2. Calls `Platform.GetEEMem(pid, pid)` and compares it to the fake process output.
3. Calls `Platform.ReadMemory` to read the 8-byte `DarkClou` marker.
4. Calls `Memory.ReadByteArray`/`ReadByte` at `0x20299540` and checks for the `Dark` boot string.
5. Calls `RegionAddresses.DetectRegion` and verifies `RegionDetected = true` and `CurrentRegion = 0` (NTSC).
6. Performs a `Memory.WriteByte`/`ReadByte` round-trip at `0x20001000`.
7. Forces PAL mode and calls `RegionAddresses.Translate(0x20299540)`, verifying the result matches the `PAL` array value.

Current result:

```text
PASS: smoke test and integration checks succeeded
```

## 10. High-coupling static classes

These classes have broad static surface areas and are referenced from many call sites. They should be the primary targets of extraction.

| Class | Responsibility | Key mutable static state |
| --- | --- | --- |
| `Memory` (`MemoryFunctions.cs`) | All high-level read/write primitives, process lookup, `EEMem` offset caching | `emulatorProcess`, `EEMemAddress`, `EEMemOffset`, `CheckEEMemAddress`, `CheckEEMemOffset` |
| `Platform` (`Platform.cs`) | OS-specific memory I/O, process suspend/resume, `EEmem` discovery | `_linuxMemStream`, `_linuxPid`, `IsLinux` |
| `RegionAddresses` (`RegionAddresses.cs`) | PAL/NTSC translation table and region detection | `CurrentRegion`, `RegionDetected`, `NTSC[]`, `PAL[]` |
| `Program` (`Program.cs`) | Entry point, console window handle, assembly resolve, `modWindowForm` | `consoleH`, `modWindowForm` |
| `MainMenuThread` (`MainMenuThread.cs`) | Session lifecycle, owns feature threads, polling state | `firstlaunch`, `ingame`, `ingameFlag`, `userMode`, `saveStateUsed`, `saveFileMessageBox`, `PID`, `townThread`, `changesThread`, `dungeonthread`, `weaponspecialeffectThread` |
| `ModWindow` (`ModWindow.cs`) | Main UI form and static notification helpers | `instance`, `launchThread`, `townThread`, etc. |
| `Addresses` (`Addresses.cs`) | Large collection of PS2 address constants | none (constants only) |

## 11. Static memory-access call sites

The static `Memory` class is the only production entry point for process reads/writes. Every feature class calls `Memory.Read...` or `Memory.Write...` directly. The following table counts `Memory.Read*` and `Memory.Write*` calls per file (includes `Memory.ReadByteArray` and `Memory.WriteByteArray`).

| File | `Memory.Read/Write` call sites |
| --- | --- |
| `Weapons.cs` | 1532 |
| `TownCharacter.cs` | 368 |
| `CustomEffects.cs` | 230 |
| `Player.cs` | 201 |
| `SideQuestManager.cs` | 165 |
| `Shop.cs` | 158 |
| `Dayuppy.cs` | 146 |
| `Dungeon.cs` | 145 |
| `Dialogues.cs` | 130 |
| `ReusableFunctions.cs` | 113 |
| `ModWindow.cs` | 98 |
| `CustomChests.cs` | 37 |
| `CheatCodes.cs` | 34 |
| `DailyShopItem.cs` | 33 |
| `MainMenuThread.cs` | 26 |
| `MiniBoss.cs` | 22 |
| `TASThread.cs` | 14 |
| `Items.cs` | 7 |
| `RubyOrbs.cs` | 6 |
| `RegionAddresses.cs` | 4 |
| `Enemies.cs` | 3 |
| `Program.cs` | 1 |
| **Total** | **3473** |

The actual byte I/O is implemented in `Platform`:

* `Platform.ReadMemory` — used by `Memory.ReadByteArray`, `Memory.ReadString`.
* `Platform.WriteMemory` — used by `Memory.Write`, `Memory.WriteString`, `Memory.WriteByteArray`.
* `Platform.ProtectMemory` — used by `Memory.VirtualProtect`, `Memory.VirtualProtectEx`, and the search helpers.

## 12. Raw thread and sleep call sites

The application creates dedicated `Thread` instances per feature and uses `Thread.Sleep` for polling. This is the complete inventory of `new Thread(...)`, `Thread.Sleep(...)`, and `.Abort()` call sites.

| File | Count | Notable locations |
| --- | --- | --- |
| `Dungeon.cs` | 43 | `boneDoorThread`, `seventhHeavenThread`, `chronicleSwordThread`, `evilciseThread`, `angelGearThread`, `tallHammerThread`, `infernoHammerThread`, `mobiusRingThread`, `herculesWrathThread`, `babelSpearThread`, `supernovaThread`, `starBreakerThread`, `elementSwapThread`, `cheatCodeThread`, `spawnsCheck`, `minibossProcess`, `miniBossMessage`, `dunEscapeConfirmThread`; sleeps at 10ms, 100ms, 120ms, 500ms, 2500ms. |
| `Dayuppy.cs` | 25 | `cheatCodeThread`, `elementSwapThread`, `messageThread`; sleeps at 1ms, 50ms, 100ms, 300ms, 750ms, 1000ms, 1100ms, 2000ms. |
| `MainMenuThread.cs` | 21 | `townThread`, `changesThread`, `dungeonthread`, `weaponspecialeffectThread`; sleeps at 1ms, 10ms, 100ms, 200ms, 800ms, 1000ms, 30000ms. |
| `CustomEffects.cs` | 17 | `damageFadeoutThread`; sleeps at 50ms, 100ms, 200ms, 250ms, 500ms, `Delay`. |
| `TownCharacter.cs` | 12 | `characterNamesFixThread`; sleeps at 10ms, 50ms, 100ms, 300ms, 350ms. |
| `ModWindow.cs` | 10 | `townThread`, `TASSThread`, `TASSThread2`, `dungeonthread`, `debugThread`, `launchThread`; `.Abort()` calls at lines 245 and 386. |
| `Weapons.cs` | 4 | `weaponsMenuListener`; sleeps at 64ms, 100ms, 1000ms. |
| `CheatCodes.cs` | 5 | `debugThread`; sleeps at 50ms, 500ms, 2000ms. |
| `Program.cs` | 1 | `Thread.Sleep(1000)` while waiting for PCSX2. |
| `CustomChests.cs` | 1 | `Thread.Sleep(100)`. |
| `MiniBoss.cs` | 1 | `Thread.Sleep(200)`. |
| `Patcher.cs` | 1 | `Thread.Sleep(500)`. |
| `ReusableFunctions.cs` | 1 | `Thread.Sleep(100)`. |

## 13. Direct WinForms dependencies outside UI classes

`ModWindow` is the only UI form, but other classes have direct WinForms coupling:

| File | Coupling |
| --- | --- |
| `Program.cs` | `using System.Windows.Forms;`, `Application.EnableVisualStyles()`, `Application.SetCompatibleTextRenderingDefault(true)`, `Application.Run(modWindowForm)`, `Form modWindowForm`, `using Application = System.Windows.Forms.Application`. |
| `Weapons.cs` | `using System.Windows.Forms;` (unused import, but present). |
| `MainMenuThread.cs` | Calls static `ModWindow.EmulatorCount`, `ModWindow.PnachNotActive`, `ModWindow.FirstLaunchGameMode`, `ModWindow.CurrentlyInMainMenu`, `ModWindow.CurrentlyInGame`, `ModWindow.SaveStateDetected`, `ModWindow.NotEnhancedModSaveFile`; also sets `ModWindow.saveFileMessageBox` flag. |
| `MemoryFunctions.cs` | Calls `ModWindow.NightlyVersionCheck()` from `Memory.Initialize()`. |

`ModWindow` itself marshals all UI updates with `InvokeRequired`/`Invoke` and shows `MessageBox` dialogs.

## 14. P/Invoke and process boundary inventory

### P/Invoke declarations in production code

`MemoryFunctions.cs`:

* `GetLastErrorWin` (`kernel32.dll`)
* `FormatMessageWin` (`kernel32.dll`)
* `OpenProcess` (`kernel32.dll`)

`Platform.cs`:

* `ReadProcessMemory` (`kernel32.dll`)
* `WriteProcessMemory` (`kernel32.dll`)
* `VirtualProtectEx` (`kernel32.dll`)
* `DebugActiveProcess` (`kernel32.dll`)
* `DebugSetProcessKillOnExit` (`kernel32.dll`)
* `DebugActiveProcessStop` (`kernel32.dll`)
* `GetLastErrorWin` (`kernel32.dll`)
* `GetEEMemWin` (`\Resources\pcsx2_offsetreader.dll`)
* `kill` (`libc.so.6`)

`Program.cs`:

* `GetConsoleWindow` (`kernel32.dll`)
* `ShowWindow` (`user32.dll`)

### `System.Diagnostics.Process` usage

* `Memory.GetProcess` — `Process.GetProcesses()` and process scoring.
* `Memory.emulatorProcess` — cached `Process` object; `.Id`, `.ProcessName`, `.Handle`.
* `Program.GetPCSX2Executable` — reads `Memory.emulatorProcess`.
* `MainMenuThread.CheckEmulatorAndGame` — reads `Memory.emulatorProcess.Id`.
* `SmokeTest.cs` — `Process.Start(fakeExe)` and `Process.GetProcessById(pid)` for reflection setup.
* `Dungeon.cs` — `minibossProcess.Start()`.
* `TownCharacter.cs` and `Dayuppy.cs` — `Memory.emulatorProcess.Handle` passed to `Memory.VirtualProtect` / `Memory.VirtualProtectEx`.
* `ModWindow.cs` — `Process.Start(url)` for Discord / feedback links.

## 15. Exact commands needed to reproduce current Windows and Linux builds

### Windows (full)

```powershell
msbuild -restore "DarkCloud-Enhanced.sln" /p:Configuration=Release /p:Platform=x64 /verbosity:minimal
```

### Linux (C# only)

```bash
dotnet build "src/DarkCloudEnhancedMod/DarkCloudEnhancedMod.csproj" -c Release
mono --aot "src/DarkCloudEnhancedMod/bin/Release/DarkCloudEnhancedMod.exe"
```

### PAL tests

```bash
python3 tests/pal/test_pal.py
```

### Linux smoke test

```bash
tests/linux_smoke/run.sh
```

## 16. Validation run results

All commands below were executed on the Linux baseline environment.

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet build "src/DarkCloudEnhancedMod/DarkCloudEnhancedMod.csproj" -c Release` | **Passed** | 15 warnings, 0 errors. |
| `mono --aot "src/DarkCloudEnhancedMod/bin/Release/DarkCloudEnhancedMod.exe"` | **Passed** | Produced `DarkCloudEnhancedMod.exe.so`. |
| `python3 tests/pal/test_pal.py` | **Passed** | 872 mappings and 317 NTSC / 317 PAL `.pnach` entries valid. |
| `tests/linux_smoke/run.sh` | **Passed** | EEmem discovery, read/write, region detection, PAL translation all succeeded. |
| `dotnet build "DarkCloud-Enhanced.sln" -c Release /p:Platform=x64` | **Failed** | Linux cannot build the `pcsx2_offsetreader.vcxproj` C++ project. This is expected on non-Windows hosts. |

### Output artifacts observed

After a successful Linux C# Release build:

```text
src/DarkCloudEnhancedMod/bin/Release/
├── DarkCloudEnhancedMod.exe
├── DarkCloudEnhancedMod.exe.config
├── DarkCloudEnhancedMod.exe.so     (from mono --aot)
├── DarkCloudEnhancedMod.pdb
├── Resources/
│   └── PNACH/
│       ├── A5C05C78.pnach
│       └── SCES-50295_0BAA8DD8.pnach
├── System.Buffers.dll
├── System.Memory.dll
├── System.Numerics.Vectors.dll
├── System.Resources.Extensions.dll
└── System.Runtime.CompilerServices.Unsafe.dll
```

## 17. Proposal for the first memory-abstraction pull request

The first architecture PR should add `src/DarkCloud.Memory.Abstractions/` and `src/DarkCloud.Memory.Abstractions/Tests/` without changing any existing call sites. It directly addresses migration Phase 2 ("Introduce memory abstractions").

### Scope

1. New `netstandard2.0` project `src/DarkCloud.Memory.Abstractions/` containing:
   * `IGameMemory` with array-based overloads compatible with `netstandard2.1`/`net48`:

     ```csharp
     public interface IGameMemory
     {
         bool TryRead(long address, byte[] destination, int offset, int count);
         bool TryWrite(long address, byte[] source, int offset, int count);
     }
     ```

   * `IGameMemoryReader` (`ReadByte`, `ReadUInt16`, `ReadInt32`, `ReadUInt32`, `ReadSingle`, `ReadString`).
   * `IGameMemoryWriter` (`WriteByte`, `WriteUInt16`, `WriteInt32`, `WriteUInt32`, `WriteSingle`, `WriteString`).
   * Explicit little-endian semantics and documented failure behavior (return `false` instead of throwing for invalid addresses; overflow returns `false`).

2. New `InMemoryGameMemory` test implementation:
   * 32 MB fixed buffer, configurable base address (default `0x20000000`).
   * Supports fixture loading from `byte[]`.
   * Validates address range and arithmetic overflow.

3. Unit tests covering:
   * Valid read/write round-trip.
   * First/last valid address.
   * Negative address, address below base, address beyond buffer.
   * Boundary-crossing multi-byte operations.
   * Empty operations, overflowing address arithmetic, primitive/string encoding.

### Out of scope

* No changes to `MemoryFunctions.cs`, `Platform.cs`, `RegionAddresses.cs`, or any feature classes.
* No WinForms references in the new project.
* No process handle logic in the new project.

### Acceptance criteria

* `DarkCloud.Memory.Abstractions` compiles for `netstandard2.0`.
* Existing `DarkCloudEnhancedMod` still builds and passes `test_pal.py` and `tests/linux_smoke/run.sh`.
* New unit tests run with `dotnet test` and pass on Linux and Windows.

---

*Baseline captured: 2026-07-25. Host: Linux with .NET SDK 8.0.423 and Mono 6.12.*
