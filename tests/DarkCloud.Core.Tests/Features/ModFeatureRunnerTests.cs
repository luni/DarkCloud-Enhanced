using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Features;
using DarkCloud.Core.Players;
using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Features
{
    public class ModFeatureRunnerTests
    {
        [Fact]
        public async Task RunAsync_InitializesAndTicksAndShutsDown()
        {
            var feature = new RecordingFeature();
            var clock = new FakeClock();
            var runner = new ModFeatureRunner(new IModFeature[] { feature }, clock);

            using (var cts = new CancellationTokenSource())
            {
                var context = new GameFeatureContext(new InMemoryGameMemory());
                var snapshot = new GameSnapshot(GameSessionState.InGame, CharacterType.Toan, true);

                Task runTask = runner.RunAsync(context, () => snapshot, TimeSpan.FromMilliseconds(1), cts.Token);

                // Let it run for two ticks.
                clock.AdvanceBy(TimeSpan.FromMilliseconds(1));
                await Task.Delay(10);
                clock.AdvanceBy(TimeSpan.FromMilliseconds(1));
                await Task.Delay(10);

                cts.Cancel();
                try { await runTask; }
                catch (OperationCanceledException) { }
            }

            Assert.True(feature.Initialized);
            Assert.True(feature.TickCount >= 2);
            Assert.True(feature.ShutDown);
            Assert.Equal("InGame", feature.LastSnapshotState);
        }

        [Fact]
        public async Task RunAsync_FailingFeatureOnTick_DoesNotStopOtherFeatures()
        {
            var failingFeature = new FailingFeature(failOnTick: true);
            var recordingFeature = new RecordingFeature();
            var clock = new FakeClock();
            var handler = new CollectingExceptionHandler();
            var runner = new ModFeatureRunner(new IModFeature[] { failingFeature, recordingFeature }, clock, handler);

            using (var cts = new CancellationTokenSource())
            {
                var context = new GameFeatureContext(new InMemoryGameMemory());
                var snapshot = new GameSnapshot(GameSessionState.InGame, CharacterType.Toan, true);

                Task runTask = runner.RunAsync(context, () => snapshot, TimeSpan.FromMilliseconds(1), cts.Token);

                clock.AdvanceBy(TimeSpan.FromMilliseconds(1));
                await Task.Delay(10);

                cts.Cancel();
                try { await runTask; }
                catch (OperationCanceledException) { }
            }

            Assert.True(recordingFeature.Initialized);
            Assert.True(recordingFeature.TickCount >= 1);
            Assert.True(recordingFeature.ShutDown);
            Assert.True(failingFeature.ShutDown);
            Assert.NotEmpty(handler.Errors);
            Assert.All(handler.Errors, e => Assert.Equal("Tick", e.Phase));
        }

        [Fact]
        public async Task RunAsync_FailingFeatureOnInit_DoesNotStopOtherFeatures()
        {
            var failingFeature = new FailingFeature(failOnInit: true);
            var recordingFeature = new RecordingFeature();
            var clock = new FakeClock();
            var handler = new CollectingExceptionHandler();
            var runner = new ModFeatureRunner(new IModFeature[] { failingFeature, recordingFeature }, clock, handler);

            using (var cts = new CancellationTokenSource())
            {
                var context = new GameFeatureContext(new InMemoryGameMemory());
                var snapshot = new GameSnapshot(GameSessionState.InGame, CharacterType.Toan, true);

                Task runTask = runner.RunAsync(context, () => snapshot, TimeSpan.FromMilliseconds(1), cts.Token);

                clock.AdvanceBy(TimeSpan.FromMilliseconds(1));
                await Task.Delay(10);

                cts.Cancel();
                try { await runTask; }
                catch (OperationCanceledException) { }
            }

            Assert.True(recordingFeature.Initialized);
            Assert.True(recordingFeature.TickCount >= 1);
            Assert.True(recordingFeature.ShutDown);
            Assert.False(failingFeature.Initialized);
            Assert.False(failingFeature.ShutDown);
            Assert.Single(handler.Errors);
            Assert.Equal("Initialize", handler.Errors[0].Phase);
        }

        [Fact]
        public async Task RunAsync_ShutsDownInitializedFeatures_WhenCancellationOccursDuringInit()
        {
            var initializedFeature = new RecordingFeature();
            var neverInitFeature = new BlockingFeature();
            var clock = new FakeClock();
            var runner = new ModFeatureRunner(new IModFeature[] { initializedFeature, neverInitFeature }, clock);

            using (var cts = new CancellationTokenSource())
            {
                var context = new GameFeatureContext(new InMemoryGameMemory());
                var snapshot = new GameSnapshot(GameSessionState.InGame, CharacterType.Toan, true);

                Task runTask = runner.RunAsync(context, () => snapshot, TimeSpan.FromMilliseconds(1), cts.Token);

                await Task.Delay(10);
                cts.Cancel();
                try { await runTask; }
                catch (OperationCanceledException) { }
            }

            Assert.True(initializedFeature.Initialized);
            Assert.True(initializedFeature.ShutDown);
            Assert.False(neverInitFeature.Initialized);
        }

        private sealed class RecordingFeature : IModFeature
        {
            public string Id => "recording";
            public bool Initialized { get; private set; }
            public int TickCount { get; private set; }
            public bool ShutDown { get; private set; }
            public string LastSnapshotState { get; private set; }

            public Task InitializeAsync(GameFeatureContext context, CancellationToken cancellationToken)
            {
                Initialized = true;
                return Task.CompletedTask;
            }

            public Task OnGameTickAsync(GameSnapshot snapshot, CancellationToken cancellationToken)
            {
                TickCount++;
                LastSnapshotState = snapshot.SessionState.ToString();
                return Task.CompletedTask;
            }

            public Task ShutdownAsync(CancellationToken cancellationToken)
            {
                ShutDown = true;
                return Task.CompletedTask;
            }
        }

        private sealed class FailingFeature : IModFeature
        {
            private readonly bool _failOnInit;
            private readonly bool _failOnTick;

            public FailingFeature(bool failOnInit = false, bool failOnTick = false)
            {
                _failOnInit = failOnInit;
                _failOnTick = failOnTick;
            }

            public string Id => "failing";
            public bool Initialized { get; private set; }
            public bool ShutDown { get; private set; }

            public Task InitializeAsync(GameFeatureContext context, CancellationToken cancellationToken)
            {
                if (_failOnInit)
                    throw new InvalidOperationException("Init failure");

                Initialized = true;
                return Task.CompletedTask;
            }

            public Task OnGameTickAsync(GameSnapshot snapshot, CancellationToken cancellationToken)
            {
                if (_failOnTick)
                    throw new InvalidOperationException("Tick failure");

                return Task.CompletedTask;
            }

            public Task ShutdownAsync(CancellationToken cancellationToken)
            {
                ShutDown = true;
                return Task.CompletedTask;
            }
        }

        private sealed class BlockingFeature : IModFeature
        {
            public string Id => "blocking";
            public bool Initialized { get; private set; }

            public async Task InitializeAsync(GameFeatureContext context, CancellationToken cancellationToken)
            {
                try
                {
                    await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
                    Initialized = true;
                }
                catch (OperationCanceledException)
                {
                    // Expected when the test cancels initialization.
                }
            }

            public Task OnGameTickAsync(GameSnapshot snapshot, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task ShutdownAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        private sealed class FakeClock : IClock
        {
            private readonly Queue<TaskCompletionSource<object>> _pendingDelays = new Queue<TaskCompletionSource<object>>();

            public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

            public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
            {
                var tcs = new TaskCompletionSource<object>();
                cancellationToken.Register(() => tcs.TrySetCanceled());
                _pendingDelays.Enqueue(tcs);
                return tcs.Task;
            }

            public void AdvanceBy(TimeSpan amount)
            {
                if (_pendingDelays.Count > 0)
                {
                    _pendingDelays.Dequeue().TrySetResult(null);
                }
            }
        }

        private sealed class CollectingExceptionHandler : IFeatureExceptionHandler
        {
            public List<(Exception Exception, IModFeature Feature, string Phase)> Errors { get; } =
                new List<(Exception, IModFeature, string)>();

            public void Handle(Exception exception, IModFeature feature, string phase)
            {
                Errors.Add((exception, feature, phase));
            }
        }
    }
}
