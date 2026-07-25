# Dark Cloud Enhanced Mod

[![Build and Release](https://img.shields.io/github/actions/workflow/status/Gundorada-Workshop/DarkCloud-Enhanced/build-and-release.yml?branch=main&label=build)](https://github.com/Gundorada-Workshop/DarkCloud-Enhanced/actions/workflows/build-and-release.yml)

Enhanced Mod is a fan-made community project, which brings new features and QoL changes for Dark Cloud. Enhanced Mod performs as an external executable program, which you run alongside PCSX2 emulator.

To know the full details and changes of the mod, read the [CHANGELOG.md](CHANGELOG.md) (converted from the original release PDF).

You can download the mod as zip folder from our [Releases](https://github.com/Gundorada-Workshop/DarkCloud-Enhanced/releases), it contains the installation instructions. ***To run Enhanced Mod, you need PCSX2 Emulator (either v1.60 or v1.7+) and a copy of Dark Cloud (NTSC-U or PAL).***

The mod is primarily built for Windows, with an in-progress Linux compatibility layer. On Linux it reads the native PCSX2 executable's ELF exports to find EE RAM (`EEmem`) instead of using the Windows `pcsx2_offsetreader.dll`. PAL support is handled automatically at runtime, and Flatpak/Snap PCSX2 builds are supported.

Our releases do not contain the game itself, the user has to have their own legal copy of Dark Cloud. Enhanced Mod acts only as an external program.

## Building

### Windows

Open `Dark Cloud Improved Version.sln` in Visual Studio 2019+ and build the **Release** configuration.

### Linux (Mono)

Make sure `mono-complete` and `xbuild` are installed, then:

```bash
xbuild "Dark Cloud Improved Version.sln" /p:Configuration=Release
```

You can also ahead-of-time compile the assembly with:

```bash
mono --aot "Dark Cloud Improved Version/bin/Release/Dark Cloud Enhanced Mod.exe"
```

## Running on Linux

The mod discovers the PCSX2 process by name (`pcsx2`, `pcsx2-qt`, or the Flatpak app ID `net.pcsx2.PCSX2`) and reads the `EEmem` exported symbol from the PCSX2 ELF.

### Memory access permissions

Reading and writing PCSX2 memory through `/proc/PID/mem` requires `PTRACE_MODE_ATTACH`. On systems with `kernel.yama.ptrace_scope = 1` (the default on Ubuntu), a process may only access its descendants. If you start PCSX2 and the mod separately, run one of the following first:

```bash
# Less secure, but simplest for a normal desktop
sudo sysctl kernel.yama.ptrace_scope=0
```

Or grant the mod `CAP_SYS_PTRACE`:

```bash
sudo setcap cap_sys_ptrace+ep "Dark Cloud Improved Version/bin/Release/Dark Cloud Enhanced Mod.exe"
```

### Flatpak PCSX2

Flatpak builds are supported. The mod tries `/proc/PID/exe` first and falls back through `/proc/PID/root/<exe-path>` and `/proc/PID/root/<path-from-maps>` to read the ELF inside the sandbox.

Example:

```bash
flatpak run net.pcsx2.PCSX2 &
# then start the mod
mono "Dark Cloud Improved Version/bin/Release/Dark Cloud Enhanced Mod.exe"
```

## Verification

### PAL port verification

`tests/pal/verify_pal.py` performs static checks on the PAL `.pnach`, `RegionAddresses.cs`, and the extracted ELF/symbol data. Point `DCEX` at the directory containing `ntsc.elf`, `pal.elf`, `ntsc_syms.txt`, and `pal_syms.txt`:

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
