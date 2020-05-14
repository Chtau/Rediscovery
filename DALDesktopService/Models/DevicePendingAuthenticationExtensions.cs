using System;
using System.Collections.Generic;
using System.Text;

namespace DALDesktopService.Models
{
    public static class DevicePendingAuthenticationExtensions
    {
        public static Device ToNewDevice(this DevicePendingAuthentication pendingDevice, Guid? id = null, bool? allowAccess = null)
        {
            return new Device
            {
                Id = id != null ? id.Value : Guid.NewGuid(),
                AllowAccess = allowAccess != null ? allowAccess.Value : true,
                DeviceIdentifier = pendingDevice.DeviceIdentifier,
                DeviceName = pendingDevice.DeviceName,
                DeviceType = pendingDevice.DeviceType,
                Idiom = pendingDevice.Idiom,
                Manufacturer = pendingDevice.Manufacturer,
                Model = pendingDevice.Model,
                OSVersion = pendingDevice.OSVersion,
                Platform = pendingDevice.Platform
            };
        }
    }
}
