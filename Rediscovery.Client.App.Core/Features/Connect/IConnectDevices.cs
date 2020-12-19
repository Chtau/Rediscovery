using Rediscovery.Client.App.Core.Features.Connect.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Connect
{
    public interface IConnectDevices
    {
        event EventHandler<object> ConnectionHeartbeat;
        event EventHandler<object> ConnectionCreated;
        event EventHandler<object> ConnectionLost;
        void Autoconnect();
        void Connect(ConnectionConfiguration connectionConfiguration);
        void Disconnect(Guid connectionConfigurationId);
    }
}
