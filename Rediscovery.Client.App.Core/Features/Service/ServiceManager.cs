using Rediscovery.Client.App.Core.Features.Device.Models;
using Rediscovery.Client.App.Core.Features.Heartbeat.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Service
{
    public class ServiceManager : IServiceManager
    {
        public event EventHandler<object> DeviceConnectionStateChanged;
        public event EventHandler<HeartbeatResult> HeartbeatReceived;

        public ServiceManager()
        {
            // TODO: initialize all provider services
        }
    }
}
