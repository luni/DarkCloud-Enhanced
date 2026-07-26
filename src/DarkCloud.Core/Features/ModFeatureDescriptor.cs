using System;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// Metadata describing a mod feature. This is kept separate from the
    /// <see cref="IModFeature"/> implementation so that UI, logging, and
    /// enablement decisions can use feature metadata without depending on the
    /// feature's runtime behavior.
    /// </summary>
    public sealed class ModFeatureDescriptor
    {
        public ModFeatureDescriptor(string id, string displayName, bool enabledByDefault = true)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Feature id must not be empty.", nameof(id));

            Id = id;
            DisplayName = displayName ?? id;
            EnabledByDefault = enabledByDefault;
        }

        /// <summary>
        /// Unique identifier for the feature. This should match the
        /// <see cref="IModFeature.Id"/> it describes.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Human-readable name for the feature.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Whether the feature should be enabled when the mod starts. Runners
        /// can override this with user configuration in later phases.
        /// </summary>
        public bool EnabledByDefault { get; }
    }
}
