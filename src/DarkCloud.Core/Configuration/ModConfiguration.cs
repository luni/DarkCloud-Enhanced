using System;
using System.Collections.Generic;

namespace DarkCloud.Core.Configuration
{
    /// <summary>
    /// Runtime configuration for the mod, including the session polling interval
    /// and per-feature enablement.
    /// </summary>
    public sealed class ModConfiguration
    {
        public ModConfiguration(TimeSpan pollInterval, IReadOnlyDictionary<string, bool> features)
        {
            if (pollInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(pollInterval), "Poll interval must be positive.");
            if (features == null)
                throw new ArgumentNullException(nameof(features));

            PollInterval = pollInterval;
            Features = features;
        }

        /// <summary>
        /// Delay between session state machine polls.
        /// </summary>
        public TimeSpan PollInterval { get; }

        /// <summary>
        /// Feature enablement keyed by feature identifier.
        /// </summary>
        public IReadOnlyDictionary<string, bool> Features { get; }
    }
}
