using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace PluginFeature.Interfaces
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
