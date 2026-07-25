using System;
using System.Threading;
using DarkCloud.Core.Session;
using DarkCloud.Memory.Abstractions;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Maps <see cref="GameSessionState"/> transitions onto the existing WinForms
    /// UI and legacy feature threads. This preserves the original
    /// <see cref="MainMenuThread"/> behavior while the session state machine now
    /// drives the lifecycle.
    /// </summary>
    internal sealed class ModWindowGameSessionObserver : IGameSessionObserver
    {
        private bool _firstLaunch = true;
        private bool _threadsStarted;

        private Thread _changesThread;
        private Thread _townThread;
        private Thread _dungeonThread;
        private Thread _weaponsThread;

        public void OnStateChanged(GameSessionState oldState, GameSessionState newState, IGameSessionContext context)
        {
            switch (newState)
            {
                case GameSessionState.NoEmulator:
                    ModWindow.EmulatorCount(0);
                    ResetForNewSession();
                    break;

                case GameSessionState.EmulatorWithoutGame:
                case GameSessionState.EmulatorExited:
                    ModWindow.EmulatorCount(1);
                    break;

                case GameSessionState.PnachDisabled:
                    ModWindow.PnachNotActive();
                    ResetForNewSession();
                    break;

                case GameSessionState.ModAlreadyOpen:
                    ModWindow.EnhancedModAlreadyOpen();
                    break;

                case GameSessionState.MainMenu:
                    MainMenuThread.userMode = true;
                    HandleMainMenuOrTitle(firstLaunchMode: false);
                    ModWindow.CurrentlyInMainMenu();
                    break;

                case GameSessionState.TitleScreen:
                    MainMenuThread.userMode = true;
                    HandleMainMenuOrTitle(firstLaunchMode: false);
                    ModWindow.CurrentlyInGame();
                    break;

                case GameSessionState.InGame:
                    MainMenuThread.userMode = true;
                    HandleInGame(oldState == GameSessionState.TitleScreen);
                    break;

                case GameSessionState.SaveStateDetected:
                    MainMenuThread.userMode = true;
                    HandleSaveState();
                    ModWindow.SaveStateDetected();
                    break;
            }
        }

        public void OnError(Exception exception, GameSessionState state)
        {
            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + $" Session error in {state}: {exception}");
        }

        public void OnShutdown()
        {
            StopGameThreads();
            Memory.WriteByte(0x21F10024, 0);
        }

        private void HandleMainMenuOrTitle(bool firstLaunchMode)
        {
            if (_firstLaunch)
            {
                ModWindow.FirstLaunchGameMode(!firstLaunchMode);
                TownCharacter.InitializeCharacterOffsetValues();
                _firstLaunch = false;
            }
        }

        private void HandleInGame(bool fromTitleScreen)
        {
            if (_firstLaunch)
            {
                ModWindow.FirstLaunchGameMode(false);
                _firstLaunch = false;
            }

            if (!fromTitleScreen)
            {
                // The original mod writes the new-game flag and shows intro text
                // when the player starts a brand new game.
                if (Memory.ReadByte(Addresses.mode) == 5)
                {
                    Memory.WriteByte(0x21CE448A, 1);
                    Dialogues.IntroTextAtNorune();
                }
            }

            StartGameThreads();
            ModWindow.CurrentlyInGame();
            CheckModWindowOptions();
        }

        private static void HandleSaveState()
        {
            try
            {
                if (Player.InDungeonFloor())
                    Memory.WriteInt(Addresses.dungeonDebugMenu, 151);
                else
                    Memory.WriteByte(Addresses.townSoftReset, 1);
            }
            catch (Exception exception)
            {
                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + " Save-state handling failed: " + exception.Message);
            }
        }

        private static void CheckModWindowOptions()
        {
            if (Memory.ReadByte(Addresses.mode) != 5)
                ModWindow.ModWindowOptionsEnabled();
        }

        private void StartGameThreads()
        {
            if (_threadsStarted)
                return;

            _threadsStarted = true;

            _changesThread = new Thread(() => MainMenuThread.ApplyNewChanges());
            _townThread = new Thread(() => TownCharacter.MainScript());
            _dungeonThread = new Thread(() => Dungeon.InsideDungeonThread());
            _weaponsThread = new Thread(() => Weapons.RerollWeaponSpecialAttributes());

            _changesThread.Start();
            _townThread.Start();
            _dungeonThread.Start();
            _weaponsThread.Start();
        }

        private void StopGameThreads()
        {
            // The legacy threads are long-running loops that terminate only when
            // the process exits. Thread.Abort is available on .NET Framework/Mono
            // and matches the original behavior when closing the mod.
            TryAbort(_changesThread);
            TryAbort(_townThread);
            TryAbort(_dungeonThread);
            TryAbort(_weaponsThread);
            _threadsStarted = false;
        }

        private static void TryAbort(Thread thread)
        {
            if (thread?.IsAlive != true)
                return;

            try
            {
                thread.Abort();
            }
            catch (ThreadStateException)
            {
            }
            catch (PlatformNotSupportedException)
            {
            }
        }

        private void ResetForNewSession()
        {
            // The legacy threads are long-running loops; stop them when the
            // emulator disappears or the PNACH is disabled so the next session
            // can start fresh.
            StopGameThreads();
        }
    }
}
