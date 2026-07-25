using DarkCloud.Core.Session;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// WinForms implementation of <see cref="IModStatusSink"/> that delegates to
    /// the existing <see cref="ModWindow"/> static helpers.
    /// </summary>
    internal sealed class WinFormsModStatusSink : IModStatusSink
    {
        public void ReportNoEmulators() => ModWindow.EmulatorCount(0);

        public void ReportTooManyEmulators() => ModWindow.EmulatorCount(2);

        public void ReportGameNotActive() => ModWindow.EmulatorCount(1);

        public void ReportPnachNotActive() => ModWindow.PnachNotActive();

        public void ReportBooted() => ModWindow.FirstLaunchGameMode(true);

        public void ReportMainMenu() => ModWindow.CurrentlyInMainMenu();

        public void ReportTitleScreen() => ModWindow.CurrentlyInGame();

        public void ReportInGame(bool isNewGame)
        {
            ModWindow.CurrentlyInGame();

            // The original mod only applied window options once the player was
            // past the new-game intro (mode 5).
            if (!isNewGame)
                ModWindow.ModWindowOptionsEnabled();
        }

        public void ReportAnotherInstanceActive() => ModWindow.EnhancedModAlreadyOpen();

        public bool PromptForGameReset() => ModWindow.PromptForGameReset();

        public void ReportNotEnhancedModSaveFile() => ModWindow.NotEnhancedModSaveFile();

        public void ReportSaveStateDetected() => ModWindow.SaveStateDetected();
    }
}
