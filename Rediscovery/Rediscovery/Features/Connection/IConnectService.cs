using Rediscovery.Features.DesktopConfiguration;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.Connection
{
    public interface IConnectService
    {
        Models.ConnectConfigurationData GetData(Guid configurationId);
        void AutoConnect(Action<string, SharedBase.Connection.Enums.ConnectionState> resultCallback);
        void Connect(DesktopConfigurationModel desktopConfigurationModel, Action<string, SharedBase.Connection.Enums.ConnectionState> resultCallback);
        void Disconnect(DesktopConfigurationModel desktopConfigurationModel, Action<bool> resultCallback);
        CommunicationHeartbeatConsumer.RoundTripResult GetHeartbeat(Guid desktopConfigurationId);
        event EventHandler<Guid> HeartbeatStateChanges;
        void InvokeLogEntry(LoggerEntry loggerEntry);
    }
}
