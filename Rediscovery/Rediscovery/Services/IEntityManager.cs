using Rediscovery.Features.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Services
{
    public interface IEntityManager
    {
        System.Collections.ObjectModel.ObservableCollection<ConnectionManifestFeature> ConnectionManifestFeatures { get; set; }
        List<ConnectionManifestFeature> GetConnectionManifestFeature();
        Task<List<ConnectionManifestFeature>> GetConnectionManifestFeatureAsync();
        void Clear();
    }
}
