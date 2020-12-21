using Rediscovery.Client.App.Core.Features.Connect.Models;
using Rediscovery.Communication.Consumer.Authentication;
using Rediscovery.Communication.Consumer.Feature;
using Rediscovery.Communication.Consumer.Heartbeat;
using Rediscovery.Communication.Consumer.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Connect
{
    public interface IConnectDevice
    {
        event EventHandler<DeviceConnectionState> ConnectionStateChanged;
        ConnectionConfiguration ConnectionConfiguration { get; }
        void SetConfiguration(ConnectionConfiguration connectionConfiguration);
        /// <summary>
        /// Checks if there is a endpoint running on the address and port from the configuration
        /// </summary>
        bool Probe();
        void Connect();
        bool Disconnect();
    }
}
