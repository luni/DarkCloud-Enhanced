using System;
using System.Collections.Generic;

namespace DarkCloud.Core.Configuration
{
    /// <summary>
    /// Default configuration values for a fresh mod install.
    /// </summary>
    public static class ModConfigurationDefaults
    {
        public static ModConfiguration Create()
        {
            return new ModConfiguration(
                TimeSpan.FromMilliseconds(100),
                new Dictionary<string, bool>
                {
                    ["apply-changes"] = true,
                    ["town-character"] = true,
                    ["dungeon"] = true,
                    ["weapons-reroll"] = true,
                });
        }
    }
}
