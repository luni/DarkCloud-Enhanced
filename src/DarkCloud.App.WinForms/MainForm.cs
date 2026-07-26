using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkCloud.Core.Logging;
using DarkCloud.Core.Session;
using DarkCloud.Memory.Windows;

namespace DarkCloud.App.WinForms
{
    public partial class MainForm : Form
    {
        private readonly TextBoxModLogger _logger;
        private readonly IModStatusSink _statusSink;
        private readonly FileLockModInstanceProvider _modInstanceProvider;
        private readonly CancellationTokenSource _runnerCts;
        private readonly Task _runnerTask;

        public Label StatusLabel { get; private set; }
        public Label SubStatusLabel { get; private set; }
        public TextBox LogTextBox { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            Text = "Dark Cloud Enhanced - Modern Host";

            _logger = new TextBoxModLogger(LogTextBox);
            _statusSink = new MainFormStatusSink(this);
            _modInstanceProvider = new FileLockModInstanceProvider();

            var memoryProvider = new ModWindowGameMemoryProvider(_logger);
            var detector = new GameSessionDetector(_modInstanceProvider);
            var clock = new SystemClock();
            var observer = new ModernHostGameSessionObserver(_statusSink, clock, _logger);
            var runner = new GameSessionRunner(
                memoryProvider,
                detector,
                observer,
                clock,
                logger: _logger);

            _runnerCts = new CancellationTokenSource();
            _runnerTask = Task.Run(() => runner.RunAsync(_runnerCts.Token));
        }

        private void InitializeComponent()
        {
            StatusLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 24,
                Text = "No emulator detected",
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new System.Windows.Forms.Padding(4)
            };

            SubStatusLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 24,
                Text = "Waiting...",
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new System.Windows.Forms.Padding(4)
            };

            LogTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new System.Drawing.Font("Consolas", 9F)
            };

            Controls.Add(LogTextBox);
            Controls.Add(SubStatusLabel);
            Controls.Add(StatusLabel);

            Size = new System.Drawing.Size(800, 480);
            MinimumSize = new System.Drawing.Size(400, 240);
            StartPosition = FormStartPosition.CenterScreen;

            FormClosing += MainForm_FormClosing;
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _runnerCts?.Cancel();
            if (_runnerTask != null)
            {
                try
                {
                    await _runnerTask;
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    _logger?.Error(exception, "Session runner shutdown error.");
                }
            }

            _modInstanceProvider?.Dispose();
        }
    }
}
