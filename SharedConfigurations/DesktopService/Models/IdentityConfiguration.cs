using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.DesktopService.Models
{
    public class IdentityConfiguration
    {
        public string Secret { get; set; }

        public int PasswordKeyLength { get; set; }
    }
}
