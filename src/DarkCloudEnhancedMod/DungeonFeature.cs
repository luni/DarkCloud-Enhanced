using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Features;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Lifecycle-managed module that runs the legacy dungeon script on a
    /// background task. This is a migration wrapper around
    /// <see cref="Dungeon.InsideDungeonThread(CancellationToken)"/>; the domain
    /// logic will be extracted into <see cref="DarkCloud.Core"/> in Phase 10.4.
    /// </summary>
    internal sealed class DungeonFeature : IModFeature
    {
        private Task _task;

        public string Id => "dungeon";

        public Task InitializeAsync(GameFeatureContext context, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            if (_task != null)
                return Task.CompletedTask;

            _task = Task.Factory.StartNew(
                () => Dungeon.InsideDungeonThread(cancellationToken),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            return Task.CompletedTask;
        }

        public Task OnGameTickAsync(GameSnapshot snapshot, CancellationToken cancellationToken)
        {
            // The legacy script runs on its own long-running task. If it has faulted,
            // surface the exception through the runner's tick handler so it is logged
            // and does not take down the whole process.
            Task task = _task;
            if (task != null && task.IsFaulted)
            {
                _task = null;
                return task;
            }

            return Task.CompletedTask;
        }

        public Task ShutdownAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            return _task ?? Task.CompletedTask;
        }
    }
}
