using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Features;
using DarkCloud.Core.Logging;

namespace DarkCloud.App.WinForms
{
    /// <summary>
    /// Pilot <see cref="IModFeature"/> that logs when it starts and on each tick.
    /// This demonstrates the feature-runner infrastructure in the modern host
    /// without depending on legacy game logic.
    /// </summary>
    public sealed class StatusLogFeature : IModFeature
    {
        private readonly IModLogger _logger;

        public StatusLogFeature(IModLogger logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public string Id => "status-log";

        public Task InitializeAsync(GameFeatureContext context, CancellationToken cancellationToken)
        {
            _logger.Information("Status log feature initialized.");
            return Task.CompletedTask;
        }

        public Task OnGameTickAsync(GameSnapshot snapshot, CancellationToken cancellationToken)
        {
            _logger.Information(
                $"Tick: state={snapshot.SessionState}, character={snapshot.CurrentCharacter}, inDungeon={snapshot.InDungeonFloor}");
            return Task.CompletedTask;
        }

        public Task ShutdownAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Status log feature shut down.");
            return Task.CompletedTask;
        }
    }
}
