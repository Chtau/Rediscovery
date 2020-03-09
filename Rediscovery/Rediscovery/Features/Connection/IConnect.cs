using Microsoft.AspNetCore.SignalR.Client;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.Connection
{
    public interface IConnect
    {
        Task AutoConnect();
        Task TryConnect(Guid connectionId);
        Task<HubConnection> GetConnectionAuth();
        Task<HubConnection> GetConnectionFeature();
        Task CloseConnections();
        Task ValidateKey(Guid connectionId, string key);
        bool IsConnected(Models.ConnectionInfo model, Connect.HubTypes hubType);
        Task<Models.ConnectionInfo> GetModel();
        event EventHandler<Models.ConnectionInfo> HelloReceived;
        event EventHandler<Tuple<Models.ConnectionInfo, List<Models.ConnectionManifestFeature>>> ManifestReceived;
        event EventHandler<Models.ConnectionInfo> ConnectionChanged;
        Task<HttpClient> GetHttpClientFeature();
    }
}
