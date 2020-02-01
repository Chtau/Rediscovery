using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage
{
    public class BaseFeatureViewModel : BaseViewModel
    {
        private IFeatureExchange featureExchange => DependencyService.Get<IFeatureExchange>() ?? new FeatureExchange();

        internal readonly Authentication.Models.ConnectionManifestFeature _connectionManifestFeature;
        internal ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();

        public BaseFeatureViewModel(Authentication.Models.ConnectionManifestFeature connectionManifestFeature)
        {
            _connectionManifestFeature = connectionManifestFeature;
            featureExchange.DesktopResponseReceived += FeatureExchange_DesktopResponseReceived;
        }

        private void FeatureExchange_DesktopResponseReceived(object sender, (Guid connectionId, Guid featureId, object data) e)
        {
            if (_connectionManifestFeature.ConnectionId == e.connectionId && _connectionManifestFeature.FeatureId == e.featureId)
            {
                Receive(e.data);
            }
        }

        internal virtual void Send(object data)
        {
            featureExchange.Send(_connectionManifestFeature, data);
        }

        internal virtual void Receive(object data)
        {

        }
    }
}
