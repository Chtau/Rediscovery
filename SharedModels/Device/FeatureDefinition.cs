using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Device
{
    public class FeatureDefinition
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string VersionText => Version.ToString();

        public string MinimalFeatureIntegrationPointText => MinimalFeatureIntegrationPoint.ToString();

        public string MinimalControlIntegrationPointText => MinimalControlIntegrationPoint.ToString();

        public Core.Version Version { get; set; }

        public Core.Version MinimalFeatureIntegrationPoint { get; set; }

        public Core.Version MinimalControlIntegrationPoint { get; set; }
    }
}
