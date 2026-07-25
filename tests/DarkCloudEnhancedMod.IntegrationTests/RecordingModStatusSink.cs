using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkCloud.Core.Session;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    internal sealed class RecordingModStatusSink : IModStatusSink
    {
        private readonly List<StatusCall> _calls = new List<StatusCall>();

        public bool PromptForGameResetResult { get; set; }

        public List<StatusCall> Calls => _calls;

        public string LastCallName => _calls.Count > 0 ? _calls[_calls.Count - 1].Name : null;

        public void ReportNoEmulators() => Record(nameof(ReportNoEmulators));
        public void ReportTooManyEmulators() => Record(nameof(ReportTooManyEmulators));
        public void ReportGameNotActive() => Record(nameof(ReportGameNotActive));
        public void ReportPnachNotActive() => Record(nameof(ReportPnachNotActive));
        public void ReportBooted() => Record(nameof(ReportBooted));
        public void ReportMainMenu() => Record(nameof(ReportMainMenu));
        public void ReportTitleScreen() => Record(nameof(ReportTitleScreen));
        public void ReportInGame(bool isNewGame) => Record(nameof(ReportInGame), isNewGame);
        public void ReportAnotherInstanceActive() => Record(nameof(ReportAnotherInstanceActive));

        public Task<bool> PromptForGameReset(CancellationToken cancellationToken = default)
        {
            Record(nameof(PromptForGameReset));
            return Task.FromResult(PromptForGameResetResult);
        }

        public void ReportNotEnhancedModSaveFile() => Record(nameof(ReportNotEnhancedModSaveFile));
        public void ReportSaveStateDetected() => Record(nameof(ReportSaveStateDetected));

        private void Record(string name, object argument = null)
        {
            _calls.Add(new StatusCall { Name = name, Argument = argument });
        }

        public sealed class StatusCall
        {
            public string Name { get; set; }
            public object Argument { get; set; }
        }
    }
}
