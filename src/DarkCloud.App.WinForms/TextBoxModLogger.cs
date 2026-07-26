using System;
using System.Windows.Forms;
using DarkCloud.Core.Logging;

namespace DarkCloud.App.WinForms
{
    /// <summary>
    /// Routes <see cref="IModLogger"/> messages to a WinForms <see cref="TextBox"/>.
    /// All writes are marshalled to the UI thread with <see cref="Control.BeginInvoke"/>.
    /// </summary>
    public sealed class TextBoxModLogger : IModLogger
    {
        private readonly TextBox _textBox;

        public TextBoxModLogger(TextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }

        public void Debug(string message) => Append("DEBUG", message);
        public void Information(string message) => Append("INFO", message);
        public void Warning(string message) => Append("WARN", message);
        public void Error(string message) => Append("ERROR", message);

        public void Error(Exception exception, string message)
        {
            Append("ERROR", message + Environment.NewLine + exception);
        }

        private void Append(string level, string message)
        {
            if (_textBox.IsDisposed)
                return;

            string line = $"[{DateTime.UtcNow:HH:mm:ss}] [{level}] {message}{Environment.NewLine}";

            if (_textBox.InvokeRequired)
            {
                try
                {
                    _textBox.BeginInvoke(new Action(() => AppendLine(line)));
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
                AppendLine(line);
            }
        }

        private void AppendLine(string line)
        {
            if (_textBox.IsDisposed)
                return;

            _textBox.AppendText(line);
        }
    }
}
