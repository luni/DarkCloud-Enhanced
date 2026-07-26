using System;
using DarkCloud.Core.Logging;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// Routes feature failures to an <see cref="IModLogger"/>.
    /// </summary>
    public sealed class ModLoggerExceptionHandler : IFeatureExceptionHandler
    {
        private readonly IModLogger _logger;

        public ModLoggerExceptionHandler(IModLogger logger)
        {
            _logger = logger ?? NullModLogger.Instance;
        }

        public void Handle(Exception exception, IModFeature feature, string phase)
        {
            _logger.Error(exception, $"Feature '{feature.Id}' {phase} error.");
        }
    }
}
