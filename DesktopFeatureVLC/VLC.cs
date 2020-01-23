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
        private readonly string ProcessName = "vlc";

        public bool IsRunning
        {
            get
            {
                return OnGetProcess() != null ? true : false;
            }
        }

        public bool VolumneDown()
        {
            return OnSendKeystrokeMultiDown(SharedFeatureFunctions.WindowKeyHook.Keys.Control, SharedFeatureFunctions.WindowKeyHook.Keys.Down);
        }

        public bool VolumneUp()
        {
            return OnSendKeystrokeMultiDown(SharedFeatureFunctions.WindowKeyHook.Keys.Control, SharedFeatureFunctions.WindowKeyHook.Keys.Up);
        }

        public bool Mute()
        {
            return OnSendKeystroke(SharedFeatureFunctions.WindowKeyHook.Keys.M);
        }

        public bool Stop()
        {
            return OnSendKeystroke(SharedFeatureFunctions.WindowKeyHook.Keys.S);
        }

        public bool Previous()
        {
            return OnSendKeystroke(SharedFeatureFunctions.WindowKeyHook.Keys.P);
        }

        public bool Next()
        {
            return OnSendKeystroke(SharedFeatureFunctions.WindowKeyHook.Keys.N);
        }

        public bool Fullscreen()
        {
            return OnSendKeystroke(SharedFeatureFunctions.WindowKeyHook.Keys.F);
        }

        public bool FullscreenExit()
        {
            return OnSendKeystroke(SharedFeatureFunctions.WindowKeyHook.Keys.Escape);
        }

        public bool PlayPause()
        {
            return OnSendKeystroke(SharedFeatureFunctions.WindowKeyHook.Keys.Space);
        }

        private Process OnGetProcess()
        {
            return Process.GetProcesses().FirstOrDefault(p => p.ProcessName == ProcessName);
        }

        private bool OnSendKeystroke(SharedFeatureFunctions.WindowKeyHook.Keys key)
        {
            var prc = OnGetProcess();
            if (prc != null)
            {
                SharedFeatureFunctions.WindowKeyHook.SendKeystroke(prc.MainWindowHandle, key);
                SharedFeatureFunctions.WindowKeyHook.SendKeystroke(prc.MainWindowHandle, key, true);

                return true;
            }
            return false;
        }

        private bool OnSendKeystrokeMultiDown(params SharedFeatureFunctions.WindowKeyHook.Keys[] keys)
        {
            var prc = OnGetProcess();
            if (prc != null)
            {
                SharedFeatureFunctions.WindowKeyHook.SendKeystroke(prc.MainWindowHandle, keys[0], false, false, true);
                SharedFeatureFunctions.WindowKeyHook.SendKeystroke(prc.MainWindowHandle, keys[1], false, false, true);
                /*foreach (var key in keys)
                    SharedFeatureFunctions.WindowKeyHook.SendKeystroke(prc.MainWindowHandle, key);*/
                /*foreach (var key in keys)
                    SharedFeatureFunctions.WindowKeyHook.SendKeystroke(prc.MainWindowHandle, key, true);*/

                return true;
            }
            return false;
        }
    }
}
