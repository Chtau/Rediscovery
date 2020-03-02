using SharedCoreModels.DesktopPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public abstract class BaseDeviceFeature : IDeviceFeatureImplementation
    {
        public class RegisteredDevice
        {
            public string DeviceId { get; set; }

            public DateTime RegisterDate { get; set; }

            public RegisteredDevice(string deviceId)
            {
                DeviceId = deviceId;
                RegisterDate = DateTime.UtcNow;
            }
        }

        internal List<RegisteredDevice> registeredDevices = new List<RegisteredDevice>();

        public IEnumerable<string> RegisteredDevices
        {
            get
            {
                return registeredDevices.Select(x => x.DeviceId);
            }
        }

        public bool IsRegister(string deviceId)
        {
            return registeredDevices.Any(x => string.Equals(x.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase));
        }

        public event EventHandler<DeviceFeatureData> SendData;

        public void OnSendData(object obj, DeviceFeatureData args)
        {
            SendData?.Invoke(obj, args);
        }

        public virtual void Dispose()
        {
            
        }

        public virtual DeviceFeatureDefinition GetDeviceFeatureInfo()
        {
            return null;
        }

        public virtual void Init()
        {
            
        }

        public virtual void ReceiveData(DeviceFeatureData data)
        {
            
        }

        public virtual void Register(string deviceId)
        {
            if (!IsRegister(deviceId))
                registeredDevices.Add(new RegisteredDevice(deviceId));
        }

        public virtual void Unregister(string deviceId)
        {
            if (IsRegister(deviceId))
            {
                var item = registeredDevices.First(x => string.Equals(x.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase));
                registeredDevices.Remove(item);
            }
        }
    }
}
