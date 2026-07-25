using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Tests.Session
{
    /// <summary>
    /// Memory provider that returns a fixed <see cref="IGameMemory"/> and
    /// can simulate connection failures.
    /// </summary>
    public sealed class FakeGameMemoryProvider : IGameMemoryProvider
    {
        public IGameMemory Current { get; set; }

        public bool ShouldFail { get; set; }

        public bool TryRefresh()
        {
            return !ShouldFail;
        }
    }
}
