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
        public event EventHandler<object> ReceivedData;

        private IFeatureExchange featureExchange => DependencyService.Get<IFeatureExchange>() ?? new FeatureExchange();

        internal readonly Features.Connection.Models.ConnectionManifestFeature _connectionManifestFeature;
        internal ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();

        public string FeatureVersion => _connectionManifestFeature.FeatureVersion;

        public BaseFeatureViewModel(Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature)
        {
            _connectionManifestFeature = connectionManifestFeature;
            featureExchange.DesktopResponseReceived += FeatureExchange_DesktopResponseReceived;
        }

        private void FeatureExchange_DesktopResponseReceived(object sender, (Guid connectionId, Guid featureId, string profileId, object data) e)
        {
            if (_connectionManifestFeature.ConnectionId == e.connectionId && _connectionManifestFeature.FeatureId == e.featureId)
            {
                Receive(e.data);
                ReceivedData?.Invoke(sender, e.data);
            }
        }

        public virtual void Send(string profileId, object data)
        {
            featureExchange.Send(_connectionManifestFeature, profileId, data);
        }

        public virtual void Receive(object data)
        {

        }

        public virtual void Start()
        {
            featureExchange.Start(_connectionManifestFeature);
        }

        public virtual void Stop()
        {
            featureExchange.Stop(_connectionManifestFeature);
        }
    }
}
