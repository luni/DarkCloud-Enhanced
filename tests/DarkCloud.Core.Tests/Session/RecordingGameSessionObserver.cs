using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Session;

namespace DarkCloud.Core.Tests.Session
{
    /// <summary>
    /// Observer that records all state changes, errors, and shutdown calls.
    /// </summary>
    public sealed class RecordingGameSessionObserver : IGameSessionObserver
    {
        public List<(GameSessionState oldState, GameSessionState newState, IGameSessionContext context)> StateChanges { get; }
            = new List<(GameSessionState, GameSessionState, IGameSessionContext)>();

        public List<Exception> Errors { get; } = new List<Exception>();

        public bool ShutdownCalled { get; private set; }

        public Task OnStateChanged(GameSessionState oldState, GameSessionState newState, IGameSessionContext context)
        {
            StateChanges.Add((oldState, newState, context));
            return Task.CompletedTask;
        }

        public void OnError(Exception exception, GameSessionState state)
        {
            Errors.Add(exception);
        }

        public Task OnShutdown(CancellationToken cancellationToken = default)
        {
            ShutdownCalled = true;
            return Task.CompletedTask;
        }
    }
}
