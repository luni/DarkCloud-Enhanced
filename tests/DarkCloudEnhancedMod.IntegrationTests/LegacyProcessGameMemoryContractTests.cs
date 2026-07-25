using DarkCloud.Memory.Abstractions;
using DarkCloud.Memory.ContractTests;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    public sealed class LegacyProcessGameMemoryContractTests : GameMemoryContractTests
    {
        protected override long BaseAddress => SnapshotTestHelper.Ps2BaseAddress;
        protected override int Capacity => 1024;

        protected override IGameMemory CreateMemory()
        {
            byte[] ram = SnapshotTestHelper.CreateEmptyRam(Capacity);
            SnapshotTestHelper.UseSnapshot(ram, Region.NTSC);
            return LegacyProcessGameMemory.Instance;
        }
    }
}
