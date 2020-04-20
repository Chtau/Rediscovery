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
    }
}
