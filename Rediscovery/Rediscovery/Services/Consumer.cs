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
        public Consumer()
        {
            SharedBase.Logging.EventLoggerProvider.Instance.LogNewEntry += Instance_LogNewEntry;
            AuthenticationConsumerService = new AuthenticationConsumerService(SharedBase.Logging.EventLoggerProvider.Instance);
            GreetingConsumerService = new GreetingConsumerService(SharedBase.Logging.EventLoggerProvider.Instance);
            FeatureConsumerService = new FeatureConsumerService(SharedBase.Logging.EventLoggerProvider.Instance);
            HeartbeatConsumerService = new HeartbeatConsumer(SharedBase.Logging.EventLoggerProvider.Instance);
            LoggerConsumer = new LoggerConsumer();
        }

        private void Instance_LogNewEntry(object sender, SharedBase.Logging.LoggerEntry e)
        {
            if (LoggerConsumer?.IsConnect == true)
                LoggerConsumer.LogEntry(e);
        }

        public AuthenticationConsumerService AuthenticationConsumerService { get; }

        public GreetingConsumerService GreetingConsumerService { get; }

        public FeatureConsumerService FeatureConsumerService { get; }

        public HeartbeatConsumer HeartbeatConsumerService { get; }

        public LoggerConsumer LoggerConsumer { get; }

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
