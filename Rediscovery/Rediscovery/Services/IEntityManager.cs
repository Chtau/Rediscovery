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
        List<Features.Connection.Models.ConnectionManifestFeature> GetConnectionManifestFeature();
        Task<List<Features.Connection.Models.ConnectionManifestFeature>> GetConnectionManifestFeatureAsync();
        void Clear();
    }
}
