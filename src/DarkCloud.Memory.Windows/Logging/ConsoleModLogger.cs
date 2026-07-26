using System;
using DarkCloud.Core.Logging;

namespace DarkCloudEnhancedMod.Logging
{
    /// <summary>
    /// Routes structured log messages to the console. This is the default
    /// diagnostic sink until a file or UI logger is wired in.
    /// </summary>
    internal sealed class ConsoleModLogger : IModLogger
    {
        public void Debug(string message)
        {
            Console.WriteLine($"[DBG] {message}");
        }

        public void Information(string message)
        {
            Console.WriteLine($"[INF] {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"[WRN] {message}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"[ERR] {message}");
        }

        public void Error(Exception exception, string message)
        {
            Console.WriteLine($"[ERR] {message}");
            if (exception != null)
                Console.WriteLine(exception);
        }
    }
}
