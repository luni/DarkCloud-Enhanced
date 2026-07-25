using System.Threading;
using System.Threading.Tasks;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Receives user-facing status updates produced by the session state machine.
    /// Implementations are responsible for any thread marshalling required by the
    /// host (e.g., a WinForms UI thread) and for any legacy actions that must stay
    /// coupled to a specific UI interaction.
    /// </summary>
    public interface IModStatusSink
    {
        void ReportNoEmulators();
        void ReportTooManyEmulators();
        void ReportGameNotActive();
        void ReportPnachNotActive();
        void ReportBooted();
        void ReportMainMenu();
        void ReportTitleScreen();
        void ReportInGame(bool isNewGame);
        void ReportAnotherInstanceActive();
        Task<bool> PromptForGameReset(CancellationToken cancellationToken = default);
        void ReportNotEnhancedModSaveFile();
        void ReportSaveStateDetected();
    }
}
