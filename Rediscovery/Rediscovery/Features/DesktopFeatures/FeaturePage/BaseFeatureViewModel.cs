using CommunicationClientConsumer;
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
        public event EventHandler<Tuple<string, object>> ReceivedData;

        private IHub communicationHub => DependencyService.Get<IHub>() ?? new Hub();

        internal readonly Features.Connection.Models.ConnectionManifestFeature _connectionManifestFeature;

        public string FeatureVersion => _connectionManifestFeature.FeatureVersion;

        public BaseFeatureViewModel(Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature)
        {
            _connectionManifestFeature = connectionManifestFeature;
            communicationHub.FeatureResponseReceived += CommunicationHub_FeatureResponseReceived;
        }

        private void CommunicationHub_FeatureResponseReceived(object sender, CommunicationClientConsumer.Models.ResponseReceived e)
        {
            _logger.Message($"{DateTime.Now.ToShortTimeString()} Feature exchange received. (ConfigurationId:{e.ConfigurationId} FeatureId:{e.FeatureId} ProfileId:{e.ProfileId})");
            if (_connectionManifestFeature.ConfigurationId == e.ConfigurationId && _connectionManifestFeature.FeatureId == e.FeatureId)
            {
                Receive(e.Data);
                ReceivedData?.Invoke(sender, new Tuple<string, object>(e.ProfileId, e.Data));
            }
        }

        public virtual void Send(string profileId, object data)
        {
            _logger.Message($"{DateTime.Now.ToShortTimeString()} Try to send from Feature. (profileId:{profileId} data:{data})");
            communicationHub.Send(_connectionManifestFeature.FeatureId, profileId, data);
        }

        public virtual void Receive(object data)
        {

        }

        public virtual void Start()
        {
            communicationHub.Start(_connectionManifestFeature.FeatureId);
        }

        public virtual void Stop()
        {
            communicationHub.Stop(_connectionManifestFeature.FeatureId);
        }
    }
}
