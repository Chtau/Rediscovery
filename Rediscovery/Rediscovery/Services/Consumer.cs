using CommunicationAuthenticationConsumer;
using CommunicationFeatureConsumer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public class Consumer : IConsumer
    {
        private AuthenticationConsumerService authenticationConsumerService;
        private GreetingConsumerService greetingConsumerService;
        private FeatureConsumerService featureConsumerService;

        public Consumer()
        {
            authenticationConsumerService = new AuthenticationConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            greetingConsumerService = new GreetingConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            featureConsumerService = new FeatureConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
        }

        public AuthenticationConsumerService AuthenticationConsumerService => authenticationConsumerService;

        public GreetingConsumerService GreetingConsumerService => greetingConsumerService;

        public FeatureConsumerService FeatureConsumerService => featureConsumerService;
    }
}
