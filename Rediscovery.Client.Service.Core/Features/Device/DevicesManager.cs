using Rediscovery.Client.Shared.Core.Features.Heartbeat.Models;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Service.Core.Features.Device
{
    public class DevicesManager : IDevicesManager
    {
        private readonly ILogger _logger;

        public event EventHandler<object> DeviceConnectionStateChanged;
        public event EventHandler<HeartbeatResult<object>> HeartbeatReceived;

        public DevicesManager(ILogger logger)
        {
            _logger = logger;
            // TODO: initialize all provider services
        }
    }
}
