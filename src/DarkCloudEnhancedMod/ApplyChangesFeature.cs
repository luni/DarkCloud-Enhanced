using System;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Features;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Lifecycle-managed feature that applies weapon balance changes and shop
    /// price updates once when the mod enters the in-game state.
    /// </summary>
    internal sealed class ApplyChangesFeature : IModFeature
    {
        private readonly IApplyChangesService _service;
        private bool _initialized;

        public ApplyChangesFeature(IApplyChangesService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public string Id => "apply-changes";

        public async Task InitializeAsync(GameFeatureContext context, CancellationToken cancellationToken)
        {
            if (_initialized)
                return;

            await _service.ApplyChangesAsync(cancellationToken).ConfigureAwait(false);
            _initialized = true;
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
}
