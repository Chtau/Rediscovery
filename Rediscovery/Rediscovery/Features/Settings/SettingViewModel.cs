using Rediscovery.Features.Settings.Models;
using Rediscovery.Models;
using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Security.Cryptography;

namespace Rediscovery.Features.Settings
{
    public class SettingViewModel : BaseViewModel
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDataStoreGuid<SettingModel> Store => DependencyService.Get<IDataStoreGuid<SettingModel>>() ?? new SettingStore();

        public Models.SettingModel Setting { get; set; } = new SettingModel();
        public LoadBinding Load { get; set; }
        public Command LoadCommand { get; set; }
        public Command SaveCommand { get; set; }

        public SettingViewModel()
        {
            Load = new LoadBinding
            {
                IsLoading = false
            };
            LoadCommand = new Command(async () =>
            {
                await OnLoad();
            });
            SaveCommand = new Command(async () =>
            {
                await OnSave();
            });
        }

        private async Task OnLoad()
        {
            var item = (await Store.GetItemsAsync()).FirstOrDefault();
            if (item != null)
            {
                Setting = item;
            }
        }

        private async Task OnSave()
        {
            if (Setting != null)
            {
                await Store.AddItemAsync(Setting);
            }
        }
    }
}
