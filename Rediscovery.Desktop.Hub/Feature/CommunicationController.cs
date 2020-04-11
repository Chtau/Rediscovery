using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rediscovery.Desktop.Hub.Feature.Device;
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

        public CommunicationController(ILogger<CommunicationController> logger,
            IDeviceService deviceService
            )
        {
            _logger = logger;
            _deviceService = deviceService;
            _deviceService.DeviceInfoReceived += _deviceService_DeviceInfoReceived;
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
            return true;
        }
    }
}
