using PluginFeature.Interfaces;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PluginFeature
{
    public abstract class BaseDeviceFeature : IDeviceFeatureImplementation
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


        public event EventHandler<PluginExchangeEntity<PluginFeatureData>> SendData;

        public void OnSendData(object obj, PluginExchangeEntity<PluginFeatureData> args)
        {
            SendData?.Invoke(obj, args);
        }

        public virtual void Dispose()
        {

        }

        public virtual PluginFeatureDefinition GetDeviceFeatureInfo()
        {
            return null;
        }

        public virtual void Init(string pluginDirectory, IPluginLogger pluginLogger)
        {
            this.pluginDirectory = pluginDirectory;
            this.pluginLogger = pluginLogger;
        }

        public virtual void ReceiveData(PluginExchangeEntity<PluginFeatureData> data)
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

        public virtual string OnGetUIZipPath(string zipFileName, string subDirectory)
        {
            string archivePath = Path.Combine(pluginDirectory, zipFileName);
            string uiDirectory = Path.Combine(pluginDirectory, subDirectory);
            if (System.IO.Directory.Exists(uiDirectory))
            {
                if (File.Exists(archivePath))
                    File.Delete(archivePath);
                ZipFile.CreateFromDirectory(uiDirectory, archivePath);
                return archivePath;
            }
            return null;
        }

        public string GetUIArchivePath()
        {
            return OnGetUIZipPath("ui.zip", "UI");
        }

        public virtual PluginFeatureSetting GetSettingsObject()
        {
            return null;
        }

        public virtual List<PluginFeatureProfil> GetProfiles()
        {
            return null;
        }

        public string GetSettingsUIArchivePath()
        {
            return OnGetUIZipPath("settingui.zip", "SettingUI");
        }

        public string GetProfilesUIArchivePath()
        {
            return OnGetUIZipPath("profileui.zip", "ProfileUI");
        }

        public virtual bool SaveSetting(PluginFeatureSetting deviceFeatureSetting)
        {
            return false;
        }

        public virtual bool SaveProfile(PluginFeatureProfil deviceFeatureProfil)
        {
            return false;
        }

        public virtual bool DeleteProfile(string profileId)
        {
            return false;
        }
    }
}
