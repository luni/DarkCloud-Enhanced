namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Provides an OS-level check for whether this process is the only running
    /// instance of the mod. Implementations typically use a named mutex or a
    /// file lock and must be process-wide.
    /// </summary>
    public interface IModInstanceProvider
    {
        /// <summary>
        /// Returns <c>true</c> when this process is the only mod instance
        /// currently running; <c>false</c> when another instance is active.
        /// </summary>
        bool IsOnlyInstance();
    }
}
