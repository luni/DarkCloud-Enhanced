using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Configuration;
using DarkCloud.Core.Features;
using DarkCloud.Core.Logging;
using DarkCloud.Core.Players;
using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;
using DarkCloudEnhancedMod;

namespace DarkCloud.Memory.Windows
{
    /// <summary>
    /// Adapts the session state machine to the modern host. It reports progress
    /// through <see cref="IModStatusSink"/> and runs the shared feature set once
    /// the player is in-game.
    /// </summary>
    public sealed class ModernHostGameSessionObserver : IGameSessionObserver
    {
        private readonly IModStatusSink _sink;
        private readonly IClock _clock;
        private readonly IModLogger _logger;
        private readonly ModConfiguration _configuration;
        private CancellationTokenSource _featureCts;

        private bool _bootedAndInitialized;
        private bool _sawMenuSinceLastInGame;
        private bool _waitingForGameReset;

        private ModFeatureRunner _featureRunner;
        private Task _featureRunnerTask;
        private PlayerPresenceService _playerPresence;

        public ModernHostGameSessionObserver(IModStatusSink sink, IClock clock, IModLogger logger = null, ModConfiguration configuration = null)
        {
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _logger = logger ?? NullModLogger.Instance;
            _configuration = configuration ?? ModConfigurationDefaults.Create();
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
                    await ResetForNewSession();
                    break;

                case GameSessionState.EmulatorWithoutGame:
                    _sink.ReportGameNotActive();
                    await ResetForNewSession();
                    break;

                case GameSessionState.EmulatorExited:
                    _sink.ReportGameNotActive();
                    await StopFeatureRunner();
                    break;

                case GameSessionState.PnachDisabled:
                    _sink.ReportPnachNotActive();
                    await ResetForNewSession();
                    break;

                case GameSessionState.ModAlreadyOpen:
                    _sink.ReportAnotherInstanceActive();
                    break;

                case GameSessionState.MainMenu:
                    MainMenuThread.userMode = true;
                    await StopFeatureRunner();
                    HandleMainMenuOrTitle();
                    _sink.ReportMainMenu();
                    break;

                case GameSessionState.TitleScreen:
                    MainMenuThread.userMode = true;
                    await StopFeatureRunner();
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
            _logger.Error(exception, $"Session error in {state}.");
        }

        public async Task OnShutdown(CancellationToken cancellationToken = default)
        {
            await ShutdownFeatureRunnerAsync(null).ConfigureAwait(false);
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

            if (!_sawMenuSinceLastInGame && !_bootedAndInitialized)
            {
                _waitingForGameReset = true;
                _logger.Warning("Mod launched while a save was already running; please reset to the main menu.");
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

            if (mode != 2 && mode != 3 && mode != 5)
                return;

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
                    _logger.Warning("Failed to write Enhanced Mod save flag for new game.");
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

            if (!TryReadByte(memory, 0x21CE448A, out byte enhancedFlag) || enhancedFlag != 1)
            {
                _waitingForGameReset = true;
                WriteGameReset(memory, Addresses.mode);
                _sink.ReportNotEnhancedModSaveFile();
                return;
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            _playerPresence = new PlayerPresenceService(memory, new PlayerPresenceMemoryLayout());

            StartFeatureRunner(memory);
            _sink.ReportInGame(mode == 5);
        }

        private void HandleSaveState(IGameMemory memory)
        {
            WriteGameReset(memory, Addresses.townSoftReset);
        }

        private void WriteGameReset(IGameMemory memory, long nonDungeonResetAddress)
        {
            if (!TryReadByte(memory, (long)Addresses.checkFloor + 1, out byte currentFloor) || currentFloor == 255)
            {
                TryWriteByte(memory, nonDungeonResetAddress, 1);
            }
            else
            {
                TryWriteInt32(memory, Addresses.dungeonDebugMenu, 151);
            }
        }

        private void StartFeatureRunner(IGameMemory memory)
        {
            var token = _featureCts.Token;

            if (_featureRunner != null)
                return;

            _featureRunner = new ModFeatureRunner(
                new List<ModFeature>
                {
                    new ModFeature(
                        new ApplyChangesFeature(new ApplyChangesService()),
                        new ModFeatureDescriptor("apply-changes", "Apply Changes", IsFeatureEnabled("apply-changes"))),
                    new ModFeature(
                        new TownCharacterFeature(),
                        new ModFeatureDescriptor("town-character", "Town Character", IsFeatureEnabled("town-character"))),
                    new ModFeature(
                        new DungeonFeature(),
                        new ModFeatureDescriptor("dungeon", "Dungeon", IsFeatureEnabled("dungeon"))),
                    new ModFeature(
                        new WeaponsFeature(),
                        new ModFeatureDescriptor("weapons-reroll", "Weapon Reroll", IsFeatureEnabled("weapons-reroll"))),
                    new ModFeature(
                        new StatusLogFeature(_logger),
                        new ModFeatureDescriptor("status-log", "Status Log", IsFeatureEnabled("status-log"))),
                },
                _clock,
                new ModLoggerExceptionHandler(_logger),
                _logger);

            _featureRunnerTask = Task.Run(() => _featureRunner.RunAsync(
                new GameFeatureContext(memory),
                CreateGameSnapshot,
                _configuration.PollInterval,
                token), token);
        }

        private bool IsFeatureEnabled(string featureId)
        {
            if (_configuration.Features.TryGetValue(featureId, out bool enabled))
                return enabled;

            return true;
        }

        private async Task ShutdownFeatureRunnerAsync(CancellationTokenSource nextFeatureCts)
        {
            var previousCts = _featureCts;
            var previousTask = _featureRunnerTask;

            if (previousCts != null)
                previousCts.Cancel();

            if (previousTask != null)
            {
                try
                {
                    await previousTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    _logger.Error(exception, "Feature runner shutdown error.");
                }
            }

            _featureRunner = null;
            _featureRunnerTask = null;
            _playerPresence = null;

            _featureCts = nextFeatureCts;

            if (previousCts != null)
                previousCts.Dispose();
        }

        private async Task StopFeatureRunner()
        {
            await ShutdownFeatureRunnerAsync(new CancellationTokenSource()).ConfigureAwait(false);
        }

        private async Task ResetForNewSession()
        {
            await StopFeatureRunner();

            _bootedAndInitialized = false;
            _sawMenuSinceLastInGame = false;
            _waitingForGameReset = false;
        }

        private GameSnapshot CreateGameSnapshot()
        {
            var presence = _playerPresence;
            if (presence == null)
                return new GameSnapshot(GameSessionState.InGame, CharacterType.Unknown, false);

            return new GameSnapshot(
                GameSessionState.InGame,
                presence.GetCurrentCharacter(),
                presence.IsInDungeonFloor());
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
