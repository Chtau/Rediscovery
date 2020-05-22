using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace PluginFeature.Interfaces
{
    public interface IDeviceFeatureImplementation
    {
        void Init(string pluginDirectory, IPluginLogger pluginLogger);
        void Dispose();
        event EventHandler<DeviceFeatureData> SendData;
        void ReceiveData(DeviceFeatureData data);
        DeviceFeatureDefinition GetDeviceFeatureInfo();

        /// <summary>
        /// Register a device id to send/receive data from the feature
        /// </summary>
        /// <param name="deviceId">unique user connection id</param>
        void Register(string deviceId);

        /// <summary>
        /// Unregister from sending/receiving data from the feature
        /// </summary>
        /// <param name="deviceId">unique user connection id</param>
        void Unregister(string deviceId);

        string GetUIArchivePath();
        string GetSettingsUIArchivePath();
        string GetProfilesUIArchivePath();

        DeviceFeatureSetting GetSettingsObject();

        List<DeviceFeatureProfil> GetProfiles();
        bool SaveSetting(DeviceFeatureSetting deviceFeatureSetting);
        bool SaveProfile(DeviceFeatureProfil deviceFeatureProfil);
        bool DeleteProfile(string profileId);
    }
}
