using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.Connection
{
    public interface IInternalHub
    {
        bool IsConnected { get; }
        Task<HubConnection> GetConnection(DesktopConfiguration.DesktopConfigurationModel model);
        Task CloseConnections();
        event EventHandler<DesktopConfiguration.DesktopConfigurationModel> ConnectionChanged;
    }
}
