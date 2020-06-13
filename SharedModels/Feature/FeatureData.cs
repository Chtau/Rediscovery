using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Feature
{
    public class FeatureData
    {
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
