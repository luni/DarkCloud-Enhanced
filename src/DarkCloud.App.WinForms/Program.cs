using System;
using System.Windows.Forms;

namespace DarkCloud.App.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var form = new MainForm())
            {
                Application.Run(form);
            }
        }
    }
}
