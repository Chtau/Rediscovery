using CommunicationAuthenticationConsumer;
using CommunicationFeatureConsumer;
using CommunicationHeartbeatConsumer;
using CommunicationLoggerConsumer;
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
        private HeartbeatConsumer heartbeatConsumerService;
        private LoggerConsumer loggerConsumer;

        public Consumer()
        {
            authenticationConsumerService = new AuthenticationConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            greetingConsumerService = new GreetingConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            featureConsumerService = new FeatureConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            heartbeatConsumerService = new HeartbeatConsumer(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            loggerConsumer = new LoggerConsumer();
        }

        public AuthenticationConsumerService AuthenticationConsumerService => authenticationConsumerService;

        public GreetingConsumerService GreetingConsumerService => greetingConsumerService;

        public FeatureConsumerService FeatureConsumerService => featureConsumerService;

        public HeartbeatConsumer HeartbeatConsumerService => heartbeatConsumerService;

        public LoggerConsumer LoggerConsumer => loggerConsumer;

        public bool Disconnect()
        {
            var retVal = true;
            if (AuthenticationConsumerService?.Disconnect() == false)
                retVal = false;
            if (FeatureConsumerService?.Disconnect() == false)
                retVal = false;
            if (GreetingConsumerService?.Disconnect() == false)
                retVal = false;
            if (HeartbeatConsumerService?.Disconnect() == false)
                retVal = false;
            if (LoggerConsumer?.Disconnect() == false)
                retVal = false;
            return retVal;
        }
    }
}
