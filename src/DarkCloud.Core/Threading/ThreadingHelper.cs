using System;
using System.Threading;

namespace DarkCloud.Core.Threading
{
    /// <summary>
    /// Helpers for managing cancellable background work. These utilities keep
    /// background threads responsive to shutdown while avoiding uninterruptible
    /// <see cref="Thread.Sleep"/> calls.
    /// </summary>
    public static class ThreadingHelper
    {
        /// <summary>
        /// Sleeps up to the specified timeout, returning immediately if the
        /// cancellation token is cancelled.
        /// </summary>
        public static void Sleep(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (millisecondsTimeout <= 0 || cancellationToken.IsCancellationRequested)
                return;

            try
            {
                // WaitOne returns immediately when the token is already cancelled.
                // Thread.Interrupt can also wake it during RestartThread, in which
                // case the caller checks IsCancellationRequested and exits.
                cancellationToken.WaitHandle.WaitOne(millisecondsTimeout);
            }
            catch (ThreadInterruptedException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Replaces a static thread field with a new thread. The old thread is
        /// interrupted and joined with a short timeout, and the new thread is
        /// marked as a background thread. The swap is atomic so only one new
        /// thread is started when multiple callers race.
        /// </summary>
        public static void RestartThread(ref Thread field, ThreadStart start, int joinTimeoutMs = 100)
        {
            Thread oldThread = Interlocked.Exchange(ref field, null);
            if (oldThread != null && oldThread.IsAlive)
            {
                try
                {
                    oldThread.Interrupt();
                }
                catch (ThreadStateException)
                {
                }

                oldThread.Join(joinTimeoutMs);
            }

            Thread newThread = new Thread(start) { IsBackground = true };
            Thread existing = Interlocked.CompareExchange(ref field, newThread, null);
            if (existing != null)
            {
                // Another caller already installed a new thread for this field.
                return;
            }

            newThread.Start();
        }
    }
}
