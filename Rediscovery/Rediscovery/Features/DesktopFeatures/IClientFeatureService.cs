using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IClientFeatureService
    {
        ClientFeatureService.QueueItem CurrentQueueItem { get; }
        bool HasQueueItem { get; }
        bool InvokeCurrentQueue();
        bool Invoke(string queueInfoText, object data, SharedBase.Enums.ClientNativeResources clientNativeResources);
        void SelectFeatureSelected(Connection.Models.ConnectionManifestFeature feature);
        event EventHandler<IEnumerable<Connection.Models.ConnectionManifestFeature>> OpenFeatureSelectDialog;
        event EventHandler<ClientFeatureService.QueueItem> ClientQueueDisplay;
        event EventHandler<ClientFeatureService.RemoteFeatureRequestState> RemoteFeatureRequest;
    }
}
