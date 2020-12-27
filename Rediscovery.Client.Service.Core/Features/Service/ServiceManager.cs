using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Service.Core.Features.Service
{
    public class ServiceManager : IServiceManager
    {
        public event EventHandler<object> DeviceConnectionStateChanged;
        public event EventHandler<object> HeartbeatReceived;

        public ServiceManager()
        {
            // TODO: initialize all provider services
        }
    }
}
