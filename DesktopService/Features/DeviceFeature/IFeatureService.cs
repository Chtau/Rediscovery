using PluginFeature.Interfaces;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace DesktopService.Features.DeviceFeature
{
    public interface IFeatureService
    {
        event EventHandler ProfilesChanged;
        event EventHandler SettingChanged;

        void Load();
        IDeviceFeatureImplementation GetFeature(Guid featureId);
        List<DeviceFeatureDefinition> GetFeaturesManifest();
        string GetFeatureUIArchivePath(Guid featureId);
        string GetFeatureSettingsUIArchivePath(Guid featureId);
        string GetFeatureProfilesUIArchivePath(Guid featureId);
        List<DeviceFeatureProfil> GetFeatureProfiles(Guid featureId);
        DeviceFeatureSetting GetFeatureSettings(Guid featureId);
        bool SaveFeatureSettings(Guid featureId, DeviceFeatureSetting deviceFeatureSetting);
        bool SaveFeatureProfile(Guid featureId, DeviceFeatureProfil deviceFeatureProfil);
        bool DeleteFeatureProfile(Guid featureId, string profileId);
    }
}
