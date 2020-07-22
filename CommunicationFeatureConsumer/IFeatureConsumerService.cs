using CommunicationBase;
using CommunicationBase.Models;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommunicationFeatureConsumer
{
    public interface IFeatureConsumerService
    {
        bool Connect(ConsumerConnectionConfiguration connectionConfiguration);
        bool Disconnect();

        event EventHandler<FeatureState> ReceiveFeatureStateChangeReply;
        void ChangeFeatureState(string token, FeatureState featureState);

        event EventHandler<FeatureData> ReceiveFeatureData;
        void StartFeatureData(string token, CancellationTokenSource cts = null);
        void SendFeatureData(FeatureData deviceFeatureData);
        event EventHandler<Models.FeatureClientData> ReceiveClientData;
        void FeatureClient(string token, Guid featureId);
    }
}
