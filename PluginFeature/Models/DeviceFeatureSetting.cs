using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Models
{
    public class DeviceFeatureSetting
    {
        public DeviceFeatureSetting()
        {

        }

        public DeviceFeatureSetting(Guid featureId, string data)
        {
            FeatureId = featureId;
            Data = data;
        }

        public Guid FeatureId { get; set; }
        public string Data { get; set; }
    }
}
