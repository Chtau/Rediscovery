using Rediscovery.Features.Authentication.Models;
using Rediscovery.Features.Connection.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Connection.ConnectionStore))]
namespace Rediscovery.Features.Connection
{
    public class ConnectionStore : IDataStoreGuid<Models.ConnectionInfo>
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDBStore db => DependencyService.Get<IDBStore>() ?? new DBStore();

        public bool AddItem(Models.ConnectionInfo item)
        {
            return AddItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> AddItemAsync(Models.ConnectionInfo item)
        {
            if (await db.Store.Table<Models.ConnectionInfo>().Where(s => s.Id == item.Id).CountAsync() > 0)
            {
                await UpdateItemAsync(item);
            }
            else
            {
                await db.Store.InsertAsync(item);
            }

            return await Task.FromResult(true);
        }

        public bool DeleteItem(Guid id)
        {
            return DeleteItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var _item = await db.Store.Table<Models.ConnectionInfo>().Where((Models.ConnectionInfo arg) => arg.Id == id).FirstOrDefaultAsync();
            await db.Store.DeleteAsync(_item);

            return await Task.FromResult(true);
        }

        public ConnectionInfo GetItem(Guid id)
        {
            return GetItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<Models.ConnectionInfo> GetItemAsync(Guid id)
        {
            return await Task.FromResult(
                await db.Store.Table<Models.ConnectionInfo>().Where(s => s.Id == id).FirstOrDefaultAsync()
                );
        }

        public IEnumerable<ConnectionInfo> GetItems(bool forceRefresh = false)
        {
            return GetItemsAsync(forceRefresh).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<Models.ConnectionInfo>> GetItemsAsync(bool forceRefresh = false)
        {
            return await db.Store.Table<Models.ConnectionInfo>().ToListAsync();
        }

        public bool UpdateItem(ConnectionInfo item)
        {
            return UpdateItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> UpdateItemAsync(Models.ConnectionInfo item)
        {
            await db.Store.UpdateAsync(item);

            return await Task.FromResult(true);
        }
    }
}
