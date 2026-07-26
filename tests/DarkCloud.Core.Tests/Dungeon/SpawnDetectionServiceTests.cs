using System.Threading;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class SpawnDetectionServiceTests
    {
        [Fact]
        public void WaitForSpawn_WhenEnemy0Rendered_ReturnsTrue()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x200);
            var layout = new FakeSpawnDetectionLayout(0x1020, 0x1021, 0x1000);
            WriteByte(memory, 0x1020, 0); // enemy14 render not 255
            WriteByte(memory, 0x1000, 2); // enemy0 rendered

            var service = new SpawnDetectionService(memory, layout);
            bool result = service.WaitForSpawn(200, CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public void WaitForSpawn_WhenEnemy0NotRendered_ReturnsFalse()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x200);
            var layout = new FakeSpawnDetectionLayout(0x1020, 0x1021, 0x1000);
            WriteByte(memory, 0x1020, 0);
            WriteByte(memory, 0x1000, 0); // enemy0 not rendered

            var service = new SpawnDetectionService(memory, layout);
            bool result = service.WaitForSpawn(200, CancellationToken.None);

            Assert.False(result);
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }

        private sealed class FakeSpawnDetectionLayout : ISpawnDetectionMemoryLayout
        {
            public FakeSpawnDetectionLayout(long enemy14Render, long enemy14Hp, long enemy0Render)
            {
                Enemy14RenderStatusAddress = enemy14Render;
                Enemy14HpAddress = enemy14Hp;
                Enemy0RenderStatusAddress = enemy0Render;
            }

            public long Enemy14RenderStatusAddress { get; }
            public long Enemy14HpAddress { get; }
            public long Enemy0RenderStatusAddress { get; }
        }
    }
}
