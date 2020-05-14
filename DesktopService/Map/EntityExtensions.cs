using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Map
{
    public static class EntityExtensions
    {
        public static SharedCoreModels.DeviceInfo ToDeviceInfo(this DALDesktopService.Models.Device device)
        {
            return new SharedCoreModels.DeviceInfo
            {
                Id = device.Id,
                AllowAccess = device.AllowAccess,
                Name = device.DeviceName,
                Identifier = device.DeviceIdentifier,
                DeviceType = device.DeviceType,
                Idiom = device.Idiom,
                Manufacturer = device.Manufacturer,
                Model = device.Model,
                OSVersion = device.OSVersion,
                Platform = device.Platform,
                RequestTime = null
            };
        }

        public static SharedCoreModels.DeviceInfo ToDeviceInfo(this DALDesktopService.Models.DevicePendingAuthentication device)
        {
            return new SharedCoreModels.DeviceInfo
            {
                Id = device.Id,
                AllowAccess = false,
                Name = device.DeviceName,
                Identifier = device.DeviceIdentifier,
                DeviceType = device.DeviceType,
                Idiom = device.Idiom,
                Manufacturer = device.Manufacturer,
                Model = device.Model,
                OSVersion = device.OSVersion,
                Platform = device.Platform,
                RequestTime = device.RequestTime
            };
        }

        public static SharedCoreModels.DeviceFeature ToDeviceFeature(this PluginFeature.Models.DeviceFeatureDefinition deviceFeatureDefinition)
        {
            return new SharedCoreModels.DeviceFeature
            {
                Id = deviceFeatureDefinition.Id,
                DisplayName = deviceFeatureDefinition.DisplayName,
                MinControlIntegrationPoint = deviceFeatureDefinition.MinControlIntegrationPoint.ToString(),
                MinFeatureIntegrationPoint = deviceFeatureDefinition.MinFeatureIntegrationPoint.ToString(),
                Version = deviceFeatureDefinition.Version.ToString()
            };
        }
    }
}
