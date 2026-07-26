using System;

namespace DarkCloud.Core.Logging
{
    /// <summary>
    /// Structured diagnostic logger used by the mod. Implementations are
    /// responsible for routing messages to the console, a file, the UI, or
    /// another diagnostic sink.
    /// </summary>
    public interface IModLogger
    {
        void Debug(string message);
        void Information(string message);
        void Warning(string message);
        void Error(string message);
        void Error(Exception exception, string message);
    }
}
