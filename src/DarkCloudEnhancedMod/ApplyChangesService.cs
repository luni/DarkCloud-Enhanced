using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Features;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Bridges the legacy <see cref="Weapons"/> and <see cref="Shop"/> static
    /// memory writes behind the <see cref="IApplyChangesService"/> abstraction.
    /// This lets <see cref="ApplyChangesFeature"/> be tested without depending
    /// directly on the legacy static <c>Memory</c> class. The implementation
    /// will be replaced with domain services as Phase 10.3 progresses.
    /// </summary>
    internal sealed class ApplyChangesService : IApplyChangesService
    {
        public Task ApplyChangesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Weapons.WeaponsBalanceChanges();

            cancellationToken.ThrowIfCancellationRequested();

            Shop.UpdateShopPrices();

            return Task.CompletedTask;
        }
    }
}
