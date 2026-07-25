using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace DarkCloudEnhancedMod.WindowsSmoke
{
    /// <summary>
    /// Synthetic Windows process that mirrors the Linux fake_pcsx2 target.
    /// It allocates a 32 MB buffer, writes known boot/region markers, reports
    /// its PID and buffer address, and waits until it is terminated.
    /// </summary>
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            int size = 32 * 1024 * 1024;
            IntPtr buffer = Marshal.AllocHGlobal(size);

            try
            {
                byte[] marker = Encoding.ASCII.GetBytes("DarkClou");
                Marshal.Copy(marker, 0, buffer, marker.Length);

                byte[] boot = Encoding.ASCII.GetBytes("Dark");
                Marshal.Copy(boot, 0, IntPtr.Add(buffer, 0x299540), boot.Length);

                Marshal.WriteByte(IntPtr.Add(buffer, 0x1F22EA0), 1);

                Console.WriteLine($"FAKE_PCSX2 pid={Process.GetCurrentProcess().Id} EEmem=0x{buffer.ToInt64():X}");
                Console.Out.Flush();

                while (true)
                    Thread.Sleep(1000);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
