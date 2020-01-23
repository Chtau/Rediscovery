using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace DesktopFeatureVLC
{
    public class VLC
    {

        public void Start()
        {
            var processes = Process.GetProcesses();
            Process myProcess = processes.FirstOrDefault(p => p.ProcessName == "vlc");
            SharedFeatureFunctions.WindowKeyHook.SendKeystroke(myProcess.MainWindowHandle, SharedFeatureFunctions.WindowKeyHook.Keys.Space);
        }
    }
}
