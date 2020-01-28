using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.TerminalPage
{
    public class DesktopFeaturePageDetailViewModel : BaseViewModel
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
