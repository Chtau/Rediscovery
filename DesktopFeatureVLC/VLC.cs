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
            return OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode.DOWN, false, true);
        }

        public bool VolumneUp()
        {
            return OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode.UP, false, true);
        }

        public bool Mute()
        {
            return OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode.KEY_M);
        }

        public bool Stop()
        {
            return OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode.KEY_S);
        }

        public bool Previous()
        {
            return OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode.KEY_P);
        }

        public bool Next()
        {
            return OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode.KEY_N);
        }

        public bool Fullscreen()
        {
            return OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode.KEY_F);
        }

        public bool FullscreenExit()
        {
            return OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode.ESC);
        }

        public bool PlayPause()
        {
            return OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode.SPACE_BAR);
        }

        private Process OnGetProcess()
        {
            return Process.GetProcesses().FirstOrDefault(p => p.ProcessName == ProcessName);
        }

        private bool OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode keyCode,
            bool altKeyPressed = false, bool controlKeyPressed = false, bool shiftKeyPressed = false)
        {
            return OnSendKeystroke(new SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode[] { keyCode }, altKeyPressed, controlKeyPressed, shiftKeyPressed);
        }

        private bool OnSendKeystroke(SharedFeatureFunctions.RemoteProcessKeyCodes.KeyCode[] keyCodes,
            bool altKeyPressed = false, bool controlKeyPressed = false, bool shiftKeyPressed = false)
        {
            var prc = OnGetProcess();
            if (prc != null)
            {
                SharedFeatureFunctions.RemoteProcessControl.SendKeys(prc.MainWindowHandle, keyCodes, altKeyPressed, controlKeyPressed, shiftKeyPressed);

                return true;
            }
            return false;
        }
    }
}
