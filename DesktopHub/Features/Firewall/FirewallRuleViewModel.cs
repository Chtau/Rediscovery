using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopHub.Features.Firewall
{
    public class FirewallRuleViewModel : BaseViewModel
    {
        string ruleName = string.Empty;
        public string RuleName
        {
            get { return ruleName; }
            set { SetProperty(ref ruleName, value); }
        }

        string exePath = string.Empty;
        public string ExePath
        {
            get { return exePath; }
            set { SetProperty(ref exePath, value); }
        }

        bool ruleSet = false;
        public bool RuleSet
        {
            get { return ruleSet; }
            set { SetProperty(ref ruleSet, value); }
        }
    }
}
