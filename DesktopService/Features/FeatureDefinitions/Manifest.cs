using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.FeatureDefinitions
{
    public class Manifest : IManifest
    {
        private SharedBase.Connection.Manifest manifest;
        private Features.DeviceFeature.IFeatureService _featureService;

        public Manifest(Features.DeviceFeature.IFeatureService featureService)
        {
            _featureService = featureService;
        }

        private bool BuildManifest()
        {
            // TODO: better integration for manifest creation
            manifest = new SharedBase.Connection.Manifest
            {
                AppMinimumVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = null },
                ClientVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = null },
                SupportedFeatures = _featureService.GetFeaturesManifest(),
                ClientName = "DEV-Desktop"
            };
            return true;
        }

        public SharedBase.Connection.Manifest GetManifest()
        {
            if (manifest == null)
                BuildManifest();
            return manifest;
        }
    }
}
