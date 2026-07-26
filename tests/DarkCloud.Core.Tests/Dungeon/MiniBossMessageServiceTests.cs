using System.Threading;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class MiniBossMessageServiceTests
    {
        [Fact]
        public void WaitAndDisplay_WhenHudVisible_DisplaysMessage()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeMiniBossMessageLayout(0x1000);
            WriteByte(memory, 0x1000, 1); // hideHud == 1

            var service = new MiniBossMessageService(memory, layout);
            string displayed = null;
            CancellationToken? token = null;
            service.WaitAndDisplay(CancellationToken.None, (msg, t) =>
            {
                displayed = msg;
                token = t;
            });

            Assert.Equal(MiniBossMessageService.WarningMessage, displayed);
            Assert.NotNull(token);
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }

        private sealed class FakeMiniBossMessageLayout : IMiniBossMessageMemoryLayout
        {
            public FakeMiniBossMessageLayout(long hideHud)
            {
                HideHudAddress = hideHud;
            }

            public long HideHudAddress { get; }
        }
    }
}
