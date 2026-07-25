namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Represents the high-level lifecycle state of a Dark Cloud play session.
    /// </summary>
    public enum GameSessionState
    {
        /// <summary>
        /// Initial state before any detection has occurred.
        /// </summary>
        None,

        /// <summary>
        /// No supported emulator process is available.
        /// </summary>
        NoEmulator,

        /// <summary>
        /// An emulator is running but the game has not booted.
        /// </summary>
        EmulatorWithoutGame,

        /// <summary>
        /// The game has booted but the player is in the main menu / title screen.
        /// </summary>
        MainMenu,

        /// <summary>
        /// The game is in the opening-book transition after pressing start.
        /// </summary>
        TitleScreen,

        /// <summary>
        /// The player is currently in-game (town, dungeon, or intro).
        /// </summary>
        InGame,

        /// <summary>
        /// A save-state load was detected from a discontinuous frame counter.
        /// </summary>
        SaveStateDetected,

        /// <summary>
        /// The emulator or game process is no longer reachable.
        /// </summary>
        EmulatorExited,

        /// <summary>
        /// The PNACH cheat flag is not active; the mod cannot function.
        /// </summary>
        PnachDisabled,

        /// <summary>
        /// Another instance of the mod already owns the mutual-exclusion flag.
        /// </summary>
        ModAlreadyOpen
    }
}
