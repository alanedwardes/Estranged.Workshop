using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Estranged.Workshop
{
    internal sealed class BrowserOpener
    {
        public void OpenBrowser(string url)
        {
            Console.WriteLine($"See: {url}");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
    }
}
