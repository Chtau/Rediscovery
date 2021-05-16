using Rediscovery.Client.App.Core.Features.Device.Models;
using Rediscovery.Client.Shared.Core.Features.Heartbeat.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Device
{
    public interface IDevicesManager
    {
        event EventHandler<DeviceConnectionState> ConnectionStateChanged;
        event EventHandler<HeartbeatResult<ConnectionConfiguration>> HeartbeatReceived;
        /// <summary>
        /// Starts a network listener for incoming probes
        /// </summary>
        void Listen();
        /// <summary>
        /// Sends a probe signal to the connection configuration to check if it exists or is reachable
        /// </summary>
        /// <param name="connectionId">Known connection configuration Id</param>
        /// <returns></returns>
        bool Probe(Guid connectionId);
        void Connect(Guid connectionId);
        bool Disconnect(Guid connectionId);
        void Autoconnect();
        void AddOrUpdateConnectionConfiguration(params ConnectionConfiguration[] connectionConfigurations);
        void RemoveConnectionConfiguration(params Guid[] connectionConfigurationIds);
    }
}
