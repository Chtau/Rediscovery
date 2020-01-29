using Rediscovery.Features.Authentication.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Authentication.ConnectionManifestFeatureStore))]
namespace Rediscovery.Features.Authentication
{

    public class ConnectionManifestFeatureStore : IDataStoreConnectionGuid<Models.ConnectionManifestFeature>
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDBStore db => DependencyService.Get<IDBStore>() ?? new DBStore();
        private Features.Authentication.IConnect connection => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();

        public async Task<bool> AddItemAsync(ConnectionManifestFeature item)
        {
            try
            {
                if (await db.Store.Table<ConnectionManifestFeature>().Where(s => s.Id == item.Id).CountAsync() > 0)
                {
                    ConnectionManifestFeature entity = await db.Store.Table<ConnectionManifestFeature>().FirstOrDefaultAsync(s => s.Id == item.Id);
                    entity.FeatureVersion = item.FeatureVersion;
                    entity.FeatureMinFeatureIntegrationPoint = item.FeatureMinFeatureIntegrationPoint;
                    entity.FeatureMinControlIntegrationPoint = item.FeatureMinControlIntegrationPoint;
                    entity.FeatureId = item.FeatureId;
                    entity.FeatureFeatureIntegrationPoint = item.FeatureFeatureIntegrationPoint;
                    entity.FeatureDisplayName = item.FeatureDisplayName;
                    entity.FeatureControlIntegrationPoint = item.FeatureControlIntegrationPoint;
                    entity.ControlIntegration = item.ControlIntegration;
                    entity.ConnectionId = item.ConnectionId;
                    await UpdateItemAsync(entity);
                }
                else
                {
                    var entity = await db.Store.Table<ConnectionManifestFeature>().FirstOrDefaultAsync(s => s.ConnectionId == item.ConnectionId && s.FeatureId == item.FeatureId);
                    if (entity != null)
                    {
                        entity.FeatureVersion = item.FeatureVersion;
                        entity.FeatureMinFeatureIntegrationPoint = item.FeatureMinFeatureIntegrationPoint;
                        entity.FeatureMinControlIntegrationPoint = item.FeatureMinControlIntegrationPoint;
                        entity.ControlIntegration = item.ControlIntegration;
                        entity.FeatureId = item.FeatureId;
                        entity.FeatureFeatureIntegrationPoint = item.FeatureFeatureIntegrationPoint;
                        entity.FeatureDisplayName = item.FeatureDisplayName;
                        entity.FeatureControlIntegrationPoint = item.FeatureControlIntegrationPoint;
                        entity.ConnectionId = item.ConnectionId;
                        await UpdateItemAsync(entity);
                    } else
                    {
                        if (item.Id == Guid.Empty)
                            item.Id = Guid.NewGuid();
                        await db.Store.InsertAsync(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAllAsync(Guid connectionId)
        {
            var _items = await db.Store.Table<ConnectionManifestFeature>().Where((ConnectionManifestFeature arg) => arg.ConnectionId == connectionId).ToListAsync();
            foreach (var item in _items)
            {
                await db.Store.DeleteAsync(item);
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(Guid connectionId, Guid id)
        {
            var _item = await db.Store.Table<ConnectionManifestFeature>().Where((ConnectionManifestFeature arg) => arg.ConnectionId == connectionId && arg.FeatureId == id).FirstOrDefaultAsync();
            await db.Store.DeleteAsync(_item);

            return await Task.FromResult(true);
        }

        public async Task<ConnectionManifestFeature> GetItemAsync(Guid connectionId, Guid id)
        {
            return await Task.FromResult(
                await db.Store.Table<ConnectionManifestFeature>().Where(s => s.ConnectionId == connectionId && s.Id == id).FirstOrDefaultAsync()
                );
        }

        public async Task<IEnumerable<ConnectionManifestFeature>> GetItemsAsync()
        {
            var conModel = await connection.GetModel();
            if (conModel != null)
                return await db.Store.Table<ConnectionManifestFeature>().Where(x => x.ConnectionId == conModel.Id).ToListAsync();
            return null;
        }

        public async Task<bool> UpdateItemAsync(ConnectionManifestFeature item)
        {
            await db.Store.UpdateAsync(item);

            return await Task.FromResult(true);
        }
    }
}
