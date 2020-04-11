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

        public DeviceController(ILogger<DeviceController> logger
            )
        {
            _logger = logger;
        }

        [HttpGet]
        public bool Refresh()
        {
            
            return true;
        }
    }
}
