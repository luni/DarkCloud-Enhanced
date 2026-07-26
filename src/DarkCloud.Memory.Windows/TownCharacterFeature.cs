using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Features;
using DarkCloudEnhancedMod;

namespace DarkCloud.Memory.Windows
{
    /// <summary>
    /// Lifecycle-managed module that runs the legacy town/overworld script on a
    /// background task. The domain logic lives in <see cref="TownCharacter"/> and is
    /// reused by both the legacy and modern hosts.
    /// </summary>
    internal sealed class TownCharacterFeature : IModFeature
    {
        private Task _task;

        public string Id => "town-character";

        public Task InitializeAsync(GameFeatureContext context, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            if (_task != null)
                return Task.CompletedTask;

            _task = Task.Factory.StartNew(
                () => TownCharacter.MainScript(cancellationToken),
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
