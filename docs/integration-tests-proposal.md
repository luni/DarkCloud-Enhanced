# Real Integration Tests for Dark Cloud Enhanced Mod

This document describes how to add deterministic, repository-safe integration tests that exercise the mod's actual memory-read/write logic against captured or synthetic PS2 EE RAM snapshots.

## 1. Goal

Go beyond the existing `tests/linux_smoke/run.sh` smoke test by:

- Loading a known EE RAM state.
- Calling the mod's `Items`, `Player`, `Weapons`, `CustomChests`, etc. code.
- Reading the modified bytes back and asserting they are correct.

No WinForms UI, live emulator, or ROM is needed for the core tests.

## 2. Architecture

Introduce a small `IMemoryBackend` abstraction behind `Platform`. The real build uses a process backend; tests use a snapshot backend.

```csharp
internal interface IMemoryBackend
{
    bool ReadMemory(IntPtr handle, long address, byte[] buffer, long size, out ulong bytesRead);
    bool WriteMemory(IntPtr handle, long address, byte[] buffer, long size, out ulong bytesWritten);
    bool ProtectMemory(IntPtr handle, long address, long size, uint newProtect, out uint oldProtect);
}
```

`Platform` becomes a thin dispatcher:

```csharp
internal static class Platform
{
    internal static IMemoryBackend Backend { get; set; } = new ProcessMemoryBackend();

    internal static bool ReadMemory(IntPtr h, long a, byte[] b, long s, out ulong r)
        => Backend.ReadMemory(h, a, b, s, out r);

    internal static bool WriteMemory(IntPtr h, long a, byte[] b, long s, out ulong w)
        => Backend.WriteMemory(h, a, b, s, out w);

    internal static bool ProtectMemory(IntPtr h, long a, long s, uint p, out uint o)
        => Backend.ProtectMemory(h, a, s, p, out o);

    // GetEEMem, IsLinux, etc. stay process-specific.
}
```

Move the existing `Platform` read/write/Protect body into `ProcessMemoryBackend`.

## 3. Snapshot backend

A test backend that holds a 32 MB (or partial) EE RAM array and treats `address` as the PS2 virtual address.

```csharp
internal class SnapshotMemoryBackend : IMemoryBackend
{
    private readonly byte[] _ram;

    public SnapshotMemoryBackend(byte[] ram) => _ram = ram;

    public bool ReadMemory(IntPtr h, long address, byte[] buffer, long size, out ulong bytesRead)
    {
        long offset = address - 0x20000000L;
        Buffer.BlockCopy(_ram, (int)offset, buffer, 0, (int)size);
        bytesRead = (ulong)size;
        return true;
    }

    public bool WriteMemory(IntPtr h, long address, byte[] buffer, long size, out ulong bytesWritten)
    {
        long offset = address - 0x20000000L;
        Buffer.BlockCopy(buffer, 0, _ram, (int)offset, (int)size);
        bytesWritten = (ulong)size;
        return true;
    }

    public bool ProtectMemory(IntPtr h, long address, long size, uint newProtect, out uint oldProtect)
    {
        oldProtect = 0;
        return true;
    }
}
```

For partial snapshots, add a `BaseAddress` field so the backend maps `address` correctly.

## 4. Test harness

A single helper that sets up the mod to use a snapshot:

```csharp
static void UseSnapshot(byte[] ram, Region region = Region.NTSC)
{
    Platform.Backend = new SnapshotMemoryBackend(ram);

    // Memory.Read/Write still reference emulatorProcess for ProcessHandle.
    // A dummy current process is enough because the snapshot backend ignores the handle.
    Memory.emulatorProcess = Process.GetCurrentProcess();
    Memory.EEMemAddress = 0x20000000L;
    Memory.EEMemOffset = 0L;

    RegionAddresses.RegionDetected = true;
    RegionAddresses.CurrentRegion = region;
}
```

Then a test looks like:

