using Rediscovery.Features.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Services
{
    public interface IEntityManager
    {
        System.Collections.ObjectModel.ObservableCollection<Features.Connection.Models.ConnectionManifestFeature> ConnectionManifestFeatures { get; set; }
        List<Features.Connection.Models.ConnectionManifestFeature> GetConnectionManifestFeature(Guid modelId);
        Task<List<Features.Connection.Models.ConnectionManifestFeature>> GetConnectionManifestFeatureAsync(Guid modelId);
        List<Features.Connection.Models.ConnectionManifestFeature> GetConnectedConnectionManifestFeature();
        Task<List<Features.Connection.Models.ConnectionManifestFeature>> GetConnectedConnectionManifestFeatureAsync();
        void Clear(Guid modelId);
    }
}
