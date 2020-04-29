using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rediscovery.Desktop.Hub.Feature.Device;
using Rediscovery.Desktop.Hub.Feature.Features;
using Rediscovery.Desktop.Hub.Feature.Logger;
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
        private readonly IDeviceService _deviceService;
        private readonly ILoggerService _loggerService;
        private readonly IFeatureService _featureService;

        public CommunicationController(ILogger<CommunicationController> logger,
            IDeviceService deviceService,
            ILoggerService loggerService,
            IFeatureService featureService
            )
        {
            _logger = logger;
            _deviceService = deviceService;
            _loggerService = loggerService;
            _featureService = featureService;
            _deviceService.DeviceInfoReceived += _deviceService_DeviceInfoReceived;
            _deviceService.ActiveDeviceInfoReceived += _deviceService_ActiveDeviceInfoReceived;
            _loggerService.LoggerDataReceived += _loggerService_LoggerDataReceived;
            _featureService.DeviceFeatureReceived += _featureService_DeviceFeatureReceived;
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
            _deviceService.Init();
            _loggerService.Init();
            _featureService.Init();
            return true;
        }
    }
}
