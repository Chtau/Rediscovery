using Rediscovery.Features.Settings.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Settings.SettingStore))]
namespace Rediscovery.Features.Settings
{
    public class SettingStore : BaseService, IDataStoreGuid<SettingModel>
    {
        private IFileSystem fs => DependencyService.Get<IFileSystem>() ?? new FileSystem();

        public string FilePath
        {
            get
            {
                return System.IO.Path.Combine(fs.AppSettingsDirectory(), "appsettings.json");
            }
        }

        private SettingModel OnGetFileContent()
        {
            try
            {
                var content = System.IO.File.ReadAllText(FilePath);
                if (!string.IsNullOrWhiteSpace(content))
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<SettingModel>(content);
                return new SettingModel();
            } catch (Exception ex)
            {
                _logger.Error(ex);
                return new SettingModel();
            }
        }

        private bool OnSetFileContent(SettingModel value)
        {
            try
            {
                var content = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                System.IO.File.WriteAllText(FilePath, content);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public bool AddItem(SettingModel item)
        {
            return AddItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> AddItemAsync(SettingModel item)
        {
            var srcItem = OnGetFileContent();
            srcItem.DeviceIdentifier = item.DeviceIdentifier;
            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();
            srcItem.Id = item.Id;
            OnSetFileContent(srcItem);
            return await Task.FromResult(true);
        }

        public bool DeleteItem(Guid id)
        {
            return DeleteItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var _item = await db.Store.Table<SettingModel>().Where((SettingModel arg) => arg.Id == id).FirstOrDefaultAsync();
            await db.Store.DeleteAsync(_item);

            return await Task.FromResult(true);
        }

        public SettingModel GetItem(Guid id)
        {
            return GetItemAsync(id).GetAwaiter().GetResult();
        }

        public async Task<SettingModel> GetItemAsync(Guid id)
        {
            return await Task.FromResult(
                await db.Store.Table<SettingModel>().Where(s => s.Id == id).FirstOrDefaultAsync()
                );
        }

        public IEnumerable<SettingModel> GetItems(bool forceRefresh = false)
        {
            return GetItemsAsync(forceRefresh).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<SettingModel>> GetItemsAsync(bool forceRefresh = false)
        {
            return await db.Store.Table<SettingModel>().ToListAsync();
        }

        public bool UpdateItem(SettingModel item)
        {
            return UpdateItemAsync(item).GetAwaiter().GetResult();
        }

        public async Task<bool> UpdateItemAsync(SettingModel item)
        {
            await db.Store.UpdateAsync(item);

            return await Task.FromResult(true);
        }
    }
}
