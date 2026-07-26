using System;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// Pairs a lifecycle-managed <see cref="IModFeature"/> with its metadata.
    /// </summary>
    public sealed class ModFeature
    {
        public ModFeature(IModFeature implementation, ModFeatureDescriptor descriptor = null)
        {
            Implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
            Descriptor = descriptor;
        }

        /// <summary>
        /// The feature implementation that the runner will invoke.
        /// </summary>
        public IModFeature Implementation { get; }

        /// <summary>
        /// Optional metadata describing the feature.
        /// </summary>
        public ModFeatureDescriptor Descriptor { get; }
    }
}
