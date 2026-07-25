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
        private int _lastProcessId;

        public IGameMemory Current => LegacyProcessGameMemory.Instance;

        public bool TryRefresh()
        {
            bool reinitialized = NeedsReinitialization();
            if (reinitialized)
            {
                Memory.Initialize();
            }

            Process process = Memory.emulatorProcess;
            if (process == null)
            {
                _lastProcessId = 0;
                return false;
            }

            int currentId;
            try
            {
                currentId = process.Id;
            }
            catch (InvalidOperationException)
            {
                _lastProcessId = 0;
                return false;
            }

            // When the provider reconnects to a different emulator process,
            // force one disconnected tick so the detector resets its ownership
            // of the mutual-exclusion flag before claiming it on the new process.
            if (reinitialized && _lastProcessId != 0 && currentId != _lastProcessId)
            {
                _lastProcessId = currentId;
                return false;
            }

            _lastProcessId = currentId;
            return true;
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
