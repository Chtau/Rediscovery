using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.DeviceData))]
namespace Rediscovery.Services
{
    public class DeviceData : IDeviceData
    {
        private IDataStoreGuid<Features.Settings.Models.SettingModel> settingStore => DependencyService.Get<IDataStoreGuid<Features.Settings.Models.SettingModel>>() ?? new Features.Settings.SettingStore();

        public string GetDeviceIdentifier()
        {
            var deviceId = Preferences.Get("rediscovery_identifier", string.Empty);
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = System.Guid.NewGuid().ToString();
                Preferences.Set("rediscovery_identifier", deviceId);
            }
            return deviceId;
        }

        public SharedCoreModels.WelcomeDeviceMessage GetWelcomeDeviceMessage()
        {
            var setting = settingStore.GetItem(Guid.Empty);
            return new WelcomeDeviceMessage
            {
                DeviceName = setting.DeviceIdentifier,
                DeviceIdentifier = GetDeviceIdentifier(),
                DeviceType = Enum.GetName(typeof(DeviceType), Xamarin.Essentials.DeviceInfo.DeviceType),
                Idiom = Xamarin.Essentials.DeviceInfo.Idiom.ToString(),
                Manufacturer = Xamarin.Essentials.DeviceInfo.Manufacturer,
                Model = Xamarin.Essentials.DeviceInfo.Model,
                OSVersion = Xamarin.Essentials.DeviceInfo.VersionString,
                Platform = Xamarin.Essentials.DeviceInfo.Platform.ToString()
            };
        }
    }
}
