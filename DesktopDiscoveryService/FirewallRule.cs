using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFirewallHelper;

namespace DesktopDiscoveryService
{
    public class FirewallRule
    {
        private const string DiscoveryServiceRuleName = "Rediscovery discovery Service";

        public bool DiscoveryRuleExists()
        {
            var rule = FirewallManager.Instance.Rules.FirstOrDefault(x => x.Name == DiscoveryServiceRuleName);
            if (rule.IsEnable && rule.LocalPorts.Any(x => x == DiscoveryClient.Port))
                return true;
            return false;
        }

        public bool DiscoveryRuleCreate()
        {
            // netsh advfirewall firewall add rule name="Rediscovery discovery Service" dir=in action=allow protocol=UDP localport=8888
            try
            {
                var rule = FirewallManager.Instance.CreatePortRule(FirewallProfiles.Private | FirewallProfiles.Domain,
    DiscoveryServiceRuleName,
    FirewallAction.Allow,
    DiscoveryClient.Port,
    FirewallProtocol.UDP
);
                FirewallManager.Instance.Rules.Add(rule);
                return true;
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString() + Environment.NewLine);
                return false;
            }
        }

        public bool DiscoveryRuleDelete()
        {
            // netsh advfirewall firewall delete rule name="Rediscovery discovery Service" protocol=UDP localport=8888
            try
            {
                var myRule = FirewallManager.Instance.Rules.SingleOrDefault(r => r.Name == DiscoveryServiceRuleName);
                if (myRule != null)
                {
                    FirewallManager.Instance.Rules.Remove(myRule);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString() + Environment.NewLine);
                return false;
            }
        }
    }
}
