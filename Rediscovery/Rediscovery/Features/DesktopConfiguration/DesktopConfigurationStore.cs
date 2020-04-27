using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopConfiguration.DesktopConfigurationStore))]
namespace Rediscovery.Features.DesktopConfiguration
{
    public class DesktopConfigurationStore : BaseService, IDataStoreGuid<DesktopConfigurationModel>
    {
        private IFileSystem fs => DependencyService.Get<IFileSystem>() ?? new FileSystem();
        private IJSONStore json => DependencyService.Get<IJSONStore>() ?? new JSONStore();

        private string filePath()
        {
            return System.IO.Path.Combine(fs.AppSettingsDirectory(), "desktopconfiguration.json");
        }

        public bool AddItem(DesktopConfigurationModel item)
        {
            return AddItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> AddItemAsync(DesktopConfigurationModel item)
        {
            var items = json.GetFileContent<DesktopConfigurationModel[]>(filePath())?.ToList();
            int index = -1;
            if (items == null)
            {
                items = new List<DesktopConfigurationModel>();
            }
            var srcItem = items?.FirstOrDefault(x => x.Id == item.Id);
            if (srcItem == null)
            {
                srcItem = new DesktopConfigurationModel();
                srcItem.Id = Guid.NewGuid();
            }
            else if (srcItem.Id == Guid.Empty)
            {
                if (item.Id == Guid.Empty)
                    srcItem.Id = Guid.NewGuid();
            }
            index = items.FindIndex(x => x.Id == srcItem.Id);
            if (index == -1)
            {
                items.Add(srcItem);
                index = items.FindIndex(x => x.Id == srcItem.Id);
            }

            items[index].LastConnection = item.LastConnection;
            items[index].LastKnownAddress = item.LastKnownAddress;
            items[index].ManifestAppMinimumVersion = item.ManifestAppMinimumVersion;
            items[index].ManifestClientName = item.ManifestClientName;
            items[index].ManifestClientVersion = item.ManifestClientVersion;
            items[index].Token = item.Token;
            items[index].AutoConnect = item.AutoConnect;
            items[index].ConnectionState = item.ConnectionState;
            items[index].DisplayName = item.DisplayName;
            var result = json.SetFileContent(items, filePath());
            return await Task.FromResult(result);
        }

        public bool DeleteItem(Guid id)
        {
            return DeleteItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var items = json.GetFileContent<DesktopConfigurationModel[]>(filePath())?.ToList();
            if (items?.Any(x => x.Id == id) == true)
            {
                var index = items.FindIndex(x => x.Id == id);
                items.RemoveAt(index);
                var result = json.SetFileContent(items, filePath());
                return await Task.FromResult(result);
            }
            return await Task.FromResult(true);
        }

        public DesktopConfigurationModel GetItem(Guid id)
        {
            return GetItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<DesktopConfigurationModel> GetItemAsync(Guid id)
        {
            return await Task.FromResult(json.GetFileContent<DesktopConfigurationModel[]>(filePath())?.FirstOrDefault(x => x.Id == id));
        }

        public IEnumerable<DesktopConfigurationModel> GetItems(bool forceRefresh = false)
        {
            return GetItemsAsync(forceRefresh).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<DesktopConfigurationModel>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(json.GetFileContent<DesktopConfigurationModel[]>(filePath())?.ToList());
        }

        public bool UpdateItem(DesktopConfigurationModel item)
        {
            return UpdateItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> UpdateItemAsync(DesktopConfigurationModel item)
        {
            return await AddItemAsync(item);
        }
    }
}
