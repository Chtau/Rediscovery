using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels;

namespace DesktopService.Features.FeatureDefinitions
{
    public class Manifest : IManifest
    {
        private SharedCoreModels.Manifest manifest;
        private Features.DeviceFeature.IFeatureService _featureService;

        public Manifest(Features.DeviceFeature.IFeatureService featureService)
        {
            _featureService = featureService;
        }

        private bool BuildManifest()
        {
            manifest = new SharedCoreModels.Manifest
            {
                AppMinimumVersion = new PluginFeature.Models.Version() { Major = 0, Minor = 0, Patch = 0, Label = null },
                ClientVersion = new PluginFeature.Models.Version() { Major = 0, Minor = 0, Patch = 0, Label = null },
                SupportedFeatures = _featureService.GetFeaturesManifest(),
                ClientName = "DEV-Desktop"
            };
            return true;
        }

        public SharedCoreModels.Manifest GetManifest()
        {
            if (manifest == null)
                BuildManifest();
            return manifest;
        }
    }
}
