using System.Threading;
using System.Threading.Tasks;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// A lifecycle-managed mod feature that replaces an independent background thread.
    /// </summary>
    public interface IModFeature
    {
        /// <summary>
        /// Unique feature identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Called once when the feature is first enabled.
        /// </summary>
        Task InitializeAsync(
            GameFeatureContext context,
            CancellationToken cancellationToken);

        /// <summary>
        /// Called on every feature tick while the game session is active.
        /// </summary>
        Task OnGameTickAsync(
            GameSnapshot snapshot,
            CancellationToken cancellationToken);

        /// <summary>
        /// Called when the feature is disabled or the session ends.
        /// </summary>
        Task ShutdownAsync(
            CancellationToken cancellationToken);
    }
}
