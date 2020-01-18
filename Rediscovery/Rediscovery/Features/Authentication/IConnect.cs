using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.DesktopConfiguration;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.Authentication
{
    public interface IConnect
    {
        Task AutoConnect();
        Task TryConnect(Guid connectionId);
        Task<HubConnection> GetConnection(Models.Connection model);
        Task CloseConnections();
        Task ValidateKey(Guid connectionId, string key);
        event EventHandler<Models.Connection> HelloReceived;
        event EventHandler<Tuple<Models.Connection, List<Models.ConnectionManifestFeature>>> ManifestReceived;
        event EventHandler<Models.Connection> ConnectionChanged;
    }
}
