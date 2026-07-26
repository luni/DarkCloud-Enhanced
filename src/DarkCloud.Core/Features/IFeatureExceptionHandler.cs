using System;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// Receives notification when a feature fails during initialization, a
    /// tick, or shutdown. One failing feature should not terminate all features
    /// unless the host configures the runner to do so.
    /// </summary>
    public interface IFeatureExceptionHandler
    {
        /// <summary>
        /// Called when a feature throws an exception.
        /// </summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="feature">The feature that failed.</param>
        /// <param name="phase">The phase that was running, e.g. "Initialize", "Tick", or "Shutdown".</param>
        void Handle(Exception exception, IModFeature feature, string phase);
    }
}
