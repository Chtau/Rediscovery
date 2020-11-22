using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Map
{
    public static class EntityExtensions
    {
        public static SharedBase.Device.DeviceInfo ToDeviceInfo(this DALDesktopService.Models.Device device)
        {
            return new SharedBase.Device.DeviceInfo
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

        public static DALDesktopService.Models.Device ToDevice(this SharedBase.Device.DeviceInfo deviceInfo)
        {
            return new DALDesktopService.Models.Device
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

        public static SharedBase.Device.DeviceInfo ToDeviceInfo(this DALDesktopService.Models.DevicePendingAuthentication device)
        {
            return new SharedBase.Device.DeviceInfo
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
