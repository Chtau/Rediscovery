using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Map
{
    public static class EntityExtensions
    {
        public static Rediscovery.Shared.Base.Device.DeviceInfo ToDeviceInfo(this Rediscovery.Service.DAL.Models.Device device)
        {
            return new Rediscovery.Shared.Base.Device.DeviceInfo
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

        public static Rediscovery.Service.DAL.Models.Device ToDevice(this Rediscovery.Shared.Base.Device.DeviceInfo deviceInfo)
        {
            return new Rediscovery.Service.DAL.Models.Device
            {
                Id = deviceInfo.Id,
                AllowAccess = deviceInfo.AllowAccess,
                DeviceName = deviceInfo.Name,
                DeviceIdentifier = deviceInfo.Identifier,
                DeviceType = deviceInfo.DeviceType,
                Idiom = deviceInfo.Idiom,
                Manufacturer = deviceInfo.Manufacturer,
                Model = deviceInfo.Model,
                OSVersion = deviceInfo.OSVersion,
                Platform = deviceInfo.Platform
            };
        }

        public static Rediscovery.Shared.Base.Device.DeviceInfo ToDeviceInfo(this Rediscovery.Service.DAL.Models.DevicePendingAuthentication device)
        {
            return new Rediscovery.Shared.Base.Device.DeviceInfo
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
    }
}
