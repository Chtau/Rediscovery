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

        public async Task<bool> AddItemAsync(ConnectionManifestFeature item)
        {
            if (await db.Store.Table<ConnectionManifestFeature>().Where(s => s.ConnectionId == item.ConnectionId && s.FeatureId == item.FeatureId).CountAsync() > 0)
            {
                await UpdateItemAsync(item);
            }
            else
            {
                await db.Store.InsertAsync(item);
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
                await db.Store.Table<ConnectionManifestFeature>().Where(s => s.ConnectionId == connectionId && s.FeatureId == id).FirstOrDefaultAsync()
                );
        }

        public async Task<IEnumerable<ConnectionManifestFeature>> GetItemsAsync(Guid connectionId)
        {
            return await db.Store.Table<ConnectionManifestFeature>().Where(x => x.ConnectionId == connectionId).ToListAsync();
        }

        public async Task<bool> UpdateItemAsync(ConnectionManifestFeature item)
        {
            await db.Store.UpdateAsync(item);

            return await Task.FromResult(true);
        }
    }
}
