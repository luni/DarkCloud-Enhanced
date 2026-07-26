using DarkCloud.Memory.Abstractions;
using DarkCloud.Memory.ContractTests;
using DarkCloud.Memory.Windows;

namespace DarkCloudEnhancedMod.Windows.IntegrationTests
{
    /// <summary>
    /// Runs the shared <see cref="IGameMemory"/> contract suite against
    /// <see cref="LegacyProcessGameMemory"/> so the modern host's memory backend
    /// satisfies the same guarantees as the legacy host.
    /// </summary>
    public sealed class LegacyProcessGameMemoryContractTests : GameMemoryContractTests
    {
        public LegacyProcessGameMemoryContractTests()
        {
            byte[] ram = SnapshotTestHelper.CreateEmptyRam(Capacity);
            SnapshotTestHelper.UseSnapshot(ram, Region.NTSC);
        }

        protected override long BaseAddress => SnapshotTestHelper.Ps2BaseAddress;

        protected override int Capacity => 1024;

        protected override IGameMemory CreateMemory()
        {
            return new LegacyProcessGameMemory();
        }
    }
}
