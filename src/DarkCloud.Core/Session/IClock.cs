using System;
using System.Threading;
using System.Threading.Tasks;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Abstracts time so the session runner can be tested without real delays.
    /// </summary>
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
        Task Delay(TimeSpan delay, CancellationToken cancellationToken);
    }
}
