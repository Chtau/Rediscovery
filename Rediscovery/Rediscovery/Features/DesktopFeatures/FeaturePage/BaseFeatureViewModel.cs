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

        //private IHub communicationHub => DependencyService.Get<IHub>() ?? new Hub();
        private CommunicationFeatureConsumer.IFeatureConsumerService featureConsumer => DependencyService.Get<CommunicationFeatureConsumer.IFeatureConsumerService>();

        internal readonly Features.Connection.Models.ConnectionManifestFeature _connectionManifestFeature;

        public string FeatureVersion => _connectionManifestFeature.FeatureVersion;

        public BaseFeatureViewModel(Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature)
        {
            _connectionManifestFeature = connectionManifestFeature;
            //communicationHub.FeatureResponseReceived += CommunicationHub_FeatureResponseReceived;
            featureConsumer.ReceiveClientData += FeatureConsumer_ReceiveClientData;
            featureConsumer.ReceiveFeatureData += FeatureConsumer_ReceiveFeatureData;
            featureConsumer.ReceiveFeatureStateChangeReply += FeatureConsumer_ReceiveFeatureStateChangeReply;
        }

        private void FeatureConsumer_ReceiveFeatureStateChangeReply(object sender, CommunicationBase.Models.FeatureState e)
        {
            //throw new NotImplementedException();
        }

        private void FeatureConsumer_ReceiveFeatureData(object sender, SharedBase.Feature.FeatureData e)
        {
            _logger.LogTrace($"{DateTime.Now.ToShortTimeString()} Feature exchange received. (FeatureId:{e.FeatureId} ProfileId:{e.ProfileId})");
            if (_connectionManifestFeature.FeatureId == e.FeatureId)
            {
                Receive(e.Data);
                ReceivedData?.Invoke(sender, new Tuple<string, object>(e.ProfileId, e.Data));
            }
        }

        private void FeatureConsumer_ReceiveClientData(object sender, CommunicationFeatureConsumer.Models.FeatureClientData e)
        {
            throw new NotImplementedException();
        }

        /*private void CommunicationHub_FeatureResponseReceived(object sender, CommunicationClientConsumer.Models.ResponseReceived e)
        {
            _logger.LogTrace($"{DateTime.Now.ToShortTimeString()} Feature exchange received. (ConfigurationId:{e.ConfigurationId} FeatureId:{e.FeatureId} ProfileId:{e.ProfileId})");
            if (_connectionManifestFeature.ConfigurationId == e.ConfigurationId && _connectionManifestFeature.FeatureId == e.FeatureId)
            {
                Receive(e.Data);
                ReceivedData?.Invoke(sender, new Tuple<string, object>(e.ProfileId, e.Data));
            }
        }*/

        public virtual void Send(string profileId, string data)
        {
            _logger.LogTrace($"{DateTime.Now.ToShortTimeString()} Try to send from Feature. (profileId:{profileId} data:{data})");
            //communicationHub.Send(_connectionManifestFeature.FeatureId, profileId, data);
            featureConsumer.SendFeatureData(new SharedBase.Feature.FeatureData("", _connectionManifestFeature.FeatureId, profileId, data));
        }

        public virtual void Receive(object data)
        {

        }

        public virtual void Start()
        {
            
            //communicationHub.Start(_connectionManifestFeature.FeatureId);
            featureConsumer.ChangeFeatureState("", new CommunicationBase.Models.FeatureState
            {
                FeatureId = _connectionManifestFeature.FeatureId,
                CurrentState = CommunicationBase.Models.FeatureState.State.Start
            });
        }

        public virtual void Stop()
        {
            //communicationHub.Stop(_connectionManifestFeature.FeatureId);
            featureConsumer.ChangeFeatureState("", new CommunicationBase.Models.FeatureState
            {
                FeatureId = _connectionManifestFeature.FeatureId,
                CurrentState = CommunicationBase.Models.FeatureState.State.Stop
            });
        }
    }
}
