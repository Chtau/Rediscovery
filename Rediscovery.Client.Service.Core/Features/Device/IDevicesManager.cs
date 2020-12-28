using Rediscovery.Client.Shared.Core.Features.Heartbeat.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Service.Core.Features.Device
{
    public interface IDevicesManager
    {
        event EventHandler<object> DeviceConnectionStateChanged;
        event EventHandler<HeartbeatResult<object>> HeartbeatReceived;
    }
}
