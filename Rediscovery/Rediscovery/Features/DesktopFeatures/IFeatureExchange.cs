using Rediscovery.Features.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IFeatureExchange
    {
        event EventHandler<(Guid connectionId, Guid featureId, object data)> DesktopResponseReceived;
        Task Send(ConnectionManifestFeature feature, object data);
        Task Start(ConnectionManifestFeature feature);
        Task Stop(ConnectionManifestFeature feature);
    }
}
