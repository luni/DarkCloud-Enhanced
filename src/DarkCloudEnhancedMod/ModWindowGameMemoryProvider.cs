using System;
using System.Diagnostics;
using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Provides the current <see cref="IGameMemory"/> for the session runner,
    /// refreshing the underlying emulator connection when it is missing or dead.
    /// </summary>
    internal sealed class ModWindowGameMemoryProvider : IGameMemoryProvider
    {
        public IGameMemory Current => LegacyProcessGameMemory.Instance;

        public bool TryRefresh()
        {
            if (NeedsReinitialization())
            {
                Memory.Initialize();
            }

            return Memory.emulatorProcess != null;
        }

        private static bool NeedsReinitialization()
        {
            if (Memory.emulatorProcess == null)
                return true;

            try
            {
                if (Memory.emulatorProcess.HasExited)
                    return true;
            }
            catch (InvalidOperationException)
            {
                // Process has not been started or exited already.
                return true;
            }
            catch (NotSupportedException)
            {
                // Platform (including Mono on some Linux configurations) may not
                // support HasExited for this process.
            }

            return false;
        }
    }
}
