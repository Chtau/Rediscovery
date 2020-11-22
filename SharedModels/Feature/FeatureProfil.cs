using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Feature
{
    public class FeatureProfil
    {
        public FeatureProfil()
        {

        }

        public FeatureProfil(Guid featureId, string id, string displayName, string data) : this()
        {
            FeatureId = featureId;
            Id = id;
            DisplayName = displayName;
            ProfileData = data;
        }

        public string Id { get; set; }
        public Guid FeatureId { get; set; }
        public string DisplayName { get; set; }
        public string ProfileData { get; set; }
    }
}
