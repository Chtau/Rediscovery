using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Configurations.Service.Models
{
    public class RoleConfiguration
    {
        public const string SectionName = "Roles";

        public string[] ResourceConsumers { get; set; }
        public string DeviceRoleName { get; set; }
        public string ResourceConsumerRoleName { get; set; }
    }
}
