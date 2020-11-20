using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;
using Rediscovery.Services;
using System.Threading;
using Rediscovery.Features.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Connection.ManifestFeatureEntityManager))]
namespace Rediscovery.Features.Connection
{
    public class ManifestFeatureEntityManager : BaseService, IManifestFeatureEntityManager
    {
        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();

        public System.Collections.ObjectModel.ObservableCollection<Features.Connection.Models.ConnectionManifestFeature> ConnectionManifestFeatures { get; set; }

        public ManifestFeatureEntityManager()
        {
            ConnectionManifestFeatures = new System.Collections.ObjectModel.ObservableCollection<Features.Connection.Models.ConnectionManifestFeature>();
        }

        public void Clear(Guid configurationId)
        {
            try
            {
                var removeFeatures = ConnectionManifestFeatures.Where(x => x.ConfigurationId == configurationId);
                if (removeFeatures != null)
                {
                    foreach (var item in removeFeatures.ToList())
                    {
                        if (ConnectionManifestFeatures.Contains(item))
                            ConnectionManifestFeatures.Remove(item);
                    }
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void AddManifestData(SharedBase.Connection.Manifest manifest, Guid configurationId, string displayName)
        {
            try
            {
                try
                {
                    var config = desktopStore.GetItem(configurationId);
                    config.ManifestAppMinimumVersion = SharedBase.Core.Version.ConvertFrom(manifest.AppMinimumVersion);
                    config.ManifestClientName = manifest.ClientName;
                    config.ManifestClientVersion = SharedBase.Core.Version.ConvertFrom(manifest.ClientVersion);
                    desktopStore.UpdateItem(config);
                } catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                foreach (var item in manifest.SupportedFeatures)
                {
                    var feature = new Connection.Models.ConnectionManifestFeature
                    {
                        ConfigurationId = configurationId,
                        ConnectionDisplayName = displayName,
                        FeatureDisplayName = item.DisplayName,
                        FeatureControlIntegrationPoint = item.ControlIntegrationPoint,
                        FeatureFeatureIntegrationPoint = item.FeatureIntegrationPoint,
                        FeatureId = item.Id,
                        FeatureMinControlIntegrationPoint = SharedBase.Core.Version.ConvertFrom(item.MinimalControlIntegrationPoint),
                        FeatureMinFeatureIntegrationPoint = SharedBase.Core.Version.ConvertFrom(item.MinimalFeatureIntegrationPoint),
                        FeatureVersion = SharedBase.Core.Version.ConvertFrom(item.Version),
                        FeatureDescription = item.ClientDescription,
                        FeatureNativeResource = (SharedBase.Enums.ClientNativeResources)item.NativeResources
                    };
                    var connectionManifestFeature = ConnectionManifestFeatures.FirstOrDefault(x => x.ConfigurationId == configurationId && x.FeatureId == feature.FeatureId);
                    if (connectionManifestFeature != null)
                    {
                        connectionManifestFeature = feature;
                    }
                    else
                    {
                        ConnectionManifestFeatures.Add(feature);
                    }
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public List<Features.Connection.Models.ConnectionManifestFeature> GetConnectionManifestFeature(Guid configurationId)
        {
            var mani = ConnectionManifestFeatures.Where(x => x.ConfigurationId == configurationId)?.ToList();
            if (mani != null)
                return mani;
            return new List<Features.Connection.Models.ConnectionManifestFeature>();
        }

        public List<Features.Connection.Models.ConnectionManifestFeature> GetConnectedConnectionManifestFeature()
        {
            var mani = ConnectionManifestFeatures?.ToList();
            if (mani != null)
                return mani;
            return new List<Features.Connection.Models.ConnectionManifestFeature>();
        }
    }
}
