using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFirewallHelper;

namespace DesktopDiscoveryService
{
    public static class FirewallRule
    {
        private const string DiscoveryServiceRuleName = "Rediscovery discovery Service";

        public static bool DiscoveryRuleExists(int port)
        {
            var rule = FirewallManager.Instance.Rules.FirstOrDefault(x => x.Name == DiscoveryServiceRuleName);
            if (rule.IsEnable && rule.LocalPorts.Any(x => x == port))
                return true;
            return false;
        }

        public static bool DiscoveryRuleCreate(int port)
        {
            try
            {
                var rule = FirewallManager.Instance.CreatePortRule(FirewallProfiles.Private | FirewallProfiles.Domain,
    DiscoveryServiceRuleName,
    FirewallAction.Allow,
    (ushort)port,
    FirewallProtocol.UDP
);
                FirewallManager.Instance.Rules.Add(rule);
                return true;
            }
            catch (System.UnauthorizedAccessException)
            {
                // required admin
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString() + Environment.NewLine);
                return false;
            }
        }

        public static bool DiscoveryRuleDelete()
        {
            try
            {
                var myRule = FirewallManager.Instance.Rules.SingleOrDefault(r => r.Name == DiscoveryServiceRuleName);
                if (myRule != null)
                {
                    FirewallManager.Instance.Rules.Remove(myRule);
                }
                return true;
            }
            catch (System.UnauthorizedAccessException)
            {
                // required admin
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString() + Environment.NewLine);
                return false;
            }
        }

        public static string GetFWRuleDelete(int port)
        {
            return $"netsh advfirewall firewall delete rule name=\"{DiscoveryServiceRuleName}\" protocol=UDP localport={port}";
        }

        public static string GetFWRuleCreate(int port)
        {
            return $"netsh advfirewall firewall add rule name=\"{DiscoveryServiceRuleName}\" dir=in action=allow protocol=UDP localport={port}";
        }
    }
}
