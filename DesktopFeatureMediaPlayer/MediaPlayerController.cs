using Rediscovery.Feature.Desktop.MediaPlayer.Models;
using PluginFeature.Interfaces;
using SharedFeatureFunctions.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Feature.Desktop.MediaPlayer
{
    public class MediaPlayerController : IDisposable
    {
        private IPluginLogger _pluginLogger;
        private CancellationTokenSource tokenSource;

        public event EventHandler UpdateProcess;
        public ProfileConfiguration ProfileConfiguration { get; private set; }
        public bool ProcessRunning { get; private set; }
        public string CurrentTitle { get; private set; }

        public MediaPlayerController(ProfileConfiguration profileConfiguration)
        {
            ProfileConfiguration = profileConfiguration;
            if (string.IsNullOrWhiteSpace(ProfileConfiguration.ProcessName))
                throw new ArgumentNullException("ProfileConfiguration.ProcessName", "Require Process name for Media Player controller");
            if (ProfileConfiguration.CommandAvailable.Count < 1)
                throw new ArgumentNullException("ProfileConfiguration.CommandAvailable", "Require commands to be configured Media Player controller");
        }

        public void InitLogger(IPluginLogger pluginLogger)
        {
            _pluginLogger = pluginLogger;
        }

        public void InitWatcher()
        {
            try
            {
                OnSetProcessRunning();
                OnSetCurrentTitle();
                if (tokenSource != null)
                {
                    tokenSource.Cancel();
                    System.Threading.Thread.Sleep(500);
                }
                tokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = tokenSource.Token;
                Task.Run(async () =>
                {
                    do
                    {
                        OnSetProcessRunning();
                        OnSetCurrentTitle();
                        await Task.Delay(100);
                        UpdateProcess?.Invoke(this, EventArgs.Empty);
                    } while (!cancellationToken.IsCancellationRequested);
                });
            } catch (Exception ex)
            {
                _pluginLogger?.LogError(ex.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                if (tokenSource != null)
                    tokenSource.Cancel();
            }
            catch (Exception ex)
            {
                _pluginLogger?.LogError(ex.ToString());
            }
        }

        private void OnSetProcessRunning()
        {
            ProcessRunning = OnGetProcess()?.MainWindowHandle != null;
        }

        private void OnSetCurrentTitle()
        {
            var title = OnGetProcess()?.MainWindowTitle;
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (title.Contains("- VLC"))
                {
                    var index = title.LastIndexOf("- VLC");
                    title = title.Substring(0, index);
                } else if (title.StartsWith("VLC"))
                {
                    title = null;
                } else if (title.StartsWith("Plex"))
                {
                    title = null;
                }
                CurrentTitle = title;
            }
            else
                CurrentTitle = "";
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
                    _pluginLogger?.LogCritical($"MediaPlayer: Can't start Process. ApplicationPath is invalid (Path:{ProfileConfiguration.ApplicationPath})");
                }
                return false;
            }
            return true;
        }

        public void ExecuteCommand(CommandConfiguration.CommandTypes commandType)
        {
            if (commandType == CommandConfiguration.CommandTypes.None)
                return;
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
                    _pluginLogger?.LogCritical("Can't load Command KeyCode");
                }
            } else
            {
                _pluginLogger?.LogCritical("Can't execute Command (Command not available in the Profile)");
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
