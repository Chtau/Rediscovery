using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface IConsumer
    {
        CommunicationAuthenticationConsumer.AuthenticationConsumerService AuthenticationConsumerService { get; }
        CommunicationAuthenticationConsumer.GreetingConsumerService GreetingConsumerService { get; }
        CommunicationFeatureConsumer.FeatureConsumerService FeatureConsumerService { get; }
        CommunicationHeartbeatConsumer.HeartbeatConsumer HeartbeatConsumerService { get; }
        bool Disconnect();
    }
}
