using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rediscovery.Desktop.Hub.Feature.Device;
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

        public CommunicationController(ILogger<CommunicationController> logger,
            IDeviceService deviceService,
            ILoggerService loggerService
            )
        {
            _logger = logger;
            _deviceService = deviceService;
            _loggerService = loggerService;
            _deviceService.DeviceInfoReceived += _deviceService_DeviceInfoReceived;
            _loggerService.LoggerDataReceived += _loggerService_LoggerDataReceived;
        }

        private void _loggerService_LoggerDataReceived(object sender, LiveLoggerModel e)
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
            // TODO: only for tests removed
            //_deviceService.Init();
            _loggerService.Init();
            return true;
        }
    }
}
