using Rediscovery.Client.App.Core.Features.Device.Models;
using Rediscovery.Client.App.Core.Features.Heartbeat.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Service
{
    public interface IServiceManager
    {
        event EventHandler<object> DeviceConnectionStateChanged;
        event EventHandler<HeartbeatResult> HeartbeatReceived;
    }
}
