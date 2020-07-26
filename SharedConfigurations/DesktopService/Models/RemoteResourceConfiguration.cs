using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.DesktopService.Models
{
    public class RemoteResourceConfiguration
    {
        public const string SectionName = "RemoteResourceSettings";

        public string RediscoveryDesktopHubPath { get; set; }
        public string RediscoveryDesktopHubApplicationKey { get; set; }

        public string RediscoveryDiscoveryServicePath { get; set; }

        public string RediscoveryDesktopInfoHubPath { get; set; }
        public string RediscoveryDesktopInfoHubApplicationKey { get; set; }

        public bool ShowServiceInfoOnStart { get; set; }
    }
}
