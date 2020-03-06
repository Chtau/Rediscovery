using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.Hub.Models
{
    public class HubConfiguration
    {
        public const string SectionName = "Hub";

        public string FirewallApp { get; set; }
    }
}
