using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Device
{
    public class DeviceController : Shared.BaseController
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly IDeviceService _deviceService;

        public DeviceController(ILogger<DeviceController> logger,
            IDeviceService deviceService)
        {
            _logger = logger;
            _deviceService = deviceService;
            _deviceService.DeviceInfoReceived += _deviceService_DeviceInfoReceived;
        }

        private void _deviceService_DeviceInfoReceived(object sender, List<DeviceInfo> e)
        {
            ElectronNET.API.Electron.IpcMain.On("async-msg", (args) =>
            {
                var mainWindow = ElectronNET.API.Electron.WindowManager.BrowserWindows.First();
                ElectronNET.API.Electron.IpcMain.Send(mainWindow, "asynchronous-reply", e);
            });
        }

        [HttpGet]
        public ActionResult Refresh()
        {
            _deviceService.Refresh();
            return Ok();
        }

        [HttpGet]
        public IEnumerable<DeviceInfo> Get()
        {
            return _deviceService.Items;
        }
    }
}
