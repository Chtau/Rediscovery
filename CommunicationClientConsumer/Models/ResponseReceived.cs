using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationClientConsumer.Models
{
    public class ResponseReceived
    {
        public Guid ConfigurationId { get; }
        public Guid FeatureId { get; }
        public string ProfileId { get; }
        public object Data { get; }

        public ResponseReceived(Guid configurationId, Guid featureId, string profileId, object data)
        {
            ConfigurationId = configurationId;
            FeatureId = featureId;
            ProfileId = profileId;
            Data = data;
        }
    }
}
