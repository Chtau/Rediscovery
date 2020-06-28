using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Feature
{
    public class FeatureData
    {
        // TODO: this entity should contain the signature of both plugin entities || or should i create a new exchange for the entities

        public string DeviceId { get; set; }
        public Guid FeatureId { get; set; }
        public string ProfileId { get; set; }
        public string Data { get; set; }

        public FeatureData(string deviceId, Guid featureId, string profileId, string data)
        {
            DeviceId = deviceId;
            FeatureId = featureId;
            ProfileId = profileId;
            Data = data;
        }
    }
}
