using Rediscovery.Features.DesktopConfiguration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.Connection
{
    public interface IConnectService
    {
        void AutoConnect(Action<bool, SharedCoreModels.Enums.ConnectionState> resultCallback);
        void Connect(DesktopConfigurationModel desktopConfigurationModel, Action<bool, SharedCoreModels.Enums.ConnectionState> resultCallback);
    }
}
