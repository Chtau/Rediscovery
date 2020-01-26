using SharedCoreModels.FeatureModels.MediaPlayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static SharedCoreModels.FeatureModels.KeyCodes;
using static SharedCoreModels.FeatureModels.MediaPlayer.CommandConfiguration;

namespace DesktopFeatureMediaPlayer
{
    public class MediaPlayerController
    {
        public ProfileConfiguration ProfileConfiguration { get; private set; }
        public bool ProcessRunning { get; private set; }

        public MediaPlayerController(ProfileConfiguration profileConfiguration)
        {
            ProfileConfiguration = profileConfiguration;
            if (string.IsNullOrWhiteSpace(ProfileConfiguration.ProcessName))
                throw new ArgumentNullException("ProfileConfiguration.ProcessName", "Require Process name for Media Player controller");
            if (ProfileConfiguration.CommandAvailable.Count < 1)
                throw new ArgumentNullException("ProfileConfiguration.CommandAvailable", "Require commands to be configured Media Player controller");
        }

        public void InitWatcher()
        {

        }

        public void ExecuteCommand(CommandTypes commandType)
        {
            if (ProfileConfiguration.CommandAvailable.Contains(commandType))
            {

            } else
            {
                System.Diagnostics.Debug.Fail("Can't execute Command (Command not available in the Profile)");
            }
        }

        private Process OnGetProcess()
        {
            return Process.GetProcesses().FirstOrDefault(p => p.ProcessName == ProfileConfiguration.ProcessName);
        }

        private bool OnSendKeystroke(KeyCode keyCode,
            bool altKeyPressed = false, bool controlKeyPressed = false, bool shiftKeyPressed = false)
        {
            return OnSendKeystroke(new KeyCode[] { keyCode }, altKeyPressed, controlKeyPressed, shiftKeyPressed);
        }

        private bool OnSendKeystroke(KeyCode[] keyCodes,
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
