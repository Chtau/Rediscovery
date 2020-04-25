using Rediscovery.Features.Authentication.Models;
using Rediscovery.Features.Connection.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Connection.ConnectionStore))]
namespace Rediscovery.Features.Connection
{
    public class ConnectionStore : BaseService, IDataStoreGuid<Models.ConnectionInfo>
    {
        private IFileSystem fs => DependencyService.Get<IFileSystem>() ?? new FileSystem();
        private IJSONStore json => DependencyService.Get<IJSONStore>() ?? new JSONStore();

        private string filePath()
        {
            return System.IO.Path.Combine(fs.AppSettingsDirectory(), "connection.json");
        }

        public bool AddItem(Models.ConnectionInfo item)
        {
            return AddItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> AddItemAsync(Models.ConnectionInfo item)
        {
            var items = json.GetFileContent<Models.ConnectionInfo[]>(filePath())?.ToList();
            int index = 0;
            if (items == null)
            {
                items = new List<ConnectionInfo>();
            }
            var srcItem = items?.FirstOrDefault(x => x.Id == item.Id);
            if (srcItem == null)
            {
                srcItem = new ConnectionInfo();
                srcItem.Id = Guid.NewGuid();
            } else if (srcItem.Id == Guid.Empty)
            {
                if (item.Id == Guid.Empty)
                    srcItem.Id = Guid.NewGuid();
            }
            index = items.IndexOf(x => x.Id == srcItem.Id);
            if (index == -1)
            {
                items.Add(srcItem);
                index = items.IndexOf(x => x.Id == srcItem.Id);
            }    

            items[index].LastConnection = item.LastConnection;
            items[index].LastKnownAddress = item.LastKnownAddress;
            items[index].ManifestAppMinimumVersion = item.ManifestAppMinimumVersion;
            items[index].ManifestClientName = item.ManifestClientName;
            items[index].ManifestClientVersion = item.ManifestClientVersion;
            items[index].Token = item.Token;
            items[index].User = item.User;
            items[index].Active = item.Active;
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
            var items = json.GetFileContent<Models.ConnectionInfo[]>(filePath())?.ToList();
            if (items?.Any(x => x.Id == id) == true)
            {
                var index = items.IndexOf(x => x.Id == id);
                items.RemoveAt(index);
                var result = json.SetFileContent(items, filePath());
                return await Task.FromResult(result);
            }
            return await Task.FromResult(true);
        }

        public ConnectionInfo GetItem(Guid id)
        {
            return GetItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<Models.ConnectionInfo> GetItemAsync(Guid id)
        {
            return await Task.FromResult(json.GetFileContent<Models.ConnectionInfo[]>(filePath())?.FirstOrDefault(x => x.Id == id));
        }

        public IEnumerable<ConnectionInfo> GetItems(bool forceRefresh = false)
        {
            return GetItemsAsync(forceRefresh).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<Models.ConnectionInfo>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(json.GetFileContent<Models.ConnectionInfo[]>(filePath())?.ToList());
        }

        public bool UpdateItem(ConnectionInfo item)
        {
            return UpdateItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> UpdateItemAsync(Models.ConnectionInfo item)
        {
            return await AddItemAsync(item);
        }
    }
}
