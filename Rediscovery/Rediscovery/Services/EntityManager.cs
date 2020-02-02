using Rediscovery.Features.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.EntityManager))]
namespace Rediscovery.Services
{
    public class EntityManager : IEntityManager
    {
        private Features.Authentication.IConnect connection => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();

        public System.Collections.ObjectModel.ObservableCollection<ConnectionManifestFeature> ConnectionManifestFeatures { get; set; }

        public EntityManager()
        {
            ConnectionManifestFeatures = new System.Collections.ObjectModel.ObservableCollection<ConnectionManifestFeature>();
        }

        public void Clear()
        {
            ConnectionManifestFeatures.Clear();
        }

        public List<ConnectionManifestFeature> GetConnectionManifestFeature()
        {
            var result = new List<ConnectionManifestFeature>();
            var task = Task.Factory.StartNew(async () =>
            {
                result = await GetConnectionManifestFeatureAsync();
            });
            task.Wait();
            return result;
        }

        public async Task<List<ConnectionManifestFeature>> GetConnectionManifestFeatureAsync()
        {
            var conModel = await connection.GetModel();
            if (conModel != null)
                return ConnectionManifestFeatures.Where(x => x.ConnectionId == conModel.Id).ToList();
            return new List<ConnectionManifestFeature>();
        }
    }
}
