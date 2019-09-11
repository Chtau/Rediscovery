using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels;

namespace DesktopService.Features.Authentication
{
    public class Manifest : IManifest
    {
        private SharedCoreModels.Manifest manifest;

        public bool BuildManifest()
        {
            manifest = new SharedCoreModels.Manifest
            {
                AppMinimumVersion = new SharedCoreModels.Version() { Major = 0, Minor = 0, Patch = 0, Label = null },
                ClientVersion = new SharedCoreModels.Version() { Major = 0, Minor = 0, Patch = 0, Label = null },
                SupportedFeatures = new List<string>(),
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
