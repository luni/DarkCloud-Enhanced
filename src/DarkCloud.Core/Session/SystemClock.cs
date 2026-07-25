using System;
using System.Threading;
using System.Threading.Tasks;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Clock implementation backed by the real system clock.
    /// </summary>
    public sealed class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
            => Task.Delay(delay, cancellationToken);
    }
}
