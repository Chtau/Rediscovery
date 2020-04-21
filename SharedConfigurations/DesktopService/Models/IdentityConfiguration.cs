using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.DesktopService.Models
{
    public class IdentityConfiguration
    {
        public const string SectionName = "IdentitySettings";

        public string Secret { get; set; }

        public int PasswordKeyLength { get; set; }

        public bool AnonymousLogin { get; set; }
    }
}
