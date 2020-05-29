using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PluginFeature.Models;
using Rediscovery.Desktop.Hub.Feature.InternalIPCModels;
using Rediscovery.Desktop.Hub.Logging;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature
{
    public class CommunicationController : Shared.BaseController
    {
        private static string lastLoggerMessage = null;

        private readonly ILogger<CommunicationController> _logger;
        private readonly CommunicationResourceConsumer.IHub _hub;
        private readonly SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration _remoteResourceSettings;

        private CommunicationBase.ConnectionConfiguration connectionConfiguration;

        public CommunicationController(ILogger<CommunicationController> logger,
            CommunicationResourceConsumer.IHub hub,
            IOptions<SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration> remoteResourceSettings
            )
        {
            _remoteResourceSettings = remoteResourceSettings.Value;
            _logger = logger;
            _hub = hub;
            connectionConfiguration = new CommunicationBase.ConnectionConfiguration
            {
                Address = _remoteResourceSettings.IP + (_remoteResourceSettings.Port != null ? ":" + _remoteResourceSettings.Port : ""),
                DisplayName = _remoteResourceSettings.DesktopHubApplicationKey,
                Id = Guid.NewGuid(),
                State = CommunicationBase.ConnectionState.None,
                Token = null
            };
            _hub.Init(_logger.ToSharedLogger(), "/remote/resource/hub");
            _hub.ActiveDeviceInfoReceived += _deviceService_ActiveDeviceInfoReceived;
            _hub.DeviceInfoReceived += _deviceService_DeviceInfoReceived;
            _hub.LogEntryReceived += _loggerService_LoggerDataReceived;
            _hub.ServiceFeatureReceived += _featureService_DeviceFeatureReceived;
            _hub.PendingAuthenticationDeviceReceived += _hub_PendingAuthenticationDeviceReceived;
            _hub.ConnectionStateChanged += _hub_ConnectionStateChanged;
            _hub.FeatureProfileUIReceived += _hub_FeatureProfileUIReceived;
            _hub.FeatureSettingUIReceived += _hub_FeatureSettingUIReceived;
            _hub.FeatureProfilesReceived += _hub_FeatureProfilesReceived;
            _hub.FeatureSettingsReceived += _hub_FeatureSettingsReceived;

            ElectronNET.API.Electron.IpcMain.On("resolvependingdevice-ipc", (args) =>
            {
                try
                {
                    var param = Newtonsoft.Json.JsonConvert.DeserializeObject<PendingAuthenticationResolve>(args?.ToString());
                    string msg = $"Resolve pending device authentication for Id:{param.Id} Accept:{param.Accept}";
                    _logger.LogDebug(msg);
                    OnRemoteLogEntry(msg, SharedBase.Logging.LoggerEntry.LoggerType.Debug);
                    _hub.RequestResolvePendingAuthenticationDevice(param.Id, param.Accept);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in IPC listener from UI [resolvependingdevice-ipc]");
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
            ElectronNET.API.Electron.IpcMain.On("deletedeviceinfo-ipc", (args) =>
            {
                try
                {
                    var param = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceInfo>(args?.ToString());
                    string msg = $"Delete device for Id:{param.Id}";
                    _logger.LogDebug(msg);
                    OnRemoteLogEntry(msg, SharedBase.Logging.LoggerEntry.LoggerType.Debug);
                    _hub.RequestDeleteDevice(param);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in IPC listener from UI [deletedeviceinfo-ipc]");
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
            ElectronNET.API.Electron.IpcMain.On("updatedeviceinfo-ipc", (args) =>
            {
                try
                {
                    var param = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceInfo>(args?.ToString());
                    string msg = $"Update device authentication for Id:{param.Id}";
                    _logger.LogDebug(msg);
                    OnRemoteLogEntry(msg, SharedBase.Logging.LoggerEntry.LoggerType.Debug);
                    _hub.RequestUpdateDevice(param);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in IPC listener from UI [updatedeviceinfo-ipc]");
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
            ElectronNET.API.Electron.IpcMain.On("open-directory", async (args) => {
                try
                {
                    string dir = args?.ToString();
                    if (!string.IsNullOrWhiteSpace(dir))
                    {
                        if (!dir.EndsWith(System.IO.Path.DirectorySeparatorChar))
                        {
                            dir += System.IO.Path.DirectorySeparatorChar;
                        }
                        if (System.IO.Directory.Exists(dir))
                        {
                            await ElectronNET.API.Electron.Shell.ShowItemInFolderAsync(dir);
                        } else
                        {
                            OnRemoteLogEntry($"UI tried to open Directory (\"{dir}\") but it don't exist.", SharedBase.Logging.LoggerEntry.LoggerType.Debug);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
            ElectronNET.API.Electron.IpcMain.On("request-features-detail-ipc", (args) => {
                try
                {
                    Guid featureId = new Guid(args.ToString());
                    _hub.RequestFeatureDetails(featureId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
            ElectronNET.API.Electron.IpcMain.On("request-features-detail-ui-ipc", (args) => {
                try
                {
                    Guid featureId = new Guid(args.ToString());
                    _hub.RequestFeatureDetailsUI(featureId);
                } catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
            ElectronNET.API.Electron.IpcMain.On("request-features-save-profile-ipc", (args) => {
                try
                {
                    var param = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCoreModels.EntityContent<Guid, DeviceFeatureProfil>>(args?.ToString());
                    _hub.RequestFeatureSaveProfile(param);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
            ElectronNET.API.Electron.IpcMain.On("request-features-delete-profile-ipc", (args) => {
                try
                {
                    var param = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCoreModels.EntityContent<Guid, DeviceFeatureProfil>>(args?.ToString());
                    _hub.RequestFeatureDeleteProfile(param);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
            ElectronNET.API.Electron.IpcMain.On("request-features-save-setting-ipc", (args) => {
                try
                {
                    var param = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCoreModels.EntityContent<Guid, DeviceFeatureSetting>>(args?.ToString());
                    _hub.RequestFeatureSaveSetting(param);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
            ElectronNET.API.Electron.IpcMain.On("request-all-features-ipc", (args) => {
                try
                {
                    _hub.RequestAllFeatures();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                }
            });
        }

        private void _hub_FeatureSettingsReceived(object sender, EntityContent<Guid, PluginFeature.Models.DeviceFeatureSetting> e)
        {
            try
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "features-settings-ipc", e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FeatureSettingsReceived via IPC from underlying connection");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private void _hub_FeatureProfilesReceived(object sender, EntityContent<Guid, List<PluginFeature.Models.DeviceFeatureProfil>> e)
        {
            try
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "features-profiles-ipc", e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FeatureProfilesReceived via IPC from underlying connection");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private void _hub_FeatureSettingUIReceived(object sender, EntityContent<Guid, byte[]> e)
        {
            try
            {
                // received byte[] is the zip file 'profileui.zip'
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "features-setting-ui-ipc", new EntityContent<Guid, string>(e.Id, OnCreateHtmlContentFromByteArrayZipArchive(e.Content)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FeatureSettingUIReceived via IPC from underlying connection");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private void _hub_FeatureProfileUIReceived(object sender, EntityContent<Guid, byte[]> e)
        {
            try
            {
                // received byte[] is the zip file 'settingui.zip'
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "features-profile-ui-ipc", new EntityContent<Guid, string>(e.Id, OnCreateHtmlContentFromByteArrayZipArchive(e.Content)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FeatureProfileUIReceived via IPC from underlying connection");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private string OnCreateHtmlContentFromByteArrayZipArchive(byte[] buffer)
        {
            string fallback = "<h3>No UI found</h3>";
            try
            {
                // TODO: handle multiple files with directory structure
                string htmlContent = null;
                if (buffer != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Seek(0, SeekOrigin.Begin);
                        using (var zipArchive = new ZipArchive(stream))
                        {
                            if (zipArchive != null)
                            {
                                foreach (var entry in zipArchive.Entries)
                                {
                                    var entryStream = entry.Open();
                                    using (StreamReader streamReader = new StreamReader(entryStream))
                                    {
                                        htmlContent += streamReader.ReadToEnd();
                                    }
                                }
                            }
                        }
                    }
                }
                return htmlContent ?? fallback;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnCreateHtmlContentFromByteArrayZipArchive");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
                return fallback;
            }
        }

        private void _hub_ConnectionStateChanged(object sender, bool e)
        {
            try
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "hubconnectionchanged-ipc", e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConnectionStateChanged via IPC from underlying connection");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private void _hub_PendingAuthenticationDeviceReceived(object sender, List<DeviceInfo> e)
        {
            try
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "pendingdevice-ipc", e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PendingAuthenticationDeviceReceived via IPC from Service");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private void _deviceService_ActiveDeviceInfoReceived(object sender, List<DeviceInfo> e)
        {
            try
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "activedeviceinfo-ipc", e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActiveDeviceInfoReceived via IPC from Service");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private void _featureService_DeviceFeatureReceived(object sender, List<DeviceFeature> e)
        {
            try
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "features-ipc", e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeviceFeatureReceived via IPC from Service");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private void _loggerService_LoggerDataReceived(object sender, SharedBase.Logging.LoggerEntry e)
        {
            try
            {
                var msg = e.Message.ToLower();
                if (lastLoggerMessage != msg)
                {
                    Console.WriteLine($"{DateTime.Now}: Logger Message received => {e.Message}");
                    var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                    ElectronNET.API.Electron.IpcMain.Send(mainWindow, "loggermessage-ipc", e);
                }
                lastLoggerMessage = msg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoggerDataReceived via IPC from Service");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private void _deviceService_DeviceInfoReceived(object sender, List<DeviceInfo> e)
        {
            try
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "registereddeviceinfo-ipc", e);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "DeviceInfoReceived via IPC from Service");
                OnRemoteLogEntry(ex.ToString(), SharedBase.Logging.LoggerEntry.LoggerType.Error);
            }
        }

        private void OnRemoteLogEntry(string message, SharedBase.Logging.LoggerEntry.LoggerType loggerType)
        {
            _hub?.LogEntry(SharedBase.Logging.LoggerEntry.CreateEntry(nameof(Rediscovery.Desktop.Hub), message, loggerType));
        }

        [HttpGet]
        public async Task<bool> InitServiceConnection()
        {
            await _hub.Disconnect();
            _hub.Authenticate(_remoteResourceSettings.DesktopHubApplicationKey, connectionConfiguration, (resultModel, state) =>
            {
                if (state)
                {
                    connectionConfiguration.Token = resultModel.Token;
                    _hub.Connect(_remoteResourceSettings.DesktopHubApplicationKey, connectionConfiguration, (listener) =>
                    {
                        if (listener)
                        {
                            OnRemoteLogEntry("Listener response valid!", SharedBase.Logging.LoggerEntry.LoggerType.Trace);
                            _hub.RequestAllData();
                        }
                        else
                        {
                            string msg = "Listener response not valid";
                            _logger.LogWarning(msg);
                            OnRemoteLogEntry(msg, SharedBase.Logging.LoggerEntry.LoggerType.Warning);
                        }
                    });
                } else
                {
                    string msg = "Could not Authenticate for remote resource access";
                    _logger.LogWarning(msg);
                    OnRemoteLogEntry(msg, SharedBase.Logging.LoggerEntry.LoggerType.Warning);
                }
            });
            return true;
        }
    }
}
