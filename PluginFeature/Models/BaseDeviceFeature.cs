using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PluginFeature.Models
{
    public abstract class BaseDeviceFeature : PluginFeature.Interfaces.IDeviceFeatureImplementation
    {
        internal string pluginDirectory = null;

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
            return OnIsRegister(deviceId, false);
        }

        private bool OnIsRegister(string deviceId, bool internalCall)
        {
            var result = registeredDevices.Any(x => string.Equals(x.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase));
            if (!internalCall)
            {
                if (!result)
                    System.Diagnostics.Debug.Fail("Device feature received data but is not registered (Service will not response to this request)");
            }
            return result;
        }


        public event EventHandler<PluginFeature.Models.DeviceFeatureData> SendData;

        public void OnSendData(object obj, PluginFeature.Models.DeviceFeatureData args)
        {
            SendData?.Invoke(obj, args);
        }

        public virtual void Dispose()
        {

        }

        public virtual PluginFeature.Models.DeviceFeatureDefinition GetDeviceFeatureInfo()
        {
            return null;
        }

        public virtual void Init(string pluginDirectory)
        {
            this.pluginDirectory = pluginDirectory;
        }

        public virtual void ReceiveData(PluginFeature.Models.DeviceFeatureData data)
        {

        }

        public virtual void Register(string deviceId)
        {
            if (!OnIsRegister(deviceId, true))
                registeredDevices.Add(new RegisteredDevice(deviceId));
            System.Diagnostics.Debug.Print($"Register device (id:{deviceId})");
        }

        public virtual void Unregister(string deviceId)
        {
            if (OnIsRegister(deviceId, true))
            {
                var item = registeredDevices.First(x => string.Equals(x.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase));
                registeredDevices.Remove(item);
            }
            System.Diagnostics.Debug.Print($"Unregister device (id:{deviceId})");
        }

        public virtual ZipArchive OnGetUIZip()
        {
            string archivePath = Path.Combine(pluginDirectory, "ui.zip");
            
            ZipFile.CreateFromDirectory(Path.Combine(pluginDirectory, "UI"), archivePath);
            return ZipFile.OpenRead(archivePath);
        }
    }
}
