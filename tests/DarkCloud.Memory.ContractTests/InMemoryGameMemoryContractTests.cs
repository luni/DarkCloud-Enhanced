using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Memory.ContractTests
{
    public sealed class InMemoryGameMemoryContractTests : GameMemoryContractTests
    {
        protected override long BaseAddress => 0x20000000L;
        protected override int Capacity => 1024;

        protected override IGameMemory CreateMemory()
        {
            return new InMemoryGameMemory(BaseAddress, Capacity);
        }
    }
}
