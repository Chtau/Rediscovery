using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.FeatureView
{
    public class FeatureViewViewModel : BaseViewModel
    {
        private DesktopFeatures.IFeatureUIService featureUIService => DependencyService.Get<DesktopFeatures.IFeatureUIService>() ?? new DesktopFeatures.FeatureUIService();

        public event EventHandler<Tuple<Guid, string>> UIDataReady;

        public readonly Features.Connection.Models.ConnectionManifestFeature ConnectionManifestFeature;

        public FeatureViewViewModel(Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature)
        {
            ConnectionManifestFeature = connectionManifestFeature;
            featureUIService.SaveUI(ConnectionManifestFeature.FeatureId, (state, directory) =>
            {
                if (state)
                {
                    UIDataReady?.Invoke(this, new Tuple<Guid, string>(ConnectionManifestFeature.FeatureId, directory));
                }
            });
        }
    }
}
