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

        public enum RemoteFeatureRequestState
        {
            NoFeatures,
            MissingSupport
        }

        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();
        private DesktopFeatures.IFeatureService featureService => DependencyService.Get<DesktopFeatures.IFeatureService>() ?? new DesktopFeatures.FeatureService();
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();

        public event EventHandler<IEnumerable<Connection.Models.ConnectionManifestFeature>> OpenFeatureSelectDialog;
        public event EventHandler<QueueItem> ClientQueueDisplay;
        public event EventHandler<RemoteFeatureRequestState> RemoteFeatureRequest;

        public QueueItem CurrentQueueItem { get; private set; }

        public bool HasQueueItem
        {
            get { return CurrentQueueItem != null; }
        }

        public bool Invoke(string queueInfoText, object data, SharedBase.Enums.ClientNativeResources clientNativeResources)
        {
            CurrentQueueItem = new QueueItem(queueInfoText, data, clientNativeResources);
            ClientQueueDisplay?.Invoke(this, CurrentQueueItem);
            if (entityManager.ConnectionManifestFeatures?.Count > 0)
            {
                OnSelectFeature(clientNativeResources);
                return true;
            }
            RemoteFeatureRequest?.Invoke(this, RemoteFeatureRequestState.NoFeatures);
            return false;
        }

        public bool InvokeCurrentQueue()
        {
            if (CurrentQueueItem != null)
            {
                return Invoke(CurrentQueueItem.QueueInfoText, CurrentQueueItem.Data, CurrentQueueItem.NativeResources);
            }
            return false;
        }

        public void SelectFeatureSelected(Connection.Models.ConnectionManifestFeature feature)
        {
            OnSendResource(CurrentQueueItem, feature);
            CurrentQueueItem = null;
            ClientQueueDisplay?.Invoke(this, CurrentQueueItem);
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
                RemoteFeatureRequest?.Invoke(this, RemoteFeatureRequestState.MissingSupport);
            }
        }

        private void OnSendResource(QueueItem queueItem, Connection.Models.ConnectionManifestFeature feature)
        {
            var config = desktopStore.GetItem(feature.ConfigurationId);
            if (featureService.LoadFeature(config, feature.FeatureId))
            {
                featureService.Start();
                featureService.Send(null, Newtonsoft.Json.JsonConvert.SerializeObject(queueItem.Data), true, (int)queueItem.NativeResources);
            }
        }
    }
}
