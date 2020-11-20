using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Storage.DeviceData))]
namespace Rediscovery.Features.Storage
{
    public class DeviceData : IDeviceData
    {
        const string ApplicationDeviceIdentifier = "rediscovery_identifier";

        private IDataStoreGuid<Features.Settings.Models.SettingModel> settingStore => DependencyService.Get<IDataStoreGuid<Features.Settings.Models.SettingModel>>() ?? new Features.Settings.SettingStore();

        public string GenerateNewDeviceIDentifier()
        {
            Preferences.Remove(ApplicationDeviceIdentifier);
            return GetDeviceIdentifier();
        }

        public string GetDeviceIdentifier()
        {
            var deviceId = Preferences.Get(ApplicationDeviceIdentifier, string.Empty);
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = System.Guid.NewGuid().ToString();
                Preferences.Set(ApplicationDeviceIdentifier, deviceId);
            }
            return deviceId;
        }

        public SharedBase.Connection.GreetingDeviceMessage GreetingDeviceMessage()
        {
            var setting = settingStore.GetItem(Guid.Empty);
            return new SharedBase.Connection.GreetingDeviceMessage
            {
                DeviceName = setting.DeviceName,
                DeviceIdentifier = GetDeviceIdentifier(),
                DeviceType = Enum.GetName(typeof(DeviceType), Xamarin.Essentials.DeviceInfo.DeviceType),
                Idiom = Xamarin.Essentials.DeviceInfo.Idiom.ToString(),
                Manufacturer = Xamarin.Essentials.DeviceInfo.Manufacturer,
                Model = Xamarin.Essentials.DeviceInfo.Model,
                OSVersion = Xamarin.Essentials.DeviceInfo.VersionString,
                Platform = Xamarin.Essentials.DeviceInfo.Platform.ToString()
            };
        }

        public SharedBase.Connection.WelcomeDeviceMessage WelcomeDeviceMessage()
        {
            return new SharedBase.Connection.WelcomeDeviceMessage
            {
                DeviceIdentifier = GetDeviceIdentifier()
            };
        }
    }
}
