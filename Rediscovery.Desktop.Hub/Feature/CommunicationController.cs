using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly CommunicationConsumer.IHub _hub;
        private readonly SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration _remoteResourceSettings;

        private CommunicationConsumer.Models.ConnectionConfiguration connectionConfiguration;

        public CommunicationController(ILogger<CommunicationController> logger,
            CommunicationConsumer.IHub hub,
            IOptions<SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration> remoteResourceSettings
            )
        {
            _remoteResourceSettings = remoteResourceSettings.Value;
            _logger = logger;
            _hub = hub;
            connectionConfiguration = new CommunicationConsumer.Models.ConnectionConfiguration
            {
                Address = _remoteResourceSettings.IP + (_remoteResourceSettings.Port != null ? ":" + _remoteResourceSettings.Port : ""),
                DisplayName = _remoteResourceSettings.DesktopHubApplicationKey,
                Id = Guid.NewGuid(),
                State = CommunicationConsumer.ConnectionState.None,
                Token = null
            };
            _hub.Init(new CommunicationConsumer.Logger(), "/remote/resource/hub");
            _hub.ActiveDeviceInfoReceived += _deviceService_ActiveDeviceInfoReceived;
            _hub.DeviceInfoReceived += _deviceService_DeviceInfoReceived;
            _hub.LogEntryReceived += _loggerService_LoggerDataReceived;
            _hub.ServiceFeatureReceived += _featureService_DeviceFeatureReceived;
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
            /*ElectronNET.API.Electron.IpcMain.On("async-msg", (args) =>
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "asynchronous-reply", e);
            });*/
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
                            _hub.RequestAllData();
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
