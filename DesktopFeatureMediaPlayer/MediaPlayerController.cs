using SharedCoreModels.FeatureModels.MediaPlayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SharedCoreModels.FeatureModels.KeyCodes;
using static SharedCoreModels.FeatureModels.MediaPlayer.CommandConfiguration;

namespace DesktopFeatureMediaPlayer
{
    public class MediaPlayerController : IDisposable
    {
        private CancellationTokenSource tokenSource;

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
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                System.Threading.Thread.Sleep(500);
            }
            tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;
            Task.Run(() =>
            {
                do
                {
                    Task.Delay(100);
                    var prc = OnGetProcess();
                    ProcessRunning = prc?.MainWindowHandle != null;
                } while (!cancellationToken.IsCancellationRequested);
            });
        }

        public bool StartProcess()
        {
            if (!ProcessRunning)
            {
                if (System.IO.File.Exists(ProfileConfiguration.ApplicationPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = ProfileConfiguration.ApplicationPath
                    });
                    System.Threading.Thread.Sleep(500);
                    return OnGetProcess()?.MainWindowHandle != null;
                } else
                {
                    Debug.Fail($"MediaPlayer: Can't start Process. ApplicationPath is invalid (Path:{ProfileConfiguration.ApplicationPath})");
                }
                return false;
            }
            return true;
        }

        public void ExecuteCommand(CommandTypes commandType)
        {
            if (ProfileConfiguration.CommandAvailable.Contains(commandType))
            {
                if (ProfileConfiguration.CommandConfiguration.CommandKeys.TryGetValue(commandType, out KeyCode[] keys))
                {
                    var altKey = keys.Contains(KeyCode.ALT);
                    var shiftKey = keys.Contains(KeyCode.SHIFT) | keys.Contains(KeyCode.LSHIFT) | keys.Contains(KeyCode.RSHIFT);
                    var ctrlKey = keys.Contains(KeyCode.CONTROL) | keys.Contains(KeyCode.LCONTROL) | keys.Contains(KeyCode.RCONTROL);
                    var useKeys = from x in keys
                                  where x != KeyCode.ALT
                                  && x != KeyCode.SHIFT && x != KeyCode.LSHIFT && x != KeyCode.RSHIFT
                                  && x != KeyCode.CONTROL && x != KeyCode.LCONTROL && x != KeyCode.RCONTROL
                                  select x;
                    OnSendKeystroke(useKeys?.ToArray(), altKey, ctrlKey, shiftKey);
                } else
                {
                    System.Diagnostics.Debug.Fail("Can't load Command KeyCode");
                }
            } else
            {
                System.Diagnostics.Debug.Fail("Can't execute Command (Command not available in the Profile)");
            }
        }

        private Process OnGetProcess()
        {
            if (!string.IsNullOrWhiteSpace(ProfileConfiguration.ProcessName))
                return Process.GetProcesses().FirstOrDefault(p => p.ProcessName == ProfileConfiguration.ProcessName);
            return null;
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

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (tokenSource != null)
                        tokenSource.Cancel();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
