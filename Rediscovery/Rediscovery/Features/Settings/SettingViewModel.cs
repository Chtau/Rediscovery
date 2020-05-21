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
using Xamarin.Essentials;

namespace Rediscovery.Features.Settings
{
    public class SettingViewModel : BaseViewModel
    {
        private IDeviceData deviceData => DependencyService.Get<IDeviceData>() ?? new DeviceData();
        private IDataStoreGuid<SettingModel> Store => DependencyService.Get<IDataStoreGuid<SettingModel>>() ?? new SettingStore();

        Models.SettingModel setting = new SettingModel();
        public Models.SettingModel Setting
        {
            get { return setting; }
            set { SetProperty(ref setting, value); }
        }

        string applicationDeviceIdentifier;
        public string ApplicationDeviceIdentifier
        {
            get { return applicationDeviceIdentifier; }
            set { SetProperty(ref applicationDeviceIdentifier, value); }
        }

        public LoadBinding Load { get; set; }
        public Command LoadCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command NativeSettingsUICommand { get; set; }
        public Command GenerateNewApplicationDeviceIdentifierCommand { get; set; }

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
            NativeSettingsUICommand = new Command(() =>
            {
                AppInfo.ShowSettingsUI();
            });
            GenerateNewApplicationDeviceIdentifierCommand = new Command(() =>
            {
                ApplicationDeviceIdentifier = deviceData.GenerateNewDeviceIDentifier();
            });
        }

        private async Task OnLoad()
        {
            ApplicationDeviceIdentifier = deviceData.GetDeviceIdentifier();
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
                _userNotification.ShowToast("Settings saved");
            }
        }
    }
}