```csharp
[Fact]
public void Items_PurchasePriceWrite_ChangesRam()
{
    byte[] ram = LoadFixture("tests/fixtures/ntsc_item_prices.bin");
    UseSnapshot(ram);

    // Re-read the cached price list so Items uses the snapshot.
    Items.PriceList = Memory.ReadByteArray(Addresses.ItemPriceTable, 1504);

    // Set a new price through the public item class.
    Items.item81.ValueBuy = 12345;

    // Verify the RAM was updated. item81.ValueBuy reads from the cached
    // PriceList, so re-read the table or use Memory directly.
    ushort index = Items.GetPurchasePriceIndex(81);
    ushort newPrice = Memory.ReadUShort(Addresses.ItemPriceTable + index);
    Assert.Equal(12345, newPrice);
}
```

## 5. How to get fixtures

### Capturing from a real game

1. Start Dark Cloud in PCSX2 and reach the state you want (e.g., shop menu open).
2. Find `EEmem` with the mod's `Platform.GetEEMem` or from `/proc/<pid>/maps`.
3. Read 32 MB from `/proc/<pid>/mem` at `EEmem`:

   ```bash
   python3 - <<'PY'
   import sys
   pid, eemem = sys.argv[1:]
   with open(f'/proc/{pid}/mem', 'rb') as mem, open('raw_dump.bin','wb') as out:
       mem.seek(int(eemem, 0))
       out.write(mem.read(32 * 1024 * 1024))
   PY
   ```

4. Extract only the small region the test needs and zero/scrub everything else. Do **not** commit full RAM dumps -- they contain copyrighted game and BIOS data.

### Synthetic fixtures

For many tests you do not need a real dump. Build the byte array by hand:

```bash
python3 - <<'PY'
import struct
# 1504-byte NTSC item price table, all prices = 10g / 5g
data = b''.join(struct.pack('<HH', 10, 5) for _ in range(376))
open('tests/fixtures/ntsc_item_prices.bin','wb').write(data)
PY
```

This is legally safe and deterministic.

## 6. What to test first

| Feature | Fixture needed | Assertion |
|---|---|---|
| `Items` purchase/sell prices | `ntsc_item_prices.bin` | `Memory.ReadUShort` matches the value set |
| `RegionAddresses.DetectRegion` | `ntsc_boot.bin` / `pal_boot.bin` | `CurrentRegion` correct after detection |
| `Player.Gilda` / `Player.Toan.SetHp` | `ntsc_player.bin` | `Player.Gilda` get returns the value set |
| `Weapons.WeaponsBalanceChanges` | `ntsc_weapon_table.bin` | Baselard endurance equals 30, etc. |
| `CustomChests.ChestRandomizer` | `ntsc_chest_floor1.bin` | Known chest address contains a valid item ID |

Start with `Items` -- it has clean `get/set` properties and a known address table.

## 7. Higher-level UI features

For things triggered by WinForms buttons, extract the logic out of the event handlers:

```csharp
// Instead of
private void BtnSpawnChest_Click(object sender, EventArgs e) { ... Memory.Write... }

// Do
public static void SpawnChestAtFloor(int dungeon, int floor, int itemId)
{
    CustomChests.ChestRandomizer(dungeon, floor, chronicle2: false);
    // or write directly to the chest table at the known address.
}
```

Then test `CustomChests.ChestRandomizer(...)` with the snapshot backend.

## 8. CI integration

Add a new test project or extend the existing `tests/linux_smoke/run.sh` path:

```bash
# after build
dotnet test IntegrationTests.csproj
```

Or add a proper `xunit`/`nunit` test project and run `dotnet test` on the .NET SDK:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/DarkCloud.Memory.Windows/DarkCloud.Memory.Windows.csproj" />
    <ProjectReference Include="../../src/DarkCloud.Core/DarkCloud.Core.csproj" />
    <PackageReference Include="xunit" Version="2.9.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.0" />
  </ItemGroup>
</Project>
```

## 9. Limitations

- Does **not** test WinForms UI, threading, or real emulator attach logic.
- Does **not** replace a human running the game, but it catches "mod logic writes the wrong bytes" regressions.
- Full 32 MB dumps should stay out of the repo; use small scrubbed/synthetic fixtures.
