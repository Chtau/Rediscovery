using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rediscovery.Desktop.Hub.Feature.InternalIPCModels;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature
{
    public class CommunicationController : Shared.BaseController
    {
        private readonly ILogger<CommunicationController> _logger;
        private readonly CommunicationResourceConsumer.IHub _hub;
        private readonly SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration _remoteResourceSettings;

        private CommunicationResourceConsumer.Models.ConnectionConfiguration connectionConfiguration;

        public CommunicationController(ILogger<CommunicationController> logger,
            CommunicationResourceConsumer.IHub hub,
            IOptions<SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration> remoteResourceSettings
            )
        {
            _remoteResourceSettings = remoteResourceSettings.Value;
            _logger = logger;
            _hub = hub;
            connectionConfiguration = new CommunicationResourceConsumer.Models.ConnectionConfiguration
            {
                Address = _remoteResourceSettings.IP + (_remoteResourceSettings.Port != null ? ":" + _remoteResourceSettings.Port : ""),
                DisplayName = _remoteResourceSettings.DesktopHubApplicationKey,
                Id = Guid.NewGuid(),
                State = CommunicationResourceConsumer.ConnectionState.None,
                Token = null
            };
            _hub.Init(new CommunicationResourceConsumer.Logger(), "/remote/resource/hub");
            _hub.ActiveDeviceInfoReceived += _deviceService_ActiveDeviceInfoReceived;
            _hub.DeviceInfoReceived += _deviceService_DeviceInfoReceived;
            _hub.LogEntryReceived += _loggerService_LoggerDataReceived;
            _hub.ServiceFeatureReceived += _featureService_DeviceFeatureReceived;
            _hub.PendingAuthenticationDeviceReceived += _hub_PendingAuthenticationDeviceReceived;

            ElectronNET.API.Electron.IpcMain.On("resolvependingdevice-ipc", (args) =>
            {
                try
                {
                    var param = Newtonsoft.Json.JsonConvert.DeserializeObject<PendingAuthenticationResolve>(args?.ToString());
                    _logger.LogDebug($"Resolve pending device authentication for Id:{param.Id} Accept:{param.Accept}");
                    _hub.RequestResolvePendingAuthenticationDevice(param.Id, param.Accept);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in IPC listener from UI [resolvependingdevice-ipc]");
                }
            });
            ElectronNET.API.Electron.IpcMain.On("deletedeviceinfo-ipc", (args) =>
            {
                try
                {
                    var param = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceInfo>(args?.ToString());
                    _logger.LogDebug($"Delete device for Id:{param.Id}");
                    _hub.RequestDeleteDevice(param);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in IPC listener from UI [deletedeviceinfo-ipc]");
                }
            });
            ElectronNET.API.Electron.IpcMain.On("updatedeviceinfo-ipc", (args) =>
            {
                try
                {
                    var param = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceInfo>(args?.ToString());
                    _logger.LogDebug($"Update device authentication for Id:{param.Id}");
                    _hub.RequestUpdateDevice(param);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in IPC listener from UI [updatedeviceinfo-ipc]");
                }
            });
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
            }
        }

        private void _loggerService_LoggerDataReceived(object sender, LoggerEntryModel e)
        {
            try
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "loggermessage-ipc", e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoggerDataReceived via IPC from Service");
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
            }
        }

        [HttpGet]
        public bool InitServiceConnection()
        {
            _hub.Authenticate(_remoteResourceSettings.DesktopHubApplicationKey, connectionConfiguration, (resultModel, state) =>
            {
                if (state)
                {
                    connectionConfiguration.Token = resultModel.Token;
                    _hub.Connect(_remoteResourceSettings.DesktopHubApplicationKey, connectionConfiguration, (listener) =>
                    {
                        if (listener)
                        {
                            _hub.RequestAllData();
                        }
                        else
                            _logger.LogWarning("Listener response not valid");
                    });
                } else
                {
                    _logger.LogWarning("Could not Authenticate for remote resource access");
                }
            });
            return true;
        }
    }
}
