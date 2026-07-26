using System;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Features;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    public class ApplyChangesFeatureTests
    {
        [Fact]
        public async Task InitializeAsync_CallsService()
        {
            var service = new RecordingApplyChangesService();
            var feature = new ApplyChangesFeature(service);
            var context = new GameFeatureContext(new InMemoryGameMemory());

            await feature.InitializeAsync(context, CancellationToken.None);

            Assert.True(service.Called);
        }

        [Fact]
        public async Task InitializeAsync_RespectsCancellationToken()
        {
            var service = new RecordingApplyChangesService();
            var feature = new ApplyChangesFeature(service);
            var context = new GameFeatureContext(new InMemoryGameMemory());

            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();
                await Assert.ThrowsAsync<OperationCanceledException>(() => feature.InitializeAsync(context, cts.Token));
            }
        }

        [Fact]
        public async Task InitializeAsync_CalledTwice_CallsServiceOnlyOnce()
        {
            var service = new RecordingApplyChangesService();
            var feature = new ApplyChangesFeature(service);
            var context = new GameFeatureContext(new InMemoryGameMemory());

            await feature.InitializeAsync(context, CancellationToken.None);
            await feature.InitializeAsync(context, CancellationToken.None);

            Assert.Equal(1, service.CallCount);
        }

        private sealed class RecordingApplyChangesService : IApplyChangesService
        {
            public bool Called => CallCount > 0;
            public int CallCount { get; private set; }

            public Task ApplyChangesAsync(CancellationToken cancellationToken)
            {
                CallCount++;
                cancellationToken.ThrowIfCancellationRequested();
                return Task.CompletedTask;
            }
        }
    }
}
