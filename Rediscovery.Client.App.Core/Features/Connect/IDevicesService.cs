using Rediscovery.Client.App.Core.Features.Connect.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Connect
{
    public interface IDevicesService
    {
        event EventHandler<DeviceConnectionState> ConnectionStateChanged;
        event EventHandler<HeartbeatResult> HeartbeatReceived;
        bool Probe(Guid connectionId);
        void Connect(Guid connectionId);
        bool Disconnect(Guid connectionId);
        void Autoconnect();
        void AddOrUpdateConnectionConfiguration(params ConnectionConfiguration[] connectionConfigurations);
        void RemoveConnectionConfiguration(params Guid[] connectionConfigurationIds);
    }
}
