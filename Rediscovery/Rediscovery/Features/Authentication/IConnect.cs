using Microsoft.AspNetCore.SignalR.Client;
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
        Task<HubConnection> GetConnectionAuth();
        Task<HubConnection> GetConnectionFeature();
        Task CloseConnections();
        Task ValidateKey(Guid connectionId, string key);
        bool IsConnected(Models.Connection model, Connect.HubTypes hubType);
        Task<Models.Connection> GetModel();
        event EventHandler<Models.Connection> HelloReceived;
        event EventHandler<Tuple<Models.Connection, List<Models.ConnectionManifestFeature>>> ManifestReceived;
        event EventHandler<Models.Connection> ConnectionChanged;
    }
}
