using Rediscovery.Features.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IFeatureExchange
    {
        void InitConnection();
        Task InitConnectionAsync();

        event EventHandler<(Guid connectionId, Guid featureId, string profileId, object data)> DesktopResponseReceived;
        Task Send(Connection.Models.ConnectionManifestFeature feature, string profileId, object data);
        Task Start(Connection.Models.ConnectionManifestFeature feature);
        Task Stop(Connection.Models.ConnectionManifestFeature feature);
    }
}
