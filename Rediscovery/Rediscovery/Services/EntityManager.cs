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
    public class EntityManager : BaseService, IEntityManager
    {
        private Features.Connection.IConnect connection => DependencyService.Get<Features.Connection.IConnect>() ?? new Features.Connection.Connect();

        public System.Collections.ObjectModel.ObservableCollection<Features.Connection.Models.ConnectionManifestFeature> ConnectionManifestFeatures { get; set; }

        public EntityManager()
        {
            ConnectionManifestFeatures = new System.Collections.ObjectModel.ObservableCollection<Features.Connection.Models.ConnectionManifestFeature>();
        }

        public void Clear(Guid modelId)
        {
            var removeFeatures = ConnectionManifestFeatures.Where(x => x.ConnectionId == modelId);
            foreach (var item in removeFeatures)
            {
                ConnectionManifestFeatures.Remove(item);
            }
        }

        public List<Features.Connection.Models.ConnectionManifestFeature> GetConnectionManifestFeature(Guid modelId)
        {
            var result = new List<Features.Connection.Models.ConnectionManifestFeature>();
            var task = Task.Factory.StartNew(async () =>
            {
                result = await GetConnectionManifestFeatureAsync(modelId);
            });
            task.Wait();
            return result;
        }

        public async Task<List<Features.Connection.Models.ConnectionManifestFeature>> GetConnectionManifestFeatureAsync(Guid modelId)
        {
            var conModel = await connection.GetModel(modelId);
            if (conModel != null)
                return ConnectionManifestFeatures.Where(x => x.ConnectionId == conModel.Id).ToList();
            return new List<Features.Connection.Models.ConnectionManifestFeature>();
        }

        public List<Features.Connection.Models.ConnectionManifestFeature> GetConnectedConnectionManifestFeature()
        {
            var result = new List<Features.Connection.Models.ConnectionManifestFeature>();
            var task = Task.Factory.StartNew(async () =>
            {
                result = await GetConnectedConnectionManifestFeatureAsync();
            });
            task.Wait();
            return result;
        }

        public async Task<List<Features.Connection.Models.ConnectionManifestFeature>> GetConnectedConnectionManifestFeatureAsync()
        {
            var conModel = await connection.GetConnectedModels();
            if (conModel != null)
                return ConnectionManifestFeatures.ToList();
            return new List<Features.Connection.Models.ConnectionManifestFeature>();
        }
    }
}
