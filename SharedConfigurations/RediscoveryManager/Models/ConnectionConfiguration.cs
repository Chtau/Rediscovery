using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Configurations.Manager.Models
{
    public class ConnectionConfiguration
    {
        public const string SectionName = "Connection";

        public int Port { get; set; }

        public string DeviceIdentifier { get; set; }

        public string IP { get; set; }

        public bool AutoConnect { get; set; }
    }
}
