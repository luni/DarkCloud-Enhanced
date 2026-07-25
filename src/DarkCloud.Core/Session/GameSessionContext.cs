using System.Threading;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Default implementation of <see cref="IGameSessionContext"/>.
    /// </summary>
    public sealed class GameSessionContext : IGameSessionContext
    {
        public GameSessionContext(IGameMemory memory, IAddressTranslator translator, CancellationToken cancellationToken = default)
        {
            Memory = memory;
            Translator = translator;
            CancellationToken = cancellationToken;
        }

        public IGameMemory Memory { get; }

        public IAddressTranslator Translator { get; }

        public CancellationToken CancellationToken { get; }
    }
}
