using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Configurations.Service.Models
{
    public class IdentityConfiguration
    {
        public const string SectionName = "IdentitySettings";

        public string Secret { get; set; }

        public bool AnonymousLogin { get; set; }
    }
}
