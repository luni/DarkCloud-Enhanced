using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Default implementation of <see cref="IGameSessionContext"/>.
    /// </summary>
    public sealed class GameSessionContext : IGameSessionContext
    {
        public GameSessionContext(IGameMemory memory, IAddressTranslator translator)
        {
            Memory = memory;
            Translator = translator;
        }

        public IGameMemory Memory { get; }

        public IAddressTranslator Translator { get; }
    }
}
