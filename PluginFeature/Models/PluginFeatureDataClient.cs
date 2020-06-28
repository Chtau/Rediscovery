using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Models
{
    public class PluginFeatureDataClient : PluginFeatureData
    {
        public Enums.ClientNativeResources NativeResourceType { get; set; } = Enums.ClientNativeResources.None;

        public PluginFeatureDataClient(string deviceId, Guid featureId, string profileId, string data, Enums.ClientNativeResources clientNativeResources) : base(deviceId, featureId, profileId, data)
        {
            NativeResourceType = clientNativeResources;
        }
    }
}
