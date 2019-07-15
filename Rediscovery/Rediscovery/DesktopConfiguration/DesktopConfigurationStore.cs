using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.DesktopConfiguration.DesktopConfigurationStore))]
namespace Rediscovery.DesktopConfiguration
{
    public class DesktopConfigurationStore : IDataStoreGuid<DesktopConfigurationModel>
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDBStore db => DependencyService.Get<IDBStore>() ?? new DBStore();

        public async Task<bool> AddItemAsync(DesktopConfigurationModel item)
        {
            if (await db.Store.Table<DesktopConfigurationModel>().Where(s => s.Id == item.Id).CountAsync() > 0)
            {
                await UpdateItemAsync(item);
            }
            else
            {
                await db.Store.InsertAsync(item);
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var _item = await db.Store.Table<DesktopConfigurationModel>().Where((DesktopConfigurationModel arg) => arg.Id == id).FirstOrDefaultAsync();
            await db.Store.DeleteAsync(_item);

            return await Task.FromResult(true);
        }

        public async Task<DesktopConfigurationModel> GetItemAsync(Guid id)
        {
            return await Task.FromResult(
                await db.Store.Table<DesktopConfigurationModel>().Where(s => s.Id == id).FirstOrDefaultAsync()
                );
        }

        public async Task<IEnumerable<DesktopConfigurationModel>> GetItemsAsync(bool forceRefresh = false)
        {
            return await db.Store.Table<DesktopConfigurationModel>().ToListAsync();
        }

        public async Task<bool> UpdateItemAsync(DesktopConfigurationModel item)
        {
            await db.Store.UpdateAsync(item);

            return await Task.FromResult(true);
        }
    }
}
