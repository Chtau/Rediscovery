using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.DesktopService.Models
{
    public class RemoteResourceConfiguration
    {
        public const string SectionName = "RemoteResourceSettings";

        public string RediscoveryManagerDeviceIdentifier { get; set; }
        public bool RediscoveryManagerAutoConnect { get; set; }
        public string RediscoveryManagerPath { get; set; }
        public string RediscoveryManagerGUIPath { get; set; }
        public string RediscoveryDiscoveryServicePath { get; set; }
    }
}
