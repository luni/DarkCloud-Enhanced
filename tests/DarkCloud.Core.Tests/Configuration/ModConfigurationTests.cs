using System;
using System.Collections.Generic;
using DarkCloud.Core.Configuration;
using Xunit;

namespace DarkCloud.Core.Tests.Configuration
{
    public class ModConfigurationTests
    {
        [Fact]
        public void Constructor_ValidArguments_SetsProperties()
        {
            var features = new Dictionary<string, bool> { ["dungeon"] = false };
            var config = new ModConfiguration(TimeSpan.FromMilliseconds(250), features);

            Assert.Equal(TimeSpan.FromMilliseconds(250), config.PollInterval);
            Assert.False(config.Features["dungeon"]);
        }

        [Fact]
        public void Constructor_NonPositivePollInterval_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ModConfiguration(TimeSpan.Zero, new Dictionary<string, bool>()));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ModConfiguration(TimeSpan.FromMilliseconds(-1), new Dictionary<string, bool>()));
        }

        [Fact]
        public void Constructor_NullFeatures_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ModConfiguration(TimeSpan.FromMilliseconds(100), null));
        }

        [Fact]
        public void Defaults_ProvidesPositivePollIntervalAndKnownFeaturesEnabled()
        {
            ModConfiguration defaults = ModConfigurationDefaults.Create();

            Assert.True(defaults.PollInterval > TimeSpan.Zero);
            Assert.All(defaults.Features, f => Assert.True(f.Value));
        }
    }
}
