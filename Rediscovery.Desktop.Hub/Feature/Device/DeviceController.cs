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
        private readonly IPCPipe.IPipeClient _pipeClient;

        public DeviceController(ILogger<DeviceController> logger,
            IDeviceService deviceService,
            IPCPipe.IPipeClient pipeClient)
        {
            _logger = logger;
            _deviceService = deviceService;
            _deviceService.DeviceInfoReceived += _deviceService_DeviceInfoReceived;
            _pipeClient = pipeClient;
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
        public bool Refresh()
        {
            Task.Run(OnHeartbeatListener);
            _deviceService.Refresh();
            return true;
        }

        /*[HttpGet]
        public IEnumerable<DeviceInfo> Get()
        {
            return _deviceService.Items;
        }*/

        private void OnHeartbeatListener()
        {
            Console.WriteLine("Init Heartbeat listner");
            try
            {
                _pipeClient.Listen("rediscoveryheartbeathub", (value) =>
                {
                    System.Diagnostics.Debug.Print($"Heartbeat received:{value}");
                    Console.WriteLine($"Heartbeat received:{value}");
                });
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
