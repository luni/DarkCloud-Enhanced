# Dark Cloud Enhanced Mod – PAL Port Plan

## Summary
The NTSC-U mod currently targets `[SCUS-97111] (A5C05C78)`. Both the NTSC and PAL CHDs are present in `/home/calvin`, so we can extract the ELFs and derive the PAL address mapping. The main work is: (1) compare NTSC/PAL ELF layouts to get address deltas, (2) translate the `.pnach` and all hardcoded PS2 addresses in C#, and (3) add runtime region detection so one binary supports both.

## Current status (in progress)
- Extracted NTSC (`SCUS_971.11`) and PAL (`SCES_502.95`) ELFs from the CHDs.
- `readelf` shows the two ELFs are **not** a single uniform shift; code/data shift by per-segment and per-symbol amounts.
- Built a symbol-table-based mapper (`/home/calvin/dc_extract/`) that pairs `readelf` symbols between NTSC and PAL and derives per-address PAL equivalents.
- Generated `RegionAddresses.cs` with 872 NTSC→PAL mappings and runtime binary-search fallback for computed offsets.
- Generated `Resources/PNACH/SCES-50295_0BAA8DD8.pnach` (PAL CRC `0BAA8DD8`), preserving `.pnach` top-nibble size/test-type digits and re-encoding MIPS `jal`/`j`/`lui`/`lw`/`sw`/`beq` values.
- Wired `Memory` read/write methods and `MainMenuThread` to detect region and translate addresses on the fly.
- Added the new pnach and `RegionAddresses.cs` to `.csproj`.
- Migrated the public changelog PDF to `CHANGELOG.md` and updated `README.md`.
- Started Linux compatibility work with a new `Platform.cs` abstraction (`process_vm`/`-proc/pid/mem` fallback, `SIGSTOP`/`SIGCONT`, heuristic `GetEEMem` from `/proc/<pid>/maps`).
- Still needs a build/test run to verify.

## New scope added
- Migrate the public-release `Full_Change_Log_Public_Release_v1.00.pdf` to `CHANGELOG.md` and update `README.md` to reference it.
- Make the mod Linux-compatible: replace Windows-only P/Invokes (`ReadProcessMemory`, `WriteProcessMemory`, `VirtualProtectEx`, `DebugActiveProcess`, `pcsx2_offsetreader.dll`) with cross-platform equivalents, and provide a Linux `GetEEMem` implementation.

## Files available for comparison
- NTSC-U: `/home/calvin/Dark Cloud (USA).chd`
- PAL: `/home/calvin/Dark Cloud (Europe) (En,Fr,De,Es,It).chd`

## Implementation Steps

### 1. Extract and compare the ELFs
- Convert both CHDs to ISO/cue+bin with `chdman extractcd`.
- Extract the root PS2 executables (`SCUS_971.11` for NTSC, `SLES_…`/`SCES_…` for PAL) using `isoinfo`.
- Run `readelf -h -l -S` on both ELFs and compare program headers/sections.
- Determine whether the difference is a uniform `delta` or per-segment shifts.

### 2. Create the PAL `.pnach`
- Parse `Resources/PNACH/A5C05C78.pnach`.
- Translate every `patch=1,EE,<addr>,extended,<value>` target address.
- Also translate addresses embedded in conditional `E…` lines (e.g. `E1010003,extended,002A2534`).
- Write a new file named for the PAL serial/CRC and add it to `Resources/PNACH/` and the `.csproj`.

### 3. Refactor C# address constants for region switching
- Introduce `enum Region { NTSC, PAL }` and a `Region CurrentRegion` static in `Memory`/`MainMenuThread`.
- If the ELF comparison shows a uniform delta, the least invasive fix is to add a `RegionDelta` inside `Memory.Read`/`Write` (which already use `EEMemOffset + address`).
- If deltas are non-uniform, convert `const int` address fields in `Addresses.cs`, `Player.cs`, `Weapons.cs`, `Shop.cs`, `SideQuestManager.cs`, `TownCharacter.cs`, `Dialogues.cs`, `Dungeon.cs`, `CustomChests.cs`, `DailyShopItem.cs`, `CheatCodes.cs`, and `ModWindow.cs` into region-aware `static int` properties.

### 4. Update boot and region detection
- Replace the hard `0x20299540 == 1802658116` ("Dark") check in `MainMenuThread` with detection that works for both regions: search for the `SCUS`/`SLES` serial string or test known NTSC/PAL signature locations.
- Set `Memory.CurrentRegion` before any feature threads start.

### 5. Verify custom mod scratch memory
- Confirm the `0x21F100xx` and `0x21CE44xx` flag region is free in PAL.
- If PAL uses those areas, pick new safe addresses and update both C# code and the PAL `.pnach`.
- Update `Resources/enhancedmodflagaddresses.txt` and comments.

