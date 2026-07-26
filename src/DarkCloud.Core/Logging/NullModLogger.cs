using System;

namespace DarkCloud.Core.Logging
{
    /// <summary>
    /// No-op logger that silently discards all messages. Used as the default
    /// logger so core tests never require a real logging sink.
    /// </summary>
    public sealed class NullModLogger : IModLogger
    {
        public static IModLogger Instance { get; } = new NullModLogger();

        public void Debug(string message)
        {
        }

        public void Information(string message)
        {
        }

        public void Warning(string message)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(Exception exception, string message)
        {
        }
    }
}
