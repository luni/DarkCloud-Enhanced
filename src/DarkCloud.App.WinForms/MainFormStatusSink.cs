using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkCloud.Core.Session;

namespace DarkCloud.App.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IModStatusSink"/> that updates the
    /// status labels on <see cref="MainForm"/>.
    /// </summary>
    public sealed class MainFormStatusSink : IModStatusSink
    {
        private readonly MainForm _form;

        public MainFormStatusSink(MainForm form)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
        }

        public void ReportNoEmulators() => SetStatus("No emulator", "Waiting for PCSX2...");
        public void ReportTooManyEmulators() => SetStatus("Too many emulators", "Please close all but one PCSX2 process.");
        public void ReportGameNotActive() => SetStatus("Emulator running", "Game not active");
        public void ReportPnachNotActive() => SetStatus("PNACH disabled", "Enable the Enhanced Mod PNACH and restart.");
        public void ReportBooted() => SetStatus("Booted", "Dark Cloud detected");
        public void ReportMainMenu() => SetStatus("Main menu", "Waiting for game start...");
        public void ReportTitleScreen() => SetStatus("Title screen", "Press Start");
        public void ReportInGame(bool isNewGame) => SetStatus("In game", isNewGame ? "New game started" : "Save loaded");
        public void ReportAnotherInstanceActive() => SetStatus("Already running", "Another mod instance is active.");
        public void ReportNotEnhancedModSaveFile() => SetStatus("Invalid save", "Not an Enhanced Mod save file.");
        public void ReportSaveStateDetected() => SetStatus("Save state", "Save-state load detected.");

        public Task<bool> PromptForGameReset(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        private void SetStatus(string status, string detail)
        {
            if (_form.IsDisposed)
                return;

            var action = new Action(() =>
            {
                if (_form.IsDisposed)
                    return;
                _form.StatusLabel.Text = status;
                _form.SubStatusLabel.Text = detail;
            });

            if (_form.InvokeRequired)
            {
                try
                {
                    _form.BeginInvoke(action);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
            else
            {
                action();
            }
        }
    }
}
