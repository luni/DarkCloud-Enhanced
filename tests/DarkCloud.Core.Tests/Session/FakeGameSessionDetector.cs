using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Tests.Session
{
    /// <summary>
    /// Detector that returns a configurable state for runner tests.
    /// </summary>
    public sealed class FakeGameSessionDetector : IGameSessionDetector
    {
        public GameSessionState NextState { get; set; }

        public GameSessionState Detect(IGameMemory memory, GameSessionState previousState)
        {
            return NextState;
        }

        public void ReleaseModFlag(IGameMemory memory)
        {
        }
    }
}
