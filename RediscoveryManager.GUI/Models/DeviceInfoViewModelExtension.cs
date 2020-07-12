using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.GUI.Models
{
    public class DeviceInfoViewModelExtension : SharedBase.Device.DeviceInfo
    {
        public Action<DeviceInfoViewModelExtension> DeleteCallback { get; set; }

        public DeviceInfoViewModelExtension(SharedBase.Device.DeviceInfo oldBase, Action<DeviceInfoViewModelExtension> deleteCallback)
        {
            base.AllowAccess = oldBase.AllowAccess;
            base.DeviceType = oldBase.DeviceType;
            base.Id = oldBase.Id;
            base.Identifier = oldBase.Identifier;
            base.Idiom = oldBase.Idiom;
            base.Manufacturer = oldBase.Manufacturer;
            base.Model = oldBase.Model;
            base.Name = oldBase.Name;
            base.OSVersion = oldBase.OSVersion;
            base.Platform = oldBase.Platform;
            base.RequestTime = oldBase.RequestTime;

            DeleteCallback = deleteCallback;
        }

        public void DeleteDevice()
        {
            DeleteCallback?.Invoke(this);
        }
    }
}
