# Dark Cloud Enhanced Mod

[![CI](https://img.shields.io/github/actions/workflow/status/Gundorada-Workshop/DarkCloud-Enhanced/ci.yml?branch=main&label=build)](https://github.com/Gundorada-Workshop/DarkCloud-Enhanced/actions/workflows/ci.yml)

Enhanced Mod is a fan-made community project that brings new features and QoL changes to Dark Cloud. It runs as an external executable alongside [PCSX2](https://pcsx2.net/).

For the full feature list and changelog, see [FEATURES.md](FEATURES.md) and [CHANGELOG.md](CHANGELOG.md).

You can download ready-to-use builds from the [Releases](https://github.com/Gundorada-Workshop/DarkCloud-Enhanced/releases) page. **To run Enhanced Mod, you need PCSX2 (v1.60 or v1.7+) and a legal copy of Dark Cloud (NTSC-U or PAL).**

The mod is primarily built for Windows, with a Linux compatibility layer that reads the native PCSX2 executable's ELF exports to locate EE RAM (`EEmem`). PAL support is handled automatically at runtime, and Flatpak/Snap PCSX2 builds are supported.

## Project layout

```
.
├── src/DarkCloudEnhancedMod/      # Main C# WinForms mod
├── native/pcsx2_offsetreader/     # Windows helper DLL (Visual C++)
├── tests/                         # PAL port and Linux smoke tests
├── DarkCloud-Enhanced.sln         # Top-level Visual Studio solution
├── Directory.Build.props          # Shared MSBuild properties
├── global.json                    # .NET SDK pinning
└── .editorconfig                  # Coding style rules
```

## Building

### Windows

Open `DarkCloud-Enhanced.sln` in Visual Studio 2019+ (or use the .NET SDK / MSBuild) and build the **Release** configuration for the **x64** platform. This builds both the mod and the `pcsx2_offsetreader.dll` helper.

```powershell
msbuild -restore "DarkCloud-Enhanced.sln" /p:Configuration=Release /p:Platform=x64
```

Output is written to `src/DarkCloudEnhancedMod/bin/Release/`.

### Linux

Install the .NET 8 SDK (or newer), the Mono runtime, `gcc`, and `python3`, then build with `dotnet`:

```bash
dotnet build "src/DarkCloudEnhancedMod/DarkCloudEnhancedMod.csproj" -c Release
```

You can also ahead-of-time compile the assembly with Mono for verification:

```bash
mono --aot "src/DarkCloudEnhancedMod/bin/Release/DarkCloudEnhancedMod.exe"
```

## Running on Linux

The mod discovers the PCSX2 process by name (`pcsx2`, `pcsx2-qt`, or the Flatpak app ID `net.pcsx2.PCSX2`) and reads the `EEmem` exported symbol from the PCSX2 ELF.

```bash
mono "src/DarkCloudEnhancedMod/bin/Release/DarkCloudEnhancedMod.exe"
```

### Memory access permissions

Reading and writing PCSX2 memory through `/proc/PID/mem` requires `PTRACE_MODE_ATTACH`. On systems with `kernel.yama.ptrace_scope = 1` (the default on Ubuntu), a process may only access its descendants. If you start PCSX2 and the mod separately, run one of the following first:

```bash
# Less secure, but simplest for a normal desktop
sudo sysctl kernel.yama.ptrace_scope=0
```

Or grant the mod `CAP_SYS_PTRACE`:

```bash
sudo setcap cap_sys_ptrace+ep "src/DarkCloudEnhancedMod/bin/Release/DarkCloudEnhancedMod.exe"
```

### Flatpak PCSX2

Flatpak builds are supported. The mod tries `/proc/PID/exe` first and falls back through `/proc/PID/root/<exe-path>` and `/proc/PID/root/<path-from-maps>` to read the ELF inside the sandbox.

```bash
flatpak run net.pcsx2.PCSX2 &
# then start the mod
mono "src/DarkCloudEnhancedMod/bin/Release/DarkCloudEnhancedMod.exe"
```

## Verification

### PAL port verification

`tests/pal/test_pal.py` performs static checks on the PAL `.pnach`, `RegionAddresses.cs`, and the extracted ELF/symbol data. Point `DCEX` at the directory containing `ntsc.elf`, `pal.elf`, `ntsc_syms.txt`, and `pal_syms.txt`:

```bash
cd tests/pal
DCEX=/path/to/dc_extract python3 verify_pal.py
```

### Linux smoke test

`tests/linux_smoke/run.sh` builds a tiny fake PIE PCSX2 process and verifies the ELF `EEmem` discovery and `/proc/PID/mem` read path end-to-end:

```bash
cd tests/linux_smoke
./run.sh
```

## Continuous Integration

The GitHub Actions workflow `.github/workflows/ci.yml` runs on every push and pull request:

- **Windows:** MSBuild Release/x64 build and artifact upload.
- **Linux:** .NET SDK Release build, `mono --aot` verification, `python3 tests/pal/test_pal.py`, and `tests/linux_smoke/run.sh`.

`.github/workflows/release.yml` runs on `v*` tags and creates the release zips from the build artifacts.

## License and legal

This project does not contain or distribute the game itself. Enhanced Mod is only an external program; you must own a legal copy of Dark Cloud to use it. See [LICENSE](LICENSE) for the project license.
