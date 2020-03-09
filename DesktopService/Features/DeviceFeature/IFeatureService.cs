using PluginFeature.Interfaces;
using PluginFeature.Models;
using SharedCoreModels.DeviceFeature;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace DesktopService.Features.DeviceFeature
{
    public interface IFeatureService
    {
        void Load();
        IDeviceFeatureImplementation GetFeature(Guid featureId);
        List<DeviceFeatureDefinition> GetFeaturesManifest();
        string GetFeatureUIArchivePath(Guid featureId);
    }
}
