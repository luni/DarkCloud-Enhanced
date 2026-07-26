using System;
using System.Collections.Generic;
using System.IO;
using DarkCloud.Core.Configuration;
using DarkCloud.Core.Logging;
using Newtonsoft.Json.Linq;

namespace DarkCloudEnhancedMod.Configuration
{
    /// <summary>
    /// Stores <see cref="ModConfiguration"/> as JSON in the user's local
    /// application data directory. Unknown top-level keys are preserved across
    /// load/save cycles so newer settings are not lost when an older mod
    /// version reads the file.
    /// </summary>
    internal sealed class JsonModConfigurationStore : IModConfigurationStore
    {
        private const int CurrentVersion = 1;

        private readonly string _path;
        private readonly IModLogger _logger;
        private JObject _unknowns;

        public JsonModConfigurationStore(IModLogger logger = null)
            : this(GetDefaultPath(), logger)
        {
        }

        public JsonModConfigurationStore(string path, IModLogger logger = null)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _logger = logger ?? NullModLogger.Instance;
        }

        public bool TryLoad(out ModConfiguration configuration)
        {
            configuration = null;

            if (!File.Exists(_path))
            {
                _logger.Information($"Configuration file not found at '{_path}'; using defaults.");
                return false;
            }

            try
            {
                string json = File.ReadAllText(_path);
                JObject root = JObject.Parse(json);

                int version = root["version"]?.Value<int>() ?? 1;
                configuration = Migrate(root, version);

                _unknowns = new JObject(root);
                _unknowns.Remove("version");
                _unknowns.Remove("pollIntervalMs");
                _unknowns.Remove("features");

                return true;
            }
            catch (Exception exception)
            {
                _logger.Error(exception, $"Failed to load configuration from '{_path}'; using defaults.");
                return false;
            }
        }

        public void Save(ModConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            string directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var root = new JObject
            {
                ["version"] = CurrentVersion,
                ["pollIntervalMs"] = (int)configuration.PollInterval.TotalMilliseconds,
            };

            var features = new JObject();
            foreach (KeyValuePair<string, bool> feature in configuration.Features)
                features[feature.Key] = feature.Value;
            root["features"] = features;

            if (_unknowns != null)
            {
                foreach (KeyValuePair<string, JToken> unknown in _unknowns)
                    root[unknown.Key] = unknown.Value;
            }

            File.WriteAllText(_path, root.ToString(Newtonsoft.Json.Formatting.Indented));
        }

        private ModConfiguration Migrate(JObject root, int version)
        {
            if (version < CurrentVersion)
            {
                _logger.Information($"Migrating configuration from version {version} to {CurrentVersion}.");
                root["version"] = CurrentVersion;
            }

            int pollIntervalMs = root["pollIntervalMs"]?.Value<int>() ?? 100;
            var pollInterval = TimeSpan.FromMilliseconds(Math.Max(1, pollIntervalMs));

            var features = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            if (root["features"] is JObject featuresObject)
            {
                foreach (KeyValuePair<string, JToken> feature in featuresObject)
                {
                    if (bool.TryParse(feature.Value?.ToString(), out bool enabled))
                        features[feature.Key] = enabled;
                }
            }

            return new ModConfiguration(pollInterval, features);
        }

        private static string GetDefaultPath()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrEmpty(localAppData))
                localAppData = AppDomain.CurrentDomain.BaseDirectory;

            string configDirectory = Path.Combine(localAppData, "DarkCloud-Enhanced");
            Directory.CreateDirectory(configDirectory);
            return Path.Combine(configDirectory, "config.json");
        }
    }
}
