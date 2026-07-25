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
        private LegacyProcessGameMemory _cachedMemory;
        private bool _isConnected;

        public IGameMemory Current => _isConnected ? _cachedMemory : null;

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
                _isConnected = false;
                return false;
            }

            int currentId;
            try
            {
                currentId = process.Id;
            }
            catch (InvalidOperationException)
            {
                _isConnected = false;
                return false;
            }

            // Keep the same IGameMemory instance across transient disconnects so
            // the detector does not lose ownership. Create a new instance only
            // when the process identity actually changes; then force one
            // disconnected tick so the runner observes the transition before
            // claiming the flag on the new process.
            bool processChanged = _lastProcessId != 0 && currentId != _lastProcessId;
            if (_cachedMemory == null || processChanged)
            {
                _cachedMemory = new LegacyProcessGameMemory();
                _lastProcessId = currentId;
            }

            if (processChanged)
            {
                _isConnected = false;
                return false;
            }

            _isConnected = true;
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
