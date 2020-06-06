using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureConsumer
{
    public interface IFeatureConsumerService
    {
        event EventHandler<CommunicationBase.Models.FeatureState> ReceiveFeatureStateChangeReply;
        void ChangeFeatureState(CommunicationBase.Models.FeatureState featureState);
    }
}
