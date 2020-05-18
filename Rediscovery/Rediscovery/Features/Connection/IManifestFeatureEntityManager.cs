using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.Connection
{
    public interface IManifestFeatureEntityManager
    {
        System.Collections.ObjectModel.ObservableCollection<Features.Connection.Models.ConnectionManifestFeature> ConnectionManifestFeatures { get; set; }
        List<Features.Connection.Models.ConnectionManifestFeature> GetConnectionManifestFeature(Guid configurationId);
        List<Features.Connection.Models.ConnectionManifestFeature> GetConnectedConnectionManifestFeature();
        void Clear(Guid configurationId);
        void AddManifestData(SharedCoreModels.Manifest manifest, Guid configurationId, string displayName);
    }
}
