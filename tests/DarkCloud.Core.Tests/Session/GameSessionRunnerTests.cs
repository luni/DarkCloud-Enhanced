using System;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Session
{
    public class GameSessionRunnerTests
    {
        [Fact]
        public async Task RunAsync_StateChange_NotifiesObserver()
        {
            var clock = new FakeClock(DateTimeOffset.UtcNow);
            var provider = new FakeGameMemoryProvider
            {
                Current = new InMemoryGameMemory(0x20000000L, 0x02000000)
            };
            var detector = new FakeGameSessionDetector { NextState = GameSessionState.NoEmulator };
            var observer = new RecordingGameSessionObserver();
            var runner = new GameSessionRunner(provider, detector, observer, clock);

            var cts = new CancellationTokenSource();
            var runTask = runner.RunAsync(cts.Token);

            // First tick (NoEmulator) is executed synchronously before the first Delay.
            clock.Advance(TimeSpan.FromSeconds(1));
            detector.NextState = GameSessionState.EmulatorWithoutGame;
            clock.Advance(TimeSpan.FromSeconds(1));

            cts.Cancel();
            await runTask;

            Assert.Equal(2, observer.StateChanges.Count);
            Assert.Equal(GameSessionState.None, observer.StateChanges[0].oldState);
            Assert.Equal(GameSessionState.NoEmulator, observer.StateChanges[0].newState);
            Assert.Equal(GameSessionState.NoEmulator, observer.StateChanges[1].oldState);
            Assert.Equal(GameSessionState.EmulatorWithoutGame, observer.StateChanges[1].newState);
        }

        [Fact]
        public async Task RunAsync_Cancellation_StopsAndCallsShutdown()
        {
            var clock = new FakeClock(DateTimeOffset.UtcNow);
            var provider = new FakeGameMemoryProvider();
            var detector = new FakeGameSessionDetector { NextState = GameSessionState.NoEmulator };
            var observer = new RecordingGameSessionObserver();
            var runner = new GameSessionRunner(provider, detector, observer, clock);

            var cts = new CancellationTokenSource();
            var runTask = runner.RunAsync(cts.Token);

            cts.Cancel();
            await runTask;

            Assert.True(observer.ShutdownCalled);
        }

        [Fact]
        public async Task RunAsync_DetectorError_CallsOnErrorAndContinues()
        {
            var clock = new FakeClock(DateTimeOffset.UtcNow);
            var provider = new FakeGameMemoryProvider();
            var detector = new ThrowingDetector();
            var observer = new RecordingGameSessionObserver();
            var runner = new GameSessionRunner(provider, detector, observer, clock);

            var cts = new CancellationTokenSource();
            var runTask = runner.RunAsync(cts.Token);

            // The first tick is executed synchronously and records the error.
            Assert.Single(observer.Errors);
            Assert.IsType<InvalidOperationException>(observer.Errors[0]);

            cts.Cancel();
            await runTask;
        }

        [Fact]
        public async Task RunAsync_DelaySelector_UsesProvidedDelays()
        {
            var clock = new FakeClock(DateTimeOffset.UtcNow);
            var provider = new FakeGameMemoryProvider();
            var detector = new FakeGameSessionDetector { NextState = GameSessionState.NoEmulator };
            var observer = new RecordingGameSessionObserver();
            var runner = new GameSessionRunner(
                provider,
                detector,
                observer,
                clock,
                delaySelector: _ => TimeSpan.FromMilliseconds(50));

            var cts = new CancellationTokenSource();
            var runTask = runner.RunAsync(cts.Token);

            // First tick (None -> NoEmulator) runs synchronously, then the custom 50 ms delay is scheduled.
            clock.Advance(TimeSpan.FromMilliseconds(50));

            cts.Cancel();
            await runTask;

            Assert.True(observer.StateChanges.Count >= 1);
        }

        private sealed class ThrowingDetector : IGameSessionDetector
        {
            public GameSessionState Detect(IGameMemory memory, GameSessionState previousState)
            {
                throw new InvalidOperationException("detector failure");
            }

            public void ReleaseModFlag(IGameMemory memory)
            {
            }
        }
    }
}
