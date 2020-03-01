using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.DesktopService.Models
{
    public class PipeConfiguration
    {
        public const string SectionName = "PipeSettings";

        public string RediscoveryDesktopHubPath { get; set; }

        public string RediscoveryDiscoveryService { get; set; }

        public bool ShowServiceInfoOnStart { get; set; }
    }
}
