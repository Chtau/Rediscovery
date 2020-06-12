using Rediscovery.Features.DesktopConfiguration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.Connection
{
    public interface IConnectService
    {
        string GetToken(Guid configurationId);
        void AutoConnect(Action<string, SharedBase.Connection.Enums.ConnectionState> resultCallback);
        void Connect(DesktopConfigurationModel desktopConfigurationModel, Action<string, SharedBase.Connection.Enums.ConnectionState> resultCallback);
        void Disconnect(DesktopConfigurationModel desktopConfigurationModel, Action<bool> resultCallback);
    }
}
