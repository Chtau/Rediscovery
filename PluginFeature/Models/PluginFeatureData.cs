using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Models
{
    /// <summary>
    /// Used for the communication between feature UI and feature implementation
    /// </summary>
    [Obsolete("Should only be used in Plugin & Desktop Service")]
    public class PluginFeatureData
    {
        public string DeviceId { get; set; }
        public Guid FeatureId { get; set; }
        public string ProfileId { get; set; }
        public string Data { get; set; }

        public PluginFeatureData(string deviceId, Guid featureId, string profileId, string data)
        {
            DeviceId = deviceId;
            FeatureId = featureId;
            ProfileId = profileId;
            Data = data;
        }
    }
}
