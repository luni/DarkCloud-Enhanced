using System;

namespace DarkCloud.Memory.Windows
{
    /// <summary>
    /// Provides a formatted UTC timestamp for console logs, matching the legacy
    /// <c>ReusableFunctions.GetDateTimeForLog</c> output without depending on it.
    /// </summary>
    internal static class LogTimestamp
    {
        public static string Now()
        {
            return "[" + DateTime.Parse(DateTime.UtcNow.ToString()).ToString("HH:mm:ss") + "] ";
        }
    }
}
