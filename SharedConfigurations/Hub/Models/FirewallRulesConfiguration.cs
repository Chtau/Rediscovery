using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.Hub.Models
{
    public class FirewallRulesConfiguration
    {
        public const string SectionName = "FirewallRules";

        public string RuleName { get; set; }

        public string ExePath { get; set; }
    }
}
