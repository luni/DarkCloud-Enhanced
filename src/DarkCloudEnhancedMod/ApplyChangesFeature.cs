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
        public string Id => "apply-changes";

        public Task InitializeAsync(GameFeatureContext context, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            Weapons.WeaponsBalanceChanges();

            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            Shop.UpdateShopPrices();

            return Task.CompletedTask;
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
