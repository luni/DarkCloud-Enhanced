using System;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Adapts the new session state machine to the legacy feature threads and
    /// WinForms UI. It starts/stops feature threads with a shared cancellation
    /// token and reports progress through <see cref="IModStatusSink"/>.
    /// </summary>
    internal sealed class ModWindowGameSessionObserver : IGameSessionObserver
    {
        private readonly IModStatusSink _sink;
        private readonly IClock _clock;
        private CancellationTokenSource _featureCts;

        private bool _bootedAndInitialized;
        private bool _sawMenuSinceLastInGame;
        private bool _waitingForGameReset;

        private Thread _changesThread;
        private Thread _townThread;
        private Thread _dungeonThread;
        private Thread _weaponsThread;

        public ModWindowGameSessionObserver(IModStatusSink sink, IClock clock)
        {
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _featureCts = new CancellationTokenSource();
        }

        public async Task OnStateChanged(GameSessionState oldState, GameSessionState newState, IGameSessionContext context)
        {
            var memory = context?.Memory;
            var cancellationToken = context?.CancellationToken ?? CancellationToken.None;

            switch (newState)
            {
                case GameSessionState.NoEmulator:
                    _sink.ReportNoEmulators();
                    ResetForNewSession();
                    break;

                case GameSessionState.EmulatorWithoutGame:
                    _sink.ReportGameNotActive();
                    ResetForNewSession();
                    break;

                case GameSessionState.EmulatorExited:
                    _sink.ReportGameNotActive();
                    break;

                case GameSessionState.PnachDisabled:
                    _sink.ReportPnachNotActive();
                    ResetForNewSession();
                    break;

                case GameSessionState.ModAlreadyOpen:
                    _sink.ReportAnotherInstanceActive();
                    break;

                case GameSessionState.MainMenu:
                    MainMenuThread.userMode = true;
                    StopFeatureThreads();
                    HandleMainMenuOrTitle();
                    _sink.ReportMainMenu();
                    break;

                case GameSessionState.TitleScreen:
                    MainMenuThread.userMode = true;
                    StopFeatureThreads();
                    HandleMainMenuOrTitle();
                    _sink.ReportTitleScreen();
                    break;

                case GameSessionState.InGame:
                    MainMenuThread.userMode = true;
                    await HandleInGameAsync(memory, cancellationToken);
                    break;

                case GameSessionState.SaveStateDetected:
                    MainMenuThread.userMode = true;
                    HandleSaveState(memory);
                    _sink.ReportSaveStateDetected();
                    break;
            }
        }

        public void OnError(Exception exception, GameSessionState state)
        {
            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + $" Session error in {state}: {exception}");
        }

        public Task OnShutdown(CancellationToken cancellationToken = default)
        {
            var cts = _featureCts;
            if (cts == null)
                return Task.CompletedTask;

            _featureCts = null;
            cts.Cancel();
            cts.Dispose();

            return Task.CompletedTask;
        }

        private void HandleMainMenuOrTitle()
        {
            if (!_bootedAndInitialized)
            {
                _sink.ReportBooted();
                TownCharacter.InitializeCharacterOffsetValues();
                _bootedAndInitialized = true;
            }

            _sawMenuSinceLastInGame = true;
            _waitingForGameReset = false;
        }

        private async Task HandleInGameAsync(IGameMemory memory, CancellationToken cancellationToken)
        {
            if (_waitingForGameReset || cancellationToken.IsCancellationRequested)
                return;

            // If the very first thing we see is in-game, the mod was launched
            // while a save was already running. Ask the user to reset.
            // Once the mod has booted from a menu, transient disconnects should
            // not re-trigger this prompt.
            if (!_sawMenuSinceLastInGame && !_bootedAndInitialized)
            {
                _waitingForGameReset = true;

                if (!cancellationToken.IsCancellationRequested && await _sink.PromptForGameReset(cancellationToken))
                    WriteGameReset(memory, Addresses.townSoftReset);

                return;
            }

            _sawMenuSinceLastInGame = false;

            try
            {
                await _clock.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            if (!TryReadByte(memory, Addresses.mode, out byte mode))
                return;

            // Re-check that the game is still in an in-game mode, mirroring the
            // original main-menu thread behavior.
            if (mode != 2 && mode != 3 && mode != 5)
                return;

            // New game (mode 5) writes the Enhanced Mod save flag, then shows
            // the custom opening text. The delays mirror the original timing
            // but now flow through IClock so the runner thread is not blocked.
            if (mode == 5)
            {
                try
                {
                    await _clock.Delay(TimeSpan.FromMilliseconds(800), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                if (!TryWriteByte(memory, 0x21CE448A, 1))
                {
                    Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + " Failed to write Enhanced Mod save flag for new game.");
                }

                try
                {
                    await _clock.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                Dialogues.IntroTextAtNorune();
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            // Loading an existing save (or a new game) requires an Enhanced Mod
            // save flag at 0x21CE448A. If it is not present, reset the game and
            // warn the user.
            if (!TryReadByte(memory, 0x21CE448A, out byte enhancedFlag) || enhancedFlag != 1)
            {
                _waitingForGameReset = true;
                WriteGameReset(memory, Addresses.mode);
                _sink.ReportNotEnhancedModSaveFile();
                return;
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            StartGameThreads();
            _sink.ReportInGame(mode == 5);
        }

        private void HandleSaveState(IGameMemory memory)
        {
            WriteGameReset(memory, Addresses.townSoftReset);
        }

        private void WriteGameReset(IGameMemory memory, long nonDungeonResetAddress)
        {
            // If the player is inside a dungeon, use the dungeon reset path so
            // the save state does not leave them stuck on an unpopulated floor.
            if (!TryReadByte(memory, (long)Addresses.checkFloor + 1, out byte currentFloor) || currentFloor == 255)
            {
                TryWriteByte(memory, nonDungeonResetAddress, 1);
            }
            else
            {
                TryWriteInt32(memory, Addresses.dungeonDebugMenu, 151);
            }
        }

        private void StartGameThreads()
        {
            var token = _featureCts.Token;

            EnsureThreadStarted(ref _changesThread, () => MainMenuThread.ApplyNewChanges(token));
            EnsureThreadStarted(ref _townThread, () => TownCharacter.MainScript(token));
            EnsureThreadStarted(ref _dungeonThread, () => Dungeon.InsideDungeonThread(token));
            EnsureThreadStarted(ref _weaponsThread, () => Weapons.RerollWeaponSpecialAttributes(token));
        }

        private static void EnsureThreadStarted(ref Thread thread, ThreadStart start)
        {
            if (thread?.IsAlive == true)
                return;

            thread = new Thread(start) { IsBackground = true };
            thread.Start();
        }

        private void StopFeatureThreads()
        {
            var previousCts = _featureCts;
            _featureCts = new CancellationTokenSource();
            previousCts.Cancel();
            previousCts.Dispose();

            _changesThread = null;
            _townThread = null;
            _dungeonThread = null;
            _weaponsThread = null;
        }

        private void ResetForNewSession()
        {
            StopFeatureThreads();

            _bootedAndInitialized = false;
            _sawMenuSinceLastInGame = false;
            _waitingForGameReset = false;
        }

        private static bool TryReadByte(IGameMemory memory, long address, out byte value)
        {
            value = 0;
            if (memory == null)
                return false;

            byte[] buffer = new byte[1];
            if (!memory.TryRead(address, buffer, 0, 1))
                return false;

            value = buffer[0];
            return true;
        }

        private static bool TryWriteByte(IGameMemory memory, long address, byte value)
        {
            if (memory == null)
                return false;

            byte[] buffer = new byte[1] { value };
            return memory.TryWrite(address, buffer, 0, 1);
        }

        private static bool TryWriteInt32(IGameMemory memory, long address, int value)
        {
            if (memory == null)
                return false;

            byte[] buffer = BitConverter.GetBytes(value);
            return memory.TryWrite(address, buffer, 0, 4);
        }
    }
}
