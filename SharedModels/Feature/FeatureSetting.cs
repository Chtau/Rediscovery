using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Feature
{
    public class FeatureSetting
    {
        public FeatureSetting()
        {

        }

        public FeatureSetting(Guid featureId, string data)
        {
            FeatureId = featureId;
            Data = data;
        }

        public Guid FeatureId { get; set; }
        public string Data { get; set; }
    }
}
