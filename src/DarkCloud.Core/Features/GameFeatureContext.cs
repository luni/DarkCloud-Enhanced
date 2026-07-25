using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// Context provided to a feature during initialization.
    /// </summary>
    public sealed class GameFeatureContext
    {
        public GameFeatureContext(IGameMemory memory)
        {
            Memory = memory;
        }

        /// <summary>
        /// The game memory backend the feature should read and write.
        /// </summary>
        public IGameMemory Memory { get; }
    }
}
