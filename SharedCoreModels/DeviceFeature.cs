using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{
    public class DeviceFeature
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string Version { get; set; }

        public string MinFeatureIntegrationPoint { get; set; }

        public string MinControlIntegrationPoint { get; set; }

        public bool HasSettings { get; set; }

        public bool HasProfiles { get; set; }

        public string Author { get; set; }

        public string Documentation { get; set; }

        public string Url { get; set; }
    }
}
