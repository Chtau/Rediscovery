using Rediscovery.Features.Connection;
using Rediscovery.Features.DesktopFeatures.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopFeatures.ClientFeatureService))]
namespace Rediscovery.Features.DesktopFeatures
{
    public class ClientFeatureService : BaseService, IClientFeatureService
    {
        public class QueueItem
        {
            public Guid Id { get; private set; }
            public string QueueInfoText { get; private set; }
            public object Data { get; private set; }
            public SharedBase.Enums.ClientNativeResources NativeResources { get; private set; }

            public QueueItem(string queueInfoText, object data, SharedBase.Enums.ClientNativeResources clientNativeResources)
            {
                Id = Guid.NewGuid();
                QueueInfoText = queueInfoText;
                Data = data;
                NativeResources = clientNativeResources;
            }
        }


        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();

        public event EventHandler<IEnumerable<Connection.Models.ConnectionManifestFeature>> OpenFeatureSelectDialog;
        public event EventHandler<QueueItem> ClientQueueDisplay;

        public QueueItem CurrentQueueItem { get; private set; }

        public bool Invoke(string queueInfoText, object data, SharedBase.Enums.ClientNativeResources clientNativeResources)
        {
            CurrentQueueItem = new QueueItem(queueInfoText, data, clientNativeResources);
            ClientQueueDisplay?.Invoke(this, CurrentQueueItem);
            if (entityManager.ConnectionManifestFeatures?.Count > 0)
            {
                OnSelectFeature(clientNativeResources);
                return true;
            }
            return false;
        }

        public void SelectFeatureSelected(Connection.Models.ConnectionManifestFeature feature)
        {
            OnSendResource(CurrentQueueItem, feature);
        }

        private void OnSelectFeature(SharedBase.Enums.ClientNativeResources clientNativeResources)
        {
            var featuresForOpenWith = entityManager.ConnectionManifestFeatures?.Where(x => x.FeatureFeatureIntegrationPoint == SharedBase.Device.IntegrationPoint.Mobile && x.FeatureNativeResource.HasFlag(clientNativeResources));
            if (featuresForOpenWith?.Count() > 0)
            {
                OpenFeatureSelectDialog?.Invoke(this, featuresForOpenWith);
            }
            else
            {
                _logger.LogWarning("No Feature found which supports [OpenWithIntent]");
            }
        }

        private void OnSendResource(QueueItem queueItem, Connection.Models.ConnectionManifestFeature feature)
        {
            switch (queueItem.NativeResources)
            {
                case SharedBase.Enums.ClientNativeResources.None:
                    break;
                case SharedBase.Enums.ClientNativeResources.OpenWithIntent:
                    OnOpenWithIntentReceived((IntentReceivedModel)queueItem.Data, feature);
                    break;
                default:
                    break;
            }
        }

        private void OnOpenWithIntentReceived(IntentReceivedModel intentReceivedModel, Connection.Models.ConnectionManifestFeature feature)
        {
            // TODO: handle if we are not connected to any device
            // TODO: if we are not connected with a device but this intent in a queue to allow the user to connect and then proceed with the action
            // TODO: check if the feature has resource setting (resource setting object should be stored in a generic serialized object in the manifest)
            
        }
    }
}
