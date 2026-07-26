namespace DarkCloud.Core.Configuration
{
    /// <summary>
    /// Persists and loads <see cref="ModConfiguration"/> values.
    /// Implementations are responsible for file format, versioning, and migration.
    /// </summary>
    public interface IModConfigurationStore
    {
        /// <summary>
        /// Loads the configuration, returning <c>true</c> when a saved
        /// configuration was found and <c>false</c> when none exists.
        /// </summary>
        bool TryLoad(out ModConfiguration configuration);

        /// <summary>
        /// Saves the configuration, preserving any unknown settings when practical.
        /// </summary>
        void Save(ModConfiguration configuration);
    }
}
