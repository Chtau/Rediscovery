using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Configurations.DesktopHub.Models
{
    public class RemoteResourceConfiguration
    {
        public const string SectionName = "RemoteResourceSettings";

        public string IP { get; set; }
        public int? Port { get; set; }
        public string DesktopHubApplicationKey { get; set; }
    }
}
