using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin.Models
{
    public class PluginFeatureSetting
    {
        public PluginFeatureSetting()
        {

        }

        public PluginFeatureSetting(Guid featureId, string data)
        {
            FeatureId = featureId;
            Data = data;
        }

        public Guid FeatureId { get; set; }
        public string Data { get; set; }
    }
}
