using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.Hub.Models
{
    public class ServiceInfoConfiguration
    {
        public const string SectionName = "ServiceInfo";

        public ushort Port { get; set; }

        public string IP { get; set; }
    }
}
