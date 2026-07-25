using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Session;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// Runs a collection of <see cref="IModFeature"/> instances with a fixed
    /// tick interval and graceful cancellation.
    /// </summary>
    public sealed class ModFeatureRunner
    {
        private readonly IReadOnlyList<IModFeature> _features;
        private readonly IClock _clock;
        private readonly IFeatureExceptionHandler _exceptionHandler;

        public ModFeatureRunner(
            IReadOnlyList<IModFeature> features,
            IClock clock,
            IFeatureExceptionHandler exceptionHandler = null)
        {
            _features = features ?? throw new ArgumentNullException(nameof(features));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _exceptionHandler = exceptionHandler;
        }

        /// <summary>
        /// Initializes all features and then runs ticks until cancellation.
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
                foreach (IModFeature feature in _features)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        await feature.InitializeAsync(context, cancellationToken).ConfigureAwait(false);
                        initializedFeatures.Add(feature);
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
                    }
                    catch
                    {
                        // Shutdown failures must not prevent other features from cleaning up.
                    }
                }
            }
        }
    }
}
