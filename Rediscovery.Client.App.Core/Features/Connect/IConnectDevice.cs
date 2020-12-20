using Rediscovery.Client.App.Core.Features.Connect.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Connect
{
    public interface IConnectDevice
    {
        event EventHandler<DeviceConnectionState> ConnectionStateChanged;
        ConnectionConfiguration ConnectionConfiguration { get; }
        void Connect(ConnectionConfiguration connectionConfiguration);
        bool Disconnect();
    }
}
