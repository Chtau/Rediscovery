using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IClientFeatureService
    {
        ClientFeatureService.QueueItem CurrentQueueItem { get; }
        bool Invoke(string queueInfoText, object data, SharedBase.Enums.ClientNativeResources clientNativeResources);
        void SelectFeatureSelected(Connection.Models.ConnectionManifestFeature feature);
        event EventHandler<IEnumerable<Connection.Models.ConnectionManifestFeature>> OpenFeatureSelectDialog;
    }
}
