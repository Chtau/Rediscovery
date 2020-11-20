using Rediscovery.Features.Settings.Models;
using Rediscovery.Features.Storage;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Settings.SettingStore))]
namespace Rediscovery.Features.Settings
{
    public class SettingStore : BaseService, IDataStoreGuid<SettingModel>
    {
        private IFileSystem fs => DependencyService.Get<IFileSystem>() ?? new FileSystem();
        private IJSONStore json => DependencyService.Get<IJSONStore>() ?? new JSONStore();

        private string filePath()
        {
            return System.IO.Path.Combine(fs.AppSettingsDirectory(), "appsettings.json");
        }

        public Tuple<bool, SettingModel> AddItem(SettingModel item)
        {
            return AddItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<Tuple<bool, SettingModel>> AddItemAsync(SettingModel item)
        {
            var srcItem = json.GetFileContent<SettingModel>(filePath());
            if (srcItem == null)
                srcItem = new SettingModel();
            srcItem.DeviceName = item.DeviceName;
            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();
            srcItem.ConnectTimeout = item.ConnectTimeout;
            srcItem.Id = item.Id;
            var result = json.SetFileContent(srcItem, filePath());
            return await Task.FromResult(new Tuple<bool, SettingModel>(result, srcItem));
        }

        public bool DeleteItem(Guid id)
        {
            return DeleteItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            return await Task.FromResult(json.DeleteFile(filePath()));
        }

        public SettingModel GetItem(Guid id)
        {
            return GetItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<SettingModel> GetItemAsync(Guid id)
        {
            var item = await Task.FromResult(json.GetFileContent<SettingModel>(filePath()));
            if (item == null)
                return new SettingModel();
            return item;
        }

        public IEnumerable<SettingModel> GetItems(bool forceRefresh = false)
        {
            return GetItemsAsync(forceRefresh).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<SettingModel>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(new List<SettingModel> { json.GetFileContent<SettingModel>(filePath()) });
        }

        public Tuple<bool, SettingModel> UpdateItem(SettingModel item)
        {
            return UpdateItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<Tuple<bool, SettingModel>> UpdateItemAsync(SettingModel item)
        {
            var srcItem = json.GetFileContent<SettingModel>(filePath());
            srcItem.DeviceName = item.DeviceName;
            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();
            srcItem.ConnectTimeout = item.ConnectTimeout;
            srcItem.Id = item.Id;
            var result = json.SetFileContent(srcItem, filePath());
            return await Task.FromResult(new Tuple<bool, SettingModel>(result, srcItem));
        }
    }
}
