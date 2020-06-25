using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace PluginFeature.Interfaces
{
    public interface IDeviceFeatureImplementation
    {
        string PluginDirectory { get; }
        void Init(string pluginDirectory, IPluginLogger pluginLogger);
        void Dispose();
        event EventHandler<PluginExchangeEntity<PluginFeatureData>> SendData;
        void ReceiveData(PluginExchangeEntity<PluginFeatureData> data);
        PluginFeatureDefinition GetDeviceFeatureInfo();

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
        void OpenSettingConfiguration();
        void OpenProfileConfiguration();
        PluginFeatureSetting GetSettingsObject();

        List<PluginFeatureProfil> GetProfiles();
    }
}
