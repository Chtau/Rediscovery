using CommunicationBase.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommunicationFeatureConsumer
{
    public interface IFeatureConsumerService
    {
        bool Connect(string ipAddress, int port, string certificatePEM);

        event EventHandler<FeatureState> ReceiveFeatureStateChangeReply;
        void ChangeFeatureState(string token, FeatureState featureState);

        event EventHandler<PluginFeature.Models.PluginFeatureData> ReceiveFeatureData;
        void StartFeatureData(string token, CancellationTokenSource cts = null);
        void SendFeatureData(PluginFeature.Models.PluginFeatureData deviceFeatureData);
        event EventHandler<Models.FeatureClientData> ReceiveClientData;
        void FeatureClient(string token, Guid featureId);
    }
}
