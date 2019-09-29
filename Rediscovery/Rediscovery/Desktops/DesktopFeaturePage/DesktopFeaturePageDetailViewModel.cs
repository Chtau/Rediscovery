using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Desktops.DesktopFeaturePage
{
    public class DesktopFeaturePageDetailViewModel
    {
        public Features.Authentication.Models.Connection Connection { get; private set; }
        public Features.Authentication.Models.ConnectionManifestFeature ConnectionManifestFeature { get; private set; }

        public DesktopFeaturePageDetailViewModel(Features.Authentication.Models.Connection connection,
            Features.Authentication.Models.ConnectionManifestFeature connectionManifestFeature)
        {
            Connection = connection;
            ConnectionManifestFeature = connectionManifestFeature;
        }
    }
}
