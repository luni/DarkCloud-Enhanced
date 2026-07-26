using System;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Logging;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Polls an <see cref="IGameMemoryProvider"/> and drives the session state
    /// machine. The runner never blocks threads with <see cref="Thread.Sleep"/>;
    /// all delays are asynchronous through <see cref="IClock"/>.
    /// </summary>
    public sealed class GameSessionRunner
    {
        private readonly IGameMemoryProvider _memoryProvider;
        private readonly IGameSessionDetector _detector;
        private readonly IGameSessionObserver _observer;
        private readonly IClock _clock;
        private readonly IAddressTranslator _translator;
        private readonly Func<GameSessionState, TimeSpan> _delaySelector;
        private readonly IModLogger _logger;

        public GameSessionRunner(
            IGameMemoryProvider memoryProvider,
            IGameSessionDetector detector,
            IGameSessionObserver observer,
            IClock clock,
            IAddressTranslator translator = null,
            Func<GameSessionState, TimeSpan> delaySelector = null,
            IModLogger logger = null)
        {
            _memoryProvider = memoryProvider ?? throw new ArgumentNullException(nameof(memoryProvider));
            _detector = detector ?? throw new ArgumentNullException(nameof(detector));
            _observer = observer ?? throw new ArgumentNullException(nameof(observer));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _translator = translator ?? new PassthroughAddressTranslator();
            _delaySelector = delaySelector ?? DefaultDelaySelector;
            _logger = logger ?? NullModLogger.Instance;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var state = GameSessionState.None;
            IGameMemory lastMemory = null;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    IGameMemory memory = null;
                    GameSessionState newState;

                    try
                    {
                        bool connected = _memoryProvider.TryRefresh();
                        memory = connected ? _memoryProvider.Current : null;
                        newState = _detector.Detect(memory, state);
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(exception, "Session runner tick error.");
                        _observer.OnError(exception, state);
                        newState = state;
                    }

                    if (memory != null)
                        lastMemory = memory;

                    if (newState != state)
                    {
                        _logger.Information($"Session state changed from {state} to {newState}.");

                        var context = new GameSessionContext(memory, _translator, cancellationToken);
                        try
                        {
                            await _observer.OnStateChanged(state, newState, context);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }

                        state = newState;
                    }

                    TimeSpan delay = _delaySelector(state);

                    try
                    {
                        await _clock.Delay(delay, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
            finally
            {
                _logger.Information("Session runner shutting down.");

                try
                {
                    await _observer.OnShutdown(cancellationToken);
                }
                finally
                {
                    _detector.ReleaseModFlag(lastMemory);
                }
            }
        }

        private static TimeSpan DefaultDelaySelector(GameSessionState state)
        {
            switch (state)
            {
                case GameSessionState.NoEmulator:
                case GameSessionState.EmulatorExited:
                case GameSessionState.EmulatorWithoutGame:
                    return TimeSpan.FromSeconds(1);
                default:
                    return TimeSpan.FromMilliseconds(1);
            }
        }
    }
}
