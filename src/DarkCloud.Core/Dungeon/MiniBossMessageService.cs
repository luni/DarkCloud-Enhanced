using System;
using System.Threading;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Waits until the HUD is visible and then invokes the caller-supplied
    /// display action with the mini-boss warning message.
    /// </summary>
    public sealed class MiniBossMessageService
    {
        public const int PollingIntervalMs = 100;
        public const int TimeoutMs = 8000;
        public const string WarningMessage = "A mysterious enemy lurks\naround. Be careful!";

        private readonly IGameMemory _memory;
        private readonly IMiniBossMessageMemoryLayout _layout;

        public MiniBossMessageService(IGameMemory memory, IMiniBossMessageMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public void WaitAndDisplay(CancellationToken cancellationToken, Action<string, CancellationToken> displayMessage)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            int ms = 0;
            while (TryReadByte(_layout.HideHudAddress, out byte hideHud) && hideHud == 1 && ms < TimeoutMs)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (cancellationToken.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(PollingIntervalMs)))
                    return;

                ms += PollingIntervalMs;
            }

            displayMessage(WarningMessage, cancellationToken);
        }

        private bool TryReadByte(long address, out byte value)
        {
            var buffer = new byte[1];
            if (!_memory.TryRead(address, buffer, 0, 1))
            {
                value = 0;
                return false;
            }

            value = buffer[0];
            return true;
        }
    }
}
