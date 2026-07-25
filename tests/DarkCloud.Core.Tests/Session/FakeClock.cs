using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Session;

namespace DarkCloud.Core.Tests.Session
{
    /// <summary>
    /// Clock implementation for tests that supports explicit time advancement
    /// and asynchronous cancellation.
    /// </summary>
    public sealed class FakeClock : IClock
    {
        private DateTimeOffset _utcNow;
        private readonly List<ScheduledDelay> _delays = new List<ScheduledDelay>();

        public FakeClock(DateTimeOffset? start = null)
        {
            _utcNow = start ?? DateTimeOffset.UtcNow;
        }

        public DateTimeOffset UtcNow => _utcNow;

        public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            var source = new TaskCompletionSource<object>();
            var registration = cancellationToken.Register(() => source.TrySetCanceled());
            _delays.Add(new ScheduledDelay { Due = _utcNow + delay, Source = source, Registration = registration });
            return source.Task;
        }

        public void Advance(TimeSpan amount)
        {
            _utcNow += amount;

            var ready = _delays.Where(d => d.Due <= _utcNow).ToList();
            _delays.RemoveAll(d => d.Due <= _utcNow);

            foreach (var delay in ready)
            {
                delay.Registration.Dispose();
                delay.Source.TrySetResult(null);
            }
        }

        private class ScheduledDelay
        {
            public DateTimeOffset Due { get; set; }
            public TaskCompletionSource<object> Source { get; set; }
            public CancellationTokenRegistration Registration { get; set; }
        }
    }
}
