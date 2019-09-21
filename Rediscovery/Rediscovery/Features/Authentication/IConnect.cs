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
        Task CloseConnections();
        event EventHandler<Models.Connection> HelloReceived;
        event EventHandler<Tuple<Models.Connection, List<Models.ConnectionManifestFeature>>> ManifestReceived;
    }
}
