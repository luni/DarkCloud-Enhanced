using System.Threading;
using System.Threading.Tasks;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// Applies the one-time weapon balance and shop price changes when the mod
    /// enters the in-game state. Implementations may initially bridge the
    /// legacy static <c>Memory</c> API and later migrate to domain services as
    /// weapon and shop logic is extracted.
    /// </summary>
    public interface IApplyChangesService
    {
        /// <summary>
        /// Applies weapon balance changes and updates shop prices.
        /// </summary>
        /// <param name="cancellationToken">Token that can cancel the operation.</param>
        Task ApplyChangesAsync(CancellationToken cancellationToken);
    }
}
