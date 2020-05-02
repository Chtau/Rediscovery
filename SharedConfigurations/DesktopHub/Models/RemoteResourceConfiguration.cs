using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.DesktopHub.Models
{
    public class RemoteResourceConfiguration
    {
        public const string SectionName = "RemoteResourceSettings";

        public string ConnectionAddress { get; set; }
        public string DesktopHubApplicationKey { get; set; }
    }
}
