using Rediscovery.Features.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Desktops
{
    public interface IFeatureExchange
    {
        event EventHandler<(Guid connectionId, Guid featureId, object data)> DesktopResponseReceived;
        Task Send(Connection model, ConnectionManifestFeature feature, object data);
        Task CloseConnections();
    }
}
