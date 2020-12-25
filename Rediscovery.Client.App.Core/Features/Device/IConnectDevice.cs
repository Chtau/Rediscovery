using Rediscovery.Client.App.Core.Features.Device.Models;
using Rediscovery.Communication.Consumer.Authentication;
using Rediscovery.Communication.Consumer.Feature;
using Rediscovery.Communication.Consumer.Heartbeat;
using Rediscovery.Communication.Consumer.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Device
{
    public interface IConnectDevice : IDisposable
    {
        event EventHandler<DeviceConnectionState> ConnectionStateChanged;
        event EventHandler<HeartbeatResult> HeartbeatReceived;
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
