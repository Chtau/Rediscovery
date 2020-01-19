using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.Authentication
{
    public interface IInternalHub
    {
        Task<HubConnection> GetConnection(Models.Connection model);
        Task CloseConnections();
        event EventHandler<Models.Connection> ConnectionChanged;
    }
}
