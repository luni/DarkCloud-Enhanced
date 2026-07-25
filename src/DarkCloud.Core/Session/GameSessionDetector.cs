using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Detects the current <see cref="GameSessionState"/> by reading known game
    /// memory values. The detector is stateful only to track the previous frame
    /// counter for save-state detection and to know whether it has claimed the
    /// mod's mutual-exclusion flag.
    /// </summary>
    public sealed class GameSessionDetector : IGameSessionDetector
    {
        // Dark Cloud boot marker "Dark" in little-endian.
        private const int BootMarker = 0x6B726144;

        // Known PS2 memory addresses (NTSC base). IGameMemory implementations
        // are responsible for translating these when the active region differs.
        private const long BootMarkerAddress = 0x20299540L;
        private const long PnachFlagAddress = 0x21F10020L;
        private const long ModFlagAddress = 0x21F10024L;
        private const long FrameCounterAddress = 0x202A2400L;
        private const long ModeAddress = 0x202A2534L;
        private const long OpeningBookAddress = 0x202A3420L;

        // Modes observed by the original mod.
        private const byte TownMode = 0;
        private const byte TitleOrIntroMode = 1;
        private const byte DungeonMode = 2;
        private const byte GeoramaMode = 3;
        private const byte OpeningCutsceneMode = 5;

        // Opening book value that appears after pressing start on the title screen.
        private const byte OpeningBookValue = 9;

        // Frame-counter delta that indicates a save-state load.
        private const int SaveStateFrameDelta = 360;

        private bool _hasClaimedModFlag;
        private int _previousFrameCounter;
        private bool _hasPreviousFrameCounter;

        public GameSessionState Detect(IGameMemory memory, GameSessionState previousState)
        {
            if (memory == null)
            {
                ResetAllState();
                return GameSessionState.NoEmulator;
            }

            if (!TryReadInt32(memory, BootMarkerAddress, out int bootMarker))
            {
                ResetFrameTracking();
                return previousState == GameSessionState.NoEmulator
                    ? GameSessionState.NoEmulator
                    : GameSessionState.EmulatorExited;
            }

            if (bootMarker != BootMarker)
            {
                ResetFrameTracking();
                return GameSessionState.EmulatorWithoutGame;
            }

            if (!TryReadByte(memory, PnachFlagAddress, out byte pnachFlag) || pnachFlag != 1)
            {
                ResetFrameTracking();
                return GameSessionState.PnachDisabled;
            }

            if (!TryReadByte(memory, ModFlagAddress, out byte modFlag))
            {
                ResetFrameTracking();
                return GameSessionState.EmulatorExited;
            }

            if (modFlag == 1 && !_hasClaimedModFlag)
            {
                // Another instance owns the flag; wait until it is released.
                return GameSessionState.ModAlreadyOpen;
            }

            if (modFlag != 1)
            {
                // Claim the flag so another instance cannot start.
                if (!TryWriteByte(memory, ModFlagAddress, 1))
                {
                    ResetFrameTracking();
                    return GameSessionState.EmulatorExited;
                }

                _hasClaimedModFlag = true;
            }

            // Save-state detection depends on a stable frame counter.
            bool frameRead = TryReadInt32(memory, FrameCounterAddress, out int frameCounter);
            if (frameRead)
            {
                if (_hasPreviousFrameCounter &&
                    (frameCounter < _previousFrameCounter ||
                     frameCounter > _previousFrameCounter + SaveStateFrameDelta ||
                     frameCounter == 0))
                {
                    _previousFrameCounter = frameCounter;
                    return GameSessionState.SaveStateDetected;
                }

                _previousFrameCounter = frameCounter;
                _hasPreviousFrameCounter = true;
            }

            if (!TryReadByte(memory, ModeAddress, out byte mode))
            {
                ResetFrameTracking();
                return GameSessionState.EmulatorExited;
            }

            if (mode == DungeonMode || mode == GeoramaMode || mode == OpeningCutsceneMode)
                return GameSessionState.InGame;

            if (mode == TownMode || mode == TitleOrIntroMode)
            {
                if (!TryReadByte(memory, OpeningBookAddress, out byte openingBook))
                {
                    ResetFrameTracking();
                    return GameSessionState.EmulatorExited;
                }

                if (openingBook == OpeningBookValue)
                    return GameSessionState.TitleScreen;

                return GameSessionState.MainMenu;
            }

            // Unknown mode; stay in the current menu state.
            return previousState == GameSessionState.InGame
                ? GameSessionState.InGame
                : GameSessionState.MainMenu;
        }

        /// <summary>
        /// Releases the mod's mutual-exclusion flag so another instance can start.
        /// </summary>
        public static bool ReleaseModFlag(IGameMemory memory)
        {
            if (memory == null)
                return false;

            return TryWriteByte(memory, ModFlagAddress, 0);
        }

        private void ResetFrameTracking()
        {
            _hasPreviousFrameCounter = false;
            _previousFrameCounter = 0;
        }

        private void ResetAllState()
        {
            _hasClaimedModFlag = false;
            ResetFrameTracking();
        }

        private static bool TryReadByte(IGameMemory memory, long address, out byte value)
        {
            value = 0;
            byte[] buffer = new byte[1];
            if (!memory.TryRead(address, buffer, 0, 1))
                return false;

            value = buffer[0];
            return true;
        }

        private static bool TryReadInt32(IGameMemory memory, long address, out int value)
        {
            value = 0;
            byte[] buffer = new byte[4];
            if (!memory.TryRead(address, buffer, 0, 4))
                return false;

            value = buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);
            return true;
        }

        private static bool TryWriteByte(IGameMemory memory, long address, byte value)
        {
            byte[] buffer = new byte[1] { value };
            return memory.TryWrite(address, buffer, 0, 1);
        }
    }
}
