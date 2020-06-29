using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Feature
{
    public class FeatureData
    {
        public bool IsClientImplementation { get; set; }
        public string DeviceId { get; set; }
        public Guid FeatureId { get; set; }
        public string ProfileId { get; set; }
        public string Data { get; set; }
        public int NativeResourceType { get; set; } = 0;

        public FeatureData(string deviceId, Guid featureId, string profileId, string data, bool isClientImplementation = false, int nativeResourceType = 0)
        {
            NativeResourceType = nativeResourceType;
            IsClientImplementation = isClientImplementation;
            DeviceId = deviceId;
            FeatureId = featureId;
            ProfileId = profileId;
            Data = data;
        }
    }
}
