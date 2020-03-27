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
