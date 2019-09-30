using Rediscovery.Features.Authentication.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Authentication.ConnectionStore))]
namespace Rediscovery.Features.Authentication
{
    public class ConnectionStore : IDataStoreGuid<Models.Connection>
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDBStore db => DependencyService.Get<IDBStore>() ?? new DBStore();

        public bool AddItem(Connection item)
        {
            return AddItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> AddItemAsync(Models.Connection item)
        {
            if (await db.Store.Table<Models.Connection>().Where(s => s.Id == item.Id).CountAsync() > 0)
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
            var _item = await db.Store.Table<Models.Connection>().Where((Models.Connection arg) => arg.Id == id).FirstOrDefaultAsync();
            await db.Store.DeleteAsync(_item);

            return await Task.FromResult(true);
        }

        public Connection GetItem(Guid id)
        {
            return GetItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<Models.Connection> GetItemAsync(Guid id)
        {
            return await Task.FromResult(
                await db.Store.Table<Models.Connection>().Where(s => s.Id == id).FirstOrDefaultAsync()
                );
        }

        public IEnumerable<Connection> GetItems(bool forceRefresh = false)
        {
            return GetItemsAsync(forceRefresh).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<Models.Connection>> GetItemsAsync(bool forceRefresh = false)
        {
            return await db.Store.Table<Models.Connection>().ToListAsync();
        }

        public bool UpdateItem(Connection item)
        {
            return UpdateItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> UpdateItemAsync(Models.Connection item)
        {
            await db.Store.UpdateAsync(item);

            return await Task.FromResult(true);
        }
    }
}
