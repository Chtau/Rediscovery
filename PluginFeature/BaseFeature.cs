using Rediscovery.Feature.Plugin.Interfaces;
using Rediscovery.Feature.Plugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Feature.Plugin
{
    public class BaseFeature<TEntity, TDefinition> : IBaseFeatureImplementation<TEntity, TDefinition>
    {
        internal string pluginDirectory = null;
        public IPluginLogger pluginLogger = null;

        public string PluginDirectory => pluginDirectory;

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
                {
                    string message = "Device feature received data but is not registered (Service will not response to this request)";
                    pluginLogger?.LogCritical(message);
                    System.Diagnostics.Debug.Fail(message);
                }
            }
            return result;
        }

        public event EventHandler<PluginExchangeEntity<TEntity>> SendData;

        public void OnSendData(object obj, PluginExchangeEntity<TEntity> args)
        {
            SendData?.Invoke(obj, args);
        }

        public virtual void Dispose()
        {
            
        }

        public virtual TDefinition GetDeviceFeatureInfo()
        {
            return default;
        }

        public virtual void Init(string pluginDirectory, IPluginLogger pluginLogger)
        {
            this.pluginDirectory = pluginDirectory;
            this.pluginLogger = pluginLogger;
        }

        public virtual void ReceiveData(PluginExchangeEntity<TEntity> data)
        {
            
        }

        public virtual void Register(string deviceId)
        {
            if (!OnIsRegister(deviceId, true))
                registeredDevices.Add(new RegisteredDevice(deviceId));
            pluginLogger?.LogInformation($"Register device (id:{deviceId})");
        }

        public virtual void Unregister(string deviceId)
        {
            if (OnIsRegister(deviceId, true))
            {
                var item = registeredDevices.First(x => string.Equals(x.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase));
                registeredDevices.Remove(item);
            }
            pluginLogger?.LogInformation($"Unregister device (id:{deviceId})");
        }
    }
}
