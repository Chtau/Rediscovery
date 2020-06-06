using CommunicationBase.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommunicationFeatureConsumer
{
    public interface IFeatureConsumerService
    {
        event EventHandler<FeatureState> ReceiveFeatureStateChangeReply;
        void ChangeFeatureState(string token, FeatureState featureState);

        event EventHandler<PluginFeature.Models.DeviceFeatureData> ReceiveFeatureData;
        void StartFeatureData(string token, CancellationTokenSource cancellationTokenSource = null);
        void SendFeatureData(PluginFeature.Models.DeviceFeatureData deviceFeatureData);
    }
}
