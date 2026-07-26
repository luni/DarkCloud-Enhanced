using System;
using System.IO;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Uses an exclusive file lock to determine whether this process is the only
    /// running instance of the mod. The lock is held for the lifetime of the
    /// process and is automatically released by the OS if the process crashes.
    /// This works cross-platform, including on Mono, where named mutexes are not
    /// process-global.
    /// </summary>
    public sealed class FileLockModInstanceProvider : IModInstanceProvider, IDisposable
    {
        private readonly FileStream _lockFile;
        private bool _isOwned;

        public FileLockModInstanceProvider()
        {
            string lockPath = GetLockFilePath();

            try
            {
                // Open or create the lock file in a user-owned directory and request
                // exclusive access. FileShare.None requests exclusivity at open time;
                // we also take an advisory byte lock so the same check works on Mono
                // where opening with FileShare.None does not always use advisory locks.
                _lockFile = File.Open(lockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                _lockFile.Lock(0, 1);
                _isOwned = true;
            }
            catch (IOException)
            {
                _isOwned = false;
                _lockFile?.Dispose();
                _lockFile = null;
            }
            catch (UnauthorizedAccessException)
            {
                _isOwned = false;
                _lockFile?.Dispose();
                _lockFile = null;
            }
        }

        /// <summary>
        /// Returns the path to the per-user lock file, creating the parent
        /// directory under LocalApplicationData if it does not exist.
        /// </summary>
        private static string GetLockFilePath()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrEmpty(localAppData))
                localAppData = AppDomain.CurrentDomain.BaseDirectory;

            string lockDirectory = Path.Combine(localAppData, "DarkCloud-Enhanced");
            Directory.CreateDirectory(lockDirectory);

            return Path.Combine(lockDirectory, "instance.lock");
        }

        /// <summary>
        /// Returns <c>true</c> when this process successfully acquired the file
        /// lock, indicating it is the only running mod instance.
        /// </summary>
        public bool IsOnlyInstance() => _isOwned;

        public void Dispose()
        {
            // Closing the stream releases the file lock. This is only called when
            // the process is shutting down so the lock is not released prematurely.
            _isOwned = false;
            _lockFile?.Dispose();
        }
    }
}
