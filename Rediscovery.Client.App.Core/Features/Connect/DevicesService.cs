using Rediscovery.Client.App.Core.Features.Connect.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Connect
{
    public class DevicesService : IDevicesService
    {
        private List<IConnectDevice> connectDevices = new List<IConnectDevice>();

        public event EventHandler<DeviceConnectionState> ConnectionStateChanged;

        public void Autoconnect()
        {
            throw new NotImplementedException();
        }

        public void Connect(Guid connectionId)
        {
            throw new NotImplementedException();
        }

        public bool Disconnect(Guid connectionId)
        {
            throw new NotImplementedException();
        }

        public bool Probe(Guid connectionId)
        {
            throw new NotImplementedException();
        }
    }
}
