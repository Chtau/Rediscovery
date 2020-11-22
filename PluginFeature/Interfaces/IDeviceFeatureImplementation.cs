using Rediscovery.Feature.Plugin.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace Rediscovery.Feature.Plugin.Interfaces
{
    public interface IDeviceFeatureImplementation : IBaseFeatureImplementation<PluginFeatureData, PluginFeatureDefinition>
    {
        string GetUIArchivePath();
        void OpenSettingConfiguration();
        void OpenProfileConfiguration();
        PluginFeatureSetting GetSettingsObject();

        List<PluginFeatureProfil> GetProfiles();
    }
}
