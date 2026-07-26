using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Features;
using DarkCloud.Memory.Windows;
using Xunit;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    public class FeatureModuleTests
    {
        [Theory]
        [InlineData(typeof(TownCharacterFeature))]
        [InlineData(typeof(DungeonFeature))]
        [InlineData(typeof(WeaponsFeature))]
        public void InitializeAsync_WithCanceledToken_DoesNotStartLongRunningTask(Type featureType)
        {
            IModFeature feature = (IModFeature)Activator.CreateInstance(featureType, nonPublic: true);
            var context = new GameFeatureContext(new LegacyProcessGameMemory());

            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();
                Task task = feature.InitializeAsync(context, cts.Token);
                Assert.True(task.IsCanceled);
            }
        }

        [Theory]
        [InlineData(typeof(TownCharacterFeature))]
        [InlineData(typeof(DungeonFeature))]
        [InlineData(typeof(WeaponsFeature))]
        public async Task ShutdownAsync_WithNoInitializedTask_ReturnsCompletedTask(Type featureType)
        {
            IModFeature feature = (IModFeature)Activator.CreateInstance(featureType, nonPublic: true);

            Task shutdownTask = feature.ShutdownAsync(CancellationToken.None);

            await shutdownTask;
            Assert.True(shutdownTask.IsCompleted);
            Assert.False(shutdownTask.IsFaulted);
            Assert.False(shutdownTask.IsCanceled);
        }

        [Theory]
        [InlineData(typeof(TownCharacterFeature))]
        [InlineData(typeof(DungeonFeature))]
        [InlineData(typeof(WeaponsFeature))]
        public async Task InitializeAsync_CalledTwice_DoesNotStartSecondLongRunningTask(Type featureType)
        {
            IModFeature feature = (IModFeature)Activator.CreateInstance(featureType, nonPublic: true);
            var context = new GameFeatureContext(new LegacyProcessGameMemory());

            using (var cts = new CancellationTokenSource())
            {
                await feature.InitializeAsync(context, cts.Token);
                FieldInfo taskField = featureType.GetField("_task", BindingFlags.Instance | BindingFlags.NonPublic);
                Task firstTask = (Task)taskField.GetValue(feature);
                Assert.NotNull(firstTask);

                await feature.InitializeAsync(context, cts.Token);
                Task secondTask = (Task)taskField.GetValue(feature);

                Assert.Same(firstTask, secondTask);

                cts.Cancel();
                try { await feature.ShutdownAsync(CancellationToken.None); }
                catch (OperationCanceledException) { }
            }
        }

        [Theory]
        [InlineData(typeof(TownCharacterFeature))]
        [InlineData(typeof(DungeonFeature))]
        [InlineData(typeof(WeaponsFeature))]
        public async Task ShutdownAsync_WithCanceledToken_ReturnsCanceledTask(Type featureType)
        {
            IModFeature feature = (IModFeature)Activator.CreateInstance(featureType, nonPublic: true);
            var context = new GameFeatureContext(new LegacyProcessGameMemory());

            using (var cts = new CancellationTokenSource())
            {
                await feature.InitializeAsync(context, cts.Token);

                cts.Cancel();
                Task shutdownTask = feature.ShutdownAsync(cts.Token);
                Assert.True(shutdownTask.IsCanceled);

                try { await feature.ShutdownAsync(CancellationToken.None); }
                catch (OperationCanceledException) { }
            }
        }
    }
}
