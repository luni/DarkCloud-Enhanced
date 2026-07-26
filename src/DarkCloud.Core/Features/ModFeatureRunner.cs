using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Logging;
using DarkCloud.Core.Session;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// Runs a collection of <see cref="ModFeature"/> entries with a fixed
    /// tick interval and graceful cancellation. Features whose metadata marks
    /// them as disabled are not initialized or ticked.
    /// </summary>
    public sealed class ModFeatureRunner
    {
        private readonly IReadOnlyList<ModFeature> _features;
        private readonly IClock _clock;
        private readonly IFeatureExceptionHandler _exceptionHandler;
        private readonly IModLogger _logger;

        public ModFeatureRunner(
            IReadOnlyList<ModFeature> features,
            IClock clock,
            IFeatureExceptionHandler exceptionHandler = null,
            IModLogger logger = null)
        {
            _features = features ?? throw new ArgumentNullException(nameof(features));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _exceptionHandler = exceptionHandler;
            _logger = logger ?? NullModLogger.Instance;
        }

        /// <summary>
        /// Initializes all enabled features and then runs ticks until cancellation.
        /// </summary>
        public async Task RunAsync(
            GameFeatureContext context,
            Func<GameSnapshot> snapshotFactory,
            TimeSpan tickInterval,
            CancellationToken cancellationToken)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (snapshotFactory == null)
                throw new ArgumentNullException(nameof(snapshotFactory));

            var initializedFeatures = new List<IModFeature>(_features.Count);

            try
            {
                foreach (ModFeature entry in _features)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (entry.Descriptor?.EnabledByDefault == false)
                        continue;

                    IModFeature feature = entry.Implementation;

                    try
                    {
                        await feature.InitializeAsync(context, cancellationToken).ConfigureAwait(false);
                        initializedFeatures.Add(feature);
                        _logger.Information($"Feature '{feature.Id}' initialized.");
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        _exceptionHandler?.Handle(exception, feature, "Initialize");
                    }
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    GameSnapshot snapshot = snapshotFactory();

                    foreach (IModFeature feature in initializedFeatures)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        try
                        {
                            await feature.OnGameTickAsync(snapshot, cancellationToken).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception exception)
                        {
                            _exceptionHandler?.Handle(exception, feature, "Tick");
                        }
                    }

                    await _clock.Delay(tickInterval, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                foreach (IModFeature feature in initializedFeatures)
                {
                    try
                    {
                        await feature.ShutdownAsync(CancellationToken.None).ConfigureAwait(false);
                        _logger.Information($"Feature '{feature.Id}' shut down.");
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when a feature's background task observes cancellation during shutdown.
                    }
                    catch (Exception exception)
                    {
                        _exceptionHandler?.Handle(exception, feature, "Shutdown");
                    }
                }
            }
        }
    }
}