### 6. Build and test
- Build with `msbuild` (Mono on Linux or VS/MSBuild on Windows).
- Test with PCSX2 + PAL ISO: pnach active flag, mod handshake, town/dungeon detection, shops, weapons, side quests.
- Regression test the NTSC build to ensure it still works.

### 7. Documentation
- Update `README.md` for PAL installation (new `.pnach` filename/CRC) and region selection.
- Add derivation notes to `AGENTS.md` or project notes.

### 8. Migrate the changelog PDF to Markdown
- Download or locate `Full_Change_Log_Public_Release_v1.00.pdf` from the project Releases.
- Convert it to `CHANGELOG.md`, preserving the per-page sections and bullet lists.
- Update `README.md` to point to `CHANGELOG.md` instead of the PDF link.

### 9. Linux compatibility
- Add OS detection in `MemoryFunctions.cs` (Windows vs Linux/Unix).
- On Linux replace `ReadProcessMemory`/`WriteProcessMemory` with `process_vm_readv`/`process_vm_writev` or `/proc/<pid>/mem`.
- Replace `VirtualProtectEx` with a no-op or `mprotect`-based helper where the target page is writable.
- Replace `DebugActiveProcess`/`DebugActiveProcessStop` suspend/resume with `kill(pid, SIGSTOP/SIGCONT)`.
- Implement a Linux `GetEEMem` that locates the PCSX2 `EEmem` exported pointer (parse `/proc/<pid>/maps` + ELF `.dynsym`, or heuristic by finding the large PCSX2 shared-memory mapping).
- Make the `pcsx2_offsetreader.dll` `DllImport` conditional/optional so it does not fail on Linux.
- Build and smoke-test on Linux with Mono (`msbuild`) or modern .NET if the project is converted to SDK-style.

## Files to Modify
- `Resources/PNACH/A5C05C78.pnach` (source for translation)
- `Resources/PNACH/<PAL>.pnach` (new)
- `Dark Cloud Improved Version.csproj` (add new embedded pnach resource)
- `MemoryFunctions.cs` (region state and optional `RegionDelta`)
- `MainMenuThread.cs` (region detection, boot signature)
- `ModWindow.cs` (region-aware option flags and UI)
- `Addresses.cs`, `Player.cs`, `Weapons.cs`, `Shop.cs`, `SideQuestManager.cs`, `TownCharacter.cs`, `Dialogues.cs`, `Dungeon.cs`, `CustomChests.cs`, `DailyShopItem.cs`, `CheatCodes.cs` (PAL address mapping if non-uniform delta)
- `README.md`
- `CHANGELOG.md` (new, converted from PDF)
- `MemoryFunctions.cs` / new `PlatformMemory.cs` (Linux P/Invokes and `GetEEMem`)

## Verification Checklist
- [x] `readelf` segment diff between NTSC/PAL ELFs produces a clear delta/translation table.
- [x] `Full_Change_Log_Public_Release_v1.00.pdf` migrated to `CHANGELOG.md`.
- [ ] `msbuild` succeeds with no errors.
- [ ] PAL `.pnach` loads in PCSX2 without invalid-address warnings.
- [ ] `0x21F10020` / `0x21F10024` handshake works in PAL.
- [ ] NTSC build still passes basic smoke test.
- [ ] Linux build/run path works on Mono / modern .NET (process memory reads, `EEmem` discovery, suspend/resume).

## Generated/Modified files
- `Dark Cloud Improved Version/RegionAddresses.cs`
- `Dark Cloud Improved Version/MemoryFunctions.cs`
- `Dark Cloud Improved Version/MainMenuThread.cs`
- `Dark Cloud Improved Version/Dark Cloud Improved Version.csproj`
- `Dark Cloud Improved Version/Resources/PNACH/SCES-50295_0BAA8DD8.pnach`
- `Dark Cloud Improved Version/Resources/PNACH/A5C05C78.pnach` (source, not modified)
- `CHANGELOG.md` (converted from PDF)
- `README.md` (PAL + changelog link updates)
- `/home/calvin/dc_extract/` (temporary ELF/symbol mapping artifacts)

## Risks / Considerations
- PAL code/data may not shift by a single uniform delta; that would require per-address mapping and a larger refactor of the ~1,500 hardcoded addresses.
- Conditional `.pnach` lines encode addresses inside their data words, so a simple address-column shift is not enough.
- `Dialogues.cs` and language-dependent dialogue IDs may differ in the multi-language PAL build.
- `pcsx2_offsetreader.dll` is a native Windows DLL; the port stays Windows-x86/x64.
- Only the provided CHDs will be used; no redistribution.
