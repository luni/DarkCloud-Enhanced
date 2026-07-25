using System;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Receives notifications when the session state changes.
    /// </summary>
    public interface IGameSessionObserver
    {
        /// <summary>
        /// Called whenever the detector reports a new state.
        /// </summary>
        void OnStateChanged(GameSessionState oldState, GameSessionState newState, IGameSessionContext context);

        /// <summary>
        /// Called when the runner encounters an unhandled exception.
        /// </summary>
        void OnError(Exception exception, GameSessionState state);

        /// <summary>
        /// Called once when the runner shuts down cleanly.
        /// </summary>
        void OnShutdown();
    }
}
